using AutoFixture;
using NUnit.Framework;

namespace MurrayGrant.ReadablePassphrase.TestHelpers
{
    [TestFixture]
    public class TestPassphraseBuilderFixture
    {
        [Test]
        public void DoesCustomiseForTestPassphraseType()
        {
            //  ARRANGE --------------------------------------------------------
            var fixture = new Fixture();
            fixture.Customizations.Add(new TestPassphraseBuilder(WordList.Words));

            //  ACT ------------------------------------------------------------
            var output = fixture.Create<TestPassphrase>().Phrase;

            //  ASSERT ---------------------------------------------------------
            Assert.That(output, Has.Length.GreaterThan(0), "expected some passphrase of some length");
        }
    }
}
