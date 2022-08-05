using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    [HiTable(IsEdit = false, TabName = "Hi_Version", TabDescript = "版本记录",TabStatus =TabStatus.Use)]
    internal class Hi_Version: StandField
    {
        [HiColumn(FieldDesc = "HiSql包名称", FieldLen = 50 , IsPrimary = true, IsBllKey = true, IsNull = false, DBDefault = HiTypeDBDefault.EMPTY, DefaultValue = "", SortNum = 1, IsSys = true)]
        public string HiPackName { get; set; }

        [HiColumn(FieldDesc = "版本号", FieldLen = 20,    DBDefault = HiTypeDBDefault.EMPTY, DefaultValue = "", SortNum = 2, IsSys = true)]
        public string Version { get; set; }


        [HiColumn(FieldDesc = "版本号值",   FieldType = HiType.INT,DBDefault = HiTypeDBDefault.VALUE, DefaultValue ="0",  SortNum = 3, IsSys = true)]
        public int VerNum { get; set; }

    }
}
