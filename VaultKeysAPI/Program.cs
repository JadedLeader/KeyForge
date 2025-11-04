using gRPCIntercommunicationService.Protos;
using KeyForgedShared.Generics;
using KeyForgedShared.SharedDataModels; 
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using VaultKeysAPI.BackgroundConsumers;
using VaultKeysAPI.DataContext;
using VaultKeysAPI.Interfaces;
using VaultKeysAPI.Repos;
using VaultKeysAPI.Services;

namespace VaultKeysAPI
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

            builder.Services.AddGrpcClient<Auth.AuthClient>(options =>
            {
                options.Address = new Uri("https://localhost:7010");
            });

            builder.Services.AddGrpcClient<Vault.VaultClient>(options =>
            {
                options.Address = new Uri("https://localhost:7149");
            });

            builder.Services.AddDbContext<VaultKeysDataContext>(options =>
            {
                string? connectionString = builder.Configuration.GetConnectionString("VaultKeysApiConnectionString");

                options.UseSqlServer(connectionString);

            });

            

            builder.Services.AddHostedService<AddAccountBackgroundConsumer>();
            builder.Services.AddHostedService<AddAuthBackgroundConsumer>();
            builder.Services.AddHostedService<AddVaultBackgroundConsumer>();
            builder.Services.AddHostedService<DeleteAccountBackgroundConsumer>();
            builder.Services.AddHostedService<UpdateAuthBackgroundConsumer>(); 


            builder.Services.AddScoped<IAccountRepo, AccountRepo>();
            builder.Services.AddScoped<IAuthRepo, AuthRepo>();
            builder.Services.AddScoped<IVaultRepo, VaultRepo>();
            builder.Services.AddScoped<IVaultKeysRepo, VaultKeysRepo>();
            builder.Services.AddScoped<VaultKeysService>();


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
