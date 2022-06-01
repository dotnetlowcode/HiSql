using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{

    /// <summary>
    /// 编号前置类型
    /// autor:tansar 2022.5.27
    /// </summary>
    public enum PreType
    {
        None = 0,//0 无前置
        Y = 1,//年  1  如
        YM = 2,//年月 2
        YMD = 3,//年月日 3
        YMDH = 4,//年月日时 4
        YMDHm = 5,//年月日时分 5
        YMDHms = 6,//年月日时分秒 6

        FixY = 11,//年+固定前值 11
        FixYM = 12,//年月+固定前值 12
        FixYMD = 13,//年月日+固定前值 13
        FixYMDH = 14,//年月日时+固定前值 14
        FixYMDHm = 15,//年月日时分+固定前值 15
        FixYMDHms = 16,//年月日时分秒+固定前值 16
    }

}
