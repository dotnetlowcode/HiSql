using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// 实例化具体的数据库连接
    /// </summary>
    public static class Instance
    {

        
        static Dictionary<string, Assembly> DbAssembly = new Dictionary<string, Assembly>();



        public static Assembly GetHiql()
        {
            if (DbAssembly.ContainsKey(Constants.NameSpace))
            {
                return DbAssembly[Constants.NameSpace];
            }
            else
            {
                lock (DbAssembly)
                {
                    if (DbAssembly.ContainsKey(Constants.NameSpace))
                    {
                        return DbAssembly[Constants.NameSpace];
                    }
                    else
                    {
                        Assembly _assembly = null;
                        try
                        {
                            _assembly = Assembly.Load($"{Constants.NameSpace}");
                            //_assembly = Assembly.Load($"{Constants.NameSpace}");
                            DbAssembly.Add(Constants.NameSpace, _assembly);
                            return _assembly;
                        }
                        catch (Exception E)
                        {
                            throw new Exception($"HiSql执行异常:请检查是否有引用[{Constants.NameSpace}.dll包!");

                        }
                    }
                }
            }
        }
        public static Assembly GetAssembly(string dbtype)
        {
            if (DbAssembly.ContainsKey(dbtype.ToString()))
            {
                return DbAssembly[dbtype.ToString()];
            }
            else
            {
                lock (DbAssembly)
                {
                    if (DbAssembly.ContainsKey(dbtype.ToString()))
                    {
                        return DbAssembly[dbtype.ToString()];
                    }
                    else
                    {
                        Assembly _assembly = null;
                        try
                        {
                            _assembly = Assembly.Load($"{Constants.NameSpace}.{dbtype.ToString()}");
                            //_assembly = Assembly.Load($"{Constants.NameSpace}");
                            DbAssembly.Add(dbtype.ToString(), _assembly);
                            return _assembly;
                        }
                        catch (Exception E)
                        {

                            throw new Exception($"HiSql执行异常:请检查是否有引用[{Constants.NameSpace}.{dbtype.ToString()}].dll包!");

                        }
                    }
                }
            }
        }


        static Dictionary<string, Type> typeCache = new Dictionary<string, Type>();
        public static bool NoCache =false;
        public static IDataBase GetDBO(ConnectionConfig connectionConfig)
        {
            IDataBase _obj = Instance.CreateInstance<IDataBase>($"{Constants.NameSpace}.{connectionConfig.DbType.ToString()}{DbInterFace.Provider.ToString()}");
            if (_obj == null)
                throw new Exception($"数据库类型[{connectionConfig.DbType.ToString()}] 不支持");
            return _obj;
            
        }


        public static IUpdate GetUpdate(ConnectionConfig currentConfig)
        {
            IUpdate _obj = Instance.CreateInstance<IUpdate>($"{Constants.NameSpace}.{currentConfig.DbType.ToString()}{DbInterFace.Update.ToString()}");
            if (_obj == null)
                throw new Exception($"数据库类型[{currentConfig.DbType.ToString()}] 不支持更新业务");
            return _obj;
        }

        public static IDelete GetDelete(ConnectionConfig currentConfig)
        {
            IDelete _obj = Instance.CreateInstance<IDelete>($"{Constants.NameSpace}.{currentConfig.DbType.ToString()}{DbInterFace.Delete.ToString()}");
            if (_obj == null)
                throw new Exception($"数据库类型[{currentConfig.DbType.ToString()}] 不支持删除业务");
            return _obj;
        }

        public static IQuery GetQuery(ConnectionConfig currentConfig)
        {
            IQuery _obj = Instance.CreateInstance<IQuery>($"{Constants.NameSpace}.{currentConfig.DbType.ToString()}{DbInterFace.Query.ToString()}");
            if(_obj == null)
                throw new Exception($"数据库类型[{currentConfig.DbType.ToString()}] 不支持查询业务");
            return _obj;
        }

        public static IInsert GetInsert(ConnectionConfig currentConfig)
        {
            IInsert _obj = Instance.CreateInstance<IInsert>($"{Constants.NameSpace}.{currentConfig.DbType.ToString()}{DbInterFace.Insert.ToString()}");
            if (_obj == null)
                throw new Exception($"数据库类型[{currentConfig.DbType.ToString()}] 不支持数据写入业务");
            return _obj;
            
        }

        #region 类缓存
        public static T CreateInstance<T>(string className)
        {
            try
            {
                if (NoCache)
                {
                    return NoCacheGetCacheInstance<T>(className);
                }
                else
                {
                    return GetCacheInstance<T>(className);
                }
            }
            catch
            {
                return NoCacheGetCacheInstance<T>(className);
            }
        }

        private static string getDbTypeName(string className)
        {
            if (className.IndexOf($"{Constants.NameSpace}.") == 0)
            {
                string[] _dbtype = className.Split('.');
                string _subclassName = _dbtype[_dbtype.Length - 1];
                if (_subclassName.IndexOf(DBType.SqlServer.ToString()) == 0)
                {
                    return DBType.SqlServer.ToString();
                }
                else if (_subclassName.IndexOf(DBType.Hana.ToString()) == 0)
                {
                    return DBType.Hana.ToString();
                }
                else if (_subclassName.IndexOf(DBType.MySql.ToString()) == 0)
                {
                    return DBType.MySql.ToString();
                }
                else if (_subclassName.IndexOf(DBType.Oracle.ToString()) == 0)
                {
                    return DBType.Oracle.ToString();
                }
                else if (_subclassName.IndexOf(DBType.PostGreSql.ToString()) == 0)
                {
                    return DBType.PostGreSql.ToString();
                }
                else if (_subclassName.IndexOf(DBType.Sqlite.ToString()) == 0)
                {
                    return DBType.Sqlite.ToString();
                }
                else if (_subclassName.IndexOf(DBType.DaMeng.ToString()) == 0)
                {
                    return DBType.DaMeng.ToString();
                }
                else
                {
                    throw new Exception($"数据库实现类[{className}]不符合标准");
                }
            }else
                throw new Exception($"数据库实现类[{className}]不符合标准");
        }

        private static T GetCacheInstance<T>(string className)
        {
            Type type;
            if (typeCache.ContainsKey(className))
            {
                type = typeCache[className];
            }
            else
            {
                lock (typeCache)
                {
                    if (typeCache.ContainsKey(className))
                    {
                        type = typeCache[className];
                    }
                    else
                    {
                        type = GetAssembly(getDbTypeName(className)).GetType(className);
                        if (type == null)
                            throw new Exception($"类[{className}]不存在");
                        if (!typeCache.ContainsKey(className))
                        {
                            typeCache.Add(className, type);
                        }
                    }
                }
            }
            var result = (T)Activator.CreateInstance(type, true);
            return result;
        }
        private static T NoCacheGetCacheInstance<T>(string className)
        {
            string[] _dbtype = className.Split('.');
            Type type = GetAssembly(getDbTypeName(className)).GetType(className);
            var result = (T)Activator.CreateInstance(type, true);
            return result;
        }
        #endregion
        //public static ISqlBuilder GetSqlBuilder(ConnectionConfig currentConfig)
        //{
        //    if (currentConfig.DbType == DBType.SqlServer)
        //    {
        //        return new SqlServerInsert();
        //    }
        //    else
        //    {
        //        throw new Exception($"暂不支持该数据");
        //    }
        //}
    }
}
