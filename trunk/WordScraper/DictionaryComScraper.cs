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

        internal async Task<IReadOnlyList<(string wordRoot, string partOfSpeech)>> ReadWords(WordDictionary dictionary, IReadOnlySet<string> uniqueRoots, IReadOnlySet<string> uniqueForms)
        {
            var result = new List<(string, string)>(Args.WordCount);
            var attemptCounter = 0;
            var uniqueFormsFromThisRun = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);
            var (wordLength, startsWith, pageNumber) = ParseResumeArg();
            
            var nonceBytes = new byte[5];
            new Random().NextBytes(nonceBytes);
            var nonce = BitConverter.ToString(nonceBytes).Replace("-", "").ToLowerInvariant();

            // From min to max length of words.
            while (wordLength <= Args.MaxLength)
            {
                // Words starting from a..z
                while (startsWith <= 'z')
                {
                    // Load a page of words.
                    while (pageNumber < Int32.MaxValue)
                    {
                        ReportMessage($"Scraping {wordLength} letter words, starting with '{startsWith}', from page {pageNumber}...");

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

                        // Scrape each word.
                        var pageOfWords = JsonConvert.DeserializeObject<PageDataWithWords>(response) ?? new PageDataWithWords();
                        foreach (var w in pageOfWords.data?.words ?? Enumerable.Empty<string>())
                        {
                            CheckCancellationToken();
                            ++attemptCounter;
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
