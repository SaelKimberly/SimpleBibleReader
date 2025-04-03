namespace Simple_Bible_Reader
{
    partial class PluginProcessFrm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PluginProcessFrm));
            this.pbPlugin = new System.Windows.Forms.ProgressBar();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblProgress = new System.Windows.Forms.Label();
            this.bkgndPluginWorker = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // pbPlugin
            // 
            resources.ApplyResources(this.pbPlugin, "pbPlugin");
            this.pbPlugin.Name = "pbPlugin";
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblProgress
            // 
            resources.ApplyResources(this.lblProgress, "lblProgress");
            this.lblProgress.Name = "lblProgress";
            // 
            // bkgndPluginWorker
            // 
            this.bkgndPluginWorker.WorkerReportsProgress = true;
            this.bkgndPluginWorker.WorkerSupportsCancellation = true;
            this.bkgndPluginWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bkgndPluginWorker_DoWork);
            this.bkgndPluginWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bkgndPluginWorker_ProgressChanged);
            this.bkgndPluginWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bkgndPluginWorker_RunWorkerCompleted);
            // 
            // PluginProcessFrm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ControlBox = false;
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.pbPlugin);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "PluginProcessFrm";
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.Load += new System.EventHandler(this.PluginProcessFrm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar pbPlugin;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblProgress;
        private System.ComponentModel.BackgroundWorker bkgndPluginWorker;
    }
}