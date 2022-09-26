using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// 日志记录类型
    /// </summary>
    public enum RecordType
    {
        ALL=0,
    }

    /// <summary>
    /// 表记录动作类型
    /// </summary>
    public enum RecordActionType
    { 
        /// <summary>
        /// 纯数据插入
        /// </summary>
        Insert=0,
        /// <summary>
        /// 更新
        /// </summary>
        Update=1,
        /// <summary>
        /// 删除
        /// </summary>
        Delete=2,
        /// <summary>
        /// 插入或更新
        /// </summary>
        Modi=3,
    }
}
