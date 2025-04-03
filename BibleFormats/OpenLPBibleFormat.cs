using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SQLite;
using System.Xml;
using System.IO;
using RtfPipe.Tokens;

namespace Simple_Bible_Reader
{
    class OpenLPBibleFormat : BibleFormat
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
        List<string[]> kv = new List<string[]>();
        String mDBVERSION = "2";
        String mCOPYRIGHT = "Eternity, God";
        String mPERMISSIONS = "Scripure is a fact. A fact translated is still a fact. No fact can be copyrighted in any country. Hence, all Scriptures are on Public Domain.";

        public OpenLPBibleFormat(string file)
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
            mycommand.CommandText = "SELECT book_id, chapter, verse, text FROM verse ORDER BY book_id, chapter, verse";
            SQLiteDataReader reader = mycommand.ExecuteReader();

            dt.Load(reader);

            mycommand.Dispose();
            reader.Dispose();
            cnn.Dispose();
            int bno = 0;
            for (int row = 0; row < dt.Rows.Count; row++)
            {

                bno = int.Parse(dt.Rows[row][0].ToString());
                if (bno > 66)
                    break;
                percent_complete =  ((_book_no) * 100) / 66;
                _book_no = bno;
                _chap_no = int.Parse(dt.Rows[row][1].ToString());
                _verse_no = int.Parse(dt.Rows[row][2].ToString());
                _verse_text = dt.Rows[row][3].ToString();
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
                return F18_OPENLP;
            }
        }

        public override List<string[]> getProperties()
        {
            if (kv.Count == 0)
            {
                kv.Add(new string[] { "Abbreviation:", BibleFormat.getInstance().ABBREVIATION });
                kv.Add(new string[] { "Description:", BibleFormat.getInstance().DESCRIPTION });
                kv.Add(new string[] { "Comments:", BibleFormat.getInstance().COMMENTS });
                kv.Add(new string[] { "DB Version:", mDBVERSION });
                kv.Add(new string[] { "Copyright:", mCOPYRIGHT});
                kv.Add(new string[] { "Permissions:", mPERMISSIONS });
            }
            return kv;
        }

        public override void setProperties(List<string[]> m_kv)
        {
            this.kv = m_kv;
        }

        private void processProperties()
        {
            for (int i = 0; i < kv.Count; i++)
            {
               
                
                switch (kv[i][0])
                {
                    case "Abbreviation:":
                        BibleFormat.getInstance().ABBREVIATION = kv[i][1];
                        break;
                    case "Description:":
                        BibleFormat.getInstance().DESCRIPTION = kv[i][1];
                        break;
                    case "Comments:":
                        BibleFormat.getInstance().COMMENTS = kv[i][1];
                        break;
                    case "DB Version:":
                        mDBVERSION = kv[i][1];
                        break;
                    case "Copyright:":
                        mCOPYRIGHT = kv[i][1];
                        break;
                    case "Permissions:":
                        mPERMISSIONS = kv[i][1];
                        break;
                }
            }
        }

        public override void ExportBible(string filename, int filter_idx)
        {
            if (!filename.EndsWith(".sqlite"))
            {
                filename = filename + ".sqlite";
            }
            if (filename.Equals(""))
            {
                return;
            }
			processProperties();
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
                    command.Append("CREATE TABLE book (	id INTEGER NOT NULL, testament_id INTEGER, name VARCHAR(50), abbreviation VARCHAR(5), PRIMARY KEY (id), FOREIGN KEY(testament_id) REFERENCES testament (id))");
                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                }

                SQLiteBulkInsert sbi1 = new SQLiteBulkInsert(sqlConn, "book");
                sbi1.AddParameter("id", DbType.Int32);
                sbi1.AddParameter("testament_id", DbType.Int32);
                sbi1.AddParameter("name", DbType.String);
                sbi1.AddParameter("abbreviation", DbType.String);

                // metadata
                using (SQLiteCommand sqlCommand = new SQLiteCommand(sqlConn))
                {
                    System.Text.StringBuilder command = new System.Text.StringBuilder();
                    command.Append("CREATE TABLE metadata (\"key\" VARCHAR(255) NOT NULL, value VARCHAR(255), PRIMARY KEY (\"key\"))");
                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                }
                //values
                SQLiteBulkInsert sbi2 = new SQLiteBulkInsert(sqlConn, "metadata");
                sbi2.AddParameter("key", DbType.String);
                sbi2.AddParameter("value", DbType.String);
                sbi2.Insert(new object[] { "dbversion", "2"});
                sbi2.Insert(new object[] { "Version", ABBREVIATION});
                sbi2.Insert(new object[] { "Copyright", "Eternity, God" });
                sbi2.Insert(new object[] { "Permissions","Scripure is a fact. A fact translated is still a fact. No fact can be copyrighted in any country. Hence, all Scriptures are on Public Domain." });
                sbi2.Flush();

                // testament
                using (SQLiteCommand sqlCommand = new SQLiteCommand(sqlConn))
                {
                    System.Text.StringBuilder command = new System.Text.StringBuilder();
                    command.Append("CREATE TABLE testament (id INTEGER NOT NULL, name VARCHAR(50), PRIMARY KEY (id))");
                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                }
                //values
                sbi2 = new SQLiteBulkInsert(sqlConn, "testament");
                sbi2.AddParameter("id", DbType.Int32);
                sbi2.AddParameter("name", DbType.String);
                sbi2.Insert(new object[] { 1, "Old Testament" });
                sbi2.Insert(new object[] { 2, "New Testament" });
                sbi2.Insert(new object[] { 3, "Apocrypha" });
                sbi2.Flush();


                //
                // verse
                using (SQLiteCommand sqlCommand = new SQLiteCommand(sqlConn))
                {
                    System.Text.StringBuilder command = new System.Text.StringBuilder();
                    command.Append("CREATE TABLE verse (id INTEGER NOT NULL, book_id INTEGER, chapter INTEGER, verse INTEGER, text TEXT, PRIMARY KEY (id), FOREIGN KEY(book_id) REFERENCES book (id))");
                    sqlCommand.CommandText = command.ToString();
                    sqlCommand.ExecuteNonQuery();
                }

                SQLiteBulkInsert vsbi = new SQLiteBulkInsert(sqlConn, "verse");
                vsbi.AddParameter("id", DbType.Int32);
                vsbi.AddParameter("book_id", DbType.Int32);
                vsbi.AddParameter("chapter", DbType.Int32);
                vsbi.AddParameter("verse", DbType.Int32);
                vsbi.AddParameter("text", DbType.String);

                XmlNodeList books = BibleXmlDocument.SelectNodes("/XMLBIBLE/BIBLEBOOK");
                XmlNode vers = null;
                string book_name = "";
                int ctr = 1;
                string _verse_text = "";
                for (int b = 1; b <= 66; b++)
                {
                    book_name = Localization.getBookNames()[b - 1];
                    if (books[b - 1] != null)
                      if(books[b - 1].Attributes.GetNamedItem("bname")!=null)
                        book_name = books[b - 1].Attributes.GetNamedItem("bname").Value;
                    sbi1.Insert(new object[] { b.ToString(), 1, book_name, FormatUtil.shortBookNames[b - 1] });
                    percent_complete = b * 100 / 66;
                    for (int c = 1; c <= FormatUtil.GetChapterCount(b); c++)
                        for (int v = 1; v <= FormatUtil.GetVerseCount(b, c); v++)
                        {
                            vers = BibleXmlDocument.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber='" + b.ToString() + "']/CHAPTER[@cnumber='" + c.ToString() + "']/VERS[@vnumber='" + v.ToString() + "']");
                            if (vers != null)
                            {
                                _verse_text = vers.InnerText;                                
                                vsbi.Insert(new object[] { ctr, b, c, v, _verse_text });
                                ctr++;
                            }
                        }
                }
                vsbi.Flush();
                sbi1.Flush();
                
                ///Indexes
                ///
                string[] index_sqls = new string[]{
                    "CREATE INDEX ix_book_abbreviation ON book (abbreviation)",
                    "CREATE INDEX ix_book_name ON book (name)",
                    "CREATE INDEX ix_metadata_key ON metadata (\"key\")",
                    "CREATE INDEX ix_verse_book_id ON verse (book_id)",
                    "CREATE INDEX ix_verse_chapter ON verse (chapter)",
                    "CREATE INDEX ix_verse_id ON verse (id)",
                    "CREATE INDEX ix_verse_text ON verse (text)",
                    "CREATE INDEX ix_verse_verse ON verse (verse)"
                };
                foreach (string sql in index_sqls)
                {
                    using (SQLiteCommand sqlCommand = new SQLiteCommand(sqlConn))
                    {
                        System.Text.StringBuilder command = new System.Text.StringBuilder();
                        command.Append(sql);
                        sqlCommand.CommandText = command.ToString();
                        sqlCommand.ExecuteNonQuery();
                    }
                }
                
            }
            percent_complete = 100;
        }             

    }
}
