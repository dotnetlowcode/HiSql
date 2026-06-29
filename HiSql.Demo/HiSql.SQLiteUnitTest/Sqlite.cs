using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Configuration;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace HiSql

{
    public class Sqlite
    {
        /// <summary>
        /// 获得连接对象
        /// </summary>
        /// <returns></returns>
        public static SqliteConnection GetSqliteConnection()
        {
            string dbName = Path.Combine(Environment.CurrentDirectory, "SampleD3B4.db");
            string connStr = new SqliteConnectionStringBuilder()
            {
                DataSource = dbName
                ,
                //  Password = "admin"
            }.ToString();

            return new SqliteConnection(connStr);
        }
        private static SqliteTransaction transaction = null;

        private static void PrepareCommand(SqliteCommand cmd, SqliteConnection conn, string cmdText, params object[] p)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Parameters.Clear();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 30;
            if (p != null)
            {
                foreach (object parm in p)
                    cmd.Parameters.AddWithValue(string.Empty, parm);
            }
        }

        public static DataSet ExecuteDataset(string cmdText, params object[] p)
        {
            DataSet ds = new DataSet();
            SqliteCommand command = new SqliteCommand();
            using (SqliteConnection connection = GetSqliteConnection())
            {
                PrepareCommand(command, connection, cmdText, p);
                SqliteDataAdapter da = new SqliteDataAdapter(command);
                da.Fill(ds);
            }
            return ds;
        }
        public static DataRow ExecuteDataRow(string cmdText, params object[] p)
        {
            DataSet ds = ExecuteDataset(cmdText, p);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0];
            return null;
        }

        public static int ExecuteNonQuery(string cmdText, params object[] p)
        {
            SqliteCommand command = new SqliteCommand();
            using (SqliteConnection connection = GetSqliteConnection())
            {
                PrepareCommand(command, connection, cmdText, p);
                return command.ExecuteNonQuery();
            }
        }

        public static async Task<int> ExecuteNonQueryAsync(string cmdText, params object[] p)
        {
            SqliteCommand command = new SqliteCommand();
            using (SqliteConnection connection = GetSqliteConnection())
            {

                PrepareCommand(command, connection, cmdText, p);

                var task = command.ExecuteNonQueryAsync();
                task.Wait();
                var a = task.Result;
                return a;
            }
        }


        /// <summary>
        /// 返回SqlDataReader对象
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters">传入的参数</param>
        /// <returns></returns>
        public static SqliteDataReader ExecuteReader(string cmdText, params object[] p)
        {
            SqliteCommand command = new SqliteCommand();
            SqliteConnection connection = GetSqliteConnection();
            try
            {
                PrepareCommand(command, connection, cmdText, p);
                SqliteDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                return reader;
            }
            catch
            {
                connection.Close();
                throw;
            }
        }
        /// <summary>
        /// 返回结果集中的第一行第一列，忽略其他行或列
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters">传入的参数</param>
        /// <returns></returns>
        public static object ExecuteScalar(string cmdText, params object[] p)
        {
            SqliteCommand cmd = new SqliteCommand();
            using (SqliteConnection connection = GetSqliteConnection())
            {
                PrepareCommand(cmd, connection, cmdText, p);
                return cmd.ExecuteScalar();
            }
        }
        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="recordCount"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="cmdText"></param>
        /// <param name="countText"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static DataSet ExecutePager(ref int recordCount, int pageIndex, int pageSize, string cmdText, string countText, params object[] p)
        {
            if (recordCount < 0)
                recordCount = int.Parse(ExecuteScalar(countText, p).ToString());
            DataSet ds = new DataSet();
            SqliteCommand command = new SqliteCommand();
            using (SqliteConnection connection = GetSqliteConnection())
            {
                PrepareCommand(command, connection, cmdText, p);
                HiSql.SqliteDataAdapter da = new HiSql.SqliteDataAdapter(command);
                da.Fill(ds, (pageIndex - 1) * pageSize, pageSize, "result");
            }
            return ds;
        }
    }
}