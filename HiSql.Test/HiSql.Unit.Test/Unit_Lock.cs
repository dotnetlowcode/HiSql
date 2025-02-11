using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Asn1.Tsp;
using Xunit;
using Xunit.Abstractions;

namespace HiSql.Unit.Test
{
    public class Unit_Lock
    {
        private readonly ITestOutputHelper _outputHelper;
        public Unit_Lock(ITestOutputHelper testOutputHelper)
        {
            _outputHelper = testOutputHelper;
        }
        [Fact(DisplayName = "MCache")]
        [Trait("MCache", "init_more")]
        public void Cache_MCache_More()
        {
            LckInfo lckinfo = new LckInfo { UName = "hone", Ip = Tool.Net.GetLocalIPAddress() };
            string _key = "order:10012245445";
            var lck1= HiSql.Lock.LockOnExecute(_key, () => {
                string _key2 = "od:125622";
                var lck2=HiSql.Lock.LockOnExecute(_key2, () => {
                    Console.WriteLine($"锁定业务处理:{_key} -{_key2}");
                }, lckinfo);
                if(lck2.Item1)
                    Assert.True(true);
                else Assert.False(true);
            }, lckinfo);
            if (lck1.Item1)
                Assert.True(true);
            else Assert.False(true);
        }

        [Fact(DisplayName = "MCache")]
        [Trait("MCache", "init")]

        public void Cache_MCache()
        {

            string _key = "mcache_test";
            var rtnexe = Lock.LockOnExecute(_key, () => {

                System.Threading.Thread.Sleep(1000);

                var  rtnlck = Lock.CheckLock(_key);
                if(rtnlck.Item1)
                    _outputHelper.WriteLine($"key:{_key} 已经被锁定中。。。。");
            }, new LckInfo { UName = "hone", Ip = Tool.Net.GetLocalIPAddress() });

            var rtnlck = Lock.CheckLock(_key);
            if (rtnlck.Item1)
            {


                _outputHelper.WriteLine($"key:{_key} 已经被锁定");



                //Assert.True(false);
            }
            else
            {
                _outputHelper.WriteLine($"key:{_key} 已经被释放");
                Assert.True(true);
            }
        }
    }
}
