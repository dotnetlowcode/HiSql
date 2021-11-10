using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql 
{
    /// <summary>
    /// 操作类型
    /// </summary>
    public enum OperType
    {
        /// <summary>
        /// 等于
        /// </summary>
        EQ=0,//等于
        /// <summary>
        /// 大于
        /// </summary>
        GT=1,//大于
        /// <summary>
        /// 小于
        /// </summary>
        LT=2,//小于
        
        /// <summary>
        /// 大于等于
        /// </summary>
        GE=3,//大于等于
        /// <summary>
        /// 小于等于
        /// </summary>
        LE=4,//小于等于


        NE=5,//不等于

        /// <summary>
        /// 模糊查询
        /// </summary>
        LIKE=5,//模糊查询 还未实现

        BETWEEN=20,//范围
        IN=30,//包含
        NOIN=31,//不包含

        //JOIN=40,//关于

    }
}
