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
	public class ZefaniaCommentaryFormat : CommentaryFormat
	{
        int percent_complete = 0;
        public ZefaniaCommentaryFormat(string file)
            : base(file)
		{
		}
		
		public override void Load()
		{
			try
			{
				CommentaryXmlDocument.Load(this.FileName);
                XmlNode abbr = CommentaryXmlDocument.SelectSingleNode("/XMLBIBLE/INFORMATION/identifier");
                XmlNode desc = CommentaryXmlDocument.SelectSingleNode("/XMLBIBLE/INFORMATION/description");
                if (abbr != null)
                    ABBREVIATION = abbr.InnerText;
                if (desc != null)
                    DESCRIPTION = desc.InnerText;
			}
			catch (Exception)
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
                return 1;
            }
        }

        public override void ExportCommentary(string filename, int filter_idx)
        {
            if (!filename.EndsWith(".xml"))
            {
                filename = filename + ".xml";
            }
            if (filename.Equals(""))
            {
                return;
            }
            XmlNodeList verses = CommentaryXmlDocument.SelectNodes("//VERS");
            int count = 0;
            foreach (XmlNode verse in verses)
            {
                percent_complete = (int)((count * 100) / verses.Count);
                verse.InnerText = SecurityElement.Escape(verse.InnerText);
                count++;
            }
            System.IO.File.WriteAllText(filename, PrintXML( XML), Encoding.UTF8);
        }
    }
	
}
