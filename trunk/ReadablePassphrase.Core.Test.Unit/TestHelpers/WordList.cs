using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;

namespace MurrayGrant.ReadablePassphrase.TestHelpers
{
    public static class WordList
    {
        public static readonly Lazy<IReadOnlyList<string>> Words =
            new Lazy<IReadOnlyList<string>>(ReadWords, LazyThreadSafetyMode.ExecutionAndPublication);

        const string InputWordList = "scowl wordlist up to 50.txt";
        private static IReadOnlyList<string> ReadWords()
        {
            // Read the word list.
            var bytesForULong = new byte[8];
            var random = new RNGCryptoServiceProvider();
            var words = new List<(string word, ulong sortOrder)>();
            using (var inStream = File.OpenText(InputWordList))
            {
                while (!inStream.EndOfStream)
                {
                    // Read the word.
                    var word = inStream.ReadLine();
                    // Conditions to ignore the word.
                    if (word.EndsWith("'s"))
                        continue;
                    if (word.Length < 3)
                        continue;
                    if (word.Length >= 10)
                        continue;

                    // Create a random number to sort by.
                    random.GetBytes(bytesForULong);
                    var sortOrder = BitConverter.ToUInt64(bytesForULong, 0);

                    // Add to list.
                    words.Add((word, sortOrder));
                }
            }

            // Sort in a random order, based on the assigned ULong.
            return words.OrderBy(w => w.sortOrder).Select(w => w.word).ToList();
        }

        public static IReadOnlyList<string> Pick(int n)
        {
            var rnd = new System.Random();
            var lim = Words.Value.Count;
            return Enumerable.Range(0, n)
                .Select(_ => Words.Value[rnd.Next(lim)])
                .ToList();
        }
    }
}
