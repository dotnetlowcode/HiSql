using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HiSql.Common.Entities.TabLog;
namespace HiSql
{
    public class UpdateProvider : IUpdate
    {

        TableDefinition _table;
        List<FieldDefinition> _list_only_field = new List<FieldDefinition>();
        List<FieldDefinition> _list_exclude_field = new List<FieldDefinition>();
        List<FilterDefinition> _list_filter = new List<FilterDefinition>();

        List<Dictionary<string, string>> _values = new List<Dictionary<string, string>>();
        Filter _where;


        bool _onlywhere = false;

        List<object> _list_data = new List<object>();

        SynTaxQueue _queue = new SynTaxQueue();

        public HiSqlProvider Context { get; set; }

        public TableDefinition Table
        {
            get { return _table; }
        }

        public List<FieldDefinition> FieldsOnly
        {
            get { return _list_only_field; }
        }

        public List<FieldDefinition> FieldsExclude
        {
            get { return _list_exclude_field; }
        }
        public List<FilterDefinition> Wheres
        {
            get { return _list_filter; }
        }

        public List<object> Data
        {
            get { return _list_data; }
        }
        public Filter Filters
        {
            get { return _where; }
        }

        public bool IsOnlyWhere
        {
            get { return _onlywhere; }
        }

        object _obkey = new object();

        IDbConfig _dbConfig = null;
        public UpdateProvider()
        {

        }
        IDbConfig buildConfig(DBType dBType)
        {
            lock (_obkey)
            {
                if (_dbConfig == null)
                {
                    _dbConfig = (IDbConfig)Instance.CreateInstance<IDbConfig>($"{Constants.NameSpace}.{dBType.ToString()}{DbInterFace.Config.ToString()}");
                    _dbConfig.Init();
                }
            }
            return _dbConfig;
        }

        public IUpdate Exclude(params string[] fields)
        {
            if (!_queue.HasQueue("only"))
            {
                if (!_queue.HasQueue("exclude"))
                {
                    foreach (string fname in fields)
                    {
                        FieldDefinition fieldDefinition = new FieldDefinition();
                        Dictionary<string, string> _dic = Tool.RegexGrp(Constants.REG_NAME, fname);
                        if (_dic.Count() > 0)
                        {
                            fieldDefinition.FieldName = _dic["name"].ToString();
                            fieldDefinition.AsFieldName = _dic["name"].ToString();
                            _list_exclude_field.Add(fieldDefinition);
                        }
                    }
                    _queue.Add("exclude");
                }
                else
                    throw new Exception($"已经指定[Exclude]方法 无法重复指定[Exclude]");
            }
            else
                throw new Exception($"已经指定[Only]方法 无法再指定[Exclude]");


            return this;
        }

        public IUpdate Only(params string[] fields)
        {
            if (!_queue.HasQueue("exclude"))
            {
                if (!_queue.HasQueue("only"))
                {
                    foreach (string name in fields)
                    {
                        FieldDefinition fieldDefinition = new FieldDefinition();
                        Dictionary<string, string> _dic = Tool.RegexGrp(Constants.REG_NAME, name);
                        if (_dic.Count() > 0)
                        {
                            fieldDefinition.FieldName = _dic["name"].ToString();
                            fieldDefinition.AsFieldName = _dic["name"].ToString();
                            _list_only_field.Add(fieldDefinition);
                        }
                    }
                    _queue.Add("only");
                }
                else
                    throw new Exception($"已经指定[Only]方法 无法重复指定[Only]");
            }
            else
                throw new Exception($"已经指定[Exclude]方法 无法再指定[Only]");
            return this;
        }


        public int ExecCommand()
        {
            return this.ExecCommandAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }


        /// <summary>
        /// 将当前操作向数据库执行
        /// </summary>
        /// <param name="credentialCallback">操作凭证</param>
        /// <returns></returns>
        public int ExecCommand(Action<HiSql.Interface.TabLog.Credential> credentialCallback)
        {
            return this.ExecCommandAsync(credentialCallback).ConfigureAwait(false).GetAwaiter().GetResult();
        }


        public async Task<int> ExecCommandAsync()
        {
            string _sql = this.ToSql();
            int updateCount = 0;
            await this.RecordLog(async () =>
            {
                updateCount = await this.Context.DBO.ExecCommandAsync(_sql);
                return updateCount > 0;
            });
            return updateCount;
        }



        public async Task<int> ExecCommandAsync(Action<HiSql.Interface.TabLog.Credential> credentialCallback)
        {
            string _sql = this.ToSql();
            var updateCount = 0;
            var credentialObj = await this.RecordLog(async () =>
            {
                updateCount = await this.Context.DBO.ExecCommandAsync(_sql);
                return updateCount > 0;
            });
            credentialCallback(credentialObj);
            return updateCount;
        }


        public virtual string ToSql()
        {

            //请重写该方法
            return "";
        }

        public IUpdate Update(string tabname)
        {
            if (!_queue.HasQueue("table"))
            {
                _table = new TableDefinition(tabname);
                _queue.Add("table");

            }
            else
            {
                throw new Exception($"已经指定了一个Update 不允许重复指定");

            }
            return this;
        }
        public IUpdate Update(string tabname, object objdata)
        {

            if (!_queue.HasQueue("table"))
            {
                _table = new TableDefinition(tabname);
                _queue.Add("table|data");
                _list_data.Add(objdata);
            }
            else
            {
                throw new Exception($"已经指定了一个Update 不允许重复指定");
            }

            return this;
        }

        public IUpdate Update(string tabname, List<object> lstdata)
        {
            if (!_queue.HasQueue("table"))
            {
                _table = new TableDefinition(tabname);
                var _typname = "";
                if (lstdata.Count > 0)
                    _typname = lstdata[0].GetType().FullName;
                //_list_data = lstdata;
                if (_typname.IndexOf("TDynamic") >= 0 || _typname.IndexOf("ExpandoObject") >= 0)
                {
                    foreach (var obj in lstdata)
                    {
                        if (_typname.IndexOf("TDynamic") >= 0)
                        {
                            TDynamic dyn = (dynamic)obj;
                            _list_data.Add((Dictionary<string, object>)dyn);
                        }
                        else if (_typname.IndexOf("ExpandoObject") >= 0)
                        {
                            TDynamic dyn = new TDynamic(obj);
                            _list_data.Add((Dictionary<string, object>)dyn);
                        }
                    }
                }
                else
                    _list_data = lstdata;

                _queue.Add("table|list");
            }
            else
                throw new Exception($"已经指定了一个Update 不允许重复指定");
            return this;
        }

        public IUpdate Set(object obj)
        {
            if (_queue.HasQueue("table", true))
            {
                if (!_queue.HasQueue("set"))
                {
                    _queue.Add("set");
                    _list_data.Add(obj);
                }
                else
                    throw new Exception($"已经指定了一个Set 不允许重复指定");
            }
            else
                throw new Exception("[Set]方法只支持Update(TableName).Set(..) 语法");
            return this;
        }

        public IUpdate Set(params string[] fields)
        {
            if (!_queue.HasQueue("set"))
            {
                throw new Exception("暂不支持该方法");
            }
            else
                throw new Exception($"已经指定了一个Set 不允许重复指定");
            //return this;
        }


        /// <summary>
        /// 仅使用该方法指定的where进行条件更新
        /// hisql语法
        /// </summary>
        /// <param name="sqlwhere"></param>
        /// <returns></returns>
        public IUpdate OnlyWhere(string sqlwhere)
        {
            Where(sqlwhere);
            _onlywhere = true;
            return this;
        }


        /// <summary>
        /// 仅使用该方法指定的where进行条件更新
        /// 
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public IUpdate OnlyWhere(Filter where)
        {
            Where(where);
            _onlywhere = true;
            return this;
        }

        public IUpdate Where(string sqlwhere)
        {
            //需要检测语法
            if (!_queue.HasQueue("where"))
            {
                if (string.IsNullOrEmpty(sqlwhere.Trim()))
                {
                    throw new Exception($"指定的hisql where语句[{sqlwhere}]为空");
                }

                Filter where = new Filter() { sqlwhere };

                _where = where;
                _queue.Add("where");

            }
            else
                throw new Exception($"已经指定了一个Where 不允许重复指定");
            return this;
        }

        public IUpdate Where(Filter where)
        {
            if (!_queue.HasQueue("table"))
                throw new Exception($"请先指定要更新的表");
            if (!_queue.HasQueue("where"))
            {
                if (where != null && where.Elements.Count > 0)
                {
                    _list_filter = where.Elements;
                    foreach (FilterDefinition filterDefinition in _list_filter)
                    {
                        if (string.IsNullOrEmpty(filterDefinition.Field.TabName))
                        {
                            filterDefinition.Field.TabName = _table.TabName;
                            filterDefinition.Field.AsTabName = _table.AsTabName;
                        }
                    }
                    _queue.Add("where");

                }

            }
            else
                throw new Exception($"已经指定了一个Where 不允许重复指定");
            return this;
        }

        public IUpdate Update<T>(T objdata)
        {
            if (!_queue.HasQueue("table"))
            {
                _table = new TableDefinition(typeof(T).Name);
                _list_data.Add((object)objdata);
                _queue.Add("table");
            }
            else
                throw new Exception($"已经指定了一个Update 不允许重复指定");
            return this;
        }

        public IUpdate Update<T>(string tabname, T objdata)
        {
            if (!_queue.HasQueue("table"))
            {
                _table = new TableDefinition(tabname);
                _list_data.Add((object)objdata);
                _queue.Add("table");
            }
            else
                throw new Exception($"已经指定了一个Update 不允许重复指定");
            return this;
        }

        public IUpdate Update<T>(List<T> lstobj)
        {
            if (!_queue.HasQueue("table"))
            {
                _table = new TableDefinition(typeof(T).Name);
                foreach (T t in lstobj)
                {
                    _list_data.Add((object)t);
                }
                _queue.Add("table|data");
            }
            else
                throw new Exception($"已经指定了一个Update 不允许重复指定");
            return this;
        }

        public IUpdate Update<T>(string tabname, List<T> lstobj)
        {
            if (!_queue.HasQueue("table"))
            {
                _table = new TableDefinition(tabname);
                //foreach (T t in lstobj)
                //{
                //    _list_data.Add((object)t);
                //}

                foreach (var obj in lstobj)
                {
                    var _typname = obj.GetType().FullName;
                    if (_typname.IndexOf("TDynamic") >= 0)
                    {
                        TDynamic dyn = (dynamic)obj;
                        _list_data.Add((Dictionary<string, object>)dyn);
                    }
                    else if (_typname.IndexOf("ExpandoObject") >= 0)
                    {
                        TDynamic dyn = new TDynamic(obj);
                        _list_data.Add((Dictionary<string, object>)dyn);
                    }
                    else
                        _list_data.Add((object)obj);
                }
                _queue.Add("table|data");
            }
            else
                throw new Exception($"已经指定了一个Update 不允许重复指定");
            return this;
        }

        /// <summary>
        /// 表数据检测
        /// </summary>
        /// <param name="hiColumns"></param>
        /// <param name="lstdata"></param>
        /// <returns></returns>
        public Tuple<List<Dictionary<string, string>>, List<Dictionary<string, string>>> CheckAllData(TableDefinition table, TabInfo tabinfo, List<string> fields, List<object> lstdata, bool hisqlwhere, bool isonly)
        {
            Tuple<List<Dictionary<string, string>>, List<Dictionary<string, string>>> rtn = new Tuple<List<Dictionary<string, string>>, List<Dictionary<string, string>>>(null, null);
            if (table != null)
            {
                _dbConfig = buildConfig(Context.CurrentConnectionConfig.DbType);
                List<Dictionary<string, string>> lst_value = new List<Dictionary<string, string>>();
                List<Dictionary<string, string>> lst_primary = new List<Dictionary<string, string>>();
                if (lstdata.Count > 0)
                {
                    Type type = lstdata[0].GetType();
                    Type _typ_dic = typeof(Dictionary<string, string>);
                    Type _typ_dic_obj = typeof(Dictionary<string, object>);
                    bool _isdic = type == _typ_dic || type == _typ_dic_obj;
                    List<PropertyInfo> attrs = type.GetProperties().Where(p => p.MemberType == MemberTypes.Property && p.CanRead == true).ToList();

                    List<HiColumn> hiColumns = tabinfo.GetColumns;
                    //将有配正则校验的列去重统计
                    Dictionary<string, HashSet<string>> dic_hash_reg = new Dictionary<string, HashSet<string>>();
                    Dictionary<string, HashSet<string>> dic_hash_tab = new Dictionary<string, HashSet<string>>();
                    var arrcol_reg = hiColumns.Where(h => !string.IsNullOrEmpty(h.Regex)).ToList();
                    var arrcol_tab = hiColumns.Where(h => h.IsRefTab).ToList();

                    var arrcol = hiColumns.Where(h => !string.IsNullOrEmpty(h.Regex) || h.IsRefTab).ToList();
                    foreach (HiColumn hi in arrcol)
                    {
                        dic_hash_reg.Add(hi.FieldName, new HashSet<string>(System.StringComparer.Create(System.Globalization.CultureInfo.CurrentCulture, true)));
                    }
                    int _rowidx = 0;
                    string _value;
                    if (_isdic)
                    {
                        if (type == _typ_dic_obj)
                        {
                            #region Dictionary<string, object>
                            foreach (Dictionary<string, object> _o in lstdata)
                            {
                                _rowidx++;
                                Dictionary<string, string> _values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                                Dictionary<string, string> _primary = new Dictionary<string, string>();
                                foreach (HiColumn hiColumn in hiColumns)
                                {
                                    _value = "";
                                    #region 判断必填字段
                                    if (hiColumn.IsBllKey && !_o.ContainsKey(hiColumn.FieldName) && !hisqlwhere)
                                    {
                                        throw new Exception($"行[{_rowidx}] 字段[{hiColumn.FieldName}] 为业务主键或表主键 更新表数据时必填");
                                    }
                                    #endregion  
                                    if (_o.ContainsKey(hiColumn.FieldName))
                                    {
                                        #region 将值转成string 及特殊处理
                                        if (hiColumn.FieldType.IsIn<HiType>(HiType.DATE, HiType.DATETIME))
                                        {

                                            if (_o[hiColumn.FieldName] != null)
                                            {
                                                DateTime dtime = string.IsNullOrEmpty(_o[hiColumn.FieldName].ToString()) ? DateTime.MinValue : Convert.ToDateTime(_o[hiColumn.FieldName].ToString());
                                                _value = dtime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                            }
                                            else
                                            {
                                                _value = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                            }
                                        }
                                        else if (hiColumn.FieldType.IsIn<HiType>(HiType.BOOL))
                                        {
                                            if ((bool)_o[hiColumn.FieldName] == true)
                                            {
                                                _value = "1";
                                            }
                                            else
                                                _value = "0";
                                        }
                                        else if (hiColumn.FieldType.IsNumberField())
                                        {
                                            _value = _o[hiColumn.FieldName] == null ? "0" : _o[hiColumn.FieldName].ToString();
                                        }
                                        else
                                            _value = _o[hiColumn.FieldName] == null ? "" : _o[hiColumn.FieldName].ToString();
                                        #endregion

                                        #region 是否需要正则校验
                                        if (arrcol.Any(h => h.FieldName == hiColumn.FieldName))
                                        {
                                            dic_hash_reg[hiColumn.FieldName].Add(_value);
                                        }
                                        #endregion

                                        #region 通用数据有效性校验
                                        if (!Constants.IsStandardCreateField(hiColumn.FieldName))
                                        {
                                            var result = checkFieldValue(hiColumn, _rowidx, _value);
                                            if (result.Item1)
                                            {
                                                _values.Add(hiColumn.FieldName, result.Item2);
                                                if (hiColumn.IsBllKey)
                                                    _primary.Add(hiColumn.FieldName, result.Item2);
                                            }

                                            if (fields.Count > 0 && !Constants.IsStandardModiField(hiColumn.FieldName))
                                            {
                                                //说明有指定更新，或排除更新字段
                                                if (isonly)
                                                {
                                                    //仅在fields列表中的 才会更新字段
                                                    if (fields.Where(f => f.ToLower() == hiColumn.FieldName.ToLower()).Count() == 0 || hiColumn.IsBllKey)
                                                    {
                                                        if (_values.ContainsKey(hiColumn.FieldName))
                                                            _values.Remove(hiColumn.FieldName);
                                                    }
                                                }
                                                else
                                                {
                                                    if (fields.Where(f => f.ToLower() == hiColumn.FieldName.ToLower()).Count() > 0 || hiColumn.IsBllKey)
                                                    {
                                                        if (_values.ContainsKey(hiColumn.FieldName))
                                                            _values.Remove(hiColumn.FieldName);
                                                    }
                                                }
                                            }

                                        }

                                        #endregion
                                    }


                                }
                                if (_values.Count > 0)
                                {
                                    var _column = hiColumns.Where(h => h.FieldName.ToLower() == "moditime").FirstOrDefault();
                                    if (_column != null)
                                    {
                                        if (_values.ContainsKey(_column.FieldName))
                                            _values.Remove(_column.FieldName);

                                        if (Context.CurrentConnectionConfig.DbType == DBType.Oracle)
                                            _values.Add(_column.FieldName, $"timestamp'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}'");
                                        else
                                            _values.Add(_column.FieldName, $"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}'");
                                    }
                                    _column = hiColumns.Where(h => h.FieldName.ToLower() == "modiname").FirstOrDefault();
                                    if (_column != null)
                                    {
                                        if (_values.ContainsKey(_column.FieldName))
                                            _values.Remove(_column.FieldName);
                                        _values.Add(_column.FieldName, $"'{this.Context.CurrentConnectionConfig.User}'");
                                    }
                                    lst_value.Add(_values);
                                    lst_primary.Add(_primary);
                                }
                                else
                                    throw new Exception($"行[{_rowidx}] 数据无字段可更新 ");

                            }
                            #endregion
                        }
                        else
                        {
                            #region Dictionary<string, string> 
                            foreach (Dictionary<string, string> _o in lstdata)
                            {
                                _rowidx++;
                                Dictionary<string, string> _values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                                Dictionary<string, string> _primary = new Dictionary<string, string>();
                                foreach (HiColumn hiColumn in hiColumns)
                                {
                                    _value = "";




                                    #region 判断必填字段
                                    if (hiColumn.IsBllKey && !_o.ContainsKey(hiColumn.FieldName) && !hisqlwhere)
                                    {
                                        throw new Exception($"行[{_rowidx}] 字段[{hiColumn.FieldName}] 为业务主键或表主键 更新表数据时必填");
                                    }
                                    #endregion  

                                    if (_o.ContainsKey(hiColumn.FieldName))
                                    {
                                        _value = _o[hiColumn.FieldName];

                                        #region 是否需要正则校验
                                        if (arrcol.Any(h => h.FieldName == hiColumn.FieldName))
                                        {
                                            dic_hash_reg[hiColumn.FieldName].Add(_value);
                                        }
                                        #endregion

                                        #region 通用数据有效性校验
                                        if (!Constants.IsStandardCreateField(hiColumn.FieldName))
                                        {
                                            var result = checkFieldValue(hiColumn, _rowidx, _value);
                                            if (result.Item1)
                                            {
                                                _values.Add(hiColumn.FieldName, result.Item2);
                                                if (hiColumn.IsBllKey)
                                                    _primary.Add(hiColumn.FieldName, result.Item2);
                                            }

                                            if (fields.Count > 0 && !Constants.IsStandardModiField(hiColumn.FieldName))
                                            {
                                                //说明有指定更新，或排除更新字段
                                                if (isonly)
                                                {
                                                    //仅在fields列表中的 才会更新字段
                                                    if (fields.Where(f => f.ToLower() == hiColumn.FieldName.ToLower()).Count() == 0 || hiColumn.IsBllKey)
                                                    {
                                                        if (_values.ContainsKey(hiColumn.FieldName))
                                                            _values.Remove(hiColumn.FieldName);
                                                    }
                                                }
                                                else
                                                {
                                                    if (fields.Where(f => f.ToLower() == hiColumn.FieldName.ToLower()).Count() > 0 || hiColumn.IsBllKey)
                                                    {
                                                        if (_values.ContainsKey(hiColumn.FieldName))
                                                            _values.Remove(hiColumn.FieldName);
                                                    }
                                                }
                                            }

                                        }

                                        #endregion



                                    }
                                }




                                if (_values.Count > 0)
                                {
                                    var _column = hiColumns.Where(h => h.FieldName.ToLower() == "moditime").FirstOrDefault();
                                    if (_column != null)
                                    {
                                        if (_values.ContainsKey(_column.FieldName))
                                            _values.Remove(_column.FieldName);
                                        if (Context.CurrentConnectionConfig.DbType == DBType.Oracle)
                                            _values.Add(_column.FieldName, $"timestamp'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}'");
                                        else
                                            _values.Add(_column.FieldName, $"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}'");
                                    }
                                    _column = hiColumns.Where(h => h.FieldName.ToLower() == "modiname").FirstOrDefault();
                                    if (_column != null)
                                    {
                                        if (_values.ContainsKey(_column.FieldName))
                                            _values.Remove(_column.FieldName);
                                        _values.Add(_column.FieldName, $"'{this.Context.CurrentConnectionConfig.User}'");
                                    }
                                    lst_value.Add(_values);
                                    lst_primary.Add(_primary);
                                }
                                else
                                    throw new Exception($"行[{_rowidx}] 数据无字段可更新 ");
                            }

                            #endregion
                        }
                    }
                    else
                    {

                        Dictionary<string, PropertyInfo> _dic_proinfo = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);
                        foreach (PropertyInfo pinfo in attrs)
                        {
                            if (!_dic_proinfo.ContainsKey(pinfo.Name.ToLower()))
                            {
                                _dic_proinfo.Add(pinfo.Name.ToLower(), pinfo);
                            }
                        }

                        foreach (object objdata in lstdata)
                        {
                            _rowidx++;
                            Dictionary<string, string> _dic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                            Dictionary<string, string> _values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                            Dictionary<string, string> _primary = new Dictionary<string, string>();
                            foreach (HiColumn hiColumn in hiColumns)
                            {
                                _value = "";
                                //var objprop = attrs.Where(p => p.Name.ToLower() == hiColumn.FieldName.ToLower()).FirstOrDefault();
                                PropertyInfo objprop = null;
                                if (_dic_proinfo.ContainsKey(hiColumn.FieldName.ToLower()))
                                {
                                    objprop = _dic_proinfo[hiColumn.FieldName.ToLower()];
                                }

                                #region  判断必填 及自增长
                                if (hiColumn.IsRequire && !hiColumn.IsIdentity && objprop == null && !hisqlwhere)
                                {
                                    throw new Exception($"行[{_rowidx}] 缺少字段[{hiColumn.FieldName}] 为必填字段");
                                }




                                if (hiColumn.IsIdentity && _dic.ContainsKey(hiColumn.FieldName))
                                {
                                    _value = _dic[hiColumn.FieldName].ToString();
                                    if (_value == "0" || string.IsNullOrEmpty(_value))
                                        continue;
                                    else
                                        throw new Exception($"行[{_rowidx}] 字段[{hiColumn.FieldName}] 为自增长字段 不需要外部赋值");
                                }
                                #endregion
                                if (objprop != null && objprop.GetValue(objdata) != null)
                                {
                                    #region 将值转成string 及特殊处理
                                    if (hiColumn.FieldType.IsIn<HiType>(HiType.DATE, HiType.DATETIME))
                                    {
                                        DateTime dtime = (DateTime)objprop.GetValue(objdata);
                                        if (dtime != null && dtime != DateTime.MinValue)
                                        {
                                            _dic.Add(hiColumn.FieldName, dtime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                        }
                                        else
                                        {
                                            _dic.Add(hiColumn.FieldName, DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                        }
                                    }
                                    else if (hiColumn.FieldType.IsIn<HiType>(HiType.BOOL))
                                    {
                                        if ((bool)objprop.GetValue(objdata) == true)
                                        {
                                            _dic.Add(hiColumn.FieldName, "1");
                                        }
                                        else
                                            _dic.Add(hiColumn.FieldName, "0");
                                    }
                                    else if (hiColumn.FieldType.IsIn<HiType>(HiType.INT, HiType.SMALLINT))
                                    {
                                        //可能是枚举型 枚举型需要获取数值
                                        if (objprop.PropertyType.Name.Contains("Bool"))
                                        {
                                            bool _boolval = (bool)objprop.GetValue(objdata);
                                            _dic.Add(hiColumn.FieldName, _boolval ? "1" : "0");
                                        }
                                        else
                                        {
                                            string _val = objprop.GetValue(objdata).ToString();
                                            if (Tool.IsInt(_val))
                                            {
                                                _dic.Add(hiColumn.FieldName, _val);
                                            }
                                            else if (Tool.RegexMatch("`(?<field>[-\\w_]+)`", _val))
                                            {
                                                _dic.Add(hiColumn.FieldName, _val);
                                            }
                                            else
                                                _dic.Add(hiColumn.FieldName, ((int)objprop.GetValue(objdata)).ToString());
                                            //_dic.Add(hiColumn.FieldName, ((int)objprop.GetValue(objdata)).ToString());
                                        }

                                    }
                                    else
                                    {
                                        if (hiColumn.IsPrimary && hiColumn.FieldType.IsIn<HiType>(HiType.VARCHAR, HiType.NVARCHAR, HiType.CHAR, HiType.NCHAR, HiType.GUID))
                                        {
                                            string _val = objprop.GetValue(objdata).ToString();
                                            if (string.IsNullOrEmpty(_val))
                                                _dic.Add(hiColumn.FieldName, _dbConfig.Key_Char_Default);
                                            else
                                                _dic.Add(hiColumn.FieldName, _val);
                                        }
                                        else
                                            _dic.Add(hiColumn.FieldName, objprop.GetValue(objdata).ToString());
                                    }
                                    _value = _dic[hiColumn.FieldName];
                                    #endregion


                                    #region 是否需要正则校验
                                    if (arrcol.Any(h => h.FieldName == hiColumn.FieldName))
                                    {
                                        dic_hash_reg[hiColumn.FieldName].Add(_dic[hiColumn.FieldName]);
                                    }

                                    #endregion

                                    #region 通用数据有效性校验
                                    if (!Constants.IsStandardCreateField(hiColumn.FieldName))
                                    {
                                        var result = checkFieldValue(hiColumn, _rowidx, _value);
                                        if (result.Item1)
                                        {
                                            _values.Add(hiColumn.FieldName, result.Item2);
                                            if (hiColumn.IsBllKey)
                                                _primary.Add(hiColumn.FieldName, result.Item2);
                                        }

                                        if (fields.Count > 0 && !Constants.IsStandardModiField(hiColumn.FieldName))
                                        {
                                            //说明有指定更新，或排除更新字段
                                            if (isonly)
                                            {
                                                //仅在fields列表中的 才会更新字段
                                                if (fields.Where(f => f.ToLower() == hiColumn.FieldName.ToLower()).Count() == 0 || hiColumn.IsBllKey)
                                                {
                                                    if (_values.ContainsKey(hiColumn.FieldName))
                                                        _values.Remove(hiColumn.FieldName);
                                                }
                                            }
                                            else
                                            {
                                                if (fields.Where(f => f.ToLower() == hiColumn.FieldName.ToLower()).Count() > 0 || hiColumn.IsBllKey)
                                                {
                                                    if (_values.ContainsKey(hiColumn.FieldName))
                                                        _values.Remove(hiColumn.FieldName);
                                                }
                                            }
                                        }

                                    }

                                    #endregion
                                }
                                else
                                {
                                    //if (Constants.IsStandardModiField(hiColumn.FieldName))
                                    //{
                                    //    if (_values.ContainsKey(hiColumn.FieldName))
                                    //        _values.Remove(hiColumn.FieldName);

                                    //    if (Constants.IsStandardTimeField(hiColumn.FieldName))
                                    //        _values.Add(hiColumn.FieldName,$"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}'");
                                    //    else
                                    //        _values.Add(hiColumn.FieldName, $"'{this.Context.CurrentConnectionConfig.User}'");
                                    //}
                                }
                            }
                            if (_values.Count > 0)
                            {
                                var _column = hiColumns.Where(h => h.FieldName.ToLower() == "moditime").FirstOrDefault();
                                if (_column != null)
                                {
                                    if (_values.ContainsKey(_column.FieldName))
                                        _values.Remove(_column.FieldName);
                                    if (Context.CurrentConnectionConfig.DbType == DBType.Oracle)
                                        _values.Add(_column.FieldName, $"timestamp'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}'");
                                    else
                                        _values.Add(_column.FieldName, $"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}'");
                                }
                                _column = hiColumns.Where(h => h.FieldName.ToLower() == "modiname").FirstOrDefault();
                                if (_column != null)
                                {
                                    if (_values.ContainsKey(_column.FieldName))
                                        _values.Remove(_column.FieldName);
                                    _values.Add(_column.FieldName, $"'{this.Context.CurrentConnectionConfig.User}'");
                                }


                                lst_value.Add(_values);
                                lst_primary.Add(_primary);
                            }
                            else
                                throw new Exception($"行[{_rowidx}] 数据无字段可更新 ");
                        }
                    }


                    #region 正则校验匹配 是否合法
                    foreach (HiColumn hiColumn in arrcol_reg)
                    {
                        Regex _regex = new Regex(hiColumn.Regex, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        if (dic_hash_reg.ContainsKey(hiColumn.FieldName))
                        {
                            foreach (string n in dic_hash_reg[hiColumn.FieldName])
                            {
                                if (!_regex.Match(n).Success)
                                {
                                    throw new Exception($@"列[{hiColumn.FieldName}]值[{n}] 不符合业务配置 {hiColumn.Regex} 要求");
                                }
                            }

                        }
                    }


                    //表关联配置校验
                    if (arrcol_tab.Count > 0 && Context.CurrentConnectionConfig.IsCheckTableRefData)
                    {
                        HiSqlClient _sqlClient = Context.CloneClient();
                        int _total = 0;
                        int _psize = 1000;
                        foreach (HiColumn hiColumn in arrcol_tab)
                        {
                            int _scount = 0;
                            if (dic_hash_reg.ContainsKey(hiColumn.FieldName))
                            {
                                _scount = dic_hash_reg[hiColumn.FieldName].Count;
                                //当源表的条件值大于需要匹配的值时 建议将匹配的值用in的方式传入进行匹配

                                HashSet<string> _hash = new HashSet<string>();
                                DataTable data = null;

                                if (!string.IsNullOrEmpty(hiColumn.RefWhere.Trim()))
                                {
                                    data = _sqlClient.Query(hiColumn.RefTab).Field(hiColumn.RefField).Where(hiColumn.RefWhere)
                                    .Skip(1).Take(_psize)
                                    .ToTable(ref _total);
                                }
                                else
                                {
                                    data = _sqlClient.Query(hiColumn.RefTab).Field(hiColumn.RefField)
                                      .Skip(1).Take(_psize)
                                      .ToTable(ref _total);

                                }

                                if (data != null && data.Rows.Count > 0)
                                {

                                    if (data.Rows.Count == _psize)
                                    {
                                        //源表值比较大
                                        string _sql = dic_hash_reg[hiColumn.FieldName].ToArray().ToSqlIn(true);
                                        if (!string.IsNullOrEmpty(hiColumn.RefWhere.Trim()))
                                        {
                                            //注意这里的语句是非原生sql 是hisql 可以在不同的数据库中编译执行
                                            _sql = $"({hiColumn.RefWhere.Trim()}) and ({_sql})";
                                        }
                                        data = _sqlClient.Query(hiColumn.RefTab).Field(hiColumn.RefField).Where(_sql).ToTable();
                                        _hash = DataConvert.DataTableFieldToHashSet(data, hiColumn.RefField);
                                    }
                                    else
                                    {
                                        //string _sql=DataTableFieldToList(data, hiColumn.RefField).ToArray().ToSqlIn(true);
                                        _hash = DataConvert.DataTableFieldToHashSet(data, hiColumn.RefField);
                                    }
                                }
                                dic_hash_reg[hiColumn.FieldName].ExceptWith(_hash);
                                if (dic_hash_reg[hiColumn.FieldName].Count > 0)
                                {
                                    throw new Exception($"字段[{hiColumn.FieldName}]配置了表检测 值 [{dic_hash_reg[hiColumn.FieldName].ToArray()[0]}] 在表[{hiColumn.RefTab}]不存在");
                                }

                            }

                        }
                    }


                    #endregion

                }
                else
                    throw new Exception($"无可更新数据");
                rtn = new Tuple<List<Dictionary<string, string>>, List<Dictionary<string, string>>>(lst_value, lst_primary);
                return rtn;
            }
            else
                throw new Exception($"找不到相关表信息");

        }

        public Tuple<Dictionary<string, string>, Dictionary<string, string>> CheckData(bool isDic, TableDefinition table, object objdata, IDMInitalize dMInitalize, TabInfo tabinfo, List<string> fields, bool isonly)
        {
            if (table != null)
            {
                Tuple<Dictionary<string, string>, Dictionary<string, string>> result;
                Type type = objdata.GetType();
                IDMTab mytab = (IDMTab)dMInitalize;
                List<PropertyInfo> _attrs = type.GetProperties().Where(p => p.MemberType == MemberTypes.Property && p.CanRead == true).ToList();
                result = CheckUpdateData(isDic, this.Wheres.Count > 0 ? false : true, _attrs, tabinfo.GetColumns, objdata, fields, isonly);
                return result;
            }
            else throw new Exception($"找不到相关表信息");
        }
        Tuple<bool, string> checkFieldValue(HiColumn hiColumn, int rowidx, string value)
        {

            string _value = "";
            Tuple<bool, string> rtn = new Tuple<bool, string>(false, "");


            #region 字典数据

            _value = value;


            if (Constants.IsStandardTimeField(hiColumn.FieldName))
            {
                if (hiColumn.DBDefault != HiTypeDBDefault.FUNDATE)
                {
                    _value = $"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}'";
                    rtn = new Tuple<bool, string>(true, _value);

                }
            }
            else if (Constants.IsStandardUserField(hiColumn.FieldName))
            {
                _value = $"'{Context.CurrentConnectionConfig.User}'";
                rtn = new Tuple<bool, string>(true, _value);
            }
            else
            {
                if (hiColumn.FieldType.IsIn<HiType>(HiType.NVARCHAR, HiType.NCHAR, HiType.GUID))
                {
                    //中文按1个字符计算
                    //_value = "test";

                    if (_value.Length > hiColumn.FieldLen && hiColumn.FieldLen > 0)
                    {
                        throw new Exception($"字段[{hiColumn.FieldName}]的值[{_value}]超过了限制长度[{hiColumn.FieldLen}] 无法数据提交");
                    }
                    if (hiColumn.IsRequire)
                    {
                        if (string.IsNullOrEmpty(_value.Trim()) && !hiColumn.IsPrimary)
                            throw new Exception($"字段[{hiColumn.FieldName}] 为必填 无法数据提交");
                    }

                    _value = $"'{_value.ToSqlInject()}'";
                    rtn = new Tuple<bool, string>(true, _value);

                }
                else if (hiColumn.FieldType.IsIn<HiType>(HiType.VARCHAR, HiType.CHAR, HiType.TEXT))
                {
                    //中文按两个字符计算

                    //_value = "test";
                    if (_value.LengthZH() > hiColumn.FieldLen && hiColumn.FieldLen > 0)
                    {
                        throw new Exception($"字段[{hiColumn.FieldName}]的值[{_value}]超过了限制长度[{hiColumn.FieldLen}] 无法数据提交");
                    }
                    if (hiColumn.IsRequire)
                    {
                        if (string.IsNullOrEmpty(_value.Trim()) && !hiColumn.IsPrimary)
                            throw new Exception($"字段[{hiColumn.FieldName}] 为必填 无法数据提交");
                    }
                    _value = $"'{_value.ToSqlInject()}'";
                    rtn = new Tuple<bool, string>(true, _value);
                }
                else if (hiColumn.FieldType.IsIn<HiType>(HiType.INT, HiType.BIGINT, HiType.DECIMAL, HiType.SMALLINT))
                {

                    //_value = "1";
                    rtn = new Tuple<bool, string>(true, $"{_value}");
                }
                else if (hiColumn.FieldType.IsIn<HiType>(HiType.BOOL))
                {

                    if (_value == "true" || _value == "1")
                    {
                        if (Context.CurrentConnectionConfig.DbType.IsIn<DBType>(DBType.PostGreSql, DBType.Hana, DBType.MySql))
                        {
                            _value = "True";
                            rtn = new Tuple<bool, string>(true, _value);
                        }
                        else
                        {
                            _value = "1";
                            rtn = new Tuple<bool, string>(true, $"'{_value}'");

                        }
                    }
                    else
                    {
                        if (Context.CurrentConnectionConfig.DbType.IsIn<DBType>(DBType.PostGreSql, DBType.Hana, DBType.MySql))
                        {
                            _value = "False";
                            rtn = new Tuple<bool, string>(true, _value);
                        }
                        else
                        {
                            _value = "0";
                            rtn = new Tuple<bool, string>(true, $"'{_value}'");
                        }
                    }
                }
                else if (hiColumn.FieldType.IsIn<HiType>(HiType.DATE, HiType.DATETIME))
                {
                    if (!string.IsNullOrEmpty(_value))
                    {
                        DateTime dtime = DateTime.Parse(_value);
                        //DateTime dtime = DateTime.Now;
                        if (dtime != null)
                        {
                            rtn = new Tuple<bool, string>(true, $"'{dtime.ToString("yyyy-MM-dd HH:mm:ss.fff")}'");
                        }
                    }
                }
                else
                {
                    rtn = new Tuple<bool, string>(true, $"'{_value}'");
                }
            }


            #endregion

            return rtn;
        }
        private Tuple<Dictionary<string, string>, Dictionary<string, string>> CheckUpdateData(bool isDic, bool isRequireKey, List<PropertyInfo> attrs, List<HiColumn> hiColumns, object objdata, List<string> fields, bool isonly)
        {

            Dictionary<string, string> _values = new Dictionary<string, string>();
            Dictionary<string, string> _primary = new Dictionary<string, string>();
            string _value;
            Dictionary<string, string> _dic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (isDic)
            {
                if (typeof(Dictionary<string, object>) == objdata.GetType())
                {
                    Dictionary<string, object> _dic2 = (Dictionary<string, object>)objdata;
                    foreach (string n in _dic2.Keys)
                    {
                        if (!_dic.ContainsKey(n))
                        {
                            if (_dic2[n].GetType() != typeof(DateTime))
                                _dic.Add(n, _dic2[n].ToString());
                            else
                            {
                                DateTime _dtime = (DateTime)_dic2[n];
                                _dic.Add(n, _dtime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                            }
                        }
                    }
                }
                else
                    _dic = (Dictionary<string, string>)objdata;
            }

            foreach (HiColumn hiColumn in hiColumns)
            {

                if (!isDic)
                {
                    //对象型数据
                    var objprop = attrs.Where(p => p.Name.ToLower() == hiColumn.FieldName.ToLower()).FirstOrDefault();
                    //if (hiColumn.IsRequire && !hiColumn.IsIdentity && objprop == null)
                    //{
                    //    throw new Exception($"字段[{hiColumn.FieldName}] 为必填 无法数据提交");
                    //}
                    if (hiColumn.IsBllKey && objprop == null && isRequireKey)
                    {
                        throw new Exception($"字段[{hiColumn.FieldName}] 为业务主键或表主键 更新表数据时必填");
                    }
                    if (objprop != null && objprop.GetValue(objdata) != null)
                    {
                        if (hiColumn.FieldType.IsIn<HiType>(HiType.NVARCHAR, HiType.NCHAR, HiType.GUID))
                        {
                            //中文按1个字符计算
                            //_value = "test";
                            _value = (string)objprop.GetValue(objdata);

                            if (_value == null)
                            {
                                if (hiColumn.IsRequire)
                                    throw new Exception($"字段[{objprop.Name}] 为必填 无法数据提交");
                                else
                                    continue;
                            }

                            if (_value.Length > hiColumn.FieldLen && hiColumn.FieldLen > 0)
                            {
                                throw new Exception($"字段[{objprop.Name}]的值[{_value}]超过了限制长度[{hiColumn.FieldLen}] 无法数据提交");
                            }
                            if (hiColumn.IsRequire)
                            {
                                if (string.IsNullOrEmpty(_value.Trim()))
                                    throw new Exception($"字段[{objprop.Name}] 为必填 无法数据提交");
                            }

                            _values.Add(hiColumn.FieldName, $"'{_value.ToSqlInject()}'");
                            if (hiColumn.IsBllKey)
                                _primary.Add(hiColumn.FieldName, $"'{_value.ToSqlInject()}'");

                        }
                        else if (hiColumn.FieldType.IsIn<HiType>(HiType.VARCHAR, HiType.CHAR, HiType.TEXT))
                        {
                            //中文按两个字符计算
                            _value = (string)objprop.GetValue(objdata);
                            if (_value == null)
                            {
                                if (hiColumn.IsRequire)
                                    throw new Exception($"字段[{objprop.Name}] 为必填 无法数据提交");
                                else
                                    continue;
                            }

                            //_value = "test";
                            if (_value.LengthZH() > hiColumn.FieldLen && hiColumn.FieldLen > 0)
                            {
                                throw new Exception($"字段[{objprop.Name}]的值[{_value}]超过了限制长度[{hiColumn.FieldLen}] 无法数据提交");
                            }
                            if (hiColumn.IsRequire)
                            {
                                if (string.IsNullOrEmpty(_value.Trim()))
                                    throw new Exception($"字段[{objprop.Name}] 为必填 无法数据提交");
                            }
                            _values.Add(hiColumn.FieldName, $"'{_value.ToSqlInject()}'");
                            if (hiColumn.IsBllKey)
                                _primary.Add(hiColumn.FieldName, $"'{_value.ToSqlInject()}'");
                        }
                        else if (hiColumn.FieldType.IsIn<HiType>(HiType.INT, HiType.BIGINT, HiType.DECIMAL, HiType.SMALLINT))
                        {
                            _value = objprop.GetValue(objdata).ToString();
                            if (_value == null)
                            {
                                if (hiColumn.IsRequire)
                                    throw new Exception($"字段[{objprop.Name}] 为必填 无法数据提交");
                                else
                                    continue;
                            }
                            //_value = "1";
                            _values.Add(hiColumn.FieldName, $"{_value}");
                            if (hiColumn.IsBllKey)
                                _primary.Add(hiColumn.FieldName, $"{_value}");
                        }
                        else if (hiColumn.FieldType.IsIn<HiType>(HiType.DATE, HiType.DATETIME))
                        {
                            DateTime dtime = (DateTime)objprop.GetValue(objdata);

                            //DateTime dtime = DateTime.Now;
                            if (dtime != null && dtime != DateTime.MinValue)
                            {
                                _values.Add(hiColumn.FieldName, $"'{dtime.ToString("yyyy-MM-dd HH:mm:ss.fff")}'");
                                if (hiColumn.IsBllKey)
                                    _primary.Add(hiColumn.FieldName, $"'{dtime.ToString("yyyy-MM-dd HH:mm:ss.fff")}'");
                            }
                        }
                        else if (hiColumn.FieldType.IsIn<HiType>(HiType.BOOL)) //add by tgm date:2021.10.27
                        {
                            if ((bool)objprop.GetValue(objdata) == true)
                            {
                                if (Context.CurrentConnectionConfig.DbType.IsIn<DBType>(DBType.PostGreSql, DBType.Hana))
                                {
                                    _value = "True";
                                    _values.Add(hiColumn.FieldName, $"{_value}");
                                }
                                else
                                {
                                    _value = "1";
                                    _values.Add(hiColumn.FieldName, $"'{_value}'");
                                }
                            }
                            else
                            {
                                if (Context.CurrentConnectionConfig.DbType.IsIn<DBType>(DBType.PostGreSql, DBType.Hana))
                                {
                                    _value = "False";
                                    _values.Add(hiColumn.FieldName, $"{_value}");
                                }
                                else
                                {
                                    _value = "0";
                                    _values.Add(hiColumn.FieldName, $"'{_value}'");
                                }
                            }


                        }
                        else
                        {
                            _value = (string)objprop.GetValue(objdata);
                            if (_value == null)
                            {
                                if (hiColumn.IsRequire)
                                    throw new Exception($"字段[{objprop.Name}] 为必填 无法数据提交");
                                else
                                    continue;
                            }
                            _values.Add(hiColumn.FieldName, $"'{_value}'");
                            if (hiColumn.IsBllKey)
                                _primary.Add(hiColumn.FieldName, $"'{_value}'");
                        }
                    }
                }
                else
                {

                    if (hiColumn.IsBllKey && _dic == null && isRequireKey)
                    {
                        throw new Exception($"字段[{hiColumn.FieldName}] 为业务主键或表主键 更新表数据时必填");
                    }

                    if (_dic.ContainsKey(hiColumn.FieldName))
                    {
                        if (hiColumn.FieldType.IsIn<HiType>(HiType.NVARCHAR, HiType.NCHAR, HiType.GUID))
                        {
                            //中文按1个字符计算
                            //_value = "test";
                            _value = _dic[hiColumn.FieldName].ToString();

                            if (_value == null)
                            {
                                if (hiColumn.IsRequire)
                                    throw new Exception($"字段[{hiColumn.FieldName}] 为必填 无法数据提交");
                                else
                                    continue;
                            }

                            if (_value.Length > hiColumn.FieldLen && hiColumn.FieldLen > 0)
                            {
                                throw new Exception($"字段[{hiColumn.FieldName}]的值[{_value}]超过了限制长度[{hiColumn.FieldLen}] 无法数据提交");
                            }
                            if (hiColumn.IsRequire)
                            {
                                if (string.IsNullOrEmpty(_value.Trim()))
                                    throw new Exception($"字段[{hiColumn.FieldName}] 为必填 无法数据提交");
                            }

                            _values.Add(hiColumn.FieldName, $"'{_value.ToSqlInject()}'");
                            if (hiColumn.IsBllKey)
                                _primary.Add(hiColumn.FieldName, $"'{_value.ToSqlInject()}'");
                        }
                        else if (hiColumn.FieldType.IsIn<HiType>(HiType.VARCHAR, HiType.CHAR, HiType.TEXT))
                        {
                            //中文按两个字符计算
                            _value = _dic[hiColumn.FieldName].ToString();
                            if (_value == null)
                            {
                                if (hiColumn.IsRequire)
                                    throw new Exception($"字段[{hiColumn.FieldName}] 为必填 无法数据提交");
                                else
                                    continue;
                            }

                            //_value = "test";
                            if (_value.LengthZH() > hiColumn.FieldLen && hiColumn.FieldLen > 0)
                            {
                                throw new Exception($"字段[{hiColumn.FieldName}]的值[{_value}]超过了限制长度[{hiColumn.FieldLen}] 无法数据提交");
                            }
                            if (hiColumn.IsRequire)
                            {
                                if (string.IsNullOrEmpty(_value.Trim()))
                                    throw new Exception($"字段[{hiColumn.FieldName}] 为必填 无法数据提交");
                            }
                            _values.Add(hiColumn.FieldName, $"'{_value.ToSqlInject()}'");
                            if (hiColumn.IsBllKey)
                                _primary.Add(hiColumn.FieldName, $"'{_value.ToSqlInject()}'");
                        }
                        else if (hiColumn.FieldType.IsIn<HiType>(HiType.INT, HiType.BIGINT, HiType.DECIMAL, HiType.SMALLINT))
                        {
                            _value = _dic[hiColumn.FieldName].ToString();
                            if (_value == null)
                            {
                                if (hiColumn.IsRequire)
                                    throw new Exception($"字段[{hiColumn.FieldName}] 为必填 无法数据提交");
                                else
                                    continue;
                            }
                            //_value = "1";
                            _values.Add(hiColumn.FieldName, $"{_value}");
                            if (hiColumn.IsBllKey)
                                _primary.Add(hiColumn.FieldName, $"{_value}");
                        }
                        else if (hiColumn.FieldType.IsIn<HiType>(HiType.DATE, HiType.DATETIME))
                        {
                            DateTime dtime = DateTime.Parse(_dic[hiColumn.FieldName].ToString());
                            //DateTime dtime = DateTime.Now;
                            if (dtime != null)
                            {
                                _values.Add(hiColumn.FieldName, $"'{dtime.ToString("yyyy-MM-dd HH:mm:ss.fff")}'");
                                if (hiColumn.IsBllKey)
                                    _primary.Add(hiColumn.FieldName, $"'{dtime.ToString("yyyy-MM-dd HH:mm:ss.fff")}'");
                            }
                        }
                        else
                        {
                            _value = _dic[hiColumn.FieldName].ToString().ToSqlInject();
                            if (_value == null)
                            {
                                if (hiColumn.IsRequire)
                                    throw new Exception($"字段[{hiColumn.FieldName}] 为必填 无法数据提交");
                                else
                                    continue;
                            }
                            _values.Add(hiColumn.FieldName, $"'{_value}'");
                            if (hiColumn.IsBllKey)
                                _primary.Add(hiColumn.FieldName, $"'{_value}'");
                        }
                    }

                }

                if (fields.Count > 0)
                {
                    //说明有指定更新，或排除更新字段
                    if (isonly)
                    {
                        //仅在fields列表中的 才会更新字段
                        if (fields.Where(f => f.ToLower() == hiColumn.FieldName.ToLower()).Count() == 0 || hiColumn.IsBllKey)
                        {
                            if (_values.ContainsKey(hiColumn.FieldName))
                                _values.Remove(hiColumn.FieldName);
                        }
                    }
                    else
                    {
                        if (fields.Where(f => f.ToLower() == hiColumn.FieldName.ToLower()).Count() > 0 || hiColumn.IsBllKey)
                        {
                            if (_values.ContainsKey(hiColumn.FieldName))
                                _values.Remove(hiColumn.FieldName);
                        }
                    }
                }


                if (hiColumn.FieldName.ToLower() == "CreateTime".ToLower() || hiColumn.FieldName.ToLower() == "CreateName".ToLower())
                {

                    //if (hiColumn.DBDefault != HiTypeDBDefault.FUNDATE)
                    //    _values.Add(hiColumn.FieldName, $"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}'");
                    if (_values.ContainsKey(hiColumn.FieldName))
                        _values.Remove(hiColumn.FieldName);
                }
                else if (hiColumn.FieldName.ToLower() == "ModiName".ToLower())
                {
                    if (!_values.ContainsKey(hiColumn.FieldName))
                        _values.Add(hiColumn.FieldName, $"'{Context.CurrentConnectionConfig.User}'");
                    else
                        _values[hiColumn.FieldName] = $"'{Context.CurrentConnectionConfig.User}'";


                }
                else if (hiColumn.FieldName.ToLower() == "ModiTime".ToLower())
                {
                    if (!_values.ContainsKey(hiColumn.FieldName))
                        _values.Add(hiColumn.FieldName, $"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}'");
                    else
                        _values[hiColumn.FieldName] = $"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}'";
                }
            }

            if (_values.Count == 0)
                throw new Exception($"可更新的字段为空,请检查!");



            return new Tuple<Dictionary<string, string>, Dictionary<string, string>>(_values, _primary);
        }

        //public virtual Dictionary<string, string> CheckData(TableDefinition table, object objdata)
        //{
        //    return null;
        //}
    }
}
