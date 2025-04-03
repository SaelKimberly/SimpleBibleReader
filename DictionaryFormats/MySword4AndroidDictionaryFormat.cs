using System.Data;
using System;
using System.Xml;
using System.Data.SQLite;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace Simple_Bible_Reader
{    

    public class MySword4AndroidDictionaryFormat : DictionaryFormat
    {

        System.Xml.XmlElement rootdoc;
        System.Xml.XmlElement item_doc;
        System.Xml.XmlElement desc_doc;
        System.Xml.XmlAttribute attrib_id;

        public System.Windows.Forms.RichTextBox rtb_dct = new System.Windows.Forms.RichTextBox();
        bool gzipped = false;
        int percent_complete = 0;

        public MySword4AndroidDictionaryFormat(string file, bool compressed)
            : base(file)
        {
            gzipped = compressed;
        }

        public override void Load()
        {
            DictionaryXmlDocument = new System.Xml.XmlDocument();
            XmlDeclaration dec = DictionaryXmlDocument.CreateXmlDeclaration("1.0", null, null);
            DictionaryXmlDocument.AppendChild(dec);
            rootdoc = DictionaryXmlDocument.CreateElement("dictionary");
            DictionaryXmlDocument.AppendChild(rootdoc);

            string id = null;
            string item = null;
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
                mycommand.CommandText = "SELECT word, data FROM dictionary ORDER BY word";
                SQLiteDataReader reader = mycommand.ExecuteReader();

                dt.Load(reader);
                mycommand.Dispose();
                reader.Dispose();
                cnn.Dispose();

                percent_complete = 0;
                for (int row = 0; row < dt.Rows.Count; row++)
                {
                    id = dt.Rows[row][0].ToString();
                    item = dt.Rows[row][1].ToString();                       
                    percent_complete = (row * 100) / dt.Rows.Count;

                    item_doc = DictionaryXmlDocument.CreateElement("item");
                    attrib_id = DictionaryXmlDocument.CreateAttribute("id");
                    attrib_id.Value = id;
                    item_doc.Attributes.Append(attrib_id);
                    desc_doc = DictionaryXmlDocument.CreateElement("description");
                    //
                    if (GlobalMemory.getInstance().ParseDictionary)
                    {
                        item = postProcessContent(item);
                        desc_doc.InnerXml = FormatUtil.EscapeXML(item);
                    }
                    else
                        desc_doc.InnerXml = FormatUtil.EscapeXML(item);

                    item_doc.AppendChild(desc_doc);
                    rootdoc.AppendChild(item_doc);
                }
                if (Regex.IsMatch(id, "[G|H][0-9]*", RegexOptions.IgnoreCase))
                    STRONG = "1";
            }
            catch (Exception ex)
            {
                // Catching exceptions           
                //Themes.DarkMessageBoxShow(ex.Message);
            }

            ////////////////////////////////////////

            SetProcessAsComplete();
            if (gzipped)
            {
                System.IO.File.Delete(tmp_file);
            }
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

        public override void ExportDictionary(string filename, int filter_idx)
        {
            if (!filename.EndsWith(".dct.mybible.gz"))
            {
                filename = filename + ".dct.mybible.gz";
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
                    command.Append("CREATE TABLE dictionary(word TEXT primary key collate nocase, data TEXT)");
                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                }

                using (SQLiteCommand sqlCommand = new SQLiteCommand(sqlConn))
                {
                    System.Text.StringBuilder command = new System.Text.StringBuilder();
                    command.Append("CREATE TABLE details(title TEXT,abbreviation TEXT,description TEXT,comments TEXT,author TEXT,strong INTEGER,version TEXT,versiondate DATETIME,publishdate TEXT,righttoleft INT default 0)");
                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                }
                //
                using (SQLiteCommand sqlCommand = new SQLiteCommand(sqlConn))
                {
                    int dir=GlobalMemory.getInstance().Direction;
                    System.Text.StringBuilder command = new System.Text.StringBuilder();
                    command.Append("INSERT INTO details VALUES('" + ABBREVIATION + "','" + DESCRIPTION + "','" + ABBREVIATION + "','" + COMMENTS + "','N/A',0,'1.0','2012-02-23 00:00:00','2012-02-23 00:00:00'," + dir.ToString() + ")");
                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                }


                SQLiteBulkInsert sbi = new SQLiteBulkInsert(sqlConn, "dictionary");
                sbi.AddParameter("word", DbType.String);
                sbi.AddParameter("data", DbType.String);

                
                XmlNodeList items = DictionaryXmlDocument.SelectNodes("/dictionary/item");
                string item = null;
                string id = null;
                System.Collections.ArrayList list = new System.Collections.ArrayList();
                int duplicate = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    id = items[i].Attributes["id"].Value;
                    if (list.Contains(id))
                    {
                        id = id +"_"+ duplicate.ToString();
                        duplicate++;
                    }
                    list.Add(id);
                    item = FormatUtil.UnescapeXML(items[i].InnerText); // some seems to have escaped html
                    sbi.Insert(new object[] { id, item });
                }
                list.Clear();
                sbi.Flush();
                sqlConn.Dispose();
                }
                percent_complete=100;
                //CompressFile(filename, tmp_file);
                //System.IO.File.Delete(tmp_file);
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
