using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Text.RegularExpressions;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading;

namespace HiSql
{

    /// <summary>
    /// ADO 操作
    /// 
    /// modi by tgm date:2022.6.7 
    /// 1.事务提交或事务回滚 自动关闭数据库连接
    /// 2.语句执行有异常 自动关闭链接
    /// 3.当出现多个开启事务时将会把前一个事务Commit然后在开启新的事务
    /// </summary>
    public abstract partial class AdoProvider : AdoExtend, IDataBase
    {




        #region 委托事件
        public delegate Task<DataTable> deleGetTable(string sql, params HiParameter[] parameters);
        public delegate Task<DataSet> deleGetDataSet(string sql, params HiParameter[] parameters);
        public delegate Task<int> deleExecCommand(string sql, params HiParameter[] parameters);
        public delegate Task<object> deleExecScalar(string sql);
        //public delegate string deleGetTable(string sql, object parameters);

        #endregion



        #region 属性
        /// <summary>
        /// 连接库连接上下文
        /// </summary>
        public virtual HiSqlProvider Context { get; set; }

        private string _keyParameter = "@";

        private CommandType _cmdtype = CommandType.Text;
        private bool _isDisabledMasterSlave = false;//是否禁用主从库

        public virtual int CommandTimeOut { get; set; }
        protected List<IDataParameter> OutputParameters { get; set; }


        /// <summary>
        /// 主库连接
        /// </summary>
        public virtual IDbConnection MasterConnection { get; set; }

        /// <summary>
        /// 从库连接列表(一主多从)
        /// 后期需要实现动态感知从库 及从库的连接 分布
        /// </summary>
        public virtual List<IDbConnection> SlaveConnections
        {
            get; set;
        }
        /// <summary>
        /// 是否开启SQL 日志
        /// </summary>
        public virtual bool IsSqlLog { get; set; }
        /// <summary>
        /// 数据库连接
        /// </summary>
        public abstract IDbConnection Connection { get; set; }
        /// <summary>
        /// 事务
        /// </summary>
        public IDbTransaction Transaction { get; set; }

        /// <summary>
        /// 操作类型Text,StoredProcedure,TableDirect
        /// </summary>
        /// <summary>
        /// <summary>
        public CommandType CmdTyp { get => _cmdtype; set => _cmdtype = value; }

        /// <summary>
        /// 设置参数前辍符号 默认@
        /// </summary>
        public virtual string KeyParameter
        {
            get { return _keyParameter; }
            set { _keyParameter = value; }
        }
        /// <summary>
        /// 是否禁用了主从库
        /// </summary>
        public bool IsDisabledMasterSlave
        {
            get { return _isDisabledMasterSlave == false && this.Context.CurrentConnectionConfig.IsMasterSlave; }
            set { _isDisabledMasterSlave = value; }
        }

        #endregion

        /// <summary>
        /// 自定关闭连接
        /// </summary>
        /// <returns></returns>
        public bool IsAutoClose()
        {
            return this.Context.CurrentConnectionConfig.IsAutoClose && this.Transaction == null;
        }
        public AdoProvider()
        {


            this.CmdTyp = CommandType.Text;//默认
            this.CommandTimeOut = 300;//超时 秒
            this.IsLogSql = true;
            this.IsSqlLog = true;

        }


        #region 抽像必实现

        /// <summary>
        /// 执行SQL命令
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public abstract DbCommand GetCommand(string sql, HiParameter[] param);


        /// <summary>
        /// 向表批量复制插入
        /// </summary>
        /// <param name="sourceTable"></param>
        /// <param name="tabName"></param>
        public abstract Task<int> BulkCopy(DataTable sourceTable, TabInfo tabInfo,Dictionary<string,string> columnMapping=null);


        /// <summary>
        /// 批量对象插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lstobj"></param>
        /// <param name="tabInfo"></param>
        /// <param name="columnMapping"></param>
        /// <returns></returns>
        public abstract Task<int> BulkCopy<T>(List<T> lstobj, TabInfo tabInfo, Dictionary<string, string> columnMapping = null);

        public abstract DbDataAdapter GetAdapter();
        public abstract void SetCommandToAdapter(IDataAdapter adapter, DbCommand command);
        #endregion


        #region 事务及数据公共操作
        /// <summary>
        /// 启用事务
        /// </summary>
        public virtual void BeginTran()
        {
            TryOpen();
            if (this.Transaction == null)
            {
                this.Transaction = this.Connection.BeginTransaction();
            }
            else
            {
                CommitTran();//将之前的事务提前 再启开新的事务

                //有可能上一个事务已经关闭 需要打开
                TryOpen();
                this.Transaction = this.Connection.BeginTransaction();
            }



        }
        /// <summary>
        /// 开启事务指定隔离级别
        /// </summary>
        /// <param name="iso"></param>
        public virtual void BeginTran(IsolationLevel iso)
        {
            TryOpen();
            if (this.Transaction == null)
            {
                this.Transaction = this.Connection.BeginTransaction(iso);
            }
            else
            {
                CommitTran();//将之前的事务提前 再启开新的事务

                //有可能上一个事务已经关闭 需要打开
                TryOpen();
                this.Transaction = this.Connection.BeginTransaction(iso);
            }

        }

        public virtual int ExecBulkCopyCommand(DataTable sourceTable, TabInfo tabInfo, Dictionary<string,string> columnMapping=null)
        {
            return  this.BulkCopy(sourceTable, tabInfo, columnMapping).ConfigureAwait(false).GetAwaiter().GetResult();
        }
        public virtual int ExecBulkCopyCommand<T>(List<T> lstdata, TabInfo tabInfo, Dictionary<string, string> columnMapping = null)
        {
            return this.BulkCopy(lstdata, tabInfo, columnMapping).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual Task<int> ExecBulkCopyCommandAsync(DataTable sourceTable, TabInfo tabInfo, Dictionary<string, string> columnMapping = null)
        {
            return this.BulkCopy(sourceTable, tabInfo, columnMapping);
        }
        public virtual Task<int> ExecBulkCopyCommandAsync<T>(List<T> lstdata, TabInfo tabInfo, Dictionary<string, string> columnMapping = null)
        {
            return this.BulkCopy(lstdata, tabInfo, columnMapping);
        }

        public virtual void CommitTran()
        {
            if (this.Transaction != null)
            {
                this.Transaction.Commit();//提交事务
                this.Transaction = null;

                if (this.Context.CurrentConnectionConfig.IsAutoClose)
                {
                    this.Close();
                }
            }
        }
        public virtual void RollBackTran()
        {
            if (this.Transaction != null)
            {
                this.Transaction.Rollback();
                this.Transaction = null;
                if (this.Context.CurrentConnectionConfig.IsAutoClose)
                {
                    this.Close();
                }
            }
        }

        public virtual void Open()
        {
            TryOpen();
        }
        public virtual void Close()
        {
            if (this.Transaction != null)
            {
                this.Transaction = null;
            }

            if (this.Connection != null && this.Connection.State == ConnectionState.Open)
            {
                this.Connection.Close();
            }
            //如果有主从库还要关闭主从库

        }
        public virtual void Dispose()
        {
            this.CommitTran();

            this.Close();//关闭
        }
        #endregion


        #region Aop事件 

        //sql执行前事件
        public virtual Action<string, HiParameter[]> OnLogSqlExecuting => this.Context.CurrentConnectionConfig.AppEvents?.OnLogSqlExecuting;

        //sql执行后事件
        public virtual Action<string, HiParameter[]> OnLogSqlExecuted => this.Context.CurrentConnectionConfig.AppEvents?.OnLogSqlExecuted;

        /// <summary>
        /// sql执行错误后事件
        /// </summary>
        public virtual Action<HiSqlException> OnSqlError => this.Context.CurrentConnectionConfig.AppEvents?.OnSqlError;


        /// <summary>
        /// 执行超时事件
        /// </summary>
        public virtual Action<int> OnTimeOut => this.Context.CurrentConnectionConfig.AppEvents?.OnTimeOut;
        #endregion


        public void ExecAfter(string sql, HiParameter[] param)
        {
            throw new NotImplementedException();
        }

        public void ExecBefore(string sql, HiParameter[] param)
        {
            throw new NotImplementedException();
        }

        int IDataBase.ExecCommand(string sql, object parameters)
        {
            throw new NotImplementedException();
        }

        private int execCommand(string sql, params HiParameter[] parameters)
        {
            return this.ExecCommandAsync(sql, parameters).ConfigureAwait(false).GetAwaiter().GetResult();
        }
        private async Task<int> execCommandAsync(string sql, params HiParameter[] parameters)
        {
            int count = 0;
            bool hassError = false;
            Exception _e = null;
            try
            {

                #region 执行前操作
                ResolveParameter(ref sql, parameters);
                if (FormatSql != null)
                {
                    sql = FormatSql(sql);
                }

                if (IsSqlLog)
                {
                    //执行前日志

                    if (OnLogSqlExecuting != null)
                    {

                        Task.Run(() =>
                        {
                            OnLogSqlExecuting(sql, parameters);
                        });

                    }
                }
                #endregion
                DbCommand sqlCommand = GetCommand(sql, parameters);
                count = await sqlCommand.ExecuteNonQueryAsync();
                sqlCommand.Dispose();

                #region 执行后操作

                if (IsSqlLog)
                {
                    //执行后日志记录
                    if (OnLogSqlExecuted != null)
                    {
                        Task.Run(() =>
                        {
                            OnLogSqlExecuted(sql, parameters);
                        });

                    }
                }
                #endregion
                //return count;
            }
            catch (Exception E)
            {
                CmdTyp = CommandType.Text;
                hassError = true;
                ExecuteError(sql, parameters, E);
                this.Close();//有异常则关闭链接
                _e = E;
                //throw E;
            }
            finally
            {
                if (this.IsAutoClose())
                {
                    this.Close();
                }
                //ChooseConnectionEnd(sql);
            }
            if (hassError)
                throw _e;
            return count;
        }



        /// <summary>
        /// 执行sql语句 返回受影响的行
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public Task<int> ExecCommandAsync(string sql, params HiParameter[] parameters)
        {
            if (Context.CurrentConnectionConfig.UpperCase)
                sql = sql.ToUpper();
            //deleExecCommand _deleExecCommand = new deleExecCommand(execCommandAsync);
            //var workTask = Task.Run(() => _deleExecCommand.Invoke(sql, parameters));
            var workTask = execCommandAsync(sql, parameters);
            bool flag = workTask.Wait(this.Context.CurrentConnectionConfig.SqlExecTimeOut, new CancellationToken(false));
            if (flag)
            {
                //在指定的时间内完成
            }
            else
            {
                if (OnTimeOut != null)
                {
                    Task.Run(() =>
                    {
                        OnTimeOut(this.Context.CurrentConnectionConfig.SqlExecTimeOut);
                    });
                }
            }
            return workTask;
        }

        private async Task<object> execScalar(string sql)
        {
            object _effect = null;
            bool hassError = false;
            Exception _e = null;
            try
            {

                #region 执行前操作
                ResolveParameter(ref sql, null);
                if (FormatSql != null)
                {
                    sql = FormatSql(sql);
                }

                if (IsSqlLog)
                {
                    //执行前日志

                    if (OnLogSqlExecuting != null)
                    {

                        Task.Run(() =>
                        {
                            OnLogSqlExecuting(sql, null);
                        });

                    }
                }
                #endregion
                DbCommand sqlCommand = GetCommand(sql, null);
                _effect = await sqlCommand.ExecuteScalarAsync();
                if (_effect == null) _effect = 1;
                sqlCommand.Dispose();

                #region 执行后操作

                if (IsSqlLog)
                {
                    //执行后日志记录
                    if (OnLogSqlExecuted != null)
                    {
                        Task.Run(() =>
                        {
                            OnLogSqlExecuted(sql, null);
                        });

                    }
                }
                #endregion
                //return count;
            }
            catch (Exception E)
            {
                CmdTyp = CommandType.Text;
                hassError = true;
                _e = E;
                this.Close();
                ExecuteError(sql, null, E);
                //throw E;
            }
            finally
            {
                if (this.IsAutoClose())
                {
                    this.Close();
                }
                //ChooseConnectionEnd(sql);
            }
            if (hassError)
                throw _e;
            return _effect;
        }

        public virtual object ExecScalar(string sql)
        {
            return ExecScalarAsync(sql).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual Task<object> ExecScalarAsync(string sql)
        {
            if (Context.CurrentConnectionConfig.UpperCase)
                sql = sql.ToUpper();
            //deleExecScalar _deleExecScalar = new deleExecScalar(execScalar);
            var workTask = execScalar(sql); //Task.Run(() => _deleExecScalar.Invoke(sql));
            bool flag = workTask.Wait(this.Context.CurrentConnectionConfig.SqlExecTimeOut, new CancellationToken(false));
            if (flag)
            {
                //在指定的时间内完成
            }
            else
            {
                if (OnTimeOut != null)
                {
                    Task.Run(() => { OnTimeOut(this.Context.CurrentConnectionConfig.SqlExecTimeOut); });
                }
            }
            return workTask;
        }


        public IDataReader GetDataReader(string sql, object parameters)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetDataReader(string sql, params HiParameter[] parameters)
        {
            return GetDataReaderAsync(sql, parameters).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public async Task<IDataReader> GetDataReaderAsync(string sql, params HiParameter[] parameters)
        {
            try
            {
                if (Context.CurrentConnectionConfig.UpperCase)
                    sql = sql.ToUpper();
                #region  执行前操作
                ResolveParameter(ref sql, parameters);
                if (FormatSql != null)
                {
                    sql = FormatSql(sql);
                }

                if (this.IsSqlLog)
                {
                    //执行前日志

                    if (OnLogSqlExecuting != null)
                    {

                        Task.Run(() =>
                        {
                            OnLogSqlExecuting(sql, parameters);
                        });

                    }
                }
                #endregion

                DbCommand sqlCommand = GetCommand(sql, parameters);

                IDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync(this.IsAutoClose() ? CommandBehavior.CloseConnection : CommandBehavior.Default);

                #region 执行后操作
                if (this.IsSqlLog)
                {
                    //执行后日志记录
                    if (OnLogSqlExecuted != null)
                    {
                        Task.Run(() =>
                        {
                            OnLogSqlExecuted(sql, parameters);
                        });

                    }
                }

                //ChooseConnectionEnd(sql);


                #endregion
                return sqlDataReader;
            }
            catch (Exception E)
            {
                CmdTyp = CommandType.Text;
                ExecuteError(sql, parameters, E);
                this.Close();
                throw E;
                
            }
            finally
            {
                if (this.IsAutoClose())
                {
                    this.Close();
                }
                //ChooseConnectionEnd(sql);
            }




        }

        //public DataTable GetDataTable(string sql, object parameters)
        //{



        //    throw new NotImplementedException();
        //}


        private async Task<DataTable> getDataTable(string sql, params HiParameter[] parameters)
        {
            try
            {
                #region  执行前操作
                ResolveParameter(ref sql, parameters);
                if (FormatSql != null)
                {
                    sql = FormatSql(sql);
                }

                if (this.IsSqlLog)
                {
                    //执行前日志

                    if (OnLogSqlExecuting != null)
                    {

                        Task.Run(() =>
                        {
                            OnLogSqlExecuting(sql, parameters);
                        });

                    }
                }
                #endregion
                //IDbCommand sqlCommand = GetCommand(sql, parameters);
                //IDataReader sqlDataReader = sqlCommand.ExecuteReader(this.IsAutoClose() ? CommandBehavior.CloseConnection : CommandBehavior.Default);
                IDataAdapter dataAdapter = this.GetAdapter();
                DbCommand sqlCommand = GetCommand(sql, parameters);

                this.SetCommandToAdapter(dataAdapter, sqlCommand);
                DataSet ds = new DataSet();
                dataAdapter.Fill(ds);

                DataTable DT = null;

                if (ds.Tables.Count > 0)
                {
                    //return ds.Tables[0];
                    DT = ds.Tables[0].Copy();
                }







                #region 执行后操作
                if (this.IsSqlLog)
                {
                    //执行后日志记录
                    if (OnLogSqlExecuted != null)
                    {
                        Task.Run(() =>
                        {
                            OnLogSqlExecuted(sql, parameters);
                        });

                    }
                }

                //ChooseConnectionEnd(sql);


                #endregion
                return DT;
            }
            catch (Exception E)
            {
                CmdTyp = CommandType.Text;
                ExecuteError(sql, parameters, E);

                this.Close();//执行现异常关闭链接 add by tgm date;2022.6.7
                throw E;
            }
            finally
            {
                if (this.IsAutoClose())
                {
                    this.Close();
                }
            }
        }

        private async Task<DataSet> getDataSet(string sql, params HiParameter[] parameters)
        {
            try
            {
                #region  执行前操作
                ResolveParameter(ref sql, parameters);
                if (FormatSql != null)
                {
                    sql = FormatSql(sql);
                }

                if (this.IsSqlLog)
                {
                    //执行前日志
                    if (OnLogSqlExecuting != null)
                    {
                        Task.Run(() =>
                        {
                            OnLogSqlExecuting(sql, parameters);
                        });

                    }
                }
                #endregion
                DataSet ds = new DataSet();
                using (DbDataAdapter dataAdapter = this.GetAdapter())
                {
                    using (DbCommand sqlCommand = GetCommand(sql, parameters)) {
                        this.SetCommandToAdapter(dataAdapter, sqlCommand);
                        dataAdapter.Fill(ds);
                    }
                }
                
                #region 执行后操作
                if (this.IsSqlLog)
                {
                    //执行后日志记录
                    if (OnLogSqlExecuted != null)
                    {
                        Task.Run(() =>
                        {
                            OnLogSqlExecuted(sql, parameters);
                        });

                    }
                }
                #endregion
                return ds;
            }
            catch (Exception E)
            {
                CmdTyp = CommandType.Text;
                ExecuteError(sql, parameters, E);
                this.Close();
                throw E;
            }
            finally
            {
                if (this.IsAutoClose())
                {
                    this.Close();
                }
            }
        }
        public DataTable GetDataTable(string sql, params HiParameter[] parameters)
        {
            if (Context.CurrentConnectionConfig.UpperCase)
                sql = sql.ToUpper();
            lock (this.Context)
            {
                deleGetTable _deleGetTable = new deleGetTable(getDataTable);

             
                var workTask = Task.Run(() => _deleGetTable.Invoke(sql, parameters));
                bool flag = workTask.Wait(this.Context.CurrentConnectionConfig.SqlExecTimeOut, new CancellationToken(false));
                if (flag)
                {
                    //在指定的时间内完成
                }
                else
                {
                    if (OnTimeOut != null)
                    {
                        Task.Run(() => { OnTimeOut(this.Context.CurrentConnectionConfig.SqlExecTimeOut); });
                    }
                }
                return workTask.Result;
            }

            //return getDataTable(sql, parameters);
        }
        public DataSet GetDataSet(List<string> sqlList, List<HiParameter[]>  parametersList)
        {
            for (int i = 0; i < sqlList.Count; i++)
            {
                if (Context.CurrentConnectionConfig.UpperCase)
                    sqlList[i]= sqlList[i].ToUpper();
            }
            List<Task<DataTable>> taskList = new List<Task<DataTable>>();
            DataSet ds = new DataSet();

            //支持返回多个结果集
            if (this.Context.CurrentConnectionConfig.DbType == DBType.SqlServer 
                || this.Context.CurrentConnectionConfig.DbType == DBType.MySql
               || this.Context.CurrentConnectionConfig.DbType == DBType.PostGreSql

                )
            {
                lock (this.Context)
                {
                    var sql = new StringBuilder();
                    var parameters = new List<HiParameter>();
                    foreach (var item in sqlList)
                    {
                        sql.AppendLine(item);
                    }
                    if (parametersList != null)
                        foreach (var item in parametersList)
                        {
                            parameters.AddRange(item);
                        }
                    var parametersGp = from p in parameters
                                       group p by p.ParameterName into g
                                       select new { ParameterName = g.Key, Count = g.Count() };
                    if (parametersGp.Any(t => t.Count > 1))
                    {
                        throw new HiSqlException($"执行查询时HiParameter[]存在参数名称{parametersGp.FirstOrDefault(t => t.Count > 1).ParameterName}重复");
                    }

                    deleGetDataSet _deleGetSet = new deleGetDataSet(getDataSet);
                    var workTask = Task.Run(() => _deleGetSet.Invoke(sql.ToString(), parameters.ToArray()));
                    bool flag = workTask.Wait(this.Context.CurrentConnectionConfig.SqlExecTimeOut, new CancellationToken(false));
                    if (flag)
                    {
                        //在指定的时间内完成
                    }
                    else
                    {
                        if (OnTimeOut != null)
                        {
                            Task.Run(() => { OnTimeOut(this.Context.CurrentConnectionConfig.SqlExecTimeOut); });
                        }
                    }

                    ds = workTask.Result;
                }
            }
            else
            {
                lock (this.Context)
                {
                    for (int i = 0; i < sqlList.Count; i++)
                    {
                        deleGetTable _deleGetTable = new deleGetTable(getDataTable);
                        var _sql = sqlList[i];
                        var parameters = parametersList != null && parametersList.Count > 0 && parametersList.Count >= i - 1 ? parametersList[i] : null;
                        var workTask = Task.Run(() => _deleGetTable.Invoke(_sql, parameters));
                        taskList.Add(workTask);
                    }

                    bool flag = Task.WaitAll(taskList.ToArray(), this.Context.CurrentConnectionConfig.SqlExecTimeOut, new CancellationToken(false));
                    if (flag)
                    {
                        //在指定的时间内完成
                    }
                    else
                    {
                        if (OnTimeOut != null)
                        {
                            Task.Run(() => { OnTimeOut(this.Context.CurrentConnectionConfig.SqlExecTimeOut); });
                        }
                    }
                    foreach (var workTask in taskList)
                    {
                        var datatable = workTask.Result;
                        datatable.TableName = $"table{taskList.IndexOf(workTask)}";
                        ds.Tables.Add(datatable);
                    }
                }
            }


            return ds;

        }


        #region 委托事件
        public virtual Func<string, string> FormatSql { get; set; }
        public bool IsLogSql { get; set; }



        #endregion


        #region 扩展方法

        /// <summary>
        /// 检测数据库连接
        /// </summary>
        public virtual void TryOpen()
        {
            //判断当前连接是否是关闭状态
            if (this.Connection.State != ConnectionState.Open)
            {
                try
                {
                    this.Connection.Open();//尝试打开
                }
                catch (Exception E)
                {
                    throw new Exception(@$"数据打开失败...{E.Message}");
                }
            }
        }



        void ExecuteError(string sql, HiParameter[] paramters, Exception E)
        {
            if (OnSqlError != null)
            {
                OnSqlError(new HiSqlException(this.Context, E, sql));
            }
        }
        #endregion


        #region 解析参数
        void ResolveParameter(ref string sql, HiParameter[] parameters)
        {
            if (parameters!=null && parameters.HasValue())
            {
                foreach (HiParameter item in parameters)
                {

                    Type type = item.Values.GetType();
                    string value = "";
                    if (Tool.RegexMatch($@"{this._keyParameter}\w+", item.ParameterName))
                    {
                        if (Tool.RegexMatch($@"{item.ParameterName}\b", sql))
                        {
                            if ( type.FullName.IsList())
                            {
                                var newValues = new List<string>();
                                foreach (var inData in item.Values as IEnumerable)
                                {
                                    newValues.Add(inData.ObjToString());
                                }

                                //newValues = item.Value.ObjToString().Split(',').ToList<string>();

                                if (newValues.IsNullOrEmpty())
                                {
                                    //newValues.Add("-1");
                                    throw new Exception($"参数[{item.ParameterName}]的值为空");
                                }

                                //参数前辍仅支持@
                                //其实也可以用正则表达式匹配
                                if (item.ParameterName.Substring(0, 1) == this._keyParameter && !type.FullName.IsList())
                                {
                                    sql = sql.Replace(this._keyParameter + item.ParameterName.Substring(1), string.Join("", newValues.ToArray()).ToSqlValue());
                                    ;
                                }
                                else if (item.ParameterName.Substring(0, 1) == this._keyParameter && type.FullName.IsList())
                                {
                                    sql = sql.Replace(this._keyParameter + item.ParameterName.Substring(1), newValues.ToArray().ToSqlIn());
                                }
                                else
                                {
                                    sql = sql.Replace(item.ParameterName, newValues.ToArray().ToSqlIn());
                                }
                                item.Values = DBNull.Value;
                            }else if (type.IsIn(HiSql.Constants.IntType, HiSql.Constants.DecType, HiSql.Constants.ShortType,
                                HiSql.Constants.LongType, HiSql.Constants.FloatType))
                            {
                                value = item.Values.ToString();

                            }
                            else if (type.IsIn(HiSql.Constants.DateTimeOffsetType, HiSql.Constants.DateType))
                            {
                                value = $"'{Convert.ToDateTime(item.Values.ToString()).ToString("yyyy-MM-dd HH:mm:ss.fff")}'";

                            }
                            else if (type.IsIn(HiSql.Constants.BoolType))
                            {
                                if ((bool)item.Values)
                                {
                                    value = "1";
                                }
                                else
                                    value = "0";
                            }
                            else
                            {
                                value = $"'{item.Values.ToString().ToSqlInject()}'";
                            }

                            Regex regex = new Regex($@"{item.ParameterName}\b",RegexOptions.IgnoreCase);
                            sql = regex.Replace(sql, value);
                        }
                        else
                            throw new Exception($"语句[{sql}]未匹配参数[{item.ParameterName}]");

                    }
                    else
                        throw new Exception($"参数化名称[{item.ParameterName}]错误!格式为【@+名称】如@TabName");
                }
                if (Context.CurrentConnectionConfig.UpperCase)
                    sql = sql.ToUpper();
            }
        }

        /// <summary>
        /// 获取操作sql的动作 通过这个可以判断是否有主从连接
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        DBAction GetAction(string sql)
        {
            string _select_action = "[\\s]*select[\\s]+";
            string _insert_action = "[\\s]*insert[\\s]+";
            string _update_action = "[\\s]*update[\\s]+";


            if (Regex.IsMatch(sql, _insert_action, RegexOptions.IgnoreCase)
               && !Regex.IsMatch(sql, $"{_select_action}|{_update_action}", RegexOptions.IgnoreCase))
            {
                // 纯数据插入
                return DBAction.Modifiy;
            }
            else if (Regex.IsMatch(sql, _select_action, RegexOptions.IgnoreCase)
                && !Regex.IsMatch(sql, $"{_insert_action}|{_update_action}", RegexOptions.IgnoreCase))
            {
                //纯查询业务
                return DBAction.Select;
            }
            else if (Regex.IsMatch(sql, _select_action, RegexOptions.IgnoreCase)
                && Regex.IsMatch(sql, _insert_action, RegexOptions.IgnoreCase)
                && Regex.IsMatch(sql, _update_action, RegexOptions.IgnoreCase))
            {
                // 有查询，插入，更新
                return DBAction.Insert;
            }
            else if (Regex.IsMatch(sql, _update_action, RegexOptions.IgnoreCase)
             && !Regex.IsMatch(sql, $"{_select_action}|{_insert_action}", RegexOptions.IgnoreCase)
             )
            {
                //仅更新
                return DBAction.UPdate;
            }
            else
            {
                return DBAction.ExecSql;
            }
        }

        #endregion


        #region  选择连接
        void ChooseConnectionStart(string sql)
        {
            DBAction _dbaction = GetAction(sql);
            if (this.Transaction == null && this.IsDisabledMasterSlave == false && _dbaction == DBAction.Select)
            {
                if (this.MasterConnection == null)
                {
                    this.MasterConnection = this.Connection;
                }
                var slaves = this.Context.CurrentConnectionConfig.SlaveConnectionConfigs.Where(it => it.Weight > 0).ToList();

                var currentIndex = UtilRandom.GetRandomIndex(slaves.ToDictionary(it => slaves.ToList().IndexOf(it), it => it.Weight));

                var currentSaveConnection = slaves[currentIndex];
                this.Connection = null;
                this.Context.CurrentConnectionConfig.ConnectionString = currentSaveConnection.ConnectionString;
                this.Connection = this.Connection;

                if (this.SlaveConnections.IsNullOrEmpty() || !this.SlaveConnections.Any(it => EqualsConnectionString(it.ConnectionString, this.Connection.ConnectionString)))
                {
                    if (this.SlaveConnections == null) this.SlaveConnections = new List<IDbConnection>();
                    this.SlaveConnections.Add(this.Connection);
                }
            }
        }

        void ChooseConnectionEnd(string sql)
        {
            DBAction _dbaction = GetAction(sql);
            if (this.Transaction == null && this.IsDisabledMasterSlave == false && _dbaction == DBAction.Select)
            {
                this.Connection = this.MasterConnection;
                this.Context.CurrentConnectionConfig.ConnectionString = this.MasterConnection.ConnectionString;
            }
        }

        bool EqualsConnectionString(string connectionString1, string connectionString2)
        {
            var connectionString1Array = connectionString1.Split(';');
            var connectionString2Array = connectionString2.Split(';');
            var result = connectionString1Array.Except(connectionString2Array);
            return result.Count() == 0;
        }



        public Task<int> ExecCommandAsync(string sql, object parameters)
        {
            throw new NotImplementedException();
        }

        public int ExecCommand(string sql, params HiParameter[] parameters)
        {
            return this.execCommand(sql, parameters);
        }


        #endregion

    }
}
