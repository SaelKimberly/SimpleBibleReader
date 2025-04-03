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
using Ionic.Zlib;
using System.Text.RegularExpressions;
	
/*
 * Encoding.UTF8 is required because char.peek doesn't seem to work without it. I believe it means for sbr, support for unicode is not yet and tbc.
 */
	
	namespace Simple_Bible_Reader
	{
		public class STEPBookFormat : BookFormat
		{
            System.Xml.XmlElement rootdoc;
            System.Xml.XmlElement bookdoc;
            System.Xml.XmlAttribute attrib;

            string _title_text;
            string _notes_text;

            System.Windows.Forms.RichTextBox rtb = new System.Windows.Forms.RichTextBox();
            int percent_complete = 0;
		
		public STEPBookFormat(string file) : base(file)
		{
		}

        public override void Load()
        {
            BookXmlDocument = new System.Xml.XmlDocument();
            XmlDeclaration dec = BookXmlDocument.CreateXmlDeclaration("1.0", null, null);
            BookXmlDocument.AppendChild(dec);
            rootdoc = BookXmlDocument.CreateElement("XMLBOOK");
            BookXmlDocument.AppendChild(rootdoc);

            string path = Path.GetDirectoryName( this.FileName);
            percent_complete = 0;

            uint book_ptr;
            uint uncompressed_size;
            uint compressed_size;

            BinaryReader view_reader = new BinaryReader(File.OpenRead(path + "\\VIEWABLE.IDX"), Encoding.UTF8);
            view_reader.ReadBytes(0x10);
            view_reader.ReadBytes(10);
            byte compressed = view_reader.ReadByte();
            view_reader.ReadBytes(5);
            long view_offset = view_reader.BaseStream.Position;

            BinaryReader book_reader = new BinaryReader(File.OpenRead(path + "\\BOOK.DAT"), Encoding.UTF8);
             
            book_reader.ReadBytes(0x10);
            uint len2=book_reader.ReadUInt32();
            byte[] headers = new byte[len2];
            headers=book_reader.ReadBytes((int)len2);
            string header_str = Encoding.UTF8.GetString(headers);
            int idx = header_str.IndexOf("{\\Title:")+"{\\Title:".Length;
            string title = header_str.Substring(idx, header_str.IndexOf("}") - idx);
            this.ABBREVIATION = title;
            this.DESCRIPTION = title;
            this.COMMENTS = title;
            
            // sections..
            BinaryReader reader = new BinaryReader(File.OpenRead(path + "\\SECTIONS.IDX"), Encoding.UTF8);
            reader.ReadBytes(0x10);//ver rec
            reader.ReadBytes(0x10);
            uint next;
            uint level = 0;
            byte[] namebytes;
            byte[] bytes_notes;
            byte[] bytes_notes1;

            while (true)
            {
                //if (reader.PeekChar() < 0 || book_reader.PeekChar() < 0 || view_reader.PeekChar() < 0)
                if(reader.BaseStream.Position==reader.BaseStream.Length)
                    break;
                reader.ReadBytes(8);
                next = reader.ReadUInt32();
                uint viewableIdx = reader.ReadUInt32();
                reader.ReadBytes(2);
                level = reader.ReadByte();
                uint name = reader.ReadUInt32();
                if (name > reader.BaseStream.Length)
                    break;
                // get the section name
                long old = reader.BaseStream.Position;
                reader.BaseStream.Seek(name, SeekOrigin.Begin);
                uint namelen = reader.ReadUInt16();
                namebytes = new byte[namelen];
                namebytes = reader.ReadBytes((int)namelen);
                reader.BaseStream.Seek(old, SeekOrigin.Begin);

                reader.ReadBytes(6);

                percent_complete = (int)(((reader.BaseStream.Position) * 100) / reader.BaseStream.Length);
                _title_text = System.Security.SecurityElement.Escape(Encoding.UTF8.GetString(ensureUtf8Bom(namebytes)));
                // get book pointer from viewable idx
                view_reader.BaseStream.Seek(view_offset+viewableIdx * 12, SeekOrigin.Begin);
                book_ptr = view_reader.ReadUInt32();
                uncompressed_size = view_reader.ReadUInt32();
                compressed_size = view_reader.ReadUInt32();

                book_reader.BaseStream.Seek(book_ptr,SeekOrigin.Begin);
                bytes_notes = new byte[uncompressed_size];
                String rtf;
                if (compressed == 0)
                {                    
                    bytes_notes = book_reader.ReadBytes((int)uncompressed_size);
                    _notes_text = Encoding.UTF8.GetString(ensureUtf8Bom(bytes_notes));
                }
                else
                {
                    bytes_notes1 = new byte[compressed_size];
                    bytes_notes1 = book_reader.ReadBytes((int)compressed_size);
                    LZSS lz = new LZSS();
                    lz.decompress(bytes_notes, bytes_notes1, uncompressed_size);
                    rtf=Encoding.UTF8.GetString(bytes_notes).Trim();
                    if(rtf.StartsWith(@"{\rtf1"))
                        rtb.Rtf = rtf;
                    else
                        rtb.Rtf = rtf_header+ rtf;
                    _notes_text = rtb.Text;
                }

                for (int i = 1; i < level; i++)
                    _title_text = "...." + _title_text;


                /*
                if (level == 1)
                    processLine();
                else
                {
                    XmlElement bookdoc2 = BookXmlDocument.CreateElement("NOTES");
                    XmlAttribute attrib2 = BookXmlDocument.CreateAttribute("title");
                    attrib2.Value = _title_text;
                    bookdoc2.Attributes.Append(attrib2);
                    bookdoc2.InnerText = _notes_text;
                    bookdoc.AppendChild(bookdoc2);                    
                }
                */


                XmlElement bookdoc2 = BookXmlDocument.CreateElement("NOTES");
                XmlAttribute attrib2 = BookXmlDocument.CreateAttribute("title");
                attrib2.Value = _title_text;
                bookdoc2.Attributes.Append(attrib2);

                if (GlobalMemory.getInstance().ParseBook)
                {
                    if (!GlobalMemory.getInstance().parseBookBkgndCleaner)
                        _notes_text = postProcessContent(_notes_text);
                    bookdoc2.InnerText = _notes_text;
                }
                else
                    bookdoc2.InnerXml = FormatUtil.EscapeXML(_notes_text);

                rootdoc.AppendChild(bookdoc2);

                //reader.BaseStream.Seek(next, SeekOrigin.Begin);
            }
            reader.Close();
            book_reader.Close();
            view_reader.Close();
            ////////////////////////////////////////
            //clean up empty nodes
            foreach (XmlNode node in rootdoc.SelectNodes("//NOTES"))
            {
                if (node.InnerText.Trim() == "")
                    rootdoc.RemoveChild(node);
            }
            SetProcessAsComplete();           
        }

            /*
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
            bookdoc.InnerText = _notes_text;
        }
             */


        private byte[] ensureUtf8Bom(byte[] bytes_notes)
        {
            if (bytes_notes[0] == 0xEF && bytes_notes[1] == 0xBB && bytes_notes[2] == 0xBF)
                return bytes_notes;
            else
            {
                byte[] bytes2 = new byte[bytes_notes.Length + 3];
                bytes2[0] = 0xEF;
                bytes2[1] = 0xBB;
                bytes2[2] = 0xBF;
                for (int i = 0; i < bytes_notes.Length; i++)
                    bytes2[i + 3] = bytes_notes[i];
                return bytes2;
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
                return 9;
            }
        }

        public override void ExportBook(string filename, int filter_idx)
        {
            Themes.MessageBox("Export of STEP Book format is not supported.", "Export Book", MessageBoxButtons.OK, MessageBoxIcon.Error);
            /*
            string path = this.FileName.Substring(0,this.FileName.LastIndexOf('.'));
            Directory.CreateDirectory(path);

            /////// quick parsing ////
            XmlNodeList notes = BookXmlDocument.SelectNodes("/XMLBOOK/NOTES");
            XmlNode note = null;
            string _note_text = "";
            Hashtable ht = new Hashtable();
            int id = 0;
            ArrayList listKeys = new ArrayList();//for order
            string title;
            for (int i = 0; i < notes.Count; i++)
            {
                percent_complete = i * 50 / notes.Count;

                note = notes[i];
                title = note.Attributes["title"].Value;
                if (note != null)
                {
                    _note_text = note.InnerText;
                    //

                    if (!listKeys.Contains(title))
                    {
                        listKeys.Add(title);
                        ht.Add(title, _note_text);
                        id++;
                    }
                    else
                    {
                        listKeys.Add(title + "_" + id.ToString());
                        ht.Add(title + "_" + id.ToString(), _note_text);
                        id++;                     
                    }
                   
                }
            } 
            //////////////
            UInt16 len3 = (UInt16)listKeys.Count;
            UInt16 hdrLen = 0x10;
            BinaryWriter view_writer = new BinaryWriter(File.OpenWrite(path + "\\VIEWABLE.IDX"), Encoding.UTF8);
            byte[] bytes;
            view_writer.Write(hdrLen); // header
            view_writer.Write(new byte[] { 1, 0 }); // publisher id
            view_writer.Write(new byte[] { 1, 0 }); // book id
            view_writer.Write(new byte[] { 0, 0 }); // set id
            view_writer.Write(new byte[] { 1, 0 ,1, 1}); // versions
            view_writer.Write(new byte[] { 0, 1, 0, 0 });
            //
            view_writer.Write(hdrLen); // header
            view_writer.Write(new byte[] { 0, 0, 0, 0 });
            view_writer.Write(new byte[] { 0, 0, 0, 0 });
            view_writer.Write(false); //not compressed.
            view_writer.Write(new byte[] { 0 }); //reserved
            view_writer.Write(len3); //size of block entry
            view_writer.Write(new byte[] { 0, 0 }); //reserved

            BinaryWriter book_writer = new BinaryWriter(File.OpenWrite(path + "\\BOOK.DAT"), Encoding.UTF8);
            book_writer.Write(hdrLen); // header
            book_writer.Write(new byte[] { 1, 0 }); // publisher id
            book_writer.Write(new byte[] { 1, 0 }); // book id
            book_writer.Write(new byte[] { 0, 0 }); // set id
            book_writer.Write(new byte[] { 1, 0, 1, 1 }); // versions
            book_writer.Write(new byte[] { 0, 1, 0, 0 });
            //hdr ctrl word area            
            //string w_title="{\\Title:"+this.ABBREVIATION+"}\n";

            string w_title = "{\\Title:"+this.ABBREVIATION+"}\n{\\Copyright:}\n{\\Edition:1}\n{\\EditionID:1}\n{\\PublisherID:1}\n{\\Publisher:}\n{\\PublisherLoc:}\n{\\BookID:1}\n{\\SetID:}\n{\\SetName:}\n{\\SyncType:}\n";
            bytes = Encoding.UTF8.GetBytes(w_title);
            book_writer.Write((UInt32)bytes.Length);
            book_writer.Write(bytes);
            book_writer.Write(0xff);
            BinaryWriter section_writer = new BinaryWriter(File.OpenWrite(path + "\\SECTIONS.IDX"), Encoding.UTF8);
            section_writer.Write(hdrLen); // header
            section_writer.Write(new byte[] { 1, 0 }); // publisher id
            section_writer.Write(new byte[] { 1, 0 }); // book id
            section_writer.Write(new byte[] { 0, 0 }); // set id
            section_writer.Write(new byte[] { 1, 0, 1, 1 }); // versions
            section_writer.Write(new byte[] { 0, 1, 0, 0 });
            //
            section_writer.Write(hdrLen); // header
            section_writer.Write(new byte[] { 0, 0, 0, 0 }); // Number of non-glossary 
            section_writer.Write(new byte[] { 0, 0, 0, 0 }); //  Number of glossary 
            
            section_writer.Write(len3); // Level Entry size
            section_writer.Write(new byte[] { 0, 0, 0, 0 }); // reserved
            //
            ///
            
            
            UInt32 len, offset;
            UInt16 len2;

            UInt32 i2 = 0;
            UInt32 prev_entry = 0,curr_entry=0,next_entry =0;
            UInt16 keylen = 0;
            UInt32 offset_section = (UInt32)(0x20 + ht.Keys.Count * 29);
           
            foreach (string key in listKeys)
            {
                percent_complete = 50 + (int)i2 * 50 / listKeys.Count;

                
                curr_entry = (UInt32)section_writer.BaseStream.Length;
                next_entry = curr_entry + 29;

                section_writer.Write(new byte[] { 0, 0, 0, 0 }); // parent pointer
                section_writer.Write(prev_entry);
                prev_entry = curr_entry;
                section_writer.Write(next_entry);                
                section_writer.Write(i2);
                section_writer.Write(new byte[] { 0, 0 });
                section_writer.Write(new byte[] { 1 });
                section_writer.Write(offset_section); //pointer to name of section
                section_writer.Write(new byte[] { 0, 0, 0, 0 ,0 ,0});
                //
                bytes = Encoding.UTF8.GetBytes(key.Trim());
                offset_section = offset_section + (uint)(2 + bytes.Length);
                len2 = (UInt16)bytes.Length;
                keylen += len2;
                //
                bytes = Encoding.UTF8.GetBytes(((string)ht[key]).Trim());
                len = (UInt32)bytes.Length;
                offset = (UInt32) book_writer.BaseStream.Position;  //////////////////////
                view_writer.Write(offset);
                view_writer.Write(len);
                view_writer.Write(new byte[] { 0, 0, 0, 0 });

                book_writer.Write(bytes); // data
                i2++;
            }
            //
            
            foreach (string key in listKeys)
            {
                bytes = Encoding.UTF8.GetBytes(key.Trim());
                len2 = (UInt16)bytes.Length;
                section_writer.Write(len2); // length
                section_writer.Write(bytes);
            }
            book_writer.Close();
            section_writer.Close();
            view_writer.Close();
            percent_complete = 100;
            */
        }
        //


    }	
}
