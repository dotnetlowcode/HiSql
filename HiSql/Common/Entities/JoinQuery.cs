using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// sql join 查询结构
    /// 做表连接
    /// </summary>
    public class JoinQuery
    {
        /// <summary>
        /// 关联类型
        /// </summary>
        public JoinType JoinType { get; set; }

        //表的类型（实体表或临时表）
        public TableType TableType { get; set; }

        /// <summary>
        /// 真实的表
        /// </summary>
        public string TabName { get; set; }

        /// <summary>
        /// 重命名的表
        /// </summary>
        public string AsName { get; set; }

        //关联表的顺利
        public int JoinIndex { get; set; }


        public string JoinWhere { get; set; }

        public Expression JoinExpression { get; set; }
    }
}
