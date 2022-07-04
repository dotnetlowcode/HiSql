using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{

    /// <summary>
    /// 数据库字段默认值
    /// </summary>
    public enum HiTypeDBDefault
    { 
        NONE=-1,//无默认值
        EMPTY=10,//字符串空值

        VALUE=11,//指定值
        FUNDATE=20,//日期
        FUNGUID=30 //GUID

    }

    /// <summary>
    /// 将字段类型进行分类
    /// </summary>
    public enum HiTypeGroup
    { 
        None=0,
        Char=1,
        Number=2,
        Date=3,
        Bool=4
    }

    /// <summary>
    /// HiSql字段类型
    /// </summary>
    public enum HiType
    {
        //表示未指定过类型应当报错
        NONE = -1,

        /// <summary>
        /// 用于可以存储中文的可变字符串 最大长度为4000
        /// </summary>
        /// 
        NVARCHAR = 11,

        /// <summary>
        /// 用于可变字符串长度
        /// </summary>
        VARCHAR = 12,

        /// <summary>
        /// 英文变长字符串
        /// </summary>
        NCHAR = 13,

        /// <summary>
        /// 英文定长字符串
        /// </summary>
        CHAR = 14,

        //超大文本
        TEXT = 15,

        /// <summary>
        /// 整型数字
        /// </summary>
        INT = 21,

        /// <summary>
        /// 超大整型
        /// </summary>
        BIGINT = 22,

        /// <summary>
        /// 短整型
        /// </summary>
        SMALLINT = 23,

        /// <summary>
        /// 带小数点的数值
        /// </summary>
        DECIMAL = 24,

        /// <summary>
        /// true/false   1/0
        /// </summary>
        BOOL = 31,

        /// <summary>
        /// 日期格式
        /// </summary>
        DATETIME = 41,

        /// <summary>
        /// 日期(不带时间)
        /// </summary>
        DATE = 42,

        /// <summary>
        /// 二进制数据
        /// </summary>
        BINARY = 51,


        /// <summary>
        /// MD5 GUID
        /// </summary>
        GUID=61,



    }
}
