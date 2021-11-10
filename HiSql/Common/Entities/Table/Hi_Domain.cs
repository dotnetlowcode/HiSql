using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql 
{
    [HiTable(IsEdit = true, TabName = "Hi_Domain",TabDescript ="数据域主表")]
    public class Hi_Domain : StandField
    {
        [HiColumn(FieldDesc = "数据域名", FieldLen = 10, IsPrimary = true, IsBllKey = true, IsNull = false, DBDefault = HiTypeDBDefault.EMPTY, SortNum = 5, IsSys = true)]
        public string Domain { get; set; }

        [HiColumn(FieldDesc = "数据域描述", FieldLen = 100,   DBDefault = HiTypeDBDefault.EMPTY, SortNum = 10, IsSys = true)]
        public string DomainDesc { get; set; }
    }
}
