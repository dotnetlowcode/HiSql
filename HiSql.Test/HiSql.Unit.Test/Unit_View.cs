using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HiSql.Unit.Test
{
    [Collection("step3")]
    public class Unit_View
    {
        private readonly ITestOutputHelper _outputHelper;

        public Unit_View(ITestOutputHelper testOutputHelper)
        {
            _outputHelper = testOutputHelper;
        }
        [Fact(DisplayName = "SqlServer视图操作")]
        [Trait("View", "init")]
        public void ViewSqlServer()
        {
            HiSqlClient sqlClient = TestClientInit.GetSqlServerClient();
            viewGroups(sqlClient);
        }

        [Fact(DisplayName = "MySql视图操作")]
        [Trait("View", "init")]
        public void ViewMySql()
        {
            HiSqlClient sqlClient = TestClientInit.GetMySqlClient();
            viewGroups(sqlClient);
        }
        [Fact(DisplayName = "Oracle视图操作")]
        [Trait("View", "init")]
        public void ViewOracle()
        {
            HiSqlClient sqlClient = TestClientInit.GetOracleClient();
            viewGroups(sqlClient);
        }
        [Fact(DisplayName = "PostgreSql视图操作")]
        [Trait("View", "init")]
        public void ViewPostgreSql()
        {
            HiSqlClient sqlClient = TestClientInit.GetPostgreSqlClient();
            viewGroups(sqlClient);

        }
        [Fact(DisplayName = "Hana视图操作")]
        [Trait("View", "init")]
        public void ViewHana()
        {
            HiSqlClient sqlClient = TestClientInit.GetHanaClient();
            viewGroups(sqlClient);
        }
        [Fact(DisplayName = "Sqlite视图操作")]
        [Trait("View", "init")]
        public void ViewSqlite()
        {
            HiSqlClient sqlClient = TestClientInit.GetSqliteClient();
            viewGroups(sqlClient);

        }
        [Fact(DisplayName = "达梦视图操作")]
        [Trait("View", "init")]
        public void ViewDaMeng()
        {
            HiSqlClient sqlClient = TestClientInit.GetDaMengClient();
            viewGroups(sqlClient);
        }



        #region
        void viewGroups(HiSqlClient sqlClient)
        {
            dropView(sqlClient, false);

            createView(sqlClient);

            modiView(sqlClient);

            dropView(sqlClient,true);


        }


        void createView(HiSqlClient sqlClient)
        {
            bool _isok = true;
            string viewname = "vw_FModel";
            var rtn = sqlClient.DbFirst.CreateView(viewname,
                sqlClient.HiSql("select a.TabName,b.TabReName,b.TabDescript,a.FieldName,a.SortNum,a.FieldType from Hi_FieldModel as a inner join Hi_TabModel as b on a.TabName=b.TabName").ToSql(),
                OpLevel.Execute);
            if (rtn.Item1)
            {
                _outputHelper.WriteLine($"视图[{viewname}] 创建成功{System.Environment.NewLine}  {System.Environment.NewLine} SQL:{rtn.Item3}");

                

                if (sqlClient.DbFirst.CheckTabExists(viewname))
                {
                    _outputHelper.WriteLine($"新创建视图[{viewname}] 已经存在于库中");
                    string json = sqlClient.HiSql($"select * from {viewname}").Take(10).Skip(1).ToJson();
                    _outputHelper.WriteLine($"验证视图[{viewname}] 查询结果！ json:{json}");
                }else{
                    _outputHelper.WriteLine($"新创建视图[{viewname}] 不存在于数据库中");
                    _isok = false;
                }
                


            }
            else
            {
                _outputHelper.WriteLine($"视图[{viewname}] 创建失败{System.Environment.NewLine} error:{rtn.Item2} {System.Environment.NewLine} SQL:{rtn.Item3}");
                _isok = false;
            }

            Assert.True(_isok);
        }

        void modiView(HiSqlClient sqlClient)
        {
            bool _isok = true;
            string viewname = "vw_FModel";
            var rtn = sqlClient.DbFirst.ModiView(viewname,
                sqlClient.HiSql("select a.TabName,b.TabReName,b.TabDescript,a.FieldName,a.SortNum,a.FieldType from Hi_FieldModel as a inner join Hi_TabModel as b on a.TabName=b.TabName where b.TabType in (0,1)").ToSql(),
                OpLevel.Execute);
            if (rtn.Item1)
            {
                _outputHelper.WriteLine($"视图[{viewname}] 修改成功{System.Environment.NewLine}  {System.Environment.NewLine} SQL:{rtn.Item3}");



                if (sqlClient.DbFirst.CheckTabExists(viewname))
                {
                    _outputHelper.WriteLine($"新修改视图[{viewname}] 已经存在于库中");
                    string json = sqlClient.HiSql($"select * from {viewname}").Take(10).Skip(1).ToJson();
                    _outputHelper.WriteLine($"验证视图[{viewname}] 查询结果！ json:{json}");
                }
                else
                {
                    _outputHelper.WriteLine($"新修改视图[{viewname}] 不存在于数据库中");
                    _isok = false;
                }



            }
            else
            {
                _outputHelper.WriteLine($"视图[{viewname}] 视图修改失败{System.Environment.NewLine} error:{rtn.Item2} {System.Environment.NewLine} SQL:{rtn.Item3}");
                _isok = false;
            }

            Assert.True(_isok);
        }


        void dropView(HiSqlClient sqlClient,bool noexiterror)
        {
            string viewname = "vw_FModel";
            if (sqlClient.DbFirst.CheckTabExists(viewname))
            {
                var rtn= sqlClient.DbFirst.DropView(viewname, OpLevel.Execute);
                if (rtn.Item1)
                {
                    _outputHelper.WriteLine($"删除视图[{viewname}] 成功{System.Environment.NewLine}  {System.Environment.NewLine} SQL:{rtn.Item3}");
                }
                else {
                    _outputHelper.WriteLine($"删除视图[{viewname}] 失败{System.Environment.NewLine} error:{rtn.Item2} {System.Environment.NewLine} SQL:{rtn.Item3}");
                }
            }
            else
            {
                _outputHelper.WriteLine($"视图[{viewname}] 不存在");

                if (noexiterror)
                    Assert.True(false);
            }
        }


        #endregion
    }
}
