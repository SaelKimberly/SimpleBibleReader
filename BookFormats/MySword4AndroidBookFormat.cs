using System.Data;
using System;
using System.Xml;
using System.Data.SQLite;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;

namespace Simple_Bible_Reader
{    

    public class MySword4AndroidBookFormat : BookFormat
    {

        System.Xml.XmlElement rootdoc;
        System.Xml.XmlElement bookdoc;
        System.Xml.XmlAttribute attrib;

        bool gzipped = false;

        string _title_text;
        string _notes_text;

        public System.Windows.Forms.RichTextBox rtb = new System.Windows.Forms.RichTextBox();

        int percent_complete = 0;

        public MySword4AndroidBookFormat(string file, bool compressed)
            : base(file)
        {
            gzipped = compressed;
        }

        public override void Load()
        {
            BookXmlDocument = new System.Xml.XmlDocument();
            XmlDeclaration dec = BookXmlDocument.CreateXmlDeclaration("1.0", null, null);
            BookXmlDocument.AppendChild(dec);
            rootdoc = BookXmlDocument.CreateElement("XMLBOOK");
            BookXmlDocument.AppendChild(rootdoc);

            string tmp_file = this.FileName;
            if (gzipped)
            {
                tmp_file = this.FileName.Substring(0, this.FileName.Length - 3);
                //tmp_file = Path.GetTempFileName();
                DecompressFile(tmp_file, this.FileName);
            }  

            percent_complete = 0;

            try
            {
                DataTable dt = new DataTable();
                string data_src = @"" + tmp_file;
                SQLiteConnection cnn = new SQLiteConnection("Data Source=" + data_src);
                cnn.Open();
                SQLiteCommand mycommand = new SQLiteCommand(cnn);
                //Book, ChapterBegin, VerseBegin, Comments
                mycommand.CommandText = "SELECT title, content FROM journal ORDER BY relativeorder";
                SQLiteDataReader reader = mycommand.ExecuteReader();

                dt.Load(reader);
                mycommand.Dispose();
                reader.Dispose();

                //
                mycommand = new SQLiteCommand(cnn);
                mycommand.CommandText = "SELECT title,abbreviation FROM details";
                reader = mycommand.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    ABBREVIATION = (string)reader["abbreviation"];
                    DESCRIPTION = (string)reader["title"];
                }

                //
                mycommand.Dispose();
                reader.Dispose();
                cnn.Dispose();
                percent_complete = 50;
                for (int row = 0; row < dt.Rows.Count; row++)
                {
                    percent_complete = ((row) * 100) / dt.Rows.Count;
                    _title_text = System.Security.SecurityElement.Escape(dt.Rows[row][0].ToString());
                    _notes_text = dt.Rows[row][1].ToString();
                    processLine();
                }
            }
            catch (Exception ex) {
            }

            ////////////////////////////////////////

            SetProcessAsComplete();
            if (gzipped)
            {
                System.IO.File.Delete(tmp_file);
            }
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
                GlobalMemory.getInstance().stripHtmlTagsBook = true;
                if (!GlobalMemory.getInstance().parseBookBkgndCleaner)
                    _notes_text = postProcessContent(_notes_text);
                bookdoc.InnerText = _notes_text;
            }
            else
                bookdoc.InnerXml = FormatUtil.EscapeXML(_notes_text);
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

        public override void ExportBook(string filename, int filter_idx)
        {
            if (!filename.EndsWith(".bok.mybible.gz"))
            {
                filename = filename + ".bok.mybible.gz";
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
                    command.Append("CREATE TABLE data(rowid INTEGER primary key autoincrement, id TEXT collate nocase, description TEXT collate nocase, date DATETIME, filename TEXT, content BLOB)");

                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                }
                

                using (SQLiteCommand sqlCommand = new SQLiteCommand(sqlConn))
                {
                    System.Text.StringBuilder command = new System.Text.StringBuilder();
                    command.Append("CREATE TABLE details(name TEXT, title TEXT, abbreviation TEXT, author TEXT, description TEXT, comments TEXT, version TEXT, versiondate DATETIME, publishdate TEXT, readonly BOOL, customcss TEXT, righttoleft INT default 0)");
                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                }
                
                using (SQLiteCommand sqlCommand = new SQLiteCommand(sqlConn))
                {
                    System.Text.StringBuilder command = new System.Text.StringBuilder();
                    command.Append("CREATE TABLE journal(rowid INTEGER primary key autoincrement, id TEXT collate nocase, title TEXT collate nocase, date DATETIME, tags TEXT, content TEXT, relativeorder INT default 0, hidden INT default 0)");
                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                }
                using (SQLiteCommand sqlCommand = new SQLiteCommand(sqlConn))
                {
                    System.Text.StringBuilder command = new System.Text.StringBuilder();
                    int rtf = 0;
                    if(GlobalMemory.getInstance().Direction==GlobalMemory.DIRECTION_RTL)
                        rtf = 1;
                    command.Append("INSERT INTO details VALUES('" + ABBREVIATION.Replace("\'", "\'\'") + "','" + ABBREVIATION.Replace("\'", "\'\'") + "','" + ABBREVIATION.Replace("\'", "\'\'") + "',' ','" + DESCRIPTION.Replace("\'", "\'\'") + "','" + COMMENTS.Replace("\'", "\'\'") + "','1.0','2012-05-05 00:00:00','0000','1',' ','"+rtf.ToString()+"')");
                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                }

                SQLiteBulkInsert sbi = new SQLiteBulkInsert(sqlConn, "journal");
                sbi.AddParameter("id", DbType.String);
                sbi.AddParameter("title", DbType.String);
                sbi.AddParameter("content", DbType.String);
                sbi.AddParameter("relativeorder", DbType.Int32);
                
        
                
                ///
                XmlNodeList titles = BookXmlDocument.SelectNodes("/XMLBOOK/NOTES/@title");
                XmlNode note = null;
                string _note_text = "";
                int order = 0;
                for (int i = 0; i < titles.Count; i++)
                {
                    percent_complete = i * 100 / titles.Count;

                    note = BookXmlDocument.SelectSingleNode("/XMLBOOK/NOTES[@title='" + titles[i].Value + "']");
                    if (note != null)
                    {         
                        _note_text = note.InnerText;                                               
                        sbi.Insert(new object[] { titles[i].Value, titles[i].Value, _note_text, order });
                        order++;
                    }
                }                
                sbi.Flush();
                sqlConn.Dispose();
                }                
                percent_complete=100;
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
