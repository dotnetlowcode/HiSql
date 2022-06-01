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

        /// <summary>
        /// 非阻塞模式，获取锁默认等等多少毫秒
        /// </summary>
        public static int NoWaitModeGetLockWaitMillSeconds = 600;
        /// <summary>
        /// 获取锁重试的时候，等待多少毫秒
        /// </summary>
        public static int GetLockRetrySleepMillSeconds = 50;

        static bool _redison = false;

        public static bool RedisOn
        {
            get =>  _redison;
            set { _redison = value;
                CacheContext.Reset();

            }
        }
        /// <summary>
        /// 启用redis缓存 
        /// </summary>
        public static RedisOptions RedisOptions
        {
            get { return redisOptions; }
            set { redisOptions = value; 
                if (_redison && redisOptions != null)
                    CacheContext.Reset();
            }
        }

    }
}
