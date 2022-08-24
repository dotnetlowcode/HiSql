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
    public class Unit_Table
    {

        private readonly ITestOutputHelper _outputHelper;
        public Unit_Table(ITestOutputHelper testOutputHelper)
        {
            _outputHelper = testOutputHelper;
        }
        [Fact(DisplayName = "SqlServer表操作")]
        [Trait("Table", "init")]
        public void TableSqlServer()
        {
            HiSqlClient sqlClient = TestClientInit.GetSqlServerClient();
            tableGroups(sqlClient);
        }

        [Fact(DisplayName = "MySql表操作")]
        [Trait("Table", "init")]
        public void TableMySql()
        {
            HiSqlClient sqlClient = TestClientInit.GetMySqlClient();
            tableGroups(sqlClient);
        }
        [Fact(DisplayName = "Oracle表操作")]
        [Trait("Table", "init")]
        public void TableOracle()
        {
            HiSqlClient sqlClient = TestClientInit.GetOracleClient();
            tableGroups(sqlClient);
        }
        [Fact(DisplayName = "PostgreSql表操作")]
        [Trait("Table", "init")]
        public void TablePostgreSql()
        {
            HiSqlClient sqlClient = TestClientInit.GetPostgreSqlClient();
            tableGroups(sqlClient);

        }
        [Fact(DisplayName = "Hana表操作")]
        [Trait("Table", "init")]
        public void TableHana()
        {
            HiSqlClient sqlClient = TestClientInit.GetHanaClient();
            tableGroups(sqlClient);
        }
        [Fact(DisplayName = "Sqlite表操作")]
        [Trait("Table", "init")]
        public void TableSqlite()
        {
            HiSqlClient sqlClient = TestClientInit.GetSqliteClient();
            tableGroups(sqlClient);

        }
        [Fact(DisplayName = "达梦表操作")]
        [Trait("Table", "init")]
        public void TableDaMeng()
        {
            HiSqlClient sqlClient = TestClientInit.GetDaMengClient();
            tableGroups(sqlClient);
        }



        #region


        void tableGroups(HiSqlClient sqlClient)
        {
            reTabName(sqlClient);

            indexDemo(sqlClient);

            createDemoDynTable(sqlClient);
        }


        void showIndexList(HiSqlClient sqlClient,string tabname)
        {
            List<TabIndex> lstindex = sqlClient.DbFirst.GetTabIndexs(tabname);
            foreach (TabIndex tabIndex in lstindex)
            {
                _outputHelper.WriteLine($"TabName:{tabIndex.TabName} IndexName:{tabIndex.IndexName} IndexType:{tabIndex.IndexType}");
            }
        }

        void indexDemo(HiSqlClient sqlClient)
        {
            Unit_Insert unit_Insert = new Unit_Insert(_outputHelper);

            //向表H_Test01中插入100条数据 

            unit_Insert.InsertData(sqlClient, 100);


            bool _isok = true;
            string tabname = typeof(H_Test01).Name;
            string keyidxname = $"{tabname}_UName";

            TabInfo tabInfo = sqlClient.Context.DMInitalize.GetTabStruct(typeof(H_Test01).Name);
            List<HiColumn> hiColumns = tabInfo.Columns.Where(c => c.FieldName.Equals( "UName",StringComparison.OrdinalIgnoreCase)).ToList();
            var rtn = sqlClient.DbFirst.CreateIndex($"{tabname}", keyidxname, hiColumns, OpLevel.Execute);
            if (rtn.Item1)
            {
                _outputHelper.WriteLine($"向表[{tabname}]新建索引：[{keyidxname}]成功 {System.Environment.NewLine} {rtn.Item3}");
                showIndexList(sqlClient,tabname);
            }
            else
            {
                _outputHelper.WriteLine($"向表[{tabname}]新建索引：[{keyidxname}]失败： {System.Environment.NewLine} {rtn.Item2}{System.Environment.NewLine} {rtn.Item3}");
                _isok = false;
                showIndexList(sqlClient, tabname);
            }


            rtn = sqlClient.DbFirst.DelIndex(tabname, keyidxname, OpLevel.Execute);

            if (rtn.Item1)
            {
                showIndexList(sqlClient, tabname);
                _outputHelper.WriteLine($"向表[{tabname}]删除索引：[{keyidxname}]成功 {System.Environment.NewLine} {rtn.Item3}");
            }
            else
            {
                _outputHelper.WriteLine($"向表[{tabname}]删除索引：[{keyidxname}]失败： {System.Environment.NewLine} {rtn.Item2}{System.Environment.NewLine} {rtn.Item3}");
                _isok = false;
                showIndexList(sqlClient, tabname);
            }

            Assert.True(_isok);
        }

        void reTabName(HiSqlClient sqlClient)
        {
            bool _isok = true;
            string tabname1=typeof(H_Test50C01).Name;
            string newtabname1 = $"{tabname1}_new";


            TabInfo oldtab = sqlClient.Context.DMInitalize.GetTabStruct(tabname1);

            var rtn = sqlClient.DbFirst.ReTable(tabname1, newtabname1, OpLevel.Execute);
            if (rtn.Item1)
            {
                _outputHelper.WriteLine($"表[{tabname1}]重命名为[{newtabname1}]");
                TabInfo tabInfo = sqlClient.Context.DMInitalize.GetTabStruct(newtabname1);

                _outputHelper.WriteLine($"重命名Sql语句:{System.Environment.NewLine}");//输出重命名表 生成的SQL
                _outputHelper.WriteLine(rtn.Item3);

                if (tabInfo.TabModel.TabName.Equals(newtabname1,StringComparison.OrdinalIgnoreCase))
                {
                    _outputHelper.WriteLine($"表[{tabname1}]重命名为[{newtabname1}]  在表结构信息中已经变更");
                }
                else
                {
                    _outputHelper.WriteLine($"表[{tabname1}]重命名为[{newtabname1}]  在表结构信息中已经变更失败");
                    _isok = false;
                }


                _outputHelper.WriteLine($"表[{newtabname1}]重命名为[{tabname1}] 将表重新变更回原来的表");
                rtn = sqlClient.DbFirst.ReTable(newtabname1, tabname1, OpLevel.Execute);

                if (rtn.Item1)
                    _outputHelper.WriteLine("表操作还原成功");
                else
                {
                    _isok = false;
                    _outputHelper.WriteLine("表操作还原失败");
                }


            }
            else
            {
                _outputHelper.WriteLine(rtn.Item2);//输出重命名失败原因
                _isok = false;
            }


            Assert.True(_isok);
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

        void createDemoDynTable(HiSqlClient sqlClient)
        {
            sqlClient.CurrentConnectionConfig.AppEvents = GetAopEvent();

            string tabname1 = "H_dyntab1";

            

            _outputHelper.WriteLine($"检测表[{tabname1}] 是否在当前库中存在");
            if (sqlClient.DbFirst.CheckTabExists(tabname1))
            {
                TabInfo tabInfo2 = sqlClient.DbFirst.GetTabStruct(tabname1);
                _outputHelper.WriteLine($"表[{tabname1}] 存在正在执行删除并清除表结构信息");
                sqlClient.DbFirst.DropTable(tabname1);
                _outputHelper.WriteLine($"表[{tabname1}] 已经删除");
            }

            
            bool iscreate = sqlClient.DbFirst.CreateTable(Test.TestTable.DynTable.BuildTabInfo(tabname1, true));
            if (iscreate)
                _outputHelper.WriteLine($"表[{tabname1}] 已经成功创建");
            else
                _outputHelper.WriteLine($"表[{tabname1}] 创建失败");


            Assert.True(iscreate);


            TabInfo tabInfo= sqlClient.DbFirst.GetTabStruct(tabname1);

            string str = "魏凤和回应佩洛台：中国军队敌人SSSDFADFADSFASDFASDF";

            int len = str.Length;
            int len2 = str.LengthZH();

            List<object> lstdata = TestTable.DynTable.BuildTabDataList(tabname1, 5);


            int v=sqlClient.Insert(tabname1,lstdata).ExecCommand();


        }
        #endregion
    }
}
