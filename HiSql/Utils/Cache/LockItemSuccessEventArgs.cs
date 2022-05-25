using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public sealed class LockItemSuccessEventArgs : EventArgs
    {
        public LockItemSuccessEventArgs(string key, LckInfo lckInfo)
        {
            LckInfo = lckInfo;
            Key = key;
        }

        /// <summary>
        /// key
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// 锁信息
        /// </summary>
        public LckInfo LckInfo { get; }
    }
}
