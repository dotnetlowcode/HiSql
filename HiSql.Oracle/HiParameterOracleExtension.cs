using System;
using System.Data.OracleClient;

namespace HiSql
{
    public static class HiParameterOracleExtension
    {
        public static OracleParameter ConvertToSqlParameter(this HiParameter hiParameter)
        {
            if (hiParameter != null)
            {
                OracleParameter sqlParameter = new OracleParameter();



                sqlParameter.ParameterName = hiParameter.ParameterName;
                //sqlParameter.UdtTypeName = hiParameter.UdtTypeName;
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
