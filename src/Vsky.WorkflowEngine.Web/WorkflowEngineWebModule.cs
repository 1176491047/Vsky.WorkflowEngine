using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vsky.WorkflowEngine.EntityFrameworkCore;
using Vsky.WorkflowEngine.Localization;
using Vsky.WorkflowEngine.MultiTenancy;
using Vsky.WorkflowEngine.Web.Menus;
using Microsoft.OpenApi.Models;
using OpenIddict.Validation.AspNetCore;
using Volo.Abp;
using Volo.Abp.Account.Web;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.AutoMapper;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity.Web;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.Web;
using Volo.Abp.SettingManagement.Web;
using Volo.Abp.Swashbuckle;
using Volo.Abp.TenantManagement.Web;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.UI;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;
using Vsky.WorkflowEngine.Web.Workflows;
using Elsa.Services;
using Elsa.Persistence.EntityFramework.Core.Extensions;
using Microsoft.AspNetCore.Mvc.Versioning;
using Volo.Abp.AspNetCore.Mvc.AntiForgery;
using Elsa.Persistence.EntityFramework.SqlServer;
using Elsa.Persistence.EntityFramework.PostgreSql;
using Elsa;
using Vsky.WorkflowEngine.Web.Activities;
using Elsa.Activities.Kafka.Extensions;
using Elsa.Activities.RabbitMq.Extensions;
using Elsa.Activities.Temporal;
using Elsa.Activities.UserTask.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Configuration;
using System.Data;
using Vsky.WorkflowEngine.Enum;
using Vsky.WorkflowEngine.Web.Activities.Other;

namespace Vsky.WorkflowEngine.Web;

[DependsOn(
    typeof(WorkflowEngineHttpApiModule),
    typeof(WorkflowEngineApplicationModule),
    typeof(WorkflowEngineEntityFrameworkCoreModule),
    typeof(AbpAutofacModule),
    typeof(AbpIdentityWebModule),
    typeof(AbpSettingManagementWebModule),
    typeof(AbpAccountWebOpenIddictModule),
    typeof(AbpAspNetCoreMvcUiLeptonXLiteThemeModule),
    typeof(AbpTenantManagementWebModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpSwashbuckleModule)
    )]
public class WorkflowEngineWebModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            Configure<AbpAntiForgeryOptions>(options => {
                options.AutoValidateIgnoredHttpMethods.Add("POST");
                options.AutoValidateIgnoredHttpMethods.Add("PUT");
                options.AutoValidateIgnoredHttpMethods.Add("DELETE");

            });
            options.AddAssemblyResource(
                typeof(WorkflowEngineResource),
                typeof(WorkflowEngineDomainModule).Assembly,
                typeof(WorkflowEngineDomainSharedModule).Assembly,
                typeof(WorkflowEngineApplicationModule).Assembly,
                typeof(WorkflowEngineApplicationContractsModule).Assembly,
                typeof(WorkflowEngineWebModule).Assembly
            );
        });

        PreConfigure<OpenIddictBuilder>(builder =>
        {
            builder.AddValidation(options =>
            {
                options.AddAudiences("WorkflowEngine");
                options.UseLocalServer();
                options.UseAspNetCore();
            });
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        ConfigureAuthentication(context);
        ConfigureUrls(configuration);
        ConfigureBundles();
        ConfigureAutoMapper();
        ConfigureVirtualFileSystem(hostingEnvironment);
        ConfigureNavigationServices();
        ConfigureAutoApiControllers();
        ConfigureSwaggerServices(context.Services);
        ConfigureElsa(context, configuration);
    }


    private DbContextOptionsBuilder DbContextBuilder(DbContextOptionsBuilder ef, IConfiguration configuration,string dbType= "")
    {
        if (dbType== DatabaseType.Sqlserver)
        {
            return Elsa.Persistence.EntityFramework.SqlServer.DbContextOptionsBuilderExtensions.UseSqlServer(ef,
                            configuration.GetConnectionString(DatabaseType.Sqlserver));
        }
        else if (dbType== DatabaseType.Pgsql)
        {

            return Elsa.Persistence.EntityFramework.PostgreSql.DbContextOptionsBuilderExtensions.UsePostgreSql(ef,
                            configuration.GetConnectionString(DatabaseType.Pgsql));
        }
        else if (dbType == DatabaseType.Oracle)
        {

            return Elsa.Persistence.EntityFramework.Oracle.DbContextOptionsBuilderExtensions.UseOracle(ef,
                            configuration.GetConnectionString(DatabaseType.Oracle));
        }
        else if (dbType == DatabaseType.Mysql)
        {
            return Elsa.Persistence.EntityFramework.MySql.DbContextOptionsBuilderExtensions.UseMySql(ef,
                            configuration.GetConnectionString(DatabaseType.Mysql));
        }
        else
        {
            return Elsa.Persistence.EntityFramework.SqlServer.DbContextOptionsBuilderExtensions.UseSqlServer(ef,
                            configuration.GetConnectionString("Default"));
        }
    }

    private void ConfigureElsa(ServiceConfigurationContext context, IConfiguration configuration)
    {
        string databaseType = configuration.GetConnectionString("DatabaseType");
        var elsaSection = configuration.GetSection("Elsa");

        context.Services.AddElsa(elsa =>
        {
            elsa
                .UseEntityFrameworkPersistence(ef => DbContextBuilder(ef, configuration, databaseType))
                .AddConsoleActivities()
                .AddHttpActivities(elsaSection.GetSection("Server").Bind)
                .AddQuartzTemporalActivities()
                .AddKafkaActivities()
                .AddRabbitMqActivities()
               .AddCommonTemporalActivities()
               .AddUserTaskActivities()
               .AddEntityActivities()
               .AddFileActivities()
               .AddQuartzTemporalActivities()
                .AddJavaScriptActivities()
                    .AddActivitiesFrom<FileHandling>()
                    .AddActivitiesFrom<SendMail>()
                    .AddActivitiesFrom<SendMailWithSMTPConfig>()
                    .AddActivitiesFrom<WeiChatActivity>()
                    .AddActivitiesFrom<WeiChatActivityWithContentType>()
                    .AddActivitiesFrom<SendHttpRequestTest>()
                .AddWorkflow<HelloWorldConsole>()
                .AddWorkflow<HelloWorldHttp>(); ;
        });

        context.Services.AddElsaApiEndpoints();
        context.Services.Configure<ApiVersioningOptions>(options =>
        {
            options.UseApiBehavior = false;
        });

        context.Services.AddCors(cors => cors.AddDefaultPolicy(policy => policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin()
            .WithExposedHeaders("Content-Disposition"))
        );

        //Uncomment the below line if your abp version is lower than v4.4 to register controllers of Elsa .
        //See https://github.com/abpframework/abp/pull/9299 (we will no longer need to specify this line of code from v4.4)
        // context.Services.AddAssemblyOf<Elsa.Server.Api.Endpoints.WorkflowRegistry.Get>();

        //Disable antiforgery validation for elsa
        Configure<AbpAntiForgeryOptions>(options =>
        {
            options.AutoValidateFilter = type =>
                type.Assembly != typeof(Elsa.Server.Api.Endpoints.WorkflowRegistry.Get).Assembly;
        });
    }

    private void ConfigureAuthentication(ServiceConfigurationContext context)
    {
        context.Services.ForwardIdentityAuthenticationForBearer(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
    }

    private void ConfigureUrls(IConfiguration configuration)
    {
        Configure<AppUrlOptions>(options =>
        {
            options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
        });
    }

    private void ConfigureBundles()
    {
        Configure<AbpBundlingOptions>(options =>
        {
            options.StyleBundles.Configure(
                LeptonXLiteThemeBundles.Styles.Global,
                bundle =>
                {
                    bundle.AddFiles("/global-styles.css");
                }
            );
        });
    }

    private void ConfigureAutoMapper()
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<WorkflowEngineWebModule>();
        });
    }

    private void ConfigureVirtualFileSystem(IWebHostEnvironment hostingEnvironment)
    {
        if (hostingEnvironment.IsDevelopment())
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.ReplaceEmbeddedByPhysical<WorkflowEngineDomainSharedModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}Vsky.WorkflowEngine.Domain.Shared"));
                options.FileSets.ReplaceEmbeddedByPhysical<WorkflowEngineDomainModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}Vsky.WorkflowEngine.Domain"));
                options.FileSets.ReplaceEmbeddedByPhysical<WorkflowEngineApplicationContractsModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}Vsky.WorkflowEngine.Application.Contracts"));
                options.FileSets.ReplaceEmbeddedByPhysical<WorkflowEngineApplicationModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}Vsky.WorkflowEngine.Application"));
                options.FileSets.ReplaceEmbeddedByPhysical<WorkflowEngineWebModule>(hostingEnvironment.ContentRootPath);
            });
        }
    }

    private void ConfigureNavigationServices()
    {
        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new WorkflowEngineMenuContributor());
        });
    }

    private void ConfigureAutoApiControllers()
    {
        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            options.ConventionalControllers.Create(typeof(WorkflowEngineApplicationModule).Assembly);
        });
    }

    private void ConfigureSwaggerServices(IServiceCollection services)
    {
        services.AddAbpSwaggerGen(
            options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "WorkflowEngine API", Version = "v1" });
                options.DocInclusionPredicate((docName, description) => true);
                options.CustomSchemaIds(type => type.FullName);
            }
        );
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseAbpRequestLocalization();

        if (!env.IsDevelopment())
        {
            app.UseErrorPage();
        }

        app.UseCorrelationId();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAbpOpenIddictValidation();

        if (MultiTenancyConsts.IsEnabled)
        {
            app.UseMultiTenancy();
        }

        app.UseUnitOfWork();
        app.UseAuthorization();
        app.UseSwagger();
        app.UseAbpSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "WorkflowEngine API");
        });
        app.UseAuditing();
        app.UseAbpSerilogEnrichers();
        app.UseHttpActivities();//ELSA http activities
        app.UseConfiguredEndpoints(endpoints =>
        {
            endpoints.MapFallbackToPage("/_Host");
        });



        var workflowRunner = context.ServiceProvider.GetRequiredService<IBuildsAndStartsWorkflow>();
        workflowRunner.BuildAndStartWorkflowAsync<HelloWorldConsole>();
    }
}
