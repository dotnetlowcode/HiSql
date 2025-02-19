using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HiSql.TabLog.Interface;
using HiSql.TabLog.Model;
using HiSql.TabLog.Module;
using HiSql.TabLog.Queue;
using HiSql.TabLog.Service;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace HiSql.TabLog.Ext
{
    public static class InstallTableLog
    {
        public static Func<IServiceScope, string, HiSqlClient> GetSqlClientByName { get; set; }

        public static Func<IServiceScope> FuncGetScope { get; set; }

        public static IServiceCollection AddTabLogServer(this IServiceCollection services)
        {
            //注册日志队列
            services.AddSingleton<HiSqlTabLogQueue>();

            //注册日志处理模块
            services.AddSingleton<HiSqlCredentialModule>();

            //注册后台日志保存服务
            services.AddHostedService<BackgroundTabLogService>();

            return services;
        }

        public static Task SetupTable(
            HiSqlClient sqlClient,
            Func<IServiceScope, string, HiSqlClient> _getSqlClientByName,
            Func<IServiceScope> _funcGetScope
        )
        {
            GetSqlClientByName = _getSqlClientByName;
            FuncGetScope = _funcGetScope;
            List<Type> _tabTypes = new List<Type> { typeof(ILogTable) };
            var _listType = AppDomain
                .CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t =>
                    _tabTypes.Any(k => k.IsAssignableFrom(t) && t != k) && t.IsClass && t.IsPublic
                )
                .ToList();

            bool _isInit = true;
            foreach (var type in _listType)
            {
                var tableName = type.Name;
                //获取type的属性标记
                var _attrs = type.GetCustomAttributes(typeof(HiSql.HiTable), true);
                if (_attrs.Length > 0)
                {
                    var tableSetting = _attrs[0] as HiSql.HiTable;
                    tableName = tableSetting?.TabName;
                }
                bool _isExits = sqlClient.DbFirst.CheckTabExists(tableName);
                if (!_isExits)
                {
                    if (!sqlClient.DbFirst.CreateTable(type))
                    {
                        Console.WriteLine($"\t\t创建表[{tableName}]失败...");
                        _isInit = false; //有一个创建失败就代表失败
                    }
                    else
                        Console.WriteLine($"\t\t创建表[{type.Name}]成功...");
                }
                else
                    Console.WriteLine($"\t\t表[{type.Name}]已经存在");
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// 初始化日志表
        /// </summary>
        /// <param name="sqlClient"></param>
        /// <returns></returns>
        public static void InitTabeLog(
            IServiceScope scope,
            HiSqlClient sqlClient,
            Hi_TabManager logTable
        )
        {
            //读取Hi_TabManager,并初始化日志表
            var tableName = typeof(Hi_TabManager).Name;
            var templateMainTableName = typeof(Th_MainLog).Name;
            var detailTemplateTableName = typeof(Th_DetailLog).Name;

            //检查主键编号规则是否存在,放在本库,不放存储库
            checkPrimaryKey(sqlClient, logTable);

            for (int i = 0; i < 2; i++)
            {
                var checkTableName = logTable.MainTabLog;
                var templateTableName = templateMainTableName;
                if (i == 1)
                {
                    checkTableName = logTable.DetailTabLog;
                    templateTableName = detailTemplateTableName;
                }
                var templateTableStruct = sqlClient.DbFirst.GetTabStruct(templateTableName);
                var jsonColumns = JArray.FromObject(templateTableStruct.Columns);
                var createTableClient = GetSqlClientByName(scope, logTable.DbServer);
                //检查表是否存在
                var _isExits = createTableClient.DbFirst.CheckTabExists(checkTableName);
                if (!_isExits)
                {
                    var columns = jsonColumns.ToObject<List<HiColumn>>();
                    var tableStruct = new TabInfo
                    {
                        TabModel = new HiTable { TabName = checkTableName },
                        Columns = columns
                    };
                    if (!createTableClient.DbFirst.CreateTable(tableStruct))
                        Console.WriteLine($"\t\t创建表[{checkTableName}]失败...");
                    else
                        Console.WriteLine($"\t\t创建表[{checkTableName}]成功...");
                }
                else
                    Console.WriteLine($"\t\t表[{checkTableName}]已经存在");
            }

            //获取配置信息
            //检查目标表是否已经初始化好
            //自增配置是否已经初始化好
            //有缓存就从缓存读,没有缓存就初始化并写入缓存
        }

        /// <summary>
        /// 检查主键编号规则是否存在,不存在则初始化
        /// </summary>
        /// <param name="client"></param>
        /// <param name="setting"></param>
        private static void checkPrimaryKey(HiSqlClient client, Hi_TabManager setting)
        {
            SnroNumber.SqlClient = client;
            var tempList = client
                .Query("Hi_Snro")
                .Field("SNRO")
                .Where(new Filter { { "SNRO", OperType.EQ, setting.SNRO } })
                .ToList<Hi_Snro>();
            if (tempList.Count < 1)
            {
                List<object> list = new List<object>
                {
                    new Hi_Snro
                    {
                        SNRO = setting.SNRO,
                        SNUM = setting.SNUM,
                        IsSnow = false,
                        SnowTick = 0,
                        StartNum = "1000000",
                        EndNum = "1999999",
                        Length = 7,
                        CurrNum = "1000000",
                        CurrAllNum = "",
                        PreChar = "",
                        IsNumber = true,
                        PreType = PreType.YMDH,
                        FixPreChar = "",
                        IsHasPre = true,
                        CacheSpace = 50,
                        Descript = setting.TabName + "日志主键"
                    }
                };
                client.Modi("Hi_Snro", list).ExecCommand();
            }
        }
    }
}
