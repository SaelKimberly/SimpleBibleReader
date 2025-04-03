using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace Simple_Bible_Reader
{
    class BibleCompanionBibleFormat : BibleFormat
    {
        string[] bib_codes = new string[] { "Gen", "Exo", "Lev", "Num", "Deu", "Jos", "Jdg", "Rth", "1Sa", "2Sa", "1Ki", "2Ki", "1Ch", "2Ch", "Ezr", "Neh", "Est", "Job", "Psa", "Pro", "Ecc", "Son", "Isa", "Jer", "Lam", "Eze", "Dan", "Hos", "Joe", "Amo", "Oba", "Jon", "Mic", "Nah", "Hab", "Zep", "Hag", "Zec", "Mal", "Mat", "Mar", "Luk", "Joh", "Act", "Rom", "1Co", "2Co", "Gal", "Eph", "Phi", "Col", "1Th", "2Th", "1Ti", "2Ti", "Tit", "Phm", "Heb", "Jam", "1Pe", "2Pe", "1Jo", "2Jo", "3Jo", "Jud", "Rev" };

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
        string[] data = null;

        public BibleCompanionBibleFormat(string file)
            : base(file)
		{
		}
		
		public override void Load()
		{
			System.IO.StreamReader reader;
			//reader = System.IO.File.OpenText((string) this.FileName);
            reader = new StreamReader(new FileStream(this.FileName, FileMode.Open), System.Text.Encoding.Default);
			string line = null;
            reader.ReadLine(); //ignore 1st line
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
            line = line.Replace('\t', ' ');
            
            line=line.Trim();
			if (line.Equals(""))
			{
				return;
			}
           
            data = line.Split(':');
            if (data.Length < 2)
            {
                return;
            }

            _book_no = FormatUtil.getBookNo(data[0].Split(' ')[0]);
            if (_book_no == -1)
            {
                return;
            }
            try
            {
                _chap_no = int.Parse(data[0].Split(' ')[1].Trim());
                _verse_no = int.Parse(data[1].Split(' ')[0].Trim());
                _verse_text = data[1].Substring(data[1].IndexOf(' ')).Trim();
                if(data.Length>2)
                    for (int d = 2; d < data.Length; d++)
                        _verse_text=_verse_text+":"+data[d];
            }
            catch (Exception)
            {
                return;
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
                return F14_BIBLE_COMPANION;
            }
        }

        public override void ExportBible(string filename,int filter_idx)
        {
            StreamWriter writer = null;
            if (!filename.EndsWith(".bib"))
            {
                filename = filename + ".bib";
            }
            if (filename.Equals(""))
            {
                return;
            }
            writer = new System.IO.StreamWriter(filename, false, Encoding.UTF8);
            writer.WriteLine(Path.GetFileNameWithoutExtension(filename) + "|" + Path.GetFileNameWithoutExtension(filename));
            XmlNodeList books = BibleXmlDocument.SelectNodes("/XMLBIBLE/BIBLEBOOK");
            XmlNode vers = null;
            string _verse_text = "";
            for (int b = 1; b <= 66; b++)
            {
                _book_no = b;
                for (int c = 1; c <= FormatUtil.GetChapterCount(b); c++)
                    for (int v = 1; v <= FormatUtil.GetVerseCount(b, c); v++)
                    {
                        vers = BibleXmlDocument.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber='" + b.ToString() + "']/CHAPTER[@cnumber='" + c.ToString() + "']/VERS[@vnumber='" + v.ToString() + "']");
                        if (vers != null)
                        {
                             _verse_text = vers.InnerText;                            
                            writer.WriteLine(bib_codes[b - 1] + " " + c.ToString() + ":" + v.ToString() + "\t" + _verse_text);
                        }
                    }
            }                
            writer.Close();
        }
    }
}
