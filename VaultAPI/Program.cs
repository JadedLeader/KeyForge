
using VaultAPI.DataModel;
using gRPCIntercommunicationService;
using gRPCIntercommunicationService.Protos;
using Microsoft.EntityFrameworkCore;
using VaultAPI.BackgroundConsumers;
using VaultAPI.DataContext;
using VaultAPI.Repos;
using KeyForgedShared.Generics;
using VaultAPI.Interfaces.RepoInterfaces;
using KeyForgedShared.Interfaces;
using KeyForgedShared.Helpers;
using VaultAPI.Interfaces.ServiceInterfaces;
using VaultAPI.Services;

namespace VaultAPI
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

            builder.Services.AddHostedService<AddAccountBackgroundConsumer>();
            builder.Services.AddHostedService<DeleteAccountBackgroundConsumer>();
            builder.Services.AddHostedService<AddAuthBackgroundConsumer>();
            builder.Services.AddHostedService<UpdateAuthBackgroundConsumer>();

            builder.Services.AddScoped<GenericRepository<AccountDataModel>>();
            builder.Services.AddScoped<GenericRepository<AuthDataModel>>();
            builder.Services.AddScoped<GenericRepository<VaultDataModel>>();

            builder.Services.AddScoped<IAccountRepo, AccountRepo>();
            builder.Services.AddScoped<IAuthRepo, AuthRepo>();
            builder.Services.AddScoped<IVaultRepo, VaultRepo>();

            builder.Services.AddScoped<IJwtHelper, JwtHelper>();
            builder.Services.AddScoped<IVaultService, VaultService>();

            builder.Services.AddGrpcClient<gRPCIntercommunicationService.Account.AccountClient>(options =>
            {
                options.Address = new Uri("https://localhost:7003");
            });

            builder.Services.AddGrpcClient<Auth.AuthClient>(options =>
            {
                options.Address = new Uri("https://localhost:7010");
            });

            builder.Services.AddDbContext<VaultDataContext>(options =>
            {
                string? vaultConnectionString = builder.Configuration.GetConnectionString("VaultApiConnectionString");
                options.UseSqlServer(vaultConnectionString);
            });

            //this line helps inject the dependency of the DbContext for the GenericRepository located in KeyForgedShared
            builder.Services.AddScoped<DbContext>(sp => sp.GetRequiredService<VaultDataContext>());

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
