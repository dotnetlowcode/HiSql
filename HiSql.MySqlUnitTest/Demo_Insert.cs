using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql.MySqlUnitTest
{
    class Demo_Insert
    {
        class H_TEST
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
            public bool ISSYS { get; set; }
        }

        public static void Init(HiSqlClient sqlClient)
        {
            //Demo1_Insert(sqlClient);
            //Demo1_Modi(sqlClient);
            //Demo2_Insert(sqlClient);
            //Demo1_Modi2(sqlClient);
            //Demo3_Insert(sqlClient);
            Demo4_Insert1(sqlClient);
            string s = Console.ReadLine();
        }

        static void Demo4_Insert1(HiSqlClient sqlClient)
        {
            TabInfo tabinfo = sqlClient.Context.DMInitalize.GetTabStruct("HTest01");

            List<Dictionary<string, object>> lstdata = new List<Dictionary<string, object>>();
            int _count = 1000000;
            Random random = new Random();
            for (int i = 0; i < _count; i++)
            {
                lstdata.Add(new Dictionary<string, object> { { "SID", (i + 1) }, { "UName", $"tansar{i}" }, { "Age", 20 + (i % 50) }, { "Salary", 5000 + (i % 2000) + random.Next(10) }, { "descript", "hello world" } });


            }

            sqlClient.CodeFirst.Truncate("HTest01");
            //string _josn = DataConvert.ToCSV(lstdata, tabinfo, DBType.MySql, true, "tansar");

            
            Stopwatch sw = new Stopwatch();
            sw.Start();
           
            
            int _effect = sqlClient.BulkCopyExecCommand("HTest01", lstdata); 

            sw.Stop();
            Console.WriteLine($"写入{_effect}条 耗时{sw.Elapsed}秒");
            var s = Console.ReadLine();
        }
        static async void Demo1_Modi2(HiSqlClient sqlClient)
        {
            int _times = 100;
            Console.WriteLine($"[{_times}]条数据 mysql插入测试");
            List<H_TEST> lstobj = new List<H_TEST>();
            for (int i = 0; i < _times; i++)
            {
                lstobj.Add(new H_TEST { UNAME = $"U{i}", UNAME2 = $"2U{i}" });
            }
            Stopwatch watch = Stopwatch.StartNew();
            //string _sql = sqlClient.Modi("Hi_Domain", lstobj).ToSql();
            int _effect =  sqlClient.Modi("H_TEST", lstobj).ExecCommand();

            watch.Stop();
            Console.WriteLine($"[{_times}]条数据,耗时：{watch.Elapsed.ToString()}");
            Console.WriteLine(_effect);
        }
        static async Task Demo1_Modi(HiSqlClient sqlClient)
        {
            int _times = 500;
            Console.WriteLine($"[{_times}]条数据 mysql插入测试");
            List<object> lstobj = new List<object>();
            for (int i = 0; i < _times; i++)
            {
                lstobj.Add(new { Domain = $"U{i.ToString()}", DomainDesc = $"用户{i.ToString()}" });
            }
            Stopwatch watch = Stopwatch.StartNew();
            //string _sql = sqlClient.Modi("Hi_Domain", lstobj).ToSql();
            int _effect =  sqlClient.Modi("Hi_Domain", lstobj).ExecCommand();

            watch.Stop();
            Console.WriteLine($"[{_times}]条数据,耗时：{watch.Elapsed.ToString()}");
            Console.WriteLine(_effect);
        }
        static async void Demo3_Insert(HiSqlClient sqlClient)
        {
            int v =  sqlClient.Insert("H_TEST", new H_TEST { UNAME = "tgm", UNAME2 = "tang", ISSYS = true }).ExecCommand();
        }


        static async Task Demo2_Insert(HiSqlClient sqlClient)
        {
            int _times = 100;
            Console.WriteLine($"[{_times}]条数据 HANA插入测试");
            List<H_TEST> lstobj = new List<H_TEST>();
            for (int i = 0; i < _times; i++)
            {
                lstobj.Add(new H_TEST { UNAME = $"U{i}", UNAME2 = $"U{i}" });
            }
            Stopwatch watch = Stopwatch.StartNew();
            //string _sql = sqlClient.Modi("Hi_Domain", lstobj).ToSql();
            int _effect = sqlClient.Insert("H_TEST", lstobj).ExecCommand();

            watch.Stop();
            Console.WriteLine($"[{_times}]条数据,耗时：{watch.Elapsed.ToString()}");
            Console.WriteLine(_effect);
        }

        static async void Demo1_Insert(HiSqlClient sqlClient)
        {

            Dictionary<string, string> _dic_data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { "uname", "tansar" }, { "Uname2", "用户123" } };
            string sql4 = sqlClient.Insert("H_Test", _dic_data).ToSql();


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
