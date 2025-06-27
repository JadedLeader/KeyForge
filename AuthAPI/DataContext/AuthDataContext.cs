using AuthAPI.DataModel;
using Microsoft.EntityFrameworkCore;

namespace AuthAPI.DataContext
{
    public class AuthDataContext : DbContext
    {

        private readonly IConfiguration _configuration;

        public AuthDataContext(IConfiguration config)
        {
            _configuration = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string? connectionString = _configuration.GetConnectionString("AuthAPIConnection");

            if(connectionString == null)
            {
                throw new ArgumentNullException(nameof(connectionString), $"No connection string was provided for the Auth API");
            }

            optionsBuilder.UseSqlServer(connectionString);
        }


        public DbSet<AccountDataModel> Account { get; set; }

        public DbSet<AuthDataModel> Auth { get; set; }
    }
}
