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
    public abstract class Word : IEquatable<Word>, IComparable<Word>
    {
        public abstract string DictionaryEntry { get; }
        public abstract Type OfType { get; }

        public override string ToString()
        {
            return this.GetType().Name + ": " + DictionaryEntry;
        }
        public override bool Equals(object obj)
            => obj is Word w && Equals(w);
        public virtual bool Equals(Word w)
            => w != null && String.Equals(w.DictionaryEntry, this.DictionaryEntry, StringComparison.OrdinalIgnoreCase);
        public override int GetHashCode()
            => StringComparer.OrdinalIgnoreCase.GetHashCode(this.DictionaryEntry) ^ this.GetType().GetHashCode();

        public int CompareTo(Word other)
            => other == null ? 1
            : StringComparer.OrdinalIgnoreCase.Compare(this.DictionaryEntry, other.DictionaryEntry);

        // Returns all the forms a word can take.
        public abstract IEnumerable<string> AllForms();
    }
}
