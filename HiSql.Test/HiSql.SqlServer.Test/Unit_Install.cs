using HiSql.SqlServer.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HiSql.Unit.Test
{
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

            try
            {
                sqlClient.CodeFirst.InstallHisql();
                bool _has_tab= sqlClient.DbFirst.CheckTabExists(Constants.HiSysTable["Hi_TabModel"]);
                bool _has_field=sqlClient.DbFirst.CheckTabExists(Constants.HiSysTable["Hi_FieldModel"]);
                bool _has_domain=sqlClient.DbFirst.CheckTabExists(Constants.HiSysTable["Hi_Domain"]);
                bool _has_element= sqlClient.DbFirst.CheckTabExists(Constants.HiSysTable["Hi_DataElement"]);


                _outputHelper.WriteLine("初始化SqlServer");

                Assert.True(_has_tab&& _has_field&& _has_domain&& _has_element);
            }
            catch (Exception ex)
            {
                Assert.True(false);
            }
        }


        [Fact(DisplayName = "MySql初始化")]
        [Trait("install", "init")]
        public void InstallMySqlServer()
        {
            HiSqlClient sqlClient = TestClientInit.GetMySqlClient();

            try
            {
                sqlClient.CodeFirst.InstallHisql();
                bool _has_tab = sqlClient.DbFirst.CheckTabExists(Constants.HiSysTable["Hi_TabModel"]);
                bool _has_field = sqlClient.DbFirst.CheckTabExists(Constants.HiSysTable["Hi_FieldModel"]);
                bool _has_domain = sqlClient.DbFirst.CheckTabExists(Constants.HiSysTable["Hi_Domain"]);
                bool _has_element = sqlClient.DbFirst.CheckTabExists(Constants.HiSysTable["Hi_DataElement"]);

                Assert.True(_has_tab && _has_field && _has_domain && _has_element);
            }
            catch (Exception ex)
            {
                Assert.True(false);
            }
        }

        [Fact(DisplayName = "Oracle初始化")]
        [Trait("install", "init")]
        public void InstallOracleServer()
        {
            HiSqlClient sqlClient = TestClientInit.GetOracleClient();

            try
            {
                sqlClient.CodeFirst.InstallHisql();
                bool _has_tab = sqlClient.DbFirst.CheckTabExists(Constants.HiSysTable["Hi_TabModel"]);
                bool _has_field = sqlClient.DbFirst.CheckTabExists(Constants.HiSysTable["Hi_FieldModel"]);
                bool _has_domain = sqlClient.DbFirst.CheckTabExists(Constants.HiSysTable["Hi_Domain"]);
                bool _has_element = sqlClient.DbFirst.CheckTabExists(Constants.HiSysTable["Hi_DataElement"]);

                Assert.True(_has_tab && _has_field && _has_domain && _has_element);
            }
            catch (Exception ex)
            {
                Assert.True(false);
            }
        }

        [Fact(DisplayName = "PostGreSql初始化")]
        [Trait("install", "init")]
        public void InstallPostgreServer()
        {
            HiSqlClient sqlClient = TestClientInit.GetPostgreSqlClient();

            try
            {
                sqlClient.CodeFirst.InstallHisql();
                bool _has_tab = sqlClient.DbFirst.CheckTabExists(Constants.HiSysTable["Hi_TabModel"]);
                bool _has_field = sqlClient.DbFirst.CheckTabExists(Constants.HiSysTable["Hi_FieldModel"]);
                bool _has_domain = sqlClient.DbFirst.CheckTabExists(Constants.HiSysTable["Hi_Domain"]);
                bool _has_element = sqlClient.DbFirst.CheckTabExists(Constants.HiSysTable["Hi_DataElement"]);

                Assert.True(_has_tab && _has_field && _has_domain && _has_element);
            }
            catch (Exception ex)
            {
                Assert.True(false);
            }
        }

        [Fact(DisplayName = "Hana初始化")]
        [Trait("install", "init")]
        public void InstallHanaServer()
        {
            HiSqlClient sqlClient = TestClientInit.GetHanaClient();

            try
            {
                sqlClient.CodeFirst.InstallHisql();
                bool _has_tab = sqlClient.DbFirst.CheckTabExists(Constants.HiSysTable["Hi_TabModel"]);
                bool _has_field = sqlClient.DbFirst.CheckTabExists(Constants.HiSysTable["Hi_FieldModel"]);
                bool _has_domain = sqlClient.DbFirst.CheckTabExists(Constants.HiSysTable["Hi_Domain"]);
                bool _has_element = sqlClient.DbFirst.CheckTabExists(Constants.HiSysTable["Hi_DataElement"]);

                Assert.True(_has_tab && _has_field && _has_domain && _has_element);
            }
            catch (Exception ex)
            {
                Assert.True(false);
            }
        }


        [Fact(DisplayName = "Sqlite初始化")]
        [Trait("install", "init")]
        public void InstallSqliteServer()
        {
            HiSqlClient sqlClient = TestClientInit.GetSqliteClient();

            try
            {
                sqlClient.CodeFirst.InstallHisql();
                bool _has_tab = sqlClient.DbFirst.CheckTabExists(Constants.HiSysTable["Hi_TabModel"]);
                bool _has_field = sqlClient.DbFirst.CheckTabExists(Constants.HiSysTable["Hi_FieldModel"]);
                bool _has_domain = sqlClient.DbFirst.CheckTabExists(Constants.HiSysTable["Hi_Domain"]);
                bool _has_element = sqlClient.DbFirst.CheckTabExists(Constants.HiSysTable["Hi_DataElement"]);

                Assert.True(_has_tab && _has_field && _has_domain && _has_element);
            }
            catch (Exception ex)
            {
                Assert.True(false);
            }
        }

        [Fact(DisplayName = "达梦初始化")]
        [Trait("install", "init")]
        public void InstallDaMengServer()
        {
            HiSqlClient sqlClient = TestClientInit.GetDaMengClient();

            try
            {
                sqlClient.CodeFirst.InstallHisql();
                bool _has_tab = sqlClient.DbFirst.CheckTabExists(Constants.HiSysTable["Hi_TabModel"]);
                bool _has_field = sqlClient.DbFirst.CheckTabExists(Constants.HiSysTable["Hi_FieldModel"]);
                bool _has_domain = sqlClient.DbFirst.CheckTabExists(Constants.HiSysTable["Hi_Domain"]);
                bool _has_element = sqlClient.DbFirst.CheckTabExists(Constants.HiSysTable["Hi_DataElement"]);

                Assert.True(_has_tab && _has_field && _has_domain && _has_element);
            }
            catch (Exception ex)
            {
                Assert.True(false);
            }
        }
    }

    
}
