﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql.MySqlUnitTest
{
    class Demo_Query
    {
        class H_TEST
        {
            public int DID
            {
                get; set;
            }
            public string UNAME
            {
                get; set;
            }
            public string UNAME2
            {
                get; set;
            }
            public bool ISSYS { get; set; }
        }
        public static void Init(HiSqlClient sqlClient)
        {
            //Query_Demo(sqlClient);
            //Query_Demo2(sqlClient);
            //Query_Demo3(sqlClient);
            //Query_Demo4(sqlClient);
            //Query_Case(sqlClient);
            //Query_Demo6(sqlClient);
            //Query_Demo7(sqlClient);
            //Query_Demo8(sqlClient);
            //Query_Demo9(sqlClient);
            //Query_Demo10(sqlClient);
            //Query_Demo11(sqlClient);
            //Query_Demo12(sqlClient);
            //Query_Demo13(sqlClient);
            //Query_Demo15(sqlClient);
            //Query_Demo16(sqlClient);
            //Query_Demo18(sqlClient);
            Query_Demo19(sqlClient);
        }

        static void Query_Demo19(HiSqlClient sqlClient)
        {
            //string sql = sqlClient.Query("Hi_FieldModel").As("A").Field("A.FieldType")
            //    .Join("Hi_TabModel").As("B").On(new HiSql.JoinOn() { { "A.TabName", "B.TabName" } })
            //    .Where("A.TabName='GD_UniqueCodeInfo'").Group(new GroupBy { { "A.FieldType" } })
            //    .Sort("A.FieldType asc", "A.TabName asc")
            //    .Take(2).Skip(2)
            //    .ToSql();


            var sql = sqlClient.HiSql("select a.tabname from hi_fieldmodel as a inner join Hi_TabModel as  b on a.tabname =b.tabname inner join Hi_TabModel as c on a.tabname=c.tabname where a.tabname='h_test'  and a.FieldType in (11,41,21)  order by a.FieldType ").Take(2).Skip(2).ToSql();
            //var sql = sqlClient.HiSql("select a.tabname from hi_fieldmodel as a inner join Hi_TabModel as  b on a.tabname =b.tabname inner join Hi_TabModel as c on a.tabname=c.tabname where a.tabname='h_test'  and a.FieldType in (11,41,21)  ").ToSql();

            Stopwatch sw = new Stopwatch();
            sw.Start();

            string sql2 = sqlClient.HiSql("select A.FieldType from Hi_FieldModel as a inner join Hi_TabModel as b on a.tabname=b.tabname where A.TabName='GD_UniqueCodeInfo' group by a.fieldtype order by a.fieldtype  asc ").Take(2).Skip(2)
                .ToSql();

            sw.Stop();
            Console.WriteLine($"语句编译 耗时{sw.Elapsed}");

            //string sql3 = sqlClient.HiSql("select  a.UniqueCode,a.BarCode,a.CategoryId from GD_UniqueCodeInfo as a").ToSql();

        }
        static void Query_Demo18(HiSqlClient sqlClient)
        {
            string sql = sqlClient.HiSql("select FieldName, count(FieldName) as NAME_count,max(FieldType) as FieldType_max from Hi_FieldModel  group by FieldName").ToSql();

            string sql_having = sqlClient.HiSql("select FieldName, count(FieldName) as NAME_count,max(FieldType) as FieldType_max from Hi_FieldModel  group by FieldName having count(FieldName) > 1").ToSql();


            int _total = 0;
            var data = sqlClient.HiSql("select FieldName, count(FieldName) as NAME_count,max(FieldType) as FieldType_max from Hi_FieldModel  group by FieldName having count(FieldName) > 1").Take(2).Skip(2).ToTable(ref _total);
            List<HiColumn> lst = sqlClient.HiSql("select FieldName, count(FieldName) as NAME_count,max(FieldType) as FieldType_max from Hi_FieldModel  group by FieldName").ToColumns();

        }
        static void Query_Demo16(HiSqlClient sqlClient)
        {
            //以下将会报错 字符串的不允许表达式条件 
            //string sql = sqlClient.Query("Hi_FieldModel", "A").Field("*")
            //    .Where(new Filter {
            //        {"A.TabName", OperType.EQ, "`A.FieldName`+1"}
            //                     })
            //    .Group(new GroupBy { { "A.FieldName" } }).ToSql();


            //string sql = sqlClient.Query("Hi_FieldModel", "A").Field("*")
            //    .Where(new Filter {
            //        {"A.FieldType", OperType.EQ, "abc"}
            //        //{"A.FieldName", OperType.EQ, "CreateName"},
            //                     })
            //    .Group(new GroupBy { { "A.FieldName" } }).ToSql();

            //string sql = sqlClient.Query("Hi_FieldModel", "A").Field("*")
            //    .Where(new Filter {
            //        {"A.TabName", OperType.EQ, "`A.FieldName`"}
            //                     })
            //    .Group(new GroupBy { { "A.FieldName" } }).ToSql();

            //string sql = sqlClient.Query("Hi_FieldModel", "A").Field("*")
            //    .Where("A.TabName=`A.TabName`+1")
            //    .Group(new GroupBy { { "A.FieldName" } }).ToSql();

            string sql = sqlClient.HiSql("select * from hi_FieldModel as a where a.TabName=`a.tabName` and a.FieldName='11'").ToSql();

        }

        static void Query_Demo15(HiSqlClient sqlClient)
        {
            //var sql = sqlClient.HiSql("select a.TabName, a.FieldName from Hi_FieldModel as a left join Hi_TabModel as b on a.TabName=b.TabName and a.TabName in ('H_Test') where a.TabName=b.TabName and a.FieldType>3 ").ToSql();

            //var sql = sqlClient.HiSql("select a.TabName, a.FieldName from Hi_FieldModel as a left join Hi_TabModel as b on a.TabName=b.TabName and a.TabName in ('H_Test') where a.TabName=b.TabName and a.FieldType>3 ").ToSql();

            //var sql=sqlClient.HiSql("select a.tabname from hi_fieldmodel as a inner join Hi_TabModel as  b on a.tabname =b.tabname inner join Hi_TabModel as c on a.tabname=c.tabname where a.tabname='h_test'  and a.FieldType in (11,41,21)  ").ToSql();

            //string jsondata = sqlClient.Query("Hi_FieldModel", "A").Field("A.FieldName as Fname")
            //    .Join("Hi_TabModel").As("B").On(new Filter { { "A.TabName", OperType.EQ, "Hi_FieldModel" } })
            //    .Where("a.tabname = 'Hi_FieldModel' and ((a.FieldType = 11)) and a.tabname in ('h_test','hi_fieldmodel')  and a.tabname in (select a.tabname from hi_fieldmodel as a inner join Hi_TabModel as  b on a.tabname =b.tabname " +
            //    " inner join Hi_TabModel as c on a.tabname=c.tabname where a.tabname='h_test' ) and a.FieldType in (11,41,21)  ")
            //    .Group(new GroupBy { { "A.FieldName" } }).ToSql();


            var cols2 = sqlClient.Query("Hi_FieldModel", "A").Field("A.FieldName as Fname")
                .Join("Hi_TabModel").As("B").On(new Filter { { "A.TabName", OperType.EQ, "Hi_FieldModel" } })
                .Where("a.tabname = 'Hi_FieldModel' and ((a.FieldType = 11)) and a.tabname in ('h_test','hi_fieldmodel')  and a.tabname in (select a.tabname from hi_fieldmodel as a inner join Hi_TabModel as  b on a.tabname =b.tabname " +
                " inner join Hi_TabModel as c on a.tabname=c.tabname where a.tabname='h_test' ) and a.FieldType in (11,41,21)  ")
                .Group(new GroupBy { { "A.FieldName" } }).ToColumns();


            var sql = sqlClient.HiSql("select max(FieldType) as fieldtype from Hi_FieldModel").ToJson();
            var cols = sqlClient.HiSql("select max(FieldType) as fieldtype from Hi_FieldModel").ToColumns();

        }
        /// <summary>
        /// distinct 
        /// </summary>
        /// <param name="sqlClient"></param>
        static void Query_Demo13(HiSqlClient sqlClient)
        {
            var _sql = sqlClient.HiSql("select distinct * from Hi_FieldModel where TabName=[$name$] and IsRequire=[$IsRequire$]",
                new Dictionary<string, object> { { "[$name$]", "Hi_FieldModel ' or (1=1)" }, { "[$IsRequire$]", 1 } }
                ).ToSql();


            var _sql2 = sqlClient.HiSql("select distinct TabName  from Hi_FieldModel where TabName='Hi_FieldModel' order by TabName ").Take(10).Skip(2).ToSql();


        }

        /// <summary>
        /// 防注入参数
        /// </summary>
        /// <param name="sqlClient"></param>
        static void Query_Demo12(HiSqlClient sqlClient)
        {
            var _sql = sqlClient.HiSql("select * from Hi_FieldModel where TabName=[$name$] and IsRequire=[$IsRequire$]",
                new Dictionary<string, object> { { "[$name$]", "Hi_FieldModel ' or (1=1)" }, { "[$IsRequire$]", 1 } }
                ).ToSql();


            var _sql2 = sqlClient.HiSql("select * from Hi_FieldModel where TabName='``Hi_FieldModel' ").ToSql();


            if (!string.IsNullOrEmpty(_sql))
            {

            }

        }
        static void Query_Demo11(HiSqlClient sqlClient)
        {
            var _sql = sqlClient.HiSql("select * from Hi_FieldModel where TabName in (select TabName from Hi_TabModel where TabName='h_test' group by tabname ) order by fieldname").ToSql();
            var _sql1 = sqlClient.HiSql("select * from Hi_FieldModel where TabName in (select TabName from Hi_TabModel where TabName='h_test' group by tabname )  ").ToSql();

            if (string.IsNullOrEmpty(_sql1))
            {

            }

        }
        static void Query_Demo10(HiSqlClient sqlClient)
        {

            var data= sqlClient.HiSql("select * from Hi_FieldModel where FieldType between  10 and 50").ToTable();

            var _sql = sqlClient.HiSql("select * from Hi_FieldModel where TabName like 'H_D%'").ToSql();
        }
        static void Query_Demo9(HiSqlClient sqlClient)
        {
            var _sql = sqlClient.HiSql("select * from HTest01 where  CreateTime>='2022-02-17 09:27:50' and CreateTime<='2022-03-22 09:27:50'").ToSql();
            //int _effect3 = sqlClient.Modi("Hi_Domain", new List<object> { new { Domain = "10097", DomainDesc = "用户类型10097" }, new { Domain = "10098", DomainDesc = "用户类型10098" } }).ExecCommand();
            DataTable dt= sqlClient.HiSql("select * from h_test").ToTable() ;
        }
        static void Query_Demo8(HiSqlClient sqlClient)
        {
            //string sql = sqlClient.HiSql($"select * from Hi_FieldModel  where (tabname = 'h_test') and  FieldType in (11,21,31) and tabname in (select tabname from Hi_TabModel)").ToSql();

            //string sql = sqlClient.HiSql($"select fieldlen,isprimary from  Hi_FieldModel  group by fieldlen,isprimary   order by fieldlen ")
            //    .Take(3).Skip(2)
            //    .ToSql();

            //string sql = sqlClient.HiSql($"select FieldName,FieldType from Hi_FieldModel  group by FieldName,FieldType ")
            // .Take(2).Skip(2)
            // .ToSql();

            string sql = sqlClient.HiSql($"select FieldName,count(*) as scount  from Hi_FieldModel group by FieldName,  Having count(*) > 0   order by fieldname")
               .ToSql();

            int _total = 0;

            DataTable dt = sqlClient.HiSql($"select FieldName,count(*) as scount  from Hi_FieldModel group by FieldName,  Having count(*) > 0   order by fieldname")
                .Take(2).Skip(2).ToTable(ref _total);


            //string sql = sqlClient.HiSql($"select b.tabname, a.fieldname,a.IsPrimary from  Hi_FieldModel as a  inner join   Hi_TabModel as  b on a.tabname = b.tabname" +
            //    $" inner join Hi_TabModel as c on a.tabname = c.tabname ").ToSql();

            //if (sql.Trim() != "")
            //{ 

            //}

            //int total = 0;
            //var table = sqlClient.HiSql($"select fieldlen,isprimary from  Hi_FieldModel     order by fieldlen ")
            //    .Take(3).Skip(2)
            //    .ToTable(ref total);
            //if (table != null)
            //{

            //}
        }

        static void Query_Demo7(HiSqlClient sqlClient)
        {
            List<H_TEST> lst = sqlClient.Query("h_test").Field("*").ToList<H_TEST>();

        }

        /// <summary>
        /// 测试where的新语法
        /// </summary>
        /// <param name="sqlClient"></param>
        static void Query_Demo6(HiSqlClient sqlClient)
        {
            string sql = sqlClient.Query("Hi_FieldModel", "A").Field("A.FieldName as Fname")
                .Join("Hi_TabModel").As("B").On(new JoinOn { { "A.TabName", "B.TabName" } })
                .Where(new Filter {
                    { "("},
                    {"A.TabName", OperType.EQ, "Hi_FieldModel"},
                    {"A.FieldName", OperType.EQ, "CreateName"},
                    { ")"},
                    { LogiType.OR},
                    { "("},
                    {"A.FieldType",OperType.BETWEEN,new RangDefinition(){ Low=10,High=99} },
                    { ")"}
                })
                .Group(new GroupBy { { "A.FieldName" } }).ToSql();


            string jsondata = sqlClient.Query("Hi_FieldModel", "A").Field("A.FieldName as Fname")
                .Join("Hi_TabModel").As("B").On(new JoinOn { { "A.TabName", "B.TabName" } })
                .Where("a.tabname = 'Hi_FieldModel' and ((a.FieldType = 11)) ")
                .Group(new GroupBy { { "A.FieldName" } }).ToJson();

        }
        static void Query_Demo2(HiSqlClient sqlClient)
        {
            //DataTable dt3 = sqlClient.Query("Hi_TabModel").Field("*").ToTable();
            //DataTable DT_RESULT1 = sqlClient.Query("Hi_Domain").Field("Domain").Sort(new SortBy { { "createtime" } }).ToTable();
           var ar = sqlClient.Query("Hi_Domain").Field("*").Sort("CreateTime asc", "ModiTime").Skip(1).Take(1000).Insert("#Hi_Domain");

        }
        static void Query_Demo3(HiSqlClient sqlClient)
        {
            string _json2 = sqlClient.Query(
                sqlClient.Query("Hi_FieldModel").Field("*").Where(new Filter { { "TabName", OperType.IN,
                        sqlClient.Query("Hi_TabModel").Field("TabName").Where(new Filter { {"TabName",OperType.IN,new List<string> { "Hone_Test", "H_TEST" } } })
                    } }),
                sqlClient.Query("Hi_FieldModel").Field("*").Where(new Filter { { "TabName", OperType.EQ, "DataDomain" } }),
                sqlClient.Query("Hi_FieldModel").Field("*").Where(new Filter { { "TabName", OperType.EQ, "Hi_FieldModel" } })
            )
                .Field("TabName", "count(*) as CHARG_COUNT")
                .WithRank(DbRank.DENSERANK, DbFunction.NONE, "TabName", "rowidx1", SortType.ASC)
                //.WithRank(DbRank.ROWNUMBER, DbFunction.COUNT, "*", "rowidx2", SortType.ASC)
                //以下实现组合排名
                .WithRank(DbRank.ROWNUMBER,new Ranks { { DbFunction.COUNT,"*"}, { DbFunction.COUNT, "*",SortType.DESC } }, "rowidx2")
                .WithRank(DbRank.RANK, DbFunction.COUNT, "*", "rowidx3", SortType.ASC)
                .Group(new GroupBy { { "TabName" } }).ToSql();


            if (string.IsNullOrEmpty(_json2))
            {

            }
        }
        static void Query_Case(HiSqlClient sqlClient)
        {
            string _sql = sqlClient.Query("Hi_TabModel").Field("TabName as tabname").
                Case("TabStatus")
                    .When("TabStatus>1").Then("'启用'")
                    .When("0").Then("'未激活'")
                    .Else("'未启用'")
                .EndAs("Tabs", typeof(string))
                .Field("IsSys")
                .ToSql()
                ;

            Console.WriteLine(_sql);

        }
        static void Query_Demo4(HiSqlClient sqlClient)
        {
            string _sql = sqlClient.Query("Hi_FieldModel", "A").Field("A.FieldName    as    Fname", "B.*").Join("Hi_TabModel").As("B").On(new JoinOn { { "A.TabName", "B.TabName" } }) //, "B.*" 
                                                                                                                                                                                       //.Join("Hi_Domain", "C"). On("A.MATNR", "C.MATNR")
               .Where(new Filter {
                    {"A.TabName", OperType.EQ, "Hi_FieldModel"},
                    //{"A.FieldName", OperType.IN,new List<string>{ "FieldName", "FieldLen" } },
                    //{"A.IsPrimary", OperType.EQ,"1"},
                    {"A.FieldType",OperType.BETWEEN,new RangDefinition(){ Low=10,High=99} },
                   //{LogiType.OR, new Filter{
                   //     {  "UserName", OperType.EQ, "TGM"},
                   //     { "DepId",OperType.IN,new List<string>{"1001","1002" } },
                   //    }
                   //}
               })
               //.From(new QueryProvider())
               //.Group(new GroupBy { { "A.FieldName" } })
               .Sort(new SortBy { { "A.SortNum", SortType.ASC } })
               .Skip(2).Take(10).ToJson();//, { "User.Name" }

            if (string.IsNullOrEmpty(_sql))
            {

            }

            DataTable DT_RESULT1 = sqlClient.Query("Hi_Domain").Field("Domain").Sort(new SortBy { { "createtime" } }).ToTable();
        }
        static void Query_Demo(HiSqlClient sqlClient)
        {

            
            //DataTable dt = sqlClient.Context.DBO.GetDataTable("select TOP 1000 \"MATNR\",\"MTART\",\"MATKL\" FROM \"SAPHANADB\".\"MARA\"");

            MySqlConfig mySqlConfig = new MySqlConfig(true);
            string _sql_schema = mySqlConfig.Get_Table_Schema.Replace("[$TabName$]", "h_test");
            IDataReader dr = sqlClient.Context.DBO.GetDataReader("select * from hone.h_test where 1=2");
            DataTable dt_schema = dr.GetSchemaTable();
            dr.Close();

            DataTable dt_s = sqlClient.Context.DBO.GetDataTable(_sql_schema);
            //TabInfo tabInfo = HiSqlCommProvider.TabDefinitionToEntity(dt_s, mySqlConfig.DbDefMapping.);
            //HanaConnection hdbconn = new HanaConnection("DRIVER=HDBODBC;UID=SAPHANADB;PWD=Hone@crd@2019;SERVERNODE =192.168.10.243:31013;DATABASENAME =QAS");
            //hdbconn.Open();
            //HanaCommand hanaCommand = new HanaCommand("select   * from \"SAPHANADB\".\"MARA\" WHERE 0=2");
            //hanaCommand.Connection = hdbconn;

            //HanaDataReader hdr= hanaCommand.ExecuteReader();



        }
    }
}
