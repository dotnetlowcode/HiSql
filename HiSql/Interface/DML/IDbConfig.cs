using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public interface IDbConfig
    {

        /// <summary>
        /// 初始化HiSql 初始安装 Sql
        /// </summary>
        public string InitSql
        {
            get;
        }


        /// <summary>
        /// 当前数据库的日期函数
        /// </summary>
        public string Fun_CurrDATE { get; }

        public int BlukSize { get; set; }
        /// <summary>
        /// Schema 前辍
        /// </summary>
        public string Schema_Pre { get;   }

        /// <summary>
        /// SCHEMA 后辍
        /// </summary>
        public string Schema_After { get;   }

        /// <summary>
        /// 表前辍
        /// </summary>
        public string Table_Pre { get;   }

        /// <summary>
        /// 表后辍
        /// </summary>
        public string Table_After { get;   }

        /// <summary>
        /// 字段字符前辍
        /// </summary>
        public string Field_Pre { get;   }

        /// <summary>
        /// 字段 字符后辍
        /// </summary>
        public string Field_After { get;   }


        /// <summary>
        /// 实体表变量模版
        /// </summary>
        public string Table_Create { get;   }

        /// <summary>
        /// 全局临时表变量模版
        /// </summary>
        public string Table_Global_Create { get;   }

        public string Table_Global_Create_Drop { get; }

        /// <summary>
        /// 本地临时表
        /// </summary>
        public string Table_Local_Create { get; }

        public string Table_Local_Create_Drop { get; }
        /// <summary>
        /// 表变量 模版
        /// </summary>
        public string Table_Declare_Table { get;   }


        /// <summary>
        /// 字段分隔
        /// </summary>
        public string Field_Split { get;   }

        /// <summary>
        /// 表的KeY模板
        /// </summary>
        public string Table_Key { get;   }

        public string Table_Key2 { get;   }

        public string Table_Key3 { get;   }

        /// <summary>
        /// 字估描述
        /// </summary>
        public string Field_Comment { get;   }

        /// <summary>
        /// 获取表结构信息SQL模板
        /// </summary>
        public string Get_Table_Schema { get;   }


        /// <summary>
        /// 表数据插入模版1
        /// </summary>
        public string Insert_StateMent { get;   }

        /// <summary>
        /// 表数据插入模版2
        /// </summary>
        public string Insert_StateMentv2 { get;   }

       
        public string Update_Statement { get; }


        public string Update_Statement_Where { get; }

        
        public string Delete_Statement { get; }

        public string Delete_Statement_Where { get; }

        public string Delete_TrunCate { get; }

        public string Drop_Table { get; }
        /// <summary>
        /// 表批量更新
        /// </summary>
        public string Table_MergeInto { get; }
        Dictionary< string,  string> FieldTempMapping { get; }
        Dictionary<HiType,  string> DbMapping { get; }


        List<DefMapping> DbDefMapping { get; }
        void Init();

    }
}
