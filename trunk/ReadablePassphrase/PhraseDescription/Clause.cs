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
using MurrayGrant.ReadablePassphrase.Dictionaries;

namespace MurrayGrant.ReadablePassphrase.PhraseDescription
{
    /// <summary>
    /// Each clause is the basic building block of passphrases.
    /// </summary>
    public abstract class Clause
    {
        static Clause()
        {
            PredefinedPhraseDescriptions = new Dictionary<PhraseStrength, IEnumerable<Clause>>();
            PredefinedPhraseDescriptions.Add(PhraseStrength.Normal, CreatePhraseDescriptionForNormal());
            PredefinedPhraseDescriptions.Add(PhraseStrength.NormalAnd, CreatePhraseDescriptionForNormalAnd());
            PredefinedPhraseDescriptions.Add(PhraseStrength.NormalSpeech, CreatePhraseDescriptionForNormalSpeech());
            PredefinedPhraseDescriptions.Add(PhraseStrength.NormalEqual, CreatePhraseDescriptionForNormalEqual());
            PredefinedPhraseDescriptions.Add(PhraseStrength.NormalEqualAnd, CreatePhraseDescriptionForNormalEqualAnd());
            PredefinedPhraseDescriptions.Add(PhraseStrength.NormalEqualSpeech, CreatePhraseDescriptionForNormalEqualSpeech());
            PredefinedPhraseDescriptions.Add(PhraseStrength.NormalRequired, CreatePhraseDescriptionForNormalRequired());
            PredefinedPhraseDescriptions.Add(PhraseStrength.NormalRequiredAnd, CreatePhraseDescriptionForNormalRequiredAnd());
            PredefinedPhraseDescriptions.Add(PhraseStrength.NormalRequiredSpeech, CreatePhraseDescriptionForNormalRequiredSpeech());

            PredefinedPhraseDescriptions.Add(PhraseStrength.Strong, CreatePhraseDescriptionForStrong());
            PredefinedPhraseDescriptions.Add(PhraseStrength.StrongAnd, CreatePhraseDescriptionForStrongAnd());
            PredefinedPhraseDescriptions.Add(PhraseStrength.StrongSpeech, CreatePhraseDescriptionForStrongSpeech());
            PredefinedPhraseDescriptions.Add(PhraseStrength.StrongEqual, CreatePhraseDescriptionForStrongEqual());
            PredefinedPhraseDescriptions.Add(PhraseStrength.StrongEqualAnd, CreatePhraseDescriptionForStrongEqualAnd());
            PredefinedPhraseDescriptions.Add(PhraseStrength.StrongEqualSpeech, CreatePhraseDescriptionForStrongEqualSpeech());
            PredefinedPhraseDescriptions.Add(PhraseStrength.StrongRequired, CreatePhraseDescriptionForStrongRequired());
            PredefinedPhraseDescriptions.Add(PhraseStrength.StrongRequiredAnd, CreatePhraseDescriptionForStrongRequiredAnd());
            PredefinedPhraseDescriptions.Add(PhraseStrength.StrongRequiredSpeech, CreatePhraseDescriptionForStrongRequiredSpeech());

            PredefinedPhraseDescriptions.Add(PhraseStrength.Insane, CreatePhraseDescriptionForInsane());
            PredefinedPhraseDescriptions.Add(PhraseStrength.InsaneAnd, CreatePhraseDescriptionForInsaneAnd());
            PredefinedPhraseDescriptions.Add(PhraseStrength.InsaneSpeech, CreatePhraseDescriptionForInsaneSpeech());
            PredefinedPhraseDescriptions.Add(PhraseStrength.InsaneEqual, CreatePhraseDescriptionForInsaneEqual());
            PredefinedPhraseDescriptions.Add(PhraseStrength.InsaneEqualAnd, CreatePhraseDescriptionForInsaneEqualAnd());
            PredefinedPhraseDescriptions.Add(PhraseStrength.InsaneEqualSpeech, CreatePhraseDescriptionForInsaneEqualSpeech());
            PredefinedPhraseDescriptions.Add(PhraseStrength.InsaneRequired, CreatePhraseDescriptionForInsaneRequired());
            PredefinedPhraseDescriptions.Add(PhraseStrength.InsaneRequiredAnd, CreatePhraseDescriptionForInsaneRequiredAnd());
            PredefinedPhraseDescriptions.Add(PhraseStrength.InsaneRequiredSpeech, CreatePhraseDescriptionForInsaneRequiredSpeech());
        }

        /// <summary>
        /// True if this clause is present in the final result.
        /// </summary>
        public virtual bool IsPresent { get; protected set; }

        /// <summary>
        /// Relates each clause to others in the overall phrase.
        /// </summary>
        public abstract void InitialiseRelationships(IEnumerable<Clause> phrase);

        /// <summary>
        /// Builds a template of words for this clause. A template has 1:1 correspondance with actual words, but has not yet chosen them from the dictionary.
        /// </summary>
        public abstract void AddWordTemplate(Random.RandomSourceBase randomness, WordDictionary dictionary, IList<WordTemplate.Template> currentTemplate);

        /// <summary>
        /// 2nd pass of building a template of words for this clause. 
        /// </summary>
        public abstract void SecondPassOfWordTemplate(Random.RandomSourceBase randomness, WordDictionary dictionary, IList<WordTemplate.Template> currentTemplate);

        /// <summary>
        /// Counts the total unique combinations possible for this clause based on dictionary word counts and the clause's configuration.
        /// </summary>
        public abstract PhraseCombinations CountCombinations(WordDictionary dictionary);

        protected PhraseCombinations CountSingleFactor<T>(WordDictionary dictionary, int factor, int optionalFactor) where T : Words.Word
        {
            var count = dictionary.CountOf<T>();
            return this.CountSingleFactor(factor, optionalFactor, count);
        }
        protected PhraseCombinations CountSingleFactor(int factor, int optionalFactor, int count)
        {
            double shortResult = 1, longResult = 1, optionalAverage = 1;

            var isPresent = factor > 0;
            var isOptional = optionalFactor > 0;
            if (isPresent && count > 0)
            {
                if (isOptional)
                {
                    longResult *= count + 1;     // Plus the optional possiblility.
                    optionalAverage *= count * (1.0 - ((double)optionalFactor / (double)(factor + optionalFactor)));
                }
                else
                {
                    shortResult *= count;
                    longResult *= count;
                    optionalAverage = count;
                }
            }

            return new PhraseCombinations(shortResult, longResult, optionalAverage);
        }


        public String ToTextString()
        {
            var sb = new StringBuilder();
            this.ToStringBuilder(sb);
            return sb.ToString();
        }
        public void ToStringBuilder(StringBuilder sb)
        {
            var rootTag = (TagInConfigurationAttribute)this.GetType().GetCustomAttributes(typeof(TagInConfigurationAttribute), true).First();
            var groupedProperties = this.GetType().GetProperties()
                                            .Select(p => new PropertyAndAttributes { Property = p, Tag = (TagInConfigurationAttribute)p.GetCustomAttributes(typeof(TagInConfigurationAttribute), true).FirstOrDefault() })
                                            .Where(p => p.Tag != null)
                                            .GroupBy(p => p.Tag.Group)
                                            .OrderBy(p => p.Key);

            sb.AppendLine(rootTag.Name + " = {");
            foreach (var g in groupedProperties)
            {
                sb.Append(' ', 4);
                sb.AppendLine(String.Join(", ", g.Select(p => p.Tag.Name + "->" + p.Property.GetValue(this, null)).ToArray()) + ",");
            }
            sb.AppendLine(" }");
        }
        private class PropertyAndAttributes
        {
            public System.Reflection.PropertyInfo Property { get; set; }
            public TagInConfigurationAttribute Tag { get; set; }
        }

        private static readonly Type[] AllowedClauseTypes = new Type[] { typeof(NounClause), typeof(VerbClause), typeof(ConjunctionClause), typeof(DirectSpeechClause) };
        public static IEnumerable<Clause> CreateCollectionFromTextString(string s)
        {
            var result = new List<Clause>();
            if (String.IsNullOrEmpty(s))
                return result;

            // TODO: on parse failure, throw an exception. If possible, give an offset in the string where the error was detected.

            int startIdx = 0, equalsIdx;
            while ((equalsIdx = s.IndexOf('=', startIdx)) != -1)
            {
                // Find and create the correct clause instance.
                var tag = s.Substring(startIdx, equalsIdx - startIdx).Trim().ToLower();
                var type = AllowedClauseTypes.FirstOrDefault(t => ((TagInConfigurationAttribute)t.GetCustomAttributes(typeof(TagInConfigurationAttribute), true).First()).Name.ToLower() == tag);
                if (type == null)
                    throw new PhraseDescriptionParseException("Unknown clause type: " + tag, startIdx);
                var clause = (Clause)Activator.CreateInstance(type);
                
                // Assign properties.
                var properties = type.GetProperties()
                                    .Select(p => new PropertyAndAttributes { Property = p, Tag = (TagInConfigurationAttribute)p.GetCustomAttributes(typeof(TagInConfigurationAttribute), true).FirstOrDefault() })
                                    .Where(p => p.Tag != null)
                                    .ToDictionary(p => p.Tag.Name.ToLower());
                int propStartIdx = s.IndexOf('{', equalsIdx) + 1;
                int propEndIdx = s.IndexOf('}', propStartIdx);
                if (propStartIdx == -1)
                    throw new PhraseDescriptionParseException(String.Format("Cannot find '{' in '{0}' clause.", tag), equalsIdx);
                if (propEndIdx == -1)
                    throw new PhraseDescriptionParseException(String.Format("Cannot find '}' in '{0}' clause.", tag), equalsIdx);

                foreach (string kvp in s.Substring(propStartIdx, propEndIdx - propStartIdx)
                                        .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (kvp.Trim() == "")
                        continue;

                    var pair = kvp.Split(new string[] { "->" }, StringSplitOptions.RemoveEmptyEntries);
                    if (pair.Length != 2)
                        throw new PhraseDescriptionParseException(String.Format("Incomplete property pair in '{0}' clause: '{1}'.", tag, pair), propStartIdx);
                    var propName = pair[0].Trim().ToLower();
                    var propVal = pair[1].Trim().ToLower();
                    if (String.IsNullOrEmpty(propName))
                        throw new PhraseDescriptionParseException(String.Format("Empty property name in '{0}' clause: '{1}'.", tag, pair), propStartIdx);
                    if (String.IsNullOrEmpty(propVal))
                        throw new PhraseDescriptionParseException(String.Format("Empty property value in '{0}' clause, property '{1}'.", tag, propName), propStartIdx);

                    if (!properties.ContainsKey(propName))
                        throw new PhraseDescriptionParseException(String.Format("Property in '{0}' clause named '{1}' is unknown.", tag, propName), propStartIdx);
                    int propValAsInt;
                    if (!Int32.TryParse(propVal, out propValAsInt))
                        throw new PhraseDescriptionParseException(String.Format("Property in '{0}' clause named '{1}' is not a whole number.", tag, propName), propStartIdx);

                    try
                    {
                        properties[propName].Property.SetValue(clause, propValAsInt, null);
                    }
                    catch (Exception ex)
                    {
                        throw new PhraseDescriptionParseException(String.Format("Could not set property in '{0}' clause named '{1}' to value '{2}': {3}.", tag, propName, propVal, ex.Message), propStartIdx, ex);
                    }
                }

                startIdx = propEndIdx+1;
                result.Add(clause);
            }

            return result;
        }

        private static readonly IDictionary<PhraseStrength, IEnumerable<Clause>> PredefinedPhraseDescriptions;
        public static IEnumerable<Clause> CreatePhraseDescription(PhraseStrength strength)
        {
            if (!PredefinedPhraseDescriptions.ContainsKey(strength))
                throw new ArgumentException(string.Format("Unsupported default PhraseStrength '{0}'. To use Custom, create an IEnumerable<Clause>. To use Random, pass a RandomSourceBase implementation.", strength), "strength");
            return PredefinedPhraseDescriptions[strength];
        }
        /// <summary>
        /// Returns phrase description at random.
        /// </summary>
        public static IEnumerable<Clause> CreatePhraseDescription(Random.RandomSourceBase randomness)
        {
            var keys = PredefinedPhraseDescriptions.Keys.ToList();
            var choise = randomness.Next(keys.Count);
            var result = CreatePhraseDescription(keys[choise]);
            return result;
        }

        public static IEnumerable<Clause> CreatePhraseDescriptionForNormal()
        {
            return new List<Clause>()
                {
                    new NounClause() { CommonNounFactor = 12, ProperNounFactor = 1,
                                        SingularityFactor = 1, PluralityFactor = 0, 
                                        NoArticleFactor = 5, DefiniteArticleFactor = 4, IndefiniteArticleFactor = 4, DemonstractiveFactor = 0, PersonalPronounFactor = 2,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 0,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                    new VerbClause() { PresentFactor = 10, PastFactor = 8, FutureFactor = 8, ContinuousFactor = 0, ContinuousPastFactor = 0, PerfectFactor = 0, SubjunctiveFactor = 0,
                                        NoAdverbFactor = 1, AdverbFactor = 0,
                                        InterrogativeFactor = 1, NoInterrogativeFactor = 8, 
                                        IntransitiveByNoNounClauseFactor = 0, IntransitiveByPrepositionFactor = 0},
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 0,
                                        SingularityFactor = 1, PluralityFactor = 0, 
                                        NoArticleFactor = 5, DefiniteArticleFactor = 4, IndefiniteArticleFactor = 4, DemonstractiveFactor = 0, PersonalPronounFactor = 2,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 0,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                };
        }
        public static IEnumerable<Clause> CreatePhraseDescriptionForNormalAnd()
        {
            return new List<Clause>()
                {
                    new NounClause() { CommonNounFactor = 12, ProperNounFactor = 1,
                                        SingularityFactor = 1, PluralityFactor = 0, 
                                        NoArticleFactor = 5, DefiniteArticleFactor = 4, IndefiniteArticleFactor = 4, DemonstractiveFactor = 0, PersonalPronounFactor = 2,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 0,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                    new VerbClause() { PresentFactor = 10, PastFactor = 8, FutureFactor = 8, ContinuousFactor = 0, ContinuousPastFactor = 0, PerfectFactor = 0, SubjunctiveFactor = 0,
                                        NoAdverbFactor = 1, AdverbFactor = 0,
                                        InterrogativeFactor = 1, NoInterrogativeFactor = 8, 
                                        IntransitiveByNoNounClauseFactor = 0, IntransitiveByPrepositionFactor = 0},
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 0,
                                        SingularityFactor = 1, PluralityFactor = 0, 
                                        NoArticleFactor = 5, DefiniteArticleFactor = 4, IndefiniteArticleFactor = 4, DemonstractiveFactor = 0, PersonalPronounFactor = 2,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 0,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                    new ConjunctionClause() { JoiningNounFactor = 1, JoiningPhraseFactor = 0 },
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 0,
                                        SingularityFactor = 1, PluralityFactor = 0, 
                                        NoArticleFactor = 5, DefiniteArticleFactor = 4, IndefiniteArticleFactor = 4, DemonstractiveFactor = 0, PersonalPronounFactor = 2,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 0,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                };
        }
        public static IEnumerable<Clause> CreatePhraseDescriptionForNormalSpeech()
        {
            return new List<Clause>()
                {
                    new NounClause() { CommonNounFactor = 7, ProperNounFactor = 1,
                                        SingularityFactor = 1, PluralityFactor = 0, 
                                        NoArticleFactor = 0, DefiniteArticleFactor = 4, IndefiniteArticleFactor = 4, DemonstractiveFactor = 0, PersonalPronounFactor = 2,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 0,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                    new DirectSpeechClause() { NoDirectSpeechFactor = 0, DirectSpeechFactor = 1 },
                    new NounClause() { CommonNounFactor = 12, ProperNounFactor = 1,
                                        SingularityFactor = 1, PluralityFactor = 0, 
                                        NoArticleFactor = 5, DefiniteArticleFactor = 4, IndefiniteArticleFactor = 4, DemonstractiveFactor = 0, PersonalPronounFactor = 2,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 0,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                    new VerbClause() { PresentFactor = 10, PastFactor = 8, FutureFactor = 8, ContinuousFactor = 0, ContinuousPastFactor = 0, PerfectFactor = 0, SubjunctiveFactor = 0,
                                        NoAdverbFactor = 1, AdverbFactor = 0,
                                        InterrogativeFactor = 1, NoInterrogativeFactor = 8, 
                                        IntransitiveByNoNounClauseFactor = 0, IntransitiveByPrepositionFactor = 0},
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 0,
                                        SingularityFactor = 1, PluralityFactor = 0, 
                                        NoArticleFactor = 5, DefiniteArticleFactor = 4, IndefiniteArticleFactor = 4, DemonstractiveFactor = 0, PersonalPronounFactor = 2,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 0,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                };
        }
        public static IEnumerable<Clause> CreatePhraseDescriptionForNormalEqual()
        {
            return new List<Clause>()
                {
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 1,
                                        SingularityFactor = 1, PluralityFactor = 0, 
                                        NoArticleFactor = 1, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 0, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 0,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                    new VerbClause() { PresentFactor = 1, PastFactor = 1, FutureFactor = 1, ContinuousFactor = 0, ContinuousPastFactor = 0, PerfectFactor = 0, SubjunctiveFactor = 0,
                                        NoAdverbFactor = 1, AdverbFactor = 0,
                                        InterrogativeFactor = 1, NoInterrogativeFactor = 1, 
                                        IntransitiveByNoNounClauseFactor = 0, IntransitiveByPrepositionFactor = 0},
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 0,
                                        SingularityFactor = 1, PluralityFactor = 0, 
                                        NoArticleFactor = 1, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 0, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 0,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                };
        }
        public static IEnumerable<Clause> CreatePhraseDescriptionForNormalEqualAnd()
        {
            return new List<Clause>()
                {
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 1,
                                        SingularityFactor = 1, PluralityFactor = 0, 
                                        NoArticleFactor = 1, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 0, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 0,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                    new VerbClause() { PresentFactor = 1, PastFactor = 1, FutureFactor = 1, ContinuousFactor = 0, ContinuousPastFactor = 0, PerfectFactor = 0, SubjunctiveFactor = 0,
                                        NoAdverbFactor = 1, AdverbFactor = 0,
                                        InterrogativeFactor = 1, NoInterrogativeFactor = 1, 
                                        IntransitiveByNoNounClauseFactor = 0, IntransitiveByPrepositionFactor = 0},
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 0,
                                        SingularityFactor = 1, PluralityFactor = 0, 
                                        NoArticleFactor = 1, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 0, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 0,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                    new ConjunctionClause() { JoiningNounFactor = 1, JoiningPhraseFactor = 0 },
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 0,
                                        SingularityFactor = 1, PluralityFactor = 0, 
                                        NoArticleFactor = 1, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 0, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 0,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                };
        }
        public static IEnumerable<Clause> CreatePhraseDescriptionForNormalEqualSpeech()
        {
            return new List<Clause>()
                {
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 1,
                                        SingularityFactor = 1, PluralityFactor = 0, 
                                        NoArticleFactor = 0, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 0, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 0,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                    new DirectSpeechClause() { NoDirectSpeechFactor = 0, DirectSpeechFactor = 1 },
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 1,
                                        SingularityFactor = 1, PluralityFactor = 0, 
                                        NoArticleFactor = 1, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 0, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 0,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                    new VerbClause() { PresentFactor = 1, PastFactor = 1, FutureFactor = 1, ContinuousFactor = 0, ContinuousPastFactor = 0, PerfectFactor = 0, SubjunctiveFactor = 0,
                                        NoAdverbFactor = 1, AdverbFactor = 0,
                                        InterrogativeFactor = 1, NoInterrogativeFactor = 1, 
                                        IntransitiveByNoNounClauseFactor = 0, IntransitiveByPrepositionFactor = 0},
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 0,
                                        SingularityFactor = 1, PluralityFactor = 0, 
                                        NoArticleFactor = 1, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 0, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 0,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                };
        }
        public static IEnumerable<Clause> CreatePhraseDescriptionForNormalRequired()
        {
            return new List<Clause>()
                {
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 1,
                                        SingularityFactor = 1, PluralityFactor = 0, 
                                        NoArticleFactor = 0, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 0, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 0,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                    new VerbClause() { PresentFactor = 1, PastFactor = 1, FutureFactor = 1, ContinuousFactor = 0, ContinuousPastFactor = 0, PerfectFactor = 0, SubjunctiveFactor = 0,
                                        NoAdverbFactor = 1, AdverbFactor = 0,
                                        InterrogativeFactor = 1, NoInterrogativeFactor = 1, 
                                        IntransitiveByNoNounClauseFactor = 0, IntransitiveByPrepositionFactor = 0},
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 0,
                                        SingularityFactor = 1, PluralityFactor = 0, 
                                        NoArticleFactor = 0, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 0, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 0,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                };
        }
        public static IEnumerable<Clause> CreatePhraseDescriptionForNormalRequiredAnd()
        {
            return new List<Clause>()
                {
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 1,
                                        SingularityFactor = 1, PluralityFactor = 0, 
                                        NoArticleFactor = 0, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 0, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 0,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                    new VerbClause() { PresentFactor = 1, PastFactor = 1, FutureFactor = 1, ContinuousFactor = 0, ContinuousPastFactor = 0, PerfectFactor = 0, SubjunctiveFactor = 0,
                                        NoAdverbFactor = 1, AdverbFactor = 0,
                                        InterrogativeFactor = 1, NoInterrogativeFactor = 1, 
                                        IntransitiveByNoNounClauseFactor = 0, IntransitiveByPrepositionFactor = 0},
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 0,
                                        SingularityFactor = 1, PluralityFactor = 0, 
                                        NoArticleFactor = 0, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 0, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 0,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                    new ConjunctionClause() { JoiningNounFactor = 1, JoiningPhraseFactor = 0 },
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 0,
                                        SingularityFactor = 1, PluralityFactor = 0, 
                                        NoArticleFactor = 0, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 0, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 0,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                };
        }
        public static IEnumerable<Clause> CreatePhraseDescriptionForNormalRequiredSpeech()
        {
            return new List<Clause>()
                {
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 1,
                                        SingularityFactor = 1, PluralityFactor = 0, 
                                        NoArticleFactor = 0, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 0, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 0,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                    new DirectSpeechClause() { NoDirectSpeechFactor = 0, DirectSpeechFactor = 1 },
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 1,
                                        SingularityFactor = 1, PluralityFactor = 0, 
                                        NoArticleFactor = 0, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 0, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 0,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                    new VerbClause() { PresentFactor = 1, PastFactor = 1, FutureFactor = 1, ContinuousFactor = 0, ContinuousPastFactor = 0, PerfectFactor = 0, SubjunctiveFactor = 0,
                                        NoAdverbFactor = 1, AdverbFactor = 0,
                                        InterrogativeFactor = 1, NoInterrogativeFactor = 1, 
                                        IntransitiveByNoNounClauseFactor = 0, IntransitiveByPrepositionFactor = 0},
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 0,
                                        SingularityFactor = 1, PluralityFactor = 0, 
                                        NoArticleFactor = 0, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 0, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 0,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                };
        }

        public static IEnumerable<Clause> CreatePhraseDescriptionForStrong()
        {
            return new List<Clause>()
                {
                    new NounClause() { CommonNounFactor = 12, ProperNounFactor = 1,
                                        SingularityFactor = 7, PluralityFactor = 3, 
                                        NoArticleFactor = 5, DefiniteArticleFactor = 4, IndefiniteArticleFactor = 4, DemonstractiveFactor = 1, PersonalPronounFactor = 2,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 0,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                    new VerbClause() { PresentFactor = 10, PastFactor = 10, FutureFactor = 10, ContinuousFactor = 5, ContinuousPastFactor = 5, PerfectFactor = 5, SubjunctiveFactor = 2,
                                        NoAdverbFactor = 1, AdverbFactor = 0,
                                        NoInterrogativeFactor = 8, InterrogativeFactor = 1, 
                                        IntransitiveByNoNounClauseFactor = 0, IntransitiveByPrepositionFactor = 4},
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 0,
                                        SingularityFactor = 1, PluralityFactor = 0, 
                                        NoArticleFactor = 5, DefiniteArticleFactor = 4, IndefiniteArticleFactor = 4, DemonstractiveFactor = 1, PersonalPronounFactor = 2,
                                        NoAdjectiveFactor = 6, AdjectiveFactor = 3,
                                        NoPrepositionFactor = 15, PrepositionFactor = 1},
                };
        }
        public static IEnumerable<Clause> CreatePhraseDescriptionForStrongAnd()
        {
            return new List<Clause>()
                {
                    new NounClause() { CommonNounFactor = 12, ProperNounFactor = 1,
                                        SingularityFactor = 7, PluralityFactor = 3, 
                                        NoArticleFactor = 5, DefiniteArticleFactor = 4, IndefiniteArticleFactor = 4, DemonstractiveFactor = 1, PersonalPronounFactor = 2,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 0,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                    new VerbClause() { PresentFactor = 10, PastFactor = 10, FutureFactor = 10, ContinuousFactor = 5, ContinuousPastFactor = 5, PerfectFactor = 5, SubjunctiveFactor = 2,
                                        NoAdverbFactor = 1, AdverbFactor = 0,
                                        NoInterrogativeFactor = 8, InterrogativeFactor = 1, 
                                        IntransitiveByNoNounClauseFactor = 0, IntransitiveByPrepositionFactor = 4},
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 0,
                                        SingularityFactor = 1, PluralityFactor = 0, 
                                        NoArticleFactor = 5, DefiniteArticleFactor = 4, IndefiniteArticleFactor = 4, DemonstractiveFactor = 1, PersonalPronounFactor = 2,
                                        NoAdjectiveFactor = 6, AdjectiveFactor = 3,
                                        NoPrepositionFactor = 15, PrepositionFactor = 1},
                    new ConjunctionClause() { JoiningNounFactor = 1, JoiningPhraseFactor = 0 },
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 0,
                                        SingularityFactor = 1, PluralityFactor = 0, 
                                        NoArticleFactor = 5, DefiniteArticleFactor = 4, IndefiniteArticleFactor = 4, DemonstractiveFactor = 1, PersonalPronounFactor = 2,
                                        NoAdjectiveFactor = 6, AdjectiveFactor = 3,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                };
        }
        public static IEnumerable<Clause> CreatePhraseDescriptionForStrongSpeech()
        {
            return new List<Clause>()
                {
                    new NounClause() { CommonNounFactor = 7, ProperNounFactor = 1,
                                        SingularityFactor = 7, PluralityFactor = 3, 
                                        NoArticleFactor = 0, DefiniteArticleFactor = 4, IndefiniteArticleFactor = 4, DemonstractiveFactor = 1, PersonalPronounFactor = 2,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 0,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                    new DirectSpeechClause() { NoDirectSpeechFactor = 0, DirectSpeechFactor = 1 },
                    new NounClause() { CommonNounFactor = 12, ProperNounFactor = 1,
                                        SingularityFactor = 7, PluralityFactor = 3, 
                                        NoArticleFactor = 5, DefiniteArticleFactor = 4, IndefiniteArticleFactor = 4, DemonstractiveFactor = 1, PersonalPronounFactor = 2,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 0,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                    new VerbClause() { PresentFactor = 10, PastFactor = 10, FutureFactor = 10, ContinuousFactor = 5, ContinuousPastFactor = 5, PerfectFactor = 5, SubjunctiveFactor = 2,
                                        NoAdverbFactor = 1, AdverbFactor = 0,
                                        NoInterrogativeFactor = 8, InterrogativeFactor = 1, 
                                        IntransitiveByNoNounClauseFactor = 0, IntransitiveByPrepositionFactor = 4},
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 0,
                                        SingularityFactor = 1, PluralityFactor = 0, 
                                        NoArticleFactor = 5, DefiniteArticleFactor = 4, IndefiniteArticleFactor = 4, DemonstractiveFactor = 1, PersonalPronounFactor = 2,
                                        NoAdjectiveFactor = 6, AdjectiveFactor = 3,
                                        NoPrepositionFactor = 15, PrepositionFactor = 1},
                };
        }
        public static IEnumerable<Clause> CreatePhraseDescriptionForStrongEqual()
        {
            return new List<Clause>()
                {
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 1,
                                        SingularityFactor = 1, PluralityFactor = 1, 
                                        NoArticleFactor = 1, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 1, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 0,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                    new VerbClause() { PresentFactor = 1, PastFactor = 1, FutureFactor = 1, ContinuousFactor = 1, ContinuousPastFactor = 1, PerfectFactor = 1, SubjunctiveFactor = 1,
                                        NoAdverbFactor = 1, AdverbFactor = 0,
                                        NoInterrogativeFactor = 1, InterrogativeFactor = 1, 
                                        IntransitiveByNoNounClauseFactor = 0, IntransitiveByPrepositionFactor = 1},
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 0,
                                        SingularityFactor = 1, PluralityFactor = 0, 
                                        NoArticleFactor = 1, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 1, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 1,
                                        NoPrepositionFactor = 1, PrepositionFactor = 1},
                };
        }
        public static IEnumerable<Clause> CreatePhraseDescriptionForStrongEqualAnd()
        {
            return new List<Clause>()
                {
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 1,
                                        SingularityFactor = 1, PluralityFactor = 1, 
                                        NoArticleFactor = 1, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 1, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 0,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                    new VerbClause() { PresentFactor = 1, PastFactor = 1, FutureFactor = 1, ContinuousFactor = 1, ContinuousPastFactor = 1, PerfectFactor = 1, SubjunctiveFactor = 1,
                                        NoAdverbFactor = 1, AdverbFactor = 0,
                                        NoInterrogativeFactor = 1, InterrogativeFactor = 1, 
                                        IntransitiveByNoNounClauseFactor = 0, IntransitiveByPrepositionFactor = 1},
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 0,
                                        SingularityFactor = 1, PluralityFactor = 0, 
                                        NoArticleFactor = 1, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 1, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 1,
                                        NoPrepositionFactor = 1, PrepositionFactor = 1},
                    new ConjunctionClause() { JoiningNounFactor = 1, JoiningPhraseFactor = 0 },
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 0,
                                        SingularityFactor = 1, PluralityFactor = 0, 
                                        NoArticleFactor = 1, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 1, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 1,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                };
        }
        public static IEnumerable<Clause> CreatePhraseDescriptionForStrongEqualSpeech()
        {
            return new List<Clause>()
                {
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 1,
                                        SingularityFactor = 1, PluralityFactor = 1, 
                                        NoArticleFactor = 0, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 1, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 0,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                    new DirectSpeechClause() { NoDirectSpeechFactor = 0, DirectSpeechFactor = 1 },
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 1,
                                        SingularityFactor = 1, PluralityFactor = 1, 
                                        NoArticleFactor = 1, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 1, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 0,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                    new VerbClause() { PresentFactor = 1, PastFactor = 1, FutureFactor = 1, ContinuousFactor = 1, ContinuousPastFactor = 1, PerfectFactor = 1, SubjunctiveFactor = 1,
                                        NoAdverbFactor = 1, AdverbFactor = 0,
                                        NoInterrogativeFactor = 1, InterrogativeFactor = 1, 
                                        IntransitiveByNoNounClauseFactor = 0, IntransitiveByPrepositionFactor = 1},
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 0,
                                        SingularityFactor = 1, PluralityFactor = 0, 
                                        NoArticleFactor = 1, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 1, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 1,
                                        NoPrepositionFactor = 1, PrepositionFactor = 1},
                };
        }
        public static IEnumerable<Clause> CreatePhraseDescriptionForStrongRequired()
        {
            return new List<Clause>()
                {
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 1,
                                        SingularityFactor = 1, PluralityFactor = 1, 
                                        NoArticleFactor = 0, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 1, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 0,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                    new VerbClause() { PresentFactor = 1, PastFactor = 1, FutureFactor = 1, ContinuousFactor = 1, ContinuousPastFactor = 1, PerfectFactor = 1, SubjunctiveFactor = 1,
                                        NoAdverbFactor = 1, AdverbFactor = 0,
                                        NoInterrogativeFactor = 1, InterrogativeFactor = 1, 
                                        IntransitiveByNoNounClauseFactor = 0, IntransitiveByPrepositionFactor = 1},
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 0,
                                        SingularityFactor = 1, PluralityFactor = 0, 
                                        NoArticleFactor = 0, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 1, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 0, AdjectiveFactor = 1,
                                        NoPrepositionFactor = 0, PrepositionFactor = 1},
                };
        }
        public static IEnumerable<Clause> CreatePhraseDescriptionForStrongRequiredAnd()
        {
            return new List<Clause>()
                {
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 1,
                                        SingularityFactor = 1, PluralityFactor = 1, 
                                        NoArticleFactor = 0, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 1, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 0,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                    new VerbClause() { PresentFactor = 1, PastFactor = 1, FutureFactor = 1, ContinuousFactor = 1, ContinuousPastFactor = 1, PerfectFactor = 1, SubjunctiveFactor = 1,
                                        NoAdverbFactor = 1, AdverbFactor = 0,
                                        NoInterrogativeFactor = 1, InterrogativeFactor = 1, 
                                        IntransitiveByNoNounClauseFactor = 0, IntransitiveByPrepositionFactor = 1},
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 0,
                                        SingularityFactor = 1, PluralityFactor = 0, 
                                        NoArticleFactor = 0, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 1, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 0, AdjectiveFactor = 1,
                                        NoPrepositionFactor = 0, PrepositionFactor = 1},
                    new ConjunctionClause() { JoiningNounFactor = 1, JoiningPhraseFactor = 0 },
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 0,
                                        SingularityFactor = 1, PluralityFactor = 0, 
                                        NoArticleFactor = 0, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 1, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 0, AdjectiveFactor = 1,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                };
        }
        public static IEnumerable<Clause> CreatePhraseDescriptionForStrongRequiredSpeech()
        {
            return new List<Clause>()
                {
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 1,
                                        SingularityFactor = 1, PluralityFactor = 1, 
                                        NoArticleFactor = 0, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 1, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 0,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                    new DirectSpeechClause() { NoDirectSpeechFactor = 0, DirectSpeechFactor = 1 },
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 1,
                                        SingularityFactor = 1, PluralityFactor = 1, 
                                        NoArticleFactor = 0, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 1, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 0,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                    new VerbClause() { PresentFactor = 1, PastFactor = 1, FutureFactor = 1, ContinuousFactor = 1, ContinuousPastFactor = 1, PerfectFactor = 1, SubjunctiveFactor = 1,
                                        NoAdverbFactor = 1, AdverbFactor = 0,
                                        NoInterrogativeFactor = 1, InterrogativeFactor = 1, 
                                        IntransitiveByNoNounClauseFactor = 0, IntransitiveByPrepositionFactor = 1},
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 0,
                                        SingularityFactor = 1, PluralityFactor = 0, 
                                        NoArticleFactor = 0, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 1, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 0, AdjectiveFactor = 1,
                                        NoPrepositionFactor = 0, PrepositionFactor = 1},
                };
        }

        public static IEnumerable<Clause> CreatePhraseDescriptionForInsane()
        {
            return new List<Clause>()
                {
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 0,
                                        SingularityFactor = 7, PluralityFactor = 3, 
                                        NoArticleFactor = 5, DefiniteArticleFactor = 4, IndefiniteArticleFactor = 4, DemonstractiveFactor = 1, PersonalPronounFactor = 2,
                                        NoAdjectiveFactor = 6, AdjectiveFactor = 3,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                    new VerbClause() { PresentFactor = 10, PastFactor = 10, FutureFactor = 10, ContinuousFactor = 5, ContinuousPastFactor = 5, PerfectFactor = 5, SubjunctiveFactor = 5,
                                        NoAdverbFactor = 10, AdverbFactor = 3,
                                        NoInterrogativeFactor = 8, InterrogativeFactor = 1, 
                                        IntransitiveByNoNounClauseFactor = 1, IntransitiveByPrepositionFactor = 5},
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 0,
                                        SingularityFactor = 7, PluralityFactor = 3, 
                                        NoArticleFactor = 5, DefiniteArticleFactor = 4, IndefiniteArticleFactor = 4, DemonstractiveFactor = 1, PersonalPronounFactor = 2,
                                        NoAdjectiveFactor = 6, AdjectiveFactor = 3,
                                        NoPrepositionFactor = 8, PrepositionFactor = 2},
                };
        }
        public static IEnumerable<Clause> CreatePhraseDescriptionForInsaneAnd()
        {
            return new List<Clause>()
                {
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 0,
                                        SingularityFactor = 7, PluralityFactor = 3, 
                                        NoArticleFactor = 5, DefiniteArticleFactor = 4, IndefiniteArticleFactor = 4, DemonstractiveFactor = 1, PersonalPronounFactor = 2,
                                        NoAdjectiveFactor = 6, AdjectiveFactor = 3,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                    new VerbClause() { PresentFactor = 10, PastFactor = 10, FutureFactor = 10, ContinuousFactor = 5, ContinuousPastFactor = 5, PerfectFactor = 5, SubjunctiveFactor = 5,
                                        NoAdverbFactor = 10, AdverbFactor = 3,
                                        NoInterrogativeFactor = 8, InterrogativeFactor = 1, 
                                        IntransitiveByNoNounClauseFactor = 1, IntransitiveByPrepositionFactor = 5},
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 0,
                                        SingularityFactor = 7, PluralityFactor = 3, 
                                        NoArticleFactor = 5, DefiniteArticleFactor = 4, IndefiniteArticleFactor = 4, DemonstractiveFactor = 1, PersonalPronounFactor = 2,
                                        NoAdjectiveFactor = 6, AdjectiveFactor = 3,
                                        NoPrepositionFactor = 8, PrepositionFactor = 2},
                    new ConjunctionClause() { JoiningNounFactor = 1, JoiningPhraseFactor = 0 },
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 0,
                                        SingularityFactor = 7, PluralityFactor = 3, 
                                        NoArticleFactor = 5, DefiniteArticleFactor = 4, IndefiniteArticleFactor = 4, DemonstractiveFactor = 1, PersonalPronounFactor = 2,
                                        NoAdjectiveFactor = 6, AdjectiveFactor = 3,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                };
        }
        public static IEnumerable<Clause> CreatePhraseDescriptionForInsaneSpeech()
        {
            return new List<Clause>()
                {
                    new NounClause() { CommonNounFactor = 7, ProperNounFactor = 1,
                                        SingularityFactor = 7, PluralityFactor = 3, 
                                        NoArticleFactor = 0, DefiniteArticleFactor = 4, IndefiniteArticleFactor = 4, DemonstractiveFactor = 1, PersonalPronounFactor = 2,
                                        NoAdjectiveFactor = 6, AdjectiveFactor = 3,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                    new DirectSpeechClause() { NoDirectSpeechFactor = 0, DirectSpeechFactor = 1 },
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 0,
                                        SingularityFactor = 7, PluralityFactor = 3, 
                                        NoArticleFactor = 5, DefiniteArticleFactor = 4, IndefiniteArticleFactor = 4, DemonstractiveFactor = 1, PersonalPronounFactor = 2,
                                        NoAdjectiveFactor = 6, AdjectiveFactor = 3,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                    new VerbClause() { PresentFactor = 10, PastFactor = 10, FutureFactor = 10, ContinuousFactor = 5, ContinuousPastFactor = 5, PerfectFactor = 5, SubjunctiveFactor = 5,
                                        NoAdverbFactor = 10, AdverbFactor = 3,
                                        NoInterrogativeFactor = 8, InterrogativeFactor = 1, 
                                        IntransitiveByNoNounClauseFactor = 1, IntransitiveByPrepositionFactor = 5},
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 0,
                                        SingularityFactor = 7, PluralityFactor = 3, 
                                        NoArticleFactor = 5, DefiniteArticleFactor = 4, IndefiniteArticleFactor = 4, DemonstractiveFactor = 1, PersonalPronounFactor = 2,
                                        NoAdjectiveFactor = 6, AdjectiveFactor = 3,
                                        NoPrepositionFactor = 8, PrepositionFactor = 2},
                };
        }
        public static IEnumerable<Clause> CreatePhraseDescriptionForInsaneEqual()
        {
            return new List<Clause>()
                {
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 0,
                                        SingularityFactor = 1, PluralityFactor = 1, 
                                        NoArticleFactor = 1, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 1, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 1,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                    new VerbClause() { PresentFactor = 1, PastFactor = 1, FutureFactor = 1, ContinuousFactor = 1, ContinuousPastFactor = 1, PerfectFactor = 1, SubjunctiveFactor = 1,
                                        NoAdverbFactor = 1, AdverbFactor = 1,
                                        NoInterrogativeFactor = 1, InterrogativeFactor = 1, 
                                        IntransitiveByNoNounClauseFactor = 1, IntransitiveByPrepositionFactor = 1},
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 0,
                                        SingularityFactor = 1, PluralityFactor = 1, 
                                        NoArticleFactor = 1, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 1, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 1,
                                        NoPrepositionFactor = 1, PrepositionFactor = 1},
                };
        }
        public static IEnumerable<Clause> CreatePhraseDescriptionForInsaneEqualAnd()
        {
            return new List<Clause>()
                {
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 0,
                                        SingularityFactor = 1, PluralityFactor = 1, 
                                        NoArticleFactor = 1, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 1, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 1,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                    new VerbClause() { PresentFactor = 1, PastFactor = 1, FutureFactor = 1, ContinuousFactor = 1, ContinuousPastFactor = 1, PerfectFactor = 1, SubjunctiveFactor = 1,
                                        NoAdverbFactor = 1, AdverbFactor = 1,
                                        NoInterrogativeFactor = 1, InterrogativeFactor = 1, 
                                        IntransitiveByNoNounClauseFactor = 1, IntransitiveByPrepositionFactor = 1},
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 0,
                                        SingularityFactor = 1, PluralityFactor = 1, 
                                        NoArticleFactor = 1, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 1, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 1,
                                        NoPrepositionFactor = 1, PrepositionFactor = 1},
                    new ConjunctionClause() { JoiningNounFactor = 1, JoiningPhraseFactor = 0 },
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 0,
                                        SingularityFactor = 1, PluralityFactor = 1, 
                                        NoArticleFactor = 1, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 1, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 1,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                };
        }
        public static IEnumerable<Clause> CreatePhraseDescriptionForInsaneEqualSpeech()
        {
            return new List<Clause>()
                {
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 1,
                                        SingularityFactor = 1, PluralityFactor = 1, 
                                        NoArticleFactor = 0, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 1, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 1,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                    new DirectSpeechClause() { NoDirectSpeechFactor = 0, DirectSpeechFactor = 1 },
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 0,
                                        SingularityFactor = 1, PluralityFactor = 1, 
                                        NoArticleFactor = 1, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 1, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 1,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                    new VerbClause() { PresentFactor = 1, PastFactor = 1, FutureFactor = 1, ContinuousFactor = 1, ContinuousPastFactor = 1, PerfectFactor = 1, SubjunctiveFactor = 1,
                                        NoAdverbFactor = 1, AdverbFactor = 1,
                                        NoInterrogativeFactor = 1, InterrogativeFactor = 1, 
                                        IntransitiveByNoNounClauseFactor = 1, IntransitiveByPrepositionFactor = 1},
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 0,
                                        SingularityFactor = 1, PluralityFactor = 1, 
                                        NoArticleFactor = 1, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 1, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 1,
                                        NoPrepositionFactor = 1, PrepositionFactor = 1},
                };
        }
        public static IEnumerable<Clause> CreatePhraseDescriptionForInsaneRequired()
        {
            return new List<Clause>()
                {
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 0,
                                        SingularityFactor = 1, PluralityFactor = 1, 
                                        NoArticleFactor = 0, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 1, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 0, AdjectiveFactor = 1,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                    new VerbClause() { PresentFactor = 1, PastFactor = 1, FutureFactor = 1, ContinuousFactor = 1, ContinuousPastFactor = 1, PerfectFactor = 1, SubjunctiveFactor = 1,
                                        NoAdverbFactor = 0, AdverbFactor = 1,
                                        NoInterrogativeFactor = 1, InterrogativeFactor = 1, 
                                        IntransitiveByNoNounClauseFactor = 1, IntransitiveByPrepositionFactor = 1},
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 0,
                                        SingularityFactor = 1, PluralityFactor = 1, 
                                        NoArticleFactor = 0, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 1, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 0, AdjectiveFactor = 1,
                                        NoPrepositionFactor = 0, PrepositionFactor = 1},
                };
         }
         public static IEnumerable<Clause> CreatePhraseDescriptionForInsaneRequiredAnd()
         {
            return new List<Clause>()
                {
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 0,
                                        SingularityFactor = 1, PluralityFactor = 1, 
                                        NoArticleFactor = 0, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 1, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 0, AdjectiveFactor = 1,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                    new VerbClause() { PresentFactor = 1, PastFactor = 1, FutureFactor = 1, ContinuousFactor = 1, ContinuousPastFactor = 1, PerfectFactor = 1, SubjunctiveFactor = 1,
                                        NoAdverbFactor = 0, AdverbFactor = 1,
                                        NoInterrogativeFactor = 1, InterrogativeFactor = 1, 
                                        IntransitiveByNoNounClauseFactor = 1, IntransitiveByPrepositionFactor = 1},
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 0,
                                        SingularityFactor = 1, PluralityFactor = 1, 
                                        NoArticleFactor = 0, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 1, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 0, AdjectiveFactor = 1,
                                        NoPrepositionFactor = 0, PrepositionFactor = 1},
                    new ConjunctionClause() { JoiningNounFactor = 1, JoiningPhraseFactor = 0 },
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 0,
                                        SingularityFactor = 1, PluralityFactor = 1, 
                                        NoArticleFactor = 0, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 1, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 0, AdjectiveFactor = 1,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                };
         }
         public static IEnumerable<Clause> CreatePhraseDescriptionForInsaneRequiredSpeech()
         {
             return new List<Clause>()
                {
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 1,
                                        SingularityFactor = 1, PluralityFactor = 1, 
                                        NoArticleFactor = 0, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 1, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 0, AdjectiveFactor = 1,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                    new DirectSpeechClause() { NoDirectSpeechFactor = 0, DirectSpeechFactor = 1 },
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 0,
                                        SingularityFactor = 1, PluralityFactor = 1, 
                                        NoArticleFactor = 0, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 1, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 0, AdjectiveFactor = 1,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                    new VerbClause() { PresentFactor = 1, PastFactor = 1, FutureFactor = 1, ContinuousFactor = 1, ContinuousPastFactor = 1, PerfectFactor = 1, SubjunctiveFactor = 1,
                                        NoAdverbFactor = 0, AdverbFactor = 1,
                                        NoInterrogativeFactor = 1, InterrogativeFactor = 1, 
                                        IntransitiveByNoNounClauseFactor = 1, IntransitiveByPrepositionFactor = 1},
                    new NounClause() { CommonNounFactor = 1, ProperNounFactor = 0,
                                        SingularityFactor = 1, PluralityFactor = 1, 
                                        NoArticleFactor = 0, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 1, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 0, AdjectiveFactor = 1,
                                        NoPrepositionFactor = 0, PrepositionFactor = 1},
                };
         }
    }
}
