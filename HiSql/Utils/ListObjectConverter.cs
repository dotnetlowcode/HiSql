using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HiSql.Utils
{
    public static class ListObjectConverter
    {


        public static List<Dictionary<string, object>> ConvertToListOfDictionary(List<object> inputList)
        {
            List<Dictionary<string, object>> resultList = new List<Dictionary<string, object>>();

            if (inputList == null)
            {
                return resultList; // 返回空列表
            }

            foreach (object item in inputList)
            {
                if (item is Dictionary<string, object> dictObject)
                {
                    resultList.Add(dictObject);
                }
                else if (item != null) // 处理匿名类、实体模型、dynamic 和 ExpandoObject
                {
                    Dictionary<string, object> objectDict = ConvertObjectToDictionary(item);
                    if (objectDict != null)
                    {
                        resultList.Add(objectDict);
                    }
                }
            }

            return resultList;
        }

        public static List<Dictionary<string, object>> ConvertToListOfDictionary(List<ExpandoObject> obj)
        {
            return obj.Select(r => ConvertObjectToDictionary(r)).ToList();
        }


        private static Dictionary<string, object> ConvertObjectToDictionary(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            Dictionary<string, object> dict = new Dictionary<string, object>();
            if (obj is Dictionary<string, string> stringDict)
            {
                Dictionary<string, object> convertedDict = new Dictionary<string, object>();
                foreach (var kvp in stringDict)
                {
                    convertedDict[kvp.Key] = kvp.Value;
                }
                return convertedDict;
            }
            else if (obj is ExpandoObject expando)
            {
                var dicObj = expando as IDictionary<string, object>;
                return dicObj.ToDictionary(r => r.Key, r => r.Value);
            }
            else if (obj is TDynamic dyn)
            {
                var dicObj = dyn.ToExpandoObject() as IDictionary<string, object>;
                return dicObj.ToDictionary(r => r.Key, r => r.Value);
            }
            else
            {
                Type type = obj.GetType();
                PropertyInfo[] properties = type.GetProperties();

                foreach (PropertyInfo property in properties)
                {
                    try
                    {
                        dict[property.Name] = property.GetValue(obj);
                    }
                    catch (Exception)
                    {
                        // 如果获取属性值时发生异常，则跳过该属性
                    }
                }
            }

            return dict;
        }



        public static List<Dictionary<string, string>> ToDeleteWhere(List<Dictionary<string, object>> list)
        {

            List<Dictionary<string, string>> outputList = new List<Dictionary<string, string>>();

            foreach (var inputDictionary in list)
            {
                Dictionary<string, string> outputDictionary = new Dictionary<string, string>();

                foreach (var kvp in inputDictionary)
                {
                    // 尝试将 object 值转换为 string
                    string stringValue = kvp.Value?.ToString(); // 使用 ?. 安全地处理 null

                    outputDictionary[kvp.Key] = stringValue;
                }

                outputList.Add(outputDictionary);
            }

            return outputList;
        }

    }
}
