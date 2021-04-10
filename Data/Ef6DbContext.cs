using System.Data.Entity;

namespace Data
{
    public class Ef6DbContext : DbContext
    {
        public Ef6DbContext() : base("Server=(LocalDb)\\MSSQLLocalDB;Database=DefaultContext;Trusted_Connection=True;")
        {  
        }

        public Ef6DbContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }

        public IDbSet<User> Users { get; set; }
    }
}
