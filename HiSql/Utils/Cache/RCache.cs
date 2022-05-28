using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;
namespace HiSql
{
    /// <summary>
    /// 基于Redis的缓存
    /// </summary>
    public class RCache : BaseCache,IRedis
    {
        
        private readonly string UniqueId = Guid.NewGuid().ToString();
        private StackExchange.Redis.IDatabase _cache;
        private ConnectionMultiplexer _connectMulti;
        private string _cache_notity_channel_remove = "hisql_cache_notity_channel@{0}__:remove";
        private MCache _MemoryCache;
        private RedisOptions _options;
        public RCache(RedisOptions options)
        {
            if (options == null)
                throw new ArgumentNullException("options");

            if (options.Host.IsNullOrEmpty())
            {
                throw new ArgumentException("Value must not be empty", nameof(options.Host));
            }
            //if (options.CacheRegion.IsNullOrEmpty())
            //{
            //    throw new ArgumentException("Value must not be empty", nameof(options.CacheRegion));
            //}
            if (options.CacheRegion.IsNullOrEmpty())
            {
                options.CacheRegion = HiSql.Constants.PlatformName;
            }
            this._options = options;

            base.Init(this._options.CacheRegion);

            CheckRedis();

            //_lockhashname = GetRegionKey(_lockhashname);
            //_lockhishashname = GetRegionKey(_lockhishashname);
            if (this._options.EnableMultiCache)
            {
                _MemoryCache = new MCache(_options.CacheRegion);
                _cache_notity_channel_remove = string.Format(_cache_notity_channel_remove, options.Database.ToString());
                //订阅移除事件
                this.BroadCastSubScriber(_cache_notity_channel_remove, (rchannel, key) =>
                {
                    var tupple = ParseRegionKey(key);
                    if (!tupple.Item3.IsNullOrEmpty() && tupple.Item3 != this.UniqueId)
                    {
                        _MemoryCache.RemoveCache($"{tupple.Item2}:{tupple.Item1}");
                        //Console.WriteLine($"Got remove event for key   {tupple.Item2}:{tupple.Item1} 需要移除");
                        return;
                    }

                    //Console.WriteLine($"Got remove event for key   {tupple.Item2}:{tupple.Item1} 不需要移除");

                });
                ///启用消息通知
                if (_options.KeyspaceNotificationsEnabled)
                {
                    this.BroadCastSubScriber($"__keyevent@{options.Database}__:expired", (rchannel, key) =>
                    {
                        var tupple = ParseRegionKey(key);
                        if (!tupple.Item3.IsNullOrEmpty() && tupple.Item3 != this.UniqueId)
                        {
                            _MemoryCache.RemoveCache($"{tupple.Item2}:{tupple.Item1}");
                            //Console.WriteLine($"Got expired event for key   {tupple.Item2}:{tupple.Item1} 需要移除");
                            return;
                        }
                        //Console.WriteLine($"Got expired event for key   {tupple.Item2}:{tupple.Item1} 不需要移除");

                    });
                    this.BroadCastSubScriber($"__keyevent@{options.Database}__:evicted", (rchannel, key) =>
                    {
                        var tupple = ParseRegionKey(key);
                        if (!tupple.Item3.IsNullOrEmpty() && tupple.Item3 != this.UniqueId)
                        {
                            _MemoryCache.RemoveCache($"{tupple.Item2}:{tupple.Item1}");
                            //Console.WriteLine($"Got evicted event for key   {tupple.Item2}:{tupple.Item1} 需要移除");
                            return;
                        }
                        //Console.WriteLine($"Got evicted event for key   {tupple.Item2}:{tupple.Item1} 不需要移除");
                    });
                    this.BroadCastSubScriber($"__keyevent@{options.Database}__:del", (rchannel, key) =>
                    {
                        var tupple = ParseRegionKey(key);
                        if (!tupple.Item3.IsNullOrEmpty() && tupple.Item3 != this.UniqueId)
                        {
                            _MemoryCache.RemoveCache($"{tupple.Item2}:{tupple.Item1}");
                            //Console.WriteLine($"Got del event for key   {tupple.Item2}:{tupple.Item1} 需要移除");
                            return;
                        }
                        //Console.WriteLine($"Got del event for key   {tupple.Item2}:{tupple.Item1} 不需要移除");
                    });
                }
            }
        }

        private const string Base64Prefix = "base64\0";
        private Tuple<string, string, string> ParseRegionKey(string value)
        {
            if (value == null)
            {
                return Tuple.Create<string, string, string>(null, null, null);
            }
            var unionId = string.Empty;
            var unionIdIndex = value.IndexOf(keySplitKey);
            if (unionIdIndex > 0)
            {
                unionId = value.Substring(unionIdIndex + keySplitKey.Length);
                value = value.Substring(0, unionIdIndex);
            }

            var sepIndex = value.IndexOf(':');

            var hasRegion = sepIndex > 0;
            var key = value;
            string region = null;

            if (hasRegion)
            {
                region = value.Substring(0, sepIndex);
                key = value.Substring(sepIndex + 1);

                if (region.StartsWith(Base64Prefix))
                {
                    region = region.Substring(Base64Prefix.Length);
                    region = Encoding.UTF8.GetString(Convert.FromBase64String(region));
                }
            }

            if (key.StartsWith(Base64Prefix))
            {
                key = key.Substring(Base64Prefix.Length);
                key = Encoding.UTF8.GetString(Convert.FromBase64String(key));
            }

            return Tuple.Create(key, region, unionId);
        }

        private string GetRegionKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (key.StartsWith(_options.CacheRegion + ":"))
                return key;

            //if (!key.Contains(HiSql.Constants.NameSpace))
            //    key = HiSql.Constants.NameSpace + ":" + key;

            //if (_options.KeyspaceNotificationsEnabled && key.Contains(":"))
            //{
            //    key = Base64Prefix + Convert.ToBase64String(Encoding.UTF8.GetBytes(key));
            //}

            var fullKey = key;

            if (!string.IsNullOrWhiteSpace(_options.CacheRegion))
            {
                //if (_options.KeyspaceNotificationsEnabled && region.Contains(":"))
                //{
                //    region = Base64Prefix + Convert.ToBase64String(Encoding.UTF8.GetBytes(region));
                //}

                fullKey = string.Concat(_options.CacheRegion, ":", key);
            }

            return fullKey;
        }

        private void CheckRedis()
        {
            if (_connectMulti == null || !_connectMulti.IsConnected)
            {
                string _connstr = $"{this._options.Host}:{this._options.Port}";
                if (!string.IsNullOrEmpty(this._options.PassWord))
                    _connstr = $"{_connstr},password={this._options.PassWord}";

                //ConnectionMultiplexer.SetFeatureFlag("preventthreadtheft", true); //add pengxy  参考 https://stackexchange.github.io/StackExchange.Redis/ThreadTheft 
                _connectMulti = ConnectionMultiplexer.Connect(_connstr);
                _cache = _connectMulti.GetDatabase(this._options.Database);

            }
        }

        public override void Dispose()
        {
            if (_connectMulti != null)
            {
                _connectMulti.Close();
                _connectMulti.Dispose();

            }
        }
        public override void Clear()
        {
            if (_connectMulti != null)
            {
                var hashKeys = _cache.HashKeys(_options.CacheRegion);
                if (hashKeys.Length > 0)
                {
                    foreach (var key in hashKeys.Where(p => p.HasValue))
                    {
                        _cache.KeyDelete(key.ToString(), CommandFlags.FireAndForget);
                    }
                }
                _cache.KeyDelete(_options.CacheRegion);
            }
        }


        public override bool Exists(string key)
        {
            CheckRedis();
            key = GetRegionKey(key);
            if (this._options.EnableMultiCache)
            {
                return _MemoryCache.Exists(key) ? true : _cache.KeyExists(key);
            }
            else
            {
                return _cache.KeyExists(key);
            }
        }
        public override string GetCache(string key)
        {
            CheckRedis();
            key = GetRegionKey(key);
            string obj = string.Empty;
            if (this._options.EnableMultiCache)
            {
                obj = _MemoryCache.GetCache(key);
                if (obj.IsNullOrEmpty())
                {
                    var val = _cache.StringGet(key);
                    if (val.HasValue)
                    {
                        obj = val.ToString();
                        _MemoryCache.SetCache(key, obj);
                    }

                    else
                        obj = string.Empty;
                }
            }
            else
            {
                var val = _cache.StringGet(key);
                if (val.HasValue)
                    obj = val.ToString();
                else
                    obj = string.Empty;
            }

            return obj;
        }

        public override T GetCache<T>(string key) where T : class
        {
            CheckRedis();
            key = GetRegionKey(key);

            var value = default(T);
            Type type = typeof(T);
            if (this._options.EnableMultiCache)
            {
                var obj = _MemoryCache.GetCache(key);
                if (obj.IsNullOrEmpty())
                {
                    var cachevalue = _cache.StringGet(key);
                    if (!cachevalue.IsNull)
                    {
                        value = JsonConvert.DeserializeObject<T>(cachevalue);
                        _MemoryCache.SetCache(key, value);
                    }
                }
                else
                {
                    value = JsonConvert.DeserializeObject<T>(obj);
                }
            }
            else
            {
                var cachevalue = _cache.StringGet(key);
                if (!cachevalue.IsNull)
                {
                    value = JsonConvert.DeserializeObject<T>(cachevalue);
                }
            }

            return value;
        }

        public override T GetOrCreate<T>(string key, Func<T> value) where T : class
        {
            DateTimeOffset currdate = DateTimeOffset.Now;
            int _seconds = int.Parse(DateTimeOffset.MaxValue.Subtract(currdate).TotalSeconds.ToString());
            return GetOrCreate<T>(key, value, _seconds);
        }

        private string keySplitKey = "~@~@";

        public override int Count
        {
            get
            {
                var count = _connectMulti.GetServer(_connectMulti.GetEndPoints(true)[0]).DatabaseSize(_cache.Database);
                return (int)count;
            }
        }

        private string GetRegionKeyForNotityKey(string key)
        {
            return key += keySplitKey + this.UniqueId;
        }

        public override T GetOrCreate<T>(string key, Func<T> value, int second) where T : class
        {
            CheckRedis();
            key = GetRegionKey(key);
            if (!Exists(key))
            {
                T _val = value.Invoke();
                string _value = JsonConvert.SerializeObject(_val);
                _cache.StringSet(key, _value, TimeSpan.FromSeconds(second), When.NotExists, CommandFlags.None);
                if (this._options.EnableMultiCache)
                {
                    //本地存储，并发布消息
                    _MemoryCache.SetCache(key, _value, DateTime.Now.Add(TimeSpan.FromSeconds(second)));
                    this.PublishMessage(_cache_notity_channel_remove, GetRegionKeyForNotityKey(key));

                }
                return _val;
            }
            else
            {
                return GetCache<T>(key);
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
        public override T GetOrCreate<T>(string key, Func<T> value, DateTimeOffset time) where T : class
        {
            DateTimeOffset currdate = DateTimeOffset.Now;
            int _seconds = int.Parse(time.Subtract(currdate).TotalSeconds.ToString());
            return GetOrCreate<T>(key, value, _seconds);

        }

        public override void RemoveCache(string key)
        {
            CheckRedis();
            var fullKey = GetRegionKey(key);

            if (!string.IsNullOrWhiteSpace(_options.CacheRegion))
            {
                _cache.HashDelete(_options.CacheRegion, fullKey, CommandFlags.FireAndForget);
            }
            _cache.KeyDelete(fullKey);
            if (this._options.EnableMultiCache)
            {
                this.PublishMessage(_cache_notity_channel_remove, GetRegionKeyForNotityKey(fullKey));
            }
        }

        public override void SetCache(string key, object value)
        {
            SetCache(key, value, -1);
        }

        public override void SetCache(string key, object value, DateTimeOffset expiressAbsoulte)
        {
            DateTimeOffset currdate = DateTimeOffset.Now;
            int _seconds = int.Parse(expiressAbsoulte.Subtract(currdate).TotalSeconds.ToString());
            if (_seconds > 0)
            {
                SetCache(key, value, _seconds);
            }
        }

        public override void SetCache(string key, object value, int second)
        {
            CheckRedis();
            var fullKey = GetRegionKey(key);
            
            string _value = JsonConvert.SerializeObject(value);
            _cache.StringSet(fullKey, _value, second > 0 ?TimeSpan.FromSeconds(second):null);
            if (!string.IsNullOrWhiteSpace(_options.CacheRegion))
            {
                _cache.HashSet(_options.CacheRegion, fullKey, "regionKey", When.Always, CommandFlags.FireAndForget);
            }
            if (this._options.EnableMultiCache)
            { //本地存储，并发布消息
                if (second > 0)
                    _MemoryCache.SetCache(fullKey, _value, DateTime.Now.Add(TimeSpan.FromSeconds(second)));
                else
                    _MemoryCache.SetCache(fullKey, _value);
                this.PublishMessage(_cache_notity_channel_remove, GetRegionKeyForNotityKey(fullKey));
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
            CheckRedis();
            _connectMulti.GetSubscriber().Subscribe(channel, (rchannel, message) =>
            {
                var tupple = ParseRegionKey(message);
                handler(rchannel, message.ToString());
            });

            //ISubscriber subscriber = _connectMulti.GetSubscriber();
            //subscriber.Subscribe("__keyevent@6__:expired", (channel, notificationType) =>
            //{
            //    Console.WriteLine(channel + "|" + notificationType);
            //});
        }

        /// <summary>
        /// 队列方式订阅 按发布顺序接收
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="handler"></param>
        public void QueueSubScriber(string channel, Action<string, string> handler = null)
        {
            CheckRedis();
            _connectMulti.GetSubscriber().Subscribe(channel).OnMessage(message =>
            {
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
            CheckRedis();
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
            CheckRedis();
            channel = GetRegionKey(channel);
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
            CheckRedis();
            channel = GetRegionKey(channel);
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
            CheckRedis();
            channel = GetRegionKey(channel);
            return _cache.ListLeftPop(channel);
        }
        /// <summary>
        /// 获取队列的记录数
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public long ListCount(string channel)
        {
            CheckRedis();
            channel = GetRegionKey(channel);
            return _cache.ListLength(channel);
        }



        /// <summary>
        /// 解除锁定
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>

        public override bool UnLock(LckInfo lckinfo, params string[] keys)
        {
            if (keys.Length == 0) return true;
            CheckRedis();

            Stopwatch stopwatch = Stopwatch.StartNew();
          
          
            

            foreach (string key in keys)
            {
                var newkey = key;
                if (!newkey.Contains(_lockkeyPrefix))
                    newkey = _lockkeyPrefix + newkey;
                newkey = GetRegionKey(newkey);
                bool isrelease = false;
                int idx = 0;
                while (!isrelease && stopwatch.Elapsed <= TimeSpan.FromSeconds(1) && idx++ < 10)
                {
                    try
                    {
                        isrelease = _cache.LockRelease(newkey, lckinfo.UName);
                        HDel(_lockhashname, newkey);
                    }
                    catch (Exception)
                    {
                        
                    }
                    Console.WriteLine($"释放锁：{newkey}  结果：{isrelease}");
                   
                }
            }
            return true;
        }
        public override bool HSet(string hashkey, string key, string value)
        {
            CheckRedis();

            if (!hashkey.Contains(_hsetkeyPrefix))
            {
                hashkey = _hsetkeyPrefix + hashkey;
            }
            hashkey = GetRegionKey(hashkey);

            return _cache.HashSet(hashkey, key, value, When.Always, CommandFlags.None);


        }

        public override bool HDel(string hashkey, string key)
        {
            CheckRedis();
            if (!hashkey.Contains(_hsetkeyPrefix))
            {
                hashkey = _hsetkeyPrefix + hashkey;
            }
            hashkey = GetRegionKey(hashkey);

            return _cache.HashDelete(hashkey, key, CommandFlags.FireAndForget);
        }

        public override string HGet(string hashkey, string key)
        {
            CheckRedis();
            if (!hashkey.Contains(_hsetkeyPrefix))
            {
                hashkey = _hsetkeyPrefix + hashkey;
            }
            hashkey = GetRegionKey(hashkey);
            return _cache.HashGet(hashkey, key);
        }
        /// <summary>
        /// 获取当前锁信息
        /// </summary>
        /// <returns></returns>
        public override List<LckInfo> GetCurrLockInfo()
        {
            List<LckInfo> lckInfos = new List<LckInfo>();
            var hashkey = GetRegionKey(_lockhashname);
            var valu = _cache.HashGetAll(hashkey);
            if (valu.Length > 0)
            {
                List<string> lckInfosWaitRemove = new List<string>();
                foreach (var item in valu)
                {
                    var isexists = _cache.KeyExists(item.Name.ToString());
                    var isexists2 = _cache.StringGet(item.Name.ToString());
                    if (isexists)
                    {
                        LckInfo lckInfo = JsonConvert.DeserializeObject<LckInfo>(item.Value);
                        lckInfo.Key = item.Name;
                        lckInfos.Add(lckInfo);
                    }
                    else
                    {
                        lckInfosWaitRemove.Add(item.Name);
                    }
                }
                foreach (var key in lckInfosWaitRemove)
                {
                    HDel(_lockhashname, key);
                }
            }
            return lckInfos;
        }

        /// <summary>
        /// 获取当前缓存历史表锁信息
        /// </summary>
        /// <returns></returns>
        public override List<LckInfo> GetHisLockInfo()
        {
            List<LckInfo> lckInfos = new List<LckInfo>();
            var key = GetRegionKey(_lockhishashname);
            var valu = _cache.HashGetAll(key);
            if (valu.Length > 0)
            {
                foreach (var item in valu)
                {
                    LckInfo lckInfo = JsonConvert.DeserializeObject<LckInfo>(item.Value);
                    lckInfo.Key = item.Name;
                    lckInfos.Add(lckInfo);
                }
            }
            return lckInfos;
        }

        public delegate bool deleLockOn(string key, int expresseconds);


        /// <summary>
        /// 加业务锁 
        /// 加锁持续时间最长不超过60秒 默认30
        /// 等待加锁有效期最长不超过10秒 （默认5秒）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expresseconds"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override Tuple<bool, string> LockOn(string key, LckInfo lckinfo, int expresseconds = 30, int timeoutseconds = 5)
        {
            CheckRedis();
            if (!key.Contains(_lockkeyPrefix))
                key = _lockkeyPrefix + key;

            key = GetRegionKey(key);

            int _max_second = int.MaxValue;//最长定锁有效期  原来是 60
            int _max_timeout = int.MaxValue;//最长加锁等待时间 原来是 10


            expresseconds = expresseconds < 0 ? 5 : expresseconds;
            expresseconds = expresseconds > _max_second ? _max_second : expresseconds;

            timeoutseconds = timeoutseconds < 0 ? 5 : timeoutseconds;
            timeoutseconds = timeoutseconds > _max_timeout ? _max_timeout : timeoutseconds;

            bool islocked = false;
            Stopwatch stopwatch = Stopwatch.StartNew(); 
            while (!islocked && stopwatch.Elapsed <= TimeSpan.FromSeconds(timeoutseconds))
            {
                islocked = _cache.LockTake(key, lckinfo.UName, TimeSpan.FromSeconds(expresseconds));
                Thread.Sleep(5);
            }
            string msg = "";
            Thread thread = null;
            //deleLockOn _deleLock = new deleLockOn(lockOn);
            //var workTask = Task.Run(() =>
            //{
            //    try
            //    {
            //        thread = System.Threading.Thread.CurrentThread;
            //        bool _success = _deleLock.Invoke(key, expresseconds);
            //        if (_success)
            //        {
            //            lckinfo = getLockInfo(lckinfo, expresseconds, 0);
            //            SaveLockInfo(key, lckinfo);
            //        }
            //        return _success;
            //    }
            //    catch (Exception ex)
            //    {
            //        return false;
            //    }
            //});
            //bool flag = workTask.Wait(timeoutseconds * 1000, new CancellationToken(false));


            //
            //if (flag)
            //{
            //    if (workTask.Result)
            //    {
            //        msg = "加锁成功";
            //    }
            //    else
            //    {

            //        flag = false;
            //        msg = "加锁失败";
            //    }

            //}
            if (islocked)
            {
                lckinfo = getLockInfo(lckinfo, expresseconds, 0);
                SaveLockInfo(key, lckinfo);
                msg = "加锁成功";
            }
            else
            {
                if (thread != null)
                    thread.Interrupt();
                msg = $"key:[{key}]锁定失败,加锁等待超过{timeoutseconds}秒!";
            }
            return new Tuple<bool, string>(islocked, msg);
        }
        private static List<string> hashtable = new List<string>();
        private static List<string> lockhashtable = new List<string>();

        bool CkeckExists(params string[] keys)
        {
            bool flag = false;
            lock (hashtable)
            {
                flag = keys.Intersect(hashtable).Count() > 0;
            }
            
            return flag;
        }
        public override Tuple<bool, string> LockOn(string[] keys, LckInfo lckinfo, int expresseconds = 30, int timeoutseconds = 5)
        {

            if (keys.Length == 1)
            {
                return LockOn(keys[0], lckinfo,  expresseconds , timeoutseconds);
            }
            lock (lockhashtable)
            {
                Tuple<bool, string> tuple = new Tuple<bool, string>(false, "获取锁超时");
                Stopwatch stopwatch = Stopwatch.StartNew();
                List<string> lockedKey = new List<string>();
                int idx = 0;
                while (keys.Length > 1 && CkeckExists(keys) && stopwatch.Elapsed <= TimeSpan.FromSeconds(timeoutseconds*3))
                {
                    Thread.Sleep(100);
                    if (stopwatch.Elapsed > TimeSpan.FromSeconds(timeoutseconds * 3))
                    {
                        return new Tuple<bool, string>(false, "等待超时。");
                    }
                }
                while (!tuple.Item1 && stopwatch.Elapsed <= TimeSpan.FromSeconds(timeoutseconds) && idx < keys.Length)
                {
                    if (keys.Length > 1 && idx == 0)
                    {
                        lock (hashtable)
                        {
                            if (CkeckExists(keys))
                            {
                                Thread.Sleep(100);
                                continue;
                            }
                            hashtable.AddRange(keys);
                        }
                    }

                    var lockResult = LockOn(keys[idx], lckinfo, expresseconds, timeoutseconds);
                    if (!lockResult.Item1)
                    {
                        tuple = lockResult;
                        break;
                    }
                    
                    lockedKey.Add(keys[idx]);
                    idx++;
                }
                if (idx == keys.Length && keys.Length > 1)
                {
                    lock (hashtable)
                    {
                        for (int i = 0; i < keys.Length; i++)
                        {
                            if (hashtable.Contains(keys[i]))
                            {
                                hashtable.Remove(keys[i]);
                            }
                        }
                    }
                    return new Tuple<bool, string>(true, $"key:[{string.Join(",", keys)}]加锁成功");
                }
                if (keys.Length > 1)
                {
                    lock (hashtable)
                    {
                        for (int i = 0; i < keys.Length; i++)
                        {
                            hashtable.Remove(keys[i]);
                        }
                    }
                }
                
                //加锁失败或超时
                UnLock(lckinfo, lockedKey.ToArray());
                return new Tuple<bool, string>(tuple.Item1, tuple.Item2);
            }
        }

        /// <summary>
        /// 检测Key是否被锁定
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private Tuple<bool, string> CheckLock(string key)
        {
            CheckRedis();
            string _key = key;
            if (!key.Contains(_lockkeyPrefix))
                key = _lockkeyPrefix + key;

            key = GetRegionKey(key);
            bool _islock = false;
            string _msg = "";
            if (_cache.KeyExists(key))
            {

                _islock = true;

                string _lockinfo = HGet(_lockhashname, key);
                if (!string.IsNullOrEmpty(_lockinfo))
                {
                    LckInfo lckInfo = JsonConvert.DeserializeObject<LckInfo>(_lockinfo);
                    _msg = $"key:[{_key}]已经被[{lckInfo.UName}]在[{lckInfo.EventName}]于[{lckInfo.LockTime.ToString("yyyy-MM-dd HH:mm:ss")}]锁定!";

                }
                else
                    _msg = $"key:[{_key}]被锁定";
            }
            else
            {
                _msg = $"key:[{_key}]未被锁定";
            }
            return new Tuple<bool, string>(_islock, _msg);

        }

        public override Tuple<bool, string> CheckLock(params string[] keys)
        {
            foreach (var key in keys)
            {
                var res = CheckLock(key);
                if (res.Item1)
                {
                    return res;
                }
            }
            return new Tuple<bool, string>(false, $"key:[{string.Join(",", keys)}]未被锁定");
        }
        /// <summary>
        /// 锁定并执行业务
        /// </summary>
        /// <param name="key">需要锁定的key</param>
        /// <param name="action">锁定后执行的方法</param>
        /// <param name="lckinfo">锁定信息</param>
        /// <param name="expresseconds">锁定持续时间</param>
        /// <param name="timeoutseconds">等待加锁时间</param>
        /// <returns></returns>
        public override Tuple<bool, string> LockOnExecute(string key, Action action, LckInfo lckinfo, int expresseconds = 30, int timeoutseconds = 5)
        {

            CheckRedis();
            if (!key.Contains(_lockkeyPrefix))
                key = _lockkeyPrefix + key;

            key = GetRegionKey(key);

            int _max_second = int.MaxValue;//最长定锁有效期
            int _max_timeout = int.MaxValue;//最长加锁等待时间
            int _times = 5;//续锁最多次数
            int _millsecond = 900;

            expresseconds = expresseconds < 0 ? 5 : expresseconds;
            expresseconds = expresseconds > _max_second ? _max_second : expresseconds;

            if (expresseconds <= timeoutseconds)
            {
                expresseconds = timeoutseconds + 2;
            }
            timeoutseconds = timeoutseconds < 0 ? 5 : timeoutseconds;
            timeoutseconds = timeoutseconds > _max_timeout ? _max_timeout : timeoutseconds;

            bool islocked = false;
            Stopwatch stopwatch = Stopwatch.StartNew();
            while (!islocked && stopwatch.Elapsed <= TimeSpan.FromSeconds(timeoutseconds))
            {
                islocked = _cache.LockTake(key, lckinfo.UName, TimeSpan.FromSeconds(expresseconds));
                if (!islocked) { Thread.Sleep(5); }
            }
            string msg = "";
            Thread thread = null;


            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = tokenSource.Token;

            //deleLockOn _deleLock = new deleLockOn(lockOn);
            //Thread threadGetlock = null;
            //Thread thread = null;
            //var workTaskGetLock = Task.Run(() =>
            //{
            //    try
            //    {
            //        threadGetlock = System.Threading.Thread.CurrentThread;
            //        bool _success = _deleLock.Invoke(key, expresseconds);
            //        if (_success)
            //        {
            //            lckinfo = getLockInfo(lckinfo, expresseconds, 0);

            //            SaveLockInfo(key, lckinfo);
            //        }
            //        return _success;
            //    }
            //    catch (Exception ex)
            //    {
            //        return false;
            //    }
            //});
            //bool islocked = workTaskGetLock.Wait(timeoutseconds * 1000, new CancellationToken(false));
            //string msg = "";


            bool flag = false;
            if (islocked) //获取锁成功
            {
                lckinfo = getLockInfo(lckinfo, expresseconds, 0);
                SaveLockInfo(key, lckinfo);
                int _timesa = 0;
                var workTask = Task.Run(() =>
                {
                    thread = System.Threading.Thread.CurrentThread;
                    try
                    {
                        action.Invoke();
                    }
                    catch (Exception ex)
                    {
                        //Console.WriteLine($"线程中断。。");
                    }
                    finally
                    {
                        UnLock(lckinfo, key);
                    }
                });
                flag = workTask.Wait(expresseconds * _millsecond, cancellationToken);

                if (flag)
                {
                    msg = $"key:[{key}]锁定并操作业务成功!锁已自动释放";
                }

                while (!flag)
                {
                    if (_timesa >= _times)
                    {
                        if (!workTask.IsCompleted && !workTask.IsCanceled)
                        {
                            UnLock(lckinfo, key);
                            tokenSource.Cancel();
                            thread.Interrupt();
                        }
                        flag = false;
                        msg = $"key:[{key}]锁定操作业务失败!超过最大[{_times}]次续锁没有完成,操作被撤销";
                        break;
                    }
                    else
                    {
                        //续锁
                        _cache.StringSet(key, 1, TimeSpan.FromSeconds(expresseconds));
                        lckinfo = getLockInfo(lckinfo, expresseconds, _timesa);
                        SaveLockInfo(key, lckinfo);

                        flag = workTask.Wait(expresseconds * _millsecond, cancellationToken);
                        if (flag)
                        {
                            flag = true;
                            msg = $"key:[{key}]锁定并操作业务成功!续锁{_timesa + 1}次,锁已经自动释放";
                            break;
                        }
                    }
                    _timesa++;
                }
            }
            else  //获取锁失败
            {
                msg = $"key:[{key}]锁定失败,加锁等待超过{timeoutseconds}秒!";
            }

            return new Tuple<bool, string>(flag, msg);
        }



        public override Tuple<bool, string> LockOnExecute(string[] keys, Action action, LckInfo lckinfo, int expresseconds = 30, int timeoutseconds = 5)
        {
            if (keys.Length == 1)
            {
                return LockOnExecute(keys[0], action, lckinfo, expresseconds, timeoutseconds);
            }
            CheckRedis();
            int _max_second = int.MaxValue;//最长定锁有效期
            int _max_timeout = int.MaxValue;//最长加锁等待时间
            int _times = 5;//续锁最多次数
            int _millsecond = 900;

            expresseconds = expresseconds < 0 ? 5 : expresseconds;
            expresseconds = expresseconds > _max_second ? _max_second : expresseconds;

            if (expresseconds <= timeoutseconds)
            {
                expresseconds = timeoutseconds + 2;
            }
            timeoutseconds = timeoutseconds < 0 ? 5 : timeoutseconds;
            timeoutseconds = timeoutseconds > _max_timeout ? _max_timeout : timeoutseconds;

            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = tokenSource.Token;           
            Thread thread = null;
            string msg = "";
            bool flag = false;
            var getlockResult = LockOn(keys, lckinfo, expresseconds, timeoutseconds);
            if (!getlockResult.Item1)
            {
                flag = false;
                msg = getlockResult.Item2;
            }
            else
            {//获取锁成功

                int _timesa = 0;
                var workTask = Task.Run(() =>
                {
                    thread = System.Threading.Thread.CurrentThread;
                    try
                    {
                        action.Invoke();
                    }
                    catch (Exception ex)
                    {
                        //Console.WriteLine($"线程中断。。");
                    }
                    finally
                    {
                        UnLock(lckinfo, keys);
                    }
                });
                flag = workTask.Wait(expresseconds * _millsecond, cancellationToken);

                if (flag)
                {

                    msg = $"key:[{keys}]锁定并操作业务成功!锁已自动释放";
                }

                while (!flag)
                {
                    if (_timesa >= _times)
                    {
                        if (!workTask.IsCompleted && !workTask.IsCanceled)
                        {
                            UnLock(lckinfo, keys);
                            tokenSource.Cancel();
                            thread.Interrupt();
                        }
                        flag = false;
                        msg = $"key:[{keys}]锁定操作业务失败!超过最大[{_times}]次续锁没有完成,操作被撤销";
                        break;
                    }
                    else
                    {
                        //续锁
                        foreach (var key in keys)
                        {
                            var newKey = key;
                            if (!newKey.Contains(_lockkeyPrefix))
                                newKey = _lockkeyPrefix + newKey;

                            newKey = GetRegionKey(newKey);

                            _cache.StringSet(newKey, 1, TimeSpan.FromSeconds(expresseconds));
                            lckinfo = getLockInfo(lckinfo, expresseconds, _timesa);
                            SaveLockInfo(newKey, lckinfo);

                        }
                        flag = workTask.Wait(expresseconds * _millsecond, cancellationToken);
                        if (flag)
                        {
                            flag = true;
                            msg = $"key:[{keys}]锁定并操作业务成功!续锁{_timesa + 1}次,锁已经自动释放";
                            break;
                        }
                    }
                    _timesa++;
                }
            }

            return new Tuple<bool, string>(flag, msg);
        }

        #region 私有变量
        LckInfo getLockInfo(LckInfo lckInfo, int expresseconds, int times)
        {

            if (lckInfo.LockTime == null || lckInfo.LockTime == DateTime.MinValue)
                lckInfo.LockTime = DateTime.Now;


            if (lckInfo.ExpireTime == null || lckInfo.ExpireTime == DateTime.MinValue)
            {
                lckInfo.ExpireTime = DateTime.Now.AddSeconds(expresseconds);
            }
            else
                lckInfo.ExpireTime = lckInfo.ExpireTime.AddSeconds(expresseconds);

            lckInfo.KeepTimes = times;


            return lckInfo;
        }

        //bool lockOn(string key, int expresseconds = 5)
        //{
        //    expresseconds = expresseconds < 0 ? 5 : expresseconds;
        //    expresseconds = expresseconds > 60 ? 60 : expresseconds;
        //    //deleLockOn _deleLock=new deleLockOn ()
        //    bool isok = false;
        //    while (!isok)
        //    {
        //        isok = _cache.LockTake(key, 1, TimeSpan.FromSeconds(expresseconds));
        //        //return isok;
        //        if (isok)
        //        {
        //            break;
        //        }
        //        Thread.Sleep(10);
        //    }
        //    return true;
        //}
        //bool lockOnNoWait(string key, int expresseconds = 5)
        //{
        //    bool isok = false;           
        //    isok = _cache.LockTake(key, 1, TimeSpan.FromSeconds(expresseconds));
        //    return isok;
        //}

        #endregion

    }
}
