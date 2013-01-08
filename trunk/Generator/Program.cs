// Copyright 2011 Murray Grant
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MurrayGrant.ReadablePassphrase.PhraseDescription;
using MurrayGrant.ReadablePassphrase.Dictionaries;
using MurrayGrant.ReadablePassphrase.Words;
using MurrayGrant.ReadablePassphrase.WordTemplate;

namespace MurrayGrant.ReadablePassphrase.Generator
{
    class Program
    {
        // This is a bit of a cheats way of doing command line arguments. Please don't consider it good practice!
        static int count = 1;
        static PhraseStrength strength = PhraseStrength.Normal;
        static bool includeSpaces = true;
        static string loaderDll = "";
        static string loaderType = "";
        static string loaderArguments = "";
        static string customPhrasePath = "";
        static bool quiet = false;
        static IEnumerable<Clause> phraseDescription = new Clause[] { };            

        static void Main(string[] args)
        {

            try
            {
                if (!ParseCommandLine(args))
                {
                    PrintUsage();
                    Environment.Exit(1);
                }

                RunMain();
                Environment.Exit(0);
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

        static void RunMain()
        {
            // Generate and print phrases.
            if (!quiet)
                Console.WriteLine("Readable Passphrase Generator");
            if (!quiet && String.IsNullOrEmpty(customPhrasePath))
                Console.WriteLine("Generating {0:N0} phrase(s) of strength '{1}'...", count, strength);
            else if (!quiet && !String.IsNullOrEmpty(customPhrasePath))
                Console.WriteLine("Generating {0:N0} phrase(s) based on phrase description in '{1}'...", count, System.IO.Path.GetFileName(customPhrasePath));

            var generator = new ReadablePassphraseGenerator();

            // Must load dictionary before trying to generate.
            var dictSw = System.Diagnostics.Stopwatch.StartNew();
            System.Reflection.Assembly loaderAsm = null;
            if (!String.IsNullOrEmpty(loaderDll))
                loaderAsm = System.Reflection.Assembly.LoadFrom(loaderDll);
            Type loaderT;
            if (!String.IsNullOrEmpty(loaderType) && loaderAsm != null)
                loaderT = loaderAsm.GetTypes().FirstOrDefault(t => t.FullName.IndexOf(loaderType, StringComparison.CurrentCultureIgnoreCase) >= 0);
            else if (!String.IsNullOrEmpty(loaderType) && loaderAsm == null)
                loaderT = AppDomain.CurrentDomain.GetAssemblies()
                            .Where(a => a.FullName.IndexOf("ReadablePassphrase", StringComparison.CurrentCultureIgnoreCase) >= 0)
                            .SelectMany(a => a.GetTypes())
                            .FirstOrDefault(t => t.FullName.IndexOf(loaderType, StringComparison.CurrentCultureIgnoreCase) >= 0);
            else if (String.IsNullOrEmpty(loaderType) && loaderAsm == null)
                loaderT = typeof(ExplicitXmlDictionaryLoader);
            else
                throw new ApplicationException(String.Format("Unable to find type '{0}' in {1} assembly.", loaderType, String.IsNullOrEmpty(loaderDll) ? "<default>" : loaderDll));
            
            // If the internal dictionary loader is being used and no other arguments are specified, tell it to use the default dictionary.
            if (loaderT == typeof(ExplicitXmlDictionaryLoader) && String.IsNullOrEmpty(loaderArguments.Trim()))
                loaderArguments = "useDefaultDictionary=true";

            // And load our dictionary!
            using (var loader = (IDictionaryLoader)Activator.CreateInstance(loaderT))
            {
                generator.LoadDictionary(loader, loaderArguments);
            }
            dictSw.Stop();

            // Summarise actions and combinations / entropy.
            if (!quiet)
            {
                Console.WriteLine("Dictionary contains {0:N0} words (loaded in {1:N2}ms)", generator.Dictionary.Count, dictSw.Elapsed.TotalMilliseconds);
                PhraseCombinations combinations;
                if (strength != PhraseStrength.Custom)
                    combinations = generator.CalculateCombinations(strength);
                else
                    combinations = generator.CalculateCombinations(phraseDescription);
                Console.WriteLine("Average combinations ~{0:N0} representing ~{1:N2} bits of entropy", combinations.OptionalAverage, combinations.OptionalAverageAsEntropyBits);
                Console.WriteLine("Total combinations {0:N0} - {1:N0} representing {2:N2} - {3:N2} bits of entropy", combinations.Shortest, combinations.Longest, combinations.ShortestAsEntropyBits, combinations.LongestAsEntropyBits);
                Console.WriteLine();
            }


            // Generate!
            var genSw = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < count; i++)
            {
                string phrase;
                if (strength != PhraseStrength.Custom)
                    phrase = generator.Generate(strength, includeSpaces);
                else
                    phrase = generator.Generate(phraseDescription, includeSpaces);
                Console.WriteLine(phrase);
            }
            genSw.Stop();

            // Summarise result.
            if (!quiet)
            {
                Console.WriteLine();
                Console.WriteLine("Generated {0} phrase(s) in {1:N2}ms.", count, genSw.Elapsed.TotalMilliseconds);
            }
        }

        static bool ParseCommandLine(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
			{
                var arg = args[i].ToLower().Trim();
                if (arg.StartsWith("-") || arg.StartsWith("--") || arg.StartsWith("/"))
                    arg = arg.Replace("--", "").Replace("-", "").Replace("/", "");

                if (arg == "c" || arg == "count")
                {
                    if (!Int32.TryParse(args[i + 1].Trim(), out count))
                    {
                        Console.WriteLine("Unable to parse number '{0}' for 'count' option.", args[i + 1]);
                        return false;
                    }
                    i++;
                }
                else if (arg == "s" || arg == "strength")
                {
                    if (!Enum.GetNames(typeof(PhraseStrength)).Select(x => x.ToLower()).Contains(args[i + 1]))
                    {
                        Console.WriteLine("Unknown 'strength' option '{0}'.", args[i + 1]);
                        return false;
                    }
                    strength = (PhraseStrength)Enum.Parse(typeof(PhraseStrength), args[i + 1], true);
                    i++;
                }
                else if (arg == "spaces")
                {
                    if (!Boolean.TryParse(args[i+1], out includeSpaces))
                    {
                        Console.WriteLine("Invalid boolean '{0}' for 'strength' option.", args[i + 1]);
                        return false;
                    }
                    i++;
                }
                else if (arg == "d" || arg == "dict")
                {
                    var customDictionaryPath = args[i + 1];
                    loaderArguments = "file=" + customDictionaryPath;
                    if (!System.IO.File.Exists(customDictionaryPath))
                    {
                        Console.WriteLine("Unable to find file '{0}' for 'dict' option.", customDictionaryPath);
                        return false;
                    }
                    i++;
                }
                else if (arg == "l" || arg == "loaderdll")
                {
                    loaderDll = args[i + 1];
                    if (!System.IO.File.Exists(loaderDll))
                    {
                        Console.WriteLine("Unable to find file '{0}' for 'loaderdll' option.", loaderDll);
                        return false;
                    }
                    i++;
                }
                else if (arg == "t" || arg == "loadertype")
                {
                    loaderType = args[i + 1];
                    i++;
                }
                else if (arg == "a" || arg == "loaderargs")
                {
                    loaderArguments = args[i + 1];
                    i++;
                }
                else if (arg == "p" || arg == "phrase")
                {
                    customPhrasePath = args[i + 1];
                    if (!System.IO.File.Exists(customPhrasePath))
                    {
                        Console.WriteLine("Unable to find file '{0}' for 'phrase' option.", args[i + 1]);
                        return false;
                    }
                    try
                    {
                        phraseDescription = ReadablePassphrase.PhraseDescription.Clause.CreateCollectionFromTextString(System.IO.File.ReadAllText(customPhrasePath));
                    }
                    catch (PhraseDescriptionParseException ex)
                    {
                        Console.WriteLine("Unable to parse file '{0}' for 'phrase' option:", args[i + 1]);
                        Console.WriteLine("  {0}", ex.Message);
                        if (ex.InnerException != null)
                            Console.WriteLine("  {0}: {1}", ex.InnerException.GetType().Name, ex.InnerException.Message);
                    }
                    i++;
                }
                else if (arg == "q" || arg == "quiet")
                {
                    quiet = true;
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
            Console.WriteLine("Usage: PassphraseGenerator.exe [options]");
            Console.WriteLine("  -c --count nnn        Generates nnn phrases (default: 1)");
            Console.WriteLine("  -s --strength xxx     Selects phrase strength (default: normal)");
            Console.WriteLine("                xxx =     [normal|strong|insane][equal] or 'custom'");
            Console.WriteLine("  --spaces true|false   Includes spaces between words (default: true)");
            Console.WriteLine("  -l --loaderdll path   Specifies a custom loader dll");
            Console.WriteLine("  -t --loadertype path  Specifies a custom loader type");
            Console.WriteLine("  -a --loaderargs str   Specifies arguments for custom loader");
            Console.WriteLine("  -d --dict str         Specifies a custom dictionary file");
            Console.WriteLine("  -p --phrase path      Specifies a custom phrase file ");
            Console.WriteLine("                          Must use -strength custom ");
            Console.WriteLine("  -q --quiet            Does not display any status messages (default: show) ");
            Console.WriteLine("  -h --help             Displays this message ");
            Console.WriteLine("See {0} for more information", ReadablePassphraseGenerator.CodeplexHomepage);
        }
    }
}
