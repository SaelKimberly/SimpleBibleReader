using DarkUI.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Simple_Bible_Reader
{
    public partial class Verse : Form
    {
        string verse_ref = null;

        public Verse(string _verse_ref)
        {
            InitializeComponent();
            verse_ref = _verse_ref;
            if (GlobalMemory.getInstance().Direction == GlobalMemory.DIRECTION_RTL)
            {
                this.RightToLeft = RightToLeft.Yes;
                VerseText.RightToLeft = RightToLeft.Yes;
            }

        }

        private void Verse_Load(object sender, EventArgs e)
        {
            if (GlobalMemory.getInstance().SelectedTheme.ToLower().Contains("dark"))
            {
                // change to dark mode.
                this.BackColor = Colors.GreyBackground;
                this.ForeColor = Colors.LightText;
                Themes.ChangeDarkMode(this.Controls);
            }
            else
            {
                this.BackColor = Form.DefaultBackColor;
                this.ForeColor = Form.DefaultForeColor;
                Themes.ChangeNormalMode(this.Controls);
            }

            this.Text = "Verse: " + verse_ref;
            VerseText.Select(0, 0);
        }

        protected override void WndProc(ref Message m)
        {
            DarkMode.DarkMode.WndProc(this, m, GlobalMemory.getInstance().Theme);
            base.WndProc(ref m);
        }

        public void setVerse(string verse)
        {
            // twice required sometimes
            var versestr = FormatUtil.UnescapeXML(verse);

            // unescape atleast twice
            versestr = FormatUtil.UnescapeXML(versestr);
            VerseText.Text=FormatUtil.UnescapeXML(versestr);
        }

        public string getVerse()
        {
            if(cbkXmlEscape.Checked)
                return FormatUtil.EscapeXML(VerseText.Text);
            else
                return VerseText.Text;
        }
    }
}
