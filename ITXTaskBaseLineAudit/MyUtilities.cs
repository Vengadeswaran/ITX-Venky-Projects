using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using ITXProjectsLibrary;
using Microsoft.SharePoint.Utilities;

namespace ITXTaskBaseLineAudit
{
    public class MyUtilities
    {
        public static void ErrorLog(string LogStr, EventLogEntryType Type)
        {
            try
            {
                WindowsImpersonationContext wic = WindowsIdentity.Impersonate(IntPtr.Zero);
                var El = new EventLog();
                if (EventLog.SourceExists("ITXTaskBaseLineAudit") == false)
                    EventLog.CreateEventSource("ITXTaskBaseLineAudit", "ITXTaskBaseLineAudit");
                El.Source = "ITXTaskBaseLineAudit";
                El.WriteEntry(LogStr, Type);
                El.Close();
                wic.Undo();
            }
            catch (Exception Ex87)
            {
                WriteTextLog(Ex87.Message + "\r" + LogStr);
            }
        }

        public static void WriteTextLog(string LogStr)
        {
            try
            {
                StreamWriter Writer = new StreamWriter(@"c:\ITXTaskBaseLineAudit.txt", true);
                Writer.WriteLine(LogStr);
                Writer.Close();
                Writer.Dispose();
            }
            catch (Exception)
            {
                return;
            }
        }

        public static void InstallStoredProcedure(Guid SiteID, bool ShowError)
        {
            try
            {
                string Connstr = Utilities.GetProjectServerSQLDatabaseConnectionString(SiteID, Utilities.DatabaseType.PublishedDatabase);
                using (var connection = new SqlConnection(Connstr))
                {
                    WindowsImpersonationContext wic = WindowsIdentity.Impersonate(IntPtr.Zero);
                    connection.Open();
                    var cmd = new SqlCommand();
                    cmd.Connection = connection;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "DROP ASSEMBLY  TaskAuditTrail ";
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        if (ShowError)
                            ErrorLog("Error while dropping an assembly due to :" + ex.Message, EventLogEntryType.Error);
                    }
                    string DllPath = SPUtility.GetGenericSetupPath(string.Empty) + @"\TEMPLATE\FEATURES\ITXTaskBaseLineAudit\TaskAuditTrail.dll";
                    cmd.CommandText = "CREATE ASSEMBLY TaskAuditTrail FROM '" + DllPath + "' WITH PERMISSION_SET = SAFE;";
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        if (ShowError)
                            ErrorLog("Error while Creating an assembly due to :" + ex.Message, EventLogEntryType.Error);
                    }

                    cmd.CommandText = "CREATE PROCEDURE dbo.ITXTaskAuditTrail @ProjectUID [nvarchar](4000) ,@ModifiedBy [nvarchar](4000) AS EXTERNAL NAME TaskAuditTrail.StoredProcedures.ITXTaskAuditTrail ";
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        if (ShowError)
                            ErrorLog("Error while Creating an Procedure due to :" + ex.Message, EventLogEntryType.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog("Error at InstallStoredProcedure due to :" + ex.Message, EventLogEntryType.Error);
            }
        }

        public static void EnableSQlServerCLR(Guid SiteID)
        {
            try
            {
                string Connstr = Utilities.GetProjectServerSQLDatabaseConnectionString(SiteID, Utilities.DatabaseType.PublishedDatabase);
                using (var connection = new SqlConnection(Connstr))
                {
                    connection.Open();
                    var cmd = new SqlCommand();
                    cmd.Connection = connection;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "EXEC sp_configure 'show advanced options' , '1'";
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception)
                    { }
                    cmd.CommandText = "reconfigure";
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception)
                    { }
                    cmd.CommandText = "EXEC sp_configure 'clr enabled' , '1'";
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception)
                    { }
                    cmd.CommandText = "reconfigure";
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception)
                    { }
                }
            }
            catch (Exception)
            {
                //ErrorLog("Error at EnableSQlServerCLR due to :" + ex.Message, EventLogEntryType.Error);
            }
        }
    }
}