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
        public abstract int Version { get; }

        public virtual Article TheArticle { get { return this.GetWordAtIndex<Article>(0); } }

        protected Dictionary<Type, List<Word>> WordsByType { get; private set; }
        /// <summary>
        /// Dictionary loaders should call this to initialise a dictionary of word type (part of speach) -> ordered list of words.
        /// Gives an order of magnitude performance benefit over linear lookups.
        /// </summary>
        public void InitWordsByTypeLookup()
        {
            var result = this.GroupBy(w => w.OfType)
                            .ToDictionary(ws => ws.Key, ws => ws.OrderBy(w => w).ToList());
            // Make sure all the different parts of speach are in the dictionary (even if they have empty lists).
            var allWordTypes = new[] { typeof(Adjective), typeof(Adverb), typeof(Article), typeof(Demonstrative), typeof(Noun), typeof(PersonalPronoun), typeof(Preposition), typeof(Verb), typeof(Interrogative) };
            foreach (var t in allWordTypes)
            {
                if (!result.ContainsKey(t))
                    result.Add(t, new List<Word>());
            }
            WordsByType = result;
        }

        public T GetWordAtIndex<T>(int idx) where T : Word
        {
            if (WordsByType != null)
                return (T)WordsByType[typeof(T)][idx];
            else
                return (T)this.OfType<T>().ElementAt(idx);
        }

        public int CountOf<T>()
        {
            if (WordsByType != null)
                return WordsByType[typeof(T)].Count;
            else 
                return this.OfType<T>().Count();
        }
        public int CountOf<T>(Func<T, bool> predicate)
        {
            return this.OfType<T>().Count(predicate);
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
