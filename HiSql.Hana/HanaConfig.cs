using System;
using System.Collections.Generic;
using System.Text;

namespace HiSql
{

    public class HanaConfig : IDbConfig
    {
        /// <summary>
        /// 数据包大小 经过测试200这个值是比较合理
        /// </summary>
        int _bluksize = 200;
        int _bulkunitsize = 250000;
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

        //本地临时表
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



        string _temp_addcolumn = "alter table [$TabName$] add ([$TempColumn$]) ";//ALTER TABLE t ADD (c NVARCHAR(10) DEFAULT 'NCHAR');

        string _temp_delcolumn = "";

        string _temp_modicolumn = "alter table [$TabName$] alter ([$TempColumn$]);";

        string _temp_recolumn = "RENAME COLUMN [$TabName$].[$FieldName$] TO [$ReFieldName$];";

        string _temp_retable = "RENAME TABLE [$TabName$] to [$ReTabName$]";

        string _temp_setdefalut = "";

        string _temp_deldefalut = "";

        /// <summary>
        /// 所有物理实体表
        /// </summary>
        string _temp_gettables = "";
        /// <summary>
        /// 分页获取表和视图
        /// </summary>
        string _temp_gettables_paging = "";
        string _temp_gettables_pagingcount = "";
        

        string _temp_getTableDataCount = "select count(*) from [$TabName$] ";
        /// <summary>
        /// 获取所有视图
        /// </summary>
        string _temp_getviews = "";

        string _temp_getviews_paging = "";

        string _temp_getviews_pagingcount = "";


        /// <summary>
        /// 获取表和视图
        /// </summary>
        string _temp_getalltables = "";
        string _temp_getalltables_paging = "";

        string _temp_getalltables_pagingcount = "";

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

        string _temp_tabel_primarykey_create = "";

        string _temp_tabel_primarykey_drop = "";

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

                return HiSql.Hana.Properties.Resources.HiSql.ToString();
            }
        }

        public List<DefMapping> DbDefMapping {
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
        /// <summary>
        /// 全局临时表
        /// </summary>
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

        
        public string Drop_Table { get => _temp_droptable; }

        public Dictionary<string, string> FieldTempMapping => _fieldtempmapping;

        public Dictionary<HiType, string> DbMapping => _dbmapping;

        public string Table_MergeInto { get => _temp_merge_into; }



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
        public string Get_TablesPaging { get => _temp_gettables_paging; }
        public string Get_TablesPagingCount { get => _temp_gettables_pagingcount; }
        
        public string Get_TableDataCount { get => _temp_getTableDataCount; }
        public string Get_Views { get => _temp_getviews; }
        public string Get_ViewsPaging { get => _temp_getviews_paging; }

        public string Get_ViewsPagingCount { get => _temp_getviews_pagingcount; }

        
        public string Get_AllTables { get => _temp_getalltables; }
        public string Get_AllTablesPaging { get => _temp_getalltables_paging; }

        public string Get_AllTablesPagingCount { get => _temp_getalltables_pagingcount; }

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
        public string Table_PrimaryKeyCreate { get => _temp_tabel_primarykey_create; }
        public string Table_PrimaryKeyDrop { get => _temp_tabel_primarykey_drop; }
        public HanaConfig()
        {


        }
        public HanaConfig(bool init)
        {

            Init();
        }
        public void Init()
        {

            _temp_fun_date = "CURRENT_TIMESTAMP";
            _lstdefmapping.Add(new DefMapping { IsRegex = false, DbValue = @"", DBDefault = HiTypeDBDefault.NONE });
            //数字默认
            _lstdefmapping.Add(new DefMapping { IsRegex = true, DbValue = @"^(?<value>[-]?\d+(?:[\.]?)[\d]*)$", DbType = HiTypeGroup.Number, DBDefault = HiTypeDBDefault.VALUE });

            //true or false
            _lstdefmapping.Add(new DefMapping { IsRegex = true, DbValue = @"^(?<value>[01]{1})$", DbType = HiTypeGroup.Bool, DBDefault = HiTypeDBDefault.VALUE });

            //指定字符默认值 如('hisql') 或('')
            _lstdefmapping.Add(new DefMapping { IsRegex = true, DbValue = $@"^(?<value>(?<!{_temp_fun_date})\w*)$", DbType = HiTypeGroup.Char, DBDefault = HiTypeDBDefault.VALUE });

            //日期
            _lstdefmapping.Add(new DefMapping { IsRegex = true, DbValue = $@"^(?<value>{_temp_fun_date})\s*$", DbType = HiTypeGroup.Date, DBDefault = HiTypeDBDefault.FUNDATE });

            //md5值
            //_lstdefmapping.Add(new DefMapping { IsRegex = true, DbValue = @"^\((?<value>newid\(\))\)$", DBDefault = HiTypeDBDefault.FUNGUID });
            _dbmapping = new Dictionary<HiType, string> {
                { HiType.NVARCHAR,"nvarchar" },
                { HiType.VARCHAR,"varchar" },
                { HiType.NCHAR,"nchar" },
                { HiType.CHAR,"char" },
                { HiType.TEXT,"text" },

                { HiType.INT,"integer" },
                { HiType.BIGINT,"bigint" },
                { HiType.SMALLINT,"smallint" },
                { HiType.DECIMAL,"decimal" },

                { HiType.BOOL,"boolean" },

                { HiType.DATETIME,"timestamp" },
                { HiType.DATE,"date" },

                { HiType.BINARY,"binary" },
                { HiType.GUID,"varchar" },
            };

            _fieldtempmapping = new Dictionary<string, string> {
                //样例：[TabName] [varchar](50) NOT NULL,
                { "nvarchar",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} nvarchar([$FieldLen$]) [$IsNull$]  [$Default$] [$COMMENT$] [$EXTEND$]{_temp_field_split} "},
                { "varchar",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} varchar([$FieldLen$]) [$IsNull$] [$Default$] [$COMMENT$] [$EXTEND$]{_temp_field_split}"},
                { "nchar",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} nchar([$FieldLen$]) [$IsNull$]  [$Default$] [$COMMENT$] [$EXTEND$]{_temp_field_split} "},
                { "char",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} char([$FieldLen$]) [$IsNull$] [$Default$] [$EXTEND$]{_temp_field_split}"},
               
                //样例：[udescript] [text] NULL,
                { "text",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} text [$IsNull$] [$Default$] [$COMMENT$] [$EXTEND$]{_temp_field_split}"},

                { "integer",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} integer [$IsIdentity$] [$IsNull$] [$Default$] [$COMMENT$] [$EXTEND$]{_temp_field_split}"},
                { "bigint",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} bigint [$IsIdentity$] [$IsNull$] [$Default$] [$COMMENT$] [$EXTEND$]{_temp_field_split}" },
                { "smallint",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} smallint  [$IsNull$] [$Default$] [$COMMENT$] [$EXTEND$]{_temp_field_split}"},
                { "decimal",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} decimal([$FieldLen$],[$FieldDec$])  [$IsNull$] [$Default$] [$COMMENT$] [$EXTEND$]{_temp_field_split}"},

                { "boolean",$"{_temp_field_pre}[$FieldName$]{_temp_field_after}  boolean   [$IsNull$] [$Default$] [$COMMENT$] [$EXTEND$]{_temp_field_split}"},

                { "timestamp",$"{_temp_field_pre}[$FieldName$]{_temp_field_after}  timestamp   [$IsNull$] [$Default$] [$COMMENT$] [$EXTEND$]{_temp_field_split}"},
                { "date",$"{_temp_field_pre}[$FieldName$]{_temp_field_after}  date    [$IsNull$] [$Default$] [$COMMENT$] [$EXTEND$]{_temp_field_split}" },

                { "binary",$"{_temp_field_pre}[$FieldName$]{_temp_field_after}  binary   [$IsNull$] [$COMMENT$] [$EXTEND$]{_temp_field_split}"},
                { "uniqueidentifier",$"{_temp_field_pre}[$FieldName$]{_temp_field_after}  varchar(36)   [$IsNull$] [$Default$] [$COMMENT$] [$EXTEND$]{_temp_field_split}"},
            };


            _temp_create_table = new StringBuilder()
                //样例：CREATE TABLE [dbo].[H_TEST_USER]
                .AppendLine("do begin")
                .AppendLine($"  DECLARE _count int:=0;")
                .AppendLine($"  DECLARE _status int:=0;")
                .AppendLine("   select count(*) into _count  from OBJECTS  where ( OBJECT_TYPE='TABLE' or OBJECT_TYPE='TABLE' ) AND OBJECT_NAME='[$TabName$]';")
                .AppendLine("   if :_count = 0 then")
                .AppendLine($"      CREATE COLUMN TABLE  {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after} (")
                .AppendLine("           [$Fields$]")
                .AppendLine("           [$Keys$]")//主键
                .AppendLine("       )[$Primary$]")
                .AppendLine($"      delete FROM  {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}{Constants.HiSysTable["Hi_TabModel"].ToString()}{_temp_table_after} where {_temp_field_pre}TabName{_temp_field_after}='[$TabName$]';")
                .AppendLine($"      delete FROM  {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}{Constants.HiSysTable["Hi_FieldModel"].ToString()}{_temp_table_after} where {_temp_field_pre}TabName{_temp_field_after}='[$TabName$]';")

                .AppendLine("       [$TabStruct$]")
                .AppendLine("       _status := :_status + 1;")

                .AppendLine("   end if;")
  
                //.AppendLine($"  select :_status as {_temp_field_pre}status{_temp_field_after} from dummy;")
                .AppendLine("end;")
                .ToString();

            ///全局临时表
            _temp_create_temp_global_table = new StringBuilder()
                //样例：CREATE TABLE [dbo].[H_TEST_USER]
                //.AppendLine("do begin")
                .AppendLine("   DECLARE _count int:=0;")
                .AppendLine("   DECLARE _status int:=0;")
                .AppendLine("   select count(*) into _count from M_TEMPORARY_TABLES where schema_name ='[$Schema$]' and table_name='[$TabName$]'  ;")
                .AppendLine("   if :_count = 0 then")
                .AppendLine($"       CREATE global TEMPORARY TABLE  {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after} (")
                .AppendLine("           [$Fields$]")
                .AppendLine("           [$Keys$]")//主键
                .AppendLine("       )[$Primary$]")
                .AppendLine("       _status := :_status + 1;")

                .AppendLine("   end if;")

                //.AppendLine($"  select :_status as {_temp_field_pre}status{_temp_field_after} from dummy;")
                //.AppendLine("end;")
                .ToString();

            ///全局临时表
            _temp_create_temp_global_table_drop = new StringBuilder()
                //样例：CREATE TABLE [dbo].[H_TEST_USER]
                //.AppendLine("do begin")
                .AppendLine("   DECLARE _count int:=0;")
                .AppendLine("   DECLARE _status int:=0;")
                .AppendLine("   select count(*) into _count from M_TEMPORARY_TABLES where schema_name ='[$Schema$]' and table_name='[$TabName$]'  ;")
                .AppendLine("   if :_count > 0 then")
                .AppendLine($"       drop table {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after};")
                .AppendLine("   end if;")

                
                .AppendLine($"  CREATE global TEMPORARY TABLE  {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after} (")
                .AppendLine("           [$Fields$]")
                .AppendLine("           [$Keys$]")//主键
                .AppendLine("   )[$Primary$]")
                .AppendLine("   _status := :_status + 1;")

                //.AppendLine($"  select :_status as {_temp_field_pre}status{_temp_field_after} from dummy;")
                //.AppendLine("end;")
                .ToString();


            ///如果不存在该临时表就创建 如果已经存在则不创建
            _temp_create_temp_local_table = new StringBuilder()
                //样例：CREATE TABLE [dbo].[H_TEST_USER]

                //.AppendLine("do begin")
                .AppendLine("   DECLARE _count int:=0;")
                .AppendLine("   DECLARE _status int:=0;")
                .AppendLine("   select count(*) into _count from M_TEMPORARY_TABLES where schema_name ='[$Schema$]' and table_name='[$TabName$]' ;")
                .AppendLine("   if :_count == 0 then")
                .AppendLine($"      CREATE local TEMPORARY TABLE  {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after} (")
                .AppendLine("           [$Fields$]")
                .AppendLine("           [$Keys$]")//主键
                .AppendLine("       )[$Primary$]")
                .AppendLine("       _status := :_status + 1;")
                .AppendLine("   else")
                .AppendLine($"       DELETE  FROM  {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after};")
                .AppendLine("   end if;")
                

                //.AppendLine($"  select :_status as {_temp_field_pre}status{_temp_field_after} from dummy;")
                //.AppendLine("end;")

                .ToString();


            _temp_create_temp_local_table_drop = new StringBuilder()
                //样例：CREATE TABLE [dbo].[H_TEST_USER]

                //.AppendLine("do begin")
                .AppendLine("   DECLARE _count int:=0;")
                .AppendLine("   DECLARE _status int:=0;")
                .AppendLine("   select count(*) into _count from M_TEMPORARY_TABLES where schema_name ='[$Schema$]' and table_name='[$TabName$]' ;")
                .AppendLine("   if :_count > 0 then")
                .AppendLine($"       drop table   {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after};")
                .AppendLine("   end if;")
                .AppendLine($"  CREATE local TEMPORARY TABLE  {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after} (")
                .AppendLine("           [$Fields$]")
                .AppendLine("           [$Keys$]")//主键
                .AppendLine("   )[$Primary$]")
                .AppendLine("   _status := :_status + 1;")

                //.AppendLine($"  select :_status as {_temp_field_pre}status{_temp_field_after} from dummy;")
                //.AppendLine("end;")

                .ToString();

            _temp_delcolumn = $"alter table {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_schema_after} drop({_temp_schema_pre}[$FieldName$]{_temp_schema_after});";

            //变量表模版
            _temp_declare_table = new StringBuilder()
                .AppendLine($"declare    [$TabName$]  TABLE(")
                .AppendLine("[$Fields$]")
                .AppendLine("[$Keys$]")//主键
                .AppendLine(")[$Primary$]")
                .ToString();

            //表创建时的KEY模版
            _temp_tabel_key = new StringBuilder()
                //.AppendLine($"CONSTRAINT {_temp_table_pre}PK_[$TabName$]_[$ConnectID$]{_temp_table_after} PRIMARY KEY CLUSTERED")
                .AppendLine("PRIMARY KEY([$Keys$])")
                .ToString();
            _temp_table_key2 = "[$FieldName$] ";//定义主键的排序方式

            _temp_table_key3 = "UNLOAD PRIORITY 5 AUTO MERGE;  ";//TEXTIMAGE_ON [PRIMARY]


            _temp_field_comment = new StringBuilder()
                //.AppendLine("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'[$FieldDesc$]' , @level0type=N'SCHEMA',@level0name=N'[$Schema$]', @level1type=N'TABLE',@level1name=N'[$TabName$]', @level2type=N'COLUMN',@level2name=N'[$FieldName$]'")
                // .AppendLine("GO")
                .ToString();

           

            _temp_insert_statement = new StringBuilder()
                .Append($"insert into {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after}([$FIELDS$]) values([$VALUES$])")
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
                .Append($"insert into {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after}([$FIELDS$]) select  [$VALUES$] from dummy")
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

            //此处一定要记得要结合IDataReader 的GetSchemaTable 数据才能找到该表的主键信息
            _temp_get_table_schema = new StringBuilder()
                .AppendLine("SELECT b.\"OBJECT_TYPE\" AS \"TabType\", a.\"TABLE_NAME\" AS \"TabName\" ,a.\"POSITION\" as \"FieldNo\",A.\"COLUMN_NAME\" AS \"FieldName\",FALSE AS  \"IsIdentity\",FALSE AS \"IsPrimary\",")
                .AppendLine("   case ")
                .AppendLine("       when a.\"DATA_TYPE_NAME\" = 'NVARCHAR' then 'nvarchar'")
                .AppendLine("       when a.\"DATA_TYPE_NAME\" = 'VARCHAR' then 'varchar'")
                .AppendLine("       when a.\"DATA_TYPE_NAME\" = 'NCHAR' then 'nchar'")
                .AppendLine("       when a.\"DATA_TYPE_NAME\" = 'CHAR' then 'char'")
                .AppendLine("       when a.\"DATA_TYPE_NAME\" = 'TEXT' then 'text'")
                .AppendLine("       when a.\"DATA_TYPE_NAME\" = 'INTEGER' then 'integer'")
                .AppendLine("       when a.\"DATA_TYPE_NAME\" = 'BIGINT' then 'bigint'")
                .AppendLine("       when a.\"DATA_TYPE_NAME\" = 'SMALLINT' then 'smallint'")
                .AppendLine("       when a.\"DATA_TYPE_NAME\" = 'TINYINT' then 'integer'")
                .AppendLine("       when a.\"DATA_TYPE_NAME\" = 'DECIMAL' then 'decimal'")
                .AppendLine("       when a.\"DATA_TYPE_NAME\" = 'FLOAT' then 'decimal'")
                .AppendLine("       when a.\"DATA_TYPE_NAME\" = 'SMALLDECIMAL' then 'decimal'")
                .AppendLine("       when a.\"DATA_TYPE_NAME\" = 'BOOLEAN' then 'boolean'")
                .AppendLine("       when a.\"DATA_TYPE_NAME\" = 'DATE' then 'date'  ")
                .AppendLine("       when a.\"DATA_TYPE_NAME\" = 'SECONDDATE' then 'timestamp'")
                .AppendLine("       when a.\"DATA_TYPE_NAME\" = 'TIMESTAMP' then 'timestamp'")
                .AppendLine("       when a.\"DATA_TYPE_NAME\" = 'BINARY' then 'binary'")
                .AppendLine("       when a.\"DATA_TYPE_NAME\" = 'VARBINARY' then 'binary'")
                .AppendLine("       ELSE 'nvarchar'")
                .AppendLine("   end   AS \"FieldType\",")
                .AppendLine("a.\"LENGTH\" * 2 AS \"UseBytes\",a.\"LENGTH\" AS \"Lens\",a.\"SCALE\" AS \"PointDec\",a.\"IS_NULLABLE\"  as \"IsNull\",")
                .AppendLine("a.\"DEFAULT_VALUE\" as \"DbDefault\",a.\"COMMENTS\" as \"FieldDesc\"")
                .AppendLine(" FROM SYS.TABLE_COLUMNS as a")
                .AppendLine("  INNER JOIN \"SYS\".\"OBJECTS\" AS b on a.\"TABLE_NAME\" = b.\"OBJECT_NAME\" AND b.\"OBJECT_TYPE\" in ('VIEW','TABLE')")
                .AppendLine("  WHERE TABLE_NAME = '[$TabName$]' ORDER BY POSITION;")
                .ToString();



            //批量MERGE更新模版
            _temp_merge_into = new StringBuilder()
                .AppendLine($"MERGE INTO {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after}  as a")
                .AppendLine($"USING {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$Source$]{_temp_table_after} as b")
                .AppendLine("on [$OnFilter$] ")
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

            _temp_delete = $"DELETE  FROM {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after}";

            _temp_delete_where = $"DELETE  FROM {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after} where [$Where$]";

            //删除不会留下任何痕迹
            _temp_truncate = $"TRUNCATE TABLE {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after};";

            _temp_droptable = $"DROP TABLE {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after};";


            //获取当前库所有的表
            _temp_gettables = new StringBuilder()
                .AppendLine("select * from ( ")
                 .AppendLine("select \"TABLE_NAME\"  as \"TabName\", 'Table' AS \"TabType\", \"CREATE_TIME\" as \"CreateTime\" FROM \"SYS\".\"TABLES\" where \"SCHEMA_NAME\" = '[$Schema$]' and \"TABLE_NAME\" not like '/%' [$Where$]")
               
                .AppendLine(")as temp")
                .AppendLine("ORDER BY \"TabName\" ASC, \"CreateTime\" desc ")
                .ToString();
            _temp_gettables_pagingcount = "select count(*)   from \"SYS\".\"TABLES\" where \"SCHEMA_NAME\" = '[$Schema$]' and \"TABLE_NAME\" not like '/%'  [$Where$] ;";
            
            _temp_gettables_paging = new StringBuilder()
               .AppendLine("select * from ( ")
                 .AppendLine("select ROW_NUMBER()OVER( order by \"TABLE_NAME\" ASC, \"CREATE_TIME\" DESC  ) as row_seq ,\"TABLE_NAME\"  as \"TabName\", 'Table' AS \"TabType\", \"CREATE_TIME\" as \"CreateTime\" FROM \"SYS\".\"TABLES\" where \"SCHEMA_NAME\" = '[$Schema$]' and \"TABLE_NAME\" not like '/%' ")
                .AppendLine("[$Where$] )as temp  WHERE row_seq > [$SeqBegin$] AND row_seq <=[$SeqEnd$] ")
                .AppendLine("order by row_seq").ToString();

            _temp_getviews = new StringBuilder()
               .AppendLine("select * from ( ")
                .AppendLine("select \"VIEW_NAME\"  as \"TabName\", 'View' AS \"TabType\", \"CREATE_TIME\" as \"CreateTime\" FROM \"SYS\".\"VIEWS\" where \"SCHEMA_NAME\" = '[$Schema$]' and \"VIEW_NAME\" not like '/%' [$Where$]")
               .AppendLine(")as temp")
               .AppendLine("ORDER BY \"TabName\" ASC, \"CreateTime\" desc ")
               .ToString();
            _temp_getviews_pagingcount = "select count(*)   from \"SYS\".\"VIEWS\" where \"SCHEMA_NAME\" = '[$Schema$]' and \"VIEW_NAME\" not like '/%' [$Where$] ;";

            _temp_getviews_paging = new StringBuilder()
               .AppendLine("select * from ( ")
                 .AppendLine("select ROW_NUMBER()OVER( order by \"VIEW_NAME\" ASC, \"CREATE_TIME\" DESC  ) as row_seq ,\"VIEW_NAME\"  as \"TabName\", 'View' AS \"TabType\", \"CREATE_TIME\" as \"CreateTime\" FROM \"SYS\".\"VIEWS\" where \"SCHEMA_NAME\" = '[$Schema$]' and \"VIEW_NAME\" not like '/%'  ")
                .AppendLine("[$Where$] )as temp  WHERE row_seq > [$SeqBegin$] AND row_seq <=[$SeqEnd$] ")
                .AppendLine("order by row_seq").ToString();


            _temp_getalltables = new StringBuilder()
               .AppendLine("select * from ( ")
                .AppendLine("select \"TABLE_NAME\"  as \"TabName\", 'Table' AS \"TabType\", \"CREATE_TIME\" as \"CreateTime\" FROM \"SYS\".\"TABLES\" where \"SCHEMA_NAME\" = '[$Schema$]' and \"TABLE_NAME\" not like '/%' ")
                .AppendLine("union all")
                .AppendLine("select \"VIEW_NAME\"  as \"TabName\", 'View' AS \"TabType\", \"CREATE_TIME\" as \"CreateTime\" FROM \"SYS\".\"VIEWS\" where \"SCHEMA_NAME\" = '[$Schema$]' and \"VIEW_NAME\" not like '/%' ")
              
               .AppendLine(")as temp [$Where$]")
               .AppendLine("ORDER BY \"TabName\" ASC, \"CreateTime\" desc ")
               .ToString();

            _temp_getalltables_pagingcount = new StringBuilder()
               .AppendLine("select count(*)  from ( ")
                .AppendLine("select \"TABLE_NAME\"  as \"TabName\", 'Table' AS \"TabType\", \"CREATE_TIME\" as \"CreateTime\" FROM \"SYS\".\"TABLES\" where \"SCHEMA_NAME\" = '[$Schema$]' and \"TABLE_NAME\" not like '/%' ")
                .AppendLine("union all")
                .AppendLine("select \"VIEW_NAME\"  as \"TabName\", 'View' AS \"TabType\", \"CREATE_TIME\" as \"CreateTime\" FROM \"SYS\".\"VIEWS\" where \"SCHEMA_NAME\" = '[$Schema$]' and \"VIEW_NAME\" not like '/%' ")

               .AppendLine(")as temp  where 1=1 [$Where$]")
               .ToString();

            _temp_getalltables_paging = new StringBuilder()
               .AppendLine("select * from ( select ROW_NUMBER()OVER( order by \"TabType\" ASC,\"TabName\" ASC, \"CreateTime\" DESC  ) as row_seq ,* from ( ")
                .AppendLine("select \"TABLE_NAME\"  as \"TabName\", 'Table' AS \"TabType\", \"CREATE_TIME\" as \"CreateTime\" FROM \"SYS\".\"TABLES\" where \"SCHEMA_NAME\" = '[$Schema$]' and \"TABLE_NAME\" not like '/%' ")
                .AppendLine("union all")
                .AppendLine("select \"VIEW_NAME\"  as \"TabName\", 'View' AS \"TabType\", \"CREATE_TIME\" as \"CreateTime\" FROM \"SYS\".\"VIEWS\" where \"SCHEMA_NAME\" = '[$Schema$]' and \"VIEW_NAME\" not like '/%' ")

               .AppendLine(")as temp where 1=1 [$Where$] ) as t WHERE row_seq > [$SeqBegin$] AND row_seq <=[$SeqEnd$] ")
               .AppendLine("ORDER BY row_seq ")
               .ToString();

            _temp_check_table_exists =
                new StringBuilder()
               .AppendLine("select * from ( ")
                .AppendLine("select \"TABLE_NAME\"  as \"TabName\", 'Table' AS \"TabType\", \"CREATE_TIME\" as \"CreateTime\" FROM \"SYS\".\"TABLES\" where \"SCHEMA_NAME\" = '[$Schema$]' and \"TABLE_NAME\" not like '/%' ")
                .AppendLine("union all")
                .AppendLine("select \"VIEW_NAME\"  as \"TabName\", 'View' AS \"TabType\", \"CREATE_TIME\" as \"CreateTime\" FROM \"SYS\".\"VIEWS\" where \"SCHEMA_NAME\" = '[$Schema$]' and \"VIEW_NAME\" not like '/%' ")

               .AppendLine(")as temp WHERE \"TabName\" = '[$TabName$]'")
               .AppendLine("ORDER BY \"TabName\" ASC, \"CreateTime\" desc ")
               .ToString();

            //删除视图
            _temp_drop_view = $"DROP VIEW  [$Schema$].[$TabName$];";

            //创建视图
            _temp_create_view = new StringBuilder()
                .AppendLine($"CREATE VIEW [$Schema$].[$TabName$] ")
                .AppendLine("AS")
                .AppendLine("[$ViewSql$];")
                .ToString();
            //修改视图
            _temp_modi_view = new StringBuilder()
                .AppendLine(_temp_drop_view)
                .AppendLine(_temp_create_view)
                .ToString();



            //表索引
            _temp_get_tabindex = "SELECT distinct \"TABLE_NAME\" as TableName , \"INDEX_NAME\" as IndexName,  case when \"CONSTRAINT\"='PRIMARY KEY' then 'Key_Index' ELSE 'Index' end as IndexType , '' as Disabled   FROM \"SYS\".\"INDEXES\" WHERE  \"SCHEMA_NAME\" = '[$Schema$]' and \"TABLE_NAME\" = '[$TabName$]'; ";


            //表索引明细
            _temp_get_indexdetail = "select 1 as TableId,  \"TABLE_NAME\" as TableName, 1 as IndexId , \"INDEX_NAME\" as IndexName,  CASE WHEN \"CONSTRAINT\"='PRIMARY KEY' THEN 'KEY_INDEX' ELSE 'INDEX' END AS INDEXTYPE"
                + "    , \"POSITION\" AS ColumnIdx, \"POSITION\" AS ColumnID, \"COLUMN_NAME\" as ColumnName, case when \"ASCENDING_ORDER\" ='TRUE' then 'asc' ELSE 'desc' end as Sort"
                            + "             , case when \"CONSTRAINT\"='PRIMARY KEY' then 'Y' ELSE 'N' end as  IPrimary ,'' as IsIncludedColumn"
                             + "            , case when \"CONSTRAINT\" ='NOT NULL UNIQUE' then 'Y' ELSE '' end as IsUnique   , '' AS Ignore_dup_key   , '' as Disabled  , '' AS Fill_factor  , '' AS Padded"


                             + " from   \"SYS\".\"INDEX_COLUMNS\" WHERE  \"SCHEMA_NAME\" = '[$TabName$]' AND \"TABLE_NAME\" = '[$TabName$]' AND \"INDEX_NAME\"='[$IndexName$]';";
            //创建索引
            _temp_create_index = $@"CREATE INDEX {_temp_schema_pre}[$IndexName$]{_temp_schema_after} ON {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after} ([$Key$]); ";

            //删除索引
            _temp_drop_index = new StringBuilder()
                .AppendLine($@"DROP INDEX  {_temp_schema_pre}[$IndexName$]{_temp_schema_after}")
                .ToString();

            _temp_tabel_primarykey_drop = $"ALTER TABLE {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after} DROP CONSTRAINT {_temp_table_pre}[$IndexName$]{_temp_table_after}";

            _temp_tabel_primarykey_create = $@"ALTER TABLE {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after} add constraint {_temp_table_pre}PK_[$TabName$]_[$ConnectID$]{_temp_table_after} [$Primary$] ";
            

        }
    }
}
