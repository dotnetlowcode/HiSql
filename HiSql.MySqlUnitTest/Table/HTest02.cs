using HiSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql.MySqlUnitTest.Table
{
    [System.Serializable]
    [HiTable(IsEdit = true, TabName = "HTest02")]
    public class HTest02 : StandField
    {
        //[SugarColumn(IsPrimaryKey =true)]
        [HiColumn(FieldDesc = "编号", IsPrimary = true,  IsBllKey = true, FieldType = HiType.INT, SortNum = 1, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public int SID
        {
            get; set;
        }
        [HiColumn(FieldDesc = "姓名", FieldType = HiType.NVARCHAR, FieldLen = 50, IsNull = false, SortNum = 2, DBDefault = HiTypeDBDefault.EMPTY)]
        public string UName
        {
            get; set;
        }

        [HiColumn(FieldDesc = "年龄", FieldType = HiType.INT, IsNull = false, SortNum = 3, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY)]
        public int Age
        {
            get; set;
        }

        [HiColumn(FieldDesc = "薪水", FieldType = HiType.DECIMAL, FieldDec = 2, FieldLen = 18, IsNull = false, SortNum = 4, DBDefault = HiTypeDBDefault.EMPTY)]
        public int Salary
        {
            get; set;
        }
        [HiColumn(FieldDesc = "描述编号", FieldType = HiType.NVARCHAR, FieldLen = 100, IsNull = false, SortNum = 5, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY)]
        public string Descript
        {
            get; set;
        }

    }
}
