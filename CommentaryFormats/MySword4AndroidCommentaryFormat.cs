using System.Data;
using System;
using System.Xml;
using System.Data.SQLite;
using System.IO;
using System.IO.Compression;

namespace Simple_Bible_Reader
{    

    public class MySword4AndroidCommentaryFormat : CommentaryFormat
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
        bool gzipped = false;
        
        int p_book_no = -1;
        int p_chap_no = -1;
        int p_verse_no = -1;

        int percent_complete = 0;

        public MySword4AndroidCommentaryFormat(string file, bool compressed)
            : base(file)
        {
            gzipped = compressed;
        }

        public override void Load()
        {
            
            CommentaryXmlDocument = new System.Xml.XmlDocument();
            XmlDeclaration dec = CommentaryXmlDocument.CreateXmlDeclaration("1.0", null, null);
            CommentaryXmlDocument.AppendChild(dec);
            rootdoc = CommentaryXmlDocument.CreateElement("XMLBIBLE");
            CommentaryXmlDocument.AppendChild(rootdoc);

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
                string data_src = @"" + tmp_file;
                SQLiteConnection cnn = new SQLiteConnection("Data Source=" + data_src);
                cnn.Open();
                SQLiteCommand mycommand = new SQLiteCommand(cnn);
                mycommand.CommandText = "SELECT book, chapter, fromverse, data FROM commentary ORDER BY book, chapter, fromverse";
                SQLiteDataReader reader = mycommand.ExecuteReader();

                dt.Load(reader);
                mycommand.Dispose();
                reader.Dispose();                
                //
                mycommand = new SQLiteCommand(cnn);
                mycommand.CommandText = "SELECT description,abbreviation FROM details";
                reader = mycommand.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    ABBREVIATION = (string)reader["abbreviation"];
                    DESCRIPTION = (string)reader["description"];
                }
                  
                //
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
                        _verse_text = ReplaceNonPrintableCharacters(_verse_text,' ');
                        processLine();
                }
            }
            catch (Exception)
            {
                // Catching exceptions                           
            } 
            ////////////////////////////////////////

            SetProcessAsComplete();
            if (gzipped)
            {
                System.IO.File.Delete(tmp_file);
            }
        }

        string ReplaceNonPrintableCharacters(string s, char replaceWith)
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                byte b = (byte)c;
                if (b < 32 && b!=10 && b!=13)
                    result.Append(replaceWith);
                else
                    result.Append(c);
            }
            return result.ToString();
        }


        private void processLine()
        {
            if (_book_no != p_book_no || (_chap_no == 1 && _verse_no == 1 && p_verse_no >= _verse_no))
            {
                bookdoc = (XmlElement)CommentaryXmlDocument.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber='"+_book_no+"']");
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
            if (_verse_no != p_verse_no || _chap_no != p_chap_no || _book_no != p_book_no)
            {
                versedoc = CommentaryXmlDocument.CreateElement("VERS");
                attrib = CommentaryXmlDocument.CreateAttribute("vnumber");
                attrib.Value = _verse_no.ToString();
                //
                if (GlobalMemory.getInstance().ParseCommentary)
                {
                    _verse_text = postProcessContent(_verse_text);
                    versedoc.InnerXml = _verse_text;
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

        /*
        public void changeRefLink(XmlNode node)
        {
            if (node.Name == "reflink")
            {
                string path = node.Attributes["target"].Value;
                string[] verse_ref = path.Split(" ".ToCharArray());
                string book = FormatUtil.getBookNo(verse_ref[0]).ToString();

                node.Attributes["target"].Value = book + ";" + verse_ref[1].Replace(":", ";");
            }
            else if (node.HasChildNodes)
            {
                foreach (XmlNode n in node.ChildNodes)
                {
                    changeRefLink(n);
                }
            }
        }
         */

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
                return 11;
            }
        }

        public override void ExportCommentary(string filename, int filter_idx)
        {
            if (!filename.EndsWith(".cmt.mybible.gz"))
            {
                filename = filename + ".cmt.mybible.gz";
            }
            if (filename.Equals(""))
            {
                return;
            }
            string tmp_file = filename.Substring(0, filename.Length - 3);

            if (System.IO.File.Exists(tmp_file))
                System.IO.File.Delete(tmp_file);
            
            SQLiteConnection.CreateFile(tmp_file);
            percent_complete=0;
            using (SQLiteConnection sqlConn = new SQLiteConnection())
            {
                sqlConn.ConnectionString = "Data source = " + tmp_file;
                sqlConn.Open();                

                using (SQLiteCommand sqlCommand = new SQLiteCommand(sqlConn))
                {
                    System.Text.StringBuilder command = new System.Text.StringBuilder();
                    command.Append("CREATE TABLE commentary(id INTEGER primary key autoincrement,book INTEGER,chapter INTEGER,fromverse INTEGER,toverse INTEGER,data TEXT)");

                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                }
                

                using (SQLiteCommand sqlCommand = new SQLiteCommand(sqlConn))
                {
                    System.Text.StringBuilder command = new System.Text.StringBuilder();
                    command.Append("CREATE TABLE details( title TEXT, abbreviation TEXT, description TEXT, comments TEXT, author TEXT, version TEXT, versiondate DATETIME, publishdate TEXT)");
                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                }
                using (SQLiteCommand sqlCommand = new SQLiteCommand(sqlConn))
                {
                    System.Text.StringBuilder command = new System.Text.StringBuilder();
                    command.Append("INSERT INTO details VALUES('" + ABBREVIATION.Replace("\'", "\'\'") + "','" + DESCRIPTION.Replace("\'", "\'\'") + "','" + ABBREVIATION.Replace("\'", "\'\'") + "','" + COMMENTS.Replace("\'", "\'\'") + "','NA','1.0','2010-02-20 00:00:00','1825')");
                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                }

                SQLiteBulkInsert sbi = new SQLiteBulkInsert(sqlConn, "commentary");
                sbi.AddParameter("book", DbType.Int32);
                sbi.AddParameter("chapter", DbType.Int32);
                sbi.AddParameter("fromverse", DbType.Int32);
                sbi.AddParameter("toverse", DbType.Int32);
                sbi.AddParameter("data", DbType.String);

                
                    XmlNodeList books = CommentaryXmlDocument.SelectNodes("/XMLBIBLE/BIBLEBOOK");
                    XmlNode vers = null;
                    string _verse_text = "";
                    for (int b = 1; b <= 66; b++)
                    {
                        percent_complete=b*100/66;
                        for (int c = 1; c <= FormatUtil.GetChapterCount(b); c++)
                            for (int v = 1; v <= FormatUtil.GetVerseCount(b, c); v++)
                            {
                                vers = CommentaryXmlDocument.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber='" + b.ToString() + "']/CHAPTER[@cnumber='" + c.ToString() + "']/VERS[@vnumber='" + v.ToString() + "']");
                                if (vers != null)
                                {
                                    _verse_text = vers.InnerText;                                    
                                    sbi.Insert(new object[] { b.ToString(), c.ToString(), v.ToString(), v.ToString(), _verse_text });
                                }
                             }
                    }
                sbi.Flush();
                sqlConn.Dispose();
                }
                percent_complete=100;
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
