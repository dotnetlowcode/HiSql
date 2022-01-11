using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql.HanaUnitTest
{
    class Demo_Delete
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
            //Delete_Demo(sqlClient);
            Drop_Demo(sqlClient);
        }
        static void Drop_Demo(HiSqlClient sqlClient)
        {
            string _sql = sqlClient.Drop("H_Test4").ToSql();
            string _sql2 = sqlClient.TrunCate("H_Test4").ToSql();
        }
        static void Delete_Demo(HiSqlClient sqlClient)
        {
            IDelete delete = sqlClient.Delete("H_Test");
            //int _effect = sqlClient.Delete("H_Test").ExecCommand();
            string _sql = delete.ToSql();

            IDelete delete1 = sqlClient.Delete("H_Test").Where(new Filter { { "DID", OperType.GT, 200 } });
            //int _effect1 = sqlClient.Delete("H_Test").Where(new Filter { { "DID", OperType.GT, 200 } }).ExecCommand();
            string _sql1 = delete1.ToSql();

            IDelete delete2 = sqlClient.Delete("H_Test", new { DID = 99 });
            //int _effect2 = sqlClient.Delete("H_Test", new { DID = 99 }).ExecCommand();
            string _sql2 = delete2.ToSql();

            IDelete delete3 = sqlClient.Delete("H_Test", new List<object> { new { DID = 99, UNAME2 = "user123" }, new { DID = 100, UNAME2 = "user124" } });
            //int _effect3 = sqlClient.Delete("H_Test", new List<object> { new { DID = 99, UNAME2 = "user123" }, new { DID = 100, UNAME2 = "user124" } }).ExecCommand();
            string _sql3 = delete3.ToSql();

            IDelete delete4 = sqlClient.Delete("H_Test", new List<H_Test> { new H_Test { DID = 99, UNAME2 = "user123" }, new H_Test { DID = 100, UNAME2 = "user124" } });
            //int _effect4 = sqlClient.Delete("H_Test", new List<H_Test> { new H_Test { DID = 99, UNAME2 = "user123" }, new H_Test { DID = 100, UNAME2 = "user124" } }).ExecCommand();
            string _sql4 = delete4.ToSql();

            IDelete delete5 = sqlClient.TrunCate("H_Test");
            //int _effect5 = sqlClient.TrunCate("H_Test").ExecCommand();
            string _sql5 = delete5.ToSql();



        }
    }
}
