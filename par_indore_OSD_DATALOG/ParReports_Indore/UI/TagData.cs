using ParReports_Indore.Logic;
using ParReports_Indore.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ParReports_Indore
{
	public partial class TagData : Form
	{
		DataClient dataClient = new DataClient();

		public TagData()
		{
			InitializeComponent();
			//Load tags

			var list = dataClient.GetTags();
			cmbTags.Items.Add(new ParReports_Indore.Model.TagInfo() { TagIndex =-1, TagName="ALL" });
			foreach(var tag in list)
			{
				cmbTags.Items.Add(tag);
			}
			cmbTags.DisplayMember = "TagName";
			cmbTags.SelectedIndex = 0;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			
			var data = dataClient.GetTagData(dateTimePicker1.Value, dateTimePicker2.Value,cmbTags.SelectedItem as TagInfo );
			dataGridView1.AutoGenerateColumns = false;
			dataGridView1.DataSource = data;

		}
		
	}
}
