using LinqToDB;
using ParReports_Indore.Logic;
using Nexion.Shared.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Nexion.Shared.Core;

namespace ParReports_Indore
{
    public partial class SelectColumns : Form
    {
       
        DataReportDB db = new DataReportDB();
        List<TrendMappingDEHLog> _data;
        public SelectColumns()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            cmbReportTypes.Items.Clear();
            cmbReportTypes.Items.Add("--Select--");
            cmbReportTypes.Items.AddRange(typeof(ReportTypes).GetAllConstantValues<string>().ToArray());
            cmbReportTypes.SelectedIndex = 0;


        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            foreach (var row in _data)
            {
                var shift = db.TrendMappingDEHLogs
                       .Where(a => a.PLCTAG == row.PLCTAG && a.AHUName == cmbReportTypes.Text)
                       .Set(p => p.Show, row.Show)
                       .Set(p => p.Description, row.Description)
                       .Set(p => p.ColumnIndex, row.ColumnIndex)
                       .Set(p => p.Logic, row.Logic)
                       .Update();
            }

            CoreMessageBox.Show("Preference Saved");
        }

        private void cmbReportTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbReportTypes.SelectedIndex == 0)
            {
                this.dataGridView1.DataSource = null;
                dataGridView1.AutoGenerateColumns = false;
            }
            else
            {
                _data = db.TrendMappingDEHLogs.Where(a => a.AHUName == cmbReportTypes.Text).OrderBy(a => a.ColumnIndex).ToList();
                this.dataGridView1.DataSource = _data;
                dataGridView1.AutoGenerateColumns = false;
            }

        }
    }

}
