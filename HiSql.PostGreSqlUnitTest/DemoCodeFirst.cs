using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql.PostGreSqlUnitTest
{
    class DemoCodeFirst
    {
        public static void Init(HiSqlClient sqlClient)
        {
            //CodeFirst_Demo(sqlClient);
            Snro_Demo(sqlClient);
            string s = Console.ReadLine();
        }


        static void Snro_Demo(HiSqlClient sqlClient)
        { 
            
        }
        static void CodeFirst_Demo(HiSqlClient sqlClient)
        {
            Tuple<HiTable, List<HiColumn>> tabomdel = sqlClient.Context.DMInitalize.BuildTabStru(typeof(Hi_TabModel));
            Tuple<HiTable, List<HiColumn>> fieldomdel = sqlClient.Context.DMInitalize.BuildTabStru(typeof(Hi_FieldModel));
            TabInfo tabinfo_tab = sqlClient.Context.DMInitalize.BuildTab(typeof(Hi_Domain));
            TabInfo tabinfo_field = sqlClient.Context.DMInitalize.BuildTab(typeof(Hi_DataElement));


            //tabinfo_tab.TabModel.TabName = "##" + tabinfo_tab.TabModel.TabName;
            //tabinfo_tab.TabModel.TabReName = tabinfo_tab.TabModel.TabName.Substring(2) + "_" + System.Threading.Thread.CurrentThread.ManagedThreadId + "_" + tabinfo_tab.TabModel.TabName.GetHashCode().ToString().Substring(1);
            TabInfo tab_info_test = sqlClient.Context.DMInitalize.GetTabStruct("H_Test");

            //string _sql = sqlClient.Context.DMTab.BuildTabCreateSql(tabomdel.Item1, tabomdel.Item2, true);
            //string _sql = sqlClient.Context.DMTab.BuildTabCreateSql(tabinfo_field.TabModel, tabinfo_field.GetColumns, true);
            //int _effect = (int)sqlClient.Context.DBO.ExecCommand(_sql);
            //int _effect = (int)sqlClient.Context.DBO.ExecScalar(_sql);
            sqlClient.CodeFirst.InstallHisql();

        }
    }
}
