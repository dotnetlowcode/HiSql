using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql.Test
{
    public class Hi_Domain2 : StandField
    {
      
        //[SqlSugar.SugarColumn(IsPrimaryKey =true)]
        public string Domain { get; set; }

       
        public string DomainDesc { get; set; }
    }
}
