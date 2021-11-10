using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// 表的存储方式
    /// </summary>
    public enum TabStoreType
    {
        Row=0,//行存储
        Column=1 //列存储  目前只有HANA具备此功能
    }
}
