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
    public class RCache : ICache
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
    }
}
