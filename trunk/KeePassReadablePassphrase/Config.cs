﻿// Copyright 2011 Murray Grant
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
using MurrayGrant.ReadablePassphrase;
using MurrayGrant.ReadablePassphrase.PhraseDescription;
using MurrayGrant.ReadablePassphrase.Mutators;
using System.Xml;

namespace KeePassReadablePassphrase
{
    public class Config
    {
        [Obsolete("Use WordSeparator instead")]
        public bool SpacesBetweenWords { get; set; }
        public WordSeparatorOption WordSeparator { get; set; }
        public string CustomSeparator { get; set; }
        private PhraseStrength _PhraseSelection;
        public PhraseStrength PhraseStrength
        {
            get { return this._PhraseSelection; }
            set
            {
                this._PhraseSelection = value;
                this.PhraseDescription = this.GetPhraseDescription();
            }
        }
        public IEnumerable<Clause> PhraseDescription { get; set; }
        public bool UseCustomDictionary { get; set; }
        public string PathOfCustomDictionary { get; set; }
        public int MinLength { get; set; }
        public int MaxLength { get; set; }

        public CountByOption CountBy { get; set; }
        public MutatorOption Mutator { get; set; }
        public AllUppercaseStyles UpperStyle { get; set; }
        public int UpperCount { get; set; }
        public NumericStyles NumericStyle { get; set; }
        public int NumericCount { get; set; }
        public ConstantStyles ConstantStyle { get; set; }
        public string ConstantValue { get; set; }

        public string ActualSeparator
        {
            get
            {
                return this.WordSeparator == WordSeparatorOption.None ? "" 
                     : this.WordSeparator == WordSeparatorOption.Space ? " "
                     : this.WordSeparator == WordSeparatorOption.Custom ? this.CustomSeparator
                     : "";
            }
        }

        public static readonly Config Default = new Config();

        public Config()
        {
            // Defaults.
#pragma warning disable CS0618
            SpacesBetweenWords = true;
#pragma warning restore
            WordSeparator = WordSeparatorOption.Space;
            CustomSeparator = "";
            PhraseStrength = PhraseStrength.Random;
            MinLength = 1;
            MaxLength = 999;
            this.CountBy = CountByOption.Letters;
            this.PhraseDescription = this.GetPhraseDescription();
            this.Mutator = MutatorOption.None;
            this.UpperStyle = AllUppercaseStyles.WholeWord;
            this.UpperCount = UppercaseWordMutator.Basic.NumberOfWordsToCapitalise;
            this.NumericStyle = NumericMutator.Basic.When;
            this.NumericCount = NumericMutator.Basic.NumberOfNumbersToAdd;
            this.ConstantStyle = ConstantMutator.Basic.When;
            this.ConstantValue = ConstantMutator.Basic.ValueToAdd;
            this.PathOfCustomDictionary = "";
        }
        public Config(string configFromKeePass)
            : this()
        {
            if (!String.IsNullOrEmpty(configFromKeePass))
                this.ParseConfig(configFromKeePass);
        }

        private void ParseConfig(string configFromKeePass)
        {
            // Load the config up from ToConfigString().
            var reader = XmlReader.Create(new System.IO.StringReader(configFromKeePass));
            bool inPhraseDescription = false;
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name.ToLower() == "readablepassphraseconfig")
                    { }// NoOp
                else if (reader.NodeType == XmlNodeType.Element && reader.Name.ToLower() == "spacesbetweenwords")
#pragma warning disable CS0618
                    this.SpacesBetweenWords = Boolean.Parse(reader.GetAttribute("value"));
#pragma warning restore
                else if (reader.NodeType == XmlNodeType.Element && reader.Name.ToLower() == "wordseparator")
                    this.WordSeparator = (WordSeparatorOption)Enum.Parse(typeof(WordSeparatorOption), reader.GetAttribute("value").Replace(" ", ""));
                else if (reader.NodeType == XmlNodeType.Element && reader.Name.ToLower() == "customseparator")
                    this.CustomSeparator = reader.GetAttribute("value");
                else if (reader.NodeType == XmlNodeType.Element && reader.Name.ToLower() == "phrasestrength")
                    this.PhraseStrength = (PhraseStrength)Enum.Parse(typeof(PhraseStrength), reader.GetAttribute("value").Replace("Speach", "Speech"));
                else if (reader.NodeType == XmlNodeType.Element && reader.Name.ToLower() == "usecustomdictionary")
                    this.UseCustomDictionary = Boolean.Parse(reader.GetAttribute("value"));
                else if (reader.NodeType == XmlNodeType.Element && reader.Name.ToLower() == "pathofcustomdictionary")
                    this.PathOfCustomDictionary = reader.GetAttribute("value");
                else if (reader.NodeType == XmlNodeType.Element && reader.Name.ToLower() == "phrasedescription")
                    inPhraseDescription = true;
                else if (reader.NodeType == XmlNodeType.Element && reader.Name.ToLower() == "maxlength")
                    this.MaxLength = Int32.Parse(reader.GetAttribute("value"));
                else if (reader.NodeType == XmlNodeType.Element && reader.Name.ToLower() == "minlength")
                    this.MinLength = Int32.Parse(reader.GetAttribute("value"));
                else if (reader.NodeType == XmlNodeType.Element && reader.Name.ToLower() == "countby")
                    this.CountBy = (CountByOption)Enum.Parse(typeof(CountByOption), reader.GetAttribute("value").Replace(" ", ""));
                else if (reader.NodeType == XmlNodeType.CDATA && inPhraseDescription)
                {
                    this.PhraseDescription = Clause.CreateCollectionFromTextString(reader.Value);
                    inPhraseDescription = false;
                }
                else if (reader.NodeType == XmlNodeType.Element && reader.Name.ToLower() == "mutator")
                    this.Mutator = (MutatorOption)Enum.Parse(typeof(MutatorOption), reader.GetAttribute("value").Replace(" ", ""));
                else if (reader.NodeType == XmlNodeType.Element && reader.Name.ToLower() == "upperstyle")
                    this.UpperStyle = (AllUppercaseStyles)Enum.Parse(typeof(AllUppercaseStyles), reader.GetAttribute("value").Replace(" ", ""));
                else if (reader.NodeType == XmlNodeType.Element && reader.Name.ToLower() == "uppercount")
                    this.UpperCount = Int32.Parse(reader.GetAttribute("value"));
                else if (reader.NodeType == XmlNodeType.Element && reader.Name.ToLower() == "numericstyle")
                    this.NumericStyle = (NumericStyles)Enum.Parse(typeof(NumericStyles), reader.GetAttribute("value").Replace(" ", ""));
                else if (reader.NodeType == XmlNodeType.Element && reader.Name.ToLower() == "numericcount")
                    this.NumericCount = Int32.Parse(reader.GetAttribute("value"));
                else if (reader.NodeType == XmlNodeType.Element && reader.Name.ToLower() == "constantstyle")
                    this.ConstantStyle = (ConstantStyles)Enum.Parse(typeof(ConstantStyles), reader.GetAttribute("value").Replace(" ", ""));
                else if (reader.NodeType == XmlNodeType.Element && reader.Name.ToLower() == "constantvalue")
                    this.ConstantValue = reader.GetAttribute("value");
            }

            if (this.PhraseStrength != PhraseStrength.Custom)
                this.PhraseDescription = this.GetPhraseDescription();

            if (MinLength < 1)
                MinLength = 1;
            if (MinLength > 999)
                MinLength = 999;
            if (MaxLength < 1)
                MaxLength = 1;
            if (MaxLength > 999)
                MaxLength = 999;

            if (UpperCount < 0)
                UpperCount = 0;
            if (UpperCount > 999)
                UpperCount = 999;
            if (NumericCount < 0)
                NumericCount = 0;
            if (NumericCount > 999)
                NumericCount = 999;

            if (CountBy == CountByOption.None)
                CountBy = CountByOption.Letters;

#pragma warning disable CS0618
            if (WordSeparator == WordSeparatorOption.Unknown && SpacesBetweenWords)
                WordSeparator = WordSeparatorOption.Space;
            else if (WordSeparator == WordSeparatorOption.Unknown && !SpacesBetweenWords)
                WordSeparator = WordSeparatorOption.None;
#pragma warning restore
        }
        public string ToConfigString()
        {
            // This gets saved / parsed from KeePass's config file.
            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine("<ReadablePassphraseConfig>");
            sb.AppendFormat("<WordSeparator value=\"{0}\"/>\n", this.WordSeparator);
            sb.AppendFormat("<CustomSeparator value=\"{0}\"/>\n", EncodeForXml(this.CustomSeparator));
            sb.AppendFormat("<PhraseStrength value=\"{0}\"/>\n", this.PhraseStrength);
            sb.AppendLine("<PhraseDescription>");
            if (!Clause.RandomMappings.ContainsKey(this.PhraseStrength))
            {
                sb.Append("<![CDATA[");
                foreach (var c in this.PhraseDescription)
                    c.ToStringBuilder(sb);
                sb.Append("]]>");
            }
            sb.AppendLine("</PhraseDescription>");
            sb.AppendFormat("<UseCustomDictionary value=\"{0}\"/>\n", this.UseCustomDictionary);
            sb.AppendFormat("<PathOfCustomDictionary value=\"{0}\"/>\n", EncodeForXml(this.PathOfCustomDictionary));
            sb.AppendFormat("<MinLength value=\"{0}\"/>\n", this.MinLength);
            sb.AppendFormat("<MaxLength value=\"{0}\"/>\n", this.MaxLength);
            sb.AppendFormat("<CountBy value=\"{0}\"/>\n", this.CountBy);
            sb.AppendFormat("<Mutator value=\"{0}\"/>\n", this.Mutator);
            sb.AppendFormat("<UpperStyle value=\"{0}\"/>\n", this.UpperStyle);
            sb.AppendFormat("<UpperCount value=\"{0}\"/>\n", this.UpperCount);
            sb.AppendFormat("<NumericStyle value=\"{0}\"/>\n", this.NumericStyle);
            sb.AppendFormat("<NumericCount value=\"{0}\"/>\n", this.NumericCount);
            sb.AppendFormat("<ConstantStyle value=\"{0}\"/>\n", this.ConstantStyle);
            sb.AppendFormat("<ConstantValue value=\"{0}\"/>\n", EncodeForXml(this.ConstantValue));
            sb.AppendLine("</ReadablePassphraseConfig>");
            return sb.ToString();
        }


        private IEnumerable<Clause> GetPhraseDescription()
        {
            if (this.PhraseStrength != PhraseStrength.Custom && !Clause.RandomMappings.ContainsKey(this.PhraseStrength))
                return Clause.CreatePhraseDescription(this.PhraseStrength, new KeePassRandomSource());
            else
                return Enumerable.Empty<Clause>();
        }

        private static string EncodeForXml(string xml)
        {
            if (string.IsNullOrEmpty(xml))
                return xml;
            var result = xml.Replace("\"", "&quot;")
                            .Replace("&", "&amp;")
                            .Replace("'", "&apos;")
                            .Replace("<", "&lt;")
                            .Replace(">", "&gt;");
            return result;
        }
    }

    public enum MutatorOption
    {
        None,
        Standard,
        Custom,
    }

    public enum WordSeparatorOption
    {
        Unknown,
        None,
        Space,
        Custom,
    }

    public enum CountByOption
    {
        None,
        Letters,
        Words,
    }
}
