using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public class EndAsDefinition
    {

        string _fieldname;
        Type _type;



        public string AsFieldName
        {
            get { return _fieldname; }
        }

        public EndAsDefinition(string fieldname, Type type)
        {
            _fieldname = fieldname;
            _type = type;
        }
        public EndAsDefinition(string fieldname)
        {
            _fieldname = fieldname;
            _type = typeof(string) ;
        }

    }
}
