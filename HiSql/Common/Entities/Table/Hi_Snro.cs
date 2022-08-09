using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    [System.Serializable]
    [HiTable(IsEdit = false, TabName = "Hi_Snro",TabDescript ="编号配置表", TabStatus = TabStatus.Use)]
    public class Hi_Snro:StandField
    {
        [HiColumn(FieldDesc = "SNRO主编号", IsPrimary = true, IsBllKey = true, IsNull = false, FieldLen = 50, SortNum = 5, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY)]
        public string SNRO {
            get;set;
        }
        [HiColumn(FieldDesc = "SNRO子编号", IsPrimary = true, IsBllKey = true, IsNull = false,   SortNum = 10, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY)]
        public int SNUM
        {
            get;set;
        }
        [HiColumn(FieldDesc = "是否雪花ID", FieldType =HiType.BOOL,   IsNull = false, SortNum = 15, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY)]
        public bool IsSnow
        {
            get; set;
        }

        [HiColumn(FieldDesc = "雪花ID时间戳", FieldType = HiType.BIGINT, IsNull = false, SortNum = 20, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY)]
        public Int64 SnowTick
        {
            get; set;
        }

        /// <summary>
        /// 开始编号值
        /// </summary>
        /// 
        [HiColumn(FieldDesc = "起始编号", FieldLen = 20, FieldType = HiType.NVARCHAR, IsNull = false, SortNum = 25, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY)]
        public string StartNum
        {
            get;set;
        }

        /// <summary>
        /// 结束编号值
        /// </summary>
        /// 
        [HiColumn(FieldDesc = "结束编号", FieldLen = 20, FieldType = HiType.NVARCHAR, IsNull = false, SortNum = 30, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY)]
        public string EndNum
        {
            get;set;
        }
        /// <summary>
        /// 当前编号值
        /// </summary>
        /// 
        [HiColumn(FieldDesc = "当前编号", FieldLen = 20, FieldType = HiType.NVARCHAR, IsNull = false, SortNum = 35, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY)]
        public string CurrNum
        {
            get;set;
        }

        /// <summary>
        /// 包括前辍当前编号值
        /// </summary>
        /// 

        [HiColumn(FieldDesc = "当前全编号", FieldLen = 40, FieldType = HiType.NVARCHAR, IsNull = false, SortNum = 40, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY)]
        public string CurrAllNum
        {
            get;set;
        }

        [HiColumn(FieldDesc = "编号长度", FieldType = HiType.INT, IsNull = false, SortNum = 45, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY)]
        public int Length
        {
            get;set;
        }

        /// <summary>
        /// 是否纯数字编号
        /// </summary>
        /// 
        [HiColumn(FieldDesc = "是否纯数字编号",  FieldType = HiType.BOOL, IsNull = false, SortNum = 50, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY)]
        public bool IsNumber
        {
            get;set;    
        }

        /// <summary>
        /// 是否有编号前辍
        /// </summary>
        /// 
        [HiColumn(FieldDesc = "是否有编号前辍", FieldType = HiType.BOOL, IsNull = false, SortNum = 55, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY)]
        public bool IsHasPre
        {
            get;set;
        }

        /// <summary>
        /// 前置编号类型
        /// </summary>
        /// 
        [HiColumn(FieldDesc = "前置编号类型", FieldType = HiType.INT, IsNull = false, SortNum = 60, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY)]
        public PreType PreType
        {
            get;set;
        }


        /// <summary>
        /// 固定前置编号字符串
        /// </summary>
        /// 
        [HiColumn(FieldDesc = "固定前置编号字符串", FieldType = HiType.NVARCHAR, FieldLen =20, IsNull = false, SortNum = 65, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY)]
        public string FixPreChar
        {
            get;set;
        }

        /// <summary>
        /// 前置字符串
        /// </summary>
        /// 
        [HiColumn(FieldDesc = "前置编号字符串", FieldType = HiType.NVARCHAR, FieldLen = 20, IsNull = false, SortNum = 70, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY)]
        public string PreChar
        {
            get;set;
        }

        /// <summary>
        /// 缓存空间大小
        /// </summary>
        /// 
        [HiColumn(FieldDesc = "缓存空间大小", FieldType = HiType.INT, IsNull = false, SortNum = 75, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY)]
        public int CacheSpace
        {
            get;set;

        }


        [HiColumn(FieldDesc = "当前缓存值", FieldType = HiType.INT, IsNull = false, SortNum = 80, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY)]
        public int CurrCacheSpace
        {
            get; set;

        }


        /// <summary>
        /// 描述编号
        /// </summary>
        /// 
        [HiColumn(FieldDesc = "描述编号", FieldType = HiType.NVARCHAR, FieldLen = 100, IsNull = false, SortNum = 85, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY)]
        public string Descript
        {
            get;set;
        }

    }
}
