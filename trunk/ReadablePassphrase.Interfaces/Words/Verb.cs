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

namespace MurrayGrant.ReadablePassphrase.Words
{
    public abstract class Verb : Word
    {
        public abstract string PresentSingular { get; }
        public abstract string PastSingular { get; }
        public abstract string PastContinuousSingular { get; }
        public abstract string FutureSingular { get; }
        public abstract string ContinuousSingular { get; }
        public abstract string PerfectSingular { get; }
        public abstract string SubjunctiveSingular { get; }

        public abstract string PresentPlural { get; }
        public abstract string PastPlural { get; }
        public abstract string PastContinuousPlural { get; }
        public abstract string FuturePlural { get; }
        public abstract string ContinuousPlural { get; }
        public abstract string PerfectPlural { get; }
        public abstract string SubjunctivePlural { get; }

        public abstract bool IsTransitive { get; }

        public override string DictionaryEntry { get { return this.PresentPlural; } }       // This is most likely to detect duplicates in english.
        public sealed override Type OfType { get { return typeof(Verb); } }


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
                throw new ApplicationException(String.Format("Unexpected case of tense ({0}) and isPlural ({1}).", tense, isPlural));
        }
    }

    public enum VerbTense
    {
        Present,
        Past,
        Future,
        Continuous,
        ContinuousPast,
        Perfect,
        Subjunctive
    }
}
