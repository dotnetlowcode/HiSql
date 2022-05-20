using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public interface ILock
    {

        /// <summary>
        /// 获取当前lock信息
        /// </summary>
        /// <returns></returns>
        public List<LckInfo> GetCurrLockInfo();
        /// <summary>
        /// 获取历史lock信息
        /// </summary>
        /// <returns></returns>
        public List<LckInfo> GetHisLockInfo();
        /// <summary>
        /// 加业务锁
        /// </summary>
        /// <param name="key">自定锁的KEY</param>
        /// <param name="expresseconds">锁的周期时间 单位秒</param>
        /// <param name="timeoutseconds">加锁等待超时时间 单位秒</param>
        /// <returns></returns>
        public Tuple<bool, string> LockOn(string key, LckInfo lckinfo, int expresseconds = 30, int timeoutseconds = 5);
        /// <summary>
        /// 加业务锁
        /// </summary>
        /// <param name="key">自定锁的KEY，支持多个key</param>
        /// <param name="expresseconds">锁的周期时间 单位秒</param>
        /// <param name="timeoutseconds">加锁等待超时时间 单位秒</param>
        /// <returns></returns>
        public Tuple<bool, string> LockOn(string[] keys, LckInfo lckinfo, int expresseconds = 30, int timeoutseconds = 5);


        /// <summary>
        /// 检测key是否已经锁定，支持同时锁多个key
        /// </summary>
        /// <param name="key">自定锁的KEY，支持多个key</param>
        /// <returns></returns>
        public Tuple<bool, string> CheckLock(params string[] keys);



        /// <summary>
        /// 加业务锁
        /// </summary>
        /// <param name="key">自定锁的KEY</param>
        /// <param name="action">加锁后执行的业务</param>
        /// <param name="expresseconds">锁的周期时间 单位秒</param>
        /// <param name="timeoutseconds">加锁等待超时时间 单位秒</param>
        /// <returns></returns>
        public Tuple<bool, string> LockOnExecute(string key, Action action, LckInfo lckinfo, int expresseconds = 30, int timeoutseconds = 5);
        /// <summary>
        /// 加业务锁，支持同时锁多个key
        /// </summary>
        /// <param name="key">一个或多个key</param>
        /// <param name="action">加锁后执行的业务</param>
        /// <param name="expresseconds">锁的周期时间 单位秒</param>
        /// <param name="timeoutseconds">加锁等待超时时间 单位秒</param>
        /// <returns></returns>
        public Tuple<bool, string> LockOnExecute(string[] keys, Action action, LckInfo lckinfo, int expresseconds = 30, int timeoutseconds = 5);

        /// <summary>
        /// 移除锁，支持多个key
        /// </summary>
        /// <param name="keys">一个或多个key</param>
        /// <returns></returns>
        public bool UnLock(params string[] keys);
    }
}
