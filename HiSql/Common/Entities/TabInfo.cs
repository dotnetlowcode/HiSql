using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    [Serializable]
    /// <summary>
    /// 表结构实体信息
    /// </summary>
    public class TabInfo
    {

        private List<HiColumn> _columns = new List<HiColumn>();
        private string _tabName;
        /// <summary>
        /// 实体名称
        /// </summary>
        public string EntityName { get; set; }

        /// <summary>
        /// 表名称
        /// </summary>
        public string DbTabName
        {
            get
            {
                if (_tabName == null)
                    return EntityName;
                else
                    return _tabName;
            }
            set
            {
                _tabName = value;
            }
        }
        /// <summary>
        /// 表模型 
        /// </summary>
        public HiTable TabModel
        {
            get; set;
        }
        /// <summary>
        /// 表字段的结构信息
        /// </summary>
        public List<HiColumn> Columns
        {
            get
            {
                return _columns;
            }
            set { _columns = value; }
        }

        [JsonIgnore]
        public List<HiColumn> GetColumns
        {
            get
            {
                return _columns.OrderByDescending(it => it.IsPrimary).ThenBy(it => it.SortNum).ToList();
            }
        }
        /// <summary>
        /// 获取主键字段
        /// </summary>
        [JsonIgnore]
        public List<HiColumn> PrimaryKey
        {
            get
            {
                return Columns.Where(it => it.IsPrimary == true).OrderBy(it => it.SortNum).ToList();
            }
        }
        /// <summary>
        /// 获取业务KEY
        /// </summary>
        [JsonIgnore]
        public List<HiColumn> BllKey
        {
            get
            {
                return Columns.Where(it => it.IsBllKey == true).OrderBy(it => it.SortNum).ToList();
            }
        }

        /// <summary>
        /// 如果一个表中的主键是自增长,且业务主键也是该主键的话是不允许进行Merge Into操作
        /// </summary>
        [JsonIgnore]
        public bool IsAllowMergeInto
        {
            get
            {
                bool _isllow = true;
                var identitykey = Columns.Where(c => c.IsIdentity && c.IsPrimary).OrderBy(it => it.SortNum).ToList();
                if (identitykey.Count > 0)
                {
                    var bllkey = Columns.Where(c => c.IsBllKey && !c.IsIdentity && !c.IsPrimary).OrderBy(it => it.SortNum).ToList();
                    if (bllkey.Count == 0)
                    {
                        _isllow = false;
                    }
                    else
                    {
                        foreach (HiColumn hiColumn in bllkey)
                        {
                            if (identitykey.Contains(hiColumn))
                                _isllow = false;
                        }
                    }
                }
                return _isllow;
            }
        }

        /// <summary>
        /// 获取标准字段
        /// </summary>
        [JsonIgnore]
        public List<HiColumn> StandKey
        {
            get {
                return Columns.Where(it => it.FieldName.ToLower().IsIn<string>("createtime", "createname", "moditime", "modiname") == true).ToList();
            }
        }


    }
}
