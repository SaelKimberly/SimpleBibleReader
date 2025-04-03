using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;

using System;
using System.Collections;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Text;
using RtfPipe;


namespace Simple_Bible_Reader
{
	public class UnboundBibleFormat : BibleFormat
	{
		System.Xml.XmlElement rootdoc;
		System.Xml.XmlElement bookdoc;
		System.Xml.XmlElement chapdoc;
		System.Xml.XmlElement versedoc;
		System.Xml.XmlAttribute attrib;
		
		int _book_no;
		int _chap_no;
		int _verse_no;
		string _verse_text;
		
		int p_book_no = -1;
		int p_chap_no = -1;
		int p_verse_no = -1;
		int mBibleFormat = F03_UNBOUND_BIBLE_BCV;

        public System.Windows.Forms.RichTextBox rtb = new System.Windows.Forms.RichTextBox();
        
        String m_language = "";
        String m_copyright = "Eternity, God";

        public UnboundBibleFormat(string file, int bibleFormat) : base(file)
		{
			mBibleFormat = bibleFormat;
        }
		
		public override void Load()
		{
			System.IO.StreamReader reader;
			//reader = System.IO.File.OpenText((string) this.FileName);
            reader = new StreamReader(new FileStream(this.FileName, FileMode.Open), System.Text.Encoding.Default);
			string line = null;
			line = reader.ReadLine();
			
			BibleXmlDocument = new System.Xml.XmlDocument();
			XmlDeclaration dec = BibleXmlDocument.CreateXmlDeclaration("1.0", null, null);
			BibleXmlDocument.AppendChild(dec);
			rootdoc = BibleXmlDocument.CreateElement("XMLBIBLE");
			BibleXmlDocument.AppendChild(rootdoc);
			
			processLine(line);
			while (!(line == null))
			{
				line = reader.ReadLine();
				processLine(line);
			}
			SetProcessAsComplete();
		}
		
		private void processLine(string line)
		{
			if (line == null)
			{
				return;
			}
			if (line.Trim().Equals("") || line.Trim().StartsWith("#"))
			{
				return;
			}
			if(line.Trim().StartsWith("filetype")) {
				if (line.Contains("BCV"))
					mBibleFormat = F03_UNBOUND_BIBLE_BCV;
				else if (line.Contains("BCVS"))
                    mBibleFormat = F04_UNBOUND_BIBLE_BCVS;
            }
			string[] data = line.Trim().Split('\t');
			if (data.Length == 4)
			{
				_book_no = int.Parse((string) (data[0].Replace("O", "").Replace("N", "")));
				_chap_no = int.Parse(data[1]);
				_verse_no = int.Parse(data[2]);
				_verse_text = data[3];
			}
            else if (data.Length == 5)
            {
                _book_no = int.Parse((string)(data[0].Replace("O", "").Replace("N", "")));
                _chap_no = int.Parse(data[1]);
                _verse_no = int.Parse(data[2]);
                _verse_text = data[4];
            }
			else if (data.Length == 6)
			{
				_book_no = int.Parse((string) (data[0].Replace("O", "").Replace("N", "")));
				_chap_no = int.Parse(data[1]);
				_verse_no = int.Parse(data[2]);
				_verse_text = data[5];
			}
			else if (data.Length == 9)
			{
				_book_no = int.Parse((string) (data[3].Replace("O", "").Replace("N", "")));
				_chap_no = int.Parse(data[4]);
				_verse_no = int.Parse(data[5]);
				_verse_text = data[8];
			}
			
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
                return mBibleFormat;
            }
        }

        public override List<string[]> getProperties()
        {
            if (kv_export.Count == 0)
            {
                kv_export.Add(new string[] { "Abbreviation:", BibleFormat.getInstance().ABBREVIATION });
                kv_export.Add(new string[] { "Description:", BibleFormat.getInstance().DESCRIPTION });
                kv_export.Add(new string[] { "Comments:", BibleFormat.getInstance().COMMENTS });
                kv_export.Add(new string[] { "Language:", m_language});
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
                    case "Language:":
                        m_language = kv_export[i][1];
                        break;
                    case "Copyright:":
                        m_copyright = kv_export[i][1];
                        break;
                }
            }
        }

        public override void ExportBible(string filename, int filter_idx)
        {
            StreamWriter writer = null;
            if (!filename.EndsWith(".txt"))
            {
                filename = filename + ".txt";
            }
            if (filename.Equals(""))
            {
                return;
            }
            processProperties();
            XmlNodeList books = BibleXmlDocument.SelectNodes("/XMLBIBLE/BIBLEBOOK");
            XmlNode vers = null;
            string ot_no = "O";
            string _verse_text = "";
            writer = new System.IO.StreamWriter(filename, false, Encoding.UTF8);
            if (F03_UNBOUND_BIBLE_BCV == mBibleFormat)
            {
                //Unmapped-BCV
                writer.WriteLine("#THE UNBOUND BIBLE (www.unboundbible.org)");
                writer.WriteLine("#name	" + DESCRIPTION);
                writer.WriteLine("filetype	Unmapped-BCV");
                writer.WriteLine("#copyright "+m_copyright);
                writer.WriteLine("#abbreviation	"+ABBREVIATION);
                writer.WriteLine("#language	"+m_language);
                writer.WriteLine("#note	"+COMMENTS);
                writer.WriteLine("#columns	orig_book_index	orig_chapter	orig_verse	order_by	text");
                int ctr = 1;
                for (int b = 1; b <= 66; b++)
                {
                    if (b < 40)
                        ot_no = "O";
                    else
                        ot_no = "N";
                    _book_no = b;
                    for (int c = 1; c <= FormatUtil.GetChapterCount(b); c++)
                        for (int v = 1; v <= FormatUtil.GetVerseCount(b, c); v++)
                        {
                            vers = BibleXmlDocument.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber='" + b.ToString() + "']/CHAPTER[@cnumber='" + c.ToString() + "']/VERS[@vnumber='" + v.ToString() + "']");

                            if (vers != null)
                            {
                                if (GlobalMemory.getInstance().ParseBible && CLEANING)
                                {
                                    rtb.Rtf = rtf_header + vers.InnerText;
                                    _verse_text = rtb.Text;
                                }
                                else
                                {
                                    _verse_text = vers.InnerText;
                                }
                                writer.WriteLine(b.ToString("00") + ot_no + "\t" + c.ToString() + "\t" + v.ToString() + "\t" + ctr.ToString() + "0\t" + _verse_text);
                                ctr++;
                            }
                        }
                }
            }
            else if (F04_UNBOUND_BIBLE_BCVS == mBibleFormat)
            {
                //Unmapped-BCVS
                writer.WriteLine("#THE UNBOUND BIBLE (www.unboundbible.org)");
                writer.WriteLine("#name	" + DESCRIPTION);
                writer.WriteLine("filetype	Unmapped-BCVS");
                writer.WriteLine("#copyright "+m_copyright);
                writer.WriteLine("#abbreviation	"+ABBREVIATION);
                writer.WriteLine("#language	"+m_language);
                writer.WriteLine("#note	"+COMMENTS);
                writer.WriteLine("#columns	orig_book_index	orig_chapter	orig_verse	orig_subverse	order_by	text");
                int ctr = 1;
                for (int b = 1; b <= 66; b++)
                {
                    if (b < 40)
                        ot_no = "O";
                    else
                        ot_no = "N";
                    _book_no = b;
                    for (int c = 1; c <= FormatUtil.GetChapterCount(b); c++)
                        for (int v = 1; v <= FormatUtil.GetVerseCount(b, c); v++)
                        {
                            vers = BibleXmlDocument.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber='" + b.ToString() + "']/CHAPTER[@cnumber='" + c.ToString() + "']/VERS[@vnumber='" + v.ToString() + "']");

                            if (vers != null)
                            {
                                _verse_text = vers.InnerText;
                                writer.WriteLine(b.ToString("00") + ot_no + "\t" + c.ToString() + "\t" + v.ToString() + "\t\t" + ctr.ToString() + "0\t" + _verse_text);
                                ctr++;
                            }
                        }
                }
            }

            writer.Close();
        }
    }
	
}
