namespace Simple_Bible_Reader
{
    partial class PreferencesFrm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreferencesFrm));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabBible = new System.Windows.Forms.TabPage();
            this.cbDisplayBIBRawView = new System.Windows.Forms.CheckBox();
            this.cbBibleRedText = new System.Windows.Forms.CheckBox();
            this.cbBibleCmtryIcon = new System.Windows.Forms.CheckBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.cbVersification = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.rbCrossWireLib = new System.Windows.Forms.RadioButton();
            this.rbCrossConnectLib = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbAutoSetBible = new System.Windows.Forms.CheckBox();
            this.cbBibleStripHtml = new System.Windows.Forms.CheckBox();
            this.cbBibleHtmlToRtf = new System.Windows.Forms.CheckBox();
            this.cbBibleRtfToHtml = new System.Windows.Forms.CheckBox();
            this.rbBibleRaw = new System.Windows.Forms.RadioButton();
            this.rbBibleParse = new System.Windows.Forms.RadioButton();
            this.cbBibleBkgndCleaning = new System.Windows.Forms.CheckBox();
            this.tabBook = new System.Windows.Forms.TabPage();
            this.cbDisplayBookRaw = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cbAutoSetBook = new System.Windows.Forms.CheckBox();
            this.cbBookStripHtml = new System.Windows.Forms.CheckBox();
            this.cbBookHtmlToRtf = new System.Windows.Forms.CheckBox();
            this.cbBookRtfToHtml = new System.Windows.Forms.CheckBox();
            this.rbBookRaw = new System.Windows.Forms.RadioButton();
            this.rbBookParse = new System.Windows.Forms.RadioButton();
            this.cbBookBkgndCleaning = new System.Windows.Forms.CheckBox();
            this.tabCommentary = new System.Windows.Forms.TabPage();
            this.cbDisplayCmtryRaw = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbAutoSetCmtry = new System.Windows.Forms.CheckBox();
            this.cbCmtryHtmlToRtf = new System.Windows.Forms.CheckBox();
            this.cbCmtryStripHtml = new System.Windows.Forms.CheckBox();
            this.cbCmtryRtfToHtml = new System.Windows.Forms.CheckBox();
            this.rbCmtryRaw = new System.Windows.Forms.RadioButton();
            this.rbCmtryParse = new System.Windows.Forms.RadioButton();
            this.tabDictionary = new System.Windows.Forms.TabPage();
            this.cbDisplayDictRaw = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cbAutoSetDict = new System.Windows.Forms.CheckBox();
            this.cbDictHtmlToRtf = new System.Windows.Forms.CheckBox();
            this.cbDictStripHtml = new System.Windows.Forms.CheckBox();
            this.cbDictRtfToHtml = new System.Windows.Forms.CheckBox();
            this.rbDictRaw = new System.Windows.Forms.RadioButton();
            this.rbDictParse = new System.Windows.Forms.RadioButton();
            this.tabMP3 = new System.Windows.Forms.TabPage();
            this.button2 = new System.Windows.Forms.Button();
            this.cbMp3Transliterate = new System.Windows.Forms.CheckBox();
            this.tbMp3Text = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.cbMp3Voices = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbMP3Speed = new System.Windows.Forms.TrackBar();
            this.tbMp3Volume = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.tabExperimental = new System.Windows.Forms.TabPage();
            this.cbHtmlRenderer = new System.Windows.Forms.ComboBox();
            this.cbUseRenderer = new System.Windows.Forms.CheckBox();
            this.cbUseCtrlHtmlRtfConv = new System.Windows.Forms.CheckBox();
            this.cbSaveLoadFilePref = new System.Windows.Forms.CheckBox();
            this.cbEditBibleVerse = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.OK = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabBible.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabBook.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tabCommentary.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabDictionary.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tabMP3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbMP3Speed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbMp3Volume)).BeginInit();
            this.tabExperimental.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.tabControl1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabBible);
            this.tabControl1.Controls.Add(this.tabBook);
            this.tabControl1.Controls.Add(this.tabCommentary);
            this.tabControl1.Controls.Add(this.tabDictionary);
            this.tabControl1.Controls.Add(this.tabMP3);
            this.tabControl1.Controls.Add(this.tabExperimental);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // tabBible
            // 
            this.tabBible.BackColor = System.Drawing.Color.Transparent;
            this.tabBible.Controls.Add(this.cbDisplayBIBRawView);
            this.tabBible.Controls.Add(this.cbBibleRedText);
            this.tabBible.Controls.Add(this.cbBibleCmtryIcon);
            this.tabBible.Controls.Add(this.groupBox5);
            this.tabBible.Controls.Add(this.groupBox1);
            resources.ApplyResources(this.tabBible, "tabBible");
            this.tabBible.Name = "tabBible";
            // 
            // cbDisplayBIBRawView
            // 
            resources.ApplyResources(this.cbDisplayBIBRawView, "cbDisplayBIBRawView");
            this.cbDisplayBIBRawView.Name = "cbDisplayBIBRawView";
            this.cbDisplayBIBRawView.UseVisualStyleBackColor = true;
            // 
            // cbBibleRedText
            // 
            resources.ApplyResources(this.cbBibleRedText, "cbBibleRedText");
            this.cbBibleRedText.Name = "cbBibleRedText";
            this.cbBibleRedText.UseVisualStyleBackColor = true;
            // 
            // cbBibleCmtryIcon
            // 
            resources.ApplyResources(this.cbBibleCmtryIcon, "cbBibleCmtryIcon");
            this.cbBibleCmtryIcon.Checked = true;
            this.cbBibleCmtryIcon.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbBibleCmtryIcon.Name = "cbBibleCmtryIcon";
            this.cbBibleCmtryIcon.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label10);
            this.groupBox5.Controls.Add(this.cbVersification);
            this.groupBox5.Controls.Add(this.label9);
            this.groupBox5.Controls.Add(this.rbCrossWireLib);
            this.groupBox5.Controls.Add(this.rbCrossConnectLib);
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // cbVersification
            // 
            this.cbVersification.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbVersification.FormattingEnabled = true;
            resources.ApplyResources(this.cbVersification, "cbVersification");
            this.cbVersification.Name = "cbVersification";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.ForeColor = System.Drawing.Color.Red;
            this.label9.Name = "label9";
            // 
            // rbCrossWireLib
            // 
            resources.ApplyResources(this.rbCrossWireLib, "rbCrossWireLib");
            this.rbCrossWireLib.Checked = true;
            this.rbCrossWireLib.Name = "rbCrossWireLib";
            this.rbCrossWireLib.TabStop = true;
            this.rbCrossWireLib.UseVisualStyleBackColor = true;
            this.rbCrossWireLib.CheckedChanged += new System.EventHandler(this.rbCrossWireLib_CheckedChanged);
            // 
            // rbCrossConnectLib
            // 
            resources.ApplyResources(this.rbCrossConnectLib, "rbCrossConnectLib");
            this.rbCrossConnectLib.Name = "rbCrossConnectLib";
            this.rbCrossConnectLib.UseVisualStyleBackColor = true;
            this.rbCrossConnectLib.CheckedChanged += new System.EventHandler(this.rbCrossConnectLib_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbAutoSetBible);
            this.groupBox1.Controls.Add(this.cbBibleStripHtml);
            this.groupBox1.Controls.Add(this.cbBibleHtmlToRtf);
            this.groupBox1.Controls.Add(this.cbBibleRtfToHtml);
            this.groupBox1.Controls.Add(this.rbBibleRaw);
            this.groupBox1.Controls.Add(this.rbBibleParse);
            this.groupBox1.Controls.Add(this.cbBibleBkgndCleaning);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // cbAutoSetBible
            // 
            resources.ApplyResources(this.cbAutoSetBible, "cbAutoSetBible");
            this.cbAutoSetBible.Checked = true;
            this.cbAutoSetBible.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAutoSetBible.Name = "cbAutoSetBible";
            this.cbAutoSetBible.UseVisualStyleBackColor = true;
            this.cbAutoSetBible.CheckedChanged += new System.EventHandler(this.cbAutoSetBible_CheckedChanged);
            // 
            // cbBibleStripHtml
            // 
            resources.ApplyResources(this.cbBibleStripHtml, "cbBibleStripHtml");
            this.cbBibleStripHtml.Name = "cbBibleStripHtml";
            this.cbBibleStripHtml.UseVisualStyleBackColor = true;
            // 
            // cbBibleHtmlToRtf
            // 
            resources.ApplyResources(this.cbBibleHtmlToRtf, "cbBibleHtmlToRtf");
            this.cbBibleHtmlToRtf.Name = "cbBibleHtmlToRtf";
            this.cbBibleHtmlToRtf.UseVisualStyleBackColor = true;
            // 
            // cbBibleRtfToHtml
            // 
            resources.ApplyResources(this.cbBibleRtfToHtml, "cbBibleRtfToHtml");
            this.cbBibleRtfToHtml.Name = "cbBibleRtfToHtml";
            this.cbBibleRtfToHtml.UseVisualStyleBackColor = true;
            // 
            // rbBibleRaw
            // 
            resources.ApplyResources(this.rbBibleRaw, "rbBibleRaw");
            this.rbBibleRaw.Name = "rbBibleRaw";
            this.rbBibleRaw.TabStop = true;
            this.rbBibleRaw.UseVisualStyleBackColor = true;
            this.rbBibleRaw.CheckedChanged += new System.EventHandler(this.rbBibleRaw_CheckedChanged);
            // 
            // rbBibleParse
            // 
            resources.ApplyResources(this.rbBibleParse, "rbBibleParse");
            this.rbBibleParse.Checked = true;
            this.rbBibleParse.Name = "rbBibleParse";
            this.rbBibleParse.TabStop = true;
            this.rbBibleParse.UseVisualStyleBackColor = true;
            this.rbBibleParse.CheckedChanged += new System.EventHandler(this.rbBibleParse_CheckedChanged);
            // 
            // cbBibleBkgndCleaning
            // 
            resources.ApplyResources(this.cbBibleBkgndCleaning, "cbBibleBkgndCleaning");
            this.cbBibleBkgndCleaning.Checked = true;
            this.cbBibleBkgndCleaning.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbBibleBkgndCleaning.Name = "cbBibleBkgndCleaning";
            this.cbBibleBkgndCleaning.UseVisualStyleBackColor = true;
            // 
            // tabBook
            // 
            this.tabBook.Controls.Add(this.cbDisplayBookRaw);
            this.tabBook.Controls.Add(this.groupBox4);
            resources.ApplyResources(this.tabBook, "tabBook");
            this.tabBook.Name = "tabBook";
            this.tabBook.UseVisualStyleBackColor = true;
            // 
            // cbDisplayBookRaw
            // 
            resources.ApplyResources(this.cbDisplayBookRaw, "cbDisplayBookRaw");
            this.cbDisplayBookRaw.Name = "cbDisplayBookRaw";
            this.cbDisplayBookRaw.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cbAutoSetBook);
            this.groupBox4.Controls.Add(this.cbBookStripHtml);
            this.groupBox4.Controls.Add(this.cbBookHtmlToRtf);
            this.groupBox4.Controls.Add(this.cbBookRtfToHtml);
            this.groupBox4.Controls.Add(this.rbBookRaw);
            this.groupBox4.Controls.Add(this.rbBookParse);
            this.groupBox4.Controls.Add(this.cbBookBkgndCleaning);
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // cbAutoSetBook
            // 
            resources.ApplyResources(this.cbAutoSetBook, "cbAutoSetBook");
            this.cbAutoSetBook.Checked = true;
            this.cbAutoSetBook.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAutoSetBook.Name = "cbAutoSetBook";
            this.cbAutoSetBook.UseVisualStyleBackColor = true;
            this.cbAutoSetBook.CheckedChanged += new System.EventHandler(this.cbAutoSetBook_CheckedChanged);
            // 
            // cbBookStripHtml
            // 
            resources.ApplyResources(this.cbBookStripHtml, "cbBookStripHtml");
            this.cbBookStripHtml.Name = "cbBookStripHtml";
            this.cbBookStripHtml.UseVisualStyleBackColor = true;
            // 
            // cbBookHtmlToRtf
            // 
            resources.ApplyResources(this.cbBookHtmlToRtf, "cbBookHtmlToRtf");
            this.cbBookHtmlToRtf.Name = "cbBookHtmlToRtf";
            this.cbBookHtmlToRtf.UseVisualStyleBackColor = true;
            // 
            // cbBookRtfToHtml
            // 
            resources.ApplyResources(this.cbBookRtfToHtml, "cbBookRtfToHtml");
            this.cbBookRtfToHtml.Name = "cbBookRtfToHtml";
            this.cbBookRtfToHtml.UseVisualStyleBackColor = true;
            // 
            // rbBookRaw
            // 
            resources.ApplyResources(this.rbBookRaw, "rbBookRaw");
            this.rbBookRaw.Name = "rbBookRaw";
            this.rbBookRaw.TabStop = true;
            this.rbBookRaw.UseVisualStyleBackColor = true;
            this.rbBookRaw.CheckedChanged += new System.EventHandler(this.rbBookRaw_CheckedChanged);
            // 
            // rbBookParse
            // 
            resources.ApplyResources(this.rbBookParse, "rbBookParse");
            this.rbBookParse.Checked = true;
            this.rbBookParse.Name = "rbBookParse";
            this.rbBookParse.TabStop = true;
            this.rbBookParse.UseVisualStyleBackColor = true;
            this.rbBookParse.CheckedChanged += new System.EventHandler(this.rbBookParse_CheckedChanged);
            // 
            // cbBookBkgndCleaning
            // 
            resources.ApplyResources(this.cbBookBkgndCleaning, "cbBookBkgndCleaning");
            this.cbBookBkgndCleaning.Checked = true;
            this.cbBookBkgndCleaning.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbBookBkgndCleaning.Name = "cbBookBkgndCleaning";
            this.cbBookBkgndCleaning.UseVisualStyleBackColor = true;
            // 
            // tabCommentary
            // 
            this.tabCommentary.Controls.Add(this.cbDisplayCmtryRaw);
            this.tabCommentary.Controls.Add(this.groupBox2);
            resources.ApplyResources(this.tabCommentary, "tabCommentary");
            this.tabCommentary.Name = "tabCommentary";
            this.tabCommentary.UseVisualStyleBackColor = true;
            // 
            // cbDisplayCmtryRaw
            // 
            resources.ApplyResources(this.cbDisplayCmtryRaw, "cbDisplayCmtryRaw");
            this.cbDisplayCmtryRaw.Name = "cbDisplayCmtryRaw";
            this.cbDisplayCmtryRaw.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbAutoSetCmtry);
            this.groupBox2.Controls.Add(this.cbCmtryHtmlToRtf);
            this.groupBox2.Controls.Add(this.cbCmtryStripHtml);
            this.groupBox2.Controls.Add(this.cbCmtryRtfToHtml);
            this.groupBox2.Controls.Add(this.rbCmtryRaw);
            this.groupBox2.Controls.Add(this.rbCmtryParse);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // cbAutoSetCmtry
            // 
            resources.ApplyResources(this.cbAutoSetCmtry, "cbAutoSetCmtry");
            this.cbAutoSetCmtry.Checked = true;
            this.cbAutoSetCmtry.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAutoSetCmtry.Name = "cbAutoSetCmtry";
            this.cbAutoSetCmtry.UseVisualStyleBackColor = true;
            this.cbAutoSetCmtry.CheckedChanged += new System.EventHandler(this.cbAutoSetCmtry_CheckedChanged);
            // 
            // cbCmtryHtmlToRtf
            // 
            resources.ApplyResources(this.cbCmtryHtmlToRtf, "cbCmtryHtmlToRtf");
            this.cbCmtryHtmlToRtf.Name = "cbCmtryHtmlToRtf";
            this.cbCmtryHtmlToRtf.UseVisualStyleBackColor = true;
            // 
            // cbCmtryStripHtml
            // 
            resources.ApplyResources(this.cbCmtryStripHtml, "cbCmtryStripHtml");
            this.cbCmtryStripHtml.Name = "cbCmtryStripHtml";
            this.cbCmtryStripHtml.UseVisualStyleBackColor = true;
            // 
            // cbCmtryRtfToHtml
            // 
            resources.ApplyResources(this.cbCmtryRtfToHtml, "cbCmtryRtfToHtml");
            this.cbCmtryRtfToHtml.Name = "cbCmtryRtfToHtml";
            this.cbCmtryRtfToHtml.UseVisualStyleBackColor = true;
            // 
            // rbCmtryRaw
            // 
            resources.ApplyResources(this.rbCmtryRaw, "rbCmtryRaw");
            this.rbCmtryRaw.Name = "rbCmtryRaw";
            this.rbCmtryRaw.TabStop = true;
            this.rbCmtryRaw.UseVisualStyleBackColor = true;
            this.rbCmtryRaw.CheckedChanged += new System.EventHandler(this.rbCmtryRaw_CheckedChanged);
            // 
            // rbCmtryParse
            // 
            resources.ApplyResources(this.rbCmtryParse, "rbCmtryParse");
            this.rbCmtryParse.Checked = true;
            this.rbCmtryParse.Name = "rbCmtryParse";
            this.rbCmtryParse.TabStop = true;
            this.rbCmtryParse.UseVisualStyleBackColor = true;
            this.rbCmtryParse.CheckedChanged += new System.EventHandler(this.rbCmtryParse_CheckedChanged);
            // 
            // tabDictionary
            // 
            this.tabDictionary.Controls.Add(this.cbDisplayDictRaw);
            this.tabDictionary.Controls.Add(this.groupBox3);
            resources.ApplyResources(this.tabDictionary, "tabDictionary");
            this.tabDictionary.Name = "tabDictionary";
            this.tabDictionary.UseVisualStyleBackColor = true;
            // 
            // cbDisplayDictRaw
            // 
            resources.ApplyResources(this.cbDisplayDictRaw, "cbDisplayDictRaw");
            this.cbDisplayDictRaw.Name = "cbDisplayDictRaw";
            this.cbDisplayDictRaw.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cbAutoSetDict);
            this.groupBox3.Controls.Add(this.cbDictHtmlToRtf);
            this.groupBox3.Controls.Add(this.cbDictStripHtml);
            this.groupBox3.Controls.Add(this.cbDictRtfToHtml);
            this.groupBox3.Controls.Add(this.rbDictRaw);
            this.groupBox3.Controls.Add(this.rbDictParse);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // cbAutoSetDict
            // 
            resources.ApplyResources(this.cbAutoSetDict, "cbAutoSetDict");
            this.cbAutoSetDict.Checked = true;
            this.cbAutoSetDict.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAutoSetDict.Name = "cbAutoSetDict";
            this.cbAutoSetDict.UseVisualStyleBackColor = true;
            this.cbAutoSetDict.CheckedChanged += new System.EventHandler(this.cbAutoSetDict_CheckedChanged);
            // 
            // cbDictHtmlToRtf
            // 
            resources.ApplyResources(this.cbDictHtmlToRtf, "cbDictHtmlToRtf");
            this.cbDictHtmlToRtf.Name = "cbDictHtmlToRtf";
            this.cbDictHtmlToRtf.UseVisualStyleBackColor = true;
            // 
            // cbDictStripHtml
            // 
            resources.ApplyResources(this.cbDictStripHtml, "cbDictStripHtml");
            this.cbDictStripHtml.Name = "cbDictStripHtml";
            this.cbDictStripHtml.UseVisualStyleBackColor = true;
            // 
            // cbDictRtfToHtml
            // 
            resources.ApplyResources(this.cbDictRtfToHtml, "cbDictRtfToHtml");
            this.cbDictRtfToHtml.Name = "cbDictRtfToHtml";
            this.cbDictRtfToHtml.UseVisualStyleBackColor = true;
            // 
            // rbDictRaw
            // 
            resources.ApplyResources(this.rbDictRaw, "rbDictRaw");
            this.rbDictRaw.Name = "rbDictRaw";
            this.rbDictRaw.TabStop = true;
            this.rbDictRaw.UseVisualStyleBackColor = true;
            this.rbDictRaw.CheckedChanged += new System.EventHandler(this.rbDictRaw_CheckedChanged);
            // 
            // rbDictParse
            // 
            resources.ApplyResources(this.rbDictParse, "rbDictParse");
            this.rbDictParse.Checked = true;
            this.rbDictParse.Name = "rbDictParse";
            this.rbDictParse.TabStop = true;
            this.rbDictParse.UseVisualStyleBackColor = true;
            this.rbDictParse.CheckedChanged += new System.EventHandler(this.rbDictParse_CheckedChanged);
            // 
            // tabMP3
            // 
            this.tabMP3.Controls.Add(this.button2);
            this.tabMP3.Controls.Add(this.cbMp3Transliterate);
            this.tabMP3.Controls.Add(this.tbMp3Text);
            this.tabMP3.Controls.Add(this.label4);
            this.tabMP3.Controls.Add(this.button1);
            this.tabMP3.Controls.Add(this.cbMp3Voices);
            this.tabMP3.Controls.Add(this.label3);
            this.tabMP3.Controls.Add(this.tbMP3Speed);
            this.tabMP3.Controls.Add(this.tbMp3Volume);
            this.tabMP3.Controls.Add(this.label2);
            resources.ApplyResources(this.tabMP3, "tabMP3");
            this.tabMP3.Name = "tabMP3";
            this.tabMP3.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            resources.ApplyResources(this.button2, "button2");
            this.button2.Name = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // cbMp3Transliterate
            // 
            resources.ApplyResources(this.cbMp3Transliterate, "cbMp3Transliterate");
            this.cbMp3Transliterate.Name = "cbMp3Transliterate";
            this.cbMp3Transliterate.UseVisualStyleBackColor = true;
            // 
            // tbMp3Text
            // 
            this.tbMp3Text.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.tbMp3Text, "tbMp3Text");
            this.tbMp3Text.Name = "tbMp3Text";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // cbMp3Voices
            // 
            this.cbMp3Voices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMp3Voices.FormattingEnabled = true;
            resources.ApplyResources(this.cbMp3Voices, "cbMp3Voices");
            this.cbMp3Voices.Name = "cbMp3Voices";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // tbMP3Speed
            // 
            resources.ApplyResources(this.tbMP3Speed, "tbMP3Speed");
            this.tbMP3Speed.Minimum = -10;
            this.tbMP3Speed.Name = "tbMP3Speed";
            this.tbMP3Speed.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbMP3Speed.Value = -2;
            // 
            // tbMp3Volume
            // 
            resources.ApplyResources(this.tbMp3Volume, "tbMp3Volume");
            this.tbMp3Volume.Maximum = 100;
            this.tbMp3Volume.Name = "tbMp3Volume";
            this.tbMp3Volume.TickFrequency = 10;
            this.tbMp3Volume.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbMp3Volume.Value = 90;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // tabExperimental
            // 
            this.tabExperimental.Controls.Add(this.label1);
            this.tabExperimental.Controls.Add(this.cbHtmlRenderer);
            this.tabExperimental.Controls.Add(this.cbUseRenderer);
            this.tabExperimental.Controls.Add(this.cbUseCtrlHtmlRtfConv);
            this.tabExperimental.Controls.Add(this.cbSaveLoadFilePref);
            this.tabExperimental.Controls.Add(this.cbEditBibleVerse);
            resources.ApplyResources(this.tabExperimental, "tabExperimental");
            this.tabExperimental.Name = "tabExperimental";
            this.tabExperimental.UseVisualStyleBackColor = true;
            // 
            // cbHtmlRenderer
            // 
            this.cbHtmlRenderer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.cbHtmlRenderer, "cbHtmlRenderer");
            this.cbHtmlRenderer.FormattingEnabled = true;
            this.cbHtmlRenderer.Items.AddRange(new object[] {
            resources.GetString("cbHtmlRenderer.Items"),
            resources.GetString("cbHtmlRenderer.Items1")});
            this.cbHtmlRenderer.Name = "cbHtmlRenderer";
            // 
            // cbUseRenderer
            // 
            resources.ApplyResources(this.cbUseRenderer, "cbUseRenderer");
            this.cbUseRenderer.Name = "cbUseRenderer";
            this.cbUseRenderer.UseVisualStyleBackColor = true;
            this.cbUseRenderer.CheckedChanged += new System.EventHandler(this.cbUseRenderer_CheckedChanged);
            // 
            // cbUseCtrlHtmlRtfConv
            // 
            resources.ApplyResources(this.cbUseCtrlHtmlRtfConv, "cbUseCtrlHtmlRtfConv");
            this.cbUseCtrlHtmlRtfConv.Name = "cbUseCtrlHtmlRtfConv";
            this.cbUseCtrlHtmlRtfConv.UseVisualStyleBackColor = true;
            // 
            // cbSaveLoadFilePref
            // 
            resources.ApplyResources(this.cbSaveLoadFilePref, "cbSaveLoadFilePref");
            this.cbSaveLoadFilePref.Name = "cbSaveLoadFilePref";
            this.cbSaveLoadFilePref.UseVisualStyleBackColor = true;
            // 
            // cbEditBibleVerse
            // 
            resources.ApplyResources(this.cbEditBibleVerse, "cbEditBibleVerse");
            this.cbEditBibleVerse.Name = "cbEditBibleVerse";
            this.cbEditBibleVerse.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.OK, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.Cancel, 1, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // OK
            // 
            resources.ApplyResources(this.OK, "OK");
            this.OK.Name = "OK";
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // Cancel
            // 
            resources.ApplyResources(this.Cancel, "Cancel");
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel.Name = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // PreferencesFrm
            // 
            this.AcceptButton = this.OK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Cancel;
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "PreferencesFrm";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.PreferencesFrm_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabBible.ResumeLayout(false);
            this.tabBible.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabBook.ResumeLayout(false);
            this.tabBook.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.tabCommentary.ResumeLayout(false);
            this.tabCommentary.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabDictionary.ResumeLayout(false);
            this.tabDictionary.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tabMP3.ResumeLayout(false);
            this.tabMP3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbMP3Speed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbMp3Volume)).EndInit();
            this.tabExperimental.ResumeLayout(false);
            this.tabExperimental.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabBible;
        private System.Windows.Forms.TabPage tabCommentary;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button OK;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.TabPage tabDictionary;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox cbBibleBkgndCleaning;
        private System.Windows.Forms.RadioButton rbBibleRaw;
        private System.Windows.Forms.RadioButton rbBibleParse;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rbCmtryRaw;
        private System.Windows.Forms.RadioButton rbCmtryParse;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton rbDictRaw;
        private System.Windows.Forms.RadioButton rbDictParse;
        private System.Windows.Forms.CheckBox cbBibleCmtryIcon;
        private System.Windows.Forms.TabPage tabBook;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton rbBookRaw;
        private System.Windows.Forms.RadioButton rbBookParse;
        private System.Windows.Forms.CheckBox cbBookBkgndCleaning;
        private System.Windows.Forms.TabPage tabMP3;
        private System.Windows.Forms.TrackBar tbMp3Volume;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TrackBar tbMP3Speed;
        private System.Windows.Forms.TextBox tbMp3Text;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox cbMp3Voices;
        private System.Windows.Forms.CheckBox cbMp3Transliterate;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox cbBibleRedText;
        private System.Windows.Forms.CheckBox cbBookRtfToHtml;
        private System.Windows.Forms.CheckBox cbBibleRtfToHtml;
        private System.Windows.Forms.CheckBox cbCmtryRtfToHtml;
        private System.Windows.Forms.CheckBox cbDictRtfToHtml;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.RadioButton rbCrossWireLib;
        private System.Windows.Forms.RadioButton rbCrossConnectLib;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox cbVersification;
        private System.Windows.Forms.CheckBox cbCmtryStripHtml;
        private System.Windows.Forms.CheckBox cbDictStripHtml;
        private System.Windows.Forms.CheckBox cbBibleStripHtml;
        private System.Windows.Forms.CheckBox cbBibleHtmlToRtf;
        private System.Windows.Forms.CheckBox cbBookStripHtml;
        private System.Windows.Forms.CheckBox cbBookHtmlToRtf;
        private System.Windows.Forms.CheckBox cbCmtryHtmlToRtf;
        private System.Windows.Forms.CheckBox cbDictHtmlToRtf;
        private System.Windows.Forms.CheckBox cbAutoSetBible;
        private System.Windows.Forms.CheckBox cbAutoSetBook;
        private System.Windows.Forms.CheckBox cbAutoSetCmtry;
        private System.Windows.Forms.CheckBox cbAutoSetDict;
        private System.Windows.Forms.CheckBox cbDisplayBIBRawView;
        private System.Windows.Forms.CheckBox cbDisplayBookRaw;
        private System.Windows.Forms.CheckBox cbDisplayCmtryRaw;
        private System.Windows.Forms.CheckBox cbDisplayDictRaw;
        private System.Windows.Forms.TabPage tabExperimental;
        private System.Windows.Forms.CheckBox cbEditBibleVerse;
        private System.Windows.Forms.CheckBox cbSaveLoadFilePref;
        private System.Windows.Forms.CheckBox cbUseCtrlHtmlRtfConv;
        private System.Windows.Forms.CheckBox cbUseRenderer;
        private System.Windows.Forms.ComboBox cbHtmlRenderer;
        private System.Windows.Forms.Label label1;
    }
}