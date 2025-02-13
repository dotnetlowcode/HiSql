using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HiSql.TabLog.Ext;
using HiSql.TabLog.Interface;
using HiSql.TabLog.Model;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

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
    public class HiSqlCredential : ICredential<HiOperateLog>
    {
        /// <summary>
        /// 操作凭证关联的表名
        /// </summary>
        public string TableName { get; set; }
    }

    public class HiSqlCredentialStorage : ICredentialStorage<HiSqlCredential, HiOperateLog>
    {
        public Hi_TabManager hi_TabManager;

        private IServiceScope scope;

        public HiSqlCredentialStorage(IServiceScope _scope, Hi_TabManager _hi_TabManager)
        {
            this.hi_TabManager = _hi_TabManager;
            this.scope = _scope;
        }

        public async Task SaveCredential(HiSqlCredential credential)
        {
            var sqlClient = InstallTableLog.GetSqlClientByName(scope, this.hi_TabManager.DbServer);
            var operateUserName = sqlClient.Context.CurrentConnectionConfig.User;
            var operateLogs = credential.OperationLogs;
            //分别统计Insert、Update、Delete的操作日志条数
            var insertNewValue = new List<string>();
            var updateNewValue = new List<string>();
            var updateOldValue = new List<string>();
            var deleteOldValue = new List<string>();

            foreach (var log in operateLogs)
            {
                switch (log.OperationType)
                {
                    case OperationType.Insert:
                        insertNewValue.Add(log.NewValue);
                        break;
                    case OperationType.Update:
                        updateNewValue.Add(log.NewValue);
                        updateOldValue.Add(log.OldValue);
                        break;
                    case OperationType.Delete:
                        deleteOldValue.Add(log.OldValue);
                        break;
                }
            }
            var mainLog = new Th_MainLog
            {
                LogId = credential.CredentialId,
                TabName = credential.TableName,
                CCount = insertNewValue.Count,
                MCount = updateNewValue.Count,
                DCount = deleteOldValue.Count,
                CreateTime = DateTime.Now,
                ModiTime = DateTime.Now,
                CreateName = operateUserName,
                ModiName = operateUserName,
                IsRecover = 0,
                LogModel = 1
            };
            var dbLogs = new List<Th_DetailLog>();
            //如果有新增、修改、删除操作，则记录日志
            if (mainLog.CCount > 0)
            {
                var insertLog = new Th_DetailLog
                {
                    LogId = credential.CredentialId,
                    TabName = credential.TableName,
                    ActionModel = "C",
                    CreateName = operateUserName,
                    ModiName = operateUserName,
                    CreateTime = DateTime.Now,
                    ModiTime = DateTime.Now,
                    NewVal = JsonConvert.SerializeObject(insertNewValue)
                };
                dbLogs.Add(insertLog);
            }

            if (mainLog.MCount > 0)
            {
                var updateLog = new Th_DetailLog
                {
                    LogId = credential.CredentialId,
                    TabName = credential.TableName,
                    ActionModel = "M",
                    CreateName = operateUserName,
                    ModiName = operateUserName,
                    CreateTime = DateTime.Now,
                    ModiTime = DateTime.Now,
                    NewVal = JsonConvert.SerializeObject(updateNewValue),
                    OldVal = JsonConvert.SerializeObject(updateOldValue)
                };
                dbLogs.Add(updateLog);
            }

            if (mainLog.DCount > 0)
            {
                var deleteLog = new Th_DetailLog
                {
                    LogId = credential.CredentialId,
                    TabName = credential.TableName,
                    ActionModel = "D",
                    CreateName = operateUserName,
                    ModiName = operateUserName,
                    CreateTime = DateTime.Now,
                    ModiTime = DateTime.Now,
                    OldVal = JsonConvert.SerializeObject(deleteOldValue)
                };
                dbLogs.Add(deleteLog);
            }

            sqlClient.BeginTran();
            await sqlClient.Modi(mainLog).ExecCommandAsync();
            await sqlClient.Modi(hi_TabManager.TabName, dbLogs).ExecCommandAsync();
            sqlClient.CommitTran();
        }
    }
}
