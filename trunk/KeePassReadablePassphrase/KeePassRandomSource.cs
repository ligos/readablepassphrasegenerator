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
using KeePassLib.Cryptography;

namespace KeePassReadablePassphrase
{
    public class KeePassRandomSource : MurrayGrant.ReadablePassphrase.Random.RandomSourceBase
    {
        private readonly CryptoRandomStream _Crs;
        public KeePassRandomSource()
        {
            var randomness = System.Security.Cryptography.RandomNumberGenerator.Create();
            var bytes = new byte[32];
            randomness.GetBytes(bytes);
            this._Crs = new CryptoRandomStream(CrsAlgorithm.Salsa20, bytes);
        }
        public KeePassRandomSource(byte[] seed)
        {
            this._Crs = new CryptoRandomStream(CrsAlgorithm.Salsa20, seed);
        }
        public KeePassRandomSource(CryptoRandomStream crs)
        {
            this._Crs = crs;
        }

        public override byte[] GetRandomBytes(int numberOfBytes)
        {
            return this._Crs.GetRandomBytes((uint)numberOfBytes);
        }
    }
}
