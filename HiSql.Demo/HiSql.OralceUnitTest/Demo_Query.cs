using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql.OralceUnitTest
{
    class Demo_Query
    {
        public static void Init(HiSqlClient sqlClient)
        {
            //Query_Demo(sqlClient);
            //Query_Demo1(sqlClient);
            // Query_Demo2(sqlClient);
            //Query_Demo3(sqlClient);
            //Query_Demo4(sqlClient);
            //Query_Demo8(sqlClient);

            //Query_Case(sqlClient);
            //Query_Demo9(sqlClient);
            // Query_Demo13(sqlClient);
            //Query_Demo14(sqlClient);
            // Query_Demo15(sqlClient);
            //Query_Demo16(sqlClient);
            Query_DemoEmit(sqlClient);
            var s = Console.ReadLine();
        }
        static void Query_DemoEmit(HiSqlClient sqlClient)
        {
            HiParameter Parm = new HiParameter("@TabName", "Hi_TabModel");

            //DataTable dt= sqlClient.Context.DBO.GetDataTable("select * from dbo.Hi_FieldModel where TabName in (@TabName)", new HiParameter("@TabName",new List<string> { "Hi_TabModel' or 1=1", "Hi_FieldModel" }));
            //DataTable dt2 = sqlClient.Context.DBO.GetDataTable("select * from dbo.Hi_FieldModel where TabName = @TabName and FieldName=@TabName and FieldType=@FieldType", new HiParameter("@TabName", "Hi_TabModel"), new HiParameter("@FieldType", 11));

            DataTable dt3 = sqlClient.Query("Hi_FieldModel").Field("*").ToTable();
            var modelList = DataConverter.ToList<Hi_FieldModel>(dt3, sqlClient.Context.CurrentConnectionConfig.DbType);


            #region 测试 查询性能 对比
            {
                Console.WriteLine($"====================测试 查询性能 对比==================");

                Stopwatch stopwatch = Stopwatch.StartNew();
                int cnt = 100;
                for (int i = 0; i < cnt; i++)
                {
                    DataTable dt3s = sqlClient.Query("Hi_FieldModel").Field("*").ToTable();
                    var _result = DataConvert.ToEntityList<Hi_FieldModel>(dt3s);

                }

                Console.WriteLine($"测试 DataTable 转 List<T> 反射 {cnt}次 耗时{stopwatch.ElapsedMilliseconds}");
                stopwatch = Stopwatch.StartNew();
                for (int i = 0; i < cnt; i++)
                {
                    DataTable dt3s = sqlClient.Query("Hi_FieldModel").Field("*").ToTable();
                    var _result = DataConverter.ToList<Hi_FieldModel>(dt3s, sqlClient.Context.CurrentConnectionConfig.DbType);
                }
                Console.WriteLine($"测试 DataTable 转 List<T> emit {cnt}次 耗时{stopwatch.ElapsedMilliseconds}");

                /*
                    结论： 
                 */
            }
            #endregion
            return;

            #region  ======================测试 普通反射 和 emit转换结果是否一致====================
            {
                Console.WriteLine($"======================测试 普通反射 和 emit转换结果是否一致====================");


                var modelListDataConverter = DataConverter.ToList<Hi_FieldModel>(dt3, sqlClient.Context.CurrentConnectionConfig.DbType);
                var modelListDataConvert = DataConvert.ToEntityList<Hi_FieldModel>(dt3);

                Console.WriteLine($"测试 DataTable 转 List<T>  一致性：  {JsonConverter.ToJson(modelListDataConverter).Equals(JsonConverter.ToJson(modelListDataConvert))}");

                List<Hi_FieldModel> _FieldModelsA = new List<Hi_FieldModel>();
                List<Hi_FieldModel> _FieldModelsB = new List<Hi_FieldModel>();

                using (IDataReader dr = sqlClient.Context.DBO.GetDataReader("select * from Hi_FieldModel", null))
                {
                    _FieldModelsB = DataConverter.ToList<Hi_FieldModel>(dr, sqlClient.Context.CurrentConnectionConfig.DbType).ToList();
                }
                using (IDataReader dr = sqlClient.Context.DBO.GetDataReader("select * from Hi_FieldModel", null))
                {
                    _FieldModelsA = DataConvert.ToList<Hi_FieldModel>(dr, sqlClient.Context.CurrentConnectionConfig.DbType);
                }
                Console.WriteLine($"测试 IDataReader 转 List<T>  一致性：  {JsonConverter.ToJson(_FieldModelsA).Equals(JsonConverter.ToJson(_FieldModelsB))}");

            }

            #endregion

            #region 测试 对象比对 CompareTabProperties
            {
                Console.WriteLine($"=====================测试 对象比对 CompareTabProperties==================");

                DataTable dtPros = sqlClient.Query("Hi_FieldModel").Field("*").ToTable();
                var _result = DataConverter.ToList<Hi_FieldModel>(dtPros, sqlClient.Context.CurrentConnectionConfig.DbType);

                var column = _result[0];
                var _column = _result[1];
                _column.DbName = "";
                int cnt = 100000;

                var result = false;
                var m1 = new Hi_FieldModel()
                {
                    FieldName = "Hi_FieldModel_m22"
                    ,
                    CreateName = "Hi_FieldModel_m22"
                };
                var m2 = new Hi_FieldModel()
                {
                    FieldName = "Hi_FieldModel_m22"
                    ,
                    CreateName = "Hi_FieldModel_m22"
                };

                Stopwatch stopwatch = Stopwatch.StartNew();

                for (int i = 0; i < cnt; i++)
                {
                    //比较 指定字段的
                    var aa = ClassExtensions.CompareTabProperties(column, _column, sqlClient.Context.CurrentConnectionConfig.DbType);
                    result = aa.Item1;
                }
                Console.WriteLine($" ClassExtensions.CompareTabProperties 反射 结果 {result} {cnt}次 耗时{stopwatch.ElapsedMilliseconds}");
                stopwatch = Stopwatch.StartNew();



                for (int i = 0; i < cnt; i++)
                {
                    //通用 比较所有字段
                    result = DataConverter.ObjCompareProperties(column, _column);
                }
                Console.WriteLine($" DataConverter.ObjCompareProperties  emit 结果 {result}  {cnt} 次 耗时{stopwatch.ElapsedMilliseconds}");

            }
            #endregion

            {
                //sqlClient.Update("hi_fieldmodel", new { DbName = "", TabName = "H_Test5", FieldName = "sid", SNO = "SALENO", SNO_NUM = "1" }).Only("SNO", "SNO_NUM").ExecCommand();
            }


            #region 测试 IDataReader 转 List<T>  和  DataTable 转 List<T> 对比
            {
                Console.WriteLine($"=====================测试 IDataReader 转 List<T>  和  DataTable 转 List<T> ==================");

                Stopwatch stopwatch = Stopwatch.StartNew();
                int cnt = 20;
                for (int i = 0; i < cnt; i++)
                {
                    DataTable dt32 = sqlClient.Query("Hi_FieldModel").Field("*").ToTable();
                    var _result = DataConverter.ToList<Hi_FieldModel>(dt3, sqlClient.Context.CurrentConnectionConfig.DbType);
                }
                Console.WriteLine($"测试 DataTable 转 List<T> DataConverter {cnt}次 耗时{stopwatch.ElapsedMilliseconds}");
                stopwatch = Stopwatch.StartNew();
                for (int i = 0; i < cnt; i++)
                {
                    using (IDataReader dr = sqlClient.Context.DBO.GetDataReader("select * from Hi_FieldModel", null))
                    {
                        var _result = DataConverter.ToList<Hi_FieldModel>(dr, sqlClient.Context.CurrentConnectionConfig.DbType);
                        dr.Close();
                        dr.Dispose();
                    }
                }
                Console.WriteLine($"测试 IDataReader 转 List<T> DataConverter {cnt}次 耗时{stopwatch.ElapsedMilliseconds}");
                /*
                    结论： 
                 */
            }
            #endregion

            #region 测试 DataTable 转 List<T>
            {
                Console.WriteLine($"=====================测试 DataTable 转 List<T> ==================");

                DataTable dt3s = sqlClient.Query("Hi_FieldModel").Field("*").ToTable();

                Stopwatch stopwatch = Stopwatch.StartNew();
                int cnt = 20000;
                for (int i = 0; i < cnt; i++)
                {
                    var _result = DataConvert.ToEntityList<Hi_FieldModel>(dt3s);

                }

                Console.WriteLine($"测试 DataTable 转 List<T> 反射 {cnt}次 耗时{stopwatch.ElapsedMilliseconds}");
                stopwatch = Stopwatch.StartNew();
                for (int i = 0; i < cnt; i++)
                {
                    var _result = DataConverter.ToList<Hi_FieldModel>(dt3s, sqlClient.Context.CurrentConnectionConfig.DbType);
                }
                Console.WriteLine($"测试 DataTable 转 List<T> emit {cnt}次 耗时{stopwatch.ElapsedMilliseconds}");

                /*
                    结论： 
                 */
            }
            #endregion

            #region 测试 IDataReader 转 List<T>
            {
                Console.WriteLine($"=====================测试 DataTable 转 List<T> ==================");

                DataTable dt3s = sqlClient.Query("Hi_FieldModel").Field("*").ToTable();

                Stopwatch stopwatch = Stopwatch.StartNew();

                List<Hi_FieldModel> _FieldModelsA = null;
                List<Hi_FieldModel> _FieldModelsB = null;
                int cnt = 1;
                for (int i = 0; i < cnt; i++)
                {
                    using (IDataReader dr = sqlClient.Context.DBO.GetDataReader("select * from Hi_FieldModel", null))
                    {
                        _FieldModelsA = DataConvert.ToList<Hi_FieldModel>(dr, sqlClient.Context.CurrentConnectionConfig.DbType);
                    }
                }
                Console.WriteLine($"测试 IDataReader 转 List<T> 反射 {cnt}次 耗时{stopwatch.ElapsedMilliseconds} ");
                stopwatch = Stopwatch.StartNew();
                for (int i = 0; i < cnt; i++)
                {
                    using (IDataReader dr = sqlClient.Context.DBO.GetDataReader("select * from Hi_FieldModel", null))
                    {
                        _FieldModelsB = DataConverter.ToList<Hi_FieldModel>(dr, sqlClient.Context.CurrentConnectionConfig.DbType).ToList(); ;
                    }

                }
                Console.WriteLine($"测试 IDataReader 转 List<T> emit {cnt}次 耗时{stopwatch.ElapsedMilliseconds} {JsonConverter.ToJson(_FieldModelsA).Equals(JsonConverter.ToJson(_FieldModelsB))}");
            }
            #endregion

            #region 测试 对象克隆
            {
                Console.WriteLine($"=====================测试 对象克隆==================");

                var m1 = new Hi_FieldModelTest()
                {
                    FieldName = "Hi_FieldModel_m1"
                    ,
                    DataType = DateTime.Now
                };
                var m2 = new Hi_FieldModelTest()
                {
                    FieldName = "Hi_FieldModel_m1"
                   ,
                    DataType = DateTime.Now // DateTime.Now  m1.DataType
                };


                m1.PropertyInfo = new Hi_FieldModel()
                {
                    FieldName = "Hi_FieldModel_m22"
                    ,
                    CreateName = "Hi_FieldModel_m22"
                };

                m2.PropertyInfo = new Hi_FieldModel()
                {
                    FieldName = "Hi_FieldModel_m226"
                    ,
                    CreateName = "Hi_FieldModel_m22"
                };


                Stopwatch stopwatch = Stopwatch.StartNew();
                List<Hi_FieldModelTest> listA = new List<Hi_FieldModelTest>();
                int cnt = 100000;
                for (int i = 0; i < cnt; i++)
                {
                    var a = ClassExtensions.CloneCopy(m1);
                    listA.Add(a);
                }
                Console.WriteLine($"测试 Newtonsoft.Json.JsonConvert.SerializeObject {cnt}次 耗时{stopwatch.ElapsedMilliseconds} ");  //{JsonConverter.ToJson(listA)}

                List<Hi_FieldModelTest> listB = new List<Hi_FieldModelTest>();

                stopwatch = Stopwatch.StartNew();
                for (int i = 0; i < cnt; i++)
                {
                    var a = DataConverter.CloneObjectWithIL(m1);
                    listB.Add(a);
                }


                Console.WriteLine($"测试  emit {cnt}次 耗时{stopwatch.ElapsedMilliseconds}  "); //{JsonConverter.ToJson(listB)}

                //stopwatch = Stopwatch.StartNew();
                //for (int i = 0; i < cnt; i++)
                //{
                //    Hi_FieldModelTest a2 = new Hi_FieldModelTest();
                //    FastCopy<Hi_FieldModelTest, Hi_FieldModelTest>.Copy(m2, a2);
                //}
                //Console.WriteLine($"测试 表达式树，不支持引用类型属性  FastCopy {cnt}次 耗时{stopwatch.ElapsedMilliseconds} "); //{JsonConverter.ToJson(listB)}

            }
            #endregion

            #region 测试   Collect
            {
                Console.WriteLine($"=====================测试   Collect=================");

                Stopwatch stopwatch = Stopwatch.StartNew();
                var collectModel = new Hi_TabNameCount();
                int cnt = 1000;
                for (int i = 0; i < cnt; i++)
                {
                    List<Hi_TabNameCount> lstcount = ClassExtensions.Collect(modelList.Where(t => t.FieldLen < 1000).ToList(), collectModel);
                }
                Console.WriteLine($"测试   反射 {cnt}次 耗时{stopwatch.ElapsedMilliseconds} ");

                //DataConverter.Collect(modelList.Where(t => t.FieldLen < 1000).FirstOrDefault(), collectModel);
                stopwatch = Stopwatch.StartNew();
                for (int i = 0; i < cnt; i++)
                {
                    List<Hi_TabNameCount> lstcount = DataConverter.Collect(modelList.Where(t => t.FieldLen < 1000).ToList(), collectModel);
                }
                Console.WriteLine($"测试  emit 反射 {cnt}次 耗时{stopwatch.ElapsedMilliseconds} ");

                stopwatch = Stopwatch.StartNew();

                for (int i = 0; i < cnt; i++)
                {
                    List<Hi_TabNameCount> lstcount = (from a in modelList
                                                      where a.FieldLen < 1000
                                                      group a by new { a.TabName }
                                                      into g
                                                      select new Hi_TabNameCount { TabName = g.Key.TabName, FieldLen = g.Sum(t => t.FieldLen) }).ToList();//  ClassExtensions.Collect(modelList, collectModel);
                }
                Console.WriteLine($"测试 Linq 反射 {cnt}次 耗时{stopwatch.ElapsedMilliseconds} ");  //{JsonConverter.ToJson(listA)}

            }

            #endregion

            #region 测试 MoveCross
            {
                Console.WriteLine($"=====================测试 MoveCross=================");

                var m1 = new Hi_FieldModelTest()
                {
                    FieldName = "Hi_FieldModel_m1"
                    ,
                    DataType = DateTime.Now
                };
                var m2 = new Hi_FieldModelTest()
                {
                    FieldName = "Hi_FieldModel_m1"
                   ,
                    DataType = DateTime.Now // DateTime.Now  m1.DataType
                };


                m1.PropertyInfo = new Hi_FieldModel()
                {
                    FieldName = "Hi_FieldModel_m22"
                    ,
                    CreateName = "Hi_FieldModel_m22"
                };
                m1.PropertyInfo = modelList[0];

                m2.PropertyInfo = new Hi_FieldModel()
                {
                    FieldName = "Hi_FieldModel_m226"
                    ,
                    CreateName = "Hi_FieldModel_m22"
                };
                m2.PropertyInfo = modelList[0];


                Stopwatch stopwatch = Stopwatch.StartNew();
                List<Hi_FieldModel> listA = new List<Hi_FieldModel>();
                int cnt = 200000;
                for (int i = 0; i < cnt; i++)
                {
                    Hi_FieldModel a2 = new Hi_FieldModel();
                    ClassExtensions.MoveCross<Hi_FieldModel>(m1.PropertyInfo, a2);
                    //listA.Add(a2);
                }
                Console.WriteLine($"测试 MoveCross 反射 {cnt}次 耗时{stopwatch.ElapsedMilliseconds} ");  //{JsonConverter.ToJson(listA)}

                List<Hi_FieldModel> listB = new List<Hi_FieldModel>();

                stopwatch = Stopwatch.StartNew();
                for (int i = 0; i < cnt; i++)
                {
                    Hi_FieldModel a2 = new Hi_FieldModel();
                    var a = DataConverter.MoveCrossWithIL(m1.PropertyInfo, a2);

                    listB.Add(a2);
                }
                Console.WriteLine($"测试 MoveCross  emit {cnt}次 耗时{stopwatch.ElapsedMilliseconds}  "); //{JsonConverter.ToJson(listB)}


            }
            #endregion

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

            //string sql = sqlClient.HiSql("select * from hi_FieldModel as a where a.FieldType=`a.FieldType`+`a.SortNum` and a.FieldName='11'").ToSql();
            var _sql = sqlClient.HiSql("select  TabName  from Hi_FieldModel where TabName='Hi_FieldModel' order by TabName ").Take(10).Skip(2).ToSql();
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
        static void Query_Demo14(HiSqlClient sqlClient)
        {
            var _sql = sqlClient.HiSql("select a.TabName, a.FieldName from Hi_FieldModel as a inner join Hi_TabModel as b on a.TabName=b.TabName where a.TabName=b.TabName and a.FieldType>3").ToSql();
        }
        //Query_Demo8(sqlClient);



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
        static void Query_Demo9(HiSqlClient sqlClient)
    {
        sqlClient.HiSql("select * from H_Test").ToTable();
    }
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
               .Skip(2).Take(10).ToSql();//, { "User.Name" }

            if (string.IsNullOrEmpty(_sql))
            {

            }

            DataTable DT_RESULT1 = sqlClient.Query("Hi_Domain").Field("Domain").Sort(new SortBy { { "createtime" } }).ToTable();
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
                .WithRank(DbRank.ROWNUMBER, new Ranks { { DbFunction.COUNT, "*" }, { DbFunction.COUNT, "*", SortType.DESC } }, "rowidx2")
                .WithRank(DbRank.RANK, DbFunction.COUNT, "*", "rowidx3", SortType.ASC)
                .Group(new GroupBy { { "TabName" } }).ToJson();


            if (string.IsNullOrEmpty(_json2))
            {

            }
        }

        static void Query_Demo2(HiSqlClient sqlClient)
        {
            //DataTable dt = sqlClient.Context.DBO.GetDataTable("select * from system.Hi_TabModel where TabName='Hi_TabModel'");
            //DataTable dt3 = sqlClient.Query("Hi_TabModel").Field("*").ToTable();
            //DataTable DT_RESULT1 = sqlClient.Query("Hi_Domain").Field("Domain").Sort(new SortBy { { "createtime" } }).ToTable();
           var aa=  sqlClient.Query("Hi_Domain").Field("*").Sort("CreateTime asc", "ModiTime").Skip(1).Take(1000).Insert("#Hi_Domain"); 
            var aa33 = sqlClient.Query("Hi_Domain").Field("*").Sort("CreateTime asc", "ModiTime").Skip(1).Take(1000).Insert("#Hi_Domain");

        }

        static void Query_Demo1(HiSqlClient sqlClient)
        {
            //string _sql = sqlClient.Query("H_TEST", "A").Field("A.*").ToJson();
            string _sql = sqlClient.Query("HI_FIELDMODEL", "A").Field("A.*").ToJson();
        }
        static void Query_Demo(HiSqlClient sqlClient)
        {

       
            HiParameter Parm = new HiParameter("@TabName", "Hi_TabModel");

            //DataTable dt= sqlClient.Context.DBO.GetDataTable("select * from dbo.Hi_FieldModel where TabName in (@TabName)", new HiParameter("@TabName",new List<string> { "Hi_TabModel' or 1=1", "Hi_FieldModel" }));
            DataTable dt2 = sqlClient.Context.DBO.GetDataTable("select * from Hi_FieldModel where TabName = @TabName and FieldName=@TabName and FieldType=@FieldType", new HiParameter("@TabName", "Hi_TabModel"), new HiParameter("@FieldType", 11));


            DataTable dt3 = sqlClient.Query("Hi_TabModel").Field("*").ToTable();
            

            //DataTable dt = sqlClient.Context.DBO.GetDataTable("select TOP 1000 \"MATNR\",\"MTART\",\"MATKL\" FROM \"SAPHANADB\".\"MARA\"");

            //PostGreSqlConfig mySqlConfig = new PostGreSqlConfig(true);
            //string _sql_schema = mySqlConfig.Get_Table_Schema.Replace("[$TabName$]", "h_test");
            IDataReader dr = sqlClient.Context.DBO.GetDataReader("select * from system.\"H_TEST\" where 1=2");
            DataTable dt_schema = dr.GetSchemaTable();
            dr.Close();
            DataTable dt = sqlClient.Context.DBO.GetDataTable("select * from SYSTEM.Hi_TabModel where TabName=:TabName", new HiParameter[] {
                new HiParameter(":TabName","Hi_TabModel")
            
            });

            



            //DataTable dt_s = sqlClient.Context.DBO.GetDataTable(_sql_schema);
            //TabInfo tabInfo = HiSqlCommProvider.TabDefinitionToEntity(dt_s, mySqlConfig.DbMapping);
            //HanaConnection hdbconn = new HanaConnection("DRIVER=HDBODBC;UID=SAPHANADB;PWD=Hone@crd@2019;SERVERNODE =192.168.10.243:31013;DATABASENAME =QAS");
            //hdbconn.Open();
            //HanaCommand hanaCommand = new HanaCommand("select   * from \"SAPHANADB\".\"MARA\" WHERE 0=2");
            //hanaCommand.Connection = hdbconn;

            //HanaDataReader hdr= hanaCommand.ExecuteReader();



        }
    }
    public class Hi_FieldModelTest
    {
        private const string privatevalueconst = "privatevalue";
        private static string privatevaluestatic = "privatevaluestatic";
        private string privatevalue = "privatevalue";
        public string FieldName { get; set; }
        public DateTime DataType { get; set; }
        public Hi_FieldModel PropertyInfo { get; set; }
        public string FieldName2 { get; set; }
        public string FieldName3 { get; set; }
    }
    public partial class Hi_TabNameCount
    {
        public string TabName { get; set; }

        public int FieldLen { get; set; }

    }
}
