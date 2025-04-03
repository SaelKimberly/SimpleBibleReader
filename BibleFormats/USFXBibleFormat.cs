using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;

using System;
using System.Collections;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;
using System.Text;


namespace Simple_Bible_Reader
{
	public class USFXBibleFormat : BibleFormat
	{
        public System.Windows.Forms.RichTextBox rtb = new System.Windows.Forms.RichTextBox();
		
		string[] book_names = new string[] {"GEN", "EXO", "LEV", "NUM", "DEU", "JOS", "JDG", "RUT", "1SA", "2SA", "1KI", "2KI", "1CH", "2CH", "EZR", "NEH", "EST", "JOB", "PSA", "PRO", "ECC", "SNG", "ISA", "JER", "LAM", "EZK", "DAN", "HOS", "JOL", "AMO", "OBA", "JON", "MIC", "NAM", "HAB", "ZEP", "HAG", "ZEC", "MAL", "MAT", "MRK", "LUK", "JHN", "ACT", "ROM", "1CO", "2CO", "GAL", "EPH", "PHP", "COL", "1TH", "2TH", "1TI", "2TI", "TIT", "PHM", "HEB", "JAS", "1PE", "2PE", "1JN", "2JN", "3JN", "JUD", "REV"};
		System.Xml.XmlElement rootdoc;
		System.Xml.XmlElement bookdoc;
		System.Xml.XmlAttribute attrib;
		
		int percent_complete = 0;
		
		public USFXBibleFormat(string file) : base(file)
		{
		}
		
		public override void Load()
		{
			BibleXmlDocument = new System.Xml.XmlDocument();
			XmlDeclaration dec = BibleXmlDocument.CreateXmlDeclaration("1.0", null, null);
			BibleXmlDocument.AppendChild(dec);
			rootdoc = BibleXmlDocument.CreateElement("XMLBIBLE");
			BibleXmlDocument.AppendChild(rootdoc);
			System.Xml.XmlDocument m_usfx_doc = new System.Xml.XmlDocument();
			m_usfx_doc.XmlResolver = null;
			m_usfx_doc.Load((string) FileName);
			int book_id = -1;
			System.Xml.XmlNodeList books = m_usfx_doc.SelectNodes("//book");
			
			foreach (XmlNode book in books)
			{
				if (book.NodeType != XmlNodeType.Element)
				{
					continue;
				}
				if (!(book.Attributes.GetNamedItem("id").Value == "FRT" || book.Attributes.GetNamedItem("id").Value == "BAK"))
				{
					book_id = System.Convert.ToInt32(Array.IndexOf(book_names, book.Attributes.GetNamedItem("id").Value) + 1);
					if (book_id == -1)
					{
						continue;
					}
					percent_complete = book_id * 100 / 66;
					bookdoc = BibleXmlDocument.CreateElement("BIBLEBOOK");
					attrib = BibleXmlDocument.CreateAttribute("bnumber");
					attrib.Value = book_id.ToString();
					bookdoc.Attributes.Append(attrib);
					rootdoc.AppendChild(bookdoc);
					//
					string strXml = FormatBookXML(book.InnerXml);
					try
					{
						bookdoc.InnerXml = strXml;
					}
					catch (Exception)
					{
					}
				}
			}
			percent_complete = 100;
			SetProcessAsComplete();
		}
		
		private string FormatBookXML(string usfx_text)
		{
			usfx_text = Regex.Replace(usfx_text, "(<)(v )([^<]*)(id=)([^<]*)(/>)", "<VERS vnumber=$5>"); //<verse>
			usfx_text = Regex.Replace(usfx_text, "(<)(c )([^<]*)(id=)([^<]*)(/>)", "<CHAPTER cnumber=$5>"); //<chapter>
			//strip all tags
			usfx_text = Regex.Replace(usfx_text, "</?(?!((/?)CHAPTER|(/?)VERS))[^>]*>", "");
			//
			usfx_text = usfx_text.Replace("<VERS", "</VERS><VERS");
			usfx_text = usfx_text.Replace("<CHAPTER", "</VERS></CHAPTER><CHAPTER");
			
			usfx_text = usfx_text.Substring(System.Convert.ToInt32(usfx_text.IndexOf("</VERS></CHAPTER>") + "</VERS></CHAPTER>".Length));
			usfx_text = Regex.Replace(usfx_text, "<CHAPTER([^>]*)>([^>]*)</VERS>", "<CHAPTER$1>");
			
			usfx_text = usfx_text + "</VERS></CHAPTER>";
			return usfx_text;
		}
		
		public override int PercentComplete
		{
			get
			{
                if (percent_complete < 0)
                    return 0;
                else
				    return percent_complete;
			}
		}


        public override int FilterIndex
        {
            get
            {
				return F29_USFX;
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

            XmlDocument usfx_out = new System.Xml.XmlDocument();
            XmlDeclaration dec = usfx_out.CreateXmlDeclaration("1.0", null, null);
            usfx_out.AppendChild(dec);
            rootdoc = usfx_out.CreateElement("usfx", "http://eBible.org/usfx/usfx-2009-07-12.xsd");
            usfx_out.AppendChild(rootdoc);                             


            XmlNode vers = null;
            XmlElement book = null;
            XmlElement chapter = null;
            XmlElement verse = null;
            XmlText verseTxt = null;

            bool chap_present = false;
            bool verse_present = false;
            percent_complete = 0;
            string _verse_text = "";
            for (int book_id = 1; book_id <= 66; book_id++)
            {
                percent_complete = book_id * 100 / 66;
                book = usfx_out.CreateElement("book");
                attrib = usfx_out.CreateAttribute("id");
                attrib.Value = book_names[book_id-1];
                book.Attributes.Append(attrib);
                rootdoc.AppendChild(book);
                chap_present = false;
                for (int chap_id = 1; chap_id <= FormatUtil.GetChapterCount(book_id); chap_id++)
                {
                    chapter = usfx_out.CreateElement("c");
                    attrib = usfx_out.CreateAttribute("id");
                    attrib.Value = chap_id.ToString();
                    chapter.Attributes.Append(attrib);
                    book.AppendChild(chapter);
                    verse_present = false;
                    for (int vers_id = 1; vers_id <= FormatUtil.GetVerseCount(book_id, chap_id); vers_id++)
                    {
                        vers = BibleXmlDocument.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber='" + book_id.ToString() + "']/CHAPTER[@cnumber='" + chap_id.ToString() + "']/VERS[@vnumber='" + vers_id.ToString() + "']");
                        if (vers != null)
                        {
                            verse = usfx_out.CreateElement("v");
                            attrib = usfx_out.CreateAttribute("id");
                            attrib.Value = vers_id.ToString();
                            verse.Attributes.Append(attrib);                            
                            book.AppendChild(verse);
                            //
                            _verse_text = vers.InnerText;
                            verseTxt = usfx_out.CreateTextNode(_verse_text);
                            book.AppendChild(verseTxt);
                            verse_present = true;
                        }
                    }
                    if (!verse_present)
                        book.RemoveChild(chapter);
                    else
                        chap_present = true;
                }
                if (!chap_present)
                    rootdoc.RemoveChild(book);
            }
            percent_complete = 100;
            System.IO.File.WriteAllText(filename,PrintXML( usfx_out.OuterXml), Encoding.UTF8);
            usfx_out = null;
        }

    }
	
}
