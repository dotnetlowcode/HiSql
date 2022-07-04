using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Linq.Expressions;
using System.Data;
namespace HiSql
{
    /// <summary>
    /// HiSql操作
    /// 通过该对象以对数据库进行 增,删,查,改 及批量操作
    /// author:tansar mail:tansar@126.com
    /// </summary>
    public partial class HiSqlClient : IHiSqlClient
    {

        /// <summary>
        /// 当前主库数据库连接
        /// </summary>
        private ConnectionConfig _currentConnectionConfig;


        /// <summary>
        /// 当前从库的数据库连接
        /// </summary>
        private ConnectionConfig _salveConnectionConfig;


        /// <summary>
        /// 线程编号
        /// </summary>
        private string _threadId;

        /// <summary>
        /// 是否打开数据库
        /// </summary>
        private bool _isOpen = false;

        private ICodeFirst codeFirst = new CodeFirst();


        private IDbFirst dbFirst = new DbFirst();

        //ICache _mcache = new MCache(null);
        /// <summary>
        /// 连接管道
        /// </summary>
        private HiSqlProvider _context = null;
        public HiSqlClient(ConnectionConfig config)
        {
            initContext(config);

            //如果有需要也可以克隆一个连接
            //this.CloneClient();
            codeFirst.SqlClient = this;
            dbFirst.SqlClient = this;


        }

        


        //public ICache MCache
        //{
        //    get { return _mcache; }
        //    set { _mcache = value; }
        //}

        #region 公共属性
        public HiSqlProvider Context { get { return getContext(); } }


        /// <summary>
        /// 当前数据连接
        /// </summary>
        public ConnectionConfig CurrentConnectionConfig {
            get { return _currentConnectionConfig; }
        }

        /// <summary>
        /// 从库数据库连接
        /// </summary>
        public ConnectionConfig SlaveConnectionConfig
        {
            get { return _salveConnectionConfig; }
        }

        /// <summary>
        /// CodeFirst
        /// </summary>
        public ICodeFirst CodeFirst => codeFirst;


        /// <summary>
        /// 
        /// </summary>
        public IDbFirst DbFirst => dbFirst;
        #endregion



        #region 私有方法

        /// <summary>
        /// 初始化连接上下文
        /// </summary>
        /// <param name="config"></param>
        void initContext(ConnectionConfig config)
        {
            //获取当前线程的编号 用于判断
            _threadId = Thread.CurrentThread.ManagedThreadId.ToString();
            if (string.IsNullOrEmpty(config.DbServer))
            {
                throw new Exception($"数据库连接配置未指定[DbServer]");
            }
            if (string.IsNullOrEmpty(config.Schema))
            {
                throw new Exception($"数据库连接配置未指定[Schema]");
            }

            _currentConnectionConfig = config;



            _context = new HiSqlProvider(config);

            _salveConnectionConfig = _context.SlaveConnectionConfig;
        }


        private HiSqlProvider getContext() {
            HiSqlProvider _provider = null;
            if (checkIsSameThreadAndShared())
            {
                _provider = SameThreadAndShared();
            }
            else if (checkIsNoSameThreadAndShare())
            {
                _provider = NoSameThreadAndShare();
            }
            else if (checkIsSynchroization())
            {
                _provider = Synchronization();
            }
            else
                _provider = _context;



            if (_provider.Root == null)
            {
                _provider.Root = this;
            }
            return _provider;
        }

        #endregion


        #region 判断是否是共享线程

        //是否开启了共享线程，且与当前线程是同一线程 
        bool checkIsSameThreadAndShared()
        {
            return _currentConnectionConfig.IsShareThread && _threadId == Thread.CurrentThread.ManagedThreadId.ToString();
        }

        /// <summary>
        /// 是否开启了共享线但， 但与当前线程不是同一个线程
        /// </summary>
        /// <returns></returns>
        bool checkIsNoSameThreadAndShare()
        {
            return _currentConnectionConfig.IsShareThread && _threadId != Thread.CurrentThread.ManagedThreadId.ToString();
        }

        /// <summary>
        /// 是否是同步线程 
        /// </summary>
        /// <returns></returns>
        bool checkIsSynchroization()
        {
            return _threadId == Thread.CurrentThread.ManagedThreadId.ToString();
        }

        #endregion


        
        HiSqlProvider CloneClient()
        {
            var provider = new HiSqlProvider(this.CurrentConnectionConfig);
            return provider;
        }

        /// <summary>
        /// 创建工作单元
        /// 默认开始事务,业务处理完成需要进行Commit 
        /// </summary>
        /// <returns></returns>
        public HiSqlClient CreateUnitOfWork()
        {
            var client=this.Context.CloneClient();
            //连接不能自动关闭 因为自动关闭时事务会自动提交
            client.CurrentConnectionConfig.IsAutoClose = false;
            client.BeginTran();
            return client;

        }

        #region 线程缓存
        /// <summary>
        /// 缓存到静态变更中。
        /// </summary>
        /// <param name="context"></param>
        void AddCacheContext(HiSqlProvider context)
        {
            CacheContext.ContextList.Value = new List<HiSqlProvider>();
            CacheContext.ContextList.Value.Add(context);
        }

        HiSqlProvider SameThreadAndShared()
        {
            if (CacheContext.ContextList.Value.IsNullOrEmpty())
            {
                AddCacheContext(_context);
                return _context;
            }
            else
            {
                var context = GetCacheContext();
                if (context == null)
                {
                    var _clonecontext = CloneClient();
                    AddCacheContext(_clonecontext);
                    return _clonecontext;
                }
                else
                    return context;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        HiSqlProvider Synchronization()
        {

            return _context;
        }


        HiSqlProvider NoSameThreadAndShare()
        {
            if (CacheContext.ContextList.Value.IsNullOrEmpty())
            {
                var _clonecontext = CloneClient();
                AddCacheContext(_clonecontext);
                return _clonecontext;
            }
            else
            {
                var context = GetCacheContext();
                if (context == null)
                {
                    var _clonecontext = CloneClient();
                    AddCacheContext(_clonecontext);
                    return _clonecontext;
                }
                else
                    return context;
            }
        }


        HiSqlProvider GetCacheContext()
        {
            return CacheContext.ContextList.Value.FirstOrDefault(hi =>
            hi.CurrentConnectionConfig.DbType == _context.CurrentConnectionConfig.DbType &&
            hi.CurrentConnectionConfig.ConnectionString == _context.CurrentConnectionConfig.ConnectionString &&
            hi.CurrentConnectionConfig.IsAutoClose == _context.CurrentConnectionConfig.IsAutoClose
            );
        }

        #endregion


        #region 数据库操作
        public void Close()
        {
            this.Context.Close();
            _isOpen = false;
        }
        public void Open()
        {
            this.Context.Open();
            _isOpen = true;
        }

        /// <summary>
        /// 开启事务
        /// </summary>
        public void BeginTran()
        {
            this.Context.DBO.BeginTran();
        }
        /// <summary>
        /// 开启事务 设定隔离级别
        /// </summary>
        /// <param name="iso"></param>
        public   void BeginTran(IsolationLevel iso)
        {
            this.Context.DBO.BeginTran(iso);
        }
        public void CommitTran()
        {
            this.Context.DBO.CommitTran();
        }
        public void RollBackTran()
        {
            this.Context.DBO.RollBackTran();
        }
        #endregion



        void IDisposable.Dispose()
        {
            this.Context.Dispose();
        }



        #region 数据查询操作

        public IQuery Query(params IQuery[] query)
        {
            return _context.Query(query);
        }

        /// <summary>
        /// 查询指定表并进行指定别名
        /// </summary>
        /// <param name="tabname">需要查询的表名</param>
        /// <param name="rename">指定的别名</param>
        /// <param name="dbMasterSlave">主从策略</param>
        /// <returns>返回当前对象支持链式操作
        /// (后面的操作中不允重复使用该方法)
        /// </returns>
        public IQuery Query(string tabname, string rename, DbMasterSlave dbMasterSlave = DbMasterSlave.Default)
        {
            return _context.Query(tabname, rename, dbMasterSlave);
        }

        /// <summary>
        /// 查询指定表进行查询
        /// </summary>
        /// <param name="tabname">查询指定的表名</param>
        /// <param name="dbMasterSlave">主从策略</param>
        /// <returns>返回当前对象支持链式操作
        /// (后面的操作中不允重复使用该方法)
        /// </returns>
        public IQuery Query(string tabname, DbMasterSlave dbMasterSlave = DbMasterSlave.Default)
        {
            return _context.Query(tabname, dbMasterSlave);
        }
        /// <summary>
        /// 执行Hisql语句
        /// 详细请参照Hisql语法
        /// 注意hisql并不是原生数据sql Hisql是一个单独的语法可以编译成不同数据库的原生sql
        /// </summary>
        /// <param name="hisql"></param>
        /// <param name="dbMasterSlave"></param>
        /// <returns></returns>
        public IQuery HiSql(string hisql, DbMasterSlave dbMasterSlave = DbMasterSlave.Default)
        {
            return _context.HiSql(hisql, dbMasterSlave);
        }
        /// <summary>
        /// hisql 参数化,防注入
        /// </summary>
        /// <param name="hisql"></param>
        /// <param name="dicparma"></param>
        /// <param name="dbMasterSlave"></param>
        /// <returns></returns>
        //public IQuery HiSql(string hisql, Dictionary<string, object> dicparma, DbMasterSlave dbMasterSlave = DbMasterSlave.Default)
        //{
        //    return _context.HiSql(hisql, dicparma, dbMasterSlave);
        //}

        /// <summary>
        /// hisql 参数化,防注入
        /// </summary>
        /// <param name="hisql">hisql语句</param>
        /// <param name="objparm">参数化对象如new {}</param>
        /// <param name="dbMasterSlave"></param>
        /// <returns></returns>

        public IQuery HiSql(string hisql, object objparm, DbMasterSlave dbMasterSlave = DbMasterSlave.Default)
        {
            return _context.HiSql(hisql, objparm, dbMasterSlave);
        }

        #endregion

        #region 数据插入操作
        /// <summary>
        /// 向指定表插入单条数据
        /// </summary>
        /// <param name="tabname">需要插入的表</param>
        /// <param name="objdata">可以是一个实体类也可以是一个匿名类如new {UserName="tansar",Age=30}</param>
        /// <returns>
        /// 返回this对象可以执行链式操作
        /// </returns>
        public IInsert Insert(string tabname, object objdata)
        {
            return _context.Insert(tabname, objdata);
        }
        /// <summary>
        /// 向指定表插入多条数据
        /// </summary>
        /// <param name="tabname">需要插入的表</param>
        /// <param name="objdata">可以是一个实体类集合也可以是一个匿名类集合如 new List<object>{new {UserName="tansar",Age=30}}</param>
        /// <returns>返回this对象可以执行链式操作</returns>
        public IInsert Insert(string tabname, List<object> lstobj)
        {
            return _context.Insert(tabname, lstobj);
        }


        public IInsert Insert<T>(string tabname, List<T> lstobj)
        {
            return _context.Insert<T>(tabname, lstobj);
        }
        public IInsert Insert<T>(T objdata)
        {
            return _context.Insert<T>(objdata);
        }
        public IInsert Insert<T>(string tabname, T objdata)
        {
            return _context.Insert<T>(tabname, objdata);
        }
        public IInsert Insert<T>(List<T> lstdata)
        {
            return _context.Insert<T>(lstdata);
        }

        public IInsert Modi(string tabname, List<object> lstobj)
        {
            return _context.Modi(tabname, lstobj);
        }
        //public IInsert Modi(string tabname, List<Dictionary<string, string>> lstdic)
        //{
        //    return _context.Modi(tabname, lstdic);
        //}


        public IInsert Modi<T>(string tabname, T objdata)
        {
            return _context.Modi<T>(tabname, objdata);
        }
        public IInsert Modi<T>(T objdata)
        {
            return _context.Modi<T>(objdata);
        }
        public IInsert Modi<T>(List<T> lstdata) {
            return _context.Modi<T>(lstdata);
        }
        public IInsert Modi<T>(string tabname, List<T> lstdata) {
            return _context.Modi<T>(tabname,lstdata);
        }

        #endregion

        #region 更新操作
        public IUpdate Update(string tabname, object objdata)
        {
            return _context.Update(tabname, objdata);
        }

        /// <summary>
        /// 指定更新表 后面要实现Set方法
        /// </summary>
        /// <param name="tabname"></param>
        /// <returns></returns>
        public IUpdate Update(string tabname)
        {
            return _context.Update(tabname);

        }
        public IUpdate Update<T>(T objdata)
        {
            return _context.Update<T>(objdata);
        }

        public IUpdate Update<T>(string tabname, List<T> lstobj)
        {
            return _context.Update<T>(tabname, lstobj);
        }
        public IUpdate Update<T>(List<T> lstobj)
        {
            return _context.Update<T>(lstobj);
        }

        public IUpdate Update(string tabname, List<object> lstdata)
        {
            return _context.Update(tabname, lstdata);
        }


        /// <summary>
        /// 批量写入
        /// </summary>
        /// <param name="tabname"></param>
        /// <param name="lstdata"></param>
        /// <returns></returns>
        public int BulkCopyExecCommand<T>(string tabname, List<T> lstdata)
        {
            TabInfo tabInfo = this.Context.DMInitalize.GetTabStruct(tabname);
            return _context.BulkCopyExecCommand(tabInfo, lstdata);

        }
        /// <summary>
        /// 批量写入
        /// </summary>
        /// <param name="tabname"></param>
        /// <param name="lstdata"></param>
        /// <returns></returns>
        public int BulkCopyExecCommand(string tabname, DataTable souretable)
        {
            TabInfo tabInfo = this.Context.DMInitalize.GetTabStruct(tabname);
            return _context.BulkCopyExecCommand(tabInfo, souretable);
        }

 

        public Task<int> BulkCopyExecCommandAsyc(string tabname, DataTable souretable)
        {
            TabInfo tabInfo = this.Context.DMInitalize.GetTabStruct(tabname);
            return _context.BulkCopyExecCommandAsyc(tabInfo, souretable);
        }

        public Task<int> BulkCopyExecCommandAsyc<T>(string tabname, List<T> lstdata)
        {
            TabInfo tabInfo = this.Context.DMInitalize.GetTabStruct(tabname);
            return _context.BulkCopyExecCommandAsyc(tabInfo, lstdata);
        }
        #endregion


        #region 数据删除操作
        public IDelete Delete(string tabname)
        {
            return _context.Delete(tabname);
        }

        public IDelete Delete(string tabname, object objdata)
        {
            return _context.Delete(tabname, objdata);
        }
        public IDelete Delete<T>(string tabname, List<T> objlst)
        {
            return _context.Delete<T>(tabname, objlst);
        }
        public IDelete Delete(string tabname, List<object> objlst) {
            return _context.Delete(tabname, objlst);
        }

        public IDelete TrunCate(string tabname)
        {
            return _context.TrunCate(tabname);

        }

        public IDelete Drop(string tabname)
        {
            return _context.Drop(tabname);

        }

        
        #endregion
    }
}
