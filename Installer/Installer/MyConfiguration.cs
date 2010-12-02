using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using ITXProjectsLibrary;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace Installer
{
    class MyConfiguration
    {
        public static string LICENSE_PARAMETERS =
     @"<EncryptedLicenseParameters>
	  <ProductName>ITX Tasks Excel Update</ProductName>
	  <RSAKeyValue>
	    <Modulus>lazUwUuHdhdzMs83TwxgaCITh3bfD0nK2r8Dytoxxk3E674LvnKWNNIRvKVl19MwN/PSczFg0hhjfQyY1wYRgXyD1vp1PVqas3akcS1WLYxz3dGbWSn26yhIrKCqAPXfSEQYrGbe0rpg4SxeSf4cEHV2FF0JFaM7OhFX5tYU5Bs=</Modulus>
	    <Exponent>AQAB</Exponent>
	  </RSAKeyValue>
	  <DesignSignature>OLwulJ+mCCSKPBkhwdS2E+NvD4kwDEnTMNbwAXFEmXW6Q0UWz0JN6sYls9zGii3w94/37RHUFbwc/3ckBxaKE8vpXDFAiwE6gVUC8Jh3gBsTVB5PTux1Jj7UBhn0+Fh3eHBAyzcuigf3etQb0Oa0dDkwjd7aAbq47Kg/F4/rgwo=</DesignSignature>
	  <RuntimeSignature>LUtowW/P3GM+q51VYGFjlJm/A0W6/Ak3+j8iqXeITiaC2+InYKrpQIdIZ+1bpFTv9ilqbGPOyn4KvuGmisApMr2gwNdoPZdOGAuuRRY2LX1TqZJS10BW6mBZW8KW68Hqn5+JCyXs9gR2vFwQWugDRkl42kBAS64m1WQOlRtZptc=</RuntimeSignature>
	  <KeyStrength>7</KeyStrength>
	</EncryptedLicenseParameters>";
        public static string licenseFile = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\LicensedApp\" + Application_Title + ".lic";

        public static bool IsDemoOn { get { return true; } }

        public static bool SkipLicensing { get { return true; } }

        public static string Application_Title { get { return "ITX BaseLine Auditor"; } }

        public static MyUtilities.DeploymentLevel DeploymentLevel { get { return MyUtilities.DeploymentLevel.Site; } }

        public static string DefaultSiteUrl { get { return "http://epm2007demo/pwa01"; } }

        public static void StartInstallation(SPSite Site, SPWeb Web, Label LblStatus)
        {
            if (Web != null)
                Web.AllowUnsafeUpdates = true;

            MyUtilities.UpdateStatus(" Copying required files  ", LblStatus);
            Deployment.CopyFilesAndFolders(Application.StartupPath + "\\Features", MyUtilities.FeaturesFolderPath, true);

            MyUtilities.UpdateStatus(" Installing required feature ", LblStatus);
            // Creating an Batch file to install the Feature
            string WebPartInstallerPath = Application.StartupPath + "\\Install2.bat";
            if (File.Exists(WebPartInstallerPath))
                File.Delete(WebPartInstallerPath);
            try
            {
                StreamWriter Writer = new StreamWriter(WebPartInstallerPath);
                Writer.WriteLine("@ echo off");
                Writer.WriteLine(@"stsadm -o installfeature -filename ITXTaskBaseLineAudit\feature.xml");
                Writer.Flush();
                Writer.Close();
                Writer.Dispose();
            }
            catch (Exception Ex23)
            {
            }
            Deployment.ExecuteProcess(WebPartInstallerPath, ProcessWindowStyle.Hidden, true);

            try
            {
                MyUtilities.UpdateStatus(" Updating web.config file ", LblStatus);
                //To Get Port from Siteurl
                Uri uri = new Uri(Site.Url);
                int Port = uri.Port;
                if (Port != 80)
                {
                    string Wss80Path = Deployment.GetWssVirtualDirectoryPath("80");
                    if (Wss80Path != string.Empty)
                    {
                        // Moving files from Current Application Path to Virtual directory
                        Wss80Path += @"\web.config";
                        Utilities.SetAttributeValueInWebConfig(Wss80Path, "configuration/system.web/trust", "level", "Full");
                    }
                }
                List<string> ExtendedUrlList = new List<string>();
                foreach (int zoneindex in Enum.GetValues(typeof(SPUrlZone)))
                {
                    string ExtendedUrl = Deployment.GetVirtualDirectoryPath((SPUrlZone)zoneindex, Site);
                    bool Found = false;
                    foreach (string s in ExtendedUrlList)
                    {
                        if (s.ToLower().Trim() == ExtendedUrl.ToLower().Trim())
                        {
                            Found = true;
                            break;
                        }
                    }
                    if (!Found)
                    {
                        ExtendedUrlList.Add(ExtendedUrl);
                    }
                }
                foreach (string Urls in ExtendedUrlList)
                {
                    try
                    {
                        // Moving files from Current Application Path to Virtual directory
                        Utilities.SetAttributeValueInWebConfig(Urls + @"\web.config", "configuration/system.web/trust", "level", "Full");
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch (Exception)
            {
            }
            // Restart IIS
            MyUtilities.UpdateStatus("Restart IIS started ...", LblStatus);
            if (MyUtilities.ShowDialog("To complete the installation you need to restart it IIS" + Environment.NewLine +
                    "Are you sure you want to restart IIS right now?", Telerik.WinControls.RadMessageIcon.Question) == DialogResult.Yes)
            {
                Application.DoEvents();

                Deployment.RestartIIS(Application.StartupPath);
            }
            Application.Exit();
        }
    }
}