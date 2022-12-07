using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HiSql.Unit.Test
{
    [Collection("snrostep1")]
    public class Unit_SNRO
    {
        private readonly ITestOutputHelper _outputHelper;

        public Unit_SNRO(ITestOutputHelper testOutputHelper)
        {
            _outputHelper = testOutputHelper;
        }
        [Fact(DisplayName = "SqlServer编号管理")]
        [Trait("Log", "init")]
        public void SnroSqlServer()
        {
            HiSqlClient sqlClient = TestClientInit.GetSqlServerClient();
            snroGroups(sqlClient);
        }

        [Fact(DisplayName = "MySql编号管理")]
        [Trait("Log", "init")]
        public void SnroMySql()
        {
            HiSqlClient sqlClient = TestClientInit.GetMySqlClient();
            snroGroups(sqlClient);

        }
        [Fact(DisplayName = "Oracle编号管理")]
        [Trait("Log", "init")]
        public void SnroOracle()
        {
            HiSqlClient sqlClient = TestClientInit.GetOracleClient();
            snroGroups(sqlClient);
        }
        [Fact(DisplayName = "PostgreSql编号管理")]
        [Trait("Log", "init")]
        public void SnroPostgreSql()
        {
            HiSqlClient sqlClient = TestClientInit.GetPostgreSqlClient();
            snroGroups(sqlClient);
        }
        [Fact(DisplayName = "Hana编号管理")]
        [Trait("Log", "init")]
        public void SnroHana()
        {
            HiSqlClient sqlClient = TestClientInit.GetHanaClient();
            snroGroups(sqlClient);
        }
        [Fact(DisplayName = "Sqlite编号管理")]
        [Trait("Log", "init")]
        public void SnroSqlite()
        {
            HiSqlClient sqlClient = TestClientInit.GetSqliteClient();
            snroGroups(sqlClient);

        }
        [Fact(DisplayName = "达梦编号管理")]
        [Trait("Log", "init")]
        public void SnroDaMeng()
        {
            HiSqlClient sqlClient = TestClientInit.GetDaMengClient();
            snroGroups(sqlClient);
        }

        #region

        void snroGroups(HiSqlClient sqlClient)
        {
            string tabname1 = "log_main_0915";
            string tabname2 = "log_detail_0915";

            startSnro(sqlClient);


            HiSql.SnroNumber.SqlClient = sqlClient;
            List<string> lstsn= HiSql.SnroNumber.NewNumber("SALENO", 1, 100);

            Assert.Equal(100, lstsn.Count);
            

            


        }

        void startSnro(HiSqlClient sqlClient)
        {
            //如果需要使用编号那么一定要开启此功能
            HiSql.Global.SnroOn = true;

            

            //开启编号后进行初始化  
            sqlClient.CodeFirst.InstallHisql();//仅需执行一次

            List<Hi_Snro> list = new List<Hi_Snro>();

            ///工作流编号配置
            ///按天产生流水号 如2205061000000-2205069999999 之间
            ///


            list.Add(new Hi_Snro { SNRO = "WFNO", SNUM = 1, IsSnow = false, SnowTick = 0, StartNum = "1000000", EndNum = "9999999", Length = 7, CurrNum = "1000000", IsNumber = true, PreType = PreType.Y2MD, FixPreChar = "", IsHasPre = true, CacheSpace = 5, Descript = "工作流编号" });

            ///生成销售订单编码 每分钟从0开始编号 如20220602145800001-20220602145899999
            list.Add(new Hi_Snro { SNRO = "SALENO", SNUM = 1, IsSnow = false, SnowTick = 0, StartNum = "10000", EndNum = "99999", Length = 5, CurrNum = "10000", IsNumber = true, PreType = PreType.YMDHm, FixPreChar = "", IsHasPre = true, CacheSpace = 10, Descript = "销售订单号流水" });
            ///生成另外一种销售订单编码 年的是取后两位 按每秒顺序生成 如22060214581200001-22060214581299999
            list.Add(new Hi_Snro { SNRO = "SALENO", SNUM = 2, IsSnow = false, SnowTick = 0, StartNum = "10000", EndNum = "99999", Length = 5, CurrNum = "10000", IsNumber = true, PreType = PreType.Y2MDHms, FixPreChar = "", IsHasPre = true, CacheSpace = 10, Descript = "销售订单号流水" });


            ///通过雪花ID生成
            list.Add(new Hi_Snro { SNRO = "Order", SNUM = 1, IsSnow = true, SnowTick = 145444, StartNum = "", EndNum = "", Length = 7, CurrNum = "", IsNumber = true, IsHasPre = false, CacheSpace = 10, Descript = "订单号雪花ID" });


            //保存配置到表中
            sqlClient.Modi("Hi_Snro", list).ExecCommand();
        }

        #endregion
    }
}
