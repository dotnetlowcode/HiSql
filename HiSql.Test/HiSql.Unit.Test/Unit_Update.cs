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
    public class Unit_Update
    {
        private readonly ITestOutputHelper _outputHelper;

        public Unit_Update(ITestOutputHelper testOutputHelper)
        {
            _outputHelper = testOutputHelper;
        }
        [Fact(DisplayName = "SqlServer修改操作")]
        [Trait("Update", "init")]
        public void UpdateSqlServer()
        {
            HiSqlClient sqlClient = TestClientInit.GetSqlServerClient();
            UpdateGroups(sqlClient);
        }

        [Fact(DisplayName = "MySql修改操作")]
        [Trait("Update", "init")]
        public void UpdateMySql()
        {
            HiSqlClient sqlClient = TestClientInit.GetMySqlClient();
            UpdateGroups(sqlClient);
        }
        [Fact(DisplayName = "Oracle修改操作")]
        [Trait("Update", "init")]
        public void UpdateOracle()
        {
            HiSqlClient sqlClient = TestClientInit.GetOracleClient();
            UpdateGroups(sqlClient);
        }
        [Fact(DisplayName = "PostgreSql修改操作")]
        [Trait("Update", "init")]
        public void UpdatePostgreSql()
        {
            HiSqlClient sqlClient = TestClientInit.GetPostgreSqlClient();
            UpdateGroups(sqlClient);

        }
        [Fact(DisplayName = "Hana修改操作")]
        [Trait("Update", "init")]
        public void UpdateHana()
        {
            HiSqlClient sqlClient = TestClientInit.GetHanaClient();
            UpdateGroups(sqlClient);
        }
        [Fact(DisplayName = "Sqlite修改操作")]
        [Trait("Update", "init")]
        public void UpdateSqlite()
        {
            HiSqlClient sqlClient = TestClientInit.GetSqliteClient();
            UpdateGroups(sqlClient);

        }
        [Fact(DisplayName = "达梦修改操作")]
        [Trait("Update", "init")]
        public void UpdateDaMeng()
        {
            HiSqlClient sqlClient = TestClientInit.GetDaMengClient();
            UpdateGroups(sqlClient);
        }

        #region
        void UpdateGroups(HiSqlClient sqlClient)
        {
            //初始化
            initDemoDynTable(sqlClient, "Hi_TestUpdate");
            UpdateWhere(sqlClient);
        }

        void UpdateWhere(HiSqlClient sqlClient)
        {
            int successCount = 0;
            int successActCount = 0;
            int rltcnt = 0;
            IUpdate Update = null;

            //按指定的where更新
            Update = sqlClient.Update("Hi_TestUpdate").Set(new { Uvarchar = "新店开业通告 广东广州天河22" }).OnlyWhere(new Filter { { "Uid", OperType.BETWEEN, new RangDefinition() { Low = 1, High = 4 } } });
            successCount++;
            _outputHelper.WriteLine(Update.ToSql());
            rltcnt = Update.ExecCommand();
            successActCount += rltcnt == 4 ? 1 : 0;
           
            Update = sqlClient.Update("Hi_TestUpdate").Set(new { Uvarchar = "新店开业通告 广东广州天河22" }).OnlyWhere("Uid BETWEEN 1 and 4 ");
            successCount++;
            _outputHelper.WriteLine(Update.ToSql());
            rltcnt = Update.ExecCommand();
            successActCount += rltcnt == 4 ? 1 : 0;

          

            //按指定 的where更新+主键更新（如有）
            Update = sqlClient.Update("Hi_TestUpdate").Set(new { Uvarchar = "新店开业通告 广东广州天河" }).Where(new Filter { { "Uid", OperType.BETWEEN, new RangDefinition() { Low = 5, High = 5 } } });
            successCount++;
            _outputHelper.WriteLine(Update.ToSql());
            rltcnt = Update.ExecCommand();
            successActCount += rltcnt == 1 ? 1 : 0;

            //按指定 的where更新+主键更新（如有） // Uvarchar = "`Uvarchar`+'新店开业通'"    //, Uvarchar = "新店开业通"
            Update = sqlClient.Update("Hi_TestUpdate").Set(new { Udecimal = "`Udecimal`+1", Uvarchar = "新店开业通" }).Where(new Filter { { "Uid", OperType.BETWEEN, new RangDefinition() { Low = 5, High = 5 } } });
            successCount++;
            _outputHelper.WriteLine(Update.ToSql());
            rltcnt = Update.ExecCommand();
            successActCount += rltcnt == 1 ? 1 : 0;

            Update = sqlClient.Update("Hi_TestUpdate").Set(new { Uvarchar = "新店开业通告 广东广州天河" }).Where("Uid BETWEEN 5 and 5 ");
            successCount++;
            _outputHelper.WriteLine(Update.ToSql());
            rltcnt = Update.ExecCommand();
            successActCount += rltcnt == 1 ? 1 : 0;

            Update = sqlClient.Update("Hi_TestUpdate", new { Uid = 7, Uvarchar = "新店开业通告 广东广州天河7777" }).Where(new Filter { { "Uid", OperType.BETWEEN, new RangDefinition() { Low = 7, High = 7 } } })
                .Only("Uvarchar")
                ;
            successCount++;
            _outputHelper.WriteLine(Update.ToSql());
            rltcnt = Update.ExecCommand();
            successActCount += rltcnt == 1 ? 1 : 0;


            Update = sqlClient.Update("Hi_TestUpdate", new { Uid = 6, Uvarchar = "新店开业通德发生的阿的州天河666" }).Exclude("UID");
            successCount++;
            _outputHelper.WriteLine(Update.ToSql());
            rltcnt = Update.ExecCommand();
            successActCount += rltcnt == 1 ? 1 : 0;


            Update = sqlClient.Update("Hi_TestUpdate", new List<object> { new { Uid = 6, Uvarchar = "新店开业通德发生的阿的州天河666" }, new { Uid = 7, Uvarchar = "新店开业通德发生的阿的州天河777" } }).Exclude("UID");
            successCount++;
            _outputHelper.WriteLine(Update.ToSql());
            rltcnt = Update.ExecCommand();
            successActCount += rltcnt > 0 ? 1 : 0;



            Update = sqlClient.Update("Hi_TestUpdate", new List<Hi_TestUpdate> { new Hi_TestUpdate { Uid = 8, Uvarchar = "新店开业通德发生的阿的州天河88" }, new Hi_TestUpdate { Uid = 9, Uvarchar = "新店开业通德发生的阿的州天河999" } }).Exclude("UID");
            successCount++;
            _outputHelper.WriteLine(Update.ToSql());
            rltcnt = Update.ExecCommand();
            successActCount += rltcnt > 0 ? 1 : 0;

            Assert.Equal(successActCount, successCount);
            /*
               
             */
        }

        public class Hi_TestUpdate { 
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
                _outputHelper.WriteLine($"表[{tabname1}] 存在正在执行修改并清除表结构信息");
                sqlClient.DbFirst.DropTable(tabname1);
                _outputHelper.WriteLine($"表[{tabname1}] 已经修改");
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
