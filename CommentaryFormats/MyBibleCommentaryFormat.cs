using System.Data;
using System;
using System.Xml;
using System.Data.SQLite;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace Simple_Bible_Reader
{    

    public class MyBibleCommentaryFormat : CommentaryFormat
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

        int[] mybible_booknumbers = { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 160, 190, 220, 230, 240, 250, 260, 290, 300, 310, 330, 340, 350, 360, 370, 380, 390, 400, 410, 420, 430, 440, 450, 460, 470, 480, 490, 500, 510, 520, 530, 540, 550, 560, 570, 580, 590, 600, 610, 620, 630, 640, 650, 660, 670, 680, 690, 700, 710, 720, 730 };


        public MyBibleCommentaryFormat(string file)
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

           int bno = -1;
           percent_complete = 0;
           string tmp_file = this.FileName;
            int mybible_bookno;
            try
            {
                DataTable dt = new DataTable();
                string data_src = @"" + tmp_file;
                SQLiteConnection cnn = new SQLiteConnection("Data Source=" + data_src);
                cnn.Open();
                SQLiteCommand mycommand = new SQLiteCommand(cnn);
                mycommand.CommandText = "SELECT book_number, chapter_number_from, verse_number_from, text FROM commentaries ORDER BY book_number, chapter_number_from, verse_number_from";
                SQLiteDataReader reader = mycommand.ExecuteReader();

                dt.Load(reader);
                mycommand.Dispose();
                reader.Dispose();                
                //
                mycommand = new SQLiteCommand(cnn);
                mycommand.CommandText = "SELECT name,value FROM info";
                reader = mycommand.ExecuteReader();
                string tmp = "";

                if (reader.HasRows)
                {
                    reader.Read();
                    tmp = (string)reader["name"];
                    if (tmp.ToLower() == "description")
                        DESCRIPTION = (string)reader["value"];
                }
                  
                //
                mycommand.Dispose();
                reader.Dispose();
                cnn.Dispose();

                percent_complete = 50;
                for (int row = 0; row < dt.Rows.Count; row++)
                {
                    mybible_bookno = int.Parse(dt.Rows[row][0].ToString());

                    bno = Array.FindIndex(mybible_booknumbers, book_no => book_no == mybible_bookno) + 1;


                    if (bno > 66)
                            break;
                        percent_complete = 50 + ((_book_no) * 50) / 66;
                        _book_no = bno;
                        _chap_no = int.Parse(dt.Rows[row][1].ToString());
                        _verse_no = int.Parse(dt.Rows[row][2].ToString());
                        if (_book_no != 0 && _chap_no != 0 && _verse_no != 0)
                        {
                            _verse_text = dt.Rows[row][3].ToString();
                            _verse_text = ReplaceNonPrintableCharacters(_verse_text, ' ');
                            processLine();
                        }
                    
                }
            }
            catch (Exception e)
            {
                // Catching exceptions                
            } 
            ////////////////////////////////////////

            SetProcessAsComplete();
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
                    _verse_text = Regex.Replace(_verse_text, @"<a[^h]+href='B:([0-9]+)\s([0-9]+):([0-9]+)[^']+'>([^<]+)</a>", @"[#reflink target=##$1 $2:$3##]#$4[#/reflink]#");
                    _verse_text = _verse_text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
                    _verse_text = _verse_text.Replace("[#", "<").Replace("]#", ">").Replace("##", "\"");
                    _verse_text = postProcessContent(_verse_text);
                    versedoc.InnerXml = _verse_text.Trim();                    
                    changeRefLink(versedoc);
                }
                else
                {
                    versedoc.InnerXml = FormatUtil.EscapeXML(_verse_text);
                }
                versedoc.Attributes.Append(attrib);
                chapdoc.AppendChild(versedoc);
            }

            p_book_no = _book_no;
            p_chap_no = _chap_no;
            p_verse_no = _verse_no;
        }

  

        public void changeRefLink(XmlNode node)
        {
            if (node.Name == "reflink")
            {
                string path = node.Attributes["target"].Value;
                string[] verse_ref = path.Split(" ".ToCharArray());
                int mybible_bookno = int.Parse(verse_ref[0]);
                int book_idx = Array.FindIndex(mybible_booknumbers, book_no => book_no == mybible_bookno) + 1;

                string book = book_idx.ToString();

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
                return 9;
            }
        }

        public override void ExportCommentary(string filename, int filter_idx)
        {
            if (!filename.ToLower().EndsWith(".commentaries.sqlite3"))
            {
                filename = filename + ".commentaries.SQLite3";
            }
            if (filename.Equals(""))
            {
                return;
            }
            string tmp_file = filename;

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
                    command.Append("CREATE TABLE info ( name TEXT NOT NULL, value TEXT NOT NULL, PRIMARY KEY ( name ))");

                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                }
                

                using (SQLiteCommand sqlCommand = new SQLiteCommand(sqlConn))
                {
                    System.Text.StringBuilder command = new System.Text.StringBuilder();
                    command.Append("CREATE TABLE commentaries ( book_number NUMERIC, chapter_number_from NUMERIC, verse_number_from NUMERIC, chapter_number_to NUMERIC, verse_number_to NUMERIC, marker TEXT, text TEXT NOT NULL DEFAULT '', PRIMARY KEY ( book_number, chapter_number_from, verse_number_from, marker ))");
                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                }



                SQLiteBulkInsert sbi1 = new SQLiteBulkInsert(sqlConn, "info");
                sbi1.AddParameter("name", DbType.String);
                sbi1.AddParameter("value", DbType.String);
                sbi1.Insert(new object[] { "description", DESCRIPTION });
                sbi1.Insert(new object[] { "detailed_info", DESCRIPTION });
                sbi1.Insert(new object[] { "language", "en" });
                sbi1.Insert(new object[] { "html_style", "p { margin-top: 0px; margin-bottom: 12px; } \r\na { text-decoration: none; }\r\na.B { text-decoration: none; color: %COLOR_GREEN%; } " });
                sbi1.Flush();
                sbi1.getSQLiteCommand().Dispose();
                

                SQLiteBulkInsert sbi = new SQLiteBulkInsert(sqlConn, "commentaries");
                sbi.AddParameter("book_number", DbType.Int32);
                sbi.AddParameter("chapter_number_from", DbType.Int32);
                sbi.AddParameter("verse_number_from", DbType.Int32);
                sbi.AddParameter("chapter_number_to", DbType.Int32);
                sbi.AddParameter("verse_number_to", DbType.Int32);
                sbi.AddParameter("marker", DbType.String);
                sbi.AddParameter("text", DbType.String);

                
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
                                    sbi.Insert(new object[] { b.ToString(), c.ToString(), v.ToString(), null,null,null, _verse_text });
                                }
                             }
                    }
                sbi.Flush();
                sbi.getSQLiteCommand().Dispose();
                sqlConn.Close();
                sqlConn.Dispose();                
                }
                percent_complete=100;
            }
        }
    }
