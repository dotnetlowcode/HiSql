/*hisql hana 初始化安装包*/

DO BEGIN
  DECLARE _COUNT INT:=0;
  DECLARE _STATUS INT:=0;
   SELECT COUNT(*) INTO _COUNT  FROM OBJECTS  WHERE OBJECT_TYPE='TABLE' AND OBJECT_NAME='Hi_TabModel';
   IF :_COUNT = 0 THEN
      CREATE COLUMN TABLE  "[$Schema$]"."Hi_TabModel" (
           "TabName" NVARCHAR(50) NOT NULL   COMMENT '表名' , 
			"TabReName" NVARCHAR(50)   DEFAULT '' COMMENT '表的别名' , 
			"TabDescript" NVARCHAR(100)   DEFAULT '' COMMENT '表描述' , 
			"TabStoreType" INTEGER   DEFAULT 0 COMMENT '表存储方式' ,
			"TabType" INTEGER   DEFAULT 0 COMMENT '表类型' ,
			"TabCacheType" INTEGER   DEFAULT 0 COMMENT '表的缓存类型' ,
			"TabStatus" INTEGER   DEFAULT 0 COMMENT '表状态' ,
			"IsSys"  BOOLEAN    DEFAULT FALSE COMMENT '是否系统内置表' ,
			"IsEdit"  BOOLEAN    DEFAULT FALSE COMMENT '是否可编辑' ,
			"IsLog"  BOOLEAN    DEFAULT FALSE COMMENT '是否开启表日志' ,
			"LogTable" NVARCHAR(50) NOT NULL  DEFAULT '' COMMENT '日志表名' , 
			"LogExprireDay" INTEGER   DEFAULT 0 COMMENT '日志保留天数' ,
			"CreateTime"  TIMESTAMP    DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间' ,
			"CreateName" NVARCHAR(50)   DEFAULT '' COMMENT '创建人' , 
			"ModiTime"  TIMESTAMP    DEFAULT CURRENT_TIMESTAMP COMMENT '修改时间' ,
			"ModiName" NVARCHAR(50)   DEFAULT '' COMMENT '修改人' , 

           PRIMARY KEY("TabName" 
)

       )UNLOAD PRIORITY 5 AUTO MERGE;  
      
       _STATUS := :_STATUS + 1;
   END IF;
   
   SELECT COUNT(*) INTO _COUNT  FROM OBJECTS  WHERE OBJECT_TYPE='TABLE' AND OBJECT_NAME='Hi_FieldModel';
   IF :_COUNT = 0 THEN
      CREATE COLUMN TABLE  "[$Schema$]"."Hi_FieldModel" (
           "TabName" NVARCHAR(50) NOT NULL   COMMENT '表名' , 
		"FieldName" NVARCHAR(50) NOT NULL   COMMENT '字段名' , 
		"FieldDesc" NVARCHAR(100)   DEFAULT '' COMMENT '字段名描述' , 
		"IsIdentity"  BOOLEAN    DEFAULT FALSE COMMENT '是否自增ID' ,
		"IsPrimary"  BOOLEAN    DEFAULT FALSE COMMENT '是否主键' ,
		"IsBllKey"  BOOLEAN    DEFAULT FALSE COMMENT '是否是业务KEY' ,
		"FieldType" INTEGER   DEFAULT 0 COMMENT '字段类型' ,
		"SortNum" INTEGER   DEFAULT 0 COMMENT '字段排序号' ,
		"Regex" NVARCHAR(200)   DEFAULT '' COMMENT '正则校验表达式' , 
		"DBDefault" INTEGER   DEFAULT 0 COMMENT '默认值类型' ,
		"DefaultValue" NVARCHAR(50)   DEFAULT '' COMMENT '默认值' , 
		"FieldLen" INTEGER   DEFAULT 0 COMMENT '字段长度' ,
		"FieldDec" INTEGER   DEFAULT 0 COMMENT '小数点位数' ,
		"SNO" NCHAR(10)   DEFAULT '' COMMENT '编号名称' , 
		"SNO_NUM" NCHAR(3)   DEFAULT '' COMMENT '子编号' , 
		"IsSys"  BOOLEAN    DEFAULT FALSE COMMENT '是否系统字段' ,
		"IsNull"  BOOLEAN    DEFAULT FALSE COMMENT '是否允许NULL' ,
		"IsRequire"  BOOLEAN    DEFAULT FALSE COMMENT '是否必填' ,
		"IsIgnore"  BOOLEAN    DEFAULT FALSE COMMENT '是否忽略' ,
		"IsObsolete"  BOOLEAN    DEFAULT FALSE COMMENT '是否作废' ,
		"IsShow"  BOOLEAN    DEFAULT FALSE COMMENT '是否显示' ,
		"IsSearch"  BOOLEAN    DEFAULT FALSE COMMENT '是否允许搜索' ,
		"SrchMode" INTEGER   DEFAULT 0 COMMENT '搜索模式' ,
		"IsRefTab"  BOOLEAN    DEFAULT FALSE COMMENT '是否引用表' ,
		"RefTab" NVARCHAR(50)   DEFAULT '' COMMENT '引用表名' , 
		"RefField" NVARCHAR(50)   DEFAULT '' COMMENT '引用的字段' , 
		"RefFields" NVARCHAR(500)   DEFAULT '' COMMENT '引用字段清单' , 
		"RefFieldDesc" NVARCHAR(500)   DEFAULT '' COMMENT '引用字段清单描述' , 
		"RefWhere" NVARCHAR(500)   DEFAULT '' COMMENT '引用条件' , 
		"CreateTime"  TIMESTAMP    DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间' ,
		"CreateName" NVARCHAR(50)   DEFAULT '' COMMENT '创建人' , 
		"ModiTime"  TIMESTAMP    DEFAULT CURRENT_TIMESTAMP COMMENT '修改时间' ,
		"ModiName" NVARCHAR(50)   DEFAULT '' COMMENT '修改人' , 

           PRIMARY KEY("TabName" ,
			"FieldName" 
			)

       )UNLOAD PRIORITY 5 AUTO MERGE;  
      
       
       _STATUS := :_STATUS + 1;
   END IF;
   
   
   SELECT COUNT(*) INTO _COUNT  FROM OBJECTS  WHERE OBJECT_TYPE='TABLE' AND OBJECT_NAME='Hi_Domain';
   IF :_COUNT = 0 THEN
      CREATE COLUMN TABLE  "[$Schema$]"."Hi_Domain" (
           "Domain" NVARCHAR(10) NOT NULL   COMMENT '数据域名' , 
		"DomainDesc" NVARCHAR(100)   DEFAULT '' COMMENT '数据域描述' , 
		"CreateTime"  TIMESTAMP    DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间' ,
		"CreateName" NVARCHAR(50)   DEFAULT '' COMMENT '创建人' , 
		"ModiTime"  TIMESTAMP    DEFAULT CURRENT_TIMESTAMP COMMENT '修改时间' ,
		"ModiName" NVARCHAR(50)   DEFAULT '' COMMENT '修改人' , 
		
		           PRIMARY KEY("Domain" 
		)

       )UNLOAD PRIORITY 5 AUTO MERGE;  
   
   
   END IF;
   
   SELECT COUNT(*) INTO _COUNT  FROM OBJECTS  WHERE OBJECT_TYPE='TABLE' AND OBJECT_NAME='Hi_DataElement';
   IF :_COUNT = 0 THEN
      CREATE COLUMN TABLE  "[$Schema$]"."Hi_DataElement" (
           "Domain" NVARCHAR(10) NOT NULL   COMMENT '数据域名' , 
	"ElementValue" NVARCHAR(50) NOT NULL   COMMENT '数据域值' , 
	"ElementDesc" NVARCHAR(100)   DEFAULT '' COMMENT '数据域值描述' , 
	"SortNum" INTEGER   DEFAULT 0 COMMENT '数据域排序号' ,
	"CreateTime"  TIMESTAMP    DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间' ,
	"CreateName" NVARCHAR(50)   DEFAULT '' COMMENT '创建人' , 
	"ModiTime"  TIMESTAMP    DEFAULT CURRENT_TIMESTAMP COMMENT '修改时间' ,
	"ModiName" NVARCHAR(50)   DEFAULT '' COMMENT '修改人' , 
	
	           PRIMARY KEY("Domain" ,
	"ElementValue" 
	)

       )UNLOAD PRIORITY 5 AUTO MERGE;  
       
    END IF;
   
   
   
   DELETE FROM  "[$Schema$]"."Hi_TabModel" WHERE "TabName"='Hi_TabModel';
  DELETE FROM  "[$Schema$]"."Hi_FieldModel" WHERE "TabName"='Hi_TabModel';
  DELETE FROM  "[$Schema$]"."Hi_TabModel" WHERE "TabName"='Hi_FieldModel';
  DELETE FROM  "[$Schema$]"."Hi_FieldModel" WHERE "TabName"='Hi_FieldModel';
  DELETE FROM  "[$Schema$]"."Hi_TabModel" WHERE "TabName"='Hi_Domain';
  DELETE FROM  "[$Schema$]"."Hi_FieldModel" WHERE "TabName"='Hi_Domain';
  DELETE FROM  "[$Schema$]"."Hi_TabModel" WHERE "TabName"='Hi_DataElement';
  DELETE FROM  "[$Schema$]"."Hi_FieldModel" WHERE "TabName"='Hi_DataElement';
      
       INSERT INTO "[$Schema$]"."Hi_TabModel" (
"TabName","TabReName","TabDescript","TabStoreType","TabType","TabCacheType","TabStatus","IsSys","IsEdit","IsLog","LogTable","LogExprireDay") VALUES('Hi_TabModel','Hi_TabModel','','0','0','0','0',FALSE,TRUE,FALSE,'','0');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_TabModel','TabName','表名',FALSE,TRUE,TRUE,'11','1','','10','','50','0','','',TRUE,FALSE,TRUE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_TabModel','TabReName','表的别名',FALSE,FALSE,FALSE,'11','5','','10','','50','0','','',FALSE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_TabModel','TabDescript','表描述',FALSE,FALSE,FALSE,'11','5','','10','','100','0','','',FALSE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_TabModel','TabStoreType','表存储方式',FALSE,FALSE,FALSE,'21','10','','10','','0','0','','',FALSE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_TabModel','TabType','表类型',FALSE,FALSE,FALSE,'21','15','','10','','0','0','','',FALSE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_TabModel','TabCacheType','表的缓存类型',FALSE,FALSE,FALSE,'21','20','','10','','0','0','','',FALSE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_TabModel','TabStatus','表状态',FALSE,FALSE,FALSE,'21','25','','10','','0','0','','',FALSE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_TabModel','IsSys','是否系统内置表',FALSE,FALSE,FALSE,'31','30','','10','','0','0','','',TRUE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_TabModel','IsEdit','是否可编辑',FALSE,FALSE,FALSE,'31','35','','10','','0','0','','',TRUE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_TabModel','IsLog','是否开启表日志',FALSE,FALSE,FALSE,'31','40','','10','','0','0','','',TRUE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_TabModel','LogTable','日志表名',FALSE,FALSE,FALSE,'11','45','','10','','50','0','','',TRUE,FALSE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_TabModel','LogExprireDay','日志保留天数',FALSE,FALSE,FALSE,'21','50','','10','','0','0','','',TRUE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_TabModel','CreateTime','创建时间',FALSE,FALSE,FALSE,'41','990','','20','','0','0','','',FALSE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_TabModel','CreateName','创建人',FALSE,FALSE,FALSE,'11','991','','10','','50','0','','',FALSE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_TabModel','ModiTime','修改时间',FALSE,FALSE,FALSE,'41','995','','20','','0','0','','',FALSE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_TabModel','ModiName','修改人',FALSE,FALSE,FALSE,'11','998','','10','','50','0','','',FALSE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
   
   
   
 INSERT INTO "[$Schema$]"."Hi_TabModel" (
"TabName","TabReName","TabDescript","TabStoreType","TabType","TabCacheType","TabStatus","IsSys","IsEdit","IsLog","LogTable","LogExprireDay") VALUES('Hi_FieldModel','Hi_FieldModel','','0','0','0','0',FALSE,TRUE,FALSE,'','0');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_FieldModel','TabName','表名',FALSE,TRUE,TRUE,'11','5','','10','','50','0','','',TRUE,FALSE,TRUE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_FieldModel','FieldName','字段名',FALSE,TRUE,TRUE,'11','10','','10','','50','0','','',TRUE,FALSE,TRUE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_FieldModel','FieldDesc','字段名描述',FALSE,FALSE,FALSE,'11','15','','10','','100','0','','',TRUE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_FieldModel','IsIdentity','是否自增ID',FALSE,FALSE,FALSE,'31','20','','10','','0','0','','',TRUE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_FieldModel','IsPrimary','是否主键',FALSE,FALSE,FALSE,'31','25','','10','','0','0','','',TRUE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_FieldModel','IsBllKey','是否是业务KEY',FALSE,FALSE,FALSE,'31','30','','10','','0','0','','',TRUE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_FieldModel','FieldType','字段类型',FALSE,FALSE,FALSE,'21','35','','10','','0','0','','',TRUE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_FieldModel','SortNum','字段排序号',FALSE,FALSE,FALSE,'21','40','','10','','0','0','','',TRUE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_FieldModel','Regex','正则校验表达式',FALSE,FALSE,FALSE,'11','45','','10','','200','0','','',TRUE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_FieldModel','DBDefault','默认值类型',FALSE,FALSE,FALSE,'21','50','','10','','0','0','','',TRUE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_FieldModel','DefaultValue','默认值',FALSE,FALSE,FALSE,'11','55','','10','','50','0','','',TRUE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_FieldModel','FieldLen','字段长度',FALSE,FALSE,FALSE,'21','60','','10','','0','0','','',TRUE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_FieldModel','FieldDec','小数点位数',FALSE,FALSE,FALSE,'21','65','','10','','0','0','','',TRUE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_FieldModel','SNO','编号名称',FALSE,FALSE,FALSE,'13','70','','10','','10','0','','',TRUE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_FieldModel','SNO_NUM','子编号',FALSE,FALSE,FALSE,'13','75','','10','','3','0','','',TRUE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_FieldModel','IsSys','是否系统字段',FALSE,FALSE,FALSE,'31','80','','10','','0','0','','',TRUE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_FieldModel','IsNull','是否允许NULL',FALSE,FALSE,FALSE,'31','85','','10','','0','0','','',TRUE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_FieldModel','IsRequire','是否必填',FALSE,FALSE,FALSE,'31','90','','10','','0','0','','',TRUE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_FieldModel','IsIgnore','是否忽略',FALSE,FALSE,FALSE,'31','95','','10','','0','0','','',TRUE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_FieldModel','IsObsolete','是否作废',FALSE,FALSE,FALSE,'31','100','','10','','0','0','','',TRUE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_FieldModel','IsShow','是否显示',FALSE,FALSE,FALSE,'31','105','','10','','0','0','','',TRUE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_FieldModel','IsSearch','是否允许搜索',FALSE,FALSE,FALSE,'31','110','','10','','0','0','','',TRUE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_FieldModel','SrchMode','搜索模式',FALSE,FALSE,FALSE,'21','115','','10','','0','0','','',TRUE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_FieldModel','IsRefTab','是否引用表',FALSE,FALSE,FALSE,'31','120','','10','','0','0','','',TRUE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_FieldModel','RefTab','引用表名',FALSE,FALSE,FALSE,'11','125','','10','','50','0','','',TRUE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_FieldModel','RefField','引用的字段',FALSE,FALSE,FALSE,'11','130','','10','','50','0','','',TRUE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_FieldModel','RefFields','引用字段清单',FALSE,FALSE,FALSE,'11','135','','10','','500','0','','',TRUE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_FieldModel','RefFieldDesc','引用字段清单描述',FALSE,FALSE,FALSE,'11','140','','10','','500','0','','',TRUE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_FieldModel','RefWhere','引用条件',FALSE,FALSE,FALSE,'11','145','','10','','500','0','','',TRUE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_FieldModel','CreateTime','创建时间',FALSE,FALSE,FALSE,'41','990','','20','','0','0','','',FALSE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_FieldModel','CreateName','创建人',FALSE,FALSE,FALSE,'11','991','','10','','50','0','','',FALSE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_FieldModel','ModiTime','修改时间',FALSE,FALSE,FALSE,'41','995','','20','','0','0','','',FALSE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_FieldModel','ModiName','修改人',FALSE,FALSE,FALSE,'11','998','','10','','50','0','','',FALSE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
 
 
 
 INSERT INTO "[$Schema$]"."Hi_TabModel" (
"TabName","TabReName","TabDescript","TabStoreType","TabType","TabCacheType","TabStatus","IsSys","IsEdit","IsLog","LogTable","LogExprireDay") VALUES('Hi_Domain','Hi_Domain','数据域主表','0','0','0','0',FALSE,TRUE,FALSE,'','0');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_Domain','Domain','数据域名',FALSE,TRUE,TRUE,'11','5','','10','','10','0','','',TRUE,FALSE,TRUE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_Domain','DomainDesc','数据域描述',FALSE,FALSE,FALSE,'11','10','','10','','100','0','','',TRUE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_Domain','CreateTime','创建时间',FALSE,FALSE,FALSE,'41','990','','20','','0','0','','',FALSE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_Domain','CreateName','创建人',FALSE,FALSE,FALSE,'11','991','','10','','50','0','','',FALSE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_Domain','ModiTime','修改时间',FALSE,FALSE,FALSE,'41','995','','20','','0','0','','',FALSE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_Domain','ModiName','修改人',FALSE,FALSE,FALSE,'11','998','','10','','50','0','','',FALSE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
 
 
 
 
 INSERT INTO "[$Schema$]"."Hi_TabModel" (
"TabName","TabReName","TabDescript","TabStoreType","TabType","TabCacheType","TabStatus","IsSys","IsEdit","IsLog","LogTable","LogExprireDay") VALUES('Hi_DataElement','Hi_DataElement','数据域明细表','0','0','0','0',FALSE,TRUE,FALSE,'','0');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_DataElement','Domain','数据域名',FALSE,TRUE,TRUE,'11','5','','10','','10','0','','',TRUE,FALSE,TRUE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_DataElement','ElementValue','数据域值',FALSE,TRUE,TRUE,'11','10','','10','','50','0','','',TRUE,FALSE,TRUE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_DataElement','ElementDesc','数据域值描述',FALSE,FALSE,FALSE,'11','15','','10','','100','0','','',TRUE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_DataElement','SortNum','数据域排序号',FALSE,FALSE,FALSE,'21','20','','10','','0','0','','',TRUE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_DataElement','CreateTime','创建时间',FALSE,FALSE,FALSE,'41','990','','20','','0','0','','',FALSE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_DataElement','CreateName','创建人',FALSE,FALSE,FALSE,'11','991','','10','','50','0','','',FALSE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_DataElement','ModiTime','修改时间',FALSE,FALSE,FALSE,'41','995','','20','','0','0','','',FALSE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
INSERT INTO "[$Schema$]"."Hi_FieldModel" (
"TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('Hi_DataElement','ModiName','修改人',FALSE,FALSE,FALSE,'11','998','','10','','50','0','','',FALSE,TRUE,FALSE,FALSE,FALSE,TRUE,TRUE,'10',FALSE,'','','','','');
 
 UPDATE  "[$Schema$]"."Hi_TabModel" SET "TabStatus" =1,"IsSys"=TRUE WHERE "TabReName" IN ('Hi_TabModel','Hi_FieldModel','Hi_Domain','Hi_DataElement');


END;
