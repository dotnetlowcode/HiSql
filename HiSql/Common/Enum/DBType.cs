using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// 支持的数据库类型
    /// </summary>
    public enum DBType
    {
        SqlServer=1,
        MySql=2,
        SQLite=3,
        Oracle=4,
        Hana=5,
        PostGreSql = 6,
        MongoDb=7

        //Access=6,

    }


    //public class DBTYPE
    //{
    //    string _name;
    //    public DBTYPE(string name) {
    //        _name = name;
    //    }
        
    //    public override string ToString()
    //    {
    //        return _name;
    //    }
    //}
    //public static partial  class DbTypeName
    //{ 
        
    //    /// <summary>
    //    /// 获取数据库支持列表
    //    /// </summary>
    //    public static List<string> SupportList
    //    {
    //        get
    //        {
    //            Type _type = typeof(DbTypeName);
    //            List<string> _list = new List<string>();

    //            List<PropertyInfo> aList = _type.GetProperties().Where(p=>p.PropertyType==Constants.StringType && p.GetGetMethod().IsStatic && p.MemberType==MemberTypes.Property).ToList<PropertyInfo>();
    //            foreach (PropertyInfo p in aList)
    //            {
    //                MemberInfo memberInfo = (MemberInfo)p;
    //                _list.Add(p.Name);


    //            }
    //            return _list;

    //        }

            
    //    }
    //    public static DBTYPE DbTypeNames()
    //    {
    //        return new DBTYPE("");
    //    }

    //}
    //public static partial class DbTypeName
    //{
    //    public static string SqlServer
    //    {
    //        get
    //        {
    //            return "SqlServer";
    //        }

    //    }
    //}

}
