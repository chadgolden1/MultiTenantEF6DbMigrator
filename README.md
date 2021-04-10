# MultiTenantEF6Migrator
An example ```net5.0``` console application that executes pending Entity Framework 6 (EF6) migrations to configured tenant databases.

## Remarks
This is useful for efficiently executing migrations in multi-tenanted scenarios where each tenant has its own connection string/database as part of a shared schema, or DbContext.

It offers a way to execute migrations using EF6 Migrator APIs in code as opposed to using the PowerShell modules or ef6.exe (formally migrate.exe). This approach could be used within deployment pipelines (e.g. Octopus Deploy)

Uses TPL to execute pending migrations in parallel - much faster than serial execution