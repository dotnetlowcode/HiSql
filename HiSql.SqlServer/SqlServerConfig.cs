using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// 针对于SQL SERVER的语法所有配置
    /// author:2021.7.9
    /// </summary>
    public  class SqlServerConfig : IDbConfig
    {
        /// <summary>
        /// 数据包大小 经过测试200这个值是比较合理
        /// </summary>
        int _bluksize = 200;
        int _bulkunitsize = 250000;
        string _temp_schema_pre = "[";
        string _temp_schema_after = "]";

        string _temp_table_pre = "[";
        string _temp_table_after = "]";

        string _temp_field_pre = "[";
        string _temp_field_after = "]";

  

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

        string _temp_fun_date = "";

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



        string _temp_addcolumn = "alter table [$TabName$] add [$TempColumn$] ";

        string _temp_delcolumn = "alter table [$TabName$] drop column [$FieldName$]";

        string _temp_modicolumn = "alter table [$TabName$] alter column [$TempColumn$]";

        string _temp_recolumn = "EXECUTE sp_rename N'[$TabName$].[$FieldName$]', N'[$ReFieldName$]', 'COLUMN' ";

        string _temp_retable = "EXECUTE sp_rename '[$TabName$]', '[$ReTabName$]'";


   

        string _temp_setdefalut = "";
        string _temp_setdefalut1 = "";

        string _temp_deldefalut = "";

        /// <summary>
        /// 所有物理实体表
        /// </summary>
        string _temp_gettables = "";

        /// <summary>
        /// 获取所有视图
        /// </summary>
        string _temp_getviews = ""; 
        string _temp_getviewsPaging = "";
        
        /// <summary>
        /// 获取表和视图
        /// </summary>
        string _temp_getalltables = "";
        string _temp_getalltables_paging = "";
        /// <summary>
        /// 分页获取表和视图
        /// </summary>
        string _temp_gettables_paging = "";

        string _temp_getTableDataCount= "select count(*) from [$TabName$] ";
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
        Dictionary<HiType, string> _dbmapping =new Dictionary<HiType, string> ();

        

        List<DefMapping> _lstdefmapping = new List<DefMapping>();


        /// <summary>
        /// 安装HiSql初始化
        /// </summary>
        public string InitSql
        {
            get { //return _temp_install;

                return HiSql.SqlServer.Properties.Resources.HiSql.ToString();
            }
        }

        public List<DefMapping> DbDefMapping
        {
            get => _lstdefmapping;
        }
        public int BlukSize { get => _bluksize; set => _bluksize = value; }
        public int BulkUnitSize { get => _bulkunitsize; set => _bulkunitsize = value; }
        public  string Schema_Pre { get => _temp_schema_pre;   }
        public  string Schema_After { get => _temp_schema_after;  }
        public  string Table_Pre { get => _temp_table_pre;  }
        public  string Table_After { get => _temp_table_after;   }
        public  string Field_Pre { get => _temp_field_pre;   }
        public  string Field_After { get => _temp_field_after;  }
        public  string Table_Create { get => _temp_create_table;   }

        public string Fun_CurrDATE { get => _temp_fun_date; }
        public string Drop_Table { get => _temp_droptable; }
        public  string Table_Global_Create { get => _temp_create_temp_global_table;   }
        public string Table_Global_Create_Drop { get => _temp_create_temp_global_table_drop; }
        public string Table_Local_Create { get => _temp_create_temp_local_table; }

        public string Table_Local_Create_Drop { get => _temp_create_temp_local_table_drop; }
        public  string Table_Declare_Table { get => _temp_declare_table;   }
        public  string Field_Split { get => _temp_field_split;   }
        public  string Table_Key { get => _temp_tabel_key;  }
        public  string Table_Key2 { get => _temp_table_key2;   }
        public  string Table_Key3 { get => _temp_table_key3;  }
        public  string Field_Comment { get => _temp_field_comment; }
        public  string Get_Table_Schema { get => _temp_get_table_schema;   }

        
        public  string Insert_StateMent { get => _temp_insert_statement;   }

        public  string Insert_StateMentv2 { get => _temp_insert_statementv2;   }

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

        
        /// <summary>
        /// sqlserver 专用
        /// </summary>
        public string Set_Default1 { get => _temp_setdefalut1; }

        public string Del_Default { get => _temp_deldefalut; }

        public string Get_Tables { get => _temp_gettables; }

        public string Get_Views { get => _temp_getviews; }
        public string Get_ViewsPaging { get => _temp_getviewsPaging; }
        
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
        public string TabFullName(TableType tableType,string tabname)
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

        public SqlServerConfig()
        {

            
        }
        public SqlServerConfig(bool init)
        {
            
            Init();
        }


        public void Init()
        {

            _temp_fun_date = "getdate()";
            
            _lstdefmapping.Add(new DefMapping { IsRegex = false, DbValue = @"", DBDefault = HiTypeDBDefault.NONE });
            //数字默认
            _lstdefmapping.Add(new DefMapping { IsRegex=true,DbValue= @"^\(\((?<value>[-]?\d+(?:[\.]?)[\d]*)\)\)$", DbType = HiTypeGroup.Number, DBDefault=HiTypeDBDefault.VALUE});

            //bool值
            _lstdefmapping.Add(new DefMapping { IsRegex = true, DbValue = @"^\(\((?<value>[01]{1})\)\)$", DbType = HiTypeGroup.Bool, DBDefault = HiTypeDBDefault.VALUE });

            //指定字符默认值 如('hisql') 或('')
            _lstdefmapping.Add(new DefMapping { IsRegex = true, DbValue = @"^\(\'(?<value>[\w\s*\S*\W*]*)\'\)$", DbType = HiTypeGroup.Char, DBDefault = HiTypeDBDefault.VALUE });

            //日期
            _lstdefmapping.Add(new DefMapping { IsRegex = true, DbValue = @"^\((?<value>getdate\(\))\)$", DbType = HiTypeGroup.Date, DBDefault = HiTypeDBDefault.FUNDATE });

            //md5值
            _lstdefmapping.Add(new DefMapping { IsRegex = true, DbValue = @"^\((?<value>newid\(\))\)$", DbType = HiTypeGroup.Char, DBDefault = HiTypeDBDefault.FUNGUID });

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
                { "nvarchar",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} {_temp_field_pre}nvarchar{_temp_field_after}([$FieldLen$]) [$IsNull$]  [$Default$] [$EXTEND$]{_temp_field_split} "},
                { "varchar",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} {_temp_field_pre}varchar{_temp_field_after}([$FieldLen$]) [$IsNull$] [$Default$] [$EXTEND$]{_temp_field_split}"},
                { "nchar",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} {_temp_field_pre}nchar{_temp_field_after}([$FieldLen$]) [$IsNull$] [$Default$] [$EXTEND$]{_temp_field_split} "},
                { "char",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} {_temp_field_pre}char{_temp_field_after}([$FieldLen$]) [$IsNull$] [$Default$] [$EXTEND$]{_temp_field_split}"},
                //样例：[udescript] [text] NULL,
                { "text",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} {_temp_field_pre}text{_temp_field_after} [$IsNull$] [$Default$] [$EXTEND$]{_temp_field_split}"},

                { "int",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} {_temp_field_pre}int{_temp_field_after} [$IsIdentity$] [$IsNull$] [$Default$] [$EXTEND$]{_temp_field_split}"},
                { "bigint",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} {_temp_field_pre}bigint{_temp_field_after} [$IsIdentity$] [$IsNull$] [$Default$] [$EXTEND$]{_temp_field_split}" },
                { "smallint",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} {_temp_field_pre}smallint{_temp_field_after}  [$IsNull$] [$Default$] [$EXTEND$]{_temp_field_split}"},
                { "decimal",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} {_temp_field_pre}decimal{_temp_field_after}([$FieldLen$],[$FieldDec$])  [$IsNull$] [$Default$] [$EXTEND$]{_temp_field_split}"},

                { "bit",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} {_temp_field_pre}bit{_temp_field_after}   [$IsNull$] [$Default$] [$EXTEND$]{_temp_field_split}"},

                { "datetime",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} {_temp_field_pre}datetime{_temp_field_after}   [$IsNull$] [$Default$] [$EXTEND$]{_temp_field_split}"},
                { "date",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} {_temp_field_pre}date{_temp_field_after}   [$IsNull$] [$Default$] [$EXTEND$]{_temp_field_split}" },

                { "image",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} {_temp_field_pre}binary{_temp_field_after}   [$IsNull$] [$EXTEND$]{_temp_field_split}"},
                { "uniqueidentifier",$"{_temp_field_pre}[$FieldName$]{_temp_field_after} {_temp_field_pre}uniqueidentifier{_temp_field_after}   [$IsNull$] [$Default$] [$EXTEND$]{_temp_field_split}"},
            };


            _temp_create_table = new StringBuilder()
                //样例：CREATE TABLE [dbo].[H_TEST_USER]
                .AppendLine("declare @_effect int")
                .AppendLine($"if not Exists(select top 1 * from dbo.sysObjects where Id=OBJECT_ID(N'[$TabName$]') and ( xtype='U' or xtype='V' )) ")
                .AppendLine("begin")
                .AppendLine($"CREATE TABLE {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after} (")
                .AppendLine("[$Fields$]")
                .AppendLine("[$Keys$]")//主键
                .AppendLine(")[$Primary$]")
                
                .AppendLine("[$Comment$]")

                .AppendLine("set @_effect=1 ")
                .AppendLine("end")
                .AppendLine($"delete dbo.{_temp_table_pre}{Constants.HiSysTable["Hi_TabModel"].ToString()}{_temp_table_after} where TabName='[$TabName$]'")
                .AppendLine($"delete dbo.{_temp_table_pre}{Constants.HiSysTable["Hi_FieldModel"].ToString()}{_temp_table_after} where TabName='[$TabName$]'")

                .AppendLine("[$TabStruct$]")
                //.AppendLine("else")
                .AppendLine("set @_effect=1 ")
                .AppendLine("select @_effect")
                .ToString();

            _temp_create_temp_global_table = new StringBuilder()
                //样例：CREATE TABLE [dbo].[H_TEST_USER]
                .AppendLine("declare @_effect int")
                .AppendLine($"if not Exists(select top 1 * from tempdb.dbo.sysObjects where Id=OBJECT_ID(N'tempdb..[$TabName$]') and xtype='U')")
                .AppendLine("begin")
                .AppendLine($"CREATE TABLE {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after} (")
                .AppendLine("[$Fields$]")
                .AppendLine("[$Keys$]")//主键
                .AppendLine(")[$Primary$]")
                 .AppendLine("set @_effect=1 ")
                .AppendLine("end")
                .AppendLine("else")
                .AppendLine("set @_effect=0 ")
                .AppendLine("select @_effect")
                .ToString();

            _temp_create_temp_global_table_drop = new StringBuilder()
                //样例：CREATE TABLE [dbo].[H_TEST_USER]
                .AppendLine($"if  Exists(select top 1 * from tempdb.dbo.sysObjects where Id=OBJECT_ID(N'tempdb..[$TabName$]') and xtype='U')")
                .AppendLine("   drop table  [$TabName$]")
                .AppendLine("begin")
                .AppendLine($"CREATE TABLE {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after} (")
                .AppendLine("[$Fields$]")
                .AppendLine("[$Keys$]")//主键
                .AppendLine(")[$Primary$]")

                .AppendLine("end")
                .ToString();


            _temp_create_temp_local_table = new StringBuilder()
                //样例：CREATE TABLE [dbo].[H_TEST_USER]
                .AppendLine("declare @_effect int")
                .AppendLine($"if not Exists(select top 1 * from tempdb.dbo.sysObjects where Id=OBJECT_ID(N'tempdb..[$TabName$]') and xtype='U')")
                .AppendLine("begin")
                .AppendLine($"CREATE TABLE {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after} (")
                .AppendLine("[$Fields$]")
                .AppendLine("[$Keys$]")//主键
                .AppendLine(")[$Primary$]")
                 .AppendLine("set @_effect=1 ")
                .AppendLine("end")
                .AppendLine("else")
                .AppendLine("begin")
                .AppendLine("set @_effect=0 ")
                .AppendLine($"delete {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after}")
                .AppendLine("end")
                .AppendLine("select @_effect")

                .ToString();

            _temp_create_temp_local_table_drop = new StringBuilder()
                //样例：CREATE TABLE [dbo].[H_TEST_USER]
                
                .AppendLine($"if  Exists(select top 1 * from tempdb.dbo.sysObjects where Id=OBJECT_ID(N'tempdb..[$TabName$]') and xtype='U')")
                .AppendLine("   drop table  [$TabName$]")
                .AppendLine("begin")
                .AppendLine($"CREATE TABLE {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after} (")
                .AppendLine("[$Fields$]")
                .AppendLine("[$Keys$]")//主键
                .AppendLine(")[$Primary$]")

                .AppendLine("end")
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
                .AppendLine($"CONSTRAINT {_temp_table_pre}PK_[$TabName$]_[$ConnectID$]{_temp_table_after} PRIMARY KEY CLUSTERED")
                .AppendLine("(")
                .AppendLine("[$Keys$]")
                .AppendLine(")WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]")
                .ToString();
            _temp_table_key2 = "[$FieldName$] ASC";//定义主键的排序方式

            _temp_table_key3 = "ON [PRIMARY] ";//TEXTIMAGE_ON [PRIMARY]


            _temp_field_comment = new StringBuilder()

                .AppendLine("if exists(select a.name as TabName,b.name as FieldName,c.value as FieldDesc")
                .AppendLine("from sys.tables as a")
                .AppendLine("inner join sys.columns as b on b.object_id=a.object_id")
                .AppendLine("left join sys.extended_properties as c on c.major_id=b.object_id and c.minor_id = b.column_id")
                .AppendLine("where a.name='[$TabName$]' and b.name = '[$FieldName$]' and c.value is not null)")
                .AppendLine("begin")
                .AppendLine("EXECUTE sp_updateextendedproperty N'MS_Description', N'[$FieldDesc$]', N'SCHEMA', N'[$Schema$]', N'TABLE', N'[$TabName$]', N'COLUMN', N'[$FieldName$]'")
                .AppendLine("end")
                .AppendLine("else")
                .AppendLine("begin")
                .AppendLine("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'[$FieldDesc$]' , @level0type=N'SCHEMA',@level0name=N'[$Schema$]', @level1type=N'TABLE',@level1name=N'[$TabName$]', @level2type=N'COLUMN',@level2name=N'[$FieldName$]'")
                .AppendLine("end")

                .ToString();

  

            _temp_insert_statement = new StringBuilder()
                .AppendLine($"insert into {_temp_schema_pre}dbo{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after}([$FIELDS$]) values([$VALUES$])")
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
                .AppendLine($"insert into {_temp_schema_pre}dbo{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after}([$FIELDS$]) [$VALUES$]")
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
                .AppendLine("	(case when d.xtype = 'U' THEN 'Table' else 'View' end) as TabType,")
                .AppendLine("	d.name AS TabName,")
                .AppendLine("	a.colorder as FieldNo,")
                .AppendLine("	a.name FieldName,")
                .AppendLine("	(case when COLUMNPROPERTY( a.id,a.name,'IsIdentity')=1 then '1'else '0' end) IsIdentity,")
                .AppendLine("	(case when (SELECT count(1) ")
                .AppendLine("		FROM sysobjects ")
                .AppendLine("		WHERE (")
                .AppendLine("			name in (")
                .AppendLine("				SELECT name FROM sysindexes WHERE (id = a.id) ")
                .AppendLine("				AND ")
                .AppendLine("					(indid in ")
                .AppendLine("						(")
                .AppendLine("							SELECT indid FROM sysindexkeys WHERE (id = a.id) AND (colid in (SELECT colid FROM syscolumns WHERE (id = a.id) AND (name = a.name)))")
                .AppendLine("						)")
                .AppendLine("					)")
                .AppendLine("				)")
                .AppendLine("			) AND (xtype = 'PK') ")
                .AppendLine("		) > 0 then '1' else '0' end")
                .AppendLine("	) as IsPrimary,")
                .AppendLine("	b.name FieldType,")
                .AppendLine("	a.length UseBytes,")
                .AppendLine("	COLUMNPROPERTY(a.id,a.name,'PRECISION') as Lens,")
                .AppendLine("	isnull(COLUMNPROPERTY(a.id,a.name,'Scale'),0) as PointDec,")
                .AppendLine("	(case when a.isnullable=1 then '1'else '0' end) [IsNull], ")
                .AppendLine("	isnull(e.text,'') DbDefault,")
                .AppendLine("	isnull(g.[value],'') AS FieldDesc   ")
                .AppendLine("	FROM  syscolumns a ")
                .AppendLine("	left join systypes b on a.xtype=b.xusertype")
                .AppendLine("	inner join sysobjects d on a.id=d.id  and ( d.xtype='U' or d.xtype='V' ) and d.name<>'dtproperties'")
                .AppendLine("	left join syscomments e on a.cdefault=e.id")
                .AppendLine("	left join sys.extended_properties g on a.id=g.major_id AND a.colid = g.minor_id  ")
                .AppendLine("	where d.name =  N'[$TabName$]'")
                .AppendLine("	order by a.id,a.colorder")
                .ToString();


            _temp_setdefalut1 = " exec('ALTER TABLE [$Schema$].[$TabName$] ADD CONSTRAINT '+@_constname_[$FieldName$] + ' [$DefValue$] FOR [$FieldName$]')";

            _temp_setdefalut = new StringBuilder()
                .AppendLine("declare @_constname_[$FieldName$] varchar(200)")
                .AppendLine("if exists(select a.name as fieldname, b.name as consname from syscolumns as a inner join sysobjects as b on a.cdefault=b.id where a.id=object_id('[$TabName$]') and a.name='[$FieldName$]')")
                .AppendLine("   begin   ")
                .AppendLine("       select @_constname_[$FieldName$] = b.name from syscolumns as a ")
                .AppendLine("       inner join sysobjects as b on a.cdefault=b.id")
                .AppendLine("       where a.id=object_id('[$TabName$]') and a.name='[$FieldName$]'")
                .AppendLine("       exec('ALTER TABLE [$Schema$].[$TabName$] DROP CONSTRAINT '+@_constname_[$FieldName$])")
                .AppendLine("       [$AddDefault$]")
                //.AppendLine("       exec('ALTER TABLE [$Schema$].[$TabName$] ADD CONSTRAINT '+@_constname_[$FieldName$]+ ' [$DefValue$] ' + ' FOR [$FieldName$]' )")
                .AppendLine("   end")
                .AppendLine("else")
                .AppendLine("   begin")
                .AppendLine("       set @_constname_[$FieldName$] ='DF_H_[$TabName$]_[$FieldName$]_[$KEY$]'")
                .AppendLine("       [$AddDefault$]")
                .AppendLine("   end")
                .ToString();


            //获取当前库中所有的表
            _temp_gettables = new StringBuilder()
                .AppendLine("select [name] as TabName,(case when xtype='U' then 'Table' else 'View' end ) as TabType,crdate as CreateTime")
                .AppendLine("from sysobjects where xtype='U' ")
                .AppendLine("[$Where$]")
                .AppendLine("order by crdate desc ")
                .ToString();


            _temp_gettables_paging = new StringBuilder()
               .AppendLine(@"select count(*)   from sysobjects where (xtype='U' )  [$Where$] ;")
               .AppendLine("SELECT * FROM (select row_seq = ROW_NUMBER()OVER( order by xtype ASC, crdate desc ) , [name] as TabName,(case when xtype='U' then 'Table' else 'View' end ) as TabType,crdate as CreateTime " +
               "from sysobjects where (xtype='U')   ")
               .AppendLine("[$Where$] ) AS t WHERE row_seq > [$SeqBegin$] AND row_seq <=[$SeqEnd$]")
               .AppendLine("order by row_seq")
               .ToString();

            //获取所有视图
            _temp_getviews = new StringBuilder()
                .AppendLine("select [name] as TabName,(case when xtype='U' then 'Table' else 'View' end ) as TabType,crdate as CreateTime")
                .AppendLine("from sysobjects where xtype='V' ")
                .AppendLine("[$Where$]")
                .AppendLine("order by crdate desc ")
                .ToString();
            _temp_getviewsPaging = new StringBuilder()
               .AppendLine(@"select count(*)   from sysobjects where (xtype='V' )  [$Where$] ;")
               .AppendLine("SELECT * FROM (select row_seq = ROW_NUMBER()OVER( order by xtype ASC, crdate desc ) , [name] as TabName,(case when xtype='U' then 'Table' else 'View' end ) as TabType,crdate as CreateTime " +
               "from sysobjects where (xtype='V')   ")
               .AppendLine("[$Where$] ) AS t WHERE row_seq > [$SeqBegin$] AND row_seq <=[$SeqEnd$]")
               .AppendLine("order by row_seq")
               .ToString();

            //获取表和视图
            _temp_getalltables = new StringBuilder()
                .AppendLine("select [name] as TabName,(case when xtype='U' then 'Table' else 'View' end ) as TabType,crdate as CreateTime ")
                .AppendLine("from sysobjects where (xtype='U' or xtype='V')   ")
                .AppendLine("[$Where$]")
                .AppendLine("order by xtype ASC, [name] ASC, crdate desc ")
                .ToString();


            _temp_getalltables_paging = new StringBuilder()
               .AppendLine(@"select count(*)   from sysobjects where (xtype='U' or xtype='V')  [$Where$] ;")
               .AppendLine("SELECT * FROM (select row_seq = ROW_NUMBER()OVER( order by xtype ASC, [name] ASC, crdate desc ) , [name] as TabName,(case when xtype='U' then 'Table' else 'View' end ) as TabType,crdate as CreateTime " +
               "from sysobjects where (xtype='U' or xtype='V')   ")
               .AppendLine("[$Where$] ) AS t WHERE row_seq > [$SeqBegin$] AND row_seq <=[$SeqEnd$]")
               .AppendLine("order by row_seq")
               .ToString();

            //删除字段默认值
            _temp_deldefalut = new StringBuilder()
                .AppendLine("declare @_constname varchar(200)")
                .AppendLine("if exists(select a.name as fieldname, b.name as consname from syscolumns as a inner join sysobjects as b on a.cdefault=b.id where a.id=object_id('[$TabName$]') and a.name='[$FieldName$]')")
                .AppendLine("   begin   ")
                .AppendLine("       select @_constname = b.name from syscolumns as a ")
                .AppendLine("       inner join sysobjects as b on a.cdefault=b.id")
                .AppendLine("       where a.id=object_id('[$TabName$]') and a.name='[$FieldName$]'")
                .AppendLine("       exec('ALTER TABLE [$Schema$].[$TabName$] DROP CONSTRAINT '+@_constname)")
                .AppendLine("   end")
                .ToString();

            


            //判断表或视图存不存在
            _temp_check_table_exists = new StringBuilder()
                .AppendLine("select [name] as TabName,(case when xtype='U' then 'Table' else 'View' end ) as TabType,crdate as CreateTime")
                .AppendLine("from sysobjects where (xtype='V' or xtype='U' )   and [name]='[$TabName$]' ")
                .ToString();


            //创建视图
            _temp_create_view = new StringBuilder()
                .AppendLine("CREATE VIEW [$Schema$].[$TabName$]")
                .AppendLine("AS")
                .AppendLine("[$ViewSql$]")
                .ToString();

            //修改视图
            _temp_modi_view= new StringBuilder()
                .AppendLine("ALTER VIEW [$Schema$].[$TabName$]")
                .AppendLine("AS")
                .AppendLine("[$ViewSql$]")
                .ToString();

            //删除视图
            _temp_drop_view = "DROP VIEW  [$Schema$].[$TabName$]";

            //获取临时表清单
            _temp_globaltempdb_query = "select [name] as TabName ,(case when name<>'' then 'Global' end) as TabType,crdate as CreateTime from tempdb.dbo.sysobjects where [name] like '##%' order by [name] , crdate desc";


            //获取索引明细
            _temp_get_indexdetail = new StringBuilder()
                .AppendLine("SELECT")
                .AppendLine("TableId=O.[object_id],")
                .AppendLine("TableName=O.Name,")
                .AppendLine("IndexId=ISNULL(KC.[object_id],IDX.index_id),")
                .AppendLine("IndexName=IDX.Name,")
                .AppendLine("IndexType=ISNULL(case KC.type_desc when 'PRIMARY_KEY_CONSTRAINT' then 'Key_Index' else 'Index' end,'Index'),")
                .AppendLine("ColumnIdx=IDXC.index_column_id,")
                .AppendLine("ColumnID=C.Column_id,")
                .AppendLine("ColumnName=C.Name,")
                .AppendLine("Sort=CASE INDEXKEY_PROPERTY(IDXC.[object_id],IDXC.index_id,IDXC.index_column_id,'IsDescending')")
                .AppendLine("WHEN 1 THEN 'DESC' WHEN 0 THEN 'ASC' ELSE '' END,")
                .AppendLine("IPrimary=CASE WHEN IDX.is_primary_key=1 THEN N'Y'ELSE N'' END,")
                .AppendLine("[IsUnique]=CASE WHEN IDX.is_unique=1 THEN N'Y'ELSE N'' END,")
                .AppendLine("Ignore_dup_key=CASE WHEN IDX.ignore_dup_key=1 THEN N'Y'ELSE N'' END,")
                .AppendLine("Disabled=CASE WHEN IDX.is_disabled=1 THEN N'Y'ELSE N'' END,")
                .AppendLine("Fill_factor=IDX.fill_factor,")
                .AppendLine("Padded=CASE WHEN IDX.is_padded=1 THEN N'Y'ELSE N'' END, IsIncludedColumn = IDXC.is_included_column")
                .AppendLine("FROM sys.indexes IDX ")
                .AppendLine("INNER JOIN sys.index_columns IDXC")
                .AppendLine("ON IDX.[object_id]=IDXC.[object_id]")
                .AppendLine("AND IDX.index_id=IDXC.index_id")
                .AppendLine("LEFT JOIN sys.key_constraints KC")
                .AppendLine("ON IDX.[object_id]=KC.[parent_object_id]")
                .AppendLine("AND IDX.index_id=KC.unique_index_id")
                .AppendLine("INNER JOIN sys.objects O")
                .AppendLine("ON O.[object_id]=IDX.[object_id]")
                .AppendLine("INNER JOIN sys.columns C")
                .AppendLine("ON O.[object_id]=C.[object_id]")
                .AppendLine("AND O.type='U'")
                .AppendLine("AND O.is_ms_shipped=0")
                .AppendLine("AND IDXC.Column_id=C.Column_id where O.name='[$TabName$]' and IDX.name='[$IndexName$]' ")
                .ToString();

            //获取指定表的索引清单
            _temp_get_tabindex = new StringBuilder()
                .AppendLine("select distinct TableName, IndexName,IndexType,[Disabled] from (")
                .AppendLine("SELECT")
                .AppendLine("TableId=O.[object_id],")
                .AppendLine("TableName=O.Name,")
                .AppendLine("IndexId=ISNULL(KC.[object_id],IDX.index_id),")
                .AppendLine("IndexName=IDX.Name,")
                .AppendLine("IndexType=ISNULL(case KC.type_desc when 'PRIMARY_KEY_CONSTRAINT' then 'Key_Index' else 'Index' end,'Index'),")
                .AppendLine("ColumnIdx=IDXC.index_column_id,")
                .AppendLine("ColumnID=C.Column_id,")
                .AppendLine("ColumnName=C.Name,")
                .AppendLine("Sort=CASE INDEXKEY_PROPERTY(IDXC.[object_id],IDXC.index_id,IDXC.index_column_id,'IsDescending')")
                .AppendLine("WHEN 1 THEN 'DESC' WHEN 0 THEN 'ASC' ELSE '' END,")
                .AppendLine("IPrimary=CASE WHEN IDX.is_primary_key=1 THEN N'Y'ELSE N'' END,")
                .AppendLine("[IsUnique]=CASE WHEN IDX.is_unique=1 THEN N'Y'ELSE N'' END,")
                .AppendLine("Ignore_dup_key=CASE WHEN IDX.ignore_dup_key=1 THEN N'Y'ELSE N'' END,")
                .AppendLine("Disabled=CASE WHEN IDX.is_disabled=1 THEN N'Y'ELSE N'' END,")
                .AppendLine("Fill_factor=IDX.fill_factor,")
                .AppendLine("Padded=CASE WHEN IDX.is_padded=1 THEN N'Y'ELSE N'' END, IsIncludedColumn = IDXC.is_included_column")
                .AppendLine("FROM sys.indexes IDX ")
                .AppendLine("INNER JOIN sys.index_columns IDXC")
                .AppendLine("ON IDX.[object_id]=IDXC.[object_id]")
                .AppendLine("AND IDX.index_id=IDXC.index_id")
                .AppendLine("LEFT JOIN sys.key_constraints KC")
                .AppendLine("ON IDX.[object_id]=KC.[parent_object_id]")
                .AppendLine("AND IDX.index_id=KC.unique_index_id")
                .AppendLine("INNER JOIN sys.objects O")
                .AppendLine("ON O.[object_id]=IDX.[object_id]")
                .AppendLine("INNER JOIN sys.columns C")
                .AppendLine("ON O.[object_id]=C.[object_id]")
                .AppendLine("AND O.type='U'")
                .AppendLine("AND O.is_ms_shipped=0")
                .AppendLine("AND IDXC.Column_id=C.Column_id where O.name='[$TabName$]'  ")
                .AppendLine(") as hisql_index  ")
                .ToString();


            //表创建非聚集索引
            _temp_create_index = new StringBuilder()
                .AppendLine("CREATE NONCLUSTERED INDEX [$IndexName$] ON [$Schema$].[$TabName$]")
                .AppendLine("(")
                .AppendLine("[$Key$]")
                .AppendLine(") WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)")
                .AppendLine("ON [PRIMARY]")
                
                .ToString();

            //删除索引
            _temp_drop_index = new StringBuilder()
                .AppendLine("DROP INDEX [$IndexName$] ON [$Schema$].[$TabName$]")
                .ToString();


            //批量MERGE更新模版
            _temp_merge_into = new StringBuilder()
                .AppendLine("MERGE INTO [dbo].[$TabName$]  as a")
                .AppendLine("USING [$Source$] as b")
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

            _temp_delete = $"delete {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after}";

            _temp_delete_where = $"delete {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after} where [$Where$]";

            //删除不会留下任何痕迹
            _temp_truncate = $"TRUNCATE TABLE {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after}";

            _temp_droptable = $"drop table {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after}";
        }
    }
}
