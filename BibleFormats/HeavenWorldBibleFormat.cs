using System;
using System.Windows.Forms;
using System.Xml;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;
using Ionic.Zlib;
using System.Reflection;
using HBB;
using System.Diagnostics;


namespace Simple_Bible_Reader
{
	public class HeavenWorldBibleFormat : BibleFormat
	{
        byte[] hbb_header_av1611 = new byte[] 
        {
            0x01, 0x08, 0x00, 0x02, 0x00, 0x08, 0x01, 0x00, 0x7E, 0x79, 0x0E, 0x9F, 0x43, 0x0B, 0x32, 0x00, 0x00, 0x00, 0x41, 0x56, 0x20, 0x31, 0x36, 0x31, 0x31, 0x00, 0x41, 0x75, 0x74, 0x68, 0x6F, 0x72, 0x69, 0x7A, 0x65, 0x64, 0x20, 0x56, 0x65, 0x72, 0x73, 0x69, 0x6F, 0x6E, 0x20, 0x31, 0x36, 0x31, 0x31, 0x00, 0x82, 0x00, 0x00, 0x00, 0x7E, 0xE6, 0x01, 0x00, 0xA1, 0x7E, 0x43, 0x00, 0x9D, 0x71, 0x44, 0x00, 0xDF, 0x45, 0x71, 0x00, 0xDF, 0x45, 0x71, 0x00, 0xDF, 0x45, 0x71, 0x00, 0xDF, 0x45, 0x71, 0x00, 0xDF, 0x45, 0x71, 0x00, 0xDF, 0x45, 0x71, 0x00, 0xDF, 0x45, 0x71, 0x00, 0xDF, 0x45, 0x71, 0x00, 0xDF, 0x45, 0x71, 0x00, 0xDF, 0x45, 0x71, 0x00, 0xDF, 0x45, 0x71, 0x00, 0xDF, 0x45, 0x71, 0x00, 0xDF, 0x45, 0x71, 0x00, 0xDF, 0x45, 0x71, 0x00, 0xDF, 0x45, 0x71, 0x00, 0xDF, 0x45, 0x71, 0x00, 0x00, 0x00, 0x00, 0x00
        };
		
		System.Xml.XmlElement rootdoc;
		System.Xml.XmlElement bookdoc;
		System.Xml.XmlElement chapdoc;
		System.Xml.XmlElement versedoc;
		System.Xml.XmlAttribute attrib;
		
		int percent_complete = 0;

		public static readonly int Type_ONT = 0;
		public static readonly int Type_OT = 1;
		public static readonly int Type_NT = 2;
	
		
		public HeavenWorldBibleFormat(string file) : base(file)
		{
		}
		
		public override void Load()
		{
            string line = null;
            int begin_book = -1;
            int end_book = -1;

            HeavenWorld hw = new HeavenWorld(FileName);            

            if (hw.iVerseSystem == Type_ONT)
            {
                begin_book = 1;
                end_book = 66;
            }
            else if (hw.iVerseSystem == Type_OT)
            {
                begin_book = 1;
                end_book = 39;
            }
            else if (hw.iVerseSystem == Type_NT)
            {
                begin_book = 40;
                end_book = 66;
            }
            try
            {
                BibleXmlDocument = new System.Xml.XmlDocument();
                XmlDeclaration dec = BibleXmlDocument.CreateXmlDeclaration("1.0", null, null);
                BibleXmlDocument.AppendChild(dec);
                rootdoc = BibleXmlDocument.CreateElement("XMLBIBLE");
                BibleXmlDocument.AppendChild(rootdoc);

                for (int i = begin_book; i <= end_book; i++)
                {
                    percent_complete = System.Convert.ToInt32((i * 100) / (end_book - begin_book));
                    bookdoc = BibleXmlDocument.CreateElement("BIBLEBOOK");
                    attrib = BibleXmlDocument.CreateAttribute("bnumber");
                    attrib.Value = i.ToString();
                    bookdoc.Attributes.Append(attrib);
                    rootdoc.AppendChild(bookdoc);
                    for (int j = 1; j <= FormatUtil.GetChapterCount(i); j++)
                    {
                        chapdoc = BibleXmlDocument.CreateElement("CHAPTER");
                        attrib = BibleXmlDocument.CreateAttribute("cnumber");
                        attrib.Value = j.ToString();
                        chapdoc.Attributes.Append(attrib);
                        bookdoc.AppendChild(chapdoc);
                        for (int k = 1; k <= FormatUtil.GetVerseCount(i, j); k++)
                        {
                            line = hw.getVerseCache(i, j, k);
                            
                            versedoc = BibleXmlDocument.CreateElement("VERS");
                            attrib = BibleXmlDocument.CreateAttribute("vnumber");
                            attrib.Value = k.ToString();
                            if (GlobalMemory.getInstance().ParseBible)
                            {
                                line = line.Replace("<B>", "").Replace("</B>", "").Replace("<I>", "").Replace("</I>", "");
                                if (!GlobalMemory.getInstance().parseBibleBkgndCleaner)
                                    line = postProcessContent(line);
                                versedoc.InnerText = line;
                            }
                            else
                                versedoc.InnerXml = FormatUtil.EscapeXML(line);
                            versedoc.Attributes.Append(attrib);
                            chapdoc.AppendChild(versedoc);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Themes.DarkMessageBoxShow(ex.Message);
            }
            
			percent_complete = 100;
			SetProcessAsComplete();
		}

		public override int PercentComplete
		{
			get
			{
				if (percent_complete > 100)
				{
					percent_complete = 100;
				}
				return percent_complete;
			}
		}

        public override int FilterIndex
        {
            get
            {
                return F26_HEAVENWORLD;
            }
        }

        public override void ExportBible(string filename,int filter_idx)
        {
            if (File.Exists(Path.GetDirectoryName(filename) + "\\AV1611.HBB"))
                File.Delete(Path.GetDirectoryName(filename) + "\\AV1611.HBB");
            BinaryWriter bw = new BinaryWriter(new FileStream(Path.GetDirectoryName(filename) + "\\AV1611.HBB", FileMode.CreateNew));
            bw.Write(hbb_header_av1611);

            XmlNodeList books = BibleXmlDocument.SelectNodes("/XMLBIBLE/BIBLEBOOK");
            XmlNode vers = null;
            StringBuilder sb=new StringBuilder();
            string verse_text=null;
            int size=0;
            for (int b = 1; b <= 66; b++)
            {
                percent_complete = System.Convert.ToInt32((b * 100) / 66);
                for (int c = 1; c <= FormatUtil.GetChapterCount(b); c++)
                    for (int v = 1; v <= FormatUtil.GetVerseCount(b, c); v++)
                    {
                        vers = BibleXmlDocument.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber='" + b.ToString() + "']/CHAPTER[@cnumber='" + c.ToString() + "']/VERS[@vnumber='" + v.ToString() + "']");
                        if (vers != null)
                            verse_text = vers.InnerText;                                            
                        else
                            verse_text=" ";                       
                        size=size+verse_text.Length;
                        bw.Write(size);
                        sb.Append(verse_text);
                    }
            }
            string content=sb.ToString();
            content=content.Replace(' ',(char)0x1);
            bw.Write(content);
            bw.Close();
            //
            File.WriteAllText(filename + ".README.txt", "Steps:\r\n1. Change ownership of C:\\Program Files\\WindowsApps\\ to you and give full permissions to you\r\n2. Copy and replace the file AV1611.HBB bible inside \r\nC:\\Program Files\\WindowsApps\\....HeavenWordBibleStudyToolbox_...._neutral__1....\\Content\r\n3. Open HeavenWorld BibleStudy and use AV1611 which will now be the translation you just exported.\r\n\r\nNote: You cannot use a new Bible but can modify an existing Bible with your favorite Bible translation on it. Also note that unicode bibles are not supported. This is a software limitation of HeavenWord for Windows 8.\r\n\r\n\t~ Simple Bible Reader - " + GlobalMemory.AUTHOR_WEBSITE);
            percent_complete = 100;
        }
    }
}
