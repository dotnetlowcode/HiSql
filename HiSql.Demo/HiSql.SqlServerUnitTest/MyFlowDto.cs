using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql.SqlServerUnitTest
{

    /// <summary>
    /// 流程管理列表DTO
    /// </summary>
    public class FlowManagerDto
    {
        /// <summary>
        /// 工作流流水号
        /// </summary>
        public string WFNum { get; set; }
        /// <summary>
        /// 工作流标题
        /// </summary>
        public string WFTitle { get; set; }
        /// <summary>
        /// 流程名称
        /// </summary>
        public string FlowName { get; set; }
        /// <summary>
        /// 流程ID
        /// </summary>
        public Guid FlowID { get; set; }
        /// <summary>
        /// 流程类型ID
        /// </summary>
        public Guid GroupID { get; set; }
        /// <summary>
        /// 流程类型
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// 流程状态
        /// </summary>
        public int? WFState { get; set; }
        /// <summary>
        /// 流程状态描述
        /// </summary>
        public string WFStateText { get; set; }

        /// <summary>
        /// 发起客户端
        /// </summary>
        public string CreateClient { get; set; }
        /// <summary>
        /// 发起系统
        /// </summary>
        public string CreateSystem { get; set; }
        /// <summary>
        /// 流程发起者ID
        /// </summary>
        public string CreateUserID { get; set; }
        /// <summary>
        /// CreateName
        /// </summary>
        public string CreateUserName { get; set; }
        /// <summary>
        /// 流程完成时间
        /// </summary>
        public DateTime? CompleteTime { get; set; }
        /// <summary>
        /// 流程申请时间
        /// </summary>
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// 流程当前节点ID
        /// </summary>
        public Guid NodeId { get; set; }
        /// <summary>
        /// 当前步骤名
        /// </summary>
        public string NodeName { get; set; }
        /// <summary>
        /// 当前节点处理人
        /// </summary>
        public string ReceiveNameS { get; set; }
        /// <summary>
        /// 当前节点状态
        /// </summary>
        public int? NodeState { get; set; }
        /// <summary>
        /// 当前节点状态描述
        /// </summary>
        public string NodeStateText { get; set; }
        /// <summary>
        /// 当前节点接收申请时间
        /// </summary>
        public DateTime? NodeCreateTime { get; set; }

    }

    public class BaseEntity
	{
	}

	///<summary>
	///Wf_Instance
	///</summary>
	[Table("Wf_Instance")]
	public partial class Wf_Instance : BaseEntity
	{
		public Wf_Instance() { }


		/// <summary>
		/// 工作流流水号
		/// </summary>
		[Key]
		public string WFNum { get; set; }


		/// <summary>
		/// 流程ID
		/// </summary>
		public Guid FlowID { get; set; }


		/// <summary>
		/// 流程名称
		/// </summary>
		public string FlowName { get; set; }


		/// <summary>
		/// 工作流标题
		/// </summary>
		public string WFTitle { get; set; }


		/// <summary>
		/// 流程状态
		/// </summary>
		public int? WFState { get; set; }


		/// <summary>
		/// 发起客户端
		/// </summary>
		public string CreateClient { get; set; }


		/// <summary>
		/// 发起系统
		/// </summary>
		public string CreateSystem { get; set; }


		/// <summary>
		/// 完成时间
		/// </summary>
		public DateTime? CompleteTime { get; set; }


		/// <summary>
		/// 流程发起者ID
		/// </summary>
		public string CreateUserID { get; set; }


		/// <summary>
		/// 流程发起者ID
		/// </summary>
		public string CreateUserName { get; set; }


		/// <summary>
		/// 发起人部门ID
		/// </summary>
		public string CreateUserDepID { get; set; }


		/// <summary>
		/// 发起人部门名称
		/// </summary>
		public string CreateUserDepName { get; set; }


		/// <summary>
		/// 发起人区域ID
		/// </summary>
		public string CreateUserOrgID { get; set; }


		/// <summary>
		/// 发起人区域名称
		/// </summary>
		public string CreateUserOrgName { get; set; }


		/// <summary>
		/// 发起人公司编号
		/// </summary>
		public string CreateUserTenantId { get; set; }


		/// <summary>
		/// 发起人公司名称
		/// </summary>
		public string CreateUserTenantName { get; set; }


		/// <summary>
		/// 流程紧急程序(0普通1重要2紧急)
		/// </summary>
		public int? Urgent { get; set; }


		/// <summary>
		/// CreateName
		/// </summary>
		public string CreateName { get; set; }


		/// <summary>
		/// CreateTime
		/// </summary>
		public DateTime? CreateTime { get; set; }


		/// <summary>
		/// ModiName
		/// </summary>
		public string ModiName { get; set; }


		/// <summary>
		/// ModiTime
		/// </summary>
		public DateTime? ModiTime { get; set; }


		/// <summary>
		/// DataState
		/// </summary>
		public int? DataState { get; set; }


		/// <summary>
		/// TenantId
		/// </summary>
		public string TenantId { get; set; }

	}
	public class MyFlowDto2
	{
		
		/// <summary>
		/// 流程状态
		/// </summary>
		public int? WFState { get; set; }
	}
	/// <summary>
	/// 我申请流程列表字段
	/// </summary>
	/// <summary>
	/// 我申请流程列表字段
	/// </summary>
	public class MyFlowDto
	{
		/// <summary>
		/// 工作流流水号
		/// </summary>
		public string WFNum { get; set; }

		/// <summary>
		/// 流程ID
		/// </summary>
		public Guid FlowID { get; set; }
		/// <summary>
		/// 流程名称
		/// </summary>
		public string FlowName { get; set; }
		/// <summary>
		/// 工作流标题
		/// </summary>
		public string WFTitle { get; set; }
		/// <summary>
		/// 流程状态
		/// </summary>
		public int? WFState { get; set; }
		/// <summary>
		/// 发起客户端
		/// </summary>
		public string CreateClient { get; set; }

		/// <summary>
		/// 发起系统
		/// </summary>
		public string CreateSystem { get; set; }
		/// <summary>
		/// 完成时间
		/// </summary>
		public DateTime? CompleteTime { get; set; }

		/// <summary>
		/// 创建时间
		/// </summary>
		public DateTime? CreateTime { get; set; }
		/// <summary>
		/// 流程发起者ID
		/// </summary>
		public string CreateUserID { get; set; }
		/// <summary>
		/// 流程发起者
		/// </summary>
		public string CreateUserName { get; set; }

		public Guid NodeInstanceID { get; set; }
		/// <summary>
		/// 流程节点ID
		/// </summary>
		public Guid NodeId { get; set; }
		/// <summary>
		/// 流程节点名
		/// </summary>
		public string NodeName { get; set; }
		/// <summary>
		/// 流程节点类型
		/// </summary>
		public string NodeType { get; set; }
		/// <summary>
		/// 来源节点ID
		/// </summary>
		public Guid? FromNodeInstanceID { get; set; }
		/// <summary>
		/// 来源节点名称
		/// </summary>
		public string FromNodeName { get; set; }
		/// <summary>
		/// 当前节点状态
		/// </summary>
		public int? NodeState { get; set; }
		/// <summary>
		/// 是否是退回的节点
		/// </summary>
		public string IsReject { get; set; }

		/// <summary>
		/// 当前节点处理人
		/// </summary>
		public string ReceiveNameS { get; set; }

		/// <summary>
		/// 当前节点状态描述
		/// </summary>
		public string NodeStateText { get; set; }
		/// <summary>
		/// 流程状态描述
		/// </summary>
		public string WFStateText { get; set; }

		/// <summary>
		/// 当前任务ID(申请人步骤的任务ID)
		/// </summary>
		public Guid TaskID { get; set; }

	}
}
