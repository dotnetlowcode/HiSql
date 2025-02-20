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
        /// 缓存区域名称。建议以系统名称命名。如 CRM
        /// </summary>
        public string CacheRegion { get; set; }
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

        /// <summary>
        /// 是否启用zip压缩
        /// </summary>
        public bool IsZip { get; set; } = false;

        /// <summary>
        /// 字符长度超过该值则进行压缩
        /// </summary>
        public int ZipLen { get; set; } = 200;

        /// <summary>
        /// 是否强制解压 
        /// 如果开启了强制解压即当所有的值从redis获取过来都尝试解压看数据是否是被压缩过的
        /// 如果为否则当内容大于等于ZipLen 的值时才会进行尝试解压，理论上讲可以提升性能，弊端时如果redis中的数据  设置过多个ZipLen不同值的数据 可能会出现解析错误
        /// </summary>
        public bool IsForceDeZip { get; set; } = false;

    }
}
