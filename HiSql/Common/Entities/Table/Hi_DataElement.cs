using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    [HiTable(IsEdit = false, TabName = "Hi_DataElement", TabDescript = "数据域明细表", TabStatus = TabStatus.Use)]
    public class Hi_DataElement : StandField
    {
        [HiColumn(FieldDesc = "数据域名", FieldLen = 10, IsPrimary = true, IsBllKey = true, IsNull = false, DBDefault = HiTypeDBDefault.EMPTY, SortNum = 5, IsSys = true)]
        public string Domain { get; set; }

        [HiColumn(FieldDesc = "数据域值", FieldLen = 50, IsPrimary = true, IsBllKey = true, IsNull = false, DBDefault = HiTypeDBDefault.EMPTY, SortNum = 10, IsSys = true)]
        public string ElementValue { get; set; }

        [HiColumn(FieldDesc = "数据域值描述", FieldLen = 100,   DBDefault = HiTypeDBDefault.EMPTY, SortNum = 15, IsSys = true)]
        public string ElementDesc { get; set; }

        [HiColumn(FieldDesc = "数据域排序号",   DBDefault = HiTypeDBDefault.VALUE, DefaultValue ="0", SortNum = 20, IsSys = true)]
        public int SortNum { get; set; }
    }
}
