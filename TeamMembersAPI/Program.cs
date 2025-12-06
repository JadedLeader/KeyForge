

using gRPCIntercommunicationService;
using Microsoft.EntityFrameworkCore;
using TeamMembersAPI.BackgroundConsumers.Accounts;
using TeamMembersAPI.BackgroundConsumers.TeamConsumer;
using TeamMembersAPI.BackgroundConsumers.TeamInvites;
using TeamMembersAPI.BackgroundConsumers.TeamVaults;
using TeamMembersAPI.DataContext;
using TeamMembersAPI.DomainService;
using TeamMembersAPI.Interfaces.DomainService;
using TeamMembersAPI.Interfaces.Services;
using TeamMembersAPI.Services;

namespace TeamMembersAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddGrpc();

            builder.Services.AddGrpcClient<Account.AccountClient>(options =>
            {
                options.Address = new Uri("https://localhost:7003");
            });

            builder.Services.AddGrpcClient<Team.TeamClient>(options =>
            {
                options.Address = new Uri("https://localhost:7285");
            });

            builder.Services.AddGrpcClient<TeamVault.TeamVaultClient>(options =>
            {
                options.Address = new Uri("https://localhost:7040");
            });

            builder.Services.AddGrpcClient<TeamInvite.TeamInviteClient>(options =>
            {
                options.Address = new Uri("https://localhost:7088");
            });

            builder.Services.AddDbContext<TeamMemberDataContext>(options =>
            {
                string? connectionString = builder.Configuration["ConnectionStrings:TeamMembersAPIConnection"];

                if(connectionString == null)
                {
                    throw new Exception("Team member API connection string is not set");
                }

                options.UseSqlServer(connectionString);
            });

            builder.Services.AddHostedService<CreateAccount>();
            builder.Services.AddHostedService<DeleteAccount>();

            builder.Services.AddHostedService<CreateTeam>();
            builder.Services.AddHostedService<DeleteTeam>();
            builder.Services.AddHostedService<UpdateTeam>();

            builder.Services.AddHostedService<CreateTeamVault>();
            builder.Services.AddHostedService<DeleteTeamVault>();
            builder.Services.AddHostedService<UpdateTeamVault>();

            builder.Services.AddHostedService<CreateTeamInvite>();
            builder.Services.AddHostedService<DeleteTeamInvite>();
            builder.Services.AddHostedService<UpdateTeamInvite>();

            builder.Services.AddScoped<ITeamMemberDomainService, TeamMemberDomainService>();
            builder.Services.AddScoped<ITeamMembersService, TeamMembersService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
