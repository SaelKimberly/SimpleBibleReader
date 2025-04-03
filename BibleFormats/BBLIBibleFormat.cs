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
using RtfPipe.Tokens;
using RtfPipe;

namespace Simple_Bible_Reader
{

    public class BBLIBibleFormat : BibleFormat
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

        int percent_complete = 0;
        public string rtf_header = @"{\rtf1\ansi\ansicpg1252\deff0\deflang1033{\fonttbl{\f1\fnil\fprq2\fcharset161 TITUS Cyberbit Basic;}{\f2\fnil\fprq2\fcharset177 TITUS Cyberbit Basic;}{\f3\fnil\fprq2\fcharset0 TITUS Cyberbit Basic;}{\f4\fnil\fprq2\fcharset136 PMingLiU;}{\f5\fnil\fprq2\fcharset204 Georgia;}{\f6\fnil\fprq2\fcharset238 Georgia;}{\f7\fnil\fprq2\fcharset134 PMingLiU;}{\f8\fnil\fprq2\fcharset222 Cordia New;}{\f9\fnil\fprq2\fcharset129 Gulim;}{\f10\fnil\fprq2\fcharset178 TITUS Cyberbit Basic;}{\f11\fnil\fprq2\fcharset162 Georgia;}{\f12\fnil\fprq2\fcharset163 Times New Roman;}{\f13\fnil\fprq2\fcharset128 MS Mincho;}}{\colortbl ;\red0\green0\blue0;\red0\green0\blue255;\red0\green255\blue255;\red0\green255\blue0;\red255\green0\blue255;\red255\green0\blue0;\red255\green255\blue0;\red255\green255\blue255;\red0\green0\blue128;\red0\green128\blue128;\red0\green128\blue0;\red128\green0\blue128;\red128\green0\blue0;\red128\green128\blue0;\red128\green128\blue128;\red192\green192\blue192;}{\viewkind4\uc1\pard\plain\cf0\fs\par}";
        public System.Windows.Forms.RichTextBox rtb = new System.Windows.Forms.RichTextBox();
        
        String mVERSION = "4";
        String mFONT = "DEFAULT";
        String mOT = "1";
        String mNT = "1";
        String mSTRONG = "0";

        public BBLIBibleFormat(string file)
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

            try
            {
                DataTable dt = new DataTable();
                string data_src = @"" + this.FileName;
                SQLiteConnection cnn = new SQLiteConnection("Data Source=" + data_src);
                cnn.Open();
                SQLiteCommand mycommand = new SQLiteCommand(cnn);
                mycommand.CommandText = "SELECT Book, Chapter, Verse, Scripture FROM Bible ORDER BY Book, Chapter, Verse";
                SQLiteDataReader reader = mycommand.ExecuteReader();

                dt.Load(reader);
                mycommand.Dispose();
                reader.Dispose();
                cnn.Dispose();

                percent_complete = 50;
                byte[] _verse_bytes, _compressed_bytes;
                int zlibhdr = 0;
                int override_compression_count = 0;
                int total_verse_count = 0;
                for (int row = 0; row < dt.Rows.Count; row++)
                {
                    bno = int.Parse(dt.Rows[row][0].ToString());

                    if (FormatUtil.eSword2Zefania_Apocrypha_Map.ContainsKey(bno))
                        bno = FormatUtil.eSword2Zefania_Apocrypha_Map[bno];

                    /* -- removed to allow apocrypha
                     
                    if (bno > 66)
                        break;

                    */
                    percent_complete = 50 + ((_book_no) * 50) / 110;
                    _book_no = bno;
                    _chap_no = int.Parse(dt.Rows[row][1].ToString());
                    _verse_no = int.Parse(dt.Rows[row][2].ToString());
                    if (dt.Rows[row][3] is string)
                    {
                        _verse_text = dt.Rows[row][3].ToString();
                    }
                    else
                    {
                        try
                        {
                            //detect if compressed
                            zlibhdr = FormatUtil.findZlibHeaderIdx((byte[])dt.Rows[row][3], 0xB);

                            if (zlibhdr == -1) // probably encrypted
                            {
                                _verse_bytes = FormatUtil.eSwordTwoFishDecrypt((byte[])dt.Rows[row][3]);
                                zlibhdr = FormatUtil.findZlibHeaderIdx(_verse_bytes, 0xB);
                                override_compression_count++;
                            }
                            else
                            {
                                // we need to be absolutely sure that it is not encrypted.

                                if (override_compression_count * 100 / total_verse_count > 90)
                                {
                                    _verse_bytes = FormatUtil.eSwordTwoFishDecrypt((byte[])dt.Rows[row][3]);
                                    zlibhdr = FormatUtil.findZlibHeaderIdx(_verse_bytes, 0xB);
                                }
                                else
                                    _verse_bytes = (byte[])dt.Rows[row][3];
                            }

                            if (_verse_bytes.Length > 0xB)
                            {
                                zlibhdr = FormatUtil.findZlibHeaderIdx(_verse_bytes, 0xB);
                                if (_verse_bytes[zlibhdr] == 0x78) // compressed with zlib 
                                {
                                    _compressed_bytes = new byte[_verse_bytes.Length - zlibhdr];
                                    Array.Copy(_verse_bytes, zlibhdr, _compressed_bytes, 0, _verse_bytes.Length - zlibhdr);
                                    _verse_text = Encoding.UTF8.GetString(FormatUtil.eSword_zLibUncompress(_compressed_bytes));
                                }
                                else
                                    _verse_text = Encoding.UTF8.GetString(_verse_bytes);
                            }
                            else
                                _verse_text = Encoding.UTF8.GetString(_verse_bytes);
                        }
                        catch (Exception)
                        {
                            _verse_text = Encoding.UTF8.GetString((byte[])dt.Rows[row][3]);

                        }
                    }
                    total_verse_count++;
                    processLine();
                }
            }
            catch (Exception e)
            {
                //Themes.DarkMessageBoxShow(e.Message);
            }
            ////////////////////////////////////////

            SetProcessAsComplete();
        }


        private void processLine()
        {
            if (_book_no != p_book_no || (_chap_no == 1 && _verse_no == 1 && p_verse_no >= _verse_no))
            {
                bookdoc = (XmlElement)BibleXmlDocument.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber='" + _book_no + "']");
                if (bookdoc == null)
                {
                    bookdoc = BibleXmlDocument.CreateElement("BIBLEBOOK");
                    attrib = BibleXmlDocument.CreateAttribute("bnumber");
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
                    if (GlobalMemory.getInstance().autoSetParsingBible)
                        GlobalMemory.getInstance().ConvertRtfToHtmlBible = true;

                    if (!GlobalMemory.getInstance().parseBibleBkgndCleaner)
                        _verse_text = postProcessContent(_verse_text);
                    versedoc.InnerText = _verse_text;
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
                return F11_ESWORD_BBLI;
            }
        }


        public override List<string[]> getProperties()
        {
            if (kv_export.Count == 0)
            {
                kv_export.Add(new string[] { "Abbreviation:", BibleFormat.getInstance().ABBREVIATION });
                kv_export.Add(new string[] { "Description:", BibleFormat.getInstance().DESCRIPTION });
                kv_export.Add(new string[] { "Comments:", BibleFormat.getInstance().COMMENTS });
                kv_export.Add(new string[] { "Version:", mVERSION });
                kv_export.Add(new string[] { "Font:", mFONT });
                kv_export.Add(new string[] { "Old Testament:", mOT });
                kv_export.Add(new string[] { "New Testament:", mNT });
                kv_export.Add(new string[] { "Strongs:", mSTRONG });
            }
            return kv_export;
        }

        public override void setProperties(List<string[]> m_kv)
        {
            kv_export = m_kv;
        }
        private void processProperties()
        {
            for (int i = 0; i < kv_export.Count; i++)
            {
               
                
                switch (kv_export[i][0])
                {
                    case "Abbreviation:":
                        BibleFormat.getInstance().ABBREVIATION = kv_export[i][1];
                        break;
                    case "Description:":
                        BibleFormat.getInstance().DESCRIPTION = kv_export[i][1];
                        break;
                    case "Comments:":
                        BibleFormat.getInstance().COMMENTS = kv_export[i][1];
                        break;
                    case "Version:":
                        mVERSION = kv_export[i][1];
                        break;
                    case "Font:":
                        mFONT = kv_export[i][1];
                        break;
                    case "Old Testament:":
                        mOT = kv_export[i][1];
                        break;
                    case "New Testament:":
                        mNT = kv_export[i][1];
                        break;
                    case "Strongs:":
                        mSTRONG = kv_export[i][1];
                        break;
                }
            }
        }

        public override void ExportBible(string filename, int filter_idx)
        {
            if (!filename.EndsWith(".bbli"))
            {
                filename = filename + ".bbli";
            }
            if (filename.Equals(""))
            {
                return;
            }
            if (System.IO.File.Exists(filename))
                System.IO.File.Delete(filename);

            SQLiteConnection.CreateFile(filename);
            percent_complete = 0;
			processProperties();
            using (SQLiteConnection sqlConn = new SQLiteConnection())
            {
                sqlConn.ConnectionString = "Data source = " + filename;
                sqlConn.Open();
                using (SQLiteCommand sqlCommand = new SQLiteCommand(sqlConn))
                {
                    System.Text.StringBuilder command = new System.Text.StringBuilder();
                    command.Append("CREATE TABLE 'Bible' (Book INT, Chapter INT, Verse INT, Scripture TEXT)");

                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                }

                using (SQLiteCommand sqlCommand = new SQLiteCommand(sqlConn))
                {
                    System.Text.StringBuilder command = new System.Text.StringBuilder();
                    command.Append("CREATE TABLE Details ( Title NVARCHAR (100), Abbreviation NVARCHAR (50), Information TEXT, Version INT, OldTestament BOOL, NewTestament BOOL, Apocrypha BOOL, Strongs BOOL, RightToLeft BOOL)");
                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                }

                using (SQLiteCommand sqlCommand = new SQLiteCommand(sqlConn))
                {
                    System.Text.StringBuilder command = new System.Text.StringBuilder();
                    command.Append("CREATE INDEX BookChapterVerseIndex ON Bible (Book, Chapter, Verse)");
                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                }

                using (SQLiteCommand sqlCommand = new SQLiteCommand(sqlConn))
                {
                    int dir = GlobalMemory.getInstance().Direction;
                    System.Text.StringBuilder command = new System.Text.StringBuilder();

                    int apocrypha = 0;
                    if (BibleFormat.isApocrypha)
                        apocrypha = 1;
                    /*
                    command.Append("INSERT INTO Details VALUES('" + Path.GetFileName(filename) + "','" + Path.GetFileNameWithoutExtension(filename) +
                        "','Exported from Simple Bible Reader (" + GlobalMemory.AUTHOR_WEBSITE + ")',1,1,1," + apocrypha.ToString() + ",1,0)");
                    */
                    command.Append("INSERT INTO Details VALUES('" + DESCRIPTION + "','" + ABBREVIATION + "','" + COMMENTS + "',"+mVERSION+","+mOT+","+mNT+"," + apocrypha.ToString() + ","+mSTRONG+","+dir.ToString()+")");

                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                }


                SQLiteBulkInsert sbi = new SQLiteBulkInsert(sqlConn, "Bible");
                sbi.AddParameter("Book", DbType.Int32);
                sbi.AddParameter("Chapter", DbType.Int32);
                sbi.AddParameter("Verse", DbType.Int32);
                sbi.AddParameter("Scripture", DbType.String);


                XmlNodeList books = BibleXmlDocument.SelectNodes("/XMLBIBLE/BIBLEBOOK");
                XmlNode vers = null;
                int book_no = -1;
                foreach (XmlNode book in books)
                {
                    try
                    {
                        int b = int.Parse(book.Attributes.GetNamedItem("bnumber").Value);
                        percent_complete = b * 100 / FormatUtil.GetBookCount2();

                        book_no = b;
                        if (FormatUtil.Zefania2eSword_Apocrypha_Map.ContainsKey(book_no))
                            book_no = FormatUtil.Zefania2eSword_Apocrypha_Map[book_no];

                        for (int c = 1; c <= FormatUtil.GetChapterCount2(b); c++)
                            for (int v = 1; v <= FormatUtil.GetVerseCount2(b, c); v++)
                            {
                                vers = BibleXmlDocument.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber='" + b.ToString() + "']/CHAPTER[@cnumber='" + c.ToString() + "']/VERS[@vnumber='" + v.ToString() + "']");
                                if (vers != null)
                                {
                                    _verse_text = processExportVerse(vers, b);
                                    _verse_text = FormatUtil.UnescapeXML(_verse_text);
                                    //sbi.Insert(new object[] { book_no.ToString(), c.ToString(), v.ToString(), GetRtfUnicodeEscapedString(_verse_text) });
                                    sbi.Insert(new object[] { book_no.ToString(), c.ToString(), v.ToString(), _verse_text });
                                }
                            }
                    }
                    catch (Exception)
                    {

                    }

                }
                sbi.Flush();
                sqlConn.Dispose();
            }
            percent_complete = 100;
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
                if (_verse_text.IndexOf("<STYLE") != -1)
                {
                    //_verse_text = _verse_text.Replace("<STYLE css=\"color:#ff0000\">", "{\\cf6 ");
                    //_verse_text = _verse_text.Replace("</STYLE>", "}");
                }
                if (_verse_text.IndexOf("<i>") != -1)
                {
                    //_verse_text = _verse_text.Replace("<i>", "{\\cf15\\i ");
                    //_verse_text = _verse_text.Replace("</i>", "}");
                }
                if (_verse_text.IndexOf("<gr") != -1)
                {
                    //_verse_text = Regex.Replace(_verse_text, "(<gr )([^s]*)(str=['\"]*)([^<'\"]*)['\"]*(>)(((?!</gr).)*)</gr>", @"$6{\cf11\super " + prefix + @"$4}", RegexOptions.IgnoreCase);
                    _verse_text = Regex.Replace(_verse_text, "(<gr )([^s]*)(str=['\"]*)([^<'\"]*)['\"]*(>)(((?!</gr).)*)</gr>", "$6<num>" + prefix + "$4</num>", RegexOptions.IgnoreCase);
                }
            }
            else
                _verse_text = vers.InnerText;
            return _verse_text;
        }
    }
}
