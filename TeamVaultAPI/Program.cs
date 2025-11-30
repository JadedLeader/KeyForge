
using gRPCIntercommunicationService;
using KeyForgedShared.Helpers;
using KeyForgedShared.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using TeamVaultAPI.BackgroundConsumers;
using TeamVaultAPI.DataContext;
using TeamVaultAPI.Interfaces.Repos;
using TeamVaultAPI.Interfaces.Services;
using TeamVaultAPI.Repos;
using TeamVaultAPI.Services;
using TeamVaultAPI.Storage;

namespace TeamVaultAPI
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

            builder.Services.AddDbContext<TeamVaultDataContext>(options =>
            {
                string? connectionString = builder.Configuration["ConnectionStrings:TeamVaultAPIConnection"];

                options.UseSqlServer(connectionString);
            });

            builder.Services.AddScoped<IAccountRepo, AccountRepo>();
            builder.Services.AddScoped<ITeamRepo, TeamRepo>();
            builder.Services.AddScoped<ITeamVaultRepo, TeamVaultRepo>();
            builder.Services.AddScoped<ITeamVaultService, TeamVaultService>();
            builder.Services.AddScoped<IJwtHelper, JwtHelper>();

            builder.Services.AddHostedService<CreateAccount>(); 
            builder.Services.AddHostedService<DeleteAccount>();
            builder.Services.AddHostedService<CreateTeam>();

            builder.Services.AddSingleton<StreamingStorage>();
            builder.Services.AddScoped<StreamingTeamVaultService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapGrpcService<StreamingTeamVaultService>();

            app.MapControllers();

            app.Run();
        }
    }
}
