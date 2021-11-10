# HiSql 
目前的ORM框架对是基于实体的，包发生变化或增加字段时比较麻烦，所以有了开发无体实ORM的想法,结合项目中对于数据库中操作的痛点通过HiSql来实现解决
支持常用的数据库且国内第一个支持Hana的ORM框架
### 特点
1. 支持无实体数据交互，（无需要创建实体类）
2. 数据动态检测（类型，长度 与表结构预先匹配）
3. 语法更帖近于原生SQL
4. 支持超时监控（如监控过5S的执行的SQL语 并记录）
5. 支持多种库，可自主选择需要支持的库

### 项目引用

1. 引用HiSql.dll文件
2. 根据使用数据库的需要可以引用以下数据库实现的sdk
   1. HiSql.sqlserver.dll 暂进只实现了这个
   2. HiSql.hana.dll
   3. HiSql.mysql.dll
   4. HiSql.oracle.dll


### 配置数据库连接

```c#
HiSqlClient sqlclient = new HiSqlClient(
                 new ConnectionConfig()
                 {
                     DbType = DBType.SqlServer,
                     DbServer="local-HoneBI",
                     ConnectionString = "server=(local);uid=sa;pwd=H---#$3;database=hisql;",//; 
                     Schema = "dbo",
                     IsEncrypt = true,
                     IsAutoClose = false,
                     SqlExecTimeOut=60000,
                     
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
                         OnTimeOut=(int timer)=>
                         {
                             //Console.WriteLine($"执行SQL语句超过[{timer.ToString()}]毫秒...");
                         }
                     }
                 }
                 );

```

### 初始安装 
```c#
sqlclient.CodeFirst.InstallHisql();

```


### 表数据插入
1. 单表单条数据插入
    Insert语法
    参1："Hi_DataElement" 

        可以是一个物理表也可以是临时表
    临时表的写法如 "#Hi_DataElement" 用1个#号表示 本地临时表 两个#号表示是全局临时表
    变更表写法如"@Hi_DataElement" 注：仅对sqlserver数据库支持

    参2：new { domain = "UTYPE" }

        可以是匿名类也可以是实体类，匿名类的属性不区分大小写 如字段写的是[domain] 数据库中的字段为Domain 也默认就是对应的是Domain 

    注：HiSql将会自动校验插入的值的类型，长度是否与底层目标数据库相匹配如果不匹配将会检测报错


    ```c#
    sqlclient.Insert("Hi_DataElement", new { domain = "UTYPE" });

    ```
    以上语句并不会立即执行插入如要执行插入如下所示
    ```c#
    sqlclient.Insert("Hi_DataElement", new { domain = "UTYPE" }).ExecCommand();
    ```
    如果要监控该语句生成的目标数据库的sql语句可以在连接配置事件
    执行前或执行后都可以监控到
    ```c#
    OnLogSqlExecuting = (sql, param) =>
    {
        //sql执行前 日志记录 (异步)

        //Console.WriteLine($"sql执行前记录{sql} time:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")}");
    },
    OnLogSqlExecuted = (sql, param) =>
    {
        //sql执行后 日志记录 (异步)
        //Console.WriteLine($"sql执行后记录{sql} time:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")}");
    }
    ```


    也可以直接返回Sql语句如下所示
    ```c#
    sqlclient.Insert("Hi_DataElement", new { domain = "UTYPE" }).ToSql();
    ```


2. 批量插入数据

    对表操作无需像其它框架一样需要体实体,HiSql可以不要实体，当然如果习惯了用实体也可以使用实体插入


    ```c#
    int _times = 100000;
    List<object> lstobj = new List<object>();
    for (int i = 0; i < _times; i++)
    {
        lstobj.Add(new { Domain = $"{i}", DomainDesc = $"用户类型{i}" });

    }
    sqlclient.Insert("Hi_Domain", new { Domain = "UTYPE", DomainDesc = "用户类型" }).ExecCommand();
    ```
---

### 表数据查询

1. 单表查询
    
    返回结构可以ToTable ToJson ToList ToDynamic 本例演示ToTable
    1. 查询返回一个DataTable 同时这里也用到了排序
        ```c#
        DataTable DT_RESULT1= sqlclient.Query("DataDomain").Field("Domain").Sort(new SortBy { { "createtime" } }).ToTable();
        ```

        对表定义别名
        ```c#
        DataTable DT_RESULT2 = sqlclient.Query("DataDomain").As("a").Field("a.Domain").Sort(new SortBy { { "a.createtime" } }).ToTable();
        ```
    2. 查询返回一个实体
        查询该表的所有字段在Field方法中传入"*"

        ```c#
        List<Hi_Domain> lstefresult = sqlclient.Query("Hi_Domain").Field("*").Sort("createtime asc", "moditime").ToList<Hi_Domain>();

        ```
    3. 分页查询
        ```c#
        DataTable DT_RESULT3 = sqlclient.Query("DataDomain").As("a").Field("a.Domain").Sort(new SortBy { { "a.createtime" } }).Skip(1).Take(100).ToTable();

        ```

2. 多表简单关联查询

    1. 多表关联并实现条件过滤且进行分页
        ```c#
        DataTable DT_RESULT = sqlclient.Query("Hi_FieldModel", "A").Field("A.FieldName as Fname")
                    .Join("Hi_TabModel").As("B").On(new JoinOn { { "A.TabName", "B.TabName" } }) 
                    .Where(new Filter {
                        {"A.TabName", OperType.EQ, "Hi_FieldModel"},
                        {"A.FieldType",OperType.BETWEEN,new RangDefinition(){ Low=10,High=99} },
                    })
                    .Skip(1).Take(10).ToTable();
        ```
    2. 实现分组查询 group分组

        ```c#
        DataTable DT_RESULT = sqlclient.Query("Hi_FieldModel", "A").Field("A.FieldName as Fname")
                .Join("Hi_TabModel").As("B").On(new JoinOn { { "A.TabName", "B.TabName" } }) 
                .Where(new Filter {
                    {"A.TabName", OperType.EQ, "Hi_FieldModel"},
                    {"A.FieldType",OperType.BETWEEN,new RangDefinition(){ Low=10,High=99} },
                })
                .Group(new GroupBy { { "A.FieldName" } })
                .ToTable();
        ```
    3. 带统计函数查询

        ```c#
        DataTable dt_resultfun = sqlclient.Query("Hi_FieldModel", "A").Field("A.FieldName    as    Fname", "count(*) as avgFieldLen")
                .Join("Hi_TabModel").As("B").On(new JoinOn { { "A.TabName", "B.TabName" } })
                .Where(new Filter {
                    {"A.TabName", OperType.EQ, "Hi_FieldModel"},
                    {"A.FieldType",OperType.BETWEEN,new RangDefinition(){ Low=10,High=99} },

                })
                .Group(new GroupBy { { "A.FieldName" } })
                .ToTable();
        ```

        支持的函数

        - count  样例 count(*) as scount
        - avg 样例 avg("score") as avgscore  注：score必须是数值型否则HiSql将会出现错误提示
        - sum 样例 sum("score") as sumscore  注：score必须是数值型否则HiSql将会出现错误提示
        - min 样例 min("score")  as minscore 
        - max 样例 max("score") as maxscore

    4. 多表关联分页查询

        可以对组数据数据进行分页查询
        ```c#
        DataTable dt_resultfun = sqlclient.Query("Hi_FieldModel", "A").Field("A.FieldName    as    Fname", "count(*) as avgFieldLen")
                .Join("Hi_TabModel").As("B").On(new JoinOn { { "A.TabName", "B.TabName" } })
                .Where(new Filter {
                    {"A.TabName", OperType.EQ, "Hi_FieldModel"},
                    {"A.FieldType",OperType.BETWEEN,new RangDefinition(){ Low=10,High=99} },

                })
                .Group(new GroupBy { { "A.FieldName" } })
                .Skip(2).Take(10)
                .ToTable();
        ```
3. 复杂子表关联union all查询

    ```c#
    DataTable dt_resultuinon = sqlclient.Query(
                sqlclient.Query("Hi_FieldModel").Field("*").Where(new Filter { { "TabName", OperType.IN,
                        sqlclient.Query("Hi_TabModel").Field("TabName").Where(new Filter { {"TabName",OperType.IN,new List<string> { "Hone_Test", "H_TEST" } } })
                    } }),
                sqlclient.Query("Hi_FieldModel").Field("*").Where(new Filter { { "TabName", OperType.EQ, "DataDomain" } }),
                sqlclient.Query("Hi_FieldModel").Field("*").Where(new Filter { { "TabName", OperType.EQ, "Hi_FieldModel" } })
            )
                .Field("TabName", "count(*) as CHARG_COUNT")
                .Group(new GroupBy { { "TabName" } }).ToTable();

    ```

4. 复杂子表关联 分组排名


    WithRank方法 可以查询结果进行排序排名

    DbRank.DENSERANK  不跳号排名

    DbRank.RANK 跳号排名
    
    DbRank.ROWNUMBER 按记录行号排名
    ```c#
    DataTable dt_resultuinon = sqlclient.Query(
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
                .Group(new GroupBy { { "TabName" } }).ToTable();
    ```



5. 直接连接底层数据库进行查询

    in 查询
    ```c#
    
    DataTable dt= sqlClient.Context.DBO.GetDataTable("select * from dbo.Hi_FieldModel where TabName in (:TabName)", new HiParameter(":TabName",new List<string> { "Hi_TabModel", "Hi_FieldModel" }));
    ```

    单值变量
    ```c#
    DataTable dt = sqlClient.Context.DBO.GetDataTable("select * from dbo.Hi_FieldModel where TabName = @TabName", new HiParameter("@TabName", "Hi_TabModel"));

    ```

6. 表数据更新

    注：主键是不会被更新的，除UNAME字段外其它的都更新

    无实体更新

    ```c#
    int _effect = sqlClient.Update("H_TEST", new { DID = 1, UNAME = "UTYPE", UNAME2 = "user123" }).Exclude("UNAME").ExecCommand();
    
    ```
    以上代码生成的SQL如下,如果字段为主键会自动识别为更新条件，如果没有则会报错误
    ```sql
    update [dbo].[H_TEST] set [UNAME2]='user123' where [DID]=1
    ```
    ----

    无实体按指定条件对表数据进行更新

    ```c#
    int _effect1 = sqlClient.Update("H_TEST").Set(new { UNAME2 = "TEST" }).Where(new Filter { { "DID", OperType.GT, 8 } }).ExecCommand();

    ```
    以上代码生成的SQL如下
    ```sql
    update [dbo].[H_TEST] set [UNAME2]='TEST' where [H_TEST].[DID] > 8

    ```
    ---

    按实体进行更新 且只更新UNAME2 字段

    ```c#
    class H_Test
    { 
        public int DID {
            get;set;
        }
        public string UNAME {
            get;set;
        }
        public string UNAME2
        {
            get;set;
        }

    }
    ```

    ```c#
    int _effect2 = sqlClient.Update<H_Test>(new H_Test { DID = 1, UNAME2 = "Haha1" }).Only("UNAME2").ExecCommand();
    ```
    以上代码生成的SQL如下
    ```
    update [dbo].[H_Test] set [UNAME2]='Haha1' where [DID]=1

    ```
    ----


    批量更新
    ```c#
    int _effect3 = sqlClient.Update("H_TEST", new List<object> { new { DID = 1, UNAME2 = "user123" }, new { DID = 2, UNAME2 = "user124" } }).Only("UNAME2").ExecCommand();


    ```
    生成SQL如下
    ```sql
    update [dbo].[H_TEST] set [UNAME2]='user123' where [DID]=1
    update [dbo].[H_TEST] set [UNAME2]='user124' where [DID]=2
    ```

7. 删除数据库

    删除整表数据
    ```c#
    int _effect = sqlClient.Delete("H_Test").ExecCommand();
    ```
    生成的SQL如下
    ```sql
    delete [dbo].H_Test
    ```
    ----
    自义定条件删除表中的数据
    ```c#
    int _effect1 = sqlClient.Delete("H_Test").Where(new Filter { { "DID", OperType.GT, 200 } }).ExecCommand();
    ```
    生成的SQL如下
    ```sql
    delete [dbo].H_Test where [H_Test].[DID] > 200

    ```
    ----

    无实体批量按主键删除数据
    ```c#
    int _effect3 = sqlClient.Delete("H_Test", new List<object> { new { DID = 99, UNAME2 = "user123" }, new { DID = 100, UNAME2 = "user124" } }).ExecCommand();
    ```
    生成的SQL如下
    ```sql
    delete [dbo].H_Test where [DID]=99
    delete [dbo].H_Test where [DID]=100

    ```
    ----

    有实体批量按主键删除表数据
    ```c#
    class H_Test
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

    }
    ```
    ```c#
    int _effect4 = sqlClient.Delete("H_Test", new List<H_Test> { new H_Test { DID = 99, UNAME2 = "user123" }, new H_Test { DID = 100, UNAME2 = "user124" } }).ExecCommand();

    ```
    生成的SQL如下
    ```sql
    delete [dbo].H_Test where [DID]=99
    delete [dbo].H_Test where [DID]=100

    ```
    ----

    无数据库日志删除表数据
    ```c#
    int _effect5 = sqlClient.TrunCate("H_Test").ExecCommand();
    ```
    生成的SQL如下
    ```sql
    TRUNCATE TABLE [dbo].H_Test

    ```


