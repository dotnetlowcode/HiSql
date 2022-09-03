using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// 表查询定义
    /// </summary>
    public class TableDefinition
    {
        private string _schema="";

        private string _tabname="";

        private string _rename="";

        private string _dbserver = "";



        private TableType tableType = TableType.Entity;


        public TableDefinition()
        { 
            
        }
        public TableDefinition(string tabname)
        {
            Dictionary<string, string> _dic = Tool.RegexGrp(Constants.REG_TABNAME, tabname);
            TabName = tabname;
            switch (tableType)
            {
                case TableType.Local:
                    _rename = $"lcl_{_dic["tab"].ToString()}";//本地临时表
                    break;
                case TableType.Global:
                    _rename = $"glo_{_dic["tab"].ToString()}";//全局临时表
                    break;
                case TableType.Var:
                    _rename = $"var_{_dic["tab"].ToString()}";//变量表
                    break;
                default:
                    _rename = tabname;
                    break;

            }
        }
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
            get { return _tabname; }
            set { 
                _tabname = value;
                if (!string.IsNullOrEmpty(_tabname))
                {
                    Dictionary<string, string> _dic = Tool.RegexGrp(Constants.REG_TABNAME, _tabname);
                    switch (_dic["flag"].ToString())
                    {
                        case "#":
                            tableType = TableType.Local;
                            break;
                        case "##":
                            tableType = TableType.Global;
                            break;
                        case "@":
                            tableType = TableType.Var;
                            break;
                    }
                }
            }
        }
        /// <summary>
        /// 数据库所在的服务器
        /// </summary>
        public string DbServer
        {
            get { return _dbserver; }
            set { _dbserver = value; }
        }
        /// <summary>
        /// 表别名
        /// </summary>
        public string AsTabName
        {
            get {
                if (string.IsNullOrEmpty(_rename))
                    return _tabname;
                else
                    return _rename.ToLower();
            
            }
            set { _rename = value; }
        }
        public TableType TableType
        {
            get { return tableType; }
        }

        
    }
}
