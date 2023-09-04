using System.Threading.Tasks;

namespace Vsky.WorkflowEngine.Data;

public interface IWorkflowEngineDbSchemaMigrator
{
    Task MigrateAsync();
}
