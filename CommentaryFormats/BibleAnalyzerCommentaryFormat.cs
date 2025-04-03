using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;
using System.Xml;

namespace Simple_Bible_Reader
{    

    public class BibleAnalyzerCommentaryFormat : CommentaryFormat
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

        public BibleAnalyzerCommentaryFormat(string file)
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
            int tmp2 = -1;
            try
            {
                DataTable dt = new DataTable();
                string data_src = @"" + this.FileName;
                SQLiteConnection cnn = new SQLiteConnection("Data Source=" + data_src);
                cnn.Open();
                SQLiteCommand 
                //
                mycommand = new SQLiteCommand(cnn);
                mycommand.CommandText = "SELECT desc,abbr FROM title";
                SQLiteDataReader reader = mycommand.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    ABBREVIATION = (string)reader["abbr"];
                    DESCRIPTION = (string)reader["desc"];
                }
                mycommand.Dispose();
                reader.Dispose();
                //


                //Book, ChapterBegin, VerseBegin, Comments
                mycommand = new SQLiteCommand(cnn);
                mycommand.CommandText = "SELECT ref, comment FROM commentary ORDER BY id";
                reader = mycommand.ExecuteReader();
                dt.Load(reader);
                mycommand.Dispose();
                reader.Dispose();
               
                cnn.Dispose();

                string tmp = null;
                percent_complete = 50;
                for (int row = 0; row < dt.Rows.Count; row++)
                {
                    tmp = dt.Rows[row][0].ToString().Trim();
                    if (tmp.Length >= 3 && tmp.IndexOf(':')!=-1)
                        bno = FormatUtil.getBookNo(tmp.Substring(0, 3));
                    else
                        continue;

                    if (bno > 66)
                        break;
                    else if (bno == -1)
                        continue;
                    
                    _book_no = bno;

                    try
                    {
                        if (tmp.Length >= 4)
                            tmp = tmp.Substring(4);
                        else
                            continue;
                        percent_complete = 50 + ((_book_no) * 50) / 66;
                        tmp2 = tmp.IndexOf(':');
                        if (tmp2 != -1)
                        {
                            _chap_no = int.Parse(tmp.Substring(0, tmp2));
                            if (tmp.Length >= tmp2 + 1)
                                tmp = tmp.Substring(tmp2 + 1);

                            if (tmp.IndexOf('-') != -1)
                                _verse_no = int.Parse(tmp.Substring(0, tmp.IndexOf('-')));
                            else
                                _verse_no = int.Parse(tmp);

                            _verse_text = dt.Rows[row][1].ToString();
                            processLine();
                        }
                    }
                    catch (Exception) { }                    
                }
            }
            catch (Exception) { }
           
            ////////////////////////////////////////

            SetProcessAsComplete();
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
                    _verse_text = FormatUtil.EscapeXML(_verse_text);
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
                return 7;
            }
        }

        public override void ExportCommentary(string filename, int filter_idx)
        {
            if (!filename.EndsWith(".cmt"))
            {
                filename = filename + ".cmt";
            }
            if (filename.Equals(""))
            {
                return;
            }
            if (System.IO.File.Exists(filename))
                System.IO.File.Delete(filename);

            SQLiteConnection.CreateFile(filename);
            percent_complete=0;
            using (SQLiteConnection sqlConn = new SQLiteConnection())
            {
                sqlConn.ConnectionString = "Data source = "+filename;
                sqlConn.Open();

                using (SQLiteCommand sqlCommand = new SQLiteCommand(sqlConn))
                {
                    System.Text.StringBuilder command = new System.Text.StringBuilder();
                    command.Append("CREATE TABLE commentary(id INTEGER PRIMARY KEY, ref, comment)");
                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                }
                using (SQLiteCommand sqlCommand = new SQLiteCommand(sqlConn))
                {
                    System.Text.StringBuilder command = new System.Text.StringBuilder();
                    command.Append("CREATE TABLE reference(ref)");
                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                }
                using (SQLiteCommand sqlCommand = new SQLiteCommand(sqlConn))
                {
                    System.Text.StringBuilder command = new System.Text.StringBuilder();
                    command.Append("CREATE TABLE title(abbr, desc, info)");
                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                }
              
                using (SQLiteCommand sqlCommand = new SQLiteCommand(sqlConn))
                {
                    System.Text.StringBuilder command = new System.Text.StringBuilder();
                    command.Append("INSERT INTO title VALUES('" + ABBREVIATION.Replace("\'", "\'\'") + "','" + DESCRIPTION.Replace("\'", "\'\'") + "','" + COMMENTS.Replace("\'", "\'\'") + "')");
                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                }

                SQLiteBulkInsert sbi = new SQLiteBulkInsert(sqlConn, "commentary");
                sbi.AddParameter("id", DbType.Int32);
                sbi.AddParameter("ref", DbType.String);
                sbi.AddParameter("comment", DbType.String);
                SQLiteBulkInsert sbi2 = new SQLiteBulkInsert(sqlConn, "reference");
                sbi2.AddParameter("ref", DbType.String);
                
                XmlNodeList books = CommentaryXmlDocument.SelectNodes("/XMLBIBLE/BIBLEBOOK");
                XmlNode vers = null;
                string _verse_text = "";
                int id = 1;
                string mref="";
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
                                mref = FormatUtil.shortBookNames[b - 1] + " " + c + ":" + v;
                                sbi.Insert(new object[] { id.ToString(), mref.ToString(), _verse_text });
                                sbi2.Insert(new object[] { mref.ToString()});
                                id++;
                            }
                            }
                }
                sbi.Flush();
                sbi2.Flush();
                sqlConn.Dispose();
                }
                percent_complete=100;
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
        }
    }
