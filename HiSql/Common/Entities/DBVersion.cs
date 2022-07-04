using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{

    /// <summary>
    /// 数据库版本信息
    /// </summary>
    public class DBVersion
    {
        public string VersionDesc { get; set; }
        public Version Version { get; set; }
    }
}
