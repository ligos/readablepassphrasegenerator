using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;
using MurrayGrant.ReadablePassphrase.Dictionaries;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MurrayGrant.WordScraper
{
    internal class DictionaryComScraper
    {
        public CommandLineArguments Args { get; }
        public HttpClient HttpClient { get; }
        public CancellationToken CancellationToken { get; }
        public Action<int, int> ReportProgress { get; }
        public Action<string> ReportMessage { get; }

        public DictionaryComScraper(CommandLineArguments args, HttpClient httpClient, CancellationToken cancellationToken, Action<string> reportMessage, Action<int, int> reportProgress)
        {
            this.Args = args;
            this.HttpClient = httpClient;
            this.CancellationToken = cancellationToken;
            this.ReportMessage = reportMessage;
            this.ReportProgress = reportProgress;
        }

        internal async Task<IReadOnlyList<(string wordRoot, PartOfSpeech pos)>> ReadWords(WordDictionary dictionary, IReadOnlySet<string> uniqueRoots, IReadOnlySet<string> uniqueForms)
        {
            var result = new List<(string, PartOfSpeech)>(Args.WordCount);
            var rootWordCount = 0;
            var attemptCounter = 0;
            var (wordLength, startsWith, pageNumber) = ParseResumeArg();
            var first = true;

            var nonceBytes = new byte[5];
            new Random().NextBytes(nonceBytes);
            var nonce = BitConverter.ToString(nonceBytes).Replace("-", "").ToLowerInvariant();
            
            ReportMessage($"Scraping {wordLength} letter words, starting with '{startsWith}', from page {pageNumber}...");

            // From min to max length of words.
            while (wordLength <= Args.MaxLength && rootWordCount < Args.WordCount)
            {
                // Words starting from a..z
                while (startsWith <= 'z' && rootWordCount < Args.WordCount)
                {
                    // Load a page of words.
                    while (pageNumber < Int32.MaxValue && rootWordCount < Args.WordCount)
                    {
                        CheckCancellationToken();
                        var pageUrl = $"https://www.dictionary.com/e/crb-ajax/cached.php?page={pageNumber}&wordLength={wordLength}&letter={startsWith}&action=get_wf_widget_page&pageType=4&nonce={nonce}";
                        var response = await HttpClient.GetStringAsync(pageUrl);
                        var errorResponseObj = (JObject?)JsonConvert.DeserializeObject<dynamic>(response);
                        if (errorResponseObj == null)
                        {
                            ReportMessage($"Error: Request to '{pageUrl}' returned null.");
                            return result;
                        }

                        // Response uses a type union on 'data' property, which we need to inspect carefully.
                        if (!errorResponseObj.TryGetValue("success", out var successToken)
                            || !errorResponseObj.TryGetValue("data", out var dataToken))
                        {
                            ReportMessage($"Error: Request to '{pageUrl}' returned unexpected data.");
                            return result;
                        }

                        if (successToken.Type == JTokenType.Boolean 
                            && successToken.Value<bool>() == false 
                            && dataToken.Type == JTokenType.String
                            && dataToken.Value<string>() == "There is a problem with the next page information")
                        {
                            // There is no next page: start next letter.
                            break;
                        }
                        else if (successToken.Type == JTokenType.Boolean && successToken.Value<bool>() == false)
                        {
                            ReportMessage($"Error: Request to '{pageUrl}' was not successful: {dataToken.Value<object>()}");
                            return result;
                        }

                        if (!first)
                            ReportMessage($"Scraping {wordLength} letter words, starting with '{startsWith}', from page {pageNumber}...");

                        // Scrape each word.
                        var pageOfWords = JsonConvert.DeserializeObject<PageDataWithWords>(response) ?? new PageDataWithWords();
                        foreach (var w in pageOfWords.data?.words ?? Enumerable.Empty<string>())
                        {
                            // Various reasons to exclude this word:
                            if (uniqueForms.Contains(w))
                                goto ReportProgressAndNext;

                            CheckCancellationToken();
                            ++attemptCounter;
                            first = false;

                            // Load HTML and scrape word + parts of speech.
                            var definitionHtml = await HttpClient.GetStringAsync($"https://www.dictionary.com/browse/{w}");
                            var htmlDoc = new HtmlDocument();
                            htmlDoc.LoadHtml(definitionHtml);
                            var rootSection = htmlDoc.DocumentNode.QuerySelector("#top-definitions-section");
                            var wordRoot = rootSection.QuerySelector("h1[data-first-headword]")?.InnerText ?? "";

                            // Various reasons to exclude this word:
                            if (uniqueRoots.Contains(w))
                                goto ReportProgressAndNext;

                            bool atLeastOneValidPartOfSpeech = false;
                            foreach (var posNode in rootSection.ParentNode.QuerySelectorAll("span.luna-pos"))
                            {
                                var partsOfSpeech = (posNode.InnerText ?? "").Split(',').Select(x => x.Trim()).Where(x => !String.IsNullOrEmpty(x));
                                foreach (var partOfSpeech in partsOfSpeech)
                                {
                                    var pos = ParsePartOfSpeech(partOfSpeech);
                                    if (pos == PartOfSpeech.Unknown)
                                    {
                                        ReportMessage($"Warning: Unknown part of speech: '{partOfSpeech}' for '{w}'.");
                                        continue;
                                    }

                                    if (pos.IsSupported())
                                    {
                                        result.Add((w, pos));
                                        atLeastOneValidPartOfSpeech = true;
                                    }
                                }
                            }
                            if (atLeastOneValidPartOfSpeech)
                                ++rootWordCount;

ReportProgressAndNext:
                            ReportProgress(rootWordCount, attemptCounter);
                            if (rootWordCount >= Args.WordCount)
                                break;
                            if (Args.DelayMs > 0)
                                await Task.Delay(Args.DelayMs);
                        }

                        ++pageNumber;
                    }

                    pageNumber = 1;
                    ++startsWith;
                }

                pageNumber = 1;
                startsWith = 'a';
                ++wordLength;
            }

            return result;

            void CheckCancellationToken()
            {
                if (CancellationToken.IsCancellationRequested)
                {
                    var resumeCode = $"{wordLength}_{startsWith}_{pageNumber}";
                    ReportMessage($"Resume code: {resumeCode}");
                    CancellationToken.ThrowIfCancellationRequested();
                }
            }
        }

        (int wordLength, char startsWith, int pageNumber) ParseResumeArg()
        {
            if (String.IsNullOrEmpty(Args.ResumeFrom))
                return (Args.MinLength, 'a', 1);
            var parts = Args.ResumeFrom.Split('_');
            if (parts.Length != 3)
            {
                ReportMessage($"Unable to parse resume from '{Args.ResumeFrom}': expected 3 parts, but found {parts.Length}.");
                return (Args.MinLength, 'a', 1);
            }
            if (!Int32.TryParse(parts[0], out var wordLength))
            {
                ReportMessage($"Unable to parse resume from '{Args.ResumeFrom}': expected 1st part to be integer.");
                return (Args.MinLength, 'a', 1);
            }
            if (parts[1].Length != 1)
            {
                ReportMessage($"Unable to parse resume from '{Args.ResumeFrom}': expected 2rd part to be single character.");
                return (Args.MinLength, 'a', 1);
            }
            if (!Int32.TryParse(parts[2], out var pageNumber))
            {
                ReportMessage($"Unable to parse resume from '{Args.ResumeFrom}': expected 3rd part to be integer.");
                return (Args.MinLength, 'a', 1);
            }
            return (wordLength, parts[1].ToLowerInvariant().First(), pageNumber);
        }

        PartOfSpeech ParsePartOfSpeech(string partOfSpeech)
            => string.Equals(partOfSpeech, "noun",                               StringComparison.OrdinalIgnoreCase) ? PartOfSpeech.Noun
            :  string.Equals(partOfSpeech, "plural noun",                        StringComparison.OrdinalIgnoreCase) ? PartOfSpeech.NounPlural
            :  string.Equals(partOfSpeech, "verb",                               StringComparison.OrdinalIgnoreCase) ? PartOfSpeech.VerbTransitive
            :  string.Equals(partOfSpeech, "verb (used without object)",         StringComparison.OrdinalIgnoreCase) ? PartOfSpeech.VerbIntransitive
            :  string.Equals(partOfSpeech, "verb (used with object)",            StringComparison.OrdinalIgnoreCase) ? PartOfSpeech.VerbTransitive
            :  string.Equals(partOfSpeech, "verb (used with or without object)", StringComparison.OrdinalIgnoreCase) ? PartOfSpeech.VerbTransitive
            :  string.Equals(partOfSpeech, "auxiliary verb",                     StringComparison.OrdinalIgnoreCase) ? PartOfSpeech.VerbOther
            :  string.Equals(partOfSpeech, "verb Phrases",                       StringComparison.OrdinalIgnoreCase) ? PartOfSpeech.VerbOther
            :  string.Equals(partOfSpeech, "adjective",                          StringComparison.OrdinalIgnoreCase) ? PartOfSpeech.Adjective
            :  string.Equals(partOfSpeech, "adverb",                             StringComparison.OrdinalIgnoreCase) ? PartOfSpeech.Adverb
            :  string.Equals(partOfSpeech, "interjection",                       StringComparison.OrdinalIgnoreCase) ? PartOfSpeech.Interjection
            :  string.Equals(partOfSpeech, "abbreviation",                       StringComparison.OrdinalIgnoreCase) ? PartOfSpeech.Abbreviation
            :  string.Equals(partOfSpeech, "conjunction",                        StringComparison.OrdinalIgnoreCase) ? PartOfSpeech.Conjunction
            :  string.Equals(partOfSpeech, "preposition",                        StringComparison.OrdinalIgnoreCase) ? PartOfSpeech.Preposition
            :  string.Equals(partOfSpeech, "pronoun",                            StringComparison.OrdinalIgnoreCase) ? PartOfSpeech.Pronoun
            :  PartOfSpeech.Unknown;

        class PageDataWithWords
        {
            public bool success { get; set; }
            public WordData? data { get; set; }
        }
        class WordData
        {
            public List<string>? words { get; set; } = new List<string>();
        }
    }
}
