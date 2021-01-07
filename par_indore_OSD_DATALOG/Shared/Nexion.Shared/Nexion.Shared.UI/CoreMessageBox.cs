using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nexion.Shared.UI
{
	public class CoreMessageBox
	{
		public static DialogResult Warning(Exception ex, string caption = "Warning")
		{
			return Warning(ex.Message, caption);
		}

		public static DialogResult Warning(string Message, string caption = "Warning")
		{
			return MessageBox.Show(Message, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}

		public static DialogResult Show(string Message, string caption = "Information")
		{
			return MessageBox.Show(Message, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		public static DialogResult Error(Exception ex, string caption = "Error")
		{
			return Error(ex.ToString(), caption);
		}

		public static DialogResult Error(string Message, string caption = "Error")
		{
			return MessageBox.Show(Message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		public static DialogResult Question(string Message, string caption = "Questioin")
		{
			return MessageBox.Show(Message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
		}


	}
}
