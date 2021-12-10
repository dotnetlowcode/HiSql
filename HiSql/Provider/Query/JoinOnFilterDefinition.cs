using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// join on 
    /// </summary>
    public class JoinOnFilterDefinition
    {
        //左边表
        private FieldDefinition _left=new FieldDefinition ();

        //连接类型
        private OperType _opertype;

        //右边表
        private FieldDefinition _right= new FieldDefinition();


        /// <summary>
        /// 左边表
        /// </summary>
        public FieldDefinition Left
        {
            get { return _left; }
            set { _left = value; }
        }

        public JoinOnFilterDefinition(string left, string right)
        {
            Tuple<bool, FieldDefinition> leftresult= Tool.CheckField(left);
            Tuple<bool, FieldDefinition> rightresult = Tool.CheckField(right);
            if (leftresult.Item1)
            {
                _left=leftresult.Item2.MoveCross<FieldDefinition>(_left);
                
            }
            else
            {
                throw new Exception($"关联条件[{left}]不符合语法规则");
            }

            if (rightresult.Item1)
            {
                _right = rightresult.Item2.MoveCross<FieldDefinition>(_right);
            }
            else
                throw new Exception($"关联条件[{right}]不符合语法规则");


        }
        public JoinOnFilterDefinition(string joinonstr)
        {
            Tuple<bool, FieldDefinition, FieldDefinition> result=Tool.CheckOnField(joinonstr);
            if (result.Item1)
            {
                _left = result.Item2.MoveCross<FieldDefinition>(_left);
                _right = result.Item3.MoveCross<FieldDefinition>(_right);
            }
            else
                throw new Exception($"关联条件[{joinonstr}]不符合语法规则");
        }

        /// <summary>
        /// 连接类型
        /// </summary>
        public OperType OnType
        {
            get { return _opertype; }
            set { _opertype = value; }
        }

        /// <summary>
        /// 右边表
        /// </summary>
        public FieldDefinition Right
        {
            get { return _right; }
            set { _right = value; }
        }
    }
}
