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
