using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace WordScraper
{
    public class Program
    {
        public const int wordCount = 10000;
        public const int wordMinLength = 2;
        public const int wordMaxLength = 11;

        static void Main(string[] args)
        {
            Console.WriteLine("WordScraper Utility for ReadablePhrase Generator");
            Console.WriteLine("Pulls AI generated words from thisworddoesnotexist.com and conjugates them.");
            Console.WriteLine("drventure 2020");
            Console.WriteLine("");
            Console.WriteLine($"Generating {wordCount} words...");

            Dictionary<string, string> words;
            words = GetRawWords();

            //words = ReadRawWords();

            var pluralNouns = words.Where(w => string.Compare(w.Value, "plural noun") == 0).ToList();
            var pluralNounsXml = new XmlDocument();
            var dict = pluralNounsXml.CreateElement("dictionary");
            pluralNounsXml.AppendChild(dict);
            foreach (var w in pluralNouns)
            {
                var wordNode = pluralNounsXml.CreateElement("noun");
                dict.AppendChild(wordNode);
                wordNode.SetAttribute("plural", w.Key);
            }
            pluralNounsXml.Save(".\\GeneratedPluralNouns.xml");


            var nouns = words.Where(w => string.Compare(w.Value, "noun") == 0).ToList();
            var nounsXml = new XmlDocument();
            dict = nounsXml.CreateElement("dictionary");
            nounsXml.AppendChild(dict);
            foreach (var w in nouns)
            {
                var wordNode = nounsXml.CreateElement("noun");
                dict.AppendChild(wordNode);
                wordNode.SetAttribute("singular", w.Key);
                if (w.Key.EndsWith("man"))
                    wordNode.SetAttribute("plural", w.Key.Substring(0, w.Key.Length - 3) + "men");
                if (w.Key.EndsWith("ch"))
                    wordNode.SetAttribute("plural", w.Key.Substring(0, w.Key.Length - 2) + "ches");
                else if (w.Key.EndsWith("sh"))
                    wordNode.SetAttribute("plural", w.Key);
                else if (w.Key.EndsWith("y"))
                    wordNode.SetAttribute("plural", w.Key.Substring(0, w.Key.Length - 1) + "ies");
                else if (w.Key.EndsWith("u"))
                    wordNode.SetAttribute("plural", w.Key.Substring(0, w.Key.Length - 1) + "s");
                else if (w.Key.EndsWith("o") || w.Key.EndsWith("x"))
                    wordNode.SetAttribute("plural", w.Key + "es");
                else if (w.Key.EndsWith("a") || w.Key.EndsWith("e") || w.Key.EndsWith("i"))
                    wordNode.SetAttribute("plural", w.Key + "s");
                else if (!w.Key.EndsWith("s"))
                    wordNode.SetAttribute("plural", w.Key + "s");
            }
            nounsXml.Save(".\\GeneratedNouns.xml");


            var adjectives = words.Where(w => string.Compare(w.Value, "adjective") == 0).ToList();
            var adjectivesXml = new XmlDocument();
            dict = adjectivesXml.CreateElement("dictionary");
            adjectivesXml.AppendChild(dict);
            foreach (var w in adjectives)
            {
                var wordNode = adjectivesXml.CreateElement("adjective");
                dict.AppendChild(wordNode);
                wordNode.SetAttribute("value", w.Key);
            }
            adjectivesXml.Save(".\\GenerateAdjectives.xml");


            var adverbs = words.Where(w => string.Compare(w.Value, "adverb") == 0).ToList();
            var adverbsXml = new XmlDocument();
            dict = adverbsXml.CreateElement("dictionary");
            adverbsXml.AppendChild(dict);
            foreach (var w in adverbs)
            {
                var wordNode = adverbsXml.CreateElement("adverb");
                dict.AppendChild(wordNode);
                wordNode.SetAttribute("value", w.Key);
            }
            adverbsXml.Save(".\\GeneratedAdverbs.xml");


            var verbs = words.Where(w => string.Compare(w.Value, "verb") == 0).ToList();
            var verbsXml = new XmlDocument();
            dict = verbsXml.CreateElement("dictionary");
            verbsXml.AppendChild(dict);
            foreach (var w in verbs)
            {
                var wordNode = verbsXml.CreateElement("verb");
                dict.AppendChild(wordNode);

                var toES = w.Key.EndsWith("s") ? $"{w.Key}es" : $"{w.Key}s";
                toES = w.Key.EndsWith("x") ? $"{w.Key}es" : toES;
                toES = w.Key.EndsWith("e") ? $"{w.Key}s" : toES;
                toES = w.Key.EndsWith("y") ? $"{w.Key.Substring(0, w.Key.Length - 1)}ies" : toES;
                toES = w.Key.EndsWith("ch") || w.Key.EndsWith("sh") ? $"{w.Key}es" : toES;

                var toED = w.Key.EndsWith("e") ? $"{w.Key}d" : $"{w.Key}ed";
                toED = w.Key.EndsWith("y") ? $"{w.Key.Substring(0, w.Key.Length - 1)}ied" : toED;

                var toING = w.Key.EndsWith("e") ? $"{w.Key.Substring(0, w.Key.Length - 1)}ing" : $"{w.Key}ing";

                wordNode.SetAttribute("presentSingular", $"{toES}");            
                wordNode.SetAttribute("pastSingular", $"{toED}");
                wordNode.SetAttribute("pastContinuousSingular", $"was {toING}");
                wordNode.SetAttribute("futureSingular", $"will {w.Key}");
                wordNode.SetAttribute("continuousSingular", $"is {toING}");
                wordNode.SetAttribute("perfectSingular", $"has {toED}");
                wordNode.SetAttribute("subjunctiveSingular", $"might {w.Key}");
                wordNode.SetAttribute("presentPlural", w.Key);
                wordNode.SetAttribute("pastPlural", $"{toED}");
                wordNode.SetAttribute("pastContinuousPlural", $"were {toING}");
                wordNode.SetAttribute("futurePlural", $"will {w.Key}");
                wordNode.SetAttribute("continuousPlural", $"are {toING}");
                wordNode.SetAttribute("perfectPlural", $"have {toED}");
                wordNode.SetAttribute("subjunctivePlural", $"might {w.Key}");
            }
            verbsXml.Save(".\\GeneratedVerbs.xml");
        }


        internal static Dictionary<string, string> ReadRawWords()
        {
            var d = new Dictionary<string, string>();
            var buf = System.IO.File.ReadAllText(".\\GeneratedWords.txt");

            var words = buf.Replace("\r\n", "\r").Split('\r');
            foreach (var w in words)
            {
                var b2 = w.Replace("---", "\r").Split('\r');
                //drop dupes
                if (b2.GetUpperBound(0) == 1 && !d.ContainsKey(b2[1]))
                {
                    d.Add(b2[1], b2[0]);
                }
            }
            return d;
        }


        private static Dictionary<string, string> GetRawWords()
        {
            var words = new Dictionary<string, string>();

            var b = string.Empty;
            var c = 0;
            do
            {
                var task = ReadRawWordPage();
                task.Wait();
                var buf = task.Result;

                var word = string.Empty;
                var pos = string.Empty;

                Parse(buf, ref word, ref pos);

                if (!string.IsNullOrWhiteSpace(pos) && !string.IsNullOrWhiteSpace(word))
                {
                    var v = $"{c}:   {word}: {pos}";
                    Console.WriteLine(v);

                    //drop dupes and words that are too long
                    if (!words.ContainsKey(word) && word.Length <= wordMaxLength && word.Length > wordMinLength)
                    {
                        words.Add(word, pos);
                        b += $"{pos}---{word}\r\n";
                        c++;
                    }
                }
            } while (c <= wordCount);

            //write to a log file
            System.IO.File.WriteAllText(".\\GeneratedWords.txt", b);

            return words;
        }


        private static void Parse(string buf, ref string word, ref string pos)
        {
            var mark = "class=\"pos\">";
            var posPos = buf.IndexOf(mark, 0, StringComparison.CurrentCultureIgnoreCase);
            if (posPos > 0)
            {
                var endMark = "</div>";
                var posEnd = buf.IndexOf(endMark, posPos);
                if (posEnd > 0)
                {
                    pos = buf.Substring(posPos + mark.Length, posEnd - posPos - mark.Length);
                    pos = pos.Replace(".", "");
                    posEnd = pos.IndexOf("[");
                    if (posEnd > 0) pos = pos.Substring(0, posEnd - 1);
                    pos = pos.Trim();
                }
            }

            mark = "class=\"word\">";
            var posWord = buf.IndexOf(mark);
            if (posWord > 0)
            {
                var endMark = "</div>";
                var posEnd = buf.IndexOf(endMark, posWord);
                if (posEnd > 0)
                {
                    word = buf.Substring(posWord + mark.Length, posEnd - posWord - mark.Length);
                    word = word.Replace(".", "");
                    word = word.Trim();
                }
            }

            if (Regex.IsMatch(word, "[^a-z&^ ]+"))
            {
                //if any non-alpha char detected, just through this one out
                pos = string.Empty;
                word = string.Empty;
            }
        }


        private static async Task<string> ReadRawWordPage()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "C# console program");

                return await client.GetStringAsync("https://www.thisworddoesnotexist.com");
            }
        }
    }
}
