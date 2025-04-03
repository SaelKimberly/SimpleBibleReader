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
    public partial class Comment : Form
    {
        string verse_ref = null;

        public Comment(string _verse_ref)
        {
            InitializeComponent();
            verse_ref = _verse_ref;
            if (GlobalMemory.getInstance().Direction == GlobalMemory.DIRECTION_RTL)
            {
                this.RightToLeft = RightToLeft.Yes;
                CommentText.RightToLeft = RightToLeft.Yes;
            }

        }

        private void Comment_Load(object sender, EventArgs e)
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

            this.Text = "Comment for " + verse_ref;
            CommentText.Select(0, 0);
        }

        protected override void WndProc(ref Message m)
        {
            DarkMode.DarkMode.WndProc(this, m, GlobalMemory.getInstance().Theme);
            base.WndProc(ref m);
        }

        public void setComment(string comment)
        {
            // twice required sometimes
            var commentstr = FormatUtil.UnescapeXML(comment);

            // unescape atleast twice
            commentstr = FormatUtil.UnescapeXML(commentstr);
            CommentText.Text=FormatUtil.UnescapeXML(commentstr);
        }

        public string getComment()
        {
            return FormatUtil.EscapeXML(CommentText.Text);
        }
    }
}
