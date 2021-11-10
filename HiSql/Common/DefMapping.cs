using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// 数据库的默认值转换
    /// </summary>
    public class DefMapping
    {
        bool _isregex = false;
        string _defdbvalue = string.Empty;
        string _defnewvalue = string.Empty;
        HiTypeDBDefault _hiTypeDBDefault = HiTypeDBDefault.NONE;
        HiTypeGroup _hiTypeGroup = HiTypeGroup.None;
        /// <summary>
        /// 是否通过正则表达式匹配
        /// </summary>
        public bool IsRegex { 
            get=> _isregex;
            set => _isregex = value;
        }

        /// <summary>
        /// 数据库中返回的默认值
        /// </summary>
        public string DbValue
        {
            set => _defdbvalue = value;
            get => _defdbvalue;
        }

        /// <summary>
        /// 当记录新产生时用的默认值
        /// </summary>
        public string DbNewValue
        {
            set => _defnewvalue = value;
            get => _defnewvalue;
        }



        /// <summary>
        /// 对应的默认值类型
        /// </summary>
        public HiTypeDBDefault DBDefault
        {
            get => _hiTypeDBDefault;
            set => _hiTypeDBDefault = value;
        }

        public HiTypeGroup DbType
        {
            set => _hiTypeGroup=value;
            get => _hiTypeGroup;
        }

    }
}
