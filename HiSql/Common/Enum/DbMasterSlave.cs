using System;
namespace HiSql
{
    /// <summary>
    /// 主从策略选择（仅对查询有效）
    /// </summary>
    public enum DbMasterSlave
    {
        /// <summary>
        /// 默认的主从规则
        /// </summary>
        Default=0,
        /// <summary>
        /// 强制使用主库（当有从库的情况下）
        /// </summary>
        Master=1
    }
}
