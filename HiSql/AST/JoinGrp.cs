using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql.AST
{
    public class JoinGrp
    {
        /// <summary>
        /// 语句类型
        /// </summary>
        public JoinStatementType JType { get; set; }

        /// <summary>
        /// 正则表达式
        /// </summary>
        public string Reg { get; set; }
    }
}
