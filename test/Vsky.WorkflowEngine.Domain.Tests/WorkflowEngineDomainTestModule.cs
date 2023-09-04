using Vsky.WorkflowEngine.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Vsky.WorkflowEngine;

[DependsOn(
    typeof(WorkflowEngineEntityFrameworkCoreTestModule)
    )]
public class WorkflowEngineDomainTestModule : AbpModule
{

}
