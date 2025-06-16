using AuthAPI.Interfaces.ServicesInterface;
using Azure.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;

namespace AuthAPI.Services
{
    public class TokenGeneratorService : ITokenGeneratorService
    {

        private IConfiguration _configuration;

        public TokenGeneratorService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

      
        public string GenerateShortLivedToken(string accountId)
        {
            string token = GenerateJwtToken(accountId, 1);

            return token;
        }

        public string GenerateLongLivedToken(string accountId)
        {
            string longLivedToken = GenerateJwtToken(accountId, 30);

            return longLivedToken;
        }

        private string GenerateJwtToken(string accountId, double daysToAdd)
        {

            string? issuer = _configuration["JWT:Issuer"];
            string? audience = _configuration["JWT:Audience"];
            string? key = _configuration["JWT:Key"];

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.NameId, accountId)
                }),
                Expires = DateTime.Now.AddDays(daysToAdd), 
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256),
               
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);

            string accessToken = tokenHandler.WriteToken(securityToken);

            return accessToken;

        }

       

    }
}
