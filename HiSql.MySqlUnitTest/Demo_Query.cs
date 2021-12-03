using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql.MySqlUnitTest
{
    class Demo_Query
    {
        public static void Init(HiSqlClient sqlClient)
        {
            //Query_Demo(sqlClient);
            //Query_Demo2(sqlClient);
            //Query_Demo3(sqlClient);
            //Query_Demo4(sqlClient);
            //Query_Case(sqlClient);
            Query_Demo6(sqlClient);
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
            sqlClient.Query("Hi_Domain").Field("*").Sort("CreateTime asc", "ModiTime").Skip(1).Take(1000).Insert("#Hi_Domain");

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
