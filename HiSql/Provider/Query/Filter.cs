using HiSql.AST;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public class Filter : IEnumerable<FilterDefinition>
    {
        private readonly List<FilterDefinition> _elements = new List<FilterDefinition>();

        bool _ishisqlwhere = false;
        string _hisqlwhere = "";

        List<string> _leftbrack = new List<string>();
        List<string> _rightbrack = new List<string>();
        WhereParse whereParse = null;
        /// <summary>
        /// 是否是hisql中间语言sql
        /// </summary>
        public bool IsHiSqlWhere
        {
            get { return _ishisqlwhere; }
        }

        /// <summary>
        /// 如果有(,) 是否是成对出现的，如果没有配置(,)则返回true
        /// 如不成对返回false  说明语法有问题
        /// </summary>
        public bool IsBracketOk
        {
            get {
                return _leftbrack.Count == _rightbrack.Count;
            }
        }

        /// <summary>
        /// 指定hisql中间语言的sql
        /// </summary>
        public string HiSqlWhere
        {
            get { return _hisqlwhere; }
            set { _hisqlwhere = value;
                _hisqlwhere = _hisqlwhere.Replace(System.Environment.NewLine, " ");
                _ishisqlwhere = true; }
        }

        public List<FilterDefinition> Elements
        {
            get { return _elements; }
            
        }

        /// <summary>
        /// hisql中间语法解析结果
        /// </summary>
        public WhereParse WhereParse
        {
            get { return whereParse; } 
        }
        /// <summary>
        /// 添加一个子查询条件
        /// </summary>
        /// <param name="name">字段名称</param>
        /// <param name="opfileter">运算符类型 如>,<,= 等</param>
        /// <param name="value">可以是具体的值 也可以是一个子查询</param>
        /// <returns></returns>
        public virtual Filter Add(string name, OperType opfileter, object value)
        {
            FilterDefinition _filter = new FilterDefinition(FilterType.CONDITION);
            _filter.Name = name;
            _filter.OpFilter = opfileter;
            _filter.Value = value;
            Tuple<bool, FieldDefinition> result = Tool.CheckField(name);
            if (result.Item1)
            {
                _elements.Add(_filter);
            }
            else
                throw new Exception($"条件字段[{name}]不符合语法规则");

            
            return this;
        }

        /// <summary>
        /// 可以填( 或 )  一定要成对出现,
        /// 或一个完整的hisql 条件语法
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public virtual Filter Add(string filter)
        {
            FilterDefinition _filter = null;
            if (Tool.RegexMatch(Constants.REG_BRACKET_LEFT, filter))
            {
                _filter = new FilterDefinition(FilterType.BRACKET_LEFT);
                _leftbrack.Add(filter);
                _elements.Add(_filter);
            }
            else if (Tool.RegexMatch(Constants.REG_BRACKET_RIGHT, filter))
            {
                _filter = new FilterDefinition(FilterType.BRACKET_RIGHT);
                _rightbrack.Add(filter);
                _elements.Add(_filter);
            }
            else
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                {
                    _ishisqlwhere = true;
                    _hisqlwhere = filter;
                    //分析hisql 中间语言的where语句
                    whereParse = new WhereParse(_hisqlwhere);

                }
                else
                {
                    throw new Exception($"如果有指定条件但不能为空[{filter}]");
                }
            }
            return this;
        }

        /// <summary>
        /// 逻辑操作标识 and, or
        /// </summary>
        /// <param name="logiType"></param>
        /// <returns></returns>
        public virtual Filter Add(LogiType logiType)
        {

            FilterDefinition _filter = new FilterDefinition(FilterType.LOGI) { LogiType=logiType};
            _elements.Add(_filter);
            return this;
        }


        public virtual Filter Add(LogiType logtype, object value)
        {
            _elements.Add(new FilterDefinition(FilterType.CONDITION) { LogiType = logtype, Value = value });
            return this;
        }
        public IEnumerator<FilterDefinition> GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _elements.GetEnumerator();
        }
    }
}
