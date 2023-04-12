using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql.HanaUnitTest
{
    class DemoCodeFirst
    {
        public static void Init(HiSqlClient sqlClient)
        {
            //CodeFirst_Demo(sqlClient);
            //CodeFirst_DbDefault(sqlClient);
            ModiTableDemo(sqlClient);
            string s = Console.ReadLine();
        }


        static void ModiTableDemo(HiSqlClient sqlClient)
        {
            TabInfo tabInfo = sqlClient.DbFirst.GetTabStruct("H_Test02");

            var jsons= tabInfo.Columns.ToJson();

            Tuple<bool, string, string> rtn= sqlClient.DbFirst.ModiTable(tabInfo, OpLevel.Check);


        }

        static void CodeFirst_DbDefault(HiSqlClient sqlClient)
        {
            List<DefMapping> _lstdefmapping = new List<DefMapping>();
            _lstdefmapping.Add(new DefMapping { IsRegex = false, DbValue = @"", DBDefault = HiTypeDBDefault.NONE });
            //数字默认
            _lstdefmapping.Add(new DefMapping { IsRegex = true,  DbValue = @"^(?<value>[-]?\d+(?:[\.]?)[\d]*)$", DbType = HiTypeGroup.Number, DBDefault = HiTypeDBDefault.VALUE });

            //true or false
            _lstdefmapping.Add(new DefMapping { IsRegex = true, DbValue = @"^(?<value>[01]{1})$", DbType = HiTypeGroup.Bool, DBDefault = HiTypeDBDefault.VALUE });

            //指定字符默认值 如('hisql') 或('')
            _lstdefmapping.Add(new DefMapping { IsRegex = true, DbValue = @"^(?<value>(?<!CURRENT_TIMESTAMP)\w*)$", DbType = HiTypeGroup.Char, DBDefault = HiTypeDBDefault.VALUE });

            //日期
            _lstdefmapping.Add(new DefMapping { IsRegex = true, DbValue = @"^(?<value>CURRENT_TIMESTAMP)\s*$", DbType = HiTypeGroup.Date, DBDefault = HiTypeDBDefault.FUNDATE });

            //md5值
            //_lstdefmapping.Add(new DefMapping { IsRegex = true, DbValue = @"^\((?<value>newid\(\))\)$", DBDefault = HiTypeDBDefault.FUNGUID });


            HiType hiType = HiType.DATE;
            string _value = "CURRENT_TIMESTAMP";
            string _defaultvalue = "";
            HiTypeDBDefault hiTypeDBDefault = HiTypeDBDefault.NONE;

            List<DefMapping> _lstdef = new List<DefMapping>();
            if (hiType.IsCharField())
            {
                _lstdef = _lstdefmapping.Where(d => d.DbType == HiTypeGroup.Char || d.DBDefault == HiTypeDBDefault.NONE).ToList();//
            }
            else if (hiType.IsNumberField())
            {
                _lstdef = _lstdefmapping.Where(d => d.DbType == HiTypeGroup.Number).ToList();
            }
            else if (hiType.IsDateField())
            {
                _lstdef = _lstdefmapping.Where(d => d.DbType == HiTypeGroup.Date).ToList();
            }
            else if (hiType.IsBoolField())
            {
                _lstdef = _lstdefmapping.Where(d => d.DbType == HiTypeGroup.Bool).ToList();
            }
            bool _flag = false;
            foreach (DefMapping def in _lstdef)
            {
                if (def.IsRegex)
                {
                    Dictionary<string, string> _dic = Tool.RegexGrp(def.DbValue, _value);
                    if (_dic.Count > 0 && _dic.ContainsKey("value"))
                    {
                        _defaultvalue = _dic["value"].ToString();
                        if (string.IsNullOrEmpty(_defaultvalue))
                        {
                            hiTypeDBDefault = HiTypeDBDefault.EMPTY;
                        }
                        if (def.DbType.IsIn<HiTypeGroup>(HiTypeGroup.Char, HiTypeGroup.Bool, HiTypeGroup.Number))
                        {
                            hiTypeDBDefault = HiTypeDBDefault.VALUE;
                        }
                        else if (def.DbType.IsIn<HiTypeGroup>(HiTypeGroup.Date))
                        {
                            hiTypeDBDefault = HiTypeDBDefault.FUNDATE;
                        }
                        else
                        { 
                            //忽略
                        }

                        Console.WriteLine($"类型[{def.DbType}]默认值{_dic["value"].ToString()} 默认值类型{hiTypeDBDefault}");
                        _flag = true;
                    }
                }
                else
                {
                    if (_value == def.DbValue)
                    {
                        Console.WriteLine("匹配空值");
                        _flag = true;
                    }
                }
            }
            if (_flag)
            {
                Console.WriteLine("匹配成功");
            }
            else
            {
                Console.WriteLine("匹配失败");
            }

            
        }

        static void CodeFirst_Demo(HiSqlClient sqlClient)
        {
            //IDM dMBuilder = Instance.CreateInstance<IDM>($"{Constants.NameSpace}.{sqlClient.Context.CurrentConnectionConfig.DbType.ToString()}{DbInterFace.DM.ToString()}");
           
            
            //Tuple<HiTable, List<HiColumn>> tabomdel = sqlClient.Context.DMInitalize.BuildTabStru(typeof(Hi_TabModel));
            //Tuple<HiTable, List<HiColumn>> fieldomdel = sqlClient.Context.DMInitalize.BuildTabStru(typeof(Hi_FieldModel));
            //TabInfo tabinfo_tab = sqlClient.Context.DMInitalize.BuildTab(typeof(Hi_Domain));
            //TabInfo tabinfo_field = sqlClient.Context.DMInitalize.BuildTab(typeof(Hi_DataElement));

            ////tabinfo_tab.TabModel.TabName = "##"+tabinfo_tab.TabModel.TabName;
            ////tabinfo_tab.TabModel.TabReName = tabinfo_tab.TabModel.TabName.Substring(2)+"_"+System.Threading.Thread.CurrentThread.ManagedThreadId+"_"+ tabinfo_tab.TabModel.TabName.GetHashCode().ToString().Substring(1);
            ////BuildTabCreateSql
            //TabInfo tab_info_test=  sqlClient.Context.DMInitalize.GetTabStruct("H_TEST");

            //string _sql= sqlClient.Context.DMTab.BuildTabCreateSql(tabinfo_field.TabModel, tabinfo_field.GetColumns, true);





            //int effec= sqlClient.Context.DBO.ExecCommand(_sql);
            //DataTable dt = sqlClient.Context.DBO.GetDataTable($"select * from   \"SAPHANADB\".\"#Hi_FieldModel\";");
            sqlClient.CodeFirst.InstallHisql();




            //int result= idfmi.BuildTabCreate(tabinfo_tab);
            //if (result > 0)
            //    Console.WriteLine($"表[{tabinfo_tab.TabModel.TabName}]创建成功!");
            //else
            //    Console.WriteLine($"表[{tabinfo_tab.TabModel.TabName}]已经存在!");




        }
    }
}
