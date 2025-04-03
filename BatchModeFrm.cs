using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Threading;
using DarkUI.Config;

namespace Simple_Bible_Reader
{
    public partial class BatchModeFrm : Form
    {
        FolderBrowserDialog srcFolder = null;
        FolderBrowserDialog tgtFolder = null;

        String sourceFolder = null;
        String targetFolder = null;

        int srcFilterIdx = 0;
        int tgtFilterIdx = 0;

        int typeIdx = 0;

        String currentFile;
        int progress;
        bool cancelled = false;

        public BatchModeFrm()
        {
            InitializeComponent();
        }

        protected override void WndProc(ref Message m)
        {
            DarkMode.DarkMode.WndProc(this, m, GlobalMemory.getInstance().Theme);
            base.WndProc(ref m);
        }

        private void btnSrcBtn_Click(object sender, EventArgs e)
        {
            if(srcFolder==null)
                srcFolder = new FolderBrowserDialog();
            if (srcFolder.ShowDialog(this) == DialogResult.OK)
                txtSrcFolder.Text = srcFolder.SelectedPath;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (tgtFolder==null)
                tgtFolder = new FolderBrowserDialog();
            if (tgtFolder.ShowDialog(this) == DialogResult.OK)
                txtTgtFolder.Text = tgtFolder.SelectedPath;
        }

        private void BatchModeFrm_Load(object sender, EventArgs e)
        {
            cbxSrcType.SelectedIndex = 0;

            // check if dark mode.
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

        private void cbxSrcType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cbx = (ComboBox)sender;
            string[] array = null;
            cbxSrcFormat.Items.Clear();
            cbxTgtFormat.Items.Clear();

            switch (cbx.SelectedIndex)
            {
                case 0:
                    array = BibleFormat.getFilters().Split(new char[] { '|' });
                    break;
                case 1:
                    array = CommentaryFormat.getFilters().Split(new char[] { '|' });
                    break;
                case 2:
                    array = DictionaryFormat.getFilters().Split(new char[] { '|' });
                    break;
                case 3:
                    array = BookFormat.getFilters().Split(new char[] { '|' });
                    break;               
                default:
                    return;
            }

            for (int i = 0; i < array.Length; i += 2)
            {
                cbxSrcFormat.Items.Add(array[i] + " (" + array[i + 1] + ")");
                cbxTgtFormat.Items.Add(array[i] + " (" + array[i + 1] + ")");
            }
            cbxSrcFormat.SelectedIndex = 0;
            cbxTgtFormat.SelectedIndex = 0;

            typeIdx = cbx.SelectedIndex;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(txtSrcFolder.Text))
            {
                Themes.MessageBox(Localization.BatchConversion_SourceFolderDoesNotExist, Localization.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!Directory.Exists(txtTgtFolder.Text))
            {
                Themes.MessageBox(Localization.BatchConversion_TargetFolderDoesNotExist, Localization.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if(cbxSrcType.SelectedIndex<0)
            {
                Themes.MessageBox("Please select the type of conversion", Localization.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (cbxSrcFormat.SelectedIndex < 0)
            {
                Themes.MessageBox("Please select the source format", Localization.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (cbxTgtFormat.SelectedIndex < 0)
            {
                Themes.MessageBox("Please select the target format", Localization.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            /////
            btnConvert.Enabled = false;
            btnCancel.Enabled = true;
            ///////////
            sourceFolder = txtSrcFolder.Text;
            targetFolder = txtTgtFolder.Text;

            progressStatus.Visible = true;
            txtCurrentFile.Visible = true;

            srcFilterIdx = cbxSrcFormat.SelectedIndex;
            tgtFilterIdx = cbxTgtFormat.SelectedIndex;

            if (batchExportWorker.IsBusy)
                batchExportWorker.CancelAsync();

            batchExportWorker.RunWorkerAsync();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            btnCancel.Enabled = false;
            btnConvert.Enabled = true;
            progressStatus.Visible = false;
            txtCurrentFile.Visible = false;
            cancelled = true;
            batchExportWorker.CancelAsync();
        }

        private void batchExportWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string[] files = Directory.GetFiles(sourceFolder);

                for(int i=0;i<files.Length;i++)
                {
                    try
                    {
                        if (cancelled)
                            break;
                        currentFile = Path.GetFileName(files[i]);
                        progress = (i * 100) / files.Length;
                        batchExportWorker.ReportProgress(progress);
                        //////////                        
                        switch (typeIdx)
                        {
                            case 0:
                                BibleFormat.BibleFactory(files[i]);
                                BibleFormat.getInstance().Load();


                                if (BibleFormat.getInstance().CLEANING && GlobalMemory.getInstance().ParseBible)
                                {
                                    XmlNodeList verses;
                                    verses = BibleFormat.getInstance().getXmlDoc().SelectNodes("//VERS");

                                    if (verses == null)
                                        break;
                                    currentFile = "Cleaning ... "+Path.GetFileName(files[i]);
                                    batchExportWorker.ReportProgress(progress);
                                    foreach (XmlNode verse in verses)
                                    {
                                        if (cancelled)
                                            break;
                                        try
                                        {
                                            verse.InnerXml = FormatUtil.convertRtfToHtml(verse.InnerText);
                                        }
                                        catch (Exception)
                                        {
                                            //Console.WriteLine(ex.Message);                   
                                        }
                                    }
                                    currentFile = "Converting ... " + Path.GetFileName(files[i]);
                                    batchExportWorker.ReportProgress(progress);
                                }
                                if (cancelled)
                                    break;
                                BibleFormat bible_format_export = BibleFormat.doExportBible(files[i], tgtFilterIdx + 1);
                                if (bible_format_export == null)
                                {
                                    if (Themes.MessageBox(Localization.BatchConversion_ErrorConverting + " " + Path.GetFileName(files[i]) + " ... " + Localization.BatchConversion_Continue, Localization.Error, MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No)
                                        break;
                                }
                                bible_format_export.ExportBible(targetFolder + "\\" + Path.GetFileNameWithoutExtension(files[i]), tgtFilterIdx + 1);
                                break;
                            case 1:
                                CommentaryFormat.CommentaryFactory(files[i]);
                                CommentaryFormat.getInstance().Load(); // cleaning automatically happens in load for commentary and dictionary
                                if (cancelled)
                                    break;
                                CommentaryFormat cmtry_format_export = CommentaryFormat.doExportCommentary(files[i], tgtFilterIdx + 1);
                                if (cmtry_format_export == null)
                                {
                                    if (Themes.MessageBox(Localization.BatchConversion_ErrorConverting + " " + Path.GetFileName(files[i]) + " ... " + Localization.BatchConversion_Continue, Localization.Error, MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No)
                                        break;
                                }
                                cmtry_format_export.ExportCommentary(targetFolder + "\\" + Path.GetFileNameWithoutExtension(files[i]), tgtFilterIdx + 1);
                                break;
                            case 2:
                                DictionaryFormat.DictionaryFactory(files[i]);
                                DictionaryFormat.getInstance().Load(); // cleaning automatically happens in load for commentary and dictionary
                                if (cancelled)
                                    break;
                                DictionaryFormat dict_format_export = DictionaryFormat.doExportDictionary(files[i], tgtFilterIdx + 1);
                                if (dict_format_export == null)
                                {
                                    if (Themes.MessageBox(Localization.BatchConversion_ErrorConverting + " " + Path.GetFileName(files[i]) + " ... " + Localization.BatchConversion_Continue, Localization.Error, MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No)
                                        break;
                                }
                                dict_format_export.ExportDictionary(targetFolder + "\\" + Path.GetFileNameWithoutExtension(files[i]), tgtFilterIdx + 1);
                                break;
                            case 3:
                                BookFormat.BookFactory(files[i]);
                                BookFormat.getInstance().Load();

                                if (BookFormat.getInstance().CLEANING && GlobalMemory.getInstance().ParseBook)
                                {
                                    XmlNodeList verses;
                                    verses = BookFormat.getInstance().getXmlDoc().SelectNodes("//NOTES");

                                    if (verses == null)
                                        break;
                                    currentFile = "Cleaning ... " + Path.GetFileName(files[i]);
                                    batchExportWorker.ReportProgress(progress);
                                    foreach (XmlNode verse in verses)
                                    {
                                        if (cancelled)
                                            break;
                                        try
                                        {
                                            verse.InnerXml = FormatUtil.convertRtfToHtml(verse.InnerText);
                                        }
                                        catch (Exception)
                                        {
                                            //Console.WriteLine(ex.Message);                   
                                        }
                                    }
                                    currentFile = "Converting ... " + Path.GetFileName(files[i]);
                                    batchExportWorker.ReportProgress(progress);
                                }
                                if (cancelled)
                                    break;
                                BookFormat book_format_export = BookFormat.doExportBook(files[i], tgtFilterIdx + 1);
                                if (book_format_export == null)
                                {
                                    if (Themes.MessageBox(Localization.BatchConversion_ErrorConverting + " " + Path.GetFileName(files[i]) + " ... " + Localization.BatchConversion_Continue, Localization.Error, MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No)
                                        break;
                                }
                                book_format_export.ExportBook(targetFolder + "\\" + Path.GetFileNameWithoutExtension(files[i]), tgtFilterIdx + 1);
                                break;
                            default:
                                break;
                        }
                    }
                    catch (Exception)
                    {
                        if (Themes.MessageBox(Localization.BatchConversion_ErrorConverting + " " + Path.GetFileName(files[i]) + " ... " + Localization.BatchConversion_Continue, Localization.Error, MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No)
                                    break;
                    }
                   
                }                
        }

        private void batchExportWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (!cancelled)
            {
                txtCurrentFile.Text = currentFile;
                progressStatus.Value = e.ProgressPercentage;
            }
            else
            {
                txtCurrentFile.Text = "Cancelling...";
                progressStatus.Value = 99;
            }
        }

        private void batchExportWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnCancel.Enabled = false;
            btnConvert.Enabled = true;
            progressStatus.Visible = false;
            txtCurrentFile.Visible = false;
             if (!cancelled)
                 Themes.MessageBox(Localization.BatchConversion_Completed, Localization.BatchConversion_Batch, MessageBoxButtons.OK, MessageBoxIcon.Information);
             else
                 Themes.MessageBox(Localization.BatchConversion_CancelledByUser, Localization.BatchConversion_Batch, MessageBoxButtons.OK, MessageBoxIcon.Warning);
             cancelled = false;
        }

        private void cbxSrcFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            srcFilterIdx = cbxSrcFormat.SelectedIndex;
        }

        private void cbxTgtFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            tgtFilterIdx = cbxTgtFormat.SelectedIndex;
        }

        private void BatchModeFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (batchExportWorker.IsBusy)
            {
                if (Themes.MessageBox(Localization.BatchConversion_ProcessBusyQuestion, Localization.BatchConversion_Batch, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cancelled = true;
                    batchExportWorker.CancelAsync();
                    txtCurrentFile.Text = "Cancelling batch process...";
                }
                else
                    e.Cancel = true;
            }
        }
    }
}
