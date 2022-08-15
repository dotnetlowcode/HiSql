using HiSql.SqlServerUnitTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    class DemoCodeFirst
    {
        public static void Init(HiSqlClient sqlClient)
        {
            //CodeFirst_Demo(sqlClient);
            CodeFirst_Table(sqlClient);
            //Snro_Demo(sqlClient);
            //CodeFirst_Install(sqlClient);
            string s = Console.ReadLine();
        }


        static void CodeFirst_Install(HiSqlClient sqlClient)
        {
            HiSql.Global.SnroOn = true;
            sqlClient.CodeFirst.InstallHisql();
        }
        static void CodeFirst_Table(HiSqlClient sqlClient)
        {
            //sqlClient.CodeFirst.CreateTable(typeof(HiSql.SNRO.Hi_Snro));

            //TabInfo tabInfo = new TabInfo();
            //tabInfo.TabModel = new HiTable() { };
            //tabInfo.Columns = new List<HiColumn> { };
            //sqlClient.CodeFirst.CreateTable(tabInfo);

            //sqlClient.CodeFirst.DropTable("H_Test");

            //sqlClient.CodeFirst.Truncate("H_Test");


            bool isok= sqlClient.CodeFirst.CreateTable(typeof(HTest04));
        }
        static async void CodeFirst_Demo(HiSqlClient sqlClient)
        {
            //Tuple<HiTable, List<HiColumn>> tabomdel = sqlClient.Context.DMInitalize.BuildTabStru(typeof(Hi_TabModel));
            //Tuple<HiTable, List<HiColumn>> fieldomdel = sqlClient.Context.DMInitalize.BuildTabStru(typeof(Hi_FieldModel));
            //TabInfo tabinfo_tab = sqlClient.Context.DMInitalize.BuildTab(typeof(Hi_Domain));
            //TabInfo tabinfo_element = sqlClient.Context.DMInitalize.BuildTab(typeof(Hi_DataElement));

            //TabInfo tabinfo_field = sqlClient.Context.DMInitalize.BuildTab(typeof(Hi_FieldModel));


            ////tabinfo_tab.TabModel.TabName = "##" + tabinfo_tab.TabModel.TabName;
            ////tabinfo_tab.TabModel.TabReName = tabinfo_tab.TabModel.TabName.Substring(2) + "_" + System.Threading.Thread.CurrentThread.ManagedThreadId + "_" + tabinfo_tab.TabModel.TabName.GetHashCode().ToString().Substring(1);
            //TabInfo tab_info_test = sqlClient.Context.DMInitalize.GetTabStruct("H_Test");

            Tuple<HiTable, List<HiColumn>> tabomdel = sqlClient.Context.DMInitalize.BuildTabStru(typeof(Hi_Snro));


            string _sql = sqlClient.Context.DMTab.BuildTabCreateSql(tabomdel.Item1, tabomdel.Item2, true);
            //string _sql = sqlClient.Context.DMTab.BuildTabCreateSql(tabinfo_element.TabModel, tabinfo_element.Columns, true);


            //sqlClient.CodeFirst.InstallHisql();




            int _effect =  sqlClient.Context.DBO.ExecCommand(_sql);
            //int _effect = (int)sqlClient.Context.DBO.ExecScalar(_sql);

        }
    }
}
