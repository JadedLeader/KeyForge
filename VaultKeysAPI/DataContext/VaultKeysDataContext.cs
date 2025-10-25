using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using KeyForgedShared.SharedDataModels;

namespace VaultKeysAPI.DataContext
{
    public class VaultKeysDataContext : DbContext
    {

        private readonly IConfiguration _configuration;
        public VaultKeysDataContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string? dbConnectionString = _configuration.GetConnectionString("VaultKeysApiConnectionString");

            if (dbConnectionString == string.Empty)
            {

                throw new Exception("No connection string provided for the vault keys API");
            }

            optionsBuilder.UseSqlServer(dbConnectionString);
        }


        public DbSet<AccountDataModel> Account { get; set; }

        public DbSet<AuthDataModel> Auth { get; set; }

        public DbSet<VaultDataModel> Vault { get; set;  }

        public DbSet<VaultKeysDataModel> VaultKeys { get; set; }
        
    }
}
