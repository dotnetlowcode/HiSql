using HiSql.TabLog.Interface;

namespace HiSql.TabLog.Model
{
    [HiTable(IsEdit = true, TabName = "Hi_MainLog")]
    public class Hi_MainLog
        : StandField, ILogTable
    {
        /// <summary>
        /// 日志编号
        /// 日志编号
        /// <summary> 
        [HiColumn(FieldDesc = "日志编号", IsPrimary = true, IsBllKey = true, IsNull = false, FieldType = HiType.VARCHAR, FieldLen = 20, FieldDec = 0, SortNum = 1, DBDefault = HiTypeDBDefault.EMPTY)]
        public string LogId { get; set; } = "";

        /// <summary>
        /// 表名
        /// 表名
        /// <summary> 
        [HiColumn(FieldDesc = "表名", IsPrimary = true, IsBllKey = true, IsNull = false, FieldType = HiType.NVARCHAR, FieldLen = 200, FieldDec = 0, SortNum = 3, DBDefault = HiTypeDBDefault.EMPTY)]
        public string TabName { get; set; } = "";

        /// <summary>
        /// 日志记录模式
        /// 日志记录模式1:全量记录 
        /// <summary> 
        [HiColumn(FieldDesc = "日志记录模式", IsPrimary = false, IsBllKey = false, IsNull = false, FieldType = HiType.INT, FieldLen = 0, FieldDec = 0, SortNum = 5, DBDefault = HiTypeDBDefault.EMPTY)]
        public int LogModel { get; set; } = 0;

        /// <summary>
        /// 明细日志表
        /// 明细日志表
        /// <summary> 
        [HiColumn(FieldDesc = "明细日志表", IsPrimary = false, IsBllKey = false, IsNull = false, FieldType = HiType.NVARCHAR, FieldLen = 200, FieldDec = 0, SortNum = 7, DBDefault = HiTypeDBDefault.EMPTY)]
        public string DetailTabLog { get; set; } = "";

        /// <summary>
        /// 修改记录数
        /// 修改记录数
        /// <summary> 
        [HiColumn(FieldDesc = "修改记录数", IsPrimary = false, IsBllKey = false, IsNull = false, FieldType = HiType.INT, FieldLen = 0, FieldDec = 0, SortNum = 9, DBDefault = HiTypeDBDefault.EMPTY)]
        public int MCount { get; set; } = 0;

        /// <summary>
        /// 创建记录数
        /// 创建记录数
        /// <summary> 
        [HiColumn(FieldDesc = "创建记录数", IsPrimary = false, IsBllKey = false, IsNull = false, FieldType = HiType.INT, FieldLen = 0, FieldDec = 0, SortNum = 11, DBDefault = HiTypeDBDefault.EMPTY)]
        public int CCount { get; set; } = 0;

        /// <summary>
        /// 删除记录数
        /// 删除记录数
        /// <summary> 
        [HiColumn(FieldDesc = "删除记录数", IsPrimary = false, IsBllKey = false, IsNull = false, FieldType = HiType.INT, FieldLen = 0, FieldDec = 0, SortNum = 13, DBDefault = HiTypeDBDefault.EMPTY)]
        public int DCount { get; set; } = 0;

        /// <summary>
        /// 0|1是否已经被恢复
        /// 是否已经被恢复
        /// <summary> 
        [HiColumn(FieldDesc = "0|1是否已经被恢复", IsPrimary = false, IsBllKey = false, IsNull = false, FieldType = HiType.INT, FieldLen = 0, FieldDec = 0, SortNum = 15, DBDefault = HiTypeDBDefault.EMPTY)]
        public int IsRecover { get; set; } = 0;

        /// <summary>
        /// 参考日志编号
        /// 参考日志编号
        /// <summary> 
        [HiColumn(FieldDesc = "参考日志编号", IsPrimary = false, IsBllKey = false, IsNull = false, FieldType = HiType.VARCHAR, FieldLen = 20, FieldDec = 0, SortNum = 17, DBDefault = HiTypeDBDefault.EMPTY)]
        public string RefLogId { get; set; } = "";

    }
}
