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
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using MurrayGrant.ReadablePassphrase.Helpers;
using System.IO;

namespace MurrayGrant.ReadablePassphrase.Dictionaries
{
    /// <summary>
    /// Default dictionary.
    /// </summary>
    public static class Default
    {
        private const string DictionaryResourceName = "MurrayGrant.ReadablePassphrase.dictionary.xml.gz";

        /// <summary>
        /// Load the default dictionary from the embedded resource.
        /// </summary>
        /// <param name="excludeTags">Zero or more tags to exclude words from the passphrase. Eg: pass <c>"fake"</c> to exclude all fake words.</param>
        public static WordDictionary Load(IReadOnlyList<string>? excludeTags = null)
        {
            var loader = new ExplicitXmlDictionaryLoader();
            using (var s = Stream())
            {
                WordDictionary result = loader.LoadFrom(s, excludeWordsWithTags: excludeTags);
                return result;
            }
        }

        /// <summary>
        /// Gets the raw dictionary stream. A gz compressed xml file.
        /// </summary>
        public static Stream Stream() => typeof(Default).GetAssembly().GetManifestResourceStream(DictionaryResourceName);
    }
}
