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
            //Demo_AddColumn(sqlClient);

            //Demo_ModiColumn(sqlClient);
            //Demo_ReColumn(sqlClient);
            //Demo_ModiTable(sqlClient);
            //Demo_ReTable(sqlClient);
            //Demo_DelColumn(sqlClient);
            //Demo_Tables(sqlClient);
            //Demo_View(sqlClient);
            Demo_AllTables(sqlClient);
            //Demo_GlobalTables(sqlClient);
            //Demo_DropView(sqlClient);
            //Demo_CreateView(sqlClient);
            //Demo_ModiView(sqlClient);

            //Demo_IndexList(sqlClient);
            //Demo_Index_Create(sqlClient);


        }

        static void Demo_ReTable(HiSqlClient sqlClient)
        {
            var rtn = sqlClient.DbFirst.ReTable("H_Test5_1", "H_Test5",OpLevel.Check);
            Console.WriteLine(rtn.Item3);
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

        static void Demo_CreateView(HiSqlClient sqlClient)
        {
            var rtn = sqlClient.DbFirst.CreateView("vw_FModel", 
                sqlClient.HiSql("select a.TabName,b.TabReName,b.TabDescript,a.FieldName,a.SortNum,a.FieldType from Hi_FieldModel as a inner join Hi_TabModel as b on a.TabName=b.TabName").ToSql(), 
                
                OpLevel.Execute);

            Console.WriteLine(rtn.Item2);
            Console.WriteLine(rtn.Item3);
        }

        static void Demo_ModiView(HiSqlClient sqlClient)
        {
            var rtn = sqlClient.DbFirst.ModiView("vw_FModel",
                sqlClient.HiSql("select a.TabName,b.TabReName,b.TabDescript,a.FieldName,a.SortNum,a.FieldType from Hi_FieldModel as a inner join Hi_TabModel as b on a.TabName=b.TabName where b.TabType in (0,1)").ToSql(),

                OpLevel.Execute);

            Console.WriteLine(rtn.Item2);
            Console.WriteLine(rtn.Item3);
        }

        static void Demo_DropView(HiSqlClient sqlClient)
        {
            var rtn = sqlClient.DbFirst.DropView("vw_FModel",
            
                OpLevel.Execute);

            Console.WriteLine(rtn.Item2);
            Console.WriteLine(rtn.Item3);
        }

        static void Demo_AddColumn(HiSqlClient sqlClient)
        {
            HiColumn column = new HiColumn()
            {
                TabName = "H_Test5",
                FieldName = "TestAdd",
                FieldType = HiType.VARCHAR,
                FieldLen = 50,
                DBDefault = HiTypeDBDefault.EMPTY,
                DefaultValue = "",
                FieldDesc = "测试字段添加"

            };

             var rtn= sqlClient.DbFirst.AddColumn("H_Test5", column, OpLevel.Execute);

            Console.WriteLine(rtn.Item2);
        }

        static void Demo_DelColumn(HiSqlClient sqlClient)
        {
            HiColumn column = new HiColumn()
            {
                TabName = "H_Test5",
                FieldName = "TestAdd",
                FieldType = HiType.VARCHAR,
                FieldLen = 50,
                DBDefault = HiTypeDBDefault.EMPTY,
                DefaultValue = "",
                FieldDesc = "测试字段添加"

            };

            var rtn = sqlClient.DbFirst.DelColumn("H_Test5", column, OpLevel.Execute);

            Console.WriteLine(rtn.Item2);
        }


        static void Demo_ReColumn(HiSqlClient sqlClient)
        {
            HiColumn column = new HiColumn()
            {
                TabName = "H_Test5",
                FieldName = "Testname3",
                ReFieldName = "Testname2",
                FieldType = HiType.VARCHAR,
                FieldLen = 50,
                DBDefault = HiTypeDBDefault.VALUE,
                DefaultValue = "TGM",
                FieldDesc = "测试字段添加"

            };

            var rtn = sqlClient.DbFirst.ReColumn("H_Test5", column, OpLevel.Execute);
            Console.WriteLine(rtn.Item2);
        }

        static void Demo_ModiTable(HiSqlClient sqlClient)
        {
            var tabinfo= sqlClient.Context.DMInitalize.GetTabStruct("H_Test5");

            TabInfo _tabcopy = ClassExtensions.DeepCopy<TabInfo>(tabinfo);
            _tabcopy.Columns[2].ReFieldName = "Testname3";
            var rtn= sqlClient.DbFirst.ModiTable(_tabcopy, OpLevel.Check);

        }

        static void Demo_ModiColumn(HiSqlClient sqlClient)
        {
            HiColumn column = new HiColumn()
            {
                TabName = "H_Test5",
                FieldName = "TestAdd",
                FieldType = HiType.VARCHAR,
                FieldLen = 50,
                DBDefault = HiTypeDBDefault.VALUE,
                DefaultValue = "TGM",
                FieldDesc = "测试字段添加"

            };

            var rtn = sqlClient.DbFirst.ModiColumn("H_Test5", column, OpLevel.Execute);

            Console.WriteLine(rtn.Item2);
            Console.WriteLine(rtn.Item3);
            


            Console.WriteLine(rtn.Item2);
        }
    }
}
