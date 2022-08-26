//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Common;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Microsoft.Data.Sqlite
//{

//    /// <summary>
//    /// SqliteDataAdapter 数据填充器
//    /// </summary>
//    public class SqliteDataAdapter : DbDataAdapter
//    {
//        private SqliteCommand command;
//        private string sql;
//        private SqliteConnection _sqlConnection;

//        /// <summary>
//        /// SqlDataAdapter
//        /// </summary>
//        /// <param name="command"></param>
//        public SqliteDataAdapter(SqliteCommand command)
//        {
//            this.command = command;
//        }

//        public SqliteDataAdapter()
//        {

//        }

//        /// <summary>
//        /// SqlDataAdapter
//        /// </summary>
//        /// <param name="sql"></param>
//        /// <param name="_sqlConnection"></param>
//        public SqliteDataAdapter(string sql, SqliteConnection _sqlConnection)
//        {
//            this.sql = sql;
//            this._sqlConnection = _sqlConnection;
//        }

//        /// <summary>
//        /// SelectCommand
//        /// </summary>
//        public SqliteCommand SelectCommand
//        {
//            get
//            {
//                if (this.command == null)
//                {
//                    this.command = new SqliteCommand(this.sql, this._sqlConnection);
//                }
//                return this.command;
//            }
//            set
//            {
//                this.command = value;
//            }
//        }

//        public MissingMappingAction MissingMappingAction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
//        public MissingSchemaAction MissingSchemaAction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

//        public ITableMappingCollection TableMappings => throw new NotImplementedException();




//        public DataTable[] FillSchema(DataSet dataSet, SchemaType schemaType)
//        {

//            throw new NotImplementedException();
//        }

//        public override int Fill(DataSet ds)
//        {
//            if (ds == null)
//            {
//                ds = new DataSet();
//            }
//            using (SqliteDataReader dr = command.ExecuteReader())
//            {
//                do
//                {
//                    var dt = new DataTable();
//                    var columns = dt.Columns;
//                    var rows = dt.Rows;
//                    for (int i = 0; i < dr.FieldCount; i++)
//                    {
//                        string name = dr.GetName(i).Trim();
//                        Type _type = dr.GetFieldType(i);

//                        if (dr.GetFieldType(i) == typeof(Byte[]) && name == "dflt_value")
//                        {
//                            _type = typeof(String);
//                        }
//                        if (!columns.Contains(name))
//                            columns.Add(new DataColumn(name, _type));
//                        else
//                        {
//                            columns.Add(new DataColumn(name + i, _type));
//                        }
//                    }

//                    while (dr.Read())
//                    {
//                        DataRow daRow = dt.NewRow();
//                        for (int i = 0; i < columns.Count; i++)
//                        {
//                            Type _type = dr.GetFieldType(i);
//                            if (_type == typeof(Byte[]) && columns[i].ColumnName =="dflt_value")
//                            {
//                                daRow[columns[i].ColumnName] = dr.GetValue(i)?.ToString();
//                            }
//                            else
//                            {
//                                daRow[columns[i].ColumnName] = dr.GetValue(i);

//                            }

//                        }
//                        dt.Rows.Add(daRow);
//                    }
//                    dt.AcceptChanges();
//                    ds.Tables.Add(dt);
//                } while (dr.NextResult());
//            }

//            return ds.Tables.Count;
//        }

//        public IDataParameter[] GetFillParameters()
//        {
//            throw new NotImplementedException();
//        }

//        public int Update(DataSet dataSet)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
