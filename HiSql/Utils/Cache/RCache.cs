using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
namespace HiSql
{
    /// <summary>
    /// 基于Redis的缓存
    /// </summary>
    public class RCache : BaseCache, IRedis
    {

        private readonly string UniqueId = Guid.NewGuid().ToString();
        private StackExchange.Redis.IDatabase _cache;
        private StackExchange.Redis.IServer _server;
        private ConnectionMultiplexer _connectMulti;
        private string _cache_notity_channel_remove = "hisql_cache_notity_channel@{0}__:remove";
        private MCache _MemoryCache;
        private RedisOptions _options;

        /// <summary>
        /// redis 加锁脚本的sha id值
        /// </summary>
        string lck_shaid = "";
        /// <summary>
        /// redis 解锁脚本的sha id值
        /// </summary>
        string unlck_shaid = "";

        /// <summary>
        /// 检测锁是否存在的 sha id值
        /// </summary>
        string chklck_shaid = "";

        /// <summary>
        /// 批量设置key的 sha id值
        /// </summary>
        string set_shaid = "";


        //检测表锁是否存在
        const String CheckScript = @"
            local _exist = 0
            local hgetkey=KEYS[1]

            local hgetval;
            local result={}

            local i = 2
            for i = 2,#KEYS do
                _exist = redis.call('exists',KEYS[i])
                if _exist==1 then
                    hgetval=redis.call('HGET', hgetkey, KEYS[i])
                    result[1]=1
                    result[2]=hgetval
                    result[3]=KEYS[i]
                    return result
                end
            end
            result[1]=_exist
            result[2]=''
            return result
            ";
        /// <summary>
        /// 解锁脚本2
        /// KEYS[1] =hsetkey 的 argv[1]=""
        /// </summary>
        const String UnlockScript_v2 = @"
                local hsetkey = KEYS[1]
                local i = 2                
                local cnt = 0
                for i = 2,#KEYS
                do
                    if redis.call('exists',KEYS[i]) then
                        redis.call('del',KEYS[i])
                        redis.call('hdel', hsetkey, KEYS[i])
                        cnt = cnt +  1
                    end
                end
                return cnt";

        /// <summary>
        /// KEYS[1]=hsetkey ARGV[1]=key_exp
        /// KEYS]2]=hsetval ARGV[2]=key_exp
        /// </summary>
        const String LockScriptFormat_v2 = @"
                        -- lock.lua
                        -- 同时锁定多个资源
                        local key_exp = ARGV[1]
                        local hsetkey = KEYS[1]
                        local hsetval = KEYS[2]
                        local non_exist = true
                        
                        local i = 3
                        for i = 3,#ARGV
                        do
                            local r = redis.call('GET',KEYS[i])
                            non_exist = (non_exist and not r)
                        end
                        if non_exist then
                            for i = 3,#ARGV
                            do
                                redis.call('set',KEYS[i],ARGV[i])
                                redis.call('expire',KEYS[i],key_exp)
                                redis.call('hset', hsetkey, KEYS[i], hsetval)
                                redis.call('expire',hsetkey,key_exp)
                            end
                            return true
                        end
                        return false
                        ";

        /// <summary>
        /// 批量设置Key
        /// </summary>
        string BatchSetScript = @"
            local _isok = false
            local _expire=ARGV[1]
            local hgetval;
            local result={}

            local i = 2
            for i = 2,#KEYS do
                redis.call('set', KEYS[i], ARGV[i])
                if _expire ~= '-1' and  _expire ~= '' then
                    redis.call('expire',KEYS[i],_expire)
                end
                _isok=true
                
            end
            return _isok
    ";

        public RCache(RedisOptions options):base(options)
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
            //加载本至redis 

            //加锁脚本 add by tgm date:2024.7.2
            lck_shaid=this.LoadScript(LockScriptFormat_v2);
            //解锁脚本 add by tgm date:2024.7.2
            unlck_shaid = this.LoadScript(UnlockScript_v2);
            //检测锁是否存在的脚本
            chklck_shaid = this.LoadScript(CheckScript);
            //批量设置key的脚本
            set_shaid = this.LoadScript(BatchSetScript);
        }



        //补偿重试的源代码
        protected bool retry(int retryCount, TimeSpan retryDelay, Func<bool> action)
        {
            int maxRetryDelay = (int)retryDelay.TotalMilliseconds;
            Random rnd = new Random();
            int currentRetry = 0;
            while (currentRetry++ < retryCount)
            {
                if (action()) return true;
                // 微循环, 隔一段时间执行一次,隔一段时间执行一次
                Thread.Sleep(rnd.Next(maxRetryDelay));
            }
            return false;
        }

        const string Base64Prefix = "base64\0";
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

        private void CheckRedisServer()
        {
            CheckRedis();
            if (_server == null || !_server.IsConnected)
            {
                string _connstr = $"{this._options.Host}:{this._options.Port}";
                //if (!string.IsNullOrEmpty(this._options.PassWord))
                //    _connstr = $"{_connstr},password={this._options.PassWord}";
                _server = _connectMulti.GetServer(_connstr);
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

                    var val = _cache.StringGetWithExpiry(key);
                    if (val.Value.HasValue)
                    {
                        obj = this.GetZipValue(val.Value);

                        _MemoryCache.SetCache(key, obj,  DateTime.Now.Add(val.Expiry.HasValue? val.Expiry.Value:TimeSpan.FromSeconds(10)));

                        if (obj.IndexOf("\"") >= 0)
                        {
                            obj = JsonConvert.DeserializeObject(obj)?.ToString();
                        }
                    }

                    else
                        obj = string.Empty;
                }
            }
            else
            {
                var val = _cache.StringGet(key);

                if (val.HasValue)
                    obj = this.GetZipValue(val);
                else
                    obj = string.Empty;

                if (obj.IndexOf("\"") >= 0 && val.HasValue)
                {
                    obj = JsonConvert.DeserializeObject(obj)?.ToString();
                }
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
                    var cachevalue = _cache.StringGetWithExpiry(key);
                    if (!cachevalue.Value.IsNull)
                    {
                        //判断是否是压缩的二进制 如果是则进行解压
                        string _strvale = string.Empty;
                        if (cachevalue.Value.HasValue)
                            _strvale = this.GetZipValue(cachevalue.Value);
                        //内存里面保存 解压后的、序列化的数据

                        _MemoryCache.SetCache(key, _strvale, DateTime.Now.Add(cachevalue.Expiry.HasValue ? cachevalue.Expiry.Value : TimeSpan.FromSeconds(10)));

                        value = JsonConvert.DeserializeObject<T>(_strvale);
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
                //判断是否是压缩的二进制 如果是则进行解压
                string _strvale = string.Empty;
                if (cachevalue.HasValue)
                    _strvale = this.GetZipValue(cachevalue);


                if (!cachevalue.IsNull)
                {
                    value = JsonConvert.DeserializeObject<T>(_strvale);
                }
            }

            return value;
        }

        public override T GetOrCreate<T>(string key, Func<T> value) where T : class
        {
            return GetOrCreate<T>(key, value, -1);
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
                if (_val == null)
                {
                    throw new Exception("不能将Null存入缓存");
                }
                string _value = JsonConvert.SerializeObject(_val);
                
                

                ///判断是否启用压缩 仅在redis环境中 设置压缩
                if (this.IsCanZip(_value))
                {
                    var _valzip = this.ZipCompress(_value);
                    _cache.StringSet(key, _valzip, second > 0 ? TimeSpan.FromSeconds(second) : null, When.NotExists, CommandFlags.None);
                }
                else
                    _cache.StringSet(key, _value, second > 0 ? TimeSpan.FromSeconds(second) : null, When.NotExists, CommandFlags.None);


                if (this._options.EnableMultiCache)
                {
                    //本地存储，并发布消息
                    if (second > 0)
                    {
                        _MemoryCache.SetCache(key, _value, DateTime.Now.Add(TimeSpan.FromSeconds(second)));
                    }
                    else
                    {
                        _MemoryCache.SetCache(key, _value);
                    }

                    this.PublishMessage(_cache_notity_channel_remove, GetRegionKeyForNotityKey(key));

                }
                return _val;
            }
            else
            {
                var obj= GetCache<T>(key);
                if (obj == default(T))
                {
                    T _val = value.Invoke();
                    string _value = JsonConvert.SerializeObject(_val);

                    //数据压缩
                    if (this.IsCanZip(_value))
                    {
                        var _valzip = this.ZipCompress(_value);
                        _cache.StringSet(key, _valzip, second > 0 ? TimeSpan.FromSeconds(second) : null, When.NotExists, CommandFlags.None);
                    }
                    else
                        _cache.StringSet(key, _value, second > 0 ? TimeSpan.FromSeconds(second) : null, When.NotExists, CommandFlags.None);

                    if (this._options.EnableMultiCache)
                    {
                        //本地存储，并发布消息
                        if (second > 0)
                        {
                            _MemoryCache.SetCache(key, _value, DateTime.Now.Add(TimeSpan.FromSeconds(second)));
                        }
                        else
                        {
                            _MemoryCache.SetCache(key, _value);
                        }

                        this.PublishMessage(_cache_notity_channel_remove, GetRegionKeyForNotityKey(key));

                    }
                    obj = _val;
                }
                return obj;
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

            //if (!string.IsNullOrWhiteSpace(_options.CacheRegion))
            //{
            //    _cache.HashDelete(_options.CacheRegion, fullKey, CommandFlags.FireAndForget);
            //}
            _cache.KeyDelete(fullKey);
            if (this._options.EnableMultiCache)
            {
                _MemoryCache.RemoveCache(key);
                this.PublishMessage(_cache_notity_channel_remove, GetRegionKeyForNotityKey(fullKey));
            }
        }

        public override void SetCache(string key, object value)
        {
            SetCache(key, value, -1);
        }

        public override void SetCache(string key, object value, DateTimeOffset expiryAbsoulte)
        {
            DateTimeOffset currdate = DateTimeOffset.Now;
            int _seconds = int.Parse(expiryAbsoulte.Subtract(currdate).TotalSeconds.ToString());
            if (_seconds > 0)
            {
                SetCache(key, value, _seconds);
            }
        }
        /// <summary>
        /// 批量设置key
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="second"></param>
        public override void SetCache(IDictionary<string, string> dic, int second = -1)
        {
            if (dic != null && dic.Count > 0)
            {

                CheckRedis();
                RedisKey[] keys = new RedisKey[dic.Count + 1];
                RedisValue[] vals = new RedisValue[dic.Count + 1];
                keys[0] = "expire";
                vals[0] = second <= 0 ? "-1" : second.ToString();
                int idx = 0;
                foreach (string key in dic.Keys)
                {
                    idx++;
                    keys[idx] = key;
                    vals[idx] = dic[key];
                }
                var redisResult = _cache.ScriptEvaluate(this.dic_sha[set_shaid], keys, vals);
            }
            else
                throw new Exception($"请传入需要批量设置的key信息");
        }

        public override void SetCache(string key, object value, int second)
        {
            if (value == null)
            {
                throw new Exception("不能将Null存入缓存");
            }
            CheckRedis();
            var fullKey = GetRegionKey(key);

            string _value = JsonConvert.SerializeObject(value);
            ///数据压缩
            if (this.IsCanZip(_value))
            {
                var _valzip = this.ZipCompress(_value);
                _cache.StringSet(fullKey, _valzip, second > 0 ? TimeSpan.FromSeconds(second) : null);
            }
            else
                _cache.StringSet(fullKey, _value, second > 0 ? TimeSpan.FromSeconds(second) : null);
            //if (!string.IsNullOrWhiteSpace(_options.CacheRegion))
            //{
            //    _cache.HashSet(_options.CacheRegion, fullKey, "regionKey", When.Always, CommandFlags.FireAndForget);
            //}
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

        public override bool UnLock(params string[] keys)
        {
            if (keys.Length == 0) return true;
            CheckRedis();

            Stopwatch stopwatch = Stopwatch.StartNew();

            int _argcount = 1;//前几个参数作为固定参与传入redis脚本
            RedisKey[] rediskeys = new RedisKey[keys.Length+ _argcount];
            //RedisValue[] redisvalues = new RedisValue[keys.Length+ _argcount];
            rediskeys[0] = _lockhashname;
            //redisvalues[0] = "";
            for (int i = 0; i < keys.Length; i++)
            {
                var newkey = keys[i];
                if (!newkey.Contains(_lockkeyPrefix))
                    newkey = _lockkeyPrefix + newkey;
                newkey = GetRegionKey(newkey);

                rediskeys[i+ _argcount] = newkey;
                //redisvalues[i+ _argcount] = lckinfo.UName;
            }
            //string luaStr = String.Format(UnlockScript, _lockhashname);
            var redisResult = _cache.ScriptEvaluate(this.dic_sha[unlck_shaid], rediskeys, null);
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

        public delegate bool deleLockOn(string key, int expirySeconds);


        /// <summary>
        /// 加业务锁 
        /// 加锁持续时间最长不超过60秒 默认30
        /// 等待加锁有效期最长不超过10秒 （默认5秒）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expirySeconds"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        //public override Tuple<bool, string> LockOn(string key, LckInfo lckinfo, int expirySeconds = 30, int timeoutSeconds = 5)
        //{
        //    CheckRedis();
        //    if (!key.Contains(_lockkeyPrefix))
        //        key = _lockkeyPrefix + key;

        //    key = GetRegionKey(key);

        //    int _max_second = int.MaxValue;//最长定锁有效期  原来是 60
        //    int _max_timeout = int.MaxValue;//最长加锁等待时间 原来是 10


        //    expirySeconds = expirySeconds < 0 ? 5 : expirySeconds;
        //    expirySeconds = expirySeconds > _max_second ? _max_second : expirySeconds;

        //    timeoutSeconds = timeoutSeconds < 0 ? 5 : timeoutSeconds;
        //    timeoutSeconds = timeoutSeconds > _max_timeout ? _max_timeout : timeoutSeconds;

        //    bool islocked = false;
        //    Stopwatch stopwatch = Stopwatch.StartNew();
        //    while (!islocked && stopwatch.Elapsed <= TimeSpan.FromSeconds(timeoutSeconds))
        //    {
        //        islocked = _cache.LockTake(key, lckinfo.UName, TimeSpan.FromSeconds(expirySeconds));
        //        Thread.Sleep(5);
        //    }
        //    string msg = "";
        //    Thread thread = null;
        //    //deleLockOn _deleLock = new deleLockOn(lockOn);
        //    //var workTask = Task.Run(() =>
        //    //{
        //    //    try
        //    //    {
        //    //        thread = System.Threading.Thread.CurrentThread;
        //    //        bool _success = _deleLock.Invoke(key, expirySeconds);
        //    //        if (_success)
        //    //        {
        //    //            lckinfo = getLockInfo(lckinfo, expirySeconds, 0);
        //    //            SaveLockInfo(key, lckinfo);
        //    //        }
        //    //        return _success;
        //    //    }
        //    //    catch (Exception ex)
        //    //    {
        //    //        return false;
        //    //    }
        //    //});
        //    //bool flag = workTask.Wait(timeoutSeconds * 1000, new CancellationToken(false));


        //    //
        //    //if (flag)
        //    //{
        //    //    if (workTask.Result)
        //    //    {
        //    //        msg = "加锁成功";
        //    //    }
        //    //    else
        //    //    {

        //    //        flag = false;
        //    //        msg = "加锁失败";
        //    //    }

        //    //}
        //    if (islocked)
        //    {
        //        lckinfo = getLockInfo(lckinfo, expirySeconds, 0);
        //        SaveLockInfo(key, lckinfo);
        //        msg = "加锁成功";
        //    }
        //    else
        //    {
        //        if (thread != null)
        //            thread.Interrupt();
        //        msg = $"key:[{key}]锁定失败,加锁等待超过{timeoutSeconds}秒!";
        //    }
        //    return new Tuple<bool, string>(islocked, msg);
        //}

        public override Tuple<bool, string> LockOn(string key, LckInfo lckinfo, int expirySeconds = 30, int timeoutSeconds = 5)
        {
            var isBlockingMode = timeoutSeconds > 0; //是否
            CheckRedis();
            if (!key.Contains(_lockkeyPrefix))
                key = _lockkeyPrefix + key;

            key = GetRegionKey(key);

            int _max_second = int.MaxValue;//最长定锁有效期  原来是 60
            int _max_timeout = int.MaxValue;//最长加锁等待时间 原来是 10


            expirySeconds = expirySeconds < 0 ? 5 : expirySeconds;
            expirySeconds = expirySeconds > _max_second ? _max_second : expirySeconds;

            timeoutSeconds = timeoutSeconds < 0 ? 5 : timeoutSeconds;
            timeoutSeconds = timeoutSeconds > _max_timeout ? _max_timeout : timeoutSeconds;

            if (lckinfo.LockTime == null)
                lckinfo.LockTime = DateTime.Now;

            lckinfo.ExpireTime = lckinfo.LockTime.AddSeconds(expirySeconds);

            bool getlocked = false;
            var getlockElapsed = TimeSpan.FromSeconds(timeoutSeconds);
            if (!isBlockingMode)
            {
                getlockElapsed = TimeSpan.FromMilliseconds(Global.LockOptions.NoWaitModeGetLockWaitMillSeconds);
            }
            Stopwatch stopwatch = Stopwatch.StartNew();
            while (!getlocked && stopwatch.Elapsed <= getlockElapsed)
            {
                getlocked = _cache.LockTake(key, lckinfo.UName, TimeSpan.FromSeconds(expirySeconds));
                if (getlocked) break;
                Thread.Sleep(Global.LockOptions.GetLockRetrySleepMillSeconds);
            }
            string msg = "";
            Thread thread = null;
            //deleLockOn _deleLock = new deleLockOn(lockOn);
            //var workTask = Task.Run(() =>
            //{
            //    try
            //    {
            //        thread = System.Threading.Thread.CurrentThread;
            //        bool _success = _deleLock.Invoke(key, expirySeconds);
            //        if (_success)
            //        {
            //            lckinfo = getLockInfo(lckinfo, expirySeconds, 0);
            //            SaveLockInfo(key, lckinfo);
            //        }
            //        return _success;
            //    }
            //    catch (Exception ex)
            //    {
            //        return false;
            //    }
            //});
            //bool flag = workTask.Wait(timeoutSeconds * 1000, new CancellationToken(false));


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
            if (getlocked)
            {
                lckinfo = getLockInfo(lckinfo, expirySeconds, 0);
                SaveLockInfo(key, lckinfo);
                msg = "加锁成功";
            }
            else
            {
                if (thread != null)
                    thread.Interrupt();

                msg = isBlockingMode ? $"key:[{key}]锁定失败,加锁等待超过{timeoutSeconds}秒!" : $"key:[{key}]锁定失败!";
            }
            return new Tuple<bool, string>(getlocked, msg);
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
        public override Tuple<bool, string> LockOn(string[] keys, LckInfo lckinfo, int expirySeconds = 30, int timeoutSeconds = 5)
        {
            var isBlockingMode = timeoutSeconds > 0; //是否
            Tuple<bool, string> tuple = new Tuple<bool, string>(false, "获取锁超时");

            if (keys.Length == 1)
            {
                return LockOn(keys[0], lckinfo, expirySeconds, timeoutSeconds);
            }

            int _argcount = 2;
            RedisKey[] rediskeys = new RedisKey[keys.Length+ _argcount];
            RedisValue[] redisvalues = new RedisValue[keys.Length+ _argcount];

            rediskeys[0] = _lockhashname;
            redisvalues[0] = expirySeconds;

            //rediskeys[1] = JsonConvert.SerializeObject(lckinfo).Replace("\"", "\\\"");
            //if(keys.Count()==1)
            //    lckinfo.Key = keys[0];

            if(lckinfo.LockTime==null)
                lckinfo.LockTime=DateTime.Now;

            lckinfo.ExpireTime= lckinfo.LockTime.AddSeconds(expirySeconds);


            string _lckjson= JsonConvert.SerializeObject(lckinfo); 
            rediskeys[1] = _lckjson;
            redisvalues[1] = expirySeconds;

            for (int i = 0; i < keys.Length; i++)
            {
                var newkey = keys[i];
                if (!newkey.Contains(_lockkeyPrefix))
                    newkey = _lockkeyPrefix + newkey;
                newkey = GetRegionKey(newkey);

                rediskeys[i + _argcount] = newkey;
                redisvalues[i+ _argcount] = lckinfo.UName;
            }
            var getlocked = false;
            //string luaStr = String.Format(LockScriptFormat, expirySeconds, _lockhashname, JsonConvert.SerializeObject(lckinfo).Replace("\"", "\\\""));

            var getlockElapsed = TimeSpan.FromSeconds(timeoutSeconds);
            if (!isBlockingMode)
            {
                getlockElapsed = TimeSpan.FromMilliseconds(Global.LockOptions.NoWaitModeGetLockWaitMillSeconds);
            }
            Stopwatch stopwatch = Stopwatch.StartNew();

            while (!getlocked && stopwatch.Elapsed <= getlockElapsed)
            {

                //var redisResult = _cache.ScriptEvaluate(luaStr, rediskeys, redisvalues);
                var redisResult = _cache.ScriptEvaluate(this.dic_sha[lck_shaid], rediskeys, redisvalues);
                if (((bool)redisResult))
                {
                    getlocked = true;
                    break;
                }
                else
                {
                    Thread.Sleep(Global.LockOptions.GetLockRetrySleepMillSeconds);
                }
            }

            if (!getlocked)
            {
                return new Tuple<bool, string>(false, "获取锁等待超时。");
            }

            return new Tuple<bool, string>(true, $"key:[{string.Join(",", keys)}]加锁成功");

            //lock (lockhashtable)
            //{

            //    List<string> lockedKey = new List<string>();
            //    int idx = 0;
            //    while (keys.Length > 1 && CkeckExists(keys) && stopwatch.Elapsed <= TimeSpan.FromSeconds(timeoutSeconds * 3))
            //    {
            //        Thread.Sleep(100);
            //        if (stopwatch.Elapsed > TimeSpan.FromSeconds(timeoutSeconds * 3))
            //        {
            //            return new Tuple<bool, string>(false, "等待超时。");
            //        }
            //    }
            //    while (!tuple.Item1 && stopwatch.Elapsed <= TimeSpan.FromSeconds(timeoutSeconds) && idx < keys.Length)
            //    {
            //        if (keys.Length > 1 && idx == 0)
            //        {
            //            lock (hashtable)
            //            {
            //                if (CkeckExists(keys))
            //                {
            //                    Thread.Sleep(100);
            //                    continue;
            //                }
            //                hashtable.AddRange(keys);
            //            }
            //        }

            //        var lockResult = LockOn(keys[idx], lckinfo, expirySeconds, timeoutSeconds);
            //        if (!lockResult.Item1)
            //        {
            //            tuple = lockResult;
            //            break;
            //        }

            //        lockedKey.Add(keys[idx]);
            //        idx++;
            //    }
            //    if (idx == keys.Length && keys.Length > 1)
            //    {
            //        lock (hashtable)
            //        {
            //            for (int i = 0; i < keys.Length; i++)
            //            {
            //                if (hashtable.Contains(keys[i]))
            //                {
            //                    hashtable.Remove(keys[i]);
            //                }
            //            }
            //        }
            //        return new Tuple<bool, string>(true, $"key:[{string.Join(",", keys)}]加锁成功");
            //    }
            //    if (keys.Length > 1)
            //    {
            //        lock (hashtable)
            //        {
            //            for (int i = 0; i < keys.Length; i++)
            //            {
            //                hashtable.Remove(keys[i]);
            //            }
            //        }
            //    }

            //    //加锁失败或超时
            //    UnLock(lckinfo, lockedKey.ToArray());
            //    return new Tuple<bool, string>(tuple.Item1, tuple.Item2);
            //}
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
            CheckRedis();
            List<string> lstkeys=new List<string> ();

            bool islock = false;
            string msg = string.Empty;
            lstkeys.Add(_lockhashname);
            foreach (var key in keys)
            {
                if (!key.Contains(_lockkeyPrefix))
                    lstkeys.Add( _lockkeyPrefix + key);
            }
            string result = EvalSha(chklck_shaid, lstkeys.ToArray(), null);
            List<string> lst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(result);
            if (lst.Count >= 2)
            {
                if (lst[0].Equals("0"))
                {
                    islock = false;
                    msg = $"未被锁定";
                }
                else if (lst[0].Equals("1"))
                {
                    islock = true;
                    if (!string.IsNullOrEmpty(lst[1]))
                    {
                        
                        LckInfo info= Newtonsoft.Json.JsonConvert.DeserializeObject<LckInfo>(lst[1]);
                        string _key = lst[2] != null ? lst[2] : "";
                        if (string.IsNullOrEmpty(info.Key))
                        {
                            if (!string.IsNullOrEmpty(_key))
                                msg = $"key:[{_key}]";
                        }
                        else
                            msg = $"key:[{info.Key}]";
                        
                        if (info != null)
                        {
                            if (!string.IsNullOrEmpty(info.UName))
                                msg += $" 被[{info.UName}]";
                            if (!string.IsNullOrEmpty(info.EventName)) msg += $" 在[{info.EventName}]";

                            if (info.LockTime != null)
                            {
                                if (info.LockTime.Year >= 1970)
                                    msg += $" 于[{info.LockTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}]进行锁定";

                                if (info.ExpireTime != null)
                                {
                                    if (info.ExpireTime.Year >= DateTime.Now.Year)
                                    {
                                        msg += $" 预计[{info.ExpireTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}]自动解锁";
                                    }
                                }
                            }
                            else
                                msg += "进行锁定";
                            if (!string.IsNullOrEmpty(info.Descript))
                            {
                                msg += $" 备注:{info.Descript}";
                            }
                        }
                        else
                        {
                            msg += $"已经被锁定";
                        }
                        
                    }else
                        msg += $"已经被锁定";

                }
                else
                {
                    islock = false;
                    msg = $"无法识别锁定状态";
                }
            }
            return new Tuple<bool, string>(islock, msg);
        }
        /// <summary>
        /// 锁定并执行业务
        /// </summary>
        /// <param name="key">需要锁定的key</param>
        /// <param name="action">锁定后执行的方法</param>
        /// <param name="lckinfo">锁定信息</param>
        /// <param name="expirySeconds">锁定持续时间</param>
        /// <param name="timeoutSeconds">等待加锁时间</param>
        /// <returns></returns>
        public override Tuple<bool, string> LockOnExecuteNoWait(string key, Action action, LckInfo lckinfo, int expirySeconds = 30)
        {
            return LockOnExecute(key, action, lckinfo, expirySeconds, 0);
        }

        /// <summary>
        /// 锁定并执行业务
        /// </summary>
        /// <param name="key">需要锁定的key</param>
        /// <param name="action">锁定后执行的方法</param>
        /// <param name="lckinfo">锁定信息</param>
        /// <param name="expirySeconds">锁定持续时间</param>
        /// <param name="timeoutSeconds">等待加锁时间</param>
        /// <returns></returns>
        public override Tuple<bool, string> LockOnExecute(string key, Action action, LckInfo lckinfo, int expirySeconds = 30, int timeoutSeconds = 5)
        {

            var isBlockingMode = timeoutSeconds > 0; //是否

            CheckRedis();
            if (!key.Contains(_lockkeyPrefix))
                key = _lockkeyPrefix + key;

            key = GetRegionKey(key);

            int _max_second = int.MaxValue;//最长定锁有效期
            int _max_timeout = int.MaxValue;//最长加锁等待时间
            int _times = 0;//续锁最多次数
            int _millsecond = 900;

            expirySeconds = expirySeconds < 0 ? 5 : expirySeconds;
            expirySeconds = expirySeconds > _max_second ? _max_second : expirySeconds;

            if (expirySeconds <= timeoutSeconds)
            {
                expirySeconds = timeoutSeconds + 2;
            }
            timeoutSeconds = timeoutSeconds < 0 ? 5 : timeoutSeconds;
            timeoutSeconds = timeoutSeconds > _max_timeout ? _max_timeout : timeoutSeconds;

            if (lckinfo.LockTime == null)
                lckinfo.LockTime = DateTime.Now;

            lckinfo.ExpireTime = lckinfo.LockTime.AddSeconds(expirySeconds);

            bool getlocked = false;
            var getlockElapsed = TimeSpan.FromSeconds(timeoutSeconds);
            if (!isBlockingMode)
            {
                getlockElapsed = TimeSpan.FromMilliseconds(Global.LockOptions.NoWaitModeGetLockWaitMillSeconds);
            }

            Stopwatch stopwatch = Stopwatch.StartNew();
            while (!getlocked && stopwatch.Elapsed <= getlockElapsed)
            {
                getlocked = _cache.LockTake(key, lckinfo.UName, TimeSpan.FromSeconds(expirySeconds));
                if (getlocked) break;

                Thread.Sleep(Global.LockOptions.GetLockRetrySleepMillSeconds);
            }

            string msg = "";
            Thread thread = null;

            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = tokenSource.Token;

            bool flag = false;
            if (getlocked) //获取锁成功
            {
                lckinfo = getLockInfo(lckinfo, expirySeconds, 0);
                SaveLockInfo(key, lckinfo);
                if (action == null)
                {
                    UnLock(key);
                    flag = true;
                    msg = $"key:[{key}]锁定成功，但是业务方法为空!,锁已经自动释放";
                    return new Tuple<bool, string>(flag, msg);
                }
                if (isBlockingMode)
                {
                    int _timesa = 0;
                    var workTask = Task.Run(() =>
                    {
                        thread = System.Threading.Thread.CurrentThread;
                        try
                        {
                            action.Invoke();
                        }
                        //catch (Exception ex) //不要处理异常，否则上层应用捕获不到异常
                        //{
                        //    flag = false;
                        //    msg = $"key:[{key}]锁定并操作业务失败!{ex}";
                        //    //Console.WriteLine($"线程中断。。");
                        //}
                        finally
                        {
                            UnLock(key);
                        }
                    });
                    flag = workTask.Wait(expirySeconds * _millsecond, cancellationToken);

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
                                UnLock(key);
                                tokenSource.Cancel();
                                if (thread != null)
                                {
                                    thread.Interrupt();
                                }
                            }
                            flag = false;
                            msg = $"key:[{key}]锁定操作业务失败!超过最大[{_times}]次续锁没有完成,操作被撤销";
                            break;
                        }
                        else
                        {
                            //续锁
                            _cache.StringSet(key, 1, TimeSpan.FromSeconds(expirySeconds));
                            lckinfo = getLockInfo(lckinfo, expirySeconds, _timesa);
                            SaveLockInfo(key, lckinfo);

                            flag = workTask.Wait(expirySeconds * _millsecond, cancellationToken);
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
                else
                {
                    try
                    {
                        action.Invoke();
                        flag = true;
                        msg = $"key:[{key}]锁定并操作业务成功!,锁已经自动释放";
                    }
                    //catch (Exception ex) //不要处理异常，否则上层应用捕获不到异常
                    //{
                    //    flag = false;
                    //    msg = $"key:[{key}]锁定并操作业务失败!{ex}";
                    //    //Console.WriteLine($"线程中断。。");
                    //}
                    finally
                    {
                        UnLock(key);
                    }

                    return new Tuple<bool, string>(flag, msg);
                }
            }
            else  //获取锁失败
            {
                msg = isBlockingMode ? $"key:[{key}]锁定失败,加锁等待超过{timeoutSeconds}秒!" : $"key:[{key}]锁定失败!";
            }

            return new Tuple<bool, string>(flag, msg);
        }

        public override Tuple<bool, string> LockOnExecuteNoWait(string[] keys, Action action, LckInfo lckinfo, int expirySeconds = 30)
        {
            return LockOnExecute(keys, action, lckinfo, expirySeconds, 0);
        }

        public override Tuple<bool, string> LockOnExecute(string[] keys, Action action, LckInfo lckinfo, int expirySeconds = 30, int timeoutSeconds = 5)
        {
            var isBlockingMode = timeoutSeconds > 0; //是否
            if (keys.Length == 1)
            {
                return LockOnExecute(keys[0], action, lckinfo, expirySeconds, timeoutSeconds);
            }
            CheckRedis();
            int _max_second = int.MaxValue;//最长定锁有效期
            int _max_timeout = int.MaxValue;//最长加锁等待时间
            int _times = 5;//续锁最多次数
            int _millsecond = 800;

            expirySeconds = expirySeconds < 0 ? 5 : expirySeconds;
            expirySeconds = expirySeconds > _max_second ? _max_second : expirySeconds;

            if (expirySeconds <= timeoutSeconds)
            {
                expirySeconds = timeoutSeconds + 2;
            }
            timeoutSeconds = timeoutSeconds < 0 ? 5 : timeoutSeconds;
            timeoutSeconds = timeoutSeconds > _max_timeout ? _max_timeout : timeoutSeconds;

            if (lckinfo.LockTime == null)
                lckinfo.LockTime = DateTime.Now;

            lckinfo.ExpireTime = lckinfo.LockTime.AddSeconds(expirySeconds);

            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = tokenSource.Token;
            Thread thread = null;
            string msg = "";
            bool flag = false;
            var getlockResult = LockOn(keys, lckinfo, expirySeconds, timeoutSeconds);
            if (!getlockResult.Item1)
            {
                flag = false;
                msg = getlockResult.Item2;
            }
            else
            {//获取锁成功

                if (action == null)
                {
                    UnLock(keys);
                    flag = true;
                    msg = $"key:[{keys}]锁定成功，但是业务方法为空!,锁已经自动释放";
                    return new Tuple<bool, string>(flag, msg);
                }
                if (isBlockingMode)
                {
                    int _timesa = 0;
                    var workTask = Task.Run(() =>
                    {
                        thread = System.Threading.Thread.CurrentThread;
                        try
                        {
                            action.Invoke();
                        }
                        //catch (Exception ex)  //不要处理异常，否则上层应用捕获不到异常
                        //{
                        //    //Console.WriteLine($"线程中断。。");
                        //}
                        finally
                        {
                            UnLock(keys);
                        }
                    });
                    flag = workTask.Wait(expirySeconds * _millsecond, cancellationToken);

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
                                UnLock(keys);
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

                                _cache.StringSet(newKey, lckinfo.UName, TimeSpan.FromSeconds(expirySeconds));
                                lckinfo = getLockInfo(lckinfo, expirySeconds, _timesa);
                                SaveLockInfo(newKey, lckinfo);

                            }
                            flag = workTask.Wait(expirySeconds * _millsecond, cancellationToken);
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
                else
                {
                    try
                    {
                        action.Invoke();
                        flag = true;
                        msg = $"key:[{keys}]锁定并操作业务成功!,锁已经自动释放";
                    }
                    //catch (Exception ex) //不要处理异常，否则上层应用捕获不到异常
                    //{
                    //    flag = false;
                    //    msg = $"key:[{keys}]锁定并操作业务失败!{ex}";
                    //}
                    finally
                    {
                        UnLock(keys);
                    }
                    return new Tuple<bool, string>(flag, msg);
                }
            }

            return new Tuple<bool, string>(flag, msg);
        }

        /// <summary>
        /// 缓存类型
        /// </summary>
        public override CacheType CacheType => CacheType.RCache;

        public string ExecLuaScript(string script, object obj = null)
        {
            var scriptObj = LuaScript.Prepare(script);
            var redisResult = _cache.ScriptEvaluate(scriptObj, obj);
            return redisResult.ToString();
        }

        public override string LoadScript(string luascript)
        {
            string shaid = string.Empty;
            CheckRedisServer();
            var bytes=_server.ScriptLoad(luascript);
            
            //成功加载脚本后需要进行缓存
            if (bytes != null && bytes.Length > 0)
            {
                shaid=BitConverter.ToString(bytes).Replace("-", "").ToLower();
                if (!this.dic_sha.ContainsKey(shaid))
                {
                    this.dic_sha.Add(shaid,bytes);
                }

            }
            return shaid;

        }

        /// <summary>
        /// 执行已经装入的redis sha脚本
        /// </summary>
        /// <param name="shaid"></param>
        /// <param name="keys"></param>
        /// <param name="values"></param>
        /// <returns>如果值是单值时返回的是string 如果是多值是返回的是List<string> 的Json</returns>
        /// <exception cref="Exception"></exception>
        public override string EvalSha(string shaid, string[] keys, string[] values)
        {
            if (this.dic_sha.ContainsKey(shaid))
            {
                RedisKey[] rediskeys=new RedisKey[] { };
                RedisValue[] redisvalues=new RedisValue[] { };

                if(keys!=null && keys.Length>0)
                {
                    rediskeys = new RedisKey[keys.Length];
                    for (int i = 0; i < keys.Length; i++)
                    {
                        rediskeys[i] = (RedisKey)keys[i];
                    }
                }
                if(values!=null && values.Length>0)
                {
                    redisvalues = new RedisValue[values.Length];
                    for (int i = 0; i < values.Length; i++)
                    {
                        redisvalues[i] =  ( RedisValue)values[i];
                    }
                }

                var result=_cache.ScriptEvaluate(this.dic_sha[shaid], rediskeys, redisvalues);
                RedisValue[] redisValues = (RedisValue[])result;
                List<string> lstresult = new List<string>();
                if (redisValues != null && redisValues.Length > 0)
                {
                    foreach (RedisValue value in redisValues)
                    {
                        if (value.HasValue)
                        {
                            lstresult.Add(value.ToString());
                        }
                        else
                            lstresult.Add(null);
                    }
                }
                return lstresult.ToJson();
                
               
            }
            else
            {
                throw new Exception($"Redis ShaId:[{shaid}] 不存在");
            }
        }
        /// <summary>
        /// 执行已经装入的redis sha脚本
        /// 注意一定要在lua脚本中自定义返回 true|false
        /// </summary>
        /// <param name="shaid"></param>
        /// <param name="keys"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public override bool EvalBoolSha(string shaid, RedisKey[] rediskeys, RedisValue[] redisvalues)
        {
            if (this.dic_sha.ContainsKey(shaid))
            {
                //RedisKey[] rediskeys = new RedisKey[] { };
                //RedisValue[] redisvalues = new RedisValue[] { };

                //if (keys != null && keys.Length > 0)
                //{
                //    rediskeys = new RedisKey[keys.Length];
                //    for (int i = 0; i < keys.Length; i++)
                //    {
                //        rediskeys[i] = (RedisKey)keys[i];
                //    }
                //}
                //if (values != null && values.Length > 0)
                //{
                //    redisvalues = new RedisValue[values.Length];
                //    for (int i = 0; i < values.Length; i++)
                //    {
                //        redisvalues[i] = (RedisValue)values[i];
                //    }
                //}

                var result = _cache.ScriptEvaluate(this.dic_sha[shaid], rediskeys, redisvalues);
                if (((bool)result))
                    return true;
                else 
                    return false; 
                


            }
            else
            {
                throw new Exception($"Redis ShaId:[{shaid}] 不存在");
            }
        }




        #region 私有变量
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

        public string ExecLuaScript(string script, string[] keys = null, string[] values = null)
        {
            RedisKey[] rediskeys = null;
            if (keys != null)
            {
                rediskeys = new RedisKey[keys.Length];
                for (int i = 0; i < keys.Length; i++)
                {
                    rediskeys[i] = keys[i]?.ToString();
                }
            }
            RedisValue[] redisvalues = null;
            if (values != null)
            {
                redisvalues = new RedisValue[values.Length];
                for (int i = 0; i < values.Length; i++)
                {
                    redisvalues[i] = values[i];
                }
            }

            var redisResult = _cache.ScriptEvaluate(script, rediskeys, redisvalues);
            return redisResult.ToString();
        }

        

        //bool lockOn(string key, int expirySeconds = 5)
        //{
        //    expirySeconds = expirySeconds < 0 ? 5 : expirySeconds;
        //    expirySeconds = expirySeconds > 60 ? 60 : expirySeconds;
        //    //deleLockOn _deleLock=new deleLockOn ()
        //    bool isok = false;
        //    while (!isok)
        //    {
        //        isok = _cache.LockTake(key, 1, TimeSpan.FromSeconds(expirySeconds));
        //        //return isok;
        //        if (isok)
        //        {
        //            break;
        //        }
        //        Thread.Sleep(10);
        //    }
        //    return true;
        //}
        //bool lockOnNoWait(string key, int expirySeconds = 5)
        //{
        //    bool isok = false;           
        //    isok = _cache.LockTake(key, 1, TimeSpan.FromSeconds(expirySeconds));
        //    return isok;
        //}

        #endregion

    }
}
