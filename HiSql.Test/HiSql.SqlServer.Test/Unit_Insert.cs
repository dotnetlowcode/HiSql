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
    [Collection("step2")]
    public class Unit_Insert
    {
        private readonly ITestOutputHelper _outputHelper;
        public Unit_Insert(ITestOutputHelper testOutputHelper)
        {
            _outputHelper = testOutputHelper;
        }
        #region 10列5条数据插入
        [Fact(DisplayName = "SqlServer表创建10列-5-1W条")]
        [Trait("TableInsert10col", "init")]
        public void TableInsert10_5SqlServer()
        {
            HiSqlClient sqlClient = TestClientInit.GetSqlServerClient();
            insertGroups(sqlClient);

        }
        [Fact(DisplayName = "MySql表创建10列-5-1W条")]
        [Trait("TableInsert10col", "init")]
        public void TableInsert10_5MySql()
        {
            HiSqlClient sqlClient = TestClientInit.GetMySqlClient();
            insertGroups(sqlClient);

        }
        [Fact(DisplayName = "Oracle表创建10列-5-1W条")]
        [Trait("TableInsert10col", "init")]
        public void TableInsert10_5Oracle()
        {
            HiSqlClient sqlClient = TestClientInit.GetOracleClient();
            insertGroups(sqlClient);


        }
        [Fact(DisplayName = "PostgreSql表创建10列-5-1W条")]
        [Trait("TableInsert10col", "init")]
        public void TableInsert10_5PostgreSql()
        {
            HiSqlClient sqlClient = TestClientInit.GetPostgreSqlClient();
            insertGroups(sqlClient);

        }
        [Fact(DisplayName = "Hana表创建10列-5-1W条")]
        [Trait("TableInsert10col", "init")]
        public void TableCreatHana()
        {
            HiSqlClient sqlClient = TestClientInit.GetHanaClient();
            insertGroups(sqlClient);

        }
        [Fact(DisplayName = "Sqlite表创建10列-5-1W条")]
        [Trait("TableInsert10col", "init")]
        public void TableInsert10_5Sqlite()
        {
            HiSqlClient sqlClient = TestClientInit.GetSqliteClient();
            insertGroups(sqlClient);

        }
        [Fact(DisplayName = "达梦表创建10列-5-1W条")]
        [Trait("TableInsert10col", "init")]
        public void TableDaMeng()
        {
            HiSqlClient sqlClient = TestClientInit.GetDaMengClient();
            insertGroups(sqlClient);
        }

        #endregion

        #region 公共方法
        public void InsertData(HiSqlClient sqlClient, int count)
        {
            insertData(sqlClient, count);
        }


        #endregion

        #region 私有方法




        void insertGroups(HiSqlClient sqlClient)
        {
            sqlClient.CurrentConnectionConfig.AppEvents = GetAopEvent();
            int count = 5;
            _outputHelper.WriteLine($"准备向表中插入[{count}]条数据测试");
            insertData(sqlClient, count);


            count = 10;
            _outputHelper.WriteLine($"准备向表中插入[{count}]条数据测试");
            insertData(sqlClient, count);


            count = 50;
            _outputHelper.WriteLine($"准备向表中插入[{count}]条数据测试");
            insertData(sqlClient, count);

            count = 100;
            _outputHelper.WriteLine($"准备向表中插入[{count}]条数据测试");
            insertData(sqlClient, count);

            count = 500;
            _outputHelper.WriteLine($"准备向表中插入[{count}]条数据测试");
            insertData(sqlClient, count);

            count = 1000;
            _outputHelper.WriteLine($"准备向表中插入[{count}]条数据测试");
            insertData(sqlClient, count);

            count = 5000;
            _outputHelper.WriteLine($"准备向表中插入[{count}]条数据测试");
            insertData(sqlClient, count);

            count = 10000;
            _outputHelper.WriteLine($"准备向表中插入[{count}]条数据测试");
            insertData(sqlClient, count);
        }

        void insertData(HiSqlClient sqlClient,int count)
        {
            string tabname1 =typeof(H_Test01).Name;
        
            _outputHelper.WriteLine($"清除表[{tabname1}]中数据");
            sqlClient.DbFirst.Truncate(tabname1);

            List<object > lstdata = buildData10Col(count);

            //预热
            var _json=  sqlClient.HiSql($"select * from {tabname1}").Skip(1).Take(1).ToJson();


            Stopwatch sw = new Stopwatch();
            sw.Start();
            sqlClient.Insert(tabname1, lstdata).ExecCommand();
            sw.Stop();
            _outputHelper.WriteLine($"向表[{tabname1}]插入[{count}]条数据 耗时：[{sw.Elapsed.ToString()}]秒");

            _outputHelper.WriteLine($"数据插入完成正在验证数据记录数");
            int scount=sqlClient.DbFirst.GetTableDataCount(tabname1);

            Assert.True(count == scount);



        }



        /// <summary>
        /// 10列以下数据插入
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        List<object> buildData10Col(int count)
        {
            List<object> lstobj = new List<object>();
            Random random = new Random();
            for (int i = 0; i < count; i++)
            {
                //hisql可以用实体类也可以用匿名类
                lstobj.Add(new H_Test01 { SID = (i + 1), UName = $"hisql{i}", Age = 20 + (i % 50), Salary = 5000 + (i % 2000) + random.Next(10), Descript = $"hisql初始创建" });
            }
            return lstobj; 
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
