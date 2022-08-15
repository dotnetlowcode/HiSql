using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HiSql.DaMengUnitTest
{
    class Demo_DbCode
    {
        public static void Init(HiSqlClient sqlClient)
        {
            //Create_Table(sqlClient); // ok
            Demo_AddColumn(sqlClient); //ok
            //Demo_ReColumn(sqlClient);//ok
            //Demo_ModiColumn(sqlClient); //ok
            //Demo_DelColumn(sqlClient);//ok
            // Demo_Tables(sqlClient);// ok
            // Demo_View(sqlClient);//ok
            //Demo_AllTables(sqlClient);//ok
            //Demo_GlobalTables(sqlClient);//  delay
            //Demo_ModiTable(sqlClient);///ok

            // Demo_DropView(sqlClient); //ok
            //  Demo_CreateView(sqlClient);//ok
            //Demo_ModiView(sqlClient);//ok
            //  Demo_IndexList(sqlClient);///ok
            // Demo_Index_Create(sqlClient);//ok
            //Demo_ReTable(sqlClient);//ok
            // Demo_AllTables(sqlClient);//ok
            // Demo_TableDataCount(sqlClient);//ok
            //Demo_TablesPaging(sqlClient);//ok
            //Demo_ViewsPaging(sqlClient);//ok
            //Demo_AllTablesPaging(sqlClient);//ok
            //Demo_Primary_Create(sqlClient);//ok
        }
        static void Demo_AllTablesPaging(HiSqlClient sqlClient)
        {
            int total = 0;
            List<TableInfo> lsttales = sqlClient.DbFirst.GetAllTables("Hi", 11, 1, out total);
            foreach (TableInfo tableInfo in lsttales)
            {
                Console.WriteLine($"{tableInfo.TabName}  {tableInfo.TabReName}  {tableInfo.TabDescript}  {tableInfo.TableType} 表结构:{tableInfo.HasTabStruct}");
            }
        }
        static void Demo_ViewsPaging(HiSqlClient sqlClient)
        {
            int total = 0;
            List<TableInfo> lsttales = sqlClient.DbFirst.GetViews("HI", 11, 1, out total);
            foreach (TableInfo tableInfo in lsttales)
            {
                Console.WriteLine($"{tableInfo.TabName}  {tableInfo.TabReName}  {tableInfo.TabDescript}  {tableInfo.TableType} 表结构:{tableInfo.HasTabStruct}");
            }
        }

        static void Demo_TablesPaging(HiSqlClient sqlClient)
        {
            int total = 0;
            List<TableInfo> lsttales = sqlClient.DbFirst.GetTables("HI", 2, 2, out total);
            foreach (TableInfo tableInfo in lsttales)
            {
                Console.WriteLine($" {tableInfo.TabName}  {tableInfo.TabReName}  {tableInfo.TabDescript}  {tableInfo.TableType} 表结构:{tableInfo.HasTabStruct}");
            }
            Console.WriteLine($"总数 {total}");

        }
        static void Demo_TableDataCount(HiSqlClient sqlClient)
        {
            int total = 0;
            int lsttales = sqlClient.DbFirst.GetTableDataCount("Hi_FieldModel");
            Console.WriteLine($" {lsttales} ");
        }
        static void Create_Table(HiSqlClient sqlClient)
        {
            sqlClient.CodeFirst.CreateTable(typeof(Table.HTest01));
        }

        static void Demo_ModiTable(HiSqlClient sqlClient)
        {
            //OpLevel.Execute  表示执行并返回生成的SQL
            //OpLevel.Check 表示仅做检测失败时返回消息且检测成功时返因生成的SQL
            var tabinfo = sqlClient.Context.DMInitalize.GetTabStruct("htest01");

            TabInfo _tabcopy = ClassExtensions.DeepCopy<TabInfo>(tabinfo);
            _tabcopy.Columns.RemoveAt(5);

            HiColumn newcol = ClassExtensions.DeepCopy<HiColumn>(_tabcopy.Columns[1]);
            newcol.FieldName = "testaasd59412365";
            newcol.ReFieldName = newcol.FieldName;
            _tabcopy.Columns.Add(newcol);

            //_tabcopy.Columns[1].ReFieldName = "UNAME_01476";
            //_tabcopy.Columns[4].IsRequire = true;

            _tabcopy.PrimaryKey.ForEach(x => {
                x.IsPrimary = false;
            });
            _tabcopy.Columns.ForEach(t => {
                if (t.FieldName == "SID" || t.FieldName == "AGE"
                )
                {
                    t.IsPrimary = true;
                }
            });


            var rtn = sqlClient.DbFirst.ModiTable(_tabcopy, OpLevel.Execute);
            if (rtn.Item1)
            {
                Console.WriteLine(rtn.Item2);//输出成功消息
                Console.WriteLine(rtn.Item3);//输出 生成的SQL
            }
            else
                Console.WriteLine(rtn.Item2);//输出失败原因

        }

        static void Demo_ReTable(HiSqlClient sqlClient)
        {
            //OpLevel.Execute  表示执行并返回生成的SQL
            //OpLevel.Check 表示仅做检测失败时返回消息且检测成功时返因生成的SQL
            var rtn = sqlClient.DbFirst.ReTable("HTEST01", "HTEST012", OpLevel.Execute);
            if (rtn.Item1)
            {
                Console.WriteLine(rtn.Item2);//输出成功消息
                Console.WriteLine(rtn.Item3);//输出重命名表 生成的SQL
            }
            else
                Console.WriteLine(rtn.Item2);//输出重命名失败原因

        }
        static void Demo_Primary_Create(HiSqlClient sqlClient)
        {
            //sqlClient.CodeFirst.CreateTable(typeof(Table.HTest01));
           
            List<TabIndex> lstindex = sqlClient.DbFirst.GetTabIndexs("HTEST012").Where(t => string.Compare(t.IndexType , "Key_Index", true) == 0).ToList();
            foreach (var item in lstindex)
            {
                var rtndel = sqlClient.DbFirst.DelPrimaryKey(item.TabName, OpLevel.Execute);
                if (rtndel.Item1)
                    Console.WriteLine(rtndel.Item3);
                else
                    Console.WriteLine(rtndel.Item2);
            }

            //创建主键
            TabInfo tabInfo = sqlClient.Context.DMInitalize.GetTabStruct("HTEST012");
           
            List<HiColumn> hiColumns = tabInfo.Columns.Where(c => string.Compare(c.FieldName, "ModiTime", true) == 0 || string.Compare(c.FieldName, "ModiName", true) == 0).ToList();

            hiColumns.ForEach((c) => {
                c.IsPrimary = true;
            });
            var rtn = sqlClient.DbFirst.CreatePrimaryKey("HTEST012", hiColumns, OpLevel.Execute);
            if (rtn.Item1)
                Console.WriteLine(rtn.Item3);
            else
                Console.WriteLine(rtn.Item2);

        }
        static void Demo_Index_Create(HiSqlClient sqlClient)
        {


            TabInfo tabInfo = sqlClient.Context.DMInitalize.GetTabStruct("htest01");
            List<HiColumn> hiColumns = tabInfo.Columns.Where(c => string.Compare(c.FieldName, "ModiTime", true) == 0 || string.Compare(c.FieldName, "ModiName", true) == 0).ToList();
            var rtn = sqlClient.DbFirst.CreateIndex("htest01", "idx_hi_test01_ModiTime123", hiColumns, OpLevel.Execute);
            if (rtn.Item1)
                Console.WriteLine(rtn.Item3);
            else
                Console.WriteLine(rtn.Item2);

            rtn = sqlClient.DbFirst.DelIndex("htest01", "idx_hi_test01_ModiTime123", OpLevel.Execute);

            if (rtn.Item1)
                Console.WriteLine(rtn.Item3);
            else
                Console.WriteLine(rtn.Item2);
        }

        static void Demo_IndexList(HiSqlClient sqlClient)
        {
            List<TabIndex> lstindex = sqlClient.DbFirst.GetTabIndexs("HI_DOMAIN");

            foreach (TabIndex tabIndex in lstindex)
            {
                Console.WriteLine($"TabName:{tabIndex.TabName} IndexName:{tabIndex.IndexName} IndexType:{tabIndex.IndexType}");
            }

            List<TabIndexDetail> lstindexdetails = sqlClient.DbFirst.GetTabIndexDetail("HI_DOMAIN", lstindex.FirstOrDefault().IndexName);
            foreach (TabIndexDetail tabIndexDetail in lstindexdetails)
            {
                Console.WriteLine($"TabName:{tabIndexDetail.TabName} IndexName:{tabIndexDetail.IndexName} IndexType:{tabIndexDetail.IndexType} ColumnName:{tabIndexDetail.ColumnName}");

            }
        }

        static void Demo_Tables(HiSqlClient sqlClient)
        {
            List<TableInfo> lsttales = sqlClient.DbFirst.GetTables();
            foreach (TableInfo tableInfo in lsttales)
            {
                Console.WriteLine($"{tableInfo.TabName}  {tableInfo.TabReName}  {tableInfo.TabDescript}  {tableInfo.TableType} 表结构:{tableInfo.HasTabStruct}");
            }
        }

        static void Demo_View(HiSqlClient sqlClient)
        {
            List<TableInfo> lsttales = sqlClient.DbFirst.GetViews();
            foreach (TableInfo tableInfo in lsttales)
            {
                Console.WriteLine($"{tableInfo.TabName}  {tableInfo.TabReName}  {tableInfo.TabDescript}  {tableInfo.TableType} 表结构:{tableInfo.HasTabStruct}");
            }
        }
        static void Demo_GlobalTables(HiSqlClient sqlClient)
        {
            List<TableInfo> lsttales = sqlClient.DbFirst.GetGlobalTempTables();
            foreach (TableInfo tableInfo in lsttales)
            {
                Console.WriteLine($"{tableInfo.TabName}  {tableInfo.TabReName}  {tableInfo.TabDescript}  {tableInfo.TableType} 表结构:{tableInfo.HasTabStruct}");
            }
        }

        static void Demo_AllTables(HiSqlClient sqlClient)
        {
            List<TableInfo> lsttales = sqlClient.DbFirst.GetAllTables();
            foreach (TableInfo tableInfo in lsttales)
            {
                Console.WriteLine($"{tableInfo.TabName}  {tableInfo.TabReName}  {tableInfo.TabDescript}  {tableInfo.TableType} 表结构:{tableInfo.HasTabStruct}");
            }
        }

        static void Demo_CreateView(HiSqlClient sqlClient)
        {
            var rtn = sqlClient.DbFirst.CreateView("vw_Hi_FieldModel_134553",
                sqlClient.HiSql("select a.TabName,b.TabReName,b.TabDescript,a.FieldName,a.SortNum,a.FieldType from Hi_FieldModel as a inner join Hi_TabModel as b on a.TabName=b.TabName").ToSql(),

                OpLevel.Execute);

            Console.WriteLine(rtn.Item2);
            Console.WriteLine(rtn.Item3);
        }

        static void Demo_ModiView(HiSqlClient sqlClient)
        {
            var rtn = sqlClient.DbFirst.ModiView("vw_Hi_FieldModel_134553",
                sqlClient.HiSql("select a.FieldName,a.SortNum,a.FieldType from Hi_FieldModel as a inner join Hi_TabModel as b on a.TabName=b.TabName where b.TabType in (0,1)").ToSql(),

                OpLevel.Execute);

            Console.WriteLine(rtn.Item2);
            Console.WriteLine(rtn.Item3);
        }

        static void Demo_DropView(HiSqlClient sqlClient)
        {
            var rtn = sqlClient.DbFirst.DropView("vw_Hi_FieldModel_134553",

                OpLevel.Execute);

            Console.WriteLine(rtn.Item2);
            Console.WriteLine(rtn.Item3);
        }

        static void Demo_AddColumn(HiSqlClient sqlClient)
        {
            HiColumn column = new HiColumn()
            {
                TabName = "htest01",
                FieldName = "TestAdd1",
                FieldType = HiType.VARCHAR,
                FieldLen = 50,
                DBDefault = HiTypeDBDefault.EMPTY,
                DefaultValue = "123",
                FieldDesc = "测试字段asdf添加aa"

            };
            //          var data = new List<object>();
            //          data.Add(new { TabName = "HTEST01", FieldName = "TESTADD1" });
            //          var a = sqlClient.Context.CloneClient();
            //          a.Delete("Hi_FieldModel", data).ExecCommand();
            //          //a = sqlClient.Context.CloneClient();
            //          //a.Delete("Hi_FieldModel", data).ExecCommand();
            //          //a = sqlClient.Context.CloneClient();
            //          //a.Delete("Hi_FieldModel", data).ExecCommand();

            //         


            //          a.Context.DBO.ExecCommandAsync(@"DECLARE 
            //V_NUMBER INTEGER;
            //          V_SECOUT INTEGER;
            //          BEGIN
            //             SELECT COUNT(*)  INTO V_NUMBER  FROM USER_TABLES  WHERE TABLE_NAME = UPPER('TMP_LOCAL_HI_FIELDMODEL_1_1664527640') AND TEMPORARY = 'Y';
            //          IF V_NUMBER > 0 THEN
            //             EXECUTE IMMEDIATE 'TRUNCATE TABLE TMP_LOCAL_HI_FIELDMODEL_1_1664527640 ';
            //              EXECUTE IMMEDIATE 'DROP TABLE TMP_LOCAL_HI_FIELDMODEL_1_1664527640 ';
            //          END IF;

            //          EXECUTE IMMEDIATE  'CREATE  GLOBAL TEMPORARY TABLE TMP_LOCAL_HI_FIELDMODEL_1_1664527640('
            //                || '""TABNAME""  NVARCHAR2 (50) NOT NULL   ,'
            //         || '""FIELDNAME""  NVARCHAR2 (50) NOT NULL   ,'
            //         || '""FIELDDESC""  NVARCHAR2 (100)   DEFAULT '''' ,'
            //         || '""ISIDENTITY""  NUMBER(1)     DEFAULT 0  ,'
            //         || '""ISPRIMARY""  NUMBER(1)     DEFAULT 0  ,'
            //         || '""ISBLLKEY""  NUMBER(1)     DEFAULT 0  ,'
            //         || '""FIELDTYPE""  INTEGER    DEFAULT 0  ,'
            //         || '""SORTNUM""  INTEGER    DEFAULT 0  ,'
            //         || '""REGEX""  NVARCHAR2 (200)   DEFAULT '''' ,'
            //         || '""DBDEFAULT""  INTEGER    DEFAULT 0  ,'
            //         || '""DEFAULTVALUE""  NVARCHAR2 (50)   DEFAULT '''' ,'
            //         || '""FIELDLEN""  INTEGER    DEFAULT 0  ,'
            //         || '""FIELDDEC""  INTEGER    DEFAULT 0  ,'
            //         || '""SNO""  NCHAR (10)  DEFAULT ''''  ,'
            //         || '""SNO_NUM""  NCHAR (3)  DEFAULT ''''  ,'
            //         || '""ISSYS""  NUMBER(1)     DEFAULT 0  ,'
            //         || '""ISNULL""  NUMBER(1)     DEFAULT 0  ,'
            //         || '""ISREQUIRE""  NUMBER(1)     DEFAULT 0  ,'
            //         || '""ISIGNORE""  NUMBER(1)     DEFAULT 0  ,'
            //         || '""ISOBSOLETE""  NUMBER(1)     DEFAULT 0  ,'
            //         || '""ISSHOW""  NUMBER(1)     DEFAULT 0  ,'
            //         || '""ISSEARCH""  NUMBER(1)     DEFAULT 0  ,'
            //         || '""SRCHMODE""  INTEGER    DEFAULT 0  ,'
            //         || '""ISREFTAB""  NUMBER(1)     DEFAULT 0  ,'
            //         || '""REFTAB""  NVARCHAR2 (50)   DEFAULT '''' ,'
            //         || '""REFFIELD""  NVARCHAR2 (50)   DEFAULT '''' ,'
            //         || '""REFFIELDS""  NVARCHAR2 (500)   DEFAULT '''' ,'
            //         || '""REFFIELDDESC""  NVARCHAR2 (500)   DEFAULT '''' ,'
            //         || '""REFWHERE""  NVARCHAR2 (500)   DEFAULT '''' ,'
            //         || '""CREATETIME""  TIMESTAMP     DEFAULT SYSTIMESTAMP  ,'
            //         || '""CREATENAME""  NVARCHAR2 (50)   DEFAULT '''' ,'
            //         || '""MODITIME""  TIMESTAMP     DEFAULT SYSTIMESTAMP  ,'
            //         || '""MODINAME""  NVARCHAR2 (50)   DEFAULT '''' '

            //          || ')ON COMMIT PRESERVE ROWS ';
            //          EXECUTE IMMEDIATE 'ALTER TABLE TMP_LOCAL_HI_FIELDMODEL_1_1664527640  ADD CONSTRAINT PK_TMP_LOCAL_HI_FIELDMODEL_1_1664527640_D027E9F5BDC64CCBA7C2FC04FB3BAA00 PRIMARY KEY (""TABNAME"" ,""FIELDNAME"" )';
            //          END;");

            //          // a.Context.DBO.Close();

            //          var aa = sqlClient.Context.CloneClient();

            //          aa.Context.DBO.ExecCommandAsync(@"DECLARE 
            //V_NUMBER INTEGER;
            //          V_SECOUT INTEGER;
            //          BEGIN
            //             SELECT COUNT(*)  INTO V_NUMBER  FROM USER_TABLES  WHERE TABLE_NAME = UPPER('TMP_LOCAL_HI_FIELDMODEL_1_1664527643') AND TEMPORARY = 'Y';
            //          IF V_NUMBER > 0 THEN
            //             EXECUTE IMMEDIATE 'TRUNCATE TABLE TMP_LOCAL_HI_FIELDMODEL_1_1664527643 ';
            //              EXECUTE IMMEDIATE 'DROP TABLE TMP_LOCAL_HI_FIELDMODEL_1_1664527643 ';
            //          END IF;

            //          EXECUTE IMMEDIATE  'CREATE  GLOBAL TEMPORARY TABLE TMP_LOCAL_HI_FIELDMODEL_1_1664527643('
            //                || '""TABNAME""  NVARCHAR2 (50) NOT NULL   ,'
            //         || '""FIELDNAME""  NVARCHAR2 (50) NOT NULL   ,'
            //         || '""FIELDDESC""  NVARCHAR2 (100)   DEFAULT '''' ,'
            //         || '""ISIDENTITY""  NUMBER(1)     DEFAULT 0  ,'
            //         || '""ISPRIMARY""  NUMBER(1)     DEFAULT 0  ,'
            //         || '""ISBLLKEY""  NUMBER(1)     DEFAULT 0  ,'
            //         || '""FIELDTYPE""  INTEGER    DEFAULT 0  ,'
            //         || '""SORTNUM""  INTEGER    DEFAULT 0  ,'
            //         || '""REGEX""  NVARCHAR2 (200)   DEFAULT '''' ,'
            //         || '""DBDEFAULT""  INTEGER    DEFAULT 0  ,'
            //         || '""DEFAULTVALUE""  NVARCHAR2 (50)   DEFAULT '''' ,'
            //         || '""FIELDLEN""  INTEGER    DEFAULT 0  ,'
            //         || '""FIELDDEC""  INTEGER    DEFAULT 0  ,'
            //         || '""SNO""  NCHAR (10)  DEFAULT ''''  ,'
            //         || '""SNO_NUM""  NCHAR (3)  DEFAULT ''''  ,'
            //         || '""ISSYS""  NUMBER(1)     DEFAULT 0  ,'
            //         || '""ISNULL""  NUMBER(1)     DEFAULT 0  ,'
            //         || '""ISREQUIRE""  NUMBER(1)     DEFAULT 0  ,'
            //         || '""ISIGNORE""  NUMBER(1)     DEFAULT 0  ,'
            //         || '""ISOBSOLETE""  NUMBER(1)     DEFAULT 0  ,'
            //         || '""ISSHOW""  NUMBER(1)     DEFAULT 0  ,'
            //         || '""ISSEARCH""  NUMBER(1)     DEFAULT 0  ,'
            //         || '""SRCHMODE""  INTEGER    DEFAULT 0  ,'
            //         || '""ISREFTAB""  NUMBER(1)     DEFAULT 0  ,'
            //         || '""REFTAB""  NVARCHAR2 (50)   DEFAULT '''' ,'
            //         || '""REFFIELD""  NVARCHAR2 (50)   DEFAULT '''' ,'
            //         || '""REFFIELDS""  NVARCHAR2 (500)   DEFAULT '''' ,'
            //         || '""REFFIELDDESC""  NVARCHAR2 (500)   DEFAULT '''' ,'
            //         || '""REFWHERE""  NVARCHAR2 (500)   DEFAULT '''' ,'
            //         || '""CREATETIME""  TIMESTAMP     DEFAULT SYSTIMESTAMP  ,'
            //         || '""CREATENAME""  NVARCHAR2 (50)   DEFAULT '''' ,'
            //         || '""MODITIME""  TIMESTAMP     DEFAULT SYSTIMESTAMP  ,'
            //         || '""MODINAME""  NVARCHAR2 (50)   DEFAULT '''' '

            //          || ')ON COMMIT PRESERVE ROWS ';
            //          EXECUTE IMMEDIATE 'ALTER TABLE TMP_LOCAL_HI_FIELDMODEL_1_1664527643  ADD CONSTRAINT PK_TMP_LOCAL_HI_FIELDMODEL_1_1664527643_D027E9F5BDC64CCBA7C2FC04FB3BAA00 PRIMARY KEY (""TABNAME"" ,""FIELDNAME"" )';
            //          END;");
            //          //aa.Context.DBO.Close();
            //          a = sqlClient.Context.CloneClient();
            //          a.Delete("Hi_FieldModel", data).ExecCommand();
            var rtn = sqlClient.DbFirst.AddColumn("htest01", column, OpLevel.Execute);
            Console.WriteLine(rtn.Item2);
        }

        static void Demo_DelColumn(HiSqlClient sqlClient)
        {
            //var data = new List<object>();
            //data.Add(new { TabName = "HTEST01", FieldName = "TESTADD1" });
            //var a = sqlClient.Context.CloneClient();
            //a.Delete("Hi_FieldModel", data).ExecCommand();
            Console.WriteLine("开始del");
            HiColumn column = new HiColumn()
            {
                TabName = "htest01",
                FieldName = "TestAdd1",
                FieldType = HiType.VARCHAR,
                FieldLen = 50,
                DBDefault = HiTypeDBDefault.EMPTY

            };

            var rtn = sqlClient.DbFirst.DelColumn("htest01", column, OpLevel.Execute);

            Console.WriteLine(rtn.Item2);
        }

        static void Demo_ModiColumn(HiSqlClient sqlClient)
        {
            HiColumn column = new HiColumn()
            {
                TabName = "htest01",
                FieldName = "MODINAME",
                FieldType = HiType.VARCHAR,
                FieldLen = 500,
                DBDefault = HiTypeDBDefault.VALUE,
                DefaultValue = "TGM",
                FieldDesc = "测试字asdfasdf asd fa段sdf 修改"

            };

            var rtn = sqlClient.DbFirst.ModiColumn(column.TabName, column, OpLevel.Execute);

            Console.WriteLine(rtn.Item2);
            Console.WriteLine(rtn.Item3);



            Console.WriteLine(rtn.Item2);
        }

        static void Demo_ReColumn(HiSqlClient sqlClient)
        {
            //OpLevel.Execute  表示执行并返回生成的SQL
            //OpLevel.Check 表示仅做检测失败时返回消息且检测成功时返因生成的SQL
            HiColumn column = new HiColumn()
            {
                TabName = "htest01",
                FieldName = "UName",
                ReFieldName = "UName_01",
                FieldType = HiType.VARCHAR,
                FieldLen = 500,
                DBDefault = HiTypeDBDefault.VALUE,
                DefaultValue = "TGM",
                FieldDesc = "测试字段变asdfa sdqq更"

            };

            var rtn = sqlClient.DbFirst.ReColumn(column.TabName, column, OpLevel.Execute);

            column = new HiColumn()
            {
                TabName = "htest01",
                FieldName = "UName_01",
                ReFieldName = "UName",
                FieldType = HiType.VARCHAR,
                FieldLen = 500,
                DBDefault = HiTypeDBDefault.VALUE,
                DefaultValue = "TGM",
                FieldDesc = "测试字段变asdfa sdqq更"

            };
            rtn = sqlClient.DbFirst.ReColumn(column.TabName, column, OpLevel.Execute);

            column = new HiColumn()
            {
                TabName = "htest01",
                FieldName = "UName",
                ReFieldName = "UName_01",
                FieldType = HiType.VARCHAR,
                FieldLen = 500,
                DBDefault = HiTypeDBDefault.VALUE,
                DefaultValue = "TGM",
                FieldDesc = "测试字段变asdfa sdqq更"

            };
            rtn = sqlClient.DbFirst.ReColumn(column.TabName, column, OpLevel.Execute);
            column = new HiColumn()
            {
                TabName = "htest01",
                FieldName = "UName_01",
                ReFieldName = "UName",
                FieldType = HiType.VARCHAR,
                FieldLen = 500,
                DBDefault = HiTypeDBDefault.VALUE,
                DefaultValue = "TGM",
                FieldDesc = "测试字段变asdfa sdqq更"

            };
            rtn = sqlClient.DbFirst.ReColumn(column.TabName, column, OpLevel.Execute);

            if (rtn.Item1)
            {
                Console.WriteLine(rtn.Item2);//输出成功消息
                Console.WriteLine(rtn.Item3);//输出 生成的SQL
            }
            else
                Console.WriteLine(rtn.Item2);//输出失败原因
        }
    }
}
