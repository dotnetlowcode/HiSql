using HiSql.AST;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// 该接口处理与表管理相关的
    /// </summary>
    public interface IDMTab
    {

        HiSqlProvider Context { get; set; }
        /// <summary>
        /// 表创建
        /// </summary>
        /// <param name="hiTable"></param>
        /// <param name="lstHiTable"></param>
        /// <param name="isdrop">在创建前是否先删除</param>
        /// <returns></returns>
        string BuildTabCreateSql(HiTable hiTable, List<HiColumn> lstHiTable, bool isdrop = false);

        /// <summary>
        /// 根据表结构信息生成创建表的SQL语句
        /// </summary>
        /// <param name="tabInfo"></param>
        /// <returns></returns>
        string BuildTabCreateSql(TabInfo tabInfo);


        /// <summary>
        /// 执行表创建 并返回受影响结果
        /// </summary>
        /// <param name="tabInfo"></param>
        /// <returns></returns>
        int BuildTabCreate(TabInfo tabInfo);


        /// <summary>
        /// 获取物理表或视图的信息
        /// </summary>
        /// <param name="tabname"></param>
        /// <returns></returns>
        DataTable GetTableDefinition(string tabname);


        DataSet GetTabModelInfo(string tabname);

        /// <summary>
        /// 获取当前库所有物理表清单
        /// </summary>
        /// <returns></returns>
        DataTable GetTableList(string tabname = "");


        DataTable GetTableList(string tabname, int pageSize, int pageIndex, out int totalCount);

        /// <summary>
        /// 获取当前表数据量
        /// </summary>
        /// <returns></returns>
        int GetTableDataCount(string tabname);
        
        /// <summary>
        /// 获取当前库所有视图清单
        /// </summary>
        /// <returns></returns>
        DataTable GetViewList(string viewname = "");
        DataTable GetViewList(string viewname, int pageSize, int pageIndex, out int totalCount);



        /// <summary>
        /// 获取版本
        /// </summary>
        /// <returns></returns>
        DBVersion DBVersion();


        /// <summary>
        /// 创建视图
        /// </summary>
        /// <param name="viewname"></param>
        /// <param name="viewsql"></param>
        /// <returns></returns>
        string CreateView(string viewname, string viewsql);

        /// <summary>
        /// 修改视图
        /// </summary>
        /// <param name="viewname"></param>
        /// <param name="viewsql"></param>
        /// <returns></returns>
        string ModiView(string viewname, string viewsql);


        /// <summary>
        /// 删除视图
        /// </summary>
        /// <param name="viewname"></param>
        /// <returns></returns>
        string DropView(string viewname);


        /// <summary>
        /// 对指定表创建主键
        /// </summary>
        /// <param name="tabname"></param>
        /// <param name="hiColumns"></param>
        /// <returns></returns>
        string CreatePrimaryKey(string tabname,List<HiColumn> hiColumns);
        /// <summary>
        /// 对指定表创建索引
        /// </summary>
        /// <param name="tabname"></param>
        /// <param name="hiColumns"></param>
        /// <returns></returns>
        string CreateIndex(string tabname, string indexname, List<HiColumn> hiColumns);
        /// <summary>
        /// 删除索引
        /// </summary>
        /// <param name="tabname"></param>
        /// <param name="indexname"></param>
        /// <returns></returns>
        string DropIndex(string tabname, string indexname, bool isPrimary);

        
        /// <summary>
        /// 获取所有表和视图
        /// </summary>
        /// <returns></returns>
        DataTable GetAllTables(string tabname="");

        /// <summary>
        /// 检查表或视图是否存在
        /// </summary>
        /// <param name="tabname">表名称或视图名称</param>
        /// <returns></returns>
        bool CheckTabExists(string tabname);


        DataTable GetAllTables(string tabname, int pageSize, int pageIndex, out int totalCount);


        /// <summary>
        /// 获取表的过引列表
        /// </summary>
        /// <param name="tabname"></param>
        /// <returns></returns>
        List<TabIndex> GetIndexs(string tabname);


        /// <summary>
        /// 获取指定表指定表的索引明细
        /// </summary>
        /// <param name="indexname"></param>
        /// <returns></returns>
        List<TabIndexDetail> GetIndexDetails(string tabname, string indexname);
        


        /// <summary>
        /// 获取全局临时表清单
        /// </summary>
        /// <returns></returns>
        DataTable GetGlobalTables();

        string BuildTabStructSql(HiTable hiTable, List<HiColumn> lstHiTable);

        //初始化HiSql相关的表

        string BuildInsertSql(Dictionary<string, string> _values, bool isbulk = false);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetinfo"></param>
        /// <param name="sourceinfo"></param>
        /// <param name="dataColLst">当存在数据时指定更新的字段</param>
        /// <returns></returns>
        string BuildMergeIntoSql(TabInfo targetinfo, TabInfo sourceinfo,List<string> dataColLst=null);
        
        //仅限于PostGreSql 其它库不支持
        string BuildMergeIntoSqlSequence(TabInfo targetinfo,List<string> dataColLst= null);


        string BuildKey(List<HiColumn> hiColumn);

        string BuildFieldStatement(HiTable hiTable, HiColumn hiColumn, bool isalteraddkey=false);


 
        /// <summary>
        /// 根据表结构信息生成修改表的SQL语句
        /// </summary>
        /// <param name="tabInfo"></param>
        /// <returns></returns>
        string BuildTabModiSql(TabInfo tabInfo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hiTable"></param>
        /// <param name="hiColumn"></param>
        /// <param name="tabFieldAction"></param>
        /// <returns></returns>
        string BuildChangeFieldStatement(HiTable hiTable, HiColumn hiColumn, TabFieldAction tabFieldAction);

        /// <summary>
        /// 生成代码块。部分数据库需要使用 begin end 包一下执行多个sql
        /// </summary>
        /// <param name="sbSql"></param>
        /// <returns></returns>
        string BuildSqlCodeBlock(string sbSql);

        string BuildReTableStatement(string tabname, string newtabname);

        //string BuildChangeFieldStatement(TabInfo tabInfo,)

        string BuildFieldStatment(HiTable hiTable, List<HiColumn> lstColumn);

        string BuildFieldDefaultValue(HiColumn hiColumn);


        /// <summary>
        /// 检测向表插入数据
        /// 匿名或实体对像转太key-value的字段方式
        /// </summary>
        /// <param name="attrs"></param>
        /// <param name="hiColumns"></param>
        /// <param name="objdata"></param>
        /// <returns></returns>
        //Dictionary<string, string> CheckInsertData(List<PropertyInfo> attrs, List<HiColumn> hiColumns, object objdata);

        /// <summary>
        /// 检测表删除条件
        /// </summary>
        /// <param name="isRequireKey"></param>
        /// <param name="attrs"></param>
        /// <param name="hiColumns"></param>
        /// <param name="objdata"></param>
        /// <returns></returns>
        //Dictionary<string, string> CheckDeleteData(bool isRequireKey, List<PropertyInfo> attrs, List<HiColumn> hiColumns, object objdata);

        /// <summary>
        /// 检测字段是否合法
        /// </summary>
        /// <param name="TableList"></param>
        /// <param name="dictabinfo"></param>
        /// <param name="Fields"></param>
        /// <param name="fieldDefinition"></param>
        /// <param name="allowstart"></param>
        /// <returns></returns>
        HiColumn CheckField(List<TableDefinition> TableList, Dictionary<string, TabInfo> dictabinfo, List<FieldDefinition> Fields, FieldDefinition fieldDefinition, bool allowstart = false);



        /// <summary>
        /// 检测更新数据
        /// </summary>
        /// <param name="requireKey">是否必须需要KEY 主键(业务主键)</param>
        /// <param name="attrs">匿名或实体</param>
        /// <param name="hiColumns">表的列结构信息</param>
        /// <param name="objdata">匿名类或实体类</param>
        /// <param name="fields">字段列表</param>
        /// <param name="isonly">是否仅更新fields中的字段</param>
        /// <returns></returns>
        //Tuple< Dictionary<string, string>,Dictionary<string,string>> CheckUpdateData(bool requireKey, List<PropertyInfo> attrs, List<HiColumn> hiColumns, object objdata,List<string> fields,bool isonly);

        string BuildDeleteSql(TableDefinition table, Dictionary<string, string> dic_value, string _where, bool istruncate = false,bool isdrop=false);
        string BuildUpdateSql(TableDefinition table, Dictionary<string, string> dic_value, Dictionary<string, string> dic_primary, string _where);

        /// <summary>
        /// 生成更新语句,实现数据 以字段更新字段带表达式的内容
        /// </summary>
        /// <param name="tabinfo"></param>
        /// <param name="table"></param>
        /// <param name="dic_value"></param>
        /// <param name="dic_primary"></param>
        /// <param name="_where"></param>
        /// <returns></returns>
        string BuildUpdateSql(TabInfo tabinfo, TableDefinition table, Dictionary<string, string> dic_value, Dictionary<string, string> dic_primary, string _where);

        string BuilderWhereSql(List<TableDefinition> TableList, Dictionary<string, TabInfo> dictabinfo, List<FieldDefinition> Fields, List<FilterDefinition> Wheres, bool issubquery);

        /// <summary>
        /// 生成去重命令
        /// </summary>
        /// <returns></returns>
        string BuilderDistinct();

        string BuilderWhereSql(List<TableDefinition> TableList, Dictionary<string, TabInfo> dictabinfo, List<FieldDefinition> Fields, List<WhereResult> lstresult, bool issubquery);

        /// <summary>
        /// 构建Group sql
        /// </summary>
        /// <param name="TableList"></param>
        /// <param name="dictabinfo"></param>
        /// <param name="Fields"></param>
        /// <param name="Groups"></param>
        /// <param name="issubquery"></param>
        /// <returns></returns>
        string BuildGroupSql(List<TableDefinition> TableList, Dictionary<string, TabInfo> dictabinfo, List<FieldDefinition> Fields, List<GroupDefinition> Groups, bool issubquery);

        /// <summary>
        /// 构建having sql
        /// </summary>
        /// <param name="TableList"></param>
        /// <param name="dictabinfo"></param>
        /// <param name="Fields"></param>
        /// <param name="lstresult"></param>
        /// <param name="issubquery"></param>
        /// <returns></returns>
        string BuildHavingSql(List<TableDefinition> TableList, Dictionary<string, TabInfo> dictabinfo, List<FieldDefinition> Fields, List<HavingResult> lstresult, bool issubquery);
    }
}
