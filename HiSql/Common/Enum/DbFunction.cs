using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public enum DbFunction
    {
        NONE = -1,//无定义
        COUNT = 1,//求记录数Count(1*)
        SUM = 2,//求和
        AVG = 3,//求平均
        MAX = 4,//求最大值
        MIN = 5//求最小值
    }
    public static class DbFunctionExt
      {
        public static string GetDbFunctionName(this DbFunction value)
        {
            if (value == DbFunction.NONE)
            {
                return "";
            }
            return value.ToString();

            //var type = value.GetType();
            //var name = Enum.GetName(type, value);

            //if (name == null) return null;

            //var field = type.GetField(name);

            //if (!(Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute))
            //{
            //    return name;
            //}

            //return attribute.Description;
        }

        /// <summary>
        /// 验证 使用函数的参数是否正确
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fieldName"></param>
        /// <exception cref="Exception"></exception>
        public static void VerifyDbFunction(this DbFunction value, string fieldName)
        {
            if (string.Equals(fieldName.Trim(),"*") && value != DbFunction.COUNT)
            {
                throw new Exception($"仅Count函数支持*");
            }

            if (fieldName.IndexOf(",") > -1)
            {
                throw new Exception($"函数只支持*或指定字段名称");
            }
        }
    }
    
}
