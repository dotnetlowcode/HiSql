using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
//using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// SqlServer数据库操作实现 
    /// AdoProvider 为公共数据库操作实现
    /// </summary>
    public class SqlServerProvider : AdoProvider
    {
        /// <summary>
        /// 连接库连接
        /// </summary>
        public override IDbConnection Connection { 
            get {
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

                    
                    base._DbConnection = new SqlConnection(base.Context.CurrentConnectionConfig.ConnectionString);
                }

                return base._DbConnection;
            
            }
            set {
                base._DbConnection = value;
            }
        }
        public override async Task<int> BulkCopy(DataTable sourceTable, TabInfo tabInfo, Dictionary<string, string> columnMapping = null)
        {
            if (sourceTable != null && sourceTable.Rows.Count > 0)
            {
                SqlServerConfig sqlServerConfig = new SqlServerConfig();
                int _batchsize = sqlServerConfig.BulkUnitSize / sourceTable.Columns.Count;
                if (_batchsize < sourceTable.Columns.Count) _batchsize = 0;

                var conn = new  SqlConnection(base.Context.CurrentConnectionConfig.ConnectionString);
                 SqlBulkCopy sqlBulkCopy = getBulkInstance(conn);

                sqlBulkCopy.DestinationTableName = tabInfo.TabModel.TabName;
               
                try
                {
                    if (columnMapping != null)
                    {
                        foreach (string n in columnMapping.Keys)
                        {
                            sqlBulkCopy.ColumnMappings.Add(n, columnMapping[n]);
                        }
                    }
                    
                    sqlBulkCopy.BatchSize = _batchsize;
                    await sqlBulkCopy.WriteToServerAsync(sourceTable);
                   
                    
                    conn.Close();
                }
                catch (Exception E)
                {
                    conn.Close();
                    throw E;
                }
                return sourceTable.Rows.Count;
            }
            else return 0;
        }

        public override Task<int> BulkCopy<T>(List<T> lstobj, TabInfo tabInfo, Dictionary<string, string> columnMapping = null)
        {
            DataTable sourceTable = DataConvert.ToTable(lstobj, tabInfo, this.Context.CurrentConnectionConfig.User);
            return BulkCopy(sourceTable, tabInfo);
            
        }

        private  SqlBulkCopy getBulkInstance( SqlConnection conn)
        {

             SqlBulkCopy bulkcopy = null;
            if (this.Context.DBO.Transaction != null)
            {
                //如果主连接有事务那么也同样开启事务
             
                bulkcopy = new  SqlBulkCopy(( SqlConnection)conn, SqlBulkCopyOptions.CheckConstraints, (SqlTransaction)this.Context.DBO.Transaction);
            }
            else
            {
                
                bulkcopy= new  SqlBulkCopy(( SqlConnection)conn);
            }
            if (conn.State == System.Data.ConnectionState.Closed)
            {
                //打开连接
                conn.Open();
            }
            ///设置超时时间 如果平台执行SQL的超时时间设置为一样
            //bulkcopy.BulkCopyTimeout = this.Context.CurrentConnectionConfig.SqlExecTimeOut;


            return bulkcopy;
        }

        public override DbDataAdapter GetAdapter()
        {
            return new SqlDataAdapter();
        }

        public override DbCommand GetCommand(string sql, HiParameter[] param)
        {
            TryOpen();
            SqlCommand sqlCommand = new SqlCommand(sql, (SqlConnection)this.Connection);
            
            sqlCommand.CommandType = this.CmdTyp;
            sqlCommand.CommandTimeout = this.CommandTimeOut;
            if (this.Transaction != null)
            {
                sqlCommand.Transaction = (SqlTransaction)this.Transaction;
                
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

        public SqlParameter[] GetSqlParameters(params HiParameter[] parameters)
        {
            List<SqlParameter> sqlList = new List<SqlParameter>();

            foreach (var parameter in parameters)
            {
                SqlParameter _sqlparam = parameter.ConvertToSqlParameter();
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
            ((SqlDataAdapter)dataAdapter).SelectCommand = (SqlCommand)command;
        }

    }
}
