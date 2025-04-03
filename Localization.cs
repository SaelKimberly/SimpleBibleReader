using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Simple_Bible_Reader
{
    public class Localization
    {
        static Dictionary<string, Dictionary<string, string>> localeStrings = null;

        // --------------------------------------------------bookNames---------------------------------------------------
        public static Hashtable bookNames = null;

        //English - Internally, everything works based on this index.
        //Except for 901-915, the index is +811 or -811
        //only during import export of zefania.

        public static string[] shortBookNames = new string[] { "Gen", "Exo", "Lev", "Num", "Deu", "Jos", "Jdg", "Rut", "1Sa", "2Sa", "1Ki", "2Ki", "1Ch", "2Ch", "Ezr", "Neh", "Est", "Job", "Psa", "Pro", "Ecc", "Sol", "Isa", "Jer", "Lam", "Eze", "Dan", "Hos", "Joe", "Amo", "Oba", "Jon", "Mic", "Nah", "Hab", "Zep", "Hag", "Zec", "Mal", "Mat", "Mar", "Luk", "Joh", "Act", "Rom", "1Co", "2Co", "Gal", "Eph", "Phi", "Col", "1Th", "2Th", "1Ti", "2Ti", "Tit", "Phm", "Heb", "Jam", "1Pe", "2Pe", "1Jo", "2Jo", "3Jo", "Jud", "Rev", "Jdt", "Wis", "Tob", "Sir", "Bar", "1Ma", "2Ma", "xDa", "xEs", "Man", "3Ma", "4Ma", "LJe", "1Es", "2Es", "Ode", "PsS", "Lao", "1En", "kGn", "Sus", "Bel", "Ps2", /* After this, zefania bookid is -811 */ "Aza", "EsG", "DaG", "Jub", "4Ez", "5Ez", "6Ez", "Ps3", "2Ba", "4Ba", "LBa", "1Mq", "2Mq", "3Mq", "Rep" };

        public static string[] bookNames_en = new string[] { "Genesis", "Exodus", "Leviticus", "Numbers", "Deuteronomy", "Joshua", "Judges", "Ruth", "1 Samuel", "2 Samuel", "1 Kings", "2 Kings", "1 Chronicles", "2 Chronicles", "Ezra", "Nehemiah", "Esther", "Job", "Psalm", "Proverbs", "Ecclesiastes", "Song of Solomon", "Isaiah", "Jeremiah", "Lamentations", "Ezekiel", "Daniel", "Hosea", "Joel", "Amos", "Obadiah", "Jonah", "Micah", "Nahum", "Habakkuk", "Zephaniah", "Haggai", "Zechariah", "Malachi", "Matthew", "Mark", "Luke", "John", "Acts", "Romans", "1 Corinthians", "2 Corinthians", "Galatians", "Ephesians", "Philippians", "Colossians", "1 Thessalonians", "2 Thessalonians", "1 Timothy", "2 Timothy", "Titus", "Philemon", "Hebrews", "James", "1 Peter", "2 Peter", "1 John", "2 John", "3 John", "Jude", "Revelation", "Judit", "Wisdom", "Tobit", "Sirach", "Baruch", "1 Maccabees", "2 Maccabees", "Additions to Daniel", "Additions to Esther", "Prayer of Manasseh", "3 Maccabees", "4 Maccabees", "Letter of Jeremiah", "1 Esdras", "2 Esdras", "Odes", "Psalms of Solomon", "Epistle to the Laodiceans", "1 Enoch", "kGen", "Susanna", "Bel and the Dragon", "Psalm 151", /* After this, zefania bookid is -811 */ "Prayer of Azariah", "Greek Esther", "Greek Daniel", "Jubilees", "Ezra Apocalypse", "5 Ezra", "6 Ezra", "5 Apocryphal Syriac Psalms", "Syriac Apocalypse of Baruch", "4 Baruch", "Letter of Baruch", "1 Meqabyan", "2 Meqabyan", "3 Meqabyan", "Reproof" };

        public static string[] bookNameKeys = new string[] { "Genesis", "Exodus", "Leviticus", "Numbers", "Deuteronomy", "Joshua", "Judges", "Ruth", "1_Samuel", "2_Samuel", "1_Kings", "2_Kings", "1_Chronicles", "2_Chronicles", "Ezra", "Nehemiah", "Esther", "Job", "Psalm", "Proverbs", "Ecclesiastes", "Song_of_Solomon", "Isaiah", "Jeremiah", "Lamentations", "Ezekiel", "Daniel", "Hosea", "Joel", "Amos", "Obadiah", "Jonah", "Micah", "Nahum", "Habakkuk", "Zephaniah", "Haggai", "Zechariah", "Malachi", "Matthew", "Mark", "Luke", "John", "Acts", "Romans", "1_Corinthians", "2_Corinthians", "Galatians", "Ephesians", "Philippians", "Colossians", "1_Thessalonians", "2_Thessalonians", "1_Timothy", "2_Timothy", "Titus", "Philemon", "Hebrews", "James", "1_Peter", "2_Peter", "1_John", "2_John", "3_John", "Jude", "Revelation", "Judit", "Wisdom", "Tobit", "Sirach", "Baruch", "1_Maccabees", "2_Maccabees", "Additions_to_Daniel", "Additions_to_Esther", "Prayer_of_Manasseh", "3_Maccabees", "4_Maccabees", "Letter_of_Jeremiah", "1_Esdras", "2_Esdras", "Odes", "Psalms_of_Solomon", "Epistle_to_the_Laodiceans", "1_Enoch", "kGen", "Susanna", "Bel_and_the_Dragon", "Psalm_151", "Prayer_of_Azariah", "Greek_Esther", "Greek_Daniel", "Jubilees", "Ezra_Apocalypse", "5_Ezra", "6_Ezra", "5_Apocryphal_Syriac_Psalms", "Syriac_Apocalypse_of_Baruch", "4_Baruch", "Letter_of_Baruch", "1_Meqabyan", "2_Meqabyan", "3_Meqabyan", "Reproof" };


        //Tamil
        //public static string[] bookNames_ta = new string[] { "ஆதியாகமம்", "யாத்திராகமம்", "லேவியராகமம்", "எண்ணாகமம்", " உபாகமம்", "யோசுவா", "நியாயாதிபதிகள்", "ரூத்", "I சாமுவேல்", "II சாமுவேல்", "I இராஜாக்கள்", "II இராஜாக்கள்", "I நாளாகமம்", "II நாளாகமம்", "எஸ்றா", "நெகேமியா", "எஸ்தர்", "யோபு", "சங்கீதம்", "நீதிமொழிகள்", "பிரசங்கி", "உன்னதப்பாட்டு", "ஏசாயா", "எரேமியா", "புலம்பல்", "எசேக்கியேல்", "தானியேல்", "ஓசியா", "யோவேல்", "ஆமோஸ்", "ஒபதியா", "யோனா", "மீகா", "நாகூம்", "ஆபகூக்", "செப்பனியா", "ஆகாய்", "சகரியா", "மல்கியா", "மத்தேயு", "மாற்கு", "லூக்கா", "யோவான்", "அப்போஸ்தலருடைய நடபடிகள்", "ரோமர்", "I கொரிந்தியர்", "II கொரிந்தியர்", "கலாத்தியர்", "எபேசியர்", "பிலிப்பியர்", "கொலோசெயர்", "I தெசலோனிக்கேயர்", "II தெசலோனிக்கேயர்", "I தீமோத்தேயு", "II தீமோத்தேயு", "தீத்து", "பிலேமோன்", "எபிரெயர்", "யாக்கோபு", "I பேதுரு", "II பேதுரு", "I யோவான்", "II யோவான்", "III யோவான்", "யூதா", "வெளிப்படுத்தின விசேஷம்" };
        //Spanish
        //public static string[] bookNames_es = new string[] { "Génesis", "Éxodo", "Levítico", "Números", "Deuteronomio", "Josué", "Jueces", "Rut", "1 Samuel", "2 Samuel", "1 Reyes", "2 Reyes", "1 Crónicas", "2 Crónicas", "Esdras", "Nehemías", "Ester", "Job", "Salmos", "Proverbios", "Eclesiastés", "Cantares", "Isaías", "Jeremías", "Lamentaciones", "Ezequiel", "Daniel", "Oseas", "Joel", "Amós", "Abdías", "Jonás", "Miqueas", "Nahum", "Habacuc", "Sofonías", "Hageo", "Zacarías", "Malaquías", "Mateo", "Marcos", "Lucas", "Juan", "Hechos", "Romanos", "1 Corintios", "2 Corintios", "Gálatas", "Efesios", "Filipenses", "Colosenses", "1 Tesalonicenses", "2 Tesalonicenses", "1 Timoteo", "2 Timoteo", "Tito", "Filemón", "Hebreos", "Santiago", "1 Pedro", "2 Pedro", "1 Juan", "2 Juan", "3 Juan", "Judas", "Apocalipsis" };


        public static void loadLocalizations()
        {
            if(localeStrings == null)
            {
                localeStrings = new Dictionary<string, Dictionary<string, string>>();
                localeStrings.Add("en", _loadLangLocalizations(Properties.Resources.en));
                localeStrings.Add("es", _loadLangLocalizations(Properties.Resources.es));
                localeStrings.Add("ta", _loadLangLocalizations(Properties.Resources.ta));
                localeStrings.Add("pt", _loadLangLocalizations(Properties.Resources.pt));
            }

            if (!localeStrings.ContainsKey(GlobalMemory.getInstance().Locale))
                GlobalMemory.getInstance().Locale = "en";


        }

        private static Dictionary<string, string> _loadLangLocalizations(string localeTexts)
        {
            Dictionary<string, string> langLocaleStrings = new Dictionary<string, string>();
            string[] lines = localeTexts.Split(new char[] { '\n' });
            foreach (string line in lines)
            {
                if (line.Trim() != "")
                {
                    string[] kvpair = line.Trim().Split(new char[] { '=' });
                    langLocaleStrings.Add(kvpair[0], kvpair[1]);
                }
            }
            return langLocaleStrings;
        }





        // --------------------------------------------------Words---------------------------------------------------

        private static string getLocalized(string word)
        {
            loadLocalizations();
            if (localeStrings[GlobalMemory.getInstance().Locale].ContainsKey(word))
                return localeStrings[GlobalMemory.getInstance().Locale][word];
            else
                return localeStrings["en"][word];
        }

        public static string Old_Testament
        {
            get
            {                
                return getLocalized("Old_Testament");
            }
        }

        public static string New_Testament
        {
            get
            {
                return getLocalized("New_Testament");
            }
        }

        public static string Apocrypha
        {
            get
            {
                return getLocalized("Apocrypha");
            }
        }




        public static string MainPage_ParagraphOne
        {
            get
            {                
                return getLocalized("MainPage_ParagraphOne");                
            }
        }

        public static string MainPage_QuickGuide
        {
            get
            {
                return getLocalized("MainPage_QuickGuide");
            }
        }

        public static string MainPage_QuickGuideText
        {
            get
            {               
                return getLocalized("MainPage_QuickGuideText");
            }
        }

        public static string MainPage_SupportedBibleFormats
        {
            get
            {
                return getLocalized("MainPage_SupportedBibleFormats");
            }
        }

        public static string MainPage_BibleFormats
        {
            get
            {
                return getLocalized("MainPage_BibleFormats");
            }
        }

        public static string MainPage_SupportedCommentaryFormats
        {
            get
            {
                return getLocalized("MainPage_SupportedCommentaryFormats");
            }
        }

        public static string MainPage_CommentaryFormats
        {
            get
            {
                return getLocalized("MainPage_CommentaryFormats");
            }
        }

        public static string MainPage_SupportedDictionaryFormats
        {
            get
            {               
                return getLocalized("MainPage_SupportedDictionaryFormats");
            }
        }

        public static string MainPage_DictionaryFormats
        {
            get
            {
                return getLocalized("MainPage_DictionaryFormats");
            }
        }

        public static string MainPage_SupportedBookFormats
        {
            get
            {
                return getLocalized("MainPage_SupportedBookFormats");
            }
        }

        public static string MainPage_BookFormats
        {
            get
            {
                return getLocalized("MainPage_BookFormats");
            }
        }

        public static string MainPage_About_PublicDomain
        {
            get
            {
                string public_domain = @"<p>
This is free and unencumbered software released into the public domain.<br/>
<br/>
Anyone is free to copy, modify, publish, use, compile, sell, or
distribute this software, either in source code form or as a compiled
binary, for any purpose, commercial or non-commercial, and by any
means.<br/>
<br/>
In jurisdictions that recognize copyright laws, the author or authors
of this software dedicate any and all copyright interest in the
software to the public domain. We make this dedication for the benefit
of the public at large and to the detriment of our heirs and
successors. We intend this dedication to be an overt act of
relinquishment in perpetuity of all present and future rights to this
software under copyright law.<br/>
<br/>
THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.<br/>
<br/>
For more information, please refer to &lt;http://unlicense.org/&gt;</p>
";
                return public_domain;
            }
        /*
            get
            {
                loadLocalizations();
                return localeStrings[GlobalMemory.getInstance().Locale]["MainPage_About_NoSell"];
            }
        */
        }

        public static string MainPage_About_Website
        {
            get
            {
                return getLocalized("MainPage_About_Website");
            }
        }
        
        public static string MainPage_PluginHelpText
        {
            get
            {
                return getLocalized("MainPage_PluginHelpText");
            }
        }

        public static string MainPage_Credits
        {
            get
            {
                return getLocalized("MainPage_Credits");
            }
        }

        public static string MainPage_Attributions
        {
            get
            {
                return getLocalized("MainPage_Attributions");
            }
        }

        public static string MainPage_Licenses
        {
            get
            {
                return getLocalized("MainPage_Licenses");
            }
        }

        public static string ErrorPage_NoBibleBooksFound
        {
            get
            {               
                return getLocalized("ErrorPage_NoBibleBooksFound");
            }
        }

        public static string ErrorPage_ErrorDisplaying
        {
            get
            {
                return getLocalized("ErrorPage_ErrorDisplaying");
            }
        }


        public static string ErrorPage_Html
        {
            get
            {
                string errhtml = getLocalized("ErrorPage_Html");
                errhtml = errhtml.Replace("@AUTHOR_WEBSITE", GlobalMemory.AUTHOR_WEBSITE);
                return errhtml;
            }
        }

        public static string BatchConversion_SourceFolderDoesNotExist
        {
            get
            {
                return getLocalized("BatchConversion_SourceFolderDoesNotExist");
            }
        }

        public static string BatchConversion_TargetFolderDoesNotExist
        {
            get
            {
                return getLocalized("BatchConversion_TargetFolderDoesNotExist");
            }
        }

        public static string Error
        {
            get
            {
                return getLocalized("Error");
            }
        }

        public static string BatchConversion_ErrorConverting
        {
            get
            {
                return getLocalized("BatchConversion_ErrorConverting");
            }
        }

        public static string BatchConversion_Continue
        {
            get
            {
                return getLocalized("BatchConversion_Continue");
            }
        }

        public static string BatchConversion_ProcessBusyQuestion
        {
            get
            {
                return getLocalized("BatchConversion_ProcessBusyQuestion");
            }
        }

        public static string BatchConversion_Batch
        {
            get
            {
                return getLocalized("BatchConversion_Batch");
            }
        }

        public static string BatchConversion_Completed
        {
            get
            {
                return getLocalized("BatchConversion_Completed");
            }
        }

        public static string BatchConversion_CancelledByUser
        {
            get
            {
                return getLocalized("BatchConversion_CancelledByUser");
            }
        }

        public static string Find_SearchResultsFor
        {
            get
            {
                return getLocalized("Find_SearchResultsFor");
            }
        }

        private static string[] _loadLangBookNanes(string lang)
        {
            string[] bookNames_lang = new string[bookNameKeys.Length];
            for (int i = 0; i < bookNameKeys.Length; i++)
            {
                if(localeStrings[lang].ContainsKey(bookNameKeys[i]))
                    bookNames_lang[i] = localeStrings[lang][bookNameKeys[i]];
                else
                    bookNames_lang[i] = localeStrings["en"][bookNameKeys[i]];
            }
            return bookNames_lang;
        }

        public static string[] getBookNames()
        {
            loadLocalizations();

            if (bookNames == null)
            {
                bookNames = new Hashtable();
                bookNames.Add("en", _loadLangBookNanes("en"));
                bookNames.Add("es", _loadLangBookNanes("es"));
                bookNames.Add("ta", _loadLangBookNanes("ta"));
                bookNames.Add("pt", _loadLangBookNanes("pt"));
            }
            string[] bnames = (string[])(bookNames[GlobalMemory.getInstance().Locale]);
            if (bnames == null)
                return (string[])(bookNames["en"]);
            else
                return bnames;
        }

        public static string[] getShortBookNames()
        {
            // to do localization
            return shortBookNames;
        }
    }
}
