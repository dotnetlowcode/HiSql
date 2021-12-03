using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{

    public class Demo_Insert
    {
        class H_Test
        {
            public int DID
            {
                get; set;
            }
            public int Hid
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
            //Demo1_Insert(sqlClient);
            //Demo1_Insert2(sqlClient);
            //Demo1_Insert3(sqlClient);
            Demo1_Insert4(sqlClient);
            //Demo_dynamic(sqlClient);
        }

        static void Demo_dynamic(HiSqlClient sqlClient)
        {
            List<object> lstdyn = new List<object>();
            for (int i = 0; i < 10; i++)
            {
                TDynamic dyn1 = new TDynamic();
                dyn1["Hid"] = 150 + i;
                dyn1["UserName"] = $"tgm{i}";
                dyn1["UserAge"] = 34;
                dyn1["Birth"] = DateTime.Now;
                //lstdyn.Add(dyn1);
            }
            TDynamic dyn = new TDynamic();
            dyn["Hid"] = 123;
            dyn["UserName"] = "tgm";
            dyn["UserAge"] = 34;
            dyn["Birth"] = DateTime.Now;

            dynamic ddyn = dyn.ToDynamic();

            ddyn.Userid = "";

            var type = ddyn.GetType();
            var prop = type.GetProperties();


            string hid = ddyn.Birth.ToString("yyyy-MM-dd HH:mm:ss.fff");

            ddyn.Hid = 99;



            Dictionary<string, object> exo = (Dictionary<string, object>)dyn;
            for (int i = 0; i < 10; i++)
            {
                lstdyn.Add(new Dictionary<string, string>
                {
                    { "Hid",$"20{i}"},
                    { "UserName","tgm"},
                    { "UserAge","36"}
                });
            }



            lstdyn.Add(dyn);

            DateTime dnow = DateTime.Now;
            Console.WriteLine(dnow.ToString());




            string _sql = sqlClient.Modi("H_Test", lstdyn).ToSql(); ;
        }

        static void Demo1_Insert4(HiSqlClient sqlClient)
        {
            ///测试 非自增长且不允许为空且没有设置默认值且没有传值 
            string _sql2 = sqlClient.Insert("H_Test", new Dictionary<string, object> { { "UserAge",34 },{ "Hid",5798 } }).ToSql();
        }


        static void Demo1_Insert3(HiSqlClient sqlClient)
        {
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();

            Dictionary<string, string> _dic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { "Hid", "2" }, { "UserName", "QXW" }, { "UserAge", "100" }, { "ReName", "xw" } };
            list.Add(_dic);

            Dictionary<string, string> _dic1 = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { "Hid", "3" }, { "UserName", "QXW1" }, { "UserAge", "101" }, { "ReName", "xw1" } };
            list.Add(_dic1);


            sqlClient.Update<Dictionary<string, string>>("H_Test", list).ExecCommand();
            sqlClient.Modi<Dictionary<string, string>>("H_Test", list).ExecCommand();
            sqlClient.Modi<Dictionary<string, string>>("H_Test", list).ExecCommand();
        }
        static void Demo1_Insert(HiSqlClient sqlClient)
        {
            //Dictionary<string, string> _dic_data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { {"uname","tansar"},{ "Uname2","用户123"} };
            string sql4= sqlClient.Modi("H_Test", new { Hid = 1, UserName = "tansar", UserAge = 100, ReName = "Tom" }).ToSql();

            //string _sql=sqlClient
            //    .Insert("H_TEST", new { UNAME = "UTYP10", UNAME2 = "用户类10" } )
            //    .Insert("Hone_Test",new { Username ="TOM5", Scount =100})
            //    .ToSql();

            //int _effect=sqlClient
            //    .Insert("H_TEST", new { UNAME = "UTYPE9", UNAME2 = "用户类型9" })
            //    .Insert("Hone_Test", new { Username = "TOM4", Scount = 99 }).ExecCommand();

            string _sql2 = sqlClient.Insert<H_Test>(new H_Test { DID = 123, UNAME="UTYPEHA" ,UNAME2="TEST'" }).ToSql();

            string _sql3= sqlClient.Modi("Hi_Domain", new List<object> { new { Domain = "10097", DomainDesc = "用户类型10097" }, new { Domain = "10098", DomainDesc = "用户类型10098" } }).ToSql();
            int _effect3 = sqlClient.Modi("Hi_Domain", new List<object> { new { Domain = "10097", DomainDesc = "用户类型10097" }, new { Domain = "10098", DomainDesc = "用户类型10098" } }).ExecCommand();



        }

        static void Demo1_Insert2(HiSqlClient sqlClient)
        {

            int _times = 100000;
            Console.WriteLine($"[{_times}]条数据 HANA插入测试");
            List<object> lstobj = new List<object>();
            for (int i = 0; i < _times; i++)
            {
                lstobj.Add(new { Domain = $"U{i.ToString()}", DomainDesc = $"用户{i.ToString()}" });
            }

            string _sql = sqlClient
            .Insert("Hi_Domain", new { Domain = "UTYPE", DomainDesc = "用户类型" })
            .Insert("Hi_Domain", new { Domain = "UTYPE2", DomainDesc = "用户类型2" }).ToSql()
           ;
            //int _effect=sqlClient
            //.Insert("Hi_Domain", new { Domain = "UTYPE", DomainDesc = "用户类型" })
            //.Insert("Hi_Domain", new { Domain = "UTYPE2", DomainDesc = "用户类型2" })
            //.ExecCommand();

            Console.WriteLine($"[{_times}]条数据 预热完成正在插入");

            Stopwatch watch = Stopwatch.StartNew();
            int _effect2 = sqlClient
            .Insert("Hi_Domain", lstobj)
            .ExecCommand();


            //string _sql2 = sqlClient
            //.Insert("Hi_Domain", lstobj)
            //.ToSql();

            watch.Stop();
            Console.WriteLine($"[{_times}]条数据,耗时：{watch.Elapsed.ToString()}");

            //int _effect=sqlClient
            //    .Insert("H_TEST", new { UNAME = "UTYPE9", UNAME2 = "用户类型9" })
            //    .Insert("Hone_Test", new { Username = "TOM4", Scount = 99 }).ExecCommand();

            //string _sql2 = sqlClient.Insert<H_Test>(new H_Test { UNAME = "UTYPEHA", UNAME2 = "TEST'" }).ToSql();

            //string _sql3 = sqlClient.Modi("Hi_Domain", new List<object> { new { Domain = "10097", DomainDesc = "用户类型10097" }, new { Domain = "10098", DomainDesc = "用户类型10098" } }).ToSql();
            //int _effect3 = sqlClient.Modi("Hi_Domain", new List<object> { new { Domain = "10097", DomainDesc = "用户类型10097" }, new { Domain = "10098", DomainDesc = "用户类型10098" } }).ExecCommand();



        }
    }
}
