using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql.AST
{
    /// <summary>
    /// where 语句按类型分组
    /// add by tgm date:20210.12.2
    /// </summary>
    public class WhereGrp
    {
        /// <summary>
        /// 语句类型
        /// </summary>
        public StatementType SType { get; set; }

        /// <summary>
        /// 正则表达式
        /// </summary>
        public string Reg { get; set; }
    }
}
