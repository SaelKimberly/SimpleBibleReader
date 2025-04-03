using System.Data;
using System;
using System.Xml;
using System.Data.SQLite;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Simple_Bible_Reader
{    

    public class MyBibleDictionaryFormat : DictionaryFormat
    {

        System.Xml.XmlElement rootdoc;
        System.Xml.XmlElement item_doc;
        System.Xml.XmlElement desc_doc;
        System.Xml.XmlAttribute attrib_id;

        public System.Windows.Forms.RichTextBox rtb_dct = new System.Windows.Forms.RichTextBox();

        int percent_complete = 0;
        int[] mybible_booknumbers = { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 160, 190, 220, 230, 240, 250, 260, 290, 300, 310, 330, 340, 350, 360, 370, 380, 390, 400, 410, 420, 430, 440, 450, 460, 470, 480, 490, 500, 510, 520, 530, 540, 550, 560, 570, 580, 590, 600, 610, 620, 630, 640, 650, 660, 670, 680, 690, 700, 710, 720, 730 };

        public MyBibleDictionaryFormat(string file)
            : base(file)
        {

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

           
            try
            {
                DataTable dt = new DataTable();
                string data_src = @"" + tmp_file;
                SQLiteConnection cnn = new SQLiteConnection("Data Source=" + data_src);
                cnn.Open();
                SQLiteCommand mycommand = new SQLiteCommand(cnn);
                mycommand.CommandText = "SELECT topic, definition FROM dictionary ORDER BY topic";
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
                        item = Regex.Replace(item, @"<a[^h]+href='B:([0-9]+)\s([0-9]+):([0-9]+)[^']+'>([^<]+)</a>", @"[#reflink target=##$1 $2:$3##]#$4[#/reflink]#");
                        
                        // replace all remaining and unwanted.
                        item = Regex.Replace(item, "<[A-Z][A-Z]>([^<]*)<[A-Z][a-z]>", "$1");
                        item = Regex.Replace(item, "<[A-Z]([A-Z]|[a-z])>", "");

                        item = item.Replace("[#", "<").Replace("]#", ">").Replace("##", "\"");
                        item = postProcessContent(item);
                        desc_doc.InnerXml = FormatUtil.EscapeXML(item.Trim());
                        changeRefLink(desc_doc);
                    }
                    else
                    {
                        desc_doc.InnerXml = FormatUtil.EscapeXML(item);
                    }
                    item_doc.AppendChild(desc_doc);
                    rootdoc.AppendChild(item_doc);
                }
                if (Regex.IsMatch(id, "[G|H][0-9]*", RegexOptions.IgnoreCase))
                    STRONG = "1";
            }
            catch (Exception ex)
            {
                // Catching exceptions                
            }

            ////////////////////////////////////////

            SetProcessAsComplete();

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

        public override void ExportDictionary(string filename, int filter_idx)
        {
            if (!filename.ToLower().EndsWith(".dictionary.sqlite3"))
            {
                filename = filename + ".dictionary.SQLite3";
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
                    command.Append("CREATE TABLE dictionary ( topic TEXT, definition TEXT)");
                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                }

                using (SQLiteCommand sqlCommand = new SQLiteCommand(sqlConn))
                {
                    System.Text.StringBuilder command = new System.Text.StringBuilder();
                    command.Append("CREATE TABLE info ( name TEXT, value TEXT )");
                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                }
                //
                SQLiteBulkInsert sbi1 = new SQLiteBulkInsert(sqlConn, "info");
                sbi1.AddParameter("name", DbType.String);
                sbi1.AddParameter("value", DbType.String);
                sbi1.Insert(new object[] { "description", DESCRIPTION });
                sbi1.Insert(new object[] { "language", "en" });
                sbi1.Insert(new object[] { "is_strong", "false" });
                sbi1.Insert(new object[] { "type", "translator" });
                sbi1.Insert(new object[] { "html_style", "a { text-decoration: none; } table, th, td { border: 1px solid black; }" });
                sbi1.Insert(new object[] { "standard_form_matching_type", "normalized form 1" }); 
                sbi1.Flush();
                sbi1.getSQLiteCommand().Dispose();


                SQLiteBulkInsert sbi = new SQLiteBulkInsert(sqlConn, "dictionary");
                sbi.AddParameter("topic", DbType.String);
                sbi.AddParameter("definition", DbType.String);

                
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
                    item = items[i].InnerText;
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
        }
    }
