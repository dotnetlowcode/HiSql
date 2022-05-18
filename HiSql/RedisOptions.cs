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

        public int DefaultExpirySecond { set; get; } = 30;
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

        public string CacheRegion { get; set; } = "HI";
        /// <summary>
        /// redis 密码 默认无密码
        /// </summary>
        public string PassWord { get => _password; set => _password = value; }

        /// <summary>
        /// 是否启用多级缓存
        /// </summary>
        public bool EnableMultiCache { get; set; } = true;

        /// <summary>
        /// 获取或设置一个值，该值指示redis缓存句柄是否应该使用键空间通知来响应，从redis中清除或过期事件，然后将这些事件转发给缓存管理器。    
        /// 具体看<see href="https://redis.io/topics/notifications"/>
        /// <para>
        /// 使用该功能更，需要先打开redis服务器配置.如配置 notify-keyspace-events "Exe"
        /// </para>
        /// </summary>
        public bool KeyspaceNotificationsEnabled { get; set; } = false;


    }
}
