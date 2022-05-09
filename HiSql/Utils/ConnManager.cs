using System;
using System.Collections.Generic;
using System.Linq;
namespace HiSql
{
    //用于主库的连接管理
    internal   class ConnManager
    {
        /// <summary>
        /// 内部用范围内
        /// </summary>
        private class Rang
        {
            //低值
            public double Low { get; set; }
            //高值
            public double High { get; set; }
            //索引序号位置
            public int Index { get; set; }
        }


        /// <summary>
        /// 根据表来确定是否走主库或从库
        /// </summary>
        /// <param name="slaveconnectionConfig"></param>
        /// <param name="tablename"></param>
        /// <param name="dbMasterSlave"></param>
        /// <returns></returns>
        public static bool ChooseSlaveForTable(ConnectionConfig slaveconnectionConfig, string tablename,DbMasterSlave dbMasterSlave)
        {
            bool _isslave = false;//默认主库
            if (dbMasterSlave == DbMasterSlave.Default)
            {
                //如果配置了从库，则默认使用从库连接，否则还是使用主库连接
                if (slaveconnectionConfig != null)
                {
                    if (slaveconnectionConfig.SlaveOnly != null)
                    {
                        //有指定仅指定表使用从库
                        _isslave = slaveconnectionConfig.SlaveOnly.Where(s => s.ToLower() == tablename.ToLower()).Count() > 0;
                    }
                    else if (slaveconnectionConfig.SlaveExclude != null)
                    {
                        //有指定排除表外都使用从库 如果不在排除中则使用从库 否则主库
                        _isslave = slaveconnectionConfig.SlaveExclude.Where(s => s.ToLower() == tablename.ToLower()).Count() == 0;
                    }
                    else
                        _isslave = true;//走从库连接
                }
                else
                    _isslave = false;
            }
            else if (dbMasterSlave == DbMasterSlave.Master)
            {
                //说明需要强制使用主库，忽略从库
                _isslave = false;
            }
            else
            {
                //其它情况则使用主库
                _isslave = false;
            }

            return _isslave;
        }
        
        /// <summary>
        /// 获取一个从库连接
        /// </summary>
        /// <param name="lstSlave"></param>
        /// <returns></returns>

        public static SlaveConnectionConfig ChooseSlave(List<SlaveConnectionConfig> lstSlave)
        {
            SlaveConnectionConfig slaveConnectionConfig = null;
            //根据数值列表的位置
            if (lstSlave != null && lstSlave.Count == 0)
            {
                return slaveConnectionConfig;
            }
            var _lstSlav = lstSlave?.Where(s => s.Weight > 0).ToList();

            List<Rang> doulst = new List<Rang>();
            double _curr = 0;
            if (_lstSlav.Count > 0)
            {
                var scount = _lstSlav.Sum(s => s.Weight);
                for (int i = 0; i < _lstSlav.Count; i++)
                {
                    Rang rang = new Rang();
                    if (i == 0)
                        rang.Low = (double)0;
                    else
                        rang.Low = _curr;

                    rang.Index = i;

                    _curr = (double)_lstSlav[i].Weight / (double)scount;



                    if (i != _lstSlav.Count - 1)
                    {
                        rang.High = rang.Low + _curr;
                        _curr = rang.High;
                    }
                    else
                        rang.High = (double)1;
                    doulst.Add(rang);
                }
                if (doulst.Count > 0)
                {

                    double d = new Random().NextDouble();
                    foreach (Rang rang in doulst)
                    {
                        if (d >= rang.Low && d < rang.High)
                        {
                            slaveConnectionConfig= _lstSlav[rang.Index];
                            break;
                        }
                    }

                }
            }
            else if (_lstSlav.Count==1)
            {
                slaveConnectionConfig = _lstSlav[0];
            }
            else
                return slaveConnectionConfig;

            if (slaveConnectionConfig != null)
            {
                slaveConnectionConfig.ConfigId = Guid.NewGuid().ToString();
            }

            return slaveConnectionConfig;
        }
    }
}
