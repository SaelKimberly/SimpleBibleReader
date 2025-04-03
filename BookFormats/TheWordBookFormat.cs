using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;

using System;
using System.Collections;
using System.Windows.Forms;
using System.Xml;
using System.Data.OleDb;
using System.Text.RegularExpressions;
using System.Data.SQLite;
using System.IO;
using System.Text;
using Ionic.Zlib;

namespace Simple_Bible_Reader
{    

    public class TheWordBookFormat : BookFormat
    {

        System.Xml.XmlElement rootdoc;
        System.Xml.XmlElement bookdoc;
        System.Xml.XmlAttribute attrib;

        string _title_text;
        string _notes_text;

        public System.Windows.Forms.RichTextBox rtb = new System.Windows.Forms.RichTextBox();
        int percent_complete = 0;

        public TheWordBookFormat(string file)
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
           
            DataTable dt = new DataTable();
            string data_src = @"" + this.FileName;
            SQLiteConnection cnn = new SQLiteConnection("Data Source=" + data_src + ";UseUTF8Encoding=True;");
            cnn.Open();
            SQLiteCommand mycommand = new SQLiteCommand(cnn);
            SQLiteCommand mycommand2 = new SQLiteCommand(cnn);
            mycommand.CommandText = "select a.subject, b.data  from topics a,content b where a.id=b.topic_id order by a.rel_order,a.pid,a.id";
            mycommand2.CommandText = mycommand.CommandText;// for getbytes
            SQLiteDataReader reader = mycommand.ExecuteReader();
            SQLiteDataReader reader2 = mycommand2.ExecuteReader(); // for getbytes

            dt.Load(reader);
            mycommand.Dispose();            
            reader.Dispose();
            //////////////
            mycommand = new SQLiteCommand(cnn);
            mycommand.CommandText = "SELECT name,value FROM config";
            string name = null;
            reader = mycommand.ExecuteReader();
            bool do_encrypt = false;
            bool secure = false;
            bool compressed = false;
            string content_type = "text";
            while (reader.HasRows)
            {
                reader.Read();
                name = reader["name"].ToString();
                if (name == "abbrev")
                    ABBREVIATION = (string)reader["value"];
                if (name == "content.type")
                    content_type = (string)reader["value"];

                if (content_type == "rtf")
                    CLEANING = true;

                if (name == "title")
                    DESCRIPTION = (string)reader["value"];
                if (name == "do.encrypt")
                {
                    if ((string)reader["value"] == "1")
                        do_encrypt = true;
                }
                if (name == "compressed")
                {
                    if ((string)reader["value"] == "1")
                        compressed = true;
                }
                if (name == "secure")
                {
                    if ((string)reader["value"] == "1")
                        secure = true;
                }
            }
            mycommand.Dispose();
            reader.Dispose();
            //////////////
            byte[] bytes = null;
            long len=0;
            byte[] _bytes = null;
            byte[] _bytes2 = null;
            percent_complete = 0;
            if (!secure)
            {
                for (int row = 0; row < dt.Rows.Count; row++)
                {
                    //reader2.Read();

                    percent_complete = ((row) * 100) / dt.Rows.Count;
                    _title_text = dt.Rows[row][0].ToString();
                    _title_text = System.Security.SecurityElement.Escape(_title_text);
                    /*
                    len = reader2.GetBytes(1, 0, null, 0, 0);
                    _bytes=new byte[len];
                    len = reader2.GetBytes(1, 0, _bytes, 0, (int)len);
                    _notes_text = Encoding.UTF8.GetString(_bytes);
                    len = len - 0x1e;
                    if (_notes_text.Length > len)
                            _notes_text = _notes_text.Substring(0, (int)len);
                    */

                    if (dt.Rows[row][1] is byte[])
                    {
                        if (content_type == "rvf")
                            _notes_text = FormatUtil.Rvf2Text((byte[])dt.Rows[row][1]);                        
                        else
                            _notes_text = Encoding.UTF8.GetString((byte[])dt.Rows[row][1]);
                    }
                    else
                    {
                        if (content_type == "rvf")
                            _notes_text = FormatUtil.Rvf2Text(Encoding.UTF8.GetBytes(dt.Rows[row][1].ToString()));
                        else
                            _notes_text = dt.Rows[row][1].ToString();
                    }
                    _notes_text = System.Security.SecurityElement.Escape(_notes_text);
                    processLine();		
                }
             }
            else
            {
                //Themes.DarkMessageBoxShow("Encrypted TheWord book are not yet supported!\nWe hope to support it soon.", "Encrypted", MessageBoxButtons.OK, MessageBoxIcon.Stop);                
                
                for (int row = 0; row < dt.Rows.Count; row++)
                {
                    reader2.Read();
                    percent_complete = ((row) * 100) / dt.Rows.Count;
                    _title_text = dt.Rows[row][0].ToString();
                    _title_text = System.Security.SecurityElement.Escape(_title_text);
                    len = reader2.GetBytes(1, 0, null, 0, 0);
                    _bytes=new byte[len];
                    len = reader2.GetBytes(1, 0, _bytes, 0, (int)len);
                    try
                    {
                        for (byte i = 0; i < 255; i++)
                        {
                            _bytes2=SCOP.decryptSCOPBytes(_bytes, i);
                            if (_bytes2[0] == 0x78 && _bytes2[1] == 0xDA)
                            {                                
                                break;
                            }
                        }
                        
                        if (_bytes2[0] == 0x78 && _bytes2[1] == 0xDA)
                        {
                            //Console.WriteLine("Found!");
                            bytes = ZlibStream.UncompressBuffer(_bytes2);
                            if (content_type == "rvf")
                                _notes_text= FormatUtil.Rvf2Text(bytes);
                            else
                                _notes_text = Encoding.UTF8.GetString(bytes);                            
                        }
                        else
                        {
                            _notes_text = "";
                        }
                    }
                    catch (Exception)
                    {
                        _notes_text = "";
                    }
                    processLine();
                }
            }
            reader2.Dispose();
            cnn.Dispose();
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
            if (GlobalMemory.getInstance().autoSetParsingBook)
            {
                GlobalMemory.getInstance().ConvertRtfToHtmlBook = true;
            }
            if (GlobalMemory.getInstance().ParseBook)
            {
                if (!GlobalMemory.getInstance().parseBookBkgndCleaner)
                    _notes_text = postProcessContent(_notes_text);
                _notes_text = System.Security.SecurityElement.Escape(_notes_text);
                bookdoc.InnerText = _notes_text;
            }
            else
                bookdoc.InnerXml = FormatUtil.EscapeXML(_notes_text);
        }	

        public void changeRefLink(XmlNode node)
        {
            if (node.Name == "reflink")
            {
                string path = node.Attributes["target"].Value;
                string[] verse_ref = path.Split(";".ToCharArray());
                if (int.Parse(verse_ref[0]) > 0)
                {
                    string book = FormatUtil.shortBookNames[int.Parse(verse_ref[0]) - 1];
                    if (BibleFormat.getInstance() != null)
                        book = BibleFormat.getInstance().getBookName(int.Parse(verse_ref[0]));
                    node.InnerText = book + " " + verse_ref[1] + ":" + verse_ref[2];
                }
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
                return 3;
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
            ZlibStream zOut = new ZlibStream(msSinkDecompressed, CompressionMode.Decompress, CompressionLevel.BestCompression, true);
            CopyStream(msSinkCompressed, zOut);
            return msSinkDecompressed.ToArray();
        }

        public override void ExportBook(string filename, int filter_idx)
        {
            if (!filename.EndsWith(".gbk.twm"))
            {
                filename = filename + ".gbk.twm";
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
                    command.Append("CREATE TABLE topics(id integer primary key, pid integer default 0, subject text, rel_order, content_type string)");

                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                }


                using (SQLiteCommand sqlCommand = new SQLiteCommand(sqlConn))
                {
                    System.Text.StringBuilder command = new System.Text.StringBuilder();
                    command.Append("CREATE TABLE content (topic_id integer primary key,data text,data2 blob)");

                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                }

                using (SQLiteCommand sqlCommand = new SQLiteCommand(sqlConn))
                {
                    System.Text.StringBuilder command = new System.Text.StringBuilder();
                    command.Append("CREATE TABLE config (name text,value text)");
                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                }               

                using (SQLiteCommand sqlCommand = new SQLiteCommand(sqlConn))
                {
                    int dir=GlobalMemory.getInstance().Direction;
                    System.Text.StringBuilder command = new System.Text.StringBuilder();
                    command.Append("INSERT INTO config VALUES('title','" + filename + "')");
                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                }

                string fileonly = Path.GetFileName(filename);
                SQLiteBulkInsert sbi_config = new SQLiteBulkInsert(sqlConn, "config");
                sbi_config.AddParameter("name", DbType.String);
                sbi_config.AddParameter("value", DbType.String);
                sbi_config.Insert(new object[] { "content.type", "rtf" });
                sbi_config.Insert(new object[] { "type", "3" });
                sbi_config.Insert(new object[] { "schema.version", "1" });
                sbi_config.Insert(new object[] { "title", ABBREVIATION });
                sbi_config.Insert(new object[] { "description", DESCRIPTION });
                sbi_config.Insert(new object[] { "creator", "Simple Bible Reader (" + GlobalMemory.AUTHOR_WEBSITE + ")" });
                sbi_config.Insert(new object[] { "id", "g19" });
                sbi_config.Insert(new object[] { "about", COMMENTS });
                sbi_config.Insert(new object[] { "description.english", DESCRIPTION });
                sbi_config.Insert(new object[] { "categories", "Reference" });
                sbi_config.Insert(new object[] { "keywords", "" });
                sbi_config.Insert(new object[] { "title.english", "" });
                sbi_config.Insert(new object[] { "abbrev", ABBREVIATION });
                sbi_config.Insert(new object[] { "user", "0" });
                sbi_config.Insert(new object[] { "version.minor", "1" });
                sbi_config.Insert(new object[] { "editorial.comments", "" });
                sbi_config.Insert(new object[] { "version.date", "" });
                sbi_config.Insert(new object[] { "preserve.fonts", "0" });
                sbi_config.Insert(new object[] { "preserve.fonts.list", "" });
                sbi_config.Flush();



                SQLiteBulkInsert sbi_topics = new SQLiteBulkInsert(sqlConn, "topics");
                SQLiteBulkInsert sbi_content = new SQLiteBulkInsert(sqlConn, "content");

                sbi_topics.AddParameter("id", DbType.Int32);
                sbi_topics.AddParameter("subject", DbType.String);                

                sbi_content.AddParameter("topic_id", DbType.Int32);
                sbi_content.AddParameter("data", DbType.String);
                
                XmlNodeList titles = BookXmlDocument.SelectNodes("/XMLBOOK/NOTES/@title");
                XmlNode note = null;
                int topic_id = 1;
                string _verse_text = "";
                for (int i = 0; i < titles.Count;i++ )
                {
                    percent_complete = i * 100 / titles.Count;

                    note = BookXmlDocument.SelectSingleNode("/XMLBOOK/NOTES[@title='" + titles[i].Value + "']");
                    if (note != null)
                    {
                        sbi_topics.Insert(new object[] { topic_id, titles[i].Value});

                        _verse_text = note.InnerText;

                        sbi_content.Insert(new object[] { topic_id, _verse_text });
                        topic_id++;
                    }
                }
                sbi_topics.Flush();
                sbi_content.Flush();
                sqlConn.Dispose();
                }
                percent_complete=100;
            }
        }
    }
