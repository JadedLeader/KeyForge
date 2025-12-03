
using KeyForgedShared.Helpers;
using KeyForgedShared.Interfaces;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TeamAPI.BackgroundConsumers;
using TeamAPI.DataContext;
using TeamAPI.Interfaces.Repos;
using TeamAPI.Interfaces.Services;
using TeamAPI.Repos;
using TeamAPI.Services;
using TeamAPI.Storage;

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
            builder.Services.AddGrpc();

            builder.Services.AddGrpcClient<gRPCIntercommunicationService.Account.AccountClient>(options =>
            {
                options.Address = new Uri("https://localhost:7003");
            })
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) =>
        {
            if (sslPolicyErrors != System.Net.Security.SslPolicyErrors.None)
            {
                Log.Information($"=== SSL Certificate Error ===");
                Log.Information($"Errors: {sslPolicyErrors}");
                Log.Information($"Certificate Subject: {cert?.Subject}");
                Log.Information($"Certificate Issuer: {cert?.Issuer}");
                if (chain != null)
                {
                    foreach (var status in chain.ChainStatus)
                    {
                        Log.Information($"Chain Status: {status.Status} - {status.StatusInformation}");
                    }
                }
            }
            return sslPolicyErrors == System.Net.Security.SslPolicyErrors.None;
        }
    };
    return handler;
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

            builder.Services.AddSingleton<StreamingStorage>();
            builder.Services.AddScoped<ITeamService, TeamService>();
            builder.Services.AddScoped<IStreamingService, StreamingService>();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapGrpcService<StreamingService>();

            app.MapControllers();

            app.Run();
        }
    }
}
