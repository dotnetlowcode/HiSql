using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HiSql.Unit.Test
{
    [Collection("step4")]
    public class Unit_Modi
    {
        private readonly ITestOutputHelper _outputHelper;

        public Unit_Modi(ITestOutputHelper testOutputHelper)
        {
            _outputHelper = testOutputHelper;
        }

        #region modi修改表数据
        [Fact(DisplayName = "SqlServer表modi操作")]
        [Trait("Modi", "init")]
        public void TableModi_SqlServer()
        {
            HiSqlClient sqlClient = TestClientInit.GetSqlServerClient();
            ModiGroups(sqlClient);
        }


        [Fact(DisplayName = "MySql表modi操作")]
        [Trait("Modi", "init")]
        public void TableModi_MySql()
        {
            HiSqlClient sqlClient = TestClientInit.GetMySqlClient();
            ModiGroups(sqlClient);
        }


        [Fact(DisplayName = "Oracle表modi操作")]
        [Trait("Modi", "init")]
        public void TableModi_Oracle()
        {
            HiSqlClient sqlClient = TestClientInit.GetOracleClient();
            ModiGroups(sqlClient);
        }

        [Fact(DisplayName = "PostgreSql表modi操作")]
        [Trait("Modi", "init")]
        public void TableModi_PostgreSql()
        {
            HiSqlClient sqlClient = TestClientInit.GetPostgreSqlClient();
            ModiGroups(sqlClient);
        }


        [Fact(DisplayName = "Hana表modi操作")]
        [Trait("Modi", "init")]
        public void TableModi_Hana()
        {
            HiSqlClient sqlClient = TestClientInit.GetHanaClient();
            ModiGroups(sqlClient);
        }


        [Fact(DisplayName = "Sqlite表modi操作")]
        [Trait("Modi", "init")]
        public void TableModi_Sqlite()
        {
            HiSqlClient sqlClient = TestClientInit.GetSqliteClient();
            ModiGroups(sqlClient);
        }


        [Fact(DisplayName = "DaMeng表modi操作")]
        [Trait("Modi", "init")]
        public void TableModi_DaMeng()
        {
            HiSqlClient sqlClient = TestClientInit.GetDaMengClient();
            ModiGroups(sqlClient);
        }
        #endregion
        #region 私有方法

        void ModiGroups(HiSqlClient sqlClient)
        {
            sqlClient.CurrentConnectionConfig.AppEvents = GetAopEvent();
            createTextTable(sqlClient);
            int count = 10;


            string tabname=typeof(H_Test02).Name;
            List<object> lstdata = buildData10Col(count);

            //预热
            var _json = sqlClient.HiSql($"select * from {tabname}").Skip(1).Take(1).ToJson();


            try
            {
                sqlClient.Modi(tabname, lstdata).ExecCommand();
                Assert.True(true);

            }catch (Exception ex)
            {
                if (sqlClient.CurrentConnectionConfig.DbType == DBType.Hana && ex.Message.Contains("TEXT")) {
                    Assert.True(true);
                }else
                    Assert.False(true);

            }


            transDemo(sqlClient);


        }

        void transDemo(HiSqlClient sqlClient)
        {
            int count = 10;
            string tabname = typeof(H_Test02).Name;
            List<object> lstdata = buildData10Col(count);

            sqlClient.Delete(tabname).ExecCommand();

            sqlClient.CurrentConnectionConfig.IsAutoClose = false;
            using (var sqlClt = sqlClient.CreateUnitOfWork())
            { 
                sqlClt.Insert(tabname, lstdata).ExecCommand();

                sqlClt.Modi(tabname, lstdata).ExecCommand();
                sqlClt.CommitTran();
                //sqlClt.RollBackTran();
            }


        }
        

        List<object> buildData10Col(int count)
        {
            List<object> lstobj = new List<object>();
            Random random = new Random();
            for (int i = 0; i < count; i++)
            {
                //hisql可以用实体类也可以用匿名类
                lstobj.Add(new H_Test02 { SID = (i + 1), UName = $"hisql{i}", Age = 20 + (i % 50), Salary = 5000 + (i % 2000) + random.Next(10), Descript = $"hisql初始创建" });
            }
            return lstobj;
        }
        void createTextTable(HiSqlClient sqlClient)
        {
            string tabname1 = typeof(H_Test02).Name;

            if (sqlClient.DbFirst.CheckTabExists(tabname1))
            {
                sqlClient.DbFirst.DropTable(tabname1);

            }
            sqlClient.DbFirst.CreateTable(typeof(H_Test02));
            _outputHelper.WriteLine($"清除表[{tabname1}]中数据");
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
