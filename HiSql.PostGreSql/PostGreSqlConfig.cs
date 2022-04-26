using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public class PostGreSqlConfig : IDbConfig
    {
        /// <summary>
        /// 数据包大小 经过测试200这个值是比较合理
        /// </summary>
        int _bluksize = 200;
        int _bulkunitsize = 250000;

        int _packagerecord = 3000;
        int _packagecell = 40;
        int _packagecells = 100000;


        string _temp_schema_pre = "\"";
        string _temp_schema_after = "\"";

        string _temp_table_pre = "\"";
        string _temp_table_after = "\"";

        string _temp_field_pre = "\"";
        string _temp_field_after = "\"";



        string _temp_field_split = ",";

        //创建实体表的模版
        string _temp_create_table = "";
        //创建临时表的模版
        string _temp_create_temp_global_table = "";

        string _temp_create_temp_global_table_drop = "";

        string _temp_create_temp_local_table = "";
        string _temp_create_temp_local_table_drop = "";
        //变量表模版
        string _temp_declare_table = "";

        string _temp_tabel_key = "";



        string _temp_table_key2 = "";//如 User ASC
        string _temp_table_key3 = "";

        string _temp_table_key3_temp = "";

        string _temp_field_comment = "";

        /// <summary>
        /// 数据插入语句模版
        /// </summary>
        string _temp_insert_statement = "";
        string _temp_insert_statementv2 = "";


        /// <summary>
        /// 获取表结构信息模版
        /// </summary>
        string _temp_get_table_schema = "";


        //本地临时表前辍
        string _temp_local_temp_pre = "#";

        //全局临时表前辍
        string _temp_global_temp_pre = "##";

        //变量表前辍
        string _temp_var_temp_pre = "@";


        string _temp_merge_into = "";

        string _temp_update = "";
        string _temp_update_where = "";

        string _temp_delete = "";
        string _temp_delete_where = "";


        string _temp_truncate = "";

        string _temp_droptable = "";

        string _temp_fun_date = "";

        /// <summary>
        /// postgresql 特有的
        /// </summary>
        string _temp_sequence = "";

        /// <summary>
        /// postgresql 特有的 临时表的sequence
        /// </summary>
        string _temp_sequence_temp = "";


        string _temp_addcolumn = "";

        string _temp_delcolumn = "";

        string _temp_modicolumn = "";

        string _temp_recolumn = "";

        string _temp_retable = "";

        string _temp_setdefalut = "";

        string _temp_deldefalut = "";


        string _temp_setNotNull = "";

        string _temp_delNotNull = "";

        /// <summary>
        /// 所有物理实体表
        /// </summary>
        string _temp_gettables = "";

        string _temp_getTableDataCount = "select count(*) from [$TabName$] ";
        /// <summary>
        /// 获取所有视图
        /// </summary>
        string _temp_getviews = "";
        string _temp_getviews_paging = "";
        /// <summary>
        /// 获取表和视图
        /// </summary>
        string _temp_getalltables = "";
        string _temp_getalltables_paging = "";

        
        /// <summary>
        /// 分页获取表和视图
        /// </summary>
        string _temp_gettables_paging = "";

        /// <summary>
        /// 检测表或视图是否存在
        /// </summary>
        string _temp_check_table_exists = "";

        /// <summary>
        /// 创建视图
        /// </summary>
        string _temp_create_view = "";


        /// <summary>
        /// 修改视图
        /// </summary>
        string _temp_modi_view = "";


        /// <summary>
        /// 删除视图
        /// </summary>
        string _temp_drop_view = "";


        /// <summary>
        /// 临时表查询
        /// </summary>
        string _temp_globaltempdb_query = "";


        /// <summary>
        /// 获取索引明细
        /// </summary>
        string _temp_get_indexdetail = "";


        /// <summary>
        /// 获取指定表的索引
        /// </summary>
        string _temp_get_tabindex = "";


        /// <summary>
        /// 索引创建模板
        /// </summary>
        string _temp_create_index = "";


        /// <summary>
        /// 删除索引
        /// </summary>
        string _temp_drop_index = "";


        /// <summary>
        /// 字段创建时的模板[$FieldName$]  这是一个可替换的字符串ColumnName是在HiColumn中的属性名
        /// </summary>
        Dictionary<string, string> _fieldtempmapping = new Dictionary<string, string> { };

        Dictionary<string, string> _fieldtempmappingForModiType = new Dictionary<string, string> { };
        Dictionary<string, string> _fieldtempmappingForModiDEFAULT = new Dictionary<string, string> { };
        Dictionary<string, string> _fieldtempmappingForModiCONSTRAINT = new Dictionary<string, string> { };
        Dictionary<HiType, string> _dbmapping = new Dictionary<HiType, string>();

        List<DefMapping> _lstdefmapping = new List<DefMapping>();


        /// <summary>
        /// 安装HiSql初始化
        /// </summary>
        public string InitSql
        {
            get
            { //return _temp_install;

                return HiSql.PostGreSql.Properties.Resources.HiSql.ToString();
            }
        }

        public List<DefMapping> DbDefMapping
        {
            get => _lstdefmapping;
        }
        public int BlukSize { get => _bluksize; set => _bluksize = value; }
        public int BulkUnitSize { get => _bulkunitsize; set => _bulkunitsize = value; }




        /// <summary>
        /// 强制分包记录数大小 结合 强制分包列数量 一起触发
        /// </summary>
        public int PackageRecord { get => _packagerecord; set => _packagerecord = value; }


        /// <summary>
        /// 强制分包列数量 结合 强制分包记录数大小 一起触发
        /// </summary>
        public int PackageCell { get => _packagecell; set => _packagecell = value; }


        /// <summary>
        /// 按分包单元格数
        /// </summary>
        public int PackageCells { get => _packagecells; set => _packagecells = value; }


        public string Schema_Pre { get => _temp_schema_pre; }
        public string Schema_After { get => _temp_schema_after; }
        public string Table_Pre { get => _temp_table_pre; }
        public string Table_After { get => _temp_table_after; }
        public string Field_Pre { get => _temp_field_pre; }
        public string Field_After { get => _temp_field_after; }
        public string Table_Create { get => _temp_create_table; }

        public string Fun_CurrDATE { get => _temp_fun_date; }
        public string Drop_Table { get => _temp_droptable; }
        public string Table_Global_Create { get => _temp_create_temp_global_table; }
        public string Table_Global_Create_Drop { get => _temp_create_temp_global_table_drop; }
        public string Table_Local_Create { get => _temp_create_temp_local_table; }

        public string Table_Local_Create_Drop { get => _temp_create_temp_local_table_drop; }
        public string Table_Declare_Table { get => _temp_declare_table; }
        public string Field_Split { get => _temp_field_split; }
        public string Table_Key { get => _temp_tabel_key; }
        public string Table_Key2 { get => _temp_table_key2; }
        public string Table_Key3 { get => _temp_table_key3; }

        public string Table_Key3_Temp { get => _temp_table_key3_temp; }
        public string Field_Comment { get => _temp_field_comment; }
        public string Get_Table_Schema { get => _temp_get_table_schema; }


        public string Insert_StateMent { get => _temp_insert_statement; }

        public string Insert_StateMentv2 { get => _temp_insert_statementv2; }

        /// <summary>
        /// 表更新模板
        /// </summary>
        public string Update_Statement { get => _temp_update; }


        public string Update_Statement_Where { get => _temp_update_where; }

        public string Delete_Statement { get => _temp_delete; }

        public string Delete_Statement_Where { get => _temp_delete_where; }

        public string Delete_TrunCate { get => _temp_truncate; }

        public Dictionary<string, string> FieldTempMapping => _fieldtempmapping;

        public Dictionary<string, string> FieldTempMappingForModi => _fieldtempmappingForModiType;
        public Dictionary<HiType, string> DbMapping => _dbmapping;

        public string Table_MergeInto { get => _temp_merge_into; }


        public string Sequence { get => _temp_sequence; }

        public string Sequence_Temp { get => _temp_sequence_temp; }





        /// <summary>
        /// 新添加列的模板
        /// </summary>
        public string Add_Column { get => _temp_addcolumn; }


        /// <summary>
        /// 删除列的模版
        /// </summary>
        public string Del_Column { get => _temp_delcolumn; }


        /// <summary>
        /// 修改列的模板
        /// </summary>
        public string Modi_Column { get => _temp_modicolumn; }


        public string Set_Default { get => _temp_setdefalut; }

        public string Del_Default { get => _temp_deldefalut; }

        public string Set_NotNull { get => _temp_setNotNull; }

        public string Del_NotNull { get => _temp_delNotNull; }



        public string Get_Tables { get => _temp_gettables; }

        public string Get_Views { get => _temp_getviews; }
        public string Get_ViewsPaging { get => _temp_getviews_paging; }
        
        public string Get_AllTables { get => _temp_getalltables; }
        public string Get_AllTablesPaging { get => _temp_getalltables_paging; }
        
        public string Get_TablesPaging { get => _temp_gettables_paging; }

        public string Get_TableDataCount { get => _temp_getTableDataCount; }

        /// <summary>
        /// 获取创建视图的模板
        /// </summary>
        public string Get_CreateView { get => _temp_create_view; }


        /// <summary>
        /// 修改视图
        /// </summary>
        public string Get_ModiView { get => _temp_modi_view; }


        /// <summary>
        /// 字段重命名
        /// </summary>
        public string Re_Column { get => _temp_recolumn; }


        /// <summary>
        /// 对表进行重命名
        /// </summary>
        public string Re_Table { get => _temp_retable; }

        /// <summary>
        /// 删除视图
        /// </summary>
        public string Get_DropView { get => _temp_drop_view; }

        /// <summary>
        /// 获取表或视图是否存在
        /// </summary>
        public string Get_CheckTabExists { get => _temp_check_table_exists; }

        /// <summary>
        /// 获取全局临时表
        /// </summary>
        public string Get_GlobalTables { get => _temp_globaltempdb_query; }


        /// <summary>
        /// 获取索引明细
        /// </summary>
        public string Get_IndexDetail { get => _temp_get_indexdetail; }

        /// <summary>
        /// 获取指定表的索引
        /// </summary>

        public string Get_TabIndexs { get => _temp_get_tabindex; }



        /// <summary>
        /// 获取创建索引
        /// </summary>
        public string Get_CreateIndex { get => _temp_create_index; }


        /// <summary>
        /// 删除索引
        /// </summary>
        public string Get_DropIndex { get => _temp_drop_index; }

        /// <summary>
        /// 根据表的类型生成对应数据库的名称
        /// </summary>
        /// <param name="tableType"></param>
        /// <returns></returns>
        public string TabFullName(TableType tableType, string tabname)
        {
            Dictionary<string, string> _dic = Tool.RegexGrp(Constants.REG_TABNAME, tabname);
            if (_dic.ContainsKey("tab"))
            {
                if (tableType == TableType.Global)
                {
                    return $"##{_dic["tab"].ToString()}";
                }
                else if (tableType == TableType.Local)
                {
                    return $"#{_dic["tab"].ToString()}";
                }
                else if (tableType == TableType.Var)
                {
                    return $"@{_dic["tab"].ToString()}";
                }
                else
                    return tabname;
            }
            else
                throw new Exception($"表名[{tabname}]非法");
        }

        public PostGreSqlConfig()
        {


        }
        public PostGreSqlConfig(bool init)
        {

            Init();
        }


        public void Init()
        {

            _temp_addcolumn = $"alter table {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after} add COLUMN [$TempColumn$] ;";

            _temp_modicolumn = $"alter table  {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after} alter column [$TempColumn$];";
            
            _temp_delcolumn = $"ALTER TABLE  {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after}  DROP COLUMN {_temp_field_pre}[$FieldName$]{_temp_field_after} CASCADE;";

            _temp_recolumn = $"ALTER TABLE {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after} RENAME COLUMN {_temp_field_pre}[$FieldName$]{_temp_field_after} TO {_temp_field_pre}[$ReFieldName$]{_temp_field_after};";

            _temp_setdefalut = $"alter table  {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after} alter column {_temp_field_pre}[$FieldName$]{_temp_field_after} set [$DEFAULT$];";
            _temp_deldefalut = $"alter table  {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after} alter column {_temp_field_pre}[$FieldName$]{_temp_field_after} DROP DEFAULT;";


            _temp_setNotNull = $"alter table  {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after} alter column {_temp_field_pre}[$FieldName$]{_temp_field_after} set NOT NULL;";
            _temp_delNotNull = $"alter table  {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after} alter column {_temp_field_pre}[$FieldName$]{_temp_field_after} DROP NOT NULL;";


            _temp_fun_date = "CURRENT_TIMESTAMP";

            _lstdefmapping.Add(new DefMapping { IsRegex = false, DbValue = @"", DBDefault = HiTypeDBDefault.NONE });
            
            //数字默认
            _lstdefmapping.Add(new DefMapping { IsRegex = true, DbValue = @"^\(\((?<value>[-]?\d+(?:[\.]?)[\d]*)\)\)$", DbType = HiTypeGroup.Number, DBDefault = HiTypeDBDefault.VALUE });

            //true or false
            _lstdefmapping.Add(new DefMapping { IsRegex = true, DbValue = @"^(?<value>false|true)$", DbType = HiTypeGroup.Bool, DBDefault = HiTypeDBDefault.VALUE });

            //指定字符默认值 如('hisql') 或('')
            _lstdefmapping.Add(new DefMapping { IsRegex = true, DbValue = @"^\(\'(?<value>[\w\s*\S*\W*]*)\'\)$", DbType = HiTypeGroup.Char, DBDefault = HiTypeDBDefault.VALUE });

            //日期
            _lstdefmapping.Add(new DefMapping { IsRegex = true, DbValue = @"^(?<value>CURRENT_TIMESTAMP)$", DbType = HiTypeGroup.Date, DBDefault = HiTypeDBDefault.FUNDATE });

           

            _dbmapping = new Dictionary<HiType, string> {
                { HiType.NVARCHAR,"nvarchar" },
                { HiType.VARCHAR,"varchar" },
                { HiType.NCHAR,"nchar" },
                { HiType.CHAR,"char" },
                { HiType.TEXT,"text" },

                { HiType.INT,"int" },
                { HiType.BIGINT,"bigint" },
                { HiType.SMALLINT,"smallint" },
                { HiType.DECIMAL,"decimal" },

                { HiType.BOOL,"bit" },

                { HiType.DATETIME,"datetime" },
                { HiType.DATE,"date" },

                { HiType.BINARY,"binary" },
                { HiType.GUID,"uniqueidentifier" },
            };

            _fieldtempmapping = new Dictionary<string, string> {
                //样例：[TabName] [varchar](50) NOT NULL,
                { "nvarchar",$"{_temp_field_pre}[$FieldName$]{_temp_field_after}  varchar([$FieldLen$])  [$IsNull$]  [$Default$]  [$EXTEND$]  "},
                { "varchar",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} varchar([$FieldLen$])   [$IsNull$] [$Default$] [$EXTEND$] "},
                { "nchar",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} char([$FieldLen$])   [$IsNull$] [$Default$]  [$EXTEND$] "},
                { "char",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} char([$FieldLen$])   [$IsNull$] [$Default$]  [$EXTEND$] "},
                //样例：[udescript] [text] NULL,
                { "text",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} text   [$IsNull$] [$Default$] [$EXTEND$] "},

                { "int",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} integer [$IsIdentity$] [$IsNull$] [$Default$] [$EXTEND$] "},
                { "bigint",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} bigint [$IsIdentity$] [$IsNull$] [$Default$]  [$EXTEND$] " },
                { "smallint",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} smallint [$IsNull$] [$Default$]  [$EXTEND$] "},
                { "decimal",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} decimal([$FieldLen$],[$FieldDec$])  [$IsNull$] [$Default$] [$EXTEND$] "},

                { "bit",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} bool  [$IsNull$] [$Default$]  [$EXTEND$] "},

                { "datetime",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} TIMESTAMP  [$IsNull$] [$Default$]  [$EXTEND$] "},
                { "date",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} date  [$IsNull$] [$Default$]  [$EXTEND$] " },

                { "binary",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} bytea  [$IsNull$]  [$EXTEND$] "},
                { "uniqueidentifier",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} varchar(36)   [$IsNull$] [$Default$]  [$EXTEND$] "},
            };

            _fieldtempmappingForModiType = new Dictionary<string, string> {
                //样例：[TabName] [varchar](50) NOT NULL,
                { "nvarchar",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} TYPE varchar([$FieldLen$])"},
                { "varchar",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} TYPE varchar([$FieldLen$])"},
                { "nchar",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} TYPE char([$FieldLen$])"},
                { "char",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} TYPE char([$FieldLen$])"},
                //样例：[udescript] [text] NULL,
                { "text",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} TYPE text"},

                { "int",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} TYPE integer"},
                { "bigint",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} TYPE bigint" },
                { "smallint",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} TYPE smallint  "},
                { "decimal",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} TYPE decimal([$FieldLen$],[$FieldDec$])"},

                { "bit",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} TYPE bool"},

                { "datetime",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} TYPE TIMESTAMP"},
                { "date",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} TYPE date" },

                { "binary",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} TYPE bytea"},
                { "uniqueidentifier",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} TYPE varchar(36)"},
            };
            //_fieldtempmappingForModiDEFAULT = new Dictionary<string, string> {
            //    //样例：[TabName] [varchar](50) NOT NULL,
            //    { "nvarchar",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} "},
            //    { "varchar",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} set [$Default$]"},
            //    { "nchar",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} set [$Default$]"},
            //    { "char",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} set [$Default$]"},
            //    //样例：[udescript] [text] NULL,
            //    { "text",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} set [$Default$]"},

            //    { "int",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} set [$Default$]"},
            //    { "bigint",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} set [$Default$]" },
            //    { "smallint",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} set [$Default$]"},
            //    { "decimal",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} set [$Default$]"},

            //    { "bit",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} set [$Default$]"},

            //    { "datetime",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} set [$Default$]"},
            //    { "date",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} set [$Default$]" },

            //    { "binary",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} set [$Default$]"},
            //    { "uniqueidentifier",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} set [$Default$]"},
            //};
           //_fieldtempmappingForModiCONSTRAINT = new Dictionary<string, string> {
           //     //样例：[TabName] [varchar](50) NOT NULL,
           //     { "nvarchar",$"{_temp_field_pre}[$FieldName$]{_temp_field_after}  varchar([$FieldLen$])  [$IsNull$]  [$Default$]  [$EXTEND$]  "},
           //     { "varchar",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} varchar([$FieldLen$])   [$IsNull$] [$Default$] [$EXTEND$] "},
           //     { "nchar",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} char([$FieldLen$])   [$IsNull$] [$Default$]  [$EXTEND$] "},
           //     { "char",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} char([$FieldLen$])   [$IsNull$] [$Default$]  [$EXTEND$] "},
           //     //样例：[udescript] [text] NULL,
           //     { "text",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} text   [$IsNull$] [$Default$] [$EXTEND$] "},

           //     { "int",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} integer [$IsIdentity$] [$IsNull$] [$Default$] [$EXTEND$] "},
           //     { "bigint",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} bigint [$IsIdentity$] [$IsNull$] [$Default$]  [$EXTEND$] " },
           //     { "smallint",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} smallint [$IsNull$] [$Default$]  [$EXTEND$] "},
           //     { "decimal",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} decimal([$FieldLen$],[$FieldDec$])  [$IsNull$] [$Default$] [$EXTEND$] "},

           //     { "bit",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} bool  [$IsNull$] [$Default$]  [$EXTEND$] "},

           //     { "datetime",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} TIMESTAMP  [$IsNull$] [$Default$]  [$EXTEND$] "},
           //     { "date",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} date  [$IsNull$] [$Default$]  [$EXTEND$] " },

           //     { "binary",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} bytea  [$IsNull$]  [$EXTEND$] "},
           //     { "uniqueidentifier",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} varchar(36)   [$IsNull$] [$Default$]  [$EXTEND$] "},
           // };

            //
            _temp_sequence = new StringBuilder()
                .AppendLine($"DROP SEQUENCE if EXISTS {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]_[$FieldName$]_seq{_temp_table_after};  ")
                .AppendLine($"CREATE SEQUENCE {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]_[$FieldName$]_seq{_temp_table_after}  ")
                .AppendLine("INCREMENT 1  ")
                .AppendLine("MINVALUE 1 ")
                .AppendLine("MAXVALUE 9223372036854775807  ")
                .AppendLine("START 1  ")
                .AppendLine("CACHE 1;  ")
                .ToString();

            _temp_sequence_temp = new StringBuilder()
                .AppendLine($"DROP SEQUENCE if EXISTS  {_temp_table_pre}[$TabName$]_[$FieldName$]_seq{_temp_table_after};  ")
                .AppendLine($"CREATE SEQUENCE  {_temp_table_pre}[$TabName$]_[$FieldName$]_seq{_temp_table_after}  ")
                .AppendLine("INCREMENT 1  ")
                .AppendLine("MINVALUE 1 ")
                .AppendLine("MAXVALUE 9223372036854775807  ")
                .AppendLine("START 1  ")
                .AppendLine("CACHE 1;  ")
                .ToString();


            _temp_create_table = new StringBuilder()
                //样例：CREATE TABLE [dbo].[H_TEST_USER]
                .AppendLine("[$Sequence$]")
                .AppendLine($"CREATE TABLE  IF NOT EXISTS {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after} (")
                .AppendLine("[$Fields$]")
                .AppendLine("[$Keys$]")//主键
                .AppendLine(");")

                .AppendLine("[$Primary$]")
                .AppendLine("[$Comment$]")
                .AppendLine($"delete from {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}{Constants.HiSysTable["Hi_TabModel"].ToString()}{_temp_table_after} where {_temp_field_pre}TabName{_temp_field_after}='[$TabName$]';")
                .AppendLine($"delete from  {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}{Constants.HiSysTable["Hi_FieldModel"].ToString()}{_temp_table_after} where {_temp_field_pre}TabName{_temp_field_after}='[$TabName$]';")

                .AppendLine("[$TabStruct$]")
                
             
                .ToString();

            _temp_create_temp_global_table = new StringBuilder()
                //样例：CREATE TABLE [dbo].[H_TEST_USER]
                .AppendLine("[$Sequence$]")
                .AppendLine($"CREATE TEMP TABLE  IF NOT EXISTS  {_temp_table_pre}[$TabName$]{_temp_table_after} (")
                .AppendLine("[$Fields$]")
                .AppendLine("[$Keys$]")//主键
                .AppendLine(");")
                .AppendLine("[$Primary$]")
   
                .ToString();

            _temp_create_temp_global_table_drop = new StringBuilder()
                //样例：CREATE TABLE [dbo].[H_TEST_USER]
                .AppendLine("[$Sequence$]")
                .AppendLine($"drop table if exists  {_temp_table_pre}[$TabName$]{_temp_table_after};")
                .AppendLine($"CREATE TEMP TABLE  IF NOT EXISTS  {_temp_table_pre}[$TabName$]{_temp_table_after} (")
                .AppendLine("[$Fields$]")
                .AppendLine("[$Keys$]")//主键
                .AppendLine(");")
                .AppendLine("[$Primary$]")
                .ToString();


            _temp_create_temp_local_table = new StringBuilder()
                //样例：CREATE TABLE [dbo].[H_TEST_USER]
                .AppendLine("[$Sequence$]")
                .AppendLine($"CREATE TEMP TABLE  IF NOT EXISTS  {_temp_table_pre}[$TabName$]{_temp_table_after} (")
                .AppendLine("[$Fields$]")
                .AppendLine("[$Keys$]")//主键
                .AppendLine(");")
                .AppendLine($"DELETE FROM {_temp_table_pre}[$TabName$]{_temp_table_after};")
                .AppendLine("[$Primary$]")
                .ToString();

            _temp_create_temp_local_table_drop = new StringBuilder()
                //样例：CREATE TABLE [dbo].[H_TEST_USER]
                .AppendLine("[$Sequence$]")
                .AppendLine($"drop table if exists  {_temp_table_pre}[$TabName$]{_temp_table_after};")
                .AppendLine($"CREATE TEMP  TABLE  IF NOT EXISTS  {_temp_table_pre}[$TabName$]{_temp_table_after} (")
                .AppendLine("[$Fields$]")
                .AppendLine("[$Keys$]")//主键
                .AppendLine(");")
                .AppendLine("[$Primary$]")
                .ToString();


            //变量表模版
            _temp_declare_table = new StringBuilder()
                .AppendLine($"declare    [$TabName$]  TABLE(")
                .AppendLine("[$Fields$]")
                .AppendLine("[$Keys$]")//主键
                .AppendLine(")[$Primary$]")
                .ToString();

            //表创建时的KEY模版
            _temp_tabel_key = new StringBuilder()
                .AppendLine("[$Keys$]")
                .ToString();
            _temp_table_key2 = "[$FieldName$] ";//定义主键的排序方式

            _temp_table_key3 = $"ALTER TABLE {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after} ADD CONSTRAINT {_temp_field_pre}[$TabName$]_pkey{_temp_field_after} PRIMARY KEY ([$Keys$]);";//TEXTIMAGE_ON [PRIMARY]
            _temp_table_key3_temp = $"ALTER TABLE {_temp_table_pre}[$TabName$]{_temp_table_after} ADD CONSTRAINT {_temp_field_pre}[$TabName$]_pkey{_temp_field_after} PRIMARY KEY ([$Keys$]);";

            _temp_field_comment = new StringBuilder()
                .AppendLine($"COMMENT ON COLUMN {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after}.{_temp_field_pre}[$FieldName$]{_temp_field_after} IS '[$FieldDesc$]';")
                // .AppendLine("GO")
                .ToString();



            _temp_insert_statement = new StringBuilder()
                .AppendLine($"insert into {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after}([$FIELDS$]) values([$VALUES$])")
                .ToString();

            /*
             * 
             * INSERT [OrderBy]  ([Name],[Price],[CreateTime],[CustomId])
 SELECT N'order11' AS [Name],N'0' AS [Price],'1900-01-01 00:00:00.000' AS [CreateTime],N'0' AS [CustomId]	
UNION ALL 
 SELECT N'order12' AS [Name],N'0' AS [Price],'1900-01-01 00:00:00.000' AS [CreateTime],N'0' AS [CustomId]
;
             * 
             * *
            */

            _temp_insert_statementv2 = new StringBuilder()
                .AppendLine($"insert into {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after}([$FIELDS$]) [$VALUES$]")
                .ToString();


            /*
             * 
             * TabType:表类型 Table表示实体表 View表示视图
             * TabName:表名
             * FieldNo:字段排序号
             * FieldName:字段名
             * IsIdentity：是否自增加0，1
             * IsPrimary：是否主键 0，1
             * FieldType：字段类型
             * UseBytes：字段占的字节数（不常用）
             * Lens：字段长度
             * PointDec：数值型的小数位长度
             * IsNull：是否允许为空 0，1
             * DbDefault：默认值
             * FieldDesc：字段描述
             * 
             * 
             */

            _temp_get_table_schema = new StringBuilder()
                .AppendLine("SELECT ")
                .AppendLine("   case when (select count(pt.tablename) from pg_tables as pt where pt.tablename=c.relname) = '1' then 'Table' else 'View' end as \"TabType\",")
                .AppendLine("   C .relname as \"TabName\",")
                .AppendLine("   A .attname AS \"FieldName\",")
                .AppendLine("   a.attnum as \"FieldNo\",")
                .AppendLine("   CASE")
                .AppendLine("       WHEN t.typname ='varchar' then 'nvarchar'  ")
                .AppendLine("       WHEN t.typname ='name' then 'nvarchar'  ")
                .AppendLine("       WHEN t.typname ='char' then 'nchar'  ")
                .AppendLine("       WHEN t.typname ='int2' then 'smallint'  ")
                .AppendLine("       WHEN t.typname ='int4' then 'int'  ")
                .AppendLine("       WHEN t.typname ='int8' then 'bigint'  ")
                .AppendLine("       WHEN t.typname ='decimal' then 'decimal'  ")
                .AppendLine("       WHEN t.typname ='numeric' then 'decimal'  ")
                .AppendLine("       WHEN t.typname ='float4' then 'decimal'  ")
                .AppendLine("       WHEN t.typname ='float8' then 'decimal'  ")
                .AppendLine("       WHEN t.typname ='money' then 'decimal'  ")
                .AppendLine("       WHEN t.typname ='timestamp' then 'datetime'  ")
                .AppendLine("       WHEN t.typname ='date' then 'date'  ")
                .AppendLine("       WHEN t.typname ='bool' then 'bit'  ")
                .AppendLine("       WHEN t.typname ='bpchar' then 'char'  ")
                .AppendLine("       WHEN t.typname ='text' then 'text'  ")
                .AppendLine("       WHEN t.typname ='xml' then 'text'  ")
                .AppendLine("       WHEN t.typname ='json' then 'text'  ")
                .AppendLine("       WHEN t.typname ='oid' then 'bigint'  ")
                .AppendLine("       WHEN t.typname ='bytea' then 'binary'  ")
                .AppendLine("       ELSE 'varchar'")
                .AppendLine("   end as \"FieldType\",")
                .AppendLine("   concat_ws('' ,SUBSTRING(format_type(a.atttypid,a.atttypmod) from '(\\d+)')) as \"UseBytes\",")
                .AppendLine("   concat_ws('' ,SUBSTRING(format_type(a.atttypid,a.atttypmod) from '(\\d+)')) as \"Lens\",")
                .AppendLine("   concat_ws('' ,SUBSTRING(format_type(a.atttypid,a.atttypmod) from ',(\\d+)')) as \"PointDec\",")
                .AppendLine("   case when b.description is null then '' else b.description end as \"FieldDesc\",")
                .AppendLine("   (select column_default from information_schema.columns where table_schema='[$Schema$]' and  table_name = C .relname and column_name =A .attname) as \"DbDefault\",")
                .AppendLine("   case  when A .attnotnull = 't' THEN 'False' else 'True' end AS \"IsNull\",")
                .AppendLine("   case when(select count(pc.conname) from pg_constraint  pc where a.attnum = pc.conkey [ 1 ] and pc.conrelid = c.oid) = '1' then 'True' else 'False' end as \"IsPrimary\",")
                .AppendLine("   case when   A .attnotnull = 't' and (select count(column_default) from information_schema.columns where table_schema='[$Schema$]' and  table_name = C .relname and column_name =A .attname and column_default like 'nextval(%' )='1'  and (select count(pc.conname) from pg_constraint  pc where a.attnum = pc.conkey [ 1 ] and pc.conrelid = c.oid) = '1' then 'True' else 'Fasle' end as \"IsIdentity\"")
                .AppendLine("FROM")
                .AppendLine("   pg_class C, pg_namespace as ns,")
                .AppendLine("   pg_attribute A ")
                .AppendLine("   LEFT OUTER JOIN pg_description b ON A .attrelid = b.objoid")
                .AppendLine("       AND A .attnum = b.objsubid,")
                .AppendLine("   pg_type T")
                .AppendLine("WHERE ns.oid = C.relnamespace and ns.nspname='[$Schema$]' and ")
                .AppendLine("   C .relname in(select pt.tablename from pg_tables pt where pt.schemaname = '[$Schema$]' UNION all select pv.viewname as tablename from pg_views as pv where pv.schemaname = '[$Schema$]') AND A.attnum > 0")
                .AppendLine("   AND A .attrelid = C .oid AND A .atttypid = T .oid and c.relname='[$TabName$]'")
                .AppendLine("ORDER BY C .relname,A .attnum;")
                .ToString();



            //批量MERGE更新模版
            /*
             
             insert into "public"."Hi_Domain"("Domain","DomainDesc","CreateName","ModiName") values('U0','用户0','Hone','Hone')
,('U1','用户1','Hone','Hone'),('U2','用户2','Hone','Hone'),('U3','用户3','Hone','Hone'),('U4','用户4','Hone','Hone'),('U5','用户5','Hone','Hone'),('U6','用户6','Hone','Hone'),('U7','用户7','Hone','Hone'),('U8','用户8','Hone','Hone'),('U9','用户9','Hone','Hone')
ON CONFLICT ("Domain") DO UPDATE
SET "DomainDesc" = excluded."DomainDesc";
             */
            _temp_merge_into = new StringBuilder()
                .AppendLine($"[$Insert$]")
                .AppendLine("ON CONFLICT ([$Keys$]) DO UPDATE")
                .AppendLine($"SET [$Update$] ;")
                .ToString();

            //表更新 不带条件 
            _temp_update = $"update {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after} set [$Fields$];";

            //表更新 带条件 
            _temp_update_where = $"update {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after} set [$Fields$] where [$Where$];";

            _temp_delete = $"delete from {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after};";

            _temp_delete_where = $"delete from {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after} where [$Where$];";

            //删除不会留下任何痕迹
            _temp_truncate = $"TRUNCATE TABLE {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after};";

            _temp_droptable = $"drop table {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after};";

            _temp_gettables = new StringBuilder()
               .AppendLine("select * from ( ")
                .AppendLine($@"SELECT tablename as {_temp_field_pre}TabName{_temp_field_after}, 'Table' as {_temp_field_pre}TabType{ _temp_field_after},'' as {_temp_field_pre}CreateTime{_temp_field_after} FROM pg_tables where schemaname='[$Schema$]' [$Where$]")
               .AppendLine(")as temp")
               .AppendLine("ORDER BY \"TabName\" ASC, \"CreateTime\" desc ")
               .ToString();

            _temp_gettables_paging = new StringBuilder()
                .AppendLine(@"select count(*)   FROM pg_tables where schemaname='[$Schema$]' [$Where$]; ")
               .AppendLine("select * from ( ")
                .AppendLine($@"SELECT ROW_NUMBER()OVER( order by tablename ASC) as row_seq ,tablename as {_temp_field_pre}TabName{_temp_field_after}, 'Table' as {_temp_field_pre}TabType{ _temp_field_after},'' as {_temp_field_pre}CreateTime{_temp_field_after} FROM pg_tables where schemaname='[$Schema$]' [$Where$]")
               .AppendLine(")as temp    WHERE row_seq > [$SeqBegin$] AND row_seq <=[$SeqEnd$] ")
               .AppendLine("ORDER BY row_seq ")
               .ToString();

            _temp_getviews = new StringBuilder()
               .AppendLine("select * from ( ")
                .AppendLine($@"SELECT viewname as {_temp_field_pre}TabName{_temp_field_after}, 'View' as {_temp_field_pre}TabType{ _temp_field_after}
                        ,'' as {_temp_field_pre}CreateTime{_temp_field_after} FROM pg_views where schemaname='[$Schema$]' [$Where$]")
               .AppendLine(")as temp  ")
               .AppendLine("ORDER BY \"TabName\" ASC, \"CreateTime\" desc ")
               .ToString();

            _temp_getviews_paging = new StringBuilder()
             .AppendLine(@"select count(*)   FROM pg_views where schemaname='[$Schema$]' [$Where$]; ")
            .AppendLine("select * from ( ")
             .AppendLine($@"SELECT ROW_NUMBER()OVER( order by viewname ASC) as row_seq ,viewname as {_temp_field_pre}TabName{_temp_field_after}, 'View' as {_temp_field_pre}TabType{ _temp_field_after}
                        ,'' as {_temp_field_pre}CreateTime{_temp_field_after} FROM pg_views where schemaname='[$Schema$]' [$Where$]")
            .AppendLine(")as temp    WHERE row_seq > [$SeqBegin$] AND row_seq <=[$SeqEnd$] ")
            .AppendLine("ORDER BY row_seq ")
            .ToString();

            
            _temp_getalltables = new StringBuilder()
              .AppendLine("select * from ( ")
                .AppendLine($@"SELECT tablename as {_temp_field_pre}TabName{_temp_field_after}, 'Table' as {_temp_field_pre}TabType{ _temp_field_after},'' as {_temp_field_pre}CreateTime{_temp_field_after} FROM pg_tables where schemaname='[$Schema$]'")
               .AppendLine("union all")
                .AppendLine($@"SELECT viewname as {_temp_field_pre}TabName{_temp_field_after}, 'View' as {_temp_field_pre}TabType{ _temp_field_after}
                        ,'' as {_temp_field_pre}CreateTime{_temp_field_after} FROM pg_views where schemaname='[$Schema$]'")
              .AppendLine(")as temp [$Where$]")
              .AppendLine("ORDER BY \"TabName\" ASC, \"CreateTime\" desc ")
              .ToString();

            _temp_getalltables_paging = new StringBuilder()
                .AppendLine("select count(*) from ( ")
                .AppendLine($@"SELECT tablename as {_temp_field_pre}TabName{_temp_field_after}, 'Table' as {_temp_field_pre}TabType{ _temp_field_after},'' as {_temp_field_pre}CreateTime{_temp_field_after} FROM pg_tables where schemaname='[$Schema$]'")
               .AppendLine("union all")
                .AppendLine($@"SELECT viewname as {_temp_field_pre}TabName{_temp_field_after}, 'View' as {_temp_field_pre}TabType{ _temp_field_after}
                        ,'' as {_temp_field_pre}CreateTime{_temp_field_after} FROM pg_views where schemaname='[$Schema$]'")
              .AppendLine(")as temp where 1=1 [$Where$]; ")
               .AppendLine($@"select * from (select ROW_NUMBER()OVER( order by {_temp_field_pre}TabName{_temp_field_pre} ASC, {_temp_field_pre}TabType{ _temp_field_after} asc,  {_temp_field_pre}CreateTime{_temp_field_pre} desc ) as row_seq ,* from ( ")
                .AppendLine($@"SELECT tablename as {_temp_field_pre}TabName{_temp_field_after}, 'Table' as {_temp_field_pre}TabType{ _temp_field_after},'' as {_temp_field_pre}CreateTime{_temp_field_after} FROM pg_tables where schemaname='[$Schema$]'")
               .AppendLine("union all")
                .AppendLine($@"SELECT viewname as {_temp_field_pre}TabName{_temp_field_after}, 'View' as {_temp_field_pre}TabType{ _temp_field_after}
                        ,'' as {_temp_field_pre}CreateTime{_temp_field_after} FROM pg_views where schemaname='[$Schema$]'")
              .AppendLine(")as temp where 1=1 [$Where$] ) as t WHERE row_seq > [$SeqBegin$] AND row_seq <=[$SeqEnd$] ORDER BY row_seq  ")
                  .ToString();

            
            _temp_check_table_exists = new StringBuilder()
                .AppendLine("select * from ( ")
                .AppendLine($@"SELECT tablename as {_temp_field_pre}TabName{_temp_field_after}, 'Table' as {_temp_field_pre}TabType{ _temp_field_after},'' as {_temp_field_pre}CreateTime{_temp_field_after} FROM pg_tables where schemaname='[$Schema$]'")
               .AppendLine("union all")
                .AppendLine($@"SELECT viewname as {_temp_field_pre}TabName{_temp_field_after}, 'View' as {_temp_field_pre}TabType{ _temp_field_after}
                        ,'' as {_temp_field_pre}CreateTime{_temp_field_after} FROM pg_views where schemaname='[$Schema$]'")
              .AppendLine(")as temp  WHERE \"TabName\" = '[$TabName$]'")
              .AppendLine("ORDER BY \"TabName\" ASC, \"CreateTime\" desc ")
              .ToString();



            //删除视图
            _temp_drop_view = $"DROP VIEW [$Schema$].[$TabName$];";

            //创建视图
            _temp_create_view = new StringBuilder()
                .AppendLine($"CREATE VIEW [$Schema$].[$TabName$] ")
                .AppendLine("AS")
                .AppendLine("[$ViewSql$];")
                .ToString();

            //修改视图
            _temp_modi_view = new StringBuilder()
                .AppendLine($"CREATE OR REPLACE VIEW [$Schema$].[$TabName$] ")
                .AppendLine("AS")
                .AppendLine("[$ViewSql$];")
                .ToString();

            //获取表索引
            _temp_get_tabindex = $"SELECT distinct A.TABLENAME as \"TableName\",A.INDEXNAME as \"IndexName\", case when INDISPRIMARY= true then 'Key_Index' ELSE 'Index' end as IndexType , '' as Disabled " +
              
                $@"FROM
                    PG_AM B
                    LEFT JOIN PG_CLASS F ON B.OID = F.RELAM
                    LEFT JOIN PG_STAT_ALL_INDEXES E ON F.OID = E.INDEXRELID
                    LEFT JOIN PG_INDEX C ON E.INDEXRELID = C.INDEXRELID
                    LEFT OUTER JOIN PG_DESCRIPTION D ON C.INDEXRELID = D.OBJOID,
                    PG_INDEXES A
                    WHERE
                    A.SCHEMANAME = E.SCHEMANAME AND A.TABLENAME = E.RELNAME AND A.INDEXNAME = E.INDEXRELNAME
                    and A.SCHEMANAME='[$Schema$]' and A.TABLENAME='[$TabName$]' ";

           //表索引明细
            _temp_get_indexdetail = @"
                                        select t.oid as ""TableId"" ,  
                                            t.relname as ""TableName"", 
                                            i.relname as ""IndexName"", case when INDISPRIMARY = true then 'Key_Index' ELSE 'Index' end as IndexType
                                            ,a.attnum as ""ColumnIdx"",a.attnum as ""ColumnID"", a.attname as ""ColumnName"", 
	                                        case when position(a.attname in substring(pg_get_indexdef(ix.indexrelid), 'INCLUDE \((.*?)\)')) > 0 then '' else 'Y' end as ""IsIncludedColumn""
	                                        , case position(a.attname || '"" DESC' in substring(pg_get_indexdef(ix.indexrelid), '\((.*?)\)'))  when 0 then 'asc' else 'desc' end as ""Sort""
	                                        , case when indisprimary = 'true' then 'Y' ELSE 'N' end as  ""IPrimary""
	                                        ,case when indisunique = 'true' then 'Y' ELSE 'N' end as  ""IsUnique""
	                                        , '' AS ""Ignore_dup_key""   , '' as ""Disabled""  , '' AS ""Fill_factor""  , '' AS ""Padded""
                                        from
                                            pg_class t,
                                            pg_class i,
                                            pg_index ix,
                                            pg_attribute a
                                        where
                                            t.oid = ix.indrelid
                                            and i.oid = ix.indexrelid
                                            and a.attrelid = t.oid
                                            and a.attnum = ANY(ix.indkey)
                                            and t.relname = '[$TabName$]'  and i.relname = '[$IndexName$]'
                                        order by
                                            t.relname,
                                            i.relname,a.attnum;
            ";


            //创建索引
            _temp_create_index = $@"
CREATE INDEX {_temp_schema_pre}[$IndexName$]{_temp_schema_after}
    ON {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after} ([$Key$]); ";

            //删除索引
            _temp_drop_index = new StringBuilder()
                .AppendLine($@"DROP INDEX {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_schema_pre}[$IndexName$]{_temp_schema_after};")
                .ToString();

            //重命名表
            _temp_retable = new StringBuilder()
                .AppendLine($@"ALTER TABLE {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after} RENAME TO {_temp_table_pre}[$ReTabName$]{_temp_table_after}; ")
                .ToString();

            

        }
    }
}
