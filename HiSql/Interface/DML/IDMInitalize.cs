using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{

    //HiSql 环境初始化
    public interface IDMInitalize
    {
        /// <summary>
        /// 根据实体生成
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        Tuple<HiTable, List<HiColumn>> BuildTabStru(Type type)  ;
        TabInfo BuildTab(Type type);

        string GetDbDefault(HiColumn hiColumn,string tabname="");

        /// <summary>
        /// 将表结构数据转成 表缓存数据
        /// </summary>
        /// <param name="table"></param>
        /// <param name="_dbmapping"></param>
        /// <returns></returns>
        TabInfo TabDefinitionToEntity(DataTable table, Dictionary<HiType, string> _dbmapping);
        string GetDbType(HiType type);


        /// <summary>
        /// 获取表结构信息
        /// </summary>
        /// <param name="tabname"></param>
        /// <returns></returns>
        TabInfo GetTabStruct(string tabname);

        /// <summary>
        /// 根据结构信息创建表
        /// </summary>
        /// <param name="tabInfo"></param>
        /// <returns></returns>
        int BuildTabCreate(TabInfo tabInfo);
        HiSqlProvider Context { get; set; }
    }
}
