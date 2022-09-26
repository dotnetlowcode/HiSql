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
            createTempTable(sqlClient, "Hi_Test_createTempTable");
            //dropTableAndTruncate(sqlClient, "Hi_Test_dropTableAndTruncate");

            ////表重命名
            //reTabName(sqlClient, "Hi_Test_reTabName");

            //reCol(sqlClient, "Hi_Test_dyntab1reCol", true);
            //reCol(sqlClient, "Hi_Test_dyntab1reCol3", false);

            // moditable(sqlClient, "Hi_Test_dyntab1moditable");

            //moditableCreatePrimaryKey(sqlClient, "Hi_TestCreatePrimaryKey");

            //////索引创建 删除修改
            //indexDemo(sqlClient, "Hi_Test_indexDemo");

        }
        void moditableCreatePrimaryKey(HiSqlClient sqlClient, string tabname)
        {
            //无主键表创建主键
            if (sqlClient.DbFirst.CheckTabExists(tabname))
            {
                sqlClient.DbFirst.DropTable(tabname);
            }
            bool iscreate = sqlClient.DbFirst.CreateTable(Test.TestTable.DynTable.BuildTabInfo(tabname, true, false));
            TestTable.DynTable.BuildTabDataList(tabname, 50);
            TabInfo tabinfo = DataConvert.CloneTabInfo(sqlClient.DbFirst.GetTabStruct(tabname));

            //无主键表新增主键
            HiColumn col = tabinfo.GetColumns.Where(c => c.FieldName.Equals("Uid", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            col.IsPrimary = true;
            col.IsBllKey = true;
            var rtn = sqlClient.DbFirst.ModiTable(tabinfo, OpLevel.Execute);

            tabinfo = DataConvert.CloneTabInfo(sqlClient.DbFirst.GetTabStruct(tabname));

            if (rtn.Item1 && tabinfo.PrimaryKey.Count == 0)
            {
                _outputHelper.WriteLine($"无主键表创建主键-表修改结果：{rtn.Item2}");
                _outputHelper.WriteLine($"无主键表创建主键-表修改sql：{rtn.Item3}");
            }
            else
            {
                _outputHelper.WriteLine($"无主键表创建主键-表修改结果：{rtn.Item2}");
                _outputHelper.WriteLine($"无主键表创建主键-表修改sql：{rtn.Item3}");
            }

            //删除主键

            tabinfo = DataConvert.CloneTabInfo(sqlClient.DbFirst.GetTabStruct(tabname));
            col = tabinfo.GetColumns.Where(c => c.FieldName.Equals("Uid", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            col.IsPrimary = false;
            col.IsBllKey = false;
            rtn = sqlClient.DbFirst.ModiTable(tabinfo, OpLevel.Execute);

            tabinfo = DataConvert.CloneTabInfo(sqlClient.DbFirst.GetTabStruct(tabname));

            if (rtn.Item1  && tabinfo.PrimaryKey.Count == 0)
            {
                _outputHelper.WriteLine($"主键表删除主键-表修改结果：{rtn.Item2}");
                _outputHelper.WriteLine($"主键表删除主键-表修改sql：{rtn.Item3}");
            }
            else
            {
                _outputHelper.WriteLine($"主键表删除主键-表修改结果：{rtn.Item2}");
                _outputHelper.WriteLine($"主键表删除主键-表修改sql：{rtn.Item3}");
            }


        }

        /// <summary>
        /// 字段类型修改
        /// </summary>
        /// <param name="sqlClient"></param>
        /// <param name="tabname"></param>
        void moditable(HiSqlClient sqlClient, string tabname)
        {
            //创建表
            if (sqlClient.DbFirst.CheckTabExists(tabname))
            {
                sqlClient.DbFirst.DropTable(tabname);
            }
            bool iscreate = sqlClient.DbFirst.CreateTable(Test.TestTable.DynTable.BuildTabInfo(tabname, true));
            TestTable.DynTable.BuildTabDataList(tabname, 50);

            TabInfo tabinfo = sqlClient.DbFirst.GetTabStruct(tabname);

            TabInfo newtabinfo = tabinfo.CloneCopy();

            HiColumn col = newtabinfo.GetColumns.Where(c => c.FieldName.Equals("Unchar", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
           
            col.FieldType = HiType.NVARCHAR;
            col.FieldLen = 300;
            col.SortNum = 2;
            col.Regex = "\\s*";
            col.IsNull = !col.IsNull;
            col.IsSearch = !col.IsSearch;
            
            var rtn =sqlClient.DbFirst.ModiTable(newtabinfo,OpLevel.Execute);
            var tabinfoNew = sqlClient.DbFirst.GetTabStruct(tabname);

            HiColumn newcol = tabinfo.GetColumns.Where(c => c.FieldName.Equals("Unchar", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (rtn.Item1
                && newcol.SortNum == col.SortNum
                    && newcol.FieldLen == col.FieldLen
                    && newcol.FieldType == col.FieldType
                      && newcol.Regex == col.Regex
                       && newcol.IsSearch == col.IsSearch
                        && newcol.IsNull == col.IsNull
                )
            {
                _outputHelper.WriteLine($"表修改结果：{rtn.Item2}");
                _outputHelper.WriteLine($"表修改sql：{rtn.Item3}");

                Assert.True(true);
            }
            else
            {
                Assert.True(false);
                _outputHelper.WriteLine($"表修改结果：{rtn.Item2}");
                _outputHelper.WriteLine($"表修改sql：{rtn.Item3}");
            }

            //删除字段
            tabinfo = DataConvert.CloneTabInfo(sqlClient.DbFirst.GetTabStruct(tabname));
            tabinfo.Columns.RemoveAll(c=> c.FieldName.Equals("Unchar", StringComparison.OrdinalIgnoreCase) || c.FieldName.Equals("Uint", StringComparison.OrdinalIgnoreCase));
            rtn = sqlClient.DbFirst.ModiTable(tabinfo, OpLevel.Execute);

            tabinfoNew = sqlClient.DbFirst.GetTabStruct(tabname);

            Assert.True(rtn.Item1 && tabinfo.Columns.Count == tabinfoNew.Columns.Count );
            if (rtn.Item1  && tabinfo.Columns.Count == tabinfoNew.Columns.Count)
            {
                _outputHelper.WriteLine($"表修改-删除字段-结果：{rtn.Item2}");
                _outputHelper.WriteLine($"表修改-删除字段-sql：{rtn.Item3}");
            }
            else
            {
                _outputHelper.WriteLine($"表修改-删除字段-结果：{rtn.Item2}");
                _outputHelper.WriteLine($"表修改-删除字段-sql：{rtn.Item3}");
            }

            //修改表的扩展字段信息
            tabinfo = DataConvert.CloneTabInfo(sqlClient.DbFirst.GetTabStruct(tabname));
            tabinfo.TabModel.TabReName = tabinfo.TabModel.TabReName + "1";
            tabinfo.TabModel.DbName = tabinfo.TabModel.DbName + "1";
            tabinfo.TabModel.DbServer = tabinfo.TabModel.DbServer + "1";
            tabinfo.TabModel.TabDescript = tabinfo.TabModel.TabDescript + "1";
            tabinfo.TabModel.TabCacheType =  TabCacheType.ALL;
            tabinfo.TabModel.IsLog = !tabinfo.TabModel.IsLog;
            rtn = sqlClient.DbFirst.ModiTable(tabinfo, OpLevel.Execute);

            tabinfoNew = sqlClient.DbFirst.GetTabStruct(tabname);
            Assert.True(rtn.Item1 &&
                tabinfoNew.TabModel.TabReName == tabinfo.TabModel.TabReName &&
                tabinfoNew.TabModel.DbName == tabinfo.TabModel.DbName &&
                tabinfoNew.TabModel.DbServer == tabinfo.TabModel.DbServer &&
                tabinfoNew.TabModel.TabDescript == tabinfo.TabModel.TabDescript &&
                tabinfoNew.TabModel.TabCacheType == tabinfo.TabModel.TabCacheType &&
                 tabinfoNew.TabModel.IsLog == tabinfo.TabModel.IsLog
                );

            //修改表的列的扩展信息
            tabinfo = DataConvert.CloneTabInfo(sqlClient.DbFirst.GetTabStruct(tabname));
            tabinfo.TabModel.TabReName = tabinfo.TabModel.TabReName + "1";
            tabinfo.TabModel.DbName = tabinfo.TabModel.DbName + "1";
            tabinfo.TabModel.DbServer = tabinfo.TabModel.DbServer + "1";
            tabinfo.TabModel.TabDescript = tabinfo.TabModel.TabDescript + "1";
            tabinfo.TabModel.TabCacheType = TabCacheType.ALL;
            tabinfo.TabModel.IsLog = !tabinfo.TabModel.IsLog;
            rtn = sqlClient.DbFirst.ModiTable(tabinfo, OpLevel.Execute);

            tabinfoNew = sqlClient.DbFirst.GetTabStruct(tabname);
            Assert.True(rtn.Item1 &&
                tabinfoNew.TabModel.TabReName == tabinfo.TabModel.TabReName &&
                tabinfoNew.TabModel.DbName == tabinfo.TabModel.DbName &&
                tabinfoNew.TabModel.DbServer == tabinfo.TabModel.DbServer &&
                tabinfoNew.TabModel.TabDescript == tabinfo.TabModel.TabDescript &&
                tabinfoNew.TabModel.TabCacheType == tabinfo.TabModel.TabCacheType &&
                 tabinfoNew.TabModel.IsLog == tabinfo.TabModel.IsLog
                );
        }

        /// <summary>
        /// 字段重命名  表重命名
        /// </summary>
        /// <param name="sqlClient"></param>
        /// <param name="tabname"></param>
        void reCol(HiSqlClient sqlClient, string tabname, bool inserData)
        {
            //创建表
            if (sqlClient.DbFirst.CheckTabExists(tabname))
            {
                sqlClient.DbFirst.DropTable(tabname);
            }
            bool iscreate = sqlClient.DbFirst.CreateTable(Test.TestTable.DynTable.BuildTabInfo(tabname, true));
            if (inserData)
            {
                int v = sqlClient.Insert(tabname, TestTable.DynTable.BuildTabDataList(tabname, 50)).ExecCommand();
            }
            

            string fieldname = "Unchar";
            string newfieldname = "Unchar2";
            _outputHelper.WriteLine($"正在修改表[{tabname}]中的字段 将字段[{fieldname}] 改为[{newfieldname}]");

            TabInfo tabinfo= sqlClient.DbFirst.GetTabStruct(tabname);

            HiColumn hiColumn=tabinfo.Columns.Where(c=>c.FieldName.Equals(fieldname, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            hiColumn.FieldDesc = "测试Unchar字段 修改";
            hiColumn.ReFieldName = newfieldname;

            var tuple= sqlClient.DbFirst.ReColumn(tabname, hiColumn, OpLevel.Execute);

            if (tuple.Item1)
            {
                TabInfo tabinfo2 = sqlClient.DbFirst.GetTabStruct(tabname);

                _outputHelper.WriteLine($"将字段[{fieldname}] 改为[{newfieldname}] 成功");
                HiColumn hiColumn2 = tabinfo2.Columns.Where(c => c.FieldName.Equals(newfieldname, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                hiColumn2.FieldDesc = "测试Unchar字段";
                hiColumn2.ReFieldName = fieldname;
                var tuple2 = sqlClient.DbFirst.ReColumn(tabname, hiColumn2, OpLevel.Execute);
                if (tuple2.Item1)
                {
                    _outputHelper.WriteLine($"将字段[{newfieldname}] 改为[{fieldname}] 成功");

                    _outputHelper.WriteLine($" msg:{tuple2.Item2}");
                    _outputHelper.WriteLine($"  sql:{tuple2.Item3}");

                    Assert.True(true);
                }
                else
                {

                    _outputHelper.WriteLine($"字段改回失败:{tuple2.Item2}");

                    _outputHelper.WriteLine($"  sql:{tuple2.Item3}");

                    Assert.True(false);
                }
            }
            else
            {

                _outputHelper.WriteLine($"修改失败:{tuple.Item2}");

                _outputHelper.WriteLine($"  sql:{tuple.Item3}");

                Assert.True(false);
            }

        }


        void dropTableAndTruncate(HiSqlClient sqlClient, string tabname)
        {
            //创建表
            if (sqlClient.DbFirst.CheckTabExists(tabname))
            {
                sqlClient.DbFirst.DropTable(tabname);
            }
            bool iscreate = sqlClient.DbFirst.CreateTable(Test.TestTable.DynTable.BuildTabInfo(tabname, true));

            int v = sqlClient.Insert(tabname, TestTable.DynTable.BuildTabDataList(tabname, 50)).ExecCommand();

            _outputHelper.WriteLine($"正在清除表[{tabname}]中所有数据");
            var resultTruncate = sqlClient.DbFirst.Truncate(tabname);
            var dataCount = sqlClient.DbFirst.GetTableDataCount(tabname);
            Assert.True(resultTruncate && dataCount == 0);

            sqlClient.DbFirst.DropTable(tabname);
            Assert.True(!sqlClient.DbFirst.CheckTabExists(tabname));

        }
        void createTempTable(HiSqlClient sqlClient, string tabname)
        {
            //创建表
            if (sqlClient.DbFirst.CheckTabExists(tabname))
            {
                sqlClient.DbFirst.DropTable(tabname);
            }
            bool iscreate = sqlClient.DbFirst.CreateTable(Test.TestTable.DynTable.BuildTabInfo(tabname, true));
            int v = sqlClient.Insert(tabname, TestTable.DynTable.BuildTabDataList(tabname, 50)).ExecCommand();

            Assert.True(sqlClient.DbFirst.CheckTabExists(tabname));
            var temptabname = "#" + tabname;
            sqlClient.Query(tabname).Field("*").Insert(temptabname);

            //tabname = "#" + tabname;
            //if (sqlClient.DbFirst.CheckTabExists(tabname))
            //{
            //    sqlClient.DbFirst.DropTable(tabname);
            //}
           
            //插入临时表后，无法查出
           var tempData = sqlClient.Query(temptabname).Field("*").ToTable();


            //Assert.True(tempData.Rows.Count == 50);

        }


        void showIndexList(HiSqlClient sqlClient,string tabname)
        {
            List<TabIndex> lstindex = sqlClient.DbFirst.GetTabIndexs(tabname);
            foreach (TabIndex tabIndex in lstindex)
            {
                _outputHelper.WriteLine($"TabName:{tabIndex.TabName} IndexName:{tabIndex.IndexName} IndexType:{tabIndex.IndexType}");
            }
        }

        void indexDemo(HiSqlClient sqlClient, string tabname)
        {
            //创建表
            if (sqlClient.DbFirst.CheckTabExists(tabname))
            {
                sqlClient.DbFirst.DropTable(tabname);
            }
            bool iscreate = sqlClient.DbFirst.CreateTable(Test.TestTable.DynTable.BuildTabInfo(tabname, true));
            int v = sqlClient.Insert(tabname, TestTable.DynTable.BuildTabDataList(tabname, 50)).ExecCommand();

            bool _isok = true;
            string keyidxname = $"{tabname}_UName";

            TabInfo tabInfo = sqlClient.Context.DMInitalize.GetTabStruct(tabname);


            List<HiColumn> hiColumns = tabInfo.Columns.Where(c => c.FieldName.Equals("Uvarchar", StringComparison.OrdinalIgnoreCase) || c.FieldName.Equals("Uint", StringComparison.OrdinalIgnoreCase)).ToList();
            var rtn = sqlClient.DbFirst.CreateIndex($"{tabname}", keyidxname, hiColumns, OpLevel.Execute);


            List<TabIndex> lstindex = sqlClient.DbFirst.GetTabIndexs(tabname);

            if (rtn.Item1 && lstindex.Any(t=> t.IndexName == keyidxname))
            {
                _outputHelper.WriteLine($"向表[{tabname}]新建索引：[{keyidxname}]成功 {System.Environment.NewLine} {rtn.Item3}");
          
            }
            else
            {
                _outputHelper.WriteLine($"向表[{tabname}]新建索引：[{keyidxname}]失败： {System.Environment.NewLine} {rtn.Item2}{System.Environment.NewLine} {rtn.Item3}");
                _isok = false;
            }

            showIndexList(sqlClient, tabname);
            rtn = sqlClient.DbFirst.DelIndex(tabname, keyidxname, OpLevel.Execute);
            lstindex = sqlClient.DbFirst.GetTabIndexs(tabname);
            if (rtn.Item1 && !lstindex.Any(t => t.IndexName == keyidxname))
            {
               
                _outputHelper.WriteLine($"向表[{tabname}]删除索引：[{keyidxname}]成功 {System.Environment.NewLine} {rtn.Item3}");
            }
            else
            {
                _outputHelper.WriteLine($"向表[{tabname}]删除索引：[{keyidxname}]失败： {System.Environment.NewLine} {rtn.Item2}{System.Environment.NewLine} {rtn.Item3}");
                _isok = false;
               
            }
            showIndexList(sqlClient, tabname);

            Assert.True(_isok);
        }

        void reTabName(HiSqlClient sqlClient, string tabname)
        {
            bool _isok = true;

            string newtabname1 = $"{tabname}_new";
           
           //创建表
            if (sqlClient.DbFirst.CheckTabExists(tabname))
            {
                sqlClient.DbFirst.DropTable(tabname);
            }
            bool iscreate = sqlClient.DbFirst.CreateTable(Test.TestTable.DynTable.BuildTabInfo(tabname, true));
            int v = sqlClient.Insert(tabname, TestTable.DynTable.BuildTabDataList(tabname, 50)).ExecCommand();

            TabInfo oldtab = sqlClient.Context.DMInitalize.GetTabStruct(tabname);

            var rtn = sqlClient.DbFirst.ReTable(tabname, newtabname1, OpLevel.Execute);
            if (rtn.Item1)
            {
                _outputHelper.WriteLine($"表[{tabname}]重命名为[{newtabname1}]");
                TabInfo tabInfo = sqlClient.Context.DMInitalize.GetTabStruct(newtabname1);

                _outputHelper.WriteLine($"重命名Sql语句:{System.Environment.NewLine}");//输出重命名表 生成的SQL
                _outputHelper.WriteLine(rtn.Item3);

                if (tabInfo.TabModel.TabName.Equals(newtabname1,StringComparison.OrdinalIgnoreCase))
                {
                    _outputHelper.WriteLine($"表[{tabname}]重命名为[{newtabname1}]  在表结构信息中已经变更");
                }
                else
                {
                    _outputHelper.WriteLine($"表[{tabname}]重命名为[{newtabname1}]  在表结构信息中已经变更失败");
                    _isok = false;
                }


                _outputHelper.WriteLine($"表[{newtabname1}]重命名为[{tabname}] 将表重新变更回原来的表");
                rtn = sqlClient.DbFirst.ReTable(newtabname1, tabname, OpLevel.Execute);

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

        void createDemoDynTable(HiSqlClient sqlClient, string tabname1)
        {
            sqlClient.CurrentConnectionConfig.AppEvents = GetAopEvent();

            
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

            List<object> lstdata = TestTable.DynTable.BuildTabDataList(tabname1, 500);

            int v = sqlClient.Insert(tabname1, lstdata).ExecCommand();


        }
        #endregion
    }
}
