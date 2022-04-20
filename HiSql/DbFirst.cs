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
        


        Tuple<bool, string, string> addColumn(IDM idm, TabInfo tabInfo, HiColumn hiColumn, OpLevel opLevel)
        {
            bool _isok = false;
            string _msg = "";
            string _sql = "";

            if (tabInfo.Columns.Any(c => c.FieldName.ToLower() == hiColumn.FieldName.ToLower()))
            {
                _msg = $"向表[{tabInfo.TabModel.TabName}]添加新字段[{hiColumn.FieldName}]失败 原因:该字段[{hiColumn.FieldName}]已经存在于表[{tabInfo.TabModel.TabName}]中";
            }
            else
            {
                _sql = idm.BuildChangeFieldStatement(tabInfo.TabModel, hiColumn, TabFieldAction.ADD);
                if (opLevel == OpLevel.Execute)
                {
                    //执行数据库命令

                    _sqlClient.BeginTran();
                    try
                    {
                        _sql = idm.BuildSqlCodeBlock(_sql);
                        _sqlClient.Context.DBO.ExecCommand(_sql, null);
                        _sqlClient.CommitTran();
                        _isok = true;
                        _msg = $"向表[{tabInfo.TabModel.TabName}]添加字段[{hiColumn.FieldName}]成功";
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
                    _msg = $"向表[{tabInfo.TabModel.TabName}]添加新字段[{hiColumn.FieldName}]检测成功";
                }
            }
            //_sql = idm.BuildChangeFieldStatement(tabInfo.TabModel, hiColumn, TabFieldAction.ADD);
            return new Tuple<bool, string, string>(_isok, _msg, _sql);
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

            //bool _isok = false;
            //string _msg = "";
            //string _sql = "";

            IDM idm = (IDM)Instance.CreateInstance<IDM>($"{Constants.NameSpace}.{_sqlClient.Context.CurrentConnectionConfig.DbType.ToString()}{DbInterFace.DM.ToString()}");

            idm.Context = SqlClient.Context;
            //获取当前最新物理表结构信息
            HiSqlCommProvider.RemoveTabInfoCache(tabname);
            //获取最新
            TabInfo tabinfo = idm.GetTabStruct(tabname);
            return addColumn(idm, tabinfo, hiColumn, opLevel);
        }

        /// <summary>
        /// 向表创建索引
        /// </summary>
        /// <param name="tabname"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public Tuple<bool, string, string> CreateIndex(string tabname, string indexname, List<HiColumn> columns, OpLevel opLevel)
        {
            bool _isok = false;
            string _msg = "";
            string _sql = "";
            IDM idm = (IDM)Instance.CreateInstance<IDM>($"{Constants.NameSpace}.{_sqlClient.Context.CurrentConnectionConfig.DbType.ToString()}{DbInterFace.DM.ToString()}");

            idm.Context = SqlClient.Context;

            List<TabIndex> lstindex = idm.GetIndexs(tabname);
            if (lstindex.Any(i => i.IndexName.ToLower() == indexname.ToLower()))
                _msg = $"索引名称[{indexname}]已经存在,不允重复创建";
            else
            {
                if (columns.Count == 0)
                {
                    _msg = $"创建索引必须要指定列";

                    return new Tuple<bool, string, string>(_isok, _msg, _sql);
                }
                else
                {
                    _sql = idm.CreateIndex(tabname, indexname, columns);
                }
                if (idm.Context.CurrentConnectionConfig.UpperCase)
                {
                    _sql = _sql.ToUpper();

                }
                if (opLevel == OpLevel.Execute)
                {
                    _sqlClient.BeginTran();
                    try
                    {
                        _sqlClient.Context.DBO.ExecCommand(_sql);
                        _sqlClient.CommitTran();
                        _isok = true;
                        _msg = $"为表[{tabname}]创建索引[{indexname}]成功";
                    }
                    catch (Exception E)
                    {
                        _msg = E.Message.ToString();
                        _sqlClient.RollBackTran();
                    }
                }
                else
                {
                    _isok = true;
                    _msg = $"为表[{tabname}]创建索引[{indexname}]检测成功";
                }

            }
            //return idm.CreateIndex(tabname, indexname, columns)

            return new Tuple<bool, string, string>(_isok, _msg, _sql);
        }


        /// <summary>
        /// 创建表
        /// 可自行构建该类可实现动态创建类
        /// </summary>
        /// <param name="tabInfo"></param>
        /// <returns></returns>
        public bool CreateTable(TabInfo tabInfo)
        {
            if (_sqlClient != null)
            {
                return _sqlClient.Context.DMInitalize.BuildTabCreate(tabInfo) > 0;


            }
            else
                throw new Exception($"请先指定数据库连接!");
        }

        /// <summary>
        /// 根据实体类型创建表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool CreateTable(Type type)
        {
            if (_sqlClient != null)
            {
                TabInfo tabInfo = _sqlClient.Context.DMInitalize.BuildTab(type);
                return _sqlClient.Context.DMInitalize.BuildTabCreate(tabInfo) > 0;
            }
            else
                throw new Exception($"请先指定数据库连接!");
        }

        /// <summary>
        /// 创建视图
        /// </summary>
        /// <param name="viewname"></param>
        /// <param name="viewsql"></param>
        /// <returns></returns>
        public Tuple<bool, string, string> CreateView(string viewname, string viewsql, OpLevel opLevel)
        {
            bool _isok = false;
            string _msg = "";
            string _sql = "";
            IDM idm = (IDM)Instance.CreateInstance<IDM>($"{Constants.NameSpace}.{_sqlClient.Context.CurrentConnectionConfig.DbType.ToString()}{DbInterFace.DM.ToString()}");
            idm.Context = SqlClient.Context;

            if (_sqlClient != null)
            {
                _sql = idm.CreateView(viewname, viewsql);
                if (idm.Context.CurrentConnectionConfig.UpperCase)
                {
                    _sql = _sql.ToUpper();

                }
                if (opLevel == OpLevel.Execute)
                {
                    _sqlClient.BeginTran();
                    try
                    {
                        _sqlClient.Context.DBO.ExecCommand(_sql, null);
                        _sqlClient.CommitTran();
                        _isok = true;
                        _msg = $"视图[{viewname}]创建成功!";
                    }
                    catch (Exception E)
                    {
                        _sqlClient.RollBackTran();
                        _isok = false;
                        _msg = $"视图[{viewname}]创建失败：{E.Message.ToString()}";
                    }
                }
                else
                {
                    _isok = true;
                    _msg = $"视图[{viewname}]创建检测成功!";
                }
            }
            else
                throw new Exception($"请先指定数据库连接!");


            return new Tuple<bool, string, string>(_isok, _msg, _sql);
        }

        public Tuple<bool, string, string> ModiView(string viewname, string viewsql, OpLevel opLevel)
        {
            bool _isok = false;
            string _msg = "";
            string _sql = "";
            IDM idm = (IDM)Instance.CreateInstance<IDM>($"{Constants.NameSpace}.{_sqlClient.Context.CurrentConnectionConfig.DbType.ToString()}{DbInterFace.DM.ToString()}");
            idm.Context = SqlClient.Context;

            if (_sqlClient != null)
            {
                _sql = idm.ModiView(viewname, viewsql);
                if (idm.Context.CurrentConnectionConfig.UpperCase)
                    _sql = _sql.ToUpper();

                if (opLevel == OpLevel.Execute)
                {
                    _sqlClient.BeginTran();
                    try
                    {
                        _sqlClient.Context.DBO.ExecCommand(_sql, null);
                        _sqlClient.CommitTran();
                        _isok = true;
                        _msg = $"视图[{viewname}]修改成功 !";
                    }
                    catch (Exception E)
                    {
                        _sqlClient.RollBackTran();
                        _isok = false;
                        _msg = $"视图[{viewname}]修改失败：{E.Message.ToString()}";
                    }
                }
                else
                {
                    _isok = true;
                    _msg = $"视图[{viewname}]修改检测成功!";
                }
            }
            else
                throw new Exception($"请先指定数据库连接!");


            return new Tuple<bool, string, string>(_isok, _msg, _sql);
        }


        public Tuple<bool, string, string> DropView(string viewname, OpLevel opLevel)
        {

            bool _isok = false;
            string _msg = "";
            string _sql = "";
            IDM idm = (IDM)Instance.CreateInstance<IDM>($"{Constants.NameSpace}.{_sqlClient.Context.CurrentConnectionConfig.DbType.ToString()}{DbInterFace.DM.ToString()}");
            idm.Context = SqlClient.Context;

            if (_sqlClient != null)
            {
                _sql = idm.DropView(viewname);
                if (idm.Context.CurrentConnectionConfig.UpperCase)
                    _sql = _sql.ToUpper();
                if (opLevel == OpLevel.Execute)
                {
                    _sqlClient.BeginTran();
                    try
                    {
                        _sqlClient.Context.DBO.ExecCommand(_sql, null);
                        _sqlClient.CommitTran();
                        _isok = true;
                        _msg = $"视图[{viewname}]删除成功 !";
                    }
                    catch (Exception E)
                    {
                        _sqlClient.RollBackTran();
                        _isok = false;
                        _msg = $"视图[{viewname}]删除失败：{E.Message.ToString()}";
                    }
                }
                else
                {
                    _isok = true;
                    _msg = $"视图[{viewname}]删除检测成功!";
                }
            }
            else
                throw new Exception($"请先指定数据库连接!");


            return new Tuple<bool, string, string>(_isok, _msg, _sql);
        }


        Tuple<bool, string, string> delColumn(IDM idm, TabInfo tabInfo, HiColumn hiColumn, OpLevel opLevel)
        {
            bool _isok = false;
            string _msg = "";
            string _sql = "";
            if (!tabInfo.Columns.Any(c => c.FieldName.ToLower() == hiColumn.FieldName.ToLower()))
            {
                _msg = $"向表[{tabInfo.TabModel.TabName}]删除字段[{hiColumn.FieldName}]失败 原因:该字段[{hiColumn.FieldName}]不存在于表[{tabInfo.TabModel.TabName}]中";
            }
            else
            {
                _sql = idm.BuildChangeFieldStatement(tabInfo.TabModel, hiColumn, TabFieldAction.DELETE);
                if (idm.Context.CurrentConnectionConfig.UpperCase)
                    _sql = _sql.ToUpper();
                if (opLevel == OpLevel.Execute)
                {
                    //执行数据库命令

                    _sqlClient.BeginTran();
                    try
                    {
                        _sqlClient.Context.DBO.ExecCommand(_sql, null);
                        _sqlClient.CommitTran();
                        _isok = true;
                        _msg = $"向表[{tabInfo.TabModel.TabName}]删除字段[{hiColumn.FieldName}]成功";
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
                    _msg = $"向表[{tabInfo.TabModel.TabName}]删除字段[{hiColumn.FieldName}]检测成功";
                }
            }
            return new Tuple<bool, string, string>(_isok, _msg, _sql);
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
            return delColumn(idm, tabinfo, hiColumn, opLevel);
        }


        /// <summary>
        /// 删除指定的索引
        /// </summary>
        /// <param name="tabname"></param>
        /// <param name="indexname"></param>
        /// <returns></returns>
        public Tuple<bool, string, string> DelIndex(string tabname, string indexname, OpLevel opLevel)
        {
            bool _isok = false;
            string _msg = "";
            string _sql = "";
            IDM idm = (IDM)Instance.CreateInstance<IDM>($"{Constants.NameSpace}.{_sqlClient.Context.CurrentConnectionConfig.DbType.ToString()}{DbInterFace.DM.ToString()}");
            idm.Context = SqlClient.Context;

            if (idm.Context.CurrentConnectionConfig.UpperCase)
            {
                tabname = tabname.ToUpper();
            }

            List<TabIndex> lst = idm.GetIndexs(tabname);

            _sql = idm.DropIndex(tabname, indexname);
            if (!lst.Any(t => t.IndexName.ToLower() == indexname.ToLower()))
            {
                _msg = $"索引[{indexname}]不存在于表[{tabname}]中";
            }
            else
            {
                if (idm.Context.CurrentConnectionConfig.UpperCase)
                {
                    _sql = _sql.ToUpper();

                }
                if (opLevel == OpLevel.Execute)
                {
                    //执行数据库命令

                    _sqlClient.BeginTran();
                    try
                    {


                        _sqlClient.Context.DBO.ExecCommand(_sql, null);
                        _sqlClient.CommitTran();
                        _isok = true;
                        _msg = $"删除索引[{indexname}]成功";
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
                    _msg = $"删除索引[{indexname}]检测成功";
                }

            }
            return new Tuple<bool, string, string>(_isok, _msg, _sql);
        }

        /// <summary>
        /// 永久删除指定的表
        /// </summary>
        /// <param name="tabname"></param>
        /// <param name="nolog"></param>
        /// <returns></returns>
        public bool DropTable(string tabname, bool nolog = false)
        {
            if (_sqlClient != null)
            {
                var _table = new TableDefinition(tabname);
                if (_table.TableType == TableType.Entity)
                {
                    if (nolog)
                        _sqlClient.TrunCate(tabname).ExecCommand();
                    int v = _sqlClient.Drop(tabname).ExecCommand();
                    _sqlClient.Delete(Constants.HiSysTable["Hi_TabModel"].ToString(), new { TabName = tabname }).ExecCommand();
                    _sqlClient.Delete(Constants.HiSysTable["Hi_FieldModel"].ToString()).Where($"TabName='{tabname.ToSqlInject()}'").ExecCommand();
                    return v > 0;
                }
                else
                    return _sqlClient.Drop(tabname).ExecCommand() > 0;
            }
            else
                throw new Exception($"请先指定数据库连接!");
        }
        /// <summary>
        /// 获取表大小
        /// </summary>
        /// <returns></returns>
        public int GetTableDataCount(string tabname)
        {
            IDM idm = (IDM)Instance.CreateInstance<IDM>($"{Constants.NameSpace}.{_sqlClient.Context.CurrentConnectionConfig.DbType.ToString()}{DbInterFace.DM.ToString()}");
            idm.Context = SqlClient.Context;
            int dataCount = idm.GetTableDataCount(tabname);
            return dataCount;
        }
        /// <summary>
        /// 获取所有物理表，视图，全局临时表 清单
        /// </summary>
        /// <returns></returns>
        public List<TableInfo> GetAllTables()
        {
            IDM idm = (IDM)Instance.CreateInstance<IDM>($"{Constants.NameSpace}.{_sqlClient.Context.CurrentConnectionConfig.DbType.ToString()}{DbInterFace.DM.ToString()}");
            idm.Context = SqlClient.Context;
            DataTable dt = idm.GetAllTables();
            List<string> lst = new List<string>();
            List<TableInfo> lsttabinfo = new List<TableInfo>();
            foreach (DataRow dr in dt.Rows)
            {

                lst.Add(dr["TabName"].ToString());
            }

            //根据获取的物理表信息获取虚拟表结构信息
            DataTable dataTable = _sqlClient.Query("Hi_TabModel").Field("*").Where(new Filter {
                {"TabName",OperType.IN, lst }
            }).Sort("TabName asc").ToTable();

            foreach (DataRow dr in dt.Rows)
            {
                TableInfo tableInfo = new TableInfo();

                tableInfo.TabName = dr["TabName"].ToString();
                var _dr = dataTable.AsEnumerable().Where(p => p.Field<string>("TabName") == tableInfo.TabName).FirstOrDefault();
                if (_dr != null)
                {
                    tableInfo.TabReName = _dr["TabReName"].ToString();
                    tableInfo.TabDescript = _dr["TabDescript"].ToString();
                    tableInfo.HasTabStruct = true;

                }
                else
                {
                    tableInfo.TabReName = tableInfo.TabName;
                    tableInfo.TabDescript = tableInfo.TabName;
                }
                tableInfo.Schema = _sqlClient.Context.CurrentConnectionConfig.Schema;

                if (dr["TabType"].ToString() == "View")
                    tableInfo.TableType = TableType.View;
                else
                    tableInfo.TableType = TableType.Entity;
                lsttabinfo.Add(tableInfo);
            }
            return lsttabinfo;
        }
        public List<TableInfo> GetAllTables(string viewName, int pageSize, int pageIndex, out int totalCount)
        {
            IDM idm = (IDM)Instance.CreateInstance<IDM>($"{Constants.NameSpace}.{_sqlClient.Context.CurrentConnectionConfig.DbType.ToString()}{DbInterFace.DM.ToString()}");
            idm.Context = SqlClient.Context;
            DataTable dt = idm.GetAllTables(viewName, pageSize, pageIndex, out totalCount);
            List<string> lst = new List<string>();
            List<TableInfo> lsttabinfo = new List<TableInfo>();
            foreach (DataRow dr in dt.Rows)
            {

                lst.Add(dr["TabName"].ToString());
            }

            //根据获取的物理表信息获取虚拟表结构信息
            DataTable dataTable = _sqlClient.Query("Hi_TabModel").Field("*").Where(new Filter {
                {"TabName",OperType.IN, lst }
            }).Sort("TabName asc").ToTable();

            foreach (DataRow dr in dt.Rows)
            {
                TableInfo tableInfo = new TableInfo();

                tableInfo.TabName = dr["TabName"].ToString();
                var _dr = dataTable.AsEnumerable().Where(p => p.Field<string>("TabName") == tableInfo.TabName).FirstOrDefault();
                if (_dr != null)
                {
                    tableInfo.TabReName = _dr["TabReName"].ToString();
                    tableInfo.TabDescript = _dr["TabDescript"].ToString();
                    tableInfo.HasTabStruct = true;

                }
                else
                {
                    tableInfo.TabReName = tableInfo.TabName;
                    tableInfo.TabDescript = tableInfo.TabName;
                }
                tableInfo.Schema = _sqlClient.Context.CurrentConnectionConfig.Schema;

                if (dr["TabType"].ToString() == "View")
                    tableInfo.TableType = TableType.View;
                else
                    tableInfo.TableType = TableType.Entity;
                lsttabinfo.Add(tableInfo);
            }
            return lsttabinfo;
        }



        /// <summary>
        /// 获取所有全局临时表
        /// </summary>
        /// <returns></returns>
        public List<TableInfo> GetGlobalTempTables()
        {
            IDM idm = (IDM)Instance.CreateInstance<IDM>($"{Constants.NameSpace}.{_sqlClient.Context.CurrentConnectionConfig.DbType.ToString()}{DbInterFace.DM.ToString()}");
            idm.Context = SqlClient.Context;
            DataTable dt = idm.GetGlobalTables();

            List<string> lst = new List<string>();
            List<TableInfo> lsttabinfo = new List<TableInfo>();
            foreach (DataRow dr in dt.Rows)
            {

                lst.Add(dr["TabName"].ToString());
            }

            //根据获取的物理表信息获取虚拟表结构信息
            if (lst.Count == 0)
                lst.Add("hi_00000000001");
            DataTable dataTable = _sqlClient.Query("Hi_TabModel").Field("*").Where(new Filter {
                {"TabName",OperType.IN, lst }
            }).Sort("TabName asc").ToTable();

            foreach (DataRow dr in dt.Rows)
            {
                TableInfo tableInfo = new TableInfo();

                tableInfo.TabName = dr["TabName"].ToString();
                tableInfo.TabName = dr["TabName"].ToString();
                var _dr = dataTable.AsEnumerable().Where(p => p.Field<string>("TabName") == tableInfo.TabName).FirstOrDefault();
                if (_dr != null)
                {
                    tableInfo.TabReName = _dr["TabReName"].ToString();
                    tableInfo.TabDescript = _dr["TabDescript"].ToString();

                    tableInfo.HasTabStruct = true;
                }
                else
                {
                    tableInfo.TabReName = tableInfo.TabName;
                    tableInfo.TabDescript = tableInfo.TabName;
                }
                tableInfo.Schema = _sqlClient.Context.CurrentConnectionConfig.Schema;
                lsttabinfo.Add(tableInfo);
            }

            return lsttabinfo;

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
        public List<TabIndex> GetTabIndexs(string tabname)
        {
            IDM idm = (IDM)Instance.CreateInstance<IDM>($"{Constants.NameSpace}.{_sqlClient.Context.CurrentConnectionConfig.DbType.ToString()}{DbInterFace.DM.ToString()}");
            idm.Context = SqlClient.Context;

            return idm.GetIndexs(tabname);
        }

        /// <summary>
        /// 获取指定索引的字段
        /// </summary>
        /// <param name="tabname"></param>
        /// <param name="indexname"></param>
        /// <returns></returns>
        public List<TabIndexDetail> GetTabIndexDetail(string tabname, string indexname)
        {
            IDM idm = (IDM)Instance.CreateInstance<IDM>($"{Constants.NameSpace}.{_sqlClient.Context.CurrentConnectionConfig.DbType.ToString()}{DbInterFace.DM.ToString()}");
            idm.Context = SqlClient.Context;

            return idm.GetIndexDetails(tabname, indexname);
        }


        /// <summary>
        /// 获取所有物理表清单
        /// </summary>
        /// <returns></returns>
        public List<TableInfo> GetTables()
        {
            IDM idm = (IDM)Instance.CreateInstance<IDM>($"{Constants.NameSpace}.{_sqlClient.Context.CurrentConnectionConfig.DbType.ToString()}{DbInterFace.DM.ToString()}");
            idm.Context = SqlClient.Context;
            DataTable dt = idm.GetTableList();
            List<string> lst = new List<string>();
            List<TableInfo> lsttabinfo = new List<TableInfo>();
            foreach (DataRow dr in dt.Rows)
            {

                lst.Add(dr["TabName"].ToString());
            }

            //根据获取的物理表信息获取虚拟表结构信息
            DataTable dataTable = _sqlClient.Query("Hi_TabModel").Field("*").Where(new Filter {
                {"TabName",OperType.IN, lst }
            }).Sort("TabName asc").ToTable();

            foreach (DataRow dr in dt.Rows)
            {
                TableInfo tableInfo = new TableInfo();

                tableInfo.TabName = dr["TabName"].ToString();
                tableInfo.TabName = dr["TabName"].ToString();
                var _dr = dataTable.AsEnumerable().Where(p => p.Field<string>("TabName") == tableInfo.TabName).FirstOrDefault();
                if (_dr != null)
                {
                    tableInfo.TabReName = _dr["TabReName"].ToString();
                    tableInfo.TabDescript = _dr["TabDescript"].ToString();

                    tableInfo.HasTabStruct = true;
                }
                else
                {
                    tableInfo.TabReName = tableInfo.TabName;
                    tableInfo.TabDescript = tableInfo.TabName;
                }
                tableInfo.Schema = _sqlClient.Context.CurrentConnectionConfig.Schema;
                lsttabinfo.Add(tableInfo);
            }

            return lsttabinfo;
        }

        /// <summary>
        /// 获取所有物理表，视图，全局临时表 清单
        /// </summary>
        /// <returns></returns>
        public List<TableInfo> GetTables(string tableName, int pageSize, int pageIndex, out int totalCount)
        {

            IDM idm = (IDM)Instance.CreateInstance<IDM>($"{Constants.NameSpace}.{_sqlClient.Context.CurrentConnectionConfig.DbType.ToString()}{DbInterFace.DM.ToString()}");
            idm.Context = SqlClient.Context;
            DataTable dt = idm.GetTableList(tableName, pageSize, pageIndex, out totalCount);
            List<string> lst = new List<string>();
            List<TableInfo> lsttabinfo = new List<TableInfo>();
            foreach (DataRow dr in dt.Rows)
            {
                lst.Add(dr["TabName"].ToString());
            }

            //根据获取的物理表信息获取虚拟表结构信息
            if (lst.Count > 0)
            {
                DataTable dataTable = _sqlClient.Query("Hi_TabModel").Field("*").Where(new Filter {
                {"TabName",OperType.IN, lst }
            }).Sort("TabName asc").ToTable();

                foreach (DataRow dr in dt.Rows)
                {
                    TableInfo tableInfo = new TableInfo();

                    tableInfo.TabName = dr["TabName"].ToString();
                    var _dr = dataTable.AsEnumerable().Where(p => p.Field<string>("TabName") == tableInfo.TabName).FirstOrDefault();
                    if (_dr != null)
                    {
                        tableInfo.TabReName = _dr["TabReName"].ToString();
                        tableInfo.TabDescript = _dr["TabDescript"].ToString();
                        tableInfo.HasTabStruct = true;
                    }
                    else
                    {
                        tableInfo.TabReName = tableInfo.TabName;
                        tableInfo.TabDescript = tableInfo.TabName;
                    }
                    tableInfo.Schema = _sqlClient.Context.CurrentConnectionConfig.Schema;

                    if (dr["TabType"].ToString() == "View")
                        tableInfo.TableType = TableType.View;
                    else
                        tableInfo.TableType = TableType.Entity;
                    lsttabinfo.Add(tableInfo);
                }
            }


            return lsttabinfo;
        }



        /// <summary>
        /// 获取表或视图的物理表结构信息
        /// </summary>
        /// <param name="tabname"></param>
        /// <returns></returns>
        public DataTable GetTabPhyInfo(string tabname)
        {
            IDM idm = (IDM)Instance.CreateInstance<IDM>($"{Constants.NameSpace}.{_sqlClient.Context.CurrentConnectionConfig.DbType.ToString()}{DbInterFace.DM.ToString()}");
            idm.Context = SqlClient.Context;
            return idm.GetTableDefinition(tabname);

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
        public List<TableInfo> GetViews()
        {
            IDM idm = (IDM)Instance.CreateInstance<IDM>($"{Constants.NameSpace}.{_sqlClient.Context.CurrentConnectionConfig.DbType.ToString()}{DbInterFace.DM.ToString()}");
            idm.Context = SqlClient.Context;
            DataTable dt = idm.GetViewList();
            List<string> lst = new List<string>();
            List<TableInfo> lsttabinfo = new List<TableInfo>();
            foreach (DataRow dr in dt.Rows)
            {

                lst.Add(dr["TabName"].ToString());
            }

            if (lst.Count > 0)
            {


                //根据获取的物理表信息获取虚拟表结构信息
                DataTable dataTable = _sqlClient.Query("Hi_TabModel").Field("*").Where(new Filter {
                {"TabName",OperType.IN, lst }
            }).Sort("TabName asc").ToTable();

                foreach (DataRow dr in dt.Rows)
                {
                    TableInfo tableInfo = new TableInfo();

                    tableInfo.TabName = dr["TabName"].ToString();
                    var _dr = dataTable.AsEnumerable().Where(p => p.Field<string>("TabName") == tableInfo.TabName).FirstOrDefault();
                    if (_dr != null)
                    {
                        tableInfo.TabReName = _dr["TabReName"].ToString();
                        tableInfo.TabDescript = _dr["TabDescript"].ToString();
                        tableInfo.HasTabStruct = true;

                    }
                    else
                    {
                        tableInfo.TabReName = tableInfo.TabName;
                        tableInfo.TabDescript = tableInfo.TabName;
                    }
                    tableInfo.Schema = _sqlClient.Context.CurrentConnectionConfig.Schema;
                    tableInfo.TableType = TableType.View;
                    lsttabinfo.Add(tableInfo);
                }
            }
            return lsttabinfo;
        }
        /// <summary>
        /// 获取所有视图清单
        /// </summary>
        /// <returns></returns>
        public List<TableInfo> GetViews(string viewName, int pageSize, int pageIndex, out int totalCount)
        {
            IDM idm = (IDM)Instance.CreateInstance<IDM>($"{Constants.NameSpace}.{_sqlClient.Context.CurrentConnectionConfig.DbType.ToString()}{DbInterFace.DM.ToString()}");
            idm.Context = SqlClient.Context;
            DataTable dt = idm.GetViewList(viewName, pageSize, pageIndex, out totalCount);
            List<string> lst = new List<string>();
            List<TableInfo> lsttabinfo = new List<TableInfo>();
            foreach (DataRow dr in dt.Rows)
            {

                lst.Add(dr["TabName"].ToString());
            }

            if (lst.Count > 0)
            {


                //根据获取的物理表信息获取虚拟表结构信息
                DataTable dataTable = _sqlClient.Query("Hi_TabModel").Field("*").Where(new Filter {
                {"TabName",OperType.IN, lst }
            }).Sort("TabName asc").ToTable();

                foreach (DataRow dr in dt.Rows)
                {
                    TableInfo tableInfo = new TableInfo();

                    tableInfo.TabName = dr["TabName"].ToString();
                    var _dr = dataTable.AsEnumerable().Where(p => p.Field<string>("TabName") == tableInfo.TabName).FirstOrDefault();
                    if (_dr != null)
                    {
                        tableInfo.TabReName = _dr["TabReName"].ToString();
                        tableInfo.TabDescript = _dr["TabDescript"].ToString();
                        tableInfo.HasTabStruct = true;

                    }
                    else
                    {
                        tableInfo.TabReName = tableInfo.TabName;
                        tableInfo.TabDescript = tableInfo.TabName;
                    }
                    tableInfo.Schema = _sqlClient.Context.CurrentConnectionConfig.Schema;
                    tableInfo.TableType = TableType.View;
                    lsttabinfo.Add(tableInfo);
                }
            }
            return lsttabinfo;
        }
        Tuple<bool, string, string> modiColumn(IDM idm, TabInfo tabInfo, HiColumn hiColumn, OpLevel opLevel)
        {
            bool _isok = false;
            string _msg = "";
            string _sql = "";
            var field = tabInfo.Columns.FirstOrDefault(c => c.FieldName.ToLower() == hiColumn.FieldName.ToLower());
            if (field == null)
            {
                _msg = $"向表[{tabInfo.TabModel.TabName}]修改字段[{hiColumn.FieldName}]失败 原因:该字段[{hiColumn.FieldName}]不存在于表[{tabInfo.TabModel.TabName}]中";
            }
            else
            {
                if (field.FieldType != hiColumn.FieldType
                    && !Constants.HiTypeAllowConvertDefinition[field.FieldType].Contains(hiColumn.FieldType)
                    )
                {

                    _isok = false;
                    _msg = $"表【{hiColumn.TabName}】的字段【{hiColumn.FieldName}】类型【{field.FieldType.ToString()}】不允许转换成【{hiColumn.FieldType.ToString()}】";
                    return new Tuple<bool, string, string>(_isok, _msg, _sql);
                }
                _sql = idm.BuildChangeFieldStatement(tabInfo.TabModel, hiColumn, TabFieldAction.MODI);
                if (opLevel == OpLevel.Execute)
                {
                    //执行数据库命令
                    _sqlClient.BeginTran();
                    try
                    {
                        _sql = idm.BuildSqlCodeBlock(_sql);
                        _sqlClient.Context.DBO.ExecCommand(_sql, null);
                        _sqlClient.CommitTran();
                        _isok = true;
                        _msg = $"向表[{tabInfo.TabModel.TabName}]修改字段[{hiColumn.FieldName}]成功";
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
                    _msg = $"向表[{tabInfo.TabModel.TabName}]修改字段[{hiColumn.FieldName}]检测成功";
                }
            }
            return new Tuple<bool, string, string>(_isok, _msg, _sql);
        }

        /// <summary>
        /// 修改指定表的指定字段
        /// </summary>
        /// <param name="tabname"></param>
        /// <param name="hiColumn"></param>
        /// <param name="opLevel"></param>
        /// <returns></returns>
        public Tuple<bool, string, string> ModiColumn(string tabname, HiColumn hiColumn, OpLevel opLevel)
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


            return modiColumn(idm, tabinfo, hiColumn, opLevel);

        }

        Tuple<bool, string, string> reColumn(IDM idm, TabInfo tabInfo, HiColumn hiColumn, OpLevel opLevel)
        {
            bool _isok = false;
            string _msg = "";
            string _sql = "";

            if (hiColumn.FieldName.ToLower() != hiColumn.ReFieldName.ToLower() && !string.IsNullOrEmpty(hiColumn.ReFieldName))
            {
                if (!tabInfo.Columns.Any(c => c.FieldName.ToLower() == hiColumn.FieldName.ToLower()))
                {
                    _msg = $"向表[{tabInfo.TabModel.TabName}]-[{hiColumn.FieldName}]重命名为字段[{hiColumn.ReFieldName}]失败 原因:该字段[{hiColumn.FieldName}]不存在于表[{tabInfo.TabModel.TabName}]中";
                }
                else
                {

                    //重命名字段不在表中才可以进行重命名
                    if (!tabInfo.Columns.Any(c => c.FieldName.ToLower() == hiColumn.ReFieldName.ToLower()))
                    {
                        hiColumn.FieldDesc = hiColumn.FieldDesc.IsNullOrEmpty() ? hiColumn.FieldName : hiColumn.FieldDesc;
                        _sql = idm.BuildChangeFieldStatement(tabInfo.TabModel, hiColumn, TabFieldAction.RENAME);
                        // 在 BuildChangeFieldStatement 中处理 by pengxy on 20220321
                        //{
                        //    hiColumn.FieldName = hiColumn.ReFieldName;
                        //    _sql += System.Environment.NewLine + idm.BuildChangeFieldStatement(tabInfo.TabModel, hiColumn, TabFieldAction.MODI);
                        //}

                        _isok = true;
                        if (opLevel == OpLevel.Execute)
                        {
                            //执行数据库命令
                            _sqlClient.BeginTran();
                            try
                            {
                                _sql = idm.BuildSqlCodeBlock(_sql);
                                _sqlClient.Context.DBO.ExecCommand(_sql, null);
                                _sqlClient.CommitTran();
                                _isok = true;
                                _msg = $"向表[{tabInfo.TabModel.TabName}]-[{hiColumn.FieldName}]重命名为字段[{hiColumn.ReFieldName}]成功";
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
                            _msg = $"向表[{tabInfo.TabModel.TabName}]-[{hiColumn.FieldName}]重命名为字段[{hiColumn.ReFieldName}]检测成功";
                        }
                    }
                    else
                    {
                        _msg = $"向表[{tabInfo.TabModel.TabName}]-[{hiColumn.FieldName}]重命名为字段[{hiColumn.ReFieldName}]失败 原因:该字段[{hiColumn.ReFieldName}] 已经存在";
                    }
                }
            }
            else
            {
                _isok = false;
                _msg = $"字段[{hiColumn.FieldName}]未重命名";
            }
            return new Tuple<bool, string, string>(_isok, _msg, _sql);
        }

        /// <summary>
        /// 字段重命名
        /// </summary>
        /// <param name="tabname"></param>
        /// <param name="hiColumn"></param>
        /// <param name="opLevel"></param>
        /// <returns></returns>
        public Tuple<bool, string, string> ReColumn(string tabname, HiColumn hiColumn, OpLevel opLevel)
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

            return reColumn(idm, tabinfo, hiColumn, opLevel);
        }
        /// <summary>
        /// 表不存在则创建，存在则修改
        /// </summary>
        /// <param name="tabInfo"></param>
        /// <param name="opLevel"></param>
        /// <returns></returns>
        public Tuple<bool, string, string> ModiTable(TabInfo tabInfo, OpLevel opLevel)
        {
            TabInfo tab = _sqlClient.Context.DMInitalize.GetTabStruct(tabInfo.TabModel.TabName);


            IDM idm = (IDM)Instance.CreateInstance<IDM>($"{Constants.NameSpace}.{_sqlClient.Context.CurrentConnectionConfig.DbType.ToString()}{DbInterFace.DM.ToString()}");

            idm.Context = SqlClient.Context;
            //获取当前最新物理表结构信息
            HiSqlCommProvider.RemoveTabInfoCache(tabInfo.TabModel.TabName);
            //获取最新
            TabInfo tabinfo = idm.GetTabStruct(tabInfo.TabModel.TabName);
            List<FieldChange> fieldChanges = HiSqlCommProvider.TabToCompare(tabInfo, tab);

            bool _isok = true;
            string _msg = "";
            string _sql = "";

            StringBuilder sb_sql = new StringBuilder();

            bool _tabchange = false;
            var changes = fieldChanges.Where(f => f.Action != TabFieldAction.NONE);


            List<HiColumn> lstchg = new List<HiColumn>();
            List<HiColumn> lstdel = new List<HiColumn>();

            foreach (FieldChange field in changes)
            {
                if (!_isok) break;

                var hicol = tabInfo.Columns.Where(c => c.FieldName.ToLower().Equals(field.FieldName.ToLower())).FirstOrDefault();
                if (field.IsTabChange)
                {
                    #region 涉及物理表变更


                    if (hicol != null)
                    {
                        if (field.Action == TabFieldAction.ADD)
                        {
                            var addrtn = addColumn(idm, tab, hicol, OpLevel.Check);
                            if (addrtn.Item1)
                            {
                                sb_sql.AppendLine(addrtn.Item3);
                                lstchg.Add(hicol);
                                _tabchange = true;
                            }
                            else
                            {
                                _msg = addrtn.Item2;
                                _isok = false;
                            }
                        }
                        else if (field.Action == TabFieldAction.MODI)
                        {
                            var modirtn = modiColumn(idm, tab, hicol, OpLevel.Check);
                            if (modirtn.Item1)
                            {
                                sb_sql.AppendLine(modirtn.Item3);
                                lstchg.Add(hicol);
                                _tabchange = true;
                            }
                            else
                            {
                                _msg = modirtn.Item2;
                                _isok = false;
                            }
                        }
                        else if (field.Action == TabFieldAction.DELETE)
                        {
                            var delrtn = delColumn(idm, tabInfo, hicol, OpLevel.Check);
                            if (delrtn.Item1)
                            {
                                sb_sql.AppendLine(delrtn.Item3);
                                lstdel.Add(hicol);
                                _tabchange = true;
                            }
                            else
                            {
                                _msg = delrtn.Item2;
                                _isok = false;
                            }
                        }
                        else if (field.Action == TabFieldAction.RENAME)
                        {
                            var rertn = reColumn(idm, tabInfo, hicol, OpLevel.Check);
                            if (rertn.Item1)
                            {
                                sb_sql.AppendLine(rertn.Item3);
                                //var _tmpcol = ClassExtensions.DeepCopy<HiColumn>(hicol);
                                //lstdel.Add(hicol);
                                //_tmpcol.FieldName = _tmpcol.ReFieldName;
                                lstchg.Add(hicol);
                                _tabchange = true;


                            }

                            else
                            {
                                _msg = rertn.Item2;
                                _isok = false;
                            }

                        }
                        else
                        {
                            _isok = false;
                            _msg = $"不支持{field.Action}";
                        }
                    }
                    else
                    {

                        if (field.Action == TabFieldAction.DELETE)
                        {
                            var _hicol = tab.Columns.Where(c => c.FieldName.ToLower().Equals(field.FieldName.ToLower())).FirstOrDefault();
                            if (_hicol != null)
                            {


                                var delrtn = delColumn(idm, tab, _hicol, OpLevel.Check);
                                if (delrtn.Item1)
                                {
                                    sb_sql.AppendLine(delrtn.Item3);
                                    lstdel.Add(_hicol);
                                    _tabchange = true;
                                }
                                else
                                {
                                    _msg = delrtn.Item2;
                                    _isok = false;
                                }
                            }
                            else
                            {
                                _msg = $"未能匹配字段[{field.FieldName}]";
                                _isok = false;
                            }
                        }
                        else
                        {
                            _msg = $"字段[{field.FieldName}] 未能识别动作[{field.Action}]";
                            _isok = false;
                        }
                    }

                    _tabchange = true;//说明物理表结构有变更
                    #endregion
                }
                else
                {
                    #region 配置信息变更

                    if (hicol != null)
                    {
                        lstchg.Add(hicol);
                    }



                    #endregion
                }
            }

            if (changes.Count() > 0)
            {
                int resultCnt = 0;
                _sql = sb_sql.ToString();
                if (_isok)
                {
                    if (opLevel == OpLevel.Execute)
                    {
                        _sqlClient.BeginTran();
                        try
                        {
                            if (_tabchange)
                            {
                                _sql = idm.BuildSqlCodeBlock(_sql);

                                resultCnt = _sqlClient.Context.DBO.ExecCommand(_sql, null);
                            }

                            int del_count = 0;
                            int modi_count = 0;
                            if (lstdel.Count > 0)
                            {
                               del_count = _sqlClient.Delete(Constants.HiSysTable["Hi_FieldModel"].ToString(), lstdel).ExecCommand();
                            }

                            if (lstchg.Count > 0)
                            {
                                modi_count = _sqlClient.Modi(Constants.HiSysTable["Hi_FieldModel"].ToString(), lstchg).ExecCommand();
                            }
                            _sqlClient.CommitTran();
                            _isok = true;
                            _msg = $"保存表[{tabInfo.TabModel.TabName}]的结构成功";
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
                        _msg = $"保存表[{tabInfo.TabModel.TabName}]的结构检测成功";
                    }
                }

            }
            else
            {
                _isok = false;
                _msg = "表结构无变更";
            }

            //刷新表结构缓存
            HiSqlCommProvider.RemoveTabInfoCache(tabInfo.TabModel.TabName);
            TabInfo _temptabinfo = idm.GetTabStruct(tabInfo.TabModel.TabName);

            return new Tuple<bool, string, string>(_isok, _msg, _sql);
        }

        /// <summary>
        /// 无日志删除表数据
        /// </summary>
        /// <param name="tabname"></param>
        /// <returns></returns>
        public bool Truncate(string tabname)
        {
            if (_sqlClient != null)
            {
                _sqlClient.TrunCate(tabname).ExecCommand();
                return true;
            }
            else
                throw new Exception($"请先指定数据库连接!");
        }

        /// <summary>
        /// 对表重命名
        /// </summary>
        /// <param name="tabname"></param>
        /// <param name="newtabname"></param>
        /// <param name="opLevel"></param>
        /// <returns></returns>
        public Tuple<bool, string, string> ReTable(string tabname, string newtabname, OpLevel opLevel)
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

            DataTable dt = idm.GetAllTables(newtabname);
            if (dt.Rows.Count == 0)
            {
                _sql = idm.BuildReTableStatement(tabname, newtabname);
                TabInfo _tabinfo = ClassExtensions.DeepCopy<TabInfo>(tabinfo);
                _tabinfo.TabModel.TabName = newtabname;
                _tabinfo.TabModel.TabReName = newtabname;

                foreach (HiColumn column in _tabinfo.Columns)
                {
                    column.TabName = _tabinfo.TabModel.TabName;
                }

                if (opLevel == OpLevel.Execute)
                {
                    //移除表缓存结构
                    HiSqlCommProvider.RemoveTabInfoCache(tabname);
                    _sqlClient.BeginTran();
                    try
                    {

                        _sqlClient.Context.DBO.ExecCommand(_sql, null);

                        _sqlClient.Delete("Hi_TabModel").Where($"TabName='{tabname.ToSqlInject()}'").ExecCommand();
                        _sqlClient.Delete("Hi_FieldModel").Where($"TabName='{tabname.ToSqlInject()}'").ExecCommand();

                        _sqlClient.Insert("Hi_TabModel", _tabinfo.TabModel)
                                  .Insert("Hi_FieldModel", _tabinfo.Columns).ExecCommand();

                        _sqlClient.CommitTran();
                        _isok = true;
                        _msg = $"表[{tabname}]重命名为[{newtabname}]成功";
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
                    StringBuilder sb_sql = new StringBuilder();
                    sb_sql.AppendLine(_sql);

                    sb_sql.AppendLine(_sqlClient.Delete("Hi_TabModel").Where($"TabName='{tabname.ToSqlInject()}'").ToSql());

                    sb_sql.AppendLine(_sqlClient.Delete("Hi_FieldModel").Where($"TabName='{tabname.ToSqlInject()}'").ToSql());

                    sb_sql.AppendLine(_sqlClient.Insert("Hi_TabModel", _tabinfo.TabModel)
                                  .Insert("Hi_FieldModel", _tabinfo.Columns).ToSql());
                    _sql = sb_sql.ToString();
                    _isok = true;
                    _msg = $"表[{tabname}]重命名为[{newtabname}]检测成功";
                }
            }
            else
            {
                _isok = false;
                _msg = $"表[{tabname}]重命名为[{newtabname}] 已经被占用";
            }
            return new Tuple<bool, string, string>(_isok, _msg, _sql);
        }
    }
}
