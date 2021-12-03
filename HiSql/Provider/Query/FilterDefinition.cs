using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// 查询过滤条件定义
    /// </summary>
    public class FilterDefinition //:IEnumerable<FiledDefinition>
    {
        private string _name;
        private OperType _opfilter;
        private LogiType _logitype;

        private FieldDefinition _field = new FieldDefinition();
        private object _value;
        /// <summary>
        /// 过滤条件的节点类型
        /// </summary>
        private FilterType _filtertype;
        //private readonly List<FiledDefinition> _elements = new List<FiledDefinition>();
        public string Name
        {
            get { return _name; }
            set { _name = value;

                Tuple<bool, FieldDefinition> fieldresult = Tool.CheckField(_name);
                if (fieldresult.Item1)
                {
                    _field = fieldresult.Item2.MoveCross<FieldDefinition>(_field);
                }
                else
                    throw new Exception($"过滤条件[{_name}]不符合语法规则");

            }
        }
        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }

        /// <summary>
        /// 操作运行符 如=,>,< 等
        /// </summary>
        public OperType OpFilter
        {
            get { return _opfilter; }
            set { _opfilter = value; }
        }

        public FilterType FilterType
        {
            get { return _filtertype; }
        }
        public FieldDefinition Field
        {
            get { return _field; }
            set { _field = value; }
        }

        /// <summary>
        /// 逻辑运算 and or
        /// </summary>
        public LogiType LogiType
        {
            get { return _logitype; }
            set { _logitype = value; }
        }
        public FilterDefinition(FilterType filterType)
        {
            this._filtertype = filterType;
        }

    }
}
