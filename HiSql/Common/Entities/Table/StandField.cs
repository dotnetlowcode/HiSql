using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// 标准字段(每次创建数据库表时默认会创建以下字段)
    /// </summary>
    /// 
    [System.Serializable]
    public partial class StandField
    {
        [HiColumn(FieldName = "CreateTime", FieldDesc = "创建时间", SortNum =990, DBDefault = HiTypeDBDefault.FUNDATE, DefaultValue = "FUNDATE")]
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        [HiColumn(FieldName = "CreateName", FieldDesc = "创建人", SortNum = 991,DBDefault = HiTypeDBDefault.EMPTY)]
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateName { get; set; }

        [HiColumn(FieldName = "ModiTime", FieldDesc = "修改时间", SortNum = 995 , DBDefault = HiTypeDBDefault.FUNDATE,DefaultValue = "FUNDATE")]
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModiTime { get; set; }

        [HiColumn(FieldName = "ModiName", FieldDesc = "修改人", DBDefault=HiTypeDBDefault.EMPTY, SortNum = 998)]
        /// <summary>
        /// 修改人
        /// </summary>
        public string ModiName { get; set; }

    }
}
