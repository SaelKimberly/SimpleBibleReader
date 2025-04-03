using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Collections;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using RtfPipe;
using TwoFish;
using System.Security.Cryptography;
using System.Globalization;
using System.Threading.Tasks;
using System.IO.Compression;
using System.Threading;
using Microsoft.Win32;

namespace Simple_Bible_Reader
{
    public class FormatUtil
    {
        public enum CONTENT_TYPE
        {
            BIBLE,
            COMMENTARY,
            DICTIONARY,
            BOOK
        };

        // the blanks are not yet figured out
        public static string[] shortBookNames = Localization.getShortBookNames();
        public static string[] longBookNames = Localization.getBookNames();
        
        public static string[] logosBookNames = new string[] { "Gen", "Exod", "Lev", "Num", "Deut", "Josh", "Judg", "Rth", "1Sa", "2Sa", "1Ki", "2Ki", "1Ch", "2Ch", "Ezr", "Neh", "Esth", "Jb", "Psa", "Prov", "Ecc", "Song", "Isa", "Jer", "Lam", "Ezek", "Dan", "Hos", "Joe", "Am", "Obad", "Jon", "Mic", "Nah", "Hab", "Zeph", "Hag", "Zec", "Mal", "Matt", "Mrk", "Luk", "Joh", "Act", "Rom", "1Co", "2Co", "Gal", "Eph", "Phil", "Col", "1Th", "2Th", "1Ti", "2Ti", "Tit", "Phm", "Heb", "Jas", "1Pe", "2Pe", "1Jo", "2Jo", "3Jo", "Jud", "Rev" };
        public static string[] olbBookNames = new string[] { "Ge","Ex","Le","Nu","De","Jos","Jud","Ru","1Sa","2Sa","1Ki","2Ki","1Ch","2Ch","Ezr","Ne","Es","Job","Ps","Pr","Ec","So","Isa","Jer","La","Eze","Da","Ho","Joe","Am","Ob","Jon","Mic","Na","Hab","Zep","Hag","Zec","Mal","Mt","Mr","Lu","Joh","Ac","Ro","1Co","2Co","Ga","Eph","Php","Col","1Th","2Th","1Ti","2Ti","Tit","Phm","Heb","Jas","1Pe","2Pe","1Jo","2Jo","3Jo","Jude","Re"};

        // index same as in Locale, same index as in Zefania
        public static string[] swordBookAbbrInZefaniaIndex = new string[] { "Gen", "Exod", "Lev", "Num", "Deut", "Josh", "Judg", "Ruth", "1Sam", "2Sam", "1Kgs", "2Kgs", "1Chr", "2Chr", "Ezra", "Neh", "Esth", "Job", "Ps", "Prov", "Eccl", "Song", "Isa", "Jer", "Lam", "Ezek", "Dan", "Hos", "Joel", "Amos", "Obad", "Jonah", "Mic", "Nah", "Hab", "Zeph", "Hag", "Zech", "Mal", "Matt", "Mark", "Luke", "John", "Acts", "Rom", "1Cor", "2Cor", "Gal", "Eph", "Phil", "Col", "1Thess", "2Thess", "1Tim", "2Tim", "Titus", "Phlm", "Heb", "Jas", "1Pet", "2Pet", "1John", "2John", "3John", "Jude", "Rev", "Jdt", "Wis", "Tob", "Sir", "Bar", "1Macc", "2Macc", "AddDan", "AddEsth", "PrMan", "3Macc", "4Macc", "EpJer", "1Esd", "2Esd", "", "", "EpLao", "1En", "kGen", "Sus", "Bel", "AddPs", "PrAzar", "EsthGr", "DanGr", "Jub" };


        public static int[] pdb_book_numbers = new int[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 160, 190, 220, 230, 240, 250, 260, 290, 300, 310, 330, 340, 350, 360, 370, 380, 390, 400, 410, 420, 430, 440, 450, 460, 470, 480, 490, 500, 510, 520, 530, 540, 550, 560, 570, 580, 590, 600, 610, 620, 630, 640, 650, 660, 670, 680, 690, 700, 710, 720, 730, 145, 146, 170, 180, 200, 210, 215, 216, 231, 235, 270, 280, 285, 315, 320, 335, 345, 346 };
        private static Hashtable pdb_book_ht=null;

        private static bool _initialized = false;
        private static XmlDocument _bookDoc = null;
        private static int[] chapter_count = new int[swordBookAbbrInZefaniaIndex.Length + 1];
        private static Hashtable[] verse_count = new Hashtable[swordBookAbbrInZefaniaIndex.Length + 1];
        private static Hashtable _possibleAbbr = null;
        
        //private static string rtf_header_esword = @"{\rtf1\ansi\ansicpg1252\deff0\deflang1033{\fonttbl{\f1\fnil\fprq2\fcharset161 TITUS Cyberbit Basic;}{\f2\fnil\fprq2\fcharset177 TITUS Cyberbit Basic;}{\f3\fnil\fprq2\fcharset0 TITUS Cyberbit Basic;}{\f4\fnil\fprq2\fcharset136 PMingLiU;}{\f5\fnil\fprq2\fcharset204 Georgia;}{\f6\fnil\fprq2\fcharset238 Georgia;}{\f7\fnil\fprq2\fcharset134 PMingLiU;}{\f8\fnil\fprq2\fcharset222 Cordia New;}{\f9\fnil\fprq2\fcharset129 Gulim;}{\f10\fnil\fprq2\fcharset178 TITUS Cyberbit Basic;}{\f11\fnil\fprq2\fcharset162 Georgia;}{\f12\fnil\fprq2\fcharset163 Times New Roman;}{\f13\fnil\fprq2\fcharset128 MS Mincho;}}{\colortbl ;\red0\green0\blue0;\red0\green0\blue255;\red0\green255\blue255;\red0\green255\blue0;\red255\green0\blue255;\red255\green0\blue0;\red255\green255\blue0;\red255\green255\blue255;\red0\green0\blue128;\red0\green128\blue128;\red0\green128\blue0;\red128\green0\blue128;\red128\green0\blue0;\red128\green128\blue0;\red128\green128\blue128;\red192\green192\blue192;}{\viewkind4\uc1\pard\plain\cf0\fs\par}";
        private static string rtf_header = "{\\rtf1\\ansi\n";
        
        
        public static Dictionary<int, int> eSword2Zefania_Apocrypha_Map = new Dictionary<int, int>()
        {

            {67,69},
            {68,67},
            {69,68},
            {74,80},
            {75,81},
            {78,76},
            {76,77},
            {77,78},
        };

        public static Dictionary<int, int> Zefania2eSword_Apocrypha_Map = new Dictionary<int, int>()
        {
            {69,67},
            {67,68},
            {68,69},
            {80,74},
            {81,75},
            {76,78},
            {77,76},
            {78,77},
        };

        internal static int findZlibHeaderIdx(byte[] inBytes, int tillOffset)
        {
            // use most common zlib headers
           for(int i=0;i<tillOffset;i++)
            {
                if ((inBytes[i] == 0x78 && inBytes[i+1] == 0x9C) || (inBytes[i] == 0x78 && inBytes[i+1] == 0xDA))
                    return i;                
            }
            return -1;
        }

        public static bool IsInitialized()
        {
            return _initialized;
        }

        public static void Init()
        {
            if (!_initialized)
            {
                // NO SUPPORT FOR APOGRAPHA YET
                // bookid.xml doesn't have/support it.

                if (Localization.bookNames == null)
                    Localization.getBookNames();//to initialize it.
                //
                _bookDoc = new XmlDocument();
                _bookDoc.LoadXml(Properties.Resources.bookid);
                _possibleAbbr = new Hashtable();
                string[] abbr;
                int k;
                for (int i = 1; i <= 66; i++)
                {
                    chapter_count[i - 1] = int.Parse((string)(_bookDoc.SelectSingleNode("/books/book[position()=" + System.Convert.ToString((short)i) + "]/chapters").InnerText));
                    verse_count[i - 1] = new Hashtable();
                    for (int j = 1; j <= chapter_count[i - 1]; j++)
                    {
                        try
                        {
                            verse_count[i - 1].Add(j, int.Parse(System.Convert.ToString(int.Parse(_bookDoc.SelectSingleNode("/books/book[position()=" + System.Convert.ToString((short)i) + "]/chapter[@n=\'" + j.ToString() + "\']/@verses").InnerText))));
                        }
                        catch (Exception)
                        {
                            Themes.MessageBox("Book:" + i.ToString() + "; Chapter:" + j.ToString());
                        }
                    }
                    //
                    XmlNodeList _books = _bookDoc.SelectNodes("/books/book[position()=" + System.Convert.ToString((short)i) + "]/abbrev");
                    abbr = new string[_books.Count * 3 + 2 + Localization.bookNames.Count-1]; // ignore english so, subtract 1
                    abbr[0] = i.ToString();
                    abbr[1] = _bookDoc.SelectSingleNode("/books/book[position()=" + System.Convert.ToString((short)i) + "]/@name").InnerText;
                    k = 2;
                    foreach (XmlNode _book in _books)
                    {
                        abbr[k] = _book.InnerText;
                        abbr[k + 1] = _book.InnerText.ToLower();
                        abbr[k + 2] = _book.InnerText.ToUpper();                                               
                        k = k + 3;
                    }

                    foreach (string enc in Localization.bookNames.Keys)
                    {
                        if (enc == "en")
                            continue;
                        abbr[k] = ((string[])Localization.bookNames[enc])[i-1];
                        k++;
                    }
                    _possibleAbbr.Add(i, abbr);
                }
                //
                _initialized = true;
            }
        }

        public static string[] getPossibleAbbr(int book_no)
        {
            return (string[])(_possibleAbbr[book_no]);
        }

        public static int convertPDBBkNoToNormalBkNo(int pdb_bk_no)
        {
            if(pdb_book_ht==null)
            {
                pdb_book_ht = new Hashtable();
                for (int i = 0; i < pdb_book_numbers.Length; i++)
                    pdb_book_ht.Add(pdb_book_numbers[i], i + 1);
            }            
            return (int)pdb_book_ht[pdb_bk_no];
        }

        public static string getBookName(int book_no)
        {
            return Localization.getBookNames()[zefaniaBookNoToIndex(book_no)];
        }

        public static string getBookNameShort(int book_no)
        {
            return shortBookNames[book_no - 1];
        }

        public static int getBookNo(string book_name)
        {
            for (int book_no = 1; book_no <= _possibleAbbr.Count; book_no++)
            {
                foreach (string posi in ((string[])_possibleAbbr[book_no]))
                {
                    if (posi.ToLower().Equals(book_name.ToLower()))
                        return book_no;
                }
            }
            for (int book_no = 1; book_no <= Localization.getBookNames().Length; book_no++)
            {
                if (Localization.getBookNames()[zefaniaBookNoToIndex( book_no)].ToLower() == book_name.ToLower() | FormatUtil.shortBookNames[book_no - 1].ToLower() == book_name.ToLower())
                {
                    return book_no;
                }
            }

            for (int book_no = 1; book_no <= Localization.getBookNames().Length; book_no++)
            {
                if (book_name.ToLower().IndexOf(Localization.getBookNames()[zefaniaBookNoToIndex(book_no)].ToLower()) != -1)
                {
                    return book_no;
                }
            }

            return -1;
        }

        public static int GetVerseCount2(int book_no, int chapter_no)
        {
            try
            {
                XmlNodeList vers = BibleFormat.BibleXmlDocument.SelectNodes("/XMLBIBLE/BIBLEBOOK[@bnumber='" + book_no.ToString() + "']/CHAPTER[@cnumber='" + chapter_no.ToString() + "']/VERS");
                return vers.Count;
            }
            catch (Exception)
            {
                return (int)(verse_count[book_no - 1][chapter_no]);
            }            
        }


        public static int GetVerseCount(int book_no, int chapter_no)
        {
            return (int)(verse_count[book_no - 1][chapter_no]);
        }


        public static int GetChapterCount2(int zef_book_no)
        {
            try
            {
                XmlNodeList chapters = BibleFormat.BibleXmlDocument.SelectNodes("/XMLBIBLE/BIBLEBOOK[@bnumber='" + zef_book_no.ToString() + "']/CHAPTER");
                return chapters.Count;
            }
            catch (Exception)
            {
                return chapter_count[zef_book_no - 1];
            }            
        }

        public static int GetChapterCount(int book_no)
        {
            return chapter_count[book_no - 1];
        }

        public static int GetBookCount2()
        {
            try
            {
                XmlNodeList books = BibleFormat.BibleXmlDocument.SelectNodes("/XMLBIBLE/BIBLEBOOK");
                return books.Count;
            }
            catch (Exception)
            {
                return 66;
            }
        }

        public static int GetBookCount()
        {
            return 66;
        }

        public static XmlDocument BookHelperDoc
        {
            get
            {
                return _bookDoc;
            }
            set
            {
                _bookDoc = value;
            }
        }

        public static string GetRtfUnicodeEscapedString(string s)
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

        public static string UnescapeXML(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;

            string returnString = s;
            returnString = returnString.Replace("&apos;", "'");
            returnString = returnString.Replace("&quot;", "\"");
            returnString = returnString.Replace("&gt;", ">");
            returnString = returnString.Replace("&lt;", "<");
            returnString = returnString.Replace("&amp;", "&");

            return returnString;
        }

        public static string EscapeXML(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;

            string returnString = s;
            returnString = returnString.Replace("'", "&apos;");
            returnString = returnString.Replace("\"", "&quot;");
            returnString = returnString.Replace(">","&gt;");
            returnString = returnString.Replace("<","&lt;");
            returnString = returnString.Replace("&","&amp;");

            return returnString;
        }

        public static string Mid(string s, int a, int b)
        {
            string temp = s.Substring(a - 1, b);
            return temp;
        }

        // RVF Specifications here - https://www.trichview.com/help/index.html?rvf_specification.html
        // TODO: Convert to HTML
        public static string Rvf2Text(byte[] bytes)
        {
            StringBuilder TEXT = new StringBuilder();

            //HTML.Append("<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"></head><body>");

            List<int> recordStartPositions = new List<int>();
            //
            string[] versionInfo = null;
            string[] rvfRecHeader = null;

            int recHdrRecordType;
            int recHdrStringsCount;
            int recHdrParagraph;
            int recHdrItemOptions;
            int recHdrQuery;
            int recHdrTag;

            string textdata;
            string rvfRecHeaderStr;
            /*
            string pic_name;
            string pic_delphi_type;
            string pic_src;
            byte[] pic_data;
            uint pic_len;
            */

            /////////////
            List<char> clist = new List<char>();
            foreach (byte b in bytes)
                clist.Add((char)b);
            char[] ca = clist.ToArray();
            string test = new string(ca);

            Regex rx = new Regex(@"([0-9,-]+ [0-9]+ [0-9,-]+ [0-9]+ [0-9]+[^\r\n]*[\r\n])");
            foreach (Match mx in rx.Matches(test))
            {
                recordStartPositions.Add(mx.Index);
            }

            ///////////////

            BinaryReader reader = new BinaryReader(new MemoryStream(bytes), Encoding.UTF8);
            // 
            if (new string(reader.ReadChars(2)) == "-8")
            {
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                versionInfo = readRecord(reader).Split(new char[] { ' ' });
                //Trace.WriteLine("Version Record Type:" + versionInfo[0]);
                //Trace.WriteLine("Version:" + versionInfo[1]);
                //Trace.WriteLine("SubVersion:" + versionInfo[2]);
                //if (versionInfo.Length == 4)
                //   Trace.WriteLine("SubSubVersion:" + versionInfo[3]);
            }
            else
                reader.BaseStream.Seek(0, SeekOrigin.Begin);            
            int end_pos = (int)reader.BaseStream.Length - 1;
            for (int i = 0; i < recordStartPositions.Count; i++)
            {
                reader.BaseStream.Seek(recordStartPositions[i], SeekOrigin.Begin);

                //reading records------------------------------
                rvfRecHeaderStr = readRecord(reader);

                rvfRecHeader = rvfRecHeaderStr.Split(new char[] { ' ' });
                recHdrRecordType = int.Parse(rvfRecHeader[0].Trim());
                recHdrStringsCount = int.Parse(rvfRecHeader[1].Trim());
                recHdrParagraph = int.Parse(rvfRecHeader[2].Trim());
                recHdrItemOptions = int.Parse(rvfRecHeader[3].Trim());
                recHdrQuery = int.Parse(rvfRecHeader[4].Trim());
                try
                {
                    recHdrTag = int.Parse(rvfRecHeader[5].Trim());
                }
                catch (Exception)
                {
                    recHdrTag = 0;
                } 
                //Trace.WriteLine("--------------Record Type # " + recHdrRecordType + " @ " + recordStartPositions[i] + "--------------");

                if (recHdrRecordType >= 0) //// text
                {
                    for (int j = 0; j < recHdrStringsCount; j++)
                    {
                        if (i + 1 < recordStartPositions.Count)
                            end_pos = recordStartPositions[i + 1] - 1;
                        else
                            end_pos = (int)reader.BaseStream.Length - 1;
                        if (reader.BaseStream.Position >= end_pos)
                            break;
                        textdata = readText(reader, end_pos);
                        //TEXT.Append("<p>");                        
                        //textdata = textdata.Replace("\r\n", "<br/>");
                        TEXT.Append(textdata);
                        TEXT.Append("\r\n");
                        //TEXT.Append("</p>");
                    }
                }
                else if (recHdrRecordType == -3) //// picture
                {
                    // not yet supported
                    /*
                        if (i + 1 < recordStartPositions.Count)
                            end_pos = recordStartPositions[i + 1] - 1;
                        else
                            end_pos = (int)reader.BaseStream.Length - 1;
                        if (reader.BaseStream.Position >= end_pos)
                            break;
                        pic_name = readRecText(reader, end_pos).Trim();
                        pic_delphi_type = readRecText(reader, end_pos).Trim();

                        pic_src = "";
                        if (pic_delphi_type == "TBitmap")
                        {
                            pic_src = "data:image/bmp;base64,";
                        }
                        else if (pic_delphi_type == "TIcon")
                        {
                            pic_src = "data:image/x-icon;base64,";
                        }
                        else if (pic_delphi_type == "TMetafile")
                        {
                            pic_src = "data:image/x-wmf;base64,";
                        }
                        else if (pic_delphi_type == "TJPEGImage")
                        {
                            pic_src = "data:image/jpeg;base64,";
                        }
                        
                        pic_len=reader.ReadUInt16();
                        reader.ReadByte();//skip 0
                        pic_data = reader.ReadBytes((int)pic_len);
                        //TBitmap is different from BMP file. Hence some extra effort is required to display                         
                        pic_src = pic_src + Convert.ToBase64String(pic_data);
                        HTML.Append("<img src='");
                        HTML.Append(pic_src);
                        HTML.Append("'>");
                     */
                    //HTML.Append("[IMG - not yet supported]");
                }
            }

            reader.Close();
            //HTML.Append("</body></html>");            
            return TEXT.ToString();
        }

        private static string readRecText(BinaryReader reader, int limit_pos)
        {
            byte ch = 0, pch = 0;
            List<byte> sb = new List<byte>();
            while (!((ch = reader.ReadByte()) == 10 && pch == 13))
            {
                sb.Add(ch);
                pch = ch;
                if (reader.BaseStream.Position > limit_pos)
                    break;
            }
            byte[] text_bytes = sb.ToArray();
            string text = Encoding.ASCII.GetString(text_bytes);
            return text;
        }

        private static string readText(BinaryReader reader, int limit_pos)
        {
            byte ch = 0, pch = 0;
            List<byte> sb = new List<byte>();
            sb.Add(0xff);
            sb.Add(0xfe);
            while (!((ch = reader.ReadByte()) == 10 && pch == 13))
            {
                if (ch == 0x20 && pch == 0x29)
                {
                    sb.RemoveAt(sb.Count - 1);
                    sb.Add(13);
                    sb.Add(0);
                    sb.Add(10);
                    sb.Add(0);
                }
                else
                    sb.Add(ch);
                pch = ch;
                if (reader.BaseStream.Position > limit_pos)
                    break;
            }
            byte[] text_bytes = sb.ToArray();
            string text = Encoding.Unicode.GetString(text_bytes);
            return text;
        }

        private static string readRecord(BinaryReader reader)
        {
            int ch = -1, pch = -1;
            StringBuilder sb = new StringBuilder();
            while (!((ch = reader.ReadByte()) == 10 && pch == 13))
            {
                sb.Append(Convert.ToChar(ch));
                pch = ch;
            }
            return sb.ToString().Trim();
        }

        public static string RemoveHTMLTags(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }

        public static string StripHTML(string source)
        {
            try
            {
                string result;

                // Remove HTML Development formatting
                // Replace line breaks with space
                // because browsers inserts space
                result = source.Replace("\r", " ");
                // Replace line breaks with space
                // because browsers inserts space
                result = result.Replace("\n", " ");
                // Remove step-formatting
                result = result.Replace("\t", string.Empty);
                // Remove repeating spaces because browsers ignore them
                result = System.Text.RegularExpressions.Regex.Replace(result,
                                                                      @"( )+", " ");

                // Remove the header (prepare first by clearing attributes)
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*head([^>])*>", "<head>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<( )*(/)( )*head( )*>)", "</head>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(<head>).*(</head>)", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // remove all scripts (prepare first by clearing attributes)
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*script([^>])*>", "<script>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<( )*(/)( )*script( )*>)", "</script>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //result = System.Text.RegularExpressions.Regex.Replace(result,
                //         @"(<script>)([^(<script>\.</script>)])*(</script>)",
                //         string.Empty,
                //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<script>).*(</script>)", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // remove all styles (prepare first by clearing attributes)
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*style([^>])*>", "<style>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<( )*(/)( )*style( )*>)", "</style>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(<style>).*(</style>)", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // insert tabs in spaces of <td> tags
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*td([^>])*>", "\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // insert line breaks in places of <BR> and <LI> tags
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*br( )*>", "\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*li( )*>", "\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // insert line paragraphs (double line breaks) in place
                // if <P>, <DIV> and <TR> tags
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*div([^>])*>", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*tr([^>])*>", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*p([^>])*>", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // Remove remaining tags like <a>, links, images,
                // comments etc - anything that's enclosed inside < >
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<[^>]*>", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // replace special characters:
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @" ", " ",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&bull;", " * ",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&lsaquo;", "<",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&rsaquo;", ">",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&trade;", "(tm)",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&frasl;", "/",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&lt;", "<",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&gt;", ">",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&copy;", "(c)",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&reg;", "(r)",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Remove all others. More can be added, see
                // http://hotwired.lycos.com/webmonkey/reference/special_characters/
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&(.{2,6});", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // for testing
                //System.Text.RegularExpressions.Regex.Replace(result,
                //       this.txtRegex.Text,string.Empty,
                //       System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // make line breaking consistent
                result = result.Replace("\n", "\r");

                // Remove extra line breaks and tabs:
                // replace over 2 breaks with 2 and over 4 tabs with 4.
                // Prepare first to remove any whitespaces in between
                // the escaped characters and remove redundant tabs in between line breaks
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)( )+(\r)", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\t)( )+(\t)", "\t\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\t)( )+(\r)", "\t\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)( )+(\t)", "\r\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Remove redundant tabs
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)(\t)+(\r)", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Remove multiple tabs following a line break with just one tab
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)(\t)+", "\r\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Initial replacement target string for line breaks
                string breaks = "\r\r\r";
                // Initial replacement target string for tabs
                string tabs = "\t\t\t\t\t";
                for (int index = 0; index < result.Length; index++)
                {
                    result = result.Replace(breaks, "\r\r");
                    result = result.Replace(tabs, "\t\t\t\t");
                    breaks = breaks + "\r";
                    tabs = tabs + "\t";
                }

                // That's it.
                return result;
            }
            catch
            {
                Themes.MessageBox("Error");
                return source;
            }
        }

        public static bool InStr(string original, string contains)
        {
            if (original.IndexOf(contains) > -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool InStr(int start, string original, string contains)
        {
            return InStr(original.Substring(start), contains);
        }

        public static int InStr2(string original, string contains)
        {
            if (original.IndexOf(contains) > -1)
            {
                return original.IndexOf(contains);
            }
            else
            {
                return 0;
            }
        }

        public static int InStr2(int start, string original, string contains)
        {
            return InStr2(original.Substring(start), contains);
        }

        public static int Len(string str)
        {
            return str.Length;
        }

        private static byte[] ParseHex(string s)
        {
            var list = new List<byte>();
            if ((s.Length & 1) == 1) s += '0';
            for (var i = 0; i < s.Length; i += 2)
            {
                var c = s.Substring(i, 2);
                var b = byte.Parse(c, NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat);
                list.Add(b);
            }
            return list.ToArray();
        }

        public static byte[] eSwordTwoFish(byte[] input, TwofishManagedTransformMode dir)
        {
            var mode = CipherMode.ECB;
            var keysize = 128;
            var key = ParseHex(Passwords.eSword9_Password); 
            var iv = ParseHex("00000000000000000000000000000000");
            byte[] outputBuffer = null;
            var bufferSize = 1024;

            using (var twofish = new TwofishManaged
            {
                KeySize = keysize,
                Mode = mode,
                Padding = PaddingMode.None
            })
            using (var transform = twofish.NewEncryptor(key, mode, iv, dir))
            {
                using (var reader = new BinaryReader(new MemoryStream(input)))
                {
                    long total = 0;
                    for (var inputBuffer = reader.ReadBytes(bufferSize * 16);
                        inputBuffer.Length > 0;
                        inputBuffer = reader.ReadBytes(bufferSize * 16))
                    {
                        Array.Resize(ref inputBuffer, (inputBuffer.Length + 15) & ~15);
                        outputBuffer = new byte[inputBuffer.Length];
                        if (mode == CipherMode.ECB)
                        {
                            Parallel.For(0, inputBuffer.Length / 16,
                                i => { transform.TransformBlock(inputBuffer, 16 * i, 16, outputBuffer, 16 * i); });
                        }
                        total += inputBuffer.Length;
                    }
                }
            }
            return outputBuffer;
        }

        public static byte[] eSwordTwoFishDecrypt(byte[] input)
        {
            return eSwordTwoFish(input, TwofishManagedTransformMode.Decrypt);
        }

        public static byte[] eSwordTwoFishEncrypt(byte[] input)
        {
            return eSwordTwoFish(input, TwofishManagedTransformMode.Encrypt);
        }

        public static byte[] eSword_zLibUncompress(byte[] inBytes)
        {
            MemoryStream compressedFileStream = new System.IO.MemoryStream(inBytes);
            compressedFileStream.Seek(2, SeekOrigin.Begin);
            MemoryStream outputFileStream = new MemoryStream();
            var decompressor = new DeflateStream(compressedFileStream, CompressionMode.Decompress);
            decompressor.CopyTo(outputFileStream);
            decompressor.Close();
            return outputFileStream.ToArray();
        }

        public static byte[] zLibUncompress(byte[] inBytes)
        {
            MemoryStream compressedFileStream = new System.IO.MemoryStream(inBytes);
            compressedFileStream.Seek(2, SeekOrigin.Begin);
            MemoryStream outputFileStream = new MemoryStream();
            var decompressor = new DeflateStream(compressedFileStream, CompressionMode.Decompress);
            decompressor.CopyTo(outputFileStream);
            decompressor.Close();
            return outputFileStream.ToArray();
        }

        public static byte[] zLibCompress(byte[] inBytes)
        {
            MemoryStream originalFileStream = new System.IO.MemoryStream(inBytes);
            MemoryStream compressedFileStream = new MemoryStream();
            var compressor = new DeflateStream(compressedFileStream, CompressionMode.Compress);
            originalFileStream.CopyTo(compressor);
            compressor.Close();
            //
            //       with header
            /*
            byte[] compressed = compressedFileStream.ToArray();
            byte[] withheader = new byte[compressed.Length + 2];
            withheader[0] = 0x78;
            withheader[1] = 0xDA;
            Array.Copy(compressed,0, withheader, 2, compressed.Length);
            return withheader;
            */
            return compressedFileStream.ToArray();
        }

        public static string convertDefaultToUTFEncoding(string value)
        {
            if (value == null)
                return "";
            byte[] bytes = Encoding.Default.GetBytes(value);
            return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }

        public static int swordBookAbbrToZefaniaBookNo(string swordAbbr)
        {
            int book_no = Array.FindIndex(swordBookAbbrInZefaniaIndex, row => row == swordAbbr)+1;
            if (book_no > 89)
                return book_no + 811;
            else
                return book_no;
        }

        public static int zefaniaBookNoToIndex(int book_no)
        {
            if (book_no > 900)
                return book_no - 811-1;
            else
                return book_no-1;
        }

        public static int zefaniaIndexToBookNo(int zef_idx)
        {
            if (zef_idx + 1 > 89)
                return zef_idx + 811 + 1;
            else
                return zef_idx + 1;
        }

        // code from
        // https://stackoverflow.com/questions/29299632/how-to-convert-html-to-rtf-using-javascript#46656606
        // converted from javascript to c#
        public static string convertHtmlToRtf(string html, string rtf_header = null)
        {

            if (GlobalMemory.getInstance().UseCtrlForHtmlRtfConv)
            {
                return convertHtmlToRtfSlowMethod(html);
            }


            string tmpRichText;
            bool hasHyperlinks = false;
            var richText = html;

            // Singleton tags
            richText = Regex.Replace(richText,@"<(?:hr)(?:\s+[^>]*)?\s*[\/]?>", "{\\pard \\brdrb \\brdrs \\brdrw10 \\brsp20 \\par}\n{\\pard\\par}\n",RegexOptions.IgnoreCase);
            richText = Regex.Replace(richText,@"<(?:br)(?:\s+[^>]*)?\s*[\/]?>", "{\\pard\\par}\n", RegexOptions.IgnoreCase);

            // Empty tags
            richText = Regex.Replace(richText,@"<(?:p|div|section|article)(?:\s+[^>]*)?\s*[\/]>", "{\\pard\\par}\n", RegexOptions.IgnoreCase);
            richText = Regex.Replace(richText,@"<(?:[^>]+)\/>", "");
          
            // Hyperlinks
            richText = Regex.Replace(richText,
                @"<a(?:\s+[^>]*)?(?:\s+href=([""'])(?:javascript:void\(0?\);?|#|return false;?|void\(0?\);?|)\1)(?:\s+[^>]*)?>", "{{{\n", RegexOptions.IgnoreCase);
            tmpRichText = richText;
            /*
            richText = Regex.Replace(richText,
                @"<a(?:\s+[^>]*)?(?:\s+href=([""'])(.+)\1)(?:\s+[^>]*)?>",
                @"{\field{\*\fldinst{HYPERLINK\n \""$2\""\n}}{\fldrslt{\ul\cf1\n", RegexOptions.IgnoreCase);
            */
            hasHyperlinks = richText != tmpRichText;
            richText = Regex.Replace(richText,@"<a(?:\s+[^>]*)?>", "{{{\n");
            richText = Regex.Replace(richText,@"<\/a(?:\s+[^>]*)?>", "\n}}}");

            // Start tags
            richText = Regex.Replace(richText,@"<(?:b|strong)(?:\s+[^>]*)?>", "{\\b\n");
            richText = Regex.Replace(richText,@"<(?:i|em)(?:\s+[^>]*)?>", "{\\i\n");
            richText = Regex.Replace(richText,@"<(?:u|ins)(?:\s+[^>]*)?>", "{\\ul\n");
            richText = Regex.Replace(richText,@"<(?:strike|del)(?:\s+[^>]*)?>", "{\\strike\n");
            richText = Regex.Replace(richText,@"<sup(?:\s+[^>]*)?>", "{\\super\n");
            richText = Regex.Replace(richText,@"<sub(?:\s+[^>]*)?>", "{\\sub\n");
            richText = Regex.Replace(richText,@"<(?:p|div|section|article)(?:\s+[^>]*)?>", "{\\pard\n");

            // header tags to bold
            richText = Regex.Replace(richText, @"<h[1-6](?:\s+[^>]*)?>", "{\\b\n");
            richText = Regex.Replace(richText, @"<\/(?:h[1-6])(?:\s+[^>]*)?>", "\n}");

            //new line
            richText = Regex.Replace(richText, @"<br[\/]>", "\\line\n");

            // End tags
            richText = Regex.Replace(richText,@"<\/(?:p|div|section|article)(?:\s+[^>]*)?>", "\n\\par}\n");
            richText = Regex.Replace(richText,@"<\/(?:b|strong|i|em|u|ins|strike|del|sup|sub)(?:\s+[^>]*)?>", "\n}");

            // Strip any other remaining HTML tags [but leave their contents]
            richText = Regex.Replace(richText,@"<(?:[^>]+)>", "");

            // Prefix and suffix the rich text with the necessary syntax
            if (rtf_header == null)
                rtf_header = "{\\rtf1\\ansi\n";
            richText = rtf_header + (hasHyperlinks ? "{\\colortbl\n;\n\\red0\\green0\\blue255;\n}\n" : "") + richText + "\n}";

            //

            return richText;
        }

        /*
        //  not used anymore
        
        public static string convertESwordSpecificRtfTags2Html(string _verse_text)
        {
            // e-Sword Specific
            //italics - translator text
            if (_verse_text.IndexOf("cf15") != -1)
            {
                _verse_text = Regex.Replace(_verse_text, @"\{\\cf15\\i ([^\}]*)\}", "<i>$1</i>", RegexOptions.IgnoreCase);
            }
            //lexicon
            if (_verse_text.IndexOf("cf11") != -1)
            {
                _verse_text = Regex.Replace(_verse_text, @"([^\{]*)(\{\\cf11\\super [H|G])([^\}]*)\}", "<gr str=\"$3\">$1</gr>", RegexOptions.IgnoreCase);
                _verse_text = Regex.Replace(_verse_text, @"(<gr str="")([0-9]*)([^HG]*)(\(){0,1}([H|G])([0-9]*)(\(){0,1}([^\""]*)("">)", @"<gr str=""$2""></gr><gr str=""$6"">", RegexOptions.IgnoreCase);
            }
            //red text
            if (_verse_text.IndexOf("cf6") != -1)
            {
                _verse_text = Regex.Replace(_verse_text, @"\{\\cf6 ([^\}]*)\}", "<STYLE css=\"color:#ff0000\">$1</STYLE>", RegexOptions.IgnoreCase);
            }
            return _verse_text;
        }
        */

        public static string convertRtfToHtmlSlowMethod(string _verse_text)
        {
            RichTextBox rtbControl = new RichTextBox();
            if (!_verse_text.StartsWith("{\\rtf1"))
                _verse_text = rtf_header + _verse_text;
            rtbControl.Rtf = _verse_text;
            _verse_text = rtbControl.Text;


            /*
             *  to avoid STA cross thread issue
             * 
            Thread thread = new Thread(function_name);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            _verse_text = richTextOutput;
            */
            return _verse_text;
        }

        public static string convertRtfToHtmlFastMethod(string rtf)
        {            
            if (!rtf.StartsWith("{\\rtf1"))
                rtf = rtf_header + rtf;
            return Rtf.ToHtml(rtf);
        }

        public static string convertRtfToHtml(string rtf)
        {
            // rtf text does not contain rtf tags
            if (rtf.Trim() == "" || !(rtf.Contains("{\\") || rtf.StartsWith("\\") || rtf.Contains("\\cf")))
                return rtf;

            if(GlobalMemory.getInstance().UseCtrlForHtmlRtfConv)
            {
                return convertRtfToHtmlSlowMethod(rtf);
            }


            string htmloutput = rtf;
            try
            {
                htmloutput = convertRtfToHtmlFastMethod(htmloutput);                
            }
            catch (Exception)
            {
                //unknown error, try again using the slow method.
                // show method should strip most of rtf tags and convert to text
                htmloutput = convertRtfToHtmlSlowMethod(htmloutput);
            }
            return htmloutput;
        }

        private static string convertHtmlToRtfSlowMethod(string html)
        {
            string rtf_text = "";
            Thread thread = new Thread(
            () =>
            {
                var webBrowser = new WebBrowser();
                var rtbControl = new RichTextBox();
                webBrowser.CreateControl(); // only if needed
                webBrowser.DocumentText = html;
                while (webBrowser.DocumentText != html)
                    Application.DoEvents();
                webBrowser.Document.ExecCommand("SelectAll", false, null);
                webBrowser.Document.ExecCommand("Copy", false, null);
                rtbControl.Paste();
                rtf_text = rtbControl.Rtf;
                webBrowser.Dispose();
                rtbControl.Dispose();
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            return rtf_text;
        }

        public static void setPrinterOptionsLegacy()
        {
            string keyName = @"Software\Microsoft\Internet Explorer\PageSetup";
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyName, true))
            {
                if (key != null)
                {
                    try
                    {
                        //string old_footer = key.GetValue("footer").ToString();
                        //string old_header = key.GetValue("header").ToString();
                        key.SetValue("footer", "Page &p of &P");
                        key.SetValue("header", "&w");
                    }
                    catch (Exception) { }
                }
            }
        }
    }
}
