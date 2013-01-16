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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MurrayGrant.ReadablePassphrase.Dictionaries;
using MurrayGrant.ReadablePassphrase.Words;

namespace KeePassReadablePassphrase
{
    public partial class DictionarySizeDetail : Form
    {
        private WordDictionary _Dictionary;
        public DictionarySizeDetail(WordDictionary dictionary)
        {
            this._Dictionary = dictionary;
            InitializeComponent();
        }

        private void DictionarySizeDetail_Load(object sender, EventArgs e)
        {
            this.txtNouns.Text = this._Dictionary.CountOf<Noun>().ToString("N0");
            this.txtVerbs.Text = this._Dictionary.CountOf<Verb>().ToString("N0");
            this.txtAdjectives.Text = this._Dictionary.CountOf<Adjective>().ToString("N0");
            this.txtAdverbs.Text = this._Dictionary.CountOf<Adverb>().ToString("N0");
            this.txtPrepositions.Text = this._Dictionary.CountOf<Preposition>().ToString("N0");
            this.txtDemonstratives.Text = this._Dictionary.CountOf<Demonstrative>().ToString("N0");
            this.txtTheArticle.Text = this._Dictionary.CountOf<Article>().ToString("N0");
            this.txtPersonalPronouns.Text = this._Dictionary.CountOf<PersonalPronoun>().ToString("N0");
            this.txtInterrogatives.Text = this._Dictionary.CountOf<Interrogative>().ToString("N0");

            this.txtTotal.Text = (this._Dictionary.CountOf<Noun>()
                                 + this._Dictionary.CountOf<Verb>()
                                 + this._Dictionary.CountOf<Adjective>()
                                 + this._Dictionary.CountOf<Adverb>()
                                 + this._Dictionary.CountOf<Preposition>()
                                 + this._Dictionary.CountOf<Demonstrative>()
                                 + this._Dictionary.CountOf<Article>()
                                 + this._Dictionary.CountOf<PersonalPronoun>()
                                 + this._Dictionary.CountOf<Interrogative>()
                                 ).ToString("N0");
            this.txtReconciledTotal.Text = this._Dictionary.Count.ToString("N0");
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
