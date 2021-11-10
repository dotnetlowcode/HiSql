using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Data;

namespace HiSql
{

    /// <summary>
    /// 公共类
    /// </summary>
    public static partial  class HiSqlCommProvider
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
            string _keyname = Constants.KEY_TABLE_CACHE_NAME.Replace("[$DBSERVER$]",  table.DbServer)
                .Replace("[$SCHEMA$]", table.Schema)
                .Replace("[$TABLE$]", table.TabName)
                ;
        }
        public static TabInfo InitTabMaping(string tabnane,Func<TabInfo> GetInfo)
        {
            
            string _keyname = Constants.KEY_TABLE_CACHE_NAME.Replace("[$TABLE$]", tabnane);
            return CacheContext.MCache.GetOrCreate<TabInfo>(_keyname, () => {
                return GetInfo();
            });
                
           
        }
        

        /// <summary>
        /// 将数据库类型转成HiSql的类型
        /// </summary>
        /// <param name="_dbmapping"></param>
        /// <param name="fieldtype"></param>
        /// <returns></returns>
        public static  HiType ConvertToHiType(Dictionary<HiType, string> _dbmapping, string fieldtype)
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
                 && table.Rows.Count>0
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
                    hiColumn.ColumnName = drow["FieldName"].ToString().Trim();
                    //hiColumn.FieldType
                    hiColumn.IsPrimary = drow["IsPrimary"].ToString().Trim().IsIn<string>("1","True") ? true : false;
                    hiColumn.IsIdentity= drow["IsIdentity"].ToString().Trim().IsIn<string>("1", "True") ? true : false;
                    hiColumn.IsBllKey = hiColumn.IsPrimary;
                    hiColumn.SortNum = Convert.ToInt32(drow["FieldNo"].ToString().Trim());
                    hiColumn.FieldType = ConvertToHiType(_dbmapping,drow["FieldType"].ToString().ToLower().Trim());
                    
                    hiColumn.FieldLen = Convert.ToInt32(string.IsNullOrEmpty(drow["Lens"].ToString().Trim())?"0": drow["Lens"].ToString().Trim());

                    if (hiColumn.FieldType.IsIn<HiType>(HiType.DECIMAL))
                    {
                        hiColumn.FieldDec= Convert.ToInt32(string.IsNullOrEmpty(drow["PointDec"].ToString().Trim()) ? "0" : drow["PointDec"].ToString().Trim());
                    }
                    hiColumn.IsNull= drow["IsNull"].ToString().Trim().IsIn<string>("1", "True") ? true : false;
                    hiColumn.IsShow = true;
                    hiColumn.SrchMode = SrchMode.Single;
                    hiColumn.IsSys = false;
                    hiColumn.FieldDesc= drow["FieldDesc"].ToString().Trim();
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
                                            prop.SetValue(hiTable,  tabModel.Rows[0][prop.Name]);
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
                                            else
                                                prop.SetValue(hiColumn, drow[prop.Name]);
                                        }
                                    }



                                    break;
                            }

                        }
                        else
                        {
                            if (prop.Name == "ColumnName")
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
            CacheContext.MCache.GetOrCreate<TabInfo>(_keyname, () => {
                TabInfo tabinfo = new TabInfo();
                HiTable hitabs = new HiTable(); 
                tabinfo.EntityName = type.Name;
                tabinfo.DbTabName = type.Name;
                //类属性必须挂上 HiTable特性
                var  _hitabs = type.GetCustomAttributes(true).Where(t => t is HiTable).Select(it=>(HiTable)it).FirstOrDefault();
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
                        if (hiColumn.ColumnName == string.Empty)
                        {
                            hiColumn.ColumnName = n.Name;
                        }
                        if (hiColumn.FieldDesc == string.Empty)
                        {
                            hiColumn.FieldDesc = hiColumn.ColumnName;
                        }

                        _hicolumn =hiColumn.MoveCross< HiColumn>(_hicolumn);

                        HiType ftype = n.PropertyType.ToDbFieldType();
                        //没有特别标记忽略
                        if (!hiColumn.IsIgnore)
                        {
                            tabinfo.Columns.Add(_hicolumn);
                        }
                    }
                    else
                    {
                        _hicolumn.ColumnName = n.Name;
                        tabinfo.Columns.Add(_hicolumn);
                       
                    }
                }
                return tabinfo;
            });
        }
    }
}
