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


        private List<RangeToTense> DistributionTable;
        private int DistributionMax;

        public bool SubjectIsPlural { get; set; }
        public NounClause Subject { get; set; }
        public NounClause Object { get; set; }

        private readonly IEnumerable<TenseData> _TenseData;

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
            var beforeMe = clause.TakeWhile(x => x != this).Reverse();
            var afterMe = clause.SkipWhile(x => x != this).Take(Int32.MaxValue);

            // Link to this verb.
            this.Subject = (NounClause)beforeMe.First(x => x is NounClause);
            this.Object = (NounClause)afterMe.First(x => x is NounClause);
        }

        public override bool AddWordTemplate(Random.RandomSourceBase randomness, IList<WordTemplate.Template> currentTemplate)
        {
            var subjectIsPlural = currentTemplate.OfType<NounTemplate>().First().IsPlural;
            var verbFormToBePlural = subjectIsPlural;

            // TODO: handle cases where probabilities are zero.

            bool makeInterogative = randomness.WeightedCoinFlip(InterrogativeFactor, NoInterrogativeFactor);
            if (makeInterogative)
                // Insert an interrogative template
                currentTemplate.Insert(0, new InterrogativeTemplate(subjectIsPlural));

            // Select a verb tense form.
            this.BuildTable();
            int choice = randomness.Next(this.DistributionMax);
            var tense = VerbTense.Present;
            if (!makeInterogative)
                // The the verb form becomes present plural whenever an interrogative is used.
                tense = this.LookupTenseFromChoice(choice);
            if (makeInterogative)
                verbFormToBePlural = true;
            currentTemplate.Add(new VerbTemplate(tense, verbFormToBePlural));

            // Include adverb?
            bool includeAdverb = randomness.WeightedCoinFlip(AdverbFactor, NoAdverbFactor);
            if (includeAdverb)
                currentTemplate.Add(new AdverbTemplate());


            return true;
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

        public override double CountCombinations(WordDictionary dictionary)
        {
            double result = 1;

            // Adverbs.
            double factor = 0;
            if (this.AdverbFactor > 0)
                factor += dictionary.CountOf<Words.Adverb>();
            if (this.NoAdverbFactor > 0 && this.AdverbFactor > 0)
                factor += 1;
            if (factor > 0)
                result *= factor;

            // Tense form.
            factor = 0;
            foreach (var t in this._TenseData.Where(t => t.Property() > 0))
                factor += 1;
            // Interrogatives are handled as another tense form, because they restrict verb forms to present singular.
            if (this.InterrogativeFactor > 0)
                factor += 1;
            if (factor > 0)     // This handles the case where no tense is selected (although this really makes no sense).
                result *= factor;

            
            // And the verbs themselves. (This assumes a verb will always be added).
            result *= dictionary.CountOf<Words.Verb>();
            // The count of interrogative forms.
            if (this.InterrogativeFactor > 0)
                result *= dictionary.CountOf<Interrogative>();

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
    }
}
