using KeyForgedShared.SharedDataModels;
using Microsoft.EntityFrameworkCore;

namespace AccountAPI.DataContext
{
    public class AccountDataContext : DbContext
    {

        private readonly IConfiguration _configuration;

        public AccountDataContext(DbContextOptions<AccountDataContext> options, IConfiguration config) : base(options)
        {
            _configuration = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string? connectionString = _configuration.GetConnectionString("AccountAPIConnection");

            if(connectionString == null)
            {
                throw new ArgumentNullException(nameof(connectionString), $"No connection string was provided");
            }

            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<AccountDataModel> Account { get; set; }

    }
}
