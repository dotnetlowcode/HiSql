using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// 扩展属性及方法
    /// </summary>
    public static class AdoExtensions
    {
        /// <summary>
        /// 将值转换成SQL的IN语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static string ToSqlIn<T>(this T[] array, bool ischar = true)
        {
            if (array == null || array.Length == 0)
            {
                return ToSqlValue(string.Empty);
            }
            else
            {
                return string.Join(",", array.Where(c => c != null).Select(it => (it + "").ToSqlValue(ischar)));
            }
        }

        public static string ToSqlValue(this string value,bool ischar=true)
        {
            //按理需要对值的长度进行检测 ，或默认大小写
            if(ischar)
                return string.Format("'{0}'", value.ToSqlInject());
            else
                return string.Format("{0}", value.ToSqlInject());
        }

        /// <summary>
        /// 中文字符按两个符计算
        /// nvarchar 类型中文也按一个字符计算
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int LengthZH(this string value)
        {

            int _gbklen = 2;
            int _enlen = 1;
            if (value.Length == 0)
                return 0;
            ASCIIEncoding ascii = new ASCIIEncoding();
            int tempLen = 0;
            byte[] s = ascii.GetBytes(value);
            for (int i = 0; i < s.Length; i++)
            {
                if ((int)s[i] == 63)
                {
                    tempLen += _gbklen;
                }
                else
                {
                    tempLen += _enlen;
                }
            }
            return tempLen;
        }


        /// <summary>
        ///防Sql 注入
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToSqlInject(this string value)
        {
            if (!value.IsNullOrEmpty())
            {
                value = value.Replace("'", "''").ToSqlDeChar();
            }
            return value;
        }

        /// <summary>
        /// 将单引号转成 ``
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToSqlEnChar(this string value)
        {
            if (!value.IsNullOrEmpty())
            {
                value = value.Replace("''", "``");
            }
            return value;
        }

        /// <summary>
        /// 将 `` 转成 ''
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToSqlDeChar(this string value)
        {
            if (!value.IsNullOrEmpty())
            {
                value = value.Replace("``", "''");
            }
            return value;
        }

        public static string ToFieldString(this List<string> thisValue)
        {
            StringBuilder _sb_temp = new StringBuilder();
            string _temp = "'[$FIELD$]'";
            for (int i = 0; i < thisValue.Count; i++)
            {
                if (i == thisValue.Count - 1)
                {
                    _sb_temp.Append($"{_temp.Replace("[$FIELD$]", thisValue[i]).ToSqlInject()}");
                }
                else
                    _sb_temp.Append($"{_temp.Replace("[$FIELD$]", thisValue[i]).ToSqlInject()},");
            }

            return _sb_temp.ToString();
        }
        public static HiType ToHiType(this Type value)
        {
            if (value == Constants.StringType)
            {
                return HiType.NVARCHAR;
                
            }
            else if (value == Constants.IntType)
            {
                return HiType.INT;
            }
            else if (value == Constants.LongType)
            {
                return HiType.BIGINT;
            }
            else if (value == Constants.ShortType)
            {
                return HiType.SMALLINT;
            }
            else if (value == Constants.DecType)
            {
                return HiType.DECIMAL;
            }
            else if (value == Constants.BoolType)
            {
                return HiType.BOOL;
            }
            else if (value == Constants.DateType)
            {
                return HiType.DATETIME;
            }
            else if (value == Constants.DateTimeOffsetType)
            {
                return HiType.DATETIME;
            }
            else if (value == Constants.ByteArrayType)
            {
                return HiType.BINARY;
            }
            else if (value == Constants.GuidType)
            {
                return HiType.GUID;
            }
            else
            {
                //无法识别的统一
                return HiType.NVARCHAR;
                 
            }
        }
        internal static string ToLower(this string value, bool isAutoToLower)
        {
            if (value == null) return null;
            if (isAutoToLower == false) return value;
            return value.ToLower();
        }
    }
}
