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
	public class CMTCommentaryFormat : CommentaryFormat
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

        public System.Windows.Forms.RichTextBox rtb = new System.Windows.Forms.RichTextBox();

		int p_book_no = -1;
		int p_chap_no = -1;
		int p_verse_no = -1;
		
		public CMTCommentaryFormat(string file) : base(file)
		{
		}
		
		public override void Load()
		{
            if (Win32NativeCalls.isRunningOnWine())
            {
                Themes.MessageBox("OLE DB is not supported on Linux systems.");
                return;
            }

            CommentaryXmlDocument = new System.Xml.XmlDocument();
			XmlDeclaration dec = CommentaryXmlDocument.CreateXmlDeclaration("1.0", null, null);
			CommentaryXmlDocument.AppendChild(dec);
			rootdoc = CommentaryXmlDocument.CreateElement("XMLBIBLE");
			CommentaryXmlDocument.AppendChild(rootdoc);

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

            da = new System.Data.OleDb.OleDbDataAdapter("SELECT * FROM Commentary ORDER BY [Book ID],Chapter,[Start Verse] ASC", conn);
            da.Fill(ds, "Commentary");
			int bno = -1;
            for (int i = 0; i <= ds.Tables["Commentary"].Rows.Count - 1; i++)
			{
                bno = int.Parse(ds.Tables["Commentary"].Rows[i]["Book ID"].ToString());
				if (bno > 66)
				{
                    continue;
				}
				_book_no = bno;
                _chap_no = int.Parse(ds.Tables["Commentary"].Rows[i]["Chapter"].ToString());
                _verse_no = int.Parse(ds.Tables["Commentary"].Rows[i]["Start Verse"].ToString());
                _verse_text = ds.Tables["Commentary"].Rows[i]["Comments"].ToString();
                
                processLine();
			}
			conn.Close();
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
                        //
                        if (GlobalMemory.getInstance().autoSetParsingCommentary)
                            GlobalMemory.getInstance().ConvertRtfToHtmlCommentary = true;
                        _verse_text = postProcessContent(_verse_text);

                        versedoc.InnerText = _verse_text;
                        changeRefLink(versedoc);
                    }
                    else
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
				return ((_book_no) * 100) / 66;
			}
		}

        public override int FilterIndex
        {
            get
            {
                return 4;
            }
        }

        public override void ExportCommentary(string filename, int filter_idx)
        {
            if (Win32NativeCalls.isRunningOnWine())
            {
                Themes.MessageBox("OLE DB is not supported on Linux systems.");
                return;
            }

            //ADOX.Catalog catNewDB = new ADOX.Catalog();

            if (!filename.EndsWith(".cmt"))
            {
                filename = filename + ".cmt";
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
			builder.Add("Jet OLEDB:Database Password", "");
			builder.Add("Data Source", FileName);
			conn.ConnectionString = builder.ConnectionString;            
            conn.Open();
            // Bible Table
            OleDbCommand cmd = new OleDbCommand("CREATE TABLE [Commentary] ([ID] AUTOINCREMENT,[Book ID] SMALLINT,[Chapter] SMALLINT,[Start Verse] SMALLINT,[End Verse] SMALLINT,[Comments] TEXT)", conn);
            cmd.ExecuteNonQuery();
            // Details Table
            cmd = new OleDbCommand("CREATE TABLE [Details] ([Description] VARCHAR(255),[Abbreviation] VARCHAR(50),[Comments] VARCHAR(255))", conn);
            cmd.ExecuteNonQuery();

            cmd = new OleDbCommand("INSERT INTO [Details] ([Description],[Abbreviation],[Comments]) VALUES('" + DESCRIPTION + "','" + ABBREVIATION + "','" + COMMENTS + "')", conn);
            cmd.ExecuteNonQuery();

            XmlNodeList books = CommentaryXmlDocument.SelectNodes("/XMLBIBLE/BIBLEBOOK");
            XmlNode vers = null;
            
            int ctr = 1;
            string _verse_text = "";
            for (int b = 1; b <= 66; b++)
            {
                _book_no = b;
                for (int c = 1; c <= FormatUtil.GetChapterCount(b); c++)
                    for (int v = 1; v <= FormatUtil.GetVerseCount(b, c); v++)
                    {
                        vers = CommentaryXmlDocument.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber='" + b.ToString() + "']/CHAPTER[@cnumber='" + c.ToString() + "']/VERS[@vnumber='" + v.ToString() + "']");

                        if (vers != null)
                        {
                            _verse_text = vers.InnerText;                            
                            cmd = new OleDbCommand("INSERT INTO [Commentary] ([Book ID],Chapter,[Start Verse],[End Verse],Comments) VALUES(" + b.ToString() + "," + c.ToString() + "," + v.ToString() + "," + v.ToString() + ",'" + GetRtfUnicodeEscapedString(_verse_text.Replace("'", "''")) + "')", conn);
                            cmd.ExecuteNonQuery();
                            ctr++;
                        }
                    }
            }
            conn.Close();
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
