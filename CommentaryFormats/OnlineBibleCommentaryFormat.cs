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
using System.IO;
using System.Text.RegularExpressions;


namespace Simple_Bible_Reader
{
	public class OnlineBibleCommentaryFormat : CommentaryFormat
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
        int tmp_1 = 0;
        int p_book_no = -1;
        int p_chap_no = -1;
        int p_verse_no = -1;
        int export_percent_complete = 0;
        int percent_complete = 0;
        string[] data = null;

        public OnlineBibleCommentaryFormat(string file)
            : base(file)
		{
		}
		
		public override void Load()
		{
            if (FileName.ToLower().EndsWith(".exp"))
            {
                System.IO.StreamReader reader;
                //reader = System.IO.File.OpenText((string) this.FileName);
                reader = new StreamReader(new FileStream(this.FileName, FileMode.Open), System.Text.Encoding.Default);
                string line = null;
                string verse_line = null;
                line = reader.ReadLine();
                verse_line = reader.ReadLine();
                CommentaryXmlDocument = new System.Xml.XmlDocument();
                XmlDeclaration dec = CommentaryXmlDocument.CreateXmlDeclaration("1.0", null, null);
                CommentaryXmlDocument.AppendChild(dec);
                rootdoc = CommentaryXmlDocument.CreateElement("XMLBIBLE");
                CommentaryXmlDocument.AppendChild(rootdoc);

                processLineExpFormat(line + " " + verse_line);
                while (!(line == null))
                {
                    line = reader.ReadLine();
                    verse_line = reader.ReadLine();
                    processLineExpFormat(line + " " + verse_line);
                }
            }
            percent_complete = 100;
            SetProcessAsComplete();
		}

        private void processLine()
        {
            if (_book_no != p_book_no || (_chap_no == 1 && _verse_no == 1 && p_verse_no >= _verse_no))
            {
                bookdoc = (XmlElement)CommentaryXmlDocument.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber='" + _book_no + "']");
                if (bookdoc == null)
                {
                    bookdoc = CommentaryXmlDocument.CreateElement("BIBLEBOOK");
                    attrib = CommentaryXmlDocument.CreateAttribute("bnumber");
                    attrib.Value = _book_no.ToString();
                    bookdoc.Attributes.Append(attrib);
                    rootdoc.AppendChild(bookdoc);
                }
            }
            if (_chap_no != p_chap_no || (_chap_no == 1 && _verse_no == 1 && p_verse_no >= _verse_no))
            {
                chapdoc = (XmlElement)bookdoc.SelectSingleNode("CHAPTER[@cnumber='" + _chap_no + "']");
                if (chapdoc == null)
                {
                    chapdoc = CommentaryXmlDocument.CreateElement("CHAPTER");
                    attrib = CommentaryXmlDocument.CreateAttribute("cnumber");
                    attrib.Value = _chap_no.ToString();
                    chapdoc.Attributes.Append(attrib);
                    bookdoc.AppendChild(chapdoc);
                }
            }
            if (_verse_no != p_verse_no || (_verse_no == p_verse_no && _chap_no != p_chap_no) || (_verse_no == p_verse_no && _book_no != p_book_no))
            {
                versedoc = CommentaryXmlDocument.CreateElement("VERS");
                attrib = CommentaryXmlDocument.CreateAttribute("vnumber");
                attrib.Value = _verse_no.ToString();

                if (GlobalMemory.getInstance().ParseCommentary)
                {
                    _verse_text = postProcessContent(_verse_text);
                    versedoc.InnerText = FormatUtil.EscapeXML(_verse_text);
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

        private void processLineExpFormat(string line)
        {
            if (line == null)
            {
                return;
            }
            line = line.Trim();
            if (!line.StartsWith("$"))
            {
                return;
            }
            line = line.Substring(4);
            tmp_1 = line.IndexOf(':');

            data = line.Split(':');
            if (data.Length < 2)
                return;

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


            if (_book_no != p_book_no || (_chap_no == 1 && _verse_no == 1 && p_verse_no >= _verse_no))
            {
                bookdoc = CommentaryXmlDocument.CreateElement("BIBLEBOOK");
                attrib = CommentaryXmlDocument.CreateAttribute("bnumber");
                attrib.Value = _book_no.ToString();
                bookdoc.Attributes.Append(attrib);
                rootdoc.AppendChild(bookdoc);
            }
            if (_chap_no != p_chap_no || (_chap_no == 1 && _verse_no == 1 && p_verse_no >= _verse_no))
            {
                chapdoc = CommentaryXmlDocument.CreateElement("CHAPTER");
                attrib = CommentaryXmlDocument.CreateAttribute("cnumber");
                attrib.Value = _chap_no.ToString();
                chapdoc.Attributes.Append(attrib);
                bookdoc.AppendChild(chapdoc);
            }
            if (_verse_no != p_verse_no || (_verse_no == p_verse_no && _chap_no != p_chap_no) || (_verse_no == p_verse_no && _book_no != p_book_no))
            {
                versedoc = CommentaryXmlDocument.CreateElement("VERS");
                attrib = CommentaryXmlDocument.CreateAttribute("vnumber");
                attrib.Value = _verse_no.ToString();
                versedoc.InnerText = _verse_text;
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
                return percent_complete;
            }
        }

        public override int FilterIndex
        {
            get
            {
                return 8;
            }
        }

        public override void ExportCommentary(string filename, int filter_idx)
        {
            StreamWriter writer = null;
            if (filename.ToLower().EndsWith(".dat"))
            {
                filename = filename.Substring(0, filename.Length - 4) + ".exp";
            }

            if (!filename.ToLower().EndsWith(".exp"))
            {
                filename = filename + ".exp";
            }
            if (filename.Equals(""))
            {
                return;
            }
            //filename = Path.GetDirectoryName(filename) + "\\" + Path.GetFileName(filename).ToLower();
            string _verse_text = "";
            XmlNodeList books = CommentaryXmlDocument.SelectNodes("/XMLBIBLE/BIBLEBOOK");
            XmlNode vers = null;
            writer = new System.IO.StreamWriter(filename, false, Encoding.ASCII);//must be ascii
            for (int b = 1; b <= 66; b++)
            {
                _book_no = b;
                for (int c = 1; c <= FormatUtil.GetChapterCount(b); c++)
                    for (int v = 1; v <= FormatUtil.GetVerseCount(b, c); v++)
                    {
                        vers = CommentaryXmlDocument.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber='" + b.ToString() + "']/CHAPTER[@cnumber='" + c.ToString() + "']/VERS[@vnumber='" + v.ToString() + "']");
                        if (vers != null)
                        {
                            _verse_text = vers.InnerText;
                            writer.WriteLine("$$$ " + FormatUtil.olbBookNames[b - 1] + " " + c.ToString() + ":" + v.ToString() + " ");
                            writer.WriteLine(_verse_text);
                        }
                    }
            }
            writer.Close();
            export_percent_complete = 100;
        }

    }
}
