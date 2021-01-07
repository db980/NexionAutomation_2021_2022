namespace ParReports_Indore
{
    partial class ReportDataGrid
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.printSettingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(0, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(688, 385);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Equipment";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.printSettingToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(181, 48);
            // 
            // printSettingToolStripMenuItem
            // 
            this.printSettingToolStripMenuItem.Name = "printSettingToolStripMenuItem";
            this.printSettingToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.printSettingToolStripMenuItem.Text = "Print Setting";
            this.printSettingToolStripMenuItem.Click += new System.EventHandler(this.printSettingToolStripMenuItem_Click);
            // 
            // ReportDataGrid
            // 
            this.Controls.Add(this.groupBox3);
            this.Name = "ReportDataGrid";
            this.Size = new System.Drawing.Size(688, 385);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        
		private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem printSettingToolStripMenuItem;
        // private Nexion.Shared.UI.NexionDataGrid dataGridView1;
        //private System.Windows.Forms.DataGridViewTextBoxColumn A;
        //private System.Windows.Forms.DataGridViewTextBoxColumn colModel;
        //private System.Windows.Forms.DataGridViewTextBoxColumn colVaccumSet;
        //private System.Windows.Forms.DataGridViewTextBoxColumn colVaccumPV;
        //private System.Windows.Forms.DataGridViewTextBoxColumn colSet2;
        //private System.Windows.Forms.DataGridViewTextBoxColumn colPV2;
        //private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        //private System.Windows.Forms.DataGridViewTextBoxColumn H;
        //private System.Windows.Forms.DataGridViewTextBoxColumn I;
    }
}
