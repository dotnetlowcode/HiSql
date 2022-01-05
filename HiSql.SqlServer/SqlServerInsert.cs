using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public class SqlServerInsert : InsertProvider
    {

        public SqlServerInsert()
        {
            this.DbConfig = new SqlServerConfig(true);
        }

        
    }



}
