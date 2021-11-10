using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{

    public class PostGreSqlInsert : InsertProvider
    {

        public PostGreSqlInsert()
        {
            this.DbConfig = new PostGreSqlConfig(true);
        }


    }
}
