using System;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Uow;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.SqlServer;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.MySQL;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.PostgreSql;
using Microsoft.Extensions.Configuration;
using System.IO;
using Vsky.WorkflowEngine.Enum;
using Vsky.WorkflowEngine.WorkflowEngineSettingsManages;

namespace Vsky.WorkflowEngine.EntityFrameworkCore;

[DependsOn(
    typeof(WorkflowEngineDomainModule),
    typeof(AbpIdentityEntityFrameworkCoreModule),
    typeof(AbpOpenIddictEntityFrameworkCoreModule),
    typeof(AbpPermissionManagementEntityFrameworkCoreModule),
    typeof(AbpSettingManagementEntityFrameworkCoreModule),
    typeof(AbpEntityFrameworkCoreSqlServerModule),
    typeof(AbpEntityFrameworkCoreMySQLModule),
    typeof(AbpEntityFrameworkCorePostgreSqlModule),
    typeof(AbpBackgroundJobsEntityFrameworkCoreModule),
    typeof(AbpAuditLoggingEntityFrameworkCoreModule),
    typeof(AbpTenantManagementEntityFrameworkCoreModule),
    typeof(AbpFeatureManagementEntityFrameworkCoreModule)
    )]
public class WorkflowEngineEntityFrameworkCoreModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        WorkflowEngineEfCoreEntityExtensionMappings.Configure();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<WorkflowEngineDbContext>(options =>
        {
                /* Remove "includeAllEntities: true" to create
                 * default repositories only for aggregate roots */
            options.AddDefaultRepositories(includeAllEntities: true);
            options.AddRepository<WorkflowEngineSettingsManage, WorkflowEngineSettingsManages.EfCoreWorkflowEngineSettingsManageRepository>();
        });

        var configuration = context.Services.GetConfiguration();
        string databaseType = configuration.GetConnectionString("DatabaseType");
        Configure<AbpDbContextOptions>(options =>
        {
            /* The main point to change your DBMS.
             * See also WorkflowEngineMigrationsDbContextFactory for EF Core tooling. */
            if (databaseType == DatabaseType.Pgsql)
            {
                options.UseNpgsql();
            }
            else if (databaseType == DatabaseType.Oracle)
            {
                options.UseOracle();
            }
            else if (databaseType == DatabaseType.Mysql)
            {
                options.UseMySQL();
            }
            else
            {
                options.UseSqlServer();
            }
        });
    }
}



