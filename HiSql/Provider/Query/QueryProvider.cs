using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{

    /// <summary>
    /// 查询语句解析核心类
    /// author tgm email:tansar@126.com
    /// 
    /// </summary>
    public partial class QueryProvider : IQuery
    {
        TableDefinition _table;
        FieldDefinition _field;

        Filter _where;
        Having _having;



        List<FilterDefinition> _list_filter = new List<FilterDefinition>();

        List<FieldDefinition> _list_field = new List<FieldDefinition>();

        List<JoinDefinition> _list_join = new List<JoinDefinition>();

        List<SortByDefinition> _list_sort = new List<SortByDefinition>();
        List<GroupDefinition> _list_group = new List<GroupDefinition>();

        List<HavingDefinition> _list_having = new List<HavingDefinition>();

        List<HiColumn> _list_column = new List<HiColumn>();


        List<string> _list_rank = new List<string>();

        List<CaseDefinition> _currcase = new List<CaseDefinition>();


        /// <summary>
        /// 是否忽略表锁
        /// </summary>
        bool _withnolock = false;


        /// <summary>
        /// 查询结果插入的表
        /// </summary>
        string _itabname = string.Empty;

        JoinDefinition _currjoin;
        JoinOnFilterDefinition _joinon;
        SortByDefinition _sort;
        GroupDefinition _group;

        LockMode _withCurLock;

        List<IQuery> _querylist = new List<IQuery>();

        bool _isMultiSubQuery = false;

        IDbConfig dbConfig;

        /// <summary>
        /// 统计需要使用的表
        /// </summary>
        List<TableDefinition> _list_table = new List<TableDefinition>();

        /// <summary>
        /// 当触发了Take方法时就会为true
        /// </summary>
        bool _ispage = false;//是否分页
        //分页是要获取当前查询的总数的sql语句
        string _pagetotalsql = string.Empty;

        SynTaxQueue _queue = new SynTaxQueue();

        int _currpage = 0;
        int _pagesize = 100;


        /// <summary>
        /// 返回插入的表名
        /// </summary>
        public string ITabName
        {
            get { return _itabname; }
        }

        public virtual IDbConfig DbConfig
        {
            get { return dbConfig; }
            set { dbConfig = value; }
        }


        /// <summary>
        /// 是否是多表子查询
        /// </summary>
        public bool IsMultiSubQuery
        {
            get { return _isMultiSubQuery; }
            set { _isMultiSubQuery = value; }
        }

        public List<IQuery> SubQuery
        {
            get { return _querylist; }

        }

        public List<string> Ranks
        {
            get { return _list_rank; }
            set { _list_rank = value; }
        }

        /// <summary>
        /// 查询的主表
        /// </summary>
        public TableDefinition Table
        {
            get { return _table; }
        }
        /// <summary>
        /// 查询的结果字段
        /// </summary>
        public List<FieldDefinition> Fields
        {
            get { return _list_field; }
        }
        /// <summary>
        /// 表关联信息
        /// </summary>
        public List<JoinDefinition> Joins
        {
            get { return _list_join; }
        }

        //分组
        public List<GroupDefinition> Groups
        {
            get { return _list_group; }
        }

        //排序
        public List<SortByDefinition> Sorts
        {
            get { return _list_sort; }
        }

        //过滤条件
        public List<FilterDefinition> Wheres
        {
            get { return _list_filter; }
        }

        /// <summary>
        /// 返回
        /// </summary>
        public Filter Filters
        {
            get { return _where; }
        }
        /// <summary>
        /// having
        /// </summary>
        public Having Havings
        {
            get { return _having; }
        }

        /// <summary>
        /// 返回结果字段结构信息
        /// </summary>
        public List<HiColumn> ResultColumn
        {
            get { return _list_column; }
            set { _list_column = value; }
        }

        public bool IsPage
        {
            get { return _ispage; }
        }


        public string PageTotalSql
        {
            get { return _pagetotalsql; }
            set { _pagetotalsql = value; }
        }

        public int CurrentPage
        {
            get { return _currpage; }
        }
        public int PageSize
        {
            get { return _pagesize; }
        }

        /// <summary>
        /// 是否忽略表锁
        /// </summary>
        public bool IsNoLock
        {
            get { return _withnolock; }
        }

        /// <summary>
        /// 表锁模式
        /// </summary>
        public LockMode WithLockMode
        {
            get { return _withCurLock; }
        }

        /// <summary>
        /// 返回SQL的请清单
        /// </summary>
        public List<TableDefinition> TableList
        {
            get
            {
                List<TableDefinition> _list = new List<TableDefinition>();
                foreach (TableDefinition _tab in _list_table)
                {
                    if (_list.Where(t => t.AsTabName == _tab.TabName).Count() == 0)
                    {
                        _list.Add(_tab);
                    }
                }
                return _list;


            }
        }


        public HiSqlProvider Context { get; set; }

        public QueryProvider()
        {

        }

        public IQuery Query(params IQuery[] query)
        {
            if (string.IsNullOrEmpty(_queue.LastQueue()))
            {
                if (query != null && query.Length > 0)
                {
                    _queue.Add("subtable");

                    foreach (IQuery _q in query)
                    {
                        _querylist.Add(_q);
                    }

                    _isMultiSubQuery = true;//表示当前查询是多表子查询
                }
                else
                    throw new Exception($"使用子查询时未指定对应的子查询");
            }
            else
                throw new Exception($"指定查询表必须在初始就指定");
            return this;
        }

        /// <summary>
        /// 执行hisql
        /// </summary>
        /// <param name="hisql"></param>
        /// <returns></returns>
        public IQuery HiSql(string hisql, IQuery query)
        {
            //编译hisql
            AST.SelectParse selectParse = new AST.SelectParse(hisql, query);
            return selectParse.Query;
        }

        /// <summary>
        /// 查询表
        /// </summary>
        /// <param name="tabname">表名</param>
        /// <returns></returns>
        public IQuery Query(string tabname)
        {
            if (string.IsNullOrEmpty(_queue.LastQueue()))
            {
                if (Tool.CheckTabName(tabname).Item1 == true)
                {
                    Dictionary<string, string> _dic = Tool.RegexGrp(Constants.REG_TABNAME, tabname);

                    _table = new TableDefinition();
                    _table.Schema = Context.CurrentConnectionConfig.Schema == null ? "" : Context.CurrentConnectionConfig.Schema;
                    _table.TabName = tabname;
                    _table.DbServer = Context.CurrentConnectionConfig.DbServer;

                    switch (_table.TableType)
                    {
                        case TableType.Local:
                            _table.AsTabName = $"lcl_{_dic["tab"].ToString()}";//本地临时表
                            break;
                        case TableType.Global:
                            _table.AsTabName = $"glo_{_dic["tab"].ToString()}";//全局临时表
                            break;
                        case TableType.Var:
                            _table.AsTabName = $"var_{_dic["tab"].ToString()}";//变量表
                            break;
                        default:
                            _table.AsTabName = tabname;
                            break;

                    }

                    mergeTable(_table);
                    _queue.Add("table");
                }
                else
                    throw new Exception($"指定的表名[{tabname}]不符合语法规则");
            }
            else
                throw new Exception($"指定查询表必须在初始就指定");
            return this;
        }

        /// <summary>
        /// 指定查询表
        /// </summary>
        /// <param name="tabname">表名</param>
        /// <param name="rename">指定表的别名</param>
        /// <returns></returns>
        public IQuery Query(string tabname, string rename)
        {
            if (string.IsNullOrEmpty(_queue.LastQueue()))
            {
                if (Tool.CheckTabName(tabname).Item1 == true && Tool.CheckFieldName(rename).Item1 == true)
                {
                    _table = new TableDefinition();
                    _table.Schema = Context.CurrentConnectionConfig.Schema;
                    _table.TabName = tabname;
                    _table.DbServer = Context.CurrentConnectionConfig.DbServer;
                    _table.AsTabName = rename;
                    mergeTable(_table);
                    _queue.Add("table|rename");
                }
                else
                    throw new Exception($"指定的表名[{tabname}]或别名[{rename}]不符合语法规则");
            }
            else
                throw new Exception($"指定查询表必须在初始就指定");

            return this;
        }

        /// <summary>
        /// 支持多字段 
        /// "UserName","Age"...
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public IQuery Field(params string[] fields)
        {
            if (fields != null && fields.Length > 0)
            {
                foreach (string f in fields)
                {
                    FieldDefinition _fieldd = new FieldDefinition(f);
                    _fieldd.Schema = Context.CurrentConnectionConfig.Schema;
                    _fieldd.DbServer = Context.CurrentConnectionConfig.DbServer;
                    _fieldd.IsVirtualFeild = this.IsMultiSubQuery;//是否多表子查询

                    //当没有指定表时默认是 Query的表
                    if (string.IsNullOrEmpty(_fieldd.TabName) && !_fieldd.IsVirtualFeild)
                    {
                        _fieldd.TabName = _table.TabName;
                        _fieldd.AsTabName = _table.AsTabName;
                    }
                    if (!_fieldd.IsVirtualFeild)
                    {
                        //mergeTable((TableDefinition)_fieldd);
                    }
                    _list_field.Add(_fieldd);
                }
                _queue.Add("field");
            }
            else
                throw new Exception($"查询的字段不能为空");

            return this;
        }

        /// <summary>
        /// 重命名表
        /// </summary>
        /// <param name="retabname"></param>
        /// <returns></returns>
        public IQuery As(string retabname)
        {
            if (Tool.CheckFieldName(retabname).Item1)
            {
                if (_queue.LastQueue().IndexOf("table") >= 0)
                {
                    _table.AsTabName = retabname;
                    _queue.Add("as");
                }
                else if (_queue.LastQueue() == "join")
                {
                    _currjoin.Right.AsTabName = retabname;
                }
                else
                {
                    throw new Exception("[As]方法只能在 在类初始化后面或已经对表进行了重命名");
                }
            }
            else
                throw new Exception($"别名[{retabname}]不符合语法规则");
            return this;
        }

        /// <summary>
        /// 单个分组指定
        /// </summary>
        /// <param name="grp"></param>
        /// <returns></returns>
        public IQuery Group(GroupDefinition grp)
        {
            if (_queue.HasQueue("field"))
            {
                if (!_queue.HasQueue("sort"))
                {
                    _queue.Add("group");

                    if (string.IsNullOrEmpty(grp.Field.TabName) && !this.IsMultiSubQuery)
                    {
                        grp.Field.TabName = _table.TabName;
                        grp.Field.AsTabName = _table.AsTabName;

                    }
                    grp.Field.IsVirtualFeild = this.IsMultiSubQuery;


                    _list_group.Add(grp);
                }
                else
                {
                    throw new Exception($"[GroupBy]方法只能在Sort之前");
                }


            }
            else throw new Exception($"[GroupBy]方法不能在Field之前");

            return this;
        }

        /// <summary>
        /// 分组 多个参数分组 param参数
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public IQuery Group(params string[] group)
        {
            GroupBy groups = new GroupBy();
            foreach (string n in group)
            {
                groups.Add(n);
            }
            return Group(groups);
        }

        /// <summary>
        /// 分组 多个结构化分组
        /// </summary>
        /// <param name="grp"></param>
        /// <returns></returns>
        public IQuery Group(GroupBy grp)
        {
            if (grp != null && grp.Elements.Count > 0)
            {
                if (_queue.HasQueue("field"))
                {
                    if (!_queue.HasQueue("sort"))
                    {
                        _queue.Add("group");
                        _list_group = grp.Elements;
                        foreach (GroupDefinition groupDefinition in _list_group)
                        {
                            groupDefinition.Field.IsVirtualFeild = this.IsMultiSubQuery;
                            if (string.IsNullOrEmpty(groupDefinition.Field.TabName) && !this.IsMultiSubQuery)
                            {
                                groupDefinition.Field.TabName = _table.TabName;
                                groupDefinition.Field.AsTabName = _table.AsTabName;

                            }
                        }
                    }
                    else
                    {
                        throw new Exception($"[GroupBy]方法只能在Sort之前");
                    }


                }
                else throw new Exception($"[GroupBy]方法不能在Field之前");
            }
            else
                throw new Exception($"指定的join on不能为空");
            return this;
        }


        public IQuery Having(string having)
        {
            if (_queue.LastQueue() == "group")
            {
                _having = new Having(having);
            }
            else
                throw new Exception("[Having]必须在Group之后");




            return this;
        }
        public IQuery Having(Having havings)
        {
            if (_queue.LastQueue() == "group")
            {


                if (havings == null || havings.Elements.Count == 0)
                {
                    throw new Exception($"[Having]未指定过滤条件");
                }
                else
                {

                    _list_having = havings.Elements;
                    foreach (HavingDefinition havingDefinition in _list_having)
                    {
                        if (string.IsNullOrEmpty(havingDefinition.Field.TabName) && !this.IsMultiSubQuery)
                        {
                            havingDefinition.Field.TabName = _table.TabName;
                            havingDefinition.Field.AsTabName = _table.AsTabName;
                        }
                    }
                }


            }
            else
                throw new Exception("[Having]必须在Group之后");

            return this;
        }

        /// <summary>
        /// 指定表关联
        /// </summary>
        /// <param name="join"></param>
        /// <returns></returns>
        public IQuery Join(JoinDefinition join)
        {
            if (join != null)
            {
                _currjoin = join;

                _queue.Add("join");
            }
            else
                throw new Exception($"不能将Join指定为null");

            return this;
        }
        /// <summary>
        /// 指定关联表
        /// </summary>
        /// <param name="tabname">关联的表名</param>
        /// <param name="retabname">重命名表</param>
        /// <param name="joinType">关联类型 默认inner</param>
        /// <returns></returns>
        public IQuery Join(string tabname, string retabname, JoinType joinType = JoinType.Inner)
        {
            _currjoin = new JoinDefinition(tabname, retabname);
            _currjoin.JoinType = joinType;
            _queue.Add("join|rename");
            return this;
        }
        /// <summary>
        /// 指定关联表
        /// </summary>
        /// <param name="tabname">关联的表名</param>
        /// <param name="joinType">关联的类型 默认inner</param>
        /// <returns></returns>
        public IQuery Join(string tabname, JoinType joinType = JoinType.Inner)
        {
            _currjoin = new JoinDefinition(tabname);
            _currjoin.JoinType = joinType;
            _queue.Add("join");
            return this;
        }

        public IQuery On(JoinOn joinon)
        {
            if (joinon != null && joinon.Elements.Count > 0)
            {
                if (_queue.LastQueue().IndexOf("join") >= 0)
                {
                    _queue.Add("on");
                    _currjoin.JoinOn = joinon.Elements;
                    _list_join.Add(_currjoin);
                    if (_currjoin.Left != null)
                        mergeTable(_currjoin.Left);

                    if (_currjoin.Right != null)
                        mergeTable(_currjoin.Right);
                    _currjoin = null;
                }
                else if (_queue.LastQueue() == "as")
                {
                    if (_queue.LastQueue(-1).IndexOf("join") >= 0)
                    {
                        _queue.Add("on");
                        _currjoin.JoinOn = joinon.Elements;
                        _list_join.Add(_currjoin);
                        _currjoin = null;
                    }
                    else
                        throw new Exception($"[On]方法只能在[Join]方法后面");
                }
                else throw new Exception($"[On]方法只能在[Join]方法后面");
            }
            else
                throw new Exception($"指定的join on不能为空");
            return this;
        }
        public IQuery On(string leftCondition, string rightCcondition)
        {
            if (!string.IsNullOrEmpty(leftCondition) && !string.IsNullOrEmpty(rightCcondition))
            {
                if (_queue.LastQueue().IndexOf("join") >= 0)
                {
                    _queue.Add("on");
                    _currjoin.JoinOn.Add(new JoinOnFilterDefinition(leftCondition, rightCcondition));
                    _list_join.Add(_currjoin);
                    if (_currjoin.Left != null)
                        mergeTable(_currjoin.Left);

                    if (_currjoin.Right != null)
                        mergeTable(_currjoin.Right);
                    _currjoin = null;

                }
                else if (_queue.LastQueue() == "as")
                {
                    if (_queue.LastQueue(-1).IndexOf("join") >= 0)
                    {
                        _queue.Add("on");
                        _currjoin.JoinOn.Add(new JoinOnFilterDefinition(leftCondition, rightCcondition));
                        _list_join.Add(_currjoin);
                        if (_currjoin.Left != null)
                            mergeTable(_currjoin.Left);

                        if (_currjoin.Right != null)
                            mergeTable(_currjoin.Right);
                        _currjoin = null;
                    }
                    else
                        throw new Exception($"[On]方法只能在[Join]方法后面");
                }
                else throw new Exception($"[On]方法只能在[Join]方法后面");

            }
            else
                throw new Exception($"On 的连接条件[{leftCondition}] 和[{rightCcondition}]不能为空");
            return this;
        }
        public IQuery On(string condition)
        {
            if (!string.IsNullOrEmpty(condition))
            {
                if (_queue.LastQueue().IndexOf("join") >= 0)
                {
                    _queue.Add("on");
                    _currjoin.JoinOn.Add(new JoinOnFilterDefinition(condition));
                    _list_join.Add(_currjoin);
                    if (_currjoin.Left != null)
                        mergeTable(_currjoin.Left);

                    if (_currjoin.Right != null)
                        mergeTable(_currjoin.Right);
                    _currjoin = null;
                }
                else if (_queue.LastQueue() == "as")
                {
                    if (_queue.LastQueue(-1).IndexOf("join") >= 0)
                    {
                        _queue.Add("on");
                        _currjoin.JoinOn.Add(new JoinOnFilterDefinition(condition));
                        _list_join.Add(_currjoin);
                        if (_currjoin.Left != null)
                            mergeTable(_currjoin.Left);

                        if (_currjoin.Right != null)
                            mergeTable(_currjoin.Right);
                        _currjoin = null;
                    }
                    else
                        throw new Exception($"[On]方法只能在[Join]方法后面");
                }
                else throw new Exception($"[On]方法只能在[Join]方法后面");

            }
            else
                throw new Exception($"On 的连接条件[{condition}] 不能为空");
            return this;
        }

        /// <summary>
        /// 用结构化的方式定义多个排序
        /// </summary>
        /// <param name="sort"></param>
        /// <returns></returns>
        public IQuery Sort(SortBy sort)
        {
            if (sort != null)
            {
                if (!_queue.HasQueue("sort"))
                {
                    if (sort.Elements.Count > 0)
                    {
                        _list_sort = sort.Elements;
                        foreach (SortByDefinition sortByDefinition in _list_sort)
                        {
                            if (string.IsNullOrEmpty(sortByDefinition.Field.TabName) && !this.IsMultiSubQuery)
                            {
                                sortByDefinition.Field.TabName = _table.TabName;
                                sortByDefinition.Field.AsTabName = _table.AsTabName;
                            }
                        }
                        _queue.Add("sort");
                    }
                }
                else
                    throw new Exception($"已经指定过一次排序,不能重复指定[Sort]排序");
            }
            else
            {
                throw new Exception($"指定排序不能为Null");
            }

            return this;
        }

        /// <summary>
        /// 指定单个排序
        /// </summary>
        /// <param name="sort"></param>
        /// <returns></returns>
        public IQuery Sort(SortByDefinition sort)
        {
            if (sort != null)
            {
                if (!_queue.HasQueue("sort"))
                {
                    _list_sort = new List<SortByDefinition>();
                    if (string.IsNullOrEmpty(sort.Field.TabName) && !this.IsMultiSubQuery)
                    {
                        sort.Field.TabName = _table.TabName;
                        sort.Field.AsTabName = _table.AsTabName;
                    }
                    _list_sort.Add(sort);
                    _queue.Add("sort");
                }
                else
                    throw new Exception($"已经指定过一次排序,不能重复指定[Sort]排序");
            }
            else
            {
                throw new Exception($"指定排序不能为Null");
            }

            return this;
        }
        /// <summary>
        /// 排序 可以多个参数 params 参数
        /// </summary>
        /// <param name="sort"></param>
        /// <returns></returns>
        public IQuery Sort(params string[] sort)
        {
            SortBy sortby = new SortBy();
            foreach (string s in sort)
            {
                Dictionary<string, string> _dic_sort = Tool.RegexGrp(Constants.REG_SORT, s);
                if (_dic_sort.Count() > 0)
                {
                    if (string.IsNullOrEmpty(_dic_sort["field"].ToString()))
                        sortby.Add(_dic_sort["field"].ToString(), SortType.ASC);
                    else
                    {
                        sortby.Add(_dic_sort["field"].ToString(), _dic_sort["sort"].ToString().ToLower() == "asc" ? SortType.ASC : SortType.DESC);
                    }
                }
                else
                {
                    throw new Exception($"排序字符串[{s}]不符合语法规则");
                }
            }

            return Sort(sortby);
            //return this;
        }

        public virtual string GetDbName(string tabname)
        {
            return tabname;
        }
        /// <summary>
        /// 将查询结果写入指定的表（临时表，全局临时表，及已经定义好的变量表）
        /// </summary>
        /// <param name="tabname"></param>
        public virtual void Insert(string tabname)
        {
            if (!_queue.HasQueue("field"))
            {
                throw new Exception($"Insert操作必须基于查询结果");
            }
            else
            {
                _itabname = GetDbName(tabname);
                string _sql = this.ToSql();

                this.Context.DBO.ExecCommand(_sql, null);

            }


        }
        public virtual IQuery WithRank(DbRank rank, DbFunction dbFunction, string field, string asname, SortType sortType)
        {
            asname = asname.ToSqlInject();
            if (field.Trim() != "*" && !string.IsNullOrEmpty(field))
                field = $"{dbConfig.Field_Pre}{field}{dbConfig.Field_After}";
            switch (rank)
            {
                case DbRank.DENSERANK:
                    _list_rank.Add($"dense_rank() over( order by {dbFunction.ToString()}({field}) {sortType.ToString()}) as {asname}");
                    break;
                case DbRank.RANK:
                    _list_rank.Add($"rank() over( order by {dbFunction.ToString()}({field}) {sortType.ToString()}) as {asname}");
                    break;
                case DbRank.ROWNUMBER:
                    _list_rank.Add($"row_number() over( order by {dbFunction.ToString()}({field}) {sortType.ToString()}) as {asname}");
                    break;
                default:
                    break;
            }

            return this;
        }
        /// <summary>
        /// 忽略查询锁
        /// </summary>
        /// <returns></returns>
        public virtual IQuery WithLock(LockMode lockMode)
        {
            if (!_queue.HasQueue("lock"))
            {
                _withCurLock = lockMode;
                _withnolock = true;
                _queue.Add("lock");
            }
            else
                throw new Exception($"已经指定了[WithNoLock]不允许重复指定");
            return this;
        }
        public virtual IQuery WithRank(DbRank rank, Ranks ranks, string asname)
        {
            asname = asname.ToSqlInject();
            List<string> _lstorderby = new List<string>();
            foreach (RankDefinition rankDefinition in ranks.Elements)
            {
                rankDefinition.Field = rankDefinition.Field.ToSqlInject();
                if (rankDefinition.Field.Trim() != "*")
                    _lstorderby.Add($"{rankDefinition.DbFunction.ToString()}({dbConfig.Field_Pre}{rankDefinition.Field}{dbConfig.Field_After}) {rankDefinition.SortType.ToString()}");
                else
                    _lstorderby.Add($"{rankDefinition.DbFunction.ToString()}({rankDefinition.Field}) {rankDefinition.SortType.ToString()}");
            }


            switch (rank)
            {
                case DbRank.DENSERANK:
                    _list_rank.Add($"dense_rank() over( order by {string.Join(",", _lstorderby.ToArray()) }) as {asname}");
                    break;
                case DbRank.RANK:
                    _list_rank.Add($"rank() over( order by {string.Join(",", _lstorderby.ToArray())}) as {asname}");
                    break;
                case DbRank.ROWNUMBER:
                    _list_rank.Add($"row_number() over( order by {string.Join(",", _lstorderby.ToArray())}) as {asname}");
                    break;
                default:
                    break;
            }
            return this;
        }

        /// <summary>
        /// 显示第几页的数据
        /// </summary>
        /// <param name="currpage"></param>
        /// <returns></returns>
        public IQuery Skip(int currpage)
        {
            this._currpage = currpage;
            return this;
        }

        /// <summary>
        /// 页大小(显示最大记录数)
        /// </summary>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public IQuery Take(int pagesize)
        {
            this._pagesize = pagesize;
            this._ispage = true;
            return this;
        }

        public IQuery Where(Filter where)
        {
            if (!_queue.HasQueue("where"))
            {
                if (where != null && where.Elements.Count > 0)
                {
                    if (!where.IsBracketOk)
                        throw new Exception("指定了括号[(,)] 但没有成对出现");

                    _list_filter = where.Elements;
                    foreach (FilterDefinition filterDefinition in _list_filter)
                    {
                        if (string.IsNullOrEmpty(filterDefinition.Field.TabName) && !this.IsMultiSubQuery)
                        {
                            filterDefinition.Field.TabName = _table.TabName;
                            filterDefinition.Field.AsTabName = _table.AsTabName;
                        }
                    }

                    if (_queue.Queue.Where(q => q == "where").Count() == 0)
                        _queue.Add("where");
                    else
                        throw new Exception($"不允许多次指定where条件");
                }
                _where = where;

            }
            else
                throw new Exception($"已经指定了一个Where 不允许重复指定");
            return this;
        }

        /// <summary>
        /// 支持Hisql 中间语言的sql条件
        /// </summary>
        /// <param name="sqlwhere"></param>
        /// <returns></returns>
        public IQuery Where(string sqlwhere)
        {
            //需要检测语法
            if (!_queue.HasQueue("where"))
            {
                if (string.IsNullOrEmpty(sqlwhere.Trim()))
                {
                    throw new Exception($"指定的hisql where语句[{sqlwhere}]为空");
                }

                Filter where = new Filter() { sqlwhere };

                _where = where;
                _queue.Add("where");
            }
            else
                throw new Exception($"已经指定了一个Where 不允许重复指定");
            return this;
        }

        public virtual string ToSql()
        {

            return "";
        }



        public  Task<List<ExpandoObject>> ToEObjectAsync(ref int total)
        {
            string _sql = this.ToSql();
            total = 0;
            if (this.IsPage && !string.IsNullOrEmpty(this.PageTotalSql.ToString().Trim()))
            {
                var obj = this.Context.DBO.ExecScalar(this.PageTotalSql.ToString());
                total = Convert.ToInt32(obj.ToString());
            }
            var dr =  this.Context.DBO.GetDataReaderAsync(_sql, null);
            var _result = DataConvert.ToEObjectSync(dr);
            return _result;
        }


        public async Task<List<ExpandoObject>> ToEObjectAsync()
        {
            string _sql = this.ToSql();
            IDataReader dr = await this.Context.DBO.GetDataReaderAsync(_sql, null);
            List<ExpandoObject> _result = DataConvert.ToEObject(dr);
            dr.Close();
            return _result;
        }


        public List<ExpandoObject> ToEObject()
        {
            return ToEObjectAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }



        public List<T> ToList<T>()
        {
            string _sql = this.ToSql();
            IDataReader dr = this.Context.DBO.GetDataReader(_sql, null);
            List<T> _result = DataConvert.ToList<T>(dr);
            dr.Close();
            return _result;
        }




        public List<T> ToList<T>(ref int total)
        {
            string _sql = this.ToSql();
            total = 0;
            if (this.IsPage && !string.IsNullOrEmpty(this.PageTotalSql.ToString().Trim()))
            {
                var obj = this.Context.DBO.ExecScalar(this.PageTotalSql.ToString());

                total = Convert.ToInt32(obj.ToString());

            }
            IDataReader dr = this.Context.DBO.GetDataReader(_sql, null);
            List<T> _result = DataConvert.ToList<T>(dr);
            dr.Close();
            return _result;
        }
        public DataTable ToTable()
        {
            string _sql = this.ToSql();
            return this.Context.DBO.GetDataTable(_sql, null);
        }
        public DataTable ToTable(ref int total)
        {
            string _sql = this.ToSql();
            total = 0;
            if (this.IsPage && !string.IsNullOrEmpty(this.PageTotalSql.ToString().Trim()))
            {
                var obj = this.Context.DBO.ExecScalar(this.PageTotalSql.ToString());

                total = Convert.ToInt32(obj.ToString());

            }


            return this.Context.DBO.GetDataTable(_sql, null);
        }
        public List<TDynamic> ToDynamic()
        {
            string _sql = this.ToSql();


            IDataReader dr = this.Context.DBO.GetDataReader(_sql, null);
            List<TDynamic> result = DataConvert.ToDynamic(dr);
            dr.Close();
            return result;
        }
        public List<TDynamic> ToDynamic(ref int total)
        {
            string _sql = this.ToSql();
            total = 0;
            if (this.IsPage && !string.IsNullOrEmpty(this.PageTotalSql.ToString().Trim()))
            {
                var obj = this.Context.DBO.ExecScalar(this.PageTotalSql.ToString());

                total = Convert.ToInt32(obj.ToString());

            }
            IDataReader dr = this.Context.DBO.GetDataReader(_sql, null);
            List<TDynamic> result = DataConvert.ToDynamic(dr);
            dr.Close();
            return result;

        }
        public string ToJson()
        {
            string _sql = this.ToSql();
            IDataReader dr = this.Context.DBO.GetDataReader(_sql, null);
            List<ExpandoObject> lstobj = DataConvert.ToEObject(dr);
            return JsonConvert.SerializeObject(lstobj);
        }
        public string ToJson(ref int total)
        {
            string _sql = this.ToSql();
            total = 0;
            if (this.IsPage && !string.IsNullOrEmpty(this.PageTotalSql.ToString().Trim()))
            {
                var obj = this.Context.DBO.ExecScalar(this.PageTotalSql.ToString());

                total = Convert.ToInt32(obj.ToString());

            }
            IDataReader dr = this.Context.DBO.GetDataReader(_sql, null);
            List<ExpandoObject> lstobj = DataConvert.ToEObject(dr);
            return JsonConvert.SerializeObject(lstobj);
        }

        /// <summary>
        /// 添加新表到当前连接查询中
        /// 主要用于动态hisql解析执行时发现有新的表将其添加进来
        /// </summary>
        /// <param name="table"></param>
        //public void MergeTable(TableDefinition table)
        //{
        //    mergeTable(table);
        //}


        void mergeTable(TableDefinition table)
        {
            if (table != null)
            {
                // && t.DbServer == table.DbServer
                // && t.Schema == table.Schema
                if (_list_table.Where(t => t != null && (((t.TabName == table.TabName && t.AsTabName == table.AsTabName) || (t.AsTabName == table.TabName && table.TabName == table.AsTabName)))).Count() == 0)
                {
                    //表示不存在 则添加
                    _list_table.Add(table);
                }
            }

        }
        /// <summary>
        /// case 样例查询
        /// sqlClient
        ///.Query("UserList")
        ///.Field("UNAME","age")
        ///.Case("age")
        ///.When("age>10").Then("bt10")
        ///.When("age>20").Then("bt20")
        ///.EndAs("ageRange",typeof(int))
        ///.Field("descrip");
        /// </summary>
        public IQuery Case(string fieldname)
        {


            if (_queue.LastQueue() == "case")
            {
                throw new Exception($"不能重复指定[Case]方法");
            }

            if (Tool.CheckField(fieldname).Item1)
            {
                _queue.Add("case");

                if (_currcase.Count == 0)
                {
                    CaseDefinition caseDefinition = new CaseDefinition(fieldname);
                    _currcase.Add(caseDefinition);
                }
                else
                {
                    throw new Exception($"上一个case未结束无法使用新的Case");
                }

            }
            else
            {
                throw new Exception($"字段名[{fieldname}]不符合语法规则");
            }
            return this;
        }


        public IQuery When(string fieldexpress)
        {
            //检测case
            if (_currcase.Count != 1)
                throw new Exception($"[When]方法只能先开始[Case]");



            if (_queue.LastQueue() != "case" && _queue.LastQueue() != "then")
            {
                throw new Exception($"[When]方法只能在[Case]或[Then]后面");
            }
            else
            {
                _queue.Add("when");
                WhenDefinition whenDefinition = new WhenDefinition(fieldexpress);
                whenDefinition.Field = _currcase[0].CaseField;
                _currcase[0].WhenList.Add(whenDefinition);
            }
            return this;
        }

        public IQuery Then(string thenvalue)
        {
            //then只能在 When后面 否则将会报错

            if (_currcase.Count != 1)
                throw new Exception($"[Then]方法只能先开始[Case]");

            if (_queue.LastQueue() != "when")
            {
                throw new Exception($"[Then]方法只能在[When]后面");
            }
            else
            {
                _queue.Add("then");
                if (_currcase[0].WhenList.Count > 0)
                {
                    ThenDefinition thenDefinition = new ThenDefinition(thenvalue);
                    _currcase[0].WhenList[_currcase[0].WhenList.Count - 1].Then = thenDefinition;
                }
                else
                {
                    throw new Exception($"[Then]方法只能在[When]后面");
                }
            }
            return this;
        }
        public IQuery Else(string elsevalue)
        {
            //else 只能在then后面

            if (_currcase.Count != 1)
                throw new Exception($"[Else]方法只能先开始[Case]");

            if (_queue.LastQueue() != "then")
            {
                throw new Exception($"[Else]方法只能在[Then]后面");
            }
            else
            {
                _queue.Add("else");
                ElseDefinition elseDefinition = new ElseDefinition(elsevalue);
                _currcase[0].Else = elseDefinition;
            }
            return this;
        }
        public IQuery EndAs(string asfieldname, Type type)
        {
            if (_currcase.Count != 1)
                throw new Exception($"[Else]方法只能先开始[Case]");
            //EndAs 只能在 Else 或 Then 后面
            if (_queue.LastQueue() != "else" && _queue.LastQueue() != "then")
            {
                throw new Exception($"[EndAs]方法只能在[Else]或[Then]后面");
            }
            else
            {
                _queue.Add("endas");
                EndAsDefinition endAsDefinition = new EndAsDefinition(asfieldname, type);
                _currcase[0].EndAs = endAsDefinition;


                FieldDefinition casefieldDefinition = new FieldDefinition(asfieldname);
                casefieldDefinition.Case = _currcase[0];

                _list_field.Add(casefieldDefinition);
                //清空当前case
                _currcase = new List<CaseDefinition>();


            }
            return this;
        }
        public IQuery EndAs(string asfieldname)
        {
            if (_currcase.Count != 1)
                throw new Exception($"[Else]方法只能先开始[Case]");
            if (_queue.LastQueue() != "else" && _queue.LastQueue() != "then")
            {
                throw new Exception($"[EndAs]方法只能在[Else]或[Then]后面");
            }
            else
            {
                _queue.Add("endas");
                EndAsDefinition endAsDefinition = new EndAsDefinition(asfieldname);
                _currcase[0].EndAs = endAsDefinition;

                FieldDefinition casefieldDefinition = new FieldDefinition(asfieldname);
                casefieldDefinition.Case = _currcase[0];
                _list_field.Add(casefieldDefinition);

                //清空当前case
                _currcase = new List<CaseDefinition>();
            }
            return this;
        }


    }


}
