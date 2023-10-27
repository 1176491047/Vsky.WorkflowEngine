using System;

namespace Vsky.WorkflowEngine.Web.Activities.Bussness.DynamicFormBind
{
    public class WorkFlowInstanceActivateDto
    {
        public Guid FormInstanceId { get; set; }

        public Guid WorkFlowDefinitionId { get; set; }
    }
}
