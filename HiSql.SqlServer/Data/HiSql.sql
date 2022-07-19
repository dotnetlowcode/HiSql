declare @_effect int
if not Exists(select top 1 * from dbo.sysObjects where Id=OBJECT_ID(N'Hi_TabModel') and xtype='U')
begin
	CREATE TABLE [[$Schema$]].[Hi_TabModel] (
	[TabName] [nvarchar](50) NOT NULL   , 
	[TabReName] [nvarchar](50) NULL  default ('') , 
	[TabDescript] [nvarchar](100) NULL  default ('') , 
	[TabStoreType] [int]  NULL default ((0)) ,
	[TabType] [int]  NULL default ((0)) ,
	[TabCacheType] [int]  NULL default ((0)) ,
	[TabStatus] [int]  NULL default ((0)) ,
	[IsSys] [bit]   NULL default ((0)) ,
	[IsEdit] [bit]   NULL default ((0)) ,
	[IsLog] [bit]   NULL default ((0)) ,
	[LogTable] [nvarchar](50) NOT NULL  default ('') , 
	[LogExprireDay] [int]  NULL default ((0)) ,
	[CreateTime] [datetime]   NULL default (getdate()) ,
	[CreateName] [nvarchar](50) NULL  default ('') , 
	[ModiTime] [datetime]   NULL default (getdate()) ,
	[ModiName] [nvarchar](50) NULL  default ('') , 

	CONSTRAINT [PK_Hi_TabModel_2cc8e377-ffc6-4e2a-8445-79127b0ff3d4] PRIMARY KEY CLUSTERED
	(
	[TabName] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	)ON [PRIMARY] 
	
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'表名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_TabModel', @level2type=N'COLUMN',@level2name=N'TabName'
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'表的别名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_TabModel', @level2type=N'COLUMN',@level2name=N'TabReName'
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'表描述' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_TabModel', @level2type=N'COLUMN',@level2name=N'TabDescript'
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'表存储方式' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_TabModel', @level2type=N'COLUMN',@level2name=N'TabStoreType'
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'表类型' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_TabModel', @level2type=N'COLUMN',@level2name=N'TabType'
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'表的缓存类型' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_TabModel', @level2type=N'COLUMN',@level2name=N'TabCacheType'

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'表状态' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_TabModel', @level2type=N'COLUMN',@level2name=N'TabStatus'

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否系统内置表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_TabModel', @level2type=N'COLUMN',@level2name=N'IsSys'

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否可编辑' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_TabModel', @level2type=N'COLUMN',@level2name=N'IsEdit'

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否开启表日志' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_TabModel', @level2type=N'COLUMN',@level2name=N'IsLog'

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'日志表名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_TabModel', @level2type=N'COLUMN',@level2name=N'LogTable'

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'日志保留天数' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_TabModel', @level2type=N'COLUMN',@level2name=N'LogExprireDay'

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_TabModel', @level2type=N'COLUMN',@level2name=N'CreateTime'

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_TabModel', @level2type=N'COLUMN',@level2name=N'CreateName'

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_TabModel', @level2type=N'COLUMN',@level2name=N'ModiTime'

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_TabModel', @level2type=N'COLUMN',@level2name=N'ModiName'
end



if not Exists(select top 1 * from dbo.sysObjects where Id=OBJECT_ID(N'Hi_FieldModel') and xtype='U')
begin
	CREATE TABLE [[$Schema$]].[Hi_FieldModel] (
	[DbName] [varchar](50) NOT NULL  default('') , 
	[TabName] [nvarchar](50) NOT NULL   , 
	[FieldName] [nvarchar](50) NOT NULL   , 
	[FieldDesc] [nvarchar](100) NULL  default ('') , 
	[IsIdentity] [bit]   NULL default ((0)) ,
	[IsPrimary] [bit]   NULL default ((0)) ,
	[IsBllKey] [bit]   NULL default ((0)) ,
	[FieldType] [int]  NULL default ((0)) ,
	[SortNum] [int]  NULL default ((0)) ,
	[Regex] [nvarchar](200) NULL  default ('') , 
	[DBDefault] [int]  NULL default ((0)) ,
	[DefaultValue] [nvarchar](50) NULL  default ('') , 
	[FieldLen] [int]  NULL default ((0)) ,
	[FieldDec] [int]  NULL default ((0)) ,
	[SNO] [varchar](50) NULL default ('') , 
	[SNO_NUM] [varchar](3) NULL default ('') , 
	[IsSys] [bit]   NULL default ((0)) ,
	[IsNull] [bit]   NULL default ((0)) ,
	[IsRequire] [bit]   NULL default ((0)) ,
	[IsIgnore] [bit]   NULL default ((0)) ,
	[IsObsolete] [bit]   NULL default ((0)) ,
	[IsShow] [bit]   NULL default ((0)) ,
	[IsSearch] [bit]   NULL default ((0)) ,
	[SrchMode] [int]  NULL default ((0)) ,
	[IsRefTab] [bit]   NULL default ((0)) ,
	[RefTab] [nvarchar](50) NULL  default ('') , 
	[RefField] [nvarchar](50) NULL  default ('') , 
	[RefFields] [nvarchar](500) NULL  default ('') , 
	[RefFieldDesc] [nvarchar](500) NULL  default ('') , 
	[RefWhere] [nvarchar](500) NULL  default ('') , 
	[CreateTime] [datetime]   NULL default (getdate()) ,
	[CreateName] [nvarchar](50) NULL  default ('') , 
	[ModiTime] [datetime]   NULL default (getdate()) ,
	[ModiName] [nvarchar](50) NULL  default ('') , 

	CONSTRAINT [PK_Hi_FieldModel_ed721f6b-296a-447e-ac67-7d02fd8e338c] PRIMARY KEY CLUSTERED
	(
	[TabName] ASC,
	[FieldName] ASC,
	[DbName] ASC

	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

	)ON [PRIMARY] 

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'数据库名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_FieldModel', @level2type=N'COLUMN',@level2name=N'DbName'

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'表名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_FieldModel', @level2type=N'COLUMN',@level2name=N'TabName'

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'字段名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_FieldModel', @level2type=N'COLUMN',@level2name=N'FieldName'

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'字段名描述' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_FieldModel', @level2type=N'COLUMN',@level2name=N'FieldDesc'

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否自增ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_FieldModel', @level2type=N'COLUMN',@level2name=N'IsIdentity'

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否主键' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_FieldModel', @level2type=N'COLUMN',@level2name=N'IsPrimary'

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否是业务Key' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_FieldModel', @level2type=N'COLUMN',@level2name=N'IsBllKey'

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'字段类型' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_FieldModel', @level2type=N'COLUMN',@level2name=N'FieldType'

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'字段排序号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_FieldModel', @level2type=N'COLUMN',@level2name=N'SortNum'

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'正则校验表达式' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_FieldModel', @level2type=N'COLUMN',@level2name=N'Regex'

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'默认值类型' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_FieldModel', @level2type=N'COLUMN',@level2name=N'DBDefault'

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'默认值' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_FieldModel', @level2type=N'COLUMN',@level2name=N'DefaultValue'

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'字段长度' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_FieldModel', @level2type=N'COLUMN',@level2name=N'FieldLen'

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'小数点位数' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_FieldModel', @level2type=N'COLUMN',@level2name=N'FieldDec'

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'编号名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_FieldModel', @level2type=N'COLUMN',@level2name=N'SNO'

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'子编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_FieldModel', @level2type=N'COLUMN',@level2name=N'SNO_NUM'

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否系统字段' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_FieldModel', @level2type=N'COLUMN',@level2name=N'IsSys'

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否允许NULL' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_FieldModel', @level2type=N'COLUMN',@level2name=N'IsNull'

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否必填' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_FieldModel', @level2type=N'COLUMN',@level2name=N'IsRequire'

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否忽略' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_FieldModel', @level2type=N'COLUMN',@level2name=N'IsIgnore'

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否作废' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_FieldModel', @level2type=N'COLUMN',@level2name=N'IsObsolete'

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否显示' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_FieldModel', @level2type=N'COLUMN',@level2name=N'IsShow'

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否允许搜索' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_FieldModel', @level2type=N'COLUMN',@level2name=N'IsSearch'

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'搜索模式' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_FieldModel', @level2type=N'COLUMN',@level2name=N'SrchMode'

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否引用表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_FieldModel', @level2type=N'COLUMN',@level2name=N'IsRefTab'

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'引用表名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_FieldModel', @level2type=N'COLUMN',@level2name=N'RefTab'

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'引用的字段' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_FieldModel', @level2type=N'COLUMN',@level2name=N'RefField'

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'引用字段清单' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_FieldModel', @level2type=N'COLUMN',@level2name=N'RefFields'

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'引用字段清单描述' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_FieldModel', @level2type=N'COLUMN',@level2name=N'RefFieldDesc'

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'引用条件' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_FieldModel', @level2type=N'COLUMN',@level2name=N'RefWhere'

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_FieldModel', @level2type=N'COLUMN',@level2name=N'CreateTime'

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_FieldModel', @level2type=N'COLUMN',@level2name=N'CreateName'

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_FieldModel', @level2type=N'COLUMN',@level2name=N'ModiTime'

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_FieldModel', @level2type=N'COLUMN',@level2name=N'ModiName'
end


if not Exists(select top 1 * from dbo.sysObjects where Id=OBJECT_ID(N'Hi_Domain') and xtype='U')
begin
	CREATE TABLE [[$Schema$]].[Hi_Domain] (
	[Domain] [nvarchar](20) NOT NULL   , 
	[DomainDesc] [nvarchar](100) NULL  default ('') , 
	[CreateTime] [datetime]   NULL default (getdate()) ,
	[CreateName] [nvarchar](50) NULL  default ('') , 
	[ModiTime] [datetime]   NULL default (getdate()) ,
	[ModiName] [nvarchar](50) NULL  default ('') , 

	CONSTRAINT [PK_Hi_Domain_6e0da6c8-45dc-46e4-967d-bb917499f139] PRIMARY KEY CLUSTERED
	(
	[Domain] ASC

	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

	)ON [PRIMARY] 

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'数据域名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_Domain', @level2type=N'COLUMN',@level2name=N'Domain'

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'数据域描述' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_Domain', @level2type=N'COLUMN',@level2name=N'DomainDesc'

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_Domain', @level2type=N'COLUMN',@level2name=N'CreateTime'

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_Domain', @level2type=N'COLUMN',@level2name=N'CreateName'

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_Domain', @level2type=N'COLUMN',@level2name=N'ModiTime'

	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_Domain', @level2type=N'COLUMN',@level2name=N'ModiName'
end

if not Exists(select top 1 * from dbo.sysObjects where Id=OBJECT_ID(N'Hi_DataElement') and xtype='U')
begin
	CREATE TABLE [[$Schema$]].[Hi_DataElement] (
	[Domain] [nvarchar](20) NOT NULL   , 
	[ElementValue] [nvarchar](50) NOT NULL   , 
	[ElementDesc] [nvarchar](100) NULL  default ('') , 
	[SortNum] [int]  NULL default ((0)) ,
	[CreateTime] [datetime]   NULL default (getdate()) ,
	[CreateName] [nvarchar](50) NULL  default ('') , 
	[ModiTime] [datetime]   NULL default (getdate()) ,
	[ModiName] [nvarchar](50) NULL  default ('') , 

	CONSTRAINT [PK_Hi_DataElement_2425bea1-accb-4385-bdc9-d0a69a3b07df] PRIMARY KEY CLUSTERED
	(
	[Domain] ASC,
	[ElementValue] ASC

	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

	)ON [PRIMARY] 


	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'数据域名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_DataElement', @level2type=N'COLUMN',@level2name=N'Domain'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'数据域值' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_DataElement', @level2type=N'COLUMN',@level2name=N'ElementValue'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'数据域值描述' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_DataElement', @level2type=N'COLUMN',@level2name=N'ElementDesc'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'数据域排序号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_DataElement', @level2type=N'COLUMN',@level2name=N'SortNum'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_DataElement', @level2type=N'COLUMN',@level2name=N'CreateTime'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_DataElement', @level2type=N'COLUMN',@level2name=N'CreateName'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_DataElement', @level2type=N'COLUMN',@level2name=N'ModiTime'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Hi_DataElement', @level2type=N'COLUMN',@level2name=N'ModiName'
end


delete dbo.[Hi_TabModel] where TabName='Hi_TabModel'
delete dbo.[Hi_FieldModel] where TabName='Hi_TabModel'

delete dbo.[Hi_TabModel] where TabName='Hi_FieldModel'
delete dbo.[Hi_FieldModel] where TabName='Hi_FieldModel'

delete dbo.[Hi_TabModel] where TabName='Hi_Domain'
delete dbo.[Hi_FieldModel] where TabName='Hi_Domain'

delete dbo.[Hi_TabModel] where TabName='Hi_DataElement'
delete dbo.[Hi_FieldModel] where TabName='Hi_DataElement'

insert into [[$Schema$]].[Hi_TabModel] (
[TabName],[TabReName],[TabDescript],[TabStoreType],[TabType],[TabCacheType],[TabStatus],[IsSys],[IsEdit],[IsLog],[LogTable],[LogExprireDay]) values('Hi_TabModel','Hi_TabModel','表结构信息主表','0','0','0','0',0,1,0,'','0')

insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_TabModel','TabName','表名',0,1,1,'11','2','','10','','50','0','','',1,0,1,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_TabModel','TabReName','表的别名',0,0,0,'11','5','','10','','50','0','','',0,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_TabModel','TabDescript','表描述',0,0,0,'11','5','','10','','100','0','','',0,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_TabModel','TabStoreType','表存储方式',0,0,0,'21','10','','10','','0','0','','',0,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_TabModel','TabType','表类型',0,0,0,'21','15','','10','','0','0','','',0,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_TabModel','TabCacheType','表的缓存类型',0,0,0,'21','20','','10','','0','0','','',0,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_TabModel','TabStatus','表状态',0,0,0,'21','25','','10','','0','0','','',0,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_TabModel','IsSys','是否系统内置表',0,0,0,'31','30','','10','','0','0','','',1,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_TabModel','IsEdit','是否可编辑',0,0,0,'31','35','','10','','0','0','','',1,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_TabModel','IsLog','是否开启表日志',0,0,0,'31','40','','10','','0','0','','',1,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_TabModel','LogTable','日志表名',0,0,0,'11','45','','10','','50','0','','',1,0,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_TabModel','LogExprireDay','日志保留天数',0,0,0,'21','50','','10','','0','0','','',1,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_TabModel','CreateTime','创建时间',0,0,0,'41','990','','20','','0','0','','',0,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_TabModel','CreateName','创建人',0,0,0,'11','991','','10','','50','0','','',0,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_TabModel','ModiTime','修改时间',0,0,0,'41','995','','20','','0','0','','',0,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_TabModel','ModiName','修改人',0,0,0,'11','998','','10','','50','0','','',0,1,0,0,0,1,1,'10',0,'','','','','')






insert into [[$Schema$]].[Hi_TabModel] (
[TabName],[TabReName],[TabDescript],[TabStoreType],[TabType],[TabCacheType],[TabStatus],[IsSys],[IsEdit],[IsLog],[LogTable],[LogExprireDay]) values('Hi_FieldModel','Hi_FieldModel','表结构信息字段表','0','0','0','0',0,1,0,'','0')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_FieldModel','DbName','数据库名',0,1,1,'12','4','','10','','50','0','','',1,0,1,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_FieldModel','TabName','表名',0,1,1,'11','5','','10','','50','0','','',1,0,1,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_FieldModel','FieldName','字段名',0,1,1,'11','10','','10','','50','0','','',1,0,1,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_FieldModel','FieldDesc','字段名描述',0,0,0,'11','15','','10','','100','0','','',1,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_FieldModel','IsIdentity','是否自增ID',0,0,0,'31','20','','10','','0','0','','',1,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_FieldModel','IsPrimary','是否主键',0,0,0,'31','25','','10','','0','0','','',1,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_FieldModel','IsBllKey','是否是业务Key',0,0,0,'31','30','','10','','0','0','','',1,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_FieldModel','FieldType','字段类型',0,0,0,'21','35','','10','','0','0','','',1,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_FieldModel','SortNum','字段排序号',0,0,0,'21','40','','10','','0','0','','',1,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_FieldModel','Regex','正则校验表达式',0,0,0,'11','45','','10','','200','0','','',1,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_FieldModel','DBDefault','默认值类型',0,0,0,'21','50','','10','','0','0','','',1,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_FieldModel','DefaultValue','默认值',0,0,0,'11','55','','10','','50','0','','',1,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_FieldModel','FieldLen','字段长度',0,0,0,'21','60','','10','','0','0','','',1,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_FieldModel','FieldDec','小数点位数',0,0,0,'21','65','','10','','0','0','','',1,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_FieldModel','SNO','编号名称',0,0,0,'12','70','','10','','50','0','','',1,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_FieldModel','SNO_NUM','子编号',0,0,0,'12','75','','10','','3','0','','',1,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_FieldModel','IsSys','是否系统字段',0,0,0,'31','80','','10','','0','0','','',1,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_FieldModel','IsNull','是否允许NULL',0,0,0,'31','85','','10','','0','0','','',1,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_FieldModel','IsRequire','是否必填',0,0,0,'31','90','','10','','0','0','','',1,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_FieldModel','IsIgnore','是否忽略',0,0,0,'31','95','','10','','0','0','','',1,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_FieldModel','IsObsolete','是否作废',0,0,0,'31','100','','10','','0','0','','',1,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_FieldModel','IsShow','是否显示',0,0,0,'31','105','','10','','0','0','','',1,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_FieldModel','IsSearch','是否允许搜索',0,0,0,'31','110','','10','','0','0','','',1,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_FieldModel','SrchMode','搜索模式',0,0,0,'21','115','','10','','0','0','','',1,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_FieldModel','IsRefTab','是否引用表',0,0,0,'31','120','','10','','0','0','','',1,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_FieldModel','RefTab','引用表名',0,0,0,'11','125','','10','','50','0','','',1,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_FieldModel','RefField','引用的字段',0,0,0,'11','130','','10','','50','0','','',1,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_FieldModel','RefFields','引用字段清单',0,0,0,'11','135','','10','','500','0','','',1,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_FieldModel','RefFieldDesc','引用字段清单描述',0,0,0,'11','140','','10','','500','0','','',1,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_FieldModel','RefWhere','引用条件',0,0,0,'11','145','','10','','500','0','','',1,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_FieldModel','CreateTime','创建时间',0,0,0,'41','990','','20','','0','0','','',0,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_FieldModel','CreateName','创建人',0,0,0,'11','991','','10','','50','0','','',0,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_FieldModel','ModiTime','修改时间',0,0,0,'41','995','','20','','0','0','','',0,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_FieldModel','ModiName','修改人',0,0,0,'11','998','','10','','50','0','','',0,1,0,0,0,1,1,'10',0,'','','','','')



insert into [[$Schema$]].[Hi_TabModel] (
[TabName],[TabReName],[TabDescript],[TabStoreType],[TabType],[TabCacheType],[TabStatus],[IsSys],[IsEdit],[IsLog],[LogTable],[LogExprireDay]) values('Hi_Domain','Hi_Domain','数据域主表','0','0','0','0',0,1,0,'','0')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_Domain','Domain','数据域名',0,1,1,'11','5','','10','','20','0','','',1,0,1,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_Domain','DomainDesc','数据域描述',0,0,0,'11','10','','10','','100','0','','',1,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_Domain','CreateTime','创建时间',0,0,0,'41','990','','20','','0','0','','',0,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_Domain','CreateName','创建人',0,0,0,'11','991','','10','','50','0','','',0,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_Domain','ModiTime','修改时间',0,0,0,'41','995','','20','','0','0','','',0,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_Domain','ModiName','修改人',0,0,0,'11','998','','10','','50','0','','',0,1,0,0,0,1,1,'10',0,'','','','','')


insert into [[$Schema$]].[Hi_TabModel] (
[TabName],[TabReName],[TabDescript],[TabStoreType],[TabType],[TabCacheType],[TabStatus],[IsSys],[IsEdit],[IsLog],[LogTable],[LogExprireDay]) values('Hi_DataElement','Hi_DataElement','数据域明细表','0','0','0','0',0,1,0,'','0')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_DataElement','Domain','数据域名',0,1,1,'11','5','','10','','20','0','','',1,0,1,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_DataElement','ElementValue','数据域值',0,1,1,'11','10','','10','','50','0','','',1,0,1,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_DataElement','ElementDesc','数据域值描述',0,0,0,'11','15','','10','','100','0','','',1,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_DataElement','SortNum','数据域排序号',0,0,0,'21','20','','10','','0','0','','',1,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_DataElement','CreateTime','创建时间',0,0,0,'41','990','','20','','0','0','','',0,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_DataElement','CreateName','创建人',0,0,0,'11','991','','10','','50','0','','',0,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_DataElement','ModiTime','修改时间',0,0,0,'41','995','','20','','0','0','','',0,1,0,0,0,1,1,'10',0,'','','','','')
insert into [[$Schema$]].[Hi_FieldModel] (
[DbName],[TabName],[FieldName],[FieldDesc],[IsIdentity],[IsPrimary],[IsBllKey],[FieldType],[SortNum],[Regex],[DBDefault],[DefaultValue],[FieldLen],[FieldDec],[SNO],[SNO_NUM],[IsSys],[IsNull],[IsRequire],[IsIgnore],[IsObsolete],[IsShow],[IsSearch],[SrchMode],[IsRefTab],[RefTab],[RefField],[RefFields],[RefFieldDesc],[RefWhere])values('','Hi_DataElement','ModiName','修改人',0,0,0,'11','998','','10','','50','0','','',0,1,0,0,0,1,1,'10',0,'','','','','')



update  dbo.[Hi_TabModel] set [TabStatus] =1,[IsSys]=1 where TabName in ('Hi_TabModel','Hi_FieldModel','Hi_Domain','Hi_DataElement')



set @_effect= 1

select @_effect