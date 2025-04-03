using System;
using System.Text;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;
using PalmBiblePlus;
using PalmBibleExport;

namespace Simple_Bible_Reader
{
    class PDBBibleFormat : BibleFormat
    {
		System.Xml.XmlElement rootdoc;
		System.Xml.XmlElement bookdoc;
		System.Xml.XmlElement chapdoc;
		System.Xml.XmlElement versedoc;
		System.Xml.XmlAttribute attrib;
        System.Xml.XmlAttribute attrib1;
        System.Xml.XmlAttribute attrib2;
		
		int _book_no;
        string _book_name;
        string _book_short_name;
		int _chap_no;
		int _verse_no;
		string _verse_text;
        int total_books = 66;
        int book_status = -1;

		int p_book_no = -1;
		int p_chap_no = -1;
		int p_verse_no = -1;
        public System.Windows.Forms.RichTextBox rtb = new System.Windows.Forms.RichTextBox();
        int tmp_1 = 0;
        string[] data = null;

        public PDBBibleFormat(string file)
            : base(file)
		{
		}
		
		public override void Load()
		{
            PDBFileStream fis = new PDBFileStream(this.FileName);            
            BiblePlusPDB bible = new BiblePlusPDB(fis, EncodingTable.hebrewtab, EncodingTable.greektab);
            bible.loadVersionInfo();
            bible.loadWordIndex();
			
			BibleXmlDocument = new System.Xml.XmlDocument();
			XmlDeclaration dec = BibleXmlDocument.CreateXmlDeclaration("1.0", null, null);
			BibleXmlDocument.AppendChild(dec);
			rootdoc = BibleXmlDocument.CreateElement("XMLBIBLE");
			BibleXmlDocument.AppendChild(rootdoc);
            total_books = bible.BookCount;
            for (int b = 0; b < total_books; b++)
            {
                book_status = b;
                BookInfo book = bible.getBook(b);
                book.openBook();	
                for (int i = 1; i <= book.ChapterCount; i++)
                {
                    for (int j = 1; j <= book.getVerseCount(i); j++)
                    {
                        try
                        {
                            _book_no = FormatUtil.convertPDBBkNoToNormalBkNo(book.BookNumber); 
                        }
                        catch (Exception)
                        {
                            _book_no = book.BookNumber;
                        }
                                               
                        _book_name = book.FullName;
                        _book_short_name = book.ShortName;
                        _chap_no = i;
                        _verse_no = j;
                        _verse_text = book.getVerse(i, j);
                        processLine();
                    }
                }
            }
            bible.close();
            fis.close();
			SetProcessAsComplete();
		}
		
		private void processLine()
		{  			
			if (_book_no != p_book_no || (_chap_no == 1 && _verse_no == 1 && p_verse_no >= _verse_no))
			{
				bookdoc = BibleXmlDocument.CreateElement("BIBLEBOOK");
				attrib = BibleXmlDocument.CreateAttribute("bnumber");
				attrib.Value = _book_no.ToString();
				
                attrib1 = BibleXmlDocument.CreateAttribute("bname");
                attrib1.Value = _book_name;
                
                attrib2 = BibleXmlDocument.CreateAttribute("bsname");
                attrib2.Value = _book_no.ToString();

                bookdoc.Attributes.Append(attrib);
                bookdoc.Attributes.Append(attrib1);
                bookdoc.Attributes.Append(attrib2);

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
                    int idx = 0;
                    string ch = Encoding.ASCII.GetString(new byte[] { 0xe });

                    while ((idx = _verse_text.IndexOf(ch + "r" + ch)) != -1)
                    {
                        _verse_text = _verse_text.Substring(0, idx) + "<STYLE css=\"color:#ff0000\">" + _verse_text.Substring(idx + (ch + "r" + ch).Length);
                        idx = _verse_text.IndexOf(ch + "r" + ch);
                        if (idx != -1)
                            _verse_text = _verse_text.Substring(0, idx) + "</STYLE>" + _verse_text.Substring(idx + (ch + "r" + ch).Length);
                        else
                            _verse_text = _verse_text + "</STYLE>";
                    }
                    //
                    while ((idx = _verse_text.IndexOf(ch + "i" + ch)) != -1)
                    {
                        _verse_text = _verse_text.Substring(0, idx) + "[" + _verse_text.Substring(idx + (ch + "i" + ch).Length).Trim();
                        idx = _verse_text.IndexOf(ch + "i" + ch);
                        if (idx != -1)
                            _verse_text = _verse_text.Substring(0, idx).Trim() + "]" + _verse_text.Substring(idx + (ch + "i" + ch).Length);
                        else
                            _verse_text = _verse_text + "]";
                    }
                    //_verse_text = _verse_text.Replace(ch, "");
                    _verse_text = Regex.Replace(_verse_text, "[\x00-\x1f]", "");


                    try
                    {
                        if (!GlobalMemory.getInstance().parseBibleBkgndCleaner)
                            _verse_text = postProcessContent(_verse_text);
                        versedoc.InnerXml = _verse_text;
                    }
                    catch (Exception)
                    {
                        versedoc.InnerText = _verse_text;
                    }
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
                return ((book_status) * 100) / total_books;
			}
		}

        public override int FilterIndex
        {
            get
            {
                return F25_PALMBIBLE;
            }
        }

        public override void ExportBible(string filename,int filter_idx)
        {
            StreamWriter writer = null;
            if (!filename.EndsWith(".pdb"))
            {
                filename = filename + ".pdb";
            }
            if (filename.Equals(""))
            {
                return;
            }
            string _verse_text = "";
            XmlNodeList books = BibleXmlDocument.SelectNodes("/XMLBIBLE/BIBLEBOOK");
            XmlNode vers = null;
            string tmpfile_input_txt = Path.GetTempFileName();
            writer = new System.IO.StreamWriter(tmpfile_input_txt, false, Encoding.ASCII);
            writer.WriteLine("<PARSERINFO ENCODE=\"iso-8859-1\" WORDTYPE=\"SPCSEP\">");
            writer.WriteLine("<BIBLE NAME=\""+ABBREVIATION+"\" INFO=\""+DESCRIPTION+"\\n"+COMMENTS+"\">");
            string book_name=null;
            XmlNode bname=null;
            total_books = 66;
            for (int b = 1; b <= 66; b++)
            {
                book_status = b;
                bname=BibleXmlDocument.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber='" + b.ToString() + "']/bname");
                if (bname != null)
                    book_name = bname.Value;
                else
                    book_name = FormatUtil.getBookName(b);
                writer.WriteLine("<BOOK NUMBER=\"" + FormatUtil.pdb_book_numbers[b - 1].ToString() + "\" NAME=\"" + book_name + "\" SHORTCUT=\""+FormatUtil.shortBookNames[b-1]+"\">");
                _book_no = b;
                for (int c = 1; c <= FormatUtil.GetChapterCount(b); c++)
                {
                    writer.WriteLine("<CHAPTER>");
                    for (int v = 1; v <= FormatUtil.GetVerseCount(b, c); v++)
                    {
                        vers = BibleXmlDocument.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber='" + b.ToString() + "']/CHAPTER[@cnumber='" + c.ToString() + "']/VERS[@vnumber='" + v.ToString() + "']");
                        if (vers != null)
                        {
                            _verse_text = vers.InnerText;
                            writer.Write("<VERSE>");
                            if (v == 1)
                            {
                                writer.Write("<CHAPTEXT>Chapter "+c.ToString()+"<VERSTEXT>");
                            }
                            writer.Write(_verse_text);
                            writer.Write("</VERSE>");
                            writer.WriteLine();
                        }
                    }
                    writer.WriteLine("</CHAPTER>");
                }
                writer.WriteLine("</BOOK>");
            }
            writer.WriteLine("</BIBLE>");
            writer.Close();            
            BibleDoc bibleDoc = new BibleDoc(tmpfile_input_txt, null, null, true, true);
            new PDBDoc(bibleDoc, filename);

            File.Delete(tmpfile_input_txt);
        }
    }
}
