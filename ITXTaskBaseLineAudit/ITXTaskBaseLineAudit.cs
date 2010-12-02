using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using ITXProjectsLibrary;
using Microsoft.Office.Project.Server.Events;
using Microsoft.SharePoint;
using PSLibrary = Microsoft.Office.Project.Server.Library;

namespace ITXTaskBaseLineAudit
{
    public class ITXTaskBaseLineAudit : ProjectEventReceiver
    {
        public override void OnPublished(PSLibrary.PSContextInfo contextInfo, ProjectPostPublishEventArgs e)
        {
            base.OnPublished(contextInfo, e);
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate
                                                         {
                                                             using (var Site = new SPSite(contextInfo.SiteGuid))
                                                             {
                                                                 MyUtilities.EnableSQlServerCLR(Site.ID);
                                                                 //MyUtilities.InstallStoredProcedure(Site.ID, false);
                                                                 string ConnectionString = Utilities.GetProjectServerSQLDatabaseConnectionString(Site.ID, Utilities.DatabaseType.PublishedDatabase);
                                                                 using (var sqlConnection = new SqlConnection(ConnectionString.Replace("15", "180")))
                                                                 {
                                                                     sqlConnection.Open();
                                                                     var cmd = new SqlCommand();
                                                                     cmd.Connection = sqlConnection;
                                                                     cmd.CommandType = CommandType.StoredProcedure;
                                                                     cmd.Parameters.Add(new SqlParameter("ProjectUID", e.ProjectGuid.ToString()));
                                                                     cmd.Parameters.Add(new SqlParameter("ModifiedBy", contextInfo.UserName));
                                                                     cmd.CommandText = "ITXTaskAuditTrail";
                                                                     DateTime starttime = DateTime.Now;
                                                                     cmd.ExecuteNonQuery();
                                                                     MyUtilities.ErrorLog("Time taken for execution : " + starttime.Subtract(DateTime.Now).Seconds, EventLogEntryType.SuccessAudit);
                                                                 }
                                                             }
                                                         });
            }
            catch (Exception ex)
            {
                if (string.IsNullOrEmpty(ex.StackTrace))
                    MyUtilities.ErrorLog("Error on Published Event due to : " + ex.Message, EventLogEntryType.Error);
                else
                    MyUtilities.ErrorLog("Error on Published Event due to : " + ex.Message + Environment.NewLine + "Stack Trace :" + ex.StackTrace, EventLogEntryType.Error);
            }
        }
    }
}