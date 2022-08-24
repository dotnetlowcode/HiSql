using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace HiSql.SqlServer.Test
{


    [HiTable(TabName = "H_TEST")]
    class H_Test
    {
        [HiColumn(FieldDesc = "自增主键", IsSys = true, IsNull = false, IsBllKey = true, IsPrimary = true, SortNum = 10, IsIdentity = true)]
        public int Id { get; set; }

        public int DID
        {
            get; set;
        }
        public string UNAME
        {
            get; set;
        }
        public string UNAME2
        {
            get; set;
        }

    }


    public class UnitMainTest
    {

        HiSqlClient sqlClient;

        string tableName = "H_TEST";

        public UnitMainTest()
        {
            sqlClient = new HiSqlClient(
                 new ConnectionConfig()
                 {
                     DbType = DBType.SqlServer,
                     DbServer = "local-HiSql",
                     ConnectionString = @"server=(local);uid=sa;pwd=Hone@123;database=Hone;Encrypt=True; TrustServerCertificate=True;",//; MultipleActiveResultSets = true;
                                                                                                                   //User="tansar",//可以指定登陆用户的帐号

                     SlaveConnectionConfigs = new List<SlaveConnectionConfig> {
                             { new SlaveConnectionConfig{ ConnectionString=@"server=(local);uid=sa;pwd=Hone@123;database=Hone;Encrypt=True; TrustServerCertificate=True;" , Weight=3} },
                             //{ new SlaveConnectionConfig{ ConnectionString=" server=(local);uid=sa;pwd=Hone@123;database=HiSql; " , Weight=3} },
                             //{ new SlaveConnectionConfig{ ConnectionString="  server=(local);uid=sa;pwd=Hone@123;database=HiSql;" , Weight=4} },
                             //{ new SlaveConnectionConfig{ ConnectionString="    erver=(local);uid=sa;pwd=Hone@123;database=HiSql;" , Weight=10} }

                         },

                     Schema = "dbo",
                     IsEncrypt = true,
                     IsAutoClose = false,
                     SqlExecTimeOut = 1,

                     AppEvents = new AopEvent()
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
                             Console.WriteLine(sqlEx.Message.ToString());
                         },
                         OnTimeOut = (int timer) =>
                         {
                             //Console.WriteLine($"执行SQL语句超过[{timer.ToString()}]毫秒...");
                         }
                     }
                 }
                 );
        }


        [Fact(Skip ="暂时不测")]
        public void InitDB()
        {
            sqlClient.CodeFirst.InstallHisql();
            Assert.True(true);
        }

        [Fact(Skip = "暂时不测")]
        public async Task InitTable()
        {
            try
            {
                sqlClient.Context.DBO.ExecCommand("DROP TABLE[dbo].[H_TEST]");
                var tab = sqlClient.Context.DMInitalize.BuildTab(typeof(H_Test));
                var sql = sqlClient.Context.DMTab.BuildTabCreateSql(tab.TabModel, tab.Columns, isdrop: true);
                int k = await sqlClient.Context.DBO.ExecCommandAsync(sql);
                Assert.True(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }



        [Fact(Skip = "暂时不测")]
        public async void Insert()
        {
            int _times = 100000;
            Console.WriteLine($"[{_times}]条数据 HANA插入测试");
            List<object> lstobj = new List<object>();
            for (int i = 0; i < _times; i++)
            {
                lstobj.Add(new
                {
                    DID = i,
                    UNAME = $"U{i}",
                    UNAME2 = $"用户{i}"
                });
            }
            Console.WriteLine($"[{_times}]条数据 预热完成正在插入");
            Stopwatch watch = Stopwatch.StartNew();
            int _effect2 = await sqlClient
            .Insert(tableName, lstobj)
            .ExecCommandAsync();


            //string _sql2 = sqlClient
            //.Insert("Hi_Domain", lstobj)
            //.ToSql();

            watch.Stop();
            Console.WriteLine($"[{_times}]条数据,耗时：{watch.Elapsed.ToString()}");
            Assert.True(true);
        }




        [Fact(Skip = "暂时不测")]
        public void Update2()
        {
            IUpdate update = sqlClient.Update(tableName, new { DID = 1, UNAME = "UTYPE", UNAME2 = "user123" });//.Exclude("UNAME");//,  1' or '1'='1'
            //int _effect = sqlClient.Update(tableName, new { DID = 1, UNAME = "UTYPE", UNAME2 = "user123" }).Exclude("UNAME").ExecCommand();
            string _dicsql = update.ToSql();
            update.ExecCommand();
            if (!Regex.Match(_dicsql.Trim(), "update(.*?)set(.*?)$", RegexOptions.IgnoreCase).Success)
            {
                Assert.True(true);
            }
            //"update(.*?)set(.*?)\\n"
            Console.WriteLine(_dicsql);
        }

        [Fact(Skip = "暂时不测")]
        public void Update()
        {
            IUpdate update1 = sqlClient.Update(tableName).Set(new { UNAME2 = "TEST" }).Where(new Filter { { "DID", OperType.GT, 8 } });
            //int _effect1 = sqlClient.Update(tableName).Set(new { UNAME2 = "TEST" }).Where(new Filter { { "DID", OperType.GT, 8 } }).ExecCommand();
            string _sql1 = update1.ToSql();
            if (!Regex.Match(_sql1.Trim(), "update(.*?)set(.*?)UNAME2(.*?)where(.*?)DID(.*?)>[\\s]{0,1}\\d{1,}", RegexOptions.IgnoreCase).Success)
            {
                Assert.True(true);
            }
            Console.WriteLine(_sql1);
        }

        [Fact(Skip = "暂时不测")]
        public void Update4()
        {
            IUpdate update2 = sqlClient.Update(tableName, new { DID = 1, UNAME2 = "Haha1" }).Only("UNAME2");
            //int _effect2 = sqlClient.Update(new  { DID = 1, UNAME2 = "Haha1" }).Only("UNAME2").ExecCommand();
            string _sql2 = update2.ToSql();
            if (!Regex.Match(_sql2.Trim(), "update(.*?)set(.*?)UNAME2(.*?)$", RegexOptions.IgnoreCase).Success)
            {
                Assert.True(true);
            }
            Console.WriteLine(_sql2);
        }



        [Fact(Skip = "暂时不测")]
        public void Delete()
        {
            //Dictionary<string, string> _dic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { "DID", "1" } };
            IDelete dic_delete = sqlClient.Delete(tableName).Where(new Filter() { { "UNAME", OperType.EQ, "00" } });
            dic_delete.ExecCommand();
            string _dicsql = dic_delete.ToSql();
            if (!Regex.Match(_dicsql.Trim(), $"^delete(.*?){tableName}(.*?)where(.*?)[\\s]+=[\\s]+(.*)$", RegexOptions.IgnoreCase).Success)
            {
                Assert.True(false);
            }
        }

        [Fact(Skip = "暂时不测")]
        public void Delete2()
        {
            IDelete delete = sqlClient.Delete(tableName);
            //int _effect = sqlClient.Delete("H_Test").ExecCommand();
            string _sql = delete.ToSql();
            if (!Regex.Match(_sql.Trim(), $"^delete(.*?){tableName}(.*?)$", RegexOptions.IgnoreCase).Success)
            {
                Assert.True(false);
            }
        }
        [Fact(Skip = "暂时不测")]
        public void Delete3()
        {
            IDelete delete1 = sqlClient.Delete(tableName).Where(new Filter { { "DID", OperType.GT, 200 } });
            //int _effect1 = sqlClient.Delete("H_Test").Where(new Filter { { "DID", OperType.GT, 200 } }).ExecCommand();
            string _sql1 = delete1.ToSql();
            if (!Regex.Match(_sql1.Trim(), $"^delete(.*?){tableName}(.*?)where(.*?)[\\s]+>[\\s]+[\\d]+$", RegexOptions.IgnoreCase).Success)
            {
                Assert.True(false);
            }
        }

        [Fact(Skip = "暂时不测")]
        public void Delete4()
        {
            IDelete delete2 = sqlClient.Delete(tableName, new H_Test { Id = 99 });
            //int _effect2 = sqlClient.Delete("H_Test", new { DID = 99 }).ExecCommand();
            string _sql2 = delete2.ToSql();
            if (!Regex.Match(_sql2.Trim(), "^delete(.*?)" + tableName + "(.*?)where(.*?)[\\s]{0,}=[\\s]{0,}[\\d]+$", RegexOptions.IgnoreCase).Success)
            {
                Assert.True(false);
            }
        }

        [Fact(Skip = "暂时不测")]
        public void Delete5()
        {
            IDelete delete3 = sqlClient.Delete(tableName, new List<object> { new { DID = 99, UNAME2 = "user'123" }, new { DID = 100, UNAME2 = "user124" } });
            //int _effect3 = sqlClient.Delete("H_Test", new List<object> { new { DID = 99, UNAME2 = "user123" }, new { DID = 100, UNAME2 = "user124" } }).ExecCommand();
            string _sql3 = delete3.ToSql();
            // "delete(.*?)H_TEST(.*?)where(.*?)=[\\s]{0,}\'(.*?)\'$"
            if (!Regex.Match(_sql3.Trim(), "^delete(.*?)" + tableName + "(.*?)where(.*?)=[\\s]{0,}\'(.*?)\'", RegexOptions.IgnoreCase).Success)
            {
                Assert.True(false);
            }
        }


        [Fact(Skip = "暂时不测")]
        public void Delete6()
        {
            IDelete delete5 = sqlClient.TrunCate(tableName);
            //int _effect5 = sqlClient.TrunCate("H_Test").ExecCommand();
            string _sql5 = delete5.ToSql();
            //
            if (!Regex.Match(_sql5.Trim(), "TRUNCATE[\\s]{1,}TABLE[\\s]{1,}(.*?)$", RegexOptions.IgnoreCase).Success)
            {
                Assert.True(false);
            }

        }

        [Fact(Skip = "暂时不测")]
        public void Query()
        {
            //sqlClient.HiSql($"select * from ");
            var data = sqlClient.Query("Hi_TabModel").Field("*").Where(new Filter { { "TabNameA", OperType.EQ, tableName } }).ToEObject(); //.ToList<Dictionary<string, string>>();
            Assert.True(true);
        }
    }
}
