using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// 表的分类
    /// </summary>
    public enum TableType
    {
        /// <summary>
        /// 实体表
        /// </summary>
        Entity = 0,

        //202202新增
        /// <summary>
        /// 视图
        /// </summary>
        View=20,

        //202202新增
        /// <summary>
        /// 存储过程
        /// </summary>
        //Procdure=21,

        /// <summary>
        /// 本地临时表
        /// </summary>
        Local = 1,

        /// <summary>
        /// 全局临时表
        /// </summary>
        Global=2,

        /// <summary>
        /// 表变量
        /// </summary>
        Var=3,
    }
}
