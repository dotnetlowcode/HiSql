using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{

    /// <summary>
    /// 
    /// </summary>
    //[HiTable(TabType=TabType.Config,TabName = "TabModel", TabReName ="表模型表")]
    /// <summary>
    /// 表信息
    /// 用于表数据信息
    /// </summary>
    [HiTable(IsEdit = true, TabName = "Hi_TabModel")]
    public partial class Hi_TabModel:StandField
    {



        /// <summary>
        /// 表名
        /// </summary>
        [HiColumn(FieldDesc = "表名", IsPrimary = true, IsBllKey = true,IsNull =false, FieldLen = 50, SortNum = 1, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY)]
        public string TabName { get; set; }



        /// <summary>
        /// 表的别名
        /// </summary>
        [HiColumn(FieldDesc= "表的别名",  FieldLen = 50, SortNum = 5, DBDefault = HiTypeDBDefault.EMPTY)]
        public string TabReName { get; set; }


        /// <summary>
        /// 表描述
        /// </summary>
        [HiColumn(FieldDesc = "表描述", FieldLen = 100, SortNum = 5, DBDefault = HiTypeDBDefault.EMPTY)]
        public string TabDescript { get; set; }

        /// <summary>
        /// 表的存储方式
        /// </summary>
        [HiColumn(FieldDesc = "表存储方式",  SortNum = 10, DBDefault = HiTypeDBDefault.EMPTY)]
        public int TabStoreType { get; set; }

        /// <summary>
        /// 表类型
        /// </summary>
        [HiColumn(FieldDesc = "表类型", SortNum = 15,DBDefault =HiTypeDBDefault.EMPTY)]
        public int TabType { get; set; }

        /// <summary>
        /// 表的缓存类型
        /// </summary>
        [HiColumn(FieldDesc = "表的缓存类型", SortNum = 20, DBDefault = HiTypeDBDefault.EMPTY)]
        public int TabCacheType { get; set; }

        /// <summary>
        /// 表状态
        /// </summary>
        [HiColumn(FieldDesc = "表状态", SortNum = 25, DBDefault = HiTypeDBDefault.EMPTY)]
        public int TabStatus { get; set; }


        /// <summary>
        /// 是否系统内置表
        /// </summary>
        [HiColumn(FieldDesc = "是否系统内置表", SortNum = 30, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY)]
        public bool IsSys { get; set; }


        /// <summary>
        /// 是否可编辑
        /// </summary>
        [HiColumn(FieldDesc = "是否可编辑", SortNum = 35, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY)]
        public bool IsEdit { get; set; }


        /// <summary>
        /// 是否开启表日志
        /// </summary>
        [HiColumn(FieldDesc = "是否开启表日志", SortNum = 40, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY)]
        public bool IsLog { get; set; }


        /// <summary>
        /// 日志记录表
        /// </summary>
        [HiColumn(FieldDesc = "日志表名", IsNull = false, FieldLen = 50, SortNum = 45, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY)]
        public string LogTable { get; set; }

        /// <summary>
        /// 日志保留天数
        /// </summary>
        [HiColumn(FieldDesc = "日志保留天数", SortNum = 50, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY)]
        public int LogExprireDay { get; set; }


        
    }
}
