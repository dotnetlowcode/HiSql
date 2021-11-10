using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public class WhenDefinition
    {

        //字段对象
        FieldDefinition _field=null;

        ThenDefinition _then = null;


        public ThenDefinition Then
        {
            get { return _then; }
            set { _then = value; }
        }


        /// <summary>
        /// 表达式的值
        /// </summary>
        string _value;
        /// <summary>
        /// 默认是等于
        /// </summary>
        OperType operType = OperType.EQ;


        /// <summary>
        /// 字段
        /// </summary>
        public FieldDefinition Field
        {
            get { return _field; }
            set { _field = value; }
        }

        /// <summary>
        /// 值
        /// </summary>
        public string Value
        {
            get { return _value; }
        }
        /// <summary>
        /// 运算符号
        /// </summary>
        public OperType OperSymbol
        {
            get { return operType; }
        }

        /// <summary>
        /// 条件表达式
        /// </summary>
        /// <param name="expression">如 "UserAge>10"</param>
        public WhenDefinition(string expression)
        {

            string _fieldstr = string.Empty;
            //需要通过正则表达式解析出表达式值
            Dictionary<string,string> dic= Tool.RegexGrp(Constants.REG_FIELDEXPRESSION, expression);
            if (dic.Count > 0)
            {
                switch (dic["oper"].Trim())
                {
                    case "=":
                        operType = OperType.EQ;
                        break;
                    case ">":
                        operType = OperType.GT;
                        break;
                    case "<":
                        operType = OperType.LT;
                        break;
                    case "!=":
                    case "<>":
                        operType = OperType.NE;
                        break;
                    case ">=":
                        operType = OperType.GE;
                        break;
                    case "<=":
                        operType = OperType.LE;
                        break;
                    default:
                        operType = OperType.EQ;
                        break;
                }

                _value = dic["value"].ToString();

                if (!string.IsNullOrEmpty(dic["field"].Trim()))
                {
                    //说明有指定的字段


                    //表标识前辍
                    if (!string.IsNullOrEmpty(dic["flag"]))
                    {
                        _fieldstr = dic["flag"];
                    }
                    if (!string.IsNullOrEmpty(dic["tab"].Trim()))
                    {
                        _fieldstr = $"{_fieldstr}.{dic["tab"].Trim()}.";
                    }

                    _fieldstr = $"{_fieldstr}{dic["field"].Trim()}";


                    _field = new FieldDefinition(_fieldstr);
                }
                else
                { 
                    //只有值没有指定字段
                    //应该默认为 Case的字段 等于当前值
                }



            }
            else
                throw new Exception($"[When]方法的参数表达式$[{expression}]语法错误");
        }
    }
}
