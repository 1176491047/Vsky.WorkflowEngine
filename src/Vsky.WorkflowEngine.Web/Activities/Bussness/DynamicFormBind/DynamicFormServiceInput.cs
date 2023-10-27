using Elsa.Server.Api.Endpoints.Activities;
using System;
using System.Collections.Generic;

namespace Vsky.WorkflowEngine.Web.Activities.Bussness.DynamicFormBind
{
    public class DynamicFormServiceInput
    {

        public Guid FormInstanceId { get; set; }
        /// <summary>
        /// 流程实例id
        /// </summary>
        public Guid WorkflowInstanseId { get; set; }
        /// <summary>
        /// 流程节点Id
        /// </summary>
        public Guid CurrentNodeId { get; set; }
        /// <summary>
        /// 流程实例名称
        /// </summary>
        public string? CurrentNodeName { get; set; }
        /// <summary>
        /// 流程定义Id
        /// </summary>
        public Guid WorkflowDefinitionId { get; set; }
        /// <summary>
        /// 责任部门
        /// </summary>
        public Guid? Department { get; set; }
        /// <summary>
        /// 责任人
        /// </summary>
        public List<string>? Users { get; set; }
    }
}
