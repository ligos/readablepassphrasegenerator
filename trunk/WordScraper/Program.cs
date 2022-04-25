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

using MurrayGrant.ReadablePassphrase.Dictionaries;
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
        static Encoding Utf8WithoutBOM = new UTF8Encoding(false);
        static CancellationTokenSource CancellationSource = new CancellationTokenSource();
        static DateTime NextProgressUpdate = DateTime.MinValue;
        static CommandLineArguments Args = new CommandLineArguments();

        public async static Task Main(string[] args)
        {
            try
            {
                Console.OutputEncoding = Encoding.UTF8;
                var (commandArgs, exitCode) = CommandLineArguments.Parse(args);
                if (commandArgs == null)
                {
                    CommandLineArguments.PrintUsage();
                    Environment.Exit(exitCode ?? 1);
                }
                Args = commandArgs;

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
            Console.WriteLine($"Scraping {Args.WordCount:N0} words from {Args.Source}...");
            Console.WriteLine($"Must be between {Args.MinLength:N0} and {Args.MaxLength:N0} characters.");
            var sourceDef = CommandLineArguments.SupportedSources[Args.Source];
            CancellationSource.Token.ThrowIfCancellationRequested();

            // Load current dictionary, so we can avoid duplicates.
            var defaultDictionary = ReadablePassphrase.Dictionaries.Default.Load();
            var allUniqueForms = defaultDictionary.SelectMany(w => w.AllForms()).ToHashSet(StringComparer.CurrentCultureIgnoreCase);
            var allUniqueRoots = defaultDictionary.Select(w => w.DictionaryEntry).ToHashSet(StringComparer.CurrentCultureIgnoreCase);
            Console.WriteLine($"Default dictionary contains {defaultDictionary.Count:N0} words, {allUniqueRoots.Count:N0} unique roots and {allUniqueForms.Count:N0} unique forms.");
            CancellationSource.Token.ThrowIfCancellationRequested();

            // Let the scraping begin!
            Console.WriteLine("Starting scraping...");
            NextProgressUpdate = DateTime.UtcNow.AddSeconds(1);
            var sw = Stopwatch.StartNew();
            var scrapedWords = await ReadWords(defaultDictionary, allUniqueRoots, allUniqueForms);
            sw.Stop();
            Console.WriteLine($"Scraping complete. {scrapedWords.Count} words found in {sw.Elapsed.TotalSeconds:N1} seconds");

            // Show samples.
            if (Args.ShowCount > 0) 
                ShowWords(scrapedWords);

            // And save as compatible XML files.
            Console.WriteLine("Writing to XML...");
            await SaveWords(scrapedWords, sourceDef);
            Console.WriteLine("Finished writing to XML.");
        }

        private static Task<IReadOnlyList<(string wordRoot, PartOfSpeech pos)>> ReadWords(WordDictionary dictionary, IReadOnlySet<string> uniqueRoots, IReadOnlySet<string> uniqueForms)
        {
            var httpClient = CreateHttpClient();
            switch (Args.Source.ToLower())
            {
                case "thisworddoesnotexist.com":
                    return new ThisWordDoesNotExistScraper(Args, httpClient, CancellationSource.Token, ReportMessage, ReportProgress).ReadWords(uniqueForms);
                case "dictionary.com":
                    return new DictionaryComScraper(Args, httpClient, CancellationSource.Token, ReportMessage, ReportProgress).ReadWords(dictionary, uniqueRoots, uniqueForms);
                default:
                    throw new ApplicationException("Unknown source: " + Args.Source);
            }
        }

        private static void ShowWords(IReadOnlyList<(string wordRoot, PartOfSpeech pos)> words)
        {
            var random = new Random();
            var randomisedWords = words
                .OrderBy(x => random.NextDouble())
                .Take(Args.ShowCount);

            Console.WriteLine();
            Console.WriteLine("Sample of Scraped Words:");
            foreach (var word in randomisedWords.OrderBy(x => x.wordRoot))
            {
                Console.WriteLine("  {0} ({1})", word.wordRoot, word.pos);
            }
            Console.WriteLine();
        }

        private static async Task SaveWords(IReadOnlyList<(string wordRoot, PartOfSpeech pos)> words, CommandLineArguments.SourceDefinition sourceDef)
        {
            var wordsByPartOfSpeech = words.ToLookup(x => x.pos, x => x.wordRoot);

            var nouns = wordsByPartOfSpeech.FirstOrDefault(g => g.Key == PartOfSpeech.Noun) ?? Enumerable.Empty<string>();
            var pluralNouns = wordsByPartOfSpeech.FirstOrDefault(g => g.Key == PartOfSpeech.NounPlural) ?? Enumerable.Empty<string>();
            await SaveNouns(nouns, pluralNouns, sourceDef);
            CancellationSource.Token.ThrowIfCancellationRequested();

            var adjectives = wordsByPartOfSpeech.FirstOrDefault(g => g.Key == PartOfSpeech.Adjective) ?? Enumerable.Empty<string>();
            await SaveAdjectives(adjectives, sourceDef);
            CancellationSource.Token.ThrowIfCancellationRequested();

            var adverbs = wordsByPartOfSpeech.FirstOrDefault(g => g.Key == PartOfSpeech.Adverb) ?? Enumerable.Empty<string>();
            await SaveAdverbs(adverbs, sourceDef);
            CancellationSource.Token.ThrowIfCancellationRequested();

            var transitiveVerbs = wordsByPartOfSpeech.FirstOrDefault(g => g.Key == PartOfSpeech.VerbTransitive) ?? Enumerable.Empty<string>();
            var intransitiveVerbs = wordsByPartOfSpeech.FirstOrDefault(g => g.Key == PartOfSpeech.VerbIntransitive) ?? Enumerable.Empty<string>();
            await SaveVerbs(transitiveVerbs, intransitiveVerbs, sourceDef);
            CancellationSource.Token.ThrowIfCancellationRequested();
        }

        private static async Task SaveNouns(IEnumerable<string> nouns, IEnumerable<string> pluralNouns, CommandLineArguments.SourceDefinition sourceDef)
        {
            if (!nouns.Any() && !pluralNouns.Any())
            {
                File.Delete("ScrapedNouns.xml");
                return;
            }

            var tags = XmlTagsAttribute(sourceDef);
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

                    await writer.WriteLineAsync($"  <noun singular=\"{EncodeForXml(noun)}\"	plural=\"{EncodeForXml(plural)}\" {tags}/>");
                }

                foreach (var plural in pluralNouns)
                {
                    var xmlSafePlural = EncodeForXml(plural);
                    await writer.WriteLineAsync($"  <noun plural=\"{xmlSafePlural}\" {tags}/>");
                }

                await writer.WriteLineAsync("</dictionary>");
            }
        }

        private static async Task SaveAdjectives(IEnumerable<string> adjectives, CommandLineArguments.SourceDefinition sourceDef)
        {
            if (!adjectives.Any())
            {
                File.Delete("ScrapedAdjectives.xml");
                return;
            }

            var tags = XmlTagsAttribute(sourceDef);
            using (var stream = new FileStream("ScrapedAdjectives.xml", FileMode.Create, FileAccess.Write, FileShare.None, 16 * 1024, true))
            using (var writer = new StreamWriter(stream, Utf8WithoutBOM))
            {
                await writer.WriteLineAsync("<dictionary>");

                foreach (var adjective in adjectives)
                {
                    await writer.WriteLineAsync($"  <adjective value=\"{EncodeForXml(adjective)}\" {tags}/>");
                }
                await writer.WriteLineAsync("</dictionary>");
            }
        }

        private static async Task SaveAdverbs(IEnumerable<string> adverbs, CommandLineArguments.SourceDefinition sourceDef)
        {
            if (!adverbs.Any())
            {
                File.Delete("ScrapedAdverbs.xml");
                return;
            }

            var tags = XmlTagsAttribute(sourceDef);
            using (var stream = new FileStream("ScrapedAdverbs.xml", FileMode.Create, FileAccess.Write, FileShare.None, 16 * 1024, true))
            using (var writer = new StreamWriter(stream, Utf8WithoutBOM))
            {
                await writer.WriteLineAsync("<dictionary>");

                foreach (var adverb in adverbs)
                {
                    await writer.WriteLineAsync($"  <adverb value=\"{EncodeForXml(adverb)}\" {tags}/>");
                }
                await writer.WriteLineAsync("</dictionary>");
            }
        }

        private static async Task SaveVerbs(IEnumerable<string> transitiveVerbs, IEnumerable<string> intransitiveVerbs, CommandLineArguments.SourceDefinition sourceDef)
        {
            if (!transitiveVerbs.Any() && !intransitiveVerbs.Any())
            {
                File.Delete("ScrapedVerbs.xml");
                return;
            }

            var tags = "\r\n        " + XmlTagsAttribute(sourceDef);
            using (var stream = new FileStream("ScrapedVerbs.xml", FileMode.Create, FileAccess.Write, FileShare.None, 16 * 1024, true))
            using (var writer = new StreamWriter(stream, Utf8WithoutBOM))
            {
                await writer.WriteLineAsync("<dictionary>");

                foreach (var verb in transitiveVerbs)
                {
                    var line = BuildXml(verb, true);
                    await writer.WriteLineAsync(line);
                }
                foreach (var verb in intransitiveVerbs)
                {
                    var line = BuildXml(verb, false);
                    await writer.WriteLineAsync(line);
                }

                await writer.WriteLineAsync("</dictionary>");
            }

            string BuildXml(string root, bool transitive)
            {
                var toES = root.EndsWith("s") ? $"{root}es" : $"{root}s";
                toES = root.EndsWith("x") ? $"{root}es" : toES;
                toES = root.EndsWith("e") ? $"{root}s" : toES;
                toES = root.EndsWith("y") ? $"{root.Substring(0, root.Length - 1)}ies" : toES;
                toES = root.EndsWith("ch") || root.EndsWith("sh") ? $"{root}es" : toES;

                var toED = root.EndsWith("e") ? $"{root}d" : $"{root}ed";
                toED = root.EndsWith("y") ? $"{root.Substring(0, root.Length - 1)}ied" : toED;

                var toING = root.EndsWith("e") ? $"{root.Substring(0, root.Length - 1)}ing" : $"{root}ing";

                var line = "";
                if (transitive)
                    line = $@"  <verb presentSingular=""{toES}"" pastSingular=""{toED}"" pastContinuousSingular=""was {toING}"" futureSingular=""will {root}"" continuousSingular=""is {toING}"" perfectSingular=""has {toED}"" subjunctiveSingular=""might {root}""
        presentPlural = ""{root}"" pastPlural = ""{toED}"" pastContinuousPlural = ""were {toING}"" futurePlural = ""will {root}"" continuousPlural = ""are {toING}"" perfectPlural = ""have {toED}"" subjunctivePlural = ""might {root}"" {tags}/>";
                else
                    line = $@"  <verb presentSingular=""{toES}"" pastSingular=""{toED}"" pastContinuousSingular=""was {toING}"" futureSingular=""will {root}"" continuousSingular=""is {toING}"" perfectSingular=""has {toED}"" subjunctiveSingular=""might {root}""
        presentPlural = ""{root}"" pastPlural = ""{toED}"" pastContinuousPlural = ""were {toING}"" futurePlural = ""will {root}"" continuousPlural = ""are {toING}"" perfectPlural = ""have {toED}"" subjunctivePlural = ""might {root}"" 
        transitive=""false"" {tags}/>";

                return line;
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

        private static string XmlTagsAttribute(CommandLineArguments.SourceDefinition sourceDef)
            => string.IsNullOrEmpty(sourceDef.Tags) ? ""
            : $"tags=\"{sourceDef.Tags}\" ";

        static void ReportProgress(int wordCount, int attempts)
        {
            if (DateTime.UtcNow < NextProgressUpdate)
                return;

            Console.WriteLine($"[{attempts:N0} of {Args.Attempts:N0} attempts, {wordCount:N0} words scraped successfully]");
            NextProgressUpdate = DateTime.UtcNow.Add(TimeSpan.FromSeconds(2).Add(TimeSpan.FromMilliseconds(Args.DelayMs)));
        }

        static void ReportMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}