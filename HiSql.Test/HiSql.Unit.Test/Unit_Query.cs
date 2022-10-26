using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HiSql.Unit.Test
{
    public class H_tst10
    {
        public int SID { get; set; }
        public string uname { get; set; }
        public string gname { get; set; }

        public DateTime? birth { get; set; }

        public int sage { get; set; }
    }
    [Collection("step9")]
    public class Unit_Query
    {
        private readonly ITestOutputHelper _outputHelper;

        public Unit_Query(ITestOutputHelper testOutputHelper)
        {
            _outputHelper = testOutputHelper;
        }
        [Fact(DisplayName = "SqlServer查询操作")]
        [Trait("Query", "init")]
        public async Task QuerySqlServer()
        {
            HiSqlClient sqlClient = TestClientInit.GetSqlServerClient();
            await QueryGroups(sqlClient);
        }

        [Fact(DisplayName = "MySql查询操作")]
        [Trait("Query", "init")]
        public async Task QueryMySql()
        {
            HiSqlClient sqlClient = TestClientInit.GetMySqlClient();
            await QueryGroups(sqlClient);
        }
        [Fact(DisplayName = "Oracle查询操作")]
        [Trait("Query", "init")]
        public async Task QueryOracle()
        {
            HiSqlClient sqlClient = TestClientInit.GetOracleClient();
            await QueryGroups(sqlClient);
        }
        [Fact(DisplayName = "PostgreSql查询操作")]
        [Trait("Query", "init")]
        public async Task QueryPostgreSql()
        {
            HiSqlClient sqlClient = TestClientInit.GetPostgreSqlClient();
            await QueryGroups(sqlClient);

        }
        [Fact(DisplayName = "Hana查询操作")]
        [Trait("Query", "init")]
        public async Task QueryHana()
        {
            HiSqlClient sqlClient = TestClientInit.GetHanaClient();
            await QueryGroups(sqlClient);
        }
        [Fact(DisplayName = "Sqlite查询操作")]
        [Trait("Query", "init")]
        public async Task QuerySqlite()
        {
            HiSqlClient sqlClient = TestClientInit.GetSqliteClient();
            await QueryGroups(sqlClient);

        }
        [Fact(DisplayName = "达梦查询操作")]
        [Trait("Query", "init")]
        public async Task QueryDaMeng()
        {
            HiSqlClient sqlClient = TestClientInit.GetDaMengClient();
            await QueryGroups(sqlClient);
        }

        #region
        async Task QueryGroups(HiSqlClient sqlClient)
        {
            //初始化
            initDemoDynTable(sqlClient, "Hi_TestQuery");
            query(sqlClient);
          
            insertNullData(sqlClient);

            queryNullData(sqlClient);
           

            queryIn(sqlClient);

            queryJoin(sqlClient);

            queryWhere(sqlClient);
            queryCase(sqlClient);
            queryGroupBy(sqlClient);    //有问题


            //query to list decimal 测试
            querydecimal(sqlClient);
            querylike(sqlClient);

        }

        class _Test_Decimal
        { 
            public string Uvarchar
            { get; set; }
            public decimal Udecimal
            { get; set; }
        }

        void querydecimal(HiSqlClient sqlClient)
        {
            //class testClass{ }
            List< _Test_Decimal> list= sqlClient.HiSql("select Uvarchar,Udecimal from Hi_TestQuery").Take(10).Skip(1).ToList<_Test_Decimal>();

        }

        void querylike(HiSqlClient sqlClient)
        {
            string sql = "";
            sql=sqlClient.HiSql("select * from Hi_FieldModel where TabName like 'Hi_%'").ToSql();
            _outputHelper.WriteLine(sql);
            DataTable dt= sqlClient.HiSql("select * from Hi_FieldModel where TabName like 'Hi_%'").ToTable();
            Assert.True(dt.Rows.Count > 0);

            sql=sqlClient.HiSql("select * from Hi_FieldModel where TabName not like 'abc_%'").ToSql();
            _outputHelper.WriteLine(sql);
            dt = sqlClient.HiSql("select * from Hi_FieldModel where TabName not like 'abc_%'").ToTable();
            Assert.True(dt.Rows.Count > 0);

        }

        void queryWhere(HiSqlClient sqlClient)
        {
            int successCount = 0;
            int successActCount = 0;

            var query = sqlClient.HiSql(@$"select * from Hi_FieldModel  where (tabname =  'Hi_FieldModel' or  tabname = 'Hi_TestQuery') and (tabname =  'Hi_FieldModel' or  tabname = 'Hi_TestQuery') and FieldType in (11,41,21,12) ");
            successCount++;
            _outputHelper.WriteLine(query.ToSql());
            successActCount += query.ToTable().Rows.Count >= 32 ? 1 : 0;

            query = sqlClient.HiSql($"select * from Hi_FieldModel  where (tabname = @TabName or tabname = @TabName2) and FieldType in (11,41,21,12) "
            , new { TabName = "Hi_FieldModel", TabName2 = "Hi_TestQuery" });
            _outputHelper.WriteLine(query.ToSql());
            successCount++;
            successActCount += query.ToTable().Rows.Count >= 32 ? 1 : 0;

            query = sqlClient.Query("Hi_FieldModel").Field("*").Where(@$"(tabname =  'Hi_FieldModel' or  tabname = 'Hi_TestQuery') and (tabname =  'Hi_FieldModel' or  tabname = 'Hi_TestQuery') and FieldType in (11,41,21,12) ");
            _outputHelper.WriteLine(query.ToSql());
            successCount++;
            successActCount += query.ToTable().Rows.Count >= 32 ? 1 : 0;


            query = sqlClient.Query("Hi_FieldModel").Field(@"*")

             .WithRank(DbRank.DENSERANK, DbFunction.NONE, "TabName", "rowidx1", SortType.ASC)
              .WithRank(DbRank.RANK, DbFunction.NONE, "TabName", "rowidx2", SortType.ASC)
              .WithRank(DbRank.ROWNUMBER, DbFunction.NONE, "TabName", "rowidx3", SortType.ASC)
                 //以下实现组合排名
                 .WithRank(DbRank.DENSERANK, new Ranks { { DbFunction.NONE, "TabName" } }, "rowidx234")
                  .WithRank(DbRank.RANK, new Ranks { { DbFunction.NONE, "TabName" } }, "rowidx244")
                   .WithRank(DbRank.ROWNUMBER, new Ranks { { DbFunction.NONE, "TabName" } }, "rowidx245")

                .Where(new Filter() {   { "("},{ "tabname", OperType.EQ,"Hi_FieldModel"},{ LogiType.OR},{ "tabname", OperType.EQ,"Hi_TestQuery"} , { ")"}
               , { LogiType.AND},{ "FieldType", OperType.IN, new List<int> { 11, 41, 21, 12 } }
            });
            successCount++;
            _outputHelper.WriteLine(query.ToSql());
            successActCount += query.ToTable().Rows.Count >= 32 ? 1 : 0;



            Assert.Equal(successActCount, successCount);
            /*
               函数 Count()
             */
        }

        void queryCase(HiSqlClient sqlClient)
        {
            int successCount = 0;
            int successActCount = 0;

            IQuery query = null;
            // hisql直接使用 case 
            //            query = sqlClient.HiSql(@$"select CASE WHEN FieldType > 21 THEN '大于21' WHEN FieldType = 21 THEN '等于21' ELSE '小于21' END as CaseValue,* from Hi_FieldModel  where (tabname =  'Hi_FieldModel' or  tabname = 'Hi_TestQuery') and FieldType in (11,41,21,12)

            //");     //  where 条件  and CASE WHEN FieldType > 21 THEN '大于21' WHEN FieldType = 21 THEN '等于21' ELSE '小于21' END = '小于21'
            //            successCount++;
            //            _outputHelper.WriteLine(query.ToSql());
            //            successActCount += query.ToTable().Rows.Count == 32 ? 1 : 0;

            // CASE WHEN FieldType > 21 THEN '大于21' WHEN FieldType = 21 THEN '等于21' ELSE '小于21' end as Case1 ,

            query = sqlClient.Query("Hi_FieldModel").Field(@"*").WithLock(LockMode.NOLOCK)
              .Case("FieldType")
              .When("FieldType > 21  and FieldType <> 1").Then("'大于21'")
              .When("FieldType = 21").Then("'等于21'")
              .When("FieldType < 21").Then("'小于21'").Else("'test'").EndAs("Case1", typeof(string))
               .Where(new Filter() {   { "("},{ "tabname", OperType.EQ,"Hi_FieldModel"},{ LogiType.OR},{ "tabname", OperType.EQ,"Hi_TestQuery"} , { ")"}
               , { LogiType.AND},{ "FieldType", OperType.IN, new List<int> { 11, 41, 21, 12 } }
           });
            successCount++;
            _outputHelper.WriteLine(query.ToSql());
            successActCount += query.ToTable().Rows.Count >= 32 ? 1 : 0;

            //case 别名
            query = sqlClient.Query("Hi_FieldModel").As("a").Field(@"*")
               .Case("a.FieldType").When("a.FieldType > 21 and a.FieldType <> 1").Then("'大于21'")
               .When("a.FieldType = 21").Then("'等于21'")
               .When("a.FieldType < 21").Then("'小于21'").Else("'test'").EndAs("Case1", typeof(string))
                .Where(new Filter() {   { "("},{ "tabname", OperType.EQ,"Hi_FieldModel"},{ LogiType.OR},{ "tabname", OperType.EQ,"Hi_TestQuery"} , { ")"}
               , { LogiType.AND},{ "FieldType", OperType.IN, new List<int> { 11, 41, 21, 12 } }
            });
            successCount++;
            _outputHelper.WriteLine(query.ToSql());
            successActCount += query.ToTable().Rows.Count >= 32 ? 1 : 0;


            Assert.Equal(successActCount, successCount);
            /*
               
             */
        }
        void queryIn(HiSqlClient sqlClient)
        {
            int successCount = 0;
            int successActCount = 0;
            DataTable table = null;
            var query = sqlClient.HiSql(@$"select * from Hi_FieldModel  where tabname in ( 'Hi_FieldModel', 'Hi_TestQuery')  
                    and FieldType in (11,41,21,12) AND FieldName IN( SELECT FieldName from Hi_FieldModel WHERE FieldName='CreateTime')");
            successCount++;
            table = query.ToTable();
            _outputHelper.WriteLine(query.ToSql() + $" 查询结果 {table.Rows.Count == 2} 应该是 2 行，实际是{table.Rows.Count}行");

            successActCount += table.Rows.Count == 2 ? 1 : 0;

            query = sqlClient.HiSql($"select * from Hi_FieldModel  where tabname in ( @TabName)   and FieldType in (@FieldType)  "
            , new { TabName = new List<string> { "Hi_FieldModel", "Hi_TestQuery" }, FieldType = new List<int> { 11, 41, 21, 12 } });
          
            successCount++;
            table = query.ToTable();
            _outputHelper.WriteLine(query.ToSql() + $" 查询结果 {table.Rows.Count == 32} 应该是 32 行，实际是{table.Rows.Count}行");

            successActCount += table.Rows.Count >= 32 ? 1 : 0;

            //错误写法
            //query = sqlClient.HiSql($"select * from Hi_FieldModel  where tabname in (@TanName)  "
            //   , new HiParameter("TabName", new List<string> { "Hi_FieldModel", "Hi_TestQuery" })
            //   );
            //_outputHelper.WriteLine(query.ToSql());
            //resultCnt += query.ToTable().Rows.Count == 53 ? 1 : 0;

            query = sqlClient.Query("Hi_FieldModel").Field("*").Where(@$"tabname in ( 'Hi_FieldModel', 'Hi_TestQuery')  
                    and FieldType in (11, 41, 21, 12) AND FieldName IN(SELECT FieldName from Hi_FieldModel WHERE FieldName = 'CreateTime')  ");

            table = query.ToTable();
            _outputHelper.WriteLine(query.ToSql() + $" 查询结果 {table.Rows.Count == 2} 应该是 2 行，实际是{table.Rows.Count}行");
            successCount++;
            successActCount += table.Rows.Count == 2 ? 1 : 0;


            query = sqlClient.Query("Hi_FieldModel").Field("*").Where(new Filter() { { "tabname", OperType.IN, new List<string> { "Hi_FieldModel", "Hi_TestQuery" } },
            { "FieldType", OperType.IN, new List<int> { 11, 41, 21, 12 } }
            });
            successCount++;
            table = query.ToTable();
            _outputHelper.WriteLine(query.ToSql() + $" 查询结果 {table.Rows.Count == 32} 应该是 32 行，实际是{table.Rows.Count}行");

            successActCount += table.Rows.Count >= 32 ? 1 : 0;


            query = sqlClient.Query("Hi_FieldModel").Field(@"TabName, FieldName, FieldDesc, IsIdentity, IsPrimary, IsBllKey , FieldType , SortNum, Regex, DBDefault, DefaultValue, FieldLen , FieldDec , SNO, SNO_NUM, IsSys, IsNull, IsRequire, IsIgnore".Split(",")).Where(new Filter() { { "tabname", OperType.IN, new List<string> { "Hi_FieldModel", "Hi_TestQuery" } },
            { "FieldType", OperType.IN, new List<int> { 11, 41, 21, 12 } }
            });
            successCount++;
            table = query.ToTable();
            _outputHelper.WriteLine(query.ToSql() +$" 查询结果 {table.Rows.Count == 32} 应该是 32 行，实际是{table.Rows.Count}行");
            successActCount += table.Rows.Count >= 32 ? 1 : 0;

            Assert.Equal(successActCount, successCount);
            /*
                DM Hi_FieldModel 值大小写问题
             */
        }
        void queryGroupBy(HiSqlClient sqlClient)
        {
            int successCount = 0;
            int successActCount = 0;
            //样例1
            var query = sqlClient.Query("Hi_FieldModel").As("A")
                .Field("A.FieldName as FieldName, count(*) as _COUNT, AVG(FieldLen) as _AVG, MIN(FieldLen) as _MIN,MAX(FieldLen) as _MAX,SUM(FieldLen) as _SUM ".Split(","))
                 .WithRank(DbRank.DENSERANK, DbFunction.NONE, "FieldName", "rowidx1", SortType.ASC)
             .WithRank(DbRank.DENSERANK, DbFunction.AVG, "FieldLen", "rowidx2", SortType.ASC)
             .WithRank(DbRank.DENSERANK, DbFunction.MIN, "FieldLen", "rowidx3", SortType.ASC)
             .WithRank(DbRank.DENSERANK, DbFunction.MAX, "FieldLen", "rowidx4", SortType.ASC)
              .WithRank(DbRank.DENSERANK, DbFunction.SUM, "FieldLen", "rowidx5", SortType.ASC)
                .WithRank(DbRank.DENSERANK, DbFunction.COUNT, "FieldLen", "rowidx6", SortType.ASC)

                .WithRank(DbRank.DENSERANK, new Ranks { { DbFunction.NONE, "FieldName" }, { DbFunction.MAX, "FieldLen", SortType.DESC }, { DbFunction.MIN, "FieldLen", SortType.DESC }, { DbFunction.COUNT, "FieldLen", SortType.DESC }, { DbFunction.AVG, "FieldLen", SortType.DESC }, { DbFunction.SUM, "FieldLen", SortType.DESC } }, "rowidx234")
                  .WithRank(DbRank.RANK, new Ranks { { DbFunction.NONE, "FieldName" }, { DbFunction.MAX, "FieldLen", SortType.DESC }, { DbFunction.MIN, "FieldLen", SortType.DESC }, { DbFunction.COUNT, "FieldLen", SortType.DESC }, { DbFunction.AVG, "FieldLen", SortType.DESC }, { DbFunction.SUM, "FieldLen", SortType.DESC } }, "rowidx244")
                   .WithRank(DbRank.ROWNUMBER, new Ranks { { DbFunction.NONE, "FieldName" }, { DbFunction.MAX, "FieldLen", SortType.DESC }, { DbFunction.MIN, "FieldLen", SortType.DESC }, { DbFunction.COUNT, "FieldLen", SortType.DESC }, { DbFunction.AVG, "FieldLen", SortType.DESC }, { DbFunction.SUM, "FieldLen", SortType.DESC } }, "rowidx245")

                .Join("Hi_TabModel").As("B").On(new HiSql.JoinOn() { { "A.TabName", "B.TabName" } })
                .Where("A.TabName='Hi_TestQuery'")
                .Group(new GroupBy { { "A.TabName" }, { "A.FieldName" } })

        //.Having("count(*)>=0") //此处语法错误
        .Having(new Having() { { "count(*)", OperType.GE, "0" } })
        .Sort("A.TabName asc", "A.FieldName asc");

            _outputHelper.WriteLine(query.ToSql()); successCount++;
            successActCount += query.ToTable().Rows.Count == 18 ? 1 : 0;

            //            //样例2 有问题
            //            query = sqlClient.HiSql(@"select A.FieldName as FieldName , count(*) as _COUNT, AVG(FieldLen) as _AVG, MIN(FieldLen) as _MIN,MAX(FieldLen) as _MAX,SUM(FieldLen) as _SUM 
            //from Hi_FieldModel as A join Hi_TabModel as b on A.TabName = B.TabName where A.TabName='Hi_TestQuery' group by A.TabName,A.FieldName having count(*)>=0 order by A.TabName asc, A.FieldName asc");

            //            //order by  HiSql语法检测错误: 子查询语句[A.TabName = 'Hi_TestQuery' group by A.TabName, A.FieldName having count(*) >= 0 order by A.TabName asc, A.FieldName asc]不允许[order by]排序

            //            _outputHelper.WriteLine(query.ToSql()); successCount++;
            //            successActCount += query.ToTable().Rows.Count == 3 ? 1 : 0;

            Assert.Equal(successActCount, successCount);
        }

        void queryJoin(HiSqlClient sqlClient)
        {
            int successCount = 0;
            int successActCount = 0;
            var query = sqlClient.Query("Hi_FieldModel").As("A").Field("*")
                .Join("Hi_TabModel").As("B").On(new HiSql.JoinOn() { { "A.TabName", "B.TabName" } })
                .Where("A.TabName='Hi_TestQuery'");
            _outputHelper.WriteLine(query.ToSql()); successCount++;
            successActCount += query.ToTable().Rows.Count == 18 ? 1 : 0;

            // { "A.FieldName", "'CreateTime'" }, { "B.TabName", "Hi_TestQuery" }  不支持  
            query = sqlClient.Query("Hi_FieldModel").As("A").Field("*")
                .Join("Hi_TabModel").As("B").On(new HiSql.JoinOn() { { "A.TabName", "B.TabName" } })
                .Where("A.TabName='Hi_TestQuery'");
            _outputHelper.WriteLine(query.ToSql()); successCount++;
            successActCount += query.ToTable().Rows.Count == 18 ? 1 : 0;


            query = sqlClient.Query("Hi_FieldModel").As("A").Field("*")
               .Join("Hi_TabModel").As("B").On("A.TabName", "B.TabName")
               .Where("A.TabName='Hi_TestQuery'");
            _outputHelper.WriteLine(query.ToSql()); successCount++;
            successActCount += query.ToTable().Rows.Count == 18 ? 1 : 0;

            query = sqlClient.Query("Hi_FieldModel").As("A").Field("*")
              .Join("Hi_TabModel").As("B").On(@"A.TabName=B.TabName  AND B.TabName = 'Hi_TestQuery'  AND A.FieldName = 'CreateTime'")
              .Where("A.TabName='Hi_TestQuery'");
            _outputHelper.WriteLine(query.ToSql()); successCount++;
            successActCount += query.ToTable().Rows.Count == 1 ? 1 : 0;


            query = sqlClient.Query("Hi_FieldModel").As("A").Field("*")
              .Join("Hi_TabModel", JoinType.Left).As("B").On(@"A.TabName=B.TabName  AND B.TabName = 'Hi_TestQuery'  AND A.FieldName = 'CreateTime'")
              .Where("A.TabName='Hi_TestQuery'")
              ;
            _outputHelper.WriteLine(query.ToSql()); successCount++;
            successActCount += query.ToTable().Rows.Count == 18 ? 1 : 0;

            if (sqlClient.CurrentConnectionConfig.DbType != DBType.Sqlite)
            {
                query = sqlClient.Query("Hi_FieldModel").As("A").Field("*")
                 .Join("Hi_TabModel", JoinType.Right).As("B").On(@"A.TabName=B.TabName  AND B.TabName = 'Hi_TestQuery'  AND A.FieldName = 'CreateTime'")
                 .Where("A.TabName='Hi_TestQuery'");
                _outputHelper.WriteLine(query.ToSql()); successCount++;
                successActCount += query.ToTable().Rows.Count == 1 ? 1 : 0;

            }


            query = sqlClient.Query("Hi_FieldModel").As("A").Field("*")
           .Join("Hi_TabModel", JoinType.Inner).As("B").On(@"A.TabName=B.TabName  AND B.TabName = 'Hi_TestQuery'  AND A.FieldName = 'CreateTime' and A.TabName='Hi_TestQuery'")
           .Where("A.TabName='Hi_TestQuery'");
            _outputHelper.WriteLine(query.ToSql()); successCount++;
            successActCount += query.ToTable().Rows.Count == 1 ? 1 : 0;

            Assert.Equal(successActCount, successCount);

            /*
            
             */
        }

        void query(HiSqlClient sqlClient)
        {
            int total = 0;
            DataTable table = sqlClient.Query("Hi_FieldModel").Field("*").Take(10).Skip(1).ToTable();
            var dataList = sqlClient.Query("Hi_FieldModel").Field("*").Take(10).Skip(1).ToList<Hi_FieldModel>();
            //测试 table to list
            List<Hi_FieldModel> _FieldModelsB = DataConverter.ToList<Hi_FieldModel>(table, sqlClient.Context.CurrentConnectionConfig.DbType);
            List<Hi_FieldModel> _FieldModelsA = DataConvert.ToEntityList<Hi_FieldModel>(table);
            var strA = JsonConverter.ToJson(_FieldModelsA);
            var strB = JsonConverter.ToJson(_FieldModelsB);
            var strC = JsonConverter.ToJson(dataList);

            bool tabletolistIsOk = strA.Equals(strB) && strA.Equals(strC);


            _outputHelper.WriteLine($"测试 DataTable 转 List<T>  一致性：  {tabletolistIsOk}");
            //测试 datareader to list

            _FieldModelsB = sqlClient.Query("Hi_FieldModel").Field("*").Take(10).Skip(1).ToList<Hi_FieldModel>(ref total);
            _FieldModelsA = sqlClient.Query("Hi_FieldModel").Field("*").Take(10).Skip(1).ToList<Hi_FieldModel>();
            bool datareader2listIsOk = JsonConverter.ToJson(_FieldModelsA).Equals(JsonConverter.ToJson(_FieldModelsB));


            _FieldModelsB = sqlClient.Query("Hi_TestQuery").Field("*").Take(10).Skip(1).ToList<Hi_FieldModel>(ref total);




            //测试 list to table
            var listJson = JsonConverter.ToJson(_FieldModelsB);

            var listToTable2List = DataConverter.ToList<Hi_FieldModel>(DataConverter.ListToDataTable(_FieldModelsB, sqlClient.Context.CurrentConnectionConfig.DbType), sqlClient.Context.CurrentConnectionConfig.DbType);
            var listToTableJson = JsonConverter.ToJson(listToTable2List);
            var listTOtableIsOk = listToTableJson.Equals(listJson);

            _outputHelper.WriteLine($"测试 ListToDataTable 再转 List<T>  一致性：  {listTOtableIsOk}");
            _FieldModelsA = sqlClient.Query("Hi_TestQuery").Field("*").Take(10).Skip(1).ToList<Hi_FieldModel>();

            bool datareader2Hi_TestQueryIsOk = JsonConverter.ToJson(_FieldModelsA).Equals(JsonConverter.ToJson(_FieldModelsB));


            _outputHelper.WriteLine($"测试 IDataReader 转 List<T>  一致性：  {datareader2listIsOk && datareader2Hi_TestQueryIsOk}");


            var list3 = sqlClient.HiSql("select * from Hi_FieldModel where TabName in (@TabName)",
                new Dictionary<string, object> {
                    {"FieldName","CreateTime" },
                    {"TabName",JArray.FromObject(new List<string>
                    {
                        "Hi_FieldModel"
                    })},
                    {"FieldNameList",new List<string>{
                        "CreateTime",
                        "IsSearch"
                    } }
               }).ToEObject();
            var list = sqlClient.HiSql("select * from Hi_FieldModel where TabName in (@TabName)",
                new Dictionary<string, object> {
                    {"FieldName","CreateTime" },
                    {"TabName",JArray.FromObject(new List<string>
                    {
                        "Hi_FieldModel",
                        "Hi_TabModel"
                    })},
                    {"FieldNameList",new List<string>{
                        "CreateTime",
                        "IsSearch"
                    } }
               }).ToList<Hi_FieldModel>(); 
            _outputHelper.WriteLine(JsonConverter.ToJson(list));
            _outputHelper.WriteLine($"测试混合参数查询！");
           

            Assert.True(tabletolistIsOk && datareader2listIsOk && listTOtableIsOk);
        }
        void initDemoDynTable(HiSqlClient sqlClient, string tabname1)
        {
            sqlClient.CurrentConnectionConfig.AppEvents = HisqlTestExt.GetAopEvent(_outputHelper);

            _outputHelper.WriteLine($"检测表[{tabname1}] 是否在当前库中存在");
            if (sqlClient.DbFirst.CheckTabExists(tabname1))
            {
                TabInfo tabInfo2 = sqlClient.DbFirst.GetTabStruct(tabname1);
                _outputHelper.WriteLine($"表[{tabname1}] 存在正在执行删除并清除表结构信息");
                sqlClient.DbFirst.DropTable(tabname1);
                _outputHelper.WriteLine($"表[{tabname1}] 已经删除");
            }


            bool iscreate = sqlClient.DbFirst.CreateTable(Test.TestTable.DynTable.BuildTabInfo(tabname1, true));
            if (iscreate)
                _outputHelper.WriteLine($"表[{tabname1}] 已经成功创建");
            else
                _outputHelper.WriteLine($"表[{tabname1}] 创建失败");


            TabInfo tabInfo = sqlClient.DbFirst.GetTabStruct(tabname1);

            List<object> lstdata = TestTable.DynTable.BuildTabDataList(tabname1, 5);


            int v = sqlClient.Insert(tabname1, lstdata).ExecCommand();


        }


        void queryNullData(HiSqlClient sqlClient)
        {
            List<TDynamic> lstdyn = sqlClient.HiSql("select * from H_tst10").ToDynamic();
            List<ExpandoObject> lstexp = sqlClient.HiSql("select * from H_tst10").ToEObject();

            string jsonstr = sqlClient.HiSql("select * from H_tst10").ToJson();
            List<H_tst10> lsttst = sqlClient.HiSql("select * from H_tst10").ToList<H_tst10>();


            jsonstr = sqlClient.HiSql("select * from H_tst10 where birth is  null").ToJson();

            jsonstr = sqlClient.HiSql("select * from H_tst10 where birth is not null").ToJson();

        }
        void insertNullData(HiSqlClient sqlClient)
        {
            string tabname = "H_tst10";
            if (sqlClient.DbFirst.CheckTabExists("H_tst10"))
            {
                sqlClient.Drop(tabname).ExecCommand();
                _outputHelper.WriteLine($" 已经删除Null值 测试表[{tabname}]");
            }

            TabInfo tabInfo = TestTable.DynTable.BuildNullTest(tabname, true);

            bool iscreate = sqlClient.DbFirst.CreateTable(tabInfo);

            if (iscreate)
            {
                _outputHelper.WriteLine($" 已经创建Null值 测试表[{tabname}]");
                sqlClient.Insert(tabname, new List<Dictionary<string, object>> {
                    new Dictionary<string, object>
                    {
                        { "SID",1},
                        { "uname","tansar"}
                    },
                    new Dictionary<string, object>
                    {
                        { "SID",2},
                        { "uname","tansar"},
                        { "birth",DateTime.Now}
                    },
                    new Dictionary<string, object>
                    {
                        { "SID",3},
                        { "gname","tgm"},
                        { "birth",DateTime.Now}
                    },
                    new Dictionary<string, object>
                    {
                        { "SID",5},
                         { "uname","tansar"},
                        { "gname","tgm"}

                    }
                }).ExecCommand();


            }
            else
            {
                _outputHelper.WriteLine($" 创建Null值 测试表[{tabname}]失败");
                Assert.True(false);
            }

        }
        #endregion
    }
}
