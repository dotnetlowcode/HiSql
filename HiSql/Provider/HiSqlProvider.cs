using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace HiSql
{
    /// <summary>
    /// HiSql连接提供
    /// </summary>
    public partial class HiSqlProvider : IHiSqlClient, IMCache
    {
        //当前数据库连接
        private ConnectionConfig _currentConnectionConfig;


        //从库连接 当有多个连接时根据提供的权重配置随机产生一个从库连接（从库一定是与主库对应）
        private ConnectionConfig _slaveConnectionConfig;



        private string _connectedid = string.Empty;

        private IDM _idm ;

        /// <summary>
        /// 数据连接ID
        /// 通过这个可以控制连接跨线程使用，当连接跨线程使用时就会出现很多不可控的错误
        /// </summary>
        public string ConnectedId { get => _connectedid; }


        private Expression _expression;

        public Expression Expression
        {
            get { return _expression; }
            set { _expression = value; }
        }
        public ConnectionConfig CurrentConnectionConfig
        {
            get { return _currentConnectionConfig; }
        }


        /// <summary>
        /// 从库连接
        /// </summary>
        public ConnectionConfig SlaveConnectionConfig
        {
            get {
                if (_slaveConnectionConfig == null && CurrentConnectionConfig.SlaveConnectionConfigs!=null)
                {
                    _slaveConnectionConfig = _currentConnectionConfig.CloneProperoty();

                    //根据权重承随机选择一个从库连接
                    SlaveConnectionConfig sconnectionConfig = ConnManager.ChooseSlave(CurrentConnectionConfig.SlaveConnectionConfigs);
                    if (sconnectionConfig != null)
                    {
                        //通过继承类的方式将 连接配置 生成 ConnectionConfig 类
                        _slaveConnectionConfig = sconnectionConfig.MoveCross<SlaveConnectionConfig, ConnectionConfig>(_slaveConnectionConfig);
                        _slaveConnectionConfig.SlaveConnectionConfigs = null;
                        _slaveConnectionConfig.AppEvents = _currentConnectionConfig.AppEvents;// 事件同步
                        _slaveConnectionConfig.IsCurrSlave = true;//给当前连接标识为从库
                    }
                    else
                        _slaveConnectionConfig = null;
                }
                return _slaveConnectionConfig;
                //return _slaveConnectionConfig;

            }
        }


        public IDMInitalize DMInitalize
        {
            get {
                if (_idm == null)
                {
                    _idm = Instance.CreateInstance<IDM>($"{Constants.NameSpace}.{Context.CurrentConnectionConfig.DbType.ToString()}{DbInterFace.DM.ToString()}");
                    _idm.Context = Context;
                }
                IDMInitalize _idfmi = (IDMInitalize)_idm;
                _idfmi.Context = Context;
                return _idfmi;
            }
        }

        public IDMTab DMTab
        {
            get
            {
                if (_idm == null)
                {
                    _idm = Instance.CreateInstance<IDM>($"{Constants.NameSpace}.{Context.CurrentConnectionConfig.DbType.ToString()}{DbInterFace.DM.ToString()}");
                    _idm.Context = Context;
                }
                IDMTab dMTab = (IDMTab)_idm;
                dMTab.Context = Context;
                return dMTab;
            }

        }

        ICache _mcache = new MCache(null);
        public HiSqlClient Root;
        public HiSqlProvider(ConnectionConfig config)
        {

            _currentConnectionConfig = config;
            _connectedid = Guid.NewGuid().ToString();

        }
        public ICache MCache
        {
            get { return _mcache; }
            set { _mcache = value; }
        }


        IDataBase _dbo;
        HiSqlProvider _Context;

        /// <summary>
        /// 数据库操作
        /// </summary>
        protected IDataBase ContextDBO
        {
            get { return this._dbo; }
            set { this._dbo = value; }
        }

        public virtual IDataBase DBO
        {
            get {
                if (this.ContextDBO == null)
                {
                    //实现数据库连接
                    var result = Instance.GetDBO(this.Context.CurrentConnectionConfig);
                    result.IsLogSql = _currentConnectionConfig.IsLog;
                    //this.ContextDBO = result;
                    _dbo = result;

                    result.Context = this;
                    return this.ContextDBO;
                }
                else
                {
                    this.ContextDBO.IsLogSql = _currentConnectionConfig.IsLog;
                    return this.ContextDBO;
                }
            }
        }

        public HiSqlProvider Context
        {
            get {
                _Context = this;
                return _Context;
            }
            set {
                _Context = value;
            }
        }

        public ICodeFirst CodeFirst => throw new NotImplementedException();

        public IDbFirst DbFirst => throw new NotImplementedException();


        /// <summary>
        /// 根据当前配置 克隆一个新的连接
        /// 1.0.2.6 以上版本才支持
        /// </summary>
        /// <returns></returns>
        public HiSqlClient CloneClient()
        {
            return new HiSqlClient(_currentConnectionConfig);
        }

        /// <summary>
        /// 创建工作单元
        /// 默认开始事务,业务处理完成需要进行Commit 
        /// </summary>
        /// <returns></returns>
        public HiSqlClient CreateUnitOfWork()
        {
            var client = CloneClient();
            //连接不能自动关闭 因为自动关闭时事务会自动提交
            client.CurrentConnectionConfig.IsAutoClose = false;
            client.BeginTran();
            return client;
        }




        #region 查询
        public IQuery Query(params IQuery[] query)
        {
            IQuery result = Instance.GetQuery(this.Context.CurrentConnectionConfig);
            result.Context = this.Context;
            foreach (IQuery _q in query)
            {
                _q.Context = this.Context;
            }
            result.Query(query);
            return result;

        }
        /// <summary>
        /// 表查询
        /// </summary>
        /// <param name="tabname">表名</param>
        /// <param name="rename">表别名</param>
        /// <param name="dbMasterSlave">主从策略</param>
        /// <returns></returns>
        public IQuery Query(string tabname, string rename,DbMasterSlave dbMasterSlave= DbMasterSlave.Default)
        {
            IQuery result = null;
            //默认主从规则
            bool _isslave = ConnManager.ChooseSlaveForTable(this.Context.SlaveConnectionConfig, tabname, dbMasterSlave);
            if (_isslave)
                result = Instance.GetQuery(this.Context.SlaveConnectionConfig);
            else
                result = Instance.GetQuery(this.Context.CurrentConnectionConfig);

            result.Context = this.Context;
            result.Query(tabname, rename);
            return result;
        }
        /// <summary>
        /// 表查询
        /// </summary>
        /// <param name="tabname">表名</param>
        /// <param name="dbMasterSlave">主从策略</param>
        /// <returns></returns>
        public IQuery Query(string tabname, DbMasterSlave dbMasterSlave = DbMasterSlave.Default)
        {
            IQuery result = null;
            //默认主从规则

            bool _isslave = ConnManager.ChooseSlaveForTable(this.Context.SlaveConnectionConfig, tabname, dbMasterSlave);
            if(_isslave)
                result = Instance.GetQuery(this.Context.SlaveConnectionConfig);
            else
                result = Instance.GetQuery(this.Context.CurrentConnectionConfig);


            
            result.Context = this.Context;
            result.Query(tabname);
            return result;
        }



        /// <summary>
        /// 执行Hisql语句
        /// 详细请参照Hisql语法
        /// 注意hisql并不是原生数据sql Hisql是一个单独的语法可以编译成不同数据库的原生sql
        /// </summary>
        /// <param name="hisql"></param>
        /// <param name="dbMasterSlave"></param>
        /// <returns></returns>
        public IQuery HiSql(string hisql, DbMasterSlave dbMasterSlave = DbMasterSlave.Default)
        {
            IQuery result = null;
            //默认主从规则

            //bool _isslave = ConnManager.ChooseSlaveForTable(this.Context.SlaveConnectionConfig, tabname, dbMasterSlave);
            //if (dbMasterSlave)
            //    result = Instance.GetQuery(this.Context.SlaveConnectionConfig);
            //else
            result = Instance.GetQuery(this.Context.CurrentConnectionConfig);



            result.Context = this.Context;
            result.HiSql(hisql,result);
            return result;
        }

        /// <summary>
        /// hisql 语句防注入参数化
        /// </summary>
        /// <param name="hisql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        //public IQuery HiSql(string hisql, Dictionary<string, object> dicparma, DbMasterSlave dbMasterSlave = DbMasterSlave.Default)
        //{
        //    //[$name$]=tgm
        //    //[$age$]=21
        //    //select * from useradmin where username='[$name$]' and age>[$age$]

        //    string _sql = hisql;

        //    if (!Tool.RegexMatch("[\'\\\"]+", _sql))
        //    {
        //        if (dicparma.Count > 0)
        //        {
        //            foreach (string n in dicparma.Keys)
        //            {
        //                if (Tool.RegexMatch(Constants.REG_HISQL_PARAM, n))
        //                {
        //                    if (_sql.IndexOf(n) >= 0)
        //                    {
        //                        Type type = dicparma[n].GetType();
        //                        if (type.IsIn<Type>(Constants.ShortType, Constants.LongType, Constants.DecType, Constants.IntType, Constants.FloatType, Constants.DobType))
        //                        {
        //                            _sql = _sql.Replace(n, dicparma[n].ToString());
        //                        }
        //                        else if (type == Constants.BoolType)
        //                        {
        //                            if (dicparma[n].ToString().ToLower().Trim() == "true")
        //                                _sql = _sql.Replace(n, "1");
        //                            else
        //                                _sql = _sql.Replace(n, "0");
        //                        }
        //                        else if (type.IsIn<Type>(Constants.DateType, Constants.DateTimeOffsetType))
        //                        {
        //                            DateTime dtime = Convert.ToDateTime(dicparma[n]);
        //                            _sql = _sql.Replace(n, $"'{dtime.ToString("yyyy-MM-dd HH:mm:ss.fff")}'");
        //                        }
        //                        else if (type.IsIn<Type>(Constants.StringType, Constants.GuidType))
        //                        {
        //                            _sql = _sql.Replace(n, $"'{dicparma[n].ToString().ToSqlInject().ToSqlEnChar()}'");
        //                        }
        //                        else if (type.FullName.IndexOf("List") > 0)
        //                        {

        //                            var _dic_p1 = Tool.RegexGrps(Constants.REG_HISQL_PARAM2, _sql);
        //                            var _dic_p2 = Tool.RegexGrps(Constants.REG_HISQL_IN_PARAM, _sql);
        //                            string _insql = "";
        //                            if (_dic_p1.Count > 0 && _dic_p1.Count == _dic_p2.Count)
        //                            {
        //                                //foreach(var _o in )

        //                                var _typ_string = typeof(List<string>);
        //                                var _typ_int = typeof(List<int>);
        //                                var _typ_decimal = typeof(List<decimal>);

        //                                if (type == _typ_string)
        //                                {
        //                                    var list = dicparma[n] as List<string>;
        //                                    _insql=AdoExtensions.ToSqlIn<string>(list.ToArray(), true);
        //                                }
        //                                else if (type == _typ_int)
        //                                {
        //                                    var list = dicparma[n] as List<int>;
        //                                    _insql = AdoExtensions.ToSqlIn<int>(list.ToArray(), false);
        //                                }
        //                                else if (type == _typ_decimal)
        //                                {
        //                                    var list = dicparma[n] as List<decimal>;
        //                                    _insql = AdoExtensions.ToSqlIn<decimal>(list.ToArray(), false);
        //                                }
        //                                else
        //                                {
        //                                    throw new Exception($"类型[{type.FullName}]不在允许的in集合内,仅允许List<string>,List<int>,List<decimal> 三种类型");
        //                                }


        //                                _sql = _sql.Replace(n, $"{_insql}");
        //                            }
        //                            else
        //                            {
        //                                throw new Exception($"参数 {n} 是集合 中能放在in ({n})中");
        //                            }



        //                        }
        //                        else if (type.IsIn<Type>(Constants.ObjType))
        //                        {
        //                            _sql = _sql.Replace(n, $"'{dicparma[n].ToString().ToSqlInject().ToSqlEnChar()}'");
        //                        }
        //                        else
        //                        {
        //                            _sql = _sql.Replace(n, $"'{dicparma[n].ToString().ToSqlInject().ToSqlEnChar()}'");
        //                        }
        //                    }
        //                    else
        //                        throw new Exception($"参数 {n} 设置多余在参数化hisql中未使用");

        //                }
        //                else
        //                {
        //                    throw new Exception($"参数 {n} 不符合参数规则 规则为[$参数名$]");
        //                }
        //            }
        //        }
        //        else
        //        {
        //            throw new Exception($"模版参数为不能为空");
        //        }
        //    }
        //    else
        //    {
        //        throw new Exception($"参数化SQL语句中不能出现[\'\"]单引号和又引号这种特殊字段");
        //    }
        //    IQuery result = null;
        //    result = Instance.GetQuery(this.Context.CurrentConnectionConfig);



        //    result.Context = this.Context;
        //    result.HiSql(_sql, result);
        //    return result;

        //}


        /// <summary>
        /// hisql 参数化,防注入
        /// </summary>
        /// <param name="sql">hisql语句</param>
        /// <param name="objparm">参数化对象如new {}</param>
        /// <param name="dbMasterSlave"></param>
        /// <returns></returns>
        public IQuery HiSql(string sql, object objparm, DbMasterSlave dbMasterSlave = DbMasterSlave.Default)
        {

            Type type = objparm.GetType();
            if (!Tool.RegexMatch("[\'\\\"]+", sql))
            {
               
                Dictionary<string, object> dicparam = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

                #region 参数解析
                if (type.IsAnonClass())
                {
                    List<PropertyInfo> attrs = type.GetProperties().Where(p => p.MemberType == MemberTypes.Property && p.CanRead == true).ToList();
                    if (attrs.Count() > 0)
                    {
                        foreach (PropertyInfo p in attrs)
                        {
                            if (dicparam.ContainsKey(p.Name))
                                dicparam[p.Name] = p.GetValue(objparm);
                            else
                                dicparam.Add(p.Name, p.GetValue(objparm));

                        }
                    }
                }
                else if (type.IsDicStringClass())
                {
                    //字典对象
                    Dictionary<string, string> _dic = objparm as Dictionary<string, string>;
                    foreach (string key in _dic.Keys)
                    {
                        if (dicparam.ContainsKey(key))
                            dicparam[key] = _dic[key];
                        else
                            dicparam.Add(key, _dic[key]);
                    }


                }
                else if (type.IsDicObjectClass())
                {
                    Dictionary<string, object> _dic = objparm as Dictionary<string, object>;
                    foreach (string key in _dic.Keys)
                    {
                        if (dicparam.ContainsKey(key))
                            dicparam[key] = _dic[key];
                        else
                            dicparam.Add(key, _dic[key]);
                    }
                }
                else if (type == typeof(List<HiParameter>))
                {
                    //参数化对象

                    List<HiParameter> lstparam = objparm as List<HiParameter>;

                    foreach (HiParameter p in lstparam)
                    {
                        if (dicparam.ContainsKey(p.ParameterName))
                            dicparam[p.ParameterName] = p.Values;
                        else
                            dicparam.Add(p.ParameterName, p.Values);
                    }

                }
                else
                {
                    //实体类对象
                    List<PropertyInfo> attrs = type.GetProperties().Where(p => p.MemberType == MemberTypes.Property && p.CanRead == true).ToList();
                    if (attrs.Count() > 0)
                    {
                        foreach (PropertyInfo p in attrs)
                        {
                            if (dicparam.ContainsKey(p.Name))
                                dicparam[p.Name] = p.GetValue(objparm);
                            else
                                dicparam.Add(p.Name, p.GetValue(objparm));

                        }
                    }
                }

                #endregion 


                if (dicparam.Count() == 0)
                    throw new Exception($"未传任何参数");


                //先判断是否有参数化
                if (Tool.RegexMatch($@"{Constants.KeyParameterPre}\w+", sql))
                {
                    //表示参数为@name 格式
                    #region 解析@name 参数格式

                    if (!Tool.RegexMatch(Constants.REG_HISQL_PARAM2, sql))
                    {

                        var lstdic = Tool.RegexGrps($@"{Constants.KeyParameterPre}(?<pname>\w+)\b", sql);

                        foreach (Dictionary<string, string> _dic in lstdic)
                        {
                            if (dicparam.ContainsKey(_dic["pname"]))
                            {
                                string n = _dic["pname"];
                                Type _type = dicparam[n].GetType();
                                Regex regex = new Regex(@$"@{n}\b",RegexOptions.IgnoreCase);

                                if (type.IsIn<Type>(Constants.ShortType, Constants.LongType, Constants.DecType, Constants.IntType, Constants.FloatType, Constants.DobType))
                                {
                                    sql = regex.Replace(sql, dicparam[n].ToString());
                                }
                                else if (_type == Constants.BoolType)
                                {
                                    
                                    if (dicparam[n].ToString().ToLower().Trim() == "true")
                                        sql = regex.Replace(sql, "1");
                                    else
                                        sql = regex.Replace(sql, "0");
                                }
                                else if (_type.IsIn<Type>(Constants.DateType, Constants.DateTimeOffsetType))
                                {
                                    DateTime dtime = Convert.ToDateTime(dicparam[n]);
                                    sql = regex.Replace(sql, $"'{dtime.ToString("yyyy-MM-dd HH:mm:ss.fff")}'");
                                }
                                else if (_type.IsIn<Type>(Constants.StringType, Constants.GuidType))
                                {
                                    sql = regex.Replace(sql, $"'{dicparam[n].ToString().ToSqlInject().ToSqlEnChar()}'");
                                }
                                else if (_type.FullName.IndexOf("List") > 0)
                                {

                                    var _dic_p1 = Tool.RegexGrps($@"{Constants.KeyParameterPre}(?<pname>\w+)\b", sql);
                                    var _dic_p2 = Tool.RegexGrps(Constants.REG_HISQL_IN_PARAM2, sql);
                                    string _insql = "";
                                    if (_dic_p1.Count > 0 && _dic_p1.Count == _dic_p2.Count)
                                    {
                                        //foreach(var _o in )

                                        var _typ_string = typeof(List<string>);
                                        var _typ_int = typeof(List<int>);
                                        var _typ_decimal = typeof(List<decimal>);

                                        if (_type == _typ_string)
                                        {
                                            var list = dicparam[n] as List<string>;
                                            _insql = AdoExtensions.ToSqlIn<string>(list.ToArray(), true);
                                        }
                                        else if (_type == _typ_int)
                                        {
                                            var list = dicparam[n] as List<int>;
                                            _insql = AdoExtensions.ToSqlIn<int>(list.ToArray(), false);
                                        }
                                        else if (_type == _typ_decimal)
                                        {
                                            var list = dicparam[n] as List<decimal>;
                                            _insql = AdoExtensions.ToSqlIn<decimal>(list.ToArray(), false);
                                        }
                                        else
                                        {
                                            throw new Exception($"类型[{_type.FullName}]不在允许的in集合内,仅允许List<string>,List<int>,List<decimal> 三种类型");
                                        }

                                        sql = regex.Replace(sql, $"{_insql}");
                                    }
                                    else
                                    {
                                        throw new Exception($"参数 {n} 是集合 只能放在in ({n})中");
                                    }
                                }
                                else if (type.IsIn<Type>(Constants.ObjType))
                                {
                                    sql = regex.Replace(sql, $"'{dicparam[n].ToString().ToSqlInject().ToSqlEnChar()}'");
                                }
                                else
                                {
                                    sql = regex.Replace(sql, $"'{dicparam[n].ToString().ToSqlInject().ToSqlEnChar()}'");
                                }
                            }
                            else
                            {
                                throw new Exception($"参数 {_dic["pname"]} 未设置");
                            }
                        }
                    }
                    else
                        throw new Exception($"参数化名称不能使用@name 又用[$name$] 格式");
                    #endregion

                }
                else if (Tool.RegexMatch(Constants.REG_HISQL_PARAM2, sql))
                {
                    //表示参数模式为[$name$] 格式
                    #region 解析 [$name$] 参数格式

                    if (dicparam.Count > 0)
                    {
                        foreach (string n in dicparam.Keys)
                        {
                            if (Tool.RegexMatch(Constants.REG_HISQL_PARAM, n))
                            {
                                if (sql.IndexOf(n) >= 0)
                                {
                                    Type _type = dicparam[n].GetType();
                                    if (type.IsIn<Type>(Constants.ShortType, Constants.LongType, Constants.DecType, Constants.IntType, Constants.FloatType, Constants.DobType))
                                    {
                                        sql = sql.Replace(n, dicparam[n].ToString());
                                    }
                                    else if (_type == Constants.BoolType)
                                    {
                                        if (dicparam[n].ToString().ToLower().Trim() == "true")
                                            sql = sql.Replace(n, "1");
                                        else
                                            sql = sql.Replace(n, "0");
                                    }
                                    else if (_type.IsIn<Type>(Constants.DateType, Constants.DateTimeOffsetType))
                                    {
                                        DateTime dtime = Convert.ToDateTime(dicparam[n]);
                                        sql = sql.Replace(n, $"'{dtime.ToString("yyyy-MM-dd HH:mm:ss.fff")}'");
                                    }
                                    else if (_type.IsIn<Type>(Constants.StringType, Constants.GuidType))
                                    {
                                        sql = sql.Replace(n, $"'{dicparam[n].ToString().ToSqlInject().ToSqlEnChar()}'");
                                    }
                                    else if (_type.FullName.IndexOf("List") > 0)
                                    {

                                        var _dic_p1 = Tool.RegexGrps(Constants.REG_HISQL_PARAM2, sql);
                                        var _dic_p2 = Tool.RegexGrps(Constants.REG_HISQL_IN_PARAM, sql);
                                        string _insql = "";
                                        if (_dic_p1.Count > 0 && _dic_p1.Count == _dic_p2.Count)
                                        {
                                            //foreach(var _o in )

                                            var _typ_string = typeof(List<string>);
                                            var _typ_int = typeof(List<int>);
                                            var _typ_decimal = typeof(List<decimal>);

                                            if (_type == _typ_string)
                                            {
                                                var list = dicparam[n] as List<string>;
                                                _insql = AdoExtensions.ToSqlIn<string>(list.ToArray(), true);
                                            }
                                            else if (_type == _typ_int)
                                            {
                                                var list = dicparam[n] as List<int>;
                                                _insql = AdoExtensions.ToSqlIn<int>(list.ToArray(), false);
                                            }
                                            else if (_type == _typ_decimal)
                                            {
                                                var list = dicparam[n] as List<decimal>;
                                                _insql = AdoExtensions.ToSqlIn<decimal>(list.ToArray(), false);
                                            }
                                            else
                                            {
                                                throw new Exception($"类型[{_type.FullName}]不在允许的in集合内,仅允许List<string>,List<int>,List<decimal> 三种类型");
                                            }


                                            sql = sql.Replace(n, $"{_insql}");
                                        }
                                        else
                                        {
                                            throw new Exception($"参数 {n} 是集合 只能放在in ({n})中");
                                        }



                                    }
                                    else if (type.IsIn<Type>(Constants.ObjType))
                                    {
                                        sql = sql.Replace(n, $"'{dicparam[n].ToString().ToSqlInject().ToSqlEnChar()}'");
                                    }
                                    else
                                    {
                                        sql = sql.Replace(n, $"'{dicparam[n].ToString().ToSqlInject().ToSqlEnChar()}'");
                                    }
                                }
                                else
                                    throw new Exception($"参数 {n} 设置多余在参数化hisql中未使用");

                            }
                            else
                            {
                                throw new Exception($"参数 {n} 不符合参数规则 规则为[$参数名$]");
                            }


                        }
                    }

                    #endregion
                }



            }
            else
                throw new Exception($"参数化SQL语句中不能出现[\'\"]单引号和又引号这种特殊字段");


            IQuery result = null;
            result = Instance.GetQuery(this.Context.CurrentConnectionConfig);



            result.Context = this.Context;
            result.HiSql(sql, result);
            return result;
        }

 


        #region 数据插入
        public IInsert Insert(string tabname, object objdata)
        {
            IInsert result = Instance.GetInsert(this.Context.CurrentConnectionConfig);
            result.Context = this.Context;
            result.Insert(tabname, objdata);
            return result;
        }
        public IInsert Insert(string tabname, List<object> lstobj)
        {
            IInsert result = Instance.GetInsert(this.Context.CurrentConnectionConfig);
            result.Context = this.Context;
            result.Insert(tabname, lstobj);
            return result;
        }

        public IInsert Insert<T>(string tabname, List<T> lstobj)
        {
            IInsert result = Instance.GetInsert(this.Context.CurrentConnectionConfig);
            result.Context = this.Context;
            result.Insert<T>(tabname, lstobj);
            return result;
        }
        public IInsert Insert<T>(T objdata)
        {
            IInsert result = Instance.GetInsert(this.Context.CurrentConnectionConfig);
            result.Context = this.Context;
            result.Insert<T>(objdata);
            return result;
        }
        public IInsert Insert<T>(string tabname, T objdata)
        {
            IInsert result = Instance.GetInsert(this.Context.CurrentConnectionConfig);
            result.Context = this.Context;
            result.Insert<T>(tabname, objdata);
            return result;
        }
        public IInsert Insert<T>(List<T> lstdata)
        {
            IInsert result = Instance.GetInsert(this.Context.CurrentConnectionConfig);
            result.Context = this.Context;
            result.Insert<T>(lstdata);
            return result;
        }

        #endregion

        /// <summary>
        /// 批量写入
        /// </summary>
        /// <param name="tabname"></param>
        /// <param name="lstdata"></param>
        /// <returns></returns>
        public int BulkCopyExecCommand<T>(TabInfo tabInfo, List<T> lstdata)
        {
            //仅mysql不通过datatable进行数据插入
            if (Context.CurrentConnectionConfig.DbType == DBType.MySql)
            {
                DataTable sourcetable = DataConvert.BuildDataTable(tabInfo);
                Dictionary<string, string> columnMap = new Dictionary<string, string>();
                foreach (DataColumn dc in sourcetable.Columns)
                {
                    columnMap.Add(dc.ColumnName, dc.ColumnName);
                }
                return this.Context.DBO.ExecBulkCopyCommand(lstdata, tabInfo, columnMap);
            }
            else
            {
                DataTable sourcetable = DataConvert.ToTable(lstdata, tabInfo, Context.CurrentConnectionConfig.User, Context.CurrentConnectionConfig.DbType != DBType.DaMeng);//DaMengBulkCopy字段必须和数据库表一致 pengxy on 20220606
                Dictionary<string, string> columnMap = new Dictionary<string, string>();
                foreach (DataColumn dc in sourcetable.Columns)
                {
                    columnMap.Add(dc.ColumnName, dc.ColumnName);
                }
                return this.Context.DBO.ExecBulkCopyCommand(sourcetable, tabInfo, columnMap);
            }
            
           

        }
        /// <summary>
        /// 批量写入
        /// </summary>
        /// <param name="tabname"></param>
        /// <param name="lstdata"></param>
        /// <returns></returns>
        public int BulkCopyExecCommand(TabInfo tabInfo, DataTable sourcetable)
        {
            Dictionary<string, string> columnMap = new Dictionary<string, string>();
            foreach (DataColumn dc in sourcetable.Columns)
            {
                columnMap.Add(dc.ColumnName, dc.ColumnName);
            }
            return  this.Context.DBO.ExecBulkCopyCommand(sourcetable, tabInfo, columnMap);
        }
        public  Task<int> BulkCopyExecCommandAsyc(TabInfo tabInfo, DataTable sourcetable)
        {
            Dictionary<string, string> columnMap = new Dictionary<string, string>();
            foreach (DataColumn dc in sourcetable.Columns)
            {
                columnMap.Add(dc.ColumnName, dc.ColumnName);
            }
            return this.Context.DBO.ExecBulkCopyCommandAsync(sourcetable, tabInfo, columnMap);
        }
        public Task<int> BulkCopyExecCommandAsyc<T>(TabInfo tabInfo, List<T> lstdata)
        {
            Dictionary<string, string> columnMap = new Dictionary<string, string>();
            if (Context.CurrentConnectionConfig.DbType == DBType.MySql)
            {
                DataTable sourcetable = DataConvert.BuildDataTable(tabInfo);
                
                foreach (DataColumn dc in sourcetable.Columns)
                {
                    columnMap.Add(dc.ColumnName, dc.ColumnName);
                }

                return this.Context.DBO.ExecBulkCopyCommandAsync(lstdata, tabInfo, columnMap);
            }
            else
            {
                DataTable sourcetable = DataConvert.ToTable(lstdata, tabInfo, this.Context.CurrentConnectionConfig.User);
                foreach (DataColumn dc in sourcetable.Columns)
                {
                    columnMap.Add(dc.ColumnName, dc.ColumnName);
                }
                return this.Context.DBO.ExecBulkCopyCommandAsync(sourcetable, tabInfo, columnMap);
            }
            
        }



        public IInsert Modi(string tabname, List<object> lstobj)
        {
            IInsert result = Instance.GetInsert(this.Context.CurrentConnectionConfig);
            result.Context = this.Context;
            result.Modi(tabname, lstobj);
            return result;
        }
        public IInsert Modi<T>(string tabname, T objdata) {
            IInsert result = Instance.GetInsert(this.Context.CurrentConnectionConfig);
            result.Context = this.Context;
            result.Modi<T>(tabname, objdata);
            return result;
        }
        public IInsert Modi<T>(T objdata)
        {
            IInsert result = Instance.GetInsert(this.Context.CurrentConnectionConfig);
            result.Context = this.Context;
            result.Modi<T>(objdata);
            return result;
        }
        public IInsert Modi<T>(List<T> lstdata)
        {
            IInsert result = Instance.GetInsert(this.Context.CurrentConnectionConfig);
            result.Context = this.Context;
            result.Modi<T>(lstdata);
            return result;
        }
        public IInsert Modi<T>(string tabname, List<T> lstdata)
        {
            IInsert result = Instance.GetInsert(this.Context.CurrentConnectionConfig);
            result.Context = this.Context;
            result.Modi<T>(tabname, lstdata);
            return result;
        }

        #endregion

        #region  更新
        public IUpdate Update(string tabname, object objdata)
        {
            IUpdate result = Instance.GetUpdate(this.Context.CurrentConnectionConfig);
            result.Context = this.Context;
            result.Update(tabname, objdata);
            return result;
        }

        public IUpdate Update(string tabname, List<object> lstdata)
        {
            IUpdate result = Instance.GetUpdate(this.Context.CurrentConnectionConfig);
            result.Context = this.Context;
            result.Update(tabname, lstdata);
            return result;
        }

        public IUpdate Update(string tabname)
        {
            IUpdate result = Instance.GetUpdate(this.Context.CurrentConnectionConfig);
            result.Context = this.Context;
            result.Update(tabname);
            return result;
        }

        public IUpdate Update<T>(T objdata)
        {
            IUpdate result = Instance.GetUpdate(this.Context.CurrentConnectionConfig);
            result.Context = this.Context;
            result.Update<T>(objdata);
            return result;
        }

        public IUpdate Update<T>(string tabname, T objdata)
        {
            IUpdate result = Instance.GetUpdate(this.Context.CurrentConnectionConfig);
            result.Context = this.Context;
            result.Update<T>(tabname, objdata);
            return result;
        }

        public IUpdate Update<T>(List<T> lstobj)
        {
            IUpdate result = Instance.GetUpdate(this.Context.CurrentConnectionConfig);
            result.Context = this.Context;
            result.Update<T>(lstobj);
            return result;
        }
        public IUpdate Update<T>(string tabname, List<T> lstobj)
        {
            IUpdate result = Instance.GetUpdate(this.Context.CurrentConnectionConfig);
            result.Context = this.Context;
            result.Update<T>(tabname, lstobj);
            return result;
        }
        #endregion

        #region 删除
        public IDelete Delete(string tabname)
        {
            IDelete result = Instance.GetDelete(this.Context.CurrentConnectionConfig);
            result.Context = this.Context;
            result.Delete (tabname);
            return result;
        }
        public IDelete Delete(string tabname, object objdata)
        {
            IDelete result = Instance.GetDelete(this.Context.CurrentConnectionConfig);
            result.Context = this.Context;
            result.Delete(tabname,objdata);
            return result;
        }
        public IDelete Delete(string tabname, List<object> objlst) 
        {
            IDelete result = Instance.GetDelete(this.Context.CurrentConnectionConfig);
            result.Context = this.Context;
            result.Delete(tabname, objlst);
            return result;

        }

        public IDelete Delete<T>(T objdata)
        {
            IDelete result = Instance.GetDelete(this.Context.CurrentConnectionConfig);
            result.Context = this.Context;
            result.Delete<T>(objdata);
            return result;
        }
        public IDelete Delete<T>(string tabname, List<T> objlst)
        {
            IDelete result = Instance.GetDelete(this.Context.CurrentConnectionConfig);
            result.Context = this.Context;
            result.Delete<T>(tabname, objlst);
            return result;
        }
        public IDelete Delete<T>(List<T> objlst)
        {
            IDelete result = Instance.GetDelete(this.Context.CurrentConnectionConfig);
            result.Context = this.Context;
            result.Delete<T>( objlst);
            return result;
        }

        /// <summary>
        /// 无记录删除 高风险操作
        /// </summary>
        /// <param name="tabname"></param>
        /// <returns></returns>
        public IDelete TrunCate(string tabname)
        {
            IDelete result = Instance.GetDelete(this.Context.CurrentConnectionConfig);
            result.Context = this.Context;
            result.TrunCate(tabname);
            return result;
        }

        /// <summary>
        /// 删除表(高风险操作)
        /// </summary>
        /// <param name="tabname"></param>
        /// <returns></returns>
        public IDelete Drop(string tabname)
        {
            IDelete result = Instance.GetDelete(this.Context.CurrentConnectionConfig);
            result.Context = this.Context;
            result.Drop(tabname);
            return result;
        }

        #endregion


        #region  打开，关闭，释放

        public virtual void Close()
        {
            if (this.Context.DBO != null)
            {
                this.Context.DBO.Close();
            }
        }

        public virtual void Open()
        {
            if (this.Context.DBO != null)
            {
                this.Context.DBO.Open();
            }
        }

        public virtual void Dispose()
        {
            if (this.Context.DBO != null)
            {
                this.Context.DBO.Dispose();
            }
        }

        public void BeginTran()
        {
            this.Context.BeginTran();
        }

        public void CommitTran()
        {
            this.Context.CommitTran();
            this.Close();
        }

        public void RollBackTran()
        {
            this.Context.RollBackTran();
            this.Close();
        }

        public void BeginTran(IsolationLevel iso)
        {
            this.Context.BeginTran(iso);
        }

       





        #endregion
    }
}
