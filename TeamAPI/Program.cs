
using KeyForgedShared.Helpers;
using KeyForgedShared.Interfaces;
using Microsoft.EntityFrameworkCore;
using TeamAPI.BackgroundConsumers;
using TeamAPI.DataContext;
using TeamAPI.Interfaces.Repos;
using TeamAPI.Interfaces.Services;
using TeamAPI.Repos;
using TeamAPI.Services;

namespace TeamAPI
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

            builder.Services.AddGrpcClient<gRPCIntercommunicationService.Account.AccountClient>(options =>
            {
                options.Address = new Uri("https://localhost:7003");
            });

            builder.Services.AddDbContext<TeamApiDataContext>(options =>
            {

                string? connectionString = builder.Configuration["ConnectionStrings:TeamApiConnection"];

                if (connectionString == null)
                {
                    throw new Exception($"Team API connection string not allocated");
                }

                options.UseSqlServer(connectionString);
            });

            builder.Services.AddScoped<IAccountRepo, AccountRepo>();
            builder.Services.AddScoped<ITeamRepo, TeamRepo>();
            builder.Services.AddScoped<IJwtHelper, JwtHelper>();

            builder.Services.AddScoped<ITeamService, TeamService>();

            builder.Services.AddHostedService<AddAccount>(); 
            builder.Services.AddHostedService<DeleteAccount>();

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
