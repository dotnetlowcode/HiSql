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



        

        /// <summary>
        /// 字段创建时的模板[$ColumnName$]  这是一个可替换的字符串ColumnName是在HiColumn中的属性名
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
                { "nvarchar",$"{_temp_field_pre}[$ColumnName$]{_temp_field_after} {_temp_field_pre}nvarchar{_temp_field_after}([$FieldLen$]) [$IsNull$]  [$Default$] [$EXTEND$]{_temp_field_split} "},
                { "varchar",$"{_temp_field_pre}[$ColumnName$]{_temp_field_after} {_temp_field_pre}varchar{_temp_field_after}([$FieldLen$]) [$IsNull$] [$Default$] [$EXTEND$]{_temp_field_split}"},
                { "nchar",$"{_temp_field_pre}[$ColumnName$]{_temp_field_after} {_temp_field_pre}nchar{_temp_field_after}([$FieldLen$]) [$IsNull$] [$Default$] [$EXTEND$]{_temp_field_split} "},
                { "char",$"{_temp_field_pre}[$ColumnName$]{_temp_field_after} {_temp_field_pre}char{_temp_field_after}([$FieldLen$]) [$IsNull$] [$Default$] [$EXTEND$]{_temp_field_split}"},
                //样例：[udescript] [text] NULL,
                { "text",$"{_temp_field_pre}[$ColumnName$]{_temp_field_after} {_temp_field_pre}text{_temp_field_after} [$IsNull$] [$Default$] [$EXTEND$]{_temp_field_split}"},

                { "int",$"{_temp_field_pre}[$ColumnName$]{_temp_field_after} {_temp_field_pre}int{_temp_field_after} [$IsIdentity$] [$IsNull$] [$Default$] [$EXTEND$]{_temp_field_split}"},
                { "bigint",$"{_temp_field_pre}[$ColumnName$]{_temp_field_after} {_temp_field_pre}bigint{_temp_field_after} [$IsIdentity$] [$IsNull$] [$Default$] [$EXTEND$]{_temp_field_split}" },
                { "smallint",$"{_temp_field_pre}[$ColumnName$]{_temp_field_after} {_temp_field_pre}smallint{_temp_field_after}  [$IsNull$] [$Default$] [$EXTEND$]{_temp_field_split}"},
                { "decimal",$"{_temp_field_pre}[$ColumnName$]{_temp_field_after} {_temp_field_pre}decimal{_temp_field_after}([$FieldLen$],[$FieldDec$])  [$IsNull$] [$Default$] [$EXTEND$]{_temp_field_split}"},

                { "bit",$"{_temp_field_pre}[$ColumnName$]{_temp_field_after} {_temp_field_pre}bit{_temp_field_after}   [$IsNull$] [$Default$] [$EXTEND$]{_temp_field_split}"},

                { "datetime",$"{_temp_field_pre}[$ColumnName$]{_temp_field_after} {_temp_field_pre}datetime{_temp_field_after}   [$IsNull$] [$Default$] [$EXTEND$]{_temp_field_split}"},
                { "date",$"{_temp_field_pre}[$ColumnName$]{_temp_field_after} {_temp_field_pre}date{_temp_field_after}   [$IsNull$] [$Default$] [$EXTEND$]{_temp_field_split}" },

                { "image",$"{_temp_field_pre}[$ColumnName$]{_temp_field_after} {_temp_field_pre}binary{_temp_field_after}   [$IsNull$] [$EXTEND$]{_temp_field_split}"},
                { "uniqueidentifier",$"{_temp_field_pre}[$ColumnName$]{_temp_field_after} {_temp_field_pre}uniqueidentifier{_temp_field_after}   [$IsNull$] [$Default$] [$EXTEND$]{_temp_field_split}"},
            };


            _temp_create_table = new StringBuilder()
                //样例：CREATE TABLE [dbo].[H_TEST_USER]
                .AppendLine("declare @_effect int")
                .AppendLine($"if not Exists(select top 1 * from dbo.sysObjects where Id=OBJECT_ID(N'[$TabName$]') and xtype='U')")
                .AppendLine("begin")
                .AppendLine($"CREATE TABLE {_temp_schema_pre}[$Schema$]{_temp_schema_after}.{_temp_table_pre}[$TabName$]{_temp_table_after} (")
                .AppendLine("[$Fields$]")
                .AppendLine("[$Keys$]")//主键
                .AppendLine(")[$Primary$]")
                .AppendLine($"delete dbo.{_temp_table_pre}{Constants.HiSysTable["Hi_TabModel"].ToString()}{_temp_table_after} where TabName='[$TabName$]'")
                .AppendLine($"delete dbo.{_temp_table_pre}{Constants.HiSysTable["Hi_FieldModel"].ToString()}{_temp_table_after} where TabName='[$TabName$]'")

                .AppendLine("[$TabStruct$]")
                .AppendLine("[$Comment$]")

                .AppendLine("set @_effect=1 ")
                .AppendLine("end")
                .AppendLine("else")
                .AppendLine("set @_effect=0 ")
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
            _temp_table_key2 = "[$ColumnName$] ASC";//定义主键的排序方式

            _temp_table_key3 = "ON [PRIMARY] ";//TEXTIMAGE_ON [PRIMARY]


            _temp_field_comment = new StringBuilder()
                .AppendLine("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'[$FieldDesc$]' , @level0type=N'SCHEMA',@level0name=N'[$Schema$]', @level1type=N'TABLE',@level1name=N'[$TabName$]', @level2type=N'COLUMN',@level2name=N'[$ColumnName$]'")
                // .AppendLine("GO")
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
