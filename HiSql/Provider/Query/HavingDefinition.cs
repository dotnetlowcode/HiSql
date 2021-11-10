using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql.Provider.Query
{
    public class HavingDefinition
    {
        private FieldDefinition _field = new FieldDefinition();

        public FieldDefinition Field
        {
            get { return _field; }
            set { _field = value; }
        }

        public HavingDefinition(string fieldname)
        {
            Tuple<bool, FieldDefinition> result = Tool.CheckField(fieldname);
            if (result.Item1)
            {

                _field = result.Item2.MoveCross<FieldDefinition>(_field);
            }
            else
            {
                throw new Exception($"Having字段[{fieldname}]不符合语法规则");
            }
        }
    }
}
