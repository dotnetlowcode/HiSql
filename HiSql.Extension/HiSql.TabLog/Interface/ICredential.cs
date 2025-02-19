using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HiSql.AST;
using HiSql.TabLog.Model;
using Microsoft.Extensions.DependencyInjection;

namespace HiSql.TabLog.Interface
{
    public abstract class ICredential<T, TState>
        where T : IOperationLog
    {
        public string CredentialId { get; set; }

        public List<T> OperationLogs { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public string OperateUserName { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 透传数据
        /// </summary>
        public TState State { get; set; }
    }
}
