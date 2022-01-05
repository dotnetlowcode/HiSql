using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// 表状态
    /// </summary>
    public enum TabStatus
    {
        Use=1,//已经激活 正常使用
        Expired=-2,//暂时可以正常使用,但已经过期 下一个版本可能废弃
        Abandon=-1,//已经废弃
        Archived=11,//表示已经归档 仅用于备用查询
        NoActive=0,//表还在维护创建中

    }
}
