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
        [Trait("MCache", "init")]

        public void Cache_MCache()
        {

            string _key = "mcache_test";
            var rtnexe = Lock.LockOnExecute(_key, () => {

                System.Threading.Thread.Sleep(1000);
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
