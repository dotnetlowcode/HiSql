using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace HiSql
{
    public class Demo_Snro
    {
        static SeriNumber number = null;
        static List<string> lstnum = new List<string>();
        public static void Init(HiSqlClient sqlClient)
        {

            //Snro_Demo(sqlClient);
            //Snro_Demo2(sqlClient);
            string s = Console.ReadLine();
        }

        static void Snro_Demo2(HiSqlClient sqlClient)
        {
            //SeriNumber number = new SeriNumber();
            //number.Load(sqlClient);
            // number.NewNumber("asdf", 1);


            //var snrolist = sqlClient.Query("Hi_Snro").Field("*").ToList<SNRO.Hi_Snro>();


            //foreach (SNRO.Hi_Snro snro in snrolist)
            //{ 

            //}

            //sqlClient.Update<SNRO.Hi_Snro>(snrolist).Only("CurrNum").ExecCommand();

            Global.SnroOn = true;
            string _a = "9000123";
            string _b = "9000124";
            string _c = "9000122";
            int _v = _a.Compare(_a);
            //-1 是小于
            //1 是大于
            //0是大于

            //number.Load(sqlClient);
            //string nums = number.NewNumber("MATDOC", 1);

            //string path = $"{Environment.CurrentDirectory}\\Snro";
            //if (!System.IO.Directory.Exists(path))
            //    System.IO.Directory.CreateDirectory(path);


            HiSql.Global.RedisOn = true;//开启redis缓存
            HiSql.Global.RedisOptions = new RedisOptions { Host = "172.16.80.178", PassWord = "qwe123", Port = 6379, CacheRegion = "HRM", Database = 2 };

            HiSql.Global.NumberOptions.MultiMode= true;

            Console.WriteLine("正在启动...");
            Thread.Sleep(5000);


            //sqlClient.DbFirst.Truncate("H_nlog");
            number = new SeriNumber(sqlClient);
            int _threadNum = 1000;

            List<object> lst = new List<object>();
            for (int i = 0; i < _threadNum; i++)
            {
                //Thread thread = new Thread(threadSnro);

                //thread.Start();

                var num=number.NewNumber("Order", 1);
                lst.Add(new { Numbers=num});
                Console.WriteLine(num);
                //Thread.Sleep(new Random().Next(1000));
            }


            sqlClient.Insert("H_nlog", lst).ExecCommand();
            Console.WriteLine($"完成...");

            //Thread.Sleep(10000);
            ///落盘
            //number.SyncDisk();
            //Console.WriteLine("落盘完成");

            //List<string> lstnum = new List<string>();
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            //for (int i = 0; i < 100; i++)
            //{
            //    string nums = number.NewNumber("MATDOC", 1);
            //    lstnum.Add(nums);
            //}
            //sw.Stop();
            //TimeSpan ts2 = sw.Elapsed;
            //Console.WriteLine("sw总共花费{0}ms.", ts2.TotalMilliseconds);




            //Console.WriteLine($"编号创建：{lstnum.Count}条");

            var s = Console.ReadLine();

        }
        static void threadSnro()
        {
            
            for (int i = 0; i < 100; i++)
            {
                string nums = number.NewNumber("MATDOC", 1);

                //Console.WriteLine($"当前线程:{Thread.CurrentThread.ManagedThreadId.ToString()} 产生编号{nums} 当前第{i}个");
                //Thread.Sleep(new Random().Next(1000));
                if (string.IsNullOrEmpty(nums))
                {
                    Console.WriteLine($"当前线程:{Thread.CurrentThread.ManagedThreadId.ToString()} 产生编号{nums} 当前第{i}个 为空");
                }
                //else
                //    Console.WriteLine(nums);
                lstnum.Add(nums);
            }
        }

        static void Snro_Demo(HiSqlClient sqlClient)
        {
            Global.SnroOn = true;
            sqlClient.CodeFirst.InstallHisql();

            var obj1 = new { SNRO = "MATDOC", SNUM = 1, IsSnow=false, SnowTick=0, StartNum = "9000000", EndNum = "9999999",Length=7, CurrNum = "9000000", IsNumber = true, IsHasPre = false, CacheSpace = 10, Descript = "物料主数据编号" };
            var obj2 = new { SNRO = "Order", SNUM = 1, IsSnow=true, SnowTick=145444, StartNum = "", EndNum = "",Length=7, CurrNum = "", IsNumber = true, IsHasPre = false, CacheSpace = 10, Descript = "订单号雪花ID" };

            List<object> list = new List<object>();
            list.Add(obj1);
            list.Add(obj2);

            sqlClient.Modi("Hi_Snro", list).ExecCommand();
            //sqlClient.Update("Hi_Snro", new SNRO.Hi_Snro { SNRO = "MATDOC", SNUM = 1, StartNum = "9000000", EndNum = "9999998", CurrNum = "9000001", IsNumber = true, IsHasPre = false, CacheSpace = 10, Descript = "物料主数据编号" })
            //    .Only("CurrNum")
            //    .ExecCommand() ;


            //sqlClient.Update("Hi_Snro", new { SNRO = "MATDOC", SNUM = 1, StartNum = "9000000", EndNum = "9999998", CurrNum = "9000001", IsNumber = true, IsHasPre = false, CacheSpace = 10, Descript = "物料主数据编号" })
            //    .Only("CurrNum")
            //    .ExecCommand();
            //string _json=JsonConvert.SerializeObject(obj);

            // var josn=DesAnonymousType(_json, new { SNRO = "", SNUM = 0, StartNum = "", EndNum = "", CurrNum = "", IsNumber = false, IsHasPre = false, CacheSpace = 0, Descript = "" });
            //SNRO.Number number = new SNRO.Number();
            //number.Load(sqlClient);
        }

        public static T DesAnonymousType<T>(string json, T anonymousTypeObject)
        {
            return JsonConvert.DeserializeAnonymousType(json, anonymousTypeObject);
        }


    }
}
