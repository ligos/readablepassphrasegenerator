// Copyright 2012 Murray Grant
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

namespace MurrayGrant.ReadablePassphrase.Dictionaries
{
    public static class DictionaryExtensions
    {
        public static T ChooseWord<T>(this WordDictionary dict, Random.RandomSourceBase randomness, IEnumerable<Word> alreadyChosen) where T : Word
        {
            return ChooseWord<T>(dict, randomness, alreadyChosen, (w) => true);
        }
        public static T ChooseWord<T>(this WordDictionary dict, Random.RandomSourceBase randomness, IEnumerable<Word> alreadyChosen, Func<T, bool> wordPredicate) where T : Word
        {
            var count = dict.CountOf<T>();
            T result;
            int attempts = 0;
            const int maxAttempts = 5;

            do
            {
                var idx = randomness.Next(count);
                result = dict.GetWordAtIndex<T>(idx);

                attempts++;
                if (attempts >= maxAttempts)
                    // This way is much slower when lots of words match the predicate, but faster if few do.
                    return ChooseWordAlternate<T>(dict, randomness, alreadyChosen, wordPredicate);
            } while (alreadyChosen.Contains(result) || !wordPredicate(result));

            return result;
        }
        private static T ChooseWordAlternate<T>(this WordDictionary dict, Random.RandomSourceBase randomness, IEnumerable<Word> alreadyChosen, Func<T, bool> wordPredicate) where T : Word
        {
            var possibleWords = dict.OfType<T>().Where(w => wordPredicate(w) && !alreadyChosen.Contains(w)).ToList();
            var matchingWordCount = possibleWords.Count;
            if (matchingWordCount == 0)
                throw new ApplicationException(String.Format("Unable to choose a {0} at random. There are no words which match the specified predicate which are not already chosen.", typeof(T).Name));
            var idx = randomness.Next(matchingWordCount);
            var result = possibleWords[idx];
            return result;
        }
    }
}
