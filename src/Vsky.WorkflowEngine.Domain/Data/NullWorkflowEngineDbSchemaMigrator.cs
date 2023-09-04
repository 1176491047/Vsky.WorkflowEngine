using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Vsky.WorkflowEngine.Data;

/* This is used if database provider does't define
 * IWorkflowEngineDbSchemaMigrator implementation.
 */
public class NullWorkflowEngineDbSchemaMigrator : IWorkflowEngineDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
