using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nexion.Shared.UI
{
	public partial class ShowProgress : Form
	{
		public ShowProgress()
		{
			InitializeComponent();
		}

		public void SetMessage(string message)
		{
			if(InvokeRequired)
			{
				this.Invoke(new Action<string>(SetMessage), message);
				return;
			}
			lblMessage.Text = message;
		}
	}
}
