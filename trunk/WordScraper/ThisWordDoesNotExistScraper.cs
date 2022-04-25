using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MurrayGrant.WordScraper
{
    internal class ThisWordDoesNotExistScraper
    {
        public CommandLineArguments Args { get; }
        public HttpClient HttpClient { get; }
        public CancellationToken CancellationToken { get; }
        public Action<int, int> ReportProgress { get; }
        public Action<string> ReportMessage { get; }

        public ThisWordDoesNotExistScraper(CommandLineArguments args, HttpClient httpClient, CancellationToken cancellationToken, Action<string> reportMessage, Action<int, int> reportProgress)
        {
            this.Args = args;
            this.HttpClient = httpClient;
            this.CancellationToken = cancellationToken;
            this.ReportMessage = reportMessage;
            this.ReportProgress = reportProgress;
        }

        public async Task<IReadOnlyList<(string wordRoot, PartOfSpeech pos)>> ReadWords(IReadOnlySet<string> uniqueForms)
        {
            var result = new List<(string, PartOfSpeech)>(Args.WordCount);
            var attemptCounter = 0;
            var uniqueFormsFromThisRun = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);

            while (result.Count < Args.WordCount && attemptCounter < Args.Attempts)
            {
                CancellationToken.ThrowIfCancellationRequested();

                // Download webpage.
                var partOfSpeech = "";
                var wordRoot = "";
                var page = await HttpClient.GetStringAsync("https://www.thisworddoesnotexist.com");
                ++attemptCounter;

                // Parse
                var mark = "class=\"pos\">";
                var posPos = page.IndexOf(mark, 0, StringComparison.CurrentCultureIgnoreCase);
                if (posPos > 0)
                {
                    var endMark = "</div>";
                    var posEnd = page.IndexOf(endMark, posPos);
                    if (posEnd > 0)
                    {
                        partOfSpeech = page.Substring(posPos + mark.Length, posEnd - posPos - mark.Length);
                        partOfSpeech = partOfSpeech.Replace(".", "");
                        posEnd = partOfSpeech.IndexOf("[");
                        if (posEnd > 0)
                            partOfSpeech = partOfSpeech.Substring(0, posEnd - 1);
                        partOfSpeech = partOfSpeech.Trim();
                    }
                }

                mark = "class=\"word\">";
                var posWord = page.IndexOf(mark);
                if (posWord > 0)
                {
                    var endMark = "</div>";
                    var posEnd = page.IndexOf(endMark, posWord);
                    if (posEnd > 0)
                    {
                        wordRoot = page.Substring(posWord + mark.Length, posEnd - posWord - mark.Length);
                        wordRoot = wordRoot.Replace(".", "");
                        wordRoot = wordRoot.Trim();
                    }
                }

                // Various reasons to exclude this word:
                if (string.IsNullOrWhiteSpace(wordRoot) || string.IsNullOrWhiteSpace(partOfSpeech))
                    goto ReportProgressAndNext;

                var pos = ParsePartOfSpeech(partOfSpeech);
                if (pos == PartOfSpeech.Unknown)
                {
                    ReportMessage($"Warning: Unknown part of speech: '{partOfSpeech}' for '{wordRoot}'.");
                    goto ReportProgressAndNext;
                }

                if (!pos.IsSupported())
                    goto ReportProgressAndNext;

                if (uniqueForms.Contains(wordRoot))
                    goto ReportProgressAndNext;

                if (wordRoot.Length < Args.MinLength || wordRoot.Length > Args.MaxLength)
                    goto ReportProgressAndNext;

                if (!uniqueFormsFromThisRun.Add(wordRoot))
                    goto ReportProgressAndNext;

                // This one is OK!
                result.Add((wordRoot, pos));

ReportProgressAndNext:
                ReportProgress(result.Count, attemptCounter);
                if (Args.DelayMs > 0)
                    await Task.Delay(Args.DelayMs);
            }

            return result;
        }

        PartOfSpeech ParsePartOfSpeech(string partOfSpeech)
            => string.Equals(partOfSpeech, "noun",         StringComparison.OrdinalIgnoreCase) ? PartOfSpeech.Noun
            :  string.Equals(partOfSpeech, "plural noun",  StringComparison.OrdinalIgnoreCase) ? PartOfSpeech.NounPlural
            :  string.Equals(partOfSpeech, "verb",         StringComparison.OrdinalIgnoreCase) ? PartOfSpeech.VerbTransitive
            :  string.Equals(partOfSpeech, "adjective",    StringComparison.OrdinalIgnoreCase) ? PartOfSpeech.Adjective
            :  string.Equals(partOfSpeech, "adverb",       StringComparison.OrdinalIgnoreCase) ? PartOfSpeech.Adverb
            :  string.Equals(partOfSpeech, "interjection", StringComparison.OrdinalIgnoreCase) ? PartOfSpeech.Interjection
            :  PartOfSpeech.Unknown;
    }
}
