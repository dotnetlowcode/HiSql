using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
namespace HiSql
{
    public class PostGreSqlProvider: AdoProvider
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

                    base._DbConnection = new NpgsqlConnection(base.Context.CurrentConnectionConfig.ConnectionString);
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
            var conn = new NpgsqlConnection(base.Context.CurrentConnectionConfig.ConnectionString);
            try
            {

                PostGreSqlConfig postGreSqlConfig = new PostGreSqlConfig();
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                List<string> lstcol = new List<string>();
                foreach (DataColumn dc in sourceTable.Columns)
                {
                    lstcol.Add($"{postGreSqlConfig.Field_Pre}{dc.ColumnName}{postGreSqlConfig.Field_After}");
                }

                if (tabInfo.Columns.Any(p => p.IsIdentity))
                {
                    throw new Exception($"PostGreSql 表[{tabInfo.TabModel.TabName}]包括自增长字段不允许进行批量写入");
                }

                using (var binary = conn.BeginBinaryImport($"COPY {postGreSqlConfig.Schema_Pre}{this.Context.CurrentConnectionConfig.Schema}{postGreSqlConfig.Schema_After}.{postGreSqlConfig.Table_Pre}{tabInfo.TabModel.TabName}{postGreSqlConfig.Table_After} ({string.Join(",", lstcol)})  FROM STDIN (FORMAT BINARY)"))
                {
                    foreach (DataRow drow in sourceTable.Rows)
                    {
                        binary.StartRow();
                        foreach (DataColumn dc in sourceTable.Columns)
                        {
                            var _value = drow[dc.ColumnName];
                            if (_value != null)
                            {
                                binary.Write(_value);
                            }
                            else
                                binary.Write(DBNull.Value);
                        }
                    }
                    binary.Complete();
                }
            }
            catch (Exception E)
            {
                throw new Exception($"批量写入时错误:{E.Message}");
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return sourceTable.Rows.Count;
        }

        public override Task<int> BulkCopy<T>(List<T> lstobj, TabInfo tabInfo, Dictionary<string, string> columnMapping = null)
        {
            DataTable sourceTable = DataConvert.ToTable(lstobj,tabInfo,this.Context.CurrentConnectionConfig.User);
            return BulkCopy(sourceTable, tabInfo);
            
        }

        


        public override IDataAdapter GetAdapter()
        {
            return new NpgsqlDataAdapter();
        }

        public override DbCommand GetCommand(string sql, HiParameter[] param)
        {
            TryOpen();
            NpgsqlCommand sqlCommand = new NpgsqlCommand(sql, (NpgsqlConnection)this.Connection);

            sqlCommand.CommandType = this.CmdTyp;
            sqlCommand.CommandTimeout = this.CommandTimeOut;
            if (this.Transaction != null)
            {
                sqlCommand.Transaction = (NpgsqlTransaction)this.Transaction;

            }
            else
            {
                if (param.HasValue())
                {
                    NpgsqlParameter[] ipars = GetSqlParameters(param);
                    sqlCommand.Parameters.AddRange(ipars);
                }
            }


            return sqlCommand;
        }

        public NpgsqlParameter[] GetSqlParameters(params HiParameter[] parameters)
        {
            List<NpgsqlParameter> sqlList = new List<NpgsqlParameter>();


            foreach (var parameter in parameters)
            {
                NpgsqlParameter _sqlparam = parameter.ConvertToSqlParameter();
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

            ((NpgsqlDataAdapter)dataAdapter).SelectCommand = (NpgsqlCommand)command;
        }
    }
}
