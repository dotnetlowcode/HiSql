using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// 锁配置参数
    /// </summary>
    public class LockOptions
    {
        /// <summary>
        /// 非阻塞模式，获取锁默认等等多少毫秒
        /// </summary>
        public  int NoWaitModeGetLockWaitMillSeconds = 600;
        /// <summary>
        /// 获取锁重试的时候，等待多少毫秒
        /// </summary>
        public  int GetLockRetrySleepMillSeconds = 50;

    }
}
