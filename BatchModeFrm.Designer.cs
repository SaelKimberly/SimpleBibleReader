namespace Simple_Bible_Reader
{
    partial class BatchModeFrm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BatchModeFrm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbxSrcFormat = new System.Windows.Forms.ComboBox();
            this.cbxSrcType = new System.Windows.Forms.ComboBox();
            this.btnSrcBtn = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSrcFolder = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbxTgtFormat = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtTgtFolder = new System.Windows.Forms.TextBox();
            this.btnConvert = new System.Windows.Forms.Button();
            this.progressStatus = new System.Windows.Forms.ProgressBar();
            this.btnCancel = new System.Windows.Forms.Button();
            this.batchExportWorker = new System.ComponentModel.BackgroundWorker();
            this.txtCurrentFile = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbxSrcFormat);
            this.groupBox1.Controls.Add(this.cbxSrcType);
            this.groupBox1.Controls.Add(this.btnSrcBtn);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtSrcFolder);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // cbxSrcFormat
            // 
            this.cbxSrcFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxSrcFormat.FormattingEnabled = true;
            resources.ApplyResources(this.cbxSrcFormat, "cbxSrcFormat");
            this.cbxSrcFormat.Name = "cbxSrcFormat";
            this.cbxSrcFormat.SelectedIndexChanged += new System.EventHandler(this.cbxSrcFormat_SelectedIndexChanged);
            // 
            // cbxSrcType
            // 
            this.cbxSrcType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxSrcType.FormattingEnabled = true;
            this.cbxSrcType.Items.AddRange(new object[] {
            resources.GetString("cbxSrcType.Items"),
            resources.GetString("cbxSrcType.Items1"),
            resources.GetString("cbxSrcType.Items2"),
            resources.GetString("cbxSrcType.Items3")});
            resources.ApplyResources(this.cbxSrcType, "cbxSrcType");
            this.cbxSrcType.Name = "cbxSrcType";
            this.cbxSrcType.SelectedIndexChanged += new System.EventHandler(this.cbxSrcType_SelectedIndexChanged);
            // 
            // btnSrcBtn
            // 
            resources.ApplyResources(this.btnSrcBtn, "btnSrcBtn");
            this.btnSrcBtn.Name = "btnSrcBtn";
            this.btnSrcBtn.UseVisualStyleBackColor = true;
            this.btnSrcBtn.Click += new System.EventHandler(this.btnSrcBtn_Click);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // txtSrcFolder
            // 
            resources.ApplyResources(this.txtSrcFolder, "txtSrcFolder");
            this.txtSrcFolder.Name = "txtSrcFolder";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbxTgtFormat);
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.txtTgtFolder);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // cbxTgtFormat
            // 
            this.cbxTgtFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxTgtFormat.FormattingEnabled = true;
            resources.ApplyResources(this.cbxTgtFormat, "cbxTgtFormat");
            this.cbxTgtFormat.Name = "cbxTgtFormat";
            this.cbxTgtFormat.SelectedIndexChanged += new System.EventHandler(this.cbxTgtFormat_SelectedIndexChanged);
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // txtTgtFolder
            // 
            resources.ApplyResources(this.txtTgtFolder, "txtTgtFolder");
            this.txtTgtFolder.Name = "txtTgtFolder";
            // 
            // btnConvert
            // 
            resources.ApplyResources(this.btnConvert, "btnConvert");
            this.btnConvert.Name = "btnConvert";
            this.btnConvert.UseVisualStyleBackColor = true;
            this.btnConvert.Click += new System.EventHandler(this.button2_Click);
            // 
            // progressStatus
            // 
            resources.ApplyResources(this.progressStatus, "progressStatus");
            this.progressStatus.Name = "progressStatus";
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // batchExportWorker
            // 
            this.batchExportWorker.WorkerReportsProgress = true;
            this.batchExportWorker.WorkerSupportsCancellation = true;
            this.batchExportWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.batchExportWorker_DoWork);
            this.batchExportWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.batchExportWorker_ProgressChanged);
            this.batchExportWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.batchExportWorker_RunWorkerCompleted);
            // 
            // txtCurrentFile
            // 
            resources.ApplyResources(this.txtCurrentFile, "txtCurrentFile");
            this.txtCurrentFile.Name = "txtCurrentFile";
            // 
            // BatchModeFrm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtCurrentFile);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnConvert);
            this.Controls.Add(this.progressStatus);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "BatchModeFrm";
            this.ShowInTaskbar = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BatchModeFrm_FormClosing);
            this.Load += new System.EventHandler(this.BatchModeFrm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSrcFolder;
        private System.Windows.Forms.Button btnSrcBtn;
        private System.Windows.Forms.ComboBox cbxSrcFormat;
        private System.Windows.Forms.ComboBox cbxSrcType;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox cbxTgtFormat;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtTgtFolder;
        private System.Windows.Forms.Button btnConvert;
        private System.Windows.Forms.ProgressBar progressStatus;
        private System.Windows.Forms.Button btnCancel;
        private System.ComponentModel.BackgroundWorker batchExportWorker;
        private System.Windows.Forms.Label txtCurrentFile;
    }
}