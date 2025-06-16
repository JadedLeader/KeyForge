using AuthAPI.Interfaces.ServicesInterface;
using gRPCIntercommunicationService.Protos;
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
        public async Task<IActionResult> UpdateShortLivedKey()
        {
            throw new NotImplementedException();
        }

        [HttpPut]
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

        [HttpDelete]
        public async Task<IActionResult> RevokeLongLivedKey()
        {
            throw new NotImplementedException();
        }


    }
}
