
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Xml;
using System.Text;
using System.IO;

namespace Simple_Bible_Reader
{
    public class GBFBibleFormat : BibleFormat
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
        int begin_idx;

        int end_idx;
        public System.Windows.Forms.RichTextBox rtb = new System.Windows.Forms.RichTextBox();

        StringBuilder sb = new StringBuilder();
        public GBFBibleFormat(string file)
            : base(file)
        {
        }

        public override void Load()
	{
		System.IO.StreamReader reader = null;
		//reader = System.IO.File.OpenText(FileName);
        reader = new StreamReader(new FileStream(this.FileName, FileMode.Open), System.Text.Encoding.Default);
		string bible_str = reader.ReadToEnd();

		BibleXmlDocument = new System.Xml.XmlDocument();
		XmlDeclaration dec = BibleXmlDocument.CreateXmlDeclaration("1.0", null, null);
		BibleXmlDocument.AppendChild(dec);
		rootdoc = BibleXmlDocument.CreateElement("XMLBIBLE");
		BibleXmlDocument.AppendChild(rootdoc);

		string book_str = null;
		string chap_str = null;
		string verse_str = null;
		int chap_count = 0;
		int verse_count = 0;
		//'
		Hashtable gbf_abbr = new Hashtable();
        string[] m_books = bible_str.Split(new String[]{"<SB"}, StringSplitOptions.RemoveEmptyEntries);
		string m_book = null;
		for (int i = 1; i <= m_books.Length - 1; i++) {
			m_book = m_books[i].Substring(0, m_books[i].IndexOf(">"));
			for (int j = 1; j <= 66; j++) {
                if (Array.IndexOf(FormatUtil.getPossibleAbbr(j), m_book) != -1 | Array.IndexOf(FormatUtil.getPossibleAbbr(j), m_book.ToLower()) != -1 | Array.IndexOf(FormatUtil.getPossibleAbbr(j), m_book.ToUpper()) != -1)
                {
                    gbf_abbr.Add(j, m_book);
                    //break; // TODO: might not be correct. Was : Exit For
                }
			}
		}
		//'
		for (int i = 1; i <= 66; i++) {
			_book_no = i;
            try
            {
                book_str = getBookString(gbf_abbr[i].ToString(), bible_str);
            }
            catch (Exception)
            {
                continue;
            }

			if (FormatUtil.Len(book_str) == 0)
				continue;
            chap_count = FormatUtil.GetChapterCount(i);
			for (int j = 1; j <= chap_count; j++) {
				_chap_no = j;
				chap_str = getChapString(book_str, j);
				if (FormatUtil.Len(chap_str) == 0)
					continue;
                verse_count = FormatUtil.GetVerseCount(i, j);
				for (int k = 1; k <= verse_count; k++) {
					_verse_no = k;
					verse_str = getVerseString(chap_str, k);
					if (FormatUtil.Len(verse_str) == 0)
						continue;
					_verse_text = verse_str;
                    processVerse();
				}
			}
		}
		SetProcessAsComplete();
	}

        private void processVerse()
        {
            if (_book_no != p_book_no | (_chap_no == 1 & _verse_no == 1 & p_verse_no > _verse_no))
            {
                bookdoc = BibleXmlDocument.CreateElement("BIBLEBOOK");
                attrib = BibleXmlDocument.CreateAttribute("bnumber");
                attrib.Value = _book_no.ToString();
                bookdoc.Attributes.Append(attrib);
                rootdoc.AppendChild(bookdoc);
            }
            if (_chap_no != p_chap_no | (_chap_no == 1 & _verse_no == 1 & p_verse_no > _verse_no))
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

        private string getVerseString(string chapstr, int verse)
        {
            string verse_str = null;
            try
            {
                sb.Length = 0;
                sb.Append("<SV");
                sb.Append(verse.ToString());
                sb.Append(">");
                begin_idx = FormatUtil.InStr2(chapstr, sb.ToString()) + (sb.ToString()).Length - 1;
                end_idx = FormatUtil.InStr2(begin_idx + 1, chapstr, "<SV") - 1;
                if (end_idx == -1)
                {
                    end_idx = chapstr.Length - 1;
                }
                verse_str = chapstr.Substring(begin_idx, end_idx - begin_idx);
            }
            catch (Exception)
            {
            }
            return verse_str;
        }

        private string getChapString(string bookstr, int chapter)
        {
            string chap_str = null;
            try
            {
                sb.Length = 0;
                sb.Append("<SC");
                sb.Append(chapter.ToString());
                sb.Append(">");
                begin_idx = FormatUtil.InStr2(bookstr, sb.ToString()) - 1;
                end_idx = FormatUtil.InStr2(begin_idx + (sb.ToString()).Length, bookstr, "<SC") - 1;
                if (end_idx == -1)
                {
                    end_idx = bookstr.Length - 1;
                }
                chap_str = bookstr.Substring(begin_idx, end_idx - begin_idx);
            }
            catch (Exception)
            {
            }
            return chap_str;
        }

        private string getBookString(string book_name, string biblestr)
        {
            string book_str = null;
            try
            {
                sb.Length = 0;
                sb.Append("<SB");
                sb.Append(book_name);
                sb.Append(">");
                begin_idx = FormatUtil.InStr2(biblestr, sb.ToString()) - 1;
                end_idx = FormatUtil.InStr2(begin_idx + (sb.ToString()).Length, biblestr, "<SB") - 1;
                if (end_idx == -1)
                {
                    end_idx = biblestr.Length - 1;
                }
                book_str = biblestr.Substring(begin_idx, end_idx - begin_idx);
            }
            catch (Exception)
            {
            }
            return book_str;
        }

        public override int PercentComplete
        {
            get { return ((_book_no) * 100) / 66; }
        }

        public override int FilterIndex
        {
            get
            {
                return F06_GBF;
            }
        }

        public override void ExportBible(string filename, int filter_idx)
        {
            StreamWriter writer = null;
            if (!filename.EndsWith(".gbf"))
            {
                filename = filename + ".gbf";
            }
            if (filename.Equals(""))
            {
                return;
            }

            XmlNodeList books = BibleXmlDocument.SelectNodes("/XMLBIBLE/BIBLEBOOK");
            XmlNode vers = null;

            writer = new System.IO.StreamWriter(filename,false,Encoding.UTF8);

            writer.WriteLine("<H000>");
            writer.Write("<H1>");
            writer.WriteLine(ABBREVIATION);
            writer.Write("<H2>");
            writer.WriteLine(DESCRIPTION);
                
            bool ot_done=false;
            bool nt_done=false;

            int ctr = 1;
            string book_name="";
            XmlNode obj=null;
            StringBuilder sv = new StringBuilder();
            StringBuilder sc = new StringBuilder();
            string _verse_text = "";
            for (int b = 1; b <= 66; b++)
            {
                _book_no = b;
                    
                sc.Length = 0;
                for (int c = 1; c <= FormatUtil.GetChapterCount(b); c++)
                {                        
                    sv.Length = 0;
                    for (int v = 1; v <= FormatUtil.GetVerseCount(b, c); v++)
                    {
                        vers = BibleXmlDocument.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber='" + b.ToString() + "']/CHAPTER[@cnumber='" + c.ToString() + "']/VERS[@vnumber='" + v.ToString() + "']");

                        if (vers != null)
                        {
                            if(!ot_done && b<40)
                            {
                                writer.WriteLine("<BO><CM>");                                    
                                ot_done =true;
                            }
                            else if(!nt_done && b>=40 && b<=66)
                            {
                                writer.WriteLine("<BN><CM>");
                                nt_done =true;
                            }
                             _verse_text = vers.InnerText;
                           
                            sv.Append("<SV" + v.ToString() + ">" + _verse_text);
                            ctr++;
                        }
                    }
                    if (ot_done && b < 40 && sv.ToString().Length!=0)
                        sc.Append("<SC" + c.ToString() + ">" + sv.ToString());
                    else if (nt_done && b >= 40 && b <= 66 && sv.ToString().Length != 0)
                        sc.Append("<SC" + c.ToString() + ">" + sv.ToString());
                }
                if (sc.Length != 0)
                {
                    if (ot_done && b < 40)
                    {
                        obj = BibleXmlDocument.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber='" + b.ToString() + "']/@bname");
                        if (obj != null)
                            book_name = obj.InnerText;
                        else
                            book_name = Localization.getBookNames()[b - 1];
                        writer.WriteLine("<SB" + b.ToString() + "><TN>" + FormatUtil.shortBookNames[b - 1] + "<Tn><TT>" + book_name + "<Tt><CM>" + sc.ToString());
                    }
                    else if (nt_done && b >= 40 && b <= 66)
                    {
                        obj = BibleXmlDocument.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber='" + b.ToString() + "']/@bname");
                        if (obj != null)
                            book_name = obj.InnerText;
                        else
                            book_name = Localization.getBookNames()[b - 1];
                        writer.WriteLine("<SB" + b.ToString() + "><TN>" + FormatUtil.shortBookNames[b - 1] + "<Tn><TT>" + book_name + "<Tt><CM>" + sc.ToString());
                    }
                }
            }
            writer.WriteLine("<ZZ>");
            writer.Close();
        }
    }
}
