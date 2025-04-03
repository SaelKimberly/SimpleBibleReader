using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace Simple_Bible_Reader
{
    class VerseVIEWBibleFormat : BibleFormat
    {
        System.Xml.XmlElement rootdoc;
        System.Xml.XmlElement bookdoc;
        System.Xml.XmlElement chapdoc;
        System.Xml.XmlElement versedoc;
        System.Xml.XmlAttribute attrib;
        public System.Windows.Forms.RichTextBox rtb = new System.Windows.Forms.RichTextBox();

        int book_id = 0;
        int total_books = 66;
        
        String m_revision = "1";
        String m_font = "Arial,Helvetica,sans-serif,Calibri";
        String m_copyright = "Public Domain, God, for Eternity";
        String m_sizefactor = "1";

        public VerseVIEWBibleFormat(string file)
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
            string _verse_text = "";
            System.Xml.XmlDocument m_xmm_doc = new System.Xml.XmlDocument();
            m_xmm_doc.XmlResolver = null;            
            m_xmm_doc.Load(this.FileName);

            XmlNodeList bibles = m_xmm_doc.SelectNodes("/bible/b");

            bool book_names_used = false;
            XmlNode bn_node = m_xmm_doc.SelectSingleNode("/bible/booknames");
            XmlNode title_node = m_xmm_doc.SelectSingleNode("/bible/title");
            if (title_node != null)
            {
                DESCRIPTION = title_node.InnerText;
            }
            string[] book_names=null;
            if (bn_node != null)
            {
                book_names = bn_node.InnerText.Split(new char[] { ',' });
                book_names_used = true;
            }
            XmlNodeList chaps = null;
            XmlNodeList vers = null;
            total_books = bibles.Count;
            int book_name_count = 0;
            foreach (XmlNode b in bibles)
            {
                if (book_names_used)
                {
                    book_id = FormatUtil.getBookNo(book_names[book_name_count]);
                    book_name_count++;
                }
                else
                {
                    book_id = FormatUtil.getBookNo(b.Attributes.GetNamedItem("n").Value);
                }
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

                        if (GlobalMemory.getInstance().ParseBible)
                        {
                            if (!GlobalMemory.getInstance().parseBibleBkgndCleaner)
                                _verse_text = postProcessContent(v.InnerText);
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
                return F27_VERSEVIEW;
            }
        }

        public override List<string[]> getProperties()
        {
            if (kv_export.Count == 0)
            {
                kv_export.Add(new string[] { "Abbreviation:", BibleFormat.getInstance().ABBREVIATION });
                kv_export.Add(new string[] { "Description:", BibleFormat.getInstance().DESCRIPTION });
                kv_export.Add(new string[] { "Comments:", BibleFormat.getInstance().COMMENTS });
                kv_export.Add(new string[] { "Revision:", m_revision });
                kv_export.Add(new string[] { "Font:", m_font });
                kv_export.Add(new string[] { "Size Factor:", m_sizefactor});
                kv_export.Add(new string[] { "Copyright:", m_copyright });
            }
            return kv_export;
        }

        public override void setProperties(List<string[]> m_kv)
        {
            kv_export = m_kv;
        }
        private void processProperties()
        {
            for (int i = 0; i < kv_export.Count; i++)
            {
               
                
                switch (kv_export[i][0])
                {
                    case "Abbreviation:":
                        BibleFormat.getInstance().ABBREVIATION = kv_export[i][1];
                        break;
                    case "Description:":
                        BibleFormat.getInstance().DESCRIPTION = kv_export[i][1];
                        break;
                    case "Comments:":
                        BibleFormat.getInstance().COMMENTS = kv_export[i][1];
                        break;
                    case "Revision:":
                        m_revision = kv_export[i][1];
                        break;
                    case "Font:":
                        m_font = kv_export[i][1];
                        break;
                    case "Size Factor:":
                        m_sizefactor = kv_export[i][1];
                        break;
                    case "Copyright:":
                        m_copyright = kv_export[i][1];
                        break;
                }
            }
        }

        public override void ExportBible(string filename,int filter_idx)
        {
            XmlDocument m_xmm_doc_out = new System.Xml.XmlDocument();
            XmlDeclaration dec = m_xmm_doc_out.CreateXmlDeclaration("1.0", null, null);
            m_xmm_doc_out.AppendChild(dec);
            rootdoc = m_xmm_doc_out.CreateElement("bible");
            m_xmm_doc_out.AppendChild(rootdoc);

            XmlElement fname = m_xmm_doc_out.CreateElement("fname");
            XmlElement revision = m_xmm_doc_out.CreateElement("revision");
            XmlElement title = m_xmm_doc_out.CreateElement("title");
            XmlElement font = m_xmm_doc_out.CreateElement("font");
            XmlElement copyright = m_xmm_doc_out.CreateElement("copyright");
            XmlElement sizefactor = m_xmm_doc_out.CreateElement("sizefactor");

            processProperties();
            fname.InnerText = Path.GetFileName(filename);
            revision.InnerText = m_revision;
            title.InnerText = ABBREVIATION;
            font.InnerText = m_font;
            copyright.InnerText = m_copyright;
            sizefactor.InnerText = m_sizefactor;


            rootdoc.AppendChild(fname);
            rootdoc.AppendChild(revision);
            rootdoc.AppendChild(title);
            rootdoc.AppendChild(font);
            rootdoc.AppendChild(copyright);
            rootdoc.AppendChild(sizefactor);

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
            if (!filename.EndsWith(".xml"))
            {
                filename = filename + ".xml";
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
