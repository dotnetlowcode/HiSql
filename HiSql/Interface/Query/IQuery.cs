using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Linq.Expressions;
using System.Dynamic;

namespace HiSql
{



    public partial interface IQuery
    {
        HiSqlProvider Context { get; set; }


        /// <summary>
        /// 是否去重
        /// </summary>
        bool IsDistinct { get; set; }


        IQuery Query(string tabname, string rename);
        IQuery Query(string tabname);
        IQuery HiSql(string hisql, IQuery query);
        IQuery Query(params IQuery[] query);

        IQuery As(string retabname);
        IQuery Where(Filter where);


        /// <summary>
        /// 支持Hisql 中间语言的sql条件
        /// </summary>
        /// <param name="sqlwhere"></param>
        /// <returns></returns>
        IQuery Where(string sqlwhere);

        /// <summary>
        /// 指定一个或多个查询字段
        /// 语法如下 表名如果指定了别名请务必用别名
        /// Field("表名1.字段1","表名1"."字段1")
        /// 也可以是  Field("表名1.字段1 as 别名字段")
        /// </summary>
        /// <param name="fields">
        /// params 参数 可以多个字段参数值
        /// </param>
        /// <returns>返回当前对象支持链式操作
        /// (后面的操作中不允重复使用该方法)
        /// </returns>
        IQuery Field(params string[] fields);


        IQuery Join(JoinDefinition join);

        IQuery Join(string tabname, string retabname, JoinType joinType = JoinType.Inner);

        IQuery Join(string tabname, JoinType joinType = JoinType.Inner);

        /// <summary>
        /// ON
        /// </summary>
        /// <param name="joinon"></param>
        /// <returns></returns>
        IQuery On(JoinOn joinon);
        IQuery On(string leftCondition, string rightCcondition);

        IQuery On(string condition);

        IQuery Sort(SortBy sort);

        //IQuery Sort(string sort);

        IQuery Sort(SortByDefinition sort);

        IQuery Sort(params string[] sort);


        /// <summary>
        /// 分组排名方法
        /// </summary>
        /// <param name="rank">排名类型</param>
        /// <param name="dbFunction">排名方法</param>
        /// <param name="field">排名字段</param>
        /// <param name="asname">排名字段重命名</param>
        /// <returns></returns>
        IQuery WithRank(DbRank rank, DbFunction dbFunction, string field, string asname, SortType sortType);
        IQuery WithRank(DbRank rank, Ranks ranks, string asname);

        /// <summary>
        ///不如果不指定锁参数则无效
        /// </summary>
        /// <param name="lockMode"></param>
        /// <returns></returns>
        IQuery WithLock(LockMode lockMode = LockMode.NONE);

        /// <summary>
        /// 显示第几页的数据
        /// </summary>
        /// <param name="currpage"></param>
        /// <returns></returns>
        IQuery Skip(int currpage);
        /// <summary>
        /// 页大小(显示最大记录数)
        /// </summary>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        IQuery Take(int pagesize);

        IQuery Group(GroupDefinition group);

        IQuery Group(GroupBy group);

        IQuery Group(params string[] group);

        IQuery Having(string having);

        IQuery Having(Having havings);




        void Insert(string tabname);





        /// <summary>
        /// case 查询
        /// sqlClient
        ///.Query("UserList")
        ///.Field("UNAME","age")
        ///.Case("age")
        ///.When("age>10").Then("bt10")
        ///.When("age>20").Then("bt20")
        ///.EndAs("ageRange",typeof(int))
        ///.Field("descrip");
        /// </summary>
        /// <param name="fieldname"></param>
        /// <returns></returns>
        IQuery Case(string fieldname);


        /// <summary>
        /// case条件 必须要结合Switch使用
        /// </summary>
        /// <param name="fieldexpress"></param>
        /// <returns></returns>
        IQuery When(string fieldexpress);

        /// <summary>
        /// 当条件满足时 必须在Case后面
        /// </summary>
        /// <param name="thenvalue"></param>
        /// <returns></returns>
        IQuery Then(string thenvalue);


        /// <summary>
        /// 当所有条件都不满足时执行的语句
        /// </summary>
        /// <param name="elsevalue"></param>
        /// <returns></returns>
        IQuery Else(string elsevalue);

        /// <summary>
        /// 结束Switch 条件判断 并指定类值的返回类型
        /// </summary>
        /// <param name="asfieldname"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        IQuery EndAs(string asfieldname, Type type);

        IQuery EndAs(string asfieldname);


        /// <summary>
        /// 获取当前数据库 转换后的表名
        /// </summary>
        /// <returns></returns>
        string GetDbName(string tabname);


        /// <summary>
        /// 将前的操作生成目库的sql语句
        /// </summary>
        /// <returns></returns>
        string ToSql();


        /// <summary>
        /// 将结果集返回指定的实体类结果集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        List<T> ToList<T>();
        /// <summary>
        /// 当有分页时返回实体数据，并返回当前查询条件的总记录数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="total">总记录数</param>
        /// <returns></returns>
        List<T> ToList<T>(ref int total);
        /// <summary>
        /// 将查询结果返回一个DataTable结果集
        /// </summary>
        /// <returns></returns>
        DataTable ToTable();
        /// <summary>
        /// 当有分页时返回DataTable，并返回当前查询条件的总记录数
        /// </summary>
        /// <param name="total"></param>
        /// <returns></returns>
        DataTable ToTable(ref int total);


        /// <summary>
        /// 返回动态类型结果集
        /// </summary>
        /// <returns></returns>
        List<TDynamic> ToDynamic();
        /// <summary>
        /// 当有分页时返回动态类型结果集，并返回当前查询条件的总记录数
        /// </summary>
        /// <param name="total"></param>
        /// <returns></returns>
        List<TDynamic> ToDynamic(ref int total);

        /// <summary>
        /// 将数据转成json字符串
        /// </summary>
        /// <returns></returns>
        string ToJson();
        /// <summary>
        /// 当有分页时将数据转成json字符串，并返回当前查询条件的总记录数
        /// </summary>
        /// <param name="total"></param>
        /// <returns></returns>
        string ToJson(ref int total);

        /// <summary>
        /// 将数据转成动态对象
        /// </summary>
        /// <returns></returns>
        List<ExpandoObject> ToEObject();

        /// <summary>
        /// 将数据转成动态对象
        /// </summary>
        /// <returns></returns>
        Task<List<ExpandoObject>> ToEObjectAsync();

        /// <summary>
        /// 当有分页时返回总条数，并将数据转成动态对象
        /// </summary>
        /// <param name="total"></param>
        /// <returns></returns>
        Task<List<ExpandoObject>> ToEObjectAsync(ref int total);
    }
 

}
