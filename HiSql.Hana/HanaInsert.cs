using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public class HanaInsert: InsertProvider
    {
        public HanaInsert():base()
        {
            this.DbConfig = new HanaConfig(true);
            this.DbConfig.BlukSize = 200;
            this.IsBatchExec = false;
        }
    }
}
