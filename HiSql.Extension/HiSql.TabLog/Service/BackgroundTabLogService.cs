using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HiSql.TabLog.Ext;
using HiSql.TabLog.Interface;
using HiSql.TabLog.Model;
using HiSql.TabLog.Module;
using HiSql.TabLog.Queue;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace HiSql.TabLog.Service
{
    public class BackgroundTabLogService : BackgroundService
    {
        private readonly HiSqlTabLogQueue logQueue;

        public BackgroundTabLogService(HiSqlTabLogQueue _logQueue)
        {
            logQueue = _logQueue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await MainTask();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("HiSql操作日志存储异常");
                    Console.WriteLine(ex);
                }
                await Task.Delay(1000, stoppingToken); // 每隔 1 秒检查一次队列
            }
        }

        public async Task MainTask()
        {
            var credentialList = logQueue.DequeueLog();
            if (credentialList.Count > 0)
            {
                //按数据库分组
                var groupByDb = credentialList.GroupBy(r => r.State.DbServer);
                foreach (var group in groupByDb)
                {
                    //按表分组
                    var groupByTable = group.GroupBy(r => r.State.TabName);
                    foreach (var tableGroup in groupByTable) { }
                    var mainLogs = new Dictionary<string, List<Th_MainLog>>();
                    var detailLogs = new Dictionary<string, List<Th_DetailLog>>();
                    foreach (var credential in group)
                    {
                        var result = BuildCredentialLogs(credential);
                        var mainLogTableName = credential.State.MainTabLog;
                        var detailLogTableName = credential.State.DetailTabLog;
                        if (result != null)
                        {
                            var mainLog = result.Item1;
                            var detailLog = result.Item2;
                            if (!mainLogs.ContainsKey(mainLogTableName))
                                mainLogs[mainLogTableName] = new List<Th_MainLog>() { mainLog };
                            else
                                mainLogs[mainLogTableName].Add(mainLog);

                            if (!detailLogs.ContainsKey(detailLogTableName))
                                detailLogs[detailLogTableName] = new List<Th_DetailLog>(detailLog);
                            else
                                detailLogs[detailLogTableName].AddRange(detailLog);
                        }
                    }

                    //按数据库连接,按表分组插入日志
                    using (var scope = InstallTableLog.FuncGetScope())
                    {
                        var client = InstallTableLog.GetSqlClientByName(scope, group.Key);
                        client.BeginTran();
                        foreach (var tableGroup in mainLogs)
                            await client
                                .Insert(tableGroup.Key, tableGroup.Value)
                                .ExecCommandAsync();

                        foreach (var tableGroup in detailLogs)
                            await client
                                .Insert(tableGroup.Key, tableGroup.Value)
                                .ExecCommandAsync();

                        client.CommitTran();
                    }
                }
            }
        }

        public Tuple<Th_MainLog, List<Th_DetailLog>> BuildCredentialLogs(HiSqlCredential credential)
        {
            var settingObj = credential.State;
            var credentialId = SnroNumber.NewNumber(settingObj.SNRO, settingObj.SNUM);
            credential.CredentialId = credentialId;
            var operateUserName = credential.OperateUserName;
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
            var tableName = settingObj.TabName;
            var mainLog = new Th_MainLog
            {
                LogId = credential.CredentialId,
                TabName = tableName,
                CCount = insertNewValue.Count,
                MCount = updateNewValue.Count,
                DCount = deleteOldValue.Count,
                CreateTime = credential.CreateTime,
                ModiTime = credential.CreateTime,
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
                    TabName = tableName,
                    ActionModel = "C",
                    CreateName = operateUserName,
                    ModiName = operateUserName,
                    CreateTime = credential.CreateTime,
                    ModiTime = credential.CreateTime,
                    NewVal = JsonConvert.SerializeObject(insertNewValue)
                };
                dbLogs.Add(insertLog);
            }

            if (mainLog.MCount > 0)
            {
                var updateLog = new Th_DetailLog
                {
                    LogId = credential.CredentialId,
                    TabName = tableName,
                    ActionModel = "M",
                    CreateName = operateUserName,
                    ModiName = operateUserName,
                    CreateTime = credential.CreateTime,
                    ModiTime = credential.CreateTime,
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
                    TabName = tableName,
                    ActionModel = "D",
                    CreateName = operateUserName,
                    ModiName = operateUserName,
                    CreateTime = DateTime.Now,
                    ModiTime = DateTime.Now,
                    OldVal = JsonConvert.SerializeObject(deleteOldValue)
                };
                dbLogs.Add(deleteLog);
            }
            if (dbLogs.Count == 0)
            {
                return null;
            }
            return new Tuple<Th_MainLog, List<Th_DetailLog>>(mainLog, dbLogs);
            // await sqlClient.Insert(hi_TabManager.MainTabLog, mainLog).ExecCommandAsync();
            // await sqlClient.Insert(hi_TabManager.DetailTabLog, dbLogs).ExecCommandAsync();
        }
    }
}
