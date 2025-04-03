using System.Data;
using System;
using System.Xml;
using System.Data.SQLite;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Collections;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Simple_Bible_Reader
{

    public class SwordSearcherDictionaryFormat : DictionaryFormat
    {

        System.Xml.XmlElement rootdoc;
        System.Xml.XmlElement item_doc;
        System.Xml.XmlElement desc_doc;
        System.Xml.XmlAttribute attrib_id;

        string _title_text;
        string _notes_text;

        int percent_complete = 0;

        public SwordSearcherDictionaryFormat(string file)
            : base(file)
        {

        }

        public override void Load()
        {
            DictionaryXmlDocument = new System.Xml.XmlDocument();
            XmlDeclaration dec = DictionaryXmlDocument.CreateXmlDeclaration("1.0", null, null);
            DictionaryXmlDocument.AppendChild(dec);
            rootdoc = DictionaryXmlDocument.CreateElement("dictionary");
            DictionaryXmlDocument.AppendChild(rootdoc);

            percent_complete = 0;

            if (this.FileName.EndsWith(".txt"))
            {
                string[] tmp, tmp1;
                string content;
                //swordsearcher forge import format.
                StreamReader reader = File.OpenText(this.FileName);
                content = reader.ReadToEnd();
                reader.Close();

                // identify description and abbr
                int v_idx = content.IndexOf("$$") - 2;
                string header = content.Substring(0, v_idx).Trim();
                tmp = header.Split(new char[] { ';' });
                for (int i = 0; i < tmp.Length; i++)
                {
                    if (tmp[i].Trim() == "")
                        continue;
                    tmp1 = tmp[i].Trim().Split(new char[] { ':' });
                    if (tmp1[0].Trim() == "TITLE")
                        DESCRIPTION = tmp1[1].Trim();
                    if (tmp1[1].Trim() == "ABBREVIATION")
                        ABBREVIATION = tmp1[1].Trim();
                }

                // get the contents.
                content = content.Substring(v_idx).Trim();
                content = Regex.Replace(content, "([$]{2}.*)([^\\r\\n]+)[\\r\\n]*([^$]+)", "$1|$3");
                content = Regex.Replace(content, "\\$\\$\\s\\{[^$]+", ""); // remove unnecessary tags
                content = content.Replace("\r\n$$", "$$");
                content = content.Replace("\r\n", "");
                content = content.Replace("$$", "\r\n");
                content = content.Trim();

                string[] words_text_line = content.Trim().Split('\n');

                //
                percent_complete = 0;
                for (int i = 0; i < words_text_line.Length; i++)
                {
                    percent_complete = i * 100 / words_text_line.Length;
                    if (words_text_line[i].Trim() == "")
                        continue;
                    tmp = words_text_line[i].Split(new char[] { '|' });
                    if (tmp.Length == 2)
                    {
                        _title_text = System.Security.SecurityElement.Escape(tmp[0].Trim().Replace("\"", ""));
                        _notes_text = tmp[1].Trim();
                        processLine();
                    }
                }
            }
            else
            {
                try
                {
                    int[] ssbook_offset;
                    int[] ssbook_length;
                    int total_words = 10;
                    int preface_offset;
                    int preface_length;
                    int wordslist_offset;
                    int wordslist_length;
                    int uknon3_offset;
                    int uknon3_length;

                    using (BinaryReader reader = new BinaryReader(File.OpenRead(this.FileName)))
                    {
                        int pos = 0;
                        // header text
                        string header_text = Encoding.UTF8.GetString(reader.ReadBytes(0xC8));//skip header text.
                        pos += 0xC8;
                        //Console.WriteLine("Header Text: " + header_text);

                        // bible book
                        byte len = reader.ReadByte();
                        string bible_book = Encoding.UTF8.GetString(reader.ReadBytes(0x3D - 1));//skip header text.
                        pos += 0x3D;
                        //Console.WriteLine("Bible Book: " + bible_book);

                        // abbr
                        len = reader.ReadByte();
                        //Console.WriteLine(len);
                        string abbr = Encoding.UTF8.GetString(reader.ReadBytes(0x9 - 1));//skip header text.
                        pos += 0x9;
                        //Console.WriteLine("Abbreviation: " + abbr);

                        reader.ReadBytes(0x2); //0x0 ,0x0

                        // abbr2?
                        len = reader.ReadByte();
                        Console.WriteLine(len);
                        string abbr2 = Encoding.UTF8.GetString(reader.ReadBytes(0x9 - 1));//skip header text.
                        pos += 0x9;
                        //Console.WriteLine("Abbreviation2: " + abbr2);

                        reader.ReadByte(); // no idea
                        pos = pos + 1;
                        total_words = reader.ReadInt16();
                        pos += 2;
                        //Console.WriteLine("Total Words?: " + total_words);

                        reader.ReadBytes(0x10);
                        pos = pos + 0x010;
                        wordslist_offset = reader.ReadInt32();
                        pos += 4;
                        wordslist_length = reader.ReadInt16();
                        pos += 2;

                        // no idea
                        reader.ReadBytes(0x6);
                        pos += 0x6;

                        // preface
                        preface_offset = reader.ReadInt32();
                        pos += 4;
                        preface_length = reader.ReadInt16();
                        pos += 2;

                        reader.ReadBytes(0x6);
                        pos = pos + 0x6;
                        uknon3_offset = reader.ReadInt32();
                        pos += 4;
                        uknon3_length = reader.ReadInt16();
                        pos += 2;

                        // unknown?
                        reader.ReadBytes(0xB9);
                        pos += 0xB9;

                        ssbook_offset = new int[total_words];
                        ssbook_length = new int[total_words];

                        for (int i = 0; i < total_words; i++)
                        {
                            //
                            ssbook_offset[i] = reader.ReadInt32();
                            pos += 4;
                            ssbook_length[i] = reader.ReadInt16();
                            pos += 2;
                            reader.ReadBytes(0x6);
                            pos += 0x6;
                        }
                    }


                    byte[] filebytes = File.ReadAllBytes(this.FileName);
                    byte[] bytes_to_read;
                    using (MemoryStream mstream = new MemoryStream(filebytes))
                    {
                        bytes_to_read = new byte[wordslist_length];
                        mstream.Position = wordslist_offset;
                        mstream.Read(bytes_to_read, 0, wordslist_length);
                        string words = Encoding.UTF8.GetString(zLibUncompress(bytes_to_read));

                        string[] wordslist = SplitQuoted(words, ',', '\"');

                        total_words = wordslist.Length;
                        percent_complete = 0;
                        for (int i = 0; i < total_words; i++)
                        {
                            percent_complete = i * 100 / total_words;
                            bytes_to_read = new byte[ssbook_length[i]];
                            mstream.Position = ssbook_offset[i];
                            mstream.Read(bytes_to_read, 0, ssbook_length[i]);
                            string uncompressed = Encoding.UTF8.GetString(zLibUncompress(bytes_to_read));
                            _title_text = System.Security.SecurityElement.Escape(wordslist[i].Replace("\"", ""));
                            _notes_text = uncompressed;
                            processLine();
                        }
                        if (Regex.IsMatch(_title_text, "[G|H][0-9]*", RegexOptions.IgnoreCase))
                            STRONG = "1";
                    }
                }
                catch (Exception e)
                {

                }
            }
          
            SetProcessAsComplete();

        }

        private static string[] SplitQuoted(string input, char separator, char quotechar)
        {
            List<string> tokens = new List<string>();

            StringBuilder sb = new StringBuilder();
            bool escaped = false;
            foreach (char c in input)
            {
                if (c.Equals(separator) && !escaped)
                {
                    // we have a token
                    tokens.Add(sb.ToString().Trim());
                    sb.Clear();
                }
                else if (c.Equals(separator) && escaped)
                {
                    // ignore but add to string
                    sb.Append(c);
                }
                else if (c.Equals(quotechar))
                {
                    escaped = !escaped;
                    sb.Append(c);
                }
                else
                {
                    sb.Append(c);
                }
            }
            tokens.Add(sb.ToString().Trim());

            return tokens.ToArray();
        }
        private void processLine()
        {
            item_doc = DictionaryXmlDocument.CreateElement("item");
            attrib_id = DictionaryXmlDocument.CreateAttribute("id");
            attrib_id.Value = _title_text;
            item_doc.Attributes.Append(attrib_id);
            desc_doc = DictionaryXmlDocument.CreateElement("description");

            if (GlobalMemory.getInstance().ParseDictionary)
            {
                //_notes_text = FormatUtil.EscapeXML(FormatUtil.StripHTML(FormatUtil.RemoveHTMLTags(_notes_text)));
                if (GlobalMemory.getInstance().autoSetParsingDictionary)
                    GlobalMemory.getInstance().stripHtmlTagsDictionary = true;
                _notes_text = postProcessContent(_notes_text);
                desc_doc.InnerXml = FormatUtil.EscapeXML(_notes_text);
            }
            else
                desc_doc.InnerXml = FormatUtil.EscapeXML(_notes_text);
            item_doc.AppendChild(desc_doc);
            rootdoc.AppendChild(item_doc);
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
                return 10;
            }
        }

        private static byte[] zLibUncompress(byte[] inBytes)
        {
            MemoryStream compressedFileStream = new System.IO.MemoryStream(inBytes);
            compressedFileStream.Seek(2, SeekOrigin.Begin);
            MemoryStream outputFileStream = new MemoryStream();
            var decompressor = new DeflateStream(compressedFileStream, CompressionMode.Decompress);
            decompressor.CopyTo(outputFileStream);
            return outputFileStream.ToArray();
        }

        public override void ExportDictionary(string filename, int filter_idx)
        {
            filename = Regex.Replace(filename, "\\.ss[56789]book", "");
            if (!filename.ToLower().EndsWith(".txt"))
            {
                filename = filename + ".txt";
            }
            if (filename.Equals(""))
            {
                return;
            }

            string tmp_file = filename;

            if (System.IO.File.Exists(tmp_file))
                System.IO.File.Delete(tmp_file);

            StreamWriter writer = new System.IO.StreamWriter(tmp_file, false, Encoding.UTF8);

            writer.WriteLine("; TITLE: " + DESCRIPTION + " ");
            writer.WriteLine("; ABBREVIATION: " + ABBREVIATION + " ");
            writer.WriteLine("$$ {AUTHORDETAIL} ");
            writer.WriteLine("<p>" + COMMENTS + "</p> ");


            XmlNodeList titles = DictionaryXmlDocument.SelectNodes("/dictionary/item");
            XmlNode note = null;

            string _verse_text = "";
            for (int i = 0; i < titles.Count; i++)
            {
                percent_complete = i * 100 / titles.Count;

                note = titles[i].Attributes["id"];
                if (note != null)
                {
                    _verse_text = titles[i].InnerText;
                    writer.WriteLine("$$ " + note.Value);
                    writer.WriteLine(FormatUtil.UnescapeXML(_verse_text));
                }
            }
            writer.Close();

            percent_complete = 100;
        }
    }
}
