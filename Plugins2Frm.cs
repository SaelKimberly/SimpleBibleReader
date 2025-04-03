using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using DarkUI.Config;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using static System.Windows.Forms.LinkLabel;
using System.Collections;
using System.IO;

namespace Simple_Bible_Reader
{
    public partial class Plugins2Frm : Form
    {
        OpenFileDialog ofd = null;
        SaveFileDialog sfd = null;

        public Plugins2Frm()
        {
            InitializeComponent();
        }

        protected override void WndProc(ref Message m)
        {
            DarkMode.DarkMode.WndProc(this, m, GlobalMemory.getInstance().Theme);
            base.WndProc(ref m);
        }

        private void cbEnablePlugins_CheckedChanged(object sender, EventArgs e)
        {
            GlobalMemory.getInstance().EnablePlugins = cbEnablePlugins.Checked;
            if (GlobalMemory.getInstance().useRenderer == "WEBVIEW2")
                RendererWebView2.getInstance().pluginsToolStripMenuItem.Checked = cbEnablePlugins.Checked;
            else if(GlobalMemory.getInstance().useRenderer == "WEBCONTROL")
                RendererWebBrowserControl.getInstance().pluginsToolStripMenuItem.Checked = cbEnablePlugins.Checked;

            tbInput.Enabled = cbEnablePlugins.Checked;
            tbOutput.Enabled=cbEnablePlugins.Checked;
            tbPlugin.Enabled = cbEnablePlugins.Checked;
            btnLoadPlugin.Enabled = cbEnablePlugins.Checked;
            btnTestRun.Enabled = cbEnablePlugins.Checked;
            cbEscapeXML.Enabled = cbEnablePlugins.Checked;
            btnClear.Enabled = cbEnablePlugins.Checked;
            btnSavePlugin.Enabled = cbEnablePlugins.Checked;

            if (getCurrentRendererVerseRawText() != "" && cbEnablePlugins.Checked == true)
                btnLoadCurrentVerse.Enabled = true;
            else
                btnLoadCurrentVerse.Enabled = false;
        }

        private void PluginsFrm_Load(object sender, EventArgs e)
        {
            cbEnablePlugins.Checked = GlobalMemory.getInstance().EnablePlugins;

            if (GlobalMemory.getInstance().SelectedTheme.ToLower().Contains("dark"))
            {
                // change to dark mode.
                this.BackColor = Colors.GreyBackground;
                this.ForeColor = Colors.LightText;
                Themes.ChangeDarkMode(this.Controls);
                //
            }
            else
            {
                this.BackColor = Form.DefaultBackColor;
                this.ForeColor = Form.DefaultForeColor;
                Themes.ChangeNormalMode(this.Controls);
                //
            }

            if (ofd == null)
            {
                ofd = new OpenFileDialog();
                ofd.Filter = "Regex Plugin|*.rxp|All files|*.*";
            }

            if(getCurrentRendererVerseRawText()!="" && cbEnablePlugins.Checked == true)
                btnLoadCurrentVerse.Enabled = true;
            else
                btnLoadCurrentVerse.Enabled = false;

            tbPlugin.Text = GlobalMemory.getInstance().pluginRxp;
            cbEscapeXML.Checked = GlobalMemory.getInstance().pluginEscapeXML;
        }

        private string getCurrentRendererVerseRawText()
        {
            if (GlobalMemory.getInstance().useRenderer == "WEBVIEW2")
                return RendererWebView2.getInstance().getCurrentVerseRawText();
            else if (GlobalMemory.getInstance().useRenderer == "WEBCONTROL")
                return RendererWebBrowserControl.getInstance().getCurrentVerseRawText();
            else
                return "";
        }

        private void PluginsFrm_FormClosing(object sender, FormClosingEventArgs e)
        {            
            GlobalMemory.getInstance().pluginRxp = tbPlugin.Text;
            GlobalMemory.getInstance().pluginEscapeXML = cbEscapeXML.Checked;

            List<string> pList = new List<string>();
            foreach (string line in tbPlugin.Text.Split(Environment.NewLine.ToCharArray()))
            {

                if (line.Trim() != "" && !line.Trim().StartsWith("#"))
                    pList.Add(line);
            }

            GlobalMemory.getInstance().pluginList = pList;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Process.Start("https://trumpet-call.org/sbr-plugins/");
        }

        private void btnTestRun_Click(object sender, EventArgs e)
        {
            string inData = tbInput.Text;
            string pluginData = tbPlugin.Text;
            string outData = "";
            List<string> pList = new List<string>();

            foreach (string line in pluginData.Split(Environment.NewLine.ToCharArray()))
            {
                
                if (line.Trim() != "" && !line.Trim().StartsWith("#"))
                    pList.Add(line);
            }

            for (int i = 0; i < pList.Count; i += 2)
            {
                outData = Regex.Replace(inData, pList[i], pList[i + 1]);
                inData = outData;
            }

            if(cbEscapeXML.Checked)
                tbOutput.Text = FormatUtil.EscapeXML(outData);
            else
                tbOutput.Text = outData;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            tbOutput.Clear();
        }

        private void btnLoadPlugin_Click(object sender, EventArgs e)
        {
            ofd = new OpenFileDialog();
            ofd.Filter = "Regex Plugin|*.rxp|Text Files|*.txt|All files|*.*";
            DialogResult result = ofd.ShowDialog();
            if(result == DialogResult.OK)
            {
                tbPlugin.Text = File.ReadAllText(ofd.FileName);
            }
            ofd.Dispose();
        }

        private void btnLoadCurrentVerse_Click(object sender, EventArgs e)
        {
            tbInput.Text = getCurrentRendererVerseRawText();
        }

        private void btnSavePlugin_Click(object sender, EventArgs e)
        {
            sfd = new SaveFileDialog();
            sfd.Filter = "Regex Plugin|*.rxp|Text Files|*.txt|All files|*.*";
            DialogResult result = sfd.ShowDialog();
            if (result == DialogResult.OK)
            {
                File.WriteAllText(sfd.FileName, tbPlugin.Text);
            }
            sfd.Dispose();
        }
    }
}
