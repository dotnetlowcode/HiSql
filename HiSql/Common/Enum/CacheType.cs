using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// 缓存类型
    /// </summary>
    public  enum CacheType
    {

      
        /// <summary>
        /// menmory cache
        /// </summary>
        MCache=1,

        /// <summary>
        /// Redis cache
        /// </summary>
        RCache=2

    }
}
