using System.Collections.Generic;

namespace HiSql.Common.Entities.TabLog
{

    public enum OperationType
    {
        Insert,
        Update,
        Delete
    }

    public class OperationLog
    {
        public List<IDictionary<string, object>> OldValue { get; set; }
        public List<IDictionary<string, object>> NewValue { get; set; }
        public OperationType OperationType { get; set; }

        /// <summary>
        /// 当前操作记录关联的表名
        /// </summary>
        public string TableName { get; set; }
    }
}
