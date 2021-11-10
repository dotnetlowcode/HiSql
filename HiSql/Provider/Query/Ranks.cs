using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public class Ranks : IEnumerable<RankDefinition>
    {
        private readonly List<RankDefinition> _elements = new List<RankDefinition>();



        public virtual Ranks Add(DbFunction dbFunction, string field, SortType sortType)
        {
            _elements.Add(new RankDefinition(dbFunction, field, sortType));
            return this;
        }

        public virtual Ranks Add(DbFunction dbFunction, string field)
        {
            _elements.Add(new RankDefinition(dbFunction, field, SortType.ASC));
            return this;
        }

        public List<RankDefinition> Elements
        {
            get { return _elements; }
        }

        public IEnumerator<RankDefinition> GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _elements.GetEnumerator();
        }
    }
}
