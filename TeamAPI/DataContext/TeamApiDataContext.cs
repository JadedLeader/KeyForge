using KeyForgedShared.SharedDataModels;
using Microsoft.EntityFrameworkCore;

namespace TeamAPI.DataContext
{
    public class TeamApiDataContext : DbContext
    {

        private readonly IConfiguration _config;
        public TeamApiDataContext(IConfiguration configuration)
        {
            _config = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string? connectionString = _config["ConnectionStrings:TeamApiConnection"];

            if(connectionString == null)
            {
                throw new Exception($"Team API connection string not allocated");
            }

            optionsBuilder.UseSqlServer(connectionString);
        }

        public DbSet<AccountDataModel> Account { get; set; }

        public DbSet<TeamDataModel> Team { get; set; }

    }
}
