using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Collections;
using DarkUI.Config;

namespace Simple_Bible_Reader
{
    public partial class PluginProcessFrm : Form
    {
        bool cancelled = false;

        public const int TYPE_BIBL = 0;
        public const int TYPE_CMTY = 1;
        public const int TYPE_DICT = 2;
        public const int TYPE_BOOK = 3;

        System.Xml.XmlDocument xmlDoc;
        int pluginType;

        int mMax = 100;
        int mCurr = 0;

        public PluginProcessFrm(System.Xml.XmlDocument m_xmlDoc, int m_pluginType)
        {
            InitializeComponent();
            xmlDoc = m_xmlDoc;
            pluginType = m_pluginType;
        }

        protected override void WndProc(ref Message m)
        {
            DarkMode.DarkMode.WndProc(this, m, GlobalMemory.getInstance().Theme);
            base.WndProc(ref m);
        }

        private void PluginProcessFrm_Load(object sender, EventArgs e)
        {
            cancelled = false;

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

            pbPlugin.Minimum = 0;
            bkgndPluginWorker.RunWorkerAsync();
        }

        public void set_ProgressBarMax(int max)
        {
            mMax = max;
        }
        public void set_ProgressBarCurrent(int curr)
        {
            mCurr = curr;
        }

        public bool isCancelled()
        {
            return cancelled;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {            
            btnCancel.Enabled = false;
            cancelled = true;
        }

        private void bkgndPluginWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (GlobalMemory.getInstance().EnablePlugins)
            {
                Random rand = new Random(DateTime.Now.TimeOfDay.Milliseconds);
                string inFile, outFile, tmpPath;
                tmpPath = Path.GetTempPath();
                if (!tmpPath.EndsWith("\\"))
                    tmpPath = tmpPath + "\\";
                XmlNodeList leaves = null;
                // background cleaner is only for Bible and Books. cleaning on dictionary and commentary takes place during load.
                if (pluginType == TYPE_BIBL)
                    leaves = BibleFormat.getInstance().getXmlDoc().SelectNodes("//VERS");
                else if (pluginType == TYPE_CMTY)
                    leaves = CommentaryFormat.getInstance().getXmlDoc().SelectNodes("//VERS");
                else if (pluginType == TYPE_BOOK)
                    leaves = BookFormat.getInstance().getXmlDoc().SelectNodes("//NOTES");
                else if (pluginType == TYPE_DICT)
                    leaves = DictionaryFormat.getInstance().getXmlDoc().SelectNodes("//item");
                if (leaves == null)
                    return;

                int count = 0;
                set_ProgressBarMax(leaves.Count);

                string outData = null;
                string inData = null;

                List<string> pList = GlobalMemory.getInstance().pluginList;                
                if (pList.Count > 0)
                {
                    foreach (XmlNode leaf in leaves)
                    {
                        set_ProgressBarCurrent(count);

                        inData = FormatUtil.UnescapeXML(leaf.InnerXml);
                        // give another try to completely unescape.
                        inData = FormatUtil.UnescapeXML(inData);

                        for (int i = 0; i < pList.Count; i += 2)
                        {
                            if (isCancelled())
                                break;

                            outData = Regex.Replace(inData, pList[i], pList[i + 1]);
                            inData = outData;
                        }

                        if(GlobalMemory.getInstance().pluginEscapeXML)
                            outData = FormatUtil.EscapeXML(outData);
                        else
                            outData = FormatUtil.UnescapeXML(outData);
                        try
                        {
                            leaf.InnerXml = outData;
                        }
                        catch (Exception)
                        {
                            leaf.InnerText = outData;
                        }

                        if (isCancelled())
                            break;
                        count++;
                        bkgndPluginWorker.ReportProgress((count * 100) / leaves.Count);
                    }
                }
            }
        }

        private void bkgndPluginWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Dispose();
        }

        private void bkgndPluginWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbPlugin.Maximum = mMax;
            pbPlugin.Value = mCurr;
            lblProgress.Text = mCurr.ToString() + " / " + pbPlugin.Maximum.ToString();
            this.Text = "Plugin Progress - " + e.ProgressPercentage.ToString()+"%";
        }
    }
}
