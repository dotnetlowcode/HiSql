using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace HiSql
{

    /// <summary>
    /// SqliteDataAdapter 数据填充器
    /// Microsoft.Data.Sqlite.Core 不包含 DbDataAdapter 实现，
    /// 此适配器通过 SqliteDataReader 填充 DataSet，保持与现有 AdoProvider 兼容
    /// </summary>
    public class SqliteDataAdapter : DbDataAdapter
    {
        private SqliteCommand command;
        private string sql;
        private SqliteConnection _sqlConnection;

        /// <summary>
        /// SqlDataAdapter
        /// </summary>
        /// <param name="command"></param>
        public SqliteDataAdapter(SqliteCommand command)
        {
            this.command = command;
        }

        public SqliteDataAdapter()
        {

        }

        /// <summary>
        /// SqlDataAdapter
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="_sqlConnection"></param>
        public SqliteDataAdapter(string sql, SqliteConnection _sqlConnection)
        {
            this.sql = sql;
            this._sqlConnection = _sqlConnection;
        }

        /// <summary>
        /// SelectCommand
        /// </summary>
        public SqliteCommand SelectCommand
        {
            get
            {
                if (this.command == null)
                {
                    this.command = new SqliteCommand(this.sql, this._sqlConnection);
                }
                return this.command;
            }
            set
            {
                this.command = value;
            }
        }

        public override int Fill(DataSet ds)
        {
            if (ds == null)
            {
                ds = new DataSet();
            }
            using (SqliteDataReader dr = command.ExecuteReader())
            {
                do
                {
                    var dt = new DataTable();
                    var columns = dt.Columns;
                    var rows = dt.Rows;
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        string name = dr.GetName(i).Trim();
                        Type _type = dr.GetFieldType(i);

                        if (dr.GetFieldType(i) == typeof(Byte[]) && name == "dflt_value")
                        {
                            _type = typeof(String);
                        }
                        if (!columns.Contains(name))
                            columns.Add(new DataColumn(name, _type));
                        else
                        {
                            columns.Add(new DataColumn(name + i, _type));
                        }
                    }

                    while (dr.Read())
                    {
                        DataRow daRow = dt.NewRow();
                        for (int i = 0; i < columns.Count; i++)
                        {
                            Type _type = dr.GetFieldType(i);
                            if (_type == typeof(Byte[]) && columns[i].ColumnName == "dflt_value")
                            {
                                daRow[columns[i].ColumnName] = dr.GetValue(i)?.ToString();
                            }
                            else
                            {
                                daRow[columns[i].ColumnName] = dr.GetValue(i);

                            }

                        }
                        dt.Rows.Add(daRow);
                    }
                    dt.AcceptChanges();
                    ds.Tables.Add(dt);
                } while (dr.NextResult());
            }

            return ds.Tables.Count;
        }

        public override DataTable[] FillSchema(DataSet dataSet, SchemaType schemaType)
        {
            throw new NotSupportedException("SqliteDataAdapter.FillSchema is not supported. Use Fill instead.");
        }

        public IDataParameter[] GetFillParameters()
        {
            throw new NotSupportedException("SqliteDataAdapter.GetFillParameters is not supported.");
        }

        public int Update(DataSet dataSet)
        {
            throw new NotSupportedException("SqliteDataAdapter.Update is not supported.");
        }
    }
}
