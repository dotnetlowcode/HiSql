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
        bool _multimode = false;
        int _workid = 0;

        SnowType _snowtype = SnowType.IdSnow;

        /// <summary>
        /// 多机模式
        /// </summary>
        public bool MultiMode
        {
            get { return _multimode; }
            set { _multimode = value; }
        }



        /// <summary>
        /// 机器节点ID
        /// 用于雪花ID生成,每台机器的ID要设置为不一样
        /// </summary>
        public int WorkId
        {
            get => _workid;
            set=> _workid = value;
        }

        /// <summary>
        /// 指定雪花ID生成类型
        /// </summary>
        public SnowType SnowType
        {
            get => _snowtype;
            set => _snowtype = value;
        }
    }
}
