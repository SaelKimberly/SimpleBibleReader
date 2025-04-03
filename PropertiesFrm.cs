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
    public partial class PropertiesFrm : Form
    {
        List<string[]> mKeyVal = new List<string[]>();
        public DialogResult RESULT = DialogResult.OK;
        TextBox[] txt = null;

        public PropertiesFrm()
        {
            InitializeComponent();
        }

        protected override void WndProc(ref Message m)
        {
            DarkMode.DarkMode.WndProc(this, m, GlobalMemory.getInstance().Theme);
            base.WndProc(ref m);
        }

        public void setProperties(List<string[]> kv)
        {
            mKeyVal = kv;
        }

        public List<string[]> getProperties()
        {
            return mKeyVal;
        }

        private void PropertiesFrm_Load(object sender, EventArgs e)
        {
            string key = null;
            string val = null;
            tableLayoutPanel1.Controls.Clear();
            //
            Label lbl1 = new Label();
            lbl1.Text = "Config Field";
            lbl1.TextAlign = ContentAlignment.MiddleCenter;
            lbl1.Font = new Font(lbl1.Font.FontFamily, lbl1.Font.SizeInPoints, FontStyle.Bold);
            
            Label lbl2 = new Label();
            lbl2.Text = "Config Value";
            lbl2.TextAlign = ContentAlignment.MiddleCenter;
            lbl2.Font = new Font(lbl2.Font.FontFamily, lbl2.Font.SizeInPoints, FontStyle.Bold);     
            
            tableLayoutPanel1.Controls.Add(lbl1);
            tableLayoutPanel1.Controls.Add(lbl2);
            //
            txt=new TextBox[mKeyVal.Count];
            int count=0;
            foreach (string[] kval in mKeyVal)
            {
                key = kval[0];
                val = kval[1];
                Label lbl = new Label();
                lbl.Text = key;
                lbl.Dock = DockStyle.Fill;
                txt[count] = new TextBox();
                txt[count].Text = val;
                txt[count].Dock = DockStyle.Fill;
                tableLayoutPanel1.Controls.Add(lbl);
                tableLayoutPanel1.Controls.Add(txt[count]) ;
                count++;
            }

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
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RESULT = DialogResult.Cancel;
            this.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < txt.Length;i++ )
            {
                mKeyVal[i][1] = txt[i].Text;
            }
            RESULT = DialogResult.OK;
            this.Visible = false;
        }
    }
}
