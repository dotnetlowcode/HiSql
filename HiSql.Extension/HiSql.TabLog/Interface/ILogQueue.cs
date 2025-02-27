using System.Collections.Concurrent;
using System.Collections.Generic;

namespace HiSql.TabLog.Interface
{
    public abstract class ITabLogQueue<T, T2, TState>
        where T : ICredential<T2, TState>
        where T2 : IOperationLog
    {
        private ConcurrentQueue<T> logsQueue = new ConcurrentQueue<T>();

        public void EnqueueLog(T log)
        {
            logsQueue.Enqueue(log);
        }

        /// <summary>
        /// 批量处理日志
        /// </summary>
        /// <returns></returns>
        public List<T> DequeueLog()
        {
            // 批量处理日志,如果日志数量大于100先批量取100条,没有100条就取所有

            if (logsQueue.Count < 1)
                return new List<T>(0);

            var logs = new List<T>();
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
