﻿using System;

namespace HiSql.DaMengUnitTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");

            HiSqlClient sqlClient = Demo_Init.GetSqlClient();
            Console.WriteLine($"数据库连接id" + sqlClient.Context.ConnectedId);
             //Demo_Query.Init(sqlClient);//ok
            //DemoCodeFirst.Init(sqlClient); //ok
            //Demo_Insert.Init(sqlClient);//ok
                                        //
            //Demo_Update.Init(sqlClient);//ok
            //Demo_Delete.Init(sqlClient);
            // Demo_DbCode.Init(sqlClient);//ok

            Demo_Upgrade.Init(sqlClient);

            string s = Console.ReadLine();

            //string _s ="``";
        }
    }
}
