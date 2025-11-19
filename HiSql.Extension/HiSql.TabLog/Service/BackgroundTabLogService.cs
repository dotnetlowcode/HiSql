using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HiSql.Common.Entities.TabLog;
using HiSql.Interface.TabLog;
using HiSql.TabLog.Ext;
using HiSql.TabLog.Interface;
using HiSql.TabLog.Model;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HiSql.TabLog.Service
{
    public class CustomDictionaryConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(IDictionary<string, object>).IsAssignableFrom(objectType);
        }

        static readonly List<string> IgnoreField = new List<string>()
        {
            "CreateTime",
            "CreateName",
            "ModiTime",
            "ModiName"
        };

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            IDictionary<string, object> dictionary = (IDictionary<string, object>)value;
            JObject jObject = new JObject();
            foreach (var kvp in dictionary)
            {
                if (!IgnoreField.Contains(kvp.Key))
                {
                    jObject.Add(kvp.Key, JToken.FromObject(kvp.Value));
                }
            }
            jObject.WriteTo(writer);
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer
        )
        {
            throw new NotImplementedException(); // 如果你不需要反序列化，可以抛出异常或实现为空
        }
    }

    public class BackgroundTabLogService : BackgroundService
    {
        private static readonly CustomDictionaryConverter convertSetting =
            new CustomDictionaryConverter();

        public BackgroundTabLogService(IServiceProvider _serviceProvider)
        {
            //给HiSql.TabLog.Service注入IServiceProvider
            //Instance.SetServiceProvider(_serviceProvider);
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
                await Task.Delay(1, stoppingToken); // 每隔1毫秒检查一次队列
            }
        }

        /// <summary>
        /// 表是否存在
        /// </summary>
        Dictionary<string, bool> tableExists = new Dictionary<string, bool>();

        /// <summary>
        /// 连接缓存
        /// </summary>
        Dictionary<string, HiSqlClient> dbClient = new Dictionary<string, HiSqlClient>();

        public async Task MainTask()
        {
            var credentialList = TabLogQueue.DequeueLog();
            if (credentialList.Count == 0)
            {
                return;
            }
            //按数据库分组
            var groupByDb = credentialList
                .Select(r =>
                {
                    var tableLogConfig = (Hi_TabManager)r.State;
                    return new { Credential = r, tableLogConfig, };
                })
                .GroupBy(r => r.tableLogConfig.DbServer);
            foreach (var group in groupByDb)
            {
                //按表分组
                var groupByTable = group.GroupBy(r => r.tableLogConfig.TabName);
                var mainLogs = new Dictionary<string, List<Hi_MainLog>>();
                var detailLogs = new Dictionary<string, List<Hi_DetailLog>>();
                foreach (var credential in group)
                {
                    var result = BuildCredentialLogs(credential.Credential);
                    var mainLogTableName = credential.tableLogConfig.MainTabLog;
                    var detailLogTableName = credential.tableLogConfig.DetailTabLog;
                    if (credential.tableLogConfig.IsSplitLog == 1)
                        detailLogTableName =
                            detailLogTableName
                            + DateTime.Now.ToString(credential.tableLogConfig.SplitFormat);

                    if (result != null)
                    {
                        var mainLog = result.Item1;
                        mainLog.DetailTabLog = detailLogTableName;
                        var detailLog = result.Item2;
                        if (!mainLogs.ContainsKey(mainLogTableName))
                            mainLogs[mainLogTableName] = new List<Hi_MainLog>() { mainLog };
                        else
                            mainLogs[mainLogTableName].Add(mainLog);

                        if (!detailLogs.ContainsKey(detailLogTableName))
                            detailLogs[detailLogTableName] = new List<Hi_DetailLog>(detailLog);
                        else
                            detailLogs[detailLogTableName].AddRange(detailLog);
                    }
                }
                //var watch = Stopwatch.StartNew();
                HiSqlClient hiSqlClient = null;
                if (!dbClient.ContainsKey(group.Key))
                {
                    if (InstallTableLog.GetSqlClientByName == null)
                        throw new Exception("未初始化函数(AddTabLogServer),表又开启了日志功能");
                    hiSqlClient = InstallTableLog.GetSqlClientByName(group.Key);
                    dbClient.Add(group.Key, hiSqlClient);
                }
                else
                {
                    hiSqlClient = dbClient[group.Key];
                }
                //按数据库连接,按表分组插入日志
                //hiSqlClient.BeginTran();
                foreach (var tableGroup in mainLogs)
                {
                    if (!tableExists.ContainsKey(tableGroup.Key))
                    {
                        InstallTableLog.CreateTableByTemplate<Hi_MainLog>(
                            hiSqlClient,
                            tableGroup.Key
                        );
                        tableExists.Add(tableGroup.Key, true);
                    }
                    await hiSqlClient.Insert(tableGroup.Key, tableGroup.Value).ExecCommandAsync();
                }
                foreach (var tableGroup in detailLogs)
                {
                    //看表是否存在，不存在就创建
                    if (!tableExists.ContainsKey(tableGroup.Key))
                    {
                        InstallTableLog.CreateTableByTemplate<Hi_DetailLog>(
                            hiSqlClient,
                            tableGroup.Key
                        );
                        tableExists.Add(tableGroup.Key, true);
                    }
                    await hiSqlClient.Insert(tableGroup.Key, tableGroup.Value).ExecCommandAsync();
                }
                //hiSqlClient.CommitTran();
                //watch.Stop();
                //Console.WriteLine($"独立日志{mainLogs.Count}条，保存耗时：{watch.ElapsedMilliseconds}ms");
            }
        }

        public Tuple<Hi_MainLog, List<Hi_DetailLog>> BuildCredentialLogs(Credential credential)
        {
            var settingObj = (Hi_TabManager)credential.State;
            var operateUserName = credential.OperateUserName;
            var operateLogs = credential.OperationLogs;
            //分别统计Insert、Update、Delete的操作日志条数
            var insertNewValue = new List<IDictionary<string, object>>();
            var updateNewValue = new List<IDictionary<string, object>>();
            var updateOldValue = new List<IDictionary<string, object>>();
            var deleteOldValue = new List<IDictionary<string, object>>();

            foreach (var log in operateLogs)
            {
                switch (log.OperationType)
                {
                    case OperationType.Insert:
                        insertNewValue = log.NewValue;
                        break;
                    case OperationType.Update:
                        updateNewValue = log.NewValue;
                        updateOldValue = log.OldValue;
                        break;
                    case OperationType.Delete:
                        deleteOldValue = log.OldValue;
                        break;
                }
            }

            var tableName = settingObj.TabName;
            var mainLog = new Hi_MainLog
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
                RefLogId = credential.RefCredentialId,
                IsRecover = 0,
                LogModel = 1
            };
            var dbLogs = new List<Hi_DetailLog>();
            //如果有新增、修改、删除操作，则记录日志
            if (mainLog.CCount > 0)
            {
                var insertLog = new Hi_DetailLog
                {
                    LogId = credential.CredentialId,
                    TabName = tableName,
                    ActionModel = "C",
                    CreateName = operateUserName,
                    ModiName = operateUserName,
                    CreateTime = credential.CreateTime,
                    ModiTime = credential.CreateTime,
                    NewVal = JsonConvert.SerializeObject(insertNewValue, convertSetting)
                };
                dbLogs.Add(insertLog);
            }

            if (mainLog.MCount > 0)
            {
                var updateLog = new Hi_DetailLog
                {
                    LogId = credential.CredentialId,
                    TabName = tableName,
                    ActionModel = "M",
                    CreateName = operateUserName,
                    ModiName = operateUserName,
                    CreateTime = credential.CreateTime,
                    ModiTime = credential.CreateTime,
                    NewVal = JsonConvert.SerializeObject(updateNewValue, convertSetting),
                    OldVal = JsonConvert.SerializeObject(updateOldValue)
                };
                dbLogs.Add(updateLog);
            }
            if (mainLog.DCount > 0)
            {
                var deleteLog = new Hi_DetailLog
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
                return null;
            return new Tuple<Hi_MainLog, List<Hi_DetailLog>>(mainLog, dbLogs);
        }
    }
}
