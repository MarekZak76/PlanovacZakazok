using System.Data.Entity;

namespace JobManager.Service
{
    public class DatabaseContext : DbContext
    {
        // import z web.config
        public const string DATABASE_CONNECTION_STRING = "name=DatabaseConnectionString";

        public DatabaseContext(string connectionString) : base(connectionString)
        {
        }

        public DbSet<SJob> SJob { get; set; }
        public DbSet<SLocation> SLocation { get; set; }

    }
}