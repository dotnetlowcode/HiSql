using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql 
{
    public class ElseDefinition
    {
        string _elsevalue = string.Empty;

        public string ElseValue
        {
            get { return _elsevalue; }
        }
        public ElseDefinition(string expression)
        {
            _elsevalue = expression;
        }
    }
}
