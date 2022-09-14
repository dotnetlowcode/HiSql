using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql.Unit.Test
{
    [System.Serializable]
    [HiTable(IsEdit = true, TabName = "H_Test50C01")]
    public class H_Test50C01 : StandField
    {
        [HiColumn(FieldDesc = "物料号", IsPrimary = true, IsBllKey = true, FieldType = HiType.VARCHAR, FieldLen = 50, SortNum = 1, DBDefault = HiTypeDBDefault.EMPTY)]
        public string Material
        {
            get; set;
        }
        [HiColumn(FieldDesc = "批次号", IsPrimary = true, IsBllKey = true, FieldType = HiType.VARCHAR, FieldLen = 50, SortNum = 2, DBDefault = HiTypeDBDefault.EMPTY)]
        public string Batch
        {
            get; set;
        }

        [HiColumn(FieldDesc = "测试数值1", FieldType = HiType.INT, IsNull = false, SortNum = 3, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public int TestNum1
        {
            get; set;
        }

        [HiColumn(FieldDesc = "测试数值2", FieldType = HiType.INT, IsNull = false, SortNum = 4, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public int TestNum2
        {
            get; set;
        }
        [HiColumn(FieldDesc = "测试数值3", FieldType = HiType.INT, IsNull = false, SortNum = 4, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public int TestNum3
        {
            get; set;
        }


        [HiColumn(FieldDesc = "测试数值4", FieldType = HiType.INT, IsNull = false, SortNum = 4, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public int TestNum4
        {
            get; set;
        }

        [HiColumn(FieldDesc = "测试数值5", FieldType = HiType.INT, IsNull = false, SortNum = 4, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public int TestNum5
        {
            get; set;
        }
        [HiColumn(FieldDesc = "测试数值6", FieldType = HiType.INT, IsNull = false, SortNum = 4, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public int TestNum6
        {
            get; set;
        }

        [HiColumn(FieldDesc = "测试数值7", FieldType = HiType.INT, IsNull = false, SortNum = 4, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public int TestNum7
        {
            get; set;
        }
        [HiColumn(FieldDesc = "测试数值8", FieldType = HiType.INT, IsNull = false, SortNum = 4, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public int TestNum8
        {
            get; set;
        }

        [HiColumn(FieldDesc = "测试数值9", FieldType = HiType.INT, IsNull = false, SortNum = 4, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public int TestNum9
        {
            get; set;
        }
        [HiColumn(FieldDesc = "测试数值10", FieldType = HiType.INT, IsNull = false, SortNum = 4, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public int TestNum10
        {
            get; set;
        }

        [HiColumn(FieldDesc = "测试数值11", FieldType = HiType.INT, IsNull = false, SortNum = 4, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public int TestNum11
        {
            get; set;
        }
        [HiColumn(FieldDesc = "测试数值12", FieldType = HiType.INT, IsNull = false, SortNum = 4, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public int TestNum12
        {
            get; set;
        }
        [HiColumn(FieldDesc = "测试数值13", FieldType = HiType.INT, IsNull = false, SortNum = 4, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public int TestNum13
        {
            get; set;
        }
        [HiColumn(FieldDesc = "测试数值14", FieldType = HiType.INT, IsNull = false, SortNum = 4, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public int TestNum14
        {
            get; set;
        }
        [HiColumn(FieldDesc = "测试数值15", FieldType = HiType.INT, IsNull = false, SortNum = 4, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public int TestNum15
        {
            get; set;
        }
        [HiColumn(FieldDesc = "测试字符1", FieldType = HiType.NVARCHAR, FieldLen = 50, IsNull = false, SortNum = 5, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public string TestStr1
        {
            get; set;
        }
        [HiColumn(FieldDesc = "测试字符2", FieldType = HiType.NVARCHAR, FieldLen = 20, IsNull = false, SortNum = 6, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public string TestStr2
        {
            get; set;
        }
        [HiColumn(FieldDesc = "测试字符3", FieldType = HiType.NVARCHAR, FieldLen = 20, IsNull = false, SortNum = 6, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public string TestStr3
        {
            get; set;
        }


        [HiColumn(FieldDesc = "测试字符4", FieldType = HiType.NVARCHAR, FieldLen = 20, IsNull = false, SortNum = 6, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public string TestStr4
        {
            get; set;
        }

        [HiColumn(FieldDesc = "测试字符5", FieldType = HiType.NVARCHAR, FieldLen = 20, IsNull = false, SortNum = 6, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public string TestStr5
        {
            get; set;
        }
        [HiColumn(FieldDesc = "测试字符6", FieldType = HiType.NVARCHAR, FieldLen = 20, IsNull = false, SortNum = 6, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public string TestStr6
        {
            get; set;
        }
        [HiColumn(FieldDesc = "测试字符7", FieldType = HiType.NVARCHAR, FieldLen = 20, IsNull = false, SortNum = 6, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public string TestStr7
        {
            get; set;
        }
        [HiColumn(FieldDesc = "测试字符8", FieldType = HiType.NVARCHAR, FieldLen = 20, IsNull = false, SortNum = 6, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public string TestStr8
        {
            get; set;
        }
        [HiColumn(FieldDesc = "测试字符9", FieldType = HiType.NVARCHAR, FieldLen = 20, IsNull = false, SortNum = 6, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public string TestStr9
        {
            get; set;
        }
        [HiColumn(FieldDesc = "测试字符10", FieldType = HiType.NVARCHAR, FieldLen = 20, IsNull = false, SortNum = 6, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public string TestStr10
        {
            get; set;
        }
        [HiColumn(FieldDesc = "测试字符11", FieldType = HiType.NVARCHAR, FieldLen = 20, IsNull = false, SortNum = 6, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public string TestStr11
        {
            get; set;
        }
        [HiColumn(FieldDesc = "测试字符12", FieldType = HiType.NVARCHAR, FieldLen = 20, IsNull = false, SortNum = 6, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public string TestStr12
        {
            get; set;
        }
        [HiColumn(FieldDesc = "测试字符13", FieldType = HiType.VARCHAR, FieldLen = 20, IsNull = false, SortNum = 6, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public string TestStr13
        {
            get; set;

        }
        [HiColumn(FieldDesc = "测试字符14", FieldType = HiType.NVARCHAR, FieldLen = 20, IsNull = false, SortNum = 6, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public string TestStr14
        {
            get; set;
        }
        [HiColumn(FieldDesc = "测试字符15", FieldType = HiType.VARCHAR, FieldLen = 20, IsNull = false, SortNum = 6, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public string TestStr15
        {
            get; set;
        }

        [HiColumn(FieldDesc = "测试小数1", FieldType = HiType.DECIMAL, FieldLen = 18, FieldDec = 3, IsNull = false, SortNum = 4, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public decimal TestDec1
        {
            get; set;
        }
        [HiColumn(FieldDesc = "测试小数2", FieldType = HiType.DECIMAL, FieldLen = 18, FieldDec = 3, IsNull = false, SortNum = 4, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public decimal TestDec2
        {
            get; set;
        }
        [HiColumn(FieldDesc = "测试小数3", FieldType = HiType.DECIMAL, FieldLen = 18, FieldDec = 3, IsNull = false, SortNum = 4, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public decimal TestDec3
        {
            get; set;
        }
        [HiColumn(FieldDesc = "测试小数4", FieldType = HiType.DECIMAL, FieldLen = 18, FieldDec = 3, IsNull = false, SortNum = 4, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public decimal TestDec4
        {
            get; set;
        }
        [HiColumn(FieldDesc = "测试小数5", FieldType = HiType.DECIMAL, FieldLen = 18, FieldDec = 3, IsNull = false, SortNum = 4, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public decimal TestDec5
        {
            get; set;
        }
        [HiColumn(FieldDesc = "测试小数6", FieldType = HiType.DECIMAL, FieldLen = 18, FieldDec = 3, IsNull = false, SortNum = 4, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public decimal TestDec6
        {
            get; set;
        }
        [HiColumn(FieldDesc = "测试小数7", FieldType = HiType.DECIMAL, FieldLen = 18, FieldDec = 3, IsNull = false, SortNum = 4, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public decimal TestDec7
        {
            get; set;
        }
        [HiColumn(FieldDesc = "测试小数8", FieldType = HiType.DECIMAL, FieldLen = 18, FieldDec = 3, IsNull = false, SortNum = 4, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public decimal TestDec8
        {
            get; set;
        }
        [HiColumn(FieldDesc = "测试小数9", FieldType = HiType.DECIMAL, FieldLen = 18, FieldDec = 3, IsNull = false, SortNum = 4, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public decimal TestDec9
        {
            get; set;
        }
        [HiColumn(FieldDesc = "测试小数10", FieldType = HiType.DECIMAL, FieldLen = 18, FieldDec = 3, IsNull = false, SortNum = 4, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public decimal TestDec10
        {
            get; set;
        }
        [HiColumn(FieldDesc = "测试小数11", FieldType = HiType.DECIMAL, FieldLen = 18, FieldDec = 3, IsNull = false, SortNum = 4, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public decimal TestDec11
        {
            get; set;
        }
        [HiColumn(FieldDesc = "测试小数12", FieldType = HiType.DECIMAL, FieldLen = 18, FieldDec = 3, IsNull = false, SortNum = 4, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public decimal TestDec12
        {
            get; set;
        }
        [HiColumn(FieldDesc = "测试小数13", FieldType = HiType.DECIMAL, FieldLen = 18, FieldDec = 3, IsNull = false, SortNum = 4, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public decimal TestDec13
        {
            get; set;
        }
        [HiColumn(FieldDesc = "测试小数14", FieldType = HiType.DECIMAL, FieldLen = 18, FieldDec = 3, IsNull = false, SortNum = 4, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public decimal TestDec14
        {
            get; set;
        }
        [HiColumn(FieldDesc = "测试小数15", FieldType = HiType.DECIMAL, FieldLen = 18, FieldDec = 3, IsNull = false, SortNum = 4, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public decimal TestDec15
        {
            get; set;
        }
        [HiColumn(FieldDesc = "是否开启", FieldType = HiType.BOOL, IsNull = false, SortNum = 4, DBDefault = HiTypeDBDefault.EMPTY)]
        public bool IsOn
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
