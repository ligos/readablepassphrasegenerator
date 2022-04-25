using MurrayGrant.ReadablePassphrase.Dictionaries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MurrayGrant.WordScraper
{
    internal class DictionaryComScraper
    {
        internal Task<IReadOnlyList<(string wordRoot, string partOfSpeech)>> ReadWords(WordDictionary dictionary, IReadOnlySet<string> uniqueRoots, IReadOnlySet<string> uniqueForms)
        {
            var result = new List<(string, string)>();
            return Task.FromResult((IReadOnlyList<(string, string)>)result);
        }
    }
}
