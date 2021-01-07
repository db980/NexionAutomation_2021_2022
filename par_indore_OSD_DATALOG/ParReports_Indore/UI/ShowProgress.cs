using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ParReports_Indore
{
	public partial class ShowProgress : Form
	{
		public ShowProgress()
		{
			InitializeComponent();
		}

		public void SetMessage(string message)
		{
			lblMessage.Text = message;
		}
	}
}
