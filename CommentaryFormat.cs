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
	public abstract class CommentaryFormat
	{

        static string filters = null;
        public string ABBREVIATION = "CMT";
        public string DESCRIPTION = "Commentary";
        public string COMMENTS = "Exported from Simple Bible Reader (" + GlobalMemory.AUTHOR_WEBSITE + ")";


        private string m_filename;
		
		private static System.Xml.XmlDocument xmldoc = new System.Xml.XmlDocument();
				
		private bool process_complete = false;

        public string rtf_header = @"{\rtf1\ansi\ansicpg1252\deff0\deflang1033{\fonttbl{\f1\fnil\fprq2\fcharset161 TITUS Cyberbit Basic;}{\f2\fnil\fprq2\fcharset177 TITUS Cyberbit Basic;}{\f3\fnil\fprq2\fcharset0 TITUS Cyberbit Basic;}{\f4\fnil\fprq2\fcharset136 PMingLiU;}{\f5\fnil\fprq2\fcharset204 Georgia;}{\f6\fnil\fprq2\fcharset238 Georgia;}{\f7\fnil\fprq2\fcharset134 PMingLiU;}{\f8\fnil\fprq2\fcharset222 Cordia New;}{\f9\fnil\fprq2\fcharset129 Gulim;}{\f10\fnil\fprq2\fcharset178 TITUS Cyberbit Basic;}{\f11\fnil\fprq2\fcharset162 Georgia;}{\f12\fnil\fprq2\fcharset163 Times New Roman;}{\f13\fnil\fprq2\fcharset128 MS Mincho;}}{\colortbl ;\red0\green0\blue0;\red0\green0\blue255;\red0\green255\blue255;\red0\green255\blue0;\red255\green0\blue255;\red255\green0\blue0;\red255\green255\blue0;\red255\green255\blue255;\red0\green0\blue128;\red0\green128\blue128;\red0\green128\blue0;\red128\green0\blue128;\red128\green0\blue0;\red128\green128\blue0;\red128\green128\blue128;\red192\green192\blue192;}{\viewkind4\uc1\pard\plain\cf0\fs\par}";
		
		public static CommentaryFormat commentary_format;

        public bool CLEANING = false;

		public CommentaryFormat(string filename)
		{
			m_filename = filename;
			process_complete = false;
		}

        public static CommentaryFormat getInstance()
        {
            return commentary_format;
        }

        public static CommentaryFormat CommentaryFactory(string filename)
        {
            commentary_format = null;            
            if (filename.ToLower().EndsWith(".xml"))
            {
                string xml_str = System.IO.File.ReadAllText(filename).ToLower();
                if (FormatUtil.InStr(xml_str, "<xmlbible"))
                {
                    xml_str = null;
                    commentary_format = new ZefaniaCommentaryFormat(filename);
                }
            }
            else if (filename.ToLower().EndsWith(".zip"))
            {
                commentary_format = new SwordCommentaryFormat(filename);
            }
            else if (System.IO.Directory.Exists(filename))
            {
                //Its a directory
                if (System.IO.Directory.Exists(filename + "\\mods.d"))
                {
                    commentary_format = new SwordCommentaryFormat(filename);
                }
            }
            else if (filename.ToLower().EndsWith(".cmt.twm"))
            {
                commentary_format = new TheWordCommentaryFormat(filename);
            }
            else if (filename.ToLower().EndsWith(".cmtx"))
            {
                commentary_format = new CMTXCommentaryFormat(filename);
            }
            else if (filename.ToLower().EndsWith(".cmti"))
            {
                commentary_format = new CMTICommentaryFormat(filename);
            }
            else if (filename.ToLower().EndsWith(".cmt"))
            {
                FileStream fs = System.IO.File.OpenRead(filename);                
                int b=fs.ReadByte();
                fs.Close();
                if (b == 0x53) // Checking for S in Sqlite 
                {
                    commentary_format = new BibleAnalyzerCommentaryFormat(filename);
                }
                else
                    commentary_format = new CMTCommentaryFormat(filename);
            }
            else if (filename.ToLower().EndsWith(".cmt.mybible.gz"))
            {
                commentary_format = new MySword4AndroidCommentaryFormat(filename, true);
            }
            else if (filename.ToLower().EndsWith(".exp"))
            {
                commentary_format = new OnlineBibleCommentaryFormat(filename);
            }
            else if (filename.ToLower().EndsWith(".cmt.mybible"))
            {
                commentary_format = new MySword4AndroidCommentaryFormat(filename, false);
            }
            else if (filename.ToLower().EndsWith(".commentaries.sqlite3"))
            {
                commentary_format = new MyBibleCommentaryFormat(filename);
            }
            else if (filename.ToLower().EndsWith(".ss5cmty"))
            {
                commentary_format = new SwordSearcherCommentaryFormat(filename);
            }
            else if (filename.ToLower().EndsWith(".txt"))
            {
                StreamReader sr = File.OpenText(filename);
                string line = sr.ReadLine();
                sr.Close();
                if (line.StartsWith(";"))
                    commentary_format = new SwordSearcherCommentaryFormat(filename);
                else
                {
                    // any other commentary in text format. nothing yet.
                }
            }
            if (commentary_format != null)
            {
                if (cleaningRequired(commentary_format.FilterIndex))
                {
                    commentary_format.CLEANING = true;
                }
                if (commentary_format.ABBREVIATION == "CMT")
                    commentary_format.ABBREVIATION = Path.GetFileNameWithoutExtension(filename);
                if (commentary_format.DESCRIPTION == "Commentary")
                    commentary_format.DESCRIPTION = Path.GetFileNameWithoutExtension(filename);
            }
            return commentary_format;
        }

        public static CommentaryFormat CommentaryFactory(string filename, int filterIdx)
        {
            if (filterIdx == 0)
                commentary_format = CommentaryFactory(filename);
            else
                commentary_format = getCommentaryFormat(filename, filterIdx);
            return commentary_format;
        }

        public static CommentaryFormat getCommentaryFormat(string filename, int filterIdx)
        {
            CommentaryFormat commentary_format_export = null;
            switch (filterIdx)
            {
                case 1:
                    commentary_format_export = new ZefaniaCommentaryFormat(filename);
                    break;
                case 2:
                    commentary_format_export = new SwordCommentaryFormat(filename);
                    break;
                case 3:
                    commentary_format_export = new TheWordCommentaryFormat(filename);
                    break;
                case 4:
                    commentary_format_export = new CMTCommentaryFormat(filename);
                    break;
                case 5:
                    commentary_format_export = new CMTXCommentaryFormat(filename);
                    break;
                case 6:
                    commentary_format_export = new CMTICommentaryFormat(filename);
                    break;
                case 7:
                    commentary_format_export = new BibleAnalyzerCommentaryFormat(filename);
                    break;
                case 8:
                    commentary_format_export = new OnlineBibleCommentaryFormat(filename);
                    break;
                case 9:
                    commentary_format_export = new MyBibleCommentaryFormat(filename);
                    break;
                case 10:
                    commentary_format_export = new SwordSearcherCommentaryFormat(filename);
                    break;
                case 11:
                    commentary_format_export = new MySword4AndroidCommentaryFormat(filename, true);
                    break;

                default:
                    if (!GlobalMemory.getInstance().ConsoleMode)
                        Themes.MessageBox("Sorry, export for this Commentary format is not yet implemented!");
                    else
                        Console.WriteLine("Sorry, export for this Commentary format is not yet implemented!");
                    return null;
            }
            return commentary_format_export;
        }

        public static CommentaryFormat doExportCommentary(string filename, int filterIdx)
        {
            CommentaryFormat commentary_format_export = getCommentaryFormat(filename, filterIdx);
            if (commentary_format_export != null)
            {
                commentary_format_export.CLEANING = commentary_format.CLEANING;
                commentary_format_export.ABBREVIATION = commentary_format.ABBREVIATION.Replace("'", "''");
                commentary_format_export.DESCRIPTION = commentary_format.DESCRIPTION.Replace("'", "''");
                commentary_format_export.COMMENTS = commentary_format.COMMENTS.Replace("'", "''");
                if (commentary_format_export.ABBREVIATION == "CMT")
                    commentary_format_export.ABBREVIATION = Path.GetFileNameWithoutExtension(filename).Replace("'", "''");
                if (commentary_format_export.DESCRIPTION == "Commentary")
                    commentary_format_export.DESCRIPTION = Path.GetFileNameWithoutExtension(filename).Replace("'", "''");
            }
            return commentary_format_export;
        }

        public static string postProcessContent(string input, string rtf_header = null)
        {
            string output = input;
            if (GlobalMemory.getInstance().ParseCommentary)
            {
                // prevent screensaver
                if (GlobalMemory.getInstance().UseCtrlForHtmlRtfConv)
                    Win32NativeCalls.preventScreensaver();

                if (GlobalMemory.getInstance().ConvertRtfToHtmlCommentary)
                {
                    output = FormatUtil.convertRtfToHtml(output);
                }
                if (GlobalMemory.getInstance().ConvertHtmlToRtfCommentary)
                {
                    output=FormatUtil.convertHtmlToRtf(output);
                }
                // finally strip html tags
                if (GlobalMemory.getInstance().stripHtmlTagsCommentary)
                {
                    output = FormatUtil.StripHTML(output);
                }

                // restore it back
                if (GlobalMemory.getInstance().UseCtrlForHtmlRtfConv)
                    Win32NativeCalls.restoreScreensaver();
            }
            return output;
        }

        public static bool cleaningRequired(int FilterIdx)
        {
            int[] required = new int[] { 3, 4, 5 };
            for (int i = 0; i < required.Length; i++)
                if (required[i] == FilterIdx)
                    return true;            
            return false;
        }

		public void SetProcessAsComplete()
		{
			process_complete = true;
		}		

        public string getFirstCommentary()
        {
            XmlNode v = xmldoc.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[1]/CHAPTER[1]/VERS[1]");
            if (v != null)
                return v.InnerText;
            else
                return "";
        }

		public static System.Xml.XmlDocument CommentaryXmlDocument
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
		
		public delegate void Clear();
        public delegate int AddItem(string str);

		public void populateVerses(ListBox list)
		{
            Clear delegatedClear = new Clear(list.Items.Clear);
            AddItem delegatedAdd = new AddItem(list.Items.Add);
            list.Invoke(delegatedClear, new object[] { });

            //list.Items.Clear();
            XmlNodeList verses = CommentaryXmlDocument.SelectNodes("//VERS");
            string book=null;
            string chap=null;
            string vers=null;
            foreach (XmlNode verse in verses)
            {                
                if (verse.ParentNode.ParentNode.Attributes["bname"] != null)
                    book = verse.ParentNode.ParentNode.Attributes["bname"].Value;
                else
                    book = FormatUtil.shortBookNames[int.Parse(verse.ParentNode.ParentNode.Attributes["bnumber"].Value) - 1];
                chap = verse.ParentNode.Attributes["cnumber"].Value;
                vers = verse.Attributes["vnumber"].Value;
                //list.Items.Add(book + " " + chap + ":" + vers);
                list.Invoke(delegatedAdd, new object[] { book + " " + chap + ":" + vers });
            }
		}


        private string getCmtHtml(string cmt_text, XmlNode child)
        {
            string tmp = null;
            if (child.Name == "title")
            {
                if (cmt_text != "")
                    cmt_text = cmt_text + "<br><br><b>" + child.InnerText + "</b><br>";
                else
                    cmt_text = cmt_text + "<b>" + child.InnerText + "</b><br>";
            }
            else if (child.Name == "see")
            {
                if (child.Attributes["target"].Value == "x-self")
                    cmt_text = cmt_text + "<a OnClick=\"chrome.webview.postMessage('C" + child.InnerText + "');\">" + child.InnerText + "</a><br>";
                else
                    cmt_text = cmt_text + "<a target='_blank' OnClick=\"chrome.webview.postMessage('" + child.Attributes["target"].Value + "');\">" + child.InnerText + "</a><br>";
            }
            else if (child.Name == "reflink")
            {
                if (child.Attributes["target"] != null)
                {
                    cmt_text = cmt_text + "<a OnClick=\"chrome.webview.postMessage('B" + child.Attributes["target"].Value + "');\">" + child.InnerText + "</a>";
                }
                else if (child.Attributes["mscope"] != null)
                {
                    string[] arr = child.Attributes["mscope"].Value.Split(';');
                    cmt_text = cmt_text + "<a OnClick=\"chrome.webview.postMessage('B" + child.Attributes["mscope"].Value + "');\">" + FormatUtil.shortBookNames[int.Parse(arr[0]) - 1] + " " + arr[1] + ":" + arr[2] + "</a>";
                }
            }
            else if (child.Name == "#text")
                cmt_text = cmt_text + child.InnerText;
            else
            {
                if (child.HasChildNodes)
                {
                    foreach (XmlNode c in child.ChildNodes)
                    {
                        cmt_text = getCmtHtml(cmt_text, c);
                    }
                }
                else
                {
                    // <a class='para' href="#hlah blah blah">text display</a>";
                    // <a class='para' OnClick="chrome.webview.postMessage('#link');">text</a>";
                    tmp = child.InnerText;
                    if(tmp.Contains("<a"))
                    {
                        // anchor tag exists.
                        tmp = Regex.Replace(tmp, @"href=""([^""]+)""([^>]*)>([^<]+)</a>", "OnClick=\"chrome.webview.postMessage('$1');\"$2>$3</a>", RegexOptions.IgnoreCase);
                        tmp = Regex.Replace(tmp, @"href='([^']+)'([^>]*)>([^<]+)</a>", "OnClick='chrome.webview.postMessage('$1');\"$2>$3</a>", RegexOptions.IgnoreCase);
                    }
                    cmt_text = cmt_text + tmp;
                }
            }
            return cmt_text;
        }

        public virtual string getCmtRaw(string cmtry_text, XmlNode child)
        {
            if (child.Value != null)
            {
                if (child.Value != "")
                    cmtry_text = cmtry_text + "<br/><textarea rows='10' readonly='readonly'>" + FormatUtil.UnescapeXML(FormatUtil.UnescapeXML(child.Value)) + "</textarea><br/>";
            }
            if (child.HasChildNodes)
            {
                foreach (XmlNode c in child.ChildNodes)
                {
                    cmtry_text = getCmtRaw(cmtry_text, c);
                }
            }
            return cmtry_text;
        }

        public virtual string getCommentaryForVerseRaw(int book_no, int chapter_no, int verse_no)
        {
            XmlDocument xdoc = CommentaryXmlDocument;
            string tmp = null;
            XmlElement vers_elmt = (XmlElement)CommentaryXmlDocument.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber='" + book_no.ToString() + "']/CHAPTER[@cnumber='" + chapter_no.ToString() + "']/VERS[@vnumber='" + verse_no.ToString() + "']");
            string commentary_text = "";
            if (vers_elmt != null)
            {
                commentary_text = getCmtRaw("", vers_elmt);
                commentary_text = "<html dir=\'ltr\'><head>" + Themes.CSS_MAIN + "</head><body>" + commentary_text + "</body></html>";

            }
            else
            {
                commentary_text = "";
            }
            return commentary_text;
        }


        public virtual string getCommentaryForVerse(int book_no,int chapter_no,int verse_no)
        {
            if (GlobalMemory.getInstance().DisplayCMTRaw)
                return getCommentaryForVerseRaw(book_no,chapter_no,verse_no);

            XmlDocument xdoc = CommentaryXmlDocument;
            string tmp = null;
            XmlElement vers_elmt = (XmlElement)CommentaryXmlDocument.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber='" + book_no.ToString() + "']/CHAPTER[@cnumber='" + chapter_no.ToString() + "']/VERS[@vnumber='" + verse_no.ToString() + "']");
            string commentary_text = "";
            if (vers_elmt != null)
            {
                //Themes.DarkMessageBoxShow(vers_elmt.InnerXml);
                if (vers_elmt.InnerXml.IndexOf("<")!=-1)
                {
                    //Console.WriteLine(vers_elmt.HasChildNodes);
                    commentary_text = getCmtHtml("", vers_elmt);//.InnerText;
                    commentary_text = commentary_text.Replace("\r\n", "<br>");
                    commentary_text = commentary_text.Replace("\n", "<br>");
                    commentary_text = commentary_text.Replace("\r", "<br>");
                }
                else
                {
                    // anchor tag exists.
                    tmp = vers_elmt.InnerText;
                    tmp = Regex.Replace(tmp, @"href=""([^""]+)""([^>]*)>([^<]+)</a>", "OnClick=\"chrome.webview.postMessage('$1');\"$2>$3</a>", RegexOptions.IgnoreCase);
                    tmp = Regex.Replace(tmp, @"href='([^']+)'([^>]*)>([^<]+)</a>", "OnClick='chrome.webview.postMessage('$1');\"$2>$3</a>", RegexOptions.IgnoreCase);


                    commentary_text = "<p class='pre'>"+tmp+"</p>";
                }
                commentary_text = "<html dir=\'ltr\'><head><title>"  + DESCRIPTION + "</title>" + Themes.CSS_COMMENTARY + "</head><body>" + commentary_text + "</body></html>";
                
            }
            else
            {
                commentary_text = "";
            }
            return commentary_text;
        }
	
		public bool IsComplete
		{
			get
			{
				return process_complete;
			}
		}

        public static String PrintXML(String XML)
        {
            String Result = "";
            MemoryStream mStream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(mStream, Encoding.UTF8);
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
            catch (XmlException e)
            {
                Themes.MessageBox(e.Message);
                Result = XML;
            }

            mStream.Close();
            writer.Close();

            return Result;
        }

		public string getReaderErrorHtml(Exception exception)
		{
			StringBuilder errhtml = new StringBuilder();
			errhtml.Append("<html><head>");
			errhtml.Append(Themes.CSS_UI);
            errhtml.Append("</head><body><h2>No Commentary details found!</h2>");
			errhtml.Append("Error displaying \'");
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
                errhtml.Append("<p>If you think this is a valid commentary format, please contact me using the contact page from " + GlobalMemory.AUTHOR_WEBSITE + " and I will try to investigate and fix that for you. The software is developed ");
                errhtml.Append("to make sure all commentary irrespective of any format, must be viewable and exportable. ");
                errhtml.Append("You can also visit <a href='https://" + GlobalMemory.AUTHOR_WEBSITE + "'>" + GlobalMemory.AUTHOR_WEBSITE + "</a> to check if ");
                errhtml.Append("there is any new version of this software.</p>");
            }
			errhtml.Append("</body></html>");
			return errhtml.ToString();
		}

        public string getExportErrorHtml(Exception exception)
        {
            StringBuilder errhtml = new StringBuilder();
            errhtml.Append("<html><head>");
            errhtml.Append(Themes.CSS_UI);
            errhtml.Append("</head><body><h2>Error Exporting Commentary</h2>");
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
                errhtml.Append("<p>If you think the input is a valid commentary format, please contact me using the contact page from " + GlobalMemory.AUTHOR_WEBSITE + " and I will try to investigate and fix that for you.</p>");
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
                sb.Append("Zefania XML Format|*.xml|"); //idx = 1    
                sb.Append("SWORD Commentary Modules|*.zip|"); //idx = 2       
                sb.Append("The Word Commentary Modules|*.cmt.twm|"); //idx = 3  
                sb.Append("e-Sword 8.x Commentary Modules|*.cmt|"); //idx = 4
                sb.Append("e-Sword 9.x and above (CMTX) |*.cmtx|"); //idx = 5   
                sb.Append("e-Sword HD (CMTI) |*.cmti|"); //idx = 6
                sb.Append("Bible Analyzer Commentary|*.cmt|"); //idx = 7
                sb.Append("OnlineBible Commentary|*.exp|"); //idx = 8
                sb.Append("MyBible Commentary|*.commentaries.SQLite3;*.commentaries.sqlite3|"); //idx = 9
                sb.Append("SwordSearcher/Forge for SwordSearcher|*.ss5cmty;*.txt|"); //idx = 10
                sb.Append("MySword for Android|*.cmt.mybible.gz;*.cmt.mybible"); //idx = 11
                filters = sb.ToString(); 
            }
            return filters;
        }

        public XmlDocument getXmlDoc()
        {
            return CommentaryXmlDocument;
        }

		public abstract void Load();
		public abstract int PercentComplete {get;}
        public abstract int FilterIndex {get;}

        public abstract void ExportCommentary(string filename, int idx);
	}
	
}
