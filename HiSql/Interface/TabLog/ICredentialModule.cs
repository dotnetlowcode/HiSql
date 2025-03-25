using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HiSql.Common.Entities.TabLog;

namespace HiSql.Interface.TabLog
{
    public abstract class ICredentialModule
    {
        /// <summary>
        /// 生成凭证ID
        /// </summary>
        /// <returns></returns>
        protected abstract Task<Credential> InitCredential();

        /// <summary>
        /// 开始操作包，返回凭证ID
        /// </summary>
        /// <returns></returns>
        public async Task Execute(
            Func<string, Task<Tuple<List<OperationLog>, object>>> operationFunc,
            string tableName,
            string operateUserName
        )
        {
            var result = await operationFunc(tableName);
            _ = Task.Run(async () =>
            {
                try
                {
                    await SaveCredential(result.Item1, result.Item2, operateUserName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
        }

        /// <summary>
        /// 保存操作包
        /// </summary>
        /// <param name="logs"></param>
        /// <param name="state"></param>
        /// <param name="operateUserName"></param>
        /// <param name="refCredentialId"></param>
        /// <returns></returns>
        protected async Task<Credential> SaveCredential(
            List<OperationLog> logs,
            object state,
            string operateUserName,
            string refCredentialId = ""
        )
        {
            var credential = await InitCredential();

            credential.State = state;
            credential.OperateUserName = operateUserName;
            credential.CreateTime = DateTime.Now;
            credential.OperationLogs = logs;
            credential.RefCredentialId = refCredentialId;
            await SaveCredential(credential);
            return credential;
        }

        /// <summary>
        /// 保存操作包
        /// </summary>
        /// <param name="credential"></param>
        /// <returns></returns>
        protected abstract Task SaveCredential(Credential credential);

        /// <summary>
        /// 根据凭证Id,回滚数据
        /// </summary>
        /// <param name="credentialId"></param>
        /// <param name="state"></param>
        /// <param name="operateUserName"></param>
        /// <returns></returns>
        public abstract Task RollbackCredential(
            HiSqlClient sqlClient,
            string tableName,
            string credentialId
        );

        /// <summary>
        /// 应用数据操作
        /// </summary>
        /// <param name="mainClient"></param>
        /// <param name="modifyRows"></param>
        /// <param name="delRows"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public abstract Task<List<OperationLog>> ApplyDataOperate(
            HiSqlClient mainClient,
            List<Dictionary<string, object>> modifyRows,
            List<Dictionary<string, string>> delRows,
            string tableName,
            List<OperationType> operationTypes
        );

        public abstract Task RecordLog(
            HiSqlProvider sqlProvider,
            string tableName,
            List<Dictionary<string, object>> data,
            List<Dictionary<string, string>> delete,
            Func<Task<bool>> func,
            List<OperationType> operationTypes
        );
    }
}
