using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql 
{
    /// <summary>
    /// join 语句
    /// </summary>
    public class JoinDefinition
    {
        //左边表
        private TableDefinition _left;

        //连接类型
        private JoinType _jointype;

        //右边表
        private TableDefinition _right;


        private string _schema;

        public string Schema
        {
            get { return _schema; }
            set { _schema = value;
                if (_right != null) _right.Schema = _schema;
            }
        }

        List<JoinOnFilterDefinition> _joinon = new List<JoinOnFilterDefinition>();


        public JoinDefinition(string tabname, string rename)
        {
           
            if (!string.IsNullOrEmpty(tabname) && !string.IsNullOrEmpty(rename))
            {
                _right = new TableDefinition();
                _right.TabName = tabname;
                _right.AsTabName = rename;
                
            }
            else
                throw new Exception($"指定的Join表名[{tabname}]或别名[{rename}]不能为空");
        }
        public JoinDefinition(string tabname)
        {

            if (!string.IsNullOrEmpty(tabname)  )
            {
                _right = new TableDefinition();
                _right.TabName = tabname;
                _right.AsTabName = tabname;

            }
            else
                throw new Exception($"指定的Join表名[{tabname}] 不能为空");
        }
        /// <summary>
        /// 左边表
        /// </summary>
        public TableDefinition Left
        {
            get { return _left; }
            set { _left = value; }
        }

        /// <summary>
        /// 连接类型
        /// </summary>
        public JoinType JoinType
        {
            get { return _jointype; }
            set { _jointype = value; }
        }

        /// <summary>
        /// 右边表
        /// </summary>
        public TableDefinition Right
        {
            get { return _right; }
            set { _right = value; }
        }


        /// <summary>
        /// 关联条件 理论上来讲必须至少有一个
        /// </summary>
        public List<JoinOnFilterDefinition> JoinOn
        {
            get { return _joinon; }
            set { _joinon = value; }
        }

    }
}
