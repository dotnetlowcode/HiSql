using System.Threading.Tasks;
using HiSql.TabLog.Interface;
using HiSql.TabLog.Model;
using HiSql.TabLog.Queue;

namespace HiSql.TabLog.Module
{
    /// <summary>
    /// HiSql操作日志
    /// </summary>
    public class HiOperateLog : IOperationLog
    {
        /// <summary>
        /// 当前操作记录关联的表名
        /// </summary>
        public string TableName { get; set; }
    }

    /// <summary>
    /// HiSql操作凭证
    /// </summary>
    public class HiSqlCredential : ICredential<HiOperateLog, Hi_TabManager>
    {
        /// <summary>
        /// 操作凭证关联的表名
        /// </summary>
        public string TableName { get; set; }
    }

    public class HiSqlCredentialModule
        : ICredentialModule<HiSqlCredential, HiOperateLog, Hi_TabManager>
    {
        HiSqlTabLogQueue queue;

        public HiSqlCredentialModule(HiSqlTabLogQueue _queue)
        {
            queue = _queue;
        }

        protected override Task<HiSqlCredential> InitCredential()
        {
            var credential = new HiSqlCredential();
            return Task.FromResult(credential);
        }

        protected override Task SaveCredential(HiSqlCredential credential)
        {
            var state = credential.State;
            credential.CredentialId = SnroNumber.NewNumber(state.SNRO, state.SNUM);
            queue.EnqueueLog(credential);
            return Task.CompletedTask;
        }
    }
}
