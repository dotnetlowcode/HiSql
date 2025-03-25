using System;
using HiSql;

namespace WebApplication1.Helper;



public class TestHiSqlClient : HiSqlClient
{

    //   "DbType": "PostGreSql",
    //   "DbServer": "ykerp",
    //   "ConnectionString": "PORT=15432;DATABASE=thirdapi;HOST=192.168.10.141;PASSWORD=hone123;USER ID=root",
    //   "Schema": "public",
    //   "IsOn": true
    public TestHiSqlClient() : base(new ConnectionConfig
    {
        DbType = DBType.PostGreSql,
        ConnectionString = "PORT=15432;DATABASE=thirdapi;HOST=192.168.10.141;PASSWORD=hone123;USER ID=root",
        Schema = "public",
        DbServer = "ykerp"
    })
    {
    }
}
