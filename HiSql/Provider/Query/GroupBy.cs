using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public class GroupBy : IEnumerable<GroupDefinition>
    {
        private readonly List<GroupDefinition> _elements = new List<GroupDefinition>();

        public IEnumerator<GroupDefinition> GetEnumerator()
        {
            return GetEnumerator();
        }

        public virtual void Add(string fieldname)
        {
            _elements.Add(new GroupDefinition(fieldname));
        }

        public virtual void Add(GroupDefinition groupDefinition)
        {
            _elements.Add(groupDefinition);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        public List<GroupDefinition> Elements
        {
            get { return _elements; }
        }
    }

}
