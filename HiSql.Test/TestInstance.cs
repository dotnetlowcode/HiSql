using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApp;

namespace HiSql.Test
{
    public class TestInstance
    {
        List<xueDTO> restultBaidu = new List<xueDTO>();
        List<KeyValuePair<string, string>> headerNameList = new List<KeyValuePair<string, string>>();   
        public TestInstance()
        {
            headerNameList.Add(new KeyValuePair<string, string>("areaname", "AAAAA"));
            headerNameList.Add(new KeyValuePair<string, string>("id", "BBBBB"));
            headerNameList.Add(new KeyValuePair<string, string>("name", "CV"));
            headerNameList.Add(new KeyValuePair<string, string>("address", "CCCCC"));
            headerNameList.Add(new KeyValuePair<string, string>("tel", "BBBBB"));
            //headerNameList.Add(new KeyValuePair<string, string>("photos", "照片"));
            headerNameList.Add(new KeyValuePair<string, string>("LB", "CCCCS"));
            headerNameList.Add(new KeyValuePair<string, string>("url", "BB阿斯顿发BB"));

            for (int i = 0; i < 100; i++)
            {
                restultBaidu.Add(new xueDTO()
                {
                    name = "沙发斯蒂芬as多了几分阿里斯柯达就发了考试的法拉第",
                    address = "as砥砺奋进as砥砺奋进阿斯利康的激发螺丝刀发手动阀收到"
                ,
                    id = Guid.NewGuid().ToString(),
                    location = "阿斯加德菲拉斯的减肥啦手动阀阿萨德法可适当",
                    url = "http://www.baidu.com"
                });
            }

            for (int i = 0; i < 1; i++)
            {
                //restultBaidu.AddRange(restultBaidu);
            }
            InitDB();
        }
        private void InitDB()
        {
            var tableName = Sqlite.ExecuteScalar("select name from sqlite_master where type = 'table' and name = 'JGInfo'");
            if (tableName == null || string.IsNullOrWhiteSpace(tableName.ToString()))
            {
                Sqlite.ExecuteNonQuery(@"CREATE TABLE JGInfo  
                (      
                    ID VARCHAR(50),  
                    JGName VARCHAR(1000),  
                    JGID VARCHAR(50),  
                    EXPORTTIME DATETIME,  
                    LB VARCHAR(20),      AREANAME VARCHAR(20),   
                    PRIMARY KEY(ID)  
                ) ");
            }

        }

        public void TestInsertToDB()
        {
            InsertToDB( restultBaidu);
        }
        private int InsertToDB(List<xueDTO> list)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            StringBuilder sbStr = new StringBuilder();
            int i = 1;
            foreach (var item in list)
            {
                sbStr.AppendLine($"INSERT INTO JGInfo VALUES ('{Guid.NewGuid().ToString()}','{item.name}','{item.id}','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}','{item.LB}','{item.areaname}');");
                i++;
                if (i % 50 == 0)
                {
                    Sqlite.ExecuteNonQuery(sbStr.ToString());
                    sbStr.Clear();
                }
            }
            int s = Sqlite.ExecuteNonQuery(sbStr.ToString());
            //MessageBox.Show(timer.ElapsedMilliseconds.ToString());
            return s;
        }
    }

    class xueDTO
    {

        public string address { get; set; }
        public string areaname { get; set; }
        public string cityname { get; set; }
        public string photos { get; set; }

        /// <summary>
        /// 机构名称
        /// </summary>
        public string name { get; set; }
        public string location { get; set; }
        public string tel { get; set; }
        public string phone { get; set; }

        /// <summary>
        /// 机构ID
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 机构类别
        /// </summary>
        public string LB { get; set; } = "GD";


        public string url { set; get; }


    }
}
