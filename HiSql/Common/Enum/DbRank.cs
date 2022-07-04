using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public enum DbRank
    {
        NONE=-1,
        /// <summary>
        /// 不跳号排名
        /// </summary>
        DENSERANK = 1,//

        /// <summary>
        /// 跳号排名
        /// </summary>
        RANK=2,

        /// <summary>
        /// 按记录行号排名
        /// </summary>
        ROWNUMBER=4,


    }
}
