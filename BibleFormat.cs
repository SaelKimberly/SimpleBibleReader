using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;

using System;
using System.Collections;
using System.Windows.Forms;
using System.Xml;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Simple_Bible_Reader
{
	public abstract class BibleFormat
	{
        //public const int F01_EXECUTABLE_BIBLE = 1; // not supported
        public const int F02_ZEFANIA_BIBLE = 1;
        public const int F03_UNBOUND_BIBLE_BCV = 2;
        public const int F04_UNBOUND_BIBLE_BCVS = 3;
        public const int F05_OSIS = 4;
        public const int F06_GBF = 5;
        public const int F07_XSEM = 6;
        public const int F08_THEWORD = 7;
        public const int F09_EWORD_BBL = 8;
        public const int F10_ESWORD_BBLX = 9;
        public const int F11_ESWORD_BBLI = 10;
        public const int F12_SWORDPROJECT = 11;
        public const int F13_VPL = 12;
        public const int F14_BIBLE_COMPANION = 13;
        public const int F15_GOBIBLE = 14;
        public const int F16_THML = 15;
        public const int F17_OPENSONG = 16;
        public const int F18_OPENLP = 17;
        public const int F19_MYSWORD = 18;
        public const int F20_BIBLE_ANALYZER = 19;
        public const int F21_EASY_SLIDES = 20;
        public const int F22_MP3 = 21;
        public const int F23_LOGOS = 22;
        public const int F24_ONLINEBIBLE = 23;
        public const int F25_PALMBIBLE = 24;
        public const int F26_HEAVENWORLD = 25;
        public const int F27_VERSEVIEW = 26;
        public const int F28_MYBIBLE = 27;        
        public const int F29_USFX = 28;
        public const int F30_SWORDSEARCHER = 29;

        ///


        static string filters = null;
        public string ABBREVIATION = "BIB";
        public string DESCRIPTION = "Bible";
        public string COMMENTS = "Exported from Simple Bible Reader ("+ GlobalMemory.AUTHOR_WEBSITE + ")";
		private Dictionary<string,int[]> chapterCount = new Dictionary<string, int[]>();
		private string m_filename;
		
		private static System.Xml.XmlDocument xmldoc = new System.Xml.XmlDocument();
		
		private bool process_complete = false;
		
		public delegate int  Add(TreeNode i);
		public delegate void Expand();        

		public static BibleFormat bible_format=null;
        public bool CLEANING = false;

        public string rtf_header = @"{\rtf1\ansi\ansicpg1252\deff0\deflang1033{\fonttbl{\f1\fnil\fprq2\fcharset161 TITUS cyberbit Basic;}{\f2\fnil\fprq2\fcharset177 TITUS Cyberbit Basic;}{\f3\fnil\fprq2\fcharset0 TITUS Cyberbit Basic;}{\f4\fnil\fprq2\fcharset136 PMingLiU;}{\f5\fnil\fprq2\fcharset204 Georgia;}{\f6\fnil\fprq2\fcharset238 Georgia;}{\f7\fnil\fprq2\fcharset134 PMingLiU;}{\f8\fnil\fprq2\fcharset222 Cordia New;}{\f9\fnil\fprq2\fcharset129 Gulim;}{\f10\fnil\fprq2\fcharset178 TITUS Cyberbit Basic;}{\f11\fnil\fprq2\fcharset162 Georgia;}{\f12\fnil\fprq2\fcharset163 Times New Roman;}{\f13\fnil\fprq2\fcharset128 MS Mincho;}}{\colortbl ;\red0\green0\blue0;\red0\green0\blue255;\red0\green255\blue255;\red0\green255\blue0;\red255\green0\blue255;\red255\green0\blue0;\red255\green255\blue0;\red255\green255\blue255;\red0\green0\blue128;\red0\green128\blue128;\red0\green128\blue0;\red128\green0\blue128;\red128\green0\blue0;\red128\green128\blue0;\red128\green128\blue128;\red192\green192\blue192;}{\viewkind4\uc1\pard\plain\cf0\fs\par}";
        public System.Windows.Forms.RichTextBox richTextCtrl = new System.Windows.Forms.RichTextBox();

        public static bool isApocrypha = false;
        public static List<string[]> kv_export = new List<string[]>();


        public BibleFormat(string filename)
		{
			m_filename = filename;
			process_complete = false;
		}

        public static BibleFormat getInstance()
        {
            return bible_format;
        }

        public static BibleFormat BibleFactory(string filename)
        {
            bible_format = null;
            if (filename.ToLower().EndsWith(".xml"))
            {                
                string xml_str = System.IO.File.ReadAllText(filename).Trim().ToLower();
                if (FormatUtil.InStr(xml_str, "<xmlbible"))
                {
                    xml_str = null;
                    bible_format = new ZefaniaBibleFormat(filename);
                }
                else if (FormatUtil.InStr(xml_str, "<osis"))
                {
                    xml_str = null;
                    bible_format = new OSISBibleFormat(filename);
                }                
                else if (FormatUtil.InStr(xml_str, "<usfx"))
                {
                    xml_str = null;
                    bible_format = new USFXBibleFormat(filename);
                }
                else if (FormatUtil.InStr(xml_str, "<thml"))
                {
                    xml_str = null;
                    bible_format = new ThMLBibleFormat(filename);
                }
                else if (FormatUtil.InStr(xml_str, "<scripture"))
                {
                    xml_str = null;
                    bible_format = new XSEMBibleFormat(filename);
                }
                else if (FormatUtil.InStr(xml_str, "<bible"))
                {
                    if (FormatUtil.InStr(xml_str, "<fname"))
                    {
                        xml_str = null;
                        bible_format = new VerseVIEWBibleFormat(filename);
                    }
                    else
                    {
                        xml_str = null;
                        bible_format = new OpenSongBibleFormat(filename);
                    }
                }  
            }
            else if (filename.ToLower().EndsWith(".thm") || filename.ToLower().EndsWith(".thml"))
            {
                bible_format = new ThMLBibleFormat(filename);
            }
            else if (filename.ToLower().EndsWith(".txt"))
            {
                StreamReader sr = File.OpenText(filename);
                string line = sr.ReadLine();
                sr.Close();
                if (line.StartsWith(";"))                
                    bible_format = new SwordSearcherBibleFormat(filename);
                else                
                    bible_format = new UnboundBibleFormat(filename,F03_UNBOUND_BIBLE_BCV); //default is BCV for drag drop or autodetect
            }
            else if (filename.ToLower().EndsWith(".gbf"))
            {
                bible_format = new GBFBibleFormat(filename);
            }
            else if (filename.ToLower().EndsWith(".ont"))
            {
                bible_format = new TheWordBibleFormat(filename, TheWordBibleFormat.Type_ONT);
            }
            else if (filename.ToLower().EndsWith(".ot"))
            {
                bible_format = new TheWordBibleFormat(filename, TheWordBibleFormat.Type_OT);
            }
            else if (filename.ToLower().EndsWith(".nt"))
            {
                bible_format = new TheWordBibleFormat(filename, TheWordBibleFormat.Type_NT);
            }
            else if (filename.ToLower().EndsWith(".ontx"))
            {
                bible_format = new TheWordBibleFormat(filename, TheWordBibleFormat.Type_ONTX);
            }
            else if (filename.ToLower().EndsWith(".otx"))
            {
                bible_format = new TheWordBibleFormat(filename, TheWordBibleFormat.Type_OTX);
            }
            else if (filename.ToLower().EndsWith(".ntx"))
            {
                bible_format = new TheWordBibleFormat(filename, TheWordBibleFormat.Type_NTX);
            }
            else if (filename.ToLower().EndsWith(".bbl"))
            {
                bible_format = new BBLBibleFormat(filename);
            }
            else if (filename.ToLower().EndsWith(".bblx"))
            {
                bible_format = new BBLXBibleFormat(filename);
            }
            else if (filename.ToLower().EndsWith(".bbli"))
            {
                bible_format = new BBLIBibleFormat(filename);
            }
            else if (filename.ToLower().EndsWith(".zip"))
            {
                bible_format = new SwordBibleFormat(filename);
            }
            else if (filename.ToLower().EndsWith(".vpl"))
            {
                bible_format = new VPLBibleFormat(filename);
            }
            else if (filename.ToLower().EndsWith(".jar"))
            {
                bible_format = new GoBibleFormat(filename);
            }
            else if (filename.ToLower().EndsWith(".xmm"))
            {
                bible_format = new OpenSongBibleFormat(filename);
            }
            else if (filename.ToLower().EndsWith(".sqlite"))
            {
                bible_format = new OpenLPBibleFormat(filename);
            }
            else if (filename.ToLower().EndsWith(".mdb"))
            {
                bible_format = new EasySlidesBibleFormat(filename);
            }
            else if (filename.ToLower().EndsWith(".hbb"))
            {
                bible_format = new HeavenWorldBibleFormat(filename);
            }
            else if (filename.ToLower().EndsWith(".bib"))
            {
                // same extension for Bible Companion and Bible Analyzer
                FileStream fs = File.OpenRead(filename);
                try
                {
                    int count = "SQLite".Length;
                    byte[] bytes = new byte[count];
                    fs.Read(bytes, 0, count);
                    fs.Close();
                    if (Encoding.ASCII.GetString(bytes).Equals("SQLite"))
                        bible_format = new BibleAnalyzerBibleFormat(filename);
                    else
                        bible_format = new BibleCompanionBibleFormat(filename);
                }
                catch (Exception)
                {
                    fs.Close();
                }
            }            
            else if (filename.ToLower().EndsWith(".bbl.mybible.gz"))
            {
                bible_format = new MySword4AndroidBibleFormat(filename,true);
            }
            else if (filename.ToLower().EndsWith(".bbl.mybible"))
            {
                bible_format = new MySword4AndroidBibleFormat(filename, false);
            }
            else if (filename.ToLower().EndsWith(".mp3"))
            {
                // required, all though not used
                bible_format = new MP3BibleFormat(filename);
            }
            else if (filename.ToLower().EndsWith(".docx"))
            {
                // required, all though not used
                bible_format = new LOGOSBibleFormat(filename);
            }
            else if (filename.ToLower().EndsWith(".exp"))
            {
                bible_format = new OnlineBibleFormat(filename);
            }
            else if (filename.ToLower().EndsWith(".sqlite3") && !filename.ToLower().EndsWith(".commentaries.sqlite3") && !filename.ToLower().EndsWith(".dictionary.sqlite3"))
            {
                bible_format = new MyBibleBibleFormat(filename);
            }
            else if (filename.ToLower().EndsWith(".pdb"))
            {
                bible_format = new PDBBibleFormat(filename);
            }
            else if (filename.ToLower().EndsWith(".ss8bible") || filename.ToLower().EndsWith(".ss7bible") || filename.ToLower().EndsWith(".ss6bible") || filename.ToLower().EndsWith(".ss5bible"))
            {
                bible_format = new SwordSearcherBibleFormat(filename);
            }
            else if (System.IO.Directory.Exists(filename))
            {
                //Its a directory
                if (System.IO.Directory.Exists(filename + "\\mods.d"))
                {
                    bible_format = new SwordBibleFormat(filename);
                }
            }            

            if (bible_format != null)
            {
                if (cleaningRequired(bible_format.FilterIndex))
                {
                    bible_format.CLEANING = true;
                }

                if (bible_format.ABBREVIATION == "BIB")
                    bible_format.ABBREVIATION = Path.GetFileNameWithoutExtension(filename);
                if (bible_format.DESCRIPTION == "Bible")
                    bible_format.DESCRIPTION = Path.GetFileNameWithoutExtension(filename);                
            }

            //            
            return bible_format;
        }

        public static BibleFormat BibleFactory(string filename, int filterIdx)
        {
            if (filterIdx == 0)
                bible_format = BibleFactory(filename);
            else
                bible_format = getBibleFormat(filename, filterIdx);
            return bible_format;
        }

        public static BibleFormat getBibleFormat(string filename, int filterIdx)
        {
            BibleFormat bible_format = null;
            switch (filterIdx)
            {
                case F02_ZEFANIA_BIBLE:
                    bible_format = new ZefaniaBibleFormat(filename);
                    break;
                case F03_UNBOUND_BIBLE_BCV:
                    bible_format = new UnboundBibleFormat(filename, F03_UNBOUND_BIBLE_BCV);
                    break;
                case F04_UNBOUND_BIBLE_BCVS:
                    bible_format = new UnboundBibleFormat(filename, F04_UNBOUND_BIBLE_BCVS);
                    break;
                case F05_OSIS:
                    bible_format = new OSISBibleFormat(filename);
                    break;
                case F06_GBF:
                    bible_format = new GBFBibleFormat(filename);
                    break;
                case F07_XSEM:
                    bible_format = new XSEMBibleFormat(filename);
                    break;
                case F08_THEWORD:
                    bible_format = new TheWordBibleFormat(filename, TheWordBibleFormat.Type_ONT);
                    break;
                case F09_EWORD_BBL:
                    bible_format = new BBLBibleFormat(filename);
                    break;
                case F10_ESWORD_BBLX:
                    bible_format = new BBLXBibleFormat(filename);
                    break;
                case F11_ESWORD_BBLI:
                    bible_format = new BBLIBibleFormat(filename);
                    break;
                case F12_SWORDPROJECT:
                    bible_format = new SwordBibleFormat(filename);
                    break;
                case F13_VPL:
                    bible_format = new VPLBibleFormat(filename);
                    break;
                case F14_BIBLE_COMPANION:
                    bible_format = new BibleCompanionBibleFormat(filename);
                    break;
                case F15_GOBIBLE:
                    bible_format = new GoBibleFormat(filename);
                    break;
                case F16_THML:
                    bible_format = new ThMLBibleFormat(filename);
                    break;
                case F17_OPENSONG:
                    bible_format = new OpenSongBibleFormat(filename);
                    break;
                case F18_OPENLP:
                    bible_format = new OpenLPBibleFormat(filename);
                    break;
                case F19_MYSWORD:
                    bible_format = new MySword4AndroidBibleFormat(filename, true);
                    break;
                case F20_BIBLE_ANALYZER:
                    bible_format = new BibleAnalyzerBibleFormat(filename);
                    break;
                case F21_EASY_SLIDES:
                    bible_format = new EasySlidesBibleFormat(filename);
                    break;
                case F22_MP3:
                    bible_format = new MP3BibleFormat(filename);
                    break;
                case F23_LOGOS:
                    bible_format = new LOGOSBibleFormat(filename);
                    break;
                case F24_ONLINEBIBLE:
                    bible_format = new OnlineBibleFormat(filename);
                    break;
                case F25_PALMBIBLE:
                    bible_format = new PDBBibleFormat(filename);
                    break;
                case F26_HEAVENWORLD:
                    bible_format = new HeavenWorldBibleFormat(filename);
                    break;
                case F27_VERSEVIEW:
                    bible_format = new VerseVIEWBibleFormat(filename);
                    break;
                case F28_MYBIBLE:
                    bible_format = new MyBibleBibleFormat(filename);
                    break;
                case F29_USFX:
                    bible_format = new USFXBibleFormat(filename);
                    break;
                case F30_SWORDSEARCHER:
                    bible_format = new SwordSearcherBibleFormat(filename);
                    break;

                default:
                    if (!GlobalMemory.getInstance().ConsoleMode)
                        Themes.MessageBox("Sorry, export for this Bible format is not yet implemented!");
                    else
                        Console.WriteLine("Sorry, export for this Bible format is not yet implemented!");
                    return null;
            }
            return bible_format;
        }

        public static BibleFormat doExportBible(string filename,int filterIdx)
        {
            bool cleaing_required = bible_format.CLEANING;
            BibleFormat bible_format_export = getBibleFormat(filename,filterIdx);
            // if the loaded bible requires cleaning, the the target much clean it before export.
            if (bible_format_export != null)
            {
                bible_format_export.CLEANING = bible_format.CLEANING;
                bible_format_export.ABBREVIATION = bible_format.ABBREVIATION.Replace("'", "''");
                bible_format_export.DESCRIPTION = bible_format.DESCRIPTION.Replace("'", "''");
                bible_format_export.COMMENTS = bible_format.COMMENTS.Replace("'", "''");
                if (bible_format_export.ABBREVIATION == "BIB")
                    bible_format_export.ABBREVIATION = Path.GetFileNameWithoutExtension(filename).Replace("'", "''");
                if (bible_format_export.DESCRIPTION == "Bible")
                    bible_format_export.DESCRIPTION = Path.GetFileNameWithoutExtension(filename).Replace("'", "''");
            }
            return bible_format_export;
        }

        public static bool openNotSupported(int FilterIdx)
        {
            int[] notsup = new int[] { F22_MP3 , F23_LOGOS};
            for (int i = 0; i < notsup.Length; i++)
                if (notsup[i] == FilterIdx)
                    return true;
            return false;
        }

        public static bool cleaningRequired(int FilterIdx)
        {
            //int[] required = new int[] { 9, 10 ,19};
            int[] required = new int[] { F09_EWORD_BBL, F10_ESWORD_BBLX, F11_ESWORD_BBLI};
            for (int i = 0; i < required.Length; i++)
                if (required[i] == FilterIdx)
                    return true;            
            return false;
        }

		public void SetProcessAsComplete()
		{
			process_complete = true;
		}
		
		
		
		
		public static System.Xml.XmlDocument BibleXmlDocument
		{
			get
			{
				return xmldoc;
			}
			set
			{
				xmldoc = value;
			}
		}
		
		public string XML
		{
			get
			{
				return xmldoc.OuterXml;
			}
		}

        public static String PrintXML(String XML)
        {
            String Result = "";

            MemoryStream mStream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(mStream, Encoding.Unicode);
            XmlDocument document = new XmlDocument();

            try
            {
                // Load the XmlDocument with the XML.
                document.LoadXml(XML);

                writer.Formatting = Formatting.Indented;

                // Write the XML into a formatting XmlTextWriter
                document.WriteContentTo(writer);
                writer.Flush();
                mStream.Flush();

                // Have to rewind the MemoryStream in order to read
                // its contents.
                mStream.Position = 0;

                // Read MemoryStream contents into a StreamReader.
                StreamReader sReader = new StreamReader(mStream);

                // Extract the text from the StreamReader.
                String FormattedXML = sReader.ReadToEnd();

                Result = FormattedXML;
            }
            catch (XmlException ex)
            {
                
            }

            mStream.Close();
            writer.Close();

            return Result;
        }
		
		public string FileName
		{
			get
			{
				return m_filename;
			}
			set
			{
				m_filename = value;
			}
		}		
		
		public void populateBook(string book, TreeNode booknode)
		{
			if (book == null)
			{
				return;
			}
			int[] mChapters = chapterCount[book];
			string[] insideBook = new string[mChapters.Length];
			for (int ctr = 1; ctr <= mChapters.Length; ctr++)
			{
                insideBook[ctr - 1] = mChapters[ctr-1].ToString();
			}
			foreach (string lvi in insideBook)
			{
				booknode.Nodes.Add(book + "_" + lvi, lvi);
			}
		}
		
        public string getVerse(int book,int chapter,int verse)
        {
            XmlNode v = xmldoc.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber=\'" + book.ToString() + "\']/CHAPTER[@cnumber='" + chapter.ToString() + "']/VERS[@vnumber='"+verse.ToString()+"']");
            if (v != null)
                return v.InnerText;
            else
                return "";
        }

        public string getBookName(int book)
        {
            XmlNode v = xmldoc.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber=\'" + book.ToString() + "\']/@bname");
            if (v != null)
                return v.InnerText;
            else
                return Localization.getBookNames()[FormatUtil.zefaniaBookNoToIndex(book)];
        }

        public string getFirstVerse()
        {
            XmlNode v = xmldoc.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[1]/CHAPTER[1]/VERS[1]");
            if (v != null)
                return v.InnerText;
            else
                return "";
        }

        public virtual string getRawChapter(string[] data, bool para_mode, CommentaryFormat cmt)
        {
            string chapHtml = "<html dir=\'ltr\'><head>";

            chapHtml = chapHtml + Themes.CSS_MAIN + "</head><body>";
            XmlNodeList verses = xmldoc.SelectNodes("/XMLBIBLE/BIBLEBOOK[@bname=\'" + data[1] + "\']/CHAPTER[@cnumber=" + data[2] + "]/VERS");
            string verse_no = "1";
            foreach (XmlNode verse in verses)
            {
                verse_no = verse.Attributes["vnumber"].Value;
                chapHtml = chapHtml + "<i>"+ verse_no + "</i><br/><textarea readonly='readonly'>" + FormatUtil.UnescapeXML(FormatUtil.UnescapeXML(verse.InnerXml)) + "</textarea><br/>";
            }
            chapHtml = chapHtml + "</body></html>";
            return chapHtml;
        }

        public virtual string getChapterHtml(string[] data, bool para_mode,CommentaryFormat cmt)
		{
            if(GlobalMemory.getInstance().DisplayBIBRaw)
                return getRawChapter(data, para_mode, cmt);
            string chapHtml = "<html dir=\'ltr\'><head><title>" + DESCRIPTION + "</title>";
            
            chapHtml =chapHtml + Themes.CSS_MAIN + "</head><body>";
			
            string verse_text = "";
			XmlNodeList verses = xmldoc.SelectNodes("/XMLBIBLE/BIBLEBOOK[@bname=\'" + data[1] + "\']/CHAPTER[@cnumber=" + data[2] + "]/VERS");            
			string verse_no = "1";
			string book_no = xmldoc.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bname=\'" + data[1] + "\']").Attributes.GetNamedItem("bnumber").Value;
            int mchap=-1;
            int mbook = -1;
            int.TryParse(data[2], out mchap);
            int.TryParse(book_no, out mbook);
            chapHtml = chapHtml+"<h3>" + ChapterHeadingUtil.getChapterTitle(mbook, mchap) + "</h3>";

            if(!para_mode)
                chapHtml = chapHtml + "<ul>";
            XmlElement cmt_elmt = null;
            int book = -1;
            string prefix = "";

            richTextCtrl = new System.Windows.Forms.RichTextBox();
            lock (richTextCtrl)
            {
                foreach (XmlNode verse in verses)
                {
                    verse_no = verse.Attributes["vnumber"].Value;
                    if (cmt != null)
                        cmt_elmt = (XmlElement)getXmlDoc().SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber='" + book_no.ToString() + "']/CHAPTER[@cnumber='" + data[2] + "']/VERS[@vnumber='" + verse_no.ToString() + "']");
                    book = int.Parse(verse.ParentNode.ParentNode.Attributes["bnumber"].Value);
                    if (book >= 40)
                        prefix = "G";
                    else if (book < 40)
                        prefix = "H";
                    else
                        prefix = "";

                    if (CLEANING && GlobalMemory.getInstance().ParseBible)
                    {
                        richTextCtrl.Rtf = rtf_header + verse.InnerText;
                        verse_text = richTextCtrl.Text;
                    }
                    else
                    {
                        if (GlobalMemory.getInstance().ParseBible)
                        {
                            verse_text = verse.InnerXml;
                            // STYLE tag:
                            if (verse_text.ToLower().IndexOf("style") != -1)
                            {
                                verse_text = Regex.Replace(verse_text, "(<)(STYLE )([^<]*)(css=)([^<]*)(>)", "<span style=$5>", RegexOptions.IgnoreCase);
                                verse_text = Regex.Replace(verse_text, "</STYLE>", "</span>", RegexOptions.IgnoreCase);
                            }
                            // gr tag 
                            //<gr str="7225">beginning </gr>                    
                            if (verse_text.ToLower().IndexOf("gr") != -1)
                            {
                                if(GlobalMemory.getInstance().useRenderer=="WEBVIEW2")
                                    verse_text = Regex.Replace(verse_text, "(<gr )([^s]*)(str=['\"]*)([^<'\"]*)['\"]*(>)(((?!</gr).)*)</gr>", "$6<sup><span onclick=\"chrome.webview.postMessage('L" + prefix + "$4');\" class='lexicon'>" + prefix + "$4</span></sup>", RegexOptions.IgnoreCase);
                                else
                                    verse_text = Regex.Replace(verse_text, "(<gr )([^s]*)(str=['\"]*)([^<'\"]*)['\"]*(>)(((?!</gr).)*)</gr>", "$6<sup><span onclick='location.href=\"L" + prefix + "$4\"' class='lexicon'>" + prefix + "$4</span></sup>", RegexOptions.IgnoreCase);
                            }
                        }
                        else
                            verse_text = verse.InnerText;
                    }

                    // remove NOTE tag from text.
                    verse_text = Regex.Replace(verse_text,"\\<NOTE\\>[^\\<]+\\</NOTE\\>","");

                    string para_style = "linear";
                    if(para_mode)
                        para_style = "para";

                    if (GlobalMemory.getInstance().useRenderer == "WEBVIEW2")
                    {
                        verse_text = "<a class=\"" + para_style + "\" id=\'" + verse_no + "\' OnClick=\"chrome.webview.postMessage('" + book_no + "-" + data[2] + "-" + verse_no + "');return false;\" OnMouseDown=\"if(event.button == 2)chrome.webview.postMessage('" + book_no + "-" + data[2] + "-" + verse_no + "');\"><sup class='versenumber'>" + verse_no + "</sup>" + verse_text + "</a> ";
                        if (!para_mode)
                            verse_text = "<li>" + verse_text;
                    }
                    else
                    {
                        if (!para_mode)
                            verse_text = "<li><a class=\"linear\" id=\'" + verse_no + "\' href=\'" + book_no + "-" + data[2] + "-" + verse_no + "\'><sup><i><font color=\'grey\'>" + verse_no + "</font></i></sup>" + verse_text + "</a></li>";
                        else
                            verse_text = "<a class=\"para\" id=\'" + verse_no + "\' href=\'" + book_no + "-" + data[2] + "-" + verse_no + "\'><sup><i><font color=\'grey\'>" + verse_no + "</font></i></sup>" + verse_text + "</a> ";
                    }

                    if (cmt_elmt != null && cmt != null)
                    {
                        if (cmt.getCommentaryForVerse(int.Parse(book_no), int.Parse(data[2]), int.Parse(verse_no)) != "")
                        {
                            if (GlobalMemory.getInstance().CommentaryIcon)
                                verse_text = verse_text + " <img id='cmtimage' border='0'/>";
                        }
                    }

                    verse_text = verse_text + "</a>";

                    ///////////////////////////////////////////////
                    // AUTOMATICALLY CONVERT OLD COMMENT TO NEW NOTE
                    if (verse.NextSibling != null)
                        if (verse.NextSibling.Name == "COMMENT")
                        {
                            XmlNode note = getXmlDoc().CreateElement("NOTE");
                            note.InnerText = verse.NextSibling.InnerText;
                            verse.NextSibling.RemoveAll();
                            verse.AppendChild(note);
                            //verse_text = verse_text + "<BLOCKQUOTE style='color:grey' id='bgimage'>" + verse.NextSibling.InnerText + "</BLOCKQUOTE>";
                        }
                    //////////////////////////////////////////////


                    if (verse.HasChildNodes)
                    {
                        XmlNode note = verse.SelectSingleNode("NOTE");
                        if (note !=null)
                            verse_text = verse_text + "<BLOCKQUOTE>" + note.InnerText + "</BLOCKQUOTE>";
                    }


                    if (!para_mode)
                        verse_text = verse_text + "</li>";

                    chapHtml = chapHtml + verse_text;
                }
            }
            if(!para_mode)
			    chapHtml = chapHtml + "</ul>";
			chapHtml = chapHtml + "</body></html>";
			return chapHtml;
		}        

		public string[] populateBooks(MirrorTreeView treeview)
		{
			chapterCount = new Dictionary<string, int[]>();
			XmlNodeList biblebooks = BibleXmlDocument.SelectNodes("/XMLBIBLE/BIBLEBOOK");
			
			string[] otBibleBooks = new string[1];
			string[] ntBibleBooks = new string[1];
            string[] apBibleBooks = new string[1];
            isApocrypha = false;

            int book;
            int[] chapters = null;
			foreach (System.Xml.XmlNode book_node in biblebooks)
			{
				try
				{
					book = int.Parse(book_node.Attributes.GetNamedItem("bnumber").Value);                    
				}
				catch (Exception ex)
				{
                    continue;
				}

                if (book >= 1 && book <= 39)
                {
                    if (book_node.Attributes.GetNamedItem("bname") == null)
                    {
                        XmlAttribute attribute = BibleXmlDocument.CreateAttribute("bname");
                        attribute.Value = Localization.getBookNames()[FormatUtil.zefaniaBookNoToIndex(book)];
                        book_node.Attributes.Append(attribute);
                    }

                    if (otBibleBooks.Length < book + 1)
                        Array.Resize(ref otBibleBooks, book + 1);

                    otBibleBooks[book - 1] = book_node.Attributes.GetNamedItem("bname").Value;
                    if (!chapterCount.ContainsKey(book_node.Attributes.GetNamedItem("bname").Value))
                    {
                        XmlNodeList xmlchaps = book_node.SelectNodes("CHAPTER");
                        List<int> chaptersList = new List<int>();
                        foreach (XmlNode ch in xmlchaps)
                            chaptersList.Add(int.Parse(ch.Attributes.GetNamedItem("cnumber").Value));
                        chapterCount.Add(book_node.Attributes.GetNamedItem("bname").Value, chaptersList.ToArray());
                    }
                }
                else if (book >= 40 && book <= 66)
                {
                    if (book_node.Attributes.GetNamedItem("bname") == null)
                    {
                        XmlAttribute attribute;
                        attribute = BibleXmlDocument.CreateAttribute("bname");
                        attribute.Value = Localization.getBookNames()[FormatUtil.zefaniaBookNoToIndex(book)];
                        book_node.Attributes.Append(attribute);
                    }
                    if (ntBibleBooks.Length < book - 39 + 1)
                        Array.Resize(ref ntBibleBooks, book - 39 + 1);
                    ntBibleBooks[book - 40] = book_node.Attributes.GetNamedItem("bname").Value;
                    if (!chapterCount.ContainsKey(book_node.Attributes.GetNamedItem("bname").Value))
                    {
                        XmlNodeList xmlchaps = book_node.SelectNodes("CHAPTER");                        
                        List<int> chaptersList = new List<int>();
                        foreach(XmlNode ch in xmlchaps)                        
                            chaptersList.Add(int.Parse(ch.Attributes.GetNamedItem("cnumber").Value));                        
                        chapterCount.Add(book_node.Attributes.GetNamedItem("bname").Value, chaptersList.ToArray());
                    }
                }
                else if (book > 66)
                {
                    isApocrypha = true;

                    /* 
                     * adding apocrypha support
                     * currently supports the following but the code doesn't restrict more.                     
                     * 
                        67	 Judit	 Jdt
                        68	 Wisdom	 Wis
                        69	 Tobit	 Tob
                        70	 Sirach	 Sir
                        71	 Baruch	 Bar
                        72	 1 Maccabees	 1Ma
                        73	 2 Maccabees	 2Ma
                        74	 Additions to Daniel	 xDa
                        75	 Additions to Esther	 xEs
                        76	 Prayer of Manasseh	 Man
                        77	 3 Maccabees	 3Ma
                        78	 4 Maccabees	 4Ma
                        79	 Letter of Jeremiah	 LJe
                        80	 1 Esdras	 1Es
                        81	 2 Esdras	 2Es
                        82	 Odes	 Ode
                        83	 Psalms of Solomon	 PsS
                        84	 Epistle to the Laodiceans	 Lao
                        85	 1 Enoch	 1En
                        86	 kGen	 kGn
                        87	 Susanna	 Sus
                        88	 Bel and the Dragon	 Bel
                        89	 Psalm 151	 Ps2

                        901	Prayer of Azariah	Aza
                        902	Greek Esther	EsG
                        903	Greek Daniel	DaG
                        904	Jubilees	Jub
                        905	Ezra Apocalypse	4Ez
                        906	5 Ezra	5Ez
                        907	6 Ezra	6Ez
                        908	5 Apocryphal Syriac Psalms	Ps3
                        909	Syriac Apocalypse of Baruch	2Ba
                        910	4 Baruch	4Ba
                        911	Letter of Baruch	LBa
                        912	1 Meqabyan	1Mq
                        913	2 Meqabyan	2Mq
                        914	3 Meqabyan	3Mq
                        915	Reproof	Rep

                    */


                    if (book_node.Attributes.GetNamedItem("bname") == null)
                    {
                        XmlAttribute attribute;
                        attribute = BibleXmlDocument.CreateAttribute("bname");
                        attribute.Value = Localization.getBookNames()[FormatUtil.zefaniaBookNoToIndex(book)];
                        book_node.Attributes.Append(attribute);
                    }


                    try
                    {
                        if (book > 900)
                        {
                            if (apBibleBooks.Length < (book - 811) - 66 + 1)
                                Array.Resize(ref apBibleBooks, book - 66 + 1);
                        }
                        else
                        {
                            if (apBibleBooks.Length < book - 66 + 1)
                                Array.Resize(ref apBibleBooks, book - 66 + 1);
                        }
                    }
                    catch (Exception e)
                    {
                        //Themes.DarkMessageBoxShow(e.Message + ", book = " + book + ", apBibleBooks.len = " + apBibleBooks.Length);
                    }

                    apBibleBooks[book - 67] = book_node.Attributes.GetNamedItem("bname").Value;
                    if (!chapterCount.ContainsKey(book_node.Attributes.GetNamedItem("bname").Value))
                    {
                        XmlNodeList xmlchaps = book_node.SelectNodes("CHAPTER");
                        List<int> chaptersList = new List<int>();
                        foreach (XmlNode ch in xmlchaps)
                            chaptersList.Add(int.Parse(ch.Attributes.GetNamedItem("cnumber").Value));
                        chapterCount.Add(book_node.Attributes.GetNamedItem("bname").Value, chaptersList.ToArray());
                    }
                }
			}
			
			string lvi;
			TreeNode ot_lvg = new TreeNode(Localization.Old_Testament);
            TreeNode nt_lvg = new TreeNode(Localization.New_Testament);
            TreeNode ap_lvg = new TreeNode(Localization.Apocrypha);

            foreach (string tempLoopVar_lvi in otBibleBooks)
			{
				lvi = tempLoopVar_lvi;
				if (lvi == null || lvi == "")
				{
					continue;
				}
				populateBook(lvi, ot_lvg.Nodes.Add(lvi));
			}
			
			foreach (string tempLoopVar_lvi in ntBibleBooks)
			{
				lvi = tempLoopVar_lvi;
				if (lvi == null || lvi == "")
				{
					continue;
				}
				populateBook(lvi, nt_lvg.Nodes.Add(lvi));
			}

            if (isApocrypha)
            {
                foreach (string tempLoopVar_lvi in apBibleBooks)
                {
                    lvi = tempLoopVar_lvi;
                    if (lvi == null || lvi == "")
                    {
                        continue;
                    }
                    populateBook(lvi, ap_lvg.Nodes.Add(lvi));
                }
            }

            Add delegatedAdd = new Add(treeview.Nodes.Add);
			
			if (ot_lvg.GetNodeCount(false) > 0)
			{
				treeview.Invoke(delegatedAdd, new object[] {ot_lvg});
			}
			if (nt_lvg.GetNodeCount(false) > 0)
			{
				treeview.Invoke(delegatedAdd, new object[] {nt_lvg});
			}
			if (isApocrypha)
			{
                treeview.Invoke(delegatedAdd, new object[] { ap_lvg });
            }
			if (treeview.Nodes.Count > 0)
			{
				Expand delegatedExpand = new Expand(treeview.Nodes[0].Expand);
				treeview.Invoke(delegatedExpand);

                string fullpath = treeview.Nodes[0].FirstNode.FirstNode.FullPath;
                return fullpath.Split(treeview.PathSeparator.ToCharArray());
			}
			return null;
		}
		
		public bool IsComplete
		{
			get
			{
				return process_complete;
			}
		}
		
		public string getReaderErrorHtml(Exception exception)
		{
			StringBuilder errhtml = new StringBuilder();
			errhtml.Append("<html><head>");
			errhtml.Append(Themes.CSS_UI);
            errhtml.Append("</head><body><h2>" +  Localization.ErrorPage_NoBibleBooksFound + "</h2>");
			errhtml.Append(Localization.ErrorPage_ErrorDisplaying+" \'");
			errhtml.Append(FileName);
			errhtml.Append("\'.");

            if (GlobalMemory.getInstance().DevMode)
            {
                errhtml.Append("<br><br>Message: ");
                errhtml.Append(exception.Message);
                int F;
                StackTrace S = new StackTrace(exception);
                errhtml.Append("<br><br>StackFrame:<br><pre>");
                //for (F = 0; F <= S.FrameCount - 1; F++)
                //{
                //    errhtml.Append("\r\n");
                //    errhtml.Append(S.GetFrame(F).ToString());
                //}
                errhtml.Append(S);
                errhtml.Append("</pre>");
            }
            else
            {
                errhtml.Append(Localization.ErrorPage_Html);
            }
			errhtml.Append("</body></html>");
			return errhtml.ToString();
		}

        public string getExportErrorHtml(Exception exception)
        {
            StringBuilder errhtml = new StringBuilder();
            errhtml.Append("<html><head>");
            errhtml.Append(Themes.CSS_UI);
            errhtml.Append("</head><body><h2>Error Exporting Bible</h2>");
            errhtml.Append("Error exporting \'");
            errhtml.Append(FileName);
            errhtml.Append("\'.");

            if (GlobalMemory.getInstance().DevMode)
            {
                errhtml.Append("<br><br>Message: ");
                errhtml.Append(exception.Message);
                int F;
                StackTrace S = new StackTrace(exception);
                errhtml.Append("<br><br>StackFrame:<br><pre>");
                for (F = 0; F <= S.FrameCount - 1; F++)
                {
                    errhtml.Append("\r\n");
                    errhtml.Append(S.GetFrame(F).ToString());
                }
                errhtml.Append("</pre>");
            }
            else
            {   
                errhtml.Append("<br><br><h4>So, what now?</h4>");
                errhtml.Append("<p>If you think the input is a valid Bible format, please contact me using the contact page from "+GlobalMemory.AUTHOR_WEBSITE +" and I will try to investigate and fix that for you.</p>");
            }
            errhtml.Append("</body></html>");
            return errhtml.ToString();
        }

        public static string getFilters()
        {
            if (filters == null)
            {
                // INDEX MUST NOT CHANGE WHEN SAVE IS TRUE!!!

                StringBuilder sb = new StringBuilder();                
                sb.Append("Zefania XML Bibles|*.xml|"); //idx = 1
                sb.Append("The Unbound Bible (Unmapped-BCV)|*.txt|"); //idx = 2
                sb.Append("The Unbound Bible (Unmapped-BCVS)|*.txt|"); //idx = 3
                sb.Append("Open Scripture Information Standard (OSIS)|*.xml|"); //idx = 4
                sb.Append("General Bible Format (GBF)|*.gbf|"); //idx = 5
                sb.Append("XML Scripture Encoding Model (XSEM)|*.xml|"); //idx = 6
                sb.Append("The Word Bible |*.ont;*.ot;*.nt;*.ontx;*.otx;*.ntx|"); //idx = 7
                sb.Append("e-Sword Bible 8.x (BBL)|*.bbl|"); //idx = 8
                sb.Append("e-Sword Bible 9.x and above (BBLX)|*.bblx|"); //idx = 9
                sb.Append("e-Sword HD Bible (BBLI)|*.bbli|"); //idx = 10
                sb.Append("The SWORD Project|*.zip|"); //idx = 11
                sb.Append("Verse Per Line|*.vpl|"); //idx = 12
                sb.Append("Bible Companion|*.bib|"); //idx = 13
                sb.Append("Go Bible|*.jar|"); //idx = 14
                sb.Append("Theological Markup Language (ThML)|*.xml;*.thm;*.thml|"); //idx = 15
                sb.Append("Open Song Bibles|*.xmm;*.xml|"); //idx = 16
                sb.Append("OpenLP.org Bibles|*.sqlite|"); //idx = 17
                sb.Append("MySword for Android|*.bbl.mybible.gz;*.bbl.mybible|"); //idx = 18
                sb.Append("Bible Analyzer Bibles|*.bib|"); //idx = 19
                sb.Append("EasySlides Bibles|*.mdb|"); //idx = 20
                sb.Append("MP3 Bibles|*.mp3|"); //idx = 21
                sb.Append("LOGOS Import File|*.docx|"); //idx = 22
                sb.Append("OnlineBible|*.exp|"); //idx = 23
                sb.Append("Palm Bible+|*.pdb|"); //idx = 24
                sb.Append("HeavenWorld Bibles for Windows 8|*.hbb|"); //idx = 25
                sb.Append("VerseVIEW|*.xml;*.db|"); //idx = 26
                sb.Append("MyBible|*.SQLite3;*.sqlite3|"); //idx = 27                
                sb.Append("Unified Scripture Format XML (USFX)|*.xml|"); //idx = 28
                sb.Append("SwordSearcher/Forge for SwordSearcher|*.ss5bible;*.ss6bible;*.ss7bible;*.ss8bible;*.txt"); //idx = 29
                filters =sb.ToString(); 

                // don't forget the | pipe symbol at the last but before index.
            }
            return filters;
        }

        public XmlDocument getXmlDoc()
        {
            return BibleXmlDocument;
        }

        public static string postProcessContent(string input, string rtf_header = null)
        {
            string output = input;
            if (GlobalMemory.getInstance().ParseBible)
            {
                // prevent screensaver
                if(GlobalMemory.getInstance().UseCtrlForHtmlRtfConv)                
                    Win32NativeCalls.preventScreensaver();
                

                if (GlobalMemory.getInstance().ConvertRtfToHtmlBible)
                {
                    output = FormatUtil.convertRtfToHtml(output);
                }
                else if(GlobalMemory.getInstance().autoSetParsingBible && BibleFormat.getInstance().CLEANING)
                {
                    // force cleaning rtf to html if set to auto and format requires it.
                    output = FormatUtil.convertRtfToHtml(output);
                }

                if (GlobalMemory.getInstance().ConvertHtmlToRtfBible)
                {
                    output = FormatUtil.convertHtmlToRtf(output);
                }

                // finally strip html
                if (GlobalMemory.getInstance().stripHtmlTagsBible)
                {
                    output = FormatUtil.StripHTML(output);
                }

                // restore it back
                if (GlobalMemory.getInstance().UseCtrlForHtmlRtfConv)
                    Win32NativeCalls.restoreScreensaver();
            }
            return output;
        }

        public void addRedText()
        {
            XmlNodeList verses = BibleXmlDocument.SelectNodes("//VERS");

            int bno = -1;
            int cno = -1;
            int vno = -1;
            string verseText = null;
            string[] arrayRed = null;
            foreach (XmlNode verse in verses)
            {
                verseText = verse.InnerText;                
                int.TryParse(verse.Attributes["vnumber"].Value, out vno);
                int.TryParse(verse.ParentNode.Attributes["cnumber"].Value, out cno);
                int.TryParse(verse.ParentNode.ParentNode.Attributes["bnumber"].Value, out bno);

                if (RedTextUtil.isRedText(bno, cno, vno))
                {
                    verseText = Regex.Replace(verseText, @"\p{Cc}", "\"");

                    // complete verse is red text.
                    if (RedTextUtil.isRedTextComplete(bno, cno, vno))
                    {
                        verse.InnerXml = "<STYLE css=\"color:#ff0000\">\"" + FormatUtil.EscapeXML(FormatUtil.UnescapeXML(verseText)) + "\"</STYLE>";                      
                    }
                    else
                    {
                        // partial verse is red text.
                        string splitChar = "\"";
                        verseText = verseText.Replace('“', '"').Replace('”', '"');
                        if (verseText.Contains("\"") && verseText.Contains("'"))
                        {
                            arrayRed = verseText.Split(new char[] { '"' });
                            splitChar = "\"";
                        }
                        else if (verseText.Contains("\"") && !verseText.Contains("'"))
                        {
                            arrayRed = verseText.Split(new char[] { '"' });
                            splitChar = "\"";
                        }
                        else if (verseText.Contains("'") && !verseText.Contains("\""))
                        {
                            arrayRed = verseText.Split(new char[] { '\'' });
                            splitChar = "\'";
                        }
                        if (arrayRed == null)
                        {
                            verse.InnerXml = "<STYLE css=\"color:#ff0000\">" + splitChar + FormatUtil.EscapeXML(FormatUtil.UnescapeXML(verseText)) + splitChar + "</STYLE>";
                            continue;
                        }


                        if (arrayRed.Length == 1 && verseText.Trim() != "")
                        {
                            // doesn't have any quotes. prob non-english
                            verse.InnerXml = "<STYLE css=\"color:#ff0000\">" + FormatUtil.EscapeXML(FormatUtil.UnescapeXML(verseText)) + "</STYLE>";
                        }
                        else if (arrayRed.Length == 2)
                        {
                            // only one quote found., hence, starts middle and ends at last
                           verse.InnerXml = FormatUtil.EscapeXML(FormatUtil.UnescapeXML(arrayRed[0])) + "<STYLE css=\"color:#ff0000\">" + splitChar + FormatUtil.EscapeXML(FormatUtil.UnescapeXML(arrayRed[1])) + splitChar + "</STYLE>";
                        }
                        else if (arrayRed.Length == 3)
                        {
                            // 2 quote found. hence, the middle segment is red.
                            verse.InnerXml = FormatUtil.EscapeXML(FormatUtil.UnescapeXML(arrayRed[0])) + "<STYLE css=\"color:#ff0000\">"+ splitChar + FormatUtil.EscapeXML(FormatUtil.UnescapeXML(arrayRed[1])) + splitChar+"</STYLE>" + FormatUtil.EscapeXML(FormatUtil.UnescapeXML(arrayRed[2]));
                        }
                        else //default is complete verse
                        {
                            if (verseText.Trim() != "")
                                verse.InnerXml = "<STYLE css=\"color:#ff0000\">"+ splitChar + FormatUtil.EscapeXML(FormatUtil.UnescapeXML(verseText)) + splitChar+"</STYLE>";
                        }
                    }
                }
            }
        }

        public virtual List<string[]> getProperties()
        {
            List<string[]> kv = new List<string[]>();
            kv.Add(new string[] { "Abbreviation:", BibleFormat.getInstance().ABBREVIATION });
            kv.Add(new string[] { "Description:", BibleFormat.getInstance().DESCRIPTION });
            kv.Add(new string[] { "Comments:", BibleFormat.getInstance().COMMENTS });
            return kv;
        }

        public virtual void setProperties(List<string[]> kv) { }

        public abstract void Load();
		public abstract int PercentComplete {get;}

        public virtual string AdditionaStatusText() { return ""; }
        public abstract int FilterIndex {get;}

        public abstract void ExportBible(string filename, int idx);
	}
	
}
