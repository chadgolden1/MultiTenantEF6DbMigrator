using Serilog;
using System.Data.Entity.Migrations.Infrastructure;

namespace Migrator
{
    internal sealed class EfLogger : MigrationsLogger
    {
        private readonly MigratorTenantInfo _tenantInfo;

        public EfLogger(MigratorTenantInfo tenantInfo) => _tenantInfo = tenantInfo;

        public override void Info(string message) => Log.Information($"{_tenantInfo.Name}: {message}");

        public override void Verbose(string message) { /* no op */ }

        public override void Warning(string message) => Log.Warning($"{_tenantInfo.Name}: {message}");
    }
}