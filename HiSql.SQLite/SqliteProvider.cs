using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// Sqlite数据库操作实现 
    /// AdoProvider 为公共数据库操作实现
    /// </summary>
    public class SqliteProvider : AdoProvider
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
                    base._DbConnection = new SQLiteConnection(base.Context.CurrentConnectionConfig.ConnectionString);
                }

                return base._DbConnection;
            
            }
            set {
                base._DbConnection = value;
            }
        }
        public override async Task<int> BulkCopy(DataTable sourceTable, TabInfo tabInfo, Dictionary<string, string> columnMapping = null)
        {
            throw new NotImplementedException();
        }

        public override Task<int> BulkCopy<T>(List<T> lstobj, TabInfo tabInfo, Dictionary<string, string> columnMapping = null)
        {
            throw new NotImplementedException();
            DataTable sourceTable = DataConvert.ToTable(lstobj, tabInfo, this.Context.CurrentConnectionConfig.User);
            return BulkCopy(sourceTable, tabInfo);
            
        }

        public override DbDataAdapter GetAdapter()
        {
            return new SQLiteDataAdapter();
        }

        public override DbCommand GetCommand(string sql, HiParameter[] param)
        {
            TryOpen(); 
            SQLiteCommand sqlCommand = new SQLiteCommand(sql, (SQLiteConnection)this.Connection);
            
            sqlCommand.CommandType = this.CmdTyp;
            sqlCommand.CommandTimeout = this.CommandTimeOut;
            if (this.Transaction != null)
            {
                sqlCommand.Transaction = (SQLiteTransaction)this.Transaction;
                
            }
            else
            {
                //参数化已经统一在AdoProvider处理
                //if (param.HasValue())
                //{
                //    SQLiteParameter[] ipars = GetSqlParameters(param);
                //    sqlCommand.Parameters.AddRange(ipars);
                //}
            }

            
            return sqlCommand;
        }

        public SQLiteParameter[] GetSqlParameters(params HiParameter[] parameters)
        {
            List<SQLiteParameter> sqlList = new List<SQLiteParameter>();

            foreach (var parameter in parameters)
            {
                SQLiteParameter _sqlparam = parameter.ConvertToSqlParameter();
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

            ((SQLiteDataAdapter)dataAdapter).SelectCommand = (SQLiteCommand)command;
        }

    }
}
