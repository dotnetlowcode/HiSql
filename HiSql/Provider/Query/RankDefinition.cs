using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public class RankDefinition
    {
        private string  _field = string.Empty;
      



        private DbFunction _dbfunction = DbFunction.NONE;
        private SortType _sortType = SortType.ASC;


        public SortType SortType
        {
            get { return _sortType; }
            set { _sortType = value; }
        }
        public string Field
        {
            get { return _field; }
            set { _field = value; }
        }

        public RankDefinition(DbFunction dbFunction, string field,  SortType sortType)
        {
            _field = field;
      
            _dbfunction = dbFunction;
            _sortType = sortType;

        }

        public DbFunction DbFunction
        {
            get { return _dbfunction; }
            set { _dbfunction = value; }
        }
    }
}
