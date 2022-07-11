using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql.UnitTest
{
    internal class Demo_Upgrade
    {
        public static void Init(HiSqlClient sqlClient)
        {
            //Demo1(sqlClient);
            Demo2(sqlClient);
        }

        static void Demo1(HiSqlClient sqlClient)
        {

            sqlClient.CodeFirst.InstallHisql();


            TabInfo tabInfo= sqlClient.Context.DMInitalize.GetTabStruct("Hi_FieldModel");

            TabInfo tabinfo2=tabInfo.CloneCopy();

            HiColumn dbnamecol = new HiColumn { TabName= "Hi_FieldModel",FieldName="DbName", SortNum=4, FieldLen=50,FieldType=HiType.VARCHAR,IsBllKey=true,IsPrimary=true,DBDefault=HiTypeDBDefault.VALUE,DefaultValue="",FieldDesc="数据库名" };
            tabinfo2.Columns.Add(dbnamecol);

            var tuple=sqlClient.DbFirst.ModiTable(tabinfo2, OpLevel.Execute);
            

        }

        static void Demo2(HiSqlClient sqlClient)
        {
            List<string> list = Constants.DbCurrentSupportList;

            Version version = Constants.HiSqlVersion;
            Console.WriteLine($"hisql:{version.ToString()}");

            foreach (string n in list)
            {
                Version ver = Constants.GetDbTypeVersion(n);
                Console.WriteLine($"{n}:{ver.ToString()}");
            }

        }
    }
}
