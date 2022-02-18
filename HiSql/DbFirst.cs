using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public class DbFirst : IDbFirst
    {


        public HiSqlClient SqlClient
        {
            get
            {
                return _sqlClient;
            }
            set { _sqlClient = value; }
        }

        private HiSqlClient _sqlClient;


        public DbFirst(HiSqlClient sqlClient)
        {
            _sqlClient = sqlClient;
        }

        public DbFirst()
        { 
            
        }

        /// <summary>
        /// 向表中新增一字段列
        /// </summary>
        /// <param name="tabname"></param>
        /// <param name="hiColumn"></param>
        /// <param name="opLevel"></param>
        /// <returns></returns>
        public Tuple<bool, string, string> AddColumn(string tabname, HiColumn hiColumn, OpLevel opLevel)
        {

            bool _isok = false;
            string _msg = "";
            string _sql = "";

            IDM idm = (IDM)Instance.CreateInstance<IDM>($"{Constants.NameSpace}.{_sqlClient.Context.CurrentConnectionConfig.DbType.ToString()}{DbInterFace.DM.ToString()}");

            idm.Context = SqlClient.Context;
            //获取当前最新物理表结构信息
            HiSqlCommProvider.RemoveTabInfoCache(tabname);
            //获取最新
            TabInfo tabinfo= idm.GetTabStruct(tabname);
            if (tabinfo.Columns.Any(c => c.FieldName.ToLower() == hiColumn.FieldName.ToLower()))
            {
                _msg = $"向表[{tabname}]添加新字段[{hiColumn.FieldName}]失败 原因:该字段[{hiColumn.FieldName}]已经存在于表[{tabname}]中";
            }
            else
            {
                _sql = idm.BuildChangeFieldStatement(tabinfo.TabModel, hiColumn, TabFieldAction.ADD);
                if (opLevel == OpLevel.Execute)
                {
                    //执行数据库命令
    
                    _sqlClient.BeginTran();
                    try
                    {
                        _sqlClient.Context.DBO.ExecCommand(_sql, null);
                        _sqlClient.CommitTran();
                        _isok = true;
                        _msg = $"向表[{tabname}]添加字段[{hiColumn.FieldName}]成功";
                    }
                    catch (Exception E)
                    {
                        _sqlClient.RollBackTran();
                        _isok = false;
                        _msg = E.Message.ToString();
                    }
                }
                else {
                    _isok = true;
                    _msg = $"向表[{tabname}]添加新字段[{hiColumn.FieldName}]检测成功";
                }
            }
                _sql = idm.BuildChangeFieldStatement(tabinfo.TabModel, hiColumn, TabFieldAction.ADD);
            return new Tuple<bool, string, string>(_isok, _msg, _sql);
            //throw new NotImplementedException();
        }

        /// <summary>
        /// 向表创建索引
        /// </summary>
        /// <param name="tabname"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public Tuple<bool, string> CreateIndex(string tabname, List<HiColumn> columns)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 向数据库中创建表
        /// </summary>
        /// <param name="tabInfo"></param>
        /// <returns></returns>
        public bool CreateTable(TabInfo tabInfo)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 根据实体类的结构向数据库中创建表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool CreateTable(Type type)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 创建视图
        /// </summary>
        /// <param name="viewname"></param>
        /// <param name="viewsql"></param>
        /// <returns></returns>
        public Tuple<bool, string> CreateView(string viewname, string viewsql)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 对指定中删除某一列的字段
        /// </summary>
        /// <param name="tabname"></param>
        /// <param name="hiColumn"></param>
        /// <param name="opLevel"></param>
        /// <returns></returns>
        public Tuple<bool, string, string> DelColumn(string tabname, HiColumn hiColumn, OpLevel opLevel)
        {
            bool _isok = false;
            string _msg = "";
            string _sql = "";
            IDM idm = (IDM)Instance.CreateInstance<IDM>($"{Constants.NameSpace}.{_sqlClient.Context.CurrentConnectionConfig.DbType.ToString()}{DbInterFace.DM.ToString()}");

            idm.Context = SqlClient.Context;
            //获取当前最新物理表结构信息
            HiSqlCommProvider.RemoveTabInfoCache(tabname);
            //获取最新
            TabInfo tabinfo = idm.GetTabStruct(tabname);
            if (!tabinfo.Columns.Any(c => c.FieldName.ToLower() == hiColumn.FieldName.ToLower()))
            {
                _msg = $"向表[{tabname}]删除字段[{hiColumn.FieldName}]失败 原因:该字段[{hiColumn.FieldName}]不存在于表[{tabname}]中";
            }
            else
            {
                _sql = idm.BuildChangeFieldStatement(tabinfo.TabModel, hiColumn, TabFieldAction.DELETE);
                if (opLevel == OpLevel.Execute)
                {
                    //执行数据库命令

                    _sqlClient.BeginTran();
                    try
                    {
                        _sqlClient.Context.DBO.ExecCommand(_sql, null);
                        _sqlClient.CommitTran();
                        _isok = true;
                        _msg = $"向表[{tabname}]删除字段[{hiColumn.FieldName}]成功";
                    }
                    catch (Exception E)
                    {
                        _sqlClient.RollBackTran();
                        _isok = false;
                        _msg = E.Message.ToString();
                    }
                }
                else
                {
                    _isok = true;
                    _msg = $"向表[{tabname}]删除字段[{hiColumn.FieldName}]检测成功";
                }
            }
            return new Tuple<bool, string, string>(_isok, _msg, _sql);
        }


        /// <summary>
        /// 删除指定的索引
        /// </summary>
        /// <param name="tabname"></param>
        /// <param name="indexname"></param>
        /// <returns></returns>
        public Tuple<bool, string> DelIndex(string tabname, string indexname)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 永久删除指定的表
        /// </summary>
        /// <param name="tabname"></param>
        /// <param name="nolog"></param>
        /// <returns></returns>
        public bool DropTable(string tabname, bool nolog = false)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 获取所有物理表，视图，全局临时表 清单
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllTables()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 获取所有全局临时表
        /// </summary>
        /// <returns></returns>
        public List<string> GetGlobalTempTables()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取所有存储过程清单
        /// </summary>
        /// <returns></returns>
        public List<string> GetStoredProc()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取指定表的所有索引清单
        /// </summary>
        /// <param name="tabname"></param>
        /// <returns></returns>
        public List<string> GetTabIndex(string tabname)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取指定索引的字段
        /// </summary>
        /// <param name="tabname"></param>
        /// <param name="indexname"></param>
        /// <returns></returns>
        public List<HiColumn> GetTabIndexColumn(string tabname, string indexname)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 获取所有物理表清单
        /// </summary>
        /// <returns></returns>
        public List<string> GetTables()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取表或视图的物理表结构信息
        /// </summary>
        /// <param name="tabname"></param>
        /// <returns></returns>
        public DataTable GetTabPhyInfo(string tabname)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取临时表清单
        /// </summary>
        /// <returns></returns>
        public List<string> GetTempTables()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取所有视图清单
        /// </summary>
        /// <returns></returns>
        public List<string> GetViews()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 修改指定表的指定字段
        /// </summary>
        /// <param name="tabname"></param>
        /// <param name="hiColumn"></param>
        /// <param name="opLevel"></param>
        /// <returns></returns>
        public Tuple<bool, string,string> ModiColumn(string tabname, HiColumn hiColumn, OpLevel opLevel)
        {
            bool _isok = false;
            string _msg = "";
            string _sql = "";
            IDM idm = (IDM)Instance.CreateInstance<IDM>($"{Constants.NameSpace}.{_sqlClient.Context.CurrentConnectionConfig.DbType.ToString()}{DbInterFace.DM.ToString()}");

            idm.Context = SqlClient.Context;
            //获取当前最新物理表结构信息
            HiSqlCommProvider.RemoveTabInfoCache(tabname);
            //获取最新
            TabInfo tabinfo = idm.GetTabStruct(tabname);
            if (!tabinfo.Columns.Any(c => c.FieldName.ToLower() == hiColumn.FieldName.ToLower()))
            {
                _msg = $"向表[{tabname}]修改字段[{hiColumn.FieldName}]失败 原因:该字段[{hiColumn.FieldName}]不存在于表[{tabname}]中";
            }
            else
            {
                _sql = idm.BuildChangeFieldStatement(tabinfo.TabModel, hiColumn, TabFieldAction.MODI);
                if (opLevel == OpLevel.Execute)
                {
                    //执行数据库命令
                    _sqlClient.BeginTran();
                    try
                    {
                        _sqlClient.Context.DBO.ExecCommand(_sql, null);
                        _sqlClient.CommitTran();
                        _isok = true;
                        _msg = $"向表[{tabname}]修改字段[{hiColumn.FieldName}]成功";
                    }
                    catch (Exception E)
                    {
                        _sqlClient.RollBackTran();
                        _isok = false;
                        _msg = E.Message.ToString();
                    }

                    

                    
                }
                else
                {
                    _isok = true;
                    _msg = $"向表[{tabname}]修改字段[{hiColumn.FieldName}]检测成功";
                }
            }
            return new Tuple<bool, string,string>(_isok, _msg, _sql);
        }

        /// <summary>
        /// 表不存在则创建，存在则修改
        /// </summary>
        /// <param name="tabInfo"></param>
        /// <param name="opLevel"></param>
        /// <returns></returns>
        public Tuple<bool, string,string> ModiTable(TabInfo tabInfo, OpLevel opLevel)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 无日志删除表数据
        /// </summary>
        /// <param name="tabname"></param>
        /// <returns></returns>
        public bool Truncate(string tabname)
        {
            throw new NotImplementedException();
        }
    }
}
