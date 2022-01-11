using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// 用于HiSql初始安装时使用
    /// author:tansar mail:tansar@126.com
    /// </summary>
    public class CodeFirst : ICodeFirst
    {
        public HiSqlClient SqlClient
        {
            get {
                return _sqlClient;
            }
            set { _sqlClient = value; }
        }

        private HiSqlClient _sqlClient;
        public CodeFirst(HiSqlClient sqlClient)
        {
            this._sqlClient = sqlClient;
        }
        public CodeFirst()
        { 
            
        }
        /// <summary>
        /// 暂不支持该功能
        /// </summary>
        public void CreateInitDataBase()
        {
            if (_sqlClient != null)
            {
                throw new Exception($"暂未支持该功能");
            }
            else
                throw new Exception($"请先指定数据库连接!");
        }

        /// <summary>
        /// 初次使用HiSql时请执行该方法
        /// 执行一次后不需要再执行
        /// </summary>
        public async Task InstallHisql()
        {
            if (_sqlClient != null)
            {
                IDbConfig dbConfig = Instance.CreateInstance<IDbConfig>($"{Constants.NameSpace}.{_sqlClient.CurrentConnectionConfig.DbType.ToString()}{DbInterFace.Config.ToString()}");

                string _sql = dbConfig.InitSql;
                _sql = _sql.Replace("[$Schema$]", _sqlClient.CurrentConnectionConfig.Schema);

                //返回受影响的行
                int _effect= _sqlClient.Context.DBO.ExecCommand(_sql);
            }
            else
                throw new Exception($"请先指定数据库连接!");
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
                return _sqlClient.Context.DMInitalize.BuildTabCreate(tabInfo) >0;

                
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
        /// 修改表
        /// </summary>
        /// <param name="tabInfo"></param>
        /// <returns></returns>
        public bool ModiTable(TabInfo tabInfo)
        {
            if (_sqlClient != null)
            {
                throw new Exception($"暂未支持该功能");
            }
            else
                throw new Exception($"请先指定数据库连接!");
        }

        /// <summary>
        /// 删除表
        /// </summary>
        /// <param name="tabname"></param>
        /// <returns></returns>
        public bool DropTable(string tabname,bool nolog=false)
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
                    return  _sqlClient.Drop(tabname).ExecCommand()>0;
            }
            else
                throw new Exception($"请先指定数据库连接!");
        }

        /// <summary>
        /// 清空表中数据
        /// 高风险操作
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
    }
}
