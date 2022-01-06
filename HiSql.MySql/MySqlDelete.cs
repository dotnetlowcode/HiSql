using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public class MySqlDelete : DeleteProvider
    {
        Dictionary<string, TabInfo> dictabinfo = new Dictionary<string, TabInfo>();

        StringBuilder sb = new StringBuilder();

    
        public override string ToSql()
        {

            dictabinfo = new Dictionary<string, TabInfo>();
            sb = new StringBuilder();
            checkData();
            return sb.ToString();
        }
        #region 私有方法

        void checkData()
        {
            //SqlServerDM sqldm = null;
            TabInfo tabinfo;
            string sql_where = string.Empty;
            if (this.Table != null)
            {
                //sqldm = Instance.CreateInstance<SqlServerDM>($"{Constants.NameSpace}.{this.Context.CurrentConnectionConfig.DbType.ToString()}{DbInterFace.DM.ToString()}");
                tabinfo = Context.DMInitalize.GetTabStruct(this.Table.TabName);
                //sqldm.Context = this.Context;
                //tabinfo = sqldm.GetTabStruct(this.Table.TabName);
                dictabinfo.Add(this.Table.TabName, tabinfo);
            }
            else
                throw new Exception("未指定要更新的表");

            if (this.Data.Count > 0)
            {
                List<string> _field = new List<string>();
                Type type = this.Data[0].GetType();
                bool _isdic = type == typeof(Dictionary<string, string>);

                if (this.Filters != null && this.Filters.IsHiSqlWhere && !string.IsNullOrEmpty(this.Filters.HiSqlWhere.Trim()))
                {
                    throw new Exception($"已经指定了按指定数据集合删除就不能再指定Where条件删除");
                    //sql_where=Context.DMTab.BuilderWhereSql(new List<TableDefinition> { this.Table }, dictabinfo, null, this.Filters.WhereParse.Result, false);
                }
                else if (this.Wheres.Count > 0)
                {
                    sql_where = Context.DMTab.BuilderWhereSql(new List<TableDefinition> { this.Table }, dictabinfo, null, this.Wheres, false);
                }
                else
                {
                    foreach (object obj in this.Data)
                    {
                        Dictionary<string, string> result = this.CheckData(_isdic,this.Table, obj, Context.DMInitalize, tabinfo);
                        if (result.Count > 0)
                        {
                            sb.AppendLine(Context.DMTab.BuildDeleteSql(this.Table, result, sql_where, false));
                        }
                    }
                }
            }
            else
            {
                if (this.Filters != null && this.Filters.IsHiSqlWhere && !string.IsNullOrEmpty(this.Filters.HiSqlWhere.Trim()))
                {
                    sql_where = Context.DMTab.BuilderWhereSql(new List<TableDefinition> { this.Table }, dictabinfo, null, this.Filters.WhereParse.Result, false);
                }
                else if (this.Wheres.Count > 0)
                {
                    sql_where = Context.DMTab.BuilderWhereSql(new List<TableDefinition> { this.Table }, dictabinfo, null, this.Wheres, false);
                }
                sb.AppendLine(Context.DMTab.BuildDeleteSql(this.Table, new Dictionary<string, string>(), sql_where.ToString(), this.IsTruncate, this.IsDrop));
            }
        }
        #endregion
    }
}
