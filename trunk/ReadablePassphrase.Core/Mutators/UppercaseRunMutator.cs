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
    /// Changes runs or sequences of letters to uppercase at random.
    /// </summary>
    public sealed class UppercaseRunMutator : ICombinations, IMutator
    {
        /// <summary>
        /// A general instance designed to get a phrase to pass password requirements more than add serious entropy.
        /// </summary>
        public readonly static UppercaseRunMutator Basic = new UppercaseRunMutator();
        public UppercaseRunMutator()
        {
            // Default to a single run of 3 characters.
            this.NumberOfCharactersInRun = 3;       
            this.NumberOfRuns = 1;
        }

        public int NumberOfCharactersInRun { get; set; }
        public int NumberOfRuns { get; set; }

        public void Mutate(StringBuilder passphrase, RandomSourceBase random)
        {
            if (this.NumberOfRuns <= 0 || this.NumberOfCharactersInRun <= 0)
                return;

            // Note: the logic here does not prevent multiple runs being adjacent or overlapping, which isn't that good.

            // Make a list of indexes which can be capitalised. 
            // The word must have, at least, the number of characters in the run.
            var possibleStartIndexes = new List<int>();
            for (int i = 0; i < passphrase.Length; i++)
            {
                // The start of a run is any letter, and not near the end of the phrase.
                if (Char.IsLetter(passphrase[i]) && ((passphrase.Length - i) - this.NumberOfCharactersInRun) > 0)
                {
                    // But we need to check there are enough letters afterwards too.
                    bool canAdd = true;
                    for (int j = 0; j < this.NumberOfCharactersInRun; j++)
                        canAdd &= Char.IsLetter(passphrase[i+j]);
                    if (canAdd)
                        possibleStartIndexes.Add(i);
                }
            }
            
            // Randomly choose up to the count allowed.
            var toCapitalise = new List<int>();
            var c = Math.Min(this.NumberOfRuns, possibleStartIndexes.Count);
            while (c > 0)
            {
                var idx = random.Next(possibleStartIndexes.Count);
                toCapitalise.Add(possibleStartIndexes[idx]);
                possibleStartIndexes.RemoveAt(idx);
                
                c--;
            }

            // Actually capitalise.
            foreach (var idx in toCapitalise)
            {
                for (int i = 0; i < this.NumberOfCharactersInRun; i++)
                    passphrase[i+idx] = Char.ToUpper(passphrase[i+idx]);
            }
        }

        public double CalculateExtraCombinations()
        {
            // TODO.
            return Double.NaN;
        }
    }
}
