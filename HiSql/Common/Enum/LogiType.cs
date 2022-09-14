using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// 逻辑运算类型
    /// </summary>
    public enum LogiType
    {
        /// <summary>
        /// 且 左右条件都要满足
        /// </summary>
        AND = 0,//且
        /// <summary>
        /// 或 左右条件有一个满足即可
        /// </summary>
        OR = 1,//或
    }

    public static class LogiTypeExt
    {
        private static Dictionary<string, LogiType> TextMapLogiType = new Dictionary<string, LogiType>(StringComparer.OrdinalIgnoreCase);
        private static Dictionary<LogiType, string> LogiTypeMapText = new Dictionary<LogiType, string>();
        static LogiTypeExt()
        {
            TextMapLogiType.Add("and", LogiType.AND);
            TextMapLogiType.Add("or", LogiType.OR);
        }
        public static string GetLogiTypeText(this LogiType logiType)
        {
            if (!LogiTypeMapText.ContainsKey(logiType))
            {
                throw new HiSqlException($"逻辑类型【{logiType.ToString()}】错误");
            }
            return LogiTypeMapText[logiType];
        }
        public static LogiType GetLogiType(this string logiTypeText)
        {
            if (!TextMapLogiType.ContainsKey(logiTypeText))
            {
                throw new HiSqlException($"逻辑类型【{logiTypeText.ToString()}】错误");
            }
            return TextMapLogiType[logiTypeText];
        }
    }
}
