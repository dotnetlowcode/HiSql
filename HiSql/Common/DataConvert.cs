using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public static class DataConvert
    {


        public static void ToDynamic(dynamic o)
        {

            var ostr = JsonConvert.SerializeObject(o);

            dynamic json = Newtonsoft.Json.Linq.JToken.Parse(ostr) as dynamic;


            Type type = o.GetType();
            dynamic x = new { UserName = "tansar", Age = 33 };
            dynamic dyn = (dynamic)o;

            Console.WriteLine($"UserName:{dyn.UserName},Age:{dyn.Age}");
            //object o1=Activator.CreateInstance(type, true);

            //if (o1 != null)
            //{ 

            //}

        }


        public static List<T> ToList<T>(IDataReader dataReader)
        {
            List<T> lst = new List<T>();
            Type type = typeof(T);
            List<string> fieldNameList = new List<string>();
            string _value = "";
            List<PropertyInfo> listInfo = type.GetProperties().Where(p => p.CanWrite && p.CanRead && p.MemberType == MemberTypes.Property).ToList();//
            for (int i = 0; i < dataReader.FieldCount; i++)
            {
                fieldNameList.Add(dataReader.GetName(i));
            }
            if (listInfo.Count > 0)
            {
                while (dataReader.Read())
                {
                    T t1 = (T)Activator.CreateInstance(type, true);
                    if (listInfo.Count > fieldNameList.Count)
                    {
                        foreach (string n in fieldNameList)
                        {
                            PropertyInfo pinfo = listInfo.Where(p => p.Name.ToLower() == n.ToLower()).FirstOrDefault();
                            if (pinfo != null)
                            {


                                if (dataReader[n] is not DBNull)
                                {
                                    if (pinfo.PropertyType.FullName.IndexOf("bool") >= 0)
                                    {
                                        _value = dataReader[n].ToString().ToLower().Trim();
                                        if (_value == "1" || _value == "true")
                                            pinfo.SetValue(t1, true);
                                        else
                                            pinfo.SetValue(t1, false);
                                    }
                                    else
                                        pinfo.SetValue(t1, dataReader[n]);

                                }
                                else
                                {
                                    //暂不启用默认值
                                    //if (pinfo.PropertyType.IsIn<Type>(Constants.LongType, Constants.IntType, Constants.DecType, Constants.FloatType, Constants.ShortType,Constants.DobType))
                                    //{
                                    //    pinfo.SetValue(t1, 0);
                                    //}
                                    //else if (pinfo.PropertyType.IsIn<Type>(Constants.StringType))
                                    //{
                                    //    pinfo.SetValue(t1, "");
                                    //}
                                    //else if (pinfo.PropertyType.IsIn<Type>(Constants.BoolType))
                                    //{
                                    //    pinfo.SetValue(t1, false);
                                    //}

                                }
                            }
                        }
                        lst.Add(t1);

                    }
                    else
                    {
                        foreach (PropertyInfo pinfo in listInfo)
                        {
                            string n = fieldNameList.Where(fn => fn.ToLower() == pinfo.Name.ToLower()).FirstOrDefault();
                            if (!string.IsNullOrEmpty(n))
                            {

                                //当不为Null值时才赋值
                                if (dataReader[n] is not DBNull)
                                {
                                    if (pinfo.PropertyType.FullName.ToLower().IndexOf("bool") >= 0)
                                    {
                                        _value = dataReader[n].ToString().ToLower().Trim();
                                        if (_value == "1" || _value == "true")
                                            pinfo.SetValue(t1, true);
                                        else
                                            pinfo.SetValue(t1, false);
                                    }
                                    else
                                        pinfo.SetValue(t1, dataReader[n]);
                                }
                                else
                                {
                                    //暂不启用默认值
                                    //if (pinfo.PropertyType.IsIn<Type>(Constants.LongType, Constants.IntType, Constants.DecType, Constants.FloatType, Constants.ShortType, Constants.DobType))
                                    //{
                                    //    pinfo.SetValue(t1, 0);
                                    //}
                                    //else if (pinfo.PropertyType.IsIn<Type>(Constants.StringType))
                                    //{
                                    //    pinfo.SetValue(t1, "");
                                    //}
                                    //else if (pinfo.PropertyType.IsIn<Type>(Constants.BoolType))
                                    //{
                                    //    pinfo.SetValue(t1, false);
                                    //}

                                }


                            }
                        }
                        lst.Add(t1);
                    }
                }
            }
            else
                throw new Exception($"实体[{type.Name}]无可用属性节点无法进行映射");

            return lst;
        }

        public static List<ExpandoObject> ToEObject(IDataReader dataReader)
        {
            List<ExpandoObject> result = new List<ExpandoObject>();
            List<string> fieldNameList = new List<string>();
            for (int i = 0; i < dataReader.FieldCount; i++)
            {
                fieldNameList.Add(dataReader.GetName(i));
            }
            while (dataReader.Read())
            {

                TDynamic _dyn = new TDynamic();
                foreach (string n in fieldNameList)
                {
                    //针对于hana 的decimal特殊处理
                    if (dataReader[n].GetType().FullName.IndexOf("HanaDecimal") >= 0)
                    {
                        _dyn[n] = Convert.ToDecimal(dataReader[n].ToString());
                    }
                    else
                    {
                        if (dataReader[n] is not DBNull)
                            _dyn[n] = dataReader[n];
                    }
                }
                result.Add((ExpandoObject)_dyn);
            }
            dataReader.Close();
            return result;
        }

        public static async Task<List<ExpandoObject>> ToEObjectSync(Task<IDataReader> dataReaderSync)
        {
            var dataReader = await dataReaderSync;
            List<ExpandoObject> result = new List<ExpandoObject>();
            List<string> fieldNameList = new List<string>();
            for (int i = 0; i < dataReader.FieldCount; i++)
            {
                fieldNameList.Add(dataReader.GetName(i));
            }
            while (dataReader.Read())
            {

                TDynamic _dyn = new TDynamic();
                foreach (string n in fieldNameList)
                {
                    //针对于hana 的decimal特殊处理
                    if (dataReader[n].GetType().FullName.IndexOf("HanaDecimal") >= 0)
                    {
                        _dyn[n] = Convert.ToDecimal(dataReader[n].ToString());
                    }
                    else
                    {
                        if (dataReader[n] is not DBNull)
                            _dyn[n] = dataReader[n];
                    }
                }
                result.Add((ExpandoObject)_dyn);
            }
            dataReader.Close();
            return result;
        }


        public static List<TDynamic> ToDynamic(IDataReader dataReader)
        {
            List<TDynamic> result = new List<TDynamic>();
            List<string> fieldNameList = new List<string>();

            for (int i = 0; i < dataReader.FieldCount; i++)
            {
                fieldNameList.Add(dataReader.GetName(i));
            }
            while (dataReader.Read())
            {

                TDynamic _dyn = new TDynamic();
                foreach (string n in fieldNameList)
                {
                    if (dataReader[n] is not DBNull)
                        _dyn[n] = dataReader[n];
                }
                result.Add(_dyn);
            }
            dataReader.Close();
            return result;
        }



        static public T ToEntity<T>(DataRow dr) where T : new()
        {
            if (dr == null)
                return default(T);
            //T t = Activator.CreateInstance<T>();
            T t = new T();
            PropertyInfo[] propertys = t.GetType().GetProperties();
            DataColumnCollection Columns = dr.Table.Columns;
            foreach (PropertyInfo property in propertys)
            {
                if (!property.CanWrite)
                    continue;
                string columnName = property.Name;
                if (Columns.Contains(columnName))
                {
                    object value = dr[columnName];
                    if (value is DBNull)
                        continue;
                    try
                    {
                        //property.SetValue(t, Convert.ChangeType(value, property.PropertyType), null);
                        property.SetValue(t, value, null);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            return t;
        }



        /// <summary>
        /// 实体或匿名类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lst"></param>
        /// <returns></returns>
        static public DataTable ToTable<T>(List<T> lst,TabInfo tabInfo,string user="HiSql") {

            DataTable table= BuildDataTable(tabInfo);
            var _typname = "";
            Type _type = null ;

            Type _typedic = typeof(Dictionary<string, object>);
            Type _typedicstr = typeof(Dictionary<string, string>);
            if (lst.Count > 0)
            {
                _type = lst[0].GetType();
                _typname = lst[0].GetType().FullName;
            }

            else
                return table;



            if (_typname.IndexOf("TDynamic") >= 0 || _typname.IndexOf("ExpandoObject") >= 0)
            {
                List<Dictionary<string, object>> _list_data = new List<Dictionary<string, object>>();
                if (_typname.IndexOf("TDynamic") >= 0)
                {

                    foreach (var obj in lst)
                    {
                        TDynamic dyn = (dynamic)obj;
                        _list_data.Add((Dictionary<string, object>)dyn);
                    }

                }
                else if (_typname.IndexOf("ExpandoObject") >= 0)
                {
                    foreach (var obj in lst)
                    {
                        TDynamic dyn = new TDynamic(obj);
                        _list_data.Add((Dictionary<string, object>)dyn);
                    }

                }

                var columns = table.Columns;
                foreach (Dictionary<string, object> _dic in _list_data)
                {
                   
                    DataRow drow = table.NewRow();
                    foreach (DataColumn dc in columns)
                    {
                        object _value = null;
                        if (_dic.ContainsKey(dc.ColumnName))
                        {
                            _value = _dic[dc.ColumnName];
                        }
                        else if (_dic.ContainsKey(dc.ColumnName.ToLower()))
                        {
                            _value = _dic[dc.ColumnName.ToLower()];
                        }
                        else if (_dic.ContainsKey(dc.ColumnName.ToUpper()))
                        {
                            _value = _dic[dc.ColumnName.ToUpper()];
                        }
                        else
                        {
                            if (Constants.IsStandardUserField(dc.ColumnName))
                            {
                                _value = user;
                            }
                            if (Constants.IsStandardUserField(dc.ColumnName))
                            {
                                _value = DateTime.Now;
                            }
                        }
                        if (_value != null)
                        {
                            Type _typ = _value.GetType();
                            if (dc.DataType == Constants.StringType)
                            {
                                drow[dc.ColumnName] = _value;
                            }
                            else if (dc.DataType == Constants.DecType || dc.DataType == Constants.FloatType)
                            {
                                if (_value != null)
                                    drow[dc.ColumnName] = Convert.ToDecimal(_value);
                                else
                                    drow[dc.ColumnName] = 0;
                            }
                            else if (dc.DataType == Constants.IntType || dc.DataType == Constants.ShortType)
                            {
                                if (_value != null)
                                    drow[dc.ColumnName] = Convert.ToInt32(_value);
                                else
                                    drow[dc.ColumnName] = 0;
                            }
                            else if (dc.DataType == Constants.LongType)
                            {
                                if (_value != null)
                                    drow[dc.ColumnName] = Convert.ToInt64(_value);
                                else
                                    drow[dc.ColumnName] = 0;
                            }
                            else if (dc.DataType == Constants.DateType || dc.DataType == Constants.DateTimeOffsetType)
                            {
                                if (_value != null && (_typ == Constants.DateType || _typ == Constants.DateType))
                                    drow[dc.ColumnName] = _value;
                                else
                                    drow[dc.ColumnName] = DateTime.MinValue;
                            }
                            else if (dc.DataType == Constants.BoolType)
                            {
                                if (_value != null)
                                {
                                    if (_value.GetType() == Constants.BoolType)
                                        drow[dc.ColumnName] = _value;
                                }
                                else
                                    drow[dc.ColumnName] = false;
                            }
                        }
                    }

                    table.Rows.Add(drow);
                }
            }
            else if (_type == _typedic)
            {
                var columns = table.Columns;
                foreach (var data in lst)
                {
                    #region 解析 Dictionary<string, object>
                    Dictionary<string, object> _dic = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                    _dic = data as Dictionary<string, object>;
                    DataRow drow = table.NewRow();
                    foreach (DataColumn dc in columns)
                    {
                        object _value = null;
                        if (_dic.ContainsKey(dc.ColumnName))
                        {
                            _value = _dic[dc.ColumnName];
                        }
                        else if (_dic.ContainsKey(dc.ColumnName.ToLower()))
                        {
                            _value = _dic[dc.ColumnName.ToLower()];
                        }
                        else if (_dic.ContainsKey(dc.ColumnName.ToUpper()))
                        {
                            _value = _dic[dc.ColumnName.ToUpper()];
                        }
                        else
                        {
                            if (Constants.IsStandardUserField(dc.ColumnName))
                            {
                                _value = user;
                            }
                            if (Constants.IsStandardTimeField(dc.ColumnName))
                            {
                                _value = DateTime.Now;
                            }
                        }
                        if (_value != null)
                        {
                            Type _typ = _value.GetType();
                            if (dc.DataType == Constants.StringType)
                            {
                                drow[dc.ColumnName] = _value;
                            }
                            else if (dc.DataType == Constants.DecType || dc.DataType == Constants.FloatType)
                            {
                                if (_value != null)
                                    drow[dc.ColumnName] = Convert.ToDecimal(_value);
                                else
                                    drow[dc.ColumnName] = 0;
                            }
                            else if (dc.DataType == Constants.IntType || dc.DataType == Constants.ShortType)
                            {
                                if (_value != null)
                                    drow[dc.ColumnName] = Convert.ToInt32(_value);
                                else
                                    drow[dc.ColumnName] = 0;
                            }
                            else if (dc.DataType == Constants.LongType)
                            {
                                if (_value != null)
                                    drow[dc.ColumnName] = Convert.ToInt64(_value);
                                else
                                    drow[dc.ColumnName] = 0;
                            }
                            else if (dc.DataType == Constants.DateType || dc.DataType == Constants.DateTimeOffsetType)
                            {
                                if (_value != null && (_typ == Constants.DateType || _typ == Constants.DateType))
                                    drow[dc.ColumnName] = _value;
                                else
                                    drow[dc.ColumnName] = DateTime.MinValue;
                            }
                            else if (dc.DataType == Constants.BoolType)
                            {
                                if (_value != null)
                                {
                                    if (_value.GetType() == Constants.BoolType)
                                        drow[dc.ColumnName] = _value;
                                }
                                else
                                    drow[dc.ColumnName] = false;
                            }
                        }
                    }

                    table.Rows.Add(drow);

                    #endregion
                }
            }
            else if (_type == _typedicstr)
            {
                var columns = table.Columns;
                foreach (var  data in lst)
                {
                    #region 解析 Dictionary<string, string>
                    Dictionary<string, string> _dic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    _dic = data as Dictionary<string, string>;
                    DataRow drow = table.NewRow();
                    foreach (DataColumn dc in columns)
                    {
                        string _value = null;
                        if (_dic.ContainsKey(dc.ColumnName))
                        {
                            _value = _dic[dc.ColumnName];
                        }
                        else if (_dic.ContainsKey(dc.ColumnName.ToLower()))
                        {
                            _value = _dic[dc.ColumnName.ToLower()];
                        }
                        else if (_dic.ContainsKey(dc.ColumnName.ToUpper()))
                        {
                            _value = _dic[dc.ColumnName.ToUpper()];
                        }
                        else
                        {
                            if (Constants.IsStandardUserField(dc.ColumnName))
                            {
                                _value = user;
                            }
                            if (Constants.IsStandardTimeField(dc.ColumnName))
                            {
                                _value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                            }
                        }
                        if (_value != null)
                        {
                            if (dc.DataType == Constants.StringType)
                            {
                                drow[dc.ColumnName] = _value;
                            }
                            else if (dc.DataType == Constants.DecType || dc.DataType == Constants.FloatType)
                            {
                                if (!string.IsNullOrEmpty(_value))
                                    drow[dc.ColumnName] = Convert.ToDecimal(_value);
                                else
                                    drow[dc.ColumnName] = 0;
                            }
                            else if (dc.DataType == Constants.IntType || dc.DataType == Constants.ShortType)
                            {
                                if (!string.IsNullOrEmpty(_value))
                                    drow[dc.ColumnName] = Convert.ToInt32(_value);
                                else
                                    drow[dc.ColumnName] = 0;
                            }
                            else if (dc.DataType == Constants.LongType)
                            {
                                if (!string.IsNullOrEmpty(_value))
                                    drow[dc.ColumnName] = Convert.ToInt64(_value);
                                else
                                    drow[dc.ColumnName] = 0;
                            }
                            else if (dc.DataType == Constants.DateType || dc.DataType == Constants.DateTimeOffsetType)
                            {
                                if (Constants.IsStandardTimeField(dc.ColumnName))
                                {
                                    drow[dc.ColumnName] = DateTime.Now;
                                }
                                else {
                                    if (!string.IsNullOrEmpty(_value))
                                        drow[dc.ColumnName] = Convert.ToDateTime(_value);
                                    else
                                        drow[dc.ColumnName] = DateTime.MinValue;
                                }
                                    
                            }
                            else if (dc.DataType == Constants.BoolType)
                            {
                                if (!string.IsNullOrEmpty(_value))
                                {
                                    _value = _value.Trim().ToLower(); ;
                                    if (_value == "true" || _value == "1")
                                        drow[dc.ColumnName] = true;
                                    else
                                        drow[dc.ColumnName] = false;
                                }
                                else
                                    drow[dc.ColumnName] = false;
                            }
                        }
                    }

                    table.Rows.Add(drow);

                    #endregion
                }
            }
            else
            {
                //实体类

                #region 解析实体类
                Type type = lst[0].GetType();
                PropertyInfo[] properties = type.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

                var columns = table.Columns;
                for (int i = 0; i < lst.Count; i++)
                {
                    DataRow drow = table.NewRow();
                    var row = new object[table.Columns.Count];
                    var rowidx = 0;
                    foreach (DataColumn dc in columns)
                    {
                        var pinfo = properties.Where(p => p.Name.ToLower() == dc.ColumnName.ToLower()).FirstOrDefault();
                        if (pinfo != null)
                        {
                            var obj = pinfo.GetValue(lst[i]);
                            drow[dc.ColumnName] = obj;
                        }
                        else
                        {
                            if (Constants.IsStandardUserField(dc.ColumnName))
                            {
                                drow[dc.ColumnName] = user;
                                //row[i] = user;
                            }
                        }
                        rowidx++;
                    }
                    table.Rows.Add(drow);
                }
                #endregion
            }
            return table;
        }
        static public DataTable BuildDataTable(TabInfo tabInfo)
        {
            DataTable table = null;
            if (tabInfo != null)
            {
                table = new DataTable();
                var lstcolumns = tabInfo.GetColumns;
                foreach (var column in lstcolumns)
                {
                    DataColumn dc = new DataColumn();

                    dc.ColumnName = column.FieldName;

                    if (Constants.IsStandardTimeField(column.FieldName))
                    {
                        if (column.DBDefault == HiTypeDBDefault.FUNDATE)
                        {
                            continue;//说明该字段在数据中设置了默认日期
                        }
                    }
                    else if (column.IsIdentity)
                    {
                        continue;//自增长字段忽略写入
                    }

                    if (column.FieldType.IsIn<HiType>(HiType.VARCHAR, HiType.TEXT, HiType.NVARCHAR, HiType.NCHAR, HiType.CHAR, HiType.GUID))
                    {
                        dc.DataType = typeof(string);

                    }
                    else if (column.FieldType.IsIn<HiType>(HiType.DATE, HiType.DATETIME))
                    {
                        dc.DataType = typeof(DateTime);
                    }
                    else if (column.FieldType.IsIn<HiType>(HiType.BOOL))
                    {
                        dc.DataType = typeof(DateTime);
                    }
                    else if (column.FieldType.IsIn<HiType>(HiType.INT, HiType.SMALLINT))
                    {
                        dc.DataType = typeof(Int32);
                    }
                    else if (column.FieldType.IsIn<HiType>(HiType.BIGINT))
                    {
                        dc.DataType = typeof(Int64);
                    }
                    else if (column.FieldType.IsIn<HiType>(HiType.DECIMAL))
                    {
                        dc.DataType = typeof(decimal);
                    }

                    else
                    {
                        dc.DataType = typeof(string);
                        //忽略
                        continue;
                    }

                    table.Columns.Add(dc);
                }

                if (table.Columns.Count == 0)
                    throw new Exception($"无有效字段信息无法生成DataTable列头");
            }


            return table;
        }

        static public string ToCSV<T>(List<T> lst, TabInfo tabInfo, DBType dbType,bool hasHeader=false, string user="HiSql")
        {
            StringBuilder sbcsv = new StringBuilder();
            DataTable table = BuildDataTable(tabInfo);
            var _typname = "";
            Type _type = null;

        
            
         


            Type _typedic = typeof(Dictionary<string, object>);
            Type _typedicstr = typeof(Dictionary<string, string>);
            if (lst.Count > 0)
            {
                _type = lst[0].GetType();
                _typname = lst[0].GetType().FullName;
            }

            else
                return "";

            if (hasHeader)
            {
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    sbcsv.Append($"\"{table.Columns[i].ColumnName}\"");
                    if (i < table.Columns.Count - 1)
                        sbcsv.Append(",");
                }
                sbcsv.AppendLine();
            }
            



            if (_typname.IndexOf("TDynamic") >= 0 || _typname.IndexOf("ExpandoObject") >= 0)
            {
                List<Dictionary<string, object>> _list_data = new List<Dictionary<string, object>>();
                if (_typname.IndexOf("TDynamic") >= 0)
                {

                    foreach (var obj in lst)
                    {
                        TDynamic dyn = (dynamic)obj;
                        _list_data.Add((Dictionary<string, object>)dyn);
                    }

                }
                else if (_typname.IndexOf("ExpandoObject") >= 0)
                {
                    foreach (var obj in lst)
                    {
                        TDynamic dyn = new TDynamic(obj);
                        _list_data.Add((Dictionary<string, object>)dyn);
                    }

                }

                var columns = table.Columns;
                int _idx = 0;
                foreach (Dictionary<string, object> _dic in _list_data)
                {

                    _idx = 0;
                    foreach (DataColumn dc in columns)
                    {
                        object _value = null;
                        if (_dic.ContainsKey(dc.ColumnName))
                        {
                            _value = _dic[dc.ColumnName];
                        }
                        else if (_dic.ContainsKey(dc.ColumnName.ToLower()))
                        {
                            _value = _dic[dc.ColumnName.ToLower()];
                        }
                        else if (_dic.ContainsKey(dc.ColumnName.ToUpper()))
                        {
                            _value = _dic[dc.ColumnName.ToUpper()];
                        }
                        else
                        {
                            if (Constants.IsStandardUserField(dc.ColumnName))
                            {
                                _value = user;
                            }
                            if (Constants.IsStandardUserField(dc.ColumnName))
                            {
                                _value = DateTime.Now;
                            }
                        }
                        if (_value != null)
                        {
                            Type _typ = _value.GetType();
                            if (dc.DataType == Constants.StringType)
                            {
                                //drow[dc.ColumnName] = _value;
                                sbcsv.Append($"\"{_value.ToString().Replace("\"", "\"\"")}\"");
                            }
                            else if (dc.DataType == Constants.DecType || dc.DataType == Constants.FloatType)
                            {
                                if (_value != null)
                                    sbcsv.Append(_value.ToString());
                                else
                                    sbcsv.Append("0");
                            }
                            else if (dc.DataType == Constants.IntType || dc.DataType == Constants.ShortType)
                            {
                                if (_value != null)
                                    sbcsv.Append(_value.ToString());
                                else
                                    sbcsv.Append("0");
                            }
                            else if (dc.DataType == Constants.LongType)
                            {
                                if (_value != null)
                                    sbcsv.Append(_value.ToString());
                                else
                                    sbcsv.Append("0");
                            }
                            else if (dc.DataType == Constants.DateType || dc.DataType == Constants.DateTimeOffsetType)
                            {

                                if (Constants.IsStandardTimeField(dc.ColumnName))
                                {
                                    sbcsv.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                }
                                else
                                {
                                    if (_value != null && (_typ == Constants.DateType || _typ == Constants.DateType))
                                        sbcsv.Append(((DateTime)_value).ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                    else
                                        sbcsv.Append(DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                }
                                
                            }
                            else if (dc.DataType == Constants.BoolType)
                            {
                                if (_value != null)
                                {
                                    if (dbType.IsIn<DBType>(DBType.PostGreSql, DBType.Hana, DBType.MySql))
                                    {
                                        if ((Boolean)_value == true)
                                            sbcsv.Append("True");
                                        else
                                            sbcsv.Append("False");
                                    }
                                    else
                                    {
                                        if ((Boolean)_value == true)
                                            sbcsv.Append("1");
                                        else
                                            sbcsv.Append("0");
                                    }
                                }
                                else
                                {
                                    if (dbType.IsIn<DBType>(DBType.PostGreSql, DBType.Hana, DBType.MySql))
                                    {
                                        sbcsv.Append("False");
                                    }
                                    else
                                        sbcsv.Append("0");
                                }
                                    
                            }
                        }

                        if (_idx < columns.Count - 1)
                            sbcsv.Append(",");
                        _idx++;
                    }
                    sbcsv.AppendLine();
                    //table.Rows.Add(drow);
                }
            }
            else if (_type == _typedic)
            {
                var columns = table.Columns;
                int _idx = 0;
                foreach (var data in lst)
                {
                    #region 解析 Dictionary<string, object>
                    Dictionary<string, object> _dic = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                    _dic = data as Dictionary<string, object>;
                    DataRow drow = table.NewRow();
                    _idx = 0;
                    foreach (DataColumn dc in columns)
                    {
                        object _value = null;
                        if (_dic.ContainsKey(dc.ColumnName))
                        {
                            _value = _dic[dc.ColumnName];
                        }
                        else if (_dic.ContainsKey(dc.ColumnName.ToLower()))
                        {
                            _value = _dic[dc.ColumnName.ToLower()];
                        }
                        else if (_dic.ContainsKey(dc.ColumnName.ToUpper()))
                        {
                            _value = _dic[dc.ColumnName.ToUpper()];
                        }
                        else
                        {
                            if (Constants.IsStandardUserField(dc.ColumnName))
                            {
                                _value = user;
                            }
                            if (Constants.IsStandardTimeField(dc.ColumnName))
                            {
                                _value = DateTime.Now;
                            }
                        }
                        if (_value != null)
                        {
                            Type _typ = _value.GetType();
                            if (dc.DataType == Constants.StringType)
                            {
                                
                                sbcsv.Append($"\"{_value.ToString().Replace("\"", "\"\"")}\"");
                            }
                            else if (dc.DataType == Constants.DecType || dc.DataType == Constants.FloatType)
                            {
                                if (_value != null)
                                    sbcsv.Append(_value.ToString());
                                else
                                    sbcsv.Append("0");
                            }
                            else if (dc.DataType == Constants.IntType || dc.DataType == Constants.ShortType)
                            {
                                if (_value != null)
                                    sbcsv.Append(_value.ToString());
                                else
                                    sbcsv.Append("0");
                            }
                            else if (dc.DataType == Constants.LongType)
                            {
                                if (_value != null)
                                    sbcsv.Append(_value.ToString());
                                else
                                    sbcsv.Append("0");
                            }
                            else if (dc.DataType == Constants.DateType || dc.DataType == Constants.DateTimeOffsetType)
                            {
                                
                                if (Constants.IsStandardTimeField(dc.ColumnName))
                                {
                                    sbcsv.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                }
                                else
                                {
                                    if (_value != null && (_typ == Constants.DateType || _typ == Constants.DateType))
                                        sbcsv.Append(((DateTime)_value).ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                    else
                                        sbcsv.Append(DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                }
                            }
                            else if (dc.DataType == Constants.BoolType)
                            {
                                if (_value != null)
                                {
                                    if (dbType.IsIn<DBType>(DBType.PostGreSql, DBType.Hana, DBType.MySql))
                                    {
                                        if ((Boolean)_value == true)
                                            sbcsv.Append("True");
                                        else
                                            sbcsv.Append("False");
                                    }
                                    else
                                    {
                                        if ((Boolean)_value == true)
                                            sbcsv.Append("1");
                                        else
                                            sbcsv.Append("0");
                                    }
                                }
                                else
                                {
                                    if (dbType.IsIn<DBType>(DBType.PostGreSql, DBType.Hana, DBType.MySql))
                                    {
                                        sbcsv.Append("False");
                                    }
                                    else
                                        sbcsv.Append("0");
                                }
                            }
                        }
                        if (_idx < columns.Count - 1)
                            sbcsv.Append(",");
                        _idx++;
                    }
                    
                    sbcsv.AppendLine();
                    #endregion
                }
            }
            else if (_type == _typedicstr)
            {
                var columns = table.Columns;
                int _idx=0;
                foreach (var data in lst)
                {
                    #region 解析 Dictionary<string, string>
                    Dictionary<string, string> _dic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    _dic = data as Dictionary<string, string>;
                    DataRow drow = table.NewRow();
                    _idx = 0;
                    foreach (DataColumn dc in columns)
                    {
                        string _value = null;
                        if (_dic.ContainsKey(dc.ColumnName))
                        {
                            _value = _dic[dc.ColumnName];
                        }
                        else if (_dic.ContainsKey(dc.ColumnName.ToLower()))
                        {
                            _value = _dic[dc.ColumnName.ToLower()];
                        }
                        else if (_dic.ContainsKey(dc.ColumnName.ToUpper()))
                        {
                            _value = _dic[dc.ColumnName.ToUpper()];
                        }
                        else
                        {
                            if (Constants.IsStandardUserField(dc.ColumnName))
                            {
                                _value = user;
                            }
                            if (Constants.IsStandardTimeField(dc.ColumnName))
                            {
                                _value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                            }
                        }
                        if (_value != null)
                        {
                            if (dc.DataType == Constants.StringType)
                            {
                                sbcsv.Append($"\"{_value.ToString().Replace("\"", "\"\"")}\"");
                            }
                            else if (dc.DataType == Constants.DecType || dc.DataType == Constants.FloatType)
                            {
                                if (!string.IsNullOrEmpty(_value))
                                    sbcsv.Append(_value.ToString());
                                else
                                    sbcsv.Append("0");
                            }
                            else if (dc.DataType == Constants.IntType || dc.DataType == Constants.ShortType)
                            {
                                if (!string.IsNullOrEmpty(_value))
                                    sbcsv.Append(_value.ToString());
                                else
                                    sbcsv.Append("0");
                            }
                            else if (dc.DataType == Constants.LongType)
                            {
                                if (!string.IsNullOrEmpty(_value))
                                    sbcsv.Append(_value.ToString());
                                else
                                    sbcsv.Append("0");
                            }
                            else if (dc.DataType == Constants.DateType || dc.DataType == Constants.DateTimeOffsetType)
                            {
                                
                                if (Constants.IsStandardTimeField(dc.ColumnName))
                                {
                                    sbcsv.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(_value))
                                        sbcsv.Append(Convert.ToDateTime(_value).ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                    else
                                        sbcsv.Append(DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                }

                            }
                            else if (dc.DataType == Constants.BoolType)
                            {
                                if (_value != null)
                                {
                                    if (dbType.IsIn<DBType>(DBType.PostGreSql, DBType.Hana, DBType.MySql))
                                    {
                                        if (_value == "true" || _value == "1")
                                            sbcsv.Append("True");
                                        else
                                            sbcsv.Append("False");
                                    }
                                    else
                                    {
                                        if (_value == "true" || _value == "1")
                                            sbcsv.Append("1");
                                        else
                                            sbcsv.Append("0");
                                    }
                                }
                                else
                                {
                                    if (dbType.IsIn<DBType>(DBType.PostGreSql, DBType.Hana, DBType.MySql))
                                    {
                                        sbcsv.Append("False");
                                    }
                                    else
                                        sbcsv.Append("0");
                                }
                            }
                        }
                        if (_idx < columns.Count - 1)
                            sbcsv.Append(",");
                        _idx++;
                    }

                    sbcsv.AppendLine();
                    
                    #endregion
                }
            }
            else
            {
                //实体类

                #region 解析实体类
                Type type = lst[0].GetType();
                PropertyInfo[] properties = type.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

                var columns = table.Columns;
                for (int i = 0; i < lst.Count; i++)
                {
                    DataRow drow = table.NewRow();
                    var row = new object[table.Columns.Count];
                    var rowidx = 0;
                    foreach (DataColumn dc in columns)
                    {
                        
                        var pinfo = properties.Where(p => p.Name.ToLower() == dc.ColumnName.ToLower()).FirstOrDefault();
                        if (pinfo != null)
                        {
                            var obj = pinfo.GetValue(lst[i]);
                            if (dc.DataType == Constants.StringType)
                            {

                                sbcsv.Append($"\"{obj.ToString().Replace("\"", "\"\"")}\"");
                            }
                            else if (dc.DataType == Constants.BoolType)
                            {
                                if (obj != null)
                                {
                                    if (dbType.IsIn<DBType>(DBType.PostGreSql, DBType.Hana, DBType.MySql))
                                    {
                                        if (obj.ToString().ToLower() == "true" || obj.ToString() == "1")
                                            sbcsv.Append("True");
                                        else
                                            sbcsv.Append("False");
                                    }
                                    else
                                    {
                                        if (obj.ToString().ToLower() == "true" || obj.ToString() == "1")
                                            sbcsv.Append("1");
                                        else
                                            sbcsv.Append("0");
                                    }
                                }
                                else
                                {
                                    if (dbType.IsIn<DBType>(DBType.PostGreSql, DBType.Hana, DBType.MySql))
                                    {
                                        sbcsv.Append("False");
                                    }
                                    else
                                        sbcsv.Append("0");
                                }
                            }
                            else
                            {
                                sbcsv.Append($"{obj.ToString()}");
                            }
                        }
                        else
                        {
                            if (Constants.IsStandardUserField(dc.ColumnName))
                            {
                                //drow[dc.ColumnName] = user;
                                //row[i] = user;
                                sbcsv.Append($"\"{user}\"");
                            }
                        }
                        if (rowidx < columns.Count - 1)
                            sbcsv.Append(",");

                        rowidx++;
                        
                    }
                    sbcsv.AppendLine();
                }
                #endregion
            }
            

            return sbcsv.ToString();
        }

        /// <summary>
        /// 将TABLE数据转成csv
        /// </summary>
        /// <param name="table"></param>
        /// <param name="dbType"></param>
        /// <returns></returns>
        static public string ToCSV(DataTable table, DBType dbType, bool hasHeader = false)
        {
            
            /*理论上讲还要根据字段的类型及长度 来校验数据 以期再更新*/
            StringBuilder sbcsv = new StringBuilder();

            if (hasHeader)
            {
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    sbcsv.Append($"\"{table.Columns[i].ColumnName}\"");
                    if (i < table.Columns.Count - 1)
                        sbcsv.Append(",");
                }
                sbcsv.AppendLine();
            }

            int colcount = table.Columns.Count;
            foreach (DataRow drow in table.Rows)
            {
                for (int i = 0; i < colcount; i++)
                {
                    if (table.Columns[i].DataType == Constants.StringType)
                    {
                        sbcsv.Append($"\"{drow[i].ToString().Replace("\"", "\"\"")}\"");
                    }
                    else if (table.Columns[i].DataType == Constants.BoolType)
                    {
                        if (dbType.IsIn<DBType>(DBType.PostGreSql, DBType.Hana, DBType.MySql))
                        {
                            if ((Boolean)drow[i] == true)
                                sbcsv.Append("True");
                            else
                                sbcsv.Append("False");
                        }
                        else
                        {
                            if ((Boolean)drow[i] == true)
                                sbcsv.Append("1");
                            else
                                sbcsv.Append("0");
                        }
                    }else
                        sbcsv.Append(drow[i].ToString());
                    if (i <= colcount - 1)
                        sbcsv.Append(",");
                }

                sbcsv.AppendLine();
            }

            return sbcsv.ToString();
        }



        /// <summary>
        /// 把DataTable对象转成实体类的列表。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        static public List<T> ToEntityList<T>(DataTable dt) where T : new()
        {
            List<T> list = new List<T>();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(ToEntity<T>(dr));
                }
            }
            return list;
        }

        

        /// <summary>
        /// 把IDataReader对象转成实体类。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr">需要参数的方法dr.Read()返回值为true。</param>
        /// <returns></returns>
        static private T DataReaderToEntity<T>(IDataReader dr) where T : new()
        {
            T t = new T();
            PropertyInfo[] propertys = t.GetType().GetProperties();
            List<string> fieldNameList = new List<string>();
            for (int i = 0; i < dr.FieldCount; i++)
            {
                fieldNameList.Add(dr.GetName(i));
            }
            foreach (PropertyInfo property in propertys)
            {
                if (!property.CanWrite)
                    continue;
                string fieldName = property.Name;
                if (fieldNameList.Contains(fieldName))
                {
                    object value = dr[fieldName];
                    if (value is DBNull)
                        continue;
                    try
                    {
                        property.SetValue(t, value, null);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            return t;
        }

        /// <summary>
        /// 把IDataReader对象转成实体类。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        static public T ToEntity<T>(IDataReader dr) where T : new()
        {
            if (dr != null && dr.Read())
            {
                return DataReaderToEntity<T>(dr);
            }
            return default(T);
        }

        /// <summary>
        /// 把IDataReader对象转成实体类的列表。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        static public List<T> ToEntityList<T>(IDataReader dr) where T : new()
        {
            List<T> list = new List<T>();
            if (dr != null)
            {
                while (dr.Read())
                {
                    list.Add(DataReaderToEntity<T>(dr));
                }
            }
            return list;
        }

        static public List<string> DataTableFieldToList(DataTable dt, string field)
        {
            List<string> lst = new List<string>();
            if (dt != null && dt.Columns.Contains(field) && dt.Rows.Count > 0)
            {
                foreach (DataRow drow in dt.Rows)
                {
                    lst.Add(drow[field].ToString());
                }
            }
            return lst;
        }
        static public HashSet<string> DataTableFieldToHashSet(DataTable dt, string field)
        {
            HashSet<string> hash = new HashSet<string>();
            if (dt != null && dt.Columns.Contains(field) && dt.Rows.Count > 0)
            {
                foreach (DataRow drow in dt.Rows)
                {
                    hash.Add(drow[field].ToString());
                }
            }
            return hash;
        }
    }
}
