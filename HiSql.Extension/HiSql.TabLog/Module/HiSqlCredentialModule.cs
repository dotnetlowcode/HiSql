using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using HiSql.Common.Entities.TabLog;
using HiSql.Interface.TabLog;
using HiSql.TabLog.Ext;
using HiSql.TabLog.Interface;
using HiSql.TabLog.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HiSql.TabLog.Module
{
    public class HiSqlCredentialModule : ICredentialModule
    {
        public HiSqlCredentialModule() { }

        protected override Task<Credential> InitCredential()
        {
            var credential = new Credential();
            return Task.FromResult(credential);
        }

        /// <summary>
        /// 应用数据操作
        /// </summary>
        /// <param name="mainClient"></param>
        /// <param name="tableName"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public override async Task<List<OperationLog>> ApplyDataOperate(
            HiSqlClient mainClient,
            List<Dictionary<string, object>> modifyRows,
            List<Dictionary<string, string>> delRows,
            string tableName,
            List<OperationType> operationTypes,
            StatisticsCallback statisticsCallback = null
        )
        {
            var operateLogs = new List<OperationLog>();
            var tableInfo = mainClient.DbFirst.GetTabStruct(tableName);
            var primaryJsonList = JArray.FromObject(
                tableInfo.Columns.Where(r => r.IsPrimary).ToList()
            );
            var randm = new Random().Next(100000, 999999);
            var tempTableName = "#" + tableName + "_" + randm;
            var primaryList = new List<HiColumn>();
            //如果主键列少于1个,则抛出异常
            if (primaryJsonList.Count < 1)
                throw new Exception("表没有主键!");
            foreach (var primary in primaryJsonList)
            {
                var column = primary.ToObject<HiColumn>();
                column.TabName = tempTableName;
                primaryList.Add(column);
            }

            var delLog = new OperationLog
            {
                NewValue = new List<IDictionary<string, object>>(0),
                OldValue = new List<IDictionary<string, object>>(),
                OperationType = OperationType.Delete,
                TableName = tableName,
            };
            var addLog = new OperationLog
            {
                NewValue = new List<IDictionary<string, object>>(0),
                OldValue = new List<IDictionary<string, object>>(),
                OperationType = OperationType.Insert,
                TableName = tableName,
            };
            var updateLog = new OperationLog
            {
                NewValue = new List<IDictionary<string, object>>(0),
                OldValue = new List<IDictionary<string, object>>(),
                OperationType = OperationType.Update,
                TableName = tableName,
            };
            // 如果operationTypes里包含删除和更新操作则需要创建临时表
            if (
                operationTypes.Contains(OperationType.Update)
                || operationTypes.Contains(OperationType.Delete)
            )
            {
                var tempTableDataList = new List<IDictionary<string, object>>();
                foreach (var row in modifyRows)
                {
                    var tempRow = new Dictionary<string, object>();
                    foreach (var primary in primaryList)
                        tempRow[primary.FieldName] = row[primary.FieldName];
                    tempTableDataList.Add(tempRow);
                }
                foreach (var row in delRows)
                {
                    var tempRow = new Dictionary<string, object>();
                    foreach (var primary in primaryList)
                        tempRow[primary.FieldName] = row[primary.FieldName];
                    tempTableDataList.Add(tempRow);
                }
                var oldDataMap = new Dictionary<string, IDictionary<string, object>>();
                List<ExpandoObject> updateOldList;
                if (tempTableDataList.Count == 1 || primaryList.Count == 1)
                {
                    //条件只有一条或者主键只有一列都不创建临时表
                    Filter oldDataFilter;
                    if (primaryJsonList.Count == 1)
                    {
                        var pkFieldName = primaryList[0].FieldName;
                        var whereValues = tempTableDataList.Select(r => r[pkFieldName]).ToList();
                        oldDataFilter = new Filter() { { pkFieldName, OperType.IN, whereValues } };
                    }
                    else
                    {
                        oldDataFilter = new Filter();
                        var onlyData = tempTableDataList[0];
                        foreach (var fieldName in onlyData.Keys)
                        {
                            oldDataFilter.Add(fieldName, OperType.EQ, onlyData[fieldName]);
                        }
                    }
                    //计时
                    //var watch = Stopwatch.StartNew();
                    updateOldList = await mainClient
                        .Query(tableName)
                        .Field("*")
                        .Where(oldDataFilter)
                        .ToEObjectAsync();
                    //watch.Stop();
                    //Console.WriteLine($"记录老数据查询耗时：{watch.ElapsedMilliseconds}ms");
                }
                else
                {
                    var tempTableInfo = new TabInfo
                    {
                        Columns = primaryList,
                        TabModel = new HiTable { TabName = tempTableName }
                    };
                    //创建临时表
                    var createResult = mainClient.DbFirst.CreateTable(tempTableInfo);
                    //将新增行和修改行插入临时表
                    var insertResult = mainClient
                        .Insert(tempTableName, tempTableDataList)
                        .ExecCommand();

                    //查询出新增行和修改行
                    //var queryR = mainClient.Query(tempTableName).As("t1").Field("*").ToTable();

                    var fields = new List<string>();
                    foreach (var field in tableInfo.Columns)
                        fields.Add("t2." + field.FieldName);

                    var query = mainClient
                        .Query(tempTableName)
                        .As("t1")
                        .Field(fields.ToArray())
                        .Join(tableName, JoinType.Left)
                        .As("t2");
                    //string joinStr = "";
                    //循环主键列生成连接条件
                    var obj = new JoinOn();
                    foreach (var primary in primaryList)
                        obj.Add("t1." + primary.FieldName, "t2." + primary.FieldName);

                    query = query.On(obj);
                    updateOldList = query.ToEObject();
                    //删除临时表
                    mainClient.DbFirst.DropTable(tempTableName);
                }
                if (updateOldList.Count > 0)
                {
                    foreach (var item in updateOldList)
                    {
                        var row = item as IDictionary<string, object>;
                        var key = "";
                        foreach (var col in primaryList)
                        {
                            var keyValue = row[col.FieldName];
                            if (keyValue == null)
                                break;

                            key += keyValue.ToString();
                        }
                        //如果是空数据跳出循环
                        if (key == "" && updateOldList.Count == 1)
                            break;
                        oldDataMap[key] = row;
                    }
                }

                foreach (var row in delRows)
                {
                    var key = "";
                    foreach (var col in primaryList)
                        key += row[col.FieldName].ToString();
                    if (oldDataMap.TryGetValue(key, out var rowValue))
                    {
                        //删除行
                        delLog.OldValue.Add(rowValue);
                        //删除字典中的键值对
                        oldDataMap.Remove(key);
                    }
                }
                //找出新增行和修改行
                foreach (var row in modifyRows)
                {
                    var key = "";
                    foreach (var col in primaryList)
                        key += row[col.FieldName].ToString();
                    if (oldDataMap.TryGetValue(key, out var rowValue))
                    {
                        //修改行
                        updateLog.OldValue.Add(rowValue);
                        updateLog.NewValue.Add(row);
                        //删除字典中的键值对
                        oldDataMap.Remove(key);
                    }
                    else
                    {
                        //新增行
                        addLog.NewValue.Add(row);
                    }
                }
            }
            else
            {
                addLog.NewValue.AddRange(modifyRows);
            }
            if (delLog.OldValue.Count > 0)
                operateLogs.Add(delLog);
            if (addLog.NewValue.Count > 0)
                operateLogs.Add(addLog);
            if (updateLog.OldValue.Count > 0)
                operateLogs.Add(updateLog);
            if (statisticsCallback != null)
                statisticsCallback(
                    addLog.NewValue.Count,
                    delLog.OldValue.Count,
                    updateLog.OldValue.Count
                );
            return operateLogs;
        }

        protected override Task SaveCredential(Credential credential)
        {
            var state = (Hi_TabManager)credential.State;
            credential.CredentialId = SnroNumber.NewNumber(state.SNRO, state.SNUM);
            TabLogQueue.EnqueueLog(credential);
            return Task.CompletedTask;
        }

        public static Hi_TabManager GetTableLogSetting(string tableName, HiSqlClient client)
        {
            var cacheKey = "HiSqlOperateAndLog:" + tableName;
            var tableLogSetting = CacheContext.Cache.GetCache<Hi_TabManager>(cacheKey);
            if (tableLogSetting != null)
                return tableLogSetting;
            if (!client.DbFirst.CheckTabExists("Hi_TabManager"))
            {
                client.DbFirst.CreateTable(typeof(Hi_TabManager));
                return null;
            }
            var managerTabName = typeof(Hi_TabManager).Name;
            var settingList = client
                .Query(managerTabName)
                .Field("*")
                .Where(new Filter { { "TabName", OperType.EQ, tableName } })
                .ToList<Hi_TabManager>();
            if (settingList == null || settingList.Count == 0)
                return null;
            var setting = settingList.First();
            //初始化自增
            InstallTableLog.InitTableLog(setting);
            if (setting != null)
                CacheContext.Cache.SetCache(cacheKey, setting);
            return setting;
        }

        public delegate IQuery QueryWhereBuilder(IQuery query, Hi_TabManager setting);

        /// <summary>
        /// 获取表日志数据详情
        /// </summary>
        /// <param name="state"></param>
        /// <param name="queryWhereBuilder"></param>
        /// <returns></returns>
        public static async Task<Tuple<List<Hi_DetailLog>, int>> GetTableLogDetail(
            HiSqlClient hiSqlClient,
            string tableName,
            string certId,
            QueryWhereBuilder queryWhereBuilder
        )
        {

            var tabManagerObj = GetTableLogSetting(tableName, hiSqlClient);
            if (InstallTableLog.GetSqlClientByName == null)
                throw new Exception("未初始化函数(AddTabLogServer),表又开启了日志功能");
            using (var queryClient = InstallTableLog.GetSqlClientByName(tabManagerObj.DbServer))
            {
                var mainLog = hiSqlClient
                    .Query(tabManagerObj.MainTabLog)
                    .Field("*")
                    .Where(new Filter { { "LogId", OperType.EQ, certId } })
                    .ToList<Hi_MainLog>()
                    .FirstOrDefault();
                IQuery query = queryClient.Query(mainLog.DetailTabLog).Field("*");
                query = queryWhereBuilder(query, tabManagerObj);
                var totalCount = 0;
                var list = query.ToList<Hi_DetailLog>(ref totalCount);
                return Tuple.Create(list, totalCount);
            }
        }

        /// <summary>
        /// 获取日志主表数据
        /// </summary>
        /// <param name="state"></param>
        /// <param name="queryWhereBuilder"></param>
        /// <returns></returns>
        public static async Task<Tuple<List<Hi_MainLog>, int>> GetTableMainLogs(
            HiSqlClient hiSqlClient,
            string tableName,
            QueryWhereBuilder queryWhereBuilder
        )
        {
            var state = GetTableLogSetting(tableName, hiSqlClient);
            if (InstallTableLog.GetSqlClientByName == null)
                throw new Exception("未初始化函数(AddTabLogServer),表又开启了日志功能");
            using (var queryClient = InstallTableLog.GetSqlClientByName(state.DbServer))
            {
                IQuery query = queryClient
                    .Query(state.MainTabLog)
                    .Where(new Filter { { "TabName", OperType.EQ, tableName } });
                query = queryWhereBuilder(query, state);
                var totalCount = 0;
                var list = query.ToList<Hi_MainLog>(ref totalCount);
                return Tuple.Create(list, totalCount);
            }
        }

        /// <summary>
        /// 连表日志数据
        /// </summary>
        /// <param name="state"></param>
        /// <param name="queryWhereBuilder"></param>
        /// <returns></returns>
        public static async Task<Tuple<List<ExpandoObject>, int>> GetTableDetailLogs(
            HiSqlClient hiSqlClient,
            string tableName,
            string detailLogTableName,
            QueryWhereBuilder queryWhereBuilder
        )
        {
            var tabManagerObj = GetTableLogSetting(tableName, hiSqlClient);
            if (InstallTableLog.GetSqlClientByName == null)
                throw new Exception("未初始化函数(AddTabLogServer),表又开启了日志功能");
            using (var queryClient = InstallTableLog.GetSqlClientByName(tabManagerObj.DbServer))
            {
                IQuery query = queryClient
                    .Query(detailLogTableName)
                    .As("t1")
                    .Field(
                        "t1.*",
                        "t2.IsRecover",
                        "t2.MCount",
                        "t2.CCount",
                        "t2.DCount",
                        "t2.RefLogId"
                    )
                    .Join(tabManagerObj.MainTabLog, JoinType.Left)
                    .As("t2");
                query = query.On(
                    new JoinOn { { "t1.LogId", "t2.LogId" }, { "t1.TabName", "t2.TabName" } }
                );
                query = queryWhereBuilder(query, tabManagerObj);
                var totalCount = 0;
                var list = await query.ToEObjectAsync(ref totalCount);
                return Tuple.Create(list, totalCount);
            }
        }

        public override async Task<Credential> RecordLog(
            HiSqlProvider sqlProvider,
            string tableName,
            List<Dictionary<string, object>> modiData,
            List<Dictionary<string, string>> delete,
            Func<Task<bool>> func,
            List<OperationType> operationTypes
        )
        {
            Credential credentialObj = null;
            if (modiData.Count == 0 && delete.Count == 0)
            {
                await func();
                return credentialObj;
            }
            //Stopwatch watch;
            using (var sqlClient = sqlProvider.CloneClient())
            {
                var settingObj = GetTableLogSetting(tableName, sqlClient);
                //未设置日志配置或未打开日志功能
                if (settingObj == null || settingObj.IsLog == 0)
                {
                    //输出警告日志
                    Console.WriteLine($"警告：未设置日志配置或未打开日志功能,但HiSql配置中却打开了，表名：{tableName}");
                    await func();
                    return credentialObj;
                }
                int cCount = 0;
                int dCount = 0;
                int mCount = 0;
                credentialObj = await Execute(
                    async (tabName) =>
                    {
                        //获取表日志设置耗时
                        //watch = Stopwatch.StartNew();
                        //watch.Stop();
                        //Console.WriteLine($"获取表日志设置耗时：{watch.ElapsedMilliseconds}ms");
                        //应用数据操作耗时
                        //watch = Stopwatch.StartNew();
                        var operateLogs = await ApplyDataOperate(
                            sqlClient,
                            modiData,
                            delete,
                            tableName,
                            operationTypes,
                            (addCount, deleteCount, updateCount) =>
                            {
                                cCount = addCount;
                                dCount = deleteCount;
                                mCount = updateCount;
                            }
                        );
                        //watch.Stop();
                        //Console.WriteLine($"应用数据操作耗时：{watch.ElapsedMilliseconds}ms");
                        return Tuple.Create(operateLogs, settingObj as object);
                    },
                    tableName,
                    sqlClient.CurrentConnectionConfig.User
                );
                credentialObj.TableName = tableName;
                credentialObj.CCount = cCount;
                credentialObj.DCount = dCount;
                credentialObj.MCount = mCount;
            }
            //执行原方法耗时
            //watch = Stopwatch.StartNew();
            await func();
            //watch.Stop();
            //Console.WriteLine($"执行原方法耗时：{watch.ElapsedMilliseconds}ms");
            return credentialObj;
        }

        public override async Task<List<Credential>> RollbackCredential(
            HiSqlClient sqlClient,
            string tableName,
            string credentialId
        )
        {
            var mangerObj = GetTableLogSetting(tableName, sqlClient);
            List<Hi_DetailLog> operateList;
            if (InstallTableLog.GetSqlClientByName == null)
                throw new Exception("未初始化函数(AddTabLogServer),表又开启了日志功能");
            //回滚操作
            using (var logClient = InstallTableLog.GetSqlClientByName(mangerObj.DbServer))
            {
                var operateCredential = logClient
                    .Query(mangerObj.MainTabLog)
                    .Field("*")
                    .Where(
                        new Filter
                        {
                            { "LogId", OperType.EQ, credentialId },
                            { "TabName", OperType.EQ, mangerObj.TabName }
                        }
                    )
                    .ToList<Hi_MainLog>()
                    .FirstOrDefault();
                if (operateCredential == null)
                    throw new Exception("该凭证不存在!");
                if (operateCredential.IsRecover == 1)
                    throw new Exception("该凭证已被回滚!");

                operateList = logClient
                    .Query(operateCredential.DetailTabLog)
                    .Field("*")
                    .Where(
                        new Filter
                        {
                            { "LogId", OperType.EQ, credentialId },
                            { "TabName", OperType.EQ, mangerObj.TabName }
                        }
                    )
                    .ToList<Hi_DetailLog>();

                var modifyRows = new List<IDictionary<string, object>>();
                var delRows = new List<IDictionary<string, string>>();
                List<OperationType> operationTypes = new List<OperationType>();
                foreach (var item in operateList)
                {
                    switch (item.ActionModel)
                    {
                        case "C":
                            var tempDelRows = JsonConvert.DeserializeObject<
                                List<IDictionary<string, string>>
                            >(item.NewVal);
                            delRows.AddRange(tempDelRows);
                            break;
                        case "M":
                            var tempModifyRows = JsonConvert.DeserializeObject<
                                List<IDictionary<string, object>>
                            >(item.OldVal);
                            modifyRows.AddRange(tempModifyRows);
                            break;
                        case "D":
                            var tempCreateRows = JsonConvert.DeserializeObject<
                                List<IDictionary<string, object>>
                            >(item.OldVal);
                            modifyRows.AddRange(tempCreateRows);
                            break;
                        default:
                            throw new Exception("不支持的操作类型:" + item.ActionModel);
                    }
                }
                List<Credential> credentialList = new List<Credential>(2);
                sqlClient.BeginTran();
                if (modifyRows.Count > 0)
                {
                    await sqlClient
                        .Modi(tableName, modifyRows)
                        .ExecCommandAsync(
                            (credentialObj) =>
                            {
                                if (credentialObj == null)
                                    return;
                                credentialObj.RefCredentialId = credentialId;
                                credentialList.Add(credentialObj);
                            }
                        );
                }
                if (delRows.Count > 0)
                {
                    await sqlClient
                        .Delete(tableName, delRows)
                        .ExecCommandAsync(
                            (credentialObj) =>
                            {
                                if (credentialObj == null)
                                    return;
                                credentialObj.RefCredentialId = credentialId;
                                credentialList.Add(credentialObj);
                            }
                        );
                }
                var upCount = await logClient
                    .Update(mangerObj.MainTabLog)
                    .Set(new { IsRecover = 1 })
                    .Where(
                        new Filter
                        {
                            { "LogId", OperType.EQ, credentialId },
                            { "TabName", OperType.EQ, mangerObj.TabName },
                            { "IsRecover", OperType.EQ, 0 }
                        }
                    )
                    .ExecCommandAsync();
                sqlClient.CommitTran();
                if (upCount < 1)
                    throw new Exception("该凭证已被回滚,或凭证不存在!");
                return credentialList;
            }
        }
    }
}
