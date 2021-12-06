using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// 过滤条件类型 
    /// </summary>
    public enum FilterType
    {
        /// <summary>
        /// 过滤条件
        /// </summary>
        CONDITION=0,

        /// <summary>
        /// 左括号
        /// </summary>
        BRACKET_LEFT=1,

        /// <summary>
        /// 右括号
        /// </summary>
        BRACKET_RIGHT=2,

        /// <summary>
        /// 逻辑类型
        /// </summary>
        LOGI=3,
    }
}
