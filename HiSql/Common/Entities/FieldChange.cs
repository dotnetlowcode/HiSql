using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public class FieldChange
    {
        bool _istabchange = false;

        List<FieldChangeDetail> _changedetail = new List<FieldChangeDetail>();
        public string FieldName { get; set; }

        public TabFieldAction Action { get; set; } 

        /// <summary>
        /// 是否是表结构有变化
        /// </summary>
        public bool IsTabChange { get => _istabchange; set => _istabchange = value; }

        public HiColumn OldColumn { get; set; }
        public HiColumn NewColumn { get; set; }


        /// <summary>
        /// 变更明细
        /// </summary>
        public List<FieldChangeDetail> ChangeDetail { get=> _changedetail; set=> _changedetail=value; }
    }

    /// <summary>
    /// 字段比明细
    /// </summary>
    public class FieldChangeDetail
    { 

        /// <summary>
        /// 属性名称
        /// </summary>
        public string AttrName { get; set; }

        /// <summary>
        /// 需要变更后的值
        /// </summary>
        public string ValueA { get; set; }

        /// <summary>
        /// 当前的值
        /// </summary>
        public string ValueB { get; set; }
    }
}
