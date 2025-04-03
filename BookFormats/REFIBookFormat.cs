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
using Ionic.Zlib;
using System.Text;

namespace Simple_Bible_Reader
{

    public class REFIBookFormat : BookFormat
    {

        System.Xml.XmlElement rootdoc;
        System.Xml.XmlElement bookdoc;
        System.Xml.XmlAttribute attrib;

        string _title_text;
        string _notes_text;
        byte[] _notes_bytes, _compressed_bytes;
        int percent_complete = 0;
        
        public System.Windows.Forms.RichTextBox rtb = new System.Windows.Forms.RichTextBox();

        public REFIBookFormat(string file)
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

            percent_complete = 0;

            try
            {
                DataTable dt = new DataTable();
                string data_src = @"" + this.FileName;
                SQLiteConnection cnn = new SQLiteConnection("Data Source=" + data_src);
                cnn.Open();
                SQLiteCommand mycommand = new SQLiteCommand(cnn);
                SQLiteDataReader reader = null;

                mycommand.CommandText = "SELECT Chapter, Content FROM Reference";
                try
                {
                    reader = mycommand.ExecuteReader();
                }
                catch (Exception)
                {
                    // refx converted from topx
                    // retry with topics table
                    mycommand.CommandText = "SELECT Title, Notes FROM topics";
                    reader = mycommand.ExecuteReader();
                }

                dt.Load(reader);

                mycommand.Dispose();
                reader.Dispose();
                cnn.Dispose();

                percent_complete = 50;
                int zlibhdr = 0;
                int override_compression_count = 0;
                int total_verse_count = 0;
                for (int row = 0; row < dt.Rows.Count; row++)
                {
                    percent_complete = 50 + ((row) * 50) / dt.Rows.Count;

                    _title_text = dt.Rows[row][0].ToString();
                    _title_text = _title_text.Replace("\"", "").Replace("'", "");

                    try
                    {
                        if (dt.Rows[row][1] is string)
                        {
                            // unencrypted
                            _notes_text = dt.Rows[row][1].ToString();
                        }
                        else
                        {
                            //detect if compressed
                            zlibhdr = FormatUtil.findZlibHeaderIdx((byte[])dt.Rows[row][1], 0xB);


                            if (zlibhdr == -1) // probably encrypted
                            {
                                _notes_bytes = FormatUtil.eSwordTwoFishDecrypt((byte[])dt.Rows[row][1]);
                                zlibhdr = FormatUtil.findZlibHeaderIdx(_notes_bytes, 0xB);
                                override_compression_count++;
                            }
                            else
                            {
                                // we need to be absolutely sure that it is not encrypted.

                                if (override_compression_count * 100 / total_verse_count > 90)
                                {
                                    _notes_bytes = FormatUtil.eSwordTwoFishDecrypt((byte[])dt.Rows[row][1]);
                                    zlibhdr = FormatUtil.findZlibHeaderIdx(_notes_bytes, 0xB);                                   
                                }
                                else
                                    _notes_bytes = (byte[])dt.Rows[row][1];
                            }

                            if (_notes_bytes.Length > 0xB)
                            {
                                
                                if (_notes_bytes[zlibhdr] == 0x78) // compressed with zlib 
                                {
                                    _compressed_bytes = new byte[_notes_bytes.Length - zlibhdr];
                                    Array.Copy(_notes_bytes, zlibhdr, _compressed_bytes, 0, _notes_bytes.Length - zlibhdr);
                                    _notes_text = Encoding.UTF8.GetString(FormatUtil.eSword_zLibUncompress(_compressed_bytes));
                                }
                                else
                                    _notes_text = Encoding.UTF8.GetString(_notes_bytes);
                            }
                            else
                                _notes_text = Encoding.UTF8.GetString(_notes_bytes);
                        }
                    }
                    catch (Exception)
                    {
                        _notes_text = Encoding.UTF8.GetString((byte[])dt.Rows[row][1]);
                    }

                    if (GlobalMemory.getInstance().ParseBook)
                    {
                        // refi is not rtf but checking doesn't hurt
                        if (GlobalMemory.getInstance().autoSetParsingBook)
                            GlobalMemory.getInstance().ConvertRtfToHtmlBook = true;

                        if (!GlobalMemory.getInstance().parseBookBkgndCleaner)
                            _notes_text = postProcessContent(_notes_text);
                    }

                    total_verse_count++;
                    processLine();
                }
            }
            catch (Exception ex)
            {

            }
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
            if(GlobalMemory.getInstance().ParseBook)
                bookdoc.InnerText = _notes_text;
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
                return 5;
            }
        }

        public override void ExportBook(string filename, int filter_idx)
        {
            
            if (!filename.EndsWith(".refi"))
            {
                filename = filename + ".refi";
            }

            if (filename.Equals(""))
            {
                return;
            }
            if (System.IO.File.Exists(filename))
                System.IO.File.Delete(filename);

            percent_complete = 0;

            SQLiteBulkInsert sbi = null;

            using (SQLiteConnection sqlConn = new SQLiteConnection())
            {
                sqlConn.ConnectionString = "Data source = " + filename;
                sqlConn.Open();
                SQLiteCommand sqlCommand;
                StringBuilder command;

                sqlCommand = new SQLiteCommand(sqlConn);
                sqlCommand.CommandText = "PRAGMA encoding = \"UTF-16le\"";
                sqlCommand.ExecuteNonQuery();
                sqlCommand.Dispose();


                //Details
                sqlCommand = new SQLiteCommand(sqlConn);
                command = new System.Text.StringBuilder();
                command.Append("CREATE TABLE Details (Title NVARCHAR(100), Abbreviation NVARCHAR(50),Information TEXT,Version INT)");
                sqlCommand.CommandText = command.ToString();
                sqlCommand.ExecuteNonQuery();
                sqlCommand.Dispose();


                sqlCommand = new SQLiteCommand(sqlConn);
                command = new System.Text.StringBuilder();
                command.Append("INSERT INTO Details VALUES ('" + DESCRIPTION + "','" + ABBREVIATION + "','" + COMMENTS + "',2)");
                sqlCommand.CommandText = command.ToString();
                sqlCommand.ExecuteNonQuery();
                sqlCommand.Dispose();

               
                // Reference
                sqlCommand = new SQLiteCommand(sqlConn);
                command = new System.Text.StringBuilder();
                command.Append("CREATE TABLE Reference (Chapter NVARCHAR (100), Content TEXT)");
                sqlCommand.CommandText = command.ToString();
                sqlCommand.ExecuteNonQuery();
                sqlCommand.Dispose();

                sbi = new SQLiteBulkInsert(sqlConn, "Reference");
                sbi.AddParameter("Chapter", DbType.String);
                sbi.AddParameter("Content", DbType.String);
                

                try
                {

                ///
                XmlNodeList titles = BookXmlDocument.SelectNodes("/XMLBOOK/NOTES/@title");
                XmlNode note = null;
                string _note_text = "";
                for (int i = 0; i < titles.Count; i++)
                {
                    percent_complete = i * 100 / titles.Count;

                    note = BookXmlDocument.SelectSingleNode("/XMLBOOK/NOTES[@title='" + titles[i].Value + "']");
                    if (note != null)
                    {
                        _note_text = note.InnerText;                        
                        sbi.Insert(new object[] { titles[i].Value, _note_text });
                    }
                }

                }
                catch (Exception e)
                {
                   
                }

                sbi.Flush();
            }
            percent_complete = 100;
        }

    }
}
