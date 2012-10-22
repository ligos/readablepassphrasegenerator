﻿// Copyright 2011 Murray Grant
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
        static void Main(string[] args)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var generator = new ReadablePassphraseGenerator(GetRandomness());
            var loader = new ExplicitXmlDictionaryLoader();
            var dict = loader.LoadFrom();
            generator.SetDictionary(dict);
            sw.Stop();
            Console.WriteLine("Loaded dictionary of type '{0}' with {1:N0} words in {2:N2}ms ({3:N3} words / sec)", loader.GetType().Name, generator.Dictionary.Count, sw.Elapsed.TotalMilliseconds, generator.Dictionary.Count / sw.Elapsed.TotalSeconds); 

            GenerateSamples(PhraseStrength.Strong, generator);
            DictionaryCheck(generator);
            CombinationCount(generator);

            BenchmarkGeneration(generator, PhraseStrength.Normal, 1000);
            BenchmarkSecureGeneration(generator, PhraseStrength.Normal, 1000);
            BenchmarkUtf8Generation(generator, PhraseStrength.Normal, 1000);
            BenchmarkGeneration(generator, PhraseStrength.Strong, 1000);
            BenchmarkGeneration(generator, PhraseStrength.Insane, 1000);

            //GenerateCustomSamples(new Clause[]
            //    {
            //        new NounClause() { SingularityFactor = 0, PluralityFactor = 1, 
            //                            NoArticleFactor = 1, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 1, PersonalPronounFactor = 1,
            //                            NoAdjectiveFactor = 1, AdjectiveFactor = 0,
            //                            NoPrepositionFactor = 1, PrepositionFactor = 0},
            //        new VerbClause() { PresentFactor = 1, PastFactor = 1, FutureFactor = 1, ContinuousFactor = 1, ContinuousPastFactor = 1, PerfectFactor = 1, SubjunctiveFactor = 1,
            //                            NoAdverbFactor = 1, AdverbFactor = 0,
            //                            NoInterrogativeFactor = 1, InterrogativeFactor = 0,
            //                            NoIntransitiveFactor = 1, IntransitiveByNoNounClauseFactor = 1, IntransitiveByPrepositionFactor = 1},
            //        new NounClause() { SingularityFactor = 1, PluralityFactor = 1, 
            //                            NoArticleFactor = 1, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 1, PersonalPronounFactor = 1,
            //                            NoAdjectiveFactor = 1, AdjectiveFactor = 0,
            //                            NoPrepositionFactor = 1, PrepositionFactor = 0},
            //    }
            //    , generator, 100);
            //TestConfigForm(generator);

            // Longer benchmarks.
            //BenchmarkGeneration(generator, ReadablePassphrase.PhraseStrength.Normal, 10000);
            //BenchmarkGeneration(generator, ReadablePassphrase.PhraseStrength.Strong, 10000);
            //BenchmarkGeneration(generator, ReadablePassphrase.PhraseStrength.Insane, 10000);            

            // Random function distribution tests.
            //TestCoinFlip(SeededRandom());
            //TestNextInt32(SeededRandom(), 4);
            //TestNextInt32(SeededRandom(), 15);
            //TestNextInt32(SeededRandom(), 50);
            //TestNextInt32(SeededRandom(), 64);
            //TestNextInt32(SeededRandom(), 256);
            //TestNextInt32(SeededRandom(), 1296);

            // Test load an alternate dictionary loader.


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
            Console.WriteLine("Nouns:           {0:N0}", generator.Dictionary.OfType<Noun>().Count());
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
            Console.WriteLine();
            Console.WriteLine("Samples:");
            for (int i = 0; i < 20; i++)
            {
                Console.WriteLine(generator.Generate(strength));
            }

        }
        private static void GenerateCustomSamples(IEnumerable<MurrayGrant.ReadablePassphrase.PhraseDescription.Clause> clause, ReadablePassphraseGenerator generator, int count)
        {
            Console.WriteLine();
            Console.WriteLine("Custom samples:");
            for (int i = 0; i < count; i++)
            {
                Console.WriteLine(generator.Generate(clause));
            }

        }

        private static void CombinationCount(ReadablePassphraseGenerator generator)
        {
            Console.WriteLine();
            Console.WriteLine("Combination count:");
            foreach (var strength in new PhraseStrength[] { PhraseStrength.Normal, PhraseStrength.Strong, PhraseStrength.Insane })
            {
                Console.WriteLine("  {0}: {1:E3} ({2:N2} bits)", strength, generator.CalculateCombinations(strength), generator.CalculateEntropyBits(strength));
            }

        }

        private static void BenchmarkGeneration(ReadablePassphraseGenerator generator, PhraseStrength strength, int iterations)
        {
            Console.WriteLine();
            Console.WriteLine("Benchmark of {0:N0} phrases of strength {1}...", iterations, strength);
            var sw = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                generator.Generate(strength);
            }
            sw.Stop();
            Console.WriteLine("  in {0:N3}ms", sw.Elapsed.TotalMilliseconds);

        }

        private static void BenchmarkSecureGeneration(ReadablePassphraseGenerator generator, PhraseStrength strength, int iterations)
        {
            Console.WriteLine();
            Console.WriteLine("Benchmark of {0:N0} secure phrases of strength {1}...", iterations, strength);
            var sw = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                generator.GenerateAsSecure(strength);
            }
            sw.Stop();
            Console.WriteLine("  in {0:N3}ms", sw.Elapsed.TotalMilliseconds);
        }
        private static void BenchmarkUtf8Generation(ReadablePassphraseGenerator generator, PhraseStrength strength, int iterations)
        {
            Console.WriteLine();
            Console.WriteLine("Benchmark of {0:N0} UTF8 phrases of strength {1}...", iterations, strength);
            var sw = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                var bytes = generator.GenerateAsUtf8Bytes(strength);
            }
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
