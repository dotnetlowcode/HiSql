using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public interface ICache
    {
        bool Exists(string key);


        T GetCache<T>(string key) where T : class;
        
        string GetCache(string key);

        void SetCache(string key, object value);


        void SetCache(string key, object value, DateTimeOffset expiressAbsoulte);//设置绝对时间过期

        //设置多少秒后过期
        void SetCache(string key, object value, int second);

        //void SetCache(string key, object value, double expirationMinute);  //设置滑动过期， 因redis暂未找到自带的滑动过期类的API，暂无需实现该接口

        T GetOrCreate<T>(string key, Func< T> value);


        T GetOrCreate<T>(string key, Func<T> value,int second);

        T GetOrCreate<T>(string key, Func<T> value, DateTimeOffset time);

        void RemoveCache(string key);


        void Dispose();


        /// <summary>
        /// 设置hash值
        /// </summary>
        /// <param name="hashkey"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool HSet(string hashkey, string key, string value);


        /// <summary>
        /// 获取hash值
        /// </summary>
        /// <param name="hashkey"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string HGet(string hashkey, string key);

        /// <summary>
        /// hash 删除
        /// </summary>
        /// <param name="hashkey"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool HDel(string hashkey, string key);

        /// <summary>
        /// 获取当前lock信息
        /// </summary>
        /// <returns></returns>
        public List<LckInfo> GetCurrLockInfo();
        /// <summary>
        /// 获取当前lock信息
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
        /// 检测key是否已经锁定
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Tuple<bool, string> CheckLock(string key);


        /// <summary>
        /// 加业务锁
        /// </summary>
        /// <param name="key">自定锁的KEY</param>
        /// <param name="action">加锁后执行的业务</param>
        /// <param name="expresseconds">锁的周期时间 单位秒</param>
        /// <param name="timeoutseconds">加锁等待超时时间 单位秒</param>
        /// <returns></returns>
        public Tuple<bool, string> LockOnExecute(string key, Action action, LckInfo lckinfo, int expresseconds = 30, int timeoutseconds = 5);



        public bool UnLock(string key);
    }
}
