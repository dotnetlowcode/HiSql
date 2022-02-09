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



        [Obsolete("该方法已经弃用请用DbFirst中的CreateTable")]
        /// <summary>
        /// 创建表
        /// </summary>
        /// <param name="tabInfo"></param>
        /// <returns></returns>
        bool CreateTable(TabInfo tabInfo);


        [Obsolete("该方法已经弃用请用DbFirst中的CreateTable")]
        /// <summary>
        /// 根据实体类型创建表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        bool CreateTable(Type type);


        [Obsolete("该方法已经弃用请用DbFirst中的ModiTable")]
        /// <summary>
        /// 修改表
        /// </summary>
        /// <param name="tabInfo"></param>
        /// <returns></returns>
        bool ModiTable(TabInfo tabInfo);


        [Obsolete("该方法已经弃用请用DbFirst中的DropTable")]
        /// <summary>
        /// 删除表
        /// </summary>
        /// <param name="tabname"></param>
        /// <param name="nolog">表示是否先通过Truncate</param>
        /// <returns></returns>
        bool DropTable(string tabname,bool nolog=false);

        [Obsolete("该方法已经弃用请用DbFirst中的Truncate")]

        /// <summary>
        /// 清空表中所有数据不留痕迹
        /// </summary>
        /// <param name="tabname"></param>
        /// <returns></returns>
        bool Truncate(string tabname);

        /// <summary>
        /// Init数据库 暂不支持该功能
        /// </summary>
        void CreateInitDataBase();
    }
}
