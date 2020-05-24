using System.Collections.Generic;
using static System.String;

namespace MurrayGrant.ReadablePassphrase.TestHelpers
{
    public class TestPassphrase
    {
        public TestPassphrase(IReadOnlyList<string> words)
        {
            Words = words;
            Phrase = Join(' ', words);
        }

        public IReadOnlyList<string> Words { get; }
        public string Phrase { get; }
    }
}
