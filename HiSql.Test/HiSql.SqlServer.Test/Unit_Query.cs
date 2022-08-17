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

        }
        void query(HiSqlClient sqlClient)
        {
            DataTable table = sqlClient.Query("Hi_FieldModel").Field("*").Take(10).Skip(1).ToTable();
            var dataList= sqlClient.Query("Hi_FieldModel").Field("*").Take(10).Skip(1).ToList<Hi_FieldModel>();
            List<Hi_FieldModel> _FieldModelsB = DataConverter.ToList<Hi_FieldModel>(table, sqlClient.Context.CurrentConnectionConfig.DbType);
            List<Hi_FieldModel> _FieldModelsA = DataConvert.ToEntityList<Hi_FieldModel>(table);
            var strA = JsonConverter.ToJson(_FieldModelsA);
            var strB = JsonConverter.ToJson(_FieldModelsB);
            var strC = JsonConverter.ToJson(dataList);

            bool tabletolistIsOk = strA.Equals(strB) && strA.Equals(strC);

            int total = 0;
            _outputHelper.WriteLine($"测试 DataTable 转 List<T>  一致性：  {tabletolistIsOk}");
            _FieldModelsB = sqlClient.Query("Hi_FieldModel").Field("*").Take(10).Skip(1).ToList<Hi_FieldModel>( ref total);

             _FieldModelsA = sqlClient.Query("Hi_FieldModel").Field("*").Take(10).Skip(1).ToList<Hi_FieldModel>();

             bool datareader2listIsOk = JsonConverter.ToJson(_FieldModelsA).Equals(JsonConverter.ToJson(_FieldModelsB));

            _outputHelper.WriteLine($"测试 IDataReader 转 List<T>  一致性：  {datareader2listIsOk}");

            Assert.True(tabletolistIsOk && datareader2listIsOk);
        }

        #endregion
    }
}
