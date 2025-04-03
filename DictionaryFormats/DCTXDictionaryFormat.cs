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
using System.Reflection;
using System.Text;

namespace Simple_Bible_Reader
{    

    public class DCTXDictionaryFormat : DictionaryFormat
    {

        System.Xml.XmlElement rootdoc;
        System.Xml.XmlElement item_doc;
        System.Xml.XmlElement desc_doc;
        System.Xml.XmlAttribute attrib_id;

        int percent_complete = 0;
        string _id = null;
        string _item = null;

        public DCTXDictionaryFormat(string file)
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


            percent_complete = 0;

 
            try
            {
                DataTable dt = new DataTable();
                string data_src = @"" + this.FileName;
                SQLiteConnection cnn = new SQLiteConnection("Data Source=" + data_src);
                cnn.Open();
                SQLiteCommand mycommand = new SQLiteCommand(cnn);
                mycommand.CommandText = "SELECT Topic, Definition FROM Dictionary ORDER BY Topic";
                SQLiteDataReader reader = mycommand.ExecuteReader();

                dt.Load(reader);

                mycommand.Dispose();
                reader.Dispose();
                cnn.Dispose();

                percent_complete = 0;
                byte[] _item_bytes, _compressed_bytes;
                int zlibhdr = 0;
                int override_compression_count = 0;
                int total_verse_count = 0;
                for (int row = 0; row < dt.Rows.Count; row++)
                {
                    _id = dt.Rows[row][0].ToString();
              
                    percent_complete = (row * 100) / dt.Rows.Count;

                    if (dt.Rows[row][1] is string)
                    {
                        _item = dt.Rows[row][1].ToString();
                    }
                    else
                    {
                        try
                        {
                            //detect if compressed
                            zlibhdr = FormatUtil.findZlibHeaderIdx((byte[])dt.Rows[row][1], 0xB);

                            if (zlibhdr == -1) // probably encrypted
                            {
                                _item_bytes = FormatUtil.eSwordTwoFishDecrypt((byte[])dt.Rows[row][1]);
                                zlibhdr = FormatUtil.findZlibHeaderIdx(_item_bytes, 0xB);
                                override_compression_count++;
                            }
                            else
                            {
                                // we need to be absolutely sure that it is not encrypted.

                                if (override_compression_count * 100 / total_verse_count > 90)
                                {
                                    _item_bytes = FormatUtil.eSwordTwoFishDecrypt((byte[])dt.Rows[row][1]);
                                    zlibhdr = FormatUtil.findZlibHeaderIdx(_item_bytes, 0xB);
                                }
                                else
                                    _item_bytes = (byte[])dt.Rows[row][1];
                            }

                            if (_item_bytes.Length > 0xB)
                            {
                                zlibhdr = FormatUtil.findZlibHeaderIdx(_item_bytes, 0xB);
                                if (_item_bytes[zlibhdr] == 0x78) // compressed with zlib 
                                {
                                    _compressed_bytes = new byte[_item_bytes.Length - zlibhdr];
                                    Array.Copy(_item_bytes, zlibhdr, _compressed_bytes, 0, _item_bytes.Length - zlibhdr);
                                    _item = Encoding.UTF8.GetString(FormatUtil.eSword_zLibUncompress(_compressed_bytes));
                                }
                                else
                                    _item = Encoding.UTF8.GetString(_item_bytes);
                            }
                            else
                                _item = Encoding.UTF8.GetString(_item_bytes);
                        }
                        catch (Exception)
                        {
                            _item = Encoding.UTF8.GetString((byte[])dt.Rows[row][1]);
                        }
                    }
                    total_verse_count++;
                    processLine();
                }
            }
            catch (Exception ex)
            {
                // Catching exceptions                   
            }
           
            SetProcessAsComplete();
        }

        private void processLine()
        {
            item_doc = DictionaryXmlDocument.CreateElement("item");
            attrib_id = DictionaryXmlDocument.CreateAttribute("id");
            attrib_id.Value = _id;
            item_doc.Attributes.Append(attrib_id);
            desc_doc = DictionaryXmlDocument.CreateElement("description");
            //
            if (GlobalMemory.getInstance().ParseDictionary)
            {
                if (CLEANING)
                {
                    if (GlobalMemory.getInstance().autoSetParsingDictionary)
                        GlobalMemory.getInstance().ConvertRtfToHtmlDictionary = true;
                    _item = postProcessContent(_item);
                    _item = Regex.Replace(_item, @"(Gen|Exo|Lev|Num|Deu|Jos|Jdg|Rut|1Sa|2Sa|1Ki|2Ki|1Ch|2Ch|Ezr|Neh|Est|Job|Psa|Pro|Ecc|Sol|Isa|Jer|Lam|Eze|Dan|Hos|Joe|Amo|Oba|Jon|Mic|Nah|Hab|Zep|Hag|Zec|Mal|Mat|Mar|Luk|Joh|Act|Rom|1Co|2Co|Gal|Eph|Phi|Col|1Th|2Th|1Ti|2Ti|Tit|Phm|Heb|Jam|1Pe|2Pe|1Jo|2Jo|3Jo|Jud|Rev)[\s_]([0-9]{1,2}):([0-9]{1,3})", @"<reflink target=""$1 $2:$3"">$1 $2:$3</reflink>");

                    desc_doc.InnerText = _item;
                    changeRefLink(desc_doc);
                }
                else
                    desc_doc.InnerText = _item;
            }
            else
                desc_doc.InnerXml = FormatUtil.EscapeXML(_item);

            item_doc.AppendChild(desc_doc);
            rootdoc.AppendChild(item_doc);
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
                return 3;
            }
        }


        public override void ExportDictionary(string filename, int filter_idx)
        {
            if (!filename.EndsWith(".dctx"))
            {
                filename = filename + ".dctx";
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
                    command.Append("CREATE TABLE Details (Description NVARCHAR(255), Abbreviation NVARCHAR(50), Comments TEXT, Version INT, Strong BOOL)");

                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                }

                using (SQLiteCommand sqlCommand = new SQLiteCommand(sqlConn))
                {
                    System.Text.StringBuilder command = new System.Text.StringBuilder();
                    command.Append("CREATE TABLE Dictionary (Topic NVARCHAR(100), Definition TEXT)");
                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                }

                using (SQLiteCommand sqlCommand = new SQLiteCommand(sqlConn))
                {
                    System.Text.StringBuilder command = new System.Text.StringBuilder();
                    command.Append("CREATE INDEX TopicIndex ON Dictionary (Topic)");
                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                }
                //
                using (SQLiteCommand sqlCommand = new SQLiteCommand(sqlConn))
                {
                    int dir=GlobalMemory.getInstance().Direction;
                    System.Text.StringBuilder command = new System.Text.StringBuilder();
                    command.Append("INSERT INTO Details VALUES('"+DESCRIPTION+"','"+ABBREVIATION+"','"+COMMENTS+"',2,1)");
                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                }


                SQLiteBulkInsert sbi = new SQLiteBulkInsert(sqlConn, "Dictionary");
                sbi.AddParameter("Topic", DbType.String);
                sbi.AddParameter("Definition", DbType.String);

                
                XmlNodeList items = DictionaryXmlDocument.SelectNodes("/dictionary/item");
                string item = null;
                string id = null;
                for (int i = 0; i < items.Count; i++)
                {
                    id = items[i].Attributes["id"].Value;
                    item = getRtf(items[i],"");
                    item = item.Replace("\n", @"\par ").Trim();
                    sbi.Insert(new object[] { id, GetRtfUnicodeEscapedString(item) });
                }
                sbi.Flush();
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
