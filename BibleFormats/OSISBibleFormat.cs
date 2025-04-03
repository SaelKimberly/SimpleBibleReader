using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;

using System;
using System.Collections;
using System.Windows.Forms;
using System.Xml;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;


namespace Simple_Bible_Reader
{
	public class OSISBibleFormat : BibleFormat
	{

        public System.Windows.Forms.RichTextBox rtb = new System.Windows.Forms.RichTextBox();
		
		string[] osisBookNames;
		
		System.Xml.XmlElement rootdoc;
		System.Xml.XmlElement bookdoc;
		System.Xml.XmlElement chapdoc;
		System.Xml.XmlElement versedoc;
		System.Xml.XmlAttribute attrib;
		
		int _book_no;
		int _chap_no;
		int _verse_no;
		string _verse_text;
		string[] _osisID;
		
		int p_book_no = -1;
		int p_chap_no = -1;
		int p_verse_no = -1;
        
        String m_subject = "Holy Bible";
        String m_rights = "Scripture is a fact. Facts remain as facts even after translation. Facts cannot be copyrighted. Hence, all scriptures are on Public Domain.";


        public OSISBibleFormat(string file) : base(file)
		{
		}
		
		public override void Load()
		{
			BibleXmlDocument = new System.Xml.XmlDocument();
			XmlDeclaration dec = BibleXmlDocument.CreateXmlDeclaration("1.0", null, null);
			BibleXmlDocument.AppendChild(dec);
			rootdoc = BibleXmlDocument.CreateElement("XMLBIBLE");
			BibleXmlDocument.AppendChild(rootdoc);
			
			//System.IO.StreamReader file_reader = System.IO.File.OpenText((string) FileName);
            System.IO.StreamReader file_reader = new StreamReader(new FileStream(this.FileName, FileMode.Open), System.Text.Encoding.Default);
			string strxml = file_reader.ReadToEnd();
			file_reader.Close();
			
			XmlDocument m_xmldoc = new XmlDocument();
			m_xmldoc.LoadXml(strxml);

            XmlNodeList _books = FormatUtil.BookHelperDoc.SelectNodes("/books/book/osisID");
			osisBookNames = new string[_books.Count + 1];
			int i = 0;
			foreach (XmlNode _book in _books)
			{
				osisBookNames[i] = _book.InnerText;
				i++;
			}
			
			XmlNodeList verses = null;
			
			
			verses = m_xmldoc.SelectNodes("//div[@type=\'book\']/chapter/verse");
			if (verses.Count == 0)
			{
				XmlNamespaceManager nsmgr = new XmlNamespaceManager(m_xmldoc.NameTable);
				nsmgr.AddNamespace("o", "http://www.bibletechnologies.net/2003/OSIS/namespace");
				verses = m_xmldoc.SelectNodes("//o:div[@type=\'book\']/o:chapter/o:verse", nsmgr);
			}
			
			//it may not be a proper xml
			//it could be a nightmare parsing this stuff.. let's go just one more try..
			
			strxml = Regex.Replace(strxml, "<verse([^>]*)sID([^>]*)/>", "<verse$1sID$2>");
			strxml = Regex.Replace(strxml, "<verse([^>]*)eID([^>]*)/>", "</verse>");
			strxml = Regex.Replace(strxml, "</?(?!((/?)verse))[^>]*>", "");
			strxml = "<xml>" + strxml + "</xml>";
			m_xmldoc = null;
			m_xmldoc = new System.Xml.XmlDocument();
			m_xmldoc.LoadXml(strxml);
			
			verses = m_xmldoc.SelectNodes("//verse");
			
			if (verses.Count == 0)
			{
				XmlNamespaceManager nsmgr = new XmlNamespaceManager(m_xmldoc.NameTable);
				nsmgr.AddNamespace("o", "http://www.bibletechnologies.net/2003/OSIS/namespace");
				verses = m_xmldoc.SelectNodes("//o:verse", nsmgr);
			}
			
			if (verses.Count == 0)
			{
				//the bible format cannot be processed.
				return;
			}
			
			foreach (XmlNode verse in verses)
			{
				processLine(verse);
			}
			m_xmldoc = null;
			SetProcessAsComplete();
		}
		
		
		private void processLine(XmlNode verse)
		{
			if (verse.Attributes.GetNamedItem("osisID") == null)
			{
				return;
			}
			_osisID = verse.Attributes.GetNamedItem("osisID").Value.Split(".".ToCharArray());
			_book_no = System.Convert.ToInt32(Array.IndexOf(osisBookNames, _osisID[0]) + 1);
			_chap_no = int.Parse(_osisID[1]);
			_verse_no = int.Parse(_osisID[2]);
			_verse_text = verse.InnerText;
			
			if (_book_no != p_book_no || (_chap_no == 1 && _verse_no == 1 && p_verse_no >= _verse_no))
			{
				bookdoc = BibleXmlDocument.CreateElement("BIBLEBOOK");
				attrib = BibleXmlDocument.CreateAttribute("bnumber");
				attrib.Value = _book_no.ToString();
				bookdoc.Attributes.Append(attrib);
				rootdoc.AppendChild(bookdoc);
			}
			if (_chap_no != p_chap_no || (_chap_no == 1 && _verse_no == 1 && p_verse_no >= _verse_no))
			{
				chapdoc = BibleXmlDocument.CreateElement("CHAPTER");
				attrib = BibleXmlDocument.CreateAttribute("cnumber");
				attrib.Value = _chap_no.ToString();
				chapdoc.Attributes.Append(attrib);
				bookdoc.AppendChild(chapdoc);
			}
			if (_verse_no != p_verse_no || (_verse_no == p_verse_no && _chap_no != p_chap_no) || (_verse_no == p_verse_no && _book_no != p_book_no))
			{
				versedoc = BibleXmlDocument.CreateElement("VERS");
				attrib = BibleXmlDocument.CreateAttribute("vnumber");
				attrib.Value = _verse_no.ToString();
				if (GlobalMemory.getInstance().ParseBible)
				{
					if (!GlobalMemory.getInstance().parseBibleBkgndCleaner)
						_verse_text = postProcessContent(_verse_text);
					versedoc.InnerText = _verse_text;
				}
                else
                    versedoc.InnerXml = FormatUtil.EscapeXML(_verse_text);

                versedoc.Attributes.Append(attrib);
				chapdoc.AppendChild(versedoc);
			}
			
			p_book_no = _book_no;
			p_chap_no = _chap_no;
			p_verse_no = _verse_no;
			
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
                return F05_OSIS;
            }
        }


        public override List<string[]> getProperties()
        {
            if (kv_export.Count == 0)
            {
                kv_export.Add(new string[] { "Abbreviation:", BibleFormat.getInstance().ABBREVIATION });
                kv_export.Add(new string[] { "Description:", BibleFormat.getInstance().DESCRIPTION });
                kv_export.Add(new string[] { "Comments:", BibleFormat.getInstance().COMMENTS });
                kv_export.Add(new string[] { "Subject:", m_subject });
                kv_export.Add(new string[] { "Rights:", m_rights });
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
                    case "Subject:":
                        m_subject = kv_export[i][1];
                        break;
                    case "Rights:":
                        m_rights = kv_export[i][1];
                        break;
                }
            }
        }



        public override void ExportBible(string filename, int filter_idx)
        {
            if (!filename.EndsWith(".xml"))
            {
                filename = filename + ".xml";
            }
            if (filename.Equals(""))
            {
                return;
            }
            processProperties();
            XmlDocument osis_out = new System.Xml.XmlDocument();
            XmlDeclaration dec = osis_out.CreateXmlDeclaration("1.0", null, null);
            osis_out.AppendChild(dec);
            rootdoc = osis_out.CreateElement("osis", "http://www.bibletechnologies.net/2003/OSIS/namespace");
            osis_out.AppendChild(rootdoc);
            //----osisText
            XmlElement osisText = osis_out.CreateElement("osisText");
            attrib = osis_out.CreateAttribute("osisRefWork");
            attrib.Value = "Bible";
            osisText.Attributes.Append(attrib);
            XmlAttribute attrib1 = osis_out.CreateAttribute("osisIDWork");
            attrib1.Value = Path.GetFileNameWithoutExtension(filename);
            osisText.Attributes.Append(attrib1);
            rootdoc.AppendChild(osisText);
            //---header
                XmlElement header = osis_out.CreateElement("header");
                XmlElement revisionDesc = osis_out.CreateElement("revisionDesc");
                XmlElement date = osis_out.CreateElement("date");
                date.InnerText = String.Format("{0:yyyy-MM-dd}", new DateTime(System.DateTime.Today.Ticks));
                XmlElement date_p = osis_out.CreateElement("p");
                date_p.InnerText = COMMENTS;
                revisionDesc.AppendChild(date);
                revisionDesc.AppendChild(date_p);
                header.AppendChild(revisionDesc);

                XmlElement work = osis_out.CreateElement("work");
                        
                XmlElement title = osis_out.CreateElement("title");
                title.InnerText = ABBREVIATION;
                work.AppendChild(title);
                  
                work.AppendChild(osis_out.CreateElement("contributor"));
                work.AppendChild(osis_out.CreateElement("creator"));

                XmlElement subject = osis_out.CreateElement("subject");
                subject.InnerText = m_subject;
                work.AppendChild(subject);

                XmlElement date_h = osis_out.CreateElement("date");
                date_h.InnerText = String.Format("{0:yyyy-MM-dd}", new DateTime(System.DateTime.Today.Ticks));
                work.AppendChild(date_h);

                XmlElement description_h = osis_out.CreateElement("description");
                description_h.InnerText = DESCRIPTION;
                work.AppendChild(description_h);
                
                work.AppendChild(osis_out.CreateElement("publisher"));

                XmlElement type = osis_out.CreateElement("type");
                attrib = osis_out.CreateAttribute("type");
                attrib.Value = "OSIS";
                type.Attributes.Append(attrib);
                type.InnerText = "Bible";
                work.AppendChild(type);

                XmlElement identifier = osis_out.CreateElement("identifier");
                attrib = osis_out.CreateAttribute("type");
                attrib.Value = "OSIS";
                identifier.Attributes.Append(attrib);
                identifier.InnerText = Path.GetFileNameWithoutExtension(filename);
                work.AppendChild(identifier);

                XmlElement rights = osis_out.CreateElement("rights");
                rights.InnerText = m_rights;
                work.AppendChild(rights);

                XmlElement refSystem = osis_out.CreateElement("refSystem");
                refSystem.InnerText = "Bible";
                work.AppendChild(refSystem);

                header.AppendChild(work);
                osisText.AppendChild(header);                
            //---
                XmlNodeList _books = FormatUtil.BookHelperDoc.SelectNodes("/books/book/osisID");
                osisBookNames = new string[_books.Count + 1];
                int i = 0;
                foreach (XmlNode _book in _books)
                {
                    osisBookNames[i] = _book.InnerText;
                    i++;
                }


            XmlNode vers = null;
            XmlElement div_book = null;
            XmlElement chapter = null;
            XmlElement verse = null;
			string redtext = null;
          
            for (int book_id = 1; book_id <= 66; book_id++)
            {
                _book_no = book_id;
                div_book = osis_out.CreateElement("div");
                attrib = osis_out.CreateAttribute("type");
                attrib.Value = "book";
                div_book.Attributes.Append(attrib);
                attrib1 = osis_out.CreateAttribute("osisID");
                attrib1.Value = osisBookNames[book_id - 1];
                div_book.Attributes.Append(attrib1);
                osisText.AppendChild(div_book);

                for (int chap_id = 1; chap_id <= FormatUtil.GetChapterCount(book_id); chap_id++)
                {                    
                    chapter = osis_out.CreateElement("chapter");
                    attrib = osis_out.CreateAttribute("osisID");
                    attrib.Value = osisBookNames[book_id - 1] + "." + chap_id.ToString();
                    chapter.Attributes.Append(attrib);                    
                    div_book.AppendChild(chapter);
                    for (int vers_id = 1; vers_id <= FormatUtil.GetVerseCount(book_id, chap_id); vers_id++)
                    {                        
                        vers = BibleXmlDocument.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber='" + book_id.ToString() + "']/CHAPTER[@cnumber='" + chap_id.ToString() + "']/VERS[@vnumber='" + vers_id.ToString() + "']");
                        if (vers != null)
                        {
                            verse = osis_out.CreateElement("verse");
                            attrib = osis_out.CreateAttribute("osisID");
                            attrib.Value = osisBookNames[book_id - 1] + "." + chap_id.ToString() + "." + vers_id.ToString();
                            verse.Attributes.Append(attrib);
							//verse.InnerText = vers.InnerText;
							redtext = vers.InnerXml;
							redtext = redtext.Replace("<STYLE css=\"color:#ff0000\">", "<q who=\"Jesus\" marker=\"\">");
							redtext = redtext.Replace("</STYLE>", "</q>");
							verse.InnerXml = redtext;
							chapter.AppendChild(verse);
                        }                        
                    }
                    if (chapter.ChildNodes.Count == 0)
                        div_book.RemoveChild(chapter);
                }
                if (div_book.ChildNodes.Count == 0)
                    osisText.RemoveChild(div_book);                
            }    
        
            System.IO.File.WriteAllText(filename, PrintXML( osis_out.OuterXml),Encoding.UTF8);
            osis_out = null;
        }
    }
	
}
