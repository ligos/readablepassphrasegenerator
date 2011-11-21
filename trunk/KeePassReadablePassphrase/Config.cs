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
using MurrayGrant.ReadablePassphrase;
using MurrayGrant.ReadablePassphrase.PhraseDescription;
using System.Xml;

namespace KeePassReadablePassphrase
{
    public class Config
    {
        public bool SpacesBetweenWords { get; set; }
        private PhraseStrength _PhraseSelection;
        public PhraseStrength PhraseStrength { get { return this._PhraseSelection; } set { this._PhraseSelection = value; this.UpdatePhraseDescription(); } }
        public IEnumerable<Clause> PhraseDescription { get; set; }
        public bool UseCustomDictionary { get; set; }
        public string PathOfCustomDictionary { get; set; }

        public Config()
        {
            this.LoadDefaults();
        }
        public Config(string configFromKeePass)
        {
            if (String.IsNullOrEmpty(configFromKeePass))
            {
                this.LoadDefaults();
            }
            else
            {
                this.ParseConfig(configFromKeePass);
            }
        }

        private void LoadDefaults()
        {
            // Defaults.
            SpacesBetweenWords = true;
            PhraseStrength = PhraseStrength.Normal;
            this.UpdatePhraseDescription();
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
                    this.SpacesBetweenWords = Boolean.Parse(reader.GetAttribute("value"));
                else if (reader.NodeType == XmlNodeType.Element && reader.Name.ToLower() == "phrasestrength")
                    this.PhraseStrength = (PhraseStrength)Enum.Parse(typeof(PhraseStrength), reader.GetAttribute("value"));
                else if (reader.NodeType == XmlNodeType.Element && reader.Name.ToLower() == "usecustomdictionary")
                    this.UseCustomDictionary = Boolean.Parse(reader.GetAttribute("value"));
                else if (reader.NodeType == XmlNodeType.Element && reader.Name.ToLower() == "pathofcustomdictionary")
                    this.PathOfCustomDictionary = reader.GetAttribute("value");
                else if (reader.NodeType == XmlNodeType.Element && reader.Name.ToLower() == "phrasedescription")
                    inPhraseDescription = true;
                else if (reader.NodeType == XmlNodeType.CDATA && inPhraseDescription)
                {
                    this.PhraseDescription = Clause.CreateCollectionFromTextString(reader.Value);
                    inPhraseDescription = false;
                }
            }

            if (this.PhraseStrength != PhraseStrength.Custom)
                this.UpdatePhraseDescription();
        }
        public string ToConfigString()
        {
            // This gets saved / parsed from KeePass's config file.
            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine("<ReadablePassphraseConfig>");
            sb.AppendFormat("<SpacesBetweenWords value=\"{0}\"/>\n", this.SpacesBetweenWords);
            sb.AppendFormat("<PhraseStrength value=\"{0}\"/>\n", this.PhraseStrength);
            sb.AppendLine("<PhraseDescription>");
            sb.Append("<![CDATA[");
            foreach (var c in this.PhraseDescription)
                c.ToStringBuilder(sb);
            sb.Append("]]>");
            sb.AppendLine("</PhraseDescription>");
            sb.AppendFormat("<UseCustomDictionary value=\"{0}\"/>\n", this.UseCustomDictionary);
            sb.AppendFormat("<PathOfCustomDictionary value=\"{0}\"/>\n", this.PathOfCustomDictionary);
            sb.AppendLine("</ReadablePassphraseConfig>");
            return sb.ToString();
        }


        private void UpdatePhraseDescription()
        {
            if (this.PhraseStrength != PhraseStrength.Custom)
                this.PhraseDescription = MurrayGrant.ReadablePassphrase.PhraseDescription.Clause.CreatePhraseDescription(this.PhraseStrength);
        }
    }
}
