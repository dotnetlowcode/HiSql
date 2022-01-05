using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// 用于操作的基本事件
    /// </summary>
    public class AopEvent
    {
        /// <summary>
        /// 数据启动连接时,如果启用了加密则通过该事件进行解密
        /// 否则数据库连接将会无效
        /// </summary>
        public Func<string, string> OnDbDecryptEvent { get; set; }

        /// <summary>
        /// 在SQL执行前记录
        /// </summary>
        public Action<string, HiParameter[]> OnLogSqlExecuting { get; set; }


        /// <summary>
        /// 在sql执行后记录
        /// </summary>
        public Action<string, HiParameter[]> OnLogSqlExecuted { get; set; }

        /// <summary>
        /// 当sql执行错误时执行事
        /// </summary>
        public Action<HiSqlException> OnSqlError { get; set; }


        /// <summary>
        /// Sql超时
        /// </summary>
        public Action<int> OnTimeOut { get; set; }


        /// <summary>
        /// 按页分批次执行
        /// </summary>
        public Action<int, int> OnPageExec { get; set; }

        
    }
}
