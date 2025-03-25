using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public interface IUpdate
    {
        /// <summary>
        /// 连接上下文
        /// </summary>
        HiSqlProvider Context { get; set; }



        /// <summary>
        /// 更新单条记录
        /// </summary>
        /// <param name="tabnamne"></param>
        /// <param name="objdata"></param>
        /// <returns></returns>
        IUpdate Update(string tabnamne, object objdata);


        IUpdate Update(string tabname);
        /// <summary>
        /// 更新多条记录
        /// </summary>
        /// <param name="tabname"></param>
        /// <param name="lstdata"></param>
        /// <returns></returns>
        IUpdate Update(string tabname, List<object> lstdata);


        IUpdate Update<T>(T objdata);

        IUpdate Update<T>(string tabname, T objdata);

        IUpdate Update<T>(List<T> lstobj);

        IUpdate Update<T>(string tabname, List<T> lstobj);

        //IUpdate As(string retabname);
        //IUpdate Join(JoinDefinition join);

        //IUpdate Join(string tabname, string retabname, JoinType joinType = JoinType.Inner);

        //IUpdate Join(string tabname, JoinType joinType = JoinType.Inner);

        ///// <summary>
        ///// ON
        ///// </summary>
        ///// <param name="joinon"></param>
        ///// <returns></returns>
        //IUpdate On(JoinOn joinon);
        //IUpdate On(string leftCondition, string rightCcondition);

        //IUpdate On(string condition);

        //IUpdate On(Filter onfilter);

        /// <summary>
        /// 只更新哪一些字段
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        IUpdate Only(params string[] fields);


        /// <summary>
        /// 排除哪些字段不更新
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        IUpdate Exclude(params string[] fields);


        /// <summary>
        /// 更新字段,可以是一个匿名类
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        IUpdate Set(object obj);

        /// <summary>
        /// 暂不支持
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        IUpdate Set(params string[] fields);

        /// <summary>
        /// hisql 语法更新条件
        /// </summary>
        /// <param name="sqlwhere"></param>
        /// <returns></returns>
        IUpdate Where(string sqlwhere);


        /// <summary>
        /// 仅使用该方法指定的where进行条件更新
        /// hisql语法
        /// </summary>
        /// <param name="sqlwhere"></param>
        /// <returns></returns>
        IUpdate OnlyWhere(string sqlwhere);


        /// <summary>
        /// 仅使用该方法指定的where进行条件更新
        /// 
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        IUpdate OnlyWhere(Filter where);

        /// <summary>
        /// 更新条件
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        IUpdate Where(Filter where);

        /// <summary>
        /// 执行结果
        /// </summary>
        /// <returns></returns>
        int ExecCommand();


        /// <summary>
        /// 执行结果
        /// </summary>
        /// <returns></returns>
        Task<int> ExecCommandAsync();


        /// <summary>
        /// 执行结果
        /// </summary>
        /// <param name="credentialCallback">返回操作凭证</param>
        /// <returns></returns>
        Task<int> ExecCommandAsync(Action<HiSql.Interface.TabLog.Credential> credentialCallback);


        /// <summary>
        /// 生成该数据库类型的原生sql
        /// </summary>
        /// <returns></returns>
        string ToSql();


        Tuple<Dictionary<string, string>, Dictionary<string, string>> CheckData(bool isDic, TableDefinition table, object objdata, IDMInitalize dMInitalize, TabInfo tabinfo, List<string> fields, bool isonly);
        Tuple<List<Dictionary<string, string>>, List<Dictionary<string, string>>> CheckAllData(TableDefinition table, TabInfo tabinfo, List<string> fields, List<object> lstdata,bool hisqlwhere, bool isonly);

    }
}
