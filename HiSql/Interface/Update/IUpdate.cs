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


        IUpdate Set(object obj);
        IUpdate Set(params string[] fields);

        IUpdate Where(string sqlwhere);
        IUpdate Where(Filter where);

        /// <summary>
        /// 执行结果
        /// </summary>
        /// <returns></returns>
        int ExecCommand();


        string ToSql();


        Tuple<Dictionary<string, string>, Dictionary<string, string>> CheckData(bool isDic, TableDefinition table, object objdata, IDMInitalize dMInitalize, TabInfo tabinfo, List<string> fields, bool isonly);
        Tuple<List<Dictionary<string, string>>, List<Dictionary<string, string>>> CheckAllData(TableDefinition table, TabInfo tabinfo, List<string> fields, List<object> lstdata, bool isonly);

    }
}
