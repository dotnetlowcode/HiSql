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
        /// <summary>
        /// 无编号前置类型
        /// </summary>
        None = 0,//0 无前置
        /// <summary>
        /// 年度前置 如2022
        /// </summary>
        Y = 1,//年  1  如
        /// <summary>
        /// 两位年前置 如22
        /// </summary>
        Y2 = 12,//

        /// <summary>
        /// 年月前置 如202206
        /// </summary>
        YM = 2,//年月 2

        /// <summary>
        /// 两位年+月前置 如2206
        /// </summary>
        Y2M=22,

        /// <summary>
        /// 年月日前置 20220602
        /// </summary>
        YMD = 3,//年月日 3

        /// <summary>
        /// 两位年+月日前置 220602
        /// </summary>
        Y2MD = 32,

        /// <summary>
        /// 年月日时前置 如2022060211
        /// </summary>
        YMDH = 4,//年月日时 4

        /// <summary>
        /// 两位年+月日时前置 如22060211
        /// </summary>
        Y2MDH = 42,


        /// <summary>
        /// 年月日时分 如202206021152
        /// </summary>
        YMDHm = 5,//年月日时分 5

        /// <summary>
        /// 两位年+月日时分 如2206021152
        /// </summary>
        Y2MDHm = 52,

        /// <summary>
        /// 年月日时分秒 如20220602115212
        /// </summary>
        YMDHms = 6,//年月日时分秒 6

        /// <summary>
        /// 两位年+月日时分秒 如220602115212
        /// </summary>
        Y2MDHms = 62,




    }

}
