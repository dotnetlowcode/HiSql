using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public interface ICodeFirst
    {

        HiSqlClient SqlClient { get; set; }

        /// <summary>
        /// 安装初始化HiSql
        /// </summary>
        Task InstallHisql();


        /// <summary>
        /// Init数据库
        /// </summary>
        void CreateInitDataBase();
    }
}
