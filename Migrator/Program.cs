using Data.Migrations;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Infrastructure;
using System.IO;
using System.Threading.Tasks;

namespace Migrator
{
    class Program
    {
        static int Main(string[] args)
        {
            ConfigureLogging();

            Log.Information("Starting parallel execution of tenant database migrations...");

            List<MigratorTenantInfo> tenants = GetConfiguredTenants();

            ExitCode exitCode = ExecuteMigrations(tenants);

            Log.Information("Parallel execution of tenant database migrations is complete.");

            return (int)exitCode;
        }

        private static ExitCode ExecuteMigrations(List<MigratorTenantInfo> tenants)
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

        private static void ConfigureLogging() => 
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

        private static List<MigratorTenantInfo> GetConfiguredTenants()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json", optional: false);

            IConfiguration config = builder.Build();

            return config.GetSection(nameof(MigratorTenantInfo)).Get<List<MigratorTenantInfo>>();
        }

        private static void MigrateTenantDatabase(MigratorTenantInfo t)
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
    }

    enum ExitCode : int
    {
        Success = 0,
        MigrationFailed = 1
    }
}
