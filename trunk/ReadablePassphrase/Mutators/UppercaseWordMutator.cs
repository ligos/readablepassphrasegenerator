// Copyright 2014 Murray Grant
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
using MurrayGrant.ReadablePassphrase.Random;

namespace MurrayGrant.ReadablePassphrase.Mutators
{
    /// <summary>
    /// Changes whole words to uppercase at random.
    /// </summary>
    public sealed class UppercaseWordMutator : ICombinations, IMutator
    {
        /// <summary>
        /// A general instance designed to get a phrase to pass password requirements more than add serious entropy.
        /// </summary>
        public readonly static UppercaseWordMutator Basic = new UppercaseWordMutator();
        public UppercaseWordMutator()
        {
            this.NumberOfWordsToCapitalise = 1;     // Default to a single word.
            this.MinimumWordLength = 3;             // Words must be at least 3 characters long to be capitalised.
        }

        public int NumberOfWordsToCapitalise { get; set; }
        public int MinimumWordLength { get; set; }

        public void Mutate(StringBuilder passphrase, RandomSourceBase random)
        {
            if (this.NumberOfWordsToCapitalise == 0)
                return;

            // Make a list of words which can be capitalised.
            var possibleWordIdxes = new List<int>();
            for (int i = 0; i < passphrase.Length; i++)
            {
                if ((i == 0 && Char.IsLetter(passphrase[i]))        // First word.
                    || (i > 0 && Char.IsWhiteSpace(passphrase[i - 1]) && Char.IsLetter(passphrase[i]))       // Any letter with the previous character being whitespace.
                    )
                    possibleWordIdxes.Add(i);       // The index of where the word starts.
            }
            
            // Ensure the words we choose are at least 3 characters long.
            int endOfWordIdx = passphrase.Length+1;
            for (int i = possibleWordIdxes.Count - 1; i >= 0; i--)
            {
                var len = endOfWordIdx - possibleWordIdxes[i];
                endOfWordIdx = possibleWordIdxes[i];
                if (len < this.MinimumWordLength)
                    possibleWordIdxes.RemoveAt(i);
            }

            // Randomly choose up to the count allowed.
            var toCapitalise = new List<int>();
            var c = Math.Min(this.NumberOfWordsToCapitalise, possibleWordIdxes.Count);
            while (c > 0)
            {
                var idx = random.Next(possibleWordIdxes.Count);
                toCapitalise.Add(possibleWordIdxes[idx]);
                possibleWordIdxes.RemoveAt(idx);
                
                c--;
            }

            // Actually capitalise.
            foreach (var idx in toCapitalise)
            {
                // Make capital until we hit whitespace or the end of the phrase.
                for(int i = idx; !Char.IsWhiteSpace(passphrase[i]) && i < passphrase.Length; i++)
                    passphrase[i] = Char.ToUpper(passphrase[i]);
            }
                
        }

        public double CalculateExtraCombinations()
        {
            // TODO.
            return Double.NaN;
        }
    }
}
