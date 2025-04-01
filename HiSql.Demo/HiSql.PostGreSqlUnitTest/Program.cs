using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace HiSql.PostGreSqlUnitTest
{
    class Program
    {
        [Serializable]
        [HiTable(IsEdit = true, TabName = "Test_SysDictData", TabDescript = "字典数据表Test_SysDictData")]
        /// <summary>
        /// 字典数据表
        public class Test_SysDictData
        {
            /// <summary>
            /// 字典主键-后台生成
            /// </summary>
            [HiColumn(FieldDesc = "字典主键-后台生成", IsPrimary = true, IsBllKey = true, IsNull = false, FieldType = HiType.VARCHAR, FieldLen = 50, FieldDec = 0, SortNum = 21, DBDefault = HiTypeDBDefault.EMPTY)]
            public string DictCode { get; set; }

            /// <summary>
            /// 数据标签
            /// </summary>
            [HiColumn(FieldDesc = "数据标签", IsPrimary = false, IsBllKey = false, IsNull = false, FieldType = HiType.VARCHAR, FieldDec = 0, SortNum = 22, DBDefault = HiTypeDBDefault.EMPTY)]
            public string DictLabel { get; set; }


        }
            static void Main(string[] args)
        {

            HiSqlClient _sqlClient = Demo_Init.GetSqlClient();
          
            //bool isexits = _sqlClient.DbFirst.CheckTabExists(typeof(Test_SysDictData).Name);
            //if (!isexits)
            //{
            //    _sqlClient.DbFirst.CreateTable(typeof(Test_SysDictData));
            //}
            //var t = _sqlClient.DbFirst.GetTabStruct("Test_SysDictData");


            //var LckInfo = new LckInfo() { Key = "test" };

            //var LckInfo2 = new LckInfo() { Key = "test" };
            //var rlt1 =  Lock.LockOnExecute("test", async  () => {
            //    await Task.Run(() =>
            //    {
            //        var rlt2 = Lock.LockOnExecute("test2", async () =>
            //        {

            //           await Task.Run(() =>
            //            {
            //                for (global::System.Int32 i = 0; i < 2; i++)
            //                {
            //                    Console.WriteLine(" == " + i);

            //                    Thread.Sleep(500);
            //                }
            //            });

            //        }, LckInfo);

            //        Thread.Sleep(500);
            //        Console.WriteLine(rlt2.ToString());

            //    });

            //}, LckInfo);


            //Console.WriteLine(rlt1.ToString());


            //var refreshtoken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJwcmltYXJ5c2lkIjoiVTAwMDAxMDAwMSIsInVuaXF1ZV9uYW1lIjoiaGFkbWluIiwiVGhpcmRVc2VySUQiOiIiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3VzZXJkYXRhIjoie1wiVXNlcklkXCI6XCJVMDAwMDEwMDAxXCIsXCJEZXB0SWRcIjpcIjFcIixcIlVzZXJUeXBlXCI6OSxcIlVzZXJOYW1lXCI6XCJoYWRtaW5cIixcIk5pY2tOYW1lXCI6XCJcIixcIkxvZ2luSVBcIjpcIjEyNy4wLjAuMVwiLFwiVGhpcmRVc2VySURcIjpudWxsLFwiSXNBZG1pblwiOnRydWV9IiwibmJmIjoxNzM1ODg0OTA1LCJleHAiOjE3MzYzMTY5MDUsImlhdCI6MTczNTg4NDkwNX0.X1bzV_9BZyBp4Ac6LBSFXCAuK2ulykfI5FGvLoW7o7M";

            //HiSql.Global.RedisOn = true;
            //HiSql.Global.RedisOptions = new RedisOptions
            //{
            //    Host = "192.168.10.141",
            //    Port = 4010,
            //    CacheRegion = "Third",
            //    Database = 4, EnableMultiCache = true,
            //    IsZip = true,
            //    ZipLen = 10
            //};

            //string valueRedis = "";
            //for (int i = 0; i < 100; i++)
            //{
            //    valueRedis += "==网盘瑟吉欧地方点咖啡的啥地方阿萨德阿萨德阿萨德阿萨德阿萨德阿萨德阿萨德阿萨德阿萨德撒阿萨德阿萨德==";
            //}
            //HiSql.RCache rCache = new RCache(HiSql.Global.RedisOptions);
            //var rfsTkKey = "RefreshToken:" + refreshtoken;
            //var rfsTkKey22 = "RefreshToken2:" + refreshtoken;

            //rCache.SetCache("test", valueRedis);

            //var tt = rCache.GetCache("test4");

            //var t222t = rCache.GetOrCreate<string>("test4", () => {
            //    string valueRedis = "";
            //    for (int i = 0; i < 100; i++)
            //    {
            //        valueRedis += "==网盘瑟吉欧地方点咖啡的啥地方阿萨德阿萨德阿萨德阿萨德阿萨德阿萨德阿萨德阿萨德阿萨德撒阿萨德阿萨德==";
            //    }
            //    return valueRedis;
            //});


            //rCache.SetCache(rfsTkKey22, valueRedis);

            //var tt2 = rCache.GetCache(rfsTkKey22);


            //var refreshTokenDB = rCache.GetCache(rfsTkKey);





            //object ttt = new List<object>(){ "1.2" };
            //var t2 = ttt.GetType();
            //var ttt33 = Tool.ConverterObjToList<decimal>(ttt);
            //foreach (var value in ttt33)
            //{
            //   Console.WriteLine(value);
            //}

            //Console.WriteLine("Hello World!");
            HiSqlClient sqlClient = Demo_Init.GetSqlClient();
            Console.WriteLine($"数据库连接Id" + sqlClient.Context.ConnectedId);
            sqlClient.CodeFirst.InstallHisql();
            //Demo_DbCode.Init(sqlClient);
           // DemoCodeFirst.Init(sqlClient);
           
            //Demo_Query.Init(sqlClient);
            //Demo_Insert.Init(sqlClient);
            //Demo_Update.Init(sqlClient);
            //Demo_Delete.Init(sqlClient);

            //Demo_Upgrade.Init(sqlClient);
        }
    }
}
