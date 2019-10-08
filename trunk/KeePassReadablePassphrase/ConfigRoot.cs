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
using MurrayGrant.ReadablePassphrase;
using MurrayGrant.ReadablePassphrase.Mutators;

namespace KeePassReadablePassphrase
{
    public partial class ConfigRoot : Form
    {
        public ConfigRoot(string configFromKeePass, MurrayGrant.ReadablePassphrase.Random.RandomSourceBase randomness)
        {
            this.ConfigForKeePass = configFromKeePass;
            this._Generator = new ReadablePassphraseGenerator(randomness);
            InitializeComponent();
        }

        public string ConfigForKeePass { get; private set; }
        private readonly ReadablePassphraseGenerator _Generator;
        private bool IsCurrentPhraseStrengthCustom { get { return (PhraseStrength)Enum.Parse(typeof(PhraseStrength), this.cboPhraseSelection.Text) == PhraseStrength.Custom; } }
        private bool _IsLoading = false;

        private void ConfigRoot_Load(object sender, EventArgs e)
        {
            var ver = ((System.Reflection.AssemblyFileVersionAttribute)typeof(ConfigRoot).Assembly.GetCustomAttributes(typeof(System.Reflection.AssemblyFileVersionAttribute), true).GetValue(0)).Version;
            var idx = ver.IndexOf('.', ver.IndexOf('.', ver.IndexOf('.')+1) +1);
            this.lblVersion.Text = "Version " + ver.Substring(0, idx);

            // Load up the config.
            Config config = null;
            try
            {
                config = new Config(this.ConfigForKeePass);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                MessageBox.Show(this, String.Format("Unable to load Readable Passphrase Config.{0}A default configuration will be used instead.{0}{0}{1}{0}{2}", Environment.NewLine, ex.Message, ex.InnerException != null ? ex.InnerException.Message : ""), "Readable Passphrase", MessageBoxButtons.OK, MessageBoxIcon.Error);
                config = new Config();
            }

            // Load the dictionary so we can display the number of words in the config window.
            var dict = PassphraseGenerator.LoadDictionary(config);
            this._Generator.SetDictionary(dict);

            // "Data Bind"
            this.ConfigObjectToForm(config);

            this.lblStatus.Text = "";
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            // Save.
            this.ConfigForKeePass = this.FormToConfigObject().ToConfigString();
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void cboPhraseSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            // If custom, display the phrase detail and grow the form size.
            var newConf = this.FormToConfigObject();
            this.UpdateDescription(newConf);
            this.UpdateCombinations(newConf);
            this.UpdateCustomStrengthVisibility(this.IsCurrentPhraseStrengthCustom);
        }
        private void mutatorStyleOrCountChanged(object sender, EventArgs e)
        {
            // The mutator style affects the number of combinations.
            var newConf = this.FormToConfigObject();
            this.UpdateCombinations(newConf);
        }
        private void radMutator_CheckedChanged(object sender, EventArgs e)
        {
            // The mutator style affects the number of combinations.
            var newConf = this.FormToConfigObject();
            this.UpdateCombinations(newConf);
            this.UpdateCustomMutatorControlEnabledStatus();
        }
        private void cboWordSeparator_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.UpdateCustomSeparatorEnabledStatus();
        }



        private void lnkWebsite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Browse to github site.
            this.bgwWorker.RunWorkerAsync(ReadablePassphraseGenerator.GitHubHomepage);            
        }
        private void lnkKeyBase_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Browse to my KeyBase page.
            this.bgwWorker.RunWorkerAsync(ReadablePassphraseGenerator.KeyBaseContact);
        }
        private void lnkPhraseHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Browse to doco page for phrases.
            this.bgwWorker.RunWorkerAsync(new Uri("https://github.com/ligos/readablepassphrasegenerator/wiki/Custom-Phrase-Description"));
        }
        private void lnkCombinationsHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Browse to doco page for combination counting.
            this.bgwWorker.RunWorkerAsync(new Uri("https://github.com/ligos/readablepassphrasegenerator/wiki/Combination-Counting"));
        }

        private void lnkDictionaryHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Browse to doco page for dictionary definition.
            this.bgwWorker.RunWorkerAsync(new Uri("https://github.com/ligos/readablepassphrasegenerator/wiki/Make-Your-Own-Dictionary"));
        }

        private void chkCustomDictionary_CheckedChanged(object sender, EventArgs e)
        {
            // If true, enable the textbox and browse button.
            this.btnBrowse.Enabled = this.chkCustomDictionary.Checked;
            this.txtDictionaryPath.Enabled = this.chkCustomDictionary.Checked;
            this.lnkDictionaryHelp.Visible = this.chkCustomDictionary.Checked;

            var conf = this.FormToConfigObject();
            if (conf != null)
            {
                // TODO: error handling.
                var dict = PassphraseGenerator.LoadDictionary(conf);
                this._Generator.SetDictionary(dict);
                this.UpdateCombinations(conf);
                this.lblStatus.Text = String.Format("Successfully loaded dictionary '{0}'.", System.IO.Path.GetFileName(this.ofdCustomDictionary.FileName));
            }
            if (this.chkCustomDictionary.Checked && System.IO.File.Exists(this.txtDictionaryPath.Text))
                this.lblStatus.Text = String.Format("Successfully loaded dictionary '{0}'.", System.IO.Path.GetFileName(this.txtDictionaryPath.Text));
            else
                this.lblStatus.Text = "Using default dictionary.";
        }
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (this.ofdCustomDictionary.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                this.txtDictionaryPath.Text = this.ofdCustomDictionary.FileName;
                var conf = this.FormToConfigObject();
                if (conf != null)
                {
                    // TODO: error handling.
                    var dict = PassphraseGenerator.LoadDictionary(conf);
                    this._Generator.SetDictionary(dict);
                    this.UpdateCombinations(conf);
                    this.lblStatus.Text = String.Format("Successfully loaded dictionary '{0}'.", System.IO.Path.GetFileName(this.ofdCustomDictionary.FileName));
                }
            }
        }
        private void btnDictionarySizeDetail_Click(object sender, EventArgs e)
        {
            using (var frm = new DictionarySizeDetail(this._Generator.Dictionary))
            {
                frm.ShowDialog(this);
            }
        }
        private void txtPhraseDescription_TextChanged(object sender, EventArgs e)
        {
            var conf = this.FormToConfigObject();
            if (conf != null)
                this.UpdateCombinations(conf);
        }



        private void ConfigObjectToForm(Config config)
        {
            _IsLoading = true;
            try
            {
                this.cboPhraseSelection.DataSource = Enum.GetNames(typeof(PhraseStrength));
                this.cboPhraseSelection.Text = config.PhraseStrength.ToString();
                this.cboWordSeparator.DataSource = Enum.GetNames(typeof(WordSeparatorOption)).Where(x => !String.Equals(x, "unknown", StringComparison.OrdinalIgnoreCase)).ToArray();
                this.cboWordSeparator.Text = config.WordSeparator.ToString();
                this.txtCustomSeparator.Text = config.CustomSeparator;
                this.chkCustomDictionary.Checked = config.UseCustomDictionary;
                this.txtDictionaryPath.Text = config.PathOfCustomDictionary;
                this.nudMinLength.Value = Math.Max(config.MinLength, (int)this.nudMinLength.Minimum);
                this.nudMaxLength.Value = Math.Min(config.MaxLength, (int)this.nudMaxLength.Maximum);

                this.radMutatorNone.Checked = config.Mutator == MutatorOption.None;
                this.radMutatorStandard.Checked = config.Mutator == MutatorOption.Standard;
                this.radMutatorCustom.Checked = config.Mutator == MutatorOption.Custom;
                this.cboUpperStyle.DataSource = Enum.GetNames(typeof(AllUppercaseStyles));
                this.cboUpperStyle.Text = config.UpperStyle.ToString();
                this.nudUpperCount.Value = config.UpperCount;
                this.cboNumericStyle.DataSource = Enum.GetNames(typeof(NumericStyles));
                this.cboNumericStyle.Text = config.NumericStyle.ToString();
                this.nudNumberCount.Value = config.NumericCount;
                this.cboConstantStyle.DataSource = Enum.GetNames(typeof(ConstantStyles));
                this.cboConstantStyle.Text = config.ConstantStyle.ToString();
                this.txtConstantValue.Text = config.ConstantValue;


                this.UpdateDescription(config);
                this.UpdateCombinations(config);
                this.UpdateDictionarySize(config);
                this.UpdateCustomStrengthVisibility(this.IsCurrentPhraseStrengthCustom);
                this.UpdateCustomDictionaryVisibility();
                this.UpdateCustomSeparatorEnabledStatus();
            }
            finally
            {
                _IsLoading = false;
            }
        }
        private void UpdateDescription(Config config)
        {
            if (config == null || MurrayGrant.ReadablePassphrase.PhraseDescription.Clause.RandomMappings.ContainsKey(config.PhraseStrength))
                this.txtPhraseDescription.Text = "";
            else
                this.txtPhraseDescription.Text = String.Join(Environment.NewLine, config.PhraseDescription.Select(c => c.ToTextString()).ToArray());
        }
        private void UpdateCombinations(Config config)
        {
            // TODO: this should take into account the mutators as well.
            PhraseCombinations combinations;
            if (config == null)
                combinations = PhraseCombinations.Zero;
            else if (MurrayGrant.ReadablePassphrase.PhraseDescription.Clause.RandomMappings.ContainsKey(config.PhraseStrength))
                combinations = this._Generator.CalculateCombinations(config.PhraseStrength);
            else
                combinations = this._Generator.CalculateCombinations(config.PhraseDescription);
            if (combinations.Shortest >= 0)
            {
                this.txtCombinationRange.Text = combinations.Shortest.ToString("E3") + " ‐ " + combinations.Longest.ToString("E3");
                this.txtEntropyRange.Text = combinations.ShortestAsEntropyBits.ToString("N2") + " ‐ " + combinations.LongestAsEntropyBits.ToString("N2");
                this.txtCombinationAverage.Text = "≈ " + combinations.OptionalAverage.ToString("N0");
                this.txtEntropyAverage.Text = "≈ " + combinations.OptionalAverageAsEntropyBits.ToString("N2");
            }
            else
            {
                this.txtCombinationRange.Text = "?";
                this.txtEntropyRange.Text = "?";
                this.txtCombinationAverage.Text = "?";
                this.txtEntropyAverage.Text = "?";
            }
        }
        private void UpdateDictionarySize(Config config)
        {
            var total = this._Generator.Dictionary.Count;
            this.txtDictionarySize.Text = total.ToString("N0");
        }
        private void UpdateCustomStrengthVisibility(bool isCustomSelected)
        {
            this.lblPhraseDetail.Visible = isCustomSelected;
            this.txtPhraseDescription.Visible = isCustomSelected;
            this.lnkPhraseHelp.Visible = isCustomSelected;

            if (isCustomSelected)
            {
                this.Height = this.txtDictionarySize.Bottom + this.btnOK.Height + 200;
            }
            else
            {
                this.Height = this.txtDictionarySize.Bottom + this.btnOK.Height + 70;
            }
        }
        private void UpdateCustomDictionaryVisibility()
        {
            this.lnkDictionaryHelp.Visible = this.chkCustomDictionary.Checked;
        }
        public void UpdateCustomMutatorControlEnabledStatus()
        {
            // Update the enabled / disabled status of custom mutator controls.
            this.cboUpperStyle.Enabled = this.radMutatorCustom.Checked;
            this.nudUpperCount.Enabled = this.radMutatorCustom.Checked;
            this.cboNumericStyle.Enabled = this.radMutatorCustom.Checked;
            this.nudNumberCount.Enabled = this.radMutatorCustom.Checked;
            this.cboConstantStyle.Enabled = this.radMutatorCustom.Checked;
            this.txtConstantValue.Enabled = this.radMutatorCustom.Checked;
        }
        public void UpdateCustomSeparatorEnabledStatus()
        {
            var sep = (WordSeparatorOption)Enum.Parse(typeof(WordSeparatorOption), this.cboWordSeparator.Text);
            if (sep == WordSeparatorOption.Custom)
            {
                this.txtCustomSeparator.Enabled = true;
            }
            else
            {
                this.txtCustomSeparator.Enabled = false;
                this.txtCustomSeparator.Text = "";
            }
        }

        private Config FormToConfigObject()
        {
            if (_IsLoading)
                return null;

            var result = new Config();
            result.PhraseStrength = (PhraseStrength)Enum.Parse(typeof(PhraseStrength), this.cboPhraseSelection.Text);
            result.WordSeparator = (WordSeparatorOption)Enum.Parse(typeof(WordSeparatorOption), this.cboWordSeparator.Text);
            result.CustomSeparator = this.txtCustomSeparator.Text;
            result.UseCustomDictionary = this.chkCustomDictionary.Checked;
            result.PathOfCustomDictionary = this.txtDictionaryPath.Text;
            try
            {
                if (result.PhraseStrength == PhraseStrength.Custom)
                    result.PhraseDescription = MurrayGrant.ReadablePassphrase.PhraseDescription.Clause.CreateCollectionFromTextString(this.txtPhraseDescription.Text);
                this.lblStatus.Text = "";
            }
            catch (MurrayGrant.ReadablePassphrase.PhraseDescription.PhraseDescriptionParseException ex)
            {
                result.PhraseDescription = new MurrayGrant.ReadablePassphrase.PhraseDescription.Clause[0];
                this.lblStatus.Text = ex.Message;
            }
            result.MinLength = (int)this.nudMinLength.Value;
            result.MaxLength = (int)this.nudMaxLength.Value;

            result.Mutator = this.radMutatorNone.Checked ? MutatorOption.None 
                                : this.radMutatorStandard.Checked ? MutatorOption.Standard
                                : this.radMutatorCustom.Checked ? MutatorOption.Custom
                                : MutatorOption.None;
            result.UpperStyle = (AllUppercaseStyles)Enum.Parse(typeof(AllUppercaseStyles), this.cboUpperStyle.Text);
            result.UpperCount = (int)this.nudUpperCount.Value;
            result.NumericStyle = (NumericStyles)Enum.Parse(typeof(NumericStyles), this.cboNumericStyle.Text);
            result.NumericCount = (int)this.nudNumberCount.Value;
            result.ConstantStyle = (ConstantStyles)Enum.Parse(typeof(ConstantStyles), this.cboConstantStyle.Text);
            result.ConstantValue = this.txtConstantValue.Text;
            return result;

        }

        private void bgwWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var url = e.Argument as Uri;
            if (url != null)
            {
                System.Diagnostics.Process.Start(url.ToString());
            }
        }

        private void txtPhraseDescription_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control == true && e.KeyCode == Keys.A)
            {
                // CTRL+A = Select All.
                this.txtPhraseDescription.SelectAll();
                e.Handled = true;
            }
        }
    }
}
