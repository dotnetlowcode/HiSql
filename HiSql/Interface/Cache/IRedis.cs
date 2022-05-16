using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// Redis 操作实现接口
    /// </summary>
    public interface IRedis: ICache
    {

        /// <summary>
        /// 广播方式订阅 不按发布顺序接口
        /// </summary>
        /// <param name="channel">频道</param>
        /// <param name="handler"></param>
        public void BroadCastSubScriber(string channel, Action<string, string> handler = null);

        /// <summary>
        /// 队列方式订阅 按发布顺序接收
        /// </summary>
        /// <param name="channel">频道</param>
        /// <param name="handler"></param>
        public void QueueSubScriber(string channel, Action<string, string> handler = null);


        /// <summary>
        /// 向某一个频道发送消息
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public long PublishMessage(string channel, string message);


        /// <summary>
        /// 入列
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public long ListPush(string channel, string value);

        /// <summary>
        /// 先进先出 出列
        /// 最先入列的最先出列
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public string ListFirstPop(string channel);

        /// <summary>
        /// 后进先出 出列
        /// 最后一个入列最先出列
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public string ListLastPop(string channel);


        /// <summary>
        /// 获取队列的记录数
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public long ListCount(string channel);



      
        public string HGet(string hashkey,string key);

        /// <summary>
        /// hash 删除
        /// </summary>
        /// <param name="hashkey"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool HDel(string hashkey, string key);
        /// <summary>
        /// 加业务锁
        /// </summary>
        /// <param name="key">自定锁的KEY</param>
        /// <param name="expresseconds">锁的周期时间 单位秒</param>
        /// <param name="timeoutseconds">加锁等待超时时间 单位秒</param>
        /// <returns></returns>
        public Tuple<bool,string> LockOn(string key, LckInfo lckinfo,int expresseconds,int timeoutseconds);


        /// <summary>
        /// 检测key是否已经锁定
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Tuple<bool, string> CheckLock(string key);


        /// <summary>
        /// 加业务锁
        /// </summary>
        /// <param name="key">自定锁的KEY</param>
        /// <param name="action">加锁后执行的业务</param>
        /// <param name="expresseconds">锁的周期时间 单位秒</param>
        /// <param name="timeoutseconds">加锁等待超时时间 单位秒</param>
        /// <returns></returns>
        public Tuple<bool, string> LockOnExecute(string key, Action action,LckInfo lckinfo, int expresseconds = 30, int timeoutseconds = 5);



        public bool UnLock(string key);

    }
}
