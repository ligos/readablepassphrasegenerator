// Copyright 2011 Murray Grant
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using MurrayGrant.ReadablePassphrase.WordTemplate;
using MurrayGrant.ReadablePassphrase.Words;

namespace MurrayGrant.ReadablePassphrase.MaterialisedWords
{
    public sealed class MaterialisedVerb : Verb
    {
        public override string PresentSingular { get; }
        public override string PastSingular { get; }
        public override string PastContinuousSingular { get; }
        public override string FutureSingular { get; }
        public override string ContinuousSingular { get; }
        public override string PerfectSingular { get; }
        public override string SubjunctiveSingular { get; }

        public override string PresentPlural { get; }
        public override string PastPlural { get; }
        public override string PastContinuousPlural { get; }
        public override string FuturePlural { get; }
        public override string ContinuousPlural { get; }
        public override string PerfectPlural { get; }
        public override string SubjunctivePlural { get; }

        public override bool IsTransitive { get; }

        public override IReadOnlyList<string> Tags { get; }

        public MaterialisedVerb(IDictionary<string, string> forms)
        {
            PresentSingular = GetOrDefault(forms, "presentSingular");
            PastSingular = GetOrDefault(forms, "pastSingular");
            PastContinuousSingular = GetOrDefault(forms, "pastContinuousSingular");
            FutureSingular = GetOrDefault(forms, "futureSingular");
            ContinuousSingular = GetOrDefault(forms, "continuousSingular");
            PerfectSingular = GetOrDefault(forms, "perfectSingular");
            SubjunctiveSingular = GetOrDefault(forms, "subjunctiveSingular");

            PresentPlural = GetOrDefault(forms, "presentPlural");
            PastPlural = GetOrDefault(forms, "pastPlural");
            PastContinuousPlural = GetOrDefault(forms, "pastContinuousPlural");
            FuturePlural = GetOrDefault(forms, "futurePlural");
            ContinuousPlural = GetOrDefault(forms, "continuousPlural");
            PerfectPlural = GetOrDefault(forms, "perfectPlural");
            SubjunctivePlural = GetOrDefault(forms, "subjunctivePlural");

            IsTransitive = GetOrDefault(forms, "transitive") == ""
                         || GetOrDefault(forms, "transitive").ToLowerInvariant() == "true";

            Tags = SplitTags(GetOrDefault(forms, "tags"));
        }
        public MaterialisedVerb(string presentSingular, string pastSingular, string pastContinuousSingular, string futureSingular, string continuousSingular, string perfectSingular, string subjunctiveSingular,
                                string presentPlural, string pastPlural, string pastContinuousPlural, string futurePlural, string continuousPlural, string perfectPlural, string subjunctivePlural,
                                bool isTransitive, 
                                IReadOnlyList<string> tags)
        {
            PresentSingular = presentSingular;
            PastSingular = pastSingular;
            PastContinuousSingular = pastContinuousSingular;
            FutureSingular = futureSingular;
            ContinuousSingular = continuousSingular;
            PerfectSingular = perfectSingular;
            SubjunctiveSingular = subjunctiveSingular;

            PresentPlural = presentPlural;
            PastPlural = pastPlural;
            PastContinuousPlural = pastContinuousPlural;
            FuturePlural = futurePlural;
            ContinuousPlural = continuousPlural;
            PerfectPlural = perfectPlural;
            SubjunctivePlural = subjunctivePlural;

            IsTransitive = isTransitive;
            Tags = tags;
        }

        private string GetOrDefault(IDictionary<string, string> dict, string key)
            => dict.TryGetValue(key, out var result) ? result : "";
    }
}
