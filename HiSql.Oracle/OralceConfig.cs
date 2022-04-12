using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public class OracleConfig : IDbConfig
    {
        /// <summary>
        /// 数据包大小 经过测试200这个值是比较合理
        /// </summary>
        int _bluksize = 200;
        int _bulkunitsize = 500000;
        string _temp_schema_pre = "";
        string _temp_schema_after = "";

        string _temp_table_pre = "";
        string _temp_table_after = "";

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



        string _temp_addcolumn = "alter table [$TabName$] add [$TempColumn$] ";

        string _temp_delcolumn = "alter table [$TabName$] drop column [$FieldName$]";

        string _temp_modicolumn = "alter table [$TabName$] MODIFY [$TempColumn$]";

        string _temp_recolumn = "alter table [$TabName$] RENAME COLUMN [$FieldName$] TO [$ReFieldName$]";
        
        string _temp_retable = "alter table [$TabName$] RENAME to [$ReTabName$]";
        string _temp_setdefalut = "";

        string _temp_deldefalut = "";

        /// <summary>
        /// 所有物理实体表
        /// </summary>
        string _temp_gettables = "";

        /// <summary>
        /// 获取所有视图
        /// </summary>
        string _temp_getviews = "";

        /// <summary>
        /// 获取表和视图
        /// </summary>
        string _temp_getalltables = "";


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
        Dictionary<HiType, string> _dbmapping = new Dictionary<HiType, string>();


        List<DefMapping> _lstdefmapping = new List<DefMapping>();


        /// <summary>
        /// 安装HiSql初始化
        /// </summary>
        public string InitSql
        {
            get
            { //return _temp_install;

                return HiSql.Oracle.Properties.Resources.HiSql.ToString();
            }
        }

        public List<DefMapping> DbDefMapping
        {
            get => _lstdefmapping;
        }
        public int BlukSize { get => _bluksize; set => _bluksize = value; }
        public int BulkUnitSize { get => _bulkunitsize; set => _bulkunitsize = value; }
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

        /// <summary>
        /// 字段重命名
        /// </summary>
        public string Re_Column { get => _temp_recolumn; }


        /// <summary>
        /// 对表进行重命名
        /// </summary>
        public string Re_Table { get => _temp_retable; }


        public string Set_Default { get => _temp_setdefalut; }

        public string Del_Default { get => _temp_deldefalut; }

        public string Get_Tables { get => _temp_gettables; }

        public string Get_Views { get => _temp_getviews; }

        public string Get_AllTables { get => _temp_getalltables; }

        /// <summary>
        /// 获取创建视图的模板
        /// </summary>
        public string Get_CreateView { get => _temp_create_view; }


        /// <summary>
        /// 修改视图
        /// </summary>
        public string Get_ModiView { get => _temp_modi_view; }


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

        public OracleConfig()
        {


        }
        public OracleConfig(bool init)
        {

            Init();
        }


        public void Init()
        {
            _temp_fun_date = "SYSTIMESTAMP";

            _lstdefmapping.Add(new DefMapping { IsRegex = false, DbValue = @"", DBDefault = HiTypeDBDefault.NONE });
            //数字默认
            _lstdefmapping.Add(new DefMapping { IsRegex = true, DbValue = @"^(?<value>[-]?\d+(?:[\.]?)[\d]*)$", DBDefault = HiTypeDBDefault.VALUE });

            //指定字符默认值 如('hisql') 或('')
            _lstdefmapping.Add(new DefMapping { IsRegex = true, DbValue = @"^\'(?<value>[\w\s*\S*\W*]*)\'$", DBDefault = HiTypeDBDefault.VALUE });

            //日期
            _lstdefmapping.Add(new DefMapping { IsRegex = true, DbValue = $@"^(?<value>{_temp_fun_date})\s*$", DBDefault = HiTypeDBDefault.FUNDATE });

            //md5值
            //_lstdefmapping.Add(new DefMapping { IsRegex = true, DbValue = @"^\((?<value>newid\(\))\)$", DBDefault = HiTypeDBDefault.FUNGUID });

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

                { HiType.BINARY,"image" },
                { HiType.GUID,"uniqueidentifier" },
            };

            _fieldtempmapping = new Dictionary<string, string> {
                //样例：[TabName] [varchar](50) NOT NULL,
                { "nvarchar",$"{_temp_field_pre}[$FieldName$]{_temp_field_after}  nvarchar2 ([$FieldLen$]) [$IsNull$]  [$Default$] [$EXTEND$]"},
                { "varchar",$"{_temp_field_pre}[$FieldName$]{_temp_field_after}  varchar2 ([$FieldLen$]) [$IsNull$] [$Default$]  [$EXTEND$] "},
                { "nchar",$"{_temp_field_pre}[$FieldName$]{_temp_field_after}  nchar ([$FieldLen$]) [$IsNull$] [$Default$] [$EXTEND$] "},
                { "char",$"{_temp_field_pre}[$FieldName$]{_temp_field_after}  char ([$FieldLen$]) [$IsNull$] [$Default$] [$EXTEND$] "},
                //样例：[udescript] [text] NULL,
                { "text",$"{_temp_field_pre}[$FieldName$]{_temp_field_after}  long  [$IsNull$] [$Default$] [$EXTEND$] "},

                { "int",$"{_temp_field_pre}[$FieldName$]{_temp_field_after}  integer  [$IsIdentity$] [$IsNull$] [$Default$] [$EXTEND$] "},
                { "bigint",$"{_temp_field_pre}[$FieldName$]{_temp_field_after}  number(18)  [$IsIdentity$] [$IsNull$] [$Default$] [$EXTEND$] " },
                { "smallint",$"{_temp_field_pre}[$FieldName$]{_temp_field_after}  number(5)   [$IsNull$] [$Default$] [$EXTEND$] "},
                { "decimal",$"{_temp_field_pre}[$FieldName$]{_temp_field_after}  decimal ([$FieldLen$],[$FieldDec$])  [$IsNull$] [$Default$] [$EXTEND$] "},

                { "bit",$"{_temp_field_pre}[$FieldName$]{_temp_field_after}  number(1)    [$IsNull$] [$Default$] [$EXTEND$] "},

                { "datetime",$"{_temp_field_pre}[$FieldName$]{_temp_field_after}  timestamp    [$IsNull$] [$Default$] [$EXTEND$] "},
                { "date",$"{_temp_field_pre}[$FieldName$]{_temp_field_after}  date   [$IsNull$] [$Default$] [$EXTEND$] " },

                { "image",$"{_temp_field_pre}[$FieldName$]{_temp_field_after}  BLOB    [$IsNull$] [$EXTEND$] "},
                { "uniqueidentifier",$"{_temp_field_pre}[$FieldName$]{_temp_field_after}  varchar2(36)   [$IsNull$] [$Default$] [$EXTEND$] "},
            };


            _temp_sequence = new StringBuilder()
                .AppendLine($"select count(*) into v_secout from user_sequences where SEQUENCE_NAME = upper('[$TabName$]_[$FieldName$]_SEQ'); ")
                .AppendLine($"IF v_secout > 0 then")
                .AppendLine($"   execute immediate 'drop sequence [$TabName$]_[$FieldName$]_SEQ';")
                .AppendLine($"end if;")
                .AppendLine($"execute immediate 'create sequence [$TabName$]_[$FieldName$]_SEQ increment by 1 start with 1 minvalue 1 maxvalue 999999999999999';")
                .ToString();

            _temp_sequence_temp = new StringBuilder()
                .AppendLine($"select count(*) into v_secout from user_sequences where SEQUENCE_NAME = upper('[$TabName$]_[$FieldName$]_SEQ'); ")
                .AppendLine($"IF v_secout > 0 then")
                .AppendLine($"   execute immediate 'drop sequence [$TabName$]_[$FieldName$]_SEQ';")
                .AppendLine($"end if;")
                .AppendLine($"execute immediate 'create sequence [$TabName$]_[$FieldName$]_SEQ increment by 1 start with 1 minvalue 1 maxvalue 999999999999999';")
                .ToString();


            _temp_create_table = new StringBuilder()
                .AppendLine("declare ")
                .AppendLine($"  v_number integer;")
                .AppendLine($"  v_number2 integer;")
                .AppendLine("   v_secout integer;")
                .AppendLine($"begin")
                .AppendLine($"   select count(*)  into v_number  from user_tables  where  table_name=upper('[$TabName$]');")
                .AppendLine($"   select count(*)  into v_number2  from user_views  where  view_name=upper('[$TabName$]');")
                .AppendLine($"   IF v_number = 0  and v_number2=0 then")
                .AppendLine($"      execute immediate  'create table [$TabName$]('")
                .AppendLine("           [$Fields$]")
                .AppendLine($"      || ') ';")
                .AppendLine($"      [$Sequence$]")

                .AppendLine("    [$Primary$]")
                .AppendLine($"   end if;")
                //.AppendLine($"        execute immediate 'drop table [$TabName$] ';")
                
                //.AppendLine($"   select count(*) into v_secout from user_sequences where SEQUENCE_NAME = upper('[$TabName$]_SEQ');")
                //.AppendLine($"   IF v_secout > 0 then")
                //.AppendLine($"       execute immediate 'drop sequence [$TabName$]_SEQ';")
                //.AppendLine($"   end if;")
                //.AppendLine($"   execute immediate 'create sequence [$TabName$]_SEQ increment by 1 start with 1 minvalue 1 maxvalue 999999999999';")

                

                .AppendLine($"   execute immediate 'delete from {Constants.HiSysTable["Hi_TabModel"].ToString()} where TabName=''[$TabName$]''';")
                .AppendLine($"   execute immediate 'delete from {Constants.HiSysTable["Hi_FieldModel"].ToString()} where TabName=''[$TabName$]''';")

                
                .AppendLine("    [$TabStruct$]")
                .AppendLine("   [$Comment$]")
                .AppendLine("end;")
                .ToString();

            _temp_create_temp_global_table = new StringBuilder()
                .AppendLine("declare ")
                .AppendLine($"  v_number integer;")
                .AppendLine("   v_secout integer;")
                .AppendLine($"begin")
                .AppendLine($"   select count(*)  into v_number  from user_tables  where  table_name=upper('[$TabName$]') AND TEMPORARY='Y';")
                .AppendLine($"   IF v_number = 0 then")

                .AppendLine($"      [$Sequence$]")
                .AppendLine($"      execute immediate  'create  Global Temporary table [$TabName$]('")
                .AppendLine("           [$Fields$]")
                .AppendLine($"      || ') ';")

                .AppendLine("       [$Primary$]")

                .AppendLine($"   end if;")
                .AppendLine("end;")
                .ToString();

            _temp_create_temp_global_table_drop = new StringBuilder()
                .AppendLine("declare ")
                .AppendLine($"  v_number integer;")
                .AppendLine("   v_secout integer;")
                .AppendLine($"begin")
                .AppendLine($"   select count(*)  into v_number  from user_tables  where  table_name=upper('[$TabName$]') AND TEMPORARY='Y';")
                .AppendLine($"   IF v_number > 0 then")
                .AppendLine($"      execute immediate 'TRUNCATE TABLE [$TabName$] ';")
                .AppendLine($"      execute immediate 'drop table [$TabName$] ';")
                .AppendLine($"   end if;")

                .AppendLine($"  [$Sequence$]")
                .AppendLine($"  execute immediate  'create  Global Temporary table [$TabName$]('")
                .AppendLine("       [$Fields$]")
                .AppendLine($"  || ') ';")

                .AppendLine("   [$Primary$]")

                
                .AppendLine("end;")
                .ToString();


            _temp_create_temp_local_table = new StringBuilder()
                .AppendLine("declare ")
                .AppendLine($"  v_number integer;")
                .AppendLine("   v_secout integer;")
                .AppendLine($"begin")
                .AppendLine($"   select count(*)  into v_number  from user_tables  where  table_name=upper('[$TabName$]') AND TEMPORARY='Y';")
                .AppendLine($"   IF v_number = 0 then")

                .AppendLine($"      [$Sequence$]")
                .AppendLine($"      execute immediate  'create  Global Temporary table [$TabName$]('")
                .AppendLine("           [$Fields$]")
                .AppendLine($"      || ')ON COMMIT PRESERVE ROWS ';")

                .AppendLine("       [$Primary$]")
                .AppendLine("   execute immediate 'delete [$TabName$] ';")
                .AppendLine($"   end if;")
                .AppendLine("end;")
                .ToString();

            _temp_create_temp_local_table_drop = new StringBuilder()
                .AppendLine("declare ")
                .AppendLine($"  v_number integer;")
                .AppendLine("   v_secout integer;")
                .AppendLine($"begin")
                .AppendLine($"   select count(*)  into v_number  from user_tables  where  table_name=upper('[$TabName$]') AND TEMPORARY='Y';")
                .AppendLine($"   IF v_number > 0 then")
                .AppendLine($"      execute immediate 'TRUNCATE TABLE [$TabName$] ';")
                .AppendLine($"      execute immediate 'drop table [$TabName$] ';")
                .AppendLine($"   end if;")

                .AppendLine($"  [$Sequence$]")
                .AppendLine($"  execute immediate  'create  Global Temporary table [$TabName$]('")
                .AppendLine("       [$Fields$]")
                .AppendLine($"  || ')ON COMMIT PRESERVE ROWS ';")

                .AppendLine("   [$Primary$]")


                .AppendLine("end;")
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
                .Append($"alter table [$TabName$]  add constraint {_temp_table_pre}PK_[$TabName$]_[$ConnectID$]{_temp_table_after} primary key ([$Keys$])")
                .ToString();
            _temp_table_key2 = "[$FieldName$] ";//定义主键的排序方式

            _temp_table_key3 = "ON [PRIMARY] ";//TEXTIMAGE_ON [PRIMARY]


            _temp_field_comment = new StringBuilder()
                .Append($"comment on column {_temp_table_pre}[$TabName$]{_temp_table_after}.{_temp_field_pre}[$FieldName$]{_temp_field_after}  is '[$FieldDesc$]'")
               
                .ToString();

            _temp_insert_statement = new StringBuilder()
                .Append($"insert all into {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after}([$FIELDS$]) values([$VALUES$])")
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
                .Append($"insert into {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after}([$FIELDS$]) select  [$VALUES$] from dual")
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
                .AppendLine("	case")
                .AppendLine("	    when (select count(*) from user_tables  where  table_name=T1.TABLE_NAME) = 1 then 'Table'")
                .AppendLine("	    when (select count(*) from user_views  where  view_name=T1.TABLE_NAME) = 1 then 'View' ")
                .AppendLine("	end as \"TabType\", ")
                .AppendLine("	T1.TABLE_NAME as \"TabName\",")
                .AppendLine("	T1.COLUMN_ID AS \"FieldNo\", ")
                .AppendLine("	T1.COLUMN_NAME AS \"FieldName\",")
                .AppendLine("	case ")
                .AppendLine("	    when t1.CHAR_LENGTH > 0 then t1.CHAR_LENGTH")
                .AppendLine("		when t1.data_precision > 0 then t1.data_precision")
                .AppendLine("	    else 0")
                .AppendLine("	end as \"Lens\",")

                .AppendLine("	case ")
                .AppendLine("	    when t1.CHAR_LENGTH > 0 then t1.CHAR_LENGTH")
                .AppendLine("		when t1.data_precision > 0 then t1.data_precision")
                .AppendLine("	    else 0")
                .AppendLine("	end as \"UseBytes\",")

                .AppendLine("	0 AS \"IsIdentity\",")
                .AppendLine("	case ")
                .AppendLine("	    when (select count(col.column_name)")
                .AppendLine("		    from user_constraints con,  user_cons_columns col")
                .AppendLine("			where con.constraint_name = col.constraint_name")
                .AppendLine("			and con.constraint_type='P'")
                .AppendLine("		    and col.table_name = T1.TABLE_NAME and T1.COLUMN_NAME= col.column_name ) = 1 then 1")
                .AppendLine("	    else 0")
                .AppendLine("	end as \"IsPrimary\",")
                .AppendLine("	case ")
                .AppendLine("	    when T1.DATA_TYPE = 'NUMBER' AND ( t1.data_scale=0 or t1.data_scale is null ) then 'int'")
                .AppendLine("	    when T1.DATA_TYPE = 'NUMBER' AND t1.data_scale>0 then 'decimal'")
                .AppendLine("	    when T1.DATA_TYPE = 'FLOAT' then 'decimal'")
                .AppendLine("	    when T1.DATA_TYPE = 'VARCHAR2' then 'varchar'")
                .AppendLine("	    when T1.DATA_TYPE = 'CHAR' then 'char'")
                .AppendLine("	    when T1.DATA_TYPE = 'NCHAR' then 'nchar'")
                .AppendLine("	    when T1.DATA_TYPE = 'NVARCHAR2' then 'nvarchar'")
                .AppendLine("	    when T1.DATA_TYPE = 'DATE' then 'date'")
                .AppendLine("	    when T1.DATA_TYPE = 'TIMESTAMP' then 'datetime' ")
                 .AppendLine("	    when T1.DATA_TYPE = 'TIMESTAMP(6)' then 'datetime' ")
                .AppendLine("	    when T1.DATA_TYPE = 'LONG' then 'text' ")
                .AppendLine("	    when T1.DATA_TYPE = 'NCLOB' then 'text'")
                .AppendLine("	    when T1.DATA_TYPE = 'CLOB' then 'text' ")
                .AppendLine("	    when T1.DATA_TYPE = 'BLOB' then 'image' ")
                .AppendLine("	    when T1.DATA_TYPE = 'BFILE' then 'image' ")
                .AppendLine("	else 'nvarchar'")
                .AppendLine("	end   AS \"FieldType\",")
                .AppendLine("	case ")
                .AppendLine("	    when t1.data_scale > 0 then  t1.data_scale")
                .AppendLine("	    else 0")
                .AppendLine("	end as \"PointDec\",")
                .AppendLine("	CASE ")
                .AppendLine("	    WHEN T1.NULLABLE='Y' THEN 1")
                .AppendLine("	    ELSE 0")
                .AppendLine("	END AS \"IsNull\",")
                .AppendLine("	t1.DATA_DEFAULT AS \"DbDefault\",")
                .AppendLine("	T2.COMMENTS as \"FieldDesc\"")
                .AppendLine("	FROM USER_TAB_COLS T1, USER_COL_COMMENTS T2")
                .AppendLine("	WHERE T1.TABLE_NAME = T2.TABLE_NAME")
                .AppendLine("	    AND T1.COLUMN_NAME = T2.COLUMN_NAME")
                .AppendLine("	    AND T1.TABLE_NAME =upper('[$TabName$]') ")
                .ToString();



            //批量MERGE更新模版
            _temp_merge_into = new StringBuilder()
                .AppendLine($"MERGE INTO {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after}    a")
                .AppendLine("USING [$Source$]   b")
                .AppendLine("on ( [$OnFilter$] )")
                .AppendLine("WHEN MATCHED THEN")
                .AppendLine("   update set [$Update$]")
                .AppendLine("WHEN NOT MATCHED THEN")
                .AppendLine("   insert ([$Field$]) values([$Values$])")
                .AppendLine(";")
                .ToString();


            //表更新 不带条件 
            _temp_update = $"update {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after} set [$Fields$]";

            //表更新 带条件 
            _temp_update_where = $"update {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after} set [$Fields$] where [$Where$]";

            _temp_delete = $"delete {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after} ";

            _temp_delete_where = $"delete {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after} where [$Where$] ";

            //删除不会留下任何痕迹
            _temp_truncate = $"execute immediate 'TRUNCATE TABLE {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after}';";

            _temp_droptable = $"execute immediate 'drop table {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after}';";

            //获取当前库所有的表
            _temp_gettables = new StringBuilder()
                .AppendLine("select * from ( ")
                 .AppendLine(@"select table_name as ""TabName"", 'Table' AS ""TabType"",  to_char(cast(LAST_ANALYZED as DATE),'yyyy-mm-dd hh24:mi:ss') as ""CreateTime"" from SYS.user_tables where TABLESPACE_NAME ='[$Schema$]' and table_name not like '/%' [$Where$]")

                .AppendLine(") temp")
                .AppendLine("ORDER BY \"TabName\" ASC, \"CreateTime\" desc ")
                .ToString();

            _temp_getviews = new StringBuilder()
             .AppendLine("select * from ( ")
                 .AppendLine(@"select VIEW_NAME as ""TabName"", 'View' AS ""TabType"",  '' as ""CreateTime"" from SYS.user_views where 1=1 [$Where$]  ")

                .AppendLine(") temp")
                .AppendLine("ORDER BY \"TabName\" ASC, \"CreateTime\" desc ")
                .ToString();


            _temp_getalltables = new StringBuilder()
               .AppendLine("select * from ( ")
                .AppendLine(@"select table_name as ""TabName"", 'Table' AS ""TabType"",  to_char(cast(LAST_ANALYZED as DATE),'yyyy-mm-dd hh24:mi:ss') as ""CreateTime"" from SYS.user_tables where TABLESPACE_NAME ='[$Schema$]' and table_name not like '/%' ")
                .AppendLine("union all")
                .AppendLine(@"select VIEW_NAME as ""TabName"", 'View' AS ""TabType"",  '' as ""CreateTime"" from SYS.user_views  ")

               .AppendLine(") temp [$Where$]")
               .AppendLine(@"ORDER BY ""TabName"" ASC, ""CreateTime"" desc ")
               .ToString();

            _temp_check_table_exists =
               new StringBuilder()
              .AppendLine("select * from ( ")
                .AppendLine(@"select table_name as ""TabName"", 'Table' AS ""TabType"",  to_char(cast(LAST_ANALYZED as DATE),'yyyy-mm-dd hh24:mi:ss') as ""CreateTime"" from SYS.user_tables where TABLESPACE_NAME ='[$Schema$]' and table_name not like '/%' ")
                .AppendLine("union all")
                .AppendLine(@"select VIEW_NAME as ""TabName"", 'View' AS ""TabType"",  '' as ""CreateTime"" from SYS.user_views  ")

              .AppendLine(") temp WHERE \"TabName\" = '[$TabName$]'")
              .AppendLine("ORDER BY \"TabName\" ASC, \"CreateTime\" desc ")
              .ToString();

            //删除视图
            _temp_drop_view = $"DROP VIEW  [$Schema$].[$TabName$]"; //千万不要加";"

            //创建视图
            _temp_create_view = new StringBuilder()
                .AppendLine($"CREATE VIEW [$Schema$].[$TabName$] ")
                .AppendLine("   AS  ")
                .AppendLine("[$ViewSql$]")
                .ToString();
            //修改视图
            _temp_modi_view = new StringBuilder()
                .AppendLine(_temp_create_view)
                .ToString();

            //表索引
            _temp_get_tabindex = @"SELECT distinct TABLE_NAME as ""TableName"" , INDEX_NAME as ""IndexName"", 
        case when UNIQUENESS='UNIQUE' AND  CONSTRAINT_INDEX ='YES' then 'Key_Index' ELSE 'Index' end as IndexType 
, '' as ""Disabled""   FROM user_indexes WHERE ""TABLE_NAME"" = '[$TabName$]' ";


            //表索引明细
            _temp_get_indexdetail = @"
select 1 as ""TableId"",  idx.TABLE_NAME as ""TableName"", 1 as IndexId , idx.INDEX_NAME as ""IndexName"",  CASE WHEN UNIQUENESS = 'UNIQUE' AND CONSTRAINT_INDEX = 'YES' then 'Key_Index' ELSE 'Index' end as ""IndexType""
                    , COLUMN_POSITION AS ""ColumnIdx"", COLUMN_POSITION AS ""ColumnID"", COLUMN_NAME as ""ColumnName""
                    , case when DESCEND = 'ASC' then 'asc' ELSE 'desc' end as ""Sort""
                              , case when UNIQUENESS = 'UNIQUE' AND CONSTRAINT_INDEX = 'YES' then 'Y' ELSE 'N' end as  ""IsPrimary"" ,'' as ""IsIncludedColumn""
                                     , case when UNIQUENESS = 'UNIQUE' then 'Y' ELSE '' end as ""IsUnique""   , '' AS ""Ignore_dup_key""  , '' as ""Disabled""  , '' AS ""Fill_factor""  , '' AS ""Padded""
                                     from user_indexes idx join user_ind_columns idxc on idx.index_name = idxc.index_name
where idx.TABLE_NAME = '[$TabName$]' and idxc.INDEX_NAME = '[$IndexName$]' ";


            //创建索引
            _temp_create_index = $@"CREATE INDEX {_temp_schema_pre}[$IndexName$]{_temp_schema_after} ON {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after} ([$Key$]) ";

            //删除索引
            _temp_drop_index = new StringBuilder()
                .AppendLine($@"DROP INDEX  {_temp_schema_pre}[$IndexName$]{_temp_schema_after}")
                .ToString();

        }
    }
}
