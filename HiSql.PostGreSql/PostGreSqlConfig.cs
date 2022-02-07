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

                return HiSql.PostGreSql.Properties.Resources.HiSql.ToString();
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

        public Dictionary<HiType, string> DbMapping => _dbmapping;

        public string Table_MergeInto { get => _temp_merge_into; }


        public string Sequence { get => _temp_sequence; }

        public string Sequence_Temp { get => _temp_sequence_temp; }

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
                .AppendLine("   pg_class C,")
                .AppendLine("   pg_attribute A")
                .AppendLine("   LEFT OUTER JOIN pg_description b ON A .attrelid = b.objoid")
                .AppendLine("       AND A .attnum = b.objsubid,")
                .AppendLine("   pg_type T")
                .AppendLine("WHERE")
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
        }
    }
}
