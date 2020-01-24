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
using System.Xml;

namespace MergePartsOfSpeech
{
    class Program
    {
        static void Main(string[] args)
        {
            // Base must be first.
            string[] files = new string[] { "base.xml", "nouns.xml", "properNouns.xml", "prepositions.xml", "adjectives.xml", "verbs.xml", "adverbs.xml" };
            string output = @".\bin\dictionary.xml";

            // The dictionary is too hard to edit / append to in a monolithic form.
            // This takes the various components and glues them into a single dictionary file for use by all other components.

            using (var dictionaryStream = new System.IO.StreamWriter(output, false, Encoding.UTF8))
            {
                bool first = true;
                foreach (var input in files)
                {
                    using (var inStream = new System.IO.StreamReader(input, Encoding.UTF8))
                    {
                        bool inDictionaryElement = false;
                        while (!inStream.EndOfStream)
                        {
                            var line = inStream.ReadLine();
                            if (line == null)
                                break;

                            if (first && !line.StartsWith("</dictionary>", StringComparison.CurrentCultureIgnoreCase))
                                dictionaryStream.WriteLine(line);
                            else if (!first && !inDictionaryElement && line.StartsWith("<dictionary", StringComparison.CurrentCultureIgnoreCase))
                                inDictionaryElement = true;
                            else if (!first && inDictionaryElement && !line.StartsWith("</dictionary>", StringComparison.CurrentCultureIgnoreCase))
                                dictionaryStream.WriteLine(line);
                        }
                    }

                    first = false;
                }
                dictionaryStream.WriteLine();
                dictionaryStream.WriteLine("</dictionary>");
            }
        }
    }
}
