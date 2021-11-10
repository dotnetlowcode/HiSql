using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql.SNRO
{
    [System.Serializable]
    [HiTable(IsEdit = true, TabName = "Hi_Snro")]
    public class Hi_Snro:StandField
    {
        [HiColumn(FieldDesc = "SNRO主编号", IsPrimary = true, IsBllKey = true, IsNull = false, FieldLen = 10, SortNum = 1, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY)]
        public string SNRO {
            get;set;
        }
        [HiColumn(FieldDesc = "SNRO子编号", IsPrimary = true, IsBllKey = true, IsNull = false,   SortNum = 2, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY)]
        public int SNUM
        {
            get;set;
        }

        /// <summary>
        /// 开始编号值
        /// </summary>
        /// 
        [HiColumn(FieldDesc = "起始编号", FieldLen = 20, FieldType = HiType.NVARCHAR, IsNull = false, SortNum = 3, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY)]
        public string StartNum
        {
            get;set;
        }

        /// <summary>
        /// 结束编号值
        /// </summary>
        /// 
        [HiColumn(FieldDesc = "结束编号", FieldLen = 20, FieldType = HiType.NVARCHAR, IsNull = false, SortNum = 4, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY)]
        public string EndNum
        {
            get;set;
        }
        /// <summary>
        /// 当前编号值
        /// </summary>
        /// 
        [HiColumn(FieldDesc = "当前编号", FieldLen = 20, FieldType = HiType.NVARCHAR, IsNull = false, SortNum = 5, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY)]
        public string CurrNum
        {
            get;set;
        }

        /// <summary>
        /// 包括前辍当前编号值
        /// </summary>
        /// 

        [HiColumn(FieldDesc = "当前全编号", FieldLen = 40, FieldType = HiType.NVARCHAR, IsNull = false, SortNum = 6, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY)]
        public string CurrAllNum
        {
            get;set;
        }

        [HiColumn(FieldDesc = "编号长度", FieldType = HiType.INT, IsNull = false, SortNum = 7, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY)]
        public int Length
        {
            get;set;
        }

        /// <summary>
        /// 是否纯数字编号
        /// </summary>
        /// 
        [HiColumn(FieldDesc = "是否纯数字编号",  FieldType = HiType.BOOL, IsNull = false, SortNum = 8, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY)]
        public bool IsNumber
        {
            get;set;    
        }

        /// <summary>
        /// 是否有编号前辍
        /// </summary>
        /// 
        [HiColumn(FieldDesc = "是否有编号前辍", FieldType = HiType.BOOL, IsNull = false, SortNum = 9, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY)]
        public bool IsHasPre
        {
            get;set;
        }

        /// <summary>
        /// 前置编号类型
        /// </summary>
        /// 
        [HiColumn(FieldDesc = "前置编号类型", FieldType = HiType.INT, IsNull = false, SortNum = 10, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY)]
        public Enums.PreType PreType
        {
            get;set;
        }


        /// <summary>
        /// 固定前置编号字符串
        /// </summary>
        /// 
        [HiColumn(FieldDesc = "固定前置编号字符串", FieldType = HiType.NVARCHAR, FieldLen =20, IsNull = false, SortNum = 11, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY)]
        public string FixPreChar
        {
            get;set;
        }

        /// <summary>
        /// 前置字符串
        /// </summary>
        /// 
        [HiColumn(FieldDesc = "前置编号字符串", FieldType = HiType.NVARCHAR, FieldLen = 20, IsNull = false, SortNum = 12, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY)]
        public string PreChar
        {
            get;set;
        }

        /// <summary>
        /// 缓存空间大小
        /// </summary>
        /// 
        [HiColumn(FieldDesc = "缓存空间大小", FieldType = HiType.INT, IsNull = false, SortNum = 13, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY)]
        public int CacheSpace
        {
            get;set;

        }


        [HiColumn(FieldDesc = "当前缓存值", FieldType = HiType.INT, IsNull = false, SortNum = 13, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY)]
        public int CurrCacheSpace
        {
            get; set;

        }


        /// <summary>
        /// 描述编号
        /// </summary>
        /// 
        [HiColumn(FieldDesc = "描述编号", FieldType = HiType.NVARCHAR, FieldLen = 100, IsNull = false, SortNum = 14, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY)]
        public string Descript
        {
            get;set;
        }

    }
}
