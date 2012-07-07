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

namespace MurrayGrant.ReadablePassphrase.Words
{
    /// <summary>
    /// Represents each word in a dictionary.
    /// </summary>
    public abstract class Word : IEquatable<Word>
    {
        public abstract string DictionaryEntry { get; }

        public override string ToString()
        {
            return this.GetType().Name + ": " + DictionaryEntry;
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is Word))
                return false;

            return ((Word)obj).DictionaryEntry.ToLower() == this.DictionaryEntry.ToLower();
        }
        public virtual bool Equals(Word obj)
        {
            if (obj == null)
                return false;
            return obj.DictionaryEntry.ToLower() == this.DictionaryEntry.ToLower();
        }
        public override int GetHashCode()
        {
            return this.DictionaryEntry.GetHashCode() ^ this.GetType().GetHashCode();
        }
    }
}
