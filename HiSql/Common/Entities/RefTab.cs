using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// 引用表
    /// </summary>
    public class RefTab
    {
        string tabname;
        string fieldname;
        string condition;
        public string TabName
        {
            get { return tabname; }
            set { fieldname = value; }
        }

        public string FieldName
        {
            get { return fieldname; }
            set { fieldname = value; }
        }

        public string Condition
        {
            get { return condition; }
            set { condition = value; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            else if (this.GetType() != obj.GetType())
                return false;
            else {
                RefTab refTab = (RefTab)obj;
                if (refTab.TabName.Equals(this.tabname) && refTab.FieldName.Equals(this.fieldname) && refTab.Condition.Equals(this.condition))
                    return true;
                else
                    return false;
            }
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
