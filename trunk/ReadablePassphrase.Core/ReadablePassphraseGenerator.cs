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
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Security;
using MurrayGrant.ReadablePassphrase.Words;
using MurrayGrant.ReadablePassphrase.Dictionaries;
using MurrayGrant.ReadablePassphrase.WordTemplate;
using MurrayGrant.ReadablePassphrase.PhraseDescription;
using MurrayGrant.ReadablePassphrase.Random;
using MurrayGrant.ReadablePassphrase.Mutators;

namespace MurrayGrant.ReadablePassphrase
{
    /// <summary>
    /// Passphrase generator.
    /// </summary>
    public sealed class ReadablePassphraseGenerator
    {
        public WordDictionary Dictionary { get; private set; }
        public RandomSourceBase Randomness { get; private set; }

        public readonly static Uri KeyBaseContact = new Uri("https://keybase.io/ligos");
        public readonly static Uri GitHubHomepage = new Uri("https://github.com/ligos/readablepassphrasegenerator");
        [Obsolete("Migrated to GitHub in 2019")]
        public readonly static Uri BitBucketHomepage = new Uri("https://bitbucket.org/ligos/readablepassphrasegenerator");
        [Obsolete("Migrated to BitBucket in 2017")]
        public readonly static Uri CodeplexHomepage = new Uri("http://readablepassphrase.codeplex.com");

        #region Constructor
        /// <summary>
        /// Initialises the object with the default random source (based on <c>RNGCryptoServiceProvider</c>) and dictionary (internal XML dictionary).
        /// </summary>
        public ReadablePassphraseGenerator() 
            : this(new EmptyDictionary(), new CryptoRandomSource())
        { 
        }
        /// <summary>
        /// Initialises the object with the given random source.
        /// </summary>
        public ReadablePassphraseGenerator(RandomSourceBase randomness) 
            : this(new EmptyDictionary(), randomness)
        {
        }
        /// <summary>
        /// Initialises the object with a preloaded dictionary
        /// </summary>
        public ReadablePassphraseGenerator(WordDictionary words) 
            : this (words, new CryptoRandomSource())
        {
        }
        /// <summary>
        /// Initialises the object with a preloaded dictionary and alternate random source.
        /// </summary>
        public ReadablePassphraseGenerator(WordDictionary words, RandomSourceBase randomness)
        {
            if (words == null)
                throw new ArgumentNullException("words");
            if (randomness == null)
                throw new ArgumentNullException("randomness");
            this.Randomness = randomness;
            this.Dictionary = words;
        }
        #endregion

        #region LoadDictionary()
        /// <summary>
        /// Loads a dictionary using the <c>IDictionaryLoader</c> and the given arguments.
        /// </summary>
        /// <param name="loader">The IDictionaryLoader to load the dictionary with.</param>
        /// <param name="arguments">The arguments to pass to the IDictionaryLoader, parsed like a database connection string.</param>
        /// <remarks>
        /// The arguments are parsed like a database connection string.
        /// An array of semicolon separated key value pairs are expected. 
        /// Whitespace is trimmed. Keys are case-insensitive.
        /// '=' and ';' are not valid characters. If you need to pass them as arguments, use the <c>IDictionary</c> overload.
        /// The meaning of arguments is determined by the <c>IDictionaryLoader</c>
        /// 
        /// Eg: url=http://server.com/file; iscompressed=true; 
        /// </remarks>
        public void LoadDictionary(IDictionaryLoader loader, string arguments)
        {
            _ = loader ?? throw new ArgumentNullException(nameof(loader));

            this.LoadDictionary(loader, this.ParseArgumentString(arguments ?? ""));
        }
        /// <summary>
        /// Loads a dictionary using the <c>IDictionaryLoader</c> and the given arguments.
        /// </summary>
        /// <param name="loader">The IDictionaryLoader to load the dictionary with.</param>
        /// <param name="arguments">The arguments to pass to the IDictionaryLoader.</param>
        public void LoadDictionary(IDictionaryLoader loader, IDictionary<string, string> arguments)
        {
            _ = loader ?? throw new ArgumentNullException(nameof(loader));
            _ = arguments ?? throw new ArgumentNullException(nameof(arguments));

            this.Dictionary = loader.Load(arguments);
        }

        /// <summary>
        /// Attempts to load a dictionary using the <c>IDictionaryLoader</c> and the given arguments.
        /// </summary>
        /// <param name="loader">The IDictionaryLoader to load the dictionary with.</param>
        /// <param name="arguments">The arguments to pass to the IDictionaryLoader.</param>
        /// <param name="error">The error which occured while loading the dictionary (if any).</param>
        /// <returns>True if the dictionary loaded successfully, false otherwise (and sets the <c>error</c> out parameter to the error).</returns>
        /// <remarks>
        /// See <c>LoadDictionary</c> for details of how <c>arguments</c> is parsed.
        /// </remarks>
        public bool TryLoadDictionary(IDictionaryLoader loader, string arguments, out Exception? error)
        {
            _ = loader ?? throw new ArgumentNullException(nameof(loader));

            try
            {
                this.Dictionary = loader.Load(this.ParseArgumentString(arguments ?? ""));
                error = null;
                return true;
            }
            catch (Exception ex)
            {
                error = ex;
                this.Dictionary = new EmptyDictionary();
                return false;
            }
        }

        /// <summary>
        /// Attempts to load a dictionary using the <c>IDictionaryLoader</c> and the given arguments.
        /// </summary>
        /// <param name="loader">The IDictionaryLoader to load the dictionary with.</param>
        /// <param name="arguments">The arguments to pass to the IDictionaryLoader.</param>
        /// <param name="error">The error which occured while loading the dictionary (if any).</param>
        /// <returns>True if the dictionary loaded successfully, false otherwise (and sets the <c>error</c> out parameter to the error).</returns>
        public bool TryLoadDictionary(IDictionaryLoader loader, IDictionary<string, string> arguments, out Exception? error)
        {
            _ = loader ?? throw new ArgumentNullException(nameof(loader));
            _ = arguments ?? throw new ArgumentNullException(nameof(arguments));

            try
            {
                this.Dictionary = loader.Load(arguments);
                error = null;
                return true;
            }
            catch (Exception ex)
            {
                error = ex;
                this.Dictionary = new EmptyDictionary();
                return false;
            }
        }

        private IDictionary<string, string> ParseArgumentString(string arguments)
        {
            return arguments.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(kvp => kvp.Split(new[] { '=' }))
                .Select(pair =>
                    {
                        if (pair.Length >= 2)
                            return new KeyValuePair<string, string>((pair[0] ?? "").Trim(), (pair[1] ?? "").Trim());
                        else if (pair.Length == 1)
                            return new KeyValuePair<string, string>((pair[0] ?? "").Trim(), "");
                        else if (pair.Length == 0)
                            return new KeyValuePair<string, string>("", "");
                        else
                            throw new Exception("Unexpected number of items when splitting argument string.");
                    })
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public void SetDictionary(WordDictionary dict)
        {
            if (dict == null)
                throw new ArgumentNullException("dict");
            this.Dictionary = dict;
        }
        #endregion

        #region CalculateCombinations
        /// <summary>
        /// Calculates the number of possible combinations of phrases based on the current dictionary and given phrase strength.
        /// </summary>
        public PhraseCombinations CalculateCombinations(PhraseStrength strength)
        {
            if (Clause.RandomMappings.ContainsKey(strength))
                return this.CalculateCombinations(Clause.RandomMappings[strength]);
            else
                return this.CalculateCombinations(Clause.CreatePhraseDescription(strength, this.Randomness));
        }
        /// <summary>
        /// Calculates the number of possible combinations of phrases based on the current dictionary and randomly choosing between the given phrase strengths.
        /// </summary>
        public PhraseCombinations CalculateCombinations(IEnumerable<PhraseStrength> strengths)
        {
            _ = strengths ?? throw new ArgumentNullException(nameof(strengths));

            if (strengths.Any(s => Clause.RandomMappings.ContainsKey(s) || s == PhraseStrength.Custom))
                throw new ArgumentException("Random or Custom phrase strengths must be passed to the singular version.");

            // Check all strengths and report min / max. 
            // Avg is somewhat meaningless, but we average the log of it anyway.
            double min = Double.MaxValue, max = 0.0, acc = 0.0;
            foreach (var s in strengths)
            {
                var comb = this.CalculateCombinations(Clause.CreatePhraseDescription(s, this.Randomness));
                min = Math.Min(min, comb.Shortest);
                max += comb.Longest;
                acc += comb.OptionalAverageAsEntropyBits;       // Max adds because of variations between phrases.
            }
            return new PhraseCombinations(min, max, Math.Pow(2, acc / strengths.Count()));        
        }

        /// <summary>
        /// Calculates the number of possible combinations of phrases based on the current dictionary and given phrase description.
        /// </summary>
        public PhraseCombinations CalculateCombinations(IEnumerable<Clause> phraseDescription)
        {
            _ = phraseDescription ?? throw new ArgumentNullException(nameof(phraseDescription));

            // Multiply all the combinations together.
            if (phraseDescription == null || !phraseDescription.Any())
                return PhraseCombinations.Zero;
            return phraseDescription
                    .Select(x => x.CountCombinations(this.Dictionary))
                    .Aggregate((accumulator, next) => accumulator * next);
        }
        #endregion

        #region GenerateAsSecure()
        /// <summary>
        /// Generates a single phrase as a <c>SecureString</c> based on <c>PasswordStrength.Random</c>.
        /// This is the slowest and most secure method.
        /// </summary>
        public SecureString GenerateAsSecure()
        {
            return GenerateAsSecure(Clause.CreatePhraseDescription(PhraseStrength.Random, Randomness), " ");
        }
        /// <summary>
        /// Generates a single phrase as a <c>SecureString</c> based on the given phrase strength.
        /// This is the slowest and most secure method.
        /// </summary>
        public SecureString GenerateAsSecure(PhraseStrength strength)
        {
            return GenerateAsSecure(Clause.CreatePhraseDescription(strength, Randomness), " ");
        }
        /// <summary>
        /// Generates a single phrase as a <c>SecureString</c> based on a randomly selected phrase strength.
        /// This is the slowest and most secure method.
        /// </summary>
        public SecureString GenerateAsSecure(IEnumerable<PhraseStrength> strengths)
        {
            return GenerateAsSecure(strengths, " ");
        }
        /// <summary>
        /// Generates a single phrase as a <c>SecureString</c> based on the given phrase description.
        /// This is the slowest and most secure method.
        /// </summary>
        public SecureString GenerateAsSecure(IEnumerable<Clause> phraseDescription)
        {
            return GenerateAsSecure(phraseDescription, " ");
        }
        /// <summary>
        /// Generates a single phrase as a <c>SecureString</c> based on the given phrase strength.
        /// This is the slowest and most secure method.
        /// </summary>
        /// <param name="strength">One of the predefined <c>PhraseStrength</c> enumeration members.</param>
        /// <param name="includeSpacesBetweenWords">Include spaces between words (defaults to true).</param>
        public SecureString GenerateAsSecure(PhraseStrength strength, bool includeSpacesBetweenWords)
        {
            return GenerateAsSecure(Clause.CreatePhraseDescription(strength, Randomness), includeSpacesBetweenWords);
        }
        /// <summary>
        /// Generates a single phrase as a <c>SecureString</c> based on the given phrase strength.
        /// This is the slowest and most secure method.
        /// </summary>
        /// <param name="strength">One of the predefined <c>PhraseStrength</c> enumeration members.</param>
        /// <param name="wordDelimiter">The string to place between each word in the passphrase.</param>
        public SecureString GenerateAsSecure(PhraseStrength strength, string wordDelimiter)
        {
            return GenerateAsSecure(Clause.CreatePhraseDescription(strength, Randomness), wordDelimiter);
        }
        /// <summary>
        /// Generates a single phrase as a <c>SecureString</c> based on the given phrase strength.
        /// This is the slowest and most secure method.
        /// </summary>
        /// <param name="strengths">A collection of the predefined <c>PhraseStrength</c> enumeration members to choose between at random.</param>
        /// <param name="includeSpacesBetweenWords">Include spaces between words (defaults to true).</param>
        public SecureString GenerateAsSecure(IEnumerable<PhraseStrength> strengths, bool includeSpacesBetweenWords)
        {
            _ = strengths ?? throw new ArgumentNullException(nameof(strengths));

            if (strengths.Any(s => Clause.RandomMappings.ContainsKey(s) || s == PhraseStrength.Custom))
                throw new ArgumentException("Random or Custom phrase strengths must be passed to the singular version.");
            var strength = this.ChooseAtRandom(strengths);
            return GenerateAsSecure(Clause.CreatePhraseDescription(strength, this.Randomness), includeSpacesBetweenWords);
        }
        /// <summary>
        /// Generates a single phrase as a <c>SecureString</c> based on the given phrase strength.
        /// This is the slowest and most secure method.
        /// </summary>
        /// <param name="strengths">A collection of the predefined <c>PhraseStrength</c> enumeration members to choose between at random.</param>
        /// <param name="wordDelimiter">The string to place between each word in the passphrase.</param>
        public SecureString GenerateAsSecure(IEnumerable<PhraseStrength> strengths, string wordDelimiter)
        {
            _ = strengths ?? throw new ArgumentNullException(nameof(strengths));

            if (strengths.Any(s => Clause.RandomMappings.ContainsKey(s) || s == PhraseStrength.Custom))
                throw new ArgumentException("Random or Custom phrase strengths must be passed to the singular version.");
            var strength = this.ChooseAtRandom(strengths);
            return GenerateAsSecure(Clause.CreatePhraseDescription(strength, this.Randomness), wordDelimiter);
        }
        /// <summary>
        /// Generates a single phrase as a <c>SecureString</c> based on the given phrase description.
        /// This is the slowest and most secure method.
        /// </summary>
        /// <param name="phraseDescription">One or more <c>Clause</c> objects defineing the details of the phrase.</param>
        /// <param name="includeSpacesBetweenWords">Include spaces between words (defaults to true).</param>
        public SecureString GenerateAsSecure(IEnumerable<Clause> phraseDescription, bool includeSpacesBetweenWords)
        {
            return this.GenerateAsSecure(phraseDescription, includeSpacesBetweenWords ? " " : "");
        }
        /// <summary>
        /// Generates a single phrase as a <c>SecureString</c> based on the given phrase description.
        /// This is the slowest and most secure method.
        /// </summary>
        /// <param name="phraseDescription">One or more <c>Clause</c> objects defineing the details of the phrase.</param>
        /// <param name="wordDelimiter">The string to place between each word in the passphrase.</param>
        public SecureString GenerateAsSecure(IEnumerable<Clause> phraseDescription, string wordDelimiter)
        {
            if (phraseDescription == null)
                throw new ArgumentNullException("phraseDescription");
            wordDelimiter = wordDelimiter ?? "";

            var result = new GenerateInSecureString();
            this.GenerateInternal(phraseDescription, wordDelimiter, result);
            if (wordDelimiter.Length > 0)
                // When a delimiter is included between words there is always a trailing one. Remove it.
                result.Target.RemoveAt(result.Target.Length - wordDelimiter.Length);
            result.Target.MakeReadOnly();
            return result.Target;
        }
        #endregion

        #region Generate()
        /// <summary>
        /// Generates a single phrase based on the given phrase strength in a <c>StringBuilder</c>.
        /// This is the fastest and least secure method.
        /// </summary>
        /// <param name="strength">One of the predefined <c>PhraseStrength</c> enumeration members (default: Random).</param>
        /// <param name="wordDelimiter">The string to place between each word in the passphrase (default: single space).</param>
        /// <param name="mutators">Applies one or more mutators to the passphrase after it is generated (default: none).</param>
        public String Generate(PhraseStrength strength = PhraseStrength.Random, string wordDelimiter = " ", IEnumerable<IMutator>? mutators = null)
        {
            return Generate(Clause.CreatePhraseDescription(strength, Randomness), wordDelimiter, mutators);
        }
        /// <summary>
        /// Generates a single phrase based on a randomly chosen phrase strength in a <c>StringBuilder</c>.
        /// This is the fastest and least secure method.
        /// </summary>
        /// <param name="strengths">A collection of the predefined <c>PhraseStrength</c> enumeration members to choose between at random.</param>
        /// <param name="wordDelimiter">The string to place between each word in the passphrase (default: single space).</param>
        /// <param name="mutators">Applies one or more mutators to the passphrase after it is generated (default: none).</param>
        public String Generate(IEnumerable<PhraseStrength> strengths, string wordDelimiter = " ", IEnumerable<IMutator>? mutators = null)
        {
            _ = strengths ?? throw new ArgumentNullException(nameof(strengths));

            if (strengths.Any(s => Clause.RandomMappings.ContainsKey(s) || s == PhraseStrength.Custom))
                throw new ArgumentException("Random or Custom phrase strengths must be passed to the singular version.");
            var strength = this.ChooseAtRandom(strengths);
            return Generate(Clause.CreatePhraseDescription(strength, Randomness), wordDelimiter, mutators);
        }
        /// <summary>
        /// Generates a single phrase based on the given phrase description in a <c>StringBuilder</c>.
        /// This is the fastest and least secure method.
        /// </summary>
        /// <param name="phraseDescription">One or more <c>Clause</c> objects defineing the details of the phrase.</param>
        /// <param name="wordDelimiter">The string to place between each word in the passphrase (default: single space).</param>
        /// <param name="mutators">Applies one or more mutators to the passphrase after it is generated (default: none).</param>
        public String Generate(IEnumerable<Clause> phraseDescription, string wordDelimiter = " ", IEnumerable<IMutator>? mutators = null)
        {
            if (phraseDescription == null)
                throw new ArgumentNullException("phraseDescription");

            var str = new GenerateInStringBuilder();

            // Mutators rely on whitespace to do their work, so we use a space if mutators are specified.
            var anyMutators = (mutators != null && mutators.Any());
            this.GenerateInternal(phraseDescription, anyMutators ? " " : wordDelimiter, str);

            foreach (var m in mutators ?? Enumerable.Empty<IMutator>())
            {
                m.Mutate(str.Target, this.Randomness);
                // We assume trailing whitespace when applying mutators, and in a few lines.
                // A buggy mutator may remove that whitespace, so ensure its there.
                if (str.Target[str.Target.Length - 1] != ' ')
                    str.Target.Append(" ");
            }
            var result = str.Target.ToString();

            // Now we replace the space with the actual delimiter.
            result = result.Replace(" ", wordDelimiter);

            // A trailing delimiter is always included when spaces are between words.
            return result.Substring(0, result.Length - wordDelimiter.Length);         
        }
        #endregion

        #region GenerateAsUtf8Bytes()
        /// <summary>
        /// Generates a single phrase based on <c>PasswordStrength.Random</c> in a UTF8 <c>byte[]</c>.
        /// This is slightly slower than <c>Generate()</c> and allows deterministic destruction of the data, but is still unencrypted.
        /// </summary>
        public byte[] GenerateAsUtf8Bytes()
        {
            return GenerateAsUtf8Bytes(Clause.CreatePhraseDescription(PhraseStrength.Random, Randomness), " ");
        }
        /// <summary>
        /// Generates a single phrase based on the given phrase strength in a UTF8 <c>byte[]</c>.
        /// This is slightly slower than <c>Generate()</c> and allows deterministic destruction of the data, but is still unencrypted.
        /// </summary>
        public byte[] GenerateAsUtf8Bytes(PhraseStrength strength)
        {
            return GenerateAsUtf8Bytes(Clause.CreatePhraseDescription(strength, Randomness), " ");
        }
        /// <summary>
        /// Generates a single phrase based on the given phrase strength in a UTF8 <c>byte[]</c>.
        /// This is slightly slower than <c>Generate()</c> and allows deterministic destruction of the data, but is still unencrypted.
        /// </summary>
        /// <param name="strength">One of the predefined <c>PhraseStrength</c> enumeration members.</param>
        /// <param name="includeSpacesBetweenWords">Include spaces between words (defaults to true).</param>
        public byte[] GenerateAsUtf8Bytes(PhraseStrength strength, bool includeSpacesBetweenWords)
        {
            return GenerateAsUtf8Bytes(Clause.CreatePhraseDescription(strength, Randomness), includeSpacesBetweenWords);
        }
        /// <summary>
        /// Generates a single phrase based on the given phrase strength in a UTF8 <c>byte[]</c>.
        /// This is slightly slower than <c>Generate()</c> and allows deterministic destruction of the data, but is still unencrypted.
        /// </summary>
        /// <param name="strength">One of the predefined <c>PhraseStrength</c> enumeration members.</param>
        /// <param name="wordDelimiter">The string to place between each word in the passphrase.</param>
        public byte[] GenerateAsUtf8Bytes(PhraseStrength strength, string wordDelimiter)
        {
            return GenerateAsUtf8Bytes(Clause.CreatePhraseDescription(strength, Randomness), wordDelimiter);
        }
        /// <summary>
        /// Generates a single phrase based on a randomly selected phrase strength in a UTF8 <c>byte[]</c>.
        /// This is slightly slower than <c>Generate()</c> and allows deterministic destruction of the data, but is still unencrypted.
        /// </summary>
        /// <param name="strengths">A collection of the predefined <c>PhraseStrength</c> enumeration members to choose between at random.</param>
        public byte[] GenerateAsUtf8Bytes(IEnumerable<PhraseStrength> strengths)
        {
            return this.GenerateAsUtf8Bytes(strengths, " ");
        }
        /// <summary>
        /// Generates a single phrase based on a randomly selected phrase strength in a UTF8 <c>byte[]</c>.
        /// This is slightly slower than <c>Generate()</c> and allows deterministic destruction of the data, but is still unencrypted.
        /// </summary>
        /// <param name="strengths">A collection of the predefined <c>PhraseStrength</c> enumeration members to choose between at random.</param>
        /// <param name="includeSpacesBetweenWords">Include spaces between words (defaults to true).</param>
        public byte[] GenerateAsUtf8Bytes(IEnumerable<PhraseStrength> strengths, bool includeSpacesBetweenWords)
        {
            _ = strengths ?? throw new ArgumentNullException(nameof(strengths));

            if (strengths.Any(s => Clause.RandomMappings.ContainsKey(s) || s == PhraseStrength.Custom))
                throw new ArgumentException("Random or Custom phrase strengths must be passed to the singular version.");
            var strength = this.ChooseAtRandom(strengths);
            return GenerateAsUtf8Bytes(Clause.CreatePhraseDescription(strength, Randomness), includeSpacesBetweenWords);
        }
        /// <summary>
        /// Generates a single phrase based on a randomly selected phrase strength in a UTF8 <c>byte[]</c>.
        /// This is slightly slower than <c>Generate()</c> and allows deterministic destruction of the data, but is still unencrypted.
        /// </summary>
        /// <param name="strengths">A collection of the predefined <c>PhraseStrength</c> enumeration members to choose between at random.</param>
        /// <param name="wordDelimiter">The string to place between each word in the passphrase.</param>
        public byte[] GenerateAsUtf8Bytes(IEnumerable<PhraseStrength> strengths, string wordDelimiter)
        {
            _ = strengths ?? throw new ArgumentNullException(nameof(strengths));

            if (strengths.Any(s => Clause.RandomMappings.ContainsKey(s) || s == PhraseStrength.Custom))
                throw new ArgumentException("Random or Custom phrase strengths must be passed to the singular version.");
            var strength = this.ChooseAtRandom(strengths);
            return GenerateAsUtf8Bytes(Clause.CreatePhraseDescription(strength, Randomness), wordDelimiter);
        }
        /// <summary>
        /// Generates a single phrase based on the given phrase description in a UTF8 <c>byte[]</c>.
        /// This is slightly slower than <c>Generate()</c> and allows deterministic destruction of the data, but is still unencrypted.
        /// </summary>
        public byte[] GenerateAsUtf8Bytes(IEnumerable<Clause> phraseDescription)
        {
            return GenerateAsUtf8Bytes(phraseDescription, " ");
        }
        /// <summary>
        /// Generates a single phrase based on the given phrase description in a UTF8 <c>byte[]</c>.
        /// This is slightly slower than <c>Generate()</c> and allows deterministic destruction of the data, but is still unencrypted.
        /// </summary>
        /// <param name="phraseDescription">One or more <c>Clause</c> objects defineing the details of the phrase.</param>
        /// <param name="includeSpacesBetweenWords">Include spaces between words (defaults to true).</param>
        public byte[] GenerateAsUtf8Bytes(IEnumerable<Clause> phraseDescription, bool includeSpacesBetweenWords)
        {
            return this.GenerateAsUtf8Bytes(phraseDescription, includeSpacesBetweenWords ? " " : "");
        }
        /// <summary>
        /// Generates a single phrase based on the given phrase description in a UTF8 <c>byte[]</c>.
        /// This is slightly slower than <c>Generate()</c> and allows deterministic destruction of the data, but is still unencrypted.
        /// </summary>
        /// <param name="phraseDescription">One or more <c>Clause</c> objects defineing the details of the phrase.</param>
        /// <param name="wordDelimiter">The string to place between each word in the passphrase.</param>
        public byte[] GenerateAsUtf8Bytes(IEnumerable<Clause> phraseDescription, string wordDelimiter)
        {
            if (phraseDescription == null)
                throw new ArgumentNullException("phraseDescription");
            wordDelimiter = wordDelimiter ?? "";

            var result = new GenerateInUtf8ByteArray();
            this.GenerateInternal(phraseDescription, wordDelimiter, result);
            if (wordDelimiter.Length > 0)
                // A trailing space is always included when spaces are between words.
                return result.Target.Take(result.Target.Length - wordDelimiter.Length).ToArray();
            else
                return result.Target;
        }
        #endregion

        #region Internal Generate Methods
        private void GenerateInternal(IEnumerable<Clause> phraseDescription, string wordDelimiter, GenerateTarget result)
        {
            if (phraseDescription == null)
                throw new ArgumentNullException("phraseDescription");
            if (result == null)
                throw new ArgumentNullException("result");
            if (this.Dictionary == null || this.Dictionary.Count == 0)
                throw new InvalidOperationException("You must call LoadDictionary() before any Generate() method.");

            // Build a detailed template by translating the clauses to something which is 1:1 with words. 
            var template = this.PhrasesToTemplate(phraseDescription);

            // Build the phrase based on that template.
            this.TemplateToWords(template, result, wordDelimiter ?? "");
        }
        private IEnumerable<Template> PhrasesToTemplate(IEnumerable<Clause> phrases)
        {
            // Apply gramatical rules of various kinds.
            var phraseList = phrases.ToList();      // NOTE: Anything more complicated and this will need a tree, possibly a trie.

            // If there's no verb, we use different logic.
            if (!phraseList.OfType<VerbClause>().Any())
                return this.PhrasesToTemplateWithoutVerb(phrases);

            // Turn the high level phrases into word templates.
            var result = new List<Template>();

            // Link NounClauses to VerbClauses.
            foreach (var clause in phraseList)
                clause.InitialiseRelationships(phraseList);
            foreach (var verb in phraseList.OfType<VerbClause>())
            {
                var thisPhraseTemplate = new List<Template>();
                var toProcess = verb.Subject.Concat(new Clause[] { verb }).Concat(verb.Object);       // Give the processing a logical order: subject, verb, object.

                // Process in specified order.
                foreach (var clause in toProcess)
                    clause.AddWordTemplate(Randomness, this.Dictionary, thisPhraseTemplate);

                // Process twice.
                foreach (var clause in toProcess)
                    clause.SecondPassOfWordTemplate(Randomness, this.Dictionary, thisPhraseTemplate);
                
                // Accumulate the whole phrase at the end.
                result.AddRange(thisPhraseTemplate);
            }
            return result;
        }
        private IEnumerable<Template> PhrasesToTemplateWithoutVerb(IEnumerable<Clause> phrases)
        {
            // If there's no verb, we just iterate over each clause.
            var result = new List<Template>();
            foreach (var clause in phrases)
                clause.AddWordTemplate(Randomness, this.Dictionary, result);
            return result;
        }
        private void TemplateToWords(IEnumerable<Template> template, GenerateTarget target, string wordDelimiter)
        {
            var chosenWords = new HashSet<Word>();
            ArticleTemplate? previousArticle = null;
            foreach (var t in template)
            {
                if (t.GetType() == typeof(ArticleTemplate))
                    // Can't directly append an article because it's form depends on the next word (whether it starts with a vowel sound or not).
                    previousArticle = (ArticleTemplate)t;
                else
                {
                    var tuple = t.ChooseWord(this.Dictionary, this.Randomness, chosenWords);
                    if (t.IncludeInAlreadyUsedList)
                        chosenWords.Add(tuple.Word);
                    
                    // Check for a previous article which must be returned before the current word.
                    if (previousArticle != null)
                    {
                        var w = previousArticle.ChooseBasedOnFollowingWord(this.Dictionary, tuple.FinalWord);
                        previousArticle = null;
                        this.AppendWord(target, wordDelimiter, w);
                    }

                    // Rather than returning IEnumerable<String>, we build directly into the target (which may be backed by either a StringBuilder or a SecureString).
                    // This interface is required by SecureString as we can only append Char to it and can't easily read its contents back.
                    this.AppendWord(target, wordDelimiter, tuple.FinalWord);
                }
            }
        }
        private void AppendWord(GenerateTarget target, string wordDelimiter, string word)
        {
            // Remember that some words in the dictionary are actually multiple words (verbs often have a helper verb with them).
            // So, if there are no spaces, we must remove spaces from words.
            // Also need to replace spaces in the word with our supplied word delimiter.

            var toAppend = word;
            toAppend = toAppend.Replace(" ", wordDelimiter);
            target.Append(toAppend);
            target.Append(wordDelimiter);
        }

        #endregion

        #region Helpers
        private PhraseStrength ChooseAtRandom(IEnumerable<PhraseStrength> strengths)
        {
            var choise = this.Randomness.Next(strengths.Count());
            var result = strengths.ElementAt(choise);
            return result;
        }
        #endregion
    }

    #region Internal GenerateTarget Classes
    internal abstract class GenerateTarget
    {
        public abstract void Append(char c);
        public abstract object Result { get; }
        public void Append(IEnumerable<Char> chars)
        {
            foreach (var c in chars)
                this.Append(c);
        }
    }
    internal class GenerateInStringBuilder : GenerateTarget
    {
        public readonly StringBuilder Target = new StringBuilder();
        public override object Result { get { return this.Target; } }
        public override void Append(char c)
        {
            this.Target.Append(c);
        }
    }
    internal class GenerateInSecureString : GenerateTarget
    {
        public readonly SecureString Target = new SecureString();
        public override object Result { get { return this.Target; } }
        public override void Append(char c)
        {
            this.Target.AppendChar(c);
        }
    }
    internal class GenerateInUtf8ByteArray : GenerateTarget
    {
        private readonly Encoding Utf8 = new UTF8Encoding(false);
        private readonly char[] TempChar = new char[1];
        private readonly List<byte> _Target = new List<byte>();
        public byte[] Target { get { return _Target.ToArray(); } }

        public override object Result { get { return this.Target; } }
        public override void Append(char c)
        {
            TempChar[0] = c;
            _Target.AddRange(Utf8.GetBytes(TempChar));
            TempChar[0] = '\0';
        }
    }
    #endregion
}
