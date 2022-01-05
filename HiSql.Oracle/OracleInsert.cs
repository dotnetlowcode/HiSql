using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public class OracleInsert : InsertProvider
    {

        public OracleInsert()
        {
            this.DbConfig = new OracleConfig(true);
            this.DbConfig.BlukSize = 200;
        }


    }
}
