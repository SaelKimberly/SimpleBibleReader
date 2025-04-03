using System.Collections.Generic;

using System;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using Sword;
using ICSharpCode.SharpZipLib.Zip;
using System.Text;
using Sword.reader;
using System.Runtime.CompilerServices;
using PalmBiblePlus;
using System.Drawing;
using System.Reflection;
using System.Linq;
using RtfPipe.Tokens;

namespace Simple_Bible_Reader
	{

    public class SwordBibleFormat : BibleFormat
    {
        BibleZtextReader zReader;
        BibleTextReader tReader;
        int export_percent_complete = 0;
        SwordLookup lookup = null;

        const int OLD_TESTAMENT = 1;
        const int NEW_TESTAMENT = 2;

        
        String m_DistributionLicense = "Public Domain";
        String m_TextSource = "Exported using Simple Bible Reader (" + GlobalMemory.AUTHOR_WEBSITE + ")";


        public SwordBibleFormat(string file) : base(file)
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
                BibleXmlDocument.LoadXml(lookup.GetZefaniaXML(versification,GlobalMemory.getInstance().ParseBible,true));
                lookup.DisposeManager();
            }
            else
            {
                Dictionary<string, string> data = new Dictionary<string, string>();
                foreach (string row in File.ReadAllLines(conf))
                {
                    try
                    {
                        data.Add(row.Split("=".ToCharArray())[0], row.Split("=".ToCharArray())[1]);
                    }
                    catch (Exception)
                    {
                    }
                }
                //ModDrv - RawText, zText
                //CompressType - LZSS or Zip
                if (data["ModDrv"].ToLower() == "ztext")
                {
                    tReader = null;
                    zReader = new BibleZtextReader(TempDir + data["DataPath"], data["Lang"], false);
                    BibleXmlDocument.LoadXml((string)(zReader.GetZefaniaXML(FormatUtil.CONTENT_TYPE.BIBLE)));
                }
                else
                {
                    zReader = null;
                    tReader = new BibleTextReader(TempDir + data["DataPath"], Path.GetFileNameWithoutExtension(conf));
                    BibleXmlDocument.LoadXml((string)(tReader.GetZefaniaXML(FormatUtil.CONTENT_TYPE.BIBLE)));
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
                if (GlobalMemory.getInstance().use_libsword)
                {
                    if (lookup != null)
                        return lookup.GetPercentComplete();
                    else
                        return 0;
                }
                else
                {
                    if (zReader != null)
                        return zReader.GetPercentComplete();
                    else if (tReader != null)
                        return tReader.GetPercentComplete();
                    else
                        return 0;
                }
            }
        }

        public override string AdditionaStatusText()
        {
            if (lookup != null)
                return lookup.statusText;
            else
                return "";
        }

        public override int FilterIndex
        {
            get
            {
                return F12_SWORDPROJECT;
            }
        }


        public override List<string[]> getProperties()
        {
            if (kv_export.Count == 0)
            {
                kv_export.Add(new string[] { "Abbreviation:", BibleFormat.getInstance().ABBREVIATION });
                kv_export.Add(new string[] { "Description:", BibleFormat.getInstance().DESCRIPTION });
                kv_export.Add(new string[] { "Comments:", BibleFormat.getInstance().COMMENTS });
                kv_export.Add(new string[] { "Distribution License:", m_DistributionLicense });
                kv_export.Add(new string[] { "Text Source:", m_TextSource });
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
                    case "Version:":
                        m_DistributionLicense = kv_export[i][1];
                        break;
                    case "Font:":
                        m_TextSource = kv_export[i][1];
                        break;
                    
                }
            }
        }


        public override void ExportBible(string filename, int filter_idx)
        {
            if(Themes.MessageBox("Module name/Abbreviation for this bible is '"+ ABBREVIATION+"'. You can change this from 'Edit Bible Metadata'. Do you want to proceed?","Confirmation",MessageBoxButtons.YesNo,MessageBoxIcon.Question)==DialogResult.No)
                return;

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

            processProperties();
            string moduleName = ABBREVIATION;
            string versification = GlobalMemory.getInstance().Versification;

            Directory.CreateDirectory(TempDir + "mods.d");
            Directory.CreateDirectory(TempDir + "modules\\texts\\rawtext\\" + moduleName + "\\");
            StreamWriter writer = new StreamWriter(TempDir + "mods.d\\" + moduleName + ".conf");
            writer.WriteLine("[" + moduleName + "]");
            writer.WriteLine("DataPath=./modules/texts/rawtext/" + moduleName + "/");
            writer.WriteLine("ModDrv=RawText");
            writer.WriteLine("Versification="+ versification.ToUpper());            
            writer.WriteLine("BlockType=BOOK");
            writer.WriteLine("Version=1.0");
            writer.WriteLine("Encoding=UTF-8");
            writer.WriteLine("Description=" + DESCRIPTION);
            writer.WriteLine("DistributionLicense="+m_DistributionLicense);
            writer.WriteLine("TextSource="+m_TextSource);
            writer.WriteLine();
            writer.Close();

            int[][][] offsets = SwordVersification.getOffsets(versification);
            generateSwordModules(OLD_TESTAMENT, versification, TempDir, moduleName, offsets);
            generateSwordModules(NEW_TESTAMENT, versification, TempDir, moduleName, offsets);

            /////==================
            if (File.Exists(filename))
                File.Delete(filename);
            FastZip fz = new FastZip();           
            fz.CreateZip(filename, TempDir, true, "");
            DeleteAllSubFolders(TempDir);
            export_percent_complete = 100;
        }

        private void generateSwordModules(int testament, string versification, string modpath, string biblecode, int[][][] offsets)
        {
            string swordBookAbbr = null;

            FileStream fs_vss = null;
            FileStream fs_raw = null;

            int book_no = -1;
            int chap_no = -1;
            int verse_no = -1;
            byte[] start = new byte[4];
            byte[] size = new byte[2];
            long tmp = 0;
            string versetxt = null;
            byte[] verse_bytes = null;

           


            int offset = 0;
            int index;

            string testament_str = (testament == OLD_TESTAMENT) ? "ot" : "nt";
            string vss_path = modpath + "modules\\texts\\rawtext\\" + biblecode + "\\"+ testament_str + ".vss";
            string raw_path = modpath + "modules\\texts\\rawtext\\" + biblecode + "\\"+ testament_str;


            string[] books = SwordVersification.getBooks(versification, testament_str);
            for (int swordBookIndex = 0; swordBookIndex < books.Length; swordBookIndex++)
            {
                swordBookAbbr = books[swordBookIndex];
                book_no = FormatUtil.swordBookAbbrToZefaniaBookNo(swordBookAbbr);
                XmlNode book = BibleXmlDocument.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber='" + book_no + "']");

                if (book == null)
                    continue;

                if(testament == OLD_TESTAMENT) // Apocrapha inc in OT
                    export_percent_complete = (swordBookIndex * 50) / books.Length;
                else
                    export_percent_complete = 50 + ((swordBookIndex * 50) / books.Length);

                if (!File.Exists(vss_path))
                {
                    if (fs_vss != null)
                        fs_vss.Close();
                    fs_vss = File.OpenWrite(vss_path);
                }
                if (!File.Exists(raw_path))
                {
                    if (fs_raw != null)
                        fs_raw.Close();
                    fs_raw = File.OpenWrite(raw_path);
                }

                foreach (XmlNode chap in book.SelectNodes("CHAPTER"))
                {
                    chap_no = int.Parse(chap.Attributes.GetNamedItem("cnumber").Value);
                    foreach (XmlNode vers in chap.SelectNodes("VERS"))
                    {
                        verse_no = int.Parse(vers.Attributes.GetNamedItem("vnumber").Value);
                        try
                        {
                            offset = offsets[testament - 1][0][swordBookIndex + 1];
                            offset = offsets[testament - 1][1][offset + chap_no];
                        }
                        catch (Exception ex)
                        {
                            /*
                            Themes.DarkMessageBoxShow(ex.Message + ", swordBookIndex=" + swordBookIndex + ", chap_no=" + chap_no + ", testament=" + testament + ", book_no=" + book_no + ", swordBookName=" + swordBookAbbr
                                + "\noffsets[testament - 1][0].Len()=" + offsets[testament - 1][0].Length + ", swordBookIndex+1=" + (swordBookIndex + 1).ToString()
                                + "\noffsets[testament - 1][1].Len()=" + offsets[testament - 1][1].Length + ", offset+chap_no = " + (offset + chap_no).ToString());
                        */
                        }

                        index = (offset + verse_no) * 6;
                        if (fs_vss == null)
                        {
                            //Themes.DarkMessageBoxShow("fs_vss is null, Book: "+swordBookAbbr);
                            continue;
                        }
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
        }

        protected byte[] getBytesFromInt(long _int)
        {
            byte[] buf = new byte[4];
            buf[0] = (byte)(_int % 256);
            buf[1] = (byte)((_int / 256) % 256);
            buf[2] = (byte)(((_int / 256) / 256) % 256);
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
