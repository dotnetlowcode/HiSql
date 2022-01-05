using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql 
{
    public interface IDM:IDMInitalize,IDMTab
    {
        HiSqlProvider Context { get; set; }
    }
}
