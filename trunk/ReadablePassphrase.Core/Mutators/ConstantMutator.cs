// Copyright 2019 Murray Grant
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
    /// Adds a constant string to a passphrase at random.
    /// </summary>
    public sealed class ConstantMutator : ICombinations, IMutator
    {
        /// <summary>
        /// A general instance designed to get a phrase to pass password requirements more than add serious entropy.
        /// </summary>
        public readonly static ConstantMutator Basic = new ConstantMutator();

        public ConstantStyles When { get; set; }
        public string ValueToAdd { get; set; }
        public char Whitespace { get; set; }

        public ConstantMutator()
        {
            this.ValueToAdd = ".";
            this.Whitespace = ' ';
            this.When = ConstantStyles.EndOfPhrase;
        }

        public void Mutate(StringBuilder passphrase, IRandomSourceBase random)
        {
            _ = passphrase ?? throw new ArgumentNullException(nameof(passphrase));
            _ = random ?? throw new ArgumentNullException(nameof(random));

            if (this.When == ConstantStyles.Never)
                return;

            // Make a list of positions which can have the constant inserted.
            var possibleInsertIndexes = new List<int>();

            if (this.When == ConstantStyles.StartOfPhrase || this.When == ConstantStyles.Anywhere)
            {
                // Start of passphrase is an easy case.
                possibleInsertIndexes.Add(0);
            }

            if (this.When == ConstantStyles.EndOfPhrase || this.When == ConstantStyles.Anywhere)
            {
                // End of passphrase is an easy case.
                // Although we still need to check for whitespace at the end.
                for (int i = passphrase.Length - 1; i >= 0; i--)
                {
                    if (passphrase[i] != this.Whitespace)
                    {
                        possibleInsertIndexes.Add(i + 2);
                        break;
                    }
                }
            } 

            if (this.When == ConstantStyles.MiddleOfPhrase || this.When == ConstantStyles.Anywhere)
            {
                // If we can look in the middle, we need to find word boundaries.
                for (int i = 1; i <= passphrase.Length - 2; i++)
                {
                    if (passphrase[i] == this.Whitespace && Char.IsLetterOrDigit(passphrase[i - 1]))
                        possibleInsertIndexes.Add(i+1);
                }
            }


            // Randomly choose an index.
            var toInsertAt = Int32.MinValue;
            if (possibleInsertIndexes.Count > 0)
            {
                var idx = random.Next(possibleInsertIndexes.Count);
                toInsertAt = possibleInsertIndexes[idx];
            }

            if (toInsertAt == passphrase.Length)
            {
                // Actually insert the constant at the end of the phrase (without whitespace).
                passphrase.Insert(toInsertAt-1, this.ValueToAdd);
            }
            else if (toInsertAt >= 0)
            {
                // Actually insert the constant (plus whitespace, in reverse order).
                passphrase.Insert(toInsertAt, this.Whitespace)
                          .Insert(toInsertAt, this.ValueToAdd);
            }
        }

        public double CalculateExtraCombinations()
        {
            // TODO.
            return Double.NaN;
        }
    }
    
    [Flags]
    public enum ConstantStyles
    {
        Never = 0,
        StartOfPhrase = 1,
        MiddleOfPhrase = 2,
        EndOfPhrase = 3,
        Anywhere = 4,
    }
}
