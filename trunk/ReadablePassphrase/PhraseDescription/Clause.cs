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

        private static readonly Type[] AllowedClauseTypes = new Type[] { typeof(NounClause), typeof(VerbClause) };
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

        public static IEnumerable<Clause> CreatePhraseDescription(PhraseStrength strength)
        {
            if (strength == ReadablePassphrase.PhraseStrength.Normal)
                return CreatePhraseDescriptionForNormal();
            else if (strength == ReadablePassphrase.PhraseStrength.NormalEqual)
                return CreatePhraseDescriptionForNormalEqual();
            else if (strength == ReadablePassphrase.PhraseStrength.Strong)
                return CreatePhraseDescriptionForStrong();
            else if (strength == ReadablePassphrase.PhraseStrength.StrongEqual)
                return CreatePhraseDescriptionForStrongEqual();
            else if (strength == ReadablePassphrase.PhraseStrength.Insane)
                return CreatePhraseDescriptionForInsane();
            else if (strength == ReadablePassphrase.PhraseStrength.InsaneEqual)
                return CreatePhraseDescriptionForInsaneEqual();
            else
                throw new ArgumentException(string.Format("Unsupported default PhraseStrength '{0}'. To use Custom, create an IEnumerable<Clause>.", strength), "strength");
        }
        public static IEnumerable<Clause> CreatePhraseDescriptionForNormal()
        {
            return new List<Clause>()
                {
                    new NounClause() { SingularityFactor = 1, PluralityFactor = 0, 
                                        NoArticleFactor = 5, DefiniteArticleFactor = 4, IndefiniteArticleFactor = 4, DemonstractiveFactor = 0, PersonalPronounFactor = 2,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 0,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                    new VerbClause() { PresentFactor = 10, PastFactor = 8, FutureFactor = 8, ContinuousFactor = 0, ContinuousPastFactor = 0, PerfectFactor = 0, SubjunctiveFactor = 0,
                                        NoAdverbFactor = 1, AdverbFactor = 0,
                                        InterrogativeFactor = 1, NoInterrogativeFactor = 8, 
                                        NoIntransitiveFactor = 1, IntransitiveByNoNounClauseFactor = 0, IntransitiveByPrepositionFactor = 0},
                    new NounClause() { SingularityFactor = 1, PluralityFactor = 0, 
                                        NoArticleFactor = 5, DefiniteArticleFactor = 4, IndefiniteArticleFactor = 4, DemonstractiveFactor = 0, PersonalPronounFactor = 2,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 0,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                };
        }
        public static IEnumerable<Clause> CreatePhraseDescriptionForNormalEqual()
        {
            return new List<Clause>()
                {
                    new NounClause() { SingularityFactor = 1, PluralityFactor = 0, 
                                        NoArticleFactor = 1, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 0, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 0,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                    new VerbClause() { PresentFactor = 1, PastFactor = 1, FutureFactor = 1, ContinuousFactor = 0, ContinuousPastFactor = 0, PerfectFactor = 0, SubjunctiveFactor = 0,
                                        NoAdverbFactor = 1, AdverbFactor = 0,
                                        InterrogativeFactor = 1, NoInterrogativeFactor = 1, 
                                        NoIntransitiveFactor = 1, IntransitiveByNoNounClauseFactor = 0, IntransitiveByPrepositionFactor = 0},
                    new NounClause() { SingularityFactor = 1, PluralityFactor = 0, 
                                        NoArticleFactor = 1, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 0, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 0,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                };
        }
        public static IEnumerable<Clause> CreatePhraseDescriptionForStrong()
        {
            return new List<Clause>()
                {
                    new NounClause() { SingularityFactor = 7, PluralityFactor = 3, 
                                        NoArticleFactor = 5, DefiniteArticleFactor = 4, IndefiniteArticleFactor = 4, DemonstractiveFactor = 1, PersonalPronounFactor = 2,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 0,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                    new VerbClause() { PresentFactor = 10, PastFactor = 10, FutureFactor = 10, ContinuousFactor = 5, ContinuousPastFactor = 5, PerfectFactor = 5, SubjunctiveFactor = 2,
                                        NoAdverbFactor = 1, AdverbFactor = 0,
                                        NoInterrogativeFactor = 8, InterrogativeFactor = 1, 
                                        NoIntransitiveFactor = 1, IntransitiveByNoNounClauseFactor = 0, IntransitiveByPrepositionFactor = 4},
                    new NounClause() { SingularityFactor = 1, PluralityFactor = 0, 
                                        NoArticleFactor = 5, DefiniteArticleFactor = 4, IndefiniteArticleFactor = 4, DemonstractiveFactor = 1, PersonalPronounFactor = 2,
                                        NoAdjectiveFactor = 6, AdjectiveFactor = 3,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                };
        }
        public static IEnumerable<Clause> CreatePhraseDescriptionForStrongEqual()
        {
            return new List<Clause>()
                {
                    new NounClause() { SingularityFactor = 1, PluralityFactor = 1, 
                                        NoArticleFactor = 1, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 1, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 0,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                    new VerbClause() { PresentFactor = 1, PastFactor = 1, FutureFactor = 1, ContinuousFactor = 1, ContinuousPastFactor = 1, PerfectFactor = 1, SubjunctiveFactor = 1,
                                        NoAdverbFactor = 1, AdverbFactor = 0,
                                        NoInterrogativeFactor = 1, InterrogativeFactor = 1, 
                                        NoIntransitiveFactor = 1, IntransitiveByNoNounClauseFactor = 0, IntransitiveByPrepositionFactor = 1},
                    new NounClause() { SingularityFactor = 1, PluralityFactor = 0, 
                                        NoArticleFactor = 1, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 1, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 1,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                };
        }
        public static IEnumerable<Clause> CreatePhraseDescriptionForInsane()
        {
            return new List<Clause>()
                {
                    new NounClause() { SingularityFactor = 7, PluralityFactor = 3, 
                                        NoArticleFactor = 5, DefiniteArticleFactor = 4, IndefiniteArticleFactor = 4, DemonstractiveFactor = 1, PersonalPronounFactor = 2,
                                        NoAdjectiveFactor = 6, AdjectiveFactor = 3,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                    new VerbClause() { PresentFactor = 10, PastFactor = 10, FutureFactor = 10, ContinuousFactor = 5, ContinuousPastFactor = 5, PerfectFactor = 5, SubjunctiveFactor = 5,
                                        NoAdverbFactor = 10, AdverbFactor = 3,
                                        NoInterrogativeFactor = 8, InterrogativeFactor = 1, 
                                        NoIntransitiveFactor = 1, IntransitiveByNoNounClauseFactor = 1, IntransitiveByPrepositionFactor = 5},
                    new NounClause() { SingularityFactor = 7, PluralityFactor = 3, 
                                        NoArticleFactor = 5, DefiniteArticleFactor = 4, IndefiniteArticleFactor = 4, DemonstractiveFactor = 1, PersonalPronounFactor = 2,
                                        NoAdjectiveFactor = 6, AdjectiveFactor = 3,
                                        NoPrepositionFactor = 8, PrepositionFactor = 2},
                };
        }
        public static IEnumerable<Clause> CreatePhraseDescriptionForInsaneEqual()
        {
            return new List<Clause>()
                {
                    new NounClause() { SingularityFactor = 1, PluralityFactor = 1, 
                                        NoArticleFactor = 1, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 1, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 1,
                                        NoPrepositionFactor = 1, PrepositionFactor = 0},
                    new VerbClause() { PresentFactor = 1, PastFactor = 1, FutureFactor = 1, ContinuousFactor = 1, ContinuousPastFactor = 1, PerfectFactor = 1, SubjunctiveFactor = 1,
                                        NoAdverbFactor = 1, AdverbFactor = 1,
                                        NoInterrogativeFactor = 1, InterrogativeFactor = 1, 
                                        NoIntransitiveFactor = 1, IntransitiveByNoNounClauseFactor = 1, IntransitiveByPrepositionFactor = 1},
                    new NounClause() { SingularityFactor = 1, PluralityFactor = 1, 
                                        NoArticleFactor = 1, DefiniteArticleFactor = 1, IndefiniteArticleFactor = 1, DemonstractiveFactor = 1, PersonalPronounFactor = 1,
                                        NoAdjectiveFactor = 1, AdjectiveFactor = 1,
                                        NoPrepositionFactor = 1, PrepositionFactor = 1},
                };
        }
    }
}
