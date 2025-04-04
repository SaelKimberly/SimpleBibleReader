﻿namespace Simple_Bible_Reader
{
    partial class FindWebBrowserControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FindWebBrowserControl));
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.chkBoxMatchCase = new System.Windows.Forms.CheckBox();
            this.button2 = new System.Windows.Forms.Button();
            this.chkBoxRegex = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkBoxNot = new System.Windows.Forms.CheckBox();
            this.radioExactBtn = new System.Windows.Forms.RadioButton();
            this.radioOrBtn = new System.Windows.Forms.RadioButton();
            this.radioAndBtn = new System.Windows.Forms.RadioButton();
            this.searchBackgroundJob = new System.ComponentModel.BackgroundWorker();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // textBox1
            // 
            this.textBox1.AcceptsReturn = true;
            resources.ApplyResources(this.textBox1, "textBox1");
            this.textBox1.Name = "textBox1";
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // chkBoxMatchCase
            // 
            resources.ApplyResources(this.chkBoxMatchCase, "chkBoxMatchCase");
            this.chkBoxMatchCase.Name = "chkBoxMatchCase";
            this.chkBoxMatchCase.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            resources.ApplyResources(this.button2, "button2");
            this.button2.Name = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // chkBoxRegex
            // 
            resources.ApplyResources(this.chkBoxRegex, "chkBoxRegex");
            this.chkBoxRegex.Name = "chkBoxRegex";
            this.chkBoxRegex.UseVisualStyleBackColor = true;
            this.chkBoxRegex.CheckedChanged += new System.EventHandler(this.chkBoxRegex_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkBoxNot);
            this.groupBox1.Controls.Add(this.radioExactBtn);
            this.groupBox1.Controls.Add(this.radioOrBtn);
            this.groupBox1.Controls.Add(this.radioAndBtn);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // chkBoxNot
            // 
            resources.ApplyResources(this.chkBoxNot, "chkBoxNot");
            this.chkBoxNot.Name = "chkBoxNot";
            this.chkBoxNot.UseVisualStyleBackColor = true;
            // 
            // radioExactBtn
            // 
            resources.ApplyResources(this.radioExactBtn, "radioExactBtn");
            this.radioExactBtn.Name = "radioExactBtn";
            this.radioExactBtn.UseVisualStyleBackColor = true;
            // 
            // radioOrBtn
            // 
            resources.ApplyResources(this.radioOrBtn, "radioOrBtn");
            this.radioOrBtn.Name = "radioOrBtn";
            this.radioOrBtn.UseVisualStyleBackColor = true;
            // 
            // radioAndBtn
            // 
            resources.ApplyResources(this.radioAndBtn, "radioAndBtn");
            this.radioAndBtn.Checked = true;
            this.radioAndBtn.Name = "radioAndBtn";
            this.radioAndBtn.TabStop = true;
            this.radioAndBtn.UseVisualStyleBackColor = true;
            // 
            // searchBackgroundJob
            // 
            this.searchBackgroundJob.WorkerReportsProgress = true;
            this.searchBackgroundJob.WorkerSupportsCancellation = true;
            this.searchBackgroundJob.DoWork += new System.ComponentModel.DoWorkEventHandler(this.searchBackgroundJob_DoWork);
            this.searchBackgroundJob.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.searchBackgroundJob_ProgressChanged);
            this.searchBackgroundJob.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.searchBackgroundJob_RunWorkerCompleted);
            // 
            // Find
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.chkBoxRegex);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.chkBoxMatchCase);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Find";
            this.ShowInTaskbar = false;
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox chkBoxMatchCase;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox chkBoxRegex;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioAndBtn;
        private System.Windows.Forms.RadioButton radioOrBtn;
        private System.Windows.Forms.RadioButton radioExactBtn;
        public System.ComponentModel.BackgroundWorker searchBackgroundJob;
        private System.Windows.Forms.CheckBox chkBoxNot;
    }
}