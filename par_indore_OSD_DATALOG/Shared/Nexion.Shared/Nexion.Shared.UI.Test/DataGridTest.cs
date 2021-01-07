using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nexion.Shared.UI.Test
{
	public partial class DataGridTest : Form
	{
		public DataGridTest()
		{
			InitializeComponent();
		}
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			NexionDataReportGrid<Employee> dataReportGrid = new NexionDataReportGrid<Employee>();
			this.Controls.Add(dataReportGrid);

			List<Employee> list = new List<Employee>();
			list.Add(new Employee() { PropA = 1, PropB = 1.25, PropC = "ABC", PropD = new DateTime(2018, 8, 1) });
			list.Add(new Employee() { PropA = 2, PropB = 1.25, PropC = "ABC", PropD = new DateTime(2018, 8, 2) });
			list.Add(new Employee() { PropA = 3, PropB = 1.25, PropC = "ABC", PropD = new DateTime(2018, 8, 1, 22, 5, 6) });

			foreach (var d in dataReportGrid.DataSource(list))
			{
				Debug.WriteLine(d.PropA);
			}


		}
	}

	public class Employee
	{
		[FieldName("propA", 1)]
		public int PropA { get; set; }
		[FieldName("propB", 2)]
		public double PropB { get; set; }
		[FieldName("propD", 4)]
		public string PropC { get; set; }
		[FieldName("propC", 3)]
		public DateTime PropD { get; set; }
	}
}
