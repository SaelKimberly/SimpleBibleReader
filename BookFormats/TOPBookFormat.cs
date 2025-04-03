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
	public class TOPBookFormat : BookFormat
	{
		System.Xml.XmlElement rootdoc;
		System.Xml.XmlElement bookdoc;
		System.Xml.XmlAttribute attrib;


        string _title_text;
        string _notes_text;

        int percent_complete = 0;

        public System.Windows.Forms.RichTextBox rtb = new System.Windows.Forms.RichTextBox();

		public TOPBookFormat(string file) : base(file)
		{
		}
		
		public override void Load()
		{
            if (Win32NativeCalls.isRunningOnWine())
            {
                Themes.MessageBox("OLE DB is not supported on Linux systems.");
                return;
            }

            BookXmlDocument = new System.Xml.XmlDocument();
			XmlDeclaration dec = BookXmlDocument.CreateXmlDeclaration("1.0", null, null);
			BookXmlDocument.AppendChild(dec);
			rootdoc = BookXmlDocument.CreateElement("XMLBOOK");
			BookXmlDocument.AppendChild(rootdoc);

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

            da = new System.Data.OleDb.OleDbDataAdapter("SELECT * FROM [Topic Notes] ORDER BY ID ASC", conn);
            da.Fill(ds, "Topic Notes");
            for (int i = 0; i <= ds.Tables["Topic Notes"].Rows.Count - 1; i++)
			{
                percent_complete = ((i) * 100) / ds.Tables["Topic Notes"].Rows.Count;
                _title_text = ds.Tables["Topic Notes"].Rows[i]["Title"].ToString();
                _notes_text = ds.Tables["Topic Notes"].Rows[i]["Comments"].ToString();
                _title_text = _title_text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
                _notes_text = _notes_text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
                processLine();				
			}
			conn.Close();
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

            if (GlobalMemory.getInstance().autoSetParsingBook)
            {
                if (_notes_text.StartsWith(@"{\rtf1"))
                    GlobalMemory.getInstance().ConvertRtfToHtmlBook = true;
                GlobalMemory.getInstance().stripHtmlTagsBook = true;
            }
            if (GlobalMemory.getInstance().ParseBook)
            {
                if (!GlobalMemory.getInstance().parseBookBkgndCleaner)
                    _notes_text = postProcessContent(_notes_text);
                bookdoc.InnerText = _notes_text;
            }
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
                return 3;
            }
        }

        public override void ExportBook(string filename, int filter_idx)
        {
            if (Win32NativeCalls.isRunningOnWine())
            {
                Themes.MessageBox("OLE DB is not supported on Linux systems.");
                return;
            }

            //ADOX.Catalog catNewDB = new ADOX.Catalog();

            if (!filename.EndsWith(".top"))
            {
                filename = filename + ".top";
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
            OleDbCommand cmd = new OleDbCommand("CREATE TABLE [Topic Notes] (ID AUTOINCREMENT,Title VARCHAR(50),Comments TEXT)", conn);
            cmd.ExecuteNonQuery();

            XmlNodeList titles = BookXmlDocument.SelectNodes("/XMLBOOK/NOTES/@title");
            XmlNode note = null;
            int ctr = 1;
            string _note_text = "";
            for (int i = 0; i < titles.Count; i++)
            {
                percent_complete = i * 100 / titles.Count;

                note = BookXmlDocument.SelectSingleNode("/XMLBOOK/NOTES[@title='" + titles[i].Value + "']");
                if (note != null)
                {
                    _note_text = note.InnerText.Replace("\n","\\par");
                    byte[] bytes = Encoding.UTF8.GetBytes(_note_text);
                    cmd = new OleDbCommand("INSERT INTO [Topic Notes] (Title,Comments) VALUES('" + titles[i].Value.Replace("'", "''") + "','" + rtf_header+_note_text.Replace("'", "''") + " }')", conn);
                    cmd.ExecuteNonQuery();
                    ctr++;
                }
            }
            conn.Close();
        }
    }
}
