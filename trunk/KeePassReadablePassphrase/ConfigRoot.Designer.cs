﻿namespace KeePassReadablePassphrase
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
            this.cboPhraseSelection = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.txtPhraseDescription = new System.Windows.Forms.TextBox();
            this.txtDictionaryPath = new System.Windows.Forms.TextBox();
            this.chkCustomDictionary = new System.Windows.Forms.CheckBox();
            this.txtDictionarySize = new System.Windows.Forms.TextBox();
            this.txtCombinationRange = new System.Windows.Forms.TextBox();
            this.txtEntropyRange = new System.Windows.Forms.TextBox();
            this.txtEntropyAverage = new System.Windows.Forms.TextBox();
            this.txtCombinationAverage = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.radMutatorNone = new System.Windows.Forms.RadioButton();
            this.radMutatorStandard = new System.Windows.Forms.RadioButton();
            this.radMutatorCustom = new System.Windows.Forms.RadioButton();
            this.cboUpperStyle = new System.Windows.Forms.ComboBox();
            this.cboNumericStyle = new System.Windows.Forms.ComboBox();
            this.cboConstantStyle = new System.Windows.Forms.ComboBox();
            this.cboWordSeparator = new System.Windows.Forms.ComboBox();
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
            this.label2 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lnkCombinationsHelp = new System.Windows.Forms.LinkLabel();
            this.label10 = new System.Windows.Forms.Label();
            this.nudMinLength = new System.Windows.Forms.NumericUpDown();
            this.nudMaxLength = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.nudUpperCount = new System.Windows.Forms.NumericUpDown();
            this.nudNumberCount = new System.Windows.Forms.NumericUpDown();
            this.label17 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.label18 = new System.Windows.Forms.Label();
            this.txtCustomSeparator = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.txtConstantValue = new System.Windows.Forms.TextBox();
            this.lnkKeyBase = new System.Windows.Forms.LinkLabel();
            this.cboCountBy = new System.Windows.Forms.ComboBox();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudUpperCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNumberCount)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // cboPhraseSelection
            // 
            this.cboPhraseSelection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPhraseSelection.FormattingEnabled = true;
            this.cboPhraseSelection.Location = new System.Drawing.Point(104, 143);
            this.cboPhraseSelection.Name = "cboPhraseSelection";
            this.cboPhraseSelection.Size = new System.Drawing.Size(167, 21);
            this.cboPhraseSelection.TabIndex = 5;
            this.toolTip1.SetToolTip(this.cboPhraseSelection, "Determines the complexity of the generated phrases. \r\nStronger phrases have more " +
        "words and grammatical options.\r\nFor example, adding prepositions, adjectives and" +
        " additional verb tenses.");
            this.cboPhraseSelection.SelectedIndexChanged += new System.EventHandler(this.cboPhraseSelection_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 146);
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
            this.txtPhraseDescription.Location = new System.Drawing.Point(11, 442);
            this.txtPhraseDescription.Multiline = true;
            this.txtPhraseDescription.Name = "txtPhraseDescription";
            this.txtPhraseDescription.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtPhraseDescription.Size = new System.Drawing.Size(461, 109);
            this.txtPhraseDescription.TabIndex = 23;
            this.toolTip1.SetToolTip(this.txtPhraseDescription, resources.GetString("txtPhraseDescription.ToolTip"));
            this.txtPhraseDescription.TextChanged += new System.EventHandler(this.txtPhraseDescription_TextChanged);
            this.txtPhraseDescription.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtPhraseDescription_KeyUp);
            // 
            // txtDictionaryPath
            // 
            this.txtDictionaryPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDictionaryPath.Location = new System.Drawing.Point(104, 368);
            this.txtDictionaryPath.Name = "txtDictionaryPath";
            this.txtDictionaryPath.ReadOnly = true;
            this.txtDictionaryPath.Size = new System.Drawing.Size(339, 20);
            this.txtDictionaryPath.TabIndex = 19;
            this.toolTip1.SetToolTip(this.txtDictionaryPath, "The path and filename for your custom dictionary.");
            // 
            // chkCustomDictionary
            // 
            this.chkCustomDictionary.AutoSize = true;
            this.chkCustomDictionary.Location = new System.Drawing.Point(12, 345);
            this.chkCustomDictionary.Name = "chkCustomDictionary";
            this.chkCustomDictionary.Size = new System.Drawing.Size(145, 17);
            this.chkCustomDictionary.TabIndex = 8;
            this.chkCustomDictionary.Text = "Use Your Own Dictionary";
            this.toolTip1.SetToolTip(this.chkCustomDictionary, "If ticked, you select your own dictionary.\r\nIf unticked, an internal dictionary i" +
        "s used.");
            this.chkCustomDictionary.UseVisualStyleBackColor = true;
            this.chkCustomDictionary.CheckedChanged += new System.EventHandler(this.chkCustomDictionary_CheckedChanged);
            // 
            // txtDictionarySize
            // 
            this.txtDictionarySize.Location = new System.Drawing.Point(104, 394);
            this.txtDictionarySize.Name = "txtDictionarySize";
            this.txtDictionarySize.ReadOnly = true;
            this.txtDictionarySize.Size = new System.Drawing.Size(87, 20);
            this.txtDictionarySize.TabIndex = 21;
            this.toolTip1.SetToolTip(this.txtDictionarySize, "The total number of words in the selected dictionary.");
            // 
            // txtCombinationRange
            // 
            this.txtCombinationRange.Location = new System.Drawing.Point(104, 284);
            this.txtCombinationRange.Name = "txtCombinationRange";
            this.txtCombinationRange.ReadOnly = true;
            this.txtCombinationRange.Size = new System.Drawing.Size(220, 20);
            this.txtCombinationRange.TabIndex = 15;
            this.toolTip1.SetToolTip(this.txtCombinationRange, "The total combinations of words based on your selected dictionary and phrase stre" +
        "ngth.\r\nBigger is better (harder to guess)! But may also be harder to remember.");
            // 
            // txtEntropyRange
            // 
            this.txtEntropyRange.Location = new System.Drawing.Point(382, 284);
            this.txtEntropyRange.Name = "txtEntropyRange";
            this.txtEntropyRange.ReadOnly = true;
            this.txtEntropyRange.Size = new System.Drawing.Size(90, 20);
            this.txtEntropyRange.TabIndex = 16;
            this.toolTip1.SetToolTip(this.txtEntropyRange, "An estimate of the number of bits of entropy this passphrase contains, based on t" +
        "he number of combinations.\r\nYou can compare this number to the \"Quality\" field i" +
        "n KeePass.");
            // 
            // txtEntropyAverage
            // 
            this.txtEntropyAverage.Location = new System.Drawing.Point(382, 310);
            this.txtEntropyAverage.Name = "txtEntropyAverage";
            this.txtEntropyAverage.ReadOnly = true;
            this.txtEntropyAverage.Size = new System.Drawing.Size(90, 20);
            this.txtEntropyAverage.TabIndex = 18;
            this.toolTip1.SetToolTip(this.txtEntropyAverage, "An estimate of the number of bits of entropy this passphrase contains, based on t" +
        "he number of combinations.\r\nYou can compare this number to the \"Quality\" field i" +
        "n KeePass.");
            // 
            // txtCombinationAverage
            // 
            this.txtCombinationAverage.Location = new System.Drawing.Point(104, 310);
            this.txtCombinationAverage.Name = "txtCombinationAverage";
            this.txtCombinationAverage.ReadOnly = true;
            this.txtCombinationAverage.Size = new System.Drawing.Size(220, 20);
            this.txtCombinationAverage.TabIndex = 17;
            this.toolTip1.SetToolTip(this.txtCombinationAverage, "The total combinations of words based on your selected dictionary and phrase stre" +
        "ngth.\r\nBigger is better (harder to guess)! But may also be harder to remember.");
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(12, 180);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(51, 13);
            this.label13.TabIndex = 34;
            this.label13.Text = "Mutators:";
            this.toolTip1.SetToolTip(this.label13, "Mutators change the final passphrase");
            // 
            // radMutatorNone
            // 
            this.radMutatorNone.AutoSize = true;
            this.radMutatorNone.Location = new System.Drawing.Point(104, 176);
            this.radMutatorNone.Name = "radMutatorNone";
            this.radMutatorNone.Size = new System.Drawing.Size(51, 17);
            this.radMutatorNone.TabIndex = 6;
            this.radMutatorNone.TabStop = true;
            this.radMutatorNone.Text = "None";
            this.toolTip1.SetToolTip(this.radMutatorNone, "Makes no changes to the final passphrase");
            this.radMutatorNone.UseVisualStyleBackColor = true;
            this.radMutatorNone.CheckedChanged += new System.EventHandler(this.radMutator_CheckedChanged);
            // 
            // radMutatorStandard
            // 
            this.radMutatorStandard.AutoSize = true;
            this.radMutatorStandard.Location = new System.Drawing.Point(170, 176);
            this.radMutatorStandard.Name = "radMutatorStandard";
            this.radMutatorStandard.Size = new System.Drawing.Size(212, 17);
            this.radMutatorStandard.TabIndex = 7;
            this.radMutatorStandard.TabStop = true;
            this.radMutatorStandard.Text = "Standard (numbers, capitals and period)";
            this.toolTip1.SetToolTip(this.radMutatorStandard, "Capitalises one word, adds 2 numbers and appends a period to the final passphrase" +
        ". Use this to meet minimum password strength requirements.");
            this.radMutatorStandard.UseVisualStyleBackColor = true;
            this.radMutatorStandard.CheckedChanged += new System.EventHandler(this.radMutator_CheckedChanged);
            // 
            // radMutatorCustom
            // 
            this.radMutatorCustom.AutoSize = true;
            this.radMutatorCustom.Location = new System.Drawing.Point(393, 176);
            this.radMutatorCustom.Name = "radMutatorCustom";
            this.radMutatorCustom.Size = new System.Drawing.Size(60, 17);
            this.radMutatorCustom.TabIndex = 8;
            this.radMutatorCustom.TabStop = true;
            this.radMutatorCustom.Text = "Custom";
            this.toolTip1.SetToolTip(this.radMutatorCustom, "Lets you configure how numbers and capitals are added to the passphrase in detail" +
        "");
            this.radMutatorCustom.UseVisualStyleBackColor = true;
            this.radMutatorCustom.CheckedChanged += new System.EventHandler(this.radMutator_CheckedChanged);
            // 
            // cboUpperStyle
            // 
            this.cboUpperStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboUpperStyle.FormattingEnabled = true;
            this.cboUpperStyle.Location = new System.Drawing.Point(104, 196);
            this.cboUpperStyle.Name = "cboUpperStyle";
            this.cboUpperStyle.Size = new System.Drawing.Size(220, 21);
            this.cboUpperStyle.TabIndex = 9;
            this.toolTip1.SetToolTip(this.cboUpperStyle, "Determines when letters are converted to upper case.\r\nYou must set Mutators to Cu" +
        "stom for this option to be available.");
            this.cboUpperStyle.SelectedIndexChanged += new System.EventHandler(this.mutatorStyleOrCountChanged);
            // 
            // cboNumericStyle
            // 
            this.cboNumericStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboNumericStyle.FormattingEnabled = true;
            this.cboNumericStyle.Location = new System.Drawing.Point(104, 220);
            this.cboNumericStyle.Name = "cboNumericStyle";
            this.cboNumericStyle.Size = new System.Drawing.Size(220, 21);
            this.cboNumericStyle.TabIndex = 11;
            this.toolTip1.SetToolTip(this.cboNumericStyle, "Determines when letters are converted to upper case.\r\nYou must set Mutators to Cu" +
        "stom for this option to be available.");
            this.cboNumericStyle.SelectedIndexChanged += new System.EventHandler(this.mutatorStyleOrCountChanged);
            // 
            // cboConstantStyle
            // 
            this.cboConstantStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboConstantStyle.FormattingEnabled = true;
            this.cboConstantStyle.Location = new System.Drawing.Point(104, 244);
            this.cboConstantStyle.Name = "cboConstantStyle";
            this.cboConstantStyle.Size = new System.Drawing.Size(220, 21);
            this.cboConstantStyle.TabIndex = 13;
            this.toolTip1.SetToolTip(this.cboConstantStyle, "Determines when an additional constant string is added.\r\nYou must set Mutators to" +
        " Custom for this option to be available.");
            this.cboConstantStyle.SelectedIndexChanged += new System.EventHandler(this.mutatorStyleOrCountChanged);
            // 
            // cboWordSeparator
            // 
            this.cboWordSeparator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboWordSeparator.FormattingEnabled = true;
            this.cboWordSeparator.Location = new System.Drawing.Point(104, 118);
            this.cboWordSeparator.Name = "cboWordSeparator";
            this.cboWordSeparator.Size = new System.Drawing.Size(167, 21);
            this.cboWordSeparator.TabIndex = 3;
            this.toolTip1.SetToolTip(this.cboWordSeparator, "Determines the complexity of the generated phrases. \r\nStronger phrases have more " +
        "words and grammatical options.\r\nFor example, adding prepositions, adjectives and" +
        " additional verb tenses.");
            this.cboWordSeparator.SelectedIndexChanged += new System.EventHandler(this.cboWordSeparator_SelectedIndexChanged);
            // 
            // lblPhraseDetail
            // 
            this.lblPhraseDetail.AutoSize = true;
            this.lblPhraseDetail.Location = new System.Drawing.Point(12, 424);
            this.lblPhraseDetail.Name = "lblPhraseDetail";
            this.lblPhraseDetail.Size = new System.Drawing.Size(73, 13);
            this.lblPhraseDetail.TabIndex = 3;
            this.lblPhraseDetail.Text = "Phrase Detail:";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(397, 557);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 25;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(307, 557);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 24;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lnkPhraseHelp
            // 
            this.lnkPhraseHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkPhraseHelp.AutoSize = true;
            this.lnkPhraseHelp.Location = new System.Drawing.Point(318, 423);
            this.lnkPhraseHelp.Name = "lnkPhraseHelp";
            this.lnkPhraseHelp.Size = new System.Drawing.Size(155, 13);
            this.lnkPhraseHelp.TabIndex = 19;
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
            this.label4.Size = new System.Drawing.Size(361, 32);
            this.label4.TabIndex = 9;
            this.label4.Text = "This plugin generates passphrases which are grammatically correct (mostly), thoug" +
    "h non-sensical, from a dictionary of words. An example:";
            // 
            // lnkWebsite
            // 
            this.lnkWebsite.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkWebsite.AutoSize = true;
            this.lnkWebsite.Location = new System.Drawing.Point(385, 41);
            this.lnkWebsite.Name = "lnkWebsite";
            this.lnkWebsite.Size = new System.Drawing.Size(58, 13);
            this.lnkWebsite.TabIndex = 16;
            this.lnkWebsite.TabStop = true;
            this.lnkWebsite.Text = "Web Page";
            this.lnkWebsite.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkWebsite_LinkClicked);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Enabled = false;
            this.btnBrowse.Location = new System.Drawing.Point(449, 367);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(24, 23);
            this.btnBrowse.TabIndex = 20;
            this.btnBrowse.Text = "...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 372);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Dictionary:";
            // 
            // lnkDictionaryHelp
            // 
            this.lnkDictionaryHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkDictionaryHelp.AutoSize = true;
            this.lnkDictionaryHelp.Location = new System.Drawing.Point(298, 346);
            this.lnkDictionaryHelp.Name = "lnkDictionaryHelp";
            this.lnkDictionaryHelp.Size = new System.Drawing.Size(175, 13);
            this.lnkDictionaryHelp.TabIndex = 18;
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
            this.label6.Location = new System.Drawing.Point(12, 398);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(80, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "Dictionary Size:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 287);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(73, 13);
            this.label7.TabIndex = 18;
            this.label7.Text = "Combinations:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(330, 287);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(46, 13);
            this.label8.TabIndex = 20;
            this.label8.Text = "Entropy:";
            // 
            // btnDictionarySizeDetail
            // 
            this.btnDictionarySizeDetail.Location = new System.Drawing.Point(197, 392);
            this.btnDictionarySizeDetail.Name = "btnDictionarySizeDetail";
            this.btnDictionarySizeDetail.Size = new System.Drawing.Size(24, 23);
            this.btnDictionarySizeDetail.TabIndex = 22;
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
            this.statusStrip1.Location = new System.Drawing.Point(0, 589);
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
            this.lblVersion.Location = new System.Drawing.Point(386, 63);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(52, 13);
            this.lblVersion.TabIndex = 23;
            this.lblVersion.Text = "lblVersion";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(330, 313);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 27;
            this.label2.Text = "Entropy:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 313);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(72, 13);
            this.label9.TabIndex = 26;
            this.label9.Text = "Avg Comb\'ns:";
            // 
            // lnkCombinationsHelp
            // 
            this.lnkCombinationsHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkCombinationsHelp.AutoSize = true;
            this.lnkCombinationsHelp.Location = new System.Drawing.Point(344, 146);
            this.lnkCombinationsHelp.Name = "lnkCombinationsHelp";
            this.lnkCombinationsHelp.Size = new System.Drawing.Size(128, 13);
            this.lnkCombinationsHelp.TabIndex = 17;
            this.lnkCombinationsHelp.TabStop = true;
            this.lnkCombinationsHelp.Text = "More About Combinations";
            this.lnkCombinationsHelp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkCombinationsHelp_LinkClicked);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 94);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(157, 13);
            this.label10.TabIndex = 29;
            this.label10.Text = "Generate Passphrase Between:";
            // 
            // nudMinLength
            // 
            this.nudMinLength.Location = new System.Drawing.Point(175, 92);
            this.nudMinLength.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.nudMinLength.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudMinLength.Name = "nudMinLength";
            this.nudMinLength.Size = new System.Drawing.Size(56, 20);
            this.nudMinLength.TabIndex = 0;
            this.nudMinLength.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // nudMaxLength
            // 
            this.nudMaxLength.Location = new System.Drawing.Point(269, 91);
            this.nudMaxLength.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.nudMaxLength.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudMaxLength.Name = "nudMaxLength";
            this.nudMaxLength.Size = new System.Drawing.Size(55, 20);
            this.nudMaxLength.TabIndex = 1;
            this.nudMaxLength.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(237, 94);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(26, 13);
            this.label11.TabIndex = 32;
            this.label11.Text = "And";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(436, 93);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(31, 13);
            this.label12.TabIndex = 33;
            this.label12.Text = "Long";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(12, 199);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(92, 13);
            this.label14.TabIndex = 38;
            this.label14.Text = "Upper Case Style:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(12, 223);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(75, 13);
            this.label15.TabIndex = 39;
            this.label15.Text = "Numeric Style:";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(334, 199);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(70, 13);
            this.label16.TabIndex = 41;
            this.label16.Text = "Upper Count:";
            // 
            // nudUpperCount
            // 
            this.nudUpperCount.Location = new System.Drawing.Point(418, 197);
            this.nudUpperCount.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.nudUpperCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudUpperCount.Name = "nudUpperCount";
            this.nudUpperCount.Size = new System.Drawing.Size(55, 20);
            this.nudUpperCount.TabIndex = 10;
            this.nudUpperCount.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudUpperCount.ValueChanged += new System.EventHandler(this.mutatorStyleOrCountChanged);
            // 
            // nudNumberCount
            // 
            this.nudNumberCount.Location = new System.Drawing.Point(418, 221);
            this.nudNumberCount.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.nudNumberCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudNumberCount.Name = "nudNumberCount";
            this.nudNumberCount.Size = new System.Drawing.Size(55, 20);
            this.nudNumberCount.TabIndex = 12;
            this.nudNumberCount.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudNumberCount.ValueChanged += new System.EventHandler(this.mutatorStyleOrCountChanged);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(334, 223);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(78, 13);
            this.label17.TabIndex = 44;
            this.label17.Text = "Number Count:";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Location = new System.Drawing.Point(20, 169);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(450, 1);
            this.panel1.TabIndex = 46;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.Color.Black;
            this.panel2.Location = new System.Drawing.Point(0, 84);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(450, 1);
            this.panel2.TabIndex = 47;
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.BackColor = System.Drawing.Color.Black;
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Location = new System.Drawing.Point(17, 277);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(450, 1);
            this.panel3.TabIndex = 47;
            // 
            // panel4
            // 
            this.panel4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel4.BackColor = System.Drawing.Color.Black;
            this.panel4.Location = new System.Drawing.Point(0, 84);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(450, 1);
            this.panel4.TabIndex = 47;
            // 
            // panel5
            // 
            this.panel5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel5.BackColor = System.Drawing.Color.Black;
            this.panel5.Controls.Add(this.panel6);
            this.panel5.Location = new System.Drawing.Point(17, 339);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(450, 1);
            this.panel5.TabIndex = 48;
            // 
            // panel6
            // 
            this.panel6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel6.BackColor = System.Drawing.Color.Black;
            this.panel6.Location = new System.Drawing.Point(0, 84);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(450, 1);
            this.panel6.TabIndex = 47;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(12, 121);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(85, 13);
            this.label18.TabIndex = 49;
            this.label18.Text = "Word Separator:";
            // 
            // txtCustomSeparator
            // 
            this.txtCustomSeparator.Location = new System.Drawing.Point(382, 118);
            this.txtCustomSeparator.Name = "txtCustomSeparator";
            this.txtCustomSeparator.Size = new System.Drawing.Size(90, 20);
            this.txtCustomSeparator.TabIndex = 4;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(283, 122);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(94, 13);
            this.label19.TabIndex = 52;
            this.label19.Text = "Custom Separator:";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(14, 248);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(78, 13);
            this.label20.TabIndex = 39;
            this.label20.Text = "Constant Style:";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(334, 248);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(37, 13);
            this.label21.TabIndex = 44;
            this.label21.Text = "Value:";
            // 
            // txtConstantValue
            // 
            this.txtConstantValue.Location = new System.Drawing.Point(382, 245);
            this.txtConstantValue.Name = "txtConstantValue";
            this.txtConstantValue.Size = new System.Drawing.Size(90, 20);
            this.txtConstantValue.TabIndex = 14;
            this.txtConstantValue.Text = ".";
            // 
            // lnkKeyBase
            // 
            this.lnkKeyBase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkKeyBase.AutoSize = true;
            this.lnkKeyBase.Location = new System.Drawing.Point(384, 18);
            this.lnkKeyBase.Name = "lnkKeyBase";
            this.lnkKeyBase.Size = new System.Drawing.Size(89, 13);
            this.lnkKeyBase.TabIndex = 53;
            this.lnkKeyBase.TabStop = true;
            this.lnkKeyBase.Text = "KeyBase Contact";
            this.lnkKeyBase.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkKeyBase_LinkClicked);
            // 
            // cboCountBy
            // 
            this.cboCountBy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCountBy.FormattingEnabled = true;
            this.cboCountBy.Items.AddRange(new object[] {
            "Letters",
            "Words"});
            this.cboCountBy.Location = new System.Drawing.Point(337, 90);
            this.cboCountBy.Name = "cboCountBy";
            this.cboCountBy.Size = new System.Drawing.Size(93, 21);
            this.cboCountBy.TabIndex = 2;
            // 
            // ConfigRoot
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(485, 611);
            this.Controls.Add(this.cboCountBy);
            this.Controls.Add(this.lnkKeyBase);
            this.Controls.Add(this.txtConstantValue);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.txtCustomSeparator);
            this.Controls.Add(this.cboWordSeparator);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.nudNumberCount);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.cboNumericStyle);
            this.Controls.Add(this.cboConstantStyle);
            this.Controls.Add(this.nudUpperCount);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.cboUpperStyle);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.radMutatorCustom);
            this.Controls.Add(this.radMutatorStandard);
            this.Controls.Add(this.radMutatorNone);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.nudMaxLength);
            this.Controls.Add(this.nudMinLength);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.lnkCombinationsHelp);
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
            this.Name = "ConfigRoot";
            this.Text = "Readable Passphrase Configuration";
            this.Load += new System.EventHandler(this.ConfigRoot_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinLength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxLength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudUpperCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNumberCount)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
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
        private System.Windows.Forms.LinkLabel lnkCombinationsHelp;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown nudMinLength;
        private System.Windows.Forms.NumericUpDown nudMaxLength;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.RadioButton radMutatorNone;
        private System.Windows.Forms.RadioButton radMutatorStandard;
        private System.Windows.Forms.RadioButton radMutatorCustom;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ComboBox cboUpperStyle;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.NumericUpDown nudUpperCount;
        private System.Windows.Forms.NumericUpDown nudNumberCount;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.ComboBox cboNumericStyle;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.ComboBox cboWordSeparator;
        private System.Windows.Forms.TextBox txtCustomSeparator;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.LinkLabel lnkKeyBase;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.ComboBox cboConstantStyle;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox txtConstantValue;
        private System.Windows.Forms.ComboBox cboCountBy;
    }
}