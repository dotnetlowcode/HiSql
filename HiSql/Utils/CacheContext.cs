using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace HiSql
{
    public static class CacheContext
    {
        internal static ThreadLocal<List<HiSqlProvider>> ContextList = new ThreadLocal<List<HiSqlProvider>>();


        static ICache _cache = null;

        static ICache _localcache = null;

        /// <summary>
        /// 提供外部访问缓存
        /// </summary>
        internal static ICache Cache
        {
            get => MCache;
        }
        internal static void Reset()
        {
            if (_cache != null)
            {
                _cache.Clear();
                _cache = null;
            }
        }
        /// <summary>
        /// 本地全局基于内存缓存
        /// </summary>
        public static ICache LocalMCahe
        {
            get {
                if (_localcache == null)
                {
                    lock (ContextList)
                    {
                        _localcache= new MCache(HiSql.Constants.NameSpace);
                    }
                }
                return _localcache;
            }
        }

        internal static ICache MCache
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
