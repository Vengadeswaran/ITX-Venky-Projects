using System;
using System.Windows.Forms;
using Infralution.Licensing;
using Telerik.WinControls;
namespace Installer
{
    public partial class Licensing : RadForm 
    {
        public Licensing()
        {
            InitializeComponent();
        }
        private DialogResult ShowDialog(string Message, Telerik.WinControls.RadMessageIcon Info)
        {
            RadMessageBox.SetThemeName("Office2007Black");
            if (Info == RadMessageIcon.Question)
                return RadMessageBox.Show(this, Message,MyConfiguration.Application_Title, MessageBoxButtons.YesNo, Info);
            return RadMessageBox.Show(this, Message, MyConfiguration.Application_Title, MessageBoxButtons.OK, Info);
        }
        private void Licensing_Load(object sender, EventArgs e)
        {
            if (!MyConfiguration.SkipLicensing)
            {
                EvaluationDialog evaluationDialog = new EvaluationDialog(MyConfiguration.Application_Title,MyConfiguration.Application_Title);
                LblDuration.Text = "You are on the day " + evaluationDialog.EvaluationMonitor.DaysInUse +" out of 30 day evaluation";
                this.Height = PnlDisplay.Height + 25;
                Application.DoEvents();
            }
            else
            {
                this.Hide();
                Application.DoEvents();
                Installer Frm = new Installer();
                Frm.ShowDialog();
                Application.Exit();
            }
        }
        private void BtnInstallLicense_Click(object sender, EventArgs e)
        {
            PnlDisplay.Visible = false;
            PnlActivation.Visible = true;
            this.Height = PnlActivation.Height + 25;
            Application.DoEvents();
        }
        private void BtnContEval_Click(object sender, EventArgs e)
        {
            this.Hide();
            Application.DoEvents();
            Installer Frm = new Installer();
            Frm.ShowDialog();
            Application.Exit();
        }
        private void radButton1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void BtnActivate_Click(object sender, EventArgs e)
        {
            if (TxtKey.Text.Trim() == string.Empty)
            {
                ShowDialog("Input Licensing Key", Telerik.WinControls.RadMessageIcon.Info);
                return;
            }
            string key = TxtKey.Text.Trim();
            if (!string.IsNullOrEmpty(key))
            {
                EncryptedLicenseProvider provider = new EncryptedLicenseProvider();
                EncryptedLicenseProvider.SetParameters(MyConfiguration.LICENSE_PARAMETERS);
                EncryptedLicense license = provider.InstallLicense(MyConfiguration.licenseFile, key);
                if (license == null)
                {
                    ShowDialog("Invalid License Key Entered", Telerik.WinControls.RadMessageIcon.Info);
                    return;
                }
                else
                {
                    ShowDialog("Activated Successfully", Telerik.WinControls.RadMessageIcon.Info);
                    this.Hide();
                    Application.DoEvents();
                    Installer Frm = new Installer();
                    Frm.ShowDialog();
                    Application.Exit();
                }
            }
        }
        private void radButton3_Click(object sender, EventArgs e)
        {
            PnlActivation.Visible = false;
            PnlDisplay.Visible = true;
            this.Height = PnlDisplay.Height + 25;
            Application.DoEvents();
        }
    }
}
