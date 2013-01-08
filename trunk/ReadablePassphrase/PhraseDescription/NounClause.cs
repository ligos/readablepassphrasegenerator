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
using MurrayGrant.ReadablePassphrase.Dictionaries;

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

        public override void InitialiseRelationships(IEnumerable<Clause> phrase)
        {
            // The verb does all this at the moment, but perhaps that shouldn't be the case.
        }

        public override void AddWordTemplate(Random.RandomSourceBase randomness, WordDictionary dictionary, IList<WordTemplate.Template> currentTemplate)
        {
            // Include a preposition?
            bool includePreposition = randomness.WeightedCoinFlip(PrepositionFactor, NoPrepositionFactor);
            if (includePreposition && currentTemplate.Last().GetType() != typeof(PrepositionTemplate))
                currentTemplate.Add(new PrepositionTemplate());

            // Will this noun be plural?
            bool isPlural = randomness.WeightedCoinFlip(PluralityFactor, SingularityFactor);

            // What kind of article will this Noun have?
            if (!isPlural)
            {
                // Singular accepts: Definite, Indefinite, Demonstrative, PersonalPronoun.
                int choice = randomness.Next(DefiniteArticleFactor + IndefiniteArticleFactor + DemonstractiveFactor + PersonalPronounFactor);
                if (choice < DefiniteArticleFactor)
                    currentTemplate.Add(new ArticleTemplate(true));
                else if (choice >= DefiniteArticleFactor && choice < DefiniteArticleFactor + IndefiniteArticleFactor)
                    currentTemplate.Add(new ArticleTemplate(false));
                else if (choice >= DefiniteArticleFactor + IndefiniteArticleFactor && choice < DefiniteArticleFactor + IndefiniteArticleFactor + DemonstractiveFactor)
                    currentTemplate.Add(new DemonstrativeTemplate(isPlural));
                else
                    currentTemplate.Add(new PersonalPronounTemplate(isPlural));
            }
            else
            {
                // Plural accepts: None, Definite, Demonstrative, PersonalPronoun.
                int choice = randomness.Next(NoArticleFactor + DefiniteArticleFactor + DemonstractiveFactor + PersonalPronounFactor);
                if (choice < NoArticleFactor)
                { } // NoOp.
                else if (choice >= NoArticleFactor && choice < NoArticleFactor + DefiniteArticleFactor)
                    currentTemplate.Add(new ArticleTemplate(true));
                else if (choice >= NoArticleFactor + DefiniteArticleFactor && choice < NoArticleFactor + DefiniteArticleFactor + DemonstractiveFactor)
                    currentTemplate.Add(new DemonstrativeTemplate(isPlural));
                else
                    currentTemplate.Add(new PersonalPronounTemplate(isPlural));
            }

            // Add an adjective?
            bool includeAdjective = randomness.WeightedCoinFlip(AdjectiveFactor, NoAdjectiveFactor);
            if (includeAdjective)
                currentTemplate.Add(new AdjectiveTemplate());

            // Finally add the noun!
            currentTemplate.Add(new NounTemplate(isPlural));
        }
        public override void SecondPassOfWordTemplate(Random.RandomSourceBase randomness, WordDictionary dictionary, IList<WordTemplate.Template> currentTemplate)
        {
            // No-op for noun clause.
        }


        public override PhraseCombinations CountCombinations(WordDictionary dictionary)
        {
            var result = PhraseCombinations.One;

            // Prepositions.
            result *= this.CountSingleFactor<Words.Preposition>(dictionary, this.PrepositionFactor, this.NoPrepositionFactor);

            // Adjectives.
            result *= this.CountSingleFactor<Words.Adjective>(dictionary, this.AdjectiveFactor, this.NoAdjectiveFactor);

            // Plural / Singular.
            double factor = 0;
            if (this.PluralityFactor > 0)
                factor += 1;
            if (this.SingularityFactor > 0)
                factor += 1;
            if (factor > 0)
                result *= new PhraseCombinations(factor, factor, factor);

            // Article / demonstrative / pronoun.
            int count = 0;
            if (this.DefiniteArticleFactor > 0)
                count += dictionary.CountOf<Words.Article>();
            if (this.IndefiniteArticleFactor > 0)
                count += dictionary.CountOf<Words.Article>();
            if (this.DemonstractiveFactor > 0)
                count += dictionary.CountOf<Words.Demonstrative>();
            if (this.PersonalPronounFactor > 0)
                count += dictionary.CountOf<Words.PersonalPronoun>();
            result *= this.CountSingleFactor(this.DefiniteArticleFactor + this.IndefiniteArticleFactor + this.DemonstractiveFactor + this.PersonalPronounFactor, this.NoArticleFactor, count);

            // Finally, the noun itself!
            result *= this.CountSingleFactor<Words.Noun>(dictionary, 1, 0);

            return result;
        }
    }
}
