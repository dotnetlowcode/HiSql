using HiSql.PostGreSqlUnitTest.Table;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql.PostGreSqlUnitTest
{
    class Demo_Insert
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
            //Demo1_Insert(sqlClient);
            //Demo2_Insert(sqlClient);
            //Demo1_Insert3(sqlClient);
            //Demo1_Modi(sqlClient);
            //Demo1_Modi2(sqlClient);
            //Demo4_Insert1(sqlClient);
            Demo5_Insert(sqlClient);
            string s = Console.ReadLine();
        }
        static void Demo5_Insert(HiSqlClient sqlClient) {
            bool isexits=sqlClient.DbFirst.CheckTabExists(typeof(HTest02).Name);
            if (!isexits) {
                sqlClient.DbFirst.CreateTable(typeof(HTest02));
            }
            sqlClient.Delete(typeof(HTest02).Name).ExecCommand();
            string _sql = sqlClient.Insert(typeof(HTest02).Name, new HTest02 { SID = 1, UName = "tansar" }).ToSql();
            sqlClient.Insert(typeof(HTest02).Name,new HTest02 {  SID=1, UName ="tansar"} ).ExecCommand();
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
        static void Demo1_Insert3(HiSqlClient sqlClient)
        {
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();

            Dictionary<string, string> _dic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { "Domain", "UTYPE" }, { "DomainDesc", "用户类型1" } };
            list.Add(_dic);

            Dictionary<string, string> _dic1 = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { "Domain", "UTYPE1" }, { "DomainDesc", "用户类型2" } };
            list.Add(_dic1);

            sqlClient.Modi<Dictionary<string, string>>("Hi_Domain", list).ExecCommand();
            sqlClient.Modi<Dictionary<string, string>>("Hi_Domain", list).ExecCommand();
        }
        static async Task Demo1_Modi2(HiSqlClient sqlClient)
        {
            int _times = 10000;
            Console.WriteLine($"[{_times}]条数据 postgresql插入测试");
            List<H_Test> lstobj = new List<H_Test>();
            for (int i = 0; i < _times; i++)
            {
                lstobj.Add(new H_Test { UNAME = $"U{i}", UNAME2 = $"2U{i}" });
            }
            Stopwatch watch = Stopwatch.StartNew();
            //string _sql = sqlClient.Modi("Hi_Domain", lstobj).ToSql();
            int _effect =  sqlClient.Modi("H_Test", lstobj).ExecCommand();

            watch.Stop();
            Console.WriteLine($"[{_times}]条数据,耗时：{watch.Elapsed.ToString()}");
            Console.WriteLine(_effect);
        }
        static async void Demo1_Modi(HiSqlClient sqlClient)
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
            int _effect = sqlClient.Modi("Hi_Domain", lstobj).ExecCommand();

            watch.Stop();
            Console.WriteLine($"[{_times}]条数据,耗时：{watch.Elapsed.ToString()}");
            Console.WriteLine(_effect);
        }
        static async void Demo2_Insert(HiSqlClient sqlClient)
        {
            int _times = 100;
            Console.WriteLine($"[{_times}]条数据 POSTGRESQL插入测试");
            List<H_Test> lstobj = new List<H_Test>();
            for (int i = 0; i < _times; i++)
            {
                lstobj.Add(new H_Test { UNAME = $"U{i}", UNAME2 = $"U{i}" });
            }
            Stopwatch watch = Stopwatch.StartNew();
            //string _sql = sqlClient.Modi("Hi_Domain", lstobj).ToSql();
            int _effect = sqlClient.Insert("H_Test", lstobj).ExecCommand();

            watch.Stop();
            Console.WriteLine($"[{_times}]条数据,耗时：{watch.Elapsed.ToString()}");
            Console.WriteLine(_effect);
        }
        static async void Demo1_Insert(HiSqlClient sqlClient)
        {

            int _times = 10000;
            Console.WriteLine($"[{_times}]条数据 postgresql插入测试");
            List<object> lstobj = new List<object>();
            for (int i = 0; i < _times; i++)
            {
                lstobj.Add(new { Domain = $"a{i.ToString()}", DomainDesc = $"用户{i.ToString()}" });
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
