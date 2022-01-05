using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// where 语句组成类型
    /// </summary>
    public enum StatementType
    {
        /// <summary>
        /// 字段值如 username='hisql'
        /// </summary>
        FieldValue=0,

        /// <summary>
        /// 如  (usertype='a' or usertype='b')
        /// </summary>
        SubCondition = 1,
        /// <summary>
        /// 如 username in ('hisql','hi')
        /// </summary>
        In=2,

        /// <summary>
        /// 如 usertype in (select utype from usertype)
        /// </summary>
        //InSelect=4,

        /// <summary>
        /// 如 userage between 20 and 30 
        /// </summary>
        FieldBetweenValue=5,


        /// <summary>
        /// 识别运算符 and | or
        /// </summary>
        Symbol = 6,

    }
}
