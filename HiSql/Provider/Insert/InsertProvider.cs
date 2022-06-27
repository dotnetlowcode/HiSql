using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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
        public virtual Action<int, int> OnPageExec => this.Context.CurrentConnectionConfig.AppEvents?.OnPageExec;
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
            get; set;

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

        public virtual int ExecCommand()
        {
            return ExecCommandAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }


        public virtual async Task<int> ExecCommandAsync()
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
                    else if (this.Context.CurrentConnectionConfig.DbType == DBType.Oracle || Context.CurrentConnectionConfig.DbType == DBType.DaMeng)
                    {
                        _Sql = new StringBuilder().AppendLine($"declare v_effect integer;  {Environment.NewLine}begin{Environment.NewLine}").AppendLine(_Sql.ToString()).AppendLine("end;");
                    }

                    //Stopwatch sw = new Stopwatch();
                    //sw.Start();
                    i = await this.Context.DBO.ExecCommandAsync(_Sql.ToString());
                    //sw.Stop();
                    //Console.WriteLine($"sql单次执行耗时："+sw.Elapsed);
                    _Sql = new StringBuilder();
                    return i;
                }
                else
                    return i;
            }
            else
            {
                int _idx = 0;
                //Stopwatch sw = new Stopwatch();
                //sw.Start();
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
                        
                        if (this.Context.CurrentConnectionConfig.DbType == DBType.Hana)
                        {
                            _sb_n.AppendLine("end;");
                        }

                        i = await this.Context.DBO.ExecCommandAsync(_sb_n.ToString());

                        if (OnPageExec != null)
                        {
                            Task.Run(() => { OnPageExec(_lstsql.Count, _idx); });
                        }
                    }
                }

                //sw.Stop();
                //Console.WriteLine($"sql批量执行耗时：" + sw.Elapsed);
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
                if (_mergeinto && Context.CurrentConnectionConfig.DbType != DBType.PostGreSql)
                {
                    string _json = JsonConvert.SerializeObject(tabinfo);
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
                            _insertTabName = $"tmp_g_{tabinfo.TabModel.TabName}{Thread.CurrentThread.ManagedThreadId.ToString() }{tabinfo.TabModel.TabName.GetHashCode().ToString().Substring(1,3)}";
                            _cacheinsertTabName = $"##{tabinfo.TabModel.TabName}";
                            tabinfo2.TabModel.TabName = $"##{tabinfo.TabModel.TabName}";
                            //tabinfo2.TabModel.TabName = _insertTabName;
                            tabinfo2.TabModel.TabReName = _insertTabName;
                        }
                        else if (Context.CurrentConnectionConfig.DbType == DBType.Oracle || Context.CurrentConnectionConfig.DbType == DBType.DaMeng)
                        {
                            _insertTabName = $"tmp_l_{tabinfo.TabModel.TabName}{Thread.CurrentThread.ManagedThreadId.ToString() }{tabinfo.TabModel.TabName.GetHashCode().ToString().Substring(1, 3)}";
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
                        string _temp_sql = dMTab.BuildTabCreateSql(tabinfo2.TabModel, tabinfo2.Columns, true);

                        if (this.IsBatchExec)
                        {
                            //创建临时表 由于HANA插入特性，当数据量大时会分页插入，所以临时表需要分开创建
                            //int _effect = this.Context.DBO.ExecCommand(_temp_sql, null);

                            if (Context.CurrentConnectionConfig.DbType == DBType.Oracle || Context.CurrentConnectionConfig.DbType == DBType.DaMeng)
                                this.Context.DBO.ExecCommand(_temp_sql);//由于oralce的特性 declare begin end;需要单独创建表
                            else
                                _lstsql.Add(_temp_sql);
                        }
                        else
                        {
                            if (Context.CurrentConnectionConfig.DbType == DBType.Oracle || Context.CurrentConnectionConfig.DbType == DBType.DaMeng)
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



            StringBuilder sb_pcksql = new StringBuilder();

            //仅限于数据插入
            StringBuilder sb_sql = new StringBuilder();
            if (this.Data != null && this.Data.Count > 0)
            {



                Type type = this.Data[0].GetType();
                bool _isdic = type == typeof(Dictionary<string, string>) || type == typeof(Dictionary<string, object>);
                List<PropertyInfo> attrs = type.GetProperties().Where(p => p.MemberType == MemberTypes.Property && p.CanRead == true).ToList();

                if (this.Data.Count > DbConfig.PackageRecord/10)
                {
                    if (attrs.Count < 10)
                        DbConfig.BlukSize = DbConfig.BlukSize;
                    else if (attrs.Count >= 10 && attrs.Count < 20)
                        DbConfig.BlukSize = DbConfig.BlukSize - (DbConfig.BlukSize / 5);
                    else if (attrs.Count >= 20 && attrs.Count < 30)
                        DbConfig.BlukSize = DbConfig.BlukSize - (DbConfig.BlukSize / 5) * 2;
                    else if (attrs.Count >= 30 && attrs.Count < 40)
                        DbConfig.BlukSize = DbConfig.BlukSize - (DbConfig.BlukSize / 5) * 3;
                    else if (attrs.Count >= 50)
                        DbConfig.BlukSize = DbConfig.BlukSize - (DbConfig.BlukSize / 5) * 4;
                    else
                        DbConfig.BlukSize = DbConfig.BlukSize;
                }

                int page = this.Data.Count <= DbConfig.BlukSize ? 1 : this.Data.Count % DbConfig.BlukSize == 0 ? this.Data.Count / DbConfig.BlukSize : this.Data.Count / DbConfig.BlukSize + 1;
                //分数据包 包的大小决定了数据插入的性能问题
                //insert values 的方式 包大小最高不能超过1000

                //List<Dictionary<string, string>> rtnlst=new List<Dictionary<string, string>> ();//=CheckAllData(tabinfo.GetColumns, this.Data);

                //Stopwatch sw= Stopwatch.StartNew();
                //sw.Start();

                List<Dictionary<string, string>> rtnlst = CheckAllData(tabinfo.GetColumns, this.Data);
                //sw.Stop();
                //Console.WriteLine($"检测{this.Data.Count} 条数据耗时{sw.Elapsed}");
                //foreach (var obj in this.Data)
                //{
                //    rtnlst.Add(CheckInsertData(_isdic, attrs, tabinfo.GetColumns, obj));
                //}

                
               
                
                //强制分页分包  如果已经指定了分批次则不需要分包
                bool _forcepage = false;
                int _pcount = 0;
                int _cells = 0;


                //如果分批次执行每个批次中的insert values块是多少
                int _packunit = 0;

                if (rtnlst.Count > 0)
                {

                    if (rtnlst[0].Count > this.DbConfig.PackageCell || rtnlst.Count> this.DbConfig.PackageRecord)
                    {
                        _forcepage = true;
                        _cells = rtnlst[0].Count * rtnlst.Count;

                        ///计算分批次数
                        _pcount= _cells <= this.DbConfig.PackageCells ? 1 : _cells % this.DbConfig.PackageCells == 0 ? _cells / this.DbConfig.PackageCells : _cells / this.DbConfig.PackageCells + 1;

                        if (page > _pcount)
                            _packunit = page / _pcount;
                        else
                            _packunit = 1;
                    }
                }



                int _times = 0;
                int _currbatchidx = 0;
                //一个insert块 200条记录 insert values...
                // 把这块
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
                            //Dictionary<string, string> _values = CheckInsertData(_isdic, attrs, tabinfo.GetColumns, this.Data[i]);
                            Dictionary<string, string> _values = rtnlst[i];
                            if (_values.Count == 0)
                                throw new Exception($"向表[{_insertTabName}]插入数据值中无任何配置的字段");
                            string _sql = sqldm.BuildInsertSql(_values, i > p * DbConfig.BlukSize).Replace("[$TabName$]", _insertTabName);//i > p * _bluksize
                            sb_sql.Append(_sql);
                            _times++;
                        }
                    }
                    if (Context.CurrentConnectionConfig.DbType.IsIn<DBType>(DBType.MySql, DBType.Oracle, DBType.DaMeng, DBType.Hana))
                        sb_sql.AppendLine(";");




                    if (Context.CurrentConnectionConfig.DbType.IsIn<DBType>(DBType.PostGreSql))
                    {
                        //由于DBType.PostGreSql mergeinto的方法实现逻辑与其它的数据库完全不一样只能特殊处理
                        if (_mergeinto)
                        {
                            if (tabinfo.IsAllowMergeInto)
                            {
                                List<string> lstcol = rtnlst[0].Keys.ToList();
                                TabInfo _tabtarget = sqldm.GetTabStruct(this.Table.TabName);
                                string _mergesql = sqldm.BuildMergeIntoSqlSequence(_tabtarget, lstcol);
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
                    if (IsBatchExec && !_forcepage)
                    {
                        _lstsql.Add(sb_sql.ToString());
                        sb_sql = new StringBuilder();
                    }
                    else if (_forcepage)
                    {

                        sb_pcksql.AppendLine(sb_sql.ToString());
                        sb_sql = new StringBuilder();


                        if ((p + 1) % _packunit == 0 )
                        {
                            _currbatchidx++;
                            if (_currbatchidx < _pcount)
                            {
                                _lstsql.Add(sb_pcksql.ToString());
                                sb_pcksql = new StringBuilder();
                            }
                        }
                        if (p == page - 1)
                        {
                            _currbatchidx++;
                            _lstsql.Add(sb_pcksql.ToString());
                            sb_pcksql = new StringBuilder();
                        }

                        //if (p == page - 1)
                        //{
                        //    _lstsql.Add(sb_pcksql.ToString());
                        //}
                        //if ((p+1) % _pcount == 0 && p>0)
                        //{
                        //    _lstsql.Add(sb_pcksql.ToString());
                        //    sb_pcksql = new StringBuilder();
                        //}
                    }

                }

                if (_times == this.Data.Count)
                { 
                    
                }
                string sql = sb_sql.ToString();
                //sql=DbConfig.Code_Block.Replace("[$SQL$]", sql);

                if (!IsBatchExec && !_forcepage)
                {
                    _Sql.Append(sql);
                }

                ///如果强制分包 那么按批次执行
                if (_forcepage)
                    IsBatchExec = _forcepage;

                sb_sql = new StringBuilder();

                if (_mergeinto && Context.CurrentConnectionConfig.DbType != DBType.PostGreSql)
                {
                    TabInfo _tabtarget = sqldm.GetTabStruct(this.Table.TabName);
                    TabInfo _tabsource = sqldm.Context.MCache.GetCache<TabInfo>(_cacheinsertTabName);

                    List<string> lstcol = rtnlst[0].Keys.ToList();

                    string _mergesql = sqldm.BuildMergeIntoSql(_tabtarget, _tabsource, lstcol);
                    if (this.Context.CurrentConnectionConfig.DbType == DBType.Hana 
                        || this.Context.CurrentConnectionConfig.DbType == DBType.Oracle
                        || this.Context.CurrentConnectionConfig.DbType == DBType.DaMeng
                        )
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

            var _typname = "";
            if (lstdata.Count > 0)
                _typname = lstdata[0].GetType().FullName;

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


            }
            else
                throw new Exception($"已经指定了[Insert]不允许再指定[Modi]");

            return this;
        }

        public IInsert Modi<T>(string tabname, T objdata)
        {
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
        public IInsert Modi<T>(T objdata)
        {
            _mergeinto = true;
            if (!_queue.HasQueue("insert"))
            {
                if (!_queue.HasQueue("modi"))
                {
                    //写入到临时表中
                    Insert<T>(objdata);

                    _queue.Add("modi");
                }
                else
                    throw new Exception($"已经指定了[Modi]不允许再指定[Modi]");


            }
            else
                throw new Exception($"已经指定了[Insert]不允许再指定[Modi]");

            return this;
        }
        public IInsert Modi<T>(List<T> lstdata)
        {
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
        public IInsert Modi<T>(string tabname, List<T> lstdata)
        {
            _mergeinto = true;
            if (!_queue.HasQueue("insert"))
            {
                if (!_queue.HasQueue("modi"))
                {
                    //写入到临时表中
                    Insert<T>(tabname, lstdata);

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


        /// <summary>
        /// 检测数据
        /// 包括表引用,正则检测,类型,长度
        /// </summary>
        /// <param name="hiColumns"></param>
        /// <param name="lstdata"></param>
        /// <returns></returns>
        List<Dictionary<string, string>> CheckAllData(List<HiColumn> hiColumns, List<object> lstdata)
        {
            List<Dictionary<string, string>> rtnlst = new List<Dictionary<string, string>>();
            if (this.Data != null && this.Data.Count > 0)
            {
                Type type = this.Data[0].GetType();

                Type _typ_dic = typeof(Dictionary<string, string>);
                Type _typ_dic_obj = typeof(Dictionary<string, object>);

                Dictionary<string, PropertyInfo> dicprop= new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);


                bool _isdic = type == _typ_dic || type == _typ_dic_obj;
                List<PropertyInfo> attrs = type.GetProperties().Where(p => p.MemberType == MemberTypes.Property && p.CanRead == true).ToList();

                foreach (PropertyInfo pt in attrs)
                {
                    if(!dicprop.ContainsKey(pt.Name))
                        dicprop.Add(pt.Name, pt);
                    else
                        dicprop[pt.Name]= pt;

                }

                //将有配正则校验的列去重统计
                Dictionary<string, HashSet<string>> dic_hash_reg = new Dictionary<string, HashSet<string>>();
                Dictionary<string, HashSet<string>> dic_hash_tab = new Dictionary<string, HashSet<string>>();


                var arrcol_reg = hiColumns.Where(h => !string.IsNullOrEmpty(h.Regex)).ToList();
                var arrcol_tab = hiColumns.Where(h => h.IsRefTab).ToList();

                var arrcol = hiColumns.Where(h => !string.IsNullOrEmpty(h.Regex) || h.IsRefTab).ToList();
                foreach (HiColumn hi in arrcol)
                {
                    //if(!string.IsNullOrEmpty(hi.Regex))
                    dic_hash_reg.Add(hi.FieldName, new HashSet<string>());
                }
                int _rowidx = 0;
                if (_isdic)
                {
                    //表示是字典 Dictionary<string, object>
                    string _value = "";
                    if (type == _typ_dic_obj)
                    {
                        #region Dictionary<string, object>
                        foreach (Dictionary<string, object> _o in lstdata)
                        {
                            _rowidx++;
                            //Dictionary<string, string> _dic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                            Dictionary<string, string> _rowdic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                            foreach (HiColumn hiColumn in hiColumns)
                            {
                                _value = "";
                                #region  判断必填 及自增长
                                if (hiColumn.IsRequire && !hiColumn.IsIdentity && !_o.ContainsKey(hiColumn.FieldName))
                                {
                                    throw new Exception($"行[{_rowidx}] 缺少字段[{hiColumn.FieldName}] 为必填字段");
                                }
                                if (hiColumn.IsIdentity && _o.ContainsKey(hiColumn.FieldName))
                                {
                                    _value = _o[hiColumn.FieldName].ToString();
                                    if (_value == "0" || string.IsNullOrEmpty(_value))
                                        continue;
                                    else
                                        throw new Exception($"行[{_rowidx}] 字段[{hiColumn.FieldName}] 为自增长字段 不需要外部赋值");
                                }
                                else if (hiColumn.IsIdentity)
                                    continue;
                                #endregion

                                if (_o.ContainsKey(hiColumn.FieldName))
                                {
                                    #region 将值转成string 及特殊处理
                                    if (hiColumn.FieldType.IsIn<HiType>(HiType.DATE, HiType.DATETIME))
                                    {
                                        DateTime dtime = DateTime.MinValue;
                                        if (_o[hiColumn.FieldName] != null)
                                        {
                                            
                                            dtime = Convert.ToDateTime(_o[hiColumn.FieldName]);

                                        }
                                        
                                        if (dtime != null && dtime != DateTime.MinValue)
                                        {
                                            _value = dtime.ToString("yyyy-MM-dd HH:mm:ss.fff");
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
                                    else if (_o.ContainsKey(hiColumn.FieldName))
                                    {
                                        _value = _o[hiColumn.FieldName].ToString();
                                    }
                                    else
                                    {
                                        _value = string.Empty;
                                    }
                                    #endregion

                                    #region 是否需要正则校验
                                    if (arrcol.Count>0 && arrcol.Any(h => h.FieldName == hiColumn.FieldName))
                                    {
                                        dic_hash_reg[hiColumn.FieldName].Add(_value);
                                    }




                                    #endregion

                                    #region 通用数据有效性校验
                                    var result = checkFieldValue(hiColumn, _rowidx, _value);
                                    if (result.Item1)
                                    {
                                        _rowdic.Add(hiColumn.FieldName, result.Item2);
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region 未赋值数据处理
                                    if (!hiColumn.IsNull && hiColumn.DBDefault == HiTypeDBDefault.NONE && !hiColumn.IsIdentity)
                                    {
                                        throw new Exception($"行[{_rowidx}] 字段[{hiColumn.FieldName}]不允许为空数据库中未设置默认值 且插入数据值中未指定值");
                                    }
                                    else if (Constants.IsStandardField(hiColumn.FieldName))
                                    {
                                        var result = checkFieldValue(hiColumn, _rowidx, _value);
                                        if (result.Item1)
                                        {
                                            _rowdic.Add(hiColumn.FieldName, result.Item2);
                                        }
                                    }
                                    #endregion
                                }

                            }

                            if (_rowdic.Count > 0)
                            {
                                rtnlst.Add(_rowdic);
                            }
                            else
                                throw new Exception($"行[{_rowidx}] 无可插入的字段数据");
                        }
                        #endregion
                    }
                    else
                    {
                        #region Dictionary<string, string> 
                        foreach (Dictionary<string, string> _o in lstdata)
                        {
                            _rowidx++;

                            Dictionary<string, string> _rowdic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                            foreach (HiColumn hiColumn in hiColumns)
                            {
                                _value = "";
                                #region  判断必填 及自增长
                                if (hiColumn.IsRequire && !hiColumn.IsIdentity && !_o.ContainsKey(hiColumn.FieldName))
                                {
                                    throw new Exception($"行[{_rowidx}] 缺少字段[{hiColumn.FieldName}] 为必填字段");
                                }
                                if (hiColumn.IsIdentity && _o.ContainsKey(hiColumn.FieldName))
                                {
                                    _value = _o[hiColumn.FieldName].ToString();
                                    if (_value == "0" || string.IsNullOrEmpty(_value))
                                        continue;
                                    else
                                        throw new Exception($"行[{_rowidx}] 字段[{hiColumn.FieldName}] 为自增长字段 不需要外部赋值");
                                }
                                else if (hiColumn.IsIdentity)
                                    continue;
                                #endregion

                                    if (_o.ContainsKey(hiColumn.FieldName) || _o.ContainsKey(hiColumn.FieldName.ToLower()))
                                {
                                    _value = _o[hiColumn.FieldName].ToString();

                                    #region 是否需要正则校验
                                    if (arrcol.Any(h => h.FieldName == hiColumn.FieldName))
                                    {
                                        dic_hash_reg[hiColumn.FieldName].Add(_value);
                                    }

                                    #endregion

                                    #region 通用数据有效性校验
                                    var result = checkFieldValue(hiColumn, _rowidx, _value);
                                    if (result.Item1)
                                    {
                                        _rowdic.Add(hiColumn.FieldName, result.Item2);
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region 未赋值数据处理
                                    if (!hiColumn.IsNull && hiColumn.DBDefault == HiTypeDBDefault.NONE && !hiColumn.IsIdentity)
                                    {
                                        throw new Exception($"行[{_rowidx}] 字段[{hiColumn.FieldName}]不允许为空数据库中未设置默认值 且插入数据值中未指定值");
                                    }
                                    else if (Constants.IsStandardField(hiColumn.FieldName))
                                    {
                                        var result = checkFieldValue(hiColumn, _rowidx, _value);
                                        if (result.Item1)
                                        {
                                            _rowdic.Add(hiColumn.FieldName, result.Item2);
                                        }
                                    }
                                    #endregion
                                }

                            }
                            if (_rowdic.Count > 0)
                            {
                                rtnlst.Add(_rowdic);
                            }
                            else
                                throw new Exception($"行[{_rowidx}] 无可插入的字段数据");
                        }
                        #endregion
                    }
                }
                else
                {
                    //非字典

                    #region  非字典
                    string _value = "";
                    foreach (object objdata in lstdata)
                    {
                        _rowidx++;
                        Dictionary<string, string> _dic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        Dictionary<string, string> _rowdic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        foreach (HiColumn hiColumn in hiColumns)
                        {
                            _value = "";


                            PropertyInfo objprop = null;
                            if(dicprop.ContainsKey(hiColumn.FieldName))
                                objprop=dicprop[hiColumn.FieldName];
                            #region  判断必填 及自增长
                            if (hiColumn.IsRequire && !hiColumn.IsIdentity && objprop == null)
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
                            else if (hiColumn.IsIdentity)
                            {
                                continue;
                            }
                            #endregion

                            if (objprop != null )
                            {
                                object objvalue = objprop.GetValue(objdata);
                                if (objvalue != null)
                                {
                                    #region 将值转成string 及特殊处理
                                    if (hiColumn.FieldType.IsIn<HiType>(HiType.DATE, HiType.DATETIME))
                                    {
                                        DateTime dtime = (DateTime)objvalue;
                                        if (dtime != null && dtime != DateTime.MinValue)
                                        {
                                            _dic.Add(hiColumn.FieldName, dtime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                        }
                                        else
                                            _dic.Add(hiColumn.FieldName, DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                    }
                                    else if (hiColumn.FieldType.IsIn<HiType>(HiType.BOOL))
                                    {
                                        if ((bool)objvalue == true)
                                        {
                                            _dic.Add(hiColumn.FieldName, "1");
                                        }
                                        else
                                            _dic.Add(hiColumn.FieldName, "0");
                                    }
                                    else
                                    {
                                        if (hiColumn.FieldType.IsIn<HiType>(HiType.INT))
                                        {
                                            _dic.Add(hiColumn.FieldName, ((int)objvalue).ToString());
                                        }
                                        else
                                            _dic.Add(hiColumn.FieldName, objvalue.ToString());
                                    }
                                    _value = _dic[hiColumn.FieldName];
                                    #endregion


                                    #region 是否需要正则校验
                                    if (arrcol.Count>0 && arrcol.Any(h => h.FieldName == hiColumn.FieldName))
                                    {
                                        dic_hash_reg[hiColumn.FieldName].Add(_dic[hiColumn.FieldName]);
                                    }

                                    #endregion

                                    #region 通用数据有效性校验
                                    var result = checkFieldValue(hiColumn, _rowidx, _value);
                                    if (result.Item1)
                                    {
                                        _rowdic.Add(hiColumn.FieldName, result.Item2);
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region 未赋值数据处理
                                    if (!hiColumn.IsNull && hiColumn.DBDefault == HiTypeDBDefault.NONE && !hiColumn.IsIdentity)
                                    {
                                        throw new Exception($"行[{_rowidx}] 字段[{hiColumn.FieldName}]不允许为空数据库中未设置默认值 且插入数据值中未指定值");
                                    }
                                    else if (Constants.IsStandardField(hiColumn.FieldName))
                                    {
                                        var result = checkFieldValue(hiColumn, _rowidx, "");
                                        if (result.Item1)
                                        {
                                            _rowdic.Add(hiColumn.FieldName, result.Item2);
                                        }
                                    }
                                    #endregion
                                }

                            }
                            else
                            {
                                #region 未赋值数据处理
                                if (!hiColumn.IsNull && hiColumn.DBDefault == HiTypeDBDefault.NONE && !hiColumn.IsIdentity)
                                {
                                    throw new Exception($"行[{_rowidx}] 字段[{hiColumn.FieldName}]不允许为空数据库中未设置默认值 且插入数据值中未指定值");
                                }
                                else if (Constants.IsStandardField(hiColumn.FieldName))
                                {
                                    var result = checkFieldValue(hiColumn, _rowidx, "");
                                    if (result.Item1)
                                    {
                                        _rowdic.Add(hiColumn.FieldName, result.Item2);
                                    }
                                }
                                #endregion
                            }


                        }


                        if (_rowdic.Count > 0)
                        {
                            rtnlst.Add(_rowdic);
                        }
                        else
                            throw new Exception($"行[{_rowidx}] 无可插入的字段数据");


                    }

                    #endregion
                }


                #region 正则校验匹配 是否合法
                if (arrcol_reg.Count > 0)
                {
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
                }

                //表关联配置校验
                if (arrcol_tab.Count > 0)
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
                return rtnlst;
            return rtnlst;
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
                    //当为max 时hiColumn.FieldLen == -1
                    if (_value.Length > hiColumn.FieldLen && hiColumn.FieldLen>0)
                    {
                        throw new Exception($"字段[{hiColumn.FieldName}]的值[{_value}]超过了限制长度[{hiColumn.FieldLen}] 无法数据提交");
                    }
                    if (hiColumn.IsRequire)
                    {
                        if (string.IsNullOrEmpty(_value.Trim()))
                            throw new Exception($"字段[{hiColumn.FieldName}] 为必填 无法数据提交");
                    }

                    _value = $"'{_value.ToSqlInject()}'";
                    rtn = new Tuple<bool, string>(true, _value);

                }
                else if (hiColumn.FieldType.IsIn<HiType>(HiType.VARCHAR, HiType.CHAR, HiType.TEXT))
                {
                    //中文按两个字符计算
                    //当为max 时hiColumn.FieldLen == -1
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



        Dictionary<string, string> CheckInsertData(bool isDic, List<PropertyInfo> attrs, List<HiColumn> hiColumns, object objdata)
        {
            Dictionary<string, string> _values = new Dictionary<string, string>();

            var arrcol_reg = hiColumns.Where(h => string.IsNullOrEmpty(h.Regex));
            var arrcol_tab = hiColumns.Where(h => h.IsRefTab);


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

            string _value;
            foreach (HiColumn hiColumn in hiColumns)
            {
                if (!isDic)
                {
                    #region 匿名类或实体类的检测
                    var objprop = attrs.Where(p => p.Name.ToLower() == hiColumn.FieldName.ToLower()).FirstOrDefault();
                    if (hiColumn.IsRequire && !hiColumn.IsIdentity && objprop == null)
                    {
                        throw new Exception($"字段[{hiColumn.FieldName}] 为必填 无法数据提交");
                    }
                    if (hiColumn.IsIdentity && objprop != null)
                    {
                        _value = objprop.GetValue(objdata).ToString();
                        if (_value == "0")
                            continue;
                        else
                            throw new Exception($"字段[{hiColumn.FieldName}] 为自增长字段 不需要外部赋值");
                    }

                    if (objprop != null && objprop.GetValue(objdata) != null)
                    {
                        if (hiColumn.FieldType.IsIn<HiType>(HiType.NVARCHAR, HiType.NCHAR, HiType.GUID))
                        {
                            //中文按1个字符计算
                            //_value = "test";

                            _value = (string)objprop.GetValue(objdata);
                            if (_value.Length > hiColumn.FieldLen && hiColumn.FieldLen>0)
                            {
                                //sqlserver 的类型定义为varchar(max)
                                if (hiColumn.FieldLen >= 0)
                                    throw new Exception($"字段[{objprop.Name}]的值[{_value}]超过了限制长度[{hiColumn.FieldLen}] 无法数据提交");
                            }
                            if (hiColumn.IsRequire)
                            {
                                if (string.IsNullOrEmpty(_value.Trim()))
                                    throw new Exception($"字段[{objprop.Name}] 为必填 无法数据提交");
                            }

                            _values.Add(hiColumn.FieldName, $"'{_value.ToSqlInject()}'");

                        }
                        else if (hiColumn.FieldType.IsIn<HiType>(HiType.VARCHAR, HiType.CHAR, HiType.TEXT))
                        {

                            //中文按两个字符计算
                            _value = (string)objprop.GetValue(objdata);
                            //_value = "test";
                            if (_value.LengthZH() > hiColumn.FieldLen &&  hiColumn.FieldLen > 0)
                            {
                                throw new Exception($"字段[{objprop.Name}]的值[{_value}]超过了限制长度[{hiColumn.FieldLen}] 无法数据提交");
                            }
                            if (hiColumn.IsRequire)
                            {
                                if (string.IsNullOrEmpty(_value.Trim()))
                                    throw new Exception($"字段[{objprop.Name}] 为必填 无法数据提交");
                            }

                            _values.Add(hiColumn.FieldName, $"'{_value.ToSqlInject()}'");
                        }
                        else if (hiColumn.FieldType.IsIn<HiType>(HiType.INT, HiType.BIGINT, HiType.DECIMAL, HiType.SMALLINT))
                        {

                            _value = objprop.GetValue(objdata).ToString();
                            if (!Tool.IsDecimal(_value))
                            {
                                throw new Exception($"字段[{hiColumn.FieldName}]的值[{_value}]非数字,不符合规范");
                            }
                            //_value = "1";


                            _values.Add(hiColumn.FieldName, $"{_value}");
                        }
                        else if (hiColumn.FieldType.IsIn<HiType>(HiType.DATE, HiType.DATETIME))
                        {

                            DateTime dtime = (DateTime)objprop.GetValue(objdata);
                            //DateTime dtime = DateTime.Now;
                            if (dtime != null && dtime != DateTime.MinValue)
                            {
                                _values.Add(hiColumn.FieldName, $"'{dtime.ToString("yyyy-MM-dd HH:mm:ss.fff")}'");
                            }


                        }
                        else if (hiColumn.FieldType.IsIn<HiType>(HiType.BOOL)) //add by tgm date:2021.10.27
                        {
                            if ((bool)objprop.GetValue(objdata) == true)
                            {
                                if (Context.CurrentConnectionConfig.DbType.IsIn<DBType>(DBType.PostGreSql, DBType.Hana, DBType.MySql))
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
                                if (Context.CurrentConnectionConfig.DbType.IsIn<DBType>(DBType.PostGreSql, DBType.Hana, DBType.MySql))
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
                            _values.Add(hiColumn.FieldName, $"'{_value}'");
                        }
                    }
                    else
                    {
                        //非自增长且不允许为空且没有设置默认值且没有传值的情况下应该抛出异常 不然底层库会报错
                        if (!hiColumn.IsNull && hiColumn.DBDefault == HiTypeDBDefault.NONE && !hiColumn.IsIdentity)
                        {
                            throw new Exception($"字段[{hiColumn.FieldName}]不允许为空数据库中未设置默认值 且插入数据值中未指定值");
                        }
                    }

                    if (hiColumn.FieldName == "CreateTime" || hiColumn.FieldName == "ModiTime")
                    {


                        if (hiColumn.DBDefault != HiTypeDBDefault.FUNDATE)
                        {
                            if (!_values.ContainsKey(hiColumn.FieldName))
                                _values.Add(hiColumn.FieldName, $"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}'");
                            else
                                _values[hiColumn.FieldName] = $"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}'";
                        }

                    }
                    else if (hiColumn.FieldName == "CreateName" || hiColumn.FieldName == "ModiName")
                    {
                        if (!_values.ContainsKey(hiColumn.FieldName))
                            _values.Add(hiColumn.FieldName, $"'{Context.CurrentConnectionConfig.User}'");
                        else
                            _values[hiColumn.FieldName] = $"'{Context.CurrentConnectionConfig.User}'";
                    }
                    #endregion
                }
                else
                {

                    #region 字典数据
                    if (hiColumn.IsRequire && !hiColumn.IsIdentity && !_dic.ContainsKey(hiColumn.FieldName))
                    {
                        throw new Exception($"字段[{hiColumn.FieldName}] 为必填 无法数据提交");
                    }
                    if (hiColumn.IsIdentity && _dic.ContainsKey(hiColumn.FieldName))
                    {
                        _value = _dic[hiColumn.FieldName].ToString();
                        if (_value == "0" || string.IsNullOrEmpty(_value))
                            continue;
                        else
                            throw new Exception($"字段[{hiColumn.FieldName}] 为自增长字段 不需要外部赋值");
                    }
                    if (_dic.ContainsKey(hiColumn.FieldName))
                    {
                        if (hiColumn.FieldType.IsIn<HiType>(HiType.NVARCHAR, HiType.NCHAR, HiType.GUID))
                        {
                            //中文按1个字符计算
                            //_value = "test";
                            _value = _dic[hiColumn.FieldName].ToString();
                            if (_value.Length > hiColumn.FieldLen && hiColumn.FieldLen>0)
                            {
                                throw new Exception($"字段[{hiColumn.FieldName}]的值[{_value}]超过了限制长度[{hiColumn.FieldLen}] 无法数据提交");
                            }
                            if (hiColumn.IsRequire)
                            {
                                if (string.IsNullOrEmpty(_value.Trim()))
                                    throw new Exception($"字段[{hiColumn.FieldName}] 为必填 无法数据提交");
                            }

                            _values.Add(hiColumn.FieldName, $"'{_value.ToSqlInject()}'");

                        }
                        else if (hiColumn.FieldType.IsIn<HiType>(HiType.VARCHAR, HiType.CHAR, HiType.TEXT))
                        {
                            //中文按两个字符计算
                            _value = _dic[hiColumn.FieldName].ToString();
                            //_value = "test";
                            if (_value.LengthZH() > hiColumn.FieldLen && hiColumn.FieldLen>0)
                            {
                                throw new Exception($"字段[{hiColumn.FieldName}]的值[{_value}]超过了限制长度[{hiColumn.FieldLen}] 无法数据提交");
                            }
                            if (hiColumn.IsRequire)
                            {
                                if (string.IsNullOrEmpty(_value.Trim()))
                                    throw new Exception($"字段[{hiColumn.FieldName}] 为必填 无法数据提交");
                            }
                            _values.Add(hiColumn.FieldName, $"'{_value.ToSqlInject()}'");
                        }
                        else if (hiColumn.FieldType.IsIn<HiType>(HiType.INT, HiType.BIGINT, HiType.DECIMAL, HiType.SMALLINT))
                        {
                            _value = _dic[hiColumn.FieldName].ToString();
                            //_value = "1";
                            _values.Add(hiColumn.FieldName, $"{_value}");
                        }
                        else if (hiColumn.FieldType.IsIn<HiType>(HiType.DATE, HiType.DATETIME))
                        {
                            DateTime dtime = DateTime.Parse(_dic[hiColumn.FieldName].ToString());
                            //DateTime dtime = DateTime.Now;
                            if (dtime != null)
                            {
                                _values.Add(hiColumn.FieldName, $"'{dtime.ToString("yyyy-MM-dd HH:mm:ss.fff")}'");
                            }
                        }
                        else
                        {
                            _value = _dic[hiColumn.FieldName].ToString();
                            _values.Add(hiColumn.FieldName, $"'{_value}'");
                        }
                    }
                    else
                    {
                        //非自增长且不允许为空且没有设置默认值且没有传值的情况下应该抛出异常 不然底层库会报错
                        if (!hiColumn.IsNull && hiColumn.DBDefault == HiTypeDBDefault.NONE && !hiColumn.IsIdentity)
                        {
                            throw new Exception($"字段[{hiColumn.FieldName}]不允许为空数据库中未设置默认值 且插入数据值中未指定值");
                        }
                    }
                    if (hiColumn.FieldName.ToLower() == "CreateTime".ToLower() || hiColumn.FieldName.ToLower() == "ModiTime".ToLower())
                    {

                        if (hiColumn.DBDefault != HiTypeDBDefault.FUNDATE)
                        {
                            if (!_values.ContainsKey(hiColumn.FieldName))
                                _values.Add(hiColumn.FieldName, $"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}'");
                            else
                                _values[hiColumn.FieldName] = $"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}'";
                        }

                    }
                    else if (hiColumn.FieldName.ToLower() == "CreateName".ToLower() || hiColumn.FieldName.ToLower() == "ModiName".ToLower())
                    {
                        if (!_values.ContainsKey(hiColumn.FieldName))
                        {
                            _values.Add(hiColumn.FieldName, $"'{Context.CurrentConnectionConfig.User}'");
                        }
                        else
                            _values[hiColumn.FieldName] = $"'{Context.CurrentConnectionConfig.User}'";


                    }
                    #endregion
                }
            }




            return _values;
        }

        #endregion
    }
}
