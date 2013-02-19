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
    [TagInConfiguration("DirectSpeach")]
    public class DirectSpeachClause : Clause
    {
        [TagInConfiguration("DirectSpeach", "DirectSpeach")]
        public int NoDirectSpeachFactor { get; set; }
        [TagInConfiguration("NoDirectSpeach", "DirectSpeach")]
        public int DirectSpeachFactor { get; set; }

        public override void InitialiseRelationships(IEnumerable<Clause> phrase)
        {
            // No-op.
        }

        public override void AddWordTemplate(Random.RandomSourceBase randomness, WordDictionary dictionary, IList<WordTemplate.Template> currentTemplate)
        {
            // Direct speach verbs are very simple cases of verbs. They rely on a preceeding noun clause to make sense.
            var chosenDirectSpeach = randomness.WeightedCoinFlip(DirectSpeachFactor, NoDirectSpeachFactor);
            if (chosenDirectSpeach)
                currentTemplate.Add(new SpeachVerbTemplate());
            else
            {
                // No direct speach: remove any preceeding noun templates.
                var nounClauseTemplates = new HashSet<Type>() 
                { 
                    typeof(NounTemplate), 
                    typeof(AdjectiveTemplate), 
                    typeof(ProperNounTemplate), 
                    typeof(ArticleTemplate), 
                    typeof(DemonstrativeTemplate), 
                    typeof(PersonalPronounTemplate), 
                    typeof(PrepositionTemplate)  
                };
                var toRemove = currentTemplate.Reverse().TakeWhile(wt => nounClauseTemplates.Contains(wt.GetType())).ToList();
                if (toRemove.Count == currentTemplate.Count)
                    currentTemplate.Clear();
                else
                    foreach (var x in toRemove)
                        currentTemplate.Remove(x);
            }
        }
        public override void SecondPassOfWordTemplate(Random.RandomSourceBase randomness, WordDictionary dictionary, IList<WordTemplate.Template> currentTemplate)
        {
            // No-op.
        }


        public override PhraseCombinations CountCombinations(WordDictionary dictionary)
        {
            var result = this.CountSingleFactor<Words.SpeachVerb>(dictionary, this.DirectSpeachFactor, this.NoDirectSpeachFactor);
            return result;
        }
    }
}
