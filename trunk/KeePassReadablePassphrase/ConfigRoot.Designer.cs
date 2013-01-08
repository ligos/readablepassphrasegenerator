namespace KeePassReadablePassphrase
{
    partial class ConfigRoot
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigRoot));
            this.chkSpacesBetweenWords = new System.Windows.Forms.CheckBox();
            this.cboPhraseSelection = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.txtPhraseDescription = new System.Windows.Forms.TextBox();
            this.txtDictionaryPath = new System.Windows.Forms.TextBox();
            this.chkCustomDictionary = new System.Windows.Forms.CheckBox();
            this.txtDictionarySize = new System.Windows.Forms.TextBox();
            this.txtCombinationRange = new System.Windows.Forms.TextBox();
            this.txtEntropyRange = new System.Windows.Forms.TextBox();
            this.lblPhraseDetail = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.lnkPhraseHelp = new System.Windows.Forms.LinkLabel();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lnkWebsite = new System.Windows.Forms.LinkLabel();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.lnkDictionaryHelp = new System.Windows.Forms.LinkLabel();
            this.bgwWorker = new System.ComponentModel.BackgroundWorker();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.btnDictionarySizeDetail = new System.Windows.Forms.Button();
            this.ofdCustomDictionary = new System.Windows.Forms.OpenFileDialog();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblVersion = new System.Windows.Forms.Label();
            this.txtEntropyAverage = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtCombinationAverage = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkSpacesBetweenWords
            // 
            this.chkSpacesBetweenWords.AutoSize = true;
            this.chkSpacesBetweenWords.Location = new System.Drawing.Point(12, 72);
            this.chkSpacesBetweenWords.Name = "chkSpacesBetweenWords";
            this.chkSpacesBetweenWords.Size = new System.Drawing.Size(179, 17);
            this.chkSpacesBetweenWords.TabIndex = 0;
            this.chkSpacesBetweenWords.Text = "Include Spaces Between Words";
            this.toolTip1.SetToolTip(this.chkSpacesBetweenWords, "If ticked, spaces are added to the phrase as a normal sentence.\r\nIf unticked, all" +
        " the words are jammed up against each other.");
            this.chkSpacesBetweenWords.UseVisualStyleBackColor = true;
            // 
            // cboPhraseSelection
            // 
            this.cboPhraseSelection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPhraseSelection.FormattingEnabled = true;
            this.cboPhraseSelection.Location = new System.Drawing.Point(104, 95);
            this.cboPhraseSelection.Name = "cboPhraseSelection";
            this.cboPhraseSelection.Size = new System.Drawing.Size(167, 21);
            this.cboPhraseSelection.TabIndex = 1;
            this.toolTip1.SetToolTip(this.cboPhraseSelection, "Determines the complexity of the generated phrases. \r\nStronger phrases have more " +
        "words and grammatical options.\r\nFor example, adding prepositions, adjectives and" +
        " additional verb tenses.");
            this.cboPhraseSelection.SelectedIndexChanged += new System.EventHandler(this.cboPhraseSelection_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 98);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Phrase Strength:";
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 12000;
            this.toolTip1.InitialDelay = 500;
            this.toolTip1.ReshowDelay = 100;
            // 
            // txtPhraseDescription
            // 
            this.txtPhraseDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPhraseDescription.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPhraseDescription.Location = new System.Drawing.Point(11, 269);
            this.txtPhraseDescription.Multiline = true;
            this.txtPhraseDescription.Name = "txtPhraseDescription";
            this.txtPhraseDescription.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtPhraseDescription.Size = new System.Drawing.Size(461, 102);
            this.txtPhraseDescription.TabIndex = 9;
            this.toolTip1.SetToolTip(this.txtPhraseDescription, resources.GetString("txtPhraseDescription.ToolTip"));
            this.txtPhraseDescription.TextChanged += new System.EventHandler(this.txtPhraseDescription_TextChanged);
            // 
            // txtDictionaryPath
            // 
            this.txtDictionaryPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDictionaryPath.Location = new System.Drawing.Point(104, 197);
            this.txtDictionaryPath.Name = "txtDictionaryPath";
            this.txtDictionaryPath.ReadOnly = true;
            this.txtDictionaryPath.Size = new System.Drawing.Size(339, 20);
            this.txtDictionaryPath.TabIndex = 6;
            this.toolTip1.SetToolTip(this.txtDictionaryPath, "The path and filename for your custom dictionary.");
            // 
            // chkCustomDictionary
            // 
            this.chkCustomDictionary.AutoSize = true;
            this.chkCustomDictionary.Location = new System.Drawing.Point(12, 174);
            this.chkCustomDictionary.Name = "chkCustomDictionary";
            this.chkCustomDictionary.Size = new System.Drawing.Size(145, 17);
            this.chkCustomDictionary.TabIndex = 4;
            this.chkCustomDictionary.Text = "Use Your Own Dictionary";
            this.toolTip1.SetToolTip(this.chkCustomDictionary, "If ticked, you select your own dictionary.\r\nIf unticked, an internal dictionary i" +
        "s used.");
            this.chkCustomDictionary.UseVisualStyleBackColor = true;
            this.chkCustomDictionary.CheckedChanged += new System.EventHandler(this.chkCustomDictionary_CheckedChanged);
            // 
            // txtDictionarySize
            // 
            this.txtDictionarySize.Location = new System.Drawing.Point(104, 223);
            this.txtDictionarySize.Name = "txtDictionarySize";
            this.txtDictionarySize.ReadOnly = true;
            this.txtDictionarySize.Size = new System.Drawing.Size(87, 20);
            this.txtDictionarySize.TabIndex = 8;
            this.toolTip1.SetToolTip(this.txtDictionarySize, "The total number of words in the selected dictionary.");
            // 
            // txtCombinationRange
            // 
            this.txtCombinationRange.Location = new System.Drawing.Point(104, 122);
            this.txtCombinationRange.Name = "txtCombinationRange";
            this.txtCombinationRange.ReadOnly = true;
            this.txtCombinationRange.Size = new System.Drawing.Size(220, 20);
            this.txtCombinationRange.TabIndex = 2;
            this.toolTip1.SetToolTip(this.txtCombinationRange, "The total combinations of words based on your selected dictionary and phrase stre" +
        "ngth.\r\nBigger is better (harder to guess)! But may also be harder to remember.");
            // 
            // txtEntropyRange
            // 
            this.txtEntropyRange.Location = new System.Drawing.Point(382, 122);
            this.txtEntropyRange.Name = "txtEntropyRange";
            this.txtEntropyRange.ReadOnly = true;
            this.txtEntropyRange.Size = new System.Drawing.Size(90, 20);
            this.txtEntropyRange.TabIndex = 3;
            this.toolTip1.SetToolTip(this.txtEntropyRange, "An estimate of the number of bits of entropy this passphrase contains, based on t" +
        "he number of combinations.\r\nYou can compare this number to the \"Quality\" field i" +
        "n KeePass.");
            // 
            // lblPhraseDetail
            // 
            this.lblPhraseDetail.AutoSize = true;
            this.lblPhraseDetail.Location = new System.Drawing.Point(12, 253);
            this.lblPhraseDetail.Name = "lblPhraseDetail";
            this.lblPhraseDetail.Size = new System.Drawing.Size(73, 13);
            this.lblPhraseDetail.TabIndex = 3;
            this.lblPhraseDetail.Text = "Phrase Detail:";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(397, 377);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(307, 377);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 10;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lnkPhraseHelp
            // 
            this.lnkPhraseHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkPhraseHelp.AutoSize = true;
            this.lnkPhraseHelp.Location = new System.Drawing.Point(318, 252);
            this.lnkPhraseHelp.Name = "lnkPhraseHelp";
            this.lnkPhraseHelp.Size = new System.Drawing.Size(155, 13);
            this.lnkPhraseHelp.TabIndex = 14;
            this.lnkPhraseHelp.TabStop = true;
            this.lnkPhraseHelp.Text = "How to Make your Own Phrase";
            this.lnkPhraseHelp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkPhraseHelp_LinkClicked);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(56, 41);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(227, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "the statesman will burgle amidst lucid sunlamps";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(12, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(446, 32);
            this.label4.TabIndex = 9;
            this.label4.Text = "This plugin generates passphrases which are grammatically correct (mostly), thoug" +
    "h non-sensical, from a dictionary of words. An example:";
            // 
            // lnkWebsite
            // 
            this.lnkWebsite.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkWebsite.AutoSize = true;
            this.lnkWebsite.Location = new System.Drawing.Point(394, 41);
            this.lnkWebsite.Name = "lnkWebsite";
            this.lnkWebsite.Size = new System.Drawing.Size(79, 13);
            this.lnkWebsite.TabIndex = 12;
            this.lnkWebsite.TabStop = true;
            this.lnkWebsite.Text = "Codeplex Page";
            this.lnkWebsite.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkWebsite_LinkClicked);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Enabled = false;
            this.btnBrowse.Location = new System.Drawing.Point(449, 194);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(24, 23);
            this.btnBrowse.TabIndex = 7;
            this.btnBrowse.Text = "...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 201);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Dictionary:";
            // 
            // lnkDictionaryHelp
            // 
            this.lnkDictionaryHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkDictionaryHelp.AutoSize = true;
            this.lnkDictionaryHelp.Location = new System.Drawing.Point(298, 175);
            this.lnkDictionaryHelp.Name = "lnkDictionaryHelp";
            this.lnkDictionaryHelp.Size = new System.Drawing.Size(175, 13);
            this.lnkDictionaryHelp.TabIndex = 13;
            this.lnkDictionaryHelp.TabStop = true;
            this.lnkDictionaryHelp.Text = "How To Make Your Own Dictionary";
            this.lnkDictionaryHelp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkDictionaryHelp_LinkClicked);
            // 
            // bgwWorker
            // 
            this.bgwWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwWorker_DoWork);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 227);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(80, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "Dictionary Size:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 125);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(73, 13);
            this.label7.TabIndex = 18;
            this.label7.Text = "Combinations:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(330, 125);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(46, 13);
            this.label8.TabIndex = 20;
            this.label8.Text = "Entropy:";
            // 
            // btnDictionarySizeDetail
            // 
            this.btnDictionarySizeDetail.Location = new System.Drawing.Point(197, 221);
            this.btnDictionarySizeDetail.Name = "btnDictionarySizeDetail";
            this.btnDictionarySizeDetail.Size = new System.Drawing.Size(24, 23);
            this.btnDictionarySizeDetail.TabIndex = 21;
            this.btnDictionarySizeDetail.Text = "...";
            this.btnDictionarySizeDetail.UseVisualStyleBackColor = true;
            this.btnDictionarySizeDetail.Click += new System.EventHandler(this.btnDictionarySizeDetail_Click);
            // 
            // ofdCustomDictionary
            // 
            this.ofdCustomDictionary.DefaultExt = "xml";
            this.ofdCustomDictionary.Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*";
            this.ofdCustomDictionary.Title = "Select a Custom Dictionary XML File";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 409);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(485, 22);
            this.statusStrip1.TabIndex = 22;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.lblStatus.Size = new System.Drawing.Size(66, 17);
            this.lblStatus.Text = "status label";
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Location = new System.Drawing.Point(394, 63);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(52, 13);
            this.lblVersion.TabIndex = 23;
            this.lblVersion.Text = "lblVersion";
            // 
            // txtEntropyAverage
            // 
            this.txtEntropyAverage.Location = new System.Drawing.Point(382, 148);
            this.txtEntropyAverage.Name = "txtEntropyAverage";
            this.txtEntropyAverage.ReadOnly = true;
            this.txtEntropyAverage.Size = new System.Drawing.Size(90, 20);
            this.txtEntropyAverage.TabIndex = 5;
            this.toolTip1.SetToolTip(this.txtEntropyAverage, "An estimate of the number of bits of entropy this passphrase contains, based on t" +
        "he number of combinations.\r\nYou can compare this number to the \"Quality\" field i" +
        "n KeePass.");
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(330, 151);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 27;
            this.label2.Text = "Entropy:";
            // 
            // txtCombinationAverage
            // 
            this.txtCombinationAverage.Location = new System.Drawing.Point(104, 148);
            this.txtCombinationAverage.Name = "txtCombinationAverage";
            this.txtCombinationAverage.ReadOnly = true;
            this.txtCombinationAverage.Size = new System.Drawing.Size(220, 20);
            this.txtCombinationAverage.TabIndex = 4;
            this.toolTip1.SetToolTip(this.txtCombinationAverage, "The total combinations of words based on your selected dictionary and phrase stre" +
        "ngth.\r\nBigger is better (harder to guess)! But may also be harder to remember.");
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 151);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(59, 13);
            this.label9.TabIndex = 26;
            this.label9.Text = "Avg Comb:";
            // 
            // ConfigRoot
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(485, 431);
            this.Controls.Add(this.txtEntropyAverage);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtCombinationAverage);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnDictionarySizeDetail);
            this.Controls.Add(this.txtEntropyRange);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtCombinationRange);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtDictionarySize);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.chkCustomDictionary);
            this.Controls.Add(this.lnkDictionaryHelp);
            this.Controls.Add(this.txtDictionaryPath);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.lnkWebsite);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lnkPhraseHelp);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.txtPhraseDescription);
            this.Controls.Add(this.lblPhraseDetail);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cboPhraseSelection);
            this.Controls.Add(this.chkSpacesBetweenWords);
            this.Name = "ConfigRoot";
            this.Text = "Readable Passphrase Configuration";
            this.Load += new System.EventHandler(this.ConfigRoot_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkSpacesBetweenWords;
        private System.Windows.Forms.ComboBox cboPhraseSelection;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label lblPhraseDetail;
        private System.Windows.Forms.TextBox txtPhraseDescription;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.LinkLabel lnkPhraseHelp;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.LinkLabel lnkWebsite;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtDictionaryPath;
        private System.Windows.Forms.LinkLabel lnkDictionaryHelp;
        private System.ComponentModel.BackgroundWorker bgwWorker;
        private System.Windows.Forms.CheckBox chkCustomDictionary;
        private System.Windows.Forms.TextBox txtDictionarySize;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtCombinationRange;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtEntropyRange;
        private System.Windows.Forms.Button btnDictionarySizeDetail;
        private System.Windows.Forms.OpenFileDialog ofdCustomDictionary;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.TextBox txtEntropyAverage;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtCombinationAverage;
        private System.Windows.Forms.Label label9;
    }
}