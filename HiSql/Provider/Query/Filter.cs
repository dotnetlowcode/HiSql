using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public class Filter : IEnumerable<FilterDefinition>
    {
        private readonly List<FilterDefinition> _elements = new List<FilterDefinition>();


        public List<FilterDefinition> Elements
        {
            get { return _elements; }
            
        }

        public virtual Filter Add(string name, OperType opfileter, object value)
        {
            FilterDefinition _filter = new FilterDefinition();
            _filter.Name = name;
            _filter.OpFilter = opfileter;
            _filter.Value = value;
            Tuple<bool, FieldDefinition> result = Tool.CheckField(name);
            if (result.Item1)
            {
                _elements.Add(_filter);
            }
            else
                throw new Exception($"条件字段[{name}]不符合语法规则");

            
            return this;
        }
        public virtual Filter Add(LogiType logtype, object value)
        {
            _elements.Add(new FilterDefinition() { LogiType = logtype, Value = value });
            return this;
        }
        public IEnumerator<FilterDefinition> GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _elements.GetEnumerator();
        }
    }
}
