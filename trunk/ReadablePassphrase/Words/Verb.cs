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

namespace MurrayGrant.ReadablePassphrase.Words
{
    public class Verb : Word
    {
        public string PresentSingular { get; private set; }
        public string PastSingular { get; private set; }
        public string PastContinuousSingular { get; private set; }
        public string FutureSingular { get; private set; }
        public string ContinuousSingular { get; private set; }
        public string PerfectSingular { get; private set; }
        public string SubjunctiveSingular { get; private set; }

        public string PresentPlural { get; private set; }
        public string PastPlural { get; private set; }
        public string PastContinuousPlural { get; private set; }
        public string FuturePlural { get; private set; }
        public string ContinuousPlural { get; private set; }
        public string PerfectPlural { get; private set; }
        public string SubjunctivePlural { get; private set; }

        public bool HasForm(VerbTense tense, bool isPlural)
        {
            return !String.IsNullOrEmpty(GetForm(tense, isPlural));
        }
        public string GetForm(VerbTense tense, bool isPlural)
        {
            if (tense == VerbTense.Present && !isPlural)
                return this.PresentSingular;
            else if (tense == VerbTense.Present && isPlural)
                return this.PresentPlural;
            else if (tense == VerbTense.Past && !isPlural)
                return this.PastSingular;
            else if (tense == VerbTense.Past && isPlural)
                return this.PastPlural;
            else if (tense == VerbTense.Future && !isPlural)
                return this.FutureSingular;
            else if (tense == VerbTense.Future && isPlural)
                return this.FuturePlural;
            else if (tense == VerbTense.Continuous && !isPlural)
                return this.ContinuousSingular;
            else if (tense == VerbTense.Continuous && isPlural)
                return this.ContinuousPlural;
            else if (tense == VerbTense.ContinuousPast && !isPlural)
                return this.PastContinuousSingular;
            else if (tense == VerbTense.ContinuousPast && isPlural)
                return this.PastContinuousPlural;
            else if (tense == VerbTense.Perfect && !isPlural)
                return this.PerfectSingular;
            else if (tense == VerbTense.Perfect && isPlural)
                return this.PerfectPlural;
            else if (tense == VerbTense.Subjunctive && !isPlural)
                return this.SubjunctiveSingular;
            else if (tense == VerbTense.Subjunctive && isPlural)
                return this.SubjunctivePlural;
            else
                throw new ApplicationException(String.Format("Unexpected case of tense and isPlural.", tense, isPlural));
        }

        public override string DictionaryEntry { get { return this.PresentPlural; } }       // This is most likely to detect duplicates.

        internal Verb(XmlReader reader)
        {
            PresentSingular = reader.GetAttribute("presentSingular");
            PastSingular = reader.GetAttribute("pastSingular");
            PastContinuousSingular = reader.GetAttribute("pastContinuousSingular");
            FutureSingular = reader.GetAttribute("futureSingular");
            ContinuousSingular = reader.GetAttribute("continuousSingular");
            PerfectSingular = reader.GetAttribute("perfectSingular");
            SubjunctiveSingular = reader.GetAttribute("subjunctiveSingular");

            PresentPlural = reader.GetAttribute("presentPlural");
            PastPlural = reader.GetAttribute("pastPlural");
            PastContinuousPlural = reader.GetAttribute("pastContinuousPlural");
            FuturePlural = reader.GetAttribute("presentSingular");
            ContinuousPlural = reader.GetAttribute("continuousPlural");
            PerfectPlural = reader.GetAttribute("futurePlural");
            SubjunctivePlural = reader.GetAttribute("subjunctivePlural");
        }
    }
}
