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
	public abstract class DictionaryFormat
	{

        static string filters = null;
        public string ABBREVIATION = "DICT";
        public string DESCRIPTION = "Dictionary";
        public string COMMENTS = "Exported from Simple Bible Reader (" + GlobalMemory.AUTHOR_WEBSITE +")";
        public string STRONG = "0";

		private string m_filename;
		
		private static System.Xml.XmlDocument xmldoc = new System.Xml.XmlDocument();
				
		private bool process_complete = false;

        public System.Windows.Forms.RichTextBox rtb = new System.Windows.Forms.RichTextBox();
        public string rtf_header = @"{\rtf1\ansi\ansicpg1252\deff0\deflang1033{\fonttbl{\f1\fnil\fprq2\fcharset161 TITUS Cyberbit Basic;}{\f2\fnil\fprq2\fcharset177 TITUS Cyberbit Basic;}{\f3\fnil\fprq2\fcharset0 TITUS Cyberbit Basic;}{\f4\fnil\fprq2\fcharset136 PMingLiU;}{\f5\fnil\fprq2\fcharset204 Georgia;}{\f6\fnil\fprq2\fcharset238 Georgia;}{\f7\fnil\fprq2\fcharset134 PMingLiU;}{\f8\fnil\fprq2\fcharset222 Cordia New;}{\f9\fnil\fprq2\fcharset129 Gulim;}{\f10\fnil\fprq2\fcharset178 TITUS Cyberbit Basic;}{\f11\fnil\fprq2\fcharset162 Georgia;}{\f12\fnil\fprq2\fcharset163 Times New Roman;}{\f13\fnil\fprq2\fcharset128 MS Mincho;}}{\colortbl ;\red0\green0\blue0;\red0\green0\blue255;\red0\green255\blue255;\red0\green255\blue0;\red255\green0\blue255;\red255\green0\blue0;\red255\green255\blue0;\red255\green255\blue255;\red0\green0\blue128;\red0\green128\blue128;\red0\green128\blue0;\red128\green0\blue128;\red128\green0\blue0;\red128\green128\blue0;\red128\green128\blue128;\red192\green192\blue192;}{\viewkind4\uc1\pard\plain\cf0\fs\par}";
	
		public static DictionaryFormat dictionary_format;

        public bool CLEANING = false;

		public DictionaryFormat(string filename)
		{
			m_filename = filename;
			process_complete = false;
		}

        public static DictionaryFormat getInstance()
        {
            return dictionary_format;
        }

        public static DictionaryFormat DictionaryFactory(string filename)
        {
            dictionary_format = null;
            if (filename.ToLower().EndsWith(".xml"))
            {
                string xml_str = System.IO.File.ReadAllText(filename).ToLower();
                if (FormatUtil.InStr(xml_str, "<dictionary"))
                {
                    xml_str = null;
                    dictionary_format = new ZefaniaDictionaryFormat(filename);
                }
            }
            else if (filename.ToLower().EndsWith(".dctx"))
            {
                dictionary_format = new DCTXDictionaryFormat(filename);
            }
            else if (filename.ToLower().EndsWith(".dcti"))
            {
                dictionary_format = new DCTIDictionaryFormat(filename);
            }
            else if (filename.ToLower().EndsWith(".dct"))
            {
                FileStream fs = System.IO.File.OpenRead(filename);
                int b = fs.ReadByte();
                fs.Close();
                if (b == 0x53) // Checking for S in Sqlite 
                {
                    dictionary_format = new BibleAnalyzerDictionaryFormat(filename);
                }
                else
                    dictionary_format = new DCTDictionaryFormat(filename);
            }
            else if (filename.ToLower().EndsWith(".dct.twm"))
            {
                dictionary_format = new TheWordDictionaryFormat(filename);
            }
            else if (filename.ToLower().EndsWith(".zip"))
            {
                dictionary_format = new SwordDictionaryFormat(filename);
            }
            else if (filename.ToLower().EndsWith(".dct.mybible.gz"))
            {
                dictionary_format = new MySword4AndroidDictionaryFormat(filename, true);
            }
            else if (filename.ToLower().EndsWith(".dct.mybible"))
            {
                dictionary_format = new MySword4AndroidDictionaryFormat(filename, false);
            }
            else if (filename.ToLower().EndsWith(".dictionary.sqlite3"))
            {
                dictionary_format = new MyBibleDictionaryFormat(filename);
            }
            else if (filename.ToLower().EndsWith(".ss5book"))
            {
                dictionary_format = new SwordSearcherDictionaryFormat(filename);
            }
            else if (filename.ToLower().EndsWith(".txt"))
            {
                StreamReader sr = File.OpenText(filename);
                string line = sr.ReadLine();
                sr.Close();
                if (line.StartsWith(";"))
                    dictionary_format = new SwordSearcherDictionaryFormat(filename);
                else
                {
                    // any other commentary in text format. nothing yet.
                }
            }
            if (dictionary_format != null)
            {
                if (cleaningRequired(dictionary_format.FilterIndex))
                {
                    dictionary_format.CLEANING = true;
                }
            if (dictionary_format.ABBREVIATION == "DICT")
                dictionary_format.ABBREVIATION = Path.GetFileNameWithoutExtension(filename);
            if (dictionary_format.DESCRIPTION == "Dictionary")
                dictionary_format.DESCRIPTION = Path.GetFileNameWithoutExtension(filename);
            }
            return dictionary_format;
        }

        public static DictionaryFormat DictionaryFactory(string filename, int filterIdx)
        {
            if (filterIdx == 0)
                dictionary_format = DictionaryFactory(filename);
            else
                dictionary_format = getDictionaryFormat(filename, filterIdx);
            return dictionary_format;
        }

        public static DictionaryFormat getDictionaryFormat(string filename, int filterIdx)
        {
            DictionaryFormat dictionary_format_export = null;
            switch (filterIdx)
            {
                case 1:
                    dictionary_format_export = new ZefaniaDictionaryFormat(filename);
                    break;
                case 2:
                    dictionary_format_export = new DCTDictionaryFormat(filename);
                    break;
                case 3:
                    dictionary_format_export = new DCTXDictionaryFormat(filename);
                    break;
                case 4:
                    dictionary_format_export = new DCTIDictionaryFormat(filename);
                    break;
                case 5:
                    dictionary_format_export = new TheWordDictionaryFormat(filename);
                    break;
                case 6:
                    dictionary_format_export = new SwordDictionaryFormat(filename);
                    break;
                case 7:
                    dictionary_format_export = new MySword4AndroidDictionaryFormat(filename, true);
                    break;
                case 8:
                    dictionary_format_export = new BibleAnalyzerDictionaryFormat(filename);
                    break;
                case 9:
                    dictionary_format_export = new MyBibleDictionaryFormat(filename);
                    break;
                case 10:
                    dictionary_format_export = new SwordSearcherDictionaryFormat(filename);
                    break;
                default:
                    if (!GlobalMemory.getInstance().ConsoleMode)
                        Themes.MessageBox("Sorry, export for this Bible format is not yet implemented!");
                    else
                        Console.WriteLine("Sorry, export for this Bible format is not yet implemented!");
                    return null;
            }
            return dictionary_format_export;
        }

        public static DictionaryFormat doExportDictionary(string filename, int filterIdx)
        {
            DictionaryFormat dictionary_format_export = getDictionaryFormat(filename, filterIdx);
            if (dictionary_format_export != null)
            {
                dictionary_format_export.CLEANING = dictionary_format.CLEANING;
                dictionary_format_export.ABBREVIATION = dictionary_format.ABBREVIATION.Replace("'", "''");
                dictionary_format_export.DESCRIPTION = dictionary_format.DESCRIPTION.Replace("'", "''");
                dictionary_format_export.COMMENTS = dictionary_format.COMMENTS.Replace("'", "''");

                if (dictionary_format_export.ABBREVIATION == "DICT")
                    dictionary_format_export.ABBREVIATION = Path.GetFileNameWithoutExtension(filename).Replace("'", "''");
                if (dictionary_format_export.DESCRIPTION == "Dictionary")
                    dictionary_format_export.DESCRIPTION = Path.GetFileNameWithoutExtension(filename).Replace("'", "''");
            }
            return dictionary_format_export;
        }

        public static bool cleaningRequired(int FilterIdx)
        {
            int[] required = new int[] { 2,3,4 };
            for (int i = 0; i < required.Length; i++)
                if (required[i] == FilterIdx)
                    return true;            
            return false;
        }

        public string getFirstDictItem()
        {
            XmlNode v = xmldoc.SelectSingleNode("/dictionary/item[1]");
            if (v != null)
                return v.InnerText;
            else
                return "";
        }

		public void SetProcessAsComplete()
		{
			process_complete = true;
		}		
		
		public static System.Xml.XmlDocument DictionaryXmlDocument
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

		public void populateTopics(ListBox list)
		{
            Clear delegatedClear = new Clear(list.Items.Clear);
            AddItem delegatedAdd = new AddItem(list.Items.Add);
            list.Invoke(delegatedClear, new object[] { });
            XmlNodeList items = DictionaryXmlDocument.SelectNodes("/dictionary/item");
            foreach (XmlNode item in items)
            {
                list.Invoke(delegatedAdd, new object[] { item.Attributes["id"].Value });                
            }
		}        

        private string getTopicHtml(string dictionary_text, XmlNode child)
        {
            //Console.WriteLine("\t"+child.Name);
            string tmp = null;
            if (child.Name == "description")
            {
                /*
                if (cleaningRequired(dictionary_format.FilterIndex))
                {
                    rtb.Rtf = rtf_header + child.InnerText;
                    dictionary_text = dictionary_text + "<p>" + rtb.Text + "</p><br>";
                }
                else
                {
                 */
                if (child.HasChildNodes)
                {
                    foreach (XmlNode c in child.ChildNodes)
                    {
                        dictionary_text = getTopicHtml(dictionary_text, c);
                    }
                }
                else
                {
                    tmp = child.InnerText;
                    if (tmp.Contains("<a"))
                    {
                        // anchor tag exists.
                        tmp = Regex.Replace(tmp, @"href=""([^""]+)""([^>]*)>([^<]+)</a>", "OnClick=\"chrome.webview.postMessage('$1');\"$2>$3</a>", RegexOptions.IgnoreCase);
                        tmp = Regex.Replace(tmp, @"href='([^']+)'([^>]*)>([^<]+)</a>", "OnClick=\"chrome.webview.postMessage('$1');\"$2>$3</a>", RegexOptions.IgnoreCase);
                    }
                    dictionary_text = dictionary_text + tmp;
                }
                //}
            }
            else if (child.Name == "title")
            {
                if (dictionary_text != "")
                    dictionary_text = dictionary_text + "<br><br><b>" + child.InnerText + "</b><br>";
                else
                    dictionary_text = dictionary_text + "<b>" + child.InnerText + "</b><br>";
            }
            else if (child.Name == "see")
            {
                if (child.Attributes["target"].Value == "x-self")
                    dictionary_text = dictionary_text + "<a OnClick=\"chrome.webview.postMessage('D" + child.InnerText + "');\">" + child.InnerText + "</a><br>";
                else
                    dictionary_text = dictionary_text + "<a OnClick=\"chrome.webview.postMessage('" + child.Attributes["target"].Value + "');\">" + child.InnerText + "</a><br>";
            }
            else if (child.Name == "reflink")
            {
                if (child.Attributes["target"] != null)
                {
                    dictionary_text = dictionary_text + "<a OnClick=\"chrome.webview.postMessage('B" + child.Attributes["target"].Value + "');\">" + child.InnerText + "</a>";
                }
                else if (child.Attributes["mscope"] != null)
                {
                    string[] arr = child.Attributes["mscope"].Value.Split(';');
                    dictionary_text = dictionary_text + "<a OnClick=\"chrome.webview.postMessage('B" + child.Attributes["mscope"].Value + "');\">" + FormatUtil.shortBookNames[int.Parse(arr[0]) - 1] + " " + arr[1] + ":" + arr[2] + "</a>";
                }
            }
            else if (child.Name == "#text")
            {
                tmp = child.InnerText;
                if (tmp.Contains("<a"))
                {
                    // anchor tag exists.
                    tmp = Regex.Replace(tmp, @"href=""([^""]+)""([^>]*)>([^<]+)</a>", "OnClick=\"chrome.webview.postMessage('$1');\"$2>$3</a>", RegexOptions.IgnoreCase);
                    tmp = Regex.Replace(tmp, @"href='([^']+)'([^>]*)>([^<]+)</a>", "OnClick=\"chrome.webview.postMessage('$1');\"$2>$3</a>", RegexOptions.IgnoreCase);
                }
                dictionary_text = dictionary_text + tmp;
            }
            return FormatUtil.UnescapeXML(dictionary_text);
        }

        public virtual string getTopicRaw(string dictionary_text, XmlNode child)
        {
            if (child.Value != null)
            {
                if (child.Value != "")
                    dictionary_text = dictionary_text + "<br/><textarea rows='10' readonly='readonly'>" + FormatUtil.UnescapeXML(FormatUtil.UnescapeXML(child.Value)) + "</textarea><br/>";                
            }
            if (child.HasChildNodes)
            {
                foreach (XmlNode c in child.ChildNodes)
                {
                    dictionary_text = getTopicRaw(dictionary_text, c);
                }
            }
            return dictionary_text;
        }

        public string getTopicRaw(string id)
        {
            string dictionary_text = "<html dir=\'ltr\'><head>" + Themes.CSS_MAIN + "</head><body>";
 
            XmlNodeList items = DictionaryXmlDocument.SelectNodes("/dictionary/item[@id=\"" + id + "\"]");
            XmlDocument doc_tmp = new XmlDocument();
            foreach (XmlNode item in items)
            {
                foreach (XmlNode child in item.ChildNodes)
                {
                    dictionary_text = getTopicRaw(dictionary_text, child);
                }
            }            
            dictionary_text = dictionary_text + "</body></html>";

            return dictionary_text;
        }


        public string getTopicHtml(string id)
        {
            if (GlobalMemory.getInstance().DisplayDCTRaw)
                return getTopicRaw(id);
            string dictionary_text = "";
            XmlNodeList items = DictionaryXmlDocument.SelectNodes("/dictionary/item[@id=\"" + id + "\"]");
            XmlDocument doc_tmp = new XmlDocument();
            foreach (XmlNode item in items)
            {
                foreach (XmlNode child in item.ChildNodes)
                {
                    dictionary_text = getTopicHtml(dictionary_text,child);
                }
            }
            dictionary_text = dictionary_text.Replace("\n", "<br>");
            dictionary_text = "<html dir=\'ltr\'><head><title>"+DESCRIPTION+"</title>" + Themes.CSS_DICTIONARY + "</head><body>" + dictionary_text + "</body></html>";            
            return dictionary_text;
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
            errhtml.Append("</head><body><h2>No Dictionary details found!</h2>");
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
                errhtml.Append("<p>If you think this is a valid dictionary format, please contact me using the contact page from " + GlobalMemory.AUTHOR_WEBSITE + " and I will try to investigate and fix that for you. The software is developed ");
                errhtml.Append("to make sure all dictionary irrespective of any format, must be viewable and exportable. ");
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
            errhtml.Append("</head><body><h2>Error Exporting Dictionary</h2>");
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
                errhtml.Append("<p>If you think the input is a valid dictionary format, please contact me using the contact page from " + GlobalMemory.AUTHOR_WEBSITE + " and I will try to investigate and fix that for you.</p>");
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
                sb.Append("Zefania XML Dictionaries|*.xml|"); //idx = 1    
                sb.Append("e-Sword 8.x Dictionary Modules|*.dct|"); //idx = 2
                sb.Append("e-Sword 9.x and above Dictionary Modules|*.dctx|"); //idx = 3
                sb.Append("e-Sword HD Dictionary Modules|*.dcti|"); //idx = 4   
                sb.Append("The Word Dictionary Modules|*.dct.twm|"); //idx = 5   
                sb.Append("SWORD Dictionary Modules|*.zip|"); //idx = 6     
                sb.Append("MySword for Android|*.dct.mybible.gz;*.dct.mybible|"); //idx = 7
                sb.Append("Bible Analyzer Dictionary|*.dct|"); //idx = 8
                sb.Append("MyBible Dictionary|*.dictionary.SQLite3;*.dictionary.sqlite3|"); //idx = 9
                sb.Append("SwordSearcher/Forge for SwordSearcher|*.ss5book;*.txt"); //idx = 10
                filters =sb.ToString();
            }
            return filters;
        }

        public static string postProcessContent(string input, string rtf_header = null)
        {
            string output = input;
            if (GlobalMemory.getInstance().ParseDictionary)
            {
                // prevent screensaver
                if (GlobalMemory.getInstance().UseCtrlForHtmlRtfConv)
                    Win32NativeCalls.preventScreensaver();

                if (GlobalMemory.getInstance().ConvertRtfToHtmlDictionary)
                {
                    output = FormatUtil.convertRtfToHtml(output);
                }
                if (GlobalMemory.getInstance().ConvertHtmlToRtfDictionary)
                {
                    output = FormatUtil.convertHtmlToRtf(output);
                }

                // finally strip html tags
                if (GlobalMemory.getInstance().stripHtmlTagsDictionary)
                {
                    output = FormatUtil.StripHTML(output);
                }

                // restore it back
                if (GlobalMemory.getInstance().UseCtrlForHtmlRtfConv)
                    Win32NativeCalls.restoreScreensaver();
            }
            return output;
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
            catch (XmlException)
            {
            }

            mStream.Close();
            writer.Close();

            return Result;
        }

        public XmlDocument getXmlDoc()
        {
            return DictionaryXmlDocument;
        }

		public abstract void Load();
		public abstract int PercentComplete {get;}
        public abstract int FilterIndex {get;}

        public abstract void ExportDictionary(string filename, int idx);
	}
	
}
