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

namespace MurrayGrant.ReadablePassphrase.Words
{
    /// <summary>
    /// A dictionary is a collection of words categorised by parts of speach. 
    /// </summary>
    public class WordDictionary : Collection<Word>
    {
        public string LanguageCode { get; private set; }
        public string Name { get; private set; }

        #region Load and Parse Methods
        public void LoadFrom(Stream s)
        {
            this.LoadFrom(XmlReader.Create(s));
        }
        public void LoadFrom(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                    this.ParseElement(reader);
            }
        }

        private void ParseElement(XmlReader reader)
        {
            var node = reader.Name.ToLower();
            if (node == "dictionary")
                this.ParseDictionaryRoot(reader);
            else if (node == "article")
                this.Add(new Article(reader));
            else if (node == "demonstrative")
                this.Add(new Demonstrative(reader));
            else if (node == "personalpronoun")
                this.Add(new PersonalPronoun(reader));
            else if (node == "noun")
                this.Add(new Noun(reader));
            else if (node == "preposition")
                this.Add(new Preposition(reader));
            else if (node == "adjective")
                this.Add(new Adjective(reader));
            else if (node == "verb")
                this.Add(new Verb(reader));
            else if (node == "adverb")
                this.Add(new Adverb(reader));
            else
                throw new DictionaryParseException(String.Format("Unknown element named '{0}' found in dictionary.", node));
        }
        private void ParseDictionaryRoot(XmlReader reader)
        {
            int version;
            if (!Int32.TryParse(reader.GetAttribute("schemaVersion"), out version) || version > 1)
                throw new DictionaryParseException(String.Format("Unknown schemaVersion '{0}'.", reader.GetAttribute("schemaVersion")));
                
            this.LanguageCode = reader.GetAttribute("language");
            this.Name = reader.GetAttribute("name");
        }
        #endregion

        public Article TheArticle { get { return this.OfType<Article>().Single(); } }

        public T ChooseWord<T>(Random.RandomSourceBase randomness, IEnumerable<Word> alreadyChosen) where T : Word
        {
            return ChooseWord<T>(randomness, alreadyChosen, (w) => true);
        }
        public T ChooseWord<T>(Random.RandomSourceBase randomness, IEnumerable<Word> alreadyChosen, Func<T, bool> wordPredicate) where T : Word
        {
            var count = CountOf<T>();
            T result;
            int attempts = 0;
            const int maxAttempts = 100;

            do
            {
                var idx = randomness.Next(count);
                result = this.OfType<T>().ElementAt(idx);

                attempts++;
                if (attempts >= maxAttempts)
                    throw new ApplicationException(String.Format("Unable to choose a {1} at random after {0} attempts. This may indicate a very small dictionary or an impossible predicate (or the breakdown of statistical laws as we know them!).", maxAttempts, typeof(T).Name));
            } while (alreadyChosen.Contains(result) || !wordPredicate(result));

            return result;
        }

        public int CountOf<T>()
        {
            return this.OfType<T>().Count();
        }
    }
}
