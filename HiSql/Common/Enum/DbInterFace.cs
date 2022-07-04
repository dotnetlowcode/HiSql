using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{

    public enum DbInterFace
    {
        /// <summary>
        /// 数据库基础操作接口
        /// </summary>
        DM=1,
        /// <summary>
        /// 数据插入实现接口
        /// </summary>
        Insert=2,

        /// <summary>
        /// 数据查询实现接口
        /// </summary>
        Query=4,

        /// <summary>
        /// 数据库ADO连接实现接口
        /// </summary>
        Provider=8,

        /// <summary>
        /// 数据更新接口
        /// </summary>
        Update=16,


        /// <summary>
        /// 数据删除接口
        /// </summary>
        Delete=32,

        /// <summary>
        /// 数据配置
        /// </summary>
        Config=64,
    }
}
