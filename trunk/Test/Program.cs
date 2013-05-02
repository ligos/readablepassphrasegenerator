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
using MurrayGrant.ReadablePassphrase;
using MurrayGrant.ReadablePassphrase.Dictionaries;
using MurrayGrant.ReadablePassphrase.Words;
using MurrayGrant.ReadablePassphrase.Random;
using MurrayGrant.ReadablePassphrase.PhraseDescription;

namespace Test
{
    class Program
    {
        private readonly static string AllStatsCharFilename = "stats.all.char.csv";
        private readonly static IEnumerable<PhraseStrength> RandomStrengths = new[] { PhraseStrength.Random, PhraseStrength.RandomForever, PhraseStrength.RandomLong, PhraseStrength.RandomShort };

        static void Main(string[] args)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var generator = new ReadablePassphraseGenerator(GetRandomness());
            var loader = new ExplicitXmlDictionaryLoader();
            var dict = loader.LoadFrom();
            generator.SetDictionary(dict);
            sw.Stop();
            Console.WriteLine("Loaded dictionary of type '{0}' with {1:N0} words in {2:N2}ms ({3:N3} words / sec)", loader.GetType().Name, generator.Dictionary.Count, sw.Elapsed.TotalMilliseconds, generator.Dictionary.Count / sw.Elapsed.TotalSeconds);

            GenerateSamples(PhraseStrength.Random, generator);
            DictionaryCheck(generator);
            CombinationCount(generator);

            BenchmarkGeneration(generator, PhraseStrength.Normal, 1000);
            BenchmarkSecureGeneration(generator, PhraseStrength.Normal, 1000);
            BenchmarkUtf8Generation(generator, PhraseStrength.Normal, 1000);
            BenchmarkGeneration(generator, PhraseStrength.Strong, 1000);
            BenchmarkGeneration(generator, PhraseStrength.Insane, 1000);
            BenchmarkGeneration(generator, PhraseStrength.Random, 1000);
            Console.WriteLine();

            var specialStrengths = RandomStrengths.Concat(new [] { PhraseStrength.Custom });
            var allToTest = Enum.GetValues(typeof(PhraseStrength)).Cast<PhraseStrength>()
                .Where(x => !specialStrengths.Contains(x));
            foreach (var strength in allToTest)
            {
                TestTextualParsing(generator, strength);
                TestGeneration(generator, strength, 50);
                TestGenerationAsUtf8(generator, strength, 20);
                TestGenerationAsSecure(generator, strength, 20);
            }
            foreach (var strength in RandomStrengths)
            {
                TestGeneration(generator, Clause.RandomMappings[strength], 10);
                TestGenerationAsUtf8(generator, Clause.RandomMappings[strength], 10);
                TestGenerationAsSecure(generator, Clause.RandomMappings[strength], 10);
            }

            // Generate statistics.
            //Console.WriteLine();
            //System.IO.File.Delete(AllStatsCharFilename);
            //foreach (var strength in allToTest)
            //    WriteStatisticsFor(generator, strength, 1000000, strength.ToString() + ".csv");
            //foreach (var strength in RandomStrengths)
            //    WriteStatisticsFor(generator, strength, 10000000, strength.ToString() + ".csv");

            //GenerateCustomSamples(new Clause[]
            //    {
            //        new NounClause() { CommonNounFactor = 1, ProperNounFactor = 1, NounFromAdjectiveFactor = 1,
            //                            SingularityFactor = 1, PluralityFactor = 1, 
            //                            NoArticleFactor = 1, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 1, PersonalPronounFactor = 1,
            //                            NoAdjectiveFactor = 1, AdjectiveFactor = 0,
            //                            NoPrepositionFactor = 1, PrepositionFactor = 0},
            //        new VerbClause() { PresentFactor = 1, PastFactor = 1, FutureFactor = 1, ContinuousFactor = 1, ContinuousPastFactor = 1, PerfectFactor = 1, SubjunctiveFactor = 1,
            //                            NoAdverbFactor = 1, AdverbFactor = 0,
            //                            NoInterrogativeFactor = 1, InterrogativeFactor = 0,
            //                            IntransitiveByNoNounClauseFactor = 1, IntransitiveByPrepositionFactor = 1},
            //        new NounClause() { CommonNounFactor = 1, ProperNounFactor = 0, NounFromAdjectiveFactor = 0,
            //                            SingularityFactor = 1, PluralityFactor = 1, 
            //                            NoArticleFactor = 1, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 1, PersonalPronounFactor = 1,
            //                            NoAdjectiveFactor = 1, AdjectiveFactor = 0,
            //                            NoPrepositionFactor = 1, PrepositionFactor = 0},
            //    }
            //    , generator, 100);
            //GenerateSamples(PhraseStrength.InsaneEqual, generator);
            //TestConfigForm(generator);

            // Longer benchmarks.
            //BenchmarkGeneration(generator, ReadablePassphrase.PhraseStrength.Normal, 10000);
            //BenchmarkGeneration(generator, ReadablePassphrase.PhraseStrength.Strong, 10000);
            //BenchmarkGeneration(generator, ReadablePassphrase.PhraseStrength.Insane, 10000);            

            // Random function distribution tests.
            //TestCoinFlip(SeededRandom());
            //TestWeightedCoinFlip(SeededRandom(), 1, 1);
            //TestNextInt32(SeededRandom(), 4);
            //TestNextInt32(SeededRandom(), 15);
            //TestNextInt32(SeededRandom(), 50);
            //TestNextInt32(SeededRandom(), 64);
            //TestNextInt32(SeededRandom(), 256);
            //TestNextInt32(SeededRandom(), 1296);

            // TODO: Test load an alternate dictionary loader.

            // Test the config form.
            //TestConfigForm(generator);

            Console.WriteLine();
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey(true);
        }

        private static void DictionaryCheck(ReadablePassphraseGenerator generator)
        {
            Console.WriteLine();
            Console.WriteLine("Name: {0}", generator.Dictionary.Name);
            Console.WriteLine("Langauge: {0}", generator.Dictionary.LanguageCode);
            Console.WriteLine("TOTAL:           {0:N0}", generator.Dictionary.Count);
            Console.WriteLine("TOTAL forms:     {0:N0}", generator.Dictionary.CountOfAllDistinctForms());
            Console.WriteLine("Nouns:           {0:N0}", generator.Dictionary.OfType<Noun>().Count());
            Console.WriteLine("Proper Nouns:    {0:N0}", generator.Dictionary.OfType<ProperNoun>().Count());
            Console.WriteLine("Verbs (all):     {0:N0}", generator.Dictionary.OfType<Verb>().Count());
            Console.WriteLine("Verbs (trans):   {0:N0}", generator.Dictionary.OfType<Verb>().Count(w => w.IsTransitive));
            Console.WriteLine("Verbs (intrans): {0:N0}", generator.Dictionary.OfType<Verb>().Count(w => !w.IsTransitive));
            Console.WriteLine("Prepositions:    {0:N0}", generator.Dictionary.OfType<Preposition>().Count());
            Console.WriteLine("Adverbs:         {0:N0}", generator.Dictionary.OfType<Adverb>().Count());
            Console.WriteLine("Adjectives:      {0:N0}", generator.Dictionary.OfType<Adjective>().Count());

            // Check for duplicates.
            foreach (var t in typeof(Word).Assembly.GetTypes().Where(t => typeof(Word).IsAssignableFrom(t) && t != typeof(Word)))
            {
                var duplicates = generator.Dictionary
                                        .Where(w => t.IsAssignableFrom(w.GetType()))
                                        .GroupBy(w => w.DictionaryEntry, StringComparer.OrdinalIgnoreCase)
                                        .Where(g => g.Count() > 1);
                if (duplicates.Any())
                {
                    Console.WriteLine("DUPLICATES for {0}:", t.Name);
                    foreach (var g in duplicates)
                        Console.WriteLine("    - " + g.Key);
                }
            }
        }

        private static void GenerateSamples(PhraseStrength strength, ReadablePassphraseGenerator generator)
        {
            GenerateSamples(strength, generator, 20);
        }
        private static void GenerateSamples(PhraseStrength strength, ReadablePassphraseGenerator generator, int count)
        {
            Console.WriteLine();
            Console.WriteLine("Samples:");
            for (int i = 0; i < count; i++)
                Console.WriteLine(generator.Generate(strength));
        }
        private static void GenerateCustomSamples(IEnumerable<MurrayGrant.ReadablePassphrase.PhraseDescription.Clause> clause, ReadablePassphraseGenerator generator, int count)
        {
            Console.WriteLine();
            Console.WriteLine("Custom samples:");
            var combinations = generator.CalculateCombinations(clause);
            Console.WriteLine("Combinations: {0:E3} ({1:N2} bits)", combinations.ToString(), combinations.EntropyBitsToString());
            for (int i = 0; i < count; i++)
            {
                Console.WriteLine(generator.Generate(clause));
            }

        }

        private static void CombinationCount(ReadablePassphraseGenerator generator)
        {
            Console.WriteLine();
            Console.WriteLine("Combination count:");

            foreach (var strength in RandomStrengths)
            {
                var combinations = generator.CalculateCombinations(strength);
                Console.WriteLine("  {0}: {1:E3} ({2:N2} bits)", strength, combinations.ToString(), combinations.EntropyBitsToString());
            }
            Console.WriteLine();

            var predefined = new PhraseStrength[] 
            { 
                PhraseStrength.Normal, PhraseStrength.NormalAnd, PhraseStrength.NormalSpeech, PhraseStrength.NormalEqual, PhraseStrength.NormalEqualAnd, PhraseStrength.NormalEqualSpeech, PhraseStrength.NormalRequired, PhraseStrength.NormalRequiredAnd, PhraseStrength.NormalRequiredSpeech, 
                PhraseStrength.Strong, PhraseStrength.StrongAnd, PhraseStrength.StrongSpeech, PhraseStrength.StrongEqual, PhraseStrength.StrongEqualAnd, PhraseStrength.StrongEqualSpeech, PhraseStrength.StrongRequired, PhraseStrength.StrongRequiredAnd, PhraseStrength.StrongRequiredSpeech,
                PhraseStrength.Insane, PhraseStrength.InsaneAnd, PhraseStrength.InsaneSpeech, PhraseStrength.InsaneEqual, PhraseStrength.InsaneEqualAnd, PhraseStrength.InsaneEqualSpeech, PhraseStrength.InsaneRequired, PhraseStrength.InsaneRequiredAnd, PhraseStrength.InsaneRequiredSpeech,
            };
            for (int i = 0; i < predefined.Length; i++)
            {
                var strength = predefined[i];
                var combinations = generator.CalculateCombinations(strength);
                Console.WriteLine("  {0}: {1:E3} ({2:N2} bits)", strength, combinations.ToString(), combinations.EntropyBitsToString());
                if ((i+1) % 9 == 0)
                    Console.WriteLine();
            }

        }

        private static void TestTextualParsing(ReadablePassphraseGenerator generator, PhraseStrength strength)
        {
            Console.WriteLine("Testing parsing of textual phrase strength {0}...", strength);
            var description = Clause.CreatePhraseDescription(strength, generator.Randomness);
            var sb = new StringBuilder();
            foreach (var c in description)
                c.ToStringBuilder(sb);
            var textualDescription = sb.ToString();
            Console.Write("    Generation OK.");
            var customStrength = Clause.CreateCollectionFromTextString(textualDescription);
            Console.Write(" Parsing OK.");
            generator.Generate(customStrength);
            Console.WriteLine(" Passphrase OK.");
        }
        private static void TestGeneration(ReadablePassphraseGenerator generator, PhraseStrength strength, int iterations)
        {
            Console.Write("Testing {0:N0} string phrases of strength {1}...", iterations, strength);
            for (int i = 0; i < iterations; i++)
                generator.Generate(strength);
            Console.WriteLine(" OK.");
        }
        private static void TestGeneration(ReadablePassphraseGenerator generator, IEnumerable<PhraseStrength> strengths, int iterations)
        {
            Console.Write("Testing {0:N0} string phrases of strength {1}...", iterations, String.Join(",", strengths.Select(x => x.ToString())));
            for (int i = 0; i < iterations; i++)
                generator.Generate(strengths);
            Console.WriteLine(" OK.");
        }
        private static void TestGenerationAsUtf8(ReadablePassphraseGenerator generator, PhraseStrength strength, int iterations)
        {
            Console.Write("Testing {0:N0} UTF8 phrases of strength {1}...", iterations, strength);
            for (int i = 0; i < iterations; i++)
                generator.GenerateAsUtf8Bytes(strength);
            Console.WriteLine(" OK.");
        }
        private static void TestGenerationAsUtf8(ReadablePassphraseGenerator generator, IEnumerable<PhraseStrength> strengths, int iterations)
        {
            Console.Write("Testing {0:N0} UTF8 phrases of strength {1}...", iterations, String.Join(",", strengths.Select(x => x.ToString())));
            for (int i = 0; i < iterations; i++)
                generator.GenerateAsUtf8Bytes(strengths);
            Console.WriteLine(" OK.");
        }
        private static void TestGenerationAsSecure(ReadablePassphraseGenerator generator, PhraseStrength strength, int iterations)
        {
            Console.Write("Testing {0:N0} secure phrases of strength {1}...", iterations, strength);
            for (int i = 0; i < iterations; i++)
                generator.GenerateAsSecure(strength);
            Console.WriteLine(" OK.");
        }
        private static void TestGenerationAsSecure(ReadablePassphraseGenerator generator, IEnumerable<PhraseStrength> strengths, int iterations)
        {
            Console.Write("Testing {0:N0} secure phrases of strength {1}...", iterations, String.Join(",", strengths.Select(x => x.ToString())));
            for (int i = 0; i < iterations; i++)
                generator.GenerateAsSecure(strengths);
            Console.WriteLine(" OK.");
        }

        private static void BenchmarkGeneration(ReadablePassphraseGenerator generator, PhraseStrength strength, int iterations)
        {
            Console.WriteLine();
            Console.WriteLine("Benchmark of {0:N0} phrases of strength {1}...", iterations, strength);
            var sw = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
                generator.Generate(strength);
            sw.Stop();
            Console.WriteLine("  in {0:N3}ms", sw.Elapsed.TotalMilliseconds);

        }

        private static void BenchmarkSecureGeneration(ReadablePassphraseGenerator generator, PhraseStrength strength, int iterations)
        {
            Console.WriteLine();
            Console.WriteLine("Benchmark of {0:N0} secure phrases of strength {1}...", iterations, strength);
            var sw = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
                generator.GenerateAsSecure(strength);
            sw.Stop();
            Console.WriteLine("  in {0:N3}ms", sw.Elapsed.TotalMilliseconds);
        }
        private static void BenchmarkUtf8Generation(ReadablePassphraseGenerator generator, PhraseStrength strength, int iterations)
        {
            Console.WriteLine();
            Console.WriteLine("Benchmark of {0:N0} UTF8 phrases of strength {1}...", iterations, strength);
            var sw = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
                generator.GenerateAsUtf8Bytes(strength);
            sw.Stop();
            Console.WriteLine("  in {0:N3}ms", sw.Elapsed.TotalMilliseconds);
        }

        private static void TestCoinFlip(RandomSourceBase randomness)
        {
            int countTrue = 0, countFalse = 0;

            Console.WriteLine("Testing CryptoRandomStream.CoinFlip()...");
            var sw = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < 1000000; i++)
            {
                if (randomness.CoinFlip())
                    countTrue++;
                else
                    countFalse++;
            }
            sw.Stop();
            Console.WriteLine("1000000 coin flips in {0:N3}ms", sw.Elapsed.TotalMilliseconds);
            Console.WriteLine("True, " + countTrue);
            Console.WriteLine("False, " + countFalse);
            Console.WriteLine();
        }

        private static void TestWeightedCoinFlip(RandomSourceBase randomness, int trueWeight, int falseWeight)
        {
            int countTrue = 0, countFalse = 0;

            Console.WriteLine("Testing CryptoRandomStream.WeightedCoinFlip()...");
            var sw = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < 1000000; i++)
            {
                if (randomness.WeightedCoinFlip(trueWeight, falseWeight))
                    countTrue++;
                else
                    countFalse++;
            }
            sw.Stop();
            Console.WriteLine("1000000 coin flips in {0:N3}ms", sw.Elapsed.TotalMilliseconds);
            Console.WriteLine("True, " + countTrue);
            Console.WriteLine("False, " + countFalse);
            Console.WriteLine();
        }

        private static void TestNextInt32(RandomSourceBase randomness, int max)
        {
            var distributionTable = new Dictionary<int, int>();
            for (int i = 0; i < max; i++)
                distributionTable.Add(i, 0);

            Console.WriteLine("Testing CryptoRandomStream.Next({0})...", max);
            var sw = System.Diagnostics.Stopwatch.StartNew();
            int trials = max * 50000;
            for (int i = 0; i < trials; i++)
            {
                var choice = randomness.Next(max);
                distributionTable[choice] = distributionTable[choice] + 1;
            }
            sw.Stop();
            Console.WriteLine("{0} choices in {1:N3}ms", trials, sw.Elapsed.TotalMilliseconds);
            for(int i = 0; i < max; i++)
                Console.WriteLine("{0}, {1}", i, distributionTable[i]);
            Console.WriteLine();
        }

        private static void TestConfigForm(ReadablePassphraseGenerator generator)
        {
            String config;
            using (var frm = new KeePassReadablePassphrase.ConfigRoot("", GetRandomness()))
            {
                frm.ShowDialog();
                Console.WriteLine();
                Console.WriteLine("Serialised Config from Form:");
                Console.WriteLine(frm.ConfigForKeePass);
                config = frm.ConfigForKeePass;
            }
            using (var frm = new KeePassReadablePassphrase.ConfigRoot(config, GetRandomness()))
            {
                frm.ShowDialog();
            }
        }

        private static void WriteStatisticsFor(ReadablePassphraseGenerator generator, PhraseStrength strength, int count, string filename)
        {
            Console.Write("Writing statistics to '{0}'...", filename);

            var wordHistogram = new Dictionary<int, int>();
            var charHistogram = new Dictionary<int, int>();
            var keepassQualityHistogram = new Dictionary<uint, int>();
            var combinations = generator.CalculateCombinations(strength);
            for (int i = 0; i < count; i++)
            {
                var phrase = generator.Generate(strength);

                if (!charHistogram.ContainsKey(phrase.Length))
                    charHistogram.Add(phrase.Length, 0);
                charHistogram[phrase.Length] = charHistogram[phrase.Length] + 1;
                
                var wordCount = phrase.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Length;
                if (!wordHistogram.ContainsKey(wordCount))
                    wordHistogram.Add(wordCount, 0);
                wordHistogram[wordCount] = wordHistogram[wordCount] + 1;

                var keePassQualityEst = KeePassLib.Cryptography.QualityEstimation.EstimatePasswordBits(phrase.ToCharArray());
                if (!keepassQualityHistogram.ContainsKey(keePassQualityEst))
                    keepassQualityHistogram.Add(keePassQualityEst, 0);
                keepassQualityHistogram[keePassQualityEst] = keepassQualityHistogram[keePassQualityEst] + 1;
            }
            
            using (var writer = new System.IO.StreamWriter(filename, false, Encoding.UTF8))
            {
                writer.WriteLine("Word histogram");
                for (int i = wordHistogram.Keys.Min(); i < wordHistogram.Keys.Max()+1; i++)
                    writer.WriteLine("{0},{1}", i, wordHistogram.ContainsKey(i) ? wordHistogram[i] : 0);
                writer.WriteLine();
                
                writer.WriteLine("Character histogram");
                for (int i = charHistogram.Keys.Min(); i < charHistogram.Keys.Max()+1; i++)
                    writer.WriteLine("{0},{1}", i, charHistogram.ContainsKey(i) ? charHistogram[i] : 0);
                writer.WriteLine();

                writer.WriteLine("KeePass Quality Estimate");
                for (uint i = keepassQualityHistogram.Keys.Min(); i < keepassQualityHistogram.Keys.Max() + 1; i++)
                    writer.WriteLine("{0},{1}", i, keepassQualityHistogram.ContainsKey(i) ? keepassQualityHistogram[i] : 0);
                writer.WriteLine();

                writer.WriteLine("Combination counts");
                writer.WriteLine("Min:,{0:E3},{1:N2}", combinations.Shortest, combinations.ShortestAsEntropyBits);
                writer.WriteLine("Max:,{0:E3},{1:N2}", combinations.Longest, combinations.LongestAsEntropyBits);
                writer.WriteLine("Avg:,{0:E3},{1:N2}", combinations.OptionalAverage, combinations.OptionalAverageAsEntropyBits);

                writer.WriteLine();
                writer.WriteLine("Samples:");
                for (int i = 0; i < 20; i++)
                    writer.WriteLine(generator.Generate(strength));
            }

            Console.WriteLine(" Done.");

            bool isFirst = !System.IO.File.Exists(AllStatsCharFilename);
            using (var writer = new System.IO.StreamWriter(AllStatsCharFilename, true, Encoding.UTF8))
            {
                if (isFirst)
                    writer.WriteLine("Strength,Min,Max,Avg,Median,Samples");
                writer.WriteLine("{0},{1},{2},{3},{4},{5}", strength.ToString(), charHistogram.Keys.Min(), charHistogram.Keys.Max(), GetAvg(charHistogram), GetMedian(charHistogram), count);
            }
        }
        private static double GetAvg(IDictionary<int, int> histogram)
        {
            var result = histogram.Select(x => (double)x.Key * (double)x.Value).Sum()
                       / histogram.Select(x => x.Value).Sum();
            return result;
        }
        private static int GetMedian(IDictionary<int, int> histogram)
        {
            var copy = histogram.ToDictionary(x => x.Key, x => x.Value);
            
            var total = copy.Values.Sum();
            while (total > 2)
            {
                var minKey = copy.Keys.Min();
                var maxKey = copy.Keys.Max();
                copy[minKey] = copy[minKey] - 1;
                copy[maxKey] = copy[maxKey] - 1;
                if (copy[minKey] <= 0)
                    copy.Remove(minKey);
                if (copy[maxKey] <= 0)
                    copy.Remove(maxKey);

                total -= 2;
            }
            var result = copy.Keys.First();
            return result;
        }

        private static RandomSourceBase GetRandomness()
        {
            return CryptoRandom();
        }
        private static RandomSourceBase SeededRandom()
        {
            var randomness = new KeePassLib.Cryptography.CryptoRandomStream(KeePassLib.Cryptography.CrsAlgorithm.Salsa20, new byte[] { 0x01 });
            return new KeePassReadablePassphrase.KeePassRandomSource(randomness);
        }
        private static RandomSourceBase CryptoRandom()
        {
            return new CryptoRandomSource();
        }
    }
}