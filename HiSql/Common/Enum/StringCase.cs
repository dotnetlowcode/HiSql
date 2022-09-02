using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// 数据库字符转换类型
    /// </summary>
    public enum StringCase
    {
        /// <summary>
        /// 默认使用数据库的规则
        /// </summary>
        Default=0,
        /// <summary>
        /// 转成大写
        /// </summary>
        UpperCase=1,

        /// <summary>
        /// 转成小写
        /// </summary>
        LowerCase=2,

    }
}
