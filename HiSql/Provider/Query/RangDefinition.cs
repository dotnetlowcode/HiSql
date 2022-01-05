using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql 
{
    /// <summary>
    /// 范围定义
    /// </summary>
    public class RangDefinition
    {
        private object low;
        private object high;

        /// <summary>
        /// 范围低值
        /// </summary>
        public object Low
        {
            get { return low; }
            set { low = value; }
        }

        /// <summary>
        /// 范围高值
        /// </summary>

        public object High
        {
            get { return high; }
            set { high = value; }
        }
    }
}
