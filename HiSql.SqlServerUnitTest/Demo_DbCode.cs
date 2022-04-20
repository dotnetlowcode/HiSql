using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    class Demo_DbCode
    {
        public static void Init(HiSqlClient sqlClient)
        {

            Console.WriteLine("表操作测试");
           Demo_AddColumn(sqlClient);

            //Demo_ModiColumn(sqlClient);
            //Demo_ReColumn(sqlClient);
            Demo_ModiTable(sqlClient);
            //Demo_ReTable(sqlClient);
            //Demo_DelColumn(sqlClient);
            //Demo_Tables(sqlClient);
            //Demo_View(sqlClient);
            //Demo_ViewsPaging(sqlClient);
            //Demo_AllTables(sqlClient);
            //Demo_TablesPaging(sqlClient);
            //Demo_TableDataCount(sqlClient);
            //Demo_GlobalTables(sqlClient);
            //Demo_DropView(sqlClient);
            //Demo_CreateView(sqlClient);
            //Demo_ModiView(sqlClient);
            //Demo_IndexList(sqlClient);
            //Demo_Index_Create(sqlClient);

           // Demo_AllTablesPaging(sqlClient);
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
            List<TableInfo> lsttales = sqlClient.DbFirst.GetViews("FieldModel", 11, 1, out total);
            foreach (TableInfo tableInfo in lsttales)
            {
                Console.WriteLine($"{tableInfo.TabName}  {tableInfo.TabReName}  {tableInfo.TabDescript}  {tableInfo.TableType} 表结构:{tableInfo.HasTabStruct}");
            }
        }
        static void Demo_ReTable(HiSqlClient sqlClient)
        {
            //OpLevel.Execute  表示执行并返回生成的SQL
            //OpLevel.Check 表示仅做检测失败时返回消息且检测成功时返因生成的SQL
            var rtn = sqlClient.DbFirst.ReTable("H_Test5", "H_Test5_1", OpLevel.Execute);
            if (rtn.Item1)
            {
                Console.WriteLine(rtn.Item2);//输出成功消息
                Console.WriteLine(rtn.Item3);//输出重命名表 生成的SQL
            }
            else
                Console.WriteLine(rtn.Item2);//输出重命名失败原因
            
        }
        static void Demo_Index_Create(HiSqlClient sqlClient)
        {


            TabInfo tabInfo = sqlClient.Context.DMInitalize.GetTabStruct("H04_OrderInfo");
            List<HiColumn> hiColumns = tabInfo.Columns.Where(c => c.FieldName == "POSOrderID").ToList();
            var rtn = sqlClient.DbFirst.CreateIndex("H04_OrderInfo", "H04_OrderInfo_POSOrderID", hiColumns, OpLevel.Execute);
            if (rtn.Item1)
                Console.WriteLine(rtn.Item3);
            else
                Console.WriteLine(rtn.Item2);


            rtn = sqlClient.DbFirst.DelIndex("H04_OrderInfo", "H04_OrderInfo_POSOrderID",OpLevel.Execute);

            if (rtn.Item1)
                Console.WriteLine(rtn.Item3);
            else
                Console.WriteLine(rtn.Item2);
        }

        static void Demo_IndexList(HiSqlClient sqlClient)
        {
            List<TabIndex> lstindex = sqlClient.DbFirst.GetTabIndexs("Hi_FieldModel");

            foreach (TabIndex tabIndex in lstindex)
            {
                Console.WriteLine($"TabName:{tabIndex.TabName} IndexName:{tabIndex.IndexName} IndexType:{tabIndex.IndexType}");
            }

            List<TabIndexDetail> lstindexdetails = sqlClient.DbFirst.GetTabIndexDetail("Hi_FieldModel","PK_Hi_FieldModel_ed721f6b-296a-447e-ac67-7d02fd8e338c");
            foreach (TabIndexDetail tabIndexDetail in lstindexdetails)
            {
                Console.WriteLine($"TabName:{tabIndexDetail.TabName} IndexName:{tabIndexDetail.IndexName} IndexType:{tabIndexDetail.IndexType} ColumnName:{tabIndexDetail.ColumnName}");
               
            }
        }

        static void Demo_Tables(HiSqlClient sqlClient)
        {
            List<TableInfo> lsttales = sqlClient.DbFirst.GetTables();
            foreach (  TableInfo tableInfo in lsttales)
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
        static void Demo_TablesPaging(HiSqlClient sqlClient)
        {
            int total = 0;
            List<TableInfo> lsttales = sqlClient.DbFirst.GetTables("HI", 11,1, out total);
            foreach (TableInfo tableInfo in lsttales)
            {
                Console.WriteLine($"{tableInfo.TabName}  {tableInfo.TabReName}  {tableInfo.TabDescript}  {tableInfo.TableType} 表结构:{tableInfo.HasTabStruct}");
            }
        }
        static void Demo_TableDataCount(HiSqlClient sqlClient)
        {
            int total = 0;
             int lsttales = sqlClient.DbFirst.GetTableDataCount("Hi_FieldModel");
            Console.WriteLine($" {lsttales} ");
        }
        



        static void Demo_CreateView(HiSqlClient sqlClient)
        {
            //OpLevel.Execute  表示执行并返回生成的SQL
            //OpLevel.Check 表示仅做检测失败时返回消息且检测成功时返因生成的SQL
            var rtn = sqlClient.DbFirst.CreateView("vw_FModel", 
                sqlClient.HiSql("select a.TabName,b.TabReName,b.TabDescript,a.FieldName,a.SortNum,a.FieldType from Hi_FieldModel as a inner join Hi_TabModel as b on a.TabName=b.TabName").ToSql(), 
                OpLevel.Execute);

            if (rtn.Item1)
            {
                Console.WriteLine(rtn.Item2);//输出成功消息
                Console.WriteLine(rtn.Item3);//输出 生成的SQL
            }
            else
                Console.WriteLine(rtn.Item2);//输出失败原因
        }

        static void Demo_ModiView(HiSqlClient sqlClient)
        {
            //OpLevel.Execute  表示执行并返回生成的SQL
            //OpLevel.Check 表示仅做检测失败时返回消息且检测成功时返因生成的SQL
            var rtn = sqlClient.DbFirst.ModiView("vw_FModel",
                sqlClient.HiSql("select a.TabName,b.TabReName,b.TabDescript,a.FieldName,a.SortNum,a.FieldType from Hi_FieldModel as a inner join Hi_TabModel as b on a.TabName=b.TabName where b.TabType in (0,1)").ToSql(),
                OpLevel.Execute);

            if (rtn.Item1)
            {
                Console.WriteLine(rtn.Item2);//输出成功消息
                Console.WriteLine(rtn.Item3);//输出 生成的SQL
            }
            else
                Console.WriteLine(rtn.Item2);//输出失败原因
        }

        static void Demo_DropView(HiSqlClient sqlClient)
        {
            //OpLevel.Execute  表示执行并返回生成的SQL
            //OpLevel.Check 表示仅做检测失败时返回消息且检测成功时返因生成的SQL
            var rtn = sqlClient.DbFirst.DropView("vw_FModel",
            
                OpLevel.Execute);

            if (rtn.Item1)
            {
                Console.WriteLine(rtn.Item2);//输出成功消息
                Console.WriteLine(rtn.Item3);//输出 生成的SQL
            }
            else
                Console.WriteLine(rtn.Item2);//输出失败原因
        }

        static void Demo_AddColumn(HiSqlClient sqlClient)
        {
            //OpLevel.Execute  表示执行并返回生成的SQL
            //OpLevel.Check 表示仅做检测失败时返回消息且检测成功时返因生成的SQL
            HiColumn column = new HiColumn()
            {
                TabName = "Hi_Test",
                FieldName = "TestAdd22",
                FieldType = HiType.VARCHAR,
                FieldLen = 50,
                DBDefault = HiTypeDBDefault.EMPTY,
                DefaultValue = "",
                FieldDesc = "测试字段添加"

            };

            var rtn= sqlClient.DbFirst.AddColumn("Hi_Test", column, OpLevel.Execute);

            if (rtn.Item1)
            {
                Console.WriteLine(rtn.Item2);//输出成功消息
                Console.WriteLine(rtn.Item3);//输出 生成的SQL
            }
            else
                Console.WriteLine(rtn.Item2);//输出失败原因
        }

        static void Demo_DelColumn(HiSqlClient sqlClient)
        {
            HiColumn column = new HiColumn()
            {
                TabName = "Hi_Test",
                FieldName = "TestAdd",
                FieldType = HiType.VARCHAR,
                FieldLen = 50,
                DBDefault = HiTypeDBDefault.EMPTY,
                DefaultValue = "",
                FieldDesc = "测试字段添加"

            };

            var rtn = sqlClient.DbFirst.DelColumn("Hi_Test", column, OpLevel.Execute);

            Console.WriteLine(rtn.Item2);
        }


        static void Demo_ReColumn(HiSqlClient sqlClient)
        {
            //OpLevel.Execute  表示执行并返回生成的SQL
            //OpLevel.Check 表示仅做检测失败时返回消息且检测成功时返因生成的SQL
            HiColumn column = new HiColumn()
            {
                TabName = "H_Test5",
                FieldName = "Testname3",
                ReFieldName = "Testname2",
                FieldType = HiType.VARCHAR,
                FieldLen = 50,
                DBDefault = HiTypeDBDefault.VALUE,
                DefaultValue = "TGM",
                FieldDesc = "测试字段变更"

            };

            var rtn = sqlClient.DbFirst.ReColumn("H_Test5", column, OpLevel.Execute);
            if (rtn.Item1)
            {
                Console.WriteLine(rtn.Item2);//输出成功消息
                Console.WriteLine(rtn.Item3);//输出 生成的SQL
            }
            else
                Console.WriteLine(rtn.Item2);//输出失败原因
        }

        static void Demo_ModiTable(HiSqlClient sqlClient)
        {
            //OpLevel.Execute  表示执行并返回生成的SQL
            //OpLevel.Check 表示仅做检测失败时返回消息且检测成功时返因生成的SQL
            var tabinfo = sqlClient.Context.DMInitalize.GetTabStruct("Hi_Test");

            TabInfo _tabcopy = ClassExtensions.DeepCopy<TabInfo>(tabinfo);
            _tabcopy.Columns.RemoveAt(4);

            HiColumn newcol = ClassExtensions.DeepCopy<HiColumn>(_tabcopy.Columns[1]);
            newcol.FieldName = "Testname23";
            newcol.ReFieldName = "Testname23";
            newcol.IsNull = true;
            _tabcopy.Columns.Add(newcol);

            _tabcopy.Columns[4].ReFieldName = "Testname337";
            _tabcopy.Columns[4].FieldDesc = "Testname337";
            _tabcopy.Columns[4].IsRequire = true;

            _tabcopy.Columns[3].FieldType = HiType.DATETIME;

            var rtn= sqlClient.DbFirst.ModiTable(_tabcopy, OpLevel.Execute);
            if (rtn.Item1)
            {
                Console.WriteLine(rtn.Item2);//输出成功消息
                Console.WriteLine(rtn.Item3);//输出 生成的SQL
            }
            else
                Console.WriteLine(rtn.Item2);//输出失败原因

        }

        static void Demo_ModiColumn(HiSqlClient sqlClient)
        {
            //OpLevel.Execute  表示执行并返回生成的SQL
            //OpLevel.Check 表示仅做检测失败时返回消息且检测成功时返因生成的SQL
            HiColumn column = new HiColumn()
            {
                TabName = "Hi_Test",
                FieldName = "TestAdd",
                FieldType = HiType.INT,
                FieldLen = 51,
                DBDefault = HiTypeDBDefault.VALUE,
                DefaultValue = "TGM",
                FieldDesc = "测试字段变更"

            };

            var rtn = sqlClient.DbFirst.ModiColumn("Hi_Test", column, OpLevel.Execute);
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
