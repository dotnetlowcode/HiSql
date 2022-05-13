using System;
using System.Collections.Generic;
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
    public class RCache : IRedis
    {

        private StackExchange.Redis.IDatabase _cache;
        private ConnectionMultiplexer _connectMulti;


        private string _lockhashname = $"{HiSql.Constants.NameSpace}:locktable";
        private string _lockhishashname = $"{HiSql.Constants.NameSpace}:locktable_his";

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
        /// 解除锁定
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>

        public bool UnLock(string key)
        {
            checkRedis();
            key = checkKey(key);
            HDel(_lockhashname, key);
            return  _cache.LockRelease(key, 1);
            
        }
        public bool HSet(string hashkey,string key, string value)
        {
            checkRedis();
            hashkey = checkKey(hashkey);

            return _cache.HashSet(hashkey, key, value, When.Always, CommandFlags.None);

            
        }

        public bool HDel(string hashkey, string key)
        {
            checkRedis();
            hashkey = checkKey(hashkey);

            return _cache.HashDelete(hashkey, key);
        }

        public string HGet(string hashkey,string key)
        {
            checkRedis();
            hashkey = checkKey(hashkey);
            
            return _cache.HashGet(hashkey,key);
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
        public Tuple<bool, string> LockOn(string key, LckInfo lckinfo, int expresseconds = 30, int timeoutseconds = 5)
        {
            checkRedis();
            key = $"lck:{key}";
            key = checkKey(key);

            int _max_second = 60;//最长定锁有效期
            int _max_timeout = 10;//最长加锁等待时间


            expresseconds = expresseconds < 0 ? 5 : expresseconds;
            expresseconds = expresseconds > _max_second ? _max_second : expresseconds;

            timeoutseconds = timeoutseconds < 0 ? 5 : timeoutseconds;
            timeoutseconds = timeoutseconds > _max_timeout ? _max_timeout : timeoutseconds;

            deleLockOn _deleLock = new deleLockOn(lockOn);
            var workTask = Task.Run(() =>
            {
                bool _success = _deleLock.Invoke(key, expresseconds);
                if (_success)
                {
                    lckinfo = getLockInfo(lckinfo, expresseconds, 0);
                    HSet(_lockhashname, key, JsonConvert.SerializeObject(lckinfo));
                    HSet(_lockhishashname, $"{key}:{lckinfo.LockTime.ToString("yyyyMMddHHmmssfff")}", JsonConvert.SerializeObject(lckinfo));
                }
                return _success;
            });
            bool flag = workTask.Wait(timeoutseconds * 1000, new CancellationToken(false));


            string msg = "";
            if (flag)
            {
                if (workTask.Result)
                {
                    msg = "加锁成功";
                }
                else
                {

                    flag = false;
                    msg = "加锁失败";
                }

            }
            else
            {
                workTask.Wait(new CancellationToken(true));
                msg = $"key:[{key}]锁定失败,加锁等待超过{timeoutseconds}秒!";
            }
            return new Tuple<bool, string>(flag, msg);
        }


        /// <summary>
        /// 检测Key是否被锁定
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Tuple<bool, string> CheckLock(string key)
        {
            checkRedis();
            string _key = key;
            key = $"lck:{key}";
            key = checkKey(key);
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

                }else
                    _msg = $"key:[{_key}]被锁定";
            }
            else
            {
                _msg = $"key:[{_key}]未被锁定";
            }
            return new Tuple<bool,string>(_islock, _msg);

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
        public Tuple<bool, string> LockOnExecute(string key, Action action,LckInfo lckinfo, int expresseconds = 30, int timeoutseconds = 5)
        {
            checkRedis();
            key = $"lck:{key}";
            key = checkKey(key);

            int _max_second = 60;//最长定锁有效期
            int _max_timeout = 10;//最长加锁等待时间

            int _times = 5;//续锁最多次数

            int _millsecond = 900;


            expresseconds = expresseconds < 0 ? 5 : expresseconds;
            expresseconds = expresseconds > _max_second ? _max_second : expresseconds;

            timeoutseconds = timeoutseconds < 0 ? 5 : timeoutseconds;
            timeoutseconds = timeoutseconds > _max_timeout ? _max_timeout : timeoutseconds;

            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = tokenSource.Token;
            deleLockOn _deleLock = new deleLockOn(lockOn);
            Thread thread = null;
            int _timesa = 0;
            var workTask = Task.Run(() => { 
                thread=System.Threading.Thread.CurrentThread;
                try
                {
                    if (_deleLock.Invoke(key, expresseconds))
                    {
                        lckinfo = getLockInfo(lckinfo, expresseconds, 0);
                        HSet(_lockhashname, key, JsonConvert.SerializeObject(lckinfo));
                        HSet(_lockhishashname, $"{key}:{lckinfo.LockTime.ToString("yyyyMMddHHmmssfff")}", JsonConvert.SerializeObject(lckinfo));
                        action.Invoke();
                    }
                }
                catch (Exception ex)
                {
                    //Console.WriteLine($"线程中断。。");
                }
            });
            bool flag = workTask.Wait(timeoutseconds * _millsecond, cancellationToken);
            string msg = "";
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
                        thread.Interrupt();
                        tokenSource.Cancel();
                    }
                    flag = false;
                    break;
                }
                else
                {
                    //续锁
                    _cache.StringSet(key, 1, TimeSpan.FromSeconds(expresseconds));
                    lckinfo = getLockInfo(lckinfo, expresseconds, _timesa);
                    HSet(_lockhashname, key, JsonConvert.SerializeObject(lckinfo));
                    HSet(_lockhishashname, $"{key}:{lckinfo.LockTime.ToString("yyyyMMddHHmmssfff")}", JsonConvert.SerializeObject(lckinfo));
                    flag = workTask.Wait(timeoutseconds * _millsecond, cancellationToken);
                    if (flag)
                    {
                        msg = $"key:[{key}]锁定并操作业务成功!续锁{_timesa+1}次,锁已经自动释放";
                    }
                }
                _timesa++;
            }

            if (!flag)
            {
                msg = $"key:[{key}]锁定操作业务失败!超过最大[{_times}]次续锁没有完成,操作被撤销";
            }
            UnLock(key);
            return new Tuple<bool, string>(flag, msg);
        }


        #region 私有变量
        LckInfo getLockInfo(LckInfo lckInfo, int expresseconds, int times)
        {

            if(lckInfo.LockTime==null || lckInfo.LockTime==DateTime.MinValue)
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

        bool lockOn(string key, int expresseconds = 5)
        {
            expresseconds = expresseconds < 0 ? 5 : expresseconds;
            expresseconds = expresseconds > 60 ? 60 : expresseconds;
            //deleLockOn _deleLock=new deleLockOn ()
            bool isok = false;
            while (!isok)
            {
                isok = _cache.LockTake(key, 1, TimeSpan.FromSeconds(expresseconds));
                
                if (isok)
                {
                    break;
                }
            }
            return true;
        }

        #endregion

    }
}
