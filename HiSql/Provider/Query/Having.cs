using HiSql.AST;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// having条件
    /// </summary>
    public class Having : IEnumerable<HavingDefinition>
    {
        private readonly List<HavingDefinition> _elements = new List<HavingDefinition>();



        AST.HavingParse havingParse = null;

        public List<HavingDefinition> Elements
        {
            get { return _elements; }

        }
        public AST.HavingParse HavingParse {
            get { return havingParse; }
        }
        /// <summary>
        /// 字符串的having条件
        /// </summary>
        /// <param name="havingstr"></param>
        public Having(string havingstr)
        {
            //解析字符串条件
            havingParse = new AST.HavingParse(havingstr);

            //解析 出 HavingDefinition
            var havingResult = havingParse.Result;
            LogiType logitype = LogiType.AND;
            foreach (HavingResult whereResult in havingResult)
            {
                if (whereResult.SType == StatementType.FieldValue)
                {
                    HavingDefinition havingDefinition = new HavingDefinition(whereResult.Result["left"].ToString(), whereResult.Result["op"].ToString().GetOperType(), logitype, whereResult.Result["value"]);
                    _elements.Add(havingDefinition);
                    logitype = LogiType.AND;
                }
                else if (whereResult.SType == StatementType.Symbol)
                {
                    logitype = whereResult.Result["mode"].ToString().GetLogiType();
                }
            }

        }
        public Having()
        { 
            
        }

        public virtual Having Add(string fieldname, OperType opertype, object value)
        {
            HavingDefinition havingDefinition = new HavingDefinition(fieldname, opertype, value);
            _elements.Add(havingDefinition);

            return this;
        }

        public virtual Having Add(string fieldname, OperType opertype, LogiType logitype, object value)
        {
            HavingDefinition havingDefinition = new HavingDefinition(fieldname, opertype, logitype, value);
            _elements.Add(havingDefinition);
            return this;
        }

        public IEnumerator<HavingDefinition> GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _elements.GetEnumerator();
        }
    }
}
