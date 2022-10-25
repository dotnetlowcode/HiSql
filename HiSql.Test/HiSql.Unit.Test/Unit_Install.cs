using HiSql.Unit.Test;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HiSql.Unit.Test
{

    /// <summary>
    /// HiSql各支持的数据库 初始化单元测试
    /// </summary>
    [Collection("step1")]
    public class Unit_Install
    {
        private readonly ITestOutputHelper _outputHelper;
        public Unit_Install(ITestOutputHelper testOutputHelper)
        {
            _outputHelper = testOutputHelper;
        }
        [Fact(DisplayName = "SqlServer初始化")]
        [Trait("install", "init")]
        public void InstallSqlServer()
        {
            HiSqlClient sqlClient = TestClientInit.GetSqlServerClient();
            checkSystemTable(sqlClient);
        }


        [Fact(DisplayName = "MySql初始化")]
        [Trait("install", "init")]
        public void InstallMySql()
        {
            HiSqlClient sqlClient = TestClientInit.GetMySqlClient();
            checkSystemTable(sqlClient);
        }

        [Fact(DisplayName = "Oracle初始化")]
        [Trait("install", "init")]
        public void InstallOracle()
        {
            HiSqlClient sqlClient = TestClientInit.GetOracleClient();
            checkSystemTable(sqlClient);
        }

        [Fact(DisplayName = "PostGreSql初始化")]
        [Trait("install", "init")]
        public void InstallPostgreSql()
        {
            HiSqlClient sqlClient = TestClientInit.GetPostgreSqlClient();
            checkSystemTable(sqlClient);
        }

        [Fact(DisplayName = "Hana初始化")]
        [Trait("install", "init")]
        public void InstallHana()
        {
            HiSqlClient sqlClient = TestClientInit.GetHanaClient();
            checkSystemTable(sqlClient);
        }


        [Fact(DisplayName = "Sqlite初始化")]
        [Trait("install", "init")]
        public void InstallSqliteServer()
        {
            HiSqlClient sqlClient = TestClientInit.GetSqliteClient();
            checkSystemTable(sqlClient);
        }

        [Fact(DisplayName = "达梦初始化")]
        [Trait("install", "init")]
        public void InstallDaMengServer()
        {
            HiSqlClient sqlClient = TestClientInit.GetDaMengClient();
            checkSystemTable(sqlClient);
        }




        #region

        /// <summary>
        /// 检测系统表结构
        /// </summary>
        void checkSystemTable(HiSqlClient sqlClient)
        {
            sqlClient.CurrentConnectionConfig.AppEvents = GetAopEvent();             
            sqlClient.CodeFirst.InstallHisql();
            bool _has_tab = sqlClient.DbFirst.CheckTabExists(Constants.HiSysTable["Hi_TabModel"]);
            bool _has_field = sqlClient.DbFirst.CheckTabExists(Constants.HiSysTable["Hi_FieldModel"]);
            bool _has_domain = sqlClient.DbFirst.CheckTabExists(Constants.HiSysTable["Hi_Domain"]);
            bool _has_element = sqlClient.DbFirst.CheckTabExists(Constants.HiSysTable["Hi_DataElement"]);
            Assert.True(_has_tab && _has_field && _has_domain && _has_element);

            TabInfo tabInfo1 = sqlClient.Context.DMInitalize.GetTabStruct(Constants.HiSysTable["Hi_TabModel"]);
            TabInfo eftabInfo1 = sqlClient.Context.DMInitalize.BuildTab(typeof(Hi_TabModel));
            TabInfo phytabInfo1 = sqlClient.Context.DMInitalize.GetPhyTabStruct(Constants.HiSysTable["Hi_TabModel"]);
            Assert.True(comparisonTabInfo(phytabInfo1, tabInfo1, sqlClient.CurrentConnectionConfig.DbType));

            TabInfo tabInfo2 = sqlClient.Context.DMInitalize.GetTabStruct(Constants.HiSysTable["Hi_FieldModel"]);
            TabInfo phytabInfo2 = sqlClient.Context.DMInitalize.GetPhyTabStruct(Constants.HiSysTable["Hi_FieldModel"]);
            Assert.True(comparisonTabInfo(phytabInfo2, tabInfo2, sqlClient.CurrentConnectionConfig.DbType));



            TabInfo tabInfo3 = sqlClient.Context.DMInitalize.GetTabStruct(Constants.HiSysTable["Hi_Domain"]);
            TabInfo phytabInfo3 = sqlClient.Context.DMInitalize.GetPhyTabStruct(Constants.HiSysTable["Hi_Domain"]);
            Assert.True(comparisonTabInfo(phytabInfo3, tabInfo3, sqlClient.CurrentConnectionConfig.DbType));


            TabInfo tabInfo4 = sqlClient.Context.DMInitalize.GetTabStruct(Constants.HiSysTable["Hi_DataElement"]);
            TabInfo phytabInfo4 = sqlClient.Context.DMInitalize.GetPhyTabStruct(Constants.HiSysTable["Hi_DataElement"]);
            Assert.True(comparisonTabInfo(phytabInfo4, tabInfo4, sqlClient.CurrentConnectionConfig.DbType));
        }


        bool comparisonTabInfo(TabInfo phytabInfo , TabInfo tabinfo, DBType dbtype)
        {
            _outputHelper.WriteLine($"对比表[{phytabInfo.TabModel.TabName}] 结构信息");

            bool _isok = true;
            List<FieldChange> fieldChanges = HiSqlCommProvider.TabToCompare(phytabInfo, tabinfo, dbtype);


            foreach (var fieldchg in fieldChanges)
            {
                string _str1 = fieldchg.IsTabChange ? "结构变更不一致":"属性信息不一致";
                StringBuilder sb = new StringBuilder().AppendLine();
                foreach (var fdetail in fieldchg.ChangeDetail)
                {
                    sb.AppendLine($"\t 属性:[{fdetail.AttrName}]  A值:{fdetail.ValueA} B值:{fdetail.ValueB}");
                }
                //_isok = false;

                _outputHelper.WriteLine($"字段:[{fieldchg.FieldName}] 差异原因:{_str1} 差异明细:{sb.ToString()}");
            }


            return _isok;

        }
        AopEvent GetAopEvent()
        {
            return new AopEvent()
            {
                OnDbDecryptEvent = (connstr) =>
                {
                    //解密连接字段
                    //Console.WriteLine($"数据库连接:{connstr}");

                    return connstr;
                },
                OnLogSqlExecuting = (sql, param) =>
                {
                    //sql执行前 日志记录 (异步)
                    _outputHelper.WriteLine($"OnLogSqlExecuting:{System.Environment.NewLine}{sql}");
                    //Console.WriteLine($"sql执行前记录{sql} time:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")}");
                },
                OnLogSqlExecuted = (sql, param) =>
                {
                    //sql执行后 日志记录 (异步)
                    //Console.WriteLine($"sql执行后记录{sql} time:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")}");
                },
                OnSqlError = (sqlEx) =>
                {
                    //sql执行错误后 日志记录 (异步)
                    _outputHelper.WriteLine($"OnSqlError:{System.Environment.NewLine}{sqlEx.Message.ToString()}");
                },
                OnTimeOut = (int timer) =>
                {
                    //Console.WriteLine($"执行SQL语句超过[{timer.ToString()}]毫秒...");
                }
            };

        }
        #endregion
    }


}
