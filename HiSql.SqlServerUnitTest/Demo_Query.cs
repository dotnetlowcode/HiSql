using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql.UnitTest
{
    public class Demo_Query
    {

        public class H_Test2
        {
            public int Uid { get; set; }
            public string UserName { get; set; }
            public DateTime createdate { get; set; }
        }
        public class Rang
        {
            public double Low { get; set; }
            public double High { get; set; }
            public string Key { get; set; }
        }

        public static void Init(HiSqlClient sqlClient)
        {
            //Query_Demo(sqlClient);
            //QuerySlave(sqlClient);
            //Query_Demo3(sqlClient);
            //Query_Case(sqlClient);
            //Query_Demo2(sqlClient);
            //Query_Demo4(sqlClient);
            //Query_Demo5(sqlClient);
            //Query_Demo6(sqlClient);
            //Query_Demo7(sqlClient);
            Query_Demo8(sqlClient);
            //Query_Demo9(sqlClient);
            //Query_Demo10(sqlClient);
            //Query_Demo11(sqlClient);
            //Query_Demo12(sqlClient);
           // Query_Demo13(sqlClient);
            var s = Console.ReadLine();
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
            var _sql = sqlClient.HiSql("select  * from Hi_FieldModel where TabName=[$name$] and IsRequire=[$IsRequire$]",
                new Dictionary<string, object> { { "[$name$]", "Hi_FieldModel ' or (1=1)" }, { "[$IsRequire$]",1 }  }
                ).ToSql();


            //var _sql1 = sqlClient.HiSql("").ToSql();



            var _sql1 = sqlClient.HiSql("select   TabName  from Hi_FieldModel where  FieldType in( [$list$]) order by TabName ",
                new Dictionary<string, object> { { "[$list$]",new List<int> { 1,2,3,4} } }).ToSql();

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

            var _sql2=sqlClient.HiSql("select * from Hi_FieldModel where FieldType between  10 and 50").ToSql();

            var _sql = sqlClient.HiSql("select * from Hi_FieldModel where TabName like 'H_D%'").ToSql();
        }


        static void Query_Demo9(HiSqlClient sqlClient)
        {

            var _sql = sqlClient.HiSql("select * from HTest01 where  CreateTime>='2022-02-17 09:27:50' and CreateTime<='2022-03-22 09:27:50'").ToSql();

            var json = sqlClient.HiSql("select * from H_Test5 ").ToJson();
            if (!json.IsNullOrEmpty())
            {
                //var sql=sqlClient.Insert("Hi_FieldModel", lst).ToSql();
            }

            json = sqlClient.HiSql("select * from H_Test5 ").ToJson();
        }
        static void Query_Demo8(HiSqlClient sqlClient)
        {
            //string sql = sqlClient.HiSql($"select FieldName,FieldType from Hi_FieldModel  group by FieldName,FieldType ")
            //   .Having("count(FieldType) > 1 and FieldName ='CreateTime'  ")
            //    .ToSql();

            string sql = sqlClient.HiSql($"select FieldName,count(*) as scount  from Hi_FieldModel group by FieldName,  Having count(*) > 0   order by fieldname")
               .ToSql();

            int _total = 0;

            DataTable dt = sqlClient.HiSql($"select FieldName,count(*) as scount  from Hi_FieldModel group by FieldName,  Having count(*) > 0   order by fieldname")
                .Take(2).Skip(2).ToTable(ref _total);

            Console.WriteLine(_total);
        }

        static void Query_Demo7(HiSqlClient sqlClient)
        {
            string sql = sqlClient.HiSql($"select * from Hi_FieldModel  where (tabname = 'Hi_FieldModel') and  FieldType in (11,21,31) and tabname in (select tabname from Hi_TabModel) order by tabname asc")
                .Take(2).Skip(2)
                .ToSql();
            int _total = 0;
            DataTable dt = sqlClient.HiSql($"select * from Hi_FieldModel  where (tabname = 'Hi_FieldModel') and  FieldType in (11,21,31) and tabname in (select tabname from Hi_TabModel) order by tabname asc")
                .Take(2).Skip(2).ToTable(ref _total);


            //string sql = sqlClient.HiSql($"select FieldName,FieldType from Hi_FieldModel  group by FieldName,FieldType ")
            //    .Take(2).Skip(2)
            //    .ToSql();

            //string sql2=sqlClient.Query("Hi_FieldModel").Field("*").Group("TabName").Having(new Having { { "FieldName", OperType.EQ, "CreateTime" } }).Sort("TabName").ToSql() ;
            //string sql2 = sqlClient.Query("Hi_FieldModel").Field("FieldName","count(*) as scount").Group("FieldName").Having("count(*)>0 and FieldName='CreateTime'").Sort("FieldName").ToSql();


            //string sql = sqlClient.HiSql($"select fieldlen,isprimary from  Hi_FieldModel     order by fieldlen ")
            //    .Take(3).Skip(2)
            //    .ToSql();

            //string sql = sqlClient.HiSql($"select b.tabname, a.fieldname,a.IsPrimary from  Hi_FieldModel as a  inner join   Hi_TabModel as  b on a.tabname = b.tabname" +
            //    $" inner join Hi_TabModel as c on a.tabname = c.tabname ").ToSql();

            //int total = 0;
            //var table = sqlClient.HiSql($"select fieldlen,isprimary from  Hi_FieldModel     order by fieldlen ")
            //    .Take(3).Skip(2)
            //    .ToTable(ref total);

        }


        /// <summary>
        /// 测试where的新语法
        /// </summary>
        /// <param name="sqlClient"></param>
        static void Query_Demo6(HiSqlClient sqlClient)
        {
            string sql = sqlClient.Query("Hi_FieldModel", "A").Field("A.FieldName as Fname")
                .Join("Hi_TabModel").As("B").On(new JoinOn { { "A.TabName", "B.TabName" } })
                .Join("Hi_TabModel").As("c").On(new JoinOn { { "A.TabName", "C.TabName" } })
                .Where(new Filter {
                    { "("},
                    { "("},
                    {"A.TabName", OperType.EQ, "Hi_FieldModel"},
                    {"A.FieldName", OperType.EQ, "CreateName"},
                    { ")"},
                    { ")"},
                    { LogiType.OR},
                    { "("},
                    {"A.FieldType",OperType.BETWEEN,new RangDefinition(){ Low=10,High=99} },
                    { ")"}
                })
                .Group(new GroupBy { { "A.FieldName" } }).ToSql();


            string jsondata = sqlClient.Query("Hi_FieldModel", "A").Field("A.FieldName as Fname")
                .Join("Hi_TabModel").As("B").On(new JoinOn { { "A.TabName", "B.TabName" } })
                .Where("a.tabname = 'Hi_FieldModel' and ((a.FieldType = 11)) and a.tabname in ('h_test','hi_fieldmodel')  and a.tabname in (select a.tabname from hi_fieldmodel as a inner join Hi_TabModel as  b on a.tabname =b.tabname " +
                " inner join Hi_TabModel as c on a.tabname=c.tabname where a.tabname='h_test' ) and a.FieldType in (11,41,21)  ")
                .Group(new GroupBy { { "A.FieldName" } }).ToJson();
            
        }


        static void Query_Demo5(HiSqlClient sqlClient)
        {
            //测试表中的值为null是赋值实体  H_test2请自己建表测试
            List<H_Test2> lst_test = sqlClient.Query("H_Test2").Field("*").ToList<H_Test2>();

            string _json=sqlClient.Query("H_Test2").Field("*").ToJson();

            List<TDynamic> lstd = sqlClient.Query("H_Test2").Field("*").ToDynamic();

        }


        static void Query_Demo4(HiSqlClient sqlClient)
        {
            

            List<int> numlst = new List<int>() { 1, 4, 5 ,10,20};
            List<Rang> doulst = new List<Rang>();
            double _curr = 0;
            var scount = numlst.Sum();
            for(int i=0;i<numlst.Count;i++)
            {
                Rang rang = new Rang();
                if (i == 0)
                    rang.Low = (double)0;
                else
                    rang.Low = _curr;

                rang.Key = $"No-{numlst[i]}";

                _curr = (double)numlst[i] / (double)scount;

                Console.WriteLine($"{rang.Key}的命中机率:{_curr}");

                if (i != numlst.Count - 1)
                {
                    rang.High = rang.Low + _curr;
                    _curr = rang.High;
                }
                else
                    rang.High = (double)1;
                doulst.Add(rang);
            }
            if (doulst.Count > 0)
            {
                for (int j = 0; j < 100; j++)
                {
                    double d = new Random().NextDouble();
                    foreach (Rang rang in doulst)
                    {
                        if (d >= rang.Low && d < rang.High)
                        {
                            Console.WriteLine($"值{d}命中:{rang.Key}");
                        }
                    }
                }
            }






        }

        static void QuerySlave(HiSqlClient sqlClient)
        {
            //默认从库查询
            sqlClient.Query("TmallOrderSKU").Field("*").Take(100).Skip(1).ToTable();
        }


        static void Query_Demo2(HiSqlClient sqlClient)
        {
            string _sql = sqlClient.Query("Hi_TabModel").WithLock(LockMode.NOLOCK).Field("*").ToSql();
            DataTable dt3 = sqlClient.Query("Hi_TabModel").WithLock(LockMode.NOLOCK).Field("*").ToTable();
            DataTable DT_RESULT1 = sqlClient.Query("Hi_Domain").Field("Domain").Sort(new SortBy { { "CreateTime" } }).ToTable();
            sqlClient.Query("Hi_Domain").Field("*").Sort("CreateTime desc", "ModiTime").Skip(1).Take(1000).Insert("#Hi_Domain");

        }


        static void Query_Case(HiSqlClient sqlClient)
        {
            string _sql=sqlClient.Query("Hi_TabModel").Field("TabName as tabname").
                Case("TabStatus")
                    .When("TabStatus>=1").Then("'启用'")
                    .When("0").Then("'未激活'")
                    .Else("'未启用'")
                .EndAs("Tabs", typeof(string))
                .Field("IsSys")
                .ToSql()
                ;

            Console.WriteLine(_sql);

        }
        static void Query_Demo3(HiSqlClient sqlClient)
        {
            string _json2 = sqlClient.Query(
                sqlClient.Query("Hi_FieldModel").Field("*").WithLock(LockMode.ROWLOCK).Where(new Filter { { "TabName", OperType.IN,
                        sqlClient.Query("Hi_TabModel").Field("TabName").Where(new Filter { {"TabName",OperType.IN,new List<string> { "Hone_Test", "H_TEST" } } })
                    } }),
                sqlClient.Query("Hi_FieldModel").WithLock(LockMode.ROWLOCK).Field("*").Where(new Filter { { "TabName", OperType.EQ, "DataDomain" } }),
                sqlClient.Query("Hi_FieldModel").Field("*").Where(new Filter { { "TabName", OperType.EQ, "Hi_FieldModel" } })
            )
                .Field("TabName", "count(*) as CHARG_COUNT")
                .WithRank(DbRank.DENSERANK, DbFunction.NONE, "TabName", "rowidx1", SortType.ASC)
                //.WithRank(DbRank.ROWNUMBER, DbFunction.COUNT, "*", "rowidx2", SortType.ASC)
                //以下实现组合排名
                .WithRank(DbRank.ROWNUMBER, new Ranks { { DbFunction.COUNT, "*" }, { DbFunction.COUNT, "*", SortType.DESC } }, "rowidx2")
                .WithRank(DbRank.RANK, DbFunction.COUNT, "*", "rowidx3", SortType.ASC)
                
                .Group(new GroupBy { { "TabName" } }).ToSql();


            if (string.IsNullOrEmpty(_json2))
            {

            }
        }
        static void Query_Demo(HiSqlClient sqlClient)
        {
            DataTable dt= sqlClient.Context.DBO.GetDataTable("select * from dbo.Hi_FieldModel where TabName in (@TabName)", new HiParameter("@TabName",new List<string> { "Hi_TabModel' or 1=1", "Hi_FieldModel" }));
            DataTable dt2 = sqlClient.Context.DBO.GetDataTable("select * from dbo.Hi_FieldModel where TabName = @TabName", new HiParameter("@TabName", "Hi_TabModel"));


            DataTable dt3 = sqlClient.Query("Hi_TabModel").Field("*").ToTable();
        }




    }
}
