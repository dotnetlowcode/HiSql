using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql.HanaUnitTest
{
    class Demo_Init
    {

        public static HanaBulkCopy GetNativeClient()
        {
            HanaConnection hdbconn = new HanaConnection("DRIVER=HDBODBC;UID=HONEBI;PWD=tt@2021;SERVERNODE =192.168.1.1:32015;DATABASENAME =HBI");
          
            hdbconn.Open();
            HanaBulkCopy mybuild = new HanaBulkCopy(hdbconn);
            return mybuild;
        }

        public static DataTable BuildBlukCopy(DataTable dt, DataTable targetStruct, string tabname)
        {
            DataTable dt_copy = new DataTable();
            foreach (DataColumn dc in dt.Columns)
            {
                DataRow _drow = targetStruct.Select($"COLUMN_NAME=\'{dc.ColumnName}\' ").FirstOrDefault();
                if (_drow != null)
                {

                    if (dc.DataType.ToString().ToLower().IndexOf("bool") >= 0)
                    {
                        DataColumn _newdc = new DataColumn();
                        _newdc.DataType = typeof(Int16);
                        _newdc.ColumnName = dc.ColumnName;
                        dt_copy.Columns.Add(_newdc);
                    }
                    else if (dc.DataType.ToString().ToLower().IndexOf("byte") >= 0)
                    {
                        DataColumn _newdc = new DataColumn();
                        _newdc.DataType = typeof(string);
                        _newdc.ColumnName = dc.ColumnName;
                        dt_copy.Columns.Add(_newdc);
                    }
                    else
                    {
                        DataColumn _newdc = new DataColumn();
                        _newdc.DataType = dc.DataType;
                        _newdc.ColumnName = dc.ColumnName;
                        dt_copy.Columns.Add(_newdc);
                    }

                }
            }

            foreach (DataRow drow in dt.Rows)
            {
                DataRow _ndrow = dt_copy.NewRow();

                string _value = "";

                foreach (DataColumn _dc in dt_copy.Columns)
                {
                    _value = "";
                    DataRow _drow = targetStruct.Select($"COLUMN_NAME=\'{_dc.ColumnName}\' ").FirstOrDefault();
                    try
                    {
                        if (dt.Columns[_dc.ColumnName].DataType.ToString().ToLower().IndexOf("date") >= 0)
                        {


                            if (_drow["DATA_TYPE_NAME"].ToString().Trim() == "TIMESTAMP")
                            {

                                if (drow[_dc.ColumnName].ToString().Trim() != "")
                                    _value = Convert.ToDateTime(drow[_dc.ColumnName].ToString()).ToString("yyyy-MM-dd HH:mm:ss.fff");
                                else
                                    _value = "1900-01-01 01:01:01.000";

                            }
                            else if (_drow["DATA_TYPE_NAME"].ToString().Trim() == "SECONDDATE")
                            {
                                if (drow[_dc.ColumnName].ToString().Trim() != "")
                                    _value = Convert.ToDateTime(drow[_dc.ColumnName].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                                else
                                    _value = "1900-01-01 01:01:01";
                            }
                            else if (_drow["DATA_TYPE_NAME"].ToString().Trim() == "DATE")
                            {
                                if (drow[_dc.ColumnName].ToString().Trim() != "")
                                    _value = Convert.ToDateTime(drow[_dc.ColumnName].ToString()).ToString("yyyy-MM-dd");
                                else
                                    _value = "1900-01-01";
                            }
                            else
                            {
                                _value = drow[_dc.ColumnName].ToString();

                                //throw new Exception($"源类型[{dc.DataType.ToString()}] 转到目标类型[{_drow["DATA_TYPE_NAME"].ToString()}]不支持");
                            }
                            _ndrow[_dc.ColumnName] = Convert.ToDateTime(_value);
                        }
                        else if (dt.Columns[_dc.ColumnName].DataType.ToString().ToLower().IndexOf("bool") >= 0)
                        {
                            if (drow[_dc.ColumnName].ToString().ToLower() == "true")
                                _value = "1";
                            else
                                _value = "0";


                            _ndrow[_dc.ColumnName] = Convert.ToInt16(_value);
                        }
                        else if (dt.Columns[_dc.ColumnName].DataType.ToString().ToLower().IndexOf("int16") >= 0)
                        {
                            _value = drow[_dc.ColumnName].ToString().Trim() != "" ? drow[_dc.ColumnName].ToString() : "0";

                            _ndrow[_dc.ColumnName] = Convert.ToInt16(_value);
                        }
                        else if (dt.Columns[_dc.ColumnName].DataType.ToString().ToLower().IndexOf("int32") >= 0)
                        {
                            _value = drow[_dc.ColumnName].ToString().Trim() != "" ? drow[_dc.ColumnName].ToString() : "0";

                            _ndrow[_dc.ColumnName] = Convert.ToInt32(_value);
                        }
                        else if (dt.Columns[_dc.ColumnName].DataType.ToString().ToLower().IndexOf("int64") >= 0)
                        {
                            _value = drow[_dc.ColumnName].ToString().Trim() != "" ? drow[_dc.ColumnName].ToString() : "0";

                            _ndrow[_dc.ColumnName] = Convert.ToUInt64(_value);
                        }
                        else if (dt.Columns[_dc.ColumnName].DataType.ToString().ToLower().IndexOf("decimal") >= 0)
                        {
                            _value = drow[_dc.ColumnName].ToString().Trim() != "" ? drow[_dc.ColumnName].ToString() : "0";

                            _ndrow[_dc.ColumnName] = Convert.ToDecimal(_value);
                        }
                        else if (dt.Columns[_dc.ColumnName].DataType.ToString().ToLower().IndexOf("byte") >= 0)
                        {
                            _value = "";
                            _ndrow[_dc.ColumnName] = _value;
                        }
                        else
                        {
                            //防止特殊字符导致sql语法错误
                            _value = drow[_dc.ColumnName].ToString().Trim().Replace("\'", "\'\'").Replace("\"", "\"\"");
                            _ndrow[_dc.ColumnName] = _value;

                        }
                        //_ndrow[_dc.ColumnName] = _value;
                    }
                    catch (Exception E)
                    {
                        throw new Exception($"表[{dt.TableName}] 列[{_dc.ColumnName}] 值[{drow[_dc.ColumnName].ToString()}] 报:{E.Message.ToString()}");
                    }
                }
                dt_copy.Rows.Add(_ndrow);


            }

            return dt_copy;
        }
        public static HiSqlClient GetSqlClient()
        {
            HiSqlClient sqlclient = new HiSqlClient(
                     new ConnectionConfig()
                     {
                         DbType = DBType.Hana,
                         DbServer = "local-HoneBI",
                         //ConnectionString = "server=192.168.1.90,8433;uid=sa;pwd=Hone@123;database=HoneBI",
                         
                         ConnectionString = "DRIVER=HDBODBC;UID=SAPHANADB;PWD=Hone@crd@2019;SERVERNODE =192.168.10.243:31013;DATABASENAME =QAS",//; MultipleActiveResultSets = true;
                         //ConnectionString = "DRIVER=HDBODBC;UID=HONEBI;PWD=Hone@2021;SERVERNODE =192.168.10.96:32015;DATABASENAME =HBI",//; MultipleActiveResultSets = true;
                         Schema = "SAPHANADB",
                         IsEncrypt = true,
                         IsAutoClose = false,
                         SqlExecTimeOut = 60000,

                         AppEvents = new AopEvent()
                         {
                             OnDbDecryptEvent = (connstr) =>
                             {
                                 //解密连接字段
                                 //Console.WriteLine($"数据库连接:{connstr}");

                                 return connstr;
                             },
                             OnLogSqlExecuting = (sql, param) =>
                             {
                                 //sql执行前 日志记录 (异步)

                                 //Console.WriteLine($"sql执行前记录{sql} time:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")}");
                             },
                             OnLogSqlExecuted = (sql, param) =>
                             {
                                 //sql执行后 日志记录 (异步)
                                 //Console.WriteLine($"sql执行后记录{sql} time:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")}");
                             },
                             OnSqlError = (sqlEx) =>
                             {
                                 //sql执行错误后 日志记录 (异步)
                                 Console.WriteLine(sqlEx.Message.ToString());
                             },
                             OnTimeOut = (int timer) =>
                             {
                                 //Console.WriteLine($"执行SQL语句超过[{timer.ToString()}]毫秒...");
                             },
                             OnPageExec=(int PageCount,int CurrPage)=>
                             {
                                 ///当批量执行并进行分包执行后 可以接收分页执行情况
                                 Console.WriteLine($"共[{PageCount}]页,当前第[{CurrPage}]页");

                             }
                         }
                     }
                     );

            return sqlclient;
        }
    }
}
