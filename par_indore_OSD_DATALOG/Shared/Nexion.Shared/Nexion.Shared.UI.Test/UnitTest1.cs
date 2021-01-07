using System;
using System.Windows.Forms;

namespace Nexion.Shared.UI.Test
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(params string[] args)
		{
			Application.Run(new DataGridTest());
		}
	}
}
