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
    /// <summary>
    /// Any word form in the dictionary at all. Used for creating non-grammatical passphrases.
    /// </summary>
    [TagInConfiguration("AnyWord")]
    public class AnyWordClause : Clause
    {
        public override void InitialiseRelationships(IEnumerable<Clause> phrase)
        {
            // No-op.
        }

        public override void AddWordTemplate(Random.RandomSourceBase randomness, WordDictionary dictionary, IList<WordTemplate.Template> currentTemplate)
        {
            _ = currentTemplate ?? throw new ArgumentNullException(nameof(currentTemplate));

            // Simply select an AnyTemplate.
            // No fancy logic here at all.
            currentTemplate.Add(new AnyTemplate());
        }
        public override void SecondPassOfWordTemplate(Random.RandomSourceBase randomness, WordDictionary dictionary, IList<WordTemplate.Template> currentTemplate)
        {
            // No-op.
        }


        public override PhraseCombinations CountCombinations(WordDictionary dictionary)
        {
            _ = dictionary ?? throw new ArgumentNullException(nameof(dictionary));

            // Simply count all word forms.
            var allFormCount = dictionary.CountOfAllDistinctForms();
            return new PhraseCombinations(allFormCount, allFormCount, allFormCount);
        }
    }
}
