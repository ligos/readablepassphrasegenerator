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
using MurrayGrant.ReadablePassphrase.Mutators;

namespace MurrayGrant.ReadablePassphrase.Generator
{
    class Program
    {
        // This is a bit of a cheats way of doing command line arguments. Please don't consider it good practice!
        static int count = 10;
        static PhraseStrength strength = PhraseStrength.Random;
        static string wordSeparator = " ";
        static bool useCustomLoader = false;
        static string loaderDll = "";
        static string loaderType = "";
        static string loaderArguments = "";
        static string customPhrasePath = "";
        static bool quiet = false;
        static bool applyStandardMutators = false;
        static bool applyAlternativeMutators = false;
        static NumericStyles numericStyle = NumericStyles.Never;
        static int numericCount = 2;
        static AllUppercaseStyles upperStyle = AllUppercaseStyles.Never;
        static int upperCount = 2;
        static ConstantStyles constantStyle = ConstantStyles.Never;
        static string constantValue = ".";
        static IEnumerable<Clause> phraseDescription = new Clause[] { };
        static int maxLength = 999;
        static int minLength = 1;
        static int anyLength = 0;       // Used to indicate a non-gramatic, totally random selection of word forms.
        static CountBy countBy = CountBy.Char;
        static bool noFake = false;

        static readonly int MaxAttemptsPerCount = 1000;

        static void Main(string[] args)
        {

            try
            {
                Console.OutputEncoding = Encoding.UTF8;
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
            {
                var ver = GetProgramVersion();
                var idx = ver.IndexOf('.', ver.IndexOf('.', ver.IndexOf('.') + 1) + 1);
                Console.WriteLine("Readable Passphrase Generator {0}", ver.Substring(0, idx));
            }
            var countByDescription = countBy == CountBy.Char ? "characters"
                                   : countBy == CountBy.Word ? "words"
                                   : "characters";
            if (!quiet && anyLength > 0)
                Console.WriteLine("Generating {0:N0} non-grammatic phrase(s) of length '{1}'...", count, anyLength);
            else if (!quiet && String.IsNullOrEmpty(customPhrasePath))
                Console.WriteLine("Generating {0:N0} phrase(s) of strength '{1}'...", count, strength);
            else if (!quiet && !String.IsNullOrEmpty(customPhrasePath))
                Console.WriteLine("Generating {0:N0} phrase(s) based on phrase description in '{1}'...", count, System.IO.Path.GetFileName(customPhrasePath));
            if (!quiet && (maxLength < Int32.MaxValue || minLength > 1))
                Console.WriteLine("Must be between {0:N0} and {1} {2}.", minLength, maxLength == Int32.MaxValue ? "∞" : maxLength.ToString("N0"), countByDescription);

            var generator = new ReadablePassphraseGenerator();

            // Must load dictionary before trying to generate.
            var dictSw = System.Diagnostics.Stopwatch.StartNew();
            var loaderT = GetDictionaryLoaderType();
            // If the internal dictionary loader is being used and no other arguments are specified, tell it to use the default dictionary.
            if (loaderT == typeof(ExplicitXmlDictionaryLoader) && String.IsNullOrEmpty(loaderArguments.Trim()))
            {
                var tagsToExclude = noFake ? Tags.Fake : "";
                loaderArguments = "useDefaultDictionary=true;excludeWordsWithTags=" + tagsToExclude;
            }

            // And load our dictionary!
            using (var loader = Activator.CreateInstance(loaderT) as IDictionaryLoader) 
            {
                if (loader == null)
                    throw new Exception($"Unable to create instance of dictionary loader {loaderT.FullName}");
                generator.LoadDictionary(loader, loaderArguments);
            }
            dictSw.Stop();

            // Tag array for fakes.
            var fakeDescription = noFake ? "non-fake " : "";

            // Summarise actions and combinations / entropy.
            if (!quiet)
            {
                Console.WriteLine("Dictionary contains {0:N0} {1}words (loaded in {2:N2}ms)", generator.Dictionary.Count, fakeDescription, dictSw.Elapsed.TotalMilliseconds);
                PhraseCombinations combinations;
                if (anyLength > 0)
                    combinations = generator.CalculateCombinations(NonGrammaticalClause(anyLength));
                else if (strength != PhraseStrength.Custom)
                    combinations = generator.CalculateCombinations(strength);
                else
                    combinations = generator.CalculateCombinations(phraseDescription);
                Console.WriteLine("Average combinations ~{0:E3} (~{1:N2} bits)", combinations.OptionalAverage, combinations.OptionalAverageAsEntropyBits);
                Console.WriteLine("Total combinations {0:E3} - {1:E3} ({2:N2} - {3:N2} bits)", combinations.Shortest, combinations.Longest, combinations.ShortestAsEntropyBits, combinations.LongestAsEntropyBits);
                
                var upperTypeText = upperStyle == AllUppercaseStyles.RunOfLetters ? " run"
                                  : upperStyle == AllUppercaseStyles.WholeWord ? " word"
                                  : "";
                var upperTypeText2 = upperStyle == AllUppercaseStyles.RunOfLetters ? "run"
                                   : upperStyle == AllUppercaseStyles.WholeWord ? "word"
                                   : "capital"; 
                if (applyStandardMutators)
                    Console.WriteLine("Using standard mutators (2 numbers, 2 capitals, period at end)");
                else if (applyAlternativeMutators)
                    Console.WriteLine("Using alternate mutators (2 numbers, 1 whole capital word, period at end)");
                else if (numericStyle != 0 && upperStyle != 0 && constantStyle != 0)
                    Console.WriteLine($"Using upper case{upperTypeText}, numeric and constant mutators ({upperCount:N0} {upperTypeText2}(s), {numericCount:N0} number(s))");
                else if (numericStyle != 0 && upperStyle != 0 && constantStyle == 0)
                    Console.WriteLine($"Using upper case{upperTypeText} and numeric mutators ({upperCount:N0} {upperTypeText2}(s), {numericCount:N0} number(s))");
                else if (numericStyle == 0 && upperStyle != 0 && constantStyle != 0)
                    Console.WriteLine($"Using upper case{upperTypeText} and constant mutators ({upperCount:N0} {upperTypeText2}(s))");
                else if (numericStyle == 0 && upperStyle != 0 && constantStyle == 0)
                    Console.WriteLine($"Using upper case{upperTypeText} mutator only ({upperCount:N0} {upperTypeText2}(s))");
                else if (numericStyle != 0 && upperStyle == 0 && constantStyle != 0)
                    Console.WriteLine($"Using numeric and constant mutators ({numericCount:N0} number(s))");
                else if (numericStyle != 0 && upperStyle == 0 && constantStyle == 0)
                    Console.WriteLine($"Using numeric mutator only ({numericCount:N0} number(s))");
                else if (numericStyle == 0 && upperStyle == 0 && constantStyle != 0)
                    Console.WriteLine($"Using constant mutator only");
                else
                    Console.WriteLine("Using no mutators");
                Console.WriteLine();
            }

            // Generate!
            var genSw = System.Diagnostics.Stopwatch.StartNew();
            int generated = 0;
            int attempts = 0;
            int maxAttempts = count * MaxAttemptsPerCount;
            var mutators = applyStandardMutators ? new IMutator[] { UppercaseMutator.Basic, NumericMutator.Basic, ConstantMutator.Basic } 
                         : applyAlternativeMutators ? new IMutator[] { UppercaseWordMutator.Basic, NumericMutator.Basic, ConstantMutator.Basic }
                         : Enumerable.Empty<IMutator>();
            if (upperStyle > 0 && upperStyle <= AllUppercaseStyles.Anywhere)
                mutators = mutators.Concat(new IMutator[] { new UppercaseMutator() { When = (UppercaseStyles)upperStyle, NumberOfCharactersToCapitalise = upperCount } });
            if (upperStyle == AllUppercaseStyles.RunOfLetters)
                mutators = mutators.Concat(new IMutator[] { new UppercaseRunMutator() { NumberOfRuns = upperCount } });
            if (upperStyle == AllUppercaseStyles.WholeWord)
                mutators = mutators.Concat(new IMutator[] { new UppercaseWordMutator() { NumberOfWordsToCapitalise = upperCount } });
            if (numericStyle != 0)
                mutators = mutators.Concat(new IMutator[] { new NumericMutator() { When = numericStyle, NumberOfNumbersToAdd = numericCount } });
            if (constantStyle != 0)
                mutators = mutators.Concat(new IMutator[] { new ConstantMutator() { When = constantStyle, ValueToAdd = constantValue } });
            while (generated < count)
            {
                // Generate phrase.
                // We always include spaces in the phrase because the mutators rely on whitespace.
                string phrase;
                attempts++;
                if (anyLength > 0)
                    phrase = generator.Generate(NonGrammaticalClause(anyLength), " ", mutators);
                else if (strength == PhraseStrength.Custom)
                    phrase = generator.Generate(phraseDescription, " ", mutators);
                else
                    phrase = generator.Generate(strength, " ", mutators);

                // After mutators are applied, it's safe to remove white space.
                if (wordSeparator != " ")
                    phrase = phrase.Replace(" ", wordSeparator);

                // Clamp the length.
                if (PhraseWithinLengthCriteria(phrase))
                {
                    Console.WriteLine(phrase);
                    generated++;
                }
                if (attempts >= maxAttempts)
                    break;
            }
            genSw.Stop();

            // Summarise result.
            if (!quiet)
            {
                Console.WriteLine();
                Console.WriteLine("Generated {0:N0} phrase(s) in {1:N2}ms.", generated, genSw.Elapsed.TotalMilliseconds);
                if (attempts >= maxAttempts)
                    Console.WriteLine("But unable to generate requested {0:N0} phrase(s) after {1:N0} attempts." + Environment.NewLine + "Perhaps try changing the minimum or maximum phrase length.", count, attempts);
            }
        }

        static string GetProgramVersion()
        {
            var fileVersionAttrs = typeof(Program).Assembly.GetCustomAttributes(typeof(System.Reflection.AssemblyFileVersionAttribute), true);
            var fileVersionAttr = (System.Reflection.AssemblyFileVersionAttribute)fileVersionAttrs[0];
            return fileVersionAttr.Version;
        }

        static Type GetDictionaryLoaderType()
        {
            System.Reflection.Assembly? loaderAsm = null;
            if (useCustomLoader && !String.IsNullOrEmpty(loaderDll))
                loaderAsm = System.Reflection.Assembly.LoadFrom(loaderDll);
            Type? loaderT;
            if (!String.IsNullOrEmpty(loaderType) && loaderAsm != null)
                loaderT = loaderAsm.GetTypes().FirstOrDefault(t => (t.FullName ?? "").IndexOf(loaderType, StringComparison.CurrentCultureIgnoreCase) >= 0);
            else if (!String.IsNullOrEmpty(loaderType) && loaderAsm == null)
                loaderT = AppDomain.CurrentDomain.GetAssemblies()
                            .Where(a => (a.FullName ?? "").IndexOf("ReadablePassphrase", StringComparison.CurrentCultureIgnoreCase) >= 0)
                            .SelectMany(a => a.GetTypes())
                            .FirstOrDefault(t => (t.FullName ?? "").IndexOf(loaderType, StringComparison.CurrentCultureIgnoreCase) >= 0);
            else if (String.IsNullOrEmpty(loaderType) && loaderAsm == null)
                loaderT = typeof(ExplicitXmlDictionaryLoader);
            else
                throw new ApplicationException(String.Format("Unable to find type '{0}' in {1} assembly.", loaderType, String.IsNullOrEmpty(loaderDll) ? "<default>" : loaderDll));

            if (loaderT == null)
                throw new ApplicationException(String.Format("Unable to find type '{0}' in {1} assembly.", loaderType, String.IsNullOrEmpty(loaderDll) ? "<default>" : loaderDll));
            return loaderT;
        }

        static IEnumerable<Clause> NonGrammaticalClause(int count)
        {
            for (int i = 0; i < count; i++)
                yield return new AnyWordClause();
        }

        static bool PhraseWithinLengthCriteria(string phrase)
        {
            if (countBy == CountBy.Word && wordSeparator == "")
                return true;
            else if (countBy == CountBy.Word && wordSeparator != "")
            {
                var separator = new[] { wordSeparator };
                var words = phrase.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                return words.Length >= minLength && words.Length <= maxLength;
            }
            else
                return phrase.Length >= minLength && phrase.Length <= maxLength;
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
                    bool includeSpaces;
                    if (!Boolean.TryParse(args[i + 1], out includeSpaces))
                    {
                        Console.WriteLine("Invalid boolean '{0}' for 'spaces' option.", args[i + 1]);
                        return false;
                    }
                    else if (includeSpaces)
                        wordSeparator = " ";
                    else if (!includeSpaces)
                        wordSeparator = "";
                    i++;
                }
                else if (arg == "separator")
                {
                    wordSeparator = args[i + 1];
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
                else if (arg == "n" || arg == "nongrammar")
                {
                    if (!Int32.TryParse(args[i + 1].Trim(), out anyLength))
                    {
                        Console.WriteLine("Unable to parse number '{0}' for 'nongrammar' option.", args[i + 1]);
                        return false;
                    }
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
                else if (arg == "countby")
                {
                    if (!Enum.GetNames(typeof(CountBy)).Select(x => x.ToLower()).Contains(args[i + 1]))
                    {
                        Console.WriteLine("Unknown 'countBy' option '{0}'.", args[i + 1]);
                        return false;
                    }
                    countBy = (CountBy)Enum.Parse(typeof(CountBy), args[i + 1], true);
                    i++;
                }
                else if (arg == "m" || arg == "stdMutators")
                {
                    applyStandardMutators = true;
                }
                else if (arg == "m2" || arg == "altMutators")
                {
                    applyAlternativeMutators = true;
                }
                else if (arg == "mutnumeric")
                {
                    if (!Enum.GetNames(typeof(NumericStyles)).Select(x => x.ToLower()).Contains(args[i + 1]))
                    {
                        Console.WriteLine("Unknown 'mutNumeric' option '{0}'.", args[i + 1]);
                        return false;
                    }
                    numericStyle = (NumericStyles)Enum.Parse(typeof(NumericStyles), args[i + 1], true);
                    i++;
                }
                else if (arg == "mutnumericcount")
                {
                    if (!Int32.TryParse(args[i + 1].Trim(), out numericCount))
                    {
                        Console.WriteLine("Unable to parse number '{0}' for 'mutNumericCount' option.", args[i + 1]);
                        return false;
                    }
                    i++;
                }
                else if (arg == "mutupper")
                {
                    if (!Enum.GetNames(typeof(AllUppercaseStyles)).Select(x => x.ToLower()).Contains(args[i + 1]))
                    {
                        Console.WriteLine("Unknown 'mutUpper' option '{0}'.", args[i + 1]);
                        return false;
                    }
                    upperStyle = (AllUppercaseStyles)Enum.Parse(typeof(AllUppercaseStyles), args[i + 1], true);
                    i++;
                }
                else if (arg == "mutuppercount")
                {
                    if (!Int32.TryParse(args[i + 1].Trim(), out upperCount))
                    {
                        Console.WriteLine("Unable to parse number '{0}' for 'mutUpperCount' option.", args[i + 1]);
                        return false;
                    }
                    i++;
                }
                else if (arg == "mutconstant")
                {
                    if (!Enum.GetNames(typeof(ConstantStyles)).Select(x => x.ToLower()).Contains(args[i + 1]))
                    {
                        Console.WriteLine("Unknown 'mutConstant' option '{0}'.", args[i + 1]);
                        return false;
                    }
                    constantStyle = (ConstantStyles)Enum.Parse(typeof(ConstantStyles), args[i + 1], true);
                    i++;
                }
                else if (arg == "mutconstantvalue")
                {
                    constantValue = args[i + 1];
                    i++;
                }
                else if (arg == "nofake")
                {
                    noFake = true;
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
            Console.WriteLine("  -c --count nnn        Generates nnn phrases (default: {0})", count);
            Console.WriteLine("  -s --strength xxx     Selects phrase strength (default: {0})", strength);
            Console.WriteLine("                xxx =     [normal|strong|insane][equal|required][and|speech]");
            Console.WriteLine("                          or 'custom' or 'random[short|long|forever]'");
            Console.WriteLine("  --min xxx             Specifies a minimum length for phrases (def: {0})", minLength);
            Console.WriteLine("  --max xxx             Specifies a maximum length for phrases (def: {0})", maxLength);
            Console.WriteLine("  --countBy xxx         Min & max by [char|word] (def: {0})", countBy);
            Console.WriteLine("  --spaces true|false   Includes spaces between words (default: true)");
            Console.WriteLine("  --separator x         Character(s) to separate words (default: {0})", wordSeparator);
            Console.WriteLine("  --noFake              Exclude fake words from passphrases (default: {0})", noFake);
            Console.WriteLine("  -n --nongrammar nn    Creates non-grammatical passphrases of length nn");
            Console.WriteLine();
            Console.WriteLine("  -m --stdMutators      Adds 2 numbers and 2 capitals to the passphrase");
            Console.WriteLine("  -m2 --altMutators     Adds 2 numbers and capitalises a single word");
            Console.WriteLine("  --mutUpper xxx        Uppercase mutator style (default: {0})", upperStyle);
            Console.WriteLine("       xxx =      [startofword|anywhere|runofwords|wholeword]");
            Console.WriteLine("  --mutUpperCount nn    Number of capitals to add (default: {0})", upperCount);
            Console.WriteLine("  --mutNumeric xxx      Numeric mutator style (default: {0})", numericStyle);
            Console.WriteLine("       xxx =      [startofphrase|startofword|endofword|startorendofword|endofphrase|anywhere]");
            Console.WriteLine("  --mutNumericCount nn  Number of numbers to add (default: {0})", numericCount);
            Console.WriteLine("  --mutConstant xxx     Constant mutator style (default: {0})", constantStyle);
            Console.WriteLine("       xxx =      [startofphrase|endofphrase|middleofphrase|anywhere]");
            Console.WriteLine("  --mutConstantValue x  String to use as constant (default: {0})", constantValue);
            Console.WriteLine(); 
            Console.WriteLine("  -l --loaderdll path   Specifies a custom loader dll");
            Console.WriteLine("  -t --loadertype path  Specifies a custom loader type");
            Console.WriteLine("  -a --loaderargs str   Specifies arguments for custom loader");
            Console.WriteLine("  -d --dict str         Specifies a custom dictionary file");
            Console.WriteLine("  -p --phrase path      Specifies a custom phrase file ");
            Console.WriteLine("                          Must use -strength custom ");
            Console.WriteLine(); 
            Console.WriteLine("  -q --quiet            Does not display any status messages (default: {0})", quiet ? "hide" : "show");
            Console.WriteLine("  -h --help             Displays this message ");
            Console.WriteLine("See {0} for more information", ReadablePassphraseGenerator.GitHubHomepage);
            Console.WriteLine("Contact Murray via GitHub or at " + ReadablePassphraseGenerator.KeyBaseContact);
        }

        public enum CountBy
        {
            Char = 1,
            Word,
        }
    }
}
