using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Windows.Forms;
using System.Speech.Synthesis;

namespace Simple_Bible_Reader
{
    class MP3BibleFormat : BibleFormat
    {
		int _book_no;

        public MP3BibleFormat(string file)
            : base(file)
		{
		}
		
		public override void Load()
		{
			// not supported!            
			//SetProcessAsComplete();
		}
		
		
		public override int PercentComplete
		{
			get
			{
				return ((_book_no) * 100) / Localization.getBookNames().Length;
			}
		}

        public override int FilterIndex
        {
            get
            {
                return F22_MP3;
            }
        }

        public override void ExportBible(string filename,int filter_idx)
        {            
            string path = this.FileName.Substring(0, this.FileName.LastIndexOf('.'));
            Directory.CreateDirectory(path);
            XmlNodeList books = BibleXmlDocument.SelectNodes("/XMLBIBLE/BIBLEBOOK");
            XmlNode vers = null;
            StringBuilder sb = new StringBuilder();
            string dir_book_path = null;
            string chapter_string = null;

            /*
            for (int b = 0; b < Localization.getBookNames().Length; b++)
            {
                _book_no = b;
                dir_book_path = path + "\\" + Localization.getBookNames()[_book_no];
                for (int c = 1; c <= FormatUtil.GetChapterCount2(FormatUtil.zefaniaIndexToBookNo(_book_no)); c++)
                {
                    sb.Length = 0;

                    for (int v = 1; v <= FormatUtil.GetVerseCount2(FormatUtil.zefaniaIndexToBookNo(_book_no), c); v++)
                    {
                        vers = BibleXmlDocument.SelectSingleNode("/XMLBIBLE/BIBLEBOOK[@bnumber='" + FormatUtil.zefaniaIndexToBookNo(_book_no).ToString() + "']/CHAPTER[@cnumber='" + c.ToString() + "']/VERS[@vnumber='" + v.ToString() + "']");
                        if (vers != null)
                        {
                            sb.Append(vers.InnerText);
                            sb.Append(" ");
                        }
                    }
                    //
                    if (sb.Length > 0)
                    {
                        chapter_string = Localization.getBookNames()[_book_no] + ", Chapter " + c.ToString() + ". ";
                        Directory.CreateDirectory(dir_book_path);
                        convertTextToMp3(chapter_string + sb.ToString(), dir_book_path + "\\Chapter " + c.ToString() + ".mp3");
                    }
                }           
            }
            */

            int chapter_no = -1;            
            foreach(XmlNode book in books)
            {
                _book_no = int.Parse(book.Attributes["bnumber"].Value);
                dir_book_path = path + "\\" + Localization.getBookNames()[FormatUtil.zefaniaBookNoToIndex(_book_no)];
                foreach (XmlNode chap in book.SelectNodes("CHAPTER"))
                {
                    sb.Length = 0;
                    chapter_no= int.Parse(chap.Attributes["cnumber"].Value);
                    foreach (XmlNode ver in chap.SelectNodes("VERS"))
                    {
                        sb.Append(ver.InnerText);
                        sb.Append(" ");
                    }
                    if (sb.Length > 0)
                    {
                        chapter_string = Localization.getBookNames()[FormatUtil.zefaniaBookNoToIndex(_book_no)] + ", Chapter " + chapter_no.ToString() + ". ";
                        Directory.CreateDirectory(dir_book_path);
                        convertTextToMp3(chapter_string + sb.ToString(), dir_book_path + "\\Chapter " + chapter_no.ToString() + ".mp3");
                    }
                }
            }

        }


        SpeechSynthesizer speechSynthesizerObj = new SpeechSynthesizer();
        private void convertTextToMp3(string text, string mp3_file)
        {
            speechSynthesizerObj = new SpeechSynthesizer();

            using (SpeechSynthesizer synth = new SpeechSynthesizer())
            {
                synth.SetOutputToWaveFile(mp3_file);
                PromptBuilder builder = new PromptBuilder();
                builder.AppendText(text);
                synth.Speak(builder);
            }
        }
    }
}
