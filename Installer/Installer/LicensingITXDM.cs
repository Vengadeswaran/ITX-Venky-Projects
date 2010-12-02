using System;
using System.Windows.Forms;
using Infralution.Licensing;
namespace Installer
{
    public partial class LicensingITXDM : Telerik.WinControls.RadForm 
    {
        public LicensingITXDM()
        {
            InitializeComponent();
        }
        private void LicensingITXAP_Load(object sender, EventArgs e)
        {
            if (!MyConfiguration.SkipLicensing)
            {
                this.Text = MyConfiguration.Application_Title;
                LblStatus.Text = "Enter Licensing Key to Activate " + MyConfiguration.Application_Title;
                EncryptedLicenseProvider provider = new EncryptedLicenseProvider();
                EncryptedLicense license = provider.GetLicense(MyConfiguration.LICENSE_PARAMETERS,MyConfiguration.licenseFile);
                if (license == null)
                {
                    PnlToActivate.Visible = true;
                    PnlActivated.Visible = false;
                    this.Height = 104;
                }
                else
                {
                    PnlToActivate.Visible = false;
                    PnlActivated.Visible = true;
                    this.Height = 104;
                }
            }
            else
            {
                PnlToActivate.Visible = false;
                PnlActivated.Visible = true;
                this.Height = 104;
            }
       }
        private void ShowDialog(string Message, Telerik.WinControls.RadMessageIcon Info)
        {
            Telerik.WinControls.RadMessageBox.SetThemeName("Office2007Black");
            Telerik.WinControls.RadMessageBox.Show(this, Message, MyConfiguration.Application_Title, MessageBoxButtons.OK, Info);
        }
        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            if (TxtUrl.Text.Trim() == "")
            {
                ShowDialog("Input Correct License Key", Telerik.WinControls.RadMessageIcon.Error);
                TxtUrl.Focus();
                return;
            }
            else
            {
                if (InstallLicenseKey(TxtUrl.Text.Trim()))
                {
                    ShowDialog( MyConfiguration.Application_Title +  " Successfully", Telerik.WinControls.RadMessageIcon.Info);
                    Application.Exit();
                }
                else
                    ShowDialog("The entered license key is not a valid key for this product", Telerik.WinControls.RadMessageIcon.Error);
            }
        }
        protected virtual bool InstallLicenseKey(string key)
        {
            EncryptedLicenseProvider LicenseProvider = new EncryptedLicenseProvider();
            EncryptedLicense license = LicenseProvider.ValidateLicenseKey(key);
            if (license == null)
                return false;
            else
                LicenseProvider.InstallLicense(MyConfiguration.licenseFile, license);
            return (license != null);
        }
    }
}
