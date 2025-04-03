namespace Simple_Bible_Reader
{
    partial class Plugins2Frm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Plugins2Frm));
            this.cbEnablePlugins = new System.Windows.Forms.CheckBox();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbInput = new System.Windows.Forms.TextBox();
            this.tbOutput = new System.Windows.Forms.TextBox();
            this.tbPlugin = new System.Windows.Forms.TextBox();
            this.btnTestRun = new System.Windows.Forms.Button();
            this.btnLoadCurrentVerse = new System.Windows.Forms.Button();
            this.btnLoadPlugin = new System.Windows.Forms.Button();
            this.cbEscapeXML = new System.Windows.Forms.CheckBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnSavePlugin = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbEnablePlugins
            // 
            resources.ApplyResources(this.cbEnablePlugins, "cbEnablePlugins");
            this.cbEnablePlugins.Name = "cbEnablePlugins";
            this.cbEnablePlugins.UseVisualStyleBackColor = true;
            this.cbEnablePlugins.CheckedChanged += new System.EventHandler(this.cbEnablePlugins_CheckedChanged);
            // 
            // button2
            // 
            resources.ApplyResources(this.button2, "button2");
            this.button2.Name = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.tableLayoutPanel1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label3, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.tbInput, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tbOutput, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.tbPlugin, 1, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // tbInput
            // 
            this.tbInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.tbInput, "tbInput");
            this.tbInput.Name = "tbInput";
            // 
            // tbOutput
            // 
            this.tbOutput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.tbOutput, "tbOutput");
            this.tbOutput.Name = "tbOutput";
            this.tbOutput.ReadOnly = true;
            // 
            // tbPlugin
            // 
            this.tbPlugin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.tbPlugin, "tbPlugin");
            this.tbPlugin.Name = "tbPlugin";
            // 
            // btnTestRun
            // 
            resources.ApplyResources(this.btnTestRun, "btnTestRun");
            this.btnTestRun.Name = "btnTestRun";
            this.btnTestRun.UseVisualStyleBackColor = true;
            this.btnTestRun.Click += new System.EventHandler(this.btnTestRun_Click);
            // 
            // btnLoadCurrentVerse
            // 
            resources.ApplyResources(this.btnLoadCurrentVerse, "btnLoadCurrentVerse");
            this.btnLoadCurrentVerse.Name = "btnLoadCurrentVerse";
            this.btnLoadCurrentVerse.UseVisualStyleBackColor = true;
            this.btnLoadCurrentVerse.Click += new System.EventHandler(this.btnLoadCurrentVerse_Click);
            // 
            // btnLoadPlugin
            // 
            resources.ApplyResources(this.btnLoadPlugin, "btnLoadPlugin");
            this.btnLoadPlugin.Name = "btnLoadPlugin";
            this.btnLoadPlugin.UseVisualStyleBackColor = true;
            this.btnLoadPlugin.Click += new System.EventHandler(this.btnLoadPlugin_Click);
            // 
            // cbEscapeXML
            // 
            resources.ApplyResources(this.cbEscapeXML, "cbEscapeXML");
            this.cbEscapeXML.Name = "cbEscapeXML";
            this.cbEscapeXML.UseVisualStyleBackColor = true;
            // 
            // btnClear
            // 
            resources.ApplyResources(this.btnClear, "btnClear");
            this.btnClear.Name = "btnClear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnSavePlugin
            // 
            resources.ApplyResources(this.btnSavePlugin, "btnSavePlugin");
            this.btnSavePlugin.Name = "btnSavePlugin";
            this.btnSavePlugin.UseVisualStyleBackColor = true;
            this.btnSavePlugin.Click += new System.EventHandler(this.btnSavePlugin_Click);
            // 
            // Plugins2Frm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnSavePlugin);
            this.Controls.Add(this.cbEscapeXML);
            this.Controls.Add(this.btnLoadCurrentVerse);
            this.Controls.Add(this.btnLoadPlugin);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnTestRun);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.cbEnablePlugins);
            this.MinimizeBox = false;
            this.Name = "Plugins2Frm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PluginsFrm_FormClosing);
            this.Load += new System.EventHandler(this.PluginsFrm_Load);
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cbEnablePlugins;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbInput;
        private System.Windows.Forms.TextBox tbOutput;
        private System.Windows.Forms.TextBox tbPlugin;
        private System.Windows.Forms.Button btnTestRun;
        private System.Windows.Forms.Button btnLoadCurrentVerse;
        private System.Windows.Forms.Button btnLoadPlugin;
        private System.Windows.Forms.CheckBox cbEscapeXML;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnSavePlugin;
    }
}