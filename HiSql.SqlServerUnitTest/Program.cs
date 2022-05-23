using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;

namespace HiSql.UnitTest
{
    class Program
    {
        public delegate string MethodCaller(string firstName, string lastName);
        static void Main(string[] args)
        {
            /*
            Console.WriteLine("1:" + Thread.CurrentThread.ManagedThreadId);
            MethodCaller method = new MethodCaller(GetFullName);
            //Task task = Task.Run(() => MethodCaller(GetFullName)) ;
            //IAsyncResult result = method.BeginInvoke("m", "n",null,null);
            var workTask = Task.Run(() => method.Invoke("m", "n" ));
            bool flag = workTask.Wait(2000,new CancellationToken(true));
            //bool flag = result.AsyncWaitHandle.WaitOne(2000, true);//请教WaitOne的第二个参数是什么作用？            
            if (flag)
            {
                Console.WriteLine("time in");
            }
            else
            {
                Console.WriteLine("time out");
            }

            string fullName = workTask.Result;


            Console.WriteLine(fullName);
            Console.WriteLine("2:" + Thread.CurrentThread.ManagedThreadId);
            */

            /*
            dynamic o1 = new { UserName = "tansar", Age = 33 };
            ExpandoObject o2 = new ExpandoObject();
            dynamic o3 = (dynamic)o2;
            o3.UserName = "tansar";
            o3.Age = 33;

            TDynamic t1 = new TDynamic();
            t1["UserName"] = "tansar";
            t1["Age"] = 33;
            dynamic o4 = (ExpandoObject)t1;
            Console.WriteLine(t1["Age"]);

            Console.WriteLine(o4.Age);

            //DataConvert.ToDynamic(new TDynamic(new { UserName = "tansar", Age = 33 }).ToDynamic());
            DataConvert.ToDynamic(t1.ToDynamic());
            */


            HiSqlClient sqlcient = Demo_Init.GetSqlClient();

            // Console.WriteLine($"数据库连接id"+sqlcient.Context.ConnectedId);

            //Demo_Update.Init(sqlcient);
            //Demo_Query.Init(sqlcient);

            //Demo_Delete.Init(sqlcient);
            //Demo_Insert.Init(sqlcient);
            //DemoCodeFirst.Init(sqlcient);
            //Demo_Snro.Init(sqlcient);
            //Demo_DbCode.Init(sqlcient);

            //Demo_Cache.Init(sqlcient);
            SnowId();
            Console.ReadLine();
        }

        static void SnowId()
        {
            //IdWorker idWorker = new IdWorker(0, IdWorker.TimeTick(new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc)));
            //IdSnow snowflake = new IdSnow(0, IdSnow.TimeTick(new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc)));
            List<long> lst=new List<long>();


         
            Snowflake.SnowType = SnowType.IdSnow;
            Snowflake.WorkerId = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < 10000; i++)
            {
                lst.Add(Snowflake.NextId());
                //Console.WriteLine(idWorker.NextId());
                //Console.WriteLine(snowflake.NextId());

                //Console.WriteLine(Snowflake.NextId());
            }
            sw.Stop();

            Console.WriteLine($"耗时：{sw.Elapsed}秒");


        }

         public static void ToAnonymous(dynamic o)
        {

            //var ostr = JsonConvert.SerializeObject(o);

            //dynamic json = Newtonsoft.Json.Linq.JToken.Parse(ostr) as dynamic;


            Type type = o.GetType();
            dynamic x = new { UserName = "tansar", Age = 33 };
            dynamic dyn = (dynamic)o;

            Console.WriteLine($"UserName:{dyn.UserName},Age:{dyn.Age}");
            //object o1=Activator.CreateInstance(type, true);

            //if (o1 != null)
            //{ 

            //}

        }
        public static string GetFullName(string firstName, string lastName)
        {
            Console.WriteLine("3:" + Thread.CurrentThread.ManagedThreadId);
            Thread.Sleep(6000);
            Console.WriteLine("3:" + Thread.CurrentThread.ManagedThreadId);
            return firstName + lastName;
        }
    }
}
