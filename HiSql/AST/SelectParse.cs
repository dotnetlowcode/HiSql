﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql.AST
{
    /// <summary>
    /// 解析hisql 语句
    /// hisql 支持以像写sql写法，但可以支持在不同的数据库运行
    /// 2021.12.7
    /// </summary>
    public class SelectParse
    {
        public static class Constants
        {


            
            
            /// <summary>
            /// 检测是不是查询语句
            /// </summary>
            public static string REG_SELECT = @"^\s*(?<cmd>select)\s+(?<field>[\w\s\S]+(?=\bfrom\b))(?:\bfrom\b)(?<from>(?:[\s]+)(?<flag>[\#]{1,2}|[\@]{1})?(?<tab>[\w]+)\s*[\s\w\S]*)";

            /// <summary>
            /// in 值带单引号
            /// </summary>
            public static string REG_INCHARVALUE = @"[\'](?<content>[^']*)[\'](?!\')";

            /// <summary>
            /// in 值 数值
            /// </summary>
            public static string REG_INVALUE = @"(?<content>[-]?\d+(?:[\.]?)[\d]*)";

            public static string REG_SELECT_CMD = @"^\s*(?<cmd>select)\s+";


            public static string REG_DISTINCT = @"^\s*\b(?<cmd>distinct)\b";

            public static string REG_SELECT_FIELD = @"^\s*(?<field>[\w\s\S]*?(?=\bfrom\b))";

            public static string REG_SELECT_FROM = @"^\s*(?:\bfrom\b)(?<from>(?:[\s]+)(?<table>(?:[\s]*)(?<flag>[\#]{1,2}|[\@]{1})?(?<tab>[\w]+))\s*(?:\bas\b\s*(?<asname>[\w]+))?\s*)";

            /// <summary>
            /// 识别 inner join ,left inner join ,outer join ,join ==inner join ,left join == left inner join 
            /// </summary>
            public static string REG_SELECT_JOIN = @"^\s*(?<join>\binner\s*join\b|left\s*\bouter\s*\bjoin\b|\bright\s*\bouter\s*\bjoin\b|\bjoin\b|\bouter\s*\bjoin\b|\bjoin\b|\bleft\s*\bjoin\b|\bright\s*\bjoin\b)\s*(?<table>(?:[\s]*)(?<flag>[\#]{1,2}|[\@]{1})?(?<tab>[\w]+))\s*(?:\bas\b\s*(?<asname>[\w]+))?\s*(?:\bon\b)\s*";


            /// <summary>
            /// 左右表关联连接
            /// </summary>
            public static string REG_SELECT_INNERJOIN = @"^\s*inner\s*join\s*$|^\s*join\s*$";

            /// <summary>
            /// 左连接
            /// </summary>
            public static string REG_SELECT_LEFTJOIN = @"^\s*left\s*join\s*|^\s*left\s*\bouter\s*\bjoin\s*|^\s*outer\s*join\s*$";

            /// <summary>
            /// 右连接
            /// </summary>
            public static string REG_SELECT_RIGHTJOIN = @"^\s*right\s*join\s*|^\s*right\s*\bouter\s*\bjoin\s*$";


            public static string REG_SELECT_WHERE = @"^\s*\b(?<cmd>where)\b";

            public static string REG_SELECT_WHERE1 = @"^\s*\b(?<cmd>where)(?<where>[\s\S.\w]*?)(?=\border\s+\bby\b|\bgroup\s+\bby\b|\bhaving\b|\bunion\b)";

            public static string REG_SELECT_WHERE2 = @"^\s*\b(?<cmd>where)(?<where>[\s\S.\w]*)";

            public static string REG_SELECT_ORDER = @"^\s*order\s+by\s+(?<order>[\s\S]*)";

            public static string REG_SELECT_ORDER_FIELD = @"^(?:[\s]*)(?<field>(?:(?:[\#]{1,2}|[\@]{1})?(?:[\w]+)(?:[\.]{1}))?(?:[\w]+))?\s*(?<sort>asc|desc)?\s*[\,]?\s*";

            public static string REG_SELECT_GROUP = @"^\s*group\s+by\s+(?<group>[\s\S]*?)(?=\bhaving\b|\border\s+\bby\b)|^\s*group\s+by\s+(?<group>[\s\S]*)";


            public static string REG_SELECT_FIELDNAME = @"^(?:[\s]*)(?<fieldname>(?<flag>[\#]{1,2}|[\@]{1})?(?<tab>[\w]+)(?:[\.]{1})?(?<field>[\w]+))\s*";

            /// <summary>
            /// where 条件中是否有字查询
            /// </summary>
            public static string REG_SELECT_SUB = @"\(\s*select(.*)\)";

            /// <summary>
            /// 分隔符
            /// </summary>
            public static string REG_SELECT_SPLIT = @"^\s*\,\s*";



            public static List<JoinGrp> REG_GRP = new List<JoinGrp> {
                new JoinGrp{ 
                    JType=JoinStatementType.SubCondition, 
                    Reg = new StringBuilder()
                    .Append(@"^(?:[\s]*)\((?<content>[^\(\)]*(((?<open>\()[^\(\)]*)+((?<-open>\))[^\(\)]*)+)*(?(open)(?!)))\)(?<close>[\)]*)")
                    .ToString() 
                },

                //
                new JoinGrp{
                    JType=JoinStatementType.Field,
                    Reg=HiSql.Constants.REG_JOINON
                },
                new JoinGrp{
                    JType=JoinStatementType.Symbol,
                    Reg=@"^[\s]*(?<mode>\band\b|\bor\b)"
                }

            };

        }
        HiSqlProvider Context=null;

        private string _sql = "";

        string _field = "";
        string _cmd = "";

        //是否去重
        bool _isdistinct = false;

        string _fromtable = "";
        IQuery _query = null;


        /// <summary>
        /// 字段
        /// </summary>
        public string Fields
        {
            get { return _field; }
        }
        public IQuery Query
        {
            get
            {
                return _query;
            }
        }

        /// <summary>
        /// 解析sql语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="_singlefield"></param>
        public SelectParse(string sql, HiSqlProvider context, bool _singlefield = false)
        {
            this._sql = sql;
            sql = sql.Replace(System.Environment.NewLine, " ").Replace("\n"," ");
            

            Context = context;
            if (context == null) throw new Exception($"context 为Null");
            parseSelect(sql, _singlefield);
        }

        public SelectParse(string sql, IQuery query)
        {
            sql = sql.Replace(System.Environment.NewLine, " ").Replace("\n", " ");
            this._sql = sql;
            _query = query;
            if (_query == null) throw new Exception($"context 为Null");
            parseSelect(sql, false);
        }

        private void parseSelect(string sql, bool _singlefield = false)
        {
            //是否匹配sql
            if (Tool.RegexMatch(Constants.REG_SELECT, sql))
            {
                #region 拆解select 查询命令
                var rtn = Tool.RegexGrpOrReplace(Constants.REG_SELECT_CMD, sql);
                if (rtn.Item1 && rtn.Item2.ContainsKey("cmd"))
                {
                    _cmd = rtn.Item2["cmd"];
                    sql = rtn.Item3;
                    
                }
                else
                {
                    throw new Exception($"{HiSql.Constants.HiSqlSyntaxError}语句{sql} 有语法错误 未能识别select");
                }
                #endregion



                #region 拆解distinct
                //add by tgm date:2022.3.24
                rtn = Tool.RegexGrpOrReplace(Constants.REG_DISTINCT, sql);
                if (rtn.Item1 && rtn.Item2.ContainsKey("cmd"))
                {
                    // 说明设置了distinct 去重关键词
                    _isdistinct = true;
                    sql = rtn.Item3;
                }
                else
                    _isdistinct = false;
                #endregion



                #region 拆解field
                rtn = Tool.RegexGrpOrReplace(Constants.REG_SELECT_FIELD, sql);
                if (rtn.Item1 && rtn.Item2.ContainsKey("field"))
                {
                    _field = rtn.Item2["field"];
                    sql = rtn.Item3;
                }
                else
                    throw new Exception($"{HiSql.Constants.HiSqlSyntaxError}语句[{sql}]附近有语法错误,未能识别指定的字段");

                #endregion

                #region 拆解from
                rtn = Tool.RegexGrpOrReplace(Constants.REG_SELECT_FROM, sql);
                if (rtn.Item1 && rtn.Item2.ContainsKey("from"))
                {
                    _fromtable = rtn.Item2["from"];
                    sql = rtn.Item3;

                    if (!string.IsNullOrEmpty(rtn.Item2["asname"].ToString()))
                    {
                        if (Context != null)
                            _query = Context.Query(rtn.Item2["table"], rtn.Item2["asname"].ToString().Trim());
                        else
                            _query.Query(rtn.Item2["table"], rtn.Item2["asname"].ToString().Trim());
                    }
                    else
                    {
                        if (Context != null)
                            _query = Context.Query(rtn.Item2["table"].ToString());
                        else
                            _query.Query(rtn.Item2["table"].ToString());
                    }

                    _query.Field(_field.Split(','));

                    _query.IsDistinct = _isdistinct;
                }
                else
                    throw new Exception($"{HiSql.Constants.HiSqlSyntaxError}语句[{sql}]附近有语法错误,未能识别[from]关键词");
                #endregion

                #region  拆解 select 非必填关键字段  
                //where order by group inner join ,

                #region join 相关
                if (!string.IsNullOrEmpty(sql.Trim()))
                {
                    var dicrtn = Tool.RegexGrp(Constants.REG_SELECT_JOIN, sql);
                    if (dicrtn.Count>0)
                    {
                        //说明是join

                        //string _tabjoin = rtn.Item2["table"].ToString();
                        //if (!string.IsNullOrEmpty(rtn.Item2["asname"].ToString()))
                        //{
                        //    _query.Join(_tabjoin).As(rtn.Item2["asname"].ToString());
                        //}
                        //else
                        //    _query.Join(_tabjoin);


                        sql = parseJoin(sql);
                    }
                }
                #endregion
                //where 
                #region where

                if (!string.IsNullOrEmpty(sql.Trim()))
                {
                    rtn = Tool.RegexGrpOrReplace(Constants.REG_SELECT_WHERE, sql);
                    if (rtn.Item1)
                    {


                        int _pos_idx = 0;
                        //获取最外层的 关键词位置顺序 可能子语句中也可能包括关键字
                        List<int> lstnum = new List<int> { 
                            sql.LastIndexOf(" order ", StringComparison.OrdinalIgnoreCase),
                            sql.LastIndexOf($" order{System.Environment.NewLine}", StringComparison.OrdinalIgnoreCase),
                            sql.LastIndexOf(" group ", StringComparison.OrdinalIgnoreCase),
                            sql.LastIndexOf($" group{System.Environment.NewLine}",StringComparison.OrdinalIgnoreCase),
                            sql.LastIndexOf(" having ", StringComparison.OrdinalIgnoreCase),
                            sql.LastIndexOf($" having{System.Environment.NewLine}",StringComparison.OrdinalIgnoreCase),
                            sql.LastIndexOf(" union ", StringComparison.OrdinalIgnoreCase) ,
                            sql.LastIndexOf($" union{System.Environment.NewLine}",StringComparison.OrdinalIgnoreCase) ,
                        };

                        List<int> lstnum2 = lstnum.Where(x => x >= 0).ToList();
                        if (lstnum2.Count > 0)
                        {
                            //按大小顺序排序
                            lstnum2.Sort((a, b) =>
                            {
                                return a.CompareTo(b);
                            });
                            _pos_idx = lstnum2[0];
                        }

                        //按大小顺序排序
                        //lstnum.Sort((a,b)=> {
                        //    return b.CompareTo(a);
                        //});
                        //_pos_idx = lstnum[0];

                        bool _has_subselect = Tool.RegexMatch(Constants.REG_SELECT_SUB, sql);

                        if (_has_subselect  && sql.LastIndexOf(')') > _pos_idx) //sql.LastIndexOf(')') > _pos_idx
                        {
                            //说明都是子语语句
                            var rtndic = Tool.RegexGrp(Constants.REG_SELECT_WHERE2, sql);
                            if (rtndic.Count > 0)
                            {
                                if (rtndic["where"].LastIndexOf(" order ", StringComparison.OrdinalIgnoreCase) > 0 || rtndic["where"].LastIndexOf($" order{System.Environment.NewLine}", StringComparison.OrdinalIgnoreCase) > 0)
                                {
                                    throw new Exception($"{HiSql.Constants.HiSqlSyntaxError} 子查询语句[{rtndic["where"]}]不允许[order by]排序 ");
                                }
                                else
                                {
                                    _query.Where(rtndic["where"]);
                                    sql = "";
                                }
                            }
                            else
                            {
                                throw new Exception($"{HiSql.Constants.HiSqlSyntaxError}语句[{sql}]附近出现错误");
                            }
                            
                        }
                        else
                        {
                            string _wheresql = sql;

                            if(_pos_idx>0)
                                _wheresql = sql.Substring(0, _pos_idx);
                            var rtndic = Tool.RegexGrp(Constants.REG_SELECT_WHERE2, _wheresql);
                            if (rtndic.Count > 0)
                            {
                                if (rtndic["where"].LastIndexOf(" order ", StringComparison.OrdinalIgnoreCase) > 0 || rtndic["where"].LastIndexOf($" order{System.Environment.NewLine}", StringComparison.OrdinalIgnoreCase) > 0)
                                {
                                    throw new Exception($"{HiSql.Constants.HiSqlSyntaxError} 子查询语句[{rtndic["where"]}]不允许[order by]排序 ");
                                }
                                else
                                {
                                    _query.Where(rtndic["where"]);
                                    if (_pos_idx > 0)
                                        sql = sql.Substring(_pos_idx);
                                    else sql = "";
                                }
                            }
                            else
                            {
                                throw new Exception($"{HiSql.Constants.HiSqlSyntaxError}语句[{sql}]附近出现错误");
                            }


                            
                            
                        }


                        //var rtndic = Tool.RegexGrp(Constants.REG_SELECT_WHERE1, sql);
                        //if (rtndic.Count>0 && rtndic.ContainsKey("where"))
                        //{
                        //    _query.Where(rtndic["where"].ToString());
                        //    rtn= Tool.RegexGrpOrReplace(Constants.REG_SELECT_WHERE1, sql);
                        //    if (rtn.Item1)
                        //        sql = rtn.Item3;
                        //}
                        //else
                        //{
                        //    rtndic = Tool.RegexGrp(Constants.REG_SELECT_WHERE2, sql);
                        //    if (rtndic.Count > 0 && rtndic.ContainsKey("where"))
                        //    {
                        //        _query.Where(rtndic["where"].ToString());
                        //        rtn = Tool.RegexGrpOrReplace(Constants.REG_SELECT_WHERE2, sql);
                        //        if (rtn.Item1)
                        //            sql = rtn.Item3;
                        //    }
                        //    else
                        //    {
                        //        throw new Exception($"{HiSql.Constants.HiSqlSyntaxError}语句[{sql}]附近出现错误");
                        //    }
                        //}
                        // 说明后面接着是where语句
                        
                        
                    }
                }
                #endregion

                #region group by 
                if (!string.IsNullOrEmpty(sql.Trim()))
                {
                    var rtndic = Tool.RegexGrpOrReplace(Constants.REG_SELECT_GROUP, sql);
                    if (rtndic.Item1  && rtndic.Item2.ContainsKey("group"))
                    {
                        parseGroup(rtndic.Item2["group"].ToString());
                        sql = rtndic.Item3;
                    }
                }


                #endregion

                #region having
                //如果having 必出现 group by
                if (!string.IsNullOrEmpty(sql.Trim()))
                {
                    var rtndic = Tool.RegexGrpOrReplace(HavingParse.Constants.REG_HAVING, sql);
                    if (rtndic.Item1)
                    {
                        parseHaving(rtndic.Item2["having"]);
                        sql = rtndic.Item3;
                    }
                }
                #endregion


                #region order by 
                //order by 只能出现在语句的最后
                if (!string.IsNullOrEmpty(sql.Trim()))
                {
                    var rtndic = Tool.RegexGrp(Constants.REG_SELECT_ORDER, sql);

                    if (rtndic.Count > 0 && rtndic.ContainsKey("order"))
                    {
                        sql = parseOrder(rtndic["order"].ToString());
                    }
                    else
                    {
                        throw new Exception($"{HiSql.Constants.HiSqlSyntaxError}语句[{sql}] 语法错误");
                    }
                }

                #endregion

                #endregion
            }
            else
            {
                throw new Exception($"{HiSql.Constants.HiSqlSyntaxError}语句[{sql}] 非查询语言");
            }
        }


        

        string parseJoin(string sql)
        {
            if (!string.IsNullOrEmpty(sql))
            {
                var rtn = Tool.RegexGrpOrReplace(Constants.REG_SELECT_JOIN, sql);
                if (rtn.Item1)
                {
                    //说明是join
                    JoinType _joinType = JoinType.Inner;
                    string _tabjoin = rtn.Item2["table"].ToString();
                    sql = rtn.Item3;

                    if (Tool.RegexMatch(Constants.REG_SELECT_INNERJOIN, rtn.Item2["join"]))
                        _joinType = JoinType.Inner;
                    else if (Tool.RegexMatch(Constants.REG_SELECT_LEFTJOIN, rtn.Item2["join"]))
                        _joinType = JoinType.Left;
                    else if (Tool.RegexMatch(Constants.REG_SELECT_RIGHTJOIN, rtn.Item2["join"]))
                        _joinType = JoinType.Right;


                    if (!string.IsNullOrEmpty(rtn.Item2["asname"].ToString()))
                    {
                        _query.Join(_tabjoin, _joinType).As(rtn.Item2["asname"].ToString());
                    }
                    else
                        _query.Join(_tabjoin, _joinType);

                    sql = parseJoinOn(sql);

                    sql = parseJoin(sql);


                }
            }
            return sql;
        }

        /// <summary>
        /// 解析group by 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        string parseGroup(string sql)
        {
            if (!string.IsNullOrEmpty(sql))
            {
                GroupBy groups = new GroupBy();
                bool _isandor = true;
            
                //当字符串为空或未匹配成功
                while (!string.IsNullOrEmpty(sql.Trim()) )
                {
                    var rtn = Tool.RegexGrpOrReplace(Constants.REG_SELECT_FIELDNAME, sql);
                    if (rtn.Item1)
                    {
                        if (!_isandor)
                        {
                            throw new Exception($"{HiSql.Constants.HiSqlSyntaxError}语句[{sql}]附近有语法错误");
                        }
                        groups.Add(new GroupDefinition(rtn.Item2["fieldname"].ToString()));

                        _isandor = !_isandor;

                        sql = rtn.Item3;
                    }
                    else
                    {
                        rtn = Tool.RegexGrpOrReplace(Constants.REG_SELECT_SPLIT, sql);
                        if (rtn.Item1)
                        {
                            if (_isandor)
                                throw new Exception($"{HiSql.Constants.HiSqlSyntaxError}语句[{sql}]附近有语法错误");
                            _isandor = !_isandor;

                            sql = rtn.Item3;
                        }
                        else
                        {
                            throw new Exception($"{HiSql.Constants.HiSqlSyntaxError}语句[{sql}]附近有语法错误");
                        }
                    }
                }
                if (groups.Elements.Count > 0)
                    _query.Group(groups);
            }
            return sql;
        }

        string parseHaving(string sql)
        {
            if (!string.IsNullOrEmpty(sql.Trim()))
                _query.Having(sql);
            else
                throw new Exception($"{HiSql.Constants.HiSqlSyntaxError} 指定的Having条件为空");
            return sql;
        }


        /// <summary>
        /// 多个Order 排序用逗号,分开
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        string parseOrder(string sql)
        {


            if (!string.IsNullOrEmpty(sql))
            {
                bool _isandor = true;
           

                SortBy sorts = new SortBy();
                while (!string.IsNullOrEmpty(sql.Trim()))
                {
                    var rtn = Tool.RegexGrpOrReplace(Constants.REG_SELECT_ORDER_FIELD, sql);
                    if (rtn.Item1)
                    {
                        if (rtn.Item2["sort"].ToString().Trim().ToLower() == "desc")
                        {
                            sorts.Add(rtn.Item2["field"].ToString(), SortType.DESC);
                        }
                        else
                            sorts.Add(rtn.Item2["field"].ToString());


                        //if (!_isandor)
                        //    throw new Exception($"{HiSql.Constants.HiSqlSyntaxError}语句[{sql}]附近有语法错误");

                        //_isandor = !_isandor;

                        sql = rtn.Item3;
                    }
                    else
                    {
                        rtn = Tool.RegexGrpOrReplace(Constants.REG_SELECT_SPLIT, sql);
                        if (rtn.Item1)
                        {
                            //if (_isandor)
                            //    throw new Exception($"{HiSql.Constants.HiSqlSyntaxError}语句[{sql}]附近有语法错误");
                            //_isandor = !_isandor;

                            sql = rtn.Item3;
                        }
                        else
                        {
                            throw new Exception($"{HiSql.Constants.HiSqlSyntaxError}语句[{sql}]附近有语法错误");
                        }
                    }
                }

                if (sorts.Elements.Count > 0)
                    _query.Sort(sorts);
            }

            return sql;
        }

        string parseJoinOn(string sql)
        {
            if (!string.IsNullOrEmpty(sql))
            {
                // 注释于2022.6.8  新增 join on 支持Hisql语法

                #region 

                //bool _isandor = true;
                //bool _ismatch = true;

                //List<string> onlist = new List<string>();
                //JoinOn joinOn = new JoinOn();
                //while (_ismatch)
                //{
                //    foreach (JoinGrp joinGrp in Constants.REG_GRP)
                //    {
                //        if (string.IsNullOrEmpty(sql.Trim())) { _ismatch = false; break; };
                //        var rtn = Tool.RegexGrpOrReplace(joinGrp.Reg, sql);
                //        if (rtn.Item1)
                //        {
                //            if (joinGrp.JType == JoinStatementType.Field)
                //            {
                //                if (!_isandor)
                //                    throw new Exception($"{HiSql.Constants.HiSqlSyntaxError}语句[{sql}]缺少 逻辑操作符and,or");
                //                _isandor = !_isandor;
                //                _ismatch = true;
                //                onlist.Add(rtn.Item2["0"]);
                //            }
                //            else if (joinGrp.JType == JoinStatementType.SubCondition)
                //            {
                //                _ismatch = false;
                //                throw new Exception($"{HiSql.Constants.HiSqlSyntaxError}语句[{sql}]处有语法错误，暂时不支持该写法");
                //            }
                //            else if (joinGrp.JType == JoinStatementType.Symbol)
                //            {

                //                if (_isandor)
                //                    throw new Exception($"{HiSql.Constants.HiSqlSyntaxError}语句[{sql}]缺少 不能重复指定and,or");
                //                _isandor = !_isandor;
                //                _ismatch = true;
                //            }
                //            else
                //            {
                //                _ismatch = false;
                //                break;
                //            }
                //            sql = rtn.Item3;
                //        }else
                //            _ismatch = false;

                //    }
                //}
                //if (_isandor)
                //{
                //    throw new Exception($"{HiSql.Constants.HiSqlSyntaxError} and,or 后一定要有表达式");
                //}
                //foreach (string _on in onlist)
                //{
                //    joinOn.Add(_on);
                //}
                //if(joinOn.Elements.Count>0)
                //    _query.On(joinOn);


                #endregion

                int _pos_idx = 0;
                //获取最外层的 关键词位置顺序 可能子语句中也可能包括关键字
                List<int> lstnum = new List<int> { 
                    sql.IndexOf(" left ", StringComparison.OrdinalIgnoreCase),
                    sql.IndexOf(" outer ", StringComparison.OrdinalIgnoreCase),
                    //sql.LastIndexOf($" left{System.Environment.NewLine}",StringComparison.OrdinalIgnoreCase),
                    sql.IndexOf(" inner ", StringComparison.OrdinalIgnoreCase), 
                    //sql.LastIndexOf($" inner{System.Environment.NewLine}",StringComparison.OrdinalIgnoreCase), 
                    sql.IndexOf(" join ",StringComparison.OrdinalIgnoreCase), 
                    //sql.LastIndexOf($" join{System.Environment.NewLine}",StringComparison.OrdinalIgnoreCase), 
                    sql.IndexOf(" where ", StringComparison.OrdinalIgnoreCase), 
                    //sql.LastIndexOf($" where{System.Environment.NewLine}", StringComparison.OrdinalIgnoreCase), 
                    sql.LastIndexOf(" order ", StringComparison.OrdinalIgnoreCase), 
                    //sql.LastIndexOf($" order{System.Environment.NewLine}",StringComparison.OrdinalIgnoreCase), 
                    sql.LastIndexOf(" group ", StringComparison.OrdinalIgnoreCase), 
                    //sql.LastIndexOf($" group{System.Environment.NewLine}",StringComparison.OrdinalIgnoreCase), 
                    sql.LastIndexOf(" having ", StringComparison.OrdinalIgnoreCase), 
                    //sql.LastIndexOf($" having{System.Environment.NewLine}",StringComparison.OrdinalIgnoreCase), 
                    sql.IndexOf(" union ", StringComparison.OrdinalIgnoreCase)
                    //sql.LastIndexOf($" union{System.Environment.NewLine}", StringComparison.OrdinalIgnoreCase)

                
                };


                List<int> lstnum2 = lstnum.Where(x => x >= 0).ToList();
                if (lstnum2.Count > 0)
                {
                    //按大小顺序排序
                    lstnum2.Sort((a, b) =>
                    {
                        return a.CompareTo(b);
                    });
                    _pos_idx = lstnum2[0];
                }

                string _wheresql = sql;
                if (_pos_idx > 0)
                {
                    _wheresql = sql.Substring(0, _pos_idx);
                    sql = sql.Substring(_pos_idx);
                }
                else
                    sql = "";

                if (_wheresql.LastIndexOf(')') > _pos_idx)
                {
                    ////说明都是子语语句
                    //var rtndic = Tool.RegexGrp(Constants.REG_SELECT_WHERE2, _wheresql);
                    //if (rtndic.Count > 0)
                    //{
                    //    if (rtndic["where"].LastIndexOf(" order ") > 0)
                    //    {
                    //        throw new Exception($"{HiSql.Constants.HiSqlSyntaxError} 子查询语句[{rtndic["where"]}]不允许[order by]排序 ");
                    //    }
                    //    else
                    //    {
                    //        //_query.Where(rtndic["where"]);
                    //        _wheresql = "";
                    //    }
                    //}
                    //else
                    //{
                    //    throw new Exception($"{HiSql.Constants.HiSqlSyntaxError}语句[{_wheresql}]附近出现错误");
                    //}
                    _query.On(_wheresql);

                }
                else
                {

                    _query.On(_wheresql);

                    //var rtndic = Tool.RegexGrp(Constants.REG_SELECT_WHERE2, _wheresql);
                    //if (rtndic.Count > 0)
                    //{
                    //    if (rtndic["where"].LastIndexOf(" order ") > 0)
                    //    {
                    //        throw new Exception($"{HiSql.Constants.HiSqlSyntaxError} 子查询语句[{rtndic["where"]}]不允许[order by]排序 ");
                    //    }
                    //    else
                    //    {
                    //        _query.Where(rtndic["where"]);
                    //        if (_pos_idx > 0)
                    //            sql = sql.Substring(_pos_idx);
                    //        else sql = "";
                    //    }
                    //}
                    //else
                    //{
                    //    throw new Exception($"{HiSql.Constants.HiSqlSyntaxError}语句[{sql}]附近出现错误");
                    //}
                }


                
                
            }
            return sql;
        }


    }
}
