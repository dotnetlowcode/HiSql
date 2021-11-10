using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql 
{
    public enum DbFunction
    {
        NONE=-1,//无定义
        COUNT=1,//求记录数Count(1*)
        SUM=2,//求和
        AVG=3,//求平均
        MAX=4,//求最大值
        MIN=5//求最小值
    }
}
