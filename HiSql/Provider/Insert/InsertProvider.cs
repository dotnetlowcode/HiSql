using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HiSql
{
    public class InsertProvider : IInsert
    {
        TableDefinition _table;
        public HiSqlProvider Context { get; set; }
        List<object> _list_data = new List<object>();

        StringBuilder _Sql = new StringBuilder();

        List<string> _lstsql = new List<string>();
        public virtual Action<int,int> OnPageExec => this.Context.CurrentConnectionConfig.AppEvents?.OnPageExec;
        //分批次执行
        bool _batch_exec = false;

        /// <summary>
        /// 是否分批次执行SQL
        /// </summary>
        public virtual bool IsBatchExec
        {
            get => _batch_exec;
            set => _batch_exec = value;
        }


        bool _mergeinto = false;
        public IDbConfig DbConfig
        {
            get;set;

        }
        SynTaxQueue _queue = new SynTaxQueue();


        public InsertProvider()
        { 
            
        }
        public TableDefinition Table
        {
            get { return _table; }
        }
        public List<object> Data
        {
            get { return _list_data; }
        }
        public  virtual int ExecCommand()
        {


            int i = 0;

            if (!this.IsBatchExec)
            {
                if (!string.IsNullOrEmpty(_Sql.ToString()))
                {
                    if (this.Context.CurrentConnectionConfig.DbType == DBType.SqlServer)
                        _Sql.AppendLine("select @@identity;");

                    if (this.Context.CurrentConnectionConfig.DbType == DBType.Hana)
                    {
                        _Sql = new StringBuilder().AppendLine("do begin").AppendLine(_Sql.ToString()).AppendLine("end;");

                    }
                    else if (this.Context.CurrentConnectionConfig.DbType == DBType.Oracle)
                    {
                        _Sql = new StringBuilder().AppendLine($"declare v_effect integer;  {Environment.NewLine}begin{Environment.NewLine}").AppendLine(_Sql.ToString()).AppendLine("end;");
                    }

                    i = this.Context.DBO.ExecCommand(_Sql.ToString());
                    _Sql = new StringBuilder();
                    return i;
                }
                else
                    return i;
            }
            else
            {
                int _idx = 0;
                foreach (string n in _lstsql)
                {
                    _idx++;

                    StringBuilder _sb_n = new StringBuilder();
                    if (!string.IsNullOrEmpty(n))
                    {
                        if (this.Context.CurrentConnectionConfig.DbType == DBType.Hana)
                        {
                            _sb_n.AppendLine("do begin");
                        }
                        _sb_n.AppendLine(n);
                        _sb_n.AppendLine("end;");
                        i = this.Context.DBO.ExecCommand(_sb_n.ToString());

                        if (OnPageExec != null)
                        {
                            Task.Run(() => { OnPageExec(_lstsql.Count, _idx); });
                        }
                    }
                }
                return i;
            }

            
        }

        void checkData()
        {
            IDM sqldm = null;
            TabInfo tabinfo;
            string _insertTabName = string.Empty;
            string _cacheinsertTabName = string.Empty;
            if (this.Table != null)
            {
                sqldm = Instance.CreateInstance<IDM>($"{Constants.NameSpace}.{this.Context.CurrentConnectionConfig.DbType.ToString()}{DbInterFace.DM.ToString()}");
                sqldm.Context = this.Context;
                tabinfo = sqldm.GetTabStruct(this.Table.TabName);
                _insertTabName = this.Table.TabName;
                //由于DBType.PostGreSql mergeinto的方法实现逻辑与其它的数据库完全不一样只能特殊处理
                if (_mergeinto  && Context.CurrentConnectionConfig.DbType != DBType.PostGreSql)
                {
                    string _json =  JsonConvert.SerializeObject(tabinfo);
                    //TabInfo tabinfo2 =  JsonConvert.DeserializeObject<TabInfo>(_json);
                    TabInfo tabinfo2 = ClassExtensions.DeepCopy<TabInfo>(tabinfo);
                    //有数据则更新无数据则插入
                    /*
                     预先先将数据插入到临时表中 再通过merge into方法写入
                     */

                    if (tabinfo.IsAllowMergeInto)
                    {
                        //设置临时表
                        if (Context.CurrentConnectionConfig.DbType == DBType.Hana)//|| Context.CurrentConnectionConfig.DbType == DBType.MySql
                        {
                            //hana 的全局临时表 与标准表的命名规则是一样的
                            //mysql没有全局临时表的概念
                            _insertTabName = $"tmp_global_{tabinfo.TabModel.TabName}_{Thread.CurrentThread.ManagedThreadId.ToString() }_{tabinfo.TabModel.TabName.GetHashCode().ToString().Substring(1)}";
                            _cacheinsertTabName = $"##{tabinfo.TabModel.TabName}";
                            tabinfo2.TabModel.TabName = $"##{tabinfo.TabModel.TabName}";
                            //tabinfo2.TabModel.TabName = _insertTabName;
                            tabinfo2.TabModel.TabReName = _insertTabName;
                        }
                        else if (Context.CurrentConnectionConfig.DbType == DBType.Oracle)
                        {
                            _insertTabName = $"tmp_local_{tabinfo.TabModel.TabName}_{Thread.CurrentThread.ManagedThreadId.ToString() }_{tabinfo.TabModel.TabName.GetHashCode().ToString().Substring(1)}";
                            _cacheinsertTabName = $"#{tabinfo.TabModel.TabName}";
                            tabinfo2.TabModel.TabName = $"#{tabinfo.TabModel.TabName}";
                            //tabinfo2.TabModel.TabName = _insertTabName;
                            tabinfo2.TabModel.TabReName = _insertTabName;
                        }
                        else
                        {
                            tabinfo2.TabModel.TabName = $"#{tabinfo.TabModel.TabName}";
                            _cacheinsertTabName = $"#{tabinfo.TabModel.TabName}";
                            _insertTabName = tabinfo2.TabModel.TabName;
                        }
                        IDMTab dMTab = (IDMTab)sqldm;
                        dMTab.Context = sqldm.Context;
                        //创建临时表
                        //int effect = dMTab.BuildTabCreate(tabinfo);
                        string _temp_sql = dMTab.BuildTabCreateSql(tabinfo2.TabModel, tabinfo2.Columns,true);

                        if (this.IsBatchExec)
                        {
                            //创建临时表 由于HANA插入特性，当数据量大时会分页插入，所以临时表需要分开创建
                            //int _effect = this.Context.DBO.ExecCommand(_temp_sql, null);

                            if (Context.CurrentConnectionConfig.DbType == DBType.Oracle)
                                this.Context.DBO.ExecCommand(_temp_sql);//由于oralce的特性 declare begin end;需要单独创建表
                            else
                                _lstsql.Add(_temp_sql);
                        }
                        else
                        {
                            if (Context.CurrentConnectionConfig.DbType == DBType.Oracle)
                                this.Context.DBO.ExecCommand(_temp_sql);//由于oralce的特性 declare begin end;需要单独创建表
                            else
                                _Sql.AppendLine(_temp_sql);
                        }
                        //缓存临时表结构
                        sqldm.Context.MCache.SetCache(tabinfo2.TabModel.TabName, tabinfo2, DateTimeOffset.Now.AddMinutes(1));
                    }
                    else
                    {
                        throw new Exception($"表[{this.Table.TabName}]不支持[Modi]操作,因为该表的主键和业务主键是自增长ID");
                    }
                }
            }
            else
                throw new Exception("未指定要插入的表");

             
            //仅限于数据插入
            StringBuilder sb_sql = new StringBuilder();
            if (this.Data != null && this.Data.Count > 0)
            {
                Type type = this.Data[0].GetType();
                bool _isdic = type == typeof(Dictionary<string, string>)  || type== typeof(Dictionary<string, object>);
                List<PropertyInfo> attrs = type.GetProperties().Where(p => p.MemberType == MemberTypes.Property && p.CanRead == true).ToList();
                int page = this.Data.Count <= DbConfig.BlukSize ? 1 : this.Data.Count % DbConfig.BlukSize == 0 ? this.Data.Count / DbConfig.BlukSize : this.Data.Count / DbConfig.BlukSize + 1;
                //分数据包 包的大小决定了数据插入的性能问题
                //insert values 的方式 包大小最高不能超过1000
                for (int p = 0; p < page; p++)
                {
                    for (int i = p * DbConfig.BlukSize; i < (p + 1) * DbConfig.BlukSize; i++)
                    {
                        if (i >= this.Data.Count)
                        {
                            break;
                        }
                        else
                        {
                            Dictionary<string, string> _values = CheckInsertData(_isdic, attrs, tabinfo.GetColumns, this.Data[i]);
                            if (_values.Count == 0)
                                throw new Exception($"向表[{_insertTabName}]插入数据值中无任何配置的字段");
                            string _sql = sqldm.BuildInsertSql(_values, i > p * DbConfig.BlukSize).Replace("[$TabName$]", _insertTabName);//i > p * _bluksize
                            sb_sql.Append(_sql);
                        }
                    }
                    if (Context.CurrentConnectionConfig.DbType.IsIn<DBType>(DBType.MySql, DBType.Oracle,DBType.Hana))
                        sb_sql.AppendLine(";");

                    


                    if (Context.CurrentConnectionConfig.DbType.IsIn<DBType>(DBType.PostGreSql))
                    {
                        //由于DBType.PostGreSql mergeinto的方法实现逻辑与其它的数据库完全不一样只能特殊处理
                        if (_mergeinto)
                        {
                            if (tabinfo.IsAllowMergeInto)
                            {
                                TabInfo _tabtarget = sqldm.GetTabStruct(this.Table.TabName);
                                string _mergesql = sqldm.BuildMergeIntoSqlSequence(_tabtarget);
                                _mergesql = _mergesql.Replace("[$Insert$]", sb_sql.ToString());
                                sb_sql = new StringBuilder().AppendLine(_mergesql);
                            }
                            else
                                throw new Exception($"表[{this.Table.TabName}]不支持[Modi]操作,因为该表的主键和业务主键是自增长ID");
                        }
                        else
                            sb_sql.Append(";");
                    }

                    //是否分批次执行
                    if (IsBatchExec)
                    {
                        _lstsql.Add(sb_sql.ToString());
                        sb_sql = new StringBuilder();
                    }

                }
                string sql = sb_sql.ToString();
                //sql=DbConfig.Code_Block.Replace("[$SQL$]", sql);

                if (!IsBatchExec)
                {
                    _Sql.Append(sql);
                }
                sb_sql = new StringBuilder();

                if (_mergeinto && Context.CurrentConnectionConfig.DbType != DBType.PostGreSql)
                {
                    TabInfo _tabtarget = sqldm.GetTabStruct(this.Table.TabName);
                    TabInfo _tabsource = sqldm.Context.MCache.GetCache<TabInfo>(_cacheinsertTabName);

                    string _mergesql = sqldm.BuildMergeIntoSql(_tabtarget, _tabsource);
                    if (this.Context.CurrentConnectionConfig.DbType == DBType.Hana || this.Context.CurrentConnectionConfig.DbType == DBType.Oracle)
                    {
                        string _truncate = DbConfig.Delete_TrunCate.Replace("[$Schema$]", Context.CurrentConnectionConfig.Schema).Replace("[$TabName$]", _insertTabName);
                        string _droptable = DbConfig.Drop_Table.Replace("[$Schema$]", Context.CurrentConnectionConfig.Schema).Replace("[$TabName$]", _insertTabName);
                        _mergesql = $"{_mergesql}{Environment.NewLine} {_truncate}{Environment.NewLine} {_droptable}";
                    }
                    

                    if (this.IsBatchExec)
                    {
                        _lstsql.Add(_mergesql);
                    }
                    else
                    {
                        _Sql.AppendLine("");
                        _Sql.AppendLine(_mergesql);
                    }
                }
                else
                {
                    //if(!this.IsBatchExec && Context.CurrentConnectionConfig.DbType == DBType.Hana)
                    //    _Sql = new StringBuilder().AppendLine("do begin").AppendLine(_Sql.ToString()).AppendLine("end;");
                }
                
            }
            else
                throw new Exception("无数据可插入");
            

        }

       
        public IInsert Insert(string tabname, object objdata)
        {
            if (_queue.HasQueue("modi"))
                throw new Exception($"已经指定了[Modi]不允许再指定[Insert]");
            _list_data = new List<object>();
            _table = new TableDefinition(tabname);
            _list_data.Add(objdata);
            
            checkData();
            _queue.Add("insert");
            return this;
        }

        

        public IInsert Insert(string tabname, List<object> lstdata)
        {
            if (_queue.HasQueue("modi"))
                throw new Exception($"已经指定了[Modi]不允许再指定[Insert]");
            _list_data = new List<object>();
            _table = new TableDefinition(tabname);
            //foreach (object obj in lstdata)
            //{
            //    _list_data.Add(obj);
            //}
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

            checkData();
            _queue.Add("insert");
            return this;
        }

        public virtual string ToSql()
        {

            if (this.IsBatchExec)
            {
                StringBuilder _sb_batch = new StringBuilder();
                if (Context.CurrentConnectionConfig.DbType == DBType.Hana)
                {
                    _sb_batch.AppendLine("do begin");
                }
                foreach (string n in _lstsql)
                {


                    _sb_batch.AppendLine(n);
                }
                _sb_batch.AppendLine("end;");
                return _sb_batch.ToString();
            }
            else
            {
                if (Context.CurrentConnectionConfig.DbType == DBType.Hana)
                    _Sql = new StringBuilder().AppendLine("do begin").AppendLine(_Sql.ToString()).AppendLine("end;");
                return _Sql.ToString();
            }
        }

        public IInsert Insert<T>(T objdata)
        {
            if (_queue.HasQueue("modi"))
                throw new Exception($"已经指定了[Modi]不允许再指定[Insert]");
            _list_data = new List<object>();
            _table = new TableDefinition(typeof(T).Name);
            _list_data.Add((object)objdata);
            checkData();
            _queue.Add("insert");


            return this;

        }

        public IInsert Insert<T>(string tabname, T objdata)
        {
            if (_queue.HasQueue("modi"))
                throw new Exception($"已经指定了[Modi]不允许再指定[Insert]");

           
            _list_data = new List<object>();
            _table = new TableDefinition(tabname);
            _list_data.Add((object)objdata);
            
            checkData();
            _queue.Add("insert");

            return this;
        }

        /// <summary>
        /// 按实体的名称写入表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lstdata"></param>
        /// <returns></returns>
        public IInsert Insert<T>(List<T> lstdata)
        {
            if (_queue.HasQueue("modi"))
                throw new Exception($"已经指定了[Modi]不允许再指定[Insert]");
            _list_data = new List<object>();
            _table = new TableDefinition(typeof(T).Name);
            foreach (T obj in lstdata)
            {
                _list_data.Add((object)obj);
            }


            checkData();
            _queue.Add("insert");


            return this;
        }

        /// <summary>
        /// 指定表中写入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tabname"></param>
        /// <param name="lstdata"></param>
        /// <returns></returns>
        public IInsert Insert<T>(string tabname, List<T> lstdata)
        {
            if (_queue.HasQueue("modi"))
                throw new Exception($"已经指定了[Modi]不允许再指定[Insert]");
            _list_data = new List<object>();
            _table = new TableDefinition(tabname);
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
            checkData();
            _queue.Add("insert");


            return this;
        }


        public IInsert Modi(string tabname, object objdata)
        {
            _mergeinto = true;
            if (!_queue.HasQueue("insert"))
            {
                if (!_queue.HasQueue("modi"))
                {
                    //写入到临时表中
                    Insert(tabname, objdata);

                    _queue.Add("modi");
                }
                else
                    throw new Exception($"已经指定了[Modi]不允许再指定[Modi]");


            }
            else
                throw new Exception($"已经指定了[Insert]不允许再指定[Modi]");

            return this;
        }
        /// <summary>
        /// 有数据则更新无数据则插入
        /// </summary>
        /// <param name="tabname"></param>
        /// <param name="lstdata"></param>
        /// <returns></returns>
        public IInsert Modi(string tabname, List<object> lstdata)
        {
            _mergeinto = true;
            if (!_queue.HasQueue("insert"))
            {
                if (!_queue.HasQueue("modi"))
                {
                    //写入到临时表中
                    Insert(tabname, lstdata);

                    _queue.Add("modi");
                }
                else
                    throw new Exception($"已经指定了[Modi]不允许再指定[Modi]");


            }else
                throw new Exception($"已经指定了[Insert]不允许再指定[Modi]");

            return this ;
        }

        public IInsert Modi<T>(string tabname, T objdata) {
            _mergeinto = true;
            if (!_queue.HasQueue("insert"))
            {
                if (!_queue.HasQueue("modi"))
                {
                    //写入到临时表中
                    Insert<T>(tabname, objdata);

                    _queue.Add("modi");
                }
                else
                    throw new Exception($"已经指定了[Modi]不允许再指定[Modi]");


            }
            else
                throw new Exception($"已经指定了[Insert]不允许再指定[Modi]");

            return this;
        }
        public IInsert Modi<T>(T objdata) {
            _mergeinto = true;
            if (!_queue.HasQueue("insert"))
            {
                if (!_queue.HasQueue("modi"))
                {
                    //写入到临时表中
                    Insert<T>( objdata);

                    _queue.Add("modi");
                }
                else
                    throw new Exception($"已经指定了[Modi]不允许再指定[Modi]");


            }
            else
                throw new Exception($"已经指定了[Insert]不允许再指定[Modi]");

            return this;
        }
        public IInsert Modi<T>(List<T> lstdata) {
            _mergeinto = true;
            if (!_queue.HasQueue("insert"))
            {
                if (!_queue.HasQueue("modi"))
                {
                    //写入到临时表中
                    Insert<T>(lstdata);

                    _queue.Add("modi");
                }
                else
                    throw new Exception($"已经指定了[Modi]不允许再指定[Modi]");


            }
            else
                throw new Exception($"已经指定了[Insert]不允许再指定[Modi]");

            return this;
        }
        public IInsert Modi<T>(string tabname, List<T> lstdata) {
            _mergeinto = true;
            if (!_queue.HasQueue("insert"))
            {
                if (!_queue.HasQueue("modi"))
                {
                    //写入到临时表中
                    Insert<T>(tabname,lstdata);

                    _queue.Add("modi");
                }
                else
                    throw new Exception($"已经指定了[Modi]不允许再指定[Modi]");


            }
            else
                throw new Exception($"已经指定了[Insert]不允许再指定[Modi]");

            return this;
        }



        #region 私有方法 add by tgm date:20211016
        Dictionary<string, string> CheckInsertData(bool isDic, List<PropertyInfo> attrs, List<HiColumn> hiColumns, object objdata)
        {
            Dictionary<string, string> _values = new Dictionary<string, string>();

            var arrcol_reg = hiColumns.Where(h => string.IsNullOrEmpty(h.Regex) );
            var arrcol_tab = hiColumns.Where(h => h.IsRefTab);
            

            Dictionary<string, string> _dic =new Dictionary<string, string> (StringComparer.OrdinalIgnoreCase);
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
                }else
                    _dic = (Dictionary<string, string>)objdata;
            }

            string _value;
            foreach (HiColumn hiColumn in hiColumns)
            {
                if (!isDic)
                {
                    #region 匿名类或实体类的检测
                    var objprop = attrs.Where(p => p.Name.ToLower() == hiColumn.ColumnName.ToLower()).FirstOrDefault();
                    if (hiColumn.IsRequire && !hiColumn.IsIdentity && objprop == null)
                    {
                        throw new Exception($"字段[{hiColumn.ColumnName}] 为必填 无法数据提交");
                    }
                    if (hiColumn.IsIdentity && objprop != null)
                    {
                        _value = objprop.GetValue(objdata).ToString();
                        if (_value == "0")
                            continue;
                        else
                            throw new Exception($"字段[{hiColumn.ColumnName}] 为自增长字段 不需要外部赋值");
                    }

                    if (objprop != null && objprop.GetValue(objdata) != null)
                    {
                        if (hiColumn.FieldType.IsIn<HiType>(HiType.NVARCHAR, HiType.NCHAR, HiType.GUID))
                        {
                            //中文按1个字符计算
                            //_value = "test";

                            _value = (string)objprop.GetValue(objdata);
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

                        }
                        else if (hiColumn.FieldType.IsIn<HiType>(HiType.VARCHAR, HiType.CHAR, HiType.TEXT))
                        {

                            //中文按两个字符计算
                            _value = (string)objprop.GetValue(objdata);
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
                        }
                        else if (hiColumn.FieldType.IsIn<HiType>(HiType.INT, HiType.BIGINT, HiType.DECIMAL, HiType.SMALLINT))
                        {

                            _value = objprop.GetValue(objdata).ToString();
                            if (!Tool.IsDecimal(_value))
                            {
                                throw new Exception($"字段[{hiColumn.ColumnName}]的值[{_value}]非数字,不符合规范");
                            }
                            //_value = "1";


                            _values.Add(hiColumn.ColumnName, $"{_value}");
                        }
                        else if (hiColumn.FieldType.IsIn<HiType>(HiType.DATE, HiType.DATETIME))
                        {

                            DateTime dtime = (DateTime)objprop.GetValue(objdata);
                            //DateTime dtime = DateTime.Now;
                            if (dtime != null && dtime != DateTime.MinValue)
                            {
                                _values.Add(hiColumn.ColumnName, $"'{dtime.ToString("yyyy-MM-dd HH:mm:ss.fff")}'");
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
                            _values.Add(hiColumn.ColumnName, $"'{_value}'");
                        }
                    }
                    else
                    {
                        //非自增长且不允许为空且没有设置默认值且没有传值的情况下应该抛出异常 不然底层库会报错
                        if (!hiColumn.IsNull && hiColumn.DBDefault == HiTypeDBDefault.NONE && !hiColumn.IsIdentity)
                        {
                            throw new Exception($"字段[{hiColumn.ColumnName}]不允许为空数据库中未设置默认值 且插入数据值中未指定值");
                        }
                    }

                    if (hiColumn.ColumnName == "CreateTime" || hiColumn.ColumnName == "ModiTime")
                    {

                        if (hiColumn.DBDefault != HiTypeDBDefault.FUNDATE)
                            _values.Add(hiColumn.ColumnName, $"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}'");
                    }
                    else if (hiColumn.ColumnName == "CreateName" || hiColumn.ColumnName == "ModiName")
                    {
                        _values.Add(hiColumn.ColumnName, $"'{Context.CurrentConnectionConfig.User}'");
                    }
                    #endregion
                }
                else
                {
                    
                    #region 字典数据
                    if (hiColumn.IsRequire && !hiColumn.IsIdentity && !_dic.ContainsKey(hiColumn.ColumnName))
                    {
                        throw new Exception($"字段[{hiColumn.ColumnName}] 为必填 无法数据提交");
                    }
                    if (hiColumn.IsIdentity && _dic.ContainsKey(hiColumn.ColumnName))
                    {
                        _value = _dic[hiColumn.ColumnName].ToString();
                        if (_value == "0" || string.IsNullOrEmpty(_value))
                            continue;
                        else
                            throw new Exception($"字段[{hiColumn.ColumnName}] 为自增长字段 不需要外部赋值");
                    }
                    if (_dic.ContainsKey(hiColumn.ColumnName))
                    {
                        if (hiColumn.FieldType.IsIn<HiType>(HiType.NVARCHAR, HiType.NCHAR, HiType.GUID))
                        {
                            //中文按1个字符计算
                            //_value = "test";
                            _value = _dic[hiColumn.ColumnName].ToString();
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

                        }
                        else if (hiColumn.FieldType.IsIn<HiType>(HiType.VARCHAR, HiType.CHAR, HiType.TEXT))
                        {
                            //中文按两个字符计算
                            _value = _dic[hiColumn.ColumnName].ToString();
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
                        }
                        else if (hiColumn.FieldType.IsIn<HiType>(HiType.INT, HiType.BIGINT, HiType.DECIMAL, HiType.SMALLINT))
                        {
                            _value = _dic[hiColumn.ColumnName].ToString();
                            //_value = "1";
                            _values.Add(hiColumn.ColumnName, $"{_value}");
                        }
                        else if (hiColumn.FieldType.IsIn<HiType>(HiType.DATE, HiType.DATETIME))
                        {
                            DateTime dtime = DateTime.Parse(_dic[hiColumn.ColumnName].ToString());
                            //DateTime dtime = DateTime.Now;
                            if (dtime != null)
                            {
                                _values.Add(hiColumn.ColumnName, $"'{dtime.ToString("yyyy-MM-dd HH:mm:ss.fff")}'");
                            }
                        }
                        else
                        {
                            _value = _dic[hiColumn.ColumnName].ToString();
                            _values.Add(hiColumn.ColumnName, $"'{_value}'");
                        }
                    }
                    else
                    {
                        //非自增长且不允许为空且没有设置默认值且没有传值的情况下应该抛出异常 不然底层库会报错
                        if (!hiColumn.IsNull && hiColumn.DBDefault == HiTypeDBDefault.NONE && !hiColumn.IsIdentity)
                        {
                            throw new Exception($"字段[{hiColumn.ColumnName}]不允许为空数据库中未设置默认值 且插入数据值中未指定值");
                        }
                    }
                    if (hiColumn.ColumnName.ToLower() == "CreateTime".ToLower() || hiColumn.ColumnName.ToLower() == "ModiTime".ToLower())
                    {

                        if (hiColumn.DBDefault != HiTypeDBDefault.FUNDATE)
                            _values.Add(hiColumn.ColumnName, $"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}'");
                    }
                    else if (hiColumn.ColumnName.ToLower() == "CreateName".ToLower() || hiColumn.ColumnName.ToLower() == "ModiName".ToLower())
                    {
                        _values.Add(hiColumn.ColumnName, $"'{Context.CurrentConnectionConfig.User}'");
                    }
                    #endregion
                }
            }

            


            return _values;
        }

        #endregion
    }
}
