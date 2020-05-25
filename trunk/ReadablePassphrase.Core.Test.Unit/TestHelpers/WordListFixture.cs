using System.Linq;
using NUnit.Framework;

namespace MurrayGrant.ReadablePassphrase.TestHelpers
{
    [TestFixture]
    public class WordListFixture
    {
        [Test]
        public void WordFileIsReadCorrectly()
        {
            //  ARRANGE --------------------------------------------------------

            //  ACT ------------------------------------------------------------
            var actual = WordList.Words.Value.FirstOrDefault(x => x == "outré");

            //  ASSERT ---------------------------------------------------------
            Assert.That(actual, Is.EqualTo("outré"));
        }
    }
}
