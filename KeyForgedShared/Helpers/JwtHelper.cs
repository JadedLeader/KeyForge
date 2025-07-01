using KeyForgedShared.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyForgedShared.Helpers
{
    public class JwtHelper : IJwtHelper
    {

        public bool IsLongLivedKeyValid(string currentLongLivedKey)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            if (currentLongLivedKey == string.Empty)
            {
                return false;
            }

            var longLivedKey = handler.ReadToken(currentLongLivedKey);

            DateTime validToDate = longLivedKey.ValidTo;

            if (validToDate < DateTime.Now)
            {
                return false;
            }

            return true;
        }

        public string ReturnRoleFromToken(string token)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            if (token == string.Empty)
            {
                return "";
            }

            JwtSecurityToken jwt = handler.ReadJwtToken(token);

            string? roleClaim = jwt.Claims.FirstOrDefault(c => c.Type == "Role").Value;

            if (roleClaim == string.Empty)
            {
                Log.Warning($"No role claim could be found for claim 'Role' ");
            }

            return roleClaim;

        }

        public string ReturnAccountIdFromToken(string token)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            if (token == string.Empty)
            {
                return "";
            }

            JwtSecurityToken jwt = handler.ReadJwtToken(token);

            string? accountIdClaim = jwt.Claims.FirstOrDefault(ac => ac.Type == "nameid").Value;

            if (accountIdClaim == string.Empty)
            {
                Log.Warning($"No account ID claim can be found within the token");
            }

            return accountIdClaim;

        }

    }
}
