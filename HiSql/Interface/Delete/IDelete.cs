using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// 删除数据库表记录接口
    /// </summary>
    public interface IDelete
    {
        /// <summary>
        /// 连接上下文
        /// </summary>
        HiSqlProvider Context { get; set; }
        IDelete Delete(string tabname,object objdata);

        IDelete Delete(string tabname,List<object> objlst);

        IDelete Delete<T>(T objdata);

        IDelete Delete<T>(string tabname, List<T> objlst);

        IDelete Delete<T>(List<T> objlst);

        IDelete Delete(string tabname);

        IDelete Where(Filter where);

        /// <summary>
        /// 删除数据条件 自定义Hisql条件
        /// </summary>
        /// <param name="sqlwhere"></param>
        /// <returns></returns>
        IDelete Where(string sqlwhere);

        IDelete TrunCate(string tabname);

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


        string ToSql();

        Dictionary<string, string> CheckData(bool isDic, TableDefinition table, object objdata, IDMInitalize dMInitalize, TabInfo tabinfo);
    }
}
