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

namespace MurrayGrant.ReadablePassphrase.Random
{
    // TODO: alternative random source (could be used as seeds or random streams in their own right).
    //   http://www.random.org - http://www.random.org/cgi-bin/randbyte?nbytes=1024&format=f
    //   A bunch of dice rolls. Each roll represents ~2.58 bits, requiring at least 13 rolls for 32 bytes.
    
    /// <summary>
    /// An abstract source of random bytes (with various easier to use methods)
    /// </summary>
    public abstract class RandomSourceBase : IRandomSourceBase
    {
        public abstract byte[] GetRandomBytes(int numberOfBytes);

        public bool CoinFlip()
        {
            var bytes = this.GetRandomBytes(1);
            return ((bytes[0] & (byte)0x01) == (byte)0x01);
        }
        public bool WeightedCoinFlip(int trueWeight, int falseWeight)
        {
            if (trueWeight <= 0 && falseWeight <= 0)
                throw new ArgumentException("Either true or false weighting must be positive.");
            if (trueWeight <= 0)
                return false;
            if (falseWeight <= 0)
                return true;
            return Next(falseWeight + trueWeight) < trueWeight;
        }

        // Implementation for Next() based on http://codereview.stackexchange.com/questions/6304/algorithm-to-convert-random-bytes-to-integers
        public int Next()
        {
            byte[] bytes = this.GetRandomBytes(4);
            int i = BitConverter.ToInt32(bytes, 0);
            return i & Int32.MaxValue;
        }
        public int Next(int maxExlusive)
        {
            if (maxExlusive <= 0) throw new ArgumentOutOfRangeException("maxExlusive", maxExlusive, "maxExlusive must be positive");

            // Let k = (Int32.MaxValue + 1) % maxExcl
            // Then we want to exclude the top k values in order to get a uniform distribution
            // You can do the calculations using uints if you prefer to only have one %
            int k = ((Int32.MaxValue % maxExlusive) + 1) % maxExlusive;
            int result = this.Next();
            while (result > Int32.MaxValue - k)
                result = this.Next();
            return result % maxExlusive;
        }
        public int Next(int minValue, int maxValue)
        {
            if (minValue < 0)
                throw new ArgumentOutOfRangeException("minValue", minValue, "minValue must be non-negative");
            if (maxValue <= minValue)
                throw new ArgumentOutOfRangeException("maxValue", maxValue, "maxValue must be greater than minValue");

            return minValue + this.Next(maxValue - minValue);

        }
    }
}
