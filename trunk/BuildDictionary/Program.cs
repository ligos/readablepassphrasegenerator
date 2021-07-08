using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;

using CommandLine;


[assembly: InternalsVisibleToAttribute("UnitTests")]


namespace BuildDictionary
{
    public class Program
    {
        private const string output = ".\\dictionary.xml";


        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options.ExtractOptions, Options.BuildOptions>(args)
                .MapResult(
                    (Options.ExtractOptions opts) => 
                    {
                        var wordFiles = GetEmbeddedWordFiles();
                        foreach (var f in wordFiles)
                        {
                            Console.WriteLine($"Extracting file '{f.Key}'.");
                            System.IO.File.WriteAllText(f.Key, f.Value);
                        }
                        Console.WriteLine("All word files extracted.");
                        //we're done
                        return 0;
                    },
                    (Options.BuildOptions opts) =>
                    {
                        BuildDictionary();
                        return 0;
                    },
                    //any error returns 1 for process return value
                    errs => 1);
        }


        /// <summary>
        /// Actually build a dictionary
        /// </summary>
        internal static void BuildDictionary()
        {
            var files = GetAllDictionaryFiles();

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
                            if (first && !line.StartsWith("</dictionary>", StringComparison.CurrentCultureIgnoreCase))
                            {
                                dictionaryStream.WriteLine(line);
                            }
                            else if (!first && !inDictionaryElement && line.StartsWith("<dictionary", StringComparison.CurrentCultureIgnoreCase))
                            {
                                inDictionaryElement = true;
                            }
                            else if (!first && inDictionaryElement && !line.StartsWith("</dictionary>", StringComparison.CurrentCultureIgnoreCase))
                            {
                                dictionaryStream.WriteLine(line);
                            }
                        }
                    }

                    first = false;
                }
                dictionaryStream.WriteLine();
                dictionaryStream.WriteLine("</dictionary>");
            }
        }


        internal static List<string> GetAllDictionaryFiles()
        {
            //get all XML files in the current dir
            var files = Directory.GetFiles(".\\", "*.xml").ToList();
            //filter out the output dictionary
            files = files.Where(f => !(f.Equals(output, StringComparison.CurrentCultureIgnoreCase))).ToList();
            //reorder to put BASE at the first of list
            files = files.OrderBy(f => !f.Equals(".\\base.xml", StringComparison.CurrentCultureIgnoreCase)).ToList();

            return files;
        }


        /// <summary>
        /// retrieve all embedded word resource files
        /// </summary>
        /// <returns></returns>
        internal static Dictionary<string, string> GetEmbeddedWordFiles()
        {
            var r = new Dictionary<string, string>();

            var assembly = Assembly.GetExecutingAssembly();
            var resources = assembly.GetManifestResourceNames();

            foreach (var resourceName in resources)
            {
                if (resourceName.EndsWith(".xml"))
                {
                    using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        var name = resourceName.Replace("BuildDictionary.resources.", "");
                        r.Add(name, reader.ReadToEnd());
                    }
                }
            }
            return r;
        }
    }


    internal class Options
    {
        // extract dictionary files to the current dir
        [Verb("extract", HelpText = "Extracts all built in dictionary files to the current folder.")]
        internal class ExtractOptions
        {
        }

        [Verb("build", HelpText = "Build dictionary.xml from component XML files.")]
        internal class BuildOptions
        {
        }
    }
}
