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
using MurrayGrant.ReadablePassphrase.Dictionaries;

namespace MurrayGrant.ReadablePassphrase.WordTemplate
{
    /// <summary>
    /// A template word has a 1:1 correspondance with actual dictionary words, where as a <c>Clause</c> does not.
    /// A <c>Clause</c> produces one or more template words, which then choose their final <c>Word</c> from a dictionary.
    /// </summary>
    public abstract class Template
    {
        public abstract bool IncludeInAlreadyUsedList { get; }
        public abstract WordAndString ChooseWord(WordDictionary words, Random.RandomSourceBase randomness, IEnumerable<Word> alreadyChosen, Func<Word, bool> wordPredicate);

        public static bool ExcludeTags(Word w, IReadOnlyList<string>? mustExcludeTheseTags)
            => mustExcludeTheseTags == null 
            || mustExcludeTheseTags.Count == 0 
            || !w.Tags.Any(x => mustExcludeTheseTags.Contains(x));
    }
}
