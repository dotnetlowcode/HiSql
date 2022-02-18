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
            Demo_AddColumn(sqlClient);
            
            Demo_ModiColumn(sqlClient);
            Demo_DelColumn(sqlClient);
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
