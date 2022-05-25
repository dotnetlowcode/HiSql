using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace HiSql
{
    internal static class CacheContext
    {
        public static ThreadLocal<List<HiSqlProvider>> ContextList = new ThreadLocal<List<HiSqlProvider>>();


        static ICache _cache = null;

        /// <summary>
        /// 提供外部访问缓存
        /// </summary>
        public static ICache Cache
        {
            get => MCache;
        }

        public static ICache MCache
        {
            get
            {

                if (_cache == null)
                {
                    lock (ContextList)
                    {
                        if (_cache == null)
                        {
                            if (!Global.RedisOn)
                                _cache = new MCache(HiSql.Constants.NameSpace);
                            else
                                _cache = new RCache(Global.RedisOptions);
                        }
                    }
                }

                return _cache; 
            }
        }
    }
}
