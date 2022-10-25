using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HiSql.Unit.Test
{


    [Collection("UinitCache")]
    public class Unit_Cache
    {
 
        private readonly ITestOutputHelper _outputHelper;

        public Unit_Cache(ITestOutputHelper testOutputHelper)
        {
            _outputHelper = testOutputHelper;
        }
        [Fact(DisplayName = "本地缓存测试")]
        [Trait("Cache", "init")]
        public void LocalCache()
        {


            string _keyvalue=HiSql.CacheContext.Cache.GetOrCreate<string>("test", () => {

                return "localcache";
            },10);
            _outputHelper.WriteLine($"key:test 已经生成 value:{_keyvalue} 有效时间为10秒");
            _keyvalue = CacheContext.Cache.GetCache<string>("test");
            if (string.IsNullOrEmpty(_keyvalue))
                _outputHelper.WriteLine($"key:test 已经不存在");
            else
                _outputHelper.WriteLine($"key:test value:{_keyvalue}");

            System.Threading.Thread.Sleep(11000);

            _keyvalue=CacheContext.Cache.GetCache<string>("test");
            if(string.IsNullOrEmpty(_keyvalue))
                _outputHelper.WriteLine($"key:test 已经不存在");
            else
                _outputHelper.WriteLine($"key:test value:{_keyvalue}");



        }

    }
}
