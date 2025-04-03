using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;

using System;
using System.Collections;
using System.Windows.Forms;
using System.Xml;
using System.Text;
using System.Security;


namespace Simple_Bible_Reader
{
	public class ZefaniaBibleFormat : BibleFormat
	{
        int percent_complete = 0;
        public System.Windows.Forms.RichTextBox rtb = new System.Windows.Forms.RichTextBox();

		public ZefaniaBibleFormat(string file) : base(file)
		{
		}
		
		public override void Load()
		{
			try
			{
				BibleXmlDocument.Load(this.FileName);
                XmlNode abbr = BibleXmlDocument.SelectSingleNode("/XMLBIBLE/INFORMATION/identifier");
                XmlNode desc = BibleXmlDocument.SelectSingleNode("/XMLBIBLE/INFORMATION/description");
                if (abbr != null)
                    ABBREVIATION = abbr.InnerText;
                if (desc != null)
                    DESCRIPTION = desc.InnerText;

                //
                XmlNodeList books = BibleXmlDocument.SelectNodes("/XMLBIBLE/BIBLEBOOK");
                XmlNode bnumber = null;
                int m_bnumber = -1;
                foreach(XmlNode book in books)
                {
                    bnumber = book.Attributes.GetNamedItem("bnumber");
                    if(bnumber != null)
                    {
                        if(int.TryParse(bnumber.Value,out m_bnumber))
                        {
                            if(m_bnumber > 900)
                            {
                                m_bnumber = m_bnumber - 811;
                                bnumber.Value = m_bnumber.ToString();                                
                            }
                        }
                    }
                }
            }
			catch (Exception e)
			{
                
			}
			SetProcessAsComplete();
		}
		
		public override int PercentComplete
		{
			get
			{
				if (IsComplete)
				{
					return 100;
				}
				else
				{
                    return percent_complete;
				}
			}
		}

        public override int FilterIndex
        {
            get
            {
                return F02_ZEFANIA_BIBLE;
            }
        }

        public override void ExportBible(string filename,int filter_idx)
        {
            if (!filename.EndsWith(".xml"))
            {
                filename = filename + ".xml";
            }
            if (filename.Equals(""))
            {
                return;
            }
            /*
            XmlNodeList verses = BibleXmlDocument.SelectNodes("//VERS");
            int count = 0;
            foreach (XmlNode verse in verses)
            {
                percent_complete = (int)((count * 100) / verses.Count);
                verse.InnerText = SecurityElement.Escape(verse.InnerText);
                count++;
            }
            */

            //
            XmlNodeList books = BibleXmlDocument.SelectNodes("/XMLBIBLE/BIBLEBOOK");
            XmlNode bnumber = null;
            int m_bnumber = -1;
            foreach (XmlNode book in books)
            {
                bnumber = book.Attributes.GetNamedItem("bnumber");
                if (bnumber != null)
                {
                    if (int.TryParse(bnumber.Value, out m_bnumber))
                    {
                        if (m_bnumber > 89)
                        {
                            m_bnumber = m_bnumber + 811;
                            bnumber.Value = m_bnumber.ToString();
                        }
                    }
                }

            }

            System.IO.File.WriteAllText(filename, BibleXmlDocument.OuterXml, Encoding.UTF8);
        }
    }
	
}
