using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple_Bible_Reader
{
    public class SbrInfo
    {
        public const string RELEASE_INFO = @"

                            <b>Release Notes - 5.6.2</b>
                            <ul>
                            <li>Bugfix: Unchecking Escape will Unescape in plugins dialog</li>
                            <li>Bible format specific properties can be edited during export</li>
                            </ul>

                            <b>Release Notes - 5.6.1</b>
                            <ul>
                            <li>'Display raw text' option in settings does not display raw text for several formats - fixed.</li>
                            </ul>

                            <b>Release Notes - 5.6.0</b>
                            <ul>
                            <li>e-Sword export to BBLX doesn't store custom properties, instead always uses default properties- fixed.</li>
                            <li>e-Sword export to BBLI doesn't display Bible information - fixed.</li>
                            </ul>

                            <b>Release Notes - 5.5.0</b>
                            <ul>
                            <li>Major compatibility release</li>
                            <li>Supports multiple Html Renders and autoselects WebBrowser Control if WebView2 is not installed. WebView2 is no longer a mandatory requirement.</li>
                            <li>Supports both x86 and x64 architectures.</li>
                            <li>Runs on Linux operating systems using Wine 9.x with limitations.</li>
                            <li>Runs on Windows Vista and above with some limitations. Recommendation is always Windows 10 and above.</li>
                            <li>.NET Framework requirement lowered to 4.6 or above.</li>
                            </ul>

                            <b>Release Notes - 5.4.9</b>
                            <ul>
                            <li>Bugfixes in theWord encrypted format (thanks to David Ford).</li>
                            <li>Several Bugfixes in theWord dictionary format.</li>
                            <li>Supports images in theWord dictionary format for display only.</li>
                            </ul>

                            <b>Release Notes - 5.4.8</b>
                            <ul>
                            <li>Button to save plugin</li>
                            <li>Experimental options - allow slow but reliable HTML/RTF conversion using WebBrowser and RichTextBox controls.</li>
                            <li>Bugfixes in dictionary plugin parsing</li>
                            </ul>

                            <b>Release Notes - 5.4.7</b>
                            <ul>
                            <li>Bugfixes in BBLI Bible format (thanks to Ngon Cyrille)</li>
                            <li>Experimental options - persistent preferences i.e, save/load prefs from file.</li>
                            </ul>

                            <b>Release Notes - 5.4.6</b>
                            <ul>
                            <li>Complete UI redesign of plugins.</li>
                            <li>Experimental options - Edit Bible Verse option allows to edit using context menu.</li>
                            </ul>

                            <b>Release Notes - 5.4.5</b>
                            <ul>
                            <li>Bugfix in raw display of bibles.</li>
                            </ul>

                            <b>Release Notes - 5.4.4</b>
                            <ul>
                            <li>Some BibleAnalyzer commentaries doesn't load - fixed.</li>
                            </ul>

                            <b>Release Notes - 5.4.3</b>
                            <ul>
                            <li>EasySlides Bible texts with empty verse references fails to load - fixed</li>
                            <li>Sword Engine doesn't work - fixed.</li>
                            </ul>

                            <b>Release Notes - 5.4.2</b>
                            <ul>
                            <li>Don't parse from preferences doesn't work - fixed</li>
                            <li>Console based operations removed in favor of multi-platform sbrcli.</li>
                            <li>Bug fixes and code improvements</li>
                            </ul>

                            <b>Release Notes - 5.4.1</b>
                            <ul>
                            <li>Commentary and Dictionary links fixed for WebView2.</li>
                            <li>Fixed red texts parsing and display.</li>
                            <li>Better dark mode support.</li>
                            <li>Automatic parser setting in preferences.</li>
                            <li>Bug fixes and code improvements</li>
                            </ul>

                            <b>Release Notes - 5.4</b>
                            <ul>
                            <li>WebBrowser replaced with WebView2.</li>
                            <li>Fixed issue unable to save Bible Preferences.</li>
                            <li>Comments in Zefania XML changed to NOTE tag within VERS as per Zefania XML Schema Definition.</li>
                            <li>Support for display themes. Experimental dark theme just for content display.</li>
                            <li>Major bug fixes and code improvements</li>
                            </ul>

                            <b>Release Notes - 5.3.1</b>
                            <ul>
                            <li>Important bug fix on loading Sword Project bibles.</li>
                            </ul>

                            <b>Release Notes - 5.3</b>
                            <ul>
                            <li>Portuguese translation for Deuterocanonical Books</li>
                            <li>Replaced cross-connect with libsword from CrossWire</li>
                            <li>Apocrypha support for The SWORD Project Bibles, MP3</li>
                            <li>Major bug fixes and code improvements</li>
                            </ul>

                            <b>Release Notes - 5.2</b>
                            <ul>
                            <li>Apocrypha support for Zefania XML Bile, MyBible, e-Sword</li>
                            <li>Several bug fixes and code improvements</li>
                            <li>Background cleaning disabled by default</li>
                            </ul>

                            <b>Release Notes - 5.1</b>
                            <ul>
                            <li>Support for e-Sword HD dcti and refi format.</li>
                            <li>Support for The Sword Project raw display/export.</li>
                            <li>Books missing in BibleAnalyzer export fixed.</li>
                            </ul>

                            <b>Release Notes - 5.0</b>
                            <ul>
                            <li>Added third-party licenses.</li>
                            <li>Minor code modification.</li>
                            </ul>

                            <b>Release Notes - 4.4</b>
                            <ul>
                            <li>Portuguese Translation</li>
                            <li>Minor Bugfix</li>
                            </ul>

                            <b>Release Notes - 4.3</b>
                            <ul>
                            <li>Support for e-Sword HD CMTI format.</li>
                            </ul>

                            <b>Release Notes - 4.2</b>
                            <ul>
                            <li>Updated UI with better toolbar icons</li>
                            <li>Bugfix on plugin processing.</li>
                            <li>A Bible reader with no Bible content? The Gospel of John from WEB translation added.</li>
                            </ul>

                            <b>Release Notes - 4.1</b>
                            <ul>
                            <li>Fixed export issue while parsing disabeld</li>
                            <li>Improved plugin support.</li>
                            <li>Updated icon with better resolution.</li>
                            <li>Updated links and several internal improvements.</li>
                            </ul>

                            <b>Release Notes - 4.0</b>
                            <ul>
                            <li>DPI awareness for high resolutions.</li>
                            <li>Fixed print issue.</li>
                            <li>Support for MyBible format.</li>
                            <li>Support e-Sword BBLI format.</li>
                            <li>Support SwordSearcher format.</li>
                            <li>Rtf to Html for better display.</li>
                            <li>Updated links and several internal improvements.</li>
                            </ul>

                            <b>Release Notes - 3.0</b>
                            <ul>
                            <li>Support for Windows 10.</li>
                            <li>Executable Bible removed.</li>
                            <li>Updated links.</li>
                            </ul>

                            <b>Release Notes - 2.9</b>
                            <ul>
                            <li>Support for VerseVIEW Bible format.</li>
                            <li>Export to theWord gives some html escaed chars - fixed.</li>
                            </ul>

                            <b>Release Notes - 2.8</b>
                            <ul>
                            <li>Support for HeavenWorld Bible format.</li>
                            <li>Support for OnlineBible Book format.</li>
                            </ul>

                            <b>Release Notes - 2.7</b>
                            <ul>
                            <li>Support for Palm Bible+ format.</li>
                            <li>Red-text simulation not enabled by default (can be enabled from Preferences) to avoid interfering with modules that already have red text.</li>
                            <li>Support for OnlineBible Commentary and Dictionary formats. Export to OnlineBible dictionaries not supported.</li>
                            </ul>

                            <b>Release Notes - 2.6</b>
                            <ul>
                            <li>Context menu on red text doesn't work - fixed.</li>
                            <li>Support for OnlineBible format.</li>
                            <li>Supports red-text for MySword for Android.</li>
                            </ul>

                            <b>Release Notes - 2.5</b>
                            <ul>
                            <li>GoBible suddenly doesn't open - fixed.</li>
                            <li>Introducing plugins based on regular expressions.</li>
                            </ul>

                            <b>Release Notes - 2.4</b>
                            <ul>
                            <li>Optional Chapter Titles</li>
                            <li>A few theWord dictionaries give error System.Byte[] - fixed.</li>
                            <li>GoBible NT gives Book names from Gen to Dan - fixed.</li>
                            </ul>

                            <b>Release Notes - 2.3</b>
                            <ul>
                            <li>Rebuilt using Microsoft .Net 2.0. Supports Windows 8</li>
                            <li>Automatically convert Christ's Words to Red Text.</li>
                            </ul>

                            <b>Release Notes - 2.2</b>
                            <ul>
                            <li>Split into two versions - A new version that supports Windows 8 (Requires .Net 4.0+) and a legacy version that supports from Windows XP to Windows 7 (Requires .Net 2.0+). The two versions will continue to co-exist temporarily until a single portable version is feasible to support all platforms including Windows 8.</li>
                            </ul>

                            <b>Release Notes - 2.1</b>
                            <ul>
                            <li><i>(Note: Version 2.0 withdrawn and re-released as 2.1 on the same day after an important bug fix)</i></li>
                            </ul>

                            <b>Release Notes - 2.0</b>
                            <ul>
                            <li>Minimum Requirement changed to Microsoft .Net 4.0 to support Windows 8.</li>
                            <li>Speak verse/chapter in context menu of reading pane to hear the Bible being read in background</li>
                            <li>Unable to export if the import file contains a single quote - Fixed</li>
                            <li>Support for e-Sword Reference Libraries (.refx files).</li>
                            </ul>

                            <b>Release Notes - 1.23</b>
                            <ul>
                            <li>Console based parameters are now available. It enables Simple Bible Reader to be silently used/or called from another programs.</li>
                            <li>Localizations are now autodetected and autoenabled</li>
                            <li>Localizations in Spanish - Thanks to Julio Behr</li>
                            </ul>

                            <b>Release Notes - 1.22</b>
                            <ul>
                            <li>Exported Unbound Bibles not able to open in 'The Unbound Bible Tools' - fixed</li>
                            <li>Experimental localization in Tamil. More languages will be added soon.</li>
                            </ul>

                            <b>Release Notes - 1.21</b>
                            <ul>
                            <li>TOPX with qoutes in titles displays System.Byte [] - fixed.</li>
                            </ul>

                            <b>Release Notes - 1.20</b>
                            <ul>
                            <li>Some files previously worked suddenly failed to load - fixed.</li>
                            </ul>

                            <b>Release Notes - 1.19</b>
                            <ul>
                            <li>Bibles exportable to LOGOS Import (docx) format.</li>
                            <li>Convert massive amount of files using batch mode.</li>
                            <li>Support for user plugins to hook into the parsing process.</li>
                            <li>Application digitally signed using trusted CA.</li>
                            <li>Greek/Hebrew letters not displayed properly in some formats- fixed.</li>
                            </ul>

                            <b>Release Notes - 1.18</b>
                            <ul>
                            <li>Bug fix for drag and drop of non-Bibles</li>
                            <li>Preference page for MP3 Bible files.</li>
                            </ul>

                            <b>Release Notes - 1.17</b>
                            <ul>
                            <li>Support for EasySlides Bibles.</li>
                            <li>Export English Bibles as MP3 files.</li>
                            <li>Export progress bar at 100% always - fixed.</li>
                            <li>Printer exception - fixed.</li>
                            </ul>

                            <b>Release Notes - 1.16</b>
                            <ul>
                            <li>Support for STEP modules but export not yet fully supported.</li>
                            </ul>

                            <b>Release Notes - 1.15</b>
                            <ul>
                            <li>Bug fix - Can't open theWord commentary modules from wordmodules.com</li>
                            </ul>

                            <b>Release Notes - 1.14</b>
                            <ul>
                            <li>Bug fix - The Word format export collapses when verses have multiple lines.</li>
                            <li>Bug fix - Some red texts disappears in e-Sword because of nested rtf tags.</li>
                            </ul>

                            <b>Release Notes - 1.13</b>
                            <ul>
                            <li>Option to unencrypt while export of e-Sword 9.x Bibles in Preferences for the support of editing Bibles using ToolTip Tool NT.</li>
                            <li>VPL format now supports Strong's</li>
                            <li>Minor Bug-fix on MySword for Android dictionary export.</li>
                            </ul>

                            <b>Release Notes - 1.12</b>
                            <ul>
                            <li>Support for the SWORD Project Books.</li>
                            <li>Support for the BibleAnalyzer Books.</li>
                            <li>Support for the MySword for Android Books.</li>
                            </ul>

                            <b>Release Notes - 1.11</b>
                            <ul>
                            <li>Support for e-Sword Topics and The Word Books.</li>
                            <li>Better text support for RVF based The Word modules.</li>
                            <li>Encrypting e-Sword modules during export.</li>
                            </ul>

                            <b>Release Notes - 1.10</b>
                            <ul>
                            <li>Support for BibleAnalyzer commentaries and dictionaries.</li>
                            </ul>

                            <b>Release Notes - 1.9</b>
                            <ul>
                            <li>Support for MySword for Android commentaries and dictionaries.</li>
                            </ul>

                            <b>Release Notes - 1.8</b>
                            <ul>
                            <li>Extended ASCII characters are displayed as squares - fixed</li>
                            </ul>

                            <b>Release Notes - 1.7</b>
                            <ul>
                            <li>You know a language to speak but doesn't know how to read or write? Don't worry. Transliteration support is now added.</li>
                            <li>Bible Statistics.</li>
                            <li>Unicode Bug introduced in 1.6 - Fixed.</li>
                            </ul>

                            <b>Release Notes - 1.6</b>
                            <ul>
                            <li>SWORD Dictionary - Bug fixed. Now supports compressed SWORD dictionaries.</li>
                            <li>Export parameters like abbrevation, description, comments does not work - fixed.</li>
                            </ul>

                            <b>Release Notes - 1.5</b>
                            <ul>
                            <li>Executable Bible now has same advanced features as Simple Bible Reader</li>
                            <li>Ability to automatically detect file format with support for drag-and-drop of any supported format irrespective of bibles, commentaries and dictionaries.</li>
                            <li>Supports SWORD Dictionaries.</li>
                            </ul>

                            <b>Release Notes - 1.4</b>
                            <ul>
                            <li>Layout changes along with advanced layout options to support commentary and dictionary. </li>
                            <li>Option to change fonts.</li>
                            <li>Paragraph view</li>
                            <li>Support for red-text and lexicon display for Zefania XML, e-Sword and The Word modules.</li>
                            <li>Support for XML, SWORD, e-Sword and The Word commentaries.</li>
                            <li>Support for Zefania, e-Sword and TheWord Dictionary modules.</li>
                            <li>Advanced search support with and, or, exact, match case and regular expressions.</li>
                            <li>Linking Bible verse to Commentaries and lexicon-dictionary.</li>
                            </ul>

                            <b>Release Notes - 1.3</b>
                            <ul><li>Compressed SWORD Bibles of OSIS SourceType doesn't open - Fixed.</li></ul>

                            <b>Release Notes - 1.2</b>
                            <ul><li>Support for The Word encrypted modules - .ontx, .otx and .ntx</li></ul>

                            <b>Release Notes - 1.1</b>
                            <ul><li>Support for MySword for Android bibles</li><li>Support for Bible Analyzer bibles</li></ul>

                            <b>Release Notes - 1.0</b>
                            <ul><li>Initial Release.</li><li>Supports nearly 20 bible formats.</li></ul>
";


        public static readonly string[][] LICENSES =  {
                // WORK, LINK, LINKTEXT, LICENSE
                 new string[] {
                    "SQLite",
                    "https://www.sqlite.org",
                    "D. Richard Hipp",
                    "Public Domain"},
                new string[] {
                    "SCOP implementation",
                    "https://web.archive.org/web/20040223193645/https://www.geocities.com/smaltchev/scop.html",
                    "Simeon Maltchev",
                    "Public Domain"},
                new string[] {
                    "DotNetZip",
                    "https://github.com/DinoChiesa/DotNetZip",
                    "Dino Chiesa",
                    "Microsoft Public License"},
                new string[] {
                    "Cross Connect",
                    "https://github.com/thomasdilts/cross-connect",
                    "Thomas Dilts",
                    "GNU General Public License v3.0"},
                new string[] {
                    "Palm Bible Plus",
                    "https://sourceforge.net/projects/palmbibleplus",
                    "Yih-Chun Hu (formerly Poetry Poon)",
                    "GNU General Public License"},
                new string[] {
                    "SharpZipLib",
                    "https://github.com/icsharpcode/SharpZipLib",
                    "John Reilly",
                    "MIT license"},
                new string[] {
                    "UnidecodeSharpFork",
                    "https://bitbucket.org/DimaStefantsov/unidecodesharpfork/",
                    "Oleg Ussanov",
                    "GNU General Public License v2"},
                new string[] {
                    "RtfPipe",
                    "https://github.com/erdomke/RtfPipe",
                    "Eric Domke",
                    "MIT license"},
                new string[] {
                    "SQLiteBulkInsert",
                    "https://github.com/jprichardson/CommonLib",
                    "JP Richardson",
                    "GNU Lesser General Public License"},
                new string[] {
                    "The SWORD Engine",
                    "https://crosswire.org/sword/develop/index.jsp",
                    "CrossWire Bible Society",
                    "GNU General Public License v2"},
                 new string[] {
                    ".NET DarkMode",
                    "https://github.com/ryanbester/dotnet-darkmode",
                    "Ryan Bester",
                    "MIT license"},
                 new string[] {
                    "DarkUI",
                    "https://github.com/RobinPerris/DarkUI",
                    "Robin Perris",
                    "MIT license"},

            };


       public static readonly string[][] ATTRIBUTIONS =  {
                // WORK, LINK, LINKTEXT
                new string[] {
                    "Front Image",
                    "https://clipartmag.com/cross-flower-clipart" ,
                    "Cross Flower Clipart" },
                new string[] {
                    "Bible Book Icon",
                    "https://www.flaticon.com/authors/konkapp" ,
                    "Konkapp - Flaticon" },
                new string[] {
                    "Toolbar Icons",
                    "https://www.flaticon.com/authors/smartline" ,
                    "Smartline - Flaticon" },
                new string[] {
                    "Toolbar Icons",
                    "https://www.flaticon.com/authors/freepik" ,
                    "Freepik - Flaticon" },
                new string[] {
                    "Toolbar Icons",
                    "https://www.flaticon.com/authors/peter-lakenbrink",
                    "Peter Lakenbrink - Flaticon" },
                new string[] {
                    "Toolbar Icons",
                    "https://www.flaticon.com/authors/ayub-irawan",
                    "Ayub Irawan - Flaticon" },
                new string[] {
                    "Toolbar Icons",
                    "https://www.flaticon.com/authors/darius-dan",
                    "Darius Dan - Flaticon" },
                new string[] {
                    "Chapter Headings",
                    "https://www.bibleocity.com",
                    "Bibleocity" }
            };

        public static readonly string[][] LOCALIZATIONS =  {
                // WORK, LINK, LINKTEXT
                new string[] {
                    "Spanish",
                    "",
                    "Julio Behr"},
                new string[] {
                    "Tamil",
                    "",
                    "Esther"},
                new string[] {
                    "Portuguese",
                    "",
                    "Rev. Fr. Paulo Giovanni Pereira" }
            };
    }
}
