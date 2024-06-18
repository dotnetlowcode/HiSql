using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{

    public abstract class BaseCache : ICache
    {
        protected string CacheRegion = string.Empty;
        protected string _lockkeyPrefix = string.Empty;
        protected string _hsetkeyPrefix = string.Empty;
        protected string _lockhashname = string.Empty;
        protected string _lockhishashname = string.Empty;


        /// <summary>
        /// 缓存 redis 脚本的ID值
        /// </summary>
        protected Dictionary<string, byte[]> dic_sha = new Dictionary<string, byte[]>();

        private CacheType _cachetype;

        protected BaseCache()
        {

        }

        /// <summary>
        /// 初始化配置
        /// </summary>
        /// <param name="cacheRegion"></param>
        protected void Init(string cacheRegion)
        {
            this.CacheRegion = cacheRegion;
            if (this.CacheRegion.IsNullOrEmpty())
            {
                this.CacheRegion = HiSql.Constants.PlatformName;
            }
            _lockhashname = $"{cacheRegion}:locktable";
            _lockhishashname = $"{cacheRegion}:locktable_his";
            _lockkeyPrefix = $"{cacheRegion}:lck:";
            _hsetkeyPrefix = $"{cacheRegion}:";
        }
        public event EventHandler<LockItemSuccessEventArgs> OnLockedSuccess;
        public bool IsSaveLockHis { get; set; } = false;

        protected void SaveLockInfo(string key, LckInfo lckInfo)
        {
            HSet(_lockhashname, key, JsonConvert.SerializeObject(lckInfo));
            if (IsSaveLockHis)
            {
                if (OnLockedSuccess == null)
                {
                    HSet(_lockhishashname, $"{key}:{lckInfo.LockTime.ToString("yyyyMMddHHmmssfff")}", JsonConvert.SerializeObject(lckInfo));
                }
                else
                {
                    OnLockedSuccess?.Invoke(this, new LockItemSuccessEventArgs(key, lckInfo));
                }
            }
        }

        public abstract CacheType CacheType { get;  }

        public abstract int Count { get; }

        public abstract Tuple<bool, string> CheckLock(params string[] keys);
        public abstract void Clear();
        public abstract void Dispose();
        public abstract bool Exists(string key);
        public abstract T GetCache<T>(string key) where T : class;
        public abstract string GetCache(string key);
        public abstract List<LckInfo> GetCurrLockInfo();
        public abstract List<LckInfo> GetHisLockInfo();
        public abstract T GetOrCreate<T>(string key, Func<T> function) where T : class;
        public abstract T GetOrCreate<T>(string key, Func<T> function, int second) where T : class;
        public abstract T GetOrCreate<T>(string key, Func<T> function, DateTimeOffset time) where T : class;
        public abstract bool HDel(string hashkey, string key);
        public abstract string HGet(string hashkey, string key);
        public abstract bool HSet(string hashkey, string key, string value);

        /// <summary>
        /// 加载lua脚本
        /// </summary>
        /// <param name="luascript"></param>
        /// <returns>返回一个sha 值有返回说明脚本加载成功</returns>
        public abstract string LoadScript(string luascript);

        /// <summary>
        /// 执行redis 脚本（提前装载至redis中的脚本）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="shaid"></param>
        /// <param name="keys"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public abstract string EvalSha(string shaid,string[] keys, object[] values);

        /// <summary>
        /// 执行redis脚本
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="luascript"></param>
        /// <param name="keys"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public abstract T ExecuteLuaScript<T>(string luascript, string[] keys, object[] values);

        public abstract Tuple<bool, string> LockOn(string key, LckInfo lckinfo, int expirySeconds = 30, int timeoutSeconds = 5);
        public abstract Tuple<bool, string> LockOn(string[] keys, LckInfo lckinfo, int expirySeconds = 30, int timeoutSeconds = 5);
        public abstract Tuple<bool, string> LockOnExecute(string key, Action action, LckInfo lckinfo, int expirySeconds = 30, int timeoutSeconds = 5);
        public abstract Tuple<bool, string> LockOnExecute(string[] keys, Action action, LckInfo lckinfo, int expirySeconds = 30, int timeoutSeconds = 5);
        public abstract Tuple<bool, string> LockOnExecuteNoWait(string key, Action action, LckInfo lckinfo, int expirySeconds = 30);
        public abstract Tuple<bool, string> LockOnExecuteNoWait(string[] keys, Action action, LckInfo lckinfo, int expirySeconds = 30);
        public abstract void RemoveCache(string key);
        public abstract void SetCache(string key, object value);
        public abstract void SetCache(string key, object value, DateTimeOffset expiryAbsoulte);
        public abstract void SetCache(string key, object value, int second);
        public abstract bool UnLock(LckInfo lckinfo, params string[] keys);
    }
}
