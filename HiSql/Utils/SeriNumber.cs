using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public class SeriNumber : INumber
    {

        ICache cache = null;
        HiSqlClient SqlClient = null;

        List<string> listsnro = new List<string>();
        internal ICache Cache
        {
            get { return cache; }
            set { cache = value; }
        }

        Dictionary<string, List<string>> _snroNumber = new Dictionary<string, List<string>>();
        Dictionary<string, object> _snroKey = new Dictionary<string, object>();

        private static object _snroKeyLock = new object();

        Dictionary<string, IdGenerate> _snowId = new Dictionary<string, IdGenerate>();
        public SeriNumber(HiSqlClient sqlClient)
        {
            SqlClient = sqlClient.Context.CloneClient();
            if (Global.NumberOptions.MultiMode)
            {
                if (!Global.RedisOn)
                    throw new Exception($"开启了编号多机模式,请启动Redis缓存");
            }
            cache=new MCache("SNRO", null);
        }


        /// <summary>
        /// 产生一个新的编号数据
        /// </summary>
        /// <param name="snro"></param>
        /// <param name="snum"></param>
        /// <returns></returns>
        public string NewNumber(string snro, int snum)
        {
            List<string> lst = NewNumber(snro, snum, 1);
            if (lst.Count > 0) return lst[0];
            else return "";
        }


        /// <summary>
        /// 产生指定数量的编号数据
        /// </summary>
        /// <param name="snro"></param>
        /// <param name="snum"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<string> NewNumber(string snro, int snum, int count)
        {
            string _key = $"{snro}:{snum}";
  
            List<string> lstnumber = new List<string>();
            if (count <= 0)
                throw new Exception($"创建的编号数量不能小于1");

            if (!Global.SnroOn)
                throw new Exception($"未启用编号服务");


            if (!_snroKey.ContainsKey(_key))
            {
                lock (_snroKeyLock)
                {
                    if (!_snroKey.ContainsKey(_key))
                    {
                        _snroKey.Add(_key, new object());
                    }
                }
            }
              

            lock (_snroKey[_key])
            {
                if (!_snroNumber.ContainsKey(_key) || _snroNumber[_key].Count < count)
                {
                    if (!_snowId.ContainsKey(_key))
                    {
                        HiSqlClient _sqlClient = SqlClient.Context.CloneClient();
                        var rtn = Lock.LockOnExecuteNoWait(_key, () =>
                        {
                            Hi_Snro _snro = null;
                            _sqlClient.BeginTran();

                            _snro = _sqlClient.HiSql($"select * from {Constants.HiSysTable["Hi_Snro"].ToString()} where SNRO='{snro.ToSqlInject()}' and SNUM='{snum.ToString()}'").ToList<Hi_Snro>().FirstOrDefault();
                            if (_snro == null)
                            {
                                throw new Exception($"编号规则{snro}-{snum}不存在");
                            }
                            if (!_snro.IsSnow)
                            {
                                //预先创建
                                Tuple<bool, string, List<string>> rtn = Create(ref _snro, count > _snro.CacheSpace ? count : _snro.CacheSpace);
                                if (rtn.Item1)
                                {
                                    _snro.CurrCacheSpace = 0;
                                    _sqlClient.Update("Hi_Snro", _snro).Only("CurrNum","PreChar", "CurrAllNum", "CurrCacheSpace").ExecCommand();
                                    _sqlClient.CommitTran();
                                    if (!_snroNumber.ContainsKey(_key))
                                        _snroNumber.Add(_key, rtn.Item3);
                                    else
                                    {
                                        foreach (string n in rtn.Item3)
                                        {
                                            if (!_snroNumber[_key].Any(_n => _n.Equals(n)))
                                                _snroNumber[_key].Add(n);
                                            else
                                                throw new Exception($"编号:{snro}-{snum} 生成了重复的号码:{n} 请检查编号配置");
                                        }
                                    }
                                }
                                else
                                {
                                    _sqlClient.RollBackTran();
                                    throw new Exception($"编号:{snro}-{snum}创建失败:{rtn.Item2}");
                                }
                            }
                            else
                            {
                                //雪花ID不需要更新表
                                _sqlClient.RollBackTran();
                               
                                IdGenerate idGenerate = null;
                                if (Global.NumberOptions.SnowType == SnowType.IdWorker)
                                    idGenerate = new IdWorker(Global.NumberOptions.WorkId, _snro.SnowTick);
                                else if (Global.NumberOptions.SnowType == SnowType.IdSnow)
                                    idGenerate = new IdSnow(Global.NumberOptions.WorkId, _snro.SnowTick);
                                else
                                    throw new Exception($"未能识别的雪ID生成引擎:{Global.NumberOptions.SnowType.ToString()}");

                                _snowId.Add(_key, idGenerate);
                                

                                List<long> ids = new List<long>();


                                ids = _snowId[_key].NextId(count);
                                if (ids.Count == 0)
                                    throw new Exception($"编号:{snro}-{snum} 创建雪花ID失败");

                                foreach (long id in ids)
                                {
                                    lstnumber.Add(id.ToString()); ;
                                }
                            }
                        }, new LckInfo { UName = _sqlClient.CurrentConnectionConfig.User });

                        if (!rtn.Item1)
                            throw new Exception($"编号:{snro}-{snum} 错误:{rtn.Item2} ");
                    }
                    else {
                        List<long> ids = new List<long>();


                        ids = _snowId[_key].NextId(count);
                        if (ids.Count == 0)
                            throw new Exception($"编号:{snro}-{snum} 创建雪花ID失败");

                        foreach (long id in ids)
                        {
                            lstnumber.Add(id.ToString()); ;
                        }
                    }
                }
                if (lstnumber.Count == 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        lstnumber.Add(_snroNumber[_key][0]);
                        _snroNumber[_key].RemoveAt(0);
                    }
                }

            }
            return lstnumber;
        }


        ///创建生成编号 
        private Tuple<bool, string, List<string>> Create(ref Hi_Snro snro, int nums)
        {
            int _maxlen = 20;
            List<string> dic_list = new List<string>();
            Tuple<bool, string, List<string>> rtntuple = new Tuple<bool, string, List<string>>(false, "", dic_list);

            if (snro.IsSnow)
                throw new Exception($"雪花ID编号类型不允许此规则");

            if (snro.SNRO.Trim() == "")
            {
                rtntuple = new Tuple<bool, string, List<string>>(false, "编号错误", dic_list);
                return rtntuple;
            }

            if (snro.StartNum.Trim() == "")
            {
                rtntuple = new Tuple<bool, string, List<string>>(false, "没有开始编号字符", dic_list);
                return rtntuple;
            }

            if (snro.EndNum.Trim() == "")
            {
                rtntuple = new Tuple<bool, string, List<string>>(false, "没有结束编号字符", dic_list);
                return rtntuple;
            }

            //大于等于1 小于等于30 个字符
            if (snro.Length <= 0 || snro.Length > _maxlen)
            {
                rtntuple = new Tuple<bool, string, List<string>>(false, "编号字符长度未指定或指定过大", dic_list);
                return rtntuple;
            }
            if (snro.Length != snro.StartNum.Trim().Length)
            {
                rtntuple = new Tuple<bool, string, List<string>>(false, "编号指定长度与开始字符长度不一致", dic_list);
                return rtntuple;
            }

            if (snro.PreType.IsIn(PreType.None, PreType.Y, PreType.Y2, PreType.Y2M, PreType.Y2MD, PreType.Y2MDH, PreType.Y2MDHm, PreType.Y2MDHms
                , PreType.YM, PreType.YMD, PreType.YMDH, PreType.YMDHm, PreType.YMDHms))
            {

            }
            else
                throw new Exception($"未能识别的编号类型[{snro.PreType.ToString()}]");


       


            int curchar;
            bool isexit = false;
            string _o_curstr = "";
            string _prestr = "";


            for (int j = 0; j < nums; j++) //创建多少个编号
            {
                if (snro.IsHasPre)//带前置符
                {
                    switch (snro.PreType)
                    {
                        case PreType.None:
                            _prestr = "";
                            break;
                        case PreType.Y:
                            _prestr = DateTime.Now.ToString("yyyy");
                            break;
                        case PreType.Y2:
                            _prestr = DateTime.Now.ToString("yy");
                            break;
                      
                        case PreType.YM:
                            _prestr = DateTime.Now.ToString("yyyyMM");
                            break;
                        case PreType.Y2M:
                            _prestr = DateTime.Now.ToString("yyMM");
                            break;
                        
                        case PreType.YMD:
                            _prestr = DateTime.Now.ToString("yyyyMMdd");
                            break;
                        case PreType.Y2MD:
                            _prestr = DateTime.Now.ToString("yyMMdd");
                            break;
                        
                     
                        case PreType.YMDH:
                            _prestr = DateTime.Now.ToString("yyyyMMddHH");
                            break;
                        case PreType.Y2MDH:
                            _prestr = DateTime.Now.ToString("yyMMddHH");
                            break;
                        
                        case PreType.YMDHm:
                            _prestr = DateTime.Now.ToString("yyyyMMddHHmm");
                            break;
                        case PreType.Y2MDHm:
                            _prestr = DateTime.Now.ToString("yyMMddHHmm");
                            break;
                       
                        case PreType.YMDHms:
                            _prestr = DateTime.Now.ToString("yyyyMMddHHmmss");
                            break;
                        case PreType.Y2MDHms:
                            _prestr = DateTime.Now.ToString("yyMMddHHmmss");
                            break;
                        
                        default:

                            break;
                    }
                    if (string.IsNullOrEmpty(snro.PreChar) || _prestr.Trim() != snro.PreChar.Trim())
                    {
                        //当前个前置符不一致时 需要重置当前字符

                        snro.CurrNum = snro.StartNum;

                        snro.PreChar = _prestr;

                    }
                    else
                    {
                        if (snro.IsHasPre)
                        {
                            if (snro.CurrNum == snro.StartNum && _prestr.Trim() == snro.PreChar.Trim())
                            {
                                rtntuple = new Tuple<bool, string, List<string>>(false, "当前编号已经到顶无法再进行编号", dic_list);
                                return rtntuple;

                            }
                        }
                        else
                        {
                            if (snro.CurrNum == snro.StartNum)
                            {
                                rtntuple = new Tuple<bool, string, List<string>>(false, "当前编号已经到顶无法再进行编号", dic_list);
                                return rtntuple;
                            }

                        }

                    }
                    snro.PreChar = _prestr;
                }

                curchar = 0;
                if (snro.CurrNum.Trim() == "")
                    snro.CurrNum = snro.StartNum;
                char[] arr_charr = snro.CurrNum.ToCharArray();//



                if (snro.IsNumber)//数字编号
                {
                    isexit = false;
                    for (int i = arr_charr.Length - 1; i >= 0; i--)
                    {
                        curchar = (int)arr_charr[i];
                        if (!isexit)
                        {
                            if (curchar == 57)
                            {
                                curchar = 48;
                                //isexit = true;//处理完成退出
                            }

                            else
                            {
                                curchar++;
                                isexit = true;
                            }
                        }
                        _o_curstr = (char)curchar + _o_curstr;
                    }
                }
                else //数字及字母混合编号
                {
                    isexit = false;
                    for (int i = arr_charr.Length - 1; i >= 0; i--)
                    {
                        curchar = (int)arr_charr[i];
                        if (!isexit)
                        {
                            if (curchar == 57)
                            {
                                curchar = 65;
                                isexit = true;//处理完成退出
                            }
                            else if (curchar == 90)
                            {
                                curchar = 48;//要向前进位
                            }
                            else
                            {
                                curchar++;
                                isexit = true;
                            }
                        }
                        _o_curstr = (char)curchar + _o_curstr;
                    }
                }
                snro.CurrNum = _o_curstr.Trim();
                //snro.PreStr = snro.PreChar.Trim() + _prestr.Trim();
                snro.CurrAllNum = string.IsNullOrEmpty(snro.FixPreChar)?  snro.PreChar.Trim() + _o_curstr.Trim(): snro.FixPreChar.Trim() + snro.PreChar.Trim() + _o_curstr.Trim();



                snro.CurrCacheSpace++;//
                dic_list.Add(snro.CurrAllNum.Trim());
                _o_curstr = "";
                curchar = 0;
                isexit = false;
                _o_curstr = "";
                _prestr = "";

            }
            rtntuple = new Tuple<bool, string, List<string>>(true, _o_curstr, dic_list);
            return rtntuple;
        }
    }
}
