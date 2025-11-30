using KeyForgedShared.SharedDataModels;
using Microsoft.EntityFrameworkCore;

namespace TeamInviteAPI.DataContext
{
    public class TeamInviteDataContext : DbContext
    {
        private readonly IConfiguration _config;
        public TeamInviteDataContext(IConfiguration config)
        {
            _config = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string? connectionString = _config["ConnectionStrings:TeamInviteAPIConnection"];

            if(connectionString == null)
            {
                throw new Exception($"Connection string for Team Invite API doesn't exist");
            }

            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TeamInviteDataModel>()
                        .HasOne(i => i.Account)
                        .WithMany()
                        .HasForeignKey(i => i.AccountId)
                        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TeamInviteDataModel>()
                        .HasOne(i => i.TeamVault)
                        .WithMany()
                        .HasForeignKey(i => i.TeamVaultId)
                        .OnDelete(DeleteBehavior.Restrict);
        }

        public DbSet<AccountDataModel> Account { get; set; }

        public DbSet<TeamDataModel> Team { get; set; }

        public DbSet<TeamVaultDataModel> TeamVault { get; set; }

        public DbSet<TeamInviteDataModel> TeamInvite { get; set; }

    }
}
