using System;
namespace HiSql
{
    /// <summary>
    /// 锁模式
    /// </summary>
    public enum LockMode
    {
        /// <summary>
        /// 不设置
        /// </summary>
        NONE=0,
        /// <summary>
        /// 不加锁（允许脏读）
        /// </summary>
        NOLOCK=1,

        /// <summary>
        /// 持续锁
        /// </summary>
        HOLDLOCK=2,

        /// <summary>
        /// 使用行锁
        /// </summary>
        ROWLOCK=3,


        /// <summary>
        /// 表锁
        /// </summary>
        TABLOCK=4,

        /// <summary>
        /// 独占表锁
        /// </summary>
        TABLOCKX=5,

        /// <summary>
        /// 页锁
        /// </summary>
        PAGLOCK=6,

        /// <summary>
        /// 更新锁
        /// </summary>
        UPLOCK=7,

        /// <summary>
        /// 跳过任何锁定的行
        /// </summary>
        READPAST=8


    }
}
