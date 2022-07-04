using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{

    /// <summary>
    /// 表的缓存类型
    /// </summary>
    public enum TabCacheType
    {
        ALL=1,//全量缓存(数据有变化时也会自动缓存)
        AllOnlyStart=11,//全量缓存(仅在启动时缓存)

        QUERY=2,//查询缓存(按查询条件缓存)


        None=0,//不缓存
    }
}
