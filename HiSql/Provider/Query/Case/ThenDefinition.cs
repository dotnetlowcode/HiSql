using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public class ThenDefinition
    {
        string _thenvalue = string.Empty;

        public string ThenValue
        {
            get { return _thenvalue; }
        }

        public ThenDefinition(string expression)
        {
            _thenvalue = expression;
        }
    }
}
