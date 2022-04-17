// Copyright 2012 Murray Grant
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
using System.IO;
using System.Xml;
using MurrayGrant.ReadablePassphrase.Words;
using MurrayGrant.ReadablePassphrase.MaterialisedWords;

namespace MurrayGrant.ReadablePassphrase.Dictionaries
{ 
    public class ExplicitXmlDictionaryLoader : IDictionaryLoader
    {
        private static readonly string[] _DefaultFilenames = new[] { "dictionary.xml", "dictionary.xml.gz", "dictionary.gz" };
        private static readonly IReadOnlyList<string> EmptyTags = new string[0];

        private ExplicitXmlWordDictionary? _Dict;
        private readonly Dictionary<string, Action<XmlReader>> _NodeLookup;
        private long _StreamSize;

        public ExplicitXmlDictionaryLoader()
        {
            // Lookup table for loading XML nodes.
            _NodeLookup = new [] {
                new KeyValuePair<string, Action<XmlReader>>("dictionary", ParseDictionaryRoot),
                new KeyValuePair<string, Action<XmlReader>>("article", ParseArticle),
                new KeyValuePair<string, Action<XmlReader>>("demonstrative", ParseDemonstrative),
                new KeyValuePair<string, Action<XmlReader>>("personalpronoun", ParsePersonalPronoun),
                new KeyValuePair<string, Action<XmlReader>>("noun", ParseNoun),
                new KeyValuePair<string, Action<XmlReader>>("propernoun", ParseProperNoun),
                new KeyValuePair<string, Action<XmlReader>>("preposition", ParsePreposition),
                new KeyValuePair<string, Action<XmlReader>>("adjective", ParseAdjective),
                new KeyValuePair<string, Action<XmlReader>>("adverb", ParseAdverb),
                new KeyValuePair<string, Action<XmlReader>>("verb", ParseVerb),
                new KeyValuePair<string, Action<XmlReader>>("interrogative", ParseInterrogative),
                new KeyValuePair<string, Action<XmlReader>>("conjunction", ParseConjunction),
                new KeyValuePair<string, Action<XmlReader>>("speechverb", ParseSpeechVerb),
                new KeyValuePair<string, Action<XmlReader>>("indefinitepronoun", ParseIndefinitePronoun),
                new KeyValuePair<string, Action<XmlReader>>("numberrange", ParseNumberRange),
            }.ToDictionary(x => x.Key, x => x.Value);
        }

        public WordDictionary Load(IDictionary<string, string>? arguments)
        {
            var args = new Dictionary<string, string>();        // Assume empty arguments on null.
            if (arguments != null)
                args = new Dictionary<string, string>(arguments
                                                        .Select(x => new KeyValuePair<string, string>(x.Key.Trim(), x.Value.Trim()))
                                                        .ToDictionary(x => x.Key, x => x.Value)
                                                    , StringComparer.CurrentCultureIgnoreCase);

            // See what's been set in the arguments.
            FileInfo? fileLocation = null;
            DirectoryInfo? dirLocation = null;
            Uri? urlLocation = null;
            bool isCompressedTemp;
            bool? isCompressed = null;
            bool useDefaultDictionaryTemp;
            bool? useDefaultDictionary = null;
            if (args.ContainsKey("file") && !String.IsNullOrEmpty(args["file"]))
                fileLocation = new FileInfo(args["file"]);
            else if (args.ContainsKey("dir") && !String.IsNullOrEmpty(args["dir"]))
                dirLocation = new DirectoryInfo(args["dir"]);
            else if (args.ContainsKey("url") && !String.IsNullOrEmpty(args["url"]))
                urlLocation = new Uri(args["url"]);
            if (args.ContainsKey("isCompressed") && Boolean.TryParse(args["isCompressed"], out isCompressedTemp)) 
                isCompressed = isCompressedTemp;
            if (args.ContainsKey("useDefaultDictionary") && Boolean.TryParse(args["useDefaultDictionary"], out useDefaultDictionaryTemp))
                useDefaultDictionary = useDefaultDictionaryTemp;
    
            // Based on what was passed in, call an appropriate LoadFrom() overload.
            if (fileLocation != null)
                return this.LoadFrom(fileLocation);
            else if (dirLocation != null)
                return this.LoadFrom(dirLocation);
            else if (urlLocation != null && !isCompressed.HasValue)
                throw new InvalidDictionaryLoaderArgumentException("If you are loading from a url, you must specify 'isCompressed=true|false'.");
            else if (urlLocation != null)
#if NETSTANDARD
                throw new InvalidDictionaryLoaderArgumentException("Loading from a URL is not supported in .NET Core; use System.Net.Http and pass your Stream / XmlReader to a LoadFrom() overload.");
#else
                return this.LoadFrom(urlLocation, isCompressed!.Value);
#endif
            else if (useDefaultDictionary == true)
#pragma warning disable CS0618
                return this.LoadFrom();
#pragma warning restore
            else
                throw new InvalidDictionaryLoaderArgumentException("Neither 'url' or 'file' parameters were specified. If you with to look for and load a defult dictionary.xml file in the current directory, specify 'useDefaultDictionary=true'.");
        }

        /// <summary>
        /// Loads the default dictionary.
        /// </summary>
        /// <remarks>
        /// This will attempt to load 'dictionary.xml, .xml.gz and .gz' from
        /// the folder of the exe (<c>Assembly.GetEntryAssembly()</c>; Windows only) or the current directory (<c>Environment.CurrentDirectory</c>; all platforms).
        /// 
        /// For information about the dictionary schema definition see the default xml file or github.
        /// </remarks>
        [Obsolete("Recommend using LoadFrom(DirectoryInfo) or LoadFrom(FileInfo) for consistancy across platforms.")]
        public ExplicitXmlWordDictionary LoadFrom()
        {
            // Check dictionary.xml, dictionary.xml.gz, dictionary.gz in entrypoint and current working directory.
            var allLocationsToCheck = Enumerable.Empty<string>();

#if !NETSTANDARD
            // Assembly.GetEntryAssembly() is only available in NetStandard 1.5
            allLocationsToCheck = allLocationsToCheck.Concat(_DefaultFilenames.Select(f => Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), f)));
#endif
            allLocationsToCheck = allLocationsToCheck.Concat(_DefaultFilenames.Select(f => Path.Combine(Directory.GetCurrentDirectory(), f)));
            
            foreach (var fileAndPath in allLocationsToCheck)
            {
                if (TryLoadDictionaryFromPath(fileAndPath, out var result))
                    return result!;
            }

            throw new UnableToLoadDictionaryException("Unable to load default dictionary. Tried the following locations: " + String.Join(", ", allLocationsToCheck));
        }
        private bool TryLoadDictionaryFromPath(string fileAndPath, out ExplicitXmlWordDictionary? dict)
        {
            if (!File.Exists(fileAndPath))
            {
                dict = null;
                return false;
            }
            try
            {
                using var stream = new FileStream(fileAndPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                dict = this.LoadFrom(stream);
                return true;
            }
            catch (Exception)
            {
                dict = null;
                return false;
            }
        }
        /// <summary>
        /// Loads a dictionary from the specified path.
        /// </summary>
        /// <remarks>
        /// The file can be plaintext or gzipped.
        /// 
        /// For information about the dictionary schema definition see the default xml file or github.
        /// </remarks>
        public ExplicitXmlWordDictionary LoadFrom(string pathToExternalFile)
        {
            if (String.IsNullOrEmpty(pathToExternalFile)) throw new ArgumentNullException(nameof(pathToExternalFile));
            return this.LoadFrom(new FileInfo(pathToExternalFile));
        }
        /// <summary>
        /// Loads a dictionary from the specified file.
        /// </summary>
        /// <remarks>
        /// The file can be plaintext or gzipped.
        /// 
        /// For information about the dictionary schema definition see the default xml file or github.
        /// </remarks>
        public ExplicitXmlWordDictionary LoadFrom(FileInfo externalFile)
        {
            if (externalFile == null) throw new ArgumentNullException(nameof(externalFile));
            using var s = externalFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
            return LoadFrom(s);
        }

        /// <summary>
        /// Loads a dictionary from the specified directory based on common names.
        /// </summary>
        /// <remarks>
        /// This will attempt to load 'dictionary.xml, .xml.gz and .gz' from the specified directory.
        /// </remarks>
        public ExplicitXmlWordDictionary LoadFrom(DirectoryInfo searchDirectory)
        {
            if (searchDirectory == null) throw new ArgumentNullException(nameof(searchDirectory));

            var allLocationsToCheck = _DefaultFilenames.Select(f => Path.Combine(searchDirectory.FullName, f));
            foreach (var fileAndPath in allLocationsToCheck)
            {
                if (TryLoadDictionaryFromPath(fileAndPath, out var result))
                    return result!;
            }

            throw new UnableToLoadDictionaryException("Unable to load default dictionary. Tried the following locations: " + String.Join(", ", allLocationsToCheck));
        }

        // System.Net.WebRequest not available in Standard 1.3
        // Use System.Net.Http to make your http request and then pass the Stream / XmlReader to a LoadFrom() overload.
        // Not referencing System.Net.Http as it has a long list of dependencies.
#if !NETSTANDARD    
        /// <summary>
        /// Loads a dictionary from the specified url.
        /// </summary>
        /// <remarks>
        /// The file can be plaintext or gzipped. But you must specify which using is <c>isCompressed</c> parameter.
        /// 
        /// For information about the dictionary schema definition see the default xml file or github.
        /// </remarks>
        public ExplicitXmlWordDictionary LoadFrom(Uri networkLocation, bool isCompressed)
        {
            return LoadFrom(networkLocation, isCompressed, TimeSpan.Zero);
        }
        /// <summary>
        /// Loads a dictionary from the specified url.
        /// </summary>
        /// <remarks>
        /// The file can be plaintext or gzipped. But you must specify which using is <c>isCompressed</c> parameter.
        /// 
        /// For information about the dictionary schema definition see the default xml file or github.
        /// </remarks>
        public ExplicitXmlWordDictionary LoadFrom(Uri networkLocation, bool isCompressed, TimeSpan timeout)
        {
            _ = networkLocation ?? throw new ArgumentNullException(nameof(networkLocation));

            var request = System.Net.WebRequest.Create(networkLocation);
            if (timeout > TimeSpan.Zero)
                request.Timeout = (int)timeout.TotalMilliseconds;
            using var response = request.GetResponse();
            return LoadFrom(response.GetResponseStream(), isCompressed);
        }
#endif

        /// <summary>
        /// Loads a dictionary from the specified stream.
        /// </summary>
        /// <remarks>
        /// The file can be plaintext or gzipped.
        /// The stream must have <c>CanSeek</c> = true.
        /// 
        /// For information about the dictionary schema definition see the default xml file or github.
        /// </remarks>
        public ExplicitXmlWordDictionary LoadFrom(Stream s)
        {
            if (s == null)
                throw new ArgumentNullException("s");
            if (!s.CanSeek)
                throw new ArgumentException("Cannot read dictionary from stream which does not support seeking. Use the LoadDictionary(Stream, bool) overload to manually specify compression.", "s");

            // Check to see if the file is compressed or plain text.
            var buf = new byte[2];
            s.Read(buf, 0, buf.Length);
            s.Position = 0;

            Stream uncompressedStream = s;
            if (buf[0] == 0x1f && buf[1] == 0x8b)
                // Found Gzip magic number, decompress before loading.
                uncompressedStream = new System.IO.Compression.GZipStream(s, System.IO.Compression.CompressionMode.Decompress);
            return this.LoadFrom(uncompressedStream, false);
        }

        /// <summary>
        /// Loads a dictionary from the specified stream.
        /// </summary>
        /// <remarks>
        /// The file can be plaintext or gzipped based on the <c>isCompressed</c> parameter.
        /// The stream does NOT require <c>CanSeek</c> = true.
        /// Use this overload for streams which do not support seeking but are compressed (eg: network stream).
        /// 
        /// For information about the dictionary schema definition see the default xml file or github.
        /// </remarks>
        public ExplicitXmlWordDictionary LoadFrom(Stream s, bool isCompressed)
        {
            if (s == null)
                throw new ArgumentNullException("s");

            Stream uncompressedStream = s;
            if (isCompressed)
                uncompressedStream = new System.IO.Compression.GZipStream(s, System.IO.Compression.CompressionMode.Decompress);

            try
            {
                // The size of the XML can be used to estimate the number of words in the dictionary; 
                _StreamSize = uncompressedStream.Length;
            }
            catch (NotSupportedException) { }

            return this.LoadFrom(XmlReader.Create(uncompressedStream));            
        }
        public ExplicitXmlWordDictionary LoadFrom(XmlReader reader)
        {
            _ = reader ?? throw new ArgumentNullException(nameof(reader));

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                    this.ParseElement(reader);
            }
            _Dict!.InitWordsByTypeLookup();
            return _Dict;
        }

        private void ParseElement(XmlReader reader)
        {
            var node = reader.Name.ToLower();
            if (!_NodeLookup.TryGetValue(node, out var action))
                throw new DictionaryParseException(String.Format("Unknown element named '{0}' found in dictionary.", node));
            action(reader);
        }
        private void ParseDictionaryRoot(XmlReader reader)
        {
            if (!Int32.TryParse(reader.GetAttribute("schemaVersion"), out var version) || version > 6)
                throw new DictionaryParseException(String.Format("Unknown schemaVersion '{0}'.", reader.GetAttribute("schemaVersion")));

            // Root element must be parsed first.
            _Dict = new ExplicitXmlWordDictionary(reader.GetAttribute("name"), reader.GetAttribute("language"), version);
            _Dict.ExpandCapacityTo((int)(_StreamSize / 100L));         // Based on the 0.10.0 version, there are about 100 bytes per word.
        }
        private void ParseArticle(XmlReader reader)
        {
            // This and all subsequent accesses of _Dict should not be null, as ParseDictionaryRoot() sets it.
            _Dict!.Add(new MaterialisedArticle(reader.GetAttribute("definite"), reader.GetAttribute("indefinite"), reader.GetAttribute("indefiniteBeforeVowel"), ReadTags(reader)));
        }
        private void ParseDemonstrative(XmlReader reader)
        {
            _Dict!.Add(new MaterialisedDemonstrative(reader.GetAttribute("singular"), reader.GetAttribute("plural"), ReadTags(reader)));
        }
        private void ParseAdverb(XmlReader reader)
        {
            _Dict!.Add(new MaterialisedAdverb(reader.GetAttribute("value"), ReadTags(reader)));
        }
        private void ParsePersonalPronoun(XmlReader reader)
        {
            _Dict!.Add(new MaterialisedPersonalPronoun(reader.GetAttribute("singular"), reader.GetAttribute("plural"), ReadTags(reader)));
        }
        private void ParseIndefinitePronoun(XmlReader reader)
        {
            _Dict!.Add(new MaterialisedIndefinitePronoun(reader.GetAttribute("singular"), reader.GetAttribute("plural"), Boolean.Parse(reader.GetAttribute("personal")), ReadTags(reader)));
        }
        private void ParseNoun(XmlReader reader)
        {
            _Dict!.Add(new MaterialisedNoun(reader.GetAttribute("singular"), reader.GetAttribute("plural"), ReadTags(reader)));
        }
        private void ParseProperNoun(XmlReader reader)
        {
            _Dict!.Add(new MaterialisedProperNoun(reader.GetAttribute("value"), ReadTags(reader)));
        }
        private void ParsePreposition(XmlReader reader)
        {
            _Dict!.Add(new MaterialisedPreposition(reader.GetAttribute("value"), ReadTags(reader)));
        }
        private void ParseAdjective(XmlReader reader)
        {
            _Dict!.Add(new MaterialisedAdjective(reader.GetAttribute("value"), ReadTags(reader)));
        }
        private void ParseSpeechVerb(XmlReader reader)
        {
            _Dict!.Add(new MaterialisedSpeechVerb(reader.GetAttribute("past"), ReadTags(reader)));
        }
        private void ParseConjunction(XmlReader reader)
        {
            var separates = reader.GetAttribute("separates");
            var separatesNouns = separates.IndexOf("noun", StringComparison.OrdinalIgnoreCase) != -1;
            var separatesPhrases = separates.IndexOf("phrase", StringComparison.OrdinalIgnoreCase) != -1;
            _Dict!.Add(new MaterialisedConjunction(reader.GetAttribute("value"), separatesNouns, separatesPhrases, ReadTags(reader)));
        }
        private void ParseVerb(XmlReader reader)
        {

            _Dict!.Add(new MaterialisedVerb(
                        reader.GetAttribute("presentSingular"), 
                        reader.GetAttribute("pastSingular"), 
                        reader.GetAttribute("pastContinuousSingular"), 
                        reader.GetAttribute("futureSingular"), 
                        reader.GetAttribute("continuousSingular"), 
                        reader.GetAttribute("perfectSingular"), 
                        reader.GetAttribute("subjunctiveSingular"),

                        reader.GetAttribute("presentPlural"),
                        reader.GetAttribute("pastPlural"), 
                        reader.GetAttribute("pastContinuousPlural"),
                        reader.GetAttribute("presentSingular"), 
                        reader.GetAttribute("continuousPlural"),
                        reader.GetAttribute("futurePlural"), 
                        reader.GetAttribute("subjunctivePlural"),

                        // The 'transitive' attribute is new and optional, verbs are assumed to be transitive by default (as most are).
                        String.IsNullOrEmpty(reader.GetAttribute("transitive")) ? true : Boolean.Parse(reader.GetAttribute("transitive")),
                        
                        ReadTags(reader)
                        )
                     );
        }
        private void ParseInterrogative(XmlReader reader)
        {
            _Dict!.Add(new MaterialisedInterrogative(reader.GetAttribute("singular"), reader.GetAttribute("plural"), ReadTags(reader)));
        }
        private void ParseNumberRange(XmlReader reader)
        {
            var start = Int32.Parse(reader.GetAttribute("start"));
            var end = Int32.Parse(reader.GetAttribute("end"));
            for (int i = start; i <= end; i++)
                _Dict!.Add(new MaterialisedNumber(i, ReadTags(reader)));
        }

        private IReadOnlyList<string> ReadTags(XmlReader reader)
        {
            var tagsCsv = reader.GetAttribute("tags") ?? "";
            if (tagsCsv == "")
                return EmptyTags;
            return tagsCsv.Split(Word.CommaArray, StringSplitOptions.RemoveEmptyEntries);
        }

#region Dispose
        private bool _IsDisposed = false;
        public void Dispose()
        {
            if (!_IsDisposed)
            {
                this._Dict = null;
                _IsDisposed = true;
            }
            GC.SuppressFinalize(this);
        }
#endregion
    }
}
