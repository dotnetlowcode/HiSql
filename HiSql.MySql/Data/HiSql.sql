

CREATE TABLE  IF NOT EXISTS `[$Schema$]`.`Hi_TabModel` (
`TabName`  varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL   COMMENT '表名' , 
`TabReName`  varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL  default '' COMMENT '表的别名' , 
`TabDescript`  varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL  default '' COMMENT '表描述' , 
`TabStoreType` int  NULL default 0 COMMENT '表存储方式' ,
`TabType` int  NULL default 0 COMMENT '表类型' ,
`TabCacheType` int  NULL default 0 COMMENT '表的缓存类型' ,
`TabStatus` int  NULL default 0 COMMENT '表状态' ,
`IsSys` bit  NULL default 0 COMMENT '是否系统内置表' ,
`IsEdit` bit  NULL default 0 COMMENT '是否可编辑' ,
`IsLog` bit  NULL default 0 COMMENT '是否开启表日志' ,
`LogTable`  varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL  default '' COMMENT '日志表名' , 
`LogExprireDay` int  NULL default 0 COMMENT '日志保留天数' ,
`CreateTime` datetime  NULL default current_timestamp COMMENT '创建时间' ,
`CreateName`  varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL  default '' COMMENT '创建人' , 
`ModiTime` datetime  NULL default current_timestamp COMMENT '修改时间' ,
`ModiName`  varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL  default '' COMMENT '修改人' , 

 PRIMARY KEY(
`TabName` ASC

)USING BTREE

)ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

CREATE TABLE  IF NOT EXISTS `[$Schema$]`.`Hi_FieldModel` (
`DbName`  varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL   COMMENT '数据库名' , 
`TabName`  varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL   COMMENT '表名' , 
`FieldName`  varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL   COMMENT '字段名' , 
`FieldDesc`  varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL  default '' COMMENT '字段名描述' , 
`IsIdentity` bit  NULL default 0 COMMENT '是否自增ID' ,
`IsPrimary` bit  NULL default 0 COMMENT '是否主键' ,
`IsBllKey` bit  NULL default 0 COMMENT '是否是业务Key' ,
`FieldType` int  NULL default 0 COMMENT '字段类型' ,
`SortNum` int  NULL default 0 COMMENT '字段排序号' ,
`Regex`  varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL  default '' COMMENT '正则校验表达式' , 
`DBDefault` int  NULL default 0 COMMENT '默认值类型' ,
`DefaultValue`  varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL  default '' COMMENT '默认值' , 
`FieldLen` int  NULL default 0 COMMENT '字段长度' ,
`FieldDec` int  NULL default 0 COMMENT '小数点位数' ,
`SNO` char(10) CHARACTER SET utf8 COLLATE utf8_general_ci NULL default '' COMMENT '编号名称' , 
`SNO_NUM` char(3) CHARACTER SET utf8 COLLATE utf8_general_ci NULL default '' COMMENT '子编号' , 
`IsSys` bit  NULL default 0 COMMENT '是否系统字段' ,
`IsNull` bit  NULL default 0 COMMENT '是否允许NULL' ,
`IsRequire` bit  NULL default 0 COMMENT '是否必填' ,
`IsIgnore` bit  NULL default 0 COMMENT '是否忽略' ,
`IsObsolete` bit  NULL default 0 COMMENT '是否作废' ,
`IsShow` bit  NULL default 0 COMMENT '是否显示' ,
`IsSearch` bit  NULL default 0 COMMENT '是否允许搜索' ,
`SrchMode` int  NULL default 0 COMMENT '搜索模式' ,
`IsRefTab` bit  NULL default 0 COMMENT '是否引用表' ,
`RefTab`  varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL  default '' COMMENT '引用表名' , 
`RefField`  varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL  default '' COMMENT '引用的字段' , 
`RefFields`  varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL  default '' COMMENT '引用字段清单' , 
`RefFieldDesc`  varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL  default '' COMMENT '引用字段清单描述' , 
`RefWhere`  varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL  default '' COMMENT '引用条件' , 
`CreateTime` datetime  NULL default current_timestamp COMMENT '创建时间' ,
`CreateName`  varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL  default '' COMMENT '创建人' , 
`ModiTime` datetime  NULL default current_timestamp COMMENT '修改时间' ,
`ModiName`  varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL  default '' COMMENT '修改人' , 

 PRIMARY KEY(
`TabName` ASC,
`FieldName` ASC,
`DbName` ASC

)USING BTREE

)ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;


CREATE TABLE  IF NOT EXISTS `[$Schema$]`.`Hi_Domain` (
`Domain`  varchar(10) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL   COMMENT '数据域名' , 
`DomainDesc`  varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL  default '' COMMENT '数据域描述' , 
`CreateTime` datetime  NULL default current_timestamp COMMENT '创建时间' ,
`CreateName`  varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL  default '' COMMENT '创建人' , 
`ModiTime` datetime  NULL default current_timestamp COMMENT '修改时间' ,
`ModiName`  varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL  default '' COMMENT '修改人' , 

 PRIMARY KEY(
`Domain` ASC

)USING BTREE

)ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

CREATE TABLE  IF NOT EXISTS `[$Schema$]`.`Hi_DataElement` (
`Domain`  varchar(10) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL   COMMENT '数据域名' , 
`ElementValue`  varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL   COMMENT '数据域值' , 
`ElementDesc`  varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL  default '' COMMENT '数据域值描述' , 
`SortNum` int  NULL default 0 COMMENT '数据域排序号' ,
`CreateTime` datetime  NULL default current_timestamp COMMENT '创建时间' ,
`CreateName`  varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL  default '' COMMENT '创建人' , 
`ModiTime` datetime  NULL default current_timestamp COMMENT '修改时间' ,
`ModiName`  varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL  default '' COMMENT '修改人' , 

 PRIMARY KEY(
`Domain` ASC,
`ElementValue` ASC

)USING BTREE

)ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;


delete from  `[$Schema$]`.`Hi_TabModel` where TabName='Hi_TabModel';
delete from  `[$Schema$]`.`Hi_FieldModel` where TabName='Hi_TabModel';
delete from  `[$Schema$]`.`Hi_TabModel` where TabName='Hi_FieldModel';
delete from  `[$Schema$]`.`Hi_FieldModel` where TabName='Hi_FieldModel';
delete from  `[$Schema$]`.`Hi_TabModel` where TabName='Hi_Domain';
delete from  `[$Schema$]`.`Hi_FieldModel` where TabName='Hi_Domain';
delete from  `[$Schema$]`.`Hi_TabModel` where TabName='Hi_DataElement';
delete from  `[$Schema$]`.`Hi_FieldModel` where TabName='Hi_DataElement';


insert into `[$Schema$]`.`Hi_TabModel` (
`TabName`,`TabReName`,`TabDescript`,`TabStoreType`,`TabType`,`TabCacheType`,`TabStatus`,`IsSys`,`IsEdit`,`IsLog`,`LogTable`,`LogExprireDay`) values('Hi_TabModel','Hi_TabModel','表结构信息主表','0','0','0','0',0,1,0,'','0');

insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_TabModel','TabName','表名',0,1,1,'11','1','','10','','50','0','','',1,0,1,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_TabModel','TabReName','表的别名',0,0,0,'11','5','','10','','50','0','','',0,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_TabModel','TabDescript','表描述',0,0,0,'11','5','','10','','100','0','','',0,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_TabModel','TabStoreType','表存储方式',0,0,0,'21','10','','10','','0','0','','',0,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_TabModel','TabType','表类型',0,0,0,'21','15','','10','','0','0','','',0,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_TabModel','TabCacheType','表的缓存类型',0,0,0,'21','20','','10','','0','0','','',0,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_TabModel','TabStatus','表状态',0,0,0,'21','25','','10','','0','0','','',0,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_TabModel','IsSys','是否系统内置表',0,0,0,'31','30','','10','','0','0','','',1,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_TabModel','IsEdit','是否可编辑',0,0,0,'31','35','','10','','0','0','','',1,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_TabModel','IsLog','是否开启表日志',0,0,0,'31','40','','10','','0','0','','',1,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_TabModel','LogTable','日志表名',0,0,0,'11','45','','10','','50','0','','',1,0,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_TabModel','LogExprireDay','日志保留天数',0,0,0,'21','50','','10','','0','0','','',1,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_TabModel','CreateTime','创建时间',0,0,0,'41','990','','20','','0','0','','',0,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_TabModel','CreateName','创建人',0,0,0,'11','991','','10','','50','0','','',0,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_TabModel','ModiTime','修改时间',0,0,0,'41','995','','20','','0','0','','',0,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_TabModel','ModiName','修改人',0,0,0,'11','998','','10','','50','0','','',0,1,0,0,0,1,1,'10',0,'','','','','');


insert into `[$Schema$]`.`Hi_TabModel` (
`TabName`,`TabReName`,`TabDescript`,`TabStoreType`,`TabType`,`TabCacheType`,`TabStatus`,`IsSys`,`IsEdit`,`IsLog`,`LogTable`,`LogExprireDay`) values('Hi_FieldModel','Hi_FieldModel','表结构信息字段表','0','0','0','0',0,1,0,'','0');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_FieldModel','TabName','表名',0,1,1,'11','5','','10','','50','0','','',1,0,1,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_FieldModel','FieldName','字段名',0,1,1,'11','10','','10','','50','0','','',1,0,1,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_FieldModel','FieldDesc','字段名描述',0,0,0,'11','15','','10','','100','0','','',1,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_FieldModel','IsIdentity','是否自增ID',0,0,0,'31','20','','10','','0','0','','',1,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_FieldModel','IsPrimary','是否主键',0,0,0,'31','25','','10','','0','0','','',1,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_FieldModel','IsBllKey','是否是业务Key',0,0,0,'31','30','','10','','0','0','','',1,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_FieldModel','FieldType','字段类型',0,0,0,'21','35','','10','','0','0','','',1,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_FieldModel','SortNum','字段排序号',0,0,0,'21','40','','10','','0','0','','',1,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_FieldModel','Regex','正则校验表达式',0,0,0,'11','45','','10','','200','0','','',1,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_FieldModel','DBDefault','默认值类型',0,0,0,'21','50','','10','','0','0','','',1,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_FieldModel','DefaultValue','默认值',0,0,0,'11','55','','10','','50','0','','',1,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_FieldModel','FieldLen','字段长度',0,0,0,'21','60','','10','','0','0','','',1,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_FieldModel','FieldDec','小数点位数',0,0,0,'21','65','','10','','0','0','','',1,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_FieldModel','SNO','编号名称',0,0,0,'13','70','','10','','10','0','','',1,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_FieldModel','SNO_NUM','子编号',0,0,0,'13','75','','10','','3','0','','',1,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_FieldModel','IsSys','是否系统字段',0,0,0,'31','80','','10','','0','0','','',1,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_FieldModel','IsNull','是否允许NULL',0,0,0,'31','85','','10','','0','0','','',1,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_FieldModel','IsRequire','是否必填',0,0,0,'31','90','','10','','0','0','','',1,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_FieldModel','IsIgnore','是否忽略',0,0,0,'31','95','','10','','0','0','','',1,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_FieldModel','IsObsolete','是否作废',0,0,0,'31','100','','10','','0','0','','',1,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_FieldModel','IsShow','是否显示',0,0,0,'31','105','','10','','0','0','','',1,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_FieldModel','IsSearch','是否允许搜索',0,0,0,'31','110','','10','','0','0','','',1,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_FieldModel','SrchMode','搜索模式',0,0,0,'21','115','','10','','0','0','','',1,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_FieldModel','IsRefTab','是否引用表',0,0,0,'31','120','','10','','0','0','','',1,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_FieldModel','RefTab','引用表名',0,0,0,'11','125','','10','','50','0','','',1,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_FieldModel','RefField','引用的字段',0,0,0,'11','130','','10','','50','0','','',1,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_FieldModel','RefFields','引用字段清单',0,0,0,'11','135','','10','','500','0','','',1,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_FieldModel','RefFieldDesc','引用字段清单描述',0,0,0,'11','140','','10','','500','0','','',1,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_FieldModel','RefWhere','引用条件',0,0,0,'11','145','','10','','500','0','','',1,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_FieldModel','CreateTime','创建时间',0,0,0,'41','990','','20','','0','0','','',0,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_FieldModel','CreateName','创建人',0,0,0,'11','991','','10','','50','0','','',0,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_FieldModel','ModiTime','修改时间',0,0,0,'41','995','','20','','0','0','','',0,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_FieldModel','ModiName','修改人',0,0,0,'11','998','','10','','50','0','','',0,1,0,0,0,1,1,'10',0,'','','','','');

insert into `[$Schema$]`.`Hi_TabModel` (
`TabName`,`TabReName`,`TabDescript`,`TabStoreType`,`TabType`,`TabCacheType`,`TabStatus`,`IsSys`,`IsEdit`,`IsLog`,`LogTable`,`LogExprireDay`) values('Hi_Domain','Hi_Domain','数据域主表','0','0','0','0',0,1,0,'','0');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_Domain','Domain','数据域名',0,1,1,'11','5','','10','','10','0','','',1,0,1,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_Domain','DomainDesc','数据域描述',0,0,0,'11','10','','10','','100','0','','',1,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_Domain','CreateTime','创建时间',0,0,0,'41','990','','20','','0','0','','',0,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_Domain','CreateName','创建人',0,0,0,'11','991','','10','','50','0','','',0,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_Domain','ModiTime','修改时间',0,0,0,'41','995','','20','','0','0','','',0,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_Domain','ModiName','修改人',0,0,0,'11','998','','10','','50','0','','',0,1,0,0,0,1,1,'10',0,'','','','','');

insert into `[$Schema$]`.`Hi_TabModel` (
`TabName`,`TabReName`,`TabDescript`,`TabStoreType`,`TabType`,`TabCacheType`,`TabStatus`,`IsSys`,`IsEdit`,`IsLog`,`LogTable`,`LogExprireDay`) values('Hi_DataElement','Hi_DataElement','数据域明细表','0','0','0','0',0,1,0,'','0');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_DataElement','Domain','数据域名',0,1,1,'11','5','','10','','10','0','','',1,0,1,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_DataElement','ElementValue','数据域值',0,1,1,'11','10','','10','','50','0','','',1,0,1,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_DataElement','ElementDesc','数据域值描述',0,0,0,'11','15','','10','','100','0','','',1,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_DataElement','SortNum','数据域排序号',0,0,0,'21','20','','10','','0','0','','',1,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_DataElement','CreateTime','创建时间',0,0,0,'41','990','','20','','0','0','','',0,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_DataElement','CreateName','创建人',0,0,0,'11','991','','10','','50','0','','',0,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_DataElement','ModiTime','修改时间',0,0,0,'41','995','','20','','0','0','','',0,1,0,0,0,1,1,'10',0,'','','','','');
insert into `[$Schema$]`.`Hi_FieldModel` (
`DbName`,`TabName`,`FieldName`,`FieldDesc`,`IsIdentity`,`IsPrimary`,`IsBllKey`,`FieldType`,`SortNum`,`Regex`,`DBDefault`,`DefaultValue`,`FieldLen`,`FieldDec`,`SNO`,`SNO_NUM`,`IsSys`,`IsNull`,`IsRequire`,`IsIgnore`,`IsObsolete`,`IsShow`,`IsSearch`,`SrchMode`,`IsRefTab`,`RefTab`,`RefField`,`RefFields`,`RefFieldDesc`,`RefWhere`)values('','Hi_DataElement','ModiName','修改人',0,0,0,'11','998','','10','','50','0','','',0,1,0,0,0,1,1,'10',0,'','','','','');





update  `[$Schema$]`.`Hi_TabModel`  set `TabStatus` =1,`IsSys`=1 where `TabName` in ('Hi_TabModel','Hi_FieldModel','Hi_Domain','Hi_DataElement');
