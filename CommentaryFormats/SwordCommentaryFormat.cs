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
namespace Simple_Bible_Reader
{
		public class SwordCommentaryFormat : CommentaryFormat
		{
            BibleZtextReader zReader;
            BibleTextReader tReader;
        int export_percent_complete = 0;
        SwordLookup lookup = null;

        const int OLD_TESTAMENT = 1;
        const int NEW_TESTAMENT = 2;

        public SwordCommentaryFormat(string file) : base(file)
		{
		}

        public override void Load()
        {
            string conf = "";
            string TempDir = "";
            if (System.IO.File.Exists((string)FileName) && FileName.ToLower().EndsWith(".zip"))
            {
                TempDir = System.IO.Path.GetTempPath() + "SimpleBibleReader_" + new Random(DateTime.Now.Millisecond).Next(100000000, 999999999).ToString() + "\\";
                FastZip fz = new FastZip();
                fz.CreateEmptyDirectories = true;
                fz.ExtractZip(FileName, TempDir, "");
            }
            else if (System.IO.Directory.Exists((string)FileName))
            {
                TempDir = (string)(FileName + "\\");
            }
            else
            {
                return;
            }

            string versification = null;
            conf = (string)(Directory.GetFiles(TempDir + "mods.d\\", "*.conf")[0]);
            string module_name = "";
            foreach (string row in File.ReadAllLines(conf))
            {
                try
                {
                    if (row.Trim().StartsWith("[") && row.Trim().EndsWith("]"))
                    {
                        module_name = row.Substring(1, row.Length - 2);
                        break;
                    }
                }
                catch (Exception)
                {
                }
            }

            if (GlobalMemory.getInstance().use_libsword)
            {
                lookup = new SwordLookup(TempDir, module_name);
                versification = lookup.getModule().GetConfigEntry("Versification");
                if (versification == null)
                    versification = "kjva";
                else
                    versification = versification.ToLower();

                ABBREVIATION = FormatUtil.convertDefaultToUTFEncoding(lookup.getModule().Name);
                DESCRIPTION = FormatUtil.convertDefaultToUTFEncoding(lookup.getModule().Description);
                CommentaryXmlDocument.LoadXml(lookup.GetZefaniaXML(versification, GlobalMemory.getInstance().ParseCommentary,false));
                lookup.DisposeManager();
            }
            else
            {
                Dictionary<string, string> data = new Dictionary<string, string>();
                foreach (string row in File.ReadAllLines(conf))
                {
                    if (row.StartsWith("[") && row.EndsWith("]"))
                    {
                        ABBREVIATION = row.Substring(1, row.Length - 2);
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

                if (data["ModDrv"].ToLower() == "zcom")
                {
                    tReader = null;
                    zReader = new BibleZtextReader(TempDir + data["DataPath"], data["Lang"], false);
                    CommentaryXmlDocument.LoadXml((string)(zReader.GetZefaniaXML(FormatUtil.CONTENT_TYPE.COMMENTARY)));
                }
                else
                {
                    zReader = null;
                    tReader = new BibleTextReader(TempDir + data["DataPath"], Path.GetFileNameWithoutExtension(conf));
                    CommentaryXmlDocument.LoadXml((string)(tReader.GetZefaniaXML(FormatUtil.CONTENT_TYPE.COMMENTARY)));
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
                if(zReader != null)
                    return zReader.GetPercentComplete();
                else if(tReader != null)
                    return tReader.GetPercentComplete();
				else
				    return 0;				
			}
		}

        public override int FilterIndex
        {
            get
            {
                return 2;
            }
        }

        public override void ExportCommentary(string filename,int filter_idx)
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

            Directory.CreateDirectory(TempDir+"mods.d");
            Directory.CreateDirectory(TempDir + "modules\\comments\\rawcom\\" + biblecode + "\\");            


            StreamWriter writer = new StreamWriter(TempDir + "mods.d\\" + biblecode + ".conf");
            writer.WriteLine("[" + biblecode + "]");
            writer.WriteLine("DataPath=./modules/comments/rawcom/" + biblecode + "/");
            writer.WriteLine("ModDrv=RawCom");
            //writer.WriteLine("BlockType=Book");
            writer.WriteLine("Version=1.0");
            writer.WriteLine("Encoding=UTF-8"); 
            writer.WriteLine("Description="+DESCRIPTION);
            writer.WriteLine("DistributionLicense=Public Domain");
            writer.WriteLine("TextSource="+COMMENTS);
            writer.WriteLine();
            writer.Close();
            
            //------------------
            string vss_ot_path = TempDir + "modules\\comments\\rawcom\\" + biblecode + "\\ot.vss";
            string vss_nt_path = TempDir + "modules\\comments\\rawcom\\" + biblecode + "\\nt.vss";
            string raw_ot_path = TempDir + "modules\\comments\\rawcom\\" + biblecode + "\\ot";
            string raw_nt_path = TempDir + "modules\\comments\\rawcom\\" + biblecode + "\\nt";
            //------------------

            XmlNodeList books = CommentaryXmlDocument.SelectNodes("/XMLBIBLE/BIBLEBOOK");
            export_percent_complete = 0;
            int book_no=-1;
            int chap_no = -1;
            int verse_no = -1;
            int testament = -1;
            int[][][] offsets = new BibleTextReader().getOffsets();
            int offset = 0;
            int index;

            byte[] start=new byte[4];
            byte[] size = new byte[2];
            
            FileStream fs_vss = null;
            FileStream fs_raw = null;
            string versetxt=null;
            byte[] verse_bytes = null;
            long tmp = 0;

            foreach (XmlNode book in books)
            {
                book_no = int.Parse(book.Attributes.GetNamedItem("bnumber").Value);
                export_percent_complete=book_no * 100 / 66;

                if (book_no < 40)
                {
                    testament = 1;
                    if (!File.Exists(vss_ot_path))
                    {
                        if (fs_vss != null)
                            fs_vss.Close();
                        fs_vss = File.OpenWrite(vss_ot_path);
                    }
                    if (!File.Exists(raw_ot_path))
                    {
                        if (fs_raw != null)
                            fs_raw.Close();
                        fs_raw = File.OpenWrite(raw_ot_path);
                    }
                }
                else if (book_no >= 40 && book_no <= 66)
                {
                    testament = 2;
                    if (!File.Exists(vss_nt_path))
                    {
                        if (fs_vss != null)
                            fs_vss.Close();
                        fs_vss = File.OpenWrite(vss_nt_path);
                    }
                    if (!File.Exists(raw_nt_path))
                    {
                        if (fs_raw != null)
                            fs_raw.Close();
                        fs_raw = File.OpenWrite(raw_nt_path);
                    }
                }
                else
                    continue;

                foreach (XmlNode chap in book.SelectNodes("CHAPTER"))
                {
                    chap_no = int.Parse(chap.Attributes.GetNamedItem("cnumber").Value);
                    foreach (XmlNode vers in chap.SelectNodes("VERS"))
                    {
                        verse_no = int.Parse(vers.Attributes.GetNamedItem("vnumber").Value);
                        if (testament == 1)
                            offset = offsets[testament - 1][0][book_no];
                        else if (testament == 2)
                            offset = offsets[testament - 1][0][book_no - 39];
                        offset = offsets[testament - 1][1][offset + chap_no];
                        index = (offset + verse_no) * 6;

                        if (index != fs_vss.Position)
                        {
                            if (index > fs_vss.Position)
                            {
                                tmp = index - fs_vss.Position;
                                fs_vss.Write(new byte[tmp], 0, (int)tmp);
                            }
                            else
                            {
                                fs_vss.Seek(index, SeekOrigin.Begin);
                            }
                        }

                        versetxt = vers.InnerText.Trim();

                        verse_bytes = Encoding.UTF8.GetBytes(versetxt);

                        start = getBytesFromInt(fs_raw.Position);
                        size = getBytesFromShortInt(verse_bytes.Length);

                        fs_vss.Write(start, 0, start.Length);
                        fs_vss.Write(size, 0, size.Length);
                        
                        fs_raw.Write(verse_bytes, 0, verse_bytes.Length);
                       
                    }
                }
            }
            if (fs_raw != null)
                fs_raw.Close();
            if (fs_vss != null)
                fs_vss.Close();

            /////==================
            FastZip fz = new FastZip();
            fz.CreateZip(filename, TempDir, true, "");
            DeleteAllSubFolders(TempDir);

            export_percent_complete = 100;
        }

        protected byte[] getBytesFromInt(long _int)
        {
            byte[] buf = new byte[4];            
            buf[0]= (byte)(_int % 256);
            buf[1]=(byte)((_int / 256) % 256);
            buf[2] = (byte)(((_int / 256)/256) % 256);
            buf[3] = (byte)(((_int / 256) / 256) / 256);
            return buf;
        }

        protected byte[] getBytesFromShortInt(int _int)
        {
            byte[] buf = new byte[2];
            buf[0] = (byte)(_int % 256);
            buf[1] = (byte)((_int / 256) % 256);
            return buf;
        }
    }	
}
