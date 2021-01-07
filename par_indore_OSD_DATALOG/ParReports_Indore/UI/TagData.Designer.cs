namespace ParReports_Indore
{
	partial class TagData
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TagData));
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
			this.label2 = new System.Windows.Forms.Label();
			this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
			this.label1 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.colTagIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colTagName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colVal = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.coltimestamp = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.label3 = new System.Windows.Forms.Label();
			this.cmbTags = new System.Windows.Forms.ComboBox();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.cmbTags);
			this.splitContainer1.Panel1.Controls.Add(this.label3);
			this.splitContainer1.Panel1.Controls.Add(this.dateTimePicker2);
			this.splitContainer1.Panel1.Controls.Add(this.label2);
			this.splitContainer1.Panel1.Controls.Add(this.dateTimePicker1);
			this.splitContainer1.Panel1.Controls.Add(this.label1);
			this.splitContainer1.Panel1.Controls.Add(this.button1);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.dataGridView1);
			this.splitContainer1.Size = new System.Drawing.Size(1253, 569);
			this.splitContainer1.SplitterDistance = 92;
			this.splitContainer1.TabIndex = 0;
			// 
			// dateTimePicker2
			// 
			this.dateTimePicker2.CustomFormat = "dd-MM-yyy HH:mm:ss";
			this.dateTimePicker2.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dateTimePicker2.Location = new System.Drawing.Point(346, 14);
			this.dateTimePicker2.Name = "dateTimePicker2";
			this.dateTimePicker2.Size = new System.Drawing.Size(200, 20);
			this.dateTimePicker2.TabIndex = 4;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(287, 21);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(35, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "label2";
			// 
			// dateTimePicker1
			// 
			this.dateTimePicker1.CustomFormat = "dd-MM-yyy HH:mm:ss";
			this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dateTimePicker1.Location = new System.Drawing.Point(64, 15);
			this.dateTimePicker1.Name = "dateTimePicker1";
			this.dateTimePicker1.Size = new System.Drawing.Size(200, 20);
			this.dateTimePicker1.TabIndex = 2;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(22, 23);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(35, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "label1";
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(999, 11);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 0;
			this.button1.Text = "&Show";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// dataGridView1
			// 
			this.dataGridView1.AllowUserToAddRows = false;
			this.dataGridView1.AllowUserToDeleteRows = false;
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colTagIndex,
            this.colTagName,
            this.colVal,
            this.coltimestamp});
			this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridView1.Location = new System.Drawing.Point(0, 0);
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.ReadOnly = true;
			this.dataGridView1.Size = new System.Drawing.Size(1253, 473);
			this.dataGridView1.TabIndex = 0;
			// 
			// colTagIndex
			// 
			this.colTagIndex.DataPropertyName = "TagIndex";
			this.colTagIndex.HeaderText = "TagIndex";
			this.colTagIndex.Name = "colTagIndex";
			this.colTagIndex.ReadOnly = true;
			// 
			// colTagName
			// 
			this.colTagName.DataPropertyName = "TagName";
			this.colTagName.HeaderText = "TagName";
			this.colTagName.Name = "colTagName";
			this.colTagName.ReadOnly = true;
			this.colTagName.Width = 300;
			// 
			// colVal
			// 
			this.colVal.DataPropertyName = "Val";
			this.colVal.HeaderText = "Value";
			this.colVal.Name = "colVal";
			this.colVal.ReadOnly = true;
			this.colVal.Width = 150;
			// 
			// coltimestamp
			// 
			this.coltimestamp.DataPropertyName = "UpdateDateTime";
			dataGridViewCellStyle1.Format = "dd-MM-yyyy HH:mm:ss";
			dataGridViewCellStyle1.NullValue = null;
			this.coltimestamp.DefaultCellStyle = dataGridViewCellStyle1;
			this.coltimestamp.HeaderText = "Timestamp";
			this.coltimestamp.Name = "coltimestamp";
			this.coltimestamp.ReadOnly = true;
			this.coltimestamp.Width = 150;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(569, 20);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(31, 13);
			this.label3.TabIndex = 5;
			this.label3.Text = "Tags";
			// 
			// cmbTags
			// 
			this.cmbTags.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbTags.FormattingEnabled = true;
			this.cmbTags.Location = new System.Drawing.Point(629, 13);
			this.cmbTags.Name = "cmbTags";
			this.cmbTags.Size = new System.Drawing.Size(270, 21);
			this.cmbTags.TabIndex = 6;
			// 
			// TagData
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1253, 569);
			this.Controls.Add(this.splitContainer1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "TagData";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "TagData";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.DateTimePicker dateTimePicker2;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.DateTimePicker dateTimePicker1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.DataGridView dataGridView1;
		private System.Windows.Forms.DataGridViewTextBoxColumn colTagIndex;
		private System.Windows.Forms.DataGridViewTextBoxColumn colTagName;
		private System.Windows.Forms.DataGridViewTextBoxColumn colVal;
		private System.Windows.Forms.DataGridViewTextBoxColumn coltimestamp;
		private System.Windows.Forms.ComboBox cmbTags;
		private System.Windows.Forms.Label label3;
	}
}