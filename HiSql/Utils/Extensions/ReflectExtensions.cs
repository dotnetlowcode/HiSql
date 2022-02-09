using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// 反射类的扩展
    /// </summary>
    public static class ReflectExtensions
    {
        public static Type GetTypeInfo(this Type typeInfo)
        {
            return typeInfo;
        }
        public static bool IsEnum(this Type type)
        {
            var reval = type.GetTypeInfo().IsEnum;
            return reval;
        }

        public static bool IsNullOrEmpty(this IEnumerable<object> thisValue)
        {
            if (thisValue == null || thisValue.Count() == 0) return true;
            return false;
        }
        public static bool IsNullOrEmpty(this object thisValue)
        {
            if (thisValue == null || thisValue == DBNull.Value) return true;
            return thisValue.ToString() == "";
        }
        public static bool HasValue(this IEnumerable<object> thisValue)
        {
            if (thisValue == null || thisValue.Count() == 0) return false;
            return true;
        }
        public static bool IsIn<T>(this T thisValue, params T[] values)
        {
            return values.Contains(thisValue);
        }

        /// <summary>
        /// not in
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="thisValue"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool NotIn<T>(this T thisValue, params T[] values)
        {
            return !values.Contains(thisValue);
        }

        /// <summary>
        /// 是否是字符串类型的字段
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsCharField(this HiType thisValue)
        {
            return thisValue.IsIn<HiType>(HiType.CHAR,HiType.VARCHAR,HiType.NCHAR,HiType.NVARCHAR,HiType.TEXT,HiType.GUID);
        }

        /// <summary>
        /// 是否是数值型的字段
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsNumberField(this HiType thisValue)
        {
            return thisValue.IsIn<HiType>(HiType.INT, HiType.DECIMAL);
        }

        public static bool IsDateField(this HiType thisValue)
        {
            return thisValue.IsIn<HiType>(HiType.DATE,HiType.DATETIME);
        }
        public static bool IsBoolField(this HiType thisValue)
        {
            return thisValue.IsIn<HiType>(HiType.BOOL);
        }

        /// <summary>
        /// 将字符串转成md5
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static string ToMd5(this string thisValue)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string t2 = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(thisValue)), 4, 8);
            t2 = t2.Replace("-", "");
            return t2;
        }

        /// <summary>
        /// 将字符串生成hash
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static string ToHash(this string thisValue)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(thisValue));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        public static string ToStringEx(this string thisValue)
        {
            if (thisValue.IsNullOrEmpty())
                return "";
            else
                return thisValue;
        }
        public  static string ToString(this  object thisValue)
        {
            return thisValue.ToString();
        }


        /// <summary>
        /// 字符串大小匹配
        /// -1是小于 0是等于 1是大于
        /// </summary>
        /// <param name="thisValue"></param>
        /// <param name="value"></param>
        /// <param name="ignore"></param>
        /// <returns></returns>
        public static int Compare(this string thisValue,string value, bool ignore = true)
        {
            int flag = 0;
            if (thisValue.Length > value.Length)
            {
                flag = 1;
            }
            else if (thisValue.Length < value.Length)
            {
                flag = -1;
            }
            else // 相等
            {
                if (ignore)
                {
                    thisValue = thisValue.ToUpper();
                    value = value.ToUpper();
                }
                int len = thisValue.Length;
                for (int i = 0; i < thisValue.Length; i++)
                {
                    if ((int)thisValue[i] > (int)value[i])
                    {
                        flag = 1;
                        break;
                    }
                    else if ((int)thisValue[i] < (int)value[i])
                    {
                        flag = -1;
                        break;
                    }
                }
            }
            return flag;
        }

        /// <summary>
        /// 判断当前字符是否是数字
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsInt(this string thisValue)
        {
            return Tool.RegexMatch(@"^\d+$", thisValue);
        }


        /// <summary>
        /// 是否动态类对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsDynamic<T>(this T thisValue)
        {
            var type = typeof(T);
            return type.FullName == "Sytem.Object" && type.IsClass == true;
        }

        public static string ObjToString(this object thisValue)
        {
            if (thisValue != null) return thisValue.ToString().Trim();
            return "";
        }

        

        /// <summary>
        /// 判断是否集合
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsList(this string thisValue)
        {
            return (thisValue + "").StartsWith("System.Collections.Generic.List") || (thisValue + "").StartsWith("System.Collections.Generic.IEnumerable");
        }

        public static HiType ToDbFieldType(this Type thisValue)
        {

            if (thisValue == typeof(string))
            {
                return HiType.NVARCHAR;
            }
            else if (thisValue == typeof(int))
            {
                return HiType.INT;
            }
            else if (thisValue == typeof(Int64))
            {
                return HiType.BIGINT;
            }
            else if (thisValue == typeof(decimal))
            {
                return HiType.DECIMAL;
            }
            else if (thisValue == typeof(DateTime) || thisValue == typeof(DateTimeOffset))
            {
                return HiType.DATETIME;
            }
            else if (thisValue == typeof(bool))
            {
                return HiType.BOOL;
            }
            else if (thisValue == typeof(byte[]) && thisValue == typeof(byte))
            {
                return HiType.BINARY;
            }
            else
                return HiType.NONE;
            //text 的无法默认转换

            
        }

        
    }
}
