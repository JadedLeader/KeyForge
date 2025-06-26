
using AuthAPI.BackgroundConsumer;
using AuthAPI.DataContext;
using AuthAPI.Interfaces.RepoInterface;
using AuthAPI.Repos;
using AuthAPI.Services;
using gRPCIntercommunicationService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using AuthAPI.Interfaces.ServicesInterface;
using AuthAPI.TransporationStorage;

namespace AuthAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddGrpc();

            builder.Services.AddScoped<IAuthRepo, AuthRepo>();
            builder.Services.AddScoped<ITokenGeneratorService, TokenGeneratorService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IAuthTransportationService, AuthTransporationService>();

            builder.Services.AddSingleton<AuthTransportationStorage>();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            builder.Services.AddGrpcClient<Account.AccountClient>(options =>
            {
                options.Address = new Uri("https://localhost:7003");
            });

            builder.Services.AddDbContext<AuthDataContext>(options =>
            {

                string? connectionString = builder.Configuration.GetConnectionString("AuthAPIConnection");
                options.UseSqlServer(connectionString);

            });

            var jwtSettings = builder.Configuration.GetSection($"JWT");

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["issuer"],
                    ValidateAudience = true,
                    ValidAudiences = new[]
                    {
                        "https://localhost:7003/*",
                        "https://localhost:7010/*",
                    },
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                };
            });

            builder.Services.AddAuthorization();
               

            builder.Services.AddHostedService<AccountBackgroundConsumer>();
            builder.Services.AddHostedService<DeleteAccountBackgroundConsumer>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.MapGrpcService<AuthService>();
            

            app.Run();
        }
    }
}
