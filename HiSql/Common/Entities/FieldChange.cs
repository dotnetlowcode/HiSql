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

        public string FieldName { get; set; }

        public TabFieldAction Action { get; set; } 

        /// <summary>
        /// 是否是表结构有变化
        /// </summary>
        public bool IsTabChange { get => _istabchange; set => _istabchange = value; }

        public HiColumn OldColumn { get; set; }
        public HiColumn NewColumn { get; set; }
    }
}
