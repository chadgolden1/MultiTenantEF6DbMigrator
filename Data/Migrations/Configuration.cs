namespace Data.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Threading;

    public sealed class Configuration : DbMigrationsConfiguration<Data.Ef6DbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Data.Ef6DbContext context)
        {
            Thread.Sleep(5000);
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
        }
    }
}
