using System.Collections.Concurrent;
using HiSql.TabLog.Interface;
using HiSql.TabLog.Model;
using HiSql.TabLog.Module;

namespace HiSql.TabLog.Queue
{
    public class HiSqlTabLogQueue : ITabLogQueue<HiSqlCredential, HiOperateLog, Hi_TabManager> { }
}
