using System.Data;
using System;
using System.Xml;
using System.Data.SQLite;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Simple_Bible_Reader
{

    public class MySword4AndroidBibleFormat : BibleFormat
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

        bool gzipped = false;
        public System.Windows.Forms.RichTextBox rtb = new System.Windows.Forms.RichTextBox();
        
        String mVERSION = "1";
        String mFONT = "DEFAULT";
        String mOT = "1";
        String mNT = "1";
        String mAPOCRYPHA = "0";
        String mSTRONG = "0";

        public MySword4AndroidBibleFormat(string file, bool compressed)
            : base(file)
        {
            gzipped = compressed;
        }

        public override void Load()
        {
            BibleXmlDocument = new System.Xml.XmlDocument();
            XmlDeclaration dec = BibleXmlDocument.CreateXmlDeclaration("1.0", null, null);
            BibleXmlDocument.AppendChild(dec);
            rootdoc = BibleXmlDocument.CreateElement("XMLBIBLE");
            BibleXmlDocument.AppendChild(rootdoc);

            int bno = -1;
            percent_complete = 0;
            string tmp_file = this.FileName;
            if (gzipped)
            {
                tmp_file = this.FileName.Substring(0, this.FileName.Length - 3);
                //tmp_file = Path.GetTempFileName();
                DecompressFile(tmp_file, this.FileName);
            }

            try
            {
                DataTable dt = new DataTable();
                //string data_src = @"" + this.FileName;
                string data_src = @"" + tmp_file;
                SQLiteConnection cnn = new SQLiteConnection("Data Source=" + data_src);
                cnn.Open();
                SQLiteCommand mycommand = new SQLiteCommand(cnn);
                mycommand.CommandText = "SELECT Book, Chapter, Verse, Scripture FROM Bible ORDER BY Book, Chapter, Verse";
                SQLiteDataReader reader = mycommand.ExecuteReader();

                dt.Load(reader);
                mycommand.Dispose();
                reader.Dispose();
                cnn.Dispose();

                percent_complete = 50;
                for (int row = 0; row < dt.Rows.Count; row++)
                {

                    bno = int.Parse(dt.Rows[row][0].ToString());
                    if (bno > 66)
                        break;
                    percent_complete = 50 + ((_book_no) * 50) / 66;
                    _book_no = bno;
                    _chap_no = int.Parse(dt.Rows[row][1].ToString());
                    _verse_no = int.Parse(dt.Rows[row][2].ToString());
                    _verse_text = dt.Rows[row][3].ToString();
                    processLine();
                }
            }
            catch (Exception)
            {
            }
            if (isUnicode(_verse_text))
                CLEANING = false;
            ////////////////////////////////////////           
            SetProcessAsComplete();
            if (gzipped)
            {
                System.IO.File.Delete(tmp_file);
            }
        }


        private void processLine()
        {
            if (_book_no != p_book_no || (_chap_no == 1 && _verse_no == 1 && p_verse_no >= _verse_no))
            {
                bookdoc = (XmlElement)BibleXmlDocument.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber='" + _book_no + "']");
                if (bookdoc == null)
                {
                    bookdoc = BibleXmlDocument.CreateElement("BIBLEBOOK");
                    attrib = BibleXmlDocument.CreateAttribute("bnumber");
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
                    chapdoc = BibleXmlDocument.CreateElement("CHAPTER");
                    attrib = BibleXmlDocument.CreateAttribute("cnumber");
                    attrib.Value = _chap_no.ToString();
                    chapdoc.Attributes.Append(attrib);
                    bookdoc.AppendChild(chapdoc);
                }
            }
            if (_verse_no != p_verse_no || (_verse_no == p_verse_no && _chap_no != p_chap_no) || (_verse_no == p_verse_no && _book_no != p_book_no))
            {
                versedoc = BibleXmlDocument.CreateElement("VERS");
                attrib = BibleXmlDocument.CreateAttribute("vnumber");
                attrib.Value = _verse_no.ToString();
                if (GlobalMemory.getInstance().ParseBible)
                {
                    try
                    {
                        processLine(versedoc, _verse_text);
                    }
                    catch (Exception)
                    {
                        versedoc.InnerText = _verse_text;
                    }
                    //versedoc.InnerText = _verse_text;
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
                return percent_complete;
            }
        }

        public override int FilterIndex
        {
            get
            {
                return F19_MYSWORD;
            }
        }

        public bool isUnicode(string input)
        {
            if (Encoding.UTF8.GetBytes(input).Length == input.Length)
                return false;
            else
                return true;
        }


        public override List<string[]> getProperties()
        {
            if (kv_export.Count == 0)
            {
                kv_export.Add(new string[] { "Abbreviation:", BibleFormat.getInstance().ABBREVIATION });
                kv_export.Add(new string[] { "Description:", BibleFormat.getInstance().DESCRIPTION });
                kv_export.Add(new string[] { "Comments:", BibleFormat.getInstance().COMMENTS });
                kv_export.Add(new string[] { "Version:", mVERSION });
                kv_export.Add(new string[] { "Font:", mFONT });
                kv_export.Add(new string[] { "Old Testament:", mOT });
                kv_export.Add(new string[] { "New Testament:", mNT });
                kv_export.Add(new string[] { "Apocrypha:", mAPOCRYPHA});
                kv_export.Add(new string[] { "Strongs:", mSTRONG });
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
                    case "Version:":
                        mVERSION = kv_export[i][1];
                        break;
                    case "Font:":
                        mFONT = kv_export[i][1];
                        break;
                    case "Old Testament:":
                        mOT = kv_export[i][1];
                        break;
                    case "New Testament:":
                        mNT = kv_export[i][1];
                        break;
                    case "Apocrypha:":
                        mAPOCRYPHA = kv_export[i][1];
                        break;
                    case "Strongs:":
                        mSTRONG = kv_export[i][1];
                        break;
                }
            }
        }

        public override void ExportBible(string filename, int filter_idx)
        {
            if (!filename.EndsWith(".bbl.mybible.gz"))
            {
                filename = filename + ".bbl.mybible.gz";
            }
            if (filename.Equals(""))
            {
                return;
            }
            processProperties();
            string tmp_file = filename.Substring(0, filename.Length - 3);

            if (System.IO.File.Exists(tmp_file))
                System.IO.File.Delete(tmp_file);

            SQLiteConnection.CreateFile(tmp_file);
            percent_complete = 0;
            SQLiteConnection sqlConn = new SQLiteConnection();

            sqlConn.ConnectionString = "Data source = " + tmp_file;
            sqlConn.Open();
            SQLiteCommand sqlCommand = new SQLiteCommand(sqlConn);

            System.Text.StringBuilder command = new System.Text.StringBuilder();
            command.Append("CREATE TABLE 'Bible' (Book INT, Chapter INT, Verse INT, Scripture TEXT)");

            sqlCommand.CommandText = command.ToString();
            sqlCommand.ExecuteNonQuery();

            sqlCommand.Dispose();
            sqlCommand = new SQLiteCommand(sqlConn);

            command = new System.Text.StringBuilder();
            command.Append("CREATE TABLE 'Details' (Description NVARCHAR(250), Abbreviation NVARCHAR(50), Comments TEXT, Version INT, Font TEXT, RightToLeft BOOL, OT BOOL, NT BOOL, Apocrypha BOOL, Strong BOOL)");
            sqlCommand.CommandText = command.ToString();
            sqlCommand.ExecuteNonQuery();

            sqlCommand.Dispose();
            sqlCommand = new SQLiteCommand(sqlConn);

            command = new System.Text.StringBuilder();
            command.Append("CREATE INDEX BookChapterVerseIndex ON Bible (Book, Chapter, Verse)");
            sqlCommand.CommandText = command.ToString();
            sqlCommand.ExecuteNonQuery();

            sqlCommand.Dispose();
            sqlCommand = new SQLiteCommand(sqlConn);

            int dir = GlobalMemory.getInstance().Direction;
            command = new System.Text.StringBuilder();
            command.Append("INSERT INTO Details VALUES('" + DESCRIPTION + "','" + ABBREVIATION +
                "','" + COMMENTS + "',"+mVERSION+",'"+mFONT+"'," + dir.ToString() + ","+mOT+","+mNT+","+mAPOCRYPHA+","+mSTRONG+")");
            sqlCommand.CommandText = command.ToString();
            sqlCommand.ExecuteNonQuery();

            sqlCommand.Dispose();

            SQLiteBulkInsert sbi = new SQLiteBulkInsert(sqlConn, "Bible");
            sbi.AddParameter("Book", DbType.Int32);
            sbi.AddParameter("Chapter", DbType.Int32);
            sbi.AddParameter("Verse", DbType.Int32);
            sbi.AddParameter("Scripture", DbType.String);


            XmlNodeList books = BibleXmlDocument.SelectNodes("/XMLBIBLE/BIBLEBOOK");
            XmlNode vers = null;
            string _verse_text = "";
            for (int b = 1; b <= 66; b++)
            {
                percent_complete = b * 100 / 66;
                for (int c = 1; c <= FormatUtil.GetChapterCount(b); c++)
                    for (int v = 1; v <= FormatUtil.GetVerseCount(b, c); v++)
                    {
                        vers = BibleXmlDocument.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber='" + b.ToString() + "']/CHAPTER[@cnumber='" + c.ToString() + "']/VERS[@vnumber='" + v.ToString() + "']");
                        if (vers != null)
                        {
                            _verse_text = processExportVerse(vers, b);
                            //_verse_text = vers.InnerText;
                            _verse_text = FormatUtil.UnescapeXML(_verse_text);
                            sbi.Insert(new object[] { b.ToString(), c.ToString(), v.ToString(), _verse_text });
                        }
                    }
            }
            sbi.Flush();
            sbi.getSQLiteCommand().Dispose();

            sqlConn.Dispose();
            percent_complete = 100;
            //CompressFile(filename, tmp_file);
            //System.IO.File.Delete(tmp_file);
        }

        string GetRtfUnicodeEscapedString(string s)
        {
            var sb = new System.Text.StringBuilder();
            foreach (var c in s)
            {
                if (c <= 0x7f)
                    sb.Append(c);
                else
                    sb.Append("\\u" + Convert.ToUInt32(c) + "?");
            }
            return sb.ToString();
        }

        private void processLine(XmlElement versedoc, string line)
        {
            if (GlobalMemory.getInstance().ParseBible)
            {
                if (line.IndexOf("<RF") != -1)
                {
                    line = Regex.Replace(line, @"\<RF\>[^<]+\<Rf\>", "", RegexOptions.Multiline);
                }

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

                line = Regex.Replace(line, "<[A-Za-z0-9\\.]+>", "");

                if(!GlobalMemory.getInstance().parseBibleBkgndCleaner && GlobalMemory.getInstance().ParseBible)
                    line = postProcessContent(line);

                versedoc.InnerXml = line;
            }
            else
                versedoc.InnerText = FormatUtil.EscapeXML(line);
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
                _verse_text = vers.InnerText;
            return _verse_text;
        }

        void DecompressFile(string out_file, string in_file)
        {
            byte[] gzip = System.IO.File.ReadAllBytes(in_file);
            // Create a GZIP stream with decompression mode.
            // ... Then create a buffer and write into while reading from the GZIP stream.
            using (GZipStream stream = new GZipStream(new MemoryStream(gzip), CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    System.IO.File.WriteAllBytes(out_file, memory.ToArray());
                }
            }
        }

        void CompressFile(string out_file, string in_file)
        {
            byte[] b;
            using (FileStream f = new FileStream(in_file, FileMode.Open))
            {
                b = new byte[f.Length];
                f.Read(b, 0, (int)f.Length);
            }
            using (FileStream f2 = new FileStream(out_file, FileMode.Create))
            using (GZipStream gz = new GZipStream(f2, CompressionMode.Compress, false))
            {
                gz.Write(b, 0, b.Length);
            }
        }
    }
}
