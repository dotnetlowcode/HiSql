using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql.AST
{
    /// <summary>
    /// 用于跨平台的update hisql语句
    /// add by tgm date:2022.6.27
    /// email:tansar@126.com
    /// </summary>
    public class UpdateParse
    {
        public static class Constants
        {
            /// <summary>
            /// 检测是否是合法的update语句
            /// </summary>
            public static string REG_UPDATE = @"\s*(?<cmd>update)\s+(?:[\s]*)(?<flag>[\#]{1,2}|[\@]{1})?(?<tab>[\w-_]+)\s+(?<set>[.\s\S]+)";

            public static string REG_UPDATE_CMD = @"^\s*(?<cmd>update)\s+";


            public static string REG_UPATE_SET = @"^\s*(?<set>set)\s+";


            public  static string REG_TABNAME = @"^(?<tabname>(?:[\s]*)(?<table>(?:[\s]*)(?<flag>[\#]{1,2}|[\@]{1})?(?<tab>[\w]+))\s*(?:\bas\b\s*(?<asname>[\w]+))?\s*)";
            //public  static string REG_TABNAME = @"^(?:[\s]*)(?<tabname>(?<flag>[\#]{1,2}|[\@]{1})?(?<tab>[\w-_]+))\s*";

            public static string REG_UPDATE_WHERE = @"^\s*\b(?<cmd>where)\b";

            public static string REG_UPDATE_WHERE1 = @"^\s*\b(?<cmd>where)(?<where>[\s\S.\w]*?)(?=\border\s+\bby\b|\bgroup\s+\bby\b|\bhaving\b|\bunion\b)";

            public static string REG_UPDATE_WHERE2 = @"^\s*\b(?<cmd>where)(?<where>[\s\S.\w]*)";


            public static readonly string REG_FIELDTEMPLATE = new StringBuilder()
                .Append(@"^(?:[\s]*)[`]?(?<fields>(?:(?<flag>[\#]{1,2}|[\@]{1})?(?<tab>[\w]+)(?:[\.]{1}))?(?<field>[\w]+))\s*[`]?[\s]*")
                .Append(@"(?<op>=)[\s]*")
                .Append(@"(?:(?<value>[\s\S\w]+))")
                .ToString();


            /// <summary>
            /// 识别 inner join ,left inner join ,outer join ,join ==inner join ,left join == left inner join 
            /// </summary>
            public static string REG_UPATE_JOIN = @"^\s*(?<join>\binner\s*join\b|\bleft\s*\binner\s*join|\bouter\s*\bjoin\b|\bjoin\b|\bleft\s*\bjoin\b)\s*(?<table>(?:[\s]*)(?<flag>[\#]{1,2}|[\@]{1})?(?<tab>[\w]+))\s*(?:\bas\b\s*(?<asname>[\w]+))?\s*(?:\bon\b)\s*";


            /// <summary>
            /// 左右表关联连接
            /// </summary>
            public static string REG_UPDATE_INNERJOIN = @"^\s*inner\s*join\s*$|^\s*join\s*$";

            /// <summary>
            /// 左连接
            /// </summary>
            public static string REG_UPDATE_LEFTJOIN = @"^\s*left\s*inner\s*join\s*$|^\s*left\s*join\s*$";

            /// <summary>
            /// 右连接
            /// </summary>
            public static string REG_UPDATE_RIGHTJOIN = @"^\s*outer\s*join\s*$";

        }
        HiSqlProvider Context = null;
        private string _sql = "";
        string _cmd = "";

        IUpdate _update = null;

        public UpdateParse(string sql, HiSqlProvider context)
        {
            this._sql = sql;
            Context = context;
            if (context == null) throw new Exception($"context 为Null");

            parseUpdate(sql);

        }

        public UpdateParse(string sql, IUpdate update)
        {
            sql = sql.Replace(System.Environment.NewLine, " ");
            this._sql = sql;
            _update = update;
            if (update == null) throw new Exception($"context 为Null");
            _update = update;

            parseUpdate(sql);
        }


        /// <summary>
        /// 解析upadate 有三条语法路线
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <exception cref="Exception"></exception>

        private void parseUpdate(string sql)
        {

            // 语法路线1： update h_tewst    set user = 'abc' where....

            // 语法路线2 update a set a.user='abc from h_test as a where ...

            // 语法路线3：update set a.user=b.user from h_test as a inner join h_test as b on a.did=b.did where....
            //如果是多表关联更新 那么统一是语法路线3

            if (Tool.RegexMatch(Constants.REG_UPDATE, sql))
            {
                #region 拆解 update 语句
                var rtn = Tool.RegexGrpOrReplace(Constants.REG_UPDATE_CMD, sql);
                if (rtn.Item1 && rtn.Item2.ContainsKey("cmd"))
                {
                    _cmd = rtn.Item2["cmd"];
                    sql = rtn.Item3;//

                }
                else
                    throw new Exception($"{HiSql.Constants.HiSqlSyntaxError}语句{sql} 有语法错误 未能识别update");
                #endregion



                #region 获取表名

                rtn=Tool.RegexGrpOrReplace(Constants.REG_TABNAME,sql);
                if (rtn.Item1)
                {
                    _update.Update(rtn.Item2["tabname"]);
                    sql = rtn.Item3;
                }
                else
                {
                    throw new Exception($"{HiSql.Constants.HiSqlSyntaxError}语句[{sql}]附近有语法错误,未能识别的表名");
                }

                #endregion

                #region 解析set 
                rtn=Tool.RegexGrpOrReplace(Constants.REG_UPATE_SET,sql);
                if (rtn.Item1)
                {
                    sql = rtn.Item3;
                    List<int> lstnum = new List<int> {
                            sql.IndexOf(" join ",StringComparison.OrdinalIgnoreCase),
                            sql.LastIndexOf(" where ", StringComparison.OrdinalIgnoreCase),
                        };
                    int _pos_idx = 0;

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
              
                    string _setsql = sql;
                    if (_pos_idx > 0)
                        _setsql = sql.Substring(0, _pos_idx);
                    else
                        _setsql = sql;

                    string[] _setparams = _setsql.Split(',');
                    if (_setparams.Length > 0)
                    {
                        foreach (string param in _setparams)
                        {
                            if (string.IsNullOrEmpty(param))
                            {
                                throw new Exception($"{HiSql.Constants.HiSqlSyntaxError}语句[{sql}]附近有语法错误,[,]后无参数");
                            }
                            _update.Set(_setparams);
                        }
                    }
                    else
                        throw new Exception($"{HiSql.Constants.HiSqlSyntaxError}语句[{sql}]附近有语法错误,未找到set参数 至少指定一个");
                }
                else
                    throw new Exception($"{HiSql.Constants.HiSqlSyntaxError}语句[{sql}]附近有语法错误,未找到set 命令");

                #endregion

                #region join 相关
                if (!string.IsNullOrEmpty(sql.Trim()))
                {
                    var dicrtn = Tool.RegexGrp(Constants.REG_UPATE_JOIN, sql);
                    if (dicrtn.Count > 0)
                    {
                        //说明是join

                        sql = parseJoin(sql);
                    }
                }

                #endregion
                #region where

                if (!string.IsNullOrEmpty(sql.Trim()))
                {
                    rtn = Tool.RegexGrpOrReplace(Constants.REG_UPDATE_WHERE, sql);
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



                        if (sql.LastIndexOf(')') > _pos_idx)
                        {
                            //说明都是子语语句
                            var rtndic = Tool.RegexGrp(Constants.REG_UPDATE_WHERE2, sql);
                            if (rtndic.Count > 0)
                            {
                                if (rtndic["where"].LastIndexOf(" order ", StringComparison.OrdinalIgnoreCase) > 0 || rtndic["where"].LastIndexOf($" order{System.Environment.NewLine}", StringComparison.OrdinalIgnoreCase) > 0)
                                {
                                    throw new Exception($"{HiSql.Constants.HiSqlSyntaxError} 子查询语句[{rtndic["where"]}]不允许[order by]排序 ");
                                }
                                else
                                {
                                    _update.Where(rtndic["where"]);
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

                            if (_pos_idx > 0)
                                _wheresql = sql.Substring(0, _pos_idx);
                            var rtndic = Tool.RegexGrp(Constants.REG_UPDATE_WHERE2, _wheresql);
                            if (rtndic.Count > 0)
                            {
                                if (rtndic["where"].LastIndexOf(" order ", StringComparison.OrdinalIgnoreCase) > 0 || rtndic["where"].LastIndexOf($" order{System.Environment.NewLine}", StringComparison.OrdinalIgnoreCase) > 0)
                                {
                                    throw new Exception($"{HiSql.Constants.HiSqlSyntaxError} 子查询语句[{rtndic["where"]}]不允许[order by]排序 ");
                                }
                                else
                                {
                                    _update.Where(rtndic["where"]);
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




                    }
                }
                #endregion
            }
            else
            {
                throw new Exception($"{HiSql.Constants.HiSqlSyntaxError}语句[{sql}] 非更新语言");
            }
        }




        string parseJoin(string sql)
        {
            if (!string.IsNullOrEmpty(sql))
            {
                var rtn = Tool.RegexGrpOrReplace(Constants.REG_UPATE_JOIN, sql);
                if (rtn.Item1)
                {
                    //说明是join
                    JoinType _joinType = JoinType.Inner;
                    string _tabjoin = rtn.Item2["table"].ToString();
                    sql = rtn.Item3;

                    if (Tool.RegexMatch(Constants.REG_UPDATE_INNERJOIN, rtn.Item2["join"]))
                        _joinType = JoinType.Inner;
                    else if (Tool.RegexMatch(Constants.REG_UPDATE_LEFTJOIN, rtn.Item2["join"]))
                        _joinType = JoinType.Left;
                    else if (Tool.RegexMatch(Constants.REG_UPDATE_RIGHTJOIN, rtn.Item2["join"]))
                        _joinType = JoinType.Right;


                    //if (!string.IsNullOrEmpty(rtn.Item2["asname"].ToString()))
                    //{
                    //    _update.Join(_tabjoin, _joinType).As(rtn.Item2["asname"].ToString());
                    //}
                    //else
                    //    _update.Join(_tabjoin, _joinType);

                    sql = parseJoinOn(sql);

                    sql = parseJoin(sql);


                }
            }
            return sql;
        }

        string parseJoinOn(string sql)
        {
            if (!string.IsNullOrEmpty(sql))
            {
                // 注释于2022.6.8  新增 join on 支持Hisql语法

                

                int _pos_idx = 0;
                //获取最外层的 关键词位置顺序 可能子语句中也可能包括关键字
                List<int> lstnum = new List<int> {
                    sql.LastIndexOf(" left ", StringComparison.OrdinalIgnoreCase),
                    //sql.LastIndexOf($" left{System.Environment.NewLine}",StringComparison.OrdinalIgnoreCase),
                    sql.LastIndexOf(" inner ", StringComparison.OrdinalIgnoreCase), 
                    //sql.LastIndexOf($" inner{System.Environment.NewLine}",StringComparison.OrdinalIgnoreCase), 
                    sql.LastIndexOf(" join ",StringComparison.OrdinalIgnoreCase), 
                    //sql.LastIndexOf($" join{System.Environment.NewLine}",StringComparison.OrdinalIgnoreCase), 
                    sql.LastIndexOf(" where ", StringComparison.OrdinalIgnoreCase), 
                    //sql.LastIndexOf($" where{System.Environment.NewLine}", StringComparison.OrdinalIgnoreCase), 
                    sql.LastIndexOf(" order ", StringComparison.OrdinalIgnoreCase), 
                    //sql.LastIndexOf($" order{System.Environment.NewLine}",StringComparison.OrdinalIgnoreCase), 
                    sql.LastIndexOf(" group ", StringComparison.OrdinalIgnoreCase), 
                    //sql.LastIndexOf($" group{System.Environment.NewLine}",StringComparison.OrdinalIgnoreCase), 
                    sql.LastIndexOf(" having ", StringComparison.OrdinalIgnoreCase), 
                    //sql.LastIndexOf($" having{System.Environment.NewLine}",StringComparison.OrdinalIgnoreCase), 
                    sql.LastIndexOf(" union ", StringComparison.OrdinalIgnoreCase)
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
                    
                    
                    
                    
                    //_update.On(_wheresql);

                }
                else
                {

                    //_update.On(_wheresql);

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
