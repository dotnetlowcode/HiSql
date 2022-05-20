using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public class Demo_Cache
    {
        public static void Init(HiSqlClient sqlClient)
        {
            Domo_Cache();
        }

        static void Demo_CheckLock()
        {
            string _key = "4900001223";
            var rtn = HiSql.Lock.CheckLock(_key);
            if (!rtn.Item1)
            {
                Console.WriteLine($"没有其它人操作采购订单[{_key}]");
            }
            else
                Console.WriteLine(rtn.Item2);//输出是谁在操作采购订单
        }

        static void Demo_LockOn()
        {
            string _key = "4900001223";
            //LckInfo 是指加锁时需要指定的信息  UName 表示加锁人，ip表示在哪一个地址加的锁，可以通过 HiSql.Lock.GetCurrLockInfo  获取所有的详细加锁信息便于后台管理
            var rtn = HiSql.Lock.LockOn(_key, new LckInfo { UName = "登陆名", Ip = "127.0.0.1" });
            if (rtn.Item1)
            {
                Console.WriteLine($"针对于采购订单[{_key}] 加锁成功");
                //执行采购订单处理业务

                //解锁
                HiSql.Lock.UnLock(_key);
            }
            
        }

        static void Demo_LockOnExcute()
        {
            string _key = "4900001223";
            //LckInfo 是指加锁时需要指定的信息  UName 表示加锁人，ip表示在哪一个地址加的锁，可以通过 HiSql.Lock.GetCurrLockInfo  获取所有的详细加锁信息便于后台管理
            var rtn = HiSql.Lock.LockOnExecute(_key, () =>
            {
                //加锁成功后执行的业务
                Console.WriteLine($"针对于采购订单[{_key}] 加锁并业务处理成功");

                //处理成功后 会自动解锁


            }, new LckInfo { UName = "登陆名", Ip = "127.0.0.1" });

        }


        static void Domo_Cache()
        {
            //HiSql.CacheContext.Cache.CheckLock

            var rtn= HiSql.Lock.CheckLock("user");
            if (rtn.Item1)
            {
                Console.WriteLine("已经在锁");
            }
            else
            {
                Console.WriteLine("不存在缓存 ");

                var rtn2=Lock.LockOnExecute("user", () => {
                    Console.WriteLine("业务处理。。。");
                
                },new LckInfo() {  UName="tansar",Ip="192.168.1.1"});

                if (rtn2.Item1)
                {
                    Console.WriteLine("业务加锁处理成功 ");
                }
                else
                {
                    Console.WriteLine($"失败：{rtn2.Item2}");
                }
            }
        }
    }
}
