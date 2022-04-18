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
            cboFilter.Text = "ALL";
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cboFilter_TextChanged(object sender, EventArgs e)
        {
            if (cboFilter.Text == "ALL")
                UpdateTotals(AllWordsPredicate);
            else if (cboFilter.Text == "Regular")
                UpdateTotals(RegularWordsPredicate);
            else if (cboFilter.Text == "Fake")
                UpdateTotals(FakeWordsPredicate);
            else
                UpdateTotals(AllWordsPredicate);
        }

        private void lnkTotals_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Browse to doco page for totals.
            this.bgwWorker.RunWorkerAsync(new Uri("https://github.com/ligos/readablepassphrasegenerator/wiki/Dictionary-Totals"));
        }

        private void bgwWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var url = e.Argument as Uri;
            if (url != null)
            {
                System.Diagnostics.Process.Start(url.ToString());
            }
        }

        private void UpdateTotals(Func<Word, bool> wordPredicate)
        {
            this.txtNouns.Text = this._Dictionary.CountOf<Noun>(wordPredicate).ToString("N0");
            this.txtProperNouns.Text = this._Dictionary.CountOf<ProperNoun>(wordPredicate).ToString("N0");
            this.txtVerbs.Text = this._Dictionary.CountOf<Verb>(wordPredicate).ToString("N0");
            this.txtSpeechVerbs.Text = this._Dictionary.CountOf<SpeechVerb>(wordPredicate).ToString("N0");
            this.txtAdjectives.Text = this._Dictionary.CountOf<Adjective>(wordPredicate).ToString("N0");
            this.txtAdverbs.Text = this._Dictionary.CountOf<Adverb>(wordPredicate).ToString("N0");
            this.txtPrepositions.Text = this._Dictionary.CountOf<Preposition>(wordPredicate).ToString("N0");
            this.txtDemonstratives.Text = this._Dictionary.CountOf<Demonstrative>(wordPredicate).ToString("N0");
            this.txtTheArticle.Text = this._Dictionary.CountOf<Article>(wordPredicate).ToString("N0");
            this.txtPersonalPronouns.Text = this._Dictionary.CountOf<PersonalPronoun>(wordPredicate).ToString("N0");
            this.txtIndefinitePronouns.Text = this._Dictionary.CountOf<IndefinitePronoun>(wordPredicate).ToString("N0");
            this.txtInterrogatives.Text = this._Dictionary.CountOf<Interrogative>(wordPredicate).ToString("N0");
            this.txtConjunctions.Text = this._Dictionary.CountOf<Conjunction>(wordPredicate).ToString("N0");
            this.txtNumbers.Text = this._Dictionary.CountOf<Number>(wordPredicate).ToString("N0");

            this.txtTotal.Text = (this._Dictionary.CountOf<Noun>(wordPredicate)
                                 + this._Dictionary.CountOf<ProperNoun>(wordPredicate)
                                 + this._Dictionary.CountOf<Verb>(wordPredicate)
                                 + this._Dictionary.CountOf<SpeechVerb>(wordPredicate)
                                 + this._Dictionary.CountOf<Adjective>(wordPredicate)
                                 + this._Dictionary.CountOf<Adverb>(wordPredicate)
                                 + this._Dictionary.CountOf<Preposition>(wordPredicate)
                                 + this._Dictionary.CountOf<Demonstrative>(wordPredicate)
                                 + this._Dictionary.CountOf<Article>(wordPredicate)
                                 + this._Dictionary.CountOf<PersonalPronoun>(wordPredicate)
                                 + this._Dictionary.CountOf<Interrogative>(wordPredicate)
                                 + this._Dictionary.CountOf<Conjunction>(wordPredicate)
                                 + this._Dictionary.CountOf<IndefinitePronoun>(wordPredicate)
                                 + this._Dictionary.CountOf<Number>(wordPredicate)
                                 ).ToString("N0");
            this.txtReconciledTotal.Text = this._Dictionary.CountAll(wordPredicate).ToString("N0");
            this.txtTotalForms.Text = this._Dictionary.CountOfAllDistinctForms(wordPredicate).ToString("N0");
        }


        private static bool AllWordsPredicate(Word w)
        {
            return true;
        }

        private static bool FakeWordsPredicate(Word w)
        {
            return w.Tags.Contains(Tags.Fake);
        }

        private static bool RegularWordsPredicate(Word w)
        {
            return w.Tags.Count == 0;
        }

    }
}
