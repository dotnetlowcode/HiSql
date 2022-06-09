using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public class SeriNumber : INumber
    {
        /// <summary>
        /// 根目录
        /// </summary>

        string snropath = $"{Environment.CurrentDirectory}\\Snro";
        string _prestr = "SNRO";
        /// <summary>
        /// 
        /// </summary>
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
        /// 初始加载编号缓存数据
        /// </summary>
        /// <returns></returns> 
        //public bool Load(HiSqlClient sqlClient)
        //{

        //    //var snrolist = sqlClient.Query("Hi_Snro").Field("*").ToList<Hi_Snro>();

        //    SqlClient = sqlClient;
        //    List<Hi_Snro> lst_snro = new List<Hi_Snro>();

        //    //全局锁定编号
        //    var rtnsnro = Lock.LockOnExecute("SNRO", () => {
        //        var snrolist = sqlClient.Query("Hi_Snro").Field("*").ToList<Hi_Snro>();
        //        // 有编号文件说明系统是正常停止的
        //    //如果文件中的编号值大于数据库中的编号值那么优先取文件中的值
        //    foreach (Hi_Snro snro in snrolist)
        //        {
        //            string _key = $"{_prestr}:{snro.SNRO}:{snro.SNUM}";
        //            Hi_Snro _snro = cache.GetOrCreate<Hi_Snro>(_key, () => {

        //                return snro;
        //            });
        //            listsnro.Add(_key);
        //            lst_snro.Add(_snro);
        //        }

        //    }, new LckInfo { UName = sqlClient.CurrentConnectionConfig.User, Ip = Tool.Net.GetLocalIPAddress() });

        //    if (!rtnsnro.Item1)
        //        throw new Exception($"初始化编号失败:");
            

            
            
        //    //if (!System.IO.Directory.Exists(snropath))
        //    //    System.IO.Directory.CreateDirectory(snropath);

            

           
        //    //List<Hi_Snro> lst_snro = GetDiskSnro();

        //    foreach (string key in listsnro)
        //    {
        //        var _key = key.Replace(":", "-");
        //        if (cache.CacheType == CacheType.MCache)
        //        {
        //            #region 使用的是内存缓存 单机
        //            Hi_Snro snro = cache.GetCache<Hi_Snro>(key);
                    
        //            lock (snro)
        //            {

        //                var _snro = lst_snro.Where(s => s.SNRO == snro.SNRO && s.SNUM == snro.SNUM).FirstOrDefault();
        //                if (_snro != null)
        //                {
        //                    //数据库中的编号大于文件缓存中的编号大小，有可能是手工中数据库中修改了编号值 那么以数据库中的为准
        //                    //数据库中的编号小于文件缓存中的编号大小，说明是缓存中的编号还没有达到缓存值大小值 应以文件缓存为准
        //                    if (snro.CurrNum.Compare(_snro.CurrNum) == -1)
        //                    {
        //                        snro.CurrNum = _snro.CurrNum;
        //                        snro.CurrAllNum = _snro.CurrAllNum;
        //                        snro.CurrCacheSpace = _snro.CurrCacheSpace;
        //                    }
        //                    //用完后删除
        //                    System.IO.File.Delete($"{snropath}\\{_key}.snro");
        //                }
        //                else
        //                {
        //                    //在文件中没有 可能是程序非正常退出那么需要跳一下缓存大小的号
        //                    if (snro.CacheSpace > 0)
        //                    {
        //                        _snro = snro;
        //                        Tuple<bool, string, List<string>> rtn = Create(ref snro, snro.CacheSpace);
        //                        if (rtn.Item1)
        //                        {
        //                            snro.CurrNum = _snro.CurrNum;
        //                            snro.CurrAllNum = _snro.CurrAllNum;
        //                            if (_snro.CacheSpace == _snro.CurrCacheSpace)
        //                                snro.CurrCacheSpace = 0;
        //                            SqlClient.Update("Hi_Snro", snro).Only("CurrNum", "CurrAllNum", "CurrCacheSpace").ExecCommand();
        //                        }
        //                    }
        //                }
        //            }
        //            #endregion 
        //        }
        //        else if (cache.CacheType == CacheType.RCache)
        //        {
        //            Lock.LockOnExecute(key, () => {
        //                Hi_Snro snro = cache.GetCache<Hi_Snro>(key);
        //                var _snro = lst_snro.Where(s => s.SNRO == snro.SNRO && s.SNUM == snro.SNUM).FirstOrDefault();
        //                if (_snro != null)
        //                {
        //                    //数据库中的编号大于文件缓存中的编号大小，有可能是手工中数据库中修改了编号值 那么以数据库中的为准
        //                    //数据库中的编号小于文件缓存中的编号大小，说明是缓存中的编号还没有达到缓存值大小值 应以文件缓存为准
        //                    if (snro.CurrNum.Compare(_snro.CurrNum) == -1)
        //                    {
        //                        snro.CurrNum = _snro.CurrNum;
        //                        snro.CurrAllNum = _snro.CurrAllNum;
        //                        snro.CurrCacheSpace = _snro.CurrCacheSpace;
        //                    }
        //                    //用完后删除
        //                    System.IO.File.Delete($"{snropath}\\{_key}.snro");
        //                }
        //                else
        //                {
        //                    //在文件中没有 可能是程序非正常退出那么需要跳一下缓存大小的号
        //                    if (snro.CacheSpace > 0)
        //                    {
        //                        _snro = snro;
        //                        Tuple<bool, string, List<string>> rtn = Create(ref snro, snro.CacheSpace);
        //                        if (rtn.Item1)
        //                        {
        //                            snro.CurrNum = _snro.CurrNum;
        //                            snro.CurrAllNum = _snro.CurrAllNum;
        //                            if (_snro.CacheSpace == _snro.CurrCacheSpace)
        //                                snro.CurrCacheSpace = 0;
        //                            SqlClient.Update("Hi_Snro", snro).Only("CurrNum", "CurrAllNum", "CurrCacheSpace").ExecCommand();
        //                        }
        //                    }
        //                }

        //            },new LckInfo { UName= sqlClient.CurrentConnectionConfig.User,Ip="127.0.0.1"});
        //        }
        //        else
        //            throw new Exception($"未能识别的缓存类型:{cache.CacheType}");
        //    }




        //    return true;
        //}

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
                _snroKey.Add(_key, new object());
            
                //_snro = cache.GetOrCreate<Hi_Snro>(_key, () =>
                //{
                //    Hi_Snro _sn = getCurrSnro(snro, snum);
                //    return _sn;
                //});
                //本地没有缓存

                //非雪花ID
                
            lock (_snroKey[_key])
            {

                if (!_snroNumber.ContainsKey(_key) || _snroNumber[_key].Count < count)
                {
                    
                    if (!_snowId.ContainsKey(_key))
                    {
                        HiSqlClient _sqlClient = SqlClient.Context.CloneClient();
                        var rtn = Lock.LockOnExecute(_key, () =>
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



                        }, new LckInfo { UName = _sqlClient.CurrentConnectionConfig.User, Ip = Tool.Net.GetLocalIPAddress() });

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


        /// <summary>
        /// 将当前缓存数据落盘到磁盘
        /// </summary>
        /// <returns></returns>
        //public bool SyncDisk()
        //{
        //    //编号缓存目录 如果不存在就删除
        //    if (!System.IO.Directory.Exists(snropath))
        //        System.IO.Directory.CreateDirectory(snropath);

        //    foreach (string key in listsnro)
        //    {
        //        Hi_Snro _snro = cache.GetCache<Hi_Snro>(key);
        //        if (_snro != null)
        //        {
        //            var _key = key;
        //            _key = _key.Replace(":", "-");
        //            lock (_snro)
        //            {
        //                Serialize<Hi_Snro>.ToFile(_snro, $"{snropath}\\{_key}.snro");
        //            }
        //        }
        //    }
        //    return true;
        //}

        /// <summary>
        /// 获取落盘的编号缓存
        /// </summary>
        /// <returns></returns>
        //public List<Hi_Snro> GetDiskSnro()
        //{
        //    List<Hi_Snro> lst_snro = new List<Hi_Snro>();
        //    foreach (string key in listsnro)
        //    {
        //        var _key = key;
        //        _key = _key.Replace(":", "-");
        //        if (System.IO.File.Exists($"{snropath}\\{_key}.snro"))
        //            lst_snro.Add(Serialize<Hi_Snro>.GetFile($"{snropath}\\{_key}.snro"));
        //    }
        //    return lst_snro;
        //}


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
            //防止其它线程更新给其加上锁
            //lock (snro)
            //{

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
            //}
            rtntuple = new Tuple<bool, string, List<string>>(true, _o_curstr, dic_list);
            return rtntuple;
        }
    }
}
