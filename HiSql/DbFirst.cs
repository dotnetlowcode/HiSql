using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public class DbFirst : IDbFirst
    {


        public HiSqlClient SqlClient
        {
            get
            {
                return _sqlClient;
            }
            set { _sqlClient = value; }
        }

        private HiSqlClient _sqlClient;


        public DbFirst(HiSqlClient sqlClient)
        {
            _sqlClient = sqlClient;
        }

        public DbFirst()
        { 
            
        }

        /// <summary>
        /// 向表中新增一字段列
        /// </summary>
        /// <param name="tabname"></param>
        /// <param name="hiColumn"></param>
        /// <param name="opLevel"></param>
        /// <returns></returns>
        public Tuple<bool, string> AddColumn(string tabname, HiColumn hiColumn, OpLevel opLevel)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 向表创建索引
        /// </summary>
        /// <param name="tabname"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public Tuple<bool, string> CreateIndex(string tabname, List<HiColumn> columns)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 向数据库中创建表
        /// </summary>
        /// <param name="tabInfo"></param>
        /// <returns></returns>
        public bool CreateTable(TabInfo tabInfo)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 根据实体类的结构向数据库中创建表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool CreateTable(Type type)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 创建视图
        /// </summary>
        /// <param name="viewname"></param>
        /// <param name="viewsql"></param>
        /// <returns></returns>
        public Tuple<bool, string> CreateView(string viewname, string viewsql)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 对指定中删除某一列的字段
        /// </summary>
        /// <param name="tabname"></param>
        /// <param name="hiColumn"></param>
        /// <param name="opLevel"></param>
        /// <returns></returns>
        public Tuple<bool, string> DelColumn(string tabname, HiColumn hiColumn, OpLevel opLevel)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 删除指定的索引
        /// </summary>
        /// <param name="tabname"></param>
        /// <param name="indexname"></param>
        /// <returns></returns>
        public Tuple<bool, string> DelIndex(string tabname, string indexname)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 永久删除指定的表
        /// </summary>
        /// <param name="tabname"></param>
        /// <param name="nolog"></param>
        /// <returns></returns>
        public bool DropTable(string tabname, bool nolog = false)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 获取所有物理表，视图，全局临时表 清单
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllTables()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 获取所有全局临时表
        /// </summary>
        /// <returns></returns>
        public List<string> GetGlobalTempTables()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取所有存储过程清单
        /// </summary>
        /// <returns></returns>
        public List<string> GetStoredProc()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取指定表的所有索引清单
        /// </summary>
        /// <param name="tabname"></param>
        /// <returns></returns>
        public List<string> GetTabIndex(string tabname)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取指定索引的字段
        /// </summary>
        /// <param name="tabname"></param>
        /// <param name="indexname"></param>
        /// <returns></returns>
        public List<HiColumn> GetTabIndexColumn(string tabname, string indexname)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 获取所有物理表清单
        /// </summary>
        /// <returns></returns>
        public List<string> GetTables()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取表或视图的物理表结构信息
        /// </summary>
        /// <param name="tabname"></param>
        /// <returns></returns>
        public DataTable GetTabPhyInfo(string tabname)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取临时表清单
        /// </summary>
        /// <returns></returns>
        public List<string> GetTempTables()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取所有视图清单
        /// </summary>
        /// <returns></returns>
        public List<string> GetViews()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 修改指定表的指定字段
        /// </summary>
        /// <param name="tabname"></param>
        /// <param name="hiColumn"></param>
        /// <param name="opLevel"></param>
        /// <returns></returns>
        public Tuple<bool, string> ModiColumn(string tabname, HiColumn hiColumn, OpLevel opLevel)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 表不存在则创建，存在则修改
        /// </summary>
        /// <param name="tabInfo"></param>
        /// <param name="opLevel"></param>
        /// <returns></returns>
        public Tuple<bool, string> ModiTable(TabInfo tabInfo, OpLevel opLevel)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 无日志删除表数据
        /// </summary>
        /// <param name="tabname"></param>
        /// <returns></returns>
        public bool Truncate(string tabname)
        {
            throw new NotImplementedException();
        }
    }
}
