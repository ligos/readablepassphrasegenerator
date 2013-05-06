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

namespace MurrayGrant.ReadablePassphrase
{
    /// <summary>
    /// Passphrase generator.
    /// </summary>
    public sealed class ReadablePassphraseGenerator
    {
        public WordDictionary Dictionary { get; private set; }
        public RandomSourceBase Randomness { get; private set; }

        public readonly static Uri CodeplexHomepage = new Uri("http://readablepassphrase.codeplex.com");

        #region Constructor
        /// <summary>
        /// Initialises the object with the default random source (based on <c>RNGCryptoServiceProvider</c>) and dictionary (internal XML dictionary).
        /// </summary>
        public ReadablePassphraseGenerator() 
            : this(new CryptoRandomSource())
        { 
        }
        /// <summary>
        /// Initialises the object with the given random source.
        /// </summary>
        public ReadablePassphraseGenerator(RandomSourceBase randomness) 
        {
            this.Randomness = randomness;
            this.Dictionary = new EmptyDictionary();          // Default empty dictionary,
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
            this.LoadDictionary(loader, this.ParseArgumentString(arguments));
        }
        /// <summary>
        /// Loads a dictionary using the <c>IDictionaryLoader</c> and the given arguments.
        /// </summary>
        /// <param name="loader">The IDictionaryLoader to load the dictionary with.</param>
        /// <param name="arguments">The arguments to pass to the IDictionaryLoader.</param>
        public void LoadDictionary(IDictionaryLoader loader, IDictionary<string, string> arguments)
        {
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
        public bool TryLoadDictionary(IDictionaryLoader loader, string arguments, out Exception error)
        {
            try
            {
                this.Dictionary = loader.Load(this.ParseArgumentString(arguments));
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
        public bool TryLoadDictionary(IDictionaryLoader loader, IDictionary<string, string> arguments, out Exception error)
        {
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
                            throw new ApplicationException("Unexpected number of items when splitting argument string.");
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
            return GenerateAsSecure(Clause.CreatePhraseDescription(PhraseStrength.Random, Randomness), true);
        }
        /// <summary>
        /// Generates a single phrase as a <c>SecureString</c> based on the given phrase strength.
        /// This is the slowest and most secure method.
        /// </summary>
        public SecureString GenerateAsSecure(PhraseStrength strength)
        {
            return GenerateAsSecure(Clause.CreatePhraseDescription(strength, Randomness), true);
        }
        /// <summary>
        /// Generates a single phrase as a <c>SecureString</c> based on a randomly selected phrase strength.
        /// This is the slowest and most secure method.
        /// </summary>
        public SecureString GenerateAsSecure(IEnumerable<PhraseStrength> strengths)
        {
            return GenerateAsSecure(strengths, true);
        }
        /// <summary>
        /// Generates a single phrase as a <c>SecureString</c> based on the given phrase description.
        /// This is the slowest and most secure method.
        /// </summary>
        public SecureString GenerateAsSecure(IEnumerable<Clause> phraseDescription)
        {
            return GenerateAsSecure(phraseDescription, true);
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
        /// <param name="strengths">A collection of the predefined <c>PhraseStrength</c> enumeration members to choose between at random.</param>
        /// <param name="includeSpacesBetweenWords">Include spaces between words (defaults to true).</param>
        public SecureString GenerateAsSecure(IEnumerable<PhraseStrength> strengths, bool includeSpacesBetweenWords)
        {
            if (strengths.Any(s => Clause.RandomMappings.ContainsKey(s) || s == PhraseStrength.Custom))
                throw new ArgumentException("Random or Custom phrase strengths must be passed to the singular version.");
            var strength = this.ChooseAtRandom(strengths);
            return GenerateAsSecure(Clause.CreatePhraseDescription(strength, this.Randomness), includeSpacesBetweenWords);
        }
        /// <summary>
        /// Generates a single phrase as a <c>SecureString</c> based on the given phrase description.
        /// This is the slowest and most secure method.
        /// </summary>
        /// <param name="phraseDescription">One or more <c>Clause</c> objects defineing the details of the phrase.</param>
        /// <param name="includeSpacesBetweenWords">Include spaces between words (defaults to true).</param>
        public SecureString GenerateAsSecure(IEnumerable<Clause> phraseDescription, bool includeSpacesBetweenWords)
        {
            if (phraseDescription == null)
                throw new ArgumentNullException("phraseDescription");

            var result = new GenerateInSecureString();
            this.GenerateInternal(phraseDescription, includeSpacesBetweenWords, result);
            if (includeSpacesBetweenWords)
                // When spaces are included between words there is always a trailing space. Remove it.
                result.Target.RemoveAt(result.Target.Length - 1);
            result.Target.MakeReadOnly();
            return result.Target;
        }
        #endregion

        #region Generate()
        /// <summary>
        /// Generates a single phrase based on <c>PasswordStrength.Random</c> in a <c>StringBuilder</c>.
        /// This is the fastest and least secure method.
        /// </summary>
        public String Generate()
        {
            return Generate(Clause.CreatePhraseDescription(PhraseStrength.Random, Randomness));
        }
        /// <summary>
        /// Generates a single phrase based on the given phrase strength in a <c>StringBuilder</c>.
        /// This is the fastest and least secure method.
        /// </summary>
        public String Generate(PhraseStrength strength)
        {
            return Generate(Clause.CreatePhraseDescription(strength, Randomness), true);
        }
        /// <summary>
        /// Generates a single phrase based on the given phrase strength in a <c>StringBuilder</c>.
        /// This is the fastest and least secure method.
        /// </summary>
        /// <param name="strength">One of the predefined <c>PhraseStrength</c> enumeration members.</param>
        /// <param name="includeSpacesBetweenWords">Include spaces between words (defaults to true).</param>
        public String Generate(PhraseStrength strength, bool includeSpacesBetweenWords)
        {
            return Generate(Clause.CreatePhraseDescription(strength, Randomness), includeSpacesBetweenWords);
        }
        /// <summary>
        /// Generates a single phrase based on a randomly chosen phrase strength in a <c>StringBuilder</c>.
        /// This is the fastest and least secure method.
        /// </summary>
        /// <param name="strengths">A collection of the predefined <c>PhraseStrength</c> enumeration members to choose between at random.</param>
        public String Generate(IEnumerable<PhraseStrength> strengths)
        {
            return this.Generate(strengths, true);
        }
        /// <summary>
        /// Generates a single phrase based on a randomly chosen phrase strength in a <c>StringBuilder</c>.
        /// This is the fastest and least secure method.
        /// </summary>
        /// <param name="strengths">A collection of the predefined <c>PhraseStrength</c> enumeration members to choose between at random.</param>
        /// <param name="includeSpacesBetweenWords">Include spaces between words (defaults to true).</param>
        public String Generate(IEnumerable<PhraseStrength> strengths, bool includeSpacesBetweenWords)
        {
            if (strengths.Any(s => Clause.RandomMappings.ContainsKey(s) || s == PhraseStrength.Custom))
                throw new ArgumentException("Random or Custom phrase strengths must be passed to the singular version.");
            var strength = this.ChooseAtRandom(strengths);
            return Generate(Clause.CreatePhraseDescription(strength, Randomness), includeSpacesBetweenWords);
        }
        /// <summary>
        /// Generates a single phrase based on the given phrase description in a <c>StringBuilder</c>.
        /// This is the fastest and least secure method.
        /// </summary>
        public String Generate(IEnumerable<Clause> phraseDescription)
        {
            return Generate(phraseDescription, true);
        }
        /// <summary>
        /// Generates a single phrase based on the given phrase description in a <c>StringBuilder</c>.
        /// This is the fastest and least secure method.
        /// </summary>
        /// <param name="phraseDescription">One or more <c>Clause</c> objects defineing the details of the phrase.</param>
        /// <param name="includeSpacesBetweenWords">Include spaces between words (defaults to true).</param>
        public String Generate(IEnumerable<Clause> phraseDescription, bool includeSpacesBetweenWords)
        {
            if (phraseDescription == null)
                throw new ArgumentNullException("phraseDescription");

            var result = new GenerateInStringBuilder();
            this.GenerateInternal(phraseDescription, includeSpacesBetweenWords, result);
            return result.Target.ToString().Trim();         // A trailing space is always included when spaces are between words.
        }
        #endregion

        #region GenerateAsUtf8Bytes()
        /// <summary>
        /// Generates a single phrase based on <c>PasswordStrength.Random</c> in a UTF8 <c>byte[]</c>.
        /// This is slightly slower than <c>Generate()</c> and allows deterministic destruction of the data, but is still unencrypted.
        /// </summary>
        public byte[] GenerateAsUtf8Bytes()
        {
            return GenerateAsUtf8Bytes(Clause.CreatePhraseDescription(PhraseStrength.Random, Randomness));
        }
        /// <summary>
        /// Generates a single phrase based on the given phrase strength in a UTF8 <c>byte[]</c>.
        /// This is slightly slower than <c>Generate()</c> and allows deterministic destruction of the data, but is still unencrypted.
        /// </summary>
        public byte[] GenerateAsUtf8Bytes(PhraseStrength strength)
        {
            return GenerateAsUtf8Bytes(Clause.CreatePhraseDescription(strength, Randomness), true);
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
        /// Generates a single phrase based on a randomly selected phrase strength in a UTF8 <c>byte[]</c>.
        /// This is slightly slower than <c>Generate()</c> and allows deterministic destruction of the data, but is still unencrypted.
        /// </summary>
        /// <param name="strengths">A collection of the predefined <c>PhraseStrength</c> enumeration members to choose between at random.</param>
        public byte[] GenerateAsUtf8Bytes(IEnumerable<PhraseStrength> strengths)
        {
            return this.GenerateAsUtf8Bytes(strengths, true);
        }
        /// <summary>
        /// Generates a single phrase based on a randomly selected phrase strength in a UTF8 <c>byte[]</c>.
        /// This is slightly slower than <c>Generate()</c> and allows deterministic destruction of the data, but is still unencrypted.
        /// </summary>
        /// <param name="strengths">A collection of the predefined <c>PhraseStrength</c> enumeration members to choose between at random.</param>
        /// <param name="includeSpacesBetweenWords">Include spaces between words (defaults to true).</param>
        public byte[] GenerateAsUtf8Bytes(IEnumerable<PhraseStrength> strengths, bool includeSpacesBetweenWords)
        {
            if (strengths.Any(s => Clause.RandomMappings.ContainsKey(s) || s == PhraseStrength.Custom))
                throw new ArgumentException("Random or Custom phrase strengths must be passed to the singular version.");
            var strength = this.ChooseAtRandom(strengths);
            return GenerateAsUtf8Bytes(Clause.CreatePhraseDescription(strength, Randomness), includeSpacesBetweenWords);
        }
        /// <summary>
        /// Generates a single phrase based on the given phrase description in a UTF8 <c>byte[]</c>.
        /// This is slightly slower than <c>Generate()</c> and allows deterministic destruction of the data, but is still unencrypted.
        /// </summary>
        public byte[] GenerateAsUtf8Bytes(IEnumerable<Clause> phraseDescription)
        {
            return GenerateAsUtf8Bytes(phraseDescription, true);
        }
        /// <summary>
        /// Generates a single phrase based on the given phrase description in a UTF8 <c>byte[]</c>.
        /// This is slightly slower than <c>Generate()</c> and allows deterministic destruction of the data, but is still unencrypted.
        /// </summary>
        /// <param name="phraseDescription">One or more <c>Clause</c> objects defineing the details of the phrase.</param>
        /// <param name="includeSpacesBetweenWords">Include spaces between words (defaults to true).</param>
        public byte[] GenerateAsUtf8Bytes(IEnumerable<Clause> phraseDescription, bool includeSpacesBetweenWords)
        {
            if (phraseDescription == null)
                throw new ArgumentNullException("phraseDescription");

            var result = new GenerateInUtf8ByteArray();
            this.GenerateInternal(phraseDescription, includeSpacesBetweenWords, result);
            if (includeSpacesBetweenWords)
                // A trailing space is always included when spaces are between words.
                return result.Target.Take(result.Target.Length - 1).ToArray();
            else
                return result.Target;
        }
        #endregion

        #region Internal Generate Methods
        private void GenerateInternal(IEnumerable<Clause> phraseDescription, bool spacesBetweenWords, GenerateTarget result)
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
            this.TemplateToWords(template, result, spacesBetweenWords);
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
        private void TemplateToWords(IEnumerable<Template> template, GenerateTarget target, bool spacesBetweenWords)
        {
            var chosenWords = new HashSet<Word>();
            ArticleTemplate previousArticle = null;
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
                        this.AppendWord(target, spacesBetweenWords, w);
                    }

                    // Rather than returning IEnumerable<String>, we build directly into the target (which may be backed by either a StringBuilder or a SecureString).
                    // This interface is required by SecureString as we can only append Char to it and can't easily read its contents back.
                    this.AppendWord(target, spacesBetweenWords, tuple.FinalWord);
                }
            }
        }
        private void AppendWord(GenerateTarget target, bool spacesBetweenWords, string word)
        {
            // Remember that some words in the dictionary are actually multiple words (verbs often have a helper verb with them).
            // So, if there are no spaces, we must remove spaces from words.
            if (spacesBetweenWords)
            {
                target.Append(word);
                target.Append(' ');
            }
            else
            {
                target.Append(word.Replace(" ", ""));
            }

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
