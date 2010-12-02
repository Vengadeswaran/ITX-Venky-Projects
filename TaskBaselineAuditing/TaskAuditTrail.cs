using System;
using System.Data;
using System.Data.SqlClient;

public partial class StoredProcedures
{
    [Microsoft.SqlServer.Server.SqlProcedure]
    public static void ITXTaskAuditTrail(string ProjectUID, string ModifiedBy)
    {
        // Put your code here
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
                        	[ProjectUID] [uniqueidentifier] NULL,
	                        [TaskUID] [uniqueidentifier] NULL,
	                        [ModifiedBy] [text] NULL,
	                        [ModifiedOn] [datetime] NULL,
	                        [Idx] [bigint] IDENTITY(1,1) NOT NULL,";
                    for (int i = 0; i < 11; i++)
                    {
                        Qry += "[TB_BASE_DUR_" + i + "] [bigint] NULL,";
                        Qry += "[TB_BASE_START_" + i + "] [datetime] NULL,";
                        Qry += "[TB_BASE_FINISH_" + i + "] [datetime] NULL,";
                        Qry += "[TB_BASE_WORK_" + i + "]  [decimal] NULL,";
                    }
                    Qry = Qry.Substring(0, Qry.Length - 1);
                    Qry += ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";
                    cmd.CommandText = Qry;
                    cmd.ExecuteNonQuery();
                }
                cmd.CommandText = "select * from ITXBaseLineLogs.sys.tables where name='SHADOW_ITXBaseLineMaster'";
                cmd.CommandType = CommandType.Text;
                adapter.SelectCommand = cmd;
                resultSet = new DataSet();
                adapter.Fill(resultSet);
                if (resultSet.Tables.Count > 0 && resultSet.Tables[0].Rows.Count == 0)
                {
                    Qry = @"CREATE TABLE ITXBaseLineLogs.[dbo].[SHADOW_ITXBaseLineMaster](
                        	[ProjectUID] [uniqueidentifier] NULL,
	                        [TaskUID] [uniqueidentifier] NULL,";
                    for (int i = 0; i < 11; i++)
                    {
                        Qry += "[TB_BASE_DUR_" + i + "] [bigint] NULL,";
                        Qry += "[TB_BASE_START_" + i + "] [datetime] NULL,";
                        Qry += "[TB_BASE_FINISH_" + i + "] [datetime] NULL,";
                        Qry += "[TB_BASE_WORK_" + i + "]  [decimal] NULL,";
                    }
                    Qry = Qry.Substring(0, Qry.Length - 1);
                    Qry += ")";
                    cmd.CommandText = Qry;
                    cmd.ExecuteNonQuery();
                }

                // Find the project has any base line value and number of tasks if it has baseline value
                cmd.CommandText = @"select count(*) AS NumTasks from task where ([TB_BASE_DUR_0]IS NOT NULL OR [TB_BASE_START_0]IS NOT NULL OR
                                    [TB_BASE_FINISH_0]IS NOT NULL OR [TB_BASE_WORK_0]IS NOT NULL OR [TB_BASE_DUR_1]IS NOT NULL OR
                                    [TB_BASE_START_1]IS NOT NULL OR [TB_BASE_FINISH_1]IS NOT NULL OR [TB_BASE_WORK_1]IS NOT NULL OR
                                    [TB_BASE_DUR_2]IS NOT NULL OR [TB_BASE_START_2]IS NOT NULL OR [TB_BASE_FINISH_2]IS NOT NULL OR
                                    [TB_BASE_WORK_2]IS NOT NULL OR [TB_BASE_DUR_3]IS NOT NULL OR [TB_BASE_START_3]IS NOT NULL OR
                                    [TB_BASE_FINISH_3]IS NOT NULL OR [TB_BASE_WORK_3]IS NOT NULL OR [TB_BASE_DUR_4]IS NOT NULL OR
                                    [TB_BASE_START_4]IS NOT NULL OR [TB_BASE_FINISH_4]IS NOT NULL OR [TB_BASE_WORK_4]IS NOT NULL OR
                                    [TB_BASE_DUR_5]IS NOT NULL OR [TB_BASE_START_5]IS NOT NULL OR [TB_BASE_FINISH_5]IS NOT NULL OR
                                    [TB_BASE_WORK_5]IS NOT NULL OR [TB_BASE_DUR_6]IS NOT NULL OR [TB_BASE_START_6]IS NOT NULL OR
                                    [TB_BASE_FINISH_6]IS NOT NULL OR [TB_BASE_WORK_6]IS NOT NULL OR [TB_BASE_DUR_7]IS NOT NULL OR
                                    [TB_BASE_START_7]IS NOT NULL OR [TB_BASE_FINISH_7]IS NOT NULL OR [TB_BASE_WORK_7]IS NOT NULL OR
                                    [TB_BASE_DUR_8]IS NOT NULL OR [TB_BASE_START_8]IS NOT NULL OR [TB_BASE_FINISH_8]IS NOT NULL OR
                                    [TB_BASE_WORK_8]IS NOT NULL OR [TB_BASE_DUR_9]IS NOT NULL OR [TB_BASE_START_9]IS NOT NULL OR
                                    [TB_BASE_FINISH_9]IS NOT NULL OR [TB_BASE_WORK_9]IS NOT NULL OR [TB_BASE_DUR_10]IS NOT NULL OR
                                    [TB_BASE_START_10]IS NOT NULL OR [TB_BASE_FINISH_10]IS NOT NULL OR [TB_BASE_WORK_10]IS NOT NULL) AND
                                    (Proj_UID = '" + ProjectUID + "')";
                cmd.CommandType = CommandType.Text;
                adapter.SelectCommand = cmd;
                resultSet = new DataSet();
                adapter.Fill(resultSet);
                foreach (DataRow trow in resultSet.Tables[0].Rows)
                {
                    if (resultSet.Tables[0].Rows.Count > 0)
                    {
                        if (Convert.ToInt32(trow["NumTasks"].ToString()) > 0)
                        {
                            cmd.CommandText = "SELECT COUNT(*) AS BlHistory from ITXBaseLineLogs.dbo.ITXBaseLineMaster WHERE ProjectUID = '" + ProjectUID + "'";
                            cmd.CommandType = CommandType.Text;
                            adapter.SelectCommand = cmd;
                            resultSet = new DataSet();
                            adapter.Fill(resultSet);
                            var selqry = new DataSet();
                            var insqry = new DataSet();
                            if (Convert.ToInt32(resultSet.Tables[0].Rows[0]["BlHistory"].ToString()) > 0)
                            {
                                cmd.CommandText = @"
                                SELECT TASK_UID,BField, BFValue
                                INTO #TMPDB
                                FROM (
                                SELECT	t1.PROJ_UID, t1.TASK_UID,
	                                CONVERT(CHAR, CASE WHEN (t1.TB_BASE_DUR_0 = t2.TB_BASE_DUR_0) THEN NULL ELSE t1.TB_BASE_DUR_0 END) AS TB_BASE_DUR_0,
                                        CONVERT(CHAR, CASE WHEN (t1.TB_BASE_START_0 = t2.TB_BASE_START_0) THEN NULL ELSE t1.TB_BASE_START_0 END) AS TB_BASE_START_0,
                                        CONVERT(CHAR, CASE WHEN (t1.TB_BASE_FINISH_0 = t2.TB_BASE_FINISH_0) THEN NULL ELSE t1.TB_BASE_FINISH_0 END) AS TB_BASE_FINISH_0,
                                        CONVERT(CHAR, CASE WHEN (t1.TB_BASE_WORK_0 = t2.TB_BASE_WORK_0) THEN NULL ELSE t1.TB_BASE_WORK_0 END) AS TB_BASE_WORK_0,
                                        CONVERT(CHAR, CASE WHEN (t1.TB_BASE_DUR_1 = t2.TB_BASE_DUR_1) THEN NULL ELSE t1.TB_BASE_DUR_1 END) AS TB_BASE_DUR_1,
                                        CONVERT(CHAR, CASE WHEN (t1.TB_BASE_START_1 = t2.TB_BASE_START_1) THEN NULL ELSE t1.TB_BASE_START_1 END) AS TB_BASE_START_1,
                                        CONVERT(CHAR, CASE WHEN (t1.TB_BASE_FINISH_1 = t2.TB_BASE_FINISH_1) THEN NULL ELSE t1.TB_BASE_FINISH_1 END) AS TB_BASE_FINISH_1,
                                        CONVERT(CHAR, CASE WHEN (t1.TB_BASE_WORK_1 = t2.TB_BASE_WORK_1) THEN NULL ELSE t1.TB_BASE_WORK_1 END) AS TB_BASE_WORK_1,
                                        CONVERT(CHAR, CASE WHEN (t1.TB_BASE_DUR_2 = t2.TB_BASE_DUR_2) THEN NULL ELSE t1.TB_BASE_DUR_2 END) AS TB_BASE_DUR_2,
                                        CONVERT(CHAR, CASE WHEN (t1.TB_BASE_START_2 = t2.TB_BASE_START_2) THEN NULL ELSE t1.TB_BASE_START_2 END) AS TB_BASE_START_2,
                                        CONVERT(CHAR, CASE WHEN (t1.TB_BASE_FINISH_2 = t2.TB_BASE_FINISH_2) THEN NULL ELSE t1.TB_BASE_FINISH_2 END) AS TB_BASE_FINISH_2,
                                        CONVERT(CHAR, CASE WHEN (t1.TB_BASE_WORK_2 = t2.TB_BASE_WORK_2) THEN NULL ELSE t1.TB_BASE_WORK_2 END) AS TB_BASE_WORK_2,
                                        CONVERT(CHAR, CASE WHEN (t1.TB_BASE_DUR_3 = t2.TB_BASE_DUR_3) THEN NULL ELSE t1.TB_BASE_DUR_3 END) AS TB_BASE_DUR_3,
                                        CONVERT(CHAR, CASE WHEN (t1.TB_BASE_START_3 = t2.TB_BASE_START_3) THEN NULL ELSE t1.TB_BASE_START_3 END) AS TB_BASE_START_3,
                                        CONVERT(CHAR, CASE WHEN (t1.TB_BASE_FINISH_3 = t2.TB_BASE_FINISH_3) THEN NULL ELSE t1.TB_BASE_FINISH_3 END) AS TB_BASE_FINISH_3,
                                        CONVERT(CHAR, CASE WHEN (t1.TB_BASE_WORK_3 = t2.TB_BASE_WORK_3) THEN NULL ELSE t1.TB_BASE_WORK_3 END) AS TB_BASE_WORK_3,
                                        CONVERT(CHAR, CASE WHEN (t1.TB_BASE_DUR_4 = t2.TB_BASE_DUR_4) THEN NULL ELSE t1.TB_BASE_DUR_4 END) AS TB_BASE_DUR_4,
                                        CONVERT(CHAR, CASE WHEN (t1.TB_BASE_START_4 = t2.TB_BASE_START_4) THEN NULL ELSE t1.TB_BASE_START_4 END) AS TB_BASE_START_4,
                                        CONVERT(CHAR, CASE WHEN (t1.TB_BASE_FINISH_4 = t2.TB_BASE_FINISH_4) THEN NULL ELSE t1.TB_BASE_FINISH_4 END) AS TB_BASE_FINISH_4,
                                        CONVERT(CHAR, CASE WHEN (t1.TB_BASE_WORK_4 = t2.TB_BASE_WORK_4) THEN NULL ELSE t1.TB_BASE_WORK_4 END) AS TB_BASE_WORK_4,
                                        CONVERT(CHAR, CASE WHEN (t1.TB_BASE_DUR_5 = t2.TB_BASE_DUR_5) THEN NULL ELSE t1.TB_BASE_DUR_5 END) AS TB_BASE_DUR_5,
                                        CONVERT(CHAR, CASE WHEN (t1.TB_BASE_START_5 = t2.TB_BASE_START_5) THEN NULL ELSE t1.TB_BASE_START_5 END) AS TB_BASE_START_5,
                                        CONVERT(CHAR, CASE WHEN (t1.TB_BASE_FINISH_5 = t2.TB_BASE_FINISH_5) THEN NULL ELSE t1.TB_BASE_FINISH_5 END) AS TB_BASE_FINISH_5,
                                        CONVERT(CHAR, CASE WHEN (t1.TB_BASE_WORK_5 = t2.TB_BASE_WORK_5) THEN NULL ELSE t1.TB_BASE_WORK_5 END) AS TB_BASE_WORK_5,
                                        CONVERT(CHAR, CASE WHEN (t1.TB_BASE_DUR_6 = t2.TB_BASE_DUR_6) THEN NULL ELSE t1.TB_BASE_DUR_6 END) AS TB_BASE_DUR_6,
                                        CONVERT(CHAR, CASE WHEN (t1.TB_BASE_START_6 = t2.TB_BASE_START_6) THEN NULL ELSE t1.TB_BASE_START_6 END) AS TB_BASE_START_6,
                                        CONVERT(CHAR, CASE WHEN (t1.TB_BASE_FINISH_6 = t2.TB_BASE_FINISH_6) THEN NULL ELSE t1.TB_BASE_FINISH_6 END) AS TB_BASE_FINISH_6,
                                        CONVERT(CHAR, CASE WHEN (t1.TB_BASE_WORK_6 = t2.TB_BASE_WORK_6) THEN NULL ELSE t1.TB_BASE_WORK_6 END) AS TB_BASE_WORK_6,
                                        CONVERT(CHAR, CASE WHEN (t1.TB_BASE_DUR_7 = t2.TB_BASE_DUR_7) THEN NULL ELSE t1.TB_BASE_DUR_7 END) AS TB_BASE_DUR_7,
                                        CONVERT(CHAR, CASE WHEN (t1.TB_BASE_START_7 = t2.TB_BASE_START_7) THEN NULL ELSE t1.TB_BASE_START_7 END) AS TB_BASE_START_7,
                                        CONVERT(CHAR, CASE WHEN (t1.TB_BASE_FINISH_7 = t2.TB_BASE_FINISH_7) THEN NULL ELSE t1.TB_BASE_FINISH_7 END) AS TB_BASE_FINISH_7,
                                        CONVERT(CHAR, CASE WHEN (t1.TB_BASE_WORK_7 = t2.TB_BASE_WORK_7) THEN NULL ELSE t1.TB_BASE_WORK_7 END) AS TB_BASE_WORK_7,
                                        CONVERT(CHAR, CASE WHEN (t1.TB_BASE_DUR_8 = t2.TB_BASE_DUR_8) THEN NULL ELSE t1.TB_BASE_DUR_8 END) AS TB_BASE_DUR_8,
                                        CONVERT(CHAR, CASE WHEN (t1.TB_BASE_START_8 = t2.TB_BASE_START_8) THEN NULL ELSE t1.TB_BASE_START_8 END) AS TB_BASE_START_8,
                                        CONVERT(CHAR, CASE WHEN (t1.TB_BASE_FINISH_8 = t2.TB_BASE_FINISH_8) THEN NULL ELSE t1.TB_BASE_FINISH_8 END) AS TB_BASE_FINISH_8,
                                        CONVERT(CHAR, CASE WHEN (t1.TB_BASE_WORK_8 = t2.TB_BASE_WORK_8) THEN NULL ELSE t1.TB_BASE_WORK_8 END) AS TB_BASE_WORK_8,
                                        CONVERT(CHAR, CASE WHEN (t1.TB_BASE_DUR_9 = t2.TB_BASE_DUR_9) THEN NULL ELSE t1.TB_BASE_DUR_9 END) AS TB_BASE_DUR_9,
                                        CONVERT(CHAR, CASE WHEN (t1.TB_BASE_START_9 = t2.TB_BASE_START_9) THEN NULL ELSE t1.TB_BASE_START_9 END) AS TB_BASE_START_9,
                                        CONVERT(CHAR, CASE WHEN (t1.TB_BASE_FINISH_9 = t2.TB_BASE_FINISH_9) THEN NULL ELSE t1.TB_BASE_FINISH_9 END) AS TB_BASE_FINISH_9,
                                        CONVERT(CHAR, CASE WHEN (t1.TB_BASE_WORK_9 = t2.TB_BASE_WORK_9) THEN NULL ELSE t1.TB_BASE_WORK_9 END) AS TB_BASE_WORK_9,
                                        CONVERT(CHAR, CASE WHEN (t1.TB_BASE_DUR_10 = t2.TB_BASE_DUR_10) THEN NULL ELSE t1.TB_BASE_DUR_10 END) AS TB_BASE_DUR_10,
                                        CONVERT(CHAR, CASE WHEN (t1.TB_BASE_START_10 = t2.TB_BASE_START_10) THEN NULL ELSE t1.TB_BASE_START_10 END) AS TB_BASE_START_10,
                                        CONVERT(CHAR, CASE WHEN (t1.TB_BASE_FINISH_10 = t2.TB_BASE_FINISH_10) THEN NULL ELSE t1.TB_BASE_FINISH_10 END) AS TB_BASE_FINISH_10,
                                        CONVERT(CHAR, CASE WHEN (t1.TB_BASE_WORK_10 = t2.TB_BASE_WORK_10) THEN NULL ELSE t1.TB_BASE_WORK_10 END) AS TB_BASE_WORK_10
                                FROM    Task AS t1 INNER JOIN
                                        (SELECT     ProjectUID, TaskUID, TB_BASE_DUR_0, TB_BASE_START_0, TB_BASE_FINISH_0, TB_BASE_WORK_0, TB_BASE_DUR_1,
                                                    TB_BASE_START_1, TB_BASE_FINISH_1, TB_BASE_WORK_1, TB_BASE_DUR_2, TB_BASE_START_2, TB_BASE_FINISH_2,
                                                    TB_BASE_WORK_2, TB_BASE_DUR_3, TB_BASE_START_3, TB_BASE_FINISH_3, TB_BASE_WORK_3, TB_BASE_DUR_4,
                                                    TB_BASE_START_4, TB_BASE_FINISH_4, TB_BASE_WORK_4, TB_BASE_DUR_5, TB_BASE_START_5, TB_BASE_FINISH_5,
                                                    TB_BASE_WORK_5, TB_BASE_DUR_6, TB_BASE_START_6, TB_BASE_FINISH_6, TB_BASE_WORK_6, TB_BASE_DUR_7,
                                                    TB_BASE_START_7, TB_BASE_FINISH_7, TB_BASE_WORK_7, TB_BASE_DUR_8, TB_BASE_START_8, TB_BASE_FINISH_8,
                                                    TB_BASE_WORK_8, TB_BASE_DUR_9, TB_BASE_START_9, TB_BASE_FINISH_9, TB_BASE_WORK_9, TB_BASE_DUR_10,
                                                    TB_BASE_START_10, TB_BASE_FINISH_10, TB_BASE_WORK_10
                                        FROM        ITXBaseLineLogs.dbo.SHADOW_ITXBaseLineMaster) AS t2 ON t2.ProjectUID = t1.PROJ_UID AND t1.TASK_UID = t2.TaskUID
                                WHERE   (PROJ_UID = '" + ProjectUID + @"')
                                ) pvt
                                UNPIVOT
                                (BFValue For BField IN (
					                                TB_BASE_DUR_0, TB_BASE_START_0, TB_BASE_FINISH_0, TB_BASE_WORK_0, TB_BASE_DUR_1,
                                                    TB_BASE_START_1, TB_BASE_FINISH_1, TB_BASE_WORK_1, TB_BASE_DUR_2, TB_BASE_START_2, TB_BASE_FINISH_2,
                                                    TB_BASE_WORK_2, TB_BASE_DUR_3, TB_BASE_START_3, TB_BASE_FINISH_3, TB_BASE_WORK_3, TB_BASE_DUR_4,
                                                    TB_BASE_START_4, TB_BASE_FINISH_4, TB_BASE_WORK_4, TB_BASE_DUR_5, TB_BASE_START_5, TB_BASE_FINISH_5,
                                                    TB_BASE_WORK_5, TB_BASE_DUR_6, TB_BASE_START_6, TB_BASE_FINISH_6, TB_BASE_WORK_6, TB_BASE_DUR_7,
                                                    TB_BASE_START_7, TB_BASE_FINISH_7, TB_BASE_WORK_7, TB_BASE_DUR_8, TB_BASE_START_8, TB_BASE_FINISH_8,
                                                    TB_BASE_WORK_8, TB_BASE_DUR_9, TB_BASE_START_9, TB_BASE_FINISH_9, TB_BASE_WORK_9, TB_BASE_DUR_10,
                                                    TB_BASE_START_10, TB_BASE_FINISH_10, TB_BASE_WORK_10
						                                )
                                ) AS unpvt
                                SELECT DISTINCT TASK_UID FROM #TMPDB
                                DROP TABLE #TMPDB
                                ";
                                cmd.CommandType = CommandType.Text;
                                adapter.SelectCommand = cmd;
                                adapter.Fill(selqry);
                                if (selqry.Tables[0].Rows.Count > 0)
                                {
                                    foreach (DataRow row in selqry.Tables[0].Rows)
                                    {
                                        Guid tuid = new Guid(row["TASK_UID"].ToString());
                                        var taskexists = new DataSet();
                                        cmd.CommandText = @"SELECT COUNT(*) AS TCount FROM ITXBaseLineLogs.dbo.ITXBaseLineMaster
                                                    WHERE ProjectUID = '" + ProjectUID + "' AND TaskUID = '" + tuid + "'";
                                        cmd.CommandType = CommandType.Text;
                                        adapter.SelectCommand = cmd;
                                        adapter.Fill(taskexists);
                                        if (taskexists.Tables.Count != 0 && taskexists.Tables[0].Rows.Count > 0)
                                        {
                                            #region Insert New Baseline update task qry Perfromance Test

                                            string qry = string.Empty;
                                            qry = @"
                                            SELECT	t1.PROJ_UID, t1.TASK_UID,
		                                            CASE WHEN (t1.TB_BASE_DUR_0 = t2.TB_BASE_DUR_0) THEN NULL ELSE t1.TB_BASE_DUR_0 END AS TB_BASE_DUR_0,
		                                            CASE WHEN (t1.TB_BASE_START_0 = t2.TB_BASE_START_0) THEN NULL ELSE t1.TB_BASE_START_0 END AS TB_BASE_START_0,
		                                            CASE WHEN (t1.TB_BASE_FINISH_0 = t2.TB_BASE_FINISH_0) THEN NULL ELSE t1.TB_BASE_FINISH_0 END AS TB_BASE_FINISH_0,
		                                            CASE WHEN (t1.TB_BASE_WORK_0 = t2.TB_BASE_WORK_0) THEN NULL ELSE t1.TB_BASE_WORK_0 END AS TB_BASE_WORK_0,
		                                            CASE WHEN (t1.TB_BASE_DUR_1 = t2.TB_BASE_DUR_1) THEN NULL ELSE t1.TB_BASE_DUR_1 END AS TB_BASE_DUR_1,
		                                            CASE WHEN (t1.TB_BASE_START_1 = t2.TB_BASE_START_1) THEN NULL ELSE t1.TB_BASE_START_1 END AS TB_BASE_START_1,
		                                            CASE WHEN (t1.TB_BASE_FINISH_1 = t2.TB_BASE_FINISH_1) THEN NULL ELSE t1.TB_BASE_FINISH_1 END AS TB_BASE_FINISH_1,
		                                            CASE WHEN (t1.TB_BASE_WORK_1 = t2.TB_BASE_WORK_1) THEN NULL ELSE t1.TB_BASE_WORK_1 END AS TB_BASE_WORK_1,
		                                            CASE WHEN (t1.TB_BASE_DUR_2 = t2.TB_BASE_DUR_2) THEN NULL ELSE t1.TB_BASE_DUR_2 END AS TB_BASE_DUR_2,
		                                            CASE WHEN (t1.TB_BASE_START_2 = t2.TB_BASE_START_2) THEN NULL ELSE t1.TB_BASE_START_2 END AS TB_BASE_START_2,
		                                            CASE WHEN (t1.TB_BASE_FINISH_2 = t2.TB_BASE_FINISH_2) THEN NULL ELSE t1.TB_BASE_FINISH_2 END AS TB_BASE_FINISH_2,
		                                            CASE WHEN (t1.TB_BASE_WORK_2 = t2.TB_BASE_WORK_2) THEN NULL ELSE t1.TB_BASE_WORK_2 END AS TB_BASE_WORK_2,
		                                            CASE WHEN (t1.TB_BASE_DUR_3 = t2.TB_BASE_DUR_3) THEN NULL ELSE t1.TB_BASE_DUR_3 END AS TB_BASE_DUR_3,
		                                            CASE WHEN (t1.TB_BASE_START_3 = t2.TB_BASE_START_3) THEN NULL ELSE t1.TB_BASE_START_3 END AS TB_BASE_START_3,
		                                            CASE WHEN (t1.TB_BASE_FINISH_3 = t2.TB_BASE_FINISH_3) THEN NULL ELSE t1.TB_BASE_FINISH_3 END AS TB_BASE_FINISH_3,
		                                            CASE WHEN (t1.TB_BASE_WORK_3 = t2.TB_BASE_WORK_3) THEN NULL ELSE t1.TB_BASE_WORK_3 END AS TB_BASE_WORK_3,
		                                            CASE WHEN (t1.TB_BASE_DUR_4 = t2.TB_BASE_DUR_4) THEN NULL ELSE t1.TB_BASE_DUR_4 END AS TB_BASE_DUR_4,
		                                            CASE WHEN (t1.TB_BASE_START_4 = t2.TB_BASE_START_4) THEN NULL ELSE t1.TB_BASE_START_4 END AS TB_BASE_START_4,
		                                            CASE WHEN (t1.TB_BASE_FINISH_4 = t2.TB_BASE_FINISH_4) THEN NULL ELSE t1.TB_BASE_FINISH_4 END AS TB_BASE_FINISH_4,
		                                            CASE WHEN (t1.TB_BASE_WORK_4 = t2.TB_BASE_WORK_4) THEN NULL ELSE t1.TB_BASE_WORK_4 END AS TB_BASE_WORK_4,
		                                            CASE WHEN (t1.TB_BASE_DUR_5 = t2.TB_BASE_DUR_5) THEN NULL ELSE t1.TB_BASE_DUR_5 END AS TB_BASE_DUR_5,
		                                            CASE WHEN (t1.TB_BASE_START_5 = t2.TB_BASE_START_5) THEN NULL ELSE t1.TB_BASE_START_5 END AS TB_BASE_START_5,
		                                            CASE WHEN (t1.TB_BASE_FINISH_5 = t2.TB_BASE_FINISH_5) THEN NULL ELSE t1.TB_BASE_FINISH_5 END AS TB_BASE_FINISH_5,
		                                            CASE WHEN (t1.TB_BASE_WORK_5 = t2.TB_BASE_WORK_5) THEN NULL ELSE t1.TB_BASE_WORK_5 END AS TB_BASE_WORK_5,
		                                            CASE WHEN (t1.TB_BASE_DUR_6 = t2.TB_BASE_DUR_6) THEN NULL ELSE t1.TB_BASE_DUR_6 END AS TB_BASE_DUR_6,
		                                            CASE WHEN (t1.TB_BASE_START_6 = t2.TB_BASE_START_6) THEN NULL ELSE t1.TB_BASE_START_6 END AS TB_BASE_START_6,
		                                            CASE WHEN (t1.TB_BASE_FINISH_6 = t2.TB_BASE_FINISH_6) THEN NULL ELSE t1.TB_BASE_FINISH_6 END AS TB_BASE_FINISH_6,
		                                            CASE WHEN (t1.TB_BASE_WORK_6 = t2.TB_BASE_WORK_6) THEN NULL ELSE t1.TB_BASE_WORK_6 END AS TB_BASE_WORK_6,
		                                            CASE WHEN (t1.TB_BASE_DUR_7 = t2.TB_BASE_DUR_7) THEN NULL ELSE t1.TB_BASE_DUR_7 END AS TB_BASE_DUR_7,
		                                            CASE WHEN (t1.TB_BASE_START_7 = t2.TB_BASE_START_7) THEN NULL ELSE t1.TB_BASE_START_7 END AS TB_BASE_START_7,
		                                            CASE WHEN (t1.TB_BASE_FINISH_7 = t2.TB_BASE_FINISH_7) THEN NULL ELSE t1.TB_BASE_FINISH_7 END AS TB_BASE_FINISH_7,
		                                            CASE WHEN (t1.TB_BASE_WORK_7 = t2.TB_BASE_WORK_7) THEN NULL ELSE t1.TB_BASE_WORK_7 END AS TB_BASE_WORK_7,
		                                            CASE WHEN (t1.TB_BASE_DUR_8 = t2.TB_BASE_DUR_8) THEN NULL ELSE t1.TB_BASE_DUR_8 END AS TB_BASE_DUR_8,
		                                            CASE WHEN (t1.TB_BASE_START_8 = t2.TB_BASE_START_8) THEN NULL ELSE t1.TB_BASE_START_8 END AS TB_BASE_START_8,
		                                            CASE WHEN (t1.TB_BASE_FINISH_8 = t2.TB_BASE_FINISH_8) THEN NULL ELSE t1.TB_BASE_FINISH_8 END AS TB_BASE_FINISH_8,
		                                            CASE WHEN (t1.TB_BASE_WORK_8 = t2.TB_BASE_WORK_8) THEN NULL ELSE t1.TB_BASE_WORK_8 END AS TB_BASE_WORK_8,
		                                            CASE WHEN (t1.TB_BASE_DUR_9 = t2.TB_BASE_DUR_9) THEN NULL ELSE t1.TB_BASE_DUR_9 END AS TB_BASE_DUR_9,
		                                            CASE WHEN (t1.TB_BASE_START_9 = t2.TB_BASE_START_9) THEN NULL ELSE t1.TB_BASE_START_9 END AS TB_BASE_START_9,
		                                            CASE WHEN (t1.TB_BASE_FINISH_9 = t2.TB_BASE_FINISH_9) THEN NULL ELSE t1.TB_BASE_FINISH_9 END AS TB_BASE_FINISH_9,
		                                            CASE WHEN (t1.TB_BASE_WORK_9 = t2.TB_BASE_WORK_9) THEN NULL ELSE t1.TB_BASE_WORK_9 END AS TB_BASE_WORK_9,
		                                            CASE WHEN (t1.TB_BASE_DUR_10 = t2.TB_BASE_DUR_10) THEN NULL ELSE t1.TB_BASE_DUR_10 END AS TB_BASE_DUR_10,
		                                            CASE WHEN (t1.TB_BASE_START_10 = t2.TB_BASE_START_10) THEN NULL ELSE t1.TB_BASE_START_10 END AS TB_BASE_START_10,
		                                            CASE WHEN (t1.TB_BASE_FINISH_10 = t2.TB_BASE_FINISH_10) THEN NULL ELSE t1.TB_BASE_FINISH_10 END AS TB_BASE_FINISH_10,
		                                            CASE WHEN (t1.TB_BASE_WORK_10 = t2.TB_BASE_WORK_10) THEN NULL ELSE t1.TB_BASE_WORK_10 END AS TB_BASE_WORK_10
                                            FROM    Task AS t1 INNER JOIN
                                                    (SELECT     ProjectUID, TaskUID, TB_BASE_DUR_0, TB_BASE_START_0, TB_BASE_FINISH_0, TB_BASE_WORK_0, TB_BASE_DUR_1,
                                                                TB_BASE_START_1, TB_BASE_FINISH_1, TB_BASE_WORK_1, TB_BASE_DUR_2, TB_BASE_START_2, TB_BASE_FINISH_2,
                                                                TB_BASE_WORK_2, TB_BASE_DUR_3, TB_BASE_START_3, TB_BASE_FINISH_3, TB_BASE_WORK_3, TB_BASE_DUR_4,
                                                                TB_BASE_START_4, TB_BASE_FINISH_4, TB_BASE_WORK_4, TB_BASE_DUR_5, TB_BASE_START_5, TB_BASE_FINISH_5,
                                                                TB_BASE_WORK_5, TB_BASE_DUR_6, TB_BASE_START_6, TB_BASE_FINISH_6, TB_BASE_WORK_6, TB_BASE_DUR_7,
                                                                TB_BASE_START_7, TB_BASE_FINISH_7, TB_BASE_WORK_7, TB_BASE_DUR_8, TB_BASE_START_8, TB_BASE_FINISH_8,
                                                                TB_BASE_WORK_8, TB_BASE_DUR_9, TB_BASE_START_9, TB_BASE_FINISH_9, TB_BASE_WORK_9, TB_BASE_DUR_10,
                                                                TB_BASE_START_10, TB_BASE_FINISH_10, TB_BASE_WORK_10
                                                    FROM        ITXBaseLineLogs.dbo.SHADOW_ITXBaseLineMaster) AS t2 ON t2.ProjectUID = t1.PROJ_UID AND t1.TASK_UID = t2.TaskUID
                                            WHERE   (TASK_UID = '" + tuid + @"')
                                        ";

                                            #endregion Insert New Baseline update task qry Perfromance Test

                                            #region NULL Verification

                                            cmd.CommandText = qry;
                                            var @instest = new DataSet();
                                            adapter.SelectCommand = cmd;
                                            adapter.Fill(@instest);
                                            bool nochanges = true;
                                            foreach (DataRow resrow in @instest.Tables[0].Rows)
                                            {
                                                foreach (DataColumn rescol in resrow.Table.Columns)
                                                {
                                                    if (!string.IsNullOrEmpty(resrow[rescol.ToString()].ToString()))
                                                    {
                                                        string insvalue = @"
                                                            INSERT INTO	ITXBaseLineLogs.dbo.ITXBaseLineMaster ([ProjectUID],[TaskUID],[ModifiedBy],[ModifiedOn],
                                                            [TB_BASE_DUR_0],[TB_BASE_START_0],[TB_BASE_FINISH_0],[TB_BASE_WORK_0],[TB_BASE_DUR_1],
                                                            [TB_BASE_START_1],[TB_BASE_FINISH_1],[TB_BASE_WORK_1],[TB_BASE_DUR_2],[TB_BASE_START_2],
                                                            [TB_BASE_FINISH_2],[TB_BASE_WORK_2],[TB_BASE_DUR_3],[TB_BASE_START_3],[TB_BASE_FINISH_3],
                                                            [TB_BASE_WORK_3],[TB_BASE_DUR_4],[TB_BASE_START_4],[TB_BASE_FINISH_4],[TB_BASE_WORK_4],
                                                            [TB_BASE_DUR_5],[TB_BASE_START_5],[TB_BASE_FINISH_5],[TB_BASE_WORK_5],[TB_BASE_DUR_6],
                                                            [TB_BASE_START_6],[TB_BASE_FINISH_6],[TB_BASE_WORK_6],[TB_BASE_DUR_7],[TB_BASE_START_7],
                                                            [TB_BASE_FINISH_7],[TB_BASE_WORK_7],[TB_BASE_DUR_8],[TB_BASE_START_8],[TB_BASE_FINISH_8],
                                                            [TB_BASE_WORK_8],[TB_BASE_DUR_9],[TB_BASE_START_9],[TB_BASE_FINISH_9],[TB_BASE_WORK_9],
                                                            [TB_BASE_DUR_10],[TB_BASE_START_10],[TB_BASE_FINISH_10],[TB_BASE_WORK_10])
                                                            Values (
                                                            '" + ProjectUID + @"','" + tuid + @"','" + ModifiedBy + @"','" + DateTime.Now + @"'," +
                        GetFieldValue(resrow, "TB_BASE_DUR_0") + @"," + GetFieldValue(resrow, "TB_BASE_START_0") + @"," + GetFieldValue(resrow, "TB_BASE_FINISH_0") + @"," + GetFieldValue(resrow, "TB_BASE_WORK_0") + @"," +
                        GetFieldValue(resrow, "TB_BASE_DUR_1") + @"," + GetFieldValue(resrow, "TB_BASE_START_1") + @"," + GetFieldValue(resrow, "TB_BASE_FINISH_1") + @"," + GetFieldValue(resrow, "TB_BASE_WORK_1") + @"," +
                        GetFieldValue(resrow, "TB_BASE_DUR_2") + @"," + GetFieldValue(resrow, "TB_BASE_START_2") + @"," + GetFieldValue(resrow, "TB_BASE_FINISH_2") + @"," + GetFieldValue(resrow, "TB_BASE_WORK_2") + @"," +
                        GetFieldValue(resrow, "TB_BASE_DUR_3") + @"," + GetFieldValue(resrow, "TB_BASE_START_3") + @"," + GetFieldValue(resrow, "TB_BASE_FINISH_3") + @"," + GetFieldValue(resrow, "TB_BASE_WORK_3") + @"," +
                        GetFieldValue(resrow, "TB_BASE_DUR_4") + @"," + GetFieldValue(resrow, "TB_BASE_START_4") + @"," + GetFieldValue(resrow, "TB_BASE_FINISH_4") + @"," + GetFieldValue(resrow, "TB_BASE_WORK_4") + @"," +
                        GetFieldValue(resrow, "TB_BASE_DUR_5") + @"," + GetFieldValue(resrow, "TB_BASE_START_5") + @"," + GetFieldValue(resrow, "TB_BASE_FINISH_5") + @"," + GetFieldValue(resrow, "TB_BASE_WORK_5") + @"," +
                        GetFieldValue(resrow, "TB_BASE_DUR_6") + @"," + GetFieldValue(resrow, "TB_BASE_START_6") + @"," + GetFieldValue(resrow, "TB_BASE_FINISH_6") + @"," + GetFieldValue(resrow, "TB_BASE_WORK_6") + @"," +
                        GetFieldValue(resrow, "TB_BASE_DUR_7") + @"," + GetFieldValue(resrow, "TB_BASE_START_7") + @"," + GetFieldValue(resrow, "TB_BASE_FINISH_7") + @"," + GetFieldValue(resrow, "TB_BASE_WORK_7") + @"," +
                        GetFieldValue(resrow, "TB_BASE_DUR_8") + @"," + GetFieldValue(resrow, "TB_BASE_START_8") + @"," + GetFieldValue(resrow, "TB_BASE_FINISH_8") + @"," + GetFieldValue(resrow, "TB_BASE_WORK_8") + @"," +
                        GetFieldValue(resrow, "TB_BASE_DUR_9") + @"," + GetFieldValue(resrow, "TB_BASE_START_9") + @"," + GetFieldValue(resrow, "TB_BASE_FINISH_9") + @"," + GetFieldValue(resrow, "TB_BASE_WORK_9") + @"," +
                        GetFieldValue(resrow, "TB_BASE_DUR_10") + @"," + GetFieldValue(resrow, "TB_BASE_START_10") + @"," + GetFieldValue(resrow, "TB_BASE_FINISH_10") + @"," + GetFieldValue(resrow, "TB_BASE_WORK_10") + @")

                                                    UPDATE	ITXBaseLineLogs.dbo.SHADOW_ITXBaseLineMaster
                                                        SET
                                                            [TB_BASE_DUR_0]=t1.[TB_BASE_DUR_0],[TB_BASE_START_0]=t1.[TB_BASE_START_0],
                                                            [TB_BASE_FINISH_0]=t1.[TB_BASE_FINISH_0],[TB_BASE_WORK_0]=t1.[TB_BASE_WORK_0],
                                                            [TB_BASE_DUR_1]=t1.[TB_BASE_DUR_1],[TB_BASE_START_1]=t1.[TB_BASE_START_1],
                                                            [TB_BASE_FINISH_1]=t1.[TB_BASE_FINISH_1],[TB_BASE_WORK_1]=t1.[TB_BASE_WORK_1],
                                                            [TB_BASE_DUR_2]=t1.[TB_BASE_DUR_2],[TB_BASE_START_2]=t1.[TB_BASE_START_2],
                                                            [TB_BASE_FINISH_2]=t1.[TB_BASE_FINISH_2],[TB_BASE_WORK_2]=t1.[TB_BASE_WORK_2],
                                                            [TB_BASE_DUR_3]=t1.[TB_BASE_DUR_3],[TB_BASE_START_3]=t1.[TB_BASE_START_3],
                                                            [TB_BASE_FINISH_3]=t1.[TB_BASE_FINISH_3],[TB_BASE_WORK_3]=t1.[TB_BASE_WORK_3],
                                                            [TB_BASE_DUR_4]=t1.[TB_BASE_DUR_4],[TB_BASE_START_4]=t1.[TB_BASE_START_4],
                                                            [TB_BASE_FINISH_4]=t1.[TB_BASE_FINISH_4],[TB_BASE_WORK_4]=t1.[TB_BASE_WORK_4],
                                                            [TB_BASE_DUR_5]=t1.[TB_BASE_DUR_5],[TB_BASE_START_5]=t1.[TB_BASE_START_5],
                                                            [TB_BASE_FINISH_5]=t1.[TB_BASE_FINISH_5],[TB_BASE_WORK_5]=t1.[TB_BASE_WORK_5],
                                                            [TB_BASE_DUR_6]=t1.[TB_BASE_DUR_6],[TB_BASE_START_6]=t1.[TB_BASE_START_6],
                                                            [TB_BASE_FINISH_6]=t1.[TB_BASE_FINISH_6],[TB_BASE_WORK_6]=t1.[TB_BASE_WORK_6],
                                                            [TB_BASE_DUR_7]=t1.[TB_BASE_DUR_7],[TB_BASE_START_7]=t1.[TB_BASE_START_7],
                                                            [TB_BASE_FINISH_7]=t1.[TB_BASE_FINISH_7],[TB_BASE_WORK_7]=t1.[TB_BASE_WORK_7],
                                                            [TB_BASE_DUR_8]=t1.[TB_BASE_DUR_8],[TB_BASE_START_8]=t1.[TB_BASE_START_8],
                                                            [TB_BASE_FINISH_8]=t1.[TB_BASE_FINISH_8],[TB_BASE_WORK_8]=t1.[TB_BASE_WORK_8],
                                                            [TB_BASE_DUR_9]=t1.[TB_BASE_DUR_9],[TB_BASE_START_9]=t1.[TB_BASE_START_9],
                                                            [TB_BASE_FINISH_9]=t1.[TB_BASE_WORK_9],[TB_BASE_WORK_9]=t1.[TB_BASE_WORK_9],
                                                            [TB_BASE_DUR_10]=t1.[TB_BASE_DUR_10],[TB_BASE_START_10]=t1.[TB_BASE_START_10],
                                                            [TB_BASE_FINISH_10]=t1.[TB_BASE_FINISH_10],[TB_BASE_WORK_10]=t1.[TB_BASE_WORK_10]
                                                    FROM    ITXBaseLineLogs.dbo.SHADOW_ITXBaseLineMaster t
                                                    RIGHT JOIN
                                                            Task t1 ON t.ProjectUID= t1.PROJ_UID AND t.TaskUID = t1.TASK_UID
                                                    WHERE   t1.Task_UID = '" + tuid + @"' AND Proj_UID = '" + ProjectUID + @"'

                                                    ";
                                                        cmd.CommandText = insvalue;
                                                        adapter.SelectCommand = cmd;
                                                        cmd.ExecuteNonQuery();
                                                        nochanges = false;
                                                        break;
                                                    }
                                                }
                                                if (!nochanges)
                                                    break;
                                            }
                                            /*
                                            if (string.IsNullOrEmpty(instest.Tables[0].Rows[0]["TB_BASE_DUR_0"].ToString())  && instest.Tables[0].Rows[0]["TB_BASE_DUR_1"] != null ||
                                                instest.Tables[0].Rows[0]["TB_BASE_DUR_2"] != null || instest.Tables[0].Rows[0]["TB_BASE_DUR_3"] != null ||
                                                instest.Tables[0].Rows[0]["TB_BASE_DUR_4"] != null || instest.Tables[0].Rows[0]["TB_BASE_DUR_5"] != null ||
                                                instest.Tables[0].Rows[0]["TB_BASE_DUR_6"] != null || instest.Tables[0].Rows[0]["TB_BASE_DUR_7"] != null ||
                                                instest.Tables[0].Rows[0]["TB_BASE_DUR_8"] != null || instest.Tables[0].Rows[0]["TB_BASE_DUR_9"] != null ||
                                                instest.Tables[0].Rows[0]["TB_BASE_DUR_10"] != null ||
                                                instest.Tables[0].Rows[0]["TB_BASE_START_0"] != null || instest.Tables[0].Rows[0]["TB_BASE_START_1"] != null ||
                                                instest.Tables[0].Rows[0]["TB_BASE_START_2"] != null || instest.Tables[0].Rows[0]["TB_BASE_START_3"] != null ||
                                                instest.Tables[0].Rows[0]["TB_BASE_START_4"] != null || instest.Tables[0].Rows[0]["TB_BASE_START_5"] != null ||
                                                instest.Tables[0].Rows[0]["TB_BASE_START_6"] != null || instest.Tables[0].Rows[0]["TB_BASE_START_7"] != null ||
                                                instest.Tables[0].Rows[0]["TB_BASE_START_8"] != null || instest.Tables[0].Rows[0]["TB_BASE_START_9"] != null ||
                                                instest.Tables[0].Rows[0]["TB_BASE_START_10"] != null ||
                                                instest.Tables[0].Rows[0]["TB_BASE_FINISH_0"] != null || instest.Tables[0].Rows[0]["TB_BASE_FINISH_1"] != null ||
                                                instest.Tables[0].Rows[0]["TB_BASE_FINISH_2"] != null || instest.Tables[0].Rows[0]["TB_BASE_FINISH_3"] != null ||
                                                instest.Tables[0].Rows[0]["TB_BASE_FINISH_4"] != null || instest.Tables[0].Rows[0]["TB_BASE_FINISH_5"] != null ||
                                                instest.Tables[0].Rows[0]["TB_BASE_FINISH_6"] != null || instest.Tables[0].Rows[0]["TB_BASE_FINISH_7"] != null ||
                                                instest.Tables[0].Rows[0]["TB_BASE_FINISH_8"] != null || instest.Tables[0].Rows[0]["TB_BASE_FINISH_9"] != null ||
                                                instest.Tables[0].Rows[0]["TB_BASE_FINISH_10"] != null ||
                                                instest.Tables[0].Rows[0]["TB_BASE_WORK_0"] != null || instest.Tables[0].Rows[0]["TB_BASE_WORK_1"] != null ||
                                                instest.Tables[0].Rows[0]["TB_BASE_WORK_2"] != null || instest.Tables[0].Rows[0]["TB_BASE_WORK_3"] != null ||
                                                instest.Tables[0].Rows[0]["TB_BASE_WORK_4"] != null || instest.Tables[0].Rows[0]["TB_BASE_WORK_5"] != null ||
                                                instest.Tables[0].Rows[0]["TB_BASE_WORK_6"] != null || instest.Tables[0].Rows[0]["TB_BASE_WORK_7"] != null ||
                                                instest.Tables[0].Rows[0]["TB_BASE_WORK_8"] != null || instest.Tables[0].Rows[0]["TB_BASE_WORK_9"] != null ||
                                                instest.Tables[0].Rows[0]["TB_BASE_WORK_10"] != null)
                                            {
                                                string testSting = "This If Condition is True, So Skip";
                                            }
                                            else
                                            {
                                                string insvalue = @"
                                                INSERT INTO	ITXBaseLineLogs.dbo.ITXBaseLineMaster ([ProjectUID],[TaskUID],[ModifiedBy],[ModifiedOn],
                                                [TB_BASE_DUR_0],[TB_BASE_START_0],[TB_BASE_FINISH_0],[TB_BASE_WORK_0],[TB_BASE_DUR_1],
                                                [TB_BASE_START_1],[TB_BASE_FINISH_1],[TB_BASE_WORK_1],[TB_BASE_DUR_2],[TB_BASE_START_2],
                                                [TB_BASE_FINISH_2],[TB_BASE_WORK_2],[TB_BASE_DUR_3],[TB_BASE_START_3],[TB_BASE_FINISH_3],
                                                [TB_BASE_WORK_3],[TB_BASE_DUR_4],[TB_BASE_START_4],[TB_BASE_FINISH_4],[TB_BASE_WORK_4],
                                                [TB_BASE_DUR_5],[TB_BASE_START_5],[TB_BASE_FINISH_5],[TB_BASE_WORK_5],[TB_BASE_DUR_6],
                                                [TB_BASE_START_6],[TB_BASE_FINISH_6],[TB_BASE_WORK_6],[TB_BASE_DUR_7],[TB_BASE_START_7],
                                                [TB_BASE_FINISH_7],[TB_BASE_WORK_7],[TB_BASE_DUR_8],[TB_BASE_START_8],[TB_BASE_FINISH_8],
                                                [TB_BASE_WORK_8],[TB_BASE_DUR_9],[TB_BASE_START_9],[TB_BASE_FINISH_9],[TB_BASE_WORK_9],
                                                [TB_BASE_DUR_10],[TB_BASE_START_10],[TB_BASE_FINISH_10],[TB_BASE_WORK_10])
                                                Values (
                                                '" + ProjectUID + "','" + tuid + "','" + ModifiedBy + "','" + DateTime.Now + @"',
                                                [TB_BASE_DUR_0],[TB_BASE_START_0],[TB_BASE_FINISH_0],[TB_BASE_WORK_0],[TB_BASE_DUR_1],
                                                [TB_BASE_START_1],[TB_BASE_FINISH_1],[TB_BASE_WORK_1],[TB_BASE_DUR_2],[TB_BASE_START_2],
                                                [TB_BASE_FINISH_2],[TB_BASE_WORK_2],[TB_BASE_DUR_3],[TB_BASE_START_3],[TB_BASE_FINISH_3],
                                                [TB_BASE_WORK_3],[TB_BASE_DUR_4],[TB_BASE_START_4],[TB_BASE_FINISH_4],[TB_BASE_WORK_4],
                                                [TB_BASE_DUR_5],[TB_BASE_START_5],[TB_BASE_FINISH_5],[TB_BASE_WORK_5],[TB_BASE_DUR_6],
                                                [TB_BASE_START_6],[TB_BASE_FINISH_6],[TB_BASE_WORK_6],[TB_BASE_DUR_7],[TB_BASE_START_7],
                                                [TB_BASE_FINISH_7],[TB_BASE_WORK_7],[TB_BASE_DUR_8],[TB_BASE_START_8],[TB_BASE_FINISH_8],
                                                [TB_BASE_WORK_8],[TB_BASE_DUR_9],[TB_BASE_START_9],[TB_BASE_FINISH_9],[TB_BASE_WORK_9],
                                                [TB_BASE_DUR_10],[TB_BASE_START_10],[TB_BASE_FINISH_10],[TB_BASE_WORK_10])
                                                ";
                                                cmd.CommandText = insvalue;
                                                adapter.SelectCommand = cmd;
                                                cmd.ExecuteNonQuery();
                                            }
                                            **/

                                            #endregion NULL Verification

                                            #region Insert new baseline update task qry

                                            /*
                                        cmd.CommandText = @"
                                    INSERT	ITXBaseLineLogs.dbo.ITXBaseLineMaster ([ProjectUID],[TaskUID],[ModifiedBy],[ModifiedOn],
                                            [TB_BASE_DUR_0],[TB_BASE_START_0],[TB_BASE_FINISH_0],[TB_BASE_WORK_0],[TB_BASE_DUR_1],
                                            [TB_BASE_START_1],[TB_BASE_FINISH_1],[TB_BASE_WORK_1],[TB_BASE_DUR_2],[TB_BASE_START_2],
                                            [TB_BASE_FINISH_2],[TB_BASE_WORK_2],[TB_BASE_DUR_3],[TB_BASE_START_3],[TB_BASE_FINISH_3],
                                            [TB_BASE_WORK_3],[TB_BASE_DUR_4],[TB_BASE_START_4],[TB_BASE_FINISH_4],[TB_BASE_WORK_4],
                                            [TB_BASE_DUR_5],[TB_BASE_START_5],[TB_BASE_FINISH_5],[TB_BASE_WORK_5],[TB_BASE_DUR_6],
                                            [TB_BASE_START_6],[TB_BASE_FINISH_6],[TB_BASE_WORK_6],[TB_BASE_DUR_7],[TB_BASE_START_7],
                                            [TB_BASE_FINISH_7],[TB_BASE_WORK_7],[TB_BASE_DUR_8],[TB_BASE_START_8],[TB_BASE_FINISH_8],
                                            [TB_BASE_WORK_8],[TB_BASE_DUR_9],[TB_BASE_START_9],[TB_BASE_FINISH_9],[TB_BASE_WORK_9],
                                            [TB_BASE_DUR_10],[TB_BASE_START_10],[TB_BASE_FINISH_10],[TB_BASE_WORK_10])
                                    SELECT  PROJ_UID, TASK_UID, '" + ModifiedBy + "','" + DateTime.Now + @"' AS ModifiedOn,
                                            CASE WHEN ([TB_BASE_DUR_0] =
                                              (SELECT     TOP (1) TB_BASE_DUR_0 AS TB_BASE_DUR_0
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_DUR_0 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_DUR_0] END AS TB_BASE_DUR_0, CASE WHEN ([TB_BASE_START_0] =
                                              (SELECT     TOP (1) TB_BASE_START_0 AS TB_BASE_START_0
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_START_0 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_START_0] END AS TB_BASE_START_0, CASE WHEN ([TB_BASE_FINISH_0] =
                                              (SELECT     TOP (1) TB_BASE_FINISH_0 AS TB_BASE_FINISH_0
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_FINISH_0 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_FINISH_0] END AS TB_BASE_FINISH_0, CASE WHEN ([TB_BASE_WORK_0] =
                                              (SELECT     TOP (1) TB_BASE_WORK_0 AS TB_BASE_WORK_0
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_WORK_0 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_WORK_0] END AS TB_BASE_WORK_0, CASE WHEN ([TB_BASE_DUR_1] =
                                              (SELECT     TOP (1) TB_BASE_DUR_1 AS TB_BASE_DUR_1
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_DUR_1 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_DUR_1] END AS TB_BASE_DUR_1, CASE WHEN ([TB_BASE_START_1] =
                                              (SELECT     TOP (1) TB_BASE_START_1 AS TB_BASE_START_1
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_START_1 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_START_1] END AS TB_BASE_START_1, CASE WHEN ([TB_BASE_FINISH_1] =
                                              (SELECT     TOP (1) TB_BASE_FINISH_1 AS TB_BASE_FINISH_1
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_FINISH_1 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_FINISH_1] END AS TB_BASE_FINISH_1, CASE WHEN ([TB_BASE_WORK_1] =
                                              (SELECT     TOP (1) TB_BASE_WORK_1 AS TB_BASE_WORK_1
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_WORK_1 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_WORK_1] END AS TB_BASE_WORK_1, CASE WHEN ([TB_BASE_DUR_2] =
                                              (SELECT     TOP (1) TB_BASE_DUR_2 AS TB_BASE_DUR_2
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_DUR_2 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_DUR_2] END AS TB_BASE_DUR_2, CASE WHEN ([TB_BASE_START_2] =
                                              (SELECT     TOP (1) TB_BASE_START_2 AS TB_BASE_START_2
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_START_2 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_START_2] END AS TB_BASE_START_2, CASE WHEN ([TB_BASE_FINISH_2] =
                                              (SELECT     TOP (1) TB_BASE_FINISH_2 AS TB_BASE_FINISH_2
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_FINISH_2 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_FINISH_2] END AS TB_BASE_FINISH_2, CASE WHEN ([TB_BASE_WORK_2] =
                                              (SELECT     TOP (1) TB_BASE_WORK_2 AS TB_BASE_WORK_2
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_WORK_2 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_WORK_2] END AS TB_BASE_WORK_2, CASE WHEN ([TB_BASE_DUR_3] =
                                              (SELECT     TOP (1) TB_BASE_DUR_3 AS TB_BASE_DUR_3
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_DUR_3 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_DUR_3] END AS TB_BASE_DUR_3, CASE WHEN ([TB_BASE_START_3] =
                                              (SELECT     TOP (1) TB_BASE_START_3 AS TB_BASE_START_3
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_START_3 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_START_3] END AS TB_BASE_START_3, CASE WHEN ([TB_BASE_FINISH_3] =
                                              (SELECT     TOP (1) TB_BASE_FINISH_3 AS TB_BASE_FINISH_3
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_FINISH_3 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_FINISH_3] END AS TB_BASE_FINISH_3, CASE WHEN ([TB_BASE_WORK_3] =
                                              (SELECT     TOP (1) TB_BASE_WORK_3 AS TB_BASE_WORK_3
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_WORK_3 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_WORK_3] END AS TB_BASE_WORK_3, CASE WHEN ([TB_BASE_DUR_4] =
                                              (SELECT     TOP (1) TB_BASE_DUR_4 AS TB_BASE_DUR_4
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_DUR_4 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_DUR_4] END AS TB_BASE_DUR_4, CASE WHEN ([TB_BASE_START_4] =
                                              (SELECT     TOP (1) TB_BASE_START_4 AS TB_BASE_START_4
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_START_4 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_START_4] END AS TB_BASE_START_4, CASE WHEN ([TB_BASE_FINISH_4] =
                                              (SELECT     TOP (1) TB_BASE_FINISH_4 AS TB_BASE_FINISH_4
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_FINISH_4 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_FINISH_4] END AS TB_BASE_FINISH_4, CASE WHEN ([TB_BASE_WORK_4] =
                                              (SELECT     TOP (1) TB_BASE_WORK_4 AS TB_BASE_WORK_4
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_WORK_4 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_WORK_4] END AS TB_BASE_WORK_4, CASE WHEN ([TB_BASE_DUR_5] =
                                              (SELECT     TOP (1) TB_BASE_DUR_5 AS TB_BASE_DUR_5
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_DUR_5 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_DUR_5] END AS TB_BASE_DUR_5, CASE WHEN ([TB_BASE_START_5] =
                                              (SELECT     TOP (1) TB_BASE_START_5 AS TB_BASE_START_5
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_START_5 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_START_5] END AS TB_BASE_START_5, CASE WHEN ([TB_BASE_FINISH_5] =
                                              (SELECT     TOP (1) TB_BASE_FINISH_5 AS TB_BASE_FINISH_5
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_FINISH_5 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_FINISH_5] END AS TB_BASE_FINISH_5, CASE WHEN ([TB_BASE_WORK_5] =
                                              (SELECT     TOP (1) TB_BASE_WORK_5 AS TB_BASE_WORK_5
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_WORK_5 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_WORK_5] END AS TB_BASE_WORK_5, CASE WHEN ([TB_BASE_DUR_6] =
                                              (SELECT     TOP (1) TB_BASE_DUR_6 AS TB_BASE_DUR_6
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_DUR_6 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_DUR_6] END AS TB_BASE_DUR_6, CASE WHEN ([TB_BASE_START_6] =
                                              (SELECT     TOP (1) TB_BASE_START_6 AS TB_BASE_START_6
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_START_6 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_START_6] END AS TB_BASE_START_6, CASE WHEN ([TB_BASE_FINISH_6] =
                                              (SELECT     TOP (1) TB_BASE_FINISH_6 AS TB_BASE_FINISH_6
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_FINISH_6 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_FINISH_6] END AS TB_BASE_FINISH_6, CASE WHEN ([TB_BASE_WORK_6] =
                                              (SELECT     TOP (1) TB_BASE_WORK_6 AS TB_BASE_WORK_6
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_WORK_6 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_WORK_6] END AS TB_BASE_WORK_6, CASE WHEN ([TB_BASE_DUR_7] =
                                              (SELECT     TOP (1) TB_BASE_DUR_7 AS TB_BASE_DUR_7
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_DUR_7 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_DUR_7] END AS TB_BASE_DUR_7, CASE WHEN ([TB_BASE_START_7] =
                                              (SELECT     TOP (1) TB_BASE_START_7 AS TB_BASE_START_7
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_START_7 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_START_7] END AS TB_BASE_START_7, CASE WHEN ([TB_BASE_FINISH_7] =
                                              (SELECT     TOP (1) TB_BASE_FINISH_7 AS TB_BASE_FINISH_7
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_FINISH_7 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_FINISH_7] END AS TB_BASE_FINISH_7, CASE WHEN ([TB_BASE_WORK_7] =
                                              (SELECT     TOP (1) TB_BASE_WORK_7 AS TB_BASE_WORK_7
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_WORK_7 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_WORK_7] END AS TB_BASE_WORK_7, CASE WHEN ([TB_BASE_DUR_8] =
                                              (SELECT     TOP (1) TB_BASE_DUR_8 AS TB_BASE_DUR_8
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_DUR_8 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_DUR_8] END AS TB_BASE_DUR_8, CASE WHEN ([TB_BASE_START_8] =
                                              (SELECT     TOP (1) TB_BASE_START_8 AS TB_BASE_START_8
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_START_8 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_START_8] END AS TB_BASE_START_8, CASE WHEN ([TB_BASE_FINISH_8] =
                                              (SELECT     TOP (1) TB_BASE_FINISH_8 AS TB_BASE_FINISH_8
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_FINISH_8 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_FINISH_8] END AS TB_BASE_FINISH_8, CASE WHEN ([TB_BASE_WORK_8] =
                                              (SELECT     TOP (1) TB_BASE_WORK_8 AS TB_BASE_WORK_8
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_WORK_8 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_WORK_8] END AS TB_BASE_WORK_8, CASE WHEN ([TB_BASE_DUR_9] =
                                              (SELECT     TOP (1) TB_BASE_DUR_9 AS TB_BASE_DUR_9
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_DUR_9 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_DUR_9] END AS TB_BASE_DUR_9, CASE WHEN ([TB_BASE_START_9] =
                                              (SELECT     TOP (1) TB_BASE_START_9 AS TB_BASE_START_9
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_START_9 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_START_9] END AS TB_BASE_START_9, CASE WHEN ([TB_BASE_FINISH_9] =
                                              (SELECT     TOP (1) TB_BASE_FINISH_9 AS TB_BASE_FINISH_9
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_FINISH_9 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_FINISH_9] END AS TB_BASE_FINISH_9, CASE WHEN ([TB_BASE_WORK_9] =
                                              (SELECT     TOP (1) TB_BASE_WORK_9 AS TB_BASE_WORK_9
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_WORK_9 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_WORK_9] END AS TB_BASE_WORK_9, CASE WHEN ([TB_BASE_DUR_10] =
                                              (SELECT     TOP (1) TB_BASE_DUR_10 AS TB_BASE_DUR_10
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_DUR_10 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_DUR_10] END AS TB_BASE_DUR_10, CASE WHEN ([TB_BASE_START_10] =
                                              (SELECT     TOP (1) TB_BASE_START_10 AS TB_BASE_START_10
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_START_10 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_START_10] END AS TB_BASE_START_10, CASE WHEN ([TB_BASE_FINISH_10] =
                                              (SELECT     TOP (1) TB_BASE_FINISH_10 AS TB_BASE_FINISH_10
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_FINISH_10 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_FINISH_10] END AS TB_BASE_FINISH_10, CASE WHEN ([TB_BASE_WORK_10] =
                                              (SELECT     TOP (1) TB_BASE_WORK_10 AS TB_BASE_WORK_10
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_WORK_10 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_WORK_10] END AS TB_BASE_WORK_10
                                        FROM    Task
                                        WHERE	((CASE WHEN ([TB_BASE_DUR_0] =
                                                (SELECT     TOP (1) TB_BASE_DUR_0 AS TB_BASE_DUR_0
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_DUR_0 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_DUR_0] END IS NOT NULL) OR (CASE WHEN ([TB_BASE_START_0] =
                                                (SELECT     TOP (1) TB_BASE_START_0 AS TB_BASE_START_0
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_START_0 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_START_0] END IS NOT NULL) OR (CASE WHEN ([TB_BASE_FINISH_0] =
                                                (SELECT     TOP (1) TB_BASE_FINISH_0 AS TB_BASE_FINISH_0
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_FINISH_0 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_FINISH_0] END IS NOT NULL) OR (CASE WHEN ([TB_BASE_WORK_0] =
                                                (SELECT     TOP (1) TB_BASE_WORK_0 AS TB_BASE_WORK_0
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_WORK_0 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_WORK_0] END IS NOT NULL) OR (CASE WHEN ([TB_BASE_DUR_1] =
                                                (SELECT     TOP (1) TB_BASE_DUR_1 AS TB_BASE_DUR_1
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_DUR_1 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_DUR_1] END IS NOT NULL) OR (CASE WHEN ([TB_BASE_START_1] =
                                                (SELECT     TOP (1) TB_BASE_START_1 AS TB_BASE_START_1
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_START_1 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_START_1] END IS NOT NULL) OR (CASE WHEN ([TB_BASE_FINISH_1] =
                                                (SELECT     TOP (1) TB_BASE_FINISH_1 AS TB_BASE_FINISH_1
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_FINISH_1 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_FINISH_1] END IS NOT NULL) OR (CASE WHEN ([TB_BASE_WORK_1] =
                                                (SELECT     TOP (1) TB_BASE_WORK_1 AS TB_BASE_WORK_1
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_WORK_1 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_WORK_1] END IS NOT NULL) OR (CASE WHEN ([TB_BASE_DUR_2] =
                                                (SELECT     TOP (1) TB_BASE_DUR_2 AS TB_BASE_DUR_2
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_DUR_2 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_DUR_2] END IS NOT NULL) OR (CASE WHEN ([TB_BASE_START_2] =
                                                (SELECT     TOP (1) TB_BASE_START_2 AS TB_BASE_START_2
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_START_2 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_START_2] END IS NOT NULL) OR (CASE WHEN ([TB_BASE_FINISH_2] =
                                                (SELECT     TOP (1) TB_BASE_FINISH_2 AS TB_BASE_FINISH_2
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_FINISH_2 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_FINISH_2] END IS NOT NULL) OR (CASE WHEN ([TB_BASE_WORK_2] =
                                                (SELECT     TOP (1) TB_BASE_WORK_2 AS TB_BASE_WORK_2
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_WORK_2 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_WORK_2] END IS NOT NULL) OR (CASE WHEN ([TB_BASE_DUR_3] =
                                                (SELECT     TOP (1) TB_BASE_DUR_3 AS TB_BASE_DUR_3
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_DUR_3 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_DUR_3] END IS NOT NULL) OR (CASE WHEN ([TB_BASE_START_3] =
                                                (SELECT     TOP (1) TB_BASE_START_3 AS TB_BASE_START_3
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_START_3 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_START_3] END IS NOT NULL) OR (CASE WHEN ([TB_BASE_FINISH_3] =
                                                (SELECT     TOP (1) TB_BASE_FINISH_3 AS TB_BASE_FINISH_3
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_FINISH_3 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_FINISH_3] END IS NOT NULL) OR (CASE WHEN ([TB_BASE_WORK_3] =
                                                (SELECT     TOP (1) TB_BASE_WORK_3 AS TB_BASE_WORK_3
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_WORK_3 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_WORK_3] END IS NOT NULL) OR (CASE WHEN ([TB_BASE_DUR_4] =
                                                (SELECT     TOP (1) TB_BASE_DUR_4 AS TB_BASE_DUR_4
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_DUR_4 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_DUR_4] END IS NOT NULL) OR (CASE WHEN ([TB_BASE_START_4] =
                                                (SELECT     TOP (1) TB_BASE_START_4 AS TB_BASE_START_4
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_START_4 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_START_4] END IS NOT NULL) OR (CASE WHEN ([TB_BASE_FINISH_4] =
                                                (SELECT     TOP (1) TB_BASE_FINISH_4 AS TB_BASE_FINISH_4
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_FINISH_4 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_FINISH_4] END IS NOT NULL) OR (CASE WHEN ([TB_BASE_WORK_4] =
                                                (SELECT     TOP (1) TB_BASE_WORK_4 AS TB_BASE_WORK_4
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_WORK_4 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_WORK_4] END IS NOT NULL) OR (CASE WHEN ([TB_BASE_DUR_5] =
                                                (SELECT     TOP (1) TB_BASE_DUR_5 AS TB_BASE_DUR_5
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_DUR_5 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_DUR_5] END IS NOT NULL) OR (CASE WHEN ([TB_BASE_START_5] =
                                                (SELECT     TOP (1) TB_BASE_START_5 AS TB_BASE_START_5
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_START_5 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_START_5] END IS NOT NULL) OR (CASE WHEN ([TB_BASE_FINISH_5] =
                                                (SELECT     TOP (1) TB_BASE_FINISH_5 AS TB_BASE_FINISH_5
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_FINISH_5 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_FINISH_5] END IS NOT NULL) OR (CASE WHEN ([TB_BASE_WORK_5] =
                                                (SELECT     TOP (1) TB_BASE_WORK_5 AS TB_BASE_WORK_5
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_WORK_5 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_WORK_5] END IS NOT NULL) OR (CASE WHEN ([TB_BASE_DUR_6] =
                                                (SELECT     TOP (1) TB_BASE_DUR_6 AS TB_BASE_DUR_6
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_DUR_6 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_DUR_6] END IS NOT NULL) OR (CASE WHEN ([TB_BASE_START_6] =
                                                (SELECT     TOP (1) TB_BASE_START_6 AS TB_BASE_START_6
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_START_6 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_START_6] END IS NOT NULL) OR (CASE WHEN ([TB_BASE_FINISH_6] =
                                                (SELECT     TOP (1) TB_BASE_FINISH_6 AS TB_BASE_FINISH_6
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_FINISH_6 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_FINISH_6] END IS NOT NULL) OR (CASE WHEN ([TB_BASE_WORK_6] =
                                                (SELECT     TOP (1) TB_BASE_WORK_6 AS TB_BASE_WORK_6
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_WORK_6 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_WORK_6] END IS NOT NULL) OR (CASE WHEN ([TB_BASE_DUR_7] =
                                                (SELECT     TOP (1) TB_BASE_DUR_7 AS TB_BASE_DUR_7
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_DUR_7 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_DUR_7] END IS NOT NULL) OR (CASE WHEN ([TB_BASE_START_7] =
                                                (SELECT     TOP (1) TB_BASE_START_7 AS TB_BASE_START_7
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_START_7 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_START_7] END IS NOT NULL) OR (CASE WHEN ([TB_BASE_FINISH_7] =
                                                (SELECT     TOP (1) TB_BASE_FINISH_7 AS TB_BASE_FINISH_7
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_FINISH_7 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_FINISH_7] END IS NOT NULL) OR (CASE WHEN ([TB_BASE_WORK_7] =
                                                (SELECT     TOP (1) TB_BASE_WORK_7 AS TB_BASE_WORK_7
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_WORK_7 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_WORK_7] END IS NOT NULL) OR (CASE WHEN ([TB_BASE_DUR_8] =
                                                (SELECT     TOP (1) TB_BASE_DUR_8 AS TB_BASE_DUR_8
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_DUR_8 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_DUR_8] END IS NOT NULL) OR (CASE WHEN ([TB_BASE_START_8] =
                                                (SELECT     TOP (1) TB_BASE_START_8 AS TB_BASE_START_8
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_START_8 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_START_8] END IS NOT NULL) OR (CASE WHEN ([TB_BASE_FINISH_8] =
                                                (SELECT     TOP (1) TB_BASE_FINISH_8 AS TB_BASE_FINISH_8
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_FINISH_8 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_FINISH_8] END IS NOT NULL) OR (CASE WHEN ([TB_BASE_WORK_8] =
                                                (SELECT     TOP (1) TB_BASE_WORK_8 AS TB_BASE_WORK_8
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_WORK_8 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_WORK_8] END IS NOT NULL) OR (CASE WHEN ([TB_BASE_DUR_9] =
                                                (SELECT     TOP (1) TB_BASE_DUR_9 AS TB_BASE_DUR_9
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_DUR_9 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_DUR_9] END IS NOT NULL) OR (CASE WHEN ([TB_BASE_START_9] =
                                                (SELECT     TOP (1) TB_BASE_START_9 AS TB_BASE_START_9
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_START_9 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_START_9] END IS NOT NULL) OR (CASE WHEN ([TB_BASE_FINISH_9] =
                                                (SELECT     TOP (1) TB_BASE_FINISH_9 AS TB_BASE_FINISH_9
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_FINISH_9 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_FINISH_9] END IS NOT NULL) OR (CASE WHEN ([TB_BASE_WORK_9] =
                                                (SELECT     TOP (1) TB_BASE_WORK_9 AS TB_BASE_WORK_9
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_WORK_9 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_WORK_9] END IS NOT NULL) OR (CASE WHEN ([TB_BASE_DUR_10] =
                                                (SELECT     TOP (1) TB_BASE_DUR_10 AS TB_BASE_DUR_10
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_DUR_10 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_DUR_10] END IS NOT NULL) OR (CASE WHEN ([TB_BASE_START_10] =
                                                (SELECT     TOP (1) TB_BASE_START_10 AS TB_BASE_START_10
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_START_10 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_START_10] END IS NOT NULL) OR (CASE WHEN ([TB_BASE_FINISH_10] =
                                                (SELECT     TOP (1) TB_BASE_FINISH_10 AS TB_BASE_FINISH_10
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_FINISH_10 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_FINISH_10] END IS NOT NULL) OR (CASE WHEN ([TB_BASE_WORK_10] =
                                                (SELECT     TOP (1) TB_BASE_WORK_10 AS TB_BASE_WORK_10
                                                FROM          ITXBaseLineLogs.dbo.ITXBaseLineMaster AS t
                                                WHERE      (TB_BASE_WORK_10 IS NOT NULL) AND [TaskUID] = '" + tuid + @"'
                                                ORDER BY Idx DESC)) THEN NULL ELSE [TB_BASE_WORK_10] END IS NOT NULL))
							                        AND (TASK_UID = '" + tuid + @"')

                                    ";

                                        cmd.ExecuteNonQuery();

                                        **/

                                            #endregion Insert new baseline update task qry
                                        }
                                        else
                                        {
                                            #region Insert new task qry

                                            cmd.CommandText = @"
                                        INSERT ITXBaseLineLogs.dbo.ITXBaseLineMaster ([ProjectUID],[TaskUID],[ModifiedBy],[ModifiedOn],
                                                [TB_BASE_DUR_0],[TB_BASE_START_0],[TB_BASE_FINISH_0],[TB_BASE_WORK_0],[TB_BASE_DUR_1],[TB_BASE_START_1],
                                                [TB_BASE_FINISH_1],[TB_BASE_WORK_1],[TB_BASE_DUR_2],[TB_BASE_START_2],[TB_BASE_FINISH_2],[TB_BASE_WORK_2],
                                                [TB_BASE_DUR_3],[TB_BASE_START_3],[TB_BASE_FINISH_3],[TB_BASE_WORK_3],[TB_BASE_DUR_4],[TB_BASE_START_4],
                                                [TB_BASE_FINISH_4],[TB_BASE_WORK_4],[TB_BASE_DUR_5],[TB_BASE_START_5],[TB_BASE_FINISH_5],[TB_BASE_WORK_5],
                                                [TB_BASE_DUR_6],[TB_BASE_START_6],[TB_BASE_FINISH_6],[TB_BASE_WORK_6],[TB_BASE_DUR_7],[TB_BASE_START_7],
                                                [TB_BASE_FINISH_7],[TB_BASE_WORK_7],[TB_BASE_DUR_8],[TB_BASE_START_8],[TB_BASE_FINISH_8],[TB_BASE_WORK_8],
                                                [TB_BASE_DUR_9],[TB_BASE_START_9],[TB_BASE_FINISH_9],[TB_BASE_WORK_9],[TB_BASE_DUR_10],[TB_BASE_START_10],
                                                [TB_BASE_FINISH_10],[TB_BASE_WORK_10])
                                        SELECT [Proj_UID],[Task_UID],'" + ModifiedBy + "','" + DateTime.Now + @"',[TB_BASE_DUR_0],[TB_BASE_START_0],
                                                [TB_BASE_FINISH_0],[TB_BASE_WORK_0],[TB_BASE_DUR_1],[TB_BASE_START_1],[TB_BASE_FINISH_1],[TB_BASE_WORK_1],
                                                [TB_BASE_DUR_2],[TB_BASE_START_2],[TB_BASE_FINISH_2],[TB_BASE_WORK_2],[TB_BASE_DUR_3],[TB_BASE_START_3],
                                                [TB_BASE_FINISH_3],[TB_BASE_WORK_3],[TB_BASE_DUR_4],[TB_BASE_START_4],[TB_BASE_FINISH_4],[TB_BASE_WORK_4],
                                                [TB_BASE_DUR_5],[TB_BASE_START_5],[TB_BASE_FINISH_5],[TB_BASE_WORK_5],[TB_BASE_DUR_6],[TB_BASE_START_6],
                                                [TB_BASE_FINISH_6],[TB_BASE_WORK_6],[TB_BASE_DUR_7],[TB_BASE_START_7],[TB_BASE_FINISH_7],[TB_BASE_WORK_7],
                                                [TB_BASE_DUR_8],[TB_BASE_START_8],[TB_BASE_FINISH_8],[TB_BASE_WORK_8],[TB_BASE_DUR_9],[TB_BASE_START_9],
                                                [TB_BASE_FINISH_9],[TB_BASE_WORK_9],[TB_BASE_DUR_10],[TB_BASE_START_10],[TB_BASE_FINISH_10],[TB_BASE_WORK_10]
                                        FROM Task WHERE Task_UID = '" + tuid + @"' AND Proj_UID = '" + ProjectUID + @"'
                                        INSERT ITXBaseLineLogs.dbo.SHADOW_ITXBaseLineMaster ([ProjectUID],[TaskUID],
                                                [TB_BASE_DUR_0],[TB_BASE_START_0],[TB_BASE_FINISH_0],[TB_BASE_WORK_0],[TB_BASE_DUR_1],[TB_BASE_START_1],
                                                [TB_BASE_FINISH_1],[TB_BASE_WORK_1],[TB_BASE_DUR_2],[TB_BASE_START_2],[TB_BASE_FINISH_2],[TB_BASE_WORK_2],
                                                [TB_BASE_DUR_3],[TB_BASE_START_3],[TB_BASE_FINISH_3],[TB_BASE_WORK_3],[TB_BASE_DUR_4],[TB_BASE_START_4],
                                                [TB_BASE_FINISH_4],[TB_BASE_WORK_4],[TB_BASE_DUR_5],[TB_BASE_START_5],[TB_BASE_FINISH_5],[TB_BASE_WORK_5],
                                                [TB_BASE_DUR_6],[TB_BASE_START_6],[TB_BASE_FINISH_6],[TB_BASE_WORK_6],[TB_BASE_DUR_7],[TB_BASE_START_7],
                                                [TB_BASE_FINISH_7],[TB_BASE_WORK_7],[TB_BASE_DUR_8],[TB_BASE_START_8],[TB_BASE_FINISH_8],[TB_BASE_WORK_8],
                                                [TB_BASE_DUR_9],[TB_BASE_START_9],[TB_BASE_FINISH_9],[TB_BASE_WORK_9],[TB_BASE_DUR_10],[TB_BASE_START_10],
                                                [TB_BASE_FINISH_10],[TB_BASE_WORK_10])
                                        SELECT [Proj_UID],[Task_UID],[TB_BASE_DUR_0],[TB_BASE_START_0],
                                                [TB_BASE_FINISH_0],[TB_BASE_WORK_0],[TB_BASE_DUR_1],[TB_BASE_START_1],[TB_BASE_FINISH_1],[TB_BASE_WORK_1],
                                                [TB_BASE_DUR_2],[TB_BASE_START_2],[TB_BASE_FINISH_2],[TB_BASE_WORK_2],[TB_BASE_DUR_3],[TB_BASE_START_3],
                                                [TB_BASE_FINISH_3],[TB_BASE_WORK_3],[TB_BASE_DUR_4],[TB_BASE_START_4],[TB_BASE_FINISH_4],[TB_BASE_WORK_4],
                                                [TB_BASE_DUR_5],[TB_BASE_START_5],[TB_BASE_FINISH_5],[TB_BASE_WORK_5],[TB_BASE_DUR_6],[TB_BASE_START_6],
                                                [TB_BASE_FINISH_6],[TB_BASE_WORK_6],[TB_BASE_DUR_7],[TB_BASE_START_7],[TB_BASE_FINISH_7],[TB_BASE_WORK_7],
                                                [TB_BASE_DUR_8],[TB_BASE_START_8],[TB_BASE_FINISH_8],[TB_BASE_WORK_8],[TB_BASE_DUR_9],[TB_BASE_START_9],
                                                [TB_BASE_FINISH_9],[TB_BASE_WORK_9],[TB_BASE_DUR_10],[TB_BASE_START_10],[TB_BASE_FINISH_10],[TB_BASE_WORK_10]
                                        FROM Task WHERE Task_UID = '" + tuid + @"' AND Proj_UID = '" + ProjectUID + @"'

                                        ";

                                            #endregion Insert new task qry

                                            cmd.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                #region Insert New Project Baseline qry

                                cmd.CommandText = @"
                                        INSERT ITXBaseLineLogs.dbo.ITXBaseLineMaster ([ProjectUID],[TaskUID],[ModifiedBy],[ModifiedOn],
                                                [TB_BASE_DUR_0],[TB_BASE_START_0],[TB_BASE_FINISH_0],[TB_BASE_WORK_0],[TB_BASE_DUR_1],[TB_BASE_START_1],
                                                [TB_BASE_FINISH_1],[TB_BASE_WORK_1],[TB_BASE_DUR_2],[TB_BASE_START_2],[TB_BASE_FINISH_2],[TB_BASE_WORK_2],
                                                [TB_BASE_DUR_3],[TB_BASE_START_3],[TB_BASE_FINISH_3],[TB_BASE_WORK_3],[TB_BASE_DUR_4],[TB_BASE_START_4],
                                                [TB_BASE_FINISH_4],[TB_BASE_WORK_4],[TB_BASE_DUR_5],[TB_BASE_START_5],[TB_BASE_FINISH_5],[TB_BASE_WORK_5],
                                                [TB_BASE_DUR_6],[TB_BASE_START_6],[TB_BASE_FINISH_6],[TB_BASE_WORK_6],[TB_BASE_DUR_7],[TB_BASE_START_7],
                                                [TB_BASE_FINISH_7],[TB_BASE_WORK_7],[TB_BASE_DUR_8],[TB_BASE_START_8],[TB_BASE_FINISH_8],[TB_BASE_WORK_8],
                                                [TB_BASE_DUR_9],[TB_BASE_START_9],[TB_BASE_FINISH_9],[TB_BASE_WORK_9],[TB_BASE_DUR_10],[TB_BASE_START_10],
                                                [TB_BASE_FINISH_10],[TB_BASE_WORK_10])
                                        SELECT [Proj_UID],[Task_UID],'" + ModifiedBy + "','" + DateTime.Now + @"',[TB_BASE_DUR_0],[TB_BASE_START_0],
                                                [TB_BASE_FINISH_0],[TB_BASE_WORK_0],[TB_BASE_DUR_1],[TB_BASE_START_1],[TB_BASE_FINISH_1],[TB_BASE_WORK_1],
                                                [TB_BASE_DUR_2],[TB_BASE_START_2],[TB_BASE_FINISH_2],[TB_BASE_WORK_2],[TB_BASE_DUR_3],[TB_BASE_START_3],
                                                [TB_BASE_FINISH_3],[TB_BASE_WORK_3],[TB_BASE_DUR_4],[TB_BASE_START_4],[TB_BASE_FINISH_4],[TB_BASE_WORK_4],
                                                [TB_BASE_DUR_5],[TB_BASE_START_5],[TB_BASE_FINISH_5],[TB_BASE_WORK_5],[TB_BASE_DUR_6],[TB_BASE_START_6],
                                                [TB_BASE_FINISH_6],[TB_BASE_WORK_6],[TB_BASE_DUR_7],[TB_BASE_START_7],[TB_BASE_FINISH_7],[TB_BASE_WORK_7],
                                                [TB_BASE_DUR_8],[TB_BASE_START_8],[TB_BASE_FINISH_8],[TB_BASE_WORK_8],[TB_BASE_DUR_9],[TB_BASE_START_9],
                                                [TB_BASE_FINISH_9],[TB_BASE_WORK_9],[TB_BASE_DUR_10],[TB_BASE_START_10],[TB_BASE_FINISH_10],[TB_BASE_WORK_10]
                                        FROM Task WHERE Proj_UID = '" + ProjectUID + @"'

                                        INSERT ITXBaseLineLogs.dbo.SHADOW_ITXBaseLineMaster ([ProjectUID],[TaskUID],
                                                [TB_BASE_DUR_0],[TB_BASE_START_0],[TB_BASE_FINISH_0],[TB_BASE_WORK_0],[TB_BASE_DUR_1],[TB_BASE_START_1],
                                                [TB_BASE_FINISH_1],[TB_BASE_WORK_1],[TB_BASE_DUR_2],[TB_BASE_START_2],[TB_BASE_FINISH_2],[TB_BASE_WORK_2],
                                                [TB_BASE_DUR_3],[TB_BASE_START_3],[TB_BASE_FINISH_3],[TB_BASE_WORK_3],[TB_BASE_DUR_4],[TB_BASE_START_4],
                                                [TB_BASE_FINISH_4],[TB_BASE_WORK_4],[TB_BASE_DUR_5],[TB_BASE_START_5],[TB_BASE_FINISH_5],[TB_BASE_WORK_5],
                                                [TB_BASE_DUR_6],[TB_BASE_START_6],[TB_BASE_FINISH_6],[TB_BASE_WORK_6],[TB_BASE_DUR_7],[TB_BASE_START_7],
                                                [TB_BASE_FINISH_7],[TB_BASE_WORK_7],[TB_BASE_DUR_8],[TB_BASE_START_8],[TB_BASE_FINISH_8],[TB_BASE_WORK_8],
                                                [TB_BASE_DUR_9],[TB_BASE_START_9],[TB_BASE_FINISH_9],[TB_BASE_WORK_9],[TB_BASE_DUR_10],[TB_BASE_START_10],
                                                [TB_BASE_FINISH_10],[TB_BASE_WORK_10])
                                        SELECT [Proj_UID],[Task_UID],[TB_BASE_DUR_0],[TB_BASE_START_0],
                                                [TB_BASE_FINISH_0],[TB_BASE_WORK_0],[TB_BASE_DUR_1],[TB_BASE_START_1],[TB_BASE_FINISH_1],[TB_BASE_WORK_1],
                                                [TB_BASE_DUR_2],[TB_BASE_START_2],[TB_BASE_FINISH_2],[TB_BASE_WORK_2],[TB_BASE_DUR_3],[TB_BASE_START_3],
                                                [TB_BASE_FINISH_3],[TB_BASE_WORK_3],[TB_BASE_DUR_4],[TB_BASE_START_4],[TB_BASE_FINISH_4],[TB_BASE_WORK_4],
                                                [TB_BASE_DUR_5],[TB_BASE_START_5],[TB_BASE_FINISH_5],[TB_BASE_WORK_5],[TB_BASE_DUR_6],[TB_BASE_START_6],
                                                [TB_BASE_FINISH_6],[TB_BASE_WORK_6],[TB_BASE_DUR_7],[TB_BASE_START_7],[TB_BASE_FINISH_7],[TB_BASE_WORK_7],
                                                [TB_BASE_DUR_8],[TB_BASE_START_8],[TB_BASE_FINISH_8],[TB_BASE_WORK_8],[TB_BASE_DUR_9],[TB_BASE_START_9],
                                                [TB_BASE_FINISH_9],[TB_BASE_WORK_9],[TB_BASE_DUR_10],[TB_BASE_START_10],[TB_BASE_FINISH_10],[TB_BASE_WORK_10]
                                        FROM Task WHERE Proj_UID = '" + ProjectUID + @"'
                                        ";

                                #endregion Insert New Project Baseline qry

                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }

                #region Un used codes

                /*
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
                                if (VerifyDs.Tables[0].Rows[0][0].ToString().Trim() != row["TB_BASE_DUR_" + i].ToString().Trim())
                                {
                                    Qry += "'" + row["TB_BASE_DUR_" + i] + "',";
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
                                if (VerifyDs.Tables[0].Rows[0][0].ToString().Trim() != row["TB_BASE_START_" + i].ToString().Trim())
                                {
                                    Qry += "'" + row["TB_BASE_START_" + i].ToString() + "',";
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
                                if (VerifyDs.Tables[0].Rows[0][0].ToString().Trim() != row["TB_BASE_FINISH_" + i].ToString().Trim())
                                {
                                    Qry += "'" + row["TB_BASE_FINISH_" + i].ToString() + "',";
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
                                if (VerifyDs.Tables[0].Rows[0][0].ToString().Trim() != row["TB_BASE_WORK_" + i].ToString().Trim())
                                {
                                    Qry += "'" + row["TB_BASE_WORK_" + i].ToString() + "',";
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
                                Qry += "'" + row["TB_BASE_WORK_" + i] + "',";
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
 * */

                #endregion Un used codes
            }
        }
    }

    public static string GetFieldValue(DataRow row, string colname)
    {
        if (row[colname] != null && row[colname].ToString() != string.Empty)
            return @"'" + row[colname].ToString() + @"'";
        else
            return "NULL";
    }
};