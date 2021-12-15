using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// having 条件表达式，条中要以带聚合函数
    /// </summary>
    public class HavingDefinition
    {
        private FieldDefinition _field = new FieldDefinition();

        /// <summary>
        /// 操作符类型
        /// </summary>
        private OperType _opertype = OperType.EQ;
       
        //逻辑操作符
        private LogiType _logitype=LogiType.AND;

        private HavingType _havingtype;
        private object _value;
        public FieldDefinition Field
        {
            get { return _field; }
            set { _field = value; }
        }

        /// <summary>
        /// 定义Having 条件
        /// </summary>
        /// <param name="fieldname">字段或聚合函数</param>
        /// <param name="opertype">操作类型=，> 等</param>
        /// <param name="value">值</param>
        public HavingDefinition(string fieldname, OperType opertype,object value)
        {

            _field = new FieldDefinition(fieldname);
            _opertype = opertype;
            _havingtype = HavingType.CONDITION;
            _value = value;
            if (_field.FieldName.ToLower().Trim() != _field.AsFieldName.ToLower().Trim())
            {
                throw new Exception($"having 中的字段不允许用别名");
            }

        }

        /// <summary>
        /// 定义Having 条件
        /// </summary>
        /// <param name="fieldname">字段或聚合函数</param>
        /// <param name="opertype">操作类型=，> 等</param>
        /// <param name="logitype">逻辑操作符</param>
        /// <param name="value">值</param>
        public HavingDefinition(string fieldname, OperType opertype, LogiType logitype, object value)
        {

            _field = new FieldDefinition(fieldname);
            _opertype = opertype;
            _havingtype = HavingType.CONDITION;
            _logitype = logitype;
            _value = value;
            if (_field.FieldName.ToLower().Trim() != _field.AsFieldName.ToLower().Trim())
            {
                throw new Exception($"having 中的字段不允许用别名");
            }

        }

        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }
        /// <summary>
        /// 操作运行符 如=,>,< 等
        /// </summary>
        public OperType OperType
        {
            get { return _opertype; }
            set { _opertype = value; }
        }
        /// <summary>
        /// 逻辑运算 and or
        /// </summary>
        public LogiType LogiType
        {
            get { return _logitype; }
            set { _logitype = value; }
        }
        public HavingType HavingType
        {
            get { return _havingtype; }
        }
    }
}
