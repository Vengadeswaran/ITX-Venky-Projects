using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Security.Principal;
using ITXProjectsLibrary;
using ITXProjectsLibrary.WebSvcEvents;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

namespace ITXTaskBaseLineAudit
{
    public class FeatureReceiver : SPFeatureReceiver
    {
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            //Adding Project Published Event Handling Code starts here
            try
            {
                MyUtilities.ErrorLog("Feature Event-Activated Started", EventLogEntryType.SuccessAudit);
                var site = (SPSite)properties.Feature.Parent;
                using (var Site = new SPSite(site.Url))
                {
                    MyUtilities.EnableSQlServerCLR(Site.ID);
                    MyUtilities.InstallStoredProcedure(Site.ID, true);
                    var Events_Svc = new Events
                                         {
                                             UseDefaultCredentials = true,
                                             AllowAutoRedirect = true,
                                             Url = (Site.Url + @"/_vti_bin/psi/Events.asmx")
                                         };
                    string FilePath = SPUtility.GetGenericSetupPath(string.Empty) + @"\TEMPLATE\FEATURES\ITXTaskBaseLineAudit\ITXTaskBaseLineAudit.dll";
                    Utilities.CreatePSEvent(Events_Svc, FilePath, "Project", "Published",
                                            "ITX Task BaseLine Change Monitor",
                                            "This code is to track the task baseline value changes.");
                }
            }
            catch (Exception ex)
            {
                MyUtilities.ErrorLog("Error at activating feature due to : " + ex.Message, EventLogEntryType.Error);
            }
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            try
            {
                MyUtilities.ErrorLog("Feature Event-De-Activation Started", EventLogEntryType.SuccessAudit);
                var site = (SPSite)properties.Feature.Parent;
                using (var Site = new SPSite(site.Url))
                {
                    try
                    {
                        string Connstr = Utilities.GetProjectServerSQLDatabaseConnectionString(Site.ID, Utilities.DatabaseType.PublishedDatabase);
                        using (var connection = new SqlConnection(Connstr))
                        {
                            WindowsImpersonationContext wic = WindowsIdentity.Impersonate(IntPtr.Zero);
                            connection.Open();
                            var cmd = new SqlCommand();
                            cmd.Connection = connection;
                            cmd.CommandType = CommandType.Text;
                            cmd.CommandText = "drop procedure dbo.ITXTaskAuditTrail";
                            try
                            {
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception)
                            { }
                            cmd.CommandText = "DROP ASSEMBLY  TaskAuditTrail ";
                            try
                            {
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception)
                            { }
                            wic.Undo();
                        }
                    }
                    catch (Exception)
                    {
                    }
                    var Events_Svc = new Events();
                    Events_Svc.UseDefaultCredentials = true;
                    Events_Svc.AllowAutoRedirect = true;
                    Events_Svc.Url = Site.Url + @"/_vti_bin/psi/Events.asmx";
                    Utilities.DeletePSEvent(Events_Svc, "ITXTaskBaseLineAudit.ITXTaskBaseLineAudit");
                }
            }
            catch (Exception ex)
            {
                MyUtilities.ErrorLog("Error at de-activating feature due to : " + ex.Message, EventLogEntryType.Error);
            }
        }

        public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        {
        }

        public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        {
        }
    }
}