using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using DarkUI.Config;
using System.IO;
using static System.Environment;
using System.Xml.Linq;
using System.Reflection;
using System.Linq;
using RtfPipe.Tokens;
using System.Collections;

namespace Simple_Bible_Reader
{
    public partial class PreferencesFrm : Form
    {
        public PreferencesFrm()
        {
            InitializeComponent();
        }

        protected override void WndProc(ref Message m)
        {
            DarkMode.DarkMode.WndProc(this, m, GlobalMemory.getInstance().Theme);
            base.WndProc(ref m);
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        private void OK_Click(object sender, EventArgs e)
        {
            GlobalMemory.getInstance().ParseBible = rbBibleParse.Checked;
            GlobalMemory.getInstance().parseBibleBkgndCleaner = cbBibleBkgndCleaning.Checked;
            GlobalMemory.getInstance().addRedText = cbBibleRedText.Checked;
            //
            GlobalMemory.getInstance().ParseBook = rbBookParse.Checked;
            GlobalMemory.getInstance().parseBookBkgndCleaner = cbBookBkgndCleaning.Checked;
            //
            GlobalMemory.getInstance().ParseCommentary = rbCmtryParse.Checked;
            //
            GlobalMemory.getInstance().ParseDictionary = rbDictParse.Checked;
            GlobalMemory.getInstance().CommentaryIcon = cbBibleCmtryIcon.Checked;

            GlobalMemory.getInstance().Mp3Volume = tbMp3Volume.Value;
            GlobalMemory.getInstance().Mp3Speed = tbMP3Speed.Value;

            GlobalMemory.getInstance().Mp3VoiceIndex = cbMp3Voices.SelectedIndex;
            GlobalMemory.getInstance().Mp3Transliterate = cbMp3Transliterate.Checked;

            //
            GlobalMemory.getInstance().ConvertRtfToHtmlBible = cbBibleRtfToHtml.Checked;
            GlobalMemory.getInstance().ConvertRtfToHtmlCommentary = cbCmtryRtfToHtml.Checked;
            GlobalMemory.getInstance().ConvertRtfToHtmlDictionary = cbDictRtfToHtml.Checked;
            GlobalMemory.getInstance().ConvertRtfToHtmlBook = cbBookRtfToHtml.Checked;

            GlobalMemory.getInstance().stripHtmlTagsBible = cbBibleStripHtml.Checked;
            GlobalMemory.getInstance().stripHtmlTagsBook = cbBookStripHtml.Checked;
            GlobalMemory.getInstance().stripHtmlTagsCommentary = cbCmtryStripHtml.Checked;
            GlobalMemory.getInstance().stripHtmlTagsDictionary = cbDictStripHtml.Checked;
            //
            GlobalMemory.getInstance().ConvertHtmlToRtfBible = cbBibleHtmlToRtf.Checked;
            GlobalMemory.getInstance().ConvertHtmlToRtfBook = cbBookHtmlToRtf.Checked;
            GlobalMemory.getInstance().ConvertHtmlToRtfCommentary = cbCmtryHtmlToRtf.Checked;
            GlobalMemory.getInstance().ConvertHtmlToRtfDictionary = cbDictHtmlToRtf.Checked;

            GlobalMemory.getInstance().autoSetParsingBible = cbAutoSetBible.Checked;
            GlobalMemory.getInstance().autoSetParsingBook = cbAutoSetBook.Checked;
            GlobalMemory.getInstance().autoSetParsingCommentary = cbAutoSetCmtry.Checked;
            GlobalMemory.getInstance().autoSetParsingDictionary = cbAutoSetDict.Checked;

            GlobalMemory.getInstance().Versification = cbVersification.SelectedItem.ToString();

            GlobalMemory.getInstance().EditBibleVerse = cbEditBibleVerse.Checked;
            GlobalMemory.getInstance().SaveLoadFilePref = cbSaveLoadFilePref.Checked;

            GlobalMemory.getInstance().DisplayBIBRaw = cbDisplayBIBRawView.Checked;
            GlobalMemory.getInstance().DisplayBOKRaw = cbDisplayBookRaw.Checked;
            GlobalMemory.getInstance().DisplayCMTRaw = cbDisplayCmtryRaw.Checked;
            GlobalMemory.getInstance().DisplayDCTRaw = cbDisplayDictRaw.Checked;

            GlobalMemory.getInstance().use_libsword = rbCrossWireLib.Checked;
            GlobalMemory.getInstance().UseCtrlForHtmlRtfConv=cbUseCtrlHtmlRtfConv.Checked;

            if (cbUseRenderer.Checked)
            {
                GlobalMemory.getInstance().useCustomRenderer = cbUseRenderer.Checked;
                switch (cbHtmlRenderer.SelectedIndex)
                {
                    case 0:
                        GlobalMemory.getInstance().useRenderer = "WEBVIEW2";
                        break;
                    case 1:
                        GlobalMemory.getInstance().useRenderer = "WEBCONTROL";
                        break;                    
                    default:
                        GlobalMemory.getInstance().useRenderer = "WEBCONTROL";
                        break;
                }
            }



            /////////////////////////////////
            //            
            if (GlobalMemory.getInstance().SaveLoadFilePref)
                GlobalMemory.savePrefToFile();
            else
                GlobalMemory.deletePrefFile();

            this.Visible = false;
        }
        

        private void PreferencesFrm_Load(object sender, EventArgs e)
        {
            // loaded already in Main FormLoad() but reloaded here just in case.
            GlobalMemory.loadPrefFromFile();

            rbBibleParse.Checked = GlobalMemory.getInstance().ParseBible;
            cbBibleBkgndCleaning.Checked = GlobalMemory.getInstance().parseBibleBkgndCleaner;
            rbBookParse.Checked = GlobalMemory.getInstance().ParseBook;
            cbBookBkgndCleaning.Checked = GlobalMemory.getInstance().parseBookBkgndCleaner;
            cbBibleRedText.Checked = GlobalMemory.getInstance().addRedText;
            rbCmtryParse.Checked = GlobalMemory.getInstance().ParseCommentary;
            rbDictParse.Checked = GlobalMemory.getInstance().ParseDictionary;
            cbBibleCmtryIcon.Checked = GlobalMemory.getInstance().CommentaryIcon;

            rbCrossConnectLib.Checked = !GlobalMemory.getInstance().use_libsword;
            rbCrossWireLib.Checked = GlobalMemory.getInstance().use_libsword;

            tbMp3Volume.Value = GlobalMemory.getInstance().Mp3Volume;
            tbMP3Speed.Value = GlobalMemory.getInstance().Mp3Speed;

            cbBibleRtfToHtml.Checked = GlobalMemory.getInstance().ConvertRtfToHtmlBible;
            cbCmtryRtfToHtml.Checked = GlobalMemory.getInstance().ConvertRtfToHtmlCommentary;
            cbDictRtfToHtml.Checked = GlobalMemory.getInstance().ConvertRtfToHtmlDictionary;
            cbBookRtfToHtml.Checked = GlobalMemory.getInstance().ConvertRtfToHtmlBook;

            cbBibleStripHtml.Checked = GlobalMemory.getInstance().stripHtmlTagsBible;
            cbBookStripHtml.Checked = GlobalMemory.getInstance().stripHtmlTagsBook;
            cbCmtryStripHtml.Checked = GlobalMemory.getInstance().stripHtmlTagsCommentary;
            cbDictStripHtml.Checked = GlobalMemory.getInstance().stripHtmlTagsDictionary;

            cbBibleHtmlToRtf.Checked = GlobalMemory.getInstance().ConvertHtmlToRtfBible;
            cbBookHtmlToRtf.Checked = GlobalMemory.getInstance().ConvertHtmlToRtfBook;
            cbCmtryHtmlToRtf.Checked = GlobalMemory.getInstance().ConvertHtmlToRtfCommentary;
            cbDictHtmlToRtf.Checked = GlobalMemory.getInstance().ConvertHtmlToRtfDictionary;
            //
            cbAutoSetBible.Checked = GlobalMemory.getInstance().autoSetParsingBible;
            cbAutoSetBook.Checked = GlobalMemory.getInstance().autoSetParsingBook;
            cbAutoSetCmtry.Checked = GlobalMemory.getInstance().autoSetParsingCommentary;
            cbAutoSetDict.Checked = GlobalMemory.getInstance().autoSetParsingDictionary;
            //
            try
            {
                SpeechLib.SpVoice oVoice = new SpeechLib.SpVoice();

                SpeechLib.ISpeechObjectTokens tokens = oVoice.GetVoices();
                cbMp3Voices.Items.Clear();
                foreach (SpeechLib.ISpeechObjectToken token in tokens)
                {
                    cbMp3Voices.Items.Add(token.GetDescription());
                }
                cbMp3Voices.SelectedIndex = GlobalMemory.getInstance().Mp3VoiceIndex;
                cbMp3Transliterate.Checked = GlobalMemory.getInstance().Mp3Transliterate;
            }
            catch(Exception ex)
            {
                // may be running in wine, ignore error.
            }


            cbVersification.Enabled = true;
            cbVersification.Items.Clear();
            cbVersification.Items.AddRange(SwordVersification.getVersifications());
            cbVersification.SelectedItem = GlobalMemory.getInstance().Versification;

            cbDisplayBIBRawView.Checked = GlobalMemory.getInstance().DisplayBIBRaw;
            cbDisplayBookRaw.Checked = GlobalMemory.getInstance().DisplayBOKRaw;
            cbDisplayCmtryRaw.Checked = GlobalMemory.getInstance().DisplayCMTRaw;
            cbDisplayDictRaw.Checked = GlobalMemory.getInstance().DisplayDCTRaw;

            cbEditBibleVerse.Checked = GlobalMemory.getInstance().EditBibleVerse;
            cbSaveLoadFilePref.Checked = GlobalMemory.getInstance().SaveLoadFilePref;

            rbCrossWireLib.Checked = GlobalMemory.getInstance().use_libsword;
            rbCrossConnectLib.Checked = !rbCrossWireLib.Checked;
            cbUseCtrlHtmlRtfConv.Checked = GlobalMemory.getInstance().UseCtrlForHtmlRtfConv;

            if (Win32NativeCalls.isRunningOnWine())
            {
                cbUseRenderer.Checked = true;
                cbHtmlRenderer.SelectedIndex = 1;
                cbUseRenderer.Enabled = false;
                cbHtmlRenderer.Enabled = false;
            }
            else
            {
                cbUseRenderer.Checked = GlobalMemory.getInstance().useCustomRenderer;
                if (cbUseRenderer.Checked)
                {
                    switch (GlobalMemory.getInstance().useRenderer)
                    {
                        case "WEBVIEW2":
                            cbHtmlRenderer.SelectedIndex = 0;
                            break;
                        case "WEBCONTROL":
                            cbHtmlRenderer.SelectedIndex = 1;
                            break;
                        default:
                            cbHtmlRenderer.SelectedIndex = 0;
                            break;
                    }
                }
            }

            if(Win32NativeCalls.isRunningOnWine())
            {
                // not supported, may be running under wine.
                TabPage tabPage = this.tabControl1.TabPages["tabMP3"];
                if (tabPage != null)
                    this.tabControl1.TabPages.Remove(tabPage);
            }

            if (GlobalMemory.getInstance().SelectedTheme.ToLower().Contains("dark"))
            {
                // change to dark mode.
                this.BackColor = Colors.GreyBackground;
                this.ForeColor = Colors.LightText;
                Themes.ChangeDarkMode(this.Controls);

                this.tabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;
                this.tabControl1.DrawItem += new DrawItemEventHandler(this.tabControl1_DrawItem);

            }
            else
            {
                this.BackColor = Form.DefaultBackColor;
                this.ForeColor = Form.DefaultForeColor;
                Themes.ChangeNormalMode(this.Controls);
            }
        }

        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            //e.DrawBackground();
            using (Brush br = new SolidBrush(Colors.GreyBackground))
            {
                e.Graphics.FillRectangle(br, e.Bounds);
                SizeF sz = e.Graphics.MeasureString(tabControl1.TabPages[e.Index].Text, e.Font);
                e.Graphics.DrawString(tabControl1.TabPages[e.Index].Text, e.Font, new SolidBrush(Colors.LightText), e.Bounds.Left + (e.Bounds.Width - sz.Width) / 2, e.Bounds.Top + (e.Bounds.Height - sz.Height) / 2 + 1);

                Rectangle rect = e.Bounds;
                rect.Offset(0, 1);
                rect.Inflate(0, -1);
                e.Graphics.DrawRectangle(Pens.DarkGray, rect);
                e.DrawFocusRectangle();
            }
        }

        private void rbBibleParse_CheckedChanged(object sender, EventArgs e)
        {
            cbBibleBkgndCleaning.Enabled = true;
            cbBibleBkgndCleaning.Checked = true;
            cbAutoSetBible.Checked = true;
            cbAutoSetBible.Enabled = true;
            rbBibleRaw.Checked = !rbBibleParse.Checked;


        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                SpeechLib.SpVoice oVoice = new SpeechLib.SpVoice();
                oVoice.Volume = tbMp3Volume.Value;
                oVoice.Rate = tbMP3Speed.Value;
                oVoice.Voice = oVoice.GetVoices().Item(cbMp3Voices.SelectedIndex);
                if (cbMp3Transliterate.Checked)
                    oVoice.Speak(Unidecoder.Unidecode(tbMp3Text.Text), SpeechLib.SpeechVoiceSpeakFlags.SVSFDefault);
                else
                    oVoice.Speak(tbMp3Text.Text, SpeechLib.SpeechVoiceSpeakFlags.SVSFDefault);
                oVoice = null;
            }
            catch {
                Themes.MessageBox("Text to speech is not supported.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Themes.MessageBox("There are many third-party high quality voices.You can install them seperately based on your needs.", "Voices", MessageBoxButtons.YesNo, MessageBoxIcon.Question);           
        }

        private void rbCrossWireLib_CheckedChanged(object sender, EventArgs e)
        {            
            cbVersification.Enabled = true;
        }

        private void rbCrossConnectLib_CheckedChanged(object sender, EventArgs e)
        {
            cbVersification.Enabled = false;
        }

        private void rbBookRaw_CheckedChanged(object sender, EventArgs e)
        {
            cbAutoSetBook.Checked = false;
            cbBookBkgndCleaning.Checked = false;
            cbBookRtfToHtml.Checked = false;
            cbBookHtmlToRtf.Checked= false;
            cbBookStripHtml.Checked = false;


            cbBookBkgndCleaning.Enabled = false;
            cbBookRtfToHtml.Enabled = false;
            cbBookHtmlToRtf.Enabled = false;
            cbBookStripHtml.Enabled = false;
            cbAutoSetBook.Enabled = false;
        }

        private void rbBookParse_CheckedChanged(object sender, EventArgs e)
        {
            cbBookBkgndCleaning.Enabled = true;
            cbBookBkgndCleaning.Checked = true;
            cbAutoSetBook.Checked = true;
            cbAutoSetBook.Enabled = true;
            rbBookRaw.Checked = !rbBookParse.Checked;
        }

        private void rbBibleRaw_CheckedChanged(object sender, EventArgs e)
        {
            cbAutoSetBible.Checked = false;
            cbBibleBkgndCleaning.Checked = false;
            cbBibleRtfToHtml.Checked = false;
            cbBibleHtmlToRtf.Checked = false;
            cbBibleStripHtml.Checked = false;

            cbAutoSetBible.Enabled = false;
            cbBibleBkgndCleaning.Enabled = false;
            cbBibleRtfToHtml.Enabled = false;
            cbBibleHtmlToRtf.Enabled = false;
            cbBibleStripHtml.Enabled = false;



        }

        private void rbCmtryParse_CheckedChanged(object sender, EventArgs e)
        {
            cbAutoSetCmtry.Checked = true;
            cbAutoSetCmtry.Enabled = true;
            rbCmtryRaw.Checked = !rbCmtryParse.Checked;
        }

        private void rbCmtryRaw_CheckedChanged(object sender, EventArgs e)
        {
            cbAutoSetCmtry.Checked = false;
            cbCmtryRtfToHtml.Checked = false;
            cbCmtryHtmlToRtf.Checked = false;
            cbCmtryStripHtml.Checked = false;
            

            cbCmtryRtfToHtml.Enabled = false;
            cbCmtryHtmlToRtf.Enabled = false;
            cbCmtryStripHtml.Enabled = false;
            cbAutoSetCmtry.Enabled = false;
        }

        private void rbDictParse_CheckedChanged(object sender, EventArgs e)
        {
            cbAutoSetDict.Checked = true;
            cbAutoSetDict.Enabled = true;
            rbDictRaw.Checked = !rbDictParse.Checked;
        }

        private void rbDictRaw_CheckedChanged(object sender, EventArgs e)
        {
            cbAutoSetDict.Checked = false;
            cbDictRtfToHtml.Checked = false;
            cbDictHtmlToRtf.Checked = false;
            cbDictStripHtml.Checked = false;
            

            cbDictRtfToHtml.Enabled = false;
            cbDictHtmlToRtf.Enabled = false;
            cbDictStripHtml.Enabled = false;
            cbAutoSetDict.Enabled = false;

        }

        private void cbAutoSetBible_CheckedChanged(object sender, EventArgs e)
        {
            cbBibleHtmlToRtf.Enabled = !cbAutoSetBible.Checked;
            cbBibleRtfToHtml.Enabled = !cbAutoSetBible.Checked;
            cbBibleStripHtml.Enabled = !cbAutoSetBible.Checked;
        }

        private void cbAutoSetBook_CheckedChanged(object sender, EventArgs e)
        {
            cbBookHtmlToRtf.Enabled = !cbAutoSetBook.Checked;
            cbBookRtfToHtml.Enabled = !cbAutoSetBook.Checked;
            cbBookStripHtml.Enabled = !cbAutoSetBook.Checked;
        }

        private void cbAutoSetCmtry_CheckedChanged(object sender, EventArgs e)
        {
            cbCmtryHtmlToRtf.Enabled = !cbAutoSetCmtry.Checked;
            cbCmtryRtfToHtml.Enabled = !cbAutoSetCmtry.Checked;
            cbCmtryStripHtml.Enabled = !cbAutoSetCmtry.Checked;
        }

        private void cbAutoSetDict_CheckedChanged(object sender, EventArgs e)
        {
            cbDictHtmlToRtf.Enabled = !cbAutoSetDict.Checked;
            cbDictRtfToHtml.Enabled = !cbAutoSetDict.Checked;
            cbDictStripHtml.Enabled = !cbAutoSetDict.Checked;
        }

        private void cbUseRenderer_CheckedChanged(object sender, EventArgs e)
        {
            cbHtmlRenderer.Enabled = cbUseRenderer.Checked;
            if (cbUseRenderer.Checked)
            {
                cbHtmlRenderer.SelectedIndex = 0;
            }
            else
                cbHtmlRenderer.SelectedIndex = -1;
        }
    }
}
