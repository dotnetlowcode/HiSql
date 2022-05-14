using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public class LckInfo
    {
        /// <summary>
        /// 键
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 锁定人
        /// </summary>
        public string UName { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string Ip { get; set; }


        /// <summary>
        /// 动作事件信息
        /// </summary>
        public string EventName { get; set; }

        /// <summary>
        /// 定锁时间
        /// </summary>
        public DateTime LockTime { get; set; }

        /// <summary>
        /// 预计过期时间
        /// </summary>

        public DateTime ExpireTime { get; set; }


        /// <summary>
        /// 续锁次数
        /// </summary>
        public int KeepTimes { get; set; }


        /// <summary>
        /// 加锁备注
        /// </summary>
        public string Descript { get; set; }

    }
}
