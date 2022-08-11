using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// 指定了属性则以属性指定的为准
    /// </summary>
    [HiTable(IsEdit =false,TabName = "Hi_FieldModel", TabDescript = "表结构信息字段表",TabStatus =TabStatus.Use)]
    /// <summary>
    /// 字段信息
    /// </summary>
    public partial class Hi_FieldModel: StandField
    {

        /// <summary>
        /// 服务器名称
        /// </summary>
        [HiColumn(FieldDesc = "DB服务器", FieldLen = 50, IsPrimary = true, IsBllKey = true, IsNull = false, DBDefault = HiTypeDBDefault.EMPTY, DefaultValue = "", SortNum = 3, IsSys = true)]
        public string DbServer { get; set; }

        [HiColumn(FieldDesc = "数据库名", FieldLen = 50, IsPrimary = true, IsBllKey = true, IsNull = false, DBDefault = HiTypeDBDefault.EMPTY, DefaultValue = "" , SortNum = 4, IsSys = true)]
        public string DbName { get; set; }

        //[HiColumn(  FieldDesc = "字段编号" ,IsPrimary =true,IsIdentity =true,SortNum =1, IsSys = true)]
        //public int Fid { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        /// 
        [HiColumn( FieldDesc ="表名", FieldLen = 50, IsPrimary =true, IsBllKey =true,IsNull =false, DBDefault = HiTypeDBDefault.EMPTY, SortNum = 5, IsSys = true)]
        public string TabName { get; set; }

        /// <summary>
        /// 字段名
        /// </summary>
        /// 
        [HiColumn(  FieldDesc = "字段名",FieldLen =50, IsPrimary = true, IsNull = false, IsBllKey =true,DBDefault =HiTypeDBDefault.EMPTY, SortNum = 10, IsSys = true)]
        public string FieldName { get; set; }



        /// <summary>
        /// 字段描述
        /// </summary>
        [HiColumn(  FieldDesc = "字段名描述", FieldLen = 100, DBDefault = HiTypeDBDefault.EMPTY, SortNum = 15, IsSys = true)]
        public string FieldDesc { get; set; }


        /// <summary>
        /// 是否自增ID
        /// </summary>
        [HiColumn(FieldDesc = "是否自增ID", DBDefault = HiTypeDBDefault.EMPTY, IsSys = true, SortNum = 20, FieldType = HiType.BOOL)]
        public bool IsIdentity
        {
            get; set;
        }


        /// <summary>
        /// 是否主键
        /// </summary>
        [HiColumn(FieldDesc = "是否主键", DBDefault = HiTypeDBDefault.EMPTY, IsSys = true, SortNum = 25, FieldType = HiType.BOOL)]
        public bool IsPrimary
        {
            get; set;
        }
        /// <summary>
        /// 是否业务Key
        /// </summary>
        [HiColumn(FieldDesc ="是否是业务Key",DBDefault = HiTypeDBDefault.EMPTY, IsSys = true, SortNum = 30, FieldType = HiType.BOOL)]
        public bool IsBllKey{get; set;}



        /// <summary>
        /// 字段类型
        /// </summary>
        [HiColumn(FieldDesc = "字段类型",  DBDefault = HiTypeDBDefault.EMPTY, IsSys = true, SortNum = 35, FieldType = HiType.INT)]
        public int FieldType{ get; set; }

        [HiColumn(FieldDesc = "字段排序号",  DBDefault = HiTypeDBDefault.EMPTY, SortNum = 40, IsSys = true, FieldType = HiType.INT)]
        public int SortNum{get;set;}

        /// <summary>
        /// 正则表达式
        /// </summary>
        [HiColumn(  FieldDesc = "正则校验表达式", FieldLen = 200, DBDefault = HiTypeDBDefault.EMPTY, IsSys = true, SortNum = 45)]
        public string Regex
        {
            get;set;
        }
        [HiColumn(FieldDesc = "默认值类型",   DBDefault = HiTypeDBDefault.EMPTY, IsSys = true, SortNum = 50, FieldType = HiType.INT)]
        public int DBDefault { get; set; }

        [HiColumn(FieldDesc = "默认值", FieldLen = 50, DBDefault = HiTypeDBDefault.EMPTY, IsSys = true, SortNum = 55)]
        public string DefaultValue { get; set; }


        /// <summary>
        /// 字段长度
        /// </summary>
        [HiColumn(FieldDesc = "字段长度", DBDefault = HiTypeDBDefault.EMPTY, IsSys = true, SortNum = 60, FieldType = HiType.INT)]
        public int FieldLen
        {
            get;set;
        }
        /// <summary>
        /// 字段小数点长度
        /// </summary>
        [HiColumn(FieldDesc = "小数点位数", DBDefault = HiTypeDBDefault.EMPTY, IsSys = true, SortNum = 65, FieldType = HiType.INT)]
        public int FieldDec
        {
            get;set;
        }

        /// <summary>
        /// 编号名称
        /// </summary>
        [HiColumn(FieldDesc = "编号名称",FieldLen =10,FieldType =HiType.VARCHAR, DBDefault = HiTypeDBDefault.EMPTY, IsSys = true, SortNum = 70)]
        public string SNO
        {
            get;set;
        }
        /// <summary>
        /// 子编号
        /// </summary>
        [HiColumn(FieldDesc = "子编号", FieldLen = 3, FieldType = HiType.VARCHAR, DBDefault = HiTypeDBDefault.EMPTY, IsSys = true, SortNum = 75)]
        public string SNO_NUM
        {
            get;set;
        }
        /// <summary>
        /// 是否系统字段
        /// </summary>
        [HiColumn(FieldDesc = "是否系统字段",IsSys =true, DBDefault = HiTypeDBDefault.EMPTY, SortNum = 80, FieldType = HiType.BOOL)]
        public bool IsSys
        {
            get;set;
        }
        /// <summary>
        /// 是否允许null值
        /// </summary>
        [HiColumn(FieldDesc = "是否允许NULL", IsSys = true, DBDefault = HiTypeDBDefault.EMPTY, SortNum = 85,FieldType = HiType.BOOL)]
        public bool IsNull
        {
            get;set;
        }

        [HiColumn(FieldDesc = "是否必填", IsSys = true, DBDefault = HiTypeDBDefault.EMPTY, SortNum = 90, FieldType = HiType.BOOL)]
        public bool IsRequire
        {
            get; set;
        }

        /// <summary>
        /// 是否忽略该字段
        /// </summary>
        [HiColumn(FieldDesc = "是否忽略", IsSys = true, DBDefault = HiTypeDBDefault.EMPTY, SortNum = 95, FieldType = HiType.BOOL)]
        public bool IsIgnore
        {
            get;set;
        }

        /// <summary>
        /// 是否作废
        /// </summary>
        [HiColumn(FieldDesc = "是否作废", IsSys = true, DBDefault = HiTypeDBDefault.EMPTY, SortNum = 100, FieldType = HiType.BOOL)]
        public bool IsObsolete { get; set; }



        /// <summary>
        /// 是否显示
        /// </summary>
        [HiColumn(FieldDesc = "是否显示", IsSys = true, DBDefault = HiTypeDBDefault.EMPTY, SortNum = 105, FieldType = HiType.BOOL)]
        public bool IsShow
        {
            get; set;
        }

        /// <summary>
        /// 是否允许搜索
        /// </summary>
        [HiColumn(FieldDesc = "是否允许搜索", IsSys = true, DBDefault = HiTypeDBDefault.EMPTY, SortNum = 110, FieldType = HiType.BOOL)]
        public bool IsSearch
        {
            get; set;
        }

        /// <summary>
        /// 搜索模式
        /// </summary>
        [HiColumn(FieldDesc = "搜索模式", IsSys = true, DBDefault = HiTypeDBDefault.EMPTY, SortNum = 115, FieldType = HiType.INT)]
        public int SrchMode
        {
            get; set;
        }

        /// <summary>
        /// 是否引用表
        /// </summary>
        [HiColumn(FieldDesc = "是否引用表", IsSys = true, DBDefault = HiTypeDBDefault.EMPTY, SortNum = 120, FieldType = HiType.BOOL)]
        public bool IsRefTab
        {
            get; set;

        }

        /// <summary>
        /// 引用表名
        /// </summary>
        [HiColumn(FieldDesc = "引用表名", FieldLen=50, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY, SortNum = 125)]
        public string RefTab
        {
            get; set;
        }

        /// <summary>
        /// 引用的字段
        /// </summary>
        [HiColumn(FieldDesc = "引用的字段", FieldLen = 50, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY, SortNum = 130)]
        public string RefField
        {
            get; set;
        }

        /// <summary>
        /// 引用字段清单
        /// </summary>
        [HiColumn(FieldDesc = "引用字段清单", FieldLen = 500, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY, SortNum = 135)]
        public string RefFields
        {
            get; set;
        }
        /// <summary>
        /// 引用字段清单描述
        /// </summary>
        [HiColumn(FieldDesc = "引用字段清单描述", FieldLen = 500, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY, SortNum = 140)]
        public string RefFieldDesc
        {
            get; set;
        }
        /// <summary>
        /// 引用条件
        /// </summary>
        [HiColumn(FieldDesc = "引用条件", FieldLen = 500, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY, SortNum = 145)]
        public string RefWhere
        {
            get; set;
        }
    }
}
