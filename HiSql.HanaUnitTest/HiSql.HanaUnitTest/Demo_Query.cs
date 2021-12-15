using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql.HanaUnitTest
{
    public class Demo_Query
    {

        public static void Init(HiSqlClient sqlClient)
        {
            //Query_Demo(sqlClient);
            //Query_Demo2(sqlClient);
            //Query_Demo3(sqlClient);
            //Query_Demo4(sqlClient);
            //Query_Demo5(sqlClient);
            //Query_Case(sqlClient);
            Query_Demo8(sqlClient);

        }

        /// <summary>
        /// 测试分页返回总数
        /// </summary>
        /// <param name="sqlClient"></param>
        static void Query_Demo8(HiSqlClient sqlClient)
        {
            //string sql = sqlClient.HiSql($"select * from Hi_FieldModel  where (tabname = 'h_test') and  FieldType in (11,21,31) and tabname in (select tabname from Hi_TabModel)").ToSql();

            //string sql = sqlClient.HiSql($"select fieldlen,isprimary from  Hi_FieldModel  group by fieldlen,isprimary   order by fieldlen ")
            //    .Take(3).Skip(2)
            //    .ToSql();

            //int total = 0;
            //var table = sqlClient.HiSql($"select fieldlen,isprimary from  Hi_FieldModel     order by fieldlen ")
            //    .Take(3).Skip(2)
            //    .ToTable(ref total);
            //if (table != null)
            //{

            //}

            string sql = sqlClient.HiSql($"select FieldName,count(*) as scount  from Hi_FieldModel group by FieldName,  Having count(*) > 0   order by fieldname")
               .ToSql();

            int _total = 0;

            DataTable dt = sqlClient.HiSql($"select FieldName,count(*) as scount  from Hi_FieldModel group by FieldName,  Having count(*) > 0   order by fieldname")
                .Take(2).Skip(2).ToTable(ref _total);


            //string sql = sqlClient.Query("Hi_TabModel").Field("*").Sort(new SortBy { { "CreateTime" } }).Take(2).Skip(2).ToSql();
        }
        static void Query_Demo5(HiSqlClient sqlClient)
        {
            string _sql = sqlClient.Query("MGS_OD_ORDERINFO", "A").Field("A.ORDERID").Sort(new SortBy { { "A.ORDERID", SortType.ASC } })
               .Skip(2).Take(1000)
                .ToSql();
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
            //string _sql = sqlClient.Query("Hi_FieldModel", "A").Field("A.FieldName    as    Fname", "B.*").Join("Hi_TabModel").As("B").On(new JoinOn { { "A.TabName", "B.TabName" } }) //, "B.*" 
            //                                                                                                                                                                                    //.Join("Hi_Domain", "C"). On("A.MATNR", "C.MATNR")
            //   .Where(new Filter {
            //        {"A.TabName", OperType.EQ, "Hi_FieldModel"},
            //        //{"A.FieldName", OperType.IN,new List<string>{ "FieldName", "FieldLen" } },
            //        //{"A.IsPrimary", OperType.EQ,"1"},
            //        {"A.FieldType",OperType.BETWEEN,new RangDefinition(){ Low=10,High=99} },
            //       //{LogiType.OR, new Filter{
            //       //     {  "UserName", OperType.EQ, "TGM"},
            //       //     { "DepId",OperType.IN,new List<string>{"1001","1002" } },
            //       //    }
            //       //}
            //   })
            //   //.From(new QueryProvider())
            //   //.Group(new GroupBy { { "A.FieldName" } })
            //   .Sort(new SortBy { { "A.SortNum", SortType.ASC } })
            //   .Skip(2).Take(10).ToJson();//, { "User.Name" }

            //if (string.IsNullOrEmpty(_sql))
            //{ 

            //}

            //DataTable DT_RESULT1 = sqlClient.Query("Hi_Domain").Field("Domain").Sort(new SortBy { { "createtime" } }).ToTable();
            DataTable DT_RESULT1 = sqlClient.Query("H_TEST").Field("*").ToTable();
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
                .WithRank(DbRank.ROWNUMBER, DbFunction.COUNT, "*", "rowidx2", SortType.ASC)
                .WithRank(DbRank.RANK, DbFunction.COUNT, "*", "rowidx3", SortType.ASC)
                .Group(new GroupBy { { "TabName" } }).ToJson();


            if (string.IsNullOrEmpty(_json2))
            { 
                
            }
        }
        static void Query_Demo2(HiSqlClient sqlClient)
        {
            DataTable dt3 = sqlClient.Query("Hi_TabModel").Field("*").ToTable();
            sqlClient.Query("Hi_Domain").Field("*").Sort("createtime asc", "moditime").Skip(1).Take(1000).Insert("#Hi_Domain");

        }
        static void Query_Demo(HiSqlClient sqlClient)
        {

            StringBuilder sb_stru = new StringBuilder();
            sb_stru
                .AppendLine("SELECT b.\"OBJECT_TYPE\" AS \"TabType\", a.\"TABLE_NAME\" AS \"TabName\" ,a.\"POSITION\" as \"FieldNo\",A.\"COLUMN_NAME\" AS \"FieldName\",FALSE AS  \"IsIdentity\",FALSE AS \"IsPrimary\"")
                .AppendLine(",a.\"DATA_TYPE_NAME\" AS \"FieldType\",a.\"LENGTH\" * 2 AS \"UseBytes\",a.\"LENGTH\" AS \"Lens\",a.\"SCALE\" AS \"PointDec\",a.\"IS_NULLABLE\"  as \"IsNull\",")
                .AppendLine("a.\"DEFAULT_VALUE\" as \"DbDefault\",a.\"COMMENTS\" as \"FieldDesc\"")
                .AppendLine("FROM SYS.TABLE_COLUMNS as a")
                .AppendLine("  INNER JOIN \"SYS\".\"OBJECTS\" AS b on a.\"TABLE_NAME\" = b.\"OBJECT_NAME\" AND b.\"OBJECT_TYPE\" in ('VIEW','TABLE')")
                .AppendLine("  WHERE TABLE_NAME = 'OD_OrderInfo' ORDER BY POSITION;")
                ;

            DataTable dt= sqlClient.Context.DBO.GetDataTable("select TOP 1000 \"MATNR\",\"MTART\",\"MATKL\" FROM \"SAPHANADB\".\"MARA\"");

            IDataReader dr= sqlClient.Context.DBO.GetDataReader("select   * from \"SAPHANADB\".\"OD_OrderInfo\" WHERE 0=2");
            DataTable dt_s = sqlClient.Context.DBO.GetDataTable(sb_stru.ToString());

            //HanaConnection hdbconn = new HanaConnection("DRIVER=HDBODBC;UID=SAPHANADB;PWD=Hone@crd@2019;SERVERNODE =192.168.10.243:31013;DATABASENAME =QAS");
            //hdbconn.Open();
            //HanaCommand hanaCommand = new HanaCommand("select   * from \"SAPHANADB\".\"MARA\" WHERE 0=2");
            //hanaCommand.Connection = hdbconn;

            //HanaDataReader hdr= hanaCommand.ExecuteReader();

            DataTable dr_schema = dr.GetSchemaTable();
            if (dr_schema.Rows.Count > 0)
            {
                foreach (DataRow drow in dt_s.Rows)
                {
                    var _drow = dr_schema.Select($"ColumnName='{drow["FieldName"].ToString()}'").FirstOrDefault();
                    if (_drow != null)
                    {
                        drow["IsPrimary"] = _drow["IsKey"];
                        drow["IsIdentity"] = _drow["IsUnique"];
                    }
                }
            }

            if (dt_s.Rows.Count > 0)
            { 
                
            }

            //HanaConfig hanaConfig = new HanaConfig(true);
            //TabInfo tabInfo= HiSqlCommProvider.TabDefinitionToEntity(dt_s, hanaConfig.DbMapping);

            //if (tabInfo != null)
            //{ 
                
            //}

            //DataTable hdr_schema = hdr.GetSchemaTable();
            //if (hdr_schema.Rows.Count > 0)
            //{

            //}

        }
    }
}
