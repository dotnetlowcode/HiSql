using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{

    /// <summary>
    /// 高性能自定义编号规则创建
    /// add by tgm date:2022.7.7
    /// </summary>
    public class SnroNumber
    {

        /// <summary>
        /// 数据库类型
        /// </summary>
        public static DBType DBType { get; set; }


        /// <summary>
        /// 数据库连接
        /// </summary>
        public static string ConnectionString { get; set; }


        public static string User { get; set; }

        public static string Schema { get; set; }

        static SeriNumber seriNumber;

        static object _objkey=new object ();

        static HiSqlClient _sqlClient;

        static HiSqlClient __sqlClient;

        public static HiSqlClient SqlClient
        {
            set { 
                __sqlClient= value;
                ConnectionString=__sqlClient.CurrentConnectionConfig.ConnectionString;
                DBType = __sqlClient.CurrentConnectionConfig.DbType;
                User = __sqlClient.CurrentConnectionConfig.User;
                Schema = __sqlClient.CurrentConnectionConfig.Schema;
            }
            get { 
                return __sqlClient;
            }
        }


        public static string NewNumber(string snro, int snum)
        {

            checkConnect();

            return seriNumber.NewNumber(snro,snum);
        }
        public static List<string> NewNumber(string snro, int snum, int count)
        {
            checkConnect();
            return seriNumber.NewNumber(snro,snum,count);
        }


        static void checkConnect()
        {
            if (seriNumber == null)
            {
                if (string.IsNullOrEmpty(ConnectionString))
                    throw new Exception($"未配置编号的连接库连接");

                if (string.IsNullOrEmpty(Schema))
                    throw new Exception($"未配置编号的Schema");


                lock (_objkey)
                {
                    _sqlClient = new HiSqlClient(
                     new ConnectionConfig()
                     {
                         DbType = DBType,
                         DbServer = "hisql-snro",
                         ConnectionString = ConnectionString,//; MultipleActiveResultSets = true;
                         User = User == null ? "hisql" : User,//可以指定登陆用户的帐号

                         Schema = Schema,
                         IsEncrypt = false,
                         IsAutoClose = false,
                         SqlExecTimeOut = 60000,

                     }
                     );

                    seriNumber = new SeriNumber(_sqlClient);

                }
            }
        }



    }
}
