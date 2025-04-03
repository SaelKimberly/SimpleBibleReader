using System.Xml;
using System.Text.RegularExpressions;
using System;
using System.Collections;
using System.Text;

namespace Simple_Bible_Reader
{
    class ThMLBibleFormat: BibleFormat
    {

        System.Xml.XmlElement rootdoc;
        System.Xml.XmlElement bookdoc;
        System.Xml.XmlElement chapdoc;
        System.Xml.XmlElement versedoc;
        System.Xml.XmlAttribute attrib;
        System.Xml.XmlAttribute attrib1;

        System.Xml.XmlDocument tmp_doc = null;
        System.Xml.XmlNodeList tmp_vers = null;

        string[] thml_book_ids = new string[] { "Gen","Exod","Lev","Num","Deut","Josh","Judg","Ruth","iSam","iiSam","iKgs","iiKgs","iChr","iiChr","Ezra","Neh","Esth","Job","Ps","Prov","Eccl","Song","Isa","Jer","Lam","Ezek","Dan","Hos","Joel","Amos","Obad","Jonah","Mic","Nah","Hab","Zeph","Hag","Zech","Mal","Matt","Mark","Luke","John","Acts","Rom","iCor","iiCor","Gal","Eph","Phil","Col","iThess","iiThess","iTim","iiTim","Titus","Phlm","Heb","Jas","iPet","iiPet","iJohn","iiJohn","iiiJohn","Jude","Rev"};

        int book_id = 0;
        int total_books = 66;
        public System.Windows.Forms.RichTextBox rtb = new System.Windows.Forms.RichTextBox();
        string _verse_text = "";
        public ThMLBibleFormat(string file)
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

            System.Xml.XmlDocument m_thml_doc = new System.Xml.XmlDocument();
            m_thml_doc.XmlResolver = null;
            m_thml_doc.Load((string)FileName);           
            int chap_id = -1;
            
            System.Xml.XmlNodeList books = m_thml_doc.SelectNodes("//div2");
            System.Xml.XmlNodeList chaps = null;

            total_books = books.Count;
            int i = -1;

            ArrayList ids = new ArrayList();
            for (i = 0; i < thml_book_ids.Length; i++)
                ids.Add(thml_book_ids[i].ToLower());


            foreach (XmlNode book in books)
            {
                if (book.NodeType != XmlNodeType.Element)
                {
                    continue;
                }
                book_id = ids.IndexOf(book.Attributes.GetNamedItem("id").Value.ToLower())+1;
                if (book_id == 0)
                    continue;
                bookdoc = BibleXmlDocument.CreateElement("BIBLEBOOK");
                attrib = BibleXmlDocument.CreateAttribute("bnumber");
                attrib.Value = book_id.ToString();

                attrib1 = BibleXmlDocument.CreateAttribute("bname");                    
                attrib1.Value = book.Attributes.GetNamedItem("title").Value;

                bookdoc.Attributes.Append(attrib);
                bookdoc.Attributes.Append(attrib1);
                rootdoc.AppendChild(bookdoc);
                //
                chaps = book.SelectNodes("div3");
                chap_id = 1;
                foreach (XmlNode chap in chaps)
                {
                    if (chap.NodeType != XmlNodeType.Element)
                    {
                        continue;
                    }
                    chapdoc = BibleXmlDocument.CreateElement("CHAPTER");
                    attrib = BibleXmlDocument.CreateAttribute("cnumber");
                    attrib.Value = chap_id.ToString();
                    chapdoc.Attributes.Append(attrib);
                    FormatXML(chap);
                    bookdoc.AppendChild(chapdoc);
                    //
                    //                        
                    chap_id++;
                }
            }

            SetProcessAsComplete();
		}

        private void FormatXML(XmlNode chap)
        {
            string xml = chap.InnerXml;
            xml = xml.Replace("<sup>", "<!--");
            xml = xml.Replace("</sup>", "-->");
            xml = Regex.Replace(xml, "</?(?!((/?)scripture))[^>]*>", "");
            xml = Regex.Replace(xml, "<scripture([^>]*)/>", "</VERS><VERS>");
            xml = "<XML><VERS>" + xml + "</VERS></XML>";
            tmp_doc = new System.Xml.XmlDocument();
            tmp_doc.LoadXml(xml);
            tmp_vers = tmp_doc.SelectNodes("/XML/VERS");
            for (int i = 1; i < tmp_vers.Count - 1; i++)
            {
                versedoc = BibleXmlDocument.CreateElement("VERS");
                attrib = BibleXmlDocument.CreateAttribute("vnumber");
                attrib.Value = i.ToString();
                versedoc.Attributes.Append(attrib);
                if (GlobalMemory.getInstance().ParseBible)
                {
                    _verse_text = tmp_vers.Item(i).InnerText;
                    if (!GlobalMemory.getInstance().parseBibleBkgndCleaner)
                        _verse_text = postProcessContent(_verse_text);
                    versedoc.InnerText = _verse_text;
                }
                else
                    versedoc.InnerXml = FormatUtil.EscapeXML(_verse_text);
                chapdoc.AppendChild(versedoc);
            }
        }
		
		public override int PercentComplete
		{
			get
			{
				return book_id*100/total_books;
			}
		}

        public override int FilterIndex
        {
            get
            {
                return F16_THML;
            }
        }

        public override void ExportBible(string filename, int filter_idx)
        {
            XmlDocument thml_out = new System.Xml.XmlDocument();
            XmlDeclaration dec = thml_out.CreateXmlDeclaration("1.0", null, null);
            thml_out.AppendChild(dec);
            XmlElement rootdoc_thml = thml_out.CreateElement("ThML");
            thml_out.AppendChild(rootdoc_thml);

            XmlElement rootdoc_body = thml_out.CreateElement("ThML.body");
            rootdoc_thml.AppendChild(rootdoc_body);

            rootdoc = thml_out.CreateElement("div1");
            rootdoc_body.AppendChild(rootdoc);

            XmlNode vers = null;
            XmlElement p = null;
            total_books = 66;
            XmlNode bname = null;
            XmlNode tmp_bname = null;

            bool verse_present = false;
            bool chapters_present = false;
            string _verse_text = "";
            for (int book_id = 1; book_id <= 66; book_id++)
            {
                this.book_id = book_id;
                bookdoc = thml_out.CreateElement("div2");
                // title
                attrib = thml_out.CreateAttribute("title");

                if ((tmp_bname=BibleXmlDocument.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber='" + book_id + "']")) != null)
                {
                    bname = tmp_bname.Attributes.GetNamedItem("id");
                }

                if (bname == null)
                    attrib.Value = Localization.getBookNames()[book_id - 1];
                else
                {
                    attrib.Value = bname.Value;
                    bname = null;
                }
                // id
                attrib1 = thml_out.CreateAttribute("id");
                attrib1.Value = thml_book_ids[book_id - 1];
                bookdoc.Attributes.Append(attrib);
                bookdoc.Attributes.Append(attrib1);
                rootdoc.AppendChild(bookdoc);
                chapters_present = false;
                for (int chap_id = 1; chap_id <= FormatUtil.GetChapterCount(book_id); chap_id++)
                {
                    chapdoc = thml_out.CreateElement("div3");
                    attrib = thml_out.CreateAttribute("title");
                    attrib.Value = "Chapter " + chap_id.ToString();
                    chapdoc.Attributes.Append(attrib);
                    bookdoc.AppendChild(chapdoc);

                    p = thml_out.CreateElement("p");
                    chapdoc.AppendChild(p);
                    verse_present = false;
                    for (int vers_id = 1; vers_id <= FormatUtil.GetVerseCount(book_id, chap_id); vers_id++)
                    {
                        vers = BibleXmlDocument.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber='" + book_id.ToString() + "']/CHAPTER[@cnumber='" + chap_id.ToString() + "']/VERS[@vnumber='" + vers_id.ToString() + "']");
                        if (vers != null)
                        {
                            p.AppendChild(thml_out.CreateElement("scripture"));
                            _verse_text = vers.InnerText;
                            p.AppendChild(thml_out.CreateTextNode(_verse_text));
                            verse_present = true;
                        }
                    }
                    if (!verse_present)
                        bookdoc.RemoveChild(chapdoc);
                    else
                        chapters_present = true;
                }
                if (!chapters_present)
                    rootdoc.RemoveChild(bookdoc);                
            }
            if (!filename.EndsWith(".xml"))
            {
                filename = filename + ".xml";
            }
            if (filename.Equals(""))
            {
                return;
            }
            System.IO.File.WriteAllText(filename, PrintXML( thml_out.OuterXml), Encoding.UTF8);
            thml_out = null;
        }
    }
}
