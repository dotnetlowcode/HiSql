using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public class PostGreSqlQuery : QueryProvider
    {
        /*
         将查询进行分块
        1.select [field]
        2.from [table] [join]
        3.where [where]
        4.orderby [orderby]
        5.group
        6.having
         */

        Dictionary<string, TabInfo> dictabinfo = new Dictionary<string, TabInfo>(StringComparer.OrdinalIgnoreCase);
        StringBuilder sb_table = new StringBuilder();
        StringBuilder sb_field = new StringBuilder();
        StringBuilder sb_field_result = new StringBuilder();
        StringBuilder sb_join = new StringBuilder();
        StringBuilder sb_where = new StringBuilder();
        StringBuilder sb_sort = new StringBuilder();
        StringBuilder sb_group = new StringBuilder();
        StringBuilder sb_having = new StringBuilder();
        StringBuilder sb_subquery = new StringBuilder();
        StringBuilder sb_distinct = new StringBuilder();
        IDbConfig dbConfig = new PostGreSqlConfig();
        public override IDbConfig DbConfig
        {
            get { return dbConfig; }
        }

        public PostGreSqlQuery() : base()
        {

        }


        public override string ToSql()
        {
            sb_table = new StringBuilder();
            sb_field = new StringBuilder();
            sb_field_result = new StringBuilder();
            sb_join = new StringBuilder();
            sb_where = new StringBuilder();
            sb_sort = new StringBuilder();
            sb_group = new StringBuilder();
            sb_having = new StringBuilder();
            sb_distinct = new StringBuilder();
            //如果有子查询应该先把子查询中的SQL语句先生成
            if (this.IsMultiSubQuery)
            {
                int _idx = 0;
                foreach (IQuery _q in this.SubQuery)
                {
                    sb_subquery.Append(_q.ToSql());
                    if (_idx < this.SubQuery.Count - 1)
                        sb_subquery.AppendLine("UNION all");

                    _idx++;
                }
            }

            checkData();
            StringBuilder sb = new StringBuilder();
            StringBuilder sb_total = new StringBuilder();
            if (!this.IsMultiSubQuery)
            {
                if (this.IsPage)
                {
                    if (this.IsDistinct)
                        throw new Exception("不允许分页情况下使用distinct");
                    if (!string.IsNullOrEmpty(sb_group.ToString().Trim()))
                    {
                        sb_total.AppendLine($"select  COUNT(*)   from ( select {sb_field.ToString()} from {sb_table.ToString()}");
                    }
                    else
                    {
                        sb_total.AppendLine($"select  COUNT(*)   from {sb_table.ToString()}");
                    }
                    if (!string.IsNullOrEmpty(sb_join.ToString()))
                    {
                        sb_total.AppendLine($" {sb_join.ToString()}");
                    }
                    if (!string.IsNullOrEmpty(sb_where.ToString()))
                    {
                        sb_total.AppendLine($" where {sb_where.ToString()}");
                    }
                    if (!string.IsNullOrEmpty(sb_group.ToString()))
                    {
                        sb_total.AppendLine($" group by {sb_group.ToString()}");

                    }
                    if (!string.IsNullOrEmpty(sb_having.ToString()))
                    {
                        sb_total.AppendLine($" having {sb_having.ToString()}");
                    }
                    if (!string.IsNullOrEmpty(sb_group.ToString().Trim()))
                        sb_total.AppendLine($") as _hi ");
                    this.PageTotalSql = sb_total.ToString();

                    if (this.CurrentPage == 1)
                    {
                        //表示第一页
                        sb.AppendLine($"select  {sb_field.ToString()} from {sb_table.ToString()}");
                        if (!string.IsNullOrEmpty(sb_join.ToString()))
                            sb.AppendLine($" {sb_join.ToString()}");
                        if (!string.IsNullOrEmpty(sb_where.ToString()))
                            sb.AppendLine($" where {sb_where.ToString()}");

                        if (!string.IsNullOrEmpty(sb_group.ToString()))
                            sb.AppendLine($" group by {sb_group.ToString()}");

                        if (!string.IsNullOrEmpty(sb_having.ToString()))
                        {
                            sb_total.AppendLine($" having {sb_having.ToString()}");
                        }

                        if (!string.IsNullOrEmpty(sb_sort.ToString()))
                            sb.AppendLine($" order by  {sb_sort.ToString()}");

                        sb.AppendLine($"limit {this.PageSize}");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(sb_sort.ToString()))
                            throw new Exception($"有分页查询时必须指定排序条件");
                        sb.AppendLine($"select {sb_field_result.ToString()} from ( ");
                        sb.AppendLine($"select ROW_NUMBER() OVER(Order by {sb_sort.ToString()}) AS {dbConfig.Field_Pre}_hi_rownum_{dbConfig.Field_After}, {sb_field.ToString()} from {sb_table.ToString()}");
                        if (!string.IsNullOrEmpty(sb_join.ToString()))
                            sb.AppendLine($" {sb_join.ToString()}");
                        if (!string.IsNullOrEmpty(sb_where.ToString()))
                            sb.AppendLine($" where {sb_where.ToString()}");

                        if (!string.IsNullOrEmpty(sb_group.ToString()))
                            sb.AppendLine($" group by {sb_group.ToString()}");

                        if (!string.IsNullOrEmpty(sb_having.ToString()))
                        {
                            sb_total.AppendLine($" having {sb_having.ToString()}");
                        }

                        sb.AppendLine(") as hi_sql ");
                        //sb.Append($"where hi_sql._hi_rownum_ BETWEEN ({this.CurrentPage }-1)*{this.PageSize}+1 and {this.CurrentPage}*{this.PageSize}");
                        //if (!string.IsNullOrEmpty(sb_sort.ToString()))
                        sb.AppendLine($" order by  {dbConfig.Field_Pre}_hi_rownum_{dbConfig.Field_After} asc");

                        sb.AppendLine(string.Format("limit {0} OFFSET {1}", (this.CurrentPage - 1) * this.PageSize, this.PageSize));
                    }

                }
                else
                {
                    sb.AppendLine($"select {sb_distinct.ToString()} {sb_field.ToString()} from {sb_table.ToString()}");
                    if (!string.IsNullOrEmpty(sb_join.ToString()))
                        sb.AppendLine($" {sb_join.ToString()}");
                    if (!string.IsNullOrEmpty(sb_where.ToString()))
                        sb.AppendLine($" where {sb_where.ToString()}");

                    if (!string.IsNullOrEmpty(sb_group.ToString()))
                        sb.AppendLine($" group by {sb_group.ToString()}");

                    if (!string.IsNullOrEmpty(sb_having.ToString()))
                        sb.AppendLine($" having {sb_having.ToString()}");

                    if (!string.IsNullOrEmpty(sb_sort.ToString()))
                        sb.AppendLine($" order by  {sb_sort.ToString()}");
                }
            }
            else
            {


                sb.Append($"select {sb_distinct.ToString()} {sb_field_result.ToString()} from (");
                sb.AppendLine($"{sb_subquery.ToString()}");
                sb.Append(") as  hi_sql");
                if (!string.IsNullOrEmpty(sb_where.ToString()))
                    sb.AppendLine($" where {sb_where.ToString()}");

                if (!string.IsNullOrEmpty(sb_group.ToString()))
                    sb.AppendLine($" group by {sb_group.ToString()}");

                if (!string.IsNullOrEmpty(sb_having.ToString()))
                    sb.AppendLine($" having {sb_having.ToString()}");

                if (!string.IsNullOrEmpty(sb_sort.ToString()))
                    sb.AppendLine($" order by  {sb_sort.ToString()}");

            }

            if (!string.IsNullOrEmpty(this.ITabName))
            {
                StringBuilder sb_struct = new StringBuilder();
                HiTable hiTable = new HiTable();
                hiTable.TabName = this.ITabName;
                TabInfo tabInfo = new TabInfo();
                tabInfo.TabModel = hiTable;
                if (this.ResultColumn == null || this.ResultColumn.Count == 0)
                {
                    throw new Exception($"无法将查询结果插入表[{this.ITabName}] 因为当前结果集无结果");
                }
                if (hiTable.TableType != TableType.Entity)
                {
                    if (!this.ResultColumn.Any(c => c.IsBllKey))
                        this.ResultColumn.Insert(0, new HiColumn { FieldName = "_ID_", IsIdentity = true, IsPrimary = true, FieldDesc = "自增ID", FieldType = HiType.INT, SortNum = 0 });

                    tabInfo.Columns = this.ResultColumn;

                    string _sql = Context.DMTab.BuildTabCreateSql(tabInfo.TabModel, tabInfo.GetColumns);
                    sb_struct.AppendLine(_sql);
                }
                sb_struct.AppendLine($"insert into   {dbConfig.Table_Pre}{this.ITabName}{dbConfig.Table_After}   {sb.ToString()} ");
                return sb_struct.ToString();
            }
            else
                return sb.ToString();
        }

        /// <summary>
        /// 返回sql语句结果集的列信息
        /// </summary>
        /// <returns></returns>
        public override List<HiColumn> ToColumns()
        {
            string sql = this.ToSql();
            List<HiColumn> colist = this.ResultColumn;
            foreach (HiColumn col in colist)
            {
                if (col.IsPrimary) col.IsPrimary = !col.IsPrimary;
                if (col.IsIdentity) col.IsIdentity = !col.IsIdentity;
                if(col.IsBllKey) col.IsBllKey = !col.IsBllKey;
            }
            return colist;
        }
        public override IQuery WithRank(DbRank rank, DbFunction dbFunction, string field, string asname, SortType sortType)
        {
            if (field.Trim() != "*" && !string.IsNullOrEmpty(field))
                field = $"{dbConfig.Field_Pre}{field}{dbConfig.Field_After}";
            switch (rank)
            {
                case DbRank.DENSERANK:
                    if (dbFunction.IsIn<DbFunction>(DbFunction.AVG, DbFunction.COUNT, DbFunction.SUM))
                        this.Ranks.Add($"dense_rank() over( order by {dbFunction.ToString()}({field}) {sortType.ToString()}) as {asname}");
                    else if (dbFunction.IsIn<DbFunction>(DbFunction.NONE))
                    {
                        this.Ranks.Add($"dense_rank() over( order by  {field} {sortType.ToString()}) as {asname}");
                    }
                    else
                        throw new Exception($"{rank.ToString()} 不支持[{dbFunction.ToString()}]此函数");
                    break;
                case DbRank.RANK:
                    if (dbFunction.IsIn<DbFunction>(DbFunction.AVG, DbFunction.COUNT, DbFunction.SUM))
                        this.Ranks.Add($"rank() over( order by {dbFunction.ToString()}({field}) {sortType.ToString()}) as {asname}");
                    else if (dbFunction.IsIn<DbFunction>(DbFunction.NONE))
                    {
                        this.Ranks.Add($"rank() over( order by  {field} {sortType.ToString()}) as {asname}");
                    }
                    else
                        throw new Exception($"{rank.ToString()} 不支持[{dbFunction.ToString()}]此函数");
                    break;
                case DbRank.ROWNUMBER:
                    if (dbFunction.IsIn<DbFunction>(DbFunction.AVG, DbFunction.COUNT, DbFunction.SUM))
                        this.Ranks.Add($"row_number() over( order by {dbFunction.ToString()}({field}) {sortType.ToString()}) as {asname}");
                    else if (dbFunction.IsIn<DbFunction>(DbFunction.NONE))
                    {
                        this.Ranks.Add($"row_number() over( order by  {field} {sortType.ToString()}) as {asname}");
                    }
                    else
                        throw new Exception($"{rank.ToString()} 不支持[{dbFunction.ToString()}]此函数");
                    break;
                default:
                    break;
            }

            return this;
        }


        /// <summary>
        /// 获取表的实际表名
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public override string GetDbName(string tabname)
        {
            TableDefinition _table;
            Dictionary<string, string> _dic = Tool.RegexGrp(Constants.REG_TABNAME, tabname);

            if (_dic.Count > 0)
            {

                _table = new TableDefinition();
                _table.Schema = Context.CurrentConnectionConfig.Schema == null ? "" : Context.CurrentConnectionConfig.Schema;
                _table.TabName = tabname;
                _table.DbServer = Context.CurrentConnectionConfig.DbServer;

                switch (_table.TableType)
                {
                    case TableType.Local:
                        return $"#{_dic["tab"].ToString()}";//本地临时表
                    case TableType.Global:
                        return $"##{_dic["tab"].ToString()}";//全局临时表
                    case TableType.Var:
                        return $"@{_dic["tab"].ToString()}";//变量表
                    default:
                        return _table.TabName;
                }

            }
            else
                return base.GetDbName(tabname);
        }




        #region 私有方法

        void checkData()
        {


            int _idx = 0;
            int _idx2 = 0;
            bool _flag = false;
            PostGreSqlDM postgresqlDM = null;
            postgresqlDM = (PostGreSqlDM)Instance.CreateInstance<PostGreSqlDM>($"{Constants.NameSpace}.{this.Context.CurrentConnectionConfig.DbType.ToString()}{DbInterFace.DM.ToString()}");
            //IDMInitalize dMInitalize = new SqlServerDM();
            postgresqlDM.Context = this.Context;

            TabInfo currTabInfo = null;

            //多表子查询的情况下 无当前查询表
            if (!this.IsMultiSubQuery)
            {
                //统计表信息,并进行缓存处理
                if (this.TableList.Count > 0)
                {

                    //this.Context;
                    foreach (TableDefinition table in this.TableList)
                    {
                        TabInfo tabinfo;
                        Dictionary<string, string> _dic = Tool.RegexGrp(Constants.REG_TABNAME, table.TabName);
                        if (_dic["flag"].ToString() != "")
                        {
                            //当前连接缓存
                            tabinfo = this.Context.MCache.GetCache<TabInfo>(table.TabName);
                        }
                        else
                        {
                            //全局缓存
                            tabinfo = postgresqlDM.GetTabStruct(table.TabName);
                        }
                        //TabInfo tabinfo = dMInitalize.GetTabStruct(table.TabName);
                        if (!dictabinfo.ContainsKey(table.TabName))
                            dictabinfo.Add(table.TabName, tabinfo);

                        if (this.Table.TabName.Equals(table.TabName, StringComparison.OrdinalIgnoreCase))
                            currTabInfo = tabinfo;

                    }
                }
                else
                    throw new Exception("没有指定查询的表");

                
                
            }

            sb_table.Append($"{dbConfig.Table_Pre}{currTabInfo.TabModel.TabName}{dbConfig.Table_After} as {dbConfig.Table_Pre}{this.Table.AsTabName.ToLower()}{dbConfig.Table_After}");

            //检测字段信息

            //检测是否有去重关键字
            if (this.IsDistinct)
            {
                sb_distinct = new StringBuilder().Append(postgresqlDM.BuilderDistinct());
            }
            else
                sb_distinct = new StringBuilder();


            //检测返回结果字段

            Tuple<string, string, List<HiColumn>> queryresult = postgresqlDM.BuildQueryFieldSql(dictabinfo, (QueryProvider)this);
            sb_field.Append(queryresult.Item1);
            sb_field_result.Append(queryresult.Item2);
            this.ResultColumn = queryresult.Item3;

            //排名
            foreach (string rank in this.Ranks)
            {
                if (!string.IsNullOrEmpty(sb_field.ToString()))
                {
                    sb_field.Append($",{rank}");

                }
                if (!string.IsNullOrEmpty(sb_field_result.ToString()))
                {
                    sb_field_result.Append($",{rank}");

                }


            }

            //检测JOIN关联条件字段
            sb_join.Append(postgresqlDM.BuildJoinSql(this.TableList, dictabinfo, this.Fields, this.Joins));


            // 检测where条件字段
            if (this.Filters != null && this.Filters.IsHiSqlWhere && !string.IsNullOrEmpty(this.Filters.HiSqlWhere.Trim()))
            {
                //this.Filters.WhereParse.Result
                sb_where.Append(postgresqlDM.BuilderWhereSql(this.TableList, dictabinfo, this.Fields, this.Filters.WhereParse.Result, this.IsMultiSubQuery));
            }
            else
                sb_where.Append(postgresqlDM.BuilderWhereSql(this.TableList, dictabinfo, this.Fields, this.Wheres, this.IsMultiSubQuery));



            //分组

            sb_group.Append(postgresqlDM.BuildGroupSql(this.TableList, dictabinfo, this.Fields, this.Groups, this.IsMultiSubQuery));

            //having
            if (this.Havings != null)
                sb_having.Append(postgresqlDM.BuildHavingSql(this.TableList, dictabinfo, this.Fields, this.Havings.HavingParse.Result, this.IsMultiSubQuery));


            //排序字段
            sb_sort.Append(postgresqlDM.BuildOrderBySql(ref sb_group, dictabinfo, (QueryProvider)this));




        }


        /// <summary>
        /// 检测字段是否正确或合法 如果正确则反回对应的字段信息
        /// </summary>
        /// <param name="fieldDefinition"></param>
        /// <returns></returns>
        HiColumn checkField(FieldDefinition fieldDefinition, bool allowstart = false)
        {
            HiColumn hiColumn = null;
            TableDefinition tabinfo = this.TableList.Where(t => t.AsTabName.ToLower() == fieldDefinition.AsTabName.ToLower()).FirstOrDefault();//&& t.Columns.Any(c=>c.FieldName==fieldDefinition.FieldName)
            if (tabinfo != null)
            {
                if (dictabinfo.ContainsKey(tabinfo.TabName))
                {
                    hiColumn = dictabinfo[tabinfo.TabName].Columns.Where(f => f.FieldName.ToLower() == fieldDefinition.FieldName.ToLower()).FirstOrDefault();
                    if (hiColumn == null)
                    {
                        FieldDefinition fieldDefinition1 = this.Fields.Where(f => f.AsFieldName.ToLower() == fieldDefinition.FieldName.ToLower()).FirstOrDefault();
                        if (fieldDefinition1 != null)
                        {
                            hiColumn = dictabinfo[tabinfo.TabName].Columns.Where(f => f.FieldName.ToLower() == fieldDefinition1.FieldName.ToLower()).FirstOrDefault();
                            if (hiColumn == null)
                            {
                                if (fieldDefinition1.FieldName.Trim() != "*" && allowstart == true)
                                    throw new Exception($"字段[{fieldDefinition1.FieldName}]在表[{fieldDefinition1.AsTabName}]中不存在");
                            }
                        }
                        else
                            throw new Exception($"字段[{fieldDefinition.FieldName}]在表[{fieldDefinition.AsTabName}]中不存在");
                    }


                }
            }
            return hiColumn;
        }

        #endregion



    }
}
