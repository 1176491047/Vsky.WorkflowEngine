using Volo.Abp.Modularity;

namespace Vsky.WorkflowEngine;

[DependsOn(
    typeof(WorkflowEngineApplicationModule),
    typeof(WorkflowEngineDomainTestModule)
    )]
public class WorkflowEngineApplicationTestModule : AbpModule
{

}
