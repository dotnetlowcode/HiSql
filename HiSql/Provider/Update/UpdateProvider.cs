using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public class UpdateProvider : IUpdate
    {

        TableDefinition _table;
        List<FieldDefinition> _list_only_field = new List<FieldDefinition>();
        List<FieldDefinition> _list_exclude_field = new List<FieldDefinition>();
        List<FilterDefinition> _list_filter = new List<FilterDefinition>();

        List<Dictionary<string, string>> _values = new List<Dictionary<string, string>>();

        List<object> _list_data = new List<object>();

        SynTaxQueue _queue = new SynTaxQueue();

        public HiSqlProvider Context { get; set; }

        public TableDefinition Table
        {
            get { return _table; }
        }

        public List<FieldDefinition> FieldsOnly {
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

        public UpdateProvider()
        { 
            
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


        public  int ExecCommand()
        {
            string _sql = this.ToSql();
            
            return this.Context.DBO.ExecCommand(_sql);
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
                //_list_data = lstdata;
                foreach (var obj in lstdata)
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

                _queue.Add("table|list");
            }
            else
                throw new Exception($"已经指定了一个Update 不允许重复指定");
            return this;
        }

        public IUpdate Set(object obj)
        {
            if (_queue.HasQueue("table",true))
            {
                if (!_queue.HasQueue("set"))
                {
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
                        if (string.IsNullOrEmpty(filterDefinition.Field.TabName) )
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


        public Tuple<Dictionary<string, string>, Dictionary<string, string>> CheckData(bool isDic,TableDefinition table, object objdata, IDMInitalize dMInitalize, TabInfo tabinfo, List<string> fields, bool isonly)
        {
            if (table != null)
            {
                Tuple<Dictionary<string, string>, Dictionary<string, string>> result;
                Type type = objdata.GetType();
                IDMTab mytab = (IDMTab)dMInitalize;
                List<PropertyInfo> _attrs = type.GetProperties().Where(p => p.MemberType == MemberTypes.Property && p.CanRead == true).ToList();
                result = CheckUpdateData(isDic,this.Wheres.Count > 0 ? false : true, _attrs, tabinfo.GetColumns, objdata, fields, isonly);
                return result;
            }
            else throw new Exception($"找不到相关表信息");
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
                    var objprop = attrs.Where(p => p.Name.ToLower() == hiColumn.ColumnName.ToLower()).FirstOrDefault();
                    //if (hiColumn.IsRequire && !hiColumn.IsIdentity && objprop == null)
                    //{
                    //    throw new Exception($"字段[{hiColumn.ColumnName}] 为必填 无法数据提交");
                    //}
                    if (hiColumn.IsBllKey && objprop == null && isRequireKey)
                    {
                        throw new Exception($"字段[{hiColumn.ColumnName}] 为业务主键或表主键 更新表数据时必填");
                    }
                    if (objprop != null && objprop.GetValue(objdata) != null   )
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

                            if (_value.Length >= hiColumn.FieldLen)
                            {
                                throw new Exception($"字段[{objprop.Name}]的值[{_value}]超过了限制长度[{hiColumn.FieldLen}] 无法数据提交");
                            }
                            if (hiColumn.IsRequire)
                            {
                                if (string.IsNullOrEmpty(_value.Trim()))
                                    throw new Exception($"字段[{objprop.Name}] 为必填 无法数据提交");
                            }

                            _values.Add(hiColumn.ColumnName, $"'{_value.ToSqlInject()}'");
                            if (hiColumn.IsBllKey)
                                _primary.Add(hiColumn.ColumnName, $"'{_value.ToSqlInject()}'");

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
                            if (_value.LengthZH() > hiColumn.FieldLen)
                            {
                                throw new Exception($"字段[{objprop.Name}]的值[{_value}]超过了限制长度[{hiColumn.FieldLen}] 无法数据提交");
                            }
                            if (hiColumn.IsRequire)
                            {
                                if (string.IsNullOrEmpty(_value.Trim()))
                                    throw new Exception($"字段[{objprop.Name}] 为必填 无法数据提交");
                            }
                            _values.Add(hiColumn.ColumnName, $"'{_value.ToSqlInject()}'");
                            if (hiColumn.IsBllKey)
                                _primary.Add(hiColumn.ColumnName, $"'{_value.ToSqlInject()}'");
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
                            _values.Add(hiColumn.ColumnName, $"{_value}");
                            if (hiColumn.IsBllKey)
                                _primary.Add(hiColumn.ColumnName, $"{_value}");
                        }
                        else if (hiColumn.FieldType.IsIn<HiType>(HiType.DATE, HiType.DATETIME))
                        {
                            DateTime dtime = (DateTime)objprop.GetValue(objdata);

                            //DateTime dtime = DateTime.Now;
                            if (dtime != null && dtime != DateTime.MinValue)
                            {
                                _values.Add(hiColumn.ColumnName, $"'{dtime.ToString("yyyy-MM-dd HH:mm:ss.fff")}'");
                                if (hiColumn.IsBllKey)
                                    _primary.Add(hiColumn.ColumnName, $"'{dtime.ToString("yyyy-MM-dd HH:mm:ss.fff")}'");
                            }
                        }
                        else if (hiColumn.FieldType.IsIn<HiType>(HiType.BOOL)) //add by tgm date:2021.10.27
                        {
                            if ((bool)objprop.GetValue(objdata) == true)
                            {
                                if (Context.CurrentConnectionConfig.DbType.IsIn<DBType>(DBType.PostGreSql, DBType.Hana))
                                {
                                    _value = "True";
                                    _values.Add(hiColumn.ColumnName, $"{_value}");
                                }
                                else
                                {
                                    _value = "1";
                                    _values.Add(hiColumn.ColumnName, $"'{_value}'");
                                }
                            }
                            else
                            {
                                if (Context.CurrentConnectionConfig.DbType.IsIn<DBType>(DBType.PostGreSql, DBType.Hana))
                                {
                                    _value = "False";
                                    _values.Add(hiColumn.ColumnName, $"{_value}");
                                }
                                else
                                {
                                    _value = "0";
                                    _values.Add(hiColumn.ColumnName, $"'{_value}'");
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
                            _values.Add(hiColumn.ColumnName, $"'{_value}'");
                            if (hiColumn.IsBllKey)
                                _primary.Add(hiColumn.ColumnName, $"'{_value}'");
                        }
                    }
                }
                else
                {
                    
                    if (hiColumn.IsBllKey && _dic == null && isRequireKey)
                    {
                        throw new Exception($"字段[{hiColumn.ColumnName}] 为业务主键或表主键 更新表数据时必填");
                    }

                    if (_dic .ContainsKey(hiColumn.ColumnName))
                    {
                        if (hiColumn.FieldType.IsIn<HiType>(HiType.NVARCHAR, HiType.NCHAR, HiType.GUID))
                        {
                            //中文按1个字符计算
                            //_value = "test";
                            _value = _dic[hiColumn.ColumnName].ToString();

                            if (_value == null)
                            {
                                if (hiColumn.IsRequire)
                                    throw new Exception($"字段[{hiColumn.ColumnName}] 为必填 无法数据提交");
                                else
                                    continue;
                            }

                            if (_value.Length >= hiColumn.FieldLen)
                            {
                                throw new Exception($"字段[{hiColumn.ColumnName}]的值[{_value}]超过了限制长度[{hiColumn.FieldLen}] 无法数据提交");
                            }
                            if (hiColumn.IsRequire)
                            {
                                if (string.IsNullOrEmpty(_value.Trim()))
                                    throw new Exception($"字段[{hiColumn.ColumnName}] 为必填 无法数据提交");
                            }

                            _values.Add(hiColumn.ColumnName, $"'{_value.ToSqlInject()}'");
                            if (hiColumn.IsBllKey)
                                _primary.Add(hiColumn.ColumnName, $"'{_value.ToSqlInject()}'");
                        }
                        else if (hiColumn.FieldType.IsIn<HiType>(HiType.VARCHAR, HiType.CHAR, HiType.TEXT))
                        {
                            //中文按两个字符计算
                            _value = _dic[hiColumn.ColumnName].ToString();
                            if (_value == null)
                            {
                                if (hiColumn.IsRequire)
                                    throw new Exception($"字段[{hiColumn.ColumnName}] 为必填 无法数据提交");
                                else
                                    continue;
                            }

                            //_value = "test";
                            if (_value.LengthZH() > hiColumn.FieldLen)
                            {
                                throw new Exception($"字段[{hiColumn.ColumnName}]的值[{_value}]超过了限制长度[{hiColumn.FieldLen}] 无法数据提交");
                            }
                            if (hiColumn.IsRequire)
                            {
                                if (string.IsNullOrEmpty(_value.Trim()))
                                    throw new Exception($"字段[{hiColumn.ColumnName}] 为必填 无法数据提交");
                            }
                            _values.Add(hiColumn.ColumnName, $"'{_value.ToSqlInject()}'");
                            if (hiColumn.IsBllKey)
                                _primary.Add(hiColumn.ColumnName, $"'{_value.ToSqlInject()}'");
                        }
                        else if (hiColumn.FieldType.IsIn<HiType>(HiType.INT, HiType.BIGINT, HiType.DECIMAL, HiType.SMALLINT))
                        {
                            _value = _dic[hiColumn.ColumnName].ToString();
                            if (_value == null)
                            {
                                if (hiColumn.IsRequire)
                                    throw new Exception($"字段[{hiColumn.ColumnName}] 为必填 无法数据提交");
                                else
                                    continue;
                            }
                            //_value = "1";
                            _values.Add(hiColumn.ColumnName, $"{_value}");
                            if (hiColumn.IsBllKey)
                                _primary.Add(hiColumn.ColumnName, $"{_value}");
                        }
                        else if (hiColumn.FieldType.IsIn<HiType>(HiType.DATE, HiType.DATETIME))
                        {
                            DateTime dtime = DateTime.Parse(_dic[hiColumn.ColumnName].ToString());
                            //DateTime dtime = DateTime.Now;
                            if (dtime != null)
                            {
                                _values.Add(hiColumn.ColumnName, $"'{dtime.ToString("yyyy-MM-dd HH:mm:ss.fff")}'");
                                if (hiColumn.IsBllKey)
                                    _primary.Add(hiColumn.ColumnName, $"'{dtime.ToString("yyyy-MM-dd HH:mm:ss.fff")}'");
                            }
                        }
                        else
                        {
                            _value = _dic[hiColumn.ColumnName].ToString().ToSqlInject();
                            if (_value == null)
                            {
                                if (hiColumn.IsRequire)
                                    throw new Exception($"字段[{hiColumn.ColumnName}] 为必填 无法数据提交");
                                else
                                    continue;
                            }
                            _values.Add(hiColumn.ColumnName, $"'{_value}'");
                            if (hiColumn.IsBllKey)
                                _primary.Add(hiColumn.ColumnName, $"'{_value}'");
                        }
                    }

                }

                if (fields.Count > 0)
                {
                    //说明有指定更新，或排除更新字段
                    if (isonly)
                    {
                        //仅在fields列表中的 才会更新字段
                        if (fields.Where(f => f.ToLower() == hiColumn.ColumnName.ToLower()).Count() == 0 || hiColumn.IsBllKey)
                        {
                            if (_values.ContainsKey(hiColumn.ColumnName))
                                _values.Remove(hiColumn.ColumnName);
                        }
                    }
                    else
                    {
                        if (fields.Where(f => f.ToLower() == hiColumn.ColumnName.ToLower()).Count() > 0 || hiColumn.IsBllKey)
                        {
                            if (_values.ContainsKey(hiColumn.ColumnName))
                                _values.Remove(hiColumn.ColumnName);
                        }
                    }
                }


                if (hiColumn.ColumnName.ToLower() == "CreateTime".ToLower() || hiColumn.ColumnName.ToLower() == "CreateName".ToLower())
                {

                    //if (hiColumn.DBDefault != HiTypeDBDefault.FUNDATE)
                    //    _values.Add(hiColumn.ColumnName, $"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}'");
                    if (_values.ContainsKey(hiColumn.ColumnName))
                        _values.Remove(hiColumn.ColumnName);
                }
                else if (hiColumn.ColumnName.ToLower() == "ModiName".ToLower())
                {
                    if(!_values.ContainsKey(hiColumn.ColumnName))
                        _values.Add(hiColumn.ColumnName, $"'{Context.CurrentConnectionConfig.User}'");
                    else
                        _values[hiColumn.ColumnName]= $"'{Context.CurrentConnectionConfig.User}'";


                }
                else if (hiColumn.ColumnName.ToLower() == "ModiTime".ToLower())
                {
                    if (!_values.ContainsKey(hiColumn.ColumnName))
                        _values.Add(hiColumn.ColumnName, $"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}'");
                    else
                        _values[hiColumn.ColumnName] = $"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}'";
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
