using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HiSql.Demo_Insert;

namespace HiSql
{
    public class Demo_Delete
    {
        //class H_Test
        //{
        //    public int Hid
        //    {
        //        get; set;
        //    }
        //    public string UNAME
        //    {
        //        get; set;
        //    }
        //    public string UserName
        //    {
        //        get; set;
        //    }

        //}
        public static void Init(HiSqlClient sqlClient)
        {
            // Delete_Demo(sqlClient); //ok
            //Delete_Demo2(sqlClient);//ok
            //Drop_Demo(sqlClient);//ok
        }

        static void Drop_Demo(HiSqlClient sqlClient)
        {
            string _sql=sqlClient.Drop("HTest04").ToSql();
            string _sql2 = sqlClient.TrunCate("HTest04").ToSql();

            sqlClient.CodeFirst.DropTable("HTest04");

        }

        static void Delete_Demo2(HiSqlClient sqlClient)
        {
            IDelete delete3 = sqlClient.Delete("H_Test", new List< object>
            { new H_Test { Hid =2 , UserName  = "user'123" }, new { Hid =2 ,UserName  = "user124" } });
            //int _effect3 = sqlClient.Delete("H_Test", new List&lt;object&gt; { new { UserName = "user123" }, new { UserName = "user124" } }).ExecCommand();
            string _sql3 = delete3.ToSql();
        }

        static void Delete_Demo(HiSqlClient sqlClient)
        {

            Dictionary<string, string> _dic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { "HID", "1" }  };

            IDelete where_del = sqlClient.Delete("H_Test", new H_Test(){ Hid = 1, UserName="tansar" });
            string _where_sql2 = where_del.ToSql();

            //IDelete where_delete = sqlClient.Delete("H_Test", new { Hid = 1 }).Where("HID=1");
            //string _where_sql = where_delete.ToSql();

            IDelete dic_delete = sqlClient.Delete("H_Test", new { Hid=1});
            var  cnt  = dic_delete.ExecCommand();
            string _dicsql = dic_delete.ToSql();



            IDelete delete = sqlClient.Delete("H_Test");
            //int _effect = sqlClient.Delete("H_Test").ExecCommand();
            string _sql = delete.ToSql();

            IDelete delete1 = sqlClient.Delete("H_Test").Where(new Filter { { "Hid", OperType.GT, 200 } });
            //int _effect1 = sqlClient.Delete("H_Test").Where(new Filter { { "Hid", OperType.GT, 200 } }).ExecCommand();
            string _sql1 = delete1.ToSql();

            IDelete delete2 = sqlClient.Delete("H_Test",new { Hid=99});
            //int _effect2 = sqlClient.Delete("H_Test", new { Hid = 99 }).ExecCommand();
            string _sql2 = delete2.ToSql();

            IDelete delete3 = sqlClient.Delete("H_Test", new List<object> { new { Hid = 99, UserName = "user123" }, new { Hid = 100, UserName = "user124" } });
            //int _effect3 = sqlClient.Delete("H_Test", new List<object> { new { Hid = 99, UserName = "user123" }, new { Hid = 100, UserName = "user124" } }).ExecCommand();
            string _sql3 = delete3.ToSql();

            IDelete delete4 = sqlClient.Delete("H_Test", new List<H_Test> { new H_Test { Hid = 99, UserName = "user123" }, new H_Test { Hid = 100, UserName = "user124" } });
            //int _effect4 = sqlClient.Delete("H_Test", new List<H_Test> { new H_Test { Hid = 99, UserName = "user123" }, new H_Test { Hid = 100, UserName = "user124" } }).ExecCommand();
            string _sql4 = delete4.ToSql();

            IDelete delete5 = sqlClient.TrunCate("H_Test");
            //int _effect5 = sqlClient.TrunCate("H_Test").ExecCommand();
            string _sql5 = delete5.ToSql();



        }
    }
}
