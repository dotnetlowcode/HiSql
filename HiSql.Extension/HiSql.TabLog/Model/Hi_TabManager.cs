using HiSql.TabLog.Interface;

namespace HiSql.TabLog.Model
{
    [System.Serializable]
    [HiTable(IsEdit = true, TabName = "Hi_TabManager")]
    public class Hi_TabManager : StandField, ILogTable
    {
        /// <summary>
        /// 表名
        /// </summary>
        [HiColumn(
            FieldDesc = "表名",
            IsPrimary = true,
            IsBllKey = true,
            IsNull = false,
            FieldType = HiType.NVARCHAR,
            FieldLen = 200,
            FieldDec = 0,
            SortNum = 1,
            DBDefault = HiTypeDBDefault.EMPTY
        )]
        public string TabName { get; set; } = "";

        /// <summary>
        /// 日志服务器连接名称
        /// </summary>
        [HiColumn(
            FieldDesc = "日志服务器连接名称",
            IsPrimary = false,
            IsBllKey = false,
            IsNull = false,
            FieldType = HiType.NVARCHAR,
            FieldLen = 50,
            FieldDec = 0,
            SortNum = 3,
            DBDefault = HiTypeDBDefault.EMPTY
        )]
        public string DbServer { get; set; } = "";

        /// <summary>
        /// 是否开启日志
        /// </summary>
        [HiColumn(
            FieldDesc = "是否开启日志",
            IsPrimary = false,
            IsBllKey = false,
            IsNull = false,
            FieldType = HiType.INT,
            FieldLen = 0,
            FieldDec = 0,
            SortNum = 5,
            DBDefault = HiTypeDBDefault.EMPTY
        )]
        public int IsLog { get; set; } = 0;

        /// <summary>
        /// 主日志表
        /// </summary>
        [HiColumn(
            FieldDesc = "主日志表",
            IsPrimary = false,
            IsBllKey = false,
            IsNull = false,
            FieldType = HiType.NVARCHAR,
            FieldLen = 200,
            FieldDec = 0,
            SortNum = 7,
            DBDefault = HiTypeDBDefault.EMPTY
        )]
        public string MainTabLog { get; set; } = "";

        /// <summary>
        /// 明细日志表
        /// 明细日志表
        /// </summary>
        [HiColumn(
            FieldDesc = "明细日志表",
            IsPrimary = false,
            IsBllKey = false,
            IsNull = false,
            FieldType = HiType.NVARCHAR,
            FieldLen = 200,
            FieldDec = 0,
            SortNum = 9,
            DBDefault = HiTypeDBDefault.EMPTY
        )]
        public string DetailTabLog { get; set; } = "";

        /// <summary>
        /// SNRO主编号
        /// </summary>
        [HiColumn(
            FieldDesc = "SNRO主编号",
            IsPrimary = false,
            IsBllKey = false,
            IsNull = false,
            FieldType = HiType.NVARCHAR,
            FieldLen = 50,
            FieldDec = 0,
            SortNum = 11,
            DBDefault = HiTypeDBDefault.EMPTY
        )]
        public string SNRO { get; set; } = "";

        /// <summary>
        /// SNRO子编号
        /// </summary>
        [HiColumn(
            FieldDesc = "SNRO子编号",
            IsPrimary = false,
            IsBllKey = false,
            IsNull = false,
            FieldType = HiType.INT,
            FieldLen = 0,
            FieldDec = 0,
            SortNum = 13,
            DBDefault = HiTypeDBDefault.EMPTY
        )]
        public int SNUM { get; set; } = 0;

        /// <summary>
        /// 0|1是否允许恢复
        /// 是否允许恢复
        /// </summary>
        [HiColumn(
            FieldDesc = "0|1是否允许恢复",
            IsPrimary = false,
            IsBllKey = false,
            IsNull = false,
            FieldType = HiType.INT,
            FieldLen = 0,
            FieldDec = 0,
            SortNum = 15,
            DBDefault = HiTypeDBDefault.EMPTY
        )]
        public int IsRecover { get; set; } = 0;

        /// <summary>
        /// 0|1是否允许删除
        /// </summary>
        [HiColumn(
            FieldDesc = "0|1是否允许删除",
            IsPrimary = false,
            IsBllKey = false,
            IsNull = false,
            FieldType = HiType.INT,
            FieldLen = 0,
            FieldDec = 0,
            SortNum = 17,
            DBDefault = HiTypeDBDefault.EMPTY
        )]
        public int IsDel { get; set; } = 0;

        /// <summary>
        /// 日志记录模式
        /// 日志记录模式1:全量记录
        /// </summary>
        [HiColumn(
            FieldDesc = "日志记录模式",
            IsPrimary = false,
            IsBllKey = false,
            IsNull = false,
            FieldType = HiType.INT,
            FieldLen = 0,
            FieldDec = 0,
            SortNum = 19,
            DBDefault = HiTypeDBDefault.EMPTY
        )]
        public int LogModel { get; set; } = 0;

        /// <summary>
        /// 日志存储天数
        /// 日志存储天数0表示不限期
        /// </summary>
        [HiColumn(
            FieldDesc = "日志存储天数",
            IsPrimary = false,
            IsBllKey = false,
            IsNull = false,
            FieldType = HiType.INT,
            FieldLen = 0,
            FieldDec = 0,
            SortNum = 21,
            DBDefault = HiTypeDBDefault.EMPTY
        )]
        public int StoreDay { get; set; } = 0;

        /// <summary>
        /// 备注
        /// </summary>
        [HiColumn(
            FieldDesc = "备注",
            IsPrimary = false,
            IsBllKey = false,
            IsNull = false,
            FieldType = HiType.NVARCHAR,
            FieldLen = 200,
            FieldDec = 0,
            SortNum = 23,
            DBDefault = HiTypeDBDefault.EMPTY
        )]
        public string Remark { get; set; } = "";
    }
}
