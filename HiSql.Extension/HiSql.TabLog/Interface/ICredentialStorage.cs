using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HiSql.AST;

namespace HiSql.TabLog.Interface
{
    public abstract class ICredential<T>
        where T : IOperationLog
    {
        public string CredentialId { get; set; }
        public List<T> OperationLogs { get; set; }
    }

    public interface ICredentialStorage<T, LogT>
        where T : ICredential<LogT>
        where LogT : IOperationLog
    {
        Task SaveCredential(T credential);
    }
}
