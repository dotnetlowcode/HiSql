using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;
namespace HiSql
{
    /// <summary>
    /// 基于Redis的缓存
    /// </summary>
    public class RCache : IRedis
    {

        private StackExchange.Redis.IDatabase _cache;
        private ConnectionMultiplexer _connectMulti;


        RedisOptions _options;
        public RCache(RedisOptions options)
        { 
            if(options==null)
                throw new ArgumentNullException("options");

           
            this._options = options;
            checkRedis();
            

        }


        /// <summary>
        /// 对Key加上前辍
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string checkKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            //if (!key.StartsWith($"{Constants.KEY_PRE}:"))
            //    key = $"{Constants.KEY_PRE}:{key}";
            return key;
        }

        private void checkRedis()
        {
            if (_connectMulti == null || !_connectMulti.IsConnected)
            {
                string _connstr = $"{this._options.Host}:{this._options.Port}";
                if (!string.IsNullOrEmpty(this._options.PassWord))
                    _connstr = $"{_connstr},password={this._options.PassWord}";
                _connectMulti = ConnectionMultiplexer.Connect(_connstr);
                _cache = _connectMulti.GetDatabase(this._options.Database);
            }
        }


        public void Dispose()
        {
            if (_connectMulti != null)
            {
                _connectMulti.Close();
                _connectMulti.Dispose();
            }

            
        }

        public bool Exists(string key)
        {
            checkRedis();
            key = checkKey(key);
            return _cache.KeyExists(key);
            
        }
        public string GetCache(string key)
        {
            checkRedis();
            key = checkKey(key);

            var val=_cache.StringGet(key);
            if (val.HasValue)
                return val.ToString();
            else
                return string.Empty;
        }

        public T GetCache<T>(string key) where T : class
        {
            checkRedis();
            key = checkKey(key);

            var value=default(T);
            Type type = typeof(T);
            var cachevalue= _cache.StringGet(key);
            if (!cachevalue.IsNull)
            {

                

                value=JsonConvert.DeserializeObject<T>(cachevalue);
            }
            return value;
            
        }

        public T GetOrCreate<T>(string key, Func<T> value)
        {
            checkRedis();
            key = checkKey(key);
            if (!Exists(key))
            {
                T _val = value.Invoke();
                string _value = JsonConvert.SerializeObject(_val);
                _cache.StringSet(key, _value, null, When.NotExists, CommandFlags.None);
                return _val;
            }
            else
            {
                T _val = default(T);
                var cachevalue = _cache.StringGet(key);
                if (!cachevalue.IsNull)
                {

                    _val = JsonConvert.DeserializeObject<T>(cachevalue);
                }
                return _val;
            }
        }

        public T GetOrCreate<T>(string key, Func<T> value, int second)
        {
            checkRedis();
            key = checkKey(key);
            if (!Exists(key))
            {
                T _val = value.Invoke();
                string _value = JsonConvert.SerializeObject(_val);
                _cache.StringSet(key, _value, TimeSpan.FromSeconds(second), When.NotExists, CommandFlags.None);
                return _val;
            }
            else
            {
                T _val = default(T);
                var cachevalue = _cache.StringGet(key);
                if (!cachevalue.IsNull)
                {

                    _val = JsonConvert.DeserializeObject<T>(cachevalue);
                }
                return _val;
            }

        }

        /// <summary>
        /// 如果有缓存则返回缓存没有则创建缓存 并返回缓存对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="time">过期时间</param>
        /// <returns></returns>
        public T GetOrCreate<T>(string key, Func<T> value, DateTimeOffset time)
        {
            checkRedis();
            key = checkKey(key);
            DateTimeOffset currdate = DateTimeOffset.Now;

            int _seconds = int.Parse( time.Subtract(currdate).TotalSeconds.ToString());

            if (_seconds > 0)
            {
                if (!Exists(key))
                {
                    T _val = value.Invoke();
                    string _value = JsonConvert.SerializeObject(_val);
                    _cache.StringSet(key, _value, TimeSpan.FromSeconds(_seconds), When.NotExists, CommandFlags.None);
                    return _val;
                }
                else
                {
                    T _val = default(T);
                    var cachevalue = _cache.StringGet(key);
                    if (!cachevalue.IsNull)
                    {

                        _val = JsonConvert.DeserializeObject<T>(cachevalue);
                    }
                    return _val;
                }
            }
            else
                return default(T);
        }

        public void RemoveCache(string key)
        {
            checkRedis();
            key = checkKey(key);
            _cache.KeyDelete(key);
        }

        public void SetCache(string key, object value)
        {
            checkRedis();
            key = checkKey(key);

            string _value = JsonConvert.SerializeObject(value);
            _cache.StringSet(key, _value);
        }

        public void SetCache(string key, object value, DateTimeOffset expiressAbsoulte)
        {
            checkRedis();
            key = checkKey(key);
            DateTimeOffset currdate = DateTimeOffset.Now;
            
            int _seconds = int.Parse(expiressAbsoulte.Subtract(currdate).TotalSeconds.ToString());
            if (_seconds > 0)
            {
                string _value = JsonConvert.SerializeObject(value);
                _cache.StringSet(key, _value, TimeSpan.FromSeconds(_seconds));
            }
        }

        public void SetCache(string key, object value, int second)
        {
            checkRedis();
            key = checkKey(key);
            if (second > 0)
            {
                string _value = JsonConvert.SerializeObject(value);
                _cache.StringSet(key, _value, TimeSpan.FromSeconds(second));
            }
        }

        /// <summary>
        /// 广播方式订阅 不按发布顺序接口
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="handler"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void BroadCastSubScriber(string channel, Action<string, string> handler = null)
        {
            checkRedis();
            channel = checkKey(channel);

            _connectMulti.GetSubscriber().Subscribe(channel, (rchannel, message) => {
                handler(rchannel, message.ToString());
            });
            
        }

        /// <summary>
        /// 队列方式订阅 按发布顺序接收
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="handler"></param>
        public void QueueSubScriber(string channel, Action<string, string> handler = null)
        {
            checkRedis();
            channel = checkKey(channel);

            _connectMulti.GetSubscriber().Subscribe(channel).OnMessage(message => {
                handler(channel, message.ToString());
            });
        }

        /// <summary>
        /// 向某一个频道发送消息
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public long PublishMessage(string channel, string message)
        {
            checkRedis();
            channel = checkKey(channel);
            return _connectMulti.GetSubscriber().Publish(channel, message);
        }

        /// <summary>
        /// 入列
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public long ListPush(string channel, string value)
        {
            checkRedis();
            channel = checkKey(channel);
            return _cache.ListLeftPush(channel, value);
           
        }

        /// <summary>
        /// 先进先出 出列
        /// 最先入列的最先出列
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public string ListFirstPop(string channel)
        {
            checkRedis();
            channel = checkKey(channel);
            return _cache.ListRightPop(channel);
        }
        /// <summary>
        /// 后进先出 出列
        /// 最后一个入列最先出列
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>

        public string ListLastPop(string channel)
        {
            checkRedis();
            channel = checkKey(channel);
            return _cache.ListLeftPop(channel);
        }
        /// <summary>
        /// 获取队列的记录数
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public long ListCount(string channel)
        {
            checkRedis();
            channel = checkKey(channel);
            return _cache.ListLength(channel);
        }



        /// <summary>
        /// 解业务锁
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>

        public Tuple<bool, string> UnLock(string key)
        {
            checkRedis();
            key = checkKey(key);

            _cache.LockRelease(key, System.Threading.Thread.CurrentThread.ManagedThreadId);
            throw new NotImplementedException();
        }

        /// <summary>
        /// 加业务锁
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expresseconds"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Tuple<bool, string> LockOn(string key, int expresseconds=5)
        {
            checkRedis();
            key = checkKey(key);

            expresseconds= expresseconds < 0? 5 : expresseconds;
            expresseconds = expresseconds > 60 ? 60 : expresseconds;

            bool isok = false;
            while (!isok)
            { 
                isok=_cache.LockTake(key,System.Threading.Thread.CurrentThread.ManagedThreadId,TimeSpan.FromSeconds(expresseconds));
                if (isok)
                {
                    break;
                }
            }
            throw new NotImplementedException();
        }
        /// <summary>
        /// 续锁
        /// 对已经加了锁且快到期了就进行续锁
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expresseconds"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>

        public Tuple<bool, string> KeepLockOn(string key, int expresseconds)
        {
            throw new NotImplementedException();
        }

       
    }
}
