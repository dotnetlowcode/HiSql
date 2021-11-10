using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
   
    public class MySqlInsert : InsertProvider
    {

        public MySqlInsert()
        {
            this.DbConfig = new MySqlConfig(true);
        }


    }

}
