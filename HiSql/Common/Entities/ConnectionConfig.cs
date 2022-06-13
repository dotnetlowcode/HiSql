using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// 数据库连接配置
    /// </summary>
    public class ConnectionConfig
    {

        /// <summary>
        /// 
        /// </summary>
        private bool isShareThread = false;

        private bool isAutoClose = false;

        private string configId = string.Empty;

        private string configUid = string.Empty;

        private string _schema="";

        private string _dbserver = "";

        private int _timeout = 3000;

        private bool _ignorecase=true;

        private bool _uppercase = false;
        /// <summary>
        /// 是否自主指定大小写
        /// </summary>
        private bool _iscustomupper = false;
        private bool _isLog = false;

        private string _user = "Hone";

        /// <summary>
        /// 仅包含的表走从库 如果排除表也配置了那么优先包含表
        /// </summary>
        private List<string> _slaveonly = null;
        /// <summary>
        /// 除此之外的表都走从库 如果包含的表也配置了那么优先包含表
        /// </summary>
        private List<string> _slaveexclude = null;

        //当前是不是从库 默认
        private bool _iscurrSlave = false;

        /// <summary>
        /// 连接Key 用作标识唯一标识(由系统自动生民)
        /// </summary>
        public string ConfigId { get { return configId; }set { configId = value; } }


        /// <summary>
        /// 用户自定义的连接ID，用于用户自己标识该连接
        /// </summary>
        public string ConfigUid { get { return configUid; }set { configUid = value; } }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public DBType DbType { get; set; }

        /// <summary>
        /// 是否启用加密
        /// </summary>
        public bool IsEncrypt = false;

        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        //数据忽略大小写
        public bool IgnoreCase { 
            get {
                
                return _ignorecase; 
            
            }
            set {
                
                _ignorecase = value; 
            } 
        
        }
        /// <summary>
        /// 是否转成大写
        /// </summary>
        public bool UpperCase
        {
            get {

                //这两种库本身是区分大小写的 忽略大小写对转成大写
                if (DbType.IsIn<DBType>(DBType.Hana, DBType.Oracle,DBType.SQLite,DBType.DaMeng) && _ignorecase && !_iscustomupper)
                {
                    _uppercase = true;
                }
                return _uppercase;
            }
            set {
                _iscustomupper = true;
                _uppercase = value; }
        }

        /// <summary>
        /// 数据库所在的服务器 格式 host:databasename  如 local:hisqldb
        /// </summary>
        public string DbServer
        {
            get{ return _dbserver;}
            set { 
                    
                _dbserver = value; 
            
            }
        }

        /// <summary>
        /// Sql执行超时设定(毫秒)
        /// </summary>

        public int SqlExecTimeOut
        {
            get { return _timeout; }
            set { _timeout = value; }
        }

        /// <summary>
        /// 数据库Schema 
        /// </summary>
        public string Schema { get { return _schema; } set { _schema = value; } }
        /// <summary>
        /// 是否共享连接线程
        /// </summary>
        public bool IsShareThread
        {
            get { return isShareThread; }
            set { isShareThread = value; }
        }

        /// <summary>
        /// 是否开始日志
        /// </summary>
        public bool IsLog
        {
            get => _isLog;
            set => _isLog = value;
        }

        /// <summary>
        /// 从库，可以有多个 但必须与主库做高可用
        /// </summary>
        public List<SlaveConnectionConfig> SlaveConnectionConfigs { get; set; }


        /// <summary>
        /// 是否启用了主从库
        /// </summary>
        public bool IsMasterSlave {
            get {
                if (SlaveConnectionConfigs != null)
                {
                    if (SlaveConnectionConfigs.Where(s => s.Weight > 0).Count() > 0)
                    {
                        return true;
                    }
                }

                return false;
            }
        }


        /// <summary>
        /// 当前是否是从库
        /// </summary>
        public bool IsCurrSlave
        {
            get { return _iscurrSlave; }
            set { _iscurrSlave = false; }
        }

        /// <summary>
        /// 仅在此列的表走从库
        /// </summary>
        public List<string> SlaveOnly { get => _slaveonly; set => _slaveonly = value; }

        /// <summary>
        /// 不在此列折表走从库
        /// </summary>

        public List<string> SlaveExclude { get => _slaveexclude; set => _slaveexclude = value; }

        /// <summary>
        /// 是否自动关闭链接
        /// </summary>
        public bool IsAutoClose { get { return isAutoClose; } set { isAutoClose = value; } }

        /// <summary>
        /// 是否调试模式 如果是调试模式则自动开启SQL监测
        /// </summary>
        public bool Deugger { get; set; }


        /// <summary>
        /// 注册AOP事件
        /// </summary>
        public AopEvent AppEvents { get; set; }


        /// <summary>
        /// 用于记录操作记录的当前用户名称(可自定义,默认是Hone)
        /// </summary>
        public string User
        {
            get { return _user; }
            set { _user = value; }
            
        }
    }
}
