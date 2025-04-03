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
using RtfPipe;
using PalmBibleExport;
using System.Threading;

namespace Simple_Bible_Reader
{    

    public class TheWordCommentaryFormat : CommentaryFormat
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
        
        public string rtf_header = @"{\rtf1\ansi\ansicpg1252\deff0\deflang1033{\fonttbl{\f1\fnil\fprq2\fcharset161 TITUS Cyberbit Basic;}{\f2\fnil\fprq2\fcharset177 TITUS Cyberbit Basic;}{\f3\fnil\fprq2\fcharset0 TITUS Cyberbit Basic;}{\f4\fnil\fprq2\fcharset136 PMingLiU;}{\f5\fnil\fprq2\fcharset204 Georgia;}{\f6\fnil\fprq2\fcharset238 Georgia;}{\f7\fnil\fprq2\fcharset134 PMingLiU;}{\f8\fnil\fprq2\fcharset222 Cordia New;}{\f9\fnil\fprq2\fcharset129 Gulim;}{\f10\fnil\fprq2\fcharset178 TITUS Cyberbit Basic;}{\f11\fnil\fprq2\fcharset162 Georgia;}{\f12\fnil\fprq2\fcharset163 Times New Roman;}{\f13\fnil\fprq2\fcharset128 MS Mincho;}}{\colortbl ;\red0\green0\blue0;\red0\green0\blue255;\red0\green255\blue255;\red0\green255\blue0;\red255\green0\blue255;\red255\green0\blue0;\red255\green255\blue0;\red255\green255\blue255;\red0\green0\blue128;\red0\green128\blue128;\red0\green128\blue0;\red128\green0\blue128;\red128\green0\blue0;\red128\green128\blue0;\red128\green128\blue128;\red192\green192\blue192;}{\viewkind4\uc1\pard\plain\cf0\fs\par}";
        public string rtf_header_novk4 = @"{\rtf1\ansi\ansicpg1252\deff0\deflang1033{\fonttbl{\f1\fnil\fprq2\fcharset161 TITUS Cyberbit Basic;}{\f2\fnil\fprq2\fcharset177 TITUS Cyberbit Basic;}{\f3\fnil\fprq2\fcharset0 TITUS Cyberbit Basic;}{\f4\fnil\fprq2\fcharset136 PMingLiU;}{\f5\fnil\fprq2\fcharset204 Georgia;}{\f6\fnil\fprq2\fcharset238 Georgia;}{\f7\fnil\fprq2\fcharset134 PMingLiU;}{\f8\fnil\fprq2\fcharset222 Cordia New;}{\f9\fnil\fprq2\fcharset129 Gulim;}{\f10\fnil\fprq2\fcharset178 TITUS Cyberbit Basic;}{\f11\fnil\fprq2\fcharset162 Georgia;}{\f12\fnil\fprq2\fcharset163 Times New Roman;}{\f13\fnil\fprq2\fcharset128 MS Mincho;}}{\colortbl ;\red0\green0\blue0;\red0\green0\blue255;\red0\green255\blue255;\red0\green255\blue0;\red255\green0\blue255;\red255\green0\blue0;\red255\green255\blue0;\red255\green255\blue255;\red0\green0\blue128;\red0\green128\blue128;\red0\green128\blue0;\red128\green0\blue128;\red128\green0\blue0;\red128\green128\blue0;\red128\green128\blue128;\red192\green192\blue192;}";
        public string rtf_content="";
        RichTextBox rtb = new RichTextBox();
        

        int percent_complete = 0;

        public TheWordCommentaryFormat(string file)
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
           
            DataTable dt = new DataTable();
            string data_src = @"" + this.FileName;
            SQLiteConnection cnn = new SQLiteConnection("Data Source=" + data_src + ";UseUTF8Encoding=True;");
            cnn.Open();
            SQLiteCommand mycommand = new SQLiteCommand(cnn);
            SQLiteCommand mycommand2 = new SQLiteCommand(cnn);
            mycommand.CommandText = "SELECT a.bi, a.ci, a.fvi, b.data Scripture FROM bible_refs a, content b WHERE a.topic_id = b.topic_id ORDER BY a.bi, a.ci, a.fvi";
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
            if (!secure && !compressed)
            {
                for (int row = 0; row < dt.Rows.Count; row++)
                {
                    //reader2.Read();

                    bno = int.Parse(dt.Rows[row][0].ToString());
                    if (bno > 66)
                        break;              
                    percent_complete = ((_book_no) * 100) / 66;
                    _book_no = bno;
                    _chap_no = int.Parse(dt.Rows[row][1].ToString());
                    _verse_no = int.Parse(dt.Rows[row][2].ToString());

                    /*
                    len = reader2.GetBytes(3, 0, null, 0, 0);
                    _bytes=new byte[len];
                    len = reader2.GetBytes(3, 0, _bytes, 0, (int)len);
                    _verse_text = Encoding.UTF8.GetString(_bytes);
                    len = len - 0x1e;
                    if (_verse_text.Length > len)
                         _verse_text = _verse_text.Substring(0, (int)len);
                    */
                    if (dt.Rows[row][3] is byte[])
                    {
                        if (content_type == "rvf")
                            _verse_text = FormatUtil.Rvf2Text((byte[])dt.Rows[row][3]);
                        else
                            _verse_text = Encoding.UTF8.GetString((byte[])dt.Rows[row][3]);
                    }
                    else
                    {
                        if (content_type == "rvf")
                            _verse_text = FormatUtil.Rvf2Text(Encoding.UTF8.GetBytes(dt.Rows[row][3].ToString()));
                        else
                            _verse_text = dt.Rows[row][3].ToString();
                    }

                    processLine();
                }
             }
            else if (!secure && compressed)
            {
                for (int row = 0; row < dt.Rows.Count; row++)
                {
                    reader2.Read();

                    bno = int.Parse(dt.Rows[row][0].ToString());
                    if (bno > 66)
                        break;
                    percent_complete = ((_book_no) * 100) / 66;
                    _book_no = bno;
                    _chap_no = int.Parse(dt.Rows[row][1].ToString());
                    _verse_no = int.Parse(dt.Rows[row][2].ToString());

                    
                    len = reader2.GetBytes(3, 0, null, 0, 0);
                    _bytes2 = new byte[len];
                    len = reader2.GetBytes(3, 0, _bytes2, 0, (int)len);
                    try
                    {                                               
                        if (_bytes2[0] == 0x78 && _bytes2[1] == 0xDA)
                        {
                            //Console.WriteLine("Found!");
                            bytes = ZlibStream.UncompressBuffer(_bytes2);
                            if (content_type == "rvf")
                                _verse_text = FormatUtil.Rvf2Text(bytes);
                            else
                                _verse_text = Encoding.UTF8.GetString(bytes);
                            //Themes.DarkMessageBoxShow(_verse_text);
                        }
                        else
                        {
                            _verse_text = "";
                        }
                    }
                    catch (Exception ex)
                    {
                        _verse_text = ""+ex.Message;
                    }
                    

                    processLine();
                }
            }
            else
            {
                //Themes.DarkMessageBoxShow("Encrypted TheWord commentary modules are not yet supported!\nWe hope to support it soon.", "Encrypted", MessageBoxButtons.OK, MessageBoxIcon.Stop);                
                for (int row = 0; row < dt.Rows.Count; row++)
                {
                    reader2.Read();

                    bno = int.Parse(dt.Rows[row][0].ToString());
                    if (bno > 66)
                        break;
                    percent_complete = ((_book_no) * 100) / 66;
                    _book_no = bno;
                    _chap_no = int.Parse(dt.Rows[row][1].ToString());
                    _verse_no = int.Parse(dt.Rows[row][2].ToString());

                    
                    len = reader2.GetBytes(3, 0, null, 0, 0);
                    _bytes=new byte[len];
                    len = reader2.GetBytes(3, 0, _bytes, 0, (int)len);
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
                                _verse_text = FormatUtil.Rvf2Text(bytes);
                            else
                                _verse_text = Encoding.UTF8.GetString(bytes);
                            //Themes.DarkMessageBoxShow(_verse_text);
                        }
                        else
                        {
                            _verse_text = "";
                        }
                    }
                    catch (Exception)
                    {
                        _verse_text = "";
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
            if (_book_no != p_book_no || (_chap_no == 1 && _verse_no == 1 && p_verse_no >= _verse_no))
            {
                bookdoc = (XmlElement)CommentaryXmlDocument.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber='" + _book_no + "']");
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
                if (GlobalMemory.getInstance().ParseCommentary)
                {
                    if (GlobalMemory.getInstance().autoSetParsingCommentary)
                        GlobalMemory.getInstance().ConvertRtfToHtmlCommentary = true;
                    _verse_text = postProcessContent(rtf_content);
                    versedoc.InnerText = _verse_text;
                }
                else
                    versedoc.InnerXml = FormatUtil.EscapeXML(_verse_text);
                ///
                //
                /*
                string _tmp = "";
                if (GlobalMemory.getInstance().ParseCommentary && CLEANING)
                {
                    try
                    {
                        _tmp = _verse_text;
                        if (_tmp.StartsWith("{\\rtf1\\"))
                            rtf_content = _tmp;
                        else if (_tmp.StartsWith("\\viewkind4"))
                            rtf_content = rtf_header_novk4 + _tmp;
                        else
                            rtf_content = rtf_header + _tmp;

                        //_tmp = rtb.Text; // if this is blank, may be the content contains html table.
                        GlobalMemory.getInstance().ConvertRtfToHtmlCommentary = true;
                        _tmp = postProcessContent(rtf_content);
                    }
                    catch (Exception)
                    {
                        _tmp = _verse_text; // try again
                        _tmp = _tmp.Replace(">", "&gt;").Replace("<", "&lt;").Replace("'", "&apos;").Replace("&", "&amp;");
                        _tmp = Regex.Replace(_tmp, @"\{\\field\{\\\*\\fldinst HYPERLINK ""tw://bible\.\*\?id=([0-9]{0,3})\.([0-9]{0,3})\.([0-9]{0,3})""\}\{\\fldrslt([^\}]*) ([^\}]*)\}\}", @"<reflink target=""$1;$2;$3"">$5</reflink>");
                        _tmp = Regex.Replace(_tmp, @"\{\\field\{\\\*\\fldinst HYPERLINK ""tw://\[self\]\?([^""]*)""\}\{\\fldrslt([^\}]*)([^\}]*)\}\}", @"<see target=""x-self"">$1</see>");

                        if (_tmp.StartsWith("{\\rtf1\\"))
                            rtb.Rtf = _tmp;
                        else if (_tmp.StartsWith("\\viewkind4"))
                            rtb.Rtf = rtf_header_novk4 + _tmp;
                        else
                            rtb.Rtf = rtf_header + _tmp;
                        _tmp = rtb.Text;
                        //
                        _tmp = Regex.Replace(_tmp, "[\x00-\x09\x0E-\x1f]*", "");
                        _tmp = _tmp.Replace("\f", "");
                        _tmp = Regex.Replace(_tmp, @"<tw://[^>]*>", "").Trim();
                    }
                    
                    _verse_text = _tmp;                   

                    try
                    {
                        _verse_text = _verse_text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
                        versedoc.InnerXml = _verse_text;
                        changeRefLink(versedoc);
                    }
                    catch (Exception)
                    {
                        versedoc.InnerText = _verse_text;
                    }

                }
                else
                    versedoc.InnerText = _verse_text;
             */

                versedoc.Attributes.Append(attrib);
                chapdoc.AppendChild(versedoc);
            }

            p_book_no = _book_no;
            p_chap_no = _chap_no;
            p_verse_no = _verse_no;
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

        public override void ExportCommentary(string filename, int filter_idx)
        {
            if (!filename.EndsWith(".cmt.twm"))
            {
                filename = filename + ".cmt.twm";
            }
            if (filename.Equals(""))
            {
                return;
            }
            if (System.IO.File.Exists(filename))
                System.IO.File.Delete(filename);

            SQLiteConnection.CreateFile(filename);
            percent_complete = 0;
            using (SQLiteConnection sqlConn = new SQLiteConnection())
            {
                sqlConn.ConnectionString = "Data source = " + filename;
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
                    command.Append("CREATE TABLE bible_refs (topic_id integer primary key AUTOINCREMENT,module text,type char,bi integer,ci integer, fvi integer, tvi integer, content_type text)");
                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                }

                using (SQLiteCommand sqlCommand = new SQLiteCommand(sqlConn))
                {
                    System.Text.StringBuilder command = new System.Text.StringBuilder();
                    command.Append("CREATE INDEX idx_bible_refs on bible_refs (bi,ci,fvi,tvi)");
                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                }
                using (SQLiteCommand sqlCommand = new SQLiteCommand(sqlConn))
                {
                    System.Text.StringBuilder command = new System.Text.StringBuilder();
                    command.Append("CREATE INDEX idx_bible_refs_bi_ci on bible_refs (bi,ci)");
                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                }

                using (SQLiteCommand sqlCommand = new SQLiteCommand(sqlConn))
                {
                    int dir = GlobalMemory.getInstance().Direction;
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
                sbi_config.Insert(new object[] { "type", "2" });
                sbi_config.Insert(new object[] { "schema.version", "1" });
                sbi_config.Insert(new object[] { "title", ABBREVIATION });
                sbi_config.Insert(new object[] { "description", DESCRIPTION });
                sbi_config.Insert(new object[] { "creator", "Simple Bible Reader (" + GlobalMemory.AUTHOR_WEBSITE + ")" });
                sbi_config.Insert(new object[] { "id", "c9" });
                sbi_config.Insert(new object[] { "about", COMMENTS });
                sbi_config.Insert(new object[] { "description.english", DESCRIPTION });
                sbi_config.Insert(new object[] { "categories", "Commentary\\OT, NT" });
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



                SQLiteBulkInsert sbi_bible_refs = new SQLiteBulkInsert(sqlConn, "bible_refs");
                SQLiteBulkInsert sbi_content = new SQLiteBulkInsert(sqlConn, "content");

                sbi_bible_refs.AddParameter("topic_id", DbType.Int32);
                sbi_bible_refs.AddParameter("type", DbType.String);
                sbi_bible_refs.AddParameter("bi", DbType.Int32);
                sbi_bible_refs.AddParameter("ci", DbType.Int32);
                sbi_bible_refs.AddParameter("fvi", DbType.Int32);
                sbi_bible_refs.AddParameter("tvi", DbType.Int32);

                sbi_content.AddParameter("topic_id", DbType.Int32);
                sbi_content.AddParameter("data", DbType.String);

                XmlNodeList books = CommentaryXmlDocument.SelectNodes("/XMLBIBLE/BIBLEBOOK");
                XmlNode vers = null;
                int topic_id = 1;
                string _verse_text = "";

                for (int b = 1; b <= 66; b++)
                {
                    percent_complete = b * 100 / 66;
                    for (int c = 1; c <= FormatUtil.GetChapterCount(b); c++)
                        for (int v = 1; v <= FormatUtil.GetVerseCount(b, c); v++)
                        {
                            vers = CommentaryXmlDocument.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber='" + b.ToString() + "']/CHAPTER[@cnumber='" + c.ToString() + "']/VERS[@vnumber='" + v.ToString() + "']");
                            if (vers != null)
                            {
                                sbi_bible_refs.Insert(new object[] { topic_id, "V", b.ToString(), c.ToString(), v.ToString(), v.ToString() });
                                _verse_text = vers.InnerText;
                                _verse_text = postProcessContent(_verse_text);
                                sbi_content.Insert(new object[] { topic_id, GetRtfUnicodeEscapedString(_verse_text) });
                                topic_id++;
                            }
                        }
                }
                sbi_bible_refs.Flush();
                sbi_content.Flush();
                sqlConn.Dispose();
            }
            percent_complete = 100;
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
