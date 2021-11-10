using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;

namespace HiSql
{
    /// <summary>
    /// ado 扩展
    /// </summary>
    public class AdoExtend
    {
        protected IDbConnection _DbConnection;
        protected IDbOperation _DbOperation;

        protected IDbFirst _DbFirst;

    }
}
