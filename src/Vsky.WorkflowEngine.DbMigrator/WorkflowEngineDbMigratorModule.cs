using Vsky.WorkflowEngine.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Vsky.WorkflowEngine.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(WorkflowEngineEntityFrameworkCoreModule),
    typeof(WorkflowEngineApplicationContractsModule)
    )]
public class WorkflowEngineDbMigratorModule : AbpModule
{
}
