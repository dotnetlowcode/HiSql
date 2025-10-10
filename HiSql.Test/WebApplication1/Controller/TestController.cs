using System.Diagnostics;
using HiSql;
using HiSql.TabLog.Module;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace WebApplication1.Controller
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TableLogController : ControllerBase
    {
        HiSqlClient hiSqlClient;

        IServiceProvider serviceProvider;

        public TableLogController(HiSqlClient _hiSqlClient, IServiceProvider _serviceProvider)
        {
            this.hiSqlClient = _hiSqlClient;
            serviceProvider = _serviceProvider;
        }

        /// <summary>
        /// 插入测试
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> InsertTest()
        {
            //for (int k = 0; k < 10; k++)
            //{
            //统计执行时间
            Console.WriteLine("A连接ID:" + this.hiSqlClient.Context.ConnectedId);
            //await Task.Run(async () =>
            //      {
            try
            {
                var watch = Stopwatch.StartNew();
                var dataList = new List<object>();

                for (int i = 0; i < 10; i++)
                {
                    dataList.Add(new
                    {
                        Id = "R" + new Random().Next().ToString() + i,
                        Name = "1111",
                        Desc = "Desc"
                    });
                }
                var insertValue = await hiSqlClient.Insert("test", dataList).ExecCommandAsync();
                watch.Stop();
                Console.WriteLine($"执行插入{k} {watch.ElapsedMilliseconds}ms");
            }

            return "OK";
        }

        /// <summary>
        /// 更新测试
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> UpdateTest()
        {
            object creObj = null;
            // var updateResult = hiSqlClient
            //     .Update("test", new { Desc = "UpdateOnly22" })
            //     .Where(
            //         new Filter()
            //         {
            //             { "Id", OperType.EQ, "R1779617504" },
            //             { "Name", OperType.EQ, "1111" }
            //         }
            //     )
            //     .ExecCommand(
            //         (tempCreObj) =>
            //         {
            //             creObj = tempCreObj;
            //         }
            //     );

            // {"UpdateSet":{"MTypeName":"SKU库存合并1","SHKZG":"S","StockFlag":1,"Ctype":"WA","IsHasSource":0,"IsHasOut":1,"SourceFlag":-4,"OutFlag":1,"IsTriggerExtSys":0,"IsWaitExtSys":1,"AutoCreate":0,"IsReversal":1,"PreMType":"","NextMType":"","CancelMType":"H212","IsUpdateStock":1,"IsCostUpdate":0,"IsCrossWorks":0,"IsCrossLocation":0,"IsVirLocation":0,"Pvid":"H211240905","SortNumber":12,"Remark":"SKU库存合并AA","Status":1,"CreateTime":"2024-11-13T11:18:15.186649","CreateName":"ThirdApi","ModiTime":"2025-03-27T16:08:42.353","ModiName":"HiSql"},"WhereJson":{"MTypeCode":"H211"},"HiSqlWhere":"","HiSqlWhereParam":{}}
            var updateResult = hiSqlClient
                .Update("test", new
                {
                    Desc = "UpdateOnly22"
                })
                .Where(
                    new Filter() { { "Id", OperType.EQ, "R1779617504" }, { "Name", OperType.EQ, "1111" } }
                )
                .ExecCommand((tempCreObj) =>
                {
                    creObj = tempCreObj;
                });
            return creObj;
        }

        /// <summary>
        /// 删除测试
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> DeleteTest()
        {
            var deleteResult = await hiSqlClient
                .Delete("test")
                 //.Where(
                 //    new Filter()
                 //    {
                 //        { "Id", OperType.EQ, "R109198543" },
                 //        { "Name", OperType.EQ, "1111" }
                 //    }
                 //)
                 .Where("Id='R1779617504' and Name='1111'")
                .ExecCommandAsync();
            return deleteResult;
        }

        /// <summary>
        /// Modi测试
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> ModiTest()
        {
            //var dynamicObj = new TDynamic();
            //dynamicObj.SetProperty("Id", "R1294696794");
            //dynamicObj.SetProperty("Name", "1111");
            //dynamicObj.SetProperty("Desc", "DynamicValue");
            //var modiData = new List<object>()
            //{
            //    new
            //    {
            //        Id = "R1109113159",
            //        Name = "1111",
            //        Desc = "ModifyNoneEntity"
            //    },
            //    new Dictionary<string, object>
            //    {
            //        { "Id", "R1197732017" },
            //        { "Name", "1111" },
            //        { "Desc", "ModifyObjectValue" }
            //    },
            //    new Dictionary<string, string>
            //    {
            //        { "Id", "R1221956109" },
            //        { "Name", "1111" },
            //        { "Desc", "ModifyStringValue" }
            //    },
            //    dynamicObj
            //};
            //var modiResult = 0;
            //foreach (var item in modiData)
            //{
            //    modiResult = await hiSqlClient.Modi("test", [
            //    item
            //    ]).ExecCommandAsync();
            //}






            var modiData = new List<object>()
            {
                new Dictionary<string, object>
                {
                    { "Id", "R1000009932144" },
                    { "Name", "1111" },
                    { "Desc", "ModifyObjectValue" }
                },
                new Dictionary<string, object>
                {
                    { "Id", "R1197732017" },
                    { "Name", "1111" },
                    { "Desc", "ModifyObjectValue" }
                },
                new Dictionary<string, object>
                {
                    { "Id", "R1221956109" },
                    { "Name", "1111" },
                    { "Desc", "ModifyStringValue" }
                }
            };
            var modiResult = await hiSqlClient.Modi("test", modiData).ExecCommandAsync((cert) =>
            {
                Console.WriteLine(cert.CredentialId);
            });

            return modiResult;
        }

        /// <summary>
        /// 回滚测试
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> Rollback([FromQuery] string credentialId)
        {
            if (string.IsNullOrWhiteSpace(credentialId))
                return "需要参数" + credentialId;
            var creList = await hiSqlClient.RollbackCredential("test", credentialId);
            return creList;
        }

        [HttpGet]
        public async Task<object> GetTableLog() //[FromQuery] string detailTableName, [FromQuery] string tableName
        {
            var tableName = "test";
            var detailTableName = "Th_DetailLog202503";
            return await HiSqlCredentialModule.GetTableDetailLogs(hiSqlClient, tableName, detailTableName, (query, settingObj) =>
               {
                   query = query.Sort(["CreateTime asc"]);
                   return query;
               });
        }

    }
}
