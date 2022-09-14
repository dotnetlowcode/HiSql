using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql 
{
    /// <summary>
    /// 操作类型
    /// </summary>
    public enum OperType
    {
        /// <summary>
        /// 等于
        /// </summary>
        EQ=0,//等于
        /// <summary>
        /// 大于
        /// </summary>
        GT=1,//大于
        /// <summary>
        /// 小于
        /// </summary>
        LT=2,//小于
        
        /// <summary>
        /// 大于等于
        /// </summary>
        GE=3,//大于等于
        /// <summary>
        /// 小于等于
        /// </summary>
        LE=4,//小于等于

        /// <summary>
        /// 不等于
        /// </summary>
        NE = 5,//

        /// <summary>
        /// 模糊查询
        /// </summary>
        LIKE=6,//模糊查询 

        /// <summary>
        /// 不匹配模糊查询
        /// </summary>
        NOLIKE=7,//否定模糊查询


        /// <summary>
        /// 范围值
        /// </summary>
        BETWEEN=20,//范围
        /// <summary>
        /// 包含
        /// </summary>
        IN=30,//包含
        /// <summary>
        /// 不包含
        /// </summary>
        NOIN=31,//不包含

        //JOIN=40,//关于

    }
    public static class OperTypeExt
    {
        private static Dictionary<string, OperType> TextMapOperType = new Dictionary<string, OperType>(StringComparer.OrdinalIgnoreCase);
        private static Dictionary<OperType, string> OperTypeMapText = new Dictionary<OperType, string>();
        static OperTypeExt()
        {
            TextMapOperType.Add("not like", OperType.NOLIKE);
            TextMapOperType.Add("like", OperType.LIKE);
            TextMapOperType.Add("BETWEEN", OperType.BETWEEN);
            TextMapOperType.Add("not in", OperType.NOIN);
            TextMapOperType.Add("in", OperType.IN);
            TextMapOperType.Add("<=", OperType.LE);
            TextMapOperType.Add(">=", OperType.GE);
            TextMapOperType.Add("<", OperType.LT);
            TextMapOperType.Add(">", OperType.GT);
            TextMapOperType.Add("<>", OperType.NE);
            TextMapOperType.Add("=", OperType.EQ);

            foreach (string key in TextMapOperType.Keys)
            {
                OperTypeMapText.Add(TextMapOperType[key], key);
            }
        }
        public static string GetOperTypeText(this OperType operType)
        {
            if (!OperTypeMapText.ContainsKey(operType))
            {
                throw new HiSqlException($"类型【{operType.ToString()}】错误");
            }
            return OperTypeMapText[operType];
        }
        public static OperType GetOperType(this string operTypeText)
        {
            if (!TextMapOperType.ContainsKey(operTypeText))
            {
                throw new HiSqlException($"类型【{operTypeText.ToString()}】错误");
            }
            return TextMapOperType[operTypeText];
        }
    }
}
