// Copyright 2020 Murray Grant
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

namespace MurrayGrant.ReadablePassphrase
{
    /// <summary>
    /// Represents the number of combinations for a given phrase strength and dictionary.
    /// </summary>
    /// <remarks>
    /// Shortest only includes clauses which are required. It represents the shortest possible phrases (assuming all optional clauses are not chosen).
    /// Longest includes all optional clauses. It represents combinations for long phrases (assuming all optional clauses are chosen).
    /// Both allows for all possible clause combinations (optional or otherwise). It represents combinations for short and long phrases together.
    /// OptionalAverage shows the combinations by using the weightings for optional clauses. It represents the average phrase. It's the least accurate of all the numbers.
    /// </remarks>
    public class PhraseCombinations
    {
        public static readonly PhraseCombinations Zero = new PhraseCombinations(0, 0, 0);
        public static readonly PhraseCombinations One = new PhraseCombinations(1.0, 1.0, 1.0);

        /// <summary>
        /// Only includes clauses which are required; representing the shortest possible phrases.
        /// </summary>
        public double Shortest { get; private set; }
        /// <summary>
        /// Incudes the longest phrases and extra combinations for optionality (ie: optional clauses add an extra combination); representing the absolute maximum combinations.
        /// </summary>
        public double Longest { get; private set; }
        /// <summary>
        /// Attempts to show the weighted average of combinations if optional clauses are excluded; tries to represent a realistic point between Shortest and Both.
        /// </summary>
        public double OptionalAverage { get; private set; }

        public double ShortestAsEntropyBits { get { return this.Shortest <= 0 ? -1 : Math.Log(this.Shortest, 2); } }
        public double LongestAsEntropyBits { get { return this.Longest <= 0 ? -1 : Math.Log(this.Longest, 2); } }
        public double OptionalAverageAsEntropyBits { get { return this.OptionalAverage <= 0 ? -1 : Math.Log(this.OptionalAverage, 2); } }

        public PhraseCombinations(double shortest, double longest, double optionalAverage)
        {
            if (shortest < 0)
                throw new ArgumentOutOfRangeException("shortest", "Shortest number must be zero or greater.");
            if (longest < 0)
                throw new ArgumentOutOfRangeException("longest", "Longest number must be zero or greater.");
            if (optionalAverage < 0)
                throw new ArgumentOutOfRangeException("optionalAverage", "OptionalAverage number must be zero or greater.");
            this.Shortest = shortest;
            this.Longest = longest;
            this.OptionalAverage = optionalAverage;
        }

        public override string ToString()
        {
            return this.ToString("E3", System.Globalization.CultureInfo.CurrentCulture);
        }
        public string ToString(IFormatProvider provider)
        {
            return this.ToString("E3", provider);
        }
        public string ToString(string format)
        {
            return this.ToString(format, System.Globalization.CultureInfo.CurrentCulture);
        }
        public string ToString(string format, IFormatProvider provider)
        {
            return this.Shortest.ToString(format, provider) + ", " + this.OptionalAverage.ToString(format, provider) + ", " + this.Longest.ToString(format, provider);
        }

        public string EntropyBitsToString()
        {
            return this.EntropyBitsToString("N2", System.Globalization.CultureInfo.CurrentCulture);
        }
        public string EntropyBitsToString(IFormatProvider provider)
        {
            return this.EntropyBitsToString("N2", provider);
        }
        public string EntropyBitsToString(string format)
        {
            return this.EntropyBitsToString(format, System.Globalization.CultureInfo.CurrentCulture);
        }
        public string EntropyBitsToString(string format, IFormatProvider provider)
        {
            return this.ShortestAsEntropyBits.ToString(format, provider) + ", " + this.OptionalAverageAsEntropyBits.ToString(format, provider) + ", " + this.LongestAsEntropyBits.ToString(format, provider);
        }

        public static PhraseCombinations operator +(PhraseCombinations left, PhraseCombinations right)
        {
            return new PhraseCombinations(left.Shortest + right.Shortest, left.Longest + right.Longest, left.OptionalAverage + right.OptionalAverage);
        }
        public static PhraseCombinations operator *(PhraseCombinations left, PhraseCombinations right)
        {
            return new PhraseCombinations(CoerceZeroToOne(left.Shortest) * CoerceZeroToOne(right.Shortest), 
                                          CoerceZeroToOne(left.Longest) * CoerceZeroToOne(right.Longest), 
                                          CoerceZeroToOne(left.OptionalAverage) * CoerceZeroToOne(right.OptionalAverage));
        }
        private static double CoerceZeroToOne(double d)
        {
            return d == 0.0 ? 1.0 : d;
        }
    }
}
