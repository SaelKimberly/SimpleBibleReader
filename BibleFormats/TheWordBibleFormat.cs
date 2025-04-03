using System;
using System.Windows.Forms;
using System.Xml;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;
using Ionic.Zlib;
using System.Reflection;
using System.Collections.Generic;
using RtfPipe.Tokens;


namespace Simple_Bible_Reader
{
	public class TheWordBibleFormat : BibleFormat
	{
		
		
		System.Xml.XmlElement rootdoc;
		System.Xml.XmlElement bookdoc;
		System.Xml.XmlElement chapdoc;
		System.Xml.XmlElement versedoc;
		System.Xml.XmlAttribute attrib;
		
		int percent_complete = 0;
        public System.Windows.Forms.RichTextBox rtb = new System.Windows.Forms.RichTextBox();

		public static readonly int Type_ONT = 0;
		public static readonly int Type_OT = 1;
		public static readonly int Type_NT = 2;
        public static readonly int Type_ONTX = 3;
        public static readonly int Type_OTX = 4;
        public static readonly int Type_NTX = 5;
		
		private int m_type;
       
        String m_lang = "eng";

        public TheWordBibleFormat(string file, int type) : base(file)
		{
			m_type = type;
		}
		
		public override void Load()
		{           
            if (m_type == Type_ONT || m_type == Type_OT || m_type == Type_NT)
            {
                //System.IO.StreamReader file_reader = System.IO.File.OpenText((string)FileName);
                System.IO.StreamReader file_reader = new StreamReader(new FileStream(this.FileName, FileMode.Open), System.Text.Encoding.Default);
                string str = file_reader.ReadToEnd();
                file_reader.Close();

                if (GlobalMemory.getInstance().ParseBible)
                {
                    str = Regex.Replace(str, "<RF>([^<]*)<Rf>", "");
                    str = str.Replace("<CM>", "");
                    str = Regex.Replace(str, "<NB>([^<]*)<Nb>", "");
                    str = Regex.Replace(str, "<FI>([^<]*)<Fi>", "<i>($1)</i>"); //Translator added words
                    str = Regex.Replace(str, "<WT[^>]*>", ""); //Remove Morphology Codes
                }

                System.IO.StringReader reader = new System.IO.StringReader(str);
                string line = null;
                int begin_book = -1;
                int end_book = -1;
                if (m_type == Type_ONT)
                {
                    begin_book = 1;
                    end_book = 66;
                }
                else if (m_type == Type_OT)
                {
                    begin_book = 1;
                    end_book = 39;
                }
                else if (m_type == Type_NT)
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
                    bool no_more_parsing = false;

                    for (int i = begin_book; i <= end_book; i++)
                    {
                        if (reader.Peek() == -1 || no_more_parsing)
                        {
                            break;
                        }
                        percent_complete = System.Convert.ToInt32((i * 100) / (end_book - begin_book));
                        bookdoc = BibleXmlDocument.CreateElement("BIBLEBOOK");
                        attrib = BibleXmlDocument.CreateAttribute("bnumber");
                        attrib.Value = i.ToString();
                        bookdoc.Attributes.Append(attrib);
                        rootdoc.AppendChild(bookdoc);

                        for (int j = 1; j <= FormatUtil.GetChapterCount(i); j++)
                        {
                            if (reader.Peek() == -1 || no_more_parsing)
                            {
                                break;
                            }
                            chapdoc = BibleXmlDocument.CreateElement("CHAPTER");
                            attrib = BibleXmlDocument.CreateAttribute("cnumber");
                            attrib.Value = j.ToString();
                            chapdoc.Attributes.Append(attrib);
                            bookdoc.AppendChild(chapdoc);
                            for (int k = 1; k <= FormatUtil.GetVerseCount(i, j); k++)
                            {
                                if (reader.Peek() == -1)
                                {
                                    no_more_parsing = true;
                                    break;
                                }
                                line = reader.ReadLine();
                                versedoc = BibleXmlDocument.CreateElement("VERS");
                                attrib = BibleXmlDocument.CreateAttribute("vnumber");
                                attrib.Value = k.ToString();

                                if (GlobalMemory.getInstance().ParseBible)
                                {
                                    try
                                    {
                                        processLine(versedoc, line);
                                    }
                                    catch (Exception em)
                                    {
                                        versedoc.InnerText = line;
                                    }
                                }
                                else
                                    versedoc.InnerXml = FormatUtil.EscapeXML(line);

                                versedoc.Attributes.Append(attrib);
                                chapdoc.AppendChild(versedoc);
                            }
                        }
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    //Themes.DarkMessageBoxShow(ex.Message);
                }
            }
            else if (m_type == Type_ONTX || m_type == Type_OTX || m_type == Type_NTX)
            {
                try
                {
                    using (BinaryReader reader = new BinaryReader(File.OpenRead((string)FileName)))
                    {
                        int pos = 0;
                        reader.ReadBytes(0x10);//skip irrelavant details.
                        pos += 0x10;
                        int books_to_read = reader.ReadByte();
                        pos += 1;
                        reader.ReadBytes(9);
                        pos += 9;

                        uint[] from = new uint[books_to_read];
                        uint[] to = new uint[books_to_read];
                        int[] book = new int[books_to_read];
                        // read position info
                        for (int i = 0; i < books_to_read; i++)
                        {
                            from[i] = reader.ReadUInt32();
                            //reader.ReadBytes(2);
                            to[i] = reader.ReadUInt32();
                            pos += 8;

                            reader.ReadBytes(9);
                            pos += 9;
                            book[i] = reader.ReadByte();
                            pos += 1;
                            reader.ReadBytes(0x1e);
                            pos += 0x1e;
                        }

                        BibleXmlDocument = new System.Xml.XmlDocument();
                        XmlDeclaration dec = BibleXmlDocument.CreateXmlDeclaration("1.0", null, null);
                        BibleXmlDocument.AppendChild(dec);
                        rootdoc = BibleXmlDocument.CreateElement("XMLBIBLE");
                        BibleXmlDocument.AppendChild(rootdoc);

                        // read actual data
                        string line;
                        int len = 0;
                        for (int i = 0; i < books_to_read; i++)
                        {
                            percent_complete = System.Convert.ToInt32((i * 100) / books_to_read);
                            int skip = (int)(from[i] - pos);
                            reader.ReadBytes(skip);
                            pos += skip;
                            // pointer at start of from[]
                            len = (int)to[i] - (int)from[i];
                            byte[] bytes = new byte[len];
                            bytes = reader.ReadBytes((int)(len));
                            pos += len;
                            string data = null;

                            try
                            {
                                data = Encoding.UTF8.GetString(zLibUncompress(SCOP.decryptSCOPBytes(bytes)));
                            }
                            catch (Exception)
                            {
                                //data = Encoding.UTF8.GetString(SCOP.decryptSCOPBytes(bytes));
                                data = "";
                            }
                            

                            StringReader str_reader = new StringReader(data);
                            //////////////// ---- populate XML
                            bookdoc = BibleXmlDocument.CreateElement("BIBLEBOOK");
                            attrib = BibleXmlDocument.CreateAttribute("bnumber");
                            attrib.Value = book[i].ToString();
                            bookdoc.Attributes.Append(attrib);
                            rootdoc.AppendChild(bookdoc);
                            for (int j = 1; j <= FormatUtil.GetChapterCount(book[i]); j++)
                            {
                                chapdoc = BibleXmlDocument.CreateElement("CHAPTER");
                                attrib = BibleXmlDocument.CreateAttribute("cnumber");
                                attrib.Value = j.ToString();
                                chapdoc.Attributes.Append(attrib);
                                bookdoc.AppendChild(chapdoc);
                                for (int k = 1; k <= FormatUtil.GetVerseCount(book[i], j); k++)
                                {
                                    line = str_reader.ReadLine();
                                    versedoc = BibleXmlDocument.CreateElement("VERS");
                                    attrib = BibleXmlDocument.CreateAttribute("vnumber");
                                    attrib.Value = k.ToString();
                                    try
                                    {
                                        processLine(versedoc, line);
                                    }
                                    catch (Exception)
                                    {
                                        versedoc.InnerText = line;
                                    }
                                    versedoc.Attributes.Append(attrib);
                                    chapdoc.AppendChild(versedoc);
                                }
                            }
                            str_reader.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Themes.DarkMessageBoxShow(ex.Message);
                }
            }
			percent_complete = 100;
			SetProcessAsComplete();
		}

        private void processLine(XmlElement versedoc, string line)
        {
            if (GlobalMemory.getInstance().ParseBible)
            {
                if (line != null)
                {
                    if (line.IndexOf("<FR>") != -1)
                    {
                        line = line.Replace("<FR>", "<STYLE css=\"color:#ff0000\">");
                        line = line.Replace("<Fr>", "</STYLE>");
                    }
                    // replace all remaining and unwanted.
                    line = Regex.Replace(line, "<[A-Z][A-Z]>([^<]*)<[A-Z][a-z]>", "$1");
                    line = Regex.Replace(line, "<[A-Z]([A-Z]|[a-z])>", "");


                    if (line.IndexOf("<W") != -1)
                    {
                        line = Regex.Replace(line, "( [^<]*)(<W[H|G])([0-9.xs*]*)(>)", "<gr str=\"$3\">$1</gr>", RegexOptions.IgnoreCase);
                        line = Regex.Replace(line, "(<W[H|G])([0-9.xs*]*)(>)", "<gr str=\"$2\">&nbsp;</gr>", RegexOptions.IgnoreCase);
                    }

                    if (line.IndexOf("<X") != -1 || line.IndexOf("<x") != -1)
                    {
                        line = line.Replace("<X>", "").Replace("<x>", "");
                    }

                    // replace all remaining
                    line = Regex.Replace(line, @"<[^>]+>", "");


                    if (!GlobalMemory.getInstance().parseBibleBkgndCleaner && GlobalMemory.getInstance().ParseBible)
                        line = postProcessContent(line);
                    versedoc.InnerXml = line;
                }
                else
                    versedoc.InnerXml = "";
            }
            else
            {
                versedoc.InnerXml = FormatUtil.EscapeXML(line);
            }
        }

        static void CopyStream(System.IO.Stream src, System.IO.Stream dest)
        {
            byte[] buffer = new byte[1024];
            int len = src.Read(buffer, 0, buffer.Length);
            while (len > 0)
            {
                dest.Write(buffer, 0, len);
                len = src.Read(buffer, 0, buffer.Length);
            }
            dest.Flush();
        }


        private byte[] zLibUncompress(byte[] inBytes)
        {
            MemoryStream msSinkCompressed = new System.IO.MemoryStream(inBytes);
            msSinkCompressed.Seek(0, System.IO.SeekOrigin.Begin);
            MemoryStream msSinkDecompressed = new System.IO.MemoryStream();
            ZlibStream  zOut = new ZlibStream(msSinkDecompressed, CompressionMode.Decompress, CompressionLevel.BestCompression, true);
            CopyStream(msSinkCompressed, zOut);
            return msSinkDecompressed.ToArray();
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
                return F08_THEWORD;
            }
        }


        public override List<string[]> getProperties()
        {
            if (kv_export.Count == 0)
            {
                kv_export.Add(new string[] { "Abbreviation:", BibleFormat.getInstance().ABBREVIATION });
                kv_export.Add(new string[] { "Description:", BibleFormat.getInstance().DESCRIPTION });
                kv_export.Add(new string[] { "Comments:", BibleFormat.getInstance().COMMENTS });
                kv_export.Add(new string[] { "Language:", m_lang });
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
                        m_lang = kv_export[i][1];
                        break;                }
            }
        }

        public override void ExportBible(string filename,int filter_idx)
        {
            if (filename.Equals(""))
                return;
            if (filename.EndsWith(".ont"))
                filename = filename.Substring(0, filename.Length - 4);
            if (filename.EndsWith(".nt"))
                filename = filename.Substring(0, filename.Length - 3);
            if (filename.EndsWith(".ot"))
                filename = filename.Substring(0, filename.Length - 3);
            if (filename.EndsWith(".ontx"))
                filename = filename.Substring(0, filename.Length - 5);
            if (filename.EndsWith(".ntx"))
                filename = filename.Substring(0, filename.Length - 4);
            if (filename.EndsWith(".otx"))
                filename = filename.Substring(0, filename.Length - 4);

            processProperties();
            XmlNodeList books = BibleXmlDocument.SelectNodes("/XMLBIBLE/BIBLEBOOK");
            StreamWriter writer = null;
            XmlNode vers = null;
            string _verse_text = "";
            percent_complete = 0;
            if (books.Count == 39)
            {
                writer = new System.IO.StreamWriter(filename + ".ot", false, Encoding.UTF8);
                for (int b = 1; b <= 39; b++)
                {
                    percent_complete = b * 100 / 39;
                    for (int c = 1; c <= FormatUtil.GetChapterCount(b); c++)
                        for (int v = 1; v <= FormatUtil.GetVerseCount(b, c); v++)
                        {
                            try
                            {
                                vers = books[b - 1].SelectSingleNode("CHAPTER[@cnumber='" + c.ToString() + "']/VERS[@vnumber='" + v.ToString() + "']");
                                _verse_text = processExportVerse(vers, b);
                                _verse_text = _verse_text.Replace("\r", " ").Replace("\n", " ").Replace("  ", " ");
                                writer.WriteLine(FormatUtil.UnescapeXML(_verse_text));
                            }
                            catch (Exception)
                            {
                                writer.WriteLine();
                            }
                        }
                }
            }
            else if (books.Count == 27)
            {
                writer = new System.IO.StreamWriter(filename + ".nt", false, Encoding.UTF8);
                for (int b = 40; b <= 66; b++)
                {
                    percent_complete = (b-39) * 100 / 27;
                    for (int c = 1; c <= FormatUtil.GetChapterCount(b); c++)
                        for (int v = 1; v <= FormatUtil.GetVerseCount(b, c); v++)
                        {
                            try
                            {
                                vers = books[b - 40].SelectSingleNode("CHAPTER[@cnumber='" + c.ToString() + "']/VERS[@vnumber='" + v.ToString() + "']");
                                if (vers != null)
                                {
                                    _verse_text = processExportVerse(vers, b);
                                    _verse_text = _verse_text.Replace("\r", " ").Replace("\n", " ").Replace("  ", " ");
                                    writer.WriteLine(FormatUtil.UnescapeXML(_verse_text));
                                }
                                else
                                {
                                    writer.WriteLine();
                                }
                            }
                            catch (Exception)
                            {
                                writer.WriteLine();
                            }
                        }
                }               
            }
            else if (books.Count == 66)
            {
                writer = new System.IO.StreamWriter(filename + ".ont", false,Encoding.UTF8);
                for (int b = 1; b <= 66; b++)
                {
                    percent_complete = b * 100 / 66;
                    for (int c = 1; c <= FormatUtil.GetChapterCount(b); c++)
                        for (int v = 1; v <= FormatUtil.GetVerseCount(b, c); v++)
                        {
                            try
                            {
                                vers = books[b - 1].SelectSingleNode("CHAPTER[@cnumber='" + c.ToString() + "']/VERS[@vnumber='" + v.ToString() + "']");
                                _verse_text = processExportVerse(vers, b);
                                _verse_text = _verse_text.Replace("\r", " ").Replace("\n", " ").Replace("  ", " ");
                                writer.WriteLine(FormatUtil.UnescapeXML(_verse_text));
                            }
                            catch (Exception)
                            {
                                writer.WriteLine();
                            }
                        }
                }        
            }
            else
            {               
                writer = new System.IO.StreamWriter(filename + ".ont", false,Encoding.UTF8);
                for (int b = 1; b <= 66; b++)
                {                    
                    percent_complete = b * 100 / 66;
                    for (int c = 1; c <= FormatUtil.GetChapterCount(b); c++)
                        for (int v = 1; v <= FormatUtil.GetVerseCount(b, c); v++)
                        {
                            vers = BibleXmlDocument.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber='" + b.ToString() + "']/CHAPTER[@cnumber='" + c.ToString() + "']/VERS[@vnumber='" + v.ToString() + "']");
                            if (vers == null)
                                writer.WriteLine();
                            else
                            {

                                _verse_text = processExportVerse(vers,b);

                                writer.WriteLine(FormatUtil.UnescapeXML(_verse_text));
                            }
                        }
                }     
            }
            percent_complete = 100;
            writer.WriteLine();
            writer.WriteLine("lang="+m_lang);
            writer.WriteLine("description="+DESCRIPTION);
            writer.WriteLine("short.title="+ABBREVIATION);
            writer.WriteLine("about="+COMMENTS);
            writer.WriteLine();
            writer.Close();
        }

        private string processExportVerse(XmlNode vers, int b)
        {
            string prefix = "G";
            string _verse_text = "";
            if (b < 40)
                prefix = "H";
            
            if (GlobalMemory.getInstance().ParseBible)
            {                
                _verse_text = vers.InnerXml;
                ///////////
                //red text
                if (_verse_text.IndexOf("<STYLE") != -1)
                {
                    _verse_text = _verse_text.Replace("<STYLE css=\"color:#ff0000\">", "<FR>");
                    _verse_text = _verse_text.Replace("</STYLE>", "<Fr>");
                }

                if (_verse_text.IndexOf("<gr") != -1)
                {
                    _verse_text = Regex.Replace(_verse_text, "(<gr )([^s]*)(str=['\"]*)([^<'\"]*)['\"]*(>)(((?!</gr).)*)</gr>", "$6<W" + prefix + "$4>");
                }
                ///////////
            }
            else
            {
                _verse_text = vers.InnerText;
            }
            return _verse_text;
        }
    }
}
