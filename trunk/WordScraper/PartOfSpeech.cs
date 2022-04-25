using System;

namespace MurrayGrant.WordScraper
{
    public enum PartOfSpeech
    {
        Unknown = 0,

        Noun,
        NounPlural,
        VerbTransitive,
        VerbIntransitive,
        VerbOther,
        Adjective,
        Adverb,

        Preposition,
        Conjunction,
        Interjection,
        Abbreviation,
        Pronoun,
    }

    public static class PartOfSpeechExtensions
    {
        public static bool IsSupported(this PartOfSpeech pos)
            => pos == PartOfSpeech.Noun
            || pos == PartOfSpeech.NounPlural
            || pos == PartOfSpeech.VerbTransitive
            || pos == PartOfSpeech.VerbIntransitive
            || pos == PartOfSpeech.VerbOther
            || pos == PartOfSpeech.Adjective
            || pos == PartOfSpeech.Adverb;
    }
}
