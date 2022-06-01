using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// 编号配置参数
    /// </summary>
    public class NumberOptions
    {
        bool _multimode = true;


        /// <summary>
        /// 多机模式
        /// </summary>
        public bool MultiMode
        {
            get { return _multimode; }
            set { _multimode = value; }
        }

    }
}
