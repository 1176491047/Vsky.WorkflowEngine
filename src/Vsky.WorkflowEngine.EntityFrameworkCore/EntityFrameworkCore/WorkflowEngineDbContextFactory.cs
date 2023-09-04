using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Vsky.WorkflowEngine.Enum;

namespace Vsky.WorkflowEngine.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class WorkflowEngineDbContextFactory : IDesignTimeDbContextFactory<WorkflowEngineDbContext>
{
    public WorkflowEngineDbContext CreateDbContext(string[] args)
    {
        WorkflowEngineEfCoreEntityExtensionMappings.Configure();

        var configuration = BuildConfiguration();

        var builder = GetDbContextOptionsBuilder(configuration);

        return new WorkflowEngineDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Vsky.WorkflowEngine.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }


    private DbContextOptionsBuilder<WorkflowEngineDbContext> GetDbContextOptionsBuilder(IConfigurationRoot configuration) {

    string databaseType = configuration.GetConnectionString("DatabaseType");
        if (databaseType == DatabaseType.Pgsql)
        {
            return new DbContextOptionsBuilder<WorkflowEngineDbContext>()
                      .UseNpgsql(configuration.GetConnectionString(DatabaseType.Pgsql));
        }
        else if (databaseType == DatabaseType.Oracle)
        {
            return new DbContextOptionsBuilder<WorkflowEngineDbContext>()
                      .UseOracle(configuration.GetConnectionString(DatabaseType.Oracle));
        }
        else if (databaseType == DatabaseType.Mysql)
        {
            return new DbContextOptionsBuilder<WorkflowEngineDbContext>()
                      .UseMySql(configuration.GetConnectionString(DatabaseType.Mysql),
                      ServerVersion.AutoDetect(configuration.GetConnectionString(DatabaseType.Mysql)), null);
        }
        else
        {
            return new DbContextOptionsBuilder<WorkflowEngineDbContext>()
                      .UseSqlServer(configuration.GetConnectionString("Default"));
        }
    }
}
