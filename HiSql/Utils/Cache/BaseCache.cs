using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

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
        RedisOptions _options = null;
        protected BaseCache(RedisOptions options)
        {
            _options = options;
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
        /// <summary>
        /// zip 文本压缩转成二进制
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected byte[] ZipCompress(string text)
        {
            byte[] compressed;
            using (var memoryStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
                {
                    byte[] bytesToCompress = Encoding.UTF8.GetBytes(text);
                    gzipStream.Write(bytesToCompress, 0, bytesToCompress.Length);
                    gzipStream.Close();
                    compressed = memoryStream.ToArray();
                }
            }
            return compressed;
        }
        /// <summary>
        /// 文本解压
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        protected string ZipDeCompress(byte[] bytes)
        {
            string text=string.Empty;
            using (var inputStream = new MemoryStream(bytes))
            {
                using (var decompressionStream = new GZipStream(inputStream, CompressionMode.Decompress))
                {
                    using (var reader = new StreamReader(decompressionStream))
                    {
                        text = reader.ReadToEnd();
                    }
                }
            }
            return text;
        }

        /// <summary>
        /// 判断值是否需要进行压缩
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected bool IsCanZip(string value)
        {
            if (_options != null)
            {
                if (_options.IsZip && value.LengthZH() >= _options.ZipLen + 2)
                    return true;
                else
                    return false;
            }
            else return false;
        }
        /// <summary>
        /// 是否是压缩数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected bool IsGzipCompressed(byte[] data)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(data))
                {
                    using (GZipStream gzip = new GZipStream(ms, CompressionMode.Decompress))
                    {
                        // 尝试解压数据
                        byte[] buffer = new byte[4];
                        int bytesRead = gzip.Read(buffer, 0, 4);
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 判断是否是zip压缩
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected bool IsZip(RedisValue value)
        {
            if (_options != null)
            {
                byte[] bytes = (byte[])value;
                /*加2是通过在存储数据时前后各加了双引号 占两个字符*/
                if (bytes.Length >= _options.ZipLen + 2 && _options.IsZip)
                {
                    //表示取出来的数据长度大于 指定要压缩的阀值 那么就可尝试解压看数据 是不是被压缩的数据 
                    return IsGzipCompressed(bytes);
                }
                else
                {
                    //数据长度小于 指定的压缩阀值
                    if (_options.IsZip)
                    {
                        //if (_options.IsForceDeZip)
                        //{
                            return IsGzipCompressed(bytes);
                        //}
                        //else
                        //    return false;
                    }
                    else return false;
                }
            }
            else return false;
        }

        /// <summary>
        /// 获取压缩值（如有)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected string GetZipValue(RedisValue value)
        { 
            if(IsZip(value))
                return ZipDeCompress(value);
            else
                return  value.ToString();
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
        public abstract string EvalSha(string shaid,string[] keys, string[] values);

        /// <summary>
        /// 执行redis脚本 并返回bool 状态
        /// 注意:一定要在脚本是自定义返回 true|false
        /// </summary>
        /// <param name="shaid"></param>
        /// <param name="keys"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public abstract bool EvalBoolSha(string shaid, RedisKey[] rediskeys, RedisValue[] redisvalues);


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
        public abstract bool UnLock(params string[] keys);

        /// <summary>
        /// 批量设置key
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="second">过期时间(s) 不设置则为-1</param>
        public abstract void SetCache(IDictionary<string, string> dic, int second=-1);

    }
}
