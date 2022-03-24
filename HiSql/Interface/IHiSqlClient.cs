using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public interface IHiSqlClient:IDisposable
    {



        /// <summary>
        /// 表查询
        /// </summary>
        /// <param name="tabname">表名</param>
        /// <param name="rename">表别名</param>
        /// <param name="dbMasterSlave">主从策略</param>
        /// <returns></returns>
        IQuery Query(string tabname,string rename, DbMasterSlave dbMasterSlave= DbMasterSlave.Default);
        /// <summary>
        /// 表查询
        /// </summary>
        /// <param name="tabname">表名</param>
        /// <param name="dbMasterSlave">主从策略</param>
        /// <returns></returns>
        IQuery Query(string tabname, DbMasterSlave dbMasterSlave = DbMasterSlave.Default);

        /// <summary>
        /// 执行Hisql语句
        /// 详细请参照Hisql语法
        /// 注意hisql并不是原生数据sql Hisql是一个单独的语法可以编译成不同数据库的原生sql
        /// </summary>
        /// <param name="hisql"></param>
        /// <param name="dbMasterSlave"></param>
        /// <returns></returns>
        IQuery HiSql(string hisql, DbMasterSlave dbMasterSlave = DbMasterSlave.Default);

        /// <summary>
        /// 执行sql 防注入模板
        /// </summary>
        /// <param name="hisql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        IQuery HiSql(string hisql, Dictionary<string,object> dicparma, DbMasterSlave dbMasterSlave = DbMasterSlave.Default);


        /// <summary>
        /// 表数据插入
        /// </summary>
        /// <param name="tabname">表名</param>
        /// <param name="objdata">数据 匿名类，实体，字典</param>
        /// <returns></returns>
        IInsert Insert(string tabname, object objdata);


        /// <summary>
        /// 打开数据库
        /// </summary>
        void Open();

        /// <summary>
        /// 关闭数据库
        /// </summary>
        void Close();

        /// <summary>
        /// 开启事务
        /// </summary>
        void BeginTran();

        /// <summary>
        /// 提交事务
        /// </summary>
        void CommitTran();

        /// <summary>
        /// 事务回滚
        /// </summary>
        void RollBackTran();


        /// <summary>
        /// CodeFirst
        /// </summary>
        ICodeFirst CodeFirst { get; }


        //int BulkCopyExecCommand(TabInfo tabInfo, List<object> lstdata);

        //int BulkCopyExecCommand(TabInfo tabInfo, DataTable lstdata);

    }
}
