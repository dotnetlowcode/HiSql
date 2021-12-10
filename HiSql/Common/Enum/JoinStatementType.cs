using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public enum JoinStatementType
    {
        /// <summary>
        /// 识别运算符
        /// </summary>
        Symbol=1,
        /// <summary>
        /// join关联条件 on
        /// </summary>
        FieldValue=2,
        /// <summary>
        /// 关联条件分组()
        /// </summary>
        SubCondition=3,
    }
}
