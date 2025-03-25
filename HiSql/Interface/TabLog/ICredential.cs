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

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        /// <summary>
        /// 操作日志
        /// </summary>
        public List<OperationLog> OperationLogs { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public string OperateUserName { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        /// <summary>
        /// 透传数据
        /// </summary>
        public object State { get; set; }

        /// <summary>
        /// 父凭证ID
        /// </summary>
        public string RefCredentialId { get; set; }

        /// <summary>
        /// 创建数量
        /// </summary>
        public int CCount { get; set; }

        /// <summary>
        /// 修改数量
        /// </summary>
        public int DCount { get; set; }

        /// <summary>
        /// 删除数量
        /// </summary>
        public int MCount { get; set; }
    }


}
