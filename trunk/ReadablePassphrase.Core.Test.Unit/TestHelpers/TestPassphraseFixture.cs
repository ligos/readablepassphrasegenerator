using System;
using NUnit.Framework;

namespace MurrayGrant.ReadablePassphrase.TestHelpers
{
    [TestFixture]
    public class TestPassphraseFixture
    {
        [Test]
        public void DoesNotGenerateWithTrailingSpace()
        {
            //  ARRANGE --------------------------------------------------------
            var words = WordList.Pick(3);
            var sut = new TestPassphrase(words);

            //  ACT ------------------------------------------------------------
            var output = sut.Phrase;

            //  ASSERT ---------------------------------------------------------
            Assert.That(output, Is.EqualTo(output.Trim()));
        }

        [Test]
        public void GeneratesCorrectNumberOfSpaceDelimitedWords([Values(3, 4)] int wordCount)
        {
            //  ARRANGE --------------------------------------------------------
            var words = WordList.Pick(wordCount);
            var sut = new TestPassphrase(words);

            //  ACT ------------------------------------------------------------
            var output = sut.Phrase;
            var actual = output.Split(new[] { ' ' });

            //  ASSERT ---------------------------------------------------------
            Assert.That(actual, Has.Length.EqualTo(wordCount));
        }

        [Test]
        public void GeneratesPhraseFromJoiningWords([Values(3, 4)] int wordCount)
        {
            //  ARRANGE --------------------------------------------------------
            var words = WordList.Pick(wordCount);
            var sut = new TestPassphrase(words);

            //  ACT ------------------------------------------------------------
            var actual = sut.Phrase;

            //  ASSERT ---------------------------------------------------------
            Assert.That(actual, Is.EqualTo(String.Join(' ', sut.Words)));
        }
    }
}
