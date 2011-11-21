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
using System.Security.Cryptography;
using System.IO;

namespace RandomiseWordList
{
    class Program
    {
        static void Main(string[] args)
        {
            const string InputWordList = "scowl wordlist up to 50.txt";
            const string OutputWordList = "randomised scowl list.txt";

            // Read the word list.
            var bytesForULong = new byte[8];
            var random = new RNGCryptoServiceProvider();
            var words = new List<Tuple<string, UInt64>>();
            using(var inStream = File.OpenText(InputWordList))
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
                    ulong sortOrder = BitConverter.ToUInt64(bytesForULong, 0);

                    // Add to list.
                    words.Add(new Tuple<string,ulong>(word, sortOrder));
                }
            }

            // Sort in a random order, based on the assigned ULong.
            var randomisedWords = words.OrderBy(w => w.Item2).Select(w => w.Item1);

            // Save the new word list.
            File.WriteAllLines(OutputWordList, randomisedWords, Encoding.UTF8);
        }
    }
}
