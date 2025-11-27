using AuthAPI.Interfaces.ServicesInterface;
using gRPCIntercommunicationService.Protos;
using KeyForgedShared.DTO_s.AuthDTO_s;
using KeyForgedShared.ReturnTypes.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace AuthAPI.Controllers
{

    [ApiController]
    [Route("[Controller]/[action]")]
    public class AuthController : ControllerBase
    {

        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

      
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateInitialKey([FromBody] CreateAuthDto createAuthAccountRequest)
        {
            CreateAuthReturn? creatingInitialAuth = await _authService.CreateAuthAccount(createAuthAccountRequest);

            Response.Cookies.Append("LongLivedToken", creatingInitialAuth.LongLivedToken, 
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTimeOffset.UtcNow.AddDays(30)
                }
            );

            if(!creatingInitialAuth.Success)
            {
                Log.Error($"{this.GetType().Namespace} An error occurred when creating an inital auth key");
                return BadRequest(creatingInitialAuth);
            }

            return Ok(creatingInitialAuth);
        }

        [HttpPut]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateShortLivedKey([FromBody] RefreshShortLivedTokenDto refreshShortLivedTokenRequest)
        {
            RefreshShortLivedTokenReturn refreshedShortLivedToken = await _authService.RefreshShortLivedToken(refreshShortLivedTokenRequest);

            if(!refreshedShortLivedToken.Success)
            {
                Log.Error($"{this.GetType().Namespace} An error ocurred while refreshing the short lived key");
                return BadRequest(refreshedShortLivedToken);
            }

            return Ok(refreshedShortLivedToken);
        }

        [HttpPut]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateLongLivedKey([FromBody] RefreshLongLivedTokenDto refreshLongLivedTokenRequest)
        {
            RefreshLongLivedTokenReturn refreshedLongLivedToken = await _authService.RefreshLongLivedToken(refreshLongLivedTokenRequest);

            if(!refreshedLongLivedToken.Success)
            {

                Log.Error($"{this.GetType().Namespace} An error occurred while updating the long lived key");
                return BadRequest(refreshedLongLivedToken);
            }

            return Ok(refreshedLongLivedToken);
        }

        [HttpPut]
        
        public async Task<IActionResult> RevokeLongLivedKey([FromBody] RevokeLongLivedTokenDto revokeLongLivedTokenRequest)
        {
            RevokeLongLivedTokenReturn revokedTokens = await _authService.RevokeLongLivedToken(revokeLongLivedTokenRequest);

            if(!revokedTokens.Success)
            {
                Log.Error($"{this.GetType().Namespace} An error ocurred when revoking account keys, this is due to an auth not existing");

                return BadRequest(revokedTokens);
            }

            return Ok(revokedTokens);
        }

        [HttpPost] 
        public async Task<IActionResult> UserLogin([FromBody] LoginDto loginRequest)
        {
            LoginReturn newLogin = await _authService.Login(loginRequest);

            if(!newLogin.Success)
            {
                Log.Error($"{this.GetType().Namespace} An error occurred when logging in to the account");

                return BadRequest(newLogin); 

            }

            return Ok(newLogin);


        }

        [HttpPut]
        public async Task<IActionResult> ReinstateAuthKeys([FromBody] ReinstateAuthKeyDto reinstateAuthKeyRequest)
        {
            ReinstateAuthKeyReturn reinstateAuthKey = await _authService.ReinstantiateAuthKey(reinstateAuthKeyRequest);

            if(!reinstateAuthKey.Success)
            {

                Log.Error($"{this.GetType().Namespace} An error occurred when trying to reinstantiate the keys");
                return BadRequest(reinstateAuthKey);
            }

            return Ok(reinstateAuthKey);
        }


        [HttpGet]
        public async Task<IActionResult> SilentShortLivedTokenRefresh()
        {
            if(Request.Cookies.TryGetValue("LongLivedToken", out string? cookieValue))
            {
                SilentShortLivedTokenRefreshReturn silentRefresh = await _authService.SilentTokenCycle(cookieValue);

                if(!silentRefresh.Success)
                {
                    return BadRequest(silentRefresh);
                }

                Response.Cookies.Append("ShortLivedToken", silentRefresh.RefreshedShortLivedToken);

                return Ok(silentRefresh);
            }

            return Unauthorized();
             
        }


    }
}
