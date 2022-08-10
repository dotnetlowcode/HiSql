using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{

    public class Hi_UpgradeCol
    {
        public TabFieldAction TabFieldAction { get; set; }

        public HiColumn ColumnInfo { get; set; }
    }
    public class Hi_UpgradeTab
    {
        public string TabName { get; set; }
        
        public List<Hi_UpgradeCol> Columns
        { get; set; }
    }
    public class Hi_UpgradeInfo
    {
        /// <summary>
        /// 大于等于低版本
        /// </summary>
        public Version MinVersion { get; set; }

        /// <summary>
        /// 小于高版本
        /// </summary>
        public Version MaxVersion { get; set; }

        public List<Hi_UpgradeTab> UpgradTabs { get; set; }

    }
}
