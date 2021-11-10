using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public class MCache :ICache
    {
        public MCache(MemoryCacheOptions? options)//这里可以做成依赖注入，但没打算做成通用类库，所以直接把选项直接封在帮助类里边
        {
            //this._cache = new MemoryCache(options);
            if (options == null)
            {
                options = new MemoryCacheOptions();
            }
            this._cache = new MemoryCache(options);
        }


        private IMemoryCache _cache;


        public bool Exists(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));


            object v = null;
            return this._cache.TryGetValue<object>(key, out v);
        }


        public T GetCache<T>(string key) where T : class
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));


            T v = null;
            this._cache.TryGetValue<T>(key, out v);

            
            return v;
        }


        public void SetCache(string key, object value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            if (value == null)
                throw new ArgumentNullException(nameof(value));


            object v = null;
            if (this._cache.TryGetValue(key, out v))
                this._cache.Remove(key);
            this._cache.Set<object>(key, value);
        }


        public void SetCache(string key, object value, double expirationMinute)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            

            object v = null;
            if (this._cache.TryGetValue(key, out v))
                this._cache.Remove(key);
            DateTime now = DateTime.Now;
            TimeSpan ts = now.AddMinutes(expirationMinute) - now;
            this._cache.Set<object>(key, value, ts);
        }


        public void SetCache(string key, object value, DateTimeOffset expirationTime)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            

            object v = null;
            if (this._cache.TryGetValue(key, out v))
                this._cache.Remove(key);
            

            this._cache.Set<object>(key, value, expirationTime);
        }
        public void SetCache(string key, object value, int second)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            object v = null;
            if (this._cache.TryGetValue(key, out v))
                this._cache.Remove(key);

            DateTimeOffset expirationTime = DateTimeOffset.Now;
            expirationTime = expirationTime.AddSeconds(second);
            this._cache.Set<object>(key, value, expirationTime);
        }


        public void RemoveCache(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));


            this._cache.Remove(key);
        }


        public void Dispose()
        {
            if (_cache != null)
                _cache.Dispose();
            GC.SuppressFinalize(this);
        }

        public T GetOrCreate<T>(string key, Func<T> value)
        {
            return this._cache.GetOrCreate<T>(key, (entry)=> {

                var objs = value.Invoke();
                
                entry.SetValue (objs);
                
                return objs;

            });
        }

        /// <summary>
        /// 获取或创建缓存 并设置有效期
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public T GetOrCreate<T>(string key, Func<T> value,int second)
        {
            return this._cache.GetOrCreate<T>(key, (entry) => {

                DateTimeOffset expirationTime = DateTimeOffset.Now;
                expirationTime = expirationTime.AddSeconds(second);
                var objs = value.Invoke();
                entry.SetValue(objs);
                entry.AbsoluteExpiration = expirationTime;
                return objs;

            });
        }

        public T GetOrCreate<T>(string key, Func<T> value,DateTimeOffset time)
        {
            return this._cache.GetOrCreate<T>(key, (entry) => {

                var objs = value.Invoke();
                entry.SetValue(objs);
                entry.AbsoluteExpiration = time;
                return objs;

            });
        }
    }
}
