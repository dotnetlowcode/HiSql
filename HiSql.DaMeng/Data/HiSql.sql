DECLARE 
  V_NUMBER INTEGER;
   V_SECOUT INTEGER;
BEGIN
   
   select count(*) into V_NUMBER from all_tables where table_name ='Hi_TabModel' and owner='[$Schema$]';
   IF V_NUMBER = 0 THEN
   EXECUTE IMMEDIATE  'CREATE TABLE [$Schema$].Hi_TabModel('
        || '"TabName"  NVARCHAR2 (50) NOT NULL    ,'
 || '"TabReName"  NVARCHAR2 (50)   DEFAULT ''''  ,'
 || '"TabDescript"  NVARCHAR2 (100)   DEFAULT ''''  ,'
 || '"TabStoreType"  INTEGER    DEFAULT 0  ,'
 || '"TabType"  INTEGER    DEFAULT 0  ,'
 || '"TabCacheType"  INTEGER    DEFAULT 0  ,'
 || '"TabStatus"  INTEGER    DEFAULT 0  ,'
 || '"IsSys"  NUMBER(1)     DEFAULT 0  ,'
 || '"IsEdit"  NUMBER(1)     DEFAULT 0  ,'
 || '"IsLog"  NUMBER(1)     DEFAULT 0  ,'
 || '"LogTable"  NVARCHAR2 (50)   DEFAULT ''''  ,'
 || '"LogExprireDay"  INTEGER    DEFAULT 0  ,'
 || '"CreateTime"  TIMESTAMP     DEFAULT SYSTIMESTAMP  ,'
 || '"CreateName"  NVARCHAR2 (50)   DEFAULT ''''  ,'
 || '"ModiTime"  TIMESTAMP     DEFAULT SYSTIMESTAMP  ,'
 || '"ModiName"  NVARCHAR2 (50)   DEFAULT ''''  '

   || ') ';
   
    EXECUTE IMMEDIATE 'ALTER TABLE [$Schema$].Hi_TabModel  ADD CONSTRAINT PK_Hi_TabModel_FC3020417A514A30B206EFF831604FC2 PRIMARY KEY ("TabName" )';
   END IF;
   
   select count(*) into V_NUMBER from all_tables where table_name ='Hi_FieldModel' and owner='[$Schema$]';
   IF V_NUMBER = 0 THEN
    EXECUTE IMMEDIATE  'CREATE TABLE [$Schema$].Hi_FieldModel('
|| '"DbName"  NVARCHAR2 (50) NOT NULL    ,'
|| '"TabName"  NVARCHAR2 (50) NOT NULL    ,'
 || '"FieldName"  NVARCHAR2 (50) NOT NULL    ,'
 || '"FieldDesc"  NVARCHAR2 (100)   DEFAULT ''''  ,'
 || '"IsIdentity"  NUMBER(1)     DEFAULT 0  ,'
 || '"IsPrimary"  NUMBER(1)     DEFAULT 0  ,'
 || '"IsBllKey"  NUMBER(1)     DEFAULT 0  ,'
 || '"FieldType"  INTEGER    DEFAULT 0  ,'
 || '"SortNum"  INTEGER    DEFAULT 0  ,'
 || '"Regex"  NVARCHAR2 (200)   DEFAULT ''''  ,'
 || '"DBDefault"  INTEGER    DEFAULT 0  ,'
 || '"DefaultValue"  NVARCHAR2 (50)   DEFAULT ''''  ,'
 || '"FieldLen"  INTEGER    DEFAULT 0  ,'
 || '"FieldDec"  INTEGER    DEFAULT 0  ,'
 || '"SNO"  NCHAR (10)  DEFAULT ''''   ,'
 || '"SNO_NUM"  NCHAR (3)  DEFAULT ''''   ,'
 || '"IsSys"  NUMBER(1)     DEFAULT 0  ,'
 || '"IsNull"  NUMBER(1)     DEFAULT 0  ,'
 || '"IsRequire"  NUMBER(1)     DEFAULT 0  ,'
 || '"IsIgnore"  NUMBER(1)     DEFAULT 0  ,'
 || '"IsObsolete"  NUMBER(1)     DEFAULT 0  ,'
 || '"IsShow"  NUMBER(1)     DEFAULT 0  ,'
 || '"IsSearch"  NUMBER(1)     DEFAULT 0  ,'
 || '"SrchMode"  INTEGER    DEFAULT 0  ,'
 || '"IsRefTab"  NUMBER(1)     DEFAULT 0  ,'
 || '"RefTab"  NVARCHAR2 (50)   DEFAULT ''''  ,'
 || '"RefField"  NVARCHAR2 (50)   DEFAULT ''''  ,'
 || '"RefFields"  NVARCHAR2 (500)   DEFAULT ''''  ,'
 || '"RefFieldDesc"  NVARCHAR2 (500)   DEFAULT ''''  ,'
 || '"RefWhere"  NVARCHAR2 (500)   DEFAULT ''''  ,'
 || '"CreateTime"  TIMESTAMP     DEFAULT SYSTIMESTAMP  ,'
 || '"CreateName"  NVARCHAR2 (50)   DEFAULT ''''  ,'
 || '"ModiTime"  TIMESTAMP     DEFAULT SYSTIMESTAMP  ,'
 || '"ModiName"  NVARCHAR2 (50)   DEFAULT ''''  '

   || ') ';
   EXECUTE IMMEDIATE 'ALTER TABLE [$Schema$].Hi_FieldModel  ADD CONSTRAINT PK_HI_FIELDMODEL_23E0AD76BE7D48B0818F1F7A117B23F2 PRIMARY KEY ("DbName","TabName" ,"FieldName" )';
   
   END IF;
   
   select count(*) into V_NUMBER from ALL_tables where table_name ='Hi_Domain' and owner='[$Schema$]';
   IF V_NUMBER = 0 THEN
    EXECUTE IMMEDIATE  'CREATE TABLE [$Schema$].Hi_Domain('
        || '"Domain"  NVARCHAR2 (10) NOT NULL    ,'
 || '"DomainDesc"  NVARCHAR2 (100)   DEFAULT ''''  ,'
 || '"CreateTime"  TIMESTAMP     DEFAULT SYSTIMESTAMP  ,'
 || '"CreateName"  NVARCHAR2 (50)   DEFAULT ''''  ,'
 || '"ModiTime"  TIMESTAMP     DEFAULT SYSTIMESTAMP  ,'
 || '"ModiName"  NVARCHAR2 (50)   DEFAULT ''''  '

   || ') ';
   
    EXECUTE IMMEDIATE 'ALTER TABLE [$Schema$].Hi_Domain  ADD CONSTRAINT PK_HI_DOMAIN_76FAEC023C73474FA477113E355B6EE1 PRIMARY KEY ("Domain" )';
   END IF;
   
   
   select count(*) into V_NUMBER from all_tables where table_name ='Hi_DataElement' and owner='[$Schema$]';
   IF V_NUMBER = 0 THEN
    EXECUTE IMMEDIATE  'CREATE TABLE [$Schema$].Hi_DataElement('
        || '"Domain"  NVARCHAR2 (10) NOT NULL    ,'
 || '"ElementValue"  NVARCHAR2 (50) NOT NULL    ,'
 || '"ElementDesc"  NVARCHAR2 (100)   DEFAULT ''''  ,'
 || '"SortNum"  INTEGER    DEFAULT 0  ,'
 || '"CreateTime"  TIMESTAMP     DEFAULT SYSTIMESTAMP  ,'
 || '"CreateName"  NVARCHAR2 (50)   DEFAULT ''''  ,'
 || '"ModiTime"  TIMESTAMP     DEFAULT SYSTIMESTAMP  ,'
 || '"ModiName"  NVARCHAR2 (50)   DEFAULT ''''  '

   || ') ';
   
    EXECUTE IMMEDIATE 'ALTER TABLE [$Schema$].Hi_DataElement  ADD CONSTRAINT PK_HI_DATAELEMENT_780B5F08535948E984E98ACA993FB667 PRIMARY KEY ("Domain" ,"ElementValue" )';
   END IF;
   
   
   
   
    EXECUTE IMMEDIATE 'DELETE FROM [$Schema$].Hi_TabModel WHERE TabName=''Hi_TabModel''';
   EXECUTE IMMEDIATE 'DELETE FROM [$Schema$].Hi_FieldModel WHERE TabName=''Hi_TabModel''';
   EXECUTE IMMEDIATE 'DELETE FROM [$Schema$].Hi_TabModel WHERE TabName=''Hi_FieldModel''';
   EXECUTE IMMEDIATE 'DELETE FROM [$Schema$].Hi_FieldModel WHERE TabName=''Hi_FieldModel''';
   EXECUTE IMMEDIATE 'DELETE FROM [$Schema$].Hi_TabModel WHERE TabName=''Hi_Domain''';
   EXECUTE IMMEDIATE 'DELETE FROM [$Schema$].Hi_FieldModel WHERE TabName=''Hi_Domain''';
   EXECUTE IMMEDIATE 'DELETE FROM [$Schema$].Hi_TabModel WHERE TabName=''Hi_DataElement''';
   EXECUTE IMMEDIATE 'DELETE FROM [$Schema$].Hi_FieldModel WHERE TabName=''Hi_DataElement''';
   
   
   
   
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_TabModel ("TabName","TabReName","TabDescript","TabStoreType","TabType","TabCacheType","TabStatus","IsSys","IsEdit","IsLog","LogTable","LogExprireDay") VALUES(''Hi_TabModel'',''Hi_TabModel'','''',''0'',''0'',''0'',''0'',0,1,0,'''',''0'')';


EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_TabModel'',''TabName'',''表名'',0,1,1,''11'',''1'','''',''10'','''',''50'',''0'','''','''',1,0,1,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_TabModel'',''TabReName'',''表的别名'',0,0,0,''11'',''5'','''',''10'','''',''50'',''0'','''','''',0,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_TabModel'',''TabDescript'',''表描述'',0,0,0,''11'',''5'','''',''10'','''',''100'',''0'','''','''',0,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_TabModel'',''TabStoreType'',''表存储方式'',0,0,0,''21'',''10'','''',''10'','''',''0'',''0'','''','''',0,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_TabModel'',''TabType'',''表类型'',0,0,0,''21'',''15'','''',''10'','''',''0'',''0'','''','''',0,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_TabModel'',''TabCacheType'',''表的缓存类型'',0,0,0,''21'',''20'','''',''10'','''',''0'',''0'','''','''',0,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_TabModel'',''TabStatus'',''表状态'',0,0,0,''21'',''25'','''',''10'','''',''0'',''0'','''','''',0,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_TabModel'',''IsSys'',''是否系统内置表'',0,0,0,''31'',''30'','''',''10'','''',''0'',''0'','''','''',1,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_TabModel'',''IsEdit'',''是否可编辑'',0,0,0,''31'',''35'','''',''10'','''',''0'',''0'','''','''',1,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_TabModel'',''IsLog'',''是否开启表日志'',0,0,0,''31'',''40'','''',''10'','''',''0'',''0'','''','''',1,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_TabModel'',''LogTable'',''日志表名'',0,0,0,''11'',''45'','''',''10'','''',''50'',''0'','''','''',1,0,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_TabModel'',''LogExprireDay'',''日志保留天数'',0,0,0,''21'',''50'','''',''10'','''',''0'',''0'','''','''',1,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_TabModel'',''CreateTime'',''创建时间'',0,0,0,''41'',''990'','''',''20'','''',''0'',''0'','''','''',0,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_TabModel'',''CreateName'',''创建人'',0,0,0,''11'',''991'','''',''10'','''',''50'',''0'','''','''',0,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_TabModel'',''ModiTime'',''修改时间'',0,0,0,''41'',''995'','''',''20'','''',''0'',''0'','''','''',0,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_TabModel'',''ModiName'',''修改人'',0,0,0,''11'',''998'','''',''10'','''',''50'',''0'','''','''',0,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
   
   
   
   EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_TabModel.TabName  IS ''表名''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_TabModel.TabReName  IS ''表的别名''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_TabModel.TabDescript  IS ''表描述''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_TabModel.TabStoreType  IS ''表存储方式''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_TabModel.TabType  IS ''表类型''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_TabModel.TabCacheType  IS ''表的缓存类型''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_TabModel.TabStatus  IS ''表状态''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_TabModel.IsSys  IS ''是否系统内置表''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_TabModel.IsEdit  IS ''是否可编辑''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_TabModel.IsLog  IS ''是否开启表日志''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_TabModel.LogTable  IS ''日志表名''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_TabModel.LogExprireDay  IS ''日志保留天数''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_TabModel.CreateTime  IS ''创建时间''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_TabModel.CreateName  IS ''创建人''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_TabModel.ModiTime  IS ''修改时间''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_TabModel.ModiName  IS ''修改人''';



EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_TabModel ("TabName","TabReName","TabDescript","TabStoreType","TabType","TabCacheType","TabStatus","IsSys","IsEdit","IsLog","LogTable","LogExprireDay") VALUES(''Hi_FieldModel'',''Hi_FieldModel'','''',''0'',''0'',''0'',''0'',0,1,0,'''',''0'')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_FieldModel'',''DbName'',''数据库名'',0,1,1,''11'',''4'','''',''10'','''',''50'',''0'','''','''',1,0,1,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_FieldModel'',''TabName'',''表名'',0,1,1,''11'',''5'','''',''10'','''',''50'',''0'','''','''',1,0,1,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_FieldModel'',''FieldName'',''字段名'',0,1,1,''11'',''10'','''',''10'','''',''50'',''0'','''','''',1,0,1,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_FieldModel'',''FieldDesc'',''字段名描述'',0,0,0,''11'',''15'','''',''10'','''',''100'',''0'','''','''',1,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_FieldModel'',''IsIdentity'',''是否自增ID'',0,0,0,''31'',''20'','''',''10'','''',''0'',''0'','''','''',1,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_FieldModel'',''IsPrimary'',''是否主键'',0,0,0,''31'',''25'','''',''10'','''',''0'',''0'','''','''',1,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_FieldModel'',''IsBllKey'',''是否是业务KEY'',0,0,0,''31'',''30'','''',''10'','''',''0'',''0'','''','''',1,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_FieldModel'',''FieldType'',''字段类型'',0,0,0,''21'',''35'','''',''10'','''',''0'',''0'','''','''',1,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_FieldModel'',''SortNum'',''字段排序号'',0,0,0,''21'',''40'','''',''10'','''',''0'',''0'','''','''',1,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_FieldModel'',''Regex'',''正则校验表达式'',0,0,0,''11'',''45'','''',''10'','''',''200'',''0'','''','''',1,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_FieldModel'',''DBDefault'',''默认值类型'',0,0,0,''21'',''50'','''',''10'','''',''0'',''0'','''','''',1,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_FieldModel'',''DefaultValue'',''默认值'',0,0,0,''11'',''55'','''',''10'','''',''50'',''0'','''','''',1,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_FieldModel'',''FieldLen'',''字段长度'',0,0,0,''21'',''60'','''',''10'','''',''0'',''0'','''','''',1,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_FieldModel'',''FieldDec'',''小数点位数'',0,0,0,''21'',''65'','''',''10'','''',''0'',''0'','''','''',1,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_FieldModel'',''SNO'',''编号名称'',0,0,0,''13'',''70'','''',''10'','''',''10'',''0'','''','''',1,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_FieldModel'',''SNO_NUM'',''子编号'',0,0,0,''13'',''75'','''',''10'','''',''3'',''0'','''','''',1,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_FieldModel'',''IsSys'',''是否系统字段'',0,0,0,''31'',''80'','''',''10'','''',''0'',''0'','''','''',1,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_FieldModel'',''IsNull'',''是否允许NULL'',0,0,0,''31'',''85'','''',''10'','''',''0'',''0'','''','''',1,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_FieldModel'',''IsRequire'',''是否必填'',0,0,0,''31'',''90'','''',''10'','''',''0'',''0'','''','''',1,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_FieldModel'',''IsIgnore'',''是否忽略'',0,0,0,''31'',''95'','''',''10'','''',''0'',''0'','''','''',1,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_FieldModel'',''IsObsolete'',''是否作废'',0,0,0,''31'',''100'','''',''10'','''',''0'',''0'','''','''',1,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_FieldModel'',''IsShow'',''是否显示'',0,0,0,''31'',''105'','''',''10'','''',''0'',''0'','''','''',1,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_FieldModel'',''IsSearch'',''是否允许搜索'',0,0,0,''31'',''110'','''',''10'','''',''0'',''0'','''','''',1,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_FieldModel'',''SrchMode'',''搜索模式'',0,0,0,''21'',''115'','''',''10'','''',''0'',''0'','''','''',1,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_FieldModel'',''IsRefTab'',''是否引用表'',0,0,0,''31'',''120'','''',''10'','''',''0'',''0'','''','''',1,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_FieldModel'',''RefTab'',''引用表名'',0,0,0,''11'',''125'','''',''10'','''',''50'',''0'','''','''',1,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_FieldModel'',''RefField'',''引用的字段'',0,0,0,''11'',''130'','''',''10'','''',''50'',''0'','''','''',1,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_FieldModel'',''RefFields'',''引用字段清单'',0,0,0,''11'',''135'','''',''10'','''',''500'',''0'','''','''',1,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_FieldModel'',''RefFieldDesc'',''引用字段清单描述'',0,0,0,''11'',''140'','''',''10'','''',''500'',''0'','''','''',1,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_FieldModel'',''RefWhere'',''引用条件'',0,0,0,''11'',''145'','''',''10'','''',''500'',''0'','''','''',1,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_FieldModel'',''CreateTime'',''创建时间'',0,0,0,''41'',''990'','''',''20'','''',''0'',''0'','''','''',0,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_FieldModel'',''CreateName'',''创建人'',0,0,0,''11'',''991'','''',''10'','''',''50'',''0'','''','''',0,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_FieldModel'',''ModiTime'',''修改时间'',0,0,0,''41'',''995'','''',''20'','''',''0'',''0'','''','''',0,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_FieldModel'',''ModiName'',''修改人'',0,0,0,''11'',''998'','''',''10'','''',''50'',''0'','''','''',0,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';


EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_FieldModel.DbName  IS ''数据库名''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_FieldModel.TabName  IS ''表名''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_FieldModel.FieldName  IS ''字段名''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_FieldModel.FieldDesc  IS ''字段名描述''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_FieldModel.IsIdentity  IS ''是否自增ID''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_FieldModel.IsPrimary  IS ''是否主键''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_FieldModel.IsBllKey  IS ''是否是业务KEY''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_FieldModel.FieldType  IS ''字段类型''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_FieldModel.SortNum  IS ''字段排序号''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_FieldModel.Regex  IS ''正则校验表达式''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_FieldModel.DBDefault  IS ''默认值类型''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_FieldModel.DefaultValue  IS ''默认值''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_FieldModel.FieldLen  IS ''字段长度''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_FieldModel.FieldDec  IS ''小数点位数''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_FieldModel.SNO  IS ''编号名称''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_FieldModel.SNO_NUM  IS ''子编号''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_FieldModel.IsSys  IS ''是否系统字段''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_FieldModel.IsNull  IS ''是否允许NULL''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_FieldModel.IsRequire  IS ''是否必填''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_FieldModel.IsIgnore  IS ''是否忽略''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_FieldModel.IsObsolete  IS ''是否作废''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_FieldModel.IsShow  IS ''是否显示''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_FieldModel.IsSearch  IS ''是否允许搜索''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_FieldModel.SrchMode  IS ''搜索模式''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_FieldModel.IsRefTab  IS ''是否引用表''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_FieldModel.RefTab  IS ''引用表名''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_FieldModel.RefField  IS ''引用的字段''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_FieldModel.RefFields  IS ''引用字段清单''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_FieldModel.RefFieldDesc  IS ''引用字段清单描述''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_FieldModel.RefWhere  IS ''引用条件''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_FieldModel.CreateTime  IS ''创建时间''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_FieldModel.CreateName  IS ''创建人''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_FieldModel.ModiTime  IS ''修改时间''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_FieldModel.ModiName  IS ''修改人''';


EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_TabModel ("TabName","TabReName","TabDescript","TabStoreType","TabType","TabCacheType","TabStatus","IsSys","IsEdit","IsLog","LogTable","LogExprireDay") VALUES(''Hi_Domain'',''Hi_Domain'',''数据域主表'',''0'',''0'',''0'',''0'',0,1,0,'''',''0'')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_Domain'',''Domain'',''数据域名'',0,1,1,''11'',''5'','''',''10'','''',''10'',''0'','''','''',1,0,1,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_Domain'',''DomainDesc'',''数据域描述'',0,0,0,''11'',''10'','''',''10'','''',''100'',''0'','''','''',1,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_Domain'',''CreateTime'',''创建时间'',0,0,0,''41'',''990'','''',''20'','''',''0'',''0'','''','''',0,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_Domain'',''CreateName'',''创建人'',0,0,0,''11'',''991'','''',''10'','''',''50'',''0'','''','''',0,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_Domain'',''ModiTime'',''修改时间'',0,0,0,''41'',''995'','''',''20'','''',''0'',''0'','''','''',0,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_Domain'',''ModiName'',''修改人'',0,0,0,''11'',''998'','''',''10'','''',''50'',''0'','''','''',0,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';

EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_Domain."Domain"  IS ''数据域名''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_Domain.DomainDesc  IS ''数据域描述''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_Domain.CreateTime  IS ''创建时间''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_Domain.CreateName  IS ''创建人''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_Domain.ModiTime  IS ''修改时间''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_Domain.ModiName  IS ''修改人''';


EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_TabModel ("TabName","TabReName","TabDescript","TabStoreType","TabType","TabCacheType","TabStatus","IsSys","IsEdit","IsLog","LogTable","LogExprireDay") VALUES(''Hi_DataElement'',''Hi_DataElement'',''数据域明细表'',''0'',''0'',''0'',''0'',0,1,0,'''',''0'')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_DataElement'',''Domain'',''数据域名'',0,1,1,''11'',''5'','''',''10'','''',''10'',''0'','''','''',1,0,1,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_DataElement'',''ElementValue'',''数据域值'',0,1,1,''11'',''10'','''',''10'','''',''50'',''0'','''','''',1,0,1,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_DataElement'',''ElementDesc'',''数据域值描述'',0,0,0,''11'',''15'','''',''10'','''',''100'',''0'','''','''',1,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_DataElement'',''SortNum'',''数据域排序号'',0,0,0,''21'',''20'','''',''10'','''',''0'',''0'','''','''',1,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_DataElement'',''CreateTime'',''创建时间'',0,0,0,''41'',''990'','''',''20'','''',''0'',''0'','''','''',0,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_DataElement'',''CreateName'',''创建人'',0,0,0,''11'',''991'','''',''10'','''',''50'',''0'','''','''',0,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_DataElement'',''ModiTime'',''修改时间'',0,0,0,''41'',''995'','''',''20'','''',''0'',''0'','''','''',0,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';
EXECUTE IMMEDIATE 'INSERT INTO [$Schema$].Hi_FieldModel ("DbName","TabName","FieldName","FieldDesc","IsIdentity","IsPrimary","IsBllKey","FieldType","SortNum","Regex","DBDefault","DefaultValue","FieldLen","FieldDec","SNO","SNO_NUM","IsSys","IsNull","IsRequire","IsIgnore","IsObsolete","IsShow","IsSearch","SrchMode","IsRefTab","RefTab","RefField","RefFields","RefFieldDesc","RefWhere")VALUES('''',''Hi_DataElement'',''ModiName'',''修改人'',0,0,0,''11'',''998'','''',''10'','''',''50'',''0'','''','''',0,1,0,0,0,1,1,''10'',0,'''','''','''','''','''')';


EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_DataElement."Domain"  IS ''数据域名''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_DataElement.ElementValue  IS ''数据域值''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_DataElement.ElementDesc  IS ''数据域值描述''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_DataElement.SortNum  IS ''数据域排序号''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_DataElement.CreateTime  IS ''创建时间''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_DataElement.CreateName  IS ''创建人''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_DataElement.ModiTime  IS ''修改时间''';
EXECUTE IMMEDIATE 'COMMENT ON COLUMN [$Schema$].Hi_DataElement.ModiName  IS ''修改人''';


 EXECUTE IMMEDIATE 'UPDATE  [$Schema$].Hi_TabModel SET TabStatus =1,IsSys =1 WHERE TabReName IN (''Hi_TabModel'',''Hi_FieldModel'',''Hi_Domain'',''Hi_DataElement'')';
END;




