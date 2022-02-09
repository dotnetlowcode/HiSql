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
        }


        static void Demo_AddColumn(HiSqlClient sqlClient)
        {
            sqlClient.DbFirst.AddColumn("HTest01", null, OpLevel.Check);
        }
    }
}
