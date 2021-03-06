﻿/*using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.SqlServer.Server;

public class StoredProcedures
{
    [SqlProcedure]
    public static void TaskAuditTrail(string ProjectUID, string ModifiedBy)
    {
        //const string ProjectUID = "6b08e331-a75e-42f7-98ab-7eede9e4c9e0";
        //const string ModifiedBy = "Administrator";
        if (!string.IsNullOrEmpty(ProjectUID))
        {
            using (var connection_Pull = new SqlConnection("Context Connection=true"))
            {
                connection_Pull.Open();

                string Qry;
                var adapter = new SqlDataAdapter();
                var resultSet = new DataSet();
                var cmd = new SqlCommand();

                cmd.Connection = connection_Pull;

                cmd.CommandText = "SELECT name FROM master..sysdatabases where name='ITXBaseLineLogs'";
                cmd.CommandType = CommandType.Text;
                adapter.SelectCommand = cmd;
                adapter.Fill(resultSet);
                if (resultSet.Tables.Count > 0 && resultSet.Tables[0].Rows.Count == 0)
                {
                    Qry = @"CREATE database ITXBaseLineLogs";
                    cmd.CommandText = Qry;
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                    }
                }

                cmd.CommandText = "select * from ITXBaseLineLogs.sys.tables where name='ITXBaseLineMaster'";
                cmd.CommandType = CommandType.Text;
                adapter.SelectCommand = cmd;
                resultSet = new DataSet();
                adapter.Fill(resultSet);
                if (resultSet.Tables.Count > 0 && resultSet.Tables[0].Rows.Count == 0)
                {
                    Qry = @"CREATE TABLE ITXBaseLineLogs.[dbo].[ITXBaseLineMaster](
                        	[ProjectUID] [text] NULL,
	                        [TaskUID] [text] NULL,
	                        [ModifiedBy] [text] NULL,
	                        [ModifiedOn] [datetime] NULL,
	                        [Idx] [bigint] IDENTITY(1,1) NOT NULL,";
                    for (int i = 0; i < 11; i++)
                    {
                        Qry += "[TB_BASE_DUR_" + i + "] [bigint] NULL,";
                        Qry += "[TB_BASE_START_" + i + "] [datetime] NULL,";
                        Qry += "[TB_BASE_FINISH_" + i + "] [datetime] NULL,";
                        Qry += "[TB_BASE_WORK_" + i + "]  [text] NULL,";
                    }
                    Qry = Qry.Substring(0, Qry.Length - 1);
                    Qry += ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";
                    cmd.CommandText = Qry;
                    cmd.ExecuteNonQuery();
                }

                cmd.CommandText = "Select ";
                for (int i = 0; i < 11; i++)
                {
                    cmd.CommandText += "TB_BASE_DUR_" + i + ",TB_BASE_START_" + i + ",TB_BASE_FINISH_" + i +
                                       ",TB_BASE_WORK_" + i + ",";
                }

                //cmd.CommandText = cmd.CommandText.Substring(0, cmd.CommandText.Length - 1);
                cmd.CommandText += "TASK_UID ";

                cmd.CommandText += " from dbo.Task where Proj_UID='" + ProjectUID + "'";

                cmd.CommandType = CommandType.Text;

                adapter.SelectCommand = cmd;
                resultSet = new DataSet();
                adapter.Fill(resultSet);

                string SelQry;
                DataSet VerifyDs;
                bool IsChangesFound = false;
                foreach (DataTable dataTable in resultSet.Tables)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        Qry = "insert into ITXBaseLineLogs.dbo.ITXBaseLineMaster ([ProjectUID],[TaskUID],[ModifiedBy],[ModifiedOn],";
                        for (int i = 0; i < 11; i++)
                        {
                            Qry += "[TB_BASE_DUR_" + i + "],";
                            Qry += "[TB_BASE_START_" + i + "],";
                            Qry += "[TB_BASE_FINISH_" + i + "],";
                            Qry += "[TB_BASE_WORK_" + i + "],";
                        }
                        Qry = Qry.Substring(0, Qry.Length - 1) + ") values('" + ProjectUID + "','" + row["TASK_UID"] + "','" + ModifiedBy + "','" + DateTime.Now + "',";

                        for (int i = 0; i < 11; i++)
                        {
                            //Field TB_BASE_DUR_* Starts
                            SelQry = "select TB_BASE_DUR_" + i + " from ITXBaseLineLogs.dbo.ITXBaseLineMaster where ";
                            SelQry += " TaskUID LIKE '" + row["TASK_UID"] + "' And Idx = (select Max(Idx) from ITXBaseLineLogs.dbo.ITXBaseLineMaster where TaskUID LIKE '" + row["TASK_UID"] + "')";
                            SqlContext.Pipe.Send(SelQry);
                            cmd.CommandText = SelQry;
                            adapter.SelectCommand = cmd;
                            VerifyDs = new DataSet();
                            adapter.Fill(VerifyDs);
                            if (VerifyDs.Tables.Count > 0 && VerifyDs.Tables[0].Rows.Count > 0)
                            {
                                if (VerifyDs.Tables[0].Rows[0][0].ToString() != row["TB_BASE_DUR_" + i].ToString())
                                {
                                    Qry += "'" + VerifyDs.Tables[0].Rows[0][0] + "',";
                                    IsChangesFound = true;
                                }
                                else
                                    Qry += "'',";
                            }
                            else
                            {
                                Qry += "'" + row["TB_BASE_DUR_" + i] + "',";
                                IsChangesFound = true;
                            }
                            //Field TB_BASE_DUR_* Ends

                            //Field TB_BASE_START_* Starts
                            SelQry = "select TB_BASE_START_" + i + " from ITXBaseLineLogs.dbo.ITXBaseLineMaster where ";
                            SelQry += " TaskUID LIKE '" + row["TASK_UID"] + "' And Idx = (select Max(Idx) from ITXBaseLineLogs.dbo.ITXBaseLineMaster where TaskUID LIKE '" + row["TASK_UID"] + "')";
                            cmd.CommandText = SelQry;
                            adapter.SelectCommand = cmd;
                            VerifyDs = new DataSet();
                            adapter.Fill(VerifyDs);
                            if (VerifyDs.Tables.Count > 0 && VerifyDs.Tables[0].Rows.Count > 0)
                            {
                                if (VerifyDs.Tables[0].Rows[0][0].ToString() != row["TB_BASE_START_" + i].ToString())
                                {
                                    Qry += "'" + VerifyDs.Tables[0].Rows[0][0] + "',";
                                    IsChangesFound = true;
                                }
                                else
                                    Qry += "'',";
                            }
                            else
                            {
                                Qry += "'" + row["TB_BASE_START_" + i] + "',";
                                IsChangesFound = true;
                            }
                            //Field TB_BASE_START_* Ends

                            //Field TB_BASE_FINISH_* Starts
                            SelQry = "select TB_BASE_FINISH_" + i + " from ITXBaseLineLogs.dbo.ITXBaseLineMaster where ";
                            SelQry += " TaskUID LIKE '" + row["TASK_UID"] + "' And  Idx = (select Max(Idx) from ITXBaseLineLogs.dbo.ITXBaseLineMaster where TaskUID LIKE '" + row["TASK_UID"] + "')";
                            cmd.CommandText = SelQry;
                            adapter.SelectCommand = cmd;
                            VerifyDs = new DataSet();
                            adapter.Fill(VerifyDs);
                            if (VerifyDs.Tables.Count > 0 && VerifyDs.Tables[0].Rows.Count > 0)
                            {
                                if (VerifyDs.Tables[0].Rows[0][0].ToString() != row["TB_BASE_FINISH_" + i].ToString())
                                {
                                    Qry += "'" + VerifyDs.Tables[0].Rows[0][0] + "',";
                                    IsChangesFound = true;
                                }
                                else
                                    Qry += "'',";
                            }
                            else
                            {
                                Qry += "'" + row["TB_BASE_FINISH_" + i] + "',";
                                IsChangesFound = true;
                            }
                            //Field TB_BASE_FINISH_* Ends

                            //Field TB_BASE_WORK_* Starts
                            SelQry = "select TB_BASE_WORK_" + i + " from ITXBaseLineLogs.dbo.ITXBaseLineMaster where ";
                            SelQry += " TaskUID LIKE '" + row["TASK_UID"] + "' And Idx = (select Max(Idx) from ITXBaseLineLogs.dbo.ITXBaseLineMaster where TaskUID LIKE '" + row["TASK_UID"] + "')";
                            cmd.CommandText = SelQry;
                            adapter.SelectCommand = cmd;
                            VerifyDs = new DataSet();
                            adapter.Fill(VerifyDs);
                            if (VerifyDs.Tables.Count > 0 && VerifyDs.Tables[0].Rows.Count > 0)
                            {
                                if (VerifyDs.Tables[0].Rows[0][0].ToString() != row["TB_BASE_WORK_" + i].ToString())
                                {
                                    Qry += "'" + VerifyDs.Tables[0].Rows[0][0] + "',";
                                    IsChangesFound = true;
                                }
                                else
                                    Qry += "'',";
                            }
                            else
                            {
                                string value = string.Empty;
                                try
                                {
                                    value = Convert.ToDouble(row["TB_BASE_WORK_" + i]).ToString("N2");
                                }
                                catch (Exception)
                                { }
                                Qry += "'" + value + "',";
                                IsChangesFound = true;
                            }
                            //Field TB_BASE_WORK_* Ends
                        }
                        if (IsChangesFound)
                        {
                            Qry = Qry.Substring(0, Qry.Length - 1) + ")";
                            cmd.CommandText = Qry;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
    }
} */