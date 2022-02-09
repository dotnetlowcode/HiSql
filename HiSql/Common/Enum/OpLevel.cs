using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// 操作级别控制
    /// </summary>
    public enum OpLevel
    {
        /// <summary>
        /// 表是进行检测是否会执行成功，但不真正执行
        /// </summary>
        Check=0,
        /// <summary>
        /// 检测并执行
        /// </summary>
        Execute=1
    }
}
