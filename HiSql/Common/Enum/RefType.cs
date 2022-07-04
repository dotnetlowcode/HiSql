using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    
    /// <summary>
    /// 引用字段类型
    /// add by tgm date:2022.6.6
    /// </summary>
    public enum RefFieldType
    {
        /// <summary>
        /// 无引用
        /// </summary>
        None=0,


        /// <summary>
        /// 参考表字段
        /// </summary>
        Field=1,


        /// <summary>
        /// 参考字段域
        /// </summary>
        Domain=2,
    }
}
