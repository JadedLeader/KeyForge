using KeyForgedShared.SharedDataModels;
using Microsoft.EntityFrameworkCore;


namespace VaultAPI.DataContext
{
    public class VaultDataContext : DbContext
    {
        private readonly IConfiguration _config;
        public VaultDataContext(IConfiguration config)
        {
            _config = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string? connectionString = _config.GetConnectionString("VaultApiConnectionString");

            if(connectionString == string.Empty)
            {
                throw new Exception($"Connection string was null when trying to configure the Vault API connection");
            }

            optionsBuilder.UseSqlServer(connectionString);
        }


        public DbSet<AccountDataModel> Account { get; set; }

        public DbSet<AuthDataModel> Auth { get; set; }

        public DbSet<VaultDataModel> Vault { get; set; }

    }
}
