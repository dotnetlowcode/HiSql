using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql.MySqlUnitTest
{
    class Demo_Init
    {
        public static HiSqlClient GetSqlClient()
        {
            HiSqlClient sqlclient = new HiSqlClient(
                     new ConnectionConfig()
                     {
                         DbType = DBType.MySql,
                         DbServer = "local-HoneBI",
                         //ConnectionString = "server=192.168.1.90,8433;uid=sa;pwd=Hone@123;database=HoneBI",
                         ConnectionString = "data source=127.0.0.1,3306;database=hone;user id=root;password=Hone@123;pooling=false;charset=utf8;AllowLoadLocalInfile=true",//; MultipleActiveResultSets = true;
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
    }
}
