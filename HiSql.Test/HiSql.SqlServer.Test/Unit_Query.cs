using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HiSql.Unit.Test
{
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
        public void QuerySqlServer()
        {
            HiSqlClient sqlClient = TestClientInit.GetSqlServerClient();
            QueryGroups(sqlClient);
        }

        [Fact(DisplayName = "MySql查询操作")]
        [Trait("Query", "init")]
        public void QueryMySql()
        {
            HiSqlClient sqlClient = TestClientInit.GetMySqlClient();
            QueryGroups(sqlClient);
        }
        [Fact(DisplayName = "Oracle查询操作")]
        [Trait("Query", "init")]
        public void QueryOracle()
        {
            HiSqlClient sqlClient = TestClientInit.GetOracleClient();
            QueryGroups(sqlClient);
        }
        [Fact(DisplayName = "PostgreSql查询操作")]
        [Trait("Query", "init")]
        public void QueryPostgreSql()
        {
            HiSqlClient sqlClient = TestClientInit.GetPostgreSqlClient();
            QueryGroups(sqlClient);

        }
        [Fact(DisplayName = "Hana查询操作")]
        [Trait("Query", "init")]
        public void QueryHana()
        {
            HiSqlClient sqlClient = TestClientInit.GetHanaClient();
            QueryGroups(sqlClient);
        }
        [Fact(DisplayName = "Sqlite查询操作")]
        [Trait("Query", "init")]
        public void QuerySqlite()
        {
            HiSqlClient sqlClient = TestClientInit.GetSqliteClient();
            QueryGroups(sqlClient);

        }
        [Fact(DisplayName = "达梦查询操作")]
        [Trait("Query", "init")]
        public void QueryDaMeng()
        {
            HiSqlClient sqlClient = TestClientInit.GetDaMengClient();
            QueryGroups(sqlClient);
        }

        #region
        void QueryGroups(HiSqlClient sqlClient)
        {
            query(sqlClient);

            queryIn(sqlClient);

            queryJoin(sqlClient);

        }

        void queryJoin(HiSqlClient sqlClient)
        {
            var query = sqlClient.Query("Hi_FieldModel").As("A").Field("*")
                .Join("Hi_TabModel").As("B").On(new HiSql.JoinOn() { { "A.TabName", "B.TabName" } })
                .Where("A.TabName='Hi_TestQuery'")
                .Sort("A.TabName asc", "A.FieldName asc");
            string sql = query
                .ToSql();
            var cnt = query.ToTable().Rows.Count;
            Assert.Equal(18, cnt);
        }
        void queryIn(HiSqlClient sqlClient)
        {
            string sql = sqlClient.HiSql($"select * from Hi_FieldModel  where tabname in ( 'Hi_FieldModel'， 'Hi_FieldModel2')  order by tabname asc")
                .Take(2).Skip(2)
                .ToSql();
        }
        void query(HiSqlClient sqlClient)
        {
            DataTable table = sqlClient.Query("Hi_FieldModel").Field("*").Take(10).Skip(1).ToTable();
            var dataList= sqlClient.Query("Hi_FieldModel").Field("*").Take(10).Skip(1).ToList<Hi_FieldModel>();
            //测试 table to list
            List<Hi_FieldModel> _FieldModelsB = DataConverter.ToList<Hi_FieldModel>(table, sqlClient.Context.CurrentConnectionConfig.DbType);
            List<Hi_FieldModel> _FieldModelsA = DataConvert.ToEntityList<Hi_FieldModel>(table);
            var strA = JsonConverter.ToJson(_FieldModelsA);
            var strB = JsonConverter.ToJson(_FieldModelsB);
            var strC = JsonConverter.ToJson(dataList);

            bool tabletolistIsOk = strA.Equals(strB) && strA.Equals(strC);

           
            _outputHelper.WriteLine($"测试 DataTable 转 List<T>  一致性：  {tabletolistIsOk}");
            //测试 datareader to list
            int total = 0;
             _FieldModelsB = sqlClient.Query("Hi_FieldModel").Field("*").Take(10).Skip(1).ToList<Hi_FieldModel>( ref total);
             _FieldModelsA = sqlClient.Query("Hi_FieldModel").Field("*").Take(10).Skip(1).ToList<Hi_FieldModel>();
             bool datareader2listIsOk = JsonConverter.ToJson(_FieldModelsA).Equals(JsonConverter.ToJson(_FieldModelsB));

            _outputHelper.WriteLine($"测试 IDataReader 转 List<T>  一致性：  {datareader2listIsOk}");


            //测试 list to table
            var listJson = JsonConverter.ToJson(_FieldModelsB);

            var listToTable2List = DataConverter.ToList<Hi_FieldModel>(DataConverter.ListToDataTable(_FieldModelsB, sqlClient.Context.CurrentConnectionConfig.DbType), sqlClient.Context.CurrentConnectionConfig.DbType);
            var listToTableJson = JsonConverter.ToJson(listToTable2List);
            var listTOtableIsOk = listToTableJson.Equals(listJson);

            _outputHelper.WriteLine($"测试 ListToDataTable 再转 List<T>  一致性：  {listTOtableIsOk}");

            Assert.True(tabletolistIsOk && datareader2listIsOk&& listTOtableIsOk);
        }

        #endregion
    }
}
