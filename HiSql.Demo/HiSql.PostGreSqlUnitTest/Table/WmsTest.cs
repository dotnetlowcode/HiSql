using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql.PostGreSqlUnitTest.Table
{
    public class WmsTest : StandField
    {
        /// <summary> 
        /// 测试编号 
        /// 测试编号 
        /// <summary>  
        [HiColumn(FieldDesc = "测试编号", IsPrimary = true, IsBllKey = true, IsNull = false, SNO = "ShopExId", SNO_NUM = "2", FieldType = HiType.VARCHAR, FieldLen = 10, FieldDec = 0, SortNum = 1, DBDefault = HiTypeDBDefault.EMPTY)]
        public string TstId { get; set; } = "";

        /// <summary> 
        /// Sku编码 
        /// Sku编码 
        /// <summary>  
        [HiColumn(FieldDesc = "Sku编码", IsPrimary = false, IsBllKey = false, IsNull = false, FieldType = HiType.NVARCHAR, FieldLen = 100, FieldDec = 0, SortNum = 3, DBDefault = HiTypeDBDefault.EMPTY)]
        public string Tdesc { get; set; } = "";

        /// <summary> 
        /// 备注 
        /// 备注 
        /// <summary>  
        [HiColumn(FieldDesc = "备注", IsPrimary = false, IsBllKey = false, IsNull = false, FieldType = HiType.NVARCHAR, FieldLen = 200, FieldDec = 0, SortNum = 5, DBDefault = HiTypeDBDefault.EMPTY)]
        public string Remark { get; set; } = "";

    }
}
