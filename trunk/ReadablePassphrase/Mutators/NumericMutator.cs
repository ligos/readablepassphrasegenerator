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
    /// Adds numbers to a passphrase at random.
    /// </summary>
    public sealed class NumericMutator : ICombinations, IMutator
    {
        /// <summary>
        /// A general instance designed to get a phrase to pass password requirements more than add serious entropy.
        /// </summary>
        public readonly static NumericMutator Basic = new NumericMutator()
        {
            When = NumericStyles.EndOfWord,
            NumberOfNumbersToAdd = 2,
        };

        public NumericStyles When { get; set; }
        public int NumberOfNumbersToAdd { get; set; }
        public char[] Numbers { get; set; }

        public NumericMutator()
        {
            this.Numbers = "0123456789".ToCharArray();
        }

        public void Mutate(StringBuilder passphrase, RandomSourceBase random)
        {
            if (this.When == NumericStyles.Never || this.NumberOfNumbersToAdd <= 0)
                return;

            // Make a list of positions which can have numbers inserted.
            var possibleInsertIndexes = new List<int>();
            for (int i = 0; i <= passphrase.Length; i++)
            {
                if (
                    ((this.When & NumericStyles.Anywhere) == NumericStyles.Anywhere)
                    || ((this.When & NumericStyles.StartOfWord) == NumericStyles.StartOfWord && 
                        ((i == 0) || (i > 0 && Char.IsWhiteSpace(passphrase[i-1]) && Char.IsLetter(passphrase[i])))
                        ) 
                    || ((this.When & NumericStyles.EndOfWord) == NumericStyles.EndOfWord && 
                        ((i == passphrase.Length) || (i < passphrase.Length && Char.IsWhiteSpace(passphrase[i]) && Char.IsLetter(passphrase[i-1])))
                        )
                    )
                    possibleInsertIndexes.Add(i);
            }
            
            // Randomly choose up to the count allowed.
            var toInsertAt = new List<int>();
            var c = Math.Min(this.NumberOfNumbersToAdd, possibleInsertIndexes.Count);
            while (c > 0)
            {
                var idx = random.Next(possibleInsertIndexes.Count);
                toInsertAt.Add(possibleInsertIndexes[idx]);
                possibleInsertIndexes.RemoveAt(idx);
                
                c--;
            }

            // Actually insert numbers.
            // Because we're inserting, we do it in reverse order of index, so we don't need to adjust indexes.
            toInsertAt.Sort();
            toInsertAt.Reverse();
            foreach (var idx in toInsertAt)
                passphrase.Insert(idx, this.Numbers[random.Next(this.Numbers.Length)]);
        }

        public double CalculateExtraCombinations()
        {
            // TODO.
            return Double.NaN;
        }
    }
    
    [Flags]
    public enum NumericStyles
    {
        Never = 0,
        StartOfWord = 1,
        EndOfWord = 2,
        StartOrEndOfWord = StartOfWord | EndOfWord,
        Anywhere = 4,
    }
}
