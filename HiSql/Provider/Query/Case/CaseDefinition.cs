using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// 查询语句中的 Case when then else endas 语句
    /// </summary>
    public class CaseDefinition
    {
        FieldDefinition _casefield;

        ElseDefinition _else;

        EndAsDefinition _endas;

        List<WhenDefinition> _lstwhen=new List<WhenDefinition> ();



        public List<WhenDefinition> WhenList
        {
            get { return _lstwhen; }
            set { _lstwhen = value; }
        }
        

        public EndAsDefinition EndAs
        {
            get { return _endas; }
            set { _endas = value; }
        }

        /// <summary>
        /// 当条件不符合时就执行此终极条件
        /// </summary>
        public ElseDefinition Else
        {
            get { return _else; }
            set { _else = value; }
        }

        public FieldDefinition CaseField
        {
            get { return _casefield; }
        }

        public CaseDefinition(string fieldname)
        {
            _casefield = new FieldDefinition(fieldname);
        }

        
    }
}
