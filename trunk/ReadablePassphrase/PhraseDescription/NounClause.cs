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
using MurrayGrant.ReadablePassphrase.WordTemplate;

namespace MurrayGrant.ReadablePassphrase.PhraseDescription
{
    [TagInConfiguration("Noun")]
    public class NounClause : Clause
    {
        [TagInConfiguration("Plural", "Number")]
        public int PluralityFactor { get; set; }
        [TagInConfiguration("Single", "Number")]
        public int SingularityFactor { get; set; }

        [TagInConfiguration("NoArticle", "Article")]
        public int NoArticleFactor { get; set; }
        [TagInConfiguration("DefiniteArticle", "Article")]
        public int DefiniteArticleFactor { get; set; }
        [TagInConfiguration("IndefiniteArticle", "Article")]
        public int IndefiniteArticleFactor { get; set; }
        [TagInConfiguration("Demonstrative", "Article")]
        public int DemonstractiveFactor { get; set; }
        [TagInConfiguration("PersonalPronoun", "Article")]
        public int PersonalPronounFactor { get; set; }

        [TagInConfiguration("Adjective", "Adjective")]
        public int AdjectiveFactor { get; set; }
        [TagInConfiguration("NoAdjective", "Adjective")]
        public int NoAdjectiveFactor { get; set; }

        [TagInConfiguration("Preposition", "Preposition")]
        public int PrepositionFactor { get; set; }
        [TagInConfiguration("NoPreposition", "Preposition")]
        public int NoPrepositionFactor { get; set; }

        public bool IsSubject { get; set; }
        public bool IsObject { get; set; }
        public VerbClause Verb { get; set; }

        public override IEnumerable<Template> GetWordTemplate(Random.RandomSourceBase randomness)
        {
            var result = new List<Template>();

            // Include a preposition?
            bool includePreposition = randomness.WeightedCoinFlip(PrepositionFactor, NoPrepositionFactor);
            if (includePreposition)
                result.Add(new PrepositionTemplate());

            // Will this noun be plural?
            bool isPlural = randomness.WeightedCoinFlip(PluralityFactor, SingularityFactor);

            // What kind of article will this Noun have?
            if (!isPlural)
            {
                // Singular accepts: Definite, Indefinite, Demonstrative, PersonalPronoun.
                int choice = randomness.Next(DefiniteArticleFactor + IndefiniteArticleFactor + DemonstractiveFactor + PersonalPronounFactor);
                if (choice < DefiniteArticleFactor)
                    result.Add(new ArticleTemplate(true));
                else if (choice >= DefiniteArticleFactor && choice < DefiniteArticleFactor + IndefiniteArticleFactor)
                    result.Add(new ArticleTemplate(false));
                else if (choice >= DefiniteArticleFactor + IndefiniteArticleFactor && choice < DefiniteArticleFactor + IndefiniteArticleFactor + DemonstractiveFactor)
                    result.Add(new DemonstrativeTemplate(isPlural));
                else
                    result.Add(new PersonalPronounTemplate(isPlural));
            }
            else
            {
                // Plural accepts: None, Definite, Demonstrative, PersonalPronoun.
                int choice = randomness.Next(NoArticleFactor + DefiniteArticleFactor + DemonstractiveFactor + PersonalPronounFactor);
                if (choice < NoArticleFactor)
                { } // NoOp.
                else if (choice >= NoArticleFactor && choice < NoArticleFactor + DefiniteArticleFactor)
                    result.Add(new ArticleTemplate(true));
                else if (choice >= NoArticleFactor + DefiniteArticleFactor && choice < NoArticleFactor + DefiniteArticleFactor + DemonstractiveFactor)
                    result.Add(new DemonstrativeTemplate(isPlural));
                else
                    result.Add(new PersonalPronounTemplate(isPlural));
            }

            // Add an adjective?
            bool includeAdjective = randomness.WeightedCoinFlip(AdjectiveFactor, NoAdjectiveFactor);
            if (includeAdjective)
                result.Add(new AdjectiveTemplate());

            result.Add(new NounTemplate(isPlural));
            return result;
        }

        public override double CountCombinations(Words.WordDictionary dictionary)
        {
            double result = 1;

            // Prepositions.
            double factor = 0;
            if (this.PrepositionFactor > 0)
                factor += dictionary.CountOf<Words.Preposition>();
            if (this.NoPrepositionFactor > 0 && this.PrepositionFactor > 0)
                factor += 1;
            if (factor > 0)
                result *= factor;

            // Adjectives.
            factor = 0;
            if (this.AdjectiveFactor > 0)
                factor += dictionary.CountOf<Words.Adjective>();
            if (this.NoAdjectiveFactor > 0 && this.AdjectiveFactor > 0)
                factor += 1;
            if (factor > 0)
                result *= factor;

            // Plural / Singular.
            factor = 0;
            if (this.PluralityFactor > 0)
                factor += 1;
            if (this.SingularityFactor > 0)
                factor += 1;
            if (factor > 0)
                result *= factor;

            // Article / demonstrative / pronoun.
            factor = 0;
            if (this.DefiniteArticleFactor > 0)
                factor += dictionary.CountOf<Words.Article>();
            if (this.IndefiniteArticleFactor > 0)
                factor += dictionary.CountOf<Words.Article>();
            if (this.DemonstractiveFactor > 0)
                factor += dictionary.CountOf<Words.Demonstrative>();
            if (this.PersonalPronounFactor > 0)
                factor += dictionary.CountOf<Words.PersonalPronoun>();
            if (this.NoArticleFactor > 0 && factor > 1)     // Cheat's way of checking if any of the other factors are present or if articles are just turned completely off.
                factor += 1;
            if (factor > 0)
                result *= factor;

            // Finally, the noun itself!
            result *= dictionary.CountOf<Words.Noun>();

            return result;
        }
    }
}
