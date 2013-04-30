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
        [TagInConfiguration("ProperNoun", "Noun")]
        public int ProperNounFactor { get; set; }
        [TagInConfiguration("CommonNoun", "Noun")]
        public int CommonNounFactor { get; set; }
        [TagInConfiguration("AdjectiveNoun", "Noun")]
        public int NounFromAdjectiveFactor { get; set; }

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
            // How do we form the noun? 
            // - Common noun.
            // - Proper noun. 
            // - Adjective with indefinite pronoun?
            if (randomness.WeightedCoinFlip(CommonNounFactor, ProperNounFactor + NounFromAdjectiveFactor))
                this.AddCommonNoun(randomness, dictionary, currentTemplate);
            else if (randomness.WeightedCoinFlip(ProperNounFactor, NounFromAdjectiveFactor))
                this.AddProperNoun(randomness, dictionary, currentTemplate);
            else if (NounFromAdjectiveFactor != 0)
                this.AddAdjectiveAsNoun(randomness, dictionary, currentTemplate);
            else
                throw new ApplicationException("Unexpected state.");
        }
        public override void SecondPassOfWordTemplate(Random.RandomSourceBase randomness, WordDictionary dictionary, IList<WordTemplate.Template> currentTemplate)
        {
            // No-op for noun clause.
        }

        private void AddCommonNoun(Random.RandomSourceBase randomness, WordDictionary dictionary, IList<WordTemplate.Template> currentTemplate)
        {
            // Common nouns allow for the full range of extra stuff.
            bool isPlural;
            AddNounPrelude(randomness, dictionary, currentTemplate, out isPlural);
            
            // Add an adjective?
            bool includeAdjective = randomness.WeightedCoinFlip(AdjectiveFactor, NoAdjectiveFactor);
            if (includeAdjective)
                currentTemplate.Add(new AdjectiveTemplate());

            // Finally add the noun!
            currentTemplate.Add(new NounTemplate(isPlural));
        }
        private void AddProperNoun(Random.RandomSourceBase randomness, WordDictionary dictionary, IList<WordTemplate.Template> currentTemplate)
        {
            // Proper noun is simply added as-is. There is never an adjective, article, plural, etc.
            currentTemplate.Add(new ProperNounTemplate());
            return;
        }
        private void AddAdjectiveAsNoun(Random.RandomSourceBase randomness, WordDictionary dictionary, IList<WordTemplate.Template> currentTemplate)
        {
            // Using an adjective as a noun means we can't add an adjective, but otherwise allows for a variety of parts of speach.
            bool isPlural;
            AddNounPrelude(randomness, dictionary, currentTemplate, out isPlural);

            currentTemplate.Add(new AdjectiveTemplate());

            // Will we use a personal or impersonal pronoun here?
            bool isPersonal = randomness.CoinFlip();
            currentTemplate.Add(new IndefinitePronounTemplate(isPlural, isPersonal));    
        }
        private void AddNounPrelude(Random.RandomSourceBase randomness, WordDictionary dictionary, IList<WordTemplate.Template> currentTemplate, out bool isPlural)
        {
            // There's a bunch of stuff common to common and adjectives as nouns.
            // Include a preposition?
            bool includePreposition = randomness.WeightedCoinFlip(PrepositionFactor, NoPrepositionFactor);
            if (includePreposition && currentTemplate.Last().GetType() != typeof(PrepositionTemplate))
                currentTemplate.Add(new PrepositionTemplate());

            // Will this noun be plural?
            isPlural = randomness.WeightedCoinFlip(PluralityFactor, SingularityFactor);

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
        }

        public override PhraseCombinations CountCombinations(WordDictionary dictionary)
        {
            var baseCombinations = this.CountBase(dictionary);
            PhraseCombinations resultCommon = PhraseCombinations.One, resultProper = PhraseCombinations.One, resultAdjective = PhraseCombinations.One, resultCommonAndAdjective = PhraseCombinations.One;
            
            // Count for just common nouns.
            if (CommonNounFactor > 0)
                resultCommon = baseCombinations * this.CountSingleFactor<Words.Noun>(dictionary, CommonNounFactor, 0);

            // Count for proper nouns (very simple).
            if (ProperNounFactor > 0)
                resultProper = this.CountSingleFactor<Words.ProperNoun>(dictionary, ProperNounFactor, 0);

            // Count for adjective nouns.
            if (NounFromAdjectiveFactor > 0)
                resultAdjective = baseCombinations * this.CountSingleFactor<Words.Adjective>(dictionary, NounFromAdjectiveFactor, 0);

            // Count combining common and adjective nouns.
            var nounAndAdjectiveCount = (this.CommonNounFactor <= 0 ? 0 : dictionary.CountOf<Words.Noun>())
                                      + (this.NounFromAdjectiveFactor <= 0 ? 0 : dictionary.CountOf<Words.Adjective>());
            if (NounFromAdjectiveFactor > 0 && CommonNounFactor > 0)
                resultCommonAndAdjective = baseCombinations * new PhraseCombinations(nounAndAdjectiveCount, nounAndAdjectiveCount, nounAndAdjectiveCount);

            // Min is whichever of the above is smallest.
            var min = new[] { resultCommon.Shortest, resultProper.Shortest, resultAdjective.Shortest, resultCommonAndAdjective.Shortest }
                            .Where(x => x > 1.0001)
                            .Min();
            // Max is whichever of the above is largest.
            var max = new[] { resultCommon.Longest, resultProper.Longest, resultAdjective.Longest, resultCommonAndAdjective.Longest }
                            .Where(x => x > 1.0001)
                            .Max();
            // Avg is a weighted average of the above.
            double total = CommonNounFactor + ProperNounFactor + NounFromAdjectiveFactor;
            double avg = 0.0;
            if (CommonNounFactor > 0 && NounFromAdjectiveFactor > 0)
                avg += resultCommonAndAdjective.OptionalAverage * ((double)(CommonNounFactor + NounFromAdjectiveFactor) / total);
            else if (CommonNounFactor > 0 && NounFromAdjectiveFactor <= 0)
                avg += resultCommon.OptionalAverage * ((double)CommonNounFactor / total);
            else if (CommonNounFactor <= 0 && NounFromAdjectiveFactor > 0)
                avg += resultAdjective.OptionalAverage * ((double)NounFromAdjectiveFactor / total);
            if (ProperNounFactor > 0)
                avg += resultProper.OptionalAverage * ((double)ProperNounFactor / total);

            return new PhraseCombinations(min, max, avg);
        }

        private PhraseCombinations CountBase(WordDictionary dictionary)
        {
            // Base factors common to most different noun forms (common nouns and adjective nouns).

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

            return result;
        }
    }
}
