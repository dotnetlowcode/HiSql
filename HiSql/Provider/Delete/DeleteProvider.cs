using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public class DeleteProvider : IDelete
    {
        TableDefinition _table;
        SynTaxQueue _queue = new SynTaxQueue();
        List<FilterDefinition> _list_filter = new List<FilterDefinition>();
        List<object> _list_data = new List<object>();
        bool _isnodblog = false;

        //无日志清除表数据
        bool _istruncate = false;
        //删除表
        bool _isdrop = false;

        Filter _where;

        IDbConfig dbconfig = null;
        HiSqlProvider _context = null;
        public HiSqlProvider Context { get=>_context; 
            set { _context = value;
                if (dbconfig == null)
                {
                    dbconfig = (IDbConfig)Instance.CreateInstance<IDbConfig>($"{Constants.NameSpace}.{_context.CurrentConnectionConfig.DbType.ToString()}{DbInterFace.Config.ToString()}");
                    dbconfig.Init();
                }
            } 
        }
        public TableDefinition Table
        {
            get { return _table; }
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

        /// <summary>
        /// 是否无日志清除表数据
        /// </summary>
        public bool IsTruncate
        {
            get { return _istruncate; }
        }
        /// <summary>
        /// 是否删除表
        /// </summary>
        public bool IsDrop
        {
            get { return _isdrop; }
        }

        /// <summary>
        /// 是否完全没有数据库日志的删除
        /// </summary>
        public bool IsNoDbLog { get => _isnodblog; }

        
        public DeleteProvider()
        {
            
        }

        public virtual string ToSql()
        {
            throw new Exception($"请在当前库中实现重写ToSql()接口");
        }
        public IDelete Delete(string tabname, object objdata)
        {
            if (_queue.HasQueue("truncate"))
            {
                throw new Exception($"已经指定了TrunCate 不允许再指定Delete");
            }
            if (!_queue.HasQueue("table"))
            {
                _table = new TableDefinition(tabname);
                _queue.Add("table|data");
                _list_data.Add(objdata);
            }
            else
            {
                throw new Exception($"已经指定了一个Delete 不允许重复指定");
            }

            return this;
        }

        public IDelete Delete(string tabname, List<object> objlst)
        {
            if (_queue.HasQueue("truncate"))
            {
                throw new Exception($"已经指定了TrunCate 不允许再指定Delete");
            }


            if (!_queue.HasQueue("table"))
            {
                _table = new TableDefinition(tabname);
                _list_data = objlst;
                _queue.Add("table|list");
            }
            else
                throw new Exception($"已经指定了一个Update 不允许重复指定");
            return this;

        }

        public IDelete Delete<T>(T objdata)
        {
            if (_queue.HasQueue("truncate"))
            {
                throw new Exception($"已经指定了TrunCate 不允许再指定Delete");
            }

            if (!_queue.HasQueue("table"))
            {
                _table = new TableDefinition(typeof(T).Name);
                _list_data.Add((object)objdata);
                _queue.Add("table");
            }
            else
                throw new Exception($"已经指定了一个Delete不允许重复指定");
            return this;
        }

        public IDelete Delete<T>(string tabname, List<T> objlst)
        {
            if (_queue.HasQueue("truncate"))
            {
                throw new Exception($"已经指定了TrunCate 不允许再指定Delete");
            }
            if (!_queue.HasQueue("table"))
            {
                _table = new TableDefinition(tabname);
                foreach (object obj in objlst)
                {
                    _list_data.Add((object)obj);
                }

                _queue.Add("table");
            }
            else
                throw new Exception($"已经指定了一个Delete不允许重复指定");
            return this;
        }
        public IDelete Delete<T>(List<T> objlst)
        {
            if (_queue.HasQueue("truncate"))
            {
                throw new Exception($"已经指定了TrunCate 不允许再指定Delete");
            }

            if (!_queue.HasQueue("table"))
            {
                _table = new TableDefinition(typeof(T).Name);
                foreach (object obj in objlst)
                {
                    _list_data.Add((object)obj);
                }

                _queue.Add("table");
            }
            else
                throw new Exception($"已经指定了一个Delete不允许重复指定");
            return this;
        }

        public IDelete Delete(string tabname)
        {
            if (_queue.HasQueue("truncate"))
            {
                throw new Exception($"已经指定了TrunCate 不允许再指定Delete");
            }
            if (!_queue.HasQueue("table"))
            {
                _table = new TableDefinition(tabname);
                _queue.Add("table");

            }
            else
            {
                throw new Exception($"已经指定了一个Delete 不允许重复指定");

            }
            return this;
        }

        /// <summary>
        /// 警告!整表数据删除 不产生任何数据库日志
        /// 请谨慎操作些方法
        /// </summary>
        /// <param name="tabname"></param>
        /// <returns></returns>
        public IDelete TrunCate(string tabname)
        {
            if (!_queue.HasQueue("table") && !_queue.HasQueue("drop"))
            {
                _table = new TableDefinition(tabname);
                if (_table.TableType != TableType.Entity)
                    throw new Exception($"TrunCate 只能对实体表进行操作");
                _isnodblog = true;
                _queue.Add("truncate");
                _istruncate = true;
            }
            else
                throw new Exception($"已经指定了Delete或Drop 不允许再指定TrunCate");
            return this;
        }
        public IDelete Drop(string tabname)
        {
            if (!_queue.HasQueue("table") && !_queue.HasQueue("truncate"))
            {
                _table = new TableDefinition(tabname);
                _isnodblog = true;
                _queue.Add("drop");
                _isdrop = true;

            }else
                throw new Exception($"已经指定了Delete或TrunCate 不允许再指定Drop");
            return this;
        }

        public IDelete Where(string sqlwhere)
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
        public IDelete Where(Filter where)
        {
            if (!_queue.HasQueue("table"))
                throw new Exception($"请先指定要删除的表");
            if (_queue.HasQueue("truncate"))
                throw new Exception($"Where 不对针对于TrunCate进行操作");

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






        public Dictionary<string, string> CheckData(bool isDic, TableDefinition table, object objdata, IDMInitalize dMInitalize, TabInfo tabinfo)
        {
            if (table != null)
            {
                Dictionary<string, string> result;
                Type type = objdata.GetType();
                IDMTab mytab = (IDMTab)dMInitalize;
                List<PropertyInfo> _attrs = type.GetProperties().Where(p => p.MemberType == MemberTypes.Property && p.CanRead == true).ToList();
                result = CheckDeleteData(isDic, this.Wheres.Count > 0 ? false : true, _attrs, tabinfo.GetColumns, objdata);
                return result;
            }
            else throw new Exception($"找不到相关表信息");
        }

        private Dictionary<string, string> CheckDeleteData(bool isDic, bool isRequireKey, List<PropertyInfo> attrs, List<HiColumn> hiColumns, object objdata)
        {

            Dictionary<string, string> _values = new Dictionary<string, string>();
            string _value = string.Empty;
            Dictionary<string, string> _dic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (isDic)
                _dic = (Dictionary<string, string>)objdata;

            if (!hiColumns.Any(h => h.IsBllKey))
                throw new Exception($"该表无主键无法用指定数据删除请在Delete方法后用Where的方式进行操作");


            //仅对字段进行条件删除
            var bllcols = hiColumns.Where(c => c.IsBllKey).ToList();

            foreach (HiColumn hiColumn in bllcols)
            {
                if (!isDic)
                {
                    var objprop = attrs.Where(p => p.Name.ToLower() == hiColumn.FieldName.ToLower()).FirstOrDefault();
                    if (objprop == null)
                    {

                        throw new Exception($"字段[{hiColumn.FieldName}] 为主键或业务主键,传入的删除对象中未发现此字段");

                        

                    }
                    else
                    {
                        object _vobj = objprop.GetValue(objdata);
                        _value = _vobj.ToString();


                    }
                    if (hiColumn.IsBllKey && objprop == null)//&& isRequireKey
                    {
                        //默认如果有主键或业务字段则删除是必填 不则会报错
                        throw new Exception($"字段[{hiColumn.FieldName}] 为业务主键或表主键 删除表数据时必填");
                    }
                    else //if (hiColumn.IsBllKey && isRequireKey && objprop != null)
                    {
                        if (hiColumn.FieldType.IsIn<HiType>(HiType.NVARCHAR, HiType.NCHAR, HiType.GUID))
                        {
                            if (_value.Length > hiColumn.FieldLen && hiColumn.FieldLen > 0)
                            {
                                throw new Exception($"字段[{objprop.Name}]的值[{_value}]超过了限制长度[{hiColumn.FieldLen}] 无法数据提交");
                            }
                            if(hiColumn.IsPrimary && string.IsNullOrEmpty(_value))
                                _values.Add(hiColumn.FieldName, $"'{dbconfig.Key_Char_Default}'");
                            else
                                _values.Add(hiColumn.FieldName, $"'{_value.ToSqlInject()}'");
                        }
                        else if (hiColumn.FieldType.IsIn<HiType>(HiType.VARCHAR, HiType.CHAR, HiType.TEXT))
                        {
                            if (_value.LengthZH() > hiColumn.FieldLen && hiColumn.FieldLen > 0)
                            {
                                throw new Exception($"字段[{objprop.Name}]的值[{_value}]超过了限制长度[{hiColumn.FieldLen}] 无法数据提交");
                            }
                            if (hiColumn.IsPrimary && string.IsNullOrEmpty(_value))
                                _values.Add(hiColumn.FieldName, $"'{dbconfig.Key_Char_Default}'");
                            else
                                _values.Add(hiColumn.FieldName, $"'{_value.ToSqlInject()}'");

                        }
                        else if (hiColumn.FieldType.IsIn<HiType>(HiType.INT, HiType.BIGINT, HiType.DECIMAL, HiType.SMALLINT))
                        {
                            if (!Tool.IsDecimal(_value))
                            {
                                throw new Exception($"字段[{hiColumn.FieldName}]的值[{_value}]与类型[{hiColumn.FieldType.ToString()}]不符可能有SQL注入风险");
                            }

                            _values.Add(hiColumn.FieldName, $"{_value.ToSqlInject()}");
                        }
                        else if (hiColumn.FieldType.IsIn<HiType>(HiType.DATE, HiType.DATETIME))
                        {
                            _value = objprop.GetValue(objdata).ToString();
                            DateTime dtime = Convert.ToDateTime(_value);
                            _values.Add(hiColumn.FieldName, $"'{dtime.ToString("yyyy-MM-dd HH:mm:ss.fff")}'");
                        }
                        else if (hiColumn.FieldType == HiType.BOOL)
                        {
                            
                            if (this.Context.CurrentConnectionConfig.DbType.IsIn(DBType.SqlServer, DBType.Oracle, DBType.DaMeng))
                            {
                                if (_value.ToLower().Equals("true") || _value.ToLower().Equals("1"))
                                {
                                    _value = "1";
                                }
                                else if (_value.ToLower().Equals("false") || _value.ToLower().Equals("0"))
                                {
                                    _value = "0";
                                }
                                else
                                    throw new Exception($"字段[{hiColumn.FieldName}]的值[{_value}]与类型[{hiColumn.FieldType.ToString()}]不符");
                            }
                            else if (this.Context.CurrentConnectionConfig.DbType.IsIn(DBType.Hana))
                            {
                                if (_value.ToLower().Equals("true") || _value.ToLower().Equals("1"))
                                {
                                    _value = "true";
                                }
                                else if (_value.ToLower().Equals("false") || _value.ToLower().Equals("0"))
                                {
                                    _value = "true";
                                }
                                else
                                    throw new Exception($"字段[{hiColumn.FieldName}]的值[{_value}]与类型[{hiColumn.FieldType.ToString()}]不符");
                            }
                            else
                            {
                                if (_value.ToLower().Equals("true") || _value.ToLower().Equals("1"))
                                {
                                    _value = "True";
                                }
                                else if (_value.ToLower().Equals("false") || _value.ToLower().Equals("0"))
                                {
                                    _value = "False";
                                }
                                else
                                    throw new Exception($"字段[{hiColumn.FieldName}]的值[{_value}]与类型[{hiColumn.FieldType.ToString()}]不符");

                            }
                            _values.Add(hiColumn.FieldName, $"'{_value}'");
                        }
                        else
                        {
                            _values.Add(hiColumn.FieldName, $"'{_value}'");
                        }
                    }

                }
                else
                {

                    if (hiColumn.IsBllKey && !_dic.ContainsKey(hiColumn.FieldName))// && isRequireKey
                    {
                        //默认如果有主键或业务字段则删除是必填 不则会报错
                        throw new Exception($"字段[{hiColumn.FieldName}] 为业务主键或表主键 删除表数据时必填");
                    }
                    if (_dic.ContainsKey(hiColumn.FieldName))
                    {
                        _value = _dic[hiColumn.FieldName].ToString();
                        if (hiColumn.IsRequire && string.IsNullOrEmpty(_value))
                        {
                            throw new Exception($"字段[{hiColumn.FieldName}] 为必填 无法数据提交");
                        }
                        if (hiColumn.IsBllKey && isRequireKey)
                        {
                            if (hiColumn.FieldType.IsIn<HiType>(HiType.NVARCHAR, HiType.NCHAR, HiType.GUID))
                            {
                                if (_value.Length > hiColumn.FieldLen && hiColumn.FieldLen>0)
                                {
                                    throw new Exception($"字段[{hiColumn.FieldName}]的值[{_value}]超过了限制长度[{hiColumn.FieldLen}] 无法数据提交");
                                }
                                if (hiColumn.IsPrimary && string.IsNullOrEmpty(_value))
                                    _values.Add(hiColumn.FieldName, $"'{dbconfig.Key_Char_Default}'");
                                else
                                    _values.Add(hiColumn.FieldName, $"'{_value.ToSqlInject()}'");
                            }
                            else if (hiColumn.FieldType.IsIn<HiType>(HiType.VARCHAR, HiType.CHAR, HiType.TEXT))
                            {

                                if (hiColumn.IsPrimary && string.IsNullOrEmpty(_value))
                                    _values.Add(hiColumn.FieldName, $"'{dbconfig.Key_Char_Default}'");
                                else
                                    _values.Add(hiColumn.FieldName, $"'{_value.ToSqlInject()}'");
                            }
                            else if (hiColumn.FieldType.IsIn<HiType>(HiType.INT, HiType.BIGINT, HiType.DECIMAL, HiType.SMALLINT))
                            {
                                if (!Tool.IsDecimal(_value))
                                {
                                    throw new Exception($"字段[{hiColumn.FieldName}]的值[{_value}]与类型[{hiColumn.FieldType.ToString()}]不符");
                                }



                                _values.Add(hiColumn.FieldName, $"{_value.ToSqlInject()}");
                            }
                            else if (hiColumn.FieldType.IsIn<HiType>(HiType.DATE, HiType.DATETIME))
                            {
                                //_value = objprop.GetValue(objdata).ToString();
                                DateTime dtime = DateTime.Parse(_dic[hiColumn.FieldName].ToString());
                                //DateTime dtime = Convert.ToDateTime(_value);
                                _values.Add(hiColumn.FieldName, $"'{dtime.ToString("yyyy-MM-dd HH:mm:ss.fff")}'");
                            }
                            else if (hiColumn.FieldType == HiType.BOOL)
                            {

                                if (this.Context.CurrentConnectionConfig.DbType.IsIn(DBType.SqlServer, DBType.Oracle, DBType.DaMeng))
                                {
                                    if (_value.ToLower().Equals("true") || _value.ToLower().Equals("1"))
                                    {
                                        _value = "1";
                                    }
                                    else if (_value.ToLower().Equals("false") || _value.ToLower().Equals("0"))
                                    {
                                        _value = "0";
                                    }
                                    else
                                        throw new Exception($"字段[{hiColumn.FieldName}]的值[{_value}]与类型[{hiColumn.FieldType.ToString()}]不符");
                                }
                                else if (this.Context.CurrentConnectionConfig.DbType.IsIn(DBType.Hana))
                                {
                                    if (_value.ToLower().Equals("true") || _value.ToLower().Equals("1"))
                                    {
                                        _value = "true";
                                    }
                                    else if (_value.ToLower().Equals("false") || _value.ToLower().Equals("0"))
                                    {
                                        _value = "true";
                                    }
                                    else
                                        throw new Exception($"字段[{hiColumn.FieldName}]的值[{_value}]与类型[{hiColumn.FieldType.ToString()}]不符");
                                }
                                else
                                {
                                    if (_value.ToLower().Equals("true") || _value.ToLower().Equals("1"))
                                    {
                                        _value = "True";
                                    }
                                    else if (_value.ToLower().Equals("false") || _value.ToLower().Equals("0"))
                                    {
                                        _value = "False";
                                    }
                                    else
                                        throw new Exception($"字段[{hiColumn.FieldName}]的值[{_value}]与类型[{hiColumn.FieldType.ToString()}]不符");

                                }
                                _values.Add(hiColumn.FieldName, $"'{_value}'");
                            }
                            else
                            {
                                _values.Add(hiColumn.FieldName, $"'{_value.ToSqlInject()}'");
                            }
                        }
                    }
                    else
                    {

                        if (hiColumn.IsBllKey && !_dic.ContainsKey(hiColumn.FieldName))
                        {
                            throw new Exception($"字段[{hiColumn.FieldName}] 为业务主键或表主键 删除表数据时必填");
                        }
                        //if (hiColumn.IsRequire)
                        //    throw new Exception($"字段[{hiColumn.FieldName}] 为必填 无法数据提交");
                        //else
                        //    continue;
                    }


                }
                //else if (!isRequireKey && objprop != null)
                //{ 

                //}
            }
            return _values;
        }

        int IDelete.ExecCommand()
        {
            var sql = this.ToSql();
            return this.Context.DBO.ExecCommand(sql);
        }

        public Task<int> ExecCommandAsync()
        {
            var sql = this.ToSql();
            return this.Context.DBO.ExecCommandAsync(sql);
        }
    }
}
