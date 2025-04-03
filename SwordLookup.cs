using PalmBibleExport;
using Sword;
using Sword.reader;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// adapted from LookupExample.cs from CrossWire - sword-1.9.0\bindings\csharp
namespace Simple_Bible_Reader
{
	class SwordLookup
	{
        /*
		public static void Main (string[] args)
		{
			string dir_path = "CPDV";

			Lookup("CPDV", "John 3:16", dir_path);

            //Lookup(args[0], args[1]);
            return;
		}
		*/

        /*
         * The following books does not exist in The SWORD Project. 
         * They are still added to the list to accomodate future compatibility
         
			Odes
			Psalms of Solomon
			Ezra Apocalypse
			5 Ezra
			6 Ezra
			5 Apocryphal Syriac Psalms
			Syriac Apocalypse of Baruch
			4 Baruch
			Letter of Baruch
			Reproof
		 
         */

		
        string mFilePath = ".";
		int percent = 0;
		public string statusText = "";
		Manager manager = null;
        Module module = null;

        public SwordLookup(string filePath, string module_name)
        {
            mFilePath = filePath;
            manager = new Manager(mFilePath);
            module = manager.GetModuleByName(module_name);
        }

		public Manager getManager()
		{ 
			return manager; 
		}

        public Module getModule()
        {
			return module; 
		}

        public void DisposeManager()
		{
			manager.Dispose();
		}

        public int getChaterCount(int book, string[] versificationBooks)
		{
			int chapters = 0;
            module.KeyText = versificationBooks[book] + " 1:1";
            chapters = int.Parse(module.KeyChildren.ElementAt(4));
            return chapters;
		}

        public int getVerseCount(int book, int chapter, string[] versificationBooks)
        {
            int verses = 0;
            module.KeyText = versificationBooks[book] + " " + (chapter + 1).ToString() + ":1";
            verses = int.Parse(module.KeyChildren.ElementAt(5));
            return verses;
        }

		public string GetZefaniaXML(string versification, bool parse, bool isBible)
		{
			StringBuilder z_xml = new StringBuilder();
			z_xml.Append("<XMLBIBLE>");
            if (isBible)
            {
                z_xml.Append(getBooks(versification, "ot", parse));
                z_xml.Append(getBooks(versification, "nt", parse));
            }
            else
            {
                z_xml.Append(getBooksCmtry(versification, "ot", parse));
                z_xml.Append(getBooksCmtry(versification, "nt", parse));
            }
            z_xml.Append("</XMLBIBLE>");
			percent = 100;
			return z_xml.ToString();
		}

		private string getBooks(string versification, string testament,bool parse)
		{
            int chapter_count = 0;
            string verse_text = "";
            string book_name = "";
            bool chap_valid = false;
            bool book_valid = false;
            int chap_count = 0;
            int verse_count = 0;
            string refbook = "";
            int zefbookno = 0;
            StringBuilder z_xml_books = new StringBuilder();
            StringBuilder z_book = new StringBuilder();
            StringBuilder z_chap = new StringBuilder();

            string[] swordBookAbbr = SwordVersification.getBooks(versification, testament);
            for (int book = 0; book < swordBookAbbr.Length; book++)
            {
                if(testament.Equals("ot"))
                    percent = ((book + 1) * 50) / swordBookAbbr.Length;
                else
                    percent = 50 + (((book + 1) * 50) / swordBookAbbr.Length);
                book_name = swordBookAbbr[book];
                zefbookno = FormatUtil.swordBookAbbrToZefaniaBookNo(book_name);

                if (book_name.Equals(""))
                    continue;

                // peek to check if book exists in this versification
                refbook = getParsedRef(book, swordBookAbbr);
                if (!refbook.ToLower().Equals((book_name + " 1:1").ToLower()))
                {
                    //Themes.DarkMessageBoxShow("book_idx: " + book + ", Sword:" + refbook + ", Input:" + book_name + " 1:1");
                    continue;
                }

                z_book.Length = 0;
                if (book_name == null)
                    z_book.Append("<BIBLEBOOK bnumber=\"" + zefbookno + "\">");
                else
                    z_book.Append("<BIBLEBOOK bnumber=\"" + zefbookno + "\" bname=\"" + Localization.getBookNames()[FormatUtil.zefaniaBookNoToIndex(zefbookno)] + "\">");

                chap_count = getChaterCount(book, swordBookAbbr);

                for (int chap = 0; chap < chap_count; chap++)
                {
                    z_chap.Length = 0;
                    z_chap.Append("<CHAPTER cnumber=\"" + (chap + 1) + "\">");
                    verse_count = getVerseCount(book, chap, swordBookAbbr);
                    for (int vers = 0; vers < verse_count; vers++)

                    {
                        try
                        {
                            statusText = ", Processing ... " + book_name + " " + (chap + 1).ToString() + ":" + (vers + 1).ToString();
                            verse_text = Lookup(book_name + " " + (chap + 1).ToString() + ":" + (vers + 1).ToString(), parse);
                            if (!verse_text.Equals(""))
                            {
                                
                                z_chap.Append("<VERS vnumber=\"" + (vers + 1) + "\">");
                                if (parse)
                                {
                                    if (GlobalMemory.getInstance().parseBibleBkgndCleaner)
                                        verse_text = BibleFormat.postProcessContent(verse_text);
                                    z_chap.Append(FormatUtil.EscapeXML(verse_text));
                                }
                                else
                                    z_chap.Append(FormatUtil.EscapeXML(verse_text));
                                z_chap.Append("</VERS>");
                                chap_valid = true;
                                book_valid = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            //Themes.DarkMessageBoxShow(ex.Message);
                        }
                    }

                    z_chap.Append("</CHAPTER>");
                    if (chap_valid)
                        z_book.Append(z_chap.ToString());
                    chap_valid = false;
                    chapter_count++;
                }

                z_book.Append("</BIBLEBOOK>");
                if (book_valid)
                    z_xml_books.Append(z_book.ToString());
                book_valid = false;
            }
            return z_xml_books.ToString();
        }

        private string getBooksCmtry(string versification, string testament, bool parse)
        {
            int chapter_count = 0;
            string verse_text = "";
            string book_name = "";
            bool chap_valid = false;
            bool book_valid = false;
            int chap_count = 0;
            int verse_count = 0;
            string refbook = "";
            int zefbookno = 0;
            StringBuilder z_xml_books = new StringBuilder();
            StringBuilder z_book = new StringBuilder();
            StringBuilder z_chap = new StringBuilder();

            string[] swordBookAbbr = SwordVersification.getBooks(versification, testament);
            for (int book = 0; book < swordBookAbbr.Length; book++)
            {
                if (testament.Equals("ot"))
                    percent = ((book + 1) * 50) / swordBookAbbr.Length;
                else
                    percent = 50 + (((book + 1) * 50) / swordBookAbbr.Length);
                book_name = swordBookAbbr[book];
                zefbookno = FormatUtil.swordBookAbbrToZefaniaBookNo(book_name);

                if (book_name.Equals(""))
                    continue;

                // peek to check if book exists in this versification
                refbook = getParsedRef(book, swordBookAbbr);
                if (!refbook.ToLower().Equals((book_name + " 1:1").ToLower()))
                {
                    //Themes.DarkMessageBoxShow("book_idx: " + book + ", Sword:" + refbook + ", Input:" + book_name + " 1:1");
                    continue;
                }
                z_book.Length = 0;
                if (book_name == null)
                    z_book.Append("<BIBLEBOOK bnumber=\"" + zefbookno + "\">");
                else
                    z_book.Append("<BIBLEBOOK bnumber=\"" + zefbookno + "\" bname=\"" + Localization.getBookNames()[FormatUtil.zefaniaBookNoToIndex(zefbookno)] + "\">");

                chap_count = getChaterCount(book, swordBookAbbr);

                for (int chap = 0; chap < chap_count; chap++)
                {
                    z_chap.Length = 0;
                    z_chap.Append("<CHAPTER cnumber=\"" + (chap + 1) + "\">");
                    verse_count = getVerseCount(book, chap, swordBookAbbr);
                    for (int vers = 0; vers < verse_count; vers++)

                    {
                        try
                        {
                            statusText = ", Processing ... " + book_name + " " + (chap + 1).ToString() + ":" + (vers + 1).ToString();
                            verse_text = Lookup(book_name + " " + (chap + 1).ToString() + ":" + (vers + 1).ToString(), parse);
                            if (!verse_text.Equals(""))
                            {                                
                                z_chap.Append("<VERS vnumber=\"" + (vers + 1) + "\">");
                                if (GlobalMemory.getInstance().ParseCommentary)
                                    verse_text = CommentaryFormat.postProcessContent(verse_text);
                                z_chap.Append(FormatUtil.EscapeXML(verse_text));
                                
                                z_chap.Append("</VERS>");
                                chap_valid = true;
                                book_valid = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            //Themes.DarkMessageBoxShow(ex.Message);
                        }
                    }

                    z_chap.Append("</CHAPTER>");
                    if (chap_valid)
                        z_book.Append(z_chap.ToString());
                    chap_valid = false;
                    chapter_count++;
                }

                z_book.Append("</BIBLEBOOK>");
                if (book_valid)
                    z_xml_books.Append(z_book.ToString());
                book_valid = false;
            }
            return z_xml_books.ToString();
        }

        public string getParsedRef(int bookIdx, string[] versificationBooks)
        {
            string parsedRef = "";            
            module.KeyText = versificationBooks[bookIdx] + " 1:1";
            parsedRef = module.KeyChildren.ElementAt(8);
            return parsedRef;
        }


        public string Lookup(string key, bool parse)
        {
            module.KeyText = key;
            string content = "";
            if (parse)
                content = FormatUtil.convertDefaultToUTFEncoding(module.StripText());
            else
                content = FormatUtil.convertDefaultToUTFEncoding(module.RawEntry);
            return content.Trim();
        }


        internal int GetPercentComplete()
        {
			return percent;
        }

        public static void checkAndSetSwordLibrary()
        {
            try
            {
                Manager manager = new Manager();
                foreach (var mod in manager.GetModInfoList())
                {
                    //     
                }
                GlobalMemory.getInstance().use_libsword = true;
            }
            catch (Exception ex)
            {
                GlobalMemory.getInstance().use_libsword = false;
            }
        }
    }
}
