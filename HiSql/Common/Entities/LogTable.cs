using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{


    [HiTable(IsEdit = false, TabName = "Hi_TabLog", TabDescript = "表日志管理表")]
    public class Hi_TabLog : StandField
    {


        /// <summary>
        /// 服务器名称
        /// </summary>
        [HiColumn(FieldDesc = "DB服务器", FieldLen = 50, FieldType = HiType.NVARCHAR, DBDefault = HiTypeDBDefault.EMPTY, DefaultValue = "", SortNum = 2, IsSys = true)]
        public string DbServer { get; set; }

        /// <summary>
        /// 数据库名
        /// </summary>
        [HiColumn(FieldDesc = "数据库名", FieldLen = 50, FieldType = HiType.NVARCHAR, DBDefault = HiTypeDBDefault.EMPTY, DefaultValue = "", SortNum = 3, IsSys = true)]
        public string DbName { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        [HiColumn(FieldDesc = "表名", IsPrimary = true, FieldLen = 50, SortNum = 4, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY)]
        public string TabName { get; set; }



    }


    /// <summary>
    /// 日志主表结构
    /// </summary>
    [HiTable(IsEdit = false, TabName = "LogTable", TabDescript = "表日志主表", TabStatus = TabStatus.Use)]
    public class LogTable : StandField
    {
        /// <summary>
        /// 日志编号ID
        /// </summary>
        [HiColumn(FieldDesc = "日志编号ID", FieldType = HiType.VARCHAR, FieldLen = 20, IsPrimary = true, IsBllKey = true, IsNull = false, DBDefault = HiTypeDBDefault.EMPTY, DefaultValue = "", SortNum = 1, IsSys = true)]
        public string LogId { get;set; }

        /// <summary>
        /// 服务器名称
        /// </summary>
        [HiColumn(FieldDesc = "DB服务器", FieldLen = 50, FieldType = HiType.NVARCHAR, DBDefault = HiTypeDBDefault.EMPTY, DefaultValue = "", SortNum = 2, IsSys = true)]
        public string DbServer { get; set; }

        /// <summary>
        /// 数据库名
        /// </summary>
        [HiColumn(FieldDesc = "数据库名", FieldLen = 50, FieldType = HiType.NVARCHAR, DBDefault = HiTypeDBDefault.EMPTY, DefaultValue = "", SortNum = 3, IsSys = true)]
        public string DbName { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        [HiColumn(FieldDesc = "表名",    FieldLen = 50, SortNum = 4, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY)]
        public string TabName { get; set; }

        [HiColumn(FieldDesc = "表记录类型", FieldType = HiType.INT, SortNum = 5, DBDefault = HiTypeDBDefault.EMPTY)]
        public int RecordType { get; set; }


        [HiColumn(FieldDesc = "修改记录数", FieldType = HiType.INT, SortNum = 6, DBDefault = HiTypeDBDefault.EMPTY)]
        public int Mcount { get; set; }

        [HiColumn(FieldDesc = "新增记录数", FieldType = HiType.INT, SortNum = 7, DBDefault = HiTypeDBDefault.EMPTY)]
        public int Ncount { get; set; }

        [HiColumn(FieldDesc = "删除记录数", FieldType =HiType.INT, SortNum = 8, DBDefault = HiTypeDBDefault.EMPTY)]
        public int Dcount { get; set; }

        [HiColumn(FieldDesc = "参照日志编号", FieldLen = 20,  DBDefault = HiTypeDBDefault.EMPTY, DefaultValue = "", SortNum = 9, IsSys = true)]
        public string RefLogId { get; set; }
    }

    [HiTable(IsEdit = false, TabName = "LogTableDetail", TabDescript = "表日志明细表", TabStatus = TabStatus.Use)]
    public class LogTableDetail : StandField
    {
        /// <summary>
        /// 日志编号ID
        /// </summary>
        [HiColumn(FieldDesc = "日志编号ID", FieldLen = 20, IsPrimary = true, IsBllKey = true, IsNull = false, DBDefault = HiTypeDBDefault.EMPTY, DefaultValue = "", SortNum = 1, IsSys = true)]
        public string LogId { get; set; }

        [HiColumn(FieldDesc = "表记录类型", IsPrimary = true, FieldType = HiType.INT, SortNum = 2, DBDefault = HiTypeDBDefault.EMPTY)]
        public int RecordActionType { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        [HiColumn(FieldDesc = "表名",   FieldType =HiType.NVARCHAR, FieldLen = 50, SortNum = 3, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY)]
        public string TabName { get; set; }


        

        [HiColumn(FieldDesc = "是否生成日志文件",  FieldType = HiType.BOOL, SortNum = 4, DBDefault = HiTypeDBDefault.EMPTY)]
        public bool  IsLogFile { get; set; }


        [HiColumn(FieldDesc = "原值", FieldType = HiType.TEXT, SortNum = 5, DBDefault = HiTypeDBDefault.EMPTY)]
        public string OldValue { get; set; }

        [HiColumn(FieldDesc = "原值json路径", FieldType = HiType.VARCHAR,FieldLen =200, SortNum = 6, DBDefault = HiTypeDBDefault.EMPTY)]
        public string OldLogFile { get; set; }

        [HiColumn(FieldDesc = "原值长度",   FieldType = HiType.BIGINT, SortNum = 7, DBDefault = HiTypeDBDefault.EMPTY)]
        public Int64 OldLen { get; set; }




        [HiColumn(FieldDesc = "新值", FieldType = HiType.TEXT, SortNum = 9, DBDefault = HiTypeDBDefault.EMPTY)]
        public string NewValue { get; set; }

        [HiColumn(FieldDesc = "新值json路径", FieldType = HiType.VARCHAR, FieldLen = 200, SortNum = 10, DBDefault = HiTypeDBDefault.EMPTY)]
        public string NewLogFile { get; set; }

        [HiColumn(FieldDesc = "新值长度",  FieldType = HiType.BIGINT, SortNum = 11, DBDefault = HiTypeDBDefault.EMPTY)]
        public Int64 NewLen { get; set; }
    }
}
