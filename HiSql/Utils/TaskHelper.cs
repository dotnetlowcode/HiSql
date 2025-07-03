using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql.Utils
{
    public static class TaskHelper
    {
        /// <summary>
        /// 计算代码执行时间
        /// </summary>
        /// <param name="name"></param>
        /// <param name="fun"></param>
        public static void ExcuteTimer(string name, Action fun)
        {
            #if DEBUG
            Stopwatch sw = new Stopwatch();
            sw.Start();
            #endif
            fun();
            #if DEBUG
            sw.Stop();
            TimeSpan ts2 = sw.Elapsed;
            Console.WriteLine($"程序{name}总共花费{ts2.TotalMilliseconds}ms.");
            #endif
        }

        /// <summary>
        /// 计算代码执行时间
        /// </summary>
        /// <param name="name"></param>
        /// <param name="fun"></param>
        public static async Task<TResult> ExcuteTimerAsync<TResult>(string name, Func<Task<TResult>> fun)
        {
#if DEBUG 
            Stopwatch sw = new Stopwatch();
            sw.Start();
#endif
            TResult excuteResult = await fun();
#if DEBUG
            sw.Stop();
            TimeSpan ts2 = sw.Elapsed;
            Console.WriteLine($"程序{name}总共花费{ts2.TotalMilliseconds}ms.");
#endif
            return excuteResult;
        }
    }
}
