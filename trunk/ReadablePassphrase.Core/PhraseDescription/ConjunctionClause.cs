// Copyright 2013 Murray Grant
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
    [TagInConfiguration("Conjunction")]
    public class ConjunctionClause : Clause
    {
        [TagInConfiguration("JoinsNoun", "Conjunction")]
        public int JoiningNounFactor { get; set; }
        [TagInConfiguration("JoinsPhrase", "Conjunction")]
        public int JoiningPhraseFactor { get; set; }

        public override void InitialiseRelationships(IEnumerable<Clause> phrase)
        {
            // No-op.
        }

        public override void AddWordTemplate(Random.RandomSourceBase randomness, WordDictionary dictionary, IList<WordTemplate.Template> currentTemplate)
        {
            _ = randomness ?? throw new ArgumentNullException(nameof(randomness));
            _ = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
            _ = currentTemplate ?? throw new ArgumentNullException(nameof(currentTemplate));

            // Conjunctions are very simple: they either join noun clauses or entire phrases (which will require another verb clause).
            // I'm not allowing a conjunction to be either though.

            if (JoiningNounFactor > 0 && JoiningPhraseFactor > 0)
                throw new Exception("Conjunctions may join noun clauses or entire phrases, but not both. Set one of JoinsNoun and JoinsPhrase to 0.");
            if (JoiningNounFactor <= 0 && JoiningPhraseFactor <= 0)
                throw new Exception("Conjunctions may join noun clauses or entire phrases. Set one of JoinsNoun and JoinsPhrase to greater than 0.");

            // Common or proper noun?
            if (JoiningNounFactor > 0)
                currentTemplate.Add(new NounConjunctionTemplate());
            if (JoiningPhraseFactor > 0)
                currentTemplate.Add(new PhraseConjunctionTemplate());
        }
        public override void SecondPassOfWordTemplate(Random.RandomSourceBase randomness, WordDictionary dictionary, IList<WordTemplate.Template> currentTemplate)
        {
            // No-op.
        }


        public override PhraseCombinations CountCombinations(WordDictionary dictionary)
        {
            _ = dictionary ?? throw new ArgumentNullException(nameof(dictionary));

            if (JoiningNounFactor > 0 && JoiningPhraseFactor > 0)
                throw new Exception("Conjunctions may join noun clauses or entire phrases, but not both. Set one of JoinsNoun and JoinsPhrase to 0.");
            if (JoiningNounFactor <= 0 && JoiningPhraseFactor <= 0)
                throw new Exception("Conjunctions may join noun clauses or entire phrases. Set one of JoinsNoun and JoinsPhrase to greater than 0.");

            var countSeparatingNouns = dictionary.CountOf<Words.Conjunction>(w => w.SeparatesNouns);
            var countSeparatingPhrases = dictionary.CountOf<Words.Conjunction>(w => w.SeparatesPhrases);
            if (JoiningNounFactor > 0)
                return new PhraseCombinations(countSeparatingNouns, countSeparatingNouns, countSeparatingNouns);
            if (JoiningPhraseFactor > 0)
                return new PhraseCombinations(countSeparatingPhrases, countSeparatingPhrases, countSeparatingPhrases);

            // Should never get here.
            throw new Exception("Unexpected state in ConjunctionClause.");
        }
    }
}
