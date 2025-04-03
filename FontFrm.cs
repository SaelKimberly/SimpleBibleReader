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
    public partial class FontFrm : Form
    {
        string _sampleText = "For God so loved the world, that he gave his only begotten Son, that whosoever believeth in him should not perish, but have everlasting life.";
        string fontFamily = "";

        public DialogResult RESULT = DialogResult.OK;

        public FontFrm(string sampleText,string default_fontFamily)
        {
            _sampleText = sampleText;
            if (_sampleText.Length > 150)
                _sampleText = _sampleText.Substring(0, 147)+"...";
            fontFamily = default_fontFamily;
            InitializeComponent();            
        }
        protected override void WndProc(ref Message m)
        {
            DarkMode.DarkMode.WndProc(this, m, GlobalMemory.getInstance().Theme);
            base.WndProc(ref m);
        }

        public string getFontFamily()
        {
            return fontFamily;
        }


        private void FontFrm_Load(object sender, EventArgs e)
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

            textBox1.Text = _sampleText;
            listBox1.Items.Clear();
            fontFamily = "";
            FontFamily[] ffArray = FontFamily.Families;
            foreach (FontFamily ff in ffArray)
            {
                if (ff.IsStyleAvailable(FontStyle.Regular))
                {
                    listBox1.Items.Add(ff.Name);
                }
            }
            if (listBox1.Items.Contains(fontFamily))
                listBox1.SelectedItem = fontFamily;

        }

        private void button1_Click(object sender, EventArgs e)
        {            
            RESULT = DialogResult.Cancel;
            this.Visible = false;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox l=(ListBox)sender;
            Font font=new Font( l.SelectedItem.ToString(),textBox1.Font.SizeInPoints);
            textBox1.Font = font;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
            {
                Themes.MessageBox("Please select a font to use. If you wish to close this dialog, click 'cancel'.", "Font");
            }
            else
                fontFamily = listBox1.SelectedItem.ToString();            
            RESULT = DialogResult.OK;
            this.Visible = false;
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            ListBox l = (ListBox)sender;
            Font font = new Font(l.SelectedItem.ToString(), textBox1.Font.SizeInPoints);
            textBox1.Font = font;
            RESULT = DialogResult.OK;
            this.Visible = false;
        }
    }
}
