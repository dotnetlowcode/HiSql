using HiSql.SqlServerUnitTest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Reflection;
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

        public class H_tst10
        { 
            public int SID { get; set; }
            public string uname { get; set; }
            public string gname { get; set; }

            public DateTime birth { get; set; }

            public int sage { get; set; }
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
            //Query_Demo8(sqlClient);
            //Query_Demo9(sqlClient);
            //Query_Demo10(sqlClient);
            //Query_Demo11(sqlClient);
            //Query_Demo12(sqlClient);
            // Query_Demo13(sqlClient);
            // Query_Demo14(sqlClient);
            //Query_Demo15(sqlClient);
            //Query_Demo16(sqlClient);
            //Query_Demo17(sqlClient);
            //Query_Demo18(sqlClient);
            //Query_Demo19(sqlClient);
            // Query_Demo20(sqlClient);
            //Query_Demo21();
           //Query_DemoEmit(sqlClient);

            Query_Null(sqlClient);
            //Query_MyFlowDto(sqlClient);
            var s = Console.ReadLine();
        }

        static void Query_MyFlowDto(HiSqlClient sqlClient)
        {
            #region  ======================测试 普通反射 和 emit转换结果是否一致====================
            {
                DataTable dt3 = sqlClient.Query("Wf_Instance").Field("*").ToTable();
                Console.WriteLine($"======================测试 普通反射 和 emit转换结果是否一致====================");


                var modelListDataConverter = DataConverter.ToList<MyFlowDto>(dt3, sqlClient.Context.CurrentConnectionConfig.DbType);
                var modelListDataConvert = DataConvert.ToEntityList<MyFlowDto>(dt3);

                Console.WriteLine($"测试 DataTable 转 List<T>  一致性：  {JsonConverter.ToJson(modelListDataConverter).Equals(JsonConverter.ToJson(modelListDataConvert))}");

                List<MyFlowDto> _FieldModelsA = new List<MyFlowDto>();
                List<MyFlowDto> _FieldModelsB = new List<MyFlowDto>();

                using (IDataReader dr = sqlClient.Context.DBO.GetDataReader("select * from Wf_Instance", null))
                {
                    _FieldModelsB = DataConverter.ToList<MyFlowDto>(dr, sqlClient.Context.CurrentConnectionConfig.DbType).ToList();
                }
                using (IDataReader dr = sqlClient.Context.DBO.GetDataReader("select * from Wf_Instance", null))
                {
                    _FieldModelsA = DataConvert.ToList<MyFlowDto>(dr, sqlClient.Context.CurrentConnectionConfig.DbType);
                }
                Console.WriteLine($"测试 IDataReader 转 List<T>  一致性：  {JsonConverter.ToJson(_FieldModelsA).Equals(JsonConverter.ToJson(_FieldModelsB))}");

            }

            #endregion
            //转到动态类
            List<MyFlowDto> lstdyn = sqlClient.HiSql($@"select WFNum,  FlowName, WFTitle, WFState, CreateClient, CreateSystem ,
        CreateUserID  from Wf_Instance where CreateUserID = 'U000101420' order by  CreateTime DESC").ToList<MyFlowDto>();
            

        }

        static void Query_Null(HiSqlClient sqlClient)
        {

            //转到动态类
            List<TDynamic> lstdyn= sqlClient.HiSql("select * from H_tst10").ToDynamic();
            int sid = 3;
            if (lstdyn.Count > 0)
            {
                foreach (TDynamic dyn in lstdyn)
                {

                    Dictionary<string, object> dic = (Dictionary<string, object>)dyn;


                    if (dic.Count > 0)
                    {
                        dic["SID"] = sid;

                        sqlClient.Insert("H_tst10", dic).ExecCommand();

                        sid++;
                    }
                }

                sqlClient.Delete("H_tst10",new List<dynamic>(){ new { SID = 3 }, new { SID = 4 } }).ExecCommand();
            }
            sid = 3;
            List<ExpandoObject> lstexp = sqlClient.HiSql("select * from H_tst10").ToEObject();
            if (lstdyn.Count > 0)
            {
                foreach (ExpandoObject dyn in lstexp)
                {

                    TDynamic dynamic=new TDynamic(dyn);

                    Dictionary<string, object> dic = (Dictionary<string, object>)dynamic;
                    if (dic.Count > 0)
                    {
                        dic["SID"] = sid;

                        sqlClient.Insert("H_tst10", dic).ExecCommand();

                        sid++;
                    }
                }
                sqlClient.Delete("H_tst10", new List<dynamic>() { new { SID = 3 }, new { SID = 4 } }).ExecCommand();
            }


            string jsonstr= sqlClient.HiSql("select * from H_tst10").ToJson();

            if (jsonstr.Length > 0)
            { 
                
            }

            DataTable DT= sqlClient.HiSql("select * from H_tst10").ToTable();



            List<H_tst10> lsttst = sqlClient.HiSql("select * from H_tst10").ToList<H_tst10>();
            sid = 3;

            H_tst10 tst1 = new H_tst10 { SID=3,uname="tansar" };
            List<H_tst10> lsttst2 = new List<H_tst10>();
            lsttst2.Add(tst1);
            H_tst10 tst2 = new H_tst10 { SID = 4, uname = "tansar" ,birth=DateTime.Now};
            lsttst2.Add(tst2);

            sqlClient.Insert("H_tst10", lsttst2).ExecCommand();


            sqlClient.Delete("H_tst10", new List<dynamic>() { new { SID = 3 }, new { SID = 4 } }).ExecCommand();



            sqlClient.Insert("H_tst10", new List<Dictionary<string, object>> { 
                new Dictionary<string, object>
                {
                    { "SID",3},
                    { "uname","tansar"}
                },
                new Dictionary<string, object>
                {
                    { "SID",4},
                    { "uname","tansar"},
                    { "birth",DateTime.Now}
                }
            }).ExecCommand();

            sqlClient.Delete("H_tst10", new List<dynamic>() { new { SID = 3 }, new { SID = 4 } }).ExecCommand();

            sqlClient.Modi("H_tst10", new List<Dictionary<string, object>> {
                new Dictionary<string, object>
                {
                    { "SID",3},
                    { "uname","tansar"}
                },
                new Dictionary<string, object>
                {
                    { "SID",4},
                    { "uname","tansar"},
                    { "birth",DateTime.Now}
                }
            }).ExecCommand();


            sqlClient.Delete("H_tst10", new List<dynamic>() { new { SID = 3 }, new { SID = 4 } }).ExecCommand();

        }


        static void Query_DemoEmit(HiSqlClient sqlClient)
        {
            HiParameter Parm = new HiParameter("@TabName", "Hi_TabModel");

            //DataTable dt= sqlClient.Context.DBO.GetDataTable("select * from dbo.Hi_FieldModel where TabName in (@TabName)", new HiParameter("@TabName",new List<string> { "Hi_TabModel' or 1=1", "Hi_FieldModel" }));
            //DataTable dt2 = sqlClient.Context.DBO.GetDataTable("select * from dbo.Hi_FieldModel where TabName = @TabName and FieldName=@TabName and FieldType=@FieldType", new HiParameter("@TabName", "Hi_TabModel"), new HiParameter("@FieldType", 11));

            DataTable dt3 = sqlClient.Query("Hi_FieldModel").Field("*").ToTable();
            var modelList = DataConverter.ToList<Hi_FieldModel>(dt3, sqlClient.Context.CurrentConnectionConfig.DbType);



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
                return;

            }
            #endregion

            #region 测试  LIST TO TABLE
            {
                for (int i = 0; i < 5; i++)
                {
                    modelList.AddRange(modelList);
                }
                Stopwatch stopwatch = Stopwatch.StartNew();
                int cnt = 10;
                for (int i = 0; i < cnt; i++)
                {
                    DataConverter.ListToDataTable<Hi_FieldModel>(modelList, sqlClient.Context.CurrentConnectionConfig.DbType);
                }
                Console.WriteLine($"测试 ToDataTable emit {cnt}次 耗时{stopwatch.ElapsedMilliseconds}");
                
                return;
            }

            #endregion

            

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
           //return;

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
                        _FieldModelsB = DataConverter.ToList<Hi_FieldModel>(dr, sqlClient.Context.CurrentConnectionConfig.DbType).ToList(); 
                    }

                }
                Console.WriteLine($"测试 IDataReader 转 List<T> emit {cnt}次 耗时{stopwatch.ElapsedMilliseconds} {JsonConverter.ToJson(_FieldModelsA).Equals(JsonConverter.ToJson(_FieldModelsB))}");
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


        static void Query_Demo20(HiSqlClient sqlClient)
        {
            //Filter filters = new Filter();
            //filters.Add("menu.status", OperType.EQ, 0);
            //filters.Add("role.status", OperType.EQ, 0);
            //filters.Add("userRole.user_id", OperType.EQ, 122);
            ////filters.AddIf(11 > 12, "userRole.user_id", OperType.EQ, 122);

            //var sql=sqlClient.HiSql(@"select menu.*,userRole.user_id from sys_menu as  menu 
            //        join sys_role_menu as roleMenu on menu.menuId = roleMenu.menu_id 
            //        join sys_user_role as userRole on userRole.Role_id = roleMenu.Role_id
            //        join sys_role as role on role.RoleId = userRole.role_id
            //     order by menu.parentId, menu.orderNum
            //    ").Where(filters).ToSql();


            var sql = sqlClient.Query("sys_menu", "a").Field("a.*").Join("sys_role_menu", JoinType.Left).As("b").On(new Filter { { "a.menuId", OperType.EQ, "`b.menu_id`" } })
                .Sort("a.menuId").ToSql();
            ;


        }

        static void Query_Demo19(HiSqlClient sqlClient)
        {
            //string sql = sqlClient.Query("Hi_FieldModel").As("A").Field("A.FieldType")
            //    .Join("Hi_TabModel").As("B").On(new HiSql.JoinOn() { { "A.TabName", "B.TabName" } })
            //    .Where("A.TabName='GD_UniqueCodeInfo'").Group(new GroupBy { { "A.FieldType" } })
            //    .Sort("A.FieldType asc","A.TabName asc")
            //    .Take(2).Skip(2)
            //    .ToSql();


            //var sql = sqlClient.HiSql("select a.tabname from hi_fieldmodel as a inner join Hi_TabModel as  b on a.tabname =b.tabname inner join Hi_TabModel as c on a.tabname=c.tabname where a.tabname='h_test'  and a.FieldType in (11,41,21)  ").ToSql();

            var sql = sqlClient.HiSql("select a.tabname from hi_fieldmodel as a inner join Hi_TabModel as  b on a.tabname =b.tabname inner join Hi_TabModel as c on a.tabname=c.tabname where a.tabname='h_test'  and a.FieldType in (11,41,21)  order by a.FieldType ").Take(2).Skip(2).ToSql();


            string sql2 = sqlClient.HiSql("select A.FieldType from Hi_FieldModel as a inner join Hi_TabModel as b on a.tabname=b.tabname where A.TabName='GD_UniqueCodeInfo' group by a.fieldtype order by a.fieldtype  asc ").Take(2).Skip(2)
                .ToSql();

            //string sql3 = sqlClient.HiSql("select  a.UniqueCode,a.BarCode,a.CategoryId from GD_UniqueCodeInfo as a").ToSql();

        }

        static void Query_Demo18(HiSqlClient sqlClient)
        {
            string sql = sqlClient.HiSql("select FieldName, count(FieldName) as NAME_count,max(FieldType) as FieldType_max from Hi_FieldModel  group by FieldName").ToSql();

            string sql_having = sqlClient.HiSql("select FieldName, count(FieldName) as NAME_count,max(FieldType) as FieldType_max from Hi_FieldModel  group by FieldName having count(FieldName) > 1").ToSql();
            List<HiColumn> lst = sqlClient.HiSql("select FieldName, count(FieldName) as NAME_count,max(FieldType) as FieldType_max from Hi_FieldModel  group by FieldName").ToColumns();

        }

        static void Query_Demo17(HiSqlClient sqlClient)
        {
            string sql1 = sqlClient.HiSql("select * from hi_tabmodel where tabname=@tabname ", new { TabName = "H_test", FieldName = "DID" }).ToSql();
            string sql2 = sqlClient.HiSql("select * from hi_tabmodel where tabname=@tabname or TabType in( @TabType)", new { TabName = "H_test", TabType = new List<int> { 1, 2, 3, 4 } }).ToSql();

            string sql3 = sqlClient.HiSql("select * from hi_tabmodel where tabname=@tabname ", new Dictionary<string, object> { { "TabName", "H_test" } }).ToSql();
            string sql4 = sqlClient.HiSql("select * from hi_tabmodel where tabname=[$tabname$] ", new Dictionary<string, object> { { "[$tabname$]", "H_test" } }).ToSql();

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

            string sql = sqlClient.HiSql(@"select * from Hi_FieldModel as a 
Where a.TabName=`a.TabName` And
a.fieldName='11'
Order By a.fieldNamE
").ToSql();

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
            //    .Group(new GroupBy { { "A.FieldNamE" } }).ToSql();

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
                new Dictionary<string, object> { { "[$name$]", "Hi_FieldModel ' or (1=1)" }, { "[$IsRequire$]", 1 } }
                ).ToSql();


            //var _sql1 = sqlClient.HiSql("").ToSql();



            var _sql1 = sqlClient.HiSql("select   TabName  from Hi_FieldModel where  FieldType in( [$list$]) order by TabName ",
                new Dictionary<string, object> { { "[$list$]", new List<int> { 1, 2, 3, 4 } } }).ToSql();

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

            var _sql2 = sqlClient.HiSql("select * from Hi_FieldModel where FieldType between  10 and 50").ToSql();

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

            string sql = sqlClient.HiSql($"select FieldName,count(*) as scount  from Hi_FieldModel group by fieldName,  Having count(*) > 0   order by fieldname")
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

            string _json = sqlClient.Query("H_Test2").Field("*").ToJson();

            List<TDynamic> lstd = sqlClient.Query("H_Test2").Field("*").ToDynamic();

        }


        static void Query_Demo4(HiSqlClient sqlClient)
        {


            List<int> numlst = new List<int>() { 1, 4, 5, 10, 20 };
            List<Rang> doulst = new List<Rang>();
            double _curr = 0;
            var scount = numlst.Sum();
            for (int i = 0; i < numlst.Count; i++)
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
            //string _sql=sqlClient.Query("Hi_TabModel").Field("TabName as tabname").
            //    Case("TabStatus")
            //        .When("TabStatus>=1").Then("'启用'")
            //        .When("0").Then("'未激活'")
            //        .Else("'未启用'")
            //    .EndAs("Tabs", typeof(string))
            //    .Field("IsSys")
            //    .ToSql()
            //    ;

            string _sql = sqlClient.Query("Hi_TabModel").As("a").Field("a.TabName as tabname").
                Case("tabStatus")
                    .When("a.Tabstatus>=1").Then("'启用'")
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
            HiParameter Parm = new HiParameter("@TabName", "Hi_TabModel");

            //DataTable dt= sqlClient.Context.DBO.GetDataTable("select * from dbo.Hi_FieldModel where TabName in (@TabName)", new HiParameter("@TabName",new List<string> { "Hi_TabModel' or 1=1", "Hi_FieldModel" }));
            DataTable dt2 = sqlClient.Context.DBO.GetDataTable("select * from dbo.Hi_FieldModel where TabName = @TabName and FieldName=@TabName and FieldType=@FieldType", new HiParameter("@TabName", "Hi_TabModel"), new HiParameter("@FieldType", 11));


            DataTable dt3 = sqlClient.Query("Hi_TabModel").Field("*").ToTable();
        }

        public static bool CreateBuilderOfMoveCrossWithIL(object P_0, object P_1)
        {
            //Error decoding local variables: Signature type sequence must have at least one element.
            ((Hi_FieldModelTest)P_0).FieldName = ((Hi_FieldModelTest)P_1).FieldName;
            ((Hi_FieldModelTest)P_0).DataType = ((Hi_FieldModelTest)P_1).DataType;
            CreateBuilderOfMoveCrossWithILHi_FieldModel((object)((Hi_FieldModelTest)P_0).PropertyInfo, (object)((Hi_FieldModelTest)P_1).PropertyInfo);
            ((Hi_FieldModelTest)P_0).FieldName2 = ((Hi_FieldModelTest)P_1).FieldName2;
            ((Hi_FieldModelTest)P_0).FieldName3 = ((Hi_FieldModelTest)P_1).FieldName3;
            return true;
        }
        public static bool CreateBuilderOfMoveCrossWithILHi_FieldModel(object P_0, object P_1)
        {
            ((Hi_FieldModel)P_0).DbName = ((Hi_FieldModel)P_1).DbName;
            ((Hi_FieldModel)P_0).TabName = ((Hi_FieldModel)P_1).TabName;
            ((Hi_FieldModel)P_0).FieldName = ((Hi_FieldModel)P_1).FieldName;
            ((Hi_FieldModel)P_0).FieldDesc = ((Hi_FieldModel)P_1).FieldDesc;
            ((Hi_FieldModel)P_0).IsIdentity = ((Hi_FieldModel)P_1).IsIdentity;
            ((Hi_FieldModel)P_0).IsPrimary = ((Hi_FieldModel)P_1).IsPrimary;
            ((Hi_FieldModel)P_0).IsBllKey = ((Hi_FieldModel)P_1).IsBllKey;
            ((Hi_FieldModel)P_0).FieldType = ((Hi_FieldModel)P_1).FieldType;
            ((Hi_FieldModel)P_0).SortNum = ((Hi_FieldModel)P_1).SortNum;
            ((Hi_FieldModel)P_0).Regex = ((Hi_FieldModel)P_1).Regex;
            ((Hi_FieldModel)P_0).DBDefault = ((Hi_FieldModel)P_1).DBDefault;
            ((Hi_FieldModel)P_0).DefaultValue = ((Hi_FieldModel)P_1).DefaultValue;
            ((Hi_FieldModel)P_0).FieldLen = ((Hi_FieldModel)P_1).FieldLen;
            ((Hi_FieldModel)P_0).FieldDec = ((Hi_FieldModel)P_1).FieldDec;
            ((Hi_FieldModel)P_0).SNO = ((Hi_FieldModel)P_1).SNO;
            ((Hi_FieldModel)P_0).SNO_NUM = ((Hi_FieldModel)P_1).SNO_NUM;
            ((Hi_FieldModel)P_0).IsSys = ((Hi_FieldModel)P_1).IsSys;
            ((Hi_FieldModel)P_0).IsNull = ((Hi_FieldModel)P_1).IsNull;
            ((Hi_FieldModel)P_0).IsRequire = ((Hi_FieldModel)P_1).IsRequire;
            ((Hi_FieldModel)P_0).IsIgnore = ((Hi_FieldModel)P_1).IsIgnore;
            ((Hi_FieldModel)P_0).IsObsolete = ((Hi_FieldModel)P_1).IsObsolete;
            ((Hi_FieldModel)P_0).IsShow = ((Hi_FieldModel)P_1).IsShow;
            ((Hi_FieldModel)P_0).IsSearch = ((Hi_FieldModel)P_1).IsSearch;
            ((Hi_FieldModel)P_0).SrchMode = ((Hi_FieldModel)P_1).SrchMode;
            ((Hi_FieldModel)P_0).IsRefTab = ((Hi_FieldModel)P_1).IsRefTab;
            ((Hi_FieldModel)P_0).RefTab = ((Hi_FieldModel)P_1).RefTab;
            ((Hi_FieldModel)P_0).RefField = ((Hi_FieldModel)P_1).RefField;
            ((Hi_FieldModel)P_0).RefFields = ((Hi_FieldModel)P_1).RefFields;
            ((Hi_FieldModel)P_0).RefFieldDesc = ((Hi_FieldModel)P_1).RefFieldDesc;
            ((Hi_FieldModel)P_0).RefWhere = ((Hi_FieldModel)P_1).RefWhere;
            ((StandField)P_0).CreateTime = ((StandField)P_1).CreateTime;
            ((StandField)P_0).CreateName = ((StandField)P_1).CreateName;
            ((StandField)P_0).ModiTime = ((StandField)P_1).ModiTime;
            ((StandField)P_0).ModiName = ((StandField)P_1).ModiName;
            return true;
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