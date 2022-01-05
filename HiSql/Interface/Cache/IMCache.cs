using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public interface IMCache
    {
        ICache MCache
        {
            get; set;
        }
    }
}
