using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Vsky.WorkflowEngine.Data;
using Volo.Abp.DependencyInjection;

namespace Vsky.WorkflowEngine.EntityFrameworkCore;

public class EntityFrameworkCoreWorkflowEngineDbSchemaMigrator
    : IWorkflowEngineDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreWorkflowEngineDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the WorkflowEngineDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<WorkflowEngineDbContext>()
            .Database
            .MigrateAsync();
    }
}
