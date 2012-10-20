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
