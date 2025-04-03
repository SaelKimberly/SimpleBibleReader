using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;

namespace Simple_Bible_Reader
{
    class VPLBibleFormat : BibleFormat
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
        public System.Windows.Forms.RichTextBox rtb = new System.Windows.Forms.RichTextBox();
        int tmp_1 = 0;
        string[] data = null;

        public VPLBibleFormat(string file)
            : base(file)
		{
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
            line=line.Trim();
			if (line.Equals("") || line.StartsWith("#"))
			{
				return;
			}

            tmp_1 = line.IndexOf(':');

            if (tmp_1 != -1 && tmp_1 < 9)
            {
                data = line.Split(':');
                if (data.Length < 2)
                    return;

                //G VPL
                _book_no = FormatUtil.getBookNo(data[0].Split(' ')[0]);
                if (_book_no == -1)
                    return;
                try
                {
                    _chap_no = int.Parse(data[0].Split(' ')[1].Trim());
                    _verse_no = int.Parse(data[1].Split(' ')[0].Trim());
                    _verse_text = data[1].Substring(data[1].IndexOf(' ')).Trim();
                    if (data.Length > 2)
                        for (int d = 2; d < data.Length; d++)
                            _verse_text = _verse_text + ":" + data[d];
                }
                catch (Exception)
                {
                    return;
                }                
            }
            else
            {
                data = line.Split('.');

                if (data.Length < 3)
                    return;

                // Crosswire's VPL
                _book_no = FormatUtil.getBookNo(data[0]);
                if (_book_no == -1)
                {
                    Themes.MessageBox(data[0]);
                    return;
                }
                try
                {
                    _chap_no = int.Parse(data[1]);
                    _verse_no = int.Parse(data[2].Split(' ')[0]);
                    _verse_text = line.Substring(line.IndexOf(' ')).Trim();
                    if (data.Length > 2)
                        for (int d = 2; d < data.Length; d++)
                            _verse_text = _verse_text + ":" + data[d];
                }
                catch (Exception)
                {
                    return;
                }                
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
                return F13_VPL;
            }
        }

        public override void ExportBible(string filename,int filter_idx)
        {
            StreamWriter writer = null;
            if (!filename.EndsWith(".vpl"))
            {
                filename = filename + ".vpl";
            }
            if (filename.Equals(""))
            {
                return;
            }
            string _verse_text = "";
            XmlNodeList books = BibleXmlDocument.SelectNodes("/XMLBIBLE/BIBLEBOOK");
            XmlNode vers = null;
            writer = new System.IO.StreamWriter(filename, false, Encoding.UTF8);
            for (int b = 1; b <= 66; b++)
            {
                _book_no = b;
                for (int c = 1; c <= FormatUtil.GetChapterCount(b); c++)
                    for (int v = 1; v <= FormatUtil.GetVerseCount(b, c); v++)
                    {
                        vers = BibleXmlDocument.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber='" + b.ToString() + "']/CHAPTER[@cnumber='" + c.ToString() + "']/VERS[@vnumber='" + v.ToString() + "']");
                        if (vers != null)
                        {
                            _verse_text = processStrongVerse(vers,b);                            
                            writer.WriteLine(FormatUtil.shortBookNames[b - 1] + " " + c.ToString() + ":" + v.ToString() + " " + _verse_text);
                        }
                    }
            }                
            writer.Close();
        }

        private string processStrongVerse(XmlNode vers, int b)
        {
            string prefix = "G";
            string _verse_text = "";
            if (b < 40)
                prefix = "H";
            if (GlobalMemory.getInstance().ParseBible)
            {
                _verse_text = vers.InnerXml;
                
                if (_verse_text.IndexOf("<gr") != -1)
                {
                    _verse_text = Regex.Replace(_verse_text, "(<gr )([^s]*)(str=['\"]*)([^<'\"]*)['\"]*(>)(((?!</gr).)*)</gr>", @"$6 <" + prefix + @"$4>", RegexOptions.IgnoreCase);
                }
            }
            else
                _verse_text = vers.InnerText;
            return _verse_text;
        }
    }
}
