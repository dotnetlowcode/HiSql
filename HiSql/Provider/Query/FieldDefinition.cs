using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql 
{
    /// <summary>
    /// 字段定义
    /// </summary>
    public class FieldDefinition:TableDefinition
    {

        private bool _isfun = false;
        //add by tgm date:202110.11
        private bool _iscasefield = false;//是否复杂的case when 语句生成的字段


        private DbFunction _dbfunction=DbFunction.NONE;
        /// <summary>
        /// 不与实体表相关联的字段 当为True时不与表字段进行校验
        /// </summary>
        private bool _is_virtualfeild = false;

        private string _fieldname;
        private string _asfieldname;

        private CaseDefinition _casedefinition;


        /// <summary>
        /// 未解析的语法字段
        /// </summary>
        private string _sqlname;


        
        /// <summary>
        /// 字段名
        /// </summary>
        public string FieldName
        {
            get { return _fieldname; }
            set { _fieldname = value; }
        }

        /// <summary>
        /// 是否虚拟字段(不与表字段关联)
        /// </summary>
        public bool IsVirtualFeild
        {
            get {
                //如果是复杂条件字段 则默认就是虚拟字段不能参考与实体表结构相关的检验
                if (_iscasefield)
                    return !_iscasefield;
                else
                return _is_virtualfeild; 
            }
            set { _is_virtualfeild = value; }
        }


        /// <summary>
        /// 是否复杂的Case条件字段
        /// </summary>
        public bool IsCaseField
        {
            get { return _iscasefield; }
            set { _iscasefield = value; }
        }
        public bool IsFun {
            get { return _isfun; }
            set { _isfun = value; }
        }
        public DbFunction DbFun
        {
            get { return _dbfunction; }
            set { _dbfunction = value; }
        }

        public CaseDefinition Case
        {
            get { return _casedefinition; }
            set { 
                _casedefinition=value;
                _iscasefield = true;
            
            }
        }

        public FieldDefinition(string fieldname, string rename)
        {
            if (Tool.CheckFieldName(fieldname).Item1 && Tool.CheckFieldName(rename).Item1)
            {
                _asfieldname = rename;
                _fieldname = fieldname;
            }
            else
                throw new Exception($"字段[{fieldname}]或别名[{rename}]不符合语法规则");
        }
        public FieldDefinition(string fieldname,bool isnoas=false)
        {
            //CheckQueryNoAsField
            Tuple<bool, FieldDefinition> result = new Tuple<bool, FieldDefinition>(false, null);
            if(!isnoas)
                result = Tool.CheckQueryField(fieldname);
            else
                result = Tool.CheckQueryNoAsField(fieldname);
            if (result.Item1)
            {
                AsTabName = result.Item2.AsTabName;
                TabName = result.Item2.TabName;


                FieldName = result.Item2.FieldName;
                SqlName = result.Item2.SqlName;
                AsFieldName = result.Item2.AsFieldName;
                IsFun = result.Item2.IsFun;
                DbFun = result.Item2.DbFun;

            }
            else
            {
                if (!isnoas)
                    throw new Exception($"字段[{fieldname}]不符合语法规则 如果是聚合字段一定要用 as 重命名");
                else
                    throw new Exception($"字段[{fieldname}]不符合语法规则");
            }
        }
        public FieldDefinition()
        { 
            
        }

        /// <summary>
        /// 字段别名
        /// </summary>
        public string AsFieldName
        {
            get {
                if (string.IsNullOrEmpty(_asfieldname))
                    return _fieldname;
                else
                    return _asfieldname; 
            
            
            }
            set { _asfieldname = value; }
        }

        /// <summary>
        ///HISQL语法 字段（未解析的）
        /// </summary>
        public string SqlName
        {
            get {
                return _sqlname;
            }
            set { _sqlname = value; }
        }

    }
}
