using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql.AST
{
    /// <summary>
    /// 用于跨平台的update hisql语句
    /// add by tgm date:2022.6.27
    /// email:tansar@126.com
    /// </summary>
    public class UpdateParse
    {
        public static class Constants
        {
            /// <summary>
            /// 检测是否是合法的update语句
            /// </summary>
            public static string REG_UPDATE = @"\s*(?<cmd>update)\s+(?:[\s]*)(?<flag>[\#]{1,2}|[\@]{1})?(?<tab>[\w-_]+)\s+(?<set>[.\s\S]+)";

        }
        HiSqlProvider Context = null;
        private string _sql = "";
        public UpdateParse(string sql, HiSqlProvider context)
        {
            this._sql = sql;
            Context = context;
            if (context == null) throw new Exception($"context 为Null");
        }
        private void parseUpdate(string sql)
        {
            if (Tool.RegexMatch(Constants.REG_UPDATE, sql))
            {
                #region 拆解 update 语句
                var rtn = Tool.RegexGrpOrReplace(Constants.REG_UPDATE, sql);

                #endregion
            }
            else
            {
                throw new Exception($"{HiSql.Constants.HiSqlSyntaxError}语句[{sql}] 非更新语言");
            }
        }


    }
}
