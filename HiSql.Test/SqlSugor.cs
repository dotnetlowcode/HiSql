using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HiSql.Test
{
    // <summary>
    /// lambda表达式转为where条件sql
    /// </summary>
    public class SqlSugor
    {
        #region Expression 转成 where
        /// <summary>
        /// Expression 转成 Where String
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="databaseType">数据类型（用于字段是否加引号）</param>
        /// <returns></returns>
        public static string GetWhereByLambda<T>(Expression<Func<T, bool>> predicate, string databaseType)
        {
            bool withQuotationMarks = GetWithQuotationMarks(databaseType);

            ConditionBuilder conditionBuilder = new ConditionBuilder();
            conditionBuilder.SetIfWithQuotationMarks(withQuotationMarks); //字段是否加引号（PostGreSql,Oracle）
            conditionBuilder.SetDataBaseType(databaseType);
            conditionBuilder.Build(predicate);

            for (int i = 0; i < conditionBuilder.Arguments.Length; i++)
            {
                object ce = conditionBuilder.Arguments[i];
                if (ce == null)
                {
                    conditionBuilder.Arguments[i] = DBNull.Value;
                }
                else if (ce is string || ce is char)
                {
                    if (ce.ToString().ToLower().Trim().IndexOf(@"in(") == 0 ||
                        ce.ToString().ToLower().Trim().IndexOf(@"not in(") == 0 ||
                         ce.ToString().ToLower().Trim().IndexOf(@" like '") == 0 ||
                        ce.ToString().ToLower().Trim().IndexOf(@"not like") == 0)
                    {
                        conditionBuilder.Arguments[i] = string.Format(" {0} ", ce.ToString());
                    }
                    else
                    {


                        //****************************************
                        conditionBuilder.Arguments[i] = string.Format("'{0}'", ce.ToString());
                    }
                }
                else if (ce is DateTime)
                {
                    conditionBuilder.Arguments[i] = string.Format("'{0}'", ce.ToString());
                }
                else if (ce is int || ce is long || ce is short || ce is decimal || ce is double || ce is float || ce is bool || ce is byte || ce is sbyte)
                {
                    conditionBuilder.Arguments[i] = ce.ToString();
                }
                else if (ce is ValueType)
                {
                    conditionBuilder.Arguments[i] = ce.ToString();
                }
                else
                {

                    conditionBuilder.Arguments[i] = string.Format("'{0}'", ce.ToString());
                }

            }
            string strWhere = string.Format(conditionBuilder.Condition, conditionBuilder.Arguments);
            return strWhere;
        }

        /// <summary>
        /// 获取是否字段加双引号
        /// </summary>
        /// <param name="databaseType"></param>
        /// <returns></returns>
        public static bool GetWithQuotationMarks(string databaseType)
        {
            bool result = false;
            switch (databaseType.ToLower())
            {

                case DataBaseType.PostGreSql:
                case DataBaseType.Oracle:

                    result = true;
                    break;
            }

            return result;


        }


        #endregion
    }
}
