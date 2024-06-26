﻿// Copyright 2012 Murray Grant
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
    public class InterrogativeTemplate : Template
    {
        public override bool IncludeInAlreadyUsedList { get { return false; } }
        public readonly bool SubjectIsPlural;

        public InterrogativeTemplate(bool subjectIsPlural)
        {
            SubjectIsPlural = subjectIsPlural;
        }
        public override WordAndString ChooseWord(WordDictionary words, Random.RandomSourceBase randomness, IEnumerable<Word> alreadyChosen)
        {
            _ = words ?? throw new ArgumentNullException(nameof(words));
            _ = randomness ?? throw new ArgumentNullException(nameof(randomness));
            _ = alreadyChosen ?? throw new ArgumentNullException(nameof(alreadyChosen));

            var word = words.ChooseWord<Interrogative>(randomness, alreadyChosen, True);
            return new WordAndString(word, SubjectIsPlural ? word.Plural : word.Singular);
        }
    }
}
