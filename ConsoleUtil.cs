using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Collections;
using System.Xml;
using System.IO;

namespace Simple_Bible_Reader
{
    public class ConsoleUtil
    {

        public static ArrayList errors = new ArrayList();

        public static void addLog(String errmsg)
        {
            errors.Add(errmsg);
        }

        public static void writeLog(String filename)
        {
            string[] err_array = new string[errors.Count];
            for (int i = 0; i < errors.Count; i++)
                err_array[i] = (string)errors[i];
            File.WriteAllLines(filename, err_array);
        }
                
        public static void Convert(string format_type, int in_type, string in_File, int out_type, string out_File, bool mClean)
        {
            switch (format_type.ToLower())
            {
                case "bib":
                    BibleFormat bible_format_input = null;
                    bible_format_input = BibleFormat.BibleFactory(in_File, in_type);
                    if (bible_format_input == null)
                    {
                        addLog("Invalid input type of '" + in_type + "' specified for '" + format_type+"'");
                        break;
                    }
                    bible_format_input.Load();
                    //     
                    if (mClean)
                    {
                        GlobalMemory.getInstance().ParseBible = true;
                        bible_format_input.CLEANING = true;
                        clean(true);
                    }
                    else
                    {
                        bible_format_input.CLEANING = false;
                        GlobalMemory.getInstance().ParseBible = false;
                    }
                    //
                    BibleFormat bible_format_export = BibleFormat.doExportBible(out_File, out_type);
                    if (bible_format_export == null)
                    {
                        addLog("Invalid output type of '" + out_type + "' specified for '" + format_type + "'");
                        break;
                    }
                    bible_format_export.CLEANING = false;
                    if (bible_format_export == null)
                    {
                        addLog("Error in creating export file.");
                    }
                    bible_format_export.ExportBible(out_File, out_type);
                    break;
                case "cmt":
                    CommentaryFormat commentary_format_input = null;
                    commentary_format_input = CommentaryFormat.CommentaryFactory(in_File, in_type);
                    if (commentary_format_input == null)
                    {
                        addLog("Invalid input type of '" + in_type + "' specified for '" + format_type + "'");
                        break;
                    }
                    commentary_format_input.Load();
                    //           
                    if (mClean)
                    {
                        GlobalMemory.getInstance().ParseCommentary = true;
                        commentary_format_input.CLEANING = true;
                    }
                    else
                    {
                        commentary_format_input.CLEANING = false;
                        GlobalMemory.getInstance().ParseCommentary = false;
                    }
                    //
                    CommentaryFormat commentary_format_export = CommentaryFormat.doExportCommentary(out_File, out_type);
                    if (commentary_format_export == null)
                    {
                        addLog("Invalid output type of '" + out_type + "' specified for '" + format_type + "'");
                        break;
                    }
                    commentary_format_export.CLEANING = false;
                    if (commentary_format_export == null)
                    {
                        addLog("Error in creating export file.");
                    }
                    commentary_format_export.ExportCommentary(out_File, out_type);
                    break;
                case "dct":
                    DictionaryFormat dictionary_format_input = null;
                    dictionary_format_input = DictionaryFormat.DictionaryFactory(in_File, in_type);
                    if (dictionary_format_input == null)
                    {
                        addLog("Invalid input type of '" + in_type + "' specified for '" + format_type + "'");
                        break;
                    }
                    dictionary_format_input.Load();
                    //           
                    if (mClean)
                    {
                        GlobalMemory.getInstance().ParseDictionary = true;
                        dictionary_format_input.CLEANING = true;
                    }
                    else
                    {
                        dictionary_format_input.CLEANING = false;
                        GlobalMemory.getInstance().ParseDictionary = false;
                    }
                    //
                    DictionaryFormat dictionary_format_export = DictionaryFormat.doExportDictionary(out_File, out_type);
                    if (dictionary_format_export == null)
                    {
                        addLog("Invalid output type of '" + out_type + "' specified for '" + format_type + "'");
                        break;
                    }
                    dictionary_format_export.CLEANING = false;
                    if (dictionary_format_export == null)
                    {
                        addLog("Error in creating export file.");
                    }
                    dictionary_format_export.ExportDictionary(out_File, out_type);
                    break;
                case "bok":
                    BookFormat book_format_input = null;
                    book_format_input = BookFormat.BookFactory(in_File, in_type);
                    if (book_format_input == null)
                    {
                        addLog("Invalid input type of '" + in_type + "' specified for '" + format_type + "'");
                        break;
                    }
                    book_format_input.Load();
                    //           
                    if (mClean)
                    {
                        GlobalMemory.getInstance().ParseBook = true;
                        book_format_input.CLEANING = true;
                        clean(true);
                    }
                    else
                    {
                        book_format_input.CLEANING = false;
                        GlobalMemory.getInstance().ParseBook = false;
                    }
                    //
                    BookFormat book_format_export = BookFormat.doExportBook(out_File, out_type);
                    if (book_format_export == null)
                    {
                        addLog("Invalid output type of '" + out_type + "' specified for '" + format_type + "'");
                        break;
                    }
                    book_format_export.CLEANING = false;
                    if (book_format_export == null)
                    {
                        addLog("Error in creating export file.");
                    }
                    book_format_export.ExportBook(out_File, out_type);
                    break;
                default:
                    goto case "bib";
            }
        }

        public static void clean(bool is_bible)
        {
            XmlNodeList verses;
            // background cleaner is only for Bible and Books. cleaning on dictionary and commentary takes place during load.
            if (is_bible)
                verses = BibleFormat.getInstance().getXmlDoc().SelectNodes("//VERS");
            else
                verses = BookFormat.getInstance().getXmlDoc().SelectNodes("//NOTES");

            if (verses == null)
                return;
            int count = 0;
            foreach (XmlNode verse in verses)
            {
                try
                {
                    count++;
                    verse.InnerXml = FormatUtil.convertRtfToHtml(verse.InnerText);
                }
                catch (Exception)
                {
                    //Console.WriteLine(ex.Message);                   
                }

            }
        }

    }
}
