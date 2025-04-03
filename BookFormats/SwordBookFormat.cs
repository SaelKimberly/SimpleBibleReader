using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;

using System;
using System.Collections;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using Sword;
using ICSharpCode.SharpZipLib.Zip;
using System.Text;
using Sword.reader;
using Ionic.Zlib;
using System.Linq;
using System.Net;

namespace Simple_Bible_Reader
{
		public class SwordBookFormat : BookFormat
		{
            System.Xml.XmlElement rootdoc;
            System.Xml.XmlElement note_doc;
            System.Xml.XmlAttribute title_id;

            public System.Windows.Forms.RichTextBox rtb = new System.Windows.Forms.RichTextBox();

            int export_percent_complete=0;
            int percent_complete = 0;
		
		public SwordBookFormat(string file) : base(file)
		{
		}
		
		public override void Load()
		{
			string conf = "";
			string TempDir = "";
            string module_name = "";

            if (System.IO.File.Exists((string) FileName) && FileName.ToLower().EndsWith(".zip"))
			{
                TempDir = System.IO.Path.GetTempPath() + "SimpleBibleReader_" + new Random(DateTime.Now.Millisecond).Next(100000000, 999999999).ToString() + "\\";
				FastZip fz = new FastZip();
				fz.CreateEmptyDirectories = true;
				fz.ExtractZip(FileName, TempDir, "");
			}
			else if (System.IO.Directory.Exists((string) FileName))
			{
				TempDir = (string) (FileName + "\\");
			}
			else
			{
				return;
			}
			
			conf = (string) (Directory.GetFiles(TempDir + "mods.d\\", "*.conf")[0]);
			
			Dictionary<string, string> data = new Dictionary<string, string>();
			foreach (string row in File.ReadAllLines(conf))
			{
                if (row.StartsWith("[") && row.EndsWith("]"))
                {
                    module_name = row.Substring(1,row.Length-2);
                    ABBREVIATION = module_name;
                    continue;
                }
				try
				{
					data.Add(row.Split("=".ToCharArray())[0].Trim(), row.Split("=".ToCharArray())[1].Trim());
                    if (row.Split("=".ToCharArray())[0].Trim().ToLower() == "description")
                        DESCRIPTION = row.Split("=".ToCharArray())[1].Trim();
				}
				catch (Exception)
				{
				}
			}
            
			//ModDrv - RawText, zText
			//CompressType - LZSS or Zip
            
            
            BookXmlDocument = new System.Xml.XmlDocument();
            XmlDeclaration dec = BookXmlDocument.CreateXmlDeclaration("1.0", null, null);
            BookXmlDocument.AppendChild(dec);
            rootdoc = BookXmlDocument.CreateElement("XMLBOOK");
            BookXmlDocument.AppendChild(rootdoc);

            if (GlobalMemory.getInstance().use_libsword)
            {
                using (var manager = new Manager(TempDir))
                {
                    var module = manager.GetModuleByName(module_name);
                    module.Begin();
                    
                    string curr_text = "";
                    string prev_text = "";
                    string prev_key = "";
                    string curr_key = "";

                    while (true)
                    {                       
                        module.Next();
                        curr_key = module.KeyText;
                        if (GlobalMemory.getInstance().ParseBook)
                            curr_text = FormatUtil.convertDefaultToUTFEncoding(module.StripText());
                        else
                            curr_text = FormatUtil.convertDefaultToUTFEncoding(module.RawEntry);
                        
                        if (curr_text == prev_text && curr_key == prev_key)
                            break;
                        
                        note_doc = BookXmlDocument.CreateElement("NOTES");
                        title_id = BookXmlDocument.CreateAttribute("title");

                        title_id.Value = module.KeyText.Replace("'", "_");
                        note_doc.Attributes.Append(title_id);
                        if(GlobalMemory.getInstance().ParseBook)
                            note_doc.InnerText = FormatUtil.EscapeXML(FormatUtil.convertDefaultToUTFEncoding(module.StripText()));
                        else
                            note_doc.InnerXml = FormatUtil.EscapeXML(FormatUtil.convertDefaultToUTFEncoding(module.StripText()));
                        rootdoc.AppendChild(note_doc);
                        prev_key = curr_key;
                        prev_text = curr_text;
                    }
                }
            }
            else
            {
                long loc = 0;
                long len = 0;


                byte[] bytes = null;
                string str = null;
                string id = null;

                FileStream bdt_reader = null;
                BinaryReader idx_reader = null;
                BinaryReader dat_reader = null;

                string bdt_path = null;
                string idx_path = null;
                string dat_path = null;

                //////////// NO ENCRYPTION

                bdt_path = TempDir + data["DataPath"] + ".bdt";
                idx_path = TempDir + data["DataPath"] + ".idx";
                dat_path = TempDir + data["DataPath"] + ".dat";

                bdt_reader = File.Open(bdt_path, FileMode.Open);
                idx_reader = new BinaryReader(File.Open(idx_path, FileMode.Open), Encoding.ASCII);
                dat_reader = new BinaryReader(File.Open(dat_path, FileMode.Open), Encoding.ASCII);

                int total_percent = 100;
                byte tmp = 0, ptmp = 0;
                StringBuilder sb = new StringBuilder();
                if (data["ModDrv"].ToLower() == "rawgenbook")
                {

                    long bdt_offset = 0, bdt_length = 0;

                    while (idx_reader.PeekChar() != -1)
                    {
                        try
                        {
                            //idx_reader.ReadInt32();// skip 8 bytes   
                            loc = idx_reader.ReadUInt32();
                            //
                            percent_complete = (int)((idx_reader.BaseStream.Position * total_percent) / idx_reader.BaseStream.Length);
                            dat_reader.BaseStream.Seek(loc + 0x0C, SeekOrigin.Begin);

                            id = "";
                            sb.Length = 0;
                            tmp = dat_reader.ReadByte();
                            while (!(tmp == 8 && ptmp == 0))
                            {
                                ptmp = tmp;
                                sb.Append((char)tmp);
                                tmp = dat_reader.ReadByte();
                            }
                            id = sb.ToString().Trim();
                            dat_reader.ReadByte();// skip 1 byte
                            bdt_offset = dat_reader.ReadUInt32();
                            bdt_length = dat_reader.ReadUInt32();
                            bdt_reader.Seek(bdt_offset, SeekOrigin.Begin);
                            bytes = new byte[bdt_length];
                            bdt_reader.Read(bytes, 0, (int)bdt_length);
                            str = Encoding.UTF8.GetString(bytes).Trim();
                        }
                        catch (Exception)
                        {
                            continue;
                        }

                        note_doc = BookXmlDocument.CreateElement("NOTES");
                        title_id = BookXmlDocument.CreateAttribute("title");

                        if (str == "")
                            continue;

                        title_id.Value = id.Replace("'", "_");
                        note_doc.Attributes.Append(title_id);
                        if(!GlobalMemory.getInstance().parseBookBkgndCleaner && GlobalMemory.getInstance().ParseBook)
                            str = postProcessContent(str);
                        note_doc.InnerText = str.Trim();

                        rootdoc.AppendChild(note_doc);

                    }
                    idx_reader.Close();
                    dat_reader.Close();
                    bdt_reader.Close();
                }
            }

            if (System.IO.File.Exists((string)FileName))
            {
                DeleteAllSubFolders(TempDir);
            }

            SetProcessAsComplete();
		}
		

		
		private void DeleteAllSubFolders(string StartPath)
		{
			DirectoryInfo myfolder = new DirectoryInfo(StartPath);
			DirectoryInfo[] mySubfolders = myfolder.GetDirectories();
			FileInfo[] strFiles = myfolder.GetFiles();
			foreach (DirectoryInfo myItem in mySubfolders)
			{
				DeleteAllSubFolders(myItem.FullName);
			}
			foreach (FileInfo myItem in strFiles)
			{
				myItem.Delete();
			}
			myfolder.Delete();
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

        public override void ExportBook(string filename, int filter_idx)
        {
            string TempDir = System.IO.Path.GetTempPath() + "SimpleBibleReader_" + new Random(DateTime.Now.Millisecond).Next(100000000, 999999999).ToString() + "\\";

            if (!filename.EndsWith(".zip"))
            {
                filename = filename + ".zip";
            }
            if (filename.Equals(""))
            {
                return;
            }

            string bookcode = ABBREVIATION;

            Directory.CreateDirectory(TempDir + "mods.d");
            Directory.CreateDirectory(TempDir + "modules\\genbook\\rawgenbook\\" + bookcode + "\\");
            export_percent_complete = 0;

            StreamWriter writer = new StreamWriter(TempDir + "mods.d\\" + bookcode + ".conf");
            writer.WriteLine("[" + bookcode + "]");
            writer.WriteLine("DataPath=./modules/genbook/rawgenbook/" + bookcode + "/" + bookcode);
            writer.WriteLine("ModDrv=RawGenBook");
            writer.WriteLine("Version=1.0");
            writer.WriteLine("Encoding=UTF-8");
            writer.WriteLine("Description=" + DESCRIPTION);
            writer.WriteLine("DistributionLicense=Public Domain");
            writer.WriteLine("TextSource=" + COMMENTS);
            writer.WriteLine();
            writer.Close();

            //------------------
            string dat_path = TempDir + "modules\\genbook\\rawgenbook\\" + bookcode + "\\" + bookcode + ".dat";
            string bdt_path = TempDir + "modules\\genbook\\rawgenbook\\" + bookcode + "\\" + bookcode + ".bdt";
            string idx_path = TempDir + "modules\\genbook\\rawgenbook\\" + bookcode + "\\" + bookcode + ".idx";
            //------------------
            BinaryWriter dat = new BinaryWriter(File.Open(dat_path,FileMode.Create));
            BinaryWriter bdt = new BinaryWriter(File.Open(bdt_path, FileMode.Create));
            BinaryWriter idx = new BinaryWriter(File.Open(idx_path, FileMode.Create));

            XmlNodeList titles = BookXmlDocument.SelectNodes("/XMLBOOK/NOTES/@title");
            XmlNode note = null;
            byte[] _note_bytes = null;
            byte[] _topic_bytes = null;
            //populate default dat header..
            idx.Write(new byte[] { 0x0F, 0x00, 0x00, 0x00 });
            dat.Write(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
            //
            uint idx_offset = 8;
            for (int i = 0; i < titles.Count; i++)
            {
                percent_complete = i * 100 / titles.Count;

                note = BookXmlDocument.SelectSingleNode("/XMLBOOK/NOTES[@title='" + titles[i].Value + "']");
                if (note != null)
                {
                    _topic_bytes = Encoding.UTF8.GetBytes(titles[i].Value.Trim());

                    _note_bytes = Encoding.UTF8.GetBytes(note.InnerText.Trim());
                    
                    //
                    dat.Write(new byte[] { 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }); // default header?
                    
                    for(int j=0;j<_topic_bytes.Length-1;j++)
                        dat.Write(_topic_bytes[j]);
                    
                    dat.Write(new byte[] { 0x00,0x00,0x00});
                    idx.Write((UInt32)dat.BaseStream.Position);
                    dat.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 });
                    if(i==titles.Count-1)
                        dat.Write(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });
                    else
                        dat.Write((UInt32)idx_offset);
                    dat.Write(new byte[] { 0xFF,0xFF,0xFF,0xFF});
                    for (int j = 0; j < _topic_bytes.Length - 1; j++)
                        dat.Write(_topic_bytes[j]);
                    dat.Write(new byte[] { 0x00, 0x08, 0x00 });
                    dat.Write((UInt32)bdt.BaseStream.Position);

                    for (int j = 0; j < _note_bytes.Length; j++)
                        bdt.Write(_note_bytes[j]);
                    bdt.Write((byte)0x0A);
                    dat.Write((UInt32)(_note_bytes.Length+1));
                    idx_offset+=4;
                }
            }
             
            dat.Close();
            bdt.Close();
            idx.Close();

            /////==================
            FastZip fz = new FastZip();
            fz.CreateZip(filename, TempDir, true, "");
            DeleteAllSubFolders(TempDir);

            export_percent_complete = 100;
        }

    }	
}
