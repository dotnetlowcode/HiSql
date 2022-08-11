using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HiSql.SqlServer.Test
{
    public class Unit_Query
    {
        HiSqlClient sqlClient;
        public Unit_Query()
        {

            sqlClient = TestClientInit.GetSqlServerClient();
        }



        [Fact]
        [Trait("level2", "查询")]
        public void InitQuery()
        {
            //sqlClient.HiSql("select * from ");
            Assert.True(true);
        }
    }
}
