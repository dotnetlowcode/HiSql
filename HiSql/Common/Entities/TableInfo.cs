using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{

    /// <summary>
    /// 用于返回前端的描述表的结构信息
    /// </summary>
    public class TableInfo
    {
        string _tabdescript = "";
        string _tabName = "";
        string _tabReName = "";
        string _dbServer = "";
        string _schema = "";

        /// <summary>
        /// 是否有表结构信息
        /// </summary>
        bool _hasTabInfo = false;

        TableType tableType = TableType.Entity;

        /// <summary>
        /// 该表存储服务器
        /// </summary>
        public string DbServer
        {
            get
            {
                return _dbServer;
            }
            set { _dbServer = value; }
        }
        /// <summary>
        /// 数据库表的schema
        /// </summary>
        public string Schema
        {
            get { return _schema; }
            set { _schema = value; }
        }

        /// <summary>
        /// 表名
        /// </summary>
        public string TabName
        {
            get
            {
                return _tabName;
            }
            set
            {
                _tabName = value;

                Dictionary<string, string> _dic = Tool.RegexGrp(Constants.REG_TABNAME, _tabName);
                if (_dic.Count > 0)
                {
                    switch (_dic["flag"].ToString())
                    {
                        case "#":
                            tableType = TableType.Local;
                            //_tabReName = _dic["tab"].ToString();
                            break;
                        case "##":
                            tableType = TableType.Global;
                            //_tabReName = $"_tmp_global_{_dic["tab"].ToString()}_{Thread.CurrentThread.ManagedThreadId.ToString() }_{_tabName.GetHashCode().ToString().Substring(1)}".ToUpper();
                            break;
                        case "@":
                            tableType = TableType.Var;
                            break;
                    }
                }
                else {
                    tableType = TableType.Var;
                }
                


            }
        }

        /// <summary>
        /// 表的别名
        /// </summary>
        public string TabReName
        {
            get
            {
                if (string.IsNullOrEmpty(_tabReName))
                    return _tabName;
                else
                    return _tabReName;
            }
            set { _tabReName = value; }
        }

        /// <summary>
        /// 表描述
        /// </summary>
        public string TabDescript
        {
            get { return _tabdescript; }
            set { _tabdescript = value; }
        }

        public TableType TableType
        {
            get { return tableType; }
            set { tableType = value; }
        }

        /// <summary>
        /// 是否有表结构信息
        /// </summary>
        public bool HasTabStruct
        {
            get { return _hasTabInfo; }
            set { _hasTabInfo = value; }
        }
    }
}
