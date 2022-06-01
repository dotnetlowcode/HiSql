using System;

namespace HiSql
{
    public interface ICache:ILock
    {
       
        /// <summary>
        /// 判断一个key在缓存中在不在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Exists(string key);

        /// <summary>
        /// 根据key获取一个泛型缓存，未获取到返回null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T GetCache<T>(string key) where T : class;

        /// <summary>
        /// 根据key获取一个缓存，未获取到返回string.Empty或null
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetCache(string key);

        /// <summary>
        /// 设置一个缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void SetCache(string key, object value);

        /// <summary>
        /// 设置一个缓存,并设置有效期
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiressAbsoulte">绝对时间过期</param>
        void SetCache(string key, object value, DateTimeOffset expiressAbsoulte);//设置绝对时间过期

        /// <summary>
        /// 设置一个缓存,并设置有效期
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="second">设置多少秒后过期</param>
        void SetCache(string key, object value, int second);

        //void SetCache(string key, object value, double expirationMinute);  //设置滑动过期， 因redis暂未找到自带的滑动过期类的API，暂无需实现该接口
        /// <summary>
        /// 根据key获取一个泛型缓存，未获取到则执行函数 function，并将函数结果存入缓存，并返回函数结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="function"></param>
        /// <returns></returns>
        T GetOrCreate<T>(string key, Func< T> function) where T : class;

        /// <summary>
        /// 根据key获取一个泛型缓存，未获取到则执行函数 function，并将函数结果存入缓存，并返回函数结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="function"></param>
        /// <param name="second">过期时间单位秒</param>
        /// <returns></returns>
        T GetOrCreate<T>(string key, Func<T> function, int second) where T : class;

        /// <summary>
        /// 根据key获取一个泛型缓存，未获取到则执行函数 function，并将函数结果存入缓存，并返回函数结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="function"></param>
        /// <param name="time">绝对时间过期</param>
        /// <returns></returns>
        T GetOrCreate<T>(string key, Func<T> function, DateTimeOffset time) where T : class;

        /// <summary>
        /// 返回键总数
        /// </summary>
        public int Count { get; }


        /// <summary>
        /// 缓存类型
        /// </summary>
        public CacheType CacheType { get;  }

        /// <summary>
        /// 移除一个缓存
        /// </summary>
        /// <param name="key"></param>
        void RemoveCache(string key);

        /// <summary>
        /// 清除所有缓存
        /// </summary>
        void Clear();

        /// <summary>
        /// 释放缓存连接对象  执行与释放或重置非托管资源关联的应用程序定义的任务
        /// </summary>
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

    }
}
