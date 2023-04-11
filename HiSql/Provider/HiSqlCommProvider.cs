using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Data;
using System.Net.Http.Headers;
using System.Threading;

namespace HiSql
{

    /// <summary>
    /// 公共类
    /// </summary>
    public static partial class HiSqlCommProvider
    {
        /// <summary>
        /// 映射表结构
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void InitMapping<T>(List<string> keylst)
        {
            InitMapping(typeof(T));
        }


        public static void InitMaping(TableDefinition table)
        {
            string _keyname = Constants.KEY_TABLE_CACHE_NAME.Replace("[$DBSERVER$]", table.DbServer)
                .Replace("[$SCHEMA$]", table.Schema)
                .Replace("[$TABLE$]", table.TabName)
                ;
        }

        /// <summary>
        /// 初始化表结构缓存
        /// </summary>
        /// <param name="keyname"></param>
        /// <param name="GetInfo"></param>
        /// <returns></returns>
        public static TabInfo InitTabMaping(string keyname, Func<TabInfo> GetInfo)
        {
            string _keyname = keyname;
            int _waitseconds = 20;//等待时间单位秒
            //string _keyname = Constants.KEY_TABLE_CACHE_NAME.Replace("[$TABLE$]", keyname.ToLower());
            TabInfo tableInfo = null;
            bool locked = false;
            var lckinfo = new LckInfo() { UName = "hisql", EventName = "InitTabMaping" };
            try
            {
                tableInfo = CacheContext.MCache.GetCache<TabInfo>(_keyname);

                if (tableInfo == null)
                {
                    //var lockResult2 = CacheContext.MCache.LockOn(_keyname, lckinfo, 60, 60);

                    
                    var lockResult2 = CacheContext.MCache.LockOnExecute(keyname, () => {
                        tableInfo = GetInfo();
                        CacheContext.MCache.SetCache(_keyname, tableInfo);
                    }, lckinfo);
                    if (!lockResult2.Item1 )
                    {
                        Thread.Sleep(_waitseconds * 1000);
                        tableInfo = CacheContext.MCache.GetCache<TabInfo>(_keyname);
                        if (tableInfo == null)
                        {
                            throw new Exception($"在等待获取表结构信息时超时[{_waitseconds * 1000}]秒");
                        }
                    }
                    else
                        throw new Exception("InitTabMaping获取表结构信息因为未获取到独占锁，无法创建并获取表信息");
                    

                    //var lockResult = CacheContext.MCache.LockOn(_keyname, lckinfo, 60, 60);
                    //if (lockResult.Item1) //加锁成功
                    //{
                    //    locked = true;
                    //    tableInfo = CacheContext.MCache.GetCache<TabInfo>(_keyname);
                    //    if (tableInfo == null)
                    //    {
                    //        tableInfo = GetInfo();
                    //        CacheContext.MCache.SetCache(_keyname, tableInfo);
                    //    }
                    //    CacheContext.MCache.UnLock(lckinfo, _keyname);
                    //    locked = false;
                    //}
                    //else
                    //{
                    //    tableInfo = CacheContext.MCache.GetCache<TabInfo>(_keyname);
                    //    if (tableInfo == null)
                    //    {
                    //        throw new Exception("InitTabMaping获取表结构信息因为未获取到独占锁，无法创建并获取表信息");
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                tableInfo = CacheContext.MCache.GetCache<TabInfo>(_keyname);
            }
            finally
            {
                if (locked) { CacheContext.MCache.UnLock(lckinfo, _keyname); }

            }
            if (tableInfo != null)
            {
                return tableInfo;
            }
            else
            {
                throw new Exception("InitTabMaping获取表结构信息因为未获取到独占锁，无法创建并获取表信息");
            }

            //以下这种方式可能导至两个请求同时执行
            //return CacheContext.MCache.GetOrCreate<TabInfo>(_keyname, () =>
            //{
            //    try
            //    {
            //        return GetInfo();
            //    }
            //    catch (Exception)
            //    {

            //        throw;
            //    }

            //});
        }

        /// <summary>
        /// 锁定表对象，执行操作
        /// </summary>
        /// <param name="tabname"></param>
        /// <param name="action"></param>
        public static void LockTableExecAction(string tabname, Action  action)
        {
            string _keyname = Constants.KEY_TABLE_CACHE_NAME.Replace("[$TABLE$]", tabname);
            

            bool locked = false;
            var lckinfo = new LckInfo() { UName = "hisql", EventName = "InitTabMaping" };
            try
            {
                if (CacheContext.MCache.CheckLock(_keyname).Item1)
                {
                    return;
                }
                var lockResult = CacheContext.MCache.LockOn(_keyname, lckinfo, 160, 3);
                if (lockResult.Item1) //加锁成功
                {
                    locked = true;
                    if (action != null)
                    {
                        action();
                    }
                }
                else  //直接返回
                {
                   
                }
            }
            catch (Exception ex)
            {
                
            }
            finally
            {
                if (locked) { CacheContext.MCache.UnLock(lckinfo, _keyname); }
            }
        }
        /// <summary>
        /// 移除表结构缓存信息
        /// </summary>
        /// <param name="tabname"></param>
        public static void RemoveTabInfoCache(string tabname,DBType dBType)
        {
            string _keyname = Constants.KEY_TABLE_CACHE_NAME.Replace("[$TABLE$]", tabname.ToLower()).Replace("[$DbType$]", dBType.ToString());
            if (CacheContext.MCache.Exists(_keyname))
                CacheContext.MCache.RemoveCache(_keyname);

        }




        /// <summary>
        /// 将数据库类型转成HiSql的类型
        /// </summary>
        /// <param name="_dbmapping"></param>
        /// <param name="fieldtype"></param>
        /// <returns></returns>
        public static HiType ConvertToHiType(Dictionary<HiType, string> _dbmapping, string fieldtype)
        {
            foreach (HiType ht in _dbmapping.Keys)
            {
                if (_dbmapping[ht] == fieldtype)
                {
                    return ht;
                }
            }
            return HiType.NVARCHAR;
        }

        public static TabInfo TabDefinitionToEntity(DataTable table, Dictionary<HiType, string> _dbmapping)
        {
            TabInfo tabInfo = null;
            if (table.Columns.Contains("TabType") && table.Columns.Contains("TabName") && table.Columns.Contains("FieldNo") && table.Columns.Contains("FieldName")
                 && table.Columns.Contains("IsIdentity") && table.Columns.Contains("IsPrimary") && table.Columns.Contains("FieldType")
                 && table.Columns.Contains("UseBytes") && table.Columns.Contains("Lens") && table.Columns.Contains("PointDec") && table.Columns.Contains("IsNull")
                 && table.Columns.Contains("DbDefault") && table.Columns.Contains("FieldDesc")
                 && table.Rows.Count > 0
                )
            {
                tabInfo = new TabInfo();
                HiTable hiTable = new HiTable();

                hiTable.TabName = table.Rows[0]["TabName"].ToString().Trim();
                hiTable.TabReName = hiTable.TabName;
                hiTable.TabStatus = TabStatus.Use;
                hiTable.TabType = table.Rows[0]["TabType"].ToString().Trim() == "View" ? TabType.View : TabType.Business;
                hiTable.IsEdit = true;

                tabInfo.TabModel = hiTable;

                foreach (DataRow drow in table.Rows)
                {
                    HiColumn hiColumn = new HiColumn();
                    hiColumn.FieldName = drow["FieldName"].ToString().Trim();
                    //hiColumn.FieldType
                    hiColumn.IsPrimary = drow["IsPrimary"].ToString().Trim().IsIn<string>("1", "True") ? true : false;
                    hiColumn.IsIdentity = drow["IsIdentity"].ToString().Trim().IsIn<string>("1", "True") ? true : false;
                    hiColumn.IsBllKey = hiColumn.IsPrimary;
                    hiColumn.SortNum = Convert.ToInt32(drow["FieldNo"].ToString().Trim());
                    hiColumn.FieldType = ConvertToHiType(_dbmapping, drow["FieldType"].ToString().ToLower().Trim());

                    hiColumn.FieldLen = Convert.ToInt32(string.IsNullOrEmpty(drow["Lens"].ToString().Trim()) ? "0" : drow["Lens"].ToString().Trim());

                    if (hiColumn.FieldType.IsIn<HiType>(HiType.DECIMAL))
                    {
                        hiColumn.FieldDec = Convert.ToInt32(string.IsNullOrEmpty(drow["PointDec"].ToString().Trim()) ? "0" : drow["PointDec"].ToString().Trim());
                    }
                    hiColumn.IsNull = drow["IsNull"].ToString().Trim().IsIn<string>("1", "True") ? true : false;
                    hiColumn.IsShow = true;
                    hiColumn.SrchMode = SrchMode.Single;
                    hiColumn.IsSys = false;
                    hiColumn.FieldDesc = drow["FieldDesc"].ToString().Trim();
                    //默认值未适配数据库类型 需要调整
                    switch ((drow["DbDefault"].ToString().Trim()))
                    {
                        case "('')":
                            hiColumn.DBDefault = HiTypeDBDefault.EMPTY;
                            break;
                        case "":
                            hiColumn.DBDefault = HiTypeDBDefault.EMPTY;
                            break;
                        case "((0))":
                            hiColumn.DBDefault = HiTypeDBDefault.EMPTY;
                            break;
                        case "0":
                            hiColumn.DBDefault = HiTypeDBDefault.EMPTY;
                            break;
                        case "(getdate())":
                            hiColumn.DBDefault = HiTypeDBDefault.FUNDATE;
                            break;
                        case "(newid())":
                            hiColumn.DBDefault = HiTypeDBDefault.FUNGUID;
                            break;
                        default:
                            break;
                    }
                    tabInfo.Columns.Add(hiColumn);

                }
                return tabInfo;

            }
            else
                throw new Exception($"获取的物理表结构信息不符合规范");


            //return tabInfo;
        }


        public static TabInfo TabMerge(TabInfo phytabInfo, TabInfo tabInfo)
        {
            //List<FieldChange> fieldChanges = TabToCompare(phytabInfo, tabInfo);

            List<HiColumn> phyhiColumns = phytabInfo.GetColumns;
            List<HiColumn> newColumns = new List<HiColumn>();
            List<HiColumn> hiColumns = tabInfo.GetColumns;
            foreach (HiColumn hiColumn in phyhiColumns)
            {
                var column = hiColumns.Where(p => p.FieldName.ToLower() == hiColumn.FieldName.ToLower()).FirstOrDefault();
                if (column != null)
                {
                    hiColumn.Regex = column.Regex;
                    hiColumn.IsRefTab = column.IsRefTab;
                    hiColumn.RefTab = column.RefTab;
                    hiColumn.RefField = column.RefField;
                    hiColumn.RefFields = column.RefFields;
                    hiColumn.RefFieldDesc = column.RefFieldDesc;


                    hiColumn.IsRequire = column.IsRequire;
                    hiColumn.IsShow = column.IsShow;
                    hiColumn.IsSearch = column.IsSearch;


                    hiColumn.SNO = column.SNO;
                    hiColumn.SNO_NUM = column.SNO_NUM;


                    hiColumn.SortNum = column.SortNum;

                    //如果有扩展信息需要在此处赋值

                }
                newColumns.Add(hiColumn);
            }
            phytabInfo.Columns = newColumns;
            return phytabInfo;
        }


        /// <summary>
        /// 物理表的结构信息与表结构表的数据进行对比
        /// </summary>
        /// <param name="phytabInfo">物理表结构信息</param>
        /// <param name="tabInfo">表结构信息</param>
        /// <returns></returns>
        public static List<FieldChange> TabToCompare(TabInfo phytabInfo, TabInfo tabInfo, DBType dbtype)
        {
            List<FieldChange> fieldChanges = new List<FieldChange>();
            var phycolumns = phytabInfo.GetColumns;
            var columns = tabInfo.GetColumns;

            //以物理表为基准匹配
            foreach (HiColumn _newcolumn in phycolumns)
            {
                var _oldcolumn = columns.Where(h => h.FieldName.ToLower() == _newcolumn.FieldName.ToLower()).FirstOrDefault();
                if (_oldcolumn != null && _newcolumn.FieldName.ToLower().Equals(_newcolumn.ReFieldName.ToLower()))
                {

                    //可能变更也可能没有变更

                    if (string.IsNullOrEmpty(_newcolumn.TabName))
                        _newcolumn.TabName = _oldcolumn.TabName;

                    var rtntuple = ClassExtensions.CompareTabProperties(_newcolumn, _oldcolumn, dbtype);

                    //if (!ClassExtensions.CompareProperties(column, _column))
                    //{
                    //    //说明有变更
                    //    fieldChanges.Add(new FieldChange { OldColumn=_column,NewColumn=column,  FieldName = column.FieldName, Action = TabFieldAction.MODI });
                    //}
                    //else
                    //{
                    //    //无变更
                    //    fieldChanges.Add(new FieldChange { FieldName = column.FieldName, Action = TabFieldAction.NONE });
                    //}

                    if (!rtntuple.Item1)
                    {
                        if (dbtype == DBType.DaMeng)
                        {

                            #region 达梦特殊业务处理
                            //达梦数据没有nvarchar的概念
                            FieldChangeDetail fieldlen = rtntuple.Item3.Where(fc => fc.AttrName.Equals("FieldLen", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                            FieldChangeDetail fieldtype = rtntuple.Item3.Where(fc => (fc.AttrName.Equals("FieldType", StringComparison.OrdinalIgnoreCase) 
                            && fc.ValueA.Equals("varchar", StringComparison.OrdinalIgnoreCase)
                            && fc.ValueB.Equals("nvarchar", StringComparison.OrdinalIgnoreCase)) || (fc.AttrName.Equals("FieldType", StringComparison.OrdinalIgnoreCase)
                            && fc.ValueA.Equals("char", StringComparison.OrdinalIgnoreCase)
                            && fc.ValueB.Equals("nchar", StringComparison.OrdinalIgnoreCase))
                            ).FirstOrDefault();
                            if (fieldtype != null && fieldlen != null)
                            {
                                int _va = int.Parse(fieldlen.ValueA);
                                int _vb = int.Parse(fieldlen.ValueB);
                                _newcolumn.FieldType = _oldcolumn.FieldType;
                                if (_va / 2 == _vb && _va % 2 == 0)
                                {
                                    //表结构无变化 
                                    _newcolumn.FieldLen = _vb;
                                }
                                else
                                {
                                    _newcolumn.FieldLen = _va / 2;
                                    fieldChanges.Add(new FieldChange { IsTabChange = rtntuple.Item2, OldColumn = _oldcolumn, NewColumn = _newcolumn, FieldName = _newcolumn.FieldName, Action = TabFieldAction.MODI, ChangeDetail = rtntuple.Item3 });

                                }
                            }
                            else
                            {
                                if (rtntuple.Item3.Count == 1 && rtntuple.Item3.Where(fc => fc.AttrName.Equals("DBDefault", StringComparison.OrdinalIgnoreCase)
                                && (
                                    fc.ValueA.Equals("EMPTY", StringComparison.OrdinalIgnoreCase) || fc.ValueA.Equals("VALUE", StringComparison.OrdinalIgnoreCase)
                                    ||
                                    fc.ValueB.Equals("EMPTY", StringComparison.OrdinalIgnoreCase) || fc.ValueB.Equals("VALUE", StringComparison.OrdinalIgnoreCase)
                                    )
                                ).Count() > 0)
                                {
                                    //达梦数据库忽略此种差异
                                }
                                else
                                    fieldChanges.Add(new FieldChange { IsTabChange = true, OldColumn = _oldcolumn, NewColumn = _newcolumn, FieldName = _newcolumn.FieldName, Action = TabFieldAction.MODI, ChangeDetail = rtntuple.Item3 });
                            }
                            #endregion


                        }
                        else if (dbtype == DBType.Oracle  || dbtype==DBType.Hana)
                        {

                            //oralce 和hana 的char 一个中文占3个字符 其它库占2位 
                            decimal _lenxs = 1.5M;

                            FieldChangeDetail fieldlen = rtntuple.Item3.Where(fc => fc.AttrName.Equals("FieldLen", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                            if (fieldlen != null)
                            {
                                int _va = int.Parse(fieldlen.ValueA);
                                int _vb = int.Parse(fieldlen.ValueB);


                                decimal _deca = Convert.ToDecimal(_va);
                                decimal _decb = _lenxs * _vb;
                                if (_deca != _decb)
                                {
                                    int len = Convert.ToInt32(Math.Floor(_deca / _lenxs));
                                    if (len!=_vb)
                                        fieldChanges.Add(new FieldChange { IsTabChange = rtntuple.Item2, OldColumn = _oldcolumn, NewColumn = _newcolumn, FieldName = _newcolumn.FieldName, Action = TabFieldAction.MODI, ChangeDetail = rtntuple.Item3 });

                                }
                                



                            }
                            else
                            {
                                if (rtntuple.Item3.Count == 1 && rtntuple.Item3.Where(fc => fc.AttrName.Equals("DBDefault", StringComparison.OrdinalIgnoreCase)
                                && (
                                    fc.ValueA.Equals("EMPTY", StringComparison.OrdinalIgnoreCase) || fc.ValueA.Equals("VALUE", StringComparison.OrdinalIgnoreCase)
                                    ||
                                    fc.ValueB.Equals("EMPTY", StringComparison.OrdinalIgnoreCase) || fc.ValueB.Equals("VALUE", StringComparison.OrdinalIgnoreCase)
                                    )
                                ).Count() > 0)
                                {

                                }
                                else
                                    fieldChanges.Add(new FieldChange { IsTabChange = rtntuple.Item2, OldColumn = _oldcolumn, NewColumn = _newcolumn, FieldName = _newcolumn.FieldName, Action = TabFieldAction.MODI, ChangeDetail = rtntuple.Item3 });
                            }
                            

                            //FieldChangeDetail fieldtype = rtntuple.Item3.Where(fc => (fc.AttrName.Equals("FieldType", StringComparison.OrdinalIgnoreCase)
                            //&& fc.ValueA.Equals("varchar", StringComparison.OrdinalIgnoreCase)
                            //&& fc.ValueB.Equals("nvarchar", StringComparison.OrdinalIgnoreCase)) || 
                            //(fc.AttrName.Equals("FieldType", StringComparison.OrdinalIgnoreCase)
                            //&& fc.ValueA.Equals("char", StringComparison.OrdinalIgnoreCase)
                            //&& fc.ValueB.Equals("nchar", StringComparison.OrdinalIgnoreCase))
                            //).FirstOrDefault();

                        }
                        else
                        {

                            //不需要特殊处理

                            if (rtntuple.Item3.Count == 1 && rtntuple.Item3.Where(fc => fc.AttrName.Equals("DBDefault", StringComparison.OrdinalIgnoreCase)
                                && (
                                    fc.ValueA.Equals("EMPTY", StringComparison.OrdinalIgnoreCase) || fc.ValueA.Equals("VALUE", StringComparison.OrdinalIgnoreCase)
                                    ||
                                    fc.ValueB.Equals("EMPTY", StringComparison.OrdinalIgnoreCase) || fc.ValueB.Equals("VALUE", StringComparison.OrdinalIgnoreCase)
                                    )
                                ).Count() > 0)
                            {

                            }
                            else
                                fieldChanges.Add(new FieldChange { IsTabChange = rtntuple.Item2, OldColumn = _oldcolumn, NewColumn = _newcolumn, FieldName = _newcolumn.FieldName, Action = TabFieldAction.MODI, ChangeDetail = rtntuple.Item3 });
                        }
                    }
                }
                else
                {
                    //说明是有新增字段或重命名字段

                    if (!_newcolumn.FieldName.ToLower().Equals(_newcolumn.ReFieldName.ToLower()) && !string.IsNullOrEmpty(_newcolumn.ReFieldName))
                    {
                        //重命名字段
                        fieldChanges.Add(new FieldChange { IsTabChange = true, NewColumn = _newcolumn, FieldName = _newcolumn.FieldName, Action = TabFieldAction.RENAME });
                    }
                    else
                        fieldChanges.Add(new FieldChange { IsTabChange = true, NewColumn = _newcolumn, FieldName = _newcolumn.FieldName, Action = TabFieldAction.ADD });
                }
            }

            foreach (HiColumn column in columns)
            {
                var _column = phycolumns.Where(h => h.FieldName.ToLower() == column.FieldName.ToLower()).FirstOrDefault();
                if (_column == null)
                {
                    //说明该字段是删除
                    fieldChanges.Add(new FieldChange { IsTabChange = true, OldColumn = column, FieldName = column.FieldName, Action = TabFieldAction.DELETE });
                }

            }



            return fieldChanges;
        }

        public static TabInfo TabToEntity(DataSet tabset)
        {
            TabInfo tabInfo = null;

            if (tabset.Tables[Constants.HiSysTable["Hi_FieldModel"].ToString()] != null && tabset.Tables[Constants.HiSysTable["Hi_TabModel"].ToString()] != null
                && tabset.Tables[Constants.HiSysTable["Hi_FieldModel"]].Rows.Count > 0 && tabset.Tables[Constants.HiSysTable["Hi_TabModel"].ToString()].Rows.Count > 0
                )
            {
                DataTable tabModel = tabset.Tables[Constants.HiSysTable["Hi_TabModel"]];
                DataTable fieldModel = tabset.Tables[Constants.HiSysTable["Hi_FieldModel"]];
                Type typ_table = typeof(HiTable);
                Type typ_column = typeof(HiColumn);
                HiTable hiTable = new HiTable();
                tabInfo = new TabInfo();
                List<HiColumn> hiColumns = new List<HiColumn>();

                var props = typ_table.GetProperties().Where(p => p.CanRead && p.CanWrite && p.MemberType == MemberTypes.Property).ToList();
                foreach (PropertyInfo prop in props)
                {
                    if (tabModel.Columns.Contains(prop.Name))
                    {
                        //oracleclient 返回的值不标准所以需要进行特殊处理
                        switch (prop.PropertyType.Name)
                        {
                            case "TabStoreType":
                                prop.SetValue(hiTable, (TabStoreType)Convert.ToInt32(tabModel.Rows[0][prop.Name].ToString()));
                                break;
                            case "TabType":
                                prop.SetValue(hiTable, (TabType)Convert.ToInt32(tabModel.Rows[0][prop.Name].ToString()));
                                break;
                            case "TabCacheType":
                                prop.SetValue(hiTable, (TabCacheType)Convert.ToInt32(tabModel.Rows[0][prop.Name].ToString()));
                                break;
                            case "TabStatus":
                                prop.SetValue(hiTable, (TabStatus)Convert.ToInt32(tabModel.Rows[0][prop.Name].ToString()));
                                break;
                            default:
                                if (prop.PropertyType.Name == "Boolean" && (tabModel.Rows[0][prop.Name].GetType().Name.ToLower().IndexOf("int") >= 0 || tabModel.Rows[0][prop.Name].GetType().Name.ToLower().IndexOf("decimal") >= 0))
                                {
                                    if (tabModel.Rows[0][prop.Name].ToString() == "1")
                                        prop.SetValue(hiTable, true);
                                    else
                                        prop.SetValue(hiTable, false);
                                }
                                else
                                {
                                    if (tabModel.Rows[0][prop.Name].GetType().Name.ToLower().IndexOf("dbnull") < 0)
                                    {
                                        if (prop.PropertyType.Name == "Int32")
                                        {
                                            prop.SetValue(hiTable, Convert.ToInt32(tabModel.Rows[0][prop.Name].ToString()));
                                        }
                                        else if (prop.PropertyType.Name == "Int64")
                                        {
                                            prop.SetValue(hiTable, Convert.ToInt64(tabModel.Rows[0][prop.Name].ToString()));
                                        }
                                        else if (prop.PropertyType.Name == "Int16")
                                        {
                                            prop.SetValue(hiTable, Convert.ToInt16(tabModel.Rows[0][prop.Name].ToString()));
                                        }
                                        else
                                            prop.SetValue(hiTable, tabModel.Rows[0][prop.Name]);
                                    }

                                }
                                break;
                        }

                    }
                    else
                    {
                        //throw new Exception($"在表[{Constants.HiSysTable["Hi_TabModel"]}]未找到字段[{prop.Name}] 请检查是否版本需要升级");
                    }
                }
                tabInfo.TabModel = hiTable;

                var props2 = typ_column.GetProperties().Where(p => p.CanRead && p.CanWrite && p.MemberType == MemberTypes.Property).ToList();


                foreach (DataRow drow in tabset.Tables[Constants.HiSysTable["Hi_FieldModel"]].Rows)
                {
                    HiColumn hiColumn = new HiColumn();
                    foreach (PropertyInfo prop in props2)
                    {
                        if (fieldModel.Columns.Contains(prop.Name))
                        {
                            switch (prop.PropertyType.Name)
                            {

                                case "HiType":
                                    prop.SetValue(hiColumn, (HiType)Convert.ToInt32(drow[prop.Name].ToString()));
                                    break;
                                case "HiTypeDBDefault":
                                    prop.SetValue(hiColumn, (HiTypeDBDefault)Convert.ToInt32(drow[prop.Name].ToString()));
                                    break;
                                case "SrchMode":
                                    prop.SetValue(hiColumn, (SrchMode)Convert.ToInt32(drow[prop.Name].ToString()));
                                    break;

                                default:
                                    if (prop.PropertyType.Name == "Boolean" && (drow[prop.Name].GetType().Name.ToLower().IndexOf("int") >= 0 || drow[prop.Name].GetType().Name.ToLower().IndexOf("decimal") >= 0))
                                    {

                                        if (drow[prop.Name].ToString() == "1")
                                        {
                                            prop.SetValue(hiColumn, true);

                                        }
                                        else
                                            prop.SetValue(hiColumn, false);
                                    }
                                    else
                                    {
                                        //prop.SetValue(hiColumn, drow[prop.Name]);
                                        if (drow[prop.Name].GetType().Name.ToLower().IndexOf("dbnull") < 0)
                                        {
                                            if (prop.PropertyType.Name == "Int32")
                                            {
                                                prop.SetValue(hiColumn, Convert.ToInt32(drow[prop.Name].ToString()));
                                            }
                                            else if (prop.PropertyType.Name == "Int64")
                                            {
                                                prop.SetValue(hiColumn, Convert.ToInt64(drow[prop.Name].ToString()));
                                            }
                                            else if (prop.PropertyType.Name == "Int16")
                                            {
                                                prop.SetValue(hiColumn, Convert.ToInt16(drow[prop.Name].ToString()));
                                            }
                                            else if (prop.PropertyType.Name == "Boolean")
                                            {
                                                prop.SetValue(hiColumn, drow[prop.Name]);
                                            }
                                            else if (prop.PropertyType.Name == "String")
                                            {
                                                prop.SetValue(hiColumn, drow[prop.Name].ToString().Trim());
                                            }
                                            else
                                                prop.SetValue(hiColumn, drow[prop.Name]);
                                        }
                                    }



                                    break;
                            }

                        }
                        else
                        {
                            if (prop.Name == "FieldName")
                            {
                                prop.SetValue(hiColumn, drow["FieldName"]);
                            }
                            //throw new Exception($"在表[{Constants.HiSysTable["Hi_TabModel"]}]未找到字段[{prop.Name}] 请检查是否版本需要升级");
                        }
                    }
                    hiColumns.Add(hiColumn);

                }



                tabInfo.Columns = hiColumns;

            }
            else
            {
                return null;
                //throw new Exception($"无法将DataSet表结构信息传成实体");
            }


            return tabInfo;
        }

        public static void InitMapping(Type type)
        {
            string _keyname = Constants.KEY_ENTITY_NAME.Replace("[$NAME$]", type.FullName);
            CacheContext.MCache.GetOrCreate<TabInfo>(_keyname, () =>
            {
                TabInfo tabinfo = new TabInfo();
                HiTable hitabs = new HiTable();
                tabinfo.EntityName = type.Name;
                tabinfo.DbTabName = type.Name;
                //类属性必须挂上 HiTable特性
                var _hitabs = type.GetCustomAttributes(true).Where(t => t is HiTable).Select(it => (HiTable)it).FirstOrDefault();
                if (hitabs != null)
                {
                    hitabs = _hitabs.MoveCross<HiTable>(hitabs);

                    if (hitabs.TabName == string.Empty)
                    {
                        hitabs.TabName = type.Name;
                    }
                    if (hitabs.TabReName == string.Empty)
                    {
                        //表别名
                        hitabs.TabReName = hitabs.TabName;
                    }
                }
                else
                {
                    hitabs.TabName = type.Name;
                    hitabs.TabReName = hitabs.TabName;
                }


                tabinfo.TabModel = hitabs;
                foreach (PropertyInfo n in type.GetProperties())
                {
                    var _hicolumn = new HiColumn();
                    var hiColumn = n.GetCustomAttributes(typeof(HiColumn), true)
                    .Where(it => it is HiColumn).Select(it => (HiColumn)it).FirstOrDefault();
                    if (!hiColumn.IsNullOrEmpty())
                    {
                        if (hiColumn.FieldName == string.Empty)
                        {
                            hiColumn.FieldName = n.Name;
                        }
                        if (hiColumn.FieldDesc == string.Empty)
                        {
                            hiColumn.FieldDesc = hiColumn.FieldName;
                        }

                        _hicolumn = hiColumn.MoveCross<HiColumn>(_hicolumn);

                        HiType ftype = n.PropertyType.ToDbFieldType();
                        //没有特别标记忽略
                        if (!hiColumn.IsIgnore)
                        {
                            tabinfo.Columns.Add(_hicolumn);
                        }
                    }
                    else
                    {
                        _hicolumn.FieldName = n.Name;
                        tabinfo.Columns.Add(_hicolumn);

                    }
                }
                return tabinfo;
            });
        }
    }
}
