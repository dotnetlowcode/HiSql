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
        ICache cache = CacheContext.MCache;
        HiSqlClient SqlClient = null;

        List<string> listsnro = new List<string>();
        internal ICache Cache
        {
            get { return cache; }
            set { cache = value; }
        }


        Dictionary<string, List<string>> _snroKey = new Dictionary<string, List<string>>();



        /// <summary>
        /// 初始加载编号缓存数据
        /// </summary>
        /// <returns></returns> 
        public bool Load(HiSqlClient sqlClient)
        {

            //var snrolist = sqlClient.Query("Hi_Snro").Field("*").ToList<Hi_Snro>();


            var rtnsnro = Lock.LockOnExecute("SNRO", () => { 
            
            
            }, new LckInfo { UName = sqlClient.CurrentConnectionConfig.User, Ip = Tool.Net.GetLocalIPAddress() });

            if (!rtnsnro.Item1)
                throw new Exception($"初始化编号失败:");
            

            SqlClient = sqlClient;
            var snrolist = sqlClient.Query("Hi_Snro").Field("*").ToList<Hi_Snro>();

            if (!System.IO.Directory.Exists(snropath))
                System.IO.Directory.CreateDirectory(snropath);

            

            //有编号文件说明系统是正常停止的
            //如果文件中的编号值大于数据库中的编号值那么优先取文件中的值
            foreach (Hi_Snro snro in snrolist)
            {
                string _key = $"{_prestr}:{snro.SNRO}:{snro.SNUM}";
                Hi_Snro _snro = cache.GetOrCreate<Hi_Snro>(_key, () => {

                    return snro;
                });
                listsnro.Add(_key);
            }
            List<Hi_Snro> lst_snro = GetDiskSnro();

            foreach (string key in listsnro)
            {
                var _key = key.Replace(":", "-");
                if (cache.CacheType == CacheType.MCache)
                {
                    #region 使用的是内存缓存 单机
                    Hi_Snro snro = cache.GetCache<Hi_Snro>(key);
                    
                    lock (snro)
                    {

                        var _snro = lst_snro.Where(s => s.SNRO == snro.SNRO && s.SNUM == snro.SNUM).FirstOrDefault();
                        if (_snro != null)
                        {
                            //数据库中的编号大于文件缓存中的编号大小，有可能是手工中数据库中修改了编号值 那么以数据库中的为准
                            //数据库中的编号小于文件缓存中的编号大小，说明是缓存中的编号还没有达到缓存值大小值 应以文件缓存为准
                            if (snro.CurrNum.Compare(_snro.CurrNum) == -1)
                            {
                                snro.CurrNum = _snro.CurrNum;
                                snro.CurrAllNum = _snro.CurrAllNum;
                                snro.CurrCacheSpace = _snro.CurrCacheSpace;
                            }
                            //用完后删除
                            System.IO.File.Delete($"{snropath}\\{_key}.snro");
                        }
                        else
                        {
                            //在文件中没有 可能是程序非正常退出那么需要跳一下缓存大小的号
                            if (snro.CacheSpace > 0)
                            {
                                _snro = snro;
                                Tuple<bool, string, List<string>> rtn = Create(ref snro, snro.CacheSpace);
                                if (rtn.Item1)
                                {
                                    snro.CurrNum = _snro.CurrNum;
                                    snro.CurrAllNum = _snro.CurrAllNum;
                                    if (_snro.CacheSpace == _snro.CurrCacheSpace)
                                        snro.CurrCacheSpace = 0;
                                    SqlClient.Update("Hi_Snro", snro).Only("CurrNum", "CurrAllNum", "CurrCacheSpace").ExecCommand();
                                }
                            }
                        }
                    }
                    #endregion 
                }
                else if (cache.CacheType == CacheType.RCache)
                {
                    Lock.LockOnExecute(key, () => {
                        Hi_Snro snro = cache.GetCache<Hi_Snro>(key);
                        var _snro = lst_snro.Where(s => s.SNRO == snro.SNRO && s.SNUM == snro.SNUM).FirstOrDefault();
                        if (_snro != null)
                        {
                            //数据库中的编号大于文件缓存中的编号大小，有可能是手工中数据库中修改了编号值 那么以数据库中的为准
                            //数据库中的编号小于文件缓存中的编号大小，说明是缓存中的编号还没有达到缓存值大小值 应以文件缓存为准
                            if (snro.CurrNum.Compare(_snro.CurrNum) == -1)
                            {
                                snro.CurrNum = _snro.CurrNum;
                                snro.CurrAllNum = _snro.CurrAllNum;
                                snro.CurrCacheSpace = _snro.CurrCacheSpace;
                            }
                            //用完后删除
                            System.IO.File.Delete($"{snropath}\\{_key}.snro");
                        }
                        else
                        {
                            //在文件中没有 可能是程序非正常退出那么需要跳一下缓存大小的号
                            if (snro.CacheSpace > 0)
                            {
                                _snro = snro;
                                Tuple<bool, string, List<string>> rtn = Create(ref snro, snro.CacheSpace);
                                if (rtn.Item1)
                                {
                                    snro.CurrNum = _snro.CurrNum;
                                    snro.CurrAllNum = _snro.CurrAllNum;
                                    if (_snro.CacheSpace == _snro.CurrCacheSpace)
                                        snro.CurrCacheSpace = 0;
                                    SqlClient.Update("Hi_Snro", snro).Only("CurrNum", "CurrAllNum", "CurrCacheSpace").ExecCommand();
                                }
                            }
                        }

                    },new LckInfo { UName= sqlClient.CurrentConnectionConfig.User,Ip="127.0.0.1"});
                }
                else
                    throw new Exception($"未能识别的缓存类型:{cache.CacheType}");
            }




            return true;
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
            string _key = $"{_prestr}:{snro}:{snum}";
            

            if (count <= 0)
                throw new Exception($"创建的编号数量不能小于1");

            Hi_Snro _snro = cache.GetCache<Hi_Snro>(_key);
            if (_snro != null)
            {

                lock (_snro)
                {
                    //Thread.Sleep(1000);
                    Tuple<bool, string, List<string>> rtn = Create(ref _snro, count);
                    if (rtn.Item1)
                    {
                        //cache.SetCache(_key, _snro);//理论上来讲这里不需要重写回去


                        if (_snro.CacheSpace == _snro.CurrCacheSpace && SqlClient != null)
                        {
                            _snro.CurrCacheSpace = 0;
                            SqlClient.Update("Hi_Snro", _snro).Only("CurrNum", "CurrAllNum", "CurrCacheSpace").ExecCommand();
                        }


                        if (rtn.Item3.Count==0)
                        {
                            throw new Exception($"编号返回为空{rtn.Item3[0].ToString()}");
                        }

                        return rtn.Item3;
                    }
                    else
                        throw new Exception($"创建失败{rtn.Item2}");
                }


            }
            else
            {
                throw new Exception($"创建失败[{_key}]在编号表不存在");

            };
        }


        /// <summary>
        /// 将当前缓存数据落盘到磁盘
        /// </summary>
        /// <returns></returns>
        public bool SyncDisk()
        {
            //编号缓存目录 如果不存在就删除
            if (!System.IO.Directory.Exists(snropath))
                System.IO.Directory.CreateDirectory(snropath);

            foreach (string key in listsnro)
            {
                Hi_Snro _snro = cache.GetCache<Hi_Snro>(key);
                if (_snro != null)
                {
                    var _key = key;
                    _key = _key.Replace(":", "-");
                    lock (_snro)
                    {
                        Serialize<Hi_Snro>.ToFile(_snro, $"{snropath}\\{_key}.snro");
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 获取落盘的编号缓存
        /// </summary>
        /// <returns></returns>
        public List<Hi_Snro> GetDiskSnro()
        {
            List<Hi_Snro> lst_snro = new List<Hi_Snro>();
            foreach (string key in listsnro)
            {
                var _key = key;
                _key = _key.Replace(":", "-");
                if (System.IO.File.Exists($"{snropath}\\{_key}.snro"))
                    lst_snro.Add(Serialize<Hi_Snro>.GetFile($"{snropath}\\{_key}.snro"));
            }
            return lst_snro;
        }


        private Tuple<bool, string, List<string>> Create(ref Hi_Snro snro, int nums)
        {
            int _maxlen = 20;
            List<string> dic_list = new List<string>();
            Tuple<bool, string, List<string>> rtntuple = new Tuple<bool, string, List<string>>(false, "", dic_list);
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


            if (snro.PreType == PreType.FixY || snro.PreType == PreType.FixYM || snro.PreType == PreType.FixYMD || snro.PreType == PreType.FixYMDH
                || snro.PreType == PreType.FixYMDHm || snro.PreType == PreType.FixYMDHms)
            {
                if (snro.PreChar.Trim() == "")
                {
                    rtntuple = new Tuple<bool, string, List<string>>(false, "前置符未指定或前置类型错误", dic_list);
                    return rtntuple;
                }
            }


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
                        case PreType.FixY:
                            _prestr = snro.PreChar + DateTime.Now.ToString("yyyy");
                            break;
                        case PreType.YM:
                            _prestr = DateTime.Now.ToString("yyyyMM");
                            break;
                        case PreType.FixYM:
                            _prestr = snro.PreChar + DateTime.Now.ToString("yyyyMM");
                            break;
                        case PreType.YMD:
                            _prestr = DateTime.Now.ToString("yyyyMMDD");
                            break;
                        case PreType.FixYMD:
                            _prestr = snro.PreChar + DateTime.Now.ToString("yyyyMMdd");
                            break;
                        case PreType.YMDH:
                            _prestr = DateTime.Now.ToString("yyyyMMDDHH");
                            break;
                        case PreType.FixYMDH:
                            _prestr = snro.PreChar + DateTime.Now.ToString("yyyyMMddHH");
                            break;
                        case PreType.YMDHm:
                            _prestr = DateTime.Now.ToString("yyyyMMDDHHmm");
                            break;
                        case PreType.FixYMDHm:
                            _prestr = snro.PreChar + DateTime.Now.ToString("yyyyMMddHHmm");
                            break;
                        case PreType.YMDHms:
                            _prestr = DateTime.Now.ToString("yyyyMMddHHmmss");
                            break;
                        case PreType.FixYMDHms:
                            _prestr = snro.PreChar + DateTime.Now.ToString("yyyyMMddHHmmss");
                            break;
                        default:

                            break;
                    }
                    if (_prestr.Trim() != snro.PreChar.Trim())
                    {
                        //当前个前置符不一致时 需要重置当前字符

                        snro.CurrNum = snro.StartNum;

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
                snro.CurrAllNum = snro.PreChar.Trim() + snro.PreChar.Trim() + _o_curstr.Trim();



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
