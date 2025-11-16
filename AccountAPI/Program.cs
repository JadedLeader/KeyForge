
using AccountAPI.DataContext;
using AccountAPI.Interfaces.RepoInterface;
using AccountAPI.Interfaces.ServiceInterface;
using AccountAPI.Repos;
using AccountAPI.Services;
using AccountAPI.Storage;
using gRPCIntercommunicationService;
using KeyForgedShared.Helpers;
using KeyForgedShared.Interfaces;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Threading.Channels;

namespace AccountAPI.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddGrpc();

            builder.Services.AddScoped<IAccountRepo, AccountRepo>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddSingleton<StreamStorage>();
            builder.Services.AddScoped<IJwtHelper, JwtHelper>();

            builder.Services.AddSingleton(Channel.CreateUnbounded<StreamAccountResponse>());

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();
            

            builder.Services.AddDbContext<AccountDataContext>(options =>
            {
                string? connection = builder.Configuration.GetConnectionString("AccountAPIConnection");
                options.UseSqlServer(connection);
            });

            var app = builder.Build();

            app.UseDefaultFiles();
            app.UseStaticFiles();

          
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapGrpcService<AccountService>();

            app.MapControllers();

            app.Run();
        }
    }
}
