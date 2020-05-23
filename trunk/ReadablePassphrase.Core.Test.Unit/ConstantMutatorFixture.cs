using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoFixture;
using Moq;
using MurrayGrant.ReadablePassphrase.Mutators;
using MurrayGrant.ReadablePassphrase.Random;
using MurrayGrant.ReadablePassphrase.TestHelpers;
using NUnit.Framework;
using static System.String;

namespace MurrayGrant.ReadablePassphrase
{
    [TestFixture]
    public class ConstantMutatorFixture
    {
        [TestFixture]
        public class MutateFixture
        {
            [Test]
            public void Mutate_ForStartOfPhrase_AddsMutation_AtStart([Values("", " ")] string endWhiteSpace)
            {
                var fixture = new Fixture();
                fixture.Customizations.Add(new TestPassphraseBuilder(WordList.Words));
                //  ARRANGE --------------------------------------------------------
                var mutChar = ".";
                //  ReadablePasswordGenerator delimits non-mutated words with spaces
                var inputSource = fixture.Create<TestPassphrase>();
                var input = inputSource.Phrase;
                // An previous mutator may/not have written something at the end with no following whitespace
                var sb = new StringBuilder(input + endWhiteSpace);
                //  The constant mutator is placed at the start with its own trailing whitespace
                var expected = $"{mutChar} {input}{endWhiteSpace}";

                var sut = new ConstantMutator
                {
                    ValueToAdd = mutChar,
                    When = ConstantStyles.StartOfPhrase,
                    Whitespace = ' '
                };

                //  ACT ------------------------------------------------------------
                sut.Mutate(sb, new CryptoRandomSource());
                var actual = sb.ToString();

                //  ASSERT ---------------------------------------------------------
                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Mutate_ForAnywhere_AddsMutation_AtStart([Values("", " ")] string endWhiteSpace)
            {
                var fixture = new Fixture();
                fixture.Customizations.Add(new TestPassphraseBuilder(WordList.Words));
                //  ARRANGE --------------------------------------------------------
                var mutChar = ".";
                //  ReadablePasswordGenerator delimits non-mutated words with spaces
                var inputSource = fixture.Create<TestPassphrase>();
                var input = inputSource.Phrase;
                // An previous mutator may/not have written something at the end with no following whitespace
                var sb = new StringBuilder(input + endWhiteSpace);
                //  The constant mutator is placed at the start with its own trailing whitespace
                var expected = $"{mutChar} {input}{endWhiteSpace}";

                var rnd = new Mock<IRandomSourceBase>();
                //  Fix the random gen to always return the end index
                rnd.Setup(x => x.Next(It.IsAny<int>()))
                    .Returns<int>(x => 0);

                var sut = new ConstantMutator
                {
                    ValueToAdd = mutChar,
                    When = ConstantStyles.Anywhere,
                    Whitespace = ' '
                };

                //  ACT ------------------------------------------------------------
                sut.Mutate(sb, rnd.Object);
                var actual = sb.ToString();

                //  ASSERT ---------------------------------------------------------
                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Mutate_ForMiddleOfPhrase_AddsMutation_BetweenWords(
                [Values("", " ")] string endWhiteSpace, [Values(0, 1, 2)] int randIndex)
            {
                var fixture = new Fixture();
                fixture.Customizations.Add(new TestPassphraseBuilder(WordList.Words, 4));
                //  ARRANGE --------------------------------------------------------
                var mutChar = ".";
                //  ReadablePasswordGenerator delimits non-mutated words with spaces
                var inputSource = fixture.Create<TestPassphrase>();
                var input = inputSource.Phrase;
                // An previous mutator may/not have written something at the end with no following whitespace
                var sb = new StringBuilder(input + endWhiteSpace);
                //  we expect one less insertion points than there are words (no inserting at start or end)
                var expectRandLimit = inputSource.Words.Count - 1;
                //  The constant mutator is placed between words with its own trailing whitespace
                var wordsCopy = new List<string>(inputSource.Words);
                wordsCopy.Insert(randIndex + 1, mutChar);
                var expected = Join(' ', wordsCopy.ToList()) + endWhiteSpace;

                var rnd = new Mock<IRandomSourceBase>();
                //  Fix the random gen to always return the target index
                var actualRandLimit = -1;
                rnd.Setup(x => x.Next(It.IsAny<int>()))
                    .Returns<int>(x =>
                    {
                        actualRandLimit = x;
                        return randIndex;
                    });

                var sut = new ConstantMutator
                {
                    ValueToAdd = mutChar,
                    When = ConstantStyles.MiddleOfPhrase,
                    Whitespace = ' '
                };

                //  ACT ------------------------------------------------------------
                sut.Mutate(sb, rnd.Object);
                var actual = sb.ToString();

                //  ASSERT ---------------------------------------------------------
                Assert.Multiple(() =>
                {
                    Assert.That(actualRandLimit, Is.EqualTo(expectRandLimit), "Random.Next max param not as expected");
                    Assert.That(actual, Is.EqualTo(expected));
                });
            }

            [Test]
            public void Mutate_ForAnywhere_AddsMutation_BetweenWords(
                [Values("", " ")] string endWhiteSpace, [Values(0, 1, 2)] int randIndex)
            {
                var fixture = new Fixture();
                fixture.Customizations.Add(new TestPassphraseBuilder(WordList.Words, 4));
                //  ARRANGE --------------------------------------------------------
                var mutChar = ".";
                //  ReadablePasswordGenerator delimits non-mutated words with spaces
                var inputSource = fixture.Create<TestPassphrase>();
                var input = inputSource.Phrase;
                // An previous mutator may/not have written something at the end with no following whitespace
                var sb = new StringBuilder(input + endWhiteSpace);
                //  we expect one less insertion points than there are words plus start and end points
                var expectRandLimit = inputSource.Words.Count - 1 + 2;
                //  The constant mutator is placed between words with its own trailing whitespace
                var wordsCopy = new List<string>(inputSource.Words);
                wordsCopy.Insert(randIndex + 1, mutChar);
                var expected = Join(' ', wordsCopy.ToList()) + endWhiteSpace;

                var rnd = new Mock<IRandomSourceBase>();
                //  Fix the random gen to always return the target index
                var actualRandLimit = -1;
                rnd.Setup(x => x.Next(It.IsAny<int>()))
                    .Returns<int>(x =>
                    {
                        actualRandLimit = x;
                        return randIndex + 2;
                    });

                var sut = new ConstantMutator
                {
                    ValueToAdd = mutChar,
                    When = ConstantStyles.Anywhere,
                    Whitespace = ' '
                };

                //  ACT ------------------------------------------------------------
                sut.Mutate(sb, rnd.Object);
                var actual = sb.ToString();

                //  ASSERT ---------------------------------------------------------
                Assert.Multiple(() =>
                {
                    Assert.That(actualRandLimit, Is.EqualTo(expectRandLimit), "Random.Next max param not as expected");
                    Assert.That(actual, Is.EqualTo(expected));
                });
            }

            [Test]
            public void Mutate_ForEndOfPhrase_AddsMutation_AtEnd([Values("", " ")] string endWhiteSpace)
            {
                var fixture = new Fixture();
                fixture.Customizations.Add(new TestPassphraseBuilder(WordList.Words));
                //  ARRANGE --------------------------------------------------------
                var mutChar = ".";
                //  ReadablePasswordGenerator delimits non-mutated words with spaces
                var inputSource = fixture.Create<TestPassphrase>();
                var input = inputSource.Phrase;
                // An previous mutator may/not have written something at the end with no following whitespace
                var sb = new StringBuilder(input + endWhiteSpace);
                //  The constant mutator is placed after the ending whitespace
                var expected = $"{input}{endWhiteSpace}{mutChar}";

                var sut = new ConstantMutator
                {
                    ValueToAdd = mutChar,
                    When = ConstantStyles.EndOfPhrase,
                    Whitespace = ' '
                };

                //  ACT ------------------------------------------------------------
                sut.Mutate(sb, new CryptoRandomSource());
                var actual = sb.ToString();

                //  ASSERT ---------------------------------------------------------
                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Mutate_ForAnywhere_AddsMutation_AtEnd([Values("", " ")] string endWhiteSpace)
            {
                var fixture = new Fixture();
                fixture.Customizations.Add(new TestPassphraseBuilder(WordList.Words));
                //  ARRANGE --------------------------------------------------------
                var mutChar = ".";
                //  ReadablePasswordGenerator delimits non-mutated words with spaces
                var inputSource = fixture.Create<TestPassphrase>();
                var input = inputSource.Phrase;
                // An previous mutator may/not have written something at the end with no following whitespace
                var sb = new StringBuilder(input + endWhiteSpace);
                //  The constant mutator is placed after the ending whitespace
                var expected = $"{input}{endWhiteSpace}{mutChar}";

                var rnd = new Mock<IRandomSourceBase>();
                //  Fix the random gen to always return the end index
                rnd.Setup(x => x.Next(It.IsAny<int>()))
                    .Returns<int>(x => 1);

                var sut = new ConstantMutator
                {
                    ValueToAdd = mutChar,
                    When = ConstantStyles.Anywhere,
                    Whitespace = ' '
                };

                //  ACT ------------------------------------------------------------
                sut.Mutate(sb, rnd.Object);
                var actual = sb.ToString();

                //  ASSERT ---------------------------------------------------------
                Assert.That(actual, Is.EqualTo(expected));
            }
        }
    }
}
