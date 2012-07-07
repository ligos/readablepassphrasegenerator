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
            const int maxAttempts = 100;

            do
            {
                var idx = randomness.Next(count);
                result = dict.OfType<T>().ElementAt(idx);

                attempts++;
                if (attempts >= maxAttempts)
                    throw new ApplicationException(String.Format("Unable to choose a {1} at random after {0} attempts. This may indicate a very small dictionary or an impossible predicate (or the breakdown of statistical laws as we know them!).", maxAttempts, typeof(T).Name));
            } while (alreadyChosen.Contains(result) || !wordPredicate(result));

            return result;
        }

    }
}
