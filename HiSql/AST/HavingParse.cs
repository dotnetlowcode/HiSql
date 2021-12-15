using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql.AST
{
    /// <summary>
    /// author tansar 
    /// date:2021.12.11
    /// 通过正则表达式解析having条件 Having中包括字段条件，还包括聚合函数条件
    /// 并对不符合语法的进行错误提示
    /// </summary>
    public class HavingParse
    {

        public static class Constants
        {

            public static string REG_HAVING  = @"^\s*having\s+(?<having>[\s\S]*?)(?=\border\s+\bby\b|\bwhere\b|\bunion\b)|^\s*having\b\s+(?<having>[\s\S]*)";
            /// <summary>
            /// 字段
            /// </summary>
            public static string REG_HAVING_FIELD =new StringBuilder()
                .Append($@"{HiSql.Constants.REG_FIELDNOASNAME}")
                .Append(@"(?<op>=|\>(?![\=\>\<\!])|\<(?![\=\>\<\!])|\!\=|\<\>)[\s]*")
                .Append(@"(?:(?<value>[-]?\d+(?:[\.]?)[\d]*)|")
                .Append(@"(?:[\'](?<value>[\s\S]*)?[\']\s*(?=\band\b|\bor\b)|[\'](?<value>[\s\S]*)?[\']\s*$)")
                .Append(")")
                .ToString()
                ;
            /// <summary>
            /// 聚合函数
            /// </summary>
            public static string REG_HAVING_AGGREGATION = new StringBuilder()
                .Append($@"{HiSql.Constants.REG_FUNCTIONNORENAME}")
                .Append(@"(?<op>=|\>(?![\=\>\<\!])|\<(?![\=\>\<\!])|\!\=|\<\>)[\s]*")
                .Append(@"(?:(?<value>[-]?\d+(?:[\.]?)[\d]*)|")
                .Append(@"(?:[\'](?<value>[\s\S]*)?[\']\s*(?=\band\b|\bor\b)|[\'](?<value>[\s\S]*)?[\']\s*$)")
                .Append(")")
                .ToString()
                ;
        }
        string _reg_symbol = @"^[\s]*(?<mode>\band\b|\bor\b)";


        List<HavingResult> _lsthaving = new List<HavingResult>();
        public List<HavingResult> Result { get { return _lsthaving; } }
        public HavingParse(string havingstr)
        {
            
            if (!string.IsNullOrEmpty(havingstr))
            {
                havingstr=parseHaving(havingstr);
            }                                                
        }

        /// <summary>
        /// having 字符串条件 
        /// 注：不能带 having 前辍
        /// </summary>
        /// <param name="havingstr"></param>
        /// <returns></returns>
        string parseHaving(string havingstr)
        {
            
            bool _isexpsymbol = true;
            
            while (!string.IsNullOrEmpty(havingstr.Trim()))
            {
                var rtndic = Tool.RegexGrpOrReplace($"{Constants.REG_HAVING_FIELD}|{Constants.REG_HAVING_AGGREGATION}", havingstr);

                if (rtndic.Item1)
                {
                    if (_isexpsymbol)
                        _isexpsymbol = !_isexpsymbol;
                    else
                        throw new Exception($"{HiSql.Constants.HiSqlSyntaxError}Having语句[{havingstr}] 附近有语法错误");

                    HavingResult havingResult = new HavingResult();
                    havingResult.SType = StatementType.FieldValue;

                    havingResult.Statement = rtndic.Item2["0"].ToString();
                    havingResult.Result = rtndic.Item2;

                    havingstr = rtndic.Item3;
                    _lsthaving.Add(havingResult);
                } 
                else
                {
                    rtndic = Tool.RegexGrpOrReplace(_reg_symbol, havingstr);
                    if (rtndic.Item1)
                    {
                        havingstr = rtndic.Item3;
                        if (!_isexpsymbol)
                            _isexpsymbol = !_isexpsymbol;
                        else
                            throw new Exception($"{HiSql.Constants.HiSqlSyntaxError} Having语句[{havingstr}] 附近有语法错误");

                        HavingResult havingResult = new HavingResult();
                        havingResult.SType = StatementType.Symbol;

                        havingResult.Statement = rtndic.Item2["0"].ToString();
                        havingResult.Result = rtndic.Item2;

                        havingstr = rtndic.Item3;
                        _lsthaving.Add(havingResult);

                    }
                    else
                        throw new Exception($"{HiSql.Constants.HiSqlSyntaxError} Having语句[{havingstr}]附近有语法错误");
                }
            }
            if(_isexpsymbol)
                throw new Exception($"{HiSql.Constants.HiSqlSyntaxError} Having语句[{havingstr}] 附近有语法错误");
            return havingstr;
        }
    }
}
