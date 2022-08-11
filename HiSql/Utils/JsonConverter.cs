using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HiSql
{
    public static class JsonConverter
    {
        private static string strTimeFormat = "yyyy-MM-dd HH:mm:ss";
        public static string strTimeFormat2 = "yyyy/MM/dd HH:mm:ss";

        public static string ToJson(this object obj)
        {
            //Newtonsoft.Json序列化 对时间格式化处理
            var timeConverter = new IsoDateTimeConverter { DateTimeFormat = strTimeFormat };
            JsonSerializerSettings settings = new JsonSerializerSettings();
            //settings.MissingMemberHandling = MissingMemberHandling.Ignore;
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            settings.Formatting = Formatting.Indented;
            settings.Converters.Add(timeConverter);
            return ReplaceDatetimeString(JsonConvert.SerializeObject(obj, settings)).Replace("\r\n", "");
        }
        public static string ToJson(this object obj, string datetimeformats)
        {
            //Newtonsoft.Json序列化 对时间格式化处理
            var timeConverter = new IsoDateTimeConverter { DateTimeFormat = datetimeformats };
            JsonSerializerSettings settings = new JsonSerializerSettings();
            //settings.MissingMemberHandling = MissingMemberHandling.Ignore;
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            settings.Formatting = Formatting.Indented;
            settings.Converters.Add(timeConverter);
            return ReplaceDatetimeString(JsonConvert.SerializeObject(obj, settings)).Replace("\r\n", "");
        }

        // Methods
        /// <summary>
        /// 将JSON字符串反序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sJsonData"></param>
        /// <returns></returns>
        public static T FromJson<T>(string sJsonData)
        {
            return JsonConvert.DeserializeObject<T>(sJsonData);
        }
        /// <summary>
        /// 日期格式化字符串
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private static string GetDatetimeString(Match m)
        {
            string result = string.Empty;
            DateTime dateTime = new DateTime(1970, 1, 1);
            dateTime = dateTime.AddMilliseconds(long.Parse(m.Groups[1].Value));
            dateTime = dateTime.ToLocalTime();
            result = dateTime.ToString(strTimeFormat);
            return result;
        }
        /// <summary>
        /// 将json字符串中json日期字符串转为正常日期字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ReplaceDatetimeString(string str)
        {
            string pattern = @"\/Date\((\d+)\)\/";
            MatchEvaluator evaluator = new MatchEvaluator(GetDatetimeString);
            Regex regex = new Regex(pattern);
            return regex.Replace(str, evaluator);
        }
    }
}
