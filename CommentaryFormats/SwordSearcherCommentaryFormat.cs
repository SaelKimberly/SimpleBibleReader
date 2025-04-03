using System.Data;
using System;
using System.Xml;
using System.Data.SQLite;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Collections;
using System.Text;
using System.Windows.Forms;

namespace Simple_Bible_Reader
{    

    public class SwordSearcherCommentaryFormat : CommentaryFormat
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

        int percent_complete = 0;

        string[] ssbib_booknames = { "Ge", "Ex", "Le", "Nu", "De", "Jos", "Jg", "Ru", "1Sa", "2Sa", "1Ki", "2Ki", "1Ch", "2Ch", "Ezr", "Ne", "Es", "Job", "Ps", "Pr", "Ec", "Song", "Isa", "Jer", "La", "Eze", "Da", "Ho", "Joe", "Am", "Ob", "Jon", "Mic", "Na", "Hab", "Zep", "Hag", "Zec", "Mal", "Mt", "Mr", "Lu", "Joh", "Ac", "Ro", "1Co", "2Co", "Ga", "Eph", "Php", "Col", "1Th", "2Th", "1Ti", "2Ti", "Tit", "Phm", "Heb", "Jas", "1Pe", "2Pe", "1Jo", "2Jo", "3Jo", "Jude", "Re" };

        struct ssbook
        {
            public sschapter[] chapter;
        };

        struct sschapter
        {
            public string[] verse;
        };

        public SwordSearcherCommentaryFormat(string file)
            : base(file)
        {
            
        }

        public override void Load()
        {
            CommentaryXmlDocument = new System.Xml.XmlDocument();
            XmlDeclaration dec = CommentaryXmlDocument.CreateXmlDeclaration("1.0", null, null);
            CommentaryXmlDocument.AppendChild(dec);
            rootdoc = CommentaryXmlDocument.CreateElement("XMLBIBLE");
            CommentaryXmlDocument.AppendChild(rootdoc);

            percent_complete = 0;

            if (this.FileName.EndsWith(".txt"))
            {
                string[] tmp, tmp1;
                string content;
                //swordsearcher forge import format.
                StreamReader reader = File.OpenText(this.FileName);
                content = reader.ReadToEnd();
                reader.Close();

                // identify description and abbr
                int v_idx = content.IndexOf("$$") - 2;
                string header = content.Substring(0, v_idx).Trim();
                tmp = header.Split(new char[] { ';' });
                for (int i = 0; i < tmp.Length; i++)
                {
                    if (tmp[i].Trim() == "")
                        continue;
                    tmp1 = tmp[i].Trim().Split(new char[] { ':' });
                    if (tmp1[0].Trim() == "TITLE")
                        DESCRIPTION = tmp1[1].Trim();
                    if (tmp1[1].Trim() == "ABBREVIATION")
                        ABBREVIATION = tmp1[1].Trim();
                }

                // get the contents.
                content = content.Substring(v_idx).Trim();
                content = Regex.Replace(content, "([$]{2}.*)([^\\r\\n]+)[\\r\\n]*([^$]+)", "$1|$3");
                content = Regex.Replace(content, "\\$\\$\\s\\{[^$]+", ""); // remove unnecessary tags
                content = content.Replace("\r\n$$", "$$");
                content = content.Replace("\r\n", "");
                content = content.Replace("$$", "\r\n");
                content = content.Trim();

                int total_books = 66;
                int verse_idx = 0;
                string[] verses_text_line = content.Trim().Split('\n');
                string line;

                // initialise
                ssbook[] ssbooks = new ssbook[total_books];
                for (int b = 0; b < total_books; b++)
                {
                    ssbooks[b].chapter = new sschapter[FormatUtil.GetChapterCount(b + 1)];
                    for (int c = 0; c < FormatUtil.GetChapterCount(b + 1); c++)
                    {
                        ssbooks[b].chapter[c].verse = new string[FormatUtil.GetVerseCount(b + 1, c + 1)];
                        for (int v = 0; v < FormatUtil.GetVerseCount(b + 1, c + 1); v++)
                        {
                            ssbooks[b].chapter[c].verse[v] = "";
                        }
                    }
                }

                //
                string verse_ref, verse_text;
                int m_book, m_chap, m_verse;
                for (int i = 0; i < verses_text_line.Length; i++)
                {
                    if (verses_text_line[i].Trim() == "")
                        continue;
                    tmp = verses_text_line[i].Split(new char[] { '|' });
                    if (tmp.Length == 2)
                    {
                        verse_ref = tmp[0].Trim();
                        verse_text = tmp[1].Trim();
                        tmp1 = verse_ref.Trim().Split(new char[] { ' ', ':' });
                        if (tmp1.Length == 3)
                        {
                            m_book = Array.FindIndex(ssbib_booknames, book_name => book_name == tmp1[0].Trim());

                            m_chap = int.Parse(tmp1[1].Trim()) - 1;
                            m_verse = int.Parse(tmp1[2].Trim()) - 1;

                            // populate
                            ssbooks[m_book].chapter[m_chap].verse[m_verse] = verse_text.Trim();
                        }
                    }
                }
                //
                percent_complete = 0;
                for (int i = 1; i <= total_books; i++)
                {
                    percent_complete = System.Convert.ToInt32((i * 100) / 66);
                    bookdoc = CommentaryXmlDocument.CreateElement("BIBLEBOOK");
                    attrib = CommentaryXmlDocument.CreateAttribute("bnumber");
                    attrib.Value = i.ToString();
                    bookdoc.Attributes.Append(attrib);
                    rootdoc.AppendChild(bookdoc);
                    for (int j = 1; j <= FormatUtil.GetChapterCount(i); j++)
                    {
                        chapdoc = CommentaryXmlDocument.CreateElement("CHAPTER");
                        attrib = CommentaryXmlDocument.CreateAttribute("cnumber");
                        attrib.Value = j.ToString();
                        chapdoc.Attributes.Append(attrib);
                        bookdoc.AppendChild(chapdoc);
                        for (int k = 1; k <= FormatUtil.GetVerseCount(i, j); k++)
                        {

                            line = ssbooks[i - 1].chapter[j - 1].verse[k - 1];


                            versedoc = CommentaryXmlDocument.CreateElement("VERS");
                            attrib = CommentaryXmlDocument.CreateAttribute("vnumber");
                            attrib.Value = k.ToString();

                            if (GlobalMemory.getInstance().ParseCommentary)
                            {
                                line = postProcessContent(line);
                                versedoc.InnerText = FormatUtil.EscapeXML(line);
                            }
                            else
                                versedoc.InnerXml = FormatUtil.EscapeXML(line);
                            versedoc.Attributes.Append(attrib);
                            chapdoc.AppendChild(versedoc);
                            verse_idx++;
                        }
                    }
                }

            }
            else
            {
                try
                {
                    int[] ssbib_offset;
                    int[] ssbib_length;
                    int total_verses = 31102;
                    int total_books = 66;

                    using (BinaryReader reader = new BinaryReader(File.OpenRead(this.FileName)))
                    {
                        int pos = 0;
                        // header text
                        string header_text = Encoding.UTF8.GetString(reader.ReadBytes(0xC8));//skip header text.
                        pos += 0xC8;
                        //Console.WriteLine("Header Text: " + header_text);

                        // bible book
                        byte len = reader.ReadByte();
                        string bible_book = Encoding.UTF8.GetString(reader.ReadBytes(0x3D - 1));//skip header text.
                        pos += 0x3D;
                        //Console.WriteLine("Bible Book: " + bible_book);
                        DESCRIPTION = bible_book;

                        // abbr
                        len = reader.ReadByte();
                        string abbr = Encoding.UTF8.GetString(reader.ReadBytes(0x0B - 1));//skip header text.
                        pos += 0x0B;
                        //Console.WriteLine("Abbreviation: " + abbr);
                        ABBREVIATION = abbr;

                        //total books
                        total_books = reader.ReadInt16();
                        pos += 2;
                        //Console.WriteLine("Total Books: " + total_books);

                        //total_verses
                        total_verses = reader.ReadInt16();
                        pos += 2;
                        //Console.WriteLine("Total Verses: " + total_verses);
                        ssbib_offset = new int[total_verses];
                        ssbib_length = new int[total_verses];

                        // unknown?
                        reader.ReadBytes(0x97);
                        pos += 0x97;

                        // preface
                        int preface_offset = reader.ReadInt32();
                        pos += 4;
                        int preface_length = reader.ReadInt16();
                        pos += 2;

                        // unknown?
                        reader.ReadBytes(0x96);
                        pos += 0x96;

                        // supportablility?
                        int sup_offset = reader.ReadInt32();
                        pos += 4;
                        int sup_length = reader.ReadInt16();
                        pos += 2;

                        // unknown?
                        reader.ReadBytes(0x1D);
                        pos += 0x1D;

                        for (int i = 0; i < total_verses; i++)
                        {
                            //
                            ssbib_offset[i] = reader.ReadInt32();
                            pos += 4;
                            ssbib_length[i] = reader.ReadInt16() + 2; // without this +2, it seems to truncate esp., strongs
                            pos += 2;
                            reader.ReadBytes(0x5);
                            pos += 0x5;

                        }
                    }


                    byte[] filebytes = File.ReadAllBytes(this.FileName);
                    byte[] bytes_to_read;
                    string[] verses_text = new string[total_verses];
                    using (MemoryStream mstream = new MemoryStream(filebytes))
                    {
                        percent_complete = 0;
                        for (int i = 0; i < total_verses; i++)
                        {
                            percent_complete = System.Convert.ToInt32((i * 100) / total_verses);
                            try
                            {
                                
                                bytes_to_read = new byte[ssbib_length[i]];
                                mstream.Position = ssbib_offset[i];
                                mstream.Read(bytes_to_read, 0, ssbib_length[i]);
                                string uncompressed = Encoding.UTF8.GetString(zLibUncompress(bytes_to_read));
                                //remove strongs
                                uncompressed = uncompressed.Replace("\u0002", "").Replace("\u0003", "");
                                verses_text[i] = uncompressed;
                                //Console.WriteLine(uncompressed);
                            }
                            catch (Exception)
                            {
                                verses_text[i] = "";
                            }

                        }
                    }

                    int verse_idx = 0;
                    string line = "";
                    for (int i = 1; i <= total_books; i++)
                    {
                        
                        for (int j = 1; j <= FormatUtil.GetChapterCount(i); j++)
                        {
                            for (int k = 1; k <= FormatUtil.GetVerseCount(i, j); k++)
                            {

                                _verse_text = verses_text[verse_idx];
                                _book_no = i;
                                _chap_no = j;
                                _verse_no = k;   
                                if(_verse_text.Trim()!="")
                                    processLine();
                                verse_idx++;
                            }
                        }
                    }

                }
                catch (Exception e)
                {

                }
            }
            //if (isUnicode(_verse_text))
            //    CLEANING = false;
            ////////////////////////////////////////           
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
                    //
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
                try
                {
                    processLine(versedoc, _verse_text);
                }
                catch (Exception e)
                {
                    versedoc.InnerText = _verse_text;
                }
                //versedoc.InnerText = _verse_text;

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
                return 10;
            }
        }

        private static byte[] zLibUncompress(byte[] inBytes)
        {
            MemoryStream compressedFileStream = new System.IO.MemoryStream(inBytes);
            compressedFileStream.Seek(2, SeekOrigin.Begin);
            MemoryStream outputFileStream = new MemoryStream();
            var decompressor = new DeflateStream(compressedFileStream, CompressionMode.Decompress);
            decompressor.CopyTo(outputFileStream);
            return outputFileStream.ToArray();
        }

        public override void ExportCommentary (string filename, int filter_idx)
        {
            filename = Regex.Replace(filename, "\\.ss[56789]cmty", "");
            if (!filename.ToLower().EndsWith(".txt"))
            {
                filename = filename + ".txt";
            }
            if (filename.Equals(""))
            {
                return;
            }

            string tmp_file = filename;

            if (System.IO.File.Exists(tmp_file))
                System.IO.File.Delete(tmp_file);

            StreamWriter writer = new System.IO.StreamWriter(tmp_file, false, Encoding.UTF8);

            writer.WriteLine("; TITLE: " + DESCRIPTION + " ");
            writer.WriteLine("; ABBREVIATION: " + ABBREVIATION + " ");
            writer.WriteLine("$$ {AUTHORDETAIL} ");
            writer.WriteLine("<p>" + COMMENTS + "</p> ");


            XmlNode vers;
            for (int b = 1; b <= 66; b++)
            {
                percent_complete = b * 100 / 66;
                for (int c = 1; c <= FormatUtil.GetChapterCount(b); c++)
                    for (int v = 1; v <= FormatUtil.GetVerseCount(b, c); v++)
                    {

                        vers = CommentaryXmlDocument.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber='" + b.ToString() + "']/CHAPTER[@cnumber='" + c.ToString() + "']/VERS[@vnumber='" + v.ToString() + "']");
                        if (vers != null)
                        {
                            _verse_text = processExportVerse(vers, b);
                            if (_verse_text != "")
                            {
                                writer.WriteLine("$$ " + ssbib_booknames[b - 1] + " " + c + ":" + v + " ");
                                writer.WriteLine(FormatUtil.UnescapeXML(_verse_text));
                            }
                        }
                    }
            }
            writer.Close();

            percent_complete = 100;
        }


        private void processLine(XmlElement versedoc, string line)
        {
            if (line.IndexOf("#") != -1)
            {
                line = FormatUtil.EscapeXML(line);
                line = Regex.Replace(line, "([^#]+)#([0-9]+)#", "$1<gr str=\"$2\"></gr>");
            }

            if (line.IndexOf("+r/") != -1)
            {
                line = line.Replace("+r/", "<STYLE css=\"color:#ff0000\">");
                line = line.Replace("-r/", "</STYLE>");
            }
            line = postProcessContent(line);
            versedoc.InnerXml = line.Trim();
        }

        private string processExportVerse(XmlNode vers, int b)
        {
            if (GlobalMemory.getInstance().ParseCommentary)
            {
                _verse_text = vers.InnerXml;
                ///////////
                //red text
                if (_verse_text.IndexOf("<STYLE") != -1)
                {
                    _verse_text = _verse_text.Replace("<STYLE css=\"color:#ff0000\">", "");
                    _verse_text = _verse_text.Replace("</STYLE>", "");
                }

                if (_verse_text.IndexOf("<gr") != -1)
                {
                    _verse_text = Regex.Replace(_verse_text, "(<gr )([^s]*)(str=['\"]*)([^<'\"]*)['\"]*(>)(((?!</gr).)*)</gr>", "$6");
                }
                ///////////
                _verse_text = FormatUtil.StripHTML(_verse_text);
            }
            else
            {
                _verse_text = vers.InnerText;
            }
            return _verse_text;
        }
    }
}
