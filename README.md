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
   6. HiSql.dameng.dll `新支持1.0.4及以上支持`
   7. HiSql.Sqlite.dll `新支持1.0.4.7以上支持`


### hisql官方群
<img src="http://hisql.net/images/group/qq.png" alt="hisql官方QQ群" >

为了更好的服务于真正使用hisql的用户， 进群的伙伴必须是在github或gitee 上star了hisql项目或进行过捐的伙伴的方能进群


### 初始安装 
注：只需要执行一次即可
```c#
   sqlclient.CodeFirst.InstallHisql();
```


 目前流行的ORM框架如果需要动态的拼接查询语句，只能用原生的sql进行拼接，无法跨不同数据库执行。hisql推出新的语法一套语句可以在不同的数据库执行

传统ORM框架最大的弊端就是完全要依赖于实体用lambda表达式写查询语句，但最大的问题就是如果业务场景需要动态拼接条件时只能又切换到原生数据库的sql语句进行完成，如果自行拼接开发人员还要解决防注入的问题,hisql 刚才完美的解决这些问题,Hisql底层已经对sql注入进行了处理，开发人员只要关注于业务开发


### 2023.6.12 hisql语法优化

以下语句的的子语句的条件可以引用上层语句的字段
```c#
var _sql = sqlClient.HiSql("select a.tabname from Hi_TabModel as a where a.tabname in (select b.tabname from Hi_FieldModel as b where b.tabname = `a.tabname`)").ToSql();
```

### 2023.5.30 修复update 的bug
1. 1.0.5.9 发布
2. 修复在特定场景定update语句生成的Bug

### 2023.4.18 参数化优化
可以看以下demo

```c#
var _paramsql = sqlClient.HiSql(@"select * from Hi_FieldModel where tabname in (@TabName)  and fieldname=@fieldname and tabname in (select tabname from hi_tabmodel where tabname in (@TabName) )", new { TabName = new List<string> { "Hi_TestQuery", "Hi_FieldModel" }, FieldName = "DbServer" }).ToSql();
            Console.WriteLine(_paramsql);
```



### 2023.04.11 更新
1. 修复第一次获取表信息在多线程下会报表错的总是
2. sqlserver的表结构信息字段为varchar|nvarchar|(max) 转到HANA及其它库抛出异常的问题
3. 针对于HANA库中表中的字段类型包括[TEXT]类型使用[sqlClient.Modi] 方法抛出错误提示（HANA中的临时表不支持text类型） 

事务优化
如果业务中需要使用多表且多动作更新请参数以下定法
为了防止死锁请参照 `业务锁` 的处理方式

```c#
void transDemo(HiSqlClient sqlClient)
{
    int count = 10;
    string tabname = typeof(H_Test02).Name;
    List<object> lstdata = buildData10Col(count);

    sqlClient.Delete(tabname).ExecCommand();

    using (var sqlClt = sqlClient.CreateUnitOfWork())
    { 
        sqlClt.Insert(tabname, lstdata).ExecCommand();

        sqlClt.Modi(tabname, lstdata).ExecCommand();
        sqlClt.CommitTran();
        //sqlClt.RollBackTran();
    }


}


```



### 2023.03.25 更新
1.hisql框架全面支持NET7.0

2.修改bool类型默认为false

3.对HiSql.Excel 更新可导出excel并带图片

图片是一个Url地址可下载下来并插入到excel中以下是demo代码



```c#
/// <summary>
/// 数据保存到Excel
/// </summary>
/// <param name="tableInfo"></param>
/// <param name="dt"></param>
/// <param name="savePath"></param>
/// <returns></returns>
static async Task SavePageExcel(TabInfo tableInfo, DataTable dt, string savePath, bool isNullTemplate, List<ColumnSetting> columnSettings = null)
{
    Extension.Excel excel = new Extension.Excel(new Extension.ExcelOptions()
    {
        TempType = Extension.TempType.HEADER
    });
    excel.Add(new Extension.ExcelHeader(1).Add("表名").Add(tableInfo.TabModel.TabName));//标识表名
                                                                                        //中文头
    Extension.ExcelHeader cnHeader = new Extension.ExcelHeader(2);
    //英文头
    Extension.ExcelHeader enHeader = new Extension.ExcelHeader(3);

    var excludeFields = new List<string>() {//模板忽略字段
        "CreateTime",
        "CreateName",
        "ModiTime",
        "ModiName"
    };
    foreach (DataColumn dataColumn in dt.Columns)
    {
        if (isNullTemplate && excludeFields.Contains(dataColumn.ColumnName))
        {
            continue;
        }
        HiColumn hiColumn = tableInfo.Columns.Where(c => c.FieldName.Equals(dataColumn.ColumnName)).FirstOrDefault();
        if (hiColumn != null)
        {
            //自增主键不能填也不能改
            var tipStr = hiColumn.IsIdentity ? "[不可修改]" : "";
            var cnName = (string.IsNullOrEmpty(hiColumn.FieldDesc) ? dataColumn.ColumnName : hiColumn.FieldDesc) + tipStr;
            cnHeader.Add(cnName);
        }
        else
        {
            cnHeader.Add(dataColumn.ColumnName);
        }
        enHeader.Add(dataColumn.ColumnName);
    }
    excel.Add(cnHeader);//字段中文描述
    excel.Add(enHeader);//字段名
    if (isNullTemplate)
    {
        dt.Clear();
    }

    var columnSettingMap = columnSettings?.ToDictionary(r => r.FieldName, r => r);
    //生成excel  columnSettingMap == null ? null :

    excel.WriteExcel(dt, savePath, cellRenderFun: (sheet, row, cell) =>
    {
        if (columnSettingMap == null)
        {
            return;
        }
        var columnIndex = cell.ColumnIndex;
        var headName = dt.Columns[columnIndex].ColumnName;
        var cSetting = columnSettingMap[headName];
        row.Height = cSetting.RowHeight;
        //如果是图片
        if (cSetting.RenderType == "1")
        {
            var imgPath = cell.StringCellValue;
            if (imgPath.IndexOf("//") == 0)
            {
                imgPath = "https:" + imgPath;
            }
            var imgData = WebHelper.GetImageDataByUrl(imgPath);
            var imgId = sheet.Workbook.AddPicture(imgData, PictureType.JPEG);
            var patriarch = sheet.CreateDrawingPatriarch();
            int rowIndex = cell.RowIndex;
            var h = row.Height;
            IClientAnchor anchor = patriarch.CreateAnchor(0, 0, 0, h, columnIndex, rowIndex, columnIndex + 1, rowIndex + 1);
            patriarch.CreatePicture(anchor, imgId);
        }
    });
}

```



### 2022.11.12 更新

1. 增加将查询结果直接插入到表中如下

```c#
//目标表可以是一个不存在的表 也可以是一个存在的表
sqlClient.HiSql("select * from Hi_FieldModel").Insert("tmp_hi_2022");
```

2. 也支持插入临时表 (除HANA,ORACLE,达梦)之外都支持
```c#
sqlClient.HiSql("select * from Hi_FieldModel").Insert("#tmp_hi_2022");
```


### 2022.9.13 更新
1. `HiSql`语句新增`is null` 和 `is not null` 查询语法
```c#
 sqlClient.HiSql("select * from H_tst10 where birth is  null").ToJson();
```


2. 数据插入时可以允许集合中的字段数不一样 如下更新所示


```c#
    sqlClient.Insert(tabname, new List<Dictionary<string, object>> {
            new Dictionary<string, object>
            {
                { "SID",1},
                { "uname","tansar"}
            },
            new Dictionary<string, object>
            {
                { "SID",2},
                { "uname","tansar"},
                { "birth",DateTime.Now}
            },
            new Dictionary<string, object>
            {
                { "SID",3},
                { "gname","tgm"},
                { "birth",DateTime.Now}
            },
            new Dictionary<string, object>
            {
                { "SID",5},
                    { "uname","tansar"},
                { "gname","tgm"}

            }
        }).ExecCommand();

```


### 2022.8.10 更新
1. 新增支持sqlite 目前已经支持sqlserver,oracle,hana,,mysql,postgresql,达梦,sqlite
2. 新增表结构升级功能（文档后续更新）
3. 之前用过Hisql的项目更新了使用了新版本后需要重新执行一下  `sqlclient.CodeFirst.InstallHisql()`

### 2022.7.7 新增自动产生SNRO编号

操作步骤
1. 启用Snro编号

```c#
HiSql.Global.SnroOn = true;

//只要执行一次 启用Snro编号 将会安装创建Hi_Snro表
sqlClient.CodeFirst.InstallHisql();


HiSql.SnroNumber.SqlClient = sqlClient;

```


2. 配置编号
   
```c#
List<object> list = new List<object>();
///生成销售订单编码 每分钟从0开始编号 如20220602145800001-20220602145899999
list.Add(new { SNRO = "SALENO", SNUM = 1, IsSnow = false, SnowTick = 0, StartNum = "10000", EndNum = "99999", Length = 5, CurrNum = "10000", CurrAllNum = "", PreChar = "", IsNumber = true, PreType = PreType.YMDHm, FixPreChar = "", IsHasPre = true, CacheSpace = 10, Descript = "销售订单号流水" });

sqlClient.Modi("Hi_Snro", list).ExecCommand();

```

3. 创建表`h_test`


![](http://hisql.net/images/demo/h_test5.png)

1. 绑定配置
   


```c#

//只要使用 H_Test5 表就会将该表配置写入到 hi_fieldmodel 中
var json=sqlClient.HiSql("select * from H_Test5").Take(1).Skip(1).ToJson();


//增另对sid字段的编号配置 绑定创建的编号规则
sqlClient.Update("hi_fieldmodel", new { TabName = "H_Test5", FieldName = "sid", SNO = "SALENO", SNO_NUM = "1" }).Only("SNO", "SNO_NUM").ExecCommand();
```

5. 插入数据
   
```c# 
List<object> list = new List<object>();
for (int i = 0; i < 10000; i++)
{

    //不需要为sid赋值 hisql底层会自动根据snro配置进行编号
    list.Add(new { uname =$"uname{i}",age=20*i,descript=$"test{i}"});

}

sqlClient.Insert("H_Test5", list).ExecCommand();
```

6. 查看结果
   
   
![](http://hisql.net/images/demo/h_test5_result.png)


### 2022.6.21 新增hisql参数化查询


注意：不管底层用的哪一种库hisql的参数化写法是一致的

```c#
    string sql1= sqlClient.HiSql("select * from hi_tabmodel where tabname=@tabname ", new { TabName="H_test" ,FieldName="DID"}).ToSql();
    string sql2= sqlClient.HiSql("select * from hi_tabmodel where tabname=@tabname or TabType in( @TabType)", new { TabName="H_test" , TabType =new List<int> { 1,2,3,4} }).ToSql();

    string sql3 = sqlClient.HiSql("select * from hi_tabmodel where tabname=@tabname ", new Dictionary<string, object> { { "TabName", "H_test" } }).ToSql();

```
原先参数写法也同样支持(但不建议使用,以后可能会删除以下语法)


```c#
    string sql4 = sqlClient.HiSql("select * from hi_tabmodel where tabname=[$tabname$] ", new Dictionary<string, object> { { "[$tabname$]", "H_test" } }).ToSql();
```


### 2022.6.21 新增工作单元模式

`CreateUnitOfWork` 默认是开始事务,在工作单元执行完成默认会执行`Commit` 事务提交，如果需要回滚请使用`RollBackTran` 进行事务回滚

```c#
    using (var client = sqlClient.CreateUnitOfWork())
    {
        client.Insert("H_UType", new { UTYP = "U4", UTypeName = "高级用户" }).ExecCommand();

        //client.RollBackTran();
        

    }
```


### 2022.6.15 hisql新增国产达梦数据库支持

1. 新增国产达梦数据库支持 目前`hisql`已经支持 `sqlserver`,`hana`,`mysql`,`oracle`,`posgresql`,`dameng`  六种库 涉及国际，国产 及行式存储和列式存储的数据库 下一步将会支持`sqlite`

2. 优化hisql语法编译解决hisql语法大小写问题，一条hisql语句跨库运行

3. 新增ToColumns()方法 返回hisql语句结果字段结构信息

该功能可以用于将某查询结果集保存到一张新表中（根据这些字段结构信息可创建一张新表）

```c#
    //返回该hisql的结果字段结构信息
    List<HiColumn> cols = sqlClient.HiSql("select max(FieldType) as fieldtype from Hi_FieldModel").ToColumns();
```

### 2022.6.11 hisql 新增查询模版语法
如下所示

用`` 符号括起来就示字段
如果是数值型的类型支持表达式

```c#
string sql = sqlClient.HiSql("select * from Hi_FieldModel as a where a.TabName=`a.TabName` and a.FieldName='11'").ToSql();
```
表达式模板语法
```c#
string sql = sqlClient.HiSql("select * from Hi_FieldModel as a where a.SortNum=`a.SortNum`+1 and a.FieldName='11'").ToSql();
```


以下是链式查询的语法样例
```c#
string sql = sqlClient.Query("Hi_FieldModel", "A").Field("*")
    .Where(new Filter {
        {"A.TabName", OperType.EQ, "`A.FieldName`"}
                     })
    .Group(new GroupBy { { "A.FieldName" } }).ToSql();
```
表达式模板语法
```c#
string sql = sqlClient.Query("Hi_FieldModel", "A").Field("*")
    .Where(new Filter {
        {"A.SortNum", OperType.EQ, "`A.SortNum`+1"}
                     })
    .Group(new GroupBy { { "A.FieldName" } }).ToSql();
```

### 2022.6.10 hisql 新增on 的语法增强


原语法 join on 只允许 字段=字段，现在可以支持像where一样的条件 如下所示

```c#
    var sql = sqlClient.HiSql("select a.TabName, a.FieldName from Hi_FieldModel as a left join Hi_TabModel as b on a.TabName=b.TabName and a.TabName in ('H_Test') where a.TabName=b.TabName and a.FieldType>3 ").ToSql();
```



### 2022.6.7 解决mysql低版本字符集问题

在低版本的mysql下使用HiSql会报如下错误
```c#
One or more errors occurred.(Unknow collation:'utf8mb4_0900_ai_ci')
```
感谢freesql作者反馈的bug



### 2022.6.1 编号服务更新
平常业务中需要根据不同的规则生成唯一的流水号作为表中的主键，或是生成雪花id号 `HiSql` 提供流水号生成功能，该功能支持分布式唯一流水号生成

详细教程请查看 [hisql 编号详细教程](https://hisql.net/guide/number.html)


如下所示 
1. 配置

```c#
    Global.SnroOn = true;//表示启用流水号功能
    sqlClient.CodeFirst.InstallHisql();//启用后初始化安装时自动会创建表 Hi_Snro


    //新增流水号配置
    //SNRO 表示主编号名称 SNUM 表示子编号ID
    var obj1 = new { SNRO = "MATDOC", SNUM = 1, IsSnow=false, SnowTick=0, StartNum = "9000000", EndNum = "9999999",Length=7, CurrNum = "9000000", IsNumber = true, IsHasPre = false, CacheSpace = 10, Descript = "物料主数据编号" };
    
    
    //新增雪花ID生成配置
    //SNRO 表示主编号名称 SNUM 表示子编号ID
    var obj2 = new { SNRO = "Order", SNUM = 1, IsSnow=true, SnowTick=145444, StartNum = "", EndNum = "",Length=7, CurrNum = "", IsNumber = true, IsHasPre = false, CacheSpace = 10, Descript = "订单号雪花ID" };

    List<object> list = new List<object>();
    list.Add(obj1);
    list.Add(obj2);

    sqlClient.Modi("Hi_Snro", list).ExecCommand();
```


2. 使用流水号
```c#
    public static SeriNumber number = null;  //定义个全局变更的流水号对象

```

3. 设置流水号的连接
```c#
//sqlClient 为数据库连接对象
    number = new SeriNumber(sqlClient);

```

4. 启用分布式流水号
   
如果项目是分布式布署的一定要启用以下代码，否则会产生重复ID

```c#
    HiSql.Global.RedisOn = true;//开启redis缓存
    HiSql.Global.RedisOptions = new RedisOptions { Host = "172.16.80.178", PassWord = "qwe123", Port = 6379, CacheRegion = "HRM", Database = 2 };

    HiSql.Global.NumberOptions.MultiMode= true;`
```


5. 产生流水号
```c#
//Order:表示主编号名称 1:表示子编号id

    string num=number.NewNumber("MATDOC", 1);
```

6. 一次性产生多个流水号

```c#
// 一次产生10个流水号
    List<string> lstnum=number.NewNumber("MATDOC", 1,10);
```








### 2022.5.25 新增获取excel 的Sheet名称方法

```c#
HiSql.Extension.Excel excel = new HiSql.Extension.Excel(new Extension.ExcelOptions() { TempType = Extension.TempType.HEADER });
            List<string> names = excel.GetExcelSheetNames(@"D:\data\GD_UniqueCodeInfo1.xlsx");
            foreach (string name in names)
            {
                Console.WriteLine($"excel sheetName:{name}");
            }

```


### 2022.5.25 新增锁定成功事件通知，可用于锁信息外部存储

```c#
    HiSql.BaseCache rCache = new RCache(new RedisOptions { Host = "127.0.0.1", Port = 6379, PassWord = "",  Database = 3});
    rCache.IsSaveLockHis = true;
    rCache.OnLockedSuccess += (object sender, LockItemSuccessEventArgs e) => {
        Console.WriteLine($"锁定成功事件：key{e.Key} info:{ JsonConvert.SerializeObject(e.LckInfo)}");
    };

```

### 2022.5.23 新增雪花ID生成方法

雪花ID引擎每微秒理论可生成4096个不重复ID,

IdSnow引擎性能实测生成10000个ID耗时`0.00623`秒
IdWorker引擎性能实测生成10000个ID耗时`0.01364 `秒



```c#
    //指定雪花ID生成引擎(IdWorker和IdSnow) 默认是IdSnow
    Snowflake.SnowType = SnowType.IdSnow;

    //指定机器码（0-31)之间 默认是0
    Snowflake.WorkerId = 0;

    List<long> lst=new List<long>();
    Stopwatch sw = new Stopwatch();
    sw.Start();
    for (int i = 0; i < 10000; i++)
    {
        lst.Add(Snowflake.NextId());
       
    }
    sw.Stop();
    Console.WriteLine($"耗时：{sw.Elapsed}秒");

```




### 2022.5.20 业务锁使用方法


为什么要用业务锁？业务锁是防止多人同时操作某一个业务或表中某一条数据导致的业务问题或并发时产生的数据库级锁的问题

通过业务锁可以控制在同一时间只允许一个任务来执行，可用于库存扣减，秒杀等高并发场景


1. 检测指定的锁是否存在

```c#
    string _key = "4900001223";
    var rtn = HiSql.Lock.CheckLock(_key);
    if (!rtn.Item1)
    {
        Console.WriteLine($"没有其它人操作采购订单[{_key}]");
    }
    else
        Console.WriteLine(rtn.Item2);//输出是谁在操作采购订单


    //同时检测多个key是否被锁定 其中有一个锁定锁则返回锁定状态
    var rtn2 = HiSql.Lock.CheckLock("4900001223", "4900001224");
```

2. 占用锁和解除锁

占用锁即加锁,加锁后不允许其它任务占用
```c#
    string _key = "4900001223";

    /*
    加锁后默认超时时间为30秒，当执行超过27秒时会自动续锁30秒默认会自动续5次 超过则会取消执行
    可通过expresseconds和timeoutseconds 参数进行修改
    */

    //LckInfo 是指加锁时需要指定的信息  UName 表示加锁人，ip表示在哪一个地址加的锁，可以通过 HiSql.Lock.GetCurrLockInfo  获取所有的详细加锁信息便于后台管理
    var rtn= HiSql.Lock.LockOn(_key, new LckInfo { UName = "登陆名", Ip = "127.0.0.1" });
    if (rtn.Item1)
    {
        Console.WriteLine($"针对于采购订单[{_key}] 加锁成功");
        //执行采购订单处理业务

        //解锁  如果没有解锁默认30秒后会自动解锁
        HiSql.Lock.UnLock(_key);
    }

    //同时加锁多个key 如果有一个key被其它任务加锁那么 锁定失败
    var rtn2 = HiSql.Lock.LockOn(new string[] { "4900001223", "4900001224" }, new LckInfo { UName = "登陆名", Ip = "127.0.0.1" });

```

3. 占用并处理业务

```c#
    string _key = "4900001223";

    /*
    加锁后默认超时时间为30秒，当执行超过27秒时会自动续锁30秒默认会自动续5次 超过则会取消执行
    可通过expresseconds和timeoutseconds 参数进行修改
    */

    //LckInfo 是指加锁时需要指定的信息  UName 表示加锁人，ip表示在哪一个地址加的锁，可以通过 HiSql.Lock.GetCurrLockInfo  获取所有的详细加锁信息便于后台管理
    var rtn = HiSql.Lock.LockOnExecute(_key, () =>
    {
        //加锁成功后执行的业务
        Console.WriteLine($"针对于采购订单[{_key}] 加锁并业务处理成功");

        //处理成功后 会自动解锁


    }, new LckInfo { UName = "登陆名", Ip = "127.0.0.1" });

```



### 2022.5.19 hisql 查询语法新增字段与字段的条件判断

如下所示
`a.TabName=b.TabName`  操作符支持>=,>,<,<=,!=,<>

```c#
var _sql = sqlClient.HiSql("select a.TabName, a.FieldName from Hi_FieldModel as a inner join Hi_TabModel as b on a.TabName=b.TabName where a.TabName=b.TabName and a.FieldType>3").ToSql();
```


### 2022.5.16 hisql缓存支持多级缓存

hisql缓存支持多级缓存，优先取MemoryCache，再找redis缓存,如下所示

```c#
    HiSql.ICache rCache = new RCache(new RedisOptions { Host = "127.0.0.1", Port = 6379, PassWord = "", CacheRegion = "HR", Database = 2,EnableMultiCache = true }); //EnableMultiCache默认是启用的
                    rCache.SetCache("test1", list);
                    Stopwatch sw = Stopwatch.StartNew();
                    sw.Start();
                    Parallel.For(0, 10000, (x, y) =>
                    {
                        rCache.GetCache("test1");
                    });
                    Console.WriteLine($"测试多级缓存性能：" + sw.ElapsedMilliseconds);
```

### 2022.5.16 新增本地锁、分布式锁(redis锁）

业务代码可以使用本地锁或分布式锁(redis锁）避免多线程同时更新数据库出现数据库死锁，也可以在扣减库存场景使用。

1.单实例可以使用本地锁，支持同时加多个key，如下所示

```c#
    HiSql.ICache rCache = new MCache();
    Tuple<bool, string> rtn = rCache.LockOnExecute(new string[] { "test_key1","test_key2" }, () =>
                            {
                                try
                                {
                                    new TestInstance().TestInsertToDB();
                                }
                                catch (Exception EX)
                                {
                                    throw;
                                }
                            }, new LckInfo { UName = "hisql", EventName = "单次获取加锁动作", Ip = "192.168.1.1" }, 60, 20);
```
2.分布式实例可以使用redis锁，支持同时加多个key，如下所示

```c#
    HiSql.ICache rCache = new RCache(new RedisOptions { Host = "127.0.0.1", Port = 6379, PassWord = "", CacheRegion = "HR", Database = 2 });

    Tuple<bool, string> rtn = rCache.LockOnExecute(new string[] { "test_key1","test_key2" }, () =>
                            {
                                try
                                {
                                    new TestInstance().TestInsertToDB();
                                }
                                catch (Exception EX)
                                {
                                    throw;
                                }
                            }, new LckInfo { UName = "hisql", EventName = "单次获取加锁动作", Ip = "192.168.1.1" }, 60, 20);
```

### 2022.5.10 新增excel操作支持
平常在开发的过程需要将表中的数据导出到excel ,在excel编辑完成后再上传保存到表中
源码在：HiSql.Extension 目录中




1. 生成带字段 及字段描述为标题的excel数据

生成excel
```c#
    HiSqlClient sqlClient = Demo_Init.GetSqlClient();
    DataTable dt = sqlClient.HiSql("select * from GD_UniqueCodeInfo").ToTable();
    TabInfo tabInfo = sqlClient.Context.DMInitalize.GetTabStruct("GD_UniqueCodeInfo");

    // TempType = Extension.TempType.HEADER 
    HiSql.Extension.Excel excel = new HiSql.Extension.Excel(new Extension.ExcelOptions() { TempType = Extension.TempType.HEADER });
    excel.Add(new Extension.ExcelHeader(1).Add("表名").Add("GD_UniqueCodeInfo"));//标识表名

    Extension.ExcelHeader excelHeader = new Extension.ExcelHeader(2);
    Extension.ExcelHeader excelHeader3 = new Extension.ExcelHeader(3);
    foreach (DataColumn dataColumn in dt.Columns)
    {
        HiColumn hiColumn = tabInfo.Columns.Where(c => c.FieldName.Equals(dataColumn.ColumnName)).FirstOrDefault();
        if (hiColumn != null)
        {
            excelHeader.Add(string.IsNullOrEmpty(hiColumn.FieldDesc) ? dataColumn.ColumnName : hiColumn.FieldDesc);
        }
        else
            excelHeader.Add(dataColumn.ColumnName);

        excelHeader3.Add(dataColumn.ColumnName);
    }
    excel.Add(excelHeader);//字段中文描述
    excel.Add(excelHeader3);//字段名

    //生成excel
    excel.WriteExcel(dt, @"D:\data\GD_UniqueCodeInfo1.xlsx");

```

读取生成的excel
```c#
    HiSql.Extension.Excel excel = new HiSql.Extension.Excel(new Extension.ExcelOptions() { TempType = Extension.TempType.HEADER });
    DataTable dt = excel.ExcelToDataTable(@"D:\data\GD_UniqueCodeInfo1.xlsx", true);
```




2. 生成带数据库字段标题的excel
   
生成excel
```c#
    HiSqlClient sqlClient = Demo_Init.GetSqlClient();
    DataTable dt = sqlClient.HiSql("select * from GD_UniqueCodeInfo").ToTable();
    TabInfo tabInfo = sqlClient.Context.DMInitalize.GetTabStruct("GD_UniqueCodeInfo");

    HiSql.Extension.Excel excel = new HiSql.Extension.Excel();
    //生成excel
    excel.WriteExcel(dt, @"D:\data\GD_UniqueCodeInfo2.xlsx");
```

读取excel
```c#
    HiSql.Extension.Excel excel = new HiSql.Extension.Excel(new Extension.ExcelOptions() { TempType = Extension.TempType.STANDARD, DataBeginRow = 2, HeaderRow = 1 });
    DataTable dt = excel.ExcelToDataTable(@"D:\data\GD_UniqueCodeInfo2.xlsx", true);
```


3. 生成自定义标题的excel
生成excel
```c#
    HiSqlClient sqlClient = Demo_Init.GetSqlClient();
    DataTable dt = sqlClient.HiSql("select * from GD_UniqueCodeInfo").ToTable();
    TabInfo tabInfo = sqlClient.Context.DMInitalize.GetTabStruct("GD_UniqueCodeInfo");
    HiSql.Extension.Excel excel = new HiSql.Extension.Excel();
    Extension.ExcelHeader excelHeader = new Extension.ExcelHeader(2);

    foreach (DataColumn dataColumn in dt.Columns)
    {
        HiColumn hiColumn = tabInfo.Columns.Where(c => c.FieldName.Equals(dataColumn.ColumnName)).FirstOrDefault();
        if (hiColumn != null)
        {
            excelHeader.Add(string.IsNullOrEmpty(hiColumn.FieldDesc) ? dataColumn.ColumnName : hiColumn.FieldDesc);
        }
        else
            excelHeader.Add(dataColumn.ColumnName);


    }
    excel.Add(excelHeader);//字段中文描述

    //生成excel
    excel.WriteExcel(dt, @"D:\data\GD_UniqueCodeInfo3.xlsx");
```

读取excel
```c#
    HiSql.Extension.Excel excel = new HiSql.Extension.Excel(new Extension.ExcelOptions() { TempType = Extension.TempType.STANDARD, DataBeginRow = 2, HeaderRow = 1 });

    DataTable dt = excel.ExcelToDataTable(@"D:\data\GD_UniqueCodeInfo3.xlsx", true);
```





### 2022.4.21 新增支持redis缓存
hisql底层默认是使用 MemoryCache 进行表结构信息的缓存处理(如果项目是单体应用的情况请建义使用这种方式)，如果项目是分布式的 可以使用redis作为缓存载体，如下所示

```c#
HiSql.Global.RedisOn = true;//开启redis缓存
HiSql.Global.RedisOptions = new RedisOptions { Host = "172.16.80.178", PassWord = "pwd123", Port = 6379, CacheRegion = "HRM", Database = 2 };
```

### 2022.4.15 更新

1. 新增获取表记录数 
```c#
//返回表Hi_FieldModel 中的记录数
int lsttales = sqlClient.DbFirst.GetTableDataCount("Hi_FieldModel");

```

2. 分页获取当前库中表清单
```c#
//获取表名出现 HI 即（ %HI%）字符串的表列表清单进行分页（每页显示10条）,且返回记录总数
int total = 0;
List<TableInfo> lsttales = sqlClient.DbFirst.GetTables("HI", 10,1, out total);
```

为什么会增加此功能?
在低代码平台可以查询库中的任意表及数据，及定时统计表中的数据增长量。一般的低代码平台中基本上都会数据库相关管理功能。


### 2022.4.9 更新
HiSql新增对oracle表的操作（目前支持`SqlServer`,`mysql`，`HANA`,`POSTGRESQL`，`oracle`） 目前已经对HIsql所表支持的数据都新增了 表结构管理的适配

操作写法请参照 2022.3.3 更新

### 2022.4.8 更新
HiSql新增对postgresql表的操作（目前支持`SqlServer`,`mysql`，`HANA`,`POSTGRESQL`，陆续会加上对其它数据库的实现）

操作写法请参照 2022.3.3 更新


### 2022.3.25 更新

1. HiSql语句新增 distinct 支持(注意在分页情况下不支持)
```c#
var _sql2 = sqlClient.HiSql("select distinct TabName  from Hi_FieldModel where TabName='Hi_FieldModel' order by TabName ").ToSql();

```
2. HiSql新增对hana表的操作（目前支持`SqlServer`,`mysql`，`Hana` 陆续会加上对其它数据库的实现）
   操作写法请参照 2022.3.3 更新

3. 参数增加in 参数如下所示
    当参数为List集合参数时 只能用于in()中
    注：不同的数据Hisql的语法是一样的
```c#
var _sql1 = sqlClient.HiSql("select   TabName  from Hi_FieldModel where  FieldType in( [$list$]) order by TabName ",
    new Dictionary<string, object> { { "[$list$]",new List<int> { 1,2,3,4} } }).ToSql();
```
生成的sql如下所示 hisql会根据不同的数据库的特性解析成不同的原生sql语句
```sql
select  [Hi_FieldModel].[TabName] from [Hi_FieldModel] as [Hi_FieldModel]
 where [Hi_FieldModel].[FieldType] in (1,2,3,4)
 order by  [Hi_FieldModel].[TabName] ASC

```


### 2022.3.24 更新
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
