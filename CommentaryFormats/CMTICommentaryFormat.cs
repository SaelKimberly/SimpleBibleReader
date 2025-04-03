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
using System.Linq;

namespace Simple_Bible_Reader
{    

    public class CMTICommentaryFormat : CommentaryFormat
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

        public CMTICommentaryFormat(string file)
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

            try
            {
                DataTable dt = new DataTable();
                string data_src = @"" + this.FileName;
                SQLiteConnection cnn = new SQLiteConnection("Data Source=" + data_src);
                cnn.Open();
                SQLiteCommand mycommand = new SQLiteCommand(cnn);
                SQLiteDataReader reader = null;

                try
                {
                    // latest version
                    mycommand.CommandText = "SELECT Book, ChapterBegin, VerseBegin, Comments FROM VerseCommentary ORDER BY Book, ChapterBegin, VerseBegin";
                    reader = mycommand.ExecuteReader();
                }
                catch (Exception)
                {
                    // latest version 9.x
                    mycommand.CommandText = "SELECT Book, ChapterBegin, VerseBegin, Comments FROM Verses ORDER BY Book, ChapterBegin, VerseBegin";
                    reader = mycommand.ExecuteReader();
                }

                

                dt.Load(reader);
                //
                
                mycommand = new SQLiteCommand(cnn);
                try
                {
                    // latest 
                    mycommand.CommandText = "SELECT Title,Abbreviation FROM Details";
                    reader = mycommand.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        ABBREVIATION = (string)reader["Abbreviation"];
                        DESCRIPTION = (string)reader["Title"];
                    }
                }
                catch (Exception)
                {
                    // 9.x
                    mycommand.CommandText = "SELECT Description,Abbreviation FROM Details";
                    reader = mycommand.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        ABBREVIATION = (string)reader["Abbreviation"];
                        DESCRIPTION = (string)reader["Description"];
                    }
                }
                


                mycommand.Dispose();
                reader.Dispose();
                cnn.Dispose();

                percent_complete = 0;
                byte[] _verse_bytes, _compressed_bytes;
                int zlibhdr = 0;
                int override_compression_count = 0;
                int total_verse_count = 0;
                for (int row = 0; row < dt.Rows.Count; row++)
                {
                    bno = int.Parse(dt.Rows[row][0].ToString());
                    if (bno > 66)
                        break;

                    percent_complete = (bno * 100) / 66;

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
            catch (Exception)
            {
                
            }           
            SetProcessAsComplete();
            
        }


        private void processLine()
        {
            if (_book_no != p_book_no || (_chap_no == 1 && _verse_no == 1 && p_verse_no >= _verse_no))
            {
                bookdoc = (XmlElement)CommentaryXmlDocument.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber='"+_book_no+"']");
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
                //
                if (GlobalMemory.getInstance().ParseCommentary)
                {
                    if (CLEANING)
                    {

                        //rtb.Rtf = rtf_header + _verse_text;
                        //_verse_text = rtb.Text;
                        //_verse_text = _verse_text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
                        //_verse_text = Regex.Replace(_verse_text, @"([A-Za-z0-9]{0,3})_([0-9]{0,3}):([0-9]{0,3})", @"<reflink target=""$1 $2:$3"">$1 $2:$3</reflink>");


                        //_verse_text = Regex.Replace(_verse_text, @"([A-Za-z0-9]{0,3})_([0-9]{0,3}):([0-9]{0,3})", @"<reflink target=""$1 $2:$3"">$1 $2:$3</reflink>");
                        _verse_text = Regex.Replace(_verse_text, @"(Gen|Exo|Lev|Num|Deu|Jos|Jdg|Rut|1Sa|2Sa|1Ki|2Ki|1Ch|2Ch|Ezr|Neh|Est|Job|Psa|Pro|Ecc|Sol|Isa|Jer|Lam|Eze|Dan|Hos|Joe|Amo|Oba|Jon|Mic|Nah|Hab|Zep|Hag|Zec|Mal|Mat|Mar|Luk|Joh|Act|Rom|1Co|2Co|Gal|Eph|Phi|Col|1Th|2Th|1Ti|2Ti|Tit|Phm|Heb|Jam|1Pe|2Pe|1Jo|2Jo|3Jo|Jud|Rev)[\s_]([0-9]{1,2}):([0-9]{1,3})", @"<reflink target=""$1 $2:$3"">$1 $2:$3</reflink>");
                        if (GlobalMemory.getInstance().autoSetParsingCommentary)
                            GlobalMemory.getInstance().ConvertRtfToHtmlCommentary = true;
                        _verse_text = postProcessContent(_verse_text);
                        versedoc.InnerText = _verse_text;

                        //
                        try
                        {
                            // inner XML required for linking fallback is inner text
                            versedoc.InnerXml = _verse_text;
                        }
                        catch (Exception e)
                        {
                            versedoc.InnerText = _verse_text;
                        }
                        changeRefLink(versedoc);
                    }
                    else
                        versedoc.InnerText = _verse_text;
                }
                else
                    versedoc.InnerXml = FormatUtil.EscapeXML( _verse_text);

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
                return 6;
            }
        }

        public override void ExportCommentary(string filename, int filter_idx)
        {
            if (!filename.EndsWith(".cmti"))
            {
                filename = filename + ".cmti";
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
                    command.Append("CREATE TABLE BookCommentary (Book INT, Comments TEXT)");
                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                }
                using (SQLiteCommand sqlCommand = new SQLiteCommand(sqlConn))
                {
                    System.Text.StringBuilder command = new System.Text.StringBuilder();
                    command.Append("CREATE TABLE ChapterCommentary (Book INT, Chapter INT, Comments TEXT)");
                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                }

                SQLiteBulkInsert sbi_book = new SQLiteBulkInsert(sqlConn, "BookCommentary");
                SQLiteBulkInsert sbi_chap = new SQLiteBulkInsert(sqlConn, "ChapterCommentary");
                sbi_book.AddParameter("Book", DbType.Int32);
                sbi_book.AddParameter("Comments", DbType.String);
                sbi_chap.AddParameter("Book", DbType.Int32);
                sbi_chap.AddParameter("Chapter", DbType.Int32);
                sbi_chap.AddParameter("Comments", DbType.String);

                for (int i = 1; i <= 66; i++)
                {
                    sbi_book.Insert(new object[] {i.ToString() ,""});
                    for (int j = 0; j < FormatUtil.GetChapterCount(i); j++)
                    {
                        sbi_chap.Insert(new object[] { i.ToString(), j.ToString(),""});
                    }
                }
                sbi_book.Flush();
                sbi_chap.Flush();

                using (SQLiteCommand sqlCommand = new SQLiteCommand(sqlConn))
                {
                    System.Text.StringBuilder command = new System.Text.StringBuilder();
                    command.Append("CREATE TABLE VerseCommentary (Book INT, ChapterBegin INT, VerseBegin INT,ChapterEnd INT,  VerseEnd INT, Comments TEXT)");

                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                }

                using (SQLiteCommand sqlCommand = new SQLiteCommand(sqlConn))
                {
                    int dir=GlobalMemory.getInstance().Direction;
                    System.Text.StringBuilder command = new System.Text.StringBuilder();
                    command.Append("CREATE INDEX BookChapterIndex ON ChapterCommentary (Book, Chapter)");
                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                }
                
                using (SQLiteCommand sqlCommand = new SQLiteCommand(sqlConn))
                {
                    System.Text.StringBuilder command = new System.Text.StringBuilder();
                    command.Append("CREATE INDEX BookChapterVerseIndex ON VerseCommentary (Book, ChapterBegin, VerseBegin)");
                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                }

                using (SQLiteCommand sqlCommand = new SQLiteCommand(sqlConn))
                {
                    System.Text.StringBuilder command = new System.Text.StringBuilder();
                    command.Append("CREATE TABLE Details (Title NVARCHAR(255), Abbreviation NVARCHAR(50), Information TEXT, Version INT)");
                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                }
                using (SQLiteCommand sqlCommand = new SQLiteCommand(sqlConn))
                {
                    System.Text.StringBuilder command = new System.Text.StringBuilder();
                    command.Append("INSERT INTO Details VALUES('" + DESCRIPTION.Replace("\'", "\'\'") + "','" + ABBREVIATION.Replace("\'", "\'\'") + "','" + COMMENTS.Replace("\'", "\'\'") + "',2)");
                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                }
                
                SQLiteBulkInsert sbi = new SQLiteBulkInsert(sqlConn, "VerseCommentary");
                sbi.AddParameter("Book", DbType.Int32);
                sbi.AddParameter("ChapterBegin", DbType.Int32);
                sbi.AddParameter("VerseBegin", DbType.Int32);
                sbi.AddParameter("ChapterEnd", DbType.Int32);                
                sbi.AddParameter("VerseEnd", DbType.Int32);
                sbi.AddParameter("Comments", DbType.String);

                
                XmlNodeList books = CommentaryXmlDocument.SelectNodes("/XMLBIBLE/BIBLEBOOK");
                XmlNode vers = null;
                string _verse_text = "";
                for (int b = 1; b <= 66; b++)
                {
                    percent_complete=b*100/66;
                    for (int c = 1; c <= FormatUtil.GetChapterCount(b); c++)
                        for (int v = 1; v <= FormatUtil.GetVerseCount(b, c); v++)
                        {
                            vers = CommentaryXmlDocument.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber='" + b.ToString() + "']/CHAPTER[@cnumber='" + c.ToString() + "']/VERS[@vnumber='" + v.ToString() + "']");
                            if (vers != null)
                            {
                                /*
                                if (GlobalMemory.getInstance().ParseCmtry && CLEANING)
                                {
                                    _verse_text = FormatUtil.ConvertToText(rtf_header + vers.InnerText);
                                }
                                else
                                {
                                    _verse_text = vers.InnerText;
                                }
                                */

                                _verse_text = vers.InnerText;
                                _verse_text = Regex.Replace(_verse_text, @"(Gen|Exo|Lev|Num|Deu|Jos|Jdg|Rut|1Sa|2Sa|1Ki|2Ki|1Ch|2Ch|Ezr|Neh|Est|Job|Psa|Pro|Ecc|Sol|Isa|Jer|Lam|Eze|Dan|Hos|Joe|Amo|Oba|Jon|Mic|Nah|Hab|Zep|Hag|Zec|Mal|Mat|Mar|Luk|Joh|Act|Rom|1Co|2Co|Gal|Eph|Phi|Col|1Th|2Th|1Ti|2Ti|Tit|Phm|Heb|Jam|1Pe|2Pe|1Jo|2Jo|3Jo|Jud|Rev)[\s_]([0-9]{1,2}):([0-9]{1,3})", "$1_$2:$3");
                                if (vers != null)
                                    sbi.Insert(new object[] { b.ToString(), c.ToString(), v.ToString(), c.ToString(), v.ToString(), GetRtfUnicodeEscapedString(_verse_text) });
                            }
                        }
                }
                sbi.Flush();
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
        }
            
    }
