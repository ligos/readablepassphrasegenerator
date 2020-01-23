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
    /// Changes letters to uppercase at random.
    /// </summary>
    public sealed class UppercaseMutator : ICombinations, IMutator
    {
        /// <summary>
        /// A general instance designed to get a phrase to pass password requirements more than add serious entropy.
        /// </summary>
        public readonly static UppercaseMutator Basic = new UppercaseMutator();
        public UppercaseMutator()
        {
            When = UppercaseStyles.StartOfWord;
            NumberOfCharactersToCapitalise = 2;
        }

        public UppercaseStyles When { get; set; }
        public int NumberOfCharactersToCapitalise { get; set; }

        public void Mutate(StringBuilder passphrase, RandomSourceBase random)
        {
            _ = passphrase ?? throw new ArgumentNullException(nameof(passphrase));
            _ = random ?? throw new ArgumentNullException(nameof(random));

            if (this.When == UppercaseStyles.Never || this.NumberOfCharactersToCapitalise <= 0)
                return;

            // Make a list of characters which can be capitalised.
            var possibleCharactersToCapitalise = new List<int>();
            for (int i = 0; i < passphrase.Length; i++)
            {
                if ((i == 0 && Char.IsLetter(passphrase[i])) 
                    || (this.When == UppercaseStyles.StartOfWord && i > 0 && Char.IsWhiteSpace(passphrase[i-1]) && Char.IsLetter(passphrase[i]))
                    || (this.When == UppercaseStyles.Anywhere && Char.IsLetter(passphrase[i]))
                    )
                possibleCharactersToCapitalise.Add(i);
            }
            
            // Randomly choose up to the count allowed.
            var toCapitalise = new List<int>();
            var c = Math.Min(this.NumberOfCharactersToCapitalise, possibleCharactersToCapitalise.Count);
            while (c > 0)
            {
                var idx = random.Next(possibleCharactersToCapitalise.Count);
                toCapitalise.Add(possibleCharactersToCapitalise[idx]);
                possibleCharactersToCapitalise.RemoveAt(idx);
                
                c--;
            }

            // Actually capitalise.
            foreach (var idx in toCapitalise)
                passphrase[idx] = Char.ToUpper(passphrase[idx]);
        }

        public double CalculateExtraCombinations()
        {
            // TODO.
            return Double.NaN;
        }
    }

    public enum UppercaseStyles
    {
        Never = 0,
        StartOfWord,
        Anywhere,
    }

    public enum AllUppercaseStyles
    {
        Never = 0,
        StartOfWord = 1,
        Anywhere = 2,
        /// <summary>
        /// The UppercaseRunMutator
        /// </summary>
        RunOfLetters = 3,
        /// <summary>
        /// The UppercaseWordMutator
        /// </summary>
        WholeWord = 4,
    }
}
