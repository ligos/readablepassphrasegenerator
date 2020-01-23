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
using MurrayGrant.ReadablePassphrase.Words;

namespace MurrayGrant.ReadablePassphrase.Dictionaries
{
    /// <summary>
    /// A dictionary is a collection of words categorised by parts of speech. 
    /// </summary>
    public class ExplicitXmlWordDictionary : WordDictionary
    {
        private readonly string _LanguageCode;
        private readonly string _Name;
        private readonly int _Version;

        public override string LanguageCode { get { return _LanguageCode; } }
        public override string Name { get { return _Name; } }
        public override int Version { get { return _Version; } }

        public ExplicitXmlWordDictionary() : this("EMPTY", "en", 1) { }
        public ExplicitXmlWordDictionary(string name, string languageCode) : this(name, languageCode, 1) { }
        public ExplicitXmlWordDictionary(string name, string languageCode, int version) : base() 
        {
            _Name = name;
            _LanguageCode = languageCode;
            _Version = version;
        }

        protected internal void ExpandCapacityTo(int count)
        {
            if (this.Items is List<Word> asList && count > 100 && asList.Capacity < count)
                asList.Capacity = count;
        }
    }
}
