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
	public class XMLBookFormat : BookFormat
	{
        int percent_complete = 0;
        public System.Windows.Forms.RichTextBox rtb = new System.Windows.Forms.RichTextBox();

		public XMLBookFormat(string file) : base(file)
		{
            
		}
		
		public override void Load()
		{
			try
			{
                BookXmlDocument.Load(this.FileName);
            }
			catch (Exception ex)
			{
                Themes.MessageBox(ex.ToString());
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
                return 1;
            }
        }

        public override void ExportBook(string filename,int filter_idx)
        {
            if (!filename.EndsWith(".xml"))
            {
                filename = filename + ".xml";
            }
            if (filename.Equals(""))
            {
                return;
            }
            XmlNodeList verses = BookXmlDocument.SelectNodes("//NOTES");
            int count = 0;
            foreach (XmlNode verse in verses)
            {
                percent_complete = (int)((count * 100) / verses.Count);
                verse.InnerText = SecurityElement.Escape(verse.InnerText);
                count++;
            }
            System.IO.File.WriteAllText(filename, PrintXML(XML), Encoding.UTF8);
        }
    }
	
}
