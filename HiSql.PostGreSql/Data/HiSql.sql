
DO $$
	DECLARE
	
	  _scount INT := 0;
	BEGIN
	SELECT count(*)
	  INTO _scount
	  FROM pg_tables where tablename ='Hi_TabModel';
	
	if _scount =0 then 
	
		CREATE TABLE  IF NOT EXISTS "[$Schema$]"."Hi_TabModel" (
		"TabName"  varchar(50)  NOT NULL      
		,"TabReName"  varchar(50)  NULL  default ''    
		,"TabDescript"  varchar(100)  NULL  default ''    
		,"TabStoreType" integer  NULL default 0  
		,"TabType" integer  NULL default 0  
		,"TabCacheType" integer  NULL default 0  
		,"TabStatus" integer  NULL default 0  
		,"IsSys" bool  NULL default False   
		,"IsEdit" bool  NULL default False   
		,"IsLog" bool  NULL default False   
		,"LogTable"  varchar(50)  NOT NULL  default ''    
		,"LogExprireDay" integer  NULL default 0  
		,"CreateTime" TIMESTAMP  NULL default current_timestamp   
		,"CreateName"  varchar(50)  NULL  default ''    
		,"ModiTime" TIMESTAMP  NULL default current_timestamp   
		,"ModiName"  varchar(50)  NULL  default ''    
		);
		ALTER TABLE "[$Schema$]"."Hi_TabModel" ADD CONSTRAINT "Hi_TabModel_pkey" PRIMARY KEY ("TabName" );
		COMMENT ON COLUMN "[$Schema$]"."Hi_TabModel"."TabName" IS '表名';
		COMMENT ON COLUMN "[$Schema$]"."Hi_TabModel"."TabReName" IS '表的别名';
		COMMENT ON COLUMN "[$Schema$]"."Hi_TabModel"."TabDescript" IS '表描述';
		COMMENT ON COLUMN "[$Schema$]"."Hi_TabModel"."TabStoreType" IS '表存储方式';
		COMMENT ON COLUMN "[$Schema$]"."Hi_TabModel"."TabType" IS '表类型';
		COMMENT ON COLUMN "[$Schema$]"."Hi_TabModel"."TabCacheType" IS '表的缓存类型';
		COMMENT ON COLUMN "[$Schema$]"."Hi_TabModel"."TabStatus" IS '表状态';
		COMMENT ON COLUMN "[$Schema$]"."Hi_TabModel"."IsSys" IS '是否系统内置表';
		COMMENT ON COLUMN "[$Schema$]"."Hi_TabModel"."IsEdit" IS '是否可编辑';
		COMMENT ON COLUMN "[$Schema$]"."Hi_TabModel"."IsLog" IS '是否开启表日志';
		COMMENT ON COLUMN "[$Schema$]"."Hi_TabModel"."LogTable" IS '日志表名';
		COMMENT ON COLUMN "[$Schema$]"."Hi_TabModel"."LogExprireDay" IS '日志保留天数';
		COMMENT ON COLUMN "[$Schema$]"."Hi_TabModel"."CreateTime" IS '创建时间';
		COMMENT ON COLUMN "[$Schema$]"."Hi_TabModel"."CreateName" IS '创建人';
		COMMENT ON COLUMN "[$Schema$]"."Hi_TabModel"."ModiTime" IS '修改时间';
		COMMENT ON COLUMN "[$Schema$]"."Hi_TabModel"."ModiName" IS '修改人';
		raise notice 'Hi_TabModel 不存在,但已经创建';
	end if;
	
	SELECT count(*)
	  INTO _scount
	  FROM pg_tables where tablename ='Hi_FieldModel';
	
	if _scount =0 then 
		CREATE TABLE  IF NOT EXISTS "[$Schema$]"."Hi_FieldModel" (
		"DbName"  varchar(50)  NOT NULL      
		,"TabName"  varchar(50)  NOT NULL      
		,"FieldName"  varchar(50)  NOT NULL      
		,"FieldDesc"  varchar(100)  NULL  default ''    
		,"IsIdentity" bool  NULL default False   
		,"IsPrimary" bool  NULL default False   
		,"IsBllKey" bool  NULL default False   
		,"FieldType" integer  NULL default 0  
		,"SortNum" integer  NULL default 0  
		,"Regex"  varchar(200)  NULL  default ''    
		,"DBDefault" integer  NULL default 0  
		,"DefaultValue"  varchar(50)  NULL  default ''    
		,"FieldLen" integer  NULL default 0  
		,"FieldDec" integer  NULL default 0  
		,"SNO" char(10)   NULL default ''   
		,"SNO_NUM" char(3)   NULL default ''   
		,"IsSys" bool  NULL default False   
		,"IsNull" bool  NULL default False   
		,"IsRequire" bool  NULL default False   
		,"IsIgnore" bool  NULL default False   
		,"IsObsolete" bool  NULL default False   
		,"IsShow" bool  NULL default False   
		,"IsSearch" bool  NULL default False   
		,"SrchMode" integer  NULL default 0  
		,"IsRefTab" bool  NULL default False   
		,"RefTab"  varchar(50)  NULL  default ''    
		,"RefField"  varchar(50)  NULL  default ''    
		,"RefFields"  varchar(500)  NULL  default ''    
		,"RefFieldDesc"  varchar(500)  NULL  default ''    
		,"RefWhere"  varchar(500)  NULL  default ''    
		,"CreateTime" TIMESTAMP  NULL default current_timestamp   
		,"CreateName"  varchar(50)  NULL  default ''    
		,"ModiTime" TIMESTAMP  NULL default current_timestamp   
		,"ModiName"  varchar(50)  NULL  default ''    



		);
		ALTER TABLE "[$Schema$]"."Hi_FieldModel" ADD CONSTRAINT "Hi_FieldModel_pkey" PRIMARY KEY ("DbName","TabName" ,"FieldName");
		COMMENT ON COLUMN "[$Schema$]"."Hi_FieldModel"."DbName" IS '数据库名';
		COMMENT ON COLUMN "[$Schema$]"."Hi_FieldModel"."TabName" IS '表名';
		COMMENT ON COLUMN "[$Schema$]"."Hi_FieldModel"."FieldName" IS '字段名';
		COMMENT ON COLUMN "[$Schema$]"."Hi_FieldModel"."FieldDesc" IS '字段名描述';
		COMMENT ON COLUMN "[$Schema$]"."Hi_FieldModel"."IsIdentity" IS '是否自增ID';
		COMMENT ON COLUMN "[$Schema$]"."Hi_FieldModel"."IsPrimary" IS '是否主键';
		COMMENT ON COLUMN "[$Schema$]"."Hi_FieldModel"."IsBllKey" IS '是否是业务Key';
		COMMENT ON COLUMN "[$Schema$]"."Hi_FieldModel"."FieldType" IS '字段类型';
		COMMENT ON COLUMN "[$Schema$]"."Hi_FieldModel"."SortNum" IS '字段排序号';
		COMMENT ON COLUMN "[$Schema$]"."Hi_FieldModel"."Regex" IS '正则校验表达式';
		COMMENT ON COLUMN "[$Schema$]"."Hi_FieldModel"."DBDefault" IS '默认值类型';
		COMMENT ON COLUMN "[$Schema$]"."Hi_FieldModel"."DefaultValue" IS '默认值';
		COMMENT ON COLUMN "[$Schema$]"."Hi_FieldModel"."FieldLen" IS '字段长度';
		COMMENT ON COLUMN "[$Schema$]"."Hi_FieldModel"."FieldDec" IS '小数点位数';
		COMMENT ON COLUMN "[$Schema$]"."Hi_FieldModel"."SNO" IS '编号名称';
		COMMENT ON COLUMN "[$Schema$]"."Hi_FieldModel"."SNO_NUM" IS '子编号';
		COMMENT ON COLUMN "[$Schema$]"."Hi_FieldModel"."IsSys" IS '是否系统字段';
		COMMENT ON COLUMN "[$Schema$]"."Hi_FieldModel"."IsNull" IS '是否允许NULL';
		COMMENT ON COLUMN "[$Schema$]"."Hi_FieldModel"."IsRequire" IS '是否必填';
		COMMENT ON COLUMN "[$Schema$]"."Hi_FieldModel"."IsIgnore" IS '是否忽略';
		COMMENT ON COLUMN "[$Schema$]"."Hi_FieldModel"."IsObsolete" IS '是否作废';
		COMMENT ON COLUMN "[$Schema$]"."Hi_FieldModel"."IsShow" IS '是否显示';
		COMMENT ON COLUMN "[$Schema$]"."Hi_FieldModel"."IsSearch" IS '是否允许搜索';
		COMMENT ON COLUMN "[$Schema$]"."Hi_FieldModel"."SrchMode" IS '搜索模式';
		COMMENT ON COLUMN "[$Schema$]"."Hi_FieldModel"."IsRefTab" IS '是否引用表';
		COMMENT ON COLUMN "[$Schema$]"."Hi_FieldModel"."RefTab" IS '引用表名';
		COMMENT ON COLUMN "[$Schema$]"."Hi_FieldModel"."RefField" IS '引用的字段';
		COMMENT ON COLUMN "[$Schema$]"."Hi_FieldModel"."RefFields" IS '引用字段清单';
		COMMENT ON COLUMN "[$Schema$]"."Hi_FieldModel"."RefFieldDesc" IS '引用字段清单描述';
		COMMENT ON COLUMN "[$Schema$]"."Hi_FieldModel"."RefWhere" IS '引用条件';
		COMMENT ON COLUMN "[$Schema$]"."Hi_FieldModel"."CreateTime" IS '创建时间';
		COMMENT ON COLUMN "[$Schema$]"."Hi_FieldModel"."CreateName" IS '创建人';
		COMMENT ON COLUMN "[$Schema$]"."Hi_FieldModel"."ModiTime" IS '修改时间';
		COMMENT ON COLUMN "[$Schema$]"."Hi_FieldModel"."ModiName" IS '修改人';
	end if;
	
	SELECT count(*)
	  INTO _scount
	  FROM pg_tables where tablename ='Hi_Domain';
	
	if _scount =0 then 
		CREATE TABLE  IF NOT EXISTS "[$Schema$]"."Hi_Domain" (
		"Domain"  varchar(20)  NOT NULL      
		,"DomainDesc"  varchar(100)  NULL  default ''    
		,"CreateTime" TIMESTAMP  NULL default current_timestamp   
		,"CreateName"  varchar(50)  NULL  default ''    
		,"ModiTime" TIMESTAMP  NULL default current_timestamp   
		,"ModiName"  varchar(50)  NULL  default ''    



		);
		ALTER TABLE "[$Schema$]"."Hi_Domain" ADD CONSTRAINT "Hi_Domain_pkey" PRIMARY KEY ("Domain" );
		COMMENT ON COLUMN "[$Schema$]"."Hi_Domain"."Domain" IS '数据域名';
		COMMENT ON COLUMN "[$Schema$]"."Hi_Domain"."DomainDesc" IS '数据域描述';
		COMMENT ON COLUMN "[$Schema$]"."Hi_Domain"."CreateTime" IS '创建时间';
		COMMENT ON COLUMN "[$Schema$]"."Hi_Domain"."CreateName" IS '创建人';
		COMMENT ON COLUMN "[$Schema$]"."Hi_Domain"."ModiTime" IS '修改时间';
		COMMENT ON COLUMN "[$Schema$]"."Hi_Domain"."ModiName" IS '修改人';
	end if;
	
	SELECT count(*)
	  INTO _scount
	  FROM pg_tables where tablename ='Hi_DataElement';
	if _scount =0 then 
		
		CREATE TABLE  IF NOT EXISTS "[$Schema$]"."Hi_DataElement" (
		"Domain"  varchar(20)  NOT NULL      
		,"ElementValue"  varchar(50)  NOT NULL      
		,"ElementDesc"  varchar(100)  NULL  default ''    
		,"SortNum" integer  NULL default 0  
		,"CreateTime" TIMESTAMP  NULL default current_timestamp   
		,"CreateName"  varchar(50)  NULL  default ''    
		,"ModiTime" TIMESTAMP  NULL default current_timestamp   
		,"ModiName"  varchar(50)  NULL  default ''    



		);
		ALTER TABLE "[$Schema$]"."Hi_DataElement" ADD CONSTRAINT "Hi_DataElement_pkey" PRIMARY KEY ("Domain" ,
		"ElementValue" 

		);
		COMMENT ON COLUMN "[$Schema$]"."Hi_DataElement"."Domain" IS '数据域名';
		COMMENT ON COLUMN "[$Schema$]"."Hi_DataElement"."ElementValue" IS '数据域值';
		COMMENT ON COLUMN "[$Schema$]"."Hi_DataElement"."ElementDesc" IS '数据域值描述';
		COMMENT ON COLUMN "[$Schema$]"."Hi_DataElement"."SortNum" IS '数据域排序号';
		COMMENT ON COLUMN "[$Schema$]"."Hi_DataElement"."CreateTime" IS '创建时间';
		COMMENT ON COLUMN "[$Schema$]"."Hi_DataElement"."CreateName" IS '创建人';
		COMMENT ON COLUMN "[$Schema$]"."Hi_DataElement"."ModiTime" IS '修改时间';
		COMMENT ON COLUMN "[$Schema$]"."Hi_DataElement"."ModiName" IS '修改人';
	end if;
	
	delete from "[$Schema$]"."Hi_TabModel" where "TabName"='Hi_TabModel';
	delete from  "[$Schema$]"."Hi_FieldModel" where "TabName"='Hi_TabModel';
	delete from "[$Schema$]"."Hi_TabModel" where "TabName"='Hi_FieldModel';
	delete from  "[$Schema$]"."Hi_FieldModel" where "TabName"='Hi_FieldModel';
	delete from "[$Schema$]"."Hi_TabModel" where "TabName"='Hi_Domain';
	delete from  "[$Schema$]"."Hi_FieldModel" where "TabName"='Hi_Domain';
	delete from "[$Schema$]"."Hi_TabModel" where "TabName"='Hi_DataElement';
	delete from  "[$Schema$]"."Hi_FieldModel" where "TabName"='Hi_DataElement';
	
	
	
	
	
	insert into  "[$Schema$]"."Hi_TabModel" (
"TabName","TabReName","TabDescript","TabStoreType","TabType","TabCacheType","TabStatus","IsSys","IsEdit","IsLog","LogTable","LogExprireDay") values('Hi_TabModel','Hi_TabModel','表结构信息主表','0','0','0','0',False,True,False,'','0');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_TabModel','TabName','表名',False,True,True,'11','1','','10','','50','0','','',True,False,True,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_TabModel','TabReName','表的别名',False,False,False,'11','5','','10','','50','0','','',False,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_TabModel','TabDescript','表描述',False,False,False,'11','5','','10','','100','0','','',False,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_TabModel','TabStoreType','表存储方式',False,False,False,'21','10','','10','','0','0','','',False,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_TabModel','TabType','表类型',False,False,False,'21','15','','10','','0','0','','',False,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_TabModel','TabCacheType','表的缓存类型',False,False,False,'21','20','','10','','0','0','','',False,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_TabModel','TabStatus','表状态',False,False,False,'21','25','','10','','0','0','','',False,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_TabModel','IsSys','是否系统内置表',False,False,False,'31','30','','10','','0','0','','',True,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_TabModel','IsEdit','是否可编辑',False,False,False,'31','35','','10','','0','0','','',True,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_TabModel','IsLog','是否开启表日志',False,False,False,'31','40','','10','','0','0','','',True,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_TabModel','LogTable','日志表名',False,False,False,'11','45','','10','','50','0','','',True,False,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_TabModel','LogExprireDay','日志保留天数',False,False,False,'21','50','','10','','0','0','','',True,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_TabModel','CreateTime','创建时间',False,False,False,'41','990','','20','','0','0','','',False,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_TabModel','CreateName','创建人',False,False,False,'11','991','','10','','50','0','','',False,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_TabModel','ModiTime','修改时间',False,False,False,'41','995','','20','','0','0','','',False,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_TabModel','ModiName','修改人',False,False,False,'11','998','','10','','50','0','','',False,True,False,False,False,True,True,'10',False,'','','','','');



insert into  "[$Schema$]"."Hi_TabModel" (
"TabName","TabReName","TabDescript","TabStoreType","TabType","TabCacheType","TabStatus","IsSys","IsEdit","IsLog","LogTable","LogExprireDay") values('Hi_FieldModel','Hi_FieldModel','表结构信息字段表','0','0','0','0',False,True,False,'','0');

insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_FieldModel','DbName','数据库名',False,True,True,'11','4','','10','','50','0','','',True,False,True,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_FieldModel','TabName','表名',False,True,True,'11','5','','10','','50','0','','',True,False,True,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_FieldModel','FieldName','字段名',False,True,True,'11','10','','10','','50','0','','',True,False,True,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_FieldModel','FieldDesc','字段名描述',False,False,False,'11','15','','10','','100','0','','',True,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_FieldModel','IsIdentity','是否自增ID',False,False,False,'31','20','','10','','0','0','','',True,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_FieldModel','IsPrimary','是否主键',False,False,False,'31','25','','10','','0','0','','',True,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_FieldModel','IsBllKey','是否是业务Key',False,False,False,'31','30','','10','','0','0','','',True,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_FieldModel','FieldType','字段类型',False,False,False,'21','35','','10','','0','0','','',True,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_FieldModel','SortNum','字段排序号',False,False,False,'21','40','','10','','0','0','','',True,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_FieldModel','Regex','正则校验表达式',False,False,False,'11','45','','10','','200','0','','',True,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_FieldModel','DBDefault','默认值类型',False,False,False,'21','50','','10','','0','0','','',True,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_FieldModel','DefaultValue','默认值',False,False,False,'11','55','','10','','50','0','','',True,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_FieldModel','FieldLen','字段长度',False,False,False,'21','60','','10','','0','0','','',True,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_FieldModel','FieldDec','小数点位数',False,False,False,'21','65','','10','','0','0','','',True,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_FieldModel','SNO','编号名称',False,False,False,'12','70','','10','','10','0','','',True,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_FieldModel','SNO_NUM','子编号',False,False,False,'12','75','','10','','3','0','','',True,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_FieldModel','IsSys','是否系统字段',False,False,False,'31','80','','10','','0','0','','',True,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_FieldModel','IsNull','是否允许NULL',False,False,False,'31','85','','10','','0','0','','',True,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_FieldModel','IsRequire','是否必填',False,False,False,'31','90','','10','','0','0','','',True,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_FieldModel','IsIgnore','是否忽略',False,False,False,'31','95','','10','','0','0','','',True,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_FieldModel','IsObsolete','是否作废',False,False,False,'31','100','','10','','0','0','','',True,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_FieldModel','IsShow','是否显示',False,False,False,'31','105','','10','','0','0','','',True,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_FieldModel','IsSearch','是否允许搜索',False,False,False,'31','110','','10','','0','0','','',True,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_FieldModel','SrchMode','搜索模式',False,False,False,'21','115','','10','','0','0','','',True,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_FieldModel','IsRefTab','是否引用表',False,False,False,'31','120','','10','','0','0','','',True,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_FieldModel','RefTab','引用表名',False,False,False,'11','125','','10','','50','0','','',True,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_FieldModel','RefField','引用的字段',False,False,False,'11','130','','10','','50','0','','',True,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_FieldModel','RefFields','引用字段清单',False,False,False,'11','135','','10','','500','0','','',True,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_FieldModel','RefFieldDesc','引用字段清单描述',False,False,False,'11','140','','10','','500','0','','',True,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_FieldModel','RefWhere','引用条件',False,False,False,'11','145','','10','','500','0','','',True,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_FieldModel','CreateTime','创建时间',False,False,False,'41','990','','20','','0','0','','',False,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_FieldModel','CreateName','创建人',False,False,False,'11','991','','10','','50','0','','',False,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_FieldModel','ModiTime','修改时间',False,False,False,'41','995','','20','','0','0','','',False,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_FieldModel','ModiName','修改人',False,False,False,'11','998','','10','','50','0','','',False,True,False,False,False,True,True,'10',False,'','','','','');



insert into  "[$Schema$]"."Hi_TabModel" (
"TabName","TabReName","TabDescript","TabStoreType","TabType","TabCacheType","TabStatus","IsSys","IsEdit","IsLog","LogTable","LogExprireDay") values('Hi_Domain','Hi_Domain','数据域主表','0','0','0','0',False,True,False,'','0');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_Domain','Domain','数据域名',False,True,True,'11','5','','10','','20','0','','',True,False,True,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_Domain','DomainDesc','数据域描述',False,False,False,'11','10','','10','','100','0','','',True,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_Domain','CreateTime','创建时间',False,False,False,'41','990','','20','','0','0','','',False,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_Domain','CreateName','创建人',False,False,False,'11','991','','10','','50','0','','',False,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_Domain','ModiTime','修改时间',False,False,False,'41','995','','20','','0','0','','',False,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_Domain','ModiName','修改人',False,False,False,'11','998','','10','','50','0','','',False,True,False,False,False,True,True,'10',False,'','','','','');


insert into  "[$Schema$]"."Hi_TabModel" (
"TabName","TabReName","TabDescript","TabStoreType","TabType","TabCacheType","TabStatus","IsSys","IsEdit","IsLog","LogTable","LogExprireDay") values('Hi_DataElement','Hi_DataElement','数据域明细表','0','0','0','0',False,True,False,'','0');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_DataElement','Domain','数据域名',False,True,True,'11','5','','10','','20','0','','',True,False,True,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_DataElement','ElementValue','数据域值',False,True,True,'11','10','','10','','50','0','','',True,False,True,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_DataElement','ElementDesc','数据域值描述',False,False,False,'11','15','','10','','100','0','','',True,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_DataElement','SortNum','数据域排序号',False,False,False,'21','20','','10','','0','0','','',True,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_DataElement','CreateTime','创建时间',False,False,False,'41','990','','20','','0','0','','',False,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_DataElement','CreateName','创建人',False,False,False,'11','991','','10','','50','0','','',False,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_DataElement','ModiTime','修改时间',False,False,False,'41','995','','20','','0','0','','',False,True,False,False,False,True,True,'10',False,'','','','','');
insert into  "[$Schema$]"."Hi_FieldModel" (
"DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")values('','Hi_DataElement','ModiName','修改人',False,False,False,'11','998','','10','','50','0','','',False,True,False,False,False,True,True,'10',False,'','','','','');

update  "[$Schema$]"."Hi_TabModel"   set "TabStatus" =1, "IsSys"=True  where "TabName" in ('Hi_TabModel','Hi_FieldModel','Hi_Domain','Hi_DataElement');

	END 
$$

