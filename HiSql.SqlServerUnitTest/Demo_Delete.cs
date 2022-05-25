using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public class Demo_Delete
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
            Delete_Demo(sqlClient);
            //Delete_Demo2(sqlClient);
            //Drop_Demo(sqlClient);
        }

        static void Drop_Demo(HiSqlClient sqlClient)
        {
            string _sql=sqlClient.Drop("H_Test4").ToSql();
            string _sql2 = sqlClient.TrunCate("H_Test4").ToSql();

            sqlClient.CodeFirst.DropTable("H_Test4");

        }

        static void Delete_Demo2(HiSqlClient sqlClient)
        {
            IDelete delete3 = sqlClient.Delete("H_Test4", new List< object>
            { new { UNAME2  = "user'123" }, new { UNAME2  = "user124" } });
            //int _effect3 = sqlClient.Delete("H_Test", new List&lt;object&gt; { new { UNAME2 = "user123" }, new { UNAME2 = "user124" } }).ExecCommand();
            string _sql3 = delete3.ToSql();
        }

        static void Delete_Demo(HiSqlClient sqlClient)
        {

            Dictionary<string, string> _dic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { "HID", "1" }  };

            IDelete where_del = sqlClient.Delete("H_Test", new { Id = "1", UNAME="tansar" });
            string _where_sql2 = where_del.ToSql();

            IDelete where_delete = sqlClient.Delete("H_Test", new { Hid = 1 }).Where("HID=1");
            string _where_sql = where_delete.ToSql();

            IDelete dic_delete = sqlClient.Delete("H_Test", new { Hid=1});
            string _dicsql = dic_delete.ToSql();



            IDelete delete = sqlClient.Delete("H_Test");
            //int _effect = sqlClient.Delete("H_Test").ExecCommand();
            string _sql = delete.ToSql();

            IDelete delete1 = sqlClient.Delete("H_Test").Where(new Filter { { "DID", OperType.GT, 200 } });
            //int _effect1 = sqlClient.Delete("H_Test").Where(new Filter { { "DID", OperType.GT, 200 } }).ExecCommand();
            string _sql1 = delete1.ToSql();

            IDelete delete2 = sqlClient.Delete("H_Test",new { DID=99});
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
