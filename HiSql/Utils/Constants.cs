using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{

    /// <summary>
    /// author:tansar
    /// mail:tansar@126.com
    /// </summary>
    public  static class Constants
    {
        public static Type IntType = typeof(int);
        public static Type LongType = typeof(Int64);
        public static Type GuidType = typeof(Guid);
        public static Type BoolType = typeof(bool);
        public static Type BoolTypeNull = typeof(bool?);
        public static Type ByteType = typeof(Byte);
        public static Type ObjType = typeof(object);
        public static Type DobType = typeof(double);
        public static Type FloatType = typeof(float);
        public static Type ShortType = typeof(Int16);
        public static Type DecType = typeof(decimal);
        public static Type StringType = typeof(string);
        public static Type DateType = typeof(DateTime);
        public static Type DateTimeOffsetType = typeof(DateTimeOffset);
        public static Type TimeSpanType = typeof(TimeSpan);
        public static Type ByteArrayType = typeof(byte[]);


        public static Type DynamicType = typeof(ExpandoObject);
        public static Type Dicii = typeof(KeyValuePair<int, int>);
        public static Type DicIS = typeof(KeyValuePair<int, string>);
        public static Type DicSi = typeof(KeyValuePair<string, int>);
        public static Type DicSS = typeof(KeyValuePair<string, string>);
        public static Type DicOO = typeof(KeyValuePair<object, object>);
        public static Type DicSo = typeof(KeyValuePair<string, object>);
        public static Type DicArraySS = typeof(Dictionary<string, string>);
        public static Type DicArraySO = typeof(Dictionary<string, object>);


        public static Type[] NumberType = new Type[] { IntType, LongType,DobType,FloatType,ShortType,DecType };

        public static string KEY_PRE = "HiSql";//KEY前辍
        public static string KEY_SEPRATE = ":";//key分隔符
        public static string KEY_ENTITY_NAME = $"{KEY_PRE}{KEY_SEPRATE}Entities{KEY_SEPRATE}[$NAME$]";//实体对象缓存KEY

        public static string HiSqlSyntaxError = $"HiSql语法检测错误:";

        /// <summary>
        /// 程序集名称
        /// </summary>
        
        public static string NameSpace = "HiSql";

        /// <summary>
        /// 用于数据库中 函数产生的日期
        /// </summary>
        public static string FunDate = "FUNDATE";

        /// <summary>
        /// 用于数据库中产生 全局唯一的GUID（md5）值
        /// </summary>
        public static string FunGuid = "FUNGUID";


        private static List<string> _dbCurrentSpport ;
        private static List<string> _dbSpport;


        /// <summary>
        /// 判断是否是标准字段
        /// </summary>
        /// <param name="columname"></param>
        /// <returns></returns>
        public static bool IsStandardField(string columname)
        {
            if (columname.ToLower().IsIn<string>("createtime", "createname", "moditime", "modiname"))
            {
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// 判断是否是标准日期字段(创建或修改）
        /// </summary>
        /// <param name="columname"></param>
        /// <returns></returns>
        public static bool IsStandardTimeField(string columname)
        {
            if (columname.ToLower().IsIn<string>("createtime",  "moditime"))
            {
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// 判断是否是标准用户字段(创建或修改)
        /// </summary>
        /// <param name="columname"></param>
        /// <returns></returns>
        public static bool IsStandardUserField(string columname)
        {
            if (columname.ToLower().IsIn<string>("createname", "modiname"))
            {
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// 是否创建字段
        /// </summary>
        /// <param name="columname"></param>
        /// <returns></returns>
        public static bool IsStandardCreateField(string columname)
        {
            if (columname.ToLower().IsIn<string>("createname", "createtime"))
            {
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// 是否修改字段
        /// </summary>
        /// <param name="columname"></param>
        /// <returns></returns>
        public static bool IsStandardModiField(string columname)
        {
            if (columname.ToLower().IsIn<string>("moditime", "modiname"))
            {
                return true;
            }
            else
                return false;
        }


        /// <summary>
        /// 获取当前支持的数据库
        /// </summary>
        public static List<string> DbCurrentSupportList
        {
            get {
                if (_dbCurrentSpport == null)
                {
                    _dbCurrentSpport = new List<string>();
                    bool _isend = false;
                    int _idx = 1;
                    while (!_isend)
                    {
                        DBType dbtype = (DBType)_idx;

                        string _name = dbtype.ToString();
                        if (_name.IsInt())
                        {
                            _isend = true;
                            break;
                        }
                        else
                        {
                            Assembly asem=Instance.GetAssembly(_name);
                            if (asem != null)
                                _dbCurrentSpport.Add(_name);
                        }
                        _idx++;
                    }
                }
                return _dbCurrentSpport;
            }
        }

        /// <summary>
        /// 获取系统可以支持的数据库
        /// </summary>
        public static List<string> DbSupportList
        {
            get
            {
                if (_dbSpport == null)
                {
                    _dbSpport = new List<string>();
                    bool _isend = false;
                    int _idx = 1;
                    while (!_isend)
                    {
                        DBType dbtype = (DBType)_idx;

                        string _name = dbtype.ToString();
                        if (_name.IsInt())
                        {
                            _isend = true;
                            break;
                        }
                        else
                        {

                            _dbSpport.Add(_name);
                        }
                        _idx++;
                    }
                }
                return _dbSpport;
            }
        }


        //public static DBTYPE DataBaseType = new DBTYPE("");
        /// <summary>
        /// HiSql系统标准表  key,表名
        /// </summary>
        public static Dictionary<string, string> HiSysTable = new Dictionary<string, string> {
            { "Hi_Domain","Hi_Domain"},
            { "Hi_DataElement","Hi_DataElement"},
            { "Hi_TabModel","Hi_TabModel"},
            { "Hi_FieldModel","Hi_FieldModel"}

        };


        /// <summary>
        /// 标准数据库类型与c#类型的映射关系
        /// </summary>
        public static Dictionary<HiType, Type> HiTypeMapping = new Dictionary<HiType, Type> {
            {HiType.NVARCHAR,StringType},
            {HiType.VARCHAR,StringType},
            {HiType.NCHAR,StringType},
            {HiType.CHAR,StringType},
            {HiType.TEXT,StringType},

            {HiType.INT,IntType},
            {HiType.BIGINT,LongType},
            {HiType.SMALLINT,ShortType},
            {HiType.DECIMAL,DecType},

            {HiType.BOOL,BoolType},

            {HiType.DATETIME,DateType},
            {HiType.DATE,DateType},

            {HiType.BINARY,ByteArrayType},

            {HiType.GUID,GuidType}
        };

        

        /// <summary>
        /// 表结构缓存路径 (该key值支持redis的分块）
        /// 如:HiSql:Table:HoneORM:dbo:TableModel
        /// </summary>
        public static string KEY_TABLE_CACHE_NAME = $"{KEY_PRE}{KEY_SEPRATE}Table{KEY_SEPRATE}[$TABLE$]";




        /// <summary>
        /// 解析 表字段及重命名 语法规则 慎重修改 改错了会影响整体SQL编译
        /// *
        //table.*
        //# table.*
        //fieldname
        //tabname.field as name
        //#tabname.field as fieldname
        //tabname.field
        //#tabname.field
        /// </summary>
        //public static string REG_FIELDANDRENAME = @"^(?:[\s]*)(?:(?<tab>[\#]?[\w]+)(?:[\.]{1}))?(?<field>[\w]+)\s*(?:(?i)as\s*(?<refield>[\w]+))?\s*$|(?:[\s]*)(?:(?<tab>[\#]?[\w]+)(?:[\.]{1}))(?<field>[\*]{1})\s*$|^(?:[\s]*)(?<field>[\*]{1})\s*$";
        public static string REG_FIELDANDRENAME = @"^(?:[\s]*)(?:(?<flag>[\#]{1,2}|[\@]{1})?(?<tab>[\w]+)(?:[\.]{1}))?(?<field>[\w]+)\s*(?:(?i)as\s*(?<refield>[\w]+))?\s*$|(?:[\s]*)(?:(?<tab>[\#]?[\w]+)(?:[\.]{1}))(?<field>[\*]{1})\s*$|^(?:[\s]*)(?<field>[\*]{1})\s*$";


        /// <summary>
        /// 字段名 不带 as 重命名
        /// </summary>
        public static string REG_FIELDNOASNAME = @"^(?:[\s]*)(?<left>(?:(?<flag>[\#]{1,2}|[\@]{1})?(?<tab>[\w]+)(?:[\.]{1}))?(?<field>[\w]+))\s*";

        
        /// <summary>
        /// 解析表或字段的名称
        /// </summary>
        public static string REG_NAME = @"^(?:[\s]*)(?<name>[\w]+)\s*$";


        /// <summary>
        /// 解析表的名称
        /// </summary>
        public static string REG_TABNAME = @"^(?:[\s]*)(?<flag>[\#]{1,2}|[\@]{1})?(?<tab>[\w]+)\s*$";

        /// <summary>
        /// 解析字段名称
        /// #A.TabName
        //A.TabName
        //TabName
        /// </summary>
        //public static string REG_FIELDNAME = @"^(?:[\s]*)(?:(?<tab>[\#]?[\w]+)(?:[\.]{1}))?(?<field>[\w]+)\s*$";
        public static string REG_FIELDNAME = @"^(?:[\s]*)(?:(?<flag>[\#]{1,2}|[\@]{1})?(?<tab>[\w]+)(?:[\.]{1}))?(?<field>[\w]+)\s*$";


        /// <summary>
        /// 字段表达式
        /// a.username <>  120.4
        /// a.username <>  'tgm'
        /// 23
        /// '123'
        /// </summary>
        //public static string REG_FIELDEXPRESSION = @"^(?:[\s]*)(?:(?<flag>[\#]{1,2}|[\@]{1})?(?<tab>[\w]+)(?:[\.]{1}))?(?<field>[\w]+)\s*(?<oper>\<\>|[\>\<=]{1}|\!=)\s*(?:(?<value>[-]?\d+(?:[\.]?)[\d]*)|[\']{1}(?<value>[\w\W0-9\s]*)[\']{1})";
        public static string REG_FIELDEXPRESSION = @"^(?:(?:[\s]*)(?:(?<flag>[\#]{1,2}|[\@]{1})?(?<tab>[\w]+)(?:[\.]{1}))?(?<field>[\w]+)\s*(?<oper>\<\>|[\>\<=]{1}|\!=|\>=|\<=))?\s*(?:(?<value>[-]?\d+(?:[\.]?)[\d]*)|[\']{1}(?<value>[\w\W0-9\s]*)[\']{1})";



        public static string REG_SORT = @"^(?:[\s]*)(?<field>(?:(?:[\#]{1,2}|[\@]{1})?(?:[\w]+)(?:[\.]{1}))?(?:[\w]+))?\s*(?<sort>asc|desc)?\s*$";



        /// <summary>
        /// 表过式字段更新
        /// </summary>
        public static string REG_UPDATE = @"[`](?:[\s]*)(?:(?<flag>[\#]{1,2}|[\@]{1})?(?<tab>[\w]+)(?:[\.]{1}))?(?<field>[\w]+)[`]";

        /// <summary>
        /// 解析表字段关联关系
        /// #a.dddd     =    #add.ad
        //UserName.Id    =    ulist.ddd
        /// </summary>
        //public static string REG_JOINON = @"^(?<left>(?:[\s]*)(?:(?<tab>[\#]?[\w]+)(?:[\.]{1}))?(?<field>[\w]+)\s*)=(?<right>(?:[\s]*)(?:(?<rtab>[\#]?[\w]+)(?:[\.]{1}))?(?<rfield>[\w]+)\s*)$";
        public static string REG_JOINON = @"^(?<left>(?:[\s]*)(?:(?<flag>[\#]{1,2}|[\@]{1})?(?<tab>[\w]+)(?:[\.]{1}))?(?<field>[\w]+)\s*)=(?<right>(?:[\s]*)(?:(?<rflag>[\#]{1,2}|[\@]{1})?(?<rtab>[\w]+)(?:[\.]{1}))?(?<rfield>[\w]+)\s*)";


        /// <summary>
        /// 解析SQL函数处理
        /// 支持max,min,avg,sum,count五种最常用的函数
        /// count(*) as avgFieldLen   
        /// avg(a.fieldlen) as avgFieldLen   
        /// </summary>
        //public static string REG_FUNCTION = @"^(?:[\s]*)(?<fun>max|min|avg|sum|count)\s*[\(]\s*(?:(?<tab>[\#]?[\w]+)(?:[\.]{1}))?(?<field>[\w]+|[\*]{1})\s*[\)]\s*(?:as)\s*(?<refield>[\w]+)\s*$";
        public static string REG_FUNCTION = @"^(?:[\s]*)(?<fun>max|min|avg|sum|count)\s*[\(]\s*(?:(?<flag>[\#]{1,2}|[\@]{1})?(?<tab>[\w]+)(?:[\.]{1}))?(?<field>[\w]+|[\*]{1})\s*[\)]\s*(?:as)\s*(?<refield>[\w]+)\s*$";

        /// <summary>
        /// 解析聚合函数 且无重命名 在having中专用
        /// </summary>
        public static string REG_FUNCTIONNORENAME = @"^(?:[\s]*)(?<left>(?<fun>max|min|avg|sum|count)\s*[\(]\s*(?:(?<flag>[\#]{1,2}|[\@]{1})?(?<tab>[\w]+)(?:[\.]{1}))?(?<field>[\w]+|[\*]{1})\s*[\)])\s*";


        public static string REG_SERVER = "";


        /// <summary>
        /// 是否是模糊查询值
        /// </summary>
        public static string REG_ISLIKEQUERY = @"[\%]+";



        /// <summary>
        /// 匹配一个左括号
        /// </summary>
        public static string REG_BRACKET_LEFT = @"^\s*\(\s*$";


        /// <summary>
        /// 匹配一个右括号
        /// </summary>
        public static string REG_BRACKET_RIGHT = @"^\s*\)\s*$";
    }
}
