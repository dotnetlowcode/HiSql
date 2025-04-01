using HiSql;
using HiSql.TabLog.Module;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net;
using WebApplication1.Helper;

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
        /// �������
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> InsertTest()
        {
            //for (int k = 0; k < 10; k++)
            //{
            //ͳ��ִ��ʱ��
            Console.WriteLine("A����ID:" + this.hiSqlClient.Context.ConnectedId);
            //await Task.Run(async () =>
            //      {
            try
            {
                var watch = Stopwatch.StartNew();
                var dataList = new List<object>();

                for (int i = 0; i < 10; i++)
                {
                    var newClient = hiSqlClient; //HiSqlInit.serviceProvider.GetService<HiSqlClient>(); //hiSqlClient.Context.CloneClient();
                    Console.WriteLine(i + "����ID" + newClient.Context.ConnectedId);

                    await Task.Delay(10);
                    dataList.Add(new
                    {
                        Id = "R" + new Random().Next(int.MinValue, int.MaxValue).ToString() + i,
                        Name = "1111",
                        Desc = "Desc"
                    });
                    var insertValue = await newClient.Modi("test", dataList).ExecCommandAsync();
                    watch.Stop();
                    Console.WriteLine("OK" + DateTime.Now.ToString());
                    //newClient.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            //});

            //Console.WriteLine($"ִ�в���{k} {watch.ElapsedMilliseconds}ms");
            //}

            return "OK";
        }


        /// <summary>
        /// ���²���
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> UpdateTest()
        {
            object creObj = null;
            var updateResult = hiSqlClient
                .Update("HTest02", new
                {
                    Age = 11
                })
                //.Where(
                //    new Filter() { { "Id", OperType.EQ, "R1779617504" }, { "Name", OperType.EQ, "1111" } }
                //)
                .Where("SID=1")
                .ExecCommand();
            return creObj;
        }


        /// <summary>
        /// ɾ������
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
        /// Modi����
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
            var modiResult = await hiSqlClient.Modi("test", modiData).ExecCommandAsync((cert) => { 
                Console.WriteLine(cert.CredentialId);
            });

            return modiResult;
        }

        /// <summary>
        /// �ع�����
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> Rollback([FromQuery] string credentialId)
        {
            if (string.IsNullOrWhiteSpace(credentialId))
                return "��Ҫ����" + credentialId;
            var creList = await hiSqlClient.RollbackCredential("test", credentialId);
            return creList;
        }

        [HttpGet]
        public async Task<object> GetTableLog()//[FromQuery] string detailTableName, [FromQuery] string tableName
        {
            var tableName = "test";
            var detailTableName = "Th_DetailLog202503";
            return await HiSqlCredentialModule.GetTableDetailLogs(hiSqlClient, tableName, detailTableName, (query, settingObj) =>
               {
                   query = query.Sort(["CreateTime asc"]).Skip(1).Take(1);
                   return query;
               });
        }

    }
}
