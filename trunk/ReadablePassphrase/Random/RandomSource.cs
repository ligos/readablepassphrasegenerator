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
    public abstract class RandomSourceBase
    {
        public abstract byte[] GetRandomBytes(int numberOfBytes);

        public bool CoinFlip()
        {
            var bytes = this.GetRandomBytes(1);
            return ((bytes[0] & (byte)0x01) == (byte)0x01);
        }
        public bool WeightedCoinFlip(int trueWeight, int falseWeight)
        {
            if (trueWeight == 0)
                return false;
            if (falseWeight == 0)
                return true;
            return Next(falseWeight + trueWeight) >= trueWeight;
        }

        public int Next()
        {
            return Next(0, Int32.MaxValue);
        }
        public int Next(int maxValue)
        {
            return Next(0, maxValue);
        }
        public int Next(int minValue, int maxValue)
        {
            if (minValue < 0)
                throw new ArgumentOutOfRangeException("minValue", minValue, "MinValue must be greater than or equal to zero.");
            if (maxValue <= minValue)
                throw new ArgumentOutOfRangeException("maxValue", maxValue, "MaxValue must be greater than minValue.");

            int range = maxValue - minValue;
            if (range == 1)     // Trivial case.
                return minValue;
            int bitsRequired = (int)Math.Ceiling(Math.Log(range, 2) + 1);
            int bitmask = (1 << bitsRequired) - 1;

            int result = -1;
            while (result < 0 || result > range - 1)
            {
                var bytes = this.GetRandomBytes(4);
                result = (Math.Abs(BitConverter.ToInt32(bytes, 0)) & bitmask) - 1;
            }
            return result + minValue;
        }
    }
}
