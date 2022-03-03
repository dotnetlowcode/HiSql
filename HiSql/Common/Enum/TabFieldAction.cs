using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql 
{
    //表变化动作
    public enum TabFieldAction
    {
        NONE=0,//无变更
        ADD = 1,//新增字段
        MODI=2,//修改字段
        DELETE=3,//删除字段

        RENAME=4,//重命名
    }
}
