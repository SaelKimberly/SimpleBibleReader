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
using System.IO;
using System.Text;

namespace Simple_Bible_Reader
{
	public class EasySlidesBibleFormat : BibleFormat
	{
		System.Xml.XmlElement rootdoc;
		System.Xml.XmlElement bookdoc;
		System.Xml.XmlElement chapdoc;
		System.Xml.XmlElement versedoc;
		System.Xml.XmlAttribute attrib;
        System.Xml.XmlAttribute attrib_name;
		
		int _book_no;
		int _chap_no;
		int _verse_no;
		string _verse_text;
		
		int p_book_no = -1;
		int p_chap_no = -1;
		int p_verse_no = -1;

        Hashtable books = new Hashtable();

        public EasySlidesBibleFormat(string file) : base(file)
		{
		}
		
		public override void Load()
		{
            if (Win32NativeCalls.isRunningOnWine())
            {
                Themes.MessageBox("OLE DB is not supported on Linux systems.");
                return;
            }

            BibleXmlDocument = new System.Xml.XmlDocument();
			XmlDeclaration dec = BibleXmlDocument.CreateXmlDeclaration("1.0", null, null);
			BibleXmlDocument.AppendChild(dec);
			rootdoc = BibleXmlDocument.CreateElement("XMLBIBLE");
			BibleXmlDocument.AppendChild(rootdoc);

            System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection();
			

            conn.Close();
            conn = null;
            conn = new System.Data.OleDb.OleDbConnection();
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
            builder.Add("Provider", "Microsoft.Jet.Oledb.4.0");
            //builder.Add("Jet OLEDB:Database Password", "");
            builder.Add("Data Source", FileName);
            conn.ConnectionString = builder.ConnectionString;
            conn.Open();
			
			DataSet ds = new DataSet();
			System.Data.OleDb.OleDbDataAdapter da;
            //
            //
            int tmp = -1;
            books.Clear();
            da = new System.Data.OleDb.OleDbDataAdapter("SELECT VERSE, BIBLETEXT FROM BIBLE WHERE BOOK=0 AND CHAPTER=10", conn);
            da.Fill(ds, "BIBLE");
            String book = "";
            for (int i = 0; i <= ds.Tables["BIBLE"].Rows.Count - 1; i++)
            {
                tmp = int.Parse(ds.Tables["BIBLE"].Rows[i]["VERSE"].ToString()); ;
                book = ds.Tables["BIBLE"].Rows[i]["BIBLETEXT"].ToString();
                books.Add(tmp, book);                
            }
            ds.Clear();
            da.Dispose();
            //
            da = new System.Data.OleDb.OleDbDataAdapter("SELECT BOOK,CHAPTER,VERSE,BIBLETEXT FROM BIBLE WHERE BOOK>0 ORDER BY BOOK,CHAPTER,VERSE ASC", conn);
            da.Fill(ds, "BIBLE");
            for (int i = 0; i <= ds.Tables["BIBLE"].Rows.Count - 1; i++)
			{
                try
                {
                    _book_no = int.Parse(ds.Tables["BIBLE"].Rows[i]["BOOK"].ToString()); ;
                    _chap_no = int.Parse(ds.Tables["BIBLE"].Rows[i]["CHAPTER"].ToString());
                    _verse_no = int.Parse(ds.Tables["BIBLE"].Rows[i]["VERSE"].ToString());
                    _verse_text = ds.Tables["BIBLE"].Rows[i]["BIBLETEXT"].ToString();
                    processLine();
                }
                catch (Exception)
                {
                    continue;
                }                
			}
            ds.Clear();
            da.Dispose();
            ///
           
            da = new System.Data.OleDb.OleDbDataAdapter("SELECT VERSE, BIBLETEXT FROM BIBLE WHERE BOOK=0 AND CHAPTER=0", conn);
            da.Fill(ds, "BIBLE");
            for (int i = 0; i <= ds.Tables["BIBLE"].Rows.Count - 1; i++)
            {
                tmp = int.Parse(ds.Tables["BIBLE"].Rows[i]["VERSE"].ToString()); ;
                if(tmp==0)
                    this.DESCRIPTION = ds.Tables["BIBLE"].Rows[i]["BIBLETEXT"].ToString();
                else if(tmp==1)
                    this.ABBREVIATION = ds.Tables["BIBLE"].Rows[i]["BIBLETEXT"].ToString();
            }
            ds.Clear();
            da.Dispose();           

			conn.Close();
			SetProcessAsComplete();
		}
		
		
		private void processLine()
		{
			if (_book_no != p_book_no || (_chap_no == 1 && _verse_no == 1 && p_verse_no >= _verse_no))
			{                
				bookdoc = (XmlElement)BibleXmlDocument.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber='"+_book_no+"']");
                if (bookdoc == null)
                {
                    bookdoc = BibleXmlDocument.CreateElement("BIBLEBOOK");
                    attrib = BibleXmlDocument.CreateAttribute("bnumber");
                    attrib.Value = _book_no.ToString();
                    bookdoc.Attributes.Append(attrib);

                    if (books.Count > 0)
                    {
                        attrib_name = BibleXmlDocument.CreateAttribute("bname");
                        attrib_name.Value = books[_book_no].ToString();
                        bookdoc.Attributes.Append(attrib_name);
                    }
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
                    if (!GlobalMemory.getInstance().parseBibleBkgndCleaner)
                        _verse_text = postProcessContent(_verse_text);
                    //_verse_text = _verse_text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
                    versedoc.InnerXml = FormatUtil.EscapeXML(_verse_text);
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
				return ((_book_no) * 100) / 66;
			}
		}

        public override int FilterIndex
        {
            get
            {
                return F21_EASY_SLIDES;
            }
        }

        public override void ExportBible(string filename, int filter_idx)
        {
            if (Win32NativeCalls.isRunningOnWine())
            {
                Themes.MessageBox("OLE DB is not supported on Linux systems.");
                return;
            }

            //ADOX.Catalog catNewDB = new ADOX.Catalog();

            if (!filename.EndsWith(".mdb"))
            {
                filename = filename + ".mdb";
            }
            if (filename.Equals(""))
            {
                return;
            }
            if(System.IO.File.Exists(filename))
                System.IO.File.Delete(filename);

            //catNewDB.Create("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filename);
            File.WriteAllBytes(filename, Properties.Resources.db_mdb);
            OleDbConnection conn = new  OleDbConnection();
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
			builder.Add("Provider", "Microsoft.Jet.Oledb.4.0");
			//builder.Add("Jet OLEDB:Database Password", "");
			builder.Add("Data Source", FileName);
			conn.ConnectionString = builder.ConnectionString;            
            conn.Open();
            // Bible Table
            OleDbCommand cmd = new OleDbCommand("CREATE TABLE BIBLE (BOOK NUMBER,CHAPTER NUMBER,VERSE NUMBER, BIBLETEXT MEMO)", conn);
            cmd.ExecuteNonQuery();

            cmd = new OleDbCommand("INSERT INTO BIBLE (BOOK,CHAPTER,VERSE,BIBLETEXT) VALUES(0,0,0,'"+this.DESCRIPTION+"')", conn);
            cmd.ExecuteNonQuery();
            cmd = new OleDbCommand("INSERT INTO BIBLE (BOOK,CHAPTER,VERSE,BIBLETEXT) VALUES(0,0,1,'" + this.ABBREVIATION + "')", conn);
            cmd.ExecuteNonQuery();
            cmd = new OleDbCommand("INSERT INTO BIBLE (BOOK,CHAPTER,VERSE,BIBLETEXT) VALUES(0,0,2,'')", conn);
            cmd.ExecuteNonQuery();
            cmd = new OleDbCommand("INSERT INTO BIBLE (BOOK,CHAPTER,VERSE,BIBLETEXT) VALUES(0,0,3,'" + this.DESCRIPTION + "')", conn);
            cmd.ExecuteNonQuery();
            cmd = new OleDbCommand("INSERT INTO BIBLE (BOOK,CHAPTER,VERSE,BIBLETEXT) VALUES(0,0,4,'" + this.COMMENTS + "')", conn);
            cmd.ExecuteNonQuery();


            XmlNodeList books = BibleXmlDocument.SelectNodes("/XMLBIBLE/BIBLEBOOK");
            XmlNode vers = null;
            //

            foreach (XmlNode node in books)
            {
                cmd = new OleDbCommand("INSERT INTO BIBLE (BOOK,CHAPTER,VERSE,BIBLETEXT) VALUES(0,10," + node.Attributes["bnumber"].Value + ",'" + node.Attributes["bname"].Value.Replace("'", "''") + "')", conn); 
                cmd.ExecuteNonQuery();
            }

            //
            int ctr = 1;
            for (int b = 1; b <= 66; b++)
            {
                _book_no = b;
                for (int c = 1; c <= FormatUtil.GetChapterCount(b); c++)
                    for (int v = 1; v <= FormatUtil.GetVerseCount(b, c); v++)
                    {
                        vers = BibleXmlDocument.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber='" + b.ToString() + "']/CHAPTER[@cnumber='" + c.ToString() + "']/VERS[@vnumber='" + v.ToString() + "']");

                        if (vers != null)
                        {
                            cmd = new OleDbCommand("INSERT INTO BIBLE (BOOK,CHAPTER,VERSE,BIBLETEXT) VALUES(" + b.ToString() + "," + c.ToString() + "," + v.ToString() + ",'" + vers.InnerText.Replace("'","''") + "')", conn);
                            cmd.ExecuteNonQuery();
                            ctr++;
                        }
                    }
            }
            conn.Close();
        }
    }
}
