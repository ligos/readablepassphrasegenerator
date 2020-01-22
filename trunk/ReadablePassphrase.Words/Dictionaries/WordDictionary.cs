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
    /// A dictionary is a collection of words categorised by parts of speech. 
    /// </summary>
    public abstract class WordDictionary : Collection<Word>
    {
        public abstract string LanguageCode { get; }
        public abstract string Name { get; }
        public abstract int Version { get; }

        public virtual Article TheArticle { get { return this.GetWordAtIndex<Article>(0); } }

        protected Dictionary<Type, List<Word>> WordsByType { get; private set; } = new Dictionary<Type, List<Word>>();
        protected int TransitiveVerbCount { get; private set; }
        protected int IntransitiveVerbCount { get; private set; }

        /// <summary>
        /// Dictionary loaders should call this to initialise a dictionary of word type (part of speech) -> ordered list of words.
        /// Gives an order of magnitude performance benefit over linear lookups.
        /// </summary>
        public void InitWordsByTypeLookup()
        {
            var result = this.GroupBy(w => w.OfType)
                            .ToDictionary(ws => ws.Key, ws => ws.OrderBy(w => w).ToList());
            // Make sure all the different parts of speech are in the dictionary (even if they have empty lists).
            var allWordTypes = new[] { typeof(Adjective), typeof(Adverb), typeof(Article), typeof(Demonstrative), typeof(Noun), typeof(ProperNoun), typeof(PersonalPronoun), typeof(Preposition), typeof(Verb), typeof(Interrogative), typeof(Conjunction), typeof(SpeechVerb), typeof(IndefinitePronoun), typeof(Number) };
            foreach (var t in allWordTypes)
            {
                if (!result.ContainsKey(t))
                    result.Add(t, new List<Word>());
            }
            WordsByType = result;
            
            // Count the number of transitive and intransitive verbs.
            TransitiveVerbCount = result[typeof(Verb)].OfType<Verb>().Count(v => v.IsTransitive);
            IntransitiveVerbCount = result[typeof(Verb)].Count - TransitiveVerbCount;
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
            _ = predicate ?? throw new ArgumentNullException(nameof(predicate));
            return this.OfType<T>().Count(predicate);
        }
        public int CountOfTransitiveVerbs()
        {
            return TransitiveVerbCount;
        }
        public int CountOfIntransitiveVerbs()
        {
            return IntransitiveVerbCount;
        }
        public int CountOfAllDistinctForms()
        {
            return this.SelectMany(w => w.AllForms()).Distinct(StringComparer.CurrentCultureIgnoreCase).Count();
        }

        public WordDictionary() : base() { }
        public WordDictionary(IList<Word> words) : base(words) { }
        public WordDictionary(IEnumerable<Word> words) : base() { AddMany(words); }

        public void AddMany(IEnumerable<Word> words)
        {
            _ = words ?? throw new ArgumentNullException(nameof(words));

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

