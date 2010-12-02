using System;
using System.Data;
using System.Data.SqlClient;
using ITXProjectsLibrary;
using Microsoft.SharePoint;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var Site = new SPSite("http://epm2007demo/pwa01"))
            {
                string ConnectionString = Utilities.GetProjectServerSQLDatabaseConnectionString(Site.ID, Utilities.DatabaseType.PublishedDatabase);
                using (var sqlConnection = new SqlConnection(ConnectionString.Replace("15", "60")))
                {
                    sqlConnection.Open();
                }
            }

            return;
            using (var site = new SPSite("http://epm2007demo/pwa01"))
            {
                var query = new SPSiteDataQuery();
            }

            return;
            using (SPSite Site = new SPSite("http://epm2007demo/pwa02"))
            {
                string constr = Utilities.GetProjectServerSQLDatabaseConnectionString(Site.ID,
                                                                                      Utilities.DatabaseType.
                                                                                          PublishedDatabase);
                using (SqlConnection connection = new SqlConnection(constr))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = connection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("ProjectUID", "6b08e331-a75e-42f7-98ab-7eede9e4c9e0"));
                    cmd.Parameters.Add(new SqlParameter("ModifiedBy", "Administrator"));
                    cmd.CommandText = "TaskAuditTrail";
                    cmd.ExecuteNonQuery();
                }
            }
            return;
            SqlConnection connection_Push =
                new SqlConnection("Server=epm2007demo;Database=ITXBaseLineLogs;Trusted_Connection=True;");
            connection_Push.Open();
            SqlConnection connection_Pull = null;
            //SqlConnection connection_Push = null;
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            SqlCommand command = new SqlCommand();
            DataSet resultSet = new DataSet();
            string Qry = string.Empty;
            try
            {
                using (SPSite Site = new SPSite("http://epm2007demo/pwa02"))
                {
                    Guid Project_UID =
                        new Guid(Site.AllWebs["Project1"].AllProperties[Utilities.ProjWSSUIDProperty].ToString());

                    connection_Pull =
                        new SqlConnection(Utilities.GetProjectServerSQLDatabaseConnectionString(Site.ID,
                                                                                                Utilities.
                                                                                                    DatabaseType.
                                                                                                    PublishedDatabase));

                    connection_Pull.Open();

                    connection_Push =
                        new SqlConnection(Utilities.GetProjectServerSQLDatabaseConnectionString(Site.ID,
                                                                                                Utilities.
                                                                                                    DatabaseType.
                                                                                                    ReportingDatabase));

                    connection_Push.Open();

                    command.Connection = connection_Pull;
                    command.CommandText = "Select ";

                    for (int i = 0; i < 11; i++)
                    {
                        command.CommandText += "TB_BASE_DUR_" + i + ",TB_BASE_DUR_IS_ESTIMATE_" + i +
                                               ",TB_BASE_START_" + i + ",TB_BASE_FINISH_" + i + ",TB_BASE_WORK_" + i +
                                               ",TB_BASE_COST_" + i + ",TB_BASE_BUDGET_WORK_" + i +
                                               ",TB_BASE_BUDGET_COST_" + i + ",";
                    }

                    command.CommandText = command.CommandText.Substring(0, command.CommandText.Length - 1);

                    command.CommandText += " from ProjectSummaries where PROJ_UID='" + Project_UID.ToString() + "'";

                    dataAdapter.SelectCommand = command;

                    dataAdapter.Fill(resultSet);

                    if (resultSet.Tables.Count > 0 && resultSet.Tables[0].Rows.Count > 0)
                    {
                        Qry = "Select * from ITXBaseLineDetail where ";
                    }
                    else
                    {
                        // Log says that empty in Publish DB
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                if (connection_Pull != null && connection_Pull.State == ConnectionState.Open)
                    connection_Pull.Close();
                if (connection_Push != null && connection_Push.State == ConnectionState.Open)
                    connection_Push.Close();
            }
        }
    }
}