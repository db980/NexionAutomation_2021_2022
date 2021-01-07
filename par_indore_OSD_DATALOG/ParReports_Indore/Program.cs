using ParReports_Indore.Logic;
using Nexion.Shared.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ParReports_Indore
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(params string[] args)
        {
            try
            {
                List<string> menus = new List<string>();
                menus.Add(MenuList.DataReport);


                if (File.Exists(AppProfile.FileName))
                {
                    Util.AppProfile = Util.Deserialize<AppProfile>(AppProfile.FileName);
                    Util.AppProfile.MenuList = menus.ToArray();
                }
                else
                {
                    Util.AppProfile = new AppProfile
                    {
                        MenuList = menus.ToArray()
                    };
                    Util.AppProfile.Initialize();
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
                Util.AppProfile.MenuList.Contains(MenuList.DataReport);

            }
            catch (Exception ex)
            {
                CoreMessageBox.Error(ex);
            }

        }
    }
}
