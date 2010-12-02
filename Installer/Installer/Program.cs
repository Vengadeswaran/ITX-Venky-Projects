using System;
using System.Windows.Forms;
namespace Installer
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (!MyConfiguration.IsDemoOn)
            {
                if (args.Length == 0)
                    Application.Run(new LicensingITXDM());
                else
                {
                    MessageBox.Show(args[0]);
                    if (args[0].ToLower() == "installer")
                        Application.Run(new Licensing());
                    else
                        Application.Run(new LicensingITXDM());
                }
            }
            else
                Application.Run(new Licensing());
        }
    }
}
