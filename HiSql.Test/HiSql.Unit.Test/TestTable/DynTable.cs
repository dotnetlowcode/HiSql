using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql.Unit.Test.TestTable
{
    public class DynTable
    {



        public static TabInfo BuildNullTest(string tabname, bool standardfeild)
        {
            TabInfo tabinfo = new TabInfo();
            tabinfo.TabModel = new HiTable { DbServer = "", DbName = "", TabName = tabname.ToSqlInject(), TabDescript = $"{tabname}自定义表", TabStoreType = TabStoreType.Column, TabStatus = TabStatus.Use };
            tabinfo.Columns.Add(new HiColumn { DbServer = tabinfo.TabModel.DbServer, DbName = tabinfo.TabModel.DbName, FieldName = "SID", FieldType = HiType.INT, IsPrimary = true, IsBllKey = true, SortNum = 1, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY });
            tabinfo.Columns.Add(new HiColumn { DbServer = tabinfo.TabModel.DbServer, DbName = tabinfo.TabModel.DbName, FieldName = "uname", FieldType = HiType.VARCHAR, FieldLen=20, SortNum = 2, IsSys = false});
            tabinfo.Columns.Add(new HiColumn { DbServer = tabinfo.TabModel.DbServer, DbName = tabinfo.TabModel.DbName, FieldName = "gname", FieldType = HiType.NVARCHAR, FieldLen = 50, SortNum = 3, IsSys = false });
            tabinfo.Columns.Add(new HiColumn { DbServer = tabinfo.TabModel.DbServer, DbName = tabinfo.TabModel.DbName, FieldName = "birth", FieldType = HiType.DATETIME,  SortNum = 4, IsSys = false });
            tabinfo.Columns.Add(new HiColumn { DbServer = tabinfo.TabModel.DbServer, DbName = tabinfo.TabModel.DbName, FieldName = "sage", FieldType = HiType.INT,  SortNum =5, IsSys = false });

            if (standardfeild)
            {
                tabinfo.Columns.Add(new HiColumn { FieldName = "CreateTime", FieldDesc = "创建时间", FieldType = HiType.DATETIME, SortNum = 990, DBDefault = HiTypeDBDefault.FUNDATE, DefaultValue = "FUNDATE" });
                tabinfo.Columns.Add(new HiColumn { FieldName = "CreateName", FieldDesc = "创建人", FieldType = HiType.NVARCHAR, FieldLen = 50, SortNum = 991, DBDefault = HiTypeDBDefault.EMPTY });
                tabinfo.Columns.Add(new HiColumn { FieldName = "ModiTime", FieldDesc = "修改时间", FieldType = HiType.DATETIME, SortNum = 995, DBDefault = HiTypeDBDefault.FUNDATE, DefaultValue = "FUNDATE" });
                tabinfo.Columns.Add(new HiColumn { FieldName = "ModiName", FieldDesc = "修改人", FieldType = HiType.NVARCHAR, FieldLen = 50, DBDefault = HiTypeDBDefault.EMPTY, SortNum = 998 });

            }
            return tabinfo;

        }

        public static TabInfo BuildTabInfo(string tabname,bool standardfeild)
        { 
            TabInfo tabinfo = new TabInfo();
            tabinfo.TabModel = new HiTable {DbServer="",DbName="",TabName=tabname.ToSqlInject(),TabDescript=$"{tabname}自定义表", TabStoreType = TabStoreType .Column, TabStatus = TabStatus .Use};


            tabinfo.Columns.Add(new HiColumn { DbServer=tabinfo.TabModel.DbServer,DbName=tabinfo.TabModel.DbName,FieldName="Uid",FieldType=HiType.BIGINT , IsPrimary = true, IsBllKey = true, SortNum = 1, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY});


            tabinfo.Columns.Add(new HiColumn { DbServer = tabinfo.TabModel.DbServer, DbName = tabinfo.TabModel.DbName, FieldName = "Uvarchar", FieldType = HiType.VARCHAR,FieldLen=50,  FieldDesc="测试varchar字段", SortNum = 2,  DBDefault = HiTypeDBDefault.EMPTY });
            tabinfo.Columns.Add(new HiColumn { DbServer = tabinfo.TabModel.DbServer, DbName = tabinfo.TabModel.DbName, FieldName = "Unvarchar", FieldType = HiType.NVARCHAR, FieldLen = 50, FieldDesc = "测试nvarchar字段", SortNum = 3, DBDefault = HiTypeDBDefault.EMPTY });
            tabinfo.Columns.Add(new HiColumn { DbServer = tabinfo.TabModel.DbServer, DbName = tabinfo.TabModel.DbName, FieldName = "Uchar", FieldType = HiType.CHAR, FieldLen = 50, FieldDesc = "测试Uchar字段", SortNum = 4, DBDefault = HiTypeDBDefault.EMPTY });
            tabinfo.Columns.Add(new HiColumn { DbServer = tabinfo.TabModel.DbServer, DbName = tabinfo.TabModel.DbName, FieldName = "Unchar", FieldType = HiType.NCHAR, FieldLen = 50, FieldDesc = "测试Unchar字段", SortNum =5, DBDefault = HiTypeDBDefault.EMPTY });
            tabinfo.Columns.Add(new HiColumn { DbServer = tabinfo.TabModel.DbServer, DbName = tabinfo.TabModel.DbName, FieldName = "Utext", FieldType = HiType.TEXT, FieldDesc = "测试Utext字段", SortNum = 6, DBDefault = HiTypeDBDefault.EMPTY });

            tabinfo.Columns.Add(new HiColumn { DbServer = tabinfo.TabModel.DbServer, DbName = tabinfo.TabModel.DbName, FieldName = "Uguid", FieldType = HiType.GUID, FieldDesc = "测试guid字段", SortNum = 7, DBDefault = HiTypeDBDefault.EMPTY });
            
            
            tabinfo.Columns.Add(new HiColumn { DbServer = tabinfo.TabModel.DbServer, DbName = tabinfo.TabModel.DbName, FieldName = "Ubigint", FieldType = HiType.BIGINT, FieldDesc = "测试bigint字段", SortNum = 8, DBDefault = HiTypeDBDefault.EMPTY });
            tabinfo.Columns.Add(new HiColumn { DbServer = tabinfo.TabModel.DbServer, DbName = tabinfo.TabModel.DbName, FieldName = "Uint", FieldType = HiType.INT, FieldDesc = "测试int字段", SortNum = 9, DBDefault = HiTypeDBDefault.EMPTY });
            tabinfo.Columns.Add(new HiColumn { DbServer = tabinfo.TabModel.DbServer, DbName = tabinfo.TabModel.DbName, FieldName = "Usmallint", FieldType = HiType.SMALLINT, FieldDesc = "测试smallint字段", SortNum = 10, DBDefault = HiTypeDBDefault.EMPTY });
            tabinfo.Columns.Add(new HiColumn { DbServer = tabinfo.TabModel.DbServer, DbName = tabinfo.TabModel.DbName, FieldName = "Udecimal", FieldType = HiType.DECIMAL,FieldLen=18,FieldDec=3, FieldDesc = "测试decimal字段", SortNum = 11, DBDefault = HiTypeDBDefault.EMPTY });

            tabinfo.Columns.Add(new HiColumn { DbServer = tabinfo.TabModel.DbServer, DbName = tabinfo.TabModel.DbName, FieldName = "Ubool", FieldType = HiType.BOOL, FieldDesc = "测试bool字段", SortNum = 12, DBDefault = HiTypeDBDefault.EMPTY });


            tabinfo.Columns.Add(new HiColumn { DbServer = tabinfo.TabModel.DbServer, DbName = tabinfo.TabModel.DbName, FieldName = "Udate", FieldType = HiType.DATE, FieldDesc = "测试date字段", SortNum = 13, DBDefault = HiTypeDBDefault.EMPTY });
            tabinfo.Columns.Add(new HiColumn { DbServer = tabinfo.TabModel.DbServer, DbName = tabinfo.TabModel.DbName, FieldName = "Udatetime", FieldType = HiType.DATETIME, FieldDesc = "测试datetime字段", SortNum = 14, DBDefault = HiTypeDBDefault.EMPTY });


            if (standardfeild)
            {
                tabinfo.Columns.Add(new HiColumn { FieldName = "CreateTime", FieldDesc = "创建时间", FieldType=HiType.DATETIME, SortNum = 990, DBDefault = HiTypeDBDefault.FUNDATE, DefaultValue = "FUNDATE" });
                tabinfo.Columns.Add(new HiColumn { FieldName = "CreateName", FieldDesc = "创建人", FieldType = HiType.NVARCHAR, FieldLen=50, SortNum = 991, DBDefault = HiTypeDBDefault.EMPTY });
                tabinfo.Columns.Add(new HiColumn { FieldName = "ModiTime", FieldDesc = "修改时间", FieldType = HiType.DATETIME, SortNum = 995, DBDefault = HiTypeDBDefault.FUNDATE, DefaultValue = "FUNDATE" });
                tabinfo.Columns.Add(new HiColumn { FieldName = "ModiName", FieldDesc = "修改人", FieldType = HiType.NVARCHAR, FieldLen = 50, DBDefault = HiTypeDBDefault.EMPTY, SortNum = 998 });
               
            }

            return tabinfo;


        }


        public static List<object> BuildTabDataList(string tabname,int count)
        {

            List<object> list = new List<object>();

            for (int i = 0; i < count; i++)
            {
                list.Add(new { Uid =i, Uvarchar =$"testlen_{i}" ,
                    Unvarchar= "新店开业通告 广东广州天河城购物广场店新店开业通告 广东广州天河城购物广场店新店开业通告 广东广州天", 
                    Uchar= "魏凤和回应佩洛台：中国军队敌人sssdfadfadsfasdfasdf", 
                    Unchar= "新店开业通告 广东广州天河城购物广场店新店开业通告 广东广州天河城购物广场店新店开业通告 广东广州a", 
                    Utext = "魏凤和说，当今世界进入新的动荡变革期，携手构建人类命运共同体才是正确抉择魏凤和说，当今世界进入新的动荡变革期，携手构建人类命运共同体才是正确抉择", Uguid =Guid.NewGuid(), Ubigint =4112, Uint =142, Udecimal =123.32, Ubool =true, Udate =DateTime.Now, Udatetime =DateTime.Now});
            }

            return list;

        }

    }
}
