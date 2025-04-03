using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Simple_Bible_Reader
{
    class OpenSongBibleFormat : BibleFormat
    {
        System.Xml.XmlElement rootdoc;
        System.Xml.XmlElement bookdoc;
        System.Xml.XmlElement chapdoc;
        System.Xml.XmlElement versedoc;
        System.Xml.XmlAttribute attrib;
        public System.Windows.Forms.RichTextBox rtb = new System.Windows.Forms.RichTextBox();

        int book_id = 0;
        int total_books = 66;
        string _verse_text = "";
        public OpenSongBibleFormat(string file)
            : base(file)
		{
		}
		
		public override void Load()
		{

            BibleXmlDocument = new System.Xml.XmlDocument();
            XmlDeclaration dec = BibleXmlDocument.CreateXmlDeclaration("1.0", null, null);
            BibleXmlDocument.AppendChild(dec);
            rootdoc = BibleXmlDocument.CreateElement("XMLBIBLE");
            BibleXmlDocument.AppendChild(rootdoc);

            System.Xml.XmlDocument m_xmm_doc = new System.Xml.XmlDocument();
            m_xmm_doc.XmlResolver = null;            
            m_xmm_doc.Load(this.FileName);

            XmlNodeList bibles = m_xmm_doc.SelectNodes("/bible/b");
            XmlNodeList chaps = null;
            XmlNodeList vers = null;
            total_books = bibles.Count;
            foreach (XmlNode b in bibles)
            {
                book_id = FormatUtil.getBookNo(b.Attributes.GetNamedItem("n").Value);
                bookdoc = BibleXmlDocument.CreateElement("BIBLEBOOK");
                attrib = BibleXmlDocument.CreateAttribute("bnumber");
                attrib.Value = book_id.ToString();
                bookdoc.Attributes.Append(attrib);
                rootdoc.AppendChild(bookdoc);
                chaps = b.SelectNodes("c");
                foreach (XmlNode c in chaps)
                {
                    chapdoc = BibleXmlDocument.CreateElement("CHAPTER");
                    attrib = BibleXmlDocument.CreateAttribute("cnumber");
                    attrib.Value = c.Attributes.GetNamedItem("n").Value;
                    chapdoc.Attributes.Append(attrib);
                    bookdoc.AppendChild(chapdoc);
                    vers = c.SelectNodes("v");
                    foreach (XmlNode v in vers)
                    {
                        versedoc = BibleXmlDocument.CreateElement("VERS");
                        attrib = BibleXmlDocument.CreateAttribute("vnumber");
                        attrib.Value = v.Attributes.GetNamedItem("n").Value;
                        versedoc.Attributes.Append(attrib);
                        _verse_text = v.InnerText;

                        if (GlobalMemory.getInstance().ParseBible)
                        {
                            if (!GlobalMemory.getInstance().parseBibleBkgndCleaner)
                                _verse_text = postProcessContent(_verse_text);
                            versedoc.InnerText = _verse_text;
                        }
                        else
                            versedoc.InnerXml = FormatUtil.EscapeXML(_verse_text);

                        chapdoc.AppendChild(versedoc);
                    }
                }
            }
            m_xmm_doc = null;
			SetProcessAsComplete();
		}
		
		public override int PercentComplete
		{
			get
			{
                return book_id*100/ total_books;			
			}
		}

        public override int FilterIndex
        {
            get
            {
                return F17_OPENSONG;
            }
        }

        public override void ExportBible(string filename,int filter_idx)
        {
            XmlDocument m_xmm_doc_out = new System.Xml.XmlDocument();
            XmlDeclaration dec = m_xmm_doc_out.CreateXmlDeclaration("1.0", null, null);
            m_xmm_doc_out.AppendChild(dec);
            rootdoc = m_xmm_doc_out.CreateElement("bible");
            m_xmm_doc_out.AppendChild(rootdoc);
            XmlNode vers=null;
            total_books = 66;

            for (int book_id = 1; book_id <= 66; book_id++)
            {
                bookdoc = m_xmm_doc_out.CreateElement("b");
                attrib = m_xmm_doc_out.CreateAttribute("n");
                attrib.Value = Localization.getBookNames()[book_id - 1];
                bookdoc.Attributes.Append(attrib);
                rootdoc.AppendChild(bookdoc);
                for (int chap_id = 1; chap_id <= FormatUtil.GetChapterCount(book_id); chap_id++)
                {
                    chapdoc = m_xmm_doc_out.CreateElement("c");
                    attrib = m_xmm_doc_out.CreateAttribute("n");
                    attrib.Value = chap_id.ToString();
                    chapdoc.Attributes.Append(attrib);
                    bookdoc.AppendChild(chapdoc);
                    for (int vers_id = 1; vers_id <= FormatUtil.GetVerseCount(book_id, chap_id); vers_id++)
                    {
                        versedoc = m_xmm_doc_out.CreateElement("v");
                        attrib = m_xmm_doc_out.CreateAttribute("n");
                        attrib.Value = vers_id.ToString();
                        versedoc.Attributes.Append(attrib);
                        vers = BibleXmlDocument.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber='"+book_id.ToString()+"']/CHAPTER[@cnumber='"+chap_id.ToString()+"']/VERS[@vnumber='"+vers_id.ToString()+"']");
                        if (vers == null)
                        {
                            versedoc.InnerText = "";
                        }
                        else
                        {
                           versedoc.InnerText = vers.InnerText;
                        }
                        chapdoc.AppendChild(versedoc);
                    }
                }
            }
            if (!filename.EndsWith(".xmm"))
            {
                filename = filename + ".xmm";
            }
            if (filename.Equals(""))
            {
                return;
            }
            System.IO.File.WriteAllText(filename, PrintXML( m_xmm_doc_out.OuterXml), Encoding.UTF8);
            m_xmm_doc_out = null;
        }
    }
}
