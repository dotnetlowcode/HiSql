using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
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
        [Trait("Insert", "init")]

        public void TableInsert10_5SqlServer()
        {
            HiSqlClient sqlClient = TestClientInit.GetSqlServerClient();
            insertGroups(sqlClient);

        }
        [Fact(DisplayName = "MySql表创建10列-5-1W条")]
        [Trait("Insert", "init")]
        public void TableInsert10_5MySql()
        {
            HiSqlClient sqlClient = TestClientInit.GetMySqlClient();
            insertGroups(sqlClient);

        }
        [Fact(DisplayName = "Oracle表创建10列-5-1W条")]
        [Trait("Insert", "init")]
        public void TableInsert10_5Oracle()
        {
            HiSqlClient sqlClient = TestClientInit.GetOracleClient();
            insertGroups(sqlClient);


        }
        [Fact(DisplayName = "PostgreSql表创建10列-5-1W条")]
        [Trait("Insert", "init")]
        public void TableInsert10_5PostgreSql()
        {
            HiSqlClient sqlClient = TestClientInit.GetPostgreSqlClient();
            insertGroups(sqlClient);

        }
        [Fact(DisplayName = "Hana表创建10列-5-1W条")]
        [Trait("Insert", "init")]
        public void TableCreatHana()
        {
            HiSqlClient sqlClient = TestClientInit.GetHanaClient();
            insertGroups(sqlClient);

        }
        [Fact(DisplayName = "Sqlite表创建10列-5-1W条")]
        [Trait("Insert", "init")]
        public void TableInsert10_5Sqlite()
        {
            HiSqlClient sqlClient = TestClientInit.GetSqliteClient();
            insertGroups(sqlClient);

        }
        [Fact(DisplayName = "达梦表创建10列-5-1W条")]
        [Trait("Insert", "init")]
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

            insertDefault(sqlClient);
            insertAutoIncreate(sqlClient);

            int count = 5;
            //bulkcopyInsertData(sqlClient, count, "Hi_Test_bulkcopyInsertData");

            //return;
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


            insertNullData(sqlClient);

            
        }

        void insertDefault(HiSqlClient sqlClient)
        {
            //将createtime ModiTime的默认值去掉测试 插入值
            if (sqlClient.DbFirst.CheckTabExists("Hi_Test2022"))
            {
                sqlClient.DbFirst.Truncate("Hi_Test2022");
                sqlClient.Insert("Hi_Test2022", new { SID = 1, SNAME = "tansar", SDESC = "测试" }).ExecCommand();
            }
        }

        void insertAutoIncreate(HiSqlClient sqlClient)
        {


            //var _sql = sqlClient.HiSql("select * from WeatherForecast2").ToSql();

            if (sqlClient.DbFirst.CheckTabExists(nameof(WeatherForecast)))
                sqlClient.DbFirst.DropTable(nameof(WeatherForecast));

            sqlClient.DbFirst.CreateTable(typeof(WeatherForecast));

            List<object> lstobj = new List<object>();
            for (int i = 0; i < 100; i++)
            {
                lstobj.Add(new { Summary=$"test{i}" , Date=DateTime.Now });
            }
            sqlClient.Insert(nameof(WeatherForecast), lstobj).ExecCommand();

        }
        void bulkcopyInsertData(HiSqlClient sqlClient, int count, string tabname)
        {

            //创建表
            if (sqlClient.DbFirst.CheckTabExists(tabname))
            {
                sqlClient.DbFirst.DropTable(tabname);
            }
            bool iscreate = sqlClient.DbFirst.CreateTable(Test.TestTable.DynTable.BuildTabInfo(tabname, true));

            TabInfo tabinfo = sqlClient.Context.DMInitalize.GetTabStruct(tabname);

            var lstdata = TestTable.DynTable.BuildTabDataList(tabname, count);

            sqlClient.TrunCate(tabname).ExecCommand();

            DataTable dt = DataConvert.ToTable(lstdata, tabinfo);
           
            int _effect = sqlClient.BulkCopyExecCommand(tabname, dt);
            int datacnt = sqlClient.DbFirst.GetTableDataCount(tabname);
            Assert.True(_effect == count && datacnt  == count);


            var lstdataEntity = TestTable.DynTable.BuildTabDataEntityList(tabname, count);

            sqlClient.TrunCate(tabname).ExecCommand();

            dt = DataConvert.ToTable(lstdataEntity, tabinfo);

            _effect = sqlClient.BulkCopyExecCommand(tabname, dt);
            datacnt = sqlClient.DbFirst.GetTableDataCount(tabname);
            Assert.True(_effect == count && datacnt == count);



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
                        { "SID",4},
                         { "uname","tansar"},
                        { "gname","tgm"}
                   
                    }
                }).ExecCommand();


                sqlClient.Insert(tabname, new List<Dictionary<string, object>> {
                    new Dictionary<string, object>
                    {
                        { "SID",5},
                        { "uname","tansar"}
                    },
                    new Dictionary<string, object>
                    {
                        { "SID",6},
                        { "uname","tansar"},
                        { "birth",DateTime.Now}
                    },
                    new Dictionary<string, object>
                    {
                        { "SID",7},
                        { "gname","tgm"},
                        { "birth",DateTime.Now}
                    },
                    new Dictionary<string, object>
                    {
                        { "SID",8},
                         { "uname","tansar"},
                        { "gname","tgm"}

                    }
                }).ExecCommandAsync();


            }
            else
            {
                _outputHelper.WriteLine($" 创建Null值 测试表[{tabname}]失败");
                Assert.True(false);
            }

        }

        void insertData(HiSqlClient sqlClient,int count)
        {
            string tabname1 =typeof(H_Test01).Name;

            if (sqlClient.DbFirst.CheckTabExists(tabname1))
            {
                sqlClient.DbFirst.DropTable(tabname1);
               
            }
            sqlClient.DbFirst.CreateTable(typeof(H_Test01));
            _outputHelper.WriteLine($"清除表[{tabname1}]中数据");
            //sqlClient.DbFirst.Truncate(tabname1);

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
