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
