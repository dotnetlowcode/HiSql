using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// 数据插入接口
    /// </summary>
    public partial interface IInsert
    {
        HiSqlProvider Context { get; set; }

        /// <summary>
        /// 向指定表插入单条数据
        /// </summary>
        /// <param name="tabname">需要插入的表</param>
        /// <param name="objdata">可以是一个实体类也可以是一个匿名类如new {UserName="tansar",Age=30}</param>
        /// <returns>返回this对象可以执行链式操作</returns>
        IInsert Insert(string tabnamne, object objdata);

        /// <summary>
        /// 向指定表插入多条数据
        /// </summary>
        /// <param name="tabname">需要插入的表</param>
        /// <param name="objdata">可以是一个实体类集合也可以是一个匿名类集合如 new List<object>{new {UserName="tansar",Age=30}}</param>
        /// <returns>返回this对象可以执行链式操作</returns>
        IInsert Insert(string tabname, List<object> lstdata);


        IInsert Insert<T>(T objdata);

        IInsert Insert<T>(string tabname, T objdata);

        IInsert Insert<T>(List<T> lstdata);

        IInsert Insert<T>(string tabname, List<T> lstdata);



        IInsert Modi(string tabname, List<object> lstdata);

        IInsert Modi(string tabname, object objdata);

        IInsert Modi<T>(string tabname, T objdata);
        IInsert Modi<T>(T objdata);
        IInsert Modi<T>(List<T> lstdata);
        IInsert Modi<T>(string tabname, List<T> lstdata);


        /// <summary>
        /// 将当前操作向数据库执行
        /// </summary>
        /// <returns>返回受影响的行数</returns>
        int ExecCommand();

        /// <summary>
        /// 将当前操作向数据库执行
        /// </summary>
        /// <returns>返回受影响的行数</returns>
        Task<int> ExecCommandAsync();

        /// <summary>
        /// 将当前操作生成SQL语句
        /// </summary>
        /// <returns>返回SQL语法</returns>
        string ToSql();
    }
}
