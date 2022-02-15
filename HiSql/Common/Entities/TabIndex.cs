using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public class TabIndex
    {
        /// <summary>
        /// 表名称
        /// </summary>
        public string TabName { get; set; }


        /// <summary>
        /// 索引名称
        /// </summary>
        public string IndexName { get; set; }

        
        /// <summary>
        /// 索引类型
        /// Index,Key_Index 两种类型
        /// </summary>
        public string IndexType { get; set; }

    }
}
