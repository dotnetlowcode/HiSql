using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql.AST
{
    /// <summary>
    /// ast 语法解析where
    /// where 条件由 字段通过运算符>,<,=,>=,<=,<>,like,not like 还有 between and ,
    /// 子运算 ( ) ,in ,not in, in (select ...),not in (select ..),exists,not exists
    /// 
    /// 这里涉及比较复杂的正则表达式 不要随便修改
    /// add by tgm  date:2021.12.1 email:tansar@126.com
    /// </summary>
    public class WhereParse
    {


        public static class  Constants
        {
            public static  List<WhereGrp> REG_WHERE = new List<WhereGrp> {
                new WhereGrp(){
                    //识别字段
                    SType = StatementType.FieldValue,
                    Reg = new StringBuilder()
                    //[`]? 2021.12.3 修改允许不带`
                    //.Append(@"^(?:[\s]*)[`]?(?:(?<flag>[\#]{1,2}|[\@]{1})?(?<tab>[\w]+)(?:[\.]{1}))?(?<field>[\w]+)\s*[`]?[\s]*")
                    .Append(@"^(?:[\s]*)[`]?(?<fields>(?:(?<flag>[\#]{1,2}|[\@]{1})?(?<tab>[\w]+)(?:[\.]{1}))?(?<field>[\w]+))\s*[`]?[\s]*")
                    .Append(@"(?<op>=|\>(?![\=\>\<\!])|\<(?![\=\>\<\!])|\!\=|\<\>)[\s]*")
                    .Append(@"(?:(?<value>[-]?\d+(?:[\.]?)[\d]*)|[\'](?<value>[^\']*)[\']")
                    .Append(")").ToString()

                },
                new WhereGrp()
                {
                    //识别括号中的条件
                    SType = StatementType.SubCondition,
                    Reg = new StringBuilder()
                    .Append(@"^(?:[\s]*)\((?<content>[^\(\)]*(((?<open>\()[^\(\)]*)+((?<-open>\))[^\(\)]*)+)*(?(open)(?!)))\)(?<close>[\)]*)")
                    .ToString()

                },
                new WhereGrp()
                {
                    //识别运算符
                    //[`]? 2021.12.3 修改允许不带`
                    SType = StatementType.In,
                    Reg = new StringBuilder()
                    //.Append(@"^(?:[\s]*)[`]?(?:(?<flag>[\#]{1,2}|[\@]{1})?(?<tab>[\w]+)(?:[\.]{1}))?(?<field>[\w]+)\s*[`]?[\s]*")
                    .Append(@"^(?:[\s]*)[`]?(?<fields>(?:(?<flag>[\#]{1,2}|[\@]{1})?(?<tab>[\w]+)(?:[\.]{1}))?(?<field>[\w]+))\s*[`]?[\s]*")
                    .Append(@"(?<symbol>in|not[\s]+in)")
                    .Append(@"(?:[\s]*)\((?<content>[^\(\)]*(((?<open>\()[^\(\)]*)+((?<-open>\))[^\(\)]*)+)*(?(open)(?!)))\)(?<close>[\)]*)")
                    .ToString()

                }

            };
        }


        private string _sqlwhere;
        private string _currwhere;
        string _reg_symbol = @"^[\s]*(?<mode>and|or)";

        List<WhereResult> lstresult = new List<WhereResult>();

        /// <summary>
        /// 返回结果
        /// </summary>
        public List<WhereResult> Result { get { return lstresult; } }

        public WhereParse(string _hisqlwhere)
        {
            _sqlwhere = _hisqlwhere;
            _currwhere = _hisqlwhere;

            #region  解析工具
            

            #endregion


            #region 解析
            lstresult = parseWhere(_currwhere);
            #endregion


        }

        /// <summary>
        /// 语法解析
        /// </summary>
        /// <param name="wherestr"></param>
        /// <returns></returns>
        private List<WhereResult>  parseWhere(string wherestr)
        {
            bool _isexpsymbol = false;
            bool _checkok = true;
            List<WhereResult> listresult = new List<WhereResult>();

            while (!string.IsNullOrEmpty(wherestr.Trim()))
            {
                if (!_isexpsymbol)
                {
                    int _idx = 0;
                    bool _ismatch = false;
                    foreach (WhereGrp _expr in Constants.REG_WHERE)
                    {
                        _idx++;
                        var result = Tool. RegexGrpOrReplace(_expr.Reg, wherestr);
                        if (result.Item1)
                        {
                            //暂时不支持in语句 下一版本会支持
                            if (!_expr.SType .IsIn<StatementType>( StatementType.In,StatementType.FieldBetweenValue))
                            {
                                _ismatch = result.Item1;
                                WhereResult whereResult = new WhereResult();
                                whereResult.SType = _expr.SType;
                                whereResult.Result = result.Item2;
                                if (result.Item2.ContainsKey("0"))
                                    whereResult.Statement = result.Item2["0"].ToString();
                                else
                                    throw new Exception($"解析错误无法识别的解析结果 {wherestr}");
                                listresult.Add(whereResult);
                               
                                wherestr = result.Item3;
                                break;
                            }
                            else
                            {
                                _checkok = false;
                                throw new Exception($"语句{wherestr} 暂时不支持 in 或not in 及between and语法");
                                //Console.WriteLine($"语句{wherestr} 暂时不支持 in 语法");
                            }
                        }

                    }
                    if (!_ismatch)
                    {
                        _checkok = false;
                        //Console.WriteLine($"语句{wherestr} 附近出现语法错误!");
                        throw new Exception($"语句 {wherestr} 附近出现语法错误!");
                        
                        
                    }
                }
                else
                {
                    var result = Tool.RegexGrpOrReplace(_reg_symbol, wherestr);
                    if (result.Item1)
                    {
                        WhereResult whereResult = new WhereResult();
                        whereResult.SType = StatementType.Symbol;
                        whereResult.Result = result.Item2;
                        if (result.Item2.ContainsKey("0"))
                            whereResult.Statement = result.Item2["0"].ToString();
                        else
                            throw new Exception($"解析错误无法识别的解析结果 {wherestr}");
                        listresult.Add(whereResult);
                        wherestr = result.Item3;
                        
                    }
                    else
                    {
                        Console.WriteLine($"语句 {wherestr} 附近缺少 and 或or 运算符");
                        _checkok = false;
                        break;
                    }
                }
                _isexpsymbol = !_isexpsymbol;
            }

            return listresult;
        }


    }
}
