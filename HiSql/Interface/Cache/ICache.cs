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
    }
}
