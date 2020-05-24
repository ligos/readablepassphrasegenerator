using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoFixture.Kernel;

namespace MurrayGrant.ReadablePassphrase.TestHelpers
{
    public class TestPassphraseBuilder : ISpecimenBuilder
    {
        private readonly int _WordCount;
        private readonly IReadOnlyList<string> _Words;
        private readonly System.Random _rnd = new System.Random();

        public TestPassphraseBuilder(Lazy<IReadOnlyList<string>> words, int wordCount = 3)
        {
            _WordCount = wordCount;
            _Words = words.Value ?? throw new ArgumentNullException(nameof(words));
            //  It should have at least some entries
            if (_Words.Count < 10) throw new ArgumentException("words does not have enough entries", nameof(words));
        }

        object ISpecimenBuilder.Create(object request, ISpecimenContext context)
        {
            var pi = request as MemberInfo;
            if (pi?.Name == nameof(TestPassphrase))
            {
                var lim = _Words.Count;
                var words = Enumerable.Range(0, _WordCount)
                    .Select(_ => _Words[_rnd.Next(lim)])
                    .ToList();
                return new TestPassphrase(words);
            }

            return new NoSpecimen();
        }
    }
}
