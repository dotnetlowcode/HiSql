using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// 编号接口
    /// </summary>
    public interface INumber
    {

        /// <summary>
        /// 新创建编号
        /// </summary>
        /// <param name="snro">编号主规则</param>
        /// <param name="snum">编号规则子号</param>
        /// <returns></returns>
        string NewNumber(string snro, int snum);

        /// <summary>
        /// 批量创建编号
        /// </summary>
        /// <param name="snro">编号主规则</param>
        /// <param name="snum">编号规则子号</param>
        /// <param name="count">创建编号个数</param>
        /// <returns></returns>
        List<string> NewNumber(string snro, int snum, int count);



        /// <summary>
        /// 初始加载编号数据
        /// </summary>
        /// <returns></returns>
        bool Load(HiSqlClient sqlClient);


        /// <summary>
        /// 将缓存数据同步落盘
        /// </summary>
        /// <returns></returns>
        bool SyncDisk();

        /// <summary>
        /// 获取硬盘上的SNRO编号
        /// </summary>
        /// <returns></returns>
        List<Hi_Snro> GetDiskSnro();

    }
}
