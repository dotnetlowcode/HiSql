using System;
using HiSql;
using Microsoft.Extensions.DependencyInjection;

namespace WebApplication1.Helper;

public static class HiSqlInit
{
    public static void Init(IServiceCollection services)
    {
        services.AddTransient<HiSqlClient, TestHiSqlClient>();
    }
    
     public static  IServiceProvider serviceProvider { get; set; }

    public static void InitApp(this IApplicationBuilder app)
    {
        serviceProvider = app.ApplicationServices;
    }

    public static HiSqlClient GetSqlClient(string name)
    {
        return new TestHiSqlClient();
    }
}
