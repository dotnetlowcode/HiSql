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

注:也可以通过nuget安装

1. 引用HiSql.dll文件
2. 根据使用数据库的需要可以引用以下数据库实现的sdk
   1. HiSql.sqlserver.dll 
   2. HiSql.hana.dll
   3. HiSql.mysql.dll
   4. HiSql.oracle.dll
   5. HiSql.postgresql.dll




### 初始安装 
注：只需要执行一次即可
```c#
sqlclient.CodeFirst.InstallHisql();
```


 目前流行的ORM框架如果需要动态的拼接查询语句，只能用原生的sql进行拼接，无法跨不同数据库执行。hisql推出新的语法一套语句可以在不同的数据库执行

传统ORM框架最大的弊端就是完全要依赖于实体用lambda表达式写查询语句，但最大的问题就是如果业务场景需要动态拼接条件时只能又切换到原生数据库的sql语句进行完成，如果自行拼接开发人员还要解决防注入的问题,hisql 刚才完美的解决这些问题,Hisql底层已经对sql注入进行了

处理，开发人员只要关注于业务开发


### 2022.3.24
新增hisql语句参数化，防止注入风险
```c#
var _sql = sqlClient.HiSql("select * from Hi_FieldModel where TabName=[$name$] and IsRequire=[$IsRequire$]",
                new Dictionary<string, object> { { "[$name$]", "Hi_FieldModel ' or (1=1)" }, { "[$IsRequire$]",1 }  }
                ).ToSql();
```


### 2022.3.22 更新

增加 in(select *...) 子查询语法
注意：子查询中的字段只允许一个

```c#
var _sql = sqlClient.HiSql("select * from Hi_FieldModel where TabName in (select TabName from Hi_TabModel where TabName='h_test' )").ToSql();
```

### 2022.3.19 更新
HiSql新增对mysql表的操作（目前支持`SqlServer`,`mysql`，陆续会加上对其它数据库的实现）

操作写法请参照 2022.3.3 更新

### 2022.3.3 更新
HiSql新增对表的操作（暂时仅支持`SqlServer`，陆续会加上对其它数据库的实现）

可能有人会疑问作为ORM框架为什么会加这些功能？HiSql是致力于低代码平台的ORM框架,如果有了解过低代码平台的原理你就会明白这些功能有多有用！

HiSql提供以下对表及视图的操作
1.  表的重命名
```c#
//OpLevel.Execute  表示执行并返回生成的SQL
//OpLevel.Check 表示仅做检测失败时返回消息且检测成功时返因生成的SQL
var rtn = sqlClient.DbFirst.ReTable("H_Test5_1", "H_Test5",OpLevel.Execute);
if (rtn.Item1)
{
    Console.WriteLine(rtn.Item2);//输出成功消息
    Console.WriteLine(rtn.Item3);//输出重命名表 生成的SQL
}
else
    Console.WriteLine(rtn.Item2);//输出重命名失败原因
```
2.  向表中新增字段
```c#
//OpLevel.Execute  表示执行并返回生成的SQL
//OpLevel.Check 表示仅做检测失败时返回消息且检测成功时返因生成的SQL
HiColumn column = new HiColumn()
{
    TabName = "H_Test5",
    FieldName = "TestAdd",
    FieldType = HiType.VARCHAR,
    FieldLen = 50,
    DBDefault = HiTypeDBDefault.EMPTY,
    DefaultValue = "",
    FieldDesc = "测试字段添加"

};

var rtn= sqlClient.DbFirst.AddColumn("H_Test5", column, OpLevel.Execute);

if (rtn.Item1)
{
    Console.WriteLine(rtn.Item2);//输出成功消息
    Console.WriteLine(rtn.Item3);//输出 生成的SQL
}
else
    Console.WriteLine(rtn.Item2);//输出失败原因
```
3.  对表中已有的字段进行变更
```c#
//OpLevel.Execute  表示执行并返回生成的SQL
//OpLevel.Check 表示仅做检测失败时返回消息且检测成功时返因生成的SQL
HiColumn column = new HiColumn()
{
    TabName = "H_Test5",
    FieldName = "TestAdd",
    FieldType = HiType.VARCHAR,
    FieldLen = 51,
    DBDefault = HiTypeDBDefault.VALUE,
    DefaultValue = "TGM",
    FieldDesc = "测试字段变更"

};

var rtn = sqlClient.DbFirst.ModiColumn("H_Test5", column, OpLevel.Execute);
if (rtn.Item1)
{
    Console.WriteLine(rtn.Item2);//输出成功消息
    Console.WriteLine(rtn.Item3);//输出 生成的SQL
}
else
    Console.WriteLine(rtn.Item2);//输出失败原因
```

4.  对表中已经字段进行重命名
```c#
//OpLevel.Execute  表示执行并返回生成的SQL
//OpLevel.Check 表示仅做检测失败时返回消息且检测成功时返因生成的SQL
HiColumn column = new HiColumn()
{
    TabName = "H_Test5",
    FieldName = "Testname3",
    ReFieldName = "Testname2",
    FieldType = HiType.VARCHAR,
    FieldLen = 50,
    DBDefault = HiTypeDBDefault.VALUE,
    DefaultValue = "TGM",
    FieldDesc = "测试字段重命名"

};

var rtn = sqlClient.DbFirst.ReColumn("H_Test5", column, OpLevel.Execute);
if (rtn.Item1)
{
    Console.WriteLine(rtn.Item2);//输出成功消息
    Console.WriteLine(rtn.Item3);//输出 生成的SQL
}
else
    Console.WriteLine(rtn.Item2);//输出失败原因
```
5.  对表中字段进行修改
```c#
//OpLevel.Execute  表示执行并返回生成的SQL
//OpLevel.Check 表示仅做检测失败时返回消息且检测成功时返因生成的SQL
HiColumn column = new HiColumn()
{
    TabName = "H_Test5",
    FieldName = "TestAdd",
    FieldType = HiType.VARCHAR,
    FieldLen = 51,
    DBDefault = HiTypeDBDefault.VALUE,
    DefaultValue = "TGM",
    FieldDesc = "测试字段变更"

};

var rtn = sqlClient.DbFirst.ModiColumn("H_Test5", column, OpLevel.Execute);
if (rtn.Item1)
{
    Console.WriteLine(rtn.Item2);//输出成功消息
    Console.WriteLine(rtn.Item3);//输出 生成的SQL
}
else
    Console.WriteLine(rtn.Item2);//输出失败原因
```
6.  

7.  自定义表结构TabInfo 自动同步变更物理表结构信息
```c#
//OpLevel.Execute  表示执行并返回生成的SQL
//OpLevel.Check 表示仅做检测失败时返回消息且检测成功时返因生成的SQL
var tabinfo = sqlClient.Context.DMInitalize.GetTabStruct("H_Test5");

TabInfo _tabcopy = ClassExtensions.DeepCopy<TabInfo>(tabinfo);
_tabcopy.Columns[2].ReFieldName = "Testname3";
var rtn= sqlClient.DbFirst.ModiTable(_tabcopy, OpLevel.Execute);
if (rtn.Item1)
{
    Console.WriteLine(rtn.Item2);//输出成功消息
    Console.WriteLine(rtn.Item3);//输出 生成的SQL
}
else
    Console.WriteLine(rtn.Item2);//输出失败原因
```
8.  获取数据库所表表和视图清单
```c#
//获取当前数据库中的所有物理表和视图
List<TableInfo> lsttales = sqlClient.DbFirst.GetAllTables();
foreach (TableInfo tableInfo in lsttales)
{
    Console.WriteLine($"{tableInfo.TabName}  {tableInfo.TabReName}  {tableInfo.TabDescript}  {tableInfo.TableType} 表结构:{tableInfo.HasTabStruct}");
}
```
9.  获取数据库所有全局临时表
```c#
//获取当前数据库中的所有的全局临时表
List<TableInfo> lsttales = sqlClient.DbFirst.GetGlobalTempTables();
foreach (TableInfo tableInfo in lsttales)
{
    Console.WriteLine($"{tableInfo.TabName}  {tableInfo.TabReName}  {tableInfo.TabDescript}  {tableInfo.TableType} 表结构:{tableInfo.HasTabStruct}");
}
```
10.  根据HiSql语句创建视图
```c#
//OpLevel.Execute  表示执行并返回生成的SQL
//OpLevel.Check 表示仅做检测失败时返回消息且检测成功时返因生成的SQL
var rtn = sqlClient.DbFirst.CreateView("vw_FModel", 
    sqlClient.HiSql("select a.TabName,b.TabReName,b.TabDescript,a.FieldName,a.SortNum,a.FieldType from Hi_FieldModel as a inner join Hi_TabModel as b on a.TabName=b.TabName").ToSql(), 
    OpLevel.Execute);

if (rtn.Item1)
{
    Console.WriteLine(rtn.Item2);//输出成功消息
    Console.WriteLine(rtn.Item3);//输出 生成的SQL
}
else
    Console.WriteLine(rtn.Item2);//输出失败原因
```
11. 删除指定视图
```c#
//OpLevel.Execute  表示执行并返回生成的SQL
//OpLevel.Check 表示仅做检测失败时返回消息且检测成功时返因生成的SQL
var rtn = sqlClient.DbFirst.DropView("vw_FModel",

    OpLevel.Execute);

if (rtn.Item1)
{
    Console.WriteLine(rtn.Item2);//输出成功消息
    Console.WriteLine(rtn.Item3);//输出 生成的SQL
}
else
    Console.WriteLine(rtn.Item2);//输出失败原因
```
12. 修改指定视图
```c#
//OpLevel.Execute  表示执行并返回生成的SQL
//OpLevel.Check 表示仅做检测失败时返回消息且检测成功时返因生成的SQL
var rtn = sqlClient.DbFirst.ModiView("vw_FModel",
    sqlClient.HiSql("select a.TabName,b.TabReName,b.TabDescript,a.FieldName,a.SortNum,a.FieldType from Hi_FieldModel as a inner join Hi_TabModel as b on a.TabName=b.TabName where b.TabType in (0,1)").ToSql(),
    OpLevel.Execute);

if (rtn.Item1)
{
    Console.WriteLine(rtn.Item2);//输出成功消息
    Console.WriteLine(rtn.Item3);//输出 生成的SQL
}
else
    Console.WriteLine(rtn.Item2);//输出失败原因
```
13. 列出指定表的索引清单
```c#
List<TabIndex> lstindex = sqlClient.DbFirst.GetTabIndexs("Hi_FieldModel");
foreach (TabIndex tabIndex in lstindex)
{
    Console.WriteLine($"TabName:{tabIndex.TabName} IndexName:{tabIndex.IndexName} IndexType:{tabIndex.IndexType}");
}

```
14. 对指定表创建指定字段索引
```c#
TabInfo tabInfo = sqlClient.Context.DMInitalize.GetTabStruct("H04_OrderInfo");
List<HiColumn> hiColumns = tabInfo.Columns.Where(c => c.FieldName == "POSOrderID").ToList();
var rtn = sqlClient.DbFirst.CreateIndex("H04_OrderInfo", "H04_OrderInfo_POSOrderID", hiColumns, OpLevel.Execute);
if (rtn.Item1)
    Console.WriteLine(rtn.Item3);
else
    Console.WriteLine(rtn.Item2);

```
15. 查看指定索引的字段明细
```c#
List<TabIndexDetail> lstindexdetails = sqlClient.DbFirst.GetTabIndexDetail("Hi_FieldModel","PK_Hi_FieldModel_ed721f6b-296a-447e-ac67-7d02fd8e338c");
foreach (TabIndexDetail tabIndexDetail in lstindexdetails)
{
    Console.WriteLine($"TabName:{tabIndexDetail.TabName} IndexName:{tabIndexDetail.IndexName} IndexType:{tabIndexDetail.IndexType} ColumnName:{tabIndexDetail.ColumnName}");
    
}
```
16. 删除指定索引
```c#
rtn = sqlClient.DbFirst.DelIndex("H04_OrderInfo", "H04_OrderInfo_POSOrderID",OpLevel.Execute);

if (rtn.Item1)
    Console.WriteLine(rtn.Item3);
else
    Console.WriteLine(rtn.Item2);
```


### 2022.2.7 更新
新增bulkcopy功能
hisql已经支持BulkCopy的数据有 SqlServer,Oacle,MySql,PostGreSql,Hana

如果表数据插入超过1W 可以用此功能，如果小于这个数建议使用常规方式插入




1. 通过DataTable向表批量写入
```c#
int _effect= await  sqlClient.BulkCopyExecCommandAsyc("HTest01", dt);

```

2. 通过字典批量写入
```c#
List<Dictionary<string, object>> lstdata = new List<Dictionary<string, object>>();
int _count = 1000000;
Random random = new Random();
for (int i = 0; i < _count; i++)
{
    lstdata.Add(new Dictionary<string, object> { { "SID", (i + 1) }, { "UName", $"tansar{i}" }, { "Age", 20 + (i % 50) }, { "Salary", 5000 + (i % 2000) + random.Next(10) }, { "descript", "hello world" } });
}

int _effect = sqlClient.BulkCopyExecCommand("HTest01", lstdata); 

```





### 2022.1.6 更新
1. CodeFirst增加表删除功能
```c#
sqlClient.CodeFirst.DropTable("H_Test4");
``` 
2. CodeFirst 增加创建表功能
```c#


//tabInfo 为 TabInfo 类型（可以自行构建该类实现动态创建表）
sqlClient.CodeFirst.CreateTable(tabInfo);

//也可以根据实体类直接创建
sqlClient.CodeFirst.CreateTable(typeof(H_Test4));
```

### 2022.1.5 更新
1. 当物理表结构有变更时 自动将物理表结构信息同步到`Hi_FieldModel` 字段中(需要重启应用)



### 2021.12.25 更新
1. 新增Update的where条件支持Hisql语句 hisql写法请参考2021.12.10-15更新
```c#
//Where方法中可以写HiSql语法的条件 
sqlClient.Update("H_Test").Set(new { UNAME = "UTYPE" }).Where("DID=1").ExecCommand();

```



### 2021.12.24 更新
1. 新增配置通过正则表达式校验字段值功能
2. 新增配置某字段值的必须是在某一关联表中存在的校验
3. bug 更新
#### 通过正则表达式匹配字段值是否符合要求

平常在系统开发的过程中如用户名 对用户名的规则是有命名规则要求的,如果前端没做判断的话是直接可以写入到数据库中`HiSql` 提供了这种可以校验配置

举例在表`HTest01` 中的字段`UName` (用户名)  如果该用户名只允许数字和字母那么我们可以做一下演示
```c#

//在系统表Hi_FieldModel 中对表HTest01 的字段加上正则配置 Regex

sqlClient.Update("Hi_FieldModel", new { TabName = "HTest01", FieldName = "UName", Regex = @"^[\w]+[^']$" }).ExecCommand();


//执行插入数据
sqlClient.Insert("HTest01", new { SID = "0", UTYP = "U4", UName = "test hisql ", Age = 36, Salary = 11, Descript = "hisql" }).ExecCommand();



```
以上代码会抛出异常 因为UName赋的值 `test hisql ` 有空格不符合正则表达式`^[\w]+[^']$`
错误显示如：`列[UName]值[dd hisql] 不符合业务配置 ^[\w]+[^']$ 要求`

这样就可以把逻辑性的错误数据拦截在系统之外


#### 通过关联表校验
举例在表`HTest01` 中的字段`UTYP` (用户类型)  这个类型值在表`H_UType` 中 我们做一下演示

```c#
//在表中添加用户类型 Modi方法的意思是 如果存在则更新没有则插入
            sqlClient.Modi("H_UType", new List<object> {
                new { UTYP = "U1", UTypeName = "普通用户" },
                new { UTYP = "U2", UTypeName = "中级用户" },
                new { UTYP = "U3", UTypeName = "高级用户" }
            }).ExecCommand();

// 增加表校验配置
sqlClient.Update("Hi_FieldModel", new { TabName = "HTest01", FieldName = "UTYP", IsRefTab=true,RefTab= "H_UType",RefField="UTYP", RefFields = "UTYP,UTypeName",RefFieldDesc= "类型编码,类型名称",RefWhere="UTYP<>''" }).ExecCommand();

//执行数据插入
sqlClient.Insert("HTest01", new { SID = "0", UTYP = "U4", UName = "hisql", Age = 36, Salary = 11, Descript = "hisql" }).ExecCommand();

```
以上代码会抛出异常 因为 `UTYP` 指定的值`U4` 不存在于表 `H_UType`  中
错误显示如：`字段[UTYP]配置了表检测 值 [U4] 在表[H_UType]不存在`






### 2021.12.15 更新

通过这个更新hisql 全面支持select 常用语法

 `HiSql`不仅仅是一个ORM框架而且是一个`HiSql` SQL语句 如下更新所示
#### hisql 语句实现 group by having
```c#
string sql = sqlClient.HiSql($"select FieldName,count(*) as scount  from Hi_FieldModel group by FieldName,  Having count(*) > 0   order by fieldname").ToSql();

int _total = 0;

DataTable dt = sqlClient.HiSql($"select FieldName,count(*) as scount  from Hi_FieldModel group by FieldName,  Having count(*) > 0   order by fieldname")
    .Take(2).Skip(2).ToTable(ref _total);
```

### 2021.12.10 更新

```c#
    string sql = sqlClient.HiSql($"select * from Hi_FieldModel  where (tabname = 'h_test') and  FieldType in (11,21,31) and tabname in (select tabname from Hi_TabModel)").ToSql();
```

以上语法是不是与sqlserver 和sql语句有点类似？是的 但他可不是原生的sql,现在统一命名为`hisql` ，这个语法可以在hisql 支持的任意库中运行

`hisql` 支持 常规sql基本`join`操作
`inner join` 或`join`
`left inner join`或 `left join`
`outer join`
也支持 子查询 `in (select ....)`

#### hisql inner join 示例

```c#
    string sql = sqlClient.HiSql($"select b.tabname, a.fieldname,a.IsPrimary from  Hi_FieldModel as a  inner join   Hi_TabModel as  b on a.tabname = b.tabname" ).ToSql();

```
以上示例的hisql代码编译后成的sqlserver的原生sql如下
```sql
select [b].[tabname],[a].[fieldname],[a].[IsPrimary] from [Hi_FieldModel] as [a]
 inner join [Hi_TabModel] as [b] on [a].[tabname]=[a].[tabname]
```

在生成的mysql 原生sql如下
```sql
select `b`.`tabname`,`a`.`fieldname`,`a`.`IsPrimary` from `Hi_FieldModel` as `a`
 inner join `Hi_TabModel` as `b` on `a`.`tabname`=`a`.`tabname`
```


也可以多表Join
```c#
string sql = sqlClient.HiSql($"select b.tabname, a.fieldname,a.IsPrimary from  Hi_FieldModel as a  inner join   Hi_TabModel as  b on a.tabname = b.tabname" +
                $" inner join Hi_TabModel as c on a.tabname = c.tabname ").ToSql();

```

注意如果出现语法错误将会抛出异常并提示`HiSql语法检测错误:xxxxx` 根据这个错误提示可自行定位问题

#### hisql 语句实现分页
示例代码如下
```c#
int total = 0;
    var table = sqlClient.HiSql($"select fieldlen,isprimary from  Hi_FieldModel     order by fieldlen ")
        .Take(3).Skip(2)
        .ToTable(ref total);
```
注意：分页查询时一定有一个order by 语句，并可以返回当前条件的数据总记录数`total`

#### hisql 语句实现 in查询 及select in 查询

```c#
    string sql = sqlClient.HiSql($"select * from Hi_FieldModel  where (tabname = 'Hi_FieldModel') and  FieldType in (11,21,31) and tabname in (select tabname from Hi_TabModel) order by tabname asc")
        .Take(2).Skip(2)
        .ToSql();
```

#### hisql 语句实现 group by 查询
```c#
string sql = sqlClient.HiSql($"select FieldName,FieldType from Hi_FieldModel  group by FieldName,FieldType ")
             .Take(2).Skip(2)
             .ToSql();

```

#### hisql 语句实现 group by having
```c#
string sql = sqlClient.HiSql($"select FieldName,count(*) as scount  from Hi_FieldModel group by FieldName,  Having count(*) > 0   order by fieldname").ToSql();

int _total = 0;

DataTable dt = sqlClient.HiSql($"select FieldName,count(*) as scount  from Hi_FieldModel group by FieldName,  Having count(*) > 0   order by fieldname")
    .Take(2).Skip(2).ToTable(ref _total);
```






### 2021.12.2 更新
1. 修复当以Dictionary 对象作为数据插入来源，且该字典的字段无与表中匹配的问题
2. 新增 查询条件语法如下 可以实现无限级分组条件

2.1 结构化写法如下所示
```c#
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
```
2.2 字符串写法如下所示
支持 字符串写法解决通过应用前端可以自定义传条件过来 `a.tabname = 'Hi_FieldModel' and ((a.FieldType = 11)) `
看这个语法类似于sqlserver的写法，与sqlserver无关， 这是Hisql自定义的中间语法 通过hisql编译后在hisql支持的库中运行

注：暂时不支持in,not in写法 下一版本将会加上


```c#
string jsondata = sqlClient.Query("Hi_FieldModel", "A").Field("A.FieldName as Fname")
    .Join("Hi_TabModel").As("B").On(new JoinOn { { "A.TabName", "B.TabName" } })
    .Where("a.tabname = 'Hi_FieldModel' and ((a.FieldType = 11)) ")
    .Group(new GroupBy { { "A.FieldName" } }).ToJson();
```

```


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
注：只需要执行一次即可
```c#
sqlclient.CodeFirst.InstallHisql();

```


### 表数据插入
1. 单表单条数据插入
    Insert语法
    参1："Hi_DataElement" 

        可以是一个物理表也可以是临时表
    临时表的写法如 "#Hi_DataElement" 用1个#号表示 本地临时表 两个#号表示是全局临时表
    变更表写法如"@Hi_DataElement" 注：其它库的语也是这个语法

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
    查看生成的sql语句 通过以下方式也可以知道hisql底层对于当前类型的数据库生成的sql语句

    ```c#
    string _sql =sqlclient.Insert("Hi_DataElement", new { domain = "UTYPE" }).ToSql();
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

3. 如果在表里存在就更新没有则更新 
   其实我们经常在业务开发中有这种业务场景，如果有该记录存在则更新没有则插入数据，一般是用存储过程或单独写sql语句，这两种方式都比较麻烦,而hisql提供了更加方便的写法如下所示
   注：如果当前表的主键是自增长的id 则无法使用该功能 HiSql检漏时会报异常
   
    ```c#
    //当该记录存在时就会更新，不存在则会插入 支持批量操作
     sqlClient.Modi("H_Test", new { Hid = 1, UserName = "tansar", UserAge = 100, ReName = "Tom" }).ExecCommand();



    ```


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




# HiSql 实现case语法操作

在SqlServer,Oralce,Hana,PostGreSql,MySql 这些数据都支持SQL case语法，平常在实现业务开发中也会常用到，那么HiSql对于case语法也提供了比较便捷的写法，HiSql将会自动适配不同的数据库，开发人员不用管具体哪一种的SQL语法。

### 这是HiSql样例写法代码
HiSql提供的语法只要开发人员本身对SQL有一定的基础，基本上上手都比较容易
```c#
    string _sql=sqlClient.Query("Hi_TabModel").Field("TabName as tabname").
        Case("TabStatus")
            .When("TabStatus>1").Then("'启用'")
            .When("0").Then("'未激活'")
            .Else("'未启用'")
        .EndAs("Tabs", typeof(string))
        .Field("IsSys")
        .ToSql()
        ;
```
#### When方法中的字条件语法 (HiSql支持的库都是以下同样的写法)
1. When("TabStatus>1")  支持的操作符 >,<,>=,<=,!=,<> 如果操作符不在此列HiSql将会检测语法错误
2. 当是字段是字符串时 值加下加单引号如 When("TabName='TabName'")
3. 当然也可以这样写 .When("0")   这里的意思与 When("TabStatus=0")

#### 注意事项
1. 当出现语法错误时HiSql会自动检测并报出错误异常
2. Case语法不支持嵌套Case语法（日常使用这样会有性能问题）




### HiSql生成的原生SqlServer 代码
```sql
select [Hi_TabModel].[TabName] as [tabname],case
        when [TabStatus] > 1 then '启用'
        when [TabStatus] = 0 then '未激活'
        else '未启用'
    end as [Tabs]
    ,[Hi_TabModel].[IsSys] from [Hi_TabModel] as [Hi_TabModel]
```
---
### HiSql生成的原生MySql 代码
```sql
select `Hi_TabModel`.`TabName` as `tabname`,case
    when `TabStatus` > 1 then '启用'
    when `TabStatus` = 0 then '未激活'
    else '未启用'
    end as `Tabs`
    ,`Hi_TabModel`.`IsSys` from `Hi_TabModel` as `Hi_TabModel`
```
---
### HiSql生成的原生HANA 代码
```sql
SELECT "HI_TABMODEL"."TABNAME" AS "TABNAME",CASE
   WHEN "TABSTATUS" > 1 THEN '启用'
   WHEN "TABSTATUS" = 0 THEN '未激活'
   ELSE '未启用'
END AS "TABS"
,"HI_TABMODEL"."ISSYS" FROM  "HONEBI"."HI_TABMODEL" AS "HI_TABMODEL" 

```

### HiSql生成的原生ORACLE 代码
```sql
SELECT HI_TABMODEL."TABNAME" AS "TABNAME",CASE
   WHEN "TABSTATUS" > 1 THEN '启用'
   WHEN "TABSTATUS" = 0 THEN '未激活'
   ELSE '未启用'
END AS "TABS"
,HI_TABMODEL."ISSYS" FROM HI_TABMODEL   HI_TABMODEL
```

### HiSql生成的原生PostGreSql 代码
```sql
select "Hi_TabModel"."TabName" as "tabname",case
   when "TabStatus" > 1 then '启用'
   when "TabStatus" = 0 then '未激活'
   else '未启用'
end as "Tabs"
,"Hi_TabModel"."IsSys" from "Hi_TabModel" as "Hi_TabModel"

```
