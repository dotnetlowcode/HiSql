﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql.PostGreSqlUnitTest
{
    class DemoCodeFirst
    {
        public static void Init(HiSqlClient sqlClient)
        {
            
            CodeFirst_Demo(sqlClient);
            //Snro_Demo(sqlClient);
            //Create_Table(sqlClient);


            string s = Console.ReadLine();
        }

        static void Create_Table(HiSqlClient sqlClient)
        {
            sqlClient.CodeFirst.CreateTable(typeof(Table.HTest02));
        }
        static void Snro_Demo(HiSqlClient sqlClient)
        { 
            
        }
        static void CodeFirst_Demo(HiSqlClient sqlClient)
        {
            //Tuple<HiTable, List<HiColumn>> tabomdel = sqlClient.Context.DMInitalize.BuildTabStru(typeof(Hi_TabModel));
            //Tuple<HiTable, List<HiColumn>> fieldomdel = sqlClient.Context.DMInitalize.BuildTabStru(typeof(Hi_FieldModel));
            TabInfo tabinfo_tab = sqlClient.Context.DMInitalize.BuildTab(typeof(Hi_Domain));
            //TabInfo tabinfo_field = sqlClient.Context.DMInitalize.BuildTab(typeof(Hi_DataElement));


            tabinfo_tab.TabModel.TabName = "#" + tabinfo_tab.TabModel.TabName;
            tabinfo_tab.TabModel.TabReName = tabinfo_tab.TabModel.TabName.Substring(2) + "_" + System.Threading.Thread.CurrentThread.ManagedThreadId + "_" + tabinfo_tab.TabModel.TabName.GetHashCode().ToString().Substring(1);

            foreach (var col in tabinfo_tab.Columns)
            {
                col.TabName = tabinfo_tab.TabModel.TabName;
            }

            //bool cresult=sqlClient.DbFirst.CreateTable(tabinfo_tab);

            var tabinfo=sqlClient.Context.MCache.GetCache<TabInfo>(tabinfo_tab.TabModel.TabName);

            var dynresult = sqlClient.Query("Hi_Domain").Field("Domain").ToDynamic();

            //sqlClient.Insert(tabinfo_tab.TabModel.TabName, dynresult).ExecCommand();
            //sqlClient.Query("Hi_Domain").Field("Domain").Insert(tabinfo_tab.TabModel.TabName);
            sqlClient.HiSql("select * from Hi_Domain").Insert(tabinfo_tab.TabModel.TabName);
            var sql = sqlClient.Query(tabinfo_tab.TabModel.TabName).Field("*").ToSql();

            var result = sqlClient.Query(tabinfo_tab.TabModel.TabName).As("A").Field("A.Domain")
                .Join("Hi_Domain").As("B").On(new HiSql.JoinOn() { { "A.Domain", "B.Domain" } })

                .Sort("A.Domain asc")

                .ToTable();

            sqlClient.DbFirst.DropTable(tabinfo_tab.TabModel.TabName);
            // TabInfo tab_info_test = sqlClient.Context.DMInitalize.GetTabStruct("H_Test");

            //string _sql = sqlClient.Context.DMTab.BuildTabCreateSql(tabomdel.Item1, tabomdel.Item2, true);
            //string _sql = sqlClient.Context.DMTab.BuildTabCreateSql(tabinfo_field.TabModel, tabinfo_field.GetColumns, true);
            //int _effect = (int)sqlClient.Context.DBO.ExecCommand(_sql);
            //int _effect = (int)sqlClient.Context.DBO.ExecScalar(_sql);

            HiSql.Global.SnroOn = true;
            sqlClient.CodeFirst.InstallHisql();

        }
    }
}
