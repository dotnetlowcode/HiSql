using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql.UnitTest
{
    class Demo_Init
    {
        
        public static HiSqlClient GetSqlClient()
        {
            //Global.RedisOn = true;
           // Global.RedisOptions = new RedisOptions { Host = "127.0.0.1", Port = 6379, PassWord = "", CacheRegion = "", Database = 3, EnableMultiCache = false, KeyspaceNotificationsEnabled = false };
            HiSqlClient sqlclient = new HiSqlClient(
                     new ConnectionConfig()
                     {
                         DbType = DBType.SqlServer,
                         DbServer = "local-HoneBI",
                         //ConnectionString = "server=(local);uid=sa;pwd=Hone@123;database=Hone;Encrypt=True; TrustServerCertificate=True;",//; MultipleActiveResultSets = true;
                         //User = "tansar",//可以指定登陆用户的帐号
                         //ConnectionString = "server=121.201.110.194,43109; uid=HoneDev;pwd=Dev@Hone; database=Hone_Flow_Dev;Encrypt=True; TrustServerCertificate=True;",//; MultipleActiveResultSets = true;
                         ConnectionString = "Data Source=192.168.10.88,7433;Initial Catalog=ThirdApi_Dev_Dev;User Id=dev003;Password=dev003@123;Connect Timeout=900;TrustServerCertificate=True;Max Pool Size=300;",//; MultipleActiveResultSets = true;
                         //SlaveConnectionConfigs = new List<SlaveConnectionConfig> {
                         //    { new SlaveConnectionConfig{ ConnectionString=" server=(local);uid=sa;pwd=Hone@123;database=Hone;Encrypt=True; TrustServerCertificate=True;" , Weight=3} },
                         //    //{ new SlaveConnectionConfig{ ConnectionString=" server=(local);uid=sa;pwd=Hone@123;database=HiSql; " , Weight=3} },
                         //    //{ new SlaveConnectionConfig{ ConnectionString="  server=(local);uid=sa;pwd=Hone@123;database=HiSql;" , Weight=4} },
                         //    //{ new SlaveConnectionConfig{ ConnectionString="    erver=(local);uid=sa;pwd=Hone@123;database=HiSql;" , Weight=10} }
                         //},
                         Schema = "dbo",
                         IsEncrypt = false,
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


            //sqlclient.CodeFirst.InstallHisql();

            Console.WriteLine($" 版本号：{sqlclient.Context.DMTab.DBVersion().Version}  版本描述：{sqlclient.Context.DMTab.DBVersion().VersionDesc}");
            return sqlclient;
        }

    }
}
