using System;
using System.Collections.Generic;
using System.Linq;

namespace MurrayGrant.WordScraper
{
    public class CommandLineArguments
    {
        public int WordCount { get; private set; } = 10;
        public int MinLength { get; private set; } = 3;
        public int MaxLength { get; private set; } = 10;
        public int Attempts { get; private set; } = 1000;
        public string Source { get; private set; } = "";
        public int DelayMs { get; private set; } = 250;
        public int ShowCount { get; private set; } = 10;
        public string ResumeFrom { get; private set; } = "";

        public static readonly IReadOnlyDictionary<string, SourceDefinition> SupportedSources = new[]
        {
            new SourceDefinition()
            {
                Name = "ThisWordDoesNotExist.com",
                Tags = "fake",
            },
            new SourceDefinition()
            {
                Name = "Dictionary.com",
            }
        }.ToDictionary(x => x.Name!, x => x, StringComparer.CurrentCultureIgnoreCase);

        public class SourceDefinition
        {
            public string? Name { get; init; }
            public string? Tags { get; init; }
        }

        public static (CommandLineArguments? result, int? exitCode) Parse(string[] args)
        {
            if (args.Length == 0)
                return (null, 0);

            var result = new CommandLineArguments();
            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i].ToLower().Trim();
                if (arg.StartsWith("-") || arg.StartsWith("--") || arg.StartsWith("/"))
                    arg = arg.Replace("--", "").Replace("-", "").Replace("/", "");

                if (i == 0)
                {
                    if (!SupportedSources.ContainsKey(arg))
                    {
                        Console.WriteLine("Source '{0}' is not supported. Supported sources:", arg);
                        foreach (var source in SupportedSources.OrderBy(x => x))
                        {
                            Console.WriteLine("  " + source);
                        }
                        return (null, 1);
                    }
                    result.Source = arg;
                }
                else if (arg == "c" || arg == "count")
                {
                    if (!Int32.TryParse(args[i + 1].Trim(), out var wordCount))
                    {
                        Console.WriteLine("Unable to parse number '{0}' for 'count' option.", args[i + 1]);
                        return (null, 1);
                    }
                    result.WordCount = wordCount;
                    i++;
                }
                else if (arg == "a" || arg == "attempts")
                {
                    if (!Int32.TryParse(args[i + 1].Trim(), out var attempts))
                    {
                        Console.WriteLine("Unable to parse number '{0}' for 'attempts' option.", args[i + 1]);
                        return (null, 1);
                    }
                    result.Attempts = attempts;
                    i++;
                }
                else if (arg == "min")
                {
                    if (!Int32.TryParse(args[i + 1].Trim(), out var minLength))
                    {
                        Console.WriteLine("Unable to parse number '{0}' for 'min' option.", args[i + 1]);
                        return (null, 1);
                    }
                    result.MinLength = minLength;
                    i++;
                }
                else if (arg == "max")
                {
                    if (!Int32.TryParse(args[i + 1].Trim(), out var maxLength))
                    {
                        Console.WriteLine("Unable to parse number '{0}' for 'max' option.", args[i + 1]);
                        return (null, 1);
                    }
                    result.MaxLength = maxLength;
                    i++;
                }
                else if (arg == "d" || arg == "delayms")
                {
                    if (!Int32.TryParse(args[i + 1].Trim(), out var delayMs))
                    {
                        Console.WriteLine("Unable to parse number '{0}' for 'delayMs' option.", args[i + 1]);
                        return (null, 1);
                    }
                    result.DelayMs = delayMs;
                    i++;
                }
                else if (arg == "d" || arg == "show")
                {
                    if (!Int32.TryParse(args[i + 1].Trim(), out var showCount))
                    {
                        Console.WriteLine("Unable to parse number '{0}' for 'show' option.", args[i + 1]);
                        return (null, 1);
                    }
                    result.ShowCount = showCount;
                    i++;
                }
                else if (arg == "resume")
                {
                    result.ResumeFrom = args[i + 1].Trim();
                    i++;
                }
                else if (arg == "h" || arg == "help")
                {
                    return (null, 0);
                }
                else
                {
                    Console.WriteLine("Unknown argument '{0}'.", arg);
                    return (null, 1);
                }
            }

            return (result, null);
        }

        public static void PrintUsage()
        {
            var defaultArgs = new CommandLineArguments();
            Console.WriteLine("Usage: WordScraper.exe source [options]");
            Console.WriteLine("  -c --count nnn        Scrapes nnn words (default: {0})", defaultArgs.WordCount);
            Console.WriteLine("  --min xxx             Specifies a minimum length for words (def: {0})", defaultArgs.MinLength);
            Console.WriteLine("  --max xxx             Specifies a maximum length for words (def: {0})", defaultArgs.MaxLength);
            Console.WriteLine("  -a --attempts nnn     Maximum attempts to scrape (default: {0})", defaultArgs.Attempts);
            Console.WriteLine("  -d --delayMs nnn      Milliseconds of delay after each attempt (default: {0})", defaultArgs.DelayMs);
            Console.WriteLine("  --show nnn            Show sample list of words after scraping (default: {0})", defaultArgs.ShowCount);
            Console.WriteLine();
            Console.WriteLine("  Supported sources:");
            foreach (var source in SupportedSources.Select(x => x.Value.Name).OrderBy(x => x))
            {
                Console.WriteLine("    " + source);
            }
            Console.WriteLine();
            Console.WriteLine("  --resume xxx          Source specific data to resume scraping", defaultArgs.ShowCount);
            Console.WriteLine();
            Console.WriteLine("  -h --help             Displays this message ");
            Console.WriteLine("See {0} for more information", ReadablePassphrase.ReadablePassphraseGenerator.GitHubHomepage);
            Console.WriteLine("Contact Murray via GitHub or at " + ReadablePassphrase.ReadablePassphraseGenerator.KeyBaseContact);
        }
    }
}
