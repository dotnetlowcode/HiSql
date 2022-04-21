using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public class RedisOptions
    {


        int _database = 0;
        int _port = 6379;
        string _password = string.Empty;

        string _host = string.Empty;


        /// <summary>
        /// redis地址
        /// </summary>
        public string Host { get => _host; set => _host = value; }
        /// <summary>
        /// redis 连接端口 默认6379
        /// </summary>
        public int Port { get => _port; set => _port = value; }

        /// <summary>
        /// 数据库编号 默认0
        /// </summary>
        public int Database { get => _database; set => _database = value; }


        /// <summary>
        /// redis 密码 默认无密码
        /// </summary>
        public string PassWord { get => _password; set => _password = value; }
    }
}
