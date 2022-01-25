using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{

    /// <summary>
    /// 使用hana sdk连接
    /// </summary>
    public class HanaProvider : AdoProvider
    {
        // <summary>
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

                    base._DbConnection = new HanaConnection(base.Context.CurrentConnectionConfig.ConnectionString);
                }

                return base._DbConnection;

            }
            set
            {
                base._DbConnection = value;
            }
        }

        public override async Task<int> BulkCopy(DataTable sourceTable, TabInfo tabInfo, Dictionary<string, string> columnMapping = null)
        {
            if (sourceTable != null && sourceTable.Rows.Count > 0)
            {
                HanaConfig hanaConfig = new HanaConfig();
                int _batchsize = hanaConfig.BulkUnitSize / sourceTable.Columns.Count;
                if (_batchsize < sourceTable.Columns.Count) _batchsize = 0;

                var conn = new HanaConnection(base.Context.CurrentConnectionConfig.ConnectionString);
                HanaBulkCopy sqlBulkCopy = getBulkInstance(conn);

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
                    sqlBulkCopy.WriteToServer(sourceTable);


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

            throw new NotImplementedException();
        }

        public override Task<int> BulkCopy<T>(List<T> lstobj, TabInfo tabInfo, Dictionary<string, string> columnMapping = null)
        {
            DataTable sourceTable = DataConvert.ToTable(lstobj, tabInfo, this.Context.CurrentConnectionConfig.User);
            return BulkCopy(sourceTable, tabInfo);
        }
        private HanaBulkCopy getBulkInstance(HanaConnection conn)
        {


            HanaBulkCopy bulkcopy = null;
            if (this.Context.DBO.Transaction != null)
            {
                //如果主连接有事务那么也同样开启事务

                bulkcopy = new HanaBulkCopy((HanaConnection)conn, HanaBulkCopyOptions.Default, (HanaTransaction)this.Context.DBO.Transaction);
            }
            else
            {

                bulkcopy = new HanaBulkCopy((HanaConnection)conn);
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
        public override IDataAdapter GetAdapter()
        {
            return new HanaDataAdapter();
        }

        public override DbCommand GetCommand(string sql, HiParameter[] param)
        {
            TryOpen();
            HanaCommand sqlCommand = new HanaCommand(sql, (HanaConnection)this.Connection);

            
            sqlCommand.CommandType = this.CmdTyp;
            sqlCommand.CommandTimeout = this.CommandTimeOut;
            if (this.Transaction != null)
            {
                sqlCommand.Transaction = (HanaTransaction)this.Transaction;

            }
            else
            {
                if (param.HasValue())
                {
                    HanaParameter[] ipars = GetSqlParameters(param);
                    sqlCommand.Parameters.AddRange(ipars);
                }
            }


            return sqlCommand;
        }
        public HanaParameter[] GetSqlParameters(params HiParameter[] parameters)
        {
            List<HanaParameter> sqlList = new List<HanaParameter>();

            foreach (var parameter in parameters)
            {
                HanaParameter _sqlparam = parameter.ConvertToSqlParameter();
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
            ((HanaDataAdapter)dataAdapter).SelectCommand = (HanaCommand)command;
        }
    }
}
