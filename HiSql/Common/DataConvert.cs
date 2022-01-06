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
