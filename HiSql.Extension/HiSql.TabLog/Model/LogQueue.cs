using System.Collections.Concurrent;
using System.Collections.Generic;
using HiSql.Common.Entities.TabLog;
using HiSql.Interface.TabLog;

namespace HiSql.TabLog.Interface
{
    public static class TabLogQueue
    {
        private static ConcurrentQueue<Credential> logsQueue = new ConcurrentQueue<Credential>();

        public static void EnqueueLog(Credential credential)
        {
            logsQueue.Enqueue(credential);
        }

        /// <summary>
        /// 批量处理日志
        /// </summary>
        /// <returns></returns>
        public static List<Credential> DequeueLog()
        {
            // 批量处理日志,如果日志数量大于10000先批量取10000条,没有10000条就取所有

            if (logsQueue.Count < 1)
                return new List<Credential>(0);

            var logs = new List<Credential>();
            while (logsQueue.TryDequeue(out var log))
            {
                logs.Add(log);
                if (logs.Count >= 10000)
                    break;
            }
            return logs;
        }
    }
}
