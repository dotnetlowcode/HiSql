using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql.UnitTest
{
    internal class Demo_Upgrade
    {
        public static void Init(HiSqlClient sqlClient)
        {
            //Demo1(sqlClient);
            //Demo2(sqlClient);
            Demo3(sqlClient);
        }

        static void Demo1(HiSqlClient sqlClient)
        {

            //sqlClient.CodeFirst.InstallHisql();


            TabInfo tabInfo= sqlClient.Context.DMInitalize.GetTabStruct("Hi_FieldModel");

            TabInfo tabinfo2=tabInfo.CloneCopy();

            HiColumn dbnamecol = new HiColumn
            {
                TabName = "Hi_FieldModel",
                FieldName = "DbName",
                SortNum = 4,
                FieldLen = 50,
                FieldType = HiType.VARCHAR,
                IsBllKey = true,
                IsPrimary = true,
                DBDefault = HiTypeDBDefault.VALUE,
                DefaultValue = "",
                FieldDesc = "数据库名"
            };

            if (!tabinfo2.Columns.Any(c => c.FieldName.Equals(dbnamecol.FieldName, StringComparison.OrdinalIgnoreCase)))
                tabinfo2.Columns.Add(dbnamecol);
            else
            {
                for(int i=0;i< tabinfo2.Columns.Count;i++)
                {
                    if (tabinfo2.Columns[i].FieldName.Equals(dbnamecol.FieldName, StringComparison.OrdinalIgnoreCase))
                    {
                        tabinfo2.Columns[i] = dbnamecol.CloneCopy();
                    }
                }
                //HiColumn col = tabinfo2.Columns.Where(c => c.FieldName.Equals(dbnamecol.FieldName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                //if(col != null)
                //    col=dbnamecol.CloneCopy();
            }
            var tuple=sqlClient.DbFirst.ModiTable(tabinfo2, OpLevel.Execute);
            

        }

        static void Demo2(HiSqlClient sqlClient)
        {
            List<string> list = Constants.DbCurrentSupportList;

            Version version = Constants.HiSqlVersion;
            Console.WriteLine($"hisql:{version.ToString()}");

            foreach (string n in list)
            {
                Version ver = Constants.GetDbTypeVersion(n);
                Console.WriteLine($"{n}:{ver.ToString()}");
            }

        }

        static void Demo3(HiSqlClient sqlClient)
        {

            List<Hi_UpgradeInfo> lstupgradeinfo = new List<Hi_UpgradeInfo>();


            lstupgradeinfo.Add(
                new Hi_UpgradeInfo
                {
                    MinVersion = new Version("1.0.0.1"),
                    MaxVersion = new Version("1.0.4.7"),
                    UpgradTabs = new List<Hi_UpgradeTab>
                    {
                        new Hi_UpgradeTab{
                            TabName="Hi_TabModel",
                            Columns=new List<Hi_UpgradeCol>{
                                new Hi_UpgradeCol {
                                    TabFieldAction = TabFieldAction.MODI ,
                                    ColumnInfo=new HiColumn { TabName= "Hi_TabModel",FieldName="DbName", SortNum=4, FieldLen=50,FieldType=HiType.NVARCHAR,IsBllKey=true,IsPrimary=true,DBDefault=HiTypeDBDefault.VALUE,DefaultValue="",FieldDesc="数据库名"
                                    }
                                }
                            }
                        }
                    }
                });

            lstupgradeinfo.Add(
                new Hi_UpgradeInfo
                {
                    MinVersion = new Version("1.0.0.1"),
                    MaxVersion = new Version("1.0.4.7"),
                    UpgradTabs = new List<Hi_UpgradeTab>
                    {
                        new Hi_UpgradeTab{
                            TabName="Hi_FieldModel",
                            Columns=new List<Hi_UpgradeCol>{
                                new Hi_UpgradeCol {
                                    TabFieldAction = TabFieldAction.MODI ,
                                    ColumnInfo=new HiColumn { TabName= "Hi_FieldModel",FieldName="DbName", SortNum=4, FieldLen=50,FieldType=HiType.NVARCHAR,IsBllKey=true,IsPrimary=true,DBDefault=HiTypeDBDefault.VALUE,DefaultValue="",FieldDesc="数据库名"
                                    }
                                }
                            }
                        }
                    }
                });

            lstupgradeinfo.Add(
                new Hi_UpgradeInfo
                {
                    MinVersion = new Version("1.0.0.1"),
                    MaxVersion = new Version("1.0.4.7"),
                    UpgradTabs = new List<Hi_UpgradeTab>
                    {
                        new Hi_UpgradeTab{
                            TabName="Hi_Domain",
                            Columns=new List<Hi_UpgradeCol>{
                                new Hi_UpgradeCol {
                                    TabFieldAction = TabFieldAction.MODI ,
                                    ColumnInfo=new HiColumn { TabName= "Hi_Domain",FieldName="Domain", SortNum=5, FieldLen=20,FieldType=HiType.NVARCHAR,IsBllKey=true,IsPrimary=true,DBDefault=HiTypeDBDefault.VALUE,DefaultValue="",FieldDesc="数据域名"
                                    }
                                }
                            }
                        }
                    }
                });
            lstupgradeinfo.Add(
                new Hi_UpgradeInfo
                {
                    MinVersion = new Version("1.0.0.1"),
                    MaxVersion = new Version("1.0.4.7"),
                    UpgradTabs = new List<Hi_UpgradeTab>
                    {
                        new Hi_UpgradeTab{
                            TabName="Hi_DataElement",
                            Columns=new List<Hi_UpgradeCol>{
                                new Hi_UpgradeCol {
                                    TabFieldAction = TabFieldAction.MODI ,
                                    ColumnInfo=new HiColumn { TabName= "Hi_DataElement",FieldName="Domain", SortNum=5, FieldLen=20,FieldType=HiType.NVARCHAR,IsBllKey=true,IsPrimary=true,DBDefault=HiTypeDBDefault.VALUE,DefaultValue="",FieldDesc="数据域名"
                                    }
                                }
                            }
                        }
                    }
                });

            lstupgradeinfo.Add(
                new Hi_UpgradeInfo
                {
                    MinVersion = new Version("1.0.0.1"),
                    MaxVersion = new Version("1.0.4.7"),
                    UpgradTabs = new List<Hi_UpgradeTab>
                    {
                        new Hi_UpgradeTab{
                            TabName="Hi_Snro",
                            Columns=new List<Hi_UpgradeCol>{
                                new Hi_UpgradeCol {
                                    TabFieldAction = TabFieldAction.MODI ,
                                    ColumnInfo=new HiColumn { TabName= "Hi_Snro",FieldName="SNRO", SortNum=5, FieldLen=50,FieldType=HiType.VARCHAR,IsBllKey=true,IsPrimary=true,DBDefault=HiTypeDBDefault.VALUE,DefaultValue="",FieldDesc="SNRO主编号"
                                    }
                                }
                            }
                        }
                    }
                });
            lstupgradeinfo.Add(
                new Hi_UpgradeInfo
                {
                    MinVersion = new Version("1.0.0.1"),
                    MaxVersion = new Version("1.0.4.7"),
                    UpgradTabs = new List<Hi_UpgradeTab>
                    {
                        new Hi_UpgradeTab{
                            TabName="Hi_FieldModel",
                            Columns=new List<Hi_UpgradeCol>{
                                new Hi_UpgradeCol {
                                    TabFieldAction = TabFieldAction.MODI ,
                                    ColumnInfo=new HiColumn { TabName= "Hi_FieldModel",FieldName="SNO", SortNum=70, FieldLen=50,FieldType=HiType.VARCHAR,DBDefault=HiTypeDBDefault.VALUE,DefaultValue="",FieldDesc="编号名称"
                                    }
                                },
                                new Hi_UpgradeCol {
                                    TabFieldAction = TabFieldAction.MODI ,
                                    ColumnInfo=new HiColumn { TabName= "Hi_FieldModel",FieldName="SNO_NUM", SortNum=75, FieldLen=3,FieldType=HiType.VARCHAR,DBDefault=HiTypeDBDefault.VALUE,DefaultValue="",FieldDesc="子编号"
                                    }
                                }
                            }
                        }
                    }
                });


            string json = Newtonsoft.Json.JsonConvert.SerializeObject(lstupgradeinfo);
            if (json != null)
            {
                List<Hi_UpgradeInfo> upgradeInfo2 =Newtonsoft.Json.JsonConvert.DeserializeObject<List<Hi_UpgradeInfo>>(json);
            }
        }
    }
}
