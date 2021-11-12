using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HiSql
{
    public class Tool
    {

        /// <summary>
        /// 正则表达式获取匹配内容
        /// \"(?<grp>[\s\S]*)\"
        /// </summary>
        /// <param name="regex"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Dictionary<string, string> RegexGrp(string regex,string text)
        {
            Regex _regex = new Regex(regex, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Dictionary<string, string> _dic = new Dictionary<string, string>();
            Match _match = _regex.Match(text);
            while (_match.Success)
            {
                foreach (string name in _regex.GetGroupNames())
                {
                    _dic.Add(name, _match.Groups[_regex.GroupNumberFromName(name)].Value);
                }
                _match = _match.NextMatch();
            }
            return _dic;
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
        public static bool IsDecimal(string text,bool isunsigned=false)
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
            if(!isunsigned)
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
        /// 检测是不是指定查询字段信息,并返回字段结构
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Tuple<bool, FieldDefinition> CheckQueryField(string text)
        {
            Tuple<bool, FieldDefinition> result =new Tuple<bool, FieldDefinition>(false,null) ;
            if (text is null) return result;
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
                    if(!string.IsNullOrEmpty(myfield.AsFieldName))
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
            Tuple<bool, string> result=new Tuple<bool, string> (false,"");
            if (text is null) return result;
            if (RegexMatch(Constants.REG_TABNAME, text))
                result = new Tuple<bool, string>(true, text.Trim());
            else
                result = new Tuple<bool, string>(false, text );
            return result;
        }
        /// <summary>
        /// 检测是否符合字段命名
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Tuple<bool, string> CheckFieldName(string text)
        {
            Tuple<bool, string> result=new Tuple<bool, string> (false,"");
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
            Tuple<bool, FieldDefinition> result=new Tuple<bool, FieldDefinition> (false,null);
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
        public static Tuple<bool, FieldDefinition,FieldDefinition> CheckOnField(string text)
        {

            Tuple<bool, FieldDefinition, FieldDefinition> result=new Tuple<bool, FieldDefinition, FieldDefinition> (false,null,null);
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

                result = new Tuple<bool, FieldDefinition,FieldDefinition>(true, leftfield, rightfield);
            }else
                result= new Tuple<bool, FieldDefinition, FieldDefinition>(false, null, null);
            return result;

        }

    }
}
