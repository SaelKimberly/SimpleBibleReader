using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;
using System.Xml;

namespace Simple_Bible_Reader
{    

    public class BibleAnalyzerBookFormat : BookFormat
    {

        System.Xml.XmlElement rootdoc;
        System.Xml.XmlElement bookdoc;
        System.Xml.XmlAttribute attrib;


        string _title_text;
        string _notes_text;

        public System.Windows.Forms.RichTextBox rtb = new System.Windows.Forms.RichTextBox();
        
        int percent_complete = 0;

        public BibleAnalyzerBookFormat(string file)
            : base(file)
        {
        }

        public override void Load()
        {
            
            BookXmlDocument = new System.Xml.XmlDocument();
            XmlDeclaration dec = BookXmlDocument.CreateXmlDeclaration("1.0", null, null);
            BookXmlDocument.AppendChild(dec);
            rootdoc = BookXmlDocument.CreateElement("XMLBOOK");
            BookXmlDocument.AppendChild(rootdoc);

           int bno = -1;
           percent_complete = 0;

            try
            {
                DataTable dt = new DataTable();
                string data_src = @"" + this.FileName;
                SQLiteConnection cnn = new SQLiteConnection("Data Source=" + data_src);
                cnn.Open();
                SQLiteCommand mycommand = new SQLiteCommand(cnn);
                //Book, ChapterBegin, VerseBegin, Comments
                mycommand.CommandText = "SELECT chapter, text FROM book ORDER BY id";
                SQLiteDataReader reader = mycommand.ExecuteReader();

                dt.Load(reader);
                mycommand.Dispose();
                reader.Dispose();

                //
                mycommand = new SQLiteCommand(cnn);
                mycommand.CommandText = "SELECT title,abbr FROM info";
                reader = mycommand.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    ABBREVIATION = (string)reader["abbr"];
                    DESCRIPTION = (string)reader["title"];
                }
                  
                //
                mycommand.Dispose();
                reader.Dispose();
                cnn.Dispose();
                string tmp = null;
                percent_complete = 50;
                for (int row = 0; row < dt.Rows.Count; row++)
                {
                    percent_complete = ((row) * 100) / dt.Rows.Count;
                    _title_text = dt.Rows[row][0].ToString();
                    _notes_text = dt.Rows[row][1].ToString();
                    _title_text = _title_text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");                    
                    processLine();
                }
            }
            catch (Exception) { }
           
            ////////////////////////////////////////

            SetProcessAsComplete();
        }


        private void processLine()
        {
            bookdoc = (XmlElement)BookXmlDocument.SelectSingleNode("/XMLBOOK/NOTES[@title='" + _title_text + "']");
            if (bookdoc == null)
            {
                bookdoc = BookXmlDocument.CreateElement("NOTES");
                attrib = BookXmlDocument.CreateAttribute("title");
                attrib.Value = _title_text.ToString();
                bookdoc.Attributes.Append(attrib);
                rootdoc.AppendChild(bookdoc);
            }
            if (GlobalMemory.getInstance().ParseBook)
            {
                if (!GlobalMemory.getInstance().parseBookBkgndCleaner)
                    _notes_text = postProcessContent(_notes_text);
                _notes_text = FormatUtil.EscapeXML(_notes_text);
                bookdoc.InnerText = _notes_text;
            }
            else
            {
                _notes_text = FormatUtil.EscapeXML(_notes_text);
                bookdoc.InnerXml = _notes_text;
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
                return 7;
            }
        }

        public override void ExportBook(string filename, int filter_idx)
        {
            if (!filename.EndsWith(".bk"))
            {
                filename = filename + ".bk";
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
                    command.Append("CREATE TABLE book(id INTEGER PRIMARY KEY, chapter, text)");
                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                }

                using (SQLiteCommand sqlCommand = new SQLiteCommand(sqlConn))
                {
                    System.Text.StringBuilder command = new System.Text.StringBuilder();
                    command.Append("CREATE TABLE info(abbr, title)");
                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                }
              
                using (SQLiteCommand sqlCommand = new SQLiteCommand(sqlConn))
                {
                    System.Text.StringBuilder command = new System.Text.StringBuilder();
                    command.Append("INSERT INTO info VALUES('" + ABBREVIATION.Replace("\'", "\'\'") + "','" + DESCRIPTION.Replace("\'", "\'\'") + "')");
                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                }

                SQLiteBulkInsert sbi = new SQLiteBulkInsert(sqlConn, "book");
                sbi.AddParameter("id", DbType.Int32);
                sbi.AddParameter("chapter", DbType.String);
                sbi.AddParameter("text", DbType.String);                
                
                ///
                XmlNodeList titles = BookXmlDocument.SelectNodes("/XMLBOOK/NOTES/@title");
                XmlNode note = null;
                string _note_text = "";
                int id = 0;
                for (int i = 0; i < titles.Count; i++)
                {
                    percent_complete = i * 100 / titles.Count;

                    note = BookXmlDocument.SelectSingleNode("/XMLBOOK/NOTES[@title='" + titles[i].Value + "']");
                    if (note != null)
                    {         
                        _note_text = note.InnerText;
                        sbi.Insert(new object[] { id,titles[i].Value, _note_text });
                        id++;
                    }
                }                
                sbi.Flush();
                sqlConn.Dispose();
                }                
                percent_complete=100;
            }
        }
    }
