using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp;

namespace Vsky.WorkflowEngine;

public class WorkflowEngineWebTestStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddApplication<WorkflowEngineWebTestModule>();
    }

    public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
    {
        app.InitializeApplication();
    }
}
