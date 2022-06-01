using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// 数据库表基础操作
    /// </summary>
    public  interface IDbFirst
    {
        HiSqlClient SqlClient { get; set; }
        /// <summary>
        /// 对列进行修改
        /// </summary>
        /// <param name="tabname"></param>
        /// <param name="hiColumn"></param>
        /// <returns></returns>
        Tuple<bool,string,string> ModiColumn(string tabname, HiColumn hiColumn,OpLevel opLevel);

        /// <summary>
        /// 添加列
        /// </summary>
        /// <param name="tabname"></param>
        /// <param name="hiColumn"></param>
        /// <returns></returns>
        Tuple<bool, string,string> AddColumn(string tabname, HiColumn hiColumn, OpLevel opLevel);


        /// <summary>
        /// 删除列
        /// </summary>
        /// <param name="tabname"></param>
        /// <param name="hiColumn"></param>
        /// <param name="opLevel"></param>
        /// <returns></returns>
        Tuple<bool, string, string> DelColumn(string tabname, HiColumn hiColumn, OpLevel opLevel);


        /// <summary>
        /// 字段重命名
        /// </summary>
        /// <param name="tabname"></param>
        /// <param name="hiColumn"></param>
        /// <param name="opLevel"></param>
        /// <returns></returns>
        Tuple<bool,string,string> ReColumn(string tabname, HiColumn hiColumn, OpLevel opLevel);



        /// <summary>
        /// 表重命名
        /// </summary>
        /// <param name="tabname"></param>
        /// <param name="newtabname"></param>
        /// <param name="opLevel"></param>
        /// <returns></returns>
        Tuple<bool, string, string> ReTable(string tabname, string newtabname, OpLevel opLevel);


        /// <summary>
        /// 不存在表则创建，有则修改表结构
        /// </summary>
        /// <param name="tabInfo"></param>
        /// <param name="opLevel"></param>
        /// <returns></returns>
        Tuple<bool, string,string> ModiTable(TabInfo tabInfo, OpLevel opLevel);


        /// <summary>
        /// 创建表索引
        /// </summary>
        /// <param name="tabname"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        Tuple<bool, string,string> CreateIndex(string tabname,string indexname, List<HiColumn> columns, OpLevel opLevel);
        
        /// <summary>
        /// 创建表主键
        /// </summary>
        /// <param name="tabname"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        Tuple<bool, string, string> CreatePrimaryKey(string tabname, List<HiColumn> columns, OpLevel opLevel);
        /// <summary>
        /// 删除表主键
        /// </summary>
        /// <param name="tabname"></param>
        /// <param name="indexname"></param>
        /// <returns></returns>
        Tuple<bool, string, string> DelPrimaryKey(string tabname, OpLevel opLevel);


        /// <summary>
        /// 删除表索引信息
        /// </summary>
        /// <param name="tabname"></param>
        /// <param name="indexname"></param>
        /// <returns></returns>
        Tuple<bool, string, string> DelIndex(string tabname,string indexname, OpLevel opLevel);



        /// <summary>
        /// 获取表索引列表
        /// </summary>
        /// <param name="tabname"></param>
        /// <returns></returns>
        List<TabIndex> GetTabIndexs(string tabname);

        /// <summary>
        /// 获取表索引的字段列
        /// </summary>
        /// <param name="tabname"></param>
        /// <param name="indexname"></param>
        /// <returns></returns>
        List<TabIndexDetail> GetTabIndexDetail(string tabname, string indexname);


        /// <summary>
        /// 获取所有物理表
        /// </summary>
        /// <returns></returns>
        List<TableInfo> GetTables();

        /// <summary>
        /// 分页获取获取所有物理表
        /// </summary>
        /// <returns></returns>
        List<TableInfo> GetTables(string tableName, int pageSize, int pageIndex, out int totalCount);

        /// <summary>
        /// 获取所有视图
        /// </summary>
        /// <returns></returns>
        List<TableInfo> GetViews();
        List<TableInfo> GetViews(string viewName, int pageSize, int pageIndex, out int totalCount);


        /// <summary>
        /// 创建视图
        /// </summary>
        /// <param name="viewname"></param>
        /// <param name="viewsql"></param>
        /// <returns></returns>
        Tuple<bool, string,string> CreateView(string viewname, string viewsql, OpLevel opLevel);


        /// <summary>
        /// 修改视图
        /// </summary>
        /// <param name="viewname"></param>
        /// <param name="viewsql"></param>
        /// <param name="opLevel"></param>
        /// <returns></returns>
        Tuple<bool, string, string> ModiView(string viewname, string viewsql, OpLevel opLevel);


        /// <summary>
        /// 删除视图
        /// </summary>
        /// <param name="viewname"></param>
        /// <param name="opLevel"></param>
        /// <returns></returns>
        Tuple<bool, string, string> DropView(string viewname,  OpLevel opLevel);
        /// <summary>
        /// 获取所有物理表和视图
        /// </summary>
        /// <returns></returns>
        List<TableInfo> GetAllTables();
        List<TableInfo> GetAllTables(string viewName, int pageSize, int pageIndex, out int totalCount);

     
        List<string> GetStoredProc();
       

        /// <summary>
        /// 获取当前表数据量
        /// </summary>
        /// <returns></returns>
        int  GetTableDataCount(string tabname);


        /// <summary>
        /// 获取表或视图的物理结构信息
        /// </summary>
        /// <param name="tabname"></param>
        /// <returns></returns>
        DataTable GetTabPhyInfo(string tabname);


        /// <summary>
        /// 获取所有临时表
        /// </summary>
        /// <returns></returns>
        List<string> GetTempTables();


        /// <summary>
        /// 获取全局临时表
        /// </summary>
        /// <returns></returns>
        List<TableInfo> GetGlobalTempTables();

        /// <summary>
        /// 创建表
        /// </summary>
        /// <param name="tabInfo"></param>
        /// <returns></returns>
        bool CreateTable(TabInfo tabInfo);

        /// <summary>
        /// 根据实体类型创建表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        bool CreateTable(Type type);

        /// <summary>
        /// 检测表或视图是否存在
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public bool CheckTabExists(string tableName);

        /// <summary>
        /// 删除表
        /// </summary>
        /// <param name="tabname"></param>
        /// <param name="nolog">表示是否先通过Truncate</param>
        /// <returns></returns>
        bool DropTable(string tabname, bool nolog = false);


        /// <summary>
        /// 清空表中所有数据不留痕迹
        /// </summary>
        /// <param name="tabname"></param>
        /// <returns></returns>
        bool Truncate(string tabname);


    }
}
