using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HiSql.Unit.Test
{
    [Collection("step10")]
    public class Unit_Log
    {
        private readonly ITestOutputHelper _outputHelper;

        public Unit_Log(ITestOutputHelper testOutputHelper)
        {
            _outputHelper = testOutputHelper;
        }
        [Fact(DisplayName = "SqlServer表日志")]
        [Trait("Log", "init")]
        public void LogSqlServer()
        {
            HiSqlClient sqlClient = TestClientInit.GetSqlServerClient();
            logGroups(sqlClient);
        }

        [Fact(DisplayName = "MySql表日志")]
        [Trait("Log", "init")]
        public void LogMySql()
        {
            HiSqlClient sqlClient = TestClientInit.GetMySqlClient();
            logGroups(sqlClient);

        }
        [Fact(DisplayName = "Oracle表日志")]
        [Trait("Log", "init")]
        public void LogOracle()
        {
            HiSqlClient sqlClient = TestClientInit.GetOracleClient();
            logGroups(sqlClient);
        }
        [Fact(DisplayName = "PostgreSql表日志")]
        [Trait("Log", "init")]
        public void LogPostgreSql()
        {
            HiSqlClient sqlClient = TestClientInit.GetPostgreSqlClient();
            logGroups(sqlClient);
        }
        [Fact(DisplayName = "Hana表日志")]
        [Trait("Log", "init")]
        public void LogHana()
        {
            HiSqlClient sqlClient = TestClientInit.GetHanaClient();
            logGroups(sqlClient);
        }
        [Fact(DisplayName = "Sqlite表日志")]
        [Trait("Log", "init")]
        public void LogSqlite()
        {
            HiSqlClient sqlClient = TestClientInit.GetSqliteClient();
            logGroups(sqlClient);

        }
        [Fact(DisplayName = "达梦表日志")]
        [Trait("Log", "init")]
        public void LogDaMeng()
        {
            HiSqlClient sqlClient = TestClientInit.GetDaMengClient();
            logGroups(sqlClient);
        }

        #region

        void logGroups(HiSqlClient sqlClient)
        {
            string tabname1 = "log_main_0915";
            string tabname2 = "log_detail_0915";



            TabInfo tablogtable = sqlClient.Context.DMInitalize.BuildTab(typeof(LogTable));
            TabInfo tablogdetail = sqlClient.Context.DMInitalize.BuildTab(typeof(LogTableDetail));

            tablogtable.TabModel.TabName= tabname1;
            foreach (HiColumn col in tablogtable.Columns)
            {
                col.TabName = tabname1;
            }

            tablogdetail.TabModel.TabName = tabname2;
            foreach (HiColumn col in tablogdetail.Columns)
            {
                col.TabName = tabname2;
            }


            if (sqlClient.DbFirst.CheckTabExists(tabname1))
            {
                _outputHelper.WriteLine($"表[{tabname1}] 存在正在执行删除并清除表结构信息");
                sqlClient.DbFirst.DropTable(tabname1);
                _outputHelper.WriteLine($"表[{tabname1}] 已经删除");
            }
            

            if (sqlClient.DbFirst.CheckTabExists(tabname2))
            {
                _outputHelper.WriteLine($"表[{tabname2}] 存在正在执行删除并清除表结构信息");
                sqlClient.DbFirst.DropTable(tabname2);
                _outputHelper.WriteLine($"表[{tabname2}] 已经删除");
            }
            


            bool logtableok=sqlClient.DbFirst.CreateTable(tablogtable);
            if(logtableok)
                _outputHelper.WriteLine($"表[{tabname1}] 已经创建成功");

            bool logdetailok = sqlClient.DbFirst.CreateTable(tablogdetail);
            if (logdetailok)
                _outputHelper.WriteLine($"表[{tabname2}] 已经创建成功");

            Assert.True(logtableok && logdetailok);
        }

        #endregion
    }
}
