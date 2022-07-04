using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public class DaMengInsert : InsertProvider
    {

        public DaMengInsert()
        {
            this.DbConfig = new DaMengConfig(true);
            this.DbConfig.BlukSize = 200;
        }


    }
}
