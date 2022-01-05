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
        public static ThreadLocal<List<HiSqlProvider>> ContextList = new ThreadLocal<List<HiSqlProvider>>();

        public static ICache MCache = new MCache(null);

    }
}
