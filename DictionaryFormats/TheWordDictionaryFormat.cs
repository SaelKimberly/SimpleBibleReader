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
using System.Drawing.Imaging;
using System.Net.NetworkInformation;

namespace Simple_Bible_Reader
{    

    public class TheWordDictionaryFormat : DictionaryFormat
    {

        System.Xml.XmlElement rootdoc;
        System.Xml.XmlElement item_doc;
        System.Xml.XmlElement desc_doc;
        System.Xml.XmlAttribute attrib_id;        

        int percent_complete = 0;

        string[] words;
        StringBuilder sb = new StringBuilder();
        string tmpword;
        string[] verseref;

        public System.Windows.Forms.RichTextBox rtb_dct = new System.Windows.Forms.RichTextBox();

        public TheWordDictionaryFormat(string file)
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
            string type = "";
           percent_complete = 0;
           string id = null;
           string item = null;

            DataTable dt = new DataTable();
            string data_src = @"" + this.FileName;
            SQLiteConnection cnn = new SQLiteConnection("Data Source=" + data_src + ";UseUTF8Encoding=True;");
            cnn.Open();
            SQLiteCommand mycommand = new SQLiteCommand(cnn);
            SQLiteCommand mycommand2 = new SQLiteCommand(cnn);
            mycommand.CommandText = "select t.subject, c.data from topics t,content c where t.id=c.topic_id";
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
            while (reader.HasRows)
            {
                reader.Read();
                name = reader["name"].ToString();
                if (name == "abbrev")
                    ABBREVIATION = (string)reader["value"];
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
            //////
            byte[] bytes = null;
            long len = 0;
            byte[] _bytes = null;
            byte[] _bytes2 = null;
            percent_complete = 0;
            string tmprtf;
            if (!secure)
            {
                for (int row = 0; row < dt.Rows.Count; row++)
                {
                    id = dt.Rows[row][0].ToString();
                    if (dt.Columns[1].DataType == typeof(byte[]))
                    {
                        item = Encoding.UTF8.GetString((byte[])dt.Rows[row][1]);
                    }
                    else
                    {
                        item = dt.Rows[row][1].ToString();
                    }
                    percent_complete = (row * 100) / dt.Rows.Count;
                    //


                    item_doc = DictionaryXmlDocument.CreateElement("item");
                    attrib_id = DictionaryXmlDocument.CreateAttribute("id");
                    attrib_id.Value = id;
                    item_doc.Attributes.Append(attrib_id);
                    desc_doc = DictionaryXmlDocument.CreateElement("description");
                    //                    
                    if (GlobalMemory.getInstance().ParseDictionary)
                    {
                        if (CLEANING || GlobalMemory.getInstance().autoSetParsingDictionary)
                        {
                            //item = item.Replace(">", "&gt;").Replace("<", "&lt;").Replace("'", "&apos;").Replace("&", "&amp;");
                            // quotes affects the next stmt
                            //item = Regex.Replace(item, @"\{\\field\{\\\*\\fldinst HYPERLINK ""tw://bible\.\*\?id=([0-9]{0,3})\.([0-9]{0,3})\.([0-9]{0,3})""\}\{\\fldrslt([^\}]*) ([^\}]*)\}\}", @"<reflink target=""$1;$2;$3"">$5</reflink>");
                            //item = Regex.Replace(item, @"\{\\field\{\\\*\\fldinst HYPERLINK ""tw://\[self\]\?([^""]*)""\}\{\\fldrslt([^\}]*)([^\}]*)\}\}", @"<see target=""x-self"">$1</see>");

                            item = Regex.Replace(item, @"(tw://[^=]+=)([0-9\.]+)([^\[]+)(\[vref\])(}})", @"$1$2$3#$2#$5");

                            /*
                            if (item.StartsWith("{\\rtf1\\"))
                            {
                                rtb_dct.Rtf = item;
                            }
                            else
                            {
                                rtb_dct.Rtf = rtf_header + item;                            
                            }
                            item = rtb_dct.Text;
                            */
                            if (GlobalMemory.getInstance().autoSetParsingDictionary)
                                GlobalMemory.getInstance().ConvertRtfToHtmlDictionary = true;
                            item = postProcessContent(item);

                            item = replaceVerseRef(item);

                            //
                            //item = Regex.Replace(item, "[\x00-\x09\x0E-\x1f]*", "");
                            //item = Regex.Replace(item, @"<tw://[^>]*>", "").Trim();

                            item = item.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");

                            item = convertWindowsMetaImagesToJpg(item);
                            try
                            {
                                desc_doc.InnerXml = item;
                            }
                            catch (Exception)
                            {
                                desc_doc.InnerText = item;
                            }

                            //changeRefLink(desc_doc);
                        }
                        else
                        {
                            try
                            {
                                desc_doc.InnerXml = item;
                            }
                            catch (Exception)
                            {
                                desc_doc.InnerText = item;
                            }
                        }
                    }
                    else
                        desc_doc.InnerXml = FormatUtil.EscapeXML(item);

                    item_doc.AppendChild(desc_doc);
                    rootdoc.AppendChild(item_doc);
                }
            }
            else
            {
                for (int row = 0; row < dt.Rows.Count; row++)
                {
                    reader2.Read();
                    id = dt.Rows[row][0].ToString();
                    item = dt.Rows[row][1].ToString();
                    percent_complete = (row * 100) / dt.Rows.Count;
                    //
                    len = reader2.GetBytes(1, 0, null, 0, 0);
                    _bytes = new byte[len];
                    len = reader2.GetBytes(1, 0, _bytes, 0, (int)len);
                    try
                    {
                        for (byte i = 0; i < 255; i++)
                        {
                            _bytes2 = SCOP.decryptSCOPBytes(_bytes, i);
                            if (_bytes2[0] == 0x78 && _bytes2[1] == 0xDA)
                            {
                                try
                                {
                                    bytes = ZlibStream.UncompressBuffer(_bytes2);
                                    item = Encoding.UTF8.GetString(bytes);
                                    break;
                                }
                                catch (Exception)
                                {
                                    // trying out differnet xorkeys until uncompression works withput error
                                    // aparently multiple keys exist that can produce zlib headers at start.
                                }
                            }
                        }

                        if (!(_bytes2[0] == 0x78 && _bytes2[1] == 0xDA))
                        {
                            item = "";
                        }
                    }
                    catch (Exception e)
                    {
                        item = "";
                    }
                    //
                    item_doc = DictionaryXmlDocument.CreateElement("item");
                    attrib_id = DictionaryXmlDocument.CreateAttribute("id");
                    attrib_id.Value = id;
                    item_doc.Attributes.Append(attrib_id);
                    desc_doc = DictionaryXmlDocument.CreateElement("description");
                    //
                    if (GlobalMemory.getInstance().ParseDictionary)
                    {
                        if (CLEANING || GlobalMemory.getInstance().autoSetParsingDictionary)
                        {

                            // quotes affects the next stmt
                            //item = Regex.Replace(item, @"\{\\field\{\\\*\\fldinst HYPERLINK ""tw://bible\.\*\?id=([0-9]{0,3})\.([0-9]{0,3})\.([0-9]{0,3})""\}\{\\fldrslt([^\}]*) ([^\}]*)\}\}", @"<reflink target=""$1;$2;$3"">$5</reflink>");                        
                            //item = Regex.Replace(item, @"\{\\field\{\\\*\\fldinst HYPERLINK ""tw://\[self\]\?([^""]*)""\}\{\\fldrslt([^\}]*)([^\}]*)\}\}", @"<see target=""x-self"">$1</see>");

                            item = Regex.Replace(item, @"(tw://[^=]+=)([0-9\.]+)([^\[]+)(\[vref\])(}})", @"$1$2$3#$2#$5");


                            GlobalMemory.getInstance().ConvertRtfToHtmlDictionary = true;
                            item = postProcessContent(item);

                            item = replaceVerseRef(item);
                            //
                            //item = Regex.Replace(item, "[\x00-\x09\x0E-\x1f]*", "");
                            //item = item.Replace("\f", "");
                            //item = Regex.Replace(item, @"<tw://[^>]*>", "").Trim();

                            item = convertWindowsMetaImagesToJpg(item);
                            item = item.Replace(">", "&gt;").Replace("<", "&lt;").Replace("'", "&apos;").Replace("&", "&amp;");


                            try
                            {
                                desc_doc.InnerXml = item;
                                //changeRefLink(desc_doc);
                            }
                            catch (Exception)
                            {
                                desc_doc.InnerText = item;
                            }
                        }
                        else
                        {
                            try
                            {
                                desc_doc.InnerXml = item;
                            }
                            catch (Exception)
                            {
                                desc_doc.InnerText = item;
                            }
                        }
                    }
                    else
                        desc_doc.InnerXml = FormatUtil.EscapeXML(item);

                    item_doc.AppendChild(desc_doc);
                    rootdoc.AppendChild(item_doc);
                }
            }
            reader2.Dispose();
            cnn.Dispose();
            ////////////////////////////////////////

            SetProcessAsComplete();
        }

        private string convertWindowsMetaImagesToJpg(string item)
        {
            if (item.IndexOf("data:windows/metafile;base64,") == -1)
                return item;

            int idx = 0; //content.IndexOf("data:windows/metafile;base64,");
            string content = FormatUtil.UnescapeXML(item);
            string wmf = "";
            string width="";
            string height="";
            int endidx = 0;
            byte[] wmfbytes = new byte[] { };
            byte[] jpgbytes = new byte[] { };

            if (idx != -1)
            {
                try
                {
                    // width
                    idx = content.IndexOf("<img width=\"");
                    content = content.Substring(idx + "<img width=\"".Length);
                    endidx = content.IndexOf("\"");
                    width = content.Substring(0, endidx);

                    // height
                    idx = content.IndexOf("height=\"");
                    content = content.Substring(idx + "height=\"".Length);
                    endidx = content.IndexOf("\"");
                    height = content.Substring(0, endidx);

                    // actual image
                    idx = content.IndexOf("data:windows/metafile;base64,");
                    content = content.Substring(idx + "data:windows/metafile;base64,".Length);
                    endidx = content.IndexOf("\"");
                    wmf = content.Substring(0, endidx);

                    wmfbytes = Convert.FromBase64String(wmf);
                    
                    jpgbytes = convertWmfToJpg(wmfbytes, int.Parse(width), int.Parse(height));

                    //
                    content = item.Replace(wmf, Convert.ToBase64String(jpgbytes));
                    return content;
                }
                catch (Exception e)
                {
                    MessageBox.Show(
                        "width: "+width+
                        "height: " + height+
                        "wmf length: "+ wmfbytes.Length+
                        "jpg length: " + jpgbytes.Length +
                        e.Message);
                    return content;
                }
            }
            else
                return content;
        }

        
        static byte[] convertWmfToJpg(byte[] wmf, int width, int height)
        {

            var ms_wmf = new MemoryStream(wmf);
            MemoryStream ms_jpg = ConvertImage(ms_wmf,ImageFormat.Jpeg);
            return ms_jpg.ToArray();
        }


        
        public static MemoryStream ConvertImage(Stream originalStream, ImageFormat format)
        {
            var image = Image.FromStream(originalStream);
            MemoryStream stream = new MemoryStream();
            image.Save(stream, format);
            stream.Position = 0;
            return stream;
        }

        private string replaceVerseRef(string item)
        {
            words = item.Split(new char[] { '#' });
            sb.Clear();

            foreach (string word in words)
            {
                tmpword = word;
                if (Regex.IsMatch(tmpword, @"^[0-9]{1,2}\.[0-9]{1,2}\.[0-9]{1,3}$"))
                {
                    verseref = tmpword.Split(new char[] { '.' });

                    sb.Append("<a href='\"B");
                    sb.Append(verseref[0]);
                    sb.Append(";");
                    sb.Append(verseref[1]);
                    sb.Append(";");
                    sb.Append(verseref[2]);
                    sb.Append("\">");
                    sb.Append(FormatUtil.getBookNameShort(int.Parse(verseref[0])));
                    sb.Append(" ");
                    sb.Append(verseref[1]);
                    sb.Append(":");
                    sb.Append(verseref[2]);
                    sb.Append("</a>");
                }
                else
                    sb.Append(tmpword);
            }
            

            return sb.ToString();
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
                return 5;
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

        public override void ExportDictionary(string filename, int filter_idx)
        {
            if (!filename.EndsWith(".dct.twm"))
            {
                filename = filename + ".dct.twm";
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
                    System.Text.StringBuilder command = new System.Text.StringBuilder();
                    command.Append("CREATE TABLE topics(id integer primary key, pid integer default 0, subject text, rel_order, content_type text)");
                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                }

                using (SQLiteCommand sqlCommand = new SQLiteCommand(sqlConn))
                {
                    System.Text.StringBuilder command = new System.Text.StringBuilder();
                    command.Append("CREATE TABLE topics_wordindex (id integer, word text, priority integer default 1)");
                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                }
                
                ///////////////////

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
                //sbi_config.Insert(new object[] { "content.type", "rtf" });
                sbi_config.Insert(new object[] { "type", "1" });
                sbi_config.Insert(new object[] { "schema.version", "1" });
                sbi_config.Insert(new object[] { "title", DESCRIPTION });
                sbi_config.Insert(new object[] { "description", DESCRIPTION });
                sbi_config.Insert(new object[] { "creator", "Simple Bible Reader (" + GlobalMemory.AUTHOR_WEBSITE + ")" });
                sbi_config.Insert(new object[] { "id", "d4" });
                sbi_config.Insert(new object[] { "about", COMMENTS });
                sbi_config.Insert(new object[] { "description.english", "" });
                sbi_config.Insert(new object[] { "categories", "Dictionary\\OT, NT" });
                sbi_config.Insert(new object[] { "keywords", "" });
                sbi_config.Insert(new object[] { "title.english", "" });
                sbi_config.Insert(new object[] { "abbrev", ABBREVIATION });
                sbi_config.Insert(new object[] { "user", "0" });
                sbi_config.Insert(new object[] { "version.major", "1" });
                sbi_config.Insert(new object[] { "version.minor", "0" });
                sbi_config.Insert(new object[] { "editorial.comments", "" });
                sbi_config.Insert(new object[] { "version.date", "" });
                sbi_config.Insert(new object[] { "preserve.fonts", "0" });
                sbi_config.Insert(new object[] { "preserve.fonts.list", "" });
                sbi_config.Insert(new object[] { "search.topics.index.ver", "3" });
                sbi_config.Flush();

                //////

                SQLiteBulkInsert sbi_topics = new SQLiteBulkInsert(sqlConn, "topics");

                sbi_topics.AddParameter("id", DbType.Int32);
                sbi_topics.AddParameter("subject", DbType.String);
                sbi_topics.AddParameter("rel_order", DbType.Int32);   
                

                SQLiteBulkInsert sbi_topics_wordindex = new SQLiteBulkInsert(sqlConn, "topics_wordindex");

                sbi_topics_wordindex.AddParameter("id", DbType.Int32);
                sbi_topics_wordindex.AddParameter("word", DbType.String);


                SQLiteBulkInsert sbi_content = new SQLiteBulkInsert(sqlConn, "content");
                sbi_content.AddParameter("topic_id", DbType.Int32);
                sbi_content.AddParameter("data", DbType.String);
                
                XmlNodeList items = DictionaryXmlDocument.SelectNodes("/dictionary/item");
                string id = null;
                int topic_id = 1;
                string dict_text=null;
                foreach (XmlNode item in items)
                {
                    percent_complete = topic_id * 100 / items.Count;

                    id = item.Attributes["id"].Value;
                    if (id != null)
                    {
                        dict_text = getRtf(item, "");
                        // the new line is required since we don't know the source format. 
                        // if text, then \par is required. 
                        dict_text = dict_text.Replace("\n", @"\par ").Trim();
                        sbi_topics_wordindex.Insert(new object[] { topic_id, id });
                        sbi_topics.Insert(new object[] { topic_id, id, topic_id });
                        sbi_content.Insert(new object[] { topic_id, GetRtfUnicodeEscapedString(dict_text) });

                        topic_id++;
                    }

                }
                sbi_topics_wordindex.Flush();
                sbi_topics.Flush();
                sbi_content.Flush();
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
                    dictionary_text = dictionary_text + "{\\field{\\*\\fldinst HYPERLINK \"tw://[self]?" + child.InnerText + "\"}{\\fldrslt " + child.InnerText + "}}";
                }
                else if (child.Name == "reflink")
                {
                    if (child.Attributes["target"] != null)
                    {
                        string[] arr = child.Attributes["target"].Value.Split(';');
                        if (BibleFormat.getInstance() != null)
                            dictionary_text = dictionary_text + " " + "{\\field{\\*\\fldinst HYPERLINK \"tw://bible.*?id=" + arr[0] + "." + arr[1] + "." + arr[2] + "\"}{\\fldrslt " + BibleFormat.getInstance().getBookName(int.Parse(arr[0])) + " " + arr[1] + ":" + arr[2] + "}}";
                        else
                            dictionary_text = dictionary_text + " " + "{\\field{\\*\\fldinst HYPERLINK \"tw://bible.*?id=" + arr[0] + "." + arr[1] + "." + arr[2] + "\"}{\\fldrslt " + FormatUtil.shortBookNames[int.Parse(arr[0]) - 1] + " " + arr[1] + ":" + arr[2] + "}}";
                    }
                    else if (child.Attributes["mscope"] != null)
                    {
                        string[] arr = child.Attributes["mscope"].Value.Split(';');
                        if(BibleFormat.getInstance()!=null)
                            dictionary_text = dictionary_text + " " + "{\\field{\\*\\fldinst HYPERLINK \"tw://bible.*?id=" + arr[0] + "." + arr[1] + "." + arr[2] + "\"}{\\fldrslt " + BibleFormat.getInstance().getBookName(int.Parse(arr[0])) + " " + arr[1] + ":" + arr[2] + "}}";
                        else
                            dictionary_text = dictionary_text + " " + "{\\field{\\*\\fldinst HYPERLINK \"tw://bible.*?id=" + arr[0] + "." + arr[1] + "." + arr[2] + "\"}{\\fldrslt "+FormatUtil.shortBookNames[int.Parse(arr[0])-1] +" "+ arr[1] + ":" + arr[2] + "}}";
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
        }
    }
