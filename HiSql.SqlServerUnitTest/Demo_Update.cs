using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HiSql.UnitTest
{
    public class Demo_Update
    {

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



        public static void Init(HiSqlClient sqlClient)
        {
            //Update_Demo(sqlClient);
            //Update_Demo2(sqlClient);
            //Update_Demo3(sqlClient);
            Update_Demo4(sqlClient);
        }

        static void Update_Demo4(HiSqlClient sqlClient)
        {
            sqlClient.BeginTran(System.Data.IsolationLevel.ReadUncommitted);
            int _effect= sqlClient.Update("HTest01", new { sid = 123456, UName = "tansar", Age = 25, Salary = 1999.9, Descript = "hello worl111da" }).ExecCommand();
            //string _sql=sqlClient.Update("HTest01").Set(new { UTYP = "U3" }).Where("SID=0").ToSql();

            sqlClient.Modi("H_UType", new List<object> {
                new { UTYP = "U1", UTypeName = "普通用户" },
                new { UTYP = "U2", UTypeName = "中级用户" },
                new { UTYP = "U3", UTypeName = "高级用户" }
            }).ExecCommand();
            //sqlClient.CommitTran();
        }

        static void Update_Demo3(HiSqlClient sqlClient)
        {
            //string _sql = sqlClient.Update("Hi_FieldModel", new { TabName = "HTest01", FieldName = "UName",Regex= @"^[\w]+[^']$" }).Only("Regex").ToSql();
            //sqlClient.Update("Hi_FieldModel", new { TabName = "HTest01", FieldName = "UTYP", Regex = @"" }).Only("Regex").ExecCommand();

            sqlClient.Update("HTest01", new { SID = "0", UTYP = "U4", UName = "hisql", Age = 36, Salary = 11, Descript = "hisql" }).ExecCommand();
        }

        static void Update_Demo2(HiSqlClient sqlClient)
        {
            //string _txt = @"`UserAge`= `UserAge` + 1";
            string _txt = @"abc";
            string _reg = @"[`](?:[\s]*)(?:(?<flag>[\#]{1,2}|[\@]{1})?(?<tab>[\w]+)(?:[\.]{1}))?(?<field>[\w]+)[`]";
            List<Dictionary<string, string>> lst_dic = Tool.RegexGrps(_reg, _txt);

            if (lst_dic.Count() > 0)
            {
                Regex regex = new Regex(_reg);
                foreach (Dictionary<string, string> dic in lst_dic)
                {
                    _txt = regex.Replace(_txt, dic["field"].ToString(), 1);
                }
            }

            string _sql = sqlClient.Update("H_TEST", new { Hid = 1, UNAME = "UTYPE", UserAge= "`UserAge`+1", UNAME2 = "user123" }).Exclude("UNAME").ToSql();



        }

        static void Update_Demo(HiSqlClient sqlClient)
        {

            Dictionary<string, string> _dic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { "DID", "1" }, { "UNAME", "user123" } };


            List<Dictionary<string, object>> _diclst = new List<Dictionary<string, object>>() {
                new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase) { { "DID", 1 }, { "UNAME", "user123" } },
                new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase) { { "DID", 2 }, { "UNAME", "us333" } }
            };

            List<object> lstdyn = new List<object>();
            for (int i = 0; i < 10; i++)
            {
                TDynamic dyn1 = new TDynamic();
                dyn1["Hid"] = 150 + i;
                dyn1["UserName"] = $"tgm{i}";
                dyn1["UserAge"] = 34;
                dyn1["Birth"] = DateTime.Now;
                lstdyn.Add(dyn1);
            }

            IUpdate _dicupdate = sqlClient.Update("H_TEST", lstdyn);
            string _dicsql = _dicupdate.ToSql();


            IUpdate update = sqlClient.Update("H_TEST", new { DID = 1, UNAME = "UTYPE" ,UNAME2= "user123" }).Exclude("UNAME");//,

            //int _effect = sqlClient.Update("H_TEST", new { DID = 1, UNAME = "UTYPE", UNAME2 = "user123" }).Exclude("UNAME").ExecCommand();
            string _sql = update.ToSql();
            Console.WriteLine(_sql);


            IUpdate update1 = sqlClient.Update("H_TEST").Set(new { UNAME2 = "TEST" }).Where(new Filter { { "DID", OperType.GT, 8 } });
            IUpdate update1_2 = sqlClient.Update("H_TEST").Set(new { UNAME2 = "TEST" }).Where("DID>8");
            //int _effect1 = sqlClient.Update("H_TEST").Set(new { UNAME2 = "TEST" }).Where(new Filter { { "DID", OperType.GT, 8 } }).ExecCommand();
            string _sql1 = update1.ToSql();
            string _sql12 = update1_2.ToSql();
            Console.WriteLine(_sql1);

            IUpdate update2 = sqlClient.Update<H_Test>(new H_Test {DID=1 ,UNAME2="Haha1"}).Only("UNAME2");
            //int _effect2 = sqlClient.Update<H_Test>(new H_Test { DID = 1, UNAME2 = "Haha1" }).Only("UNAME2").ExecCommand();
            string _sql2 = update2.ToSql();
            Console.WriteLine(_sql2);

             
            IUpdate update3 = sqlClient.Update("H_TEST", new List<object> { new { DID = 1,   UNAME2 = "user123" }, new { DID = 2,   UNAME2 = "user124" } }).Only("UNAME2");
            //int _effect3 = sqlClient.Update("H_TEST", new List<object> { new { DID = 1, UNAME2 = "user123" }, new { DID = 2, UNAME2 = "user124" } }).Only("UNAME2").ExecCommand();
            string _sql3 = update3.ToSql();
            Console.WriteLine(_sql3);

            //"did > 8"



            if (string.IsNullOrEmpty(_sql2))
            { 
                
            }

        }
    }
}
