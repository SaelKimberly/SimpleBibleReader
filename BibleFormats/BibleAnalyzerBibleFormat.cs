using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SQLite;
using System.Xml;
using System.IO;
using System.Windows.Forms;

namespace Simple_Bible_Reader
{
    class BibleAnalyzerBibleFormat : BibleFormat
    {
        System.Xml.XmlElement rootdoc;
        System.Xml.XmlElement bookdoc;
        System.Xml.XmlElement chapdoc;
        System.Xml.XmlElement versedoc;
        System.Xml.XmlAttribute attrib;
        int percent_complete = 0;
        int _book_no;
        int _chap_no;
        int _verse_no;
        string _verse_text;

        int p_book_no = -1;
        int p_chap_no = -1;
        int p_verse_no = -1;
        public System.Windows.Forms.RichTextBox rtb = new System.Windows.Forms.RichTextBox();

        static string[] bibShortBookNames = new string[] { "Gen", "Exo", "Lev", "Num", "Deu", "Jos", "Jdg", "Rth", "1Sa", "2Sa", "1Ki", "2Ki", "1Ch", "2Ch", "Ezr", "Neh", "Est", "Job", "Psa", "Pro", "Ecc", "Son", "Isa", "Jer", "Lam", "Eze", "Dan", "Hos", "Joe", "Amo", "Oba", "Jon", "Mic", "Nah", "Hab", "Zep", "Hag", "Zec", "Mal", "Mat", "Mar", "Luk", "Joh", "Act", "Rom", "1Co", "2Co", "Gal", "Eph", "Phi", "Col", "1Th", "2Th", "1Ti", "2Ti", "Tit", "Phm", "Heb", "Jam", "1Pe", "2Pe", "1Jo", "2Jo", "3Jo", "Jud", "Rev" };


        public BibleAnalyzerBibleFormat(string file)
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


            DataTable dt = new DataTable();
            string data_src = @"" + this.FileName;
            SQLiteConnection cnn = new SQLiteConnection("Data Source=" + data_src);
            cnn.Open();
            SQLiteCommand mycommand = new SQLiteCommand(cnn);
            mycommand.CommandText = "SELECT id, ref, verse FROM bible ORDER BY id";
            SQLiteDataReader reader = mycommand.ExecuteReader();

            dt.Load(reader);

            mycommand.Dispose();
            reader.Dispose();
            cnn.Dispose();

            string tmp;
            string[] data;
            for (int row = 0; row < dt.Rows.Count; row++)
            {

                percent_complete =  ((_book_no) * 100) / 66;
                
                tmp = dt.Rows[row][1].ToString();
                tmp = tmp.Replace(' ', ':');
                data = tmp.Split(':');

                for (int i = 0; i < bibShortBookNames.Length; i++)
                {
                    if (data[0].ToLower().Equals(bibShortBookNames[i].ToLower()))
                    {
                        _book_no = i + 1;
                        break;
                    }
                }

                _chap_no = int.Parse(data[1]);
                _verse_no = int.Parse(data[2]);
                _verse_text = dt.Rows[row][2].ToString();
                processLine();
            }
			SetProcessAsComplete();
		}

        private void processLine()
        {
            if (_book_no != p_book_no || (_chap_no == 1 && _verse_no == 1 && p_verse_no >= _verse_no))
            {
                bookdoc = BibleXmlDocument.CreateElement("BIBLEBOOK");
                attrib = BibleXmlDocument.CreateAttribute("bnumber");
                attrib.Value = _book_no.ToString();
                bookdoc.Attributes.Append(attrib);
                rootdoc.AppendChild(bookdoc);
            }
            if (_chap_no != p_chap_no || (_chap_no == 1 && _verse_no == 1 && p_verse_no >= _verse_no))
            {
                chapdoc = BibleXmlDocument.CreateElement("CHAPTER");
                attrib = BibleXmlDocument.CreateAttribute("cnumber");
                attrib.Value = _chap_no.ToString();
                chapdoc.Attributes.Append(attrib);
                bookdoc.AppendChild(chapdoc);
            }
            if (_verse_no != p_verse_no || (_verse_no == p_verse_no && _chap_no != p_chap_no) || (_verse_no == p_verse_no && _book_no != p_book_no))
            {
                versedoc = BibleXmlDocument.CreateElement("VERS");
                attrib = BibleXmlDocument.CreateAttribute("vnumber");
                attrib.Value = _verse_no.ToString();
                if (GlobalMemory.getInstance().ParseBible)
                {
                    if (!GlobalMemory.getInstance().parseBibleBkgndCleaner)
                        _verse_text = postProcessContent(_verse_text);
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
                return F20_BIBLE_ANALYZER;
            }
        }

        public override void ExportBible(string filename, int filter_idx)
        {
            if (!filename.EndsWith(".bib"))
            {
                filename = filename + ".bib";
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
                
                //
                // verse
                SQLiteCommand sqlCommand = new SQLiteCommand(sqlConn);
                
                    System.Text.StringBuilder command = new System.Text.StringBuilder();
                    command.Append("CREATE TABLE bible (id INTEGER,ref,verse,PRIMARY KEY (id))");
                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();

                SQLiteBulkInsert vsbi = new SQLiteBulkInsert(sqlConn, "bible");
                vsbi.AddParameter("id", DbType.Int32);
                vsbi.AddParameter("ref", DbType.String);
                vsbi.AddParameter("verse", DbType.String);

                XmlNodeList books = BibleXmlDocument.SelectNodes("/XMLBIBLE/BIBLEBOOK");
                XmlNode vers = null;
                string book_name = "";
                int ctr = 1;
                string refverse;
                string _verse_text = "";
                for (int b = 1; b <= 66; b++)
                {
                    book_name = Localization.getBookNames()[b - 1];
                    if (books[b - 1] != null)
                      if(books[b - 1].Attributes.GetNamedItem("bname")!=null)
                        book_name = books[b - 1].Attributes.GetNamedItem("bname").Value;
                    percent_complete = b * 100 / 66;
                    for (int c = 1; c <= FormatUtil.GetChapterCount(b); c++)
                        for (int v = 1; v <= FormatUtil.GetVerseCount(b, c); v++)
                        {
                            vers = BibleXmlDocument.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber='" + b.ToString() + "']/CHAPTER[@cnumber='" + c.ToString() + "']/VERS[@vnumber='" + v.ToString() + "']");
                            if (vers != null)
                            {
                                refverse = bibShortBookNames[b - 1] + " " + c + ":" + v;
                                _verse_text = vers.InnerText;                                
                                vsbi.Insert(new object[] { ctr, refverse, _verse_text + "\r\n" });
                                ctr++;
                            }
                        }
                }
                vsbi.getSQLiteCommand().Dispose();
                vsbi.Flush();
                
                    sqlCommand = new SQLiteCommand(sqlConn);                   
                    command = new System.Text.StringBuilder();
                    command.Append("CREATE TABLE title (abbr,desc,info)");
                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();                    
                    sqlCommand.Dispose();

                    sqlCommand = new SQLiteCommand(sqlConn);
                    command = new System.Text.StringBuilder();                   
                    command.Append("INSERT INTO title VALUES('" + ABBREVIATION + "','"+DESCRIPTION+"','"+COMMENTS+"')");
                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();                    

                    sqlCommand = new SQLiteCommand(sqlConn);
                    command = new System.Text.StringBuilder();
                    command.Append("CREATE TABLE option (strongs,interlinear)");
                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();

                    sqlCommand = new SQLiteCommand(sqlConn);
                    command = new System.Text.StringBuilder();
                    command.Append("INSERT INTO option VALUES('0','0')");
                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();

                    sqlConn.Dispose();
                
            }
            percent_complete = 100;
        }
    }
}


