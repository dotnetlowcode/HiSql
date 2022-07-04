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


        /// <summary>
        /// 对服务器执行Lua脚本
        /// </summary>
        /// <param name="script">Lua脚本
        ///   local i = 1
        ///        local cnt = 0
        ///                 for i = 1,#ARGV
        ///                do
        /// redis.call('set',KEYS[i],ARGV[i])
        /// cnt = cnt + 1
        /// end
        ///  return cnt
        /// </param>
        /// <param name="keys">键数组</param>
        /// <param name="values">值数组</param>
        /// <returns></returns>
        public string ExecLuaScript(string script, string[] keys = null, string[] values = null);

        /// <summary>
        ///  对服务器执行Lua脚本
        /// </summary>
        /// <param name="script">Lua脚本 
        /// <code>local non_exist = true                
        ///       local r = redis.call('GET',@key)
        ///      non_exist = (non_exist and not r)
        ///        if non_exist then    
        ///         redis.call('set',@key,@value)
        ///            return 1
        ///       else
        ///            return 0
        ///end</code>
        /// </param>
        /// <param name="obj">new {Keys="test", Value="testsss"}</param>
        /// <returns></returns>
        public string ExecLuaScript(string script, object obj = null);

    }
}
