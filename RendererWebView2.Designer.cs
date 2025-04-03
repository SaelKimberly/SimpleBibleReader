using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;

using System;
using System.Collections;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;

namespace Simple_Bible_Reader
{
	partial class RendererWebView2
	{
		
		//Form overrides dispose to clean up the component list.
		[System.Diagnostics.DebuggerNonUserCode()]protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing && components != null)
				{
					components.Dispose();
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

        //Required by the Windows Form Designer
		
		//NOTE: The following procedure is required by the Windows Form Designer
		//It can be modified using the Windows Form Designer.
		//Do not modify it using the code editor.
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RendererWebView2));
            this.ToolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.StatusStrip1 = new System.Windows.Forms.StatusStrip();
            this.ProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.StatusText = new System.Windows.Forms.ToolStripStatusLabel();
            this.SplitContainer1 = new System.Windows.Forms.SplitContainer();
            this.pictureBoxFront = new System.Windows.Forms.PictureBox();
            this.MirrorTreeView1 = new Simple_Bible_Reader.MirrorTreeView();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.backBtn = new System.Windows.Forms.Button();
            this.frontBtn = new System.Windows.Forms.Button();
            this.zoomOutBtn = new System.Windows.Forms.Button();
            this.zoomInBtn = new System.Windows.Forms.Button();
            this.fullScreenBtn = new System.Windows.Forms.CheckBox();
            this.paraBtn = new System.Windows.Forms.CheckBox();
            this.fontBtn = new System.Windows.Forms.Button();
            this.editBtn = new System.Windows.Forms.Button();
            this.findBtn = new System.Windows.Forms.Button();
            this.printChapterBtn = new System.Windows.Forms.Button();
            this.transliterateBtn = new System.Windows.Forms.CheckBox();
            this.btnBible = new System.Windows.Forms.Button();
            this.statBtn = new System.Windows.Forms.Button();
            this.btnPlugin = new System.Windows.Forms.Button();
            this.bibleTitleLbl = new System.Windows.Forms.Label();
            this.helpBtn = new System.Windows.Forms.Button();
            this.ChapterArea = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.chkBoxShowList = new System.Windows.Forms.CheckBox();
            this.cmtFullScreenBtn = new System.Windows.Forms.CheckBox();
            this.cmtZoomInBtn = new System.Windows.Forms.Button();
            this.cmtZoomOutBtn = new System.Windows.Forms.Button();
            this.cmtFontBtn = new System.Windows.Forms.Button();
            this.commentaryTitleLbl = new System.Windows.Forms.Label();
            this.cmtPrintBtn = new System.Windows.Forms.Button();
            this.cmtEditBtn = new System.Windows.Forms.Button();
            this.splitContainer5 = new System.Windows.Forms.SplitContainer();
            this.cmtListBox = new System.Windows.Forms.ListBox();
            this.CommentaryArea = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
            this.chkBoxShowDictList = new System.Windows.Forms.CheckBox();
            this.dctFullScreenBtn = new System.Windows.Forms.CheckBox();
            this.dictEditBtn = new System.Windows.Forms.Button();
            this.dictZoomInBtn = new System.Windows.Forms.Button();
            this.dctFontBtn = new System.Windows.Forms.Button();
            this.dictZoomOutBtn = new System.Windows.Forms.Button();
            this.dictionaryTitleLbl = new System.Windows.Forms.Label();
            this.dictPrintBtn = new System.Windows.Forms.Button();
            this.splitContainer6 = new System.Windows.Forms.SplitContainer();
            this.dictListBox = new System.Windows.Forms.ListBox();
            this.DictionaryArea = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.SearchResultsArea = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnCollapse = new System.Windows.Forms.Button();
            this.btnExpand = new System.Windows.Forms.Button();
            this.moreVersesBtn = new System.Windows.Forms.CheckBox();
            this.fullScreenSearchBtn = new System.Windows.Forms.CheckBox();
            this.MenuStrip1 = new System.Windows.Forms.MenuStrip();
            this.ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportBibleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.openBookToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportBookToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.openCommentaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportCommentaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.openDictionaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportDictionaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.batchConvertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.RTL = new System.Windows.Forms.ToolStripMenuItem();
            this.ExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.layoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.booksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bibleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.commentaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dictionaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.preferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pluginsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.localizationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.englishToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.spanishToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tamilToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.portuguesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.themesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.HelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.WebsiteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.releaseNotesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.creditsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.BackgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.Timer1 = new System.Windows.Forms.Timer(this.components);
            this.backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.timerSelectVerse = new System.Windows.Forms.Timer(this.components);
            this.timer5 = new System.Windows.Forms.Timer(this.components);
            this.backgroundCleaner = new System.ComponentModel.BackgroundWorker();
            this.readerOpenDialog = new System.Windows.Forms.OpenFileDialog();
            this.readerSaveDialog = new System.Windows.Forms.SaveFileDialog();
            this.backgroundWorker3 = new System.ComponentModel.BackgroundWorker();
            this.timer6 = new System.Windows.Forms.Timer(this.components);
            this.printPreviewDialog1 = new System.Windows.Forms.PrintPreviewDialog();
            this.BibleReaderToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.bkgndTransLitJob = new System.ComponentModel.BackgroundWorker();
            this.speechBkgndWorker = new System.ComponentModel.BackgroundWorker();
            this.ToolStripContainer1.BottomToolStripPanel.SuspendLayout();
            this.ToolStripContainer1.ContentPanel.SuspendLayout();
            this.ToolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.ToolStripContainer1.SuspendLayout();
            this.StatusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer1)).BeginInit();
            this.SplitContainer1.Panel1.SuspendLayout();
            this.SplitContainer1.Panel2.SuspendLayout();
            this.SplitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFront)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ChapterArea)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer5)).BeginInit();
            this.splitContainer5.Panel1.SuspendLayout();
            this.splitContainer5.Panel2.SuspendLayout();
            this.splitContainer5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CommentaryArea)).BeginInit();
            this.tableLayoutPanel7.SuspendLayout();
            this.tableLayoutPanel8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer6)).BeginInit();
            this.splitContainer6.Panel1.SuspendLayout();
            this.splitContainer6.Panel2.SuspendLayout();
            this.splitContainer6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DictionaryArea)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SearchResultsArea)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.MenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ToolStripContainer1
            // 
            // 
            // ToolStripContainer1.BottomToolStripPanel
            // 
            this.ToolStripContainer1.BottomToolStripPanel.Controls.Add(this.StatusStrip1);
            // 
            // ToolStripContainer1.ContentPanel
            // 
            this.ToolStripContainer1.ContentPanel.Controls.Add(this.SplitContainer1);
            resources.ApplyResources(this.ToolStripContainer1.ContentPanel, "ToolStripContainer1.ContentPanel");
            resources.ApplyResources(this.ToolStripContainer1, "ToolStripContainer1");
            this.ToolStripContainer1.Name = "ToolStripContainer1";
            // 
            // ToolStripContainer1.TopToolStripPanel
            // 
            this.ToolStripContainer1.TopToolStripPanel.Controls.Add(this.MenuStrip1);
            // 
            // StatusStrip1
            // 
            resources.ApplyResources(this.StatusStrip1, "StatusStrip1");
            this.StatusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.StatusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ProgressBar,
            this.StatusText});
            this.StatusStrip1.Name = "StatusStrip1";
            // 
            // ProgressBar
            // 
            this.ProgressBar.Name = "ProgressBar";
            resources.ApplyResources(this.ProgressBar, "ProgressBar");
            // 
            // StatusText
            // 
            this.StatusText.Name = "StatusText";
            resources.ApplyResources(this.StatusText, "StatusText");
            // 
            // SplitContainer1
            // 
            resources.ApplyResources(this.SplitContainer1, "SplitContainer1");
            this.SplitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.SplitContainer1.Name = "SplitContainer1";
            // 
            // SplitContainer1.Panel1
            // 
            this.SplitContainer1.Panel1.Controls.Add(this.pictureBoxFront);
            this.SplitContainer1.Panel1.Controls.Add(this.MirrorTreeView1);
            // 
            // SplitContainer1.Panel2
            // 
            this.SplitContainer1.Panel2.Controls.Add(this.splitContainer2);
            // 
            // pictureBoxFront
            // 
            this.pictureBoxFront.BackColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(this.pictureBoxFront, "pictureBoxFront");
            this.pictureBoxFront.Image = global::Simple_Bible_Reader.Properties.Resources.books;
            this.pictureBoxFront.Name = "pictureBoxFront";
            this.pictureBoxFront.TabStop = false;
            // 
            // MirrorTreeView1
            // 
            this.MirrorTreeView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.MirrorTreeView1, "MirrorTreeView1");
            this.MirrorTreeView1.Name = "MirrorTreeView1";
            this.MirrorTreeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.MirrorTreeView1_AfterSelect);
            // 
            // splitContainer2
            // 
            resources.ApplyResources(this.splitContainer2, "splitContainer2");
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer3);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.tableLayoutPanel1);
            // 
            // splitContainer3
            // 
            resources.ApplyResources(this.splitContainer3, "splitContainer3");
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.tableLayoutPanel3);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.splitContainer4);
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel4, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.ChapterArea, 0, 1);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // tableLayoutPanel4
            // 
            resources.ApplyResources(this.tableLayoutPanel4, "tableLayoutPanel4");
            this.tableLayoutPanel4.Controls.Add(this.backBtn, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.frontBtn, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.zoomOutBtn, 3, 0);
            this.tableLayoutPanel4.Controls.Add(this.zoomInBtn, 2, 0);
            this.tableLayoutPanel4.Controls.Add(this.fullScreenBtn, 4, 0);
            this.tableLayoutPanel4.Controls.Add(this.paraBtn, 5, 0);
            this.tableLayoutPanel4.Controls.Add(this.fontBtn, 6, 0);
            this.tableLayoutPanel4.Controls.Add(this.editBtn, 7, 0);
            this.tableLayoutPanel4.Controls.Add(this.findBtn, 8, 0);
            this.tableLayoutPanel4.Controls.Add(this.printChapterBtn, 9, 0);
            this.tableLayoutPanel4.Controls.Add(this.transliterateBtn, 10, 0);
            this.tableLayoutPanel4.Controls.Add(this.btnBible, 12, 0);
            this.tableLayoutPanel4.Controls.Add(this.statBtn, 13, 0);
            this.tableLayoutPanel4.Controls.Add(this.btnPlugin, 11, 0);
            this.tableLayoutPanel4.Controls.Add(this.bibleTitleLbl, 15, 0);
            this.tableLayoutPanel4.Controls.Add(this.helpBtn, 14, 0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            // 
            // backBtn
            // 
            resources.ApplyResources(this.backBtn, "backBtn");
            this.backBtn.FlatAppearance.BorderSize = 0;
            this.backBtn.Image = global::Simple_Bible_Reader.Properties.Resources.backward;
            this.backBtn.Name = "backBtn";
            this.BibleReaderToolTip.SetToolTip(this.backBtn, resources.GetString("backBtn.ToolTip"));
            this.backBtn.UseVisualStyleBackColor = false;
            this.backBtn.Click += new System.EventHandler(this.backBtn_Click);
            // 
            // frontBtn
            // 
            resources.ApplyResources(this.frontBtn, "frontBtn");
            this.frontBtn.FlatAppearance.BorderSize = 0;
            this.frontBtn.Image = global::Simple_Bible_Reader.Properties.Resources.forward;
            this.frontBtn.Name = "frontBtn";
            this.BibleReaderToolTip.SetToolTip(this.frontBtn, resources.GetString("frontBtn.ToolTip"));
            this.frontBtn.UseVisualStyleBackColor = false;
            this.frontBtn.Click += new System.EventHandler(this.frontBtn_Click);
            // 
            // zoomOutBtn
            // 
            this.zoomOutBtn.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.zoomOutBtn, "zoomOutBtn");
            this.zoomOutBtn.Image = global::Simple_Bible_Reader.Properties.Resources.zoom_out;
            this.zoomOutBtn.Name = "zoomOutBtn";
            this.BibleReaderToolTip.SetToolTip(this.zoomOutBtn, resources.GetString("zoomOutBtn.ToolTip"));
            this.zoomOutBtn.UseVisualStyleBackColor = false;
            this.zoomOutBtn.Click += new System.EventHandler(this.zoomOutBtn_Click);
            // 
            // zoomInBtn
            // 
            this.zoomInBtn.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.zoomInBtn, "zoomInBtn");
            this.zoomInBtn.Image = global::Simple_Bible_Reader.Properties.Resources.zoom_in;
            this.zoomInBtn.Name = "zoomInBtn";
            this.BibleReaderToolTip.SetToolTip(this.zoomInBtn, resources.GetString("zoomInBtn.ToolTip"));
            this.zoomInBtn.UseVisualStyleBackColor = false;
            this.zoomInBtn.Click += new System.EventHandler(this.zoomInBtn_Click);
            // 
            // fullScreenBtn
            // 
            resources.ApplyResources(this.fullScreenBtn, "fullScreenBtn");
            this.fullScreenBtn.FlatAppearance.BorderSize = 0;
            this.fullScreenBtn.Image = global::Simple_Bible_Reader.Properties.Resources.fullscreen;
            this.fullScreenBtn.Name = "fullScreenBtn";
            this.BibleReaderToolTip.SetToolTip(this.fullScreenBtn, resources.GetString("fullScreenBtn.ToolTip"));
            this.fullScreenBtn.UseVisualStyleBackColor = false;
            this.fullScreenBtn.CheckedChanged += new System.EventHandler(this.fullScreenBtn_CheckedChanged);
            // 
            // paraBtn
            // 
            resources.ApplyResources(this.paraBtn, "paraBtn");
            this.paraBtn.FlatAppearance.BorderSize = 0;
            this.paraBtn.Image = global::Simple_Bible_Reader.Properties.Resources.paragraph;
            this.paraBtn.Name = "paraBtn";
            this.BibleReaderToolTip.SetToolTip(this.paraBtn, resources.GetString("paraBtn.ToolTip"));
            this.paraBtn.UseVisualStyleBackColor = false;
            this.paraBtn.CheckedChanged += new System.EventHandler(this.paraBtn_CheckedChanged);
            // 
            // fontBtn
            // 
            this.fontBtn.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.fontBtn, "fontBtn");
            this.fontBtn.Image = global::Simple_Bible_Reader.Properties.Resources.font;
            this.fontBtn.Name = "fontBtn";
            this.BibleReaderToolTip.SetToolTip(this.fontBtn, resources.GetString("fontBtn.ToolTip"));
            this.fontBtn.UseVisualStyleBackColor = false;
            this.fontBtn.Click += new System.EventHandler(this.fontBtn_Click);
            // 
            // editBtn
            // 
            this.editBtn.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.editBtn, "editBtn");
            this.editBtn.Image = global::Simple_Bible_Reader.Properties.Resources.edit;
            this.editBtn.Name = "editBtn";
            this.BibleReaderToolTip.SetToolTip(this.editBtn, resources.GetString("editBtn.ToolTip"));
            this.editBtn.UseVisualStyleBackColor = false;
            this.editBtn.Click += new System.EventHandler(this.editBtn_Click);
            // 
            // findBtn
            // 
            this.findBtn.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.findBtn, "findBtn");
            this.findBtn.Image = global::Simple_Bible_Reader.Properties.Resources.find;
            this.findBtn.Name = "findBtn";
            this.BibleReaderToolTip.SetToolTip(this.findBtn, resources.GetString("findBtn.ToolTip"));
            this.findBtn.UseVisualStyleBackColor = false;
            this.findBtn.Click += new System.EventHandler(this.findBtn_Click);
            // 
            // printChapterBtn
            // 
            this.printChapterBtn.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.printChapterBtn, "printChapterBtn");
            this.printChapterBtn.Image = global::Simple_Bible_Reader.Properties.Resources.print;
            this.printChapterBtn.Name = "printChapterBtn";
            this.BibleReaderToolTip.SetToolTip(this.printChapterBtn, resources.GetString("printChapterBtn.ToolTip"));
            this.printChapterBtn.UseVisualStyleBackColor = false;
            this.printChapterBtn.Click += new System.EventHandler(this.printChapterBtn_Click);
            // 
            // transliterateBtn
            // 
            resources.ApplyResources(this.transliterateBtn, "transliterateBtn");
            this.transliterateBtn.FlatAppearance.BorderSize = 0;
            this.transliterateBtn.Image = global::Simple_Bible_Reader.Properties.Resources.transliterate;
            this.transliterateBtn.Name = "transliterateBtn";
            this.BibleReaderToolTip.SetToolTip(this.transliterateBtn, resources.GetString("transliterateBtn.ToolTip"));
            this.transliterateBtn.UseVisualStyleBackColor = false;
            this.transliterateBtn.CheckedChanged += new System.EventHandler(this.transliterateBtn_CheckedChanged);
            // 
            // btnBible
            // 
            this.btnBible.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btnBible, "btnBible");
            this.btnBible.Image = global::Simple_Bible_Reader.Properties.Resources.bible;
            this.btnBible.Name = "btnBible";
            this.BibleReaderToolTip.SetToolTip(this.btnBible, resources.GetString("btnBible.ToolTip"));
            this.btnBible.UseVisualStyleBackColor = false;
            this.btnBible.Click += new System.EventHandler(this.btnBible_Click);
            // 
            // statBtn
            // 
            this.statBtn.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.statBtn, "statBtn");
            this.statBtn.Image = global::Simple_Bible_Reader.Properties.Resources.stat;
            this.statBtn.Name = "statBtn";
            this.BibleReaderToolTip.SetToolTip(this.statBtn, resources.GetString("statBtn.ToolTip"));
            this.statBtn.UseVisualStyleBackColor = false;
            this.statBtn.Click += new System.EventHandler(this.statBtn_Click);
            // 
            // btnPlugin
            // 
            this.btnPlugin.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btnPlugin, "btnPlugin");
            this.btnPlugin.Image = global::Simple_Bible_Reader.Properties.Resources.run;
            this.btnPlugin.Name = "btnPlugin";
            this.BibleReaderToolTip.SetToolTip(this.btnPlugin, resources.GetString("btnPlugin.ToolTip"));
            this.btnPlugin.UseVisualStyleBackColor = false;
            this.btnPlugin.Click += new System.EventHandler(this.btnPlugin_Click);
            // 
            // bibleTitleLbl
            // 
            resources.ApplyResources(this.bibleTitleLbl, "bibleTitleLbl");
            this.bibleTitleLbl.Name = "bibleTitleLbl";
            // 
            // helpBtn
            // 
            this.helpBtn.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.helpBtn, "helpBtn");
            this.helpBtn.Image = global::Simple_Bible_Reader.Properties.Resources.help;
            this.helpBtn.Name = "helpBtn";
            this.BibleReaderToolTip.SetToolTip(this.helpBtn, resources.GetString("helpBtn.ToolTip"));
            this.helpBtn.UseVisualStyleBackColor = false;
            this.helpBtn.Click += new System.EventHandler(this.helpBtn_Click);
            // 
            // ChapterArea
            // 
            this.ChapterArea.AllowExternalDrop = true;
            this.ChapterArea.CreationProperties = null;
            this.ChapterArea.DefaultBackgroundColor = System.Drawing.Color.White;
            resources.ApplyResources(this.ChapterArea, "ChapterArea");
            this.ChapterArea.Name = "ChapterArea";
            this.ChapterArea.ZoomFactor = 1D;
            // 
            // splitContainer4
            // 
            resources.ApplyResources(this.splitContainer4, "splitContainer4");
            this.splitContainer4.Name = "splitContainer4";
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.tableLayoutPanel5);
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.Controls.Add(this.tableLayoutPanel7);
            // 
            // tableLayoutPanel5
            // 
            resources.ApplyResources(this.tableLayoutPanel5, "tableLayoutPanel5");
            this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel6, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.splitContainer5, 0, 1);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            // 
            // tableLayoutPanel6
            // 
            resources.ApplyResources(this.tableLayoutPanel6, "tableLayoutPanel6");
            this.tableLayoutPanel6.Controls.Add(this.chkBoxShowList, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.cmtFullScreenBtn, 4, 0);
            this.tableLayoutPanel6.Controls.Add(this.cmtZoomInBtn, 1, 0);
            this.tableLayoutPanel6.Controls.Add(this.cmtZoomOutBtn, 2, 0);
            this.tableLayoutPanel6.Controls.Add(this.cmtFontBtn, 3, 0);
            this.tableLayoutPanel6.Controls.Add(this.commentaryTitleLbl, 7, 0);
            this.tableLayoutPanel6.Controls.Add(this.cmtPrintBtn, 6, 0);
            this.tableLayoutPanel6.Controls.Add(this.cmtEditBtn, 4, 0);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            // 
            // chkBoxShowList
            // 
            resources.ApplyResources(this.chkBoxShowList, "chkBoxShowList");
            this.chkBoxShowList.Checked = true;
            this.chkBoxShowList.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBoxShowList.FlatAppearance.BorderSize = 0;
            this.chkBoxShowList.Image = global::Simple_Bible_Reader.Properties.Resources.show_menu;
            this.chkBoxShowList.Name = "chkBoxShowList";
            this.BibleReaderToolTip.SetToolTip(this.chkBoxShowList, resources.GetString("chkBoxShowList.ToolTip"));
            this.chkBoxShowList.UseVisualStyleBackColor = false;
            this.chkBoxShowList.CheckedChanged += new System.EventHandler(this.chkBoxShowList_CheckedChanged);
            // 
            // cmtFullScreenBtn
            // 
            resources.ApplyResources(this.cmtFullScreenBtn, "cmtFullScreenBtn");
            this.cmtFullScreenBtn.FlatAppearance.BorderSize = 0;
            this.cmtFullScreenBtn.Image = global::Simple_Bible_Reader.Properties.Resources.fullscreen;
            this.cmtFullScreenBtn.Name = "cmtFullScreenBtn";
            this.BibleReaderToolTip.SetToolTip(this.cmtFullScreenBtn, resources.GetString("cmtFullScreenBtn.ToolTip"));
            this.cmtFullScreenBtn.UseVisualStyleBackColor = false;
            this.cmtFullScreenBtn.CheckedChanged += new System.EventHandler(this.cmtFullScreenBtn_CheckedChanged);
            // 
            // cmtZoomInBtn
            // 
            this.cmtZoomInBtn.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.cmtZoomInBtn, "cmtZoomInBtn");
            this.cmtZoomInBtn.Image = global::Simple_Bible_Reader.Properties.Resources.zoom_in;
            this.cmtZoomInBtn.Name = "cmtZoomInBtn";
            this.BibleReaderToolTip.SetToolTip(this.cmtZoomInBtn, resources.GetString("cmtZoomInBtn.ToolTip"));
            this.cmtZoomInBtn.UseVisualStyleBackColor = false;
            this.cmtZoomInBtn.Click += new System.EventHandler(this.cmtZoomInBtn_Click);
            // 
            // cmtZoomOutBtn
            // 
            this.cmtZoomOutBtn.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.cmtZoomOutBtn, "cmtZoomOutBtn");
            this.cmtZoomOutBtn.Image = global::Simple_Bible_Reader.Properties.Resources.zoom_out;
            this.cmtZoomOutBtn.Name = "cmtZoomOutBtn";
            this.BibleReaderToolTip.SetToolTip(this.cmtZoomOutBtn, resources.GetString("cmtZoomOutBtn.ToolTip"));
            this.cmtZoomOutBtn.UseVisualStyleBackColor = false;
            this.cmtZoomOutBtn.Click += new System.EventHandler(this.cmtZoomOutBtn_Click);
            // 
            // cmtFontBtn
            // 
            this.cmtFontBtn.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.cmtFontBtn, "cmtFontBtn");
            this.cmtFontBtn.Image = global::Simple_Bible_Reader.Properties.Resources.font;
            this.cmtFontBtn.Name = "cmtFontBtn";
            this.BibleReaderToolTip.SetToolTip(this.cmtFontBtn, resources.GetString("cmtFontBtn.ToolTip"));
            this.cmtFontBtn.UseVisualStyleBackColor = false;
            this.cmtFontBtn.Click += new System.EventHandler(this.cmtFontBtn_Click);
            // 
            // commentaryTitleLbl
            // 
            resources.ApplyResources(this.commentaryTitleLbl, "commentaryTitleLbl");
            this.commentaryTitleLbl.Name = "commentaryTitleLbl";
            // 
            // cmtPrintBtn
            // 
            this.cmtPrintBtn.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.cmtPrintBtn, "cmtPrintBtn");
            this.cmtPrintBtn.Image = global::Simple_Bible_Reader.Properties.Resources.print;
            this.cmtPrintBtn.Name = "cmtPrintBtn";
            this.BibleReaderToolTip.SetToolTip(this.cmtPrintBtn, resources.GetString("cmtPrintBtn.ToolTip"));
            this.cmtPrintBtn.UseVisualStyleBackColor = false;
            this.cmtPrintBtn.Click += new System.EventHandler(this.cmtPrintBtn_Click);
            // 
            // cmtEditBtn
            // 
            this.cmtEditBtn.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.cmtEditBtn, "cmtEditBtn");
            this.cmtEditBtn.Image = global::Simple_Bible_Reader.Properties.Resources.edit;
            this.cmtEditBtn.Name = "cmtEditBtn";
            this.BibleReaderToolTip.SetToolTip(this.cmtEditBtn, resources.GetString("cmtEditBtn.ToolTip"));
            this.cmtEditBtn.UseVisualStyleBackColor = false;
            this.cmtEditBtn.Click += new System.EventHandler(this.cmtEditBtn_Click);
            // 
            // splitContainer5
            // 
            resources.ApplyResources(this.splitContainer5, "splitContainer5");
            this.splitContainer5.Name = "splitContainer5";
            // 
            // splitContainer5.Panel1
            // 
            this.splitContainer5.Panel1.Controls.Add(this.cmtListBox);
            // 
            // splitContainer5.Panel2
            // 
            this.splitContainer5.Panel2.Controls.Add(this.CommentaryArea);
            // 
            // cmtListBox
            // 
            resources.ApplyResources(this.cmtListBox, "cmtListBox");
            this.cmtListBox.FormattingEnabled = true;
            this.cmtListBox.Name = "cmtListBox";
            this.cmtListBox.SelectedIndexChanged += new System.EventHandler(this.cmtListBox_SelectedIndexChanged);
            this.cmtListBox.DoubleClick += new System.EventHandler(this.cmtListBox_DoubleClick);
            // 
            // CommentaryArea
            // 
            this.CommentaryArea.AllowExternalDrop = false;
            this.CommentaryArea.CreationProperties = null;
            this.CommentaryArea.DefaultBackgroundColor = System.Drawing.Color.White;
            resources.ApplyResources(this.CommentaryArea, "CommentaryArea");
            this.CommentaryArea.Name = "CommentaryArea";
            this.CommentaryArea.ZoomFactor = 1D;
            // 
            // tableLayoutPanel7
            // 
            resources.ApplyResources(this.tableLayoutPanel7, "tableLayoutPanel7");
            this.tableLayoutPanel7.Controls.Add(this.tableLayoutPanel8, 0, 0);
            this.tableLayoutPanel7.Controls.Add(this.splitContainer6, 0, 1);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            // 
            // tableLayoutPanel8
            // 
            resources.ApplyResources(this.tableLayoutPanel8, "tableLayoutPanel8");
            this.tableLayoutPanel8.Controls.Add(this.chkBoxShowDictList, 0, 0);
            this.tableLayoutPanel8.Controls.Add(this.dctFullScreenBtn, 4, 0);
            this.tableLayoutPanel8.Controls.Add(this.dictEditBtn, 3, 0);
            this.tableLayoutPanel8.Controls.Add(this.dictZoomInBtn, 1, 0);
            this.tableLayoutPanel8.Controls.Add(this.dctFontBtn, 3, 0);
            this.tableLayoutPanel8.Controls.Add(this.dictZoomOutBtn, 2, 0);
            this.tableLayoutPanel8.Controls.Add(this.dictionaryTitleLbl, 7, 0);
            this.tableLayoutPanel8.Controls.Add(this.dictPrintBtn, 6, 0);
            this.tableLayoutPanel8.Name = "tableLayoutPanel8";
            // 
            // chkBoxShowDictList
            // 
            resources.ApplyResources(this.chkBoxShowDictList, "chkBoxShowDictList");
            this.chkBoxShowDictList.Checked = true;
            this.chkBoxShowDictList.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBoxShowDictList.FlatAppearance.BorderSize = 0;
            this.chkBoxShowDictList.Image = global::Simple_Bible_Reader.Properties.Resources.show_menu;
            this.chkBoxShowDictList.Name = "chkBoxShowDictList";
            this.BibleReaderToolTip.SetToolTip(this.chkBoxShowDictList, resources.GetString("chkBoxShowDictList.ToolTip"));
            this.chkBoxShowDictList.UseVisualStyleBackColor = false;
            this.chkBoxShowDictList.CheckedChanged += new System.EventHandler(this.chkBoxShowDictList_CheckedChanged);
            // 
            // dctFullScreenBtn
            // 
            resources.ApplyResources(this.dctFullScreenBtn, "dctFullScreenBtn");
            this.dctFullScreenBtn.FlatAppearance.BorderSize = 0;
            this.dctFullScreenBtn.Image = global::Simple_Bible_Reader.Properties.Resources.fullscreen;
            this.dctFullScreenBtn.Name = "dctFullScreenBtn";
            this.BibleReaderToolTip.SetToolTip(this.dctFullScreenBtn, resources.GetString("dctFullScreenBtn.ToolTip"));
            this.dctFullScreenBtn.UseVisualStyleBackColor = false;
            this.dctFullScreenBtn.CheckedChanged += new System.EventHandler(this.dctFullScreenBtn_CheckedChanged);
            // 
            // dictEditBtn
            // 
            this.dictEditBtn.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.dictEditBtn, "dictEditBtn");
            this.dictEditBtn.Image = global::Simple_Bible_Reader.Properties.Resources.edit;
            this.dictEditBtn.Name = "dictEditBtn";
            this.BibleReaderToolTip.SetToolTip(this.dictEditBtn, resources.GetString("dictEditBtn.ToolTip"));
            this.dictEditBtn.UseVisualStyleBackColor = false;
            this.dictEditBtn.Click += new System.EventHandler(this.dictEditBtn_Click);
            // 
            // dictZoomInBtn
            // 
            this.dictZoomInBtn.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.dictZoomInBtn, "dictZoomInBtn");
            this.dictZoomInBtn.Image = global::Simple_Bible_Reader.Properties.Resources.zoom_in;
            this.dictZoomInBtn.Name = "dictZoomInBtn";
            this.BibleReaderToolTip.SetToolTip(this.dictZoomInBtn, resources.GetString("dictZoomInBtn.ToolTip"));
            this.dictZoomInBtn.UseVisualStyleBackColor = false;
            this.dictZoomInBtn.Click += new System.EventHandler(this.dictZoomInBtn_Click);
            // 
            // dctFontBtn
            // 
            this.dctFontBtn.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.dctFontBtn, "dctFontBtn");
            this.dctFontBtn.Image = global::Simple_Bible_Reader.Properties.Resources.font;
            this.dctFontBtn.Name = "dctFontBtn";
            this.BibleReaderToolTip.SetToolTip(this.dctFontBtn, resources.GetString("dctFontBtn.ToolTip"));
            this.dctFontBtn.UseVisualStyleBackColor = false;
            this.dctFontBtn.Click += new System.EventHandler(this.dctFontBtn_Click);
            // 
            // dictZoomOutBtn
            // 
            this.dictZoomOutBtn.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.dictZoomOutBtn, "dictZoomOutBtn");
            this.dictZoomOutBtn.Image = global::Simple_Bible_Reader.Properties.Resources.zoom_out;
            this.dictZoomOutBtn.Name = "dictZoomOutBtn";
            this.BibleReaderToolTip.SetToolTip(this.dictZoomOutBtn, resources.GetString("dictZoomOutBtn.ToolTip"));
            this.dictZoomOutBtn.UseVisualStyleBackColor = false;
            this.dictZoomOutBtn.Click += new System.EventHandler(this.dictZoomOutBtn_Click);
            // 
            // dictionaryTitleLbl
            // 
            resources.ApplyResources(this.dictionaryTitleLbl, "dictionaryTitleLbl");
            this.dictionaryTitleLbl.Name = "dictionaryTitleLbl";
            // 
            // dictPrintBtn
            // 
            this.dictPrintBtn.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.dictPrintBtn, "dictPrintBtn");
            this.dictPrintBtn.Image = global::Simple_Bible_Reader.Properties.Resources.print;
            this.dictPrintBtn.Name = "dictPrintBtn";
            this.BibleReaderToolTip.SetToolTip(this.dictPrintBtn, resources.GetString("dictPrintBtn.ToolTip"));
            this.dictPrintBtn.UseVisualStyleBackColor = false;
            this.dictPrintBtn.Click += new System.EventHandler(this.dictPrintBtn_Click);
            // 
            // splitContainer6
            // 
            resources.ApplyResources(this.splitContainer6, "splitContainer6");
            this.splitContainer6.Name = "splitContainer6";
            // 
            // splitContainer6.Panel1
            // 
            this.splitContainer6.Panel1.Controls.Add(this.dictListBox);
            // 
            // splitContainer6.Panel2
            // 
            this.splitContainer6.Panel2.Controls.Add(this.DictionaryArea);
            // 
            // dictListBox
            // 
            resources.ApplyResources(this.dictListBox, "dictListBox");
            this.dictListBox.FormattingEnabled = true;
            this.dictListBox.Name = "dictListBox";
            this.dictListBox.SelectedIndexChanged += new System.EventHandler(this.dictListBox_SelectedIndexChanged);
            // 
            // DictionaryArea
            // 
            this.DictionaryArea.AllowExternalDrop = false;
            this.DictionaryArea.CreationProperties = null;
            this.DictionaryArea.DefaultBackgroundColor = System.Drawing.Color.White;
            resources.ApplyResources(this.DictionaryArea, "DictionaryArea");
            this.DictionaryArea.Name = "DictionaryArea";
            this.DictionaryArea.ZoomFactor = 1D;
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.SearchResultsArea, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // SearchResultsArea
            // 
            this.SearchResultsArea.AllowExternalDrop = false;
            this.SearchResultsArea.CreationProperties = null;
            this.SearchResultsArea.DefaultBackgroundColor = System.Drawing.Color.White;
            resources.ApplyResources(this.SearchResultsArea, "SearchResultsArea");
            this.SearchResultsArea.Name = "SearchResultsArea";
            this.SearchResultsArea.ZoomFactor = 1D;
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.btnCollapse, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnExpand, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.moreVersesBtn, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.fullScreenSearchBtn, 2, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // btnCollapse
            // 
            this.btnCollapse.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btnCollapse, "btnCollapse");
            this.btnCollapse.Image = global::Simple_Bible_Reader.Properties.Resources.collapse;
            this.btnCollapse.Name = "btnCollapse";
            this.BibleReaderToolTip.SetToolTip(this.btnCollapse, resources.GetString("btnCollapse.ToolTip"));
            this.btnCollapse.UseVisualStyleBackColor = false;
            this.btnCollapse.Click += new System.EventHandler(this.button3_Click);
            // 
            // btnExpand
            // 
            this.btnExpand.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btnExpand, "btnExpand");
            this.btnExpand.Image = global::Simple_Bible_Reader.Properties.Resources.expand;
            this.btnExpand.Name = "btnExpand";
            this.BibleReaderToolTip.SetToolTip(this.btnExpand, resources.GetString("btnExpand.ToolTip"));
            this.btnExpand.UseVisualStyleBackColor = false;
            this.btnExpand.Click += new System.EventHandler(this.button2_Click);
            // 
            // moreVersesBtn
            // 
            resources.ApplyResources(this.moreVersesBtn, "moreVersesBtn");
            this.moreVersesBtn.Checked = true;
            this.moreVersesBtn.CheckState = System.Windows.Forms.CheckState.Checked;
            this.moreVersesBtn.FlatAppearance.BorderSize = 0;
            this.moreVersesBtn.Image = global::Simple_Bible_Reader.Properties.Resources.more;
            this.moreVersesBtn.Name = "moreVersesBtn";
            this.BibleReaderToolTip.SetToolTip(this.moreVersesBtn, resources.GetString("moreVersesBtn.ToolTip"));
            this.moreVersesBtn.UseVisualStyleBackColor = false;
            this.moreVersesBtn.CheckedChanged += new System.EventHandler(this.moreVersesBtn_CheckedChanged);
            // 
            // fullScreenSearchBtn
            // 
            resources.ApplyResources(this.fullScreenSearchBtn, "fullScreenSearchBtn");
            this.fullScreenSearchBtn.FlatAppearance.BorderSize = 0;
            this.fullScreenSearchBtn.Image = global::Simple_Bible_Reader.Properties.Resources.fullscreen;
            this.fullScreenSearchBtn.Name = "fullScreenSearchBtn";
            this.BibleReaderToolTip.SetToolTip(this.fullScreenSearchBtn, resources.GetString("fullScreenSearchBtn.ToolTip"));
            this.fullScreenSearchBtn.UseVisualStyleBackColor = false;
            this.fullScreenSearchBtn.CheckedChanged += new System.EventHandler(this.fullScreenSearchBtn_CheckedChanged);
            // 
            // MenuStrip1
            // 
            resources.ApplyResources(this.MenuStrip1, "MenuStrip1");
            this.MenuStrip1.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.MenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.MenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem1,
            this.editToolStripMenuItem,
            this.layoutToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.HelpToolStripMenuItem});
            this.MenuStrip1.Name = "MenuStrip1";
            // 
            // ToolStripMenuItem1
            // 
            this.ToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OpenToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.exportBibleToolStripMenuItem,
            this.toolStripSeparator6,
            this.openBookToolStripMenuItem,
            this.exportBookToolStripMenuItem,
            this.toolStripSeparator3,
            this.openCommentaryToolStripMenuItem,
            this.exportCommentaryToolStripMenuItem,
            this.toolStripSeparator4,
            this.openDictionaryToolStripMenuItem,
            this.exportDictionaryToolStripMenuItem,
            this.toolStripSeparator5,
            this.batchConvertToolStripMenuItem,
            this.toolStripSeparator7,
            this.RTL,
            this.ExitToolStripMenuItem});
            this.ToolStripMenuItem1.Name = "ToolStripMenuItem1";
            resources.ApplyResources(this.ToolStripMenuItem1, "ToolStripMenuItem1");
            // 
            // OpenToolStripMenuItem
            // 
            this.OpenToolStripMenuItem.Name = "OpenToolStripMenuItem";
            resources.ApplyResources(this.OpenToolStripMenuItem, "OpenToolStripMenuItem");
            this.OpenToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            resources.ApplyResources(this.saveToolStripMenuItem, "saveToolStripMenuItem");
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // exportBibleToolStripMenuItem
            // 
            this.exportBibleToolStripMenuItem.Name = "exportBibleToolStripMenuItem";
            resources.ApplyResources(this.exportBibleToolStripMenuItem, "exportBibleToolStripMenuItem");
            this.exportBibleToolStripMenuItem.Click += new System.EventHandler(this.exportBibleToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            resources.ApplyResources(this.toolStripSeparator6, "toolStripSeparator6");
            // 
            // openBookToolStripMenuItem
            // 
            this.openBookToolStripMenuItem.Name = "openBookToolStripMenuItem";
            resources.ApplyResources(this.openBookToolStripMenuItem, "openBookToolStripMenuItem");
            this.openBookToolStripMenuItem.Click += new System.EventHandler(this.openBookToolStripMenuItem_Click);
            // 
            // exportBookToolStripMenuItem
            // 
            this.exportBookToolStripMenuItem.Name = "exportBookToolStripMenuItem";
            resources.ApplyResources(this.exportBookToolStripMenuItem, "exportBookToolStripMenuItem");
            this.exportBookToolStripMenuItem.Click += new System.EventHandler(this.exportBookToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // openCommentaryToolStripMenuItem
            // 
            this.openCommentaryToolStripMenuItem.Name = "openCommentaryToolStripMenuItem";
            resources.ApplyResources(this.openCommentaryToolStripMenuItem, "openCommentaryToolStripMenuItem");
            this.openCommentaryToolStripMenuItem.Click += new System.EventHandler(this.openCommentaryToolStripMenuItem_Click);
            // 
            // exportCommentaryToolStripMenuItem
            // 
            this.exportCommentaryToolStripMenuItem.Name = "exportCommentaryToolStripMenuItem";
            resources.ApplyResources(this.exportCommentaryToolStripMenuItem, "exportCommentaryToolStripMenuItem");
            this.exportCommentaryToolStripMenuItem.Click += new System.EventHandler(this.exportCommentaryToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // openDictionaryToolStripMenuItem
            // 
            this.openDictionaryToolStripMenuItem.Name = "openDictionaryToolStripMenuItem";
            resources.ApplyResources(this.openDictionaryToolStripMenuItem, "openDictionaryToolStripMenuItem");
            this.openDictionaryToolStripMenuItem.Click += new System.EventHandler(this.openDictionaryToolStripMenuItem_Click);
            // 
            // exportDictionaryToolStripMenuItem
            // 
            this.exportDictionaryToolStripMenuItem.Name = "exportDictionaryToolStripMenuItem";
            resources.ApplyResources(this.exportDictionaryToolStripMenuItem, "exportDictionaryToolStripMenuItem");
            this.exportDictionaryToolStripMenuItem.Click += new System.EventHandler(this.exportDictionaryToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
            // 
            // batchConvertToolStripMenuItem
            // 
            this.batchConvertToolStripMenuItem.Name = "batchConvertToolStripMenuItem";
            resources.ApplyResources(this.batchConvertToolStripMenuItem, "batchConvertToolStripMenuItem");
            this.batchConvertToolStripMenuItem.Click += new System.EventHandler(this.batchConvertToolStripMenuItem_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            resources.ApplyResources(this.toolStripSeparator7, "toolStripSeparator7");
            // 
            // RTL
            // 
            this.RTL.CheckOnClick = true;
            this.RTL.Name = "RTL";
            resources.ApplyResources(this.RTL, "RTL");
            this.RTL.Click += new System.EventHandler(this.RTL_Click);
            // 
            // ExitToolStripMenuItem
            // 
            this.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem";
            resources.ApplyResources(this.ExitToolStripMenuItem, "ExitToolStripMenuItem");
            this.ExitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.findToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            resources.ApplyResources(this.editToolStripMenuItem, "editToolStripMenuItem");
            // 
            // findToolStripMenuItem
            // 
            this.findToolStripMenuItem.Name = "findToolStripMenuItem";
            resources.ApplyResources(this.findToolStripMenuItem, "findToolStripMenuItem");
            this.findToolStripMenuItem.Click += new System.EventHandler(this.findToolStripMenuItem_Click);
            // 
            // layoutToolStripMenuItem
            // 
            this.layoutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.booksToolStripMenuItem,
            this.bibleToolStripMenuItem,
            this.commentaryToolStripMenuItem,
            this.dictionaryToolStripMenuItem,
            this.searchToolStripMenuItem});
            this.layoutToolStripMenuItem.Name = "layoutToolStripMenuItem";
            resources.ApplyResources(this.layoutToolStripMenuItem, "layoutToolStripMenuItem");
            // 
            // booksToolStripMenuItem
            // 
            this.booksToolStripMenuItem.Checked = true;
            this.booksToolStripMenuItem.CheckOnClick = true;
            this.booksToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.booksToolStripMenuItem.Name = "booksToolStripMenuItem";
            resources.ApplyResources(this.booksToolStripMenuItem, "booksToolStripMenuItem");
            this.booksToolStripMenuItem.Click += new System.EventHandler(this.booksToolStripMenuItem_Click);
            // 
            // bibleToolStripMenuItem
            // 
            this.bibleToolStripMenuItem.Checked = true;
            this.bibleToolStripMenuItem.CheckOnClick = true;
            this.bibleToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.bibleToolStripMenuItem.Name = "bibleToolStripMenuItem";
            resources.ApplyResources(this.bibleToolStripMenuItem, "bibleToolStripMenuItem");
            this.bibleToolStripMenuItem.Click += new System.EventHandler(this.bibleToolStripMenuItem_Click);
            // 
            // commentaryToolStripMenuItem
            // 
            this.commentaryToolStripMenuItem.CheckOnClick = true;
            this.commentaryToolStripMenuItem.Name = "commentaryToolStripMenuItem";
            resources.ApplyResources(this.commentaryToolStripMenuItem, "commentaryToolStripMenuItem");
            this.commentaryToolStripMenuItem.Click += new System.EventHandler(this.commentaryToolStripMenuItem_Click);
            // 
            // dictionaryToolStripMenuItem
            // 
            this.dictionaryToolStripMenuItem.CheckOnClick = true;
            this.dictionaryToolStripMenuItem.Name = "dictionaryToolStripMenuItem";
            resources.ApplyResources(this.dictionaryToolStripMenuItem, "dictionaryToolStripMenuItem");
            this.dictionaryToolStripMenuItem.Click += new System.EventHandler(this.dictionaryToolStripMenuItem_Click);
            // 
            // searchToolStripMenuItem
            // 
            this.searchToolStripMenuItem.CheckOnClick = true;
            this.searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            resources.ApplyResources(this.searchToolStripMenuItem, "searchToolStripMenuItem");
            this.searchToolStripMenuItem.Click += new System.EventHandler(this.searchToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.preferencesToolStripMenuItem,
            this.pluginsToolStripMenuItem,
            this.localizationsToolStripMenuItem,
            this.themesToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            resources.ApplyResources(this.settingsToolStripMenuItem, "settingsToolStripMenuItem");
            // 
            // preferencesToolStripMenuItem
            // 
            this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
            resources.ApplyResources(this.preferencesToolStripMenuItem, "preferencesToolStripMenuItem");
            this.preferencesToolStripMenuItem.Click += new System.EventHandler(this.preferencesToolStripMenuItem_Click);
            // 
            // pluginsToolStripMenuItem
            // 
            this.pluginsToolStripMenuItem.Name = "pluginsToolStripMenuItem";
            resources.ApplyResources(this.pluginsToolStripMenuItem, "pluginsToolStripMenuItem");
            this.pluginsToolStripMenuItem.Click += new System.EventHandler(this.pluginsToolStripMenuItem_Click);
            // 
            // localizationsToolStripMenuItem
            // 
            this.localizationsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.englishToolStripMenuItem,
            this.spanishToolStripMenuItem,
            this.tamilToolStripMenuItem,
            this.portuguesToolStripMenuItem});
            this.localizationsToolStripMenuItem.Name = "localizationsToolStripMenuItem";
            resources.ApplyResources(this.localizationsToolStripMenuItem, "localizationsToolStripMenuItem");
            // 
            // englishToolStripMenuItem
            // 
            this.englishToolStripMenuItem.Name = "englishToolStripMenuItem";
            resources.ApplyResources(this.englishToolStripMenuItem, "englishToolStripMenuItem");
            this.englishToolStripMenuItem.Click += new System.EventHandler(this.englishToolStripMenuItem_Click);
            // 
            // spanishToolStripMenuItem
            // 
            this.spanishToolStripMenuItem.Name = "spanishToolStripMenuItem";
            resources.ApplyResources(this.spanishToolStripMenuItem, "spanishToolStripMenuItem");
            this.spanishToolStripMenuItem.Click += new System.EventHandler(this.spanishToolStripMenuItem_Click);
            // 
            // tamilToolStripMenuItem
            // 
            this.tamilToolStripMenuItem.Name = "tamilToolStripMenuItem";
            resources.ApplyResources(this.tamilToolStripMenuItem, "tamilToolStripMenuItem");
            this.tamilToolStripMenuItem.Click += new System.EventHandler(this.tamilToolStripMenuItem_Click);
            // 
            // portuguesToolStripMenuItem
            // 
            this.portuguesToolStripMenuItem.Name = "portuguesToolStripMenuItem";
            resources.ApplyResources(this.portuguesToolStripMenuItem, "portuguesToolStripMenuItem");
            this.portuguesToolStripMenuItem.Click += new System.EventHandler(this.portuguesToolStripMenuItem_Click);
            // 
            // themesToolStripMenuItem
            // 
            this.themesToolStripMenuItem.Name = "themesToolStripMenuItem";
            resources.ApplyResources(this.themesToolStripMenuItem, "themesToolStripMenuItem");
            // 
            // HelpToolStripMenuItem
            // 
            this.HelpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.WebsiteToolStripMenuItem,
            this.releaseNotesToolStripMenuItem,
            this.creditsToolStripMenuItem,
            this.AboutToolStripMenuItem});
            this.HelpToolStripMenuItem.Name = "HelpToolStripMenuItem";
            resources.ApplyResources(this.HelpToolStripMenuItem, "HelpToolStripMenuItem");
            // 
            // WebsiteToolStripMenuItem
            // 
            this.WebsiteToolStripMenuItem.Name = "WebsiteToolStripMenuItem";
            resources.ApplyResources(this.WebsiteToolStripMenuItem, "WebsiteToolStripMenuItem");
            this.WebsiteToolStripMenuItem.Click += new System.EventHandler(this.WebsiteToolStripMenuItem_Click);
            // 
            // releaseNotesToolStripMenuItem
            // 
            this.releaseNotesToolStripMenuItem.Name = "releaseNotesToolStripMenuItem";
            resources.ApplyResources(this.releaseNotesToolStripMenuItem, "releaseNotesToolStripMenuItem");
            this.releaseNotesToolStripMenuItem.Click += new System.EventHandler(this.releaseNotesToolStripMenuItem_Click);
            // 
            // creditsToolStripMenuItem
            // 
            this.creditsToolStripMenuItem.Name = "creditsToolStripMenuItem";
            resources.ApplyResources(this.creditsToolStripMenuItem, "creditsToolStripMenuItem");
            this.creditsToolStripMenuItem.Click += new System.EventHandler(this.creditsToolStripMenuItem_Click);
            // 
            // AboutToolStripMenuItem
            // 
            this.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem";
            resources.ApplyResources(this.AboutToolStripMenuItem, "AboutToolStripMenuItem");
            this.AboutToolStripMenuItem.Click += new System.EventHandler(this.AboutToolStripMenuItem_Click);
            // 
            // BackgroundWorker1
            // 
            this.BackgroundWorker1.WorkerReportsProgress = true;
            this.BackgroundWorker1.WorkerSupportsCancellation = true;
            this.BackgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorker1_DoWork);
            this.BackgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackgroundWorker1_RunWorkerCompleted);
            // 
            // Timer1
            // 
            this.Timer1.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // backgroundWorker2
            // 
            this.backgroundWorker2.WorkerReportsProgress = true;
            this.backgroundWorker2.WorkerSupportsCancellation = true;
            this.backgroundWorker2.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker2_DoWork);
            this.backgroundWorker2.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker2_RunWorkerCompleted);
            // 
            // timer2
            // 
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // timerSelectVerse
            // 
            this.timerSelectVerse.Tick += new System.EventHandler(this.timerSelectVerse_Tick);
            // 
            // timer5
            // 
            this.timer5.Tick += new System.EventHandler(this.timer5_Tick);
            // 
            // backgroundCleaner
            // 
            this.backgroundCleaner.WorkerReportsProgress = true;
            this.backgroundCleaner.WorkerSupportsCancellation = true;
            this.backgroundCleaner.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundCleaner_DoWork);
            this.backgroundCleaner.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundCleaner_ProgressChanged);
            this.backgroundCleaner.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundCleaner_RunWorkerCompleted);
            // 
            // backgroundWorker3
            // 
            this.backgroundWorker3.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker3_DoWork);
            this.backgroundWorker3.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker3_RunWorkerCompleted);
            // 
            // timer6
            // 
            this.timer6.Tick += new System.EventHandler(this.timer6_Tick);
            // 
            // printPreviewDialog1
            // 
            resources.ApplyResources(this.printPreviewDialog1, "printPreviewDialog1");
            this.printPreviewDialog1.Name = "printPreviewDialog1";
            // 
            // bkgndTransLitJob
            // 
            this.bkgndTransLitJob.WorkerReportsProgress = true;
            this.bkgndTransLitJob.WorkerSupportsCancellation = true;
            this.bkgndTransLitJob.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bkgndTransLitJob_DoWork);
            this.bkgndTransLitJob.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bkgndTransLitJob_ProgressChanged);
            this.bkgndTransLitJob.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bkgndTransLitJob_RunWorkerCompleted);
            // 
            // speechBkgndWorker
            // 
            this.speechBkgndWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.speechBkgndWorker_DoWork);
            // 
            // SimpleBibleReader
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.ToolStripContainer1);
            this.Name = "SimpleBibleReader";
            this.Load += new System.EventHandler(this.SimpleBibleReader_Load);
            this.ToolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
            this.ToolStripContainer1.BottomToolStripPanel.PerformLayout();
            this.ToolStripContainer1.ContentPanel.ResumeLayout(false);
            this.ToolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.ToolStripContainer1.TopToolStripPanel.PerformLayout();
            this.ToolStripContainer1.ResumeLayout(false);
            this.ToolStripContainer1.PerformLayout();
            this.StatusStrip1.ResumeLayout(false);
            this.StatusStrip1.PerformLayout();
            this.SplitContainer1.Panel1.ResumeLayout(false);
            this.SplitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer1)).EndInit();
            this.SplitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFront)).EndInit();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ChapterArea)).EndInit();
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            this.splitContainer5.Panel1.ResumeLayout(false);
            this.splitContainer5.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer5)).EndInit();
            this.splitContainer5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.CommentaryArea)).EndInit();
            this.tableLayoutPanel7.ResumeLayout(false);
            this.tableLayoutPanel7.PerformLayout();
            this.tableLayoutPanel8.ResumeLayout(false);
            this.tableLayoutPanel8.PerformLayout();
            this.splitContainer6.Panel1.ResumeLayout(false);
            this.splitContainer6.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer6)).EndInit();
            this.splitContainer6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DictionaryArea)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SearchResultsArea)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.MenuStrip1.ResumeLayout(false);
            this.MenuStrip1.PerformLayout();
            this.ResumeLayout(false);

		}


        internal System.Windows.Forms.ToolStripContainer ToolStripContainer1;
		internal System.Windows.Forms.SplitContainer SplitContainer1;
		internal System.Windows.Forms.MenuStrip MenuStrip1;
		internal System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem1;
		internal System.Windows.Forms.ToolStripMenuItem ExitToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem OpenToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem HelpToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem AboutToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem RTL;
        internal System.Windows.Forms.ToolStripMenuItem WebsiteToolStripMenuItem;
		internal System.Windows.Forms.ToolStripStatusLabel StatusText;
        internal System.Windows.Forms.ToolStripProgressBar ProgressBar;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem findToolStripMenuItem;
        private System.ComponentModel.IContainer components;
        private ToolStripMenuItem settingsToolStripMenuItem;
        internal System.ComponentModel.BackgroundWorker BackgroundWorker1;
        internal PictureBox pictureBoxFront;
        internal StatusStrip StatusStrip1;
        internal ToolStripMenuItem exportBibleToolStripMenuItem;
        internal MirrorTreeView MirrorTreeView1;
        internal Timer Timer1;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem openCommentaryToolStripMenuItem;
        private ToolStripMenuItem exportCommentaryToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator4;
        internal System.ComponentModel.BackgroundWorker backgroundWorker2;
        private Timer timer2;
        private SplitContainer splitContainer2;
        private SplitContainer splitContainer3;
        private SplitContainer splitContainer4;
        private ToolStripMenuItem layoutToolStripMenuItem;
        private ToolStripMenuItem booksToolStripMenuItem;
        private ToolStripMenuItem bibleToolStripMenuItem;
        private ToolStripMenuItem commentaryToolStripMenuItem;
        private ToolStripMenuItem dictionaryToolStripMenuItem;
        private TableLayoutPanel tableLayoutPanel1;
        private WebView2 SearchResultsArea;
        private TableLayoutPanel tableLayoutPanel2;
        private Button btnExpand;
        private Button btnCollapse;
        private TableLayoutPanel tableLayoutPanel3;
        internal WebView2 ChapterArea;
        private TableLayoutPanel tableLayoutPanel4;
        private Button frontBtn;
        private Button zoomInBtn;
        private Button zoomOutBtn;
        private Button backBtn;
        private Button findBtn;
        public ToolStripMenuItem searchToolStripMenuItem;
        private CheckBox fullScreenBtn;
        private CheckBox fullScreenSearchBtn;
        private Button printChapterBtn;
        public CheckBox moreVersesBtn;
        private Timer timerSelectVerse;
        private Timer timer5;
        private ToolStripMenuItem preferencesToolStripMenuItem;
        private ToolStripMenuItem releaseNotesToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker backgroundCleaner;
        private CheckBox paraBtn;
        private Button fontBtn;
        private TableLayoutPanel tableLayoutPanel5;
        private TableLayoutPanel tableLayoutPanel6;
        private CheckBox chkBoxShowList;
        private Button cmtZoomInBtn;
        private CheckBox cmtFullScreenBtn;
        private Button cmtZoomOutBtn;
        private Button cmtPrintBtn;
        private SplitContainer splitContainer5;
        private ListBox cmtListBox;
        private WebView2 CommentaryArea;
        private TableLayoutPanel tableLayoutPanel7;
        private TableLayoutPanel tableLayoutPanel8;
        private Label dictionaryTitleLbl;
        private CheckBox chkBoxShowDictList;
        private Button dictZoomInBtn;
        private CheckBox dctFullScreenBtn;
        private Button dictZoomOutBtn;
        private Button dictPrintBtn;
        private SplitContainer splitContainer6;
        private ListBox dictListBox;
        private WebView2 DictionaryArea;
        private ToolStripMenuItem openDictionaryToolStripMenuItem;
        private ToolStripMenuItem exportDictionaryToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator5;
        private Button cmtFontBtn;
        private OpenFileDialog readerOpenDialog;
        private SaveFileDialog readerSaveDialog;
        private System.ComponentModel.BackgroundWorker backgroundWorker3;
        private Timer timer6;
        private Button editBtn;
        private Label commentaryTitleLbl;
        private Button cmtEditBtn;
        private Button dictEditBtn;
        private Button dctFontBtn;
        private PrintPreviewDialog printPreviewDialog1;
        private Button helpBtn;
        private ToolTip BibleReaderToolTip;
        private CheckBox transliterateBtn;
        private System.ComponentModel.BackgroundWorker bkgndTransLitJob;
        private Button statBtn;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripMenuItem openBookToolStripMenuItem;
        private ToolStripMenuItem exportBookToolStripMenuItem;
        private ToolStripMenuItem batchConvertToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator7;
        internal ToolStripMenuItem pluginsToolStripMenuItem;
        private ToolStripMenuItem localizationsToolStripMenuItem;
        private ToolStripMenuItem englishToolStripMenuItem;
        private ToolStripMenuItem tamilToolStripMenuItem;
        private ToolStripMenuItem spanishToolStripMenuItem;
        private ToolStripMenuItem creditsToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker speechBkgndWorker;
        private Label bibleTitleLbl;
        private Button btnBible;
        private ToolStripMenuItem portuguesToolStripMenuItem;
        private ToolStripMenuItem themesToolStripMenuItem;
        private Button btnPlugin;
    }
	
}
