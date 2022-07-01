using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using HiSql;
using SqlSugar;

namespace TestProject
{
    class Program
    {
        static void Main(string[] args)
        {



            //插入的记录数设置
            int _count =100000;

            Test.TestSqlServerInsert(_count);

            //Test.TestSqlServer50ColInsert(_count);

           // Test.TestSqlServerBulkCopy(_count);

            //Test.TestSqlServer50ColBulkCopy(_count);


            //Test.TestPosgreSqlInsert(_count);

            //Test.TestPosgreSqlBulkCopy(_count);


            //Test.TestMySqlInsert(_count);
            //Test.TestMySqlBulkCopy(_count);


            //Test.TestOracleInsert(_count);
            //Test.TestOralceBulkCopy(_count);


            var s = Console.ReadLine();
        }


        static List<Dictionary<string, string>> CheckData(List<HiColumn> hiColumns, List<object> lstdata, HiSqlClient sqlClient)
        {
            List<Dictionary<string, string>> rtnlst = new List<Dictionary<string, string>>();
            if (lstdata != null && lstdata.Count > 0)
            {
                Type type = lstdata[0].GetType();
                List<PropertyInfo> attrs = type.GetProperties().Where(p => p.MemberType == MemberTypes.Property && p.CanRead == true).ToList();
                Type _typ_dic = typeof(Dictionary<string, string>);
                Type _typ_dic_obj = typeof(Dictionary<string, object>);

                bool _isdic = type == _typ_dic || type == _typ_dic_obj;

                // 将有配正则校验的列去重统计
                Dictionary<string, HashSet<string>> dic_hash_reg = new Dictionary<string, HashSet<string>>();
                Dictionary<string, HashSet<string>> dic_hash_tab = new Dictionary<string, HashSet<string>>();


                var arrcol_reg = hiColumns.Where(h => !string.IsNullOrEmpty(h.Regex)).ToList();
                var arrcol_tab = hiColumns.Where(h => h.IsRefTab).ToList();

                var arrcol = hiColumns.Where(h => !string.IsNullOrEmpty(h.Regex) || h.IsRefTab).ToList();

                if (arrcol.Count > 0)
                {
                    foreach (HiColumn hi in arrcol)
                    {
                        //if(!string.IsNullOrEmpty(hi.Regex))
                        dic_hash_reg.Add(hi.FieldName, new HashSet<string>());
                    }
                }

                int _rowidx = 0;
                string _value = "";
                if (_isdic)
                {
                    // 表示是字典 Dictionary<string, object>
                    
                    if (type == _typ_dic_obj)
                    {
                        #region Dictionary<string, object>
                        foreach (Dictionary<string, object> _o in lstdata)
                        {
                            _rowidx++;
                            //Dictionary<string, string> _dic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                            Dictionary<string, string> _rowdic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                            foreach (HiColumn hiColumn in hiColumns)
                            {
                                _value = "";
                                #region  判断必填 及自增长
                                if (hiColumn.IsRequire && !hiColumn.IsIdentity && !_o.ContainsKey(hiColumn.FieldName))
                                {
                                    throw new Exception($"行[{_rowidx}] 缺少字段[{hiColumn.FieldName}] 为必填字段");
                                }
                                if (hiColumn.IsIdentity && _o.ContainsKey(hiColumn.FieldName))
                                {
                                    _value = _o[hiColumn.FieldName].ToString();
                                    if (_value == "0" || string.IsNullOrEmpty(_value))
                                        continue;
                                    else
                                        throw new Exception($"行[{_rowidx}] 字段[{hiColumn.FieldName}] 为自增长字段 不需要外部赋值");
                                }
                                else if (hiColumn.IsIdentity)
                                    continue;
                                #endregion

                                if (_o.ContainsKey(hiColumn.FieldName))
                                {
                                    #region 将值转成string 及特殊处理
                                    if (hiColumn.FieldType.IsIn<HiType>(HiType.DATE, HiType.DATETIME))
                                    {
                                        DateTime dtime = DateTime.MinValue;
                                        if (_o[hiColumn.FieldName] != null)
                                        {

                                            dtime = Convert.ToDateTime(_o[hiColumn.FieldName]);

                                        }

                                        if (dtime != null && dtime != DateTime.MinValue)
                                        {
                                            _value = dtime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                        }
                                    }
                                    else if (hiColumn.FieldType.IsIn<HiType>(HiType.BOOL))
                                    {
                                        if ((bool)_o[hiColumn.FieldName] == true)
                                        {
                                            _value = "1";
                                        }
                                        else
                                            _value = "0";
                                    }
                                    else if (_o.ContainsKey(hiColumn.FieldName))
                                    {
                                        _value = _o[hiColumn.FieldName].ToString();
                                    }
                                    else
                                    {
                                        _value = string.Empty;
                                    }
                                    #endregion

                                    #region 是否需要正则校验
                                    if (arrcol.Count > 0 && arrcol.Any(h => h.FieldName == hiColumn.FieldName))
                                    {
                                        dic_hash_reg[hiColumn.FieldName].Add(_value);
                                    }




                                    #endregion

                                    #region 通用数据有效性校验
                                    var result = checkFieldValue(hiColumn, _rowidx, _value, sqlClient);
                                    if (result.Item1)
                                    {
                                        _rowdic.Add(hiColumn.FieldName, result.Item2);
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region 未赋值数据处理
                                    if (!hiColumn.IsNull && hiColumn.DBDefault == HiTypeDBDefault.NONE && !hiColumn.IsIdentity)
                                    {
                                        throw new Exception($"行[{_rowidx}] 字段[{hiColumn.FieldName}]不允许为空数据库中未设置默认值 且插入数据值中未指定值");
                                    }
                                    else if (Constants.IsStandardField(hiColumn.FieldName))
                                    {
                                        var result = checkFieldValue(hiColumn, _rowidx, _value, sqlClient);
                                        if (result.Item1)
                                        {
                                            _rowdic.Add(hiColumn.FieldName, result.Item2);
                                        }
                                    }
                                    #endregion
                                }

                            }

                            if (_rowdic.Count > 0)
                            {
                                rtnlst.Add(_rowdic);
                            }
                            else
                                throw new Exception($"行[{_rowidx}] 无可插入的字段数据");
                        }
                        #endregion
                    }
                    else
                    {
                        #region Dictionary<string, string> 
                        foreach (Dictionary<string, string> _o in lstdata)
                        {
                            _rowidx++;

                            Dictionary<string, string> _rowdic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                            foreach (HiColumn hiColumn in hiColumns)
                            {
                                _value = "";
                                #region  判断必填 及自增长
                                if (hiColumn.IsRequire && !hiColumn.IsIdentity && !_o.ContainsKey(hiColumn.FieldName))
                                {
                                    throw new Exception($"行[{_rowidx}] 缺少字段[{hiColumn.FieldName}] 为必填字段");
                                }
                                if (hiColumn.IsIdentity && _o.ContainsKey(hiColumn.FieldName))
                                {
                                    _value = _o[hiColumn.FieldName].ToString();
                                    if (_value == "0" || string.IsNullOrEmpty(_value))
                                        continue;
                                    else
                                        throw new Exception($"行[{_rowidx}] 字段[{hiColumn.FieldName}] 为自增长字段 不需要外部赋值");
                                }
                                else if (hiColumn.IsIdentity)
                                    continue;
                                #endregion

                                if (_o.ContainsKey(hiColumn.FieldName) || _o.ContainsKey(hiColumn.FieldName.ToLower()))
                                {
                                    _value = _o[hiColumn.FieldName].ToString();

                                    #region 是否需要正则校验
                                    if (arrcol.Any(h => h.FieldName == hiColumn.FieldName))
                                    {
                                        dic_hash_reg[hiColumn.FieldName].Add(_value);
                                    }

                                    #endregion

                                    #region 通用数据有效性校验
                                    var result = checkFieldValue(hiColumn, _rowidx, _value, sqlClient);
                                    if (result.Item1)
                                    {
                                        _rowdic.Add(hiColumn.FieldName, result.Item2);
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region 未赋值数据处理
                                    if (!hiColumn.IsNull && hiColumn.DBDefault == HiTypeDBDefault.NONE && !hiColumn.IsIdentity)
                                    {
                                        throw new Exception($"行[{_rowidx}] 字段[{hiColumn.FieldName}]不允许为空数据库中未设置默认值 且插入数据值中未指定值");
                                    }
                                    else if (Constants.IsStandardField(hiColumn.FieldName))
                                    {
                                        var result = checkFieldValue(hiColumn, _rowidx, _value, sqlClient);
                                        if (result.Item1)
                                        {
                                            _rowdic.Add(hiColumn.FieldName, result.Item2);
                                        }
                                    }
                                    #endregion
                                }

                            }
                            if (_rowdic.Count > 0)
                            {
                                rtnlst.Add(_rowdic);
                            }
                            else
                                throw new Exception($"行[{_rowidx}] 无可插入的字段数据");
                        }
                        #endregion
                    }
                }
                else
                {
                    foreach (var o in lstdata)
                    {
                        _rowidx++;
                        Dictionary<string, string> _dic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        Dictionary<string, string> _rowdic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        foreach (HiColumn hiColumn in hiColumns)
                        {
                            var objprop = attrs.FirstOrDefault(p => p.Name.Equals(hiColumn.FieldName, StringComparison.OrdinalIgnoreCase));
                            #region  判断必填 及自增长
                            if (hiColumn.IsRequire && !hiColumn.IsIdentity && objprop == null)
                            {
                                throw new Exception($"行[{_rowidx}] 缺少字段[{hiColumn.FieldName}] 为必填字段");
                            }
                            if (hiColumn.IsIdentity && _dic.ContainsKey(hiColumn.FieldName))
                            {
                                _value = _dic[hiColumn.FieldName].ToString();
                                if (_value == "0" || string.IsNullOrEmpty(_value))
                                    continue;
                                else
                                    throw new Exception($"行[{_rowidx}] 字段[{hiColumn.FieldName}] 为自增长字段 不需要外部赋值");
                            }
                            else if (hiColumn.IsIdentity)
                            {
                                continue;
                            }
                            #endregion

                            if (objprop != null)
                            {
                                object objvalue = objprop.GetValue(o);
                                if (objvalue != null)
                                {
                                    //#region 将值转成string 及特殊处理
                                    if (hiColumn.FieldType.IsIn<HiType>(HiType.DATE, HiType.DATETIME))
                                    {
                                        DateTime dtime = (DateTime)objvalue;
                                        if (dtime != null && dtime != DateTime.MinValue)
                                        {
                                            _dic.Add(hiColumn.FieldName, dtime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                        }
                                        else
                                            _dic.Add(hiColumn.FieldName, DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                    }
                                    else if (hiColumn.FieldType.IsIn<HiType>(HiType.BOOL))
                                    {
                                        if ((bool)objvalue == true)
                                        {
                                            _dic.Add(hiColumn.FieldName, "1");
                                        }
                                        else
                                            _dic.Add(hiColumn.FieldName, "0");
                                    }
                                    else
                                    {
                                        if (hiColumn.FieldType.IsIn<HiType>(HiType.INT))
                                        {
                                            _dic.Add(hiColumn.FieldName, ((int)objvalue).ToString());
                                        }
                                        else
                                            _dic.Add(hiColumn.FieldName, objvalue.ToString());
                                    }
                                    _value = _dic[hiColumn.FieldName];
                                    //#endregion

                                    #region 是否需要正则校验
                                    if (arrcol.Count > 0 && arrcol.Any(h => h.FieldName == hiColumn.FieldName))
                                    {
                                        dic_hash_reg[hiColumn.FieldName].Add(_dic[hiColumn.FieldName]);
                                    }

                                    #endregion

                                    #region 通用数据有效性校验
                                    var result = checkFieldValue(hiColumn, _rowidx, objvalue.ToString(), sqlClient);
                                    if (result.Item1)
                                    {
                                        _rowdic.Add(hiColumn.FieldName, result.Item2);
                                    }
                                    #endregion


                                }
                                else
                                {
                                    #region 未赋值数据处理
                                    if (!hiColumn.IsNull && hiColumn.DBDefault == HiTypeDBDefault.NONE && !hiColumn.IsIdentity)
                                    {
                                        throw new Exception($"行[{_rowidx}] 字段[{hiColumn.FieldName}]不允许为空数据库中未设置默认值 且插入数据值中未指定值");
                                    }
                                    else if (Constants.IsStandardField(hiColumn.FieldName))
                                    {
                                        var result = checkFieldValue(hiColumn, _rowidx, "", sqlClient);
                                        if (result.Item1)
                                        {
                                            _rowdic.Add(hiColumn.FieldName, result.Item2);
                                        }
                                    }
                                    #endregion
                                }

                            }
                            else
                            {
                                #region 未赋值数据处理
                                if (!hiColumn.IsNull && hiColumn.DBDefault == HiTypeDBDefault.NONE && !hiColumn.IsIdentity)
                                {
                                    throw new Exception($"行[{_rowidx}] 字段[{hiColumn.FieldName}]不允许为空数据库中未设置默认值 且插入数据值中未指定值");
                                }
                                else if (Constants.IsStandardField(hiColumn.FieldName))
                                {
                                    var result = checkFieldValue(hiColumn, _rowidx, "", sqlClient);
                                    if (result.Item1)
                                    {
                                        _rowdic.Add(hiColumn.FieldName, result.Item2);
                                    }
                                }
                                #endregion
                            }
                        }

                        if (_rowdic.Count > 0)
                        {
                            rtnlst.Add(_rowdic);
                        }
                        else
                            throw new Exception($"行[{_rowidx}] 无可插入的字段数据");

                        //foreach (PropertyInfo p1 in attrs)
                        //{
                        //    object value = p1.GetValue(o);
                        //}

                        //List<PropertyInfo> attrs = type.GetProperties().Where(p => p.MemberType == MemberTypes.Property && p.CanRead == true).ToList();

                    }
                }


                #region 正则校验匹配 是否合法
                if (arrcol_reg.Count > 0)
                {
                    foreach (HiColumn hiColumn in arrcol_reg)
                    {
                        Regex _regex = new Regex(hiColumn.Regex, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        if (dic_hash_reg.ContainsKey(hiColumn.FieldName))
                        {
                            foreach (string n in dic_hash_reg[hiColumn.FieldName])
                            {
                                if (!_regex.Match(n).Success)
                                {
                                    throw new Exception($@"列[{hiColumn.FieldName}]值[{n}] 不符合业务配置 {hiColumn.Regex} 要求");
                                }
                            }

                        }
                    }
                }

                //表关联配置校验
                if (arrcol_tab.Count > 0)
                {
                    HiSqlClient _sqlClient = sqlClient.Context.CloneClient();
                    int _total = 0;
                    int _psize = 1000;
                    foreach (HiColumn hiColumn in arrcol_tab)
                    {
                        int _scount = 0;
                        if (dic_hash_reg.ContainsKey(hiColumn.FieldName))
                        {
                            _scount = dic_hash_reg[hiColumn.FieldName].Count;
                            //当源表的条件值大于需要匹配的值时 建议将匹配的值用in的方式传入进行匹配

                            HashSet<string> _hash = new HashSet<string>();
                            DataTable data = null;

                            if (!string.IsNullOrEmpty(hiColumn.RefWhere.Trim()))
                            {
                                data = _sqlClient.Query(hiColumn.RefTab).Field(hiColumn.RefField).Where(hiColumn.RefWhere)
                                .Skip(1).Take(_psize)
                                .ToTable(ref _total);
                            }
                            else
                            {
                                data = _sqlClient.Query(hiColumn.RefTab).Field(hiColumn.RefField)
                                  .Skip(1).Take(_psize)
                                  .ToTable(ref _total);

                            }

                            if (data != null && data.Rows.Count > 0)
                            {

                                if (data.Rows.Count == _psize)
                                {
                                    //源表值比较大
                                    string _sql = dic_hash_reg[hiColumn.FieldName].ToArray().ToSqlIn(true);
                                    if (!string.IsNullOrEmpty(hiColumn.RefWhere.Trim()))
                                    {
                                        //注意这里的语句是非原生sql 是hisql 可以在不同的数据库中编译执行
                                        _sql = $"({hiColumn.RefWhere.Trim()}) and ({_sql})";
                                    }
                                    data = _sqlClient.Query(hiColumn.RefTab).Field(hiColumn.RefField).Where(_sql).ToTable();
                                    _hash = DataConvert.DataTableFieldToHashSet(data, hiColumn.RefField);
                                }
                                else
                                {
                                    //string _sql=DataTableFieldToList(data, hiColumn.RefField).ToArray().ToSqlIn(true);
                                    _hash = DataConvert.DataTableFieldToHashSet(data, hiColumn.RefField);
                                }
                            }
                            dic_hash_reg[hiColumn.FieldName].ExceptWith(_hash);
                            if (dic_hash_reg[hiColumn.FieldName].Count > 0)
                            {
                                throw new Exception($"字段[{hiColumn.FieldName}]配置了表检测 值 [{dic_hash_reg[hiColumn.FieldName].ToArray()[0]}] 在表[{hiColumn.RefTab}]不存在");
                            }

                        }

                    }
                }


                #endregion

            }
            return rtnlst;
        }

        static List<Dictionary<string, string>> CheckAllData(List<HiColumn> hiColumns, List<object> lstdata, HiSqlClient sqlClient)
        {
            List<Dictionary<string, string>> rtnlst = new List<Dictionary<string, string>>();
            if (lstdata != null && lstdata.Count > 0)
            {
                Type type = lstdata[0].GetType();

                Type _typ_dic = typeof(Dictionary<string, string>);
                Type _typ_dic_obj = typeof(Dictionary<string, object>);

                bool _isdic = type == _typ_dic || type == _typ_dic_obj;
                List<PropertyInfo> attrs = type.GetProperties().Where(p => p.MemberType == MemberTypes.Property && p.CanRead == true).ToList();

                //将有配正则校验的列去重统计
                Dictionary<string, HashSet<string>> dic_hash_reg = new Dictionary<string, HashSet<string>>();
                Dictionary<string, HashSet<string>> dic_hash_tab = new Dictionary<string, HashSet<string>>();


                var arrcol_reg = hiColumns.Where(h => !string.IsNullOrEmpty(h.Regex)).ToList();
                var arrcol_tab = hiColumns.Where(h => h.IsRefTab).ToList();

                var arrcol = hiColumns.Where(h => !string.IsNullOrEmpty(h.Regex) || h.IsRefTab).ToList();
                foreach (HiColumn hi in arrcol)
                {
                    //if(!string.IsNullOrEmpty(hi.Regex))
                    dic_hash_reg.Add(hi.FieldName, new HashSet<string>());
                }
                int _rowidx = 0;
                if (_isdic)
                {
                    //表示是字典 Dictionary<string, object>
                    string _value = "";
                    if (type == _typ_dic_obj)
                    {
                        #region Dictionary<string, object>
                        foreach (Dictionary<string, object> _o in lstdata)
                        {
                            _rowidx++;
                            //Dictionary<string, string> _dic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                            Dictionary<string, string> _rowdic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                            foreach (HiColumn hiColumn in hiColumns)
                            {
                                _value = "";
                                #region  判断必填 及自增长
                                if (hiColumn.IsRequire && !hiColumn.IsIdentity && !_o.ContainsKey(hiColumn.FieldName))
                                {
                                    throw new Exception($"行[{_rowidx}] 缺少字段[{hiColumn.FieldName}] 为必填字段");
                                }
                                if (hiColumn.IsIdentity && _o.ContainsKey(hiColumn.FieldName))
                                {
                                    _value = _o[hiColumn.FieldName].ToString();
                                    if (_value == "0" || string.IsNullOrEmpty(_value))
                                        continue;
                                    else
                                        throw new Exception($"行[{_rowidx}] 字段[{hiColumn.FieldName}] 为自增长字段 不需要外部赋值");
                                }
                                else if (hiColumn.IsIdentity)
                                    continue;
                                #endregion

                                if (_o.ContainsKey(hiColumn.FieldName))
                                {
                                    #region 将值转成string 及特殊处理
                                    if (hiColumn.FieldType.IsIn<HiType>(HiType.DATE, HiType.DATETIME))
                                    {
                                        DateTime dtime = DateTime.MinValue;
                                        if (_o[hiColumn.FieldName] != null)
                                        {

                                            dtime = Convert.ToDateTime(_o[hiColumn.FieldName]);

                                        }

                                        if (dtime != null && dtime != DateTime.MinValue)
                                        {
                                            _value = dtime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                        }
                                    }
                                    else if (hiColumn.FieldType.IsIn<HiType>(HiType.BOOL))
                                    {
                                        if ((bool)_o[hiColumn.FieldName] == true)
                                        {
                                            _value = "1";
                                        }
                                        else
                                            _value = "0";
                                    }
                                    else if (_o.ContainsKey(hiColumn.FieldName))
                                    {
                                        _value = _o[hiColumn.FieldName].ToString();
                                    }
                                    else
                                    {
                                        _value = string.Empty;
                                    }
                                    #endregion

                                    #region 是否需要正则校验
                                    if (arrcol.Count > 0 && arrcol.Any(h => h.FieldName == hiColumn.FieldName))
                                    {
                                        dic_hash_reg[hiColumn.FieldName].Add(_value);
                                    }




                                    #endregion

                                    #region 通用数据有效性校验
                                    var result = checkFieldValue(hiColumn, _rowidx, _value, sqlClient);
                                    if (result.Item1)
                                    {
                                        _rowdic.Add(hiColumn.FieldName, result.Item2);
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region 未赋值数据处理
                                    if (!hiColumn.IsNull && hiColumn.DBDefault == HiTypeDBDefault.NONE && !hiColumn.IsIdentity)
                                    {
                                        throw new Exception($"行[{_rowidx}] 字段[{hiColumn.FieldName}]不允许为空数据库中未设置默认值 且插入数据值中未指定值");
                                    }
                                    else if (Constants.IsStandardField(hiColumn.FieldName))
                                    {
                                        var result = checkFieldValue(hiColumn, _rowidx, _value, sqlClient);
                                        if (result.Item1)
                                        {
                                            _rowdic.Add(hiColumn.FieldName, result.Item2);
                                        }
                                    }
                                    #endregion
                                }

                            }

                            if (_rowdic.Count > 0)
                            {
                                rtnlst.Add(_rowdic);
                            }
                            else
                                throw new Exception($"行[{_rowidx}] 无可插入的字段数据");
                        }
                        #endregion
                    }
                    else
                    {
                        #region Dictionary<string, string> 
                        foreach (Dictionary<string, string> _o in lstdata)
                        {
                            _rowidx++;

                            Dictionary<string, string> _rowdic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                            foreach (HiColumn hiColumn in hiColumns)
                            {
                                _value = "";
                                #region  判断必填 及自增长
                                if (hiColumn.IsRequire && !hiColumn.IsIdentity && !_o.ContainsKey(hiColumn.FieldName))
                                {
                                    throw new Exception($"行[{_rowidx}] 缺少字段[{hiColumn.FieldName}] 为必填字段");
                                }
                                if (hiColumn.IsIdentity && _o.ContainsKey(hiColumn.FieldName))
                                {
                                    _value = _o[hiColumn.FieldName].ToString();
                                    if (_value == "0" || string.IsNullOrEmpty(_value))
                                        continue;
                                    else
                                        throw new Exception($"行[{_rowidx}] 字段[{hiColumn.FieldName}] 为自增长字段 不需要外部赋值");
                                }
                                else if (hiColumn.IsIdentity)
                                    continue;
                                #endregion

                                if (_o.ContainsKey(hiColumn.FieldName) || _o.ContainsKey(hiColumn.FieldName.ToLower()))
                                {
                                    _value = _o[hiColumn.FieldName].ToString();

                                    #region 是否需要正则校验
                                    if (arrcol.Any(h => h.FieldName == hiColumn.FieldName))
                                    {
                                        dic_hash_reg[hiColumn.FieldName].Add(_value);
                                    }

                                    #endregion

                                    #region 通用数据有效性校验
                                    var result = checkFieldValue(hiColumn, _rowidx, _value, sqlClient);
                                    if (result.Item1)
                                    {
                                        _rowdic.Add(hiColumn.FieldName, result.Item2);
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region 未赋值数据处理
                                    if (!hiColumn.IsNull && hiColumn.DBDefault == HiTypeDBDefault.NONE && !hiColumn.IsIdentity)
                                    {
                                        throw new Exception($"行[{_rowidx}] 字段[{hiColumn.FieldName}]不允许为空数据库中未设置默认值 且插入数据值中未指定值");
                                    }
                                    else if (Constants.IsStandardField(hiColumn.FieldName))
                                    {
                                        var result = checkFieldValue(hiColumn, _rowidx, _value, sqlClient);
                                        if (result.Item1)
                                        {
                                            _rowdic.Add(hiColumn.FieldName, result.Item2);
                                        }
                                    }
                                    #endregion
                                }

                            }
                            if (_rowdic.Count > 0)
                            {
                                rtnlst.Add(_rowdic);
                            }
                            else
                                throw new Exception($"行[{_rowidx}] 无可插入的字段数据");
                        }
                        #endregion
                    }
                }
                else
                {
                    //非字典

                    #region  非字典
                    string _value = "";
                    foreach (object objdata in lstdata)
                    {
                        _rowidx++;
                        Dictionary<string, string> _dic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        Dictionary<string, string> _rowdic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        foreach (HiColumn hiColumn in hiColumns)
                        {
                            _value = "";
                            var objprop = attrs.FirstOrDefault(p => p.Name.Equals(hiColumn.FieldName.ToLower(), StringComparison.OrdinalIgnoreCase));
                            #region  判断必填 及自增长
                            if (hiColumn.IsRequire && !hiColumn.IsIdentity && objprop == null)
                            {
                                throw new Exception($"行[{_rowidx}] 缺少字段[{hiColumn.FieldName}] 为必填字段");
                            }
                            if (hiColumn.IsIdentity && _dic.ContainsKey(hiColumn.FieldName))
                            {
                                _value = _dic[hiColumn.FieldName].ToString();
                                if (_value == "0" || string.IsNullOrEmpty(_value))
                                    continue;
                                else
                                    throw new Exception($"行[{_rowidx}] 字段[{hiColumn.FieldName}] 为自增长字段 不需要外部赋值");
                            }
                            else if (hiColumn.IsIdentity)
                            {
                                continue;
                            }
                            #endregion

                            if (objprop != null)
                            {
                                object objvalue = objprop.GetValue(objdata);
                                if (objvalue != null)
                                {
                                    #region 将值转成string 及特殊处理
                                    if (hiColumn.FieldType.IsIn<HiType>(HiType.DATE, HiType.DATETIME))
                                    {
                                        DateTime dtime = (DateTime)objvalue;
                                        if (dtime != null && dtime != DateTime.MinValue)
                                        {
                                            _dic.Add(hiColumn.FieldName, dtime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                        }
                                        else
                                            _dic.Add(hiColumn.FieldName, DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                    }
                                    else if (hiColumn.FieldType.IsIn<HiType>(HiType.BOOL))
                                    {
                                        if ((bool)objvalue == true)
                                        {
                                            _dic.Add(hiColumn.FieldName, "1");
                                        }
                                        else
                                            _dic.Add(hiColumn.FieldName, "0");
                                    }
                                    else
                                    {
                                        if (hiColumn.FieldType.IsIn<HiType>(HiType.INT))
                                        {
                                            _dic.Add(hiColumn.FieldName, ((int)objvalue).ToString());
                                        }
                                        else
                                            _dic.Add(hiColumn.FieldName, objvalue.ToString());
                                    }
                                    _value = _dic[hiColumn.FieldName];
                                    #endregion


                                    #region 是否需要正则校验
                                    if (arrcol.Count > 0 && arrcol.Any(h => h.FieldName == hiColumn.FieldName))
                                    {
                                        dic_hash_reg[hiColumn.FieldName].Add(_dic[hiColumn.FieldName]);
                                    }

                                    #endregion

                                    #region 通用数据有效性校验
                                    var result = checkFieldValue(hiColumn, _rowidx, _value, sqlClient);
                                    if (result.Item1)
                                    {
                                        _rowdic.Add(hiColumn.FieldName, result.Item2);
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region 未赋值数据处理
                                    if (!hiColumn.IsNull && hiColumn.DBDefault == HiTypeDBDefault.NONE && !hiColumn.IsIdentity)
                                    {
                                        throw new Exception($"行[{_rowidx}] 字段[{hiColumn.FieldName}]不允许为空数据库中未设置默认值 且插入数据值中未指定值");
                                    }
                                    else if (Constants.IsStandardField(hiColumn.FieldName))
                                    {
                                        var result = checkFieldValue(hiColumn, _rowidx, "", sqlClient);
                                        if (result.Item1)
                                        {
                                            _rowdic.Add(hiColumn.FieldName, result.Item2);
                                        }
                                    }
                                    #endregion
                                }

                            }
                            else
                            {
                                #region 未赋值数据处理
                                if (!hiColumn.IsNull && hiColumn.DBDefault == HiTypeDBDefault.NONE && !hiColumn.IsIdentity)
                                {
                                    throw new Exception($"行[{_rowidx}] 字段[{hiColumn.FieldName}]不允许为空数据库中未设置默认值 且插入数据值中未指定值");
                                }
                                else if (Constants.IsStandardField(hiColumn.FieldName))
                                {
                                    var result = checkFieldValue(hiColumn, _rowidx, "", sqlClient);
                                    if (result.Item1)
                                    {
                                        _rowdic.Add(hiColumn.FieldName, result.Item2);
                                    }
                                }
                                #endregion
                            }


                        }


                        if (_rowdic.Count > 0)
                        {
                            rtnlst.Add(_rowdic);
                        }
                        else
                            throw new Exception($"行[{_rowidx}] 无可插入的字段数据");


                    }

                    #endregion
                }


                #region 正则校验匹配 是否合法
                if (arrcol_reg.Count > 0)
                {
                    foreach (HiColumn hiColumn in arrcol_reg)
                    {
                        Regex _regex = new Regex(hiColumn.Regex, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        if (dic_hash_reg.ContainsKey(hiColumn.FieldName))
                        {
                            foreach (string n in dic_hash_reg[hiColumn.FieldName])
                            {
                                if (!_regex.Match(n).Success)
                                {
                                    throw new Exception($@"列[{hiColumn.FieldName}]值[{n}] 不符合业务配置 {hiColumn.Regex} 要求");
                                }
                            }

                        }
                    }
                }

                //表关联配置校验
                if (arrcol_tab.Count > 0)
                {
                    HiSqlClient _sqlClient = sqlClient.Context.CloneClient();
                    int _total = 0;
                    int _psize = 1000;
                    foreach (HiColumn hiColumn in arrcol_tab)
                    {
                        int _scount = 0;
                        if (dic_hash_reg.ContainsKey(hiColumn.FieldName))
                        {
                            _scount = dic_hash_reg[hiColumn.FieldName].Count;
                            //当源表的条件值大于需要匹配的值时 建议将匹配的值用in的方式传入进行匹配

                            HashSet<string> _hash = new HashSet<string>();
                            DataTable data = null;

                            if (!string.IsNullOrEmpty(hiColumn.RefWhere.Trim()))
                            {
                                data = _sqlClient.Query(hiColumn.RefTab).Field(hiColumn.RefField).Where(hiColumn.RefWhere)
                                .Skip(1).Take(_psize)
                                .ToTable(ref _total);
                            }
                            else
                            {
                                data = _sqlClient.Query(hiColumn.RefTab).Field(hiColumn.RefField)
                                  .Skip(1).Take(_psize)
                                  .ToTable(ref _total);

                            }

                            if (data != null && data.Rows.Count > 0)
                            {

                                if (data.Rows.Count == _psize)
                                {
                                    //源表值比较大
                                    string _sql = dic_hash_reg[hiColumn.FieldName].ToArray().ToSqlIn(true);
                                    if (!string.IsNullOrEmpty(hiColumn.RefWhere.Trim()))
                                    {
                                        //注意这里的语句是非原生sql 是hisql 可以在不同的数据库中编译执行
                                        _sql = $"({hiColumn.RefWhere.Trim()}) and ({_sql})";
                                    }
                                    data = _sqlClient.Query(hiColumn.RefTab).Field(hiColumn.RefField).Where(_sql).ToTable();
                                    _hash = DataConvert.DataTableFieldToHashSet(data, hiColumn.RefField);
                                }
                                else
                                {
                                    //string _sql=DataTableFieldToList(data, hiColumn.RefField).ToArray().ToSqlIn(true);
                                    _hash = DataConvert.DataTableFieldToHashSet(data, hiColumn.RefField);
                                }
                            }
                            dic_hash_reg[hiColumn.FieldName].ExceptWith(_hash);
                            if (dic_hash_reg[hiColumn.FieldName].Count > 0)
                            {
                                throw new Exception($"字段[{hiColumn.FieldName}]配置了表检测 值 [{dic_hash_reg[hiColumn.FieldName].ToArray()[0]}] 在表[{hiColumn.RefTab}]不存在");
                            }

                        }

                    }
                }


                #endregion


            }
            else
                return rtnlst;
            return rtnlst;
        }
        static Tuple<bool, string> checkFieldValue(HiColumn hiColumn, int rowidx, string value, HiSqlClient sqlClient)
        {

            string _value = "";
            Tuple<bool, string> rtn = new Tuple<bool, string>(false, "");


            #region 字典数据

            _value = value;



            if (Constants.IsStandardTimeField(hiColumn.FieldName))
            {
                if (hiColumn.DBDefault != HiTypeDBDefault.FUNDATE)
                {
                    _value = $"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}'";
                    rtn = new Tuple<bool, string>(true, _value);

                }
            }
            else if (Constants.IsStandardUserField(hiColumn.FieldName))
            {
                _value = $"'{sqlClient.Context.CurrentConnectionConfig.User}'";
                rtn = new Tuple<bool, string>(true, _value);
            }
            else
            {
                if (hiColumn.FieldType.IsIn<HiType>(HiType.NVARCHAR, HiType.NCHAR, HiType.GUID))
                {
                    //中文按1个字符计算
                    //_value = "test";
                    //当为max 时hiColumn.FieldLen == -1
                    if (_value.Length > hiColumn.FieldLen && hiColumn.FieldLen > 0)
                    {
                        throw new Exception($"字段[{hiColumn.FieldName}]的值[{_value}]超过了限制长度[{hiColumn.FieldLen}] 无法数据提交");
                    }
                    if (hiColumn.IsRequire)
                    {
                        if (string.IsNullOrEmpty(_value.Trim()))
                            throw new Exception($"字段[{hiColumn.FieldName}] 为必填 无法数据提交");
                    }

                    _value = $"'{_value.ToSqlInject()}'";
                    rtn = new Tuple<bool, string>(true, _value);

                }
                else if (hiColumn.FieldType.IsIn<HiType>(HiType.VARCHAR, HiType.CHAR, HiType.TEXT))
                {
                    //中文按两个字符计算
                    //当为max 时hiColumn.FieldLen == -1
                    //_value = "test";
                    if (_value.LengthZH() > hiColumn.FieldLen && hiColumn.FieldLen > 0)
                    {
                        throw new Exception($"字段[{hiColumn.FieldName}]的值[{_value}]超过了限制长度[{hiColumn.FieldLen}] 无法数据提交");
                    }
                    if (hiColumn.IsRequire)
                    {
                        if (string.IsNullOrEmpty(_value.Trim()))
                            throw new Exception($"字段[{hiColumn.FieldName}] 为必填 无法数据提交");
                    }
                    _value = $"'{_value.ToSqlInject()}'";
                    rtn = new Tuple<bool, string>(true, _value);
                }
                else if (hiColumn.FieldType.IsIn<HiType>(HiType.INT, HiType.BIGINT, HiType.DECIMAL, HiType.SMALLINT))
                {

                    //_value = "1";
                    rtn = new Tuple<bool, string>(true, $"{_value}");
                }
                else if (hiColumn.FieldType.IsIn<HiType>(HiType.BOOL))
                {

                    if (_value == "true" || _value == "1")
                    {
                        if (sqlClient.Context.CurrentConnectionConfig.DbType.IsIn<DBType>(DBType.PostGreSql, DBType.Hana, DBType.MySql))
                        {
                            _value = "True";
                            rtn = new Tuple<bool, string>(true, _value);
                        }
                        else
                        {
                            _value = "1";
                            rtn = new Tuple<bool, string>(true, $"'{_value}'");

                        }
                    }
                    else
                    {
                        if (sqlClient.Context.CurrentConnectionConfig.DbType.IsIn<DBType>(DBType.PostGreSql, DBType.Hana, DBType.MySql))
                        {
                            _value = "False";
                            rtn = new Tuple<bool, string>(true, _value);
                        }
                        else
                        {
                            _value = "0";
                            rtn = new Tuple<bool, string>(true, $"'{_value}'");
                        }
                    }
                }
                else if (hiColumn.FieldType.IsIn<HiType>(HiType.DATE, HiType.DATETIME))
                {
                    if (!string.IsNullOrEmpty(_value))
                    {
                        DateTime dtime = DateTime.Parse(_value);
                        //DateTime dtime = DateTime.Now;
                        if (dtime != null)
                        {
                            rtn = new Tuple<bool, string>(true, $"'{dtime.ToString("yyyy-MM-dd HH:mm:ss.fff")}'");
                        }
                    }
                }
                else
                {
                    rtn = new Tuple<bool, string>(true, $"'{_value}'");
                }
            }


            #endregion

            return rtn;
        }

    }

}
