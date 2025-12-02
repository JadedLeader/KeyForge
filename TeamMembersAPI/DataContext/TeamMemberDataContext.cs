using KeyForgedShared.SharedDataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;

namespace TeamMembersAPI.DataContext
{
    public class TeamMemberDataContext : DbContext
    {

        private readonly IConfiguration _config;

        public TeamMemberDataContext(IConfiguration config)
        {
            _config = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string? connectionString = _config["ConnectionStrings:TeamMembersAPIConnection"];

            if(connectionString == null)
            {
                throw new Exception("Team member API connection string is not set");
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

            modelBuilder.Entity<TeamInviteDataModel>()
                        .Property(x => x.InviteStatus)
                         .HasConversion<string>();
        }

        public DbSet<AccountDataModel> Account { get; set; }

        public DbSet<TeamDataModel> Team { get; set; }

        public DbSet<TeamVaultDataModel> TeamVault { get; set; }

        public DbSet<TeamInviteDataModel> TeamInvite { get; set; }

        public DbSet<TeamMemberDataModel> TeamMember { get; set; }
    }
}
