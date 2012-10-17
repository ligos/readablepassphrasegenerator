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
        private ExplicitXmlWordDictionary _Dict;
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
                new KeyValuePair<string, Action<XmlReader>>("preposition", ParsePreposition),
                new KeyValuePair<string, Action<XmlReader>>("adjective", ParseAdjective),
                new KeyValuePair<string, Action<XmlReader>>("adverb", ParseAdverb),
                new KeyValuePair<string, Action<XmlReader>>("verb", ParseVerb),
                new KeyValuePair<string, Action<XmlReader>>("interrogative", ParseInterrogative),
            }.ToDictionary(x => x.Key, x => x.Value);
        }

        public WordDictionary Load(IDictionary<string, string> arguments)
        {
            var args = new Dictionary<string, string>();        // Assume empty arguments on null.
            if (arguments != null)
                args = new Dictionary<string, string>(arguments
                                                        .Select(x => new KeyValuePair<string, string>(x.Key.Trim(), x.Value.Trim()))
                                                        .ToDictionary(x => x.Key, x => x.Value)
                                                    , StringComparer.CurrentCultureIgnoreCase);

            // See what's been set in the arguments.
            FileInfo fileLocation = null;
            Uri urlLocation = null;
            bool isCompressedTemp;
            bool? isCompressed = null;
            bool useDefaultDictionaryTemp;
            bool? useDefaultDictionary = null;
            if (args.ContainsKey("file") && !String.IsNullOrEmpty(args["file"]))
                fileLocation = new FileInfo(args["file"]);
            else if (args.ContainsKey("url") && !String.IsNullOrEmpty(args["url"]))
                urlLocation = new Uri(args["url"]);
            if (args.ContainsKey("isCompressed") && Boolean.TryParse(args["isCompressed"], out isCompressedTemp)) 
                isCompressed = isCompressedTemp;
            if (args.ContainsKey("useDefaultDictionary") && Boolean.TryParse(args["useDefaultDictionary"], out useDefaultDictionaryTemp))
                useDefaultDictionary = useDefaultDictionaryTemp;
    
            // Based on what was passed in, call an appropriate LoadFrom() overload.
            if (fileLocation != null)
                return this.LoadFrom(fileLocation);
            else if (urlLocation != null && !isCompressed.HasValue)
                throw new InvalidDictionaryLoaderArgumentException("If you are loading from a url, you must specify 'isCompressed=true|false'.");
            else if (urlLocation != null)
                return this.LoadFrom(urlLocation, isCompressed.Value);
            else if (useDefaultDictionary == true)
                return this.LoadFrom();
            else
                throw new InvalidDictionaryLoaderArgumentException("Neither 'url' or 'file' parameters were specified. If you with to look for and load a defult dictionary.xml file in the current directory, specify 'useDefaultDictionary=true'.");
        }

        /// <summary>
        /// Loads the default dictionary.
        /// </summary>
        /// <remarks>
        /// This will attempt to load 'dictionary.xml, .xml.gz and .gz' from
        /// the folder of the exe (<c>Assembly.GetEntryAssembly()</c>) or the current directory (<c>Environment.CurrentDirectory</c>).
        /// 
        /// For information about the dictionary schema definition see the default xml file or codeplex website.
        /// </remarks>
        public ExplicitXmlWordDictionary LoadFrom()
        {
            // Check dictionary.xml, dictionary.xml.gz, dictionary.gz in entrypoint and current working directory.
            var filenames = new string[] { "dictionary.xml", "dictionary.xml.gz", "dictionary.gz" };
            var allLocationsToCheck = filenames.Select(f => Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), f))
                .Concat(filenames.Select(f => Path.Combine(Environment.CurrentDirectory, f)))
                .ToArray();

            foreach (var fileAndPath in allLocationsToCheck)
            {
                ExplicitXmlWordDictionary result = null;
                if (TryLoadDictionaryFromPath(fileAndPath, out result))
                    return result;
            }

            throw new UnableToLoadDictionaryException("Unable to load default dictionary. Tried the following locations: " + String.Join(", ", allLocationsToCheck));
        }
        private bool TryLoadDictionaryFromPath(string fileAndPath, out ExplicitXmlWordDictionary dict)
        {
            if (!File.Exists(fileAndPath))
            {
                dict = null;
                return false;
            }
            try
            {
                using (var stream = new FileStream(fileAndPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    dict = this.LoadFrom(stream);
                    return true;
                }
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
        /// For information about the dictionary schema definition see the default xml file or codeplex website.
        /// </remarks>
        public ExplicitXmlWordDictionary LoadFrom(string pathToExternalFile)
        {
            if (!String.IsNullOrEmpty(pathToExternalFile))
                return this.LoadFrom(new FileInfo(pathToExternalFile));
            else
                return this.LoadFrom();
        }
        /// <summary>
        /// Loads a dictionary from the specified file.
        /// </summary>
        /// <remarks>
        /// The file can be plaintext or gzipped.
        /// 
        /// For information about the dictionary schema definition see the default xml file or codeplex website.
        /// </remarks>
        public ExplicitXmlWordDictionary LoadFrom(FileInfo externalFile)
        {
            using (var s = externalFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
                return LoadFrom(s);
        }
        
        
        /// <summary>
        /// Loads a dictionary from the specified url.
        /// </summary>
        /// <remarks>
        /// The file can be plaintext or gzipped. But you must specify which using is <c>isCompressed</c> parameter.
        /// 
        /// For information about the dictionary schema definition see the default xml file or codeplex website.
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
        /// For information about the dictionary schema definition see the default xml file or codeplex website.
        /// </remarks>
        public ExplicitXmlWordDictionary LoadFrom(Uri networkLocation, bool isCompressed, TimeSpan timeout)
        {
            var request = System.Net.WebRequest.Create(networkLocation);
            if (timeout > TimeSpan.Zero)
                request.Timeout = (int)timeout.TotalMilliseconds;
            using (var response = request.GetResponse())
                return LoadFrom(response.GetResponseStream(), isCompressed);
        }


        /// <summary>
        /// Loads a dictionary from the specified stream.
        /// </summary>
        /// <remarks>
        /// The file can be plaintext or gzipped.
        /// The stream must have <c>CanSeek</c> = true.
        /// 
        /// For information about the dictionary schema definition see the default xml file or codeplex website.
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
        /// For information about the dictionary schema definition see the default xml file or codeplex website.
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
            this._Dict = new ExplicitXmlWordDictionary();
            this._Dict.ExpandCapacityTo((int)(_StreamSize / 170L));         // Based on the 0.6.1 version, there are about 170 bytes per word.

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                    this.ParseElement(reader);
            }
            _Dict.InitWordsByTypeLookup();
            return _Dict;
        }

        private void ParseElement(XmlReader reader)
        {
            var node = reader.Name.ToLower();
            Action<XmlReader> action;
            if (!_NodeLookup.TryGetValue(node, out action))
                throw new DictionaryParseException(String.Format("Unknown element named '{0}' found in dictionary.", node));
            action(reader);
        }
        private void ParseDictionaryRoot(XmlReader reader)
        {
            int version;
            if (!Int32.TryParse(reader.GetAttribute("schemaVersion"), out version) || version > 1)
                throw new DictionaryParseException(String.Format("Unknown schemaVersion '{0}'.", reader.GetAttribute("schemaVersion")));

            _Dict.SetNameAndLanguageCode(reader.GetAttribute("name"), reader.GetAttribute("language"));
        }
        private void ParseArticle(XmlReader reader)
        {
            _Dict.Add(new MaterialisedArticle(reader.GetAttribute("definite"), reader.GetAttribute("indefinite"), reader.GetAttribute("indefiniteBeforeVowel")));
        }
        private void ParseDemonstrative(XmlReader reader)
        {
            _Dict.Add(new MaterialisedDemonstrative(reader.GetAttribute("singular"), reader.GetAttribute("plural")));
        }
        private void ParseAdverb(XmlReader reader)
        {
            _Dict.Add(new MaterialisedAdverb(reader.GetAttribute("value")));
        }
        private void ParsePersonalPronoun(XmlReader reader)
        {
            _Dict.Add(new MaterialisedPersonalPronoun(reader.GetAttribute("singular"), reader.GetAttribute("plural")));
        }
        private void ParseNoun(XmlReader reader)
        {
            _Dict.Add(new MaterialisedNoun(reader.GetAttribute("singular"), reader.GetAttribute("plural")));
        }
        private void ParsePreposition(XmlReader reader)
        {
            _Dict.Add(new MaterialisedPreposition(reader.GetAttribute("value")));
        }
        private void ParseAdjective(XmlReader reader)
        {
            _Dict.Add(new MaterialisedAdjective(reader.GetAttribute("value")));
        }
        private void ParseVerb(XmlReader reader)
        {

            _Dict.Add(new MaterialisedVerb(
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
                        reader.GetAttribute("subjunctivePlural")
                        )
                     );
        }
        private void ParseInterrogative(XmlReader reader)
        {
            _Dict.Add(new MaterialisedInterrogative(reader.GetAttribute("singular"), reader.GetAttribute("plural")));
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
