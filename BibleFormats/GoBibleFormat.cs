using System;
using System.Text;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using System.Xml;

namespace Simple_Bible_Reader
{
    class GoBibleFormat : BibleFormat
    {
        System.Xml.XmlElement rootdoc;
        System.Xml.XmlElement bookdoc;
        System.Xml.XmlElement chapdoc;
        System.Xml.XmlElement versedoc;
        System.Xml.XmlAttribute attrib;
        System.Xml.XmlAttribute attrib2;
        int _book_no=0;
        int total_books = 66;
        public System.Windows.Forms.RichTextBox rtb = new System.Windows.Forms.RichTextBox();

        public GoBibleFormat(string file)
            : base(file)
		{
		}

        public override void Load()
        {
            string global_idx_file = "";
            string TempDir = "";
            
            if (System.IO.File.Exists((string)FileName) && FileName.ToLower().EndsWith(".jar"))
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

            global_idx_file = TempDir + "Bible Data\\Index";
            
            BinaryReader global_idx_reader=new BinaryReader(System.IO.File.OpenRead(global_idx_file));
            BinaryReader book_idx_reader = null;
            BinaryReader chapter_reader = null;

            string book_name = "";
            string idx_book_file = "";
            int start_chapter = 0;
            int no_of_chapters = 0;
            int chapter_char_count = 0;
            int chapter_file_no = 0;
            int verse_count = 0;
            int verse_length = 0;
            string verse_content = null;
            int tmp = 0;
            int prev_chap_no = -1;
            int _bno = -1;
            BibleXmlDocument = new System.Xml.XmlDocument();
            XmlDeclaration dec = BibleXmlDocument.CreateXmlDeclaration("1.0", null, null);
            BibleXmlDocument.AppendChild(dec);
            rootdoc = BibleXmlDocument.CreateElement("XMLBIBLE");
            BibleXmlDocument.AppendChild(rootdoc);
            
            total_books=global_idx_reader.ReadByte();
            //Themes.DarkMessageBoxShow("");
            for (int book_no = 1; book_no <= total_books; book_no++)
            {
                if (global_idx_reader.PeekChar() == 0)
                    global_idx_reader.ReadByte();
                else
                {
                    // structure may not be valid
                    break;
                }
                

                book_name = global_idx_reader.ReadString();
                global_idx_reader.ReadByte();
                idx_book_file = global_idx_reader.ReadString();
                book_idx_reader = new BinaryReader(System.IO.File.OpenRead(TempDir + "Bible Data\\" + idx_book_file+"\\Index"));
                
                start_chapter = global_idx_reader.ReadByte();
                if(start_chapter==0)
                    start_chapter = global_idx_reader.ReadByte();
                
                no_of_chapters = global_idx_reader.ReadByte();           
                if(no_of_chapters==0)
                    no_of_chapters = global_idx_reader.ReadByte();

                bookdoc = BibleXmlDocument.CreateElement("BIBLEBOOK");
                attrib = BibleXmlDocument.CreateAttribute("bnumber");
                _bno = FormatUtil.getBookNo(idx_book_file);
                if(_bno!=-1)
                    attrib.Value = _bno.ToString(); //book_no.ToString();
                else
                    attrib.Value = book_no.ToString();
                attrib2 = BibleXmlDocument.CreateAttribute("bname");
                attrib2.Value = idx_book_file;
                bookdoc.Attributes.Append(attrib);
                bookdoc.Attributes.Append(attrib2);
                rootdoc.AppendChild(bookdoc);
                prev_chap_no = -1;

                for (int chap_no = start_chapter; chap_no < start_chapter + no_of_chapters; chap_no++)
                {
                    chapter_file_no = global_idx_reader.ReadByte();
                    global_idx_reader.ReadInt16();
                    tmp = global_idx_reader.ReadByte();
                    chapter_char_count = tmp * 256 + global_idx_reader.ReadByte();
                    //Themes.DarkMessageBoxShow(chapter_char_count.ToString());

                    verse_count = global_idx_reader.ReadByte();                    
                    chapdoc = BibleXmlDocument.CreateElement("CHAPTER");
                    attrib = BibleXmlDocument.CreateAttribute("cnumber");
                    attrib.Value = chap_no.ToString();
                    chapdoc.Attributes.Append(attrib);
                    bookdoc.AppendChild(chapdoc);

                    if (prev_chap_no != chapter_file_no)
                    {
                        if(chapter_reader!=null)
                            chapter_reader.Close();
                        chapter_reader = new BinaryReader(System.IO.File.OpenRead(TempDir + "Bible Data\\" + idx_book_file + "\\" + idx_book_file + " " + chapter_file_no.ToString()));
                        chapter_reader.ReadBytes(4);
                    }
                   

                    for (int verse_no = 1; verse_no <= verse_count; verse_no++)
                    {                        
                        tmp = book_idx_reader.ReadByte();
                        verse_length = tmp*256+book_idx_reader.ReadByte();
                        verse_content = new string(chapter_reader.ReadChars(verse_length));

                        versedoc = BibleXmlDocument.CreateElement("VERS");
                        attrib = BibleXmlDocument.CreateAttribute("vnumber");
                        attrib.Value = verse_no.ToString();
                        if (GlobalMemory.getInstance().ParseBible)
                        {
                            if (!GlobalMemory.getInstance().parseBibleBkgndCleaner)
                                verse_content = postProcessContent(verse_content);
                            versedoc.InnerText = verse_content;
                        }
                        else
                            versedoc.InnerXml = FormatUtil.EscapeXML(verse_content);
                        versedoc.Attributes.Append(attrib);
                        chapdoc.AppendChild(versedoc);
                    }
                    prev_chap_no=chapter_file_no;
                }
                book_idx_reader.Close();
            }
            chapter_reader.Close();
            global_idx_reader.Close();
            
            ///
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
                return ((_book_no) * 100) / total_books;
            }
        }

        public override int FilterIndex
        {
            get
            {
                return F15_GOBIBLE;
            }
        }

        public override void ExportBible(string filename,int filter_idx)
        {
            //string TempDir = "C:\\Users\\felix\\Desktop\\Go_Bible\\";
            string TempDir = System.IO.Path.GetTempPath() + "SimpleBibleReader_" + new Random(DateTime.Now.Millisecond).Next(100000000, 999999999).ToString() + "\\";
            
             if (!filename.EndsWith(".jar"))
            {
                filename = filename + ".jar";
            }
            if (filename.Equals(""))
            {
                return;
            }

            
            FastZip fz = new FastZip();
            fz.CreateEmptyDirectories = true;
            MemoryStream jar_stream = new MemoryStream(Properties.Resources.gobible);
            
            //
            fz.ExtractZip(jar_stream, TempDir,FastZip.Overwrite.Always,null,"","",true,true);
            Directory.CreateDirectory(TempDir + "META-INF");

            StreamWriter writer = new StreamWriter(TempDir + "META-INF\\MANIFEST.MF");
            writer.WriteLine("Manifest-Version: 1.0");
            writer.WriteLine("MIDlet-Data-Size: 100");
            writer.WriteLine("MicroEdition-Configuration: CLDC-1.0");
            writer.WriteLine("MIDlet-Name: "+ABBREVIATION);
            writer.WriteLine("MIDlet-Icon: Icon.png");
            if (GlobalMemory.getInstance().Direction == GlobalMemory.DIRECTION_LTR)
                writer.WriteLine("Go-Bible-Align: Left");
            else
                writer.WriteLine("Go-Bible-Align: Right");
            writer.WriteLine("MIDlet-Vendor: Jolon Faichney");
            writer.WriteLine("MIDlet-1: " + ABBREVIATION + " Go Bible, Icon.png, GoBible");
            writer.WriteLine("Go-Bible-Info: "+COMMENTS);
            writer.WriteLine("MIDlet-Version: 2.2.0");
            writer.WriteLine("MicroEdition-Profile: MIDP-2.0");
            writer.WriteLine();
            writer.WriteLine();
            writer.Close();  
            /////==================
            Directory.CreateDirectory(TempDir + "Bible Data");

            XmlNodeList books = BibleXmlDocument.SelectNodes("/XMLBIBLE/BIBLEBOOK");
            WriteGlobalIndex(TempDir + "Bible Data\\Index",books);

            foreach (XmlNode book in books)
            {
                _book_no=int.Parse(book.Attributes.GetNamedItem("bnumber").Value);
                Directory.CreateDirectory(TempDir + "Bible Data\\" + Localization.getBookNames()[_book_no - 1]);
                WriteBookIndex(TempDir + "Bible Data\\" + Localization.getBookNames()[_book_no - 1] + "\\Index", _book_no, book);
                WriteBookData(TempDir + "Bible Data\\" + Localization.getBookNames()[_book_no - 1] + "\\" + Localization.getBookNames()[_book_no - 1], _book_no, book);
            }

            /////==================
            fz = new FastZip();
            fz.CreateZip(filename, TempDir, true, "");            
            DeleteAllSubFolders(TempDir);
        }

        private void WriteGlobalIndex(string p,XmlNodeList books)
        {
            BinaryWriter writer = new BinaryWriter(System.IO.File.OpenWrite(p));
            writer.Write((byte)books.Count);            
            byte[] bname=null;
            int book_no = -1;
            XmlNodeList vers = null;
            XmlNodeList chaps = null;
            int chap_size = 0;
            foreach (XmlNode book in books)
            {
                book_no = int.Parse(book.Attributes.GetNamedItem("bnumber").Value);
                bname = getBytes(Localization.getBookNames()[book_no - 1]);
                writer.Write((byte)0);
                writer.Write((byte)bname.Length);// bookname length
                writer.Write(bname); // bookname
                writer.Write((byte)0);
                writer.Write((byte)bname.Length); // book filename length
                writer.Write(bname); // book filename
                writer.Write((byte)1);//start chapter
                chaps = book.SelectNodes("CHAPTER");
                writer.Write((byte)chaps.Count);//no of chapters

                for (int i = 1; i <= chaps.Count; i++)
                {
                    writer.Write((byte)(i/5));//file no that contains (trying to have 5 chapters in one file...)
                    writer.Write((byte)0);
                    writer.Write((byte)0);
                    vers = chaps[i-1].SelectNodes("VERS");
                    chap_size = 0;
                    foreach (XmlNode v in vers)
                        chap_size = chap_size + getBytes(v.InnerText).Length;

                    writer.Write((byte)(chap_size / 256));//no of chars in chapter.
                    writer.Write((byte)(chap_size % 256));//no of chars in chapter.
                    writer.Write((byte)FormatUtil.GetVerseCount(book_no, i));//no of verses in the chapter
                }
            }            
            writer.Close();
        }

        private void WriteBookIndex(string p,int book_no,XmlNode book)
        {
            BinaryWriter writer = new BinaryWriter(System.IO.File.OpenWrite(p));
            int len=-1;
            XmlNodeList vers = null;
            XmlNodeList chaps = book.SelectNodes("CHAPTER");
            string _verse_text = "";
            for (int i = 1; i <= chaps.Count; i++)
            {
                vers = chaps[i - 1].SelectNodes("VERS");
                foreach (XmlNode v in vers)
                {
                    //len = getBytes(v.InnerText).Length;
                    //len = v.InnerText.Length;

                    if (GlobalMemory.getInstance().ParseBible && CLEANING)
                    {
                        rtb.Rtf = rtf_header + v.InnerText;
                        _verse_text = rtb.Text;
                        len = _verse_text.Length;
                    }
                    else
                    {
                        len = v.InnerText.Length;
                    }

                    writer.Write((byte)(len / 256));
                    writer.Write((byte)(len % 256));
                }
            }
            writer.Close();
        }

        private void WriteBookData(string p, int book_no, XmlNode book)
        {
            int file_counter = 0;
            BinaryWriter writer = null;
            XmlNodeList vers = null;
            XmlNodeList chaps = book.SelectNodes("CHAPTER");
            string _verse_text = "";
            for (int i = 1; i <= chaps.Count; i++)
            {
                file_counter = i / 5;
                
                if(File.Exists(p + " " + file_counter.ToString()+".data"))
                    writer = new BinaryWriter(System.IO.File.Open(p + " " + file_counter.ToString() + ".data", FileMode.Append));
                else
                    writer = new BinaryWriter(System.IO.File.OpenWrite(p + " " + file_counter.ToString()+".data"));
                
                vers = chaps[i - 1].SelectNodes("VERS");
                foreach (XmlNode v in vers)
                {
                    _verse_text = v.InnerText;                    
                    writer.Write(getBytes(_verse_text));
                }
                writer.Close();
            }
            // insert bytes at beginning
            string data_filename="";
            byte[] data=null;
            for (int i = 1; i <= chaps.Count; i++)
            {
                file_counter = i / 5;
                data_filename=p + " " + file_counter.ToString() + ".data";
                if(!File.Exists(data_filename))
                    continue;
                data=File.ReadAllBytes(data_filename);
                
                writer = new BinaryWriter(System.IO.File.OpenWrite(p + " " + file_counter.ToString()));
                writer.Write((byte)0);
                writer.Write((byte)0);
                writer.Write((byte)(data.Length / 256));
                writer.Write((byte)(data.Length % 256));
                writer.Write(data);
                writer.Close();
                File.Delete(data_filename);
            }           
        }

        private byte[] getBytes(string p)
        {
            return Encoding.UTF8.GetBytes(p);
        }
        
    }
}
