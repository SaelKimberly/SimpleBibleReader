using System.Drawing;
using System.Diagnostics;
using System;
using System.Windows.Forms;
using System.Text;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;
using System.Security.Policy;
using Simple_Bible_Reader.Properties;
using System.Security.Cryptography;
using DarkUI.Config;

namespace Simple_Bible_Reader
{
	public partial class RendererWebView2 : System.Windows.Forms.Form
	{
		public RendererWebView2()
		{
            GlobalMemory.getInstance().Locale = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName; // Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.ToLower();
            if(GlobalMemory.getInstance().Locale!="en")
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(GlobalMemory.getInstance().Locale);            
			InitializeComponent();

            //Added to support default instance behavour in C#
            if (defaultInstance == null)
				defaultInstance = this;
		}
		
#region Default Instance
		
		private static RendererWebView2 defaultInstance;
		
		/// <summary>
		/// Added by the VB.Net to C# Converter to support default instance behavour in C#
		/// </summary>
		public static RendererWebView2 Default
		{
			get
			{
				if (defaultInstance == null)
				{
					defaultInstance = new RendererWebView2();
					defaultInstance.FormClosed += new FormClosedEventHandler(defaultInstance_FormClosed);
				}
				
				return defaultInstance;
			}
		}

        protected override void WndProc(ref Message m)
        {
            DarkMode.DarkMode.WndProc(this, m, GlobalMemory.getInstance().Theme);
            base.WndProc(ref m);
        }

        static void defaultInstance_FormClosed(object sender, FormClosedEventArgs e)
		{
			defaultInstance = null;
		}
		
#endregion
		
		const int WS_EX_LAYOUTRTL = 0x400000;
		const int WS_EX_NOINHERITLAYOUT = 0x100000;
		
		protected override System.Windows.Forms.CreateParams CreateParams
		{
			get
			{
				System.Windows.Forms.CreateParams CP = base.CreateParams;
				if (! base.DesignMode)
				{
					if (System.Convert.ToBoolean(this.RightToLeft))
					{
						CP.ExStyle = CP.ExStyle | WS_EX_LAYOUTRTL | WS_EX_NOINHERITLAYOUT;
					}
				}
				return CP;
			}
		}

        bool is_bible = true;
        //-------------------------Bibles        
        string biblefile = null;
        bool exporting_bible = false;
        string biblefile_only = null;
        //-------------------------Books        
        string bookfile = null;
        bool exporting_book = false;
        //-------------------------Commentary
        string commentaryfile = null;
        bool exporting_commentary = false;
        //-------------------------Dictionary
        string dictionaryfile = null;
        bool exporting_dictionary = false;
        //-------------------------

        /*
         * Post-open flags (used only for Executable Bibles 
         * to do post operations after populating docxml)
         */
        public bool post_open_cmtry = false;
        public bool post_open_dict = false;
        //----------------------------------------------
        /*
         * Used for trying to open commentary and then dict if bible fails.
         */
        public bool try_cmtry = false;
        public bool try_dict = false;
        public bool try_book = false;

        FontFrm fontDialog = null;
        PropertiesFrm propDialog = null;
        PreferencesFrm prefDialog = null;
        BatchModeFrm batchDialog = null;
        Plugins2Frm pluginsFrm = null;

        bool programatic_action = false;
        List<string> versionHistory = new List<string>();
        int versionHistoryCounter = 0;

        FindWebView2 find = null;

        public string[] data;
        string formatsSupportedHtml = null;
        string export_filename = null;
        int export_filter_idx = 1;
        string verse_to_highlight = "1";
        

        string selected_verse="";
        string selected_2verses = "";
        string selected_3verses = "";
        XmlNode selected_chapter_elmt = null;
        TreeNode last_selected_node = null;       
        XmlNode xml_verse_elmt = null;
        string verse_ref = null;
        bool commented = false;
        bool form_closing = false;

        BibleFormat bible_format_export;
        CommentaryFormat cmtry_format_export;
        BookFormat book_format_export;
        DictionaryFormat dict_format_export;

        bool prev_s1p1 = false;
        bool prev_s1p2 = false;
        bool prev_s2p1 = false;
        bool prev_s2p2 = true;
        bool prev_s3p1 = false;
        bool prev_s3p2 = true;
        bool prev_s4p1 = false; // commentary
        bool prev_s4p2 = false; // dictionary
        bool prev_s5p1 = false;
        bool prev_s5p2 = false;
        bool prev_s6p1 = false;
        bool prev_s6p2 = false;

        int currentSelectedBook = 1, currentSelectedChapter = 1, currentSelectedVerse = 1;

		public void ExitToolStripMenuItem_Click(System.Object sender, System.EventArgs e)
		{
			Application.Exit();
		}
		
		public void OpenToolStripMenuItem_Click(System.Object sender, System.EventArgs e)
		{
            readerOpenDialog.Filter = BibleFormat.getFilters();            
            //
            int result = (int)(readerOpenDialog.ShowDialog());
			if (result == System.Convert.ToInt32(System.Windows.Forms.DialogResult.OK))
			{
                biblefile = readerOpenDialog.FileName;
                OpenBible();                
			}
		}
		
		public void OpenBible()
		{
            is_bible = true;
            if(backgroundCleaner.IsBusy)
                backgroundCleaner.CancelAsync();
            if (exporting_bible)
            {
                ChapterArea.NavigateToString("<html><head>" + Themes.CSS_UI + "</head><body><h3>Opening Bible during export is disabled</h3><p>You can read the Bible during export, but opening another is not possible. If you wish to cancel the export, just close and re-open this software or open another instance of the same software.</p></body></html>");
                return;
            }
            if (BackgroundWorker1.IsBusy)
            {
                ChapterArea.NavigateToString("<html><head>" + Themes.CSS_UI + "</head><body><h3>Bible is already being opened</h3><p>Please be patient as another Bible is being opened. Depending on the format, it may take a while.</p></body></html>");
                return;
            }
            pictureBoxFront.Visible = false;
			Timer1.Enabled = true;
			StatusStrip1.Visible = true;
			MirrorTreeView1.Nodes.Clear();
            ChapterArea.NavigateToString("<html><head>" + Themes.CSS_UI + "</head><body/></html>");
            exportBibleToolStripMenuItem.Enabled = false;
             exportBookToolStripMenuItem.Enabled = false;
            findToolStripMenuItem.Enabled = false;
            paraBtn.Enabled = true;
            findBtn.Enabled = false;
            fontBtn.Enabled = false;
            statBtn.Enabled = false;
            saveToolStripMenuItem.Enabled = false;
            biblefile_only=System.IO.Path.GetFileName(biblefile);
            exporting_bible = false;
            commented = false;
            bibleTitleLbl.Visible = true;
			BackgroundWorker1.RunWorkerAsync();
		}
		
		public void MirrorTreeView1_AfterSelect(System.Object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			MirrorTreeView tv = (MirrorTreeView) sender;     
            if(is_bible)
                selectTreeViewVerse(tv);  
            else
                selectTreeViewNote(tv);  
		}

        private void selectTreeViewVerse(MirrorTreeView tv)
        {
            string[] data = tv.SelectedNode.FullPath.Split(tv.PathSeparator.ToCharArray());
            if (data.Length == 3)
            {
                if (!programatic_action)
                {
                    versionHistoryCounter = versionHistory.Count - 1;
                    versionHistory.Add(tv.SelectedNode.FullPath);
                    backBtn.Enabled = true;
                }
            }
            selectTreeViewVerse(data);
        }

        private void selectTreeViewVerse(string[] data)
        {            
            if (data.Length == 3)
            {
                if (RTL.Checked)
                {
                    if (BibleFormat.getInstance() != null)
                        ChapterArea.NavigateToString((string)(BibleFormat.getInstance().getChapterHtml(data, paraBtn.Checked, CommentaryFormat.getInstance()).Replace("dir=\'ltr\'", "dir=\'rtl\'")));                    
                }
                else
                {
                    if (BibleFormat.getInstance() != null)
                        ChapterArea.NavigateToString((string)(BibleFormat.getInstance().getChapterHtml(data, paraBtn.Checked, CommentaryFormat.getInstance()).Replace("dir=\'rtl\'", "dir=\'ltr\'")));                    
                }
                last_selected_node = MirrorTreeView1.SelectedNode;
                setTitles(data);
            }
        }

        private void selectTreeViewNote(MirrorTreeView tv)
        {
            string[] data = tv.SelectedNode.FullPath.Split(tv.PathSeparator.ToCharArray());
            selectTreeViewNote(data);
        }

        private void selectTreeViewNote(string[] data)
        {
            if (RTL.Checked)
            {
                if (BookFormat.getInstance() != null)
                    ChapterArea.NavigateToString((string)(BookFormat.getInstance().getChapterHtml(data).Replace("dir=\'ltr\'", "dir=\'rtl\'")));
            }
            else
            {
                if (BookFormat.getInstance() != null)
                    ChapterArea.NavigateToString((string)(BookFormat.getInstance().getChapterHtml(data).Replace("dir=\'rtl\'", "dir=\'ltr\'")));
            }
            last_selected_node = MirrorTreeView1.SelectedNode;
        }

        public void setTitles()
        {
            setTitles(this.data);
        }
        

        public void setTitles(string[] data)
        {
            if (BibleFormat.getInstance() != null)
            {
                if(data!=null)
                    bibleTitleLbl.Text = "[" + BibleFormat.getInstance().ABBREVIATION + "] " + data[1] + " " + data[2];
                else
                    bibleTitleLbl.Text = "[" + BibleFormat.getInstance().ABBREVIATION + "]";
            }
            if (CommentaryFormat.getInstance() != null)
            {
                commentaryTitleLbl.Text = "[" + CommentaryFormat.getInstance().ABBREVIATION + "]";                
            }
            if (DictionaryFormat.getInstance() != null)
            {
                dictionaryTitleLbl.Text = "[" + DictionaryFormat.getInstance().ABBREVIATION + "]";
            }
        }

		public void RTL_Click(System.Object sender, System.EventArgs e)
		{
            if (RTL.Checked)
            {
                /*
                ChapterArea.NavigateToString(getWebView2Html(ChapterArea).Replace("dir=\'ltr\'", "dir=\'rtl\'"));
                SearchResultsArea.NavigateToString(getWebView2Html(SearchResultsArea).Replace("dir=\'ltr\'", "dir=\'rtl\'"));
                CommentaryArea.NavigateToString(getWebView2Html(CommentaryArea).Replace("dir=\'ltr\'", "dir=\'rtl\'"));
                DictionaryArea.NavigateToString(getWebView2Html(DictionaryArea).Replace("dir=\'ltr\'", "dir=\'rtl\'"));
                */
                changeDirection(ChapterArea, "rtl");
                changeDirection(SearchResultsArea, "rtl");
                changeDirection(CommentaryArea, "rtl");
                changeDirection(DictionaryArea, "rtl");


                ToolStripContainer1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
                MirrorTreeView1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
                MirrorTreeView1.Mirrored = true;
                MirrorTreeView1.RightToLeftLayout = true;
                this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
                this.RightToLeftLayout = true;

                GlobalMemory.getInstance().Direction = GlobalMemory.DIRECTION_RTL;
            }
            else
            {
                /*
                ChapterArea.NavigateToString(getWebView2Html(ChapterArea).Replace("dir=\'rtl\'", "dir=\'ltr\'"));
                SearchResultsArea.NavigateToString(getWebView2Html(SearchResultsArea).Replace("dir=\'rtl\'", "dir=\'ltr\'"));
                CommentaryArea.NavigateToString(getWebView2Html(CommentaryArea).Replace("dir=\'rtl\'", "dir=\'ltr\'"));
                DictionaryArea.NavigateToString(getWebView2Html(DictionaryArea).Replace("dir=\'rtl\'", "dir=\'ltr\'"));
                */
                changeDirection(ChapterArea, "ltr");
                changeDirection(SearchResultsArea, "ltr");
                changeDirection(CommentaryArea, "ltr");
                changeDirection(DictionaryArea, "ltr");

                ToolStripContainer1.RightToLeft = System.Windows.Forms.RightToLeft.No;
                MirrorTreeView1.RightToLeft = System.Windows.Forms.RightToLeft.No;
                MirrorTreeView1.Mirrored = false;
                MirrorTreeView1.RightToLeftLayout = false;
                this.RightToLeft = System.Windows.Forms.RightToLeft.No;
                this.RightToLeftLayout = false;

                GlobalMemory.getInstance().Direction = GlobalMemory.DIRECTION_LTR;
            }      
		}

        public void changeFont(WebView2 wv, string fontfamily)
        {
            string changeFontJS = @"
 var body = document.getElementsByTagName('body')[0];
body.style.fontFamily ='"+ fontfamily + "';";
            wv.ExecuteScriptAsync(changeFontJS);
        }

        public void searchResultCollapse(WebView2 wv)
        {
            string collapseJS = @"
var input = document.getElementsByTagName('li')
var inputList = Array.prototype.slice.call(input);

inputList.forEach(result => {
if(result.className == 'Expanded')
  result.className = 'Collapsed';
});";
            wv.ExecuteScriptAsync(collapseJS);
        }

        public void searchResultExpand(WebView2 wv)
        {
            string expandJS = @"
var input = document.getElementsByTagName('li')
var inputList = Array.prototype.slice.call(input);

inputList.forEach(result => {
if(result.className == 'Collapsed')
  result.className = 'Expanded';
});";
            wv.ExecuteScriptAsync(expandJS);
        }

        public void changeDirection(WebView2 wv, string direction)
        {
            wv.ExecuteScriptAsync("document.body.dir = \""+direction+"\";");            
        }

        public void AboutToolStripMenuItem_Click(System.Object sender, System.EventArgs e)
		{
            string[][] licenses = SbrInfo.LICENSES;

            StringBuilder sb = new StringBuilder();
            sb.Append("<html><head>");
            sb.Append(Themes.CSS_UI);
            sb.Append("</head><body><h2>");
            sb.Append(Application.ProductName + " " + Application.ProductVersion);
            sb.Append("</h2>");
            sb.Append("<h3>AUTHOR</h3>");
            sb.Append("The software is developed by Felix Immanuel. You can contact him using <a style=\'text-decoration:none\' href=\'https://trumpet-call.org/contact/\'>https://trumpet-call.org/contact/</a>.");
            sb.Append("<h3>UNLICENSE</h3>");
            sb.Append(Localization.MainPage_About_PublicDomain);
            sb.Append("<h3>LICENSES</h3>");
            sb.Append("<p>The source code for Simple Bible Reader is Public Domain. Simple Bible Reader uses the below third party software with their own licenses.</p>");

            sb.Append("<table width='90%'>");

            for (int i = 0; i < licenses.Length; i++)
            {
                sb.Append("<tr><td>");
                if (licenses[i][1] == "")
                    sb.Append(licenses[i][0]);
                else
                    sb.Append("<a href='" + licenses[i][1] + "'>" + licenses[i][0] + "</a>");
                sb.Append("</td><td>" + licenses[i][2] + "</td>");
                sb.Append("</td><td>" + licenses[i][3] + "</td>");
                sb.Append("<tr><td colspan='4'><hr></td><tr>");
            }
            sb.Append("</table>");

            sb.Append(GetFormatsSupported());
            sb.Append("</body></html>");
            ChapterArea.NavigateToString(sb.ToString());
        }

        public string GetFormatsSupported()
        {
            if (formatsSupportedHtml == null)
            {
                StringBuilder sb = new StringBuilder();
                // Bible Formats
                sb.Append("<h3>" + Localization.MainPage_SupportedBibleFormats + ":</h3><p align='justify'>");
                sb.Append(Localization.MainPage_BibleFormats); 
                sb.Append("</p>");
                // Commentary Formats
                sb.Append("<h3>" + Localization.MainPage_SupportedCommentaryFormats + ":</h3><p align='justify'>");
                sb.Append(Localization.MainPage_CommentaryFormats);
                sb.Append("</p>");
                // Dictionary Formats
                sb.Append("<h3>" + Localization.MainPage_SupportedDictionaryFormats + ":</h3><p align='justify'>");
                sb.Append(Localization.MainPage_DictionaryFormats);
                sb.Append("</p>");
                // Book Formats
                sb.Append("<h3>" + Localization.MainPage_SupportedBookFormats + ":</h3><p align='justify'>");
                sb.Append(Localization.MainPage_BookFormats);
                sb.Append("</p>");
                formatsSupportedHtml = sb.ToString();
            }
            return formatsSupportedHtml;
        }
		
		public void WebsiteToolStripMenuItem_Click(System.Object sender, System.EventArgs e)
		{
			Process.Start("https://" + GlobalMemory.AUTHOR_WEBSITE);

        }
		
		public void BackgroundWorker1_DoWork(System.Object sender, System.ComponentModel.DoWorkEventArgs e)
		{
            if (is_bible)
            {
                // bible
                if (exporting_bible)
                {
                    bible_format_export = BibleFormat.doExportBible(export_filename, export_filter_idx);
                    if (bible_format_export == null)
                    {
                        throw new Exception("doExportBible created a null bible object.");
                    }
                    bible_format_export.ExportBible(export_filename, export_filter_idx);

                }
                else
                {
                    BibleFormat.BibleFactory(biblefile);
                    if (BibleFormat.getInstance() != null)
                    {
                        if (!BibleFormat.openNotSupported(BibleFormat.getInstance().FilterIndex))
                        {
                            BibleFormat.getInstance().Load();
                            if (GlobalMemory.getInstance().addRedText && !(GlobalMemory.getInstance().parseBibleBkgndCleaner))
                            {
                                // add red text only if no cleaning and/or bkgnd cleaner is specified. this is because, red text will be added after cleaning. 
                                BibleFormat.getInstance().addRedText();
                            }
                            //RTL.Checked = false;
                            data = BibleFormat.getInstance().populateBooks(MirrorTreeView1);
                        }
                    }
                    else
                    {
                        try_cmtry = true;
                    }
                }
            }
            else
            {
                // book
                if (exporting_book)
                {
                    book_format_export = BookFormat.doExportBook(export_filename, export_filter_idx);
                    if (book_format_export == null)
                    {
                        throw new Exception("doExportBook created a null bible object.");
                    }
                    book_format_export.ExportBook(export_filename, export_filter_idx);

                }
                else
                {                    
                    BookFormat.BookFactory(bookfile);
                    if (BookFormat.getInstance() != null)
                    {
                        BookFormat.getInstance().Load();
                        //RTL.Checked = false;
                        data = BookFormat.getInstance().populateBooks(MirrorTreeView1);
                    }                    
                }
            }
		}        
		
		public void BackgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
		{
            if (is_bible)
            {
                // bible
                if (exporting_bible)
                {
                    Timer1.Enabled = false;
                    StatusStrip1.Visible = false;
                    ProgressBar.Value = 0;
                    StatusText.Text = "0%";
                    exporting_bible = false;
                }
                else
                {
                    if (try_cmtry)
                    {
                        try_cmtry = false;
                        commentaryfile = biblefile;
                        biblefile = null;
                        OpenCommentary();
                        return;
                    }
                    else
                    {
                        try
                        {
                            if (e.Error == null)
                            {
                                Timer1.Enabled = false;
                                StatusStrip1.Visible = false;
                                ProgressBar.Value = 0;
                                StatusText.Text = "0%";

                                if (BibleFormat.openNotSupported(BibleFormat.getInstance().FilterIndex))
                                {
                                    Themes.MessageBox("Opening this type of file(s) are not supported. Only export is supported. Sorry for this inconvenience.", "Not Supported", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    //
                                    return;
                                }

                                setTitles();
                                
                                paraBtn.Enabled = true;
                                exportBibleToolStripMenuItem.Enabled = true;
                                exportBookToolStripMenuItem.Enabled = false;
                                //exportCommentaryToolStripMenuItem.Enabled = false;
                                findToolStripMenuItem.Enabled = true;
                                findBtn.Enabled = true;
                                fontBtn.Enabled = true;
                                editBtn.Enabled = true;
                                statBtn.Enabled = true;
                                btnPlugin.Enabled = true;
                                transliterateBtn.Enabled = true;
                                
                                last_selected_node = MirrorTreeView1.Nodes[0].FirstNode.FirstNode;

                                versionHistory.Add(last_selected_node.FullPath);
                                versionHistoryCounter = versionHistory.Count - 1;

                                backBtn.Enabled = true;
                                saveToolStripMenuItem.Enabled = true;
                                if (GlobalMemory.getInstance().parseBibleBkgndCleaner)
                                {
                                    backgroundCleaner.RunWorkerAsync(); // start cleaning at backend                            
                                    StatusStrip1.Visible = true;
                                    ProgressBar.Value = 0;
                                    StatusText.Text = "Cleaning Texts ...";
                                }
                                else
                                {
                                    if (GlobalMemory.getInstance().EnablePlugins)
                                    {
                                        PluginProcessFrm frm = new PluginProcessFrm(BibleFormat.BibleXmlDocument, PluginProcessFrm.TYPE_BIBL);
                                        frm.ShowDialog(this);
                                    }
                                }
                                Thread.Sleep(100);
                                ChapterArea.NavigateToString(BibleFormat.getInstance().getChapterHtml(data, paraBtn.Checked, CommentaryFormat.getInstance()));
                                /*
                                 * below is used only for executable bible (as it has cmtry and dict attached in a single file)
                                 * 
                                 */
                                postCmtOpen();
                                postDictOpen();
                            }
                            else
                            {
                                ChapterArea.NavigateToString(BibleFormat.getInstance().getReaderErrorHtml(e.Error.GetBaseException()));
                                StatusStrip1.Visible = false;
                                ProgressBar.Value = 0;
                                StatusText.Text = "0%";
                            }

                        }
                        catch (Exception ex)
                        {
                            if (BibleFormat.getInstance() == null)
                            {
                                Themes.MessageBox("something went wrong in my code ;(");
                                return;
                            }
                            ChapterArea.NavigateToString(BibleFormat.getInstance().getReaderErrorHtml(ex));
                            StatusStrip1.Visible = false;
                            ProgressBar.Value = 0;
                            StatusText.Text = "0%";
                        }
                    }
                }
            }
            else
            {
                // book
                if (exporting_book)
                {
                    Timer1.Enabled = false;
                    StatusStrip1.Visible = false;
                    ProgressBar.Value = 0;
                    StatusText.Text = "0%";
                    exporting_book = false;
                }
                else
                {
                    try
                    {
                        if (e.Error == null)
                        {
                            setTitles();
                            Timer1.Enabled = false;
                            StatusStrip1.Visible = false;
                            paraBtn.Enabled = false;
                            ProgressBar.Value = 0;
                            StatusText.Text = "0%";
                            exportBibleToolStripMenuItem.Enabled = false;
                            exportBookToolStripMenuItem.Enabled = true;
                            //exportCommentaryToolStripMenuItem.Enabled = false;
                            findToolStripMenuItem.Enabled = false;
                            findBtn.Enabled = false;
                            fontBtn.Enabled = true;
                            editBtn.Enabled = true;
                            statBtn.Enabled = false;
                            transliterateBtn.Enabled = false;
                            backBtn.Enabled = false;

                            if (MirrorTreeView1.Nodes.Count > 0)
                            {
                                TreeNode tmp = MirrorTreeView1.Nodes[0];
                                while (tmp != null)
                                {
                                    last_selected_node = tmp;
                                    tmp = tmp.FirstNode;
                                }
                            }

                            if (GlobalMemory.getInstance().parseBookBkgndCleaner)
                            {
                                backgroundCleaner.RunWorkerAsync(); // start cleaning at backend                            
                                StatusStrip1.Visible = true;
                                ProgressBar.Value = 0;
                                StatusText.Text = "Cleaning Texts ...";
                            }
                            else
                            {
                                if (GlobalMemory.getInstance().EnablePlugins)
                                {
                                    PluginProcessFrm frm = new PluginProcessFrm(BookFormat.BookXmlDocument, PluginProcessFrm.TYPE_BOOK);
                                    frm.ShowDialog(this);
                                }
                            }
                            Thread.Sleep(100);
                            ChapterArea.NavigateToString(BookFormat.getInstance().getChapterHtml(data));      
                        }
                        else
                        {
                            ChapterArea.NavigateToString(BookFormat.getInstance().getReaderErrorHtml(e.Error.GetBaseException()));
                            StatusStrip1.Visible = false;
                            ProgressBar.Value = 0;
                            StatusText.Text = "0%";
                        }
                    }
                    catch (Exception ex)
                    {
                        if (BookFormat.getInstance() == null)
                        {
                            Themes.MessageBox("something went wrong in my code ;(");
                            return;
                        }
                        ChapterArea.NavigateToString(BookFormat.getInstance().getReaderErrorHtml(ex));
                        StatusStrip1.Visible = false;
                        ProgressBar.Value = 0;
                        StatusText.Text = "0%";
                    }
                }
            }
		}
		
		public void Timer1_Tick(System.Object sender, System.EventArgs e)
		{
            if (is_bible)
            {
                if (BibleFormat.getInstance() == null)
                {
                    return;
                }
                try
                {
                    if (exporting_bible)
                    {
                        StatusText.Text = bible_format_export.PercentComplete.ToString() + "%";
                        ProgressBar.Value = bible_format_export.PercentComplete < 0 ? 0 : bible_format_export.PercentComplete;
                    }
                    else
                    {
                        StatusText.Text = BibleFormat.getInstance().PercentComplete.ToString() + "%"+ BibleFormat.getInstance().AdditionaStatusText();
                        ProgressBar.Value = BibleFormat.getInstance().PercentComplete < 0 ? 0 : BibleFormat.getInstance().PercentComplete;
                    }
                }
                catch (Exception)
                {
                    //BackgroundWorker may not have started setting the progress bar.. just ignore..
                }
            }
            else
            {
                if (BookFormat.getInstance() == null)
                {
                    return;
                }
                try
                {
                    if (exporting_book)
                    {
                        StatusText.Text = book_format_export.PercentComplete.ToString() + "%";
                        ProgressBar.Value = book_format_export.PercentComplete < 0 ? 0 : book_format_export.PercentComplete;
                    }
                    else
                    {
                        StatusText.Text = BookFormat.getInstance().PercentComplete.ToString() + "%";
                        ProgressBar.Value = BookFormat.getInstance().PercentComplete < 0 ? 0 : BookFormat.getInstance().PercentComplete;
                    }
                }
                catch (Exception)
                {
                    //BackgroundWorker may not have started setting the progress bar.. just ignore..
                }
            }
		}

		public async void SimpleBibleReader_Load(System.Object sender, System.EventArgs e)
		{
			StatusStrip1.Visible = false;
			exportBibleToolStripMenuItem.Enabled = false;
            exportCommentaryToolStripMenuItem.Enabled = false;
            exportDictionaryToolStripMenuItem.Enabled = false;
            exportBookToolStripMenuItem.Enabled = false;
            saveToolStripMenuItem.Enabled = false;
            findToolStripMenuItem.Enabled = false;
            findBtn.Enabled = false;
            fontBtn.Enabled = false;
            bibleTitleLbl.Visible = false;
            paraBtn.Enabled = false;
            editBtn.Enabled = false;
            cmtEditBtn.Enabled = false;
            cmtZoomInBtn.Enabled = false;
            cmtZoomOutBtn.Enabled = false;
            dictEditBtn.Enabled = false;
            dictZoomInBtn.Enabled = false;
            dictZoomOutBtn.Enabled = false;
            cmtFontBtn.Enabled = false;
            cmtPrintBtn.Enabled = false;
            dctFontBtn.Enabled = false;
            dictPrintBtn.Enabled = false;
            transliterateBtn.Enabled = false;
            statBtn.Enabled = false;
            //this.Text = Application.ProductName + " " + Application.ProductVersion;
            //
            splitContainer2.Panel2Collapsed=true;
            splitContainer3.Panel2Collapsed = true;
            //

            buildThemesMenuItems();

            //
            await ChapterArea.EnsureCoreWebView2Async();
            await CommentaryArea.EnsureCoreWebView2Async();
            await DictionaryArea.EnsureCoreWebView2Async();
            await SearchResultsArea.EnsureCoreWebView2Async();
            //

            this.FormClosing+=new FormClosingEventHandler(SimpleBibleReader_FormClosing);

            ChapterArea.CoreWebView2.NewWindowRequested += ChapterArea_NewWindowRequested;
            CommentaryArea.CoreWebView2.NewWindowRequested += CommentaryArea_NewWindowRequested;
            DictionaryArea.CoreWebView2.NewWindowRequested += DictionaryArea_NewWindowRequested;
            SearchResultsArea.CoreWebView2.NewWindowRequested += SearchResultsArea_NewWindowRequested;

            ChapterArea.NavigationStarting += ChapterArea_NavigationStarting;

            ChapterArea.WebMessageReceived += ChapterArea_WebMessageReceived;
            CommentaryArea.WebMessageReceived += CommentaryArea_WebMessageReceived;
            DictionaryArea.WebMessageReceived += DictionaryArea_WebMessageReceived;
            SearchResultsArea.WebMessageReceived += SearchResultsArea_WebMessageReceived;

            ChapterArea.CoreWebView2.ContextMenuRequested += ChapterArea_ContextMenuRequested;


            // webview2 settings
            ChapterArea.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;
            CommentaryArea.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            DictionaryArea.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            SearchResultsArea.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;

            /*
            ChapterArea.CoreWebView2.Settings.AreDevToolsEnabled = false;
            CommentaryArea.CoreWebView2.Settings.AreDevToolsEnabled = false;
            DictionaryArea.CoreWebView2.Settings.AreDevToolsEnabled = false;
            SearchResultsArea.CoreWebView2.Settings.AreDevToolsEnabled = false;
            */

            ChapterArea.CoreWebView2.Settings.IsStatusBarEnabled = false;
            CommentaryArea.CoreWebView2.Settings.IsStatusBarEnabled = false;
            DictionaryArea.CoreWebView2.Settings.IsStatusBarEnabled = false;
            SearchResultsArea.CoreWebView2.Settings.IsStatusBarEnabled = false;

            // check if dark mode.
            if (GlobalMemory.getInstance().SelectedTheme.ToLower().Contains("dark"))
            {
                // change to dark mode.
                this.BackColor = Colors.GreyBackground;
                this.ForeColor = Colors.LightText;
                MenuStrip1.Renderer = new DarkUI.Renderers.DarkMenuRenderer();//Themes.getDarkRender();
                Themes.ChangeDarkMode(this.Controls);
                Themes.ChangeMenuDarkMode(MenuStrip1.Items);                
                changeImagesToDarkMode();
                CommentaryArea.NavigateToString("<html><head>"+Themes.CSS_COMMENTARY+"</head><body/></html>");
                DictionaryArea.NavigateToString("<html><head>" + Themes.CSS_COMMENTARY + "</head><body/></html>");
                SearchResultsArea.NavigateToString("<html><head>" + Themes.CSS_COMMENTARY + "</head><body/></html>");
            }
            else
            {
                this.ForeColor = Form.DefaultForeColor;
                Themes.ChangeNormalMode(this.Controls);
                Themes.ChangeMenuNormalMode(MenuStrip1.Items);
                cmtListBox.BackColor = Color.White;
                dictListBox.BackColor = Color.White;
                MirrorTreeView1.BackColor = Color.White;
                pictureBoxFront.BackColor = Color.White;
                changeImagesToNormalMode();
            }

            StringBuilder frontPage = new StringBuilder();
            frontPage.Append("<html><head>");
            frontPage.Append(Themes.CSS_UI);
            frontPage.Append("</head><body>");            
            frontPage.Append("<p>" + Localization.MainPage_ParagraphOne + "</p>");

            

            frontPage.Append("<h3>"+Localization.MainPage_QuickGuide+"</h3>");
            frontPage.Append(Localization.MainPage_QuickGuideText);
            frontPage.Append(GetFormatsSupported());
            frontPage.Append("</body></html>");
            
            ChapterArea.NavigateToString(frontPage.ToString());
        }

        private void changeImagesToNormalMode()
        {
            // font picture
            pictureBoxFront.Image = Resources.books;
            // main tool bar
            backBtn.Image = Resources.backward;
            frontBtn.Image = Resources.forward;
            zoomInBtn.Image = Resources.zoom_in;
            zoomOutBtn.Image = Resources.zoom_out;
            fullScreenBtn.Image = Resources.fullscreen;
            paraBtn.Image = Resources.paragraph;
            fontBtn.Image = Resources.font;
            editBtn.Image = Resources.edit;
            findBtn.Image = Resources.find;
            printChapterBtn.Image = Resources.print;
            btnPlugin.Image = Resources.run;
            transliterateBtn.Image = Resources.transliterate;
            statBtn.Image = Resources.stat;
            btnBible.Image = Resources.bible;
            helpBtn.Image = Resources.help;
            //-- search toolbar
            btnCollapse.Image = Resources.collapse;
            btnExpand.Image = Resources.expand;
            fullScreenSearchBtn.Image = Resources.fullscreen;
            moreVersesBtn.Image = Resources.more;
            //--commentary
            chkBoxShowList.Image = Resources.show_menu;
            cmtZoomInBtn.Image = Resources.zoom_in;
            cmtZoomOutBtn.Image = Resources.zoom_out;
            cmtFontBtn.Image = Resources.font;
            cmtEditBtn.Image = Resources.edit;
            cmtFullScreenBtn.Image = Resources.fullscreen;
            cmtPrintBtn.Image = Resources.print;
            //--dictionary
            chkBoxShowDictList.Image = Resources.show_menu;
            dictZoomInBtn.Image = Resources.zoom_in;
            dictZoomOutBtn.Image = Resources.zoom_out;
            dctFontBtn.Image = Resources.font;
            dictEditBtn.Image = Resources.edit;
            dctFullScreenBtn.Image = Resources.fullscreen;
            dictPrintBtn.Image = Resources.print;
        }


        private void changeImagesToDarkMode()
        {
            // font picture
            pictureBoxFront.Image = Resources.books_inv;
            // main tool bar
            backBtn.Image = Resources.backward_inv;
            frontBtn.Image = Resources.forward_inv;
            zoomInBtn.Image = Resources.zoom_in_inv;
            zoomOutBtn.Image = Resources.zoom_out_inv;
            fullScreenBtn.Image = Resources.fullscreen_inv;
            paraBtn.Image = Resources.paragraph_inv;
            fontBtn.Image= Resources.font_inv;
            editBtn.Image= Resources.edit_inv;
            findBtn.Image= Resources.find_inv;
            printChapterBtn.Image=Resources.print_inv;
            btnPlugin.Image = Resources.run_inv;
            transliterateBtn.Image= Resources.transliterate_inv;
            statBtn.Image= Resources.stat_inv;
            btnBible.Image = Resources.bible_inv;
            helpBtn.Image= Resources.help_inv;
            //-- search toolbar
            btnCollapse.Image = Resources.collapse_inv;
            btnExpand.Image= Resources.expand_inv;
            fullScreenSearchBtn.Image = Resources.fullscreen_inv;
            moreVersesBtn.Image = Resources.more_inv;
            //--commentary
            chkBoxShowList.Image = Resources.show_menu_inv;
            cmtZoomInBtn.Image = Resources.zoom_in_inv;
            cmtZoomOutBtn.Image = Resources.zoom_out_inv;
            cmtFontBtn.Image = Resources.font_inv;
            cmtEditBtn.Image = Resources.edit_inv;
            cmtFullScreenBtn.Image = Resources.fullscreen_inv;
            cmtPrintBtn.Image = Resources.print_inv;
            //--dictionary
            chkBoxShowDictList.Image = Resources.show_menu_inv;
            dictZoomInBtn.Image = Resources.zoom_in_inv;
            dictZoomOutBtn.Image = Resources.zoom_out_inv;
            dctFontBtn.Image = Resources.font_inv;
            dictEditBtn.Image = Resources.edit_inv;
            dctFullScreenBtn.Image = Resources.fullscreen_inv;
            dictPrintBtn.Image = Resources.print_inv;
        }

        private void buildThemesMenuItems()
        {

            //Themes Menu
            string[] themesList = Themes.getThemes();

            ToolStripMenuItem[] items = new ToolStripMenuItem[themesList.Length];
            for (int i = 0; i < items.Length; i++)
            {
                items[i] = new ToolStripMenuItem();
                items[i].Name = "themeToolStripMenuItem_" + i.ToString();
                items[i].Tag = themesList[i];
                items[i].Text = themesList[i];
                items[i].Click += new EventHandler(MenuItemClickHandler);
            }

            themesToolStripMenuItem.DropDownItems.AddRange(items);
        }

        private void MenuItemClickHandler(object sender, EventArgs e)
        {
            ToolStripMenuItem clickedTheme = (ToolStripMenuItem)sender;
            GlobalMemory.getInstance().SelectedTheme = clickedTheme.Text;
            if (GlobalMemory.getInstance().SelectedTheme.ToLower().Contains("dark"))
                GlobalMemory.getInstance().Theme = DarkMode.DarkMode.Theme.DARK;
            else if (GlobalMemory.getInstance().SelectedTheme.ToLower().Contains("light"))
                GlobalMemory.getInstance().Theme = DarkMode.DarkMode.Theme.LIGHT;
            else
                GlobalMemory.getInstance().Theme = DarkMode.DarkMode.Theme.SYSTEM;
            DarkMode.DarkMode.UpdateWindowTheme(this, GlobalMemory.getInstance().Theme);
            RestartApplication();

        }

        public string getCurrentVerseRawText()
        {
            BibleFormat bible_local = BibleFormat.getInstance();
            if (bible_local == null)
            {
                return "";
            }

            int book_no = currentSelectedBook;
            int chapter_no = currentSelectedChapter;
            int verse_no = currentSelectedVerse;


            selected_chapter_elmt = bible_local.getXmlDoc().SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber='" + book_no.ToString() + "']/CHAPTER[@cnumber='" + chapter_no.ToString() + "']");
            int current_verse = verse_no;

            string book_name = last_selected_node.FullPath.Split('\\')[1];

            if (selected_chapter_elmt != null)
            {
                xml_verse_elmt = selected_chapter_elmt.SelectSingleNode("VERS[@vnumber='" + verse_no.ToString() + "']");
                if (xml_verse_elmt != null)
                {
                    return xml_verse_elmt.InnerXml;
                }
            }
            return "";
        }
        private void ChapterArea_ContextMenuRequested(object sender, CoreWebView2ContextMenuRequestedEventArgs e)
        {
            BibleFormat bible_local = BibleFormat.getInstance();
            if (bible_local == null)
            {
                e.Handled = true;
                return;
            }

            IList<CoreWebView2ContextMenuItem> menuList = e.MenuItems;
            menuList.Clear();

            int book_no = currentSelectedBook;
            int chapter_no = currentSelectedChapter;
            int verse_no = currentSelectedVerse;


            selected_chapter_elmt = bible_local.getXmlDoc().SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber='" + book_no.ToString() + "']/CHAPTER[@cnumber='" + chapter_no.ToString() + "']");
            int current_verse = verse_no;

            string book_name = last_selected_node.FullPath.Split('\\')[1];


            //if it's other language leave it.. otherwise make short version.
            if (Localization.getBookNames()[book_no - 1] == book_name)
                book_name = FormatUtil.shortBookNames[book_no - 1];



            ///// MENU CREATION /////////////////

            // Speak Verse
            CoreWebView2ContextMenuItem speakVerse = ChapterArea.CoreWebView2.Environment.CreateContextMenuItem(
        "Speak Verse", null, CoreWebView2ContextMenuItemKind.Command);
            speakVerse.CustomItemSelected += delegate (object send, Object ex)
            {
                string pageUri = e.ContextMenuTarget.PageUri;
                System.Threading.SynchronizationContext.Current.Post((_) =>
                {
                    ContextMenuItem_speakVerse();
                }, null);
            };               
            

            // Copy Verse
            CoreWebView2ContextMenuItem copyVerse = ChapterArea.CoreWebView2.Environment.CreateContextMenuItem(
        "Copy " + book_name + " " + chapter_no.ToString() + ":" + verse_no.ToString(), null, CoreWebView2ContextMenuItemKind.Command);
            copyVerse.CustomItemSelected += delegate (object send, Object ex)
            {
                System.Threading.SynchronizationContext.Current.Post((_) =>
                {
                    ContextMenuItem_copyVerse();
                }, null);
            };


            // Copy 2 Verses
            CoreWebView2ContextMenuItem copy2Verse = ChapterArea.CoreWebView2.Environment.CreateContextMenuItem(
         "Copy " + book_name + " " + chapter_no.ToString() + ":" + verse_no.ToString() + "-" + (verse_no + 1).ToString(), null, CoreWebView2ContextMenuItemKind.Command);
            copy2Verse.CustomItemSelected += delegate (object send, Object ex)
            {
                System.Threading.SynchronizationContext.Current.Post((_) =>
                {
                    ContextMenuItem_copy2Verses();
                }, null);
            };

            // Copy 3 Verses
            CoreWebView2ContextMenuItem copy3Verse = ChapterArea.CoreWebView2.Environment.CreateContextMenuItem(
            "Copy " + book_name + " " + chapter_no.ToString() + ":" + verse_no.ToString() + "-" + (verse_no + 2).ToString(), null, CoreWebView2ContextMenuItemKind.Command);
            copy3Verse.CustomItemSelected += delegate (object send, Object ex)
            {
                System.Threading.SynchronizationContext.Current.Post((_) =>
                {
                    ContextMenuItem_copy3Verses();
                }, null);
            };

 
            // Edit Verse
            CoreWebView2ContextMenuItem editVerse = ChapterArea.CoreWebView2.Environment.CreateContextMenuItem(
            "Edit Verse", null, CoreWebView2ContextMenuItemKind.Command);
            editVerse.CustomItemSelected += delegate (object send, Object ex)
            {
                System.Threading.SynchronizationContext.Current.Post((_) =>
                {
                    ContextMenuItem_editVerse();
                }, null);
            };
            


            // Add Comment
            CoreWebView2ContextMenuItem addComment = ChapterArea.CoreWebView2.Environment.CreateContextMenuItem(
            "Add Comment", null, CoreWebView2ContextMenuItemKind.Command);
            addComment.CustomItemSelected += delegate (object send, Object ex)
            {
                System.Threading.SynchronizationContext.Current.Post((_) =>
                {
                    ContextMenuItem_addComment();
                }, null);
            };
            

            // Edit Comment
            CoreWebView2ContextMenuItem editComment = ChapterArea.CoreWebView2.Environment.CreateContextMenuItem(
            "Edit Comment", null, CoreWebView2ContextMenuItemKind.Command);
            editComment.CustomItemSelected += delegate (object send, Object ex)
            {
                System.Threading.SynchronizationContext.Current.Post((_) =>
                {
                    ContextMenuItem_editComment();
                }, null);
            };
            

            // Delete Comment
            CoreWebView2ContextMenuItem deleteComment = ChapterArea.CoreWebView2.Environment.CreateContextMenuItem(
            "Delete Comment", null, CoreWebView2ContextMenuItemKind.Command);
            deleteComment.CustomItemSelected += delegate (object send, Object ex)
            {
                System.Threading.SynchronizationContext.Current.Post((_) =>
                {
                    ContextMenuItem_deleteComment();
                }, null);
            };
            

            // Speak Chapter
            CoreWebView2ContextMenuItem speakChapter = ChapterArea.CoreWebView2.Environment.CreateContextMenuItem(
            "Speak Chapter", null, CoreWebView2ContextMenuItemKind.Command);
            speakChapter.CustomItemSelected += delegate (object send, Object ex)
            {
                System.Threading.SynchronizationContext.Current.Post((_) =>
                {
                    ContextMenuItem_speakChapter();
                }, null);
            };
           

            // Copy Chapter
            CoreWebView2ContextMenuItem copyChapter = ChapterArea.CoreWebView2.Environment.CreateContextMenuItem(
            "Copy " + book_name + " " + chapter_no.ToString(), null, CoreWebView2ContextMenuItemKind.Command);
            copyChapter.CustomItemSelected += delegate (object send, Object ex)
            {
                System.Threading.SynchronizationContext.Current.Post((_) =>
                {
                    ContextMenuItem_copyChapter();
                }, null);
            };
            

            // Print Chapter
            CoreWebView2ContextMenuItem printChapter = ChapterArea.CoreWebView2.Environment.CreateContextMenuItem(
            "Print " + book_name + " " + chapter_no.ToString(), null, CoreWebView2ContextMenuItemKind.Command);
            printChapter.CustomItemSelected += delegate (object send, Object ex)
            {
                System.Threading.SynchronizationContext.Current.Post((_) =>
                {
                    ContextMenuItem_printChapter();
                }, null);
            };







            xml_verse_elmt = selected_chapter_elmt.SelectSingleNode("VERS[@vnumber='" + verse_no.ToString() + "']");
            string texts = "";

            XmlNode tmp_elmt = null;
            XmlNode tmp_elmt2 = null;
            verse_ref = book_name + " " + chapter_no.ToString() + ":" + verse_no.ToString();

            if (xml_verse_elmt != null)
            {
                menuList.Insert(menuList.Count, speakVerse);
                menuList.Insert(menuList.Count, copyVerse);
                texts = xml_verse_elmt.InnerText;
                selected_verse = texts + " " + " (" + book_name + " " + chapter_no.ToString() + ":" + verse_no.ToString() + ")";
            }

            //// --------------------------------------------
            if(GlobalMemory.getInstance().EditBibleVerse)
                menuList.Insert(menuList.Count, editVerse);


            tmp_elmt = xml_verse_elmt.SelectSingleNode("NOTE");
            if (tmp_elmt == null)
            {
                menuList.Insert(menuList.Count, addComment);
            }
            else
            {
                menuList.Insert(menuList.Count, editComment);
                menuList.Insert(menuList.Count, deleteComment);
            }

            tmp_elmt = xml_verse_elmt.NextSibling;
            if (tmp_elmt != null)
            {

                texts = texts + " " + tmp_elmt.InnerText;
                selected_2verses = texts + " " + " (" + book_name + " " + chapter_no.ToString() + ":" + verse_no.ToString() + "-" + (verse_no + 1).ToString() + ")";
                menuList.Insert(menuList.Count, copy2Verse);
            }

            if (tmp_elmt != null)
                tmp_elmt2 = tmp_elmt.NextSibling;

            if (tmp_elmt2 != null)
                if (tmp_elmt2.Name == "NOTE")
                    tmp_elmt2 = tmp_elmt2.NextSibling;

            if (tmp_elmt2 != null)
            {
                menuList.Insert(menuList.Count, copy3Verse);
                texts = texts + " " + tmp_elmt2.InnerText;
                selected_3verses = texts + " " + " (" + book_name + " " + chapter_no.ToString() + ":" + verse_no.ToString() + "-" + (verse_no + 2).ToString() + ")";
            }

            /*
            tmp_elmt = xml_verse_elmt.NextSibling;
            if (tmp_elmt != null)
                if (tmp_elmt.Name == "COMMENT")
                {
                    //// Comment
                    menuList.Insert(menuList.Count, editComment);
                    menuList.Insert(menuList.Count, deleteComment);
                    tmp_elmt = xml_verse_elmt.NextSibling;
                }
                else
                {
                    menuList.Insert(menuList.Count, addComment);
                }

            if (tmp_elmt != null)
            {

                texts = texts + " " + tmp_elmt.InnerText;
                selected_2verses = texts + " " + " (" + book_name + " " + chapter_no.ToString() + ":" + verse_no.ToString() + "-" + (verse_no + 1).ToString() + ")";
                menuList.Insert(menuList.Count, copy2Verse);
            }

            if (tmp_elmt != null)
                tmp_elmt2 = tmp_elmt.NextSibling;

            if (tmp_elmt2 != null)
                if (tmp_elmt2.Name == "COMMENT")
                    tmp_elmt2 = tmp_elmt2.NextSibling;

            if (tmp_elmt2 != null)
            {
                menuList.Insert(menuList.Count, copy3Verse);
                texts = texts + " " + tmp_elmt2.InnerText;
                selected_3verses = texts + " " + " (" + book_name + " " + chapter_no.ToString() + ":" + verse_no.ToString() + "-" + (verse_no + 2).ToString() + ")";
            }
            */

            menuList.Insert(menuList.Count, speakChapter);
            menuList.Insert(menuList.Count, copyChapter);
            menuList.Insert(menuList.Count, printChapter);

        }

        private void SearchResultsArea_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            string message = e.TryGetWebMessageAsString().Trim();
            if (message != null)
            {
                if (message.StartsWith("$"))
                {
                    verse_ref = message.Substring(1).Replace("%20", " ");
                    string book = verse_ref.Substring(0, verse_ref.LastIndexOf(' ')).Trim();
                    string chapter = verse_ref.Substring(verse_ref.LastIndexOf(' ')).Trim().Split(':')[0];
                    string verse = verse_ref.Substring(verse_ref.LastIndexOf(' ')).Trim().Split(':')[1];
                    TreeNode tn = findNode(MirrorTreeView1.Nodes[0].FullPath + MirrorTreeView1.PathSeparator + book + MirrorTreeView1.PathSeparator + chapter);
                    if (tn == null) // try NT
                        tn = findNode(MirrorTreeView1.Nodes[1].FullPath + MirrorTreeView1.PathSeparator + book + MirrorTreeView1.PathSeparator + chapter);
                    if (tn == null) // try Apocrypha
                        tn = findNode(MirrorTreeView1.Nodes[2].FullPath + MirrorTreeView1.PathSeparator + book + MirrorTreeView1.PathSeparator + chapter);
                    //programatic_action = true;
                    MirrorTreeView1.SelectedNode = tn;
                    MirrorTreeView1.Focus();
                    MirrorTreeView1.SelectedNode.EnsureVisible();
                    //programatic_action = false;
                    verse_to_highlight = verse;
                    timerSelectVerse.Enabled = true;                   
                }
            }
        }

        private void DictionaryArea_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            string message = e.TryGetWebMessageAsString().Trim();
            if (message != null)
            {
                if (message.StartsWith("#d"))
                {
                    string item = message.Substring(2).Trim();
                    if (dictListBox != null)
                        if (dictListBox.Items.Contains(item))
                            dictListBox.SelectedItem = item;
                }
                else if (message.StartsWith("B"))
                {
                    message = message.Substring(1);
                    if (message.IndexOf(";") != -1)
                    {
                        // bible reference
                        if (BibleFormat.getInstance() == null)
                        {
                            Themes.MessageBox("Please load a Bible for me to take you to a particular verse.", "Dictionary");
                            return;
                        }

                        string[] verse_ref = message.Split(";".ToCharArray());
                        currentSelectedChapter = int.Parse(verse_ref[1]);
                        currentSelectedVerse = int.Parse(verse_ref[2]);
                        string book = BibleFormat.bible_format.getBookName(int.Parse(verse_ref[0]));
                        string chapter = verse_ref[1];
                        string verse = verse_ref[2];
                        TreeNode tn = findNode(MirrorTreeView1.Nodes[0].FullPath + MirrorTreeView1.PathSeparator + book + MirrorTreeView1.PathSeparator + chapter);
                        if (tn == null)
                            tn = findNode(MirrorTreeView1.Nodes[1].FullPath + MirrorTreeView1.PathSeparator + book + MirrorTreeView1.PathSeparator + chapter);
                        //programatic_action = true;
                        MirrorTreeView1.SelectedNode = tn;
                        MirrorTreeView1.Focus();
                        MirrorTreeView1.SelectedNode.EnsureVisible();
                        verse_to_highlight = verse;
                        timerSelectVerse.Enabled = true;
                        //programatic_action = false;
                    }
                }
                else if (message.StartsWith("D"))
                {
                    string item = message.Substring(1).Trim();
                    if (dictListBox != null)
                        if (dictListBox.Items.Contains(item))
                            dictListBox.SelectedItem = item;
                }
            }
        }

        private void CommentaryArea_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            // bible not loaded
            if (BibleFormat.getInstance() == null)
            {
                Themes.MessageBox("Please load a Bible for me to take you to a particular verse.", "Commentary");             
                return;
            }

            string message = e.TryGetWebMessageAsString().Trim();
            if (message != null)
            {
                // TODO
                // MySword for Android format '#b43.1.1'
                if (message.Split(".".ToCharArray()).Length == 3)
                {
                    string[] verse_ref = message.Split(".".ToCharArray());
                    if (verse_ref[0].StartsWith("#b"))
                        verse_ref[0] = verse_ref[0].Substring(2);
                    currentSelectedBook = int.Parse(verse_ref[0]);
                    currentSelectedChapter = int.Parse(verse_ref[1]);
                    currentSelectedVerse = int.Parse(verse_ref[2]);

                    string book = BibleFormat.bible_format.getBookName(int.Parse(verse_ref[0]));
                    string chapter = verse_ref[1];
                    string verse = verse_ref[2];
                    TreeNode tn = findNode(MirrorTreeView1.Nodes[0].FullPath + MirrorTreeView1.PathSeparator + book + MirrorTreeView1.PathSeparator + chapter);
                    if (tn == null)
                        tn = findNode(MirrorTreeView1.Nodes[1].FullPath + MirrorTreeView1.PathSeparator + book + MirrorTreeView1.PathSeparator + chapter);
                    //programatic_action = true;
                    MirrorTreeView1.SelectedNode = tn;
                    MirrorTreeView1.Focus();
                    MirrorTreeView1.SelectedNode.EnsureVisible();
                    verse_to_highlight = verse;
                    timerSelectVerse.Enabled = true;
                }
                else if (message.StartsWith("B"))
                {
                    message = message.Substring(1);
                    if (message.IndexOf(";") != -1)
                    {

                        string[] verse_ref = message.Split(";".ToCharArray());
                        currentSelectedChapter = int.Parse(verse_ref[1]);
                        currentSelectedVerse = int.Parse(verse_ref[2]);
                        string book = BibleFormat.bible_format.getBookName(int.Parse(verse_ref[0]));
                        string chapter = verse_ref[1];
                        string verse = verse_ref[2];
                        TreeNode tn = findNode(MirrorTreeView1.Nodes[0].FullPath + MirrorTreeView1.PathSeparator + book + MirrorTreeView1.PathSeparator + chapter);
                        if (tn == null)
                            tn = findNode(MirrorTreeView1.Nodes[1].FullPath + MirrorTreeView1.PathSeparator + book + MirrorTreeView1.PathSeparator + chapter);
                        //programatic_action = true;
                        MirrorTreeView1.SelectedNode = tn;
                        MirrorTreeView1.Focus();
                        MirrorTreeView1.SelectedNode.EnsureVisible();
                        verse_to_highlight = verse;
                        timerSelectVerse.Enabled = true;
                        //programatic_action = false;
                    }
                }
                else if (message.StartsWith("C"))
                {
                    string item = message.Substring(1).Trim();
                    if (cmtListBox != null)
                        if (cmtListBox.Items.Contains(item))
                            cmtListBox.SelectedItem = item;
                }
            }
        }

        private void ChapterArea_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            string message = e.TryGetWebMessageAsString().Trim();
            if (message != null)
            {
                if (message.Split("-".ToCharArray()).Length == 3)
                {
                    string[] verse_ref = message.Split("-".ToCharArray());
                    currentSelectedBook = int.Parse(verse_ref[0]);
                    currentSelectedChapter = int.Parse(verse_ref[1]);
                    currentSelectedVerse = int.Parse(verse_ref[2]);

                    string chap_ver = " " + verse_ref[1] + ":" + verse_ref[2];
                    string vref = null;

                    if (cmtListBox.Items.Contains(FormatUtil.shortBookNames[currentSelectedBook - 1] + chap_ver))
                        cmtListBox.SelectedItem = FormatUtil.shortBookNames[currentSelectedBook - 1] + chap_ver;
                    else if (cmtListBox.Items.Contains(FormatUtil.swordBookAbbrInZefaniaIndex[currentSelectedBook - 1] + chap_ver))
                        cmtListBox.SelectedItem = FormatUtil.swordBookAbbrInZefaniaIndex[currentSelectedBook - 1] + chap_ver;
                    else if (cmtListBox.Items.Contains(FormatUtil.olbBookNames[currentSelectedBook - 1] + chap_ver))
                        cmtListBox.SelectedItem = FormatUtil.olbBookNames[currentSelectedBook - 1] + chap_ver;
                    else if (cmtListBox.Items.Contains(FormatUtil.logosBookNames[currentSelectedBook - 1] + chap_ver))
                        cmtListBox.SelectedItem = FormatUtil.logosBookNames[currentSelectedBook - 1] + chap_ver;
                    else if (cmtListBox.Items.Contains(Localization.getShortBookNames()[currentSelectedBook - 1] + chap_ver))
                        cmtListBox.SelectedItem = Localization.getShortBookNames()[currentSelectedBook - 1] + chap_ver;
                    else if (cmtListBox.Items.Contains(Localization.getBookNames()[currentSelectedBook - 1] + chap_ver))
                        cmtListBox.SelectedItem = Localization.getBookNames()[currentSelectedBook - 1] + chap_ver;
                    else
                    {
                        // finally try book names from zefania
                        if (BibleFormat.getInstance() != null)
                        {
                            // try again
                            vref = BibleFormat.getInstance().getBookName(currentSelectedBook) + " " + verse_ref[1] + ":" + verse_ref[2];
                            if (cmtListBox.Items.Contains(vref))
                                cmtListBox.SelectedItem = vref;
                        }
                    }

                    string para_style = "linear";
                    if(paraBtn.Checked)
                        para_style = "para";

                    string clearJS = @"
var a = document.getElementsByTagName('a');
for (var idx = 0; idx < a.length; ++idx)
{
    a[idx].className = '"+ para_style + @"';
}
var elem = document.getElementById('" + currentSelectedVerse + @"');
elem.className = 'selectedverse';";

                    ChapterArea.ExecuteScriptAsync(clearJS);

                    timer5.Enabled = true;

                    return;

                }
                else if (message.StartsWith("L"))
                {
                    string lexicon = message.Substring(1);
                    lexicon = Regex.Replace(lexicon, "[^HG0-9]", "");
                    if (dictListBox.Items.Contains(lexicon))
                        dictListBox.SelectedItem = lexicon;
                    return;
                }
            }
        }

        private void ChapterArea_NewWindowRequested(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NewWindowRequestedEventArgs e)
        {
            e.NewWindow = ChapterArea.CoreWebView2;
        }

        private void SearchResultsArea_NewWindowRequested(object sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            e.NewWindow = SearchResultsArea.CoreWebView2;            
        }

        private void DictionaryArea_NewWindowRequested(object sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            e.NewWindow = DictionaryArea.CoreWebView2;
        }

        private void CommentaryArea_NewWindowRequested(object sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            e.NewWindow = CommentaryArea.CoreWebView2;
        }
        //

        // begin navigations funcs()

        private void ChapterArea_NavigationStarting(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
        {
            Uri uri = new Uri(e.Uri);
            if (e.Uri.ToString().StartsWith("http://") || e.Uri.ToString().StartsWith("https://"))
            {
                Process.Start(e.Uri.ToString());
                e.Cancel = true;
                return;
            }            
            else if (System.IO.File.Exists(uri.LocalPath) || System.IO.Directory.Exists(uri.LocalPath))
            {
                biblefile = uri.LocalPath;
                OpenBible();
                e.Cancel = true;
                return;
            }
            else if (e.Uri.StartsWith("data:"))
            {
                // allow navigation
                return;
            }
            else if (e.Uri.StartsWith("about:blank"))
            {
                // avoid navigating to blank screen - not working
                e.Cancel = true;
                return;
            }
        }   

        // -- end of navigations ..

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(find ==null)
                find = new FindWebView2(SearchResultsArea,this);
            find.ShowDialog(this);
        }     

        private void ContextMenuItem_copyVerse()
        {
            Clipboard.SetText(selected_verse);
        }

        private void ContextMenuItem_printChapter()
        {            
            ChapterArea.CoreWebView2.ShowPrintUI();
        }

        private void ContextMenuItem_copy2Verses()
        {
            Clipboard.SetText(selected_2verses);
        }

        private void ContextMenuItem_copy3Verses()
        {
            Clipboard.SetText(selected_3verses);
        }

        private void ContextMenuItem_copyChapter()
        {
            //selected_book_name
            StringBuilder text=new StringBuilder();
            foreach (XmlNode ver in selected_chapter_elmt.SelectNodes("VERS"))
            {
                text.Append(ver.Attributes.GetNamedItem("vnumber").Value);
                text.Append(". ");
                text.Append(ver.InnerText);
                text.Append("\r\n");
            }            
            Clipboard.SetText(text.ToString());
        }

        private void ContextMenuItem_addComment()
        {
            bool proceed=true;
            BibleFormat bible_local = BibleFormat.getInstance();
            if ((bible_local.FilterIndex == 9 || bible_local.FilterIndex == 10))
            {
                proceed = false;
                Themes.MessageBox("Comments are only supported on Zefania XML bibles. You can export this bible in Zefania format and then save comments.", "Not Supported", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (proceed)
            {
                Comment cmt = new Comment(verse_ref);
                cmt.ShowDialog(this);
                string cmt_text = cmt.getComment();
                if (cmt_text != "")
                {
                    XmlElement cmt_elmt = bible_local.getXmlDoc().CreateElement("NOTE");
                    cmt_elmt.InnerText = cmt_text;
                    xml_verse_elmt.AppendChild(cmt_elmt);
                    /*
                    HtmlElement html_elmt = selected_html_elmt.Document.CreateElement("BLOCKQUOTE");
                    html_elmt.SetAttribute("id", "bgimage");
                    html_elmt.Style = "color:grey";
                    html_elmt.InnerText = cmt_text;
                    selected_html_elmt.InsertAdjacentElement(HtmlElementInsertionOrientation.AfterEnd, html_elmt);
                    */

                    //
                    string addCommentJS = @"
var elem = document.getElementById('" + currentSelectedVerse + @"');
    var cmt = document.createElement('BLOCKQUOTE');
    cmt.innerHTML = '" +  cmt_text.Replace("\n", "<br/>").Replace("\r","") + @"';
elem.parentNode.insertBefore(cmt, elem.nextSibling);";

                    ChapterArea.ExecuteScriptAsync(addCommentJS);

                }
                commented = true;
                cmt.Dispose();
            }
        }

        private void ContextMenuItem_editComment()
        {
            Comment cmt = new Comment(verse_ref);
            XmlNode note = xml_verse_elmt.SelectSingleNode("NOTE");
            if (note == null)            
                return;            
            cmt.setComment(note.InnerText);
            cmt.ShowDialog(this);
            string cmt_text = cmt.getComment();
            if (cmt_text != "")
            {
                note.InnerText = cmt_text;
                //selected_html_elmt.NextSibling.InnerText = cmt_text;

                string editCommentJS = @"
var elem = document.getElementById('" + currentSelectedVerse + @"');
elem.nextElementSibling.innerHTML = '" + cmt_text.Replace("\n", "<br/>").Replace("\r", "") + @"';";

                ChapterArea.ExecuteScriptAsync(editCommentJS);
            }
            else
            {   
                // remove...
                if(note.Name=="NOTE")
                    xml_verse_elmt.RemoveChild(note);
                /*
                if (selected_html_elmt.NextSibling.TagName == "BLOCKQUOTE")
                    selected_html_elmt.NextSibling.OuterHtml = "";
                */

                string delCommentJS = @"
var elem = document.getElementById('" + currentSelectedVerse + @"');
var next = elem.nextElementSibling;
if(next.tagName=='BLOCKQUOTE')
  next.remove();";

                ChapterArea.ExecuteScriptAsync(delCommentJS);
            }
            commented = true;
            cmt.Dispose();
        }

        private void ContextMenuItem_editVerse()
        {
            Verse vrs = new Verse(verse_ref);
            XmlNode note = xml_verse_elmt;
            if (note == null)
                return;
            vrs.setVerse(note.InnerXml);
            vrs.ShowDialog(this);
            string vrs_text = vrs.getVerse();
            if (vrs_text != "")
            {
                note.InnerXml = FormatUtil.UnescapeXML(vrs_text);

                string editCommentJS = @"
var elem = document.getElementById('" + currentSelectedVerse + @"');
var verseno = elem.firstChild.outerHTML;
elem.innerHTML = verseno + '" + FormatUtil.UnescapeXML(vrs_text).Replace("\n", "<br/>").Replace("\r", "") + @"';";

                ChapterArea.ExecuteScriptAsync(editCommentJS);
            }
            vrs.Dispose();
            
        }

        private void ContextMenuItem_deleteComment()
        {
            XmlNode note = xml_verse_elmt.SelectSingleNode("NOTE");
            if (note == null)
                return;
            if (note.Name == "NOTE")
                xml_verse_elmt.RemoveChild(note);
            /*
            if (selected_html_elmt.NextSibling.TagName == "BLOCKQUOTE")
                selected_html_elmt.NextSibling.OuterHtml = "";
            */

            string delCommentJS = @"
var elem = document.getElementById('" + currentSelectedVerse + @"');
var next = elem.nextElementSibling;
if(next.tagName=='BLOCKQUOTE')
  next.remove();";

            ChapterArea.ExecuteScriptAsync(delCommentJS);
            commented = true;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
             BibleFormat bible_local = BibleFormat.getInstance();
            if (bible_local == null)
            {
                Themes.MessageBox("Please load a Bible first!","Save");
                return;
            }

            if (bible_local.FilterIndex == 1)
            {
                if (!commented)
                    return;

                bible_local = BibleFormat.doExportBible(biblefile, 1);
                if (bible_local == null)
                {
                    Themes.MessageBox("Oops! Something went wrong ;)");
                    return;
                }
                bible_local.ExportBible(biblefile, 1);
                commented = false;
            }
            else if (bible_local.FilterIndex == 2)
            {
                if (!commented)
                    return;
                File.WriteAllText(biblefile, bible_local.XML);
                commented = false;
            }            
            else
            {
                if (Themes.MessageBox("Saving Bible with comments supported only in Zefania XML format.\n\nDo you want to save the Bible in Zefania XML format?", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = "Zefania XML (with Comments)|*.xml";
                    int result = (int)(sfd.ShowDialog());
                    if (result == System.Convert.ToInt32(System.Windows.Forms.DialogResult.OK))
                    {
                        biblefile = sfd.FileName.Trim();
                        if (sfd.FilterIndex == 1)
                        {
                            File.WriteAllText(biblefile, bible_local.XML);                            
                        }
                        commented = false;
                        if(form_closing)
                            OpenBible();
                    }
                }
            }
        }       

        private void SimpleBibleReader_FormClosing(Object sender,FormClosingEventArgs e)
        {
            DialogResult dialog_result = DialogResult.Cancel;
            if (commented)
            {
                dialog_result = Themes.MessageBox("Do you want to save comments to Bible?", "Comments not saved", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (dialog_result == DialogResult.Yes)
                {
                    form_closing = true;
                    saveToolStripMenuItem.PerformClick();
                }
                else if (dialog_result == DialogResult.Cancel)
                {                   
                    e.Cancel = true;
                }
            }
            
        }

        private void openCommentaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            readerOpenDialog.Filter = CommentaryFormat.getFilters();
            int result = (int)(readerOpenDialog.ShowDialog());
            if (result == System.Convert.ToInt32(System.Windows.Forms.DialogResult.OK))
            {
                commentaryfile = readerOpenDialog.FileName;
                OpenCommentary();
            }
        }

        public void OpenCommentary()
        {
            if (exporting_commentary)
            {
                ChapterArea.NavigateToString("<html><head>" + Themes.CSS_UI + "</head><body><h3>Opening Commentary during export is disabled</h3><p>You can read the Commentary during export, but opening another is not possible. If you wish to cancel the export, just close and re-open this software or open another instance of the same software.</p></body></html>");
                return;
            }
            if (backgroundWorker2.IsBusy)
            {
                ChapterArea.NavigateToString("<html><head>" + Themes.CSS_UI + "</head><body><h3>Commentary already being opened</h3><p>Please be patient as another Commentary is being opened. Depending on the format, it may take a while.</p></body></html>");
                return;
            }
            pictureBoxFront.Visible = false;
            timer2.Enabled = true;
            StatusStrip1.Visible = true;
            exporting_commentary = false;
            backgroundWorker2.RunWorkerAsync();
        }

        private void backgroundWorker2_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (exporting_commentary)
            {
                cmtry_format_export = CommentaryFormat.doExportCommentary(export_filename, export_filter_idx);
                if (cmtry_format_export == null)
                {
                    throw new Exception("doExportCommentary created a null bible object.");
                }
                cmtry_format_export.ExportCommentary(export_filename, export_filter_idx);
            }
            else
            {
                CommentaryFormat.CommentaryFactory(commentaryfile);
                if (CommentaryFormat.getInstance() != null)
                {
                    CommentaryFormat.getInstance().Load();
                    //RTL.Checked = false;
                    CommentaryFormat.getInstance().populateVerses(cmtListBox);
                    //data = commentary.populateBooks(commentaryfile, MirrorTreeView1);
                }
                else
                {
                    try_dict = true;
                }
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (CommentaryFormat.getInstance() == null)
            {
                return;
            }
            try
            {
                if (exporting_commentary)
                {
                    StatusText.Text = cmtry_format_export.PercentComplete.ToString() + "%";
                    ProgressBar.Value = cmtry_format_export.PercentComplete < 0 ? 0 : cmtry_format_export.PercentComplete;
                }
                else
                {
                    StatusText.Text = CommentaryFormat.getInstance().PercentComplete.ToString() + "%";
                    ProgressBar.Value = CommentaryFormat.getInstance().PercentComplete < 0 ? 0 : CommentaryFormat.getInstance().PercentComplete;
                }
            }
            catch (Exception)
            {
                //BackgroundWorker may not have started setting the progress bar.. just ignore..
            }
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (exporting_commentary)
            {
                timer2.Enabled = false;
                StatusStrip1.Visible = false;
                ProgressBar.Value = 0;
                StatusText.Text = "0%";
                exporting_commentary = false;
            }
            else
            {
                if (try_dict)
                {
                    try_dict = false;
                    dictionaryfile = commentaryfile;
                    commentaryfile = null;
                    OpenDictionary();
                    return;
                }
                else
                {
                    try
                    {
                        if (e.Error == null)
                        {
                            setTitles();
                            if (BibleFormat.getInstance() != null)
                                ChapterArea.NavigateToString(BibleFormat.getInstance().getChapterHtml(data, paraBtn.Checked, CommentaryFormat.getInstance()));
                            else
                                ChapterArea.NavigateToString("<html dir=\'ltr\'><head>" + Themes.CSS_MAIN + "</head><body>No Bibles loaded yet. Reading commentaries along with a bible gives you better experience in understanding commentaries as they can be linked with each other.</body></html>");


                            timer2.Enabled = false;
                            StatusStrip1.Visible = false;
                            ProgressBar.Value = 0;
                            StatusText.Text = "0%";
                            exportCommentaryToolStripMenuItem.Enabled = true;
                            cmtEditBtn.Enabled = true;
                            cmtZoomInBtn.Enabled = true;
                            cmtZoomOutBtn.Enabled = true;
                            cmtFontBtn.Enabled = true;
                            cmtPrintBtn.Enabled = true;

                            if (!commentaryToolStripMenuItem.Checked)
                                commentaryToolStripMenuItem.PerformClick();
                            if (cmtListBox.SelectedIndex == -1)
                                cmtListBox.SelectedIndex = 0;

                            if (GlobalMemory.getInstance().EnablePlugins)
                            {
                                PluginProcessFrm frm = new PluginProcessFrm(CommentaryFormat.CommentaryXmlDocument, PluginProcessFrm.TYPE_CMTY);
                                frm.ShowDialog(this);
                            }
                        }
                        else
                        {
                            ChapterArea.NavigateToString(CommentaryFormat.getInstance().getReaderErrorHtml(e.Error.GetBaseException()));
                            StatusStrip1.Visible = false;
                            ProgressBar.Value = 0;
                            StatusText.Text = "0%";
                        }

                    }
                    catch (Exception ex)
                    {
                        if (CommentaryFormat.getInstance() == null)
                        {
                            Themes.MessageBox("something went wrong in my code ;(");
                            return;
                        }
                        ChapterArea.NavigateToString(CommentaryFormat.getInstance().getReaderErrorHtml(ex));
                        StatusStrip1.Visible = false;
                        ProgressBar.Value = 0;
                        StatusText.Text = "0%";
                    }
                }
            }
        }

        private void exportCommentaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            readerSaveDialog.Filter = CommentaryFormat.getFilters();
            int result = (int)(readerSaveDialog.ShowDialog());
            if (result == System.Convert.ToInt32(System.Windows.Forms.DialogResult.OK))
            {
                export_filename = readerSaveDialog.FileName.Trim();
                export_filter_idx = readerSaveDialog.FilterIndex;
                timer2.Enabled = true;
                StatusStrip1.Visible = true;
                ProgressBar.Value = 0;
                StatusText.Text = "0%";
                exporting_commentary = true;
                backgroundWorker2.RunWorkerAsync();
            }
        }


        public void setProgressStatus(int progressValue,string statusText)
        {
            StatusStrip1.Visible = true;
            ProgressBar.Value = progressValue;
            StatusText.Text = statusText;
        }

        public void closeProgressStatus()
        {
            StatusStrip1.Visible = false;
            ProgressBar.Value = 0;
            StatusText.Text = "";
        }

        private void exportBibleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            readerSaveDialog.Filter = BibleFormat.getFilters();
            int result = (int)(readerSaveDialog.ShowDialog());
            if (result == System.Convert.ToInt32(System.Windows.Forms.DialogResult.OK))
            {
                export_filename = readerSaveDialog.FileName.Trim();
                export_filter_idx = readerSaveDialog.FilterIndex;

                /// - show export properties before export
                bible_format_export = BibleFormat.doExportBible(export_filename, export_filter_idx);
                List<string[]> kv = bible_format_export.getProperties();
                if (propDialog == null)
                    propDialog = new PropertiesFrm();
                propDialog.Text = "Verify Bible Properties for Export";
                propDialog.setProperties(kv);
                propDialog.ShowDialog(this);
                if (propDialog.RESULT == DialogResult.OK)
                {
                    kv = propDialog.getProperties();
                    bible_format_export.setProperties(kv);
                    for (int i = 0; i < kv.Count; i++)
                    {
                       
                        
                        switch (kv[i][0])
                        {
                            case "Abbreviation:":
                                BibleFormat.getInstance().ABBREVIATION = kv[i][1];
                                break;
                            case "Description:":
                                BibleFormat.getInstance().DESCRIPTION = kv[i][1];
                                break;
                            case "Comments:":
                                BibleFormat.getInstance().COMMENTS = kv[i][1];
                                break;
                        }
                    }
                }
                ////

                Timer1.Enabled = true;
                StatusStrip1.Visible = true;
                ProgressBar.Value = 0;
                StatusText.Text = "0%";
                exporting_bible = true;
                BackgroundWorker1.RunWorkerAsync();
            }
        }

        private void booksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SplitContainer1.Panel1Collapsed = !booksToolStripMenuItem.Checked;
        }

        private void bibleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            splitContainer3.Panel1Collapsed = !bibleToolStripMenuItem.Checked;
        }

        private void searchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            splitContainer2.Panel2Collapsed = !searchToolStripMenuItem.Checked;
        }

        private void commentaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            splitContainer4.Panel1Collapsed = !commentaryToolStripMenuItem.Checked;
            splitContainer3.Panel2Collapsed = splitContainer4.Panel1Collapsed;
            if (!dictionaryToolStripMenuItem.Checked)
                splitContainer4.Panel2Collapsed = true;
        }

        private void dictionaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            splitContainer4.Panel2Collapsed = !dictionaryToolStripMenuItem.Checked;
            splitContainer3.Panel2Collapsed = splitContainer4.Panel1Collapsed;
            if (!commentaryToolStripMenuItem.Checked)
                splitContainer4.Panel1Collapsed = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            searchResultExpand(SearchResultsArea);
            //<li class='Collapsed'>
            //SearchResultsArea.NavigateToString(getWebView2Html(SearchResultsArea).Replace("<li class='Collapsed'>", "<li class=''>"));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            searchResultCollapse(SearchResultsArea);
            //SearchResultsArea.NavigateToString(getWebView2Html(SearchResultsArea).Replace("<li class=''>", "<li class='Collapsed'>"));
        }

        private void doZoom()
        {
            if (GlobalMemory.getInstance().ZOOMFACTOR_IDX_BIB < 1)
                GlobalMemory.getInstance().ZOOMFACTOR_IDX_BIB = 1;
            if (GlobalMemory.getInstance().ZOOMFACTOR_IDX_BIB > 2)
                GlobalMemory.getInstance().ZOOMFACTOR_IDX_BIB = 2;

            ChapterArea.ZoomFactor = GlobalMemory.getInstance().ZOOMFACTOR_IDX_BIB;
        }

        private void doCommentaryZoom()
        {
            if (GlobalMemory.getInstance().ZOOMFACTOR_IDX_CMT < 1)
                GlobalMemory.getInstance().ZOOMFACTOR_IDX_CMT = 1;
            if (GlobalMemory.getInstance().ZOOMFACTOR_IDX_CMT > 2)
                GlobalMemory.getInstance().ZOOMFACTOR_IDX_CMT = 2;

            CommentaryArea.ZoomFactor = GlobalMemory.getInstance().ZOOMFACTOR_IDX_CMT;
        }

        private void doDictionaryZoom()
        {
            if (GlobalMemory.getInstance().ZOOMFACTOR_IDX_DICT < 1)
                GlobalMemory.getInstance().ZOOMFACTOR_IDX_DICT = 1;
            if (GlobalMemory.getInstance().ZOOMFACTOR_IDX_DICT > 2)
                GlobalMemory.getInstance().ZOOMFACTOR_IDX_DICT = 2;

            DictionaryArea.ZoomFactor = GlobalMemory.getInstance().ZOOMFACTOR_IDX_DICT;
        }

        private void zoomInBtn_Click(object sender, EventArgs e)
        {
            GlobalMemory.getInstance().ZOOMFACTOR_IDX_BIB += (0.1);
            doZoom();
        }

        private void zoomOutBtn_Click(object sender, EventArgs e)
        {
            GlobalMemory.getInstance().ZOOMFACTOR_IDX_BIB -= (0.1);
            doZoom();
        }

        private void findBtn_Click(object sender, EventArgs e)
        {
            if (find == null) 
                find = new FindWebView2(SearchResultsArea, this);
            find.ShowDialog(this);            
        }

        private void fullScreenBtn_CheckedChanged(object sender, EventArgs e)
        {
            if (fullScreenBtn.Checked)
            {
                savePanellayout();

                booksToolStripMenuItem.Checked = false;
                bibleToolStripMenuItem.Checked = true;
                searchToolStripMenuItem.Checked = false;
                dictionaryToolStripMenuItem.Checked = false;
                commentaryToolStripMenuItem.Checked = false;

                SplitContainer1.Panel1Collapsed = true;
                splitContainer2.Panel2Collapsed = true;
                splitContainer3.Panel2Collapsed = true;
            }
            else
            {
                loadPanellayout();
            }
        }

        private void fullScreenSearchBtn_CheckedChanged(object sender, EventArgs e)
        {
            if (fullScreenSearchBtn.Checked)
            {
                savePanellayout();

                booksToolStripMenuItem.Checked = false;
                bibleToolStripMenuItem.Checked = false;
                searchToolStripMenuItem.Checked = true;
                dictionaryToolStripMenuItem.Checked = false;
                commentaryToolStripMenuItem.Checked = false;

                SplitContainer1.Panel1Collapsed = true;
                splitContainer2.Panel1Collapsed = true;
            }
            else
            {
                loadPanellayout();
            }
        }

        private void printChapterBtn_Click(object sender, EventArgs e)
        {
            ChapterArea.CoreWebView2.ShowPrintUI();
        }

        private void moreVersesBtn_CheckedChanged(object sender, EventArgs e)
        {
            if (find == null) 
                find = new FindWebView2(SearchResultsArea, this);
            find.showExtraVerses = moreVersesBtn.Checked;
            if (!find.searchBackgroundJob.IsBusy)
                find.searchBackgroundJob.RunWorkerAsync();
        }

        private TreeNode findNode(string path)
        {
            foreach (TreeNode testament in MirrorTreeView1.Nodes)
            {
                foreach (TreeNode book in testament.Nodes)
                {
                    foreach (TreeNode chapter in book.Nodes)
                    {
                        if (chapter.FullPath == path)
                            return chapter;
                    }
                }
            }
            return null;
        }

        private void backBtn_Click(object sender, EventArgs e)
        {            
            versionHistoryCounter--;
            frontBtn.Enabled = true;
            if (versionHistoryCounter <= 0)
            {
                versionHistoryCounter = 0;
                backBtn.Enabled = false;
            }
            programatic_action = true;
            string path = versionHistory[versionHistoryCounter];
            MirrorTreeView1.SelectedNode = findNode(path);
            MirrorTreeView1.Focus();
            MirrorTreeView1.SelectedNode.EnsureVisible();
            programatic_action = false;
        }

        private void frontBtn_Click(object sender, EventArgs e)
        {
            versionHistoryCounter++;
            if (versionHistoryCounter >= versionHistory.Count - 1)
            {
                versionHistoryCounter = versionHistory.Count - 1;
                frontBtn.Enabled = false;
            }
            else
                backBtn.Enabled = true;
            programatic_action = true;
            string path = versionHistory[versionHistoryCounter];            
            MirrorTreeView1.SelectedNode = findNode(path);
            MirrorTreeView1.Focus();
            MirrorTreeView1.SelectedNode.EnsureVisible();
            programatic_action = false;
        }

        private void timerSelectVerse_Tick(object sender, EventArgs e)
        {
            timerSelectVerse.Enabled = false;

            string para_style = "linear";
            if (paraBtn.Checked)
                para_style = "para";

            string clearJS = @"
var a = document.getElementsByTagName('a');
for (var idx = 0; idx < a.length; ++idx)
{
    a[idx].className = '"+para_style+@"';
}
var elem = document.getElementById('" + verse_to_highlight + @"');
elem.className = 'selectedverse';
elem.scrollIntoView(true);";

            ChapterArea.ExecuteScriptAsync(clearJS);
        }

        private void timer5_Tick(object sender, EventArgs e)
        {
            timer5.Enabled = false;
            if (CommentaryFormat.getInstance() == null)
                return;
            string commentary_text = CommentaryFormat.getInstance().getCommentaryForVerse(currentSelectedBook, currentSelectedChapter, currentSelectedVerse);
            commentaryTitleLbl.Text = "[" +Path.GetFileName(CommentaryFormat.getInstance().FileName) + "] " + FormatUtil.shortBookNames[currentSelectedBook - 1] + " " + currentSelectedChapter.ToString() + ":" + currentSelectedVerse.ToString();
            if (commentary_text != "")
            {
                CommentaryArea.NavigateToString(commentary_text);
                if(!commentaryToolStripMenuItem.Checked)
                    commentaryToolStripMenuItem.PerformClick();
            }
            else
                CommentaryArea.NavigateToString("");
        }

        private void cmtZoomInBtn_Click(object sender, EventArgs e)
        {
            GlobalMemory.getInstance().ZOOMFACTOR_IDX_CMT += (0.1);
            doCommentaryZoom();
        }

        private void cmtZoomOutBtn_Click(object sender, EventArgs e)
        {
            GlobalMemory.getInstance().ZOOMFACTOR_IDX_CMT -= (0.1);
            doCommentaryZoom();
        }

        private void cmtFullScreenBtn_CheckedChanged(object sender, EventArgs e)
        {
            if (cmtFullScreenBtn.Checked)
            {
                savePanellayout();

                booksToolStripMenuItem.Checked = false;
                bibleToolStripMenuItem.Checked = false;
                searchToolStripMenuItem.Checked = false;
                dictionaryToolStripMenuItem.Checked = false;
                commentaryToolStripMenuItem.Checked = true;

                SplitContainer1.Panel1Collapsed = true;
                splitContainer2.Panel2Collapsed = true;
                splitContainer3.Panel1Collapsed = true;
                splitContainer4.Panel2Collapsed = true;
            }
            else
            {
                loadPanellayout();
            }

        }

        private void cmtPrintBtn_Click(object sender, EventArgs e)
        {
            CommentaryArea.CoreWebView2.ShowPrintUI();
        }

        private void releaseNotesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChapterArea.NavigateToString(GlobalMemory.changeReleaseHtml());
        }

        private void cmtListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox listbox = (ListBox)sender;
            string item=listbox.SelectedItem.ToString();
            string[] refs = item.Split(':');
            currentSelectedVerse = int.Parse(refs[1]);
            int idx = refs[0].LastIndexOf(' ');
            string book = refs[0].Substring(0, idx);
            currentSelectedChapter = int.Parse( refs[0].Substring(idx));
            for (int b = 0; b < 66; b++)
            {
                if (BibleFormat.getInstance() != null)
                {                    
                    if (BibleFormat.getInstance().getBookName(b + 1) == book)
                    {
                        currentSelectedBook = b + 1;
                        break;
                    }
                }
                else
                {
                    if (FormatUtil.shortBookNames[b] == book)
                    {
                        currentSelectedBook = b + 1;
                        break;
                    }
                }
            }
            timer5.Enabled = true;
        }

        private void paraBtn_CheckedChanged(object sender, EventArgs e)
        {
            string[] data = versionHistory[versionHistory.Count-1].Split(MirrorTreeView1.PathSeparator.ToCharArray());
            ChapterArea.NavigateToString(BibleFormat.getInstance().getChapterHtml(data, paraBtn.Checked, CommentaryFormat.getInstance()));            
        }

        private void chkBoxShowList_CheckedChanged(object sender, EventArgs e)
        {
            splitContainer5.Panel1Collapsed = !chkBoxShowList.Checked;
        }

        private void fontBtn_Click(object sender, EventArgs e)
        {
            if (fontDialog == null)
            {
                if(is_bible)
                    fontDialog = new FontFrm(BibleFormat.bible_format.getFirstVerse(), GlobalMemory.getInstance().FontFamilyBIB);
                else
                    fontDialog = new FontFrm(BookFormat.book_format.getFirstVerse(), GlobalMemory.getInstance().FontFamilyBIB);
            }
            fontDialog.ShowDialog(this);
            if (fontDialog.RESULT == DialogResult.OK)
            {
                GlobalMemory.getInstance().FontFamilyBIB = fontDialog.getFontFamily();
                //string txt = getWebView2Html(ChapterArea);
                //ChapterArea.NavigateToString(Regex.Replace(txt, "font-family: ([^;])*;", "font-family: " + GlobalMemory.getInstance().FontFamilyBIB + ";"));               
                changeFont(ChapterArea, GlobalMemory.getInstance().FontFamilyBIB);
            }
        }

        private void cmtListBox_DoubleClick(object sender, EventArgs e)
        {
            ListBox listbox = (ListBox)sender;
            string item = listbox.SelectedItem.ToString();
            string[] refs = item.Split(':');
            currentSelectedVerse = int.Parse(refs[1]);
            int idx = refs[0].LastIndexOf(' ');
            string book = refs[0].Substring(0, idx);
            currentSelectedChapter = int.Parse(refs[0].Substring(idx));
            for (int b = 0; b < 66; b++)
            {
                if (BibleFormat.getInstance() != null)
                {
                    if (BibleFormat.getInstance().getBookName(b + 1) == book || FormatUtil.getBookNo(book) == b+1) 
                    {
                        currentSelectedBook = b + 1;
                        break;
                    }
                }
                else
                {
                    if (Localization.getBookNames()[b] == book)
                    {
                        currentSelectedBook = b + 1;
                        break;
                    }
                }
            }
            timer5.Enabled = true;
            //------------- navigate in bible chapter
            //book = BibleFormat.bible_format.getBookName(currentSelectedBook);
            string chapter = currentSelectedChapter.ToString();
            string verse = currentSelectedVerse.ToString();
            if (MirrorTreeView1.Nodes.Count == 0)
                return;
            TreeNode tn = findNode(MirrorTreeView1.Nodes[0].FullPath + MirrorTreeView1.PathSeparator + book + MirrorTreeView1.PathSeparator + chapter);
            if (tn == null)
                tn = findNode(MirrorTreeView1.Nodes[1].FullPath + MirrorTreeView1.PathSeparator + book + MirrorTreeView1.PathSeparator + chapter);
            
            if (tn == null && currentSelectedBook <= 39)
            {

                book = MirrorTreeView1.Nodes[0].Nodes[currentSelectedBook - 1].Text;
                tn = findNode(MirrorTreeView1.Nodes[0].FullPath + MirrorTreeView1.PathSeparator + book + MirrorTreeView1.PathSeparator + chapter);
            }
            if (tn == null && currentSelectedBook > 39)
            {
                book = MirrorTreeView1.Nodes[1].Nodes[currentSelectedBook - 39 -1].Text;
                tn = findNode(MirrorTreeView1.Nodes[1].FullPath + MirrorTreeView1.PathSeparator + book + MirrorTreeView1.PathSeparator + chapter);
            }
            //programatic_action = true;
            MirrorTreeView1.SelectedNode = tn;
            MirrorTreeView1.Focus();
            MirrorTreeView1.SelectedNode.EnsureVisible();
            //programatic_action = false;
            verse_to_highlight = verse;
            timerSelectVerse.Enabled = true;
        }

        private void chkBoxShowDictList_CheckedChanged(object sender, EventArgs e)
        {
            splitContainer6.Panel1Collapsed = !chkBoxShowDictList.Checked;
        }

        private void dctFullScreenBtn_CheckedChanged(object sender, EventArgs e)
        {
            if (dctFullScreenBtn.Checked)
            {
                savePanellayout();

                booksToolStripMenuItem.Checked = false;
                bibleToolStripMenuItem.Checked = false;
                searchToolStripMenuItem.Checked = false;
                dictionaryToolStripMenuItem.Checked = true;
                commentaryToolStripMenuItem.Checked = false;

                SplitContainer1.Panel1Collapsed = true;
                splitContainer2.Panel2Collapsed = true;
                splitContainer3.Panel1Collapsed = true;
                splitContainer4.Panel1Collapsed = true;
            }
            else
            {
                loadPanellayout();
            }
        }

        private void savePanellayout()
        {
            prev_s1p1 = SplitContainer1.Panel1Collapsed;
            prev_s1p2 = SplitContainer1.Panel2Collapsed;
            prev_s2p1 = splitContainer2.Panel1Collapsed; 
            prev_s2p2 = splitContainer2.Panel2Collapsed; 
            prev_s3p1 = splitContainer3.Panel1Collapsed; 
            prev_s3p2 = splitContainer3.Panel2Collapsed;
            prev_s4p1 = splitContainer4.Panel1Collapsed;  // commentary
            prev_s4p2 = splitContainer4.Panel2Collapsed;  // dictionary
            prev_s5p1 = splitContainer5.Panel1Collapsed; 
            prev_s5p2 = splitContainer5.Panel2Collapsed; 
            prev_s6p1 = splitContainer6.Panel1Collapsed; 
            prev_s6p2 = splitContainer6.Panel2Collapsed; 
        }

        private void loadPanellayout()
        {
            SplitContainer1.Panel1Collapsed=prev_s1p1;
            SplitContainer1.Panel2Collapsed=prev_s1p2;
            splitContainer2.Panel1Collapsed=prev_s2p1; 
            splitContainer2.Panel2Collapsed=prev_s2p2; 
            splitContainer3.Panel1Collapsed=prev_s3p1; 
            splitContainer3.Panel2Collapsed=prev_s3p2;
            splitContainer4.Panel1Collapsed=prev_s4p1;
            splitContainer4.Panel2Collapsed=prev_s4p2;
            splitContainer5.Panel1Collapsed=prev_s5p1; 
            splitContainer5.Panel2Collapsed=prev_s5p2; 
            splitContainer6.Panel1Collapsed=prev_s6p1; 
            splitContainer6.Panel2Collapsed=prev_s6p2;

            booksToolStripMenuItem.Checked = !prev_s1p1;
            bibleToolStripMenuItem.Checked = !prev_s2p1 && !prev_s3p1;
            searchToolStripMenuItem.Checked = !prev_s2p2;
            commentaryToolStripMenuItem.Checked = !prev_s3p2 && !prev_s4p1;
            dictionaryToolStripMenuItem.Checked = !prev_s3p2 && !prev_s4p2;
        }

        private void dictZoomInBtn_Click(object sender, EventArgs e)
        {
            GlobalMemory.getInstance().ZOOMFACTOR_IDX_DICT += (0.1);
            doDictionaryZoom();
        }

        private void dictZoomOutBtn_Click(object sender, EventArgs e)
        {
            GlobalMemory.getInstance().ZOOMFACTOR_IDX_DICT -= (0.1);
            doDictionaryZoom();
        }

        private void dictPrintBtn_Click(object sender, EventArgs e)
        { 
            DictionaryArea.CoreWebView2.ShowPrintUI();
        }

        private void openDictionaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            readerOpenDialog.Filter = DictionaryFormat.getFilters();
            int result = (int)(readerOpenDialog.ShowDialog());
            if (result == System.Convert.ToInt32(System.Windows.Forms.DialogResult.OK))
            {
                dictionaryfile = readerOpenDialog.FileName;
                OpenDictionary();
            }
        }

        public void OpenDictionary()
        {
            if (exporting_dictionary)
            {
                ChapterArea.NavigateToString("<html><head>" + Themes.CSS_UI + "</head><body><h3>Opening Dictionary during export is disabled</h3><p>You can read during export, but opening another is not possible. If you wish to cancel the export, just close and re-open this software or open another instance of the same software.</p></body></html>");
                return;
            }
            if (backgroundWorker3.IsBusy)
            {
                ChapterArea.NavigateToString("<html><head>" + Themes.CSS_UI + "</head><body><h3>Dictionary already being opened</h3><p>Please be patient as another Dictionary is being opened. Depending on the format, it may take a while.</p></body></html>");
                return;
            }
            pictureBoxFront.Visible = false;
            timer6.Enabled = true;
            StatusStrip1.Visible = true;
            exporting_dictionary = false;
            backgroundWorker3.RunWorkerAsync();
        }

        private void cmtFontBtn_Click(object sender, EventArgs e)
        {
            if (fontDialog == null)
                fontDialog = new FontFrm(CommentaryFormat.getInstance().getFirstCommentary(), GlobalMemory.getInstance().FontFamilyCMT);
            fontDialog.ShowDialog(this);
            if (fontDialog.RESULT == DialogResult.OK)
            {
                GlobalMemory.getInstance().FontFamilyCMT = fontDialog.getFontFamily();
                changeFont(CommentaryArea, GlobalMemory.getInstance().FontFamilyCMT);

                //string txt = getWebView2Html(CommentaryArea);
                //CommentaryArea.NavigateToString(Regex.Replace(txt, "font-family: ([^;])*;", "font-family: " + GlobalMemory.getInstance().FontFamilyCMT + ";"));
            }
        }

        private void timer6_Tick(object sender, EventArgs e)
        {
            if (DictionaryFormat.getInstance() == null)
            {
                return;
            }
            try
            {
                if (exporting_dictionary)
                {
                    StatusText.Text = dict_format_export.PercentComplete.ToString() + "%";
                    ProgressBar.Value = dict_format_export.PercentComplete < 0 ? 0 : dict_format_export.PercentComplete;
                }
                else
                {
                    StatusText.Text = DictionaryFormat.getInstance().PercentComplete.ToString() + "%";
                    ProgressBar.Value = DictionaryFormat.getInstance().PercentComplete < 0 ? 0 : DictionaryFormat.getInstance().PercentComplete;
                }
            }
            catch (Exception)
            {
                //BackgroundWorker may not have started setting the progress bar.. just ignore..
            }
        }

        private void backgroundWorker3_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (exporting_dictionary)
            {
                dict_format_export = DictionaryFormat.doExportDictionary(export_filename, export_filter_idx);
                if (dict_format_export == null)
                {
                    throw new Exception("doExportDict created a null dict object.");
                }
                dict_format_export.ExportDictionary(export_filename, export_filter_idx);

            }
            else
            {
                DictionaryFormat.DictionaryFactory(dictionaryfile);
                if (DictionaryFormat.getInstance() != null)
                {
                    DictionaryFormat.getInstance().Load();
                    //RTL.Checked = false;
                    DictionaryFormat.getInstance().populateTopics(dictListBox);
                }
                else
                {
                    try_book = true;
                }
            }
        }

        private void backgroundWorker3_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (exporting_dictionary)
            {
                timer6.Enabled = false;
                StatusStrip1.Visible = false;
                ProgressBar.Value = 0;
                StatusText.Text = "0%";
                exporting_dictionary = false;
            }
            else
            {
                if (try_book)
                {
                    try_book = false;
                    is_bible = false;                    
                    bookfile = dictionaryfile;
                    dictionaryfile = null;
                    OpenBook();
                    return;
                }

                try
                {
                    if (e.Error == null)
                    {
                        setTitles();
                        if (BibleFormat.getInstance() != null)
                            ChapterArea.NavigateToString(BibleFormat.getInstance().getChapterHtml(data, paraBtn.Checked, CommentaryFormat.getInstance()));
                        else
                            ChapterArea.NavigateToString("<html dir=\'ltr\'><head>" + Themes.CSS_MAIN + "</head><body>No Bibles loaded yet. Reading dictionaries along with a bible gives you better experience in understanding the words used with context as they can be linked with each other.</body></html>");

                        timer6.Enabled = false;
                        StatusStrip1.Visible = false;
                        ProgressBar.Value = 0;
                        StatusText.Text = "0%";
                        exportDictionaryToolStripMenuItem.Enabled = true;
                        dictEditBtn.Enabled = true;
                        dictZoomInBtn.Enabled = true;
                        dictZoomOutBtn.Enabled = true;
                        dctFontBtn.Enabled = true;
                        dictPrintBtn.Enabled = true;

                        if (!dictionaryToolStripMenuItem.Checked)
                            dictionaryToolStripMenuItem.PerformClick();
                        if (dictListBox.SelectedIndex == -1)
                            dictListBox.SelectedIndex = 0;

                        if (GlobalMemory.getInstance().EnablePlugins)
                        {
                            PluginProcessFrm frm = new PluginProcessFrm(DictionaryFormat.DictionaryXmlDocument, PluginProcessFrm.TYPE_DICT);
                            frm.ShowDialog(this);
                        }
                    }
                    else
                    {                        
                        ChapterArea.NavigateToString(DictionaryFormat.getInstance().getReaderErrorHtml(e.Error.GetBaseException()));
                        StatusStrip1.Visible = false;
                        ProgressBar.Value = 0;
                        StatusText.Text = "0%";
                    }
                }
                catch (Exception ex)
                {
                    if (DictionaryFormat.getInstance() == null)
                    {
                        Themes.MessageBox("something went wrong in my code ;(");
                        return;
                    }
                    ChapterArea.NavigateToString(DictionaryFormat.getInstance().getReaderErrorHtml(ex));
                    StatusStrip1.Visible = false;
                    ProgressBar.Value = 0;
                    StatusText.Text = "0%";
                }
            }
        }

        private void dictListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox listbox = (ListBox)sender;
            string item = listbox.SelectedItem.ToString();
            DictionaryArea.NavigateToString(DictionaryFormat.getInstance().getTopicHtml(item));
        }


        private void exportDictionaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            readerSaveDialog.Filter = DictionaryFormat.getFilters();
            int result = (int)(readerSaveDialog.ShowDialog());
            if (result == System.Convert.ToInt32(System.Windows.Forms.DialogResult.OK))
            {
                export_filename = readerSaveDialog.FileName.Trim();
                export_filter_idx = readerSaveDialog.FilterIndex;
                timer6.Enabled = true;
                StatusStrip1.Visible = true;
                ProgressBar.Value = 0;
                StatusText.Text = "0%";
                exporting_dictionary = true;
                backgroundWorker3.RunWorkerAsync();
            }
        }

        private void backgroundCleaner_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            setProgressStatus(e.ProgressPercentage, "Cleaning Texts .." + e.ProgressPercentage.ToString()+" %");
        }

        private void backgroundCleaner_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            // do cleaning at background.           

           XmlNodeList verses;
            // background cleaner is only for Bible and Books. cleaning on dictionary and commentary takes place during load.
            Object classtype = null;
            if (is_bible)
            {
                verses = BibleFormat.getInstance().getXmlDoc().SelectNodes("//VERS");
                classtype = BibleFormat.getInstance();
            }
            else
            {
                verses = BookFormat.getInstance().getXmlDoc().SelectNodes("//NOTES");
                classtype = BookFormat.getInstance();
            }

           if (verses == null)
               return;
            int count = 0;
            int p_pecent = 0;

            foreach (XmlNode verse in verses)
            {
                try
                {
                    if (p_pecent != ((count * 100) / verses.Count))
                        backgroundCleaner.ReportProgress(((count * 100) / verses.Count));
                    p_pecent = ((count * 100) / verses.Count);
                    count++;

                    //verse.InnerXml = FormatUtil.CleanRTF(verse.InnerText);
                    //verse.InnerXml = FormatUtil.ConvertRtfToHtml(verse.InnerText, classtype);
                    if(is_bible)
                        verse.InnerXml = BibleFormat.postProcessContent(verse.InnerText);
                    else
                        verse.InnerXml = BookFormat.postProcessContent(verse.InnerText);
                }
                catch (Exception)
                {
                    //Console.WriteLine(ex.Message);                   
                }
                
            }
            //
            //now add red text for all background cleaning bibles
            if (GlobalMemory.getInstance().addRedText && is_bible)
            {
                BibleFormat.getInstance().addRedText();
            }
        }

        private void backgroundCleaner_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            //cleaning done.
            if (is_bible)
            {
                BibleFormat.getInstance().CLEANING = false;
                closeProgressStatus();
                if (last_selected_node == null)
                    ChapterArea.NavigateToString(BibleFormat.getInstance().getChapterHtml(data, paraBtn.Checked, CommentaryFormat.getInstance()));
                else
                {
                    string[] d = last_selected_node.FullPath.Split(MirrorTreeView1.PathSeparator.ToCharArray());
                    ChapterArea.NavigateToString(BibleFormat.getInstance().getChapterHtml(d, paraBtn.Checked, CommentaryFormat.getInstance()));
                }
                //

                if (GlobalMemory.getInstance().EnablePlugins)
                {
                    PluginProcessFrm frm = new PluginProcessFrm(BibleFormat.BibleXmlDocument, PluginProcessFrm.TYPE_BIBL);
                    frm.ShowDialog(this);
                }
                                
            }
            else
            {
                BookFormat.getInstance().CLEANING = false;
                closeProgressStatus();
                if (last_selected_node == null)
                    ChapterArea.NavigateToString(BookFormat.getInstance().getChapterHtml(data));
                else
                {
                    string[] d = last_selected_node.FullPath.Split(MirrorTreeView1.PathSeparator.ToCharArray());
                    ChapterArea.NavigateToString(BookFormat.getInstance().getChapterHtml(d));
                }

                if (GlobalMemory.getInstance().EnablePlugins)
                {
                    PluginProcessFrm frm = new PluginProcessFrm(BookFormat.BookXmlDocument, PluginProcessFrm.TYPE_BOOK);
                    frm.ShowDialog(this);
                }
            }
        }

        private void dctFontBtn_Click(object sender, EventArgs e)
        {
            if (fontDialog == null)
                fontDialog = new FontFrm(DictionaryFormat.getInstance().getFirstDictItem(), GlobalMemory.getInstance().FontFamilyDICT);
            fontDialog.ShowDialog(this);
            if (fontDialog.RESULT == DialogResult.OK)
            {
                GlobalMemory.getInstance().FontFamilyDICT = fontDialog.getFontFamily();
                changeFont(DictionaryArea, GlobalMemory.getInstance().FontFamilyDICT);

                //string txt = getWebView2Html( DictionaryArea);
                //DictionaryArea.NavigateToString(Regex.Replace(txt, "font-family: ([^;])*;", "font-family: " + GlobalMemory.getInstance().FontFamilyDICT + ";"));
            }
        }

        private void editBtn_Click(object sender, EventArgs e)
        {
            if (is_bible)
            {
                List<string[]> kv = new List<string[]>();

                kv.Add(new string[] { "Abbreviation:", BibleFormat.getInstance().ABBREVIATION });
                kv.Add(new string[] { "Description:", BibleFormat.getInstance().DESCRIPTION });
                kv.Add(new string[] { "Comments:", BibleFormat.getInstance().COMMENTS });

                if (propDialog == null)
                    propDialog = new PropertiesFrm();
                propDialog.Text = "Bible Properties";
                propDialog.setProperties(kv);
                propDialog.ShowDialog(this);
                if (propDialog.RESULT == DialogResult.OK)
                {
                    kv = propDialog.getProperties();
                    BibleFormat.getInstance().ABBREVIATION = kv[0][1];
                    BibleFormat.getInstance().DESCRIPTION = kv[1][1];
                    BibleFormat.getInstance().COMMENTS = kv[2][1];                    
                }
            }
            else
            {
                List<string[]> kv = new List<string[]>();

                kv.Add(new string[] { "Abbreviation:", BookFormat.getInstance().ABBREVIATION });
                kv.Add(new string[] { "Description:", BookFormat.getInstance().DESCRIPTION });
                kv.Add(new string[] { "Comments:", BookFormat.getInstance().COMMENTS });

                if (propDialog == null)
                    propDialog = new PropertiesFrm();
                propDialog.Text = "Book Properties";
                propDialog.setProperties(kv);
                propDialog.ShowDialog(this);
                if (propDialog.RESULT == DialogResult.OK)
                {
                    kv = propDialog.getProperties();
                    BookFormat.getInstance().ABBREVIATION = kv[0][1];
                    BookFormat.getInstance().DESCRIPTION = kv[1][1];
                    BookFormat.getInstance().COMMENTS = kv[2][1];
                }
            }
        }

        private void cmtEditBtn_Click(object sender, EventArgs e)
        {
            List<string[]> kv = new List<string[]>();
            
            kv.Add(new string[] { "Abbreviation:", CommentaryFormat.getInstance().ABBREVIATION });
            kv.Add(new string[] { "Description:", CommentaryFormat.getInstance().DESCRIPTION });
            kv.Add(new string[] { "Comments:", CommentaryFormat.getInstance().COMMENTS });

            if (propDialog == null)
                propDialog = new PropertiesFrm();
            propDialog.Text = "Commentary Properties";
            propDialog.setProperties(kv);
            propDialog.ShowDialog(this);
            if (propDialog.RESULT == DialogResult.OK)
            {
                kv = propDialog.getProperties();
                CommentaryFormat.getInstance().ABBREVIATION = kv[0][1];
                CommentaryFormat.getInstance().DESCRIPTION = kv[1][1];
                CommentaryFormat.getInstance().COMMENTS = kv[2][1];
            }
        }

        private void dictEditBtn_Click(object sender, EventArgs e)
        {
            List<string[]> kv = new List<string[]>();

            kv.Add(new string[] { "Abbreviation:",DictionaryFormat.getInstance().ABBREVIATION });
            kv.Add(new string[] {"Description:",DictionaryFormat.getInstance().DESCRIPTION });
            kv.Add(new string[] {"Comments:",DictionaryFormat.getInstance().COMMENTS });
            kv.Add(new string[] { "Strong:", DictionaryFormat.getInstance().STRONG });

            if (propDialog == null)
                propDialog = new PropertiesFrm();
            propDialog.Text = "Dictionary Properties";
            propDialog.setProperties(kv);
            propDialog.ShowDialog(this);
            if (propDialog.RESULT == DialogResult.OK)
            {
                kv = propDialog.getProperties();
                DictionaryFormat.getInstance().ABBREVIATION = kv[0][1];
                DictionaryFormat.getInstance().DESCRIPTION = kv[1][1];
                DictionaryFormat.getInstance().COMMENTS = kv[2][1];
                DictionaryFormat.getInstance().STRONG = kv[3][1];
            }
        }

        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (prefDialog == null)
                prefDialog = new PreferencesFrm();
            prefDialog.ShowDialog(this);
        }

        private void helpBtn_Click(object sender, EventArgs e)
        {
            Process.Start("https://" + GlobalMemory.AUTHOR_WEBSITE);
        }

        public static RendererWebView2 getInstance()
        {
            return defaultInstance;
        }

        //
        public ListBox getDictionaryListBox()
        {
            return dictListBox;
        }

        public ListBox getCommentaryListBox()
        {
            return cmtListBox;
        }
        //
        public void postDictOpen()
        {
            if (post_open_dict)
            {
                setTitles();
                if (BibleFormat.getInstance() != null)
                    ChapterArea.NavigateToString(BibleFormat.getInstance().getChapterHtml(data, paraBtn.Checked, CommentaryFormat.getInstance()));
                else
                    ChapterArea.NavigateToString("<html dir=\'ltr\'><head>" + Themes.CSS_MAIN + "</head><body>No Bibles loaded yet. Reading dictionaries along with a bible gives you better experience in understanding the words used with context as they can be linked with each other.</body></html>");

                exportDictionaryToolStripMenuItem.Enabled = true;
                dictEditBtn.Enabled = true;
                dictZoomInBtn.Enabled = true;
                dictZoomOutBtn.Enabled = true;
                dctFontBtn.Enabled = true;
                dictPrintBtn.Enabled = true;

                if (!dictionaryToolStripMenuItem.Checked)
                    dictionaryToolStripMenuItem.PerformClick();
                if (dictListBox.SelectedIndex == -1)
                    dictListBox.SelectedIndex = 0;
                post_open_dict = false;
            }                
        }

        public void postCmtOpen()
        {
            if (post_open_cmtry)
            {
                setTitles();
                if (BibleFormat.getInstance() != null)
                    ChapterArea.NavigateToString(BibleFormat.getInstance().getChapterHtml(data, paraBtn.Checked, CommentaryFormat.getInstance()));
                else
                    ChapterArea.NavigateToString("<html dir=\'ltr\'><head>" + Themes.CSS_MAIN + "</head><body>No Bibles loaded yet. Reading dictionaries along with a bible gives you better experience in understanding the words used with context as they can be linked with each other.</body></html>");

                exportCommentaryToolStripMenuItem.Enabled = true;
                cmtEditBtn.Enabled = true;
                cmtZoomInBtn.Enabled = true;
                cmtZoomOutBtn.Enabled = true;
                cmtFontBtn.Enabled = true;
                cmtPrintBtn.Enabled = true;

                if (!commentaryToolStripMenuItem.Checked)
                    commentaryToolStripMenuItem.PerformClick();
                if (cmtListBox.SelectedIndex == -1)
                    cmtListBox.SelectedIndex = 0;
                post_open_cmtry = false;
            }
        }

        /*
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            simple_bible_reader = new SimpleBibleReader();
            Application.Run(simple_bible_reader);            
        }
         */

        private void transliterateBtn_CheckedChanged(object sender, EventArgs e)
        {
            if (BibleFormat.getInstance() == null)
            {
                transliterateBtn.Enabled = false;
                transliterateBtn.Checked = false;
                return;
            }

            if (bkgndTransLitJob.IsBusy||backgroundCleaner.IsBusy || BackgroundWorker1.IsBusy || backgroundWorker2.IsBusy || backgroundWorker3.IsBusy)
            {
                ChapterArea.NavigateToString("<html><head>"+Themes.CSS_UI+"</head><body>I am currently busy with another process. Please try once the process is complete.</body></html>");
                return;
            }
            string[] data = versionHistory[versionHistory.Count - 1].Split(MirrorTreeView1.PathSeparator.ToCharArray());
            GlobalMemory.getInstance().Transliteration = transliterateBtn.Checked;
            setProgressStatus(0, "0 %");
            bkgndTransLitJob.RunWorkerAsync();
        }

        private void bkgndTransLitJob_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (GlobalMemory.getInstance().Transliteration)
            {

                XmlNodeList list = BibleFormat.getInstance().getXmlDoc().SelectNodes("/XMLBIBLE/BIBLEBOOK/CHAPTER/VERS");
                int count = 0;
                int percent = 0;
                foreach (XmlNode node in list)
                {
                    if (percent != (count * 50) / list.Count)
                    {
                        percent = (count * 50) / list.Count;
                        bkgndTransLitJob.ReportProgress(percent);
                    }
                    node.InnerXml = Unidecoder.Unidecode(node.InnerXml);
                    count++;
                }
                
                list = BibleFormat.getInstance().getXmlDoc().SelectNodes("/XMLBIBLE/BIBLEBOOK");
                XmlElement elmt = null;
                percent = 50;
                count = 0;
                foreach (XmlNode node in list)
                {
                    if (percent !=50+ (count * 50) / list.Count)
                    {
                        percent =50+ (count * 50) / list.Count;
                        bkgndTransLitJob.ReportProgress(percent);
                    }
                    elmt = (XmlElement)node;
                    if (elmt.GetAttribute("bname") != null)
                        elmt.SetAttribute("bname", Unidecoder.Unidecode(elmt.GetAttribute("bname")));
                }                
            }
            else
            {
                biblefile = BibleFormat.getInstance().FileName;
                BibleFormat.BibleFactory(biblefile);
                if (BibleFormat.getInstance() != null)
                {
                    BibleFormat.getInstance().Load();
                    //RTL.Checked = false;
                    data = BibleFormat.getInstance().populateBooks(MirrorTreeView1);
                }                
            }
        }

        private void bkgndTransLitJob_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            closeProgressStatus();           
            MirrorTreeView1.Nodes.Clear();
            data = BibleFormat.getInstance().populateBooks(MirrorTreeView1);
            ChapterArea.NavigateToString(BibleFormat.getInstance().getChapterHtml(data, paraBtn.Checked, CommentaryFormat.getInstance()));
        }

        private void bkgndTransLitJob_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            setProgressStatus(e.ProgressPercentage, e.ProgressPercentage.ToString()+" %");
        }

        private void statBtn_Click(object sender, EventArgs e)
        {
            if (BibleFormat.getInstance() == null)
            {
                ChapterArea.NavigateToString("<html dir=\'ltr\'><head>" + Themes.CSS_MAIN + "</head><body>No Bibles loaded yet.</body></html>");
                return;
            }
            StringBuilder table = new StringBuilder();
            XmlDocument doc=BibleFormat.getInstance().getXmlDoc();
            table.Append("<h3>Bible Statistics for ["+Path.GetFileName(BibleFormat.getInstance().FileName)+"]</h3>");
            table.Append("<table>");
            table.Append("<tr><td>Books:</td><td>" + doc.SelectNodes("/XMLBIBLE/BIBLEBOOK").Count + "</td></tr>");
            table.Append("<tr><td>Chapters:</td><td>" + doc.SelectNodes("/XMLBIBLE/BIBLEBOOK/CHAPTER").Count + "</td></tr>");
            XmlNodeList vers = doc.SelectNodes("/XMLBIBLE/BIBLEBOOK/CHAPTER/VERS");            
            table.Append("<tr><td>Verses:</td><td>" + vers.Count + "</td></tr>");          
            int wcount = 0;
            
            int small_verse_len = int.MaxValue;
            List<string> small_verse_ref = new List<string>();
            
            int long_verse_len = 0;
            List<string> long_verse_ref = new List<string>();

            int small_word_len = int.MaxValue;
            List<string> small_word = new List<string>();
            List<string> small_word_ref = new List<string>();

            int long_word_len = 0;
            List<string> long_word = new List<string>();
            List<string> long_word_ref = new List<string>();

            string[] words=null;            
            //List<string> wlist = new List<string>();
            string verse_text = null;
            foreach (XmlNode n in vers)
            {
                verse_text=n.InnerText.Trim();

                words = verse_text
                        .Replace(",", " ")
                        .Replace(";", " ")
                        .Replace(":", " ")
                        .Replace(".", " ")
                        .Replace("?", " ")
                        .Replace("\"", " ")
                        .Replace("'", " ")
                        .Replace("(", " ")
                        .Replace(")", " ")
                        .Replace("[", " ")
                        .Replace("]", " ")
                        .Replace("!", " ")
                        .Split(" ".ToCharArray());

                //words = verse_text.Split(" ".ToCharArray());
                wcount += words.Length;
                
                foreach (string word in words)
                {
                    //if (!wlist.Contains(word))
                    //    wlist.Add(word);
                    if (word.Length <= small_word_len && word.Length != 0 && !small_word.Contains(word))
                    {
                        if (word.Length < small_word_len)
                        {
                            small_word_len = word.Length;
                            small_word.Clear();
                            small_word_ref.Clear();
                        }
                        small_word.Add(word);
                        small_word_ref.Add(getVerseRef(n));
                    }

                    if (word.Length >= long_word_len && word.Length != 0 && !long_word.Contains(word))
                    {
                        if (word.Length > long_word_len)
                        {
                            long_word_len = word.Length;
                            long_word.Clear();
                            long_word_ref.Clear();
                        }
                        long_word.Add(word);
                        long_word_ref.Add(getVerseRef(n));
                    }
                }
                if (verse_text.Length <= small_verse_len && verse_text.Length != 0)
                {
                    if (verse_text.Length < small_verse_len)
                    {
                        small_verse_len = verse_text.Length;
                        small_verse_ref.Clear();                        
                    }
                    small_verse_ref.Add(getVerseRef(n));
                }
                if (verse_text.Length >= long_verse_len && verse_text.Length != 0)
                {
                    if (verse_text.Length > long_verse_len)
                    {
                        long_verse_len = verse_text.Length;
                        long_verse_ref.Clear();                        
                    }
                    long_verse_ref.Add(getVerseRef(n));
                }
            }
            table.Append("<tr><td>Total Words:</td><td>" + wcount + "</td></tr>");
            //table.Append("<tr><td>Unique Words:</td><td>" + wlist.Count.ToString() + "</td></tr>");
            //wlist.Clear();
            table.Append("<tr><td colspan='2'><hr></tr>");
            //shortest verse
            if(small_verse_ref.Count==1)
                table.Append("<tr><td>Shortest Verse:</td><td>");
            else
                table.Append("<tr><td>Shortest Verses:</td><td>");

            table.Append(" <i>(" + small_verse_len + ")</i><br><br>");
            foreach (string vref in small_verse_ref)
            {
                table.Append(vref);
            }
            table.Append("</td></tr>");
            table.Append("<tr><td colspan='2'><hr></tr>");
            //longest verse
            if (long_verse_ref.Count == 1)
                table.Append("<tr><td>Longest Verse:</td><td>");
            else
                table.Append("<tr><td>Longest Verses:</td><td>");

            table.Append(" <i>(" + long_verse_len + ")</i><br><br>");
            foreach (string vref in long_verse_ref)
            {
                table.Append(vref);
            }
            table.Append("</td></tr>");
            table.Append("<tr><td colspan='2'><hr></tr>");
            //
            //shortest word
            if(small_word_ref.Count==1)
                table.Append("<tr><td>Shortest Word:</td><td>");
            else
                table.Append("<tr><td>Shortest Words:</td><td>");

            table.Append(" <i>(" + small_word_len + ")</i><br>");
            for (int i = 0; i < small_word_ref.Count; i++)
            {
                table.Append(small_word[i]);
                table.Append(small_word_ref[i]);
            }
            table.Append("</td></tr>");
            table.Append("<tr><td colspan='2'><hr></tr>");
            //longest word
            if (long_word_ref.Count == 1)
                table.Append("<tr><td>Longest Word:</td><td>");
            else
                table.Append("<tr><td>Longest Words:</td><td>");

            table.Append(" <i>(" + long_word_len + ")</i><br>");
            for (int i = 0; i < long_word_ref.Count; i++)
            {
                table.Append(long_word[i]);
                table.Append(long_word_ref[i]);
            }
            table.Append("</td></tr>");
            //
            table.Append("</table>");

            ChapterArea.NavigateToString("<html dir=\'ltr\'><head>" + Themes.CSS_MAIN + "</head><body>" + table.ToString() + "</body></html>");
        }

        private string getVerseRef(XmlNode node)
        {
            string str = "<BLOCKQUOTE>"+node.InnerText+" ";
            int bidx = -1;
            if (node.ParentNode.ParentNode.Attributes["bname"] != null)
                str +=  " <sub>("+node.ParentNode.ParentNode.Attributes["bname"].Value + " " + node.ParentNode.Attributes["cnumber"].Value + ":" + node.Attributes["vnumber"].Value+")</sub>";
            else
            {
                bidx = int.Parse(node.ParentNode.ParentNode.Attributes["bnumber"].Value);
                str += " <sub>("+FormatUtil.shortBookNames[bidx-1] + " " + node.ParentNode.Attributes["cnumber"].Value + ":" + node.Attributes["vnumber"].Value+")</sub>";
            }
            str = str + "</BLOCKQUOTE>";
            return str;
        }

        private void openBookToolStripMenuItem_Click(object sender, EventArgs e)
        {
            readerOpenDialog.Filter = BookFormat.getFilters();
            int result = (int)(readerOpenDialog.ShowDialog());
            if (result == System.Convert.ToInt32(System.Windows.Forms.DialogResult.OK))
            {
                bookfile = readerOpenDialog.FileName;
                OpenBook();
            }
        }

        public void OpenBook()
        {
            is_bible = false;
            if (backgroundCleaner.IsBusy)
                backgroundCleaner.CancelAsync();
            if (exporting_book)
            {
                ChapterArea.NavigateToString("<html><head>" + Themes.CSS_UI + "</head><body><h3>Opening Book during export is disabled</h3><p>You can read the Book during export, but opening another is not possible. If you wish to cancel the export, just close and re-open this software or open another instance of the same software.</p></body></html>");
                return;
            }
            if (BackgroundWorker1.IsBusy)
            {
                ChapterArea.NavigateToString("<html><head>" + Themes.CSS_UI + "</head><body><h3>Book is already being opened</h3><p>Please be patient as another Bible is being opened. Depending on the format, it may take a while.</p></body></html>");
                return;
            }
            pictureBoxFront.Visible = false;
            Timer1.Enabled = true;
            StatusStrip1.Visible = true;
            MirrorTreeView1.Nodes.Clear();
            ChapterArea.NavigateToString("");
            exportBibleToolStripMenuItem.Enabled = false;
            exportBookToolStripMenuItem.Enabled = false;
            findToolStripMenuItem.Enabled = false;
            paraBtn.Enabled = true;
            findBtn.Enabled = false;
            fontBtn.Enabled = false;
            statBtn.Enabled = false;
            saveToolStripMenuItem.Enabled = false;            
            exporting_bible = false;
            commented = false;
            bibleTitleLbl.Visible = false;
            BackgroundWorker1.RunWorkerAsync();
        }

        private void exportBookToolStripMenuItem_Click(object sender, EventArgs e)
        {
            readerSaveDialog.Filter = BookFormat.getFilters();
            int result = (int)(readerSaveDialog.ShowDialog());
            if (result == System.Convert.ToInt32(System.Windows.Forms.DialogResult.OK))
            {
                export_filename = readerSaveDialog.FileName.Trim();
                export_filter_idx = readerSaveDialog.FilterIndex;
                Timer1.Enabled = true;
                StatusStrip1.Visible = true;
                ProgressBar.Value = 0;
                StatusText.Text = "0%";
                exporting_book = true;
                BackgroundWorker1.RunWorkerAsync();
            }
        }

        private void batchConvertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (batchDialog == null)
            {
                batchDialog = new BatchModeFrm();
            }
            batchDialog.ShowDialog(this);            
        }

        private void pluginsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pluginsToolStripMenuItem.Checked = GlobalMemory.getInstance().EnablePlugins;

            if (pluginsFrm == null)
            {
                pluginsFrm = new Plugins2Frm();
            }
            pluginsFrm.ShowDialog(this);            
            runPluginNow();
        }

        private void RestartApplication()
        {
            formatsSupportedHtml = null;
            bool bWasMaximized = (this.WindowState == FormWindowState.Maximized);
            this.WindowState = FormWindowState.Normal;
            // clear controls
            Controls.Clear();

            // Change current language
            if (GlobalMemory.getInstance().Locale == "en")
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;
            }
            else
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new
                System.Globalization.CultureInfo(GlobalMemory.getInstance().Locale);
            }

            // Populate components
            InitializeComponent();

            // Initialize UI dynamically calling the Load event handler of the Form
            // In this case the Load event handler of the form is MyForm_Load
            SimpleBibleReader_Load(null, null);

            this.WindowState = FormWindowState.Normal;
            this.WindowState = FormWindowState.Maximized;
            if (!bWasMaximized)
                this.WindowState = FormWindowState.Normal;
            this.Refresh();
            this.CenterToScreen();
        }

        private void englishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GlobalMemory.getInstance().Locale = "en";
            RestartApplication();
        }

        private void tamilToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GlobalMemory.getInstance().Locale = "ta";
            RestartApplication();
        }

        private void spanishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GlobalMemory.getInstance().Locale = "es";
            RestartApplication();
        }

        private void creditsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[][] attributions = SbrInfo.ATTRIBUTIONS;
            string[][] localizations = SbrInfo.LOCALIZATIONS;

            StringBuilder sb = new StringBuilder();
            sb.Append("<html><head>");
            sb.Append(Themes.CSS_UI);
            sb.Append("</head><body><h2>");
            sb.Append(Localization.MainPage_Attributions);
            sb.Append("</h2>");

            // Attributions - Localisations
            sb.Append("<h3>Localizations</h3>");
            sb.Append("<table width='95%'>");
            for (int i = 0; i < localizations.Length; i++)
            {
                sb.Append("<tr><td>" + localizations[i][0] + "</td><td>");
                if (localizations[i][1] == "")
                    sb.Append(localizations[i][2]);
                else
                    sb.Append("<a href='" + localizations[i][1] + "'>" + localizations[i][2] + "</a>");
                sb.Append("</td></tr>");
            }
            sb.Append("</table>");

            sb.Append("<hr/>");
            // Attributions - UI
            sb.Append("<h3>User Interface</h3>");
            sb.Append("<table width='95%'>");
            for (int i = 0; i < attributions.Length; i++)
            {
                sb.Append("<tr><td>" + attributions[i][0] + "</td><td>");
                if(attributions[i][1]=="")
                    sb.Append(attributions[i][2]);
                else
                    sb.Append("<a href='" + attributions[i][1] + "'>" + attributions[i][2] + "</a>");
                sb.Append("</td></tr>");
            }
            sb.Append("</table>");

            //sb.Append("<hr/>");

            sb.Append("</body></html>");
            ChapterArea.NavigateToString(sb.ToString());
        }

        private void ContextMenuItem_speakVerse()
        {
            if (!speechBkgndWorker.IsBusy)
                speechBkgndWorker.RunWorkerAsync(selected_verse.Substring(0, selected_verse.LastIndexOf('(') - 1).Replace("[", "").Replace("]", ""));
        }

        private void btnBible_Click(object sender, EventArgs e)
        {

            pictureBoxFront.Visible = false;
            exportBibleToolStripMenuItem.Enabled = false;
            exportBookToolStripMenuItem.Enabled = true;
            findToolStripMenuItem.Enabled = true;
            paraBtn.Enabled = false;
            findBtn.Enabled = false;
            fontBtn.Enabled = false;
            statBtn.Enabled = false;
            saveToolStripMenuItem.Enabled = false;
            exporting_bible = false;
            commented = false;
            bibleTitleLbl.Visible = true;



            is_bible = false;
            BookFormat.book_format = new XMLBookFormat("");
            BookFormat.getInstance().getXmlDoc().LoadXml(Properties.Resources.gospel_of_john);
            BookFormat.getInstance().ABBREVIATION = "The Gospel According to John";
            BookFormat.getInstance().DESCRIPTION = "The Gospel According to John";
            BookFormat.getInstance().COMMENTS = "The Gospel According to John from the World English Bible";
            RTL.Checked = false;
            if (MirrorTreeView1.Nodes != null)
                MirrorTreeView1.Nodes.Clear();
            BookFormat._internal_maincss = true;
            data = BookFormat.getInstance().populateBooks(MirrorTreeView1);
            MirrorTreeView1.ExpandAll();
            ChapterArea.NavigateToString(BookFormat.getInstance().getChapterHtml(data));

            TreeNode tn = MirrorTreeView1.Nodes[0].Nodes[0];
            MirrorTreeView1.SelectedNode = tn;
            MirrorTreeView1.Focus();
            MirrorTreeView1.Nodes[0].EnsureVisible();

            this.bibleTitleLbl.Text = "The Gospel According to John";
        }

        private void portuguesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GlobalMemory.getInstance().Locale = "pt";
            RestartApplication();
        }

        private void btnPlugin_Click(object sender, EventArgs e)
        {
            pluginsToolStripMenuItem.Checked = GlobalMemory.getInstance().EnablePlugins;
            if (pluginsFrm == null)
                pluginsFrm = new Plugins2Frm();            
            pluginsFrm.ShowDialog(this);
            runPluginNow();                        
        }

        private void runPluginNow()
        {
            if (GlobalMemory.getInstance().EnablePlugins)
            {
                if (BibleFormat.getInstance() != null)
                    if (Themes.MessageBox("Do you want to run the enabled plugins on the loaded bible now?", "Plugins Execution", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        PluginProcessFrm frm = new PluginProcessFrm(BibleFormat.BibleXmlDocument, PluginProcessFrm.TYPE_BIBL);
                        frm.ShowDialog(this);

                        TreeNode tn = MirrorTreeView1.Nodes[0].FirstNode.FirstNode;
                        Themes.MessageBox("Plugin(s) successfully executed on the bible.");
                        MirrorTreeView1.SelectedNode = tn;
                        MirrorTreeView1.Focus();
                        MirrorTreeView1.SelectedNode.EnsureVisible();
                    }
                //
                if (BookFormat.getInstance() != null)
                    if (Themes.MessageBox("Do you want to run the enabled plugins on the loaded book now?", "Plugins Execution", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        PluginProcessFrm frm = new PluginProcessFrm(BookFormat.BookXmlDocument, PluginProcessFrm.TYPE_BOOK);
                        frm.ShowDialog(this);

                        try
                        {
                            TreeNode tn = MirrorTreeView1.Nodes[0];
                            while (tn.Nodes.Count > 0)
                                tn = tn.FirstNode;
                            MirrorTreeView1.SelectedNode = tn;
                        }
                        catch (Exception) { }

                        Themes.MessageBox("Plugin(s) successfully executed on the book.");

                        MirrorTreeView1.Focus();
                        MirrorTreeView1.SelectedNode.EnsureVisible();
                    }
                //
                if (CommentaryFormat.getInstance() != null)
                    if (Themes.MessageBox("Do you want to run the enabled plugins on the loaded commentary now?", "Plugins Execution", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        PluginProcessFrm frm = new PluginProcessFrm(CommentaryFormat.CommentaryXmlDocument, PluginProcessFrm.TYPE_CMTY);
                        frm.ShowDialog(this);

                        try
                        {
                            TreeNode tn = MirrorTreeView1.Nodes[0];
                            while (tn.Nodes.Count > 0)
                                tn = tn.FirstNode;
                            MirrorTreeView1.SelectedNode = tn;
                        }
                        catch (Exception) { }

                        Themes.MessageBox("Plugin(s) successfully executed on the commentary.");

                        MirrorTreeView1.Focus();
                        if (MirrorTreeView1.SelectedNode != null)
                            MirrorTreeView1.SelectedNode.EnsureVisible();
                    }
                //
                if (DictionaryFormat.getInstance() != null)
                    if (Themes.MessageBox("Do you want to run the enabled plugins on the loaded dictionary now?", "Plugins Execution", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        PluginProcessFrm frm = new PluginProcessFrm(DictionaryFormat.DictionaryXmlDocument, PluginProcessFrm.TYPE_DICT);
                        frm.ShowDialog(this);

                        try
                        {
                            TreeNode tn = MirrorTreeView1.Nodes[0];
                            while (tn.Nodes.Count > 0)
                                tn = tn.FirstNode;
                            MirrorTreeView1.SelectedNode = tn;
                        }
                        catch (Exception) { }

                        Themes.MessageBox("Plugin(s) successfully executed on the dictionary.");

                        MirrorTreeView1.Focus();
                        if(MirrorTreeView1.SelectedNode!=null)
                            MirrorTreeView1.SelectedNode.EnsureVisible();
                        
                    }
            }
        }

        private void ContextMenuItem_speakChapter()
        {
            StringBuilder text = new StringBuilder();
            foreach (XmlNode ver in selected_chapter_elmt.SelectNodes("VERS"))
            {
                text.Append(ver.InnerText);
                text.Append("\r\n");
            }

            Thread.Sleep(1000);
            if (!speechBkgndWorker.IsBusy)
                speechBkgndWorker.RunWorkerAsync(text.ToString().Replace("[","").Replace("]",""));
        }

        private void speechBkgndWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            SpeechUtil.speak((string)e.Argument);
        }

        
    }
}
