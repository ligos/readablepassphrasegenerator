// Copyright 2018 Murray Grant
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

namespace MurrayGrant.ReadablePassphrase
{
    public static class Generator
    {
        /// <summary>
        /// Creates an instance of the ReadablePassphraseGenerator. 
        /// The default dictionary (from ReadablePassphrase.DefaultDictionary) and system crypto random source as used by default.
        /// </summary>
        /// <param name="words">Use null for the default dictionary, or supply your own.</param>
        /// <param name="randomness">Use null for system crypto random source, or supply your own.</param>
        /// <returns></returns>
        /// <remarks>For further information see MurrayGrant.ReadablePassphrase.ReadablePassphraseGenerator</remarks>
        public static ReadablePassphraseGenerator Create(Dictionaries.WordDictionary? words = null, Random.RandomSourceBase? randomness = null)
        {
            var ws = words ?? MurrayGrant.ReadablePassphrase.Dictionaries.Default.Load();
            var rand = randomness ?? new Random.CryptoRandomSource();
            return new ReadablePassphraseGenerator(ws, rand);
        }
    }
}
