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
using System.Net;

namespace Simple_Bible_Reader
{
		public class SwordDictionaryFormat : DictionaryFormat
		{
            System.Xml.XmlElement rootdoc;
            System.Xml.XmlElement item_doc;
            System.Xml.XmlElement desc_doc;
            System.Xml.XmlAttribute attrib_id;

            int export_percent_complete=0;
            int percent_complete = 0;

        public SwordDictionaryFormat(string file) : base(file)
		{
		}
		
		public override void Load()
		{
			string conf = "";
			string TempDir = "";
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
            string module_name = "";
            Dictionary<string, string> data = new Dictionary<string, string>();
            foreach (string row in File.ReadAllLines(conf))
            {
                try
                {
                    if (row.Trim().StartsWith("[") && row.Trim().EndsWith("]"))
                    {
                        module_name = row.Substring(1, row.Length - 2);
                        ABBREVIATION = module_name;
                        continue;
                    }
                    data.Add(row.Split("=".ToCharArray())[0].Trim(), row.Split("=".ToCharArray())[1].Trim());
                    if (row.Split("=".ToCharArray())[0].Trim().ToLower() == "description")
                        DESCRIPTION = row.Split("=".ToCharArray())[1].Trim();
                }
                catch (Exception)
                {
                }
            }

            if (GlobalMemory.getInstance().use_libsword)
            {
                //ABBREVIATION = FormatUtil.convertDefaultToUTFEncoding(lookup.getModule().Name);
                //DESCRIPTION = FormatUtil.convertDefaultToUTFEncoding(lookup.getModule().Description);
                try
                {
                    DictionaryXmlDocument = new System.Xml.XmlDocument();
                    XmlDeclaration dec = DictionaryXmlDocument.CreateXmlDeclaration("1.0", null, null);
                    DictionaryXmlDocument.AppendChild(dec);
                    rootdoc = DictionaryXmlDocument.CreateElement("dictionary");
                    DictionaryXmlDocument.AppendChild(rootdoc);

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
                            if (GlobalMemory.getInstance().ParseDictionary)
                                curr_text = FormatUtil.convertDefaultToUTFEncoding(module.StripText());
                            else
                                curr_text = FormatUtil.convertDefaultToUTFEncoding(module.RawEntry);

                            if (curr_text == prev_text && curr_key == prev_key)
                                break;
                            
                            item_doc = DictionaryXmlDocument.CreateElement("item");
                            attrib_id = DictionaryXmlDocument.CreateAttribute("id");

                            attrib_id.Value = module.KeyText;
                            item_doc.Attributes.Append(attrib_id);
                            desc_doc = DictionaryXmlDocument.CreateElement("description");
                            if (GlobalMemory.getInstance().ParseDictionary)
                            {
                                curr_text = postProcessContent(curr_text);
                                desc_doc.InnerText = FormatUtil.EscapeXML(curr_text);
                            }
                            else
                                desc_doc.InnerXml = FormatUtil.EscapeXML(curr_text);

                            item_doc.AppendChild(desc_doc);
                            rootdoc.AppendChild(item_doc);
                            prev_key = curr_key;
                            prev_text = curr_text;
                        }                        
                    }

                   
                }
                catch (Exception ex)
                {
                    //Themes.DarkMessageBoxShow(ex.Message);
                }
            }
            else
            {
                //ModDrv - RawText, zText
                //CompressType - LZSS or Zip


                DictionaryXmlDocument = new System.Xml.XmlDocument();
                XmlDeclaration dec = DictionaryXmlDocument.CreateXmlDeclaration("1.0", null, null);
                DictionaryXmlDocument.AppendChild(dec);
                rootdoc = DictionaryXmlDocument.CreateElement("dictionary");
                DictionaryXmlDocument.AppendChild(rootdoc);


                long loc = 0;
                long len = 0;
                long b_loc = 0;
                long b_len = 0;

                byte[] bytes = null;
                byte[] block = null;
                byte[] block_bytes = null;
                string str = null;
                string id = null;
                int idx = 0;
                long block_count = 0;
                FileStream dat_reader = null;
                BinaryReader idx_reader = null;
                FileStream zdt_reader = null;
                BinaryReader zdx_reader = null;

                string dat_path = null;
                string idx_path = null;


                /////////////// ENCRYPION AND NO ENCRYPTION ////////////////////

                //////////// NO ENCRYPTION
                dat_path = TempDir + data["DataPath"] + ".dat";
                idx_path = TempDir + data["DataPath"] + ".idx";

                dat_reader = File.Open(dat_path, FileMode.Open);
                idx_reader = new BinaryReader(File.Open(idx_path, FileMode.Open), Encoding.ASCII);

                int total_percent = 100;
                if (data["ModDrv"].ToLower() == "zld")
                    total_percent = 50;

                while (idx_reader.PeekChar() != -1)
                {
                    try
                    {
                        loc = idx_reader.ReadUInt32();
                        if (data["ModDrv"].ToLower() == "zld")
                            len = idx_reader.ReadUInt32();
                        else
                            len = idx_reader.ReadUInt16();
                    }
                    catch (Exception)
                    {
                        //unable to read further...
                        break;
                    }


                    percent_complete = (int)((loc * total_percent) / dat_reader.Length);

                    dat_reader.Seek(loc, SeekOrigin.Begin);
                    bytes = new byte[len];
                    dat_reader.Read(bytes, 0, (int)len);
                    str = Encoding.UTF8.GetString(bytes).Trim();
                    item_doc = DictionaryXmlDocument.CreateElement("item");
                    attrib_id = DictionaryXmlDocument.CreateAttribute("id");


                    idx = str.IndexOf("\\");

                    if (idx == -1)
                        idx = str.IndexOf("\n");

                    if (idx != -1)
                    {
                        id = str.Substring(0, idx);
                        if (id.Length > 50)
                            id = id.Substring(0, 47) + "..";
                        attrib_id.Value = id;
                        item_doc.Attributes.Append(attrib_id);
                        desc_doc = DictionaryXmlDocument.CreateElement("description");
                        desc_doc.InnerText = str.Substring(idx + 1).Trim();
                    }
                    else
                    {
                        // must not come here for zld
                        attrib_id.Value = "Preface";
                        item_doc.Attributes.Append(attrib_id);
                        desc_doc = DictionaryXmlDocument.CreateElement("description");
                        str = postProcessContent(str);
                        desc_doc.InnerText = str;
                    }

                    item_doc.AppendChild(desc_doc);
                    rootdoc.AppendChild(item_doc);

                }
                idx_reader.Close();
                dat_reader.Close();

                MemoryStream ms = null;
                BinaryReader br = null;

                if (data["ModDrv"].ToLower() == "zld")
                {
                    dat_path = TempDir + data["DataPath"] + ".zdt";
                    idx_path = TempDir + data["DataPath"] + ".zdx";
                    //////////// ENCRYPTION
                    zdt_reader = File.Open(dat_path, FileMode.Open);
                    zdx_reader = new BinaryReader(File.Open(idx_path, FileMode.Open), Encoding.ASCII);
                    XmlNodeList nodes = (XmlNodeList)DictionaryXmlDocument.SelectNodes("/dictionary/item/description");
                    int count = 0;
                    while (zdx_reader.PeekChar() != -1)
                    {
                        try
                        {
                            loc = zdx_reader.ReadUInt32();
                            len = zdx_reader.ReadUInt32();
                        }
                        catch (Exception)
                        {
                            //unable to read further...
                            break;
                        }

                        percent_complete = 50 + (int)((loc * 50) / zdt_reader.Length);

                        zdt_reader.Seek(loc, SeekOrigin.Begin);
                        bytes = new byte[len];
                        zdt_reader.Read(bytes, 0, (int)len);
                        block = ZlibStream.UncompressBuffer(bytes);
                        /////////////////
                        ms = new MemoryStream(block);
                        br = new BinaryReader(new MemoryStream(block), Encoding.ASCII);

                        block_count = br.ReadUInt32();
                        for (int i = 0; i < block_count; i++)
                        {
                            b_loc = br.ReadUInt32();
                            b_len = br.ReadUInt32();
                            block_bytes = new byte[b_len];
                            ms.Seek(b_loc, SeekOrigin.Begin);
                            ms.Read(block_bytes, 0, (int)b_len);
                            str = Encoding.UTF8.GetString(block_bytes).Trim();
                            if (GlobalMemory.getInstance().ParseDictionary)
                            {
                                str = postProcessContent(str);
                                nodes[count].InnerText = str;
                            }
                            else
                                nodes[count].InnerXml = FormatUtil.EscapeXML(str);
                            count++;
                        }
                        ms.Close();
                        br.Close();
                        ///////////////////////////////////////////
                    }
                    zdx_reader.Close();
                    zdt_reader.Close();
                }
            }
            

			if (System.IO.File.Exists((string) FileName))
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

        public override void ExportDictionary(string filename, int filter_idx)
        {
            //string TempDir = "C:\\Users\\felix\\Desktop\\Sword_Bible\\";
            string TempDir = System.IO.Path.GetTempPath() + "SimpleBibleReader_" + new Random(DateTime.Now.Millisecond).Next(100000000, 999999999).ToString() + "\\";

            if (!filename.EndsWith(".zip"))
            {
                filename = filename + ".zip";
            }
            if (filename.Equals(""))
            {
                return;
            }

            string biblecode = ABBREVIATION;

            Directory.CreateDirectory(TempDir + "mods.d");
            Directory.CreateDirectory(TempDir + "modules\\lexdict\\rawld\\" + biblecode + "\\");
            export_percent_complete = 0;

            StreamWriter writer = new StreamWriter(TempDir + "mods.d\\" + biblecode + ".conf");
            writer.WriteLine("[" + biblecode + "]");
            writer.WriteLine("DataPath=./modules/lexdict/rawld/" + biblecode + "/" + biblecode);
            writer.WriteLine("ModDrv=RawLD");
            writer.WriteLine("Version=1.0");
            writer.WriteLine("Encoding=UTF-8");
            writer.WriteLine("Description=" + DESCRIPTION);
            writer.WriteLine("DistributionLicense=Public Domain");
            writer.WriteLine("TextSource=" + COMMENTS);
            writer.WriteLine();
            writer.Close();

            //------------------
            string dat_path = TempDir + "modules\\lexdict\\rawld\\" + biblecode + "\\" + biblecode + ".dat";
            string idx_path = TempDir + "modules\\lexdict\\rawld\\" + biblecode + "\\" + biblecode + ".idx";
            //------------------
            BinaryWriter dat = new BinaryWriter(File.Open(dat_path,FileMode.Create));
            BinaryWriter idx = new BinaryWriter(File.Open(idx_path, FileMode.Create));

            byte[] bytes = null;
            byte[] idb = null;
            //int location = 0;

            XmlNodeList items = DictionaryXmlDocument.SelectNodes("/dictionary/item");
            string item = null;
            string id = null;
            //
            bytes = Encoding.UTF8.GetBytes(DESCRIPTION+"\n\n");
            idx.Write((UInt32)dat.BaseStream.Length);
            dat.Write(bytes);
            idx.Write((UInt16)bytes.Length);
            //
            int count = 1;
            for (int i = 0; i < items.Count; i++)
            {
                export_percent_complete = (i*100)/items.Count;
                id = items[i].Attributes["id"].Value;
                item = items[i].InnerText;
                //        
                if (item.Trim() == DESCRIPTION.Trim())
                    continue;
                
                bytes = Encoding.UTF8.GetBytes("\\" + id.Trim() + "\\\n" + item.Trim() + "\n\n");
                //idb = Encoding.UTF8.GetBytes("$$T" + count.ToString("0000000") + "\n");
                //dat.Write(idb);
                idx.Write((UInt32)dat.BaseStream.Length + 1);

                dat.Write(bytes);
                idx.Write((UInt16)(bytes.Length-1));
                count++;
            }
             
            dat.Close();
            idx.Close();

            /////==================
            FastZip fz = new FastZip();
            fz.CreateZip(filename, TempDir, true, "");
            DeleteAllSubFolders(TempDir);

            export_percent_complete = 100;
        }

    }	
}
