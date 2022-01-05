using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// sql join语法
    /// </summary>
    public enum JoinType
    {
        Inner=0,//标准连接
        Left=1,
        Right=2
    }
}
