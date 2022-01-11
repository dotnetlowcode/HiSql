using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HiSql
{

    /// <summary>
    /// 表的索引
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class HiIndex : Attribute
    {

    }
    /// <summary>
    /// 用于描述表结构属性信息(可以根据这些信息动态创建表)
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    [Serializable]
    public class HiTable : Attribute
    {
        [JsonIgnore]
        public override object TypeId { get; }


        string _tabName = "";
        string _tabReName = "";
        string _dbServer = "";
        string _schema = "";

        string _tabdescript = "";

        string _logtable = "";
        int _logexprireday = 0;


        TabStoreType _tabStoreType = TabStoreType.Row;//默认行式存储
        TabType _tabType = TabType.Business;
        TabCacheType _tabCacheType = TabCacheType.None;
        TabStatus _tabStatus = TabStatus.NoActive;
        TableType tableType = TableType.Entity;


        bool _isSystem = false;
        bool _isEdit = false;
        bool _isLog = false;




        public TableType TableType
        {
            get { return tableType; }
        }

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
        /// <summary>
        /// 表的存储类型
        /// </summary>
        public TabStoreType TabStoreType
        {
            get { return _tabStoreType; }

            set { _tabStoreType = value; }
        }

        /// <summary>
        /// 表类型
        /// </summary>
        public TabType TabType
        {
            get { return _tabType; }
            set { _tabType = value; }
        }

        /// <summary>
        /// 表的缓存类型
        /// </summary>
        public TabCacheType TabCacheType { get { return _tabCacheType; } set { _tabCacheType = value; } }

        /// <summary>
        /// 表状态
        /// </summary>
        public TabStatus TabStatus { get { return _tabStatus; } set { _tabStatus = value; } }


        /// <summary>
        /// 是否系统内置表
        /// </summary>
        public bool IsSys { get { return _isSystem; } set { _isSystem = value; } }


        /// <summary>
        /// 是否可编辑的表
        /// </summary>
        public bool IsEdit { get { return _isEdit; } set { _isEdit = value; } }


        /// <summary>
        /// 是否开启日志
        /// </summary>
        public bool IsLog { get { return _isLog; } set { _isLog = value; } }


        /// <summary>
        /// 日志表
        /// </summary>
        public string LogTable { get { return _logtable; } set { _logtable = value; } }

        public int LogExprireDay { get { return _logexprireday; } set { _logexprireday = value; } }
        public HiTable()
        {

        }
    }

    /// <summary>
    /// 字段列描述信息(根据这个信息可以动态创建表的列)
    /// </summary>

    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    [Serializable]
    public class HiColumn : Attribute
    {
        private string _ColumnName= "";
        string _tabName = "";
        private string _fieldDesc = "";
        bool _isPrimary = false;
        bool _isIdentity = false;
        bool _isBllKey = false;
        bool _isIgnore = false;
        bool _isSys = false;
        bool _isNull = true;//默认允许NULL 如果是主键则需要默认false


        bool _custombllkey = false;

        bool _isrequire = false;
        bool _isobsolete = false;

        int _fieldLen = 0;
        int _fieldDec = 0;
        string _sno = "";
        string _sno_num = "";
        string _regex = "";

        string _default = "";//设置默认值

        bool _isshow = true;
        bool _issearch = true;
        SrchMode _srchmode = SrchMode.Single;
        bool _isreftab = false;
        string _reftab = "";
        string _reffield = "";
        string _reffields = "";
        string _reffielddesc = "";
        string _refwhere = "";
        HiType _fieldtype = HiType.NONE;//默认为未设置
        HiTypeDBDefault _dbdefalut = HiTypeDBDefault.NONE;

        int _sortnum = 0;


        [JsonIgnore]
        public override object TypeId { get; }
        
        /// <summary>
        /// 表名
        /// </summary>
        public string TabName
        {
            get { return _tabName; }
            set { _tabName = value; }
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public string FieldName
        {
            get { return _ColumnName; }
            set { _ColumnName = value; }
        }

        /// <summary>
        /// 字段类型
        /// </summary>
        public HiType FieldType
        {
            get { return _fieldtype; }
            set { _fieldtype = value; }
        }

        /// <summary>
        /// 设置数据库默认值
        /// </summary>
        public HiTypeDBDefault DBDefault
        {

            get { return _dbdefalut; }
            set { _dbdefalut = value; }
        }

        /// <summary>
        /// 字段排序号
        /// </summary>
        public int SortNum
        {
            get { return _sortnum; }
            set { _sortnum = value; }
        }
        /// <summary>
        /// 字段描述
        /// </summary>
        public string FieldDesc
        {
            get
            {

                if (_fieldDesc == string.Empty)
                {
                    return _ColumnName;
                }
                return _fieldDesc;

            }
            set { _fieldDesc = value; }
        }

        /// <summary>
        /// 字段长度
        /// </summary>
        public int FieldLen
        {
            get { return _fieldLen; }
            set { _fieldLen = value; }
        }
        /// <summary>
        /// 字段小数点长度
        /// </summary>
        public int FieldDec
        {
            get { return _fieldDec; }
            set { _fieldDec = value; }
        }

        /// <summary>
        /// 编号
        /// </summary>
        public string SNO
        {
            get { return _sno; }
            set { _sno = value; }
        }
        /// <summary>
        /// 子编号
        /// </summary>
        public string SNO_NUM
        {
            get { return _sno_num; }
            set { _sno_num = value; }
        }

        /// <summary>
        /// 正则表达式规则
        /// </summary>
        public string Regex
        {
            get { return _regex; }
            set { _regex = value; }
        }

        /// <summary>
        /// 是否主键
        /// </summary>
        public bool IsPrimary
        {
            get { return _isPrimary; }

            //如果没有指定Bllkey 那么是主键的情况下就默认为BLLkey 如果指定了就按指定的
            set { _isPrimary = value; if (!_custombllkey && _isPrimary) { _isBllKey = _isPrimary; } }
        }
        /// <summary>
        /// 是否自增长
        /// </summary>
        public bool IsIdentity
        {
            get { return _isIdentity; }
            set { _isIdentity = value; }
        }

        /// <summary>
        /// 是否允许null值
        /// </summary>
        public bool IsNull
        {
            get { return _isNull; }
            set { _isNull = value; }
        }

        /// <summary>
        /// 是否业务Key
        /// </summary>
        public bool IsBllKey
        {
            get { return _isBllKey; }
            set { _isBllKey = value; _custombllkey = true; }
        }
        /// <summary>
        /// 是否忽略
        /// </summary>
        public bool IsIgnore
        {
            get { return _isIgnore; }
            set { _isIgnore = value; }
        }
        public bool IsRequire
        {
            get
            {
                if (_isPrimary && !IsIdentity)
                    return _isPrimary;
                else
                    return _isrequire;
            }
            set { _isrequire = value; }
        }

        public bool IsObsolete
        {
            get { return _isobsolete; }
            set
            {
                _isobsolete = value;
            }
        }
        /// <summary>
        /// 是否系统字段
        /// </summary>
        public bool IsSys
        {
            get { return _isSys; }
            set { _isSys = value; }
        }

        public string DefaultValue
        {
            get { return _default; }
            set { _default = value; }
        }




        //扩展 字段

        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsShow
        {
            get { return _isshow; }
            set { _isshow = value; }
        }

        /// <summary>
        /// 是否允许搜索
        /// </summary>
        public bool IsSearch
        {
            get { return _issearch; }
            set { _issearch = value; }
        }

        /// <summary>
        /// 搜索模式
        /// </summary>
        public SrchMode SrchMode
        {
            get { return _srchmode; }
            set { _srchmode = value; }
        }

        /// <summary>
        /// 是否引用表
        /// </summary>
        public bool IsRefTab
        {
            get { return _isreftab; }
            set { _isreftab = value; }

        }

        /// <summary>
        /// 引用表名
        /// </summary>
        public string RefTab
        {
            get { return _reftab; }
            set { _reftab = value; }
        }

        /// <summary>
        /// 引用的字段
        /// </summary>
        public string RefField
        {
            get { return _reffield; }
            set { _reffield = value; }
        }

        /// <summary>
        /// 引用字段清单
        /// </summary>
        public string RefFields
        {
            get { return _reffields; }
            set { _reffields = value; }
        }
        /// <summary>
        /// 引用字段清单 描述
        /// </summary>
        public string RefFieldDesc
        {
            get { return _reffielddesc; }
            set { _reffielddesc = value; }
        }
        /// <summary>
        /// 引用条件
        /// </summary>
        public string RefWhere
        {
            get { return _refwhere; }
            set { _refwhere = value; }
        }

        public bool IsStandardField()
        {

            return Constants.IsStandardField(this.FieldName);
            //if (this.FieldName.ToLower().IsIn<string>("createtime", "createname", "moditime", "modiname"))
            //{
            //    return true;
            //}
            //else
            //    return false;
        }

        public bool IsCreateField()
        {
            if (this.FieldName.ToLower().IsIn<string>("createtime", "createname" ))
            {
                return true;
            }
            else
                return false;
        }
        public bool IsModiField()
        {
            if (this.FieldName.ToLower().IsIn<string>("moditime", "modiname"))
            {
                return true;
            }
            else
                return false;
        }
        public bool IsStandardTime()
        {
            if (this.FieldName.ToLower().IsIn<string>("moditime", "createtime"))
            {
                return true;
            }
            else
                return false;
        }
    }
}
