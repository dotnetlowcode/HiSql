using HiSql.AST;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HiSql
{
    public class MySqlDM : IDM
    {
        StringBuilder _Sql = new StringBuilder();
        public virtual HiSqlProvider Context { get; set; }
        MySqlConfig dbConfig = new MySqlConfig(true);
        public MySqlDM()
        {
        }
        MCache mcache = new MCache(typeof(MySqlDM).FullName);
        public DBVersion DBVersion()
        {

            var version = mcache.GetOrCreate(typeof(MySqlDM).FullName + "_DBVersion", () =>
            {
                var _version = new DBVersion() { Version = new Version() };

                var tablerows = Context.DBO.GetDataTable(dbConfig.GetVersion).Rows;
                if (tablerows.Count > 0)
                {
                    _version.VersionDesc = tablerows[0][0]?.ToString();

                    _version.Version = new Version(tablerows[0][0]?.ToString());
                }
                return _version;
            });
            return version;
        }
        public bool InstallHisql(HiSqlClient hiSqlClient)
        {
            throw new NotImplementedException();
        }
        #region IDMInitalize接口实现
        public TabInfo BuildTab(Type type)
        {
            TabInfo tabInfo = new TabInfo();

            Tuple<HiTable, List<HiColumn>> tuple = BuildTabStru(type);
            tabInfo.TabModel = tuple.Item1;
            tabInfo.Columns = tuple.Item2;
            return tabInfo;
        }
        public Tuple<HiTable, List<HiColumn>> BuildTabStru(Type type)
        {
            if (type == null)
                throw new Exception($"Type 不能为Null");

            var attrTab = type.GetCustomAttributes().Where(a => a.GetType().Name == "HiTable").ToList();
            if (attrTab == null || attrTab.Count() == 0)
                throw new Exception($"实体类[{type.FullName}]无法生成表结构信息缺少[HiTable]属性");

            List<PropertyInfo> plist = type.GetProperties().Where(p => p.CanRead == true && p.MemberType == MemberTypes.Property).ToList();
            if (plist == null || plist.Count() == 0)
                throw new Exception($"实体类[{type.FullName}]无法映射成数据表 类缺少属性");
            HiTable hiTable = new HiTable();
            List<HiColumn> lstColumn = new List<HiColumn>();
            bool _hasTabAttr = false;
            foreach (Attribute pa in attrTab)
            {
                hiTable = (HiTable)pa; ;
                _hasTabAttr = true;
            }
            if (!_hasTabAttr)
            {
                if (string.IsNullOrEmpty(hiTable.TabName))
                    hiTable.TabName = type.Name;
            }


            foreach (PropertyInfo p in plist)
            {
                //如果字段没有设置属性那么就新建一个
                HiColumn hiColumn = new HiColumn();
                bool _hasAttru = false;
                foreach (Attribute pa in p.GetCustomAttributes().Where(a => a.GetType().Name == "HiColumn"))
                {
                    _hasAttru = true;
                    hiColumn = (HiColumn)pa;
                }
                if (!_hasAttru)
                {
                    hiColumn.FieldName = p.Name;
                    //hiColumn.FieldType
                }
                if (hiColumn.FieldType == HiType.NONE)
                {
                    //未指定类型时默认
                    hiColumn.FieldType = p.PropertyType.ToHiType();
                }
                if (string.IsNullOrEmpty(hiColumn.FieldName))
                    hiColumn.FieldName = p.Name;


                if (hiColumn.FieldType.IsIn<HiType>(HiType.NVARCHAR, HiType.VARCHAR, HiType.NCHAR, HiType.CHAR) && hiColumn.FieldLen == 0)
                {
                    hiColumn.FieldLen = 50;//默认50个字符串
                }


                lstColumn.Add(hiColumn);
            }
            return new Tuple<HiTable, List<HiColumn>>(hiTable, lstColumn);
        }
        /// <summary>
        /// 获取对应的字段类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetDbType(HiType type)
        {
            if (dbConfig.DbMapping.ContainsKey(type))
            {
                return dbConfig.DbMapping[type];
            }
            return "";
        }
        /// <summary>
        /// 将现有表中的数据转成 表结构数据
        /// </summary>
        /// <param name="table"></param>
        /// <param name="_dbmapping"></param>
        /// <returns></returns>
        public TabInfo TabDefinitionToEntity(DataTable table, Dictionary<HiType, string> _dbmapping)
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
                    hiColumn.FieldType = HiSqlCommProvider.ConvertToHiType(_dbmapping, drow["FieldType"].ToString().ToLower().Trim());

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
                    string _dbdefault = drow["DbDefault"].ToString().Trim();


                    List<DefMapping> _lstdef = new List<DefMapping>();
                    if (hiColumn.FieldType.IsCharField())
                    {
                        _lstdef = dbConfig.DbDefMapping.Where(d => d.DbType == HiTypeGroup.Char || d.DBDefault == HiTypeDBDefault.NONE).ToList();//
                    }
                    else if (hiColumn.FieldType.IsNumberField())
                    {
                        _lstdef = dbConfig.DbDefMapping.Where(d => d.DbType == HiTypeGroup.Number).ToList();
                    }
                    else if (hiColumn.FieldType.IsDateField())
                    {
                        _lstdef = dbConfig.DbDefMapping.Where(d => d.DbType == HiTypeGroup.Date).ToList();
                    }
                    else if (hiColumn.FieldType.IsBoolField())
                    {
                        _lstdef = dbConfig.DbDefMapping.Where(d => d.DbType == HiTypeGroup.Bool).ToList();
                    }

                    bool _flag = false;
                    foreach (DefMapping def in _lstdef)
                    {
                        if (def.IsRegex)
                        {
                            Dictionary<string, string> _dic = Tool.RegexGrp(def.DbValue, _dbdefault);
                            if (_dic.Count > 0 && _dic.ContainsKey("value"))
                            {
                                if (def.DbType.IsIn<HiTypeGroup>(HiTypeGroup.Char, HiTypeGroup.Bool, HiTypeGroup.Number))
                                {
                                    if (string.IsNullOrEmpty(_dic["value"].ToString()))
                                        hiColumn.DBDefault = HiTypeDBDefault.EMPTY;
                                    else
                                        hiColumn.DBDefault = HiTypeDBDefault.VALUE;
                                    hiColumn.DefaultValue = _dic["value"].ToString();
                                }
                                else if (def.DbType.IsIn<HiTypeGroup>(HiTypeGroup.Date))
                                {
                                    hiColumn.DBDefault = HiTypeDBDefault.FUNDATE;
                                    hiColumn.DefaultValue = Constants.FunDate;
                                    //hiColumn.DefaultValue = dbConfig.Fun_CurrDATE;
                                }
                                else
                                {
                                    //忽略
                                }

                                //Console.WriteLine($"类型[{def.DbType}]默认值{_dic["value"].ToString()} 默认值类型{hiTypeDBDefault}");
                                _flag = true;
                            }
                        }
                        else
                        {
                            if (_dbdefault == def.DbValue)
                            {
                                //Console.WriteLine("匹配空值");
                                _flag = true;

                            }
                        }
                    }



                    //if (_dbdefault == "''" || _dbdefault == "" || _dbdefault == "0")
                    //{
                    //    hiColumn.DBDefault = HiTypeDBDefault.EMPTY;
                    //    hiColumn.DefaultValue = _dbdefault;
                    //}
                    //else if (_dbdefault.ToLower() == "current_timestamp".ToLower())
                    //{
                    //    hiColumn.DBDefault = HiTypeDBDefault.FUNDATE;
                    //}
                    //else if (Tool.IsDecimal(_dbdefault))
                    //{
                    //    //指定非空的默认值
                    //    hiColumn.DBDefault = HiTypeDBDefault.VALUE;
                    //    hiColumn.DefaultValue = _dbdefault;
                    //}
                    //else if (string.IsNullOrEmpty(_dbdefault))
                    //{
                    //    hiColumn.DBDefault = HiTypeDBDefault.VALUE;
                    //    hiColumn.DefaultValue = _dbdefault;
                    //}

                    tabInfo.Columns.Add(hiColumn);

                }
                return tabInfo;

            }
            else
                throw new Exception($"获取的物理表结构信息不符合规范");


            //return tabInfo;
        }
        /// <summary>
        /// 默认空值
        /// </summary>
        /// <returns></returns>
        public string GetDbDefault(HiColumn hiColumn, string tabname = "")
        {
            string _default = "";
            if (hiColumn.DBDefault != HiTypeDBDefault.NONE)
            {
                if (hiColumn.FieldType.IsIn<HiType>(HiType.NCHAR, HiType.CHAR, HiType.NVARCHAR, HiType.VARCHAR))
                    _default = $"'{hiColumn.DefaultValue.Trim()}'";
                else if (hiColumn.FieldType.IsIn<HiType>(HiType.INT, HiType.BIGINT, HiType.SMALLINT, HiType.DECIMAL, HiType.BOOL))
                {
                    if (Tool.IsDecimal(hiColumn.DefaultValue.Trim()))
                    {
                        _default = $"{hiColumn.DefaultValue.Trim()}";
                    }
                    else
                        _default = "0";
                }

                else if (hiColumn.FieldType.IsIn<HiType>(HiType.GUID))
                {
                    _default = "''";
                }
                else if (hiColumn.FieldType.IsIn<HiType>(HiType.DATE, HiType.DATETIME))
                {
                    if (hiColumn.DefaultValue.Trim().ToLower() == Constants.FunDate.ToLower().Trim())
                        _default = "current_timestamp";
                    else
                        _default = "current_timestamp";

                }
                else _default = "";

                if (!string.IsNullOrEmpty(_default))
                    _default = $"default {_default}";
            }
            return _default;
        }
        /// <summary>
        /// 获取表结构信息并缓存
        /// </summary>
        /// <param name="tabname"></param>
        /// <returns></returns>
        public TabInfo GetTabStruct(string tabname)
        {

            string _schema = string.IsNullOrEmpty(Context.CurrentConnectionConfig.Schema) ? throw new Exception($"当前数据类型为[{Context.CurrentConnectionConfig.DbType.ToString()}] 请指定数据库Schema") : Context.CurrentConnectionConfig.Schema;
            List<HiColumn> _lstmodi = new List<HiColumn>();
            List<object> _lstdel = new List<object>();
            HiSqlClient _client = null;
            TabInfo newtabinfo = HiSqlCommProvider.InitTabMaping(tabname, () =>
            {
                tabname = tabname.ToSqlInject();
                DataSet ds = new DataSet();
                DataTable dt_model = this.Context.DBO.GetDataTable($"select * from {dbConfig.Schema_Pre}{Context.CurrentConnectionConfig.Schema}{dbConfig.Schema_After}.{dbConfig.Table_Pre}{Constants.HiSysTable["Hi_TabModel"].ToString()}{dbConfig.Table_After} where {dbConfig.Field_Pre}TabName{dbConfig.Field_After}=@TabName ", new HiParameter("@TabName", tabname));
                dt_model.TableName = Constants.HiSysTable["Hi_TabModel"].ToString();
                ds.Tables.Add(dt_model);

                DataTable dt_struct = Context.DBO.GetDataTable($"select * from {dbConfig.Schema_Pre}{Context.CurrentConnectionConfig.Schema}{dbConfig.Schema_After}.{dbConfig.Table_Pre}{Constants.HiSysTable["Hi_FieldModel"].ToString()}{dbConfig.Table_After} where {dbConfig.Field_Pre}TabName{dbConfig.Field_After}=@TabName order by {dbConfig.Field_Pre}sortnum{dbConfig.Field_After} asc", new HiParameter("@TabName", tabname));
                dt_struct.TableName = Constants.HiSysTable["Hi_FieldModel"].ToString();
                ds.Tables.Add(dt_struct);

                TabInfo tabInfo = HiSqlCommProvider.TabToEntity(ds);
                tabname = tabInfo != null ? tabInfo.TabModel.TabName : tabname;
                //获取表结构信息因为可能数据库中的物理表结构可能会有变更,需要与缓存表中的数据进行比对
                DataTable dts = GetTableDefinition(tabname);
                if (dts == null || dts.Rows.Count == 0)
                    throw new Exception($"表[{tabname}]不存在");
                else
                {
                    if (tabInfo == null)
                        tabname = dts.Rows[0]["TabName"].ToString();
                }
                dts.TableName = tabname;

                if (tabInfo == null)
                {
                    //说明该表不是通过工具创建的,他的表结构信息不存在于Hi_TabModel和Hi_FieldModel中
                    //那么需要通过底层SQL代码获取表结构信息 然后再添加到Hi_TabModel和Hi_FieldModel中 再进行缓存处理
                    tabInfo = TabDefinitionToEntity(dts, dbConfig.DbMapping);
                    HiSqlCommProvider.LockTableExecAction(tabname, () =>
                    {
                        int rtn = this.BuildTabCreate(tabInfo);
                    });
                    //string _sql = this.BuildTabStructSql(tabInfo.TabModel, tabInfo.Columns).ToString();
                    //this.Context.DBO.ExecCommand(_sql);
                    //GetTabStruct(tabname);
                }
                else
                {
                    //物理表结构已经存在于Hi_TabModel和Hi_FieldModel中
                    //需要进行比对变更以物理表为准，原有表的扩展配置以Hi_FieldModel为准
                    //避免出现检测死循环 Hi_FieldModel,Hi_TabModel 变更需要通过升级工具升级
                    if (tabname.ToLower() != Constants.HiSysTable["Hi_FieldModel"].ToString().ToLower()
                        &&
                        tabname.ToLower() != Constants.HiSysTable["Hi_TabModel"].ToString().ToLower()
                    )
                    {
                        #region 与物理表进行比对
                        //如果不一样（则有变更物理表） 则以物理表的数据为准

                        var phytabInfo = TabDefinitionToEntity(dts, dbConfig.DbMapping);
                        List<FieldChange> fieldChanges = HiSqlCommProvider.TabToCompare(phytabInfo, tabInfo);
                        List<HiColumn> lstcolumn = tabInfo.GetColumns;

                        phytabInfo = HiSqlCommProvider.TabMerge(phytabInfo, tabInfo);
                        List<HiColumn> phyclumn = phytabInfo.GetColumns;
                        var delfield = fieldChanges.Where(f => f.Action == TabFieldAction.DELETE).ToList();
                        var modifield = fieldChanges.Where(f => f.Action == TabFieldAction.ADD || (f.Action == TabFieldAction.MODI && f.IsTabChange == true)).ToList();
                        if (delfield != null && delfield.Count > 0)
                        {
                            List<object> lstobj = new List<object>();
                            foreach (FieldChange fieldChange in delfield)
                            {
                                HiColumn column = lstcolumn.Where(h => h.FieldName.ToLower() == fieldChange.FieldName.ToLower()).FirstOrDefault();
                                if (column != null)
                                {
                                    //lstobj.Add(new { TabName = column.TabName, FieldName = column.FieldName });
                                    _lstdel.Add(new { TabName = column.TabName, FieldName = column.FieldName });
                                }
                            }
                            if (lstobj.Count > 0)
                            {
                                //_client.Delete(Constants.HiSysTable["Hi_FieldModel"].ToString(), lstobj).ExecCommand();
                            }
                        }
                        if (modifield != null && modifield.Count > 0)
                        {
                            List<HiColumn> lstobj = new List<HiColumn>();
                            foreach (FieldChange fieldChange in modifield)
                            {
                                HiColumn _column = phyclumn.Where(h => h.FieldName.ToLower() == fieldChange.FieldName.ToLower()).FirstOrDefault();
                                if (_column != null)
                                {
                                    _column.TabName = phytabInfo.TabModel.TabName;
                                    //lstobj.Add(_column);
                                    _lstmodi.Add(_column);
                                }
                            }
                            if (lstobj.Count > 0)
                            {
                                //_client.Modi(Constants.HiSysTable["Hi_FieldModel"].ToString(), lstobj).ExecCommand();
                            }
                        }
                        tabInfo = phytabInfo;

                        #endregion
                    }
                }
                return tabInfo;
            });

            if (_lstdel.Count > 0 || _lstmodi.Count > 0)
            {
                HiSqlCommProvider.LockTableExecAction(tabname, () =>
                {
                    _client = this.Context.CloneClient();

                    if (_lstdel.Count > 0)
                    {
                        try
                        {
                            _client.Delete(Constants.HiSysTable["Hi_FieldModel"].ToString(), _lstdel).ExecCommand();
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                        finally
                        {
                            _client.Context.DBO.Close();
                            _client.Context.DBO.Dispose();
                        }


                    }
                    if (_lstmodi.Count > 0)
                    {
                        try
                        {
                            _client.Modi(Constants.HiSysTable["Hi_FieldModel"].ToString(), _lstmodi).ExecCommand();
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                        finally
                        {
                            _client.Context.DBO.Close();
                            _client.Context.DBO.Dispose();
                        }
                    }
                });
            }

            return newtabinfo;
        }
        #endregion


        #region 私有方法
        /// <summary>
        /// 生成插入表结构表的语句
        /// </summary>
        /// <param name="hiTable"></param>
        /// <param name="lstHiTable"></param>
        /// <returns></returns>
        string inertTabStruct(HiTable hiTable, List<HiColumn> lstHiTable)
        {
            StringBuilder sb = new StringBuilder();

            string _schema = this.Context.CurrentConnectionConfig.Schema;
            sb.AppendLine($"insert into {dbConfig.Schema_Pre}{_schema}{dbConfig.Schema_After}.{dbConfig.Table_Pre}{Constants.HiSysTable["Hi_TabModel"]}{dbConfig.Table_After} (")
               .Append($"{dbConfig.Field_Pre}TabName{dbConfig.Field_After}{dbConfig.Field_Split}")
                .Append($"{dbConfig.Field_Pre}TabReName{dbConfig.Field_After}{dbConfig.Field_Split}")
                .Append($"{dbConfig.Field_Pre}TabDescript{dbConfig.Field_After}{dbConfig.Field_Split}")
                .Append($"{dbConfig.Field_Pre}TabStoreType{dbConfig.Field_After}{dbConfig.Field_Split}")
                .Append($"{dbConfig.Field_Pre}TabType{dbConfig.Field_After}{dbConfig.Field_Split}")
                .Append($"{dbConfig.Field_Pre}TabCacheType{dbConfig.Field_After}{dbConfig.Field_Split}")
                .Append($"{dbConfig.Field_Pre}TabStatus{dbConfig.Field_After}{dbConfig.Field_Split}")
                .Append($"{dbConfig.Field_Pre}IsSys{dbConfig.Field_After}{dbConfig.Field_Split}")
                .Append($"{dbConfig.Field_Pre}IsEdit{dbConfig.Field_After}{dbConfig.Field_Split}")
                .Append($"{dbConfig.Field_Pre}IsLog{dbConfig.Field_After}{dbConfig.Field_Split}")
                .Append($"{dbConfig.Field_Pre}LogTable{dbConfig.Field_After}{dbConfig.Field_Split}")
                .Append($"{dbConfig.Field_Pre}LogExprireDay{dbConfig.Field_After}")
                .Append(") values(")
                .Append($"'{hiTable.TabName}',")
                .Append($"'{hiTable.TabReName.ToSqlInject()}',")
                .Append($"'{hiTable.TabDescript.ToSqlInject()}',")
                .Append($"'{(int)hiTable.TabStoreType}',")
                .Append($"'{(int)hiTable.TabType}',")
                .Append($"'{(int)hiTable.TabCacheType}',")
                .Append($"'{(int)hiTable.TabStatus}',")
                .Append(hiTable.IsSys == true ? 1 : 0)
                .Append(',')
                .Append(hiTable.IsEdit == true ? 1 : 0)
                .Append(',')
                .Append(hiTable.IsLog == true ? 1 : 0)
                .Append(',')
                .Append($"'{ hiTable.LogTable}',")
                .Append($"'{ hiTable.LogExprireDay}'")
                .Append($");")
                ;
            sb.AppendLine("");
            foreach (HiColumn hiColumn in lstHiTable)
            {
                sb.AppendLine($"insert into {dbConfig.Schema_Pre}{_schema}{dbConfig.Schema_After}.{dbConfig.Table_Pre}{Constants.HiSysTable["Hi_FieldModel"]}{dbConfig.Table_After} (")
                    .Append($"{dbConfig.Field_Pre}TabName{dbConfig.Field_After}{dbConfig.Field_Split}")
                    .Append($"{dbConfig.Field_Pre}FieldName{dbConfig.Field_After}{dbConfig.Field_Split}")
                    .Append($"{dbConfig.Field_Pre}FieldDesc{dbConfig.Field_After}{dbConfig.Field_Split}")
                    .Append($"{dbConfig.Field_Pre}IsIdentity{dbConfig.Field_After}{dbConfig.Field_Split}")
                    .Append($"{dbConfig.Field_Pre}IsPrimary{dbConfig.Field_After}{dbConfig.Field_Split}")
                    .Append($"{dbConfig.Field_Pre}IsBllKey{dbConfig.Field_After}{dbConfig.Field_Split}")
                    .Append($"{dbConfig.Field_Pre}FieldType{dbConfig.Field_After}{dbConfig.Field_Split}")
                    .Append($"{dbConfig.Field_Pre}SortNum{dbConfig.Field_After}{dbConfig.Field_Split}")
                    .Append($"{dbConfig.Field_Pre}Regex{dbConfig.Field_After}{dbConfig.Field_Split}")
                    .Append($"{dbConfig.Field_Pre}DBDefault{dbConfig.Field_After}{dbConfig.Field_Split}")
                    .Append($"{dbConfig.Field_Pre}DefaultValue{dbConfig.Field_After}{dbConfig.Field_Split}")
                    .Append($"{dbConfig.Field_Pre}FieldLen{dbConfig.Field_After}{dbConfig.Field_Split}")
                    .Append($"{dbConfig.Field_Pre}FieldDec{dbConfig.Field_After}{dbConfig.Field_Split}")
                    .Append($"{dbConfig.Field_Pre}SNO{dbConfig.Field_After}{dbConfig.Field_Split}")
                    .Append($"{dbConfig.Field_Pre}SNO_NUM{dbConfig.Field_After}{dbConfig.Field_Split}")
                    .Append($"{dbConfig.Field_Pre}IsSys{dbConfig.Field_After}{dbConfig.Field_Split}")
                    .Append($"{dbConfig.Field_Pre}IsNull{dbConfig.Field_After}{dbConfig.Field_Split}")
                    .Append($"{dbConfig.Field_Pre}IsRequire{dbConfig.Field_After}{dbConfig.Field_Split}")
                    .Append($"{dbConfig.Field_Pre}IsIgnore{dbConfig.Field_After}{dbConfig.Field_Split}")
                    .Append($"{dbConfig.Field_Pre}IsObsolete{dbConfig.Field_After}{dbConfig.Field_Split}")
                    .Append($"{dbConfig.Field_Pre}IsShow{dbConfig.Field_After}{dbConfig.Field_Split}")
                    .Append($"{dbConfig.Field_Pre}IsSearch{dbConfig.Field_After}{dbConfig.Field_Split}")
                    .Append($"{dbConfig.Field_Pre}SrchMode{dbConfig.Field_After}{dbConfig.Field_Split}")
                    .Append($"{dbConfig.Field_Pre}IsRefTab{dbConfig.Field_After}{dbConfig.Field_Split}")
                    .Append($"{dbConfig.Field_Pre}RefTab{dbConfig.Field_After}{dbConfig.Field_Split}")
                    .Append($"{dbConfig.Field_Pre}RefField{dbConfig.Field_After}{dbConfig.Field_Split}")
                    .Append($"{dbConfig.Field_Pre}RefFields{dbConfig.Field_After}{dbConfig.Field_Split}")
                    .Append($"{dbConfig.Field_Pre}RefFieldDesc{dbConfig.Field_After}{dbConfig.Field_Split}")
                    .Append($"{dbConfig.Field_Pre}RefWhere{dbConfig.Field_After}")
                    .Append(")values(")
                    .Append($"'{hiTable.TabName}',")
                    .Append($"'{hiColumn.FieldName}',")
                    .Append($"'{hiColumn.FieldDesc.ToSqlInject()}',")
                    .Append(hiColumn.IsIdentity == true ? 1 : 0)
                    .Append(',')
                    .Append(hiColumn.IsPrimary == true ? 1 : 0)
                    .Append(',')
                    .Append(hiColumn.IsBllKey == true ? 1 : 0)
                    .Append(',')
                    .Append($"'{(int)hiColumn.FieldType}',")
                    .Append($"'{hiColumn.SortNum}',")
                    .Append($"'{hiColumn.Regex.ToSqlInject()}',")
                    .Append($"'{(int)hiColumn.DBDefault}',")
                    .Append($"'{hiColumn.DefaultValue.ToSqlInject()}',")
                    .Append($"'{hiColumn.FieldLen}',")
                    .Append($"'{hiColumn.FieldDec}',")
                    .Append($"'{hiColumn.SNO}',")
                    .Append($"'{hiColumn.SNO_NUM}',")
                    .Append(hiColumn.IsSys == true ? 1 : 0)
                    .Append(',')
                    .Append(hiColumn.IsNull == true ? 1 : 0)
                    .Append(',')
                    .Append(hiColumn.IsRequire == true ? 1 : 0)
                    .Append(',')
                    .Append(hiColumn.IsIgnore == true ? 1 : 0)
                    .Append(',')
                    .Append(hiColumn.IsObsolete == true ? 1 : 0)
                    .Append(',')
                    .Append(hiColumn.IsShow == true ? 1 : 0)
                    .Append(',')
                    .Append(hiColumn.IsSearch == true ? 1 : 0)
                    .Append(',')
                    .Append($"'{(int)hiColumn.SrchMode}',")
                    .Append(hiColumn.IsRefTab == true ? 1 : 0)
                    .Append(',')
                    .Append($"'{hiColumn.RefTab.ToSqlInject()}',")
                    .Append($"'{hiColumn.RefField.ToSqlInject()}',")
                    .Append($"'{hiColumn.RefFields.ToSqlInject()}',")
                    .Append($"'{hiColumn.RefFieldDesc.ToSqlInject()}',")
                    .Append($"'{hiColumn.RefWhere.ToSqlInject()}'")
                    .Append($");")
                    ;
                sb.AppendLine("");
            }
            return sb.ToString();
        }
        /// <summary>
        /// 生成 通过insert () union all select  的插入语句
        /// </summary>
        /// <param name="_values"></param>
        /// <param name="isbulk"></param>
        /// <returns></returns>
        string buildInsertV2(Dictionary<string, string> _values, bool isbulk = false)
        {
            string _insert_temp = dbConfig.Insert_StateMentv2;
            StringBuilder _sb_field = new StringBuilder();
            StringBuilder _sb_value = new StringBuilder();
            int _i = 0;
            foreach (string n in _values.Keys)
            {
                if (_i != _values.Count - 1)
                {
                    _sb_field.Append($"{n},");
                    _sb_value.Append($"{_values[n]},");
                }
                else
                {
                    _sb_field.Append($"{n}");
                    _sb_value.Append($"{_values[n]}");
                }
                _i++;
            }
            if (!isbulk)
                return _insert_temp
                    .Replace("[$FIELDS$]", _sb_field.ToString())
                    .Replace("[$VALUES$]", $"select {_sb_value.ToString()}")
                    ;
            else
                return $" UNION ALL select {_sb_value.ToString()}";
        }
        #endregion


        #region 接口实现

        public string BuildInsertSql(Dictionary<string, string> _values, bool isbulk = false)
        {

            string _insert_temp = dbConfig.Insert_StateMent;
            StringBuilder _sb_field = new StringBuilder();
            StringBuilder _sb_value = new StringBuilder();
            _insert_temp = _insert_temp.Replace("[$Schema$]", Context.CurrentConnectionConfig.Schema);
            int _i = 0;
            foreach (string n in _values.Keys)
            {
                if (_i != _values.Count - 1)
                {
                    _sb_field.Append($"{dbConfig.Field_Pre}{n}{dbConfig.Field_After},");
                    _sb_value.Append($"{_values[n]},");
                }
                else
                {
                    _sb_field.Append($"{dbConfig.Field_Pre}{n}{dbConfig.Field_After}");
                    _sb_value.Append($"{_values[n]}");
                }
                _i++;
            }
            if (!isbulk)
                return _insert_temp
                    .Replace("[$FIELDS$]", _sb_field.ToString())
                    .Replace("[$VALUES$]", _sb_value.ToString())
                    ;
            else
                return $",({_sb_value.ToString()})";
        }
        public string BuildMergeIntoSqlSequence(TabInfo targetinfo, List<string> dataColLst = null)
        {
            throw new NotSupportedException("该方法仅支持PostGreSql数据库");
        }
        public string BuildMergeIntoSql(TabInfo targetinfo, TabInfo sourceinfo, List<string> dataColLst = null)
        {

            string _merge_temp = dbConfig.Table_MergeInto;

            List<string> _filer = new List<string>();
            List<string> _lstupdate = new List<string>();
            List<string> _newfield = new List<string>();
            List<string> _newvalue = new List<string>();
            var bllkeys = targetinfo.BllKey;
            string _filterstr = string.Empty;
            string _updatestr = string.Empty;
            string _fieldstr = string.Empty;
            string _valuestr = string.Empty;
            foreach (HiColumn hiColumn in bllkeys)
            {
                _filer.Add($"a.{dbConfig.Field_Pre}{hiColumn.FieldName}{dbConfig.Field_After}=b.{dbConfig.Field_Pre}{hiColumn.FieldName}{dbConfig.Field_After}");
            }

            if (_filer.Count == 0)
                throw new Exception($"表[{targetinfo.TabModel.TabName}]无业务KEY");

            _filterstr = string.Join(" and ", _filer);

            foreach (HiColumn hiColumn in targetinfo.GetColumns)
            {
                //非业务KEY，非自增长ID，且非创建时的标准字段
                if (!hiColumn.IsBllKey && !hiColumn.IsIdentity && !hiColumn.IsCreateField())
                {
                    //_lstupdate.Add($"{dbConfig.Schema_Pre}{Context.CurrentConnectionConfig.Schema}{dbConfig.Schema_After}.{dbConfig.Table_Pre}{targetinfo.TabModel.TabReName}{dbConfig.Table_After}.{dbConfig.Field_Pre}{hiColumn.FieldName}{dbConfig.Field_After}=values({dbConfig.Field_Pre}{hiColumn.FieldName}{dbConfig.Field_After})");
                    //add by tgm date:2022.4.8 
                    if (dataColLst != null && dataColLst.Count > 0)
                    {
                        if (dataColLst.Any(c => c.ToLower().Equals(hiColumn.FieldName.ToLower())))
                            _lstupdate.Add($"{dbConfig.Schema_Pre}{Context.CurrentConnectionConfig.Schema}{dbConfig.Schema_After}.{dbConfig.Table_Pre}{targetinfo.TabModel.TabReName}{dbConfig.Table_After}.{dbConfig.Field_Pre}{hiColumn.FieldName}{dbConfig.Field_After}=values({dbConfig.Field_Pre}{hiColumn.FieldName}{dbConfig.Field_After})");
                        else if (hiColumn.IsModiField())
                            _lstupdate.Add($"{dbConfig.Schema_Pre}{Context.CurrentConnectionConfig.Schema}{dbConfig.Schema_After}.{dbConfig.Table_Pre}{targetinfo.TabModel.TabReName}{dbConfig.Table_After}.{dbConfig.Field_Pre}{hiColumn.FieldName}{dbConfig.Field_After}=values({dbConfig.Field_Pre}{hiColumn.FieldName}{dbConfig.Field_After})");
                    }
                    else
                        _lstupdate.Add($"{dbConfig.Schema_Pre}{Context.CurrentConnectionConfig.Schema}{dbConfig.Schema_After}.{dbConfig.Table_Pre}{targetinfo.TabModel.TabReName}{dbConfig.Table_After}.{dbConfig.Field_Pre}{hiColumn.FieldName}{dbConfig.Field_After}=values({dbConfig.Field_Pre}{hiColumn.FieldName}{dbConfig.Field_After})");

                }

                if (!hiColumn.IsIdentity)
                {
                    _newfield.Add($"{dbConfig.Field_Pre}{hiColumn.FieldName}{dbConfig.Field_After}");
                    _newvalue.Add($"b.{dbConfig.Field_Pre}{hiColumn.FieldName}{dbConfig.Field_After}");
                }
            }
            if (_lstupdate.Count == 0 || _newfield.Count == 0)
                throw new Exception($"表[{targetinfo.TabModel.TabName}]可更新或插入的字段");

            _updatestr = string.Join(dbConfig.Field_Split, _lstupdate);
            _fieldstr = string.Join(dbConfig.Field_Split, _newfield);
            _valuestr = string.Join(dbConfig.Field_Split, _newvalue);
            _merge_temp = _merge_temp
                .Replace("[$TabName$]", targetinfo.TabModel.TabName)
                .Replace("[$Source$]", sourceinfo.TabModel.TabName)
                .Replace("[$OnFilter$]", _filterstr)
                .Replace("[$Update$]", _updatestr)
                .Replace("[$Field$]", _fieldstr)
                .Replace("[$Values$]", _valuestr)
                .Replace("[$Schema$]", Context.CurrentConnectionConfig.Schema)
                ;

            return _merge_temp;
        }
        public string BuildDeleteSql(TableDefinition table, Dictionary<string, string> dic_value, string _where, bool istrunctate = false, bool isdrop = false)
        {
            string _temp_delete = string.Empty;
            StringBuilder sb_field = new StringBuilder();
            string _schema = string.IsNullOrEmpty(Context.CurrentConnectionConfig.Schema) ? "dbo" : Context.CurrentConnectionConfig.Schema;
            int i = 0;
            if ((!istrunctate && !isdrop && dic_value.Count > 0) || !string.IsNullOrEmpty(_where))
            {
                _temp_delete = dbConfig.Delete_Statement_Where;
                if (dic_value.Count > 0)
                {
                    foreach (string n in dic_value.Keys)
                    {
                        sb_field.Append($"{dbConfig.Field_Pre}{n}{dbConfig.Field_After}={dic_value[n].ToString()}");
                        if (i != dic_value.Count() - 1)
                            sb_field.Append($" and ");
                        i++;
                    }
                }
                else
                {
                    sb_field.Append(_where);
                }
                _temp_delete = _temp_delete
                    .Replace("[$Schema$]", _schema)
                    .Replace("[$TabName$]", table.TabName)
                    .Replace("[$Where$]", sb_field.ToString());
                ;
            }
            else
            {
                if (!istrunctate && !isdrop)
                {
                    //删除表
                    _temp_delete = dbConfig.Delete_Statement;
                    _temp_delete = _temp_delete
                        .Replace("[$Schema$]", _schema)
                        .Replace("[$TabName$]", table.TabName);
                }
                else if (istrunctate && !isdrop)
                {
                    //Truncate 删除表数据 无日志
                    _temp_delete = dbConfig.Delete_TrunCate;
                    _temp_delete = _temp_delete
                        .Replace("[$Schema$]", _schema)
                        .Replace("[$TabName$]", table.TabName);
                }
                else if (!istrunctate && isdrop)
                {
                    ///删除表数据
                    _temp_delete = dbConfig.Drop_Table;
                    _temp_delete = _temp_delete
                        .Replace("[$Schema$]", _schema)
                        .Replace("[$TabName$]", table.TabName);
                }
            }


            return _temp_delete;
        }

        /// <summary>
        /// 生成SQL更新语句
        /// </summary>
        /// <param name="table"></param>
        /// <param name="dic_value"></param>
        /// <param name="dic_primary"></param>
        /// <param name="_where"></param>
        /// <returns></returns>
        public string BuildUpdateSql(TableDefinition table, Dictionary<string, string> dic_value, Dictionary<string, string> dic_primary, string _where)
        {
            string _temp_sql = string.Empty;
            int i = 0;
            StringBuilder sb = new StringBuilder();
            StringBuilder sb_field = new StringBuilder();
            StringBuilder sb_primary = new StringBuilder();

            string _schema = string.IsNullOrEmpty(Context.CurrentConnectionConfig.Schema) ? "" : Context.CurrentConnectionConfig.Schema;

            if (dic_primary.Count() > 0 || !string.IsNullOrEmpty(_where))
            {
                _temp_sql = dbConfig.Update_Statement_Where;
            }
            else
                _temp_sql = dbConfig.Update_Statement;


            foreach (string n in dic_value.Keys)
            {
                sb_field.Append($"{dbConfig.Field_Pre}{n}{dbConfig.Field_After}={dic_value[n].ToString()}");
                if (i != dic_value.Count() - 1)
                    sb_field.Append($"{dbConfig.Field_Split}");
                i++;
            }
            i = 0;
            foreach (string n in dic_primary.Keys)
            {
                sb_primary.Append($"{dbConfig.Field_Pre}{n}{dbConfig.Field_After}={dic_primary[n].ToString()}");
                if (i != dic_primary.Count() - 1)
                    sb_primary.Append($" and ");
                i++;
            }

            if (!string.IsNullOrEmpty(sb_primary.ToString()) && !string.IsNullOrEmpty(_where))
                sb_primary.Append($" and {_where}");
            else
                sb_primary.Append($"{_where}");

            _temp_sql = _temp_sql
                .Replace("[$Schema$]", _schema)
                .Replace("[$TabName$]", table.TabName)
                .Replace("[$Fields$]", sb_field.ToString())
                .Replace("[$Where$]", sb_primary.ToString())
                ;
            return _temp_sql;
        }

        public string BuildUpdateSql(TabInfo tabinfo, TableDefinition table, Dictionary<string, string> dic_value, Dictionary<string, string> dic_primary, string _where)
        {
            string _temp_sql = string.Empty;
            int i = 0;
            StringBuilder sb = new StringBuilder();
            StringBuilder sb_field = new StringBuilder();
            StringBuilder sb_primary = new StringBuilder();

            string _schema = string.IsNullOrEmpty(Context.CurrentConnectionConfig.Schema) ? "" : Context.CurrentConnectionConfig.Schema;

            if (dic_primary.Count() > 0 || !string.IsNullOrEmpty(_where))
            {
                _temp_sql = dbConfig.Update_Statement_Where;
            }
            else
                _temp_sql = dbConfig.Update_Statement;


            foreach (string n in dic_value.Keys)
            {
                var columninfo = tabinfo.Columns.Where(c => c.FieldName.ToLower() == n.ToLower()).FirstOrDefault();
                string _str = dic_value[n];
                //只有是字段类型为数字的才支持
                if (columninfo != null && columninfo.FieldType.IsIn<HiType>(HiType.INT, HiType.BIGINT, HiType.DECIMAL, HiType.SMALLINT))
                {
                    ///检测是否有以字段更新字段的语法
                    List<Dictionary<string, string>> _lstdic = Tool.RegexGrps(Constants.REG_TEMPLATE_FIELDS, dic_value[n]);
                    if (_lstdic.Count() > 0)
                    {
                        //说明是基于
                        Regex regex = new Regex(Constants.REG_TEMPLATE_FIELDS);
                        
                        foreach (Dictionary<string, string> dic in _lstdic)
                        {
                            _str = regex.Replace(_str, $"{dbConfig.Field_Pre}{dic["field"].ToString()}{dbConfig.Field_After}", 1);
                        }
                        
                    }
                }

                sb_field.Append($"{dbConfig.Field_Pre}{n}{dbConfig.Field_After}={_str}");
                if (i != dic_value.Count() - 1)
                    sb_field.Append($"{dbConfig.Field_Split}");
                i++;
            }
            i = 0;
            foreach (string n in dic_primary.Keys)
            {
                sb_primary.Append($"{dbConfig.Field_Pre}{n}{dbConfig.Field_After}={dic_primary[n].ToString()}");
                if (i != dic_primary.Count() - 1)
                    sb_primary.Append($" and ");
                i++;
            }

            if (!string.IsNullOrEmpty(sb_primary.ToString()) && !string.IsNullOrEmpty(_where))
                sb_primary.Append($" and {_where}");
            else
                sb_primary.Append($"{_where}");

            _temp_sql = _temp_sql
                .Replace("[$Schema$]", _schema)
                .Replace("[$TabName$]", table.TabName)
                .Replace("[$Fields$]", sb_field.ToString())
                .Replace("[$Where$]", sb_primary.ToString())
                ;
            return _temp_sql;
        }

        //生成Key字段
        public string BuildKey(List<HiColumn> hiColumn)
        {
            StringBuilder sb_field = new StringBuilder();
            var cols = hiColumn.Where(c => c.IsPrimary == true).ToList();
            if (cols.Count > 0)
            {
                string _tempfield = "";
                for (int i = 0; i < cols.Count; i++)
                {
                    _tempfield = dbConfig.Table_Key2;
                    _tempfield = _tempfield.Replace("[$FieldName$]", $"{dbConfig.Field_Pre}{cols[i].FieldName}{dbConfig.Field_After}");
                    if (i != cols.Count - 1)
                        _tempfield += dbConfig.Field_Split;

                    sb_field.AppendLine(_tempfield);

                }
                return sb_field.ToString();
            }
            else return "";
        }

        /// <summary>
        /// 生成修改表结构字段语句
        /// </summary>
        /// <param name="hiTable"></param>
        /// <param name="hiColumn"></param>
        /// <returns></returns>
        public string BuildChangeFieldStatement(HiTable hiTable, HiColumn hiColumn, TabFieldAction tabFieldAction)
        {
            string _fieldsql = BuildFieldStatement(hiTable, hiColumn);

            var rtn = Tool.RegexGrpOrReplace(@"" + dbConfig.Field_Split + @"{1}\s*$", _fieldsql);
            if (rtn.Item1)
            {
                _fieldsql = rtn.Item3;
            }

            string _changesql = string.Empty;
            if (tabFieldAction == TabFieldAction.ADD)
                _changesql = dbConfig.Add_Column.Replace("[$TabName$]", $"{dbConfig.Table_Pre}{hiTable.TabName}{dbConfig.Table_After}").Replace("[$TempColumn$]", _fieldsql);
            else if (tabFieldAction == TabFieldAction.DELETE)
                _changesql = dbConfig.Del_Column.Replace("[$TabName$]", $"{dbConfig.Table_Pre}{hiTable.TabName}{dbConfig.Table_After}").Replace("[$FieldName$]", hiColumn.FieldName);
            else if (tabFieldAction == TabFieldAction.MODI)
                _changesql = dbConfig.Modi_Column.Replace("[$TabName$]", $"{dbConfig.Table_Pre}{hiTable.TabName}{dbConfig.Table_After}").Replace("[$TempColumn$]", _fieldsql);
            else if (tabFieldAction == TabFieldAction.RENAME)
            {
                var fieldNameFind = dbConfig.Field_Pre + hiColumn.FieldName + dbConfig.Field_After;
                var fieldIndex = _fieldsql.IndexOf(fieldNameFind) + fieldNameFind.Length;
                _fieldsql = _fieldsql.Substring(0, fieldIndex) + $"     {dbConfig.Field_Pre}{hiColumn.ReFieldName}{dbConfig.Field_After} " + _fieldsql.Substring(fieldIndex);
                //字段重命名
                _changesql = dbConfig.Re_Column.Replace("[$TabName$]", $"{dbConfig.Table_Pre}{hiTable.TabName}{dbConfig.Table_After}").Replace("[$TempColumn$]", _fieldsql);
            }
            else
                return "";


            return _changesql;
        }
        /// <summary>
        /// 生成字段语句
        /// </summary>
        /// <param name="hiColumn"></param>
        /// <returns></returns>
        public string BuildFieldStatement(HiTable hiTable, HiColumn hiColumn)
        {
            string _str_temp_field = "";
            if (dbConfig.DbMapping.ContainsKey(hiColumn.FieldType))
            {
                if (dbConfig.FieldTempMapping.ContainsKey(dbConfig.DbMapping[hiColumn.FieldType].ToString()))
                {
                    _str_temp_field = dbConfig.FieldTempMapping[dbConfig.DbMapping[hiColumn.FieldType].ToString()].ToString();

                    switch (dbConfig.DbMapping[hiColumn.FieldType].ToString())
                    {
                        case "nvarchar":
                        case "varchar":
                        case "nchar":
                        case "char":
                            _str_temp_field = _str_temp_field.Replace("[$FieldName$]", hiColumn.FieldName)
                                .Replace("[$FieldLen$]", hiColumn.FieldLen.ToString())
                                .Replace("[$IsNull$]", hiColumn.IsPrimary ? "NOT NULL" : hiColumn.IsNull == true ? "NULL" : "NOT NULL")
                                .Replace("[$Default$]", hiColumn.IsPrimary ? "" : GetDbDefault(hiColumn))
                                .Replace("[$EXTEND$]", hiTable.TableType == TableType.Var && hiColumn.IsPrimary ? "primary key" : "")
                                ;
                            break;
                        case "int":
                        case "bigint":
                            _str_temp_field = _str_temp_field.Replace("[$FieldName$]", hiColumn.FieldName)
                                .Replace("[$IsIdentity$]", hiColumn.IsIdentity ? " AUTO_INCREMENT " : "")
                                .Replace("[$IsNull$]", hiColumn.IsPrimary ? "NOT NULL" : hiColumn.IsIdentity ? "NOT NULL" : hiColumn.IsNull == true ? "NULL" : "NOT NULL")
                                .Replace("[$Default$]", hiColumn.IsPrimary || hiColumn.IsIdentity ? "" : GetDbDefault(hiColumn))
                                .Replace("[$EXTEND$]", hiTable.TableType == TableType.Var && hiColumn.IsPrimary ? "primary key" : "")
                                ;
                            break;
                        case "smallint":
                            _str_temp_field = _str_temp_field.Replace("[$FieldName$]", hiColumn.FieldName)
                                .Replace("[$IsNull$]", hiColumn.IsPrimary ? "" : hiColumn.IsNull == true ? "NULL" : "NOT NULL")
                                .Replace("[$EXTEND$]", hiTable.TableType == TableType.Var && hiColumn.IsPrimary ? "primary key" : "")
                                ;
                            break;
                        case "decimal":
                            _str_temp_field = _str_temp_field.Replace("[$FieldName$]", hiColumn.FieldName)
                                .Replace("[$FieldLen$]", hiColumn.FieldLen.ToString() == "0" ? "18" : hiColumn.FieldLen.ToString())
                                .Replace("[$FieldDec$]", hiColumn.FieldDec.ToString())
                                .Replace("[$IsNull$]", hiColumn.IsIdentity ? "NOT NULL" : hiColumn.IsNull == true ? "NULL" : "NOT NULL")
                                .Replace("[$Default$]", hiColumn.IsPrimary || hiColumn.IsIdentity ? "" : GetDbDefault(hiColumn))
                                .Replace("[$EXTEND$]", hiTable.TableType == TableType.Var && hiColumn.IsPrimary ? "primary key" : "")
                                ;
                            break;
                        case "date":
                        case "datetime":
                            _str_temp_field = _str_temp_field.Replace("[$FieldName$]", hiColumn.FieldName)
                                .Replace("[$IsNull$]", hiColumn.IsIdentity ? "NOT NULL" : hiColumn.IsNull == true ? "NULL" : "NOT NULL")
                                .Replace("[$Default$]", hiColumn.IsPrimary ? "" : GetDbDefault(hiColumn))
                                .Replace("[$EXTEND$]", hiTable.TableType == TableType.Var && hiColumn.IsPrimary ? "primary key" : "")
                                ;
                            break;
                        case "image":
                            _str_temp_field = _str_temp_field.Replace("[$FieldName$]", hiColumn.FieldName)
                               .Replace("[$IsNull$]", hiColumn.IsIdentity ? "NOT NULL" : hiColumn.IsNull == true ? "NULL" : "NOT NULL")
                               .Replace("[$Default$]", hiColumn.IsPrimary ? "" : GetDbDefault(hiColumn))
                               .Replace("[$EXTEND$]", hiTable.TableType == TableType.Var && hiColumn.IsPrimary ? "primary key" : "")
                               ;
                            break;
                        case "uniqueidentifier":
                            _str_temp_field = _str_temp_field.Replace("[$FieldName$]", hiColumn.FieldName)
                               .Replace("[$IsNull$]", hiColumn.IsIdentity ? "NOT NULL" : hiColumn.IsNull == true ? "NULL" : "NOT NULL")
                               .Replace("[$Default$]", hiColumn.IsPrimary ? "" : GetDbDefault(hiColumn))
                               .Replace("[$EXTEND$]", hiTable.TableType == TableType.Var && hiColumn.IsPrimary ? "primary key" : "")
                               ;
                            break;
                        case "bit":
                            _str_temp_field = _str_temp_field.Replace("[$FieldName$]", hiColumn.FieldName)
                               .Replace("[$IsNull$]", hiColumn.IsIdentity ? "NOT NULL" : hiColumn.IsNull == true ? "NULL" : "NOT NULL")
                               .Replace("[$Default$]", hiColumn.IsPrimary ? "" : GetDbDefault(hiColumn))
                               .Replace("[$EXTEND$]", hiTable.TableType == TableType.Var && hiColumn.IsPrimary ? "primary key" : "");
                            break;
                        default:
                            throw new Exception($"字段类型[{dbConfig.DbMapping[hiColumn.FieldType].ToString()}]没有作处理");
                    }

                    _str_temp_field = _str_temp_field.Replace("[$COMMENT$]", $"COMMENT '{hiColumn.FieldDesc.ToSqlInject()}'");
                }
                else
                    throw new Exception($"字段类型[{dbConfig.DbMapping[hiColumn.FieldType].ToString()} 在SqlServer中没有配置字段模版");
            }
            else
                throw new Exception($"字段[{hiColumn.FieldName}] 对应的字段类型在SqlServer中没有做实现,帮该库不支持该类型");




            return _str_temp_field;
        }





        /// <summary>
        /// 通过底层SQL语句获取批定表的表结构信息
        /// （一定要符合HiSql）规范
        /// </summary>
        /// <param name="tabname"></param>
        /// <returns></returns>
        public DataTable GetTableDefinition(string tabname)
        {
            DataTable dt = Context.DBO.GetDataTable(dbConfig.Get_Table_Schema.Replace("[$TabName$]", tabname).Replace("[$Schema$]", this.Context.CurrentConnectionConfig.Schema));
            if (dt.Rows.Count == 0)
                throw new Exception($"表[{tabname}]不存在");
            return dt;

        }

        /// <summary>
        /// 根据表结构信息生成 插入表模型表的语句
        /// </summary>
        /// <param name="hiTable"></param>
        /// <param name="lstHiTable"></param>
        /// <returns></returns>
        public string BuildTabStructSql(HiTable hiTable, List<HiColumn> lstHiTable)
        {
            return inertTabStruct(hiTable, lstHiTable);
        }


        public int BuildTabCreate(TabInfo tabInfo)
        {
            string _sql = BuildTabCreateSql(tabInfo.TabModel, tabInfo.GetColumns);
            string v = this.Context.DBO.ExecCommand(_sql).ToString();
            int _effect = Convert.ToInt32(v);
            _effect=_effect > 1 ? 1 : _effect;
            return _effect;
        }
        /// <summary>
        /// 根据表结构信息生成底层创建表的语句
        /// </summary>
        /// <param name="hiTable"></param>
        /// <param name="lstHiTable"></param>
        /// <returns></returns>
        public string BuildTabCreateSql(HiTable hiTable, List<HiColumn> lstHiTable, bool isdrop = false)
        {
            string _temp_create = dbConfig.Table_Create;
            string _create_tabname = hiTable.TabName;
            if (hiTable != null)
            {
                if (hiTable.TableType == TableType.Var)  // && !hiTable.IsGlobalTemp && !hiTable.IsLocalTemp
                {
                    //hiTable.TabName = $"@{hiTable.TabName}";
                    _temp_create = dbConfig.Table_Declare_Table;
                }
                else if (hiTable.TableType == TableType.Local)
                {
                    //hiTable.TabName = $"#{hiTable.TabName}";
                    if (!isdrop)
                        _temp_create = dbConfig.Table_Global_Create;
                    else
                        _temp_create = dbConfig.Table_Local_Create;
                }
                else if (hiTable.TableType == TableType.Global)
                {
                    //hiTable.TabName = $"##{hiTable.TabName}";
                    if (!isdrop)
                        _temp_create = dbConfig.Table_Global_Create;
                    else
                        _temp_create = dbConfig.Table_Global_Create_Drop;
                    _create_tabname = hiTable.TabReName;
                }
                string keys = BuildKey(lstHiTable);
                if (!string.IsNullOrEmpty(keys))
                {
                    keys = dbConfig.Table_Key.Replace("[$TabName$]", hiTable.TabName)
                        .Replace("[$Keys$]", keys).Replace("[$ConnectID$]", this.Context.ConnectedId);
                }

                string _fields_str = BuildFieldStatment(hiTable, lstHiTable);
                if (hiTable.TableType == TableType.Var)
                {
                    _fields_str = _fields_str.Trim();
                    _fields_str = _fields_str.Substring(0, _fields_str.Length - 2);


                }
                _temp_create = _temp_create.Replace("[$Schema$]", Context.CurrentConnectionConfig.Schema)
                    .Replace("[$TabName$]", _create_tabname)
                    .Replace("[$Fields$]", _fields_str)
                    .Replace("[$Keys$]", hiTable.TableType == TableType.Var ? "" : keys)
                    .Replace("[$Primary$]", hiTable.TableType == TableType.Var ? "" : dbConfig.Table_Key3);
                if (hiTable.TabName.Substring(0, 1) != "#" && (hiTable.TabName.Substring(0, 1) != "@"))
                {
                    //带描述的创建创建
                    StringBuilder sb_sqlcomment = new StringBuilder();
                    foreach (HiColumn hiColumn in lstHiTable)
                    {
                        sb_sqlcomment.AppendLine(
                            dbConfig.Field_Comment
                                .Replace("[$FieldDesc$]", hiColumn.FieldDesc)
                                .Replace("[$Schema$]", Context.CurrentConnectionConfig.Schema)
                                .Replace("[$TabName$]", _create_tabname)
                                .Replace("[$FieldName$]", hiColumn.FieldName)
                            );
                    }

                    _temp_create = _temp_create.Replace("[$Comment$]", sb_sqlcomment.ToString())
                        .Replace("[$TabStruct$]", inertTabStruct(hiTable, lstHiTable))
                        ;
                    //_temp_create = new StringBuilder().AppendLine(_temp_create).AppendLine(sb_sqlcomment.ToString())
                    //    .AppendLine(inertTabStruct(hiTable, lstHiTable))
                    //    .ToString();
                }
                return _temp_create;
            }
            else
                return "";
        }
        public string BuildTabCreateSql(TabInfo tabInfo)
        {
            return BuildTabCreateSql(tabInfo.TabModel, tabInfo.Columns);
        }

        public string BuildFieldDefaultValue(HiColumn hiColumn)
        {
            string _setsql = "";// dbConfig.Set_Default;

            string _value = GetDbDefault(hiColumn);

            _setsql = _setsql.Replace("[$TabName$]", hiColumn.TabName)
                .Replace("[$FieldName$]", hiColumn.FieldName)
                .Replace("[$Schema$]", this.Context.CurrentConnectionConfig.Schema)
                .Replace("[$KEY$]", (hiColumn.TabName + hiColumn.FieldName).ToHash())
                .Replace("[$DefValue$]", _value.Replace("'", "''"))
                ;

            return _setsql;
        }

        /// <summary>
        /// 生成创建表字段的语句
        /// </summary>
        /// <param name="lstColumn"></param>
        /// <returns></returns>
        public string BuildFieldStatment(HiTable hiTable, List<HiColumn> lstColumn)
        {
            StringBuilder sb_sql = new StringBuilder();
            foreach (HiColumn hiColumn in lstColumn)
            {
                sb_sql.AppendLine(BuildFieldStatement(hiTable, hiColumn));
            }
            return sb_sql.ToString();
        }

        /// <summary>
        /// 解析hisql中间语言语法
        /// </summary>
        /// <param name="TableList"></param>
        /// <param name="dictabinfo"></param>
        /// <param name="Fields"></param>
        /// <param name="lstresult"></param>
        /// <param name="issubquery"></param>
        /// <returns></returns>
        public string BuilderWhereSql(List<TableDefinition> TableList, Dictionary<string, TabInfo> dictabinfo, List<FieldDefinition> Fields, List<WhereResult> lstresult, bool issubquery)
        {
            StringBuilder sb_sql = new StringBuilder();
            if (lstresult != null && lstresult.Count() > 0)
            {
                foreach (WhereResult whereResult in lstresult)
                {
                    //字段值
                    if (whereResult.SType == StatementType.FieldValue)
                    {
                        if (whereResult.Result.ContainsKey("fields"))
                        {

                            FieldDefinition field = new FieldDefinition(whereResult.Result["fields"].ToString());
                            HiColumn hiColumn = CheckField(TableList, dictabinfo, Fields, field);
                            
                            if (hiColumn != null)
                            {
                                sb_sql.Append($"{dbConfig.Table_Pre}{ Tool.GetDbTabName(hiColumn, field)}{dbConfig.Table_After}.{dbConfig.Field_Pre}{ Tool.GetDbFieldName(hiColumn, field)}{dbConfig.Field_After}");
                                string _value = whereResult.Result["value"].ToString();
                                if (hiColumn != null)
                                {
                                    sb_sql.Append($" {whereResult.Result["op"].ToString()} ");
                                    sb_sql.Append(getSingleValue(issubquery, hiColumn, _value));
                                }
                            }
                            else
                                throw new Exception($"字段[{whereResult.Result["fields"].ToString()}]出现错误");
                        }
                        else
                        {
                            throw new Exception($"未能识别的解析结果");
                        }
                    }
                    //add by tgm date:2022.5.19 新增字段与字段的条件
                    else if (whereResult.SType == StatementType.Field)
                    {
                        if (whereResult.Result.ContainsKey("fields") && whereResult.Result.ContainsKey("rfields"))
                        {
                            FieldDefinition field = new FieldDefinition(whereResult.Result["fields"].ToString());
                            HiColumn hiColumn = CheckField(TableList, dictabinfo, Fields, field);
                            if (hiColumn == null)
                                throw new Exception($"字段[{whereResult.Result["fields"].ToString()}]出现错误");

                            FieldDefinition rfield = new FieldDefinition(whereResult.Result["rfields"].ToString());
                            HiColumn rhiColumn = CheckField(TableList, dictabinfo, Fields, rfield);
                            if (rhiColumn == null)
                                throw new Exception($"字段[{whereResult.Result["rfields"].ToString()}]出现错误");

                            if (hiColumn != null && rhiColumn != null)
                            {
                                sb_sql.Append($"{dbConfig.Table_Pre}{Tool.GetDbTabName(hiColumn, field)}{dbConfig.Table_After}.{dbConfig.Field_Pre}{Tool.GetDbFieldName(hiColumn, field)}{dbConfig.Field_After}");
                                sb_sql.Append($" {whereResult.Result["op"].ToString()} ");
                                sb_sql.Append($"{dbConfig.Table_Pre}{Tool.GetDbTabName(rhiColumn, rfield)}{dbConfig.Table_After}.{dbConfig.Field_Pre}{Tool.GetDbFieldName(rhiColumn, rfield)}{dbConfig.Field_After}");
                            }
                        }
                        else
                            throw new Exception($"未能识别解析结果");
                    }
                    else if (whereResult.SType == StatementType.SubCondition)
                    {
                        //子条件中可能会嵌套多个深度子条件
                        if (whereResult.Result != null)
                        {
                            WhereParse whereParse = new WhereParse(whereResult.Result["content"].ToString());
                            sb_sql.Append($" ({BuilderWhereSql(TableList, dictabinfo, Fields, whereParse.Result, issubquery)})");
                        }
                    }
                    else if (whereResult.SType == StatementType.FieldBetweenValue)
                    {
                        if (whereResult.Result.ContainsKey("fields"))
                        {
                            if (!whereResult.Result.ContainsKey("value") || !whereResult.Result.ContainsKey("value2"))
                                throw new Exception($"未能识别的语法 between  and 的值");


                            FieldDefinition field = new FieldDefinition(whereResult.Result["fields"].ToString());
                            HiColumn hiColumn = CheckField(TableList, dictabinfo, Fields, field);
                            sb_sql.Append($"{dbConfig.Table_Pre}{Tool.GetDbTabName(hiColumn, field)}{dbConfig.Table_After}.{dbConfig.Field_Pre}{Tool.GetDbFieldName(hiColumn, field)}{dbConfig.Field_After}");

                            if (hiColumn != null)
                            {
                                string _value = whereResult.Result["value"].ToString();
                                string _value2 = whereResult.Result["value2"].ToString();
                                if (hiColumn != null)
                                {
                                    sb_sql.Append($" {whereResult.Result["op"].ToString()} ");
                                    sb_sql.Append(getSingleValue(issubquery, hiColumn, _value));
                                    sb_sql.Append(" and ");
                                    sb_sql.Append(getSingleValue(issubquery, hiColumn, _value2));
                                }
                            }
                            else
                                throw new Exception($"字段[{whereResult.Result["fields"].ToString()}]出现错误");
                        }
                        else
                        {
                            throw new Exception($"未能识别的解析结果");
                        }


                    }
                    else if (whereResult.SType == StatementType.Symbol)
                    {
                        sb_sql.Append($" {whereResult.Result["mode"].ToString()} ");
                    }
                    else if (whereResult.SType == StatementType.In)
                    {
                        //解析in条件
                        // in 一般有三种写法
                        // 1  a.user in ('tgm','tansar')
                        // 2  a.user in (select User from UserAadmin)
                        // 3  a.utype in (1,2,3)
                        if (whereResult.Result.ContainsKey("fields") && whereResult.Result.ContainsKey("fields"))
                        {
                            FieldDefinition field = new FieldDefinition(whereResult.Result["fields"].ToString());
                            HiColumn hiColumn = CheckField(TableList, dictabinfo, Fields, field);
                            sb_sql.Append($"{dbConfig.Table_Pre}{Tool.GetDbTabName(hiColumn, field)}{dbConfig.Table_After}.{dbConfig.Field_Pre}{Tool.GetDbFieldName(hiColumn, field)}{dbConfig.Field_After}");

                            if (hiColumn == null)
                                throw new Exception($"字段[{whereResult.Result["fields"].ToString()}]出现错误");

                            string _content = whereResult.Result["content"].ToString();
                            if (Tool.RegexMatch(AST.SelectParse.Constants.REG_SELECT, _content))
                            {


                                //检测出是select 语句
                                SelectParse selectParse = new SelectParse(_content, Context, true);
                                if (!Tool.CheckQueryField(selectParse.Fields).Item1)
                                {
                                    throw new Exception($"语句[{_content}] 附近语法错误 select in 中的子查询仅允许查询一个字段");
                                }
                                string _sql = selectParse.Query.ToSql();


                                sb_sql.Append($" {whereResult.Result["symbol"].ToString()} ");
                                sb_sql.Append($"({_sql})");
                            }
                            else if (Tool.RegexMatch(AST.SelectParse.Constants.REG_INCHARVALUE, _content))
                            {
                                //a.user in ('tgm','tansar')

                                var diclst = Tool.RegexGrps(AST.SelectParse.Constants.REG_INCHARVALUE, _content, "content");
                                if (diclst.Count > 0)
                                {
                                    string _value = diclst.ToArray().ToSqlIn();
                                    sb_sql.Append($" {whereResult.Result["symbol"].ToString()} ");
                                    sb_sql.Append($"({_value})");
                                }
                                else
                                    throw new Exception($"语句[{whereResult.Result["fields"].ToString()}] 附近有语法错误,无匹配的in范围值");



                            }
                            else if (Tool.RegexMatch(AST.SelectParse.Constants.REG_INVALUE, _content))
                            {
                                //a.utype in (1,2,3)
                                var diclst = Tool.RegexGrps(AST.SelectParse.Constants.REG_INVALUE, _content, "content");
                                if (diclst.Count > 0)
                                {
                                    string _value = diclst.ToArray().ToSqlIn(false);
                                    sb_sql.Append($" {whereResult.Result["symbol"].ToString()} ");
                                    sb_sql.Append($"({_value})");
                                }
                                else
                                    throw new Exception($"语句[{whereResult.Result["fields"].ToString()}] 附近有语法错误,无匹配的in范围值");

                            }
                            else
                                throw new Exception($"语句[{_content}]附近有语法错误");



                        }
                        else
                            throw new Exception("未能识别的解析结果");

                    }
                    else if (whereResult.SType == StatementType.FieldTemplate)
                    {
                        //add by tgm date:2022.6.11 解析模版语法
                        if (whereResult.Result.ContainsKey("fields"))
                        {
                            FieldDefinition field = new FieldDefinition(whereResult.Result["fields"].ToString());
                            HiColumn hiColumn = CheckField(TableList, dictabinfo, Fields, field);
                            sb_sql.Append($"{dbConfig.Table_Pre}{Tool.GetDbTabName(hiColumn, field)}{dbConfig.Table_After}.{dbConfig.Field_Pre}{Tool.GetDbFieldName(hiColumn, field)}{dbConfig.Field_After}");

                            if (hiColumn == null)
                                throw new Exception($"字段[{whereResult.Result["fields"].ToString()}]出现错误");


                            FilterDefinition _filter = new FilterDefinition(FilterType.CONDITION);
                            _filter.Name = whereResult.Result["fields"];

                            switch (whereResult.Result["op"].ToString().ToLower())
                            {
                                case ">":
                                    _filter.OpFilter = OperType.GT;
                                    break;

                                case "<":
                                    _filter.OpFilter = OperType.LT;
                                    break;
                                case "=":
                                    _filter.OpFilter = OperType.EQ;
                                    break;
                                case ">=":
                                    _filter.OpFilter = OperType.GE;
                                    break;
                                case "<=":
                                    _filter.OpFilter = OperType.LE;
                                    break;
                                case "<>":
                                    _filter.OpFilter = OperType.NE;
                                    break;
                                case "!=":
                                    _filter.OpFilter = OperType.NE;
                                    break;
                                default:
                                    throw new Exception($"{HiSql.Constants.HiSqlSyntaxError} 附近语法错误[{whereResult.Statement}] 使用模板语法时不支持操作符[{whereResult.Result["op"]}]");

                            }


                            _filter.OpFilter = OperType.EQ;
                            _filter.Value = whereResult.Result["value"];

                            sb_sql.Append($" {whereResult.Result["op"].ToString()} ");
                            sb_sql.Append(getSingleValue(issubquery, hiColumn, _filter, _filter.Value, TableList, dictabinfo));
                        }

                    }
                    else
                        throw new Exception("暂时不支持该语法");
                }
            }

            return sb_sql.ToString();
        }

        /// <summary>
        /// 生成WHERE语句
        /// </summary>
        /// <returns></returns>
        public string BuilderWhereSql(List<TableDefinition> TableList, Dictionary<string, TabInfo> dictabinfo, List<FieldDefinition> Fields, List<FilterDefinition> Wheres, bool issubquery)
        {
            StringBuilder sb_where = new StringBuilder();
            int _idx = 0;
            foreach (FilterDefinition filterDefinition in Wheres)
            {
                if (filterDefinition.FilterType == FilterType.BRACKET_LEFT)
                {
                    sb_where.Append("(");
                }
                else if (filterDefinition.FilterType == FilterType.BRACKET_RIGHT)
                {
                    sb_where.Append(")");
                    if (_idx < Wheres.Count - 1)
                    {
                        if (Wheres[_idx + 1].FilterType == FilterType.LOGI)
                        {
                            sb_where.Append($" {Wheres[_idx + 1].LogiType} ");
                        }
                        else
                        {
                            if (Wheres[_idx + 1].FilterType != FilterType.BRACKET_RIGHT)
                                throw new Exception($"在括号[)]后如果还有其它的条件则必需要有逻辑操作符[and|or]");
                        }
                    }
                }
                else if (filterDefinition.FilterType == FilterType.CONDITION)
                {
                    HiColumn hiColumn = CheckField(TableList, dictabinfo, Fields, filterDefinition.Field);
                    if (hiColumn == null)
                        throw new Exception($"字段[{filterDefinition.Field.AsFieldName}]在表[{filterDefinition.Field.AsTabName}]中不存在");

                    sb_where.Append($"{dbConfig.Table_Pre}{Tool.GetDbTabName(hiColumn, filterDefinition.Field)}{dbConfig.Table_After}.{dbConfig.Field_Pre}{Tool.GetDbFieldName(hiColumn, filterDefinition.Field)}{dbConfig.Field_After}");
                    switch (filterDefinition.OpFilter)
                    {
                        case OperType.EQ:
                            sb_where.Append(" = ");
                            sb_where.Append(getSingleValue(issubquery, hiColumn, filterDefinition, filterDefinition.Value, TableList, dictabinfo));
                            break;
                        case OperType.GT:
                            sb_where.Append(" > ");
                            sb_where.Append(getSingleValue(issubquery, hiColumn, filterDefinition, filterDefinition.Value, TableList, dictabinfo));
                            break;
                        case OperType.LT:
                            sb_where.Append(" < ");
                            sb_where.Append(getSingleValue(issubquery, hiColumn, filterDefinition, filterDefinition.Value, TableList, dictabinfo));
                            break;
                        case OperType.GE:
                            sb_where.Append(" >= ");
                            sb_where.Append(getSingleValue(issubquery, hiColumn, filterDefinition, filterDefinition.Value, TableList, dictabinfo));
                            break;
                        case OperType.LE:
                            sb_where.Append(" <= ");
                            sb_where.Append(getSingleValue(issubquery, hiColumn, filterDefinition, filterDefinition.Value, TableList, dictabinfo));
                            break;
                        case OperType.IN:
                            sb_where.Append(" in ");
                            sb_where.Append($"({getInValue(issubquery, hiColumn, filterDefinition, filterDefinition.Value)})");
                            break;
                        case OperType.NOIN:
                            sb_where.Append(" not in ");
                            sb_where.Append($"({getInValue(issubquery, hiColumn, filterDefinition, filterDefinition.Value)})");
                            break;
                        case OperType.BETWEEN:
                            sb_where.Append(" BETWEEN ");
                            sb_where.Append(getBetweenValue(hiColumn, filterDefinition, filterDefinition.Value));
                            break;

                        case OperType.LIKE:
                            //模糊查询
                            sb_where.Append(" like ");
                            sb_where.Append(getLikeValue(issubquery, hiColumn, filterDefinition, filterDefinition.Value));
                            break;
                        case OperType.NOLIKE:
                            //模糊查询
                            sb_where.Append(" not like ");
                            sb_where.Append(getLikeValue(issubquery, hiColumn, filterDefinition, filterDefinition.Value));
                            break;
                    }
                    if (_idx < Wheres.Count - 1)
                    {

                        if (Wheres[_idx + 1].FilterType == FilterType.CONDITION)
                        {
                            if (filterDefinition.LogiType == LogiType.AND)
                                sb_where.Append(" and ");
                            else
                                sb_where.Append(" or ");
                        }
                        else if (Wheres[_idx + 1].FilterType == FilterType.LOGI)
                        {
                            sb_where.Append($" {Wheres[_idx + 1].LogiType} ");
                        }
                    }
                }

                _idx++;
            }


            return sb_where.ToString();
        }


        /// <summary>
        /// 生成查询去重命令
        /// </summary>
        /// <returns></returns>
        public string BuilderDistinct()
        {
            return $"distinct";
            //return $"{dbConfig.Field_Pre}distinct{dbConfig.Field_After}";
        }

        /// <summary>
        /// 生成join语句
        /// </summary>
        /// <param name="TableList"></param>
        /// <param name="dictabinfo"></param>
        /// <param name="Fields"></param>
        /// <param name="Joins"></param>
        /// <returns></returns>
        public string BuildJoinSql(List<TableDefinition> TableList, Dictionary<string, TabInfo> dictabinfo, List<FieldDefinition> Fields, List<JoinDefinition> Joins, bool issubquery = false)
        {
            StringBuilder sb_join = new StringBuilder();
            foreach (JoinDefinition joinDefinition in Joins)
            {
                if (joinDefinition.JoinType == JoinType.Inner)
                    sb_join.Append($" inner join");
                else if (joinDefinition.JoinType == JoinType.Left)
                    sb_join.Append($" left  join");
                else if (joinDefinition.JoinType == JoinType.Right)
                    sb_join.Append($" outer join");
                sb_join.Append($" {dbConfig.Table_Pre}{dictabinfo[joinDefinition.Right.TabName].TabModel.TabName}{dbConfig.Table_After} as {dbConfig.Table_Pre}{joinDefinition.Right.AsTabName.ToLower()}{dbConfig.Table_After}");
                sb_join.Append(" on ");

                if (!joinDefinition.IsFilter && joinDefinition.Filter == null)
                {

                    if (joinDefinition.Right != null && joinDefinition.JoinOn.Count > 0)
                    {

                        foreach (JoinOnFilterDefinition joinOnFilterDefinition in joinDefinition.JoinOn)
                        {
                            if (joinOnFilterDefinition.Left != null && joinOnFilterDefinition.Right != null)
                            {

                                HiColumn hiColumnL = CheckField(TableList, dictabinfo, Fields, joinOnFilterDefinition.Left);
                                HiColumn hiColumnR = CheckField(TableList, dictabinfo, Fields, joinOnFilterDefinition.Right);

                                if (hiColumnL.FieldType != hiColumnR.FieldType)
                                {
                                    throw new Exception($"join 关联表[{joinDefinition.Right.AsTabName}] 条件字段[{hiColumnL.FieldName}]与[{hiColumnR.FieldName}]类型不一致 会导致性能问题");
                                }
                                if (hiColumnL.FieldLen != hiColumnR.FieldLen)
                                {
                                    throw new Exception($"join 关联表[{joinDefinition.Right.AsTabName}] 条件字段[{hiColumnL.FieldName}]与[{hiColumnR.FieldName}]长度不一致 会导致性能问题");
                                }
                                sb_join.Append($"{dbConfig.Table_Pre}{joinOnFilterDefinition.Left.AsTabName}{dbConfig.Table_After}.{dbConfig.Field_Pre}{joinOnFilterDefinition.Left.AsFieldName}{dbConfig.Field_After}={dbConfig.Table_Pre}{joinOnFilterDefinition.Right.AsTabName}{dbConfig.Table_After}.{dbConfig.Field_Pre}{joinOnFilterDefinition.Right.AsFieldName}{dbConfig.Field_After}");
                            }
                        }
                        //sb_join.AppendLine("");
                    }
                }
                else
                {
                    //join on的语句与where语法一样
                    if (joinDefinition.Filter != null)
                    {
                        //通过hisql语写实现的 on条件
                        if (joinDefinition.Filter.IsHiSqlWhere && !string.IsNullOrEmpty(joinDefinition.Filter.HiSqlWhere))
                            sb_join.Append(BuilderWhereSql(TableList, dictabinfo, Fields, joinDefinition.Filter.WhereParse.Result, issubquery));
                        else  //通过结构化filter对象生成的on条件
                            sb_join.Append(BuilderWhereSql(TableList, dictabinfo, Fields, joinDefinition.Filter.Elements, issubquery));
                    }
                    else
                        throw new Exception($"{Constants.HiSqlSyntaxError} [{joinDefinition.Filter.HiSqlWhere}]附近出现语法错误");
                }

            }
            return sb_join.ToString();
        }


        /// <summary>
        /// 生成分组SQL
        /// </summary>
        /// <param name="TableList"></param>
        /// <param name="dictabinfo"></param>
        /// <param name="Fields"></param>
        /// <param name="Groups"></param>
        /// <param name="issubquery"></param>
        /// <returns></returns>
        public string BuildGroupSql(List<TableDefinition> TableList, Dictionary<string, TabInfo> dictabinfo, List<FieldDefinition> Fields, List<GroupDefinition> Groups, bool issubquery)
        {
            StringBuilder sb_group = new StringBuilder();
            int _idx = 0;
            foreach (GroupDefinition groupDefinition in Groups)
            {
                if (!issubquery)
                {

                    HiColumn hiColumn = CheckField(TableList, dictabinfo, Fields, groupDefinition.Field);

                    sb_group.Append($"{dbConfig.Table_Pre}{Tool.GetDbTabName(hiColumn, groupDefinition.Field)}{dbConfig.Table_After}.{dbConfig.Field_Pre}{Tool.GetDbFieldName(hiColumn, groupDefinition.Field)}{dbConfig.Field_After}");

                }
                else
                {
                    sb_group.Append($"{dbConfig.Field_Pre}{groupDefinition.Field.AsFieldName}{dbConfig.Field_After}");
                }
                if (_idx < Groups.Count - 1)
                    sb_group.Append(",");
                _idx++;
            }

            return sb_group.ToString();
        }

        /// <summary>
        /// 生成排序SQL
        /// </summary>
        /// <param name="sb_group"></param>
        /// <param name="TableList"></param>
        /// <param name="dictabinfo"></param>
        /// <param name="Fields"></param>
        /// <param name="Sorts"></param>
        /// <param name="Groups"></param>
        /// <param name="IsPage"></param>
        /// <param name="CurrentPage"></param>
        /// <param name="PageSize"></param>
        /// <param name="issubquery"></param>
        /// <returns></returns>
        public string BuildOrderBySql(ref StringBuilder sb_group, Dictionary<string, TabInfo> dictabinfo, QueryProvider queryProvider)
        {
            int _idx = 0;
            bool _flag = false;
            StringBuilder sb_sort = new StringBuilder();
            foreach (SortByDefinition sortByDefinition in queryProvider.Sorts)
            {
                if (!queryProvider.IsMultiSubQuery)
                {
                    HiColumn hiColumn = CheckField(queryProvider.TableList, dictabinfo, queryProvider.Fields, sortByDefinition.Field);

                    _flag = false;
                    sb_sort.Append($"{dbConfig.Table_Pre}{Tool.GetDbTabName(hiColumn, sortByDefinition.Field)}{dbConfig.Table_After}.{dbConfig.Field_Pre}{Tool.GetDbFieldName(hiColumn, sortByDefinition.Field)}{dbConfig.Field_After}");
                    if (queryProvider.Groups.Count > 0)
                    {
                        if (sb_group.ToString().ToLower().IndexOf($"{dbConfig.Table_Pre}{Tool.GetDbTabName(hiColumn, sortByDefinition.Field)}{dbConfig.Table_After}.{dbConfig.Field_Pre}{Tool.GetDbFieldName(hiColumn, sortByDefinition.Field)}{dbConfig.Field_After}".ToLower()) >= 0)
                        {
                            _flag = true;
                        }
                        else
                        {
                            if (queryProvider.Groups.Count > 0)
                            {
                                sb_group.Append(",");
                            }
                            sb_group.Append($"{dbConfig.Table_Pre}{Tool.GetDbTabName(hiColumn, sortByDefinition.Field)}{dbConfig.Table_After}.{dbConfig.Field_Pre}{Tool.GetDbFieldName(hiColumn, sortByDefinition.Field)}{dbConfig.Field_After}");
                        }
                    }
                }
                else
                {
                    sb_group.Append($"{dbConfig.Field_Pre}{sortByDefinition.Field.AsFieldName}{dbConfig.Field_After}");
                }

                if (sortByDefinition.IsAsc)
                    sb_sort.Append(" ASC");
                else
                    sb_sort.Append(" DESC");

                if (_idx < queryProvider.Sorts.Count - 1)
                {
                    sb_sort.Append(",");
                }
                _idx++;
            }

            if (queryProvider.IsPage && !queryProvider.IsMultiSubQuery)
            {
                //如果启用了分页,有分组，但没有排序字段则按分组字段进行排序
                if (queryProvider.Sorts.Count == 0 && queryProvider.Groups.Count > 0)
                {
                    _idx = 0;
                    foreach (GroupDefinition groupDefinition in queryProvider.Groups)
                    {
                        sb_sort.Append($"{dbConfig.Table_Pre}{groupDefinition.Field.AsTabName}{dbConfig.Table_After}.{dbConfig.Field_Pre}{groupDefinition.Field.AsFieldName}{dbConfig.Field_After}");
                        sb_sort.Append(" ASC");
                        if (_idx < queryProvider.Groups.Count - 1)
                        {
                            sb_sort.Append(",");
                        }
                        _idx++;
                    }
                }


                if (queryProvider.CurrentPage < 0 || queryProvider.PageSize <= 0)
                    throw new Exception($"启用了分页但Skip({queryProvider.CurrentPage})或Take({queryProvider.PageSize})值指定错误!");
            }
            return sb_sort.ToString();
        }

        /// <summary>
        /// 解析having
        /// </summary>
        /// <param name="TableList"></param>
        /// <param name="dictabinfo"></param>
        /// <param name="Fields"></param>
        /// <param name="lstresult"></param>
        /// <param name="issubquery"></param>
        /// <returns></returns>
        public string BuildHavingSql(List<TableDefinition> TableList, Dictionary<string, TabInfo> dictabinfo, List<FieldDefinition> Fields, List<HavingResult> lstresult, bool issubquery)
        {
            StringBuilder sb_sql = new StringBuilder();
            if (lstresult != null && lstresult.Count() > 0)
            {
                foreach (HavingResult whereResult in lstresult)
                {
                    //字段值
                    if (whereResult.SType == StatementType.FieldValue)
                    {
                        FieldDefinition field = new FieldDefinition(whereResult.Result["left"].ToString(), true);

                        HiColumn hiColumn = CheckField(TableList, dictabinfo, Fields, field, true);


                        if (field.IsFun)
                        {
                            if (Tool.IsDecimal(whereResult.Result["value"].ToString()))
                            {
                                //表示函数
                                switch (field.DbFun)
                                {
                                    case DbFunction.AVG:
                                        sb_sql.Append($"avg({dbConfig.Field_Pre}{field.FieldName}{dbConfig.Field_After}) {whereResult.Result["op"].ToString()} {whereResult.Result["value"].ToString()}");
                                        break;
                                    case DbFunction.COUNT:
                                        sb_sql.Append($"count(*) {whereResult.Result["op"].ToString()} {whereResult.Result["value"].ToString()}");
                                        break;
                                    case DbFunction.MAX:
                                        sb_sql.Append($"max({dbConfig.Field_Pre}{field.FieldName}{dbConfig.Field_After}) {whereResult.Result["op"].ToString()} {whereResult.Result["value"].ToString()}");
                                        break;
                                    case DbFunction.MIN:
                                        sb_sql.Append($"min({dbConfig.Field_Pre}{field.FieldName}{dbConfig.Field_After}) {whereResult.Result["op"].ToString()} {whereResult.Result["value"].ToString()}");
                                        break;
                                    case DbFunction.SUM:
                                        sb_sql.Append($"sum({dbConfig.Field_Pre}{field.FieldName}{dbConfig.Field_After}) {whereResult.Result["op"].ToString()} {whereResult.Result["value"].ToString()}");
                                        break;
                                    default:
                                        break;
                                }
                            }
                            else
                                throw new Exception($"Having字段[{field.FieldName}] 值 [{whereResult.Result["value"].ToString()}] 非数字有注入风险");
                        }
                        else
                        {
                            if (hiColumn != null)
                                sb_sql.Append($"{dbConfig.Table_Pre}{field.AsTabName}{dbConfig.Table_After}.{dbConfig.Table_Pre}{field.AsFieldName}{dbConfig.Table_After} {whereResult.Result["op"].ToString()} '{whereResult.Result["value"].ToString()}'");
                            else
                                throw new Exception($"Having字段[{whereResult.Result["left"]}]在表中不存在");

                        }

                        //if (whereResult.Result.ContainsKey("fields"))
                        //{

                        //FieldDefinition field = new FieldDefinition(whereResult.Result["fields"].ToString());
                        //HiColumn hiColumn = CheckField(TableList, dictabinfo, Fields, field);
                        //sb_sql.Append($"{dbConfig.Table_Pre}{field.AsTabName}{dbConfig.Table_After}.{dbConfig.Table_Pre}{field.AsFieldName}{dbConfig.Table_After}");

                        //if (hiColumn != null)
                        //{
                        //    string _value = whereResult.Result["value"].ToString();
                        //    if (hiColumn != null)
                        //    {
                        //        sb_sql.Append($" {whereResult.Result["op"].ToString()} ");
                        //        sb_sql.Append(getSingleValue(issubquery, hiColumn, _value));
                        //    }
                        //}
                        //else
                        //    throw new Exception($"字段[{whereResult.Result["fields"].ToString()}]出现错误");
                        //}
                        //else
                        //{
                        //    throw new Exception($"未能识别的解析结果");
                        //}
                    }
                    else if (whereResult.SType == StatementType.SubCondition)
                    {
                        //子条件中可能会嵌套多个深度子条件
                        throw new Exception($"暂时不支持该写法[{whereResult.Result["0"].ToString()}]");
                    }
                    else if (whereResult.SType == StatementType.Symbol)
                    {
                        sb_sql.Append($" {whereResult.Result["mode"].ToString()} ");
                    }
                    else if (whereResult.SType == StatementType.In)
                    {
                        throw new Exception($"暂不支持in写法");

                    }

                    else
                        throw new Exception("暂时不支持该语法");
                }
            }

            return sb_sql.ToString();
        }
        /// <summary>
        /// 生成查询字段清单
        /// </summary>
        /// <param name="dictabinfo"></param>
        /// <param name="queryProvider"></param>
        /// <returns></returns>
        public Tuple<string, string, List<HiColumn>> BuildQueryFieldSql(Dictionary<string, TabInfo> dictabinfo, QueryProvider queryProvider)
        {
            int _idx = 0;
            int _idx2 = 0;
            StringBuilder sb_field_result = new StringBuilder();
            StringBuilder sb_field = new StringBuilder();

            List<HiColumn> lstcol = new List<HiColumn>();

            foreach (FieldDefinition fieldDefinition in queryProvider.Fields)
            {
                if (!fieldDefinition.IsFun && !fieldDefinition.IsCaseField)
                {
                    if (!queryProvider.IsMultiSubQuery)
                    {
                        #region 非子查询

                        HiColumn hiColumn = CheckField(queryProvider.TableList, dictabinfo, queryProvider.Fields, fieldDefinition, true);
                        if (fieldDefinition.FieldName == fieldDefinition.AsFieldName)
                        {

                            if (fieldDefinition.FieldName == "*")
                            {
                                TableDefinition table = queryProvider.TableList.Where(t => t.AsTabName == fieldDefinition.AsTabName).FirstOrDefault();
                                if (table != null)
                                {
                                    _idx2 = 0;
                                    foreach (HiColumn col in dictabinfo[table.TabName].GetColumns)
                                    {
                                        lstcol.Add(col.CloneProperoty<HiColumn>());
                                        sb_field_result.Append($"{dbConfig.Field_Pre}{col.FieldName}{dbConfig.Field_After}");
                                        sb_field.Append($"{dbConfig.Table_Pre}{Tool.GetDbTabName(col, fieldDefinition)}{dbConfig.Table_After}.{dbConfig.Field_Pre}{col.FieldName}{dbConfig.Field_After}");
                                        if (_idx2 < dictabinfo[table.TabName].Columns.Count - 1)
                                        {
                                            sb_field_result.Append(",");
                                            sb_field.Append(",");
                                        }
                                        _idx2++;
                                    }
                                }
                            }
                            else
                            {
                                if (hiColumn != null)
                                {
                                    var _col = hiColumn.CloneProperoty<HiColumn>();
                                    _col.FieldName = Tool.GetDbFieldName(hiColumn, fieldDefinition);
                                    lstcol.Add(_col);
                                }
                                sb_field.Append($"{dbConfig.Table_Pre}{Tool.GetDbTabName(hiColumn, fieldDefinition)}{dbConfig.Table_After}.{dbConfig.Field_Pre}{Tool.GetDbFieldName(hiColumn, fieldDefinition)}{dbConfig.Field_After}");
                                sb_field_result.Append($"{dbConfig.Field_Pre}{Tool.GetDbFieldName(hiColumn, fieldDefinition)}{dbConfig.Field_After}");
                            }

                        }
                        else
                        {
                            if (hiColumn != null)
                            {
                                var _col = hiColumn.CloneProperoty<HiColumn>();
                                _col.FieldName = fieldDefinition.AsFieldName;
                                lstcol.Add(_col);
                            }
                            sb_field.Append($"{dbConfig.Table_Pre}{Tool.GetDbTabName(hiColumn, fieldDefinition)}{dbConfig.Table_After}.{dbConfig.Field_Pre}{hiColumn.FieldName}{dbConfig.Field_After} as {dbConfig.Field_Pre}{fieldDefinition.AsFieldName}{dbConfig.Field_After}");
                            sb_field_result.Append($"{dbConfig.Field_Pre}{Tool.GetDbFieldName(hiColumn, fieldDefinition)}{dbConfig.Field_After}");
                        }
                        #endregion
                    }
                    else
                    {
                        //当是有子查询时这些字段都是从子查询的结果的
                        sb_field.Append($"{dbConfig.Field_Pre}{fieldDefinition.FieldName}{dbConfig.Field_After}");
                        sb_field_result.Append($"{dbConfig.Field_Pre}{fieldDefinition.FieldName}{dbConfig.Field_After}");
                        bool _isfind = false;
                        foreach (QueryProvider q in queryProvider.SubQuery)
                        {
                            foreach (HiColumn hiColumn in q.ResultColumn)
                            {
                                if (hiColumn.FieldName.ToUpper() == fieldDefinition.FieldName.ToUpper())
                                {
                                    lstcol.Add(hiColumn.CloneProperoty<HiColumn>());
                                    _isfind = true;
                                    break;
                                }
                            }
                            if (_isfind)
                                break;
                        }
                        if (!_isfind)
                            throw new Exception($"字段[{fieldDefinition.FieldName}]在子查询中不存在");

                    }
                }
                else if (fieldDefinition.IsFun && !fieldDefinition.IsCaseField)
                {
                    if (!queryProvider.IsMultiSubQuery)
                    {
                        #region 非子查询

                        HiColumn hiColumn = CheckField(queryProvider.TableList, dictabinfo, queryProvider.Fields, fieldDefinition, true);
                        if (hiColumn != null)
                        {
                            switch (fieldDefinition.DbFun)
                            {
                                case DbFunction.AVG:
                                    if (hiColumn.FieldType.IsIn<HiType>(HiType.BIGINT, HiType.DECIMAL, HiType.INT, HiType.SMALLINT))
                                    {
                                        sb_field.Append($"avg({dbConfig.Table_Pre}{Tool.GetDbTabName(hiColumn, fieldDefinition)}{dbConfig.Table_After}.{dbConfig.Field_Pre}{hiColumn.FieldName}{dbConfig.Field_After}) as {fieldDefinition.AsFieldName}");
                                        sb_field_result.Append($"{dbConfig.Field_Pre}{fieldDefinition.AsFieldName}{dbConfig.Field_After}");
                                        lstcol.Add(new HiColumn { FieldName = fieldDefinition.AsFieldName, DBDefault = HiTypeDBDefault.EMPTY, FieldType = hiColumn.FieldType, FieldDec = hiColumn.FieldDec, FieldLen = hiColumn.FieldLen, FieldDesc = $"avg_{fieldDefinition.AsFieldName}" });
                                    }
                                    else
                                        throw new Exception($"表[{fieldDefinition.AsTabName}]字段[{hiColumn.FieldName}]不属于数值型无法使用AVG函数");
                                    break;
                                case DbFunction.COUNT:
                                    sb_field.Append($"count(*) as {fieldDefinition.AsFieldName}");
                                    sb_field_result.Append($"{dbConfig.Field_Pre}{fieldDefinition.AsFieldName}{dbConfig.Field_After}");
                                    lstcol.Add(new HiColumn { FieldName = fieldDefinition.AsFieldName, DBDefault = HiTypeDBDefault.EMPTY, FieldType = HiType.INT, FieldDesc = $"count_{fieldDefinition.AsFieldName}" });
                                    break;
                                case DbFunction.SUM:
                                    if (hiColumn.FieldType.IsIn<HiType>(HiType.BIGINT, HiType.DECIMAL, HiType.INT, HiType.SMALLINT))
                                    {
                                        sb_field.Append($"sum({dbConfig.Table_Pre}{Tool.GetDbTabName(hiColumn, fieldDefinition)}{dbConfig.Table_After}.{dbConfig.Field_Pre}{hiColumn.FieldName}{dbConfig.Field_After}) as {fieldDefinition.AsFieldName}");
                                        sb_field_result.Append($"{dbConfig.Field_Pre}{fieldDefinition.AsFieldName}{dbConfig.Field_After}");
                                        lstcol.Add(new HiColumn { FieldName = fieldDefinition.AsFieldName, DBDefault = HiTypeDBDefault.EMPTY, FieldType = hiColumn.FieldType, FieldDec = hiColumn.FieldDec, FieldLen = hiColumn.FieldLen, FieldDesc = $"sum_{fieldDefinition.AsFieldName}" });
                                    }
                                    else
                                        throw new Exception($"表[{fieldDefinition.AsTabName}]字段[{hiColumn.FieldName}]不属于数值型无法使用SUM函数");
                                    break;
                                case DbFunction.MAX:
                                    if (hiColumn.FieldType.IsIn<HiType>(HiType.BIGINT, HiType.DECIMAL, HiType.INT, HiType.SMALLINT, HiType.NVARCHAR, HiType.VARCHAR, HiType.NCHAR, HiType.CHAR, HiType.DATE,
                                        HiType.DATETIME))
                                    {
                                        sb_field.Append($"max({dbConfig.Table_Pre}{Tool.GetDbTabName(hiColumn, fieldDefinition)}{dbConfig.Table_After}.{dbConfig.Field_Pre}{hiColumn.FieldName}{dbConfig.Field_After}) as {fieldDefinition.AsFieldName}");
                                        sb_field_result.Append($"{dbConfig.Field_Pre}{fieldDefinition.AsFieldName}{dbConfig.Field_After}");
                                        lstcol.Add(new HiColumn { FieldName = fieldDefinition.AsFieldName, DBDefault = HiTypeDBDefault.EMPTY, FieldType = hiColumn.FieldType, FieldDec = hiColumn.FieldDec, FieldLen = hiColumn.FieldLen, FieldDesc = $"max_{fieldDefinition.AsFieldName}" });
                                    }
                                    else
                                        throw new Exception($"表[{fieldDefinition.AsTabName}]字段[{hiColumn.FieldName}]不属于数值型无法使用MAX函数");
                                    break;
                                case DbFunction.MIN:
                                    if (hiColumn.FieldType.IsIn<HiType>(HiType.BIGINT, HiType.DECIMAL, HiType.INT, HiType.SMALLINT, HiType.NVARCHAR, HiType.VARCHAR, HiType.NCHAR, HiType.CHAR, HiType.DATE,
                                        HiType.DATETIME))
                                    {
                                        sb_field.Append($"min({dbConfig.Table_Pre}{Tool.GetDbTabName(hiColumn, fieldDefinition)}{dbConfig.Table_After}.{dbConfig.Field_Pre}{hiColumn.FieldName}{dbConfig.Field_After}) as {fieldDefinition.AsFieldName}");
                                        sb_field_result.Append($"{dbConfig.Field_Pre}{fieldDefinition.AsFieldName}{dbConfig.Field_After}");
                                        lstcol.Add(new HiColumn { FieldName = fieldDefinition.AsFieldName, DBDefault = HiTypeDBDefault.EMPTY, FieldType = hiColumn.FieldType, FieldDec = hiColumn.FieldDec, FieldLen = hiColumn.FieldLen, FieldDesc = $"min_{fieldDefinition.AsFieldName}" });
                                    }
                                    else
                                        throw new Exception($"表[{fieldDefinition.AsTabName}]字段[{hiColumn.FieldName}]不属于数值型无法使用MIN函数");
                                    break;
                            }
                        }
                        else
                        {
                            if (fieldDefinition.DbFun == DbFunction.COUNT)
                            {
                                sb_field.Append($"count(*) as {fieldDefinition.AsFieldName}");
                                sb_field_result.Append($"{dbConfig.Field_Pre}{fieldDefinition.AsFieldName}{dbConfig.Field_After}");
                                lstcol.Add(new HiColumn { FieldName = fieldDefinition.AsFieldName, DBDefault = HiTypeDBDefault.EMPTY, FieldType = HiType.INT, FieldDesc = $"count_{fieldDefinition.AsFieldName}" });
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        bool _isfind = false;
                        switch (fieldDefinition.DbFun)
                        {
                            case DbFunction.AVG:
                                sb_field.Append($"avg({dbConfig.Field_Pre}{fieldDefinition.FieldName}{dbConfig.Field_After}) as {dbConfig.Field_Pre}{fieldDefinition.AsFieldName}{dbConfig.Field_After}");
                                sb_field_result.Append($"avg({dbConfig.Field_Pre}{fieldDefinition.FieldName}{dbConfig.Field_After}) as {dbConfig.Field_Pre}{fieldDefinition.AsFieldName}{dbConfig.Field_After}");
                                lstcol.Add(new HiColumn { FieldName = fieldDefinition.AsFieldName, DBDefault = HiTypeDBDefault.EMPTY, FieldType = HiType.DECIMAL, FieldDec = 3, FieldDesc = $"avg_{fieldDefinition.AsFieldName}" });

                                break;
                            case DbFunction.COUNT:
                                sb_field.Append($"count(*) as  {dbConfig.Field_Pre}{fieldDefinition.AsFieldName}{dbConfig.Field_After}");
                                sb_field_result.Append($"count(*) as  {dbConfig.Field_Pre}{fieldDefinition.AsFieldName}{dbConfig.Field_After}");

                                lstcol.Add(new HiColumn { FieldName = fieldDefinition.AsFieldName, DBDefault = HiTypeDBDefault.EMPTY, FieldType = HiType.INT, FieldDesc = $"count_{fieldDefinition.AsFieldName}" });
                                break;
                            case DbFunction.MAX:
                                sb_field.Append($"max({dbConfig.Field_Pre}{fieldDefinition.FieldName}{dbConfig.Field_After}) as  {dbConfig.Field_Pre}{fieldDefinition.AsFieldName}{dbConfig.Field_After}");
                                sb_field_result.Append($"max({dbConfig.Field_Pre}{fieldDefinition.FieldName}{dbConfig.Field_After}) as  {dbConfig.Field_Pre}{fieldDefinition.AsFieldName}{dbConfig.Field_After}");

                                lstcol.Add(new HiColumn { FieldName = fieldDefinition.AsFieldName, DBDefault = HiTypeDBDefault.EMPTY, FieldType = HiType.DECIMAL, FieldDec = 3, FieldDesc = $"max_{fieldDefinition.AsFieldName}" });

                                break;
                            case DbFunction.MIN:
                                sb_field.Append($"min({dbConfig.Field_Pre}{fieldDefinition.FieldName}{dbConfig.Field_After}) as  {dbConfig.Field_Pre}{fieldDefinition.AsFieldName}{dbConfig.Field_After}");
                                sb_field_result.Append($"min({dbConfig.Field_Pre}{fieldDefinition.FieldName}{dbConfig.Field_After}) as  {dbConfig.Field_Pre}{fieldDefinition.AsFieldName}{dbConfig.Field_After}");
                                lstcol.Add(new HiColumn { FieldName = fieldDefinition.AsFieldName, DBDefault = HiTypeDBDefault.EMPTY, FieldType = HiType.DECIMAL, FieldDec = 3, FieldDesc = $"min_{fieldDefinition.AsFieldName}" });
                                break;
                            case DbFunction.SUM:
                                sb_field.Append($"sum({dbConfig.Field_Pre}{fieldDefinition.FieldName}{dbConfig.Field_After}) as  {dbConfig.Field_Pre}{fieldDefinition.AsFieldName}{dbConfig.Field_After}");
                                sb_field_result.Append($"sum({dbConfig.Field_Pre}{fieldDefinition.FieldName}{dbConfig.Field_After}) as  {dbConfig.Field_Pre}{fieldDefinition.AsFieldName}{dbConfig.Field_After}");
                                lstcol.Add(new HiColumn { FieldName = fieldDefinition.AsFieldName, DBDefault = HiTypeDBDefault.EMPTY, FieldType = HiType.DECIMAL, FieldDec = 3, FieldDesc = $"sum_{fieldDefinition.AsFieldName}" });
                                break;
                            default:
                                break;
                        }
                        if (fieldDefinition.DbFun.IsIn<DbFunction>(DbFunction.MAX, DbFunction.MIN, DbFunction.SUM))
                        {
                            _isfind = false;
                            foreach (QueryProvider q in queryProvider.SubQuery)
                            {
                                foreach (HiColumn hiColumn in q.ResultColumn)
                                {
                                    if (hiColumn.FieldName == fieldDefinition.FieldName)
                                    {
                                        lstcol.Add(hiColumn.CloneProperoty<HiColumn>());
                                        _isfind = true;
                                        break;
                                    }
                                }
                                if (_isfind)
                                    break;
                            }
                            if (!_isfind)
                                throw new Exception($"字段[{fieldDefinition.FieldName}]在子查询中不存在");
                        }
                    }
                }
                else
                {
                    // 表示是case字段
                    //fieldDefinition.IsCaseField==true
                    //需要解析case语法
                    if (fieldDefinition.Case != null)
                    {
                        string _case_str = buildCaseSql(dictabinfo, queryProvider, fieldDefinition);
                        sb_field.Append(_case_str);
                        sb_field_result.Append(_case_str);
                        lstcol.Add(new HiColumn { FieldName = fieldDefinition.Case.EndAs.AsFieldName, DBDefault = HiTypeDBDefault.EMPTY, FieldType = HiType.NVARCHAR, FieldLen = 50, FieldDec = 0, FieldDesc = $"case_{fieldDefinition.Case.EndAs.AsFieldName}" });
                    }
                    else
                    {
                        throw new Exception($"字段[{fieldDefinition.AsFieldName}]标识了是Case条件字段但未找到值");
                    }
                }

                if (_idx < queryProvider.Fields.Count - 1)
                {
                    sb_field.Append(",");
                    sb_field_result.Append(",");
                }
                _idx++;
            }


            return new Tuple<string, string, List<HiColumn>>(sb_field.ToString(), sb_field_result.ToString(), lstcol);
        }

        /// <summary>
        /// 字段检测合法性
        /// </summary>
        /// <param name="TableList"></param>
        /// <param name="dictabinfo"></param>
        /// <param name="Fields"></param>
        /// <param name="fieldDefinition"></param>
        /// <param name="allowstart"></param>
        /// <returns></returns>
        public HiColumn CheckField(List<TableDefinition> TableList, Dictionary<string, TabInfo> dictabinfo, List<FieldDefinition> Fields, FieldDefinition fieldDefinition, bool allowstart = false)
        {
            HiColumn hiColumn = null;
            //2021.12.8 add by tgm
            if (string.IsNullOrEmpty(fieldDefinition.AsTabName))
            {
                if (TableList.Count == 1)
                {
                    fieldDefinition.AsTabName = TableList[0].AsTabName;
                }
                else
                {
                    throw new Exception($"查询多张表时 字段[{fieldDefinition.FieldName}]需要指定表");
                }
            }
            TableDefinition tabinfo = TableList.Where(t => t.AsTabName.ToLower() == fieldDefinition.AsTabName.ToLower()).FirstOrDefault();//&& t.Columns.Any(c=>c.FieldName==fieldDefinition.FieldName)
            if (tabinfo != null)
            {
                if (dictabinfo.ContainsKey(tabinfo.TabName))
                {
                    hiColumn = dictabinfo[tabinfo.TabName].Columns.Where(f => f.FieldName.ToLower() == fieldDefinition.FieldName.ToLower()).FirstOrDefault();
                    if (hiColumn == null && Fields != null && Fields.Count > 0)
                    {
                        FieldDefinition fieldDefinition1 = Fields.Where(f => f.AsFieldName.ToLower() == fieldDefinition.FieldName.ToLower()).FirstOrDefault();
                        if (fieldDefinition1 != null)
                        {
                            hiColumn = dictabinfo[tabinfo.TabName].Columns.Where(f => f.FieldName.ToLower() == fieldDefinition1.FieldName.ToLower()).FirstOrDefault();
                            if (hiColumn == null)
                            {
                                if (fieldDefinition1.FieldName.Trim() != "*" && allowstart == true)
                                    throw new Exception($"字段[{fieldDefinition1.FieldName}]在表[{fieldDefinition1.AsTabName}]中不存在");
                            }
                        }
                        else
                        {
                            if (fieldDefinition.FieldName.Trim() != "*" && allowstart == true)
                                throw new Exception($"字段[{fieldDefinition.FieldName}]在表[{fieldDefinition.AsTabName}]中不存在");
                        }
                    }


                }
            }
            return hiColumn;
        }
        #endregion



        #region 私有方法集
        string buildCaseSql(Dictionary<string, TabInfo> dictabinfo, QueryProvider queryProvider, FieldDefinition fieldDefinition)
        {
            //if (fieldDefinition.Case != null)
            //{
            StringBuilder sb_case = new StringBuilder();

            sb_case.AppendLine("case");
            foreach (WhenDefinition whenDefinition in fieldDefinition.Case.WhenList)
            {

                HiColumn hiColumn = CheckField(queryProvider.TableList, dictabinfo, queryProvider.Fields, whenDefinition.Field, true);
                switch (whenDefinition.OperSymbol)
                {
                    case OperType.EQ:
                        sb_case.AppendLine($"   when {dbConfig.Table_Pre}{Tool.GetDbTabName(hiColumn, whenDefinition.Field)}{dbConfig.Table_After}{dbConfig.Field_Pre}{Tool.GetDbFieldName(hiColumn, whenDefinition.Field)}{dbConfig.Field_After} = {whenDefinition.Value} then {whenDefinition.Then.ThenValue}");
                        break;
                    case OperType.GT:
                        sb_case.AppendLine($"   when {dbConfig.Table_Pre}{Tool.GetDbTabName(hiColumn, whenDefinition.Field)}{dbConfig.Table_After}{dbConfig.Field_Pre}{Tool.GetDbFieldName(hiColumn, whenDefinition.Field)}{dbConfig.Field_After} > {whenDefinition.Value} then {whenDefinition.Then.ThenValue}");
                        break;
                    case OperType.LT:
                        sb_case.AppendLine($"   when {dbConfig.Table_Pre}{Tool.GetDbTabName(hiColumn, whenDefinition.Field)}{dbConfig.Table_After}{dbConfig.Field_Pre}{Tool.GetDbFieldName(hiColumn, whenDefinition.Field)}{dbConfig.Field_After} < {whenDefinition.Value} then {whenDefinition.Then.ThenValue}");
                        break;
                    case OperType.GE:
                        sb_case.AppendLine($"   when {dbConfig.Table_Pre}{Tool.GetDbTabName(hiColumn, whenDefinition.Field)}{dbConfig.Table_After}{dbConfig.Field_Pre}{Tool.GetDbFieldName(hiColumn, whenDefinition.Field)}{dbConfig.Field_After} >= {whenDefinition.Value} then {whenDefinition.Then.ThenValue}");
                        break;
                    case OperType.LE:
                        sb_case.AppendLine($"   when {dbConfig.Table_Pre}{Tool.GetDbTabName(hiColumn, whenDefinition.Field)}{dbConfig.Table_After}{dbConfig.Field_Pre}{Tool.GetDbFieldName(hiColumn, whenDefinition.Field)}{dbConfig.Field_After} <= {whenDefinition.Value} then {whenDefinition.Then.ThenValue}");
                        break;
                    case OperType.NE:
                        sb_case.AppendLine($"   when {dbConfig.Table_Pre}{Tool.GetDbTabName(hiColumn, whenDefinition.Field)}{dbConfig.Table_After}{dbConfig.Field_Pre}{Tool.GetDbFieldName(hiColumn, whenDefinition.Field)}{dbConfig.Field_After} <> {whenDefinition.Value} then {whenDefinition.Then.ThenValue}");
                        break;

                    default:
                        sb_case.AppendLine($"   when {dbConfig.Table_Pre}{Tool.GetDbTabName(hiColumn, whenDefinition.Field)}{dbConfig.Table_After}{dbConfig.Field_Pre}{Tool.GetDbFieldName(hiColumn, whenDefinition.Field)}{dbConfig.Field_After} = {whenDefinition.Value} then {whenDefinition.Then.ThenValue}");
                        break;
                }

            }
            if (fieldDefinition.Case.Else != null)
            {
                sb_case.AppendLine($"   else {fieldDefinition.Case.Else.ElseValue}");
            }
            if (fieldDefinition.Case.EndAs != null)
            {
                sb_case.AppendLine($"end as {dbConfig.Field_Pre}{fieldDefinition.Case.EndAs.AsFieldName}{dbConfig.Field_After}");
            }
            return sb_case.ToString();
            //}
            //else
            //{
            //    throw new Exception($"字段[{fieldDefinition.AsFieldName}]标识了是Case条件字段但未找到值");
            //}
        }
        string getBetweenValue(HiColumn hiColumn, FilterDefinition filterDefinition, object value)
        {

            if (value.GetType().FullName.IndexOf("HiSql.RangDefinition") >= 0)
            {
                RangDefinition rangDefinition = (RangDefinition)value;
                return $"'{rangDefinition.Low.ToString().ToSqlInject()}' and '{rangDefinition.High.ToString().ToSqlInject()}'";
            }
            else
            {
                throw new Exception($"过滤条件字段[{filterDefinition.Field.AsFieldName}] 指定是[Between] 必须指定[RangDefinition]");
            }

        }

        string getInValue(bool issubquery, HiColumn hiColumn, FilterDefinition filterDefinition, object value)
        {
            string _value = string.Empty;
            StringBuilder _sb_value = new StringBuilder();
            int _idx = 0;
            Type _type = value.GetType();
            bool _islist = _type.FullName.IsList();
            bool _isquery = _type.FullName.IndexOf("HiSql.") >= 0 && _type.FullName.IndexOf("Query") > 0;
            if (!issubquery)
            {
                #region 非子查询
                if (_islist && hiColumn.FieldType.IsIn<HiType>(HiType.NCHAR, HiType.NVARCHAR, HiType.GUID, HiType.VARCHAR, HiType.CHAR))
                {
                    List<string> lstobj = (List<string>)value;
                    bool _isen = hiColumn.FieldType.IsIn<HiType>(HiType.NCHAR, HiType.NVARCHAR, HiType.GUID);
                    foreach (string str in lstobj)
                    {
                        if (_isen)
                        {
                            if (str.Length <= hiColumn.FieldLen)
                            {
                                if (_idx < lstobj.Count - 1)
                                    _sb_value.Append($"'{str.ToSqlInject()}',");
                                else
                                    _sb_value.Append($"'{str.ToSqlInject()}'");
                            }
                            else
                                throw new Exception($"过滤条件字段[{filterDefinition.Field.AsFieldName}]指定的值超过了限定长度[{hiColumn.FieldLen}]");
                        }
                        else
                        {
                            if (str.LengthZH() <= hiColumn.FieldLen)
                            {
                                if (_idx < lstobj.Count - 1)
                                    _sb_value.Append($"'{str.ToSqlInject()}',");
                                else
                                    _sb_value.Append($"'{str.ToSqlInject()}'");
                            }
                            else
                                throw new Exception($"过滤条件字段[{filterDefinition.Field.AsFieldName}]指定的值超过了限定长度[{hiColumn.FieldLen}]");
                        }
                        _idx++;
                    }
                }
                else if (_islist && hiColumn.FieldType.IsIn<HiType>(HiType.BIGINT))
                {
                    List<Int64> lstobj = (List<Int64>)value;
                    foreach (Int64 di in lstobj)
                    {
                        if (_idx < lstobj.Count - 1)
                            _sb_value.Append($"{di},");
                        else
                            _sb_value.Append($"{di}");

                        _idx++;
                    }

                }
                else if (_islist && hiColumn.FieldType.IsIn<HiType>(HiType.INT))
                {
                    List<int> lstobj = (List<int>)value;
                    foreach (int di in lstobj)
                    {
                        if (_idx < lstobj.Count - 1)
                            _sb_value.Append($"{di},");
                        else
                            _sb_value.Append($"{di}");

                        _idx++;
                    }

                }
                else if (_islist && hiColumn.FieldType.IsIn<HiType>(HiType.SMALLINT))
                {
                    List<Int16> lstobj = (List<Int16>)value;
                    foreach (Int16 di in lstobj)
                    {
                        if (_idx < lstobj.Count - 1)
                            _sb_value.Append($"{di},");
                        else
                            _sb_value.Append($"{di}");

                        _idx++;
                    }

                }
                else if (_islist && hiColumn.FieldType.IsIn<HiType>(HiType.DECIMAL))
                {
                    List<decimal> lstobj = (List<decimal>)value;
                    foreach (decimal di in lstobj)
                    {
                        if (_idx < lstobj.Count - 1)
                            _sb_value.Append($"{di},");
                        else
                            _sb_value.Append($"{di}");

                        _idx++;
                    }
                }
                else if (_islist && hiColumn.FieldType.IsIn<HiType>(HiType.DATETIME, HiType.DATE))
                {
                    List<string> lstobj = (List<string>)value;
                    foreach (string str in lstobj)
                    {
                        if (_idx < lstobj.Count - 1)
                            _sb_value.Append($"'{str.ToSqlInject()}',");
                        else
                            _sb_value.Append($"'{str.ToSqlInject()}'");
                        _idx++;
                    }
                }
                else if (_islist)
                {
                    List<string> lstobj = (List<string>)value;
                    foreach (string str in lstobj)
                    {
                        if (_idx < lstobj.Count - 1)
                            _sb_value.Append($"'{str.ToSqlInject()}',");
                        else
                            _sb_value.Append($"'{str.ToSqlInject()}'");
                        _idx++;
                    }
                }

                else
                {
                    if (_isquery)
                    {
                        IQuery query = (IQuery)value;
                        _sb_value.Append(query.ToSql());
                    }
                    else
                        throw new Exception($"过滤条件字段[{filterDefinition.Field.AsFieldName}]使用了操作符In 那么值必须为List数据集合");
                }

                #endregion
            }
            else
            {
                if (_islist)
                {
                    List<string> lstobj = (List<string>)value;
                    foreach (string str in lstobj)
                    {
                        if (_idx < lstobj.Count - 1)
                            _sb_value.Append($"'{str.ToSqlInject()}',");
                        else
                            _sb_value.Append($"'{str.ToSqlInject()}'");
                        _idx++;
                    }
                }
                else
                {
                    if (_isquery)
                    {
                        IQuery query = (IQuery)value;
                        _sb_value.Append(query.ToSql());
                    }
                    else
                        throw new Exception($"过滤条件字段[{filterDefinition.Field.AsFieldName}]使用了操作符In 那么值必须为List数据集合");
                }
            }

            return _sb_value.ToString();
        }
        string getSingleValue(bool issubquery, HiColumn hiColumn, object value)
        {
            string _value = string.Empty;
            if (!issubquery)
            {
                if (hiColumn.FieldType.IsIn<HiType>(HiType.NCHAR, HiType.NVARCHAR, HiType.GUID))
                {
                    _value = value.ToString();
                    if (_value.Length <= hiColumn.FieldLen || hiColumn.FieldLen < 0)
                        _value = $"'{_value.ToSqlInject()}'";
                    else
                        throw new Exception($"过滤条件字段[{hiColumn.FieldName}]指定的值超过了限定长度[{hiColumn.FieldLen}]");
                }
                else if (hiColumn.FieldType.IsIn<HiType>(HiType.VARCHAR, HiType.CHAR))
                {
                    _value = value.ToString();
                    if (_value.LengthZH() <= hiColumn.FieldLen || hiColumn.FieldLen < 0)
                        _value = $"'{_value.ToSqlInject()}'";
                    else
                        throw new Exception($"过滤条件字段[{hiColumn.FieldName}]指定的值超过了限定长度[{hiColumn.FieldLen}]");
                }
                else if (hiColumn.FieldType.IsIn<HiType>(HiType.INT, HiType.DECIMAL, HiType.SMALLINT, HiType.BIGINT))
                {
                    _value = value.ToString();
                }
                else if (hiColumn.FieldType.IsIn<HiType>(HiType.BOOL))
                {
                    _value = value.ToString();
                }
                else if (hiColumn.FieldType.IsIn<HiType>(HiType.DATETIME))
                {
                    DateTime _time = Convert.ToDateTime(value.ToString());
                    _value = $"'{_time.ToString("yyyy-MM-dd HH:mm:ss.fff")}'";
                }
                else if (hiColumn.FieldType.IsIn<HiType>(HiType.DATE))
                {
                    DateTime _time = Convert.ToDateTime(value.ToString());
                    _value = $"'{_time.ToString("yyyy-MM-dd")}'";
                }
                else
                {
                    _value = $"'{value.ToString().ToSqlInject()}'";
                }
            }
            else
            {
                _value = $"'{value.ToString().ToSqlInject()}'";
            }
            return _value;
        }
        /// <summary>
        /// 检测模板字段
        /// </summary>
        /// <param name="lsttemp"></param>
        /// <param name="hiColumn"></param>
        /// <param name="filterDefinition"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        string checkTempValue(List<Dictionary<string, string>> lsttemp, HiColumn hiColumn, FilterDefinition filterDefinition, string value, bool ischar = true, List<TableDefinition> TableList = null, Dictionary<string, TabInfo> dictabinfo = null)
        {
            //字符串字段只允许 一个模版字段 如 a.username=`b.user` 而不允许 a.username=`b.user`+1
            if (lsttemp.Count > 1 && ischar)
            {
                throw new Exception($"{HiSql.Constants.HiSqlSyntaxError} [{value}] 字段[{hiColumn.FieldName}]为[{hiColumn.FieldType.ToString()}] 条件不允许多个模板!");
            }
            else
            {
                if (Tool.RegexMatch(Constants.REG_TEMPLATE_FIELD, value))
                {
                    string nvalue = "";
                    var dic = Tool.RegexGrp(Constants.REG_TEMPLATE_FIELD, value);

                    FieldDefinition _field = new FieldDefinition(dic["0"].Replace("`", ""));

                    HiColumn _hiColumn = CheckField(TableList, dictabinfo, null, _field, false);

                    if (!string.IsNullOrEmpty(dic["flag"].ToString()))
                        nvalue = dic["flag"].ToString();
                    if (!string.IsNullOrEmpty(dic["tab"].ToString()))
                        nvalue = nvalue + $"{dbConfig.Table_Pre}{Tool.GetDbTabName(_hiColumn, _field)}{dbConfig.Table_After}.{dbConfig.Field_Pre}{Tool.GetDbFieldName(_hiColumn, _field)}{dbConfig.Field_After}";
                    else
                        nvalue = nvalue + $"{dbConfig.Field_Pre}{Tool.GetDbFieldName(_hiColumn, _field)}{dbConfig.Field_After}";


                    value = nvalue;
                }
                else
                {
                    if (ischar)
                        throw new Exception($"{HiSql.Constants.HiSqlSyntaxError} [{value}] 字段[{hiColumn.FieldName}]为[{hiColumn.FieldType.ToString()}] 不允许表达式");
                    else
                    {
                        //非字符串的允许使用表达式 
                        Regex regex = new Regex(Constants.REG_TEMPLATE_FIELDS);
                        string nvalue = value;
                        foreach (Dictionary<string, string> dic in lsttemp)
                        {
                            string _fieldv = "";

                            FieldDefinition _field = new FieldDefinition(dic["0"].Replace("`", ""));
                            HiColumn _hiColumn = CheckField(TableList, dictabinfo, null, _field, false);

                            if (!string.IsNullOrEmpty(dic["flag"].ToString()))
                                _fieldv = dic["flag"].ToString();
                            if (!string.IsNullOrEmpty(dic["tab"].ToString()))
                                _fieldv = _fieldv + $"{dbConfig.Table_Pre}{Tool.GetDbTabName(_hiColumn, _field)}{dbConfig.Table_After}.{dbConfig.Field_Pre}{Tool.GetDbFieldName(_hiColumn, _field)}{dbConfig.Field_After}";
                            else
                                _fieldv = _fieldv + $"{dbConfig.Field_Pre}{Tool.GetDbFieldName(_hiColumn, _field)}{dbConfig.Field_After}";

                            nvalue = regex.Replace(nvalue, _fieldv, 1);
                        }
                        value = nvalue;
                    }
                }
            }
            return value;
        }
        string getSingleValue(bool issubquery, HiColumn hiColumn, FilterDefinition filterDefinition, object value, List<TableDefinition> TableList = null, Dictionary<string, TabInfo> dictabinfo = null)
        {
            string _value = string.Empty;
            //是否是模板字段
            bool _istempfield = false;
            if (!issubquery)
            {
                _value = value.ToString();
                List<Dictionary<string, string>> _lstdic = Tool.RegexGrps(Constants.REG_TEMPLATE_FIELDS, _value);
                if (_lstdic.Count > 0) _istempfield = true;

                if (hiColumn.FieldType.IsIn<HiType>(HiType.NCHAR, HiType.NVARCHAR, HiType.GUID))
                {
                    _value = value.ToString();
                    if (!_istempfield)
                    {
                        if (_value.Length <= hiColumn.FieldLen || hiColumn.FieldLen < 0)
                            _value = $"'{_value.ToSqlInject()}'";
                        else
                            throw new Exception($"过滤条件字段[{filterDefinition.Field.AsFieldName}]指定的值超过了限定长度[{hiColumn.FieldLen}]");
                    }
                    else
                    {
                        _value = checkTempValue(_lstdic, hiColumn, filterDefinition, _value, true, TableList, dictabinfo);
                    }
                }
                else if (hiColumn.FieldType.IsIn<HiType>(HiType.VARCHAR, HiType.CHAR))
                {
                    _value = value.ToString();
                    if (!_istempfield)
                    {
                        if (_value.LengthZH() <= hiColumn.FieldLen || hiColumn.FieldLen < 0)
                            _value = $"'{_value.ToSqlInject()}'";
                        else
                            throw new Exception($"过滤条件字段[{filterDefinition.Field.AsFieldName}]指定的值超过了限定长度[{hiColumn.FieldLen}]");
                    }
                    else {
                        _value = checkTempValue(_lstdic, hiColumn, filterDefinition, _value, true, TableList, dictabinfo);
                    }
                }
                else if (hiColumn.FieldType.IsIn<HiType>(HiType.INT, HiType.DECIMAL, HiType.SMALLINT, HiType.BIGINT))
                {
                    if (!_istempfield)
                    {
                        _value = value.ToString();
                        if (!Tool.IsDecimal(_value))
                            throw new Exception($"字段[{hiColumn.FieldName}]是[{hiColumn.FieldType}]值为[{_value}]类型不匹配有SQL注入风险");
                    }
                    else
                    {
                        _value = checkTempValue(_lstdic, hiColumn, filterDefinition, _value, true, TableList, dictabinfo);
                    }
                }
                else if (hiColumn.FieldType.IsIn<HiType>(HiType.BOOL))
                {
                    //if ("0".Equals(value.ToString()))
                    //{
                    //    return "false";
                    //}
                    //if ("1".Equals(value.ToString()))
                    //{
                    //    return "true";
                    //}
                    _value = value.ToString();
                }
                else if (hiColumn.FieldType.IsIn<HiType>(HiType.DATETIME))
                {
                    if (!_istempfield)
                    {
                        DateTime _time = Convert.ToDateTime(_value.ToString());
                        _value = $"'{_time.ToString("yyyy-MM-dd HH:mm:ss.fff")}'";
                    }
                    else
                    {
                        _value = checkTempValue(_lstdic, hiColumn, filterDefinition, _value, true, TableList, dictabinfo);
                    }
                }
                else if (hiColumn.FieldType.IsIn<HiType>(HiType.DATE))
                {
                    if (!_istempfield)
                    {
                        DateTime _time = Convert.ToDateTime(_value.ToString());
                        _value = $"'{_time.ToString("yyyy-MM-dd")}'";
                    }
                    else
                        _value = checkTempValue(_lstdic, hiColumn, filterDefinition, _value, true, TableList, dictabinfo);
                }
                else
                {
                    if (!_istempfield)
                        _value = $"'{value.ToString().ToSqlInject()}'";
                    else
                        _value = checkTempValue(_lstdic, hiColumn, filterDefinition, _value, true, TableList, dictabinfo);
                }
            }
            else
            {
                _value = $"'{value.ToString().ToSqlInject()}'";
            }
            return _value;
        }

        /// <summary>
        /// 获取模糊查询值
        /// </summary>
        /// <param name="issubquery"></param>
        /// <param name="hiColumn"></param>
        /// <param name="filterDefinition"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        string getLikeValue(bool issubquery, HiColumn hiColumn, FilterDefinition filterDefinition, object value)
        {
            string _value = string.Empty;
            //没有子查询
            if (!issubquery)
            {
                if (hiColumn.FieldType.IsIn<HiType>(HiType.NCHAR, HiType.NVARCHAR))
                {
                    _value = value.ToString();
                    if (!Tool.RegexMatch(Constants.REG_ISLIKEQUERY, _value))
                        throw new Exception($"当前使用了模糊查询但值[{_value}]未指定[%]符号 ");
                    if (_value.Length <= hiColumn.FieldLen)
                        _value = $"'{_value.ToSqlInject()}'";
                    else
                        throw new Exception($"过滤条件字段[{filterDefinition.Field.AsFieldName}]指定的值超过了限定长度[{hiColumn.FieldLen}]");
                }
                else if (hiColumn.FieldType.IsIn<HiType>(HiType.VARCHAR, HiType.CHAR))
                {
                    _value = value.ToString();
                    if (!Tool.RegexMatch(Constants.REG_ISLIKEQUERY, _value))
                        throw new Exception($"当前使用了模糊查询但值[{_value}]未指定[%]符号 ");
                    if (_value.LengthZH() <= hiColumn.FieldLen)
                        _value = $"'{_value.ToSqlInject()}'";
                    else
                        throw new Exception($"过滤条件字段[{filterDefinition.Field.AsFieldName}]指定的值超过了限定长度[{hiColumn.FieldLen}]");
                }
                else if (hiColumn.FieldType.IsIn<HiType>(HiType.TEXT))
                {
                    throw new Exception($"超大文本不支持用模糊查询");
                }
                else
                {
                    throw new Exception($"当前字段类型{hiColumn.FieldType.ToString()} 不支持模糊查询");
                }
            }

            return _value;
        }

        #endregion

        public DataTable GetTableList(string tabname = "")
        {
            string _tempsql = dbConfig.Get_Tables.Replace("[$Schema$]", this.Context.CurrentConnectionConfig.Schema);
            if (string.IsNullOrEmpty(tabname))
                _tempsql = _tempsql.Replace("[$Where$]", "");
            else
                _tempsql = _tempsql.Replace("[$Where$]", $" and table_name='{tabname.ToSqlInject()}'");
            return Context.DBO.GetDataTable(_tempsql);
        }

        public DataTable GetTableList(string tabname, int pageSize, int pageIndex, out int totalCount)
        {
            int SeqBegin = (pageIndex - 1) * pageSize;
            int SeqEnd = (pageIndex) * pageSize;
            var _where = string.Empty;

            string _tempsql = dbConfig.Get_TablesPaging.Replace("[$Schema$]", this.Context.CurrentConnectionConfig.Schema);

            if (!string.IsNullOrEmpty(tabname))
                _where += $" and table_name like '%{tabname.ToSqlInject()}%'";

            _tempsql = _tempsql.Replace("[$SeqBegin$]", SeqBegin.ToString()).Replace("[$SeqEnd$]", SeqEnd.ToString());

            _tempsql = _tempsql.Replace("[$Where$]", _where);

            List<string> sqlList = new List<string>() { _tempsql };
            List<HiParameter[]> parameters = new List<HiParameter[]>() {
                new HiParameter[]{}
            };
            var dataSet = Context.DBO.GetDataSet(sqlList, parameters);
            totalCount = int.Parse(dataSet.Tables[0].Rows[0][0].ToString());
            return dataSet.Tables[1];
        }

        public DataTable GetViewList(string viewname = "")
        {
            string _tempsql = dbConfig.Get_Views.Replace("[$Schema$]", this.Context.CurrentConnectionConfig.Schema);
            if (string.IsNullOrEmpty(viewname))
                _tempsql = _tempsql.Replace("[$Where$]", "");
            else
                _tempsql = _tempsql.Replace("[$Where$]", $" and table_name='{viewname.ToSqlInject()}'");
            return Context.DBO.GetDataTable(_tempsql);

        }
        public DataTable GetViewList(string viewname, int pageSize, int pageIndex, out int totalCount)
        {
            int SeqBegin = (pageIndex - 1) * pageSize;
            int SeqEnd = (pageIndex) * pageSize;
            var _where = string.Empty;

            string _tempsql = dbConfig.Get_ViewsPaging.Replace("[$Schema$]", this.Context.CurrentConnectionConfig.Schema);

            if (!string.IsNullOrEmpty(viewname))
                _where += $" and table_name like '%{viewname.ToSqlInject()}%'";

            _tempsql = _tempsql.Replace("[$SeqBegin$]", SeqBegin.ToString()).Replace("[$SeqEnd$]", SeqEnd.ToString());

            _tempsql = _tempsql.Replace("[$Where$]", _where);

            List<string> sqlList = new List<string>() { _tempsql };
            List<HiParameter[]> parameters = new List<HiParameter[]>() {
                new HiParameter[]{}
            };
            var dataSet = Context.DBO.GetDataSet(sqlList, parameters);
            totalCount = int.Parse(dataSet.Tables[0].Rows[0][0].ToString());
            return dataSet.Tables[1];
        }
        /// <summary>
        /// 检查表或视图是否存在
        /// </summary>
        /// <param name="tabname"></param>
        /// <returns></returns>
        public bool CheckTabExists(string tabname = "")
        {
            DataTable dt = Context.DBO.GetDataTable(dbConfig.Get_CheckTabExists.Replace("[$TabName$]", tabname).Replace("[$Schema$]", this.Context.CurrentConnectionConfig.Schema));
            return dt.Rows.Count > 0;
        }

        public DataTable GetAllTables(string tabname = "")
        {

            string _tempsql = dbConfig.Get_AllTables.Replace("[$Schema$]", this.Context.CurrentConnectionConfig.Schema);
            if (string.IsNullOrEmpty(tabname))
                _tempsql = _tempsql.Replace("[$Where$]", "");
            else
                _tempsql = _tempsql.Replace("[$Where$]", $" and table_name='{tabname.ToSqlInject()}'");
            return Context.DBO.GetDataTable(_tempsql);

        }
        public DataTable GetAllTables(string tabname, int pageSize, int pageIndex, out int totalCount)
        {
            int SeqBegin = (pageIndex - 1) * pageSize;
            int SeqEnd = (pageIndex) * pageSize;
            var _where = string.Empty;

            string _tempsql = dbConfig.GetAllTablesPaging.Replace("[$Schema$]", this.Context.CurrentConnectionConfig.Schema);

            if (!string.IsNullOrEmpty(tabname))
                _where += $" and table_name like '%{tabname.ToSqlInject()}%'";

            _tempsql = _tempsql.Replace("[$SeqBegin$]", SeqBegin.ToString()).Replace("[$SeqEnd$]", SeqEnd.ToString());

            _tempsql = _tempsql.Replace("[$Where$]", _where);

            List<string> sqlList = new List<string>() { _tempsql };
            List<HiParameter[]> parameters = new List<HiParameter[]>() {
                new HiParameter[]{}
            };
            var dataSet = Context.DBO.GetDataSet(sqlList, parameters);
            totalCount = int.Parse(dataSet.Tables[0].Rows[0][0].ToString());
            return dataSet.Tables[1];
        }
        public string CreateView(string viewname, string viewsql)
        {
            DataTable dt = Context.DBO.GetDataTable(dbConfig.Get_CheckTabExists
                .Replace("[$TabName$]", $"{viewname}")
                .Replace("[$Schema$]", $"{this.Context.CurrentConnectionConfig.Schema}")
                );
            if (dt.Rows.Count == 0)
            {
                //视图名称没有被占用

                string _tempsql = dbConfig.Get_CreateView
                    .Replace("[$Schema$]", $"{dbConfig.Schema_Pre}{Context.CurrentConnectionConfig.Schema}{dbConfig.Schema_After}")
                    .Replace("[$TabName$]", $"{dbConfig.Table_Pre}{viewname}{dbConfig.Table_After}")
                    .Replace("[$ViewSql$]", viewsql)
                    ;
                return _tempsql;
            }
            else
            {
                throw new Exception($"视图名称[{viewname}]已经被使用");
            }
        }
        public string ModiView(string viewname, string viewsql)
        {
            DataTable dt = Context.DBO.GetDataTable(dbConfig.Get_CheckTabExists
                .Replace("[$TabName$]", $"{viewname}")
                .Replace("[$Schema$]", $"{this.Context.CurrentConnectionConfig.Schema}")
                );
            if (dt.Rows.Count > 0)
            {
                string _tempsql = dbConfig.Get_ModiView
                    .Replace("[$Schema$]", $"{dbConfig.Schema_Pre}{Context.CurrentConnectionConfig.Schema}{dbConfig.Schema_After}")
                    .Replace("[$TabName$]", $"{dbConfig.Table_Pre}{viewname}{dbConfig.Table_After}")
                    .Replace("[$ViewSql$]", viewsql)
                    ;
                return _tempsql;
            }
            else
                throw new Exception($"视图名称[{viewname}]不存在无法修改");
        }

        public string DropView(string viewname)
        {
            DataTable dt = Context.DBO.GetDataTable(dbConfig.Get_CheckTabExists
                .Replace("[$TabName$]", $"{viewname}")
                .Replace("[$Schema$]", $"{this.Context.CurrentConnectionConfig.Schema}")
                );
            if (dt.Rows.Count > 0)
            {
                string _tempsql = dbConfig.Get_DropView
                    .Replace("[$Schema$]", $"{dbConfig.Schema_Pre}{Context.CurrentConnectionConfig.Schema}{dbConfig.Schema_After}")
                    .Replace("[$TabName$]", $"{dbConfig.Table_Pre}{viewname}{dbConfig.Table_After}");
                return _tempsql;
            }
            else
                throw new Exception($"视图名称[{viewname}]不存在无法删除");
        }

        public DataTable GetGlobalTables()
        {
            throw new NotImplementedException();
        }

        public List<TabIndex> GetIndexs(string tabname)
        {

            string _sql = dbConfig.Get_TabIndexs.Replace("[$TabName$]", tabname);

            List<TabIndex> lstindex = new List<TabIndex>();
            DataTable dt = Context.DBO.GetDataTable(_sql);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    TabIndex tabIndex = new TabIndex();
                    tabIndex.TabName = tabname;
                    tabIndex.IndexName = dr["IndexName"].ToString();
                    tabIndex.IndexType = dr["IndexType"].ToString();
                    lstindex.Add(tabIndex);
                }
                return lstindex;
            }
            else
                return new List<TabIndex>();

            throw new NotImplementedException();
        }

        public List<TabIndexDetail> GetIndexDetails(string tabname, string indexname)
        {
            string _sql = dbConfig.Get_IndexDetail.Replace("[$TabName$]", tabname).Replace("[$IndexName$]", indexname);
            List<TabIndexDetail> lstindex = new List<TabIndexDetail>();
            DataTable dt = Context.DBO.GetDataTable(_sql);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    TabIndexDetail tabIndex = new TabIndexDetail();
                    tabIndex.TabName = tabname;
                    tabIndex.ColumnName = dr["ColumnName"].ToString();
                    tabIndex.IndexName = dr["IndexName"].ToString();
                    tabIndex.IndexType = dr["IndexType"].ToString();
                    tabIndex.ColumnIdx = Convert.ToInt32(dr["ColumnIdx"].ToString());
                    tabIndex.ColumnId = Convert.ToInt32(dr["ColumnID"].ToString());
                    tabIndex.SortType = dr["Sort"].ToString().ToLower() == "asc" ? SortType.ASC : SortType.DESC;
                    tabIndex.IsPrimary = dr["IsPrimary"].ToString() == "Y" ? true : false;
                    tabIndex.IsUnique = dr["IsUnique"].ToString() == "Y" ? true : false;

                    lstindex.Add(tabIndex);
                }
                return lstindex;
            }
            else
                return new List<TabIndexDetail>();
            throw new NotImplementedException();
        }
        /// <summary>
        /// 创建主键
        /// </summary>
        /// <param name="tabname"></param>
        /// <param name="primarykeyname"></param>
        /// <param name="hiColumns"></param>
        /// <returns></returns>
        public string CreatePrimaryKey(string tabname, List<HiColumn> hiColumns)
        {
            //注未判断索引是否重复，由底层库抛出
            string _sql = dbConfig.Table_PrimaryKeyCreate.Replace("[$TabName$]", tabname).Replace("[$Schema$]", Context.CurrentConnectionConfig.Schema);
            DataTable dt = GetTableDefinition(tabname);
            string _fields = string.Empty;

            string keys = BuildKey(hiColumns);
            if (!string.IsNullOrEmpty(keys))
            {
                keys = dbConfig.Table_Key.Replace("[$TabName$]", tabname)
                    .Replace("[$Keys$]", keys).Replace("[$ConnectID$]", this.Context.ConnectedId.Replace("-", "_"));
            }

            _sql = _sql.Replace("[$Primary$]", keys);
            return _sql;
        }

        public string CreateIndex(string tabname, string indexname, List<HiColumn> hiColumns)
        {
            //注未判断索引是否重复，由底层库抛出
            string _sql = dbConfig.Get_CreateIndex.Replace("[$TabName$]", tabname).Replace("[$IndexName$]", indexname).Replace("[$Schema$]", Context.CurrentConnectionConfig.Schema);
            DataTable dt = GetTableDefinition(tabname);
            string _fields = string.Empty;

            if (dt.Rows.Count > 0)
            {
                if (hiColumns.Count > 0)
                {
                    int i = 0;
                    StringBuilder keys = new StringBuilder();
                    foreach (HiColumn hiColumn in hiColumns)
                    {
                        if (!dt.AsEnumerable().Any(t => t.Field<string>("FieldName").ToLower() == hiColumn.FieldName.ToLower()))
                            throw new Exception($"为表[{tabname}]创建的索引指的字段[{hiColumn.FieldName}]不存在于表[{tabname}]中");

                        string _tempkey = dbConfig.Table_Key2.Replace("[$FieldName$]", hiColumn.FieldName);
                        if (i < hiColumns.Count - 1)
                            keys.AppendLine($"{_tempkey}{dbConfig.Field_Split}");
                        else
                            keys.AppendLine($"{_tempkey}");
                        i++;
                    }
                    _sql = _sql.Replace("[$Key$]", keys.ToString());
                    return _sql;
                }
                else
                    throw new Exception("索引创建必须指定字段");
            }
            else
                throw new Exception($"表[{tabname}]不存在,无法创建索引");

            throw new NotImplementedException();
        }

        public string DropIndex(string tabname, string indexname, bool isPrimary)
        {
            //暂未校验索引是否存在 由底层数据库抛出
            if (!isPrimary)
            {
                //暂未校验索引是否存在 由底层数据库抛出
                string _sql = dbConfig.Get_DropIndex.Replace("[$IndexName$]", indexname).Replace("[$Schema$]", Context.CurrentConnectionConfig.Schema)
                    .Replace("[$TabName$]", tabname);
                return _sql;
            }
            else
            {
                string _sql = dbConfig.Table_PrimaryKeyDrop.Replace("[$IndexName$]", indexname).Replace("[$Schema$]", Context.CurrentConnectionConfig.Schema)
                               .Replace("[$TabName$]", tabname);
                return _sql;
            }
        }

        /// <summary>
        /// 生成表重命名语句
        /// </summary>
        /// <param name="tabname"></param>
        /// <param name="newtabname"></param>
        /// <returns></returns>
        public string BuildReTableStatement(string tabname, string newtabname)
        {
            string _sql = dbConfig.Re_Table.Replace("[$TabName$]", $"{dbConfig.Schema_Pre}{this.Context.CurrentConnectionConfig.Schema}{dbConfig.Schema_After}.{dbConfig.Table_Pre}{tabname}{dbConfig.Table_After}").Replace("[$ReTabName$]", $"{dbConfig.Table_Pre}{newtabname}{dbConfig.Table_After}");
            return _sql;

        }

        public string BuildSqlCodeBlock(string sbSql)
        {
            return sbSql;
        }


        public int GetTableDataCount(string tabname)
        {
            string _sql = dbConfig.Get_TableDataCount.Replace("[$TabName$]", $"{dbConfig.Schema_Pre}{this.Context.CurrentConnectionConfig.Schema}{dbConfig.Schema_After}.{dbConfig.Table_Pre}{tabname}{dbConfig.Table_After}");
            var tabinfo = this.Context.DMInitalize.GetTabStruct(tabname);
            if (tabinfo.PrimaryKey.Any())
            {
                _sql = _sql.Replace("count(*)", $"count({dbConfig.Field_Pre}{tabinfo.PrimaryKey.FirstOrDefault().FieldName}{dbConfig.Field_After})");
            }
            string v = this.Context.DBO.ExecScalar(_sql).ToString();
            int _effect = Convert.ToInt32(v);
            return _effect;
        }
        public string BuildTabModiSql(TabInfo tabInfo)
        {
            throw new NotImplementedException();
        }
    }
}
