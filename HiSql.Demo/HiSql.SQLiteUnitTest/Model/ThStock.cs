using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql.SqliteUnitTest.Model
{
    [System.Serializable]
    [HiTable(IsEdit = true, TabName = "ThStock")]
    public class ThStock : StandField
    {

        /// <summary>
        /// 公司代码
        /// </summary>
        /// 
        [HiColumn(FieldDesc = "公司代码", IsPrimary = true, IsBllKey = true, FieldType = HiType.VARCHAR, FieldLen = 4, SortNum = 1, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public string CompanyCode { get; set; }


        /// <summary>
        /// 工厂
        /// </summary>
        /// 

        [HiColumn(FieldDesc = "工厂", IsPrimary = true, IsBllKey = true, FieldType = HiType.VARCHAR, FieldLen = 4, SortNum = 5, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public string WorksCode { get; set; }

        /// <summary>
        /// 库位
        /// </summary>
        /// 
        [HiColumn(FieldDesc = "库位", IsPrimary = true, IsBllKey = true, FieldType = HiType.VARCHAR, FieldLen = 4, SortNum = 10, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public string LocationCode { get; set; }

        /// <summary>
        /// 条码号
        /// </summary>
        /// 

        [HiColumn(FieldDesc = "条码号", IsPrimary = true, IsBllKey = true, FieldType = HiType.VARCHAR, FieldLen = 20, SortNum = 15, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public string BarCode { get; set; }

        /// <summary>
        /// 物料号
        /// </summary>
        /// 
        [HiColumn(FieldDesc = "物料号", IsPrimary = true, IsBllKey = true, FieldType = HiType.VARCHAR, FieldLen = 20, SortNum = 20, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public string MaterialCode { get; set; }

        /// <summary>
        /// 物料描述
        /// </summary>
        /// 
        [HiColumn(FieldDesc = "物料描述", FieldType = HiType.NVARCHAR, FieldLen = 100, SortNum = 25, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public string MaterialName { get; set; }


        /// <summary>
        /// 根据配置生成的Sku编码
        /// </summary>
        [HiColumn(FieldDesc = "Sku编码", FieldType = HiType.NVARCHAR, FieldLen = 100, SortNum = 26, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public string SkuCode { get; set; }


        /// <summary>
        /// 是否按批次管理
        /// </summary>
        [HiColumn(FieldDesc = "是否批次管理", FieldType = HiType.INT, SortNum = 27, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public int IsBatch { get; set; }

        /// <summary>
        /// 在库库存 状态标识为1
        /// </summary>
        /// 
        [HiColumn(FieldDesc = "在库库存", FieldType = HiType.DECIMAL, FieldLen = 18, FieldDec = 3, SortNum = 30, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public decimal StockInventory { get; set; }

        /// <summary>
        /// 在途传输库存 状态标识为2
        /// </summary>
        /// 
        [HiColumn(FieldDesc = "在途库存", FieldType = HiType.DECIMAL, FieldLen = 18, FieldDec = 3, SortNum = 35, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public decimal TransInventory { get; set; }

        /// <summary>
        /// 冻结库存 状态标识4
        /// </summary>
        /// 
        [HiColumn(FieldDesc = "冻结库存", FieldType = HiType.DECIMAL, FieldLen = 18, FieldDec = 3, SortNum = 40, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public decimal FreezeInventory { get; set; }


        /// <summary>
        /// 保留库存，为某种业务进行预留（不与SAP同步）  状态标识8
        /// </summary>
        /// 
        [HiColumn(FieldDesc = "保留库存", FieldType = HiType.DECIMAL, FieldLen = 18, FieldDec = 3, SortNum = 45, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public decimal WithHoldInventory { get; set; }




        /// <summary>
        /// 单位
        /// </summary>
        /// 
        [HiColumn(FieldDesc = "库存单位", FieldType = HiType.VARCHAR, FieldLen = 5, SortNum = 50, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public string Unit { get; set; }


        /// <summary>
        /// 库存状态标识
        /// </summary>
        /// 
        [HiColumn(FieldDesc = "库存状态", FieldType = HiType.INT, SortNum = 51, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public int InventoryFlag { get; set; }

        /// <summary>
        /// 是否锁定 锁定后不允许任意操作,不与SAP同步  状态标识16
        /// </summary>
        /// 
        [HiColumn(FieldDesc = "是否锁定库存", FieldType = HiType.INT, SortNum = 55, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public int IsLock { get; set; } = 0;

        /// <summary>
        /// 库存锁定人
        /// </summary>
        [HiColumn(FieldDesc = "库存锁定人", FieldType = HiType.NVARCHAR, FieldLen = 50, SortNum = 60, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public string LockUser { get; set; }

        /// <summary>
        /// 锁定时间
        /// </summary>
        [HiColumn(FieldDesc = "锁定时间", FieldType = HiType.DATETIME, SortNum = 65, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public DateTime LockTime { get; set; }

        /// <summary>
        /// 解锁时间
        /// </summary>
        [HiColumn(FieldDesc = "解锁时间", FieldType = HiType.DATETIME, SortNum = 65, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public DateTime UnLockTime { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        [HiColumn(FieldDesc = "备注信息", FieldType = HiType.NVARCHAR, FieldLen = 100, SortNum = 70, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public string Descript { get; set; }

        /// <summary>
        /// 备注信息2
        /// </summary>
        [HiColumn(FieldDesc = "备注信息2", FieldType = HiType.NVARCHAR, FieldLen = 100, SortNum = 75, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
        public string Descript2 { get; set; }


    }
}
