using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
namespace HiSql
{
    public class MySqlProvider : AdoProvider
    {
        /// <summary>
        /// 连接库连接
        /// </summary>
        public override IDbConnection Connection
        {
            get
            {
                if (base._DbConnection == null)
                {
                    var _connstring = base.Context.CurrentConnectionConfig.ConnectionString;
                    if (base.Context.CurrentConnectionConfig.IsEncrypt)
                    {
                        if (base.Context.CurrentConnectionConfig.AppEvents?.OnDbDecryptEvent != null)
                        {
                            //解密
                            _connstring = base.Context.CurrentConnectionConfig.AppEvents.OnDbDecryptEvent(_connstring);
                        }
                    }

                    base._DbConnection = new MySqlConnection(base.Context.CurrentConnectionConfig.ConnectionString);
                }

                return base._DbConnection;

            }
            set
            {
                base._DbConnection = value;
            }
        }
        private MySqlBulkLoader getBulkInstance(MySqlConnection conn)
        {

            MySqlBulkLoader bulkcopy = null;
            if (this.Context.DBO.Transaction != null)
            {
                //如果主连接有事务那么也同样开启事务

                bulkcopy = new MySqlBulkLoader((MySqlConnection)conn);
            }
            else
            {

                bulkcopy = new MySqlBulkLoader((MySqlConnection)conn);
            }

            bulkcopy.CharacterSet = "UTF8";
            bulkcopy.FieldTerminator = ",";
            bulkcopy.FieldQuotationCharacter = '"';
            bulkcopy.EscapeCharacter = '"';
            bulkcopy.LineTerminator = System.Environment.NewLine;
            bulkcopy.Local = true;

            if (conn.State == System.Data.ConnectionState.Closed)
            {
                //打开连接
                conn.Open();
            }
            ///设置超时时间 如果平台执行SQL的超时时间设置为一样
            //bulkcopy.BulkCopyTimeout = this.Context.CurrentConnectionConfig.SqlExecTimeOut;


            return bulkcopy;
        }
        public override async Task<int> BulkCopy(DataTable sourceTable, TabInfo tabInfo, Dictionary<string, string> columnMapping = null)
        {
            string _csv_content = DataConvert.ToCSV(sourceTable, Context.CurrentConnectionConfig.DbType, false);
            var conn = new MySqlConnection(base.Context.CurrentConnectionConfig.ConnectionString);
            int _rtn = 0;
            try
            {

                var dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "hitemp");
                DirectoryInfo directoryInfo = new DirectoryInfo(dllPath);
                if (!directoryInfo.Exists)
                    directoryInfo.Create();//如果不存在则创建

                string _filepath = $"{ Path.Combine(dllPath, Guid.NewGuid().ToString() + ".csv")}";




                File.WriteAllText(_filepath, _csv_content);



                MySqlBulkLoader bulk = getBulkInstance(conn);
                bulk.TableName = tabInfo.TabModel.TabName;
                bulk.NumberOfLinesToSkip = 0;//无列头
                bulk.FileName = _filepath;

                bulk.Columns.AddRange(columnMapping.Keys);

                _rtn= bulk.Load();
                if(File.Exists(_filepath))
                    File.Delete(_filepath);
            }
            catch (Exception E)
            {
                
                throw new Exception($"批量写入时错误:{E.Message}");
            }
            finally {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return _rtn;


        }

        public override async Task<int> BulkCopy<T>(List<T> lstobj, TabInfo tabInfo, Dictionary<string, string> columnMapping = null)
        {
            string _csv_content = DataConvert.ToCSV(lstobj, tabInfo, Context.CurrentConnectionConfig.DbType, false);
           
            var conn = new MySqlConnection(base.Context.CurrentConnectionConfig.ConnectionString);
            int _rtn = 0;
            string _filepath="";
            try
            {
                //this.Context.DBO.ExecCommand($"set global local_infile = 1;");
                var dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "hitemp");
                DirectoryInfo directoryInfo = new DirectoryInfo(dllPath);
                if (!directoryInfo.Exists)
                    directoryInfo.Create();//如果不存在则创建

                _filepath = $"{ Path.Combine(dllPath, Guid.NewGuid().ToString() + ".csv")}";




                File.WriteAllText(_filepath, _csv_content);



                MySqlBulkLoader bulk = getBulkInstance(conn);
                bulk.TableName = tabInfo.TabModel.TabName;
                bulk.NumberOfLinesToSkip = 0;//无列头
                bulk.FileName = _filepath;

                bulk.Columns.AddRange(columnMapping.Keys);

                _rtn = bulk.Load();
                if (File.Exists(_filepath))
                    File.Delete(_filepath);
            }
            catch (Exception E)
            {

                throw new Exception($"批量写入时错误:{E.Message}");
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();

                if (File.Exists(_filepath))
                    File.Delete(_filepath);
            }
            return 0;
        }

        public override DbDataAdapter GetAdapter()
        {
            return new MySqlDataAdapter();
        }

        public override DbCommand GetCommand(string sql, HiParameter[] param)
        {
            TryOpen();
            MySqlCommand sqlCommand = new MySqlCommand(sql, (MySqlConnection)this.Connection);
            
            sqlCommand.CommandType = this.CmdTyp;
            sqlCommand.CommandTimeout = this.CommandTimeOut;
            if (this.Transaction != null)
            {
                sqlCommand.Transaction = (MySqlTransaction)this.Transaction;

            }
            else
            {
                //参数化已经统一在AdoProvider处理
                //if (param.HasValue())
                //{
                //    SqlParameter[] ipars = GetSqlParameters(param);
                //    sqlCommand.Parameters.AddRange(ipars);
                //}
            }


            return sqlCommand;
        }

        public MySqlParameter[] GetSqlParameters(params HiParameter[] parameters)
        {
            List<MySqlParameter> sqlList = new List<MySqlParameter>();
            
            
            foreach (var parameter in parameters)
            {
                MySqlParameter _sqlparam = parameter.ConvertToSqlParameter();
                if (_sqlparam != null)
                {

                    if (_sqlparam.Direction.IsIn(ParameterDirection.Output, ParameterDirection.InputOutput, ParameterDirection.ReturnValue))
                    {
                        if (this.OutputParameters == null) this.OutputParameters = new List<IDataParameter>();
                        this.OutputParameters.RemoveAll(it => it.ParameterName == _sqlparam.ParameterName);
                        this.OutputParameters.Add(_sqlparam);
                    }

                    sqlList.Add(_sqlparam);
                }
            }
            return sqlList.ToArray();
        }

        public override void SetCommandToAdapter(IDataAdapter dataAdapter, DbCommand command)
        {
            
            ((MySqlDataAdapter)dataAdapter).SelectCommand = (MySqlCommand)command;
        }
    }
}
