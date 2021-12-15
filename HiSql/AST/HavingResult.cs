using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql.AST
{
    public class HavingResult
    {
        /// <summary>
        /// 当前语句类型
        /// </summary>
        public StatementType SType { get; set; }

        /// <summary>
        /// 当前语句段
        /// </summary>
        public string Statement { get; set; }

        /// <summary>
        /// 当前语句分析结果
        /// </summary>
        public Dictionary<string, string> Result { get; set; }
    }
}
