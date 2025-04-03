using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using static System.Environment;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;

namespace Simple_Bible_Reader
{
    public class GlobalMemory
    {
        bool dev_mode = true;

        public const string AUTHOR_WEBSITE = "trumpet-call.org/simplebiblereader/";

        static GlobalMemory _gm = null;

        public const int DIRECTION_LTR = 0;
        public const int DIRECTION_RTL = 1;        

        int _dir = 0;

        public string LegacyFontSizeBIB = "x-small";
        public int LEGACY_FONT_SIZE_IDX_BIB = 1;

        public string LegacyFontSizeCMT = "x-small";
        public int LEGACY_FONT_SIZE_IDX_CMT = 1;

        public string LegacyFontSizeDCT = "x-small";
        public int LEGACY_FONT_SIZE_IDX_DICT = 1;

        public double ZOOMFACTOR_IDX_BIB = 1.0;
        public double ZOOMFACTOR_IDX_CMT = 1.0;
        public double ZOOMFACTOR_IDX_DICT = 1.0;

        string font_family_bib = "";
        string font_family_cmt = "";
        string font_family_dct = "";

        public string Locale = "en";
       
        bool transliterate = false;

        public bool ParseBible = true;
        public bool parseBibleBkgndCleaner = true;

        public bool ParseCommentary = true;
        public bool ParseDictionary = true;
        
        public bool ParseBook = true;
        public bool parseBookBkgndCleaner = true;

        public int Mp3Volume = 90;
        public int Mp3Speed = -2;
        public int Mp3VoiceIndex = 0;
        public bool Mp3Transliterate = false;

        public bool CommentaryIcon = true;

        public bool addRedText = false;

        public bool EnablePlugins = false;

        public bool ConsoleMode = false;

        public bool use_libsword = true;

        public bool DisplayBIBRaw = false;
        public bool DisplayCMTRaw = false;
        public bool DisplayBOKRaw = false;
        public bool DisplayDCTRaw = false;


        public bool ConvertRtfToHtmlBible = false;
        public bool ConvertRtfToHtmlCommentary = false;
        public bool ConvertRtfToHtmlDictionary = false;
        public bool ConvertRtfToHtmlBook = false;

        public bool ConvertHtmlToRtfBible = false;
        public bool ConvertHtmlToRtfCommentary = false;
        public bool ConvertHtmlToRtfDictionary = false;
        public bool ConvertHtmlToRtfBook = false;

        public bool stripHtmlTagsBible = false;
        public bool stripHtmlTagsBook = false;
        public bool stripHtmlTagsCommentary = false;
        public bool stripHtmlTagsDictionary = false;

        public bool autoSetParsingBible = true;
        public bool autoSetParsingBook = true;
        public bool autoSetParsingCommentary = true;
        public bool autoSetParsingDictionary = true;

        public string Versification = "kjva";

        public DarkMode.DarkMode.Theme Theme = DarkMode.DarkMode.Theme.SYSTEM;
        public string SelectedTheme = "Default";

        public bool EditBibleVerse = false;

        public static string[] HtmlRenderer = {"WEBCONTROL","WEBVIEW2"};
        public bool useCustomRenderer = false;
        public string useRenderer = "WEBVIEW2";

        // PLugin Data
        public string pluginRxp = @"
# Example plugin to convert theWord to e-Sword

# Before you continue, enable both options 
#   - 'Don't parse but render raw text.' and 
#   - 'Display unrendered raw content'.
# Any line starting with a hash or empty line is ignored
# The first line is the search pattern in regular expression
# The next line is the replacement pattern.
# The regex rules follows .Net C# patterns.  

# convert strongs number
<gr\sstr=""([^""]+)"">([^<]+)</gr>
<num>$1</num>$2

# convert Jesus words to red text
<STYLE\scss=""color:#ff0000"">([^<]+)</STYLE>
<red>$1</red>

# Additional rules...";
        public List<string> pluginList = new List<string>();
        public bool pluginEscapeXML = false;
        public bool SaveLoadFilePref = false;
        public bool UseCtrlForHtmlRtfConv = false;

        public static string localAppDataPath = GetFolderPath(SpecialFolder.LocalApplicationData)+Path.DirectorySeparatorChar +"SimpleBibleReader"+ Path.DirectorySeparatorChar;
        
        public static string[] prefsToSaveInFile = new string[] { 
            "ParseBible", "parseBibleBkgndCleaner", "addRedText", "ParseBook", 
            "parseBookBkgndCleaner", "ParseCommentary", "ParseDictionary", "CommentaryIcon", 
            "Mp3Volume", "Mp3Speed", "Mp3VoiceIndex", "Mp3Transliterate", "ConvertRtfToHtmlBible", 
            "ConvertRtfToHtmlCommentary", "ConvertRtfToHtmlDictionary", "ConvertRtfToHtmlBook", 
            "stripHtmlTagsBible", "stripHtmlTagsBook", "stripHtmlTagsCommentary", 
            "stripHtmlTagsDictionary", "ConvertHtmlToRtfBible", "ConvertHtmlToRtfBook", 
            "ConvertHtmlToRtfCommentary", "ConvertHtmlToRtfDictionary", "autoSetParsingBible", 
            "autoSetParsingBook", "autoSetParsingCommentary", "autoSetParsingDictionary", 
            "Versification", "EditBibleVerse", "SaveLoadFilePref",
        "DisplayBIBRaw","DisplayBOKRaw","DisplayCMTRaw","DisplayDCTRaw","use_libsword","UseCtrlForHtmlRtfConv",
                "useCustomRenderer","useRenderer"};


        public static GlobalMemory getInstance()
        {
            if (_gm == null)
                _gm = new GlobalMemory();
            return _gm;
        }

        public bool DevMode
        {
            get
            {
                return dev_mode;
            }
            set
            {
                dev_mode = value;
            }
        }

        public bool Transliteration
        {
            get
            {
                return transliterate;
            }
            set
            {
                transliterate = value;
            }
        }

        public int Direction
        {
            get
            {
                return _dir;
            }
            set
            {
                _dir = value;
            }
        }

        public string FontFamilyBIB
        {
            get
            {
                return font_family_bib;
            }
            set
            {
                font_family_bib = value;
            }
        }

        public string FontFamilyCMT
        {
            get
            {
                return font_family_cmt;
            }
            set
            {
                font_family_cmt = value;
            }
        }

        public string FontFamilyDICT
        {
            get
            {
                return font_family_dct;
            }
            set
            {
                font_family_dct = value;
            }
        }

        public static string changeReleaseHtml()
        {
            StringBuilder relnotes = new StringBuilder();
            relnotes.Append("<html><head>");
            relnotes.Append(Themes.CSS_UI);
            relnotes.Append("</head><body>");

            relnotes.Append(SbrInfo.RELEASE_INFO);

            relnotes.Append("</body></html>");
            return relnotes.ToString();
        }

        public static void deletePrefFile()
        {
            if (File.Exists(GlobalMemory.localAppDataPath + "pref.props"))
                File.Delete(GlobalMemory.localAppDataPath + "pref.props");
        }

        public static void savePrefToFile()
        {
            List<string> prefsToSave = GlobalMemory.prefsToSaveInFile.ToList();
            StringBuilder sb = new StringBuilder();
            Type fieldsType = typeof(GlobalMemory);
            FieldInfo[] fields = fieldsType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo fi in fields)
                if (prefsToSave.Contains(fi.Name))
                {
                    sb.Append(fi.Name + "=" + fi.GetValue(GlobalMemory.getInstance()).ToString() + "\r\n");
                }

            if(!Directory.Exists(GlobalMemory.localAppDataPath))
                Directory.CreateDirectory(GlobalMemory.localAppDataPath);
            File.WriteAllText(GlobalMemory.localAppDataPath + "pref.props", sb.ToString());
        }

        public static void loadPrefFromFile()
        {
            if (File.Exists(GlobalMemory.localAppDataPath + "pref.props"))
            {
                string[] props = File.ReadAllLines(GlobalMemory.localAppDataPath + "pref.props");
                string[] tmp = null;
                string propname = null;
                string propval = null;

                Type fieldsType = typeof(GlobalMemory);
                FieldInfo fi = null;
                foreach (string prop in props)
                {
                    tmp = prop.Split('=');
                    propname = tmp[0];
                    propval = tmp[1];
                    fi = fieldsType.GetField(propname);

                    if (fi.FieldType == typeof(bool))
                        fi.SetValue(GlobalMemory.getInstance(), bool.Parse(propval));
                    else if (fi.FieldType == typeof(int))
                        fi.SetValue(GlobalMemory.getInstance(), int.Parse(propval));
                    else
                        fi.SetValue(GlobalMemory.getInstance(), propval);
                }
            }
        }
    }
}
