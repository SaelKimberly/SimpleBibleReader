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
	public class BBLBibleFormat : BibleFormat
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
        public System.Windows.Forms.RichTextBox rtb = new System.Windows.Forms.RichTextBox();
        
        String mSTRONG = "FALSE";

        public BBLBibleFormat(string file) : base(file)
		{

		}

        public override void Load()
		{
            if(Win32NativeCalls.isRunningOnWine())
            {
                Themes.MessageBox("OLE DB is not supported on Linux systems.");
                return;
            }

			BibleXmlDocument = new System.Xml.XmlDocument();
			XmlDeclaration dec = BibleXmlDocument.CreateXmlDeclaration("1.0", null, null);
			BibleXmlDocument.AppendChild(dec);
			rootdoc = BibleXmlDocument.CreateElement("XMLBIBLE");
			BibleXmlDocument.AppendChild(rootdoc);

            string[] default_password = Passwords.eSword8_Passwords;
			System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection();
			bool bbl_open_success = false;
			
			foreach (string password in default_password)
			{
				try
				{
					conn.Close();
					conn = null;
					conn = new System.Data.OleDb.OleDbConnection();
                    OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
					builder.Add("Provider", "Microsoft.Jet.Oledb.4.0");
					builder.Add("Jet OLEDB:Database Password", password);
					builder.Add("Data Source", FileName);
					conn.ConnectionString = builder.ConnectionString;
                    conn.Open();
					bbl_open_success = true;
					goto endOfForLoop;
				}
				catch (Exception)
				{
					continue;
				}
			}
endOfForLoop:
			
			if (! bbl_open_success)
			{
				conn.Close();
				SetProcessAsComplete();
				return;
			}
			
			DataSet ds = new DataSet();
			System.Data.OleDb.OleDbDataAdapter da;

            da = new System.Data.OleDb.OleDbDataAdapter("SELECT * FROM Bible ORDER BY [Book ID],Chapter,Verse ASC", conn);
			da.Fill(ds, "Bible");
			int bno = -1;
			for (int i = 0; i <= ds.Tables["Bible"].Rows.Count - 1; i++)
			{
                bno = int.Parse(ds.Tables["Bible"].Rows[i]["Book ID"].ToString());

                if (FormatUtil.eSword2Zefania_Apocrypha_Map.ContainsKey(bno))
                    bno = FormatUtil.eSword2Zefania_Apocrypha_Map[bno];

                /* -- process all books for apocrypha
				if (bno > 66)
				{
                    continue;
				}
                */
                _book_no = bno;
				_chap_no = int.Parse(ds.Tables["Bible"].Rows[i]["Chapter"].ToString());
                _verse_no = int.Parse(ds.Tables["Bible"].Rows[i]["Verse"].ToString());
                _verse_text = ds.Tables["Bible"].Rows[i]["Scripture"].ToString();
                
                processLine();
			}
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
				return ((_book_no) * 100) / 110;
			}
		}

        public override int FilterIndex
        {
            get
            {
                return F09_EWORD_BBL;
            }
        }


        public override List<string[]> getProperties()
        {
            if (kv_export.Count == 0)
            {
                kv_export.Add(new string[] { "Abbreviation:", BibleFormat.getInstance().ABBREVIATION });
                kv_export.Add(new string[] { "Description:", BibleFormat.getInstance().DESCRIPTION });
                kv_export.Add(new string[] { "Comments:", BibleFormat.getInstance().COMMENTS });
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
                    case "Strongs:":
                        mSTRONG = kv_export[i][1];
                        break;
                }
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

            if (!filename.EndsWith(".bbl"))
            {
                filename = filename + ".bbl";
            }
            if (filename.Equals(""))
            {
                return;
            }
			processProperties();
            if(System.IO.File.Exists(filename))
                System.IO.File.Delete(filename);

            //catNewDB.Create("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filename);
            File.WriteAllBytes(filename, Properties.Resources.db_mdb);
            OleDbConnection conn = new  OleDbConnection();
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
			builder.Add("Provider", "Microsoft.Jet.Oledb.4.0");
			builder.Add("Jet OLEDB:Database Password", "");
			builder.Add("Data Source", FileName);
			conn.ConnectionString = builder.ConnectionString;            
            conn.Open();
            // Bible Table
            OleDbCommand cmd = new OleDbCommand("CREATE TABLE [Bible] ([ID] AUTOINCREMENT,[Book ID] SMALLINT,[Chapter] SMALLINT,[Verse] SMALLINT,[Scripture] TEXT)", conn);
            cmd.ExecuteNonQuery();
            // Details Table
            cmd = new OleDbCommand("CREATE TABLE [Details] ([Description] VARCHAR(255),[Abbreviation] VARCHAR(50),[Comments] VARCHAR(255),[Font] VARCHAR(50),[Apocrypha] YesNo,[Strongs] YesNo)", conn);
            cmd.ExecuteNonQuery();
            
            cmd = new OleDbCommand("INSERT INTO [Details] ([Description],[Abbreviation],[Comments],[Font],[Apocrypha],[Strongs]) VALUES('" + DESCRIPTION + "','" + ABBREVIATION + "','"+COMMENTS+"','DEFAULT',"+BibleFormat.isApocrypha.ToString().ToUpper()+","+ mSTRONG + ")", conn);
            cmd.ExecuteNonQuery();

            XmlNodeList books = BibleXmlDocument.SelectNodes("/XMLBIBLE/BIBLEBOOK");
            XmlNode vers = null;
            
            int ctr = 1;
            string _verse_text = "";
            int m_book_no = -1;
            int total_books = FormatUtil.GetBookCount2();
            foreach (XmlNode book in books)
            {
                try
                {
                    int b = int.Parse(book.Attributes.GetNamedItem("bnumber").Value);
                    m_book_no = b;

                    if (FormatUtil.Zefania2eSword_Apocrypha_Map.ContainsKey(m_book_no))
                        m_book_no = FormatUtil.Zefania2eSword_Apocrypha_Map[m_book_no];

                    for (int c = 1; c <= FormatUtil.GetChapterCount2(b); c++)
                        for (int v = 1; v <= FormatUtil.GetVerseCount2(b, c); v++)
                        {
                            vers = BibleXmlDocument.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber='" + b.ToString() + "']/CHAPTER[@cnumber='" + c.ToString() + "']/VERS[@vnumber='" + v.ToString() + "']");

                            if (vers != null)
                            {
                                _verse_text = processExportVerse(vers, b);
                                cmd = new OleDbCommand("INSERT INTO [Bible] ([Book ID],Chapter,Verse,Scripture) VALUES(" + m_book_no.ToString() + "," + c.ToString() + "," + v.ToString() + ",'" + FormatUtil.GetRtfUnicodeEscapedString(_verse_text.Replace("'", "''")) + "')", conn);
                                cmd.ExecuteNonQuery();
                                ctr++;
                            }
                        }
                }
                catch (Exception e)
                {
                    //Themes.DarkMessageBoxShow(e.Message);
                }
                
            }
            conn.Close();
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
                if (_verse_text.IndexOf("<STYLE") != -1 )
                {
                    _verse_text = _verse_text.Replace("<STYLE css=\"color:#ff0000\">", "{\\cf6 ");
                    _verse_text = _verse_text.Replace("</STYLE>", "}");
                }
                if (_verse_text.IndexOf("<i>") != -1)
                {
                    _verse_text = _verse_text.Replace("<i>", "{\\cf15\\i ");
                    _verse_text = _verse_text.Replace("</i>", "}");
                }
                if (_verse_text.IndexOf("<gr") != -1)
                {
                    _verse_text = Regex.Replace(_verse_text, "(<gr )([^s]*)(str=['\"]*)([^<'\"]*)['\"]*(>)(((?!</gr).)*)</gr>", @"$6{\cf11\super " + prefix + @"$4}", RegexOptions.IgnoreCase);
                }
            }
            else
            {
                _verse_text = vers.InnerText;
            }
            return _verse_text;
        }
    }
}
