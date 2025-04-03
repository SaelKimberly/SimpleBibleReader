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
using System.Reflection;

namespace Simple_Bible_Reader
{
	public abstract class BookFormat
	{

        static string filters = null;
        public string ABBREVIATION = "BOOK";
        public string DESCRIPTION = "Book";
        public string COMMENTS = "Exported from Simple Bible Reader (" + GlobalMemory.AUTHOR_WEBSITE + ")";
		private Hashtable chapterCount = new Hashtable();
		private string m_filename;
		private static System.Xml.XmlDocument xmldoc = new System.Xml.XmlDocument();
		
		private bool process_complete = false;
		
		public delegate int  Add(TreeNode i);
		public delegate void Expand();
        public static bool _internal_maincss = false;

        public static BookFormat book_format=null;
        public bool CLEANING = false;

        public string rtf_header = @"{\rtf1\ansi\ansicpg1252\deff0\deflang1033{\fonttbl{\f1\fnil\fprq2\fcharset161 TITUS Cyberbit Basic;}{\f2\fnil\fprq2\fcharset177 TITUS Cyberbit Basic;}{\f3\fnil\fprq2\fcharset0 TITUS Cyberbit Basic;}{\f4\fnil\fprq2\fcharset136 PMingLiU;}{\f5\fnil\fprq2\fcharset204 Georgia;}{\f6\fnil\fprq2\fcharset238 Georgia;}{\f7\fnil\fprq2\fcharset134 PMingLiU;}{\f8\fnil\fprq2\fcharset222 Cordia New;}{\f9\fnil\fprq2\fcharset129 Gulim;}{\f10\fnil\fprq2\fcharset178 TITUS Cyberbit Basic;}{\f11\fnil\fprq2\fcharset162 Georgia;}{\f12\fnil\fprq2\fcharset163 Times New Roman;}{\f13\fnil\fprq2\fcharset128 MS Mincho;}}{\colortbl ;\red0\green0\blue0;\red0\green0\blue255;\red0\green255\blue255;\red0\green255\blue0;\red255\green0\blue255;\red255\green0\blue0;\red255\green255\blue0;\red255\green255\blue255;\red0\green0\blue128;\red0\green128\blue128;\red0\green128\blue0;\red128\green0\blue128;\red128\green0\blue0;\red128\green128\blue0;\red128\green128\blue128;\red192\green192\blue192;}{\viewkind4\uc1\pard\plain\cf0\fs\par}";
        public System.Windows.Forms.RichTextBox richTextCtrl = new System.Windows.Forms.RichTextBox();

		public BookFormat(string filename)
		{
			m_filename = filename;
			process_complete = false;
            _internal_maincss = false;
        }

        public static BookFormat getInstance()
        {
            return book_format;
        }

        public static BookFormat BookFactory(string filename)
        {
            book_format = null;
            if (filename.ToLower().EndsWith(".xml"))
            {                
                book_format = new XMLBookFormat(filename);            
            }
            else if (filename.ToLower().EndsWith(".gbk.twm"))
            {
                book_format = new TheWordBookFormat(filename);
            } 
            else if (filename.ToLower().EndsWith(".top"))
            {
                book_format = new TOPBookFormat(filename);
            }
            else if (filename.ToLower().EndsWith(".topx") || filename.ToLower().EndsWith(".refx"))
            {
                book_format = new TOPXBookFormat(filename);
            }
            else if (filename.ToLower().EndsWith(".refi"))
            {
                book_format = new REFIBookFormat(filename);
            }
            else if (filename.ToLower().EndsWith(".zip"))
            {
                book_format = new SwordBookFormat(filename);
            }
            else if (filename.ToLower().EndsWith(".bk"))
            {
                book_format = new BibleAnalyzerBookFormat(filename);
            }
            else if (filename.ToLower().EndsWith(".bok.mybible.gz"))
            {
                book_format = new MySword4AndroidBookFormat(filename, true);
            }
            else if (filename.ToLower().EndsWith(".bok.mybible"))
            {
                book_format = new MySword4AndroidBookFormat(filename, false);
            }
            else if (Path.GetFileName(filename).ToUpper().Equals("BOOK.DAT"))
            {
                book_format = new STEPBookFormat(filename);
            }            
            else if (filename.ToLower().EndsWith(".ss5book"))
            {
                book_format = new SwordSearcherBookFormat(filename);
            }
            else if (filename.ToLower().EndsWith(".txt"))
            {
                StreamReader sr = File.OpenText(filename);
                string line = sr.ReadLine();
                sr.Close();
                if (line.StartsWith(";"))
                    book_format = new SwordSearcherBookFormat(filename);
                else
                {
                    // other books in text format
                }
            }
            else if (System.IO.Directory.Exists(filename))
            {
                //Its a directory
                if (System.IO.Directory.Exists(filename + "\\mods.d"))
                {
                    book_format = new SwordBookFormat(filename);
                }                
            }
                              

            if (book_format != null)
            {
                if (cleaningRequired(book_format.FilterIndex))
                {
                    book_format.CLEANING = true;
                }

                if (book_format.ABBREVIATION == "BOOK")
                    book_format.ABBREVIATION = Path.GetFileNameWithoutExtension(filename);
                if (book_format.DESCRIPTION == "Book")
                    book_format.DESCRIPTION = Path.GetFileNameWithoutExtension(filename);
            }
            return book_format;
        }

        public static BookFormat BookFactory(string filename, int filterIdx)
        {
            if (filterIdx == 0)
                book_format = BookFactory(filename);
            else
                book_format = getBookFormat(filename, filterIdx);
            return book_format;
        }

        public static BookFormat getBookFormat(string filename, int filterIdx)
        {
            BookFormat book_format_export = null;
            switch (filterIdx)
            {
                case 1:
                    book_format_export = new XMLBookFormat(filename);
                    break;
                case 2:
                    book_format_export = new TheWordBookFormat(filename);
                    break;
                case 3:
                    book_format_export = new TOPBookFormat(filename);
                    break;
                case 4:
                    book_format_export = new TOPXBookFormat(filename);
                    break;
                case 5:
                    book_format_export = new REFIBookFormat(filename);
                    break;
                case 6:
                    book_format_export = new SwordBookFormat(filename);
                    break;
                case 7:
                    book_format_export = new BibleAnalyzerBookFormat(filename);
                    break;
                case 8:
                    book_format_export = new MySword4AndroidBookFormat(filename, true);
                    break;
                case 9: // NOT SUPPORTED
                    book_format_export = new STEPBookFormat(filename);
                    break;
                case 10:
                    book_format_export = new SwordSearcherBookFormat(filename);
                    break;

                default:
                    if (!GlobalMemory.getInstance().ConsoleMode)
                        Themes.MessageBox("Sorry, export for this Book format is not yet implemented!");
                    else
                        Console.WriteLine("Sorry, export for this Book format is not yet implemented!");
                    return null;
            }
            return book_format_export;
        }

        public static BookFormat doExportBook(string filename,int filterIdx)
        {
            bool cleaing_required = book_format.CLEANING;
            BookFormat book_format_export = getBookFormat(filename, filterIdx);
            if (book_format_export != null)
            {
                // if the loaded bible requires cleaning, the the target much clean it before export.
                book_format_export.CLEANING = book_format.CLEANING;
                book_format_export.ABBREVIATION = book_format.ABBREVIATION.Replace("'", "''");
                book_format_export.DESCRIPTION = book_format.DESCRIPTION.Replace("'", "''");
                book_format_export.COMMENTS = book_format.COMMENTS.Replace("'", "''");
                if (book_format_export.ABBREVIATION == "BOOK")
                    book_format_export.ABBREVIATION = Path.GetFileNameWithoutExtension(filename).Replace("'", "''");
                if (book_format_export.DESCRIPTION == "Book")
                    book_format_export.DESCRIPTION = Path.GetFileNameWithoutExtension(filename).Replace("'", "''");
            }
            return book_format_export;
        }

        public static bool cleaningRequired(int FilterIdx)
        {
            int[] required = new int[] { 3,4};
            for (int i = 0; i < required.Length; i++)
                if (required[i] == FilterIdx)
                    return true;            
            return false;
        }

		public void SetProcessAsComplete()
		{
			process_complete = true;
		}
		
		
		
		
		public static System.Xml.XmlDocument BookXmlDocument
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
		
		
		public void populateBook(System.Xml.XmlNode note, string title, TreeNode booknode)
		{
			if (title == null)
			{
				return;
			}
			int count = System.Convert.ToInt32(chapterCount[title]);
			string[] insideBook = new string[count - 1 + 1];
			for (int ctr = 1; ctr <= count; ctr++)
			{
				insideBook[ctr - 1] = ctr.ToString();
			}
			foreach (string lvi in insideBook)
			{
				booknode.Nodes.Add(title + "_" + lvi, lvi);
			}

            XmlNodeList notes = note.SelectNodes("NOTES");
            foreach (System.Xml.XmlNode n in notes)
            {
                string title2 = FormatUtil.UnescapeXML(n.Attributes.GetNamedItem("title").Value);
                populateBook(n, title2, booknode.Nodes.Add(title2));
            }
		}
		
        public string getNote(string title)
        {
            XmlNode v = xmldoc.SelectSingleNode("//NOTES[@title=\'" + System.Security.SecurityElement.Escape(title) + "\']");
            if (v != null)
                if (v.FirstChild != null)
                    return v.FirstChild.Value;
                return "";
        }
        
        public string getFirstVerse()
        {
            XmlNode v = xmldoc.SelectSingleNode("/XMLBOOK/NOTES[1]");
            if (v != null)
                return v.InnerText;
            else
                return "";
        }

        public virtual string getRawChapter(string[] data)
        {
            string chapHtml = "<html dir=\'ltr\'><head>";
            chapHtml = chapHtml + Themes.CSS_MAIN + "</head><body>";
            chapHtml = chapHtml + "<br/><textarea rows='10' readonly='readonly'>" + FormatUtil.UnescapeXML(FormatUtil.UnescapeXML(getNote(data[data.Length - 1]))) + "</textarea><br/>";
            chapHtml = chapHtml + "</body></html>";
            return chapHtml;
        }

        public virtual string getChapterHtml(string[] data)
		{
            if (GlobalMemory.getInstance().DisplayBOKRaw)
                return getRawChapter(data);

            string chapHtml = "<html dir=\'ltr\'><head><title>" + DESCRIPTION + "</title>";   
            if(_internal_maincss)
                chapHtml = chapHtml + Themes.CSS_UI + "</head><body>";
            else
                chapHtml =chapHtml + Themes.CSS_MAIN + "</head><body>";
            chapHtml = chapHtml + getNote(data[data.Length - 1]);            
            chapHtml = chapHtml + "</body></html>";
			return chapHtml;
		}

        public string[] populateBooks(MirrorTreeView treeview)
		{
			chapterCount = new Hashtable();
			XmlNodeList notes = BookXmlDocument.SelectNodes("/XMLBOOK/NOTES");
			
			string title;
            Add delegatedAdd = new Add(treeview.Nodes.Add);
            TreeNode title_tn = null;
            if(this.ABBREVIATION!="BOOK")
                title_tn = new TreeNode(Path.GetFileNameWithoutExtension(this.ABBREVIATION));
            else
                title_tn = new TreeNode(Path.GetFileNameWithoutExtension(this.FileName));
            foreach (System.Xml.XmlNode note in notes)
			{
                title = FormatUtil.UnescapeXML( note.Attributes.GetNamedItem("title").Value);
                populateBook(note, title, title_tn.Nodes.Add(title));
			}
            if (title_tn.GetNodeCount(false) > 0)
            {
                treeview.Invoke(delegatedAdd, new object[] { title_tn });
            }
			if (treeview.Nodes.Count > 0)
			{
				Expand delegatedExpand = new Expand(treeview.Nodes[0].Expand);
				treeview.Invoke(delegatedExpand);
                string fullpath = "" ;

                TreeNode tmp = treeview.Nodes[0];
                while (tmp != null)
                {
                    fullpath = tmp.FullPath;
                    tmp = tmp.FirstNode;
                }
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
			errhtml.Append("</head><body><h2>No Book details found!</h2>");
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
                errhtml.Append("<p>If you think this is a valid Book format, please contact me using the contact page from " + GlobalMemory.AUTHOR_WEBSITE + " and I will try to investigate and fix that for you. The software is developed ");
                errhtml.Append("to make sure all bibles irrespective of any format, must be viewable and exportable. ");
                errhtml.Append("You can also visit <a href='https://"+GlobalMemory.AUTHOR_WEBSITE+"'>"+ GlobalMemory.AUTHOR_WEBSITE + "</a> to check if ");
                errhtml.Append("there is any new version of this software.</p>");
            }
			errhtml.Append("</body></html>");
			return errhtml.ToString();
		}

        //public string getExportErrorHtml(Exception exception)
        //{
        //    StringBuilder errhtml = new StringBuilder();
        //    errhtml.Append("<html><head>");
        //    errhtml.Append(BookFormat.CSS_English);
        //    errhtml.Append("</head><body><h2>Error Exporting Book</h2>");
        //    errhtml.Append("Error exporting \'");
        //    errhtml.Append(FileName);
        //    errhtml.Append("\'.");

        //    if (GlobalMemory.getInstance().DevMode)
        //    {
        //        errhtml.Append("<br><br>Message: ");
        //        errhtml.Append(exception.Message);
        //        int F;
        //        StackTrace S = new StackTrace(exception);
        //        errhtml.Append("<br><br>StackFrame:<br><pre>");
        //        for (F = 0; F <= S.FrameCount - 1; F++)
        //        {
        //            errhtml.Append("\r\n");
        //            errhtml.Append(S.GetFrame(F).ToString());
        //        }
        //        errhtml.Append("</pre>");
        //    }
        //    else
        //    {   
        //        errhtml.Append("<br><br><h4>So, what now?</h4>");
        //        errhtml.Append("<p>If you think the input is a valid Book format, ");
        //        errhtml.Append("just send me the book and the format specs (if available) to me at ");
        //        errhtml.Append(GlobalMemory.AUTHOR_EMAIL+" and I will try to fix that for you and see if it is a bug.</p>");
        //    }
        //    errhtml.Append("</body></html>");
        //    return errhtml.ToString();
        //}

        public static string postProcessContent(string input, string rtf_header = null)
        {
            string output = input;
            if (GlobalMemory.getInstance().ParseBook)
            {
                // prevent screensaver
                if (GlobalMemory.getInstance().UseCtrlForHtmlRtfConv)
                    Win32NativeCalls.preventScreensaver();

                if (GlobalMemory.getInstance().ConvertRtfToHtmlBook)
                {
                    output = FormatUtil.convertRtfToHtml(output);
                }
                else if (GlobalMemory.getInstance().autoSetParsingBook && BookFormat.getInstance().CLEANING)
                {
                    // force cleaning rtf to html if set to auto and format requires it.
                    output = FormatUtil.convertRtfToHtml(output);
                }

                if (GlobalMemory.getInstance().ConvertHtmlToRtfBook)
                {
                    output = FormatUtil.convertHtmlToRtf(output);
                }

                // finally strip html tags
                if (GlobalMemory.getInstance().stripHtmlTagsBook)
                {
                    output = FormatUtil.StripHTML(output);
                }

                // restore it back
                if (GlobalMemory.getInstance().UseCtrlForHtmlRtfConv)
                    Win32NativeCalls.restoreScreensaver();
            }
            return output;
        }

        public static string getFilters()
        {
            if (filters == null)
            {
                // INDEX MUST NOT CHANGE WHEN SAVE IS TRUE!!!

                StringBuilder sb = new StringBuilder();
                sb.Append("XML Books|*.xml|"); //idx = 1
                sb.Append("The Word Bible Books|*.gbk.twm|"); //idx = 2
                sb.Append("e-Sword Bible 8.x Topics|*.top|"); //idx = 3
                sb.Append("e-Sword Bible 9.x Topics / References|*.topx;*.refx|"); //idx = 4
                sb.Append("e-Sword Bible HD References|*.refi|"); //idx = 5
                sb.Append("SWORD Book Modules|*.zip|"); //idx = 6
                sb.Append("Bible Analyzer Book|*.bk|"); //idx = 7
                sb.Append("MySword for Android|*.bok.mybible.gz;*.bok.mybible|"); //idx = 8
                sb.Append("STEP Files|BOOK.DAT|"); //idx = 9               
                sb.Append("SwordSearcher/Forge for SwordSearcher|*.ss5book;*.txt"); //idx = 10
                filters =sb.ToString(); 
            }
            return filters;
        }

        public XmlDocument getXmlDoc()
        {
            return BookXmlDocument;
        }

		public abstract void Load();
		public abstract int PercentComplete {get;}
        public abstract int FilterIndex {get;}

        public abstract void ExportBook(string filename, int idx);
	}
	
}
