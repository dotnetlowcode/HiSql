using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HiSql
{

    /// <summary>
    /// 本地Memory缓存
    /// </summary>
    public class MCache : BaseCache
    {
        private readonly object lockList = new object();
        private readonly Hashtable globalHSet = new Hashtable();
        private readonly object lockHstObj = new object();
        internal MemoryCacheOptions MemoryCacheOptions { get; }
        public override int Count => _cache.Count;

        private void SetCacheRegion(string resion)
        {
            base.Init(resion);
        }

        /// <summary>
        ///  初始化构造函数
        /// </summary>
        /// <param name="cacheRegion">缓存区域名称。建议以系统名称命名。如 CRM</param>
        /// <param name="memoryCacheOptions">MemoryCache缓存参数配置</param>
        public MCache(string cacheRegion, MemoryCacheOptions memoryCacheOptions = null)//
        {
            if (cacheRegion.IsNullOrEmpty())
            {
                cacheRegion = HiSql.Constants.PlatformName;
            }
            MemoryCacheOptions = memoryCacheOptions ?? new MemoryCacheOptions();
            this._cache = new MemoryCache(MemoryCacheOptions);
            SetCacheRegion(cacheRegion);
        }
        private MemoryCache _cache;
        private string GetRegionKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (key.StartsWith(CacheRegion + ":"))
                return key;

            //if (!key.StartsWith(HiSql.Constants.NameSpace))
            //    key = HiSql.Constants.NameSpace + ":" + key;

            var fullKey = key;

            if (!string.IsNullOrWhiteSpace(CacheRegion))
            {
                fullKey = string.Concat(CacheRegion, ":", key);
            }

            return fullKey;
        }

        public override bool Exists(string key)
        {
            key = GetRegionKey(key);

            object v = null;
            return this._cache.TryGetValue<object>(key, out v);
        }


        public override string GetCache(string key)
        {
            key = GetRegionKey(key);
            string value = string.Empty;
            this._cache.TryGetValue(key, out value);
            return value;
        }

        public override T GetCache<T>(string key) where T : class
        {
            key = GetRegionKey(key);

            T v = null;
            this._cache.TryGetValue<T>(key, out v);


            return v;
        }

        /// <summary>
        /// 设置缓存对象
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public override void SetCache(string key, object value)
        {
            SetCache(key, value, DateTimeOffset.MaxValue);
        }

        /// <summary>
        /// 设置缓存对象（带有效期）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expirationTime"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public override void SetCache(string key, object value, DateTimeOffset expirationTime)
        {
            
            key = GetRegionKey(key);

            if (value == null)
            {
                throw new Exception("不能将Null存入缓存");
            }

            object v = null;
            if (this._cache.TryGetValue(key, out v))
                this._cache.Remove(key);
            this._cache.Set<object>(key, value, expirationTime);
        }

        /// <summary>
        /// 设置缓存对象
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="second"></param>
        public override void SetCache(string key, object value, int second)
        {
            DateTimeOffset expirationTime = DateTimeOffset.Now;
            expirationTime = expirationTime.AddSeconds(second);
            SetCache(key, value, expirationTime);
        }


        /// <summary>
        /// 移除缓存缓存对象
        /// </summary>
        /// <param name="key"></param>
        public override void RemoveCache(string key)
        {
            key = GetRegionKey(key);
            this._cache.Remove(key);
        }

        public override void Dispose()
        {
            if (_cache != null)
            {
                _cache.Dispose();
                GC.SuppressFinalize(this);
            }

        }
        public override void Clear()
        {
            var old = this._cache;
            _cache = new MemoryCache(MemoryCacheOptions);
            old.Dispose();

        }
        /// <summary>
        /// 获取或设置缓存对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override T GetOrCreate<T>(string key, Func<T> value) where T : class
        {
            return GetOrCreate(key, value, DateTimeOffset.MaxValue);
        }

        /// <summary>
        /// 获取或创建缓存 并设置有效期
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public override T GetOrCreate<T>(string key, Func<T> value, int second) where T : class
        {
            DateTimeOffset expirationTime = DateTimeOffset.Now;
            expirationTime = expirationTime.AddSeconds(second);
            return GetOrCreate(key, value, expirationTime);
        }

        public override T GetOrCreate<T>(string key, Func<T> value, DateTimeOffset time) where T : class
        {
            key = GetRegionKey(key);
            return this._cache.GetOrCreate<T>(key, (entry) =>
            {
                var objs = value.Invoke();
                if (objs == null)
                {
                    throw new Exception("不能将Null存入缓存");
                }

                entry.SetValue(objs);
                entry.AbsoluteExpiration = time;
                return objs;
            });
        }

        /// <summary>
        /// 设置hash值
        /// </summary>
        /// <param name="hashkey"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool HSet(string hashkey, string key, string value)
        {
            if (!hashkey.Contains(_hsetkeyPrefix))
            {
                hashkey = _hsetkeyPrefix + hashkey;
            }
            hashkey = GetRegionKey(hashkey);

            if (!globalHSet.ContainsKey(hashkey))
            {
                lock (lockHstObj)
                {
                    if (!globalHSet.ContainsKey(hashkey))
                    {
                        globalHSet.Add(hashkey, new Hashtable { { key, value } });
                        return true;
                    }
                }
            }
            Hashtable keyValuePairs = (Hashtable)globalHSet[hashkey];
            if (keyValuePairs.ContainsKey(key))
            {
                keyValuePairs[key] = value;
            }
            else
            {
                keyValuePairs.Add(key, value);
            }
            globalHSet[hashkey] = keyValuePairs;
            return true;
        }

        /// <summary>
        /// 获取hash对象
        /// </summary>
        /// <param name="hashkey"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public override string HGet(string hashkey, string key)
        {
            if (!hashkey.Contains(_hsetkeyPrefix))
            {
                hashkey = _hsetkeyPrefix + hashkey;
            }
            hashkey = GetRegionKey(hashkey);
            if (globalHSet.ContainsKey(hashkey))
            {
                Hashtable keyValuePairs = (Hashtable)globalHSet[hashkey];
                return keyValuePairs.ContainsKey(key) ? keyValuePairs[key]?.ToString() : null;
            }
            return null;
        }

        /// <summary>
        /// 删除hash
        /// </summary>
        /// <param name="hashkey"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public override bool HDel(string hashkey, string key)
        {
            if (!hashkey.Contains(_hsetkeyPrefix))
            {
                hashkey = _hsetkeyPrefix + hashkey;
            }
            hashkey = GetRegionKey(hashkey);

            if (globalHSet.ContainsKey(hashkey))
            {
                Hashtable keyValuePairs = (Hashtable)globalHSet[hashkey];
                if (keyValuePairs.ContainsKey(key))
                    keyValuePairs.Remove(key);
            }
            return false;
        }

        /// <summary>
        /// 业务锁
        /// </summary>
        /// <param name="key"></param>
        /// <param name="lckinfo"></param>
        /// <param name="expirySeconds"></param>
        /// <param name="timeoutSeconds"></param>
        /// <returns></returns>
        public override Tuple<bool, string> LockOn(string key, LckInfo lckinfo, int expirySeconds = 30, int timeoutSeconds = 5)
        {
            var isBlockingMode = timeoutSeconds > 0; //是否阻塞等待模式
            if (!key.Contains(_lockkeyPrefix))
                key = _lockkeyPrefix + key;

            key = GetRegionKey(key);

            int _max_second = int.MaxValue;//最长定锁有效期
            int _max_timeout = int.MaxValue;//最长加锁等待时间

            expirySeconds = expirySeconds < 0 ? 5 : expirySeconds;
            expirySeconds = expirySeconds > _max_second ? _max_second : expirySeconds;

            timeoutSeconds = timeoutSeconds < 0 ? 5 : timeoutSeconds;
            timeoutSeconds = timeoutSeconds > _max_timeout ? _max_timeout : timeoutSeconds;

            string msg = "";
            var flag = false;
            //创建key
            if (GetCache<LckInfo>(key) == null)
            {
                lock (lockList)
                {
                    if (GetCache<LckInfo>(key) == null)
                    {
                        SetCache(key, lckinfo, expirySeconds);
                    }
                }
            }
            var isgetlocked = false;
            var getlockElapsed = TimeSpan.FromSeconds(timeoutSeconds);
            if (!isBlockingMode)
            {
                getlockElapsed = TimeSpan.FromSeconds(Global.LockOptions.NoWaitModeGetLockWaitMillSeconds);
            }
            isgetlocked = System.Threading.Monitor.TryEnter(GetCache<LckInfo>(key), getlockElapsed);

            if (!isgetlocked)
            {
                flag = false;
                msg = $"key:[{key}]锁定失败,加锁等待超过{timeoutSeconds}秒!";
            }
            else
            {
                SaveLockInfo(key, lckinfo);

                msg = "加锁成功";
                flag = true;
            }
            return new Tuple<bool, string>(flag, msg);
        }

        //private static Hashtable hashtable = new Hashtable();
        //private static List<string> hashtable = new List<string>();

        bool CkeckExists(params string[] keys)
        {
            bool flag = false;
            lock (hashtable)
            {
                flag = keys.Intersect(hashtable).Count() > 0;
            }
            return flag;
        }
        private static List<string> hashtable = new List<string>();
        private static List<string> lockhashtable = new List<string>();
        public override Tuple<bool, string> LockOn(string[] keys, LckInfo lckinfo, int expirySeconds = 30, int timeoutSeconds = 5)
        {
            var isBlockingMode = timeoutSeconds > 0; //是否阻塞等待模式
            Tuple<bool, string> tuple = new Tuple<bool, string>(false, "");
            Stopwatch stopwatch = Stopwatch.StartNew();

            if (keys.Length == 1)
            {
                return LockOn(keys[0], lckinfo, expirySeconds, timeoutSeconds);
            }

            List<string> lockedKey = new List<string>();
            int idx = 0;
            lock (lockhashtable)
            {
                var getlockElapsed = TimeSpan.FromSeconds(timeoutSeconds);
                if (!isBlockingMode)
                {
                    getlockElapsed = TimeSpan.FromSeconds(Global.LockOptions.NoWaitModeGetLockWaitMillSeconds);
                }

                while (keys.Length > 1 && CkeckExists(keys) && stopwatch.Elapsed <= getlockElapsed)
                {
                    Thread.Sleep(Global.LockOptions.GetLockRetrySleepMillSeconds);
                    if (stopwatch.Elapsed > getlockElapsed)
                    {
                        return new Tuple<bool, string>(false, "准备加锁操作等待超时。");
                    }
                }

                while (!tuple.Item1 && stopwatch.Elapsed <= getlockElapsed && idx < keys.Length)
                {
                    if (keys.Length > 1 && idx == 0)
                    {
                        lock (hashtable)
                        {
                            hashtable.AddRange(keys);
                        }
                    }

                    var lockResult = LockOn(keys[idx], lckinfo, expirySeconds, timeoutSeconds);
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
                foreach (var key in lockedKey)
                {
                    UnLock(lckinfo, key);
                }
                return new Tuple<bool, string>(tuple.Item1, tuple.Item2);

            }
        }

        private Tuple<bool, string> CheckLock(string key)
        {
            string _key = key;
            if (!key.Contains(_lockkeyPrefix))
                key = _lockkeyPrefix + key;

            key = GetRegionKey(key);

            bool _islock = false;
            string _msg = "";
            if (GetCache<LckInfo>(key) != null)
            {
                //MCaceh 检查锁只有是 缓存对象在，且 HashTable有值才是锁定状态  pengxy on 2022 10 14 
                string _lockinfo = HGet(_lockhashname, key);
                if (!string.IsNullOrEmpty(_lockinfo))
                {
                    _islock = true;
                    LckInfo lckInfo = JsonConvert.DeserializeObject<LckInfo>(_lockinfo);
                    _msg = $"key:[{_key}]已经被[{lckInfo.UName}]在[{lckInfo.EventName}]于[{lckInfo.LockTime.ToString("yyyy-MM-dd HH:mm:ss")}]锁定!";
                }
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
        public override Tuple<bool, string> LockOnExecuteNoWait(string key, Action action, LckInfo lckinfo, int expirySeconds = 30)
        {
            return LockOnExecute(key, action, lckinfo, expirySeconds, 0);
        }

        public override Tuple<bool, string> LockOnExecute(string key, Action action, LckInfo lckinfo, int expirySeconds = 30, int timeoutSeconds = 5)
        {
            var isBlockingMode = timeoutSeconds > 0; //是否阻塞等待模式

            if (!key.Contains(_lockkeyPrefix))
                key = _lockkeyPrefix + key;

            key = GetRegionKey(key);

            int _max_second = int.MaxValue;//最长定锁有效期
            int _max_timeout = int.MaxValue;//最长加锁等待时间

            int _times = 5;//续锁最多次数

            int _millsecond = 1000;

            expirySeconds = expirySeconds < 0 ? 5 : expirySeconds;
            expirySeconds = expirySeconds > _max_second ? _max_second : expirySeconds;

            timeoutSeconds = timeoutSeconds < 0 ? 5 : timeoutSeconds;
            timeoutSeconds = timeoutSeconds > _max_timeout ? _max_timeout : timeoutSeconds;

            string msg = "";
            var flag1 = false;
            //创建key
            if (GetCache<LckInfo>(key) == null)
            {
                lock (lockList)
                {
                    if (GetCache<LckInfo>(key) == null)
                    {
                        SetCache(key, lckinfo, expirySeconds);
                    }
                }
            }
            bool flag = false;

            bool isgetlock = false;
            var getlockElapsed = TimeSpan.FromSeconds(timeoutSeconds);
            if (!isBlockingMode)
            {
                getlockElapsed = TimeSpan.FromSeconds(Global.LockOptions.NoWaitModeGetLockWaitMillSeconds);
            }

            isgetlock = System.Threading.Monitor.TryEnter(GetCache<LckInfo>(key), getlockElapsed);


            if (!isgetlock)
            {
                flag1 = false;
                msg = $"key:[{key}]锁定失败,加锁等待超过{timeoutSeconds}秒!";
            }
            else
            {
                SaveLockInfo(key, lckinfo);

                if (isBlockingMode)
                {
                    CancellationTokenSource tokenSource = new CancellationTokenSource();
                    CancellationToken cancellationToken = tokenSource.Token;

                    Thread thread = null;
                    var workTask = Task.Run(() =>
                    {
                        thread = System.Threading.Thread.CurrentThread;
                        try
                        {
                            if (action != null)
                            {
                                action.Invoke();
                            }

                        }
                        //catch (Exception ex) //不要处理异常，否则上层应用捕获不到异常
                        //{
                        //    flag = false;
                        //    msg = $"操作业务失败!{ex}";
                        //    //Console.WriteLine($"线程中断。。");
                        //}
                        finally
                        {
                            //UnLock(lckinfo, key);不可以再此处解锁
                        }
                    });

                    flag = workTask.Wait(timeoutSeconds * _millsecond, cancellationToken);
                    if (flag)
                    {
                        UnLock(lckinfo, key);
                        msg = $"key:[{key}]锁定并操作业务成功!锁已自动释放";
                    }
                    else
                    {
                        var _timesa = 0;
                        while (!flag)
                        {
                            if (_timesa >= _times)
                            {
                                if (!workTask.IsCompleted)
                                {

                                    tokenSource.Cancel();
                                    thread.Interrupt();
                                }
                                UnLock(lckinfo, key);
                                flag = false;
                                msg = $"key:[{key}]锁定操作业务失败!超过最大[{_times}]次续锁没有完成,操作被撤销";
                                break;
                            }
                            else
                            {
                                //续锁
                                SetCache(key, lckinfo, expirySeconds);
                                lckinfo = getLockInfo(lckinfo, expirySeconds, _timesa);

                                SaveLockInfo(key, lckinfo);

                                flag = workTask.Wait(timeoutSeconds * _millsecond, cancellationToken);
                                if (flag)
                                {
                                    flag = true;
                                    UnLock(lckinfo, key);
                                    msg = $"key:[{key}]锁定并操作业务成功!续锁{_timesa + 1}次,锁已经自动释放";
                                    break;
                                }
                            }
                            _timesa++;
                        }

                        if (!flag)
                        {
                            msg = $"key:[{key}]锁定操作业务失败!超过最大[{_times}]次续锁没有完成,操作被撤销";
                        }
                    }
                }
                else
                {
                    try
                    {
                        action.Invoke();
                        flag = true;
                        msg = $"key:[{key}]锁定并操作业务成功!,锁已经自动释放";
                    }
                    //catch (Exception ex)  //不要处理异常，否则上层应用捕获不到异常
                    //{
                    //    flag = false;
                    //    msg = $"key:[{key}]锁定并操作业务失败!{ex}";
                    //}
                    finally
                    {
                        UnLock(lckinfo, key);
                    }
                    return new Tuple<bool, string>(flag, msg);
                }
            }

            var rtnlcks = Lock.CheckLock(key);
            return new Tuple<bool, string>(flag, msg);
        }


        /// <summary>
        /// 业务锁等待并执行
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="action"></param>
        /// <param name="lckinfo"></param>
        /// <param name="expirySeconds"></param>
        /// <param name="timeoutSeconds"></param>
        /// <returns></returns>
        public override Tuple<bool, string> LockOnExecute(string[] keys, Action action, LckInfo lckinfo, int expirySeconds = 30, int timeoutSeconds = 5)
        {
            var isBlockingMode = timeoutSeconds > 0; //是否阻塞等待模式
            if (keys.Length == 1)
            {
                return LockOnExecute(keys[0], action, lckinfo, expirySeconds, timeoutSeconds);
            }
            int _max_second = int.MaxValue;//最长定锁有效期
            int _max_timeout = int.MaxValue;//最长加锁等待时间

            int _times = 5;//续锁最多次数

            int _millsecond = 1000;

            expirySeconds = expirySeconds < 0 ? 5 : expirySeconds;
            expirySeconds = expirySeconds > _max_second ? _max_second : expirySeconds;

            timeoutSeconds = timeoutSeconds < 0 ? 5 : timeoutSeconds;
            timeoutSeconds = timeoutSeconds > _max_timeout ? _max_timeout : timeoutSeconds;

            string msg = "";
            bool flag = false;
            var getlockResult = LockOn(keys, lckinfo, expirySeconds, timeoutSeconds);
            if (!getlockResult.Item1)
            {
                flag = false;
                msg = getlockResult.Item2;
            }
            else
            {
                if (isBlockingMode)
                {
                    CancellationTokenSource tokenSource = new CancellationTokenSource();
                    CancellationToken cancellationToken = tokenSource.Token;
                    Thread thread = null;
                    var workTask = Task.Run(() =>
                    {
                        thread = System.Threading.Thread.CurrentThread;
                        try
                        {
                            if (action != null)
                            {
                                action.Invoke();
                            }
                        }
                        //catch (Exception ex)
                        //{
                        //    //Console.WriteLine($"线程中断。。");
                        //}
                        finally
                        {
                            //UnLock(keys); 不可以再此处解锁，否则会提示跨现场错误
                        }
                    });

                    flag = workTask.Wait(timeoutSeconds * _millsecond, cancellationToken);
                    if (flag)
                    {
                        UnLock(lckinfo, keys);
                        msg = $"key:[{string.Join(",", keys)}]锁定并操作业务成功!锁已自动释放";
                    }
                    else
                    {
                        var _timesa = 0;
                        while (!flag)
                        {
                            if (_timesa >= _times)
                            {
                                if (!workTask.IsCompleted)
                                {
                                    tokenSource.Cancel();
                                    thread.Interrupt();
                                }
                                UnLock(lckinfo, keys);
                                flag = false;
                                msg = $"key:[{string.Join(",", keys)}]锁定操作业务失败!超过最大[{_times}]次续锁没有完成,操作被撤销";
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

                                    SetCache(newKey, lckinfo, expirySeconds);
                                    lckinfo = getLockInfo(lckinfo, expirySeconds, _timesa);
                                    SaveLockInfo(newKey, lckinfo);
                                }
                                flag = workTask.Wait(timeoutSeconds * _millsecond, cancellationToken);
                                if (flag)
                                {
                                    UnLock(lckinfo, keys);
                                    flag = true;
                                    msg = $"key:[{string.Join(",", keys)}]锁定并操作业务成功!续锁{_timesa + 1}次,锁已经自动释放";
                                    break;
                                }
                            }
                            _timesa++;
                        }

                        if (!flag)
                        {
                            msg = $"key:[{keys}]锁定操作业务失败!超过最大[{_times}]次续锁没有完成,操作被撤销";
                        }
                    }
                }
                else
                {
                    try
                    {
                        action.Invoke();
                        flag = true;
                        msg = $"key:[{keys}]锁定并操作业务成功!,锁已经自动释放";
                    }
                    //catch (Exception ex)  //不要处理异常，否则上层应用捕获不到异常
                    //{
                    //    flag = false;
                    //    msg = $"key:[{keys}]锁定并操作业务失败!{ex}";
                    //}
                    finally
                    {
                        UnLock(lckinfo, keys);
                    }
                    return new Tuple<bool, string>(flag, msg);
                }
            }

            return new Tuple<bool, string>(flag, msg);
        }

        public override Tuple<bool, string> LockOnExecuteNoWait(string[] keys, Action action, LckInfo lckinfo, int expirySeconds = 30)
        {
            return LockOnExecute(keys, action, lckinfo, expirySeconds, 0);
        }
        LckInfo getLockInfo(LckInfo lckInfo, int expirySeconds, int times)
        {
            if (lckInfo.LockTime == null || lckInfo.LockTime == DateTime.MinValue)
                lckInfo.LockTime = DateTime.Now;

            if (lckInfo.ExpireTime == null || lckInfo.ExpireTime == DateTime.MinValue)
            {
                lckInfo.ExpireTime = DateTime.Now.AddSeconds(expirySeconds);
            }
            else
                lckInfo.ExpireTime = lckInfo.ExpireTime.AddSeconds(expirySeconds);

            lckInfo.KeepTimes = times;


            return lckInfo;
        }
        /// <summary>
        /// 解锁
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public override bool UnLock(LckInfo lckInfo, params string[] keys)
        {
            if (keys.Length == 0) return true;
            foreach (string key in keys)
            {
                var newkey = key;
                if (!newkey.Contains(_lockkeyPrefix))
                    newkey = _lockkeyPrefix + newkey;
                newkey = GetRegionKey(newkey);
                //创建key
                var cacheObj = GetCache<LckInfo>(newkey);
                if (cacheObj != null)
                {
                    if (System.Threading.Monitor.IsEntered(cacheObj))
                        System.Threading.Monitor.Exit(cacheObj);
                    //RemoveCache(newkey);  //不能移除缓存，否则多线程下，无法获得锁对象，另外的线程就没法锁定同一个对象，通过移除  Hashtable 实现所现场锁。
                    HDel(_lockhashname, newkey);
                }
            }

            return true;
        }

        /// <summary>
        /// 获取当前缓存锁信息
        /// </summary>
        /// <returns></returns>
        public override List<LckInfo> GetCurrLockInfo()
        {
            List<LckInfo> lckInfos = new List<LckInfo>();
            var newkey = GetRegionKey(_lockhashname);
            if (globalHSet.ContainsKey(newkey))
            {
                List<string> lckInfosWaitRemove = new List<string>();
                var list = (Hashtable)globalHSet[newkey];
                foreach (string key in list.Keys)
                {
                    if (GetCache<LckInfo>(key) != null && list[key] != null)
                    {
                        LckInfo lckInfo = JsonConvert.DeserializeObject<LckInfo>(list[key]?.ToString());
                        lckInfo.Key = key;
                        lckInfos.Add(lckInfo);
                    }
                    else
                    {
                        lckInfosWaitRemove.Add(key);

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
        /// 获取历史表锁信息
        /// </summary>
        /// <returns></returns>
        public override List<LckInfo> GetHisLockInfo()
        {
            var newkey = GetRegionKey(_lockhishashname);

            List<LckInfo> lckInfos = new List<LckInfo>();
            if (globalHSet.ContainsKey(newkey))
            {
                var list = (Hashtable)globalHSet[newkey];
                foreach (string key in list.Keys)
                {
                    if (list[key] != null)
                    {
                        LckInfo lckInfo = JsonConvert.DeserializeObject<LckInfo>(list[key]?.ToString());
                        lckInfo.Key = key;
                        lckInfos.Add(lckInfo);
                    }

                }
            }
            return lckInfos;
        }

        public override string LoadScript(string luascript)
        {
            throw new Exception($"基于本机的访问模式无法使用[LoadScript]方法,该方法仅限于Redis环境下使用");
        }

        public override string EvalSha(string shaid, string[] keys, string[] values)
        {
            throw new Exception($"本地缓存模式无法使用[EvalSha]方法,该方法仅限于Redis环境下使用");
        }

        public override bool EvalBoolSha(string shaid, RedisKey[] rediskeys, RedisValue[] redisvalues)
        {
            throw new Exception($"本地缓存模式无法使用[EvalBoolSha]方法,该方法仅限于Redis环境下使用");
        }



        /// <summary>
        /// 缓存类型
        /// </summary>
        public override CacheType CacheType => CacheType.MCache;
    }
}
