//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.SqlClient;
//using Microsoft.SqlServer.Server;

//public class StoredProcedures
//{
//    [SqlProcedure]
//    public static void TaskAuditTrail(string ProjectUID, string ModifiedBy)
//    {
//        //const string ProjectUID = "6b08e331-a75e-42f7-98ab-7eede9e4c9e0";
//        //const string ModifiedBy = "Administrator";
//        if (!string.IsNullOrEmpty(ProjectUID))
//        {
//            using (var connection_Pull = new SqlConnection("Context Connection=true"))
//            {
//                connection_Pull.Open();

//                string Qry;
//                var adapter = new SqlDataAdapter();
//                var resultSet = new DataSet();
//                var cmd = new SqlCommand();

//                cmd.Connection = connection_Pull;

//                cmd.CommandText = "SELECT name FROM master..sysdatabases where name='ITXBaseLineLogs'";
//                cmd.CommandType = CommandType.Text;
//                adapter.SelectCommand = cmd;
//                adapter.Fill(resultSet);
//                if (resultSet.Tables.Count > 0 && resultSet.Tables[0].Rows.Count == 0)
//                {
//                    Qry = @"CREATE database ITXBaseLineLogs";
//                    cmd.CommandText = Qry;
//                    try
//                    {
//                        cmd.ExecuteNonQuery();
//                    }
//                    catch (Exception)
//                    {
//                    }
//                }

//                cmd.CommandText = "select * from ITXBaseLineLogs.sys.tables where name='ITXBaseLineMaster'";
//                cmd.CommandType = CommandType.Text;
//                adapter.SelectCommand = cmd;
//                resultSet = new DataSet();
//                adapter.Fill(resultSet);
//                if (resultSet.Tables.Count > 0 && resultSet.Tables[0].Rows.Count == 0)
//                {
//                    Qry = @"CREATE TABLE ITXBaseLineLogs.[dbo].[ITXBaseLineMaster](
//                        	[ProjectUID] [text] NULL,
//	                        [TaskUID] [text] NULL,
//	                        [ModifiedBy] [text] NULL,
//	                        [ModifiedOn] [datetime] NULL,
//	                        [Idx] [bigint] IDENTITY(1,1) NOT NULL,";
//                    Qry = Qry.Substring(0, Qry.Length - 1);
//                    Qry += ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";
//                    cmd.CommandText = Qry;
//                    cmd.ExecuteNonQuery();
//                }

//                cmd.CommandText = "select * from ITXBaseLineLogs.sys.tables where name='ITXBaseLineSlave'";
//                cmd.CommandType = CommandType.Text;
//                adapter.SelectCommand = cmd;
//                resultSet = new DataSet();
//                adapter.Fill(resultSet);
//                if (resultSet.Tables.Count > 0 && resultSet.Tables[0].Rows.Count == 0)
//                {
//                    Qry = @"CREATE TABLE ITXBaseLineLogs.[dbo].[ITXBaseLineSlave](
//                            [Idx] [bigint] ,
//                            [TaskUID] [text] NULL,
//                        	[Field] [text] NULL,
//	                        [Value] [text] NULL)";
//                    cmd.CommandText = Qry;
//                    cmd.ExecuteNonQuery();
//                }

//                cmd.CommandText = "Select ";
//                for (int i = 0; i < 11; i++)
//                {
//                    cmd.CommandText += "TB_BASE_DUR_" + i + ",TB_BASE_START_" + i + ",TB_BASE_FINISH_" + i +
//                                       ",TB_BASE_WORK_" + i + ",";
//                }

//                //cmd.CommandText = cmd.CommandText.Substring(0, cmd.CommandText.Length - 1);
//                cmd.CommandText += "TASK_UID ";

//                cmd.CommandText += " from dbo.Task where Proj_UID='" + ProjectUID + "'";

//                cmd.CommandType = CommandType.Text;

//                adapter.SelectCommand = cmd;
//                resultSet = new DataSet();
//                adapter.Fill(resultSet);

//                string SelQry;
//                DataSet VerifyDs;
//                string SlaveQry = string.Empty;
//                Int64 Idx;
//                foreach (DataTable dataTable in resultSet.Tables)
//                {
//                    foreach (DataRow row in dataTable.Rows)
//                    {
//                        var Queries = new List<string>();

//                        for (int i = 0; i < 11; i++)
//                        {
//                            //    Field TB_BASE_DUR_* Starts
//                            SelQry = "select value from  ITXBaseLineLogs.dbo.ITXBaseLineSlave where TaskUID Like '" + row["TASK_UID"] + "' and Field Like '" + "TB_BASE_DUR_" + i;
//                            SelQry += "'  and Idx = (select max(Idx) from ITXBaseLineLogs.dbo.ITXBaseLineSlave where TaskUID Like '" + row["TASK_UID"] + "' and Field Like '" + "TB_BASE_DUR_" + i + "')";
//                            //SqlContext.Pipe.Send(SelQry);
//                            cmd.CommandText = SelQry;
//                            adapter.SelectCommand = cmd;
//                            VerifyDs = new DataSet();
//                            adapter.Fill(VerifyDs);
//                            if (VerifyDs.Tables.Count > 0 && VerifyDs.Tables[0].Rows.Count > 0)
//                            {
//                                if (VerifyDs.Tables[0].Rows[0][0].ToString() != row["TB_BASE_DUR_" + i].ToString())
//                                {
//                                    SlaveQry = "Insert into ITXBaseLineLogs.dbo.ITXBaseLineSlave values($Idx$,'" +
//                                               row["TASK_UID"] + "','TB_BASE_DUR_" + i + "','" +
//                                               VerifyDs.Tables[0].Rows[0][0].ToString() + "')";
//                                    Queries.Add(SlaveQry);
//                                }
//                            }
//                            else if (row["TB_BASE_DUR_" + i] != null && !string.IsNullOrEmpty(row["TB_BASE_DUR_" + i].ToString()))
//                            {
//                                SlaveQry = "Insert into ITXBaseLineLogs.dbo.ITXBaseLineSlave values($Idx$,'" +
//                                               row["TASK_UID"] + "','TB_BASE_DUR_" + i + "','" +
//                                               row["TB_BASE_DUR_" + i] + "')";
//                                Queries.Add(SlaveQry);
//                            }
//                            //Field TB_BASE_DUR_* Ends

//                            //Field TB_BASE_START_* Starts
//                            SelQry = "select value from  ITXBaseLineLogs.dbo.ITXBaseLineSlave where TaskUID Like '" + row["TASK_UID"] + "' and Field Like '" + "TB_BASE_START_" + i;
//                            SelQry += "'  and Idx = (select max(Idx) from ITXBaseLineLogs.dbo.ITXBaseLineSlave where TaskUID Like '" + row["TASK_UID"] + "' and Field Like '" + "TB_BASE_START_" + i + "')";
//                            cmd.CommandText = SelQry;
//                            adapter.SelectCommand = cmd;
//                            VerifyDs = new DataSet();
//                            adapter.Fill(VerifyDs);
//                            if (VerifyDs.Tables.Count > 0 && VerifyDs.Tables[0].Rows.Count > 0)
//                            {
//                                if (VerifyDs.Tables[0].Rows[0][0].ToString() != row["TB_BASE_START_" + i].ToString())
//                                {
//                                    SlaveQry = "Insert into ITXBaseLineLogs.dbo.ITXBaseLineSlave values($Idx$,'" +
//                                               row["TASK_UID"] + "','TB_BASE_START_" + i + "','" +
//                                               VerifyDs.Tables[0].Rows[0][0].ToString() + "')";
//                                    Queries.Add(SlaveQry);
//                                }
//                            }
//                            else if (row["TB_BASE_START_" + i] != null && !string.IsNullOrEmpty(row["TB_BASE_START_" + i].ToString()))
//                            {
//                                SlaveQry = "Insert into ITXBaseLineLogs.dbo.ITXBaseLineSlave values($Idx$,'" +
//                                               row["TASK_UID"] + "','TB_BASE_START_" + i + "','" +
//                                               row["TB_BASE_START_" + i] + "')";
//                                Queries.Add(SlaveQry);
//                            }
//                            //Field TB_BASE_START_* Ends

//                            //Field TB_BASE_FINISH_* Starts
//                            SelQry = "select value from  ITXBaseLineLogs.dbo.ITXBaseLineSlave where TaskUID Like '" + row["TASK_UID"] + "' and Field Like '" + "TB_BASE_FINISH_" + i;
//                            SelQry += "'  and Idx = (select max(Idx) from ITXBaseLineLogs.dbo.ITXBaseLineSlave where TaskUID Like '" + row["TASK_UID"] + "' and Field Like '" + "TB_BASE_FINISH_" + i + "')";
//                            cmd.CommandText = SelQry;
//                            adapter.SelectCommand = cmd;
//                            VerifyDs = new DataSet();
//                            adapter.Fill(VerifyDs);
//                            if (VerifyDs.Tables.Count > 0 && VerifyDs.Tables[0].Rows.Count > 0)
//                            {
//                                if (VerifyDs.Tables[0].Rows[0][0].ToString() != row["TB_BASE_FINISH_" + i].ToString())
//                                {
//                                    SlaveQry = "Insert into ITXBaseLineLogs.dbo.ITXBaseLineSlave values($Idx$,'" +
//                                               row["TASK_UID"] + "','TB_BASE_FINISH_" + i + "','" +
//                                               VerifyDs.Tables[0].Rows[0][0].ToString() + "')";
//                                    Queries.Add(SlaveQry);
//                                }
//                            }
//                            else if (row["TB_BASE_FINISH_" + i] != null && !string.IsNullOrEmpty(row["TB_BASE_FINISH_" + i].ToString()))
//                            {
//                                SlaveQry = "Insert into ITXBaseLineLogs.dbo.ITXBaseLineSlave values($Idx$,'" +
//                                               row["TASK_UID"] + "','TB_BASE_FINISH_" + i + "','" +
//                                               row["TB_BASE_FINISH_" + i] + "')";
//                                Queries.Add(SlaveQry);
//                            }
//                            //Field TB_BASE_FINISH_* Ends

//                            //Field TB_BASE_WORK_* Starts
//                            SelQry = "select value from  ITXBaseLineLogs.dbo.ITXBaseLineSlave where TaskUID Like '" + row["TASK_UID"] + "' and Field Like '" + "TB_BASE_WORK_" + i;
//                            SelQry += "'  and Idx = (select max(Idx) from ITXBaseLineLogs.dbo.ITXBaseLineSlave where TaskUID Like '" + row["TASK_UID"] + "' and Field Like '" + "TB_BASE_WORK_" + i + "')";
//                            cmd.CommandText = SelQry;
//                            adapter.SelectCommand = cmd;
//                            VerifyDs = new DataSet();
//                            adapter.Fill(VerifyDs);
//                            if (VerifyDs.Tables.Count > 0 && VerifyDs.Tables[0].Rows.Count > 0)
//                            {
//                                if (VerifyDs.Tables[0].Rows[0][0].ToString() != row["TB_BASE_WORK_" + i].ToString())
//                                {
//                                    SlaveQry = "Insert into ITXBaseLineLogs.dbo.ITXBaseLineSlave values($Idx$,'" +
//                                               row["TASK_UID"] + "','TB_BASE_WORK_" + i + "','" +
//                                               VerifyDs.Tables[0].Rows[0][0].ToString() + "')";
//                                    Queries.Add(SlaveQry);
//                                }
//                            }
//                            else if (row["TB_BASE_WORK_" + i] != null && !string.IsNullOrEmpty(row["TB_BASE_WORK_" + i].ToString()))
//                            {
//                                SlaveQry = "Insert into ITXBaseLineLogs.dbo.ITXBaseLineSlave values($Idx$,'" +
//                                               row["TASK_UID"] + "','TB_BASE_WORK_" + i + "','" +
//                                               row["TB_BASE_WORK_" + i] + "')";
//                                Queries.Add(SlaveQry);
//                            }
//                            //Field TB_BASE_WORK_* Ends
//                        }
//                        if (Queries.Count > 0)
//                        {
//                            Idx = 1;
//                            Qry =
//                                "insert into ITXBaseLineLogs.dbo.ITXBaseLineMaster ([ProjectUID],[TaskUID],[ModifiedBy],[ModifiedOn])";
//                            Qry += " values('" + ProjectUID + "','" + row["TASK_UID"] + "','" + ModifiedBy + "','" +
//                                   DateTime.Now + "')";
//                            cmd.CommandText = Qry;
//                            cmd.ExecuteNonQuery();

//                            Qry = "Select max(Idx) from ITXBaseLineLogs.dbo.ITXBaseLineMaster";
//                            cmd.CommandText = Qry;
//                            adapter.SelectCommand = cmd;
//                            VerifyDs = new DataSet();
//                            adapter.Fill(VerifyDs);
//                            if (VerifyDs.Tables.Count > 0 && VerifyDs.Tables[0].Rows.Count > 0)
//                            {
//                                if (VerifyDs.Tables[0].Rows[0][0] != null)
//                                {
//                                    Idx = Convert.ToInt64(VerifyDs.Tables[0].Rows[0][0]);
//                                }
//                            }

//                            foreach (string query in Queries)
//                            {
//                                cmd.CommandText = query.Replace("$Idx$", Idx.ToString());
//                                cmd.ExecuteNonQuery();
//                            }
//                        }
//                    }
//                }
//            }
//        }
//    }
//}