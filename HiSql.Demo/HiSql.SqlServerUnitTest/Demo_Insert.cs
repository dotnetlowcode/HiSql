﻿using HiSql.SqlServerUnitTest;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{

    public class Demo_Insert
    {
        [System.Serializable]
        [HiTable(IsEdit = true, TabName = "HTest01")]
        public class HTest01 : StandField
        {
            [HiColumn(FieldDesc = "编号", IsPrimary = true, IsBllKey = true, FieldType = HiType.INT, SortNum = 1, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
            public int SID
            {
                get; set;
            }
            [HiColumn(FieldDesc = "姓名", FieldType = HiType.NVARCHAR, FieldLen = 50, IsNull = false, SortNum = 2, DBDefault = HiTypeDBDefault.EMPTY)]
            public string UName
            {
                get; set;
            }

            [HiColumn(FieldDesc = "年龄", FieldType = HiType.INT, IsNull = false, SortNum = 3, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY)]
            public int Age
            {
                get; set;
            }

            [HiColumn(FieldDesc = "薪水", FieldType = HiType.DECIMAL, FieldDec = 2, FieldLen = 18, IsNull = false, SortNum = 4, DBDefault = HiTypeDBDefault.EMPTY)]
            public int Salary
            {
                get; set;
            }
            [HiColumn(FieldDesc = "描述编号", FieldType = HiType.NVARCHAR, FieldLen = 100, IsNull = false, SortNum = 5, IsSys = true, DBDefault = HiTypeDBDefault.EMPTY)]
            public string Descript
            {
                get; set;
            }
            [HiColumn(IsIgnore = true)]
            public string Descript22
            {
                get; set;
            }
            [HiColumn(IsIgnore = true)]
            public string Descript33
            {
                get; set;
            }
            [HiColumn(IsIgnore = true)]
            public string Descript44
            {
                get; set;
            }
        }

        [System.Serializable]
        [HiTable(IsEdit = true, TabName = "H_Test")]
        public class H_Test : StandField
        {
            [HiColumn(FieldDesc = "编号", IsPrimary = true, IsBllKey = true, FieldType = HiType.INT, SortNum = 1, IsSys = false, DBDefault = HiTypeDBDefault.EMPTY)]
            public int Hid
            {
                get; set;
            }
            public string UserName
            {
                get; set;
            }
            public int UserAge
            {
                get; set;
            }
            public string ReName
            {
                get; set;
            }
            public DateTime CreateTime
            {
                get; set;
            }
            public string CreateName
            {
                get; set;
            }
            public DateTime ModiTime
            {
                get; set;
            }
            public string ModiName
            {
                get; set;
            }

        }
        public static void Init(HiSqlClient sqlClient)
        {
            //Demo1_Insert(sqlClient);
            //Demo1_Insert2(sqlClient);
            //Demo1_Insert3(sqlClient);
            //Demo1_Insert4(sqlClient);
            //Demo1_Insert5(sqlClient);
            //Demo1_Insert6(sqlClient);
            Demo1_Insert7(sqlClient);
            //Demo_dynamic(sqlClient);

            //Demo1_Insert8(sqlClient);
            //Demo1_Insert9(sqlClient);

           // Demo1_Insert11(sqlClient);
            //Demo1_Insert12(sqlClient);
            //Demo1_Insert13(sqlClient);
        }


        static void Demo1_Insert13(HiSqlClient sqlClient)
        {

            HiSql.Global.SnroOn = true;
            HiSql.SnroNumber.SqlClient = sqlClient;

            var json=sqlClient.HiSql("select * from H_Test4").Take(1).Skip(1).ToJson();
            var json1 = sqlClient.HiSql("select * from H_Test5").Take(1).Skip(1).ToJson();

            //sqlClient.Update("hi_fieldmodel", new { TabName= "H_Test4",FieldName= "sid", SNO = "SALENO", SNO_NUM = "1" }).Only("SNO", "SNO_NUM").ExecCommand();
            //HiSqlCommProvider.RemoveTabInfoCache("H_Test4");
            sqlClient.Update("hi_fieldmodel", new { DbName="", TabName = "H_Test5", FieldName = "sid", SNO = "SALENO", SNO_NUM = "1" }).Only("SNO", "SNO_NUM").ExecCommand();
            HiSqlCommProvider.RemoveTabInfoCache("H_Test5", sqlClient.Context.CurrentConnectionConfig);

            List<object> list = new List<object>();

            List<string> list2 = new List<string>();

            list2.Add("20220708090210095");
            list2.Add("20220708090210096");
            list2.Add("20220708090210097");
            list2.Add("20220708090210098");
            list2.Add("20220708090210099");
            list2.Add("20220708090210100");

            for (int i = 0; i < 1000000; i++)
            {
                if (list2.Count > 0)
                {
                    list.Add(new { sid= list2[0], uname = $"uname{i}", age = 20 * i, descript = $"test{i}" });
                    list2.RemoveAt(0);
                }
                else
                {
                    list.Add(new { sid="", uname = $"uname{i}", age = 20 * i, descript = $"test{i}" });
                }

            }

            sqlClient.Modi("H_Test5", list).ExecCommand();

            //sqlClient.Insert("H_Test5", list).ExecCommand();




        }




        static void Demo1_Insert12(HiSqlClient sqlClient)
        {
            ///工作单
            using (var client = sqlClient.CreateUnitOfWork())
            {
                client.Insert("H_UType", new { UTYP = "U4", UTypeName = "高级用户" }).ExecCommand();
                client.CommitTran();
            }
        }

        static void Demo1_Insert11(HiSqlClient sqlClient)
        {

            
            sqlClient.BeginTran(IsolationLevel.ReadUncommitted);
            sqlClient.BeginTran();
            sqlClient.Modi("H_UType", new List<object> {
                new { UTYP = "U1", UTypeName = "普通用户" },
                new { UTYP = "U2", UTypeName = "中级用户" },
                new { UTYP = "U3", UTypeName = "高级用户" }
            }).ExecCommand();

            sqlClient.CommitTran();

        }

        static void Demo1_Insert9(HiSqlClient sqlClient)
        {
            TabInfo tabinfo = sqlClient.Context.DMInitalize.GetTabStruct("HTest01");

            List<Dictionary<string, object>> lstdata = new List<Dictionary<string, object>>();
            int _count = 100;
            Random random = new Random();
            for (int i = 0; i < _count; i++)
            {
                lstdata.Add(new Dictionary<string, object> { { "SID", (i + 1) }, { "UName", $"tansar{i}" }, { "Age", 20 + (i % 50) }, { "Salary", 5000 + (i % 2000) + random.Next(10) }, { "descript", "hello world" } });


            }
            string _josn=DataConvert.ToCSV(lstdata, tabinfo, DBType.MySql,true, "tansar");
        }
        public static string ReadTextFile(string path)
        {
            StringBuilder sb = new StringBuilder();
            if (System.IO.File.Exists(path))
            {
                StreamReader sr = new StreamReader(path, Encoding.Default);
                string content;
                while ((content = sr.ReadLine()) != null)
                {
                    //Console.WriteLine(content.ToString());
                    sb.AppendLine(content);
                }
                sr.Close();
            }

            return sb.ToString();
        }
        static Dictionary<string, object> ToDictionary(Newtonsoft.Json.Linq.JObject jobject)
        {
            var dico = new Dictionary<string, object>();

            foreach (var o in jobject)
            {
                var oValue = o.Value;

                var jArray = o.Value as JArray;
                if (jArray != null)
                {
                    var first = jArray[0]; // use first element to guess if we are dealing with a list of dico or an array
                    var isValueArray = first is JValue;

                    if (isValueArray)
                    {
                        var array = jArray.Values().Select(x => ((JValue)x).Value).ToArray();
                        dico[o.Key] = array;
                    }
                    else
                    {
                        var list = new List<IDictionary<string, object>>();
                        foreach (var token in jArray)
                        {
                            var elt = ToDictionary((JObject)token);//((JObject)token).ToDictionary();
                            list.Add(elt);
                        }

                        dico[o.Key] = list;
                    }
                }
                else
                {
                    dico[o.Key] = ((JValue)oValue).Value;
                }
            }

            return dico;
        }
        static async void Demo1_Insert8(HiSqlClient sqlClient)
        {
            TabInfo tabinfo = sqlClient.Context.DMInitalize.GetTabStruct("GD_UniqueCodeInfo");

            //List<Dictionary<string, object>> lstdata = new List<Dictionary<string, object>> {
            //    new Dictionary<string, object> { { "SID", 123456 }, { "UName", "tansar" }, { "Age", 25 }, { "Salary", 1999.9 }, { "descript", "hello world" } },
            //    new Dictionary<string, object> { { "SID", 123457 }, { "UName", "tansar" }, { "Age", 25 }, { "Salary", 1999.9 }, { "descript", "hello world" } }
            //};



            //sqlClient.TrunCate("HTest01").ExecCommand();
            //List<Dictionary<string, object>> lstdata = new List<Dictionary<string, object>>();
            //int _count = 10000;
            //Random random = new Random();
            //for (int i = 0; i < _count; i++)
            //{
            //    lstdata.Add(new Dictionary<string, object> { { "SID", (i + 1) }, { "UName", $"tansar{i}" }, { "Age", 20 + (i % 50) }, { "Salary", 5000 + (i % 2000) + random.Next(10) }, { "descript", "hello world" } });


            //}
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            //sqlClient.Insert("HTest01", lstdata).ExecCommand();
            //sw.Stop();
            //Console.WriteLine($"分包插入{_count}条 耗时{sw.Elapsed}秒");

            //List<TDynamic> lstdyn = new List<TDynamic>();
            //TDynamic t1 = new TDynamic();
            //t1["SID"] = 123456;
            //t1["UName"] = "tansar";
            //t1["Age"] = 25;
            //t1["Salary"] = 1999.9;
            //t1["descript"] = "hello world";
            //lstdyn.Add(t1);

            //TDynamic t2 = new TDynamic();
            //t2["SID"] = 123457;
            //t2["UName"] = "tansar";
            //t2["Age"] = 25;
            //t2["Salary"] = 1999.9;
            //t2["descript"] = "hello world";
            //lstdyn.Add(t2);

            //var lstdata2 = new List<object> {
            //    new { UTYP = "U1", UTypeName = "普通用户" },
            //    new { UTYP = "U2", UTypeName = "中级用户" },
            //    new { UTYP = "U3", UTypeName = "高级用户" }
            //};

            //sqlClient.TrunCate("HTest01").ExecCommand();
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            //DataTable dt= DataConvert.ToTable(lstdata, tabinfo,sqlClient.CurrentConnectionConfig.User);
            //sw.Stop();
            //Console.WriteLine($"数据转换Table{_count}条 耗时{sw.Elapsed}秒");
            //sw.Reset();
            //sw.Start();
            //int _effect= await  sqlClient.BulkCopyExecCommandAsyc("HTest01", dt);
            //Console.WriteLine($"写入{_effect}条 耗时{sw.Elapsed}秒");
            var s = Console.ReadLine();

        }
        public static TabInfo GetTempTabInfo(TabInfo tabinfo)
        {
            var primaryJsonList = JArray.FromObject(
                tabinfo.GetColumns.Where(col => col.IsPrimary).ToList()
            );
            List<HiColumn> lstcolumn = tabinfo.GetColumns.Where(col => col.IsPrimary).ToList();
            var primaryList = new List<HiColumn>();
            var randm = new Random().Next(100000, 999999);
            //var tempTableName = "#" + tableName + "_" + randm;
            string tempName = $"#{tabinfo.TabModel.TabName}_{randm}";

            foreach (var primary in primaryJsonList)
            {
                var column = primary.ToObject<HiColumn>();
                if (column != null)
                {
                    column.TabName = tempName;
                    column.IsPrimary = false;
                    column.IsIdentity = false;
                    primaryList.Add(column);
                }
            }

            TabInfo _tabinfo = new TabInfo
            {
                TabModel = new HiTable { TabName = tempName },
                Columns = primaryList
            };
            return _tabinfo;
        }
        //测试 表校验检测
        static async Task Demo1_Insert7(HiSqlClient sqlClient)
        {
            try
            {

                TabInfo stockTabInfo = sqlClient.DbFirst.GetTabStruct("ThStock");
                TabInfo tabInfo = GetTempTabInfo(stockTabInfo);
                //
                sqlClient.DbFirst.CreateTable(tabInfo);

                var data = sqlClient.HiSql($"select * from {tabInfo.TabModel.TabName}").ToTable();
                var tmp_struct = sqlClient.DbFirst.GetTabStruct(tabInfo.TabModel.TabName);



                ///
                var ttt = sqlClient.Modi("H02_ShopInfo", new List<object> {
                    new { ShopCode="A0001", AreaCode = "AreaCode", WhseName = "WhseName" },

                    new { ShopCode="A0002", AreaCode = "AreaCode", WhseName = "WhseName" }
                }).ToSql();

                var ttt2 = sqlClient.Insert("H02_ShopInfo", new List<object> {
                    new { ShopCode="A00021", AreaCode = "AreaCode", WhseName = "WhseName" },

                    new { ShopCode="A00022", AreaCode = "AreaCode", WhseName = "WhseName" }
                }).ToSql();


                var ttt233 = sqlClient.Update("H02_ShopInfo", new List<object> {
                    new { ShopCode="A00021", AreaCode = "AreaCode", WhseName = "WhseName" },

                    new { ShopCode="A00022", AreaCode = "AreaCode", WhseName = "WhseName" }
                }).ToSql();

                sqlClient.DbFirst.CreateTable(new TabInfo() { });
            }
            catch (Exception ex)
            {

                throw;
            }
            

            //sqlClient.Update("Hi_FieldModel", new { TabName = "HTest01", FieldName = "UTYP", Regex = @"" ,IsRefTab=true,RefTab= "H_UType",RefField="UTYP", RefFields = "UTYP,UTypeName",RefFieldDesc= "类型编码,类型名称",RefWhere="UTYP<>''" }).ExecCommand();
            //sqlClient.Update("Hi_FieldModel", new { TabName = "HTest01", FieldName = "UName", Regex = @"^[\w]+[^']$" ,IsRefTab=false,RefTab= "",RefField="", RefFields = "",RefFieldDesc= "",RefWhere="" }).ExecCommand();
            //sqlClient.BeginTran(IsolationLevel.ReadUncommitted);
            // sqlClient.DbFirst.DropTable(typeof(HTest01).Name);

            bool isok = sqlClient.DbFirst.CheckTabExists(typeof(HTest01).Name);
            if (isok == false)
            {
                sqlClient.DbFirst.CreateTable(typeof(HTest01));
            }


            sqlClient.CodeFirst.Truncate("HTest01");

            int _effect1 = sqlClient.Insert("HTest01", new HTest01 { SID = (int) DateTime.Now.Ticks, UName = "tansar", Age = 25, Salary = 1999, Descript = "hello world" }).ExecCommand();


            //sqlClient.Delete("HTest01", new HTest01 { SID = 123456 }).ExecCommand();
            //await sqlClient.Insert("HTest01", new { SID = "0", UTYP = "U4", UName = "hisql", Age = 36, Salary = 11, Descript = "hisql" }).ExecCommandAsync();

        }
        static async Task Demo1_Insert6(HiSqlClient sqlClient)
        {
            var table = sqlClient.Context.DMInitalize.BuildTabStru(typeof(H_Test));
            var sql = sqlClient.Context.DMTab.BuildTabCreateSql(table.Item1, table.Item2);
            var i= await sqlClient.Context.DBO.ExecCommandAsync(sql);
            Console.WriteLine(i);
            //int v = sqlClient.Insert("H_Test", new H_Test() { Hid = 1, UserName = "tansar", UserAge = 33, ReName = "tgm", CreateTime = DateTime.Now, CreateName = "tansara", ModiTime = DateTime.Now, ModiName = "tansarb" }).ExecCommand();
        }

        static async Task Demo1_Insert5(HiSqlClient sqlClient)
        {
            var insertObj = sqlClient.Insert("H_TEST", new { descript = "hello world" });
            int v = await insertObj.ExecCommandAsync();
            if (v > 0)
            {

            }
        }
        static async Task Demo_dynamic(HiSqlClient sqlClient)
        {
            List<object> lstdyn = new List<object>();
            for (int i = 0; i < 10; i++)
            {
                TDynamic dyn1 = new TDynamic();
                dyn1["Hid"] = 150 + i;
                dyn1["UserName"] = $"tgm{i}";
                dyn1["UserAge"] = 34;
                dyn1["Birth"] = DateTime.Now;
                //lstdyn.Add(dyn1);
            }
            TDynamic dyn = new TDynamic();
            dyn["Hid"] = 123;
            dyn["UserName"] = "tgm";
            dyn["UserAge"] = 34;
            dyn["Birth"] = DateTime.Now;

            dynamic ddyn = dyn.ToDynamic();

            ddyn.Userid = "";

            var type = ddyn.GetType();
            var prop = type.GetProperties();


            string hid = ddyn.Birth.ToString("yyyy-MM-dd HH:mm:ss.fff");

            ddyn.Hid = 99;

            int scount = 1000;
            Random random = new Random();
            Dictionary<string, object> exo = (Dictionary<string, object>)dyn;
            for (int i = 0; i < scount; i++)
            {
                lstdyn.Add(new Dictionary<string, object>
                {
                    //{ "Hid",$"12{i}"},
                    //{ "UserName",$"tgm1{i}"},
                    //{ "UserAge","32"}
                    { "SID",(i + 1)},
                    { "UName",$"hisql{i}"},
                    { "Age",20 + (i % 50)},
                    { "Salary",5000 + (i % 2000) + random.Next(10)},
                    { "Descript",$"hisql"}
                });
            }



            //lstdyn.Add(dyn);

            DateTime dnow = DateTime.Now;
            Console.WriteLine(dnow.ToString());



            sqlClient.Insert("HTest01", new { SID = "0", UTYP = "U1", UName = "hisql", Age = 36, Salary = 11, Descript = "hisql" }).ExecCommand();
            //string _sql = sqlClient.Modi("H_Test", lstdyn).ToSql(); 
            //string _sql = sqlClient.Insert("H_Test", lstdyn).ToSql(); 

            Stopwatch SW = new Stopwatch();
            SW.Start();
            int v = sqlClient.Insert("HTest01", lstdyn).ExecCommand();
            SW.Stop();
            Console.WriteLine($" hisql插入{scount} 耗时{SW.Elapsed}秒");

        }

        static void Demo1_Insert4(HiSqlClient sqlClient)
        {
            ///测试 非自增长且不允许为空且没有设置默认值且没有传值 
            string _sql2 = sqlClient.Insert("H_Test", new Dictionary<string, object> { { "UserAge", 34 }, { "Hid", 5798 } }).ToSql();
        }


        static void Demo1_Insert3(HiSqlClient sqlClient)
        {
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();

            Dictionary<string, string> _dic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { "DID", "2" }, { "UserName", "QXW" }, { "UserAge", "100" }, { "ReName", "xw" } };
            list.Add(_dic);

            Dictionary<string, string> _dic1 = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { "DID", "3" }, { "UserName", "QXW1" }, { "UserAge", "101" }, { "ReName", "xw1" } };
            list.Add(_dic1);


            var SQL= sqlClient.Update<Dictionary<string, string>>("H_Test", list).ToSql();
            //sqlClient.Modi<Dictionary<string, string>>("H_Test", list).ExecCommand();
            //sqlClient.Modi<Dictionary<string, string>>("H_Test", list).ExecCommand();
        }
        static async void Demo1_Insert(HiSqlClient sqlClient)
        {
            //Dictionary<string, string> _dic_data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { {"uname","tansar"},{ "Uname2","用户123"} };
            string sql4 = sqlClient.Modi("H_Test", new { Hid = 1, UserName = "tansar", UserAge = 100, ReName = "Tom" }).ToSql();

            //string _sql=sqlClient
            //    .Insert("H_TEST", new { UNAME = "UTYP10", UNAME2 = "用户类10" } )
            //    .Insert("Hone_Test",new { Username ="TOM5", Scount =100})
            //    .ToSql();

            //int _effect=sqlClient
            //    .Insert("H_TEST", new { UNAME = "UTYPE9", UNAME2 = "用户类型9" })
            //    .Insert("Hone_Test", new { Username = "TOM4", Scount = 99 }).ExecCommand();

            //string _sql2 = sqlClient.Insert<H_Test>(new H_Test { DID = 123, UNAME="UTYPEHA" ,UNAME2="TEST'" }).ToSql();

            string _sql3 = sqlClient.Modi("Hi_Domain", new List<object> { new { Domain = "10097", DomainDesc = "用户类型10097" }, new { Domain = "10098", DomainDesc = "用户类型10098" } }).ToSql();
            int _effect3 = sqlClient.Modi("Hi_Domain", new List<object> { new { Domain = "10097", DomainDesc = "用户类型10097" }, new { Domain = "10098", DomainDesc = "用户类型10098" } }).ExecCommand();



        }

        static async Task Demo1_Insert2(HiSqlClient sqlClient)
        {

            int _times = 100000;
            Console.WriteLine($"[{_times}]条数据 HANA插入测试");
            List<object> lstobj = new List<object>();
            for (int i = 0; i < _times; i++)
            {
                lstobj.Add(new { Domain = $"U{i.ToString()}", DomainDesc = $"用户{i.ToString()}" });
            }

            string _sql = sqlClient
            .Insert("Hi_Domain", new { Domain = "UTYPE", DomainDesc = "用户类型" })
            .Insert("Hi_Domain", new { Domain = "UTYPE2", DomainDesc = "用户类型2" }).ToSql();
            //int _effect=sqlClient
            //.Insert("Hi_Domain", new { Domain = "UTYPE", DomainDesc = "用户类型" })
            //.Insert("Hi_Domain", new { Domain = "UTYPE2", DomainDesc = "用户类型2" })
            //.ExecCommand();

            Console.WriteLine($"[{_times}]条数据 预热完成正在插入");

            Stopwatch watch = Stopwatch.StartNew();
            int _effect2 = sqlClient
            .Insert("Hi_Domain", lstobj)
            .ExecCommand();


            //string _sql2 = sqlClient
            //.Insert("Hi_Domain", lstobj)
            //.ToSql();

            watch.Stop();
            Console.WriteLine($"[{_times}]条数据,耗时：{watch.Elapsed.ToString()}");

            //int _effect=sqlClient
            //    .Insert("H_TEST", new { UNAME = "UTYPE9", UNAME2 = "用户类型9" })
            //    .Insert("Hone_Test", new { Username = "TOM4", Scount = 99 }).ExecCommand();

            //string _sql2 = sqlClient.Insert<H_Test>(new H_Test { UNAME = "UTYPEHA", UNAME2 = "TEST'" }).ToSql();

            //string _sql3 = sqlClient.Modi("Hi_Domain", new List<object> { new { Domain = "10097", DomainDesc = "用户类型10097" }, new { Domain = "10098", DomainDesc = "用户类型10098" } }).ToSql();
            //int _effect3 = sqlClient.Modi("Hi_Domain", new List<object> { new { Domain = "10097", DomainDesc = "用户类型10097" }, new { Domain = "10098", DomainDesc = "用户类型10098" } }).ExecCommand();



        }
    }
}
