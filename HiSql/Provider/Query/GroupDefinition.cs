using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql 
{
    /// <summary>
    /// 分组定义
    /// </summary>
    public class GroupDefinition
    {
        //继承FieldDefinition 够用了
        private FieldDefinition _field = new FieldDefinition();

        public FieldDefinition Field
        {
            get { return _field; }
            set { _field = value; }
        }

        public GroupDefinition(string fieldname)
        {
            Tuple<bool, FieldDefinition> result = Tool.CheckField(fieldname);
            if (result.Item1)
            {

                _field = result.Item2.MoveCross<FieldDefinition>(_field);
            }
            else
            {
                throw new Exception($"GroupBy字段[{fieldname}]不符合语法规则");
            }
        }
    }
}
