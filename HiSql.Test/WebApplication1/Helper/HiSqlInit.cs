using System;
using HiSql;

namespace WebApplication1.Helper;

public class HiSqlInit
{
    public static void Init(IServiceCollection services)
    {
        services.AddTransient<HiSqlClient, TestHiSqlClient>();
    }

    public static HiSqlClient GetSqlClient(string name)
    {
        return new TestHiSqlClient();
    }
}
