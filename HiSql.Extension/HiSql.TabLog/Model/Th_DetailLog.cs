using HiSql.TabLog.Interface;

namespace HiSql.TabLog.Model
{
    [System.Serializable]
    [HiTable(IsEdit = true, TabName = "Th_DetailLog")]
    public class Th_DetailLog : StandField, ILogTable
    {
        /// <summary>
        /// 日志编号
        /// 日志编号
        /// </summary>
        [HiColumn(
            FieldDesc = "日志编号",
            IsPrimary = true,
            IsBllKey = true,
            IsNull = false,
            FieldType = HiType.VARCHAR,
            FieldLen = 20,
            FieldDec = 0,
            SortNum = 1,
            DBDefault = HiTypeDBDefault.EMPTY
        )]
        public string LogId { get; set; } = "";

        /// <summary>
        /// 表名
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
            SortNum = 3,
            DBDefault = HiTypeDBDefault.EMPTY
        )]
        public string TabName { get; set; } = "";

        /// <summary>
        /// D|C|M
        /// 删除,创建,修改
        /// </summary>
        [HiColumn(
            FieldDesc = "D|C|M",
            IsPrimary = false,
            IsBllKey = false,
            IsNull = false,
            FieldType = HiType.VARCHAR,
            FieldLen = 4,
            FieldDec = 0,
            SortNum = 5,
            DBDefault = HiTypeDBDefault.EMPTY
        )]
        public string ActionModel { get; set; } = "0";

        /// <summary>
        /// 原值
        /// 原值:Json格式
        /// </summary>
        [HiColumn(
            FieldDesc = "原值",
            IsPrimary = false,
            IsBllKey = false,
            IsNull = false,
            FieldType = HiType.TEXT,
            FieldLen = 0,
            FieldDec = 0,
            SortNum = 7,
            DBDefault = HiTypeDBDefault.EMPTY
        )]
        public string OldVal { get; set; } = "";

        /// <summary>
        /// 新值
        /// 新值:Json格式
        /// </summary>
        [HiColumn(
            FieldDesc = "新值",
            IsPrimary = false,
            IsBllKey = false,
            IsNull = false,
            FieldType = HiType.TEXT,
            FieldLen = 0,
            FieldDec = 0,
            SortNum = 9,
            DBDefault = HiTypeDBDefault.EMPTY
        )]
        public string NewVal { get; set; } = "";
    }
}
