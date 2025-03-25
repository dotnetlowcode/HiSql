using HiSql;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controller
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TableLogController : ControllerBase
    {
        HiSqlClient hiSqlClient;

        public TableLogController(HiSqlClient _hiSqlClient)
        {
            this.hiSqlClient = _hiSqlClient;
        }


        /// <summary>
        /// ≤Â»Î≤‚ ‘
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> InsertTest()
        {
            var dataList = await this
                .hiSqlClient.Insert(
                    "test",
                    new
                    {
                        Id = "R" + new Random().Next().ToString(),
                        Name = "1111",
                        Desc = "Desc"
                    }
                )
                .ExecCommandAsync();
            return dataList;
        }


        /// <summary>
        /// ∏¸–¬≤‚ ‘
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> UpdateTest()
        {
            var updateResult = await hiSqlClient
                .Update("test", new
                {
                    Desc = "UpdateOnly22"
                })
                .Where(
                    new Filter() { { "Id", OperType.EQ, "KDAD" }, { "Name", OperType.EQ, "1111" } }
                )
                .ExecCommandAsync();
            return updateResult;
        }


        /// <summary>
        /// …æ≥˝≤‚ ‘
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> DeleteTest()
        {
            var deleteResult = await hiSqlClient
                .Delete("test")
                .Where(
                    new Filter()
                    {
                        { "Id", OperType.EQ, "R109198543" },
                        { "Name", OperType.EQ, "1111" }
                    }
                )
                .ExecCommandAsync();
            return deleteResult;
        }

        /// <summary>
        /// Modi≤‚ ‘
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> ModiTest()
        {
            var dynamicObj = new TDynamic();
            dynamicObj.SetProperty("Id", "R1294696794");
            dynamicObj.SetProperty("Name", "1111");
            dynamicObj.SetProperty("Desc", "DynamicValue");
            var modiData = new List<object>()
            {
                new
                {
                    Id = "R1109113159",
                    Name = "1111",
                    Desc = "ModifyNoneEntity"
                },
                new Dictionary<string, object>
                {
                    { "Id", "R1197732017" },
                    { "Name", "1111" },
                    { "Desc", "ModifyObjectValue" }
                },
                new Dictionary<string, string>
                {
                    { "Id", "R1221956109" },
                    { "Name", "1111" },
                    { "Desc", "ModifyStringValue" }
                },
                dynamicObj
            };
            var modiResult = 0;
            foreach (var item in modiData)
            {
                modiResult = await hiSqlClient.Modi("test", [
                item
                ]).ExecCommandAsync();
            }
            return modiResult;
        }

        /// <summary>
        /// ªÿπˆ≤‚ ‘
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<string> Rollback()
        {
            await hiSqlClient.RollbackCredential("test", "20250324191000156");
            return "OK";
        }

    }
}
