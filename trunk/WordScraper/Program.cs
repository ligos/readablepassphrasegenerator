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
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MurrayGrant.WordScraper
{
    public class Program
    {
        // This is a bit of a cheats way of doing command line arguments. Please don't consider it good practice!
        static int wordCount = 100;
        static int minLength = 3;
        static int maxLength = 10;
        static string source = "";

        static CancellationTokenSource CancellationSource;

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
                CancellationSource = new CancellationTokenSource();
                await RunMain();
                Environment.Exit(0);
            }
            catch (TaskCanceledException)
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
            Console.WriteLine($"Scraping {wordCount:N0} words from {source}...");
            Console.WriteLine($"Must be between {minLength:N0} and {maxLength:N0} characters.");

            // Load current dictionary, so we can avoid duplicates.
            var defaultDictionary = ReadablePassphrase.Dictionaries.Default.Load();
            var allUniqueForms = defaultDictionary.SelectMany(w => w.AllForms()).ToHashSet(StringComparer.CurrentCultureIgnoreCase);
            Console.WriteLine($"Default dictionary contains {defaultDictionary.Count:N0} words and {allUniqueForms.Count:N0} unique forms.");
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
                    source = arg;
                }
                else if (arg == "c" || arg == "count")
                {
                    if (!Int32.TryParse(args[i + 1].Trim(), out wordCount))
                    {
                        Console.WriteLine("Unable to parse number '{0}' for 'count' option.", args[i + 1]);
                        return false;
                    }
                    i++;
                }
                else if (arg == "min")
                {
                    if (!Int32.TryParse(args[i + 1].Trim(), out minLength))
                    {
                        Console.WriteLine("Unable to parse number '{0}' for 'min' option.", args[i + 1]);
                        return false;
                    }
                    i++;
                }
                else if (arg == "max")
                {
                    if (!Int32.TryParse(args[i + 1].Trim(), out maxLength))
                    {
                        Console.WriteLine("Unable to parse number '{0}' for 'max' option.", args[i + 1]);
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
            Console.WriteLine("  -c --count nnn        Scrapes nnn words (default: {0})", wordCount);
            Console.WriteLine("  --min xxx             Specifies a minimum length for words (def: {0})", minLength);
            Console.WriteLine("  --max xxx             Specifies a maximum length for words (def: {0})", maxLength);
            Console.WriteLine();
            Console.WriteLine("  Supported sources:");
            Console.WriteLine("    thisworddoesnotexist.com");
            Console.WriteLine();
            Console.WriteLine("  -h --help             Displays this message ");
            Console.WriteLine("See {0} for more information", ReadablePassphrase.ReadablePassphraseGenerator.GitHubHomepage);
            Console.WriteLine("Contact Murray via GitHub or at " + ReadablePassphrase.ReadablePassphraseGenerator.KeyBaseContact);
        }
    }
}