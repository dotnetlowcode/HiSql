using HiSql.Unit.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HiSql.Unit.Test
{
    [Collection("step2")]
    public class Unit_TableCreate
    {
        private readonly ITestOutputHelper _outputHelper;
        public Unit_TableCreate(ITestOutputHelper testOutputHelper)
        {
            _outputHelper = testOutputHelper;
        }

        [Fact(DisplayName = "SqlServerge表创建")]
        [Trait("TableCreate", "init")]
        public void TableCreateSqlServer()
        {
            HiSqlClient sqlClient = TestClientInit.GetSqlServerClient();

            


            createDemoTable(sqlClient);


        }
        [Fact(DisplayName = "MySql表创建")]
        [Trait("TableCreate", "init")]
        public void TableCreateMySql()
        {
            HiSqlClient sqlClient = TestClientInit.GetMySqlClient();
            createDemoTable(sqlClient);

        }
        [Fact(DisplayName = "Oracle表创建")]
        [Trait("TableCreate", "init")]
        public void TableCreateOracle()
        {
            HiSqlClient sqlClient = TestClientInit.GetOracleClient();
            createDemoTable(sqlClient);

        }
        [Fact(DisplayName = "PostgreSql表创建")]
        [Trait("TableCreate", "init")]
        public void TableCreatePostgreSql()
        {
            HiSqlClient sqlClient = TestClientInit.GetPostgreSqlClient();

            

            createDemoTable(sqlClient);

        }
        [Fact(DisplayName = "Hana表创建")]
        [Trait("TableCreate", "init")]
        public void TableCreatHana()
        {
            HiSqlClient sqlClient = TestClientInit.GetHanaClient();
            createDemoTable(sqlClient);

        }
        [Fact(DisplayName = "Sqlite表创建")]
        [Trait("TableCreate", "init")]
        public void TableCreateSqlite()
        {
            HiSqlClient sqlClient = TestClientInit.GetSqliteClient();
            createDemoTable(sqlClient);

        }
        [Fact(DisplayName = "达梦表创建")]
        [Trait("TableCreate", "init")]
        public void TableDaMeng()
        {
            HiSqlClient sqlClient = TestClientInit.GetDaMengClient();
            createDemoTable(sqlClient);

        }


        #region
        void createDemoTable(HiSqlClient sqlClient)
        {
            sqlClient.CurrentConnectionConfig.AppEvents = GetAopEvent();

            string tabname1 = typeof(H_Test50C01).Name;
            string tabname2 = typeof(H_Test01).Name;

            _outputHelper.WriteLine($"检测表[{tabname1}] 是否在当前库中存在");
            if (sqlClient.DbFirst.CheckTabExists(tabname1))
            {
                _outputHelper.WriteLine($"表[{tabname1}] 存在正在执行删除并清除表结构信息");
                sqlClient.DbFirst.DropTable(tabname1);
                _outputHelper.WriteLine($"表[{tabname1}] 已经删除");
            }else
                _outputHelper.WriteLine($"检测表[{tabname1}] 是否在当前库中不存在");



            _outputHelper.WriteLine($"检测表[{tabname2}] 是否在当前库中存在");
            if (sqlClient.DbFirst.CheckTabExists(tabname2))
            {
                _outputHelper.WriteLine($"表[{tabname2}] 存在正在执行删除并清除表结构信息");
                sqlClient.DbFirst.DropTable(tabname2);
                _outputHelper.WriteLine($"表[{tabname2}] 已经删除");
            }
            else
                _outputHelper.WriteLine($"检测表[{tabname2}] 是否在当前库中不存在");


            _outputHelper.WriteLine($"正在准备创建表[{tabname1}]");

            sqlClient.DbFirst.CreateTable(sqlClient.Context.DMInitalize.BuildTab(typeof(H_Test50C01)));

            _outputHelper.WriteLine($"正在准备创建表[{tabname2}]");
            sqlClient.DbFirst.CreateTable(sqlClient.Context.DMInitalize.BuildTab(typeof(H_Test01)));



            _outputHelper.WriteLine($"正在Truncate 表[{tabname1}]");
            sqlClient.DbFirst.Truncate(tabname1);
            _outputHelper.WriteLine($"正在Truncate 表[{tabname2}]");
            sqlClient.DbFirst.Truncate(tabname2);


            //_outputHelper.WriteLine($"正在准备创建表[{tabname1}]");

            //sqlClient.DbFirst.CreateTable(sqlClient.Context.DMInitalize.BuildTab(typeof(H_Test50C01)));

            //_outputHelper.WriteLine($"正在准备创建表[{tabname2}]");
            //sqlClient.DbFirst.CreateTable(sqlClient.Context.DMInitalize.BuildTab(typeof(H_Test01)));





        }


        AopEvent GetAopEvent()
        { 
            return new AopEvent()
            {
                OnDbDecryptEvent = (connstr) =>
                {
                    //解密连接字段
                    //Console.WriteLine($"数据库连接:{connstr}");

                    return connstr;
                },
                OnLogSqlExecuting = (sql, param) =>
                {
                    //sql执行前 日志记录 (异步)
                    _outputHelper.WriteLine($"OnLogSqlExecuting:{System.Environment.NewLine}{sql}");
                    //Console.WriteLine($"sql执行前记录{sql} time:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")}");
                },
                OnLogSqlExecuted = (sql, param) =>
                {
                    //sql执行后 日志记录 (异步)
                    //Console.WriteLine($"sql执行后记录{sql} time:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")}");
                },
                OnSqlError = (sqlEx) =>
                {
                    //sql执行错误后 日志记录 (异步)
                    _outputHelper.WriteLine($"OnSqlError:{System.Environment.NewLine}{sqlEx.Message.ToString()}");
                },
                OnTimeOut = (int timer) =>
                {
                    //Console.WriteLine($"执行SQL语句超过[{timer.ToString()}]毫秒...");
                }
            };

        }


        #endregion


    }
}
