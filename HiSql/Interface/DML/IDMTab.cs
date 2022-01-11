using HiSql.AST;
using System;
using System.Collections.Generic;
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



        string BuildTabStructSql(HiTable hiTable, List<HiColumn> lstHiTable);

        //初始化HiSql相关的表

        string BuildInsertSql(Dictionary<string, string> _values, bool isbulk = false);


        string BuildMergeIntoSql(TabInfo targetinfo, TabInfo sourceinfo);
        
        //仅限于PostGreSql 其它库不支持
        string BuildMergeIntoSqlSequence(TabInfo targetinfo);


        string BuildKey(List<HiColumn> hiColumn);

        string BuildFieldStatement(HiTable hiTable, HiColumn hiColumn);

        string BuildFieldStatment(HiTable hiTable, List<HiColumn> lstColumn);

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
