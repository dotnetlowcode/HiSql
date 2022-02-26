using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public class TabIndexDetail:TabIndex
    {
        /// <summary>
        /// 列名
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 索引中的字段位置
        /// </summary>
        public int ColumnIdx { get; set; }

        /// <summary>
        /// 列是表中的位置 
        /// </summary>
        public int ColumnId { get; set; }

        //排序类型
        public SortType SortType
        {
            get;set;
        }

        /// <summary>
        /// 是否主键
        /// </summary>
        public bool IsPrimary
        {
            get;set;
        }
        
        /// <summary>
        /// 是否唯一值
        /// </summary>
        public bool IsUnique
        {
            get;set;
        }

    }
}
