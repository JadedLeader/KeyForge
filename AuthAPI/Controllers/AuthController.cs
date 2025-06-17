using AuthAPI.Interfaces.ServicesInterface;
using gRPCIntercommunicationService.Protos;
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
        public async Task<IActionResult> CreateInitialKey([FromBody] CreateAuthAccountRequest createAuthAccountRequest)
        {
            CreateAuthAccountResponse? creatingInitialAuth = await _authService.CreateAuthAccount(createAuthAccountRequest);

            if(creatingInitialAuth.Successful == false)
            {
                Log.Error($"{this.GetType().Namespace} An error occurred when creating an inital auth key");
                return BadRequest(creatingInitialAuth);
            }

            return Ok(creatingInitialAuth);
        }

        [HttpPut]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateShortLivedKey([FromBody] RefreshShortLivedTokenRequest refreshShortLivedTokenRequest)
        {
            RefreshShortLivedTokenResponse refreshedShortLivedToken = await _authService.RefreshShortLivedToken(refreshShortLivedTokenRequest);

            if(refreshedShortLivedToken.Successful == false)
            {
                Log.Error($"{this.GetType().Namespace} An error ocurred while refreshing the short lived key");
                return BadRequest(refreshedShortLivedToken);
            }

            return Ok(refreshedShortLivedToken);
        }

        [HttpPut]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateLongLivedKey([FromBody] RefreshLongLivedTokenRequest refreshLongLivedTokenRequest)
        {
            RefreshLongLivedTokenResponse refreshedLongLivedToken = await _authService.RefreshLongLivedToken(refreshLongLivedTokenRequest);

            if(refreshedLongLivedToken.Successful == false)
            {

                Log.Error($"{this.GetType().Namespace} An error occurred while updating the long lived key");
                return BadRequest(refreshedLongLivedToken);
            }

            return Ok(refreshedLongLivedToken);
        }

        [HttpPut]
        
        public async Task<IActionResult> RevokeLongLivedKey([FromBody] RevokeLongLivedTokenRequest revokeLongLivedTokenRequest)
        {
            RevokeLongLivedTokenResponse revokedTokens = await _authService.RevokeLongLivedToken(revokeLongLivedTokenRequest);

            if(revokedTokens.Successful == false)
            {
                Log.Error($"{this.GetType().Namespace} An error ocurred when revoking account keys, this is due to an auth not existing");

                return BadRequest(revokedTokens);
            }

            return Ok(revokedTokens);
        }

        [HttpGet] 
        public async Task<IActionResult> Login()
        {
            throw new NotImplementedException();
        }


    }
}
