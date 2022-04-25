using MurrayGrant.ReadablePassphrase.Dictionaries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MurrayGrant.WordScraper
{
    internal class DictionaryComScraper
    {
        public CommandLineArguments Args { get; }
        public HttpClient HttpClient { get; }
        public CancellationToken CancellationToken { get; }
        public Action<int, int> ReportProgress { get; }

        public DictionaryComScraper(CommandLineArguments args, HttpClient httpClient, CancellationToken cancellationToken, Action<int, int> reportProgress)
        {
            this.Args = args;
            this.HttpClient = httpClient;
            this.CancellationToken = cancellationToken;
            this.ReportProgress = reportProgress;
        }

        internal Task<IReadOnlyList<(string wordRoot, string partOfSpeech)>> ReadWords(WordDictionary dictionary, IReadOnlySet<string> uniqueRoots, IReadOnlySet<string> uniqueForms)
        {
            var result = new List<(string, string)>();
            return Task.FromResult((IReadOnlyList<(string, string)>)result);
        }
    }
}
