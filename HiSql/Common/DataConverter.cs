using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public static class DataConverter<T>
    {
        public static ConcurrentDictionary<string, Func<T, string>> CollectGetValueKeyHandler =
         new ConcurrentDictionary<string, Func<T, string>>();
    }
        public static class DataConverter
    {
        static ConcurrentDictionary<string, List<PropertyInfo>> typePropertyForCollect = new ConcurrentDictionary<string, List<PropertyInfo>>();

        static ConcurrentDictionary<string, object> copiers = new ConcurrentDictionary<string, object>();

        static Object lockHandleDict = new Object();
        static Object lockDataRecordHandler = new Object();

        static Dictionary<string, Func<DataRow, object>> handleDict =
           new Dictionary<string, Func<DataRow, object>>();

        static Dictionary<string, Func<IDataRecord, List<string>, object>> DataRecordHandler =
           new Dictionary<string, Func<IDataRecord, List<string>, object>>();


        static Object lockCloneEntityHandler = new Object();
        private static ConcurrentDictionary<string, Func<object, object>> CloneEntityHandler { set; get; } = new ConcurrentDictionary<string, Func<object, object>>();
        
        static Object lockCompareEntityHandler = new Object();

        static Dictionary<string, Func<object, object, bool>> CompareEntityHandler =
          new Dictionary<string, Func<object, object, bool>>();


        public static List<T> ToList<T>(DataTable table) where T : class, new()
        {
            List<T> list = new List<T>();
            if (table == null || table.Rows.Count == 0)
                return list;

            Func<DataRow, object> handler = null;
            string key = string.IsNullOrEmpty(table.TableName) ? "DataTable" : table.TableName + "_" + typeof(T).Name;
            if (handleDict.ContainsKey(key))
            {
                handler = (Func<DataRow, object>)handleDict[key];
            }
            else
            {
                lock (lockHandleDict)
                {
                    if (handleDict.ContainsKey(key))
                    {
                        handler = (Func<DataRow, object>)handleDict[key];
                    }
                    else
                    {
                        DataTableEntityBuilder<T> eblist = DataTableEntityBuilder<T>.CreateBuilderOfDataRow();
                        handler = eblist.DataRowHandler;
                        handleDict.Add(key, handler);
                    }
                }
            }


            foreach (DataRow info in table.Rows)
            {
                list.Add((T)handler(info));
            }


            return list;
        }

        public static T DataReaderToEntity<T>(IDataReader dataReader, List<string> fieldNameList) where T : class, new()
        {
            T t = new T();
            if (dataReader == null)
                return t;
            Func<IDataRecord, List<string>, object> handler = null;
            string key = "DataReader_" + typeof(T).Name;

            if (DataRecordHandler.ContainsKey(key))
            {
                handler = (Func<IDataRecord, List<string>, object>)DataRecordHandler[key];
            }
            else
            {
                lock (lockDataRecordHandler)
                {
                    if (DataRecordHandler.ContainsKey(key))
                    {
                        handler = (Func<IDataRecord, List<string>, object>)DataRecordHandler[key];
                    }
                    else
                    {
                        DataTableEntityBuilder<T> eblist = DataTableEntityBuilder<T>.CreateBuilderOfDataRecord();
                        handler = eblist.DataRecordHandler;
                        DataRecordHandler.Add(key, handler);
                    }
                }
            }
            List<string> cols = new List<string>();
            for (int i = 0; i < dataReader.FieldCount; i++)
            {
                cols.Add(dataReader.GetName(i).ToLower());
            }
            t = ((T)handler(dataReader, cols));
            return t;
        }


        public static object CloneObjectWithILExt(object obj1)
        {
            var s = CloneObjectWithIL(obj1);
            return s;
        }
        public static T CloneObjectWithIL<T>(T myObject)
        {
            Type t1 = myObject.GetType();

            string key = "CloneObjectWithIL_" + t1.FullName;

            Func<object, object> handler = null;
            if (CloneEntityHandler.ContainsKey(key))
            {
                handler = CloneEntityHandler[key];
            }
            else
            {
                lock (lockCloneEntityHandler)
                {
                    if (CloneEntityHandler.ContainsKey(key))
                    {
                        handler = CloneEntityHandler[key];
                    }
                    else
                    {

                        DataTableEntityBuilder<T> eblist = DataTableEntityBuilder<T>.CreateBuilderOfCloneObject(myObject);
                        handler = eblist.CloneEntityHandler;
                        CloneEntityHandler.TryAdd(key, handler);
                    }
                }
            }
            return (T)handler(myObject);
        }

        public static object MoveCrossWithILExt(object sourceObj, object targetObj)
        {
            MoveCrossWithIL(sourceObj, targetObj);
            return targetObj;
        }

        public static bool MoveCrossWithIL<T,T1>(T sourceObj, T1 targetObj, List<PropertyInfo> onlyProperty = null, bool isSum = false)
        {
            if (sourceObj == null) return false;
            Type typeSource = sourceObj.GetType();
            Type typeTarget = targetObj.GetType();
            string key = "MoveCrossWithIL"+ (isSum ? "SUM" : "") + typeSource.Name + "_" + typeTarget.Name;
           
            if (onlyProperty != null)
            {
                key = "MoveCrossWithIL" + (isSum ? "SUM" : "") + typeSource.Name + "_" + string.Join("_", onlyProperty.Select(t=>t.Name));
            }
            Func<object, object,bool> handler = null;
            if (CompareEntityHandler.ContainsKey(key))
            {
                handler = CompareEntityHandler[key];
            }
            else
            {
                lock (lockCompareEntityHandler)
                {
                    if (CompareEntityHandler.ContainsKey(key))
                    {
                        handler = CompareEntityHandler[key];
                    }
                    else
                    {
                        handler = DataTableEntityBuilder<T>.CreateBuilderOfMoveCross( typeSource, typeTarget, onlyProperty, isSum);
                        CompareEntityHandler.Add(key, handler);
                    }
                }
            }
            return (bool)handler(sourceObj, targetObj);
        }

        private static string CollectGetValueKeyWithIL<T>(T sourceObj, Type typeSource, List<PropertyInfo> onlyProperty = null)
        {
            if (sourceObj == null) return "";
            string key = "CollectGetValueKeyWithIL" +  typeSource.Name;

            if (onlyProperty != null)
            {
                key = "CollectGetValueKeyWithIL" + typeSource.Name + "_" + string.Join("_", onlyProperty.Select(t => t.Name));
            }
            Func<T, string> handler = null;
            if (DataConverter<T>.CollectGetValueKeyHandler.ContainsKey(key))
            {
                handler = DataConverter < T > .CollectGetValueKeyHandler[key];
            }
            else
            {
                handler = DataTableEntityBuilder<T>.CreateBuilderOfCollectGetValueKey(onlyProperty);
                DataConverter<T> .CollectGetValueKeyHandler.TryAdd(key, handler);
            }            
            return handler(sourceObj);
        }

        public static bool ObjComparePropertiesExt(object obj1, object obj2)
        {
            bool s = ObjCompareProperties(obj1, obj2);
            return s;
        }
        public static bool ObjCompareProperties<T>(T obj1, T obj2)
        {
            //为空判断
            if (obj1 == null && obj2 == null)
                return true;
            else if (obj1 == null || obj2 == null)
                return false;

            Type t1 = obj1.GetType();
            Type t2 = obj2.GetType();
            if (t1 != t2) return false;

            string key = "ObjCompareProperties_" + t1.FullName;
            Func<object, object, bool> handler = null;
            if (CompareEntityHandler.ContainsKey(key))
            {
                handler = CompareEntityHandler[key];
            }
            else
            {
                lock (lockCompareEntityHandler)
                {
                    if (CompareEntityHandler.ContainsKey(key))
                    {
                        handler = CompareEntityHandler[key];
                    }
                    else
                    {
                        DataTableEntityBuilder<T> eblist = DataTableEntityBuilder<T>.CreateBuilderOfCompareEntity(obj1);
                        handler = eblist.CompareEntityHandler;
                        CompareEntityHandler.Add(key, handler);
                    }
                }
            }

            return (bool)handler(obj1, obj2);
        }

        public static List<T> ToList<T>(IDataReader dataReader, bool closeDataReader = true) where T : class, new()
        {
            List<T> list = new List<T>();
            if (dataReader == null)
                return list;
            Func<IDataRecord, List<string>, object> handler = null;
            string key = "DataReader_" + typeof(T).Name;

            if (DataRecordHandler.ContainsKey(key))
            {
                handler = (Func<IDataRecord, List<string>, object>)DataRecordHandler[key];
            }
            else
            {
                lock (lockDataRecordHandler)
                {
                    if (DataRecordHandler.ContainsKey(key))
                    {
                        handler = (Func<IDataRecord, List<string>, object>)DataRecordHandler[key];
                    }
                    else
                    {
                        DataTableEntityBuilder<T> eblist = DataTableEntityBuilder<T>.CreateBuilderOfDataRecord();
                        handler = eblist.DataRecordHandler;
                        DataRecordHandler.Add(key, handler);
                    }
                }

            }
            List<string> cols = new List<string>();
            for (int i = 0; i < dataReader.FieldCount; i++)
            {
                cols.Add(dataReader.GetName(i).ToLower());
            }
            try
            {
                while (dataReader.Read())
                {
                    list.Add((T)handler(dataReader, cols));
                }
            }
            finally
            {
                if (closeDataReader)
                {
                    dataReader.Close();
                    dataReader.Dispose();
                    dataReader = null;
                }
            }

            return list;
        }

        private static List<Hi_FieldModel> ToList2(IDataReader dataReader, bool closeDataReader = true)
        {
            List<Hi_FieldModel> list = new List<Hi_FieldModel>();
            if (dataReader == null)
                return list;
            Func<IDataRecord, List<string>, object> handler = null;
            string key = "DataReader_" + typeof(Hi_FieldModel).Name;

            HiColumn hiColumn = new HiColumn();

            List<string> cols = new List<string>();
            dataReader.Read();
            for (int i = 0; i < dataReader.FieldCount; i++)
            {
                cols.Add(dataReader.GetName(i).ToLower());

                var type = dataReader.GetFieldType(i);
                Console.WriteLine(type.Name);
                var a = dataReader.GetValue(i, new HiModelFeildsInfo() { DataType = type.Name, FieldName = dataReader.GetName(i).ToLower() });
            }
            try
            {
                while (dataReader.Read())
                {
                    //list.Add(DynamicCreateEntity2(dataReader, cols));
                }
            }
            finally
            {
                if (closeDataReader)
                {
                    dataReader.Close();
                    dataReader.Dispose();
                    dataReader = null;
                }
            }
            return list;
        }

        private static string CreateBuilderOfCollectGetValueKey(object P_0)
        {
            //IL_0015: Expected I4, but got O
            string text = "";
            text = ((Hi_FieldModel)P_0).TabName + text;
            return text;
        }
        private static string CreateBuilderOfCollectGetValueKey2(Hi_FieldModel P_0)
        {
            string text = "";
            return P_0.DbName + text;
        }

        /// <summary>
        /// 对数据进行分类汇总--有问题
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <param name="thisValue"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static List<T1> Collect<T, T1>(this List<T> thisValue, T1 t) where T : class where T1 : class, new()
        {
            var aType = typeof(T);
            var bType = typeof(T1);

           
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            List<PropertyInfo> aList = null;
            if (!typePropertyForCollect.ContainsKey(aType.FullName))
            {
                aList = aType.GetProperties().Where(it => it.PropertyType.IsIn(Constants.StringType, Constants.DateTimeOffsetType, Constants.TimeSpanType, Constants.BoolType) && it.CanRead).ToList<PropertyInfo>();
                typePropertyForCollect.TryAdd(aType.FullName, aList);
            }
            else
            {
                aList = typePropertyForCollect[aType.FullName];
            }

            List<PropertyInfo> bList = null;
            if (!typePropertyForCollect.ContainsKey(bType.FullName))
            {
                bList = bType.GetProperties().ToList<PropertyInfo>();
                typePropertyForCollect.TryAdd(bType.FullName, bList);
            }
            else
            {
                bList = typePropertyForCollect[bType.FullName];
            }

            //创建索引 增加性能
            Dictionary<string, T1> indexDic = new Dictionary<string, T1>();
            List<T1> result = new List<T1>();

            var aStrLst = aList;
            var bNumLst = bList.Where(it => it.PropertyType.IsIn(Constants.IntType, Constants.LongType, Constants.DobType, Constants.FloatType, Constants.ShortType, Constants.DecType)
            && it.CanWrite).ToList();
            var bStrLst = bList.Where(it => it.PropertyType.IsIn(Constants.StringType, Constants.DateTimeOffsetType, Constants.TimeSpanType, Constants.BoolType) && it.CanWrite).ToList();

            //sw.Stop();
            //TimeSpan ts2 = sw.Elapsed;
            //Console.WriteLine("Collect 初始 总共花费{0}ms.", ts2.TotalMilliseconds);
            //var key= string.Join("-",(string[]) bNumLst.Select(it => it.Name.ToMd5()).ToArray());
            string _keyvalue = string.Empty;
            //sw.Reset();
            //sw.Start();

            var CollectGetValueKeyWithILHandler = DataTableEntityBuilder<T>.CreateBuilderOfCollectGetValueKey(bStrLst);
            PropertyInfo aProp = null;
            foreach (var aTemp in thisValue)
            {
                _keyvalue = string.Empty;

                _keyvalue = CollectGetValueKeyWithILHandler(aTemp);
               
                //foreach (var bProp in bStrLst)
                //{
                //    aProp = aType.GetProperty(bProp.Name);
                //    _keyvalue += aProp.GetValue(aTemp, null).ToString();
                //}
                
                //_keyvalue = _keyvalue.ToMd5();
                T1 item;
                if (!indexDic.ContainsKey(_keyvalue))
                {
                    //item = (T1)Activator.CreateInstance(bType);
                    item = new T1();
                    //MoveCrossWithIL<T, T1>(aTemp, item, bStrLst);
                    foreach (var bProp in bStrLst)
                    {
                        aProp = aType.GetProperty(bProp.Name);
                        bProp.SetValue(item, aProp.GetValue(aTemp, null));
                    }
                    //MoveCrossWithIL<T, T1>(aTemp, item, bNumLst);

                    foreach (var bProp in bNumLst)
                    {
                        var btypinfo = aType.GetProperty(bProp.Name);
                        //类型要一致 目前是强校验 其实也可以做成隐式转换
                        if (btypinfo != null && btypinfo.PropertyType == bProp.PropertyType)
                        {
                            bProp.SetValue(item, btypinfo.GetValue(aTemp, null));
                        }
                        else
                        {
                            bProp.SetValue(item, 0);
                        }
                    }
                    indexDic.Add(_keyvalue, item);
                }
                else
                {
                    item = (T1)indexDic[_keyvalue];

                    //MoveCrossWithIL<T, T1>(aTemp, item, bNumLst, true);

                    foreach (var bProp in bNumLst)
                    {
                        var btypinfo = aType.GetProperty(bProp.Name);

                        //var btypinfo = _dic_a_info[bProp.Name];

                        //类型要一致 目前是强校验 其实也可以做成隐式转换
                        if (btypinfo != null && btypinfo.PropertyType == bProp.PropertyType)
                        {
                            //temp += bProp.GetValue(aTemp, null) ;
                            if (btypinfo.PropertyType == Constants.IntType)
                            {
                                int _tmp = (int)bProp.GetValue(item);
                                _tmp += (int)btypinfo.GetValue(aTemp, null);
                                bProp.SetValue(item, _tmp);
                                continue;
                            }
                            else if (btypinfo.PropertyType == Constants.LongType)
                            {
                                long _tmp = (long)bProp.GetValue(item);
                                _tmp += (long)btypinfo.GetValue(aTemp, null);
                                bProp.SetValue(item, _tmp);
                                continue;
                            }
                            else if (btypinfo.PropertyType == Constants.DobType)
                            {
                                double _tmp = (double)bProp.GetValue(item);
                                _tmp += (double)btypinfo.GetValue(aTemp, null);
                                bProp.SetValue(item, _tmp);
                                continue;
                            }
                            else if (btypinfo.PropertyType == Constants.FloatType)
                            {
                                float _tmp = (float)bProp.GetValue(item);
                                _tmp += (float)btypinfo.GetValue(aTemp, null);
                                bProp.SetValue(item, _tmp);
                                continue;
                            }
                            else if (btypinfo.PropertyType == Constants.ShortType)
                            {
                                short _tmp = (short)bProp.GetValue(item);
                                _tmp += (short)btypinfo.GetValue(aTemp, null);
                                bProp.SetValue(item, _tmp);
                                continue;
                            }
                            else if (btypinfo.PropertyType == Constants.DecType)
                            {
                                decimal _tmp = (decimal)bProp.GetValue(item);
                                _tmp += (decimal)btypinfo.GetValue(aTemp, null);
                                bProp.SetValue(item, _tmp);
                                continue;
                            }
                            else
                                bProp.SetValue(item, 0);

                        }
                        else
                        {
                            bProp.SetValue(item, 0);
                        }
                    }
                }
            }
            //sw.Stop();
            //ts2 = sw.Elapsed;
            //Console.WriteLine("Collect 转换 初始 总共花费{0}ms.", ts2.TotalMilliseconds);

            foreach (string key in indexDic.Keys)
            {
                result.Add(indexDic[key]);
            }
            return result;
        }


    }

    /// <summary>
    /// 使用EMIT
    /// </summary>
    /// <typeparam name="Entity"></typeparam>
    public class DataTableEntityBuilder<Entity>
    {
        //数据类型和对应的强制转换方法的methodinfo，供实体属性赋值时调用
        private static Dictionary<Type, MethodInfo> ConvertMethods = new Dictionary<Type, MethodInfo>()
           {
               {typeof(int),typeof(Convert).GetMethod("ToInt32",new Type[]{typeof(object)})},
               {typeof(Int16),typeof(Convert).GetMethod("ToInt16",new Type[]{typeof(object)})},
               {typeof(Int64),typeof(Convert).GetMethod("ToInt64",new Type[]{typeof(object)})},
               {typeof(DateTime),typeof(Convert).GetMethod("ToDateTime",new Type[]{typeof(object)})},
               {typeof(decimal),typeof(Convert).GetMethod("ToDecimal",new Type[]{typeof(object)})},
               {typeof(Double),typeof(Convert).GetMethod("ToDouble",new Type[]{typeof(object)})},
               {typeof(Boolean),typeof(Convert).GetMethod("ToBoolean",new Type[]{typeof(object)})},
               {typeof(string),typeof(Convert).GetMethod("ToString",new Type[]{typeof(object)})},
			   //{typeof(Nullable<int>),typeof(Convert).GetMethod("ToInt32",new Type[]{typeof(object)})},
			   //{typeof(Nullable<Int16>),typeof(Convert).GetMethod("ToInt16",new Type[]{typeof(object)})},
			   //{typeof(Nullable<Int64>),typeof(Convert).GetMethod("ToInt64",new Type[]{typeof(object)})},
			   //{typeof(Nullable<DateTime>),typeof(Convert).GetMethod("ToDateTime",new Type[]{typeof(object)})},
			   //{typeof(Nullable<decimal>),typeof(Convert).GetMethod("ToDecimal",new Type[]{typeof(object)})},
			   //{typeof(Nullable<Double>),typeof(Convert).GetMethod("ToDouble",new Type[]{typeof(object)})},
			   //{typeof(Nullable<Boolean>),typeof(Convert).GetMethod("ToBoolean",new Type[]{typeof(object)})},
			   //{typeof(string),typeof(Convert).GetMethod("ToString",new Type[]{typeof(object)})}
		   };

        #region DataRow

        private static readonly MethodInfo getValueMethod = typeof(DataRow).GetMethod("get_Item", new Type[] { typeof(string) });
        private static readonly MethodInfo isDBNullMethod = typeof(DataRow).GetMethod("IsNull", new Type[] { typeof(string) });
        private static readonly MethodInfo getTableMethod = typeof(DataRow).GetMethod("get_Table", new Type[] { });
        private static readonly MethodInfo getColumns = typeof(DataTable).GetMethod("get_Columns", new Type[] { });
        private static readonly MethodInfo Contains = typeof(DataColumnCollection).GetMethod("Contains", new Type[] { typeof(string) });
        public Func<DataRow, object> DataRowHandler { get; set; }

        #endregion

        #region IDataRecord

        private static readonly MethodInfo IndexOf = typeof(List<string>).GetMethod("IndexOf", new Type[] { typeof(string) });
        private static readonly MethodInfo getCount = typeof(List<string>).GetMethod("get_Count", new Type[] { });
        private static readonly MethodInfo IsDBNull = typeof(IDataRecord).GetMethod("IsDBNull", new Type[] { typeof(int) });
        private static readonly MethodInfo getItem = typeof(IDataRecord).GetMethod("get_Item", new Type[] { typeof(int) });

        private static readonly MethodInfo getObjectEqualsMethod = typeof(System.Object).GetMethod("Equals", new Type[] { typeof(object), typeof(object) });

        private static readonly MethodInfo getDateTimeEqualsMethod = typeof(System.DateTime).GetMethod("Equals", new Type[] { typeof(DateTime), typeof(DateTime) });


        public Func<IDataRecord, List<string>, object> DataRecordHandler { get; set; }
        public Func<object, object, bool> CompareEntityHandler { get; set; }

        public Func<object, object> CloneEntityHandler { get; set; }

        #endregion

        private DataTableEntityBuilder() { }

        /// <summary>
        /// 如果DataRow中的列与实体中的不一致，会报错
        /// </summary>
        /// <param name="dataRow"></param>
        /// <returns></returns>
        public static DataTableEntityBuilder<Entity> CreateBuilder(DataRow dataRow)
        {
            DataTableEntityBuilder<Entity> dynamicBuilder = new DataTableEntityBuilder<Entity>();
            DynamicMethod method = new DynamicMethod("DynamicCreateEntity", typeof(Entity), new Type[] { typeof(DataRow) }, typeof(Entity), true);


            ILGenerator generator = method.GetILGenerator();
            LocalBuilder result = generator.DeclareLocal(typeof(Entity));
            generator.Emit(OpCodes.Newobj, typeof(Entity).GetConstructor(Type.EmptyTypes));
            generator.Emit(OpCodes.Stloc, result);

            for (int index = 0; index < dataRow.ItemArray.Length; index++)
            {
                PropertyInfo propertyInfo = typeof(Entity).GetProperty(dataRow.Table.Columns[index].ColumnName);
                Label endIfLabel = generator.DefineLabel();
                if (propertyInfo != null && propertyInfo.GetSetMethod() != null)
                {
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldc_I4, index);
                    generator.Emit(OpCodes.Callvirt, isDBNullMethod);
                    generator.Emit(OpCodes.Brtrue, endIfLabel);
                    generator.Emit(OpCodes.Ldloc, result);
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldc_I4, index);
                    generator.Emit(OpCodes.Callvirt, getValueMethod);
                    generator.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
                    generator.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());
                    generator.MarkLabel(endIfLabel);
                }
            }
            generator.Emit(OpCodes.Ldloc, result);
            generator.Emit(OpCodes.Ret);
            dynamicBuilder.DataRowHandler = (Func<DataRow, object>)method.CreateDelegate(typeof(Func<DataRow, object>));
            return dynamicBuilder;
        }

        /// <summary>
        /// DataRow转Entity
        /// </summary>
        /// <returns></returns>
        public static DataTableEntityBuilder<Entity> CreateBuilderOfDataRow()
        {
            DataTableEntityBuilder<Entity> dynamicBuilder = new DataTableEntityBuilder<Entity>();
            DynamicMethod method = new DynamicMethod("DataRow2Entity", typeof(Entity), new Type[] { typeof(DataRow) }, typeof(Entity), true);
            ILGenerator generator = method.GetILGenerator();
            // 返回值(如Role对象)
            // T result;
            LocalBuilder result = generator.DeclareLocal(typeof(Entity));
            // result = new T();
            generator.Emit(OpCodes.Newobj, typeof(Entity).GetConstructor(Type.EmptyTypes));
            generator.Emit(OpCodes.Stloc, result);

            foreach (PropertyInfo property in typeof(Entity).GetProperties())
            {
                // if (dr.Table.Columns.Contains(property.Name))
                Label endIfContains = generator.DefineLabel();
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Callvirt, getTableMethod);
                generator.Emit(OpCodes.Callvirt, getColumns);
                generator.Emit(OpCodes.Ldstr, property.Name);
                generator.Emit(OpCodes.Callvirt, Contains);
                // 为false，则到generator.MarkLabel(endIfContains);中的代码不执行
                generator.Emit(OpCodes.Brfalse, endIfContains);

                // if (!dr.IsNull(property.Name))
                Label endIfDBNull = generator.DefineLabel();
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldstr, property.Name);
                generator.Emit(OpCodes.Callvirt, isDBNullMethod);
                // 如果dr.IsNull(property.Name)为true,则到generator.MarkLabel(endIfDBNull)间的代码不执行
                generator.Emit(OpCodes.Brtrue, endIfDBNull);

                //generator.Emit(OpCodes.Ldstr, "I'm test");
                //generator.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[] { typeof(String) }));

                generator.Emit(OpCodes.Ldloc, result);
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldstr, property.Name);
                generator.Emit(OpCodes.Callvirt, getValueMethod);

                bool bIsNullable = false;
                Type columnType = property.PropertyType;
                if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    bIsNullable = true;
                    // If it is NULLABLE, then get the underlying type. eg if "Nullable<int>" then this will return just "int"
                    columnType = property.PropertyType.GetGenericArguments()[0];
                }

                var castMethod = typeof(Convert).GetMethod("To" + columnType.Name, new Type[] { typeof(object) });
                if (columnType == typeof(DateTime))
                {
                    //generator.Emit(OpCodes.Unbox_Any, columnType);
                    //generator.Emit(OpCodes.Callvirt, property.GetSetMethod());
                    if (bIsNullable)
                    {
                        generator.Emit(OpCodes.Call, castMethod);
                        generator.Emit(OpCodes.Newobj, property.PropertyType.GetConstructor(new[] { columnType }));
                    }
                    else
                    {
                        generator.Emit(OpCodes.Unbox_Any, columnType);
                    }
                }
                else
                {
                    // 使用下面的方法时，日期类型会出错
                    if ((columnType.IsValueType ||
                        columnType == typeof(string))
                        && ConvertMethods.ContainsKey(columnType))
                    {
                        //generator.Emit(OpCodes.Call, ConvertMethods[columnType]);
                        generator.Emit(OpCodes.Call, castMethod);
                        if (bIsNullable)
                        {
                            generator.Emit(OpCodes.Newobj, property.PropertyType.GetConstructor(new[] { columnType }));
                        }
                    }
                    else
                        generator.Emit(OpCodes.Castclass, columnType);
                    //generator.Emit(OpCodes.Callvirt, property.GetSetMethod());
                }
                generator.Emit(OpCodes.Callvirt, property.GetSetMethod());

                generator.MarkLabel(endIfDBNull);
                generator.MarkLabel(endIfContains);
            }
            generator.Emit(OpCodes.Ldloc, result);
            generator.Emit(OpCodes.Ret);

            dynamicBuilder.DataRowHandler = (Func<DataRow, object>)method.CreateDelegate(typeof(Func<DataRow, object>));
            return dynamicBuilder;
        }



        /// <summary>
        /// 克隆 对象，支持 属性是 引用类型
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static DataTableEntityBuilder<Entity> CreateBuilderOfCloneObject(Entity entity)
        {
            DataTableEntityBuilder<Entity> dynamicBuilder = new DataTableEntityBuilder<Entity>();
            DynamicMethod dymMethod = new DynamicMethod("CloneObjectWithIL", typeof(object), new Type[] { typeof(object) }, typeof(Entity), true);
            Type type = entity.GetType();
            ConstructorInfo cInfo = type.GetConstructor(new Type[] { });

            ILGenerator generator = dymMethod.GetILGenerator();
            LocalBuilder lbf = generator.DeclareLocal(type);

            generator.Emit(OpCodes.Newobj, cInfo);
            generator.Emit(OpCodes.Stloc_0);
            var pros = type.GetProperties().Where(p => p.CanWrite && p.CanRead);
            foreach (var temp in pros)
            {
                if (temp != null && temp.GetMethod != null)  //判断是否 可以 get
                {
                    if (temp.PropertyType.IsValueType || temp.PropertyType == typeof(string))
                    {
                        generator.Emit(OpCodes.Ldloc_0);
                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Callvirt, temp.GetMethod);
                        generator.Emit(OpCodes.Callvirt, temp.SetMethod);
                        // generator.Emit(OpCodes.Ldstr, " field:" + field.Name);
                        //generator.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }));
                    }
                    else {
                        generator.Emit(OpCodes.Ldloc_0);
                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Callvirt, temp.GetMethod);
                        generator.Emit(OpCodes.Call, typeof(DataConverter).GetMethod("CloneObjectWithILExt")); //stringMethod
                        generator.Emit(OpCodes.Callvirt, temp.SetMethod);
                    }
                }
            }
            var fields = type.GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).Where(t=> !pros.Any(b=>b.Name == t.Name));
            foreach (var temp in fields)
            {
                if (temp.Name.Contains("k__BackingField"))
                    continue;
                if (temp.IsStatic && temp.IsLiteral) //常量
                    continue;
                if (temp.FieldType.IsValueType || temp.FieldType == typeof(string))
                {
                    generator.Emit(OpCodes.Ldloc_0);
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldfld, temp);
                    generator.Emit(OpCodes.Stfld, temp);
                }
                else
                {
                    generator.Emit(OpCodes.Ldloc_0);
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldfld, temp);
                    generator.Emit(OpCodes.Call, typeof(DataConverter).GetMethod("CloneObjectWithILExt"));
                    generator.Emit(OpCodes.Stfld, temp);
                }
            }
            generator.Emit(OpCodes.Ldloc_0);
            generator.Emit(OpCodes.Ret);
            //var delegatea = dymMethod.CreateDelegate(typeof(Func<object, object>));
            //return delegatea;

            var delegatea = dymMethod.CreateDelegate(typeof(Func<object, object>));
            dynamicBuilder.CloneEntityHandler = (Func<object, object>)delegatea;
            return dynamicBuilder;
        }


        public static Func<object, object, bool> CreateBuilderOfCollectSum<T, T1>(T sourceData, T1 targert)
        {
            DataTableEntityBuilder<Entity> dynamicBuilder = new DataTableEntityBuilder<Entity>();
            // T result;
            DynamicMethod dymMethod = new DynamicMethod("CreateBuilderOfCollectSumWithIL", typeof(List<T1>), new Type[] { typeof(List<T>), typeof(T1) }, typeof(T), true);
            Type typeSource = sourceData.GetType();
            Type typeTargert = targert.GetType();
            List<PropertyInfo> aList = typeSource.GetProperties().ToList<PropertyInfo>();
            List<PropertyInfo> bList = typeTargert.GetProperties().ToList<PropertyInfo>();
            var pros = aList.Where(it => it.CanRead).Where(t => bList.Any(b => b.Name == t.Name && b.CanWrite && t.PropertyType == b.PropertyType)).ToList();
            ILGenerator generator = dymMethod.GetILGenerator();
            LocalBuilder result = generator.DeclareLocal(typeof(bool));
            generator.Emit(OpCodes.Ldc_I4_1);
            generator.Emit(OpCodes.Stloc_0);

            // T result;
            LocalBuilder resultT = generator.DeclareLocal(typeof(T1));
            LocalBuilder j = generator.DeclareLocal(typeof(int));
            // result = new T();
            generator.Emit(OpCodes.Newobj, typeof(Entity).GetConstructor(Type.EmptyTypes));
            generator.Emit(OpCodes.Stloc, result);

            foreach (var temp in pros)
            {
                // bProp.SetValue(t, prop.GetValue(thisValue, null), null);
                //if(temp.Name == "FieldName")
                if (temp.PropertyType.IsValueType || temp.PropertyType == typeof(string))
                {
                    generator.Emit(OpCodes.Ldarg_1);
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Callvirt, temp.GetMethod);
                    generator.Emit(OpCodes.Callvirt, temp.SetMethod);

                    //generator.Emit(OpCodes.Ldarg_0);
                    //generator.Emit(OpCodes.Ldarg_1);
                    //generator.Emit(OpCodes.Callvirt, temp.GetMethod);
                    //generator.Emit(OpCodes.Callvirt, temp.SetMethod);
                    // generator.Emit(OpCodes.Ldstr, " field:" + field.Name);
                    //generator.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }));
                }
                else
                {
                    generator.Emit(OpCodes.Ldarg_1);
                    generator.Emit(OpCodes.Ldarg_1);
                    generator.Emit(OpCodes.Callvirt, temp.GetMethod);
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Callvirt, temp.GetMethod);
                    generator.Emit(OpCodes.Call, typeof(DataConverter).GetMethod("MoveCrossWithILExt")); //stringMethod
                    generator.Emit(OpCodes.Callvirt, temp.SetMethod);
                }
            }

            generator.Emit(OpCodes.Ldloc_0);
            generator.Emit(OpCodes.Ret);
            //var delegatea = dymMethod.CreateDelegate(typeof(Func<object, object>));
            //return delegatea;

            var delegatea = dymMethod.CreateDelegate(typeof(Func<object, object, bool>));
            return (Func<object, object, bool>)delegatea;
        }


        public static Func<object, object, bool> CreateBuilderOfMoveCross(Type typeSource, Type typeTargert, List<PropertyInfo> onlyProperty = null, bool isSum = false)
        {
            DataTableEntityBuilder<Entity> dynamicBuilder = new DataTableEntityBuilder<Entity>();
            DynamicMethod dymMethod = new DynamicMethod("CreateBuilderOfMoveCrossWithIL", typeof(bool), new Type[] { typeof(object), typeof(object) }, typeof(Entity), true);
            //Type typeSource = entitySource.GetType();
            //Type typeTargert = entityTargert.GetType();
            List<PropertyInfo> aList = typeSource.GetProperties().ToList<PropertyInfo>();
            List<PropertyInfo> bList = typeTargert.GetProperties().ToList<PropertyInfo>();
            var pros = aList.Where(it => it.CanRead).Where(t=> bList.Any(b=>b.Name == t.Name && b.CanWrite && t.PropertyType == b.PropertyType)).ToList();
            ILGenerator generator = dymMethod.GetILGenerator();
            LocalBuilder result = generator.DeclareLocal(typeof(bool));
            generator.Emit(OpCodes.Ldc_I4_1);
            generator.Emit(OpCodes.Stloc_0);
            if (onlyProperty != null)
            {
                pros = pros.Where(t=> onlyProperty.Any(b=>b.Name == t.Name)).ToList();
            }
            foreach (var temp in pros)
            {
               
                // bProp.SetValue(t, prop.GetValue(thisValue, null), null);
                //if(temp.Name == "FieldName")
                if (temp.PropertyType.IsValueType || temp.PropertyType == typeof(string))
                {
                    if (isSum && temp.PropertyType.IsValueType)
                    {
                        generator.Emit(OpCodes.Ldarg_1);
                        generator.Emit(OpCodes.Callvirt, temp.GetMethod);
                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Callvirt, temp.GetMethod);
                        generator.Emit(OpCodes.Add);
                        generator.Emit(OpCodes.Callvirt, temp.SetMethod);

                    }
                    else
                    {
                        generator.Emit(OpCodes.Ldarg_1);
                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Callvirt, temp.GetMethod);
                        generator.Emit(OpCodes.Callvirt, temp.SetMethod);
                    }
                   

                    //generator.Emit(OpCodes.Ldarg_0);
                    //generator.Emit(OpCodes.Ldarg_1);
                    //generator.Emit(OpCodes.Callvirt, temp.GetMethod);
                    //generator.Emit(OpCodes.Callvirt, temp.SetMethod);
                    // generator.Emit(OpCodes.Ldstr, " field:" + field.Name);
                    //generator.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }));
                }
                else
                {
                    if (isSum) //引用类型不需要汇总
                    {
                       
                    }
                    else
                    {
                        generator.Emit(OpCodes.Ldarg_1);
                        generator.Emit(OpCodes.Ldarg_1);
                        generator.Emit(OpCodes.Callvirt, temp.GetMethod);
                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Callvirt, temp.GetMethod);
                        generator.Emit(OpCodes.Call, typeof(DataConverter).GetMethod("MoveCrossWithILExt")); //stringMethod
                        generator.Emit(OpCodes.Callvirt, temp.SetMethod);
                    }
                }
            }
            
            generator.Emit(OpCodes.Ldloc_0);
            generator.Emit(OpCodes.Ret);
            //var delegatea = dymMethod.CreateDelegate(typeof(Func<object, object>));
            //return delegatea;

            var delegatea = dymMethod.CreateDelegate(typeof(Func<object, object, bool>));
            return  (Func<object, object,bool>)delegatea;
        }

       


        public static Func<Entity, string> CreateBuilderOfCollectGetValueKey(List<PropertyInfo> property = null)
        {
            DataTableEntityBuilder<Entity> dynamicBuilder = new DataTableEntityBuilder<Entity>();
            DynamicMethod dymMethod = new DynamicMethod("CreateBuilderOfCollectGetValueKey", typeof(string), new Type[] { typeof(Entity), }, typeof(Entity), true);
            ILGenerator generator = dymMethod.GetILGenerator();
            LocalBuilder result = generator.DeclareLocal(typeof(string));
            generator.Emit(OpCodes.Ldstr,"");
            generator.Emit(OpCodes.Stloc_0);
            
            foreach (var temp in property)
            {
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Callvirt, temp.GetMethod);
                generator.Emit(OpCodes.Ldloc_0);
                generator.Emit(OpCodes.Call, typeof(string).GetMethod("Concat", new Type[] { typeof(string), typeof(string) }));
                generator.Emit(OpCodes.Stloc_0);
            }

            generator.Emit(OpCodes.Ldloc_0);
            generator.Emit(OpCodes.Ret);
            var delegatea = dymMethod.CreateDelegate(typeof(Func<Entity, string>));
            return (Func<Entity, string>)delegatea;
        }

        /// <summary>
        /// 对比实体
        /// </summary>
        /// <returns></returns>
        public static DataTableEntityBuilder<Entity> CreateBuilderOfCompareEntity(Entity entity)
        {
            DataTableEntityBuilder<Entity> dynamicBuilder = new DataTableEntityBuilder<Entity>();
            DynamicMethod method = new DynamicMethod("CreateBuilderOfCompareEntity", typeof(bool), new Type[] { typeof(object), typeof(object) }, typeof(Entity), true);
            ILGenerator generator = method.GetILGenerator();

            // bool result = false;
            LocalBuilder result = generator.DeclareLocal(typeof(bool));
            generator.Emit(OpCodes.Ldc_I4_1);
            generator.Emit(OpCodes.Stloc_0);
            LocalBuilder result2 = generator.DeclareLocal(typeof(bool));
            generator.Emit(OpCodes.Ldc_I4_1);
            generator.Emit(OpCodes.Stloc_1);

            PropertyInfo[] props = null;

            props = entity.GetType().GetProperties().Where(p => p.CanWrite == true && !Constants.IsStandardField(p.Name)

                //&&
                //    //(p.Name.ToLower()!= "SortNum".ToLower() && p.Name.ToLower() != "IsSys".ToLower()) 
                //    (
                //         p.Name.ToLower().IsIn("FieldDesc".ToLower(), "IsIdentity".ToLower(), "FieldName".ToLower(),
                //        "FieldType".ToLower(), "DefaultValue".ToLower(), "FieldLen".ToLower(), "FieldDec".ToLower(), "IsNull".ToLower(), "ReFieldName".ToLower())
                //    ) 
                && p.MemberType == MemberTypes.Property).ToArray();


            var obj = new object();
            // typeof(Object).GetMethod("ObjCompareProperties", new Type[] { typeof(object), typeof(object) });

            Label endIfLabel = generator.DefineLabel();
            Label returnLabel = generator.DefineLabel();
            Type typeInt = typeof(int);

            Type datetimeType = typeof(DateTime);

            foreach (var temp in props)
            {
                if (temp != null && temp.GetMethod != null)  //判断是否 可以 get
                {
                    //  if (new List<string> { "FieldName", "PropertyInfo", "CreateName", "DataType" }.Contains(temp.Name))

                    if (temp.PropertyType.IsValueType || temp.PropertyType == typeof(string))
                    {
                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Callvirt, temp.GetMethod);
                        generator.Emit(OpCodes.Ldarg_1);
                        generator.Emit(OpCodes.Callvirt, temp.GetMethod);

                        if (datetimeType == temp.PropertyType)
                        {
                            generator.Emit(OpCodes.Call, getDateTimeEqualsMethod);
                            generator.Emit(OpCodes.Stloc_0);
                            generator.Emit(OpCodes.Ldloc_0);
                            generator.Emit(OpCodes.Brfalse, endIfLabel);
                        }
                        else if (temp.PropertyType.IsValueType)
                        {
                            generator.Emit(OpCodes.Clt);
                            generator.Emit(OpCodes.Brfalse, endIfLabel);
                        }
                        else
                        {
                            generator.Emit(OpCodes.Call, getObjectEqualsMethod);
                            generator.Emit(OpCodes.Stloc_0);
                            generator.Emit(OpCodes.Ldloc_0);
                            generator.Emit(OpCodes.Brfalse, endIfLabel);

                        }
                    }
                    else
                    {
                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Callvirt, temp.GetMethod);
                        generator.Emit(OpCodes.Ldarg_1);
                        generator.Emit(OpCodes.Callvirt, temp.GetMethod);
                        // generator.EmitCall(OpCodes.Call, stringMethod, new Type[] { temp.GetType(), temp.GetType() }); //getComparePropertiesExtMethod

                        generator.Emit(OpCodes.Call, typeof(DataConverter).GetMethod("ObjComparePropertiesExt")); //stringMethod
                        generator.Emit(OpCodes.Stloc_1);
                        generator.Emit(OpCodes.Ldloc_1);
                        generator.Emit(OpCodes.Brfalse, returnLabel);
                    }
                }
            }

            generator.MarkLabel(returnLabel);
            generator.Emit(OpCodes.Ldloc_1);
            generator.Emit(OpCodes.Ret);

            generator.MarkLabel(endIfLabel);
            generator.Emit(OpCodes.Ldloc_0);
            generator.Emit(OpCodes.Ret);

            var delegatea = method.CreateDelegate(typeof(Func<object, object, bool>));
            dynamicBuilder.CompareEntityHandler = (Func<object, object, bool>)delegatea;
            return dynamicBuilder;
        }

        /// <summary>
        /// DataReader转Entity
        /// </summary>
        /// <returns></returns>
        public static DataTableEntityBuilder<Entity> CreateBuilderOfDataRecord()
        {
            DataTableEntityBuilder<Entity> dynamicBuilder = new DataTableEntityBuilder<Entity>();
            DynamicMethod method = new DynamicMethod("DataReader2Entity", typeof(Entity), new Type[] { typeof(IDataRecord), typeof(List<string>) }, typeof(Entity), true);
            ILGenerator generator = method.GetILGenerator();
            // 返回值(如Role对象)
            // T result;
            LocalBuilder result = generator.DeclareLocal(typeof(Entity));
            LocalBuilder j = generator.DeclareLocal(typeof(int));
            // result = new T();
            generator.Emit(OpCodes.Newobj, typeof(Entity).GetConstructor(Type.EmptyTypes));
            generator.Emit(OpCodes.Stloc, result);
            foreach (PropertyInfo property in typeof(Entity).GetProperties())
            {
                //int j = cols.IndexOf(property.Name.ToLower());
                generator.Emit(OpCodes.Ldarg_1);
                generator.Emit(OpCodes.Ldstr, property.Name.ToLower());
                generator.Emit(OpCodes.Callvirt, IndexOf);
                generator.Emit(OpCodes.Stloc, j);
                // if (j >= 0)
                Label endIfBlt = generator.DefineLabel();
                generator.Emit(OpCodes.Ldloc_S, j);
                generator.Emit(OpCodes.Ldc_I4_0);
                // 为false，则到generator.MarkLabel(endIfContains); 中的代码不执行
                generator.Emit(OpCodes.Blt_S, endIfBlt);

                // if (j < cols.Count)
                Label endIfBge = generator.DefineLabel();
                generator.Emit(OpCodes.Ldloc_S, j);
                generator.Emit(OpCodes.Ldarg_1);
                generator.Emit(OpCodes.Callvirt, getCount);
                generator.Emit(OpCodes.Bge_S, endIfBge);

                // if (!dr.IsDBNull(j))
                Label endIfDBNull = generator.DefineLabel();
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldloc_S, j);
                generator.Emit(OpCodes.Callvirt, IsDBNull);
                // 如果dr.IsNull(property.Name)为true,则到generator.MarkLabel(endIfDBNull)间的代码不执行
                generator.Emit(OpCodes.Brtrue, endIfDBNull);

                //generator.Emit(OpCodes.Ldstr, "I'm test");
                //generator.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[] { typeof(String) }));

                generator.Emit(OpCodes.Ldloc, result);
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldloc_S, j);
                generator.Emit(OpCodes.Callvirt, getItem);
                generator.Emit(OpCodes.Unbox_Any, property.PropertyType);
                generator.Emit(OpCodes.Callvirt, property.GetSetMethod());

                // System.Nullable`1<int32>
                // 使用下面的方法时，日期类型会出错
                //if ((property.PropertyType.IsValueType ||
                //    property.PropertyType == typeof(string))
                //    && ConvertMethods.ContainsKey(property.PropertyType))
                //    generator.Emit(OpCodes.Call, ConvertMethods[property.PropertyType]);
                //else
                //    generator.Emit(OpCodes.Castclass, property.PropertyType);
                //generator.Emit(OpCodes.Callvirt, property.GetSetMethod());

                generator.MarkLabel(endIfDBNull);
                generator.MarkLabel(endIfBge);
                generator.MarkLabel(endIfBlt);
            }
            generator.Emit(OpCodes.Ldloc, result);
            generator.Emit(OpCodes.Ret);

            dynamicBuilder.DataRecordHandler = (Func<IDataRecord, List<string>, object>)method.CreateDelegate(typeof(Func<IDataRecord, List<string>, object>));
            return dynamicBuilder;
        }


        /// <summary>
        /// DataReader转Entity
        /// </summary>
        /// <returns></returns>
        public static DataTableEntityBuilder<TDynamic> CreateBuilderOfTDynamicDataRecord()
        {
            DataTableEntityBuilder<TDynamic> dynamicBuilder = new DataTableEntityBuilder<TDynamic>();
            DynamicMethod method = new DynamicMethod("DataReader2Entity", typeof(TDynamic), new Type[] { typeof(IDataRecord), typeof(List<string>) }, typeof(TDynamic), true);
            ILGenerator generator = method.GetILGenerator();
            // 返回值(如Role对象)
            // T result;
            LocalBuilder result = generator.DeclareLocal(typeof(TDynamic));
            LocalBuilder j = generator.DeclareLocal(typeof(int));
            // result = new T();
            generator.Emit(OpCodes.Newobj, typeof(TDynamic).GetConstructor(Type.EmptyTypes));
            generator.Emit(OpCodes.Stloc, result);
            foreach (PropertyInfo property in typeof(TDynamic).GetProperties())
            {
                //int j = cols.IndexOf(property.Name.ToLower());
                generator.Emit(OpCodes.Ldarg_1);
                generator.Emit(OpCodes.Ldstr, property.Name.ToLower());
                generator.Emit(OpCodes.Callvirt, IndexOf);
                generator.Emit(OpCodes.Stloc, j);
                // if (j >= 0)
                Label endIfBlt = generator.DefineLabel();
                generator.Emit(OpCodes.Ldloc_S, j);
                generator.Emit(OpCodes.Ldc_I4_0);
                // 为false，则到generator.MarkLabel(endIfContains); 中的代码不执行
                generator.Emit(OpCodes.Blt_S, endIfBlt);

                // if (j < cols.Count)
                Label endIfBge = generator.DefineLabel();
                generator.Emit(OpCodes.Ldloc_S, j);
                generator.Emit(OpCodes.Ldarg_1);
                generator.Emit(OpCodes.Callvirt, getCount);
                generator.Emit(OpCodes.Bge_S, endIfBge);

                // if (!dr.IsDBNull(j))
                Label endIfDBNull = generator.DefineLabel();
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldloc_S, j);
                generator.Emit(OpCodes.Callvirt, IsDBNull);
                // 如果dr.IsNull(property.Name)为true,则到generator.MarkLabel(endIfDBNull)间的代码不执行
                generator.Emit(OpCodes.Brtrue, endIfDBNull);

                //generator.Emit(OpCodes.Ldstr, "I'm test");
                //generator.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[] { typeof(String) }));

                generator.Emit(OpCodes.Ldloc, result);
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldloc_S, j);
                generator.Emit(OpCodes.Callvirt, getItem);
                generator.Emit(OpCodes.Unbox_Any, property.PropertyType);
                generator.Emit(OpCodes.Callvirt, property.GetSetMethod());

                // System.Nullable`1<int32>
                // 使用下面的方法时，日期类型会出错
                //if ((property.PropertyType.IsValueType ||
                //    property.PropertyType == typeof(string))
                //    && ConvertMethods.ContainsKey(property.PropertyType))
                //    generator.Emit(OpCodes.Call, ConvertMethods[property.PropertyType]);
                //else
                //    generator.Emit(OpCodes.Castclass, property.PropertyType);
                //generator.Emit(OpCodes.Callvirt, property.GetSetMethod());

                generator.MarkLabel(endIfDBNull);
                generator.MarkLabel(endIfBge);
                generator.MarkLabel(endIfBlt);
            }
            generator.Emit(OpCodes.Ldloc, result);
            generator.Emit(OpCodes.Ret);

            dynamicBuilder.DataRecordHandler = (Func<IDataRecord, List<string>, object>)method.CreateDelegate(typeof(Func<IDataRecord, List<string>, object>));
            return dynamicBuilder;
        }
    }

    /// <summary>
    /// 此方法速度快，但是不支持 属性是引用类型的复制
    /// </summary>
    /// <typeparam name="S"></typeparam>
    /// <typeparam name="T"></typeparam>
    public static class FastCopy<S, T>
    {
        static Action<S, T> action = CreateCopier();


        static Action<S, T> action2 = CreateCopier2();

        /// <summary>
        /// 复制两个对象同名属性值
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">源对象</param>
        /// <param name="target">目标对象</param>
        /// <param name="copyNull">源对象属性值为null时，是否将值复制给目标对象</param>
        public static void Copy(S source, T target, bool copyNull = true)
        {
            //action(source, target);
            action2(source, target);
        }
        /// <summary>
        /// 为指定的两种类型编译生成属性复制委托
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="copyNull">源对象属性值为null时，是否将值复制给目标对象</param>
        /// <returns></returns>
        private static Action<S, T> CreateCopier2()
        {
            ParameterExpression source = Expression.Parameter(typeof(S));
            ParameterExpression target = Expression.Parameter(typeof(T));
            var sourceProps = typeof(S).GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.CanRead).ToList();
            var targetProps = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.CanWrite).ToList();

            // 查找可进行赋值的属性
            var copyProps = targetProps.Where(tProp => sourceProps.Where(sProp => sProp.Name == tProp.Name// 名称一致 且
&& (
            sProp.PropertyType == tProp.PropertyType// 属性类型一致 或
|| sProp.PropertyType.IsAssignableFrom(tProp.PropertyType) // 源属性类型 为 目标属性类型 的 子类；eg：object target = string source;   或
|| (tProp.PropertyType.IsValueType && sProp.PropertyType.IsValueType && // 属性为值类型且基础类型一致，但目标属性为可空类型 eg：int? num = int num;
((tProp.PropertyType.GenericTypeArguments.Length > 0 ? tProp.PropertyType.GenericTypeArguments[0] : tProp.PropertyType) == sProp.PropertyType))
)).Count() > 0);

            List<Expression> expressionList = new List<Expression>();
            foreach (var prop in copyProps)
            {
                if (prop.PropertyType.IsValueType)// 属性为值类型
                {
                    PropertyInfo sProp = typeof(S).GetProperty(prop.Name);
                    PropertyInfo tProp = typeof(T).GetProperty(prop.Name);
                    if (sProp.PropertyType == tProp.PropertyType)// 属性类型一致 eg：int num = int num;    或   int? num = int? num;
                    {
                        var assign = Expression.Assign(Expression.Property(target, prop.Name), Expression.Property(source, prop.Name));
                        expressionList.Add(assign);
                    }
                    else if (sProp.PropertyType.GenericTypeArguments.Length <= 0 && tProp.PropertyType.GenericTypeArguments.Length > 0)// 属性类型不一致且目标属性类型为可空类型 eg：int? num = int num;
                    {
                        var convert = Expression.Convert(Expression.Property(source, prop.Name), tProp.PropertyType);
                        var cvAssign = Expression.Assign(Expression.Property(target, prop.Name), convert);
                        expressionList.Add(cvAssign);
                    }
                }
                else// 属性为引用类型
                {
                    var assign = Expression.Assign(Expression.Property(target, prop.Name), Expression.Property(source, prop.Name));// 编译生成属性赋值语句   target.{PropertyName} = source.{PropertyName};
                    var sourcePropIsNull = Expression.Equal(Expression.Constant(null, prop.PropertyType), Expression.Property(source, prop.Name));// 判断源属性值是否为Null；编译生成  source.{PropertyName} == null
                    var setNull = Expression.IsTrue(Expression.Constant(false));// 判断是否复制Null值 编译生成  copyNull == True
                    var setNullTest = Expression.IfThen(setNull, assign);
                    var condition = Expression.IfThenElse(sourcePropIsNull, setNullTest, assign);

                    /**
       * 编译生成
       * if(source.{PropertyName} == null)
       * {
       *   if(setNull)
       *   {
       *     target.{PropertyName} = source.{PropertyName};
       *   }
       * }
       * else
       * {
       *   target.{PropertyName} = source.{PropertyName};
       * }
       */
                    expressionList.Add(condition);
                }
            }
            var block = Expression.Block(expressionList.ToArray());
            Expression<Action<S, T>> lambda = Expression.Lambda<Action<S, T>>(block, source, target);
            return lambda.Compile();
        }

        /// <summary>
        /// 为指定的两种类型编译生成属性复制委托
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="copyNull">源对象属性值为null时，是否将值复制给目标对象</param>
        /// <returns></returns>
        private static Action<S, T> CreateCopier()
        {
            ParameterExpression source = Expression.Parameter(typeof(S));
            ParameterExpression target = Expression.Parameter(typeof(T));
            var sourceProps = typeof(S).GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.CanRead).ToList();
            var targetProps = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.CanWrite).ToList();

            // 查找可进行赋值的属性
            var copyProps = targetProps.Where(tProp => sourceProps.Where(sProp => sProp.Name == tProp.Name// 名称一致 且
&& (
            sProp.PropertyType == tProp.PropertyType// 性能检测一致
)).Count() > 0);

            var block = Expression.Block(from p in copyProps select Expression.Assign(Expression.Property(target, p.Name), Expression.Property(source, p.Name)));
            Expression<Action<S, T>> lambda = Expression.Lambda<Action<S, T>>(block, source, target);
            return lambda.Compile();
        }
    }

}
