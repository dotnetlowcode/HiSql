using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.Data.Sqlite;

namespace HiSql
{

    public static class HiParameterSQLiteExtension
    {


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hiParameter"></param>
        /// <returns></returns>
        public static SqliteParameter ConvertToSqlParameter(this HiParameter hiParameter)
        {
            if (hiParameter != null)
            {
                SqliteParameter sqlParameter = new SqliteParameter();
                sqlParameter.ParameterName = hiParameter.ParameterName;
                sqlParameter.Size = hiParameter.Size;
                sqlParameter.Value = hiParameter.Value;
                sqlParameter.DbType = hiParameter.DbType;

                if (sqlParameter.Value != null && sqlParameter.Value != DBNull.Value
                    && sqlParameter.DbType == System.Data.DbType.DateTime
                    )
                {
                    var date = Convert.ToDateTime(sqlParameter.Value);
                    if (date == DateTime.MinValue)
                    {
                        sqlParameter.Value = Convert.ToDateTime("1972/01/01");
                    }
                }


                sqlParameter.Direction = hiParameter.Direction;

                return sqlParameter;
            }
            else
                return null;
        }

    }
}
