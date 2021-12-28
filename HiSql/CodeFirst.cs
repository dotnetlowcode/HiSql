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
    }
}
