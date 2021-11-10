using System;
namespace HiSql
{
    public enum LogMode
    {
        /// <summary>
        /// 不记录
        /// </summary>
        NONE=0,
        /// <summary>
        /// 全记录
        /// </summary>
        ALL=1,
        /// <summary>
        /// 只记录变更
        /// </summary>
        CHANG=0
    }
}
