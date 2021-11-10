using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql 
{
    /// <summary>
    /// 排序定义
    /// </summary>
    public class SortByDefinition
    {
        private SortType sortType ;//默认为降序
        private FieldDefinition _field = new FieldDefinition();

        
        public FieldDefinition Field
        {
            get { return _field; }
            set { _field = value; }
        }
        
        public SortByDefinition(string fieldname, SortType sortType) 
        {
            Tuple<bool, FieldDefinition> result=Tool.CheckField(fieldname);
            if (result.Item1)
            {
                this.sortType = sortType;
                _field = result.Item2.MoveCross<FieldDefinition>(_field);
            }
            else
            {
                throw new Exception($"排序字段[{fieldname}]不符合语法规则");
            }
        }
        /// <summary>
        /// 是否升序
        /// </summary>
        public bool IsAsc
        {
            get { return sortType==SortType.ASC; }
            set {

                if (value)
                    sortType = SortType.ASC;//升序
                else
                    sortType = SortType.DESC;//降
            }
        }

        /// <summary>
        /// 是否降序
        /// </summary>
        public bool IsDesc
        {
            get { return   sortType == SortType.DESC;  }
            set {
                if (!value)
                    sortType = SortType.ASC;//升序
                else
                    sortType = SortType.DESC;//降
            }
        }

    }
}
