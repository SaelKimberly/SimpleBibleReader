using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;

using System;
using System.Collections;
using System.Windows.Forms;
using System.Xml;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;


namespace Simple_Bible_Reader
{
	public class XSEMBibleFormat : BibleFormat
	{

        public System.Windows.Forms.RichTextBox rtb = new System.Windows.Forms.RichTextBox();
		string[] book_names = new string[] {"GEN", "EXO", "LEV", "NUM", "DEU", "JOS", "JDG", "RUT", "1SA", "2SA", "1KI", "2KI", "1CH", "2CH", "EZR", "NEH", "EST", "JOB", "PSA", "PRO", "ECC", "SNG", "ISA", "JER", "LAM", "EZK", "DAN", "HOS", "JOL", "AMO", "OBA", "JON", "MIC", "NAM", "HAB", "ZEP", "HAG", "ZEC", "MAL", "MAT", "MRK", "LUK", "JHN", "ACT", "ROM", "1CO", "2CO", "GAL", "EPH", "PHP", "COL", "1TH", "2TH", "1TI", "2TI", "TIT", "PHM", "HEB", "JAS", "1PE", "2PE", "1JN", "2JN", "3JN", "JUD", "REV"};
		int percent_complete = 0;
		
		public XSEMBibleFormat(string file) : base(file)
		{
		}
		
		public override void Load()
		{
			System.IO.StreamReader reader;
			//reader = System.IO.File.OpenText((string) FileName);
            reader = new StreamReader(new FileStream(this.FileName, FileMode.Open), System.Text.Encoding.Default);
			string xsem_text = reader.ReadToEnd();
			
			percent_complete = 0;
			xsem_text = xsem_text.Replace("</book>", "</BIBLEBOOK>"); //</book>
			percent_complete = 10;
			xsem_text = Regex.Replace(xsem_text, "(<)(book )([^<]*)(value=)([^<]*)(>)", "<BIBLEBOOK bnumber=$5>"); //<book>
			percent_complete = 20;
			//slow
			xsem_text = Regex.Replace(xsem_text, "<verseEnd[^<]*/>", "</VERS>"); //</verse>
			percent_complete = 30;
			xsem_text = Regex.Replace(xsem_text, "(<)(verse )([^<]*)(value=)([^<]*)(/>)", "<VERS vnumber=$5>"); //<verse>
			percent_complete = 40;
			xsem_text = Regex.Replace(xsem_text, "<chapterEnd[^<]*/>", "</CHAPTER>"); //</chapter
			percent_complete = 50;
			xsem_text = Regex.Replace(xsem_text, "(<)(chapter )([^<]*)(value=)([^<]*)(/>)", "<CHAPTER cnumber=$5>"); //<chapter>
			percent_complete = 60;
			//strip all tags
			xsem_text = Regex.Replace(xsem_text, "</?(?!((/?)BIBLEBOOK|(/?)CHAPTER|(/?)VERS))[^>]*>", "");
			percent_complete = 70;
			xsem_text = "<XMLBIBLE>" + xsem_text + "</XMLBIBLE>";
			percent_complete = 80;
			
			for (int i = 1; i <= 66; i++)
			{
				percent_complete = System.Convert.ToInt32(80 + (i * 10) / 66);
				xsem_text = xsem_text.Replace("\"" + book_names[i - 1] + "\"", "\"" + i.ToString() + "\"");
			}
			percent_complete = 90;
			try
			{
				BibleXmlDocument.LoadXml(xsem_text);
			}
			catch (Exception)
			{
				//Fix errors (very slow)
				xsem_text = Regex.Replace(xsem_text, "(<)(VERS[^<]*)(>)([^(</)]*)(<VERS)", "$1$2$3$4</VERS>$5", RegexOptions.Singleline);
				try
				{
					BibleXmlDocument.LoadXml(xsem_text);
				}
				catch (Exception)
				{
				}
			}
			percent_complete = 100;
			SetProcessAsComplete();
		}
		
		public override int PercentComplete
		{
			get
			{
				return percent_complete;
			}
		}

        public override int FilterIndex
        {
            get
            {
                return F07_XSEM;
            }
        }

        public override void ExportBible(string filename, int filter_idx)
        {
            System.Xml.XmlElement rootdoc;
            System.Xml.XmlElement shdr;
            System.Xml.XmlAttribute attrib;
            System.Xml.XmlAttribute attrib1;

            if (!filename.EndsWith(".xml"))
            {
                filename = filename + ".xml";
            }
            if (filename.Equals(""))
            {
                return;
            }

            XmlDocument xsem_out = new System.Xml.XmlDocument();
            XmlDeclaration dec = xsem_out.CreateXmlDeclaration("1.0", null, null);
            xsem_out.AppendChild(dec);
            rootdoc = xsem_out.CreateElement("scripture");
            xsem_out.AppendChild(rootdoc);

            shdr = xsem_out.CreateElement("scriptureHeader");
            shdr.InnerXml = Properties.Resources.xsem_scripture_header;
            rootdoc.AppendChild(shdr);

            XmlNode vers = null;
            XmlElement canon = null;
            XmlElement book = null;
            XmlElement chapter = null;
            XmlElement verse = null;
            XmlText verseTxt = null;

            bool chap_present = false;
            bool verse_present = false;
            percent_complete = 0;
            bool ot_cannon_done = false;
            bool nt_cannon_done = false;
            string _verse_text = "";
            for (int book_id = 1; book_id <= 66; book_id++)
            {
                percent_complete = book_id * 100 / 66;

                if (book_id < 40 && !ot_cannon_done)
                {
                    canon = xsem_out.CreateElement("canon");
                    attrib = xsem_out.CreateAttribute("id");
                    attrib.Value = "OT";
                    attrib1 = xsem_out.CreateAttribute("value");
                    attrib1.Value = "OT";
                    canon.Attributes.Append(attrib);
                    canon.Attributes.Append(attrib1);
                    rootdoc.AppendChild(canon);
                }
                else if (book_id >= 40 && book_id<=66 && !nt_cannon_done)
                {
                    canon = xsem_out.CreateElement("canon");
                    attrib = xsem_out.CreateAttribute("id");
                    attrib.Value = "NT";
                    attrib1 = xsem_out.CreateAttribute("value");
                    attrib1.Value = "NT";
                    canon.Attributes.Append(attrib);
                    canon.Attributes.Append(attrib1);
                    rootdoc.AppendChild(canon);
                }
                else if (book_id > 66)
                {
                    continue;
                }

                book = xsem_out.CreateElement("book");
                attrib = xsem_out.CreateAttribute("value");
                attrib.Value = book_names[book_id - 1];
                book.Attributes.Append(attrib);
                canon.AppendChild(book);
                chap_present = false;
                for (int chap_id = 1; chap_id <= FormatUtil.GetChapterCount(book_id); chap_id++)
                {
                    chapter = xsem_out.CreateElement("chapter");
                    attrib = xsem_out.CreateAttribute("value");
                    attrib.Value = chap_id.ToString();
                    chapter.Attributes.Append(attrib);
                    book.AppendChild(chapter);
                    verse_present = false;
                    for (int vers_id = 1; vers_id <= FormatUtil.GetVerseCount(book_id, chap_id); vers_id++)
                    {
                        vers = BibleXmlDocument.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber='" + book_id.ToString() + "']/CHAPTER[@cnumber='" + chap_id.ToString() + "']/VERS[@vnumber='" + vers_id.ToString() + "']");
                        if (vers != null)
                        {
                            verse = xsem_out.CreateElement("verse");
                            attrib = xsem_out.CreateAttribute("value");
                            attrib.Value = vers_id.ToString();
                            verse.Attributes.Append(attrib);
                            book.AppendChild(verse);
                            //
                            _verse_text = vers.InnerText;                            

                            verseTxt = xsem_out.CreateTextNode(_verse_text);
                            book.AppendChild(verseTxt);
                            //
                            book.AppendChild(xsem_out.CreateElement("verseEnd"));
                            verse_present = true;
                        }
                    }
                    if (!verse_present)
                        book.RemoveChild(chapter);
                    else
                    {
                        book.AppendChild(xsem_out.CreateElement("chapterEnd"));
                        chap_present = true;
                    }
                }
                if (!chap_present)
                    canon.RemoveChild(book);
            }
            percent_complete = 100;
            System.IO.File.WriteAllText(filename, PrintXML( xsem_out.OuterXml), Encoding.UTF8);
            xsem_out = null;
        }
    }
	
}
