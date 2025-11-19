using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// 表结构 缓存配置参数
    /// </summary>
    public class TabStructOptions
    {
        /// <summary>
        /// 表结构缓存是否强制使用内存缓存
        /// </summary>
        bool _tabstructcacheforcemcache = false;

        /// <summary>
        ///  表结构缓存是否强制使用内存缓存
        ///  注意：如果启用该选项，则不使用redis缓存，默认是不会开启该选项的
        /// </summary>
        public bool TabStructCacheForceMCache
        {
            get { return _tabstructcacheforcemcache; }
            set { _tabstructcacheforcemcache = value; }
        }
    }
}
