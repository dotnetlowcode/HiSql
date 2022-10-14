using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql.Unit.Test
{
    [HiTable(IsEdit = true, TabName = "WeatherForecast", TabDescript = "天气预报")]
    public class WeatherForecast : StandField
    {
        [HiColumn(FieldDesc = "编号", IsPrimary = true, FieldType = HiType.INT, SortNum = 1, IsIdentity = true)]
        public int SID { get; set; }

        [HiColumn(FieldDesc = "日期", FieldType = HiType.DATETIME)]
        public DateTime Date { get; set; }

        [HiColumn(FieldDesc = "摘要", FieldType = HiType.NVARCHAR, FieldLen = 100)]
        public string? Summary { get; set; }
    }
}
