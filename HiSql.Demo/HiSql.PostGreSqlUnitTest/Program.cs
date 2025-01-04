using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace HiSql.PostGreSqlUnitTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var refreshtoken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJwcmltYXJ5c2lkIjoiVTAwMDAxMDAwMSIsInVuaXF1ZV9uYW1lIjoiaGFkbWluIiwiVGhpcmRVc2VySUQiOiIiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3VzZXJkYXRhIjoie1wiVXNlcklkXCI6XCJVMDAwMDEwMDAxXCIsXCJEZXB0SWRcIjpcIjFcIixcIlVzZXJUeXBlXCI6OSxcIlVzZXJOYW1lXCI6XCJoYWRtaW5cIixcIk5pY2tOYW1lXCI6XCJcIixcIkxvZ2luSVBcIjpcIjEyNy4wLjAuMVwiLFwiVGhpcmRVc2VySURcIjpudWxsLFwiSXNBZG1pblwiOnRydWV9IiwibmJmIjoxNzM1ODg0OTA1LCJleHAiOjE3MzYzMTY5MDUsImlhdCI6MTczNTg4NDkwNX0.X1bzV_9BZyBp4Ac6LBSFXCAuK2ulykfI5FGvLoW7o7M";

            HiSql.Global.RedisOn = true;
            HiSql.Global.RedisOptions = new RedisOptions
            {
                Host = "192.168.10.141",
                Port = 4010,
                CacheRegion = "Third",
                Database = 4, EnableMultiCache = true,
                IsZip = true,
                ZipLen = 10
            };

            string valueRedis = "";
            for (int i = 0; i < 100; i++)
            {
                valueRedis += "==网盘瑟吉欧地方点咖啡的啥地方阿萨德阿萨德阿萨德阿萨德阿萨德阿萨德阿萨德阿萨德阿萨德撒阿萨德阿萨德==";
            }
            HiSql.RCache rCache = new RCache(HiSql.Global.RedisOptions);
            var rfsTkKey = "RefreshToken:" + refreshtoken;
            var rfsTkKey22 = "RefreshToken2:" + refreshtoken;

            rCache.SetCache("test", valueRedis);

            var tt = rCache.GetCache("test4");

            var t222t = rCache.GetOrCreate<string>("test4", () => {
                string valueRedis = "";
                for (int i = 0; i < 100; i++)
                {
                    valueRedis += "==网盘瑟吉欧地方点咖啡的啥地方阿萨德阿萨德阿萨德阿萨德阿萨德阿萨德阿萨德阿萨德阿萨德撒阿萨德阿萨德==";
                }
                return valueRedis;
            });


            rCache.SetCache(rfsTkKey22, valueRedis);

            var tt2 = rCache.GetCache(rfsTkKey22);


            var refreshTokenDB = rCache.GetCache(rfsTkKey);





            object ttt = new List<object>(){ "1.2" };
            var t2 = ttt.GetType();
            var ttt33 = Tool.ConverterObjToList<decimal>(ttt);
            foreach (var value in ttt33)
            {
               Console.WriteLine(value);
            }

            //Console.WriteLine("Hello World!");
            HiSqlClient sqlClient = Demo_Init.GetSqlClient();
            sqlClient.CodeFirst.InstallHisql();
            Console.WriteLine($"数据库连接id" + sqlClient.Context.ConnectedId);
            //Demo_DbCode.Init(sqlClient);
            //DemoCodeFirst.Init(sqlClient);
           
            //Demo_Query.Init(sqlClient);
            Demo_Insert.Init(sqlClient);
            //Demo_Update.Init(sqlClient);
            //Demo_Delete.Init(sqlClient);

            //Demo_Upgrade.Init(sqlClient);
        }
    }
}
