using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql 
{
    /// <summary>
    /// 逻辑运算类型
    /// </summary>
    public enum LogiType
    {
        /// <summary>
        /// 且 左右条件都要满足
        /// </summary>
        AND = 0,//且
        /// <summary>
        /// 或 左右条件有一个满足即可
        /// </summary>
        OR = 1,//或
    }
}
