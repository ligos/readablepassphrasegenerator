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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using MurrayGrant.ReadablePassphrase.Words;

namespace MurrayGrant.ReadablePassphrase.Dictionaries
{
    /// <summary>
    /// A dictionary is a collection of words categorised by parts of speach. 
    /// </summary>
    public abstract class WordDictionary : Collection<Word>
    {
        public abstract string LanguageCode { get; }
        public abstract string Name { get; }

        public virtual Article TheArticle { get { return this.OfType<Article>().Single(); } }

        public int CountOf<T>()
        {
            return this.OfType<T>().Count();
        }

        public WordDictionary() : base() { }
        public WordDictionary(IList<Word> words) : base(words) { }
        public WordDictionary(IEnumerable<Word> words) : base() { AddMany(words); }

        public void AddMany(IEnumerable<Word> words)
        {
            var asList = this.Items as List<Word>;
            if (asList != null)
                asList.AddRange(words);
            else
            {
                foreach (var w in words)
                    this.Add(w);
            }
        }
    }
}
