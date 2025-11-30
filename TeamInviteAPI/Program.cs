
using gRPCIntercommunicationService;
using KeyForgedShared.Helpers;
using KeyForgedShared.Interfaces;
using KeyForgedShared.SharedDataModels;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TeamInviteAPI.BackgroundConsumers.Accounts;
using TeamInviteAPI.BackgroundConsumers.TeamConsumer;
using TeamInviteAPI.BackgroundConsumers.TeamVaults;
using TeamInviteAPI.DataContext;
using TeamInviteAPI.Interfaces.Repos;
using TeamInviteAPI.Interfaces.Services;
using TeamInviteAPI.Repos;
using TeamInviteAPI.Services;

namespace TeamInviteAPI
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

            builder.Services.AddDbContext<TeamInviteDataContext>(options =>
            {
                string? connectionString = builder.Configuration["ConnectionStrings:TeamInviteAPIConnection"];

                if(connectionString == null)
                {
                    throw new Exception("Connection string for Team Invite API doesn't exist");
                }

                options.UseSqlServer(connectionString);
            });

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            builder.Services.AddScoped<IAccountRepo, AccountRepo>();
            builder.Services.AddScoped<ITeamRepo, TeamRepo>();
            builder.Services.AddScoped<ITeamInviteRepo, TeamInviteRepo>();
            builder.Services.AddScoped<ITeamVaultRepo, TeamVaultRepo>();

            builder.Services.AddHostedService<CreateAccount>(); 
            builder.Services.AddHostedService<DeleteAccount>();

            builder.Services.AddHostedService<CreateTeam>();
            builder.Services.AddHostedService<DeleteTeam>();
            builder.Services.AddHostedService<UpdateTeam>(); 

            builder.Services.AddHostedService<CreateTeamVault>();
            builder.Services.AddHostedService<DeleteTeamVault>();
            builder.Services.AddHostedService<UpdateTeamVault>();

            builder.Services.AddScoped<ITeamInviteService, TeamInviteService>();
            builder.Services.AddScoped<IJwtHelper, JwtHelper>();

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
