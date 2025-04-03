using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;
using ICSharpCode.SharpZipLib.Zip;

namespace Simple_Bible_Reader
{
    class LOGOSBibleFormat : BibleFormat
    {
		System.Xml.XmlElement rootdoc;
		System.Xml.XmlElement bookdoc;
		System.Xml.XmlElement chapdoc;
		System.Xml.XmlElement versedoc;
		System.Xml.XmlAttribute attrib;
		
		int _book_no;
		int _chap_no;
		int _verse_no;
		string _verse_text;
		
		int p_book_no = -1;
		int p_chap_no = -1;
		int p_verse_no = -1;
        public System.Windows.Forms.RichTextBox rtb = new System.Windows.Forms.RichTextBox();
        int tmp_1 = 0;
        string[] data = null;

        public LOGOSBibleFormat(string file)
            : base(file)
		{
		}
		
		public override void Load()
		{
			//not supported
			//SetProcessAsComplete();
		}
		
		public override int PercentComplete
		{
			get
			{
				return ((_book_no) * 100) / 66;
			}
		}

        public override int FilterIndex
        {
            get
            {
                return F23_LOGOS;
            }
        }

        public override void ExportBible(string filename,int filter_idx)
        {
            if (!filename.EndsWith(".docx"))
            {
                filename = filename + ".docx";
            }
            if (filename.Equals(""))
            {
                return;
            }
            string _verse_text = "";
            
            string logos_doc_xml = Encoding.UTF8.GetString(Properties.Resources.logos_document_xml);
            logos_doc_xml = logos_doc_xml.Replace("@title", this.ABBREVIATION);

            XmlNodeList books = BibleXmlDocument.SelectNodes("/XMLBIBLE/BIBLEBOOK");
            XmlNode vers = null;
            StringBuilder doc_book_xml = new StringBuilder();

            for (int b = 1; b <= 66; b++)
            {
                _book_no = b;
                doc_book_xml.Append("<w:p w:rsidR=\"0073553E\" w:rsidRPr=\"00853ED5\" w:rsidRDefault=\"00CB5105\" w:rsidP=\"00853ED5\"><w:pPr><w:pStyle w:val=\"Heading1\"/></w:pPr><w:r w:rsidRPr=\"00853ED5\"><w:t>");
                doc_book_xml.Append(Localization.getBookNames()[_book_no-1]);
                doc_book_xml.Append("</w:t></w:r></w:p>");
                for (int c = 1; c <= FormatUtil.GetChapterCount(b); c++)
                {
                    doc_book_xml.Append("<w:p w:rsidR=\"00CB5105\" w:rsidRPr=\"00853ED5\" w:rsidRDefault=\"00CB5105\" w:rsidP=\"00853ED5\"><w:pPr><w:pStyle w:val=\"Heading2\"/></w:pPr><w:r w:rsidRPr=\"00853ED5\"><w:t>");
                    doc_book_xml.Append("Chapter "+c.ToString());
                    doc_book_xml.Append("</w:t></w:r></w:p>");
                    for (int v = 1; v <= FormatUtil.GetVerseCount(b, c); v++)
                    {
                        vers = BibleXmlDocument.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber='" + b.ToString() + "']/CHAPTER[@cnumber='" + c.ToString() + "']/VERS[@vnumber='" + v.ToString() + "']");
                        if (vers != null)
                        {
                            _verse_text = processStrongVerse(vers, b);                            
                            doc_book_xml.Append("<w:p w:rsidR=\"006C48C3\" w:rsidRPr=\"003E1806\" w:rsidRDefault=\"00E50994\" w:rsidP=\"001618AF\"><w:r><w:t xml:space=\"preserve\">");
                            doc_book_xml.Append("[[@Bible:" + FormatUtil.logosBookNames[b - 1] + " " + c.ToString() + ":" + v.ToString() + " ]][[" + c.ToString() + ":" + v.ToString() + " &gt;&gt; " + FormatUtil.logosBookNames[b - 1] + " " + c.ToString() + ":" + v.ToString() + "]] {{field-on:Bible}} " + _verse_text + " {{field-off:Bible}}");
                            doc_book_xml.Append("</w:t></w:r></w:p>");                            
                        }
                    }
                }
            }
            logos_doc_xml = logos_doc_xml.Replace("@bookxml", doc_book_xml.ToString());
            createDocx(logos_doc_xml, filename);
        }

        public static void createDocx(String document_xml, String outfile)
        {
            String tmp_path = System.IO.Path.GetTempPath();
            Random rand = new Random((int) DateTime.Now.Ticks & 0x0000FFFF);
            if (!tmp_path.EndsWith("\\"))
                tmp_path = tmp_path + "\\";
            tmp_path = tmp_path + rand.Next(int.MaxValue).ToString("x") + "\\";
            //
            FastZip zip = new FastZip();
            zip.ExtractZip(new MemoryStream(Properties.Resources.logos4_docx), tmp_path, FastZip.Overwrite.Always, confirmDelegate, ".*", ".*", true, true);

            File.WriteAllText(tmp_path + "word\\document.xml", document_xml);
            zip.CreateZip(outfile, tmp_path, true, ".*");
            Directory.Delete(tmp_path, true);
        }

        public static FastZip.ConfirmOverwriteDelegate confirmDelegate { get; set; }

        private string processStrongVerse(XmlNode vers, int b)
        {
            string prefix = "G";
            string _verse_text = "";
            if (b < 40)
                prefix = "H";
            if (GlobalMemory.getInstance().ParseBible)
            {
                _verse_text = vers.InnerXml;
                
                if (_verse_text.IndexOf("<gr") != -1)
                {
                    _verse_text = Regex.Replace(_verse_text, "(<gr )([^s]*)(str=['\"]*)([^<'\"]*)['\"]*(>)(((?!</gr).)*)</gr>", @"$6 <" + prefix + @"$4>", RegexOptions.IgnoreCase);
                }
            }
            else
                _verse_text = vers.InnerText;
            return _verse_text;
        }
    }
}
