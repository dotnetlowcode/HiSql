using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// HiSql 排序集
    /// </summary>
    public class SortBy : IEnumerable<SortByDefinition>
    {

        private readonly List<SortByDefinition> _elements = new List<SortByDefinition>();
        

        public virtual void Add(string fieldname, SortType sorttype)
        {
            _elements.Add(new SortByDefinition(fieldname, sorttype));
        }
        public virtual void Add(string fieldname)
        {
            _elements.Add(new SortByDefinition(fieldname, SortType.ASC));//默认升序
        }
        public IEnumerator<SortByDefinition>  GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _elements.GetEnumerator();
        }
        public List<SortByDefinition> Elements
        {
            get { return _elements; }
        }
    }
}
