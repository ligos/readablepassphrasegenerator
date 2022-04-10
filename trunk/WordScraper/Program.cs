// Copyright 2022 Murray Grant & drventure
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// Based on following pull request
// https://github.com/ligos/readablepassphrasegenerator/pull/9

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace MurrayGrant.WordScraper
{
    public class Program
    {
        // This is a bit of a cheats way of doing command line arguments. Please don't consider it good practice!
        static int WordCount = 10;
        static int MinLength = 3;
        static int MaxLength = 10;
        static int Attempts = 1000;
        static string Source = "";
        static int DelayMs = 250;
        static int ShowCount = 10;

        static HashSet<string> SupportedSources = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase)
        {
            "ThisWordDoesNotExist.com"
        };

        static Encoding Utf8WithoutBOM = new UTF8Encoding(false);
        static CancellationTokenSource CancellationSource = new CancellationTokenSource();
        static DateTime NextProgressUpdate = DateTime.MinValue;

        public async static Task Main(string[] args)
        {
            try
            {
                Console.OutputEncoding = Encoding.UTF8;
                if (!ParseCommandLine(args))
                {
                    PrintUsage();
                    Environment.Exit(1);
                }

                Console.CancelKeyPress += Console_CancelKeyPress;
                await RunMain();
                Environment.Exit(0);
            }
            catch (OperationCanceledException)
            {
                Environment.Exit(1);
            }
            catch (Exception ex)
            {
                var originalColour = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(ex);
                Console.ForegroundColor = originalColour;
                Environment.Exit(2);
            }
        }

        private static void Console_CancelKeyPress(object? sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            CancellationSource.Cancel();
            Console.WriteLine("Received CTRL+C, stopping...");
        }

        async static Task RunMain()
        {
            // Show what we're about to do.
            Console.WriteLine("WordScraper Utility for ReadablePhrase Generator");
            Console.WriteLine("Pulls AI generated words from thisworddoesnotexist.com and conjugates them.");
            Console.WriteLine("drventure 2020 & Murray Grant 2022");
            Console.WriteLine();
            Console.WriteLine($"Scraping {WordCount:N0} words from {Source}...");
            Console.WriteLine($"Must be between {MinLength:N0} and {MaxLength:N0} characters.");
            CancellationSource.Token.ThrowIfCancellationRequested();

            // Load current dictionary, so we can avoid duplicates.
            var defaultDictionary = ReadablePassphrase.Dictionaries.Default.Load();
            var allUniqueForms = defaultDictionary.SelectMany(w => w.AllForms()).ToHashSet(StringComparer.CurrentCultureIgnoreCase);
            Console.WriteLine($"Default dictionary contains {defaultDictionary.Count:N0} words and {allUniqueForms.Count:N0} unique forms.");
            CancellationSource.Token.ThrowIfCancellationRequested();

            // Let the scraping begin!
            Console.WriteLine("Starting scraping...");
            NextProgressUpdate = DateTime.UtcNow.AddSeconds(1);
            var sw = Stopwatch.StartNew();
            var scrapedWords = await ReadWords(allUniqueForms);
            sw.Stop();
            Console.WriteLine($"Scraping complete. {scrapedWords.Count} words found in {sw.Elapsed.TotalSeconds:N1} seconds");

            // Show samples.
            if (ShowCount > 0) 
                ShowWords(scrapedWords);

            // And save as compatible XML files.
            Console.WriteLine("Writing to XML...");
            await SaveWords(scrapedWords);
            Console.WriteLine("Finished writing to XML.");
        }

        private static Task<IReadOnlyList<(string wordRoot, string partOfSpeech)>> ReadWords(IReadOnlySet<string> uniqueForms)
        {
            switch (Source.ToLower())
            {
                case "thisworddoesnotexist.com":
                    return ReadWordsFromThisWordDoesNotExist(uniqueForms);
                default:
                    throw new ApplicationException("Unknown source: " + Source);
            }
        }

        private static async Task<IReadOnlyList<(string wordRoot, string partOfSpeech)>> ReadWordsFromThisWordDoesNotExist(IReadOnlySet<string> uniqueForms)
        {
            var result = new List<(string, string)>(WordCount);
            var httpClient = CreateHttpClient();
            var attemptCounter = 0;
            var uniqueFormsFromThisRun = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);

            while (result.Count < WordCount && attemptCounter < Attempts)
            {
                CancellationSource.Token.ThrowIfCancellationRequested();

                // Download webpage.
                var partOfSpeech = "";
                var wordRoot = "";
                var page = await httpClient.GetStringAsync("https://www.thisworddoesnotexist.com");
                ++attemptCounter;

                // Parse
                var mark = "class=\"pos\">";
                var posPos = page.IndexOf(mark, 0, StringComparison.CurrentCultureIgnoreCase);
                if (posPos > 0)
                {
                    var endMark = "</div>";
                    var posEnd = page.IndexOf(endMark, posPos);
                    if (posEnd > 0)
                    {
                        partOfSpeech = page.Substring(posPos + mark.Length, posEnd - posPos - mark.Length);
                        partOfSpeech = partOfSpeech.Replace(".", "");
                        posEnd = partOfSpeech.IndexOf("[");
                        if (posEnd > 0) 
                            partOfSpeech = partOfSpeech.Substring(0, posEnd - 1);
                        partOfSpeech = partOfSpeech.Trim();
                    }
                }

                mark = "class=\"word\">";
                var posWord = page.IndexOf(mark);
                if (posWord > 0)
                {
                    var endMark = "</div>";
                    var posEnd = page.IndexOf(endMark, posWord);
                    if (posEnd > 0)
                    {
                        wordRoot = page.Substring(posWord + mark.Length, posEnd - posWord - mark.Length);
                        wordRoot = wordRoot.Replace(".", "");
                        wordRoot = wordRoot.Trim();
                    }
                }

                // Various reasons to exclude this word:
                if (string.IsNullOrWhiteSpace(wordRoot) || string.IsNullOrWhiteSpace(partOfSpeech))
                    goto ReportProgressAndNext;

                if (uniqueForms.Contains(wordRoot))
                    goto ReportProgressAndNext;

                if (wordRoot.Length < MinLength || wordRoot.Length > MaxLength)
                    goto ReportProgressAndNext;
                
                if (!uniqueFormsFromThisRun.Add(wordRoot))
                    goto ReportProgressAndNext;

                // This one is OK!
                result.Add((wordRoot, partOfSpeech.ToLowerInvariant()));

ReportProgressAndNext:
                ReportProgress(result.Count, attemptCounter);
                if (DelayMs > 0)
                    await Task.Delay(DelayMs);
            }

            return result;
        }

        private static void ShowWords(IReadOnlyList<(string wordRoot, string partOfSpeech)> words)
        {
            var random = new Random();
            var randomisedWords = words
                .OrderBy(x => random.NextDouble())
                .Take(ShowCount);

            Console.WriteLine();
            Console.WriteLine("Sample of Scraped Words:");
            foreach (var word in randomisedWords.OrderBy(x => x.wordRoot))
            {
                Console.WriteLine("  {0} ({1})", word.wordRoot, word.partOfSpeech);
            }
            Console.WriteLine();
        }

        private static async Task SaveWords(IReadOnlyList<(string wordRoot, string partOfSpeech)> words)
        {
            var wordsByPartOfSpeech = words.ToLookup(x => x.partOfSpeech, x => x.wordRoot);

            CheckForUnsupportedPartsOfSpeech(wordsByPartOfSpeech);

            var nouns = wordsByPartOfSpeech.FirstOrDefault(g => g.Key == "noun") ?? Enumerable.Empty<string>();
            var pluralNouns = wordsByPartOfSpeech.FirstOrDefault(g => g.Key == "plural noun") ?? Enumerable.Empty<string>();
            await SaveNouns(nouns, pluralNouns);

            var adjectives = wordsByPartOfSpeech.FirstOrDefault(g => g.Key == "adjective") ?? Enumerable.Empty<string>();
            await SaveAdjectives(adjectives);

            var adverbs = wordsByPartOfSpeech.FirstOrDefault(g => g.Key == "adverb") ?? Enumerable.Empty<string>();
            await SaveAdverbs(adverbs);

            var verbs = wordsByPartOfSpeech.FirstOrDefault(g => g.Key == "verb") ?? Enumerable.Empty<string>();
            await SaveVerbs(verbs);
        }

        private static void CheckForUnsupportedPartsOfSpeech(ILookup<string, string> wordsByPartOfSpeech)
        {
            var supportedPartsOfSpeech = new[]
{
                "verb",
                "noun",
                "plural noun",
                "adjective",
                "adverb",
            }; 
            
            var unsupportedWords = wordsByPartOfSpeech.Where(x => !supportedPartsOfSpeech.Contains(x.Key));
            if (unsupportedWords.Any())
            {
                Console.WriteLine("WARNING: unsupported parts of speech found in scraped words.");
                foreach (var g in unsupportedWords)
                {
                    Console.WriteLine("  " + g.Key);
                    foreach (var w in g.OrderBy(w => w))
                    {
                        Console.WriteLine("    " + w);
                    }
                }
            }
        }

        private static async Task SaveNouns(IEnumerable<string> nouns, IEnumerable<string> pluralNouns)
        {
            if (!nouns.Any() && !pluralNouns.Any())
            {
                File.Delete("ScrapedNouns.xml");
                return;
            }

            using (var stream = new FileStream("ScrapedNouns.xml", FileMode.Create, FileAccess.Write, FileShare.None, 16*1024, true))
            using (var writer = new StreamWriter(stream, Utf8WithoutBOM))
            {
                await writer.WriteLineAsync("<dictionary>");
                
                foreach (var noun in nouns)
                {
                    var plural = noun;
                    if (noun.EndsWith("man"))
                        plural = noun.Substring(0, noun.Length - 3) + "men";
                    if (noun.EndsWith("ch"))
                        plural = noun.Substring(0, noun.Length - 2) + "ches";
                    else if (noun.EndsWith("sh"))
                        plural = noun;
                    else if (noun.EndsWith("y"))
                        plural = noun.Substring(0, noun.Length - 1) + "ies";
                    else if (noun.EndsWith("u"))
                        plural = noun.Substring(0, noun.Length - 1) + "s";
                    else if (noun.EndsWith("o") || noun.EndsWith("x"))
                        plural = noun + "es";
                    else if (noun.EndsWith("a") || noun.EndsWith("e") || noun.EndsWith("i"))
                        plural = noun + "s";
                    else if (!noun.EndsWith("s"))
                        plural = noun + "s";

                    await writer.WriteLineAsync($"  <noun singular=\"{EncodeForXml(noun)}\"	plural=\"{EncodeForXml(plural)}\" />");
                }

                foreach (var plural in pluralNouns)
                {
                    var xmlSafePlural = EncodeForXml(plural);
                    await writer.WriteLineAsync($"  <noun plural=\"{xmlSafePlural}\" />");
                }

                await writer.WriteLineAsync("</dictionary>");
            }
        }

        private static async Task SaveAdjectives(IEnumerable<string> adjectives)
        {
            if (!adjectives.Any())
            {
                File.Delete("ScrapedAdjectives.xml");
                return;
            }

            using (var stream = new FileStream("ScrapedAdjectives.xml", FileMode.Create, FileAccess.Write, FileShare.None, 16 * 1024, true))
            using (var writer = new StreamWriter(stream, Utf8WithoutBOM))
            {
                await writer.WriteLineAsync("<dictionary>");

                foreach (var adjective in adjectives)
                {
                    await writer.WriteLineAsync($"  <adjective value=\"{EncodeForXml(adjective)}\" />");
                }
                await writer.WriteLineAsync("</dictionary>");
            }
        }

        private static async Task SaveAdverbs(IEnumerable<string> adverbs)
        {
            if (!adverbs.Any())
            {
                File.Delete("ScrapedAdverbs.xml");
                return;
            }

            using (var stream = new FileStream("ScrapedAdverbs.xml", FileMode.Create, FileAccess.Write, FileShare.None, 16 * 1024, true))
            using (var writer = new StreamWriter(stream, Utf8WithoutBOM))
            {
                await writer.WriteLineAsync("<dictionary>");

                foreach (var adverb in adverbs)
                {
                    await writer.WriteLineAsync($"  <adverb value=\"{EncodeForXml(adverb)}\" />");
                }
                await writer.WriteLineAsync("</dictionary>");
            }
        }

        private static async Task SaveVerbs(IEnumerable<string> verbs)
        {
            if (!verbs.Any())
            {
                File.Delete("ScrapedVerbs.xml");
                return;
            }

            using (var stream = new FileStream("ScrapedVerbs.xml", FileMode.Create, FileAccess.Write, FileShare.None, 16 * 1024, true))
            using (var writer = new StreamWriter(stream, Utf8WithoutBOM))
            {
                await writer.WriteLineAsync("<dictionary>");

                foreach (var verb in verbs)
                {
                    var toES = verb.EndsWith("s") ? $"{verb}es" : $"{verb}s";
                    toES = verb.EndsWith("x") ? $"{verb}es" : toES;
                    toES = verb.EndsWith("e") ? $"{verb}s" : toES;
                    toES = verb.EndsWith("y") ? $"{verb.Substring(0, verb.Length - 1)}ies" : toES;
                    toES = verb.EndsWith("ch") || verb.EndsWith("sh") ? $"{verb}es" : toES;

                    var toED = verb.EndsWith("e") ? $"{verb}d" : $"{verb}ed";
                    toED = verb.EndsWith("y") ? $"{verb.Substring(0, verb.Length - 1)}ied" : toED;

                    var toING = verb.EndsWith("e") ? $"{verb.Substring(0, verb.Length - 1)}ing" : $"{verb}ing";

                    var line = $@"  <verb presentSingular=""{toES}"" pastSingular=""{toED}"" pastContinuousSingular=""was {toING}"" futureSingular=""will {verb}"" continuousSingular=""is {toING}"" perfectSingular=""has {toED}"" subjunctiveSingular=""might {verb}""
        presentPlural = ""{verb}"" pastPlural = ""{toED}"" pastContinuousPlural = ""were {toING}"" futurePlural = ""will {verb}"" continuousPlural = ""are {toING}"" perfectPlural = ""have {toED}"" subjunctivePlural = ""might {verb}"" />";
                    await writer.WriteLineAsync(line);
                }
                
                await writer.WriteLineAsync("</dictionary>");
            }
        }

        private static HttpClient CreateHttpClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "makemeapassword.ligos.net (Batch Process)");
            return client;
        }

        private static string EncodeForXml(string xml)
        {
            if (string.IsNullOrEmpty(xml))
                return xml;
            var result = xml.Replace("\"", "&quot;")
                            .Replace("&", "&amp;")
                            .Replace("'", "&apos;")
                            .Replace("<", "&lt;")
                            .Replace(">", "&gt;");
            return result;
        }

        static void ReportProgress(int wordCount, int attempts)
        {
            if (DateTime.UtcNow < NextProgressUpdate)
                return;

            Console.WriteLine($"[{attempts:N0} of {Attempts:N0} attempts, {wordCount:N0} words scraped successfully]");
            NextProgressUpdate = DateTime.UtcNow.AddSeconds(2);
        }

        static bool ParseCommandLine(string[] args)
        {
            if (args.Length == 0)
            {
                PrintUsage();
                Environment.Exit(0);
            }

            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i].ToLower().Trim();
                if (arg.StartsWith("-") || arg.StartsWith("--") || arg.StartsWith("/"))
                    arg = arg.Replace("--", "").Replace("-", "").Replace("/", "");

                if (i == 0)
                {
                    if (!SupportedSources.Contains(arg))
                    {
                        Console.WriteLine("Source '{0}' is not supported. Supported sources:", arg);
                        foreach (var source in SupportedSources.OrderBy(x => x))
                        {
                            Console.WriteLine("  " + source);
                        }
                        return false;
                    }
                    Source = arg;
                }
                else if (arg == "c" || arg == "count")
                {
                    if (!Int32.TryParse(args[i + 1].Trim(), out WordCount))
                    {
                        Console.WriteLine("Unable to parse number '{0}' for 'count' option.", args[i + 1]);
                        return false;
                    }
                    i++;
                }
                else if (arg == "a" || arg == "attempts")
                {
                    if (!Int32.TryParse(args[i + 1].Trim(), out Attempts))
                    {
                        Console.WriteLine("Unable to parse number '{0}' for 'attempts' option.", args[i + 1]);
                        return false;
                    }
                    i++;
                }
                else if (arg == "min")
                {
                    if (!Int32.TryParse(args[i + 1].Trim(), out MinLength))
                    {
                        Console.WriteLine("Unable to parse number '{0}' for 'min' option.", args[i + 1]);
                        return false;
                    }
                    i++;
                }
                else if (arg == "max")
                {
                    if (!Int32.TryParse(args[i + 1].Trim(), out MaxLength))
                    {
                        Console.WriteLine("Unable to parse number '{0}' for 'max' option.", args[i + 1]);
                        return false;
                    }
                    i++;
                }
                else if (arg == "d" || arg == "delayms")
                {
                    if (!Int32.TryParse(args[i + 1].Trim(), out DelayMs))
                    {
                        Console.WriteLine("Unable to parse number '{0}' for 'delayMs' option.", args[i + 1]);
                        return false;
                    }
                    i++;
                }
                else if (arg == "d" || arg == "show")
                {
                    if (!Int32.TryParse(args[i + 1].Trim(), out ShowCount))
                    {
                        Console.WriteLine("Unable to parse number '{0}' for 'show' option.", args[i + 1]);
                        return false;
                    }
                    i++;
                }
                else if (arg == "h" || arg == "help")
                {
                    PrintUsage();
                    Environment.Exit(0);
                }
                else
                {
                    Console.WriteLine("Unknown argument '{0}'.", arg);
                    return false;
                }
            }

            return true;
        }

        static void PrintUsage()
        {
            Console.WriteLine("Usage: WordScraper.exe source [options]");
            Console.WriteLine("  -c --count nnn        Scrapes nnn words (default: {0})", WordCount);
            Console.WriteLine("  --min xxx             Specifies a minimum length for words (def: {0})", MinLength);
            Console.WriteLine("  --max xxx             Specifies a maximum length for words (def: {0})", MaxLength);
            Console.WriteLine("  -a --attempts nnn     Maximum attempts to scrape (default: {0})", Attempts);
            Console.WriteLine("  -d --delayMs nnn      Milliseconds of delay after each attempt (default: {0})", DelayMs);
            Console.WriteLine("  --show nnn            Show sample list of words after scraping (default: {0})", ShowCount);
            Console.WriteLine();
            Console.WriteLine("  Supported sources:");
            foreach (var source in SupportedSources.OrderBy(x => x))
            {
                Console.WriteLine("    " + source);
            }
            Console.WriteLine();
            Console.WriteLine("  -h --help             Displays this message ");
            Console.WriteLine("See {0} for more information", ReadablePassphrase.ReadablePassphraseGenerator.GitHubHomepage);
            Console.WriteLine("Contact Murray via GitHub or at " + ReadablePassphrase.ReadablePassphraseGenerator.KeyBaseContact);
        }
    }
}