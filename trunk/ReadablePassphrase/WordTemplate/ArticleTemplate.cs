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
    public class ArticleTemplate : Template
    {
        private readonly static char[] VowelSounds = new char[] { 'a', 'e', 'i', 'o', 'u' };
        public readonly bool IsDefinite;
        public override bool IncludeInAlreadyUsedList { get { return false; } }
        public ArticleTemplate(bool isDefinite)
        {
            this.IsDefinite = isDefinite;
        }

        public override WordAndString ChooseWord(WordDictionary words, Random.RandomSourceBase randomness, IEnumerable<Word> alreadyChosen)
        {
            // This won't always produce the correct result.
            if (this.IsDefinite)
                return new WordAndString(words.TheArticle, words.TheArticle.Definite);
            else 
                return new WordAndString(words.TheArticle, words.TheArticle.Indefinite);
        }

        public string ChooseBasedOnFollowingWord(WordDictionary words, string nextWord)
        {
            if (this.IsDefinite)
                return words.TheArticle.Definite;
            else if (VowelSounds.Contains(nextWord[0]))
                return words.TheArticle.IndefiniteBeforeVowel;
            else
                return words.TheArticle.Indefinite;
        }
    }
}
