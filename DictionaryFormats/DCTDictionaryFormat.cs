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
	public class DCTDictionaryFormat : DictionaryFormat
	{
        System.Xml.XmlElement rootdoc;
        System.Xml.XmlElement item_doc;
        System.Xml.XmlElement desc_doc;
        System.Xml.XmlAttribute attrib_id;

        public string rtf_header_esword = @"{\rtf1\ansi\ansicpg1252\deff0\deflang1033{\fonttbl{\f1\fnil\fprq2\fcharset161 TITUS Cyberbit Basic;}{\f2\fnil\fprq2\fcharset177 TITUS Cyberbit Basic;}{\f3\fnil\fprq2\fcharset0 TITUS Cyberbit Basic;}{\f4\fnil\fprq2\fcharset136 PMingLiU;}{\f5\fnil\fprq2\fcharset204 Georgia;}{\f6\fnil\fprq2\fcharset238 Georgia;}{\f7\fnil\fprq2\fcharset134 PMingLiU;}{\f8\fnil\fprq2\fcharset222 Cordia New;}{\f9\fnil\fprq2\fcharset129 Gulim;}{\f10\fnil\fprq2\fcharset178 TITUS Cyberbit Basic;}{\f11\fnil\fprq2\fcharset162 Georgia;}{\f12\fnil\fprq2\fcharset163 Times New Roman;}{\f13\fnil\fprq2\fcharset128 MS Mincho;}}{\colortbl ;\red0\green0\blue0;\red0\green0\blue255;\red0\green255\blue255;\red0\green255\blue0;\red255\green0\blue255;\red255\green0\blue0;\red255\green255\blue0;\red255\green255\blue255;\red0\green0\blue128;\red0\green128\blue128;\red0\green128\blue0;\red128\green0\blue128;\red128\green0\blue0;\red128\green128\blue0;\red128\green128\blue128;\red192\green192\blue192;}{\viewkind4\uc1\pard\plain\cf0\fs\par}";
        public System.Windows.Forms.RichTextBox rtb_dct = new System.Windows.Forms.RichTextBox();

        int percent_complete = 0;
	
		public DCTDictionaryFormat(string file) : base(file)
		{
		}
		
		public override void Load()
		{
            if (Win32NativeCalls.isRunningOnWine())
            {
                Themes.MessageBox("OLE DB is not supported on Linux systems.");
                return;
            }

            DictionaryXmlDocument = new System.Xml.XmlDocument();
			XmlDeclaration dec = DictionaryXmlDocument.CreateXmlDeclaration("1.0", null, null);
			DictionaryXmlDocument.AppendChild(dec);
			rootdoc = DictionaryXmlDocument.CreateElement("dictionary");
			DictionaryXmlDocument.AppendChild(rootdoc);

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
            string id = null;
            string item = null;
            da = new System.Data.OleDb.OleDbDataAdapter("SELECT * FROM Dictionary ORDER BY ID ASC", conn);
            da.Fill(ds, "Dictionary");

            for (int i = 0; i <= ds.Tables["Dictionary"].Rows.Count - 1; i++)
			{
                id = ds.Tables["Dictionary"].Rows[i]["Topic"].ToString();
                item = ds.Tables["Dictionary"].Rows[i]["Definition"].ToString();
                percent_complete = (i * 100) / ds.Tables["Dictionary"].Rows.Count;

                item_doc = DictionaryXmlDocument.CreateElement("item");
                attrib_id = DictionaryXmlDocument.CreateAttribute("id");
                attrib_id.Value = id;
                item_doc.Attributes.Append(attrib_id);
                desc_doc = DictionaryXmlDocument.CreateElement("description");
                //
                if (GlobalMemory.getInstance().ParseDictionary)
                {
                    if (CLEANING)
                    {
                        item = Regex.Replace(item, @"([A-Za-z0-9]{0,3})_([0-9]{0,3}):([0-9]{0,3})", @"<reflink target=""$1 $2:$3"">$1 $2:$3</reflink>");
                        if (GlobalMemory.getInstance().autoSetParsingDictionary)
                            GlobalMemory.getInstance().ConvertRtfToHtmlDictionary = true;
                        item = postProcessContent(item, rtf_header_esword);
                        //item = item.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
                        desc_doc.InnerText = item;
                        //
                        //desc_doc.InnerXml = item;
                    }
                    else
                        desc_doc.InnerText = item;
                }
                else
                    desc_doc.InnerXml = FormatUtil.EscapeXML(item);

                changeRefLink(desc_doc);

                item_doc.AppendChild(desc_doc);
                rootdoc.AppendChild(item_doc);
			}
			conn.Close();
			SetProcessAsComplete();
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
                return 2;
            }
        }

        public override void ExportDictionary(string filename, int filter_idx)
        {
            if (Win32NativeCalls.isRunningOnWine())
            {
                Themes.MessageBox("OLE DB is not supported on Linux systems.");
                return;
            }
            //ADOX.Catalog catNewDB = new ADOX.Catalog();

            if (!filename.EndsWith(".dct"))
            {
                filename = filename + ".dct";
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
            OleDbCommand cmd = new OleDbCommand("CREATE TABLE [Dictionary] ([ID] AUTOINCREMENT,[Topic]  VARCHAR(255),[Definition] TEXT)", conn);
            cmd.ExecuteNonQuery();
            // Details Table
            cmd = new OleDbCommand("CREATE TABLE [Details] ([Description] VARCHAR(255),[Abbreviation] VARCHAR(50),[Comments] VARCHAR(255))", conn);
            cmd.ExecuteNonQuery();

            cmd = new OleDbCommand("INSERT INTO [Details] ([Description],[Abbreviation],[Comments]) VALUES('" + DESCRIPTION + "','" + ABBREVIATION + "','"+COMMENTS+"')", conn);
            cmd.ExecuteNonQuery();

            XmlNodeList items = DictionaryXmlDocument.SelectNodes("/dictionary/item");
            string item = null;
            string id = null;
            for (int i = 0; i < items.Count; i++)
            {
                id = items[i].Attributes["id"].Value;                
                item = getRtf(items[i],"");
                item = item.Replace("\n", @"\par ").Trim();
                cmd = new OleDbCommand("INSERT INTO [Dictionary] (Topic,Definition) VALUES('" + id.Replace("'", "''") + "','" + GetRtfUnicodeEscapedString(item.Replace("'", "''")) + "')", conn);
                cmd.ExecuteNonQuery();
            }
            conn.Close();
        }

        private string getRtf(XmlNode child, string dictionary_text)
        {
            if (child.Name == "description")
            {
                if (child.HasChildNodes)
                {
                    foreach (XmlNode c in child.ChildNodes)
                    {
                        dictionary_text = getRtf(c, dictionary_text);
                    }
                }
                else
                    dictionary_text = dictionary_text + child.InnerText;
                //}
            }
            else if (child.Name == "title")
            {
                if (dictionary_text != "")
                    dictionary_text = dictionary_text + @"\par \par \b" + child.InnerText + @"\b\par ";
                else
                    dictionary_text = dictionary_text + @"\b" + child.InnerText + @"\b\par ";
            }
            else if (child.Name == "see")
            {
                /*
                if (child.Attributes["target"].Value == "x-self")
                    dictionary_text = dictionary_text + "<a class='para' href='D" + child.InnerText + "'>" + child.InnerText + "</a><br>";
                else
                    dictionary_text = dictionary_text + "<a class='para' target='_blank' href='" + child.Attributes["target"].Value + "'>" + child.InnerText + "</a><br>";
                */
                dictionary_text = dictionary_text + child.InnerText;
            }
            else if (child.Name == "reflink")
            {
                if (child.Attributes["target"] != null)
                {
                    string[] arr = child.Attributes["target"].Value.Split(';');
                    dictionary_text = dictionary_text + " " + FormatUtil.shortBookNames[int.Parse(arr[0]) - 1] + "_" + arr[1] + ":" + arr[2];
                }
                else if (child.Attributes["mscope"] != null)
                {
                    string[] arr = child.Attributes["mscope"].Value.Split(';');
                    dictionary_text = dictionary_text + " " + FormatUtil.shortBookNames[int.Parse(arr[0]) - 1] + "_" + arr[1] + ":" + arr[2];
                }
            }
            else if (child.Name == "#text")
                dictionary_text = dictionary_text + child.InnerText;
            else
            {
                if (child.HasChildNodes)
                {
                    foreach (XmlNode c in child.ChildNodes)
                    {
                        dictionary_text = getRtf(c, dictionary_text);
                    }
                }
                else
                    dictionary_text = dictionary_text + child.InnerText;
            }
            return dictionary_text;
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
