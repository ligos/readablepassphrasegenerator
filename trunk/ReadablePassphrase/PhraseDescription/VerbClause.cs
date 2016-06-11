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
using MurrayGrant.ReadablePassphrase.Words;
using MurrayGrant.ReadablePassphrase.WordTemplate;
using MurrayGrant.ReadablePassphrase.Dictionaries;

namespace MurrayGrant.ReadablePassphrase.PhraseDescription
{
    [TagInConfiguration("Verb")]
    public class VerbClause : Clause
    {
        [TagInConfiguration("Present", "Tense")]
        public int PresentFactor { get; set; }
        [TagInConfiguration("Past", "Tense")]
        public int PastFactor { get; set; }
        [TagInConfiguration("Future", "Tense")]
        public int FutureFactor { get; set; }
        [TagInConfiguration("ContinuousPast", "Tense")]
        public int ContinuousPastFactor { get; set; }
        [TagInConfiguration("Continuous", "Tense")]
        public int ContinuousFactor { get; set; }
        [TagInConfiguration("Perfect", "Tense")]
        public int PerfectFactor { get; set; }
        [TagInConfiguration("Subjunctive", "Tense")]
        public int SubjunctiveFactor { get; set; }

        [TagInConfiguration("Adverb", "Adverb")]
        public int AdverbFactor { get; set; }
        [TagInConfiguration("NoAdverb", "Adverb")]
        public int NoAdverbFactor { get; set; }

        [TagInConfiguration("Interrogative", "Interrogative")]
        public int InterrogativeFactor { get; set; }
        [TagInConfiguration("NoInterrogative", "Interrogative")]
        public int NoInterrogativeFactor { get; set; }

        [TagInConfiguration("IntransitiveByNoNoun", "Intransitive")]
        public int IntransitiveByNoNounClauseFactor { get; set; }
        [TagInConfiguration("IntransitiveByPreposition", "Intransitive")]
        public int IntransitiveByPrepositionFactor { get; set; }

        private List<RangeToTense> DistributionTable;
        private int DistributionMax;

        public bool SubjectIsPlural { get; set; }
        public IEnumerable<Clause> Subject { get; set; }
        public IEnumerable<Clause> Object { get; set; }

        private readonly IEnumerable<TenseData> _TenseData;
        private bool _IsSecondCall = false;
        private int _LastVerbTemplateIndex = -1;

        public VerbClause()
        {
            this._TenseData = new TenseData[] 
                    { 
                        new TenseData(VerbTense.Present, () => this.PresentFactor),
                        new TenseData(VerbTense.Past, () => this.PastFactor),
                        new TenseData(VerbTense.Future, () => this.FutureFactor),
                        new TenseData(VerbTense.Continuous, () => this.ContinuousFactor),
                        new TenseData(VerbTense.ContinuousPast, () => this.ContinuousPastFactor),
                        new TenseData(VerbTense.Perfect, () => this.PerfectFactor),
                        new TenseData(VerbTense.Subjunctive, () => this.SubjunctiveFactor),
                    };      
        }

        public override void InitialiseRelationships(IEnumerable<Clause> clause)
        {
            // TODO: support more than one verb?? Or zero verbs?
            // Find the noun before and after this verb.
            var beforeMe = clause.TakeWhile(x => x != this);
            var afterMe = clause.SkipWhile(x => x != this).Skip(1).Take(Int32.MaxValue);

            // Link to this verb.
            this.Subject = beforeMe.ToList();
            this.Object = afterMe.ToList();
        }

        public override void AddWordTemplate(Random.RandomSourceBase randomness, WordDictionary dictionary, IList<WordTemplate.Template> currentTemplate)
        {
            this._LastVerbTemplateIndex = -1;
            
            // Figuring out if the verb will be plural or not is... well... complicated.
            bool subjectIsPlural;
            int speechVerbIdx = -1, indefiniteNounIdx = -1;
            bool containsDirectSpeech = currentTemplate.OfType<SpeechVerbTemplate>().Any();
            bool containsIndefiniteNoun = currentTemplate.OfType<IndefinitePronounTemplate>().Any();
            if (containsDirectSpeech)
                speechVerbIdx = CollectionHelper.IndexOfType(currentTemplate, typeof(SpeechVerbTemplate));
            if (containsIndefiniteNoun)
                indefiniteNounIdx = CollectionHelper.IndexOfType(currentTemplate, typeof(IndefinitePronounTemplate));
            
            var ntemp = !containsDirectSpeech ? currentTemplate.OfType<NounTemplate>().FirstOrDefault()
                                              : currentTemplate.Skip(speechVerbIdx).OfType<NounTemplate>().FirstOrDefault();
            var ptemp = containsIndefiniteNoun ? (currentTemplate[indefiniteNounIdx] as IndefinitePronounTemplate) : null;

            if (ptemp == null && ntemp == null)
                subjectIsPlural = false;        // Proper nouns are never plural.
            else if (ptemp == null && ntemp != null)
                subjectIsPlural = ntemp.IsPlural;
            else if (ptemp != null && ntemp == null)
                subjectIsPlural = ptemp.IsPlural;
            else if (ptemp != null && ntemp != null)
                // This probably shouldn't happen, but if it does, we'll take the noun.
                subjectIsPlural = ntemp.IsPlural;
            else
                throw new ApplicationException("Unexpected state.");
            var verbFormToBePlural = subjectIsPlural;

            // Choose how to handle intransitive verbs.
            bool selectTransitive = true;
            bool removeAccusativeNoun = false;
            bool addPreposition = false;
            int choice = 0;
            if (IntransitiveByNoNounClauseFactor > 0 || IntransitiveByPrepositionFactor > 0)
            {
                choice = randomness.Next(dictionary.CountOf<Verb>());     // Choose between transitive or not by the number of trans/intrans verbs in dictionary.
                selectTransitive = choice < dictionary.CountOfTransitiveVerbs();
                if (!selectTransitive)
                {
                    // OK, so we chose an intransitive verb, how will we handle that?
                    // Note that we've established either IntransitiveByNoNounClauseFactor or IntransitiveByPrepositionFactor is greater than zero, so WeightedCoinFlip() will never throw.
                    bool handleIntransByNoObjectNoun = randomness.WeightedCoinFlip(IntransitiveByNoNounClauseFactor, IntransitiveByPrepositionFactor);
                    if (handleIntransByNoObjectNoun)
                        // Remove the noun clause.
                        removeAccusativeNoun = true;
                    else
                        // Add a preposition.
                        addPreposition = true;
                }
            }

            bool makeInterrogative = (InterrogativeFactor + NoInterrogativeFactor > 0)      // Case where neither is specified: assume no interrogative.
                                   && randomness.WeightedCoinFlip(InterrogativeFactor, NoInterrogativeFactor);
            if (makeInterrogative && containsDirectSpeech)
                // Insert an interrogative template after the direct speech verb.
                currentTemplate.Insert(speechVerbIdx + 1, new InterrogativeTemplate(subjectIsPlural));
            else if (makeInterrogative && !containsDirectSpeech)
                // Insert an interrogative template at the start of the phrase.
                currentTemplate.Insert(0, new InterrogativeTemplate(subjectIsPlural));

            // Include adverb?
            bool includeAdverb = (AdverbFactor + NoAdverbFactor > 0)        // Case where neither is specified: assume no adverb.
                               && randomness.WeightedCoinFlip(AdverbFactor, NoAdverbFactor);
            bool includeAdverbBeforeVerb = randomness.CoinFlip();
            if (includeAdverb && includeAdverbBeforeVerb)
                currentTemplate.Add(new AdverbTemplate());

            // Select a verb tense form.
            this.BuildTable();
            choice = randomness.Next(this.DistributionMax);
            var tense = VerbTense.Present;
            if (!makeInterrogative)
                // The the verb form becomes present plural whenever an interrogative is used.
                tense = this.LookupTenseFromChoice(choice);
            if (makeInterrogative)
                verbFormToBePlural = true;
            currentTemplate.Add(new VerbTemplate(tense, verbFormToBePlural, selectTransitive));

            // Include adverb after the verb.
            if (includeAdverb && !includeAdverbBeforeVerb)
                currentTemplate.Add(new AdverbTemplate());

            // Add a preposition to make the intransitive verb work?
            if (addPreposition)
                currentTemplate.Add(new PrepositionTemplate());

            // Signal to the second pass we're going to remove the accusative noun clause.
            if (removeAccusativeNoun)
                _LastVerbTemplateIndex = currentTemplate.Count - 1;
        }
        public override void SecondPassOfWordTemplate(Random.RandomSourceBase randomness, WordDictionary dictionary, IList<WordTemplate.Template> currentTemplate)
        {
            // If we deal with intransitive by having no object, remove the object!
            if (this._LastVerbTemplateIndex >= 0)
            {
                while (currentTemplate.Count > this._LastVerbTemplateIndex + 1)
                    currentTemplate.RemoveAt(this._LastVerbTemplateIndex + 1);
            }
        }

        private void BuildTable()
        {
            var result = new List<RangeToTense>();
            var accumulator = 0;

            foreach(var x in this._TenseData)
			{
                result.Add(new RangeToTense(new Range(accumulator, accumulator + x.Property() - 1), x.Tense));
                accumulator += x.Property();
            }

            this.DistributionMax = accumulator;
            this.DistributionTable = result;
        }
        private VerbTense LookupTenseFromChoice(int choice)
        {
            foreach (var x in this.DistributionTable)
            {
                if (x.Range.Within(choice))
                    return x.Tense;
            }
            throw new ApplicationException("Unexpected state for choice " + choice);
        }
        private class RangeToTense
        {
            public readonly Range Range;
            public readonly VerbTense Tense;
            public RangeToTense(Range r, VerbTense t)
            {
                this.Range = r;
                this.Tense = t;
            }
        }
        private struct Range
        {
            public readonly int Min;
            public readonly int Max;
            public Range(int min, int max)
            {
                Min = min;
                Max = max;
            }

            public bool Within(int num)
            {
                return num >= this.Min && num <= this.Max;
            }
        }

        public override PhraseCombinations CountCombinations(WordDictionary dictionary)
        {
            var result = PhraseCombinations.One;

            // Adverbs.
            result *= this.CountSingleFactor<Words.Adverb>(dictionary, this.AdverbFactor, this.NoAdverbFactor);

            // Tense form.
            double shortest = 1, longest = 1, average = 1;
            shortest = this._TenseData.Where(t => t.Property() > 0).Select(x => 1.0).Sum();
            longest = shortest + (this.InterrogativeFactor > 0 ? 1.0 : 0.0);
            longest = longest + (this.InterrogativeFactor > 0 && this.NoInterrogativeFactor > 0 ? 1.0 : 0.0);
            average = longest + (1.0 * (1.0 - ((double)this.NoInterrogativeFactor / (double)(this.InterrogativeFactor + this.NoInterrogativeFactor))));
            result *= new PhraseCombinations(shortest, longest, average);

            
            // And the verbs themselves. (This assumes a verb will always be added).
            var allVerbCount = dictionary.CountOf<Words.Verb>();
            var transitiveVerbCount = dictionary.CountOf<Words.Verb>(v => v.IsTransitive);
            var intransitiveVerbCount = dictionary.CountOf<Words.Verb>(v => !v.IsTransitive);
            if (IntransitiveByPrepositionFactor == 0 && IntransitiveByNoNounClauseFactor == 0)
                // Handling intransitive problems by only choosing from transitive verbs!
                result *= new PhraseCombinations(transitiveVerbCount, transitiveVerbCount, transitiveVerbCount);
            else 
                result *= new PhraseCombinations(allVerbCount, allVerbCount, allVerbCount);


            // If we're handling intransitives by adding a preposition, count prepositions!
            var prepositionCountFraction = dictionary.CountOf<Words.Preposition>()                             
                // But only increase by the fraction of verbs which are intransitive.
                            * ((double)allVerbCount / (double)intransitiveVerbCount);
            if (IntransitiveByPrepositionFactor > 0)
                result *= new PhraseCombinations(prepositionCountFraction, prepositionCountFraction, prepositionCountFraction);

            // The count of interrogative forms.
            result *= this.CountSingleFactor<Interrogative>(dictionary, this.InterrogativeFactor, this.NoInterrogativeFactor);

            return result;
        }

        private class TenseData
        {
            public TenseData(VerbTense t, Func<int> p)
            {
                this.Tense = t;
                this.Property = p;
            }
            public readonly Func<int> Property;
            public readonly VerbTense Tense;
        }

        private static class CollectionHelper
        {
            public static int IndexOfType<T>(IList<T> collection, Type t)
            {
                for (int i = 0; i < collection.Count; i++)
                {
                    if (collection[i].GetType() == t)
                        return i;
                }
                return -1;
            }
        }
    }
}
