﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql.PostGreSqlUnitTest
{
    public class Demo_Update
    {

        class H_Test
        {
            public int DID
            {
                get; set;
            }
            public string UNAME
            {
                get; set;
            }
            public string UNAME2
            {
                get; set;
            }

        }



        public static void Init(HiSqlClient sqlClient)
        {
            //Update_Demo(sqlClient);
            Update_Demo4(sqlClient);
        }

        static void Update_Demo4(HiSqlClient sqlClient)
        {
            string _sql = sqlClient.Update("H_Test").Set(new { UNAME = "UTYPE" }).Where("DID=1").ToSql();


        }
        static async Task Update_Demo(HiSqlClient sqlClient)
        {
            IUpdate update = sqlClient.Update("H_Test", new { DID = 1, UNAME = "UTYPE", UNAME2 = "user123" }).Exclude("UNAME");//,

            int _effect = sqlClient.Update("H_Test", new { DID = 1, UNAME = "UTYPE", UNAME2 = "user123" }).Exclude("UNAME").ExecCommand();
            string _sql = update.ToSql();
            Console.WriteLine(_sql);


            IUpdate update1 = sqlClient.Update("H_Test").Set(new { UNAME2 = "TEST" }).Where(new Filter { { "DID", OperType.GT, 8 } });
            int _effect1 = sqlClient.Update("H_Test").Set(new { UNAME2 = "TEST" }).Where(new Filter { { "DID", OperType.GT, 8 } }).ExecCommand();
            string _sql1 = update1.ToSql();
            Console.WriteLine(_sql1);

            IUpdate update2 = sqlClient.Update<H_Test>(new H_Test { DID = 1, UNAME2 = "Haha1" }).Only("UNAME2");
            int _effect2 = sqlClient.Update<H_Test>(new H_Test { DID = 1, UNAME2 = "Haha1" }).Only("UNAME2").ExecCommand();
            string _sql2 = update2.ToSql();
            Console.WriteLine(_sql2);


            IUpdate update3 = sqlClient.Update("H_Test", new List<object> { new { DID = 1, UNAME2 = "user123" }, new { DID = 2, UNAME2 = "user124" } }).Only("UNAME2");
            int _effect3 = sqlClient.Update("H_Test", new List<object> { new { DID = 1, UNAME2 = "user123" }, new { DID = 2, UNAME2 = "user124" } }).Only("UNAME2").ExecCommand();
            string _sql3 = update3.ToSql();
            Console.WriteLine(_sql3);

            //"did > 8"



            if (string.IsNullOrEmpty(_sql2))
            {

            }

        }
    }
}
