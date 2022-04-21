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

        public static ICache MCache
        {
            get {

                if (_cache == null)
                {
                    if (!Global.RedisOn)
                        _cache = new MCache(null);
                    else
                        _cache = new RCache(Global.RedisOptions);
                }
                    
                return _cache; ;

            }
        }

    }
}
