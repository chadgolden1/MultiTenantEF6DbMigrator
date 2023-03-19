using Data.Migrations;
using Microsoft.Extensions.Configuration;
using Migrator;
using Serilog;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Infrastructure;

ConfigureLogging();

Log.Information("Starting parallel execution of tenant database migrations...");

List<MigratorTenantInfo> tenants = GetConfiguredTenants();

ExitCode exitCode = ExecuteMigrations(tenants);

Log.Information("Parallel execution of tenant database migrations is complete.");

return (int)exitCode;

void ConfigureLogging() =>
    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .CreateLogger();

List<MigratorTenantInfo> GetConfiguredTenants()
{
    var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appSettings.json", optional: false);

    IConfiguration config = builder.Build();

    return config.GetSection(nameof(MigratorTenantInfo)).Get<List<MigratorTenantInfo>>();
}

ExitCode ExecuteMigrations(List<MigratorTenantInfo> tenants)
{
    ExitCode exitCode = ExitCode.Success;
    try
    {
        Parallel.ForEach(tenants, tenant =>
        {
            MigrateTenantDatabase(tenant);
        });
    }
    catch
    {
        exitCode = ExitCode.MigrationFailed;
    }

    return exitCode;
}

void MigrateTenantDatabase(MigratorTenantInfo t)
{
    var dbMigrator = new MigratorLoggingDecorator(
        new DbMigrator(new Configuration
        {
            TargetDatabase = new DbConnectionInfo(t.ConnectionString, "System.Data.SqlClient")
        }),
        new EfLogger(t)
    );

    try
    {
        dbMigrator.Update();
    }
    catch (Exception e)
    {
        Log.Error(e, $"Error executing migrations for {t.Name}");
        throw;
    }
}
