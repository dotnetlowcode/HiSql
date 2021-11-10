using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// 表类型
    /// </summary>
    public enum TabType
    {
        Business=0,//业务表
        Transcation=1,//事务表
        Config=2,//配置业务表
        Record=3,//仅是记录表 不重要


        View=11,//视图

        Struct=21,//结构


    }
}
