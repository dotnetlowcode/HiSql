using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HiSql;
using System.Linq;
using System.Linq.Expressions;
using System.Data;
using System.Reflection.Emit;
//using SqlSugar;
using System.Transactions;
using System.Text.RegularExpressions;

namespace HiSql.Test
{


    //public delegate void SetValueDelegate(object target, object arg);

    //public static class DynamicMethodFactory
    //{
    //    public static SetValueDelegate CreatePropertySetter(PropertyInfo property)
    //    {
    //        if (property == null)
    //            throw new ArgumentNullException("property");

    //        if (!property.CanWrite)
    //            return null;

    //        MethodInfo setMethod = property.GetSetMethod(true);

    //        DynamicMethod dm = new DynamicMethod("PropertySetter", null,
    //            new Type[] { typeof(object), typeof(object) }, property.DeclaringType, true);

    //        ILGenerator il = dm.GetILGenerator();

    //        if (!setMethod.IsStatic)
    //        {
    //            il.Emit(OpCodes.Ldarg_0);
    //        }
    //        il.Emit(OpCodes.Ldarg_1);

    //        EmitCastToReference(il, property.PropertyType);
    //        if (!setMethod.IsStatic && !property.DeclaringType.IsValueType)
    //        {
    //            il.EmitCall(OpCodes.Callvirt, setMethod, null);
    //        }
    //        else
    //            il.EmitCall(OpCodes.Call, setMethod, null);

    //        il.Emit(OpCodes.Ret);

    //        return (SetValueDelegate)dm.CreateDelegate(typeof(SetValueDelegate));
    //    }

    //    private static void EmitCastToReference(ILGenerator il, Type type)
    //    {
    //        if (type.IsValueType)
    //            il.Emit(OpCodes.Unbox_Any, type);
    //        else
    //            il.Emit(OpCodes.Castclass, type);
    //    }
    //}
    class Program
    {

        [HiTable(TabName = "test")]
        class UserDemo1
        {
            [HiColumn(IsPrimary = true, FieldDesc = "用户名")]
            public string Name { get; set; }

            public string ClassName { get; set; }
            public int Age { get; set; }

            public int Score { get; set; }
            public DateTimeOffset Birth { get; set; }


        }

        class UserDemo2
        {
            public string Name { get; set; }

            public int Age { get; set; }

            public DateTimeOffset Birth { get; set; }

            public string ReName { get; set; }
        }

        class UserDemo3
        {
            public string ClassName { get; set; }
            public int Score { get; set; }
        }

        class TaskDemo
        {

            public Action<string, int> OnExecuteBefore;
            public Action<string, int> OnExecuteAfter;
            public TaskDemo()
            {

            }

            public void Execute(string sql, int time)
            {
                Console.WriteLine($"准备执行sql 线程号:{Thread.CurrentThread.ManagedThreadId}");
                if (OnExecuteBefore != null)
                {
                    Task task = Task.Run(() =>
                    {
                        OnExecuteBefore(sql, time);
                    });
                    //task.Start();
                }

                Console.WriteLine($"正在sql完成:{sql}");
                if (OnExecuteAfter != null)
                {
                    Task task = Task.Run(() =>
                    {
                        OnExecuteAfter(sql, time);
                    });
                    //task.Start();
                }
                Console.WriteLine($"执行sql完成");
            }

        }
        async static Task<string> GetContentAsync(string filename)
        {

            FileStream fs = new FileStream(filename, FileMode.Open);
            var bytes = new byte[fs.Length];
            //ReadAync方法异步读取内容，不阻塞线程
            Console.WriteLine($"开始读取文件 线程号:{Thread.CurrentThread.ManagedThreadId}");
            int len = await fs.ReadAsync(bytes, 0, bytes.Length);
            string result = Encoding.UTF8.GetString(bytes);
            return result;
        }
        static string filterparse(FilterDefinition filterDefinition)
        {
            Type _type = filterDefinition.GetType();

            MemberInfo[] members = _type.GetMembers();
            Console.WriteLine(_type.FullName);

            StringBuilder sb_sql = new StringBuilder();


            /*
             当object 类型时实际值与定义的值可能不一样
             */

            if (!string.IsNullOrEmpty(filterDefinition.Name) && filterDefinition.Value != null)
            {
                //Console.WriteLine($"value type:{filterDefinition.Value.GetType().FullName}");
                //表示匿名类
                if (filterDefinition.Value.GetType().FullName.IndexOf("_AnonymousType") >= 0)
                {

                }
                else if (filterDefinition.Value.GetType().FullName.IndexOf("FilterDefinition") > 0)
                {

                }
                else if (filterDefinition.Value.GetType().FullName.IndexOf("RangDefinition") > 0)
                {
                    RangDefinition _rang = (RangDefinition)filterDefinition.Value;
                    if (filterDefinition.OpFilter == OperType.BETWEEN)
                    {
                        Console.WriteLine($"\t low:{_rang.Low } hight:{_rang.High}");

                        sb_sql.Append($" {filterDefinition.Name} BETWEEN {_rang.Low } and {_rang.High}");
                    }
                    else
                    {
                        throw new Exception($"RangDefinition 只能用于BETWEEN");
                    }
                }

                else if (filterDefinition.OpFilter == OperType.IN)
                {
                    //当为IN 时 value 的值类型为List<String>
                    if (filterDefinition.Value is List<string>)
                    {

                        sb_sql.Append($" {filterDefinition.Name} in ({(filterDefinition.Value as List<string>).ToFieldString() })");
                        Console.WriteLine($"\t Value:{(filterDefinition.Value as List<string>).ToFieldString() }");
                    }
                    else
                        throw new Exception($"当操作符为IN时 Value只能为List<string>");
                }
                else if (filterDefinition.Value.GetType().FullName.IndexOf("System.") == 0)
                {
                    if (filterDefinition.OpFilter == OperType.EQ)
                    {
                        sb_sql.Append($" {filterDefinition.Name} = '{filterDefinition.Value}'");
                    }
                    else if (filterDefinition.OpFilter == OperType.GT)
                    {
                        sb_sql.Append($" {filterDefinition.Name} > '{filterDefinition.Value}'");
                    }
                    else if (filterDefinition.OpFilter == OperType.LT)
                    {
                        sb_sql.Append($" {filterDefinition.Name} < '{filterDefinition.Value}'");
                    }
                    else
                    {
                        throw new Exception("暂不支持 该方法");
                    }

                    Console.WriteLine($"\t Value:{filterDefinition.Value.ToString()}");
                }



            }
            else if (string.IsNullOrEmpty(filterDefinition.Name) && filterDefinition.Value != null && filterDefinition.LogiType == LogiType.OR)
            {
                //或
                //Console.WriteLine($"value type:{filterDefinition.Value.GetType().FullName}");
                if (filterDefinition.Value.GetType().FullName.IndexOf("List") >= 0)
                {
                    if (filterDefinition.Value is List<FilterDefinition>)
                    {
                        List<FilterDefinition> _lsto = (List<FilterDefinition>)filterDefinition.Value;
                        if (_lsto.Count > 0) sb_sql.Append("(");
                        for (int i = 0; i < _lsto.Count; i++)
                        {
                            if (i == _lsto.Count - 1)
                            {
                                sb_sql.Append($" {filterparse(_lsto[i])} ");
                            }
                            else
                            {
                                sb_sql.Append($" {filterparse(_lsto[i])} or ");
                            }
                        }
                        if (_lsto.Count > 0) sb_sql.Append(")");
                        //foreach (FilterDefinition filter in _lsto)
                        //{
                        //    filterparse(filter);
                        //}

                    }
                    else if (filterDefinition.OpFilter == OperType.IN)
                    {
                        //字符串范围
                        //当为IN 时 value 的值类型为List<String>
                        if (filterDefinition.Value is List<string>)
                        {
                            sb_sql.Append($" {filterDefinition.Name} in ({(filterDefinition.Value as List<string>).ToFieldString() })");
                            Console.WriteLine($"\t Value:{(filterDefinition.Value as List<string>).ToFieldString() }");
                        }
                        else
                            throw new Exception($"当操作符为IN时 Value只能为List<string>");
                    }

                }
                else if (filterDefinition.Value.GetType().FullName.IndexOf("FilterDefinition") >= 0)
                {
                    filterparse(filterDefinition.Value as FilterDefinition);
                }
                else if (filterDefinition.Value.GetType().FullName.IndexOf("RangDefinition") > 0)
                {
                    RangDefinition _rang = (RangDefinition)filterDefinition.Value;
                    if (filterDefinition.OpFilter == OperType.BETWEEN)
                    {
                        sb_sql.Append($" {filterDefinition.Name} BETWEEN {_rang.Low } and {_rang.High}");
                        Console.WriteLine($"\t low:{_rang.Low } hight:{_rang.High}");
                    }
                    else
                    {
                        throw new Exception($"RangDefinition 只能用于BETWEEN");
                    }
                }
                else if (filterDefinition.Value.GetType().FullName.IndexOf("System.") == 0)
                {
                    if (filterDefinition.OpFilter == OperType.EQ)
                    {
                        sb_sql.Append($" {filterDefinition.Name} = '{filterDefinition.Value}'");
                    }
                    else if (filterDefinition.OpFilter == OperType.GT)
                    {
                        sb_sql.Append($" {filterDefinition.Name} > '{filterDefinition.Value}'");
                    }
                    else if (filterDefinition.OpFilter == OperType.LT)
                    {
                        sb_sql.Append($" {filterDefinition.Name} < '{filterDefinition.Value}'");
                    }
                    else
                    {
                        throw new Exception("暂不支持 该方法");
                    }

                    Console.WriteLine($"\t Value:{filterDefinition.Value.ToString()}");
                }
            }

            else
                throw new Exception("无效过滤条件！");

            //foreach (MemberInfo mem in members.Where(m => m.MemberType == MemberTypes.Property).ToList())
            //{
            //    var p = _type.GetProperties().Where(_p => _p.MemberType == MemberTypes.Property && _p.CanRead == true && _p.CanWrite == true && _p.Name == mem.Name).FirstOrDefault();
            //    if (p != null)
            //    {

            //        Console.WriteLine($"{mem.Name},{mem.GetType().FullName},Property:{p.Name},{p.PropertyType.FullName} value type：{_type.GetProperty(mem.Name).GetValue(filterDefinition).GetType().FullName}");

            //    }



            //}
            return sb_sql.ToString();
        }


        static void Main(string[] args)
        {

            //string _regs = "(?<=\\d)(?=(?:\\d{3})+(?!\\d))";
            //string _regs = "(?<=\\d)(?=(?:\\d{4})+(?!\\d))";
            //Regex reg = new Regex(_regs);

            ////Match match = reg.Match("145411");
            ////string sr = reg.Replace("1451454545414411元", ",");
            //string sr = reg.Replace("15084942050", "-");

            //if (sr != null)
            //{ 

            //}


            
            Dictionary<string, string> _dic_key = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { "uname","tansar"},{ "unamE", "30"} };
         
            Type type = _dic_key.GetType();
            if (type == typeof(Dictionary<string,object>))
            {
                if (type.Name != "")
                {

                }
            }
            

            




            Stopwatch sw = new Stopwatch();




            #region task demo
            /*
            Console.WriteLine($"当前主线程号{Thread.CurrentThread.ManagedThreadId}");


            Task task = new Task(()=> {
                Thread.Sleep(100);
                Console.WriteLine($" 当前线程号{Thread.CurrentThread.ManagedThreadId}");
            });

            Task task2 = Task.Factory.StartNew(() => {
                Thread.Sleep(100);
                Console.WriteLine($" 当前线程号{Thread.CurrentThread.ManagedThreadId}");
            });

            Task task3 = Task.Factory.StartNew(() => {
                Thread.Sleep(100);
                Console.WriteLine($" 当前线程号{Thread.CurrentThread.ManagedThreadId}");
            });
            Task task4 = Task.Factory.StartNew(() => {
                Thread.Sleep(100);
                Console.WriteLine($" 当前线程号{Thread.CurrentThread.ManagedThreadId}");
            });
            */
            #endregion

            #region test task
            /*
            TaskDemo mydemo = new TaskDemo();
            mydemo.OnExecuteBefore = (string a, int b) =>
            {
                Thread.Sleep(b);

                Console.WriteLine($"执行前调用 线程号{Thread.CurrentThread.ManagedThreadId}");
            };


            mydemo.OnExecuteAfter= (string a, int b) =>
            {
                Thread.Sleep(b);

                Console.WriteLine($"执行后调用 线程号{Thread.CurrentThread.ManagedThreadId}");
            };


            mydemo.Execute("select * from test", 0);


            string _content = GetContentAsync(@"D:\Project\code\20200521.txt").Result;
            Console.WriteLine($"文档内容:{_content}");
            */
            #endregion

            #region 缓存测试



            /*
             * 
            string _sql = "select * from dbo.user";
            CacheContext.MCache.SetCache($"hisql:query:{(_sql ).ToMd5()}", _sql );
            for (int i = 0; i < 10; i++)
            { 

                CacheContext.MCache.SetCache($"hisql:query:{(_sql+i.ToString()).ToMd5()}",new TabInfo());
                var tabinfo=CacheContext.MCache.GetCache<TabInfo>($"hisql:query:{(_sql + i.ToString()).ToMd5()}");
            }


            string _key= CacheContext.MCache.GetOrCreate<string>("key", ()=> {
                var str = "hello world";
                return str;
            },2);
            Console.WriteLine(_key);

            _key = CacheContext.MCache.GetCache<string>("key");
            Console.WriteLine(_key);
            _key = CacheContext.MCache.GetOrCreate<string>("key", () => {
                var str = "hello world 1223";
                return str;
            });
            Console.WriteLine(_key);

            Console.WriteLine("解析完成");
            */

            #endregion


            #region hisql测试

            //创建基础连接 当实际需要执行SQL语句会 再会自动创建数据库连接



            HiSqlClient sqlclient = new HiSqlClient(
                 new ConnectionConfig()
                 {
                     DbType = DBType.SqlServer,
                     DbServer = "local-HoneBI",
                     //ConnectionString = "server=192.168.1.90,8433;uid=sa;pwd=Hone@123;database=HoneBI",
                     ConnectionString = "server=(local);uid=sa;pwd=Hone@123;database=Hone;",//; MultipleActiveResultSets = true;
                     Schema = "dbo",
                     IsEncrypt = true,
                     IsAutoClose = false,
                     SqlExecTimeOut = 60000,

                     AppEvents = new AopEvent()
                     {
                         OnDbDecryptEvent = (connstr) =>
                         {
                             //解密连接字段
                             //Console.WriteLine($"数据库连接:{connstr}");

                             return connstr;
                         },
                         OnLogSqlExecuting = (sql, param) =>
                         {
                             //sql执行前 日志记录 (异步)

                             //Console.WriteLine($"sql执行前记录{sql} time:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")}");
                         },
                         OnLogSqlExecuted = (sql, param) =>
                         {
                             //sql执行后 日志记录 (异步)
                             //Console.WriteLine($"sql执行后记录{sql} time:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")}");
                         },
                         OnSqlError = (sqlEx) =>
                         {
                             //sql执行错误后 日志记录 (异步)
                             Console.WriteLine(sqlEx.Message.ToString());
                         },
                         OnTimeOut = (int timer) =>
                           {
                             //Console.WriteLine($"执行SQL语句超过[{timer.ToString()}]毫秒...");
                         }
                     }
                 }
                 );




            #region 语法糖测试
            ////需要实现扩展方法Add

            //HsonDocument hdoc = new HsonDocument {
            //    { "UserName11111","tansar"},
            //    { "Age",33}
            //};


            //Dictionary<string, FilterDefinition> dic_where=new Dictionary<string, FilterDefinition>{
            //    { "UserName",new FilterDefinition{ Name="" } }
            //};
            //List<FilterDefinition> dic_list = new List<FilterDefinition> { 
            //    new FilterDefinition{ Name="UserName",OpFilter=OperType.EQ,Value="tansar"},
            //    new FilterDefinition{ Name="DepId",OpFilter=OperType.IN,Value=new List<string>{"1001","1002" } },
            //    new FilterDefinition{ Name="Utype",OpFilter=OperType.GT,Value=1001 },
            //    new FilterDefinition{ Name="Price",OpFilter=OperType.BETWEEN,Value=new RangDefinition(){ Low=199,High=2999} },
            //    new FilterDefinition{   LogiType=LogiType.OR,Value=new List<FilterDefinition>{
            //        new FilterDefinition{ Name="UserName",OpFilter=OperType.EQ,Value="TGM"},
            //        new FilterDefinition{ Name="DepId",OpFilter=OperType.IN,Value=new List<string>{"1001","1002" } },
            //    } }

            //};
            //StringBuilder sb_sql = new StringBuilder();
            //for (int i = 0; i < dic_list.Count; i++)
            //{
            //    if (i == dic_list.Count - 1)
            //    {
            //        sb_sql.Append($"{filterparse(dic_list[i])}");
            //    }else
            //        sb_sql.Append($"{filterparse(dic_list[i])} and ");
            //}
            //Console.WriteLine(sb_sql.ToString());


            //foreach (FilterDefinition filterDefinition in dic_list)
            //{
            //    filterparse(filterDefinition);

            //    //foreach (PropertyInfo p in _type.GetProperties().Where(_p=>_p.MemberType==MemberTypes.Property && _p.CanRead==true && _p.CanWrite==true ).ToList())
            //    //{
            //    //    Console.WriteLine($"Name:{p.Name} Type:{p.PropertyType.FullName}");

            //    //}
            //    //members[12].MemberType



            //}

            //Console.WriteLine(dic_list[1].Value.GetType().Name);

            //2889,3999,3999  

            /*
             
            sqlclient.Query("UserList","A").Field("UserName  as Uname,Utype").Join("UserType","B").on({"A.Utype",FilterNum.EQ,"B.Utype"}).Where().OrderBy().Top(10).Take(100)
            sqlclient.Query("UserList").Join("UserType","B").on({"A.Utype",FilterNum.EQ,"B.Utype"}).Where
            
            //sqlclient.Query("select * from dbo.UserList")


            merage into 
            sqlclient.Query<TabStruct>("UserList","A").Field("Uname as UserName,").Join("UserType","B").on({"A.Utype",FilterNum.EQ,"B.Utype"}).Where().OrderBy().Top(10).Take(100).ToList()
             */



            //var type = typeof(string);
            //if (type.ToDbFieldType() == FieldType.NVARCHAR)
            //{
            //    //Console.WriteLine($"数据类型值:{FieldType.NVARCHAR}");
            //}

            //string _sql11 = sqlclient.Query<TabModel>(x=>x.TabName.Contains("sys")).ToSql();

            //string _sql3 = sqlclient.Query<TabModel>(x => (x.TabName.Contains("sys") || x.TabName=="USR02") && x.TabReName=="TabName").ToSql();
            //string _sql4 = sqlclient.Query<TDynamic, TDynamic>((x, y) => (x.Field<string>("TabName").Contains("USR") && y.Field<Decimal>("SalesPirce")>100)).ToSql();


            //sqlclient.Query<TabModel>();

            //Console.WriteLine($"实体lambda写法:sqlclient.Query<TabModel>(x=>x.TabName.Contains(\"sys\")).ToSql()");

            //Console.WriteLine($"生成sql:\n{_sql}");

            //Console.WriteLine("");
            //Console.WriteLine("");
            //Console.WriteLine("");

            //string _sql2 = sqlclient.Query<TDynamic>(x => x.Field<string>("TabName").Contains("sys")).ToSql();
            //Console.WriteLine($"动态类lambda写法:sqlclient.Query<TDynamic>(x => x.Field<string>(\"TabName\").Contains(\"sys\")).ToSql()");

            //Console.WriteLine($"生成sql:\n{_sql2}");

            #endregion




            #region  sqlclient查询

            //单表查询
            //DataTable DT_RESULT1= sqlclient.Query("DataDomain").Field("Domain").Sort(new SortBy { { "createtime" } }).ToTable();
            //DataTable DT_RESULT2 = sqlclient.Query("DataDomain").As("a").Field("a.Domain").Sort(new SortBy { { "a.createtime" } }).ToTable();
            //DataTable DT_RESULT3 = sqlclient.Query("DataDomain").As("a").Field("a.Domain").Sort(new SortBy { { "a.createtime" } }).Skip(1).Take(100).ToTable();

            //DataTable DT_RESULT2= sqlclient.Query("DataDomain").Field("*").Sort("createtime ,lasttime ").ToTable();

            sw.Start();

            //for (int i = 0; i < 1000; i++)
            //{
            //    List<TDynamic> lstresult = sqlclient.Query("DataDomain").Field("*").Sort(new SortBy { { "createtime" } }).Skip(1).Take(1000).ToDynamic();
            //}

            //List<Hi_Domain> lstefresult = sqlclient.Query("Hi_Domain").Field("*").Sort(new SortBy { { "createtime", SortType.ASC } }).Skip(1).Take(1000).ToList<Hi_Domain>();
            //List<Hi_Domain> lstefresult = sqlclient.Query("Hi_Domain").Field("*").Sort("createtime asc","moditime").Skip(1).Take(1000).ToList<Hi_Domain>();
            


            //string _json= sqlclient.Query("DataDomain").Field("Domain").Sort(new SortBy { { "createtime" } }).ToJson();


            sw.Stop();
            TimeSpan ts2 = sw.Elapsed;
            Console.WriteLine("sw总共花费{0}ms.", ts2.TotalMilliseconds);

            //复杂统计分组统计DEMO
            //string _sql = sqlclient.Query(
            //    sqlclient.Query("Hi_FieldModel").Field("*").Where(new Filter { { "TabName", OperType.IN,
            //            sqlclient.Query("Hi_TabModel").Field("TabName").Where(new Filter { {"TabName",OperType.IN,new List<string> { "Hone_Test", "H_TEST" } } })
            //        } }),
            //    sqlclient.Query("Hi_FieldModel").Field("*").Where(new Filter { { "TabName", OperType.EQ, "DataDomain" } }),
            //    sqlclient.Query("Hi_FieldModel").Field("*").Where(new Filter { { "TabName", OperType.EQ, "Hi_FieldModel" } })
            //)
            //    .Field("TabName", "count(*) as CHARG_COUNT")
            //    .WithRank(DbRank.DENSERANK, DbFunction.NONE, "TabName", "rowidx1", SortType.ASC)
            //    .WithRank(DbRank.ROWNUMBER, DbFunction.COUNT, "*", "rowidx2", SortType.ASC)
            //    .WithRank(DbRank.RANK, DbFunction.COUNT, "*", "rowidx3", SortType.ASC)
            //    .Group(new GroupBy { { "TabName" } }).ToSql();

            string _json2 = sqlclient.Query(
                sqlclient.Query("Hi_FieldModel").Field("*").Where(new Filter { { "TabName", OperType.IN,
                        sqlclient.Query("Hi_TabModel").Field("TabName").Where(new Filter { {"TabName",OperType.IN,new List<string> { "Hone_Test", "H_TEST" } } })
                    } }),
                sqlclient.Query("Hi_FieldModel").Field("*").Where(new Filter { { "TabName", OperType.EQ, "DataDomain" } }),
                sqlclient.Query("Hi_FieldModel").Field("*").Where(new Filter { { "TabName", OperType.EQ, "Hi_FieldModel" } })
            )
                .Field("TabName", "count(*) as CHARG_COUNT")
                .WithRank(DbRank.DENSERANK, DbFunction.NONE, "TabName", "rowidx1", SortType.ASC)
                .WithRank(DbRank.ROWNUMBER, DbFunction.COUNT, "*", "rowidx2", SortType.ASC)
                .WithRank(DbRank.RANK, DbFunction.COUNT, "*", "rowidx3", SortType.ASC)
                .Group(new GroupBy { { "TabName" } }).ToJson();
            

            Console.WriteLine(_json2);

            //DataTable DT_RESULT = sqlclient.Query("Hi_FieldModel", "A").Field("A.FieldName as Fname")
            //    .Join("Hi_TabModel").As("B").On(new JoinOn { { "A.TabName", "B.TabName" } }) 
            //    .Where(new Filter {
            //        {"A.TabName", OperType.EQ, "Hi_FieldModel"},
            //        {"A.FieldType",OperType.BETWEEN,new RangDefinition(){ Low=10,High=99} },
            //    })
            //    .Group(new GroupBy { { "A.FieldName" } })
            //    .Skip(1).Take(10).ToTable();


            //带函数查询

            //DataTable dt_resultfun = sqlclient.Query("Hi_FieldModel", "A").Field("A.FieldName    as    Fname", "count(*) as avgFieldLen")
            //    .Join("Hi_TabModel").As("B").On(new JoinOn { { "A.TabName", "B.TabName" } })
            //    .Where(new Filter {
            //        {"A.TabName", OperType.EQ, "Hi_FieldModel"},
            //        {"A.FieldType",OperType.BETWEEN,new RangDefinition(){ Low=10,High=99} },

            //    })
            //    .Group(new GroupBy { { "A.FieldName" } })
            //    .Skip(2).Take(10)
            //    .ToTable();


            //string _sql = sqlclient.Query("Hi_FieldModel", "A").Field("A.FieldName    as    Fname" ,"count(*) as avgFieldLen").Join("Hi_TabModel").As("B").On(new JoinOn { { "A.TabName", "B.TabName" } })                                                                                                                                                             
            //    .Where(new Filter {
            //        {"A.TabName", OperType.EQ, "Hi_FieldModel"},
            //        {"A.FieldType",OperType.BETWEEN,new RangDefinition(){ Low=10,High=99} },

            //    })
            //    .Group(new GroupBy { { "A.FieldName" } })
            //    .Skip(2).Take(10)
            //    .ToSql();

            /*
            //复杂 分页查询
            DataTable DT_RESULT1 = sqlclient.Query("Hi_FieldModel", "A").Field("A.FieldName    as    Fname", "B.*").Join("Hi_TabModel").As("B").On(new JoinOn { { "A.TabName", "B.TabName" } }) //, "B.*" 
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
                .Skip(2).Take(10).ToTable();//, { "User.Name" }



            //分组
            DataTable DT_RESULT = sqlclient.Query("Hi_FieldModel", "A").Field("A.FieldName    as    Fname").Join("Hi_TabModel").As("B").On(new JoinOn { { "A.TabName", "B.TabName" } }) //, "B.*" 
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
                .Group(new GroupBy { { "A.FieldName"} })
                //.Sort(new SortBy { { "A.SortNum", SortType.ASC } })
                .Skip(1).Take(10).ToTable();//, { "User.Name" }
            
            */

            //if (DT_RESULT2.Rows.Count > 0)
            //{ }
            //Console.WriteLine(_sql);

            //Console.WriteLine(_sql);
            ////Tuple<bool, FieldDefinition> result = Tool.CheckQueryField("A.TabName    as    name");
            //Tuple<bool, FieldDefinition, FieldDefinition> result = Tool.CheckOnField("#UserName.Id    =    ulist.ddd");
            //if (result.Item1 == true)
            //{ 

            //}

            #endregion

            #region CodeFirst

            //SqlSugarClient db = new SqlSugarClient(new SqlSugar.ConnectionConfig()
            //{
            //    ConnectionString = "server=(local);uid=sa;pwd=Hone@123;database=Hone",//连接符字串
            //    DbType = SqlSugar.DbType.SqlServer,
            //    IsAutoCloseConnection = true
            //});





            //数据插入语法糖
            //sqlclient
            //    .Insert("H_TEST", new { UNAME = "UTYPE9", UNAME2 = "用户类型9" } )
            //    .Insert("Hone_Test",new { Username ="TOM3", Scount =89})
            //    .ExecCommand();


            //动态的实现类创建
            IDMInitalize dMBuilder = Instance.CreateInstance<IDMInitalize>($"{Constants.NameSpace}.{sqlclient.Context.CurrentConnectionConfig.DbType.ToString()}{DbInterFace.DM.ToString()}");
            IDMTab dMTab = Instance.CreateInstance<IDMTab>($"{Constants.NameSpace}.{sqlclient.Context.CurrentConnectionConfig.DbType.ToString()}{DbInterFace.DM.ToString()}");
            Tuple<HiTable, List<HiColumn>> tabtuple = dMBuilder.BuildTabStru(typeof(Hi_Domain));

            TabInfo tabinfo = dMBuilder.BuildTab(typeof(Hi_Domain));
            //tabinfo.TabModel.IsGlobalTemp = true;
            tabinfo.TabModel.TabName = $"##{tabinfo.TabModel.TabName}";
            dMTab.Context = sqlclient.Context;
            sqlclient.Context.MCache.SetCache($"{tabinfo.TabModel.TabName}", tabinfo);

            
            int effect=dMTab.BuildTabCreate(tabinfo);
            if (effect > 0)
                Console.WriteLine("表创建成功");
            else
                Console.WriteLine("表创建失败 该表已经存在");


            sqlclient.Query("Hi_Domain").Field("*").Sort("createtime asc", "moditime").Skip(1).Take(1000).Insert("#Hi_Domain");
            List<Hi_Domain> lstefresult = sqlclient.Query("##Hi_Domain").Field("*").Sort("createtime asc", "moditime").Skip(1).Take(1000).ToList<Hi_Domain>();
            //sqlclient.Insert(tabinfo.TabModel.TabName, new { Domain="test",DomainDesc="测试tset" }).ExecCommand();


            using (TransactionScope tsCope = new TransactionScope())
            { 
                
            }


                //DataTable dts = sqlclient.Context.DBO.GetDataTable($"select * from {tabinfo.TabModel.TabName}", null);
                //DataTable dts2=sqlclient.Query($"{tabinfo.TabModel.TabName}").Field("*").Sort(new SortBy { { "createtime" } }).ToTable();

            //string _sql2 = dMTab.BuildTabCreateSql(tabinfo);





            //获取数据库支撑清单
            List<string> _dbcurrsupport = Constants.DbCurrentSupportList;
            List<string> _dbsupport = Constants.DbSupportList;





            //Console.WriteLine(_sql);

            //var cl = new { Domain = "", ElementValue = "", ElementDesc = "", SortNum = "" };
            //Type t = cl.GetType();

            //string str = "ＷＷabcdef0g";
            //Stopwatch watch4 = Stopwatch.StartNew();
            //int _times = 100000;
            ////Console.WriteLine("中文按1字符长度:" + str.Length + "中文按两个长度:" + str.LengthZH());
            ////sqlbuilder.Insert("Hi_DataElement", new { domain = "UTYPE" });//, ElementValue = "", ElementDesc = "", SortNum = ""


            //Console.WriteLine($"SqlSugar 有实体批量插入 {_times}条");
            //db.Insertable(new Dictionary<string, object>() {
            //    {  "Domain",$"UTYPE"} ,
            //    { "DomainDesc","用户类型"} }
            //).AS("Hi_Domain2").ExecuteCommand(); ;
            //watch4.Reset();

            //List<Hi_Domain2> lstobj2 = new List<Hi_Domain2>();
            //for (int i = 0; i < _times; i++)
            //{
            //    lstobj2.Add(new Hi_Domain2() { Domain="us"+i.ToString(),DomainDesc= $"s用户类型{i}" });
            //}
            //watch4.Start();
            ////db.Insertable(lstobj2).AS("Hi_Domain2").ExecuteCommand();


            //watch4.Stop();
            //Console.WriteLine($"耗时：{watch4.Elapsed.ToString()}");

            //List<object> lstobj = new List<object>();
            //for (int i = 0; i < _times; i++)
            //{
            //    lstobj.Add(new { Domain = $"{i}", DomainDesc = $"用户类型{i}" });
            //}

            sqlclient.Insert("Hi_Domain", new { Domain = "UTYPE", DomainDesc = "用户类型" }).ExecCommand();



            //Console.WriteLine($"Hisql 无实体批量插入 {_times}条");
            //watch4.Reset();
            //watch4.Start();
            //sqlclient.Insert("Hi_Domain", lstobj).ExecCommand();
            ////string _hisql = sqlclient.Insert("Hi_Domain", lstobj).ToSql();
            ////db.Context.Ado.ExecuteCommand(_hisql, new List<SugarParameter>() { });

            //watch4.Stop();
            //Console.WriteLine($"耗时：{watch4.Elapsed.ToString()}");



            //sqlclient.Insert("Hi_Domain", new { Domain = "UTYPE", DomainDesc = "用户类型" });
            //Console.WriteLine($"Hisql 无实体批量插入 {_times}条");
            //Stopwatch watch4 = Stopwatch.StartNew();
            //for (int i = 0; i < _times; i++)
            //{
            //    sqlclient.Insert("Hi_Domain", new { Domain = $"{i}", DomainDesc = $"用户类型{i}" });
            //}
            //watch4.Stop();
            //Console.WriteLine($"耗时：{watch4.Elapsed.ToString()}");

            //watch4.Reset();
            //watch4.Start();
            //db.Insertable(new Dictionary<string, object>() {
            //    {  "Domain",$"UTYPE"} ,
            //    { "DomainDesc","用户类型"} }
            //).AS("Hi_Domain2").ExecuteCommand(); ;
            //Console.WriteLine($"SqlSugar 有实体批量插入 {_times}条");
            //for (int i = 0; i < _times; i++)
            //{
            //    db.Insertable(new Dictionary<string, object>() {
            //    {  "Domain",$"{i}"} ,
            //    { "DomainDesc",$"用户类型{i}"} 

            //    }).AS("Hi_Domain2").ExecuteCommand();
            //}
            //watch4.Stop();
            //Console.WriteLine($"耗时：{watch4.Elapsed.ToString()}");



            //TabInfo tabinfo= sqlbuilder.GetTabStruct("Hi_Domain");

            //tabinfo = sqlbuilder.GetTabStruct("Hi_Domain");

            //sqlbuilder.Insert<Hi_DataElement>("", new Hi_DataElement() { Domain=""});

            //int ss= -1;
            //Console.WriteLine((int)true);

            //foreach (HiColumn hiColumn in tabtuple.Item2)
            //{
            //    Console.WriteLine(sqlbuilder.BuildFieldStatement(hiColumn));
            //}

            #endregion
            /*
             * {
             * {"UserName 'tansar'"}
             * }

             */

            //Console.WriteLine(_sql);


            //Console.WriteLine("Query ok");

            #endregion



            #region  反射性能测试Emmit 

            //UserDemo2 myuser = new UserDemo2();
            //PropertyInfo propInfo = typeof(UserDemo2).GetProperty("Name");
            //TDynamicFactory  dynamicFactory = new TDynamicFactory ();
            //List<TDynamicFactory.SetValueDelegate> setlist = new List<TDynamicFactory.SetValueDelegate>();
            //TDynamicFactory.SetValueDelegate setter2 =dynamicFactory.CreatePropertySetter(propInfo);
            //setlist.Add(setter2);
            ////SetValueDelegate setter2 = TDynamicFactory.CreatePropertySetter(propInfo);
            //Console.Write("EmitSet花费时间：        ");

            //Stopwatch watch1 = Stopwatch.StartNew();
            //for (int i = 0; i < 1000000; i++)
            //{
            //    setter2(myuser, $"tansar{i}");
            //}
            //watch1.Stop();
            //Console.WriteLine(watch1.Elapsed.ToString());
            //Console.Write("直接赋值花费时间：        ");
            //Stopwatch watch2 = Stopwatch.StartNew();

            //for (int i = 0; i < 1000000; i++)
            //{
            //    myuser.Name = $"tansar{i}";
            //}
            //watch2.Stop();
            //Console.WriteLine(watch2.Elapsed.ToString());
            //Console.Write("纯反射花费时间：　       ");
            //Stopwatch watch3 = Stopwatch.StartNew();

            //for (int i = 0; i < 1000000; i++)
            //{
            //    propInfo.SetValue(myuser, $"tansar{i}", null);
            //}
            //watch3.Stop();
            //Console.WriteLine(watch3.Elapsed.ToString());
            #endregion




            #region movecross 



            //List<UserDemo1> ulist = new List<UserDemo1>();
            //for (int i = 0; i < 100000; i++)
            //{
            //    UserDemo1 u1 = new UserDemo1();
            //    u1.Name = $"TGM{i.ToString()}";
            //    u1.Age = 33;
            //    u1.Birth = DateTime.Now.AddDays(0 - i);
            //    u1.Score = i % 100;
            //    u1.ClassName = "CLASS_A";
            //    ulist.Add(u1);

            //}

            //for (int i = 100000; i < 200000; i++)
            //{
            //    UserDemo1 u1 = new UserDemo1();
            //    u1.Name = $"TGM{i.ToString()}";
            //    u1.Age = 33;
            //    u1.Birth = DateTime.Now.AddDays(0 - i);
            //    u1.Score = i % 99;
            //    u1.ClassName = "CLASS_B";
            //    ulist.Add(u1);
            //}
            //UserDemo1 user1 = new UserDemo1();
            //user1.Name = "tansar";
            //user1.Age = 33;
            //user1.Birth = DateTime.Now;

            //for (int i = 0; i < 1000; i++)
            //{ 
            //    UserDemo2 user2 = new UserDemo2();
            //    user2 = user1.MoveCross(user2);

            //}



            //var u3 = new UserDemo3();
            //List<UserDemo3> u3Lst = ulist.Collect(u3);


            //foreach (var usr in u3Lst)
            //{
            //    Console.WriteLine($"className:{usr.ClassName} score:{usr.Score}");
            //}


            //var listSum = ulist.GroupBy(a => new
            //{
            //    ClassName = a.ClassName,
            //    //Score = a.Score
            //}).Select(g => new { ClassName = g.Key.ClassName, Score = g.Count() }).ToList();



            //Console.WriteLine($"Name:{user1.Name},Age:{user1.Age},Birth:{user1.Birth.ToString("yyyy-MM-dd")}");

            #endregion

            #region demo
            /*
            TabInfo tabInfo = new TabInfo();
            Type type = typeof(FieldModel);
            int idx = 0;
            List<FieldModel> fieldList = new List<FieldModel>();
            if (type!=null)
            {

                TabModel tabModel = new TabModel();

                tabModel.TabName = type.Name;
                tabModel.TabReName = type.Name;
                tabModel.DbServer = "(local)";
                tabModel.TabStoreType = TabStoreType.Row;
                tabModel.TabCacheType = TabCacheType.ALL;
                tabModel.TabStatus = TabStatus.NoActive;
                tabModel.TabType = TabType.Config;
                tabModel.IsSystem = true;
                tabModel.IsEdit = true;
                tabModel.IsLog = true;






                foreach (var attr in type.GetCustomAttributes())
                {
                    if (attr is HiTable)
                    {
                        HiTable hiattr = (HiTable)attr;
                        tabInfo.EntityName = type.Name;//实体名称
                        tabInfo.DbTabName = hiattr.TabName;

                        tabInfo.TabModel = hiattr;

                    }
                }


                foreach (PropertyInfo n in type.GetProperties())
                {

                    if (((System.Attribute[])n.GetCustomAttributes()).Length > 0)
                    {
                        foreach (var _attr in n.GetCustomAttributes())
                        {
                            if (_attr is HiColumn)
                            {
                                tabInfo.Columns.Add((HiColumn)_attr);
                            }
                        }
                    }
                    else
                    {
                        tabInfo.Columns.Add(
                            new HiColumn()
                            {
                                FieldName=n.Name,
                            }
                            );
                    }

                    FieldModel fieldModel = new FieldModel();
                    fieldModel.FieldName = n.Name;
                    fieldModel.FieldDesc = fieldModel.FieldName;
                    fieldModel.TabName = tabModel.TabName;
                    fieldModel.FieldType = n.DeclaringType;
                    fieldModel.IsSys = tabModel.IsSystem;
                    fieldModel.IsIdentity = false;
                    fieldModel.IsPrimary = false;
                    fieldModel.IsSearch = true;
                    fieldModel.IsShow = false;
                    fieldModel.SrchMode = SrchMode.Single;
                    fieldModel.IsRefTab = false;
                    fieldModel.RefTab = "";
                    fieldModel.RefField = "";
                    fieldModel.RefFieldDesc = "";
                    fieldModel.RefFields = "";
                    fieldModel.RefWhere = "";
                    fieldList.Add(fieldModel);

                    //Console.WriteLine($"Index={idx} FieldName:{n.Name}");
                    idx++;
                }
            }
            */
            #endregion

            #region lambda 表达式

            //ConstantExpression _constExp = Expression.Constant("tansar", typeof(string));
            //ParameterExpression _paramExp = Expression.Parameter(typeof(string), "MyParameter");


            //MethodCallExpression methodCallExpression = Expression.Call(typeof(Console)
            //    .GetMethod("WriteLine", new Type[] { typeof(string) }), _paramExp);

            //Expression<Action<string>> consoleLambdaExp = Expression.Lambda<Action<string>>(methodCallExpression, _paramExp);

            //consoleLambdaExp.Compile()("hello world");

            //string sql1 = ExpContext.GetWhereByLambda<Users>(x => x.Name.StartsWith("test") && x.Id > 2, DBType.SqlServer);
            //string sql2 = ExpContext.GetWhereByLambda<Users>(x => x.Name.EndsWith("test") && (x.Id > 4 || x.Id == 3), DBType.Oracle);
            //string sql3 = ExpContext.GetWhereByLambda<Users>(x => x.Name.Contains("test") && (x.Id > 4 && x.Id <= 8), DBType.SqlLite);
            //string sql4 = ExpContext.GetWhereByLambda<Users>(x => x.Name == "FengCode" && (x.Id >= 1), DBType.MySql);



            /*
            string sql5 = ExpContext.Query<TDynamic,TDynamic>((x ,y)=> 
            x.Field<string>("ID").StartsWith("T") && 
            x.Field<int>("Score")>100 || 
            ( x.Field<string>("Name").Contains("TGM")
            && x.Field<int>("Score") > y.Field<int>("AvgScore")
            ), DBType.SqlServer);
            */


            //Console.WriteLine($"sqlserver:{sql1}");
            //Console.WriteLine($"oracle:{sql2}");
            //Console.WriteLine($"access:{sql3}");
            //Console.WriteLine($"mysql:{sql4}");
            /*
            int[] id = { 4, 5, 1, 3, 2, 7, 6 };
            string[] name = { "Tom", "Jack", "HelloWorld", "Visual Studio", "Gril", "Timmy", "Geo" };
            DataTable table = new DataTable("Student");
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("Name", typeof(string));
            for (int i = 0; i < id.Length; i++)
            {
                table.Rows.Add(new object[] { id[i], name[i] });
            }

            var students = table.AsEnumerable();
            var result = students.OrderBy(x => x.Field<int>("ID"));
            Console.WriteLine("ID" + "\t" + "Name");
            foreach (DataRow row in result)
            {
                Console.WriteLine(row["ID"].ToString() + "\t" + row["Name"].ToString());
            }*/

            #endregion




            //Console.WriteLine($"主线程执行完成 线程号{Thread.CurrentThread.ManagedThreadId}");
            string s = Console.ReadLine();
            //Console.WriteLine("Hello World!");
        }
        
    }
}
