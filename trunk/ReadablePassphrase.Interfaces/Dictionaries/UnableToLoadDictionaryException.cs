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

namespace MurrayGrant.ReadablePassphrase.Dictionaries
{
    /// <summary>
    /// Thrown if an <c>IDictionaryLoader</c> cannot load the dictionary.
    /// </summary>
#if !NETSTANDARD
    [Serializable]
#endif
    public class UnableToLoadDictionaryException : Exception
    {
        public UnableToLoadDictionaryException() { }
        public UnableToLoadDictionaryException(string message) : base(message) { }
        public UnableToLoadDictionaryException(string message, Exception inner) : base(message, inner) { }
#if !NETSTANDARD
        protected UnableToLoadDictionaryException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
#endif
    }
}
