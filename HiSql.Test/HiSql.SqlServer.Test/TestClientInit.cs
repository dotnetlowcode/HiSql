using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql.Unit.Test
{
    /// <summary>
    /// 数据库连接初始化
    /// </summary>
    public class TestClientInit
    {
        /// <summary>
        /// 获取sqlserver连接
        /// </summary>
        /// <returns></returns>
        public static HiSqlClient GetSqlServerClient()
        {
            return new HiSqlClient(
                 new ConnectionConfig()
                 {
                     DbType = DBType.SqlServer,
                     DbServer = "local-HiSql",
                     ConnectionString = @"server=(local);uid=sa;pwd=Hone@123;database=HiSql;Encrypt=True; TrustServerCertificate=True;",//; MultipleActiveResultSets = true;
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




        /// <summary>
        /// 获取sqlite数据库连接
        /// </summary>
        /// <returns></returns>
        public static HiSqlClient GetSqliteClient()
        {
            //Global.RedisOn = true;
            // Global.RedisOptions = new RedisOptions { Host = "127.0.0.1", Port = 6379, PassWord = "", CacheRegion = "", Database = 3, EnableMultiCache = false, KeyspaceNotificationsEnabled = false };

            string dbName = Path.Combine(Environment.CurrentDirectory, "SampleDB3.db");
            string connStr = new SqliteConnectionStringBuilder()
            {
                DataSource = dbName
                ,
                //  Password = "admin"
            }.ToString();

            HiSqlClient sqlclient = new HiSqlClient(
                     new ConnectionConfig()
                     {
                         DbType = DBType.Sqlite,
                         DbServer = "local-HoneBI",
                         ConnectionString = connStr,
                         User = "hisql",//可以指定登陆用户的帐号
                         Schema = "main",
                         IsEncrypt = true,
                         IsAutoClose = false,
                         SqlExecTimeOut = 60000,
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


            sqlclient.CodeFirst.InstallHisql();
            return sqlclient;
        }


        /// <summary>
        /// 获取postgresql连接
        /// </summary>
        /// <returns></returns>
        public static HiSqlClient GetPostgreSqlClient()
        {
            HiSqlClient sqlclient = new HiSqlClient(
                     new ConnectionConfig()
                     {
                         DbType = DBType.PostGreSql,
                         DbServer = "local-HoneBI",
                         //ConnectionString = "server=192.168.1.90,8433;uid=sa;pwd=Hone@123;database=HoneBI",
                         ConnectionString = "PORT=5432;DATABASE=postgres;HOST=localhost;PASSWORD=Hone@123;USER ID=postgres",//; MultipleActiveResultSets = true;
                         Schema = "public",
                         IsEncrypt = true,
                         IsAutoClose = false,
                         SqlExecTimeOut = 60000,

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
                             },
                             OnPageExec = (int PageCount, int CurrPage) =>
                             {
                                 ///当批量执行并进行分包执行后 可以接收分页执行情况
                                 Console.WriteLine($"共[{PageCount}]页,当前第[{CurrPage}]页");

                             }
                         }
                     }
                     );
            Console.WriteLine($" 版本号：{sqlclient.Context.DMTab.DBVersion().Version}  版本描述：{sqlclient.Context.DMTab.DBVersion().VersionDesc}");
            return sqlclient;
        }


        /// <summary>
        /// 获取oralce连接
        /// </summary>
        /// <returns></returns>
        public static HiSqlClient GetOracleClient()
        {
            HiSqlClient sqlclient = new HiSqlClient(
                     new ConnectionConfig()
                     {
                         DbType = DBType.Oracle,
                         DbServer = "local-HoneBI",
                         //ConnectionString = "server=192.168.1.90,8433;uid=sa;pwd=Hone@123;database=HoneBI",
                         ConnectionString = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=orcl)));User Id=SYSTEM;Password=root",//; MultipleActiveResultSets = true;
                         Schema = "SYSTEM",
                         //ConnectionString = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.10.172)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=helowinXDB)));User Id=test;Password=test",//; MultipleActiveResultSets = true;
                         //Schema = "test",
                         IsEncrypt = true,
                         IsAutoClose = false,
                         SqlExecTimeOut = 60000,
                         IgnoreCase = true,
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
                             },
                             OnPageExec = (int PageCount, int CurrPage) =>
                             {
                                 ///当批量执行并进行分包执行后 可以接收分页执行情况
                                 Console.WriteLine($"共[{PageCount}]页,当前第[{CurrPage}]页");

                             }
                         }
                     }
                     );

            Console.WriteLine($" 版本号：{sqlclient.Context.DMTab.DBVersion().Version}  版本描述：{sqlclient.Context.DMTab.DBVersion().VersionDesc}");
            return sqlclient;
        }


        /// <summary>
        /// 获取mysql连接
        /// </summary>
        /// <returns></returns>
        public static HiSqlClient GetMySqlClient()
        {
            HiSqlClient sqlclient = new HiSqlClient(
                     new ConnectionConfig()
                     {
                         DbType = DBType.MySql,
                         DbServer = "local-HoneBI",
                         //ConnectionString = "server=192.168.1.90,8433;uid=sa;pwd=Hone@123;database=HoneBI",
                         ConnectionString = "data source=192.168.10.172;Port=8029;database=hone;user id=root;password=hone@123;charset=utf8",//; MultipleActiveResultSets = true;
                                                                                                                                                                           //ConnectionString = "data source=192.168.10.172;database=hone;user id=root;password=hone@123;charset=utf8",//; MultipleActiveResultSets = true;
                                                                                                                                                                           // ConnectionString = "data source=192.168.10.172;Port=8029;database=hone;user id=root;password=hone@123;charset=utf8",//; MultipleActiveResultSets = true;
                                                                                                                                                                           //ConnectionString = "data source=192.168.10.172;Port=8000;database=hone;user id=root;password=hone@123;charset=utf8",
                                                                                                                                                                           //ConnectionString = "data source=127.0.0.1;Port=5706;database=hone;user id=root;password=hone@123;charset=utf8",

                         Schema = "hone",
                         IsEncrypt = true,
                         IsAutoClose = false,
                         SqlExecTimeOut = 60000,

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
                             },
                             OnPageExec = (int PageCount, int CurrPage) =>
                             {
                                 ///当批量执行并进行分包执行后 可以接收分页执行情况
                                 Console.WriteLine($"共[{PageCount}]页,当前第[{CurrPage}]页");

                             }
                         }
                     }
                     );

            Console.WriteLine($" 版本号：{sqlclient.Context.DMTab.DBVersion().Version}  版本描述：{sqlclient.Context.DMTab.DBVersion().VersionDesc}");
            return sqlclient;
        }


        /// <summary>
        /// 获取hana连接
        /// </summary>
        /// <returns></returns>
        public static HiSqlClient GetHanaClient()
        {

            string dbstr = "DRIVER=HDBODBC;UID=SAPHANADB;PWD=Hone@crd@2019;SERVERNODE =192.168.10.243:31013;DATABASENAME =QAS";
            HiSqlClient sqlclient = new HiSqlClient(
                     new ConnectionConfig()
                     {
                         DbType = DBType.Hana,
                         DbServer = "local-HoneBI",
                         ConnectionString = dbstr,//; MultipleActiveResultSets = true;
                         Schema = "SAPHANADB",
                         IsEncrypt = true,
                         IsAutoClose = false,
                         SqlExecTimeOut = 60000,

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
                             },
                             OnPageExec = (int PageCount, int CurrPage) =>
                             {
                                 ///当批量执行并进行分包执行后 可以接收分页执行情况
                                 Console.WriteLine($"共[{PageCount}]页,当前第[{CurrPage}]页");

                             }
                         }
                     }
                     );

            Console.WriteLine($" 版本号：{sqlclient.Context.DMTab.DBVersion().Version}  版本描述：{sqlclient.Context.DMTab.DBVersion().VersionDesc}");
            return sqlclient;
        }


        /// <summary>
        /// 获取达梦连接
        /// </summary>
        /// <returns></returns>
        public static HiSqlClient GetDaMengClient()
        {
            HiSqlClient sqlclient = new HiSqlClient(
                     new ConnectionConfig()
                     {
                         DbType = DBType.DaMeng,
                         DbServer = "local-HoneBI",
                         //ConnectionString = "server=192.168.1.90,8433;uid=sa;pwd=Hone@123;database=HoneBI",
                         ConnectionString = "Server=192.168.10.175; UserId=SYSDBA; PWD=SYSDBA",//; MultipleActiveResultSets = true;
                         Schema = "SYSDBA",
                         IsEncrypt = true,
                         IsAutoClose = false,
                         SqlExecTimeOut = 60000,
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
                             },
                             OnPageExec = (int PageCount, int CurrPage) =>
                             {
                                 ///当批量执行并进行分包执行后 可以接收分页执行情况
                                 Console.WriteLine($"共[{PageCount}]页,当前第[{CurrPage}]页");

                             }
                         }
                     }
                     );

            Console.WriteLine($" 版本号：{sqlclient.Context.DMTab.DBVersion().Version}  版本描述：{sqlclient.Context.DMTab.DBVersion().VersionDesc}");
            return sqlclient;
        }
    }
}
