using System;
using System.Data;
using System.Data.SQLite;
using System.Xml;

namespace Simple_Bible_Reader
{    

    public class BibleAnalyzerDictionaryFormat : DictionaryFormat
    {

        System.Xml.XmlElement rootdoc;
        System.Xml.XmlElement item_doc;
        System.Xml.XmlElement desc_doc;
        System.Xml.XmlAttribute attrib_id;
 
        int percent_complete = 0;

        public BibleAnalyzerDictionaryFormat(string file)
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

           
            try
            {
                DataTable dt = new DataTable();
                string data_src = @"" + this.FileName;
                SQLiteConnection cnn = new SQLiteConnection("Data Source=" + data_src);
                cnn.Open();
                SQLiteCommand mycommand = new SQLiteCommand(cnn);
                mycommand.CommandText = "SELECT topic, definition FROM dictionary ORDER BY id";
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
                    //
                    if (GlobalMemory.getInstance().ParseDictionary)
                    {
                        item = postProcessContent(item);
                        desc_doc.InnerXml = FormatUtil.EscapeXML( item);
                    }
                    else
                        desc_doc.InnerXml = FormatUtil.EscapeXML(item);

                    item_doc.AppendChild(desc_doc);
                    rootdoc.AppendChild(item_doc);
                }
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

        public override void ExportDictionary(string filename, int filter_idx)
        {
            if (!filename.EndsWith(".dct"))
            {
                filename = filename + ".dct";
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
                    command.Append("CREATE TABLE dictionary(id INTEGER PRIMARY KEY, topic, definition)");

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
                    command.Append("CREATE TABLE topics(topic)");
                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                }
                //
                using (SQLiteCommand sqlCommand = new SQLiteCommand(sqlConn))
                {
                    int dir=GlobalMemory.getInstance().Direction;
                    System.Text.StringBuilder command = new System.Text.StringBuilder();
                    command.Append("INSERT INTO title VALUES('" + ABBREVIATION.Replace("\'", "\'\'") + "','" + DESCRIPTION.Replace("\'", "\'\'") + "','" + COMMENTS.Replace("\'", "\'\'") + "')");
                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                }


                SQLiteBulkInsert sbi = new SQLiteBulkInsert(sqlConn, "dictionary");
                sbi.AddParameter("id", DbType.Int32);
                sbi.AddParameter("topic", DbType.String);
                sbi.AddParameter("definition", DbType.String);
                SQLiteBulkInsert sbi2 = new SQLiteBulkInsert(sqlConn, "topics");
                sbi2.AddParameter("topic", DbType.String);
                
                XmlNodeList items = DictionaryXmlDocument.SelectNodes("/dictionary/item");
                string item = null;
                string id = null;
                for (int i = 0; i < items.Count; i++)
                {
                    id = items[i].Attributes["id"].Value;
                    item = items[i].InnerText;
                    sbi.Insert(new object[] { (i+1).ToString(), id, item});
                    sbi2.Insert(new object[] { id });
                }
                sbi.Flush();
                sbi2.Flush();
                sqlConn.Dispose();
                }
                percent_complete=100;
            }

            private string getRtf(XmlNode child, string dictionary_text)
            {
                if (child.Name == "description")
                {
                    if (child.HasChildNodes)
                    {
                        foreach (XmlNode c in child.ChildNodes)
                        {
                            dictionary_text = getRtf(c, dictionary_text);
                        }
                    }
                    else
                        dictionary_text = dictionary_text + child.InnerText;
                    //}
                }
                else if (child.Name == "title")
                {
                    if (dictionary_text != "")
                        dictionary_text = dictionary_text + @"\par \par \b" + child.InnerText + @"\b\par ";
                    else
                        dictionary_text = dictionary_text + @"\b" + child.InnerText + @"\b\par ";
                }
                else if (child.Name == "see")
                {
                    /*
                    if (child.Attributes["target"].Value == "x-self")
                        dictionary_text = dictionary_text + "<a class='para' href='D" + child.InnerText + "'>" + child.InnerText + "</a><br>";
                    else
                        dictionary_text = dictionary_text + "<a class='para' target='_blank' href='" + child.Attributes["target"].Value + "'>" + child.InnerText + "</a><br>";
                    */
                    dictionary_text = dictionary_text + child.InnerText;
                }
                else if (child.Name == "reflink")
                {
                    if (child.Attributes["target"] != null)
                    {
                        string[] arr = child.Attributes["target"].Value.Split(';');
                        dictionary_text = dictionary_text + " " + FormatUtil.shortBookNames[int.Parse(arr[0]) - 1] + "_" + arr[1] + ":" + arr[2];
                    }
                    else if (child.Attributes["mscope"] != null)
                    {
                        string[] arr = child.Attributes["mscope"].Value.Split(';');
                        dictionary_text = dictionary_text + " " + FormatUtil.shortBookNames[int.Parse(arr[0]) - 1] + "_" + arr[1] + ":" + arr[2];
                    }
                }
                else if (child.Name == "#text")
                    dictionary_text = dictionary_text + child.InnerText;
                else
                {
                    if (child.HasChildNodes)
                    {
                        foreach (XmlNode c in child.ChildNodes)
                        {
                            dictionary_text = getRtf(c, dictionary_text);
                        }
                    }
                    else
                        dictionary_text = dictionary_text + child.InnerText;
                }
                return dictionary_text;
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

            /*
            string GetUnicodeFromRtf(string s)
            {
                var sb = new System.Text.StringBuilder();
                string no = "" ;
                bool trace = false;
                s = s + " ";
                for(int i=0;i<s.Length;i++)
                {
                    if (trace && s[i] == '?')
                    {                        
                            trace = false;
                            //Console.WriteLine("[" + no + "]");
                            sb.Append(Convert.ToChar(int.Parse(no.Trim())));
                            no = "";
                    }
                    else if (!trace && s[i] == '\\' && s[i + 1] == 'u')
                    {
                        i++;
                        trace = true;
                    }
                    else
                    {
                        if (trace)
                        {
                            no = no + s[i];
                        }
                        else
                            sb.Append(s[i]);
                    }
                }
                return sb.ToString().Trim();
            }
            */
        }
    }
