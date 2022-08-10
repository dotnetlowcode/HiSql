using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public class SqliteInsert : InsertProvider
    {

        public SqliteInsert()
        {
            this.DbConfig = new SqliteConfig(true);

            this.DbConfig.PackageRecord = 2000;
            this.DbConfig.PackageCells = 20000;
            this.DbConfig.PackageCell = 50;
        }

        
    }



}
