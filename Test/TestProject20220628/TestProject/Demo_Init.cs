using HiSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TestProject.Table;

namespace TestProject
{
    public class Demo_Init
    {


        #region hisql连接

        public static HiSqlClient GetMySqlClient()
        {
            HiSqlClient sqlclient = new HiSqlClient(
                     new HiSql.ConnectionConfig()
                     {
                         DbType = DBType.MySql,
                         DbServer = "local-HoneBI",
                         //ConnectionString = "server=192.168.1.90,8433;uid=sa;pwd=Hone@123;database=HoneBI",
                         ConnectionString = "data source=localhost;database=hone;user id=root;password=Hone@123;pooling=false;charset=utf8;AllowLoadLocalInfile=true",//; MultipleActiveResultSets = true;
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
                                 //Console.WriteLine($"共[{PageCount}]页,当前第[{CurrPage}]页");

                             }
                         }
                     }
                     );

            return sqlclient;
        }
        public static HiSqlClient GetSqlClient()
        {

            HiSqlClient sqlclient = new HiSqlClient(
                     new HiSql.ConnectionConfig()
                     {
                         DbType = DBType.SqlServer,
                         DbServer = "local-HoneBI",
                         ConnectionString = "server=(local);uid=sa;pwd=Hone@123;database=HiSql;Encrypt=True; TrustServerCertificate=True;",//; MultipleActiveResultSets = true;
                                                                                                                                           //User="tansar",//可以指定登陆用户的帐号



                         Schema = "dbo",
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


            //sqlclient.CodeFirst.InstallHisql();

            return sqlclient;
        }

        public static HiSqlClient GetPosegreClient()
        {
            HiSqlClient sqlclient = new HiSqlClient(
                     new HiSql.ConnectionConfig()
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

            return sqlclient;
        }


        public static HiSqlClient GetOracleClient()
        {
            HiSqlClient sqlclient = new HiSqlClient(
                     new HiSql.ConnectionConfig()
                     {
                         DbType = DBType.Oracle,
                         DbServer = "local-HoneBI",
                         //ConnectionString = "server=192.168.1.90,8433;uid=sa;pwd=Hone@123;database=HoneBI",
                         ConnectionString = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=orcl)));User Id=SYSTEM;Password=root",//; MultipleActiveResultSets = true;
                         Schema = "SYSTEM",
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

            return sqlclient;
        }

        #endregion


        #region sqlsugar连接
        public static SqlSugarClient GetSugarClient()
        {
            SqlSugarClient db = new SqlSugarClient(new SqlSugar.ConnectionConfig()
            {
                ConnectionString = "server=(local);uid=sa;pwd=Hone@123;database=HiSql;Encrypt=True; TrustServerCertificate=True;",//连接符字串
                DbType = SqlSugar.DbType.SqlServer,
                IsAutoCloseConnection = true
            });
            return db;
        }
        public static SqlSugarClient GetSugarMySqlClient()
        {
            SqlSugarClient db = new SqlSugarClient(new SqlSugar.ConnectionConfig()
            {
                ConnectionString = "data source=localhost;database=hone;user id=root;password=Hone@123;pooling=false;charset=utf8;AllowLoadLocalInfile=true",//连接符字串
                DbType = SqlSugar.DbType.MySql,
                IsAutoCloseConnection = true
            });
            return db;
        }
        public static SqlSugarClient GetSugarPoseGreSqlClient()
        {
            SqlSugarClient db = new SqlSugarClient(new SqlSugar.ConnectionConfig()
            {
                ConnectionString = "PORT=5432;DATABASE=postgres;HOST=localhost;PASSWORD=Hone@123;USER ID=postgres",//连接符字串
                DbType = SqlSugar.DbType.PostgreSQL,
                IsAutoCloseConnection = true
            });
            return db;
        }

        public static SqlSugarClient GetSugarOralceClient()
        {
            SqlSugarClient db = new SqlSugarClient(new SqlSugar.ConnectionConfig()
            {
                ConnectionString = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=orcl)));User Id=SYSTEM;Password=root",//连接符字串
                DbType = SqlSugar.DbType.Oracle,
                IsAutoCloseConnection = true
            });
            return db;
        }
        #endregion


        #region  EFCore连接

        public static MyDBContext GetEFCoreClient()
        {
            var context = new MyDBContext();
            return context;
        }

        public static IFreeSql GetFreeSqlClient()
        {

            IFreeSql fsql = new FreeSql.FreeSqlBuilder()
               .UseConnectionString(FreeSql.DataType.SqlServer, "server=(local);uid=sa;pwd=Hone@123;database=HiSql;Encrypt=True; TrustServerCertificate=True;")
               .UseAutoSyncStructure(true) //自动同步实体结构到数据库
               .UseNoneCommandParameter(true)
               .Build();
            return fsql;
        }

        public static IFreeSql GetFreeMySqlClient()
        {

            IFreeSql fsql = new FreeSql.FreeSqlBuilder()
               .UseConnectionString(FreeSql.DataType.MySql, "data source=localhost;database=hone;user id=root;password=Hone@123;pooling=false;charset=utf8;AllowLoadLocalInfile=true")
               .UseAutoSyncStructure(true) //自动同步实体结构到数据库
               .UseNoneCommandParameter(true)
               .Build();
            return fsql;
        }

        public static IFreeSql GetFreePosGreSqlClient()
        {

            IFreeSql fsql = new FreeSql.FreeSqlBuilder()
               .UseConnectionString(FreeSql.DataType.PostgreSQL, "PORT=5432;DATABASE=postgres;HOST=localhost;PASSWORD=Hone@123;USER ID=postgres")
               .UseAutoSyncStructure(true) //自动同步实体结构到数据库
               .UseNoneCommandParameter(true)
               .Build();
            return fsql;
        }

        public static IFreeSql GetFreeOraclelClient()
        {

            IFreeSql fsql = new FreeSql.FreeSqlBuilder()
               .UseConnectionString(FreeSql.DataType.Oracle, "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=orcl)));User Id=SYSTEM;Password=root")
               .UseAutoSyncStructure(true) //自动同步实体结构到数据库
               .UseNoneCommandParameter(true)
               .Build();
            return fsql;
        }
        #endregion



        #region dapper 连接

        public static IDbConnection GetDapperSqlClient()
        {
            return new SqlConnection("server=(local);uid=sa;pwd=Hone@123;database=HiSql;Encrypt=True; TrustServerCertificate=True;");
        }

        #endregion
    }

    public class MyDBContext : DbContext
    {
        public DbSet<Table.H_Test50C05> H_Test50C05 { get; set; }
        public DbSet<Table.HTest05> HTest05 { get; set; }

        //这里是配置
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //DbContextOptionsBuilder注入
            base.OnConfiguring(optionsBuilder);
            //设置连接字符串
            optionsBuilder.UseSqlServer("server=(local);uid=sa;pwd=Hone@123;database=HiSql;Encrypt=True; TrustServerCertificate=True;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HTest05>().HasKey("SID"); 
            modelBuilder.Entity<H_Test50C05>().HasKey("Material","Batch");
            
            base.OnModelCreating(modelBuilder);
            //获取当前程序集默认的是查找所有继承了IEntityTypeConfiguration的类
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);

            
        }
    }
}
