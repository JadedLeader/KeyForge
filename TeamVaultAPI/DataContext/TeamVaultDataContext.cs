using KeyForgedShared.SharedDataModels;
using Microsoft.EntityFrameworkCore;

namespace TeamVaultAPI.DataContext
{
    public class TeamVaultDataContext : DbContext
    {

        private readonly IConfiguration _config;

        public TeamVaultDataContext(IConfiguration config)
        {
            _config = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string? connectionString = _config["ConnectionStrings:TeamVaultAPIConnection"];

            if(connectionString == null)
            {
                throw new Exception($"Team vault API connection string not provided");
            }

            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }

        public DbSet<AccountDataModel> Account { get; set; }

        public DbSet<TeamDataModel> Team { get; set; }

        public DbSet<TeamVaultDataModel> TeamVault { get; set; }

    }
}
