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
    [Collection("step10")]
    public class Unit_Delete
    {
        private readonly ITestOutputHelper _outputHelper;

        public Unit_Delete(ITestOutputHelper testOutputHelper)
        {
            _outputHelper = testOutputHelper;
        }
        [Fact(DisplayName = "SqlServer删除操作")]
        [Trait("Delete", "init")]
        public void DeleteSqlServer()
        {
            HiSqlClient sqlClient = TestClientInit.GetSqlServerClient();
            DeleteGroups(sqlClient);
        }

        [Fact(DisplayName = "MySql删除操作")]
        [Trait("Delete", "init")]
        public void DeleteMySql()
        {
            HiSqlClient sqlClient = TestClientInit.GetMySqlClient();
            DeleteGroups(sqlClient);
        }
        [Fact(DisplayName = "Oracle删除操作")]
        [Trait("Delete", "init")]
        public void DeleteOracle()
        {
            HiSqlClient sqlClient = TestClientInit.GetOracleClient();
            DeleteGroups(sqlClient);
        }
        [Fact(DisplayName = "PostgreSql删除操作")]
        [Trait("Delete", "init")]
        public void DeletePostgreSql()
        {
            HiSqlClient sqlClient = TestClientInit.GetPostgreSqlClient();
            DeleteGroups(sqlClient);

        }
        [Fact(DisplayName = "Hana删除操作")]
        [Trait("Delete", "init")]
        public void DeleteHana()
        {
            HiSqlClient sqlClient = TestClientInit.GetHanaClient();
            DeleteGroups(sqlClient);
        }
        [Fact(DisplayName = "Sqlite删除操作")]
        [Trait("Delete", "init")]
        public void DeleteSqlite()
        {
            HiSqlClient sqlClient = TestClientInit.GetSqliteClient();
            DeleteGroups(sqlClient);

        }
        [Fact(DisplayName = "达梦删除操作")]
        [Trait("Delete", "init")]
        public void DeleteDaMeng()
        {
            HiSqlClient sqlClient = TestClientInit.GetDaMengClient();
            DeleteGroups(sqlClient);
        }

        #region
        void DeleteGroups(HiSqlClient sqlClient)
        {
            //初始化
            initDemoDynTable(sqlClient, "Hi_TestDelete");
            DeleteWhere(sqlClient);
        }

        void DeleteWhere(HiSqlClient sqlClient)
        {
            int successCount = 0;
            int successActCount = 0;
            int rltcnt = 0;
            IDelete delete = null;

            delete = sqlClient.Delete("Hi_TestDelete").Where(new Filter { { "Uid", OperType.BETWEEN, new RangDefinition() { Low = 1, High = 4 } } });
            successCount++;
            _outputHelper.WriteLine(delete.ToSql());
            rltcnt = delete.ExecCommand();
            successActCount += rltcnt == 4 ? 1 : 0;

            delete = sqlClient.Delete("Hi_TestDelete", new { Uid = 5, Uvarchar = "新店开业通告 广东广州天河" });
            successCount++;
            _outputHelper.WriteLine(delete.ToSql());
            rltcnt = delete.ExecCommand();
            successActCount += rltcnt == 1 ? 1 : 0;


            delete = sqlClient.Delete("Hi_TestDelete", new { Uid = 6 });//.Where(new Filter { { "Uid", OperType.BETWEEN, new RangDefinition() { Low = 6, High = 7 } } }); ;
            successCount++;
            _outputHelper.WriteLine(delete.ToSql());
            rltcnt = delete.ExecCommand();
            successActCount += rltcnt == 1 ? 1 : 0;

            delete = sqlClient.Delete("Hi_TestDelete").Where(" Uid >=498");
            successCount++;
            _outputHelper.WriteLine(delete.ToSql());
            rltcnt = delete.ExecCommand();
            successActCount += rltcnt == 2 ? 1 : 0;


            delete = sqlClient.Delete("Hi_TestDelete").Where(" Uid >=498 and Uvarchar = 'asdf' or  Uid >=498 or ( Uid BETWEEN '1' and '4' and Uid >=498 ) ");
            successCount++;
            _outputHelper.WriteLine(delete.ToSql());
            rltcnt = delete.ExecCommand();
            successActCount += rltcnt == 0 ? 1 : 0;

            delete = sqlClient.Delete("Hi_TestDelete").Where(new Filter { { "Uid", OperType.GE, 498 } , { "Uvarchar", OperType.EQ, "asdf" }
             , { "Uid", OperType.GE, 498 } ,{ "("},  { "Uid", OperType.BETWEEN, new RangDefinition() { Low = 1, High = 4 } },{ LogiType.OR},  { "Uid", OperType.GE, 498 } ,   { ")"},
            });
            successCount++;
            _outputHelper.WriteLine(delete.ToSql());
            rltcnt = delete.ExecCommand();
            successActCount += rltcnt == 0 ? 1 : 0;

            //delete = sqlClient.Delete("Hi_TestDelete").Where(new Filter { { "Uid", OperType.BETWEEN, new RangDefinition() { Low = 1, High = 4 } } })

            //    .Where(" Uid >=498 and Uvarchar = 'asdf' or  Uid >=498 or ( Uid BETWEEN '1' and '4' and Uid >=498 ) ");
            //successCount++;
            //_outputHelper.WriteLine(delete.ToSql());
            //rltcnt = delete.ExecCommand();
            //successActCount += rltcnt == 0 ? 1 : 0;



            delete = sqlClient.Delete("Hi_TestDelete", new List<Hi_TestDelete>() { new Hi_TestDelete { Uid = 7, Uvarchar = "testlen7", Unvarchar = "新店开业通告 广东广州天河城购物广场店新店开业通告 广东广州天河城购物广场店新店开业通告 广东广州天" } });
            successCount++;
            _outputHelper.WriteLine(delete.ToSql());
            rltcnt = delete.ExecCommand();
            successActCount += rltcnt == 1 ? 1 : 0;



            delete = sqlClient.Delete("Hi_TestDelete");
            successCount++;
            _outputHelper.WriteLine(delete.ToSql());
            rltcnt = delete.ExecCommand();
            successActCount += rltcnt >0 ? 1 : 0;


            delete = sqlClient.TrunCate("Hi_TestDelete");
            successCount++;
            _outputHelper.WriteLine(delete.ToSql());
            rltcnt = delete.ExecCommand();
            successActCount += 1;


            Assert.Equal(successActCount, successCount);
            /*
               
             */
        }

        public class Hi_TestDelete { 
            public int Uid { get; set; }

            
            public string Uvarchar { get; set; }
            public string Unvarchar { get; set; }
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

            List<object> lstdata = TestTable.DynTable.BuildTabDataList(tabname1, 500);


            int v = sqlClient.Insert(tabname1, lstdata).ExecCommand();


        }
        #endregion
    }
}
