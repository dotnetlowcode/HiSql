
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace HiSql
{
    public class Tool
    {

        public static class Net
        {
            /// <summary>
            /// 获取本机IP
            /// </summary>
            /// <returns></returns>
            public static string GetLocalIPAddress()
            {
                string strNativeIP = "";
                string strServerIP = "";
                try
                {
                    System.Net.IPAddress[] addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;

                    if (addressList.Length > 1)
                    {
                        strNativeIP = addressList[0].ToString();
                        strServerIP = addressList[1].ToString();
                    }
                    else if (addressList.Length == 1)
                    {
                        strServerIP = addressList[0].ToString();
                    }

                    bool isRunningInContainer = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"));
                    if (isRunningInContainer)
                    {
                        strServerIP = strServerIP + GetContainerName();
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        // 获取可用网卡
                        var nics = NetworkInterface.GetAllNetworkInterfaces()?.Where(network => network.OperationalStatus == OperationalStatus.Up);

                        // 获取所有可用网卡IP信息
                        var ipCollection = nics?.Select(x => x.GetIPProperties())?.SelectMany(x => x.UnicastAddresses);

                        foreach (var ipadd in ipCollection)
                        {
                            if (!IPAddress.IsLoopback(ipadd.Address) && ipadd.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                strServerIP = ipadd.Address.ToString();
                                break;
                            }
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                    //strServerIP = Environment.MachineName;
                }

               
                return strServerIP;
            }

            public static string GetMachineName()
            {
                string MachineName = "";
                try
                {
                    MachineName = Environment.MachineName;
                    bool isRunningInContainer = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"));
                    if (isRunningInContainer)
                    {
                        MachineName = MachineName + GetContainerName();
                    }

                    return MachineName;
                }
                catch (Exception)
                {

                }

                return MachineName;
            }
            private static string GetContainerName()
            {
                try
                {
                    string containerIdFile = "/proc/1/cpuset";
                    string containerId = System.IO.File.ReadAllText(containerIdFile).Trim();
                    string containerName = containerId.Substring(containerId.LastIndexOf('/') + 1);
                    if (containerName.IsNullOrEmpty() == false)
                    {
                        containerName = containerName.Length > 20 ? containerName.Substring(0, 12) : containerName;
                    }
                    return "_" + containerName;
                }
                catch (Exception)
                {
                    return "_null";
                }
            }
        }

        

        /// <summary>
        /// 正则表达式获取匹配内容
        /// \"(?<grp>[\s\S]*)\"
        /// </summary>
        /// <param name="regex"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Dictionary<string, string> RegexGrp(string regex, string text)
        {
            Regex _regex = new Regex(regex, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Dictionary<string, string> _dic = new Dictionary<string, string>();
            Match _match = _regex.Match(text);
            while (_match.Success)
            {
                foreach (string name in _regex.GetGroupNames())
                {
                    if (!_dic.ContainsKey(name))
                        _dic.Add(name, _match.Groups[_regex.GroupNumberFromName(name)].Value);
                }
                _match = _match.NextMatch();
            }
            return _dic;
        }
        /// <summary>
        /// 匹配值且进行替换
        /// </summary>
        /// <param name="regex"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Tuple<bool, Dictionary<string, string>, string> RegexGrpOrReplace(string regex, string text)
        {
            Tuple<bool, Dictionary<string, string>, string> tuple = new Tuple<bool, Dictionary<string, string>, string>(false, null, "");
            Regex _regex = new Regex(regex, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Dictionary<string, string> _dic = new Dictionary<string, string>();

            //bool _ismach = _regex.IsMatch(text);
            Match _match = _regex.Match(text);

            if (_match.Success)
            {
                foreach (string name in _regex.GetGroupNames())
                {
                    _dic.Add(name, _match.Groups[_regex.GroupNumberFromName(name)].Value);
                }
                text = _regex.Replace(text, "");
                //_match = _match.NextMatch();
            }
            return new Tuple<bool, Dictionary<string, string>, string>(_match.Success, _dic, text);
        }

        /// <summary>
        /// 正则替换
        /// </summary>
        /// <param name="regex">正则表达式</param>
        /// <param name="text">文本</param>
        /// <param name="repstr">替换值</param>
        /// <returns></returns>
        public static string RegexReplace(string regex, string text, string repstr)
        {
            Regex _regex = new Regex(regex, RegexOptions.IgnoreCase | RegexOptions.Multiline);

            return _regex.Replace(text, repstr);
        }

        /// <summary>
        /// 获取多个组合匹配
        /// </summary>
        /// <param name="_regex"></param>
        /// <param name="_text"></param>
        /// <returns></returns>
        public static List<Dictionary<string, string>> RegexGrps(string _regex, string _text)
        {
            List<Dictionary<string, string>> _lstresult = new List<Dictionary<string, string>>();

            Regex regex = new Regex(_regex.Trim(), RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Match _match = regex.Match(_text);
            while (_match.Success)
            {
                Dictionary<string, string> _dic = new Dictionary<string, string>();
                foreach (string name in regex.GetGroupNames())
                {
                    _dic.Add(name, _match.Groups[regex.GroupNumberFromName(name)].Value);
                }
                _lstresult.Add(_dic);

                _match = _match.NextMatch();
            }
            return _lstresult;
        }


        /// <summary>
        /// 批量匹配结果并返回指定的值
        /// </summary>
        /// <param name="_regex"></param>
        /// <param name="_text"></param>
        /// <param name="grpname"></param>
        /// <returns></returns>
        public static List<string> RegexGrps(string _regex, string _text, string grpname)
        {
            List<string> lst = new List<string>();

            Regex regex = new Regex(_regex.Trim(), RegexOptions.IgnoreCase | RegexOptions.Multiline);

            MatchCollection matches = regex.Matches(_text);
            for (int i = 0; i < matches.Count; i++)
            {
                lst.Add(matches[i].Groups[grpname].Value);
            }


            return lst;
        }

        /// <summary>
        /// (?<name>[\w]+[.]{1}[\[]{1}[\w]+[\]]{1})
        /// </summary>
        /// <param name="regex"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool RegexMatch(string regex, string text)
        {
            Regex _regex = new Regex(regex, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Match _match = _regex.Match(text);
            return _match.Success;
        }

        /// <summary>
        /// 是否数值型 如0 或0.44
        /// </summary>
        /// <param name="text"></param>
        /// <param name="isunsigned">是否无符号</param>
        /// <returns></returns>
        public static bool IsDecimal(string text, bool isunsigned = false)
        {
            if (!isunsigned)
                return new Regex(@"^[-]?\d+(?:[\.]?)[\d]*$", RegexOptions.IgnoreCase | RegexOptions.Multiline).Match(text).Success;
            else
                return new Regex(@"^\d+(?:[\.]?)[\d]*$", RegexOptions.IgnoreCase | RegexOptions.Multiline).Match(text).Success;
        }
        /// <summary>
        /// char中是否是数值
        /// </summary>
        /// <param name="text"></param>
        /// <param name="isunsigned"></param>
        /// <returns></returns>
        public static bool CharIsDecimal(string text, bool isunsigned = false)
        {
            if (!isunsigned)
                return new Regex(@"^[\']{1}(?<num>[-]?\d+(?:[\.]?)[\d]*)[\']{1}$", RegexOptions.IgnoreCase | RegexOptions.Multiline).Match(text).Success;
            else
                return new Regex(@"^[\']{1}(?<num>\d+(?:[\.]?)[\d]*)[\']{1}$", RegexOptions.IgnoreCase | RegexOptions.Multiline).Match(text).Success;
        }


        /// <summary>
        /// char中是否是ovpb
        /// </summary>
        /// <param name="text"></param>
        /// <param name="isunsigned">是否无符号</param>
        /// <returns></returns>
        public static bool CharIsInt(string text, bool isunsigned = false)
        {
            if (!isunsigned)
                return new Regex(@"^[\']{1}(?<num>[-]?\d+)[\']{1}$", RegexOptions.IgnoreCase | RegexOptions.Multiline).Match(text).Success;
            else
                return new Regex(@"^[\']{1}(?<num>\d+)[\']{1}$", RegexOptions.IgnoreCase | RegexOptions.Multiline).Match(text).Success;
        }

        /// <summary>
        /// 是否是数字型
        /// </summary>
        /// <param name="text"></param>
        /// <param name="isunsigned">是否无符号</param>
        /// <returns></returns>
        public static bool IsInt(string text, bool isunsigned = false)
        {
            if (!isunsigned)
                return new Regex(@"^[-]?\d+$", RegexOptions.IgnoreCase | RegexOptions.Multiline).Match(text).Success;
            else
                return new Regex(@"^\d+$", RegexOptions.IgnoreCase | RegexOptions.Multiline).Match(text).Success;

        }

        /// <summary>
        /// 获取数据库实际字段名称
        /// </summary>
        /// <param name="hiColumn"></param>
        /// <param name="fieldDefinition"></param>
        /// <returns></returns>
        public static string GetDbFieldName(HiColumn hiColumn, FieldDefinition fieldDefinition)
        {
            if (!fieldDefinition.IsAsField && fieldDefinition.AsFieldName.Equals(hiColumn.FieldName, StringComparison.OrdinalIgnoreCase))
                return hiColumn.FieldName;
            else
            {
                if (hiColumn.FieldName.Equals(fieldDefinition.AsFieldName, StringComparison.OrdinalIgnoreCase))
                    return hiColumn.FieldName;
                else
                    return fieldDefinition.AsFieldName;
            }
        }
        /// <summary>
        /// 获取数据库实际表名
        /// </summary>
        /// <param name="hiColumn"></param>
        /// <param name="fieldDefinition"></param>
        /// <returns></returns>
        public static string GetDbTabName(HiColumn hiColumn, FieldDefinition fieldDefinition)
        {
            if (hiColumn.TabName.Equals(fieldDefinition.AsTabName, StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrEmpty(hiColumn.TabName))
                    return hiColumn.TabName.ToLower();
                else
                    throw new Exception($"字段[{fieldDefinition.FieldName}]在表[{fieldDefinition.AsTabName}]不存在!");
            }
            else
                return fieldDefinition.AsTabName.ToLower();//重命名的表统一改成小写
        }


        /// <summary>
        /// 检测Having的条件字段
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Tuple<bool, FieldDefinition> CheckHavingField(string text)
        {
            Tuple<bool, FieldDefinition> result = new Tuple<bool, FieldDefinition>(false, null);
            if (text is null || string.IsNullOrEmpty(text)) return result;
            Dictionary<string, string> _reg = RegexGrp(Constants.REG_FUNCTIONNORENAME, text);

            if (_reg.Count > 0)
            {
                FieldDefinition myfield = new FieldDefinition();
                myfield.SqlName = text;
                myfield.TabName = _reg["tab"].ToString();
                myfield.FieldName = _reg["field"].ToString();
                result = new Tuple<bool, FieldDefinition>(true, myfield);
            }
            else
            {
                Dictionary<string, string> _reg_fun = RegexGrp(Constants.REG_FUNCTION, text);
                if (_reg_fun.Count > 0)
                {
                    FieldDefinition myfield = new FieldDefinition();
                    myfield.IsFun = true;
                    switch (_reg_fun["fun"].ToLower())
                    {
                        case "count":
                            myfield.DbFun = DbFunction.COUNT;
                            break;
                        case "avg":
                            myfield.DbFun = DbFunction.AVG;
                            break;
                        case "max":
                            myfield.DbFun = DbFunction.MAX;
                            break;
                        case "min":
                            myfield.DbFun = DbFunction.MIN;
                            break;
                        case "sum":
                            myfield.DbFun = DbFunction.SUM;
                            break;
                        default:
                            break;

                    }

                    myfield.SqlName = text;
                    myfield.TabName = _reg_fun["tab"].ToString();
                    myfield.FieldName = _reg_fun["field"].ToString();


                    if (!string.IsNullOrEmpty(myfield.AsFieldName))
                        result = new Tuple<bool, FieldDefinition>(true, myfield);
                    else
                        result = new Tuple<bool, FieldDefinition>(false, null);
                }
            }
            return result;
        }

        /// <summary>
        /// 检测是不是指定查询字段信息,并返回字段结构
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Tuple<bool, FieldDefinition> CheckQueryField(string text)
        {
            Tuple<bool, FieldDefinition> result = new Tuple<bool, FieldDefinition>(false, null);
            if (text is null || string.IsNullOrEmpty(text)) return result;
            Dictionary<string, string> _reg = RegexGrp(Constants.REG_FIELDANDRENAME, text);
            if (_reg.Count > 0)
            {
                FieldDefinition myfield = new FieldDefinition();
                myfield.SqlName = text;
                myfield.TabName = _reg["tab"].ToString();
                myfield.FieldName = _reg["field"].ToString();
                myfield.AsFieldName = _reg["refield"].ToString();
                result = new Tuple<bool, FieldDefinition>(true, myfield);
            }

            else
            {
                Dictionary<string, string> _reg_fun = RegexGrp(Constants.REG_FUNCTION, text);
                if (_reg_fun.Count > 0)
                {
                    FieldDefinition myfield = new FieldDefinition();
                    myfield.IsFun = true;

                    switch (_reg_fun["fun"].ToLower())
                    {
                        case "count":
                            myfield.DbFun = DbFunction.COUNT;
                            break;
                        case "avg":
                            myfield.DbFun = DbFunction.AVG;
                            break;
                        case "max":
                            myfield.DbFun = DbFunction.MAX;
                            break;
                        case "min":
                            myfield.DbFun = DbFunction.MIN;
                            break;
                        case "sum":
                            myfield.DbFun = DbFunction.SUM;
                            break;
                        default:
                            break;

                    }

                    myfield.SqlName = text;
                    myfield.TabName = _reg_fun["tab"].ToString();
                    myfield.FieldName = _reg_fun["field"].ToString();

                    myfield.AsFieldName = _reg_fun["refield"].ToString();
                    if (!string.IsNullOrEmpty(myfield.AsFieldName))
                        result = new Tuple<bool, FieldDefinition>(true, myfield);
                    else
                        result = new Tuple<bool, FieldDefinition>(false, null);
                }
                else
                    result = new Tuple<bool, FieldDefinition>(false, null);
            }
            return result;
        }

        /// <summary>
        /// 检测是不是指定查询字段信息,并返回字段结构 检测不带as的
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Tuple<bool, FieldDefinition> CheckQueryNoAsField(string text)
        {
            Tuple<bool, FieldDefinition> result = new Tuple<bool, FieldDefinition>(false, null);
            if (text is null || string.IsNullOrEmpty(text)) return result;
            Dictionary<string, string> _reg = RegexGrp($"{Constants.REG_FIELDNOASNAME}$", text);
            if (_reg.Count > 0)
            {
                FieldDefinition myfield = new FieldDefinition();
                myfield.SqlName = text;
                myfield.TabName = _reg["tab"].ToString();
                myfield.FieldName = _reg["field"].ToString();
                myfield.AsFieldName = myfield.FieldName;
                result = new Tuple<bool, FieldDefinition>(true, myfield);
            }

            else
            {
                Dictionary<string, string> _reg_fun = RegexGrp($"{Constants.REG_FUNCTIONNORENAME}$", text);
                if (_reg_fun.Count > 0)
                {
                    FieldDefinition myfield = new FieldDefinition();
                    myfield.IsFun = true;

                    switch (_reg_fun["fun"].ToLower())
                    {
                        case "count":
                            myfield.DbFun = DbFunction.COUNT;
                            break;
                        case "avg":
                            myfield.DbFun = DbFunction.AVG;
                            break;
                        case "max":
                            myfield.DbFun = DbFunction.MAX;
                            break;
                        case "min":
                            myfield.DbFun = DbFunction.MIN;
                            break;
                        case "sum":
                            myfield.DbFun = DbFunction.SUM;
                            break;
                        default:
                            break;

                    }

                    myfield.SqlName = text;
                    myfield.TabName = _reg_fun["tab"].ToString();
                    myfield.FieldName = _reg_fun["field"].ToString();

                    myfield.AsFieldName = myfield.FieldName;
                    if (!string.IsNullOrEmpty(myfield.AsFieldName))
                        result = new Tuple<bool, FieldDefinition>(true, myfield);
                    else
                        result = new Tuple<bool, FieldDefinition>(false, null);
                }
                else
                    result = new Tuple<bool, FieldDefinition>(false, null);
            }
            return result;
        }

        /// <summary>
        /// 检测是否符合表名规划
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Tuple<bool, string> CheckTabName(string text)
        {
            Tuple<bool, string> result = new Tuple<bool, string>(false, "");
            if (text is null) return result;
            if (RegexMatch(Constants.REG_TABNAME, text))
                result = new Tuple<bool, string>(true, text.Trim());
            else
                result = new Tuple<bool, string>(false, text);
            return result;
        }
        /// <summary>
        /// 检测是否符合字段命名
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Tuple<bool, string> CheckFieldName(string text)
        {
            Tuple<bool, string> result = new Tuple<bool, string>(false, "");
            if (text is null) return result;
            if (RegexMatch(Constants.REG_NAME, text))
                result = new Tuple<bool, string>(true, text.Trim());
            else
                result = new Tuple<bool, string>(false, text);
            return result;
        }


        /// <summary>
        /// 检测是否是字段信息(不包括重命名）
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Tuple<bool, FieldDefinition> CheckField(string text)
        {
            Tuple<bool, FieldDefinition> result = new Tuple<bool, FieldDefinition>(false, null);
            if (text is null) return result;
            Dictionary<string, string> _reg = RegexGrp(Constants.REG_FIELDNAME, text);
            if (_reg.Count > 0)
            {
                FieldDefinition myfield = new FieldDefinition();
                myfield.SqlName = text;
                myfield.TabName = _reg["tab"].ToString();
                myfield.FieldName = _reg["field"].ToString();
                myfield.AsFieldName = _reg["field"].ToString();//默认
                result = new Tuple<bool, FieldDefinition>(true, myfield);
            }
            else
            {
                result = new Tuple<bool, FieldDefinition>(false, null);
            }
            return result;
        }

        /// <summary>
        /// 检测字段On关联条件
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Tuple<bool, FieldDefinition, FieldDefinition> CheckOnField(string text)
        {

            Tuple<bool, FieldDefinition, FieldDefinition> result = new Tuple<bool, FieldDefinition, FieldDefinition>(false, null, null);
            if (text is null) return result;
            Dictionary<string, string> _reg = RegexGrp(Constants.REG_JOINON, text);
            if (_reg.Count > 0)
            {
                FieldDefinition leftfield = new FieldDefinition();
                leftfield.SqlName = text;
                leftfield.TabName = _reg["tab"].ToString();
                leftfield.FieldName = _reg["field"].ToString();
                leftfield.AsFieldName = _reg["field"].ToString();//

                FieldDefinition rightfield = new FieldDefinition();
                rightfield.SqlName = text;
                rightfield.TabName = _reg["rtab"].ToString();
                rightfield.FieldName = _reg["rfield"].ToString();
                rightfield.AsFieldName = _reg["rfield"].ToString();//默认

                result = new Tuple<bool, FieldDefinition, FieldDefinition>(true, leftfield, rightfield);
            }
            else
                result = new Tuple<bool, FieldDefinition, FieldDefinition>(false, null, null);
            return result;
        }

        /// <summary>
        /// 将 Hisql查询条件的 object，在 In 条件匹配时，转换为List<T>。T根据字段类型需要指定
        /// </summary>
        /// <typeparam name="T">根据字段类型，指定返回</typeparam>
        /// <param name="obj"></param>
        /// <returns>返回指定类型的集合</returns>
        public static List<T> ConverterObjToList<T>(object obj)
        {
            var list = new List<T>();

            if (obj is List<int> intList)
            {
                if (typeof(T) == typeof(int))
                    return intList as List<T>;

                foreach (int value in intList)
                {
                    list.Add((T)Convert.ChangeType(value, typeof(T)));
                }
            }
            if (obj is List<Int64> Int64List)
            {
                if (typeof(T) == typeof(Int64))
                    return Int64List as List<T>;
                if (typeof(T) == typeof(int))
                    return Int64List as List<T>;

                foreach (var value in Int64List)
                {
                    list.Add((T)Convert.ChangeType(value, typeof(T)));
                }
            }
            if (obj is List<Int16> Int16List)
            {
                if (typeof(T) == typeof(Int16))
                    return Int16List as List<T>;
                foreach (var value in Int16List)
                {
                    list.Add((T)Convert.ChangeType(value, typeof(T)));
                }
            }
            if (obj is List<decimal> decimalList)
            {
                if (typeof(T) == typeof(decimal))
                    return decimalList as List<T>;
                foreach (var value in decimalList)
                {
                    list.Add((T)Convert.ChangeType(value, typeof(T)));
                }
            }
            if (obj is List<string> stringList)
            {
                if (typeof(T) == typeof(string))
                    return stringList as List<T>;

                foreach (var value in stringList)
                {
                    list.Add((T)Convert.ChangeType(value, typeof(T)));
                }
            }
            if (obj is List<object> objList)
            {
                if (typeof(T) == typeof(object))
                    return objList as List<T>;

                foreach (var value in objList)
                {
                    list.Add((T)Convert.ChangeType(value, typeof(T)));
                }
            }
            return list;
        }

    }
}
