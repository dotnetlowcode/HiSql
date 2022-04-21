using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{

    /// <summary>
    /// HiSql全局配置
    /// </summary>
    public static class Global
    {

        static RedisOptions redisOptions = null;



        static bool _redison = false;

        public static bool RedisOn
        {
            get =>  _redison; 
            set => _redison = value; 
        }
        /// <summary>
        /// 启用redis缓存 
        /// </summary>
        public static RedisOptions RedisOptions
        {
            get { return redisOptions; }
            set { redisOptions = value; }
        }

    }
}
