using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// 数据操作动作
    /// </summary>
    public enum DBAction
    {
        Insert,//插入
        UPdate,//更新
        Modifiy,// 表中无数据则插入,有数据则更新
        Delete,//删除

        Select,//查询

        ExecSql,//说明是批处理SQL

    }
}
