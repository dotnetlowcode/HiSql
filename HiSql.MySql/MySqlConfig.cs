using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public class MySqlConfig : IDbConfig
    {
        /// <summary>
        /// 数据包大小 经过测试200这个值是比较合理
        /// </summary>
        int _bluksize = 200;
        int _bulkunitsize = 250000;
        string _temp_schema_pre = "`";
        string _temp_schema_after = "`";

        string _temp_table_pre = "`";
        string _temp_table_after = "`";

        string _temp_field_pre = "`";
        string _temp_field_after = "`";



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


        string _temp_addcolumn = "alter table [$TabName$] add [$TempColumn$] ;";

        string _temp_delcolumn = "alter table [$TabName$] drop column [$FieldName$] ;";

        string _temp_modicolumn = "alter table [$TabName$] MODIFY column [$TempColumn$] ;";

        string _temp_recolumn = "alter table [$TabName$] CHANGE column [$TempColumn$] ;";
        string _temp_retable = "ALTER TABLE [$TabName$]  rename to [$ReTabName$] ;";

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

        public List<DefMapping> DbDefMapping
        {
            get => _lstdefmapping;
        }
        public string Fun_CurrDATE { get => _temp_fun_date; }
        public int BlukSize { get => _bluksize; set => _bluksize = value; }
        public int BulkUnitSize { get => _bulkunitsize; set => _bulkunitsize = value; }
        public string Schema_Pre { get => _temp_schema_pre; }
        public string Schema_After { get => _temp_schema_after; }
        public string Table_Pre { get => _temp_table_pre; }
        public string Table_After { get => _temp_table_after; }
        public string Field_Pre { get => _temp_field_pre; }
        public string Field_After { get => _temp_field_after; }
        public string Table_Create { get => _temp_create_table; }

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

        /// <summary>
        /// 安装HiSql初始化
        /// </summary>
        public string InitSql
        {
            get
            { //return _temp_install;

                return HiSql.MySql.Properties.Resources.HiSql.ToString();
            }
        }

        public MySqlConfig()
        {


        }
        public MySqlConfig(bool init)
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
            _lstdefmapping.Add(new DefMapping { IsRegex = true, DbValue = @"^b\'(?<value>[01]{1})\'", DbType = HiTypeGroup.Bool, DBDefault = HiTypeDBDefault.VALUE });

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
                { "nvarchar",$"{_temp_field_pre}[$FieldName$]{_temp_field_after}  varchar([$FieldLen$]) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci [$IsNull$]  [$Default$] [$COMMENT$] [$EXTEND$]{_temp_field_split} "},
                { "varchar",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} varchar([$FieldLen$]) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci [$IsNull$] [$Default$] [$COMMENT$] [$EXTEND$]{_temp_field_split}"},
                { "nchar",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} char([$FieldLen$]) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci [$IsNull$] [$Default$] [$COMMENT$] [$EXTEND$]{_temp_field_split} "},
                { "char",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} char([$FieldLen$]) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci [$IsNull$] [$Default$] [$COMMENT$] [$EXTEND$]{_temp_field_split}"},
                //样例：[udescript] [text] NULL,
                { "text",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci [$IsNull$] [$Default$] [$COMMENT$] [$EXTEND$]{_temp_field_split}"},

                { "int",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} int [$IsIdentity$] [$IsNull$] [$Default$] [$COMMENT$] [$EXTEND$]{_temp_field_split}"},
                { "bigint",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} bigint [$IsIdentity$] [$IsNull$] [$Default$] [$COMMENT$] [$EXTEND$]{_temp_field_split}" },
                { "smallint",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} smallint [$IsNull$] [$Default$] [$COMMENT$] [$EXTEND$]{_temp_field_split}"},
                { "decimal",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} decimal([$FieldLen$],[$FieldDec$])  [$IsNull$] [$Default$] [$COMMENT$] [$EXTEND$]{_temp_field_split}"},

                { "bit",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} bit  [$IsNull$] [$Default$] [$COMMENT$] [$EXTEND$]{_temp_field_split}"},

                { "datetime",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} datetime  [$IsNull$] [$Default$] [$COMMENT$] [$EXTEND$]{_temp_field_split}"},
                { "date",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} date  [$IsNull$] [$Default$] [$COMMENT$] [$EXTEND$]{_temp_field_split}" },

                { "binary",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} binary  [$IsNull$] [$COMMENT$] [$EXTEND$]{_temp_field_split}"},
                { "uniqueidentifier",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci   [$IsNull$] [$Default$] [$COMMENT$] [$EXTEND$]{_temp_field_split}"},
            };


            _temp_create_table = new StringBuilder()
                //样例：CREATE TABLE [dbo].[H_TEST_USER]
              
                .AppendLine($"CREATE TABLE  IF NOT EXISTS {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after} (")
                .AppendLine("[$Fields$]")
                .AppendLine("[$Keys$]")//主键
                .AppendLine(")[$Primary$]")
                .AppendLine($"delete from  {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}{Constants.HiSysTable["Hi_TabModel"].ToString()}{_temp_table_after} where TabName='[$TabName$]';")
                .AppendLine($"delete from  {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}{Constants.HiSysTable["Hi_FieldModel"].ToString()}{_temp_table_after} where TabName='[$TabName$]';")

                .AppendLine("[$TabStruct$]")

                //.AppendLine("set @_effect=1; ")
                //.AppendLine("select @_effect;")
                .AppendLine("select 1;")
                .ToString();

            _temp_create_temp_global_table = new StringBuilder()
                //样例：CREATE TABLE [dbo].[H_TEST_USER]
                .AppendLine($"CREATE TABLE  IF NOT EXISTS {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after} (")
                .AppendLine("[$Fields$]")
                .AppendLine("[$Keys$]")//主键
                .AppendLine(")[$Primary$]")
                //.AppendLine("set @_effect=1; ")
                //.AppendLine("select @_effect;")
                .AppendLine("select 1;")
                .ToString();

            _temp_create_temp_global_table_drop = new StringBuilder()
                //样例：CREATE TABLE [dbo].[H_TEST_USER]
                .AppendLine($"drop table if exists {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after};")
                .AppendLine($"CREATE TABLE  IF NOT EXISTS {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after} (")
                .AppendLine("[$Fields$]")
                .AppendLine("[$Keys$]")//主键
                .AppendLine(")[$Primary$]")
                //.AppendLine("set @_effect=1; ")
                //.AppendLine("select @_effect;")
                .AppendLine("select 1;")
                .ToString();


            _temp_create_temp_local_table = new StringBuilder()
                //样例：CREATE TABLE [dbo].[H_TEST_USER]
                .AppendLine($"CREATE temporary  TABLE  IF NOT EXISTS {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after} (")
                .AppendLine("[$Fields$]")
                .AppendLine("[$Keys$]")//主键
                .AppendLine(")[$Primary$]")
                //.AppendLine("set @_effect=1; ")
                .AppendLine($"delete from {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after} ; ")
                //.AppendLine("select @_effect;")
                .AppendLine("select 1;")
                .ToString();

            _temp_create_temp_local_table_drop = new StringBuilder()
                //样例：CREATE TABLE [dbo].[H_TEST_USER]
                .AppendLine($"drop table if exists {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after};")
                .AppendLine($"CREATE temporary  TABLE  IF NOT EXISTS {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after} (")
                .AppendLine("[$Fields$]")
                .AppendLine("[$Keys$]")//主键
                .AppendLine(")[$Primary$]")
                //.AppendLine("set @_effect=1; ")
                //.AppendLine("select @_effect;")
                .AppendLine("select 1;")
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
                .AppendLine(" PRIMARY KEY(")
                .AppendLine("[$Keys$]")
                .AppendLine(")USING BTREE")
                .ToString();
            _temp_table_key2 = "[$FieldName$] ASC";//定义主键的排序方式

            _temp_table_key3 = "ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;";//TEXTIMAGE_ON [PRIMARY]


            _temp_field_comment = new StringBuilder()
                .AppendLine("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'[$FieldDesc$]' , @level0type=N'SCHEMA',@level0name=N'[$Schema$]', @level1type=N'TABLE',@level1name=N'[$TabName$]', @level2type=N'COLUMN',@level2name=N'[$FieldName$]'")
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
                .AppendLine("	case b.table_type")
                .AppendLine("	    when 'BASE TABLE' then 'Table'")
                .AppendLine("	    when 'SYSTEM VIEW' then 'View'")
                .AppendLine("	    else 'NONE'")
                .AppendLine("	END `TabType`,")
                .AppendLine("   a.TABLE_NAME as `TabName`,")
                .AppendLine("   a.COLUMN_NAME as `FieldName`,")
                .AppendLine("   a.ORDINAL_POSITiON AS `FieldNo`,")
                .AppendLine("	case a.extra")
                .AppendLine("		when 'auto_increment' then true")
                .AppendLine("		else false")
                .AppendLine("	end `IsIdentity`,")
                .AppendLine("	case a.column_key")
                .AppendLine("		when 'PRI' then true")
                .AppendLine("		else false")
                .AppendLine("	end `IsPrimary`,")
                .AppendLine("   case")
                .AppendLine("       when a.data_type ='varchar' then 'nvarchar' ")
                .AppendLine("       when a.data_type ='char' then 'nchar' ")
                .AppendLine("       when a.data_type ='text' then 'text' ")
                .AppendLine("       when a.data_type ='json' then 'text' ")
                .AppendLine("       when a.data_type ='longtext' then 'text' ")
                .AppendLine("       when a.data_type ='longtext' then 'text' ")
                .AppendLine("       when a.data_type ='datetime' then 'datetime' ")
                .AppendLine("       when a.data_type ='timestamp' then 'datetime' ")
                .AppendLine("       when a.data_type ='date' then 'date' ")
                .AppendLine("       when a.data_type ='int' then 'int' ")
                .AppendLine("       when a.data_type ='integer' then 'int' ")
                .AppendLine("       when a.data_type ='smallint' then 'smallint' ")
                .AppendLine("       when a.data_type ='bigint' then 'bigint' ")
                .AppendLine("       when a.data_type ='decimal' then 'decimal' ")
                .AppendLine("       when a.data_type ='float' then 'decimal' ")
                .AppendLine("       when a.data_type ='double' then 'decimal' ")
                .AppendLine("       when a.data_type ='bit' then 'bit' ")
                .AppendLine("       when a.data_type ='binary' then 'binary' ")
                .AppendLine("       else 'nvarchar'")
                .AppendLine("   end  as `FieldType`, ")

                .AppendLine("	CASE WHEN a.character_maximum_length IS NULL THEN 0 ELSE a.character_maximum_length*2 END as `UseBytes`,")
                .AppendLine("	CASE WHEN a.character_maximum_length IS NULL THEN case when a.NUMERIC_PRECISION is null then 0 else a.NUMERIC_PRECISION end ELSE a.character_maximum_length END as `Lens`,")
                .AppendLine("	CASE WHEN a.numeric_scale IS NULL THEN 0 ELSE a.numeric_scale END as `PointDec`,")
                .AppendLine("	case a.is_nullable when 'YES' THEN true ELSE false END AS  `IsNull`,")
                .AppendLine("	case when a.column_default  is null then '' else a.column_default  end as `DbDefault`,")
                .AppendLine("	a.column_comment as `FieldDesc` ")
                .AppendLine("from  `INFORMATION_SCHEMA`.`COLUMNS` as a ")
                .AppendLine("inner join `INFORMATION_SCHEMA`.`TABLES` as b on a.table_catalog = b.table_catalog and a.table_schema=b.table_schema and a.table_name=b.table_name")
                .AppendLine("where b.table_name='[$TabName$]'")

                .ToString();

           //表索引
            _temp_get_tabindex = @"SELECT distinct TABLE_NAME as TableName , INDEX_NAME as IndexName,  case when INDEX_NAME='PRIMARY' then 'Key_Index' ELSE 'Index' end as IndexType
                                    , case when is_visible = 'YES' then '' ELSE 'Y' end as Disabled
                                    FROM INFORMATION_SCHEMA.STATISTICS
                                    WHERE TABLE_NAME = '[$TabName$]'; ";
            //表索引明细
            _temp_get_indexdetail = @"SELECT 1 as TableId, TABLE_NAME as TableName, 1 as IndexId , INDEX_NAME as IndexName,  case when INDEX_NAME='PRIMARY' then 'Key_Index' ELSE 'Index' end as IndexType
                                        , SEQ_IN_INDEX AS ColumnIdx, SEQ_IN_INDEX AS ColumnID, COLUMN_NAME as ColumnName, 'asc' as Sort, case when INDEX_NAME='PRIMARY' then 'Y' ELSE 'N' end as  IPrimary 
                                        , case when NON_UNIQUE ='1' then '' ELSE 'Y' end as IsUnique
                                         , '' AS Ignore_dup_key
                                        , case when is_visible = 'YES' then '' ELSE 'Y' end as Disabled 
                                        , '' AS Fill_factor
                                        , '' AS Padded ,'' as IsIncludedColumn
                                        FROM INFORMATION_SCHEMA.STATISTICS
                                        WHERE TABLE_NAME='[$TabName$]' AND INDEX_NAME='[$IndexName$]';";

            //创建索引
            _temp_create_index = @"ALTER TABLE [$Schema$].[$TabName$]
                                    ADD INDEX `[$IndexName$]` ([$Key$]) USING BTREE ;
                                    ";
            //删除索引
            _temp_drop_index = new StringBuilder()
                .AppendLine("DROP INDEX [$IndexName$] ON [$Schema$].[$TabName$]")
                .ToString();

            //创建视图
            _temp_create_view = new StringBuilder()
                .AppendLine("CREATE VIEW [$Schema$].[$TabName$]")
                .AppendLine("AS")
                .AppendLine("[$ViewSql$]")
                .ToString();


            //修改视图
            _temp_modi_view = new StringBuilder()
                .AppendLine("CREATE  OR REPLACE VIEW [$Schema$].[$TabName$]")
                .AppendLine("AS")
                .AppendLine("[$ViewSql$]")
                .ToString();

            //删除视图
            _temp_drop_view = "DROP VIEW  [$Schema$].[$TabName$]";

            //获取当前库所有的表
            _temp_gettables = new StringBuilder()
                .AppendLine("SELECT   table_name  as `TabName`, ")
                .AppendLine("case   table_type")
                .AppendLine("when 'BASE TABLE' then 'Table'")
                .AppendLine("when 'VIEW' then 'View'")
                .AppendLine("else 'NONE'")
                .AppendLine("END `TabType`,")
                .AppendLine("create_time as `CreateTime`")
                .AppendLine("from   `INFORMATION_SCHEMA`.`TABLES` ")
                .AppendLine("where    table_type in ('BASE TABLE') and  table_schema = '[$Schema$]'")
                .AppendLine("[$Where$]")
                .AppendLine("order by table_type ASC,   create_time desc ")
                .ToString();

            // 获取所有视图
            _temp_getviews = new StringBuilder()
                .AppendLine("SELECT   table_name  as `TabName`, ")
                .AppendLine("case   table_type")
                .AppendLine("when 'BASE TABLE' then 'Table'")
                .AppendLine("when 'VIEW' then 'View'")
                .AppendLine("else 'NONE'")
                .AppendLine("END `TabType`,")
                .AppendLine("create_time as `CreateTime`")
                .AppendLine("from   `INFORMATION_SCHEMA`.`TABLES` ")
                .AppendLine("where    table_type in ('VIEW') and  table_schema = '[$Schema$]'")
                .AppendLine("[$Where$]")
                .AppendLine("order by table_type ASC,   create_time desc ")
                .ToString();


            _temp_check_table_exists= new StringBuilder()
                .AppendLine("SELECT   table_name  as `TabName`, ")
                .AppendLine("case   table_type")
                .AppendLine("when 'BASE TABLE' then 'Table'")
                .AppendLine("when 'VIEW' then 'View'")
                .AppendLine("else 'NONE'")
                .AppendLine("END `TabType`,")
                .AppendLine("create_time as `CreateTime`")
                .AppendLine("from   `INFORMATION_SCHEMA`.`TABLES` ")
                .AppendLine("where    table_type in ('BASE TABLE','VIEW') and  table_schema = '[$Schema$]'")
                .AppendLine("and table_name='[$TabName$]'")
                .AppendLine("order by table_type ASC,   create_time desc ")

                .ToString();

            //获取所有表和视图
            _temp_getalltables = new StringBuilder()
                .AppendLine("SELECT   table_name  as `TabName`, ")
                .AppendLine("case   table_type")
                .AppendLine("when 'BASE TABLE' then 'Table'")
                .AppendLine("when 'VIEW' then 'View'")
                .AppendLine("else 'NONE'")
                .AppendLine("END `TabType`,")
                .AppendLine("create_time as `CreateTime`")
                .AppendLine("from   `INFORMATION_SCHEMA`.`TABLES` ")
                .AppendLine("where    table_type in ('BASE TABLE','VIEW') and  table_schema = '[$Schema$]'")
                .AppendLine("[$Where$]")
                .AppendLine("order by table_type ASC,   create_time desc ")

                .ToString();


            //批量MERGE更新模版
            //_temp_merge_into = new StringBuilder()
            //    .AppendLine("MERGE INTO [dbo].[$TabName$]  as a")
            //    .AppendLine("USING [$Source$] as b")
            //    .AppendLine("on [$OnFilter$] ")
            //    .AppendLine("WHEN MATCHED THEN")
            //    .AppendLine("   update set [$Update$]")
            //    .AppendLine("WHEN NOT MATCHED THEN")
            //    .AppendLine("   insert ([$Field$]) values([$Values$])")
            //    .AppendLine(";")
            //    .ToString();
            _temp_merge_into = new StringBuilder()
                .AppendLine($"INSERT INTO  {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after}")
                .AppendLine($"([$Field$])")
                .AppendLine($"select [$Field$] from  {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$Source$]{_temp_table_after}")
                .AppendLine("ON DUPLICATE KEY")
                .AppendLine($"UPDATE [$Update$] ;")

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
        }
    }
}
