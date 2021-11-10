using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
namespace HiSql
{

    /// <summary>
    /// Sql执行错误类
    /// </summary>
    public class HiSqlException:Exception
    {
        public string Sql { get; set; }
        public new Exception InnerException;
        public new string StackTrace;
        public new MethodBase TargetSite;
        public new string Source;
        HiSqlProvider _context;
        public HiSqlProvider Context
        {
            get { return _context; }
        }
        public HiSqlException(string message) : base(message)
        { 
            
        }
        public HiSqlException(HiSqlProvider context, string message, string sql) : base(message)
        {
            this.Sql = sql;
        }

        public HiSqlException(HiSqlProvider context, Exception ex, string sql) : base(ex.Message)
        {
            this.Sql = sql;
            this.InnerException = ex.InnerException;
            this.StackTrace = ex.StackTrace;
            this.TargetSite = ex.TargetSite;
            this.Source = ex.Source;
        }

    }
}
