﻿using System;

namespace HiSql.PostGreSqlUnitTest
{
    class Program
    {
        static void Main(string[] args)
        {
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
