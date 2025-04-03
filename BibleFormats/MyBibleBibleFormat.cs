using System.Data;
using System;
using System.Xml;
using System.Data.SQLite;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Simple_Bible_Reader
{    

    public class MyBibleBibleFormat : BibleFormat
    {

        System.Xml.XmlElement rootdoc;
        System.Xml.XmlElement bookdoc;
        System.Xml.XmlElement chapdoc;
        System.Xml.XmlElement versedoc;
        System.Xml.XmlAttribute attrib;
        System.Xml.XmlAttribute attrib2;

        int _book_no;
        int _chap_no;
        int _verse_no;
        string _verse_text;
        
        int p_book_no = -1;
        int p_chap_no = -1;
        int p_verse_no = -1;

        int percent_complete = 0;

        // -1 means, not book supported as in MyBibles modules format.
        // Ref: https://docs.google.com/document/d/12rf4Pqy13qhnAW31uKkaWNTBDTtRbNW0s7cM0vcimlA/edit

        int[] mybible_booknumbers = { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 160, 190, 220, 230, 240, 250, 260, 290, 300, 310, 330, 340, 350, 360, 370, 380, 390, 400, 410, 420, 430, 440, 450, 460, 470, 480, 490, 500, 510, 520, 530, 540, 550, 560, 570, 580, 590, 600, 610, 620, 630, 640, 650, 660, 670, 680, 690, 700, 710, 720, 730, /* Apocrypha (-1 means not supported by MyBible) -> */ 180,270,170,280,320,462,464,323,192,145,466,467,315,165,468,-1,-1,780,-1,-1,325,345,-1,305 };
        string[] mybible_bookcolors = { "#ccccff", "#ccccff", "#ccccff", "#ccccff", "#ccccff", "#ffcc99", "#ffcc99", "#ffcc99", "#ffcc99", "#ffcc99", "#ffcc99", "#ffcc99", "#ffcc99", "#ffcc99", "#ffcc99", "#ffcc99", "#ffcc99", "#66ff99", "#66ff99", "#66ff99", "#66ff99", "#66ff99", "#ff9fb4", "#ff9fb4", "#ff9fb4", "#ff9fb4", "#ff9fb4", "#ffff99", "#ffff99", "#ffff99", "#ffff99", "#ffff99", "#ffff99", "#ffff99", "#ffff99", "#ffff99", "#ffff99", "#ffff99", "#ffff99", "#ff6600", "#ff6600", "#ff6600", "#ff6600", "#00ffff", "#ffff00", "#ffff00", "#ffff00", "#ffff00", "#ffff00", "#ffff00", "#ffff00", "#ffff00", "#ffff00", "#ffff00", "#ffff00", "#ffff00", "#ffff00", "#ffff00", "#00ff00", "#00ff00", "#00ff00", "#00ff00", "#00ff00", "#00ff00", "#00ff00", "#ff7c80",  /* Apocrypha -> */ "#C0C0C0", "#C0C0C0", "#C0C0C0", "#C0C0C0", "#C0C0C0", "#C0C0C0", "#C0C0C0", "#C0C0C0", "#C0C0C0", "#C0C0C0", "#C0C0C0", "#C0C0C0", "#C0C0C0", "#C0C0C0", "#C0C0C0", "#C0C0C0", "#C0C0C0", "#C0C0C0", "#C0C0C0", "#C0C0C0", "#C0C0C0", "#C0C0C0", "#C0C0C0", "#C0C0C0" };
        string[] mybible_booknames = Localization.getBookNames();

        public MyBibleBibleFormat(string file)
            : base(file)
        {
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

            int mybible_bookno;
            try
            {
                DataTable dt = new DataTable();
                //string data_src = @"" + this.FileName;
                string data_src = @"" + tmp_file;
                SQLiteConnection cnn = new SQLiteConnection("Data Source=" + data_src);
                cnn.Open(); 
                SQLiteCommand mycommand = new SQLiteCommand(cnn);
                mycommand.CommandText = "SELECT book_number, chapter, verse, text FROM verses ORDER BY book_number, chapter, verse";
                SQLiteDataReader reader = mycommand.ExecuteReader();
                   
                dt.Load(reader);
                mycommand.Dispose();
                reader.Dispose();

                // books

                int m_book_number = -1;
                string m_book_color = null;
                string m_short_name = null;
                string m_long_name = null;
                int book_idx = -1;

                try
                {
                    mycommand = new SQLiteCommand(cnn);
                    mycommand.CommandText = "SELECT book_number,book_color,short_name,long_name FROM books order by book_number asc";
                    reader = mycommand.ExecuteReader();

                    while (reader.HasRows)
                    {
                        reader.Read();
                        m_book_number = int.Parse(reader["book_number"].ToString());
                        m_book_color = (string)reader["book_color"];
                        m_short_name = (string)reader["short_name"];
                        m_long_name = (string)reader["long_name"];

                        book_idx = Array.FindIndex(mybible_booknumbers, book_no => book_no == m_book_number);
                        
                        if (book_idx != -1)
                        {
                            mybible_bookcolors[book_idx] = m_book_color;
                            mybible_booknames[book_idx] = m_long_name;
                        }
                    }
                }
                catch (Exception e)
                {
                    //Themes.DarkMessageBoxShow(e.Message);
                }

                // info
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

                    if (bno != 0) // since we are adding one.
                    {
                        if (bno > mybible_booknumbers.Length)
                            break;
                        percent_complete = 50 + ((_book_no) * 50) / mybible_booknumbers.Length;
                        _book_no = bno;
                        _chap_no = int.Parse(dt.Rows[row][1].ToString());
                        _verse_no = int.Parse(dt.Rows[row][2].ToString());
                        _verse_text = dt.Rows[row][3].ToString();
                        processLine();
                    }
                }
            }
            catch (Exception e)
            {
                //Themes.DarkMessageBoxShow(e.Message);
            }

            if (isUnicode(_verse_text))
                CLEANING = false;
            ////////////////////////////////////////           
            SetProcessAsComplete();

        }

        private void processLine()
        {
            if (_book_no != p_book_no || (_chap_no == 1 && _verse_no == 1 && p_verse_no >= _verse_no))
            {
                bookdoc = (XmlElement)BibleXmlDocument.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber='"+_book_no+"']");
                if (bookdoc == null)
                {
                    bookdoc = BibleXmlDocument.CreateElement("BIBLEBOOK");
                    attrib = BibleXmlDocument.CreateAttribute("bnumber");
                    attrib.Value = _book_no.ToString();
                    bookdoc.Attributes.Append(attrib);
                    
                    attrib2 = BibleXmlDocument.CreateAttribute("bname");
                    attrib2.Value = mybible_booknames[_book_no-1];
                    bookdoc.Attributes.Append(attrib2);
                    //
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
                return F28_MYBIBLE;
            }
        }

        public bool isUnicode(string input)
        {            
            if (Encoding.UTF8.GetBytes(input).Length == input.Length)
                return false;
            else
                return true;
        }

        public override void ExportBible(string filename, int filter_idx)
        {
            if (!filename.ToLower().EndsWith(".sqlite3"))
            {
                filename = filename + ".SQLite3";
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
            SQLiteConnection sqlConn = new SQLiteConnection();
          
            sqlConn.ConnectionString = "Data source = " + tmp_file;                
            sqlConn.Open();
            
            SQLiteCommand sqlCommand = new SQLiteCommand(sqlConn);
            System.Text.StringBuilder command = new System.Text.StringBuilder();
            command.Append("CREATE TABLE books (book_color  TEXT, book_number INTEGER, short_name  TEXT, long_name TEXT)");
            sqlCommand.CommandText = command.ToString();
            sqlCommand.ExecuteNonQuery();                    
                
            sqlCommand.Dispose();

            sqlCommand = new SQLiteCommand(sqlConn);
            command = new System.Text.StringBuilder();
            command.Append("CREATE TABLE info (name  TEXT, value TEXT)");
            sqlCommand.CommandText = command.ToString();
            sqlCommand.ExecuteNonQuery();                                    
            sqlCommand.Dispose();

            sqlCommand = new SQLiteCommand(sqlConn);
            command = new System.Text.StringBuilder();
            command.Append("CREATE TABLE verses (book_number NUMERIC, chapter  NUMERIC, verse NUMERIC, text TEXT)");
            sqlCommand.CommandText = command.ToString();
            sqlCommand.ExecuteNonQuery();    
            sqlCommand.Dispose();

            int dir = GlobalMemory.getInstance().Direction;
            SQLiteBulkInsert sbi1 = new SQLiteBulkInsert(sqlConn, "info");
            sbi1.AddParameter("name", DbType.String);
            sbi1.AddParameter("value", DbType.String);
            sbi1.Insert(new object[] { "description", DESCRIPTION });
            sbi1.Insert(new object[] { "chapter_string", "Chapter" });
            sbi1.Insert(new object[] { "strong_numbers", "false" });
            sbi1.Insert(new object[] { "russian_numbering", "false" });
            if(dir ==GlobalMemory.DIRECTION_RTL)
                sbi1.Insert(new object[] { "right_to_left", "true" });
            else
                sbi1.Insert(new object[] { "right_to_left", "false" });
            sbi1.Insert(new object[] { "language", "en" });
            sbi1.Insert(new object[] { "contains_accents", "false" });
            sbi1.Insert(new object[] { "detailed_info", DESCRIPTION });
            sbi1.Insert(new object[] { "introduction_string", "About edition" });
            sbi1.Insert(new object[] { "chapter_string_ps", "Psalm" });
            sbi1.Insert(new object[] { "history_of_changes", "" });
            sbi1.Flush();
            sbi1.getSQLiteCommand().Dispose();





            SQLiteBulkInsert sbi = new SQLiteBulkInsert(sqlConn, "verses");
            sbi.AddParameter("book_number", DbType.Int32);
            sbi.AddParameter("chapter", DbType.Int32);
            sbi.AddParameter("verse", DbType.Int32);
            sbi.AddParameter("text", DbType.String);

                
            XmlNodeList books = BibleXmlDocument.SelectNodes("/XMLBIBLE/BIBLEBOOK");
            XmlNode vers = null;
            string _verse_text = "";            
            for (int b = 1; b <= mybible_booknumbers.Length; b++)
            {
                percent_complete=b*100/mybible_booknumbers.Length;
                for (int c = 1; c <= FormatUtil.GetChapterCount2(b); c++)
                    for (int v = 1; v <= FormatUtil.GetVerseCount2(b, c); v++)
                    {
                        vers = BibleXmlDocument.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber='" + b.ToString() + "']/CHAPTER[@cnumber='" + c.ToString() + "']/VERS[@vnumber='" + v.ToString() + "']");
                        if (vers != null)
                        {
                            _verse_text = processExportVerse(vers, b);
                            //_verse_text = vers.InnerText;
                            // required to make sure the sqlite data contains html tagsif it supports and if plugins were used to process
                            
                            _verse_text=FormatUtil.UnescapeXML(_verse_text);
                            
                            sbi.Insert(new object[] { mybible_booknumbers[b-1].ToString(), c.ToString(), v.ToString(), _verse_text });
                        }
                        }
            }
            sbi.Flush();
            sbi.getSQLiteCommand().Dispose();


            SQLiteBulkInsert sbi2 = new SQLiteBulkInsert(sqlConn, "books");
            sbi2.AddParameter("book_color", DbType.String);
            sbi2.AddParameter("book_number", DbType.Int32);
            sbi2.AddParameter("short_name", DbType.String);
            sbi2.AddParameter("long_name", DbType.String);
            string bname;
            XmlElement bname_elmt;
            for (int i=0;i<mybible_booknumbers.Length;i++)
            {
                try
                {
                    bname_elmt = (XmlElement)BibleXmlDocument.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber='" + (i + 1).ToString() + "']");
                    bname = bname_elmt.GetAttribute("bname").ToString();
                }
                catch (Exception)
                {
                    bname = FormatUtil.longBookNames[i];
                }
                
                sbi2.Insert(new object[] { mybible_bookcolors[i], mybible_booknumbers[i], FormatUtil.shortBookNames[i], bname });
            }
            sbi2.Flush();
            sbi2.getSQLiteCommand().Dispose();

            sqlConn.Dispose();
            percent_complete=100;      
        }
       

        private void processLine(XmlElement versedoc, string line)
        {

            // replace all remaining and unwanted.
            line = line.Replace("<pb/>", "");
            line = line.Replace("<br/>", "");

            // remove everything except J (red text) and S (strongs)
            line = Regex.Replace(line, "<[\\/]{0,1}[^JS]>", "");

            if (line.IndexOf("<S>") != -1)
            {                    
                line = Regex.Replace(line, "( [^<]*)<S>([0-9]+)</S>", "<gr str=\"$2\">$1</gr>");
                line = Regex.Replace(line, "<S>([0-9]+)</S>", "<gr str=\"$1\">&nbsp;</gr>");
            }

            if (line.IndexOf("<J>") != -1)
            {
                line = line.Replace("<J>", "<STYLE css=\"color:#ff0000\">");
                line = line.Replace("</J>", "</STYLE>");
            }

            if(!GlobalMemory.getInstance().parseBibleBkgndCleaner && GlobalMemory.getInstance().ParseBible)
                line = postProcessContent(line);

            versedoc.InnerXml = line.Trim();

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
                    _verse_text = _verse_text.Replace("<STYLE css=\"color:#ff0000\">", "<J>");
                    _verse_text = _verse_text.Replace("</STYLE>", "</J>");
                }

                if (_verse_text.IndexOf("<gr") != -1)
                {
                    _verse_text = Regex.Replace(_verse_text, "(<gr )([^s]*)(str=['\"]*)([^<'\"]*)['\"]*(>)(((?!</gr).)*)</gr>", "$6<S>$4</S>");
                }
                ///////////
            }
            else
                _verse_text = vers.InnerText;
            return _verse_text;
        }
    }
}
