using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public class JoinOn : IEnumerable<JoinOnFilterDefinition>
    {

        private readonly List<JoinOnFilterDefinition> _elements = new List<JoinOnFilterDefinition>();

        public virtual JoinOn Add(string left, string right)
        {

            _elements.Add(new JoinOnFilterDefinition(left, right));
            return this;
        }

        public virtual JoinOn Add(string joinonstr)
        {
            _elements.Add(new JoinOnFilterDefinition(joinonstr));
            return this;
        }

        public List<JoinOnFilterDefinition> Elements
        {
            get { return _elements; }
        }


        public IEnumerator<JoinOnFilterDefinition> GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _elements.GetEnumerator();
        }
    }
}
