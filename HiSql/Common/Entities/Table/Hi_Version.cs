using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    [HiTable(IsEdit = true, TabName = "Hi_Version")]
    internal class Hi_Version: StandField
    {
        [HiColumn(FieldDesc = "HiSql包名称", FieldLen = 50,FieldType =HiType.VARCHAR, IsPrimary = true, IsBllKey = true, IsNull = false, DBDefault = HiTypeDBDefault.EMPTY, DefaultValue = "", SortNum = 1, IsSys = true)]
        public string HiPackName { get; set; }

        [HiColumn(FieldDesc = "版本号", FieldLen = 20, FieldType = HiType.VARCHAR, IsNull = false, DBDefault = HiTypeDBDefault.EMPTY, DefaultValue = "", SortNum = 2, IsSys = true)]
        public string Version { get; set; }


        [HiColumn(FieldDesc = "版本号值",   FieldType = HiType.INT, IsNull = false, DBDefault = HiTypeDBDefault.EMPTY,  SortNum = 3, IsSys = true)]
        public int VerNum { get; set; }

    }
}
