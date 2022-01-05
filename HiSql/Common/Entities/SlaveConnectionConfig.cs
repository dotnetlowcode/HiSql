using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// 从库连接配置
    /// </summary>
    public class SlaveConnectionConfig
    {

        private bool isShareThread = false;
        private string configId = string.Empty;

        private string configUid = string.Empty;
        /// <summary>
        /// 权重 ，权重越大 连接命中基率越大
        /// </summary>
        public int Weight = 0;
        public string ConfigId { get { return configId; } set { configId = value; } }


        // <summary>
        /// 用户自定义的连接ID，用于用户自己标识该连接
        /// </summary>
        public string ConfigUid { get { return configUid; } set { configUid = value; } }

        /// <summary>
        /// 是否加密
        /// </summary>
        public bool IsEncrypt = false;

        //数据库类型与主库一致

        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionString { get; set; }


        /// <summary>
        /// 是否共享连接线程 该属性已经过时
        /// </summary>
        ///
        [Obsolete]
        public bool IsShareThread
        {
            get { return isShareThread; }
            set { isShareThread = value; }
        }

    }
}
