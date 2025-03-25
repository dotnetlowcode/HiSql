using System;
using System.Collections.Generic;
using HiSql.Common.Entities.TabLog;

namespace HiSql.Interface.TabLog
{
    public class Credential
    {

        /// <summary>
        /// 操作凭证关联的表名
        /// </summary>
        public string TableName { get; set; }


        public string CredentialId { get; set; }

        public List<OperationLog> OperationLogs { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public string OperateUserName { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 透传数据
        /// </summary>
        public object State { get; set; }

        /// <summary>
        /// 父凭证ID
        /// </summary>
        public string RefCredentialId { get; set; }
    }


}
